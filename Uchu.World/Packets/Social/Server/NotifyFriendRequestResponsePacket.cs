using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class NotifyFriendRequestResponsePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x1C;

        public ServerFriendRequestResponse Response { get; set; }
        
        public bool IsPlayerOnline { get; set; }
        
        public string PlayerName { get; set; }

        public long PlayerId { get; set; } = -1;
        
        public ZoneId ZoneId { get; set; }
        
        public ushort WorldInstance { get; set; }
        
        public uint WorldClone { get; set; }
        
        public bool IsBestFriend { get; set; }
        
        public bool IsFreeToPlay { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) Response);

            writer.Write((byte) (IsPlayerOnline ? 1 : 0));

            writer.WriteString(PlayerName, wide: true);

            writer.Write(PlayerId);

            writer.Write((ushort) ZoneId);

            writer.Write(WorldInstance, 6 * 8);
            writer.Write(WorldClone, 6 * 8);

            writer.Write((byte) (IsBestFriend ? 1 : 0));
            writer.Write((byte) (IsFreeToPlay ? 1 : 0));
        }
    }
}