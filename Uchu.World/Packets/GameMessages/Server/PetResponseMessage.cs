using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PetResponseMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.PetResponse;

        public GameObject ObjIDPet;
        public int iPetCommandType;
        public int iResponse;
        public int iTypeID;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(ObjIDPet);
            writer.Write(iPetCommandType);
            writer.Write(iResponse);
            writer.Write(iTypeID);
        }
    }
}