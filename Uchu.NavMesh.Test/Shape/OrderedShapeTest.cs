using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Shape;

namespace Uchu.NavMesh.Test.Shape;

public class OrderedShapeTest
{
    /// <summary>
    /// Tests the Optimize method.
    /// </summary>
    [Test]
    public void TestOptimize()
    {
        // Create the test shape.
        var shape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(0, 2),
                new Vector2(0, 3),
                new Vector2(1, 4),
                new Vector2(1, 3),
                new Vector2(1, 2),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(1, -1),
                new Vector2(0, -1),
            },
        };
        
        // Test optimizing the shape.
        shape.Optimize();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 3),
            new Vector2(1, 4),
            new Vector2(1, -1),
            new Vector2(0, -1),
        }, shape.Points);
    }

    /// <summary>
    /// Tests the PointInShape method.
    /// </summary>
    [Test]
    public void TestPointInShape()
    {
        // Create the test shape.
        var shape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(0, 0),
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2),
            },
        };
        
        // Test that various parts are in the shape.
        Assert.IsTrue(shape.PointInShape(new Vector2(0, 0)));
        Assert.IsTrue(shape.PointInShape(new Vector2(0.5f, 1)));
        Assert.IsTrue(shape.PointInShape(new Vector2(1.5f, -1)));
        Assert.IsTrue(shape.PointInShape(new Vector2(0, 1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(0, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(-3, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(3, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(0, 3)));
        
        // Test edges cases where the point is inline with the lines.
        Assert.IsTrue(shape.PointInShape(new Vector2(-1, 0)));
        Assert.IsTrue(shape.PointInShape(new Vector2(1, 0)));
        Assert.IsFalse(shape.PointInShape(new Vector2(-3, 0)));
        Assert.IsFalse(shape.PointInShape(new Vector2(3, 0)));
    }

    /// <summary>
    /// Tests the LineValid method.
    /// </summary>
    [Test]
    public void TestLineValid()
    {
        // Create the test shape.
        var shape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(0, 0),
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2),
            },
        };
        
        // Test with lines that make up the shape.
        Assert.IsTrue(shape.LineValid(new Vector2(0, 0), new Vector2(-2, -2)));
        Assert.IsTrue(shape.LineValid(new Vector2(2, 2), new Vector2(-2, 2)));
        
        // Test with lines completely inside or outside the shape.
        Assert.IsTrue(shape.LineValid(new Vector2(-1, 1), new Vector2(1, 1)));
        Assert.IsFalse(shape.LineValid(new Vector2(-2, -2), new Vector2(2, -2)));
        
        // Test with intersections.
        Assert.IsFalse(shape.LineValid(new Vector2(-1, -1), new Vector2(1, -1)));
        Assert.IsFalse(shape.LineValid(new Vector2(-2, -2), new Vector2(2, 2)));
    }

    /// <summary>
    /// Tests the LineValid method with a containing shape..
    /// </summary>
    [Test]
    public void TestLineValidContainingShape()
    {
        // Create the test shape.
        var shape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2),
            },
            Shapes = new List<OrderedShape>()
            {
                new OrderedShape()
                {
                    Points = new List<Vector2>()
                    {
                        new Vector2(-1, -1),
                        new Vector2(-1, 1),
                        new Vector2(1, 1),
                        new Vector2(1, -1),
                    },
                }
            }
        };
        
        // Test with lines that make up the shape.
        Assert.IsTrue(shape.LineValid(new Vector2(-2, 2), new Vector2(-2, -2)));
        Assert.IsTrue(shape.LineValid(new Vector2(2, 2), new Vector2(-2, 2)));
        
        // Test with lines completely inside or outside the shape.
        Assert.IsTrue(shape.LineValid(new Vector2(-1, 1.5f), new Vector2(1, 1.5f)));
        Assert.IsFalse(shape.LineValid(new Vector2(-3, -3), new Vector2(3, -3)));
        
        // Test with lines that are part of the inner shape.
        Assert.IsTrue(shape.LineValid(new Vector2(-1, -1), new Vector2(-1, 1)));
        Assert.IsTrue(shape.LineValid(new Vector2(1, -1), new Vector2(1, 1)));
        
        // Test with lines that intersect the inner shape.
        Assert.IsFalse(shape.LineValid(new Vector2(-2, -2), new Vector2(2, 2)));
        Assert.IsFalse(shape.LineValid(new Vector2(-2, 2), new Vector2(2, -2)));
        
        // Test with lines inside the inner shape.
        Assert.IsFalse(shape.LineValid(new Vector2(-1, -1), new Vector2(1, 1)));
        Assert.IsFalse(shape.LineValid(new Vector2(-1, 1), new Vector2(-1, 1)));
        
        // Test with intersections.
        Assert.IsFalse(shape.LineValid(new Vector2(-3, -1), new Vector2(-1, -1)));
    }

    /// <summary>
    /// Tests the TryAddShape method.
    /// </summary>
    [Test]
    public void TestTryAddShape()
    {
        // Create several rectangles.
        var shape1 = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(-1, 0),
                new Vector2(-1, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            },
        };
        var shape2 = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(-1, 0),
                new Vector2(-1, -1),
                new Vector2(1, -1),
                new Vector2(1, 0),
            },
        };
        var shape3 = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2),
            },
        };
        var shape4 = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(-3, -3),
                new Vector2(-3, 3),
                new Vector2(3, 3),
                new Vector2(3, -3),
            },
        };
        
        // Assert certain shapes that can't be added.
        Assert.IsFalse(shape1.TryAddShape(shape2));
        Assert.IsFalse(shape1.TryAddShape(shape3));
        
        // Assert adding shapes.
        Assert.IsTrue(shape4.TryAddShape(shape1));
        Assert.IsTrue(shape4.TryAddShape(shape3));
        Assert.IsTrue(shape4.TryAddShape(shape2));
        
        // Assert the correct shapes are stored.
        Assert.AreEqual(new List<OrderedShape>(), shape1.Shapes);
        Assert.AreEqual(new List<OrderedShape>(), shape2.Shapes);
        Assert.AreEqual(new List<OrderedShape>() {shape1, shape2}, shape3.Shapes);
        Assert.AreEqual(new List<OrderedShape>() {shape3}, shape4.Shapes);
    }

    /// <summary>
    /// Tests the GenerateGraph method.
    /// </summary>
    [Test]
    public void TestGenerateGraph()
    {
        // Create the test shape.
        var shape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(0, -1),
                new Vector2(-2, -2),
                new Vector2(-2, 2),
                new Vector2(2, 2),
                new Vector2(2, -2),
            },
        };
        shape.GenerateGraph();
        
        // Test that the connected nodes are correct.
        Assert.AreEqual(4, shape.Nodes[0].Nodes.Count);
        Assert.AreEqual(new Vector2(-2, -2), shape.Nodes[0].Nodes[0].Point);
        Assert.AreEqual(new Vector2(-2, 2), shape.Nodes[0].Nodes[1].Point);
        Assert.AreEqual(new Vector2(2, 2), shape.Nodes[0].Nodes[2].Point);
        Assert.AreEqual(new Vector2(2, -2), shape.Nodes[0].Nodes[3].Point);
        Assert.AreEqual(3, shape.Nodes[1].Nodes.Count);
        Assert.AreEqual(new Vector2(0, -1), shape.Nodes[1].Nodes[0].Point);
        Assert.AreEqual(new Vector2(-2, 2), shape.Nodes[1].Nodes[1].Point);
        Assert.AreEqual(new Vector2(2, 2), shape.Nodes[1].Nodes[2].Point);
        Assert.AreEqual(4, shape.Nodes[2].Nodes.Count);
        Assert.AreEqual(4, shape.Nodes[3].Nodes.Count);
        Assert.AreEqual(3, shape.Nodes[4].Nodes.Count);
    }
}