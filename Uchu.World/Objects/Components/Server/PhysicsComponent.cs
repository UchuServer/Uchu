using System.Collections.Generic;
using Uchu.Physics;

namespace Uchu.World
{
    public class PhysicsComponent : Component
    {
        public PhysicsObject Physics { get; private set; }
        
        public Event<PhysicsComponent> OnCollision { get; }
        
        public Event<PhysicsComponent> OnEnter { get; }
        
        public Event<PhysicsComponent> OnLeave { get; }
        
        private List<PhysicsObject> Check { get; set; }
        
        private List<PhysicsObject> Active { get; }
        
        protected PhysicsComponent()
        {
            Check = new List<PhysicsObject>();
            
            Active = new List<PhysicsObject>();
            
            OnCollision = new Event<PhysicsComponent>();

            OnEnter = new Event<PhysicsComponent>();
            
            OnLeave = new Event<PhysicsComponent>();
            
            Listen(OnStart, () =>
            {
                Listen(Zone.EarlyPhysics, Early);

                Listen(Zone.LatePhysics, Late);
            });
            
            Listen(OnDestroyed, () =>
            {
                Physics?.Dispose();

                OnCollision.Clear();
                
                OnEnter.Clear();
                
                OnLeave.Clear();
            });
        }

        public void SetPhysics(PhysicsObject physics)
        {
            physics.Associate = this;

            physics.OnCollision = HandleCollision;
            
            Physics = physics;
        }

        private void Early()
        {
            Active.Clear();
        }
        
        private void HandleCollision(PhysicsObject other)
        {
            if (other == default) return;
            
            Active.Add(other);

            if (!Check.Contains(other))
            {
                OnEnter.Invoke(other.Associate as PhysicsComponent);
            }
            else
            {
                OnCollision.Invoke(other.Associate as PhysicsComponent);
            }
        }

        private void Late()
        {
            foreach (var other in Check)
            {
                if (other?.Associate == default) continue;
                
                if (!Active.Contains(other))
                {
                    OnLeave.Invoke(other.Associate as PhysicsComponent);
                }
            }
            
            Check.Clear();

            foreach (var active in Active)
            {
                if (active?.Associate == default) continue;

                Check.Add(active);
            }
        }

        public override string ToString()
        {
            return GameObject.ToString();
        }
    }
}