using RakDotNet.IO;

namespace Uchu.World
{
    public class ModifyLegoScoreMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ModifyLegoScore;

        public long Score { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Score);

            writer.WriteBit(true);
            writer.Write(2);
        }
    }
}