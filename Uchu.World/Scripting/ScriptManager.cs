using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Python;
using Uchu.World.Scripting.Managed;
using Uchu.World.Scripting.Native;

namespace Uchu.World.Scripting
{
    public class ScriptManager
    {
        private Zone Zone { get; }
        
        private ManagedScriptEngine ManagedScriptEngine { get; }
        
        internal List<ScriptPack> ScriptPacks { get; private set; }

        public ScriptManager(Zone zone)
        {
            Zone = zone;
            
            ManagedScriptEngine = new ManagedScriptEngine();
        }
        
        internal async Task LoadDefaultScriptsAsync()
        {
            ScriptPacks = new List<ScriptPack>();
            
            ScriptPacks.AddRange(LoadNativeScripts());
            ScriptPacks.AddRange(await LoadManagedScriptsAsync());
        }

        private List<ScriptPack> LoadNativeScripts()
        {
            Logger.Information($"Loading native scripts...");
            
            var scriptPacks = new List<ScriptPack>();
            
            foreach (var scriptPackPath in Zone.Server.Config.DllSource.ScriptDllSource)
            {
                try
                {
                    var scriptPack = new NativeScriptPack(Zone, scriptPackPath);

                    scriptPack.ReadAssembly();
                    
                    scriptPacks.Add(scriptPack);
                    
                    Logger.Information($"Loaded {scriptPackPath} native script pack");
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            return scriptPacks;
        }

        private async Task<List<ScriptPack>> LoadManagedScriptsAsync()
        {
            Logger.Information($"Loading managed scripts...");

            var scriptPacks = new List<ScriptPack>();
            
            ManagedScriptEngine.Init();

            foreach (var script in Zone.Server.Config.ManagedScriptSources?.Scripts ?? new List<string>())
            {
                Logger.Information($"Loading {script} managed script pack");
                
                string source = default;

                var location = Path.Combine(Zone.Server.MasterPath, script);
                
                if (File.Exists(location))
                {
                    source = await File.ReadAllTextAsync(location);
                }

                var managedScript = new PythonScriptPack(Zone, location, source);

                scriptPacks.Add(managedScript);
            }

            return scriptPacks;
        }

        public async Task SetManagedScript(string location, string source = default)
        {
            var list = ScriptPacks.ToList();
            
            foreach (var pack in ScriptPacks.OfType<PythonScriptPack>())
            {
                if (pack.Name != location) continue;

                await pack.UnloadAsync();

                list.Remove(pack);
            }

            ScriptPacks = list;

            location = Path.Combine(Zone.Server.MasterPath, location);

            if (File.Exists(location))
            {
                source = await File.ReadAllTextAsync(location);
            }
            else if (string.IsNullOrEmpty(source)) return;

            var managedScript = new PythonScriptPack(Zone, location, source);

            ScriptPacks.Add(managedScript);

            await managedScript.LoadAsync();
        }
    }
}