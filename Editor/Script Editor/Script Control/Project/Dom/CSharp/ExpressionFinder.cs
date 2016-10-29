// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1751 $</version>
// </file>

using System;
using System.Text;

namespace AIMS.Libraries.Scripting.Dom.CSharp
{
    /// <summary>
    /// Supports getting the expression including context from the cursor position.
    /// </summary>
    public class CSharpExpressionFinder : IExpressionFinder
    {
        private string _fileName;

        public CSharpExpressionFinder(string fileName)
        {
            _fileName = fileName;
        }

        #region Capture Context

        private ExpressionResult CreateResult(string expression, string inText, int offset)
        {
            if (expression == null)
                return new ExpressionResult(null);
            if (expression.StartsWith("using "))
                return new ExpressionResult(expression.Substring(6).TrimStart(), ExpressionContext.Namespace, null);
            if (!_hadParenthesis && expression.StartsWith("new "))
            {
                return new ExpressionResult(expression.Substring(4).TrimStart(), GetCreationContext(), null);
            }
            if (IsInAttribute(inText, offset))
                return new ExpressionResult(expression, ExpressionContext.GetAttribute(HostCallback.GetCurrentProjectContent()));
            return new ExpressionResult(expression);
        }

        private ExpressionContext GetCreationContext()
        {
            UnGetToken();
            if (GetNextNonWhiteSpace() == '=')
            { // was: "= new"
                ReadNextToken();
                if (_curTokenType == s_ident)
                {     // was: "ident = new"
                    int typeEnd = _offset;
                    ReadNextToken();
                    int typeStart = -1;
                    while (_curTokenType == s_ident)
                    {
                        typeStart = _offset + 1;
                        ReadNextToken();
                        if (_curTokenType == s_dot)
                        {
                            ReadNextToken();
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (typeStart >= 0)
                    {
                        string className = _text.Substring(typeStart, typeEnd - typeStart);
                        int pos = className.IndexOf('<');
                        string nonGenericClassName, genericPart;
                        int typeParameterCount = 0;
                        if (pos > 0)
                        {
                            nonGenericClassName = className.Substring(0, pos);
                            genericPart = className.Substring(pos);
                            pos = 0;
                            do
                            {
                                typeParameterCount += 1;
                                pos = genericPart.IndexOf(',', pos + 1);
                            } while (pos > 0);
                        }
                        else
                        {
                            nonGenericClassName = className;
                            genericPart = null;
                        }
                        ClassFinder finder = new ClassFinder(_fileName, _text, typeStart);
                        IReturnType t = finder.SearchType(nonGenericClassName, typeParameterCount);
                        IClass c = (t != null) ? t.GetUnderlyingClass() : null;
                        if (c != null)
                        {
                            ExpressionContext context = ExpressionContext.TypeDerivingFrom(c, true);
                            if (context.ShowEntry(c))
                            {
                                if (genericPart != null)
                                {
                                    DefaultClass genericClass = new DefaultClass(c.CompilationUnit, c.ClassType, c.Modifiers, c.Region, c.DeclaringType);
                                    genericClass.FullyQualifiedName = c.FullyQualifiedName + genericPart;
                                    genericClass.Documentation = c.Documentation;
                                    context.SuggestedItem = genericClass;
                                }
                                else
                                {
                                    context.SuggestedItem = c;
                                }
                            }
                            return context;
                        }
                    }
                }
            }
            else
            {
                UnGet();
                if (ReadIdentifier(GetNextNonWhiteSpace()) == "throw")
                {
                    return ExpressionContext.TypeDerivingFrom(HostCallback.GetCurrentProjectContent().GetClass("System.Exception"), true);
                }
            }
            return ExpressionContext.ObjectCreation;
        }

        private bool IsInAttribute(string txt, int offset)
        {
            // Get line start:
            int lineStart = offset;
            while (--lineStart > 0 && txt[lineStart] != '\n') ;

            bool inAttribute = false;
            int parens = 0;
            for (int i = lineStart + 1; i < offset; i++)
            {
                char ch = txt[i];
                if (char.IsWhiteSpace(ch))
                    continue;
                if (!inAttribute)
                {
                    // outside attribute
                    if (ch == '[')
                        inAttribute = true;
                    else
                        return false;
                }
                else if (parens == 0)
                {
                    // inside attribute, outside parameter list
                    if (ch == ']')
                        inAttribute = false;
                    else if (ch == '(')
                        parens = 1;
                    else if (!char.IsLetterOrDigit(ch) && ch != ',')
                        return false;
                }
                else
                {
                    // inside attribute, inside parameter list
                    if (ch == '(')
                        parens++;
                    else if (ch == ')')
                        parens--;
                }
            }
            return inAttribute && parens == 0;
        }

        #endregion Capture Context

        #region RemoveLastPart

        /// <summary>
        /// Removed the last part of the expression.
        /// </summary>
        /// <example>
        /// "arr[i]" => "arr"
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

        #endregion RemoveLastPart

        #region Find Expression

        public ExpressionResult FindExpression(string inText, int offset)
        {
            inText = FilterComments(inText, ref offset);
            return CreateResult(FindExpressionInternal(inText, offset), inText, offset);
        }

        public string FindExpressionInternal(string inText, int offset)
        {
            // warning: Do not confuse this.offset and offset
            _text = inText;
            _offset = _lastAccept = offset;
            _state = s_START;
            _hadParenthesis = false;
            if (_text == null)
            {
                return null;
            }

            while (_state != s_ERROR)
            {
                ReadNextToken();
                _state = s_stateTable[_state, _curTokenType];

                if (_state == s_ACCEPT || _state == s_ACCEPT2)
                {
                    _lastAccept = _offset;
                }
                if (_state == s_ACCEPTNOMORE)
                {
                    _lastExpressionStartPosition = _offset + 1;
                    return _text.Substring(_offset + 1, offset - _offset);
                }
            }

            if (_lastAccept < -1)
                return null;

            _lastExpressionStartPosition = _lastAccept + 1;

            return _text.Substring(_lastAccept + 1, offset - _lastAccept);
        }

        private int _lastExpressionStartPosition;

        /// <summary>
        /// Gets the position in the source string (after filtering out comments)
        /// where the beginning of last expression was found.
        /// </summary>
        public int LastExpressionStartPosition
        {
            get
            {
                return _lastExpressionStartPosition;
            }
        }

        #endregion Find Expression

        #region FindFullExpression

        public ExpressionResult FindFullExpression(string inText, int offset)
        {
            int offsetWithoutComments = offset;
            string textWithoutComments = FilterComments(inText, ref offsetWithoutComments);
            string expressionBeforeOffset = FindExpressionInternal(textWithoutComments, offsetWithoutComments);
            if (expressionBeforeOffset == null || expressionBeforeOffset.Length == 0)
                return CreateResult(null, textWithoutComments, offsetWithoutComments);
            StringBuilder b = new StringBuilder(expressionBeforeOffset);
            // append characters after expression
            bool wordFollowing = false;
            int i;
            for (i = offset + 1; i < inText.Length; ++i)
            {
                char c = inText[i];
                if (Char.IsLetterOrDigit(c) || c == '_')
                {
                    if (Char.IsWhiteSpace(inText, i - 1))
                    {
                        wordFollowing = true;
                        break;
                    }
                    b.Append(c);
                }
                else if (Char.IsWhiteSpace(c))
                {
                    // ignore whitespace
                }
                else if (c == '(' || c == '[')
                {
                    int otherBracket = SearchBracketForward(inText, i + 1, c, (c == '(') ? ')' : ']');
                    if (otherBracket < 0)
                        break;
                    if (c == '[')
                    {
                        // do not include [] when it is an array declaration (versus indexer call)
                        bool ok = false;
                        for (int j = i + 1; j < otherBracket; j++)
                        {
                            if (inText[j] != ',' && !char.IsWhiteSpace(inText, j))
                            {
                                ok = true;
                                break;
                            }
                        }
                        if (!ok)
                        {
                            break;
                        }
                    }
                    b.Append(inText, i, otherBracket - i + 1);
                    break;
                }
                else if (c == '<')
                {
                    // accept only if this is a generic type reference
                    int typeParameterEnd = FindEndOfTypeParameters(inText, i);
                    if (typeParameterEnd < 0)
                        break;
                    b.Append(inText, i, typeParameterEnd - i + 1);
                    i = typeParameterEnd;
                }
                else
                {
                    break;
                }
            }
            ExpressionResult res = CreateResult(b.ToString(), textWithoutComments, offsetWithoutComments);
            if (res.Context == ExpressionContext.Default && wordFollowing)
            {
                b = new StringBuilder();
                for (; i < inText.Length; ++i)
                {
                    char c = inText[i];
                    if (char.IsLetterOrDigit(c) || c == '_')
                        b.Append(c);
                    else
                        break;
                }
                if (b.Length > 0)
                {
                    if (AIMS.Libraries.Scripting.NRefactory.Parser.CSharp.Keywords.GetToken(b.ToString()) < 0)
                    {
                        res.Context = ExpressionContext.Type;
                    }
                }
            }
            return res;
        }

        private int FindEndOfTypeParameters(string inText, int offset)
        {
            int level = 0;
            for (int i = offset; i < inText.Length; ++i)
            {
                char c = inText[i];
                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
                {
                    // ignore identifiers and whitespace
                }
                else if (c == ',' || c == '?' || c == '[' || c == ']')
                {
                    // ,  : seperating generic type parameters
                    // ?  : nullable types
                    // [] : arrays
                }
                else if (c == '<')
                {
                    ++level;
                }
                else if (c == '>')
                {
                    --level;
                }
                else
                {
                    return -1;
                }
                if (level == 0)
                    return i;
            }
            return -1;
        }

        #endregion FindFullExpression

        #region SearchBracketForward

        // like CSharpFormattingStrategy.SearchBracketForward, but operates on a string.
        private int SearchBracketForward(string text, int offset, char openBracket, char closingBracket)
        {
            bool inString = false;
            bool inChar = false;
            bool verbatim = false;

            bool lineComment = false;
            bool blockComment = false;

            if (offset < 0) return -1;

            int brackets = 1;

            for (; offset < text.Length; ++offset)
            {
                char ch = text[offset];
                switch (ch)
                {
                    case '\r':
                    case '\n':
                        lineComment = false;
                        inChar = false;
                        if (!verbatim) inString = false;
                        break;

                    case '/':
                        if (blockComment)
                        {
                            if (offset > 0 && text[offset - 1] == '*')
                            {
                                blockComment = false;
                            }
                        }
                        if (!inString && !inChar && offset + 1 < text.Length)
                        {
                            if (!blockComment && text[offset + 1] == '/')
                            {
                                lineComment = true;
                            }
                            if (!lineComment && text[offset + 1] == '*')
                            {
                                blockComment = true;
                            }
                        }
                        break;

                    case '"':
                        if (!(inChar || lineComment || blockComment))
                        {
                            if (inString && verbatim)
                            {
                                if (offset + 1 < text.Length && text[offset + 1] == '"')
                                {
                                    ++offset; // skip escaped quote
                                    inString = false; // let the string go on
                                }
                                else
                                {
                                    verbatim = false;
                                }
                            }
                            else if (!inString && offset > 0 && text[offset - 1] == '@')
                            {
                                verbatim = true;
                            }
                            inString = !inString;
                        }
                        break;

                    case '\'':
                        if (!(inString || lineComment || blockComment))
                        {
                            inChar = !inChar;
                        }
                        break;

                    case '\\':
                        if ((inString && !verbatim) || inChar)
                            ++offset; // skip next character
                        break;

                    default:
                        if (ch == openBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                ++brackets;
                            }
                        }
                        else if (ch == closingBracket)
                        {
                            if (!(inString || inChar || lineComment || blockComment))
                            {
                                --brackets;
                                if (brackets == 0)
                                {
                                    return offset;
                                }
                            }
                        }
                        break;
                }
            }
            return -1;
        }

        #endregion SearchBracketForward

        #region Comment Filter and 'inside string watcher'

        private int _initialOffset;

        public string FilterComments(string text, ref int offset)
        {
            if (text.Length < offset)
                return null;
            if (text.Length == offset)
                return text;
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

                    case '\'':
                        outText.Append(ch);
                        curOffset++;
                        if (!ReadChar(outText, text, ref curOffset))
                        {
                            return null;
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

                    case '/':
                        if (curOffset + 1 < text.Length && text[curOffset + 1] == '/')
                        {
                            offset -= 2;
                            curOffset += 2;
                            if (!ReadToEOL(text, ref curOffset, ref offset))
                            {
                                return null;
                            }
                        }
                        else if (curOffset + 1 < text.Length && text[curOffset + 1] == '*')
                        {
                            offset -= 2;
                            curOffset += 2;
                            if (!ReadMultiLineComment(text, ref curOffset, ref offset))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case '#':
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

        private bool ReadChar(StringBuilder outText, string text, ref int curOffset)
        {
            if (curOffset > _initialOffset)
                return false;
            char first = text[curOffset++];
            outText.Append(first);
            if (curOffset > _initialOffset)
                return false;
            char second = text[curOffset++];
            outText.Append(second);
            if (first == '\\')
            {
                // character is escape sequence, so read one char more
                char next;
                do
                {
                    if (curOffset > _initialOffset)
                        return false;
                    next = text[curOffset++];
                    outText.Append(next);
                    // unicode or hexadecimal character literals can have more content characters
                } while ((second == 'u' || second == 'x') && char.IsLetterOrDigit(next));
            }
            return text[curOffset - 1] == '\'';
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
                else if (ch == '\\')
                {
                    if (curOffset <= _initialOffset)
                        outText.Append(text[curOffset++]);
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

        #endregion Comment Filter and 'inside string watcher'

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

        private char GetNextNonWhiteSpace()
        {
            char ch;
            do
            {
                ch = GetNext();
            } while (char.IsWhiteSpace(ch));
            return ch;
        }

        private char Peek(int n)
        {
            if (_offset - n >= 0)
            {
                return _text[_offset - n];
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

        private void UnGetToken()
        {
            do
            {
                UnGet();
            } while (char.IsLetterOrDigit(Peek()));
        }

        // tokens for our lexer
        private static int s_err = 0;

        private static int s_dot = 1;
        private static int s_strLit = 2;
        private static int s_ident = 3;
        private static int s_new = 4;
        private static int s_bracket = 5;
        private static int s_parent = 6;
        private static int s_curly = 7;
        private static int s_using = 8;
        private static int s_digit = 9;
        private int _curTokenType;

        private static readonly string[] s_tokenStateName = new string[] {
            "Err", "Dot", "StrLit", "Ident", "New", "Bracket", "Paren", "Curly", "Using", "Digit"
        };

        private string GetTokenName(int state)
        {
            return s_tokenStateName[state];
        }

        /// <summary>
        /// used to control whether an expression is in a ObjectCreation context (new *expr*),
        /// or is in the default context (e.g. "new MainForm().Show()", 'new ' is there part of the expression
        /// </summary>
        private bool _hadParenthesis;

        private string _lastIdentifier;

        private void ReadNextToken()
        {
            _curTokenType = s_err;
            char ch = GetNextNonWhiteSpace();
            if (ch == '\0')
            {
                return;
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
                        _hadParenthesis = true;
                        _curTokenType = s_parent;
                    }
                    break;

                case ']':
                    if (ReadBracket('[', ']'))
                    {
                        _curTokenType = s_bracket;
                    }
                    break;

                case '>':
                    if (ReadTypeParameters())
                    {
                        // hack: ignore type parameters and continue reading without changing state
                        ReadNextToken();
                    }
                    break;

                case '.':
                    _curTokenType = s_dot;
                    break;

                case ':':
                    if (GetNext() == ':')
                    {
                        // treat :: like dot
                        _curTokenType = s_dot;
                    }
                    break;

                case '\'':
                case '"':
                    if (ReadStringLiteral(ch))
                    {
                        _curTokenType = s_strLit;
                    }
                    break;

                default:
                    if (IsNumber(ch))
                    {
                        ReadDigit(ch);
                        _curTokenType = s_digit;
                    }
                    else if (IsIdentifierPart(ch))
                    {
                        string ident = ReadIdentifier(ch);
                        if (ident != null)
                        {
                            switch (ident)
                            {
                                case "new":
                                    _curTokenType = s_new;
                                    break;

                                case "using":
                                    _curTokenType = s_using;
                                    break;

                                case "return":
                                case "throw":
                                case "in":
                                case "else":
                                    // treat as error / end of expression
                                    break;

                                default:
                                    _curTokenType = s_ident;
                                    _lastIdentifier = ident;
                                    break;
                            }
                        }
                    }

                    break;
            }
        }

        private bool IsNumber(char ch)
        {
            if (!Char.IsDigit(ch))
                return false;
            int n = 0;
            while (true)
            {
                ch = Peek(n);
                if (Char.IsDigit(ch))
                {
                    n++;
                    continue;
                }
                return n > 0 && !Char.IsLetter(ch);
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

        private bool ReadTypeParameters()
        {
            int level = 1;
            while (level > 0)
            {
                char ch = GetNext();
                switch (ch)
                {
                    case '?':
                    case '[':
                    case ',':
                    case ']':
                        break;

                    case '<':
                        --level;
                        break;

                    case '>':
                        ++level;
                        break;

                    default:
                        if (!char.IsWhiteSpace(ch) && !char.IsLetterOrDigit(ch))
                            return false;
                        break;
                }
            }
            return true;
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
                switch (ch)
                {
                    case '\0':
                        return false;

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

        private void ReadDigit(char ch)
        {
            //string digit = ch.ToString();
            while (Char.IsDigit(Peek()) || Peek() == '.')
            {
                GetNext();
                //digit = GetNext() + digit;
            }
            //return digit;
        }

        private bool IsIdentifierPart(char ch)
        {
            return Char.IsLetterOrDigit(ch) || ch == '_' || ch == '@';
        }

        #endregion mini backward lexer

        #region finite state machine

        private static readonly int s_ERROR = 0;
        private static readonly int s_START = 1;
        private static readonly int s_DOT = 2;
        private static readonly int s_MORE = 3;
        private static readonly int s_CURLY = 4;
        private static readonly int s_CURLY2 = 5;
        private static readonly int s_CURLY3 = 6;

        private static readonly int s_ACCEPT = 7;
        private static readonly int s_ACCEPTNOMORE = 8;
        private static readonly int s_ACCEPT2 = 9;

        private static readonly string[] s_stateName = new string[] {
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
			//                   Err,     Dot,     Str,      ID,         New,     Brk,     Par,     Cur,   Using,       digit
			/*ERROR*/        { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR},
			/*START*/        { s_ERROR,     s_DOT,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE, s_ACCEPT2,   s_CURLY,   s_ACCEPTNOMORE, s_ERROR},
			/*DOT*/          { s_ERROR,   s_ERROR,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE,  s_ACCEPT,   s_CURLY,   s_ERROR,        s_ACCEPT},
			/*MORE*/         { s_ERROR,   s_ERROR,  s_ACCEPT,  s_ACCEPT,        s_ERROR,   s_MORE, s_ACCEPT2,   s_CURLY,   s_ERROR,        s_ACCEPT},
			/*CURLY*/        { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR, s_CURLY2,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR},
			/*CURLY2*/       { s_ERROR,   s_ERROR,   s_ERROR,  s_CURLY3,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_CURLY3},
			/*CURLY3*/       { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR, s_ACCEPTNOMORE,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR},
			/*ACCEPT*/       { s_ERROR,    s_MORE,   s_ERROR,   s_ERROR,       s_ACCEPT,  s_ERROR,   s_ERROR,   s_ERROR,   s_ACCEPTNOMORE, s_ERROR},
			/*ACCEPTNOMORE*/ { s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ERROR},
			/*ACCEPT2*/      { s_ERROR,    s_MORE,   s_ERROR,  s_ACCEPT,       s_ACCEPT,  s_ERROR,   s_ERROR,   s_ERROR,   s_ERROR,        s_ACCEPT},
        };

        #endregion finite state machine
    }
}