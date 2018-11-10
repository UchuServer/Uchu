using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public class WorldInfoPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x02;
        
        [AutoSerialize]
        public ZoneId ZoneId { get; set; }
        
        [AutoSerialize]
        public ushort Instance { get; set; }
        
        [AutoSerialize]
        public uint Clone { get; set; }
        
        [AutoSerialize]
        public ZoneChecksum Checksum { get; set; }

        [AutoSerialize]
        public ushort Unknown1 { get; set; } = 0;
        
        [AutoSerialize]
        public Vector3 Position { get; set; }

        [AutoSerialize]
        public WorldType Type { get; set; } = WorldType.Normal;
    }
}