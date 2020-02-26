using System;

namespace Uchu.Navigation
{
    /// <summary>
    /// A heuristic is a function that associates a value with a node to gauge it considering the node to reach.
    /// </summary>
    public delegate double Heuristic(Node nodeToEvaluate, Node targetNode);

    /// <summary>
    /// Class to search the best path between two nodes on a graph.
    /// </summary>
    public class AStar
    {
        private readonly Graph _graph;
        private readonly SortableList _open;
        private readonly SortableList _closed;
        private Track _leafToGoBackUp;
        private int _nbIterations = -1;

        private readonly SortableList.Equality _sameNodesReached = Track.SameEndNode;

        /// <summary>
        /// Heuristic based on the euclidian distance : Sqrt(Dx�+Dy�+Dz�)
        /// </summary>
        public static Heuristic EuclidianHeuristic => Node.EuclidianDistance;

        /// <summary>
        /// Heuristic based on the maximum distance : Max(|Dx|, |Dy|, |Dz|)
        /// </summary>
        public static Heuristic MaxAlongAxisHeuristic => Node.MaxDistanceAlongAxis;

        /// <summary>
        /// Heuristic based on the manhattan distance : |Dx|+|Dy|+|Dz|
        /// </summary>
        public static Heuristic ManhattanHeuristic => Node.ManhattanDistance;

        /// <summary>
        /// Gets/Sets the heuristic that AStar will use.
        /// It must be homogeneous to arc's cost.
        /// </summary>
        public Heuristic ChoosenHeuristic
        {
            get => Track.ChoosenHeuristic;
            set => Track.ChoosenHeuristic = value;
        }

        /// <summary>
        /// This value must belong to [0; 1] and it determines the influence of the heuristic on the algorithm.
        /// If this influence value is set to 0, then the search will behave in accordance with the Dijkstra algorithm.
        /// If this value is set to 1, then the cost to come to the current node will not be used whereas only the heuristic will be taken into account.
        /// </summary>
        /// <exception cref="ArgumentException">Value must belong to [0;1].</exception>
        public double DijkstraHeuristicBalance
        {
            get => Track.DijkstraHeuristicBalance;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentException("DijkstraHeuristicBalance value must belong to [0;1].");
                Track.DijkstraHeuristicBalance = value;
            }
        }

        /// <summary>
        /// AStar Constructor.
        /// </summary>
        /// <param name="g">The graph on which AStar will perform the search.</param>
        public AStar(Graph g)
        {
            _graph = g;
            _open = new SortableList();
            _closed = new SortableList();
            ChoosenHeuristic = EuclidianHeuristic;
            DijkstraHeuristicBalance = 0.5;
        }

        /// <summary>
        /// Searches for the best path to reach the specified EndNode from the specified StartNode.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="startNode">The node from which the path must start.</param>
        /// <param name="endNode">The node to which the path must end.</param>
        /// <returns>'true' if succeeded / 'false' if failed.</returns>
        public bool SearchPath(Node startNode, Node endNode)
        {
            lock (_graph)
            {
                Initialize(startNode, endNode);
                while (NextStep())
                {
                }

                return PathFound;
            }
        }

        /// <summary>
        /// Use for debug in 'step by step' mode only.
        /// Returns all the tracks found in the 'Open' list of the algorithm at a given time.
        /// A track is a list of the nodes visited to come to the current node.
        /// </summary>
        public Node[][] Open
        {
            get
            {
                var nodesList = new Node[_open.Count][];
                for (var i = 0; i < _open.Count; i++) nodesList[i] = GoBackUpNodes((Track) _open[i]);
                return nodesList;
            }
        }

