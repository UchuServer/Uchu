using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class CheckWhitelistRequestPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x19;

        public byte ChatMode { get; set; }

        public byte ChatChannel { get; set; }

        public string PrivateReceiver { get; set; }

        public ushort ChatMessageLength { get; set; }

        public string ChatMessage { get; set; }

        public override void Deserialize(BitReader reader)
        {
            ChatMode = reader.Read<byte>();

            ChatChannel = reader.Read<byte>();

            PrivateReceiver = reader.ReadString(42, true);

            ChatMessageLength = reader.Read<ushort>();

            ChatMessage = reader.ReadString(ChatMessageLength, true);
        }
    }
}