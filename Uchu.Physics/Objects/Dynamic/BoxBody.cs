using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Uchu.Physics
{
    public class BoxBody : PhysicsBody
    {
        public BoxBody(PhysicsSimulation simulation, BodyHandle handle) : base(simulation, handle)
        {
        }

        public static BoxBody Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);

            shape.ComputeInertia(1, out var inertia);

            var index = simulation.Simulation.Shapes.Add(shape);

            var collidable = new CollidableDescription(index, 0.1f);
            
            var activity = new BodyActivityDescription(0);

            var pose = new RigidPose(position, rotation);
            
            var descriptor = BodyDescription.CreateDynamic(pose, inertia, collidable, activity);

            var handle = simulation.Simulation.Bodies.Add(descriptor);

            var obj = new BoxBody(simulation, handle);

            simulation.Register(obj);

            return obj;
        }
    }
}