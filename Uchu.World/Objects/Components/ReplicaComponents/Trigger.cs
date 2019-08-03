using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Trigger : ReplicaComponent
    {
        public int TriggerId { get; set; } = -1;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Trigger;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            var hasId = TriggerId != -1;

            writer.WriteBit(hasId);

            if (hasId) writer.Write(TriggerId);
        }
    }
}