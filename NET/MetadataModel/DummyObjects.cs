//-----------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All Rights Reserved.
//
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

//^ using Microsoft.Contracts;

//  TODO: Sometime make the methods and properties of dummy objects Explicit impls so
//  that we can track addition and removal of methods and properties.

namespace Microsoft.Cci
{
#pragma warning disable 1591

    public static class Dummy
    {
        public static IAliasForType AliasForType
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_aliasForType == null)
                    Dummy.s_aliasForType = new DummyAliasForType();
                return Dummy.s_aliasForType;
            }
        }

        private static IAliasForType/*?*/ s_aliasForType;

        public static IMetadataHost CompilationHostEnvironment
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_compilationHostEnvironment == null)
                    Dummy.s_compilationHostEnvironment = new DummyMetadataHost();
                return Dummy.s_compilationHostEnvironment;
            }
        }

        private static IMetadataHost/*?*/ s_compilationHostEnvironment;

        public static IMetadataConstant Constant
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_constant == null)
                    Dummy.s_constant = new DummyMetadataConstant();
                return Dummy.s_constant;
            }
        }

        private static IMetadataConstant/*?*/ s_constant;

        public static ICustomModifier CustomModifier
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_customModifier == null)
                    Dummy.s_customModifier = new DummyCustomModifier();
                return Dummy.s_customModifier;
            }
        }

        private static ICustomModifier/*?*/ s_customModifier;

        public static IEventDefinition Event
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_event == null)
                    Dummy.s_event = new DummyEventDefinition();
                return Dummy.s_event;
            }
        }

        private static IEventDefinition/*?*/ s_event;

        public static IFieldDefinition Field
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_field == null)
                    Dummy.s_field = new DummyFieldDefinition();
                return Dummy.s_field;
            }
        }

        private static IFieldDefinition/*?*/ s_field;

        public static IMetadataExpression Expression
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_expression == null)
                    Dummy.s_expression = new DummyMetadataExpression();
                return Dummy.s_expression;
            }
        }

        private static IMetadataExpression/*?*/ s_expression;

        public static IFunctionPointer FunctionPointer
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_functionPointer == null)
                    Dummy.s_functionPointer = new DummyFunctionPointerType();
                return Dummy.s_functionPointer;
            }
        }

        private static IFunctionPointer/*?*/ s_functionPointer;

        public static IGenericMethodParameter GenericMethodParameter
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_genericMethodParameter == null)
                    Dummy.s_genericMethodParameter = new DummyGenericMethodParameter();
                return Dummy.s_genericMethodParameter;
            }
        }

        private static DummyGenericMethodParameter/*?*/ s_genericMethodParameter;

        public static IGenericTypeInstance GenericTypeInstance
        {
            [DebuggerNonUserCode]
            get
            //^ ensures !result.IsGeneric;
            {
                if (Dummy.s_genericTypeInstance == null)
                    Dummy.s_genericTypeInstance = new DummyGenericTypeInstance();
                DummyGenericTypeInstance result = Dummy.s_genericTypeInstance;
                //^ assume !result.IsGeneric; //the post condition says so
                return result;
            }
        }

        private static DummyGenericTypeInstance/*?*/ s_genericTypeInstance;

        public static IGenericTypeParameter GenericTypeParameter
        {
            get
            {
                if (Dummy.s_genericTypeParameter == null)
                    Dummy.s_genericTypeParameter = new DummyGenericTypeParameter();
                return Dummy.s_genericTypeParameter;
            }
        }

        private static IGenericTypeParameter/*?*/ s_genericTypeParameter;

        public static IMethodDefinition Method
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_method == null)
                    Dummy.s_method = new DummyMethodDefinition();
                return Dummy.s_method;
            }
        }

        private static IMethodDefinition/*?*/ s_method;

        public static IMethodBody MethodBody
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_methodBody == null)
                    Dummy.s_methodBody = new DummyMethodBody();
                return Dummy.s_methodBody;
            }
        }

        private static IMethodBody/*?*/ s_methodBody;

        public static IName Name
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_name == null)
                    Dummy.s_name = new DummyName();
                return Dummy.s_name;
            }
        }

        private static IName/*?*/ s_name;

        public static IMetadataNamedArgument NamedArgument
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_namedArgument == null)
                    Dummy.s_namedArgument = new DummyNamedArgument();
                return Dummy.s_namedArgument;
            }
        }

        private static IMetadataNamedArgument/*?*/ s_namedArgument;

        public static INameTable NameTable
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_nameTable == null)
                    Dummy.s_nameTable = new DummyNameTable();
                return Dummy.s_nameTable;
            }
        }

        private static INameTable/*?*/ s_nameTable;

        public static INestedTypeDefinition NestedType
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_nestedType == null)
                    Dummy.s_nestedType = new DummyNestedType();
                return Dummy.s_nestedType;
            }
        }

        private static INestedTypeDefinition/*?*/ s_nestedType;

        public static IPlatformType PlatformType
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_platformType == null)
                    Dummy.s_platformType = new DummyPlatformType();
                return Dummy.s_platformType;
            }
        }

        private static IPlatformType/*?*/ s_platformType;

        public static IPropertyDefinition Property
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_property == null)
                    Dummy.s_property = new DummyPropertyDefinition();
                return Dummy.s_property;
            }
        }

        private static IPropertyDefinition/*?*/ s_property;

        public static ITypeDefinition Type
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_type == null)
                    Dummy.s_type = new DummyType();
                return Dummy.s_type;
            }
        }

        private static ITypeDefinition/*?*/ s_type;

        public static ITypeReference TypeReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_typeReference == null)
                    Dummy.s_typeReference = new DummyTypeReference();
                return Dummy.s_typeReference;
            }
        }

        private static ITypeReference/*?*/ s_typeReference;

        public static IUnit Unit
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_unit == null)
                    Dummy.s_unit = new DummyUnit();
                return Dummy.s_unit;
            }
        }

        private static IUnit/*?*/ s_unit;

        public static IRootUnitNamespace RootUnitNamespace
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_rootUnitNamespace == null)
                    Dummy.s_rootUnitNamespace = new DummyRootUnitNamespace();
                return Dummy.s_rootUnitNamespace;
            }
        }

        private static IRootUnitNamespace/*?*/ s_rootUnitNamespace;

        public static INestedUnitNamespace NestedUnitNamespace
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_nestedUnitNamespace == null)
                    Dummy.s_nestedUnitNamespace = new DummyNestedUnitNamespace();
                return Dummy.s_nestedUnitNamespace;
            }
        }

        private static INestedUnitNamespace/*?*/ s_nestedUnitNamespace;

        public static IUnitSet UnitSet
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_unitSet == null)
                    Dummy.s_unitSet = new DummyUnitSet();
                return Dummy.s_unitSet;
            }
        }

        private static IUnitSet/*?*/ s_unitSet;

        public static IRootUnitSetNamespace RootUnitSetNamespace
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_rootUnitSetNamespace == null)
                    Dummy.s_rootUnitSetNamespace = new DummyRootUnitSetNamespace();
                return Dummy.s_rootUnitSetNamespace;
            }
        }

        private static IRootUnitSetNamespace/*?*/ s_rootUnitSetNamespace;

        public static IModule Module
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_module == null)
                    Dummy.s_module = new DummyModule();
                return Dummy.s_module;
            }
        }

        private static IModule/*?*/ s_module;

        //  Issue: This is kind of bad thing to do. What happens to IModule m = loadAssembly(...)   m != Dummy.Module?!?
        public static IAssembly Assembly
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_assembly == null)
                    Dummy.s_assembly = new DummyAssembly();
                return Dummy.s_assembly;
            }
        }

        private static IAssembly/*?*/ s_assembly;

        public static IMethodReference MethodReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_methodReference == null)
                    Dummy.s_methodReference = new DummyMethodReference();
                return Dummy.s_methodReference;
            }
        }

        private static IMethodReference/*?*/ s_methodReference;

        public static Version Version
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_version == null)
                    Dummy.s_version = new Version(0, 0);
                return Dummy.s_version;
            }
        }

        private static Version/*?*/ s_version;

        public static ICustomAttribute CustomAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_customAttribute == null)
                    Dummy.s_customAttribute = new DummyCustomAttribute();
                return Dummy.s_customAttribute;
            }
        }

        private static ICustomAttribute/*?*/ s_customAttribute;

        public static IFileReference FileReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_fileReference == null)
                    Dummy.s_fileReference = new DummyFileReference();
                return Dummy.s_fileReference;
            }
        }

        private static IFileReference/*?*/ s_fileReference;

        public static IResource Resource
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_resource == null)
                    Dummy.s_resource = new DummyResource();
                return Dummy.s_resource;
            }
        }

        private static IResource/*?*/ s_resource;

        public static IModuleReference ModuleReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_moduleReference == null)
                    Dummy.s_moduleReference = new DummyModuleReference();
                return Dummy.s_moduleReference;
            }
        }

        private static IModuleReference/*?*/ s_moduleReference;

        public static IAssemblyReference AssemblyReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_assemblyReference == null)
                    Dummy.s_assemblyReference = new DummyAssemblyReference();
                return Dummy.s_assemblyReference;
            }
        }

        private static IAssemblyReference/*?*/ s_assemblyReference;

        public static IMarshallingInformation MarshallingInformation
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_marshallingInformation == null)
                    Dummy.s_marshallingInformation = new DummyMarshallingInformation();
                return Dummy.s_marshallingInformation;
            }
        }

        private static IMarshallingInformation/*?*/ s_marshallingInformation;

        public static ISecurityAttribute SecurityAttribute
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_securityAttribute == null)
                    Dummy.s_securityAttribute = new DummySecurityAttribute();
                return Dummy.s_securityAttribute;
            }
        }

        private static ISecurityAttribute/*?*/ s_securityAttribute;

        public static IParameterTypeInformation ParameterTypeInformation
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_parameterTypeInformation == null)
                    Dummy.s_parameterTypeInformation = new DummyParameterTypeInformation();
                return Dummy.s_parameterTypeInformation;
            }
        }

        private static IParameterTypeInformation/*?*/ s_parameterTypeInformation;

        public static INamespaceTypeDefinition NamespaceTypeDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_namespaceTypeDefinition == null)
                    Dummy.s_namespaceTypeDefinition = new DummyNamespaceTypeDefinition();
                return Dummy.s_namespaceTypeDefinition;
            }
        }

        private static INamespaceTypeDefinition/*?*/ s_namespaceTypeDefinition;

        public static INamespaceTypeReference NamespaceTypeReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_namespaceTypeReference == null)
                    Dummy.s_namespaceTypeReference = new DummyNamespaceTypeReference();
                return Dummy.s_namespaceTypeReference;
            }
        }

        private static INamespaceTypeReference/*?*/ s_namespaceTypeReference;

        public static ISpecializedNestedTypeDefinition SpecializedNestedTypeDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_specializedNestedTypeDefinition == null)
                    Dummy.s_specializedNestedTypeDefinition = new DummySpecializedNestedTypeDefinition();
                return Dummy.s_specializedNestedTypeDefinition;
            }
        }

        private static ISpecializedNestedTypeDefinition/*?*/ s_specializedNestedTypeDefinition;

        public static ISpecializedFieldDefinition SpecializedFieldDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_specializedFieldDefinition == null)
                    Dummy.s_specializedFieldDefinition = new DummySpecializedFieldDefinition();
                return Dummy.s_specializedFieldDefinition;
            }
        }

        private static ISpecializedFieldDefinition/*?*/ s_specializedFieldDefinition;

        public static ISpecializedMethodDefinition SpecializedMethodDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_specializedMethodDefinition == null)
                    Dummy.s_specializedMethodDefinition = new DummySpecializedMethodDefinition();
                return Dummy.s_specializedMethodDefinition;
            }
        }

        private static ISpecializedMethodDefinition/*?*/ s_specializedMethodDefinition;

        public static ISpecializedPropertyDefinition SpecializedPropertyDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_specializedPropertyDefinition == null)
                    Dummy.s_specializedPropertyDefinition = new DummySpecializedPropertyDefinition();
                return Dummy.s_specializedPropertyDefinition;
            }
        }

        private static ISpecializedPropertyDefinition/*?*/ s_specializedPropertyDefinition;

        public static ILocalDefinition LocalVariable
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_localVariable == null)
                    Dummy.s_localVariable = new DummyLocalVariable();
                return Dummy.s_localVariable;
            }
        }

        private static ILocalDefinition/*?*/ s_localVariable;

        public static IFieldReference FieldReference
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_fieldReference == null)
                    Dummy.s_fieldReference = new DummyFieldReference();
                return Dummy.s_fieldReference;
            }
        }

        private static IFieldReference/*?*/ s_fieldReference;

        public static IParameterDefinition ParameterDefinition
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_parameterDefinition == null)
                    Dummy.s_parameterDefinition = new DummyParameterDefinition();
                return Dummy.s_parameterDefinition;
            }
        }

        private static IParameterDefinition/*?*/ s_parameterDefinition;

        public static ISectionBlock SectionBlock
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_sectionBlock == null)
                    Dummy.s_sectionBlock = new DummySectionBlock();
                return Dummy.s_sectionBlock;
            }
        }

        private static ISectionBlock/*?*/ s_sectionBlock;

        public static IPlatformInvokeInformation PlatformInvokeInformation
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_platformInvokeInformation == null)
                    Dummy.s_platformInvokeInformation = new DummyPlatformInvokeInformation();
                return Dummy.s_platformInvokeInformation;
            }
        }

        private static IPlatformInvokeInformation/*?*/ s_platformInvokeInformation;

        public static IGlobalMethodDefinition GlobalMethod
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_globalMethodDefinition == null)
                    Dummy.s_globalMethodDefinition = new DummyGlobalMethodDefinition();
                return Dummy.s_globalMethodDefinition;
            }
        }

        private static IGlobalMethodDefinition/*?*/ s_globalMethodDefinition;

        public static IGlobalFieldDefinition GlobalField
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_globalFieldDefinition == null)
                    Dummy.s_globalFieldDefinition = new DummyGlobalFieldDefinition();
                return Dummy.s_globalFieldDefinition;
            }
        }

        private static IGlobalFieldDefinition/*?*/ s_globalFieldDefinition;

        public static IOperation Operation
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_operation == null)
                    Dummy.s_operation = new DummyOperation();
                return Dummy.s_operation;
            }
        }

        private static IOperation/*?*/ s_operation;

        public static ILocation Location
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_location == null)
                    Dummy.s_location = new DummyLocation();
                return Dummy.s_location;
            }
        }

        private static ILocation/*?*/ s_location;

        public static IDocument Document
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_document == null)
                    Dummy.s_document = new DummyDocument();
                return Dummy.s_document;
            }
        }

        private static IDocument/*?*/ s_document;

        public static IOperationExceptionInformation OperationExceptionInformation
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_operationExceptionInformation == null)
                    Dummy.s_operationExceptionInformation = new DummyOperationExceptionInformation();
                return Dummy.s_operationExceptionInformation;
            }
        }

        private static IOperationExceptionInformation/*?*/ s_operationExceptionInformation;

        public static IInternFactory InternFactory
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_internFactory == null)
                    Dummy.s_internFactory = new DummyInternFactory();
                return Dummy.s_internFactory;
            }
        }

        private static IInternFactory/*?*/ s_internFactory;

        public static IArrayType ArrayType
        {
            [DebuggerNonUserCode]
            get
            {
                if (Dummy.s_arrayType == null)
                    Dummy.s_arrayType = new DummyArrayType();
                return Dummy.s_arrayType;
            }
        }

        private static IArrayType/*?*/ s_arrayType;
    }

    internal sealed class DummyAliasForType : IAliasForType
    {
        #region IAliasForType Members

        public ITypeReference AliasedType
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IAliasForType Members

        #region IContainer<IAliasMember> Members

        public IEnumerable<IAliasMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<IAliasMember>(); }
        }

        #endregion IContainer<IAliasMember> Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IDefinition Members

        #region IScope<IAliasMember> Members

        public bool Contains(IAliasMember member)
        {
            return false;
        }

        public IEnumerable<IAliasMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<IAliasMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<IAliasMember>();
        }

        public IEnumerable<IAliasMember> GetMatchingMembers(Function<IAliasMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<IAliasMember>();
        }

        public IEnumerable<IAliasMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<IAliasMember>();
        }

        #endregion IScope<IAliasMember> Members
    }

    internal sealed class DummyAssembly : IAssembly
    {
        #region IAssembly Members

        public IEnumerable<ICustomAttribute> AssemblyAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public string Culture
        {
            get { return string.Empty; }
        }

        public IEnumerable<IAliasForType> ExportedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<IAliasForType>(); }
        }

        public IEnumerable<IResourceReference> Resources
        {
            get { return IteratorHelper.GetEmptyEnumerable<IResourceReference>(); }
        }

        public IEnumerable<IFileReference> Files
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFileReference>(); }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public IEnumerable<IModule> MemberModules
        {
            get { return IteratorHelper.GetEmptyEnumerable<IModule>(); }
        }

        public uint Flags
        {
            get { return 0; }
        }

        public IEnumerable<byte> PublicKey
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        public Version Version
        {
            get { return Dummy.Version; }
        }

        public AssemblyIdentity AssemblyIdentity
        {
            get
            {
                return new AssemblyIdentity(Dummy.Name, string.Empty, new Version(0, 0), new byte[0], string.Empty);
            }
        }

        #endregion IAssembly Members

        #region IModule Members

        public IName ModuleName
        {
            get
            {
                return Dummy.Name;
            }
        }

        public IAssembly/*?*/ ContainingAssembly
        {
            get
            {
                return this;
            }
        }

        public IEnumerable<IAssemblyReference> AssemblyReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IAssemblyReference>(); }
        }

        public ulong BaseAddress
        {
            get { return 0; }
        }

        public ushort DllCharacteristics
        {
            get { return 0; }
        }

        public IMethodReference EntryPoint
        {
            get { return Dummy.MethodReference; }
        }

        public uint FileAlignment
        {
            get { return 0; }
        }

        public bool ILOnly
        {
            get { return false; }
        }

        public ModuleKind Kind
        {
            get { return ModuleKind.ConsoleApplication; }
        }

        public byte LinkerMajorVersion
        {
            get { return 0; }
        }

        public byte LinkerMinorVersion
        {
            get { return 0; }
        }

        public byte MetadataFormatMajorVersion
        {
            get { return 0; }
        }

        public byte MetadataFormatMinorVersion
        {
            get { return 0; }
        }

        public IEnumerable<ICustomAttribute> ModuleAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<IModuleReference> ModuleReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IModuleReference>(); }
        }

        public Guid PersistentIdentifier
        {
            get { return Guid.Empty; }
        }

        public bool RequiresAmdInstructionSet
        {
            get { return false; }
        }

        public bool Requires32bits
        {
            get { return false; }
        }

        public bool Requires64bits
        {
            get { return false; }
        }

        public ulong SizeOfHeapReserve
        {
            get { return 0; }
        }

        public ulong SizeOfHeapCommit
        {
            get { return 0; }
        }

        public ulong SizeOfStackReserve
        {
            get { return 0; }
        }

        public ulong SizeOfStackCommit
        {
            get { return 0; }
        }

        public string TargetRuntimeVersion
        {
            get { return string.Empty; }
        }

        public bool TrackDebugData
        {
            get { return false; }
        }

        public bool UsePublicKeyTokensForAssemblyReferences
        {
            get { return false; }
        }

        public IEnumerable<IWin32Resource> Win32Resources
        {
            get { return IteratorHelper.GetEmptyEnumerable<IWin32Resource>(); }
        }

        public IEnumerable<string> GetStrings()
        {
            return IteratorHelper.GetEmptyEnumerable<string>();
        }

        public IEnumerable<INamedTypeDefinition> GetAllTypes()
        {
            return IteratorHelper.GetEmptyEnumerable<INamedTypeDefinition>();
        }

        public ModuleIdentity ModuleIdentity
        {
            get
            {
                return this.AssemblyIdentity;
            }
        }

        #endregion IModule Members

        #region IUnit Members

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public string Location
        {
            get { return string.Empty; }
        }

        public IName Name
        {
            get { return Dummy.Name; }
        }

        public IRootUnitNamespace UnitNamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        public IEnumerable<IUnitReference> UnitReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IUnitReference>(); }
        }

        public UnitIdentity UnitIdentity
        {
            get
            {
                return this.AssemblyIdentity;
            }
        }

        #endregion IUnit Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region INamespaceRootOwner Members

        public INamespaceDefinition NamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        #endregion INamespaceRootOwner Members

        #region IUnitReference Members

        public IUnit ResolvedUnit
        {
            get { return this; }
        }

        #endregion IUnitReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region IModuleReference Members

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get { return null; }
        }

        public IModule ResolvedModule
        {
            get { return this; }
        }

        #endregion IModuleReference Members

        #region IAssemblyReference Members

        public IEnumerable<IName> Aliases
        {
            get { return IteratorHelper.GetEmptyEnumerable<IName>(); }
        }

        public IAssembly ResolvedAssembly
        {
            get { return this; }
        }

        public IEnumerable<byte> PublicKeyToken
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        public AssemblyIdentity UnifiedAssemblyIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        #endregion IAssemblyReference Members
    }

    internal sealed class DummyMetadataHost : IMetadataHost
    {
        #region ICompilationHostEnvironment Members

        public event EventHandler<ErrorEventArgs> Errors;

        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public IAssembly FindAssembly(AssemblyIdentity assemblyIdentity)
        {
            return Dummy.Assembly;
        }

        public IModule FindModule(ModuleIdentity moduleIdentity)
        {
            return Dummy.Module;
        }

        public IUnit FindUnit(UnitIdentity unitIdentity)
        {
            return Dummy.Unit;
        }

        public IAssembly LoadAssembly(AssemblyIdentity assemblyIdentity)
        {
            return Dummy.Assembly;
        }

        public IModule LoadModule(ModuleIdentity moduleIdentity)
        {
            return Dummy.Module;
        }

        public IUnit LoadUnit(UnitIdentity unitIdentity)
        {
            return Dummy.Unit;
        }

        public IUnit LoadUnitFrom(string location)
        {
            return Dummy.Unit;
        }

        public INameTable NameTable
        {
            get { return Dummy.NameTable; }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public byte PointerSize
        {
            get { return 4; }
        }

        public void ReportErrors(ErrorEventArgs errorEventArguments)
        {
            if (this.Errors != null)
                this.Errors(this, errorEventArguments); //Do this only to shut up warning about not using this.Errors
        }

        public void ReportError(IErrorMessage error)
        {
        }

        //^ [Pure]
        public AssemblyIdentity ProbeAssemblyReference(IUnit unit, AssemblyIdentity referedAssemblyIdentity)
        {
            return referedAssemblyIdentity;
        }

        //^ [Pure]
        public ModuleIdentity ProbeModuleReference(IUnit unit, ModuleIdentity referedModuleIdentity)
        {
            return referedModuleIdentity;
        }

        //^ [Pure]
        public AssemblyIdentity UnifyAssembly(AssemblyIdentity assemblyIdentity)
        {
            return assemblyIdentity;
        }

        public IEnumerable<IUnit> LoadedUnits
        {
            get { return IteratorHelper.GetEmptyEnumerable<IUnit>(); }
        }

        public IInternFactory InternFactory
        {
            get { return Dummy.InternFactory; }
        }

        #endregion ICompilationHostEnvironment Members
    }

    internal sealed class DummyMetadataConstant : IMetadataConstant
    {
        #region IMetadataConstant Members

        public object/*?*/ Value
        {
            get { return null; }
        }

        #endregion IMetadataConstant Members

        #region IMetadataExpression Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IMetadataExpression Members
    }

    internal sealed class DummyCustomAttribute : ICustomAttribute
    {
        #region ICustomAttribute Members

        public IEnumerable<IMetadataExpression> Arguments
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMetadataExpression>(); }
        }

        public IMethodReference Constructor
        {
            get { return Dummy.MethodReference; }
        }

        public IEnumerable<IMetadataNamedArgument> NamedArguments
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMetadataNamedArgument>(); }
        }

        public ushort NumberOfNamedArguments
        {
            get { return 0; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ICustomAttribute Members
    }

    internal sealed class DummyCustomModifier : ICustomModifier
    {
        #region ICustomModifier Members

        public bool IsOptional
        {
            get { return false; }
        }

        public ITypeReference Modifier
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ICustomModifier Members
    }

    internal sealed class DummyEventDefinition : IEventDefinition
    {
        #region IEventDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodReference>(); }
        }

        public IMethodReference Adder
        {
            get { return Dummy.MethodReference; }
        }

        public IMethodReference/*?*/ Caller
        {
            get { return null; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public IMethodReference Remover
        {
            get { return Dummy.MethodReference; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IEventDefinition Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return Dummy.Event; }
        }

        #endregion ITypeMemberReference Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members
    }

    internal sealed class DummyMetadataExpression : IMetadataExpression
    {
        #region IMetadataExpression Members

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IMetadataExpression Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members
    }

    internal sealed class DummyFieldDefinition : IFieldDefinition
    {
        #region IFieldDefinition Members

        public uint BitLength
        {
            get { return 0; }
        }

        public bool IsBitField
        {
            get { return false; }
        }

        public bool IsCompileTimeConstant
        {
            get { return false; }
        }

        public bool IsMapped
        {
            get { return false; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return false; }
        }

        public bool IsNotSerialized
        {
            get { return true; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public ISectionBlock FieldMapping
        {
            get { return Dummy.SectionBlock; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public uint Offset
        {
            get { return 0; }
        }

        public int SequenceNumber
        {
            get { return 0; }
        }

        public IMetadataConstant CompileTimeValue
        {
            get { return Dummy.Constant; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get
            {
                //^ assume false;
                IMarshallingInformation/*?*/ dummyValue = null;
                //^ assume dummyValue != null;
                return dummyValue;
            }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IFieldDefinition Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region IFieldReference Members

        public IFieldDefinition ResolvedField
        {
            get { return this; }
        }

        #endregion IFieldReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummyFileReference : IFileReference
    {
        #region IFileReference Members

        public IAssembly ContainingAssembly
        {
            get { return Dummy.Assembly; }
        }

        public bool HasMetadata
        {
            get { return false; }
        }

        public IName FileName
        {
            get { return Dummy.Name; }
        }

        public IEnumerable<byte> HashValue
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        #endregion IFileReference Members
    }

    internal sealed class DummyGenericTypeInstance : IGenericTypeInstance
    {
        #region IGenericTypeInstance Members

        public IEnumerable<ITypeReference> GenericArguments
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public ITypeReference GenericType
        {
            get
            {
                //^ assume false;
                return Dummy.TypeReference;
            }
        }

        #endregion IGenericTypeInstance Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get
            //^ ensures result == false;
            {
                return false;
            }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region ITypeReference Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members
    }

    internal sealed class DummyGenericTypeParameter : IGenericTypeParameter
    {
        #region IGenericTypeParameter Members

        public ITypeDefinition DefiningType
        {
            get { return Dummy.Type; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IGenericTypeParameter Members

        #region IGenericParameter Members

        public IEnumerable<ITypeReference> Constraints
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool MustBeReferenceType
        {
            get { return false; }
        }

        public bool MustBeValueType
        {
            get { return false; }
        }

        public bool MustHaveDefaultConstructor
        {
            get { return false; }
        }

        public TypeParameterVariance Variance
        {
            get { return TypeParameterVariance.NonVariant; }
        }

        #endregion IGenericParameter Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region IParameterListEntry Members

        public ushort Index
        {
            get { return 0; }
        }

        #endregion IParameterListEntry Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region IGenericTypeParameterReference Members

        ITypeReference IGenericTypeParameterReference.DefiningType
        {
            get { return Dummy.TypeReference; }
        }

        IGenericTypeParameter IGenericTypeParameterReference.ResolvedType
        {
            get { return this; }
        }

        #endregion IGenericTypeParameterReference Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return this; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummyGenericMethodParameter : IGenericMethodParameter
    {
        #region IGenericMethodParameter Members

        public IMethodDefinition DefiningMethod
        {
            get
            {
                //^ assume false; //TODO; need a dummy generic method
                return Dummy.Method;
            }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IGenericMethodParameter Members

        #region IGenericParameter Members

        public IEnumerable<ITypeReference> Constraints
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool MustBeReferenceType
        {
            get { return false; }
        }

        public bool MustBeValueType
        {
            get { return false; }
        }

        public bool MustHaveDefaultConstructor
        {
            get { return false; }
        }

        public TypeParameterVariance Variance
        {
            get { return TypeParameterVariance.NonVariant; }
        }

        #endregion IGenericParameter Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region IParameterListEntry Members

        public ushort Index
        {
            get { return 0; }
        }

        #endregion IParameterListEntry Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region IGenericMethodParameterReference Members

        IMethodReference IGenericMethodParameterReference.DefiningMethod
        {
            get { return Dummy.MethodReference; }
        }

        IGenericMethodParameter IGenericMethodParameterReference.ResolvedType
        {
            get { return this; }
        }

        #endregion IGenericMethodParameterReference Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return this; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummyMethodBody : IMethodBody
    {
        #region IMethodBody Members

        public IMethodDefinition MethodDefinition
        {
            get { return Dummy.Method; }
        }

        //public IBlockStatement Block {
        //  get { return Dummy.Block; }
        //}

        //public IOperation GetOperationAt(int offset, out int offsetOfNextOperation) {
        //  offsetOfNextOperation = -1;
        //  return Dummy.Operation;
        //}

        public IEnumerable<ILocalDefinition> LocalVariables
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocalDefinition>(); }
        }

        public bool LocalsAreZeroed
        {
            get { return false; }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return IteratorHelper.GetEmptyEnumerable<IOperation>(); }
        }

        public IEnumerable<ITypeDefinition> PrivateHelperTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinition>(); }
        }

        public ushort MaxStack
        {
            get { return 0; }
        }

        public IEnumerable<IOperationExceptionInformation> OperationExceptionInformation
        {
            get { return IteratorHelper.GetEmptyEnumerable<IOperationExceptionInformation>(); }
        }

        #endregion IMethodBody Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members
    }

    internal sealed class DummyMethodDefinition : IMethodDefinition
    {
        #region IMethodDefinition Members

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
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public bool HasExplicitThisParameter
        {
            get { return false; }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return false; }
        }

        public bool IsCil
        {
            get { return false; }
        }

        public bool IsConstructor
        {
            get { return false; }
        }

        public bool IsStaticConstructor
        {
            get { return false; }
        }

        public bool IsExternal
        {
            get { return false; }
        }

        public bool IsForwardReference
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsHiddenBySignature
        {
            get { return false; }
        }

        public bool IsNativeCode
        {
            get { return false; }
        }

        public bool IsNewSlot
        {
            get { return false; }
        }

        public bool IsNeverInlined
        {
            get { return false; }
        }

        public bool IsNeverOptimized
        {
            get { return false; }
        }

        public bool IsPlatformInvoke
        {
            get { return false; }
        }

        public bool IsRuntimeImplemented
        {
            get { return false; }
        }

        public bool IsRuntimeInternal
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsVirtual
        {
            get { return false; }
        }

        public bool IsUnmanaged
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.Default; }
        }

        public bool PreserveSignature
        {
            get { return false; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return Dummy.PlatformInvokeInformation; }
        }

        public bool RequiresSecurityObject
        {
            get { return false; }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get { return false; }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get { return Dummy.MarshallingInformation; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        #endregion IMethodDefinition Members

        #region ISignature Members

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterDefinition>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ISignature Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Default; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion ISignature Members

        #region IMethodReference Members

        public bool AcceptsExtraArguments
        {
            get { return false; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        public ushort ParameterCount
        {
            get { return 0; }
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
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members
    }

    internal sealed class DummyMethodReference : IMethodReference
    {
        #region IMethodReference Members

        public bool AcceptsExtraArguments
        {
            get { return false; }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public ushort ParameterCount
        {
            get { return 0; }
        }

        public IMethodDefinition ResolvedMethod
        {
            get
            {
                //^ assume false;
                return Dummy.Method;
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
            get { return CallingConvention.C; }
        }

        public IEnumerable<IParameterTypeInformation> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ISignature Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return Dummy.Method; }
        }

        #endregion ITypeMemberReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IReference Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class DummyModule : IModule
    {
        #region IModule Members

        public IName ModuleName
        {
            get
            {
                return Dummy.Name;
            }
        }

        public IAssembly/*?*/ ContainingAssembly
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<IAssemblyReference> AssemblyReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IAssemblyReference>(); }
        }

        public ulong BaseAddress
        {
            get { return 0; }
        }

        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public ushort DllCharacteristics
        {
            get { return 0; }
        }

        public IMethodReference EntryPoint
        {
            get { return Dummy.MethodReference; }
        }

        public uint FileAlignment
        {
            get { return 0; }
        }

        public bool ILOnly
        {
            get { return false; }
        }

        public ModuleKind Kind
        {
            get { return ModuleKind.ConsoleApplication; }
        }

        public byte LinkerMajorVersion
        {
            get { return 0; }
        }

        public byte LinkerMinorVersion
        {
            get { return 0; }
        }

        public byte MetadataFormatMajorVersion
        {
            get { return 0; }
        }

        public byte MetadataFormatMinorVersion
        {
            get { return 0; }
        }

        public IEnumerable<ICustomAttribute> ModuleAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<IModuleReference> ModuleReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IModuleReference>(); }
        }

        public Guid PersistentIdentifier
        {
            get { return Guid.Empty; }
        }

        public bool RequiresAmdInstructionSet
        {
            get { return false; }
        }

        public bool Requires32bits
        {
            get { return false; }
        }

        public bool Requires64bits
        {
            get { return false; }
        }

        public ulong SizeOfHeapReserve
        {
            get { return 0; }
        }

        public ulong SizeOfHeapCommit
        {
            get { return 0; }
        }

        public ulong SizeOfStackReserve
        {
            get { return 0; }
        }

        public ulong SizeOfStackCommit
        {
            get { return 0; }
        }

        public string TargetRuntimeVersion
        {
            get { return string.Empty; }
        }

        public bool TrackDebugData
        {
            get { return false; }
        }

        public bool UsePublicKeyTokensForAssemblyReferences
        {
            get { return false; }
        }

        public IEnumerable<IWin32Resource> Win32Resources
        {
            get { return IteratorHelper.GetEmptyEnumerable<IWin32Resource>(); }
        }

        public IEnumerable<string> GetStrings()
        {
            return IteratorHelper.GetEmptyEnumerable<string>();
        }

        public IEnumerable<INamedTypeDefinition> GetAllTypes()
        {
            return IteratorHelper.GetEmptyEnumerable<INamedTypeDefinition>();
        }

        public ModuleIdentity ModuleIdentity
        {
            get
            {
                return new ModuleIdentity(Dummy.Name, this.Location);
            }
        }

        #endregion IModule Members

        #region IUnit Members

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public string Location
        {
            get { return string.Empty; }
        }

        public IName Name
        {
            get { return Dummy.Name; }
        }

        public IRootUnitNamespace UnitNamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        public IEnumerable<IUnitReference> UnitReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IUnitReference>(); }
        }

        public UnitIdentity UnitIdentity
        {
            get
            {
                return this.ModuleIdentity;
            }
        }

        #endregion IUnit Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region INamespaceRootOwner Members

        public INamespaceDefinition NamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        #endregion INamespaceRootOwner Members

        #region IUnitReference Members

        public IUnit ResolvedUnit
        {
            get { return this; }
        }

        #endregion IUnitReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region IModuleReference Members

        IAssemblyReference/*?*/ IModuleReference.ContainingAssembly
        {
            get { return null; }
        }

        public IModule ResolvedModule
        {
            get { return this; }
        }

        #endregion IModuleReference Members
    }

    internal sealed class DummyModuleReference : IModuleReference
    {
        #region IUnitReference Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion IUnitReference Members

        #region IModuleReference Members

        public IAssemblyReference/*?*/ ContainingAssembly
        {
            get { return null; }
        }

        public IModule ResolvedModule
        {
            get { return Dummy.Module; }
        }

        #endregion IModuleReference Members

        #region IUnitReference Members

        public IUnit ResolvedUnit
        {
            get { return Dummy.Unit; }
        }

        #endregion IUnitReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region IModuleReference Members

        public ModuleIdentity ModuleIdentity
        {
            get { return Dummy.Module.ModuleIdentity; }
        }

        #endregion IModuleReference Members

        #region IUnitReference Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public UnitIdentity UnitIdentity
        {
            get { return this.ModuleIdentity; }
        }

        #endregion IUnitReference Members
    }

    internal sealed class DummyName : IName
    {
        #region IName Members

        public int UniqueKey
        {
            get { return 1; }
        }

        public int UniqueKeyIgnoringCase
        {
            get { return 1; }
        }

        public string Value
        {
            get { return string.Empty; }
        }

        #endregion IName Members
    }

    internal sealed class DummyNamedArgument : IMetadataNamedArgument
    {
        #region IMetadataNamedArgument Members

        public IName ArgumentName
        {
            get { return Dummy.Name; }
        }

        public IMetadataExpression ArgumentValue
        {
            get { return Dummy.Expression; }
        }

        public bool IsField
        {
            get { return false; }
        }

        public object ResolvedDefinition
        {
            get { return Dummy.Property; }
        }

        #endregion IMetadataNamedArgument Members

        #region IMetadataExpression Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.Type; }
        }

        #endregion IMetadataExpression Members
    }

    internal sealed class DummyNamespaceTypeDefinition : INamespaceTypeDefinition
    {
        #region INamespaceTypeDefinition Members

        public IUnitNamespace ContainingUnitNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        public bool IsPublic
        {
            get { return false; }
        }

        #endregion INamespaceTypeDefinition Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        public INamespaceDefinition Container
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        public IScope<INamespaceMember> ContainingScope
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region INamespaceTypeReference Members

        IUnitNamespaceReference INamespaceTypeReference.ContainingUnitNamespace
        {
            get { return this.ContainingUnitNamespace; }
        }

        INamespaceTypeDefinition INamespaceTypeReference.ResolvedType
        {
            get { return this; }
        }

        #endregion INamespaceTypeReference Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return this; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummyNamespaceTypeReference : INamespaceTypeReference
    {
        #region INamespaceTypeReference Members

        public ushort GenericParameterCount
        {
            get { return 0; }
        }

        public IUnitNamespaceReference ContainingUnitNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        INamespaceTypeDefinition INamespaceTypeReference.ResolvedType
        {
            get { return Dummy.NamespaceTypeDefinition; }
        }

        #endregion INamespaceTypeReference Members

        #region ITypeReference Members

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get
            {
                //^ assume false;
                return Dummy.Type;
            }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        public bool IsAlias
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        #endregion ITypeReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return Dummy.NamespaceTypeDefinition; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummyNameTable : INameTable
    {
        #region INameTable Members

        public IName Address
        {
            get { return Dummy.Name; }
        }

        public IName AllowMultiple
        {
            get { return Dummy.Name; }
        }

        public IName BoolOpBool
        {
            get { return Dummy.Name; }
        }

        public IName DecimalOpDecimal
        {
            get { return Dummy.Name; }
        }

        public IName DelegateOpDelegate
        {
            get { return Dummy.Name; }
        }

        public IName Cctor
        {
            get { return Dummy.Name; }
        }

        public IName Ctor
        {
            get { return Dummy.Name; }
        }

        public IName EmptyName
        {
            get { return Dummy.Name; }
        }

        public IName EnumOpEnum
        {
            get { return Dummy.Name; }
        }

        public IName EnumOpNum
        {
            get { return Dummy.Name; }
        }

        public new IName Equals
        {
            get { return Dummy.Name; }
        }

        public IName Float32OpFloat32
        {
            get { return Dummy.Name; }
        }

        public IName Float64OpFloat64
        {
            get { return Dummy.Name; }
        }

        public IName Get
        {
            get { return Dummy.Name; }
        }

        //^ [Pure]
        public IName GetNameFor(string name)
        {
            //^ assume false;
            return Dummy.Name;
        }

        public IName global
        {
            get { return Dummy.Name; }
        }

        public IName HasValue
        {
            get { return Dummy.Name; }
        }

        public IName Inherited
        {
            get { return Dummy.Name; }
        }

        public IName Invoke
        {
            get { return Dummy.Name; }
        }

        public IName Int16OpInt16
        {
            get { return Dummy.Name; }
        }

        public IName Int32OpInt32
        {
            get { return Dummy.Name; }
        }

        public IName Int32OpUInt32
        {
            get { return Dummy.Name; }
        }

        public IName Int64OpInt32
        {
            get { return Dummy.Name; }
        }

        public IName Int64OpUInt32
        {
            get { return Dummy.Name; }
        }

        public IName Int64OpUInt64
        {
            get { return Dummy.Name; }
        }

        public IName Int64OpInt64
        {
            get { return Dummy.Name; }
        }

        public IName Int8OpInt8
        {
            get { return Dummy.Name; }
        }

        public IName NullCoalescing
        {
            get { return Dummy.Name; }
        }

        public IName NumOpEnum
        {
            get { return Dummy.Name; }
        }

        public IName ObjectOpObject
        {
            get { return Dummy.Name; }
        }

        public IName ObjectOpString
        {
            get { return Dummy.Name; }
        }

        public IName OpAddition
        {
            get { return Dummy.Name; }
        }

        public IName OpBoolean
        {
            get { return Dummy.Name; }
        }

        public IName OpChar
        {
            get { return Dummy.Name; }
        }

        public IName OpDecimal
        {
            get { return Dummy.Name; }
        }

        public IName OpEnum
        {
            get { return Dummy.Name; }
        }

        public IName OpEquality
        {
            get { return Dummy.Name; }
        }

        public IName OpInequality
        {
            get { return Dummy.Name; }
        }

        public IName OpInt8
        {
            get { return Dummy.Name; }
        }

        public IName OpInt16
        {
            get { return Dummy.Name; }
        }

        public IName OpInt32
        {
            get { return Dummy.Name; }
        }

        public IName OpInt64
        {
            get { return Dummy.Name; }
        }

        public IName OpBitwiseAnd
        {
            get { return Dummy.Name; }
        }

        public IName OpBitwiseOr
        {
            get { return Dummy.Name; }
        }

        public IName OpComma
        {
            get { return Dummy.Name; }
        }

        public IName OpConcatentation
        {
            get { return Dummy.Name; }
        }

        public IName OpDivision
        {
            get { return Dummy.Name; }
        }

        public IName OpExclusiveOr
        {
            get { return Dummy.Name; }
        }

        public IName OpExplicit
        {
            get { return Dummy.Name; }
        }

        public IName OpExponentiation
        {
            get { return Dummy.Name; }
        }

        public IName OpFalse
        {
            get { return Dummy.Name; }
        }

        public IName OpFloat32
        {
            get { return Dummy.Name; }
        }

        public IName OpFloat64
        {
            get { return Dummy.Name; }
        }

        public IName OpGreaterThan
        {
            get { return Dummy.Name; }
        }

        public IName OpGreaterThanOrEqual
        {
            get { return Dummy.Name; }
        }

        public IName OpImplicit
        {
            get { return Dummy.Name; }
        }

        public IName OpIntegerDivision
        {
            get { return Dummy.Name; }
        }

        public IName OpLeftShift
        {
            get { return Dummy.Name; }
        }

        public IName OpLessThan
        {
            get { return Dummy.Name; }
        }

        public IName OpLessThanOrEqual
        {
            get { return Dummy.Name; }
        }

        public IName OpLike
        {
            get { return Dummy.Name; }
        }

        public IName OpLogicalNot
        {
            get { return Dummy.Name; }
        }

        public IName OpLogicalOr
        {
            get { return Dummy.Name; }
        }

        public IName OpModulus
        {
            get { return Dummy.Name; }
        }

        public IName OpMultiply
        {
            get { return Dummy.Name; }
        }

        public IName OpOnesComplement
        {
            get { return Dummy.Name; }
        }

        public IName OpDecrement
        {
            get { return Dummy.Name; }
        }

        public IName OpIncrement
        {
            get { return Dummy.Name; }
        }

        public IName OpRightShift
        {
            get { return Dummy.Name; }
        }

        public IName OpSubtraction
        {
            get { return Dummy.Name; }
        }

        public IName OpTrue
        {
            get { return Dummy.Name; }
        }

        public IName OpUInt8
        {
            get { return Dummy.Name; }
        }

        public IName OpUInt16
        {
            get { return Dummy.Name; }
        }

        public IName OpUInt32
        {
            get { return Dummy.Name; }
        }

        public IName OpUInt64
        {
            get { return Dummy.Name; }
        }

        public IName OpUnaryNegation
        {
            get { return Dummy.Name; }
        }

        public IName OpUnaryPlus
        {
            get { return Dummy.Name; }
        }

        public IName StringOpObject
        {
            get { return Dummy.Name; }
        }

        public IName StringOpString
        {
            get { return Dummy.Name; }
        }

        public IName value
        {
            get { return Dummy.Name; }
        }

        public IName UIntPtrOpUIntPtr
        {
            get { return Dummy.Name; }
        }

        public IName UInt32OpInt32
        {
            get { return Dummy.Name; }
        }

        public IName UInt32OpUInt32
        {
            get { return Dummy.Name; }
        }

        public IName UInt64OpInt32
        {
            get { return Dummy.Name; }
        }

        public IName UInt64OpUInt32
        {
            get { return Dummy.Name; }
        }

        public IName UInt64OpUInt64
        {
            get { return Dummy.Name; }
        }

        public IName System
        {
            get { return Dummy.Name; }
        }

        public IName Void
        {
            get { return Dummy.Name; }
        }

        public IName VoidPtrOpVoidPtr
        {
            get { return Dummy.Name; }
        }

        public IName Boolean
        {
            get { return Dummy.Name; }
        }

        public IName Char
        {
            get { return Dummy.Name; }
        }

        public IName Byte
        {
            get { return Dummy.Name; }
        }

        public IName SByte
        {
            get { return Dummy.Name; }
        }

        public IName Int16
        {
            get { return Dummy.Name; }
        }

        public IName UInt16
        {
            get { return Dummy.Name; }
        }

        public IName Int32
        {
            get { return Dummy.Name; }
        }

        public IName UInt32
        {
            get { return Dummy.Name; }
        }

        public IName Int64
        {
            get { return Dummy.Name; }
        }

        public IName UInt64
        {
            get { return Dummy.Name; }
        }

        public IName String
        {
            get { return Dummy.Name; }
        }

        public IName IntPtr
        {
            get { return Dummy.Name; }
        }

        public IName UIntPtr
        {
            get { return Dummy.Name; }
        }

        public IName Object
        {
            get { return Dummy.Name; }
        }

        public IName Set
        {
            get { return Dummy.Name; }
        }

        public IName Single
        {
            get { return Dummy.Name; }
        }

        public IName Double
        {
            get { return Dummy.Name; }
        }

        public IName TypedReference
        {
            get { return Dummy.Name; }
        }

        public IName Enum
        {
            get { return Dummy.Name; }
        }

        public IName MulticastDelegate
        {
            get { return Dummy.Name; }
        }

        public IName ValueType
        {
            get { return Dummy.Name; }
        }

        public IName Type
        {
            get { return Dummy.Name; }
        }

        public IName Array
        {
            get { return Dummy.Name; }
        }

        public IName AttributeUsageAttribute
        {
            get { return Dummy.Name; }
        }

        public IName Attribute
        {
            get { return Dummy.Name; }
        }

        public IName Combine
        {
            get { return Dummy.Name; }
        }

        public IName Concat
        {
            get { return Dummy.Name; }
        }

        public IName DateTime
        {
            get { return Dummy.Name; }
        }

        public IName DebuggerHiddenAttribute
        {
            get { return Dummy.Name; }
        }

        public IName Decimal
        {
            get { return Dummy.Name; }
        }

        public IName Delegate
        {
            get { return Dummy.Name; }
        }

        public IName Diagnostics
        {
            get { return Dummy.Name; }
        }

        public IName DBNull
        {
            get { return Dummy.Name; }
        }

        public IName Length
        {
            get { return Dummy.Name; }
        }

        public IName LongLength
        {
            get { return Dummy.Name; }
        }

        public IName Nullable
        {
            get { return Dummy.Name; }
        }

        public IName Remove
        {
            get { return Dummy.Name; }
        }

        public IName Result
        {
            get { return Dummy.Name; }
        }

        #endregion INameTable Members
    }

    internal sealed class DummyNestedType : INestedTypeDefinition
    {
        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Public; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return this.ContainingTypeDefinition; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return this.ContainingTypeDefinition; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region ITypeMemberReference Members

        ITypeReference ITypeMemberReference.ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ITypeMemberReference Members

        #region INestedTypeReference Members

        INestedTypeDefinition INestedTypeReference.ResolvedType
        {
            get { return this; }
        }

        #endregion INestedTypeReference Members

        #region ITypeMemberReference Members

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return this; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummyPlatformType : IPlatformType
    {
        #region IPlatformType Members

        public INamespaceTypeReference SystemDiagnosticsContractsContract
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public byte PointerSize
        {
            get { return 4; }
        }

        public INamespaceTypeReference SystemArgIterator
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemArray
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemAttributeUsageAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemAsyncCallback
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemBoolean
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemChar
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsGenericDictionary
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsGenericICollection
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsGenericIEnumerable
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsGenericIEnumerator
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsGenericIList
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsICollection
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsIEnumerable
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsIEnumerator
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemCollectionsIList
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemIAsyncResult
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemICloneable
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemDateTime
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemDecimal
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemDelegate
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemDBNull
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemEnum
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemFloat32
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemFloat64
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemInt16
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemInt32
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemInt64
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemInt8
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemIntPtr
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemMulticastDelegate
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemNullable
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemObject
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeArgumentHandle
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeFieldHandle
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeMethodHandle
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeTypeHandle
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesCallConvCdecl
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesCompilerGeneratedAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesFriendAccessAllowedAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesIsConst
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesIsVolatile
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeCompilerServicesReferenceAssemblyAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemRuntimeInteropServicesDllImportAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemSecurityPermissionsSecurityAction
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemSecuritySecurityCriticalAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemSecuritySecuritySafeCriticalAttribute
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemString
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemType
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemTypedReference
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemUInt16
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemUInt32
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemUInt64
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemUInt8
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemUIntPtr
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemValueType
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference SystemVoid
        {
            get { return Dummy.NamespaceTypeReference; }
        }

        public INamespaceTypeReference GetTypeFor(PrimitiveTypeCode typeCode)
        {
            return Dummy.NamespaceTypeReference;
        }

        #endregion IPlatformType Members
    }

    internal sealed class DummyPropertyDefinition : IPropertyDefinition
    {
        #region IPropertyDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodReference>(); }
        }

        public IMetadataConstant DefaultValue
        {
            get { return Dummy.Constant; }
        }

        public IMethodReference/*?*/ Getter
        {
            get { return null; }
        }

        public bool HasDefaultValue
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public IMethodReference/*?*/ Setter
        {
            get { return null; }
        }

        #endregion IPropertyDefinition Members

        #region ISignature Members

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterDefinition>(); }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.C; }
        }

        #endregion ISignature Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return Dummy.Property; }
        }

        #endregion ITypeMemberReference Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get
            {
                return Dummy.Type;
            }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion ISignature Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummyType : ITypeDefinition
    {
        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return true; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        public ITypeDefinition ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members
    }

    internal sealed class DummyTypeReference : ITypeReference
    {
        #region ITypeReference Members

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get
            {
                //^ assume false;
                return Dummy.Type;
            }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        public bool IsAlias
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        #endregion ITypeReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members
    }

    internal sealed class DummyUnit : IUnit
    {
        #region IUnit Members

        public AssemblyIdentity ContractAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public AssemblyIdentity CoreAssemblySymbolicIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public string Location
        {
            get { return string.Empty; }
        }

        public IName Name
        {
            get { return Dummy.Name; }
        }

        public IRootUnitNamespace UnitNamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        public IEnumerable<IUnitReference> UnitReferences
        {
            get { return IteratorHelper.GetEmptyEnumerable<IUnitReference>(); }
        }

        public UnitIdentity UnitIdentity
        {
            get
            {
                return new ModuleIdentity(Dummy.Name, string.Empty);
            }
        }

        #endregion IUnit Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region INamespaceRootOwner Members

        public INamespaceDefinition NamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitNamespace;
            }
        }

        #endregion INamespaceRootOwner Members

        #region IUnitReference Members

        public IUnit ResolvedUnit
        {
            get { return Dummy.Unit; }
        }

        #endregion IUnitReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members
    }

    internal sealed class DummyRootUnitNamespace : IRootUnitNamespace
    {
        #region IUnitNamespace Members

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public IUnit Unit
        {
            get { return Dummy.Unit; }
        }

        #endregion IUnitNamespace Members

        #region INamespaceDefinition Members

        public INamespaceRootOwner RootOwner
        {
            get { return Dummy.Unit; }
        }

        public IEnumerable<INamespaceMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<INamespaceMember>(); }
        }

        #endregion INamespaceDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScope<INamespaceMember> Members

        public bool Contains(INamespaceMember member)
        {
            return false;
        }

        public IEnumerable<INamespaceMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMatchingMembers(Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        #endregion IScope<INamespaceMember> Members

        #region IUnitNamespaceReference Members

        IUnitReference IUnitNamespaceReference.Unit
        {
            get { return Dummy.Unit; }
        }

        public IUnitNamespace ResolvedUnitNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IUnitNamespaceReference Members
    }

    internal sealed class DummyNestedUnitNamespace : INestedUnitNamespace
    {
        #region IUnitNamespace Members

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public IUnit Unit
        {
            get { return Dummy.Unit; }
        }

        #endregion IUnitNamespace Members

        #region INamespaceDefinition Members

        public INamespaceRootOwner RootOwner
        {
            get { return Dummy.Unit; }
        }

        public IEnumerable<INamespaceMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<INamespaceMember>(); }
        }

        #endregion INamespaceDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScope<INamespaceMember> Members

        public bool Contains(INamespaceMember member)
        {
            return false;
        }

        public IEnumerable<INamespaceMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMatchingMembers(Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        #endregion IScope<INamespaceMember> Members

        #region IUnitNamespaceReference Members

        IUnitReference IUnitNamespaceReference.Unit
        {
            get { return Dummy.Unit; }
        }

        public IUnitNamespace ResolvedUnitNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IUnitNamespaceReference Members

        #region INestedUnitNamespace Members

        public IUnitNamespace ContainingUnitNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion INestedUnitNamespace Members

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion INamespaceMember Members

        #region INestedUnitNamespaceReference Members

        IUnitNamespaceReference INestedUnitNamespaceReference.ContainingUnitNamespace
        {
            get { return this; }
        }

        public INestedUnitNamespace ResolvedNestedUnitNamespace
        {
            get { return this; }
        }

        #endregion INestedUnitNamespaceReference Members

        #region IContainerMember<INamespaceDefinition> Members

        public INamespaceDefinition Container
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        public IScope<INamespaceMember> ContainingScope
        {
            get { return this; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members
    }

    internal sealed class DummyUnitSet : IUnitSet
    {
        #region IUnitSet Members

        public bool Contains(IUnit unit)
        {
            return false;
        }

        public IEnumerable<IUnit> Units
        {
            get { return IteratorHelper.GetEmptyEnumerable<IUnit>(); }
        }

        public IUnitSetNamespace UnitSetNamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitSetNamespace;
            }
        }

        #endregion IUnitSet Members

        #region INamespaceRootOwner Members

        public INamespaceDefinition NamespaceRoot
        {
            get
            {
                //^ assume false;
                return Dummy.RootUnitSetNamespace;
            }
        }

        #endregion INamespaceRootOwner Members
    }

    internal sealed class DummyRootUnitSetNamespace : IRootUnitSetNamespace
    {
        #region IUnitSetNamespace Members

        public IUnitSet UnitSet
        {
            get { return Dummy.UnitSet; }
        }

        #endregion IUnitSetNamespace Members

        #region INamespaceDefinition Members

        public INamespaceRootOwner RootOwner
        {
            get { return Dummy.UnitSet; }
        }

        public IEnumerable<INamespaceMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<INamespaceMember>(); }
        }

        #endregion INamespaceDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScope<INamespaceMember> Members

        public bool Contains(INamespaceMember member)
        {
            return false;
        }

        public IEnumerable<INamespaceMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMatchingMembers(Function<INamespaceMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        public IEnumerable<INamespaceMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<INamespaceMember>();
        }

        #endregion IScope<INamespaceMember> Members
    }

    internal sealed class DummyResource : IResource
    {
        #region IResource Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<byte> Data
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        public IAssemblyReference DefiningAssembly
        {
            get { return Dummy.AssemblyReference; }
        }

        public bool IsInExternalFile
        {
            get { return false; }
        }

        public IFileReference ExternalFile
        {
            get { return Dummy.FileReference; }
        }

        public bool IsPublic
        {
            get { return false; }
        }

        public IName Name
        {
            get { return Dummy.Name; }
        }

        public IResource Resource
        {
            get { return this; }
        }

        #endregion IResource Members
    }

    internal sealed class DummyAssemblyReference : IAssemblyReference
    {
        #region IAssemblyReference Members

        public IEnumerable<IName> Aliases
        {
            get { return IteratorHelper.GetEmptyEnumerable<IName>(); }
        }

        public string Culture
        {
            get { return string.Empty; }
        }

        public IEnumerable<byte> PublicKeyToken
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        public Version Version
        {
            get { return new Version(0, 0); }
        }

        #endregion IAssemblyReference Members

        #region IUnitReference Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion IUnitReference Members

        #region IAssemblyReference Members

        public IAssembly ResolvedAssembly
        {
            get { return Dummy.Assembly; }
        }

        public AssemblyIdentity AssemblyIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        public AssemblyIdentity UnifiedAssemblyIdentity
        {
            get { return Dummy.Assembly.AssemblyIdentity; }
        }

        #endregion IAssemblyReference Members

        #region IModuleReference Members

        public ModuleIdentity ModuleIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        public IAssemblyReference/*?*/ ContainingAssembly
        {
            get { return null; }
        }

        public IModule ResolvedModule
        {
            get { return Dummy.Module; }
        }

        #endregion IModuleReference Members

        #region IUnitReference Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public UnitIdentity UnitIdentity
        {
            get { return this.AssemblyIdentity; }
        }

        public IUnit ResolvedUnit
        {
            get { return Dummy.Unit; }
        }

        #endregion IUnitReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members
    }

    internal sealed class DummyMarshallingInformation : IMarshallingInformation
    {
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
            get { return System.Runtime.InteropServices.UnmanagedType.Error; }
        }

        public uint IidParameterIndex
        {
            get { return 0; }
        }

        public System.Runtime.InteropServices.UnmanagedType UnmanagedType
        {
            get { return System.Runtime.InteropServices.UnmanagedType.Error; }
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
            get { return System.Runtime.InteropServices.VarEnum.VT_VOID; }
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

    internal sealed class DummySecurityAttribute : ISecurityAttribute
    {
        #region ISecurityAttribute Members

        public SecurityAction Action
        {
            get { return SecurityAction.LinkDemand; }
        }

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion ISecurityAttribute Members
    }

    internal sealed class DummyParameterTypeInformation : IParameterTypeInformation
    {
        #region IParameterTypeInformation Members

        public ISignature ContainingSignature
        {
            get { return Dummy.Method; }
        }

        public IEnumerable<ICustomModifier> CustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool IsByReference
        {
            get { return false; }
        }

        public bool IsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IParameterTypeInformation Members

        #region IParameterListEntry Members

        public ushort Index
        {
            get { return 0; }
        }

        #endregion IParameterListEntry Members
    }

    internal sealed class DummySpecializedNestedTypeDefinition : ISpecializedNestedTypeDefinition
    {
        #region ISpecializedNestedTypeDefinition Members

        public INestedTypeDefinition UnspecializedVersion
        {
            get { return Dummy.NestedType; }
        }

        #endregion ISpecializedNestedTypeDefinition Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Public; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return this.ContainingTypeDefinition; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return this.ContainingTypeDefinition; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region ITypeMemberReference Members

        ITypeReference ITypeMemberReference.ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ITypeMemberReference Members

        #region INestedTypeReference Members

        INestedTypeDefinition INestedTypeReference.ResolvedType
        {
            get { return Dummy.NestedType; }
        }

        #endregion INestedTypeReference Members

        #region ITypeMemberReference Members

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region INamedTypeReference Members

        public bool MangleName
        {
            get { return false; }
        }

        public INamedTypeDefinition ResolvedType
        {
            get { return this; }
        }

        #endregion INamedTypeReference Members
    }

    internal sealed class DummySpecializedFieldDefinition : ISpecializedFieldDefinition
    {
        #region IFieldDefinition Members

        public uint BitLength
        {
            get { return 0; }
        }

        public bool IsBitField
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public bool IsCompileTimeConstant
        {
            get { return false; }
        }

        public bool IsMapped
        {
            get { return false; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return false; }
        }

        public bool IsNotSerialized
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public ISectionBlock FieldMapping
        {
            get { return Dummy.SectionBlock; }
        }

        public uint Offset
        {
            get { return 0; }
        }

        public int SequenceNumber
        {
            get { return 0; }
        }

        public IMetadataConstant CompileTimeValue
        {
            get { return Dummy.Constant; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get { return Dummy.MarshallingInformation; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IFieldDefinition Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Default; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISpecializedFieldDefinition Members

        public IFieldDefinition UnspecializedVersion
        {
            get { return Dummy.Field; }
        }

        #endregion ISpecializedFieldDefinition Members

        #region IFieldReference Members

        public IFieldDefinition ResolvedField
        {
            get { return Dummy.Field; }
        }

        #endregion IFieldReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummySpecializedMethodDefinition : ISpecializedMethodDefinition
    {
        #region ISpecializedMethodDefinition Members

        public IMethodDefinition UnspecializedVersion
        {
            get { return Dummy.Method; }
        }

        #endregion ISpecializedMethodDefinition Members

        #region IMethodDefinition Members

        public bool AcceptsExtraArguments
        {
            get { return false; }
        }

        public IMethodBody Body
        {
            get { return Dummy.MethodBody; }
        }

        public IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericMethodParameter>(); }
        }

        //^ [Pure]
        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public bool HasExplicitThisParameter
        {
            get { return false; }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return false; }
        }

        public bool IsCil
        {
            get { return false; }
        }

        public bool IsConstructor
        {
            get { return false; }
        }

        public bool IsStaticConstructor
        {
            get { return false; }
        }

        public bool IsExternal
        {
            get { return false; }
        }

        public bool IsForwardReference
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsHiddenBySignature
        {
            get { return false; }
        }

        public bool IsNativeCode
        {
            get { return false; }
        }

        public bool IsNewSlot
        {
            get { return false; }
        }

        public bool IsNeverInlined
        {
            get { return false; }
        }

        public bool IsNeverOptimized
        {
            get { return false; }
        }

        public bool IsPlatformInvoke
        {
            get { return false; }
        }

        public bool IsRuntimeImplemented
        {
            get { return false; }
        }

        public bool IsRuntimeInternal
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsVirtual
        {
            get { return false; }
        }

        public bool IsUnmanaged
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.Default; }
        }

        public bool PreserveSignature
        {
            get { return false; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return Dummy.PlatformInvokeInformation; }
        }

        public bool RequiresSecurityObject
        {
            get { return false; }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get { return false; }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get { return Dummy.MarshallingInformation; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        #endregion IMethodDefinition Members

        #region ISignature Members

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterDefinition>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ISignature Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Default; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion ISignature Members

        #region IMethodReference Members

        public uint InternedKey
        {
            get { return 0; }
        }

        public ushort ParameterCount
        {
            get { return 0; }
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
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members
    }

    internal sealed class DummySpecializedPropertyDefinition : ISpecializedPropertyDefinition
    {
        #region ISpecializedPropertyDefinition Members

        public IPropertyDefinition UnspecializedVersion
        {
            get { return Dummy.Property; }
        }

        #endregion ISpecializedPropertyDefinition Members

        #region IPropertyDefinition Members

        public IEnumerable<IMethodReference> Accessors
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodReference>(); }
        }

        public IMetadataConstant DefaultValue
        {
            get { return Dummy.Constant; }
        }

        public IMethodReference/*?*/ Getter
        {
            get { return null; }
        }

        public bool HasDefaultValue
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public IMethodReference/*?*/ Setter
        {
            get { return null; }
        }

        #endregion IPropertyDefinition Members

        #region ISignature Members

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterDefinition>(); }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.C; }
        }

        #endregion ISignature Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return Dummy.Property; }
        }

        #endregion ITypeMemberReference Members

        #region IContainerMember<ITypeDefinition> Members

        public ITypeDefinition Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IDoubleDispatcher Members

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDoubleDispatcher Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        public IScope<ITypeDefinitionMember> ContainingScope
        {
            get
            {
                return Dummy.Type;
            }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion ISignature Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummyFunctionPointerType : IFunctionPointer
    {
        #region IFunctionPointer Members

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.Default; }
        }

        public IEnumerable<IParameterTypeInformation> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        public IEnumerable<IParameterTypeInformation> ExtraArgumentTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IFunctionPointer Members

        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members



        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        ITypeDefinition ITypeReference.ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members
    }

    internal sealed class DummyLocalVariable : ILocalDefinition
    {
        #region ILocalDefinition Members

        public IMetadataConstant CompileTimeValue
        {
            get { return Dummy.Constant; }
        }

        public IEnumerable<ICustomModifier> CustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool IsConstant
        {
            get { return false; }
        }

        public bool IsModified
        {
            get { return false; }
        }

        public bool IsPinned
        {
            get { return false; }
        }

        public bool IsReference
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion ILocalDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class DummyFieldReference : IFieldReference
    {
        #region IFieldReference Members

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        public IFieldDefinition ResolvedField
        {
            get { return Dummy.Field; }
        }

        #endregion IFieldReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.Type; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return Dummy.Field; }
        }

        #endregion ITypeMemberReference Members

        #region IReference Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IReference Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members
    }

    internal sealed class DummyParameterDefinition : IParameterDefinition
    {
        #region IParameterDefinition Members

        public ISignature ContainingSignature
        {
            get { return Dummy.Method; }
        }

        public IMetadataConstant DefaultValue
        {
            get { return Dummy.Constant; }
        }

        public bool HasDefaultValue
        {
            get { return false; }
        }

        public bool IsIn
        {
            get { return false; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return false; }
        }

        public bool IsOptional
        {
            get { return false; }
        }

        public bool IsOut
        {
            get { return false; }
        }

        public bool IsParameterArray
        {
            get { return false; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get { return Dummy.MarshallingInformation; }
        }

        public ITypeReference ParamArrayElementType
        {
            get { return Dummy.TypeReference; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        #endregion IParameterDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region IParameterListEntry Members

        public ushort Index
        {
            get { return 0; }
        }

        #endregion IParameterListEntry Members

        #region IParameterTypeInformation Members

        public IEnumerable<ICustomModifier> CustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool IsByReference
        {
            get { return false; }
        }

        public bool IsModified
        {
            get { return false; }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IParameterTypeInformation Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummySectionBlock : ISectionBlock
    {
        #region ISectionBlock Members

        public PESectionKind PESectionKind
        {
            get { return PESectionKind.Illegal; }
        }

        public uint Offset
        {
            get { return 0; }
        }

        public uint Size
        {
            get { return 0; }
        }

        public IEnumerable<byte> Data
        {
            get { return IteratorHelper.GetEmptyEnumerable<byte>(); }
        }

        #endregion ISectionBlock Members
    }

    internal sealed class DummyPlatformInvokeInformation : IPlatformInvokeInformation
    {
        #region IPlatformInvokeInformation Members

        public IName ImportName
        {
            get { return Dummy.Name; }
        }

        public IModuleReference ImportModule
        {
            get { return Dummy.ModuleReference; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Unspecified; }
        }

        public bool NoMangle
        {
            get { return false; }
        }

        public bool SupportsLastError
        {
            get { return false; }
        }

        public PInvokeCallingConvention PInvokeCallingConvention
        {
            get { return PInvokeCallingConvention.CDecl; }
        }

        public bool? UseBestFit
        {
            get { return null; }
        }

        public bool? ThrowExceptionForUnmappableChar
        {
            get { return null; }
        }

        #endregion IPlatformInvokeInformation Members
    }

    internal sealed class DummyGlobalMethodDefinition : IGlobalMethodDefinition
    {
        #region ISignature Members

        public bool ReturnValueIsByRef
        {
            get { return false; }
        }

        public IEnumerable<IParameterDefinition> Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterDefinition>(); }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public IEnumerable<ICustomAttribute> ReturnValueAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public IEnumerable<ICustomModifier> ReturnValueCustomModifiers
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomModifier>(); }
        }

        public bool ReturnValueIsModified
        {
            get { return false; }
        }

        #endregion ISignature Members

        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        public INamespaceDefinition Container
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IMethodDefinition Members

        public bool AcceptsExtraArguments
        {
            get { return false; }
        }

        public IMethodBody Body
        {
            get { return Dummy.MethodBody; }
        }

        public IEnumerable<IGenericMethodParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericMethodParameter>(); }
        }

        //^ [Pure]
        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public bool HasExplicitThisParameter
        {
            get { return false; }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsAccessCheckedOnOverride
        {
            get { return false; }
        }

        public bool IsCil
        {
            get { return false; }
        }

        public bool IsConstructor
        {
            get { return false; }
        }

        public bool IsStaticConstructor
        {
            get { return false; }
        }

        public bool IsExternal
        {
            get { return false; }
        }

        public bool IsForwardReference
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsHiddenBySignature
        {
            get { return false; }
        }

        public bool IsNativeCode
        {
            get { return false; }
        }

        public bool IsNewSlot
        {
            get { return false; }
        }

        public bool IsNeverInlined
        {
            get { return false; }
        }

        public bool IsNeverOptimized
        {
            get { return false; }
        }

        public bool IsPlatformInvoke
        {
            get { return false; }
        }

        public bool IsRuntimeImplemented
        {
            get { return false; }
        }

        public bool IsRuntimeInternal
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsVirtual
        {
            get { return false; }
        }

        public bool IsUnmanaged
        {
            get { return false; }
        }

        public CallingConvention CallingConvention
        {
            get { return CallingConvention.Default; }
        }

        public bool PreserveSignature
        {
            get { return false; }
        }

        public IPlatformInvokeInformation PlatformInvokeData
        {
            get { return Dummy.PlatformInvokeInformation; }
        }

        public bool RequiresSecurityObject
        {
            get { return false; }
        }

        public bool ReturnValueIsMarshalledExplicitly
        {
            get { return false; }
        }

        public IMarshallingInformation ReturnValueMarshallingInformation
        {
            get { return Dummy.MarshallingInformation; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        #endregion IMethodDefinition Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        public IScope<INamespaceMember> ContainingScope
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        ITypeDefinition IContainerMember<ITypeDefinition>.Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        IScope<ITypeDefinitionMember> IScopeMember<IScope<ITypeDefinitionMember>>.ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region ISignature Members

        IEnumerable<IParameterTypeInformation> ISignature.Parameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IParameterTypeInformation>(); }
        }

        #endregion ISignature Members

        #region IMethodReference Members

        public uint InternedKey
        {
            get { return 0; }
        }

        public ushort ParameterCount
        {
            get { return 0; }
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
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members
    }

    internal sealed class DummyGlobalFieldDefinition : IGlobalFieldDefinition
    {
        #region INamedEntity Members

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion INamedEntity Members

        #region INamespaceMember Members

        public INamespaceDefinition ContainingNamespace
        {
            get { return Dummy.RootUnitNamespace; }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion INamespaceMember Members

        #region IContainerMember<INamespaceDefinition> Members

        public INamespaceDefinition Container
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IContainerMember<INamespaceDefinition> Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        #endregion IDefinition Members

        #region IScopeMember<IScope<INamespaceMember>> Members

        public IScope<INamespaceMember> ContainingScope
        {
            get { return Dummy.RootUnitNamespace; }
        }

        #endregion IScopeMember<IScope<INamespaceMember>> Members

        #region IFieldDefinition Members

        public uint BitLength
        {
            get { return 0; }
        }

        public bool IsBitField
        {
            get { return false; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public bool IsCompileTimeConstant
        {
            get { return false; }
        }

        public bool IsMapped
        {
            get { return false; }
        }

        public bool IsMarshalledExplicitly
        {
            get { return false; }
        }

        public bool IsNotSerialized
        {
            get { return true; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsStatic
        {
            get { return false; }
        }

        public ISectionBlock FieldMapping
        {
            get { return Dummy.SectionBlock; }
        }

        public uint Offset
        {
            get { return 0; }
        }

        public int SequenceNumber
        {
            get { return 0; }
        }

        public IMetadataConstant CompileTimeValue
        {
            get { return Dummy.Constant; }
        }

        public IMarshallingInformation MarshallingInformation
        {
            get
            {
                //^ assume false;
                IMarshallingInformation/*?*/ dummyValue = null;
                //^ assume dummyValue != null;
                return dummyValue;
            }
        }

        public ITypeReference Type
        {
            get { return Dummy.TypeReference; }
        }

        #endregion IFieldDefinition Members

        #region ITypeDefinitionMember Members

        public ITypeDefinition ContainingTypeDefinition
        {
            get { return Dummy.Type; }
        }

        public TypeMemberVisibility Visibility
        {
            get { return TypeMemberVisibility.Other; }
        }

        #endregion ITypeDefinitionMember Members

        #region IContainerMember<ITypeDefinition> Members

        ITypeDefinition IContainerMember<ITypeDefinition>.Container
        {
            get { return Dummy.Type; }
        }

        #endregion IContainerMember<ITypeDefinition> Members

        #region IScopeMember<IScope<ITypeDefinitionMember>> Members

        IScope<ITypeDefinitionMember> IScopeMember<IScope<ITypeDefinitionMember>>.ContainingScope
        {
            get { return Dummy.Type; }
        }

        #endregion IScopeMember<IScope<ITypeDefinitionMember>> Members

        #region IFieldReference Members

        public IFieldDefinition ResolvedField
        {
            get { return this; }
        }

        #endregion IFieldReference Members

        #region ITypeMemberReference Members

        public ITypeReference ContainingType
        {
            get { return Dummy.TypeReference; }
        }

        public ITypeDefinitionMember ResolvedTypeDefinitionMember
        {
            get { return this; }
        }

        #endregion ITypeMemberReference Members

        #region IMetadataConstantContainer

        public IMetadataConstant Constant
        {
            get { return Dummy.Constant; }
        }

        #endregion IMetadataConstantContainer
    }

    internal sealed class DummyOperation : IOperation
    {
        #region IOperation Members

        public OperationCode OperationCode
        {
            get { return OperationCode.Nop; }
        }

        public uint Offset
        {
            get { return 0; }
        }

        public ILocation Location
        {
            get { return Dummy.Location; }
        }

        public object/*?*/ Value
        {
            get { return null; }
        }

        #endregion IOperation Members
    }

    internal sealed class DummyDocument : IDocument
    {
        #region IDocument Members

        public string Location
        {
            get { return string.Empty; }
        }

        public IName Name
        {
            get { return Dummy.Name; }
        }

        #endregion IDocument Members
    }

    internal sealed class DummyLocation : ILocation
    {
        #region ILocation Members

        public IDocument Document
        {
            get { return Dummy.Document; }
        }

        #endregion ILocation Members
    }

    internal sealed class DummyOperationExceptionInformation : IOperationExceptionInformation
    {
        #region IOperationExceptionInformation Members

        public HandlerKind HandlerKind
        {
            get { return HandlerKind.Illegal; }
        }

        public ITypeReference ExceptionType
        {
            get { return Dummy.TypeReference; }
        }

        public uint TryStartOffset
        {
            get { return 0; }
        }

        public uint TryEndOffset
        {
            get { return 0; }
        }

        public uint FilterDecisionStartOffset
        {
            get { return 0; }
        }

        public uint HandlerStartOffset
        {
            get { return 0; }
        }

        public uint HandlerEndOffset
        {
            get { return 0; }
        }

        #endregion IOperationExceptionInformation Members
    }

    internal sealed class DummyInternFactory : IInternFactory
    {
        #region IInternFactory Members

        public uint GetAssemblyInternedKey(AssemblyIdentity assemblyIdentity)
        {
            return 0;
        }

        public uint GetModuleInternedKey(ModuleIdentity moduleIdentity)
        {
            return 0;
        }

        public uint GetMethodInternedKey(IMethodReference methodReference)
        {
            return 0;
        }

        public uint GetVectorTypeReferenceInternedKey(ITypeReference elementTypeReference)
        {
            return 0;
        }

        public uint GetMatrixTypeReferenceInternedKey(ITypeReference elementTypeReference, int rank, IEnumerable<ulong> sizes, IEnumerable<int> lowerBounds)
        {
            return 0;
        }

        public uint GetGenericTypeInstanceReferenceInternedKey(ITypeReference genericTypeReference, IEnumerable<ITypeReference> genericArguments)
        {
            return 0;
        }

        public uint GetPointerTypeReferenceInternedKey(ITypeReference targetTypeReference)
        {
            return 0;
        }

        public uint GetManagedPointerTypeReferenceInternedKey(ITypeReference targetTypeReference)
        {
            return 0;
        }

        public uint GetFunctionPointerTypeReferenceInternedKey(CallingConvention callingConvention, IEnumerable<IParameterTypeInformation> parameters, IEnumerable<IParameterTypeInformation> extraArgumentTypes, IEnumerable<ICustomModifier> returnValueCustomModifiers, bool returnValueIsByRef, ITypeReference returnType)
        {
            return 0;
        }

        public uint GetTypeReferenceInternedKey(ITypeReference typeReference)
        {
            return 0;
        }

        public uint GetNamespaceTypeReferenceInternedKey(IUnitNamespaceReference containingUnitNamespace, IName typeName, uint genericParameterCount, bool forPrivateModuleType)
        {
            return 0;
        }

        public uint GetNestedTypeReferenceInternedKey(ITypeReference containingTypeReference, IName typeName, uint genericParameterCount)
        {
            return 0;
        }

        public uint GetGenericTypeParameterReferenceInternedKey(ITypeReference definingTypeReference, int index)
        {
            return 0;
        }

        public uint GetModifiedTypeReferenceInternedKey(ITypeReference typeReference, IEnumerable<ICustomModifier> customModifiers)
        {
            return 0;
        }

        public uint GetGenericMethodParameterReferenceInternedKey(IMethodReference definingMethodReference, int index)
        {
            return 0;
        }

        #endregion IInternFactory Members
    }

    internal sealed class DummyArrayType : IArrayType
    {
        #region ITypeDefinition Members

        public ushort Alignment
        {
            get { return 0; }
        }

        public IEnumerable<ITypeReference> BaseClasses
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public IEnumerable<IEventDefinition> Events
        {
            get { return IteratorHelper.GetEmptyEnumerable<IEventDefinition>(); }
        }

        public IEnumerable<IFieldDefinition> Fields
        {
            get { return IteratorHelper.GetEmptyEnumerable<IFieldDefinition>(); }
        }

        public IEnumerable<IMethodDefinition> Methods
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodDefinition>(); }
        }

        public IEnumerable<INestedTypeDefinition> NestedTypes
        {
            get { return IteratorHelper.GetEmptyEnumerable<INestedTypeDefinition>(); }
        }

        public IEnumerable<IPropertyDefinition> Properties
        {
            get { return IteratorHelper.GetEmptyEnumerable<IPropertyDefinition>(); }
        }

        public IEnumerable<IMethodImplementation> ExplicitImplementationOverrides
        {
            get { return IteratorHelper.GetEmptyEnumerable<IMethodImplementation>(); }
        }

        public IEnumerable<IGenericTypeParameter> GenericParameters
        {
            get { return IteratorHelper.GetEmptyEnumerable<IGenericTypeParameter>(); }
        }

        public ushort GenericParameterCount
        {
            get
            {
                //^ assume false;
                return 0;
            }
        }

        public IGenericTypeInstanceReference InstanceType
        {
            get { return Dummy.GenericTypeInstance; }
        }

        public IEnumerable<ITypeReference> Interfaces
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeReference>(); }
        }

        public bool IsAbstract
        {
            get { return false; }
        }

        public bool IsClass
        {
            get { return true; }
        }

        public bool IsDelegate
        {
            get { return false; }
        }

        public bool IsEnum
        {
            get { return false; }
        }

        public bool IsGeneric
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsReferenceType
        {
            get { return false; }
        }

        public bool IsSealed
        {
            get { return true; }
        }

        public bool IsStatic
        {
            get { return true; }
        }

        public bool IsValueType
        {
            get { return false; }
        }

        public bool IsStruct
        {
            get { return false; }
        }

        public IEnumerable<ITypeDefinitionMember> Members
        {
            get { return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>(); }
        }

        public IPlatformType PlatformType
        {
            get { return Dummy.PlatformType; }
        }

        public IEnumerable<ITypeDefinitionMember> PrivateHelperMembers
        {
            get { return this.Members; }
        }

        public uint SizeOf
        {
            get { return 0; }
        }

        public IEnumerable<ISecurityAttribute> SecurityAttributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ISecurityAttribute>(); }
        }

        public ITypeReference UnderlyingType
        {
            get { return Dummy.TypeReference; }
        }

        public PrimitiveTypeCode TypeCode
        {
            get { return PrimitiveTypeCode.Invalid; }
        }

        public IEnumerable<ILocation> Locations
        {
            get { return IteratorHelper.GetEmptyEnumerable<ILocation>(); }
        }

        public LayoutKind Layout
        {
            get { return LayoutKind.Auto; }
        }

        public bool IsSpecialName
        {
            get { return false; }
        }

        public bool IsComObject
        {
            get { return false; }
        }

        public bool IsSerializable
        {
            get { return false; }
        }

        public bool IsBeforeFieldInit
        {
            get { return false; }
        }

        public StringFormatKind StringFormat
        {
            get { return StringFormatKind.Ansi; }
        }

        public bool IsRuntimeSpecial
        {
            get { return false; }
        }

        public bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        #endregion ITypeDefinition Members

        #region IDefinition Members

        public IEnumerable<ICustomAttribute> Attributes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ICustomAttribute>(); }
        }

        public void Dispatch(IMetadataVisitor visitor)
        {
        }

        #endregion IDefinition Members

        #region IScope<ITypeDefinitionMember> Members

        public bool Contains(ITypeDefinitionMember member)
        {
            return false;
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembersNamed(IName name, bool ignoreCase, Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMatchingMembers(Function<ITypeDefinitionMember, bool> predicate)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        public IEnumerable<ITypeDefinitionMember> GetMembersNamed(IName name, bool ignoreCase)
        {
            return IteratorHelper.GetEmptyEnumerable<ITypeDefinitionMember>();
        }

        #endregion IScope<ITypeDefinitionMember> Members

        #region ITypeReference Members

        public bool IsAlias
        {
            get { return false; }
        }

        public IAliasForType AliasForType
        {
            get { return Dummy.AliasForType; }
        }

        public ITypeDefinition ResolvedType
        {
            get { return this; }
        }

        public uint InternedKey
        {
            get { return 0; }
        }

        #endregion ITypeReference Members

        #region IArrayTypeReference Members

        public ITypeReference ElementType
        {
            get { return Dummy.TypeReference; }
        }

        public bool IsVector
        {
            get { return true; }
        }

        public IEnumerable<int> LowerBounds
        {
            get { return IteratorHelper.GetEmptyEnumerable<int>(); }
        }

        public uint Rank
        {
            get { return 0; }
        }

        public IEnumerable<ulong> Sizes
        {
            get { return IteratorHelper.GetEmptyEnumerable<ulong>(); }
        }

        #endregion IArrayTypeReference Members
    }

#pragma warning restore 1591
}