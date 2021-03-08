using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;

namespace Uchu.Physics
{
    public class TriggerBox : PhysicsStatic
    {
        /// <summary>
        /// Creates a trigger box.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="handle">Static handle to use.</param>
        public TriggerBox(PhysicsSimulation simulation, StaticHandle handle) : base(simulation, handle)
        {
        }

        /// <summary>
        /// Creates a trigger box.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The static position of the trigger box.</param>
        /// <param name="rotation">The static rotation of the trigger box.</param>
        /// <param name="size">The static size of the trigger box.</param>
        /// <returns>The trigger box that was created.</returns>
        public static TriggerBox Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector3 size)
        {
            var shape = new Box(size.X, size.Y, size.Z);
            var index = simulation.RegisterShape(shape);
            var collidable = new CollidableDescription(index, 0.01f);
            var descriptor = new StaticDescription(position, rotation, collidable);
            var handle = simulation.CreateStaticHandle(descriptor);
            var obj = new TriggerBox(simulation, handle);
            simulation.Register(obj);

            return obj;
        }
    }
}