namespace Uchu.Core
{
    public class DisconnectNotifyPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.General;
        public override uint PacketId => 0x01;

        [AutoSerialize]
        public DisconnectId DisconnectId { get; set; }
    }
}