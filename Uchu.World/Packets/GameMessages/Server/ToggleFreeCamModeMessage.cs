using RakDotNet.IO;

namespace Uchu.World
{
    public class ToggleFreeCamModeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ToggleFreeCamMode;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}