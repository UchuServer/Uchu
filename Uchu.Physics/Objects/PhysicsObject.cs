using System;
using System.Numerics;

namespace Uchu.Physics
{
    public abstract class PhysicsObject : IDisposable
    {
        /// <summary>
        /// Event for a collision occuring with another object.
        /// </summary>
        public Action<PhysicsObject> OnCollision { get; set; }
        
        /// <summary>
        /// Id of the physics object.
        /// </summary>
        public abstract int Id { get; }
        
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
        public virtual Vector3 Position { get; set; }

        /// <summary>
        /// Disposes the physics object.
        /// </summary>
        public abstract void Dispose();
    }
}