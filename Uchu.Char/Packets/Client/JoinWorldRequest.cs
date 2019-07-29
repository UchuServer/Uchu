using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class JoinWorldRequest : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x4;

        public long CharacterId { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            CharacterId = reader.Read<long>();
        }
    }
}