using RakDotNet;

namespace Uchu.Core
{
    public class ChatMessagePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;
        public override uint PacketId => 0x01;

        public ulong UnknownObjectId { get; set; } = 0;
        public ChatChannel Channel { get; set; } = ChatChannel.Public;
        public string SenderName { get; set; } = "";
        public ulong SenderObjectId { get; set; } = 0;
        public bool IsMythran { get; set; } = false;
        public string Message { get; set; }

        public override void Serialize(BitStream stream)
        {
            base.Serialize(stream);

            stream.WriteULong(UnknownObjectId);
            stream.WriteByte((byte) Channel);
            stream.WriteByte((byte) (Message.Length * 2));
            stream.Write(new byte[3]);
            stream.WriteString(SenderName, wide: true);
            stream.WriteULong(SenderObjectId);
            stream.WriteUShort(0);
            stream.WriteByte((byte) (IsMythran ? 1 : 0));
            stream.WriteString(Message, Message.Length, true);
        }
    }
}