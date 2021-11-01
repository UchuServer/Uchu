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
            // Capsule = 2, cube = 1, sphere = 0

            if (first is BoxBody boxSphereFirst && second is SphereBody boxSphereSecond)
                return BoxSphereCollision(boxSphereFirst, boxSphereSecond);
            if (first is SphereBody sphereSphereFirst && second is SphereBody sphereSphereSecond)
                return SphereSphereCollision(sphereSphereFirst, sphereSphereSecond);
            if (first is BoxBody boxBoxFirst && second is BoxBody boxBoxSecond)
                return BoxBoxCollision(boxBoxFirst, boxBoxSecond);
            if (first is CapsuleBody capsuleSphereFirst && second is SphereBody capsuleSphereSecond)
                return CapsuleSphereCollision(capsuleSphereFirst, capsuleSphereSecond);
            if (first is CapsuleBody capsuleBoxFirst && second is BoxBody capsuleBoxSecond)
                return CapsuleBoxCollision(capsuleBoxFirst, capsuleBoxSecond);
            if (first is CapsuleBody capsuleCapsuleFirst && second is CapsuleBody capsuleCapsuleSecond)
                return CapsuleCapsuleCollision(capsuleCapsuleFirst, capsuleCapsuleSecond);

            throw new NotSupportedException();
        }

        /// <summary>
        /// Determine whether two boxes intersect.
        /// </summary>
        /// <remarks>
        /// Not 100% accurate; it only checks vertex-face collisions, not edge-edge.
        /// However, this should be accurate enough for our use case.
        /// </remarks>
        /// <param name="firstBox">The first box</param>
        /// <param name="secondBox">The second box</param>
        public static bool BoxBoxCollision(BoxBody firstBox, BoxBody secondBox)
        {
            return firstBox.ContainsAnyPoint(secondBox.Vertices) || secondBox.ContainsAnyPoint(firstBox.Vertices);
        }

        /// <summary>
        /// Determine whether a box and a sphere intersect.
        /// </summary>
        /// <param name="box">Box physics object</param>
        /// <param name="sphere">Sphere physics object</param>
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
        public static bool SphereSphereCollision(SphereBody firstSphere, SphereBody secondSphere)
        {
            var maxDistanceSquared = Math.Pow(firstSphere.Radius + secondSphere.Radius, 2);
            var distanceSquared = Vector3.DistanceSquared(firstSphere.Position, secondSphere.Position);
            return distanceSquared < maxDistanceSquared;
        }

        /// <summary>
        /// Determine whether a capsule and a sphere intersect.
        /// </summary>
        /// <param name="capsule">Capsule physics object</param>
        /// <param name="sphere">Sphere physics object</param>
        public static bool CapsuleSphereCollision(CapsuleBody capsule, SphereBody sphere)
        {
            // reference frame: capsule at (0, 0, 0), pointing in positive y direction
            var sphereCoordsRelative = sphere.Position - capsule.Position;

            // to transform coordinate system to have capsule be axis-aligned, we apply
            // the reverse of the rotation to the sphere coordinates
            var inverse = new Quaternion(capsule.Rotation.X, capsule.Rotation.Y, capsule.Rotation.Z, -capsule.Rotation.W);
            var sphereCoordsTransformed = Vector3.Transform(sphereCoordsRelative, inverse);

            // coordinates of the line in the centre of the cylinder part
            const int capsuleMinY = 0;
            var capsuleMaxY = capsule.Height;

            const int closestPointToSphereX = 0;
            var closestPointToSphereY = Math.Clamp(sphereCoordsTransformed.Y, capsuleMinY, capsuleMaxY);
            const int closestPointToSphereZ = 0;

            return Vector3.DistanceSquared(sphereCoordsTransformed,
                       new Vector3(closestPointToSphereX, closestPointToSphereY, closestPointToSphereZ))
                   < Math.Pow(capsule.Radius + sphere.Radius, 2);
        }

        /// <summary>
        /// Determine whether two capsules intersect.
        /// </summary>
        /// <param name="firstCapsule">The first capsule</param>
        /// <param name="secondCapsule">The second capsule</param>
        public static bool CapsuleCapsuleCollision(CapsuleBody firstCapsule, CapsuleBody secondCapsule)
        {
            throw new NotImplementedException("Capsule-capsule collision checks are not implemented.");
        }

        /// <summary>
        /// Determine whether a capsule and a box intersect.
        /// </summary>
        /// <param name="capsule">The capsule</param>
        /// <param name="box">The box</param>
        public static bool CapsuleBoxCollision(CapsuleBody capsule, BoxBody box)
        {
            throw new NotImplementedException("Capsule-box collision checks are not implemented.");
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
