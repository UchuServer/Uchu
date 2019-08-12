using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class RemoveFriendPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Chat;

        public override uint PacketId => 0x9;
        
        public string FriendName { get; set; }

        public override void Deserialize(BitReader reader)
        {
            reader.Read<ulong>();
            
            FriendName = reader.ReadString(wide: true);
        }
    }
}