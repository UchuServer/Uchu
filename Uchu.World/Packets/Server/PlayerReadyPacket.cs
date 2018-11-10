using Uchu.Core;

namespace Uchu.World
{
    public class PlayerReadyPacket : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1FD;
    }
}