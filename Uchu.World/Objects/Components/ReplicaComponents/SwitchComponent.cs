using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet.IO;
using Uchu.Physics;

namespace Uchu.World
{
    public class SwitchComponent : StructReplicaComponent<SwitchSerialize>
    {
        public bool State { get; set; }

        public bool IsHitSwitch { get; set; }
        
        public bool IsPressureSwitch { get; set; }
        
        public bool IsTimedSwitch { get; set; }
        
        public int SwitchResetTime { get; set; }

        public uint SwitchMaxUsers { get; set; } = 20;

        public uint SwitchUserRequirement { get; set; } = 1;
        
        public Player Activator { get; set; }

        public Event<Player> OnActivated { get; }
        
        public Event OnDeactivated { get; }

        /// <summary>
        /// Players that are currently on the switch.
        /// </summary>
        private List<Player> _activePlayers = new List<Player>();

        /// <summary>
        /// Timer for deactivating the switch.
        /// </summary>
        private Timer _switchDeactivateTimer = new Timer()
        {
            AutoReset = false,
        };

        public override ComponentId Id => ComponentId.SwitchComponent;

        /// <summary>
        /// Creates the switch component.
        /// </summary>
        protected SwitchComponent()
        {
            OnActivated = new Event<Player>();
            OnDeactivated = new Event();

            // Listen to the component starting.
            Listen(OnStart, () =>
            {
                // Set the values.
                if (GameObject.Settings.TryGetValue("is_hit_switch", out var isHit))
                {
                    IsHitSwitch = (bool) isHit;
                }
                if (GameObject.Settings.TryGetValue("is_pressure_switch", out var isPressure))
                {
                    IsPressureSwitch = (bool) isPressure;
                }
                if (GameObject.Settings.TryGetValue("is_timed_switch", out var isTimed))
                {
                    IsTimedSwitch = (bool) isTimed;
                }
                if (GameObject.Settings.TryGetValue("switch_reset_time", out var resetTime))
                {
                    SwitchResetTime = (int) resetTime;
                    this._switchDeactivateTimer.Interval = SwitchResetTime * 1000;
                }
                if (GameObject.Settings.TryGetValue("switch_users_max", out var maxUsers))
                {
                    SwitchMaxUsers = (uint) maxUsers;
                }
                if (GameObject.Settings.TryGetValue("switch_users_required", out var requiredUsers))
                {
                    SwitchUserRequirement = (uint) requiredUsers;
                }

                // Add the physics for the switch.
                var physics = this.GameObject.AddComponent<PhysicsComponent>();
                var size = Vector3.One;
                var physicsObject = BoxBody.Create(
                    this.GameObject.Zone.Simulation,
                    this.GameObject.Transform.Position,
                    this.GameObject.Transform.Rotation,
                    size
                );
                physics.SetPhysics(physicsObject);

                // Listen to the timer running out.
                this._switchDeactivateTimer.Elapsed += async (sender, args) =>
                {
                    await Deactivate();
                };
                
                // Listen to players entering and leaving the switch.
                this.GameObject.TryGetComponent<QuickBuildComponent>(out var buttonQuickBuild);
                Listen(physics.OnEnter, other =>
                {
                    // Return if the object isn't a player or the switch isn't build.
                    if (!(other.GameObject is Player player)) return;
                    if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return;
                    
                    // Activate the switch.
                    if (this._activePlayers.Contains(player)) return;
                    this._switchDeactivateTimer.Stop();
                    this._activePlayers.Add(player);
                    Task.Run(async () =>
                    {
                        await Activate(player);
                    });
                });
                Listen(physics.OnLeave, other =>
                {
                    // Return if the object isn't a player.
                    if (!(other.GameObject is Player player)) return;
                    if (!this._activePlayers.Contains(player)) return;
                    
                    // Start the timer if the last player left.
                    this._activePlayers.Remove(player);
                    if (this._activePlayers.Count != 0) return;
                    this._switchDeactivateTimer.Start();
                });
                
                // Listen to the switch being interacted.
                Listen(this.GameObject.OnInteract, player =>
                {
                    if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return;
                    Task.Run(async () =>
                    {
                        this._switchDeactivateTimer.Stop();
                        this._switchDeactivateTimer.Start();
                        await Activate(player);
                    });
                });
                
                // Listen to the quickbuld state changing.
                if (buttonQuickBuild == default) return;
                Listen(buttonQuickBuild.OnStateChange, state =>
                {
                    // Start the timer if it isn't active and there are players on the switch.
                    if (state == RebuildState.Completed) return;
                    if (this._switchDeactivateTimer.Enabled) return;
                    if (this._activePlayers.Count == 0) return;
                    this._activePlayers.Clear();
                    this._switchDeactivateTimer.Start();
                });
            });
        }

        /// <summary>
        /// Activate this switch.
        /// </summary>
        /// <param name="player">The player that activates this switch.</param>
        public async Task Activate(Player player)
        {
            // Activate the switch.
            if (State) return;
            Activator = player;
            State = true;
            GameObject.Serialize(GameObject);
            await OnActivated.InvokeAsync(player);
        }

        /// <summary>
        /// Deactivate this switch.
        /// </summary>
        public async Task Deactivate()
        {
            if (!State) return;
            State = false;
            GameObject.Serialize(GameObject);
            await OnDeactivated.InvokeAsync();
        }
    }
}