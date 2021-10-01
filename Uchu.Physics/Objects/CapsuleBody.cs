using System.Numerics;

namespace Uchu.Physics
{
    public class CapsuleBody : PhysicsObject
    {
        /// <summary>
        /// Value used to determine the order in which objects are passed to the collision detection functions.
        /// </summary>
        public override int CollisionPrecedence { get; } = 2;

        /// <summary>
        /// Creates a capsule body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        public CapsuleBody(PhysicsSimulation simulation) : base(simulation)
        {
        }

        /// <summary>
        /// Creates a capsule body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="height">The height of the cylinder part of the capsule.</param>
        /// <returns>The capsule body that was created.</returns>
        public static CapsuleBody Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation,
            float radius, float height)
        {
            var obj = new CapsuleBody(simulation);
            obj.Position = position;
            obj.Rotation = rotation;
            obj.Radius = radius;
            obj.Height = height;
            simulation.Register(obj);
            return obj;
        }

        /// <summary>
        /// Radius of the capsule.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Height of the cylinder part of the capsule.
        /// </summary>
        public float Height;
    }
}
