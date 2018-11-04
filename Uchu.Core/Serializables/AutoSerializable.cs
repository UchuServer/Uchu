using System;
using System.Linq;
using System.Reflection;
using RakDotNet;

namespace Uchu.Core
{
    public abstract class AutoSerializable : ISerializable
    {
        public virtual void Serialize(BitStream stream)
        {
            var type = GetType();
            var props = type.GetProperties().Where(p => p.GetMethod != null && !p.GetMethod.IsPrivate);
            var fields = type.GetFields().Where(f => !f.IsPrivate);

            foreach (var prop in props) _serializeProperty(stream, prop);
            foreach (var field in fields) _serializeField(stream, field);
        }

        public virtual void Deserialize(BitStream stream)
        {
            var type = GetType();
            var props = type.GetProperties().Where(p => p.SetMethod != null && !p.SetMethod.IsPrivate);
            var fields = type.GetFields().Where(f => !f.IsPrivate);

            foreach (var prop in props) _deserializeProperty(stream, prop);
            foreach (var field in fields) _deserializeField(stream, field);
        }

        private void _serializeProperty(BitStream stream, PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes<AutoSerializeAttribute>().ToArray();

            if (attrs.Length == 0)
                return;

            var attr = attrs[0];
            var val = property.GetValue(this);
            var type = attr.Type ?? property.PropertyType;

            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            _serializeType(stream, type, val, attr);
        }

        private void _serializeField(BitStream stream, FieldInfo field)
        {
            var attrs = field.GetCustomAttributes<AutoSerializeAttribute>().ToArray();

            if (attrs.Length == 0)
                return;

            var attr = attrs[0];
            var val = field.GetValue(this);
            var type = attr.Type ?? field.FieldType;

            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            _serializeType(stream, type, val, attr);
        }

        private void _deserializeProperty(BitStream stream, PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes<AutoSerializeAttribute>().ToArray();

            if (attrs.Length == 0)
                return;

            var attr = attrs[0];
            var type = attr.Type ?? property.PropertyType;

            var val = _deserializeType(stream, type, attr);

            property.SetValue(this, val);
        }

        private void _deserializeField(BitStream stream, FieldInfo field)
        {
            var attrs = field.GetCustomAttributes<AutoSerializeAttribute>().ToArray();

            if (attrs.Length == 0)
                return;

            var attr = attrs[0];
            var type = attr.Type ?? field.FieldType;

            var val = _deserializeType(stream, type, attr);

            field.SetValue(this, val);
        }

        private void _serializeType(BitStream stream, Type type, object val, AutoSerializeAttribute attr)
        {
            var compressed = attr.Compressed;
            var unsigned = attr.Unsigned;
            var wide = attr.Wide;
            var length = attr.Length;
            var boolean = attr.Bool;
            var writeType = type.IsEnum ? type.GetEnumUnderlyingType() : type;

            if (writeType == typeof(sbyte))
            {
                if (compressed) stream.WriteSByteCompressed((sbyte) val);
                else stream.WriteSByte((sbyte) val);
            }
            else if (writeType == typeof(byte))
            {
                if (compressed) stream.WriteByteCompressed((byte) val);
                else stream.WriteByte((byte) val);
            }
            else if (writeType == typeof(short))
            {
                if (compressed) stream.WriteShortCompressed((short) val);
                else stream.WriteShort((short) val);
            }
            else if (writeType == typeof(ushort))
            {
                if (compressed) stream.WriteUShortCompressed((ushort) val);
                else stream.WriteUShort((ushort) val);
            }
            else if (writeType == typeof(char))
            {
                if (compressed) stream.WriteCharCompressed((char) val);
                else stream.WriteChar((char) val);
            }
            else if (writeType == typeof(int))
            {
                if (compressed) stream.WriteIntCompressed((int) val);
                else stream.WriteInt((int) val);
            }
            else if (writeType == typeof(uint))
            {
                if (compressed) stream.WriteUIntCompressed((uint) val);
                else stream.WriteUInt((uint) val);
            }
            else if (writeType == typeof(long))
            {
                if (compressed) stream.WriteLongCompressed((long) val);
                else stream.WriteLong((long) val);
            }
            else if (writeType == typeof(ulong))
            {
                if (compressed) stream.WriteULongCompressed((ulong) val);
                else stream.WriteULong((ulong) val);
            }
            else if (writeType == typeof(double))
            {
                if (compressed) stream.WriteDoubleCompressed((double) val);
                else stream.WriteDouble((double) val);
            }
            else if (writeType == typeof(float))
            {
                if (compressed) stream.WriteFloatCompressed((float) val);
                else stream.WriteFloat((float) val);
            }
            else if (writeType == typeof(bool))
            {
                if (boolean) stream.WriteByte((byte) ((bool) val ? 1 : 0));
                else stream.WriteBit((bool) val);
            }
            else if (writeType == typeof(byte[]))
            {
                if (compressed) stream.WriteBitsCompressed((byte[]) val, length, unsigned);
                else stream.WriteBits((byte[]) val, length);
            }
            else if (writeType == typeof(string))
            {
                stream.WriteString((string) val, length, wide);
            }
            else if (typeof(ISerializable).IsAssignableFrom(writeType))
            {
                stream.WriteSerializable((ISerializable) val);
            }
            else
            {
                throw new NotSupportedException($"Type {writeType} is not supported");
            }
        }

