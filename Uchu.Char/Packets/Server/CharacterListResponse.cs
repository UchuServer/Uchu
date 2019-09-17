using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterListResponse : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x6;
        
        public byte CharacterCount { get; set; }

        public byte CharacterIndex { get; set; }

        public Character[] Characters { get; set; }

        public class Character : ISerializable
        {
            public long CharacterId { get; set; }

            public string Name { get; set; }

            public string UnnaprovedName { get; set; }

            public bool NameRejected { get; set; }

            public bool FreeToPlay { get; set; }

            public uint ShirtColor { get; set; } // TODO: use enum

            public uint ShirtStyle { get; set; } // TODO: use enum

            public uint PantsColor { get; set; } // TODO: use enum

            public uint HairStyle { get; set; } // TODO: use enum

            public uint HairColor { get; set; } // TODO: use enum

            public uint Lh { get; set; }

            public uint Rh { get; set; }

            public uint EyebrowStyle { get; set; } // TODO: use enum

            public uint EyeStyle { get; set; } // TODO: use enum

            public uint MouthStyle { get; set; } // TODO: use enum

            public ZoneId LastZone { get; set; }

            public ushort LastInstance { get; set; }

            public uint LastClone { get; set; }

            public ulong LastActivity { get; set; }

            public ushort ItemCount { get; set; } = 0;

            public uint[] Items { get; set; } = new uint[0];
            
            public void Serialize(BitWriter writer)
            {
                writer.Write(CharacterId);

                writer.Write<uint>(0);

                writer.WriteString(Name, wide: true);

                writer.WriteString(UnnaprovedName, wide: true);
                
                writer.Write((byte) (NameRejected ? 1 : 0));

                writer.Write((byte) (FreeToPlay ? 1 : 0));

                writer.Write(new byte[10], 10 * 8);

                writer.Write(ShirtColor);

                writer.Write(ShirtStyle);

                writer.Write(PantsColor);
                
                writer.Write(HairStyle);
                
                writer.Write(HairColor);

                writer.Write(Lh);

                writer.Write(Rh);

                writer.Write(EyebrowStyle);

                writer.Write(EyeStyle);

                writer.Write(MouthStyle);

                writer.Write<uint>(0);

                writer.Write((ushort) LastZone);

                writer.Write(LastInstance);

                writer.Write(LastClone);

                writer.Write(LastActivity);

                writer.Write(ItemCount);

                foreach (var item in Items)
                {
                    writer.Write(item);
                }
            }

            public void Deserialize(BitReader reader)
            {
                throw new System.NotImplementedException();
            }
        }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write(CharacterCount);

            writer.Write(CharacterIndex);

            for (var i = 0; i < CharacterCount; i++)
            {
                writer.Write(Characters[i]);
            }
        }
    }
}