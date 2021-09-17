using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterListRequest : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x2;

        public override void Deserialize(BitReader reader)
        {

        }
    }
}
