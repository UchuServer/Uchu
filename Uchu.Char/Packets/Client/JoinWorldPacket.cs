using Uchu.Core;

namespace Uchu.Char
{
    public class JoinWorldPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x04;

        [AutoSerialize]
        public long CharacterId { get; set; }
    }
}