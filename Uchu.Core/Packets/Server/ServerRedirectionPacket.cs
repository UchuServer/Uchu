using RakDotNet.IO;

namespace Uchu.Core
{
    public class ServerRedirectionPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0xE;
        
        public string Address { get; set; }
        
        public ushort Port { get; set; }
        
        public bool Forced { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.WriteString(Address);
            writer.Write(Port);
            writer.Write((byte) (Forced ? 1 : 0));
        }
    }
}