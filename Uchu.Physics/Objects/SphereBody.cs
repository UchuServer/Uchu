using System.Numerics;

namespace Uchu.Physics
{
    public class SphereBody : PhysicsObject
    {
        /// <summary>
        /// Value used to determine the order in which objects are passed to the collision detection functions.
        /// </summary>
        public override int CollisionPrecedence { get; } = 0;

        /// <summary>
        /// Creates a sphere body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        private SphereBody(PhysicsSimulation simulation) : base(simulation)
        {
        }

        /// <summary>
        /// Creates a sphere body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>The sphere body that was created.</returns>
        public static SphereBody Create(PhysicsSimulation simulation, Vector3 position, float radius)
        {
            var obj = new SphereBody(simulation);
            obj.Position = position;
            obj.Radius = radius;
            simulation.Register(obj);
            return obj;
        }

        /// <summary>
        /// Radius of the sphere.
        /// </summary>
        public float Radius;
    }
}
