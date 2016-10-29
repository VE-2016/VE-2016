using System;
using System.Collections;
using T = AIMS.Libraries.CodeEditor.Syntax.Segment;

namespace AIMS.Libraries.CodeEditor.Syntax
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SegmentCollection : ICollection, IList, IEnumerable, ICloneable
    {
        private const int DefaultMinimumCapacity = 16;

        private T[] _array = new T[DefaultMinimumCapacity];
        private int _count = 0;
        private int _version = 0;

        // Construction
        /// <summary>
        /// 
        /// </summary>
        public SegmentCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public SegmentCollection(SegmentCollection collection)
        {
            AddRange(collection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public SegmentCollection(T[] array)
        {
            AddRange(array);
        }

        // Operations (type-safe ICollection)
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public void CopyTo(T[] array)
        {
            this.CopyTo(array, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        public void CopyTo(T[] array, int start)
        {
            if (_count > array.GetUpperBound(0) + 1 - start)
                throw new ArgumentException("Destination array was not long enough.");

            // for (int i=0; i < m_count; ++i) array[start+i] = m_array[i];
            Array.Copy(_array, 0, array, start, _count);
        }

        // Operations (type-safe IList)
        /// <summary>
        /// 
        /// </summary>
        public T this[int index]
        {
            get
            {
                ValidateIndex(index); // throws
                return _array[index];
            }
            set
            {
                ValidateIndex(index); // throws

                ++_version;
                _array[index] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int Add(T item)
        {
            if (NeedsGrowth())
                Grow();

            ++_version;
            _array[_count] = item;

            return _count++;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ++_version;
            _array = new T[DefaultMinimumCapacity];
            _count = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return ((IndexOf(item) == -1) ? false : true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; ++i)
                if (_array[i] == (item))
                    return i;
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        public void Insert(int position, T item)
        {
            ValidateIndex(position, true); // throws

            if (NeedsGrowth())
                Grow();

            ++_version;
            // for (int i=m_count; i > position; --i) m_array[i] = m_array[i-1];
            Array.Copy(_array, position, _array, position + 1, _count - position);

            _array[position] = item;
            _count++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            int index = IndexOf(item);
            if (index < 0)
                throw new ArgumentException("Cannot remove the specified item because it was not found in the specified Collection.");

            RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            ValidateIndex(index); // throws

            ++_version;
            _count--;
            // for (int i=index; i < m_count; ++i) m_array[i] = m_array[i+1];
            Array.Copy(_array, index + 1, _array, index, _count - index);

            if (NeedsTrimming())
                Trim();
        }

        // Operations (type-safe IEnumerable)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        // Operations (type-safe ICloneable)
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SegmentCollection Clone()
        {
            SegmentCollection tc = new SegmentCollection();
            tc.AddRange(this);
            tc.Capacity = _array.Length;
            tc._version = _version;
            return tc;
        }

        // Public helpers (just to mimic some nice features of ArrayList)
        /// <summary>
        /// 
        /// </summary>
        public int Capacity
        {
            get { return _array.Length; }
            set
            {
                if (value < _count) value = _count;
                if (value < DefaultMinimumCapacity) value = DefaultMinimumCapacity;

                if (_array.Length == value) return;

                ++_version;

                T[] temp = new T[value];
                // for (int i=0; i < m_count; ++i) temp[i] = m_array[i];
                Array.Copy(_array, 0, temp, 0, _count);
                _array = temp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(SegmentCollection collection)
        {
            // for (int i=0; i < collection.Count; ++i) Add(collection[i]);

            ++_version;

            Capacity += collection.Count;
            Array.Copy(collection._array, 0, _array, _count, collection._count);
            _count += collection.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        public void AddRange(T[] array)
        {
            // for (int i=0; i < array.Length; ++i) Add(array[i]);

            ++_version;

            Capacity += array.Length;
            Array.Copy(array, 0, _array, _count, array.Length);
            _count += array.Length;
        }

        // Implementation (helpers)

        private void ValidateIndex(int index)
        {
            ValidateIndex(index, false);
        }

        private void ValidateIndex(int index, bool allowEqualEnd)
        {
            int max = (allowEqualEnd) ? (_count) : (_count - 1);
            if (index < 0 || index > max)
                throw new ArgumentOutOfRangeException("Index was out of range.  Must be non-negative and less than the size of the collection.", (object)index, "Specified argument was out of the range of valid values.");
        }

        private bool NeedsGrowth()
        {
            return (_count >= Capacity);
        }

        private void Grow()
        {
            if (NeedsGrowth())
                Capacity = _count * 2;
        }

        private bool NeedsTrimming()
        {
            return (_count <= Capacity / 2);
        }

        private void Trim()
        {
            if (NeedsTrimming())
                Capacity = _count;
        }

        // Implementation (ICollection)

        /* redundant w/ type-safe method
			int ICollection.Count
			{
				get
				{ return m_count; }
			}
			*/

        bool ICollection.IsSynchronized
        {
            get { return _array.IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return _array.SyncRoot; }
        }

        void ICollection.CopyTo(Array array, int start)
        {
            this.CopyTo((T[])array, start);
        }

        // Implementation (IList)

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        object IList.this[int index]
        {
            get { return (object)this[index]; }
            set { this[index] = (T)value; }
        }

        int IList.Add(object item)
        {
            return this.Add((T)item);
        }

        /* redundant w/ type-safe method
			void IList.Clear()
			{
				this.Clear();
			}
			*/

        bool IList.Contains(object item)
        {
            return this.Contains((T)item);
        }

        int IList.IndexOf(object item)
        {
            return this.IndexOf((T)item);
        }

        void IList.Insert(int position, object item)
        {
            this.Insert(position, (T)item);
        }

        void IList.Remove(object item)
        {
            this.Remove((T)item);
        }

        /* redundant w/ type-safe method
			void IList.RemoveAt(int index)
			{
				this.RemoveAt(index);
			}
			*/

        // Implementation (IEnumerable)

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)(this.GetEnumerator());
        }

        // Implementation (ICloneable)

        object ICloneable.Clone()
        {
            return (object)(this.Clone());
        }

        // Nested enumerator class
        /// <summary>
        /// 
        /// </summary>
        public class Enumerator : IEnumerator
        {
            private SegmentCollection _collection;
            private int _index;
            private int _version;

            // Construction

            public Enumerator(SegmentCollection tc)
            {
                _collection = tc;
                _index = -1;
                _version = tc._version;
            }

            // Operations (type-safe IEnumerator)
            /// <summary>
            /// 
            /// </summary>
            public T Current
            {
                get { return _collection[_index]; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                //if (m_version != m_collection.m_version)
                //	throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                ++_index;
                return (_index < _collection.Count) ? true : false;
            }

            /// <summary>
            /// 
            /// </summary>
            public void Reset()
            {
                //if (m_version != m_collection.m_version)
                //	throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                _index = -1;
            }

            // Implementation (IEnumerator)

            object IEnumerator.Current
            {
                get { return (object)(this.Current); }
            }

            /* redundant w/ type-safe method
				bool IEnumerator.MoveNext()
				{
					return this.MoveNext();
				}
				*/

            /* redundant w/ type-safe method
				void IEnumerator.Reset()
				{
					this.Reset();
				}
				*/
        }
    }
}