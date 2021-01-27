using RakDotNet.IO;

namespace Uchu.World
{
    public class ClientExitTamingMinigameMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.ClientExitTamingMinigame;

        private bool bVoluntaryExit = true;
        public override void Deserialize(BitReader reader)
        {
            bVoluntaryExit = reader.ReadBit();
        }
    }
}