using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using InfectedRose.Lvl;
using Uchu.Physics;

namespace Uchu.World.Scripting.Native
{
    public enum PhysicsCollisionStatus
    {
        Enter,
        Leave,
    }
    
    public abstract class ObjectScript : NativeScript
    {
        private class FXEffect
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public int Id { get; set; }
        }
        
        private class Animation
        {
            public string Name { get; set; }
            public bool PlayImmediate { get; set; }
        }

        /// <summary>
        /// List of the run-once methods that were ran.
        /// </summary>
        private static List<Type> _runOnceMethodsRan = new List<Type>();
        
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
        /// Active effects to play for new players.
        /// </summary>
        private Dictionary<string, FXEffect> _effects = new Dictionary<string, FXEffect>();

        /// <summary>
        /// Active animation to play for new players.
        /// </summary>
        private Animation _animation;
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ObjectScript(GameObject gameObject)
        {
            this.GameObject = gameObject;
            this.SetZone(gameObject.Zone);
            
            // Determine if the run-once method should be run.
            var firstRun = !_runOnceMethodsRan.Contains(this.GetType());
            if (firstRun)
            {
                _runOnceMethodsRan.Add(this.GetType());
            }
            
            // Connect clearing on destroyed.
            Listen(gameObject.OnDestroyed, () =>
            {
                // Stop the timers.
                this.CancelAllTimers();
                
                // Unload the script.
                this.ClearListeners();
                this.UnloadAsync();
            });
            
            // Run the run-once method.
            if (firstRun)
            {
                this.CompleteOnce();
            }
            
            // Connect players loading the objects.
            foreach (var player in this.Zone.Players)
            {
                Listen(player.OnReadyForUpdatesEvent, (message) =>
                {
                    if (message.GameObject != this.GameObject) return;
                    this.SendEffectsToPlayer(player);
                });
            }
            Listen(this.Zone.OnPlayerLoad, (player) =>
            {
                Listen(player.OnReadyForUpdatesEvent, (message) =>
                {
                    if (message.GameObject != this.GameObject) return;
                    this.SendEffectsToPlayer(player);
                });
            });
        }
        
