using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Uchu.Core.Data
{
    public class ThreadedList<T> : IList<T> where T : class
    {
        /// <summary>
        /// Count of the entries in the list.
        /// </summary>
        public int Count => this._list.Count;
        
        /// <summary>
        /// Whether the list is read-only.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        /// List to store the data.
        /// </summary>
        private List<T> _list = new List<T>();

        /// <summary>
        /// Semaphore for performing operations.
        /// </summary>
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        /// <summary>
        /// Dictionary of cached arrays that are stored until the list changes.
        /// </summary>
        private readonly Dictionary<int, object[]> _arrays = new Dictionary<int, object[]>();
        
        /// <summary>
        /// Indexes the list.
        /// </summary>
        public T this[int index]
        {
            get => this._list[index];
            set => this.Set(index, value);
        }

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator() => this._list.GetEnumerator();

        /// <summary>
        /// Returns an enumerator for the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns if the list contains the given item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>Whether the item exists or not in the list.</returns>
        public bool Contains(T item) => this._list.Contains(item);
        
        /// <summary>
        /// Returns the index of the given item in the list.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns>The index of the item.</returns>
        public int IndexOf(T item) => this._list.IndexOf(item);
        
        /// <summary>
        /// Copies the contents of the list to an array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Index to start copying at.</param>
        public void CopyTo(T[] array, int arrayIndex) => this._list.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an array that is cached until the data changes.
        /// </summary>
        /// <returns>The converted array.</returns>
        public T[] ToArray() => this.GetArray((list) => list.ToArray());
        
        /// <summary>
        /// Returns an array that is cached until the data changes.
        /// </summary>
        /// <param name="convertFunction">Function that converts the list into an array.</param>
        /// <typeparam name="T2">Object to convert to.</typeparam>
        /// <returns>The converted array.</returns>
        public T2[] GetArray<T2>(Func<List<T>, T2[]> convertFunction) where T2 : class
        {
            if (convertFunction == null) return Array.Empty<T2>();
            this._semaphore.Wait();
            
            // Add the cache entry if it doesn't exist.
            var functionIndex = convertFunction.GetHashCode();
            if (!this._arrays.ContainsKey(functionIndex))
            {
                this._arrays[functionIndex] = convertFunction(this._list);
            }

            // Return the cached list.
            var array = this._arrays[functionIndex];
            this._semaphore.Release();
            return (T2[]) array;
        }
        
        /// <summary>
        /// Invalidates the "sub lists" of the list.
        /// </summary>
        private void InvalidateSubLists()
        {
            this._arrays.Clear();
        }
        
        /// <summary>
        /// Sets the value of the list.
        /// </summary>
        /// <param name="index">Index to change.</param>
        /// <param name="value">Value to change to.</param>
        public void Set(int index, T value)
        {
            this._semaphore.Wait();
            this._list[index] = value;
            this.InvalidateSubLists();
            this._semaphore.Release();
        }

        /// <summary>
        /// Adds an entry to the list.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item)
        {
            this._semaphore.Wait();
            this._list.Add(item);
            this.InvalidateSubLists();
            this._semaphore.Release();
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            this._semaphore.Wait();
            this._list.Clear();
            this.InvalidateSubLists();
            this._semaphore.Release();
        }

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">Item to remove from the list.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool Remove(T item)
        {
            this._semaphore.Wait();
            var result = this._list.Remove(item);
            this.InvalidateSubLists();
            this._semaphore.Release();
            return result;
        }
        
        /// <summary>
        /// Inserts an item into the list.
        /// </summary>
        /// <param name="index">Index to insert at.</param>
        /// <param name="item">Item to add.</param>
        public void Insert(int index, T item)
        {
            this._semaphore.Wait();
            this._list.Insert(index, item);
            this.InvalidateSubLists();
            this._semaphore.Release();
        }

        /// <summary>
        /// Removes an item at the given index.
        /// </summary>
        /// <param name="index">Index to remove.</param>
        public void RemoveAt(int index)
        {
            this._semaphore.Wait();
            this._list.RemoveAt(index);
            this.InvalidateSubLists();
            this._semaphore.Release();
        }
    }
}