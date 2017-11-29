using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptControl
{
    /// <summary>
    /// Wraps any collection to make it read-only.
    /// </summary>
    public sealed class ReadOnlyCollectionWrapper<T> : ICollection<T>
    {
        private readonly ICollection<T> _c;

        public ReadOnlyCollectionWrapper(ICollection<T> c)
        {
            if (c == null)
                throw new ArgumentNullException("c");
            _c = c;
        }

        public int Count
        {
            get
            {
                return _c.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _c.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _c.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _c.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_c).GetEnumerator();
        }
    }
}