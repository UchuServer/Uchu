using System.Numerics;

namespace Uchu.NavMesh.Graph;

public class Node
{
    /// <summary>
    /// Point of the node.
    /// </summary>
    public Vector2 Point { get; set; }

    /// <summary>
    /// Nodes that are connected.
    /// </summary>
    public List<Node> Nodes { get; set; } = new List<Node>();

    /// <summary>
    /// Creates the node.
    /// </summary>
    /// <param name="point">Point of the node.</param>
    public Node(Vector2 point)
    {
        this.Point = point;
    }
}