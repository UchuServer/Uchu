using Uchu.Core;

namespace Uchu.World
{
    public class PlayerReadyMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1FD;
    }
}