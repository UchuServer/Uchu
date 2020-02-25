using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World.AI.AStar;

namespace Uchu.World.AI
{
    public class NavMeshManager
    {
        public Zone Zone { get; }
        
        public Solver Solver { get; }
        
        public Dictionary<int, Dictionary<int, Vector3>> Points { get; private set; }

        public NavMeshManager(Zone zone)
        {
            Zone = zone;
            
            Solver = new Solver();
        }

        public async Task GeneratePointsAsync()
        {
            const float scale = 3.125f;

            var terrain = Zone.ZoneInfo.TerrainFile;

            var heightMap = terrain.GenerateHeightMap();

            var inGameValues = new Dictionary<int, Dictionary<int, Vector3>>();

            var centerX = (heightMap.GetLength(0) - 1) / 2;
            var centerY = (heightMap.GetLength(1) - 1) / 2;
            
            for (var x = 0; x < heightMap.GetLength(0); x++)
            {
                for (var y = 0; y < heightMap.GetLength(1); y++)
                {
                    var value = heightMap[x, y];

                    var realX = x - centerX;
                    var realY = y - centerY;
                    
                    var inGame = new Vector3(realX, 0, realY);

                    inGame *= scale;

                    inGame.Y = value;

                    if (inGameValues.TryGetValue(x, out var dict))
                    {
                        dict[y] = inGame;
                    }
                    else
                    {
                        inGameValues[x] = new Dictionary<int, Vector3>
                        {
                            [y] = inGame
                        };
                    }
                }
            }

            Points = inGameValues;

            await Solver.GenerateAsync(Points, heightMap.GetLength(0), heightMap.GetLength(1));
        }

        public Vector3[] GeneratePath(Vector3 start, Vector3 end) => Solver.GeneratePath(start, end);
    }
}