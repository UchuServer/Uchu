using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Grid;
using Uchu.NavMesh.Shape;

namespace Uchu.NavMesh.Test.Shape;

public class UnorderedShapeTest
{
    /// <summary>
    /// Tests the FromNodes method with connected squares.
    /// </summary>
    [Test]
    public void TestFromNodesSquare()
    {
        // Test with unfilled square.
        var node1 = new Node(new Vector3(0, 0, 0));
        var node2 = new Node(new Vector3(0, 0, 1));
        var node3 = new Node(new Vector3(1, 0, 0));
        var node4 = new Node(new Vector3(1, 0, 1));
        node1.Neighbors = new List<Node>()
        {
            node2,
            node3,
        };
        node2.Neighbors = new List<Node>()
        {
            node1,
            node4,
        };
        node3.Neighbors = new List<Node>()
        {
            node1,
            node4,
        };
        node4.Neighbors = new List<Node>()
        {
            node2,
            node3,
        };
        Assert.IsNull(UnorderedShape.FromNodes(node1, node2, node3, node4));
        
        // Test with a filled square.
        node2.Neighbors.Add(node3);
        Assert.AreEqual(4, UnorderedShape.FromNodes(node1, node2, node3, node4)!.Edges.Count);
    }

    /// <summary>
    /// Tests the FromNodes method with connected triangles.
    /// </summary>
    [Test]
    public void TestFromNodesTriangles()
    {
        var node1 = new Node(new Vector3(0, 0, 0));
        var node2 = new Node(new Vector3(0, 0, 1));
        var node3 = new Node(new Vector3(1, 0, 0));
        var node4 = new Node(new Vector3(1, 0, 1));
        node1.Neighbors = new List<Node>()
        {
            node2,
            node3,
        };
        node2.Neighbors = new List<Node>()
        {
            node1,
            node3,
        };
        node3.Neighbors = new List<Node>()
        {
            node1,
            node2,
        };
        
        Assert.AreEqual(3, UnorderedShape.FromNodes(node1, node2, node3, node4)!.Edges.Count);
        Assert.AreEqual(3, UnorderedShape.FromNodes(node2, node3, node4, node1)!.Edges.Count);
        Assert.AreEqual(3, UnorderedShape.FromNodes(node3, node4, node1, node2)!.Edges.Count);
        Assert.AreEqual(3, UnorderedShape.FromNodes(node4, node1, node2, node3)!.Edges.Count);
    }

    /// <summary>
    /// Tests the CanMerge method.
    /// </summary>
    [Test]
    public void TestCanMerge()
    {
        // Test CanMerge on the same shape.
        var shape1 = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(0, 0, 0), new Vector3(1, 0, 1)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            }
        };
        Assert.IsFalse(shape1.CanMerge(shape1));
        
        // Test CanMerge with a common side.
        var shape2 = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(1, 0, 0), new Vector3(1, 0, 1)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            }
        };
        Assert.IsTrue(shape1.CanMerge(shape2));
        
        // Test CanMerge with no common side.
        var shape3 = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(0, 0, 0), new Vector3(2, 0, 2)),
                new Edge(new Vector3(2, 0, 2), new Vector3(0, 0, 2)),
                new Edge(new Vector3(0, 0, 2), new Vector3(0, 0, 0)),
            }
        };
        Assert.IsFalse(shape1.CanMerge(shape3));
    }

    /// <summary>
    /// Tests the Merge method.
    /// </summary>
    [Test]
    public void TestMerge()
    {
        var shape1 = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(0, 0, 0), new Vector3(1, 0, 1)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            }
        };
        var shape2 = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(1, 0, 0), new Vector3(1, 0, 1)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            }
        };
        var originalEdges1 = shape1.Edges.ToList();
        var originalEdges2 = shape2.Edges.ToList();
        shape1.Merge(shape2);

        var edges = shape1.Edges.ToList();
        Assert.IsTrue(edges.Contains(originalEdges1[0]));
        Assert.IsFalse(edges.Contains(originalEdges1[1]));
        Assert.IsTrue(edges.Contains(originalEdges1[2]));
        Assert.IsTrue(edges.Contains(originalEdges2[0]));
        Assert.IsFalse(edges.Contains(originalEdges2[1]));
        Assert.IsTrue(edges.Contains(originalEdges2[2]));
    }

    /// <summary>
    /// Tests the GetOrderedShapes method with a single shape.
    /// </summary>
    [Test]
    public void TestGetOrderedShapesSingleShape()
    {
        var shape = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                new Edge(new Vector3(0, 0, 0), new Vector3(1, 0, 0)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new Edge(new Vector3(1, 0, 1), new Vector3(1, 0, 0)),
            }
        };

        var orderedShapes = shape.GetOrderedShapes();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        }, orderedShapes[0].Points);
    }

    /// <summary>
    /// Tests the GetOrderedShapes method with a two shapes.
    /// </summary>
    [Test]
    public void TestGetOrderedShapesTwoShapes()
    {
        var shape = new UnorderedShape()
        {
            Edges = new HashSet<Edge>()
            {
                // Shape 1.
                new Edge(new Vector3(0, 0, 0), new Vector3(1, 0, 0)),
                new Edge(new Vector3(1, 0, 1), new Vector3(0, 0, 1)),
                new Edge(new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new Edge(new Vector3(1, 0, 1), new Vector3(1, 0, 0)),
                
                // Shape 2.
                new Edge(new Vector3(0, 0, 0), new Vector3(-1, 0, 0)),
                new Edge(new Vector3(-1, 0, 0), new Vector3(-1, 0, -1)),
                new Edge(new Vector3(-1, 0, -1), new Vector3(0, 0, 0)),
            }
        };

        var orderedShapes = shape.GetOrderedShapes();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        }, orderedShapes[0].Points);
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
        }, orderedShapes[1].Points);
    }
}