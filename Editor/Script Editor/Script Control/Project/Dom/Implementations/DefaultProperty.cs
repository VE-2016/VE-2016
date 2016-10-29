// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2363 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultProperty : AbstractMember, IProperty
    {
        private DomRegion _getterRegion = DomRegion.Empty;
        private DomRegion _setterRegion = DomRegion.Empty;

        private IList<IParameter> _parameters = null;
        internal byte accessFlags;
        private const byte indexerFlag = 0x01;
        private const byte getterFlag = 0x02;
        private const byte setterFlag = 0x04;
        private const byte extensionFlag = 0x08;

        public bool IsIndexer
        {
            get { return (accessFlags & indexerFlag) == indexerFlag; }
            set { if (value) accessFlags |= indexerFlag; else accessFlags &= 255 - indexerFlag; }
        }

        public bool CanGet
        {
            get { return (accessFlags & getterFlag) == getterFlag; }
            set { if (value) accessFlags |= getterFlag; else accessFlags &= 255 - getterFlag; }
        }

        public bool CanSet
        {
            get { return (accessFlags & setterFlag) == setterFlag; }
            set { if (value) accessFlags |= setterFlag; else accessFlags &= 255 - setterFlag; }
        }

        public bool IsExtensionMethod
        {
            get { return (accessFlags & extensionFlag) == extensionFlag; }
            set { if (value) accessFlags |= extensionFlag; else accessFlags &= 255 - extensionFlag; }
        }

        public override string DocumentationTag
        {
            get
            {
                return "P:" + this.DotNetName;
            }
        }

        public override IMember Clone()
        {
            DefaultProperty p = new DefaultProperty(Name, ReturnType, Modifiers, Region, BodyRegion, DeclaringType);
            p._parameters = DefaultParameter.Clone(this.Parameters);
            p.accessFlags = this.accessFlags;
            foreach (ExplicitInterfaceImplementation eii in InterfaceImplementations)
            {
                p.InterfaceImplementations.Add(eii.Clone());
            }
            return p;
        }

        public virtual IList<IParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<IParameter>();
                }
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        public DomRegion GetterRegion
        {
            get
            {
                return _getterRegion;
            }
            set
            {
                _getterRegion = value;
            }
        }

        public DomRegion SetterRegion
        {
            get
            {
                return _setterRegion;
            }
            set
            {
                _setterRegion = value;
            }
        }

        public DefaultProperty(IClass declaringType, string name) : base(declaringType, name)
        {
        }

        public DefaultProperty(string name, IReturnType type, ModifierEnum m, DomRegion region, DomRegion bodyRegion, IClass declaringType) : base(declaringType, name)
        {
            this.ReturnType = type;
            this.Region = region;
            this.BodyRegion = bodyRegion;
            Modifiers = m;
        }

        public virtual int CompareTo(IProperty value)
        {
            int cmp;

            if (FullyQualifiedName != null)
            {
                cmp = FullyQualifiedName.CompareTo(value.FullyQualifiedName);
                if (cmp != 0)
                {
                    return cmp;
                }
            }

            return DiffUtility.Compare(Parameters, value.Parameters);
        }

        int IComparable.CompareTo(object value)
        {
            return CompareTo((IProperty)value);
        }
    }
}
