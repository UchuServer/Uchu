using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterDeleteResponse : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0xB;

        public bool Success { get; set; } = true;
        
        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) (Success ? 1 : 0));
        }
    }
}