using System.Numerics;
using BepuPhysics;

namespace Uchu.Physics
{
    public abstract class PhysicsStatic : PhysicsObject
    {
        /// <summary>
        /// Static handle for physics.
        /// </summary>
        internal StaticHandle Handle { get; }
        
        /// <summary>
        /// Reference for the static physics object.
        /// </summary>
        internal StaticReference Reference { get; }

        /// <summary>
        /// Creates a static physics object.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="handle">Static handle to use.</param>
        public PhysicsStatic(PhysicsSimulation simulation, StaticHandle handle) : base(simulation)
        {
            Handle = handle;
            Reference = simulation.GetStaticReference(handle);
        }

        /// <summary>
        /// Id of the physics object.
        /// </summary>
        public override int Id => Handle.Value;

        /// <summary>
        /// Position of the physics object.
        /// </summary>
        public override Vector3 Position => !Reference.Exists ? default : Reference.Pose.Position;

        /// <summary>
        /// Disposes the physics object.
        /// </summary>
        public override void Dispose()
        {
            Simulation.Release(this);
            if (!Reference.Exists) return;
            Simulation.RemoveStaticHandle(Handle);
        }
    }
}