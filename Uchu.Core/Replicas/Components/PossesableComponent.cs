using RakDotNet;

namespace Uchu.Core
{
    public class PossesableComponent : ReplicaComponent
    {
        public long DriverObjectId { get; set; } = -1;

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);

            var hasDriver = DriverObjectId != -1;

            stream.WriteBit(hasDriver);

            if (hasDriver)
            {
                stream.WriteLong(DriverObjectId);
            }

            stream.WriteBit(false);

            stream.WriteBit(false);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}