using System.Numerics;
using Uchu.NavMesh.Grid;

namespace Uchu.NavMesh.Shape;

public class UnorderedShape
{
    /// <summary>
    /// Edges of the shape.
    /// </summary>
    public HashSet<Edge> Edges { get; set; } = new HashSet<Edge>();
    
    /// <summary>
    /// Returns a shape from a set of nodes.
    /// </summary>
    /// <param name="node1">Corner 1 of the shape.</param>
    /// <param name="node2">Corner 2 of the shape.</param>
    /// <param name="node3">Corner 3 of the shape.</param>
    /// <param name="node4">Corner 4 of the shape.</param>
    /// <returns>The created shape.</returns>
    public static UnorderedShape? FromNodes(Node node1, Node node2, Node node3, Node node4)
    {
        // Return either a shape of the square or null if the square is not filled.
        if (node1.Neighbors.Contains(node2) && node1.Neighbors.Contains(node3) && node4.Neighbors.Contains(node2) && node4.Neighbors.Contains(node3))
        {
            if (node1.Neighbors.Contains(node4) || node2.Neighbors.Contains(node3))
            {
                return new UnorderedShape()
                {
                    Edges = {
                        new Edge(node1.Position, node2.Position),
                        new Edge(node1.Position, node3.Position),
                        new Edge(node4.Position, node2.Position),
                        new Edge(node4.Position, node3.Position),
                    },
                };
            }
            return null;
        }
        
        // Return a triangle shape.
        if (node1.Neighbors.Contains(node2) && node2.Neighbors.Contains(node3) && node3.Neighbors.Contains(node1))
        {
            // Return a shape without node 4.
            return new UnorderedShape()
            {
                Edges = {
                    new Edge(node1.Position, node2.Position),
                    new Edge(node2.Position, node3.Position),
                    new Edge(node3.Position, node1.Position),
                },
            };
        }
        if (node2.Neighbors.Contains(node3) && node3.Neighbors.Contains(node4) && node4.Neighbors.Contains(node2))
        {
            // Return a shape without node 1.
            return new UnorderedShape()
            {
                Edges = {
                    new Edge(node2.Position, node3.Position),
                    new Edge(node3.Position, node4.Position),
                    new Edge(node4.Position, node2.Position),
                },
            };
        }
        if (node1.Neighbors.Contains(node3) && node3.Neighbors.Contains(node4) && node4.Neighbors.Contains(node1))
        {
            // Return a shape without node 2.
            return new UnorderedShape()
            {
                Edges = {
                    new Edge(node1.Position, node3.Position),
                    new Edge(node3.Position, node4.Position),
                    new Edge(node4.Position, node1.Position),
                },
            };
        }
        if (node1.Neighbors.Contains(node2) && node2.Neighbors.Contains(node4) && node4.Neighbors.Contains(node1))
        {
            // Return a shape without node 3.
            return new UnorderedShape()
            {
                Edges = {
                    new Edge(node1.Position, node2.Position),
                    new Edge(node2.Position, node4.Position),
                    new Edge(node4.Position, node1.Position),
                },
            };
        }
        
        // Return null (not valid).
        return null;
    }
    
    /// <summary>
    /// Returns if a shape can merge.
    /// </summary>
    /// <param name="shape">Shape to check merging.</param>
    /// <returns>Whether the merge can be done.</returns>
    public bool CanMerge(UnorderedShape shape)
    {
        if (shape == this) return false;
        return (from edge in this.Edges from otherEdge in shape.Edges where edge.Equals(otherEdge) select edge).Any();
    }
    
    /// <summary>
    /// Merges another shape.
    /// </summary>
    /// <param name="shape">Shape to merge.</param>
    public void Merge(UnorderedShape shape)
    {
        foreach (var edge in shape.Edges)
        {
            if (this.Edges.Contains(edge))
            {
                this.Edges.Remove(edge);
            }
            else
            {
                this.Edges.Add(edge);
            }
        }
    }
    
    /// <summary>
    /// Returns a list of ordered shapes for the current shape.
    /// </summary>
    /// <returns>Ordered shapes from the current edges.</returns>
    public List<OrderedShape> GetOrderedShapes()
    {
        // Iterate over the edges.
        var orderedShapes = new List<OrderedShape>();
        var remainingEdges = this.Edges.ToList();
        var currentPoints = new List<Vector3>();
        while (remainingEdges.Count != 0)
        {
            if (currentPoints.Count == 0)
            {
                // Add the points of the current edge.
                var edge = remainingEdges[0];
                remainingEdges.RemoveAt(0);
                currentPoints.Add(edge.Start);
                currentPoints.Add(edge.End);
            }
            else
            {
                // Get the next point.
                var lastPoint = currentPoints[^1];
                var nextEdge = remainingEdges.FirstOrDefault(edge => edge.Start == lastPoint || edge.End == lastPoint);
                if (nextEdge == null)
                {
                    currentPoints.RemoveAt(currentPoints.Count - 1);
                    continue;
                }
                remainingEdges.Remove(nextEdge);
                var nextPoint = (nextEdge.Start == lastPoint ? nextEdge.End : nextEdge.Start);
                
                // Add the current points as a shape if a cycle was made.
                if (currentPoints.Contains(nextPoint))
                {
                    var newPoints = new List<Vector2>();
                    var startIndex = currentPoints.IndexOf(nextPoint);
                    for (var i = startIndex; i < currentPoints.Count; i++)
                    {
                        var point = currentPoints[i];
                        newPoints.Add(new Vector2(point.X, point.Z));
                    }
                    orderedShapes.Add(new OrderedShape()
                    {
                        Points = newPoints,
                    });
                    currentPoints.RemoveRange(startIndex, currentPoints.Count - startIndex);
                }
                
                // Add the point.
                currentPoints.Add(nextPoint);
            }
        }

        // Return the ordered shapes.
        return orderedShapes;
    }
}