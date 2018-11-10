using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterDeleteResponsePacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x0B;

        [AutoSerialize(Bool = true)]
        public bool Success { get; set; } = true;
    }
}