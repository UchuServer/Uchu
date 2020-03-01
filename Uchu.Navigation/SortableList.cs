using System;
using System.Collections;

namespace Uchu.Navigation
{
	/// <summary>
	/// The SortableList allows to maintain a list sorted as long as needed.
	/// If no IComparer interface has been provided at construction, then the list expects the Objects to implement IComparer.
	/// If the list is not sorted it behaves like an ordinary list.
	/// When sorted, the list's "Add" method will put new objects at the right place.
	/// As well the "Contains" and "IndexOf" methods will perform a binary search.
	/// </summary>
	[Serializable]
	public class SortableList : IList, ICloneable
	{
		private ArrayList _list;
		private IComparer _comparer;
		private bool _useObjectsComparison;
		private bool _isSorted;
		private bool _keepSorted;
		private bool _addDuplicates;

		/// <summary>
		/// Default constructor.
		/// Since no IComparer is provided here, added objects must implement the IComparer interface.
		/// </summary>
		public SortableList() { InitProperties(null, 0); }

		/// <summary>
		/// Constructor.
		/// Since no IComparer is provided, added objects must implement the IComparer interface.
		/// </summary>
		/// <param name="capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
		public SortableList(int capacity) { InitProperties(null, capacity); }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer">Will be used to compare added elements for sort and search operations.</param>
		public SortableList(IComparer comparer) { InitProperties(comparer, 0); }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="comparer">Will be used to compare added elements for sort and search operations.</param>
		/// <param name="capacity">Capacity of the list (<see cref="ArrayList.Capacity">ArrayList.Capacity</see>)</param>
		public SortableList(IComparer comparer, int capacity) { InitProperties(comparer, capacity); }

		/// <summary>
		/// 'Get only' property that indicates if the list is sorted.
		/// </summary>
		public bool IsSorted => _isSorted;

		/// <summary>
		/// Get : Indicates if the list must be kept sorted from now on.
		/// Set : Tells the list if it must stay sorted or not. Impossible to set to true if the list is not sorted.
		/// <see cref="KeepSorted">KeepSorted</see>==true implies that <see cref="IsSorted">IsSorted</see>==true
		/// </summary>
		/// <exception cref="InvalidOperationException">Cannot be set to true if the list is not sorted yet.</exception>
		public bool KeepSorted
		{
			set
			{
				if ( value==true && !_isSorted ) throw new InvalidOperationException("The SortableList can only be kept sorted if it is sorted.");
				_keepSorted = value;
			}
			get => _keepSorted;
		}

		/// <summary>
		/// If set to true, it will not be possible to add an object to the list if its value is already in the list.
		/// </summary>
		public bool AddDuplicates { set => _addDuplicates = value;
			get => _addDuplicates;
		}

		/// <summary>
		/// IList implementation.
		/// Gets - or sets - object's value at a specified index.
		/// The set operation is impossible if the <see cref="KeepSorted">KeepSorted</see> property is set to true.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Index is less than zero or Index is greater than Count.</exception>
		/// <exception cref="InvalidOperationException">[] operator cannot be used to set a value if KeepSorted property is set to true.</exception>
		public object this[int index]
		{
			get
			{
				if ( index>=_list.Count || index<0 ) throw new ArgumentOutOfRangeException("Index is less than zero or Index is greater than Count.");
				return _list[index];
			}
			set
			{
				if ( _keepSorted ) throw new InvalidOperationException("[] operator cannot be used to set a value if KeepSorted property is set to true.");
				if ( index>=_list.Count || index<0 ) throw new ArgumentOutOfRangeException("Index is less than zero or Index is greater than Count.");
				if ( ObjectIsCompliant(value) )
				{
					var oBefore = index>0 ? _list[index-1] : null;
					var oAfter = index<Count-1 ? _list[index+1] : null;
					if ( oBefore!=null && _comparer.Compare(oBefore, value)>0 || oAfter!=null && _comparer.Compare(value, oAfter)>0 ) _isSorted = false;
					_list[index] = value;
				}
			}
		}

