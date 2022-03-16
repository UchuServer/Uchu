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
        Assert.IsTrue(shape.PointInShape(new Vector2(1, 1)));
        Assert.IsTrue(shape.PointInShape(new Vector2(1, -1)));
        Assert.IsTrue(shape.PointInShape(new Vector2(0, 1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(0, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(-3, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(3, -1)));
        Assert.IsFalse(shape.PointInShape(new Vector2(0, 3)));
    }

    /// <summary>
    /// Tests teh LineValid method.
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
}