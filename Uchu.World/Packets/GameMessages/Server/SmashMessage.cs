using RakDotNet.IO;

namespace Uchu.World
{
    public class SmashMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Smash;
        
        public bool IgnoreObjectVisibility { get; set; }
        
        public float Force { get; set; }
        
        public float GhostOpacity { get; set; }
        
        public GameObject Killer { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(IgnoreObjectVisibility);

            writer.Write(Force);
            writer.Write(GhostOpacity);

            writer.Write(Killer);
        }
    }
}