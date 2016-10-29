// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2066 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public interface IEvent : IMember
    {
        IMethod AddMethod
        {
            get;
        }

        IMethod RemoveMethod
        {
            get;
        }

        IMethod RaiseMethod
        {
            get;
        }
    }
}
