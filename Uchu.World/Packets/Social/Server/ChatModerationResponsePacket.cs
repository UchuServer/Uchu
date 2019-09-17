using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class ChatModerationResponsePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x3b;

        public bool RequestAccepted { get; set; }

        public byte RequestId { get; set; }

        public byte ChatMode { get; set; }

        public string PlayerName { get; set; }

        public (byte start, byte length)[] UnacceptedRanges { get; set; } = new (byte start, byte length)[0];

        public override void Serialize(BitWriter writer)
        {
            writer.Write((byte) (RequestAccepted ? 1 : 0));

            writer.Write<ushort>(0);

            writer.Write(RequestId);
            writer.Write(ChatMode);

            writer.WriteString(PlayerName, 33, true);

            foreach (var (start, length) in UnacceptedRanges)
            {
                writer.Write(start);
                writer.Write(length);
            }

            if (UnacceptedRanges.Length == 32) return;
            writer.Write<byte>(0);
            writer.Write<byte>(0);
        }
    }
}