using RakDotNet.IO;

namespace Uchu.World
{
    public class ActivityStartMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ActivityStart;
        
        public override void SerializeMessage(BitWriter writer)
        {
            
        }
    }
}