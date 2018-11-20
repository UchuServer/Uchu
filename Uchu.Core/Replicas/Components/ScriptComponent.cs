using RakDotNet;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class ScriptComponent : ReplicaComponent
    {
        public LegoDataDictionary Data { get; set; }

        public override void Serialize(BitStream stream)
        {
        }

        public override void Construct(BitStream stream)
        {
            var hasData = Data != null;
            stream.WriteBit(hasData);

            if (hasData)
                stream.WriteLDFCompressed(Data);
        }
    }
}