// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2611 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// Class capable of loading xml documentation files. XmlDoc automatically creates a
    /// binary cache for big xml files to reduce memory usage.
    /// </summary>
    public sealed class XmlDoc : IDisposable
    {
        private static readonly List<string> s_xmlDocLookupDirectories = new List<string>(
            new string[] { System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() }
        );

        public static IList<string> XmlDocLookupDirectories
        {
            get { return s_xmlDocLookupDirectories; }
        }

        private struct IndexEntry : IComparable<IndexEntry>
        {
            public int HashCode;
            public int FileLocation;

            public int CompareTo(IndexEntry other)
            {
                return HashCode.CompareTo(other.HashCode);
            }

            public IndexEntry(int HashCode, int FileLocation)
            {
                this.HashCode = HashCode;
                this.FileLocation = FileLocation;
            }
        }

        private Dictionary<string, string> _xmlDescription = new Dictionary<string, string>();
        private IndexEntry[] _index; // SORTED array of index entries
        private Queue<string> _keyCacheQueue;

        private const int cacheLength = 150; // number of strings to cache when working in file-mode

        private void ReadMembersSection(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "members")
                        {
                            return;
                        }
                        break;
                    case XmlNodeType.Element:
                        if (reader.LocalName == "member")
                        {
                            string memberAttr = reader.GetAttribute(0);
                            string innerXml = reader.ReadInnerXml();
                            _xmlDescription[memberAttr] = innerXml;
                        }
                        break;
                }
            }
        }

        public string GetDocumentation(string key)
        {
            if (_xmlDescription == null)
                throw new ObjectDisposedException("XmlDoc");
            lock (_xmlDescription)
            {
                string result;
                if (_xmlDescription.TryGetValue(key, out result))
                    return result;
                if (_index == null)
                    return null;
                return LoadDocumentation(key);
            }
        }

        #region Save binary files
        // FILE FORMAT FOR BINARY DOCUMENTATION
        // long  magic = 0x4244636f446c6d58 (identifies file type = 'XmlDocDB')
        private const long magic = 0x4244636f446c6d58;
        // short version = 2              (file version)
        private const short version = 2;
        // long  fileDate                 (last change date of xml file in DateTime ticks)
        // int   testHashCode = magicTestString.GetHashCode() // (check if hash-code implementation is compatible)
        // int   entryCount               (count of entries)
        // int   indexPointer             (points to location where index starts in the file)
        // {
        //   string key                   (documentation key as length-prefixed string)
        //   string docu                  (xml documentation as length-prefixed string)
        // }
        // indexPointer points to the start of the following section:
        // {
        //   int hashcode
        //   int    index           (index where the docu string starts in the file)
        // }

        private void Save(string fileName, DateTime fileDate)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    w.Write(magic);
                    w.Write(version);
                    w.Write(fileDate.Ticks);

                    IndexEntry[] index = new IndexEntry[_xmlDescription.Count];
                    w.Write(index.Length);

                    int indexPointerPos = (int)fs.Position;
                    w.Write(0); // skip 4 bytes

                    int i = 0;
                    foreach (KeyValuePair<string, string> p in _xmlDescription)
                    {
                        index[i] = new IndexEntry(p.Key.GetHashCode(), (int)fs.Position);
                        w.Write(p.Key);
                        w.Write(p.Value.Trim());
                        i += 1;
                    }

                    Array.Sort(index);

                    int indexStart = (int)fs.Position;
                    foreach (IndexEntry entry in index)
                    {
                        w.Write(entry.HashCode);
                        w.Write(entry.FileLocation);
                    }
                    w.Seek(indexPointerPos, SeekOrigin.Begin);
                    w.Write(indexStart);
                }
            }
        }
        #endregion

        #region Load binary files
        private BinaryReader _loader;
        private FileStream _fs;

        private bool LoadFromBinary(string fileName, DateTime fileDate)
        {
            _keyCacheQueue = new Queue<string>(cacheLength);
            _fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            int len = (int)_fs.Length;
            _loader = new BinaryReader(_fs);
            try
            {
                if (_loader.ReadInt64() != magic)
                {
                    LoggingService.Warn("Cannot load XmlDoc: wrong magic");
                    return false;
                }
                if (_loader.ReadInt16() != version)
                {
                    LoggingService.Warn("Cannot load XmlDoc: wrong version");
                    return false;
                }
                if (_loader.ReadInt64() != fileDate.Ticks)
                {
                    LoggingService.Info("Not loading XmlDoc: file changed since cache was created");
                    return false;
                }
                int count = _loader.ReadInt32();
                int indexStartPosition = _loader.ReadInt32(); // go to start of index
                if (indexStartPosition >= len)
                {
                    LoggingService.Error("XmlDoc: Cannot find index, cache invalid!");
                    return false;
                }
                _fs.Position = indexStartPosition;
                IndexEntry[] index = new IndexEntry[count];
                for (int i = 0; i < index.Length; i++)
                {
                    index[i] = new IndexEntry(_loader.ReadInt32(), _loader.ReadInt32());
                }
                _index = index;
                return true;
            }
            catch (Exception ex)
            {
                LoggingService.Error("Cannot load from cache", ex);
                return false;
            }
        }

        private string LoadDocumentation(string key)
        {
            if (_keyCacheQueue.Count > cacheLength - 1)
            {
                _xmlDescription.Remove(_keyCacheQueue.Dequeue());
            }

            int hashcode = key.GetHashCode();

            // use interpolation search to find the item
            string resultDocu = null;

            int m = Array.BinarySearch(_index, new IndexEntry(hashcode, 0));
            if (m >= 0)
            {
                // correct hash code found.
                // possibly there are multiple items with the same hash, so go to the first.
                while (--m >= 0 && _index[m].HashCode == hashcode) ;
                // go through all items that have the correct hash
                while (++m < _index.Length && _index[m].HashCode == hashcode)
                {
                    _fs.Position = _index[m].FileLocation;
                    string keyInFile = _loader.ReadString();
                    if (keyInFile == key)
                    {
                        resultDocu = _loader.ReadString();
                        break;
                    }
                    else
                    {
                        LoggingService.Warn("Found " + keyInFile + " instead of " + key);
                    }
                }
            }

            _keyCacheQueue.Enqueue(key);
            _xmlDescription.Add(key, resultDocu);

            return resultDocu;
        }

        public void Dispose()
        {
            if (_loader != null)
            {
                _loader.Close();
                _fs.Close();
            }
            _xmlDescription = null;
            _index = null;
            _keyCacheQueue = null;
            _loader = null;
            _fs = null;
        }
        #endregion

        public static XmlDoc Load(XmlReader reader)
        {
            XmlDoc newXmlDoc = new XmlDoc();
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "members":
                            newXmlDoc.ReadMembersSection(reader);
                            break;
                    }
                }
            }
            return newXmlDoc;
        }

        public static XmlDoc Load(string fileName, string cachePath)
        {
            LoggingService.Debug("Loading XmlDoc for " + fileName);
            Directory.CreateDirectory(cachePath);
            string cacheName = cachePath + "/" + Path.GetFileNameWithoutExtension(fileName)
                + "." + fileName.GetHashCode().ToString("x") + ".dat";
            XmlDoc doc;
            if (File.Exists(cacheName))
            {
                doc = new XmlDoc();
                if (doc.LoadFromBinary(cacheName, File.GetLastWriteTimeUtc(fileName)))
                {
                    LoggingService.Debug("XmlDoc: Load from cache successful");
                    return doc;
                }
                else
                {
                    doc.Dispose();
                    try
                    {
                        File.Delete(cacheName);
                    }
                    catch { }
                }
            }

            try
            {
                using (XmlTextReader xmlReader = new XmlTextReader(fileName))
                {
                    doc = Load(xmlReader);
                }
            }
            catch (XmlException ex)
            {
                LoggingService.Warn("Error loading XmlDoc", ex);
                return new XmlDoc();
            }

            if (doc._xmlDescription.Count > cacheLength * 2)
            {
                LoggingService.Debug("XmlDoc: Creating cache");
                DateTime date = File.GetLastWriteTimeUtc(fileName);
                try
                {
                    doc.Save(cacheName, date);
                }
                catch (Exception ex)
                {
                    LoggingService.Error("Cannot write to cache file", ex);
                    return doc;
                }
                doc.Dispose();
                doc = new XmlDoc();
                doc.LoadFromBinary(cacheName, date);
            }
            return doc;
        }
    }
}
