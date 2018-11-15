using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientChatMessagePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x0E;

        public string Message { get; set; }

        public override void Deserialize(BitStream stream)
        {
            stream.Read(3);

            var length = stream.ReadByte();

            stream.Read(3);

            Message = stream.ReadString(length, true);
        }
    }
}