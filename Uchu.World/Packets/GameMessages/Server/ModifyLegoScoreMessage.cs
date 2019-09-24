using RakDotNet.IO;

namespace Uchu.World
{
    public class ModifyLegoScoreMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ModifyLegoScore;

        public long Score { get; set; }

        public Lot Source { get; set; } = -1;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Score);

            var hasSource = !Source.Equals(-1);
            writer.Write(hasSource);
            if (hasSource) writer.Write(Source);
        }
    }
}