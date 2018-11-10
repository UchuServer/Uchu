using Uchu.Core;

namespace Uchu.World
{
    public class ClientLoadCompletePacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x13;
        
        [AutoSerialize]
        public ZoneId ZoneId { get; set; }
        
        [AutoSerialize]
        public ushort Instance { get; set; }
        
        [AutoSerialize]
        public uint Clone { get; set; }
    }
}