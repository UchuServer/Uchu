using System;
using System.Collections;

namespace Uchu.Navigation
{
	/// <summary>
	/// Basically a node is defined with a geographical position in space.
	/// It is also characterized with both collections of outgoing arcs and incoming arcs.
	/// </summary>
	[Serializable]
	public class Node
	{
		private Point3D _position;
		private bool _passable;
		private ArrayList _incomingArcs, _outgoingArcs;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="positionX">X coordinate.</param>
		/// <param name="positionY">Y coordinate.</param>
		/// <param name="positionZ">Z coordinate.</param>
		public Node(double positionX, double positionY, double positionZ)
		{
			_position = new Point3D(positionX, positionY, positionZ);
			_passable = true;
			_incomingArcs = new ArrayList();
			_outgoingArcs = new ArrayList();
		}

		/// <summary>
		/// Gets the list of the arcs that lead to this node.
		/// </summary>
		public IList IncomingArcs => _incomingArcs;

		/// <summary>
		/// Gets the list of the arcs that start from this node.
		/// </summary>
		public IList OutgoingArcs => _outgoingArcs;

		/// Gets/Sets the functional state of the node.
		/// 'true' means that the node is in its normal state.
		/// 'false' means that the node will not be taken into account (as if it did not exist).
		public bool Passable
		{
			set
			{
				foreach (Arc a in _incomingArcs) a.Passable = value;
				foreach (Arc a in _outgoingArcs) a.Passable = value;
				_passable = value;
			}
			get => _passable;
		}

		/// <summary>
		/// Gets X coordinate.
		/// </summary>
		public double X => Position.X;

		/// <summary>
		/// Gets Y coordinate.
		/// </summary>
		public double Y => Position.Y;

		/// <summary>
		/// Gets Z coordinate.
		/// </summary>
		public double Z => Position.Z;

		/// <summary>
		/// Modifies X, Y and Z coordinates
		/// </summary>
		/// <param name="positionX">X coordinate.</param>
		/// <param name="positionY">Y coordinate.</param>
		/// <param name="positionZ">Z coordinate.</param>
		public void ChangeXyz(double positionX, double positionY, double positionZ)
		{
			Position = new Point3D(positionX, positionY, positionZ);
		}

		/// <summary>
		/// Gets/Sets the geographical position of the node.
		/// </summary>
		/// <exception cref="ArgumentNullException">Cannot set the Position to null.</exception>
		public Point3D Position
		{
			set
			{
				if ( value==null ) throw new ArgumentNullException();
				foreach (Arc a in _incomingArcs) a.LengthUpdated = false;
				foreach (Arc a in _outgoingArcs) a.LengthUpdated = false;
				_position = value;
			}
			get => _position;
		}

		/// <summary>
		/// Gets the array of nodes that can be directly reached from this one.
		/// </summary>
		public Node[] AccessibleNodes
		{
			get
			{
				var tableau = new Node[_outgoingArcs.Count];
				var i=0;
				foreach (Arc a in OutgoingArcs) tableau[i++] = a.EndNode;
				return tableau;
			}
		}

		/// <summary>
		/// Gets the array of nodes that can directly reach this one.
		/// </summary>
		public Node[] AccessingNodes
		{
			get
			{
				var tableau = new Node[_incomingArcs.Count];
				var i=0;
				foreach (Arc a in IncomingArcs) tableau[i++] = a.StartNode;
				return tableau;
			}
		}
		
		/// <summary>
		/// Gets the array of nodes directly linked plus this one.
		/// </summary>
		public Node[] Molecule
		{
			get
			{
				var nbNodes = 1+_outgoingArcs.Count+_incomingArcs.Count;
				var tableau = new Node[nbNodes];
				tableau[0] = this;
				var i=1;
				foreach (Arc a in OutgoingArcs) tableau[i++] = a.EndNode;
				foreach (Arc a in IncomingArcs) tableau[i++] = a.StartNode;
				return tableau;
			}
		}
		
		/// <summary>
		/// Unlink this node from all current connected arcs.
		/// </summary>
		public void Isolate()
		{
			UntieIncomingArcs();
			UntieOutgoingArcs();
		}

		/// <summary>
		/// Unlink this node from all current incoming arcs.
		/// </summary>
		public void UntieIncomingArcs()
		{
			foreach (Arc a in _incomingArcs)
				a.StartNode.OutgoingArcs.Remove(a);
			_incomingArcs.Clear();
		}

		/// <summary>
		/// Unlink this node from all current outgoing arcs.
		/// </summary>
		public void UntieOutgoingArcs()
		{
			foreach (Arc a in _outgoingArcs)
				a.EndNode.IncomingArcs.Remove(a);
			_outgoingArcs.Clear();
		}

		/// <summary>
		/// Returns the arc that leads to the specified node if it exists.
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument node must not be null.</exception>
		/// <param name="n">A node that could be reached from this one.</param>
		/// <returns>The arc leading to N from this / null if there is no solution.</returns>
		public Arc ArcGoingTo(Node n)
		{
			if ( n==null ) throw new ArgumentNullException();
			foreach (Arc a in _outgoingArcs)
				if (a.EndNode == n) return a;
			return null;
		}

		/// <summary>
		/// Returns the arc that arc that comes to this from the specified node if it exists.
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument node must not be null.</exception>
		/// <param name="n">A node that could reach this one.</param>
		/// <returns>The arc coming to this from N / null if there is no solution.</returns>
		public Arc ArcComingFrom(Node n)
		{
			if ( n==null ) throw new ArgumentNullException();
			foreach (Arc a in _incomingArcs)
				if (a.StartNode == n) return a;
			return null;
		}

