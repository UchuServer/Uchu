using RakDotNet.IO;

namespace Uchu.World
{
    public class PetTamingMinigameResultMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.PetTamingMinigameResult;
        public bool bSuccess;
        public override void Deserialize(BitReader reader)
        {
            bSuccess = reader.ReadBit();
        }
    }
}