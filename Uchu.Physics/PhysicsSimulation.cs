using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Uchu.Core;

namespace Uchu.Physics
{
    public class PhysicsSimulation
    {
        /// <summary>
        /// All objects in the simulation.
        /// </summary>
        private List<PhysicsObject> Objects { get; }

        /// <summary>
        /// Objects that can move into other objects (e.g. players).
        /// </summary>
        private IEnumerable<PhysicsObject> Colliders => Objects.Where(o => o.CanCollideIntoThings);

        /// <summary>
        /// Objects that are generally static and can be entered by other objects (e.g. POI triggers).
        /// </summary>
        private IEnumerable<PhysicsObject> Collidees => Objects.Where(o => o.CanBeCollidedInto);

        /// <summary>
        /// Creates the physics simulation.
        /// </summary>
        public PhysicsSimulation()
        {
            Objects = new List<PhysicsObject>();
        }

        /// <summary>
        /// Run a collision check.
        /// </summary>
        /// <param name="deltaTime">Delta time in milliseconds since last tick.</param>
        public void Step(float deltaTime)
        {
            foreach (var dynamic in Colliders.ToArray())
            {
                foreach (var other in Collidees.ToArray())
                {
                    var colliding = Collides(dynamic, other);
                    if (colliding)
                    {
                        other.OnCollision?.Invoke(dynamic);
                    }
                }
            }
        }

        /// <summary>
        /// Determine whether two physics objects intersect.
        /// </summary>
        /// <param name="firstObject">The first object</param>
        /// <param name="secondObject">The second object</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Occurs when an unknown type of physics object is passed to the method</exception>
        public static bool Collides(PhysicsObject firstObject, PhysicsObject secondObject)
        {
            PhysicsObject first;
            PhysicsObject second;
            if (firstObject.CollisionPrecedence > secondObject.CollisionPrecedence)
            {
                first = firstObject;
                second = secondObject;
            }
            else
            {
                first = secondObject;
                second = firstObject;
            }

            // First precedence value >= second precedence value
            // Cube = 1, sphere = 0

            if (first is BoxBody box1 && second is BoxBody box2)
                return BoxBoxCollision(box1, box2);
            if (first is BoxBody box && second is SphereBody sphere)
                return BoxSphereCollision(box, sphere);
            if (first is SphereBody sphere1 && second is SphereBody sphere2)
                return SphereSphereCollision(sphere1, sphere2);

            throw new NotSupportedException();
        }

        /// <summary>
        /// Determine whether two boxes intersect.
        /// </summary>
        /// <param name="firstBox"></param>
        /// <param name="secondBox"></param>
        /// <returns></returns>
        public static bool BoxBoxCollision(BoxBody firstBox, BoxBody secondBox)
        {
            // Not implemented. Might not be necessary.
            return false;
        }

        /// <summary>
        /// Determine whether a box and a sphere intersect.
        /// </summary>
        /// <param name="box">Box physics object</param>
        /// <param name="sphere">Sphere physics object</param>
        /// <returns></returns>
        public static bool BoxSphereCollision(BoxBody box, SphereBody sphere)
        {
            // reference frame: box at (0, 0, 0), sides aligned with axes
            var sphereCoordsRelative = sphere.Position - box.Position;

            // to transform coordinate system to have box be axis-aligned, we apply
            // the reverse of the rotation to the sphere coordinates
            var inverse = new Quaternion(box.Rotation.X, box.Rotation.Y, box.Rotation.Z, -box.Rotation.W);
            var sphereCoordsTransformed = Vector3.Transform(sphereCoordsRelative, inverse);

            // to check collision, we need to know if the shortest distance between the box
            // and the center of the sphere is smaller than the radius of the sphere
            // X coordinate of closest point = sphere.X if sphere.x is between the minimum
            // and maximum x coordinate of the box - same for y and z
            var boxMinX = - box.Size.X / 2;
            var boxMaxX = + box.Size.X / 2;
            var boxMinY = - box.Size.Y / 2;
            var boxMaxY = + box.Size.Y / 2;
            var boxMinZ = - box.Size.Z / 2;
            var boxMaxZ = + box.Size.Z / 2;

            var closestPointToSphereX = Math.Clamp(sphereCoordsTransformed.X, boxMinX, boxMaxX);
            var closestPointToSphereY = Math.Clamp(sphereCoordsTransformed.Y, boxMinY, boxMaxY);
            var closestPointToSphereZ = Math.Clamp(sphereCoordsTransformed.Z, boxMinZ, boxMaxZ);

            return Vector3.DistanceSquared(sphereCoordsTransformed, new Vector3(closestPointToSphereX, closestPointToSphereY, closestPointToSphereZ))
                   < Math.Pow(sphere.Radius, 2);
        }

        /// <summary>
        /// Determine whether two spheres intersect.
        /// </summary>
        /// <param name="firstSphere">The first sphere</param>
        /// <param name="secondSphere">The second sphere</param>
        /// <returns></returns>
        public static bool SphereSphereCollision(SphereBody firstSphere, SphereBody secondSphere)
        {
            var maxDistanceSquared = Math.Pow(firstSphere.Radius + secondSphere.Radius, 2);
            var distanceSquared = Vector3.DistanceSquared(firstSphere.Position, secondSphere.Position);
            return distanceSquared < maxDistanceSquared;
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
    }
}
