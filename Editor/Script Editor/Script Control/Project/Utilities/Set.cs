using System.Collections;
using System.Collections.Generic;

namespace ScriptControl
{
    /// <summary>
    /// Set container class. Contains a sorted list of unique objects.
    /// When adding an object that is already in the set, it is not added again.
    /// Add, Remove and Contains are O(n log n)-operations.
    /// </summary>
    public sealed class Set<T> : ICollection<T>
    {
        private SortedDictionary<T, object> _dict;

        #region Constructors

        public Set()
        {
            _dict = new SortedDictionary<T, object>();
        }

        public Set(IEnumerable<T> list)
            : this()
        {
            AddRange(list);
        }

        public Set(params T[] list)
            : this()
        {
            AddRange(list);
        }

        public Set(IComparer<T> comparer)
        {
            _dict = new SortedDictionary<T, object>(comparer);
        }

        public Set(IEnumerable<T> list, IComparer<T> comparer)
            : this(comparer)
        {
            AddRange(list);
        }

        #endregion Constructors

        public void Add(T element)
        {
            _dict[element] = null;
        }

        public void AddRange(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Add(element);
            }
        }

        public bool Contains(T element)
        {
            return _dict.ContainsKey(element);
        }

        public bool Remove(T element)
        {
            return _dict.Remove(element);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dict.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _dict.Count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dict.Keys.CopyTo(array, arrayIndex);
        }

        public T[] ToArray()
        {
            T[] arr = new T[_dict.Count];
            _dict.Keys.CopyTo(arr, 0);
            return arr;
        }

        public ReadOnlyCollectionWrapper<T> AsReadOnly()
        {
            return new ReadOnlyCollectionWrapper<T>(_dict.Keys);
        }
    }
}