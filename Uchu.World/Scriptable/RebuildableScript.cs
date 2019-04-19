using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Core;
using Uchu.Core.Packets.Server.GameMessages;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    /// <inheritdoc />
    /// <summary>
    ///     Script for EVERY rebuildable object.
    /// </summary>
    [AutoAssign(typeof(RebuildComponent))]
    public class RebuildableScript : GameScript
    {
        /*
         * Rebuildables have five states.
         * 
         * Open: The Quickbuild is available and ready to be built.
         * Complete: The Quickbuild can not be built, does not mean it can not be used.
         * Resetting: This has to be sent to the client once, but does not have to be on the object for any amount of time.
         * Building: The Quickbuild is being built.
         * Incomplete: The Quickbuild is not complete but can be restarted.
         *
         * Open -> Building     ->     Complete -> Resetting -> Open
         *                \\          /           /
         *                  Incomplete     ->    /
         * 
         * All of the changes in the state of the quickbuild has to be notified to the player building and updated
         * in the world.
         *
         * NOTE: Rebuildables in AG are weird.
         */
        
        /// <summary>
        ///     The cost of the Quickbuild.
        ///     TODO: Look into another way of getting this value.
        /// </summary>
        private uint _cost;

        /// <summary>
        ///     The amount of imagination taken from the player building
        /// </summary>
        private uint _taken;

        /// <summary>
        ///     Timer to determine when the quickbuild is done.
        /// </summary>
        private PausableTimer _timer;

        /// <summary>
        ///     Inherited Constructor
        /// </summary>
        /// <param name="world"></param>
        /// <param name="replicaPacket"></param>
        public RebuildableScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        /// <summary>
        ///     Called once at the start of world load.
        /// </summary>
        public override void Start()
        {
            _cost = (uint) Math.Floor((float) ReplicaPacket.Settings["compTime"]);
        }

        /// <summary>
        ///     Called when player requests to use quickbuild.
        /// </summary>
        /// <param name="player"></param>
        public override void OnUse(Player player)
        {
            // Player's stats.
            var stats = (StatsComponent) World.GetObject(player.CharacterId).Components.First(c => c is StatsComponent);

            // If the player has no imagination left, don't start the quickbuild.
            if (stats.CurrentImagination == 0) return;

            // RebuildComponent on this Quickbuild.
            var comp = (RebuildComponent) ReplicaPacket.Components.First(c => c is RebuildComponent);

            // This should never happen, but just in case.
            if (comp.State != RebuildState.Open && comp.State != RebuildState.Incomplete) return;

            // Experimental, make the player face the object that they are building.
            Server.Send(new OrientToPositionMessage
            {
                ObjectId = player.CharacterId,
                Position = comp.ActivatorPosition
            }, player.EndPoint);

            // Notify the player that the quickbuild is now being built.
            Server.Send(new RebuildNotifyStateMessage
            {
                ObjectId = ObjectID,
                PreviousState = comp.State,
                NewState = RebuildState.Building,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            // Makes the player start the building of this Quickbuild.
            Server.Send(new EnableRebuildMessage
            {
                ObjectId = ObjectID,
                Enable = true,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            /*
             * Update the object in the World.
             */
            comp.State = RebuildState.Building;
            comp.Enabled = true;
            comp.Success = false;
            comp.Players = new[] {(ulong) player.CharacterId};

            World.UpdateObject(ReplicaPacket);

            // Determine the completion time for this Quickbuild.
            var completeTime = (float) ReplicaPacket.Settings["compTime"];

            /*
             * Set the timer.
             * TODO: Make sure imagination gets taken even though building starts when it is incomplete.
             */
            if (_timer == null)
            {
                _timer = new PausableTimer(completeTime * 1000);

                _timer.Elapsed += (sender, args) => { CompleteBuild(player); };

                var imgTimer = new Timer
                {
                    AutoReset = true,
                    Interval = 1000
                };

                imgTimer.Elapsed += (sender, args) =>
                {
                    using (var ctx = new UchuContext())
                    {
                        var character = ctx.Characters.First(c => c.CharacterId == player.CharacterId);

                        // Check if the player is out of imagination.
                        if (character.CurrentImagination == 0)
                        {
                            imgTimer.Stop();
                            imgTimer.Dispose();
                            StopRebuild(player, RebuildFailReason.OutOfImagination);
                        }

                        // Take one imagination.
                        if (_taken != _cost - 1)
                        {
                            _taken++;
                            character.CurrentImagination--;
                        }

                        // If this Quickbuild is canceled.
                        if (comp.State != RebuildState.Building)
                        {
                            imgTimer.Stop();
                            imgTimer.Dispose();
                        }

                        ctx.SaveChanges();
                    }

                    // Keep the player stats up to date.
                    player.UpdateStats();
                };

                Task.Run(() => { _timer.Start(); });
                Task.Run(() => { imgTimer.Start(); });
            }
            else
            {
                _timer.Resume();
            }
        }

        /// <summary>
        ///     Called when to player requests to stop the building.
        /// </summary>
        /// <param name="player"></param>
        public override void OnRebuildCanceled(Player player)
        {
            StopRebuild(player);
        }

        /// <summary>
        ///     Stop the quickbuild and make it Incomplete.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reason">The Reason why the quickbuild was canceled.</param>
        private void StopRebuild(Player player, RebuildFailReason reason = RebuildFailReason.Canceled)
        {
            // Rebuild Component
            var comp = (RebuildComponent) ReplicaPacket.Components.First(c => c is RebuildComponent);

            if (comp.State != RebuildState.Building) return;

            _timer.Pause();

            // Notify the player the quickbuild is incomplete.
            Server.Send(new RebuildNotifyStateMessage
            {
                ObjectId = ObjectID,
                PreviousState = comp.State,
                NewState = RebuildState.Incomplete,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            // Stops the player from building.
            Server.Send(new EnableRebuildMessage
            {
                ObjectId = ObjectID,
                IsFail = true,
                FailReason = reason,
                PlayerObjectId = player.CharacterId,
                Enable = false
            }, player.EndPoint);

            /*
             * Update the object in the World.
             */
            comp.State = RebuildState.Incomplete;
            comp.Enabled = true;
            comp.Success = false;
            comp.PausedTime = (float) (_timer.Time / 1000);
            comp.TimeSinceStart = (float) (_timer.Time / 1000);
            comp.Players = new ulong[0];

            World.UpdateObject(ReplicaPacket);

            // Determine the completion time for this Quickbuild.
            var completeTime = (float) ReplicaPacket.Settings["compTime"];

            /*
             * Reset this quickbuild after three times its completion time.
             * TODO: Change this to something more correct.
             */
            var timer = new Timer
            {
                AutoReset = false,
                Interval = completeTime * 1000
            };

            timer.Elapsed += (sender, args) =>
            {
                if (comp.State == RebuildState.Incomplete)
                    ResetBuild(player);
            };

            Task.Run(() => { timer.Start(); });
        }

        /// <summary>
        ///     Complete the Quickbuild.
        /// </summary>
        /// <param name="player"></param>
        public async void CompleteBuild(Player player)
        {
            /*
             * Stop The timer.
             */
            _timer.Dispose();
            _timer = null;

            // RebuildComponent on this Quickbuild.
            var comp = (RebuildComponent) ReplicaPacket.Components.First(c => c is RebuildComponent);

            // Notify the player this Quickbuild in now complete.
            Server.Send(new RebuildNotifyStateMessage
            {
                ObjectId = ObjectID,
                PreviousState = comp.State,
                NewState = RebuildState.Completed,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            // Makes the player complete this Quickbuild.
            Server.Send(new EnableRebuildMessage
            {
                ObjectId = ObjectID,
                IsSuccess = true,
                PlayerObjectId = player.CharacterId,
                Enable = false
            }, player.EndPoint);

            player.UpdateStats();

            /*
             * Update the object in the World.
             */
            comp.State = RebuildState.Completed;
            comp.Success = true;
            comp.Enabled = true;
            comp.Players = new[] {(ulong) player.CharacterId};

            World.UpdateObject(ReplicaPacket);

            // Update any mission task that required this quickbuild.
            await player.UpdateTaskAsync(LOT, MissionTaskType.QuickBuild);

            // Determine the completion time for this Quickbuild.
            var completeTime = (float) ReplicaPacket.Settings["compTime"];

            /*
             * Reset this quickbuild after three times its completion time.
             * TODO: Change this to something more correct.
             */
            var timer = new Timer
            {
                AutoReset = false,
                Interval = completeTime * 1000
            };

            timer.Elapsed += (sender, args) => { ResetBuild(player); };

            await Task.Run(() => { timer.Start(); });
        }

        /// <summary>
        ///     Reset the Quickbuild
        /// </summary>
        /// <param name="player"></param>
        public void ResetBuild(Player player)
        {
            // Stop the timer if there is one active.
            _taken = 0;
            _timer?.Stop();

            // RebuildComponent on this Quickbuild.
            var comp = (RebuildComponent) ReplicaPacket.Components.First(c => c is RebuildComponent);

            /*
             * The client has to be sent a message telling them this quickbuild is resetting, but this does/should not
             * last. The client will receive a different notice imminently.
             */

            // Notify the client this quickbuild is resetting.
            Server.Send(new RebuildNotifyStateMessage
            {
                ObjectId = ObjectID,
                PreviousState = comp.State,
                NewState = RebuildState.Resetting,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            player.UpdateStats();

            /*
             * Update the object in the World.
             */
            comp.Players = new ulong[0];
            comp.Success = false;
            comp.Enabled = true;
            comp.PausedTime = 0;
            comp.TimeSinceStart = 0;
            comp.State = RebuildState.Resetting;

            World.UpdateObject(ReplicaPacket);

            // Open up the quickbuild for use.
            OpenBuild(player);
        }

        /// <summary>
        ///     Open up the quickbuild for use.
        /// </summary>
        /// <param name="player"></param>
        public void OpenBuild(Player player)
        {
            // Remove the timer.
            _taken = 0;
            _timer = null;

            // RebuildComponent on this Quickbuild.
            var comp = (RebuildComponent) ReplicaPacket.Components.First(c => c is RebuildComponent);

            // Notify the client this Quickbuild is not open.
            Server.Send(new RebuildNotifyStateMessage
            {
                ObjectId = ObjectID,
                PreviousState = comp.State,
                NewState = RebuildState.Open,
                PlayerObjectId = player.CharacterId
            }, player.EndPoint);

            player.UpdateStats();

            /*
             * Update the object in the World.
             */
            comp.Players = new ulong[] { };
            comp.Enabled = true;
            comp.Success = false;
            comp.TimeSinceStart = 0;
            comp.PausedTime = 0;
            comp.State = RebuildState.Open;

            World.UpdateObject(ReplicaPacket);
        }
    }
}
