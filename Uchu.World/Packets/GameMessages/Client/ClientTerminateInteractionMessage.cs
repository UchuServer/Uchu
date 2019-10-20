using RakDotNet.IO;

namespace Uchu.World
{
    public class ClientTerminateInteractionMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.TerminateInteraction;

        public GameObject Terminator { get; set; }
        
        public TerminateType Type { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Terminator = reader.ReadGameObject(Associate.Zone);

            Type = (TerminateType) reader.Read<int>();
        }
    }
}