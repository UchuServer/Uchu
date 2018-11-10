using RakDotNet;

namespace Uchu.Core
{
    public class ScriptComponent : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
        }

        public override void Construct(BitStream stream)
        {
            stream.WriteBit(false);
        }
    }
}