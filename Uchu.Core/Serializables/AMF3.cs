using System;
using System.Collections.Generic;
using RakDotNet;

namespace Uchu.Core
{
    public class AMF3<T> : ISerializable
    {
        private readonly List<string> _strRefTable;
        private T _value;

        public T Value => _value;

        public AMF3(T value)
        {
            _strRefTable = new List<string>();
            _value = value;
        }

        public void Serialize(BitStream stream)
            => _writeType(stream, _value);

        public void Deserialize(BitStream stream)
            => _value = (T) _readType(stream);

        private void _writeU29(BitStream stream, long value)
        {
            if (value < 0x80)
            {
                stream.WriteByte((byte) value);
            }
            else if (value < 0x4000)
            {
                stream.WriteByte((byte) ((value >> 7) | 0x80));
                stream.WriteByte((byte) (value & 0x7f));
            }
            else if (value < 0x200000)
            {
                stream.WriteByte((byte) ((value >> 14) | 0x80));
                stream.WriteByte((byte) ((value >> 7) | 0x80));
                stream.WriteByte((byte) (value & 0x7f));
            }
            else if (value < 0x20000000)
            {
                stream.WriteByte((byte) ((value >> 22) | 0x80));
                stream.WriteByte((byte) ((value >> 15) | 0x80));
                stream.WriteByte((byte) ((value >> 7) | 0x80));
                stream.WriteByte((byte) (value & 0x7f));
            }
        }

        private void _writeString(BitStream stream, string str)
        {
            _writeU29(stream, (str.Length << 1) | 0x01);
            stream.WriteString(str, str.Length);
        }

        private void _writeDictionary(BitStream stream, Dictionary<string, object> dict)
        {
            _writeU29(stream, 0x01);

            foreach (var (k, v) in dict)
            {
                _writeString(stream, k);
                _writeType(stream, v);
            }

            _writeString(stream, "");
        }

        private void _writeType(BitStream stream, object type)
        {
            switch (type)
            {
                case null:
                    stream.WriteByte((byte) AMF3Marker.Undefined);
                    break;

                case bool b:
                    stream.WriteByte((byte) (b ? AMF3Marker.True : AMF3Marker.False));
                    break;

                case int i:
                    stream.WriteByte((byte) AMF3Marker.Integer);
                    _writeU29(stream, i);
                    break;

                case double d:
                    stream.WriteByte((byte) AMF3Marker.Double);
                    stream.WriteDouble(d);
                    break;

                case string s:
                    stream.WriteByte((byte) AMF3Marker.String);
                    _writeString(stream, s);
                    break;

                case Dictionary<string, object> dict:
                    stream.WriteByte((byte) AMF3Marker.Array);
                    _writeDictionary(stream, dict);
                    break;
            }
        }

        private long _readU29(BitStream stream)
        {
            var val = 0;

            for (var i = 0; i < 4; i++)
            {
                var b = stream.ReadByte();

                val = (val << 7) | b & 0x7f;

                if ((b & 0x80) == 0)
                    break;
            }

            return val;
        }

        private string _readString(BitStream stream)
        {
            var val = _readU29(stream);
            var isLiteral = (val & 0x01) != 0;

            val >>= 1;

            if (!isLiteral)
                return _strRefTable[(int) val];

            var str = stream.ReadString((int) val);

            if (!string.IsNullOrEmpty(str))
                _strRefTable.Add(str);

            return str;
        }

        private Dictionary<object, object> _readDictionary(BitStream stream)
        {
            var val = _readU29(stream);
            var isLiteral = (val & 0x01) != 0;

            val >>= 1;

            if (!isLiteral)
                throw new NotImplementedException();

            var dict = new Dictionary<object, object>();

            while (true)
            {
                var key = _readString(stream);

                if (string.IsNullOrEmpty(key))
                    break;

                dict[key] = _readType(stream);
            }

            for (var i = 0; i < val; i++)
            {
                dict[i] = _readType(stream);
            }

            return dict;
        }

        private object _readType(BitStream stream)
        {
            var marker = (AMF3Marker) stream.ReadByte();

            switch (marker)
            {
                case AMF3Marker.Undefined:
                case AMF3Marker.Null:
                    return null;

                case AMF3Marker.False:
                    return false;

                case AMF3Marker.True:
                    return true;

                case AMF3Marker.Integer:
                    return (int) _readU29(stream);

                case AMF3Marker.Double:
                    return stream.ReadDouble();

                case AMF3Marker.String:
                    return _readString(stream);

                case AMF3Marker.Array:
                    return _readDictionary(stream);

                default:
                    throw new NotImplementedException(marker.ToString());
            }
        }
    }
}