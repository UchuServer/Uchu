using RakDotNet.IO;

namespace Uchu.World
{
    public class ResurrectMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0xA0;

        public bool ResurrectImminently { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(ResurrectImminently);
        }
    }
}