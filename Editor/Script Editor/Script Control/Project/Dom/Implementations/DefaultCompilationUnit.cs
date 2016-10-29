// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultCompilationUnit : ICompilationUnit
    {
        public static readonly ICompilationUnit DummyCompilationUnit = new DefaultCompilationUnit(DefaultProjectContent.DummyProjectContent);

        private List<IUsing> _usings = new List<IUsing>();
        private List<IClass> _classes = new List<IClass>();
        private List<IAttribute> _attributes = new List<IAttribute>();
        private List<FoldingRegion> _foldingRegions = new List<FoldingRegion>();
        private List<TagComment> _tagComments = new List<TagComment>();

        private bool _errorsDuringCompile = false;
        private object _tag = null;
        private string _fileName = null;
        private IProjectContent _projectContent;

        /// <summary>
        /// Source code file this compilation unit was created from. For compiled are compiler-generated
        /// code, this property returns null.
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public ArrayList Delegates { get; set; }

        public IProjectContent ProjectContent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return _projectContent;
            }
        }

        public bool ErrorsDuringCompile
        {
            get
            {
                return _errorsDuringCompile;
            }
            set
            {
                _errorsDuringCompile = value;
            }
        }

        public object Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public virtual List<IUsing> Usings
        {
            get
            {
                return _usings;
            }
        }

        public virtual List<IAttribute> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public virtual List<IClass> Classes
        {
            get
            {
                return _classes;
            }
        }

        public List<FoldingRegion> FoldingRegions
        {
            get
            {
                return _foldingRegions;
            }
        }

        public virtual List<IComment> MiscComments
        {
            get
            {
                return null;
            }
        }

        public virtual List<IComment> DokuComments
        {
            get
            {
                return null;
            }
        }

        public virtual List<TagComment> TagComments
        {
            get
            {
                return _tagComments;
            }
        }

        public DefaultCompilationUnit(IProjectContent projectContent)
        {
            Debug.Assert(projectContent != null);
            _projectContent = projectContent;
        }

        public IClass GetInnermostClass(int caretLine, int caretColumn)
        {
            foreach (IClass c in Classes)
            {
                if (c != null && c.Region.IsInside(caretLine, caretColumn))
                {
                    return c.GetInnermostClass(caretLine, caretColumn);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all (nested) classes in which the caret currently is exept
        /// the innermost class, returns an empty collection if the caret is in
        /// no class or only in the innermost class.
        /// Zhe most outer class is the last in the collection.
        /// </summary>
        public List<IClass> GetOuterClasses(int caretLine, int caretColumn)
        {
            List<IClass> classes = new List<IClass>();
            IClass innerMostClass = GetInnermostClass(caretLine, caretColumn);
            foreach (IClass c in Classes)
            {
                if (c != null && c.Region.IsInside(caretLine, caretColumn))
                {
                    if (c != innerMostClass)
                    {
                        GetOuterClasses(classes, c, caretLine, caretColumn);
                        if (!classes.Contains(c))
                        {
                            classes.Add(c);
                        }
                    }
                    break;
                }
            }
            return classes;
        }

        private void GetOuterClasses(List<IClass> classes, IClass curClass, int caretLine, int caretColumn)
        {
            if (curClass != null && curClass.InnerClasses.Count > 0)
            {
                IClass innerMostClass = GetInnermostClass(caretLine, caretColumn);
                foreach (IClass c in curClass.InnerClasses)
                {
                    if (c != null && c.Region.IsInside(caretLine, caretColumn))
                    {
                        if (c != innerMostClass)
                        {
                            GetOuterClasses(classes, c, caretLine, caretColumn);
                            if (!classes.Contains(c))
                            {
                                classes.Add(c);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return String.Format("[CompilationUnit: classes = {0}, fileName = {1}]",
                                 _classes.Count,
                                 _fileName);
        }
    }
}