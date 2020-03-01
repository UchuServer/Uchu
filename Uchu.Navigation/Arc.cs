using System;

namespace Uchu.Navigation
{
	/// <summary>
	/// An arc is defined with its two extremity nodes StartNode and EndNode therefore it is oriented.
	/// It is also characterized by a crossing factor named 'Weight'.
	/// This value represents the difficulty to reach the ending node from the starting one.
	/// </summary>
	[Serializable]
	public class Arc
	{
		private Node _startNode, _endNode;
		private bool _passable;
		private double _length;
		private bool _lengthUpdated;

		/// <summary>
		/// Arc constructor.
		/// </summary>
		/// <exception cref="ArgumentNullException">Extremity nodes cannot be null.</exception>
		/// <exception cref="ArgumentException">StartNode and EndNode must be different.</exception>
		/// <param name="start">The node from which the arc starts.</param>
		/// <param name="end">The node to which the arc ends.</param>
		public Arc(Node start, Node end)
		{
			StartNode = start;
			EndNode = end;
			LengthUpdated = false;
			Passable = true;
		}

		/// <summary>
		/// Gets/Sets the node from which the arc starts.
		/// </summary>
		/// <exception cref="ArgumentNullException">StartNode cannot be set to null.</exception>
		/// <exception cref="ArgumentException">StartNode cannot be set to EndNode.</exception>
		public Node StartNode
		{
			set
			{
				if ( value==null ) throw new ArgumentNullException("StartNode");
				if ( EndNode!=null && value.Equals(EndNode) ) throw new ArgumentException("StartNode and EndNode must be different");
				_startNode?.OutgoingArcs.Remove(this);
				_startNode = value;
				_startNode.OutgoingArcs.Add(this);
			}
			get => _startNode;
		}

		/// <summary>
		/// Gets/Sets the node to which the arc ends.
		/// </summary>
		/// <exception cref="ArgumentNullException">EndNode cannot be set to null.</exception>
		/// <exception cref="ArgumentException">EndNode cannot be set to StartNode.</exception>
		public Node EndNode
		{
			set
			{
				if ( value==null ) throw new ArgumentNullException("EndNode");
				if ( StartNode!=null && value.Equals(StartNode) ) throw new ArgumentException("StartNode and EndNode must be different");
				_endNode?.IncomingArcs.Remove(this);
				_endNode = value;
				_endNode.IncomingArcs.Add(this);
			}
			get => _endNode;
		}

		/// <summary>
		/// Sets/Gets the weight of the arc.
		/// This value is used to determine the cost of moving through the arc.
		/// </summary>
		public double Weight => 1;

		/// <summary>
		/// Gets/Sets the functional state of the arc.
		/// 'true' means that the arc is in its normal state.
		/// 'false' means that the arc will not be taken into account (as if it did not exist or if its cost were infinite).
		/// </summary>
		public bool Passable
		{
			set => _passable = value;
			get => _passable;
		}

		internal bool LengthUpdated
		{
			set => _lengthUpdated = value;
			get => _lengthUpdated;
		}

		/// <summary>
		/// Gets arc's length.
		/// </summary>
		public double Length
		{
			get
			{
				if ( LengthUpdated==false )
				{
					_length = CalculateLength();
					LengthUpdated = true;
				}
				return _length;
			}
		}

		/// <summary>
		/// Performs the calculous that returns the arc's length
		/// Can be overriden for derived types of arcs that are not linear.
		/// </summary>
		/// <returns></returns>
		protected virtual double CalculateLength()
		{
			return Point3D.DistanceBetween(_startNode.Position, _endNode.Position);
		}

		/// <summary>
		/// Gets the cost of moving through the arc.
		/// Can be overriden when not simply equals to Weight*Length.
		/// </summary>
		public virtual double Cost => Weight*Length;

		/// <summary>
		/// Returns the textual description of the arc.
		/// object.ToString() override.
		/// </summary>
		/// <returns>String describing this arc.</returns>
		public override string ToString()
		{
			return _startNode+"-->"+_endNode;
		}

		/// <summary>
		/// Object.Equals override.
		/// Tells if two arcs are equal by comparing StartNode and EndNode.
		/// </summary>
		/// <exception cref="ArgumentException">Cannot compare an arc with another type.</exception>
		/// <param name="o">The arc to compare with.</param>
		/// <returns>'true' if both arcs are equal.</returns>
		public override bool Equals(object o)
		{
			var a = (Arc) o;
			if ( a==null ) throw new ArgumentException("Cannot compare type "+GetType()+" with type "+o.GetType()+" !");
			return _startNode.Equals(a._startNode) && _endNode.Equals(a._endNode);
		}

		/// <summary>
		/// Object.GetHashCode override.
		/// </summary>
		/// <returns>HashCode value.</returns>
		public override int GetHashCode() { return (int)Length; }
	}
}

