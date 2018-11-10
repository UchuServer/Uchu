using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterRenameRequestPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x07;
        
        [AutoSerialize]
        public long CharacterId { get; set; }
        
        [AutoSerialize(Wide = true)]
        public string Name { get; set; }
    }
}