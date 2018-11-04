using System.Diagnostics;

namespace Uchu.Core
{
    public class HandshakePacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.General;
        public override uint PacketId => 0x00;

        [AutoSerialize]
        public uint GameVersion { get; set; } = 171022;

        [AutoSerialize]
        public uint Unknown { get; set; } = 0x93;

        [AutoSerialize]
        public uint ConnectionType { get; set; }

        [AutoSerialize]
        public uint ProcessId { get; set; } = (uint) Process.GetCurrentProcess().Id;

        [AutoSerialize]
        public ushort Port { get; set; } = 0xFFFF;

        [AutoSerialize]
        public string Address { get; set; } = "127.0.0.1";
    }
}