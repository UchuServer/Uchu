using System;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.Core
{
    /// <summary>
    ///     A UserPacket.
    /// </summary>
    public abstract class Packet : IPacket
    {
        public abstract RemoteConnectionType RemoteConnectionType { get; }

        public abstract uint PacketId { get; }

        public virtual void Serialize(BitWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer), "Received null writer in serialize");
            
            writer.Write((byte) MessageIdentifier.UserPacketEnum);
            writer.Write((ushort) RemoteConnectionType);
            writer.Write(PacketId);
            writer.Write<byte>(0);

            SerializePacket(writer);
        }

        public virtual void SerializePacket(BitWriter writer)
        {
            Logger.Warning(
                $"{GetType().FullName} has no override for {nameof(SerializePacket)} but is still being serialized.");
        }

        public virtual void Deserialize(BitReader reader)
        {
            Logger.Warning(
                $"{GetType().FullName} has no override for {nameof(Deserialize)} but is still being deserialized.");
        }
    }
}