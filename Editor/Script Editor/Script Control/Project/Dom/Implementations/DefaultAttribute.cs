// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultAttribute : IAttribute
    {
        public static readonly IList<IAttribute> EmptyAttributeList = new List<IAttribute>().AsReadOnly();

        private string _name;
        private List<AttributeArgument> _positionalArguments;
        private SortedList<string, AttributeArgument> _namedArguments;
        private AttributeTarget _attributeTarget;

        public DefaultAttribute(string name) : this(name, AttributeTarget.None)
        {
        }

        public DefaultAttribute(string name, AttributeTarget attributeTarget)
        {
            _name = name;
            _attributeTarget = attributeTarget;
            _positionalArguments = new List<AttributeArgument>();
            _namedArguments = new SortedList<string, AttributeArgument>();
        }

        public DefaultAttribute(string name, AttributeTarget attributeTarget, List<AttributeArgument> positionalArguments, SortedList<string, AttributeArgument> namedArguments)
        {
            _name = name;
            _attributeTarget = attributeTarget;
            _positionalArguments = positionalArguments;
            _namedArguments = namedArguments;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public AttributeTarget AttributeTarget
        {
            get
            {
                return _attributeTarget;
            }
            set
            {
                _attributeTarget = value;
            }
        }

        public List<AttributeArgument> PositionalArguments
        {
            get
            {
                return _positionalArguments;
            }
        }

        public SortedList<string, AttributeArgument> NamedArguments
        {
            get
            {
                return _namedArguments;
            }
        }

        public virtual int CompareTo(IAttribute value)
        {
            return Name.CompareTo(value.Name);
        }

        int IComparable.CompareTo(object value)
        {
            return CompareTo((IAttribute)value);
        }
    }
}