        private object _deserializeType(BitStream stream, Type type, AutoSerializeAttribute attr)
        {
            var compressed = attr.Compressed;
            var unsigned = attr.Unsigned;
            var wide = attr.Wide;
            var length = attr.Length;
            var boolean = attr.Bool;
            var readType = type.IsEnum ? type.GetEnumUnderlyingType() : type;
            object val;

            if (readType == typeof(sbyte))
            {
                val = compressed ? stream.ReadCompressedSByte() : stream.ReadSByte();
            }
            else if (readType == typeof(byte))
            {
                val = compressed ? stream.ReadCompressedByte() : stream.ReadByte();
            }
            else if (readType == typeof(short))
            {
                val = compressed ? stream.ReadCompressedShort() : stream.ReadShort();
            }
            else if (readType == typeof(ushort))
            {
                val = compressed ? stream.ReadCompressedUShort() : stream.ReadUShort();
            }
            else if (readType == typeof(char))
            {
                val = compressed ? stream.ReadCompressedChar() : stream.ReadChar();
            }
            else if (readType == typeof(int))
            {
                val = compressed ? stream.ReadCompressedInt() : stream.ReadInt();
            }
            else if (readType == typeof(uint))
            {
                val = compressed ? stream.ReadCompressedUInt() : stream.ReadUInt();
            }
            else if (readType == typeof(long))
            {
                val = compressed ? stream.ReadCompressedLong() : stream.ReadLong();
            }
            else if (readType == typeof(ulong))
            {
                val = compressed ? stream.ReadCompressedULong() : stream.ReadULong();
            }
            else if (readType == typeof(double))
            {
                val = compressed ? stream.ReadCompressedDouble() : stream.ReadDouble();
            }
            else if (readType == typeof(float))
            {
                val = compressed ? stream.ReadCompressedFloat() : stream.ReadFloat();
            }
            else if (readType == typeof(bool))
            {
                val = boolean ? stream.ReadByte() == 1 : stream.ReadBit();
            }
            else if (readType == typeof(byte[]))
            {
                val = compressed ? stream.ReadCompressedBits(length, unsigned) : stream.ReadBits(length);
            }
            else if (readType == typeof(string))
            {
                val = stream.ReadString(length, wide);
            }
            else if (typeof(ISerializable).IsAssignableFrom(readType))
            {
                var instance = (ISerializable) Activator.CreateInstance(readType);

                stream.ReadSerializable(instance);

                val = instance;
            }
            else
            {
                throw new NotSupportedException($"Type {readType} is not supported");
            }

            return val;
        }
    }
}