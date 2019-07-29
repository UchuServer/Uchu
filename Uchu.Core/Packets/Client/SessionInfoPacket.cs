using RakDotNet;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Core
{
    public class SessionInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x1;
        
        public string Username { get; set; }
        
        public string SessionKey { get; set; }
        
        public string UnknownHash { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Username = reader.ReadString(wide: true);

            SessionKey = reader.ReadString(wide: true);

            UnknownHash = reader.ReadString();
        }
    }
}