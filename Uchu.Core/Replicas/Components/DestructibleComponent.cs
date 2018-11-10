using RakDotNet;

namespace Uchu.Core
{
    public class DestructibleComponent : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
        }

        public override void Construct(BitStream stream)
        {
            stream.WriteBit(false);
            stream.WriteBit(false);
        }
    }
}