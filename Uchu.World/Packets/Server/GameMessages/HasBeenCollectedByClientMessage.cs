using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class HasBeenCollectedByClientMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x01E7;

        public long PlayerObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteLong(PlayerObjectId);
        }
    }
}