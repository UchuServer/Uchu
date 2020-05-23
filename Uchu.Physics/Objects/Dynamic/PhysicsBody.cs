using System.Numerics;
using BepuPhysics;

namespace Uchu.Physics
{
    public abstract class PhysicsBody : PhysicsObject
    {
        internal BodyHandle Handle { get; }
        
        internal BodyReference Reference { get; }

        protected PhysicsBody(PhysicsSimulation simulation, BodyHandle handle) : base(simulation)
        {
            Handle = handle;

            Reference = simulation.Simulation.Bodies.GetBodyReference(handle);
        }

        public override int Id => Handle.Value;

        public override Vector3 Position
        {
            get
            {
                if (!Reference.Exists) return default;
                
                return Reference.Pose.Position;
            }
            set
            {
                if (!Reference.Exists) return;
                
                Reference.Pose.Position = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                if (!Reference.Exists) return default;

                return Reference.Pose.Orientation;
            }
            set
            {
                if (!Reference.Exists) return;
                
                Reference.Pose.Orientation = value;
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                if (!Reference.Exists) return default;

                return Reference.Velocity.Angular;
            }
            set
            {
                if (!Reference.Exists) return;
                
                Reference.Velocity.Angular = value;
            }
        }

        public Vector3 LinearVelocity
        {
            get
            {
                if (!Reference.Exists) return default;

                return Reference.Velocity.Linear;
            }
            set
            {
                if (!Reference.Exists) return;
                
                Reference.Velocity.Linear = value;
            }
        }

        public override void Dispose()
        {
            Simulation.Release(this);
            
            if (!Reference.Exists) return;
            
            Simulation.Simulation.Bodies.Remove(Handle);
        }
    }
}