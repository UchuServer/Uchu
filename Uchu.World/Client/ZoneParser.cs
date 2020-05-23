using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using InfectedRose.Luz;
using InfectedRose.Lvl;
using InfectedRose.Terrain;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.IO;

namespace Uchu.World.Client
{
    public class ZoneParser
    {
        private readonly IFileResources _resources;
        private readonly XmlSerializer _triggerSerializer = new XmlSerializer(typeof(TriggerCollection));

        public Dictionary<int, ZoneInfo> Zones { get; }

        public ZoneParser(IFileResources resources)
        {
            _resources = resources;
            
            Zones = new Dictionary<int, ZoneInfo>();
        }

        public async Task LoadZoneDataAsync(int seek)
        {
            Zones.Clear();

            Logger.Information("Parsing zone info...");
            
            var luzFiles = _resources.GetAllFilesWithExtension("luz");

            foreach (var luzFile in luzFiles)
            {
                Logger.Information($"Parsing: {luzFile}");
                
                try
                {
                    var luz = new LuzFile();

                    await using var stream = _resources.GetStream(luzFile);

                    var reader = new BitReader(stream);

                    luz.Deserialize(reader);

                    if (luz.WorldId != seek) continue;
                    
                    var path = Path.GetDirectoryName(luzFile);

                    var lvlFiles = new List<LvlFile>();

                    foreach (var scene in luz.Scenes)
                    {
                        await using var sceneStream = _resources.GetStream(Path.Combine(path, scene.FileName));

                        using var sceneReader = new BitReader(sceneStream);
                        
                        Logger.Information($"Parsing: {scene.FileName}");
                        
                        var lvl = new LvlFile();

                        lvl.Deserialize(sceneReader);

                        lvlFiles.Add(lvl);

                        if (lvl.LevelObjects?.Templates == default) continue;
                        
                        foreach (var template in lvl.LevelObjects.Templates)
                        {
                            template.ObjectId |= 70368744177664;
                        }
                    }

                    var terrainStream = _resources.GetStream(Path.Combine(path, luz.TerrainFileName));

                    using var terrainReader = new BitReader(terrainStream);
                    
                    var terrain = new TerrainFile();

                    terrain.Deserialize(terrainReader);
                    
                    var triggers = await TriggerDictionary.FromDirectoryAsync(Path.Combine(_resources.RootPath, path));
                    
                    Logger.Information($"Parsed: {(ZoneId) luz.WorldId}");

                    Zones[(int) luz.WorldId] = new ZoneInfo
                    {
                        LuzFile = luz,
                        LvlFiles = lvlFiles,
                        TriggerDictionary = triggers,
                        TerrainFile = terrain
                    };
                    
                    break;
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to parse {luzFile}: {e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}