using RakDotNet;

namespace Uchu.Core
{
    public class SwitchComponent : ReplicaComponent
    {
        public bool IsActive { get; set; } = false;

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(IsActive);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}