		/// <summary>
		/// IList implementation.
		/// If the <see cref="KeepSorted">KeepSorted</see> property is set to true, the object will be added at the right place.
		/// Else it will be added at the end of the list.
		/// </summary>
		/// <param name="o">The object to add.</param>
		/// <returns>The index where the object has been added.</returns>
		/// <exception cref="ArgumentException">The SortableList is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
		public int Add(object o)
		{
			var @return = -1;
			if ( ObjectIsCompliant(o) )
			{
				if ( _keepSorted )
				{
					var index = IndexOf(o);
					var newIndex = index>=0 ? index : -index-1;
					if (newIndex>=Count) _list.Add(o);
					else _list.Insert(newIndex, o);
					@return = newIndex;
				}
				else
				{
					_isSorted = false;
					@return = _list.Add(o);
				}
			}
			return @return;
		}

		/// <summary>
		/// IList implementation.
		/// Search for a specified object in the list.
		/// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
		/// Else the <see cref="Equals">Object.Equals</see> implementation is used.
		/// </summary>
		/// <param name="o">The object to look for</param>
		/// <returns>true if the object is in the list, otherwise false.</returns>
		public bool Contains(object o)
		{
			return _isSorted ? _list.BinarySearch(o, _comparer)>=0 : _list.Contains(o);
		}

