// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2384 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// The SearchClassReturnType is used when only a part of the class name is known and the
    /// type can only be resolved on demand (the ConvertVisitor uses SearchClassReturnType's).
    /// </summary>
    public sealed class SearchClassReturnType : ProxyReturnType
    {
        private IClass _declaringClass;
        private IProjectContent _pc;
        private int _caretLine;
        private int _caretColumn;
        private string _name;
        private string _shortName;
        private int _typeParameterCount;

        public SearchClassReturnType(IProjectContent projectContent, IClass declaringClass, int caretLine, int caretColumn, string name, int typeParameterCount)
        {
            if (declaringClass == null)
                throw new ArgumentNullException("declaringClass");
            _declaringClass = declaringClass;
            _pc = projectContent;
            _caretLine = caretLine;
            _caretColumn = caretColumn;
            _typeParameterCount = typeParameterCount;
            _name = name;
            int pos = name.LastIndexOf('.');
            if (pos < 0)
                _shortName = name;
            else
                _shortName = name.Substring(pos + 1);
        }

        public override int TypeParameterCount
        {
            get
            {
                return _typeParameterCount;
            }
        }

        public override bool Equals(object o)
        {
            IReturnType rt2 = o as IReturnType;
            if (rt2 != null && rt2.IsDefaultReturnType)
                return DefaultReturnType.Equals(this, rt2);
            else
                return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return _declaringClass.GetHashCode() ^ _name.GetHashCode()
                    ^ (_typeParameterCount << 16 + _caretLine << 8 + _caretColumn);
            }
        }

        // we need to use a static Dictionary as cache to provide a easy was to clear all cached
        // BaseTypes.
        // When the cached BaseTypes could not be cleared as soon as the parse information is updated
        // (in contrast to a check if the parse information was updated when the base type is needed
        // the next time), we can get a memory leak:
        // The cached type of a property in Class1 is Class2. Then Class2 is updated, but the property
        // in Class1 is not needed again -> the reference causes the GC to keep the old version
        // of Class2 in memory.
        // The solution is this static cache which is cleared when some parse information updates.
        // That way, there can never be any reference to an out-of-date class.
        private static Dictionary<SearchClassReturnType, IReturnType> s_cache = new Dictionary<SearchClassReturnType, IReturnType>(new ReferenceComparer());

        private class ReferenceComparer : IEqualityComparer<SearchClassReturnType>
        {
            public bool Equals(SearchClassReturnType x, SearchClassReturnType y)
            {
                return x == y; // don't use x.Equals(y) - Equals might cause a FullyQualifiedName lookup on its own
            }

            public int GetHashCode(SearchClassReturnType obj)
            {
                return obj.GetHashCode();
            }
        }

        /// <summary>
        /// Clear the static searchclass cache. You should call this method
        /// whenever the DOM changes.
        /// </summary>
        /// <remarks>
        /// automatically called by DefaultProjectContent.UpdateCompilationUnit
        /// and DefaultProjectContent.OnReferencedContentsChanged.
        /// </remarks>
        internal static void ClearCache()
        {
            lock (s_cache)
            {
                s_cache.Clear();
            }
        }

        private bool _isSearching;

        public override IReturnType BaseType
        {
            get
            {
                if (_isSearching)
                    return null;
                IReturnType type;
                lock (s_cache)
                {
                    if (s_cache.TryGetValue(this, out type))
                        return type;
                }
                try
                {
                    _isSearching = true;
                    type = _pc.SearchType(new SearchTypeRequest(_name, _typeParameterCount, _declaringClass, _caretLine, _caretColumn)).Result;
                    lock (s_cache)
                    {
                        s_cache[this] = type;
                    }
                    return type;
                }
                finally
                {
                    _isSearching = false;
                }
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                string tmp = base.FullyQualifiedName;
                if (tmp == "?")
                {
                    return _name;
                }
                return tmp;
            }
        }

        public override string Name
        {
            get
            {
                return _shortName;
            }
        }

        public override string DotNetName
        {
            get
            {
                string tmp = base.DotNetName;
                if (tmp == "?")
                {
                    return _name;
                }
                return tmp;
            }
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return true;
            }
        }

        public override string ToString()
        {
            return String.Format("[SearchClassReturnType: {0}]", _name);
        }
    }
}
