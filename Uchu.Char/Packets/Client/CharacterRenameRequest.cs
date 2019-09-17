using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterRenameRequest : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x7;

        public long CharacterId { get; set; }
        
        public string Name { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            CharacterId = reader.Read<long>();

            Name = reader.ReadString(wide: true);
        }
    }
}