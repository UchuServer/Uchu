using System;
using System.Collections.Generic;
using System.Text;
using RakDotNet.IO;

namespace Uchu.Core
{
    public static class BitReaderExtensions
    {
        public static void Read(this BitReader @this, IDeserializable serializable) => serializable.Deserialize(@this);

        public static IEnumerable<byte> ReadBytes(this BitReader @this, int bytes)
        {
            Span<byte> buf = stackalloc byte[bytes];
            @this.Read(buf, buf.Length * 8);
            return buf.ToArray();
        }
        
        public static string ReadString(this BitReader @this, int length = 33, bool wide = false)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                if (wide) builder.Append((char) @this.Read<short>());
                else builder.Append((char) @this.Read<byte>());
                if (builder[builder.Length - 1] != '\0') continue;
                builder.Length--;
                break;
            }

            for (var i = 0; i < length - builder.Length - 1; i++)
            {
                if (wide) @this.Read<short>();
                else @this.Read<byte>();
            }

            return builder.ToString();
        }
    }
}