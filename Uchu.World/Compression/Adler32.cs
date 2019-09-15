using System;

namespace Uchu.World.Compression
{
    // https://stackoverflow.com/questions/70347/zlib-compatible-compression-streams/2331025#2331025
    public class Adler32
    {
        private int _a;
        private int _b;

        public Adler32()
        {
            _a = 1;
            _b = 0;
        }

        public byte[] Checksum
        {
            get
            {
                var checksum = BitConverter.GetBytes(_b * 65536 + _a);

                Array.Reverse(checksum);

                return checksum;
            }
        }

        public void Update(byte[] data, int offset, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _a = (_a + data[offset + i]) % 65521;
                _b = (_b + _a) % 65521;
            }
        }
    }
}