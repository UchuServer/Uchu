using System.IO;
using System.Linq;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientRoutedPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x15;

        public byte[] Packet { get; set; }

        public override void Deserialize(BitReader reader)
        {
            var length = reader.Read<uint>();

            var packet = reader.ReadBytes((int) length).ToArray();

            var stream = new MemoryStream();

            using (var writer = new BitWriter(stream))
            {
                writer.Write<byte>(0x53);
                writer.Write(packet);
            }

            Packet = stream.ToArray();
        }
    }
}