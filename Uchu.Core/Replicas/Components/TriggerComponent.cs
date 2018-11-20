using RakDotNet;

namespace Uchu.Core
{
    public class TriggerComponent : ReplicaComponent
    {
        public int TriggerId { get; set; } = -1;

        public override void Serialize(BitStream stream)
        {
            var hasId = TriggerId != -1;

            stream.WriteBit(hasId);

            if (hasId)
                stream.WriteInt(TriggerId);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}