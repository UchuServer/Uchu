using RakDotNet;
using Uchu.Core;

namespace Uchu.Auth
{
    public class ServerLoginInfoPacket : AutoSerializingPacket
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;
        public override uint PacketId => 0x00;

        [AutoSerialize]
        public LoginCode LoginCode { get; set; }

        [AutoSerialize]
        public string TalkLikeAPirate { get; set; } = "Talk_Like_A_Pirate";

        [AutoSerialize(Length = 33 * 7)]
        public string Unknown1 { get; set; } = "";

        [AutoSerialize]
        public GameVersion Version { get; set; } = new GameVersion();

        [AutoSerialize(Wide = true)]
        public string UserKey { get; set; }

        [AutoSerialize]
        public string WorldInstanceAddress { get; set; }

        [AutoSerialize]
        public string ChatInstanceAddress { get; set; }

        [AutoSerialize]
        public ushort WorldInstancePort { get; set; }

        [AutoSerialize]
        public ushort ChatInstancePort { get; set; }

        [AutoSerialize]
        public string UnknownIp { get; set; } = "127.0.0.1";

        [AutoSerialize(Length = 37)]
        public string LegoUUID { get; set; } = "00000000-0000-0000-0000-000000000000";

        [AutoSerialize]
        public uint Unknown2 { get; set; } = 0;

        [AutoSerialize(Length = 3)]
        public string Locale { get; set; } = "US";

        [AutoSerialize(Bool = true)]
        public bool FirstLoginWithSubscription { get; set; }

        [AutoSerialize(Bool = true)]
        public bool FreeToPlay { get; set; }

        [AutoSerialize]
        public ulong Unknown3 { get; set; } = 0;

        [AutoSerialize]
        public ErrorMessage Error { get; set; } = new ErrorMessage {Message = null};

        [AutoSerialize]
        public uint StampCount { get; set; } = 0;

        // TODO: add stamps

        public class GameVersion : AutoSerializable
        {
            [AutoSerialize]
            public ushort Major { get; set; } = 1;

            [AutoSerialize]
            public ushort Current { get; set; } = 10;

            [AutoSerialize]
            public ushort Minor { get; set; } = 64;
        }

        public class ErrorMessage : ISerializable
        {
            public string Message { get; set; }

            public void Serialize(BitStream stream)
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    stream.WriteUShort((ushort) Message.Length);
                    stream.WriteString(Message, Message.Length, true);
                }
                else
                {
                    stream.WriteUShort(0);
                }
            }

            public void Deserialize(BitStream stream)
            {
                var length = stream.ReadUShort();

                Message = length > 0 ? stream.ReadString(length, true) : null;
            }
        }
    }
}