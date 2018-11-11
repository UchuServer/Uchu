using System;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class ClientGameMessage : Packet, IGameMessage
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x05;

        public abstract ushort GameMessageId { get; }

        public long ObjectId { get; set; }
    }
}