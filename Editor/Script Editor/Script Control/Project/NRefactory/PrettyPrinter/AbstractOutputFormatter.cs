// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2522 $</version>
// </file>

using System;
using System.Collections;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    /// <summary>
    /// Base class of output formatters.
    /// </summary>
    public abstract class AbstractOutputFormatter : IOutputFormatter
    {
        private StringBuilder _text = new StringBuilder();

        private int _indentationLevel = 0;
        private bool _indent = true;
        private bool _doNewLine = true;
        private AbstractPrettyPrintOptions _prettyPrintOptions;

        public int IndentationLevel
        {
            get
            {
                return _indentationLevel;
            }
            set
            {
                _indentationLevel = value;
            }
        }

        public string Text
        {
            get
            {
                return _text.ToString();
            }
        }

        public int TextLength
        {
            get
            {
                return _text.Length;
            }
        }


        public bool DoIndent
        {
            get
            {
                return _indent;
            }
            set
            {
                _indent = value;
            }
        }

        public bool DoNewLine
        {
            get
            {
                return _doNewLine;
            }
            set
            {
                _doNewLine = value;
            }
        }

        protected AbstractOutputFormatter(AbstractPrettyPrintOptions prettyPrintOptions)
        {
            _prettyPrintOptions = prettyPrintOptions;
        }

        public void Indent()
        {
            if (DoIndent)
            {
                int indent = 0;
                while (indent < _prettyPrintOptions.IndentSize * _indentationLevel)
                {
                    char ch = _prettyPrintOptions.IndentationChar;
                    if (ch == '\t' && indent + _prettyPrintOptions.TabSize > _prettyPrintOptions.IndentSize * _indentationLevel)
                    {
                        ch = ' ';
                    }
                    _text.Append(ch);
                    if (ch == '\t')
                    {
                        indent += _prettyPrintOptions.TabSize;
                    }
                    else
                    {
                        ++indent;
                    }
                }
            }
        }

        public void Space()
        {
            _text.Append(' ');
        }

        internal int lastLineStart = 0;
        internal int lineBeforeLastStart = 0;

        public bool LastCharacterIsNewLine
        {
            get
            {
                return _text.Length == lastLineStart;
            }
        }

        public bool LastCharacterIsWhiteSpace
        {
            get
            {
                return _text.Length == 0 || char.IsWhiteSpace(_text[_text.Length - 1]);
            }
        }

        public virtual void NewLine()
        {
            if (DoNewLine)
            {
                if (!LastCharacterIsNewLine)
                {
                    lineBeforeLastStart = lastLineStart;
                }
                _text.AppendLine();
                lastLineStart = _text.Length;
            }
        }

        public virtual void EndFile()
        {
        }

        protected void WriteLineInPreviousLine(string txt, bool forceWriteInPreviousBlock)
        {
            WriteInPreviousLine(txt + Environment.NewLine, forceWriteInPreviousBlock);
        }

        protected void WriteInPreviousLine(string txt, bool forceWriteInPreviousBlock)
        {
            if (txt.Length == 0) return;

            bool lastCharacterWasNewLine = LastCharacterIsNewLine;
            if (lastCharacterWasNewLine)
            {
                if (forceWriteInPreviousBlock == false)
                {
                    if (txt != Environment.NewLine) Indent();
                    _text.Append(txt);
                    lineBeforeLastStart = lastLineStart;
                    lastLineStart = _text.Length;
                    return;
                }
                lastLineStart = lineBeforeLastStart;
            }
            string lastLine = _text.ToString(lastLineStart, _text.Length - lastLineStart);
            _text.Remove(lastLineStart, _text.Length - lastLineStart);
            if (txt != Environment.NewLine)
            {
                if (forceWriteInPreviousBlock) ++_indentationLevel;
                Indent();
                if (forceWriteInPreviousBlock) --_indentationLevel;
            }
            _text.Append(txt);
            lineBeforeLastStart = lastLineStart;
            lastLineStart = _text.Length;
            _text.Append(lastLine);
            if (lastCharacterWasNewLine)
            {
                lineBeforeLastStart = lastLineStart;
                lastLineStart = _text.Length;
            }
        }

        /// <summary>
        /// Prints a text that cannot be inserted before using WriteInPreviousLine
        /// into the current line
        /// </summary>
        protected void PrintSpecialText(string specialText)
        {
            lineBeforeLastStart = _text.Length;
            _text.Append(specialText);
            lastLineStart = _text.Length;
        }

        public void PrintTokenList(ArrayList tokenList)
        {
            foreach (int token in tokenList)
            {
                PrintToken(token);
                Space();
            }
        }

        public abstract void PrintComment(Comment comment, bool forceWriteInPreviousBlock);

        public virtual void PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock)
        {
            if (string.IsNullOrEmpty(directive.Arg))
                WriteLineInPreviousLine(directive.Cmd, forceWriteInPreviousBlock);
            else
                WriteLineInPreviousLine(directive.Cmd + " " + directive.Arg, forceWriteInPreviousBlock);
        }

        public void PrintBlankLine(bool forceWriteInPreviousBlock)
        {
            WriteInPreviousLine(Environment.NewLine, forceWriteInPreviousBlock);
        }

        public abstract void PrintToken(int token);

        public void PrintText(string text)
        {
            _text.Append(text);
        }

        public abstract void PrintIdentifier(string identifier);
    }
}
