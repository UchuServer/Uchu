using System;
using RakDotNet.IO;

namespace Uchu.Core
{
    public static class BitWriterExtensions
    {
        public static void Write(this BitWriter @this, ISerializable serializable) => serializable.Serialize(@this);
        
        public static int Write(this BitWriter @this, Span<byte> buf) =>
            @this.Write(buf, buf.Length * 8);
        
        public static void WriteString(this BitWriter @this, string val, int length = 33, bool wide = false)
        {
            foreach (var c in val)
            {
                if (wide) @this.Write((short) c);
                else @this.Write((byte) c);
            }

            Write(@this, new byte[(length - val.Length) * (wide ? sizeof(char) : sizeof(byte))]);
        }
    }
}