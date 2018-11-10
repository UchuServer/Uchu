using RakDotNet;

namespace Uchu.Core
{
    public class CollectibleComponent : ReplicaComponent
    {
        public ushort CollectibleId { get; set; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteUShort(CollectibleId);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}