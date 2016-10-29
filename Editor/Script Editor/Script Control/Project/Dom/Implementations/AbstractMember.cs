// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2066 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public abstract class AbstractMember : AbstractNamedEntity, IMember
    {
        private IReturnType _returnType;
        private DomRegion _region;
        private DomRegion _bodyRegion;
        private List<ExplicitInterfaceImplementation> _interfaceImplementations;
        private IReturnType _declaringTypeReference;

        public virtual DomRegion Region
        {
            get
            {
                return _region;
            }
            set
            {
                _region = value;
            }
        }

        public virtual DomRegion BodyRegion
        {
            get
            {
                return _bodyRegion;
            }
            protected set
            {
                _bodyRegion = value;
            }
        }

        public virtual IReturnType ReturnType
        {
            get
            {
                return _returnType;
            }
            set
            {
                _returnType = value;
            }
        }

        /// <summary>
        /// Gets the declaring type reference (declaring type incl. type arguments)
        /// </summary>
        public virtual IReturnType DeclaringTypeReference
        {
            get
            {
                return _declaringTypeReference ?? this.DeclaringType.DefaultReturnType;
            }
            set
            {
                _declaringTypeReference = value;
            }
        }

        public IList<ExplicitInterfaceImplementation> InterfaceImplementations
        {
            get
            {
                return _interfaceImplementations ?? (_interfaceImplementations = new List<ExplicitInterfaceImplementation>());
            }
        }

        public AbstractMember(IClass declaringType, string name) : base(declaringType, name)
        {
        }

        public abstract IMember Clone();

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}