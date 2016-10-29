// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2339 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// The GetClassReturnType is used when the class should be resolved on demand, but the
    /// full name is already known. Example: ReflectionReturnType
    /// </summary>
    public sealed class GetClassReturnType : ProxyReturnType
    {
        private IProjectContent _content;
        private string _fullName;
        private string _shortName;
        private int _typeParameterCount;

        public GetClassReturnType(IProjectContent content, string fullName, int typeParameterCount)
        {
            _content = content;
            _typeParameterCount = typeParameterCount;
            SetFullyQualifiedName(fullName);
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return true;
            }
        }

        public override int TypeParameterCount
        {
            get
            {
                return _typeParameterCount;
            }
        }

        public override bool Equals(object o)
        {
            IReturnType rt = o as IReturnType;
            if (rt != null && rt.IsDefaultReturnType)
                return DefaultReturnType.Equals(this, rt);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _content.GetHashCode() ^ _fullName.GetHashCode() ^ (_typeParameterCount * 5);
        }

        public override IReturnType BaseType
        {
            get
            {
                IClass c = _content.GetClass(_fullName, _typeParameterCount);
                return (c != null) ? c.DefaultReturnType : null;
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                return _fullName;
            }
        }

        public void SetFullyQualifiedName(string fullName)
        {
            if (fullName == null)
                throw new ArgumentNullException("fullName");
            _fullName = fullName;
            int pos = fullName.LastIndexOf('.');
            if (pos < 0)
                _shortName = fullName;
            else
                _shortName = fullName.Substring(pos + 1);
        }

        public override string Name
        {
            get
            {
                return _shortName;
            }
        }

        public override string Namespace
        {
            get
            {
                string tmp = base.Namespace;
                if (tmp == "?")
                {
                    if (_fullName.IndexOf('.') > 0)
                        return _fullName.Substring(0, _fullName.LastIndexOf('.'));
                    else
                        return "";
                }
                return tmp;
            }
        }

        public override string DotNetName
        {
            get
            {
                string tmp = base.DotNetName;
                if (tmp == "?")
                {
                    return _fullName;
                }
                return tmp;
            }
        }

        public override string ToString()
        {
            return String.Format("[GetClassReturnType: {0}]", _fullName);
        }
    }
}
