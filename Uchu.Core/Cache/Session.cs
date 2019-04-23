using RakDotNet;

namespace Uchu.Core
{
    public class Session
    {
        public string Key { get; set; }
        public long CharacterId { get; set; } = -1;
        public long UserId { get; set; }
        public int ZoneId { get; set; } = -1;

        public byte[] ToBytes()
        {
            var stream = new BitStream();
            stream.WriteString(Key, 24, true);
            stream.WriteInt64(CharacterId);
            stream.WriteInt64(UserId);
            stream.WriteInt32(ZoneId);
            return stream.BaseBuffer;
        }

        public static Session FromBytes(byte[] bytes)
        {
            var stream = new BitStream(bytes);
            return new Session
            {
                Key = stream.ReadString(24, true),
                CharacterId = stream.ReadInt64(),
                UserId = stream.ReadInt64(),
                ZoneId = stream.ReadInt32()
            };
        }
    }
}