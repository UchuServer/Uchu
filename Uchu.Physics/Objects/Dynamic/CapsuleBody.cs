using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Uchu.Physics
{
    public class CapsuleBody : PhysicsBody
    {
        /// <summary>
        /// Creates a capsule body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="handle">Body handle to use.</param>
        public CapsuleBody(PhysicsSimulation simulation, BodyHandle handle) : base(simulation, handle)
        {
        }

        /// <summary>
        /// Creates a capsule body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="size">The size of the body.</param>
        /// <returns>The capsule body that was created.</returns>
        public static CapsuleBody Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector2 size)
        {
            var shape = new Capsule(size.X, size.Y);
            shape.ComputeInertia(1, out var inertia);
            var index = simulation.RegisterShape(shape);
            var collidable = new CollidableDescription(index, 0.1f);
            var activity = new BodyActivityDescription(0.01f);
            var pose = new RigidPose(position, rotation);
            var descriptor = BodyDescription.CreateDynamic(pose, inertia, collidable, activity);
            var handle = simulation.CreateBodyHandle(descriptor);
            var obj = new CapsuleBody(simulation, handle);
            simulation.Register(obj);

            return obj;
        }
    }
}