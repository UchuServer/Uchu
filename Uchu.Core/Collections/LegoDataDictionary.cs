using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RakDotNet;

namespace Uchu.Core.Collections
{
    public class LegoDataDictionary : IDictionary<string, object>, ISerializable
    {
        private readonly Dictionary<string, (byte, object)> _map;

        public int Count => _map.Count;
        public bool IsReadOnly => false;

        public ICollection<string> Keys => _map.Keys;
        public ICollection<object> Values => _map.Values.Select(v => v.Item2).ToArray();

        public LegoDataDictionary()
        {
            _map = new Dictionary<string, (byte, object)>();
        }

        public object this[string key]
        {
            get => null;
            set => Add(key, value);
        }

        public object this[string key, byte type]
        {
            get => null;
            set => Add(key, value, type);
        }

        public void Add(string key, object value, byte type)
            => _map[key] = (type, value);

        public void Add(string key, object value)
        {
            var type =
                value is string ? 0 :
                value is int ? 1 :
                value is float ? 3 :
                value is double ? 4 :
                value is uint ? 5 :
                value is bool ? 7 :
                value is long ? 8 :
                value is byte[] ? 13 :
                throw new ArgumentException("Invalid type", nameof(value));

            Add(key, value, (byte) type);
        }

        public void Add(KeyValuePair<string, object> item)
            => Add(item.Key, item.Value);

        public void Clear()
            => _map.Clear();

        public bool ContainsKey(string key)
            => _map.ContainsKey(key);

        public bool Contains(KeyValuePair<string, object> item)
            => false;

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
            => _map.Remove(key);

        public bool Remove(KeyValuePair<string, object> item)
            => false;

        public bool TryGetValue(string key, out object value)
        {
            value = null;

            return false;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Serialize(BitStream stream)
        {
            stream.WriteUInt((uint) _map.Count);

            foreach (var (key, (type, value)) in _map)
            {
                stream.WriteByte((byte) (key.Length * 2));
                stream.WriteString(key, key.Length, true);
                stream.WriteByte(type);

                switch (type)
                {
                    case 1:
                    case 2:
                        stream.WriteInt((int) value);
                        break;

                    case 3:
                        stream.WriteFloat((float) value);
                        break;

                    case 4:
                        stream.WriteDouble((double) value);
                        break;

                    case 5:
                    case 6:
                        stream.WriteUInt((uint) value);
                        break;

                    case 7:
                        stream.WriteBit((bool) value);
                        break;

                    case 8:
                    case 9:
                        stream.WriteLong((long) value);
                        break;

                    case 13:
                        var bytes = (byte[]) value;

                        stream.WriteUInt((uint) bytes.Length);
                        stream.Write(bytes);
                        break;

                    default:
                        var str = (string) value;

                        stream.WriteUInt((uint) str.Length);
                        stream.WriteString(str, str.Length, true);
                        break;
                }
            }
        }

        public void Deserialize(BitStream stream)
        {
            throw new NotImplementedException();
        }
    }
}