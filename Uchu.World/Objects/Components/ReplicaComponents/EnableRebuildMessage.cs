using RakDotNet.IO;

namespace Uchu.World
{
    public class EnableRebuildMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.EnableRebuild;
        
        public bool Enable { get; set; }
        
        public bool IsFail { get; set; }
        
        public bool IsSuccess { get; set; }
        
        public RebuildFailReason FailReason { get; set; }
        
        public float Duration { get; set; }
        
        public Player Player { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Enable);
            writer.WriteBit(IsFail);
            writer.WriteBit(IsSuccess);

            if (writer.Flag(FailReason != RebuildFailReason.Canceled))
            {
                writer.Write((uint) FailReason);
            }

            writer.Write(Duration);
            writer.Write(Player);
        }
    }
}