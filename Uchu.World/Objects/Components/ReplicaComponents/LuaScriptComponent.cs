using System.Linq;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class LuaScriptComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.ScriptComponent;
        
        public LegoDataDictionary Data { get; set; }
        
        public string ScriptName { get; set; }
        
        public string ClientScriptName { get; set; }

        public override void FromLevelObject(LevelObject levelObject)
        {
            using var ctx = new CdClientContext();
            
            var scriptId = levelObject.Lot.GetComponentId(ComponentId.ScriptComponent);

            var script = ctx.ScriptComponentTable.First(s => s.Id == scriptId);

            ScriptName = script.Scriptname;
            ClientScriptName = script.Clientscriptname;
            
            Logger.Debug($"{GameObject} -> {ScriptName}");
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