		private void Invalidate()
		{
			foreach (Arc a in _incomingArcs) a.LengthUpdated = false;
			foreach (Arc a in _outgoingArcs) a.LengthUpdated = false;
		}

		/// <summary>
		/// object.ToString() override.
		/// Returns the textual description of the node.
		/// </summary>
		/// <returns>String describing this node.</returns>
		public override string ToString() { return Position.ToString(); }

		/// <summary>
		/// Object.Equals override.
		/// Tells if two nodes are equal by comparing positions.
		/// </summary>
		/// <exception cref="ArgumentException">A Node cannot be compared with another type.</exception>
		/// <param name="o">The node to compare with.</param>
		/// <returns>'true' if both nodes are equal.</returns>
		public override bool Equals(object o)
		{
			var n = (Node)o;
			if ( n==null ) throw new ArgumentException("Type "+o.GetType()+" cannot be compared with type "+GetType()+" !");
			return Position.Equals(n.Position);
		}

		/// <summary>
		/// Returns a copy of this node.
		/// </summary>
		/// <returns>The reference of the new object.</returns>
		public object Clone()
		{
			var n = new Node(X, Y, Z);
			n._passable = _passable;
			return n;
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// </summary>
		/// <returns>HashCode value.</returns>
		public override int GetHashCode() { return Position.GetHashCode(); }

		/// <summary>
		/// Returns the euclidian distance between two nodes : Sqrt(Dx�+Dy�+Dz�)
		/// </summary>
		/// <param name="n1">First node.</param>
		/// <param name="n2">Second node.</param>
		/// <returns>Distance value.</returns>
		public static double EuclidianDistance(Node n1, Node n2)
		{
			return Math.Sqrt( SquareEuclidianDistance(n1, n2) );
		}

		/// <summary>
		/// Returns the square euclidian distance between two nodes : Dx�+Dy�+Dz�
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument nodes must not be null.</exception>
		/// <param name="n1">First node.</param>
		/// <param name="n2">Second node.</param>
		/// <returns>Distance value.</returns>
		public static double SquareEuclidianDistance(Node n1, Node n2)
		{
			if ( n1==null || n2==null ) throw new ArgumentNullException();
			var dx = n1.Position.X - n2.Position.X;
			var dy = n1.Position.Y - n2.Position.Y;
			var dz = n1.Position.Z - n2.Position.Z;
			return dx*dx+dy*dy+dz*dz;
		}

		/// <summary>
		/// Returns the manhattan distance between two nodes : |Dx|+|Dy|+|Dz|
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument nodes must not be null.</exception>
		/// <param name="n1">First node.</param>
		/// <param name="n2">Second node.</param>
		/// <returns>Distance value.</returns>
		public static double ManhattanDistance(Node n1, Node n2)
		{
			if ( n1==null || n2==null ) throw new ArgumentNullException();
			var dx = n1.Position.X - n2.Position.X;
			var dy = n1.Position.Y - n2.Position.Y;
			var dz = n1.Position.Z - n2.Position.Z;
			return Math.Abs(dx)+Math.Abs(dy)+Math.Abs(dz);
		}

		/// <summary>
		/// Returns the maximum distance between two nodes : Max(|Dx|, |Dy|, |Dz|)
		/// </summary>
		/// <exception cref="ArgumentNullException">Argument nodes must not be null.</exception>
		/// <param name="n1">First node.</param>
		/// <param name="n2">Second node.</param>
		/// <returns>Distance value.</returns>
		public static double MaxDistanceAlongAxis(Node n1, Node n2)
		{
			if ( n1==null || n2==null ) throw new ArgumentNullException();
			var dx = Math.Abs(n1.Position.X - n2.Position.X);
			var dy = Math.Abs(n1.Position.Y - n2.Position.Y);
			var dz = Math.Abs(n1.Position.Z - n2.Position.Z);
			return Math.Max(dx, Math.Max(dy, dz));
		}
		
		/// <summary>
		/// Returns the bounding box that wraps the specified list of nodes.
		/// </summary>
		/// <exception cref="ArgumentException">The list must only contain elements of type Node.</exception>
		/// <exception cref="ArgumentException">The list of nodes is empty.</exception>
		/// <param name="nodesGroup">The list of nodes to wrap.</param>
		/// <param name="minPoint">The point of minimal coordinates for the box.</param>
		/// <param name="maxPoint">The point of maximal coordinates for the box.</param>
		public static void BoundingBox(IList nodesGroup, out double[] minPoint, out double[] maxPoint)
		{
			var n1 = nodesGroup[0] as Node;
			if ( n1==null ) throw new ArgumentException("The list must only contain elements of type Node.");
			if ( nodesGroup.Count==0 ) throw new ArgumentException("The list of nodes is empty.");
			var dim = 3;
			minPoint = new double[dim];
			maxPoint = new double[dim];
			for (var i=0; i<dim; i++) minPoint[i]=maxPoint[i]=n1.Position[i];
			foreach ( Node n in nodesGroup )
			{
				for ( var i=0; i<dim; i++ )
				{
					if ( minPoint[i]>n.Position[i] ) minPoint[i]=n.Position[i];
					if ( maxPoint[i]<n.Position[i] ) maxPoint[i]=n.Position[i];
				}
			}
		}
	}
}

