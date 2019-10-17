using RakDotNet.IO;

namespace Uchu.World
{
    public class BuybackFromVendorMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.BuybackFromVendor;
        
        public bool Confirmed { get; set; }

        public int Count { get; set; } = 1;
        
        public Item Item { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Confirmed = reader.ReadBit();

            if (reader.ReadBit())
            {
                Count = reader.Read<int>();
            }

            Item = reader.ReadGameObject<Item>(Associate.Zone);
        }
    }
}