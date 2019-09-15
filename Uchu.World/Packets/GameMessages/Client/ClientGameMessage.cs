using Uchu.Core;

namespace Uchu.World
{
    public abstract class ClientGameMessage : Packet, IGameMessage
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x5;

        public abstract GameMessageId GameMessageId { get; }

        public GameObject Associate { get; set; }
    }
}