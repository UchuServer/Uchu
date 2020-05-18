using System;
using System.Numerics;

namespace Uchu.Physics
{
    public abstract class PhysicsObject : IDisposable
    {
        public Action<PhysicsObject> OnCollision { get; set; }
        
        public abstract int Id { get; }
        
        public PhysicsSimulation Simulation { get; }
        
        public object Associate { get; set; }

        protected PhysicsObject(PhysicsSimulation simulation)
        {
            Simulation = simulation;
        }
        
        public virtual Vector3 Position { get; set; }

        public abstract void Dispose();
    }
}