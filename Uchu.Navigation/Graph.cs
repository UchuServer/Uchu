using System;
using System.Collections;

namespace Uchu.Navigation
{
    /// <summary>
    /// Graph structure. It is defined with :
    /// It is defined with both a list of nodes and a list of arcs.
    /// </summary>
    [Serializable]
    public class Graph
    {
        public ArrayList Ln;
        public ArrayList La;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Graph()
        {
            Ln = new ArrayList();
            La = new ArrayList();
        }

        /// <summary>
        /// Gets the List interface of the nodes in the graph.
        /// </summary>
        public IList Nodes => Ln;

        /// <summary>
        /// Gets the List interface of the arcs in the graph.
        /// </summary>
        public IList Arcs => La;

        /// <summary>
        /// Empties the graph.
        /// </summary>
        public void Clear()
        {
            Ln.Clear();
            La.Clear();
        }

        /// <summary>
        /// Directly Adds a node to the graph.
        /// </summary>
        /// <param name="newNode">The node to add.</param>
        /// <returns>'true' if it has actually been added / 'false' if the node is null or if it is already in the graph.</returns>
        public bool AddNode(Node newNode)
        {
            if (newNode == null || Ln.Contains(newNode)) return false;
            Ln.Add(newNode);
            return true;
        }

        /// <summary>
        /// Creates a node, adds to the graph and returns its reference.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        /// <returns>The reference of the new node / null if the node is already in the graph.</returns>
        public Node AddNode(float x, float y, float z)
        {
            var newNode = new Node(x, y, z);
            return AddNode(newNode) ? newNode : null;
        }

        /// <summary>
        /// Directly Adds an arc to the graph.
        /// </summary>
        /// <exception cref="ArgumentException">Cannot add an arc if one of its extremity nodes does not belong to the graph.</exception>
        /// <param name="newArc">The arc to add.</param>
        /// <returns>'true' if it has actually been added / 'false' if the arc is null or if it is already in the graph.</returns>
        public bool AddArc(Arc newArc)
        {
            if (newArc == null || La.Contains(newArc)) return false;
            if (!Ln.Contains(newArc.StartNode) || !Ln.Contains(newArc.EndNode))
                throw new ArgumentException(
                    "Cannot add an arc if one of its extremity nodes does not belong to the graph.");
            La.Add(newArc);
            return true;
        }

        /// <summary>
        /// Creates an arc between two nodes that are already registered in the graph, adds it to the graph and returns its reference.
        /// </summary>
        /// <exception cref="ArgumentException">Cannot add an arc if one of its extremity nodes does not belong to the graph.</exception>
        /// <param name="startNode">Start node for the arc.</param>
        /// <param name="endNode">End node for the arc.</param>
        /// <param name="weight">Weight for the arc.</param>
        /// <returns>The reference of the new arc / null if the arc is already in the graph.</returns>
        public Arc AddArc(Node startNode, Node endNode, float weight)
        {
            var newArc = new Arc(startNode, endNode);
            newArc.Weight = weight;
            return AddArc(newArc) ? newArc : null;
        }

        /// <summary>
        /// Adds the two opposite arcs between both specified nodes to the graph.
        /// </summary>
        /// <exception cref="ArgumentException">Cannot add an arc if one of its extremity nodes does not belong to the graph.</exception>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="weight"></param>
        public void Add2Arcs(Node node1, Node node2, float weight)
        {
            AddArc(node1, node2, weight);
            AddArc(node2, node1, weight);
        }

