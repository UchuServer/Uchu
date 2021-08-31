using System;
using System.Collections.Generic;
using System.Numerics;
using Uchu.Core;
using Uchu.Physics;

namespace Uchu.World
{
    public class PhysicsComponent : Component
    {
        public PhysicsObject Physics { get; private set; }
        
        public Event<PhysicsComponent> OnCollision { get; }
        
        public Event<PhysicsComponent> OnEnter { get; }
        
        public Event<PhysicsComponent> OnLeave { get; }
        
        private List<PhysicsObject> Check { get; set; }
        
        private List<PhysicsObject> Active { get; }
        
        protected PhysicsComponent()
        {
            Check = new List<PhysicsObject>();
            
            Active = new List<PhysicsObject>();
            
            OnCollision = new Event<PhysicsComponent>();

            OnEnter = new Event<PhysicsComponent>();
            
            OnLeave = new Event<PhysicsComponent>();
            
            Listen(OnStart, () =>
            {
                Listen(Zone.EarlyPhysics, Early);

                Listen(Zone.LatePhysics, Late);
            });
            
            Listen(OnDestroyed, () =>
            {
                Physics?.Dispose();

                OnCollision.Clear();
                
                OnEnter.Clear();
                
                OnLeave.Clear();
            });
        }

        public void SetPhysics(PhysicsObject physics)
        {
            physics.Associate = this;

            physics.OnCollision = HandleCollision;

            Physics = physics;

            // This is for POIs with a POI key in the settings, used for example everywhere in AG except for
            // Monument Orange Path (AG_Mon_3).
            // Not all POIs are handled this way, for example those in GF are done using triggers.
            if (GameObject.Settings.TryGetValue("POI", out var poiGroup))
            {
                Logger.Information($"Registered POI location {poiGroup}");

                Listen(GameObject.GetComponent<PhysicsComponent>().OnEnter, async component =>
                {
                    if (!(component.GameObject is Player player)) return;

                    Logger.Debug($"{player.Name} entered {(string) poiGroup}");

                    var inventory = player.GetComponent<MissionInventoryComponent>();
                    await inventory.DiscoverAsync((string) poiGroup);
                });
            }

            if (GameObject.Settings.TryGetValue("respawnVol", out var isRespawnVolume) && (bool) isRespawnVolume)
            {
                Listen(GameObject.GetComponent<PhysicsComponent>().OnEnter, async component =>
                {
                    if (!(component.GameObject is Player player)) return;

                    var position = (Vector3) GameObject.Settings["rspPos"];
                    var rotation = (Vector4) GameObject.Settings["rspRot"];
                    var newRotation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W);
                    if (player.TryGetComponent<CharacterComponent>(out var characterComponent))
                    {
                        characterComponent.SpawnPosition = position;
                        characterComponent.SpawnRotation = newRotation;
                    }

                    player.Message(new PlayerReachedRespawnCheckpointMessage
                    {
                        Associate = player,
                        Position = position,
                        Rotation = newRotation,
                    });
                });
            }
        }

        public void SetPhysicsByPath(string path) // We can't read HKX so this is basically just a bodge
        {
            path ??= "";

            path = path.ToLower();
            PhysicsObject finalObject;

            Vector3 size;

            // 10 x 5 x 1 (the file name is wrong)
            if (path.Contains("misc_phys_10x1x5"))
            {
                size = new Vector3(10, 5, 1)* Math.Abs(GameObject.Transform.Scale);
                // the origin is at the bottom center for this one, instead of in the middle
                // it's also offset a little bit in the x direction but it's not really noticeable
                var posRelativeToHkxOrigin = new Vector3(0, 0.5f * size.Y, 0);
                var pos = Transform.Position + Vector3.Transform(posRelativeToHkxOrigin, Transform.Rotation);
                finalObject = BoxBody.Create(Zone.Simulation, pos, Transform.Rotation, size );
            }
            // 640 x 1 x 640. used for https://lu.lcdruniverse.org/explorer/objects/5633
            else if (path.Contains("misc_phys_640x640"))
            {
                size = new Vector3(640, 12.5f, 640)* Math.Abs(GameObject.Transform.Scale);
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, size );
            }
            // 20 x 50 x 1. used for https://lu.lcdruniverse.org/explorer/objects/8575
            else if (path.Contains("trigger_wall_tall"))
            {
                size = new Vector3(20, 50, 1) * Math.Abs(GameObject.Transform.Scale);
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, size);
            }
            // 1 x 13 x 20. used for https://lu.lcdruniverse.org/explorer/objects/12384
            else if (path.Contains("poi_trigger_wall.hkx"))
            {
                size = new Vector3(1, 13, 20) * Math.Abs(GameObject.Transform.Scale);
                // the origin is at the bottom center for this one, instead of in the middle
                var posRelativeToHkxOrigin = new Vector3(0, 0.5f * size.Y, 0);
                var pos = Transform.Position + Vector3.Transform(posRelativeToHkxOrigin, Transform.Rotation);

                finalObject = BoxBody.Create(Zone.Simulation, pos, Transform.Rotation, size);
            }
            // approx. 20 x 5 x 7, used for https://lu.lcdruniverse.org/explorer/objects/12041
            else if (path.Contains("fx_nt_sentinal_ground_arrows.hkx"))
            {
                size = new Vector3(20, 5, 7) * Math.Abs(GameObject.Transform.Scale);
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, size);
            }
            // default is a 5x5x5 cube
            else
            {
                size = Vector3.One * 5f * Math.Abs(GameObject.Transform.Scale);
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, size);
            }

            SetPhysics(finalObject);

            Logger.Information($"Loaded physics object {path}");
        }

        private void Early()
        {
            Active.Clear();
        }
        
        private void HandleCollision(PhysicsObject other)
        {
            if (other == default) return;
            
            Active.Add(other);

            if (!Check.Contains(other))
            {
                OnEnter.Invoke(other.Associate as PhysicsComponent);
            }
            else
            {
                OnCollision.Invoke(other.Associate as PhysicsComponent);
            }
        }

        private void Late()
        {
            foreach (var other in Check.ToArray())
            {
                if (other?.Associate == default) continue;
                
                if (!Active.Contains(other))
                {
                    OnLeave.Invoke(other.Associate as PhysicsComponent);
                }
            }
            
            Check.Clear();

            foreach (var active in Active.ToArray())
            {
                if (active?.Associate == default) continue;

                Check.Add(active);
            }
        }

        public override string ToString()
        {
            return GameObject.ToString();
        }
    }
}
