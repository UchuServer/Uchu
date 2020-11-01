using System;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core.Resources;

namespace Uchu.Core
{
    public class PacketHeader : ISerializable, IDeserializable
    {
        public MessageIdentifier MessageId { get; set; }
        public RemoteConnectionType RemoteConnectionType { get; set; }
        public uint PacketId { get; set; }
        public byte Pad { get; set; }
        
        public void Serialize(BitWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), 
                    ResourceStrings.PacketHeader_Serialize_WriterNullException);
            
            writer.Write((byte) MessageId);
            writer.Write((ushort) RemoteConnectionType);
            writer.Write(PacketId);
            writer.Write(Pad);
        }

        public void Deserialize(BitReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader), 
                    ResourceStrings.PacketHeader_Deserialize_NullReaderException);
            
            MessageId = (MessageIdentifier) reader.Read<byte>();
            RemoteConnectionType = (RemoteConnectionType) reader.Read<ushort>();
            PacketId = reader.Read<uint>();
            Pad = reader.Read<byte>();
        }
    }
}