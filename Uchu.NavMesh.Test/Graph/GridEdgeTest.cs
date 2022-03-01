using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Graph;

namespace Uchu.NavMesh.Test.Graph;

public class GridEdgeTest
{
    /// <summary>
    /// First test edge used.
    /// </summary>
    public GridEdge TestEdge1 = new GridEdge(Vector3.One, Vector3.Zero);
    
    /// <summary>
    /// Second test edge used.
    /// </summary>
    public GridEdge TestEdge2 = new GridEdge(Vector3.Zero, Vector3.One);

    /// <summary>
    /// Tests the Equals method.
    /// </summary>
    [Test]
    public void TestEquals()
    {
        Assert.AreEqual(this.TestEdge1, this.TestEdge1);
        Assert.AreEqual(this.TestEdge2, this.TestEdge2);
        Assert.AreEqual(this.TestEdge1, this.TestEdge2);
    }

    /// <summary>
    /// Tests the GetHashCode method.
    /// </summary>
    [Test]
    public void TestGetHashCode()
    {
        Assert.AreEqual(this.TestEdge1.GetHashCode(), this.TestEdge2.GetHashCode());
    }
}