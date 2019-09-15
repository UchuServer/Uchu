using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class GeneralGameMessage : Packet, IGameMessage
    {
        public override uint PacketId => 0x5;

        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public abstract GameMessageId GameMessageId { get; }

        public GameObject Associate { get; set; }

        public override void Serialize(BitWriter writer)
        {
            writer.Write((byte) MessageIdentifiers.UserPacketEnum);
            writer.Write((ushort) RemoteConnectionType.Server);
            writer.Write<uint>(0xC);
            writer.Write<byte>(0);

            SerializePacket(writer);
        }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write(Associate);
            writer.Write((ushort) GameMessageId);

            SerializeMessage(writer);
        }

        public virtual void SerializeMessage(BitWriter writer)
        {
        }
    }
}