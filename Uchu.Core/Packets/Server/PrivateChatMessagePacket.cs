using RakDotNet;

namespace Uchu.Core
{
    public class PrivateChatMessagePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;
        public override uint PacketId => 0x02;

        public ulong UnknownObjectId { get; set; } = 0;
        public ChatChannel Channel { get; set; } = ChatChannel.Public;
        public string SenderName { get; set; } = "";
        public long SenderObjectId { get; set; } = 0;
        public bool IsMythran { get; set; } = false;
        public string RecipientName { get; set; }
        public bool IsRecipientMythran { get; set; } = false;
        public ChatReturnCode ReturnCode { get; set; } = ChatReturnCode.Success;
        public string Message { get; set; }

        public override void Serialize(BitStream stream)
        {
            base.Serialize(stream);

            stream.WriteULong(UnknownObjectId);
            stream.WriteByte((byte) Channel);
            stream.WriteByte((byte) (Message.Length * 2));
            stream.Write(new byte[3]);
            stream.WriteString(SenderName, wide: true);
            stream.WriteLong(SenderObjectId);
            stream.WriteUShort(0);
            stream.WriteByte((byte) (IsMythran ? 1 : 0));
            stream.WriteString(RecipientName, wide: true);
            stream.WriteByte((byte) (IsRecipientMythran ? 1 : 0));
            stream.WriteByte((byte) ReturnCode);
            stream.WriteString(Message, Message.Length, true);
        }
    }
}