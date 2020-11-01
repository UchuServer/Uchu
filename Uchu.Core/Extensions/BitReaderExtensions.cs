using System;
using System.Text;
using RakDotNet.IO;

namespace Uchu.Core
{
    public static class BitReaderExtensions
    {
        public static void Read(this BitReader @this, IDeserializable serializable)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), 
                    Resources.ResourceStrings.BitReader_Read_NullReaderException);
            if (serializable == null)
                throw new ArgumentNullException(nameof(serializable), 
                    Resources.ResourceStrings.BitReader_Read_NullSerializableException);
            serializable.Deserialize(@this);
        }

        public static byte[] ReadBytes(this BitReader @this, int bytes)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), 
                    Resources.ResourceStrings.BitReader_Readbytes_NullReaderException);
            
            Span<byte> buf = stackalloc byte[bytes];

            @this.Read(buf, buf.Length * 8);

            return buf.ToArray();
        }

        public static string ReadString(this BitReader @this, int length = 33, bool wide = false)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), 
                    Resources.ResourceStrings.BitReader_ReadString_NullReaderException);
            
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                if (wide) builder.Append((char) @this.Read<short>());
                else builder.Append((char) @this.Read<byte>());
                if (builder[^1] != '\0') continue;
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