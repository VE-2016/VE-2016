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
    /// <summary>The type of the 'null'/'nothing' literal.</summary>
    public sealed class NullReturnType : AbstractReturnType
    {
        public static readonly NullReturnType Instance = new NullReturnType();

        public override bool Equals(object o)
        {
            return o is NullReturnType;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return false;
            }
        }

        public override IClass GetUnderlyingClass() { return null; }
        public override List<IMethod> GetMethods() { return new List<IMethod>(); }
        public override List<IProperty> GetProperties() { return new List<IProperty>(); }
        public override List<IField> GetFields() { return new List<IField>(); }
        public override List<IEvent> GetEvents() { return new List<IEvent>(); }
    }
}
