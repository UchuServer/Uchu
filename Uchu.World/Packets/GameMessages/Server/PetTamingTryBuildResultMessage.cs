using RakDotNet.IO;

namespace Uchu.World
{
    public class PetTamingTryBuildResultMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.PetTamingTryBuildResult;

        public bool bSuccess = true;
        public int iNumCorrect = 0;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(bSuccess);

            writer.WriteBit(iNumCorrect != 0);
            if (iNumCorrect != 0)
            {
                writer.Write<int>(iNumCorrect);
            }
        }
    }
}