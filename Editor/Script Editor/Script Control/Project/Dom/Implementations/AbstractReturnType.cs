﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2339 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// Abstract return type for return types that are not a <see cref="ProxyReturnType"/>.
    /// </summary>
    public abstract class AbstractReturnType : IReturnType
    {
        public abstract IClass GetUnderlyingClass();

        public abstract List<IMethod> GetMethods();

        public abstract List<IProperty> GetProperties();

        public abstract List<IField> GetFields();

        public abstract List<IEvent> GetEvents();

        public virtual int TypeParameterCount
        {
            get
            {
                return 0;
            }
        }

        public override bool Equals(object o)
        {
            IReturnType rt = o as IReturnType;
            if (rt == null) return false;
            return rt.IsDefaultReturnType && DefaultReturnType.Equals(this, rt);
        }

        public override int GetHashCode()
        {
            return _fullyQualifiedName.GetHashCode();
        }

        private string _fullyQualifiedName = null;

        public virtual string FullyQualifiedName
        {
            get
            {
                if (_fullyQualifiedName == null)
                {
                    return String.Empty;
                }
                return _fullyQualifiedName;
            }
            set
            {
                _fullyQualifiedName = value;
            }
        }

        public virtual string Name
        {
            get
            {
                if (FullyQualifiedName == null)
                {
                    return null;
                }
                int index = FullyQualifiedName.LastIndexOf('.');
                return index < 0 ? FullyQualifiedName : FullyQualifiedName.Substring(index + 1);
            }
        }

        public virtual string Namespace
        {
            get
            {
                if (FullyQualifiedName == null)
                {
                    return null;
                }
                int index = FullyQualifiedName.LastIndexOf('.');
                return index < 0 ? String.Empty : FullyQualifiedName.Substring(0, index);
            }
        }

        public virtual string DotNetName
        {
            get
            {
                return FullyQualifiedName;
            }
        }

        public virtual bool IsDefaultReturnType
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsArrayReturnType
        {
            get
            {
                return false;
            }
        }

        public virtual ArrayReturnType CastToArrayReturnType()
        {
            throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
        }

        public virtual bool IsGenericReturnType
        {
            get
            {
                return false;
            }
        }

        public virtual GenericReturnType CastToGenericReturnType()
        {
            throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
        }

        public virtual bool IsConstructedReturnType
        {
            get
            {
                return false;
            }
        }

        public virtual ConstructedReturnType CastToConstructedReturnType()
        {
            throw new InvalidCastException("Cannot cast " + ToString() + " to expected type.");
        }
    }
}