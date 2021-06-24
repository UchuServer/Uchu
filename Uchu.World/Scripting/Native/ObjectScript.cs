using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using Uchu.Physics;
using Uchu.World.Systems.Behaviors;

namespace Uchu.World.Scripting.Native
{
    public enum PhysicsCollisionStatus
    {
        Enter,
        Leave,
    }
    
    public abstract class ObjectScript : NativeScript
    {
        /// <summary>
        /// Object controlled by the script.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// Dictionary of the variables stored by the script.
        /// </summary>
        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        /// <summary>
        /// Timers used by the script.
        /// </summary>
        private Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ObjectScript(GameObject gameObject)
        {
            this.GameObject = gameObject;
            this.SetZone(gameObject.Zone);
            
            // Connect clearing on destroyed.
            Listen(gameObject.OnDestroyed, () =>
            {
                // Stop the timers.
                this.CancelAllTimers();
                
                // Unload the script.
                this.UnloadAsync();
            });
        }

        #region Object
        /// <summary>
        /// Destroys the object.
        /// </summary>
        /// <param name="killerId">Killer of the object.</param>
        public void Die(Player killerId)
        {
            Task.Run(async () =>
            {
                await this.GameObject.GetComponent<DestructibleComponent>().SmashAsync(killerId);
            });
        }
        #endregion

        #region Variable Storage
        /// <summary>
        /// Sets a stored variable of the script.
        /// </summary>
        /// <param name="name">Name of the variable to store.</param>
        /// <param name="value">Value of the variable to store.</param>
        public void SetVar(string name, object value)
        {
            this._variables[name] = value;
        }

        /// <summary>
        /// Gets a stored variable of the script.
        /// </summary>
        /// <param name="name">Name of the variable to fetch.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value of the variable to fetch.</returns>
        public T GetVar<T>(string name)
        {
            return (T) this._variables[name];
        }
        #endregion

        #region Timers
        /// <summary>
        /// Starts a timer to be called back later. The timer
        /// can be cancelled.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="timerName"></param>
        public void AddTimerWithCancel(float duration, string timerName)
        {
            // Cancel the existing timer.
            this.CancelTimer(timerName);
            
            // Create and add the timer.
            var timer = new Timer(duration * 1000);
            timer.Elapsed += (sender, args) =>
            {
                this.OnTimerDone(timerName);
            };
            timer.AutoReset = false;
            timer.Start();
            this._timers[timerName] = timer;
        }

        /// <summary>
        /// Cancels a timer.
        /// </summary>
        /// <param name="timerName">Timer to cancel.</param>
        public void CancelTimer(string timerName)
        {
            if (!this._timers.ContainsKey(timerName)) return;
            this._timers[timerName].Stop();
            this._timers.Remove(timerName);
        }