        /// <summary>
        /// Removes a node from the graph as well as the linked arcs.
        /// </summary>
        /// <param name="nodeToRemove">The node to remove.</param>
        /// <returns>'true' if succeeded / 'false' otherwise.</returns>
        public bool RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null) return false;
            try
            {
                foreach (Arc a in nodeToRemove.IncomingArcs)
                {
                    a.StartNode.OutgoingArcs.Remove(a);
                    La.Remove(a);
                }

                foreach (Arc a in nodeToRemove.OutgoingArcs)
                {
                    a.EndNode.IncomingArcs.Remove(a);
                    La.Remove(a);
                }

                Ln.Remove(nodeToRemove);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes a node from the graph as well as the linked arcs.
        /// </summary>
        /// <param name="arcToRemove">The arc to remove.</param>
        /// <returns>'true' if succeeded / 'false' otherwise.</returns>
        public bool RemoveArc(Arc arcToRemove)
        {
            if (arcToRemove == null) return false;
            try
            {
                La.Remove(arcToRemove);
                arcToRemove.StartNode.OutgoingArcs.Remove(arcToRemove);
                arcToRemove.EndNode.IncomingArcs.Remove(arcToRemove);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines the bounding box of the entire graph.
        /// </summary>
        /// <exception cref="InvalidOperationException">Impossible to determine the bounding box for this graph.</exception>
        /// <param name="minPoint">The point of minimal coordinates for the box.</param>
        /// <param name="maxPoint">The point of maximal coordinates for the box.</param>
        public void BoundingBox(out double[] minPoint, out double[] maxPoint)
        {
            try
            {
                Node.BoundingBox(Nodes, out minPoint, out maxPoint);
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException("Impossible to determine the bounding box for this graph.\n", e);
            }
        }

        /// <summary>
        /// This function will find the closest node from a geographical position in space.
        /// </summary>
        /// <param name="ptX">X coordinate of the point from which you want the closest node.</param>
        /// <param name="ptY">Y coordinate of the point from which you want the closest node.</param>
        /// <param name="ptZ">Z coordinate of the point from which you want the closest node.</param>
        /// <param name="distance">The distance to the closest node.</param>
        /// <param name="ignorePassableProperty">if 'false', then nodes whose property Passable is set to false will not be taken into account.</param>
        /// <returns>The closest node that has been found.</returns>
        public Node ClosestNode(double ptX, double ptY, double ptZ, out double distance, bool ignorePassableProperty)
        {
            Node nodeMin = null;
            double distanceMin = -1;
            var p = new Point3D(ptX, ptY, ptZ);
            foreach (Node n in Ln)
            {
                if (ignorePassableProperty && n.Passable == false) continue;
                var distanceTemp = Point3D.DistanceBetween(n.Position, p);
                
                if (!distanceMin.Equals(-1) && !(distanceMin > distanceTemp)) continue;
                
                distanceMin = distanceTemp;
                nodeMin = n;
            }

            distance = distanceMin;
            return nodeMin;
        }

        /// <summary>
        /// This function will find the closest arc from a geographical position in space using projection.
        /// </summary>
        /// <param name="ptX">X coordinate of the point from which you want the closest arc.</param>
        /// <param name="ptY">Y coordinate of the point from which you want the closest arc.</param>
        /// <param name="ptZ">Z coordinate of the point from which you want the closest arc.</param>
        /// <param name="distance">The distance to the closest arc.</param>
        /// <param name="ignorePassableProperty">if 'false', then arcs whose property Passable is set to false will not be taken into account.</param>
        /// <returns>The closest arc that has been found.</returns>
        public Arc ClosestArc(double ptX, double ptY, double ptZ, out double distance, bool ignorePassableProperty)
        {
            Arc arcMin = null;
            double distanceMin = -1;
            var p = new Point3D(ptX, ptY, ptZ);
            foreach (Arc a in La)
            {
                if (ignorePassableProperty && a.Passable == false) continue;
                var projection = Point3D.ProjectOnLine(p, a.StartNode.Position, a.EndNode.Position);
                var distanceTemp = Point3D.DistanceBetween(p, projection);
                
                if (!distanceMin.Equals(-1) && !(distanceMin > distanceTemp)) continue;
                
                distanceMin = distanceTemp;
                arcMin = a;
            }

            distance = distanceMin;
            return arcMin;
        }
    }
}