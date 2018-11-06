using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterListResponsePacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x06;

        [AutoSerialize]
        public byte CharacterCount { get; set; }

        [AutoSerialize]
        public byte CharacterIndex { get; set; }

        [AutoSerialize]
        public Character[] Characters { get; set; }

        public class Character : AutoSerializable
        {
            [AutoSerialize]
            public long CharacterId { get; set; }

            [AutoSerialize]
            public uint Unknown1 { get; set; } = 0;

            [AutoSerialize(Wide = true)]
            public string Name { get; set; }

            [AutoSerialize(Wide = true)]
            public string UnnaprovedName { get; set; }

            [AutoSerialize(Bool = true)]
            public bool NameRejected { get; set; }

            [AutoSerialize(Bool = true)]
            public bool FreeToPlay { get; set; }

            [AutoSerialize(Length = 10)]
            public byte[] Unknown2 { get; set; } = new byte[10];

            [AutoSerialize]
            public uint ShirtColor { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint ShirtStyle { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint PantsColor { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint HairStyle { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint HairColor { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint Lh { get; set; }

            [AutoSerialize]
            public uint Rh { get; set; }

            [AutoSerialize]
            public uint EyebrowStyle { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint EyeStyle { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint MouthStyle { get; set; } // TODO: use enum

            [AutoSerialize]
            public uint Unknown3 { get; set; } = 0;

            [AutoSerialize]
            public ZoneId LastZone { get; set; }

            [AutoSerialize]
            public ushort LastInstance { get; set; }

            [AutoSerialize]
            public uint LastClone { get; set; }

            [AutoSerialize]
            public ulong LastActivity { get; set; }

            [AutoSerialize]
            public ushort ItemCount { get; set; } = 0;

            [AutoSerialize]
            public uint[] Items { get; set; } = new uint[0];
        }
    }
}