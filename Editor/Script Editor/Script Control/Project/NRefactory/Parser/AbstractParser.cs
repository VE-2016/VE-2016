// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2517 $</version>
// </file>

using System;
using System.Collections.Generic;
using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
    public abstract class AbstractParser : IParser
    {
        protected const int MinErrDist = 2;
        protected const string ErrMsgFormat = "-- line {0} col {1}: {2}";  // 0=line, 1=column, 2=text


        private Errors _errors;
        private ILexer _lexer;

        protected int errDist = MinErrDist;

        [CLSCompliant(false)]
        protected CompilationUnit compilationUnit;

        private bool _parseMethodContents = true;

        public bool ParseMethodBodies
        {
            get
            {
                return _parseMethodContents;
            }
            set
            {
                _parseMethodContents = value;
            }
        }

        public ILexer Lexer
        {
            get
            {
                return _lexer;
            }
        }

        public Errors Errors
        {
            get
            {
                return _errors;
            }
        }

        public CompilationUnit CompilationUnit
        {
            get
            {
                return compilationUnit;
            }
        }

        internal AbstractParser(ILexer lexer)
        {
            _errors = lexer.Errors;
            _lexer = lexer;
            _errors.SynErr = new ErrorCodeProc(SynErr);
        }

        public abstract void Parse();

        public abstract Expression ParseExpression();
        public abstract BlockStatement ParseBlock();
        public abstract List<INode> ParseTypeMembers();

        protected abstract void SynErr(int line, int col, int errorNumber);

        protected void SynErr(int n)
        {
            if (errDist >= MinErrDist)
            {
                _errors.SynErr(_lexer.LookAhead.line, _lexer.LookAhead.col, n);
            }
            errDist = 0;
        }

        protected void SemErr(string msg)
        {
            if (errDist >= MinErrDist)
            {
                _errors.Error(_lexer.Token.line, _lexer.Token.col, msg);
            }
            errDist = 0;
        }

        protected void Expect(int n)
        {
            if (_lexer.LookAhead.kind == n)
            {
                _lexer.NextToken();
            }
            else
            {
                SynErr(n);
            }
        }

        #region System.IDisposable interface implementation
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            _errors = null;
            if (_lexer != null)
            {
                _lexer.Dispose();
            }
            _lexer = null;
        }
        #endregion
    }
}