        /// <summary>
        /// Callback that is run once with the first GameObject created.
        /// </summary>
        public virtual void CompleteOnce()
        {
            
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
        /// Prepares an LDF wot use with ScriptNetworkVarUpdateMessage and
        /// sets the internal LDF data of the script component.
        /// </summary>
        /// <param name="name">Name of the variable to store.</param>
        /// <param name="value">Value of the variable to store.</param>
        /// <param name="ldf">Dictionary to set up for sending with ScriptNetworkVarUpdateMessage.</param>
        private void PrepareDictionary(string name, object value, LegoDataDictionary ldf)
        {
            // Add the value.
            if (value is IList list)
            {
                // Add the list entries.
                // These aren't stored in the script.
                for (var i = 0; i < list.Count; i++)
                {
                    var subValue = list[i];
                    var key = name + "." + (i + 1);
                    if (subValue is GameObject gameObject)
                    {
                        ldf.Add(key, gameObject.Id);
                    }
                    else
                    {
                        ldf.Add(key, subValue);
                    }
                }
            }
            else
            {
                // Convert the object.
                var newValue = value;
                if (value is GameObject gameObject)
                {
                    newValue = (long) gameObject.Id;
                }
                
                // Store the object.
                if (this.GameObject.TryGetComponent<LuaScriptComponent>(out var component))
                {
                    component.Data ??= new LegoDataDictionary();
                    component.Data.Add(name, newValue);
                }
                ldf.Add(name, newValue);
            }
        }
        
        /// <summary>
        /// Sets a networked variable of the script.
        /// </summary>
        /// <param name="name">Name of the variable to store.</param>
        /// <param name="value">Value of the variable to store.</param>
        public void SetNetworkVar(string name, object value)
        {
            // Set the variable.
            this.SetVar(name, value);
            
            // Create the LDF.
            var ldf = new LegoDataDictionary();
            PrepareDictionary(name, value, ldf);

            // Broadcast the variable change.
            Task.Run(() =>
            {
                this.Zone.BroadcastMessage(new ScriptNetworkVarUpdateMessage()
                {
                    Associate = this.GameObject,
                    LDFInText = ldf.ToString(),
                });
            });
        }
        
        /// <summary>
        /// Sets a networked variable of the script.
        /// </summary>
        /// <param name="values">Values to send in the same request.</param>
        public void SetNetworkVar((string, object)[] values)
        {
            // Set the variables and create the LDF.
            var ldf = new LegoDataDictionary();
            foreach (var (key, value) in values)
            {
                this.SetVar(key, value);
                PrepareDictionary(key, value, ldf);
            }
            

            // Broadcast the variable change.
            Task.Run(() =>
            {
                this.Zone.BroadcastMessage(new ScriptNetworkVarUpdateMessage()
                {
                    Associate = this.GameObject,
                    LDFInText = ldf.ToString(),
                });
            });
        }

        /// <summary>
        /// Gets a stored variable of the script.
        /// </summary>
        /// <param name="name">Name of the variable to fetch.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value of the variable to fetch.</returns>
        public T GetVar<T>(string name)
        {
            // Add the value if it doesn't exist and is a value type.
            if (!this._variables.ContainsKey(name) && typeof(T).IsValueType)
            {
                this._variables[name] = Activator.CreateInstance(typeof(T));
            }
            
            // Return the value.
            return (T) this._variables[name];
        }

        /// <summary>
        /// Gets a stored variable of the script.
        /// </summary>
        /// <param name="name">Name of the variable to fetch.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>Value of the variable to fetch.</returns>
        public T GetNetworkVar<T>(string name)
        {
            return GetVar<T>(name);
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
            // Add an object and a cylinder physics body.
            // One game object/script may require multiple physics bodies.
            var proximityObject = GameObject.Instantiate(GameObject.Zone);
            var physics = proximityObject.AddComponent<PhysicsComponent>();
            var physicsObject = CylinderBody.Create(
                this.GameObject.Zone.Simulation,
                this.GameObject.Transform.Position,
                this.GameObject.Transform.Rotation,
                new Vector2(radius, radius * 2));
            physics.SetPhysics(physicsObject);
            
            // Listen for players entering and leaving.
            var playersInPhysicsObject = new List<Player>();
            Listen(physics.OnEnter, (component) =>
            {
                if (!(component.GameObject is Player player)) return;
                if (playersInPhysicsObject.Contains(player)) return;
                playersInPhysicsObject.Add(player);
                this.OnProximityUpdate(name, PhysicsCollisionStatus.Enter, player);
            });
            Listen(physics.OnLeave, (component) =>
            {
                if (!(component.GameObject is Player player)) return;
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
        /// Starts an effect on the game object. Intended for
        /// effects that are long-running and may be active
        /// when a player loads the area.
        /// </summary>
        /// <param name="name">Name of the effect.</param>
        /// <param name="type">Type of the effect.</param>
        /// <param name="id">Id of the effect.</param>
        public void StartFXEffect(string name, string type, int id = -1)
        {
            this.GameObject.PlayFX(name, type, id);
            this._effects[name] = new FXEffect()
            {
                Name = name,
                Type = type,
                Id = id,
            };
        }

        /// <summary>
        /// Stops an effect on the game object.
        /// </summary>
        /// <param name="name">Name of the effect.</param>
        public void StopFXEffect(string name)
        {
            this.GameObject.StopFX(name);
            this._effects.Remove(name);
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
        
        /// <summary>
        /// Starts an animation on the game object. Intended for
        /// animations that are long-running and may be active
        /// when a player loads the area.
        /// </summary>
        /// <param name="name">Name of the animation.</param>
        /// <param name="playImmediate">Whether the animation should be played immediately.</param>
        public void StartAnimation(string name, bool playImmediate = false)
        {
            this.GameObject.Animate(name, playImmediate);
            this._animation = new Animation()
            {
                Name = name,
                PlayImmediate = playImmediate,
            };
        }

        /// <summary>
        /// Sends the current animations and effects to the player.
        /// </summary>
        /// <param name="player">Player to send the animations and effects.</param>
        private void SendEffectsToPlayer(Player player)
        {
            // Send the animation.
            if (this._animation != null)
            {
                player.Message(new PlayAnimationMessage
                {
                    Associate = this.GameObject,
                    AnimationsId = this._animation.Name,
                    PlayImmediate = this._animation.PlayImmediate,
                });
            }
            
            // Send the effects.
            foreach (var effect in this._effects.Values)
            {
                player.Message(new PlayFXEffectMessage
                {
                    Associate = this.GameObject,
                    EffectId = effect.Id,
                    EffectType = effect.Type,
                    Name = effect.Name,
                });
            }
        }
        #endregion

        #region Activities/// <summary>
        /// Adds a player to the current activity.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void AddActivityUser(Player player)
        {
            // TODO: More work required for activity scores.
            if (!this.GameObject.TryGetComponent<ScriptedActivityComponent>(out var component)) return;
            component.Participants.Add(player);
            Serialize(this.GameObject);
        }
        
        /// <summary>
        /// Removes a player from the current activity.
        /// </summary>
        /// <param name="player">Player to remove.</param>
        public void RemoveActivityUser(Player player)
        {
            // TODO: Implement.
        }
        
        /// <summary>
        /// Sets the activity value for a given player.
        /// </summary>
        /// <param name="player">Player to set.</param>
        /// <param name="index">Index of the value to set.</param>
        /// <param name="value">Value to set.</param>
        public void SetActivityUserData(Player player, int index, float value)
        {
            // TODO: Implement.
        }
        
        /// <summary>
        /// Returns the activity value for a given player.
        /// </summary>
        /// <param name="player">Player to set.</param>
        /// <param name="index">Index of the value to set.</param>
        /// <returns>The activity value for the player.</returns>
        public float GetActivityUserData(Player player, int index)
        {
            return 0; // TODO: Implement.
        }

        /// <summary>
        /// Starts the activity.
        /// </summary>
        public void ActivityStart()
        {
            GameObject.Zone.BroadcastMessage(new ActivityStartMessage()
            {
                Associate = this.GameObject,
            });
        }
        
        /// <summary>
        /// Adds a player to the current minigame.
        /// </summary>
        /// <param name="player">Player to add.</param>
        public void MiniGameAddPlayer(Player player)
        {
            this.AddActivityUser(player);
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