using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class CelebrationCompletedMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.CelebrationCompleted;
        public string Animation { get; set; }
        public int CelebrationID { get; set; } = -1;
    }
}