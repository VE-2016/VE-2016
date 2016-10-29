// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2363 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public abstract class AbstractDecoration : IDecoration
    {
        private ModifierEnum _modifiers = ModifierEnum.None;
        private IList<IAttribute> _attributes = null;

        private IClass _declaringType;
        private object _userData = null;

        public IClass DeclaringType
        {
            get
            {
                return _declaringType;
            }
        }

        public object UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        public ModifierEnum Modifiers
        {
            get
            {
                return _modifiers;
            }
            set
            {
                _modifiers = value;
            }
        }

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();
                }
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }

        private string _documentation;

        public string Documentation
        {
            get
            {
                if (_documentation == null)
                {
                    string documentationTag = this.DocumentationTag;
                    if (documentationTag != null)
                    {
                        IProjectContent pc = null;
                        if (this is IClass)
                        {
                            pc = ((IClass)this).ProjectContent;
                        }
                        else if (_declaringType != null)
                        {
                            pc = _declaringType.ProjectContent;
                        }
                        if (pc != null)
                        {
                            return pc.GetXmlDocumentation(documentationTag);
                        }
                    }
                }
                return _documentation;
            }
            set
            {
                _documentation = value;
            }
        }

        public abstract string DocumentationTag
        {
            get;
        }

        public bool IsAbstract
        {
            get
            {
                return (_modifiers & ModifierEnum.Abstract) == ModifierEnum.Abstract;
            }
        }

        public bool IsSealed
        {
            get
            {
                return (_modifiers & ModifierEnum.Sealed) == ModifierEnum.Sealed;
            }
        }

        public bool IsStatic
        {
            get
            {
                return ((_modifiers & ModifierEnum.Static) == ModifierEnum.Static) || IsConst;
            }
        }

        public bool IsConst
        {
            get
            {
                return (_modifiers & ModifierEnum.Const) == ModifierEnum.Const;
            }
        }

        public bool IsVirtual
        {
            get
            {
                return (_modifiers & ModifierEnum.Virtual) == ModifierEnum.Virtual;
            }
        }

        public bool IsPublic
        {
            get
            {
                return (_modifiers & ModifierEnum.Public) == ModifierEnum.Public;
            }
        }

        public bool IsProtected
        {
            get
            {
                return (_modifiers & ModifierEnum.Protected) == ModifierEnum.Protected;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return (_modifiers & ModifierEnum.Private) == ModifierEnum.Private;
            }
        }

        public bool IsInternal
        {
            get
            {
                return (_modifiers & ModifierEnum.Internal) == ModifierEnum.Internal;
            }
        }

        public bool IsProtectedAndInternal
        {
            get
            {
                return (_modifiers & (ModifierEnum.Internal | ModifierEnum.Protected)) == (ModifierEnum.Internal | ModifierEnum.Protected);
            }
        }

        public bool IsProtectedOrInternal
        {
            get
            {
                return IsProtected || IsInternal;
            }
        }

        public bool IsReadonly
        {
            get
            {
                return (_modifiers & ModifierEnum.Readonly) == ModifierEnum.Readonly;
            }
        }

        public bool IsOverride
        {
            get
            {
                return (_modifiers & ModifierEnum.Override) == ModifierEnum.Override;
            }
        }

        public bool IsOverridable
        {
            get
            {
                return (IsOverride || IsVirtual || IsAbstract) && !IsSealed;
            }
        }

        public bool IsNew
        {
            get
            {
                return (_modifiers & ModifierEnum.New) == ModifierEnum.New;
            }
        }

        public bool IsSynthetic
        {
            get
            {
                return (_modifiers & ModifierEnum.Synthetic) == ModifierEnum.Synthetic;
            }
        }

        public AbstractDecoration(IClass declaringType)
        {
            _declaringType = declaringType;
        }

        private bool IsInnerClass(IClass c, IClass possibleInnerClass)
        {
            foreach (IClass inner in c.InnerClasses)
            {
                if (inner.FullyQualifiedName == possibleInnerClass.FullyQualifiedName)
                {
                    return true;
                }
                if (IsInnerClass(inner, possibleInnerClass))
                {
                    return true;
                }
            }
            return false;
        }

        // TODO: check inner classes for protected members too.
        public bool IsAccessible(IClass callingClass, bool isClassInInheritanceTree)
        {
            if (IsInternal)
            {
                return true;
            }
            if (IsPublic)
            {
                return true;
            }
            if (isClassInInheritanceTree && IsProtected)
            {
                return true;
            }

            return callingClass != null && (DeclaringType.FullyQualifiedName == callingClass.FullyQualifiedName || IsInnerClass(DeclaringType, callingClass));
        }

        public bool MustBeShown(IClass callingClass, bool showStatic, bool isClassInInheritanceTree)
        {
            if (DeclaringType.ClassType == ClassType.Enum)
            {
                return true;
            }
            if (!showStatic && IsStatic || (showStatic && !(IsStatic || IsConst)))
            { // const is automatically static
                return false;
            }
            return IsAccessible(callingClass, isClassInInheritanceTree);
        }

        public virtual int CompareTo(IDecoration value)
        {
            return this.Modifiers - value.Modifiers;
        }

        int IComparable.CompareTo(object value)
        {
            return CompareTo((IDecoration)value);
        }
    }
}