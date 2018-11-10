using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterDeleteRequestPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x06;
        
        [AutoSerialize]
        public long CharacterId { get; set; }
    }
}