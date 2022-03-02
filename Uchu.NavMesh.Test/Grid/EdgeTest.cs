using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Grid;

namespace Uchu.NavMesh.Test.Grid;

public class EdgeTest
{
    /// <summary>
    /// First test edge used.
    /// </summary>
    public Edge TestEdge1 = new Edge(Vector3.One, Vector3.Zero);
    
    /// <summary>
    /// Second test edge used.
    /// </summary>
    public Edge TestEdge2 = new Edge(Vector3.Zero, Vector3.One);

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