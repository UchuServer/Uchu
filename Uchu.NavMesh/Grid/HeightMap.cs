using System.Numerics;
using Uchu.World.Client;

namespace Uchu.NavMesh.Grid;

public class HeightMap
{
    /// <summary>
    /// Scale to apply to the positions.
    /// </summary>
    public const float Scale = 3.125f;
    
    /// <summary>
    /// Height values of the height map.
    /// </summary>
    public float[,] Heights { get; private set; }

    /// <summary>
    /// Width of the heightmap.
    /// </summary>
    public int SizeX => Heights.GetLength(0);

    /// <summary>
    /// Depth of the heightmap.
    /// </summary>
    public int SizeY => Heights.GetLength(1);

    /// <summary>
    /// Generates the height map for a zone.
    /// </summary>
    /// <param name="zoneInfo">Zone info to use.</param>
    /// <returns>The height map for the zone.</returns>
    public static HeightMap FromZoneInfo(ZoneInfo zoneInfo)
    {
        // Generate the heightmap.
        var terrain = zoneInfo.TerrainFile;
        var heightMap = new HeightMap()
        {
            Heights = terrain.GenerateHeightMap(),
        };
        
        // Return the heightmap.
        return heightMap;
    }

    /// <summary>
    /// Returns the position in the world for a given grid position.
    /// </summary>
    /// <param name="x">X position in the grid.</param>
    /// <param name="y">Y position in the grid.</param>
    /// <returns>Position in the world.</returns>
    public Vector3 GetPosition(int x, int y)
    {
        var centerX = (this.Heights.GetLength(0) - 1) / 2;
        var centerY = (this.Heights.GetLength(1) - 1) / 2;
        return new Vector3((x - centerX) * Scale, this.Heights[x, y], (y - centerY) * Scale);
    }
}