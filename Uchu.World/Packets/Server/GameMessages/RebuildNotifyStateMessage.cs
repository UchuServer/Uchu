using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class RebuildNotifyStateMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0150;

        public RebuildState PreviousState { get; set; }
        public RebuildState NewState { get; set; }
        public long PlayerObjectId { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteInt((int) PreviousState);
            stream.WriteInt((int) NewState);
            stream.WriteLong(PlayerObjectId);
        }
    }
}