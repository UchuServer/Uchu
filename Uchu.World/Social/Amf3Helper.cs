using RakDotNet.IO;

namespace Uchu.World.Social
{
    // Stolen from https://github.com/LUNIServer/UniverseServer/blob/master/Source/WorldServer.cpp
    // TODO: Replace with sane code
    public static class Amf3Helper
    {
        public static void WriteNumber(BitWriter writer, uint n)
        {
            if (n < 128)
            {
                writer.Write((byte) n);
            }
            else
            {
                writer.WriteBit(true);
                writer.WriteBit(true);
                if (n < 2048)
                {
                    writer.WriteBit(false);
                }
                else
                {
                    writer.WriteBit(true);
                    if (n < 65536)
                    {
                        writer.WriteBit(false);
                    }
                    else
                    {
                        if (n > 2097151) n = 2097151;

                        writer.WriteBit(true);
                        writer.WriteBit(false);
                        writer.WriteBit((n & 1048576) > 0);
                        writer.WriteBit((n & 524288) > 0);
                        writer.WriteBit((n & 262144) > 0);

                        writer.WriteBit(true);
                        writer.WriteBit(false);
                        writer.WriteBit((n & 131072) > 0);
                        writer.WriteBit((n & 65536) > 0);
                    }

                    writer.WriteBit((n & 32768) > 0);
                    writer.WriteBit((n & 16384) > 0);
                    writer.WriteBit((n & 8192) > 0);
                    writer.WriteBit((n & 4096) > 0);

                    writer.WriteBit(true);
                    writer.WriteBit(false);
                    writer.WriteBit((n & 2048) > 0);
                }

                writer.WriteBit((n & 1024) > 0);
                writer.WriteBit((n & 512) > 0);
                writer.WriteBit((n & 256) > 0);
                writer.WriteBit((n & 128) > 0);
                writer.WriteBit((n & 64) > 0);

                writer.WriteBit(true);
                writer.WriteBit(false);
                writer.WriteBit((n & 32) > 0);
                writer.WriteBit((n & 16) > 0);
                writer.WriteBit((n & 8) > 0);
                writer.WriteBit((n & 4) > 0);
                writer.WriteBit((n & 2) > 0);
                writer.WriteBit((n & 1) > 0);
            }
        }

        public static void WriteNumber2(BitWriter writer, uint n)
        {
            if (n < 128)
            {
                writer.Write((byte) n);
            }
            else
            {
                writer.WriteBit(true);

                var flag = n > 2097151;

                if (n < 16383)
                {
                    if (flag)
                    {
                        writer.WriteBit((n & 268435456) > 0);
                        writer.WriteBit((n & 134217728) > 0);
                        writer.WriteBit((n & 67108864) > 0);
                        writer.WriteBit((n & 33554432) > 0);
                        writer.WriteBit((n & 16777216) > 0);
                        writer.WriteBit((n & 8388608) > 0);
                        writer.WriteBit((n & 4194304) > 0);
                        writer.WriteBit(true);
                        writer.WriteBit((n & 2097152) > 0);
                    }

                    writer.WriteBit((n & 1048576) > 0);
                    writer.WriteBit((n & 524288) > 0);
                    writer.WriteBit((n & 262144) > 0);
                    writer.WriteBit((n & 131072) > 0);
                    writer.WriteBit((n & 65536) > 0);
                    writer.WriteBit((n & 32768) > 0);
                    
                    if (flag) writer.WriteBit(true);

                    writer.WriteBit((n & 16384) > 0);
                    
                    if (!flag) writer.WriteBit(true);
                }

                writer.WriteBit((n & 8192) > 0);
                writer.WriteBit((n & 4096) > 0);
                writer.WriteBit((n & 2048) > 0);
                writer.WriteBit((n & 1024) > 0);
                writer.WriteBit((n & 512) > 0);
                writer.WriteBit((n & 256) > 0);
                writer.WriteBit((n & 128) > 0);

                if (!flag) writer.WriteBit(false);

                writer.WriteBit((n & 64) > 0);
                writer.WriteBit((n & 32) > 0);
                writer.WriteBit((n & 16) > 0);
                writer.WriteBit((n & 8) > 0);
                writer.WriteBit((n & 4) > 0);
                writer.WriteBit((n & 2) > 0);
                writer.WriteBit((n & 1) > 0);
            }
        }

        public static void WriteText(BitWriter writer, string value)
        {
            var size = (value.Length << 1) + 1;

            WriteNumber2(writer, (uint) size);

            foreach (var character in value)
            {
                writer.Write((byte) character);
            }
        }

        public static void Array(BitWriter writer, int length)
        {
            writer.Write((byte) Amf3Type.Array);
            
            var size = (length << 1) | 1;

            WriteNumber2(writer, (uint) size);
        }
    }
}