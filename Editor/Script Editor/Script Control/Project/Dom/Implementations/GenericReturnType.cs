// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1943 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// GenericReturnType is a reference to a type parameter.
    /// </summary>
    public sealed class GenericReturnType : ProxyReturnType
    {
        private ITypeParameter _typeParameter;

        public ITypeParameter TypeParameter
        {
            get
            {
                return _typeParameter;
            }
        }

        public override bool Equals(object o)
        {
            IReturnType rt = o as IReturnType;
            if (rt == null || !rt.IsGenericReturnType)
                return false;
            return _typeParameter.Equals(rt.CastToGenericReturnType()._typeParameter);
        }

        public override int GetHashCode()
        {
            return _typeParameter.GetHashCode();
        }

        public GenericReturnType(ITypeParameter typeParameter)
        {
            if (typeParameter == null)
                throw new ArgumentNullException("typeParameter");
            _typeParameter = typeParameter;
        }

        public override string FullyQualifiedName
        {
            get
            {
                return _typeParameter.Name;
            }
        }

        public override string Name
        {
            get
            {
                return _typeParameter.Name;
            }
        }

        public override string Namespace
        {
            get
            {
                return "";
            }
        }

        public override string DotNetName
        {
            get
            {
                if (_typeParameter.Method != null)
                    return "``" + _typeParameter.Index;
                else
                    return "`" + _typeParameter.Index;
            }
        }

        public override IClass GetUnderlyingClass()
        {
            return null;
        }

        public override IReturnType BaseType
        {
            get
            {
                int count = _typeParameter.Constraints.Count;
                if (count == 0)
                    return _typeParameter.Class.ProjectContent.SystemTypes.Object;
                if (count == 1)
                    return _typeParameter.Constraints[0];
                return new CombinedReturnType(_typeParameter.Constraints,
                                              FullyQualifiedName,
                                              Name, Namespace,
                                              DotNetName);
            }
        }

        // remove static methods (T.ReferenceEquals() is not possible)
        public override List<IMethod> GetMethods()
        {
            List<IMethod> list = base.GetMethods();
            if (list != null)
            {
                list.RemoveAll(delegate (IMethod m) { return m.IsStatic || m.IsConstructor; });
                if (_typeParameter.HasConstructableConstraint || _typeParameter.HasValueTypeConstraint)
                {
                    list.Add(new Constructor(ModifierEnum.Public, this,
                                             DefaultTypeParameter.GetDummyClassForTypeParameter(_typeParameter)));
                }
            }
            return list;
        }

        public override string ToString()
        {
            return String.Format("[GenericReturnType: {0}]", _typeParameter);
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return false;
            }
        }

        public override bool IsArrayReturnType
        {
            get
            {
                return false;
            }
        }

        public override bool IsConstructedReturnType
        {
            get
            {
                return false;
            }
        }

        public override bool IsGenericReturnType
        {
            get
            {
                return true;
            }
        }

        public override AIMS.Libraries.Scripting.Dom.GenericReturnType CastToGenericReturnType()
        {
            return this;
        }
    }
}
