using RakDotNet.IO;
using Uchu.World.Collections;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class LuaScriptComponent : ReplicaComponent
    {
        public LegoDataDictionary Data { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Script;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
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