using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using InfectedRose.Luz;
using InfectedRose.Lvl;
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

        public Dictionary<ZoneId, ZoneInfo> Zones { get; }

        public ZoneParser(IFileResources resources)
        {
            _resources = resources;
            
            Zones = new Dictionary<ZoneId, ZoneInfo>();
        }

        public async Task LoadZoneDataAsync(int seek)
        {
            Zones.Clear();

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

                    var triggers = await GetTriggers(path);
                    
                    var lvlFiles = new List<LvlFile>();

                    foreach (var scene in luz.Scenes)
                    {
                        await using var sceneStream = _resources.GetStream(Path.Combine(path, scene.FileName));

                        var sceneReader = new BitReader(sceneStream);
                        
                        var lvl = new LvlFile();

                        lvl.Deserialize(sceneReader);

                        lvlFiles.Add(lvl);

                        if (lvl.LevelObjects?.Templates == default) continue;
                        
                        foreach (var template in lvl.LevelObjects.Templates)
                        {
                            template.ObjectId |= 70368744177664;
                        }
                    }
                    
                    Logger.Information($"Parsed: {(ZoneId) luz.WorldId}");
                    
                    Zones[(ZoneId) luz.WorldId] = new ZoneInfo
                    {
                        LuzFile = luz,
                        LvlFiles = lvlFiles,
                        Triggers = triggers.ToList()
                    };
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to parse {luzFile}: {e.Message}\n{e.StackTrace}");
                }
            }
        }
        
        private async Task<Trigger[]> GetTriggers(string path)
        {
            var files = _resources.GetAllFilesWithExtension(path, "lutriggers");

            var triggerCollection = new List<Trigger>();
            
            foreach (var file in files)
            {
                await using var stream = File.OpenRead(file);

                var triggers = (TriggerCollection) _triggerSerializer.Deserialize(stream);

                var fileName = Path.GetFileNameWithoutExtension(file);

                var parts = fileName.Split('_');

                foreach (var part in parts)
                {
                    //
                    // I don't know if there is a better way of getting this ID.
                    //
                    
                    if (!int.TryParse(part, out var primaryId)) continue;
                    
                    /*
                    foreach (var trigger in triggers.Triggers)
                    {
                        trigger.Id = primaryId;
                    }
                    */
                    
                    triggerCollection.AddRange(triggers.Triggers);
                    
                    break;
                }
            }

            return triggerCollection.ToArray();
        }
    }
}