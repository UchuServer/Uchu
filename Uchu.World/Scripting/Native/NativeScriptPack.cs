using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Scripting.Native
{
    internal class NativeScriptPack : ScriptPack
    {
        private List<NativeScript> _scripts;

        private Assembly _assembly;

        public override string Name => Location;

        public IEnumerable<NativeScript> Scripts => _scripts.AsReadOnly();
        
        public NativeScriptPack(Zone zone, string location) : base(zone, location)
        {
            _scripts = new List<NativeScript>();
        }

        internal void ReadAssembly()
        {
            _assembly = Assembly.Load(File.ReadAllBytes(Location));
            
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

            await LoadAsync();
        }
    }
}