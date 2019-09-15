using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientLoadCompletePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x13;

        public ZoneId ZoneId { get; set; }

        public ushort Instance { get; set; }

        public uint Clone { get; set; }

        public override void Deserialize(BitReader reader)
        {
            ZoneId = (ZoneId) reader.Read<ushort>();
            Instance = reader.Read<ushort>();
            Clone = reader.Read<uint>();
        }
    }
}