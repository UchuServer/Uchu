using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PrivateChatMessagePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0x2;
        
        public ulong UnknownObjectId { get; set; }

        public ChatChannel Channel { get; set; } = ChatChannel.Public;

        public Player Sender { get; set; }

        public bool IsMythran { get; set; }

        public string Message { get; set; }
        
        public override void SerializePacket(BitWriter writer)
        {
            writer.Write(UnknownObjectId);

            writer.Write((byte) Channel);

            writer.Write((uint) (Message.Length * 2));
            
            if (Sender != null)
            {
                writer.WriteString(Sender.Name, wide: true);

                writer.Write((ulong) Sender.Id);
            }
            else
            {
                writer.WriteString("", wide: true);

                writer.Write<ulong>(default);
            }
            
            writer.Write<ushort>(0);
            
            writer.Write((byte) (IsMythran ? 1 : 0));

            writer.WriteString(Sender != null ? Sender.Name : "", wide: true);

            writer.Write((byte) (IsMythran ? 1 : 0));

            writer.Write<byte>(0);
            
            writer.WriteString(Message, Message.Length, true);
        }
    }
}