using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Scripting;

namespace Uchu.World
{
    public partial class Zone
    {
        internal ScriptPack[] ScriptPacks { get; private set; }

        private async Task LoadScripts()
        {
            var scriptPacks = new List<ScriptPack>();

            var path = Path.Combine(Directory.GetCurrentDirectory(), Server.Config.DllSource.ServerDLLSourcePath);

            var libraries = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            
            foreach (var scriptPackName in Server.Config.DllSource.ScriptDLLSource)
            {
                string? dll = default;
                
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
                    var assembly = Assembly.LoadFile(dll);

                    var scriptPack = new ScriptPack(this, assembly);

                    await scriptPack.LoadAsync();
                    
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