using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Graph;

namespace Uchu.NavMesh.Test.Graph;

public class OrderedPolygonTest
{
    /// <summary>
    /// Tests the Optimize method.
    /// </summary>
    [Test]
    public void TestOptimize()
    {
        var polygon = new OrderedPolygon()
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
        
        polygon.Optimize();
        Assert.AreEqual(new List<Vector2>()
        {
            new Vector2(0, 3),
            new Vector2(1, 4),
            new Vector2(1, -1),
            new Vector2(0, -1),
        }, polygon.Points);
    }
}