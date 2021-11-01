using System;
using System.Collections.Generic;
using RakDotNet.IO;

namespace Uchu.World.Social
{
    // Stolen from https://github.com/lcdr/utils/blob/master/utils/amf3.py
    // (Python code for WriteNumber translated to C#. Python code is
    // available under the AGPLv3 license, of which you can find a copy
    // in the top-level directory of Uchu.)
    public static class Amf3Helper
    {
        public static void WriteNumber(BitWriter writer, uint n)
        {
            if (n < 0x80)
            {
                writer.Write((byte) n);
            }
            else if (n < 0x4000)
            {
                writer.Write((byte) ((n >> 7) | 0x80));
                writer.Write((byte) (n & 0x7f));
            }
            else if (n < 0x200000)
            {
                writer.Write((byte) ((n >> 14) | 0x80));
                writer.Write((byte) ((n >> 7) | 0x80));
                writer.Write((byte) (n & 0x7f));
            }
            else if (n < 0x20000000)
            {
                writer.Write((byte) ((n >> 22) | 0x80));
                writer.Write((byte) ((n >> 15) | 0x80));
                writer.Write((byte) ((n >> 7) | 0x80));
                writer.Write((byte) (n & 0xff));
            }
        }

        public static void Write(BitWriter writer, object value)
        {
            switch (value)
            {
                case string str:
                    writer.Write((byte) Amf3Type.String);
                    WriteText(writer, str);
                    break;
                case int integer:
                    writer.Write((byte) Amf3Type.Integer);
                    WriteNumber(writer, (uint) integer);
                    break;
                case uint unsigned:
                    writer.Write((byte) Amf3Type.Integer);
                    WriteNumber(writer, unsigned);
                    break;
                case bool boolean:
                    writer.Write((byte) (boolean ? Amf3Type.True : Amf3Type.False));
                    break;
                case IDictionary<string, object> dict:
                    writer.Write((byte) Amf3Type.Array);
                    WriteDict(writer, dict);
                    break;
                case Array array:
                    writer.Write((byte) Amf3Type.Array);
                    WriteArray(writer, array);
                    break;
                case null:
                    writer.Write((byte) Amf3Type.Undefined);
                    break;
            }
        }

        public static void WriteText(BitWriter writer, string value)
        {
            var size = (value.Length << 1) + 1;

            WriteNumber(writer, (uint) size);

            foreach (var character in value)
            {
                writer.Write((byte) character);
            }
        }

        public static void WriteDict(BitWriter writer, IDictionary<string, object> dict) => WriteArrayAndDict(writer, dict: dict);

        public static void WriteArray(BitWriter writer, Array array) => WriteArrayAndDict(writer, array: array);

        public static void WriteArrayAndDict(BitWriter writer, Array array = null, IDictionary<string, object> dict = null)
        {
            var arrayLength = (uint) (array?.Length ?? 0);
            WriteNumber(writer, (arrayLength << 1) | 0x01);
            if (dict != null)
                foreach (var (key, value) in dict)
                {
                    WriteText(writer, key);
                    Write(writer, value);
                }
            WriteText(writer, "");
            if (array != null)
                foreach (var value in array)
                {
                    Write(writer, value);
                }
        }
    }
}
