using RakDotNet.IO;

namespace Uchu.World
{
    public class MatchResponse : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MatchResponse;

        public int Response { get; set; } // TODO: Can/should this be an enum?
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Response);
        }
    }
}