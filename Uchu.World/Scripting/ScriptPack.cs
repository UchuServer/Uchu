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

        private readonly Assembly _assembly;

        private readonly Zone _zone;

        public IEnumerable<Script> Scripts => _scripts.AsReadOnly();
        
        public ScriptPack(Zone zone, Assembly assembly)
        {
            _scripts = new List<Script>();

            _assembly = assembly;

            _zone = zone;
        }

        internal void ReadAssembly()
        {
            foreach (var type in _assembly.GetTypes())
            {
                if (type.BaseType != typeof(Script)) return;

                var zoneSpecific = type.GetCustomAttribute<ZoneSpecificAttribute>();

                if (zoneSpecific != null)
                {
                    if (zoneSpecific.ZoneId != _zone.ZoneId) continue;
                }

                var instance = (Script) Activator.CreateInstance(type);
                
                instance.SetZone(_zone);

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