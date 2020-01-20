using System.Collections;
using System.Collections.Generic;

namespace Uchu.World.Collections
{
    public class ActionMessageFormat : IList<object>
    {
        private List<object> Objects { get; set; }

        public ActionMessageFormat(IEnumerable<object> objects) : this()
        {
            Objects.AddRange(objects);
        }

        public ActionMessageFormat()
        {
            Objects = new List<object>();
        }
        
        public IEnumerator<object> GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(object item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(object item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(object item)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public int IndexOf(object item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index, object item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public object this[int index]
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }
    }
}