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
    public sealed class ExplicitInterfaceImplementation : IEquatable<ExplicitInterfaceImplementation>
    {
        private readonly IReturnType _interfaceReference;
        private readonly string _memberName;

        public ExplicitInterfaceImplementation(IReturnType interfaceReference, string memberName)
        {
            _interfaceReference = interfaceReference;
            _memberName = memberName;
        }

        public IReturnType InterfaceReference
        {
            get { return _interfaceReference; }
        }

        public string MemberName
        {
            get { return _memberName; }
        }

        public ExplicitInterfaceImplementation Clone()
        {
            return this; // object is immutable, no Clone() required
        }

        public override int GetHashCode()
        {
            return _interfaceReference.GetHashCode() ^ _memberName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExplicitInterfaceImplementation);
        }

        public bool Equals(ExplicitInterfaceImplementation other)
        {
            if (other == null)
                return false;
            else
                return _interfaceReference == other._interfaceReference && _memberName == other._memberName;
        }
    }
}
