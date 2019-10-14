using RakDotNet.IO;

namespace Uchu.World
{
    public class ChangeObjectWorldStateMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ChangeObjectWorldState;
        
        public ObjectWorldState State { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((int) State);
        }
    }
}