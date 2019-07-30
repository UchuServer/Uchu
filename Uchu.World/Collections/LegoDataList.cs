using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Uchu.World.Collections
{
    public class LegoDataList : IList<object>
    {
        public const char InfoSeparator = '\u001F';
        private readonly List<(byte, object)> _list;

        public int Count => _list.Count;
        public bool IsReadOnly => false;

        public object this[int index]
        {
            get => _list[index].Item2;
            set => Insert(index, value);
        }

        public object this[int index, byte type]
        {
            get => _list[index].Item2;
            set => Insert(index, value, type);
        }

        public LegoDataList()
        {
            _list = new List<(byte, object)>();
        }

        public void Add(object item, byte type)
            => _list.Add((type, item));

        public void Add(object item)
        {
            var type =
                item is int ? 1 :
                item is float ? 3 :
                item is double ? 4 :
                item is uint ? 5 :
                item is bool ? 7 :
                item is long ? 8 :
                item is byte[] ? 13 : 0;

            Add(item, (byte) type);
        }

        public void Insert(int index, object item, byte type)
            => _list.Insert(index, (type, item));

        public void Insert(int index, object item)
        {
            var type =
                item is int ? 1 :
                item is float ? 3 :
                item is double ? 4 :
                item is uint ? 5 :
                item is bool ? 7 :
                item is long ? 8 :
                item is byte[] ? 13 : 0;

            Insert(index, item, (byte) type);
        }

        public void Clear()
            => _list.Clear();

        public bool Contains(object item)
            => _list.Exists(i => i.Item2 == item);

        public void RemoveAt(int index)
            => _list.RemoveAt(index);

        public bool Remove(object item)
            => _list.Remove(_list.Find(i => i.Item2 == item));

        public int IndexOf(object item)
            => _list.FindIndex(i => i.Item2 == item);

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var (_, v) in _list) yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override string ToString()
            => ToString("+");

        public string ToString(string separator)
        {
            var str = new StringBuilder();

            foreach (var (t, v) in _list)
            {
                string val;

                switch (v)
                {
                    case Vector2 vec2:
                        val = $"{vec2.X}{InfoSeparator}{vec2.Y}";
                        break;

                    case Vector3 vec3:
                        val = $"{vec3.X}{InfoSeparator}{vec3.Z}{InfoSeparator}{vec3.Y}";
                        break;

                    case Vector4 vec4:
                        val = $"{vec4.X}{InfoSeparator}{vec4.Z}{InfoSeparator}{vec4.Y}{InfoSeparator}{vec4.W}";
                        break;

                    case LegoDataList list:
                        val = list.ToString();
                        break;

                    default:
                        val = v.ToString();
                        break;
                }

                str.Append($"{t}:{val}");

                if (IndexOf(v) + 1 < Count)
                    str.Append(separator);
            }

            return str.ToString();
        }

        public static LegoDataList FromEnumerable<T>(IEnumerable<T> list)
        {
            var ldl = new LegoDataList();

            foreach (var item in list) ldl.Add(item);

            return ldl;
        }

        public static LegoDataList FromString(string text)
        {
            var list = new LegoDataList();
            var entries = text.Split('+');

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var firstColon = entry.IndexOf(':');
                var type = int.Parse(entry.Substring(0, firstColon));
                var val = entry.Substring(firstColon + 1);

                object v = null;

                switch (type)
                {
                    case 1:
                    case 2:
                        v = int.Parse(val);
                        break;

                    case 3:
                        v = float.Parse(val, CultureInfo.InvariantCulture);
                        break;

                    case 4:
                        v = double.Parse(val);
                        break;

                    case 5:
                    case 6:
                        v = uint.Parse(val);
                        break;

                    case 7:
                        v = int.Parse(val) == 1;
                        break;

                    case 8:
                    case 9:
                        v = long.Parse(val);
                        break;

                    default:
                        if (val.Contains('+'))
                        {
                            v = FromString(val);
                        }
                        else if (val.Contains('\u001F'))
                        {
                            var floats = val.Split('\u001F').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();

                            v =
                                floats.Length == 1 ? floats[0] :
                                floats.Length == 2 ? new Vector2(floats[0], floats[1]) :
                                floats.Length == 3 ? new Vector3(floats[0], floats[1], floats[2]) :
                                floats.Length == 4 ? new Vector4(floats[0], floats[1], floats[2], floats[3]) :
                                (object) val;
                        }
                        else
                        {
                            v = val;
                        }
                        break;
                }

                list[i, (byte) type] = v;
            }

            return list;
        }
    }
}