// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2493 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultParameter : IParameter
    {
        public static readonly IList<IParameter> EmptyParameterList = new List<IParameter>().AsReadOnly();

        private string _name;
        private string _documentation;

        //		int nameHashCode      = -1;
        //		int documentationHash = -1;

        private IReturnType _returnType;
        private ParameterModifiers _modifier;
        private DomRegion _region;
        private IList<IAttribute> _attributes;

        protected DefaultParameter(string name)
        {
            Name = name;
        }

        public DefaultParameter(IParameter p)
        {
            _name = p.Name;
            _region = p.Region;
            _modifier = p.Modifiers;
            _returnType = p.ReturnType;
        }

        public DefaultParameter(string name, IReturnType type, DomRegion region) : this(name)
        {
            _returnType = type;
            _region = region;
        }

        public DomRegion Region
        {
            get
            {
                return _region;
            }
        }
        public bool IsOut
        {
            get
            {
                return (_modifier & ParameterModifiers.Out) == ParameterModifiers.Out;
            }
        }
        public bool IsRef
        {
            get
            {
                return (_modifier & ParameterModifiers.Ref) == ParameterModifiers.Ref;
            }
        }
        public bool IsParams
        {
            get
            {
                return (_modifier & ParameterModifiers.Params) == ParameterModifiers.Params;
            }
        }
        public bool IsOptional
        {
            get
            {
                return (_modifier & ParameterModifiers.Optional) == ParameterModifiers.Optional;
            }
        }

        public virtual string Name
        {
            get
            {
                return _name;
                //				return (string)AbstractNamedEntity.fullyQualifiedNames[nameHashCode];
            }
            set
            {
                _name = value;
                //				nameHashCode = value.GetHashCode();
                //				if (AbstractNamedEntity.fullyQualifiedNames[nameHashCode] == null) {
                //					AbstractNamedEntity.fullyQualifiedNames[nameHashCode] = value;
                //				}
            }
        }

        public virtual IReturnType ReturnType
        {
            get
            {
                return _returnType;
            }
            set
            {
                _returnType = value;
            }
        }

        public virtual IList<IAttribute> Attributes
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

        public virtual ParameterModifiers Modifiers
        {
            get
            {
                return _modifier;
            }
            set
            {
                _modifier = value;
            }
        }

        public string Documentation
        {
            get
            {
                return _documentation;
                //				if (documentationHash == -1) {
                //					return String.Empty;
                //				}
                //				return (string)AbstractDecoration.documentationHashtable[documentationHash];
            }
            set
            {
                _documentation = value;
                //				documentationHash = value.GetHashCode();
                //				if (AbstractDecoration.documentationHashtable[documentationHash] == null) {
                //					AbstractDecoration.documentationHashtable[documentationHash] = value;
                //				}
            }
        }

        public static List<IParameter> Clone(IList<IParameter> l)
        {
            List<IParameter> r = new List<IParameter>(l.Count);
            for (int i = 0; i < l.Count; ++i)
            {
                r.Add(new DefaultParameter(l[i]));
            }
            return r;
        }

        public virtual int CompareTo(IParameter value)
        {
            if (value == null) return -1;

            // two parameters are equal if they have the same return type
            // (they may have different names)
            if (object.Equals(ReturnType, value.ReturnType))
            {
                return 0;
            }
            else
            {
                // if the parameters are not equal, use the parameter name to provide the ordering
                int r = string.Compare(this.Name, value.Name);
                if (r != 0)
                    return r;
                else
                    return -1; // but equal names don't make parameters of different return types equal
            }
        }

        int IComparable.CompareTo(object value)
        {
            return CompareTo(value as IParameter);
        }
    }
}
