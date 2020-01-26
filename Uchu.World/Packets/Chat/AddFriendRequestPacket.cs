using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class AddFriendRequestPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0x7;

        public string PlayerName { get; set; }

        public bool IsRequestingBestFriend { get; set; }

        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();

            PlayerName = reader.ReadString(wide: true);
            IsRequestingBestFriend = reader.Read<byte>() > 0;
        }
    }
}