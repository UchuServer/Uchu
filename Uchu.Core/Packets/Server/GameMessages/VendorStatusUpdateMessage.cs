using System;
using RakDotNet;

namespace Uchu.Core.Packets.Server.GameMessages
{
    public class VendorStatusUpdateMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1a1;

        public bool UpdateOnly { get; set; }

        public int[] LOTs { get; set; } = new int[0];
        
        public override void Serialize(BitStream stream)
        {
            Console.WriteLine($"Sending {GameMessageId} from {ObjectId}");
            stream.WriteBit(UpdateOnly);
            stream.WriteUInt32((uint) LOTs.Length);
            for (var index = 0; index < LOTs.Length; index++)
            {
                stream.WriteInt32(LOTs[index]);
                stream.WriteInt32(index);
            }
        }
    }
}