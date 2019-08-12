using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class ServerGameMessage : Packet, IGameMessage
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0xC;
        
        public abstract ushort GameMessageId { get; }
        
        public GameObject Associate { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write(Associate);
            writer.Write(GameMessageId);

            SerializeMessage(writer);
        }

        public abstract void SerializeMessage(BitWriter writer);
    }
}