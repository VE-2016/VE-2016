// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public interface IField : IMember
    {
        /// <summary>Gets if this field is a local variable that has been converted into a field.</summary>
        bool IsLocalVariable { get; }

        /// <summary>Gets if this field is a parameter that has been converted into a field.</summary>
        bool IsParameter { get; }
    }
}
