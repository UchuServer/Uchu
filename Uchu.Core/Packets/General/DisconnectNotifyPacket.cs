using System;
using RakDotNet.IO;
using Uchu.Core.Resources;

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
                throw new ArgumentNullException(nameof(writer), 
                    ResourceStrings.DisconnectNotifyPacket_Serialize_PacketWriterNullException);
            
            writer.Write((uint) DisconnectId);
        }

        public override void Deserialize(BitReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), 
                    ResourceStrings.DisconnectNotifyPacket_Deserialize_ReaderNullException);
            
            DisconnectId = (DisconnectId) reader.Read<uint>();
        }
    }
}