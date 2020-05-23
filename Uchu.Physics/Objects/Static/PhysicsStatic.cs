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

        public override Vector3 Position
        {
            get
            {
                if (!Reference.Exists) return default;
                
                return Reference.Pose.Position;
            }
        }

        public override void Dispose()
        {
            Simulation.Release(this);
            
            if (!Reference.Exists) return;
            
            Simulation.Simulation.Statics.Remove(Handle);
        }
    }
}