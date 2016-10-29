//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using System.Collections.Generic;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.UtilityDataStructures
{
    internal sealed class EnumerableArrayWrapper<T> : IEnumerable<T>
      where T : struct
    {
        internal readonly T[] RawArray;

        internal EnumerableArrayWrapper(
          T[] rawArray
        )
        {
            this.RawArray = rawArray;
        }

        internal struct ArrayEnumerator : IEnumerator<T>
        {
            private T[] _rawArray;
            private int _currentIndex;

            public ArrayEnumerator(
              T[] rawArray
            )
            {
                _rawArray = rawArray;
                _currentIndex = -1;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get
                {
                    return _rawArray[_currentIndex];
                }
            }

            #endregion IEnumerator<T> Members

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion IDisposable Members

            #region IEnumerator Members

            //^ [Confined]
            object/*?*/ System.Collections.IEnumerator.Current
            {
                get
                {
                    return _rawArray[_currentIndex];
                }
            }

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _rawArray.Length;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            #endregion IEnumerator Members
        }

        #region IEnumerable<T> Members

        //^ [Pure]
        public IEnumerator<T> GetEnumerator()
        {
            return new ArrayEnumerator(this.RawArray);
        }

        #endregion IEnumerable<T> Members

        #region IEnumerable Members

        //^ [Pure]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ArrayEnumerator(this.RawArray);
        }

        #endregion IEnumerable Members
    }

    internal sealed class EnumerableArrayWrapper<T, U> : IEnumerable<U>
      where T : class, U
      where U : class
    {
        internal readonly T[] RawArray;
        internal readonly U DummyValue;

        internal EnumerableArrayWrapper(
          T[] rawArray,
          U dummyValue
        )
        {
            this.RawArray = rawArray;
            this.DummyValue = dummyValue;
        }

        internal struct ArrayEnumerator : IEnumerator<U>
        {
            private T[] _rawArray;
            private int _currentIndex;
            private U _dummyValue;

            public ArrayEnumerator(
              T[] rawArray,
              U dummyValue
            )
            {
                _rawArray = rawArray;
                _currentIndex = -1;
                _dummyValue = dummyValue;
            }

            #region IEnumerator<U> Members

            public U Current
            {
                get
                {
                    U retValue = _rawArray[_currentIndex];
                    return retValue == null ? _dummyValue : retValue;
                }
            }

            #endregion IEnumerator<U> Members

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion IDisposable Members

            #region IEnumerator Members

            //^ [Confined]
            object/*?*/ System.Collections.IEnumerator.Current
            {
                get
                {
                    U retValue = _rawArray[_currentIndex];
                    return retValue == null ? _dummyValue : retValue;
                }
            }

            public bool MoveNext()
            {
                _currentIndex++;
                return _currentIndex < _rawArray.Length;
            }

            public void Reset()
            {
                _currentIndex = -1;
            }

            #endregion IEnumerator Members
        }

        #region IEnumerable<U> Members

        //^ [Pure]
        public IEnumerator<U> GetEnumerator()
        {
            return new ArrayEnumerator(this.RawArray, this.DummyValue);
        }

        #endregion IEnumerable<U> Members

        #region IEnumerable Members

        //^ [Pure]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ArrayEnumerator(this.RawArray, this.DummyValue);
        }

        #endregion IEnumerable Members
    }

    internal sealed class EnumberableMemoryBlockWrapper : IEnumerable<byte>
    {
        internal readonly MemoryBlock MemBlock;

        internal EnumberableMemoryBlockWrapper(
          MemoryBlock memBlock
        )
        {
            this.MemBlock = memBlock;
        }

        internal unsafe struct MemoryBlockEnumerator : IEnumerator<byte>
        {
            private MemoryBlock _memBlock;
            private int _currentOffset;

            internal MemoryBlockEnumerator(
              MemoryBlock memBlock
            )
            {
                _memBlock = memBlock;
                _currentOffset = -1;
            }

            #region IEnumerator<byte> Members

            public byte Current
            {
                get
                {
                    return *(_memBlock.Buffer + _currentOffset);
                }
            }

            #endregion IEnumerator<byte> Members

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion IDisposable Members

            #region IEnumerator Members

            //^ [Confined]
            object/*?*/ System.Collections.IEnumerator.Current
            {
                get
                {
                    return *(_memBlock.Buffer + _currentOffset);
                }
            }

            public bool MoveNext()
            {
                _currentOffset++;
                return _currentOffset < _memBlock.Length;
            }

            public void Reset()
            {
                _currentOffset = -1;
            }

            #endregion IEnumerator Members
        }

        #region IEnumerable<byte> Members

        //^ [Pure]
        public IEnumerator<byte> GetEnumerator()
        {
            return new MemoryBlockEnumerator(this.MemBlock);
        }

        #endregion IEnumerable<byte> Members

        #region IEnumerable Members

        //^ [Pure]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MemoryBlockEnumerator(this.MemBlock);
        }

        #endregion IEnumerable Members
    }

    internal sealed class EnumerableBinaryDocumentMemoryBlockWrapper : IEnumerable<byte>
    {
        internal readonly IBinaryDocumentMemoryBlock BinaryDocumentMemoryBlock;

        internal EnumerableBinaryDocumentMemoryBlockWrapper(
          IBinaryDocumentMemoryBlock binaryDocumentMemoryBlock
        )
        {
            this.BinaryDocumentMemoryBlock = binaryDocumentMemoryBlock;
        }

        internal unsafe struct MemoryBlockEnumerator : IEnumerator<byte>
        {
            private IBinaryDocumentMemoryBlock _binaryDocumentMemoryBlock;
            private byte* _pointer;
            private int _length;
            private int _currentOffset;

            internal MemoryBlockEnumerator(
              IBinaryDocumentMemoryBlock binaryDocumentMemoryBlock
            )
            {
                _binaryDocumentMemoryBlock = binaryDocumentMemoryBlock;
                _pointer = binaryDocumentMemoryBlock.Pointer;
                _length = (int)binaryDocumentMemoryBlock.Length;
                _currentOffset = -1;
            }

            #region IEnumerator<byte> Members

            public byte Current
            {
                get
                {
                    return *(_pointer + _currentOffset);
                }
            }

            #endregion IEnumerator<byte> Members

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion IDisposable Members

            #region IEnumerator Members

            //^ [Confined]
            object/*?*/ System.Collections.IEnumerator.Current
            {
                get
                {
                    return *(_pointer + _currentOffset);
                }
            }

            public bool MoveNext()
            {
                _currentOffset++;
                return _currentOffset < _length;
            }

            public void Reset()
            {
                _currentOffset = -1;
            }

            #endregion IEnumerator Members
        }

        #region IEnumerable<byte> Members

        //^ [Pure]
        public IEnumerator<byte> GetEnumerator()
        {
            return new MemoryBlockEnumerator(this.BinaryDocumentMemoryBlock);
        }

        #endregion IEnumerable<byte> Members

        #region IEnumerable Members

        //^ [Pure]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new MemoryBlockEnumerator(this.BinaryDocumentMemoryBlock);
        }

        #endregion IEnumerable Members
    }
}