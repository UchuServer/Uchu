using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting.Managed
{
    public class PythonScriptPack : ScriptPack
    {
        public override string Name => Location.Replace('\\', '/').Split('/').Last(s => !string.IsNullOrWhiteSpace(s));

        public string Source { get; }

        public List<PythonScript> Scripts { get; }
        
        public PythonScriptPack(Zone zone, string location, string source = default) : base(zone, location)
        {
            Source = source;

            Scripts = new List<PythonScript>();
        }
        
        public override async Task LoadAsync()
        {
            Logger.Information($"Loading python script pack: {Name}");

            var location = Path.Combine(Zone.Server.MasterPath, Location);
            
            if (File.Exists(location))
            {
                var source = await File.ReadAllTextAsync(Path.Combine(Zone.Server.MasterPath, location));

                var script = new PythonScript(source, Zone);

                Scripts.Add(script);
                
                Logger.Information($"Loaded python from file: {source}");
            }
            else if (!string.IsNullOrEmpty(Source))
            {
                var script = new PythonScript(Source, Zone);

                Scripts.Add(script);
                
                Logger.Information($"Loaded python from source: {Source}");
            }
            else
            {
                Logger.Error($"Failed to load script pack: {location} -> {Source}");
            }

            foreach (var script in Scripts)
            {
                await script.LoadAsync();
            }
        }

        public override async Task UnloadAsync()
        {
            foreach (var script in Scripts)
            {
                await script.UnloadAsync();
            }

            Scripts.Clear();
        }

        public override async Task ReloadAsync()
        {
            await UnloadAsync();

            await LoadAsync();
        }
    }
}