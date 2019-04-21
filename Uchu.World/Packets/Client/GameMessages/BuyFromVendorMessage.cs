using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class BuyFromVendorMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0175;
        
        public bool Confirmed { get; set; }

        public int Count { get; set; } = 1;
        
        public int LOT { get; set; }

        public override void Deserialize(BitStream stream)
        {
            Confirmed = stream.ReadBit();
            
            if (stream.ReadBit())
            {
                Count = stream.ReadInt32();
            }

            LOT = stream.ReadInt32();
        }
    }
}