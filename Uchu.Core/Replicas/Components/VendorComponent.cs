using RakDotNet;

namespace Uchu.Core
{
    public class VendorComponent : ReplicaComponent
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