using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Uchu.Physics
{
    public class TriggerBox : PhysicsStatic
    {
        public TriggerBox(PhysicsSimulation simulation, StaticHandle handle) : base(simulation, handle)
        {
        }

        public static TriggerBox Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);

            var index = simulation.Simulation.Shapes.Add(shape);

            var collidable = new CollidableDescription(index, 0.01f);

            var descriptor = new StaticDescription(position, rotation, collidable);

            var handle = simulation.Simulation.Statics.Add(descriptor);

            var obj = new TriggerBox(simulation, handle);

            simulation.Register(obj);

            return obj;
        }
    }
}