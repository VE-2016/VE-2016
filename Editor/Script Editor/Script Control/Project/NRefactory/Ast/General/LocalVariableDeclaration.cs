// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public class LocalVariableDeclaration : Statement
    {
        private TypeReference _typeReference;
        private Modifiers _modifier = Modifiers.None;
        private List<VariableDeclaration> _variables = new List<VariableDeclaration>(1);

        public TypeReference TypeReference
        {
            get
            {
                return _typeReference;
            }
            set
            {
                _typeReference = TypeReference.CheckNull(value);
            }
        }

        public Modifiers Modifier
        {
            get
            {
                return _modifier;
            }
            set
            {
                _modifier = value;
            }
        }

        public List<VariableDeclaration> Variables
        {
            get
            {
                return _variables;
            }
        }

        public TypeReference GetTypeForVariable(int variableIndex)
        {
            if (!_typeReference.IsNull)
            {
                return _typeReference;
            }

            for (int i = variableIndex; i < Variables.Count; ++i)
            {
                if (!((VariableDeclaration)Variables[i]).TypeReference.IsNull)
                {
                    return ((VariableDeclaration)Variables[i]).TypeReference;
                }
            }
            return null;
        }

        public LocalVariableDeclaration(VariableDeclaration declaration) : this(TypeReference.Null)
        {
            Variables.Add(declaration);
        }

        public LocalVariableDeclaration(TypeReference typeReference)
        {
            this.TypeReference = typeReference;
        }

        public LocalVariableDeclaration(TypeReference typeReference, Modifiers modifier)
        {
            this.TypeReference = typeReference;
            _modifier = modifier;
        }

        public LocalVariableDeclaration(Modifiers modifier)
        {
            _typeReference = TypeReference.Null;
            _modifier = modifier;
        }

        public VariableDeclaration GetVariableDeclaration(string variableName)
        {
            foreach (VariableDeclaration variableDeclaration in _variables)
            {
                if (variableDeclaration.Name == variableName)
                {
                    return variableDeclaration;
                }
            }
            return null;
        }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return visitor.VisitLocalVariableDeclaration(this, data);
        }

        public override string ToString()
        {
            return String.Format("[LocalVariableDeclaration: Type={0}, Modifier ={1} Variables={2}]",
                                 _typeReference,
                                 _modifier,
                                 GetCollectionString(_variables));
        }
    }
}
