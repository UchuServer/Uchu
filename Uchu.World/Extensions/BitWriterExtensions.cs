using System.IO;
using RakDotNet.IO;
using Uchu.World.Collections;
using Uchu.World.Compression;

namespace Uchu.World
{
    public static class BitWriterExtensions
    {
        public static int Write(this BitWriter @this, GameObject gameObject)
        {
            return @this.Write(gameObject?.ObjectId ?? -1);
        }

        public static void WriteLdfCompressed(this BitWriter @this, LegoDataDictionary dict)
        {
            var stream = new MemoryStream();

            using (var temp = new BitWriter(stream))
            {
                Core.BitWriterExtensions.Write(temp, dict);

                var buffer = stream.GetBuffer();

                var compressed = Zlib.CompressBytes(buffer);

                @this.Write((uint) (compressed.Length + 9));

                @this.Write<byte>(1);
                @this.Write((uint) buffer.Length);
                @this.Write((uint) compressed.Length);
                Core.BitWriterExtensions.Write(@this, compressed);
            }
        }

        public static bool Flag(this BitWriter @this, bool condition)
        {
            @this.WriteBit(condition);
            return condition;
        }
    }
}