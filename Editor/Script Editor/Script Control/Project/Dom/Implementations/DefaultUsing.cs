// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2066 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultUsing : IUsing
    {
        private DomRegion _region;
        private IProjectContent _projectContent;

        public DefaultUsing(IProjectContent projectContent)
        {
            _projectContent = projectContent;
        }

        public DefaultUsing(IProjectContent projectContent, DomRegion region) : this(projectContent)
        {
            _region = region;
        }

        private List<string> _usings = new List<string>();
        private SortedList<string, IReturnType> _aliases = null;

        public DomRegion Region
        {
            get
            {
                return _region;
            }
        }

        public List<string> Usings
        {
            get
            {
                return _usings;
            }
        }

        public SortedList<string, IReturnType> Aliases
        {
            get
            {
                return _aliases;
            }
        }

        public bool HasAliases
        {
            get
            {
                return _aliases != null && _aliases.Count > 0;
            }
        }

        public void AddAlias(string alias, IReturnType type)
        {
            if (_aliases == null) _aliases = new SortedList<string, IReturnType>();
            _aliases.Add(alias, type);
        }

        public string SearchNamespace(string partialNamespaceName)
        {
            if (HasAliases)
            {
                foreach (KeyValuePair<string, IReturnType> entry in _aliases)
                {
                    if (!entry.Value.IsDefaultReturnType)
                        continue;
                    string aliasString = entry.Key;
                    string nsName;
                    if (_projectContent.Language.NameComparer.Equals(partialNamespaceName, aliasString))
                    {
                        nsName = entry.Value.FullyQualifiedName;
                        if (_projectContent.NamespaceExists(nsName))
                            return nsName;
                    }
                    if (partialNamespaceName.Length > aliasString.Length)
                    {
                        if (_projectContent.Language.NameComparer.Equals(partialNamespaceName.Substring(0, aliasString.Length + 1), aliasString + "."))
                        {
                            nsName = String.Concat(entry.Value.FullyQualifiedName, partialNamespaceName.Remove(0, aliasString.Length));
                            if (_projectContent.NamespaceExists(nsName))
                            {
                                return nsName;
                            }
                        }
                    }
                }
            }
            if (_projectContent.Language.ImportNamespaces)
            {
                foreach (string str in _usings)
                {
                    string possibleNamespace = String.Concat(str, ".", partialNamespaceName);
                    if (_projectContent.NamespaceExists(possibleNamespace))
                        return possibleNamespace;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a collection of possible types that could be meant when using this Import
        /// to search the type.
        /// Types with the incorrect type parameter count might be returned, but for each
        /// same using entry or alias entry at most one (the best matching) type should be returned.
        /// </summary>
        public IEnumerable<IReturnType> SearchType(string partialTypeName, int typeParameterCount)
        {
            if (HasAliases)
            {
                foreach (KeyValuePair<string, IReturnType> entry in _aliases)
                {
                    string aliasString = entry.Key;
                    if (_projectContent.Language.NameComparer.Equals(partialTypeName, aliasString))
                    {
                        if (entry.Value.IsDefaultReturnType && entry.Value.GetUnderlyingClass() == null)
                            continue; // type not found, maybe entry was a namespace
                        yield return entry.Value;
                    }
                    if (partialTypeName.Length > aliasString.Length)
                    {
                        if (_projectContent.Language.NameComparer.Equals(partialTypeName.Substring(0, aliasString.Length + 1), aliasString + "."))
                        {
                            string className = entry.Value.FullyQualifiedName + partialTypeName.Remove(0, aliasString.Length);
                            IClass c = _projectContent.GetClass(className, typeParameterCount);
                            if (c != null)
                            {
                                yield return c.DefaultReturnType;
                            }
                        }
                    }
                }
            }
            if (_projectContent.Language.ImportNamespaces)
            {
                foreach (string str in _usings)
                {
                    IClass c = _projectContent.GetClass(str + "." + partialTypeName, typeParameterCount);
                    if (c != null)
                    {
                        yield return c.DefaultReturnType;
                    }
                }
            }
            else
            {
                int pos = partialTypeName.IndexOf('.');
                string className, subClassName;
                if (pos < 0)
                {
                    className = partialTypeName;
                    subClassName = null;
                }
                else
                {
                    className = partialTypeName.Substring(0, pos);
                    subClassName = partialTypeName.Substring(pos + 1);
                }
                foreach (string str in _usings)
                {
                    IClass c = _projectContent.GetClass(str + "." + className, typeParameterCount);
                    if (c != null)
                    {
                        c = _projectContent.GetClass(str + "." + partialTypeName, typeParameterCount);
                        if (c != null)
                        {
                            yield return c.DefaultReturnType;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("[DefaultUsing: ");
            foreach (string str in _usings)
            {
                builder.Append(str);
                builder.Append(", ");
            }
            if (HasAliases)
            {
                foreach (KeyValuePair<string, IReturnType> p in _aliases)
                {
                    builder.Append(p.Key);
                    builder.Append("=");
                    builder.Append(p.Value.ToString());
                    builder.Append(", ");
                }
            }
            builder.Length -= 2; // remove last ", "
            builder.Append("]");
            return builder.ToString();
        }
    }
}
