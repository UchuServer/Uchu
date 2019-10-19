using RakDotNet.IO;

namespace Uchu.World
{
    public class EmotePlayedMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.EmotePlayed;
        
        public int EmoteId { get; set; }
        
        public GameObject Target { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(EmoteId);

            writer.Write(Target);
        }
    }
}