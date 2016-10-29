// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2029 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultClass : AbstractNamedEntity, IClass, IComparable
    {
        private ClassType _classType;
        private DomRegion _region;
        private DomRegion _bodyRegion;

        private ICompilationUnit _compilationUnit;

        private List<IReturnType> _baseTypes = null;

        private List<IClass> _innerClasses = null;
        private List<IField> _fields = null;
        private List<IProperty> _properties = null;
        private List<IMethod> _methods = null;
        private List<IEvent> _events = null;
        private IList<ITypeParameter> _typeParameters = null;

        private byte _flags;
        private const byte hasPublicOrInternalStaticMembersFlag = 0x02;
        private const byte hasExtensionMethodsFlag = 0x04;

        internal byte Flags
        {
            get
            {
                if (_flags == 0)
                {
                    _flags = 1;
                    foreach (IMember m in this.Fields)
                    {
                        if (m.IsStatic && (m.IsPublic || m.IsInternal))
                        {
                            _flags |= hasPublicOrInternalStaticMembersFlag;
                        }
                    }
                    foreach (IProperty m in this.Properties)
                    {
                        if (m.IsStatic && (m.IsPublic || m.IsInternal))
                        {
                            _flags |= hasPublicOrInternalStaticMembersFlag;
                        }
                        if (m.IsExtensionMethod)
                        {
                            _flags |= hasExtensionMethodsFlag;
                        }
                    }
                    foreach (IMethod m in this.Methods)
                    {
                        if (m.IsStatic && (m.IsPublic || m.IsInternal))
                        {
                            _flags |= hasPublicOrInternalStaticMembersFlag;
                        }
                        if (m.IsExtensionMethod)
                        {
                            _flags |= hasExtensionMethodsFlag;
                        }
                    }
                    foreach (IMember m in this.Events)
                    {
                        if (m.IsStatic && (m.IsPublic || m.IsInternal))
                        {
                            _flags |= hasPublicOrInternalStaticMembersFlag;
                        }
                    }
                    foreach (IClass c in this.InnerClasses)
                    {
                        if (c.IsPublic || c.IsInternal)
                        {
                            _flags |= hasPublicOrInternalStaticMembersFlag;
                        }
                    }
                }
                return _flags;
            }
            set
            {
                _flags = value;
            }
        }

        public bool HasPublicOrInternalStaticMembers
        {
            get
            {
                return (Flags & hasPublicOrInternalStaticMembersFlag) == hasPublicOrInternalStaticMembersFlag;
            }
        }

        public bool HasExtensionMethods
        {
            get
            {
                return (Flags & hasExtensionMethodsFlag) == hasExtensionMethodsFlag;
            }
        }

        public DefaultClass(ICompilationUnit compilationUnit, string fullyQualifiedName) : base(null)
        {
            _compilationUnit = compilationUnit;
            this.FullyQualifiedName = fullyQualifiedName;
        }

        public DefaultClass(ICompilationUnit compilationUnit, IClass declaringType) : base(declaringType)
        {
            _compilationUnit = compilationUnit;
        }

        public DefaultClass(ICompilationUnit compilationUnit, ClassType classType, ModifierEnum modifiers, DomRegion region, IClass declaringType) : base(declaringType)
        {
            _compilationUnit = compilationUnit;
            _region = region;
            _classType = classType;
            Modifiers = modifiers;
        }

        private IReturnType _defaultReturnType;

        public IReturnType DefaultReturnType
        {
            get
            {
                if (_defaultReturnType == null)
                    _defaultReturnType = CreateDefaultReturnType();
                return _defaultReturnType;
            }
        }

        protected virtual IReturnType CreateDefaultReturnType()
        {
            if (IsPartial)
            {
                return new GetClassReturnType(ProjectContent, FullyQualifiedName, TypeParameters.Count);
            }
            else
            {
                return new DefaultReturnType(this);
            }
        }

        public bool IsPartial
        {
            get
            {
                return (this.Modifiers & ModifierEnum.Partial) == ModifierEnum.Partial;
            }
            set
            {
                if (value)
                    this.Modifiers |= ModifierEnum.Partial;
                else
                    this.Modifiers &= ~ModifierEnum.Partial;
                _defaultReturnType = null; // re-create default return type
            }
        }

        public IClass GetCompoundClass()
        {
            return this.DefaultReturnType.GetUnderlyingClass() ?? this;
        }

        protected override void OnFullyQualifiedNameChanged(EventArgs e)
        {
            base.OnFullyQualifiedNameChanged(e);
            GetClassReturnType rt = _defaultReturnType as GetClassReturnType;
            if (rt != null)
            {
                rt.SetFullyQualifiedName(FullyQualifiedName);
            }
        }

        public ICompilationUnit CompilationUnit
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return _compilationUnit;
            }
        }

        public IProjectContent ProjectContent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return CompilationUnit.ProjectContent;
            }
        }

        public ClassType ClassType
        {
            get
            {
                return _classType;
            }
            set
            {
                _classType = value;
            }
        }

        public DomRegion Region
        {
            get
            {
                return _region;
            }
            set
            {
                _region = value;
            }
        }

        public DomRegion BodyRegion
        {
            get
            {
                return _bodyRegion;
            }
            set
            {
                _bodyRegion = value;
            }
        }

        public override string DotNetName
        {
            get
            {
                if (_typeParameters == null || _typeParameters.Count == 0)
                {
                    return FullyQualifiedName;
                }
                else
                {
                    return FullyQualifiedName + "`" + _typeParameters.Count;
                }
            }
        }

        public override string DocumentationTag
        {
            get
            {
                return "T:" + DotNetName;
            }
        }

        public List<IReturnType> BaseTypes
        {
            get
            {
                if (_baseTypes == null)
                {
                    _baseTypes = new List<IReturnType>();
                }
                return _baseTypes;
            }
        }

        public virtual List<IClass> InnerClasses
        {
            get
            {
                if (_innerClasses == null)
                {
                    _innerClasses = new List<IClass>();
                }
                return _innerClasses;
            }
        }

        public virtual List<IField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new List<IField>();
                }
                return _fields;
            }
        }

        public virtual List<IProperty> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new List<IProperty>();
                }
                return _properties;
            }
        }

        public virtual List<IMethod> Methods
        {
            get
            {
                if (_methods == null)
                {
                    _methods = new List<IMethod>();
                }
                return _methods;
            }
        }

        public virtual List<IEvent> Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new List<IEvent>();
                }
                return _events;
            }
        }

        public virtual IList<ITypeParameter> TypeParameters
        {
            get
            {
                if (_typeParameters == null)
                {
                    _typeParameters = new List<ITypeParameter>();
                }
                return _typeParameters;
            }
            set
            {
                _typeParameters = value;
            }
        }

        public virtual int CompareTo(IClass value)
        {
            int cmp;

            if (0 != (cmp = base.CompareTo((IDecoration)value)))
            {
                return cmp;
            }

            if (FullyQualifiedName != null)
            {
                cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
                if (cmp != 0)
                {
                    return cmp;
                }
                return this.TypeParameters.Count - value.TypeParameters.Count;
            }
            return -1;
        }

        int IComparable.CompareTo(object o)
        {
            return CompareTo((IClass)o);
        }

        private List<IClass> _inheritanceTreeCache;

        public IEnumerable<IClass> ClassInheritanceTree
        {
            get
            {
                if (_inheritanceTreeCache != null)
                    return _inheritanceTreeCache;
                List<IClass> visitedList = new List<IClass>();
                Queue<IReturnType> typesToVisit = new Queue<IReturnType>();
                bool enqueuedLastBaseType = false;
                IClass currentClass = this;
                IReturnType nextType;
                do
                {
                    if (currentClass != null)
                    {
                        if (!visitedList.Contains(currentClass))
                        {
                            visitedList.Add(currentClass);
                            foreach (IReturnType type in currentClass.BaseTypes)
                            {
                                typesToVisit.Enqueue(type);
                            }
                        }
                    }
                    if (typesToVisit.Count > 0)
                    {
                        nextType = typesToVisit.Dequeue();
                    }
                    else
                    {
                        nextType = enqueuedLastBaseType ? null : GetBaseTypeByClassType();
                        enqueuedLastBaseType = true;
                    }
                    if (nextType != null)
                    {
                        currentClass = nextType.GetUnderlyingClass();
                    }
                } while (nextType != null);
                if (UseInheritanceCache)
                    _inheritanceTreeCache = visitedList;
                return visitedList;
            }
        }

        protected bool UseInheritanceCache = false;

        protected override bool CanBeSubclass
        {
            get
            {
                return true;
            }
        }

        public IReturnType GetBaseType(int index)
        {
            return BaseTypes[index];
        }

        private IReturnType _cachedBaseType;

        public IReturnType BaseType
        {
            get
            {
                if (_cachedBaseType == null)
                {
                    foreach (IReturnType baseType in this.BaseTypes)
                    {
                        IClass baseClass = baseType.GetUnderlyingClass();
                        if (baseClass != null && baseClass.ClassType == this.ClassType)
                        {
                            _cachedBaseType = baseType;
                            break;
                        }
                    }
                }
                if (_cachedBaseType == null)
                {
                    return GetBaseTypeByClassType();
                }
                else
                {
                    return _cachedBaseType;
                }
            }
        }

        private IReturnType GetBaseTypeByClassType()
        {
            switch (ClassType)
            {
                case ClassType.Class:
                case ClassType.Interface:
                    if (FullyQualifiedName != "System.Object")
                    {
                        return this.ProjectContent.SystemTypes.Object;
                    }
                    break;

                case ClassType.Enum:
                    return this.ProjectContent.SystemTypes.Enum;

                case ClassType.Delegate:
                    return this.ProjectContent.SystemTypes.Delegate;

                case ClassType.Struct:
                    return this.ProjectContent.SystemTypes.ValueType;
            }
            return null;
        }

        public IClass BaseClass
        {
            get
            {
                foreach (IReturnType baseType in this.BaseTypes)
                {
                    IClass baseClass = baseType.GetUnderlyingClass();
                    if (baseClass != null && baseClass.ClassType == this.ClassType)
                        return baseClass;
                }
                switch (ClassType)
                {
                    case ClassType.Class:
                        if (FullyQualifiedName != "System.Object")
                        {
                            return this.ProjectContent.SystemTypes.Object.GetUnderlyingClass();
                        }
                        break;

                    case ClassType.Enum:
                        return this.ProjectContent.SystemTypes.Enum.GetUnderlyingClass();

                    case ClassType.Delegate:
                        return this.ProjectContent.SystemTypes.Delegate.GetUnderlyingClass();

                    case ClassType.Struct:
                        return this.ProjectContent.SystemTypes.ValueType.GetUnderlyingClass();
                }
                return null;
            }
        }

        public bool IsTypeInInheritanceTree(IClass possibleBaseClass)
        {
            if (possibleBaseClass == null)
            {
                return false;
            }
            foreach (IClass baseClass in this.ClassInheritanceTree)
            {
                if (possibleBaseClass.FullyQualifiedName == baseClass.FullyQualifiedName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Searches the member with the specified name. Returns the first member/overload found.
        /// </summary>
        public IMember SearchMember(string memberName, LanguageProperties language)
        {
            if (memberName == null || memberName.Length == 0)
            {
                return null;
            }
            StringComparer cmp = language.NameComparer;
            foreach (IProperty p in Properties)
            {
                if (cmp.Equals(p.Name, memberName))
                {
                    return p;
                }
            }
            foreach (IEvent e in Events)
            {
                if (cmp.Equals(e.Name, memberName))
                {
                    return e;
                }
            }
            foreach (IField f in Fields)
            {
                if (cmp.Equals(f.Name, memberName))
                {
                    return f;
                }
            }
            foreach (IMethod m in Methods)
            {
                if (cmp.Equals(m.Name, memberName))
                {
                    return m;
                }
            }
            return null;
        }

        public IClass GetInnermostClass(int caretLine, int caretColumn)
        {
            foreach (IClass c in InnerClasses)
            {
                if (c != null && c.Region.IsInside(caretLine, caretColumn))
                {
                    return c.GetInnermostClass(caretLine, caretColumn);
                }
            }
            return this;
        }

        public List<IClass> GetAccessibleTypes(IClass callingClass)
        {
            List<IClass> types = new List<IClass>();
            List<IClass> visitedTypes = new List<IClass>();

            IClass currentClass = this;
            do
            {
                if (visitedTypes.Contains(currentClass))
                    break;
                visitedTypes.Add(currentClass);
                bool isClassInInheritanceTree = callingClass.IsTypeInInheritanceTree(currentClass);
                foreach (IClass c in currentClass.InnerClasses)
                {
                    if (c.IsAccessible(callingClass, isClassInInheritanceTree))
                    {
                        types.Add(c);
                    }
                }
                currentClass = currentClass.BaseClass;
            } while (currentClass != null);
            return types;
        }
    }
}