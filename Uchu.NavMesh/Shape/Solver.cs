using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.NavMesh.Grid;
using Uchu.World.Client;

namespace Uchu.NavMesh.Shape;

public class Solver : ISerializable, IDeserializable
{
    /// <summary>
    /// Version of the solver stored with the cache files. If the version does not match, the cached version of the
    /// solver is discarded. If changes are made that change the results of generating the solver, this number
    /// should be incremented to invalidate the cache entries of updating servers.
    /// </summary>
    public const int SolverVersion = 0;
    
    /// <summary>
    /// Maximum distance 2 nodes on the heightmap can be before being considered to steep to connect.
    /// </summary>
    public const int MaximumNodeDistance = 6;

    /// <summary>
    /// Minimum distance the nodes must be from the lowest node to be used for generating the shapes.
    /// </summary>
    public const int MinimumDistanceFromBottom = 5;
    
    /// <summary>
    /// Height map of the solver.
    /// </summary>
    public HeightMap HeightMap { get; private set; }
    
    /// <summary>
    /// Shape that define the boundaries in 2D.
    /// </summary>
    public OrderedShape BoundingShape { get; private set; }

    /// <summary>
    /// Creates a solver for a zone. The zone may be cached.
    /// </summary>
    /// <param name="zoneInfo">Zone info with a terrain file to read.</param>
    public static async Task<Solver> FromZoneAsync(ZoneInfo zoneInfo)
    {
        // Create the solver with the heightmap.
        var solver = new Solver();
        solver.HeightMap = HeightMap.FromZoneInfo(zoneInfo);
        
        // Return a cached entry.
        try
        {
            await solver.LoadFromCacheAsync(zoneInfo);
            return solver;
        }
        catch (FileNotFoundException)
        {
            // Cache file not found.
            Logger.Information("Cached version of map path solver not found.");
        }
        catch (InvalidDataException)
        {
            // Delete the invalid cache file.
            Logger.Information("");
            File.Delete(Path.Combine("MapSolverCache", zoneInfo.LuzFile.WorldId + ".bin"));
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        // Load the solver from the heightmap.
        await solver.GenerateShapesAsync();
        await solver.SaveToCacheAsync(zoneInfo);
        return solver;
    }
    
    /// <summary>
    /// Saves the file for caching.
    /// </summary>
    /// <param name="zoneInfo">Zone info to save as.</param>
    private async Task SaveToCacheAsync(ZoneInfo zoneInfo)
    {
        // Create the directory if it does not exist.
        if (!Directory.Exists("MapSolverCache"))
        {
            Directory.CreateDirectory("MapSolverCache");
        }

        // Save the file.
        var path = Path.Combine("MapSolverCache", zoneInfo.LuzFile.WorldId + ".bin");
        if (File.Exists(path))
            return;
        await using var memoryStream = new MemoryStream();
        var writer = new BitWriter(memoryStream);
        writer.Write(SolverVersion);
        this.Serialize(writer);
        await File.WriteAllBytesAsync(path, memoryStream.ToArray());
    }

    /// <summary>
    /// Loads the solver from the cache. Throws an exception if the cache is invalid.
    /// </summary>
    /// <param name="zoneInfo">Zone info to save as.</param>
    private async Task LoadFromCacheAsync(ZoneInfo zoneInfo)
    {
        // Throw an exception if the cache file does not exist.
        var path = Path.Combine("MapSolverCache", zoneInfo.LuzFile.WorldId + ".bin");
        if (!File.Exists(path))
            throw new FileNotFoundException("Cache file not found");
        
        // Start to read the file and throw an exception if the version does not match.
        var cacheData = await File.ReadAllBytesAsync(path);
        await using var memoryStream = new MemoryStream(cacheData.Length);
        memoryStream.Write(cacheData);
        await memoryStream.FlushAsync();
        memoryStream.Position = 0;
        var reader = new BitReader(memoryStream);
        var cacheVersion = reader.Read<int>();
        if (cacheVersion != SolverVersion)
            throw new InvalidDataException("Cache version is not current. (Expected " + SolverVersion + ", got " + cacheVersion);
        
        // Deserialize the file.
        this.Deserialize(reader);
    }
    
    /// <summary>
    /// Generates the shapes of the zone.
    /// </summary>
    private async Task GenerateShapesAsync()
    {
        // Create the nodes.
        var minimumHeight = float.MaxValue;
        var nodes = new Node[this.HeightMap.SizeX, this.HeightMap.SizeY];
        for (var x = 0; x < this.HeightMap.SizeX; x++)
        {
            for (var y = 0; y < this.HeightMap.SizeY; y++)
            {
                // Get the position.
                var position = this.HeightMap.GetPosition(x, y);
                if (position.Y < minimumHeight)
                    minimumHeight = position.Y;
                
                // Add the node.
                nodes[x, y] = new Node(position);
            }
        }
        
        // Populate the edges.
        // This can be done in parallel.
        var tasks = new List<Task>();
        for (var x = 0; x < this.HeightMap.SizeX; x++)
        {
            for (var y = 0; y < this.HeightMap.SizeY; y++)
            {
                var currentX = x;
                var currentY = y;
                var currentNode = nodes[x, y];
                if (Math.Abs(currentNode.Position.Y - minimumHeight) < MinimumDistanceFromBottom) continue;
                tasks.Add(Task.Run(() =>
                {
                    for (var offsetX = -1; offsetX <= 1; offsetX++)
                    {
                        var otherX = currentX + offsetX;
                        if (otherX < 0 || otherX >= this.HeightMap.SizeX) continue;
                        for (var offsetY = -1; offsetY <= 1; offsetY++)
                        {
                            if (offsetX == 0 && offsetY == 0) continue;
                            var otherY = currentY + offsetY;
                            if (otherY < 0 || otherY >= this.HeightMap.SizeY) continue;
                            var otherNode = nodes[otherX, otherY];
                            if (Vector3.Distance(otherNode.Position, currentNode.Position) > MaximumNodeDistance) continue;
                            currentNode.Neighbors.Add(otherNode);
                        }
                    }
                }));
            }
        }
        await Task.WhenAll(tasks);
        
        // Create the rows of shapes.
        var shapeRows = new List<UnorderedShape>[this.HeightMap.SizeX - 1];
        tasks = new List<Task>();
        for (var x = 0; x < this.HeightMap.SizeX - 1; x++)
        {
            var currentX = x;
            tasks.Add(Task.Run(() =>
            {
                // Create and merge the shapes for the row.
                var rowShapes = new List<UnorderedShape>();
                for (var y = 0; y < this.HeightMap.SizeY - 1; y++)
                {
                    var shape = UnorderedShape.FromNodes(nodes[currentX, y], nodes[currentX + 1, y], nodes[currentX, y + 1], nodes[currentX + 1, y + 1]);
                    if (shape == null) continue;

                    if (rowShapes.Count > 0 && rowShapes[^1].CanMerge(shape))
                    {
                        rowShapes[^1].Merge(shape);
                        continue;
                    }
                    rowShapes.Add(shape);
                }

                // Store the row.
                lock (shapeRows)
                {
                    shapeRows[currentX] = rowShapes;
                }
            }));
        }
        await Task.WhenAll(tasks);
        
        // Merge the rows.
        // This is done by constantly merging pairs of rows in parallel until every row is merged.
        while (shapeRows.Length > 1)
        {
            // Create the list for the merged rows and add the last row if it is odd.
            var totalNewShapeRows = (int) Math.Ceiling((shapeRows.Length / 2.0));
            var newShapeRows = new List<UnorderedShape>[totalNewShapeRows];
            if (shapeRows.Length % 2 == 1)
            {
                newShapeRows[totalNewShapeRows - 1] = shapeRows[^1];
            }

            // Create tasks to merge evert set of 2 rows.
            tasks = new List<Task>();
            for (var x = 0; x < Math.Floor(shapeRows.Length / 2.0) * 2; x += 2)
            {
                // Merge the 2 rows.
                var shapesToMerge = shapeRows[x].ToList();
                shapesToMerge.AddRange(shapeRows[x + 1]);
                var rowShapes = new List<UnorderedShape>();
                newShapeRows[x / 2] = rowShapes;
                tasks.Add(Task.Run(() =>
                {
                    while (shapesToMerge.Count > 0) {
                        var changesMade = false;
                        var shapeToMerge = shapesToMerge[0];
                        foreach (var otherShape in shapesToMerge.ToList())
                        {
                            if (!shapeToMerge.CanMerge(otherShape)) continue;
                            shapeToMerge.Merge(otherShape);
                            shapesToMerge.Remove(otherShape);
                            changesMade = true;
                        }

                        if (changesMade) continue;
                        shapesToMerge.Remove(shapeToMerge);
                        rowShapes.Add(shapeToMerge);
                    }
                }));
            }

            // Wait for the rows to complete and prepare for the next step.
            await Task.WhenAll(tasks);
            shapeRows = newShapeRows;
        }
        
        // Separate the shapes and make them 2D.
        var shapes = new List<OrderedShape>();
        tasks = new List<Task>();
        foreach (var shape in shapeRows[0])
        {
            tasks.Add(Task.Run(() =>
            {
                var newShapes = shape.GetOrderedShapes();
                lock (shapes)
                {
                    shapes.AddRange(newShapes);
                }
            }));
        }
        await Task.WhenAll(tasks);
        
        // Optimize the shapes.
        tasks = new List<Task>();
        foreach (var shape in shapes)
        {
            tasks.Add(Task.Run(() =>
            {
                shape.Optimize();
            }));
        }
        await Task.WhenAll(tasks);
        
        // Store the shapes.
        this.BoundingShape = new OrderedShape()
        {
            Points = new List<Vector2>()
            {
                new Vector2(float.MaxValue, float.MaxValue),
                new Vector2(float.MinValue, float.MaxValue),
                new Vector2(float.MinValue, float.MinValue),
                new Vector2(float.MaxValue, float.MinValue),
            }
        };
        foreach (var shape in shapes)
        {
            this.BoundingShape.TryAddShape(shape);
        }
        
        // Generate the nodes for each shape.
        tasks = new List<Task>();
        foreach (var shape in shapes)
        {
            tasks.Add(Task.Run(() =>
            {
                shape.GenerateGraph();
            }));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="writer">Writer to write to.</param>
    public void Serialize(BitWriter writer)
    {
        this.BoundingShape.Serialize(writer);
    }

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="reader">Reader to read to.</param>
    public void Deserialize(BitReader reader)
    {
        this.BoundingShape = new OrderedShape();
        this.BoundingShape.Deserialize(reader);
    }
}