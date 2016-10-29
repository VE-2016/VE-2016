// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2066 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// Class that stores a source code context and can resolve type names
    /// in that context.
    /// </summary>
    public sealed class ClassFinder
    {
        private int _caretLine,_caretColumn;
        private ICompilationUnit _cu;
        private IClass _callingClass;
        private IProjectContent _projectContent;

        public IClass CallingClass
        {
            get
            {
                return _callingClass;
            }
        }

        public IProjectContent ProjectContent
        {
            get
            {
                return _projectContent;
            }
        }

        public LanguageProperties Language
        {
            get
            {
                return _projectContent.Language;
            }
        }

        public ClassFinder(string fileName, string fileContent, int offset)
        {
            _caretLine = 0;
            _caretColumn = 0;
            for (int i = 0; i < offset; i++)
            {
                if (fileContent[i] == '\n')
                {
                    _caretLine++;
                    _caretColumn = 0;
                }
                else
                {
                    _caretColumn++;
                }
            }
            Init(fileName);
        }

        public ClassFinder(string fileName, int caretLineNumber, int caretColumn)
        {
            _caretLine = caretLineNumber;
            _caretColumn = caretColumn;

            Init(fileName);
        }

        public ClassFinder(IMember classMember)
            : this(classMember.DeclaringType, classMember.Region.BeginLine, classMember.Region.BeginColumn)
        {
        }

        public ClassFinder(IClass callingClass, int caretLine, int caretColumn)
        {
            _caretLine = caretLine;
            _caretColumn = caretColumn;
            _callingClass = callingClass;
            _cu = callingClass.CompilationUnit;
            _projectContent = _cu.ProjectContent;
            if (_projectContent == null)
                throw new ArgumentException("callingClass must have a project content!");
        }

        // currently callingMember is not required
        public ClassFinder(IClass callingClass, IMember callingMember, int caretLine, int caretColumn)
            : this(callingClass, caretLine, caretColumn)
        {
        }

        private void Init(string fileName)
        {
            ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
            if (parseInfo != null)
            {
                _cu = parseInfo.MostRecentCompilationUnit;
            }

            if (_cu != null)
            {
                _callingClass = _cu.GetInnermostClass(_caretLine, _caretColumn);
                _projectContent = _cu.ProjectContent;
            }
            else
            {
                _projectContent = HostCallback.GetCurrentProjectContent();
            }
            if (_projectContent == null)
                throw new ArgumentException("projectContent not found!");
        }

        public IClass GetClass(string fullName, int typeParameterCount)
        {
            return _projectContent.GetClass(fullName, typeParameterCount);
        }

        public IReturnType SearchType(string name, int typeParameterCount)
        {
            return _projectContent.SearchType(new SearchTypeRequest(name, typeParameterCount, _callingClass, _cu, _caretLine, _caretColumn)).Result;
        }

        public string SearchNamespace(string name)
        {
            return _projectContent.SearchNamespace(name, _callingClass, _cu, _caretLine, _caretColumn);
        }
    }
}