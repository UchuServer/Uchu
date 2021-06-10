using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestMoveItemBetweenInventoryTypesMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestMoveItemBetweenInventoryTypes;

        public bool AllowPartial { get; set; } = true;

        public uint Count { get; set; } = 1;

        public InventoryType Destination { get; set; } = InventoryType.Items;

        public InventoryType Source { get; set; } = InventoryType.Items;

        public Item Item { get; set; }

        public bool ShowFlyingLoot { get; set; } = false;

        public ObjectId Subkey { get; set; } = ObjectId.Invalid;

        public Lot TemplateId { get; set; } = 0;

        public int DestinationSlot { get; set; } = -1;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit()) AllowPartial = reader.ReadBit();

            if (reader.ReadBit()) Count = reader.Read<uint>();

            if (reader.ReadBit()) Destination = (InventoryType) reader.Read<int>();

            if (reader.ReadBit()) Source = (InventoryType) reader.Read<int>();

            if (reader.ReadBit()) Item = reader.ReadGameObject<Item>(Associate.Zone);

            if (reader.ReadBit()) ShowFlyingLoot = reader.ReadBit();

            if (reader.ReadBit()) Subkey = reader.Read<ObjectId>();

            if (reader.ReadBit()) TemplateId = reader.Read<Lot>();

            if (reader.ReadBit()) DestinationSlot = reader.Read<int>();
        }
    }
}
