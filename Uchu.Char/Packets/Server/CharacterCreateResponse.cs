using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterCreateResponse : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x7;
        
        public CharacterCreationResponse ResponseId { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) ResponseId);
        }
    }
}