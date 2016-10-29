// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System.Collections;
using AIMS.Libraries.Scripting.NRefactory.Parser.CSharp;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public sealed class CSharpOutputFormatter : AbstractOutputFormatter
    {
        private PrettyPrintOptions _prettyPrintOptions;

        private bool _emitSemicolon = true;

        public bool EmitSemicolon
        {
            get
            {
                return _emitSemicolon;
            }
            set
            {
                _emitSemicolon = value;
            }
        }

        public CSharpOutputFormatter(PrettyPrintOptions prettyPrintOptions) : base(prettyPrintOptions)
        {
            _prettyPrintOptions = prettyPrintOptions;
        }

        public override void PrintToken(int token)
        {
            if (token == Tokens.Semicolon && !EmitSemicolon)
            {
                return;
            }
            PrintText(Tokens.GetTokenString(token));
        }

        private Stack _braceStack = new Stack();

        public void BeginBrace(BraceStyle style)
        {
            switch (style)
            {
                case BraceStyle.EndOfLine:
                    if (!LastCharacterIsWhiteSpace)
                    {
                        Space();
                    }
                    PrintToken(Tokens.OpenCurlyBrace);
                    NewLine();
                    ++IndentationLevel;
                    break;
                case BraceStyle.NextLine:
                    NewLine();
                    Indent();
                    PrintToken(Tokens.OpenCurlyBrace);
                    NewLine();
                    ++IndentationLevel;
                    break;
                case BraceStyle.NextLineShifted:
                    NewLine();
                    ++IndentationLevel;
                    Indent();
                    PrintToken(Tokens.OpenCurlyBrace);
                    NewLine();
                    break;
                case BraceStyle.NextLineShifted2:
                    NewLine();
                    ++IndentationLevel;
                    Indent();
                    PrintToken(Tokens.OpenCurlyBrace);
                    NewLine();
                    ++IndentationLevel;
                    break;
            }
            _braceStack.Push(style);
        }

        public void EndBrace()
        {
            BraceStyle style = (BraceStyle)_braceStack.Pop();
            switch (style)
            {
                case BraceStyle.EndOfLine:
                case BraceStyle.NextLine:
                    --IndentationLevel;
                    Indent();
                    PrintToken(Tokens.CloseCurlyBrace);
                    NewLine();
                    break;
                case BraceStyle.NextLineShifted:
                    Indent();
                    PrintToken(Tokens.CloseCurlyBrace);
                    NewLine();
                    --IndentationLevel;
                    break;
                case BraceStyle.NextLineShifted2:
                    --IndentationLevel;
                    Indent();
                    PrintToken(Tokens.CloseCurlyBrace);
                    NewLine();
                    --IndentationLevel;
                    break;
            }
        }

        public override void PrintIdentifier(string identifier)
        {
            if (Keywords.GetToken(identifier) >= 0)
                PrintText("@");
            PrintText(identifier);
        }

        public override void PrintComment(Comment comment, bool forceWriteInPreviousBlock)
        {
            switch (comment.CommentType)
            {
                case CommentType.Block:
                    if (forceWriteInPreviousBlock)
                    {
                        WriteInPreviousLine("/*" + comment.CommentText + "*/", forceWriteInPreviousBlock);
                    }
                    else
                    {
                        PrintSpecialText("/*" + comment.CommentText + "*/");
                    }
                    break;
                case CommentType.Documentation:
                    WriteLineInPreviousLine("///" + comment.CommentText, forceWriteInPreviousBlock);
                    break;
                default:
                    WriteLineInPreviousLine("//" + comment.CommentText, forceWriteInPreviousBlock);
                    break;
            }
        }
    }
}
