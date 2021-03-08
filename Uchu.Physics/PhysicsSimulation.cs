using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Uchu.Core;
using Uchu.Physics.Callbacks;

namespace Uchu.Physics
{
    public class PhysicsSimulation : IDisposable
    {
        /// <summary>
        /// BepuPhysics simulation.
        /// </summary>
        internal Simulation Simulation { get; }
        
        /// <summary>
        /// Buffer for BepuPhysics.
        /// </summary>
        private BufferPool Buffer { get; }
        
        /// <summary>
        /// Callback object for handling collisions.
        /// </summary>
        private NarrowPhaseCallbacks NarrowPhaseCallbacks { get; }
        
        /// <summary>
        /// Callback object for pose integration.
        /// </summary>
        private PoseIntegratorCallbacks PoseIntegratorCallbacks { get; }
        
        /// <summary>
        /// Objects in the simulation.
        /// </summary>
        private List<PhysicsObject> Objects { get; }

        /// <summary>
        /// Getter for the static physics objects.
        /// </summary>
        public IEnumerable<PhysicsStatic> Statics => Objects.OfType<PhysicsStatic>();

        /// <summary>
        /// Getter for the physics bodies.
        /// </summary>
        public IEnumerable<PhysicsBody> Bodies => Objects.OfType<PhysicsBody>();

        /// <summary>
        /// Creates the physics simulation.
        /// </summary>
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

        /// <summary>
        /// Steps the physics
        /// </summary>
        /// <param name="deltaTime">Delta time in milliseconds since last tick</param>
        public void Step(float deltaTime)
        {
            foreach (var physicsBody in Bodies.ToArray())
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

            Simulation.Timestep(deltaTime / 1000);
        }

        /// <summary>
        /// Registers a physics object.
        /// </summary>
        /// <param name="obj">Physics object to register.</param>
        internal void Register(PhysicsObject obj)
        {
            Objects.Add(obj);
        }

        /// <summary>
        /// Removes a physics object.
        /// </summary>
        /// <param name="obj">Physics object to remove.</param>
        internal void Release(PhysicsObject obj)
        {
            Objects.Remove(obj);
        }
        
        /// <summary>
        /// Handles a collision between 2 objects.
        /// </summary>
        /// <param name="referenceA">Reference to the first object.</param>
        /// <param name="referenceB">Reference to the second object.</param>
        /// <returns>Always false.</returns> 
        private bool HandleCollision(CollidableReference referenceA, CollidableReference referenceB)
        {
            // Get the references.
            var a = FindObject(referenceA.StaticHandle, referenceA.BodyHandle);
            var b = FindObject(referenceB.StaticHandle, referenceB.BodyHandle);

            // Invoke the collision events.
            try
            {
                a.OnCollision?.Invoke(b);
                b.OnCollision?.Invoke(a);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            // Return false.
            return false;
        }

        /// <summary>
        /// Returns the object for the handles.
        /// </summary>
        /// <param name="staticHandle">Static handle to search by.</param>
        /// <param name="bodyHandle">Body handle to search by.</param>
        /// <returns>The physics object for the handles.</returns>
        /// <exception cref="Exception">Failed to find the physics object.</exception>
        private PhysicsObject FindObject(StaticHandle staticHandle, BodyHandle bodyHandle)
        {
            // Return if a body handle matches.
            foreach (var physicsObject in Bodies)
            {
                if (!physicsObject.Reference.Exists) continue;
                if (physicsObject.Id == bodyHandle.Value)
                {
                    return physicsObject;
                }
            }
            
            // Return if a static handle matches.
            foreach (var physicsObject in Statics)
            {
                if (!physicsObject.Reference.Exists) continue;
                if (physicsObject.Id == staticHandle.Value)
                {
                    return physicsObject;
                }
            }

            // Throw an exception if nothing was found.
            throw new Exception($"Failed to find physics object: Got {staticHandle}/{bodyHandle}");
        }
        
        /// <summary>
        /// Disposes the physics simulation.
        /// </summary>
        public void Dispose()
        {
            Simulation?.Dispose();
            Buffer?.Clear();
        }
    }
}