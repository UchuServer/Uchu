using System;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class SellToVendorMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0176;

        public int Count { get; set; } = 1;
        
        public long ItemId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            if (stream.ReadBit())
            {
                Count = stream.ReadInt32();
            }

            ItemId = stream.ReadInt64();
        }
    }
}