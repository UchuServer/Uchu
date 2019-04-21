using System;
using RakDotNet;

namespace Uchu.Core
{
    public class VendorOpenWindowMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x171;

        public override void SerializeMessage(BitStream stream)
        {
            Console.WriteLine($"Sending {GameMessageId} from {ObjectId}");
        }
    }
}