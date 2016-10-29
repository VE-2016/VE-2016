//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci
{
#if !COMPACTFX

    /// <summary>
    /// Class representing the unmanaged memory mapped file. This can be used to open the file as memory mapped file and get the pointer to the buffer of file content.
    /// </summary>
    public unsafe sealed class MemoryMappedFile : IBinaryDocumentMemoryBlock
    {
        private MemoryMappedFile(
          IBinaryDocument binaryDocument,
          byte* buffer,
          uint length
        )
        {
            _binaryDocument = binaryDocument;
            _buffer = buffer;
            _length = length;
        }

        /// <summary>
        /// Finalizer for the Memory mapped file. Calls the CloseMap.
        /// </summary>
        ~MemoryMappedFile()
        {
            this.CloseMap();
        }

        private void CloseMap()
        {
            if (_buffer != null)
            {
                MemoryMappedFile.UnmapViewOfFile(_buffer);
                _buffer = null;
            }
        }

        #region IBinaryDocumentMemoryBlock Members

        byte* IBinaryDocumentMemoryBlock.Pointer
        {
            get { return _buffer; }
        }

        private byte* _buffer;

        uint IBinaryDocumentMemoryBlock.Length
        {
            get { return _length; }
        }

        private uint _length;

        IBinaryDocument IBinaryDocumentMemoryBlock.BinaryDocument
        {
            get { return _binaryDocument; }
        }

        private IBinaryDocument _binaryDocument;

        #endregion IBinaryDocumentMemoryBlock Members

        /// <summary>
        /// Factory method for opening the memory mapped file. The content of the map is assumed to come from localFileName.
        /// This can throw FileLoadException in case of error.
        /// </summary>
        /// <param name="localFileName">Name of the file from where the binary document needs to be opened.
        /// This is useful in case we want to copy the file to temporary location and then open or when we want to open document on the network.</param>
        /// <param name="binaryDocument">The binary document for which the memory mapping is requested.</param>
        public static MemoryMappedFile CreateMemoryMappedFile(
          string localFileName,
          IBinaryDocument binaryDocument
        )
        {
            uint length;
            Byte* buffer;
            MemoryMappedFile.OpenFileMemoryMap(localFileName, out buffer, out length);
            if (length != binaryDocument.Length)
                throw new IOException("File size difference: " + localFileName);
            return new MemoryMappedFile(
              binaryDocument,
              buffer,
              length
            );
        }

        private static void OpenFileMemoryMap(string filename, out Byte* buffer, out uint length)
        {
            IntPtr hmap;
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length > Int32.MaxValue)
                    throw new IOException("File too Big: " + filename);
                length = unchecked((uint)stream.Length);
                Microsoft.Win32.SafeHandles.SafeFileHandle/*?*/ safeHandle = stream.SafeFileHandle;
                if (safeHandle == null)
                {
                    throw new IOException("Unable to create Memory map: " + filename);
                }
                hmap = MemoryMappedFile.CreateFileMapping(safeHandle.DangerousGetHandle(), IntPtr.Zero, PageAccess.PAGE_READONLY, 0, length, null);
                if (hmap == IntPtr.Zero)
                {
                    int rc = Marshal.GetLastWin32Error();
                    throw new IOException("Unable to create Memory map: " + filename + " - " + rc.ToString("X"));
                }
            }
            buffer = (byte*)MemoryMappedFile.MapViewOfFile(hmap, FileMapAccess.FILE_MAP_READ, 0, 0, (IntPtr)length);
            MemoryMappedFile.CloseHandle(hmap);
            if (buffer == null)
            {
                int rc = Marshal.GetLastWin32Error();
                throw new IOException("Unable to create Memory map: " + filename + " - " + rc.ToString("X"));
            }
        }

        #region Interop stuff

        private enum PageAccess : int { PAGE_READONLY = 0x02 };

        private enum FileMapAccess : int { FILE_MAP_READ = 0x0004 };

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFileMapping(
          IntPtr hFile,           // handle to file
          IntPtr lpAttributes,    // security
          PageAccess flProtect,   // protection
          uint dwMaximumSizeHigh,  // high-order DWORD of size
          uint dwMaximumSizeLow,   // low-order DWORD of size
          string/*?*/ lpName           // object name
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void* MapViewOfFile(
          IntPtr hFileMappingObject,      // handle to file-mapping object
          FileMapAccess dwDesiredAccess,  // access mode
          int dwFileOffsetHigh,           // high-order DWORD of offset
          int dwFileOffsetLow,            // low-order DWORD of offset
          IntPtr dwNumberOfBytesToMap        // number of bytes to map
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnmapViewOfFile(
          void* lpBaseAddress // starting address
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(
          IntPtr hObject  // handle to object
        );

        #endregion Interop stuff
    }

#endif

    /// <summary>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    public unsafe sealed class UnmanagedBinaryMemoryBlock : IBinaryDocumentMemoryBlock
    { //TODO: implement IDisposable
        private IBinaryDocument _binaryDocument;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr _pointer;

        internal UnmanagedBinaryMemoryBlock(IBinaryDocument binaryDocument)
        {
            _binaryDocument = binaryDocument;
            _pointer = Marshal.AllocHGlobal((int)binaryDocument.Length);
            if (_pointer == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }
        }

        /// <summary>
        /// Destructor for UnmanagedBinaryMemoryBlock
        /// </summary>
        ~UnmanagedBinaryMemoryBlock()
        {
            if (_pointer != IntPtr.Zero)
                Marshal.FreeHGlobal(_pointer);
            _pointer = IntPtr.Zero;
        }

        #region IBinaryDocumentMemoryBlock Members

        IBinaryDocument IBinaryDocumentMemoryBlock.BinaryDocument
        {
            get { return _binaryDocument; }
        }

        byte* IBinaryDocumentMemoryBlock.Pointer
        {
            get { return (byte*)_pointer; }
        }

        uint IBinaryDocumentMemoryBlock.Length
        {
            get { return _binaryDocument.Length; }
        }

        #endregion IBinaryDocumentMemoryBlock Members

        /// <summary>
        /// Factory method for opening the memory mapped file. The content of the map is assumed to come from localFileName.
        /// This can throw FileLoadException in case of error.
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="binaryDocument"></param>
        /// <returns></returns>
        public static UnmanagedBinaryMemoryBlock CreateUnmanagedBinaryMemoryBlock(
          string localFileName,
          IBinaryDocument binaryDocument
        )
        {
            using (FileStream stream = new FileStream(localFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length != binaryDocument.Length)
                    throw new IOException("File size difference: " + localFileName);
                if (stream.Length > Int32.MaxValue)
                    throw new IOException("File too Big: " + localFileName);
                UnmanagedBinaryMemoryBlock unmanagedBinaryMemoryBlock = new UnmanagedBinaryMemoryBlock(binaryDocument);
                byte* pMainBuffer = (byte*)unmanagedBinaryMemoryBlock._pointer;
                //Read a fixed length block at a time, so that the GC does not come under pressure from lots of large byte arrays.
                int fileRemainingLen = (int)binaryDocument.Length;
                int copyBufferLen = 8096;
                byte[] tempBuffer = new byte[copyBufferLen];
                fixed (byte* tempBufferPtr = tempBuffer)
                {
                    while (fileRemainingLen > 0)
                    {
                        if (fileRemainingLen < copyBufferLen)
                        {
                            copyBufferLen = fileRemainingLen;
                        }
                        stream.Read(tempBuffer, 0, copyBufferLen);
                        byte* iterBuffer = tempBufferPtr;
                        byte* endBuffer = tempBufferPtr + copyBufferLen;
                        while (iterBuffer < endBuffer)
                        {
                            *pMainBuffer++ = *iterBuffer++;
                        }
                        fileRemainingLen -= copyBufferLen;
                    }
                }
                return unmanagedBinaryMemoryBlock;
            }
        }
    }

    /// <summary>
    /// Class representing a binary document
    /// </summary>
    public sealed class BinaryDocument : IBinaryDocument
    {
        /// <summary>
        /// Constructor for the Binay Document.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="name"></param>
        /// <param name="length"></param>
        public BinaryDocument(
          string location,
          IName name,
          uint length
        )
        //^ requires length >= 0;
        {
            _location = location;
            _name = name;
            _length = length;
        }

        #region IBinaryDocument Members

        uint IBinaryDocument.Length
        {
            get { return _length; }
        }

        private uint _length;

        #endregion IBinaryDocument Members

        #region IDocument Members

        string IDocument.Location
        {
            get { return _location; }
        }

        private string _location;

        IName IDocument.Name
        {
            get { return _name; }
        }

        private IName _name;

        #endregion IDocument Members

        /// <summary>
        /// Static factory method for getting the Binary document given full file path. Note this reads the file on the disk hence can throw some of the IO exceptions.
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <param name="compilationHost"></param>
        public static BinaryDocument GetBinaryDocumentForFile(string fullFilePath, IMetadataHost compilationHost)
        {
            IName name = compilationHost.NameTable.GetNameFor(Path.GetFileName(fullFilePath));
            FileInfo fileInfo = new FileInfo(fullFilePath);
            uint length = 0;
            if (fileInfo.Exists)
            {
                //TODO: error if file too large
                length = (uint)fileInfo.Length;
            }
            return new BinaryDocument(fullFilePath, name, length);
        }
    }

    /// <summary>
    /// Class representing the Binary location.
    /// </summary>
    public sealed class BinaryLocation : IBinaryLocation
    {
        //^ [SpecPublic]
        private IBinaryDocument _binaryDocument;

        private uint _offset;
        //^ invariant offset >= 0 && offset <= binaryDocument.Length;

        /// <summary>
        /// Constructor for the Binary location
        /// </summary>
        /// <param name="binaryDocument"></param>
        /// <param name="offset"></param>
        public BinaryLocation(
          IBinaryDocument binaryDocument,
          uint offset
        )
        {
            _binaryDocument = binaryDocument;
            _offset = offset;
        }

        #region IBinaryLocation Members

        IBinaryDocument IBinaryLocation.BinaryDocument
        {
            get
            //^ ensures result == this.binaryDocument;
            {
                return _binaryDocument;
            }
        }

        uint IBinaryLocation.Offset
        {
            get
            {
                //^ assume ((IBinaryLocation)this).BinaryDocument == this.binaryDocument; //see above
                return _offset;
            }
        }

        #endregion IBinaryLocation Members

        #region ILocation Members

        IDocument ILocation.Document
        {
            get { return _binaryDocument; }
        }

        #endregion ILocation Members

        /// <summary>
        /// Compares the equality of two locations.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override bool Equals(object/*?*/ obj)
        {
            BinaryLocation/*?*/ binaryLocation = obj as BinaryLocation;
            if (binaryLocation == null)
                return false;
            if (_offset != binaryLocation._offset)
                return false;
            return _binaryDocument.Location.Equals(binaryLocation._binaryDocument.Location);
        }

        /// <summary>
        /// Gives the hash code of the location
        /// </summary>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override int GetHashCode()
        {
            return _offset.GetHashCode();
        }

        /// <summary>
        /// Gives the string representing the location
        /// </summary>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "BinaryLocation({0},{1})", _binaryDocument.Location, _offset);
            return sb.ToString();
        }
    }

    /// <summary>
    /// Class representing the location in IL stream.
    /// </summary>
    public sealed class ILLocation : IILLocation
    {
        private readonly IBinaryDocument _binaryDocument;
        private readonly IMethodDefinition _methodDefinition;
        private readonly uint _offset;

        /// <summary>
        /// Constructor for IL location
        /// </summary>
        /// <param name="binaryDocument"></param>
        /// <param name="methodDefinition"></param>
        /// <param name="offset"></param>
        public ILLocation(
          IBinaryDocument binaryDocument,
          IMethodDefinition methodDefinition,
          uint offset
        )
        {
            _binaryDocument = binaryDocument;
            _methodDefinition = methodDefinition;
            _offset = offset;
        }

        #region IILLocation Members

        IMethodDefinition IILLocation.MethodDefinition
        {
            get { return _methodDefinition; }
        }

        uint IILLocation.Offset
        {
            get { return _offset; }
        }

        #endregion IILLocation Members

        #region ILocation Members

        IDocument ILocation.Document
        {
            get { return _binaryDocument; }
        }

        #endregion ILocation Members

        /// <summary>
        /// Compares the equality of two locations.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override bool Equals(object/*?*/ obj)
        {
            ILLocation/*?*/ ilLocation = obj as ILLocation;
            if (ilLocation == null)
                return false;
            if (_offset != ilLocation._offset)
                return false;
            if (_methodDefinition.Equals(ilLocation._methodDefinition))
                return false;
            return _binaryDocument.Location.Equals(ilLocation._binaryDocument.Location);
        }

        /// <summary>
        /// Gives the hash code of the location
        /// </summary>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override int GetHashCode()
        {
            return _offset.GetHashCode() ^ _methodDefinition.GetHashCode();
        }

        /// <summary>
        /// Gives the string representing the location
        /// </summary>
        /// <returns></returns>
        //^ [Confined, MustOverride]
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(CultureInfo.InvariantCulture, "ILLocation({0},0x{1})", _methodDefinition.ToString(), _offset.ToString("X8", CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}