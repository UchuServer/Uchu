using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class TeamInvitePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0xF;

        public string InvitedPlayer { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();

            InvitedPlayer = reader.ReadString(wide: true);
        }
    }
}