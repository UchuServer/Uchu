using InfectedRose.Terrain;

namespace Uchu.World
{
    public static class TerrainFileExtensions
    {
        public static float[,] CalculateHeightMap(this TerrainFile @this)
        {
            var heightMapWidth = @this.Chunks[0].HeightMap.Width;
            var heightMapHeight = @this.Chunks[0].HeightMap.Height;

            var heights = new float[heightMapWidth * @this.Chunks.Count, heightMapHeight * @this.Chunks.Count];

            //
            // Render HeightMap
            //
            for (var chunkY = 0; chunkY < @this.Height; ++chunkY)
            {
                for (var chunkX = 0; chunkX < @this.Weight; ++chunkX)
                {
                    var chunk = @this.Chunks[chunkY * @this.Weight + chunkX];

                    for (var y = 0; y < chunk.HeightMap.Height; ++y)
                    {
                        for (var x = 0; x < chunk.HeightMap.Width; ++x)
                        {
                            var value = chunk.HeightMap.Data[y * chunk.HeightMap.Width + x];

                            var pixelX = chunkX * heightMapWidth + x;
                            var pixelY = chunkY * heightMapHeight + y;

                            heights[pixelX, pixelY] = value;
                        }
                    }
                }
            }

            return heights;
        }
    }
}