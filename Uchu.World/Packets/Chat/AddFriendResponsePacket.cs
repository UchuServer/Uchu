using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class AddFriendResponsePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0x8;

        public ClientFriendRequestResponse Response { get; set; }

        public string FriendName { get; set; }

        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();

            Response = (ClientFriendRequestResponse) reader.Read<byte>();
            FriendName = reader.ReadString(wide: true);
        }
    }
}