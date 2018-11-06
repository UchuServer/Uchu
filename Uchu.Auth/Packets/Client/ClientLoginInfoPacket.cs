using RakDotNet;
using Uchu.Core;

namespace Uchu.Auth
{
    public class ClientLoginInfoPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Auth;
        public override uint PacketId => 0x00;

        [AutoSerialize(Wide = true)]
        public string Username { get; set; }

        [AutoSerialize(Length = 82)]
        public string Password { get; set; }

        [AutoSerialize]
        public ushort ComLang { get; set; }

        [AutoSerialize]
        public PlatformType Platform { get; set; }

        [AutoSerialize(Length = 256, Wide = true)]
        public string MemoryInfo { get; set; }

        [AutoSerialize(Length = 128, Wide = true)]
        public string GraphicsDriverInfo { get; set; }

        [AutoSerialize]
        public ProcessorInfo Processor { get; set; }

        [AutoSerialize]
        public OperatingSystemInfo OperatingSystem { get; set; }

        public class ProcessorInfo : AutoSerializable
        {
            [AutoSerialize]
            public uint Cores { get; set; }

            [AutoSerialize]
            public ProcessorType Type { get; set; }

            [AutoSerialize]
            public ushort Level { get; set; }

            [AutoSerialize]
            public ushort Revision { get; set; }
        }

        public class OperatingSystemInfo : ISerializable
        {
            public PlatformVersion Version { get; set; }
            public uint BuildNumber { get; set; }
            public uint PlatformId { get; set; }

            public void Serialize(BitStream stream)
            {
                switch (Version)
                {
                    case PlatformVersion.Windows2000:
                        stream.WriteUInt(5);
                        stream.WriteUInt(0);
                        break;

                    case PlatformVersion.WindowsXP:
                        stream.WriteUInt(5);
                        stream.WriteUInt(1);
                        break;

                    case PlatformVersion.WindowsServer2003:
                        stream.WriteUInt(5);
                        stream.WriteUInt(2);
                        break;

                    case PlatformVersion.WindowsVista:
                        stream.WriteUInt(6);
                        stream.WriteUInt(0);
                        break;

                    case PlatformVersion.Windows7:
                        stream.WriteUInt(6);
                        stream.WriteUInt(1);
                        break;

                    case PlatformVersion.Windows8:
                        stream.WriteUInt(6);
                        stream.WriteUInt(2);
                        break;

                    case PlatformVersion.Windows8_1:
                        stream.WriteUInt(6);
                        stream.WriteUInt(3);
                        break;

                    case PlatformVersion.Windows10:
                        stream.WriteUInt(10);
                        stream.WriteUInt(0);
                        break;

                    default:
                        stream.WriteUInt(0);
                        stream.WriteUInt(0);
                        break;
                }

                stream.WriteUInt(BuildNumber);
                stream.WriteUInt(PlatformId);
            }

            public void Deserialize(BitStream stream)
            {
                var major = stream.ReadUInt();
                var minor = stream.ReadUInt();

                switch (major)
                {
                    case 5:
                        Version = minor == 0 ? PlatformVersion.Windows2000 :
                            minor == 1 ? PlatformVersion.WindowsXP :
                            minor == 2 ? PlatformVersion.WindowsServer2003 : PlatformVersion.Unknown;

                        break;
                    case 6:
                        Version = minor == 0 ? PlatformVersion.WindowsVista :
                            minor == 1 ? PlatformVersion.Windows7 :
                            minor == 2 ? PlatformVersion.Windows8 :
                            minor == 3 ? PlatformVersion.Windows8_1 : PlatformVersion.Unknown;
                        break;
                    case 10:
                        Version = minor == 0 ? PlatformVersion.Windows10 : PlatformVersion.Unknown;
                        break;

                    default:
                        Version = PlatformVersion.Unknown;
                        break;
                }

                BuildNumber = stream.ReadUInt();
                PlatformId = stream.ReadUInt();
            }
        }
    }
}