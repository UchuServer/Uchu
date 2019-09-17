using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Char
{
    public class CharacterCreateRequest : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x3;
        
        public string CharacterName { get; set; }
        
        public PredefinedName Predefined { get; set; }

        public uint ShirtColor { get; set; }

        public uint ShirtStyle { get; set; }

        public uint PantsColor { get; set; }

        public uint HairStyle { get; set; }

        public uint HairColor { get; set; }

        public uint Lh { get; set; }

        public uint Rh { get; set; }

        public uint EyebrowStyle { get; set; }

        public uint EyeStyle { get; set; }

        public uint MouthStyle { get; set; }
        
        public struct PredefinedName
        {
            public uint First { get; set; }

            public uint Middle { get; set; }

            public uint Last { get; set; }
        }

        public override void Deserialize(BitReader reader)
        {
            CharacterName = reader.ReadString(wide: true);

            Predefined = reader.Read<PredefinedName>();

            reader.Read(new byte[9], 9 * 8);

            ShirtColor = reader.Read<uint>();

            ShirtStyle = reader.Read<uint>();

            PantsColor = reader.Read<uint>();

            HairStyle = reader.Read<uint>();

            HairColor = reader.Read<uint>();

            Lh = reader.Read<uint>();

            Rh = reader.Read<uint>();

            EyebrowStyle = reader.Read<uint>();

            EyeStyle = reader.Read<uint>();

            MouthStyle = reader.Read<uint>();

            reader.Read<byte>();
        }
    }
}