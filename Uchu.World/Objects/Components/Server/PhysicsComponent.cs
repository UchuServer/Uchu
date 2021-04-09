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

            if (GameObject.Settings.TryGetValue("POI", out var poiGroup))
            {
                Logger.Information($"Registered POI location {poiGroup}");

                Listen(GameObject.GetComponent<World.PhysicsComponent>().OnEnter, async component =>
                {
                    if (!(component.GameObject is Player player)) return;

                    Logger.Information($"{player.Name} entered and {(string) poiGroup}");

                    var inventory = player.GetComponent<MissionInventoryComponent>();
                    await inventory.DiscoverAsync((string) poiGroup);
                });
            }

            // Jett's stuff
            // if (GameObject.Settings.TryGetValue("POI", out var group))
            // {
            //     Logger.Information($"Registered POI location {group}");
            //     var task = ClientCache.GetTable<MissionTasks>().Where(i => i.TargetGroup == (string)group).FirstOrDefault();
            //     if (task == default) return;
            //     var missionID = task.Id.Value;
            //     Listen(GameObject.GetComponent<World.PhysicsComponent>().OnEnter, async component =>
            //     {
            //         if (!(component.GameObject is Player)) return;
            //         Player player = (Player)component.GameObject;
            //         var missionComponent = player.GetComponent<MissionInventoryComponent>();
            //         if (missionComponent.HasCompleted(missionID)) return;
            //         Logger.Information($"{player.Name} entered and discovered {(string)group}");
            //         if (missionComponent.HasMission(missionID)) await missionComponent.GetMission(missionID).CompleteAsync();
            //         else
            //         {
            //             var poiAchievement = await missionComponent.AddMissionAsync(missionID, player);
            //             await poiAchievement.StartAsync();
            //             await poiAchievement.CompleteAsync();
            //         }
            //     });
            // }
        }

        public void SetPhysicsByPath(string path) // We can't read HKX so this is basically just a bodge
        {
            path ??= "";

            path = path.ToLower();
            PhysicsObject finalObject = null;

            if (path.Contains("misc_phys_10x1x5"))
            {
                // 10 x 5 x 1, the file name is messed up this is correct
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, new Vector3(10, 5, 1) * GameObject.Transform.Scale);
            }
            else if (path.Contains("misc_phys_640x640"))
            {
                // 640 x 1 x 640
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, new Vector3(640, 640, 12.5f) * GameObject.Transform.Scale);
            }
            else if (path.Contains("trigger_wall_tall"))
            {
                // 20 x 50 x 1
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, new Vector3(20, 50, 1) * GameObject.Transform.Scale);
            }
            else
            {
                finalObject = BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, new Vector3(2, 2, 2) * GameObject.Transform.Scale);
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
            foreach (var other in Check)
            {
                if (other?.Associate == default) continue;
                
                if (!Active.Contains(other))
                {
                    OnLeave.Invoke(other.Associate as PhysicsComponent);
                }
            }
            
            Check.Clear();

            foreach (var active in Active)
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