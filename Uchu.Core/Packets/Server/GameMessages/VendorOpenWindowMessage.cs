using System;
using RakDotNet;

namespace Uchu.Core.Packets.Server.GameMessages
{
    public class VendorOpenWindowMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x171;

        public override void Serialize(BitStream stream)
        {
            Console.WriteLine($"Sending {GameMessageId} from {ObjectId}");
        }
    }
}