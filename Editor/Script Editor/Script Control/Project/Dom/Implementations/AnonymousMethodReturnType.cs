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
    /// The return type of anonymous method expressions or lambda expressions.
    /// </summary>
    public sealed class AnonymousMethodReturnType : ProxyReturnType
    {
        private IReturnType _returnType;
        private IList<IParameter> _parameters = new List<IParameter>();
        private ICompilationUnit _cu;

        public AnonymousMethodReturnType(ICompilationUnit cu)
        {
            _cu = cu;
        }

        /// <summary>
        /// Return type of the anonymous method. Can be null if inferred from context.
        /// </summary>
        public IReturnType MethodReturnType
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
        /// Gets the list of method parameters.
        /// </summary>
        public IList<IParameter> MethodParameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                _parameters = value;
            }
        }

        public override bool IsDefaultReturnType
        {
            get
            {
                return false;
            }
        }

        private volatile DefaultClass _cachedClass;

        public override IClass GetUnderlyingClass()
        {
            if (_cachedClass != null) return _cachedClass;
            DefaultClass c = new DefaultClass(_cu, ClassType.Delegate, ModifierEnum.None, DomRegion.Empty, null);
            c.BaseTypes.Add(_cu.ProjectContent.SystemTypes.Delegate);
            AddDefaultDelegateMethod(c, _returnType ?? _cu.ProjectContent.SystemTypes.Object, _parameters);
            _cachedClass = c;
            return c;
        }

        internal static void AddDefaultDelegateMethod(DefaultClass c, IReturnType returnType, IList<IParameter> parameters)
        {
            ModifierEnum modifiers = ModifierEnum.Public | ModifierEnum.Synthetic;
            DefaultMethod invokeMethod = new DefaultMethod("Invoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
            foreach (IParameter par in parameters)
            {
                invokeMethod.Parameters.Add(par);
            }
            c.Methods.Add(invokeMethod);
            invokeMethod = new DefaultMethod("BeginInvoke", c.ProjectContent.SystemTypes.IAsyncResult, modifiers, c.Region, DomRegion.Empty, c);
            foreach (IParameter par in parameters)
            {
                invokeMethod.Parameters.Add(par);
            }
            invokeMethod.Parameters.Add(new DefaultParameter("callback", c.ProjectContent.SystemTypes.AsyncCallback, DomRegion.Empty));
            invokeMethod.Parameters.Add(new DefaultParameter("object", c.ProjectContent.SystemTypes.Object, DomRegion.Empty));
            c.Methods.Add(invokeMethod);
            invokeMethod = new DefaultMethod("EndInvoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
            invokeMethod.Parameters.Add(new DefaultParameter("result", c.ProjectContent.SystemTypes.IAsyncResult, DomRegion.Empty));
            c.Methods.Add(invokeMethod);
        }

        public override IReturnType BaseType
        {
            get
            {
                return GetUnderlyingClass().DefaultReturnType;
            }
        }

        public override string Name
        {
            get
            {
                return "delegate";
            }
        }

        public override string FullyQualifiedName
        {
            get
            {
                StringBuilder b = new StringBuilder("delegate(");
                bool first = true;
                foreach (IParameter p in _parameters)
                {
                    if (first) first = false; else b.Append(", ");
                    b.Append(p.Name);
                    if (p.ReturnType != null)
                    {
                        b.Append(":");
                        b.Append(p.ReturnType.Name);
                    }
                }
                b.Append(")");
                if (_returnType != null)
                {
                    b.Append(":");
                    b.Append(_returnType.Name);
                }
                return b.ToString();
            }
        }

        public override string Namespace
        {
            get
            {
                return "";
            }
        }

        public override string DotNetName
        {
            get
            {
                return Name;
            }
        }
    }
}