using System;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class DisconnectNotifyPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.General;

        public override uint PacketId => 0x1;
        
        public DisconnectId DisconnectId { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), "Received null writer in serialize packet");
            writer.Write((uint) DisconnectId);
        }

        public override void Deserialize(BitReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "Received null reader in deserialize");
            DisconnectId = (DisconnectId) reader.Read<uint>();
        }
    }
}