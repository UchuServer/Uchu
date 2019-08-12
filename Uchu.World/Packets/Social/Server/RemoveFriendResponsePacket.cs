using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class RemoveFriendResponsePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x1D;

        public bool Success { get; set; }
        
        public string FriendName { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) (Success ? 1 : 0));

            writer.WriteString(FriendName, wide: true);
        }
    }
}