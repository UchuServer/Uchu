using RakDotNet.IO;

namespace Uchu.World
{
    public class ToggleGhostReferenceOverrideMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ToggleGhostReferenceOverride;
        
        public bool Override { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Override = reader.ReadBit();
        }
    }
}