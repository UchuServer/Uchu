using RakDotNet.IO;

namespace Uchu.World
{
    public class ServerTerminateInteractionMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ServerTerminateInteraction;
        
        public GameObject Terminator { get; set; }
            
        public TerminateType Type { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Terminator);

            writer.Write((int) Type);
        }
    }
}