using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class DropClientLootMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x001E;

        public bool UsePosition { get; set; } = false;

        public Vector3 FinalPosition { get; set; } = Vector3.Zero;

        public int Currency { get; set; }

        public int ItemLOT { get; set; }

        public long LootObjectId { get; set; }

        public long OwnerObjectId { get; set; }

        public long SourceObjectId { get; set; }

        public Vector3 SpawnPosition { get; set; } = Vector3.Zero;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(UsePosition);

            var hasFinal = FinalPosition != Vector3.Zero;

            stream.WriteBit(hasFinal);

            if (hasFinal)
            {
                stream.WriteFloat(FinalPosition.X);
                stream.WriteFloat(FinalPosition.Y);
                stream.WriteFloat(FinalPosition.Z);
            }

            stream.WriteInt(Currency);
            stream.WriteInt(ItemLOT);
            stream.WriteLong(LootObjectId);
            stream.WriteLong(OwnerObjectId);
            stream.WriteLong(SourceObjectId);

            var hasSpawn = SpawnPosition != Vector3.Zero;

            stream.WriteBit(hasSpawn);

            if (hasSpawn)
            {
                stream.WriteFloat(SpawnPosition.X);
                stream.WriteFloat(SpawnPosition.Y);
                stream.WriteFloat(SpawnPosition.Z);
            }
        }
    }
}