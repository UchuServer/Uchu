using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetCurrencyMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetCurrency;

        public long Currency { get; set; }

        public int LootType { get; set; } = -1;

        public Vector3 Position { get; set; }

        public Lot SourceLot { get; set; } = -1;

        public GameObject SourceGameObject { get; set; }

        public long SourceTradeId { get; set; } = -1;

        public int SourceType { get; set; } = -1;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Currency);

            var hasLootType = LootType != -1;
            writer.WriteBit(hasLootType);
            if (hasLootType) writer.Write(LootType);

            writer.Write(Position);

            var hasLot = SourceLot != -1;
            writer.WriteBit(hasLot);
            if (hasLot) writer.Write(SourceLot);

            var hasSourceObject = SourceGameObject != null;
            writer.WriteBit(hasSourceObject);
            if (hasSourceObject) writer.Write(SourceGameObject);

            var hasTradeId = SourceTradeId != -1;
            writer.WriteBit(hasTradeId);
            if (hasTradeId) writer.Write(SourceTradeId);

            var hasSourceType = SourceType != -1;
            writer.WriteBit(hasSourceType);
            if (hasSourceType) writer.Write(SourceType);
        }
    }
}