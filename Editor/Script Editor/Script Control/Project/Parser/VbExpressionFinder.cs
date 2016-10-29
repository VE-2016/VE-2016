using System;
using System.Collections.Generic;
using System.Text;
using AIMS.Libraries.Scripting.Dom;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
    internal class VbExpressionFinder : IExpressionFinder
    {
        private ExpressionResult CreateResult(string expression)
        {
            if (expression == null)
                return new ExpressionResult(null);
            if (expression.Length > 8 && expression.Substring(0, 8).Equals("Imports ", StringComparison.InvariantCultureIgnoreCase))
                return new ExpressionResult(expression.Substring(8).TrimStart(), ExpressionContext.Type, null);
            if (expression.Length > 4 && expression.Substring(0, 4).Equals("New ", StringComparison.InvariantCultureIgnoreCase))
                return new ExpressionResult(expression.Substring(4).TrimStart(), ExpressionContext.ObjectCreation, null);
            if (_curTokenType == s_ident && _lastIdentifier.Equals("as", StringComparison.InvariantCultureIgnoreCase))
                return new ExpressionResult(expression, ExpressionContext.Type);
            return new ExpressionResult(expression);
        }

        public ExpressionResult FindExpression(string inText, int offset)
        {
            return CreateResult(FindExpressionInternal(inText, offset));
        }

        public string FindExpressionInternal(string inText, int offset)
        {
            _text = FilterComments(inText, ref offset);
            _offset = _lastAccept = offset;
            _state = s_START;
            if (_text == null)
            {
                return null;
            }
            //Console.WriteLine("---------------");
            while (_state != s_ERROR)
            {
                ReadNextToken();
                //Console.WriteLine("cur state {0} got token {1}/{3} going to {2}", GetStateName(state), GetTokenName(curTokenType), GetStateName(stateTable[state, curTokenType]), curTokenType);
                _state = s_stateTable[_state, _curTokenType];

                if (_state == s_ACCEPT || _state == s_ACCEPT2 || _state == s_DOT)
                {
                    _lastAccept = _offset;
                }
                if (_state == s_ACCEPTNOMORE)
                {
                    return _text.Substring(_offset + 1, offset - _offset);
                }
            }
            return _text.Substring(_lastAccept + 1, offset - _lastAccept);
        }

        internal int LastExpressionStartPosition
        {
            get
            {
                return ((_state == s_ACCEPTNOMORE) ? _offset : _lastAccept) + 1;
            }
        }

        public ExpressionResult FindFullExpression(string inText, int offset)
        {
            string expressionBeforeOffset = FindExpressionInternal(inText, offset);
            if (expressionBeforeOffset == null || expressionBeforeOffset.Length == 0)
                return CreateResult(null);
            StringBuilder b = new StringBuilder(expressionBeforeOffset);
            // append characters after expression
            for (int i = offset + 1; i < inText.Length; ++i)
            {
                char c = inText[i];
                if (Char.IsLetterOrDigit(c))
                {
                    if (Char.IsWhiteSpace(inText, i - 1))
                        break;
                    b.Append(c);
                }
                else if (c == ' ')
                {
                    b.Append(c);
                }
                else if (c == '(')
                {
                    int otherBracket = SearchBracketForward(inText, i + 1, '(', ')');
                    if (otherBracket < 0)
                        break;
                    b.Append(inText, i, otherBracket - i + 1);
                    break;
                }
                else
                {
                    break;
                }
            }
            return CreateResult(b.ToString());
        }

        // Like VBNetFormattingStrategy.SearchBracketForward, but operates on a string.
        private int SearchBracketForward(string text, int offset, char openBracket, char closingBracket)
        {
            bool inString = false;
            bool inComment = false;
            int brackets = 1;
            for (int i = offset; i < text.Length; ++i)
            {
                char ch = text[i];
                if (ch == '\n')
                {
                    inString = false;
                    inComment = false;
                }
                if (inComment) continue;
                if (ch == '"') inString = !inString;
                if (inString) continue;
                if (ch == '\'')
                {
                    inComment = true;
                }
                else if (ch == openBracket)
                {
                    ++brackets;
                }
                else if (ch == closingBracket)
                {
                    --brackets;
                    if (brackets == 0) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Removed the last part of the expression.
        /// </summary>
        /// <example>
        /// "obj.Field" => "obj"
        /// "obj.Method(args,...)" => "obj.Method"
        /// </example>
        public string RemoveLastPart(string expression)
        {
            _text = expression;
            _offset = _text.Length - 1;
            ReadNextToken();
            if (_curTokenType == s_ident && Peek() == '.')
                GetNext();
            return _text.Substring(0, _offset + 1);
        }

        #region Comment Filter and 'inside string watcher'
        private int _initialOffset;
        public string FilterComments(string text, ref int offset)
        {
            if (text.Length <= offset)
                return null;
            _initialOffset = offset;
            StringBuilder outText = new StringBuilder();
            int curOffset = 0;
            while (curOffset <= _initialOffset)
            {
                char ch = text[curOffset];

                switch (ch)
                {
                    case '@':
                        if (curOffset + 1 < text.Length && text[curOffset + 1] == '"')
                        {
                            outText.Append(text[curOffset++]); // @
                            outText.Append(text[curOffset++]); // "
                            if (!ReadVerbatimString(outText, text, ref curOffset))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            outText.Append(ch);
                            ++curOffset;
                        }
                        break;
                    case '"':
                        outText.Append(ch);
                        curOffset++;
                        if (!ReadString(outText, text, ref curOffset))
                        {
                            return null;
                        }
                        break;
                    case '\'':
                        offset -= 1;
                        curOffset += 1;
                        if (!ReadToEOL(text, ref curOffset, ref offset))
                        {
                            return null;
                        }
                        break;
                    default:
                        outText.Append(ch);
                        ++curOffset;
                        break;
                }
            }

            return outText.ToString();
        }

        private bool ReadToEOL(string text, ref int curOffset, ref int offset)
        {
            while (curOffset <= _initialOffset)
            {
                char ch = text[curOffset++];
                --offset;
                if (ch == '\n')
                {
                    return true;
                }
            }
            return false;
        }

        private bool ReadString(StringBuilder outText, string text, ref int curOffset)
        {
            while (curOffset <= _initialOffset)
            {
                char ch = text[curOffset++];
                outText.Append(ch);
                if (ch == '"')
                {
                    return true;
                }
            }
            return false;
        }

        private bool ReadVerbatimString(StringBuilder outText, string text, ref int curOffset)
        {
            while (curOffset <= _initialOffset)
            {
                char ch = text[curOffset++];
                outText.Append(ch);
                if (ch == '"')
                {
                    if (curOffset < text.Length && text[curOffset] == '"')
                    {
                        outText.Append(text[curOffset++]);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ReadMultiLineComment(string text, ref int curOffset, ref int offset)
        {
            while (curOffset <= _initialOffset)
            {
                char ch = text[curOffset++];
                --offset;
                if (ch == '*')
                {
                    if (curOffset < text.Length && text[curOffset] == '/')
                    {
                        ++curOffset;
                        --offset;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region mini backward lexer
        private string _text;
        private int _offset;

        private char GetNext()
        {
            if (_offset >= 0)
            {
                return _text[_offset--];
            }
            return '\0';
        }

        private char Peek()
        {
            if (_offset >= 0)
            {
                return _text[_offset];
            }
            return '\0';
        }

        private void UnGet()
        {
            ++_offset;
        }

        // tokens for our lexer
        private static int s_err = 0;
        private static int s_dot = 1;
        private static int s_strLit = 2;
        private static int s_ident = 3;
        private static int s_new = 4;
        //		static int Bracket = 5;
        private static int s_parent = 6;
        private static int s_curly = 7;
        private static int s_using = 8;
        private int _curTokenType;

        private readonly static string[] s_tokenStateName = new string[] {
            "Err", "Dot", "StrLit", "Ident", "New", "Bracket", "Paren", "Curly", "Using"
        };
        private string GetTokenName(int state)
        {
            return s_tokenStateName[state];
        }

        private string _lastIdentifier;

        private void ReadNextToken()
        {
            char ch = GetNext();

            _curTokenType = s_err;
            if (ch == '\0' || ch == '\n' || ch == '\r')
            {
                return;
            }
            while (Char.IsWhiteSpace(ch))
            {
                ch = GetNext();
                if (ch == '\n' || ch == '\r')
                {
                    return;
                }
            }

            switch (ch)
            {
                case '}':
                    if (ReadBracket('{', '}'))
                    {
                        _curTokenType = s_curly;
                    }
                    break;
                case ')':
                    if (ReadBracket('(', ')'))
                    {
                        _curTokenType = s_parent;
                    }
                    break;
                case ']':
                    if (ReadBracket('[', ']'))
                    {
                        _curTokenType = s_ident;
                    }
                    break;
                case '.':
                    _curTokenType = s_dot;
                    break;
                case '\'':
                case '"':
                    if (ReadStringLiteral(ch))
                    {
                        _curTokenType = s_strLit;
                    }
                    break;
                default:
                    if (IsIdentifierPart(ch))
                    {
                        string ident = ReadIdentifier(ch);
                        if (ident != null)
                        {
                            switch (ident.ToLowerInvariant())
                            {
                                case "new":
                                    _curTokenType = s_new;
                                    break;
                                case "imports":
                                    _curTokenType = s_using;
                                    break;
                                default:
                                    _lastIdentifier = ident;
                                    _curTokenType = s_ident;
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        private bool ReadStringLiteral(char litStart)
        {
            while (true)
            {
                char ch = GetNext();
                if (ch == '\0')
                {
                    return false;
                }
                if (ch == litStart)
                {
                    if (Peek() == '@' && litStart == '"')
                    {
                        GetNext();
                    }
                    return true;
                }
            }
        }

        private bool ReadBracket(char openBracket, char closingBracket)
        {
            int curlyBraceLevel = 0;
            int squareBracketLevel = 0;
            int parenthesisLevel = 0;
            switch (openBracket)
            {
                case '(':
                    parenthesisLevel++;
                    break;
                case '[':
                    squareBracketLevel++;
                    break;
                case '{':
                    curlyBraceLevel++;
                    break;
            }

            while (parenthesisLevel != 0 || squareBracketLevel != 0 || curlyBraceLevel != 0)
            {
                char ch = GetNext();
                if (ch == '\0')
                {
                    return false;
                }
                switch (ch)
                {
                    case '(':
                        parenthesisLevel--;
                        break;
                    case '[':
                        squareBracketLevel--;
                        break;
                    case '{':
                        curlyBraceLevel--;
                        break;
                    case ')':
                        parenthesisLevel++;
                        break;
                    case ']':
                        squareBracketLevel++;
                        break;
                    case '}':
                        curlyBraceLevel++;
                        break;
                }
            }
            return true;
        }

        private string ReadIdentifier(char ch)
        {
            string identifier = ch.ToString();
            while (IsIdentifierPart(Peek()))
            {
                identifier = GetNext() + identifier;
            }
            return identifier;
        }

        private bool IsIdentifierPart(char ch)
        {
            return Char.IsLetterOrDigit(ch) || ch == '_';
        }
        #endregion

        #region finite state machine
        private readonly static int s_ERROR = 0;
        private readonly static int s_START = 1;
        private readonly static int s_DOT = 2;
        private readonly static int s_MORE = 3;
        private readonly static int s_CURLY = 4;
        private readonly static int s_CURLY2 = 5;
        private readonly static int s_CURLY3 = 6;

        private readonly static int s_ACCEPT = 7;
        private readonly static int s_ACCEPTNOMORE = 8;
        private readonly static int s_ACCEPT2 = 9;

        private readonly static string[] s_stateName = new string[] {
            "ERROR",
            "START",
            "DOT",
            "MORE",
            "CURLY",
            "CURLY2",
            "CURLY3",
            "ACCEPT",
            "ACCEPTNOMORE",
            "ACCEPT2"
        };

        private string GetStateName(int state)
        {
            return s_stateName[state];
        }

        private int _state = 0;
        private int _lastAccept = 0;
        private static int[,] s_stateTable = new int[,] {
			//                   Err,     Dot,     Str,      ID,         New,     Brk,     Par,     Cur,   Using
			/*ERROR*/        { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR},
			/*START*/        { s_ERROR,   s_ERROR,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE, s_ACCEPT2,   s_CURLY,   s_ACCEPTNOMORE},
			/*DOT*/          { s_ERROR,   s_ERROR,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE, s_ACCEPT2,   s_CURLY,   s_ERROR},
			/*MORE*/         { s_ERROR,   s_ERROR,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE, s_ACCEPT2,   s_CURLY,   s_ERROR},
			/*CURLY*/        { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR, s_CURLY2,   s_ERROR,   s_ERROR,   s_ERROR},
			/*CURLY2*/       { s_ERROR,   s_ERROR,   s_ERROR,  s_CURLY3,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR},
			/*CURLY3*/       { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR, s_ACCEPTNOMORE,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR},
			/*ACCEPT*/       { s_ERROR,     s_DOT,   s_ERROR,   s_ERROR,       s_ACCEPT,  s_ERROR,   s_ERROR,   s_ERROR,   s_ACCEPTNOMORE},
			/*ACCEPTNOMORE*/ { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR},
			/*ACCEPT2*/      { s_ERROR,     s_DOT,   s_ERROR,  s_ACCEPT,       s_ACCEPT,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR},
        };
        #endregion
    }
}
