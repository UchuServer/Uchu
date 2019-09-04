using RakDotNet.IO;

namespace Uchu.World
{
    public class ToggleFreeCamModeMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x1F8;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}