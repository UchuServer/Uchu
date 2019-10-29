using System.Linq;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;

namespace Uchu.World
{
    public class LuaScriptComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ScriptComponent;
        
        public LegoDataDictionary Data { get; set; }
        
        public string ScriptName { get; set; }
        
        public string ClientScriptName { get; set; }

        public LuaScriptComponent()
        {
            OnStart.AddListener(() =>
            {
                using var ctx = new CdClientContext();
            
                var scriptId = GameObject.Lot.GetComponentId(ComponentId.ScriptComponent);

                var script = ctx.ScriptComponentTable.FirstOrDefault(s => s.Id == scriptId);

                if (script == default)
                {
                    Logger.Warning($"{GameObject} has an invalid script component entry: {scriptId}");
                    
                    return;
                }
                
                ScriptName = script.Scriptname;
                ClientScriptName = script.Clientscriptname;
            
                Logger.Debug($"{GameObject} -> {ScriptName}");
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