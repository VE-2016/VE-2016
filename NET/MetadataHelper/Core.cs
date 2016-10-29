//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using Microsoft.Cci.UtilityDataStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci
{
    /// <summary>
    /// Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.
    /// </summary>
    public abstract class MetadataHostEnvironment : IMetadataHost
    {
        /// <summary>
        /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
        /// </summary>
        /// <param name="nameTable">
        /// A collection of IName instances that represent names that are commonly used during compilation.
        /// This is a provided as a parameter to the host environment in order to allow more than one host
        /// environment to co-exist while agreeing on how to map strings to IName instances.
        /// </param>
        /// <param name="pointerSize">The size of a pointer on the runtime that is the target of the metadata units to be loaded
        /// into this metadta host. This parameter only matters if the host application wants to work out what the exact layout
        /// of a struct will be on the target runtime. The framework uses this value in methods such as TypeHelper.SizeOfType and
        /// TypeHelper.TypeAlignment. If the host application does not care about the pointer size it can provide 0 as the value
        /// of this parameter. In that case, the first reference to IMetadataHost.PointerSize will probe the list of loaded assemblies
        /// to find an assembly that either requires 32 bit pointers or 64 bit pointers. If no such assembly is found, the default is 32 bit pointers.
        /// </param>
        protected MetadataHostEnvironment(INameTable nameTable, byte pointerSize)
        //^ requires pointerSize == 0 || pointerSize == 4 || pointerSize == 8;
        {
            _nameTable = nameTable;
            _internFactory = new InternFactory();
            _pointerSize = pointerSize;
        }

        /// <summary>
        /// The errors reported by this event are discovered in background threads by an opend ended
        /// set of error reporters. Listeners to this event should thus be prepared to be called at abitrary times from arbitrary threads.
        /// Each occurrence of the event concerns a particular source location and a particular error reporter.
        /// The reported error collection (possibly empty) supercedes any errors previously reported by the same error reporter for the same source location.
        /// A source location can be an entire ISourceDocument, or just a part of it (the latter would apply to syntax errors discovered by an incremental
        /// parser after an edit to the source document).
        /// </summary>
        public event EventHandler<Microsoft.Cci.ErrorEventArgs> Errors;

        /// <summary>
        /// The identity of the assembly containing Microsoft.Contracts.Contract.
        /// </summary>
        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get
            {
                if (_contractAssemblySymbolicIdentity == null)
                    _contractAssemblySymbolicIdentity = this.GetContractAssemblySymbolicIdentity();
                return _contractAssemblySymbolicIdentity;
            }
        }

        private AssemblyIdentity/*?*/ _contractAssemblySymbolicIdentity;

        /// <summary>
        /// Returns the identity of the assembly containing the Microsoft.Contracts.Contract, by asking
        /// each of the loaded units for its opinion on the matter and returning the opinion with the highest version number.
        /// If none of the loaded units have an opinion, the result is the same as CoreAssemblySymbolicIdentity.
        /// </summary>
        protected virtual AssemblyIdentity GetContractAssemblySymbolicIdentity()
        {
            if (_unitCache.Count > 0)
            {
                AssemblyIdentity/*?*/ result = null;
                foreach (IUnit unit in _unitCache.Values)
                {
                    AssemblyIdentity contractId = unit.ContractAssemblySymbolicIdentity;
                    if (contractId.Name.Value.Length == 0) continue;
                    if (result == null || result.Version < contractId.Version) result = contractId;
                }
                if (result != null) return result;
            }
            return this.CoreAssemblySymbolicIdentity;
        }

        /// <summary>
        /// The identity of the assembly containing the core system types such as System.Object.
        /// </summary>
        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get
            {
                if (_coreAssemblySymbolicIdentity == null)
                    _coreAssemblySymbolicIdentity = this.GetCoreAssemblySymbolicIdentity();
                return _coreAssemblySymbolicIdentity;
            }
        }

        private AssemblyIdentity/*?*/ _coreAssemblySymbolicIdentity;

        /// <summary>
        /// Returns the identity of the assembly containing the core system types such as System.Object, by asking
        /// each of the loaded units for its opinion on the matter and returning the opinion with the highest version number.
        /// If none of the loaded units have an opinion, the identity of the runtime executing the compiler itself is returned.
        /// </summary>
        protected virtual AssemblyIdentity GetCoreAssemblySymbolicIdentity()
        {
            if (_unitCache.Count > 0)
            {
                AssemblyIdentity/*?*/ result = null;
                foreach (IUnit unit in _unitCache.Values)
                {
                    AssemblyIdentity coreId = unit.CoreAssemblySymbolicIdentity;
                    if (coreId.Name.Value.Length == 0) continue;
                    if (result == null || result.Version < coreId.Version) result = coreId;
                }
                if (result != null) return result;
            }
            var coreAssemblyName = typeof(object).Assembly.GetName();
            string/*?*/ loc = coreAssemblyName.CodeBase;
            if (loc == null) loc = "";
            return new AssemblyIdentity(this.NameTable.GetNameFor(coreAssemblyName.Name), "", coreAssemblyName.Version, coreAssemblyName.GetPublicKeyToken(), loc);
        }

        /// <summary>
        /// Finds the assembly that matches the given identifier among the already loaded set of assemblies,
        /// or a dummy assembly if no matching assembly can be found.
        /// </summary>
        public IAssembly FindAssembly(AssemblyIdentity assemblyIdentity)
        {
            IUnit/*?*/ unit;
            lock (GlobalLock.LockingObject)
            {
                _unitCache.TryGetValue(assemblyIdentity, out unit);
            }
            IAssembly/*?*/ result = unit as IAssembly;
            if (result != null)
                return result;
            return Dummy.Assembly;
        }

        /// <summary>
        /// Finds the module that matches the given identifier among the already loaded set of modules,
        /// or a dummy module if no matching module can be found.
        /// </summary>
        public IModule FindModule(ModuleIdentity moduleIdentity)
        {
            IUnit/*?*/ unit;
            lock (GlobalLock.LockingObject)
            {
                _unitCache.TryGetValue(moduleIdentity, out unit);
            }
            IModule/*?*/ result = unit as IModule;
            if (result != null)
                return result;
            return Dummy.Module;
        }

        /// <summary>
        /// Finds the unit that matches the given identifier, or a dummy unit if no matching unit can be found.
        /// </summary>
        public IUnit FindUnit(UnitIdentity unitIdentity)
        {
            IUnit/*?*/ unit;
            lock (GlobalLock.LockingObject)
            {
                _unitCache.TryGetValue(unitIdentity, out unit);
            }
            if (unit != null)
                return unit;
            return Dummy.Unit;
        }

        /// <summary>
        /// Returns the intern factory.
        /// </summary>
        public IInternFactory InternFactory
        {
            get { return _internFactory; }
        }

        private InternFactory _internFactory;

        /// <summary>
        /// The assembly that matches the given reference, or a dummy assembly if no matching assembly can be found.
        /// </summary>
        public virtual IAssembly LoadAssembly(AssemblyIdentity assemblyIdentity)
        {
            if (assemblyIdentity.Location == null) return Dummy.Assembly;
            IUnit/*?*/ unit;
            lock (GlobalLock.LockingObject)
            {
                _unitCache.TryGetValue(assemblyIdentity, out unit);
            }
            if (unit == null)
            {
                unit = this.LoadUnitFrom(assemblyIdentity.Location);
            }
            IAssembly/*?*/ result = unit as IAssembly;
            if (result != null && assemblyIdentity.Equals(UnitHelper.GetAssemblyIdentity(result)))
                return result;
            return Dummy.Assembly;
        }

        /// <summary>
        /// The module that matches the given reference, or a dummy module if no matching module can be found.
        /// </summary>
        public virtual IModule LoadModule(ModuleIdentity moduleIdentity)
        {
            if (moduleIdentity.Location == null) return Dummy.Module;
            IUnit/*?*/ unit;
            lock (GlobalLock.LockingObject)
            {
                _unitCache.TryGetValue(moduleIdentity, out unit);
            }
            if (unit == null)
            {
                unit = this.LoadUnitFrom(moduleIdentity.Location);
            }
            IModule/*?*/ result = unit as IModule;
            if (result != null && moduleIdentity.Equals(UnitHelper.GetModuleIdentity(result)))
                return result;
            return Dummy.Module;
        }

        /// <summary>
        /// The unit that matches the given identity, or a dummy unit if no matching unit can be found.
        /// </summary>
        public IUnit LoadUnit(UnitIdentity unitIdentity)
        {
            AssemblyIdentity/*?*/ assemblyIdentity = unitIdentity as AssemblyIdentity;
            if (assemblyIdentity != null) return this.LoadAssembly(assemblyIdentity);
            ModuleIdentity/*?*/ moduleIdentity = unitIdentity as ModuleIdentity;
            if (moduleIdentity != null) return this.LoadModule(moduleIdentity);
            return this.LoadUnitFrom(unitIdentity.Location);
        }

        /// <summary>
        /// Returns the unit that is stored at the given location, or a dummy unit if no unit exists at that location or if the unit at that location is not accessible.
        /// </summary>
        public abstract IUnit LoadUnitFrom(string location);

        /// <summary>
        /// Returns enumeration of all the units loaded so far.
        /// </summary>
        public IEnumerable<IUnit> LoadedUnits
        {
            get
            {
                return _unitCache.Values;
            }
        }

        /// <summary>
        /// A table used to intern strings used as names.
        /// </summary>
        public INameTable NameTable
        {
            [DebuggerNonUserCode]
            get
            { return _nameTable; }
        }

        private readonly INameTable _nameTable;

        /// <summary>
        /// A collection of references to types from the core platform, such as System.Object and System.String.
        /// </summary>
        public IPlatformType PlatformType
        {
            get
            {
                if (_platformType == null)
                    _platformType = this.GetPlatformType();
                return _platformType;
            }
        }

        private IPlatformType/*?*/ _platformType;

        /// <summary>
        /// Returns an object that provides a collection of references to types from the core platform, such as System.Object and System.String.
        /// </summary>
        protected virtual IPlatformType GetPlatformType()
        {
            return new PlatformType(this);
        }

        /// <summary>
        /// The size (in bytes) of a pointer on the platform on which the host is targetting.
        /// The value of this property is either 4 (32-bits) or 8 (64-bit).
        /// </summary>
        public byte PointerSize
        {
            get
            {
                //^^ ensures result == 4 || result == 8;
                if (_pointerSize == 0)
                    _pointerSize = this.GetTargetPlatformPointerSize();
                return _pointerSize;
            }
        }

        private byte _pointerSize;
        //^ invariant pointerSize == 0 || pointerSize == 4 || pointerSize == 8;

        /// <summary>
        /// Returns an opinion about the size of a pointer on the target runtime for the set of modules
        /// currently in this.unitCache. If none of the modules requires either 32 bit pointers or 64 bit pointers
        /// the result is 4 (i.e. 32 bit pointers). This method is only called if a host application has not
        /// explicitly provided the pointer size of the target platform.
        /// </summary>
        protected virtual byte GetTargetPlatformPointerSize()
        //^ ensures result == 4 || result == 8;
        {
            if (_unitCache.Count > 0)
            {
                foreach (IUnit unit in _unitCache.Values)
                {
                    IModule/*?*/ module = unit as IModule;
                    if (module == null) continue;
                    if (module.Requires32bits) return 4;
                    if (module.Requires64bits) return 8;
                }
            }
            return 4;
        }

        /// <summary>
        /// Registers the given unit as the latest one associated with the unit's location.
        /// Such units can then be discovered by clients via GetUnit.
        /// </summary>
        /// <param name="unit">The unit to register.</param>
        protected void RegisterAsLatest(IUnit unit)
        {
            lock (GlobalLock.LockingObject)
            {
                _unitCache[unit.UnitIdentity] = unit;
            }
        }

        /// <summary>
        /// Raises the CompilationErrors event with the given error event arguments.
        /// The event is raised on a separate thread.
        /// </summary>
        public virtual void ReportErrors(Microsoft.Cci.ErrorEventArgs errorEventArguments)
        {
            if (this.Errors != null)
                ThreadPool.QueueUserWorkItem(this.SynchronousReportErrors, errorEventArguments);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        protected void SynchronousReportErrors(object/*?*/ state)
        //^ requires state is Microsoft.Cci.ErrorEventArgs;
        {
            Microsoft.Cci.ErrorEventArgs errorEventArguments = (Microsoft.Cci.ErrorEventArgs)state;
            if (this.Errors != null)
                this.Errors(this, errorEventArguments);
        }

        /// <summary>
        /// Raises the CompilationErrors event with the given error wrapped up in an error event arguments object.
        /// The event is raised on a separate thread.
        /// </summary>
        /// <param name="error">The error to report.</param>
        public void ReportError(IErrorMessage error)
        {
            if (this.Errors != null)
            {
                List<IErrorMessage> errors = new List<IErrorMessage>(1);
                errors.Add(error);
                Microsoft.Cci.ErrorEventArgs errorEventArguments = new Microsoft.Cci.ErrorEventArgs(error.ErrorReporter, error.Location, errors.AsReadOnly());
                this.ReportErrors(errorEventArguments);
            }
        }

        private readonly Dictionary<UnitIdentity, IUnit> _unitCache = new Dictionary<UnitIdentity, IUnit>();

        /// <summary>
        /// Default implementation of UnifyAssemblyReference. Override this method to change the behaviour.
        /// </summary>
        /// <param name="referringUnit"></param>
        /// <param name="referencedAssembly"></param>
        /// <returns></returns>
        //^ [Pure]
        public virtual AssemblyIdentity ProbeAssemblyReference(IUnit referringUnit, AssemblyIdentity referencedAssembly)
        {
            return referencedAssembly;
        }

        /// <summary>
        /// Default implementation of UnifyModuleReference. Override this method to change the behaviour.
        /// </summary>
        /// <param name="referringUnit"></param>
        /// <param name="referencedModule"></param>
        /// <returns></returns>
        //^ [Pure]
        public virtual ModuleIdentity ProbeModuleReference(IUnit referringUnit, ModuleIdentity referencedModule)
        {
            return referencedModule;
        }

        /// <summary>
        /// Default implementation of UnifyAssembly. Override this method to change the behaviour.
        /// </summary>
        //^ [Pure]
        public virtual AssemblyIdentity UnifyAssembly(AssemblyIdentity assemblyIdentity)
        {
            if (assemblyIdentity.Name.UniqueKeyIgnoringCase == this.CoreAssemblySymbolicIdentity.Name.UniqueKeyIgnoringCase &&
              assemblyIdentity.Culture == this.CoreAssemblySymbolicIdentity.Culture &&
              IteratorHelper.EnumerablesAreEqual(assemblyIdentity.PublicKeyToken, this.CoreAssemblySymbolicIdentity.PublicKeyToken))
                return this.CoreAssemblySymbolicIdentity;
            return assemblyIdentity;
        }
    }

    /// <summary>
    /// Static class encasulating the global lock object.
    /// </summary>
    public static class GlobalLock
    {
        /// <summary>
        /// All synchronization code should exclusively use this lock object,
        /// hence making it trivial to ensure that there are no deadlocks.
        /// It also means that the lock should never be held for long.
        /// In particular, no code holding this lock should ever wait on another thread.
        /// </summary>
        public static readonly object LockingObject = new object();
    }

    /// <summary>
    /// An interface provided by the application hosting the metadata reader. The interface allows the host application
    /// to control how assembly references are unified, where files are found and so on.
    /// </summary>
    public interface IMetadataReaderHost : IMetadataHost
    {
        /// <summary>
        /// Open the binary document as a memory block in host dependent fashion.
        /// </summary>
        /// <param name="sourceDocument">The binary document that is to be opened.</param>
        /// <returns>The unmanaged memory block corresponding to the source document.</returns>
        IBinaryDocumentMemoryBlock/*?*/ OpenBinaryDocument(IBinaryDocument sourceDocument);

        /// <summary>
        /// Open the child binary document within the context of parent source document.as a memory block in host dependent fashion
        /// For example: in multimodule assemblies the main module will be parentSourceDocument, where as other modules will be child
        /// docuements.
        /// </summary>
        /// <param name="parentSourceDocument">The source document indicating the child document location.</param>
        /// <param name="childDocumentName">The name of the child document.</param>
        /// <returns>The unmanaged memory block corresponding to the child document.</returns>
        IBinaryDocumentMemoryBlock/*?*/ OpenBinaryDocument(IBinaryDocument parentSourceDocument, string childDocumentName);

        /// <summary>
        /// This method is called when the assebly reference is being resolved and its not already loaded by the host.
        /// </summary>
        /// <param name="referringUnit">The unit that is referencing the assembly.</param>
        /// <param name="referencedAssembly">Assembly identifier for the assembly being referenced.</param>
        void ResolvingAssemblyReference(IUnit referringUnit, AssemblyIdentity referencedAssembly);

        /// <summary>
        /// This method is called when the module reference is being resolved and its not already loaded by the host.
        /// </summary>
        /// <param name="referringUnit">The unit that is referencing the module.</param>
        /// <param name="referencedModule">Module identifier for the assembly being referenced.</param>
        void ResolvingModuleReference(IUnit referringUnit, ModuleIdentity referencedModule);
    }

    /// <summary>
    /// A base class for an object provided by the application hosting the metadata reader. The object allows the host application
    /// to control how assembly references are unified, where files are found and so on.
    /// </summary>
    public abstract class MetadataReaderHost : MetadataHostEnvironment, IMetadataReaderHost
    {
        /// <summary>
        /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
        /// </summary>
        protected MetadataReaderHost()
          : this(new NameTable(), 0)
        {
        }

        /// <summary>
        /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
        /// </summary>
        /// <param name="nameTable">
        /// A collection of IName instances that represent names that are commonly used during compilation.
        /// This is a provided as a parameter to the host environment in order to allow more than one host
        /// environment to co-exist while agreeing on how to map strings to IName instances.
        /// </param>
        protected MetadataReaderHost(INameTable nameTable)
          : this(nameTable, 0)
        {
        }

        /// <summary>
        /// Allocates an object that provides an abstraction over the application hosting compilers based on this framework.
        /// </summary>
        /// <param name="nameTable">
        /// A collection of IName instances that represent names that are commonly used during compilation.
        /// This is a provided as a parameter to the host environment in order to allow more than one host
        /// environment to co-exist while agreeing on how to map strings to IName instances.
        /// </param>
        /// <param name="pointerSize">The size of a pointer on the runtime that is the target of the metadata units to be loaded
        /// into this metadta host. This parameter only matters if the host application wants to work out what the exact layout
        /// of a struct will be on the target runtime. The framework uses this value in methods such as TypeHelper.SizeOfType and
        /// TypeHelper.TypeAlignment. If the host application does not care about the pointer size it can provide 0 as the value
        /// of this parameter. In that case, the first reference to IMetadataHost.PointerSize will probe the list of loaded assemblies
        /// to find an assembly that either requires 32 bit pointers or 64 bit pointers. If no such assembly is found, the default is 32 bit pointers.
        /// </param>
        protected MetadataReaderHost(INameTable nameTable, byte pointerSize)
          : base(nameTable, pointerSize)
        //^ requires pointerSize == 0 || pointerSize == 4 || pointerSize == 8;
        {
        }

        #region IMetadataReaderHost Members

        /// <summary>
        /// Open the binary document as a memory block in host dependent fashion.
        /// </summary>
        /// <param name="sourceDocument">The binary document that is to be opened.</param>
        /// <returns>The unmanaged memory block corresponding to the source document.</returns>
        public virtual IBinaryDocumentMemoryBlock/*?*/ OpenBinaryDocument(IBinaryDocument sourceDocument)
        {
            try
            {
#if !COMPACTFX
                IBinaryDocumentMemoryBlock binDocMemoryBlock = MemoryMappedFile.CreateMemoryMappedFile(sourceDocument.Location, sourceDocument);
#else
        IBinaryDocumentMemoryBlock binDocMemoryBlock = UnmanagedBinaryMemoryBlock.CreateUnmanagedBinaryMemoryBlock(sourceDocument.Location, sourceDocument);
#endif
                return binDocMemoryBlock;
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// Open the child binary document within the context of parent source document.as a memory block in host dependent fashion
        /// For example: in multimodule assemblies the main module will be parentSourceDocument, where as other modules will be child
        /// docuements.
        /// </summary>
        /// <param name="parentSourceDocument">The source document indicating the child document location.</param>
        /// <param name="childDocumentName">The name of the child document.</param>
        /// <returns>The unmanaged memory block corresponding to the child document.</returns>
        public virtual IBinaryDocumentMemoryBlock/*?*/ OpenBinaryDocument(IBinaryDocument parentSourceDocument, string childDocumentName)
        {
            try
            {
                string directory = Path.GetDirectoryName(parentSourceDocument.Location);
                string fullPath = Path.Combine(directory, childDocumentName);
                IBinaryDocument newBinaryDocument = BinaryDocument.GetBinaryDocumentForFile(fullPath, this);
#if !COMPACTFX
                IBinaryDocumentMemoryBlock binDocMemoryBlock = MemoryMappedFile.CreateMemoryMappedFile(newBinaryDocument.Location, newBinaryDocument);
#else
        IBinaryDocumentMemoryBlock binDocMemoryBlock = UnmanagedBinaryMemoryBlock.CreateUnmanagedBinaryMemoryBlock(newBinaryDocument.Location, newBinaryDocument);
#endif
                return binDocMemoryBlock;
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>
        /// This method is called when the assebly reference is being resolved and its not already loaded by the Read/Write host.
        /// </summary>
        /// <param name="referringUnit">The unit that is referencing the assembly.</param>
        /// <param name="referencedAssembly">Assembly identity for the assembly being referenced.</param>
        public virtual void ResolvingAssemblyReference(IUnit referringUnit, AssemblyIdentity referencedAssembly)
        {
        }

        /// <summary>
        /// This method is called when the module reference is being resolved and its not already loaded by the Read/Write host.
        /// </summary>
        /// <param name="referringUnit">The unit that is referencing the module.</param>
        /// <param name="referencedModule">Module identity for the assembly being referenced.</param>
        public virtual void ResolvingModuleReference(IUnit referringUnit, ModuleIdentity referencedModule)
        {
        }

        #endregion IMetadataReaderHost Members
    }

    internal sealed class InternFactory : IInternFactory
    {
        private sealed class AssemblyStore
        {
            internal readonly AssemblyIdentity AssemblyIdentity;
            internal uint InternedIdWithCount;
            internal readonly uint RootNamespaceInternedId;

            internal AssemblyStore(
              AssemblyIdentity assemblyIdentity,
              uint internedId,
              uint rootNamespaceInternedId
            )
            {
                this.AssemblyIdentity = assemblyIdentity;
                this.InternedIdWithCount = internedId;
                this.RootNamespaceInternedId = rootNamespaceInternedId;
            }

            internal uint InternedId
            {
                get
                {
                    return this.InternedIdWithCount & 0xFFFFF000;
                }
            }
        }

        private sealed class ModuleStore
        {
            internal readonly ModuleIdentity ModuleIdentitity;
            internal readonly uint InternedId;
            internal readonly uint RootNamespaceInternedId;

            internal ModuleStore(
              ModuleIdentity moduleIdentitity,
              uint internedId,
              uint rootNamespaceInternedId
            )
            {
                this.ModuleIdentitity = moduleIdentitity;
                this.InternedId = internedId;
                this.RootNamespaceInternedId = rootNamespaceInternedId;
            }
        }

        private sealed class NamespaceTypeStore
        {
            internal readonly uint ContainingNamespaceInternedId;
            internal readonly uint GenericParameterCount;
            internal readonly uint InternedId;

            internal NamespaceTypeStore(
              uint containingNamespaceInternedId,
              uint genericParameterCount,
              uint internedId
            )
            {
                this.ContainingNamespaceInternedId = containingNamespaceInternedId;
                this.GenericParameterCount = genericParameterCount;
                this.InternedId = internedId;
            }
        }

        private sealed class NestedTypeStore
        {
            internal readonly uint ContainingTypeInternedId;
            internal readonly uint GenericParameterCount;
            internal readonly uint InternedId;

            internal NestedTypeStore(
              uint containingTypeInternedId,
              uint genericParameterCount,
              uint internedId
            )
            {
                this.ContainingTypeInternedId = containingTypeInternedId;
                this.GenericParameterCount = genericParameterCount;
                this.InternedId = internedId;
            }
        }

        private sealed class MatrixTypeStore
        {
            internal readonly int Rank;
            internal readonly int[] LowerBounds;
            internal readonly ulong[] Sizes;
            internal readonly uint InternedId;

            internal MatrixTypeStore(
              int rank,
              int[] lowerBounds,
              ulong[] sizes,
              uint internedId
            )
            {
                this.Rank = rank;
                this.LowerBounds = lowerBounds;
                this.Sizes = sizes;
                this.InternedId = internedId;
            }
        }

        private sealed class ParameterTypeStore
        {
            internal readonly bool IsByReference;
            internal readonly uint CustomModifiersInternId;
            internal readonly uint InternedId;

            internal ParameterTypeStore(
              bool isByReference,
              uint customModifiersInternId,
              uint internedId
            )
            {
                this.IsByReference = isByReference;
                this.CustomModifiersInternId = customModifiersInternId;
                this.InternedId = internedId;
            }
        }

        private sealed class SignatureStore
        {
            internal readonly CallingConvention CallingConvention;
            internal readonly uint RequiredParameterListInternedId;
            internal readonly uint ExtraParameterListInternedId;
            internal readonly bool ReturnValueIsByRef;
            internal readonly uint ReturnValueCustomModifiersListInteredId;
            internal readonly uint ReturnTypeReferenceInternedId;
            internal readonly uint InternedId;

            internal SignatureStore(
             CallingConvention callingConvention,
             uint requiredParameterListInteredId,
             uint extraParameterListInteredId,
             bool returnValueIsByRef,
             uint returnValueCustomModifiersListInteredId,
             uint returnTypeReferenceInteredId,
             uint internedId
            )
            {
                this.CallingConvention = callingConvention;
                this.RequiredParameterListInternedId = requiredParameterListInteredId;
                this.ExtraParameterListInternedId = extraParameterListInteredId;
                this.ReturnValueIsByRef = returnValueIsByRef;
                this.ReturnValueCustomModifiersListInteredId = returnValueCustomModifiersListInteredId;
                this.ReturnTypeReferenceInternedId = returnTypeReferenceInteredId;
                this.InternedId = internedId;
            }
        }

        private uint _currentAssemblyInternValue;
        private uint _currentModuleInternValue;
        private uint _currentNamespaceInternValue;
        private uint _currentTypeInternValue;
        private uint _currentTypeListInternValue;
        private uint _currentCustomModifierInternValue;
        private uint _currentCustomModifierListInternValue;
        private uint _currentParameterTypeInternValue;
        private uint _currentParameterTypeListInternValue;
        private uint _currentSignatureInternValue;
        private uint _currentMethodReferenceInternValue;
        private IMethodReference _currentMethodReference; //The method reference currently being interned
        private readonly MultiHashtable<AssemblyStore> _assemblyHashtable;
        private readonly MultiHashtable<ModuleStore> _moduleHashtable;
        private readonly DoubleHashtable _nestedNamespaceHashtable;
        private readonly MultiHashtable<NamespaceTypeStore> _namespaceTypeHashtable;
        private readonly MultiHashtable<NestedTypeStore> _nestedTypeHashtable;
        private readonly Hashtable _vectorTypeHashTable;
        private readonly Hashtable _pointerTypeHashTable;
        private readonly Hashtable _managedPointerTypeHashTable;
        private readonly MultiHashtable<MatrixTypeStore> _matrixTypeHashtable;
        private readonly DoubleHashtable _typeListHashtable;
        private readonly DoubleHashtable _genericInstanceHashtable;
        private readonly DoubleHashtable _genericTypeParameterHashtable;
        private readonly DoubleHashtable _genericMethodTypeParameterHashTable;
        private readonly DoubleHashtable _customModifierHashTable;
        private readonly DoubleHashtable _customModifierListHashTable;
        private readonly MultiHashtable<ParameterTypeStore> _parameterTypeHashtable;
        private readonly DoubleHashtable _parameterTypeListHashtable;
        private readonly MultiHashtable<SignatureStore> _signatureHashtable;
        private readonly Hashtable _functionTypeHashTable;
        private readonly DoubleHashtable _modifiedTypeHashtable;
        private readonly Hashtable<MultiHashtable<SignatureStore>> _methodReferenceHashtable;

        public InternFactory()
        {
            _currentAssemblyInternValue = 0x00001000;
            _currentMethodReference = Dummy.MethodReference;
            _currentModuleInternValue = 0x00000001;
            _currentNamespaceInternValue = 0x00000001;
            _currentTypeInternValue = 0x00000100;
            _currentTypeListInternValue = 0x00000001;
            _currentCustomModifierInternValue = 0x00000001;
            _currentCustomModifierListInternValue = 0x00000001;
            _currentParameterTypeInternValue = 0x00000001;
            _currentParameterTypeListInternValue = 0x00000001;
            _currentSignatureInternValue = 0x00000001;
            _assemblyHashtable = new MultiHashtable<AssemblyStore>();
            _moduleHashtable = new MultiHashtable<ModuleStore>();
            _nestedNamespaceHashtable = new DoubleHashtable();
            _namespaceTypeHashtable = new MultiHashtable<NamespaceTypeStore>();
            _nestedTypeHashtable = new MultiHashtable<NestedTypeStore>();
            _vectorTypeHashTable = new Hashtable();
            _pointerTypeHashTable = new Hashtable();
            _managedPointerTypeHashTable = new Hashtable();
            _matrixTypeHashtable = new MultiHashtable<MatrixTypeStore>();
            _typeListHashtable = new DoubleHashtable();
            _genericInstanceHashtable = new DoubleHashtable();
            _genericTypeParameterHashtable = new DoubleHashtable();
            _genericMethodTypeParameterHashTable = new DoubleHashtable();
            _customModifierHashTable = new DoubleHashtable();
            _customModifierListHashTable = new DoubleHashtable();
            _parameterTypeHashtable = new MultiHashtable<ParameterTypeStore>();
            _parameterTypeListHashtable = new DoubleHashtable();
            _signatureHashtable = new MultiHashtable<SignatureStore>();
            _functionTypeHashTable = new Hashtable();
            _modifiedTypeHashtable = new DoubleHashtable();
            _methodReferenceHashtable = new Hashtable<MultiHashtable<SignatureStore>>();
        }

        private AssemblyStore GetAssemblyStore(AssemblyIdentity assemblyIdentity)
        {
            IName assemblyName = assemblyIdentity.Name;
            foreach (AssemblyStore aStore in _assemblyHashtable.GetValuesFor((uint)assemblyName.UniqueKey))
            {
                if (assemblyIdentity.Equals(aStore.AssemblyIdentity))
                {
                    return aStore;
                }
            }
            uint value = _currentAssemblyInternValue;
            _currentAssemblyInternValue += 0x00001000;
            AssemblyStore aStore1 = new AssemblyStore(assemblyIdentity, value, _currentNamespaceInternValue++);
            _assemblyHashtable.Add((uint)assemblyName.UniqueKey, aStore1);
            return aStore1;
        }

        private ModuleStore GetModuleStore(ModuleIdentity moduleIdentity)
        {
            IName moduleName = moduleIdentity.Name;
            foreach (ModuleStore mStore in _moduleHashtable.GetValuesFor((uint)moduleName.UniqueKey))
            {
                if (moduleIdentity.Equals(mStore.ModuleIdentitity))
                {
                    return mStore;
                }
            }
            uint value;
            if (moduleIdentity.ContainingAssembly != null)
            {
                AssemblyStore assemblyStore = this.GetAssemblyStore(moduleIdentity.ContainingAssembly);
                assemblyStore.InternedIdWithCount++;
                value = assemblyStore.InternedIdWithCount;
            }
            else
            {
                value = _currentModuleInternValue++;
            }
            ModuleStore mStore1 = new ModuleStore(moduleIdentity, value, _currentNamespaceInternValue++);
            _moduleHashtable.Add((uint)moduleName.UniqueKey, mStore1);
            return mStore1;
        }

        private uint GetUnitRootNamespaceInternId(
          IUnitReference unitReference,
          bool forPrivateModuleType
        )
        {
            IAssemblyReference/*?*/ assemblyReference = unitReference as IAssemblyReference;
            if (assemblyReference != null)
            {
                AssemblyStore assemblyStore = this.GetAssemblyStore(assemblyReference.UnifiedAssemblyIdentity);
                return assemblyStore.RootNamespaceInternedId;
            }
            IModuleReference/*?*/ moduleReference = unitReference as IModuleReference;
            if (moduleReference != null)
            {
                if (forPrivateModuleType && moduleReference.ContainingAssembly != null)
                {
                    AssemblyStore assemblyStore = this.GetAssemblyStore(moduleReference.ContainingAssembly.UnifiedAssemblyIdentity);
                    return assemblyStore.RootNamespaceInternedId;
                }
                ModuleStore moduleStore = this.GetModuleStore(moduleReference.ModuleIdentity);
                return moduleStore.RootNamespaceInternedId;
            }
            return 0;
        }

        private uint GetNestedNamespaceInternId(
          INestedUnitNamespaceReference nestedUnitNamespaceReference,
          bool forPrivateModuleType
        )
        {
            uint parentNamespaceInternedId = this.GetUnitNamespaceInternId(nestedUnitNamespaceReference.ContainingUnitNamespace, forPrivateModuleType);
            uint value = _nestedNamespaceHashtable.Find(parentNamespaceInternedId, (uint)nestedUnitNamespaceReference.Name.UniqueKey);
            if (value == 0)
            {
                value = _currentNamespaceInternValue++;
                _nestedNamespaceHashtable.Add(parentNamespaceInternedId, (uint)nestedUnitNamespaceReference.Name.UniqueKey, value);
            }
            return value;
        }

        private uint GetUnitNamespaceInternId(
          IUnitNamespaceReference unitNamespaceReference,
          bool forPrivateModuleType
        )
        {
            INestedUnitNamespaceReference/*?*/ nestedUnitNamespaceReference = unitNamespaceReference as INestedUnitNamespaceReference;
            if (nestedUnitNamespaceReference != null)
            {
                return this.GetNestedNamespaceInternId(nestedUnitNamespaceReference, forPrivateModuleType);
            }
            return this.GetUnitRootNamespaceInternId(unitNamespaceReference.Unit, forPrivateModuleType);
        }

        private uint GetNamespaceTypeReferenceInternId(
          IUnitNamespaceReference containingUnitNamespace,
          IName typeName,
          uint genericParameterCount,
          bool forPrivateModuleType
        )
        {
            uint containingUnitNamespaceInteredId = this.GetUnitNamespaceInternId(containingUnitNamespace, forPrivateModuleType);
            foreach (NamespaceTypeStore nsTypeStore in _namespaceTypeHashtable.GetValuesFor((uint)typeName.UniqueKey))
            {
                if (
                  nsTypeStore.ContainingNamespaceInternedId == containingUnitNamespaceInteredId
                  && nsTypeStore.GenericParameterCount == genericParameterCount
                )
                {
                    return nsTypeStore.InternedId;
                }
            }
            NamespaceTypeStore nsTypeStore1 = new NamespaceTypeStore(containingUnitNamespaceInteredId, genericParameterCount, _currentTypeInternValue++);
            _namespaceTypeHashtable.Add((uint)typeName.UniqueKey, nsTypeStore1);
            return nsTypeStore1.InternedId;
        }

        private uint GetNestedTypeReferenceInternId(
          ITypeReference containingTypeReference,
          IName typeName,
          uint genericParameterCount
        )
        {
            uint containingTypeReferenceInteredId = this.GetTypeReferenceInternId(containingTypeReference);
            foreach (NestedTypeStore nstTypeStore in _nestedTypeHashtable.GetValuesFor((uint)typeName.UniqueKey))
            {
                if (
                  nstTypeStore.ContainingTypeInternedId == containingTypeReferenceInteredId
                  && nstTypeStore.GenericParameterCount == genericParameterCount
                )
                {
                    return nstTypeStore.InternedId;
                }
            }
            NestedTypeStore nstTypeStore1 = new NestedTypeStore(containingTypeReferenceInteredId, genericParameterCount, _currentTypeInternValue++);
            _nestedTypeHashtable.Add((uint)typeName.UniqueKey, nstTypeStore1);
            return nstTypeStore1.InternedId;
        }

        private uint GetVectorTypeReferenceInternId(ITypeReference elementTypeReference)
        {
            uint elementTypeReferenceInternId = this.GetTypeReferenceInternId(elementTypeReference);
            uint value = _vectorTypeHashTable.Find(elementTypeReferenceInternId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _vectorTypeHashTable.Add(elementTypeReferenceInternId, value);
            }
            return value;
        }

        private uint GetMatrixTypeReferenceInternId(
          ITypeReference elementTypeReference,
          int rank,
          IEnumerable<ulong> sizes,
          IEnumerable<int> lowerBounds
        )
        {
            uint elementTypeReferenceInternId = this.GetTypeReferenceInternId(elementTypeReference);
            foreach (MatrixTypeStore matrixTypeStore in _matrixTypeHashtable.GetValuesFor(elementTypeReferenceInternId))
            {
                if (
                  matrixTypeStore.Rank == rank
                  && IteratorHelper.EnumerablesAreEqual<ulong>(matrixTypeStore.Sizes, sizes)
                  && IteratorHelper.EnumerablesAreEqual<int>(matrixTypeStore.LowerBounds, lowerBounds)
                )
                {
                    return matrixTypeStore.InternedId;
                }
            }
            MatrixTypeStore matrixTypeStore1 = new MatrixTypeStore(rank, new List<int>(lowerBounds).ToArray(), new List<ulong>(sizes).ToArray(), _currentTypeInternValue++);
            _matrixTypeHashtable.Add(elementTypeReferenceInternId, matrixTypeStore1);
            return matrixTypeStore1.InternedId;
        }

        private uint GetTypeReferenceListInternedId(IEnumerator<ITypeReference> typeReferences)
        {
            if (!typeReferences.MoveNext())
            {
                return 0;
            }
            ITypeReference currentTypeRef = typeReferences.Current;
            uint currentTypeRefInternedId = this.GetTypeReferenceInternId(currentTypeRef);
            uint tailInternedId = this.GetTypeReferenceListInternedId(typeReferences);
            uint value = _typeListHashtable.Find(currentTypeRefInternedId, tailInternedId);
            if (value == 0)
            {
                value = _currentTypeListInternValue++;
                _typeListHashtable.Add(currentTypeRefInternedId, tailInternedId, value);
            }
            return value;
        }

        private uint GetGenericTypeInstanceReferenceInternId(
          ITypeReference genericTypeReference,
          IEnumerable<ITypeReference> genericArguments
        )
        {
            uint genericTypeInternedId = this.GetTypeReferenceInternId(genericTypeReference);
            uint genericArgumentsInternedId = this.GetTypeReferenceListInternedId(genericArguments.GetEnumerator());
            uint value = _genericInstanceHashtable.Find(genericTypeInternedId, genericArgumentsInternedId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _genericInstanceHashtable.Add(genericTypeInternedId, genericArgumentsInternedId, value);
            }
            return value;
        }

        private uint GetPointerTypeReferenceInternId(ITypeReference targetTypeReference)
        {
            uint targetTypeReferenceInternId = this.GetTypeReferenceInternId(targetTypeReference);
            uint value = _pointerTypeHashTable.Find(targetTypeReferenceInternId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _pointerTypeHashTable.Add(targetTypeReferenceInternId, value);
            }
            return value;
        }

        private uint GetManagedPointerTypeReferenceInternId(ITypeReference targetTypeReference)
        {
            uint targetTypeReferenceInternId = this.GetTypeReferenceInternId(targetTypeReference);
            uint value = _managedPointerTypeHashTable.Find(targetTypeReferenceInternId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _managedPointerTypeHashTable.Add(targetTypeReferenceInternId, value);
            }
            return value;
        }

        private uint GetGenericTypeParameterReferenceInternId(
          ITypeReference definingTypeReference,
          int index
        )
        {
            uint definingTypeReferenceInternId = this.GetTypeReferenceInternId(GetUninstantiatedGenericType(definingTypeReference));
            uint value = _genericTypeParameterHashtable.Find(definingTypeReferenceInternId, (uint)index);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _genericTypeParameterHashtable.Add(definingTypeReferenceInternId, (uint)index, value);
            }
            return value;
        }

        private static ITypeReference GetUninstantiatedGenericType(ITypeReference typeReference)
        {
            IGenericTypeInstanceReference/*?*/ genericTypeInstanceReference = typeReference as IGenericTypeInstanceReference;
            if (genericTypeInstanceReference != null) return genericTypeInstanceReference.GenericType;
            INestedTypeReference/*?*/ nestedTypeReference = typeReference as INestedTypeReference;
            if (nestedTypeReference != null)
            {
                ISpecializedNestedTypeReference/*?*/ specializedNestedType = nestedTypeReference as ISpecializedNestedTypeReference;
                if (specializedNestedType != null) return specializedNestedType.UnspecializedVersion;
                return nestedTypeReference;
            }
            return typeReference;
        }

        /// <summary>
        /// Returns the interned key for the generic method parameter constructed with the given index
        /// </summary>
        /// <param name="definingMethodReference">A reference to the method defining the referenced generic parameter.</param>
        /// <param name="index">The index of the referenced generic parameter. This is an index rather than a name because metadata in CLR
        /// PE files contain only the index, not the name.</param>
        private uint GetGenericMethodParameterReferenceInternId(
          IMethodReference definingMethodReference,
          uint index
        )
        {
            if (_currentMethodReference != Dummy.MethodReference)
            {
                //this happens when the defining method reference contains a type in its signature which either is, or contains,
                //a reference to this generic method type parameter. In that case we break the cycle by just using the index of
                //the generic parameter. Only method references that refer to their own type parameters will ever
                //get this version of the interned id.
                return index + 1;
            }
            _currentMethodReference = definingMethodReference; //short circuit recursive calls back to this method
            uint definingMethodReferenceInternId = this.GetMethodReferenceInternedId(definingMethodReference);
            _currentMethodReference = Dummy.MethodReference;
            uint value = _genericMethodTypeParameterHashTable.Find(definingMethodReferenceInternId, index);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _genericMethodTypeParameterHashTable.Add(definingMethodReferenceInternId, index, value);
            }
            return value;
        }

        private uint GetParameterTypeInternId(IParameterTypeInformation parameterTypeInformation)
        {
            uint typeReferenceInternId = this.GetTypeReferenceInternId(parameterTypeInformation.Type);
            uint customModifiersInternId = 0;
            if (parameterTypeInformation.IsModified)
                customModifiersInternId = this.GetCustomModifierListInternId(parameterTypeInformation.CustomModifiers.GetEnumerator());
            foreach (ParameterTypeStore parameterTypeStore in _parameterTypeHashtable.GetValuesFor(typeReferenceInternId))
            {
                if (
                  parameterTypeStore.IsByReference == parameterTypeInformation.IsByReference
                  && parameterTypeStore.CustomModifiersInternId == customModifiersInternId
                )
                {
                    return parameterTypeStore.InternedId;
                }
            }
            ParameterTypeStore parameterTypeStore1 = new ParameterTypeStore(parameterTypeInformation.IsByReference, customModifiersInternId, _currentParameterTypeInternValue++);
            _parameterTypeHashtable.Add(typeReferenceInternId, parameterTypeStore1);
            return parameterTypeStore1.InternedId;
        }

        private uint GetParameterTypeListInternId(IEnumerator<IParameterTypeInformation> parameterTypeInformations)
        {
            if (!parameterTypeInformations.MoveNext())
            {
                return 0;
            }
            uint currentParameterInternedId = this.GetParameterTypeInternId(parameterTypeInformations.Current);
            uint tailInternedId = this.GetParameterTypeListInternId(parameterTypeInformations);
            uint value = _parameterTypeListHashtable.Find(currentParameterInternedId, tailInternedId);
            if (value == 0)
            {
                value = _currentParameterTypeListInternValue++;
                _parameterTypeListHashtable.Add(currentParameterInternedId, tailInternedId, value);
            }
            return value;
        }

        private uint GetSignatureInternId(
          CallingConvention callingConvention,
          IEnumerable<IParameterTypeInformation> parameters,
          IEnumerable<IParameterTypeInformation> extraArgumentTypes,
          IEnumerable<ICustomModifier> returnValueCustomModifiers,
          bool returnValueIsByRef,
          ITypeReference returnType
        )
        {
            uint requiredParameterTypesInternedId = this.GetParameterTypeListInternId(parameters.GetEnumerator());
            uint extraArgumentTypesInteredId = this.GetParameterTypeListInternId(extraArgumentTypes.GetEnumerator());
            uint returnValueCustomModifiersInternedId = this.GetCustomModifierListInternId(returnValueCustomModifiers.GetEnumerator());
            uint returnTypeReferenceInternedId = this.GetTypeReferenceInternId(returnType);
            foreach (SignatureStore signatureStore in _signatureHashtable.GetValuesFor(requiredParameterTypesInternedId))
            {
                if (
                  signatureStore.CallingConvention == callingConvention
                  && signatureStore.RequiredParameterListInternedId == requiredParameterTypesInternedId
                  && signatureStore.ExtraParameterListInternedId == extraArgumentTypesInteredId
                  && signatureStore.ReturnValueCustomModifiersListInteredId == returnValueCustomModifiersInternedId
                  && signatureStore.ReturnValueIsByRef == returnValueIsByRef
                  && signatureStore.ReturnTypeReferenceInternedId == returnTypeReferenceInternedId
                )
                {
                    return signatureStore.InternedId;
                }
            }
            SignatureStore signatureStore1 = new SignatureStore(callingConvention, requiredParameterTypesInternedId, extraArgumentTypesInteredId, returnValueIsByRef, returnValueCustomModifiersInternedId, returnTypeReferenceInternedId, _currentSignatureInternValue++);
            _signatureHashtable.Add(requiredParameterTypesInternedId, signatureStore1);
            return signatureStore1.InternedId;
        }

        private uint GetMethodReferenceInternedId(
          IMethodReference methodReference
        )
        {
            uint containingTypeReferenceInternedId = this.GetTypeReferenceInternId(methodReference.ContainingType);
            uint requiredParameterTypesInternedId = this.GetParameterTypeListInternId(methodReference.Parameters.GetEnumerator());
            uint returnValueCustomModifiersInternedId = 0;
            if (methodReference.ReturnValueIsModified)
                returnValueCustomModifiersInternedId = this.GetCustomModifierListInternId(methodReference.ReturnValueCustomModifiers.GetEnumerator());
            uint returnTypeReferenceInternedId = this.GetTypeReferenceInternId(methodReference.Type);
            MultiHashtable<SignatureStore>/*?*/ methods = _methodReferenceHashtable.Find(containingTypeReferenceInternedId);
            if (methods == null)
            {
                methods = new MultiHashtable<SignatureStore>();
                _methodReferenceHashtable.Add(containingTypeReferenceInternedId, methods);
            }
            foreach (SignatureStore signatureStore in methods.GetValuesFor((uint)methodReference.Name.UniqueKey))
            {
                if (
                  signatureStore.CallingConvention == methodReference.CallingConvention
                  && signatureStore.RequiredParameterListInternedId == requiredParameterTypesInternedId
                  && signatureStore.ReturnValueCustomModifiersListInteredId == returnValueCustomModifiersInternedId
                  && signatureStore.ReturnValueIsByRef == methodReference.ReturnValueIsByRef
                  && signatureStore.ReturnTypeReferenceInternedId == returnTypeReferenceInternedId
                )
                {
                    return signatureStore.InternedId;
                }
            }
            SignatureStore signatureStore1 = new SignatureStore(methodReference.CallingConvention, requiredParameterTypesInternedId,
              0, methodReference.ReturnValueIsByRef, returnValueCustomModifiersInternedId, returnTypeReferenceInternedId,
              _currentMethodReferenceInternValue++);
            methods.Add((uint)methodReference.Name.UniqueKey, signatureStore1);
            return signatureStore1.InternedId;
        }

        private uint GetFunctionPointerTypeReferenceInternId(
          CallingConvention callingConvention,
          IEnumerable<IParameterTypeInformation> parameters,
          IEnumerable<IParameterTypeInformation> extraArgumentTypes,
          IEnumerable<ICustomModifier> returnValueCustomModifiers,
          bool returnValueIsByRef,
          ITypeReference returnType
        )
        {
            uint signatureInternedId = this.GetSignatureInternId(
              callingConvention,
              parameters,
              extraArgumentTypes,
              returnValueCustomModifiers,
              returnValueIsByRef,
              returnType
            );
            uint value = _functionTypeHashTable.Find(signatureInternedId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _functionTypeHashTable.Add(signatureInternedId, value);
            }
            return value;
        }

        private uint GetCustomModifierInternId(ICustomModifier customModifier)
        {
            uint currentTypeRefInternedId = this.GetTypeReferenceInternId(customModifier.Modifier);
            uint isOptionalIntneredId = customModifier.IsOptional ? 0xF0F0F0F0 : 0x0F0F0F0F;  //  Just for the heck of it...
            uint value = _customModifierHashTable.Find(currentTypeRefInternedId, isOptionalIntneredId);
            if (value == 0)
            {
                value = _currentCustomModifierInternValue++;
                _customModifierHashTable.Add(currentTypeRefInternedId, isOptionalIntneredId, value);
            }
            return value;
        }

        private uint GetCustomModifierListInternId(IEnumerator<ICustomModifier> customModifiers)
        {
            if (!customModifiers.MoveNext())
            {
                return 0;
            }
            uint currentCustomModifierInternedId = this.GetCustomModifierInternId(customModifiers.Current);
            uint tailInternedId = this.GetCustomModifierListInternId(customModifiers);
            uint value = _customModifierListHashTable.Find(currentCustomModifierInternedId, tailInternedId);
            if (value == 0)
            {
                value = _currentCustomModifierListInternValue++;
                _customModifierListHashTable.Add(currentCustomModifierInternedId, tailInternedId, value);
            }
            return value;
        }

        private uint GetTypeReferenceInterendIdIgnoringCustomModifiers(ITypeReference typeReference)
        {
            INamespaceTypeReference/*?*/ namespaceTypeReference = typeReference as INamespaceTypeReference;
            if (namespaceTypeReference != null)
            {
                return this.GetNamespaceTypeReferenceInternId(
                  namespaceTypeReference.ContainingUnitNamespace,
                  namespaceTypeReference.Name,
                  namespaceTypeReference.GenericParameterCount,
                  !(namespaceTypeReference.ResolvedType == Dummy.NamespaceTypeDefinition || namespaceTypeReference.ResolvedType.IsPublic)
                );
            }
            INestedTypeReference/*?*/ nestedTypeReference = typeReference as INestedTypeReference;
            if (nestedTypeReference != null)
            {
                return this.GetNestedTypeReferenceInternId(
                  nestedTypeReference.ContainingType,
                  nestedTypeReference.Name,
                  nestedTypeReference.GenericParameterCount
                );
            }
            IArrayTypeReference/*?*/ arrayTypeReference = typeReference as IArrayTypeReference;
            if (arrayTypeReference != null)
            {
                if (arrayTypeReference.IsVector)
                {
                    return this.GetVectorTypeReferenceInternId(arrayTypeReference.ElementType);
                }
                else
                {
                    return this.GetMatrixTypeReferenceInternId(
                      arrayTypeReference.ElementType,
                      (int)arrayTypeReference.Rank,
                      arrayTypeReference.Sizes,
                      arrayTypeReference.LowerBounds
                    );
                }
            }
            IGenericTypeInstanceReference/*?*/ genericTypeInstanceReference = typeReference as IGenericTypeInstanceReference;
            if (genericTypeInstanceReference != null)
            {
                return this.GetGenericTypeInstanceReferenceInternId(
                  genericTypeInstanceReference.GenericType,
                  genericTypeInstanceReference.GenericArguments
                );
            }
            IPointerTypeReference/*?*/ pointerTypeReference = typeReference as IPointerTypeReference;
            if (pointerTypeReference != null)
            {
                return this.GetPointerTypeReferenceInternId(pointerTypeReference.TargetType);
            }
            IManagedPointerTypeReference managedPointerTypeReference = typeReference as IManagedPointerTypeReference;
            if (managedPointerTypeReference != null)
            {
                return this.GetManagedPointerTypeReferenceInternId(managedPointerTypeReference.TargetType);
            }
            IGenericTypeParameterReference/*?*/ genericTypeParameterReference = typeReference as IGenericTypeParameterReference;
            if (genericTypeParameterReference != null)
            {
                return this.GetGenericTypeParameterReferenceInternId(
                  genericTypeParameterReference.DefiningType,
                  (int)genericTypeParameterReference.Index
                );
            }
            IGenericMethodParameterReference/*?*/ genericMethodParameterReference = typeReference as IGenericMethodParameterReference;
            if (genericMethodParameterReference != null)
            {
                return this.GetGenericMethodParameterReferenceInternId(genericMethodParameterReference.DefiningMethod, genericMethodParameterReference.Index);
            }
            IFunctionPointerTypeReference/*?*/ functionPointerTypeReference = typeReference as IFunctionPointerTypeReference;
            if (functionPointerTypeReference != null)
            {
                IEnumerable<ICustomModifier> returnValueCustomModifiers;
                if (functionPointerTypeReference.ReturnValueIsModified)
                    returnValueCustomModifiers = functionPointerTypeReference.ReturnValueCustomModifiers;
                else
                    returnValueCustomModifiers = IteratorHelper.GetEmptyEnumerable<ICustomModifier>();
                return this.GetFunctionPointerTypeReferenceInternId(
                  functionPointerTypeReference.CallingConvention,
                  functionPointerTypeReference.Parameters,
                  functionPointerTypeReference.ExtraArgumentTypes,
                  returnValueCustomModifiers,
                  functionPointerTypeReference.ReturnValueIsByRef,
                  functionPointerTypeReference.Type
                );
            }
            //^ assume false; //It is an informal requirement that all classes implementing ITypeReference should produce a non null result for one of the calls above.
            return 0;
        }

        private uint GetModifiedTypeReferenceInternId(ITypeReference typeReference, IEnumerable<ICustomModifier> customModifiers)
        {
            uint typeReferenceInternId = this.GetTypeReferenceInterendIdIgnoringCustomModifiers(typeReference);
            uint customModifiersInternId = this.GetCustomModifierListInternId(customModifiers.GetEnumerator());
            uint value = _modifiedTypeHashtable.Find(typeReferenceInternId, customModifiersInternId);
            if (value == 0)
            {
                value = _currentTypeInternValue++;
                _modifiedTypeHashtable.Add(typeReferenceInternId, customModifiersInternId, value);
            }
            return value;
        }

        private uint GetTypeReferenceInternId(ITypeReference typeReference)
        {
            if (typeReference.IsAlias)
            {
                return this.GetTypeReferenceInternId(typeReference.AliasForType.AliasedType);
            }
            IModifiedTypeReference/*?*/ modifiedTypeReference = typeReference as IModifiedTypeReference;
            if (modifiedTypeReference != null)
            {
                return this.GetModifiedTypeReferenceInternId(modifiedTypeReference.UnmodifiedType, modifiedTypeReference.CustomModifiers);
            }
            return this.GetTypeReferenceInterendIdIgnoringCustomModifiers(typeReference);
        }

        #region IInternFactory Members

        // Interning of module and assembly
        // enables fast comparision of the nominal types. The interned module id takes into account the unification policy applied by the host.
        // For example if mscorlib 1.0 and mscorlib 2.0 is unified by the host both of them will have same intered id, otherwise not.
        // Interned id is 32 bit integer. it is split into 22 bit part and 10 bit part. First part represents the assembly and other part represents module.
        // Simple module which are not part of any assembly is represented by 0 in assembly part and number in module part.
        // Main module of the assembly is represented with something in the assembly part and 0 in the module part.
        // Other modules of the multimodule assembly are represented with assembly part being containing assembly and module part distinct for each module.
        // Note that this places limit on number of modules that can be loaded to be 2^20 and number of modules loaded at 2^12

        uint IInternFactory.GetAssemblyInternedKey(AssemblyIdentity assemblyIdentity)
        {
            lock (GlobalLock.LockingObject)
            {
                AssemblyStore assemblyStore = this.GetAssemblyStore(assemblyIdentity);
                return assemblyStore.InternedId;
            }
        }

        uint IInternFactory.GetMethodInternedKey(IMethodReference methodReference)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetMethodReferenceInternedId(methodReference);
            }
        }

        uint IInternFactory.GetModuleInternedKey(ModuleIdentity moduleIdentity)
        {
            lock (GlobalLock.LockingObject)
            {
                ModuleStore moduleStore = this.GetModuleStore(moduleIdentity);
                return moduleStore.InternedId;
            }
        }

        uint IInternFactory.GetVectorTypeReferenceInternedKey(ITypeReference elementTypeReference)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetVectorTypeReferenceInternId(elementTypeReference);
            }
        }

        uint IInternFactory.GetMatrixTypeReferenceInternedKey(ITypeReference elementTypeReference, int rank, IEnumerable<ulong> sizes, IEnumerable<int> lowerBounds)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetMatrixTypeReferenceInternId(elementTypeReference, rank, sizes, lowerBounds);
            }
        }

        uint IInternFactory.GetGenericTypeInstanceReferenceInternedKey(ITypeReference genericTypeReference, IEnumerable<ITypeReference> genericArguments)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetGenericTypeInstanceReferenceInternId(genericTypeReference, genericArguments);
            }
        }

        uint IInternFactory.GetPointerTypeReferenceInternedKey(ITypeReference targetTypeReference)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetPointerTypeReferenceInternId(targetTypeReference);
            }
        }

        uint IInternFactory.GetManagedPointerTypeReferenceInternedKey(ITypeReference targetTypeReference)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetManagedPointerTypeReferenceInternId(targetTypeReference);
            }
        }

        uint IInternFactory.GetFunctionPointerTypeReferenceInternedKey(CallingConvention callingConvention, IEnumerable<IParameterTypeInformation> parameters, IEnumerable<IParameterTypeInformation> extraArgumentTypes, IEnumerable<ICustomModifier> returnValueCustomModifiers, bool returnValueIsByRef, ITypeReference returnType)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetFunctionPointerTypeReferenceInternId(callingConvention, parameters, extraArgumentTypes, returnValueCustomModifiers, returnValueIsByRef, returnType);
            }
        }

        uint IInternFactory.GetTypeReferenceInternedKey(ITypeReference typeReference)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetTypeReferenceInternId(typeReference);
            }
        }

        uint IInternFactory.GetNamespaceTypeReferenceInternedKey(IUnitNamespaceReference containingUnitNamespace, IName typeName, uint genericParameterCount, bool forPrivateModuleType)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetNamespaceTypeReferenceInternId(containingUnitNamespace, typeName, genericParameterCount, forPrivateModuleType);
            }
        }

        uint IInternFactory.GetNestedTypeReferenceInternedKey(ITypeReference containingTypeReference, IName typeName, uint genericParameterCount)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetNestedTypeReferenceInternId(containingTypeReference, typeName, genericParameterCount);
            }
        }

        uint IInternFactory.GetGenericTypeParameterReferenceInternedKey(ITypeReference definingTypeReference, int index)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetGenericTypeParameterReferenceInternId(definingTypeReference, index);
            }
        }

        uint IInternFactory.GetModifiedTypeReferenceInternedKey(ITypeReference typeReference, IEnumerable<ICustomModifier> customModifiers)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetModifiedTypeReferenceInternId(typeReference, customModifiers);
            }
        }

        uint IInternFactory.GetGenericMethodParameterReferenceInternedKey(IMethodReference methodReference, int index)
        {
            lock (GlobalLock.LockingObject)
            {
                return this.GetGenericMethodParameterReferenceInternId(methodReference, (uint)index);
            }
        }

        #endregion IInternFactory Members
    }

    /// <summary>
    /// Reusable implementation of name table.
    /// </summary>
    public sealed class NameTable : INameTable
    {
        //TODO: replace BCL Dictionary with a private implementation that is thread safe and does not need a new list to be allocated for each name
        private Dictionary<string, int> _caseInsensitiveTable = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        //^ invariant forall{int i in caseInsensitiveTable.Values; i > 0};

        private Dictionary<string, IName> _caseSensitiveTable = new Dictionary<string, IName>();

        private int _caseInsensitiveCounter = 1; //^ invariant caseInsensitiveCounter >= 0;
        private int _caseSensitiveCounter = 3; //^ invariant caseSensitiveCounter >= 0;

        /// <summary>
        /// Constructor for the name table.
        /// </summary>
        //^ [NotDelayed]
        public NameTable()
        {
            _emptyName = Dummy.Name;
            //^ base();
            _emptyName = this.GetNameFor("");
        }

        IName INameTable.Address
        {
            get
            {
                if (_address == null)
                    _address = this.GetNameFor("Address");
                return _address;
            }
        }

        private IName/*?*/ _address;

        /// <summary>
        /// The Empty name.
        /// </summary>
        public IName EmptyName
        {
            get
            {
                return _emptyName;
            }
        }

        private readonly IName _emptyName;

        IName INameTable.Get
        {
            get
            {
                if (_get == null)
                    _get = this.GetNameFor("Get");
                return _get;
            }
        }

        private IName/*?*/ _get;

        /// <summary>
        /// Gets a cached IName instance corresponding to the given string. If no cached instance exists, a new instance is created.
        /// The method is only available to fully trusted code since it allows the caller to cause new objects to be added to the cache.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //^ [Pure]
        public IName GetNameFor(string name)
        //^^ ensures result.Value == name;
        {
            lock (_caseInsensitiveTable)
            {
                IName/*?*/ result;
                if (_caseSensitiveTable.TryGetValue(name, out result))
                {
                    //^ assume result != null; //Follows from the way Dictionary is instantiated, but the verifier is ignorant of this.
                    //^ assume result.Value == name; //Only this routine ever adds entries to the table and it only ever adds entries for which this is true. TODO: it would be nice to be able express this as an invariant.
                    return result;
                }
                //string lowerCaseName = name.ToLower(CultureInfo.InvariantCulture); //REVIEW: is it safer to use ToUpperInvariant, or does it make no difference?
                int caseInsensitiveCounter;
                if (!_caseInsensitiveTable.TryGetValue(name, out caseInsensitiveCounter))
                {
                    caseInsensitiveCounter = _caseInsensitiveCounter;
                    caseInsensitiveCounter += 17;
                    if (caseInsensitiveCounter <= 0)
                    {
                        caseInsensitiveCounter = (caseInsensitiveCounter - int.MinValue) + 1000000;
                    }
                    _caseInsensitiveCounter = caseInsensitiveCounter;
                    _caseInsensitiveTable.Add(name, caseInsensitiveCounter);
                }
                //^ assume caseInsensitiveCounter > 0; //Follows from the invariant, but this is beyond the verifier right now.
                int caseSensitiveCounter = _caseSensitiveCounter;
                caseSensitiveCounter += 17;
                if (caseSensitiveCounter <= 0)
                {
                    caseSensitiveCounter = (caseSensitiveCounter - int.MinValue) + 1000000;
                    //^ assume caseSensitiveCounter > 0;
                }
                result = new Name(caseInsensitiveCounter, caseSensitiveCounter, name);
                //^ assume result.Value == name;
                _caseSensitiveCounter = caseSensitiveCounter;
                _caseSensitiveTable.Add(name, result);
                return result;
            }
        }

        IName INameTable.global
        {
            get
            {
                if (_globalCache == null)
                    _globalCache = this.GetNameFor("global");
                return _globalCache;
            }
        }

        private IName/*?*/ _globalCache;

        private class Name : IName
        {
            private string _value;
            private int _caseInsensitiveUniqueKey; //^ invariant caseInsensitiveUniqueKey > 0;
            private int _uniqueKey; //^ invariant uniqueKey > 0;

            internal Name(int caseInsensitiveUniqueKey, int uniqueKey, string value)
            //^ requires caseInsensitiveUniqueKey > 0 && uniqueKey > 0;
            {
                _caseInsensitiveUniqueKey = caseInsensitiveUniqueKey;
                _uniqueKey = uniqueKey;
                _value = value;
            }

            public int UniqueKeyIgnoringCase
            {
                get { return _caseInsensitiveUniqueKey; }
            }

            public int UniqueKey
            {
                get { return _uniqueKey; }
            }

            public string Value
            {
                get { return _value; }
            }

            //^ [Confined]
            public override string ToString()
            {
                return this.Value;
            }
        }

        IName INameTable.AllowMultiple
        {
            get
            {
                if (_allowMultiple == null)
                    _allowMultiple = this.GetNameFor("AllowMultiple");
                return _allowMultiple;
            }
        }

        private IName/*?*/ _allowMultiple;

        IName INameTable.BoolOpBool
        {
            get
            {
                if (_boolOpBool == null)
                    _boolOpBool = this.GetNameFor("bool op bool");
                return _boolOpBool;
            }
        }

        private IName/*?*/ _boolOpBool;

        IName INameTable.DecimalOpDecimal
        {
            get
            {
                if (_decimalOpAddition == null)
                    _decimalOpAddition = this.GetNameFor("decimal op decimal");
                return _decimalOpAddition;
            }
        }

        private IName/*?*/ _decimalOpAddition;

        IName INameTable.DelegateOpDelegate
        {
            get
            {
                if (_delegateOpAddition == null)
                    _delegateOpAddition = this.GetNameFor("delegate op delegate");
                return _delegateOpAddition;
            }
        }

        private IName/*?*/ _delegateOpAddition;

        IName INameTable.EnumOpEnum
        {
            get
            {
                if (_enumOpEnum == null)
                    _enumOpEnum = this.GetNameFor("enum op enum");
                return _enumOpEnum;
            }
        }

        private IName/*?*/ _enumOpEnum;

        IName INameTable.EnumOpNum
        {
            get
            {
                if (_enumOpNum == null)
                    _enumOpNum = this.GetNameFor("enum op num");
                return _enumOpNum;
            }
        }

        private IName/*?*/ _enumOpNum;

        IName INameTable.Equals
        {
            get
            {
                if (_equals == null)
                    _equals = this.GetNameFor("Equals");
                return _equals;
            }
        }

        private IName/*?*/ _equals;

        IName INameTable.Float32OpFloat32
        {
            get
            {
                if (_float32OpAddition == null)
                    _float32OpAddition = this.GetNameFor("float32 op float32");
                return _float32OpAddition;
            }
        }

        private IName/*?*/ _float32OpAddition;

        IName INameTable.Float64OpFloat64
        {
            get
            {
                if (_float64OpAddition == null)
                    _float64OpAddition = this.GetNameFor("float64 op float64");
                return _float64OpAddition;
            }
        }

        private IName/*?*/ _float64OpAddition;

        IName INameTable.HasValue
        {
            get
            {
                if (_hasValue == null)
                    _hasValue = this.GetNameFor("HasValue");
                return _hasValue;
            }
        }

        private IName/*?*/ _hasValue;

        IName INameTable.Inherited
        {
            get
            {
                if (_inherited == null)
                    _inherited = this.GetNameFor("Inherited");
                return _inherited;
            }
        }

        private IName/*?*/ _inherited;

        IName INameTable.Invoke
        {
            get
            {
                if (_invoke == null)
                    _invoke = this.GetNameFor("Invoke");
                return _invoke;
            }
        }

        private IName/*?*/ _invoke;

        IName INameTable.Int16OpInt16
        {
            get
            {
                if (_int16OpInt16 == null)
                    _int16OpInt16 = this.GetNameFor("int16 op int16");
                return _int16OpInt16;
            }
        }

        private IName/*?*/ _int16OpInt16;

        IName INameTable.Int32OpInt32
        {
            get
            {
                if (_int32OpInt32 == null)
                    _int32OpInt32 = this.GetNameFor("int32 op int32");
                return _int32OpInt32;
            }
        }

        private IName/*?*/ _int32OpInt32;

        IName INameTable.Int32OpUInt32
        {
            get
            {
                if (_int32OpUInt32 == null)
                    _int32OpUInt32 = this.GetNameFor("int32 op uint32");
                return _int32OpUInt32;
            }
        }

        private IName/*?*/ _int32OpUInt32;

        IName INameTable.Int64OpInt32
        {
            get
            {
                if (_int64OpInt32 == null)
                    _int64OpInt32 = this.GetNameFor("int64 op int32");
                return _int64OpInt32;
            }
        }

        private IName/*?*/ _int64OpInt32;

        IName INameTable.Int64OpUInt32
        {
            get
            {
                if (_int64OpUInt32 == null)
                    _int64OpUInt32 = this.GetNameFor("int64 op uint32");
                return _int64OpUInt32;
            }
        }

        private IName/*?*/ _int64OpUInt32;

        IName INameTable.Int64OpUInt64
        {
            get
            {
                if (_int64OpUInt64 == null)
                    _int64OpUInt64 = this.GetNameFor("int64 op uint64");
                return _int64OpUInt64;
            }
        }

        private IName/*?*/ _int64OpUInt64;

        IName INameTable.Int64OpInt64
        {
            get
            {
                if (_int64OpInt64 == null)
                    _int64OpInt64 = this.GetNameFor("int64 op int64");
                return _int64OpInt64;
            }
        }

        private IName/*?*/ _int64OpInt64;

        IName INameTable.Int8OpInt8
        {
            get
            {
                if (_int8OpInt8 == null)
                    _int8OpInt8 = this.GetNameFor("int8 op int8");
                return _int8OpInt8;
            }
        }

        private IName/*?*/ _int8OpInt8;

        IName INameTable.NullCoalescing
        {
            get
            {
                if (_nullCoalescing == null)
                    _nullCoalescing = this.GetNameFor("operator ??(object, object)");
                return _nullCoalescing;
            }
        }

        private IName/*?*/ _nullCoalescing;

        IName INameTable.NumOpEnum
        {
            get
            {
                if (_numOpEnum == null)
                    _numOpEnum = this.GetNameFor("num op enum");
                return _numOpEnum;
            }
        }

        private IName/*?*/ _numOpEnum;

        IName INameTable.ObjectOpObject
        {
            get
            {
                if (_objectOpObject == null)
                    _objectOpObject = this.GetNameFor("object op object");
                return _objectOpObject;
            }
        }

        private IName/*?*/ _objectOpObject;

        IName INameTable.ObjectOpString
        {
            get
            {
                if (_objectOpString == null)
                    _objectOpString = this.GetNameFor("object op string");
                return _objectOpString;
            }
        }

        private IName/*?*/ _objectOpString;

        IName INameTable.OpAddition
        {
            get
            {
                if (_opAddition == null)
                    _opAddition = this.GetNameFor("op_Addition");
                return _opAddition;
            }
        }

        private IName/*?*/ _opAddition;

        IName INameTable.OpBoolean
        {
            get
            {
                if (_opBoolean == null)
                    _opBoolean = this.GetNameFor("op boolean");
                return _opBoolean;
            }
        }

        private IName/*?*/ _opBoolean;

        IName INameTable.OpChar
        {
            get
            {
                if (_opChar == null)
                    _opChar = this.GetNameFor("op char");
                return _opChar;
            }
        }

        private IName/*?*/ _opChar;

        IName INameTable.OpDecimal
        {
            get
            {
                if (_opDecimal == null)
                    _opDecimal = this.GetNameFor("op decimal");
                return _opDecimal;
            }
        }

        private IName/*?*/ _opDecimal;

        IName INameTable.OpEnum
        {
            get
            {
                if (_opEnum == null)
                    _opEnum = this.GetNameFor("op enum");
                return _opEnum;
            }
        }

        private IName/*?*/ _opEnum;

        IName INameTable.OpEquality
        {
            get
            {
                if (_opEquality == null)
                    _opEquality = this.GetNameFor("op_Equality");
                return _opEquality;
            }
        }

        private IName/*?*/ _opEquality;

        IName INameTable.OpExplicit
        {
            get
            {
                if (_opExplicit == null)
                    _opExplicit = this.GetNameFor("op_Explicit");
                return _opExplicit;
            }
        }

        private IName/*?*/ _opExplicit;

        IName INameTable.OpImplicit
        {
            get
            {
                if (_opImplicit == null)
                    _opImplicit = this.GetNameFor("op_Implicit");
                return _opImplicit;
            }
        }

        private IName/*?*/ _opImplicit;

        IName INameTable.OpInequality
        {
            get
            {
                if (_opInequality == null)
                    _opInequality = this.GetNameFor("op_Inequality");
                return _opInequality;
            }
        }

        private IName/*?*/ _opInequality;

        IName INameTable.OpInt8
        {
            get
            {
                if (_opInt8 == null)
                    _opInt8 = this.GetNameFor("op int8");
                return _opInt8;
            }
        }

        private IName/*?*/ _opInt8;

        IName INameTable.OpInt16
        {
            get
            {
                if (_opInt16 == null)
                    _opInt16 = this.GetNameFor("op int16");
                return _opInt16;
            }
        }

        private IName/*?*/ _opInt16;

        IName INameTable.OpInt32
        {
            get
            {
                if (_opInt32 == null)
                    _opInt32 = this.GetNameFor("op int32");
                return _opInt32;
            }
        }

        private IName/*?*/ _opInt32;

        IName INameTable.OpInt64
        {
            get
            {
                if (_opInt64 == null)
                    _opInt64 = this.GetNameFor("op int64");
                return _opInt64;
            }
        }

        private IName/*?*/ _opInt64;

        IName INameTable.OpBitwiseAnd
        {
            get
            {
                if (_opBitwiseAnd == null)
                    _opBitwiseAnd = this.GetNameFor("op_BitwiseAnd");
                return _opBitwiseAnd;
            }
        }

        private IName/*?*/ _opBitwiseAnd;

        IName INameTable.OpBitwiseOr
        {
            get
            {
                if (_opBitwiseOr == null)
                    _opBitwiseOr = this.GetNameFor("op_BitwiseOr");
                return _opBitwiseOr;
            }
        }

        private IName/*?*/ _opBitwiseOr;

        IName INameTable.OpComma
        {
            get
            {
                if (_opComma == null)
                    _opComma = this.GetNameFor("op_Comma");
                return _opComma;
            }
        }

        private IName/*?*/ _opComma;

        IName INameTable.OpConcatentation
        {
            get
            {
                if (_opConcatentation == null)
                    _opConcatentation = this.GetNameFor("op_Concatentation");
                return _opConcatentation;
            }
        }

        private IName/*?*/ _opConcatentation;

        IName INameTable.OpDivision
        {
            get
            {
                if (_opDivision == null)
                    _opDivision = this.GetNameFor("op_Division");
                return _opDivision;
            }
        }

        private IName/*?*/ _opDivision;

        IName INameTable.OpExclusiveOr
        {
            get
            {
                if (_opExclusiveOr == null)
                    _opExclusiveOr = this.GetNameFor("op_ExclusiveOr");
                return _opExclusiveOr;
            }
        }

        private IName/*?*/ _opExclusiveOr;

        IName INameTable.OpExponentiation
        {
            get
            {
                if (_opExponentiation == null)
                    _opExponentiation = this.GetNameFor("op_Exponentiation");
                return _opExponentiation;
            }
        }

        private IName/*?*/ _opExponentiation;

        IName INameTable.OpFalse
        {
            get
            {
                if (_opFalse == null)
                    _opFalse = this.GetNameFor("op_False");
                return _opFalse;
            }
        }

        private IName/*?*/ _opFalse;

        IName INameTable.OpFloat32
        {
            get
            {
                if (_opFloat32 == null)
                    _opFloat32 = this.GetNameFor("op float32");
                return _opFloat32;
            }
        }

        private IName/*?*/ _opFloat32;

        IName INameTable.OpFloat64
        {
            get
            {
                if (_opFloat64 == null)
                    _opFloat64 = this.GetNameFor("op float64");
                return _opFloat64;
            }
        }

        private IName/*?*/ _opFloat64;

        IName INameTable.OpGreaterThan
        {
            get
            {
                if (_opGreaterThan == null)
                    _opGreaterThan = this.GetNameFor("op_GreaterThan");
                return _opGreaterThan;
            }
        }

        private IName/*?*/ _opGreaterThan;

        IName INameTable.OpGreaterThanOrEqual
        {
            get
            {
                if (_opGreaterThanOrEqual == null)
                    _opGreaterThanOrEqual = this.GetNameFor("op_GreaterThanOrEqual");
                return _opGreaterThanOrEqual;
            }
        }

        private IName/*?*/ _opGreaterThanOrEqual;

        IName INameTable.OpIntegerDivision
        {
            get
            {
                if (_opIntegerDivision == null)
                    _opIntegerDivision = this.GetNameFor("op_IntegerDivision");
                return _opIntegerDivision;
            }
        }

        private IName/*?*/ _opIntegerDivision;

        IName INameTable.OpLeftShift
        {
            get
            {
                if (_opLeftShift == null)
                    _opLeftShift = this.GetNameFor("op_LeftShift");
                return _opLeftShift;
            }
        }

        private IName/*?*/ _opLeftShift;

        IName INameTable.OpLessThan
        {
            get
            {
                if (_opLessThan == null)
                    _opLessThan = this.GetNameFor("op_LessThan");
                return _opLessThan;
            }
        }

        private IName/*?*/ _opLessThan;

        IName INameTable.OpLessThanOrEqual
        {
            get
            {
                if (_opLessThanOrEqual == null)
                    _opLessThanOrEqual = this.GetNameFor("op_LessThanOrEqual");
                return _opLessThanOrEqual;
            }
        }

        private IName/*?*/ _opLessThanOrEqual;

        IName INameTable.OpLike
        {
            get
            {
                if (_opLogicalAnd == null)
                    _opLogicalAnd = this.GetNameFor("op_Like");
                return _opLogicalAnd;
            }
        }

        private IName/*?*/ _opLogicalAnd;

        IName INameTable.OpLogicalNot
        {
            get
            {
                if (_opLogicalNot == null)
                    _opLogicalNot = this.GetNameFor("op_LogicalNot");
                return _opLogicalNot;
            }
        }

        private IName/*?*/ _opLogicalNot;

        IName INameTable.OpLogicalOr
        {
            get
            {
                if (_opLogicalOr == null)
                    _opLogicalOr = this.GetNameFor("op_LogicalOr");
                return _opLogicalOr;
            }
        }

        private IName/*?*/ _opLogicalOr;

        IName INameTable.OpModulus
        {
            get
            {
                if (_opModulus == null)
                    _opModulus = this.GetNameFor("op_Modulus");
                return _opModulus;
            }
        }

        private IName/*?*/ _opModulus;

        IName INameTable.OpMultiply
        {
            get
            {
                if (_opMultiplication == null)
                    _opMultiplication = this.GetNameFor("op_Multiply");
                return _opMultiplication;
            }
        }

        private IName/*?*/ _opMultiplication;

        IName INameTable.OpOnesComplement
        {
            get
            {
                if (_opOnesComplement == null)
                    _opOnesComplement = this.GetNameFor("op_OnesComplement");
                return _opOnesComplement;
            }
        }

        private IName/*?*/ _opOnesComplement;

        IName INameTable.OpDecrement
        {
            get
            {
                if (_opDecrement == null)
                    _opDecrement = this.GetNameFor("op_Decrement");
                return _opDecrement;
            }
        }

        private IName/*?*/ _opDecrement;

        IName INameTable.OpIncrement
        {
            get
            {
                if (_opIncrement == null)
                    _opIncrement = this.GetNameFor("op_Increment");
                return _opIncrement;
            }
        }

        private IName/*?*/ _opIncrement;

        IName INameTable.OpRightShift
        {
            get
            {
                if (_opRightShift == null)
                    _opRightShift = this.GetNameFor("op_RightShift");
                return _opRightShift;
            }
        }

        private IName/*?*/ _opRightShift;

        IName INameTable.OpSubtraction
        {
            get
            {
                if (_opSubtraction == null)
                    _opSubtraction = this.GetNameFor("op_Subtraction");
                return _opSubtraction;
            }
        }

        private IName/*?*/ _opSubtraction;

        IName INameTable.OpTrue
        {
            get
            {
                if (_opTrue == null)
                    _opTrue = this.GetNameFor("op_True");
                return _opTrue;
            }
        }

        private IName/*?*/ _opTrue;

        IName INameTable.OpUInt8
        {
            get
            {
                if (_opUInt8 == null)
                    _opUInt8 = this.GetNameFor("op uint8");
                return _opUInt8;
            }
        }

        private IName/*?*/ _opUInt8;

        IName INameTable.OpUInt16
        {
            get
            {
                if (_opUInt16 == null)
                    _opUInt16 = this.GetNameFor("op uint16");
                return _opUInt16;
            }
        }

        private IName/*?*/ _opUInt16;

        IName INameTable.OpUInt32
        {
            get
            {
                if (_opUInt32 == null)
                    _opUInt32 = this.GetNameFor("op uint32");
                return _opUInt32;
            }
        }

        private IName/*?*/ _opUInt32;

        IName INameTable.OpUInt64
        {
            get
            {
                if (_opUInt64 == null)
                    _opUInt64 = this.GetNameFor("op uint64");
                return _opUInt64;
            }
        }

        private IName/*?*/ _opUInt64;

        IName INameTable.OpUnaryNegation
        {
            get
            {
                if (_opUnaryNegation == null)
                    _opUnaryNegation = this.GetNameFor("op_UnaryNegation");
                return _opUnaryNegation;
            }
        }

        private IName/*?*/ _opUnaryNegation;

        IName INameTable.OpUnaryPlus
        {
            get
            {
                if (_opUnaryPlus == null)
                    _opUnaryPlus = this.GetNameFor("op_UnaryPlus");
                return _opUnaryPlus;
            }
        }

        private IName/*?*/ _opUnaryPlus;

        IName INameTable.Remove
        {
            get
            {
                if (_remove == null)
                    _remove = this.GetNameFor("Remove");
                return _remove;
            }
        }

        private IName/*?*/ _remove;

        IName INameTable.Result
        {
            get
            {
                if (_result == null)
                    _result = this.GetNameFor("result");
                return _result;
            }
        }

        private IName/*?*/ _result;

        IName INameTable.StringOpString
        {
            get
            {
                if (_stringOpString == null)
                    _stringOpString = this.GetNameFor("string op string");
                return _stringOpString;
            }
        }

        private IName/*?*/ _stringOpString;

        IName INameTable.StringOpObject
        {
            get
            {
                if (_stringOpObject == null)
                    _stringOpObject = this.GetNameFor("string op object");
                return _stringOpObject;
            }
        }

        private IName/*?*/ _stringOpObject;

        IName INameTable.UInt32OpInt32
        {
            get
            {
                if (_uint32OpInt32 == null)
                    _uint32OpInt32 = this.GetNameFor("uint32 op int32");
                return _uint32OpInt32;
            }
        }

        private IName/*?*/ _uint32OpInt32;

        IName INameTable.UInt32OpUInt32
        {
            get
            {
                if (_uint32OpUInt32 == null)
                    _uint32OpUInt32 = this.GetNameFor("uint32 op uint32");
                return _uint32OpUInt32;
            }
        }

        private IName/*?*/ _uint32OpUInt32;

        IName INameTable.UInt64OpInt32
        {
            get
            {
                if (_uint64OpInt32 == null)
                    _uint64OpInt32 = this.GetNameFor("uint64 op int32");
                return _uint64OpInt32;
            }
        }

        private IName/*?*/ _uint64OpInt32;

        IName INameTable.UInt64OpUInt32
        {
            get
            {
                if (_uint64OpUInt32 == null)
                    _uint64OpUInt32 = this.GetNameFor("uint64 op uint32");
                return _uint64OpUInt32;
            }
        }

        private IName/*?*/ _uint64OpUInt32;

        IName INameTable.UInt64OpUInt64
        {
            get
            {
                if (_uint64OpUInt64 == null)
                    _uint64OpUInt64 = this.GetNameFor("uint64 op uint64");
                return _uint64OpUInt64;
            }
        }

        private IName/*?*/ _uint64OpUInt64;

        IName INameTable.UIntPtrOpUIntPtr
        {
            get
            {
                if (_uintPtrOpUIntPtr == null)
                    _uintPtrOpUIntPtr = this.GetNameFor("uintPtr op uintPtr");
                return _uintPtrOpUIntPtr;
            }
        }

        private IName/*?*/ _uintPtrOpUIntPtr;

        IName INameTable.value
        {
            get
            {
                if (_valueCache == null)
                    _valueCache = this.GetNameFor("value");
                return _valueCache;
            }
        }

        private IName/*?*/ _valueCache;

        IName INameTable.VoidPtrOpVoidPtr
        {
            get
            {
                if (_voidPtrOpVoidPtr == null)
                    _voidPtrOpVoidPtr = this.GetNameFor("void* op void*");
                return _voidPtrOpVoidPtr;
            }
        }

        private IName/*?*/ _voidPtrOpVoidPtr;

        IName INameTable.System
        {
            get
            {
                if (_systemCache == null)
                    _systemCache = this.GetNameFor("System");
                return _systemCache;
            }
        }

        private IName/*?*/ _systemCache;

        IName INameTable.Void
        {
            get
            {
                if (_voidCache == null)
                    _voidCache = this.GetNameFor("Void");
                return _voidCache;
            }
        }

        private IName/*?*/ _voidCache;

        IName INameTable.Boolean
        {
            get
            {
                if (_booleanCache == null)
                    _booleanCache = this.GetNameFor("Boolean");
                return _booleanCache;
            }
        }

        private IName/*?*/ _booleanCache;

        IName INameTable.Cctor
        {
            get
            {
                if (_cctorCache == null)
                    _cctorCache = this.GetNameFor(".cctor");
                return _cctorCache;
            }
        }

        private IName/*?*/ _cctorCache;

        IName INameTable.Char
        {
            get
            {
                if (_charCache == null)
                    _charCache = this.GetNameFor("Char");
                return _charCache;
            }
        }

        private IName/*?*/ _charCache;

        IName INameTable.Ctor
        {
            get
            {
                if (_ctorCache == null)
                    _ctorCache = this.GetNameFor(".ctor");
                return _ctorCache;
            }
        }

        private IName/*?*/ _ctorCache;

        IName INameTable.Byte
        {
            get
            {
                if (_byteCache == null)
                    _byteCache = this.GetNameFor("Byte");
                return _byteCache;
            }
        }

        private IName/*?*/ _byteCache;

        IName INameTable.SByte
        {
            get
            {
                if (_sbyteCache == null)
                    _sbyteCache = this.GetNameFor("SByte");
                return _sbyteCache;
            }
        }

        private IName/*?*/ _sbyteCache;

        IName INameTable.Int16
        {
            get
            {
                if (_int16Cache == null)
                    _int16Cache = this.GetNameFor("Int16");
                return _int16Cache;
            }
        }

        private IName/*?*/ _int16Cache;

        IName INameTable.UInt16
        {
            get
            {
                if (_uint16Cache == null)
                    _uint16Cache = this.GetNameFor("UInt16");
                return _uint16Cache;
            }
        }

        private IName/*?*/ _uint16Cache;

        IName INameTable.Int32
        {
            get
            {
                if (_int32Cache == null)
                    _int32Cache = this.GetNameFor("Int32");
                return _int32Cache;
            }
        }

        private IName/*?*/ _int32Cache;

        IName INameTable.UInt32
        {
            get
            {
                if (_uint32Cache == null)
                    _uint32Cache = this.GetNameFor("UInt32");
                return _uint32Cache;
            }
        }

        private IName/*?*/ _uint32Cache;

        IName INameTable.Int64
        {
            get
            {
                if (_int64Cache == null)
                    _int64Cache = this.GetNameFor("Int64");
                return _int64Cache;
            }
        }

        private IName/*?*/ _int64Cache;

        IName INameTable.UInt64
        {
            get
            {
                if (_uint64Cache == null)
                    _uint64Cache = this.GetNameFor("UInt64");
                return _uint64Cache;
            }
        }

        private IName/*?*/ _uint64Cache;

        IName INameTable.String
        {
            get
            {
                if (_stringCache == null)
                    _stringCache = this.GetNameFor("String");
                return _stringCache;
            }
        }

        private IName/*?*/ _stringCache;

        IName INameTable.IntPtr
        {
            get
            {
                if (_intPtrCache == null)
                    _intPtrCache = this.GetNameFor("IntPtr");
                return _intPtrCache;
            }
        }

        private IName/*?*/ _intPtrCache;

        IName INameTable.UIntPtr
        {
            get
            {
                if (_uintPtrCache == null)
                    _uintPtrCache = this.GetNameFor("UIntPtr");
                return _uintPtrCache;
            }
        }

        private IName/*?*/ _uintPtrCache;

        IName INameTable.Object
        {
            get
            {
                if (_objectCache == null)
                    _objectCache = this.GetNameFor("Object");
                return _objectCache;
            }
        }

        private IName/*?*/ _objectCache;

        IName INameTable.Set
        {
            get
            {
                if (_set == null)
                    _set = this.GetNameFor("Set");
                return _set;
            }
        }

        private IName/*?*/ _set;

        IName INameTable.Single
        {
            get
            {
                if (_singleCache == null)
                    _singleCache = this.GetNameFor("Single");
                return _singleCache;
            }
        }

        private IName/*?*/ _singleCache;

        IName INameTable.Double
        {
            get
            {
                if (_doubleCache == null)
                    _doubleCache = this.GetNameFor("Double");
                return _doubleCache;
            }
        }

        private IName/*?*/ _doubleCache;

        IName INameTable.TypedReference
        {
            get
            {
                if (_typedReferenceCache == null)
                    _typedReferenceCache = this.GetNameFor("TypedReference");
                return _typedReferenceCache;
            }
        }

        private IName/*?*/ _typedReferenceCache;

        IName INameTable.Enum
        {
            get
            {
                if (_enumCache == null)
                    _enumCache = this.GetNameFor("Enum");
                return _enumCache;
            }
        }

        private IName/*?*/ _enumCache;

        IName INameTable.MulticastDelegate
        {
            get
            {
                if (_multicastDelegateCache == null)
                    _multicastDelegateCache = this.GetNameFor("MulticastDelegate");
                return _multicastDelegateCache;
            }
        }

        private IName/*?*/ _multicastDelegateCache;

        IName INameTable.ValueType
        {
            get
            {
                if (_valueTypeCache == null)
                    _valueTypeCache = this.GetNameFor("ValueType");
                return _valueTypeCache;
            }
        }

        private IName/*?*/ _valueTypeCache;

        IName INameTable.Type
        {
            get
            {
                if (_type == null)
                    _type = this.GetNameFor("Type");
                return _type;
            }
        }

        private IName/*?*/ _type;

        IName INameTable.Array
        {
            get
            {
                if (_array == null)
                    _array = this.GetNameFor("Array");
                return _array;
            }
        }

        private IName/*?*/ _array;

        IName INameTable.AttributeUsageAttribute
        {
            get
            {
                if (_attributeUsage == null)
                    _attributeUsage = this.GetNameFor("AttributeUsageAttribute");
                return _attributeUsage;
            }
        }

        private IName/*?*/ _attributeUsage;

        IName INameTable.Attribute
        {
            get
            {
                if (_attribute == null)
                    _attribute = this.GetNameFor("Attribute");
                return _attribute;
            }
        }

        private IName/*?*/ _attribute;

        IName INameTable.DateTime
        {
            get
            {
                if (_dateTime == null)
                    _dateTime = this.GetNameFor("DateTime");
                return _dateTime;
            }
        }

        private IName/*?*/ _dateTime;

        IName INameTable.DebuggerHiddenAttribute
        {
            get
            {
                if (_debuggerHiddenAttribute == null)
                    _debuggerHiddenAttribute = this.GetNameFor("DebuggerHiddenAttribute");
                return _debuggerHiddenAttribute;
            }
        }

        private IName/*?*/ _debuggerHiddenAttribute;

        IName INameTable.Decimal
        {
            get
            {
                if (_decimal == null)
                    _decimal = this.GetNameFor("Decimal");
                return _decimal;
            }
        }

        private IName/*?*/ _decimal;

        IName INameTable.Delegate
        {
            get
            {
                if (_delegate == null)
                    _delegate = this.GetNameFor("Delegate");
                return _delegate;
            }
        }

        private IName/*?*/ _delegate;

        IName INameTable.Diagnostics
        {
            get
            {
                if (_diagnostics == null)
                    _diagnostics = this.GetNameFor("Diagnostics");
                return _diagnostics;
            }
        }

        private IName/*?*/ _diagnostics;

        IName INameTable.DBNull
        {
            get
            {
                if (_dbNull == null)
                    _dbNull = this.GetNameFor("DBNull");
                return _dbNull;
            }
        }

        private IName/*?*/_dbNull;

        IName INameTable.Length
        {
            get
            {
                if (_length == null)
                    _length = this.GetNameFor("Length");
                return _length;
            }
        }

        private IName/*?*/ _length;

        IName INameTable.LongLength
        {
            get
            {
                if (_longLength == null)
                    _longLength = this.GetNameFor("LongLength");
                return _longLength;
            }
        }

        private IName/*?*/ _longLength;

        IName INameTable.Nullable
        {
            get
            {
                if (_nullable == null)
                    _nullable = this.GetNameFor("Nullable");
                return _nullable;
            }
        }

        private IName/*?*/ _nullable;

        IName INameTable.Combine
        {
            get
            {
                if (_combine == null)
                    _combine = this.GetNameFor("Combine");
                return _combine;
            }
        }

        private IName/*?*/ _combine;

        IName INameTable.Concat
        {
            get
            {
                if (_concat == null)
                    _concat = this.GetNameFor("Concat");
                return _concat;
            }
        }

        private IName/*?*/ _concat;
    }
}