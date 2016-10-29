//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using Microsoft.Cci.MetadataReader.PEFileFlags;
using Microsoft.Cci.UtilityDataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci.MetadataReader.ObjectModelImplementation
{
    #region Base Objects for Object Model

    internal interface IModuleModuleReference : IModuleReference
    {
        uint InternedModuleId { get; }
    }

    internal interface IModuleMemberReference : ITypeMemberReference
    {
        IModuleTypeReference/*?*/ OwningTypeReference { get; }
    }

    internal interface IModuleFieldReference : IModuleMemberReference, IFieldReference
    {
        IModuleTypeReference/*?*/ FieldType { get; }
    }

    internal interface IModuleMethodReference : IModuleMemberReference, IMethodReference
    {
        EnumerableArrayWrapper<CustomModifier, ICustomModifier> ReturnCustomModifiers { get; }
        IModuleTypeReference/*?*/ ReturnType { get; }
        bool IsReturnByReference { get; }
        EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> RequiredModuleParameterInfos { get; }
        EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> VarArgModuleParameterInfos { get; }
    }

    /// <summary>
    /// Represents a metadata entity. This has an associated Token Value...
    /// This is used in maintaining type spec cache.
    /// </summary>
    internal abstract class MetadataObject : IReference, IMetadataObjectWithToken
    {
        internal PEFileToObjectModel PEFileToObjectModel;

        protected MetadataObject(
          PEFileToObjectModel peFileToObjectModel
        )
        {
            this.PEFileToObjectModel = peFileToObjectModel;
        }

        internal abstract uint TokenValue { get; }

        public IPlatformType PlatformType
        {
            get { return this.PEFileToObjectModel.PlatformType; }
        }

        #region IReference Members

        public virtual IEnumerable<ICustomAttribute> Attributes
        {
            get
            {
                uint customAttributeRowIdStart;
                uint customAttributeRowIdEnd;
                this.PEFileToObjectModel.GetCustomAttributeInfo(this, out customAttributeRowIdStart, out customAttributeRowIdEnd);
                for (uint customAttributeIter = customAttributeRowIdStart; customAttributeIter < customAttributeRowIdEnd; ++customAttributeIter)
                {
                    yield return this.PEFileToObjectModel.GetCustomAttributeAtRow(this, customAttributeIter);
                }
            }
        }

        public abstract void Dispatch(IMetadataVisitor visitor);

        public virtual IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region IMetadataObjectWithToken Members

        uint IMetadataObjectWithToken.TokenValue
        {
            get { return this.TokenValue; }
        }

        #endregion IMetadataObjectWithToken Members
    }

    /// <summary>
    /// Base class of Namespaces/Types/TypeMembers.
    /// </summary>
    internal abstract class MetadataDefinitionObject : MetadataObject, IDefinition
    {
        protected MetadataDefinitionObject(
          PEFileToObjectModel peFileToObjectModel
        )
          : base(peFileToObjectModel)
        {
        }
    }

    internal enum ContainerState : byte
    {
        Initialized,
        StartedLoading,
        Loaded,
    }

    ///// <summary>
    ///// Contains generic implementation of being a container as well as a scope.
    ///// </summary>
    ///// <typeparam name="InternalMemberType">The type of actual objects that are stored</typeparam>
    ///// <typeparam name="ExternalMemberType">The type of objects as they are exposed outside</typeparam>
    ///// <typeparam name="ExternalContainerType">Externally visible container type</typeparam>
    internal abstract class ScopedContainerMetadataObject<InternalMemberType, ExternalMemberType, ExternalContainerType> : MetadataDefinitionObject, IContainer<ExternalMemberType>, IScope<ExternalMemberType>
      where InternalMemberType : class, ExternalMemberType
      where ExternalMemberType : class, IScopeMember<IScope<ExternalMemberType>>, IContainerMember<ExternalContainerType>
    {
        private MultiHashtable<InternalMemberType>/*?*/  _caseSensitiveMemberHashTable;
        private MultiHashtable<InternalMemberType>/*?*/ _caseInsensitiveMemberHashTable;

        //^ [SpecPublic]
        protected ContainerState ContainerState;

        //^ invariant this.ContainerState != ContainerState.Initialized ==> this.caseSensitiveMemberHashTable != null;
        //^ invariant this.ContainerState != ContainerState.Initialized ==> this.caseInsensitiveMemberHashTable != null;

        protected ScopedContainerMetadataObject(
          PEFileToObjectModel peFileToObjectModel
        )
          : base(peFileToObjectModel)
        {
            this.ContainerState = ContainerState.Initialized;
        }

        internal void StartLoadingMembers()
        //^ ensures this.ContainerState == ContainerState.StartedLoading;
        {
            if (this.ContainerState == ContainerState.Initialized)
            {
                _caseSensitiveMemberHashTable = new MultiHashtable<InternalMemberType>();
                _caseInsensitiveMemberHashTable = new MultiHashtable<InternalMemberType>();
                this.ContainerState = ContainerState.StartedLoading;
            }
        }

        internal void AddMember(InternalMemberType/*!*/ member)
        //^ requires this.ContainerState != ContainerState.Loaded;
        {
            Debug.Assert(this.ContainerState != ContainerState.Loaded);
            if (this.ContainerState == ContainerState.Initialized)
                this.StartLoadingMembers();
            //^ assert this.caseSensitiveMemberHashTable != null;
            //^ assert this.caseInsensitiveMemberHashTable != null;
            IName name = ((IContainerMember<ExternalContainerType>)member).Name;
            _caseSensitiveMemberHashTable.Add((uint)name.UniqueKey, member);
            _caseInsensitiveMemberHashTable.Add((uint)name.UniqueKeyIgnoringCase, member);
        }

        protected void DoneLoadingMembers()
        //^ requires this.ContainerState == ContainerState.StartedLoading;
        //^ ensures this.ContainerState == ContainerState.Loaded;
        {
            Debug.Assert(this.ContainerState == ContainerState.StartedLoading);
            this.ContainerState = ContainerState.Loaded;
            //^ assert this.caseSensitiveMemberHashTable != null;
            //^ assert this.caseInsensitiveMemberHashTable != null;
        }

        internal abstract void LoadMembers()
          //^ requires this.ContainerState == ContainerState.StartedLoading;
          //^ ensures this.ContainerState == ContainerState.Loaded;
          ;

        internal MultiHashtable<InternalMemberType>.ValuesEnumerable InternalMembers
        {
            get
            {
                if (this.ContainerState != ContainerState.Loaded)
                {
                    this.LoadMembers();
                }
                //^ assert this.caseSensitiveMemberHashTable != null;
                return _caseSensitiveMemberHashTable.Values;
            }
        }

        #region IContainer<ExternalMemberType> Members

        //^ [Pure]
        public bool Contains(ExternalMemberType/*!*/ member)
        {
            if (this.ContainerState != ContainerState.Loaded)
            {
                this.LoadMembers();
            }
            //^ assert this.caseSensitiveMemberHashTable != null;
            InternalMemberType/*?*/ internalMember = member as InternalMemberType;
            if (internalMember == null)
                return false;
            return _caseSensitiveMemberHashTable.Contains((uint)member.Name.UniqueKey, internalMember);
        }

        #endregion IContainer<ExternalMemberType> Members

        #region IScope<ExternalMemberType> Members

        //^ [Pure]
        public IEnumerable<ExternalMemberType> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ExternalMemberType, bool> predicate)
        {
            if (this.ContainerState != ContainerState.Loaded)
            {
                this.LoadMembers();
            }
            int key = ignoreCase ? name.UniqueKeyIgnoringCase : name.UniqueKey;
            //^ assert this.caseSensitiveMemberHashTable != null;
            //^ assert this.caseInsensitiveMemberHashTable != null;
            MultiHashtable<InternalMemberType> hashTable = ignoreCase ? _caseInsensitiveMemberHashTable : _caseSensitiveMemberHashTable;
            foreach (ExternalMemberType member in hashTable.GetValuesFor((uint)key))
            {
                if (predicate(member))
                    yield return member;
            }
        }

        //^ [Pure]
        public IEnumerable<ExternalMemberType> GetMatchingMembers(Function<ExternalMemberType, bool> predicate)
        {
            if (this.ContainerState != ContainerState.Loaded)
            {
                this.LoadMembers();
            }
            //^ assert this.caseSensitiveMemberHashTable != null;
            foreach (ExternalMemberType member in _caseSensitiveMemberHashTable.Values)
                if (predicate(member))
                    yield return member;
        }

        //^ [Pure]
        public IEnumerable<ExternalMemberType> GetMembersNamed(IName name, bool ignoreCase)
        {
            if (this.ContainerState != ContainerState.Loaded)
            {
                this.LoadMembers();
            }
            int key = ignoreCase ? name.UniqueKeyIgnoringCase : name.UniqueKey;
            //^ assert this.caseSensitiveMemberHashTable != null;
            //^ assert this.caseInsensitiveMemberHashTable != null;
            MultiHashtable<InternalMemberType> hashTable = ignoreCase ? _caseInsensitiveMemberHashTable : _caseSensitiveMemberHashTable;
            foreach (ExternalMemberType member in hashTable.GetValuesFor((uint)key))
            {
                yield return member;
            }
        }

        public IEnumerable<ExternalMemberType> Members
        {
            get
            {
                if (this.ContainerState != ContainerState.Loaded)
                {
                    this.LoadMembers();
                }
                //^ assert this.caseSensitiveMemberHashTable != null;
                foreach (ExternalMemberType member in _caseSensitiveMemberHashTable.Values)
                    yield return member;
            }
        }

        #endregion IScope<ExternalMemberType> Members
    }

    #endregion Base Objects for Object Model

    #region Assembly/Module Level Object Model

    internal class Module : MetadataObject, IModule, IModuleModuleReference
    {
        internal readonly IName ModuleName;
        private readonly COR20Flags _cor20Flags;
        internal readonly uint InternedModuleId;
        internal readonly ModuleIdentity ModuleIdentity;
        private IMethodReference/*?*/ _entryPointMethodReference;

        internal Module(
          PEFileToObjectModel peFileToObjectModel,
          IName moduleName,
          COR20Flags cor20Flags,
          uint internedModuleId,
          ModuleIdentity moduleIdentity
        )
          : base(peFileToObjectModel)
        {
            this.ModuleName = moduleName;
            _cor20Flags = cor20Flags;
            this.InternedModuleId = internedModuleId;
            this.ModuleIdentity = moduleIdentity;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.Module | (uint)0x00000001; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return this.ModuleIdentity.ToString();
        }

        #region IModule Members

        ulong IModule.BaseAddress
        {
            get
            {
                return this.PEFileToObjectModel.PEFileReader.ImageBase;
            }
        }

        IAssembly/*?*/ IModule.ContainingAssembly
        {
            get
            {
                return this.PEFileToObjectModel.ContainingAssembly;
            }
        }

        IEnumerable<IAssemblyReference> IModule.AssemblyReferences
        {
            get
            {
                return this.PEFileToObjectModel.GetAssemblyReferences();
            }
        }

        ushort IModule.DllCharacteristics
        {
            get { return (ushort)this.PEFileToObjectModel.GetDllCharacteristics(); }
        }

        IMethodReference IModule.EntryPoint
        {
            get
            {
                if (_entryPointMethodReference == null)
                {
                    _entryPointMethodReference = this.PEFileToObjectModel.GetEntryPointMethod();
                }
                return _entryPointMethodReference;
            }
        }

        uint IModule.FileAlignment
        {
            get { return this.PEFileToObjectModel.PEFileReader.FileAlignment; }
        }

        bool IModule.ILOnly
        {
            get { return (_cor20Flags & COR20Flags.ILOnly) == COR20Flags.ILOnly; }
        }

        ModuleKind IModule.Kind
        {
            get { return this.PEFileToObjectModel.ModuleKind; }
        }

        byte IModule.LinkerMajorVersion
        {
            get { return this.PEFileToObjectModel.PEFileReader.LinkerMajorVersion; }
        }

        byte IModule.LinkerMinorVersion
        {
            get { return this.PEFileToObjectModel.PEFileReader.LinkerMinorVersion; }
        }

        byte IModule.MetadataFormatMajorVersion
        {
            get { return this.PEFileToObjectModel.MetadataFormatMajorVersion; }
        }

        byte IModule.MetadataFormatMinorVersion
        {
            get { return this.PEFileToObjectModel.MetadataFormatMinorVersion; }
        }

        IName IModule.ModuleName
        {
            get { return this.ModuleName; }
        }

        IEnumerable<IModuleReference> IModule.ModuleReferences
        {
            get
            {
                return this.PEFileToObjectModel.GetModuleReferences();
            }
        }

        Guid IModule.PersistentIdentifier
        {
            get
            {
                return this.PEFileToObjectModel.ModuleGuidIdentifier;
            }
        }

        bool IModule.RequiresAmdInstructionSet
        {
            get { return this.PEFileToObjectModel.RequiresAmdInstructionSet; }
        }

        bool IModule.Requires32bits
        {
            get { return (_cor20Flags & COR20Flags.Bit32Required) == COR20Flags.Bit32Required; }
        }

        bool IModule.Requires64bits
        {
            get { return this.PEFileToObjectModel.Requires64Bits; }
        }

        ulong IModule.SizeOfHeapCommit
        {
            get { return this.PEFileToObjectModel.PEFileReader.SizeOfHeapCommit; }
        }

        ulong IModule.SizeOfHeapReserve
        {
            get { return this.PEFileToObjectModel.PEFileReader.SizeOfHeapReserve; }
        }

        ulong IModule.SizeOfStackCommit
        {
            get { return this.PEFileToObjectModel.PEFileReader.SizeOfStackCommit; }
        }

        ulong IModule.SizeOfStackReserve
        {
            get { return this.PEFileToObjectModel.PEFileReader.SizeOfStackReserve; }
        }

        string IModule.TargetRuntimeVersion
        {
            get { return this.PEFileToObjectModel.TargetRuntimeVersion; }
        }

        bool IModule.TrackDebugData
        {
            get { return (_cor20Flags & COR20Flags.TrackDebugData) == COR20Flags.TrackDebugData; }
        }

        bool IModule.UsePublicKeyTokensForAssemblyReferences
        {
            get { return true; }
        }

        IEnumerable<IWin32Resource> IModule.Win32Resources
        {
            get
            {
                return this.PEFileToObjectModel.GetWin32Resources();
            }
        }

        IEnumerable<ICustomAttribute> IModule.ModuleAttributes
        {
            get
            {
                return this.PEFileToObjectModel.GetModuleCustomAttributes();
            }
        }

        IEnumerable<string> IModule.GetStrings()
        {
            return this.PEFileToObjectModel.PEFileReader.UserStringStream.GetStrings();
        }

        IEnumerable<INamedTypeDefinition> IModule.GetAllTypes()
        {
            return this.PEFileToObjectModel.GetAllTypes();
        }

        #endregion IModule Members

        #region IUnit Members

        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get { return this.PEFileToObjectModel.ContractAssemblySymbolicIdentity; }
        }

        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get { return this.PEFileToObjectModel.CoreAssemblySymbolicIdentity; }
        }

        IPlatformType IUnit.PlatformType
        {
            get { return this.PEFileToObjectModel.PlatformType; }
        }

        string IUnit.Location
        {
            get { return this.ModuleIdentity.Location; }
        }

        IRootUnitNamespace IUnit.UnitNamespaceRoot
        {
            get
            {
                return this.PEFileToObjectModel.RootModuleNamespace;
            }
        }

        IEnumerable<IUnitReference> IUnit.UnitReferences
        {
            get
            {
                foreach (IUnitReference ur in this.PEFileToObjectModel.GetAssemblyReferences())
                {
                    yield return ur;
                }
                foreach (IUnitReference ur in this.PEFileToObjectModel.GetModuleReferences())
                {
                    yield return ur;
                }
            }
        }

        #endregion IUnit Members

        #region INamespaceRootOwner Members

        INamespaceDefinition INamespaceRootOwner.NamespaceRoot
        {
            get
            {
                return this.PEFileToObjectModel.RootModuleNamespace;
            }
        }

        #endregion INamespaceRootOwner Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.ModuleName; }
        }

        #endregion INamedEntity Members

        #region IModuleReference Members

        ModuleIdentity IModuleReference.ModuleIdentity
        {
            get
            {
                return this.ModuleIdentity;
            }
        }

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get
            {
                return this.PEFileToObjectModel.ContainingAssembly;
            }
        }

        IModule IModuleReference.ResolvedModule
        {
            get { return this; }
        }

        #endregion IModuleReference Members

        #region IUnitReference Members

        public UnitIdentity UnitIdentity
        {
            get
            {
                return this.ModuleIdentity;
            }
        }

        public IUnit ResolvedUnit
        {
            get { return this; }
        }

        #endregion IUnitReference Members

        #region IModuleModuleReference Members

        uint IModuleModuleReference.InternedModuleId
        {
            get { return this.InternedModuleId; }
        }

        #endregion IModuleModuleReference Members
    }

    internal sealed class Assembly : Module, IAssembly, IModuleModuleReference
    {
        private readonly IName _assemblyName;
        private readonly AssemblyFlags _assemblyFlags;
        private readonly byte[] _publicKey;
        internal readonly AssemblyIdentity AssemblyIdentity;
        internal EnumerableArrayWrapper<Module, IModule> MemberModules;

        internal Assembly(
          PEFileToObjectModel peFileToObjectModel,
          IName moduleName,
          COR20Flags corFlags,
          uint internedModuleId,
          AssemblyIdentity assemblyIdentity,
          IName assemblyName,
          AssemblyFlags assemblyFlags,
          byte[] publicKey
        )
          : base(peFileToObjectModel, moduleName, corFlags, internedModuleId, assemblyIdentity)
        //^ requires peFileToObjectModel.PEFileReader.IsAssembly;
        {
            _assemblyName = assemblyName;
            _assemblyFlags = assemblyFlags;
            _publicKey = publicKey;
            this.AssemblyIdentity = assemblyIdentity;
            this.MemberModules = TypeCache.EmptyModuleArray;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.Assembly | (uint)0x00000001; }
        }

        internal Module/*?*/ FindMemberModuleNamed(
          IName moduleName
        )
        {
            Module[] memberModuleArray = this.MemberModules.RawArray;
            for (int i = 0; i < memberModuleArray.Length; ++i)
            {
                if (memberModuleArray[i].ModuleName.UniqueKeyIgnoringCase != moduleName.UniqueKeyIgnoringCase)
                    continue;
                return memberModuleArray[i];
            }
            return null;
        }

        internal void SetMemberModules(
          EnumerableArrayWrapper<Module, IModule> memberModules
        )
        {
            this.MemberModules = memberModules;
        }

        //^ [Confined]
        public override string ToString()
        {
            return this.AssemblyIdentity.ToString();
        }

        #region IAssembly Members

        IEnumerable<IAliasForType> IAssembly.ExportedTypes
        {
            get { return this.PEFileToObjectModel.GetEnumberableForExportedTypes(); }
        }

        //public IEnumerable<byte> StrongNameSignature {
        //  get { return this.PEFileToObjectModel.GetStrongNameSignature(); }
        //}

        IEnumerable<IResourceReference> IAssembly.Resources
        {
            get
            {
                return this.PEFileToObjectModel.GetResources();
            }
        }

        IEnumerable<IFileReference> IAssembly.Files
        {
            get
            {
                return this.PEFileToObjectModel.GetFiles();
            }
        }

        IEnumerable<IModule> IAssembly.MemberModules
        {
            get { return this.MemberModules; }
        }

        IEnumerable<ISecurityAttribute> IAssembly.SecurityAttributes
        {
            get
            {
                uint secAttributeRowIdStart;
                uint secAttributeRowIdEnd;
                this.PEFileToObjectModel.GetSecurityAttributeInfo(this, out secAttributeRowIdStart, out secAttributeRowIdEnd);
                for (uint secAttributeIter = secAttributeRowIdStart; secAttributeIter < secAttributeRowIdEnd; ++secAttributeIter)
                {
                    yield return this.PEFileToObjectModel.GetSecurityAttributeAtRow(this, secAttributeIter);
                }
            }
        }

        uint IAssembly.Flags
        {
            get { return (uint)_assemblyFlags; }
        }

        IEnumerable<byte> IAssembly.PublicKey
        {
            get
            {
                return new EnumerableArrayWrapper<byte>(_publicKey);
            }
        }

        IEnumerable<ICustomAttribute> IAssembly.AssemblyAttributes
        {
            get
            {
                return this.PEFileToObjectModel.GetAssemblyCustomAttributes();
            }
        }

        #endregion IAssembly Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get
            {
                return _assemblyName;
            }
        }

        #endregion INamedEntity Members

        #region IModuleReference Members

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get { return this; }
        }

        #endregion IModuleReference Members

        #region IAssemblyReference Members

        AssemblyIdentity IAssemblyReference.AssemblyIdentity
        {
            get
            {
                return this.AssemblyIdentity;
            }
        }

        AssemblyIdentity IAssemblyReference.UnifiedAssemblyIdentity
        {
            get
            {
                return this.AssemblyIdentity;
            }
        }

        IEnumerable<IName> IAssemblyReference.Aliases
        {
            get { return IteratorHelper.GetEmptyEnumerable<IName>(); }
        }

        IAssembly IAssemblyReference.ResolvedAssembly
        {
            get { return this; }
        }

        string IAssemblyReference.Culture
        {
            get { return this.AssemblyIdentity.Culture; }
        }

        IEnumerable<byte> IAssemblyReference.PublicKeyToken
        {
            get { return this.AssemblyIdentity.PublicKeyToken; }
        }

        Version IAssemblyReference.Version
        {
            get { return this.AssemblyIdentity.Version; }
        }

        #endregion IAssemblyReference Members
    }

    internal sealed class ModuleReference : MetadataObject, IModuleModuleReference
    {
        private readonly uint _moduleRefRowId;
        internal readonly uint InternedId;
        internal readonly ModuleIdentity ModuleIdentity;
        private IModule/*?*/ _resolvedModule;

        internal ModuleReference(
          PEFileToObjectModel peFileToObjectModel,
          uint moduleRefRowId,
          uint internedId,
          ModuleIdentity moduleIdentity
        )
          : base(peFileToObjectModel)
        {
            _moduleRefRowId = moduleRefRowId;
            this.InternedId = internedId;
            this.ModuleIdentity = moduleIdentity;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.ModuleRef | _moduleRefRowId; }
        }

        internal IModule ResolvedModule
        {
            get
            {
                if (_resolvedModule == null)
                {
                    Module/*?*/ resModule = this.PEFileToObjectModel.ResolveModuleRefReference(this);
                    if (resModule == null)
                    {
                        //  Cant resolve error...
                        _resolvedModule = Dummy.Module;
                    }
                    else
                    {
                        _resolvedModule = resModule;
                    }
                }
                return _resolvedModule;
            }
        }

        //^ [Confined]
        public override string ToString()
        {
            return this.ModuleIdentity.ToString();
        }

        #region IUnitReference Members

        UnitIdentity IUnitReference.UnitIdentity
        {
            get { return this.ModuleIdentity; }
        }

        IUnit IUnitReference.ResolvedUnit
        {
            get { return this.ResolvedModule; }
        }

        #endregion IUnitReference Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.ModuleIdentity.Name; }
        }

        #endregion INamedEntity Members

        #region IModuleReference Members

        ModuleIdentity IModuleReference.ModuleIdentity
        {
            get { return this.ModuleIdentity; }
        }

        IModule IModuleReference.ResolvedModule
        {
            get { return this.ResolvedModule; }
        }

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get { return this.PEFileToObjectModel.ContainingAssembly; }
        }

        #endregion IModuleReference Members

        #region IModuleModuleReference Members

        public uint InternedModuleId
        {
            get { return this.InternedId; }
        }

        #endregion IModuleModuleReference Members
    }

    internal sealed class AssemblyReference : MetadataObject, IAssemblyReference, IModuleModuleReference
    {
        private readonly uint _assemblyRefRowId;
        internal readonly AssemblyIdentity AssemblyIdentity;

        internal AssemblyReference(
          PEFileToObjectModel peFileToObjectModel,
          uint assemblyRefRowId,
          AssemblyIdentity assemblyIdentity
        )
          : base(peFileToObjectModel)
        {
            _assemblyRefRowId = assemblyRefRowId;
            this.AssemblyIdentity = assemblyIdentity;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal uint InternedId
        {
            get
            {
                if (_internedId == 0)
                {
                    AssemblyIdentity unifiedProbedAssemblyIdentity = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.ProbeAssemblyReference(
                      this.PEFileToObjectModel.Module, this.UnifiedAssemblyIdentity);
                    _internedId = (uint)this.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetAssemblyInternedKey(unifiedProbedAssemblyIdentity);
                }
                return _internedId;
            }
        }

        private uint _internedId;

        internal IAssembly ResolvedAssembly
        {
            get
            {
                if (_resolvedAssembly == null)
                {
                    Assembly/*?*/ assembly = this.PEFileToObjectModel.ResolveAssemblyRefReference(this);
                    if (assembly == null)
                    {
                        //  Cant resolve error...
                        _resolvedAssembly = Dummy.Assembly;
                    }
                    else
                    {
                        _resolvedAssembly = assembly;
                    }
                }
                return _resolvedAssembly;
            }
        }

        private IAssembly/*?*/ _resolvedAssembly;

        internal override uint TokenValue
        {
            get { return TokenTypeIds.AssemblyRef | _assemblyRefRowId; }
        }

        internal AssemblyIdentity UnifiedAssemblyIdentity
        {
            get
            {
                if (_unifiedAssemblyIdentity == null)
                    _unifiedAssemblyIdentity = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.UnifyAssembly(this.AssemblyIdentity);
                return _unifiedAssemblyIdentity;
            }
        }

        private AssemblyIdentity/*?*/ _unifiedAssemblyIdentity;

        //^ [Confined]
        public override string ToString()
        {
            return this.AssemblyIdentity.ToString();
        }

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.AssemblyIdentity.Name; }
        }

        #endregion INamedEntity Members

        #region IUnitReference Members

        UnitIdentity IUnitReference.UnitIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        IUnit IUnitReference.ResolvedUnit
        {
            get { return this.ResolvedAssembly; }
        }

        #endregion IUnitReference Members

        #region IModuleReference Members

        ModuleIdentity IModuleReference.ModuleIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get { return this; }
        }

        IModule IModuleReference.ResolvedModule
        {
            get { return this.ResolvedAssembly; }
        }

        #endregion IModuleReference Members

        #region IAssemblyReference Members

        AssemblyIdentity IAssemblyReference.AssemblyIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        AssemblyIdentity IAssemblyReference.UnifiedAssemblyIdentity
        {
            get { return this.UnifiedAssemblyIdentity; }
        }

        IAssembly IAssemblyReference.ResolvedAssembly
        {
            get { return this.ResolvedAssembly; }
        }

        IEnumerable<IName> IAssemblyReference.Aliases
        {
            get { return IteratorHelper.GetEmptyEnumerable<IName>(); }
        }

        string IAssemblyReference.Culture
        {
            get { return this.AssemblyIdentity.Culture; }
        }

        IEnumerable<byte> IAssemblyReference.PublicKeyToken
        {
            get { return this.AssemblyIdentity.PublicKeyToken; }
        }

        Version IAssemblyReference.Version
        {
            get { return this.AssemblyIdentity.Version; }
        }

        #endregion IAssemblyReference Members

        #region IModuleModuleReference Members

        public uint InternedModuleId
        {
            get { return this.InternedId; }
        }

        #endregion IModuleModuleReference Members
    }

    #endregion Assembly/Module Level Object Model

    #region Namespace Level Object Model

    internal abstract class Namespace : ScopedContainerMetadataObject<INamespaceMember, INamespaceMember, INamespaceDefinition>, IUnitNamespace
    {
        internal readonly IName NamespaceName;
        internal readonly IName NamespaceFullName;
        private uint _namespaceNameOffset;

        protected Namespace(
          PEFileToObjectModel peFileToObjectModel,
          IName namespaceName,
          IName namespaceFullName
        )
          : base(peFileToObjectModel)
        {
            this.NamespaceName = namespaceName;
            this.NamespaceFullName = namespaceFullName;
            _namespaceNameOffset = 0xFFFFFFFF;
        }

        internal void SetNamespaceNameOffset(
          uint namespaceNameOffset
        )
        {
            _namespaceNameOffset = namespaceNameOffset;
        }

        internal uint NamespaceNameOffset
        {
            get
            {
                return _namespaceNameOffset;
            }
        }

        internal override uint TokenValue
        {
            get { return 0xFFFFFFFF; }
        }

        internal override void LoadMembers()
        {
            //  Part of double check pattern. This method should be called after checking the flag FillMembers.
            lock (GlobalLock.LockingObject)
            {
                if (this.ContainerState == ContainerState.Loaded)
                    return;
                this.StartLoadingMembers();
                if (_namespaceNameOffset != 0xFFFFFFFF)
                    this.PEFileToObjectModel.LoadTypesInNamespace(this);
                this.PEFileToObjectModel._Module_.LoadMembers();
                this.DoneLoadingMembers();
            }
        }

        //^ [Confined]
        public override string ToString()
        {
            return TypeHelper.GetNamespaceName((IUnitNamespaceReference)this, NameFormattingOptions.None);
        }

        #region IUnitNamespace Members

        public IUnit Unit
        {
            get { return this.PEFileToObjectModel.Module; }
        }

        #endregion IUnitNamespace Members

        #region INamespaceDefinition Members

        public INamespaceRootOwner RootOwner
        {
            get { return this.PEFileToObjectModel.Module; }
        }

        #endregion INamespaceDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return this.NamespaceName; }
        }

        #endregion INamedEntity Members

        #region IUnitNamespaceReference Members

        IUnitReference IUnitNamespaceReference.Unit
        {
            get { return this.PEFileToObjectModel.Module; }
        }

        IUnitNamespace IUnitNamespaceReference.ResolvedUnitNamespace
        {
            get { return this; }
        }

        #endregion IUnitNamespaceReference Members
    }

    internal sealed class RootNamespace : Namespace, IRootUnitNamespace
    {
        //^ [NotDelayed]
        internal RootNamespace(
          PEFileToObjectModel peFileToObjectModel
        )
          : base(peFileToObjectModel, peFileToObjectModel.NameTable.EmptyName, peFileToObjectModel.NameTable.EmptyName)
        {
            //^ base;
            this.SetNamespaceNameOffset(0);
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    internal sealed class NestedNamespace : Namespace, INestedUnitNamespace
    {
        private readonly Namespace _parentModuleNamespace;

        internal NestedNamespace(
          PEFileToObjectModel peFileToObjectModel,
          IName namespaceName,
          IName namespaceFullName,
          Namespace parentModuleNamespace
        )
          : base(peFileToObjectModel, namespaceName, namespaceFullName)
        {
            _parentModuleNamespace = parentModuleNamespace;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return _parentModuleNamespace; }
        }

        public IUnitNamespace ContainingUnitNamespace
        {
            get { return _parentModuleNamespace; }
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        public INamespaceDefinition Container
        {
            get { return _parentModuleNamespace; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        public IScope<INamespaceMember> ContainingScope
        {
            get { return _parentModuleNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members

        #region INestedUnitNamespaceReference Members

        IUnitNamespaceReference INestedUnitNamespaceReference.ContainingUnitNamespace
        {
            get { return _parentModuleNamespace; }
        }

        INestedUnitNamespace INestedUnitNamespaceReference.ResolvedNestedUnitNamespace
        {
            get { return this; }
        }

        #endregion INestedUnitNamespaceReference Members
    }

    internal abstract class NamespaceReference : MetadataObject, IUnitNamespaceReference
    {
        internal readonly IName NamespaceName;
        internal readonly IName NamespaceFullName;
        internal readonly IModuleModuleReference ModuleReference;

        protected NamespaceReference(
          PEFileToObjectModel peFileToObjectModel,
          IModuleModuleReference moduleReference,
          IName namespaceName,
          IName namespaceFullName
        )
          : base(peFileToObjectModel)
        {
            this.NamespaceName = namespaceName;
            this.ModuleReference = moduleReference;
            this.NamespaceFullName = namespaceFullName;
        }

        internal override uint TokenValue
        {
            get { return 0xFFFFFFFF; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return TypeHelper.GetNamespaceName(this, NameFormattingOptions.None);
        }

        #region IUnitNamespaceReference Members

        public IUnitReference Unit
        {
            get { return this.ModuleReference; }
        }

        public abstract IUnitNamespace ResolvedUnitNamespace
        {
            get;
        }

        #endregion IUnitNamespaceReference Members

        #region INamedEntity Members

        public IName Name
        {
            get { return this.NamespaceName; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class RootNamespaceReference : NamespaceReference, IRootUnitNamespaceReference
    {
        internal RootNamespaceReference(
          PEFileToObjectModel peFileToObjectModel,
          IModuleModuleReference moduleReference
        )
          : base(peFileToObjectModel, moduleReference, peFileToObjectModel.NameTable.EmptyName, peFileToObjectModel.NameTable.EmptyName)
        {
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IUnitNamespace ResolvedUnitNamespace
        {
            get
            {
                return this.ModuleReference.ResolvedModule.UnitNamespaceRoot;
            }
        }
    }

    internal sealed class NestedNamespaceReference : NamespaceReference, INestedUnitNamespaceReference
    {
        private readonly NamespaceReference _parentModuleNamespaceReference;
        private INestedUnitNamespace/*?*/ _resolvedNamespace;

        internal NestedNamespaceReference(
          PEFileToObjectModel peFileToObjectModel,
          IName namespaceName,
          IName namespaceFullName,
          NamespaceReference parentModuleNamespaceReference
        )
          : base(peFileToObjectModel, parentModuleNamespaceReference.ModuleReference, namespaceName, namespaceFullName)
        {
            _parentModuleNamespaceReference = parentModuleNamespaceReference;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IUnitNamespace ResolvedUnitNamespace
        {
            get { return this.ResolvedNestedUnitNamespace; }
        }

        #region INestedUnitNamespaceReference Members

        public IUnitNamespaceReference ContainingUnitNamespace
        {
            get { return _parentModuleNamespaceReference; }
        }

        public INestedUnitNamespace ResolvedNestedUnitNamespace
        {
            get
            {
                if (_resolvedNamespace == null)
                {
                    foreach (INestedUnitNamespace nestedUnitNamespace
                      in IteratorHelper.GetFilterEnumerable<INamespaceMember, INestedUnitNamespace>(
                        _parentModuleNamespaceReference.ResolvedUnitNamespace.GetMembersNamed(this.NamespaceName, false)
                      )
                    )
                    {
                        _resolvedNamespace = nestedUnitNamespace;
                        break;
                    }
                    _resolvedNamespace = Dummy.NestedUnitNamespace;
                }
                return _resolvedNamespace;
            }
        }

        #endregion INestedUnitNamespaceReference Members
    }

    #endregion Namespace Level Object Model

    #region TypeMember Level Object Model

    internal abstract class TypeMember : MetadataDefinitionObject, IModuleTypeDefinitionMember
    {
        protected readonly IName MemberName;

        //^ [SpecPublic]
        internal readonly TypeBase OwningModuleType;

        protected TypeMember(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase owningModuleType
        )
          : base(peFileToObjectModel)
        {
            this.MemberName = memberName;
            this.OwningModuleType = owningModuleType;
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMemberSignature(this, NameFormattingOptions.None);
        }

        #region IModuleTypeDefinitionMember Members

        public abstract ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        );

        #endregion IModuleTypeDefinitionMember Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get
            {
                return this.OwningModuleType;
            }
        }

        public abstract TypeMemberVisibility Visibility { get; }

        #endregion ITypeDefinitionMember Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return this.ContainingTypeDefinition; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IContainerMember<ITypeDefinition> Members

        ITypeDefinition IContainerMember<ITypeDefinition>.Container
        {
            get { return this.OwningModuleType; }
        }

        IName IContainerMember<ITypeDefinition>.Name
        {
            get { return this.MemberName; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        IScope<ITypeDefinitionMember> IScopeMember<IScope<ITypeDefinitionMember>>.ContainingScope
        {
            get { return this.OwningModuleType; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region INamedEntity Members

        public virtual IName Name
        {
            get { return this.MemberName; }
        }

        #endregion INamedEntity Members
    }

    internal class FieldDefinition : TypeMember, IFieldDefinition, IModuleFieldReference
    {
        internal readonly uint FieldDefRowId;
        private FieldFlags _fieldFlags;
        private IModuleTypeReference/*?*/ _fieldType;
        //^ invariant ((this.FieldFlags & FieldFlags.FieldLoaded) == FieldFlags.FieldLoaded) ==> this.FieldType != null;

        //^ [NotDelayed]
        internal FieldDefinition(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint fieldDefRowId,
          FieldFlags fieldFlags
        )
          : base(peFileToObjectModel, memberName, parentModuleType)
        {
            this.FieldDefRowId = fieldDefRowId;
            _fieldFlags = fieldFlags;
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.FieldDef | this.FieldDefRowId; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        //  Half of the double check lock. Other half done by the caller...
        private void InitFieldSignature()
        //^ ensures (this.FieldFlags & FieldFlags.FieldLoaded) == FieldFlags.FieldLoaded;
        {
            FieldSignatureConverter fieldSignature = this.PEFileToObjectModel.GetFieldSignature(this);
            _fieldType = fieldSignature.TypeReference;
            _fieldFlags |= FieldFlags.FieldLoaded;
        }

        public override ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        )
        {
            Debug.Assert(genericTypeInstance.RawTemplateModuleType == this.OwningModuleType);
            return new GenericTypeInstanceField(genericTypeInstance, this);
        }

        public override TypeMemberVisibility Visibility
        {
            get
            {
                //  IF this becomes perf bottle neck use array...
                switch (_fieldFlags & FieldFlags.AccessMask)
                {
                    case FieldFlags.CompilerControlledAccess:
                        return TypeMemberVisibility.Other;

                    case FieldFlags.PrivateAccess:
                        return TypeMemberVisibility.Private;

                    case FieldFlags.FamilyAndAssemblyAccess:
                        return TypeMemberVisibility.FamilyAndAssembly;

                    case FieldFlags.AssemblyAccess:
                        return TypeMemberVisibility.Assembly;

                    case FieldFlags.FamilyAccess:
                        return TypeMemberVisibility.Family;

                    case FieldFlags.FamilyOrAssemblyAccess:
                        return TypeMemberVisibility.FamilyOrAssembly;

                    case FieldFlags.PublicAccess:
                        return TypeMemberVisibility.Public;

                    default:
                        return TypeMemberVisibility.Private;
                }
            }
        }

        #region IFieldDefinition Members

        public uint BitLength
        {
            get { return 1; }
        }

        public bool IsBitField
        {
            get { return false; }
        }

        public bool IsCompileTimeConstant
        {
            get { return (_fieldFlags & FieldFlags.LiteralContract) == FieldFlags.LiteralContract; }
        }

        public bool IsMapped
        {
            get { return (_fieldFlags & FieldFlags.HasFieldRVAReserved) == FieldFlags.HasFieldRVAReserved; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return (_fieldFlags & FieldFlags.HasFieldMarshalReserved) == FieldFlags.HasFieldMarshalReserved; }
        }

        public bool IsNotSerialized
        {
            get { return (_fieldFlags & FieldFlags.NotSerializedContract) == FieldFlags.NotSerializedContract; }
        }

        public bool IsReadOnly
        {
            get { return (_fieldFlags & FieldFlags.InitOnlyContract) == FieldFlags.InitOnlyContract; }
        }

        public bool IsRuntimeSpecial
        {
            get { return (_fieldFlags & FieldFlags.RTSpecialNameReserved) == FieldFlags.RTSpecialNameReserved; }
        }

        public bool IsSpecialName
        {
            get { return (_fieldFlags & FieldFlags.SpecialNameImpl) == FieldFlags.SpecialNameImpl; }
        }

        public bool IsStatic
        {
            get { return (_fieldFlags & FieldFlags.StaticContract) == FieldFlags.StaticContract; }
        }

        public uint Offset
        {
            get { return this.PEFileToObjectModel.GetFieldOffset(this); }
        }

        public int SequenceNumber
        {
            get { return this.PEFileToObjectModel.GetFieldSequenceNumber(this); }
        }

        public IMetadataConstant CompileTimeValue
        {
            get { return this.PEFileToObjectModel.GetDefaultValue(this); }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get { return this.PEFileToObjectModel.GetMarshallingInformation(this); }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ fieldType = this.FieldType;
                if (fieldType == null) return Dummy.TypeReference;
                return fieldType;
            }
        }

        public ISectionBlock FieldMapping
        {
            get { return this.PEFileToObjectModel.GetFieldMapping(this); }
        }

        #endregion IFieldDefinition Members

        #region IModuleMemberReference Members

        public IModuleTypeReference/*?*/ OwningTypeReference
        {
            get { return this.OwningModuleType; }
        }

        #endregion IModuleMemberReference Members

        #region IModuleFieldReference Members

        public IModuleTypeReference/*?*/ FieldType
        {
            get
            {
                if ((_fieldFlags & FieldFlags.FieldLoaded) != FieldFlags.FieldLoaded)
                {
                    this.InitFieldSignature();
                }
                //^ assert (this.FieldFlags & FieldFlags.FieldLoaded) == FieldFlags.FieldLoaded;
                //^ assert this.fieldType != null;
                return _fieldType;
            }
        }

        #endregion IModuleFieldReference Members

        #region IFieldReference Members

        public IFieldDefinition ResolvedField
        {
            get { return this; }
        }

        #endregion IFieldReference Members

        #region IMetadataConstantContainer

        IMetadataConstant IMetadataConstantContainer.Constant
        {
            get { return this.CompileTimeValue; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class GlobalFieldDefinition : FieldDefinition, IGlobalFieldDefinition
    {
        private readonly Namespace _parentModuleNamespace;
        private readonly IName _namespaceMemberName;

        //^ [NotDelayed]
        internal GlobalFieldDefinition(
          PEFileToObjectModel peFileToObjectModel,
          IName typeMemberName,
          TypeBase parentModuleType,
          uint fieldDefRowId,
          FieldFlags fieldFlags,
          IName namespaceMemberName,
          Namespace parentModuleNamespace
        )
          : base(peFileToObjectModel, typeMemberName, parentModuleType, fieldDefRowId, fieldFlags)
        {
            _namespaceMemberName = namespaceMemberName;
            _parentModuleNamespace = parentModuleNamespace;
            //^ base;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return _parentModuleNamespace; }
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        INamespaceDefinition IContainerMember<INamespaceDefinition>.Container
        {
            get { return _parentModuleNamespace; }
        }

        IName IContainerMember<INamespaceDefinition>.Name
        {
            get { return _namespaceMemberName; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        IScope<INamespaceMember> IScopeMember<IScope<INamespaceMember>>.ContainingScope
        {
            get { return _parentModuleNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members
    }

    internal sealed class SectionBlock : ISectionBlock
    {
        private readonly PESectionKind _PESectionKind;
        private readonly uint _offset;
        private readonly MemoryBlock _memoryBlock;

        internal SectionBlock(
          PESectionKind peSectionKind,
          uint offset,
          MemoryBlock memoryBlock
        )
        {
            _PESectionKind = peSectionKind;
            _offset = offset;
            _memoryBlock = memoryBlock;
        }

        #region ISectionBlock Members

        PESectionKind ISectionBlock.PESectionKind
        {
            get { return _PESectionKind; }
        }

        uint ISectionBlock.Offset
        {
            get { return _offset; }
        }

        uint ISectionBlock.Size
        {
            get { return (uint)_memoryBlock.Length; }
        }

        IEnumerable<byte> ISectionBlock.Data
        {
            get { return new EnumberableMemoryBlockWrapper(_memoryBlock); }
        }

        #endregion ISectionBlock Members
    }

    internal sealed class ReturnParameter : MetadataObject
    {
        internal readonly ParamFlags ReturnParamFlags;
        internal readonly uint ReturnParamRowId;

        internal override uint TokenValue
        {
            get { return TokenTypeIds.ParamDef | this.ReturnParamRowId; }
        }

        internal ReturnParameter(
          PEFileToObjectModel peFileToObjectModel,
          ParamFlags returnParamFlags,
          uint returnParamRowId
        )
          : base(peFileToObjectModel)
        {
            this.ReturnParamFlags = returnParamFlags;
            this.ReturnParamRowId = returnParamRowId;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
        }

        //^[Pure]
        public bool IsMarshalledExplicitly
        {
            get { return (this.ReturnParamFlags & ParamFlags.HasFieldMarshalReserved) == ParamFlags.HasFieldMarshalReserved; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get { return this.PEFileToObjectModel.GetMarshallingInformation(this); }
        }
    }

    internal sealed class PlatformInvokeInformation : IPlatformInvokeInformation
    {
        private readonly PInvokeMapFlags _PInvokeMapFlags;
        private readonly IName _importName;
        private readonly ModuleReference _importModule;

        internal PlatformInvokeInformation(
          PInvokeMapFlags pInvokeMapFlags,
          IName importName,
          ModuleReference importModule
        )
        {
            _PInvokeMapFlags = pInvokeMapFlags;
            _importName = importName;
            _importModule = importModule;
        }

        #region IPlatformInvokeInformation Members

        IName IPlatformInvokeInformation.ImportName
        {
            get { return _importName; }
        }

        IModuleReference IPlatformInvokeInformation.ImportModule
        {
            get { return _importModule; }
        }

        StringFormatKind IPlatformInvokeInformation.StringFormat
        {
            get
            {
                switch (_PInvokeMapFlags & PInvokeMapFlags.CharSetMask)
                {
                    case PInvokeMapFlags.CharSetAnsi:
                        return StringFormatKind.Ansi;

                    case PInvokeMapFlags.CharSetUnicode:
                        return StringFormatKind.Unicode;

                    case PInvokeMapFlags.CharSetAuto:
                        return StringFormatKind.AutoChar;

                    case PInvokeMapFlags.CharSetNotSpec:
                    default:
                        return StringFormatKind.Unspecified;
                }
            }
        }

        bool IPlatformInvokeInformation.NoMangle
        {
            get { return (_PInvokeMapFlags & PInvokeMapFlags.NoMangle) == PInvokeMapFlags.NoMangle; }
        }

        bool IPlatformInvokeInformation.SupportsLastError
        {
            get { return (_PInvokeMapFlags & PInvokeMapFlags.SupportsLastError) == PInvokeMapFlags.SupportsLastError; }
        }

        public PInvokeCallingConvention PInvokeCallingConvention
        {
            get
            {
                switch (_PInvokeMapFlags & PInvokeMapFlags.CallingConventionMask)
                {
                    case PInvokeMapFlags.WinAPICallingConvention:
                    default:
                        return PInvokeCallingConvention.WinApi;

                    case PInvokeMapFlags.CDeclCallingConvention:
                        return PInvokeCallingConvention.CDecl;

                    case PInvokeMapFlags.StdCallCallingConvention:
                        return PInvokeCallingConvention.StdCall;

                    case PInvokeMapFlags.ThisCallCallingConvention:
                        return PInvokeCallingConvention.ThisCall;

                    case PInvokeMapFlags.FastCallCallingConvention:
                        return PInvokeCallingConvention.FastCall;
                }
            }
        }

        bool? IPlatformInvokeInformation.ThrowExceptionForUnmappableChar
        {
            get
            {
                switch (_PInvokeMapFlags & PInvokeMapFlags.ThrowOnUnmappableCharMask)
                {
                    case PInvokeMapFlags.EnabledThrowOnUnmappableChar: return true;
                    case PInvokeMapFlags.DisabledThrowOnUnmappableChar: return false;
                    default: return null;
                }
            }
        }

        bool? IPlatformInvokeInformation.UseBestFit
        {
            get
            {
                switch (_PInvokeMapFlags & PInvokeMapFlags.BestFitMask)
                {
                    case PInvokeMapFlags.EnabledBestFit: return true;
                    case PInvokeMapFlags.DisabledBestFit: return false;
                    default: return null;
                }
            }
        }

        #endregion IPlatformInvokeInformation Members
    }

    internal abstract class MethodDefinition : TypeMember, IMethodDefinition, IModuleMethodReference
    {
        internal readonly uint MethodDefRowId;
        private MethodFlags _methodFlags;
        private MethodImplFlags _methodImplFlags;
        private EnumerableArrayWrapper<CustomModifier, ICustomModifier>/*?*/ _returnCustomModifiers;
        private IModuleTypeReference/*?*/ _returnType;
        private byte _firstSignatureByte;
        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition>/*?*/ _moduleParameters;
        private ReturnParameter/*?*/ _returnParameter;
        //^ invariant this.returnCustomModifiers != null ==> this.returnType != null;
        //^ invariant this.returnCustomModifiers != null ==> this.moduleParameters != null;
        //^ invariant this.returnCustomModifiers != null ==> this.returnParameter != null;

        //^ [NotDelayed]
        internal MethodDefinition(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint methodDefRowId,
          MethodFlags methodFlags,
          MethodImplFlags methodImplFlags
        )
          : base(peFileToObjectModel, memberName, parentModuleType)
        {
            this.MethodDefRowId = methodDefRowId;
            _methodFlags = methodFlags;
            _methodImplFlags = methodImplFlags;
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.MethodDef | this.MethodDefRowId; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override TypeMemberVisibility Visibility
        {
            get
            {
                switch (_methodFlags & MethodFlags.AccessMask)
                {
                    case MethodFlags.CompilerControlledAccess:
                        return TypeMemberVisibility.Other;

                    case MethodFlags.PrivateAccess:
                        return TypeMemberVisibility.Private;

                    case MethodFlags.FamilyAndAssemblyAccess:
                        return TypeMemberVisibility.FamilyAndAssembly;

                    case MethodFlags.AssemblyAccess:
                        return TypeMemberVisibility.Assembly;

                    case MethodFlags.FamilyAccess:
                        return TypeMemberVisibility.Family;

                    case MethodFlags.FamilyOrAssemblyAccess:
                        return TypeMemberVisibility.FamilyOrAssembly;

                    case MethodFlags.PublicAccess:
                        return TypeMemberVisibility.Public;

                    default:
                        return TypeMemberVisibility.Private;
                }
            }
        }

        //  Half of the double check lock. Other half done by the caller...
        private void InitMethodSignature()
        {
            MethodDefSignatureConverter methodSignature = this.PEFileToObjectModel.GetMethodSignature(this);
            _returnCustomModifiers = methodSignature.ReturnCustomModifiers;
            _returnType = methodSignature.ReturnTypeReference;
            _firstSignatureByte = methodSignature.FirstByte;
            _moduleParameters = methodSignature.Parameters;
            _returnParameter = methodSignature.ReturnParameter;
        }

        public override IEnumerable<ILocation> Locations
        {
            get
            {
                yield return new MethodBodyLocation(new MethodBodyDocument(this), 0);
            }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.Signature | NameFormattingOptions.TypeParameters);
        }

        #region IMethodDefinition Members

        public bool AcceptsExtraArguments
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return SignatureHeader.IsVarArgCallSignature(_firstSignatureByte);
            }
        }

        public IMethodBody Body
        {
            get { return this.PEFileToObjectModel.GetMethodBody(this); }
        }

        public abstract IEnumerable<IGenericMethodParameter> GenericParameters { get; }

        public abstract ushort GenericParameterCount { get; }

        public bool HasDeclarativeSecurity
        {
            get { return (_methodFlags & MethodFlags.HasSecurityReserved) == MethodFlags.HasSecurityReserved; }
        }

        public bool HasExplicitThisParameter
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return SignatureHeader.IsExplicitThis(_firstSignatureByte);
            }
        }

        public bool IsAbstract
        {
            get { return (_methodFlags & MethodFlags.AbstractImpl) == MethodFlags.AbstractImpl; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return (_methodFlags & MethodFlags.CheckAccessOnOverrideImpl) == MethodFlags.CheckAccessOnOverrideImpl; }
        }

        public bool IsCil
        {
            get { return (_methodImplFlags & MethodImplFlags.CodeTypeMask) == MethodImplFlags.ILCodeType; }
        }

        public bool IsExternal
        {
            get
            {
                return this.IsPlatformInvoke || this.IsRuntimeInternal || this.IsRuntimeImplemented ||
                this.PEFileToObjectModel.PEFileReader.GetMethodIL(this.MethodDefRowId) == null;
            }
        }

        public bool IsForwardReference
        {
            get { return (_methodImplFlags & MethodImplFlags.ForwardRefInterop) == MethodImplFlags.ForwardRefInterop; }
        }

        public abstract bool IsGeneric { get; }

        public bool IsHiddenBySignature
        {
            get { return (_methodFlags & MethodFlags.HideBySignatureContract) == MethodFlags.HideBySignatureContract; }
        }

        public bool IsNativeCode
        {
            get { return (_methodImplFlags & MethodImplFlags.CodeTypeMask) == MethodImplFlags.NativeCodeType; }
        }

        public bool IsNewSlot
        {
            get { return (_methodFlags & MethodFlags.NewSlotVTable) == MethodFlags.NewSlotVTable; }
        }

        public bool IsNeverInlined
        {
            get { return (_methodImplFlags & MethodImplFlags.NoInlining) == MethodImplFlags.NoInlining; }
        }

        public bool IsNeverOptimized
        {
            get { return (_methodImplFlags & MethodImplFlags.NoOptimization) == MethodImplFlags.NoOptimization; }
        }

        public bool IsPlatformInvoke
        {
            get { return (_methodFlags & MethodFlags.PInvokeInterop) == MethodFlags.PInvokeInterop; }
        }

        public bool IsRuntimeImplemented
        {
            get { return (_methodImplFlags & MethodImplFlags.CodeTypeMask) == MethodImplFlags.RuntimeCodeType; }
        }

        public bool IsRuntimeInternal
        {
            get { return (_methodImplFlags & MethodImplFlags.InternalCall) == MethodImplFlags.InternalCall; }
        }

        public bool IsRuntimeSpecial
        {
            get { return (_methodFlags & MethodFlags.RTSpecialNameReserved) == MethodFlags.RTSpecialNameReserved; }
        }

        public bool IsSealed
        {
            get { return (_methodFlags & MethodFlags.FinalContract) == MethodFlags.FinalContract; }
        }

        public bool IsSpecialName
        {
            get { return (_methodFlags & MethodFlags.SpecialNameImpl) == MethodFlags.SpecialNameImpl; }
        }

        public bool IsStatic
        {
            get { return (_methodFlags & MethodFlags.StaticContract) == MethodFlags.StaticContract; }
        }

        public bool IsSynchronized
        {
            get { return (_methodImplFlags & MethodImplFlags.Synchronized) == MethodImplFlags.Synchronized; }
        }

        public bool IsVirtual
        {
            get { return (_methodFlags & MethodFlags.VirtualContract) == MethodFlags.VirtualContract; }
        }

        public bool IsUnmanaged
        {
            get { return (_methodImplFlags & MethodImplFlags.Unmanaged) == MethodImplFlags.Unmanaged; }
        }

        public bool PreserveSignature
        {
            get { return (_methodImplFlags & MethodImplFlags.PreserveSigInterop) == MethodImplFlags.PreserveSigInterop; }
        }

        public bool RequiresSecurityObject
        {
            get { return (_methodFlags & MethodFlags.RequiresSecurityObjectReserved) == MethodFlags.RequiresSecurityObjectReserved; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get
            {
                uint secAttributeRowIdStart;
                uint secAttributeRowIdEnd;
                this.PEFileToObjectModel.GetSecurityAttributeInfo(this, out secAttributeRowIdStart, out secAttributeRowIdEnd);
                for (uint secAttributeIter = secAttributeRowIdStart; secAttributeIter < secAttributeRowIdEnd; ++secAttributeIter)
                {
                    yield return this.PEFileToObjectModel.GetSecurityAttributeAtRow(this, secAttributeIter);
                }
            }
        }

        public bool IsConstructor
        {
            get { return this.Name.UniqueKey == this.PEFileToObjectModel.NameTable.Ctor.UniqueKey && this.IsRuntimeSpecial; }
        }

        public bool IsStaticConstructor
        {
            get { return this.Name.UniqueKey == this.PEFileToObjectModel.NameTable.Cctor.UniqueKey && this.IsRuntimeSpecial; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return this.PEFileToObjectModel.GetPlatformInvokeInformation(this); }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get
            {
                return this.RequiredModuleParameters;
            }
        }

        public ushort ParameterCount
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    return this.PEFileToObjectModel.GetMethodParameterCount(this);
                }
                return (ushort)_moduleParameters.RawArray.Length;
            }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.returnParameter != null;
                uint customAttributeRowIdStart;
                uint customAttributeRowIdEnd;
                this.PEFileToObjectModel.GetCustomAttributeInfo(_returnParameter, out customAttributeRowIdStart, out customAttributeRowIdEnd);
                for (uint customAttributeIter = customAttributeRowIdStart; customAttributeIter < customAttributeRowIdEnd; ++customAttributeIter)
                {
                    //^ assert this.returnParameter != null;
                    yield return this.PEFileToObjectModel.GetCustomAttributeAtRow(_returnParameter, customAttributeIter);
                }
            }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get
            {
                return _returnParameter != null && _returnParameter.IsMarshalledExplicitly;
            }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get
            {
                return _returnParameter == null ? Dummy.MarshallingInformation : _returnParameter.MarshallingInformation;
            }
        }

        #endregion IMethodDefinition Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetConversionEnumerable<IParameterDefinition, IParameterTypeInformation>(this.Parameters); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get
            {
                return this.ReturnCustomModifiers;
            }
        }

        public bool ReturnValueIsByRef
        {
            get
            {
                return this.IsReturnByReference;
            }
        }

        public bool ReturnValueIsModified
        {
            get
            {
                return this.ReturnCustomModifiers.RawArray.Length > 0;
            }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ typeRef = this.ReturnType;
                if (typeRef == null) return Dummy.TypeReference;
                return typeRef;
            }
        }

        public CallingConvention CallingConvention
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return (CallingConvention)_firstSignatureByte;
            }
        }

        #endregion ISignature Members

        #region IModuleMethodReference Members

        public IModuleTypeReference/*?*/ OwningTypeReference
        {
            get { return this.OwningModuleType; }
        }

        public EnumerableArrayWrapper<CustomModifier, ICustomModifier> ReturnCustomModifiers
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.returnCustomModifiers != null;
                return _returnCustomModifiers;
            }
        }

        public IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return _returnType;
            }
        }

        public EnumerableArrayWrapper<IModuleParameter, IParameterDefinition> RequiredModuleParameters
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.moduleParameters != null;
                return _moduleParameters;
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> RequiredModuleParameterInfos
        {
            get
            {
                return new EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation>(
                  this.RequiredModuleParameters.RawArray, Dummy.ParameterTypeInformation);
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> VarArgModuleParameterInfos
        {
            get { return TypeCache.EmptyParameterInfoArray; }
        }

        public bool IsReturnByReference
        {
            get
            {
                if (_returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.returnParameter != null;
                return (_returnParameter.ReturnParamFlags & ParamFlags.ByReference) == ParamFlags.ByReference;
            }
        }

        #endregion IModuleMethodReference Members

        #region IMethodReference Members

        public uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                {
                    _internedKey = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetMethodInternedKey(this);
                }
                return _internedKey;
            }
        }

        private uint _internedKey;

        public IMethodDefinition ResolvedMethod
        {
            get { return this; }
        }

        public IEnumerable<IParameterTypeInformation> ExtraParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion IMethodReference Members
    }

    internal class NonGenericMethod : MethodDefinition
    {
        //^ [NotDelayed]
        internal NonGenericMethod(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint methodDefRowId,
          MethodFlags methodFlags,
          MethodImplFlags methodImplFlags
        )
          : base(peFileToObjectModel, memberName, parentModuleType, methodDefRowId, methodFlags, methodImplFlags)
        {
        }

        public override ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        )
        {
            Debug.Assert(genericTypeInstance.RawTemplateModuleType == this.OwningModuleType);
            return new GenericTypeInstanceNonGenericMethod(genericTypeInstance, this);
        }

        public override IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericMethodParameter>(); }
        }

        public override bool IsGeneric
        {
            get { return false; }
        }

        public override ushort GenericParameterCount
        {
            get { return 0; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.Signature);
        }
    }

    internal sealed class GlobalNonGenericMethod : NonGenericMethod, IGlobalMethodDefinition
    {
        private readonly Namespace _parentModuleNamespace;
        private readonly IName _namespaceMemberName;

        //^ [NotDelayed]
        internal GlobalNonGenericMethod(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint methodDefRowId,
          MethodFlags methodFlags,
          MethodImplFlags methodImplFlags,
          IName namespaceMemberName,
          Namespace parentModuleNamespace
        )
          : base(peFileToObjectModel, memberName, parentModuleType, methodDefRowId, methodFlags, methodImplFlags)
        {
            _namespaceMemberName = namespaceMemberName;
            _parentModuleNamespace = parentModuleNamespace;
            //^ base;
        }

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return _parentModuleNamespace; }
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        INamespaceDefinition IContainerMember<INamespaceDefinition>.Container
        {
            get { return _parentModuleNamespace; }
        }

        IName IContainerMember<INamespaceDefinition>.Name
        {
            get { return _namespaceMemberName; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        IScope<INamespaceMember> IScopeMember<IScope<INamespaceMember>>.ContainingScope
        {
            get { return _parentModuleNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members
    }

    internal class GenericMethod : MethodDefinition, IModuleGenericMethod
    {
        internal readonly uint GenericParamRowIdStart;
        internal readonly uint GenericParamRowIdEnd;

        //^ [NotDelayed]
        internal GenericMethod(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint methodDefRowId,
          MethodFlags methodFlags,
          MethodImplFlags methodImplFlags,
          uint genericParamRowIdStart,
          uint genericParamRowIdEnd
        )
          : base(peFileToObjectModel, memberName, parentModuleType, methodDefRowId, methodFlags, methodImplFlags)
        {
            this.GenericParamRowIdStart = genericParamRowIdStart;
            this.GenericParamRowIdEnd = genericParamRowIdEnd;
        }

        public override ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        )
        {
            Debug.Assert(genericTypeInstance.RawTemplateModuleType == this.OwningModuleType);
            return new GenericTypeInstanceGenericMethod(genericTypeInstance, this);
        }

        public override IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get
            {
                uint genericRowIdEnd = this.GenericParamRowIdEnd;
                for (uint genericParamIter = this.GenericParamRowIdStart; genericParamIter < genericRowIdEnd; ++genericParamIter)
                {
                    GenericMethodParameter/*?*/ mgmp = this.PEFileToObjectModel.GetGenericMethodParamAtRow(genericParamIter, this);
                    if (mgmp == null)
                        yield return Dummy.GenericMethodParameter;
                    else
                        yield return mgmp;
                }
            }
        }

        public override ushort GenericParameterCount
        {
            get
            {
                return (ushort)(this.GenericParamRowIdEnd - this.GenericParamRowIdStart);
            }
        }

        public override bool IsGeneric
        {
            get
            {
                return true;
            }
        }

        #region IModuleGenericMethod Members

        public ushort GenericMethodParameterCardinality
        {
            get
            {
                return (ushort)(this.GenericParamRowIdEnd - this.GenericParamRowIdStart);
            }
        }

        public IModuleTypeReference/*?*/ GetGenericMethodParameterFromOrdinal(
          ushort genericParamOrdinal
        )
        {
            if (genericParamOrdinal >= this.GenericMethodParameterCardinality)
                return null;
            uint genericRowId = this.GenericParamRowIdStart + genericParamOrdinal;
            return this.PEFileToObjectModel.GetGenericMethodParamAtRow(genericRowId, this);
        }

        #endregion IModuleGenericMethod Members
    }

    internal sealed class GlobalGenericMethod : GenericMethod, IGlobalMethodDefinition
    {
        private readonly Namespace _parentModuleNamespace;
        private readonly IName _namespaceMemberName;

        //^ [NotDelayed]
        internal GlobalGenericMethod(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint methodDefRowId,
          MethodFlags methodFlags,
          MethodImplFlags methodImplFlags,
          uint genericParamRowIdStart,
          uint genericParamRowIdEnd,
          IName namespaceMemberName,
          Namespace parentModuleNamespace
        )
          : base(peFileToObjectModel, memberName, parentModuleType, methodDefRowId, methodFlags, methodImplFlags, genericParamRowIdStart, genericParamRowIdEnd)
        {
            _namespaceMemberName = namespaceMemberName;
            _parentModuleNamespace = parentModuleNamespace;
            //^ base;
        }

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return _parentModuleNamespace; }
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        INamespaceDefinition IContainerMember<INamespaceDefinition>.Container
        {
            get { return _parentModuleNamespace; }
        }

        IName IContainerMember<INamespaceDefinition>.Name
        {
            get { return _namespaceMemberName; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        IScope<INamespaceMember> IScopeMember<IScope<INamespaceMember>>.ContainingScope
        {
            get { return _parentModuleNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members
    }

    internal sealed class EventDefinition : TypeMember, IEventDefinition
    {
        internal readonly uint EventRowId;
        private EventFlags _eventFlags;
        private bool _eventTypeInited;
        private IModuleTypeReference/*?*/ _eventType;
        private IMethodDefinition/*?*/ _adderMethod;
        private IMethodDefinition/*?*/ _removerMethod;
        private MethodDefinition/*?*/ _fireMethod;
        private TypeMemberVisibility _visibility;
        //^ invariant ((this.EventFlags & EventFlags.AdderLoaded) == EventFlags.AdderLoaded) ==> this.adderMethod != null;
        //^ invariant ((this.EventFlags & EventFlags.RemoverLoaded) == EventFlags.RemoverLoaded) ==> this.removerMethod != null;

        //^ [NotDelayed]
        internal EventDefinition(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint eventRowId,
          EventFlags eventFlags
        )
          : base(peFileToObjectModel, memberName, parentModuleType)
        {
            this.EventRowId = eventRowId;
            _eventFlags = eventFlags;
            _visibility = TypeMemberVisibility.Mask;
        }

        public override ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        )
        {
            Debug.Assert(genericTypeInstance.RawTemplateModuleType == this.OwningModuleType);
            return new GenericTypeInstanceEvent(genericTypeInstance, this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.Event | this.EventRowId; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal IMethodDefinition AdderMethod
        {
            get
            {
                if ((_eventFlags & EventFlags.AdderLoaded) != EventFlags.AdderLoaded)
                {
                    _adderMethod = this.PEFileToObjectModel.GetEventAddOrRemoveOrFireMethod(this, MethodSemanticsFlags.AddOn);
                    if (_adderMethod == null)
                    {
                        //  MDError
                        _adderMethod = Dummy.Method;
                    }
                    _eventFlags |= EventFlags.AdderLoaded;
                }
                //^ assert this.adderMethod != null;
                return _adderMethod;
            }
        }

        internal IMethodDefinition RemoverMethod
        {
            get
            {
                if ((_eventFlags & EventFlags.RemoverLoaded) != EventFlags.RemoverLoaded)
                {
                    _removerMethod = this.PEFileToObjectModel.GetEventAddOrRemoveOrFireMethod(this, MethodSemanticsFlags.RemoveOn);
                    if (_removerMethod == null)
                    {
                        //  MDError
                        _removerMethod = Dummy.Method;
                    }
                    _eventFlags |= EventFlags.RemoverLoaded;
                }
                //^ assert this.removerMethod != null;
                return _removerMethod;
            }
        }

        internal MethodDefinition/*?*/ FireMethod
        {
            get
            {
                if ((_eventFlags & EventFlags.FireLoaded) != EventFlags.FireLoaded)
                {
                    _fireMethod = this.PEFileToObjectModel.GetEventAddOrRemoveOrFireMethod(this, MethodSemanticsFlags.Fire);
                    _eventFlags |= EventFlags.FireLoaded;
                }
                return _fireMethod;
            }
        }

        public override TypeMemberVisibility Visibility
        {
            get
            {
                if (_visibility == TypeMemberVisibility.Mask)
                {
                    TypeMemberVisibility adderVisibility = this.AdderMethod.Visibility;
                    TypeMemberVisibility removerVisibility = this.RemoverMethod.Visibility;
                    _visibility = TypeCache.LeastUpperBound(adderVisibility, removerVisibility);
                }
                return _visibility;
            }
        }

        internal IModuleTypeReference/*?*/ EventType
        {
            get
            {
                if (!_eventTypeInited)
                {
                    _eventTypeInited = true;
                    _eventType = this.PEFileToObjectModel.GetEventType(this);
                }
                return _eventType;
            }
        }

        #region IEventDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get { return this.PEFileToObjectModel.GetEventAccessorMethods(this); }
        }

        public IMethodReference Adder
        {
            get
            {
                if (this.AdderMethod == Dummy.Method) return Dummy.MethodReference;
                return this.AdderMethod;
            }
        }

        public IMethodReference/*?*/ Caller
        {
            get { return this.FireMethod; }
        }

        public bool IsRuntimeSpecial
        {
            get { return (_eventFlags & EventFlags.RTSpecialNameReserved) == EventFlags.RTSpecialNameReserved; }
        }

        public bool IsSpecialName
        {
            get { return (_eventFlags & EventFlags.SpecialNameImpl) == EventFlags.SpecialNameImpl; }
        }

        public IMethodReference Remover
        {
            get
            {
                if (this.RemoverMethod == Dummy.Method) return Dummy.MethodReference;
                return this.RemoverMethod;
            }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.EventType;
                if (moduleTypeRef == null) return Dummy.TypeReference;
                return moduleTypeRef;
            }
        }

        #endregion IEventDefinition Members
    }

    internal sealed class PropertyDefinition : TypeMember, IPropertyDefinition
    {
        internal readonly uint PropertyRowId;
        private PropertyFlags _propertyFlags;
        private byte _firstSignatureByte;
        private EnumerableArrayWrapper<CustomModifier, ICustomModifier>/*?*/ _returnModuleCustomModifiers;
        private IModuleTypeReference/*?*/ _returnType;
        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition>/*?*/ _moduleParameters;
        private MethodDefinition/*?*/ _getterMethod;
        private MethodDefinition/*?*/ _setterMethod;
        private TypeMemberVisibility _visibility;
        //^ invariant this.ReturnModuleCustomModifiers != null ==> this.ReturnType != null;
        //^ invariant this.ReturnModuleCustomModifiers != null ==> this.ModuleParameters != null;

        //^ [NotDelayed]
        internal PropertyDefinition(
          PEFileToObjectModel peFileToObjectModel,
          IName memberName,
          TypeBase parentModuleType,
          uint propertyRowId,
          PropertyFlags propertyFlags
        )
          : base(peFileToObjectModel, memberName, parentModuleType)
        {
            this.PropertyRowId = propertyRowId;
            _propertyFlags = propertyFlags;
            _visibility = TypeMemberVisibility.Mask;
        }

        public override ITypeDefinitionMember SpecializeTypeDefinitionMemberInstance(
          GenericTypeInstance genericTypeInstance
        )
        {
            Debug.Assert(genericTypeInstance.RawTemplateModuleType == this.OwningModuleType);
            return new GenericTypeInstanceProperty(genericTypeInstance, this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.Property | this.PropertyRowId; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override TypeMemberVisibility Visibility
        {
            get
            {
                if (_visibility == TypeMemberVisibility.Mask)
                {
                    MethodDefinition/*?*/ getterMethod = this.GetterMethod;
                    MethodDefinition/*?*/ setterMethod = this.SetterMethod;
                    TypeMemberVisibility getterVisibility = getterMethod == null ? TypeMemberVisibility.Other : getterMethod.Visibility;
                    TypeMemberVisibility setterVisibility = setterMethod == null ? TypeMemberVisibility.Other : setterMethod.Visibility;
                    _visibility = TypeCache.LeastUpperBound(getterVisibility, setterVisibility);
                }
                return _visibility;
            }
        }

        //  Half of the double check lock. Other half done by the caller...
        private void InitPropertySignature()
        //^ ensures this.ReturnModuleCustomModifiers != null;
        {
            PropertySignatureConverter propertySignature = this.PEFileToObjectModel.GetPropertySignature(this);
            _firstSignatureByte = propertySignature.FirstByte;
            _returnModuleCustomModifiers = propertySignature.ReturnCustomModifiers;
            _returnType = propertySignature.ReturnTypeReference;
            _moduleParameters = propertySignature.Parameters;
            if (propertySignature.ReturnValueIsByReference)
                _propertyFlags |= PropertyFlags.ReturnValueIsByReference;
        }

        internal EnumerableArrayWrapper<CustomModifier, ICustomModifier> ReturnModuleCustomModifiers
        {
            get
            {
                if (_returnModuleCustomModifiers == null)
                {
                    this.InitPropertySignature();
                }
                //^ assert this.returnModuleCustomModifiers != null;
                return _returnModuleCustomModifiers;
            }
        }

        internal IModuleTypeReference ReturnType
        {
            get
            {
                if (_returnModuleCustomModifiers == null)
                {
                    this.InitPropertySignature();
                }
                //^ assert this.returnType != null;
                return _returnType;
            }
        }

        internal EnumerableArrayWrapper<IModuleParameter, IParameterDefinition> ModuleParameters
        {
            get
            {
                if (_returnModuleCustomModifiers == null)
                {
                    this.InitPropertySignature();
                }
                //^ assert this.moduleParameters != null;
                return _moduleParameters;
            }
        }

        internal MethodDefinition/*?*/ GetterMethod
        {
            get
            {
                if ((_propertyFlags & PropertyFlags.GetterLoaded) != PropertyFlags.GetterLoaded)
                {
                    _getterMethod = this.PEFileToObjectModel.GetPropertyGetterOrSetterMethod(this, MethodSemanticsFlags.Getter);
                    _propertyFlags |= PropertyFlags.GetterLoaded;
                }
                return _getterMethod;
            }
        }

        internal MethodDefinition/*?*/ SetterMethod
        {
            get
            {
                if ((_propertyFlags & PropertyFlags.SetterLoaded) != PropertyFlags.SetterLoaded)
                {
                    _setterMethod = this.PEFileToObjectModel.GetPropertyGetterOrSetterMethod(this, MethodSemanticsFlags.Setter);
                    _propertyFlags |= PropertyFlags.SetterLoaded;
                }
                return _setterMethod;
            }
        }

        #region IPropertyDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get { return this.PEFileToObjectModel.GetPropertyAccessorMethods(this); }
        }

        public IMetadataConstant DefaultValue
        {
            get { return this.PEFileToObjectModel.GetDefaultValue(this); }
        }

        public IMethodReference/*?*/ Getter
        {
            get { return this.GetterMethod; }
        }

        public bool HasDefaultValue
        {
            get { return (_propertyFlags & PropertyFlags.HasDefaultReserved) == PropertyFlags.HasDefaultReserved; }
        }

        public bool IsRuntimeSpecial
        {
            get { return (_propertyFlags & PropertyFlags.RTSpecialNameReserved) == PropertyFlags.RTSpecialNameReserved; }
        }

        public bool IsSpecialName
        {
            get { return (_propertyFlags & PropertyFlags.SpecialNameImpl) == PropertyFlags.SpecialNameImpl; }
        }

        public IMethodReference/*?*/ Setter
        {
            get { return this.SetterMethod; }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get
            {
                return this.ModuleParameters;
            }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IPropertyDefinition Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get
            {
                return IteratorHelper.GetConversionEnumerable<IParameterDefinition, IParameterTypeInformation>(this.Parameters);
            }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get
            {
                return this.ReturnModuleCustomModifiers;
            }
        }

        public bool ReturnValueIsByRef
        {
            get
            {
                if (_returnModuleCustomModifiers == null)
                {
                    this.InitPropertySignature();
                }
                return (_propertyFlags & PropertyFlags.ReturnValueIsByReference) != 0;
            }
        }

        public bool ReturnValueIsModified
        {
            get { return this.ReturnModuleCustomModifiers.RawArray.Length > 0; }
        }

        public ITypeReference Type
        {
            get
            {
                if (this.ReturnType == null)
                {
                    //TODO: error
                    return Dummy.TypeReference;
                }
                return this.ReturnType;
            }
        }

        public CallingConvention CallingConvention
        {
            get
            {
                if (_returnModuleCustomModifiers == null)
                {
                    this.InitPropertySignature();
                }
                return (CallingConvention)(_firstSignatureByte & ~0x08);
            }
        }

        #endregion ISignature Members

        #region IMetadataConstantContainer

        IMetadataConstant IMetadataConstantContainer.Constant
        {
            get { return this.DefaultValue; }
        }

        #endregion IMetadataConstantContainer
    }

    #endregion TypeMember Level Object Model

    #region Generic TypeMember Level Object Model

    internal abstract class GenericTypeInstanceMember : ITypeDefinitionMember
    {
        protected readonly GenericTypeInstance OwningModuleGenericTypeInstance;

        protected GenericTypeInstanceMember(
          GenericTypeInstance owningModuleGenericTypeInstance
        )
        {
            this.OwningModuleGenericTypeInstance = owningModuleGenericTypeInstance;
        }

        internal abstract TypeMember RawTemplateModuleTypeMember { get; }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMemberSignature(this, NameFormattingOptions.None);
        }

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return this.OwningModuleGenericTypeInstance; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return this.RawTemplateModuleTypeMember.Visibility; }
        }

        /// <summary>
        /// Calls the visitor.Visit(T) method where T is the most derived object model node interface type implemented by the concrete type
        /// of the object implementing IDoubleDispatcher. The dispatch method does not invoke Dispatch on any child objects. If child traversal
        /// is desired, the implementations of the Visit methods should do the subsequent dispatching.
        /// </summary>
        public abstract void Dispatch(IMetadataVisitor visitor);

        #endregion ITypeDefinitionMember Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return this.ContainingTypeDefinition; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return this.OwningModuleGenericTypeInstance; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return this.RawTemplateModuleTypeMember.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return this.RawTemplateModuleTypeMember.Attributes; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IDefinition Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return this.OwningModuleGenericTypeInstance; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members
    }

    internal sealed class GenericTypeInstanceField : GenericTypeInstanceMember, IModuleFieldReference, ISpecializedFieldDefinition
    {
        private readonly FieldDefinition _rawTemplateModuleField;
        private bool _fieldTypeInited;
        private IModuleTypeReference/*?*/ _fieldType;

        internal GenericTypeInstanceField(
          GenericTypeInstance owningModuleGenericTypeInstance,
          FieldDefinition rawTemplateModuleField
        )
          : base(owningModuleGenericTypeInstance)
        {
            _rawTemplateModuleField = rawTemplateModuleField;
        }

        internal override TypeMember RawTemplateModuleTypeMember
        {
            get { return _rawTemplateModuleField; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region IFieldDefinition Members

        public uint BitLength
        {
            get { return _rawTemplateModuleField.BitLength; }
        }

        public bool IsBitField
        {
            get { return _rawTemplateModuleField.IsBitField; }
        }

        public bool IsCompileTimeConstant
        {
            get { return _rawTemplateModuleField.IsCompileTimeConstant; }
        }

        public bool IsMapped
        {
            get { return _rawTemplateModuleField.IsMapped; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return _rawTemplateModuleField.IsMarshalledExplicitly; }
        }

        public bool IsNotSerialized
        {
            get { return _rawTemplateModuleField.IsNotSerialized; }
        }

        public bool IsReadOnly
        {
            get { return _rawTemplateModuleField.IsReadOnly; }
        }

        public bool IsRuntimeSpecial
        {
            get { return _rawTemplateModuleField.IsRuntimeSpecial; }
        }

        public bool IsSpecialName
        {
            get { return _rawTemplateModuleField.IsSpecialName; }
        }

        public bool IsStatic
        {
            get { return _rawTemplateModuleField.IsStatic; }
        }

        public uint Offset
        {
            get { return _rawTemplateModuleField.Offset; }
        }

        public int SequenceNumber
        {
            get { return _rawTemplateModuleField.SequenceNumber; }
        }

        public IMetadataConstant CompileTimeValue
        {
            get { return _rawTemplateModuleField.CompileTimeValue; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get { return _rawTemplateModuleField.MarshallingInformation; }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ moduleFieldType = this.FieldType;
                if (moduleFieldType == null) return Dummy.TypeReference;
                return moduleFieldType;
            }
        }

        public ISectionBlock FieldMapping
        {
            get { return _rawTemplateModuleField.FieldMapping; }
        }

        #endregion IFieldDefinition Members

        #region IModuleMemberReference Members

        public IModuleTypeReference/*?*/ OwningTypeReference
        {
            get { return this.OwningModuleGenericTypeInstance; }
        }

        #endregion IModuleMemberReference Members

        #region ISpecializedFieldDefinition Members

        public IFieldDefinition UnspecializedVersion
        {
            get { return _rawTemplateModuleField; }
        }

        #endregion ISpecializedFieldDefinition Members

        #region IModuleFieldReference Members

        public IModuleTypeReference/*?*/ FieldType
        {
            get
            {
                if (!_fieldTypeInited)
                {
                    _fieldTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = _rawTemplateModuleField.FieldType;
                    if (moduleTypeRef != null)
                        _fieldType = moduleTypeRef.SpecializeTypeInstance(this.OwningModuleGenericTypeInstance);
                }
                return _fieldType;
            }
        }

        #endregion IModuleFieldReference Members

        #region IFieldReference Members

        public IFieldDefinition ResolvedField
        {
            get { return this; }
        }

        #endregion IFieldReference Members

        #region IMetadataConstantContainer

        IMetadataConstant IMetadataConstantContainer.Constant
        {
            get { return this.CompileTimeValue; }
        }

        #endregion IMetadataConstantContainer
    }

    internal abstract class GenericTypeInstanceMethod : GenericTypeInstanceMember, ISpecializedMethodDefinition, IModuleMethodReference
    {
        private bool _returnTypeInited;
        private IModuleTypeReference/*?*/ _returnType;
        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition>/*?*/ _moduleParameters;

        internal GenericTypeInstanceMethod(
          GenericTypeInstance owningModuleGenericTypeInstance
        )
          : base(owningModuleGenericTypeInstance)
        {
        }

        internal abstract MethodDefinition RawTemplateModuleMethod { get; }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.Signature | NameFormattingOptions.TypeParameters);
        }

        #region IMethodDefinition Members

        public bool AcceptsExtraArguments
        {
            get { return this.RawTemplateModuleMethod.AcceptsExtraArguments; }
        }

        public IMethodBody Body
        {
            get { return Dummy.MethodBody; }
        }

        public abstract IEnumerable<IGenericMethodParameter> GenericParameters { get; }

        public abstract ushort GenericParameterCount { get; }

        public bool HasDeclarativeSecurity
        {
            get { return this.RawTemplateModuleMethod.HasDeclarativeSecurity; }
        }

        public bool HasExplicitThisParameter
        {
            get { return this.RawTemplateModuleMethod.HasExplicitThisParameter; }
        }

        public bool IsAbstract
        {
            get { return this.RawTemplateModuleMethod.IsAbstract; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return this.RawTemplateModuleMethod.IsAccessCheckedOnOverride; }
        }

        public bool IsCil
        {
            get { return this.RawTemplateModuleMethod.IsCil; }
        }

        public bool IsExternal
        {
            get { return this.RawTemplateModuleMethod.IsExternal; }
        }

        public bool IsForwardReference
        {
            get { return this.RawTemplateModuleMethod.IsForwardReference; }
        }

        public abstract bool IsGeneric { get; }

        public bool IsHiddenBySignature
        {
            get { return this.RawTemplateModuleMethod.IsHiddenBySignature; }
        }

        public bool IsNativeCode
        {
            get { return this.RawTemplateModuleMethod.IsNativeCode; }
        }

        public bool IsNewSlot
        {
            get { return this.RawTemplateModuleMethod.IsNewSlot; }
        }

        public bool IsNeverInlined
        {
            get { return this.RawTemplateModuleMethod.IsNeverInlined; }
        }

        public bool IsNeverOptimized
        {
            get { return this.RawTemplateModuleMethod.IsNeverOptimized; }
        }

        public bool IsPlatformInvoke
        {
            get { return this.RawTemplateModuleMethod.IsPlatformInvoke; }
        }

        public bool IsRuntimeImplemented
        {
            get { return this.RawTemplateModuleMethod.IsRuntimeImplemented; }
        }

        public bool IsRuntimeInternal
        {
            get { return this.RawTemplateModuleMethod.IsRuntimeInternal; }
        }

        public bool IsRuntimeSpecial
        {
            get { return this.RawTemplateModuleMethod.IsRuntimeSpecial; }
        }

        public bool IsSealed
        {
            get { return this.RawTemplateModuleMethod.IsSealed; }
        }

        public bool IsSpecialName
        {
            get { return this.RawTemplateModuleMethod.IsSpecialName; }
        }

        public bool IsStatic
        {
            get { return this.RawTemplateModuleMethod.IsStatic; }
        }

        public bool IsSynchronized
        {
            get { return this.RawTemplateModuleMethod.IsSynchronized; }
        }

        public bool IsVirtual
        {
            get { return this.RawTemplateModuleMethod.IsVirtual; }
        }

        public bool IsUnmanaged
        {
            get { return this.RawTemplateModuleMethod.IsUnmanaged; }
        }

        public bool PreserveSignature
        {
            get { return this.RawTemplateModuleMethod.PreserveSignature; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return this.RawTemplateModuleMethod.PlatformInvokeData; }
        }

        public bool RequiresSecurityObject
        {
            get { return this.RawTemplateModuleMethod.RequiresSecurityObject; }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get { return this.RawTemplateModuleMethod.ReturnValueIsMarshalledExplicitly; }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get { return this.RawTemplateModuleMethod.ReturnValueMarshallingInformation; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return this.RawTemplateModuleMethod.SecurityAttributes; }
        }

        public bool IsConstructor
        {
            get { return this.RawTemplateModuleMethod.IsConstructor; }
        }

        public bool IsStaticConstructor
        {
            get { return this.RawTemplateModuleMethod.IsStaticConstructor; }
        }

        #endregion IMethodDefinition Members

        #region ISignature Members

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return this.RequiredModuleParameters; }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return this.RawTemplateModuleMethod.ReturnValueAttributes; }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return this.RawTemplateModuleMethod.ReturnValueCustomModifiers; }
        }

        public bool ReturnValueIsByRef
        {
            get { return this.RawTemplateModuleMethod.ReturnValueIsByRef; }
        }

        public bool ReturnValueIsModified
        {
            get { return this.RawTemplateModuleMethod.ReturnValueIsModified; }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ typeRef = this.ReturnType;
                if (typeRef == null) return Dummy.TypeReference;
                return typeRef;
            }
        }

        public CallingConvention CallingConvention
        {
            get { return this.RawTemplateModuleMethod.CallingConvention; }
        }

        #endregion ISignature Members

        #region ISpecializedMethodDefinition Members

        public IMethodDefinition UnspecializedVersion
        {
            get { return this.RawTemplateModuleMethod; }
        }

        #endregion ISpecializedMethodDefinition Members

        #region IModuleMethodReference Members

        public IModuleTypeReference/*?*/ OwningTypeReference
        {
            get { return this.OwningModuleGenericTypeInstance; }
        }

        public EnumerableArrayWrapper<CustomModifier, ICustomModifier> ReturnCustomModifiers
        {
            get { return this.RawTemplateModuleMethod.ReturnCustomModifiers; }
        }

        public IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (!_returnTypeInited)
                {
                    _returnTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = this.RawTemplateModuleMethod.ReturnType;
                    if (moduleTypeRef != null)
                        _returnType = moduleTypeRef.SpecializeTypeInstance(this.OwningModuleGenericTypeInstance);
                }
                return _returnType;
            }
        }

        public EnumerableArrayWrapper<IModuleParameter, IParameterDefinition> RequiredModuleParameters
        {
            get
            {
                if (_moduleParameters == null)
                {
                    _moduleParameters = TypeCache.SpecializeInstantiatedParameters(this, this.RawTemplateModuleMethod.RequiredModuleParameters, this.OwningModuleGenericTypeInstance);
                }
                return _moduleParameters;
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> RequiredModuleParameterInfos
        {
            get
            {
                return new EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation>(
                  this.RequiredModuleParameters.RawArray, Dummy.ParameterTypeInformation);
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> VarArgModuleParameterInfos
        {
            get { return TypeCache.EmptyParameterInfoArray; }
        }

        public bool IsReturnByReference
        {
            get { return this.RawTemplateModuleMethod.IsReturnByReference; }
        }

        #endregion IModuleMethodReference Members



        #region IMethodReference Members

        public uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                {
                    _internedKey = this.RawTemplateModuleMethod.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetMethodInternedKey(this);
                }
                return _internedKey;
            }
        }

        private uint _internedKey;

        public IMethodDefinition ResolvedMethod
        {
            get { return this; }
        }

        public IEnumerable<IParameterTypeInformation> ExtraParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        public ushort ParameterCount
        {
            get { return this.RawTemplateModuleMethod.ParameterCount; }
        }

        #endregion IMethodReference Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetConversionEnumerable<IParameterDefinition, IParameterTypeInformation>(this.Parameters); }
        }

        #endregion ISignature Members
    }

    internal sealed class GenericTypeInstanceNonGenericMethod : GenericTypeInstanceMethod
    {
        private readonly NonGenericMethod _rawTemplateModuleNonGenericMethod;

        internal GenericTypeInstanceNonGenericMethod(
          GenericTypeInstance owningModuleGenericTypeInstance,
          NonGenericMethod rawTemplateModuleNonGenericMethod
        )
          : base(owningModuleGenericTypeInstance)
        {
            _rawTemplateModuleNonGenericMethod = rawTemplateModuleNonGenericMethod;
        }

        internal override MethodDefinition RawTemplateModuleMethod
        {
            get { return _rawTemplateModuleNonGenericMethod; }
        }

        internal override TypeMember RawTemplateModuleTypeMember
        {
            get { return _rawTemplateModuleNonGenericMethod; }
        }

        public override IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericMethodParameter>(); }
        }

        public override ushort GenericParameterCount
        {
            get { return 0; }
        }

        public override bool IsGeneric
        {
            get { return false; }
        }
    }

    internal sealed class GenericTypeInstanceGenericMethod : GenericTypeInstanceMethod, IModuleGenericMethod
    {
        private readonly GenericMethod _rawTemplateModuleGenericMethod;
        private readonly EnumerableArrayWrapper<IModuleGenericMethodParameter, IGenericMethodParameter> _genericMethodParameters;

        //^ [NotDelayed]
        internal GenericTypeInstanceGenericMethod(
          GenericTypeInstance owningModuleGenericTypeInstance,
          GenericMethod rawTemplateModuleGenericMethod
        )
          : base(owningModuleGenericTypeInstance)
        {
            _rawTemplateModuleGenericMethod = rawTemplateModuleGenericMethod;
            //^ this.GenericMethodParameters = TypeCache.EmptyGenericMethodParameters;
            //^ base;
            uint genericParams = rawTemplateModuleGenericMethod.GenericMethodParameterCardinality;
            IModuleGenericMethodParameter[] specializedGenericParamArray = new IModuleGenericMethodParameter[genericParams];
            for (uint i = 0; i < genericParams; ++i)
            {
                uint genericRowId = rawTemplateModuleGenericMethod.GenericParamRowIdStart + i;
                GenericMethodParameter/*?*/ mgmp = rawTemplateModuleGenericMethod.PEFileToObjectModel.GetGenericMethodParamAtRow(genericRowId, rawTemplateModuleGenericMethod);
                if (mgmp != null)
                    specializedGenericParamArray[i] = new TypeSpecializedGenericMethodParameter(owningModuleGenericTypeInstance, this, mgmp);
            }
            //^ NonNullType.AssertInitialized(specializedGenericParamArray);
            _genericMethodParameters = new EnumerableArrayWrapper<IModuleGenericMethodParameter, IGenericMethodParameter>(specializedGenericParamArray, Dummy.GenericMethodParameter);
        }

        internal override MethodDefinition RawTemplateModuleMethod
        {
            get { return _rawTemplateModuleGenericMethod; }
        }

        internal override TypeMember RawTemplateModuleTypeMember
        {
            get { return _rawTemplateModuleGenericMethod; }
        }

        #region IModuleGenericMethod Members

        public ushort GenericMethodParameterCardinality
        {
            get { return _rawTemplateModuleGenericMethod.GenericMethodParameterCardinality; }
        }

        public IModuleTypeReference/*?*/ GetGenericMethodParameterFromOrdinal(ushort genericParamOrdinal)
        {
            if (genericParamOrdinal >= _genericMethodParameters.RawArray.Length)
                return null;
            return _genericMethodParameters.RawArray[genericParamOrdinal];
        }

        #endregion IModuleGenericMethod Members

        public override IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return _genericMethodParameters; }
        }

        public override ushort GenericParameterCount
        {
            get { return _rawTemplateModuleGenericMethod.GenericParameterCount; }
        }

        public override bool IsGeneric
        {
            get { return _rawTemplateModuleGenericMethod.IsGeneric; }
        }
    }

    internal sealed class GenericTypeInstanceEvent : GenericTypeInstanceMember, ISpecializedEventDefinition
    {
        private readonly EventDefinition _rawTemplateModuleEvent;
        private bool _eventTypeInited;
        private IModuleTypeReference/*?*/ _eventType;
        private IMethodDefinition/*?*/ _adderMethod;
        private IMethodDefinition/*?*/ _removerMethod;
        private IMethodDefinition/*?*/ _fireMethod;
        private EventFlags _eventFlags;
        //^ invariant ((this.EventFlags & EventFlags.AdderLoaded) == EventFlags.AdderLoaded) ==> this.adderMethod != null;
        //^ invariant ((this.EventFlags & EventFlags.RemoverLoaded) == EventFlags.RemoverLoaded) ==> this.removerMethod != null;

        internal GenericTypeInstanceEvent(
          GenericTypeInstance owningModuleGenericTypeInstance,
          EventDefinition rawTemplateModuleEvent
        )
          : base(owningModuleGenericTypeInstance)
        {
            _rawTemplateModuleEvent = rawTemplateModuleEvent;
        }

        internal override TypeMember RawTemplateModuleTypeMember
        {
            get { return _rawTemplateModuleEvent; }
        }

        internal IModuleTypeReference/*?*/ EventType
        {
            get
            {
                if (!_eventTypeInited)
                {
                    _eventTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = _rawTemplateModuleEvent.EventType;
                    if (moduleTypeRef != null)
                        _eventType = moduleTypeRef.SpecializeTypeInstance(this.OwningModuleGenericTypeInstance);
                }
                return _eventType;
            }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region IEventDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get
            {
                foreach (IMethodDefinition otherMethIter in _rawTemplateModuleEvent.Accessors)
                {
                    IMethodDefinition/*?*/ mappedMeth = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(((object)otherMethIter) as TypeMember) as IMethodDefinition;
                    if (mappedMeth == null)
                        continue;
                    yield return mappedMeth;
                }
            }
        }

        public IMethodReference Adder
        {
            get
            {
                if ((_eventFlags & EventFlags.AdderLoaded) != EventFlags.AdderLoaded)
                {
                    _adderMethod = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(((object)_rawTemplateModuleEvent.AdderMethod) as TypeMember) as IMethodDefinition;
                    if (_adderMethod == null)
                    {
                        //  MDError
                        _adderMethod = Dummy.Method;
                    }
                    _eventFlags |= EventFlags.AdderLoaded;
                }
                //^ assert this.adderMethod != null;
                return _adderMethod;
            }
        }

        public IMethodReference/*?*/ Caller
        {
            get
            {
                if ((_eventFlags & EventFlags.FireLoaded) != EventFlags.FireLoaded)
                {
                    _fireMethod = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(((object)_rawTemplateModuleEvent.FireMethod) as TypeMember) as IMethodDefinition;
                    _eventFlags |= EventFlags.FireLoaded;
                }
                return _fireMethod;
            }
        }

        public bool IsRuntimeSpecial
        {
            get { return _rawTemplateModuleEvent.IsRuntimeSpecial; }
        }

        public bool IsSpecialName
        {
            get { return _rawTemplateModuleEvent.IsSpecialName; }
        }

        public IMethodReference Remover
        {
            get
            {
                if ((_eventFlags & EventFlags.RemoverLoaded) != EventFlags.RemoverLoaded)
                {
                    _removerMethod = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(((object)_rawTemplateModuleEvent.RemoverMethod) as TypeMember) as IMethodDefinition;
                    if (_removerMethod == null)
                    {
                        //  MDError
                        _removerMethod = Dummy.Method;
                    }
                    _eventFlags |= EventFlags.RemoverLoaded;
                }
                //^ assert this.removerMethod != null;
                return _removerMethod;
            }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.EventType;
                if (moduleTypeRef == null) return Dummy.TypeReference;
                return moduleTypeRef;
            }
        }

        #endregion IEventDefinition Members

        #region ISpecializedEventDefinition Members

        public IEventDefinition UnspecializedVersion
        {
            get { return _rawTemplateModuleEvent; }
        }

        #endregion ISpecializedEventDefinition Members
    }

    internal sealed class GenericTypeInstanceProperty : GenericTypeInstanceMember, ISpecializedPropertyDefinition
    {
        private readonly PropertyDefinition _rawTemplateModuleProperty;
        private bool _returnTypeInited;
        private IModuleTypeReference/*?*/ _returnType;
        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition>/*?*/ _moduleParameters;
        private IMethodDefinition/*?*/ _getterMethod;
        private IMethodDefinition/*?*/ _setterMethod;
        private PropertyFlags _propertyFlags;

        internal GenericTypeInstanceProperty(
          GenericTypeInstance owningModuleGenericTypeInstance,
          PropertyDefinition rawTemplateModuleProperty
        )
          : base(owningModuleGenericTypeInstance)
        {
            _rawTemplateModuleProperty = rawTemplateModuleProperty;
        }

        internal override TypeMember RawTemplateModuleTypeMember
        {
            get { return _rawTemplateModuleProperty; }
        }

        internal IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (!_returnTypeInited)
                {
                    _returnTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = _rawTemplateModuleProperty.ReturnType;
                    if (moduleTypeRef != null)
                        _returnType = moduleTypeRef.SpecializeTypeInstance(this.OwningModuleGenericTypeInstance);
                }
                return _returnType;
            }
        }

        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition> ModuleParameters
        {
            get
            {
                if (_moduleParameters == null)
                {
                    _moduleParameters = TypeCache.SpecializeInstantiatedParameters(this, _rawTemplateModuleProperty.ModuleParameters, this.OwningModuleGenericTypeInstance);
                }
                return _moduleParameters;
            }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        #region IPropertyDefinition Members

        public IMetadataConstant DefaultValue
        {
            get { return _rawTemplateModuleProperty.DefaultValue; }
        }

        public IMethodReference/*?*/ Getter
        {
            get
            {
                if ((_propertyFlags & PropertyFlags.GetterLoaded) != PropertyFlags.GetterLoaded)
                {
                    _getterMethod = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(_rawTemplateModuleProperty.GetterMethod as TypeMember) as IMethodDefinition;
                    _propertyFlags |= PropertyFlags.GetterLoaded;
                }
                return _getterMethod;
            }
        }

        public bool HasDefaultValue
        {
            get { return _rawTemplateModuleProperty.HasDefaultValue; }
        }

        public bool IsRuntimeSpecial
        {
            get { return _rawTemplateModuleProperty.IsRuntimeSpecial; }
        }

        public bool IsSpecialName
        {
            get { return _rawTemplateModuleProperty.IsSpecialName; }
        }

        public IEnumerable<IMethodReference> Accessors
        {
            get
            {
                foreach (IMethodReference otherMethIter in _rawTemplateModuleProperty.Accessors)
                {
                    IMethodReference/*?*/ mappedMeth = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(((object)otherMethIter) as TypeMember) as IMethodReference;
                    if (mappedMeth == null)
                        continue;
                    yield return mappedMeth;
                }
            }
        }

        public IMethodReference/*?*/ Setter
        {
            get
            {
                if ((_propertyFlags & PropertyFlags.SetterLoaded) != PropertyFlags.SetterLoaded)
                {
                    _setterMethod = this.OwningModuleGenericTypeInstance.FindInstantiatedMemberFor(_rawTemplateModuleProperty.SetterMethod as TypeMember) as IMethodDefinition;
                    _propertyFlags |= PropertyFlags.SetterLoaded;
                }
                return _setterMethod;
            }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return _rawTemplateModuleProperty.ReturnValueAttributes; }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return this.ModuleParameters; }
        }

        #endregion IPropertyDefinition Members

        #region ISignature Members

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return _rawTemplateModuleProperty.ReturnValueCustomModifiers; }
        }

        public bool ReturnValueIsByRef
        {
            get { return _rawTemplateModuleProperty.ReturnValueIsByRef; }
        }

        public bool ReturnValueIsModified
        {
            get { return _rawTemplateModuleProperty.ReturnValueIsModified; }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.ReturnType;
                if (moduleTypeRef == null) return Dummy.TypeReference;
                return moduleTypeRef;
            }
        }

        public CallingConvention CallingConvention
        {
            get { return _rawTemplateModuleProperty.CallingConvention; }
        }

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetConversionEnumerable<IParameterDefinition, IParameterTypeInformation>(this.Parameters); }
        }

        #endregion ISignature Members

        #region ISpecializedPropertyDefinition Members

        public IPropertyDefinition UnspecializedVersion
        {
            get { return _rawTemplateModuleProperty; }
        }

        #endregion ISpecializedPropertyDefinition Members

        #region IMetadataConstantContainer

        IMetadataConstant IMetadataConstantContainer.Constant
        {
            get { return this.DefaultValue; }
        }

        #endregion IMetadataConstantContainer
    }

    #endregion Generic TypeMember Level Object Model

    #region Generic Method level object model

    internal sealed class GenericMethodInstanceReference : MetadataObject, IModuleGenericMethodInstance
    {
        internal readonly uint MethodSpecToken;
        internal readonly IModuleMethodReference ModuleMethodReference;
        internal readonly EnumerableArrayWrapper<IModuleTypeReference/*?*/, ITypeReference> CummulativeTypeArguments;
        private IMethodDefinition/*?*/ _resolvedGenericMethodInstance;
        private EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation>/*?*/ _moduleParameters;
        private bool _returnTypeInited;
        private IModuleTypeReference/*?*/ _returnType;

        internal GenericMethodInstanceReference(
          PEFileToObjectModel peFileToObjectModel,
          uint methodSpecToken,
          IModuleMethodReference moduleMethodReference,
          EnumerableArrayWrapper<IModuleTypeReference/*?*/, ITypeReference> cummulativeTypeArguments
        )
          : base(peFileToObjectModel)
        {
            this.MethodSpecToken = methodSpecToken;
            this.ModuleMethodReference = moduleMethodReference;
            this.CummulativeTypeArguments = cummulativeTypeArguments;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (!_returnTypeInited)
                {
                    _returnTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = this.ModuleMethodReference.ReturnType;
                    if (moduleTypeRef != null)
                        _returnType = moduleTypeRef.SpecializeMethodInstance(this);
                }
                return _returnType;
            }
        }

        internal EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> ModuleParameters
        {
            get
            {
                if (_moduleParameters == null)
                {
                    _moduleParameters = TypeCache.SpecializeInstantiatedParameters(this, this.ModuleMethodReference.RequiredModuleParameterInfos, this);
                }
                return _moduleParameters;
            }
        }

        internal override uint TokenValue
        {
            get { return this.MethodSpecToken; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.TypeParameters | NameFormattingOptions.Signature);
        }

        #region IModuleGenericMethodInstance Members

        public IModuleMethodReference RawGenericTemplate
        {
            get { return this.ModuleMethodReference; }
        }

        public ushort GenericMethodArgumentCardinality
        {
            get { return (ushort)this.CummulativeTypeArguments.RawArray.Length; }
        }

        public IModuleTypeReference/*?*/ GetGenericMethodArgumentFromOrdinal(ushort genericArgumentOrdinal)
        {
            IModuleTypeReference/*?*/[] arr = this.CummulativeTypeArguments.RawArray;
            if (genericArgumentOrdinal >= arr.Length)
            {
                return null;
            }
            return arr[genericArgumentOrdinal];
        }

        PEFileToObjectModel IModuleGenericMethodInstance.PEFileToObjectModel
        {
            get { return this.PEFileToObjectModel; }
        }

        #endregion IModuleGenericMethodInstance Members

        #region IMethodReference Members

        public bool AcceptsExtraArguments
        {
            get { return (this.ModuleMethodReference.CallingConvention & (CallingConvention)0x7) == CallingConvention.ExtraArguments; }
        }

        public ushort GenericParameterCount
        {
            get { return this.ModuleMethodReference.GenericParameterCount; }
        }

        public uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                {
                    _internedKey = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetMethodInternedKey(this);
                }
                return _internedKey;
            }
        }

        private uint _internedKey;

        public bool IsGeneric
        {
            get { return this.ModuleMethodReference.GenericParameterCount > 0; }
        }

        public ushort ParameterCount
        {
            get { return this.ModuleMethodReference.ParameterCount; }
        }

        public IMethodDefinition ResolvedMethod
        {
            get
            {
                if (_resolvedGenericMethodInstance == null)
                {
                    IModuleGenericMethod/*?*/ moduleGenericMethod = this.ModuleMethodReference.ResolvedMethod as IModuleGenericMethod;
                    if (moduleGenericMethod != null)
                    {
                        _resolvedGenericMethodInstance = new GenericMethodInstance(this.PEFileToObjectModel, this, moduleGenericMethod);
                    }
                    else
                    {
                        //  Error
                        _resolvedGenericMethodInstance = Dummy.Method;
                    }
                }
                return _resolvedGenericMethodInstance;
            }
        }

        public IEnumerable<IParameterTypeInformation> ExtraParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion IMethodReference Members

        #region ISignature Members

        public CallingConvention CallingConvention
        {
            get { return this.ModuleMethodReference.CallingConvention; }
        }

        public IEnumerable<IParameterTypeInformation> Parameters
        {
            get { return this.ModuleParameters; }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return this.ModuleMethodReference.ReturnValueCustomModifiers; }
        }

        public bool ReturnValueIsByRef
        {
            get { return this.ModuleMethodReference.ReturnValueIsByRef; }
        }

        public bool ReturnValueIsModified
        {
            get { return this.ModuleMethodReference.ReturnValueIsModified; }
        }

        public ITypeReference Type
        {
            get { return this.ReturnType; }
        }

        #endregion ISignature Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return this.ModuleMethodReference.ContainingType; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this.ResolvedMethod; }
        }

        #endregion ITypeMemberReference Members

        #region INamedEntity Members

        public IName Name
        {
            get { return this.ModuleMethodReference.Name; }
        }

        #endregion INamedEntity Members

        #region IGenericMethodInstance Members

        public IEnumerable<ITypeReference> GenericArguments
        {
            get { return this.CummulativeTypeArguments; }
        }

        public IMethodReference GenericMethod
        {
            get { return this.ModuleMethodReference; }
        }

        #endregion IGenericMethodInstance Members
    }

    internal sealed class GenericMethodInstance : MetadataObject, IGenericMethodInstance, IModuleGenericMethodInstance
    {
        private readonly GenericMethodInstanceReference _genericMethodInstanceReference;
        private readonly IModuleGenericMethod _moduleGenericMethodTemplate;
        private bool _returnTypeInited;
        private IModuleTypeReference/*?*/ _returnType;
        private EnumerableArrayWrapper<IModuleParameter, IParameterDefinition>/*?*/ _moduleParameters;

        internal GenericMethodInstance(
          PEFileToObjectModel peFileToObjectModel,
          GenericMethodInstanceReference genericMethodInstanceReference,
          IModuleGenericMethod moduleGenericMethodTemplate
        )
          : base(peFileToObjectModel)
        {
            _genericMethodInstanceReference = genericMethodInstanceReference;
            _moduleGenericMethodTemplate = moduleGenericMethodTemplate;
        }

        internal override uint TokenValue
        {
            get { return 0xFFFFFFFF; }
        }

        internal IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (!_returnTypeInited)
                {
                    _returnTypeInited = true;
                    IModuleTypeReference/*?*/ moduleTypeRef = _moduleGenericMethodTemplate.ReturnType;
                    if (moduleTypeRef != null)
                        _returnType = moduleTypeRef.SpecializeMethodInstance(this);
                }
                return _returnType;
            }
        }

        internal EnumerableArrayWrapper<IModuleParameter, IParameterDefinition> ModuleParameters
        {
            get
            {
                if (_moduleParameters == null)
                {
                    _moduleParameters = TypeCache.SpecializeInstantiatedParameters(this, _moduleGenericMethodTemplate.RequiredModuleParameters, this);
                }
                return _moduleParameters;
            }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.Signature);
        }

        #region IGenericMethodInstance Members

        public IEnumerable<ITypeReference> GenericArguments
        {
            get { return _genericMethodInstanceReference.CummulativeTypeArguments; }
        }

        public IMethodReference GenericMethod
        {
            get { return _moduleGenericMethodTemplate; }
        }

        #endregion IGenericMethodInstance Members

        #region IMethodDefinition Members

        public bool AcceptsExtraArguments
        {
            get { return _moduleGenericMethodTemplate.AcceptsExtraArguments; }
        }

        public IMethodBody Body
        {
            get { return Dummy.MethodBody; }
        }

        public IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericMethodParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get { return 0; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return _moduleGenericMethodTemplate.HasDeclarativeSecurity; }
        }

        public bool HasExplicitThisParameter
        {
            get { return _moduleGenericMethodTemplate.HasExplicitThisParameter; }
        }

        public bool IsAbstract
        {
            get { return _moduleGenericMethodTemplate.IsAbstract; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return _moduleGenericMethodTemplate.IsAccessCheckedOnOverride; }
        }

        public bool IsCil
        {
            get { return _moduleGenericMethodTemplate.IsCil; }
        }

        public bool IsExternal
        {
            get { return _moduleGenericMethodTemplate.IsExternal; }
        }

        public bool IsForwardReference
        {
            get { return _moduleGenericMethodTemplate.IsForwardReference; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsHiddenBySignature
        {
            get { return _moduleGenericMethodTemplate.IsHiddenBySignature; }
        }

        public bool IsNativeCode
        {
            get { return _moduleGenericMethodTemplate.IsNativeCode; }
        }

        public bool IsNewSlot
        {
            get { return _moduleGenericMethodTemplate.IsNewSlot; }
        }

        public bool IsNeverInlined
        {
            get { return _moduleGenericMethodTemplate.IsNeverInlined; }
        }

        public bool IsNeverOptimized
        {
            get { return _moduleGenericMethodTemplate.IsNeverOptimized; }
        }

        public bool IsPlatformInvoke
        {
            get { return _moduleGenericMethodTemplate.IsPlatformInvoke; }
        }

        public bool IsRuntimeImplemented
        {
            get { return _moduleGenericMethodTemplate.IsRuntimeImplemented; }
        }

        public bool IsRuntimeInternal
        {
            get { return _moduleGenericMethodTemplate.IsRuntimeInternal; }
        }

        public bool IsRuntimeSpecial
        {
            get { return _moduleGenericMethodTemplate.IsRuntimeSpecial; }
        }

        public bool IsSealed
        {
            get { return _moduleGenericMethodTemplate.IsSealed; }
        }

        public bool IsSpecialName
        {
            get { return _moduleGenericMethodTemplate.IsSpecialName; }
        }

        public bool IsStatic
        {
            get { return _moduleGenericMethodTemplate.IsStatic; }
        }

        public bool IsSynchronized
        {
            get { return _moduleGenericMethodTemplate.IsSynchronized; }
        }

        public bool IsVirtual
        {
            get { return _moduleGenericMethodTemplate.IsVirtual; }
        }

        public bool IsUnmanaged
        {
            get { return _moduleGenericMethodTemplate.IsUnmanaged; }
        }

        public bool PreserveSignature
        {
            get { return _moduleGenericMethodTemplate.PreserveSignature; }
        }

        public bool RequiresSecurityObject
        {
            get { return _moduleGenericMethodTemplate.RequiresSecurityObject; }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get { return _moduleGenericMethodTemplate.ReturnValueIsMarshalledExplicitly; }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get { return _moduleGenericMethodTemplate.ReturnValueMarshallingInformation; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return _moduleGenericMethodTemplate.SecurityAttributes; }
        }

        public bool IsConstructor
        {
            get { return _moduleGenericMethodTemplate.IsConstructor; }
        }

        public bool IsStaticConstructor
        {
            get { return _moduleGenericMethodTemplate.IsStaticConstructor; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return _moduleGenericMethodTemplate.PlatformInvokeData; }
        }

        #endregion IMethodDefinition Members

        #region ISignature Members

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return this.ModuleParameters; }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return _moduleGenericMethodTemplate.ReturnValueAttributes; }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return _moduleGenericMethodTemplate.ReturnValueCustomModifiers; }
        }

        public bool ReturnValueIsByRef
        {
            get { return _moduleGenericMethodTemplate.ReturnValueIsByRef; }
        }

        public bool ReturnValueIsModified
        {
            get { return _moduleGenericMethodTemplate.ReturnValueIsModified; }
        }

        public ITypeReference Type
        {
            get
            {
                IModuleTypeReference/*?*/ moduleType = this.ReturnType;
                if (moduleType == null) return Dummy.TypeReference;
                return moduleType;
            }
        }

        public CallingConvention CallingConvention
        {
            get { return ((IModuleMethodReference)_moduleGenericMethodTemplate).CallingConvention; }
        }

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetConversionEnumerable<IParameterDefinition, IParameterTypeInformation>(this.Parameters); }
        }

        #endregion ISignature Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return _moduleGenericMethodTemplate.ContainingTypeDefinition; }
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit((IGenericMethodInstanceReference)this);
        }

        public TypeMemberVisibility Visibility
        {
            get { return _moduleGenericMethodTemplate.Visibility; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return _moduleGenericMethodTemplate.Container; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return ((INamedEntity)_moduleGenericMethodTemplate).Name; }
        }

        #endregion INamedEntity Members

        #region IReference Members

        public override IEnumerable<ICustomAttribute> Attributes
        {
            get { return _moduleGenericMethodTemplate.Attributes; }
        }

        public override IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return _moduleGenericMethodTemplate.ContainingScope; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region IMethodReference Members

        public uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                {
                    _internedKey = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetMethodInternedKey(this);
                }
                return _internedKey;
            }
        }

        private uint _internedKey;

        public ushort ParameterCount
        {
            get { return this.GenericMethod.ParameterCount; }
        }

        public IMethodDefinition ResolvedMethod
        {
            get { return this; }
        }

        public IEnumerable<IParameterTypeInformation> ExtraParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion IMethodReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return _genericMethodInstanceReference.ContainingType; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IModuleGenericMethodInstance Members

        public IModuleMethodReference RawGenericTemplate
        {
            get { return _genericMethodInstanceReference.RawGenericTemplate; }
        }

        public ushort GenericMethodArgumentCardinality
        {
            get { return (ushort)_genericMethodInstanceReference.CummulativeTypeArguments.RawArray.Length; }
        }

        public IModuleTypeReference/*?*/ GetGenericMethodArgumentFromOrdinal(ushort genericArgumentOrdinal)
        {
            IModuleTypeReference/*?*/[] arr = _genericMethodInstanceReference.CummulativeTypeArguments.RawArray;
            if (genericArgumentOrdinal >= arr.Length)
            {
                return null;
            }
            return arr[genericArgumentOrdinal];
        }

        PEFileToObjectModel IModuleGenericMethodInstance.PEFileToObjectModel
        {
            get { return this.PEFileToObjectModel; }
        }

        #endregion IModuleGenericMethodInstance Members
    }

    #endregion Generic Method level object model

    #region Member Ref level Object Model

    internal abstract class MemberReference : MetadataObject, IModuleMemberReference
    {
        internal readonly uint MemberRefRowId;
        internal readonly IName Name;
        internal readonly IModuleTypeReference/*?*/ ParentTypeReference;

        internal MemberReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleTypeReference/*?*/ parentTypeReference,
          IName name
        )
          : base(peFileToObjectModel)
        {
            this.MemberRefRowId = memberRefRowId;
            this.ParentTypeReference = parentTypeReference;
            this.Name = name;
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.MemberRef | this.MemberRefRowId; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMemberSignature(this, NameFormattingOptions.None);
        }

        #region IModuleMemberReference Members

        public IModuleTypeReference/*?*/ OwningTypeReference
        {
            get { return this.ParentTypeReference; }
        }

        #endregion IModuleMemberReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get
            {
                if (this.OwningTypeReference == null)
                    return Dummy.TypeReference;
                return this.OwningTypeReference;
            }
        }

        public abstract ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get;
        }

        #endregion ITypeMemberReference Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.Name; }
        }

        #endregion INamedEntity Members
    }

    internal class FieldReference : MemberReference, IModuleFieldReference
    {
        protected bool signatureLoaded;
        protected IModuleTypeReference/*?*/ typeReference;

        internal FieldReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleTypeReference/*?*/ parentTypeReference,
          IName name
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name)
        {
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected virtual void InitFieldSignature()
        //^ ensures this.signatureLoaded;
        {
            FieldSignatureConverter fieldSignature = this.PEFileToObjectModel.GetFieldRefSignature(this);
            this.typeReference = fieldSignature.TypeReference;
            this.signatureLoaded = true;
        }

        public override ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this.ResolvedField; }
        }

        #region IModuleFieldReference Members

        public IModuleTypeReference/*?*/ FieldType
        {
            get
            {
                if (!this.signatureLoaded)
                {
                    this.InitFieldSignature();
                }
                //^ assert this.typeReference != null;
                return this.typeReference;
            }
        }

        #endregion IModuleFieldReference Members

        #region IFieldReference Members

        public virtual IFieldDefinition ResolvedField
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Field;
                IModuleTypeDefAndRef/*?*/ moduleType = moduleTypeRef.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Field;
                return moduleType.ResolveFieldReference(this);
            }
        }

        public ITypeReference Type
        {
            get
            {
                ITypeReference/*?*/ result = this.FieldType;
                if (result == null) result = Dummy.TypeReference;
                return result;
            }
        }

        #endregion IFieldReference Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class GenericInstanceFieldReference : FieldReference, ISpecializedFieldReference
    {
        internal GenericInstanceFieldReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleGenericTypeInstance/*?*/ parentTypeReference,
          IName name
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name)
        {
            _unspecializedVersion = new FieldReference(peFileToObjectModel, memberRefRowId, parentTypeReference.ModuleGenericTypeReference, name);
        }

        protected override void InitFieldSignature()
        {
            FieldSignatureConverter fieldSignature = this.PEFileToObjectModel.GetFieldRefSignature(this);
            //^ assume this.ParentTypeReference is IModuleGenericTypeInstance; //gauranteed by constructor
            IModuleGenericTypeInstance moduleGenericTypeInstance = (IModuleGenericTypeInstance)this.ParentTypeReference;
            if (fieldSignature.TypeReference != null)
            {
                this.typeReference = fieldSignature.TypeReference.SpecializeTypeInstance(moduleGenericTypeInstance);
            }
        }

        public override IFieldDefinition ResolvedField
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Field;
                IModuleTypeDefAndRef/*?*/ moduleType = moduleTypeRef.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Field;
                return moduleType.ResolveFieldReference(_unspecializedVersion);
            }
        }

        #region ISpecializedFieldReference Members

        public IFieldReference UnspecializedVersion
        {
            get { return _unspecializedVersion; }
        }

        private readonly FieldReference _unspecializedVersion;

        #endregion ISpecializedFieldReference Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class SpecializedNestedTypeFieldReference : FieldReference, ISpecializedFieldReference
    {
        private IModuleSpecializedNestedTypeReference _specializedParentTypeReference;

        internal SpecializedNestedTypeFieldReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleTypeReference parentTypeReference,
          IModuleSpecializedNestedTypeReference specializedParentTypeReference,
          IName name
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name)
        {
            _unspecializedVersion = new FieldReference(peFileToObjectModel, memberRefRowId, specializedParentTypeReference.UnspecializedModuleType, name);
            _specializedParentTypeReference = specializedParentTypeReference;
        }

        protected override void InitFieldSignature()
        {
            FieldSignatureConverter fieldSignature = this.PEFileToObjectModel.GetFieldRefSignature(this);
            IModuleSpecializedNestedTypeReference/*?*/ neType = _specializedParentTypeReference;
            while (neType.ContainingType is IGenericTypeInstanceReference)
            {
                neType = neType.ContainingType as IModuleSpecializedNestedTypeReference;
                if (neType == null)
                {
                    //TODO: error
                    return;
                }
            }
            IModuleGenericTypeInstance/*?*/ moduleGenericTypeInstance = (IModuleGenericTypeInstance)neType.ContainingType;
            if (fieldSignature.TypeReference != null)
            {
                this.typeReference = fieldSignature.TypeReference.SpecializeTypeInstance(moduleGenericTypeInstance);
            }
        }

        public override IFieldDefinition ResolvedField
        {
            get
            {
                IModuleTypeReference/*?*/ moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Field;
                IModuleTypeDefAndRef/*?*/ moduleType = moduleTypeRef.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Field;
                return moduleType.ResolveFieldReference(_unspecializedVersion);
            }
        }

        #region ISpecializedFieldReference Members

        public IFieldReference UnspecializedVersion
        {
            get { return _unspecializedVersion; }
        }

        private readonly FieldReference _unspecializedVersion;

        #endregion ISpecializedFieldReference Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.Name; }
        }

        #endregion INamedEntity Members
    }

    internal class MethodReference : MemberReference, IModuleMethodReference
    {
        internal readonly byte FirstByte;
        protected ushort genericParameterCount;
        protected EnumerableArrayWrapper<CustomModifier, ICustomModifier>/*?*/ returnCustomModifiers;
        protected IModuleTypeReference/*?*/ returnTypeReference;
        protected bool isReturnByReference;
        protected EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation>/*?*/ requiredParameters;
        protected EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation>/*?*/ varArgParameters;

        internal MethodReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleTypeReference/*?*/ parentTypeReference,
          IName name,
          byte firstByte
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name)
        {
            this.FirstByte = firstByte;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        //  Half of the double check lock. Other half done by the caller...
        protected virtual void InitMethodSignature()
        {
            MethodRefSignatureConverter methodSignature = this.PEFileToObjectModel.GetMethodRefSignature(this);
            this.genericParameterCount = methodSignature.GenericParamCount;
            this.returnCustomModifiers = methodSignature.ReturnCustomModifiers;
            this.returnTypeReference = methodSignature.ReturnTypeReference;
            this.isReturnByReference = methodSignature.IsReturnByReference;
            this.requiredParameters = methodSignature.RequiredParameters;
            this.varArgParameters = methodSignature.VarArgParameters;
        }

        public override ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this.ResolvedMethod; }
        }

        //^ [Confined]
        public override string ToString()
        {
            return MemberHelper.GetMethodSignature(this, NameFormattingOptions.ReturnType | NameFormattingOptions.TypeParameters | NameFormattingOptions.Signature);
        }

        #region IModuleMethodReference Members

        public EnumerableArrayWrapper<CustomModifier, ICustomModifier> ReturnCustomModifiers
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.returnCustomModifiers != null;
                return this.returnCustomModifiers;
            }
        }

        public IModuleTypeReference/*?*/ ReturnType
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.returnTypeReference != null;
                return this.returnTypeReference;
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> RequiredModuleParameterInfos
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.requiredParameters != null;
                return this.requiredParameters;
            }
        }

        public EnumerableArrayWrapper<IModuleParameterTypeInformation, IParameterTypeInformation> VarArgModuleParameterInfos
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                //^ assert this.varArgParameters != null;
                return this.varArgParameters;
            }
        }

        public bool IsReturnByReference
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return this.isReturnByReference;
            }
        }

        #endregion IModuleMethodReference Members

        #region IMethodReference Members

        public bool AcceptsExtraArguments
        {
            get { return (this.CallingConvention & (CallingConvention)0x7) == CallingConvention.ExtraArguments; }
        }

        public ushort GenericParameterCount
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    return (ushort)this.PEFileToObjectModel.GetMethodRefGenericParameterCount(this);
                }
                return this.genericParameterCount;
            }
        }

        public uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                {
                    _internedKey = this.PEFileToObjectModel.ModuleReader.metadataReaderHost.InternFactory.GetMethodInternedKey(this);
                }
                return _internedKey;
            }
        }

        private uint _internedKey;

        public bool IsGeneric
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    this.InitMethodSignature();
                }
                return this.genericParameterCount > 0;
            }
        }

        public virtual IMethodDefinition ResolvedMethod
        {
            get
            {
                IModuleTypeReference/*?*/moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Method;
                IModuleTypeDefAndRef/*?*/ moduleType = this.OwningTypeReference.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Method;
                return moduleType.ResolveMethodReference(this);
            }
        }

        public ushort ParameterCount
        {
            get
            {
                if (this.returnCustomModifiers == null)
                {
                    return (ushort)this.PEFileToObjectModel.GetMethodRefParameterCount(this);
                }
                return (ushort)(this.RequiredModuleParameterInfos.RawArray.Length + this.VarArgModuleParameterInfos.RawArray.Length);
            }
        }

        public IEnumerable<IParameterTypeInformation> ExtraParameters
        {
            get { return this.VarArgModuleParameterInfos; }
        }

        #endregion IMethodReference Members

        #region ISignature Members

        public CallingConvention CallingConvention
        {
            get { return (CallingConvention)this.FirstByte; }
        }

        public IEnumerable<IParameterTypeInformation> Parameters
        {
            get { return this.RequiredModuleParameterInfos; }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return this.ReturnCustomModifiers; }
        }

        public bool ReturnValueIsByRef
        {
            get { return this.IsReturnByReference; }
        }

        public bool ReturnValueIsModified
        {
            get { return this.ReturnCustomModifiers.RawArray.Length > 0; }
        }

        public ITypeReference Type
        {
            get
            {
                if (this.ReturnType == null)
                {
                    return Dummy.TypeReference;
                }
                return this.ReturnType;
            }
        }

        #endregion ISignature Members

        #region INamedEntity Members

        IName INamedEntity.Name
        {
            get { return this.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class GenericInstanceMethodReference : MethodReference, ISpecializedMethodReference
    {
        internal GenericInstanceMethodReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleGenericTypeInstance/*?*/ parentTypeReference,
          IName name,
          byte firstByte
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name, firstByte)
        {
            _unspecializedMethodReference = new MethodReference(peFileToObjectModel, memberRefRowId, parentTypeReference.ModuleGenericTypeReference, name, firstByte);
        }

        //  Half of the double check lock. Other half done by the caller...
        protected override void InitMethodSignature()
        {
            MethodRefSignatureConverter methodSignature = this.PEFileToObjectModel.GetMethodRefSignature(this);
            this.genericParameterCount = methodSignature.GenericParamCount;
            this.returnCustomModifiers = methodSignature.ReturnCustomModifiers;
            this.isReturnByReference = methodSignature.IsReturnByReference;
            this.requiredParameters = methodSignature.RequiredParameters; //Needed so that the method reference can be interned during specialization
            this.varArgParameters = methodSignature.VarArgParameters; //Ditto
                                                                      //^ assume this.ParentTypeReference is IModuleGenericTypeInstance; //ensured by the constructor
            IModuleGenericTypeInstance moduleGenericTypeInstance = (IModuleGenericTypeInstance)this.ParentTypeReference;
            if (methodSignature.ReturnTypeReference != null)
            {
                this.returnTypeReference = methodSignature.ReturnTypeReference.SpecializeTypeInstance(moduleGenericTypeInstance);
            }
            this.requiredParameters = TypeCache.SpecializeInstantiatedParameters(this, methodSignature.RequiredParameters, moduleGenericTypeInstance);
            this.varArgParameters = TypeCache.SpecializeInstantiatedParameters(this, methodSignature.VarArgParameters, moduleGenericTypeInstance);
        }

        public override IMethodDefinition ResolvedMethod
        {
            get
            {
                IModuleTypeReference/*?*/moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Method;
                IModuleTypeDefAndRef/*?*/ moduleType = this.OwningTypeReference.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Method;
                return moduleType.ResolveMethodReference(_unspecializedMethodReference);
            }
        }

        #region ISpecializedMethodReference Members

        public IMethodReference UnspecializedVersion
        {
            get { return _unspecializedMethodReference; }
        }

        private readonly MethodReference _unspecializedMethodReference;

        #endregion ISpecializedMethodReference Members
    }

    internal sealed class SpecializedNestedTypeMethodReference : MethodReference, ISpecializedMethodReference
    {
        private IModuleSpecializedNestedTypeReference _specializedParentTypeReference;

        internal SpecializedNestedTypeMethodReference(
          PEFileToObjectModel peFileToObjectModel,
          uint memberRefRowId,
          IModuleTypeReference parentTypeReference,
          IModuleSpecializedNestedTypeReference/*?*/ specializedParentTypeReference,
          IName name,
          byte firstByte
        )
          : base(peFileToObjectModel, memberRefRowId, parentTypeReference, name, firstByte)
        {
            _unspecializedMethodReference = new MethodReference(peFileToObjectModel, memberRefRowId, specializedParentTypeReference.UnspecializedModuleType, name, firstByte);
            _specializedParentTypeReference = specializedParentTypeReference;
        }

        //  Half of the double check lock. Other half done by the caller...
        protected override void InitMethodSignature()
        {
            MethodRefSignatureConverter methodSignature = this.PEFileToObjectModel.GetMethodRefSignature(this);
            this.genericParameterCount = methodSignature.GenericParamCount;
            this.returnCustomModifiers = methodSignature.ReturnCustomModifiers;
            this.isReturnByReference = methodSignature.IsReturnByReference;
            this.requiredParameters = methodSignature.RequiredParameters; //Needed so that the method reference can be interned during specialization
            this.varArgParameters = methodSignature.VarArgParameters; //Ditto
            IModuleSpecializedNestedTypeReference/*?*/ neType = _specializedParentTypeReference;
            while (neType.ContainingType is IGenericTypeInstanceReference)
            {
                neType = neType.ContainingType as IModuleSpecializedNestedTypeReference;
                if (neType == null)
                {
                    //TODO: error
                    return;
                }
            }
            //TODO: add methods to IModuleSpecializedNestedTypeReference that will allow the cast below to go away.
            IModuleGenericTypeInstance/*?*/ moduleGenericTypeInstance = (IModuleGenericTypeInstance)neType.ContainingType;
            if (methodSignature.ReturnTypeReference != null)
            {
                this.returnTypeReference = methodSignature.ReturnTypeReference.SpecializeTypeInstance(moduleGenericTypeInstance);
            }
            this.requiredParameters = TypeCache.SpecializeInstantiatedParameters(this, methodSignature.RequiredParameters, moduleGenericTypeInstance);
            this.varArgParameters = TypeCache.SpecializeInstantiatedParameters(this, methodSignature.VarArgParameters, moduleGenericTypeInstance);
        }

        public override IMethodDefinition ResolvedMethod
        {
            get
            {
                IModuleTypeReference/*?*/moduleTypeRef = this.OwningTypeReference;
                if (moduleTypeRef == null)
                    return Dummy.Method;
                IModuleTypeDefAndRef/*?*/ moduleType = this.OwningTypeReference.ResolvedModuleType;
                if (moduleType == null)
                    return Dummy.Method;
                return moduleType.ResolveMethodReference(_unspecializedMethodReference);
            }
        }

        #region ISpecializedMethodReference Members

        public IMethodReference UnspecializedVersion
        {
            get { return _unspecializedMethodReference; }
        }

        private readonly MethodReference _unspecializedMethodReference;

        #endregion ISpecializedMethodReference Members
    }

    #endregion Member Ref level Object Model

    #region Miscellaneous Stuff

    internal sealed class ByValArrayMarshallingInformation : IMarshallingInformation
    {
        private readonly System.Runtime.InteropServices.UnmanagedType _arrayElementType;
        private readonly uint _numberOfElements;

        internal ByValArrayMarshallingInformation(
          System.Runtime.InteropServices.UnmanagedType arrayElementType,
          uint numberOfElements
        )
        {
            _arrayElementType = arrayElementType;
            _numberOfElements = numberOfElements;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return _arrayElementType; }
        }

        System.Runtime.InteropServices.UnmanagedType IMarshallingInformation.UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.ByValArray; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return _numberOfElements; }
        }

        public uint? ParamIndex
        {
            get { return null; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class ByValTStrMarshallingInformation : IMarshallingInformation
    {
        private readonly uint _numberOfElements;

        internal ByValTStrMarshallingInformation(
          uint numberOfElements
        )
        {
            _numberOfElements = numberOfElements;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.AsAny; }
        }

        public System.Runtime.InteropServices.UnmanagedType UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.ByValTStr; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return _numberOfElements; }
        }

        public uint? ParamIndex
        {
            get { return null; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class IidParameterIndexMarshallingInformation : IMarshallingInformation
    {
        private readonly uint _iidParameterIndex;

        internal IidParameterIndexMarshallingInformation(
          uint iidParameterIndex
        )
        {
            _iidParameterIndex = iidParameterIndex;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.AsAny; }
        }

        public System.Runtime.InteropServices.UnmanagedType UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.Interface; }
        }

        public uint IidParameterIndex
        {
            get { return _iidParameterIndex; }
        }

        public uint NumberOfElements
        {
            get { return 0; }
        }

        public uint? ParamIndex
        {
            get { return null; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class LPArrayMarshallingInformation : IMarshallingInformation
    {
        private readonly System.Runtime.InteropServices.UnmanagedType _arrayElementType;
        private int _paramIndex;
        private uint _elementSize;
        private uint _numElement;

        internal LPArrayMarshallingInformation(
          System.Runtime.InteropServices.UnmanagedType arrayElementType,
          int paramIndex,
          uint elementSize,
          uint numElement
        )
        {
            _arrayElementType = arrayElementType;
            _paramIndex = paramIndex;
            _elementSize = elementSize;
            _numElement = numElement;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return _elementSize; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return _arrayElementType; }
        }

        System.Runtime.InteropServices.UnmanagedType IMarshallingInformation.UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.LPArray; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return _numElement; }
        }

        public uint? ParamIndex
        {
            get { return _paramIndex < 0 ? (uint?)null : (uint)_paramIndex; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class SafeArrayMarshallingInformation : IMarshallingInformation
    {
        private readonly System.Runtime.InteropServices.VarEnum _arrayElementType;
        private readonly ITypeReference _safeArrayElementUserDefinedSubType;

        internal SafeArrayMarshallingInformation(
          System.Runtime.InteropServices.VarEnum arrayElementType,
          ITypeReference safeArrayElementUserDefinedSubType
        )
        {
            _arrayElementType = arrayElementType;
            _safeArrayElementUserDefinedSubType = safeArrayElementUserDefinedSubType;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.AsAny; }
        }

        System.Runtime.InteropServices.UnmanagedType IMarshallingInformation.UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.SafeArray; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return 0; }
        }

        public uint? ParamIndex
        {
            get { return null; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return _arrayElementType; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return _safeArrayElementUserDefinedSubType; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class SimpleMarshallingInformation : IMarshallingInformation
    {
        private readonly System.Runtime.InteropServices.UnmanagedType _unmanagedType;

        internal SimpleMarshallingInformation(
          System.Runtime.InteropServices.UnmanagedType unmanagedType
        )
        {
            _unmanagedType = unmanagedType;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return Dummy.TypeReference; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return string.Empty; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.AsAny; }
        }

        public System.Runtime.InteropServices.UnmanagedType UnmanagedType
        {
            get { return _unmanagedType; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return 0; }
        }

        public uint? ParamIndex
        {
            get { return null; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class CustomMarshallingInformation : IMarshallingInformation
    {
        private readonly ITypeReference _marshaller;
        private readonly string _marshallerRuntimeArgument;

        internal CustomMarshallingInformation(
          ITypeReference marshaller,
          string marshallerRuntimeArgument
        )
        {
            _marshaller = marshaller;
            _marshallerRuntimeArgument = marshallerRuntimeArgument;
        }

        #region IMarshallingInformation Members

        public ITypeReference CustomMarshaller
        {
            get { return _marshaller; }
        }

        public string CustomMarshallerRuntimeArgument
        {
            get { return _marshallerRuntimeArgument; }
        }

        public uint ElementSize
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType ElementType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.AsAny; }
        }

        System.Runtime.InteropServices.UnmanagedType IMarshallingInformation.UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.CustomMarshaler; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public uint NumberOfElements
        {
            get { return 0; }
        }

        public uint? ParamIndex
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.VarEnum SafeArrayElementSubtype
        {
            get { return System.Runtime.InteropServices.VarEnum.VT_EMPTY; }
        }

        public ITypeReference SafeArrayElementUserDefinedSubtype
        {
            get { return Dummy.TypeReference; }
        }

        public uint ElementSizeMultiplier
        {
            get { return 0; }
        }

        #endregion IMarshallingInformation Members
    }

    internal sealed class Win32Resource : IWin32Resource
    {
        internal readonly PEFileToObjectModel PEFileToObjectModel;
        internal readonly int TypeIdOrName;
        internal readonly int IdOrName;
        internal readonly int LanguageIdOrName;
        internal readonly int RVAToData;
        internal readonly uint Size;
        internal readonly uint CodePage;

        internal Win32Resource(
          PEFileToObjectModel peFileTOObjectModel,
          int typeIdOrName,
          int idOrName,
          int languageIdOrName,
          int rvaToData,
          uint size,
          uint codePage
        )
        {
            this.PEFileToObjectModel = peFileTOObjectModel;
            this.TypeIdOrName = typeIdOrName;
            this.IdOrName = idOrName;
            this.LanguageIdOrName = languageIdOrName;
            this.RVAToData = rvaToData;
            this.Size = size;
            this.CodePage = codePage;
        }

        #region IWin32Resource Members

        public string TypeName
        {
            get
            {
                return this.PEFileToObjectModel.GetWin32ResourceName(this.TypeIdOrName);
            }
        }

        public int TypeId
        {
            get { return this.TypeIdOrName; }
        }

        public string Name
        {
            get
            {
                return this.PEFileToObjectModel.GetWin32ResourceName(this.IdOrName);
            }
        }

        public int Id
        {
            get { return this.IdOrName; }
        }

        public uint LanguageId
        {
            get { return (uint)this.LanguageIdOrName; }
        }

        uint IWin32Resource.CodePage
        {
            get
            {
                return this.CodePage;
            }
        }

        public IEnumerable<byte> Data
        {
            get
            {
                return this.PEFileToObjectModel.GetWin32ResourceBytes(this.RVAToData, (int)this.Size);
            }
        }

        #endregion IWin32Resource Members
    }

    internal sealed class FileReference : MetadataObject, IFileReference
    {
        internal readonly uint FileRowId;
        internal readonly FileFlags FileFlags;
        internal readonly IName Name;

        internal FileReference(
          PEFileToObjectModel peFileToObjectModel,
          uint fileRowId,
          FileFlags fileFlags,
          IName name
        )
          : base(peFileToObjectModel)
        {
            this.FileRowId = fileRowId;
            this.FileFlags = fileFlags;
            this.Name = name;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override uint TokenValue
        {
            get
            {
                return TokenTypeIds.File | this.FileRowId;
            }
        }

        #region IFileReference Members

        public IAssembly ContainingAssembly
        {
            get
            {
                IAssembly/*?*/ assem = this.PEFileToObjectModel.Module as IAssembly;
                return assem == null ? Dummy.Assembly : assem;
            }
        }

        public bool HasMetadata
        {
            get { return (this.FileFlags & FileFlags.ContainsNoMetadata) != FileFlags.ContainsNoMetadata; }
        }

        public IName FileName
        {
            get { return this.Name; }
        }

        public IEnumerable<byte> HashValue
        {
            get
            {
                return this.PEFileToObjectModel.GetFileHash(this.FileRowId);
            }
        }

        #endregion IFileReference Members
    }

    internal class ResourceReference : MetadataObject, IResourceReference
    {
        internal readonly uint ResourceRowId;
        private readonly IAssemblyReference _definingAssembly;
        protected readonly ManifestResourceFlags Flags;
        internal readonly IName Name;
        private IResource/*?*/ _resolvedResource;

        internal ResourceReference(
          PEFileToObjectModel peFileToObjectModel,
          uint resourceRowId,
          IAssemblyReference definingAssembly,
          ManifestResourceFlags flags,
          IName name
        )
          : base(peFileToObjectModel)
        {
            this.ResourceRowId = resourceRowId;
            _definingAssembly = definingAssembly;
            this.Flags = flags;
            this.Name = name;
        }

        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.ManifestResource | this.ResourceRowId; }
        }

        #region IResourceReference Members

        IAssemblyReference IResourceReference.DefiningAssembly
        {
            get { return _definingAssembly; }
        }

        public bool IsPublic
        {
            get { return (this.Flags & ManifestResourceFlags.PublicVisibility) == ManifestResourceFlags.PublicVisibility; }
        }

        IName IResourceReference.Name
        {
            get { return this.Name; }
        }

        public IResource Resource
        {
            get
            {
                if (_resolvedResource == null)
                {
                    _resolvedResource = this.PEFileToObjectModel.ResolveResource(this, this);
                }
                return _resolvedResource;
            }
        }

        #endregion IResourceReference Members
    }

    internal sealed class Resource : ResourceReference, IResource
    {
        //^ [NotDelayed]
        internal Resource(
          PEFileToObjectModel peFileToObjectModel,
          uint resourceRowId,
          IName name,
          ManifestResourceFlags flags,
          bool inExternalFile
        )
          : base(peFileToObjectModel, resourceRowId, Dummy.Assembly, inExternalFile ? flags | ManifestResourceFlags.InExternalFile : flags, name)
        {
        }

        internal override uint TokenValue
        {
            get { return TokenTypeIds.ManifestResource | this.ResourceRowId; }
        }

        #region IResource Members

        public IEnumerable<byte> Data
        {
            get
            {
                return this.PEFileToObjectModel.GetResourceData(this);
            }
        }

        public IFileReference ExternalFile
        {
            get { return this.PEFileToObjectModel.GetExternalFileForResource(this.ResourceRowId); }
        }

        public bool IsInExternalFile
        {
            get { return (this.Flags & ManifestResourceFlags.InExternalFile) == ManifestResourceFlags.InExternalFile; }
        }

        #endregion IResource Members

        #region IResourceReference Members

        IAssemblyReference IResourceReference.DefiningAssembly
        {
            get
            {
                IAssembly/*?*/ assem = this.PEFileToObjectModel.Module as IAssembly;
                return assem == null ? Dummy.Assembly : assem;
            }
        }

        IResource IResourceReference.Resource
        {
            get { return this; }
        }

        #endregion IResourceReference Members
    }

    #endregion Miscellaneous Stuff
}