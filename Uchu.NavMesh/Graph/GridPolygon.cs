using System.Numerics;

namespace Uchu.NavMesh.Graph;

public class GridPolygon
{
    /// <summary>
    /// Edges of the polygon.
    /// </summary>
    public HashSet<GridEdge> Edges { get; set; } = new HashSet<GridEdge>();
    
    /// <summary>
    /// Returns a polygon from a set of nodes.
    /// </summary>
    /// <param name="node1">Corner 1 of the polygon.</param>
    /// <param name="node2">Corner 2 of the polygon.</param>
    /// <param name="node3">Corner 3 of the polygon.</param>
    /// <param name="node4">Corner 4 of the polygon.</param>
    /// <returns>The created polygon.</returns>
    public static GridPolygon? FromNodes(GridNode node1, GridNode node2, GridNode node3, GridNode node4)
    {
        // Return either a polygon of the square or null if the square is not filled.
        if (node1.Neighbors.Contains(node2) && node1.Neighbors.Contains(node3) && node4.Neighbors.Contains(node2) && node4.Neighbors.Contains(node3))
        {
            if (node1.Neighbors.Contains(node4) || node2.Neighbors.Contains(node3))
            {
                return new GridPolygon()
                {
                    Edges = {
                        new GridEdge(node1.Position, node2.Position),
                        new GridEdge(node1.Position, node3.Position),
                        new GridEdge(node4.Position, node2.Position),
                        new GridEdge(node4.Position, node3.Position),
                    },
                };
            }
            return null;
        }
        
        // Return a triangle polygon.
        if (node1.Neighbors.Contains(node2) && node2.Neighbors.Contains(node3) && node3.Neighbors.Contains(node1))
        {
            // Return a polygon without node 4.
            return new GridPolygon()
            {
                Edges = {
                    new GridEdge(node1.Position, node2.Position),
                    new GridEdge(node2.Position, node3.Position),
                    new GridEdge(node3.Position, node1.Position),
                },
            };
        }
        if (node2.Neighbors.Contains(node3) && node3.Neighbors.Contains(node4) && node4.Neighbors.Contains(node2))
        {
            // Return a polygon without node 1.
            return new GridPolygon()
            {
                Edges = {
                    new GridEdge(node2.Position, node3.Position),
                    new GridEdge(node3.Position, node4.Position),
                    new GridEdge(node4.Position, node2.Position),
                },
            };
        }
        if (node1.Neighbors.Contains(node3) && node3.Neighbors.Contains(node4) && node4.Neighbors.Contains(node1))
        {
            // Return a polygon without node 2.
            return new GridPolygon()
            {
                Edges = {
                    new GridEdge(node1.Position, node3.Position),
                    new GridEdge(node3.Position, node4.Position),
                    new GridEdge(node4.Position, node1.Position),
                },
            };
        }
        if (node1.Neighbors.Contains(node2) && node2.Neighbors.Contains(node4) && node4.Neighbors.Contains(node1))
        {
            // Return a polygon without node 3.
            return new GridPolygon()
            {
                Edges = {
                    new GridEdge(node1.Position, node2.Position),
                    new GridEdge(node2.Position, node4.Position),
                    new GridEdge(node4.Position, node1.Position),
                },
            };
        }
        
        // Return null (not valid).
        return null;
    }
    
    /// <summary>
    /// Returns if a polygon can merge.
    /// </summary>
    /// <param name="polygon">Polygon to check merging.</param>
    /// <returns>Whether the merge can be done.</returns>
    public bool CanMerge(GridPolygon polygon)
    {
        if (polygon == this) return false;
        return (from edge in this.Edges from otherEdge in polygon.Edges where edge.Equals(otherEdge) select edge).Any();
    }
    
    /// <summary>
    /// Merges another polygon.
    /// </summary>
    /// <param name="polygon">Polygon to merge.</param>
    public void Merge(GridPolygon polygon)
    {
        foreach (var edge in polygon.Edges)
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
    /// Returns a list of ordered polygons for the current polygon.
    /// </summary>
    /// <returns>Ordered polygons from the current edges.</returns>
    public List<OrderedPolygon> GetOrderedPolygons()
    {
        // Iterate over the edges.
        var orderedPolygons = new List<OrderedPolygon>();
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
                    orderedPolygons.Add(new OrderedPolygon()
                    {
                        Points = newPoints,
                    });
                    currentPoints.RemoveRange(startIndex, currentPoints.Count - startIndex);
                }
                
                // Add the point.
                currentPoints.Add(nextPoint);
            }
        }

        // Return the ordered polygons.
        return orderedPolygons;
    }
}