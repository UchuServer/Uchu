using RakDotNet.IO;

namespace Uchu.World
{
    public class BuyFromVendorMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.BuyFromVendor;
        
        public bool Confirmed { get; set; }

        public int Count { get; set; } = 1;
        
        public Lot Lot { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Confirmed = reader.ReadBit();

            if (reader.ReadBit())
            {
                Count = reader.Read<int>();
            }

            Lot = reader.Read<Lot>();
        }
    }
}