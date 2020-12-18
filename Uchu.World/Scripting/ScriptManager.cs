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
        
        internal List<NativeScriptPack> NativeScriptPacks { get; private set; }
        internal List<PythonScriptPack> ManagedScriptPacks { get; private set; }

        public ScriptManager(Zone zone)
        {
            Zone = zone;
            
            ManagedScriptEngine = new ManagedScriptEngine();
        }
        
        internal async Task LoadDefaultScriptsAsync()
        {
            NativeScriptPacks = new List<NativeScriptPack>();
            NativeScriptPacks.AddRange(LoadNativeScripts());
            ManagedScriptPacks.AddRange(await LoadManagedScriptsAsync());
        }

        private List<NativeScriptPack> LoadNativeScripts()
        {
            Logger.Information($"Loading native scripts...");
            
            var scriptPacks = new List<NativeScriptPack>();

            foreach (var scriptPackPath in Zone.UchuServer.Config.DllSource.ScriptDllSource)
            {
                if (scriptPackPath == "Uchu.StandardScripts") continue;
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

        private async Task<List<PythonScriptPack>> LoadManagedScriptsAsync()
        {
            Logger.Information($"Loading managed scripts...");

            var scriptPacks = new List<PythonScriptPack>();
            
            ManagedScriptEngine.Init();

            foreach (var script in Zone.UchuServer.Config.ManagedScriptSources?.Scripts ?? new List<string>())
            {
                Logger.Information($"Loading {script} managed script pack");
                
                string source = default;

                var location = Path.Combine(Zone.UchuServer.MasterPath, script);
                
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
            var list = ManagedScriptPacks.ToList();
            
            foreach (var pack in ManagedScriptPacks)
            {
                if (pack.Name != location) continue;

                await pack.UnloadAsync();

                list.Remove(pack);
            }

            ManagedScriptPacks = list;

            location = Path.Combine(Zone.UchuServer.MasterPath, location);

            if (File.Exists(location))
            {
                source = await File.ReadAllTextAsync(location);
            }
            else if (string.IsNullOrEmpty(source)) return;

            var managedScript = new PythonScriptPack(Zone, location, source);

            ManagedScriptPacks.Add(managedScript);

            await managedScript.LoadAsync();
        }
    }
}