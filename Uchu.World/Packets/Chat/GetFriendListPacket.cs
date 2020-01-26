using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class GetFriendListPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0xA;

        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();
        }
    }
}