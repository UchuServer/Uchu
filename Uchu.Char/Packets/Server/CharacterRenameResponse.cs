using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterRenameResponse : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x8;

        public CharacterRenamingResponse ResponseId { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) ResponseId);
        }
    }
}