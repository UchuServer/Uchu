using RakDotNet;

namespace Uchu.Core
{
    public class TriggerComponent : ReplicaComponent
    {
        public int TriggerId { get; set; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteInt(TriggerId);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}