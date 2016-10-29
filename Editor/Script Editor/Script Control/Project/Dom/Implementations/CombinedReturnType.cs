// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// Combines multiple return types for use in contraints.
    /// </summary>
    public sealed class CombinedReturnType : AbstractReturnType
    {
        private IList<IReturnType> _baseTypes;

        private string _fullName;
        private string _name;
        private string _namespace;
        private string _dotnetName;

        public override bool Equals(object obj)
        {
            CombinedReturnType combined = obj as CombinedReturnType;
            if (combined == null) return false;
            if (_baseTypes.Count != combined._baseTypes.Count) return false;
            for (int i = 0; i < _baseTypes.Count; i++)
            {
                if (!_baseTypes[i].Equals(combined._baseTypes[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int res = 0;
                foreach (IReturnType rt in _baseTypes)
                {
                    res *= 27;
                    res += rt.GetHashCode();
                }
                return res;
            }
        }

        public CombinedReturnType(IList<IReturnType> baseTypes, string fullName, string name, string @namespace, string dotnetName)
        {
            _baseTypes = baseTypes;
            _fullName = fullName;
            _name = name;
            _namespace = @namespace;
            _dotnetName = dotnetName;
        }

        public IList<IReturnType> BaseTypes
        {
            get
            {
                return _baseTypes;
            }
        }

        private List<T> Combine<T>(Converter<IReturnType, List<T>> conv) where T : IMember
        {
            int count = _baseTypes.Count;
            if (count == 0)
                return null;
            List<T> list = null;
            foreach (IReturnType baseType in _baseTypes)
            {
                List<T> newList = conv(baseType);
                if (newList == null)
                    continue;
                if (list == null)
                {
                    list = newList;
                }
                else
                {
                    foreach (T element in newList)
                    {
                        bool found = false;
                        foreach (T t in list)
                        {
                            if (t.CompareTo(element) == 0)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            list.Add(element);
                        }
                    }
                }
            }
            return list;
        }

        public override List<IMethod> GetMethods()
        {
            return Combine<IMethod>(delegate (IReturnType type) { return type.GetMethods(); });
        }

        public override List<IProperty> GetProperties()
        {
            return Combine<IProperty>(delegate (IReturnType type) { return type.GetProperties(); });
        }

        public override List<IField> GetFields()
        {
            return Combine<IField>(delegate (IReturnType type) { return type.GetFields(); });
        }

        public override List<IEvent> GetEvents()
        {
            return Combine<IEvent>(delegate (IReturnType type) { return type.GetEvents(); });
        }

        public override string FullyQualifiedName
        {
            get
            {
                return _fullName;
            }
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override string Namespace
        {
            get
            {
                return _namespace;
            }
        }

        public override string DotNetName
        {
            get
            {
                return _dotnetName;
            }
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return false;
            }
        }

        public override int TypeParameterCount
        {
            get
            {
                return 0;
            }
        }

        public override IClass GetUnderlyingClass()
        {
            return null;
        }
    }
}