using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Uchu.World.Compression
{
    public static class Zlib
    {
        public static async Task<byte[]> CompressBytesAsync(byte[] data,
            CompressionLevel compressionLevel = CompressionLevel.Fastest)
            => await CompressBytesAsync(data, 0, data.Length, compressionLevel);

        public static async Task<byte[]> CompressBytesAsync(byte[] data, int offset, int length,
            CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, compressionLevel))
                {
                    await ds.WriteAsync(data, offset, length);
                }

                var compressed = ms.ToArray();
                var buffer = new byte[2 /* zlib header */ + compressed.Length + 4 /* Adler32 checksum */];

                buffer[0] = 0x78;
                buffer[1] = (byte) (
                    compressionLevel == CompressionLevel.Optimal ? 0xDA :
                    compressionLevel == CompressionLevel.Fastest ? 0x9C : 0x01
                );

                var adler = new Adler32();

                adler.Update(data, offset, length);

                Buffer.BlockCopy(compressed, 0, buffer, 2, compressed.Length);
                Buffer.BlockCopy(adler.Checksum, 0, buffer, compressed.Length + 2, 4);

                return buffer;
            }
        }

        public static byte[] CompressBytes(byte[] data, CompressionLevel compressionLevel = CompressionLevel.Fastest)
            => CompressBytes(data, 0, data.Length, compressionLevel);

        public static byte[] CompressBytes(byte[] data, int offset, int length,
            CompressionLevel compressionLevel = CompressionLevel.Fastest)
        {
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, compressionLevel))
                {
                    ds.Write(data, offset, length);
                }

                var compressed = ms.ToArray();
                var buffer = new byte[2 /* zlib header */ + compressed.Length + 4 /* Adler32 checksum */];

                buffer[0] = 0x78;
                buffer[1] = (byte) (
                    compressionLevel == CompressionLevel.Optimal ? 0xDA :
                    compressionLevel == CompressionLevel.Fastest ? 0x9C : 0x01
                );

                var adler = new Adler32();

                adler.Update(data, offset, length);

                Buffer.BlockCopy(compressed, 0, buffer, 2, compressed.Length);
                Buffer.BlockCopy(adler.Checksum, 0, buffer, compressed.Length + 2, 4);

                return buffer;
            }
        }
    }
}