        /// <summary>
        /// Use for debug in a 'step by step' mode only.
        /// Returns all the tracks found in the 'Closed' list of the algorithm at a given time.
        /// A track is a list of the nodes visited to come to the current node.
        /// </summary>
        public Node[][] Closed
        {
            get
            {
                var nodesList = new Node[_closed.Count][];
                for (var i = 0; i < _closed.Count; i++) nodesList[i] = GoBackUpNodes((Track) _closed[i]);
                return nodesList;
            }
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// Initializes AStar before performing search steps manually with NextStep.
        /// </summary>
        /// <exception cref="ArgumentNullException">StartNode and EndNode cannot be null.</exception>
        /// <param name="startNode">The node from which the path must start.</param>
        /// <param name="endNode">The node to which the path must end.</param>
        public void Initialize(Node startNode, Node endNode)
        {
            if (startNode == null || endNode == null) throw new ArgumentNullException();
            _closed.Clear();
            _open.Clear();
            Track.Target = endNode;
            _open.Add(new Track(startNode));
            _nbIterations = 0;
            _leafToGoBackUp = null;
        }

        /// <summary>
        /// Use for a 'step by step' search only. This method is alternate to SearchPath.
        /// The algorithm must have been initialize before.
        /// </summary>
        /// <exception cref="InvalidOperationException">You must initialize AStar before using NextStep().</exception>
        /// <returns>'true' unless the search ended.</returns>
        public bool NextStep()
        {
            if (!Initialized)
                throw new InvalidOperationException("You must initialize AStar before launching the algorithm.");
            if (_open.Count == 0) return false;
            _nbIterations++;

            var indexMin = _open.IndexOfMin();
            var bestTrack = (Track) _open[indexMin];
            _open.RemoveAt(indexMin);
            if (bestTrack.Succeed)
            {
                _leafToGoBackUp = bestTrack;
                _open.Clear();
            }
            else
            {
                Propagate(bestTrack);
                _closed.Add(bestTrack);
            }

            return _open.Count > 0;
        }

        private void Propagate(Track trackToPropagate)
        {
            foreach (Arc a in trackToPropagate.EndNode.OutgoingArcs)
            {
                if (!a.Passable || !a.EndNode.Passable) continue;
                
                var successor = new Track(trackToPropagate, a);
                var posNf = _closed.IndexOf(successor, _sameNodesReached);
                var posNo = _open.IndexOf(successor, _sameNodesReached);
                if (posNf > 0 && successor.Cost >= ((Track) _closed[posNf]).Cost) continue;
                if (posNo > 0 && successor.Cost >= ((Track) _open[posNo]).Cost) continue;
                if (posNf > 0) _closed.RemoveAt(posNf);
                if (posNo > 0) _open.RemoveAt(posNo);
                _open.Add(successor);
            }
        }

        /// <summary>
        /// To know if the search has been initialized.
        /// </summary>
        public bool Initialized => _nbIterations >= 0;

        /// <summary>
        /// To know if the search has been started.
        /// </summary>
        public bool SearchStarted => _nbIterations > 0;

        /// <summary>
        /// To know if the search has ended.
        /// </summary>
        public bool SearchEnded => SearchStarted && _open.Count == 0;

        /// <summary>
        /// To know if a path has been found.
        /// </summary>
        public bool PathFound => _leafToGoBackUp != null;

        /// <summary>
        /// Use for a 'step by step' search only.
        /// Gets the number of the current step.
        /// -1 if the search has not been initialized.
        /// 0 if it has not been started.
        /// </summary>
        public int StepCounter => _nbIterations;

        private void CheckSearchHasEnded()
        {
            if (!SearchEnded)
                throw new InvalidOperationException("You cannot get a result unless the search has ended.");
        }

        /// <summary>
        /// Returns information on the result.
        /// </summary>
        /// <param name="nbArcsOfPath">The number of arcs in the result path / -1 if no result.</param>
        /// <param name="costOfPath">The cost of the result path / -1 if no result.</param>
        /// <returns>'true' if the search succeeded / 'false' if it failed.</returns>
        public bool ResultInformation(out int nbArcsOfPath, out double costOfPath)
        {
            CheckSearchHasEnded();
            if (!PathFound)
            {
                nbArcsOfPath = -1;
                costOfPath = -1;
                return false;
            }
            else
            {
                nbArcsOfPath = _leafToGoBackUp.NbArcsVisited;
                costOfPath = _leafToGoBackUp.Cost;
                return true;
            }
        }

        /// <summary>
        /// Gets the array of nodes representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public Node[] PathByNodes
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                return GoBackUpNodes(_leafToGoBackUp);
            }
        }

        private Node[] GoBackUpNodes(Track T)
        {
            var nb = T.NbArcsVisited;
            var path = new Node[nb + 1];
            for (var i = nb; i >= 0; i--, T = T.Queue)
                path[i] = T.EndNode;
            return path;
        }

        /// <summary>
        /// Gets the array of arcs representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public Arc[] PathByArcs
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                var nb = _leafToGoBackUp.NbArcsVisited;
                var path = new Arc[nb];
                var cur = _leafToGoBackUp;
                for (var i = nb - 1; i >= 0; i--, cur = cur.Queue)
                    path[i] = cur.Queue.EndNode.ArcGoingTo(cur.EndNode);
                return path;
            }
        }

        /// <summary>
        /// Gets the array of points representing the found path.
        /// </summary>
        /// <exception cref="InvalidOperationException">You cannot get a result unless the search has ended.</exception>
        public Point3D[] PathByCoordinates
        {
            get
            {
                CheckSearchHasEnded();
                if (!PathFound) return null;
                var nb = _leafToGoBackUp.NbArcsVisited;
                var path = new Point3D[nb + 1];
                var cur = _leafToGoBackUp;
                for (var i = nb; i >= 0; i--, cur = cur.Queue)
                    path[i] = cur.EndNode.Position;
                return path;
            }
        }
    }
}