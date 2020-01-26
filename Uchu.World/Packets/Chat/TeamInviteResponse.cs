using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class TeamInviteResponse : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0x10;

        public bool IsDeclined { get; set; }

        public long InviterObjectId { get; set; }

        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();

            IsDeclined = reader.Read<byte>() > 0;

            InviterObjectId = reader.Read<long>();
        }
    }
}