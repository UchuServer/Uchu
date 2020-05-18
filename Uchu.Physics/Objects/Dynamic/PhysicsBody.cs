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
            get => Reference.Pose.Position;
            set => Reference.Pose.Position = value;
        }

        public Quaternion Rotation
        {
            get => Reference.Pose.Orientation;
            set => Reference.Pose.Orientation = value;
        }

        public Vector3 AngularVelocity
        {
            get => Reference.Velocity.Angular;
            set => Reference.Velocity.Angular = value;
        }

        public Vector3 LinearVelocity
        {
            get => Reference.Velocity.Linear;
            set => Reference.Velocity.Linear = value;
        }

        public override void Dispose()
        {
            Simulation.Simulation.Bodies.Remove(Handle);

            Simulation.Release(this);
        }
    }
}