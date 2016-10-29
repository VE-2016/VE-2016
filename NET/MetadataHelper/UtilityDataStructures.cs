//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using System.Diagnostics;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.UtilityDataStructures
{
#pragma warning disable 1591

    public static class HashHelper
    {
        public static uint HashInt1(uint key)
        {
            unchecked
            {
                uint a = 0x9e3779b9 + key;
                uint b = 0x9e3779b9;
                uint c = 16777619;
                a -= b; a -= c; a ^= (c >> 13);
                b -= c; b -= a; b ^= (a << 8);
                c -= a; c -= b; c ^= (b >> 13);
                a -= b; a -= c; a ^= (c >> 12);
                b -= c; b -= a; b ^= (a << 16);
                c -= a; c -= b; c ^= (b >> 5);
                a -= b; a -= c; a ^= (c >> 3);
                b -= c; b -= a; b ^= (a << 10);
                c -= a; c -= b; c ^= (b >> 15);
                return c;
            }
        }

        public static uint HashInt2(uint key)
        {
            unchecked
            {
                uint hash = 0xB1635D64 + key;
                hash += (hash << 3);
                hash ^= (hash >> 11);
                hash += (hash << 15);
                hash |= 0x00000001; //  To make sure that this is relatively prime with power of 2
                return hash;
            }
        }

        public static uint HashDoubleInt1(
          uint key1,
          uint key2
        )
        {
            unchecked
            {
                uint a = 0x9e3779b9 + key1;
                uint b = 0x9e3779b9 + key2;
                uint c = 16777619;
                a -= b; a -= c; a ^= (c >> 13);
                b -= c; b -= a; b ^= (a << 8);
                c -= a; c -= b; c ^= (b >> 13);
                a -= b; a -= c; a ^= (c >> 12);
                b -= c; b -= a; b ^= (a << 16);
                c -= a; c -= b; c ^= (b >> 5);
                a -= b; a -= c; a ^= (c >> 3);
                b -= c; b -= a; b ^= (a << 10);
                c -= a; c -= b; c ^= (b >> 15);
                return c;
            }
        }

        public static uint HashDoubleInt2(
          uint key1,
          uint key2
        )
        {
            unchecked
            {
                uint hash = 0xB1635D64 + key1;
                hash += (hash << 10);
                hash ^= (hash >> 6);
                hash += key2;
                hash += (hash << 3);
                hash ^= (hash >> 11);
                hash += (hash << 15);
                hash |= 0x00000001; //  To make sure that this is relatively prime with power of 2
                return hash;
            }
        }

        public static uint StartHash(uint key)
        {
            uint hash = 0xB1635D64 + key;
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        public static uint ContinueHash(uint prevHash, uint key)
        {
            unchecked
            {
                uint hash = prevHash + key;
                hash += (hash << 10);
                hash ^= (hash >> 6);
                return hash;
            }
        }

#pragma warning restore 1591
    }

    /// <summary>
    /// Hashtable that can host multiple values for the same uint key.
    /// </summary>
    /// <typeparam name="InternalT"></typeparam>
    public sealed class MultiHashtable<InternalT> where InternalT : class
    {
        private struct KeyValuePair
        {
            internal uint Key;
            internal InternalT Value;
        }

        private KeyValuePair[] _keyValueTable;
        private uint _size;
        private uint _resizeCount;
        private uint _count;
        private const int LoadPercent = 60;
        // ^ invariant (this.Size&(this.Size-1)) == 0;

        private static uint SizeFromExpectedEntries(uint expectedEntries)
        {
            uint expectedSize = (expectedEntries * 10) / 6; ;
            uint initialSize = 16;
            while (initialSize < expectedSize && initialSize > 0) initialSize <<= 1;
            return initialSize;
        }

        /// <summary>
        /// Constructor for MultiHashtable
        /// </summary>
        public MultiHashtable()
          : this(16)
        {
        }

        /// <summary>
        /// Constructor for MultiHashtable
        /// </summary>
        public MultiHashtable(uint expectedEntries)
        {
            _size = SizeFromExpectedEntries(expectedEntries);
            _resizeCount = _size * 6 / 10;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
        }

        /// <summary>
        /// Count of elements in MultiHashtable
        /// </summary>
        public uint Count
        {
            get
            {
                return _count;
            }
        }

        private void Expand()
        {
            KeyValuePair[] oldKeyValueTable = _keyValueTable;
            _size <<= 1;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
            _resizeCount = _size * 6 / 10;
            int len = oldKeyValueTable.Length;
            for (int i = 0; i < len; ++i)
            {
                uint key = oldKeyValueTable[i].Key;
                InternalT value = oldKeyValueTable[i].Value;
                if (value != null)
                    this.AddInternal(key, value);
            }
        }

        private void AddInternal(
          uint key,
          InternalT value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key == key && _keyValueTable[tableIndex].Value == value)
                        return;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                _keyValueTable[tableIndex].Key = key;
                _keyValueTable[tableIndex].Value = value;
                _count++;
            }
        }

        /// <summary>
        /// Add element to MultiHashtable
        /// </summary>
        public void Add(
          uint key,
          InternalT value
        )
        {
            if (_count >= _resizeCount)
            {
                this.Expand();
            }
            this.AddInternal(key, value);
        }

        /// <summary>
        /// Checks if key and value is present in the MultiHashtable
        /// </summary>
        public bool Contains(
          uint key,
          InternalT value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key == key && _keyValueTable[tableIndex].Value == value)
                        return true;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                return false;
            }
        }

        /// <summary>
        /// Enumerator to enumerate values with given key.
        /// </summary>
        public struct KeyedValuesEnumerator
        {
            private MultiHashtable<InternalT> _multiHashtable;
            private uint _key;
            private uint _hash1;
            private uint _hash2;
            private uint _currentIndex;

            internal KeyedValuesEnumerator(
              MultiHashtable<InternalT> multiHashtable,
              uint key
            )
            {
                _multiHashtable = multiHashtable;
                _key = key;
                _hash1 = HashHelper.HashInt1(key);
                _hash2 = HashHelper.HashInt2(key);
                _currentIndex = 0xFFFFFFFF;
            }

            /// <summary>
            /// Get the current element.
            /// </summary>
            /// <returns></returns>
            public InternalT Current
            {
                get
                {
                    return _multiHashtable._keyValueTable[_currentIndex].Value;
                }
            }

            /// <summary>
            /// Move to next element.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                unchecked
                {
                    uint size = _multiHashtable._size;
                    uint mask = size - 1;
                    uint key = _key;
                    uint hash1 = _hash1;
                    uint hash2 = _hash2;
                    KeyValuePair[] keyValueTable = _multiHashtable._keyValueTable;
                    uint currentIndex = _currentIndex;
                    if (currentIndex == 0xFFFFFFFF)
                        currentIndex = hash1 & mask;
                    else
                        currentIndex = (currentIndex + hash2) & mask;
                    while (keyValueTable[currentIndex].Value != null)
                    {
                        if (keyValueTable[currentIndex].Key == key)
                            break;
                        currentIndex = (currentIndex + hash2) & mask;
                    }
                    _currentIndex = currentIndex;
                    return keyValueTable[currentIndex].Value != null;
                }
            }

            /// <summary>
            /// Reset the enumeration.
            /// </summary>
            /// <returns></returns>
            public void Reset()
            {
                _currentIndex = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Enumerable to enumerate values with given key.
        /// </summary>
        public struct KeyedValuesEnumerable
        {
            private MultiHashtable<InternalT> _multiHashtable;
            private uint _key;

            internal KeyedValuesEnumerable(
              MultiHashtable<InternalT> multiHashtable,
              uint key
            )
            {
                _multiHashtable = multiHashtable;
                _key = key;
            }

            /// <summary>
            /// Return the enumerator.
            /// </summary>
            /// <returns></returns>
            public KeyedValuesEnumerator GetEnumerator()
            {
                return new KeyedValuesEnumerator(_multiHashtable, _key);
            }
        }

        /// <summary>
        /// Enumeration to return all the values associated with the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public KeyedValuesEnumerable GetValuesFor(uint key)
        {
            return new KeyedValuesEnumerable(this, key);
        }

        /// <summary>
        /// Enumerator to enumerate all values.
        /// </summary>
        public struct ValuesEnumerator
        {
            private MultiHashtable<InternalT> _multiHashtable;
            private uint _currentIndex;

            internal ValuesEnumerator(
              MultiHashtable<InternalT> multiHashtable
            )
            {
                _multiHashtable = multiHashtable;
                _currentIndex = 0xFFFFFFFF;
            }

            /// <summary>
            /// Get the current element.
            /// </summary>
            /// <returns></returns>
            public InternalT Current
            {
                get
                {
                    return _multiHashtable._keyValueTable[_currentIndex].Value;
                }
            }

            /// <summary>
            /// Move to next element.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                unchecked
                {
                    uint size = _multiHashtable._size;
                    uint currentIndex = _currentIndex + 1;
                    if (currentIndex >= size)
                    {
                        return false;
                    }
                    KeyValuePair[] keyValueTable = _multiHashtable._keyValueTable;
                    while (currentIndex < size && keyValueTable[currentIndex].Value == null)
                    {
                        currentIndex++;
                    }
                    _currentIndex = currentIndex;
                    return currentIndex < size && keyValueTable[currentIndex].Value != null;
                }
            }

            /// <summary>
            /// Reset the enumeration.
            /// </summary>
            /// <returns></returns>
            public void Reset()
            {
                _currentIndex = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Enumerable to enumerate all values.
        /// </summary>
        public struct ValuesEnumerable
        {
            private MultiHashtable<InternalT> _multiHashtable;

            internal ValuesEnumerable(
              MultiHashtable<InternalT> multiHashtable
            )
            {
                _multiHashtable = multiHashtable;
            }

            /// <summary>
            /// Return the enumerator.
            /// </summary>
            /// <returns></returns>
            public ValuesEnumerator GetEnumerator()
            {
                return new ValuesEnumerator(_multiHashtable);
            }
        }

        /// <summary>
        /// Enumeration of all the values
        /// </summary>
        public ValuesEnumerable Values
        {
            get
            {
                return new ValuesEnumerable(this);
            }
        }
    }

    /// <summary>
    /// Hashtable that can hold only single value per uint key.
    /// </summary>
    /// <typeparam name="InternalT"></typeparam>
    public sealed class Hashtable<InternalT> where InternalT : class
    {
        private struct KeyValuePair
        {
            internal uint Key;
            internal InternalT Value;
        }

        private KeyValuePair[] _keyValueTable;
        private uint _size;
        private uint _resizeCount;
        private uint _count;
        private const int LoadPercent = 60;
        // ^ invariant (this.Size&(this.Size-1)) == 0;

        private static uint SizeFromExpectedEntries(uint expectedEntries)
        {
            uint expectedSize = (expectedEntries * 10) / 6; ;
            uint initialSize = 16;
            while (initialSize < expectedSize && initialSize > 0) initialSize <<= 1;
            return initialSize;
        }

        /// <summary>
        /// Constructor for Hashtable
        /// </summary>
        public Hashtable()
          : this(16)
        {
        }

        /// <summary>
        /// Constructor for Hashtable
        /// </summary>
        public Hashtable(uint expectedEntries)
        {
            _size = SizeFromExpectedEntries(expectedEntries);
            _resizeCount = _size * 6 / 10;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public uint Count
        {
            get
            {
                return _count;
            }
        }

        private void Expand()
        {
            KeyValuePair[] oldKeyValueTable = _keyValueTable;
            _size <<= 1;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
            _resizeCount = _size * 6 / 10;
            int len = oldKeyValueTable.Length;
            for (int i = 0; i < len; ++i)
            {
                uint key = oldKeyValueTable[i].Key;
                InternalT value = oldKeyValueTable[i].Value;
                if (value != null)
                    this.AddInternal(key, value);
            }
        }

        private void AddInternal(
          uint key,
          InternalT value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key == key)
                    {
                        Debug.Assert(_keyValueTable[tableIndex].Value == value);
                        return;
                    }
                    tableIndex = (tableIndex + hash2) & mask;
                }
                _keyValueTable[tableIndex].Key = key;
                _keyValueTable[tableIndex].Value = value;
                _count++;
            }
        }

        /// <summary>
        /// Add element to the Hashtable
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(
          uint key,
          InternalT value
        )
        {
            if (_count >= _resizeCount)
            {
                this.Expand();
            }
            this.AddInternal(key, value);
        }

        /// <summary>
        /// Find element in the Hashtable
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public InternalT/*?*/ Find(
          uint key
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key == key)
                        return _keyValueTable[tableIndex].Value;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                return null;
            }
        }

        /// <summary>
        /// Enumerator for elements
        /// </summary>
        public struct ValuesEnumerator
        {
            private Hashtable<InternalT> _hashtable;
            private uint _currentIndex;

            internal ValuesEnumerator(
              Hashtable<InternalT> hashtable
            )
            {
                _hashtable = hashtable;
                _currentIndex = 0xFFFFFFFF;
            }

            /// <summary>
            /// Current element
            /// </summary>
            public InternalT Current
            {
                get
                {
                    return _hashtable._keyValueTable[_currentIndex].Value;
                }
            }

            /// <summary>
            /// Move to next element
            /// </summary>
            public bool MoveNext()
            {
                unchecked
                {
                    uint size = _hashtable._size;
                    uint currentIndex = _currentIndex + 1;
                    if (currentIndex >= size)
                    {
                        return false;
                    }
                    KeyValuePair[] keyValueTable = _hashtable._keyValueTable;
                    while (currentIndex < size && keyValueTable[currentIndex].Value == null)
                    {
                        currentIndex++;
                    }
                    _currentIndex = currentIndex;
                    return currentIndex < size && keyValueTable[currentIndex].Value != null;
                }
            }

            /// <summary>
            /// Reset the enumerator
            /// </summary>
            public void Reset()
            {
                _currentIndex = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Enumerable for elements
        /// </summary>
        public struct ValuesEnumerable
        {
            private Hashtable<InternalT> _hashtable;

            internal ValuesEnumerable(
              Hashtable<InternalT> hashtable
            )
            {
                _hashtable = hashtable;
            }

            /// <summary>
            /// Get the enumerator
            /// </summary>
            /// <returns></returns>
            public ValuesEnumerator GetEnumerator()
            {
                return new ValuesEnumerator(_hashtable);
            }
        }

        /// <summary>
        /// Enumerable of all the values
        /// </summary>
        public ValuesEnumerable Values
        {
            get
            {
                return new ValuesEnumerable(this);
            }
        }
    }

    /// <summary>
    /// Hashtable that can hold only single uint value per uint key.
    /// </summary>
    public sealed class Hashtable
    {
        private struct KeyValuePair
        {
            internal uint Key;
            internal uint Value;
        }

        private KeyValuePair[] _keyValueTable;
        private uint _size;
        private uint _resizeCount;
        private uint _count;
        private const int LoadPercent = 60;
        // ^ invariant (this.Size&(this.Size-1)) == 0;

        private static uint SizeFromExpectedEntries(uint expectedEntries)
        {
            uint expectedSize = (expectedEntries * 10) / 6; ;
            uint initialSize = 16;
            while (initialSize < expectedSize && initialSize > 0) initialSize <<= 1;
            return initialSize;
        }

        /// <summary>
        /// Constructor for Hashtable
        /// </summary>
        public Hashtable()
          : this(16)
        {
        }

        /// <summary>
        /// Constructor for Hashtable
        /// </summary>
        public Hashtable(uint expectedEntries)
        {
            _size = SizeFromExpectedEntries(expectedEntries);
            _resizeCount = _size * 6 / 10;
            _keyValueTable = new KeyValuePair[_size];
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public uint Count
        {
            get
            {
                return _count;
            }
        }

        private void Expand()
        {
            KeyValuePair[] oldKeyValueTable = _keyValueTable;
            _size <<= 1;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
            _resizeCount = _size * 6 / 10;
            int len = oldKeyValueTable.Length;
            for (int i = 0; i < len; ++i)
            {
                uint key = oldKeyValueTable[i].Key;
                uint value = oldKeyValueTable[i].Value;
                if (value != 0)
                    this.AddInternal(key, value);
            }
        }

        private void AddInternal(
          uint key,
          uint value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != 0)
                {
                    if (_keyValueTable[tableIndex].Key == key)
                    {
                        Debug.Assert(_keyValueTable[tableIndex].Value == value);
                        return;
                    }
                    tableIndex = (tableIndex + hash2) & mask;
                }
                _keyValueTable[tableIndex].Key = key;
                _keyValueTable[tableIndex].Value = value;
                _count++;
            }
        }

        /// <summary>
        /// Add element to the Hashtable
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(
          uint key,
          uint value
        )
        {
            if (_count >= _resizeCount)
            {
                this.Expand();
            }
            this.AddInternal(key, value);
        }

        /// <summary>
        /// Find element in the Hashtable
        /// </summary>
        /// <param name="key"></param>
        public uint Find(
          uint key
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashInt1(key);
                uint hash2 = HashHelper.HashInt2(key);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != 0)
                {
                    if (_keyValueTable[tableIndex].Key == key)
                        return _keyValueTable[tableIndex].Value;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                return 0;
            }
        }

        /// <summary>
        /// Enumerator for elements
        /// </summary>
        public struct ValuesEnumerator
        {
            private Hashtable _hashtable;
            private uint _currentIndex;

            internal ValuesEnumerator(
              Hashtable hashtable
            )
            {
                _hashtable = hashtable;
                _currentIndex = 0xFFFFFFFF;
            }

            /// <summary>
            /// Current element
            /// </summary>
            public uint Current
            {
                get
                {
                    return _hashtable._keyValueTable[_currentIndex].Value;
                }
            }

            /// <summary>
            /// Move to next element
            /// </summary>
            public bool MoveNext()
            {
                unchecked
                {
                    uint size = _hashtable._size;
                    uint currentIndex = _currentIndex + 1;
                    if (currentIndex >= size)
                    {
                        return false;
                    }
                    KeyValuePair[] keyValueTable = _hashtable._keyValueTable;
                    while (currentIndex < size && keyValueTable[currentIndex].Value == 0)
                    {
                        currentIndex++;
                    }
                    _currentIndex = currentIndex;
                    return currentIndex < size && keyValueTable[currentIndex].Value != 0;
                }
            }

            /// <summary>
            /// Reset the enumerator
            /// </summary>
            public void Reset()
            {
                _currentIndex = 0xFFFFFFFF;
            }
        }

        /// <summary>
        /// Enumerable for elements
        /// </summary>
        public struct ValuesEnumerable
        {
            private Hashtable _hashtable;

            internal ValuesEnumerable(
              Hashtable hashtable
            )
            {
                _hashtable = hashtable;
            }

            /// <summary>
            /// Get the enumerator
            /// </summary>
            /// <returns></returns>
            public ValuesEnumerator GetEnumerator()
            {
                return new ValuesEnumerator(_hashtable);
            }
        }

        /// <summary>
        /// Enumerable of all the values
        /// </summary>
        public ValuesEnumerable Values
        {
            get
            {
                return new ValuesEnumerable(this);
            }
        }
    }

    /// <summary>
    /// Hashtable that has two uints as its key. Its value is also uint
    /// </summary>
    public sealed class DoubleHashtable
    {
        private struct KeyValuePair
        {
            internal uint Key1;
            internal uint Key2;
            internal uint Value;
        }

        private KeyValuePair[] _keyValueTable;
        private uint _size;
        private uint _resizeCount;
        private uint _count;
        private const int LoadPercent = 60;
        // ^ invariant (this.Size&(this.Size-1)) == 0;

        private static uint SizeFromExpectedEntries(uint expectedEntries)
        {
            uint expectedSize = (uint)(expectedEntries * 10) / 6; ;
            uint initialSize = 16;
            while (initialSize < expectedSize && initialSize > 0) initialSize <<= 1;
            return initialSize;
        }

        /// <summary>
        /// Constructor for DoubleHashtable
        /// </summary>
        public DoubleHashtable()
          : this(16)
        {
        }

        /// <summary>
        /// Constructor for DoubleHashtable
        /// </summary>
        public DoubleHashtable(uint expectedEntries)
        {
            _size = SizeFromExpectedEntries(expectedEntries);
            _resizeCount = _size * 6 / 10;
            _keyValueTable = new KeyValuePair[_size];
        }

        /// <summary>
        /// Count of elements
        /// </summary>
        public uint Count
        {
            get
            {
                return _count;
            }
        }

        private void Expand()
        {
            KeyValuePair[] oldKeyValueTable = _keyValueTable;
            _size <<= 1;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
            _resizeCount = _size * 6 / 10;
            int len = oldKeyValueTable.Length;
            for (int i = 0; i < len; ++i)
            {
                uint key1 = oldKeyValueTable[i].Key1;
                uint key2 = oldKeyValueTable[i].Key2;
                uint value = oldKeyValueTable[i].Value;
                if (value != 0)
                {
                    bool ret = this.AddInternal(key1, key2, value);
                    Debug.Assert(ret);
                }
            }
        }

        private bool AddInternal(
          uint key1,
          uint key2,
          uint value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashDoubleInt1(key1, key2);
                uint hash2 = HashHelper.HashDoubleInt2(key1, key2);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != 0)
                {
                    if (_keyValueTable[tableIndex].Key1 == key1 && _keyValueTable[tableIndex].Key2 == key2)
                    {
                        return false;
                    }
                    tableIndex = (tableIndex + hash2) & mask;
                }
                _keyValueTable[tableIndex].Key1 = key1;
                _keyValueTable[tableIndex].Key2 = key2;
                _keyValueTable[tableIndex].Value = value;
                _count++;
                return true;
            }
        }

        /// <summary>
        /// Add element to the Hashtable
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(
          uint key1,
          uint key2,
          uint value
        )
        {
            if (_count >= _resizeCount)
            {
                this.Expand();
            }
            return this.AddInternal(key1, key2, value);
        }

        /// <summary>
        /// Fine element in the Hashtable
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public uint Find(
          uint key1,
          uint key2
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashDoubleInt1(key1, key2);
                uint hash2 = HashHelper.HashDoubleInt2(key1, key2);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != 0)
                {
                    if (_keyValueTable[tableIndex].Key1 == key1 && _keyValueTable[tableIndex].Key2 == key2)
                        return _keyValueTable[tableIndex].Value;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                return 0;
            }
        }
    }

    /// <summary>
    /// Hashtable that has two uints as its key.
    /// </summary>
    public sealed class DoubleHashtable<T> where T : class
    {
        private struct KeyValuePair
        {
            internal uint Key1;
            internal uint Key2;
            internal T Value;
        }

        private KeyValuePair[] _keyValueTable;
        private uint _size;
        private uint _resizeCount;
        private uint _count;
        private const int LoadPercent = 60;
        // ^ invariant (this.Size&(this.Size-1)) == 0;

        private static uint SizeFromExpectedEntries(uint expectedEntries)
        {
            uint expectedSize = (uint)(expectedEntries * 10) / 6; ;
            uint initialSize = 16;
            while (initialSize < expectedSize && initialSize > 0) initialSize <<= 1;
            return initialSize;
        }

        /// <summary>
        /// Constructor for DoubleHashtable
        /// </summary>
        public DoubleHashtable()
          : this(16)
        {
        }

        /// <summary>
        /// Constructor for DoubleHashtable
        /// </summary>
        public DoubleHashtable(uint expectedEntries)
        {
            _size = SizeFromExpectedEntries(expectedEntries);
            _resizeCount = _size * 6 / 10;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
        }

        /// <summary>
        /// Count of elements
        /// </summary>
        public uint Count
        {
            get
            {
                return _count;
            }
        }

        private void Expand()
        {
            KeyValuePair[] oldKeyValueTable = _keyValueTable;
            _size <<= 1;
            _keyValueTable = new KeyValuePair[_size];
            _count = 0;
            _resizeCount = _size * 6 / 10;
            int len = oldKeyValueTable.Length;
            for (int i = 0; i < len; ++i)
            {
                uint key1 = oldKeyValueTable[i].Key1;
                uint key2 = oldKeyValueTable[i].Key2;
                T value = oldKeyValueTable[i].Value;
                if (value != null)
                {
                    bool ret = this.AddInternal(key1, key2, value);
                    Debug.Assert(ret);
                }
            }
        }

        private bool AddInternal(
          uint key1,
          uint key2,
          T value
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashDoubleInt1(key1, key2);
                uint hash2 = HashHelper.HashDoubleInt2(key1, key2);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key1 == key1 && _keyValueTable[tableIndex].Key2 == key2)
                    {
                        return false;
                    }
                    tableIndex = (tableIndex + hash2) & mask;
                }
                _keyValueTable[tableIndex].Key1 = key1;
                _keyValueTable[tableIndex].Key2 = key2;
                _keyValueTable[tableIndex].Value = value;
                _count++;
                return true;
            }
        }

        /// <summary>
        /// Add element to the DoubleHashtable
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(
          uint key1,
          uint key2,
          T value
        )
        {
            if (_count >= _resizeCount)
            {
                this.Expand();
            }
            return this.AddInternal(key1, key2, value);
        }

        /// <summary>
        /// Find element in DoubleHashtable
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns></returns>
        public T/*?*/ Find(
          uint key1,
          uint key2
        )
        {
            unchecked
            {
                uint hash1 = HashHelper.HashDoubleInt1(key1, key2);
                uint hash2 = HashHelper.HashDoubleInt2(key1, key2);
                uint mask = _size - 1;
                uint tableIndex = hash1 & mask;
                while (_keyValueTable[tableIndex].Value != null)
                {
                    if (_keyValueTable[tableIndex].Key1 == key1 && _keyValueTable[tableIndex].Key2 == key2)
                        return _keyValueTable[tableIndex].Value;
                    tableIndex = (tableIndex + hash2) & mask;
                }
                return null;
            }
        }
    }
}