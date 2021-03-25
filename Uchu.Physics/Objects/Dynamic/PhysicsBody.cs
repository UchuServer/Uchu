using System.Numerics;
using BepuPhysics;

namespace Uchu.Physics
{
    public abstract class PhysicsBody : PhysicsObject
    {
        /// <summary>
        /// Handle of the physics body.
        /// </summary>
        internal BodyHandle Handle { get; }
        
        /// <summary>
        /// Reference of the physics body.
        /// </summary>
        internal BodyReference Reference { get; }

        /// <summary>
        /// Creates a physics body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="handle">Body handle to use.</param>
        protected PhysicsBody(PhysicsSimulation simulation, BodyHandle handle) : base(simulation)
        {
            Handle = handle;
            Reference = simulation.GetBodyReference(handle);
        }

        /// <summary>
        /// Id of the physics object.
        /// </summary>
        public override int Id => Handle.Value;

        /// <summary>
        /// Position of the physics object.
        /// </summary>
        public override Vector3 Position
        {
            get => !Reference.Exists ? default : Reference.Pose.Position;
            set
            {
                if (!Reference.Exists) return;
                Reference.Pose.Position = value;
            }
        }

        /// <summary>
        /// Rotation of the physics object.
        /// </summary>
        public Quaternion Rotation
        {
            get => !Reference.Exists ? default : Reference.Pose.Orientation;
            set
            {
                if (!Reference.Exists) return;
                Reference.Pose.Orientation = value;
            }
        }
        
        /// <summary>
        /// Angular velocity of the physics object.
        /// </summary>
        public Vector3 AngularVelocity
        {
            get => !Reference.Exists ? default : Reference.Velocity.Angular;
            set
            {
                if (!Reference.Exists) return;
                Reference.Velocity.Angular = value;
            }
        }

        /// <summary>
        /// Velocity of the physics object.
        /// </summary>
        public Vector3 LinearVelocity
        {
            get => !Reference.Exists ? default : Reference.Velocity.Linear;
            set
            {
                if (!Reference.Exists) return;
                Reference.Velocity.Linear = value;
            }
        }

        /// <summary>
        /// Disposes the physics object.
        /// </summary>
        public override void Dispose()
        {
            Simulation.Release(this);
            if (!Reference.Exists) return;
            Simulation.RemoveBodyHandle(Handle);
        }
    }
}