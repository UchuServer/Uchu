using System;
using System.Numerics;

namespace Uchu.Physics
{
    public abstract class PhysicsObject : IDisposable
    {
        /// <summary>
        /// Value used to determine the order in which objects are passed to the collision detection functions.
        /// </summary>
        public abstract int CollisionPrecedence { get; }

        public bool CanCollideIntoThings = false;
        public bool CanBeCollidedInto = true;

        /// <summary>
        /// Event for a collision occuring with another object.
        /// </summary>
        public Action<PhysicsObject> OnCollision { get; set; }
        
        /// <summary>
        /// Simulation attached to the physics object.
        /// </summary>
        public PhysicsSimulation Simulation { get; }
        
        /// <summary>
        /// Game object attached to the physics component.
        /// </summary>
        public object Associate { get; set; }

        /// <summary>
        /// Creates the physics object,
        /// </summary>
        /// <param name="simulation">Simulation to use.</param>
        protected PhysicsObject(PhysicsSimulation simulation)
        {
            Simulation = simulation;
        }

        /// <summary>
        /// Position of the physics object.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Orientation of the cube.
        /// </summary>
        public Quaternion Rotation { get; set; }

        /// <summary>
        /// Disposes the physics object.
        /// </summary>
        public void Dispose()
        {
            Simulation.Release(this);
        }
    }
}
