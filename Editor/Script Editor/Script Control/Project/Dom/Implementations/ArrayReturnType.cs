// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// The ArrayReturnType wraps around another type, converting it into an array
    /// with the specified number of dimensions.
    /// The element type is only used as return type for the indexer; all methods and fields
    /// are retrieved from System.Array.
    /// </summary>
    public sealed class ArrayReturnType : ProxyReturnType
    {
        private IReturnType _elementType;
        private int _dimensions;
        private IProjectContent _pc;

        internal IProjectContent ProjectContent
        {
            get
            {
                return _pc;
            }
        }

        public ArrayReturnType(IProjectContent pc, IReturnType elementType, int dimensions)
        {
            if (pc == null)
                throw new ArgumentNullException("pc");
            if (dimensions <= 0)
                throw new ArgumentOutOfRangeException("dimensions", dimensions, "dimensions must be positive");
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            _pc = pc;
            _elementType = elementType;
            _dimensions = dimensions;
        }

        public override bool Equals(object o)
        {
            IReturnType rt = o as IReturnType;
            if (rt == null || !rt.IsArrayReturnType) return false;
            ArrayReturnType art = rt.CastToArrayReturnType();
            if (art.ArrayDimensions != _dimensions) return false;
            return _elementType.Equals(art.ArrayElementType);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 2 * _elementType.GetHashCode() + 27 * _dimensions;
            }
        }

        public IReturnType ArrayElementType
        {
            get
            {
                return _elementType;
            }
        }

        public int ArrayDimensions
        {
            get
            {
                return _dimensions;
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                return _elementType.FullyQualifiedName;
            }
        }

        public override string Name
        {
            get
            {
                return _elementType.Name;
            }
        }

        public override string DotNetName
        {
            get
            {
                return AppendArrayString(_elementType.DotNetName);
            }
        }

        public override IReturnType BaseType
        {
            get
            {
                return _pc.SystemTypes.Array;
            }
        }

        /// <summary>
        /// Indexer used exclusively for array return types
        /// </summary>
        public class ArrayIndexer : DefaultProperty
        {
            public ArrayIndexer(IReturnType elementType, IClass systemArray)
                : base("Indexer", elementType, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, systemArray)
            {
                IsIndexer = true;
            }
        }

        public override List<IProperty> GetProperties()
        {
            List<IProperty> l = base.GetProperties();
            ArrayIndexer property = new ArrayIndexer(_elementType, this.BaseType.GetUnderlyingClass());
            IReturnType int32 = _pc.SystemTypes.Int32;
            for (int i = 0; i < _dimensions; ++i)
            {
                property.Parameters.Add(new DefaultParameter("index", int32, DomRegion.Empty));
            }
            l.Add(property);
            return l;
        }

        /// <summary>
        /// Appends the array characters ([,,,]) to the string <paramref name="a"/>.
        /// </summary>
        private string AppendArrayString(string a)
        {
            StringBuilder b = new StringBuilder(a, a.Length + 1 + _dimensions);
            b.Append('[');
            for (int i = 1; i < _dimensions; ++i)
            {
                b.Append(',');
            }
            b.Append(']');
            return b.ToString();
        }

        public override string ToString()
        {
            return String.Format("[ArrayReturnType: {0}, dimensions={1}]", _elementType, AppendArrayString(""));
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return false;
            }
        }

        public override bool IsArrayReturnType
        {
            get
            {
                return true;
            }
        }

        public override ArrayReturnType CastToArrayReturnType()
        {
            return this;
        }

        public override bool IsConstructedReturnType
        {
            get
            {
                return false;
            }
        }

        public override bool IsGenericReturnType
        {
            get
            {
                return false;
            }
        }
    }
}