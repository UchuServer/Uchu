using System.Numerics;

namespace Uchu.NavMesh.Graph;

public class GridNode
{
    /// <summary>
    /// Position of the grid node.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// Neighbors of the node.
    /// </summary>
    public List<GridNode> Neighbors { get; set; } = new List<GridNode>(8);

    /// <summary>
    /// Creates the node.
    /// </summary>
    /// <param name="position">Position of the node.</param>
    public GridNode(Vector3 position)
    {
        this.Position = position;
    }

    /// <summary>
    /// Returns the "rotation" on the XZ plane to a given target. Each increase in 1 represents 45 degrees and is
    /// used since the nodes are on a 2D grid with every neighbor being +/-1 on the X and Z on the grid.
    /// </summary>
    /// <param name="target">Position to target. The Y value is ignored.</param>
    /// <returns>The byte multiplier of the angle.</returns>
    public byte RotationTo(Vector3 target)
    {
        // Return 0 if the position is 0 to avoid a divide-by-zero error.
        if (this.Position.X == target.X && this.Position.Z == target.Z)
        {
            return 0;
        }
        
        // Return the angle.
        var angle = Math.Atan2(target.X - this.Position.X, target.Z - this.Position.Z);
        if (angle < 0)
        {
            angle += (2 * Math.PI);
        }
        return (byte) Math.Round(angle / (Math.PI * 0.25));
    }

    /// <summary>
    /// Returns the neighbor node that is a rotation * 45 degrees from the node.
    /// </summary>
    /// <param name="rotation">Rotation multiplier to use.</param>
    /// <returns>Node at the given rotation.</returns>
    public GridNode? GetNodeByRotation(byte rotation)
    {
        return this.Neighbors.FirstOrDefault(node => this.RotationTo(node.Position) == rotation);
    }

    /// <summary>
    /// Splits the node so that no 2 shapes share the same node instance.
    /// </summary>
    /// <returns>The nodes that were created.</returns>
    public List<GridNode> SplitNode()
    {
        // Get the nodes for each angle.
        var nodesAtAngles = new GridNode?[8];
        var totalNodes = 0;
        for (byte i = 0; i < 8; i++)
        {
            var node = this.GetNodeByRotation(i);
            if (node == null) continue;
            nodesAtAngles[i] = node;
            totalNodes += 1;
        }
        
        // Return if all 8 angles are covered, or there are no nodes to operate on.
        if (totalNodes == 0 || totalNodes == 8)
        {
            return new List<GridNode>(1) { this };
        }
        
        // Determine a starting offset where the first index does not need to be replaced.
        // This ensures that the replaced nodes do not start at the middle.
        var startOffset = 0;
        for (var i = 0; i < 8; i++)
        {
            if (nodesAtAngles[i] != null) continue;
            startOffset = i;
        }
        
        // Replaces the node references of the neighbors.
        var createdNodes = new List<GridNode>(3);
        var newNodesForNeighbor = new GridNode?[8];
        for (var i = 0; i < 8; i++)
        {
            // Get the current neighbor and continue if there is no neighbor to act on.
            var currentIndex = (i + startOffset) % 8;
            var currentNeighbor = nodesAtAngles[currentIndex];
            if (currentNeighbor == null) continue;
            
            // Get the previous and next neighbors.
            // The previous index calculation uses +7 instead of -1 to ensure a positive result.
            var previousIndex = (currentIndex + 7) % 8;
            var nextIndex = (currentIndex + 1) % 8;
            var previousNeighbor = nodesAtAngles[previousIndex];
            var nextNeighbor = nodesAtAngles[nextIndex];
            
            // Remove the edge and continue if there is no previous or next neighbor.
            currentNeighbor.Neighbors.Remove(this);
            if (previousNeighbor == null && nextNeighbor == null) continue;
            
            // Get the new node to use.
            var newNode = newNodesForNeighbor[previousIndex];
            if (newNode == null)
            {
                newNode = new GridNode(this.Position);
                createdNodes.Add(newNode);
            }
            newNodesForNeighbor[currentIndex] = newNode;
            
            // Replace the neighbor.
            currentNeighbor.Neighbors.Add(newNode);
        }

        // Return the created nodes.
        this.Neighbors.Clear();
        return createdNodes;
    }
}