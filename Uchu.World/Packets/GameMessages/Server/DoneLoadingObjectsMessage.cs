using RakDotNet.IO;

namespace Uchu.World
{
    public class DoneLoadingObjectsMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x66A;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}