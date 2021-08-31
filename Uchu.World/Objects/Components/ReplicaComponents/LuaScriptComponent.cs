using System;
using System.Linq;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

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
                var script = ClientCache.Find<ScriptComponent>(scriptId);
                if (script == default)
                {
                    Logger.Warning($"{GameObject} has an invalid script component entry: {scriptId}");
                    return;
                }

                // Set the server script name.
                if (GameObject.Settings.TryGetValue("custom_script_server", out var serverScriptOverride) && (string) serverScriptOverride != "")
                {
                    this.ScriptName = (string) serverScriptOverride;
                }
                else
                {
                    this.ScriptName = script.Scriptname;
                }
                
                // Set the client script name.
                if (GameObject.Settings.TryGetValue("custom_script_client", out var clientScriptOverride) && (string) clientScriptOverride != "")
                {
                    this.ClientScriptName = (string) clientScriptOverride;
                }
                else
                {
                    this.ClientScriptName = script.Scriptname;
                }
                Logger.Debug($"{GameObject} -> {this.ScriptName}, {this.ClientScriptName}");
                
                // Start the object script.
                this.Zone.LoadScriptForObject(this.GameObject);
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
