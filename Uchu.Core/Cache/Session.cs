using System.IO;
using RakDotNet.IO;

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
            var stream = new MemoryStream();
            using (var writer = new BitWriter(stream))
            {
                writer.WriteString(Key, wide: true);
                writer.Write(CharacterId);
                writer.Write(UserId);
                writer.Write(ZoneId);
            }

            return stream.ToArray();
        }

        public static Session FromBytes(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            using (var reader = new BitReader(stream))
            {
                return new Session
                {
                    Key = reader.ReadString(wide: true),
                    CharacterId = reader.Read<long>(),
                    UserId = reader.Read<long>(),
                    ZoneId = reader.Read<int>()
                };
            }
        }
    }
}