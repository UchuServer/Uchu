using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyFriendRequestPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x1B;

        public string FriendName { get; set; }

        public bool IsBestFriendRequest { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.WriteString(FriendName, wide: true);

            writer.Write((byte) (IsBestFriendRequest ? 1 : 0));
        }
    }
}