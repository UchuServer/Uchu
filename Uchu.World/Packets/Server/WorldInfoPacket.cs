using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class WorldInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x2;

        public ZoneId ZoneId { get; set; }
        
        public ushort Instance { get; set; }
        
        public uint Clone { get; set; }
        
        public ZoneChecksum Checksum { get; set; }
        
        public Vector3 SpawnPosition { get; set; }

        public WorldType WorldType { get; set; } = WorldType.Normal;
        
        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((ushort) ZoneId);

            writer.Write(Instance);

            writer.Write(Clone);

            writer.Write((uint) Checksum);

            writer.Write(SpawnPosition);

            writer.Write((uint) WorldType);
        }
    }
}