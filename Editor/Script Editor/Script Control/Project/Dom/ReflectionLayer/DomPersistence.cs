// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2492 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// This class can write Dom entity into a binary file for fast loading.
    /// </summary>
    public sealed class DomPersistence
    {
        public const long FileMagic = 0x11635233ED2F428C;
        public const long IndexFileMagic = 0x11635233ED2F427D;
        public const short FileVersion = 11;

        private ProjectContentRegistry _registry;
        private string _cacheDirectory;

        internal string CacheDirectory
        {
            get
            {
                return _cacheDirectory;
            }
        }

        internal DomPersistence(string cacheDirectory, ProjectContentRegistry registry)
        {
            _cacheDirectory = cacheDirectory;
            _registry = registry;

            _cacheIndex = LoadCacheIndex();
        }

        #region Cache management
        public string SaveProjectContent(ReflectionProjectContent pc)
        {
            string assemblyFullName = pc.AssemblyFullName;
            int pos = assemblyFullName.IndexOf(',');
            string fileName = Path.Combine(_cacheDirectory,
                                           assemblyFullName.Substring(0, pos)
                                           + "." + assemblyFullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture)
                                           + "." + pc.AssemblyLocation.GetHashCode().ToString("x", CultureInfo.InvariantCulture)
                                           + ".dat");
            AddFileNameToCacheIndex(Path.GetFileName(fileName), pc);
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                WriteProjectContent(pc, fs);
            }
            return fileName;
        }

        public ReflectionProjectContent LoadProjectContentByAssemblyName(string assemblyName)
        {
            string cacheFileName;
            if (CacheIndex.TryGetValue(assemblyName, out cacheFileName))
            {
                cacheFileName = Path.Combine(_cacheDirectory, cacheFileName);
                if (File.Exists(cacheFileName))
                {
                    return LoadProjectContent(cacheFileName);
                }
            }
            return null;
        }

        public ReflectionProjectContent LoadProjectContent(string cacheFileName)
        {
            using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read))
            {
                return LoadProjectContent(fs);
            }
        }
        #endregion

        #region Cache index
        private string GetIndexFileName() { return Path.Combine(_cacheDirectory, "index.dat"); }

        private Dictionary<string, string> _cacheIndex;

        private Dictionary<string, string> CacheIndex
        {
            get
            {
                return _cacheIndex;
            }
        }

        private Dictionary<string, string> LoadCacheIndex()
        {
            string indexFile = GetIndexFileName();
            Dictionary<string, string> list = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (File.Exists(indexFile))
            {
                try
                {
                    using (FileStream fs = new FileStream(indexFile, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader reader = new BinaryReader(fs))
                        {
                            if (reader.ReadInt64() != IndexFileMagic)
                            {
                                LoggingService.Warn("Index cache has wrong file magic");
                                return list;
                            }
                            if (reader.ReadInt16() != FileVersion)
                            {
                                LoggingService.Warn("Index cache has wrong file version");
                                return list;
                            }
                            int count = reader.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                string key = reader.ReadString();
                                list[key] = reader.ReadString();
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    LoggingService.Warn("Error reading DomPersistance cache index", ex);
                }
            }
            return list;
        }

        private void SaveCacheIndex(Dictionary<string, string> cacheIndex)
        {
            string indexFile = GetIndexFileName();
            using (FileStream fs = new FileStream(indexFile, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(IndexFileMagic);
                    writer.Write(FileVersion);
                    writer.Write(cacheIndex.Count);
                    foreach (KeyValuePair<string, string> e in cacheIndex)
                    {
                        writer.Write(e.Key);
                        writer.Write(e.Value);
                    }
                }
            }
        }

        private void AddFileNameToCacheIndex(string cacheFile, ReflectionProjectContent pc)
        {
            Dictionary<string, string> l = LoadCacheIndex();
            l[pc.AssemblyLocation] = cacheFile;
            string txt = pc.AssemblyFullName;
            l[txt] = cacheFile;
            int pos = txt.LastIndexOf(',');
            do
            {
                txt = txt.Substring(0, pos);
                if (l.ContainsKey(txt))
                    break;
                l[txt] = cacheFile;
                pos = txt.LastIndexOf(',');
            } while (pos >= 0);
            SaveCacheIndex(l);
            _cacheIndex = l;
        }
        #endregion

        #region Saving / Loading without cache
        /// <summary>
        /// Saves the project content to the stream.
        /// </summary>
        public static void WriteProjectContent(ReflectionProjectContent pc, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            new ReadWriteHelper(writer).WriteProjectContent(pc);
            // do not close the stream
        }

        /// <summary>
        /// Load a project content from a stream.
        /// </summary>
        public ReflectionProjectContent LoadProjectContent(Stream stream)
        {
            return LoadProjectContent(stream, _registry);
        }

        public static ReflectionProjectContent LoadProjectContent(Stream stream, ProjectContentRegistry registry)
        {
            ReflectionProjectContent pc;
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                pc = new ReadWriteHelper(reader).ReadProjectContent(registry);
                if (pc != null)
                {
                    pc.InitializeSpecialClasses();
                }
                return pc;
            }
            catch (EndOfStreamException)
            {
                LoggingService.Warn("Read dom: EndOfStreamException");
                return null;
            }
            // do not close the stream
        }
        #endregion

        private struct ClassNameTypeCountPair
        {
            public readonly string ClassName;
            public readonly byte TypeParameterCount;

            public ClassNameTypeCountPair(IClass c)
            {
                this.ClassName = c.FullyQualifiedName;
                this.TypeParameterCount = (byte)c.TypeParameters.Count;
            }

            public ClassNameTypeCountPair(IReturnType rt)
            {
                this.ClassName = rt.FullyQualifiedName;
                this.TypeParameterCount = (byte)rt.TypeParameterCount;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ClassNameTypeCountPair)) return false;
                ClassNameTypeCountPair myClassNameTypeCountPair = (ClassNameTypeCountPair)obj;
                if (!ClassName.Equals(myClassNameTypeCountPair.ClassName, StringComparison.InvariantCultureIgnoreCase)) return false;
                if (TypeParameterCount != myClassNameTypeCountPair.TypeParameterCount) return false;
                return true;
            }

            public override int GetHashCode()
            {
                return StringComparer.InvariantCultureIgnoreCase.GetHashCode(ClassName) ^ ((int)TypeParameterCount * 5);
            }
        }

        private sealed class ReadWriteHelper
        {
            private ReflectionProjectContent _pc;

            // for writing:
            private readonly BinaryWriter _writer;
            private readonly Dictionary<ClassNameTypeCountPair, int> _classIndices = new Dictionary<ClassNameTypeCountPair, int>();
            private readonly Dictionary<string, int> _stringDict = new Dictionary<string, int>();

            // for reading:
            private readonly BinaryReader _reader;
            private IReturnType[] _types;
            private string[] _stringArray;

            #region Write/Read ProjectContent
            public ReadWriteHelper(BinaryWriter writer)
            {
                _writer = writer;
            }

            public void WriteProjectContent(ReflectionProjectContent pc)
            {
                _pc = pc;
                _writer.Write(FileMagic);
                _writer.Write(FileVersion);
                _writer.Write(pc.AssemblyFullName);
                _writer.Write(pc.AssemblyLocation);
                long time = 0;
                try
                {
                    time = File.GetLastWriteTimeUtc(pc.AssemblyLocation).ToFileTime();
                }
                catch { }
                _writer.Write(time);
                _writer.Write(pc.ReferencedAssemblyNames.Count);
                foreach (DomAssemblyName name in pc.ReferencedAssemblyNames)
                {
                    _writer.Write(name.FullName);
                }
                WriteClasses();
            }

            public ReadWriteHelper(BinaryReader reader)
            {
                _reader = reader;
            }

            public ReflectionProjectContent ReadProjectContent(ProjectContentRegistry registry)
            {
                if (_reader.ReadInt64() != FileMagic)
                {
                    LoggingService.Warn("Read dom: wrong magic");
                    return null;
                }
                if (_reader.ReadInt16() != FileVersion)
                {
                    LoggingService.Warn("Read dom: wrong version");
                    return null;
                }
                string assemblyName = _reader.ReadString();
                string assemblyLocation = _reader.ReadString();
                long time = 0;
                try
                {
                    time = File.GetLastWriteTimeUtc(assemblyLocation).ToFileTime();
                }
                catch { }
                if (_reader.ReadInt64() != time)
                {
                    LoggingService.Warn("Read dom: assembly changed since cache was created");
                    return null;
                }
                DomAssemblyName[] referencedAssemblies = new DomAssemblyName[_reader.ReadInt32()];
                for (int i = 0; i < referencedAssemblies.Length; i++)
                {
                    referencedAssemblies[i] = new DomAssemblyName(_reader.ReadString());
                }
                _pc = new ReflectionProjectContent(assemblyName, assemblyLocation, referencedAssemblies, registry);
                if (ReadClasses())
                {
                    return _pc;
                }
                else
                {
                    LoggingService.Warn("Read dom: error in file (invalid control mark)");
                    return null;
                }
            }

            private void WriteClasses()
            {
                ICollection<IClass> classes = _pc.Classes;

                _classIndices.Clear();
                _stringDict.Clear();
                int i = 0;
                foreach (IClass c in classes)
                {
                    _classIndices[new ClassNameTypeCountPair(c)] = i;
                    i += 1;
                }

                List<ClassNameTypeCountPair> externalTypes = new List<ClassNameTypeCountPair>();
                List<string> stringList = new List<string>();
                CreateExternalTypeList(externalTypes, stringList, classes.Count, classes);

                _writer.Write(classes.Count);
                _writer.Write(externalTypes.Count);
                foreach (IClass c in classes)
                {
                    _writer.Write(c.FullyQualifiedName);
                }
                foreach (ClassNameTypeCountPair type in externalTypes)
                {
                    _writer.Write(type.ClassName);
                    _writer.Write(type.TypeParameterCount);
                }
                _writer.Write(stringList.Count);
                foreach (string text in stringList)
                {
                    _writer.Write(text);
                }
                foreach (IClass c in classes)
                {
                    WriteClass(c);
                    // BinaryReader easily reads junk data when the file does not have the
                    // expected format, so we put a checking byte after each class.
                    _writer.Write((byte)64);
                }
            }

            private bool ReadClasses()
            {
                int classCount = _reader.ReadInt32();
                int externalTypeCount = _reader.ReadInt32();
                _types = new IReturnType[classCount + externalTypeCount];
                DefaultClass[] classes = new DefaultClass[classCount];
                for (int i = 0; i < classes.Length; i++)
                {
                    DefaultClass c = new DefaultClass(_pc.AssemblyCompilationUnit, _reader.ReadString());
                    classes[i] = c;
                    _types[i] = c.DefaultReturnType;
                }
                for (int i = classCount; i < _types.Length; i++)
                {
                    string name = _reader.ReadString();
                    _types[i] = new GetClassReturnType(_pc, name, _reader.ReadByte());
                }
                _stringArray = new string[_reader.ReadInt32()];
                for (int i = 0; i < _stringArray.Length; i++)
                {
                    _stringArray[i] = _reader.ReadString();
                }
                for (int i = 0; i < classes.Length; i++)
                {
                    ReadClass(classes[i]);
                    _pc.AddClassToNamespaceList(classes[i]);
                    if (_reader.ReadByte() != 64)
                    {
                        return false;
                    }
                }
                return true;
            }
            #endregion

            #region Write/Read Class
            private IClass _currentClass;

            private void WriteClass(IClass c)
            {
                _currentClass = c;
                WriteTemplates(c.TypeParameters);
                _writer.Write(c.BaseTypes.Count);
                foreach (IReturnType type in c.BaseTypes)
                {
                    WriteType(type);
                }
                _writer.Write((int)c.Modifiers);
                if (c is DefaultClass)
                {
                    _writer.Write(((DefaultClass)c).Flags);
                }
                else
                {
                    _writer.Write((byte)0);
                }
                _writer.Write((byte)c.ClassType);
                WriteAttributes(c.Attributes);
                _writer.Write(c.InnerClasses.Count);
                foreach (IClass innerClass in c.InnerClasses)
                {
                    _writer.Write(innerClass.FullyQualifiedName);
                    WriteClass(innerClass);
                }
                _currentClass = c;
                _writer.Write(c.Methods.Count);
                foreach (IMethod method in c.Methods)
                {
                    WriteMethod(method);
                }
                _writer.Write(c.Properties.Count);
                foreach (IProperty property in c.Properties)
                {
                    WriteProperty(property);
                }
                _writer.Write(c.Events.Count);
                foreach (IEvent evt in c.Events)
                {
                    WriteEvent(evt);
                }
                _writer.Write(c.Fields.Count);
                foreach (IField field in c.Fields)
                {
                    WriteField(field);
                }
                _currentClass = null;
            }

            private void WriteTemplates(IList<ITypeParameter> list)
            {
                // read code exists twice: in ReadClass and ReadMethod
                _writer.Write((byte)list.Count);
                foreach (ITypeParameter typeParameter in list)
                {
                    WriteString(typeParameter.Name);
                }
                foreach (ITypeParameter typeParameter in list)
                {
                    _writer.Write(typeParameter.Constraints.Count);
                    foreach (IReturnType type in typeParameter.Constraints)
                    {
                        WriteType(type);
                    }
                }
            }

            private void ReadClass(DefaultClass c)
            {
                _currentClass = c;
                int count;
                count = _reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    c.TypeParameters.Add(new DefaultTypeParameter(c, ReadString(), i));
                }
                if (count > 0)
                {
                    foreach (ITypeParameter typeParameter in c.TypeParameters)
                    {
                        count = _reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            typeParameter.Constraints.Add(ReadType());
                        }
                    }
                }
                else
                {
                    c.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
                }
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    c.BaseTypes.Add(ReadType());
                }
                c.Modifiers = (ModifierEnum)_reader.ReadInt32();
                c.Flags = _reader.ReadByte();
                c.ClassType = (ClassType)_reader.ReadByte();
                ReadAttributes(c);
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    DefaultClass innerClass = new DefaultClass(c.CompilationUnit, c);
                    innerClass.FullyQualifiedName = _reader.ReadString();
                    c.InnerClasses.Add(innerClass);
                    ReadClass(innerClass);
                }
                _currentClass = c;
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    c.Methods.Add(ReadMethod());
                }
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    c.Properties.Add(ReadProperty());
                }
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    c.Events.Add(ReadEvent());
                }
                count = _reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    c.Fields.Add(ReadField());
                }
                _currentClass = null;
            }
            #endregion

            #region Write/Read return types / Collect strings
            /// <summary>
            /// Finds all return types used in the class collection and adds the unknown ones
            /// to the externalTypeIndices and externalTypes collections.
            /// </summary>
            private void CreateExternalTypeList(List<ClassNameTypeCountPair> externalTypes,
                                        List<string> stringList,
                                        int classCount, ICollection<IClass> classes)
            {
                foreach (IClass c in classes)
                {
                    CreateExternalTypeList(externalTypes, stringList, classCount, c.InnerClasses);
                    AddStrings(stringList, c.Attributes);
                    foreach (IReturnType returnType in c.BaseTypes)
                    {
                        AddExternalType(returnType, externalTypes, classCount);
                    }
                    foreach (ITypeParameter tp in c.TypeParameters)
                    {
                        AddString(stringList, tp.Name);
                        foreach (IReturnType returnType in tp.Constraints)
                        {
                            AddExternalType(returnType, externalTypes, classCount);
                        }
                    }
                    foreach (IField f in c.Fields)
                    {
                        CreateExternalTypeListMember(externalTypes, stringList, classCount, f);
                    }
                    foreach (IEvent f in c.Events)
                    {
                        CreateExternalTypeListMember(externalTypes, stringList, classCount, f);
                    }
                    foreach (IProperty p in c.Properties)
                    {
                        CreateExternalTypeListMember(externalTypes, stringList, classCount, p);
                        foreach (IParameter parameter in p.Parameters)
                        {
                            AddString(stringList, parameter.Name);
                            AddStrings(stringList, parameter.Attributes);
                            AddExternalType(parameter.ReturnType, externalTypes, classCount);
                        }
                    }
                    foreach (IMethod m in c.Methods)
                    {
                        CreateExternalTypeListMember(externalTypes, stringList, classCount, m);
                        foreach (IParameter parameter in m.Parameters)
                        {
                            AddString(stringList, parameter.Name);
                            AddStrings(stringList, parameter.Attributes);
                            AddExternalType(parameter.ReturnType, externalTypes, classCount);
                        }
                        foreach (ITypeParameter tp in m.TypeParameters)
                        {
                            AddString(stringList, tp.Name);
                            foreach (IReturnType returnType in tp.Constraints)
                            {
                                AddExternalType(returnType, externalTypes, classCount);
                            }
                        }
                    }
                }
            }

            private void CreateExternalTypeListMember(List<ClassNameTypeCountPair> externalTypes,
                                              List<string> stringList, int classCount,
                                              IMember member)
            {
                AddString(stringList, member.Name);
                AddStrings(stringList, member.Attributes);
                foreach (ExplicitInterfaceImplementation eii in member.InterfaceImplementations)
                {
                    AddString(stringList, eii.MemberName);
                    AddExternalType(eii.InterfaceReference, externalTypes, classCount);
                }
                AddExternalType(member.ReturnType, externalTypes, classCount);
            }

            private void AddString(List<string> stringList, string text)
            {
                text = text ?? string.Empty;
                if (!_stringDict.ContainsKey(text))
                {
                    _stringDict.Add(text, stringList.Count);
                    stringList.Add(text);
                }
            }

            private void AddExternalType(IReturnType rt, List<ClassNameTypeCountPair> externalTypes, int classCount)
            {
                if (rt.IsDefaultReturnType)
                {
                    ClassNameTypeCountPair pair = new ClassNameTypeCountPair(rt);
                    if (!_classIndices.ContainsKey(pair))
                    {
                        _classIndices.Add(pair, externalTypes.Count + classCount);
                        externalTypes.Add(pair);
                    }
                }
                else if (rt.IsGenericReturnType)
                {
                    // ignore
                }
                else if (rt.IsArrayReturnType)
                {
                    AddExternalType(rt.CastToArrayReturnType().ArrayElementType, externalTypes, classCount);
                }
                else if (rt.IsConstructedReturnType)
                {
                    AddExternalType(rt.CastToConstructedReturnType().UnboundType, externalTypes, classCount);
                    foreach (IReturnType typeArgument in rt.CastToConstructedReturnType().TypeArguments)
                    {
                        AddExternalType(typeArgument, externalTypes, classCount);
                    }
                }
                else
                {
                    LoggingService.Warn("Unknown return type: " + rt.ToString());
                }
            }

            private const int ArrayRTCode = -1;
            private const int ConstructedRTCode = -2;
            private const int TypeGenericRTCode = -3;
            private const int MethodGenericRTCode = -4;
            private const int NullRTReferenceCode = -5;
            private const int VoidRTCode = -6;

            private void WriteType(IReturnType rt)
            {
                if (rt == null)
                {
                    _writer.Write(NullRTReferenceCode);
                    return;
                }
                if (rt.IsDefaultReturnType)
                {
                    string name = rt.FullyQualifiedName;
                    if (name == "System.Void")
                    {
                        _writer.Write(VoidRTCode);
                    }
                    else
                    {
                        _writer.Write(_classIndices[new ClassNameTypeCountPair(rt)]);
                    }
                }
                else if (rt.IsGenericReturnType)
                {
                    GenericReturnType grt = rt.CastToGenericReturnType();
                    if (grt.TypeParameter.Method != null)
                    {
                        _writer.Write(MethodGenericRTCode);
                    }
                    else
                    {
                        _writer.Write(TypeGenericRTCode);
                    }
                    _writer.Write(grt.TypeParameter.Index);
                }
                else if (rt.IsArrayReturnType)
                {
                    _writer.Write(ArrayRTCode);
                    _writer.Write(rt.CastToArrayReturnType().ArrayDimensions);
                    WriteType(rt.CastToArrayReturnType().ArrayElementType);
                }
                else if (rt.IsConstructedReturnType)
                {
                    ConstructedReturnType crt = rt.CastToConstructedReturnType();
                    _writer.Write(ConstructedRTCode);
                    WriteType(crt.UnboundType);
                    _writer.Write((byte)crt.TypeArguments.Count);
                    foreach (IReturnType typeArgument in crt.TypeArguments)
                    {
                        WriteType(typeArgument);
                    }
                }
                else
                {
                    _writer.Write(NullRTReferenceCode);
                    LoggingService.Warn("Unknown return type: " + rt.ToString());
                }
            }

            // outerClass and outerMethod are required for generic return types
            private IReturnType ReadType()
            {
                int index = _reader.ReadInt32();
                switch (index)
                {
                    case ArrayRTCode:
                        int dimensions = _reader.ReadInt32();
                        return new ArrayReturnType(_pc, ReadType(), dimensions);
                    case ConstructedRTCode:
                        IReturnType baseType = ReadType();
                        IReturnType[] typeArguments = new IReturnType[_reader.ReadByte()];
                        for (int i = 0; i < typeArguments.Length; i++)
                        {
                            typeArguments[i] = ReadType();
                        }
                        return new ConstructedReturnType(baseType, typeArguments);
                    case TypeGenericRTCode:
                        return new GenericReturnType(_currentClass.TypeParameters[_reader.ReadInt32()]);
                    case MethodGenericRTCode:
                        return new GenericReturnType(_currentMethod.TypeParameters[_reader.ReadInt32()]);
                    case NullRTReferenceCode:
                        return null;
                    case VoidRTCode:
                        return VoidReturnType.Instance;
                    default:
                        return _types[index];
                }
            }
            #endregion

            #region Write/Read class member
            private void WriteString(string text)
            {
                text = text ?? string.Empty;
                _writer.Write(_stringDict[text]);
            }

            private string ReadString()
            {
                return _stringArray[_reader.ReadInt32()];
            }

            private void WriteMember(IMember m)
            {
                WriteString(m.Name);
                _writer.Write((int)m.Modifiers);
                WriteAttributes(m.Attributes);
                _writer.Write((ushort)m.InterfaceImplementations.Count);
                foreach (ExplicitInterfaceImplementation iee in m.InterfaceImplementations)
                {
                    WriteType(iee.InterfaceReference);
                    WriteString(iee.MemberName);
                }
                if (!(m is IMethod))
                {
                    // method must store ReturnType AFTER Template definitions
                    WriteType(m.ReturnType);
                }
            }

            private void ReadMember(AbstractMember m)
            {
                // name is already read by the method that calls the member constructor
                m.Modifiers = (ModifierEnum)_reader.ReadInt32();
                ReadAttributes(m);
                int interfaceImplCount = _reader.ReadUInt16();
                for (int i = 0; i < interfaceImplCount; i++)
                {
                    m.InterfaceImplementations.Add(new ExplicitInterfaceImplementation(ReadType(), ReadString()));
                }
                if (!(m is IMethod))
                {
                    m.ReturnType = ReadType();
                }
            }
            #endregion

            #region Write/Read attributes
            private void WriteAttributes(IList<IAttribute> attributes)
            {
                _writer.Write((ushort)attributes.Count);
                foreach (IAttribute a in attributes)
                {
                    WriteString(a.Name);
                    _writer.Write((byte)a.AttributeTarget);
                }
            }

            private void AddStrings(List<string> stringList, IList<IAttribute> attributes)
            {
                foreach (IAttribute a in attributes)
                {
                    AddString(stringList, a.Name);
                }
            }

            private void ReadAttributes(DefaultParameter parameter)
            {
                int count = _reader.ReadUInt16();
                if (count > 0)
                {
                    ReadAttributes(parameter.Attributes, count);
                }
                else
                {
                    parameter.Attributes = DefaultAttribute.EmptyAttributeList;
                }
            }

            private void ReadAttributes(AbstractDecoration decoration)
            {
                int count = _reader.ReadUInt16();
                if (count > 0)
                {
                    ReadAttributes(decoration.Attributes, count);
                }
                else
                {
                    decoration.Attributes = DefaultAttribute.EmptyAttributeList;
                }
            }

            private void ReadAttributes(IList<IAttribute> attributes, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    string name = ReadString();
                    attributes.Add(new DefaultAttribute(name, (AttributeTarget)_reader.ReadByte()));
                }
            }
            #endregion

            #region Write/Read parameters
            private void WriteParameters(IList<IParameter> parameters)
            {
                _writer.Write((ushort)parameters.Count);
                foreach (IParameter p in parameters)
                {
                    WriteString(p.Name);
                    WriteType(p.ReturnType);
                    _writer.Write((byte)p.Modifiers);
                    WriteAttributes(p.Attributes);
                }
            }

            private void ReadParameters(DefaultMethod m)
            {
                int count = _reader.ReadUInt16();
                if (count > 0)
                {
                    ReadParameters(m.Parameters, count);
                }
                else
                {
                    m.Parameters = DefaultParameter.EmptyParameterList;
                }
            }

            private void ReadParameters(DefaultProperty m)
            {
                int count = _reader.ReadUInt16();
                if (count > 0)
                {
                    ReadParameters(m.Parameters, count);
                }
                else
                {
                    m.Parameters = DefaultParameter.EmptyParameterList;
                }
            }

            private void ReadParameters(IList<IParameter> parameters, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    string name = ReadString();
                    DefaultParameter p = new DefaultParameter(name, ReadType(), DomRegion.Empty);
                    p.Modifiers = (ParameterModifiers)_reader.ReadByte();
                    ReadAttributes(p);
                    parameters.Add(p);
                }
            }
            #endregion

            #region Write/Read Method
            private IMethod _currentMethod;

            private void WriteMethod(IMethod m)
            {
                _currentMethod = m;
                WriteMember(m);
                WriteTemplates(m.TypeParameters);
                WriteType(m.ReturnType);
                _writer.Write(m.IsExtensionMethod);
                WriteParameters(m.Parameters);
                _currentMethod = null;
            }

            private IMethod ReadMethod()
            {
                DefaultMethod m = new DefaultMethod(_currentClass, ReadString());
                _currentMethod = m;
                ReadMember(m);
                int count = _reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    m.TypeParameters.Add(new DefaultTypeParameter(m, ReadString(), i));
                }
                if (count > 0)
                {
                    foreach (ITypeParameter typeParameter in m.TypeParameters)
                    {
                        count = _reader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            typeParameter.Constraints.Add(ReadType());
                        }
                    }
                }
                else
                {
                    m.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
                }
                m.ReturnType = ReadType();
                m.IsExtensionMethod = _reader.ReadBoolean();
                ReadParameters(m);
                _currentMethod = null;
                return m;
            }
            #endregion

            #region Write/Read Property
            private void WriteProperty(IProperty p)
            {
                WriteMember(p);
                DefaultProperty dp = p as DefaultProperty;
                if (dp != null)
                {
                    _writer.Write(dp.accessFlags);
                }
                else
                {
                    _writer.Write((byte)0);
                }
                WriteParameters(p.Parameters);
            }

            private IProperty ReadProperty()
            {
                DefaultProperty p = new DefaultProperty(_currentClass, ReadString());
                ReadMember(p);
                p.accessFlags = _reader.ReadByte();
                ReadParameters(p);
                return p;
            }
            #endregion

            #region Write/Read Event
            private void WriteEvent(IEvent p)
            {
                WriteMember(p);
            }

            private IEvent ReadEvent()
            {
                DefaultEvent p = new DefaultEvent(_currentClass, ReadString());
                ReadMember(p);
                return p;
            }
            #endregion

            #region Write/Read Field
            private void WriteField(IField p)
            {
                WriteMember(p);
            }

            private IField ReadField()
            {
                DefaultField p = new DefaultField(_currentClass, ReadString());
                ReadMember(p);
                return p;
            }
            #endregion
        }
    }
}
