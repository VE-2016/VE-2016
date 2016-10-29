// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia"/>
//     <version>$Revision: 2522 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;

namespace AIMS.Libraries.Scripting.NRefactory
{
    /// <summary>
    /// The snippet parser supports parsing code snippets that are not valid as a full compilation unit.
    /// </summary>
    public class SnippetParser
    {
        private readonly SupportedLanguage _language;

        public SnippetParser(SupportedLanguage language)
        {
            _language = language;
        }

        private Errors _errors;
        private List<ISpecial> _specials;

        /// <summary>
        /// Gets the errors of the last call to Parse(). Returns null if parse was not yet called.
        /// </summary>
        public Errors Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Gets the specials of the last call to Parse(). Returns null if parse was not yet called.
        /// </summary>
        public List<ISpecial> Specials
        {
            get { return _specials; }
        }

        /// <summary>
        /// Parse the code. The result may be a CompilationUnit, an Expression, a list of statements or a list of class
        /// members.
        /// </summary>
        public INode Parse(string code)
        {
            IParser parser = ParserFactory.CreateParser(_language, new StringReader(code));
            parser.Parse();
            _errors = parser.Errors;
            _specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
            INode result = parser.CompilationUnit;

            if (_errors.Count > 0)
            {
                if (_language == SupportedLanguage.CSharp)
                {
                    // SEMICOLON HACK : without a trailing semicolon, parsing expressions does not work correctly
                    parser = ParserFactory.CreateParser(_language, new StringReader(code + ";"));
                }
                else
                {
                    parser = ParserFactory.CreateParser(_language, new StringReader(code));
                }
                Expression expression = parser.ParseExpression();
                if (expression != null && parser.Errors.Count < _errors.Count)
                {
                    _errors = parser.Errors;
                    _specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
                    result = expression;
                }
            }
            if (_errors.Count > 0)
            {
                parser = ParserFactory.CreateParser(_language, new StringReader(code));
                BlockStatement block = parser.ParseBlock();
                if (block != null && parser.Errors.Count < _errors.Count)
                {
                    _errors = parser.Errors;
                    _specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
                    result = block;
                }
            }
            if (_errors.Count > 0)
            {
                parser = ParserFactory.CreateParser(_language, new StringReader(code));
                List<INode> members = parser.ParseTypeMembers();
                if (members != null && members.Count > 0 && parser.Errors.Count < _errors.Count)
                {
                    _errors = parser.Errors;
                    _specials = parser.Lexer.SpecialTracker.RetrieveSpecials();
                    result = new NodeListNode(members);
                }
            }
            return result;
        }

        private sealed class NodeListNode : INode
        {
            private List<INode> _nodes;

            public NodeListNode(List<INode> nodes)
            {
                _nodes = nodes;
            }

            public INode Parent
            {
                get { return null; }
                set { throw new NotSupportedException(); }
            }

            public List<INode> Children
            {
                get { return _nodes; }
            }

            public Location StartLocation
            {
                get { return Location.Empty; }
                set { throw new NotSupportedException(); }
            }

            public Location EndLocation
            {
                get { return Location.Empty; }
                set { throw new NotSupportedException(); }
            }

            public object AcceptChildren(IAstVisitor visitor, object data)
            {
                foreach (INode n in _nodes)
                {
                    n.AcceptVisitor(visitor, data);
                }
                return null;
            }

            public object AcceptVisitor(IAstVisitor visitor, object data)
            {
                return AcceptChildren(visitor, data);
            }
        }
    }
}
