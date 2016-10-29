//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

//^ using Microsoft.Contracts;

namespace Microsoft.Cci
{
    /// <summary>
    /// A reference to a .NET assembly.
    /// </summary>
    public sealed class AssemblyReference : IAssemblyReference
    {
        /// <summary>
        /// Allocates a reference to a .NET assembly.
        /// </summary>
        /// <param name="host">Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.</param>
        /// <param name="assemblyIdentity">The identity of the referenced assembly.</param>
        public AssemblyReference(IMetadataHost host, AssemblyIdentity assemblyIdentity)
        {
            _host = host;
            _assemblyIdentity = assemblyIdentity;
        }

        /// <summary>
        /// A list of aliases for the root namespace of the referenced assembly.
        /// </summary>
        public IEnumerable<IName> Aliases
        {
            get { return IteratorHelper.GetEmptyEnumerable<IName>(); }
        }

        /// <summary>
        /// The identity of the referenced assembly.
        /// </summary>
        public AssemblyIdentity AssemblyIdentity
        {
            get { return _assemblyIdentity; }
        }

        private readonly AssemblyIdentity _assemblyIdentity;

        /// <summary>
        /// A collection of metadata custom attributes that are associated with this definition.
        /// </summary>
        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        /// <summary>
        /// The Assembly that contains this module. May be null if the module is not part of an assembly.
        /// </summary>
        public IAssemblyReference/*?*/ ContainingAssembly
        {
            get { return this; }
        }

        /// <summary>
        /// Identifies the culture associated with the assembly reference. Typically specified for sattelite assemblies with localized resources.
        /// Empty if not specified.
        /// </summary>
        public string Culture
        {
            get { return this.AssemblyIdentity.Culture; }
        }

        /// <summary>
        /// Calls visitor.Visit(IAssemblyReference).
        /// </summary>
        public void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.
        /// </summary>
        private readonly IMetadataHost _host;

        /// <summary>
        /// A potentially empty collection of locations that correspond to this AssemblyReference instance.
        /// </summary>
        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        /// <summary>
        /// The identity of the referenced module.
        /// </summary>
        public ModuleIdentity ModuleIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        /// <summary>
        /// The name of the referenced assembly.
        /// </summary>
        public IName Name
        {
            get { return this.AssemblyIdentity.Name; }
        }

        /// <summary>
        /// The hashed 8 bytes of the public key of the referenced assembly. This is empty if the referenced assembly does not have a public key.
        /// </summary>
        public IEnumerable<byte> PublicKeyToken
        {
            get { return this.AssemblyIdentity.PublicKeyToken; }
        }

        /// <summary>
        /// The referenced assembly, or Dummy.Assembly if the reference cannot be resolved.
        /// </summary>
        public IAssembly ResolvedAssembly
        {
            get { return _host.FindAssembly(this.UnifiedAssemblyIdentity); }
        }

        /// <summary>
        /// The referenced module, or Dummy.Module if the reference cannot be resolved.
        /// </summary>
        public IModule ResolvedModule
        {
            get
            {
                if (this.ResolvedAssembly == Dummy.Assembly) return Dummy.Module;
                return this.ResolvedAssembly;
            }
        }

        /// <summary>
        /// The referenced unit, or Dummy.Unit if the reference cannot be resolved.
        /// </summary>
        public IUnit ResolvedUnit
        {
            get
            {
                if (this.ResolvedModule == Dummy.Module) return Dummy.Unit;
                return this.ResolvedModule;
            }
        }

        /// <summary>
        /// Returns the identity of the assembly reference to which this assembly reference has been unified.
        /// </summary>
        public AssemblyIdentity UnifiedAssemblyIdentity
        {
            get { return _host.UnifyAssembly(this.AssemblyIdentity); }
        }

        /// <summary>
        /// The identity of the unit reference.
        /// </summary>
        public UnitIdentity UnitIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        /// <summary>
        /// The version of the referenced assembly.
        /// </summary>
        public Version Version
        {
            get { return this.AssemblyIdentity.Version; }
        }
    }

    /// <summary>
    /// A reference to a type.
    /// </summary>
    public abstract class BaseTypeReference : ITypeReference
    {
        /// <summary>
        /// Allocates a reference to a type.
        /// </summary>
        /// <param name="host">Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.</param>
        /// <param name="isEnum">True if the type is an enumeration (it extends System.Enum and is sealed). Corresponds to C# enum.</param>
        /// <param name="isValueType">True if the referenced type is a value type.</param>
        protected BaseTypeReference(IMetadataHost host, bool isEnum, bool isValueType)
        {
            this.host = host;
            _isEnum = isEnum;
            _isValueType = isValueType;
        }

        /// <summary>
        /// Gives the alias for the type
        /// </summary>
        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        /// <summary>
        /// A collection of metadata custom attributes that are associated with this definition.
        /// </summary>
        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        /// <summary>
        /// Calls the visitor.Visit(T) method where T is the most derived object model node interface type implemented by the concrete type
        /// of the object implementing IDefinition. The dispatch method does not invoke Dispatch on any child objects. If child traversal
        /// is desired, the implementations of the Visit methods should do the subsequent dispatching.
        /// </summary>
        public abstract void Dispatch(IMetadataVisitor visitor);

        /// <summary>
        /// Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.
        /// </summary>
        protected readonly IMetadataHost host;

        /// <summary>
        /// Returns the unique interned key associated with the type. This takes unification/aliases/custom modifiers into account .
        /// </summary>
        public abstract uint InternedKey
        {
            get;
        }

        /// <summary>
        /// Indicates if this type reference resolved to an alias rather than a type
        /// </summary>
        public bool IsAlias
        {
            get { return false; }
        }

        /// <summary>
        /// True if the type is an enumeration (it extends System.Enum and is sealed). Corresponds to C# enum.
        /// </summary>
        public bool IsEnum
        {
            get { return _isEnum; }
        }

        private readonly bool _isEnum;

