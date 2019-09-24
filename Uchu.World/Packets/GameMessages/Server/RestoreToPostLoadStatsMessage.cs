using RakDotNet.IO;

namespace Uchu.World
{
    public class RestoreToPostLoadStatsMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RestoreToPostLoadStats;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}