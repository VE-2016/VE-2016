// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2035 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public class TypeReference : AbstractNode, INullable, ICloneable
    {
        public static readonly TypeReference StructConstraint = new TypeReference("constraint: struct");
        public static readonly TypeReference ClassConstraint = new TypeReference("constraint: class");
        public static readonly TypeReference NewConstraint = new TypeReference("constraint: new");

        private string _type = "";
        private string _systemType = "";
        private int _pointerNestingLevel;
        private int[] _rankSpecifier;
        private List<TypeReference> _genericTypes = new List<TypeReference>();
        private bool _isGlobal;

        #region Static primitive type list
        private static Dictionary<string, string> s_types = new Dictionary<string, string>();
        private static Dictionary<string, string> s_vbtypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<string, string> s_typesReverse = new Dictionary<string, string>();
        private static Dictionary<string, string> s_vbtypesReverse = new Dictionary<string, string>();

        static TypeReference()
        {
            // C# types
            s_types.Add("bool", "System.Boolean");
            s_types.Add("byte", "System.Byte");
            s_types.Add("char", "System.Char");
            s_types.Add("decimal", "System.Decimal");
            s_types.Add("double", "System.Double");
            s_types.Add("float", "System.Single");
            s_types.Add("int", "System.Int32");
            s_types.Add("long", "System.Int64");
            s_types.Add("object", "System.Object");
            s_types.Add("sbyte", "System.SByte");
            s_types.Add("short", "System.Int16");
            s_types.Add("string", "System.String");
            s_types.Add("uint", "System.UInt32");
            s_types.Add("ulong", "System.UInt64");
            s_types.Add("ushort", "System.UInt16");
            s_types.Add("void", "System.Void");

            // VB.NET types
            s_vbtypes.Add("Boolean", "System.Boolean");
            s_vbtypes.Add("Byte", "System.Byte");
            s_vbtypes.Add("SByte", "System.SByte");
            s_vbtypes.Add("Date", "System.DateTime");
            s_vbtypes.Add("Char", "System.Char");
            s_vbtypes.Add("Decimal", "System.Decimal");
            s_vbtypes.Add("Double", "System.Double");
            s_vbtypes.Add("Single", "System.Single");
            s_vbtypes.Add("Integer", "System.Int32");
            s_vbtypes.Add("Long", "System.Int64");
            s_vbtypes.Add("UInteger", "System.UInt32");
            s_vbtypes.Add("ULong", "System.UInt64");
            s_vbtypes.Add("Object", "System.Object");
            s_vbtypes.Add("Short", "System.Int16");
            s_vbtypes.Add("UShort", "System.UInt16");
            s_vbtypes.Add("String", "System.String");

            foreach (KeyValuePair<string, string> pair in s_types)
            {
                s_typesReverse.Add(pair.Value, pair.Key);
            }
            foreach (KeyValuePair<string, string> pair in s_vbtypes)
            {
                s_vbtypesReverse.Add(pair.Value, pair.Key);
            }
        }

        /// <summary>
        /// Gets a shortname=>full name dictionary of C# types.
        /// </summary>
        public static IDictionary<string, string> PrimitiveTypesCSharp
        {
            get { return s_types; }
        }

        /// <summary>
        /// Gets a shortname=>full name dictionary of VB types.
        /// </summary>
        public static IDictionary<string, string> PrimitiveTypesVB
        {
            get { return s_vbtypes; }
        }

        /// <summary>
        /// Gets a full name=>shortname dictionary of C# types.
        /// </summary>
        public static IDictionary<string, string> PrimitiveTypesCSharpReverse
        {
            get { return s_typesReverse; }
        }

        /// <summary>
        /// Gets a full name=>shortname dictionary of VB types.
        /// </summary>
        public static IDictionary<string, string> PrimitiveTypesVBReverse
        {
            get { return s_vbtypesReverse; }
        }


        private static string GetSystemType(string type)
        {
            if (s_types == null) return type;

            string systemType;
            if (s_types.TryGetValue(type, out systemType))
            {
                return systemType;
            }
            if (s_vbtypes.TryGetValue(type, out systemType))
            {
                return systemType;
            }
            return type;
        }
        #endregion

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public virtual TypeReference Clone()
        {
            TypeReference c = new TypeReference(_type, _systemType);
            CopyFields(this, c);
            return c;
        }

        /// <summary>
        /// Copies the pointerNestingLevel, RankSpecifier, GenericTypes and IsGlobal flag
        /// from <paramref name="from"/> to <paramref name="to"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="to"/> already contains generics, the new generics are appended to the list.
        /// </remarks>
        protected static void CopyFields(TypeReference from, TypeReference to)
        {
            to._pointerNestingLevel = from._pointerNestingLevel;
            if (from._rankSpecifier != null)
            {
                to._rankSpecifier = (int[])from._rankSpecifier.Clone();
            }
            foreach (TypeReference r in from._genericTypes)
            {
                to._genericTypes.Add(r.Clone());
            }
            to._isGlobal = from._isGlobal;
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                Debug.Assert(value != null);
                _type = value;
                _systemType = GetSystemType(_type);
            }
        }

        /// <summary>
        /// Removes the last identifier from the type.
        /// e.g. "System.String.Length" becomes "System.String" or
        /// "System.Collections.IEnumerable(of string).Current" becomes "System.Collections.IEnumerable(of string)"
        /// This is used for explicit interface implementation in VB.
        /// </summary>
        public static string StripLastIdentifierFromType(ref TypeReference tr)
        {
            if (tr is InnerClassTypeReference && ((InnerClassTypeReference)tr).Type.IndexOf('.') < 0)
            {
                string ident = ((InnerClassTypeReference)tr).Type;
                tr = ((InnerClassTypeReference)tr).BaseType;
                return ident;
            }
            else
            {
                int pos = tr.Type.LastIndexOf('.');
                if (pos < 0)
                    return tr.Type;
                string ident = tr.Type.Substring(pos + 1);
                tr.Type = tr.Type.Substring(0, pos);
                return ident;
            }
        }

        public string SystemType
        {
            get
            {
                return _systemType;
            }
        }

        public int PointerNestingLevel
        {
            get
            {
                return _pointerNestingLevel;
            }
            set
            {
                _pointerNestingLevel = value;
            }
        }

        /// <summary>
        /// The rank of the array type.
        /// For "object[]", this is { 0 }; for "object[,]", it is {1}.
        /// For "object[,][,,][]", it is {1, 2, 0}.
        /// For non-array types, this property is null or {}.
        /// </summary>
        public int[] RankSpecifier
        {
            get
            {
                return _rankSpecifier;
            }
            set
            {
                _rankSpecifier = value;
            }
        }

        public List<TypeReference> GenericTypes
        {
            get
            {
                return _genericTypes;
            }
        }

        public bool IsArrayType
        {
            get
            {
                return _rankSpecifier != null && _rankSpecifier.Length > 0;
            }
        }

        public static TypeReference CheckNull(TypeReference typeReference)
        {
            return typeReference ?? NullTypeReference.Instance;
        }

        public static NullTypeReference Null
        {
            get
            {
                return NullTypeReference.Instance;
            }
        }

        public virtual bool IsNull
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets/Sets if the type reference had a "global::" prefix.
        /// </summary>
        public bool IsGlobal
        {
            get
            {
                return _isGlobal;
            }
            set
            {
                _isGlobal = value;
            }
        }

        public TypeReference(string type)
        {
            this.Type = type;
        }

        public TypeReference(string type, string systemType)
        {
            _type = type;
            _systemType = systemType;
        }

        public TypeReference(string type, List<TypeReference> genericTypes) : this(type)
        {
            if (genericTypes != null)
            {
                _genericTypes = genericTypes;
            }
        }

        public TypeReference(string type, int[] rankSpecifier) : this(type, 0, rankSpecifier)
        {
        }

        public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier) : this(type, pointerNestingLevel, rankSpecifier, null)
        {
        }

        public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier, List<TypeReference> genericTypes)
        {
            Debug.Assert(type != null);
            _type = type;
            _systemType = GetSystemType(type);
            _pointerNestingLevel = pointerNestingLevel;
            _rankSpecifier = rankSpecifier;
            if (genericTypes != null)
            {
                _genericTypes = genericTypes;
            }
        }

        protected TypeReference()
        { }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return visitor.VisitTypeReference(this, data);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(_type);
            if (_genericTypes != null && _genericTypes.Count > 0)
            {
                b.Append('<');
                for (int i = 0; i < _genericTypes.Count; i++)
                {
                    if (i > 0) b.Append(',');
                    b.Append(_genericTypes[i].ToString());
                }
                b.Append('>');
            }
            if (_pointerNestingLevel > 0)
            {
                b.Append('*', _pointerNestingLevel);
            }
            if (IsArrayType)
            {
                foreach (int rank in _rankSpecifier)
                {
                    b.Append('[');
                    if (rank < 0)
                        b.Append('`', -rank);
                    else
                        b.Append(',', rank);
                    b.Append(']');
                }
            }
            return b.ToString();
        }

        public static bool AreEqualReferences(TypeReference a, TypeReference b)
        {
            if (a == b) return true;
            if (a == null || b == null) return false;
            if (a is InnerClassTypeReference) a = ((InnerClassTypeReference)a).CombineToNormalTypeReference();
            if (b is InnerClassTypeReference) b = ((InnerClassTypeReference)b).CombineToNormalTypeReference();
            if (a._systemType != b._systemType) return false;
            if (a._pointerNestingLevel != b._pointerNestingLevel) return false;
            if (a.IsArrayType != b.IsArrayType) return false;
            if (a.IsArrayType)
            {
                if (a._rankSpecifier.Length != b._rankSpecifier.Length) return false;
                for (int i = 0; i < a._rankSpecifier.Length; i++)
                {
                    if (a._rankSpecifier[i] != b._rankSpecifier[i]) return false;
                }
            }
            if (a._genericTypes.Count != b._genericTypes.Count) return false;
            for (int i = 0; i < a._genericTypes.Count; i++)
            {
                if (!AreEqualReferences(a._genericTypes[i], b._genericTypes[i]))
                    return false;
            }
            return true;
        }
    }

    public class NullTypeReference : TypeReference
    {
        private static NullTypeReference s_nullTypeReference = new NullTypeReference();
        public override bool IsNull
        {
            get
            {
                return true;
            }
        }
        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return null;
        }
        public static NullTypeReference Instance
        {
            get
            {
                return s_nullTypeReference;
            }
        }
        private NullTypeReference() { }
        public override string ToString()
        {
            return String.Format("[NullTypeReference]");
        }
    }

    /// <summary>
    /// We need this special type reference for cases like
    /// OuterClass(Of T1).InnerClass(Of T2) (in expression or type context)
    /// or Dictionary(Of String, NamespaceStruct).KeyCollection (in type context, otherwise it's a
    /// MemberReferenceExpression)
    /// </summary>
    public class InnerClassTypeReference : TypeReference
    {
        private TypeReference _baseType;

        public TypeReference BaseType
        {
            get { return _baseType; }
            set { _baseType = value; }
        }

        public override TypeReference Clone()
        {
            InnerClassTypeReference c = new InnerClassTypeReference(_baseType.Clone(), Type, GenericTypes);
            CopyFields(this, c);
            return c;
        }

        public InnerClassTypeReference(TypeReference outerClass, string innerType, List<TypeReference> innerGenericTypes)
            : base(innerType, innerGenericTypes)
        {
            _baseType = outerClass;
        }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return visitor.VisitInnerClassTypeReference(this, data);
        }

        /// <summary>
        /// Creates a type reference where all type parameters are specified for the innermost class.
        /// Namespace.OuterClass(of string).InnerClass(of integer).InnerInnerClass
        /// becomes Namespace.OuterClass.InnerClass.InnerInnerClass(of string, integer)
        /// </summary>
        public TypeReference CombineToNormalTypeReference()
        {
            TypeReference tr = (_baseType is InnerClassTypeReference)
                ? ((InnerClassTypeReference)_baseType).CombineToNormalTypeReference()
                : _baseType.Clone();
            CopyFields(this, tr);
            tr.Type += "." + Type;
            return tr;
        }

        public override string ToString()
        {
            return "[InnerClassTypeReference: (" + _baseType.ToString() + ")." + base.ToString() + "]";
        }
    }
}
