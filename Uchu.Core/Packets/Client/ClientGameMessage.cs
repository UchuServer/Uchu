using System;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class ClientGameMessage : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x05;

        public abstract ushort GameMessageId { get; protected set; }

        public long ObjectId { get; set; }

        public override void Deserialize(BitStream stream)
        {
            ObjectId = stream.ReadLong();
            GameMessageId = stream.ReadUShort();

            DeserializeMessage(stream);
        }

        public virtual void DeserializeMessage(BitStream stream)
        {
        }
    }
}