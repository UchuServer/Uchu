using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Scripting;

namespace Uchu.World
{
    public class ScriptManager
    {
        private readonly Zone _zone;
        
        internal ScriptPack[] ScriptPacks { get; private set; }

        public ScriptManager(Zone zone)
        {
            _zone = zone;
        }
        
        internal void ReadAssemblies()
        {
            var scriptPacks = new List<ScriptPack>();

            var path = Path.Combine(Directory.GetCurrentDirectory(), _zone.Server.Config.DllSource.ServerDllSourcePath);

            var libraries = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            
            foreach (var scriptPackName in _zone.Server.Config.DllSource.ScriptDllSource)
            {
                string dll = default;
                
                foreach (var library in libraries)
                {
                    if (Path.GetFileName(library) != $"{scriptPackName}.dll") continue;

                    dll = library;
                }

                if (dll == default)
                {
                    Logger.Error($"Could not find DLL for script pack: {scriptPackName}");
                    
                    return;
                }

                try
                {
                    var assembly = Assembly.LoadFrom(dll);

                    var scriptPack = new ScriptPack(_zone, assembly);

                    scriptPack.ReadAssembly();
                    
                    Logger.Information($"Loaded {scriptPackName} script pack");
                    
                    scriptPacks.Add(scriptPack);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }

            ScriptPacks = scriptPacks.ToArray();
        }
    }
}