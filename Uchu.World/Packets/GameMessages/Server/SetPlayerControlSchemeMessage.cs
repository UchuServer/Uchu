using RakDotNet.IO;

namespace Uchu.World
{
    public class SetPlayerControlSchemeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetPlayerControlScheme;
        
        public bool DelayCameraSwitchIfInCinematic { get; set; }
        
        public bool SwitchCamera { get; set; }
        
        public int ControlScheme { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(DelayCameraSwitchIfInCinematic);

            writer.WriteBit(SwitchCamera);

            if (writer.Flag(ControlScheme != 0))
                writer.Write(ControlScheme);
        }
    }
}