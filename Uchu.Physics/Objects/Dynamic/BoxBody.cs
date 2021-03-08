using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Uchu.Physics
{
    public class BoxBody : PhysicsBody
    {
        /// <summary>
        /// Creates a box body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="handle">Body handle to use.</param>
        public BoxBody(PhysicsSimulation simulation, BodyHandle handle) : base(simulation, handle)
        {
        }

        /// <summary>
        /// Creates a box body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="size">The size of the body.</param>
        /// <returns>The box body that was created.</returns>
        public static BoxBody Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            shape.ComputeInertia(1, out var inertia);
            var index = simulation.RegisterShape(shape);
            var collidable = new CollidableDescription(index, 0.1f);
            var activity = new BodyActivityDescription(0);
            var pose = new RigidPose(position, rotation);
            var descriptor = BodyDescription.CreateDynamic(pose, inertia, collidable, activity);
            var handle = simulation.CreateBodyHandle(descriptor);
            var obj = new BoxBody(simulation, handle);
            simulation.Register(obj);
            
            return obj;
        }
    }
}