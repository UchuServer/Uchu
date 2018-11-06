namespace Uchu.Core
{
    public class ServerRedirectionPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x0E;

        [AutoSerialize]
        public string Address { get; set; }

        [AutoSerialize]
        public ushort Port { get; set; }

        [AutoSerialize(Bool = true)]
        public bool Forced { get; set; } = false;
    }
}