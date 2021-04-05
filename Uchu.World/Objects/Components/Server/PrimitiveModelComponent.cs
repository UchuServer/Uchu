using System.Linq;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Physics;
using Uchu.World.Client;

namespace Uchu.World
{
    public class PrimitiveModelComponent : Component
    {
        protected PrimitiveModelComponent()
        {
            Listen(OnStart, LoadPhysics);
        }
        
        private void LoadPhysics()
        {
            var physics = LoadPrimitiveModel();
            
            if (physics == default) return;
            
            var physicsComponent = GameObject.AddComponent<PhysicsComponent>();

            physicsComponent.SetPhysics(physics);
            
            if (GameObject.Settings.TryGetValue("POI", out var group))
            {
                var task = ClientCache.GetTable<MissionTasks>().Where(i => i.TargetGroup == (string)group).FirstOrDefault();
                if (task == default) return;
                var missionID = task.Id.Value;
                Listen(physicsComponent.OnCollision, async component =>
                {
                    if (!(component.GameObject is Player)) return;
                    Player player = (Player) component.GameObject;
                    var missionComponent = player.GetComponent<MissionInventoryComponent>();
                    if (missionComponent.HasMission(missionID))
                    {
                        await missionComponent.GetMission(missionID).CompleteAsync();
                    }
                    else
                    {
                        var poiAchievement = await missionComponent.AddMissionAsync(missionID, player);
                        await poiAchievement.StartAsync();
                        await poiAchievement.CompleteAsync();
                    }
                });
            }
        }

        // TODO: Support more than cuboids
        private PhysicsObject LoadPrimitiveModel()
        {
            if (!GameObject.Settings.TryGetValue("primitiveModelType", out var typeString))
            {
                return default;
            }

            var type = (PrimitiveModelType) (int) typeString;

            switch (type)
            {
                case PrimitiveModelType.Cuboid:
                    var cuboidSize = new Vector3
                    {
                        X = (float) GameObject.Settings["primitiveModelValueX"],
                        Y = (float) GameObject.Settings["primitiveModelValueY"],
                        Z = (float) GameObject.Settings["primitiveModelValueZ"]
                    };

                    return BoxBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, cuboidSize);
                case PrimitiveModelType.Cone:
                    break;
                case PrimitiveModelType.Cylinder:
                    var cylinderSize = new Vector2
                    {
                        X = (float) GameObject.Settings["primitiveModelValueX"],
                        Y = (float) GameObject.Settings["primitiveModelValueY"]
                    };

                    return CylinderBody.Create(Zone.Simulation, Transform.Position, Transform.Rotation, cylinderSize);
                case PrimitiveModelType.Sphere:
                    break;
                case PrimitiveModelType.Invalid:
                    goto default;
                default:
                    Logger.Error($"{type} is not a valid {nameof(PrimitiveModelType)}!");
                    return default;
            }
            
            Logger.Error($"{type} is not a supported {nameof(PrimitiveModelType)}!");

            return default;
        }
    }
}