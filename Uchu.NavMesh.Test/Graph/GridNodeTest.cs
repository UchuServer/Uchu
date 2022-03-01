using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Uchu.NavMesh.Graph;

namespace Uchu.NavMesh.Test.Graph;

public class GridNodeTest
{
    /// <summary>
    /// Test node used with most of the tests.
    /// </summary>
    public GridNode TestNode { get; set; }

    /// <summary>
    /// Sets up the test node.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create the test node.
        this.TestNode = new GridNode(new Vector3(2, 0, 2))
        {
            Neighbors = new List<GridNode>()
            {
                new GridNode(new Vector3(2, 4, 3)),
                new GridNode(new Vector3(3, -2, 3)),
                new GridNode(new Vector3(3, 0, 2)),
                new GridNode(new Vector3(1, 1, 1)),
                new GridNode(new Vector3(1, 1, 2)),
            }
        };
        
        // Set up the neighbors.
        foreach (var neighbor in this.TestNode.Neighbors)
        {
            neighbor.Neighbors.Add(this.TestNode);
        }
    }

    /// <summary>
    /// Tests the RotationTo method.
    /// </summary>
    [Test]
    public void TestRotationTo()
    {
        // Test with itself.
        Assert.AreEqual(0, this.TestNode.RotationTo(this.TestNode.Position));
        
        // Test the eight rotation multipliers.
        Assert.AreEqual(0, this.TestNode.RotationTo(new Vector3(2, 0, 3)));
        Assert.AreEqual(1, this.TestNode.RotationTo(new Vector3(3, 0, 3)));
        Assert.AreEqual(2, this.TestNode.RotationTo(new Vector3(3, 0, 2)));
        Assert.AreEqual(3, this.TestNode.RotationTo(new Vector3(3, 0, 1)));
        Assert.AreEqual(4, this.TestNode.RotationTo(new Vector3(2, 0, 1)));
        Assert.AreEqual(5, this.TestNode.RotationTo(new Vector3(1, 0, 1)));
        Assert.AreEqual(6, this.TestNode.RotationTo(new Vector3(1, 0, 2)));
        Assert.AreEqual(7, this.TestNode.RotationTo(new Vector3(1, 0, 3)));
    }

    /// <summary>
    /// Tests the GetNodeByRotation method.
    /// </summary>
    [Test]
    public void TestGetNodeByRotation()
    {
        Assert.AreEqual(this.TestNode.Neighbors[0], this.TestNode.GetNodeByRotation(0));
        Assert.AreEqual(this.TestNode.Neighbors[1], this.TestNode.GetNodeByRotation(1));
        Assert.AreEqual(this.TestNode.Neighbors[2], this.TestNode.GetNodeByRotation(2));
        Assert.IsNull(this.TestNode.GetNodeByRotation(3));
        Assert.IsNull(this.TestNode.GetNodeByRotation(4));
        Assert.AreEqual(this.TestNode.Neighbors[3], this.TestNode.GetNodeByRotation(5));
        Assert.AreEqual(this.TestNode.Neighbors[4], this.TestNode.GetNodeByRotation(6));
        Assert.IsNull(this.TestNode.GetNodeByRotation(7));
    }

    /// <summary>
    /// Tests the SplitNode method with 2 shapes.
    /// </summary>
    [Test]
    public void TestSplitNodeTwoShapes()
    {
        // Split the nodes and make sure 2 were created.
        var createdNodes = this.TestNode.SplitNode();
        Assert.AreEqual(createdNodes.Count, 2);
        
        // Check the neighbors.
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[0].Neighbors[0]);
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[1].Neighbors[0]);
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[2].Neighbors[0]);
        Assert.AreEqual(createdNodes[1], this.TestNode.Neighbors[3].Neighbors[0]);
        Assert.AreEqual(createdNodes[1], this.TestNode.Neighbors[4].Neighbors[0]);
    }

    /// <summary>
    /// Tests the SplitNode method with 1 shape and 1 extra edge.
    /// </summary>
    [Test]
    public void TestSplitNodeOneShapeOneExtraEdge()
    {
        // Split the nodes and make sure 1 was created.
        this.TestNode.Neighbors.RemoveAt(4);
        var createdNodes = this.TestNode.SplitNode();
        Assert.AreEqual(createdNodes.Count, 1);
        
        // Check the neighbors.
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[0].Neighbors[0]);
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[1].Neighbors[0]);
        Assert.AreEqual(createdNodes[0], this.TestNode.Neighbors[2].Neighbors[0]);
        Assert.AreEqual(0, this.TestNode.Neighbors[3].Neighbors.Count);
    }

    /// <summary>
    /// Tests the SplitNode method with the optimization for 8 all edges.
    /// </summary>
    [Test]
    public void TestSplitNodeAllEdges()
    {
        this.TestNode.Neighbors.Add(new GridNode(new Vector3(1, 0, 3)));
        this.TestNode.Neighbors.Add(new GridNode(new Vector3(2, 0, 1)));
        this.TestNode.Neighbors.Add(new GridNode(new Vector3(3, 0, 1)));
        Assert.AreEqual(this.TestNode,  this.TestNode.SplitNode()[0]);
    }
}