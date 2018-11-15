using RakDotNet;

namespace Uchu.Core
{
    public class PetComponent : ReplicaComponent
    {
        public long OwnerObjectId { get; set; } = -1;

        public string Name { get; set; } = "";
        public string OwnerName { get; set; } = "";

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteUInt(0);
            stream.WriteUInt(0);
            stream.WriteBit(false);

            var hasOwner = OwnerObjectId != -1;

            stream.WriteBit(hasOwner);

            if (hasOwner)
                stream.WriteLong(OwnerObjectId);

            stream.WriteBit(true);
            stream.WriteUInt(0);
            stream.WriteByte((byte) Name.Length);
            stream.WriteString(Name, Name.Length, true);
            stream.WriteByte((byte) OwnerName.Length);
            stream.WriteString(OwnerName, OwnerName.Length, true);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}