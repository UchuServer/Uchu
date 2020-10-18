using System;
using System.Diagnostics;
using RakDotNet.IO;

namespace Uchu.Core
{
    /// <summary>
    ///     Client and Server packet to verify Game version.
    /// </summary>
    public class HandshakePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.General;

        public override uint PacketId => 0x0;

        public uint GameVersion { get; set; } = 171022;

        public uint ConnectionType { get; set; }

        public uint ProcessId { get; set; } = (uint) Process.GetCurrentProcess().Id;

        public ushort Port { get; set; } = 0xFFFF;

        public string Address { get; set; } = "127.0.0.1";

        public override void SerializePacket(BitWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), "Received null writer in serialize packet");
            
            writer.Write(GameVersion);
            writer.Write((uint) 0x93);
            writer.Write(ConnectionType);
            writer.Write(ProcessId);
            writer.Write(Port);
            writer.WriteString(Address);
        }

        public override void Deserialize(BitReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "Received null reader in deserialize");
            
            GameVersion = reader.Read<uint>();
            reader.Read<uint>();
            ConnectionType = reader.Read<uint>();
            ProcessId = reader.Read<uint>();
            Port = reader.Read<ushort>();
            Address = reader.ReadString();
        }
    }
}