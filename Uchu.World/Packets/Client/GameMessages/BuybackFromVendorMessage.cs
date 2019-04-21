using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class BuybackFromVendorMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0546;

        public bool Confirmed { get; set; }
        
        public int Count { get; set; } = 1;
        
        public long ItemId { get; set; }
        
        public override void Deserialize(BitStream stream)
        {
            Confirmed = stream.ReadBit();
            
            if (stream.ReadBit())
            {
                Count = stream.ReadInt32();
            }

            ItemId = stream.ReadInt64();
        }
    }
}