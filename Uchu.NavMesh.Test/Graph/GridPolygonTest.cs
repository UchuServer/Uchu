using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Graph;

namespace Uchu.NavMesh.Test.Graph;

public class GridPolygonTest
{
    /// <summary>
    /// Tests the FromNodes method with connected squares.
    /// </summary>
    [Test]
    public void TestFromNodesSquare()
    {
        // Test with unfilled square.
        var node1 = new GridNode(new Vector3(0, 0, 0));
        var node2 = new GridNode(new Vector3(0, 0, 1));
        var node3 = new GridNode(new Vector3(1, 0, 0));
        var node4 = new GridNode(new Vector3(1, 0, 1));
        node1.Neighbors = new List<GridNode>()
        {
            node2,
            node3,
        };
        node2.Neighbors = new List<GridNode>()
        {
            node1,
            node4,
        };
        node3.Neighbors = new List<GridNode>()
        {
            node1,
            node4,
        };
        node4.Neighbors = new List<GridNode>()
        {
            node2,
            node3,
        };
        Assert.IsNull(GridPolygon.FromNodes(node1, node2, node3, node4));
        
        // Test with a filled square.
        node2.Neighbors.Add(node3);
        Assert.AreEqual(4, GridPolygon.FromNodes(node1, node2, node3, node4)!.Edges.Count);
    }

    /// <summary>
    /// Tests the FromNodes method with connected triangles.
    /// </summary>
    [Test]
    public void TestFromNodesTriangles()
    {
        var node1 = new GridNode(new Vector3(0, 0, 0));
        var node2 = new GridNode(new Vector3(0, 0, 1));
        var node3 = new GridNode(new Vector3(1, 0, 0));
        var node4 = new GridNode(new Vector3(1, 0, 1));
        node1.Neighbors = new List<GridNode>()
        {
            node2,
            node3,
        };
        node2.Neighbors = new List<GridNode>()
        {
            node1,
            node3,
        };
        node3.Neighbors = new List<GridNode>()
        {
            node1,
            node2,
        };
        
        Assert.AreEqual(3, GridPolygon.FromNodes(node1, node2, node3, node4)!.Edges.Count);
        Assert.AreEqual(3, GridPolygon.FromNodes(node2, node3, node4, node1)!.Edges.Count);
        Assert.AreEqual(3, GridPolygon.FromNodes(node3, node4, node1, node2)!.Edges.Count);
        Assert.AreEqual(3, GridPolygon.FromNodes(node4, node1, node2, node3)!.Edges.Count);
    }

    /// <summary>
    /// Tests the CanMerge method.
    /// </summary>
    [Test]
    public void TestCanMerge()
    {
        // Test CanMerge on the same polygon.
        var polygon1 = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(0, 0, 0), new Vector3(1, 0, 1)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            }
        };
        Assert.IsFalse(polygon1.CanMerge(polygon1));
        
        // Test CanMerge with a common side.
        var polygon2 = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(1, 0, 0), new Vector3(1, 0, 1)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            }
        };
        Assert.IsTrue(polygon1.CanMerge(polygon2));
        
        // Test CanMerge with no common side.
        var polygon3 = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(0, 0, 0), new Vector3(2, 0, 2)),
                new GridEdge(new Vector3(2, 0, 2), new Vector3(0, 0, 2)),
                new GridEdge(new Vector3(0, 0, 2), new Vector3(0, 0, 0)),
            }
        };
        Assert.IsFalse(polygon1.CanMerge(polygon3));
    }

    /// <summary>
    /// Tests the Merge method.
    /// </summary>
    [Test]
    public void TestMerge()
    {
        var polygon1 = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(0, 0, 0), new Vector3(1, 0, 1)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            }
        };
        var polygon2 = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(1, 0, 0), new Vector3(1, 0, 1)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            }
        };
        var originalEdges1 = polygon1.Edges.ToList();
        var originalEdges2 = polygon2.Edges.ToList();
        polygon1.Merge(polygon2);

        var edges = polygon1.Edges.ToList();
        Assert.IsTrue(edges.Contains(originalEdges1[0]));
        Assert.IsFalse(edges.Contains(originalEdges1[1]));
        Assert.IsTrue(edges.Contains(originalEdges1[2]));
        Assert.IsTrue(edges.Contains(originalEdges2[0]));
        Assert.IsFalse(edges.Contains(originalEdges2[1]));
        Assert.IsTrue(edges.Contains(originalEdges2[2]));
    }

    /// <summary>
    /// Tests the GetOrderedPolygonsSingleShape method with a single shape.
    /// </summary>
    [Test]
    public void TestGetOrderedPolygonsSingleShape()
    {
        var polygon = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                new GridEdge(new Vector3(0, 0, 0), new Vector3(1, 0, 0)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(1, 0, 0)),
            }
        };

        var orderedPolygons = polygon.GetOrderedPolygons();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        }, orderedPolygons[0].Points);
    }

    /// <summary>
    /// Tests the GetOrderedPolygonsSingleShape method with a two shapes.
    /// </summary>
    [Test]
    public void TestGetOrderedPolygonsTwoShapes()
    {
        var polygon = new GridPolygon()
        {
            Edges = new HashSet<GridEdge>()
            {
                // Shape 1.
                new GridEdge(new Vector3(0, 0, 0), new Vector3(1, 0, 0)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new GridEdge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new GridEdge(new Vector3(1, 0, 1), new Vector3(1, 0, 0)),
                
                // Shape 2.
                new GridEdge(new Vector3(0, 0, 0), new Vector3(-1, 0, 0)),
                new GridEdge(new Vector3(-1, 0, 0), new Vector3(-1, 0, -1)),
                new GridEdge(new Vector3(-1, 0, -1), new Vector3(0, 0, 0)),
            }
        };

        var orderedPolygons = polygon.GetOrderedPolygons();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        }, orderedPolygons[0].Points);
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
        }, orderedPolygons[1].Points);
    }
}