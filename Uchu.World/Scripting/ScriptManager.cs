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
        /// <summary>
        /// Object scripts types in the script manager.
        /// </summary>
        public Dictionary<string, Type> ObjectScriptTypes { get; } = new Dictionary<string, Type>();
        
        private Zone Zone { get; }
        
        private ManagedScriptEngine ManagedScriptEngine { get; }
        
        internal List<ScriptPack> ScriptPacks { get; private set; }

        public ScriptManager(Zone zone)
        {
            Zone = zone;
            
            ManagedScriptEngine = new ManagedScriptEngine();
        }

        /// <summary>
        /// Loads the zone scripts.
        /// </summary>
        internal async Task LoadDefaultScriptsAsync()
        {
            ScriptPacks = new List<ScriptPack>();
            
            ScriptPacks.AddRange(LoadNativeScripts());
            ScriptPacks.AddRange(await LoadManagedScriptsAsync());
            
            // Store the object script types.
            foreach (var pack in ScriptPacks)
            {
                if (!(pack is NativeScriptPack nativePack)) continue;
                foreach (var (scriptName, objectScriptType) in nativePack.ObjectScriptTypes)
                {
                    if (ObjectScriptTypes.ContainsKey(scriptName))
                    {
                        Logger.Warning($"Object script time registered multiple times: {scriptName}");
                    }
                    ObjectScriptTypes[scriptName] = objectScriptType;
                    Logger.Information($"Registered object script {scriptName}");
                }
            }
        }

        private List<ScriptPack> LoadNativeScripts()
        {
            Logger.Information($"Loading native scripts...");
            
            var scriptPacks = new List<ScriptPack>();
            
            foreach (var scriptPackPath in UchuServer.Config.DllSource.ScriptDllSource)
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

            foreach (var script in UchuServer.Config.ManagedScriptSources?.Scripts ?? new List<string>())
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