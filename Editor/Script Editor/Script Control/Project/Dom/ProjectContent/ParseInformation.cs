// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public class ParseInformation
    {
        private ICompilationUnit _validCompilationUnit;
        private ICompilationUnit _dirtyCompilationUnit;

        public ICompilationUnit ValidCompilationUnit
        {
            get
            {
                return _validCompilationUnit;
            }
            set
            {
                _validCompilationUnit = value;
            }
        }

        public ICompilationUnit DirtyCompilationUnit
        {
            get
            {
                return _dirtyCompilationUnit;
            }
            set
            {
                _dirtyCompilationUnit = value;
            }
        }

        public ICompilationUnit BestCompilationUnit
        {
            get
            {
                return _validCompilationUnit == null ? _dirtyCompilationUnit : _validCompilationUnit;
            }
        }

        public ICompilationUnit MostRecentCompilationUnit
        {
            get
            {
                return _dirtyCompilationUnit == null ? _validCompilationUnit : _dirtyCompilationUnit;
            }
        }
    }
}
