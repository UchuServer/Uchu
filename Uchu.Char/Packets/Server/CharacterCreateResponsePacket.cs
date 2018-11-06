using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterCreateResponsePacket : AutoSerializingPacket
    {
        public override uint PacketId => 0x07;
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        [AutoSerialize]
        public CharacterCreationResponse ResponseId { get; set; }
    }
}