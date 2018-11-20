using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class SetCurrencyMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0085;

        public long Currency { get; set; }
        public int LootType { get; set; } = -1;
        public Vector3 Position { get; set; }
        public int SourceLOT { get; set; } = -1;
        public long SourceObjectId { get; set; } = -1;
        public long SourceTradeId { get; set; } = -1;
        public int SourceType { get; set; } = -1;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteLong(Currency);

            var hasType = LootType != -1;

            stream.WriteBit(hasType);

            if (hasType)
                stream.WriteInt(LootType);

            stream.WriteFloat(Position.X);
            stream.WriteFloat(Position.Y);
            stream.WriteFloat(Position.Z);

            var hasLOT = SourceLOT != -1;

            stream.WriteBit(hasLOT);

            if (hasLOT)
                stream.WriteInt(SourceLOT);

            var hasObject = SourceObjectId != -1;

            stream.WriteBit(hasObject);

            if (hasObject)
                stream.WriteLong(SourceObjectId);

            var hasTrade = SourceTradeId != -1;

            stream.WriteBit(hasTrade);

            if (hasTrade)
                stream.WriteLong(SourceTradeId);

            var hasSrcType = SourceType != -1;

            stream.WriteBit(hasSrcType);

            if (hasSrcType)
                stream.WriteInt(SourceType);
        }
    }
}