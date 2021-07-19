using System.Numerics;

namespace Uchu.Physics
{
    public class BoxBody : PhysicsObject
    {
        /// <summary>
        /// Value used to determine the order in which objects are passed to the collision detection functions.
        /// </summary>
        public override int CollisionPrecedence { get; } = 1;

        /// <summary>
        /// Creates a box body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        private BoxBody(PhysicsSimulation simulation) : base(simulation)
        {
        }

        /// <summary>
        /// Creates a box body.
        /// </summary>
        /// <param name="simulation">The simulation to use.</param>
        /// <param name="position">The position of the body.</param>
        /// <param name="rotation">The rotation of the body.</param>
        /// <param name="size">The size of the body (length in x, y and z direction).</param>
        /// <returns>The box body that was created.</returns>
        public static BoxBody Create(PhysicsSimulation simulation, Vector3 position, Quaternion rotation, Vector3 size)
        {
            var obj = new BoxBody(simulation);
            obj.Size = size;
            obj.Position = position;
            obj.Rotation = rotation;
            obj.Vertices = FindVertices(obj);
            simulation.Register(obj);
            return obj;
        }

        /// <summary>
        /// Length of the box in each direction.
        /// </summary>
        public Vector3 Size;

        /// <summary>
        /// Array containing the vertices of the box.
        /// </summary>
        public Vector3[] Vertices;

        /// <summary>
        /// Finds the vertices of a box body.
        /// </summary>
        /// <returns>A List containing the positions of the vertices.</returns>
        public static Vector3[] FindVertices(BoxBody box)
        {
            var vertices = new Vector3[8];

            var boxMinX = - box.Size.X / 2;
            var boxMaxX = + box.Size.X / 2;
            var boxMinY = - box.Size.Y / 2;
            var boxMaxY = + box.Size.Y / 2;
            var boxMinZ = - box.Size.Z / 2;
            var boxMaxZ = + box.Size.Z / 2;

            vertices[0] = new Vector3(boxMinX, boxMinY, boxMinZ);
            vertices[1] = new Vector3(boxMinX, boxMinY, boxMaxZ);
            vertices[2] = new Vector3(boxMinX, boxMaxY, boxMinZ);
            vertices[3] = new Vector3(boxMinX, boxMaxY, boxMaxZ);
            vertices[4] = new Vector3(boxMaxX, boxMinY, boxMinZ);
            vertices[5] = new Vector3(boxMaxX, boxMinY, boxMaxZ);
            vertices[6] = new Vector3(boxMaxX, boxMaxY, boxMinZ);
            vertices[7] = new Vector3(boxMaxX, boxMaxY, boxMaxZ);

            // The resulting vectors are still relative to the axis-aligned box
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Transform(vertices[i], box.Rotation) + box.Position;
            }
            return vertices;
        }

    }
}
