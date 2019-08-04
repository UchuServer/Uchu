using RakDotNet.IO;

namespace Uchu.World
{
    public class ReadyForUpdateMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x378;

        public long GameObject { get; set; }

        public override void Deserialize(BitReader reader)
        {
            GameObject = reader.Read<long>();
        }
    }
}