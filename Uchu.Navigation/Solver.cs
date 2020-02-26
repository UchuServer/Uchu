using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Star = Uchu.Navigation.AStar;

namespace Uchu.Navigation
{
    public class Solver
    {
        private Node[] Nodes { get; set; }
        
        private Graph Graph { get; set; }
        
        private Star Star { get; set; }
        
        public async Task GenerateAsync(Dictionary<int, Dictionary<int, Vector3>> points, int weight, int height, float min)
        {
            var graph = new Graph();

            var nodes = new Dictionary<Vector2, Node>();

            for (var x = 0; x < weight; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var point = points[x][y];
                    
                    if (Math.Abs(point.Y - min) < 0.1f) continue;

                    nodes.Add(new Vector2(x, y), new Node(point.X, point.Y, point.Z));
                }
            }

            var connections = new List<Arc>();

            var tasks = new List<Task>();
            
            foreach (var (key, node) in nodes)
            {
                var task = Task.Run(() =>
                {
                    var toCheck = new[]
                    {
                        key - Vector2.UnitX,
                        key + Vector2.UnitX,
                        key - Vector2.UnitY,
                        key + Vector2.UnitY,
                        key - Vector2.UnitX - Vector2.UnitY,
                        key + Vector2.UnitX - Vector2.UnitY,
                        key - Vector2.UnitX + Vector2.UnitY,
                        key + Vector2.UnitX + Vector2.UnitY
                    };

                    foreach (var vector2 in toCheck)
                    {
                        if (!nodes.TryGetValue(vector2, out var child)) continue;
                        
                        if (Vector3.Distance(child.ToVector3(), node.ToVector3()) > 6) return;

                        lock (connections)
                            connections.Add(new Arc(node, child));
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            graph.Ln = new ArrayList(nodes.Values.ToArray());
            graph.La = new ArrayList(connections.ToArray());

            Nodes = nodes.Values.ToArray();

            Graph = graph;

            Star = new Star(Graph);
        }

        public Vector3[] GeneratePath(Vector3 start, Vector3 end)
        {
            lock (Star)
            {
                var startNode = GetClosest(start);
                var endNode = GetClosest(end);

                Star.SearchPath(startNode, endNode);

                var coordinates = Star.PathByCoordinates;

                if (coordinates == default || coordinates.Length == default)
                {
                    return new[] {start};
                }

                return coordinates.Where(c => !ReferenceEquals(c, default)).Select(p => p.ToVector3()).ToArray();
            }
        }

        private Node GetClosest(Vector3 value)
        {
            var closest = Nodes[default];

            foreach (var node in Nodes)
            {
                var distance = Vector3.Distance(node.ToVector3(), value);
                var closestDistance = Vector3.Distance(closest.ToVector3(), value);

                if (distance < closestDistance)
                    closest = node;
            }

            return closest;
        }
    }
}