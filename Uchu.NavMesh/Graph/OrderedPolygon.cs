using System.Numerics;

namespace Uchu.NavMesh.Graph;

public class OrderedPolygon
{
    /// <summary>
    /// Points of the ordered polygon.
    /// </summary>
    public List<Vector2> Points { get; set; } = new List<Vector2>();
}