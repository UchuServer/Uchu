using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting
{
    internal class ScriptPack
    {
        private readonly List<Script> _scripts;

        public IReadOnlyCollection<Script> Scripts => _scripts.AsReadOnly();
        
        public ScriptPack(Zone zone, Assembly assembly)
        {
            _scripts = new List<Script>();
            
            foreach (var type in assembly.GetTypes())
            {
                Logger.Information($"SCRIPT TYPE: {type}");
                
                if (type.BaseType != typeof(Script)) return;

                var zoneSpecific = type.GetCustomAttribute<ZoneSpecificAttribute>();

                if (zoneSpecific != null)
                {
                    if (zoneSpecific.ZoneId != zone.ZoneId) continue;
                }

                var instance = (Script) Activator.CreateInstance(type);
                
                Logger.Information($"{type} is script");

                instance.SetZone(zone);

                _scripts.Add(instance);
            }
        }

        public async Task LoadAsync()
        {
            foreach (var script in Scripts)
            {
                await script.LoadAsync();
            }
        }

        public async Task UnloadAsync()
        {
            foreach (var script in Scripts)
            {
                await script.UnloadAsync();
            }
        }
    }
}