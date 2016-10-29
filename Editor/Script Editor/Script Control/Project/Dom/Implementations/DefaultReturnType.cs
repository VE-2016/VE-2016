// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2339 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// DefaultReturnType is a reference to a normal class or a reference to a generic class where
    /// the type parameters are NOT specified.
    /// E.g. "System.Int32", "System.Void", "System.String", "System.Collections.Generic.List"
    /// </summary>
    public class DefaultReturnType : AbstractReturnType
    {
        public static bool Equals(IReturnType rt1, IReturnType rt2)
        {
            return rt1.FullyQualifiedName == rt2.FullyQualifiedName && rt1.TypeParameterCount == rt2.TypeParameterCount;
        }

        private IClass _c;

        public DefaultReturnType(IClass c)
        {
            if (c == null)
                throw new ArgumentNullException("c");
            _c = c;
        }

        public override string ToString()
        {
            return _c.FullyQualifiedName;
        }

        public override int TypeParameterCount
        {
            get
            {
                return _c.TypeParameters.Count;
            }
        }

        public override IClass GetUnderlyingClass()
        {
            return _c;
        }

        private bool _getMembersBusy;

        public override List<IMethod> GetMethods()
        {
            if (_getMembersBusy) return new List<IMethod>();
            _getMembersBusy = true;
            List<IMethod> l = new List<IMethod>();
            l.AddRange(_c.Methods);
            if (_c.ClassType == ClassType.Interface)
            {
                if (_c.BaseTypes.Count == 0)
                {
                    AddMethodsFromBaseType(l, _c.ProjectContent.SystemTypes.Object);
                }
                else
                {
                    foreach (IReturnType baseType in _c.BaseTypes)
                    {
                        AddMethodsFromBaseType(l, baseType);
                    }
                }
            }
            else
            {
                AddMethodsFromBaseType(l, _c.BaseType);
            }
            _getMembersBusy = false;
            return l;
        }

        private void AddMethodsFromBaseType(List<IMethod> l, IReturnType baseType)
        {
            if (baseType != null)
            {
                foreach (IMethod m in baseType.GetMethods())
                {
                    if (m.IsConstructor)
                        continue;

                    bool ok = true;
                    if (m.IsOverridable)
                    {
                        StringComparer comparer = m.DeclaringType.ProjectContent.Language.NameComparer;
                        foreach (IMethod oldMethod in _c.Methods)
                        {
                            if (comparer.Equals(oldMethod.Name, m.Name))
                            {
                                if (m.IsStatic == oldMethod.IsStatic && object.Equals(m.ReturnType, oldMethod.ReturnType))
                                {
                                    if (DiffUtility.Compare(oldMethod.Parameters, m.Parameters) == 0)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (ok)
                        l.Add(m);
                }
            }
        }

        public override List<IProperty> GetProperties()
        {
            if (_getMembersBusy) return new List<IProperty>();
            _getMembersBusy = true;
            List<IProperty> l = new List<IProperty>();
            l.AddRange(_c.Properties);
            if (_c.ClassType == ClassType.Interface)
            {
                foreach (IReturnType baseType in _c.BaseTypes)
                {
                    AddPropertiesFromBaseType(l, baseType);
                }
            }
            else
            {
                AddPropertiesFromBaseType(l, _c.BaseType);
            }
            _getMembersBusy = false;
            return l;
        }

        private void AddPropertiesFromBaseType(List<IProperty> l, IReturnType baseType)
        {
            if (baseType != null)
            {
                foreach (IProperty p in baseType.GetProperties())
                {
                    bool ok = true;
                    if (p.IsOverridable)
                    {
                        StringComparer comparer = p.DeclaringType.ProjectContent.Language.NameComparer;
                        foreach (IProperty oldProperty in _c.Properties)
                        {
                            if (comparer.Equals(oldProperty.Name, p.Name))
                            {
                                if (p.IsStatic == oldProperty.IsStatic && object.Equals(p.ReturnType, oldProperty.ReturnType))
                                {
                                    if (DiffUtility.Compare(oldProperty.Parameters, p.Parameters) == 0)
                                    {
                                        ok = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (ok)
                        l.Add(p);
                }
            }
        }

        public override List<IField> GetFields()
        {
            if (_getMembersBusy) return new List<IField>();
            _getMembersBusy = true;
            List<IField> l = new List<IField>();
            l.AddRange(_c.Fields);
            if (_c.ClassType == ClassType.Interface)
            {
                foreach (IReturnType baseType in _c.BaseTypes)
                {
                    l.AddRange(baseType.GetFields());
                }
            }
            else
            {
                IReturnType baseType = _c.BaseType;
                if (baseType != null)
                {
                    l.AddRange(baseType.GetFields());
                }
            }
            _getMembersBusy = false;
            return l;
        }

        public override List<IEvent> GetEvents()
        {
            if (_getMembersBusy) return new List<IEvent>();
            _getMembersBusy = true;
            List<IEvent> l = new List<IEvent>();
            l.AddRange(_c.Events);
            if (_c.ClassType == ClassType.Interface)
            {
                foreach (IReturnType baseType in _c.BaseTypes)
                {
                    l.AddRange(baseType.GetEvents());
                }
            }
            else
            {
                IReturnType baseType = _c.BaseType;
                if (baseType != null)
                {
                    l.AddRange(baseType.GetEvents());
                }
            }
            _getMembersBusy = false;
            return l;
        }

        public override string FullyQualifiedName
        {
            get
            {
                return _c.FullyQualifiedName;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override string Name
        {
            get
            {
                return _c.Name;
            }
        }

        public override string Namespace
        {
            get
            {
                return _c.Namespace;
            }
        }

        public override string DotNetName
        {
            get
            {
                return _c.DotNetName;
            }
        }
    }
}
