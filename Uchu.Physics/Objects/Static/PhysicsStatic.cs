using System.Numerics;
using BepuPhysics;

namespace Uchu.Physics
{
    public abstract class PhysicsStatic : PhysicsObject
    {
        internal StaticHandle Handle { get; }
        
        internal StaticReference Reference { get; }

        public PhysicsStatic(PhysicsSimulation simulation, StaticHandle handle) : base(simulation)
        {
            Handle = handle;

            Reference = simulation.Simulation.Statics.GetStaticReference(handle);
        }

        public override int Id => Handle.Value;

        public override Vector3 Position => Reference.Pose.Position;

        public override void Dispose()
        {
            Simulation.Simulation.Statics.Remove(Handle);

            Simulation.Release(this);
        }
    }
}