        /// <summary>
        /// Cancels all the timers of the script.
        /// </summary>
        public void CancelAllTimers()
        {
            foreach (var timer in this._timers.Values)
            {
                timer.Stop();
            }
            this._timers.Clear();
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public virtual void OnTimerDone(string timerName)
        {
            
        }
        #endregion

        #region Physics
        /// <summary>
        /// Adds a cylinder physics body to the object.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="name"></param>
        public void SetProximityRadius(float radius, string name)
        {
            // Add the cylinder physics body.
            var physics = this.GameObject.AddNewComponent<PhysicsComponent>();
            var physicsObject = CylinderBody.Create(
                this.GameObject.Zone.Simulation,
                this.GameObject.Transform.Position,
                this.GameObject.Transform.Rotation,
                new Vector2(radius, radius * 2));
            physics.SetPhysics(physicsObject);
            
            // Listen for players entering and leaving.
            var playersInPhysicsObject = new List<Player>();
            Listen(physics.OnEnter, (other) =>
            {
                if (!(other.GameObject is Player player)) return;
                if (playersInPhysicsObject.Contains(player)) return;
                playersInPhysicsObject.Add(player);
                this.OnProximityUpdate(name, PhysicsCollisionStatus.Enter, player);
                
            });
            Listen(physics.OnLeave, (other) =>
            {
                if (!(other.GameObject is Player player)) return;
                if (!playersInPhysicsObject.Contains(player)) return;
                playersInPhysicsObject.Remove(player);
                this.OnProximityUpdate(name, PhysicsCollisionStatus.Leave, player);
            });
            Listen(this.GameObject.Zone.OnPlayerLeave, (player) =>
            {
                if (!playersInPhysicsObject.Contains(player)) return;
                playersInPhysicsObject.Remove(player);
                this.OnProximityUpdate(name, PhysicsCollisionStatus.Leave, player);
            });
        }

        /// <summary>
        /// Callback for the proximity of a player updating (entering
        /// or leaving a physics body).
        /// </summary>
        /// <param name="name">Name of the physics body.</param>
        /// <param name="status">Status of the player.</param>
        /// <param name="player">Player that changed.</param>
        public virtual void OnProximityUpdate(string name, PhysicsCollisionStatus status, Player player)
        {
            
        }
        #endregion
        
        #region Platforms
        /// <summary>
        /// Stops the automatic movement of the platform for the object.
        /// </summary>
        public void StopPathing()
        {
            if (!this.GameObject.TryGetComponent<MovingPlatformComponent>(out var movingPlatformComponent)) return;
            movingPlatformComponent.Stop();
        }

        /// <summary>
        /// Moves the platform to the given index.
        /// </summary>
        /// <param name="pathIndex">Index to move to.</param>
        public void GoToWaypoint(uint pathIndex)
        {
            if (!this.GameObject.TryGetComponent<MovingPlatformComponent>(out var movingPlatformComponent)) return;
            movingPlatformComponent.MoveTo((pathIndex == 0 ? (uint) (movingPlatformComponent.Path.Waypoints.Length - 1) : pathIndex - 1), () =>
            {
                OnArrivedAtDesiredWaypoint(pathIndex);
            });
        }

        /// <summary>
        /// Callback for the path index being reached.
        /// </summary>
        /// <param name="pathIndex">Index of the path reached.</param>
        public virtual void OnArrivedAtDesiredWaypoint(uint pathIndex)
        {
            
        }
        #endregion

        #region Effects
        /// <summary>
        /// Plays an ND sound.
        /// </summary>
        /// <param name="guid">GUID of the sound to play.</param>
        public void PlayNDAudioEmitter(string guid)
        {
            GameObject.Zone.BroadcastMessage(new PlayNDAudioEmitterMessage
            {
                Associate = this.GameObject,
                NDAudioEventGUID = guid,
            });
        }

        /// <summary>
        /// Plays an effect on the game object.
        /// </summary>
        /// <param name="name">Name of the effect.</param>
        /// <param name="type">Type of the effect.</param>
        /// <param name="id">Id of the effect.</param>
        public void PlayFXEffect(string name, string type, int id = -1)
        {
            this.GameObject.PlayFX(name, type, id);
        }
        
        /// <summary>
        /// Stops an effect on the game object.
        /// </summary>
        /// <param name="name">Name of the effect.</param>
        public void StopFXEffect(string name)
        {
            this.GameObject.StopFX(name);
        }
        
        /// <summary>
        /// Plays an animation on the game object.
        /// </summary>
        /// <param name="name">Name of the animation.</param>
        /// <param name="playImmediate">Whether the animation should be played immediately.</param>
        public void PlayAnimation(string name, bool playImmediate = false)
        {
            this.GameObject.Animate(name, playImmediate);
        }
        #endregion
        
        /// <summary>
        /// Loads the script.
        /// </summary>
        public override Task LoadAsync()
        {
            throw new InvalidOperationException("Object scripts can't be loaded as native scripts.");
        }
    }
}