using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyClientFlagChangeMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x01D8;

        public bool Flag { get; set; }
        public int FlagId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(Flag);
            stream.WriteInt(FlagId);
        }
    }
}