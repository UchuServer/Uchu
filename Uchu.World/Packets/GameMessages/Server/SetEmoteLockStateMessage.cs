using RakDotNet.IO;

namespace Uchu.World
{
    public class SetEmoteLockStateMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetEmoteLockState;
        
        public bool Lock { get; set; }
        
        public int EmoteId { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Lock);
            writer.Write(EmoteId);
        }
    }
}