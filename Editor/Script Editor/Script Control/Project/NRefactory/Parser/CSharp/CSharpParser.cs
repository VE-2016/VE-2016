// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2638 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.CSharp
{
    internal sealed partial class Parser : AbstractParser
    {
        private Lexer _lexer;

        public Parser(ILexer lexer) : base(lexer)
        {
            _lexer = (Lexer)lexer;
        }

        private StringBuilder _qualidentBuilder = new StringBuilder();

        private Token t
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return _lexer.Token;
            }
        }

        private Token la
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                return _lexer.LookAhead;
            }
        }

        public void Error(string s)
        {
            if (errDist >= MinErrDist)
            {
                this.Errors.Error(_lexer.Token.line, _lexer.Token.col, s);
            }
            errDist = 0;
        }

        public override Expression ParseExpression()
        {
            _lexer.NextToken();
            Expression expr;
            Expr(out expr);
            // SEMICOLON HACK : without a trailing semicolon, parsing expressions does not work correctly
            if (la.kind == Tokens.Semicolon) _lexer.NextToken();
            Expect(Tokens.EOF);
            return expr;
        }

        public override BlockStatement ParseBlock()
        {
            _lexer.NextToken();
            compilationUnit = new CompilationUnit();

            BlockStatement blockStmt = new BlockStatement();
            blockStmt.StartLocation = la.Location;
            compilationUnit.BlockStart(blockStmt);

            while (la.kind != Tokens.EOF)
            {
                Token oldLa = la;
                Statement();
                if (la == oldLa)
                {
                    // did not advance lexer position, we cannot parse this as a statement block
                    return null;
                }
            }

            compilationUnit.BlockEnd();
            Expect(Tokens.EOF);
            return blockStmt;
        }

        public override List<INode> ParseTypeMembers()
        {
            _lexer.NextToken();
            compilationUnit = new CompilationUnit();

            TypeDeclaration newType = new TypeDeclaration(Modifiers.None, null);
            compilationUnit.BlockStart(newType);
            ClassBody();
            compilationUnit.BlockEnd();
            Expect(Tokens.EOF);
            return newType.Children;
        }

        // Begin ISTypeCast
        private bool IsTypeCast()
        {
            if (la.kind != Tokens.OpenParenthesis)
            {
                return false;
            }
            if (IsSimpleTypeCast())
            {
                return true;
            }
            return GuessTypeCast();
        }

        // "(" ( typeKW [ "[" {","} "]" | "*" ] | void  ( "[" {","} "]" | "*" ) ) ")"
        // only for built-in types, all others use GuessTypeCast!
        private bool IsSimpleTypeCast()
        {
            // assert: la.kind == _lpar
            _lexer.StartPeek();
            Token pt = _lexer.Peek();

            if (!IsTypeKWForTypeCast(ref pt))
            {
                return false;
            }
            if (pt.kind == Tokens.Question)
                pt = _lexer.Peek();
            return pt.kind == Tokens.CloseParenthesis;
        }

        /* !!! Proceeds from current peek position !!! */
        private bool IsTypeKWForTypeCast(ref Token pt)
        {
            if (Tokens.TypeKW[pt.kind])
            {
                pt = _lexer.Peek();
                return IsPointerOrDims(ref pt) && SkipQuestionMark(ref pt);
            }
            else if (pt.kind == Tokens.Void)
            {
                pt = _lexer.Peek();
                return IsPointerOrDims(ref pt);
            }
            return false;
        }

        /* !!! Proceeds from current peek position !!! */
        private bool IsTypeNameOrKWForTypeCast(ref Token pt)
        {
            if (IsTypeKWForTypeCast(ref pt))
                return true;
            else
                return IsTypeNameForTypeCast(ref pt);
        }

        // TypeName = ident [ "::" ident ] { ["<" TypeNameOrKW { "," TypeNameOrKW } ">" ] "." ident } ["?"] PointerOrDims
        /* !!! Proceeds from current peek position !!! */
        private bool IsTypeNameForTypeCast(ref Token pt)
        {
            // ident
            if (pt.kind != Tokens.Identifier)
            {
                return false;
            }
            pt = Peek();
            // "::" ident
            if (pt.kind == Tokens.DoubleColon)
            {
                pt = Peek();
                if (pt.kind != Tokens.Identifier)
                {
                    return false;
                }
                pt = Peek();
            }
            // { ["<" TypeNameOrKW { "," TypeNameOrKW } ">" ] "." ident }
            while (true)
            {
                if (pt.kind == Tokens.LessThan)
                {
                    do
                    {
                        pt = Peek();
                        if (!IsTypeNameOrKWForTypeCast(ref pt))
                        {
                            return false;
                        }
                    } while (pt.kind == Tokens.Comma);
                    if (pt.kind != Tokens.GreaterThan)
                    {
                        return false;
                    }
                    pt = Peek();
                }
                if (pt.kind != Tokens.Dot)
                    break;
                pt = Peek();
                if (pt.kind != Tokens.Identifier)
                {
                    return false;
                }
                pt = Peek();
            }
            // ["?"]
            if (pt.kind == Tokens.Question)
            {
                pt = Peek();
            }
            if (pt.kind == Tokens.Times || pt.kind == Tokens.OpenSquareBracket)
            {
                return IsPointerOrDims(ref pt);
            }
            return true;
        }

        // "(" TypeName ")" castFollower
        private bool GuessTypeCast()
        {
            // assert: la.kind == _lpar
            StartPeek();
            Token pt = Peek();

            if (!IsTypeNameForTypeCast(ref pt))
            {
                return false;
            }

            // ")"
            if (pt.kind != Tokens.CloseParenthesis)
            {
                return false;
            }
            // check successor
            pt = Peek();
            return Tokens.CastFollower[pt.kind] || (Tokens.TypeKW[pt.kind] && _lexer.Peek().kind == Tokens.Dot);
        }
        // END IsTypeCast

        /* Checks whether the next sequences of tokens is a qualident *
		 * and returns the qualident string                           */
        /* !!! Proceeds from current peek position !!! */
        private bool IsQualident(ref Token pt, out string qualident)
        {
            if (pt.kind == Tokens.Identifier)
            {
                _qualidentBuilder.Length = 0; _qualidentBuilder.Append(pt.val);
                pt = Peek();
                while (pt.kind == Tokens.Dot || pt.kind == Tokens.DoubleColon)
                {
                    pt = Peek();
                    if (pt.kind != Tokens.Identifier)
                    {
                        qualident = String.Empty;
                        return false;
                    }
                    _qualidentBuilder.Append('.');
                    _qualidentBuilder.Append(pt.val);
                    pt = Peek();
                }
                qualident = _qualidentBuilder.ToString();
                return true;
            }
            qualident = String.Empty;
            return false;
        }

        /* Skips generic type extensions */
        /* !!! Proceeds from current peek position !!! */

        /* skip: { "*" | "[" { "," } "]" } */
        /* !!! Proceeds from current peek position !!! */
        private bool IsPointerOrDims(ref Token pt)
        {
            for (;;)
            {
                if (pt.kind == Tokens.OpenSquareBracket)
                {
                    do pt = Peek();
                    while (pt.kind == Tokens.Comma);
                    if (pt.kind != Tokens.CloseSquareBracket) return false;
                }
                else if (pt.kind != Tokens.Times) break;
                pt = Peek();
            }
            return true;
        }

        /* Return the n-th token after the current lookahead token */
        private void StartPeek()
        {
            _lexer.StartPeek();
        }

        private Token Peek()
        {
            return _lexer.Peek();
        }

        private Token Peek(int n)
        {
            _lexer.StartPeek();
            Token x = la;
            while (n > 0)
            {
                x = _lexer.Peek();
                n--;
            }
            return x;
        }

        /*-----------------------------------------------------------------*
		 * Resolver routines to resolve LL(1) conflicts:                   *                                                  *
		 * These resolution routine return a boolean value that indicates  *
		 * whether the alternative at hand shall be choosen or not.        *
		 * They are used in IF ( ... ) expressions.                        *
		 *-----------------------------------------------------------------*/

        /* True, if ident is followed by "=" */
        private bool IdentAndAsgn()
        {
            return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.Assign;
        }

        private bool IsAssignment() { return IdentAndAsgn(); }

        /* True, if ident is followed by ",", "=", "[" or ";" */
        private bool IsVarDecl()
        {
            int peek = Peek(1).kind;
            return la.kind == Tokens.Identifier &&
                (peek == Tokens.Comma || peek == Tokens.Assign || peek == Tokens.Semicolon || peek == Tokens.OpenSquareBracket);
        }

        /* True, if the comma is not a trailing one, *
		 * like the last one in: a, b, c,            */
        private bool NotFinalComma()
        {
            int peek = Peek(1).kind;
            return la.kind == Tokens.Comma &&
                peek != Tokens.CloseCurlyBrace && peek != Tokens.CloseSquareBracket;
        }

        /* True, if "void" is followed by "*" */
        private bool NotVoidPointer()
        {
            return la.kind == Tokens.Void && Peek(1).kind != Tokens.Times;
        }

        /* True, if "checked" or "unchecked" are followed by "{" */
        private bool UnCheckedAndLBrace()
        {
            return la.kind == Tokens.Checked || la.kind == Tokens.Unchecked &&
                Peek(1).kind == Tokens.OpenCurlyBrace;
        }

        /* True, if "." is followed by an ident */
        private bool DotAndIdent()
        {
            return la.kind == Tokens.Dot && Peek(1).kind == Tokens.Identifier;
        }

        /* True, if ident is followed by ":" */
        private bool IdentAndColon()
        {
            return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.Colon;
        }

        private bool IsLabel() { return IdentAndColon(); }

        /* True, if ident is followed by "(" */
        private bool IdentAndLPar()
        {
            return la.kind == Tokens.Identifier && Peek(1).kind == Tokens.OpenParenthesis;
        }

        /* True, if "catch" is followed by "(" */
        private bool CatchAndLPar()
        {
            return la.kind == Tokens.Catch && Peek(1).kind == Tokens.OpenParenthesis;
        }
        private bool IsTypedCatch() { return CatchAndLPar(); }

        /* True, if "[" is followed by the ident "assembly" */
        private bool IsGlobalAttrTarget()
        {
            Token pt = Peek(1);
            return la.kind == Tokens.OpenSquareBracket &&
                pt.kind == Tokens.Identifier && pt.val == "assembly";
        }

        /* True, if "[" is followed by "," or "]" */
        private bool LBrackAndCommaOrRBrack()
        {
            int peek = Peek(1).kind;
            return la.kind == Tokens.OpenSquareBracket &&
                (peek == Tokens.Comma || peek == Tokens.CloseSquareBracket);
        }

        /* True, if "[" is followed by "," or "]" */
        /* or if the current token is "*"         */
        private bool TimesOrLBrackAndCommaOrRBrack()
        {
            return la.kind == Tokens.Times || LBrackAndCommaOrRBrack();
        }
        private bool IsPointerOrDims() { return TimesOrLBrackAndCommaOrRBrack(); }
        private bool IsPointer() { return la.kind == Tokens.Times; }


        private bool SkipGeneric(ref Token pt)
        {
            if (pt.kind == Tokens.LessThan)
            {
                do
                {
                    pt = Peek();
                    if (!IsTypeNameOrKWForTypeCast(ref pt)) return false;
                } while (pt.kind == Tokens.Comma);
                if (pt.kind != Tokens.GreaterThan) return false;
                pt = Peek();
            }
            return true;
        }
        private bool SkipQuestionMark(ref Token pt)
        {
            if (pt.kind == Tokens.Question)
            {
                pt = Peek();
            }
            return true;
        }

        /* True, if lookahead is a primitive type keyword, or */
        /* if it is a type declaration followed by an ident   */
        private bool IsLocalVarDecl()
        {
            if (IsYieldStatement())
            {
                return false;
            }
            if ((Tokens.TypeKW[la.kind] && Peek(1).kind != Tokens.Dot) || la.kind == Tokens.Void)
            {
                return true;
            }

            StartPeek();
            Token pt = la;
            return IsTypeNameOrKWForTypeCast(ref pt) && pt.kind == Tokens.Identifier;
        }

        /* True if lookahead is type parameters (<...>) followed by the specified token */
        private bool IsGenericFollowedBy(int token)
        {
            Token t = la;
            if (t.kind != Tokens.LessThan) return false;
            StartPeek();
            return SkipGeneric(ref t) && t.kind == token;
        }

        private bool IsExplicitInterfaceImplementation()
        {
            StartPeek();
            Token pt = la;
            pt = Peek();
            if (pt.kind == Tokens.Dot || pt.kind == Tokens.DoubleColon)
                return true;
            if (pt.kind == Tokens.LessThan)
            {
                if (SkipGeneric(ref pt))
                    return pt.kind == Tokens.Dot;
            }
            return false;
        }

        /* True, if lookahead ident is "where" */
        private bool IdentIsWhere()
        {
            return la.kind == Tokens.Identifier && la.val == "where";
        }

        /* True, if lookahead ident is "get" */
        private bool IdentIsGet()
        {
            return la.kind == Tokens.Identifier && la.val == "get";
        }

        /* True, if lookahead ident is "set" */
        private bool IdentIsSet()
        {
            return la.kind == Tokens.Identifier && la.val == "set";
        }

        /* True, if lookahead ident is "add" */
        private bool IdentIsAdd()
        {
            return la.kind == Tokens.Identifier && la.val == "add";
        }

        /* True, if lookahead ident is "remove" */
        private bool IdentIsRemove()
        {
            return la.kind == Tokens.Identifier && la.val == "remove";
        }

        /* True, if lookahead ident is "yield" and than follows a break or return */
        private bool IsYieldStatement()
        {
            return la.kind == Tokens.Identifier && la.val == "yield" && (Peek(1).kind == Tokens.Return || Peek(1).kind == Tokens.Break);
        }

        /* True, if lookahead is a local attribute target specifier, *
		 * i.e. one of "event", "return", "field", "method",         *
		 *             "module", "param", "property", or "type"      */
        private bool IsLocalAttrTarget()
        {
            int cur = la.kind;
            string val = la.val;

            return (cur == Tokens.Event || cur == Tokens.Return ||
                    (cur == Tokens.Identifier &&
                     (val == "field" || val == "method" || val == "module" ||
                      val == "param" || val == "property" || val == "type"))) &&
                Peek(1).kind == Tokens.Colon;
        }

        private bool IsShiftRight()
        {
            Token next = Peek(1);
            // TODO : Add col test (seems not to work, lexer bug...) :  && la.col == next.col - 1
            return (la.kind == Tokens.GreaterThan && next.kind == Tokens.GreaterThan);
        }

        private bool IsTypeReferenceExpression(Expression expr)
        {
            if (expr is TypeReferenceExpression) return ((TypeReferenceExpression)expr).TypeReference.GenericTypes.Count == 0;
            while (expr is FieldReferenceExpression)
            {
                expr = ((FieldReferenceExpression)expr).TargetObject;
                if (expr is TypeReferenceExpression) return true;
            }
            return expr is IdentifierExpression;
        }

        private TypeReferenceExpression GetTypeReferenceExpression(Expression expr, List<TypeReference> genericTypes)
        {
            TypeReferenceExpression tre = expr as TypeReferenceExpression;
            if (tre != null)
            {
                return new TypeReferenceExpression(new TypeReference(tre.TypeReference.Type, tre.TypeReference.PointerNestingLevel, tre.TypeReference.RankSpecifier, genericTypes));
            }
            StringBuilder b = new StringBuilder();
            if (!WriteFullTypeName(b, expr))
            {
                // there is some TypeReferenceExpression hidden in the expression
                while (expr is FieldReferenceExpression)
                {
                    expr = ((FieldReferenceExpression)expr).TargetObject;
                }
                tre = expr as TypeReferenceExpression;
                if (tre != null)
                {
                    TypeReference typeRef = tre.TypeReference;
                    if (typeRef.GenericTypes.Count == 0)
                    {
                        typeRef = typeRef.Clone();
                        typeRef.Type += "." + b.ToString();
                        typeRef.GenericTypes.AddRange(genericTypes);
                    }
                    else
                    {
                        typeRef = new InnerClassTypeReference(typeRef, b.ToString(), genericTypes);
                    }
                    return new TypeReferenceExpression(typeRef);
                }
            }
            return new TypeReferenceExpression(new TypeReference(b.ToString(), 0, null, genericTypes));
        }

        /* Writes the type name represented through the expression into the string builder. */
        /* Returns true when the expression was converted successfully, returns false when */
        /* There was an unknown expression (e.g. TypeReferenceExpression) in it */
        private bool WriteFullTypeName(StringBuilder b, Expression expr)
        {
            FieldReferenceExpression fre = expr as FieldReferenceExpression;
            if (fre != null)
            {
                bool result = WriteFullTypeName(b, fre.TargetObject);
                if (b.Length > 0) b.Append('.');
                b.Append(fre.FieldName);
                return result;
            }
            else if (expr is IdentifierExpression)
            {
                b.Append(((IdentifierExpression)expr).Identifier);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsMostNegativeIntegerWithoutTypeSuffix()
        {
            Token token = la;
            if (token.kind == Tokens.Literal)
            {
                return token.val == "2147483648" || token.val == "9223372036854775808";
            }
            else
            {
                return false;
            }
        }

        private bool LastExpressionIsUnaryMinus(System.Collections.ArrayList expressions)
        {
            if (expressions.Count == 0) return false;
            UnaryOperatorExpression uoe = expressions[expressions.Count - 1] as UnaryOperatorExpression;
            if (uoe != null)
            {
                return uoe.Op == UnaryOperatorType.Minus;
            }
            else
            {
                return false;
            }
        }
    }
}
