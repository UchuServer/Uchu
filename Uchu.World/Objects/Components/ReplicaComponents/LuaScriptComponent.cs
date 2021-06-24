using System;
using System.Linq;
using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.World
{
    public class LuaScriptComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ScriptComponent;
        
        public LegoDataDictionary Data { get; set; }
        
        public string ScriptName { get; set; }
        
        public string ClientScriptName { get; set; }

        protected LuaScriptComponent()
        {
            Listen(OnStart, () =>
            {
                // Get the script component or custom script.
                var scriptId = GameObject.Lot.GetComponentId(ComponentId.ScriptComponent);
                var script = ClientCache.GetTable<ScriptComponent>().FirstOrDefault(s => s.Id == scriptId);
                if (script == default)
                {
                    Logger.Warning($"{GameObject} has an invalid script component entry: {scriptId}");
                    return;
                }

                if (GameObject.Settings.TryGetValue("custom_script_server", out var scriptOverride))
                {
                    ScriptName = (string) scriptOverride;
                }
                else
                {
                    ScriptName = script.Scriptname;
                }
                
                // Set the script name.
                ClientScriptName = script.Clientscriptname;
                var scriptName = ScriptName ?? ClientScriptName;
                Logger.Debug($"{GameObject} -> {scriptName}");
                
                // Start the object script.
                if (scriptName == null) return;
                foreach (var (objectScriptName, objectScriptType) in Zone.ScriptManager.ObjectScriptTypes)
                {
                    if (!scriptName.ToLower().EndsWith(objectScriptName)) continue;
                    Activator.CreateInstance(objectScriptType, GameObject);
                    break;
                }
            });
        }

        public override void Construct(BitWriter writer)
        {
            var hasData = Data != null;

            writer.WriteBit(hasData);
            if (hasData) writer.WriteLdfCompressed(Data);
        }

        public override void Serialize(BitWriter writer)
        {
            
        }
    }
}