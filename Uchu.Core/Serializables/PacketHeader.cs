using RakDotNet;

namespace Uchu.Core
{
    public class PacketHeader : ISerializable
    {
        public MessageIdentifiers MessageId { get; set; }
        public RemoteConnectionType RemoteConnectionType { get; set; }
        public uint PacketId { get; set; }
        public byte Pad { get; set; }

        public void Serialize(BitStream stream)
        {
            stream.WriteByte((byte) MessageId);
            stream.WriteUShort((ushort) RemoteConnectionType);
            stream.WriteUInt(PacketId);
            stream.WriteByte(Pad);
        }

        public void Deserialize(BitStream stream)
        {
            MessageId = (MessageIdentifiers) stream.ReadByte();
            RemoteConnectionType = (RemoteConnectionType) stream.ReadUShort();
            PacketId = stream.ReadUInt();
            Pad = stream.ReadByte();
        }
    }
}