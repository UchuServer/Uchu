using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using EMK.Cartography;
using Uchu.Core;
using Star = EMK.Cartography.AStar;

namespace Uchu.World.AI.AStar
{
    public class Solver
    {
        private Star Star { get; set; }
        
        private Node[] Nodes { get; set; }
        
        private int Complete { get; set; }
        
        private bool Done { get; set; }
        
        public async Task GenerateAsync(Dictionary<int, Dictionary<int, Vector3>> points, int weight, int height)
        {
            var graph = new Graph();

            var nodes = new Dictionary<Vector2, Node>();

            Complete = 0;
            
            var _ = Task.Run(async () =>
            {
                while (!Done)
                {
                    await Task.Delay(1000);
                    
                    Console.WriteLine(Complete);
                }
            });

            Logger.Information($"Generating nav mesh: {weight * height}");
            
            for (var x = 0; x < weight; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var point = points[x][y];

                    nodes.Add(new Vector2(x, y), new Node(point.X, point.Y, point.Z));

                    Complete++;
                }
            }

            Logger.Information($"Generating nav mesh: {nodes.Count}");

            var connections = new List<Arc>();

            var tasks = new List<Task>();

            Complete = 0;
            
            foreach (var (key, node) in nodes)
            {
                var task = Task.Run(() =>
                {
                    if (nodes.TryGetValue(key - Vector2.UnitX, out var child))
                    {
                        if (Vector3.Distance(child.ToVector3(), node.ToVector3()) > 6) return;

                        lock (connections)
                            connections.Add(new Arc(node, child));
                    }

                    if (nodes.TryGetValue(key + Vector2.UnitX, out child))
                    {
                        if (Vector3.Distance(child.ToVector3(), node.ToVector3()) > 6) return;

                        lock (connections)
                            connections.Add(new Arc(node, child));
                    }

                    if (nodes.TryGetValue(key - Vector2.UnitY, out child))
                    {
                        if (Vector3.Distance(child.ToVector3(), node.ToVector3()) > 6) return;

                        lock (connections)
                            connections.Add(new Arc(node, child));
                    }

                    if (nodes.TryGetValue(key + Vector2.UnitY, out child))
                    {
                        if (Vector3.Distance(child.ToVector3(), node.ToVector3()) > 6) return;

                        lock (connections)
                            connections.Add(new Arc(node, child));
                    }

                    lock (this)
                    {
                        Complete++;
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            graph.LN = new ArrayList(nodes.Values.ToArray());
            graph.LA = new ArrayList(connections.ToArray());

            Console.WriteLine($"Connections: {connections.Count}");

            Logger.Information("Generated nav mesh!");

            Star = new Star(graph);

            Nodes = nodes.Values.ToArray();

            Done = true;
        }

        public Vector3[] GeneratePath(Vector3 start, Vector3 end)
        {
            lock (Star)
            {
                var startNode = GetClosest(start);
                var endNode = GetClosest(end);

                Star.SearchPath(startNode, endNode);

                return Star.PathByCoordinates.Select(p => p.ToVector3()).ToArray();
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