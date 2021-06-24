using System.Collections.Generic;
using System.Numerics;
using Uchu.Physics;

namespace Uchu.World.Scripting.Native
{
    public enum PhysicsCollisionStatus
    {
        Enter,
        Leave,
    }
    
    public abstract class ObjectScript : ObjectBase
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
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ObjectScript(GameObject gameObject)
        {
            this.GameObject = gameObject;
            
            // Connect clearing on destroyed.
            Listen(gameObject.OnDestroyed, this.Clear);
        }

        /// <summary>
        /// Clears the script.
        /// Should be called when the GameObject is destroyed.
        /// </summary>
        public virtual void Clear()
        {
            this.ClearListeners();
        }

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
        #endregion
    }
}