        /// <summary>
        /// True if the type is a value type.
        /// Value types are sealed and extend System.ValueType or System.Enum.
        /// A type parameter for which MustBeValueType (the struct constraint in C#) is true also returns true for this property.
        /// </summary>
        public bool IsValueType
        {
            get { return _isValueType; }
        }

        private readonly bool _isValueType;

        /// <summary>
        /// A potentially empty collection of locations that correspond to this IReference instance.
        /// </summary>
        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        /// <summary>
        /// A collection of references to types from the core platform, such as System.Object and System.String.
        /// </summary>
        public IPlatformType PlatformType
        {
            get { return this.host.PlatformType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get
            {
                return this.Resolve();
            }
        }

        /// <summary>
        /// The type definition being referred to.
        /// In case this type was alias, this is also the type of the aliased type
        /// </summary>
        protected abstract ITypeDefinition Resolve();

        //^ ensures this.IsAlias ==> result == this.AliasForType.AliasedType.ResolvedType;
        //^ ensures (this is ITypeDefinition) ==> result == this;

        /// <summary>
        /// Unless the value of TypeCode is PrimitiveTypeCode.NotPrimitive, the type corresponds to a "primitive: CLR type (such as System.Int32) and
        /// the type code identifies which of the primitive types it corresponds to.
        /// </summary>
        public virtual PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.NotPrimitive; }
        }
    }

    /// <summary>
    /// A reference to a .NET module.
    /// </summary>
    public sealed class ModuleReference : IModuleReference
    {
        /// <summary>
        /// Allocates a reference to a .NET module.
        /// </summary>
        /// <param name="host">Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.</param>
        /// <param name="moduleIdentity"></param>
        public ModuleReference(IMetadataHost host, ModuleIdentity moduleIdentity)
        {
            _host = host;
            _moduleIdentity = moduleIdentity;
        }

        /// <summary>
        /// A collection of metadata custom attributes that are associated with this definition.
        /// </summary>
        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        /// <summary>
        /// The Assembly that contains this module. May be null if the module is not part of an assembly.
        /// </summary>
        public IAssemblyReference/*?*/ ContainingAssembly
        {
            get
            {
                if (this.ModuleIdentity.ContainingAssembly == null) return null;
                return new AssemblyReference(_host, this.ModuleIdentity.ContainingAssembly);
            }
        }

        /// <summary>
        /// Calls visitor.Visit(IModuleReference).
        /// </summary>
        public void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.
        /// </summary>
        private readonly IMetadataHost _host;

        /// <summary>
        /// A potentially empty collection of locations that correspond to this ModuleReference instance.
        /// </summary>
        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        /// <summary>
        /// The identity of the referenced module.
        /// </summary>
        public ModuleIdentity ModuleIdentity
        {
            get { return _moduleIdentity; }
        }

        private readonly ModuleIdentity _moduleIdentity;

        /// <summary>
        /// The name of the referenced assembly.
        /// </summary>
        public IName Name
        {
            get { return this.ModuleIdentity.Name; }
        }

        /// <summary>
        /// The referenced module, or Dummy.Module if the reference cannot be resolved.
        /// </summary>
        public IModule ResolvedModule
        {
            get { return _host.FindModule(this.ModuleIdentity); }
        }

        /// <summary>
        /// The referenced unit, or Dummy.Unit if the reference cannot be resolved.
        /// </summary>
        public IUnit ResolvedUnit
        {
            get
            {
                if (this.ResolvedModule == Dummy.Module) return Dummy.Unit;
                return this.ResolvedModule;
            }
        }

        /// <summary>
        /// The identity of the unit reference.
        /// </summary>
        public UnitIdentity UnitIdentity
        {
            get { return this.ModuleIdentity; }
        }
    }

    /// <summary>
    /// A reference to a type definition that is a member of a namespace definition.
    /// </summary>
    public class NamespaceTypeReference : BaseTypeReference, INamespaceTypeReference
    {
        /// <summary>
        /// Allocates a type definition that is a member of a namespace definition.
        /// </summary>
        /// <param name="host">Provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.</param>
        /// <param name="containingUnitNamespace">The namespace that contains the referenced type.</param>
        /// <param name="name">The name of the referenced type.</param>
        /// <param name="genericParameterCount">The number of generic parameters. Zero if the type is not generic.</param>
        /// <param name="isEnum">True if the type is an enumeration (it extends System.Enum and is sealed). Corresponds to C# enum.</param>
        /// <param name="isValueType">True if the referenced type is a value type.</param>
        /// <param name="typeCode">A value indicating if the type is a primitive type or not.</param>
        public NamespaceTypeReference(IMetadataHost host, IUnitNamespaceReference containingUnitNamespace, IName name, ushort genericParameterCount, bool isEnum, bool isValueType, PrimitiveTypeCode typeCode)
          : base(host, isEnum, isValueType)
        {
            _containingUnitNamespace = containingUnitNamespace;
            _name = name;
            _genericParameterCount = genericParameterCount;
            _typeCode = typeCode;
        }

        /// <summary>
        /// The namespace that contains the referenced type.
        /// </summary>
        public IUnitNamespaceReference ContainingUnitNamespace
        {
            get { return _containingUnitNamespace; }
        }

        private readonly IUnitNamespaceReference _containingUnitNamespace;

        /// <summary>
        /// Calls visitor.Visit(INamespaceTypeReference)
        /// </summary>
        public override void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The number of generic parameters. Zero if the type is not generic.
        /// </summary>
        public ushort GenericParameterCount
        {
            get { return _genericParameterCount; }
        }

        private readonly ushort _genericParameterCount;

        /// <summary>
        /// Returns the unique interned key associated with the type. This takes unification/aliases/custom modifiers into account .
        /// </summary>
        public override uint InternedKey
        {
            get
            {
                if (_internedKey == 0)
                    _internedKey = this.host.InternFactory.GetNamespaceTypeReferenceInternedKey(this.ContainingUnitNamespace, this.Name, this.GenericParameterCount, false);
                return _internedKey;
            }
        }

        private uint _internedKey;

        /// <summary>
        /// The name of the referenced type.
        /// </summary>
        public IName Name
        {
            get { return _name; }
        }

        private readonly IName _name;

        /// <summary>
        /// The namespace type this reference resolves to.
        /// </summary>
        public INamespaceTypeDefinition ResolvedType
        {
            get
            {
                if (_resolvedType == null)
                    _resolvedType = this.GetResolvedType();
                return _resolvedType;
            }
        }

        private INamespaceTypeDefinition/*?*/ _resolvedType;

        /// <summary>
        /// The namespace type this reference resolves to.
        /// </summary>
        private INamespaceTypeDefinition GetResolvedType()
        {
            foreach (INamespaceMember nsMember in this.ContainingUnitNamespace.ResolvedUnitNamespace.GetMembersNamed(this.Name, false))
            {
                INamespaceTypeDefinition/*?*/ nsTypeDef = nsMember as INamespaceTypeDefinition;
                if (nsTypeDef != null) return nsTypeDef;
            }
            return Dummy.NamespaceTypeDefinition;
        }

        /// <summary>
        /// The type definition being referred to.
        /// In case this type was alias, this is also the type of the aliased type
        /// </summary>
        protected override ITypeDefinition Resolve()
        //^^ ensures this.IsAlias ==> result == this.AliasForType.AliasedType.ResolvedType;
        //^^ ensures (this is ITypeDefinition) ==> result == this;
        {
            return this.ResolvedType;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        public override string ToString()
        {
            return TypeHelper.GetTypeName(this);
        }

        /// <summary>
        /// Unless the value of TypeCode is PrimitiveTypeCode.NotPrimitive, the type corresponds to a "primitive: CLR type (such as System.Int32) and
        /// the type code identifies which of the primitive types it corresponds to.
        /// </summary>
        public override PrimitiveTypeCode TypeCode
        {
            get { return _typeCode; }
        }

        private readonly PrimitiveTypeCode _typeCode;

        /// <summary>
        /// If true, the type name is mangled by appending "`n" where n is the number of type parameters, if the number of type parameters is greater than 0.
        /// </summary>
        public bool MangleName
        {
            get { return true; }
        }

        INamedTypeDefinition INamedTypeReference.ResolvedType
        {
            get { return this.ResolvedType; }
        }
    }

    /// <summary>
    /// A reference to a nested unit namespace.
    /// </summary>
    public sealed class NestedUnitNamespaceReference : INestedUnitNamespaceReference
    {
        /// <summary>
        /// Allocates a reference to a nested unit namespace.
        /// </summary>
        /// <param name="containingUnitNamespace">A reference to the unit namespace that contains the referenced nested unit namespace.</param>
        /// <param name="name">The name of the referenced nested unit namespace.</param>
        public NestedUnitNamespaceReference(IUnitNamespaceReference containingUnitNamespace, IName name)
        {
            _containingUnitNamespace = containingUnitNamespace;
            _name = name;
        }

        /// <summary>
        /// A reference to the unit namespace that contains the referenced nested unit namespace.
        /// </summary>
        public IUnitNamespaceReference ContainingUnitNamespace
        {
            get { return _containingUnitNamespace; }
        }

        private readonly IUnitNamespaceReference _containingUnitNamespace;

        /// <summary>
        /// Calls the visitor.Visit(INestedUnitNamespaceReference) method.
        /// </summary>
        public void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The name of the referenced nested unit namespace.
        /// </summary>
        public IName Name
        {
            get { return _name; }
        }

        private readonly IName _name;

        /// <summary>
        /// The namespace definition being referred to.
        /// </summary>
        public INestedUnitNamespace ResolvedNestedUnitNamespace
        {
            get
            {
                foreach (INamespaceMember member in this.ContainingUnitNamespace.ResolvedUnitNamespace.GetMembersNamed(this.Name, false))
                {
                    INestedUnitNamespace/*?*/ nuns = member as INestedUnitNamespace;
                    if (nuns != null) return nuns;
                }
                return Dummy.NestedUnitNamespace;
            }
        }

        /// <summary>
        /// A reference to the unit that defines the referenced namespace.
        /// </summary>
        public IUnitReference Unit
        {
            get { return this.ContainingUnitNamespace.Unit; }
        }

        IUnitNamespace IUnitNamespaceReference.ResolvedUnitNamespace
        {
            get { return this.ResolvedNestedUnitNamespace; }
        }

        IEnumerable<ICustomAttribute> IReference.Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        IEnumerable<ILocation> IObjectWithLocations.Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }
    }

    /// <summary>
    /// A collection of references to types from the core platform, such as System.Object and System.String.
    /// </summary>
    public class PlatformType : IPlatformType
    {
        private IMetadataHost _host;

        /// <summary>
        /// Allocates a collection of references to types from the core platform, such as System.Object and System.String.
        /// </summary>
        /// <param name="host">
        /// An object that provides a standard abstraction over the applications that host components that provide or consume objects from the metadata model.
        /// </param>
        public PlatformType(IMetadataHost host)
        {
            _host = host;
        }

        /// <summary>
        /// Creates a type reference to a namespace type from the given assembly, where the last element of the names
        /// array is the name of the type and the other elements are the names of the namespaces.
        /// </summary>
        /// <param name="assemblyReference">A reference to the assembly that contains the type for which a reference is desired.</param>
        /// <param name="names">The last entry of this array is the name of the type, the others are the names of the containing namespaces.</param>
        protected INamespaceTypeReference CreateReference(IAssemblyReference assemblyReference, params string[] names)
        {
            return this.CreateReference(assemblyReference, false, 0, PrimitiveTypeCode.NotPrimitive, names);
        }

        /// <summary>
        /// Creates a type reference to a namespace type from the given assembly, where the last element of the names
        /// array is the name of the type and the other elements are the names of the namespaces.
        /// </summary>
        /// <param name="assemblyReference">A reference to the assembly that contains the type for which a reference is desired.</param>
        /// <param name="isValueType">True if the referenced type is known to be a value type.</param>
        /// <param name="names">The last entry of this array is the name of the type, the others are the names of the containing namespaces.</param>
        protected INamespaceTypeReference CreateReference(IAssemblyReference assemblyReference, bool isValueType, params string[] names)
        {
            return this.CreateReference(assemblyReference, isValueType, 0, PrimitiveTypeCode.NotPrimitive, names);
        }

        /// <summary>
        /// Creates a type reference to a namespace type from the given assembly, where the last element of the names
        /// array is the name of the type and the other elements are the names of the namespaces.
        /// </summary>
        /// <param name="assemblyReference">A reference to the assembly that contains the type for which a reference is desired.</param>
        /// <param name="typeCode">A code that identifies what kind of type is being referenced.</param>
        /// <param name="names">The last entry of this array is the name of the type, the others are the names of the containing namespaces.</param>
        protected INamespaceTypeReference CreateReference(IAssemblyReference assemblyReference, PrimitiveTypeCode typeCode, params string[] names)
        {
            return this.CreateReference(assemblyReference, true, 0, typeCode, names);
        }

        /// <summary>
        /// Creates a type reference to a namespace type from the given assembly, where the last element of the names
        /// array is the name of the type and the other elements are the names of the namespaces.
        /// </summary>
        /// <param name="assemblyReference">A reference to the assembly that contains the type for which a reference is desired.</param>
        /// <param name="genericParameterCount">The number of generic parameters, if any, that the type has must. Must be zero or more.</param>
        /// <param name="names">The last entry of this array is the name of the type, the others are the names of the containing namespaces.</param>
        protected INamespaceTypeReference CreateReference(IAssemblyReference assemblyReference, ushort genericParameterCount, params string[] names)
        {
            return this.CreateReference(assemblyReference, false, genericParameterCount, PrimitiveTypeCode.NotPrimitive, names);
        }

        /// <summary>
        /// Creates a type reference to a namespace type from the given assembly, where the last element of the names
        /// array is the name of the type and the other elements are the names of the namespaces.
        /// </summary>
        /// <param name="assemblyReference">A reference to the assembly that contains the type for which a reference is desired.</param>
        /// <param name="isValueType">True if the referenced type is known to be a value type.</param>
        /// <param name="genericParameterCount">The number of generic parameters, if any, that the type has must. Must be zero or more.</param>
        /// <param name="typeCode">A code that identifies what kind of type is being referenced.</param>
        /// <param name="names">The last entry of this array is the name of the type, the others are the names of the containing namespaces.</param>
        protected INamespaceTypeReference CreateReference(IAssemblyReference assemblyReference, bool isValueType, ushort genericParameterCount, PrimitiveTypeCode typeCode, params string[] names)
        {
            IUnitNamespaceReference ns = new RootUnitNamespaceReference(assemblyReference);
            for (int i = 0, n = names.Length - 1; i < n; i++)
                ns = new NestedUnitNamespaceReference(ns, _host.NameTable.GetNameFor(names[i]));
            return new NamespaceTypeReference(_host, ns, _host.NameTable.GetNameFor(names[names.Length - 1]), genericParameterCount, false, isValueType, typeCode);
        }

        /// <summary>
        /// A reference to the assembly that contains the types and methods used to encode information about code contracts.
        /// </summary>
        protected IAssemblyReference ContractAssemblyRef
        {
            get
            {
                if (_contractAssemblyRef == null)
                    _contractAssemblyRef = new AssemblyReference(_host, _host.ContractAssemblySymbolicIdentity);
                return _contractAssemblyRef;
            }
        }

        private IAssemblyReference/*?*/ _contractAssemblyRef;

        /// <summary>
        /// A reference to the assembly that contains the system types that have special encodings in metadata.
        /// </summary>
        protected IAssemblyReference CoreAssemblyRef
        {
            get
            {
                if (_coreAssemblyRef == null)
                    _coreAssemblyRef = new AssemblyReference(_host, _host.CoreAssemblySymbolicIdentity);
                return _coreAssemblyRef;
            }
        }

        private IAssemblyReference/*?*/ _coreAssemblyRef;

        #region IPlatformType Members

        /// <summary>
        /// A reference to the class that contains the standard contract methods, such as System.Diagnostics.Contracts.Contract.Requires.
        /// </summary>
        public INamespaceTypeReference SystemDiagnosticsContractsContract
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemDiagnosticsContractsContract == null)
                {
                    _systemDiagnosticsContractsContract = this.CreateReference(this.ContractAssemblyRef, "System", "Diagnostics", "Contracts", "Contract");
                }
                return _systemDiagnosticsContractsContract;
            }
        }

        private INamespaceTypeReference/*?*/ _systemDiagnosticsContractsContract;

        /// <summary>
        /// The size (in bytes) of a pointer on the platform on which these types are implemented.
        /// The value of this property is either 4 (32-bits) or 8 (64-bit).
        /// </summary>
        public byte PointerSize
        {
            get { return _host.PointerSize; }
        }

        /// <summary>
        /// System.ArgIterator
        /// </summary>
        public INamespaceTypeReference SystemArgIterator
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemArgIterator == null)
                {
                    _systemArgIterator = this.CreateReference(this.CoreAssemblyRef, true, "System", "ArgIterator");
                }
                return _systemArgIterator;
            }
        }

        private INamespaceTypeReference/*?*/ _systemArgIterator;

        /// <summary>
        /// System.Array
        /// </summary>
        public INamespaceTypeReference SystemArray
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemArray == null)
                {
                    _systemArray = this.CreateReference(this.CoreAssemblyRef, "System", "Array");
                }
                return _systemArray;
            }
        }

        private INamespaceTypeReference/*?*/ _systemArray;

        /// <summary>
        /// System.AsyncCallBack
        /// </summary>
        public INamespaceTypeReference SystemAsyncCallback
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemAsyncCallback == null)
                {
                    _systemAsyncCallback = this.CreateReference(this.CoreAssemblyRef, "System", "AsyncCallback");
                }
                return _systemAsyncCallback;
            }
        }

        private INamespaceTypeReference/*?*/ _systemAsyncCallback;

        /// <summary>
        /// System.Attribute
        /// </summary>
        public INamespaceTypeReference SystemAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemAttribute == null)
                {
                    _systemAttribute = this.CreateReference(this.CoreAssemblyRef, "System", "Attribute");
                }
                return _systemAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemAttribute;

        /// <summary>
        /// System.AttributeUsageAttribute
        /// </summary>
        public INamespaceTypeReference SystemAttributeUsageAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemAttributeUsageAttribute == null)
                {
                    _systemAttributeUsageAttribute = this.CreateReference(this.CoreAssemblyRef, "System", "AttributeUsageAttribute");
                }
                return _systemAttributeUsageAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemAttributeUsageAttribute;

        /// <summary>
        /// System.Boolean
        /// </summary>
        public INamespaceTypeReference SystemBoolean
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemBoolean == null)
                {
                    _systemBoolean = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Boolean, "System", "Boolean");
                }
                return _systemBoolean;
            }
        }

        private INamespaceTypeReference/*?*/ _systemBoolean;

        /// <summary>
        /// System.Char
        /// </summary>
        public INamespaceTypeReference SystemChar
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemChar == null)
                {
                    _systemChar = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Char, "System", "Char");
                }
                return _systemChar;
            }
        }

        private INamespaceTypeReference/*?*/ _systemChar;

        /// <summary>
        /// System.Collections.Generic.Dictionary
        /// </summary>
        public INamespaceTypeReference SystemCollectionsGenericDictionary
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsGenericDictionary == null)
                {
                    _systemCollectionsGenericDictionary = this.CreateReference(this.CoreAssemblyRef, 2, "System", "Collections", "Generic", "Dictionary");
                }
                return _systemCollectionsGenericDictionary;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsGenericDictionary;

        /// <summary>
        /// System.Collections.Generic.ICollection
        /// </summary>
        public INamespaceTypeReference SystemCollectionsGenericICollection
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsGenericICollection == null)
                {
                    _systemCollectionsGenericICollection = this.CreateReference(this.CoreAssemblyRef, 1, "System", "Collections", "Generic", "ICollection");
                }
                return _systemCollectionsGenericICollection;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsGenericICollection;

        /// <summary>
        /// System.Collections.Generic.IEnumerable
        /// </summary>
        public INamespaceTypeReference SystemCollectionsGenericIEnumerable
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsGenericIEnumerable == null)
                {
                    _systemCollectionsGenericIEnumerable = this.CreateReference(this.CoreAssemblyRef, 1, "System", "Collections", "Generic", "IEnumerable");
                }
                return _systemCollectionsGenericIEnumerable;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsGenericIEnumerable;

        /// <summary>
        /// System.Collections.Generic.IEnumerator
        /// </summary>
        public INamespaceTypeReference SystemCollectionsGenericIEnumerator
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsGenericIEnumerator == null)
                {
                    _systemCollectionsGenericIEnumerator = this.CreateReference(this.CoreAssemblyRef, 1, "System", "Collections", "Generic", "IEnumerator");
                }
                return _systemCollectionsGenericIEnumerator;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsGenericIEnumerator;

        /// <summary>
        /// System.Collections.Generic.IList
        /// </summary>
        public INamespaceTypeReference SystemCollectionsGenericIList
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsGenericIList == null)
                {
                    _systemCollectionsGenericIList = this.CreateReference(this.CoreAssemblyRef, 1, "System", "Collections", "Generic", "IList");
                }
                return _systemCollectionsGenericIList;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsGenericIList;

        /// <summary>
        /// System.Collections.ICollection
        /// </summary>
        public INamespaceTypeReference SystemCollectionsICollection
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsICollection == null)
                {
                    _systemCollectionsICollection = this.CreateReference(this.CoreAssemblyRef, "System", "Collections", "ICollection");
                }
                return _systemCollectionsICollection;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsICollection;

        /// <summary>
        /// System.Collections.IEnumerable
        /// </summary>
        public INamespaceTypeReference SystemCollectionsIEnumerable
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsIEnumerable == null)
                {
                    _systemCollectionsIEnumerable = this.CreateReference(this.CoreAssemblyRef, "System", "Collections", "IEnumerable");
                }
                return _systemCollectionsIEnumerable;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsIEnumerable;

        /// <summary>
        /// System.Collections.IEnumerator
        /// </summary>
        public INamespaceTypeReference SystemCollectionsIEnumerator
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsIEnumerator == null)
                {
                    _systemCollectionsIEnumerator = this.CreateReference(this.CoreAssemblyRef, "System", "Collections", "IEnumerator");
                }
                return _systemCollectionsIEnumerator;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsIEnumerator;

        /// <summary>
        /// System.Collections.IList
        /// </summary>
        public INamespaceTypeReference SystemCollectionsIList
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemCollectionsIList == null)
                {
                    _systemCollectionsIList = this.CreateReference(this.CoreAssemblyRef, "System", "Collections", "IList");
                }
                return _systemCollectionsIList;
            }
        }

        private INamespaceTypeReference/*?*/ _systemCollectionsIList;

        /// <summary>
        /// System.DateTime
        /// </summary>
        public INamespaceTypeReference SystemDateTime
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemDateTime == null)
                {
                    _systemDateTime = this.CreateReference(this.CoreAssemblyRef, true, "System", "DateTime");
                }
                return _systemDateTime;
            }
        }

        private INamespaceTypeReference/*?*/ _systemDateTime;

        /// <summary>
        /// System.Decimal
        /// </summary>
        public INamespaceTypeReference SystemDecimal
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemDecimal == null)
                {
                    _systemDecimal = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.NotPrimitive, "System", "Decimal");
                }
                return _systemDecimal;
            }
        }

        private INamespaceTypeReference/*?*/ _systemDecimal;

        /// <summary>
        /// System.Delegate
        /// </summary>
        public INamespaceTypeReference SystemDelegate
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemDelegate == null)
                {
                    _systemDelegate = this.CreateReference(this.CoreAssemblyRef, "System", "Delegate");
                }
                return _systemDelegate;
            }
        }

        private INamespaceTypeReference/*?*/ _systemDelegate;

        /// <summary>
        /// System.DBNull
        /// </summary>
        public INamespaceTypeReference SystemDBNull
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemDBNull == null)
                {
                    _systemDBNull = this.CreateReference(this.CoreAssemblyRef, true, "System", "DBNull");
                }
                return _systemDBNull;
            }
        }

        private INamespaceTypeReference/*?*/ _systemDBNull;

        /// <summary>
        /// System.Enum
        /// </summary>
        public INamespaceTypeReference SystemEnum
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemEnum == null)
                {
                    _systemEnum = this.CreateReference(this.CoreAssemblyRef, "System", "Enum");
                }
                return _systemEnum;
            }
        }

        private INamespaceTypeReference/*?*/ _systemEnum;

        /// <summary>
        /// System.Float32
        /// </summary>
        public INamespaceTypeReference SystemFloat32
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemFloat32 == null)
                {
                    _systemFloat32 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Float32, "System", "Single");
                }
                return _systemFloat32;
            }
        }

        private INamespaceTypeReference/*?*/ _systemFloat32;

        /// <summary>
        /// System.Float64
        /// </summary>
        public INamespaceTypeReference SystemFloat64
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemFloat64 == null)
                {
                    _systemFloat64 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Float64, "System", "Double");
                }
                return _systemFloat64;
            }
        }

        private INamespaceTypeReference/*?*/ _systemFloat64;

        /// <summary>
        /// System.IAsyncResult
        /// </summary>
        public INamespaceTypeReference SystemIAsyncResult
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemIAsyncResult == null)
                {
                    _systemIAsyncResult = this.CreateReference(this.CoreAssemblyRef, "System", "IAsyncResult");
                }
                return _systemIAsyncResult;
            }
        }

        private INamespaceTypeReference/*?*/ _systemIAsyncResult;

        /// <summary>
        /// System.ICloneable
        /// </summary>
        public INamespaceTypeReference SystemICloneable
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemICloneable == null)
                {
                    _systemICloneable = this.CreateReference(this.CoreAssemblyRef, "System", "ICloneable");
                }
                return _systemICloneable;
            }
        }

        private INamespaceTypeReference/*?*/ _systemICloneable;

        /// <summary>
        /// System.Int16
        /// </summary>
        public INamespaceTypeReference SystemInt16
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemInt16 == null)
                {
                    _systemInt16 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Int16, "System", "Int16");
                }
                return _systemInt16;
            }
        }

        private INamespaceTypeReference/*?*/ _systemInt16;

        /// <summary>
        /// System.Int32
        /// </summary>
        public INamespaceTypeReference SystemInt32
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemInt32 == null)
                {
                    _systemInt32 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Int32, "System", "Int32");
                }
                return _systemInt32;
            }
        }

        private INamespaceTypeReference/*?*/ _systemInt32;

        /// <summary>
        /// System.Int64
        /// </summary>
        public INamespaceTypeReference SystemInt64
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemInt64 == null)
                {
                    _systemInt64 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Int64, "System", "Int64");
                }
                return _systemInt64;
            }
        }

        private INamespaceTypeReference/*?*/ _systemInt64;

        /// <summary>
        /// System.Int8
        /// </summary>
        public INamespaceTypeReference SystemInt8
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemInt8 == null)
                {
                    _systemInt8 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Int8, "System", "SByte");
                }
                return _systemInt8;
            }
        }

        private INamespaceTypeReference/*?*/ _systemInt8;

        /// <summary>
        /// System.IntPtr
        /// </summary>
        public INamespaceTypeReference SystemIntPtr
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemIntPtr == null)
                {
                    _systemIntPtr = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.IntPtr, "System", "IntPtr");
                }
                return _systemIntPtr;
            }
        }

        private INamespaceTypeReference/*?*/ _systemIntPtr;

        /// <summary>
        /// System.MulticastDelegate
        /// </summary>
        public INamespaceTypeReference SystemMulticastDelegate
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemMulticastDelegate == null)
                {
                    _systemMulticastDelegate = this.CreateReference(this.CoreAssemblyRef, "System", "MulticastDelegate");
                }
                return _systemMulticastDelegate;
            }
        }

        private INamespaceTypeReference/*?*/ _systemMulticastDelegate;

        /// <summary>
        /// System.Nullable&lt;T&gt;
        /// </summary>
        public INamespaceTypeReference SystemNullable
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemNullable == null)
                {
                    _systemNullable = this.CreateReference(this.CoreAssemblyRef, true, 1, PrimitiveTypeCode.NotPrimitive, "System", "Nullable");
                }
                return _systemNullable;
            }
        }

        private INamespaceTypeReference/*?*/ _systemNullable;

        /// <summary>
        /// System.Object
        /// </summary>
        public INamespaceTypeReference SystemObject
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemObject == null)
                {
                    _systemObject = this.CreateReference(this.CoreAssemblyRef, "System", "Object");
                }
                return _systemObject;
            }
        }

        private INamespaceTypeReference/*?*/ _systemObject;

        /// <summary>
        /// System.RuntimeArgumentHandle
        /// </summary>
        public INamespaceTypeReference SystemRuntimeArgumentHandle
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeArgumentHandle == null)
                {
                    _systemRuntimeArgumentHandle = this.CreateReference(this.CoreAssemblyRef, true, "System", "RuntimeArgumentHandle");
                }
                return _systemRuntimeArgumentHandle;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeArgumentHandle;

        /// <summary>
        /// System.RuntimeFieldHandle
        /// </summary>
        public INamespaceTypeReference SystemRuntimeFieldHandle
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeFieldHandle == null)
                {
                    _systemRuntimeFieldHandle = this.CreateReference(this.CoreAssemblyRef, true, "System", "RuntimeFieldHandle");
                }
                return _systemRuntimeFieldHandle;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeFieldHandle;

        /// <summary>
        /// System.RuntimeMethodHandle
        /// </summary>
        public INamespaceTypeReference SystemRuntimeMethodHandle
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeMethodHandle == null)
                {
                    _systemRuntimeMethodHandle = this.CreateReference(this.CoreAssemblyRef, true, "System", "RuntimeMethodHandle");
                }
                return _systemRuntimeMethodHandle;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeMethodHandle;

        /// <summary>
        /// System.RuntimeTypeHandle
        /// </summary>
        public INamespaceTypeReference SystemRuntimeTypeHandle
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeTypeHandle == null)
                {
                    _systemRuntimeTypeHandle = this.CreateReference(this.CoreAssemblyRef, true, "System", "RuntimeTypeHandle");
                }
                return _systemRuntimeTypeHandle;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeTypeHandle;

        /// <summary>
        /// System.Runtime.CompilerServices.CallConvCdecl
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesCallConvCdecl
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesCallConvCdecl == null)
                {
                    _systemRuntimeCompilerServicesCallConvCdecl =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "CallConvDecl");
                }
                return _systemRuntimeCompilerServicesCallConvCdecl;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesCallConvCdecl;

        /// <summary>
        /// System.Runtime.CompilerServices.CompilerGeneratedAttribute
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesCompilerGeneratedAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesCompilerGeneratedAttribute == null)
                {
                    _systemRuntimeCompilerServicesCompilerGeneratedAttribute =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "CompilerGeneratedAttribute");
                }
                return _systemRuntimeCompilerServicesCompilerGeneratedAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesCompilerGeneratedAttribute;

        /// <summary>
        /// System.Runtime.CompilerServices.FriendAccessAllowedAttribute
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesFriendAccessAllowedAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesFriendAccessAllowedAttribute == null)
                {
                    _systemRuntimeCompilerServicesFriendAccessAllowedAttribute =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "FriendAccessAllowedAttribute");
                }
                return _systemRuntimeCompilerServicesFriendAccessAllowedAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesFriendAccessAllowedAttribute;

        /// <summary>
        /// System.Runtime.CompilerServices.IsConst
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesIsConst
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesIsConst == null)
                {
                    _systemRuntimeCompilerServicesIsConst =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "IsConst");
                }
                return _systemRuntimeCompilerServicesIsConst;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesIsConst;

        /// <summary>
        /// System.Runtime.CompilerServices.IsVolatile
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesIsVolatile
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesIsVolatile == null)
                {
                    _systemRuntimeCompilerServicesIsVolatile =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "IsVolatile");
                }
                return _systemRuntimeCompilerServicesIsVolatile;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesIsVolatile;

        /// <summary>
        /// System.Runtime.CompilerServices.ReferenceAssemblyAttribute
        /// </summary>
        public INamespaceTypeReference SystemRuntimeCompilerServicesReferenceAssemblyAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeCompilerServicesReferenceAssemblyAttribute == null)
                {
                    _systemRuntimeCompilerServicesReferenceAssemblyAttribute =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "CompilerServices", "ReferenceAssemblyAttribute");
                }
                return _systemRuntimeCompilerServicesReferenceAssemblyAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeCompilerServicesReferenceAssemblyAttribute;

        /// <summary>
        /// System.Runtime.InteropServices.DllImportAttribute
        /// </summary>
        public INamespaceTypeReference SystemRuntimeInteropServicesDllImportAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemRuntimeInteropServicesDllImportAttribute == null)
                {
                    _systemRuntimeInteropServicesDllImportAttribute =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Runtime", "InteropServices", "DllImportAttribute");
                }
                return _systemRuntimeInteropServicesDllImportAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemRuntimeInteropServicesDllImportAttribute;

        /// <summary>
        /// System.Security.Permissions.SecurityAction
        /// </summary>
        public INamespaceTypeReference SystemSecurityPermissionsSecurityAction
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemSecurityPermissionsSecurityAction == null)
                {
                    _systemSecurityPermissionsSecurityAction =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Security", "Permissions", "SecurityAction");
                }
                return _systemSecurityPermissionsSecurityAction;
            }
        }

        private INamespaceTypeReference/*?*/ _systemSecurityPermissionsSecurityAction;

        /// <summary>
        /// System.Security.SecurityCriticalAttribute
        /// </summary>
        public INamespaceTypeReference SystemSecuritySecurityCriticalAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemSecuritySecurityCriticalAttribute == null)
                {
                    _systemSecuritySecurityCriticalAttribute =
                      this.CreateReference(this.CoreAssemblyRef, "System", "Security", "SecurityCriticalAttribute");
                }
                return _systemSecuritySecurityCriticalAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemSecuritySecurityCriticalAttribute;

        /// <summary>
        /// System.Security.SecuritySafeCriticalAttribute
        /// </summary>
        public INamespaceTypeReference SystemSecuritySecuritySafeCriticalAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemSecuritySecuritySafeCriticalAttribute == null)
                {
                    _systemSecuritySecuritySafeCriticalAttribute =
                        this.CreateReference(this.CoreAssemblyRef, "System", "Security", "SecuritySafeCriticalAttribute");
                }
                return _systemSecuritySecuritySafeCriticalAttribute;
            }
        }

        private INamespaceTypeReference/*?*/ _systemSecuritySecuritySafeCriticalAttribute;

        /// <summary>
        /// System.String
        /// </summary>
        public INamespaceTypeReference SystemString
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemString == null)
                {
                    _systemString = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.String, "System", "String");
                }
                return _systemString;
            }
        }

        private INamespaceTypeReference/*?*/ _systemString;

        /// <summary>
        /// System.Type
        /// </summary>
        public INamespaceTypeReference SystemType
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemType == null)
                {
                    _systemType = this.CreateReference(this.CoreAssemblyRef, "System", "Type");
                }
                return _systemType;
            }
        }

        private INamespaceTypeReference/*?*/ _systemType;

        /// <summary>
        /// System.TypedReference
        /// </summary>
        public INamespaceTypeReference SystemTypedReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemTypedReference == null)
                {
                    _systemTypedReference = this.CreateReference(this.CoreAssemblyRef, true, "System", "TypedReference");
                }
                return _systemTypedReference;
            }
        }

        private INamespaceTypeReference/*?*/ _systemTypedReference;

        /// <summary>
        /// System.UInt16
        /// </summary>
        public INamespaceTypeReference SystemUInt16
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemUInt16 == null)
                {
                    _systemUInt16 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.UInt16, "System", "UInt16");
                }
                return _systemUInt16;
            }
        }

        private INamespaceTypeReference/*?*/ _systemUInt16;

        /// <summary>
        /// System.UInt32
        /// </summary>
        public INamespaceTypeReference SystemUInt32
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemUInt32 == null)
                {
                    _systemUInt32 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.UInt32, "System", "UInt32");
                }
                return _systemUInt32;
            }
        }

        private INamespaceTypeReference/*?*/ _systemUInt32;

        /// <summary>
        /// System.UInt64
        /// </summary>
        public INamespaceTypeReference SystemUInt64
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemUInt64 == null)
                {
                    _systemUInt64 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.UInt64, "System", "UInt64");
                }
                return _systemUInt64;
            }
        }

        private INamespaceTypeReference/*?*/ _systemUInt64;

        /// <summary>
        /// System.UInt8
        /// </summary>
        public INamespaceTypeReference SystemUInt8
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemUInt8 == null)
                {
                    _systemUInt8 = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.UInt8, "System", "Byte");
                }
                return _systemUInt8;
            }
        }

        private INamespaceTypeReference/*?*/ _systemUInt8;

        /// <summary>
        /// System.UIntPtr
        /// </summary>
        public INamespaceTypeReference SystemUIntPtr
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemUIntPtr == null)
                {
                    _systemUIntPtr = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.UIntPtr, "System", "UIntPtr");
                }
                return _systemUIntPtr;
            }
        }

        private INamespaceTypeReference/*?*/ _systemUIntPtr;

        /// <summary>
        /// System.ValueType
        /// </summary>
        public INamespaceTypeReference SystemValueType
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemValueType == null)
                {
                    _systemValueType = this.CreateReference(this.CoreAssemblyRef, "System", "ValueType");
                }
                return _systemValueType;
            }
        }

        private INamespaceTypeReference/*?*/ _systemValueType;

        /// <summary>
        /// System.Void
        /// </summary>
        public INamespaceTypeReference SystemVoid
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemVoid == null)
                {
                    _systemVoid = this.CreateReference(this.CoreAssemblyRef, PrimitiveTypeCode.Void, "System", "Void");
                }
                return _systemVoid;
            }
        }

        private INamespaceTypeReference/*?*/ _systemVoid;

        /// <summary>
        /// System.Void*
        /// </summary>
        public IPointerTypeReference SystemVoidPtr
        {
            [DebuggerNonUserCode]
            get
            {
                if (_systemVoidPtr == null)
                    _systemVoidPtr = PointerType.GetPointerType(this.SystemVoid, _host.InternFactory);
                return _systemVoidPtr;
            }
        }

        private IPointerTypeReference/*?*/ _systemVoidPtr;

        /// <summary>
        /// Maps a PrimitiveTypeCode value (other than Pointer, Reference and NotPrimitive) to a corresponding ITypeDefinition instance.
        /// </summary>
        //^ [Pure]
        public INamespaceTypeReference GetTypeFor(PrimitiveTypeCode typeCode)
        //^^ requires typeCode != PrimitiveTypeCode.Pointer && typeCode != PrimitiveTypeCode.Reference && typeCode != PrimitiveTypeCode.NotPrimitive;
        {
            switch (typeCode)
            {
                case PrimitiveTypeCode.Float32: return this.SystemFloat32;
                case PrimitiveTypeCode.Float64: return this.SystemFloat64;
                case PrimitiveTypeCode.Int16: return this.SystemInt16;
                case PrimitiveTypeCode.Int32: return this.SystemInt32;
                case PrimitiveTypeCode.Int64: return this.SystemInt64;
                case PrimitiveTypeCode.Int8: return this.SystemInt8;
                case PrimitiveTypeCode.IntPtr: return this.SystemIntPtr;
                case PrimitiveTypeCode.UInt16: return this.SystemUInt16;
                case PrimitiveTypeCode.UInt32: return this.SystemUInt32;
                case PrimitiveTypeCode.UInt64: return this.SystemUInt64;
                case PrimitiveTypeCode.UInt8: return this.SystemUInt8;
                case PrimitiveTypeCode.UIntPtr: return this.SystemUIntPtr;
                case PrimitiveTypeCode.Void: return this.SystemVoid;
                default:
                    //^ assume false; //TODO: make Boogie aware of distinction between bit maps and enums
                    return Dummy.NamespaceTypeReference;
            }
        }

        #endregion IPlatformType Members
    }

    /// <summary>
    /// A reference to a root unit namespace.
    /// </summary>
    public sealed class RootUnitNamespaceReference : IRootUnitNamespaceReference
    {
        /// <summary>
        /// Allocates a reference to a root unit namespace.
        /// </summary>
        /// <param name="unit">A reference to the unit that defines the referenced namespace.</param>
        public RootUnitNamespaceReference(IUnitReference unit)
        {
            _unit = unit;
        }

        /// <summary>
        /// Calls visitor.Visit(IRootUnitNamespaceReference).
        /// </summary>
        public void Dispatch(IMetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        /// <summary>
        /// The namespace definition being referred to, if it can be resolved. Otherwise Dummy.UnitNamespace;
        /// </summary>
        public IUnitNamespace ResolvedUnitNamespace
        {
            get { return this.Unit.ResolvedUnit.UnitNamespaceRoot; }
        }

        /// <summary>
        /// A reference to the unit that defines the referenced namespace.
        /// </summary>
        public IUnitReference Unit
        {
            get { return _unit; }
        }

        private readonly IUnitReference _unit;

        #region IReference Members

        IEnumerable<ICustomAttribute> IReference.Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        IEnumerable<ILocation> IObjectWithLocations.Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members
    }
}