using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class FriendUpdatePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x1F;

        public FriendUpdate Update { get; set; }
        
        public string FriendName { get; set; }
        
        public ZoneId ZoneId { get; set; }
        
        public ushort WorldInstance { get; set; }
        
        public uint WorldClone { get; set; }
        
        public bool IsBestFriend { get; set; }
        
        public bool IsFreeToPlay { get; set; }
        
        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) Update);

            writer.WriteString(FriendName, wide: true);
            
            writer.Write((ushort) ZoneId);
            writer.Write(WorldInstance);
            writer.Write(WorldClone);

            writer.Write((byte) (IsBestFriend ? 1 : 0));
            writer.Write((byte) (IsFreeToPlay ? 1 : 0));
        }
    }
}