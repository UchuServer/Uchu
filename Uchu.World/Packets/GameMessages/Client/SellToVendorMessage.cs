using RakDotNet.IO;

namespace Uchu.World
{
    public class SellToVendorMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SellToVendor;

        public int Count { get; set; } = 1;
        
        public Item Item { get; set; }

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit())
            {
                Count = reader.Read<int>();
            }

            Item = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}