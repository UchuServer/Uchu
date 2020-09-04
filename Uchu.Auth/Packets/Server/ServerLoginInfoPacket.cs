using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.Auth
{
    public class ServerLoginInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x0;
        
        public LoginCode LoginCode { get; set; }

        public string TalkLikeAPirate { get; set; } = "Talk_Like_A_Pirate";

        public GameVersion Version { get; set; } = new GameVersion {Major = 1, Current = 10, Minor = 64};

        public string UserKey { get; set; } = "";

        public string CharacterInstanceAddress { get; set; }

        public string ChatInstanceAddress { get; set; }

        public ushort CharacterInstancePort { get; set; }

        public ushort ChatInstancePort { get; set; }

        public string UnknownIp { get; set; } = "127.0.0.1";

        public string LegoUUID { get; set; } = "00000000-0000-0000-0000-000000000000";

        public string Locale { get; set; } = "US";

        public bool FirstLoginWithSubscription { get; set; }

        public bool FreeToPlay { get; set; }

        public ErrorMessage Error { get; set; } = new ErrorMessage {Message = null};

        public uint StampCount { get; set; } = 0x0144;

        // TODO: add stamps

        public struct GameVersion
        {
            public ushort Major { get; set; }

            public ushort Current { get; set; }

            public ushort Minor { get; set; }
        }

        public class ErrorMessage : ISerializable
        {
            public string Message { get; set; }

            public void Serialize(BitWriter writer)
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    writer.Write((ushort) Message.Length);
                    writer.WriteString(Message, Message.Length, true);
                }
                else
                {
                    writer.Write<ushort>(0);
                }
            }

            public void Deserialize(BitReader reader)
            {
                var length = reader.Read<uint>();
 
                Message = length > 0 ? reader.ReadString((int) length, true) : null;
            }
        }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((byte) LoginCode);
            
            writer.WriteString(TalkLikeAPirate);
            
            writer.WriteString("", 33 * 7);
            
            writer.Write(Version);
            
            writer.WriteString(UserKey, wide: true);
            
            writer.WriteString(CharacterInstanceAddress);
            
            writer.WriteString(ChatInstanceAddress);
            writer.Write(CharacterInstancePort);
            
            writer.Write(ChatInstancePort);
            writer.WriteString(UnknownIp);

            writer.WriteString(LegoUUID, 37);

            writer.Write<uint>(0);
            
            writer.WriteString(Locale, 3);

            writer.Write((byte) (FirstLoginWithSubscription ? 1 : 0));

            writer.Write((byte) (FreeToPlay ? 1 : 0));

            writer.Write<ulong>(0);

            writer.Write(Error);

            writer.Write(StampCount);
            
            
            // Stamp data
            writer.Write<ulong>(0x0000000000000000);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x0000001c00000007);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x0000000300000008);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x0000000000000009);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000000000000000a);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000000010000000b);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000000010000000e);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000000000000000f);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x0000000100000011);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x0000000000000005);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x0000000100000006);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x0000000100000014);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x000029ca00000013);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x0000000000000015);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x0000000000000016);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000029c400000017);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x000029c40000001b);	
            writer.Write<ulong>(0x000000004ee27a4d);
            writer.Write<ulong>(0x000000010000001c);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000000000000001d);	
            writer.Write<ulong>(0x000000004ee27a4c);
            writer.Write<ulong>(0x000029ca0000001e);	
            writer.Write<ulong>(0x000000004ee27a4d);
        }
    }
}