		/// <summary>
		/// IList implementation.
		/// Returns the index of the specified object in the list.
		/// If the list is sorted, a <see cref="ArrayList.BinarySearch">BinarySearch</see> is performed using IComparer interface.
		/// Else the <see cref="Equals">Object.Equals</see> implementation of objects is used.
		/// </summary>
		/// <param name="o">The object to locate.</param>
		/// <returns>
		/// If the object has been found, a positive integer corresponding to its position.
		/// If the objects has not been found, a negative integer which is the bitwise complement of the index of the next element.
		/// </returns>
		public int IndexOf(object o)
		{
			var result = -1;
			if ( _isSorted )
			{
				result = _list.BinarySearch(o, _comparer);
				while ( result>0 && _list[result-1].Equals(o) ) result--; // We want to point at the FIRST occurence
			}
			else result = _list.IndexOf(o);
			return result;
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsFixedSize => _list.IsFixedSize;

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsReadOnly => _list.IsReadOnly;

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public void Clear() { _list.Clear(); }

		/// <summary>
		/// IList implementation.
		/// Inserts an objects at a specified index.
		/// Cannot be used if the list has its KeepSorted property set to true.
		/// </summary>
		/// <param name="index">The index before which the object must be added.</param>
		/// <param name="o">The object to add.</param>
		/// <exception cref="ArgumentException">The SortableList is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Index is less than zero or Index is greater than Count.</exception>
		/// <exception cref="InvalidOperationException">If the object is added at the specify index, the list will not be sorted any more and the <see cref="KeepSorted"/> property is set to true.</exception>
		public void Insert(int index, object o)
		{
			if ( _keepSorted ) throw new InvalidOperationException("Insert method cannot be called if KeepSorted property is set to true.");
			if ( index>=_list.Count || index<0 ) throw new ArgumentOutOfRangeException("Index is less than zero or Index is greater than Count.");
			if ( ObjectIsCompliant(o) )
			{
				var oBefore = index>0 ? _list[index-1] : null;
				var oAfter = _list[index];
				if ( oBefore!=null && _comparer.Compare(oBefore, o)>0 || oAfter!=null && _comparer.Compare(o, oAfter)>0 ) _isSorted = false;
				_list.Insert(index, o);
			}
		}

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="value">The object whose value must be removed if found in the list.</param>
		public void Remove(object value) { _list.Remove(value); }

		/// <summary>
		/// IList implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="index">Index of object to remove.</param>
		public void RemoveAt(int index) { _list.RemoveAt(index); }

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(Array array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }
		
		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public int Count => _list.Count;

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public bool IsSynchronized => _list.IsSynchronized;

		/// <summary>
		/// IList.ICollection implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public object SyncRoot => _list.SyncRoot;

		/// <summary>
		/// IList.IEnumerable implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <returns>Enumerator on the list.</returns>
		public IEnumerator GetEnumerator() { return _list.GetEnumerator(); }

		/// <summary>
		/// ICloneable implementation.
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		/// <returns>Cloned object.</returns>
		public object Clone()
		{
			var clone = new SortableList(_comparer, _list.Capacity);
			clone._list = (ArrayList)_list.Clone();
			clone._addDuplicates = _addDuplicates;
			clone._isSorted = _isSorted;
			clone._keepSorted = _keepSorted;
			return clone;
		}

		/// <summary>
		/// Idem IndexOf(object), but starting at a specified position in the list
		/// </summary>
		/// <param name="o">The object to locate.</param>
		/// <param name="start">The index for start position.</param>
		/// <returns></returns>
		public int IndexOf(object o, int start)
		{
			var result = -1;
			if ( _isSorted )
			{
				result = _list.BinarySearch(start, _list.Count-start, o, _comparer);
				while ( result>start && _list[result-1].Equals(o) ) result--; // We want to point at the first occurence
			}
			else result = _list.IndexOf(o, start);
			return result;
		}

		/// <summary>
		/// Defines an equality for two objects
		/// </summary>
		public delegate bool Equality(object o1, object o2);

		/// <summary>
		/// Idem IndexOf(object), but with a specified equality function
		/// </summary>
		/// <param name="o">The object to locate.</param>
		/// <param name="areEqual">Equality function to use for the search.</param>
		/// <returns></returns>
		public int IndexOf(object o, Equality areEqual)
		{
			for (var i=0; i<_list.Count; i++)
				if ( areEqual(_list[i], o) ) return i;
			return -1;
		}

		/// <summary>
		/// Idem IndexOf(object), but with a start index and a specified equality function
		/// </summary>
		/// <param name="o">The object to locate.</param>
		/// <param name="start">The index for start position.</param>
		/// <param name="areEqual">Equality function to use for the search.</param>
		/// <returns></returns>
		public int IndexOf(object o, int start, Equality areEqual)
		{
			if ( start<0 || start>=_list.Count ) throw new ArgumentException("Start index must belong to [0; Count-1].");
			for (var i=start; i<_list.Count; i++)
				if ( areEqual(_list[i], o) ) return i;
			return -1;
		}

		/// <summary>
		/// Idem <see cref="ArrayList">ArrayList</see>
		/// </summary>
		public int Capacity { get => _list.Capacity;
			set => _list.Capacity = value;
		}

		/// <summary>
		/// Object.ToString() override.
		/// Build a string to represent the list.
		/// </summary>
		/// <returns>The string refecting the list.</returns>
		public override string ToString()
		{
			var outString = "{";
			for (var i=0; i<_list.Count; i++)
				outString += _list[i] + (i!=_list.Count-1 ? "; " : "}");
			return outString;
		}

		/// <summary>
		/// Object.Equals() override.
		/// </summary>
		/// <returns>true if object is equal to this, otherwise false.</returns>
		public override bool Equals(object o)
		{
			var sl = (SortableList)o;
			if ( sl.Count!=Count ) return false;
			for (var i=0; i<Count; i++)
				if ( !sl[i].Equals(this[i]) ) return false;
			return true;
		}

		/// <summary>
		/// Object.GetHashCode() override.
		/// </summary>
		/// <returns>HashCode value.</returns>
		public override int GetHashCode() { return _list.GetHashCode(); }

		/// <summary>
		/// Sorts the elements in the list using <see cref="ArrayList.Sort">ArrayList.Sort</see>.
		/// Does nothing if the list is already sorted.
		/// </summary>
		public void Sort()
		{
			if (_isSorted) return;
			_list.Sort(_comparer);
			_isSorted = true;
		}

		/// <summary>
		/// If the <see cref="KeepSorted">KeepSorted</see> property is set to true, the object will be added at the right place.
		/// Else it will be appended to the list.
		/// </summary>
		/// <param name="c">The object to add.</param>
		/// <returns>The index where the object has been added.</returns>
		/// <exception cref="ArgumentException">The SortableList is set to use object's IComparable interface, and the specifed object does not implement this interface.</exception>
		public void AddRange(ICollection c)
		{
			if ( _keepSorted ) foreach (var o in c) Add(o);
			else _list.AddRange(c);
		}

		/// <summary>
		/// Inserts a collection of objects at a specified index.
		/// Should not be used if the list is the KeepSorted property is set to true.
		/// </summary>
		/// <param name="index">The index before which the objects must be added.</param>
		/// <param name="c">The object to add.</param>
		/// <exception cref="ArgumentException">The SortableList is set to use objects's IComparable interface, and the specifed object does not implement this interface.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Index is less than zero or Index is greater than Count.</exception>
		/// <exception cref="InvalidOperationException">If the object is added at the specify index, the list will not be sorted any more and the <see cref="KeepSorted"/> property is set to true.</exception>
		public void InsertRange(int index, ICollection c)
		{
			if ( _keepSorted ) foreach (var o in c) Insert(index++, o);
			else _list.InsertRange(index, c);
		}

		/// <summary>
		/// Limits the number of occurrences of a specified value.
		/// Same values are equals according to the Equals() method of objects in the list.
		/// The first occurrences encountered are kept.
		/// </summary>
		/// <param name="value">Value whose occurrences number must be limited.</param>
		/// <param name="nbValuesToKeep">Number of occurrences to keep</param>
		public void LimitNbOccurrences(object value, int nbValuesToKeep)
		{
			if (value==null) throw new ArgumentNullException("value");
			var pos = 0;
			while ( (pos=IndexOf(value, pos)) >= 0 )
			{
				 if ( nbValuesToKeep<=0 ) _list.RemoveAt(pos);
				else { pos++; nbValuesToKeep--; }
				if ( _isSorted && _comparer.Compare(_list[pos], value)>0 ) break; // No need to follow
			}
		}

		/// <summary>
		/// Removes all duplicates in the list.
		/// Each value encountered will have only one representant.
		/// </summary>
		public void RemoveDuplicates()
		{
			int posIt;
			if (_isSorted)
			{
				posIt = 0;
				while ( posIt<Count-1 )
				{
					if ( _comparer.Compare(this[posIt], this[posIt+1])==0 ) RemoveAt(posIt);
					else posIt++;
				}
			}
			else
			{
				var left = 0;
				while ( left>=0 )
				{
					posIt = left+1;
					while (posIt>0)
					{
						if ( left!=posIt && _comparer.Compare(this[left], this[posIt])==0 ) RemoveAt(posIt);
						else posIt++;
					}
					left++;
				}
			}
		}

		/// <summary>
		/// Returns the object of the list whose value is minimum
		/// </summary>
		/// <returns>The minimum object in the list</returns>
		public int IndexOfMin()
		{
			var retInt = -1;
			if ( _list.Count>0 )
			{
				retInt = 0;
				var retObj = _list[0];
				if ( !_isSorted )
				{
					for ( var i=1; i<_list.Count; i++ )
						if ( _comparer.Compare(retObj, _list[i])>0 )
						{
							retObj = _list[i];
							retInt = i;
						}
				}
			}
			return retInt;
		}

		/// <summary>
		/// Returns the object of the list whose value is maximum
		/// </summary>
		/// <returns>The maximum object in the list</returns>
		public int IndexOfMax()
		{
			var retInt = -1;
			if ( _list.Count>0 )
			{
				retInt = _list.Count-1;
				var retObj = _list[_list.Count-1];
				if ( !_isSorted )
				{
					for ( var i=_list.Count-2; i>=0; i-- )
						if ( _comparer.Compare(retObj, _list[i])<0 )
						{
							retObj = _list[i];
							retInt = i;
						}
				}
			}
			return retInt;
		}

		private bool ObjectIsCompliant(object o)
		{
			if ( _useObjectsComparison && !(o is IComparable) ) throw new ArgumentException("The SortableList is set to use the IComparable interface of objects, and the object to add does not implement the IComparable interface.");
			if ( !_addDuplicates && Contains(o) ) return false;
			return true;
		}

		private class Comparison : IComparer
		{
			public int Compare(object o1, object o2)
			{
				var c = o1 as IComparable;
				return c.CompareTo(o2);
			}
		}

		private void InitProperties(IComparer comparer, int capacity)
		{
			if ( comparer!=null )
			{
				_comparer = comparer;
				_useObjectsComparison = false;
			}
			else
			{
				_comparer = new Comparison();
				_useObjectsComparison = true;
			}
			_list = capacity>0 ? new ArrayList(capacity) : new ArrayList();
			_isSorted = true;
			_keepSorted = true;
			_addDuplicates = true;
		}
	}
}
