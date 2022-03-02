using System.Numerics;

namespace Uchu.NavMesh.Grid;

public class Node
{
    /// <summary>
    /// Position of the grid node.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Neighbors of the node.
    /// </summary>
    public List<Node> Neighbors { get; set; } = new List<Node>(8);

    /// <summary>
    /// Creates the node.
    /// </summary>
    /// <param name="position">Position of the node.</param>
    public Node(Vector3 position)
    {
        this.Position = position;
    }
}