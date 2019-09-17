using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Auth
{
    public class ClientLoginInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Auth;
        
        public override uint PacketId => 0x0;
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public ushort ComLang { get; set; }
        
        public PlatformType Platform { get; set; }
        
        public string MemoryInfo { get; set; }
        
        public string GraphicsDriverInfo { get; set; }
        
        public ProcessorInfo Processor { get; set; }

        public OperatingSystemInfo OperatingSystem { get; set; }

        public struct ProcessorInfo
        {
            public uint Cores { get; set; }

            public ProcessorType Type { get; set; }

            public ushort Level { get; set; }

            public ushort Revision { get; set; }
        }

        public class OperatingSystemInfo : ISerializable
        {
            public PlatformVersion Version { get; set; }
            public uint BuildNumber { get; set; }
            public uint PlatformId { get; set; }

            public void Serialize(BitWriter writer)
            {
                switch (Version)
                {
                    case PlatformVersion.Windows2000:
                        writer.Write<uint>(5);
                        writer.Write<uint>(0);
                        break;

                    case PlatformVersion.WindowsXP:
                        writer.Write<uint>(5);
                        writer.Write<uint>(1);
                        break;

                    case PlatformVersion.WindowsServer2003:
                        writer.Write<uint>(5);
                        writer.Write<uint>(2);
                        break;

                    case PlatformVersion.WindowsVista:
                        writer.Write<uint>(6);
                        writer.Write<uint>(0);
                        break;

                    case PlatformVersion.Windows7:
                        writer.Write<uint>(6);
                        writer.Write<uint>(1);
                        break;

                    case PlatformVersion.Windows8:
                        writer.Write<uint>(6);
                        writer.Write<uint>(2);
                        break;

                    case PlatformVersion.Windows8_1:
                        writer.Write<uint>(6);
                        writer.Write<uint>(3);
                        break;

                    case PlatformVersion.Windows10:
                        writer.Write<uint>(10);
                        writer.Write<uint>(0);
                        break;

                    default:
                        writer.Write<uint>(0);
                        writer.Write<uint>(0);
                        break;
                }

                writer.Write(BuildNumber);
                writer.Write(PlatformId);
            }

            public void Deserialize(BitReader reader)
            {
                var major = reader.Read<uint>();
                var minor = reader.Read<uint>();

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

                BuildNumber = reader.Read<uint>();;
                PlatformId = reader.Read<uint>();;
            }
        }

        public override void Deserialize(BitReader reader)
        {
            Username = reader.ReadString(wide: true);

            Password = reader.ReadString(42, true);

            ComLang = reader.Read<ushort>();

            Platform = (PlatformType) reader.Read<byte>();

            MemoryInfo = reader.ReadString(256, true);

            GraphicsDriverInfo = reader.ReadString(128, true);

            Processor = reader.Read<ProcessorInfo>();
            
            OperatingSystem = new OperatingSystemInfo();
            OperatingSystem.Deserialize(reader);
        }
    }
}