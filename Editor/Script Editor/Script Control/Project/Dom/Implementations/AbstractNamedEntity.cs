// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public abstract class AbstractNamedEntity : AbstractDecoration
    {
        private static char[] s_nameDelimiters = new char[] { '.', '+' };
        private string _fullyQualifiedName = null;
        private string _name = null;
        private string _nspace = null;

        public string FullyQualifiedName
        {
            get
            {
                if (_fullyQualifiedName == null)
                {
                    if (_name != null && _nspace != null)
                    {
                        _fullyQualifiedName = _nspace + '.' + _name;
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                return _fullyQualifiedName;
            }
            set
            {
                if (_fullyQualifiedName == value)
                    return;
                _fullyQualifiedName = value;
                _name = null;
                _nspace = null;
                OnFullyQualifiedNameChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnFullyQualifiedNameChanged(EventArgs e)
        {
        }

        public virtual string DotNetName
        {
            get
            {
                if (this.DeclaringType != null)
                {
                    return this.DeclaringType.DotNetName + "." + this.Name;
                }
                else
                {
                    return FullyQualifiedName;
                }
            }
        }

        public string Name
        {
            get
            {
                if (_name == null && FullyQualifiedName != null)
                {
                    int lastIndex;

                    if (CanBeSubclass)
                    {
                        lastIndex = FullyQualifiedName.LastIndexOfAny(s_nameDelimiters);
                    }
                    else
                    {
                        lastIndex = FullyQualifiedName.LastIndexOf('.');
                    }

                    if (lastIndex < 0)
                    {
                        _name = FullyQualifiedName;
                    }
                    else
                    {
                        _name = FullyQualifiedName.Substring(lastIndex + 1);
                    }
                }
                return _name;
            }
        }

        public string Namespace
        {
            get
            {
                if (_nspace == null && FullyQualifiedName != null)
                {
                    int lastIndex = FullyQualifiedName.LastIndexOf('.');

                    if (lastIndex < 0)
                    {
                        _nspace = String.Empty;
                    }
                    else
                    {
                        _nspace = FullyQualifiedName.Substring(0, lastIndex);
                    }
                }
                return _nspace;
            }
        }

        protected virtual bool CanBeSubclass
        {
            get
            {
                return false;
            }
        }

        public AbstractNamedEntity(IClass declaringType) : base(declaringType)
        {
        }

        public AbstractNamedEntity(IClass declaringType, string name) : base(declaringType)
        {
            System.Diagnostics.Debug.Assert(declaringType != null);
            _name = name;
            _nspace = declaringType.FullyQualifiedName;

            // lazy-computing the fully qualified name for class members saves ~7 MB RAM (when loading the SharpDevelop solution).
            //fullyQualifiedName = nspace + '.' + name;
        }

        public override string ToString()
        {
            return String.Format("[{0}: {1}]", GetType().Name, FullyQualifiedName);
        }
    }
}