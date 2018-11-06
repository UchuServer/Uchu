using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterListRequestPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x02;
    }
}