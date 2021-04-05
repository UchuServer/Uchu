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