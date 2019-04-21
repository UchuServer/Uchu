using System;
using RakDotNet;

namespace Uchu.Core
{
    public class VendorStatusUpdateMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1a1;

        public bool UpdateOnly { get; set; }

        public Tuple<int, int>[] LOTs { get; set; } = new Tuple<int, int>[0];
        
        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(UpdateOnly);
            stream.WriteUInt32((uint) LOTs.Length);
            foreach (var (item1, item2) in LOTs)
            {
                stream.WriteInt32(item1);
                stream.WriteInt32(item2);
            }
        }
    }
}