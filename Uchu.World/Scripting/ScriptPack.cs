using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting
{
    internal class ScriptPack
    {
        private readonly Zone _zone;
        
        private List<Script> _scripts;

        private readonly string _location;

        private Assembly _assembly;

        private AssemblyLoadContext _context;

        public string Name => Path.GetFileNameWithoutExtension(_location);

        public IEnumerable<Script> Scripts => _scripts.AsReadOnly();
        
        public ScriptPack(Zone zone, string location)
        {
            _scripts = new List<Script>();

            _location = location;

            _zone = zone;
            
            _context = new AssemblyLoadContext($"{Name} Assembly", true);
        }

        internal void ReloadAssembly()
        {
            _context.Unload();
            
            _context = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            
            _context = new AssemblyLoadContext($"{Name} Assembly", true);
            
            _assembly = null;
            
            ReadAssembly();
        }

        internal void ReadAssembly()
        {
            _assembly = _context.LoadFromAssemblyPath(_location);
            
            _scripts = new List<Script>();

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
                await script.InternalUnloadAsync();
            }

            _scripts = new List<Script>();
        }

        public async Task ReloadAsync()
        {
            await UnloadAsync();

            ReloadAssembly();
            
            await LoadAsync();
        }
    }
}