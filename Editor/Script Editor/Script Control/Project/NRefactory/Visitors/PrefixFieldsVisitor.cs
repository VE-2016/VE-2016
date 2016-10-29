// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1609 $</version>
// </file>

using System;
using System.Collections.Generic;
using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
    /// <summary>
    /// Prefixes the names of the specified fields with the prefix and replaces the use.
    /// </summary>
    public class PrefixFieldsVisitor : AbstractAstVisitor
    {
        private List<VariableDeclaration> _fields;
        private List<string> _curBlock = new List<string>();
        private Stack<List<string>> _blocks = new Stack<List<string>>();
        private string _prefix;

        public PrefixFieldsVisitor(List<VariableDeclaration> fields, string prefix)
        {
            _fields = fields;
            _prefix = prefix;
        }

        public void Run(INode typeDeclaration)
        {
            typeDeclaration.AcceptVisitor(this, null);
            foreach (VariableDeclaration decl in _fields)
            {
                decl.Name = _prefix + decl.Name;
            }
        }

        public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            Push();
            object result = base.VisitTypeDeclaration(typeDeclaration, data);
            Pop();
            return result;
        }

        public override object VisitBlockStatement(BlockStatement blockStatement, object data)
        {
            Push();
            object result = base.VisitBlockStatement(blockStatement, data);
            Pop();
            return result;
        }

        public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
        {
            Push();
            object result = base.VisitMethodDeclaration(methodDeclaration, data);
            Pop();
            return result;
        }

        public override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
        {
            Push();
            object result = base.VisitPropertyDeclaration(propertyDeclaration, data);
            Pop();
            return result;
        }

        public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
        {
            Push();
            object result = base.VisitConstructorDeclaration(constructorDeclaration, data);
            Pop();
            return result;
        }

        private void Push()
        {
            _blocks.Push(_curBlock);
            _curBlock = new List<string>();
        }

        private void Pop()
        {
            _curBlock = _blocks.Pop();
        }

        public override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
        {
            // process local variables only
            if (_fields.Contains(variableDeclaration))
            {
                return null;
            }
            _curBlock.Add(variableDeclaration.Name);
            return base.VisitVariableDeclaration(variableDeclaration, data);
        }

        public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
        {
            _curBlock.Add(parameterDeclarationExpression.ParameterName);
            //print("add parameter ${parameterDeclarationExpression.ParameterName} to block")
            return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
        }

        public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
        {
            _curBlock.Add(foreachStatement.VariableName);
            return base.VisitForeachStatement(foreachStatement, data);
        }

        public override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
        {
            string name = identifierExpression.Identifier;
            foreach (VariableDeclaration var in _fields)
            {
                if (var.Name == name && !IsLocal(name))
                {
                    identifierExpression.Identifier = _prefix + name;
                    break;
                }
            }
            return base.VisitIdentifierExpression(identifierExpression, data);
        }

        public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
        {
            if (fieldReferenceExpression.TargetObject is ThisReferenceExpression)
            {
                string name = fieldReferenceExpression.FieldName;
                foreach (VariableDeclaration var in _fields)
                {
                    if (var.Name == name)
                    {
                        fieldReferenceExpression.FieldName = _prefix + name;
                        break;
                    }
                }
            }
            return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
        }

        private bool IsLocal(string name)
        {
            foreach (List<string> block in _blocks)
            {
                if (block.Contains(name))
                    return true;
            }
            return _curBlock.Contains(name);
        }
    }
}
