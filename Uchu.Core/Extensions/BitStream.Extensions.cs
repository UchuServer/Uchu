using System.IO.Compression;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core.Collections;
using Uchu.Core.IO.Compression;

namespace Uchu.Core
{
    public static class BitStream_Extensions
    {
        public static async Task WriteLDFCompressedAsync(this BitStream @this, LegoDataDictionary dict)
        {
            var temp = new BitStream();

            temp.WriteSerializable(dict);

            var compressed = await Zlib.CompressBytesAsync(temp.BaseBuffer);

            @this.WriteUInt((uint) (compressed.Length + 9));

            @this.WriteByte(1);
            @this.WriteUInt((uint) temp.BaseBuffer.Length);
            @this.WriteUInt((uint) compressed.Length);
            @this.Write(compressed);
        }

        public static void WriteLDFCompressed(this BitStream @this, LegoDataDictionary dict)
        {
            var temp = new BitStream();

            temp.WriteSerializable(dict);

            var compressed = Zlib.CompressBytes(temp.BaseBuffer);

            @this.WriteUInt((uint) (compressed.Length + 9));

            @this.WriteByte(1);
            @this.WriteUInt((uint) temp.BaseBuffer.Length);
            @this.WriteUInt((uint) compressed.Length);
            @this.Write(compressed);
        }
    }
}