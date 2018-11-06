using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterCreateResponsePacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x07;

        [AutoSerialize]
        public CharacterCreationResponse ResponseId { get; set; }
    }
}