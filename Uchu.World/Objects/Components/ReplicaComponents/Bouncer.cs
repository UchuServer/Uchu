using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Bouncer : ReplicaComponent
    {
        public bool PetRequired { get; set; } = false;
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Bouncer;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.WriteBit(!PetRequired);
        }
    }
}