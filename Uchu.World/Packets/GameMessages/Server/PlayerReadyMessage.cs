using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayerReadyMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1FD;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}