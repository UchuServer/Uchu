using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ChatModerationResponsePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x3b;

        public bool RequestAccepted { get; set; }

        public byte ChatChannel { get; set; }

        public byte ChatMode { get; set; }

        public string PlayerName { get; set; }

        public (byte start, byte length)[] UnacceptedRanges { get; set; } = new (byte start, byte length)[0];

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) (RequestAccepted ? 1 : 0));

            writer.Write<ushort>(0);

            writer.Write(ChatChannel);
            writer.Write(ChatMode);

            writer.WriteString(PlayerName, 33, true);

            foreach (var (start, length) in UnacceptedRanges)
            {
                writer.Write(start);
                writer.Write(length);
            }

            for (var i = 0; i < 32 - UnacceptedRanges.Length; i++)
            {
                writer.Write<byte>(0);
                writer.Write<byte>(0);
            }
        }
    }
}