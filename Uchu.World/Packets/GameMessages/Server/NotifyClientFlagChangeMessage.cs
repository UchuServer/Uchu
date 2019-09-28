using RakDotNet.IO;

namespace Uchu.World
{
    public class NotifyClientFlagChangeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyClientFlagChange;
        
        public bool Flag { get; set; }
        
        public int FlagId { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Flag);
            writer.Write(FlagId);
        }
    }
}