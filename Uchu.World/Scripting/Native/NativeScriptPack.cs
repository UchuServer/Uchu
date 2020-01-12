using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting.Native
{
    internal class NativeScriptPack : ScriptPack
    {
        private List<NativeScript> _scripts;

        private Assembly _assembly;

        private AssemblyLoadContext _context;

        public override string Name => Path.GetFileNameWithoutExtension(Location);

        public IEnumerable<NativeScript> Scripts => _scripts.AsReadOnly();
        
        public NativeScriptPack(Zone zone, string location) : base(zone, location)
        {
            _scripts = new List<NativeScript>();
            
            _context = new AssemblyLoadContext($"{Path.GetFileNameWithoutExtension(Location)} Assembly", true);
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
            _assembly = _context.LoadFromAssemblyPath(Location);
            
            _scripts = new List<NativeScript>();

            foreach (var type in _assembly.GetTypes())
            {
                if (type.BaseType != typeof(NativeScript)) return;

                var zoneSpecific = type.GetCustomAttribute<ZoneSpecificAttribute>();

                if (zoneSpecific != null)
                {
                    if (zoneSpecific.ZoneId != Zone.ZoneId) continue;
                }

                var instance = (NativeScript) Activator.CreateInstance(type);
                
                instance.SetZone(Zone);

                _scripts.Add(instance);
            }
        }

        public override async Task LoadAsync()
        {
            foreach (var script in Scripts)
            {
                Logger.Information($"Running native script: {script}");
                
                try
                {
                    await script.LoadAsync();
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
                
                Logger.Information($"Ran native script: {script}");
            }
        }

        public override async Task UnloadAsync()
        {
            foreach (var script in Scripts)
            {
                await script.InternalUnloadAsync();
            }

            _scripts = new List<NativeScript>();
        }

        public override async Task ReloadAsync()
        {
            await UnloadAsync();

            ReloadAssembly();
            
            await LoadAsync();
        }
    }
}