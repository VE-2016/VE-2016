// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1943 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
    /// <summary>
    /// Type parameter of a generic class/method.
    /// </summary>
    public class DefaultTypeParameter : ITypeParameter
    {
        public static readonly IList<ITypeParameter> EmptyTypeParameterList = new List<ITypeParameter>().AsReadOnly();

        private string _name;
        private IMethod _method;
        private IClass _targetClass;
        private int _index;
        private List<IReturnType> _constraints = new List<IReturnType>();

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public IMethod Method
        {
            get
            {
                return _method;
            }
        }

        public IClass Class
        {
            get
            {
                return _targetClass;
            }
        }

        public IList<IReturnType> Constraints
        {
            get
            {
                return _constraints;
            }
        }

        public IList<IAttribute> Attributes
        {
            get
            {
                return DefaultAttribute.EmptyAttributeList;
            }
        }

        private bool _hasConstructableConstraint = false;
        private bool _hasReferenceTypeConstraint = false;
        private bool _hasValueTypeConstraint = false;

        /// <summary>
        /// Gets/Sets if the type parameter has the 'new()' constraint.
        /// </summary>
        public bool HasConstructableConstraint
        {
            get { return _hasConstructableConstraint; }
            set { _hasConstructableConstraint = value; }
        }

        /// <summary>
        /// Gets/Sets if the type parameter has the 'class' constraint.
        /// </summary>
        public bool HasReferenceTypeConstraint
        {
            get { return _hasReferenceTypeConstraint; }
            set { _hasReferenceTypeConstraint = value; }
        }

        /// <summary>
        /// Gets/Sets if the type parameter has the 'struct' constraint.
        /// </summary>
        public bool HasValueTypeConstraint
        {
            get { return _hasValueTypeConstraint; }
            set { _hasValueTypeConstraint = value; }
        }

        public DefaultTypeParameter(IMethod method, string name, int index)
        {
            _method = method;
            _targetClass = method.DeclaringType;
            _name = name;
            _index = index;
        }

        public DefaultTypeParameter(IMethod method, Type type)
        {
            _method = method;
            _targetClass = method.DeclaringType;
            _name = type.Name;
            _index = type.GenericParameterPosition;
        }

        public DefaultTypeParameter(IClass targetClass, string name, int index)
        {
            _targetClass = targetClass;
            _name = name;
            _index = index;
        }

        public DefaultTypeParameter(IClass targetClass, Type type)
        {
            _targetClass = targetClass;
            _name = type.Name;
            _index = type.GenericParameterPosition;
        }

        public override bool Equals(object obj)
        {
            DefaultTypeParameter tp = obj as DefaultTypeParameter;
            if (tp == null) return false;
            if (tp._index != _index) return false;
            if (tp._name != _name) return false;
            if (tp._hasConstructableConstraint != _hasConstructableConstraint) return false;
            if (tp._hasReferenceTypeConstraint != _hasReferenceTypeConstraint) return false;
            if (tp._hasValueTypeConstraint != _hasValueTypeConstraint) return false;
            if (tp._method != _method)
            {
                if (tp._method == null || _method == null) return false;
                if (tp._method.FullyQualifiedName == _method.FullyQualifiedName) return false;
            }
            else
            {
                if (tp._targetClass.FullyQualifiedName == _targetClass.FullyQualifiedName) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("[{0}: {1}]", GetType().Name, _name);
        }

        public static DefaultClass GetDummyClassForTypeParameter(ITypeParameter p)
        {
            DefaultClass c = new DefaultClass(p.Class.CompilationUnit, p.Name);
            if (p.Method != null)
            {
                c.Region = new DomRegion(p.Method.Region.BeginLine, p.Method.Region.BeginColumn);
            }
            else
            {
                c.Region = new DomRegion(p.Class.Region.BeginLine, p.Class.Region.BeginColumn);
            }
            c.Modifiers = ModifierEnum.Public;
            if (p.HasValueTypeConstraint)
            {
                c.ClassType = ClassType.Struct;
            }
            else if (p.HasConstructableConstraint)
            {
                c.ClassType = ClassType.Class;
            }
            else
            {
                c.ClassType = ClassType.Interface;
            }
            return c;
        }
    }
}
