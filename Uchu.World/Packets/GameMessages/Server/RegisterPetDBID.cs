using RakDotNet.IO;

namespace Uchu.World
{
    public class RegisterPetDBIDMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.RegisterPetDBID;
        public GameObject PetItemObject;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(PetItemObject);
        }
    }
}