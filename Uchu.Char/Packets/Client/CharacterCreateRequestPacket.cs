using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterCreateRequestPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x03;

        [AutoSerialize(Wide = true)]
        public string CustomName { get; set; }

        [AutoSerialize]
        public PredefinedName Predefined { get; set; }

        [AutoSerialize(Length = 9)]
        public byte[] Unknown1 { get; set; } = new byte[9];

        [AutoSerialize]
        public uint ShirtColor { get; set; }

        [AutoSerialize]
        public uint ShirtStyle { get; set; }

        [AutoSerialize]
        public uint PantsColor { get; set; }

        [AutoSerialize]
        public uint HairStyle { get; set; }

        [AutoSerialize]
        public uint HairColor { get; set; }

        [AutoSerialize]
        public uint Lh { get; set; }

        [AutoSerialize]
        public uint Rh { get; set; }

        [AutoSerialize]
        public uint EyebrowStyle { get; set; }

        [AutoSerialize]
        public uint EyeStyle { get; set; }

        [AutoSerialize]
        public uint MouthStyle { get; set; }

        [AutoSerialize]
        public byte Unknown2 { get; set; }

        public class PredefinedName : AutoSerializable
        {
            [AutoSerialize]
            public uint First { get; set; }

            [AutoSerialize]
            public uint Middle { get; set; }

            [AutoSerialize]
            public uint Last { get; set; }
        }
    }
}