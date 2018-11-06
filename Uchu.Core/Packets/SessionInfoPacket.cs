namespace Uchu.Core
{
    public class SessionInfoPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x01;

        [AutoSerialize(Wide = true)]
        public string Username { get; set; }

        [AutoSerialize(Wide = true)]
        public string UserKey { get; set; }

        [AutoSerialize]
        public string Hashed { get; set; }
    }
}