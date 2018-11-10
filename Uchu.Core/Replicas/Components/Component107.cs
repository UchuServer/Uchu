using RakDotNet;

namespace Uchu.Core
{
    public class Component107 : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(false);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}