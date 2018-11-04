using RakDotNet;

namespace Uchu.Core
{
    public abstract class AutoSerializingPacket : AutoSerializable, IPacket
    {
        public abstract RemoteConnectionType RemoteConnectionType { get; }
        public abstract uint PacketId { get; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteByte((byte) MessageIdentifiers.UserPacketEnum);
            stream.WriteUShort((ushort) RemoteConnectionType);
            stream.WriteUInt(PacketId);
            stream.WriteByte(0);

            base.Serialize(stream);
        }
    }
}