using System;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class Packet : IPacket
    {
        public abstract RemoteConnectionType RemoteConnectionType { get; }
        public abstract uint PacketId { get; }

        public virtual void Serialize(BitStream stream)
        {
            stream.WriteByte((byte) MessageIdentifiers.UserPacketEnum);
            stream.WriteUShort((ushort) RemoteConnectionType);
            stream.WriteUInt(PacketId);
            stream.WriteByte(0);
        }

        public virtual void Deserialize(BitStream stream)
        {
            throw new NotSupportedException();
        }
    }
}