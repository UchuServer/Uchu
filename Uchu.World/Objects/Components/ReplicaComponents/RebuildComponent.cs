using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [RequireComponent(typeof(StatsComponent), true)]
    public class RebuildComponent : ScriptedActivityComponent
    {
        private float _completeTime;
        private int _imaginationCost;
        private float _timeToSmash;
        private float _resetTime;

        private PauseTimer _timer;
        private Timer _imaginationTimer;
        private int _taken;

        public RebuildState State { get; set; } = RebuildState.Open;

        public bool Success { get; set; }

        public bool Enabled { get; set; } = true;

        public float TimeSinceStart { get; set; }

        public float PauseTime { get; set; }

        public Vector3 ActivatorPosition { get; set; }
        
        public GameObject Activator { get; set; }

        public override ComponentId Id => ComponentId.QuickBuildComponent;

        protected RebuildComponent()
        {
            OnStart.AddListener(async () =>
            {
                if (GameObject.Settings.TryGetValue("rebuild_activators", out var rebuildActivators))
                {
                    ActivatorPosition = (Vector3) rebuildActivators;
                }
                else
                {
                    ActivatorPosition = Transform.Position;
                }
                
                Logger.Information($"{GameObject} is a rebuild-able!");

                await using var cdClient = new CdClientContext();

                var clientComponent = await cdClient.RebuildComponentTable.FirstOrDefaultAsync(
                    r => r.Id == GameObject.Lot.GetComponentId(ComponentId.QuickBuildComponent)
                );

                if (clientComponent == default)
                {
                    Logger.Error(
                        $"{GameObject} does not have a valid {nameof(ComponentId.QuickBuildComponent)} component."
                    );
                    
                    return;
                }

                _completeTime = clientComponent.Completetime ?? 0;
                _imaginationCost = clientComponent.Takeimagination ?? 0;
                _timeToSmash = clientComponent.Timebeforesmash ?? 0;
                _resetTime = clientComponent.Resettime ?? 0;

                if (!GameObject.Settings.TryGetValue("spawnActivator", out var spawnActivator) ||
                    !(bool) spawnActivator) return;
                
                //
                // This activator is that imagination cost display of the quickbuild.
                // It is required to be able to start the quickbuild.
                //
                
                Activator = GameObject.Instantiate(new LevelObject
                {
                    Lot = 6604,
                    Position = (Vector3) GameObject.Settings["rebuild_activators"],
                    Rotation = Quaternion.Identity,
                    Scale = -1,
                    Settings = new LegoDataDictionary()
                }, GameObject);

                Start(Activator);
                GameObject.Construct(Activator);
                
                // Delay to attempt to prevent client from "panic"
                await Task.Delay(30);
                
                GameObject.Serialize(GameObject);

                GameObject.OnInteract.AddListener(StartRebuild);
            });
        }
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);

            writer.WriteBit(false);

            writer.Write(ActivatorPosition);

            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);

            writer.WriteBit(true);

            writer.Write((uint) State);

            writer.WriteBit(Success);
            writer.WriteBit(Enabled);

            writer.Write(TimeSinceStart);
            writer.Write(PauseTime);
        }
        
        //
        // Rebuildables have five states.
        // 
        // Open: The Quickbuild is available and ready to be built.
        // Complete: The Quickbuild can not be built, does not mean it can not be used.
        // Resetting: This has to be sent to the client once, but does not have to be on the object for any amount of time.
        // Building: The Quickbuild is being built.
        // Incomplete: The Quickbuild is not complete but can be restarted.
        //
        // Open -> Building     ->     Complete -> Resetting -> Open
        //                \\          /           /
        //                  Incomplete     ->    /
        // 
        // All of the changes in the state of the quickbuild has to be notified to the player building and updated
        // in the world.
        //
        // NOTE: Rebuildables in AG are weird.
        //

        public void StartRebuild(Player player)
        {
            if (State != RebuildState.Open && State != RebuildState.Incomplete) return;
            
            var playerStats = player.GetComponent<Stats>();
            
            if (playerStats.Imagination == default) return;
            
            Zone.BroadcastMessage(new RebuildNotifyStateMessage
            {
                Associate = GameObject,
                CurrentState = State,
                NewState = RebuildState.Building,
                Player = player
            });
            
            Zone.BroadcastMessage(new EnableRebuildMessage
            {
                Associate = GameObject,
                Enable = true,
                Player = player
            });

            State = RebuildState.Building;
            Enabled = true;
            Success = false;

            if (!Participants.Contains(player)) Participants.Add(player);

            GameObject.Serialize(GameObject);

            if (_timer == default)
            {
                _timer = new PauseTimer(_completeTime * 1000);

                _timer.Elapsed += (sender, args) => {  };
            }
            else
            {
                _timer.Resume();
            }

            _imaginationTimer = new Timer
            {
                AutoReset = true,
                Interval = 1000
            };

            _imaginationTimer.Elapsed += (sender, args) =>
            {
                if (playerStats.Imagination == default)
                {
                    _imaginationTimer.Stop();
                    _imaginationTimer.Dispose();

                    StopRebuild(player, RebuildFailReason.OutOfImagination);
                    
                    return;
                }

                if (_taken != _imaginationCost)
                {
                    _taken++;

                    playerStats.Imagination--;

                    if (_taken == _imaginationCost)
                    {
                        _imaginationTimer.Stop();
                        _imaginationTimer.Dispose();
                        
                        CompleteBuild(player);
                    }
                }

                if (State == RebuildState.Building) return;
                
                _imaginationTimer.Stop();
                _imaginationTimer.Dispose();
            };

            Task.Run(_timer.Start);
            Task.Run(_imaginationTimer.Start);
        }

        public void StopRebuild(Player player, RebuildFailReason reason)
        {
            if (State != RebuildState.Building) return;
            
            _timer.Pause();
            
            Zone.BroadcastMessage(new RebuildNotifyStateMessage
            {
                Associate = GameObject,
                CurrentState = State,
                NewState = RebuildState.Incomplete,
                Player = player
            });
            
            Zone.BroadcastMessage(new EnableRebuildMessage
            {
                Associate = GameObject,
                IsFail = true,
                FailReason = reason,
                Player = player
            });

            var time = (float) (_timer.Time / 1000);
            
            State = RebuildState.Incomplete;
            Enabled = true;
            Success = false;
            PauseTime = time;
            TimeSinceStart = time;
            
            if (Participants.Contains(player)) Participants.Remove(player);

            GameObject.Serialize(GameObject);
            
            var timer = new Timer
            {
                AutoReset = false,
                Interval = _resetTime * 1000
            };

            timer.Elapsed += (sender, args) =>
            {
                if (State == RebuildState.Incomplete)
                    ResetBuild(player);
            };
        }
        
        public void CompleteBuild(Player player)
        {
            Activator.Layer = Layer.Hidden;

            /*
             * Stop The timer.
             */
            _timer.Dispose();
            _timer = null;

            // Notify the player this Quickbuild in now complete.
            Zone.BroadcastMessage(new RebuildNotifyStateMessage
            {
                Associate = GameObject,
                CurrentState = State,
                NewState = RebuildState.Completed,
                Player = player
            });

            // Makes the player complete this Quickbuild.
            Zone.BroadcastMessage(new EnableRebuildMessage
            {
                Associate = GameObject,
                IsSuccess = true,
                Player = player
            });

            /*
             * Update the object in the World.
             */
            State = RebuildState.Completed;
            Success = true;
            Enabled = true;
            
            if (!Participants.Contains(player)) Participants.Add(player);

            GameObject.Serialize(GameObject);

            // Update any mission task that required this quickbuild.
            Task.Run(async () =>
            {
                await player.GetComponent<QuestInventory>().UpdateObjectTaskAsync(
                    MissionTaskType.QuickBuild,
                    GameObject.Lot,
                    GameObject
                );
            });

            /*
             * Reset this quickbuild after three times its completion time.
             * TODO: Change this to something more correct.
             */
            var timer = new Timer
            {
                AutoReset = false,
                Interval = _resetTime * 1000
            };

            timer.Elapsed += (sender, args) => { ResetBuild(player); };

            Task.Run(() => { timer.Start(); });
        }

        public void ResetBuild(Player player)
        {
            _taken = default;
            _timer?.Stop();
            
            Zone.BroadcastMessage(new RebuildNotifyStateMessage
            {
                Associate = GameObject,
                CurrentState = State,
                NewState = RebuildState.Resetting,
                Player = player
            });
            
            Participants.Clear();
            Success = false;
            Enabled = true;
            PauseTime = default;
            TimeSinceStart = default;
            State = RebuildState.Resetting;

            GameObject.Serialize(GameObject);

            OpenBuild(player);
        }

        public void OpenBuild(Player player)
        {
            Activator.Layer = Layer.Default;
            
            _taken = default;
            _timer = default;
            
            Zone.BroadcastMessage(new RebuildNotifyStateMessage
            {
                Associate = GameObject,
                CurrentState = State,
                NewState = RebuildState.Open,
                Player = player
            });
            
            Participants.Clear();
            Success = false;
            Enabled = true;
            PauseTime = default;
            TimeSinceStart = default;
            State = RebuildState.Open;

            GameObject.Serialize(GameObject);
        }
    }
}