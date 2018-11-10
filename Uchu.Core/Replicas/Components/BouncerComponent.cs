using RakDotNet;

namespace Uchu.Core
{
    public class BouncerComponent  : ReplicaComponent
    {
        public bool PetRequired { get; set; } = false;

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteBit(!PetRequired);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}