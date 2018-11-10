using Uchu.Core;

namespace Uchu.World
{
    public class DoneLoadingObjectsPacket : ServerGameMessage
    {
        public override ushort GameMessageId => 0x066A;
    }
}