using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class CelebrationCompletedMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.CelebrationCompleted;
        public string Animation { get; set; }
        public int CelebrationToFinishID { get; set; } = -1;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint)Animation.Length);
            writer.WriteString(Animation, Animation.Length, true);

            writer.WriteBit(CelebrationToFinishID != -1);
            if (CelebrationToFinishID != -1) writer.Write(CelebrationToFinishID);
        }
    }
}