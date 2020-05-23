using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Uchu.Physics.Callbacks;

namespace Uchu.Physics
{
    public class PhysicsSimulation : IDisposable
    {
        internal Simulation Simulation { get; }
        
        internal BufferPool Buffer { get; }
        
        internal NarrowPhaseCallbacks NarrowPhaseCallbacks { get; }
        
        internal PoseIntegratorCallbacks PoseIntegratorCallbacks { get; }
        
        public List<PhysicsObject> Objects { get; }

        public IEnumerable<PhysicsStatic> Statics => Objects.OfType<PhysicsStatic>();

        public IEnumerable<PhysicsBody> Bodies => Objects.OfType<PhysicsBody>();

        public PhysicsSimulation()
        {
            Objects = new List<PhysicsObject>();
            
            Buffer = new BufferPool();
            
            NarrowPhaseCallbacks = new NarrowPhaseCallbacks
            {
                OnCollision = HandleCollision
            };

            PoseIntegratorCallbacks = new PoseIntegratorCallbacks(Vector3.Zero);

            Simulation = Simulation.Create(Buffer, NarrowPhaseCallbacks, PoseIntegratorCallbacks);
        }

        public void Step(float deltaTime)
        {
            /*
             * DON'T SLEEP ON THE JOB!
             */
            var bodies = Bodies.ToArray();
            
            foreach (var physicsBody in bodies)
            {
                if (!physicsBody.Reference.Exists)
                {
                    Objects.Remove(physicsBody);
                    
                    continue;
                }
                
                physicsBody.Reference.Activity.SleepCandidate = false;

                if (!physicsBody.Reference.Awake)
                {
                    Simulation.Awakener.AwakenBody(physicsBody.Handle);
                }
            }

            Simulation.Timestep(deltaTime);
        }

        internal void Register(PhysicsObject obj)
        {
            Objects.Add(obj);
        }

        internal void Release(PhysicsObject obj)
        {
            Objects.Remove(obj);
        }
        
        private bool HandleCollision(CollidableReference referenceA, CollidableReference referenceB)
        {
            var a = FindObject(referenceA.StaticHandle, referenceA.BodyHandle);
            
            var b = FindObject(referenceB.StaticHandle, referenceB.BodyHandle);

            try
            {
                a.OnCollision?.Invoke(b);

                b.OnCollision?.Invoke(a);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private PhysicsObject FindObject(StaticHandle staticHandle, BodyHandle bodyHandle)
        {
            foreach (var physicsObject in Bodies)
            {
                if (!physicsObject.Reference.Exists) continue;
                
                if (physicsObject.Id == bodyHandle.Value)
                {
                    return physicsObject;
                }
            }
            
            foreach (var physicsObject in Statics)
            {
                if (!physicsObject.Reference.Exists) continue;

                if (physicsObject.Id == staticHandle.Value)
                {
                    return physicsObject;
                }
            }

            throw new Exception($"Failed to find physics object: Got {staticHandle}/{bodyHandle}");
        }

        public void Dispose()
        {
            Simulation?.Dispose();
            Buffer?.Clear();
        }
    }
}