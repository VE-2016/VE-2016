// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public interface IProperty : IMethodOrProperty
    {
        DomRegion GetterRegion
        {
            get;
        }

        DomRegion SetterRegion
        {
            get;
        }

        bool CanGet
        {
            get;
        }

        bool CanSet
        {
            get;
        }

        bool IsIndexer
        {
            get;
        }
    }
}
