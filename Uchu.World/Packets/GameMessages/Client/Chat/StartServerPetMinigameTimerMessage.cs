using RakDotNet.IO;

namespace Uchu.World
{
    public class StartServerPetMinigameTimerMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.StartServerPetMinigameTimer;

        public override void Deserialize(BitReader reader)
        {
            
        }
    }
}