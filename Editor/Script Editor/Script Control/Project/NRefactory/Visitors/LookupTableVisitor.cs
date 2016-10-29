// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2337 $</version>
// </file>

using System;
using System.Collections.Generic;

using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Visitors
{
    public class LocalLookupVariable
    {
        private TypeReference _typeRef;
        private Location _startPos;
        private Location _endPos;
        private bool _isConst;

        public TypeReference TypeRef
        {
            get
            {
                return _typeRef;
            }
        }
        public Location StartPos
        {
            get
            {
                return _startPos;
            }
        }
        public Location EndPos
        {
            get
            {
                return _endPos;
            }
        }

        public bool IsConst
        {
            get
            {
                return _isConst;
            }
        }

        public LocalLookupVariable(TypeReference typeRef, Location startPos, Location endPos, bool isConst)
        {
            _typeRef = typeRef;
            _startPos = startPos;
            _endPos = endPos;
            _isConst = isConst;
        }
    }

    public class LookupTableVisitor : AbstractAstVisitor
    {
        private Dictionary<string, List<LocalLookupVariable>> _variables;
        private SupportedLanguage _language;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Dictionary<string, List<LocalLookupVariable>> Variables
        {
            get
            {
                return _variables;
            }
        }

        private List<WithStatement> _withStatements = new List<WithStatement>();

        public List<WithStatement> WithStatements
        {
            get
            {
                return _withStatements;
            }
        }

        public LookupTableVisitor(SupportedLanguage language)
        {
            _language = language;
            if (language == SupportedLanguage.VBNet)
            {
                _variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCultureIgnoreCase);
            }
            else
            {
                _variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCulture);
            }
        }

        public void AddVariable(TypeReference typeRef, string name, Location startPos, Location endPos, bool isConst)
        {
            if (name == null || name.Length == 0)
            {
                return;
            }
            List<LocalLookupVariable> list;
            if (!_variables.ContainsKey(name))
            {
                _variables[name] = list = new List<LocalLookupVariable>();
            }
            else
            {
                list = (List<LocalLookupVariable>)_variables[name];
            }
            list.Add(new LocalLookupVariable(typeRef, startPos, endPos, isConst));
        }

        public override object VisitWithStatement(WithStatement withStatement, object data)
        {
            _withStatements.Add(withStatement);
            return base.VisitWithStatement(withStatement, data);
        }

        private Stack<Location> _endLocationStack = new Stack<Location>();

        private Location CurrentEndLocation
        {
            get
            {
                return (_endLocationStack.Count == 0) ? Location.Empty : _endLocationStack.Peek();
            }
        }

        public override object VisitBlockStatement(BlockStatement blockStatement, object data)
        {
            _endLocationStack.Push(blockStatement.EndLocation);
            base.VisitBlockStatement(blockStatement, data);
            _endLocationStack.Pop();
            return null;
        }

        public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
        {
            for (int i = 0; i < localVariableDeclaration.Variables.Count; ++i)
            {
                VariableDeclaration varDecl = (VariableDeclaration)localVariableDeclaration.Variables[i];

                AddVariable(localVariableDeclaration.GetTypeForVariable(i),
                            varDecl.Name,
                            localVariableDeclaration.StartLocation,
                            CurrentEndLocation,
                            (localVariableDeclaration.Modifier & Modifiers.Const) == Modifiers.Const);
            }
            return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
        }

        public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
        {
            foreach (ParameterDeclarationExpression p in anonymousMethodExpression.Parameters)
            {
                AddVariable(p.TypeReference, p.ParameterName, anonymousMethodExpression.StartLocation, anonymousMethodExpression.EndLocation, false);
            }
            return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
        }

        public override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
        {
            // uses LocalVariableDeclaration, we just have to put the end location on the stack
            if (forNextStatement.EmbeddedStatement.EndLocation.IsEmpty)
            {
                return base.VisitForNextStatement(forNextStatement, data);
            }
            else
            {
                _endLocationStack.Push(forNextStatement.EmbeddedStatement.EndLocation);
                base.VisitForNextStatement(forNextStatement, data);
                _endLocationStack.Pop();
                return null;
            }
        }

        public override object VisitForStatement(ForStatement forStatement, object data)
        {
            // uses LocalVariableDeclaration, we just have to put the end location on the stack
            if (forStatement.EmbeddedStatement.EndLocation.IsEmpty)
            {
                return base.VisitForStatement(forStatement, data);
            }
            else
            {
                _endLocationStack.Push(forStatement.EmbeddedStatement.EndLocation);
                base.VisitForStatement(forStatement, data);
                _endLocationStack.Pop();
                return null;
            }
        }

        public override object VisitUsingStatement(UsingStatement usingStatement, object data)
        {
            // uses LocalVariableDeclaration, we just have to put the end location on the stack
            if (usingStatement.EmbeddedStatement.EndLocation.IsEmpty)
            {
                return base.VisitUsingStatement(usingStatement, data);
            }
            else
            {
                _endLocationStack.Push(usingStatement.EmbeddedStatement.EndLocation);
                base.VisitUsingStatement(usingStatement, data);
                _endLocationStack.Pop();
                return null;
            }
        }

        public override object VisitSwitchSection(SwitchSection switchSection, object data)
        {
            if (_language == SupportedLanguage.VBNet)
            {
                return VisitBlockStatement(switchSection, data);
            }
            else
            {
                return base.VisitSwitchSection(switchSection, data);
            }
        }

        public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
        {
            AddVariable(foreachStatement.TypeReference,
                        foreachStatement.VariableName,
                        foreachStatement.StartLocation,
                        foreachStatement.EndLocation,
                        false);

            if (foreachStatement.Expression != null)
            {
                foreachStatement.Expression.AcceptVisitor(this, data);
            }
            if (foreachStatement.EmbeddedStatement == null)
            {
                return data;
            }
            return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
        }

        public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
        {
            if (tryCatchStatement == null)
            {
                return data;
            }
            if (tryCatchStatement.StatementBlock != null)
            {
                tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
            }
            if (tryCatchStatement.CatchClauses != null)
            {
                foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
                {
                    if (catchClause != null)
                    {
                        if (catchClause.TypeReference != null && catchClause.VariableName != null)
                        {
                            AddVariable(catchClause.TypeReference,
                                        catchClause.VariableName,
                                        catchClause.StatementBlock.StartLocation,
                                        catchClause.StatementBlock.EndLocation,
                                        false);
                        }
                        catchClause.StatementBlock.AcceptVisitor(this, data);
                    }
                }
            }
            if (tryCatchStatement.FinallyBlock != null)
            {
                return tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
            }
            return data;
        }
    }
}
