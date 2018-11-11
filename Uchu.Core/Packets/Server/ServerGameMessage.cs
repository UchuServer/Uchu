using System;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class ServerGameMessage : Packet, IGameMessage
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x0C;

        public abstract ushort GameMessageId { get; }

        public long ObjectId { get; set; }

        public override void Serialize(BitStream stream)
        {
            base.Serialize(stream);

            stream.WriteLong(ObjectId);
            stream.WriteUShort(GameMessageId);

            SerializeMessage(stream);
        }

        public virtual void SerializeMessage(BitStream stream)
        {
        }
    }
}