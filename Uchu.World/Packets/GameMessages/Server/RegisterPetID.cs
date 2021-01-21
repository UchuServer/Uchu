using RakDotNet.IO;

namespace Uchu.World
{
    public class RegisterPetIDMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.RegisterPetID;
        public GameObject Pet;
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Pet);
        }
    }
}