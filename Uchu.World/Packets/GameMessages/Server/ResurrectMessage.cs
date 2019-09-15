using RakDotNet.IO;

namespace Uchu.World
{
    public class ResurrectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Resurrect;

        public bool ResurrectImminently { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(ResurrectImminently);
        }
    }
}