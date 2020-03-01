
using System;

namespace Uchu.Navigation
{
	/// <summary>
	/// A track is a succession of nodes which have been visited.
	/// Thus when it leads to the target node, it is easy to return the result path.
	/// These objects are contained in Open and Closed lists.
	/// </summary>
	internal class Track : IComparable
	{
		private static Node _target = null;
		private static double _coeff = 0.5;
		private static Heuristic _choosenHeuristic = AStar.EuclidianHeuristic;

		public static Node Target { set => _target = value;
			get => _target;
		}

		public Node EndNode;
		public Track Queue;

		public static double DijkstraHeuristicBalance
		{
			get => _coeff;
			set
			{
				if ( value<0 || value>1 ) throw new ArgumentException(
@"The coefficient which balances the respective influences of Dijkstra and the Heuristic must belong to [0; 1].
-> 0 will minimize the number of nodes explored but will not take the real cost into account.
-> 0.5 will minimize the cost without developing more nodes than necessary.
-> 1 will only consider the real cost without estimating the remaining cost.");
				_coeff = value;
			}
		}

		public static Heuristic ChoosenHeuristic
		{
			set => _choosenHeuristic = value;
			get => _choosenHeuristic;
		}

		private readonly int _nbArcsVisited;
		public int NbArcsVisited => _nbArcsVisited;

		private readonly double _cost;
		public double Cost => _cost;

		public virtual double Evaluation => _coeff*_cost+(1-_coeff)*_choosenHeuristic(EndNode, _target);

		public bool Succeed => EndNode==_target;

		public Track(Node graphNode)
		{
			if ( _target==null ) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			_cost = 0;
			_nbArcsVisited = 0;
			Queue = null;
			EndNode = graphNode;
		}

		public Track(Track previousTrack, Arc transition)
		{
			if (_target==null) throw new InvalidOperationException("You must specify a target Node for the Track class.");
			Queue = previousTrack;
			_cost = Queue.Cost + transition.Cost;
			_nbArcsVisited = Queue._nbArcsVisited + 1;
			EndNode = transition.EndNode;
		}

		public int CompareTo(object objet)
		{
			var otherTrack = (Track) objet;
			return Evaluation.CompareTo(otherTrack.Evaluation);
		}

		public static bool SameEndNode(object o1, object o2)
		{
			if ( !(o1 is Track p1) || !(o2 is Track p2) ) throw new ArgumentException("Objects must be of 'Track' type.");
			return p1.EndNode==p2.EndNode;
		}
	}
}
