using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using VBTokens = AIMS.Libraries.Scripting.NRefactory.Parser.VB.Tokens;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    public class VBNetCompletionBinding : NRefactoryCodeCompletionBinding
    {
        public VBNetCompletionBinding()
            : base(SupportedLanguage.VBNet)
        {
            // Don't use indexer insight for '[', VB uses '(' for indexer access
            this.EnableIndexerInsight = false;
        }

        public override bool HandleKeyPress(CodeEditorControl editor, char ch)
        {
            VBExpressionFinder ef = new VBExpressionFinder();
            int cursor = editor.ActiveViewControl.Caret.Offset;
            ExpressionContext context = null;

            if (ch == '(')
            {
                if (CodeCompletionOptions.KeywordCompletionEnabled)
                {
                    switch (editor.ActiveViewControl.Caret.CurrentWord.Text.Trim())
                    {
                        case "For":
                        case "Lock":
                            context = ExpressionContext.Default;
                            break;

                        case "using":
                            context = ExpressionContext.TypeDerivingFrom(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.IDisposable"), false);
                            break;

                        case "Catch":
                            context = ExpressionContext.TypeDerivingFrom(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.Exception"), false);
                            break;

                        case "For Each":
                        case "typeof":
                        case "default":
                            context = ExpressionContext.Type;
                            break;
                    }
                }
                if (context != null)
                {
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ch);
                    return true;
                }
                else if (EnableMethodInsight)
                {
                    editor.ActiveViewControl.ShowInsightWindow(new MethodInsightDataProvider());
                    return true;
                }
                return false;
            }
            else if (ch == '<')
            {
                Word curWord = editor.ActiveViewControl.Caret.CurrentWord;
                //Word preWord =  curWord.Row.FormattedWords[curWord.Index-1];
                //int lineOffset = editor.ActiveViewControl.Document.Text.Substring(0, editor.ActiveViewControl.Caret.Offset - editor.ActiveViewControl.Caret.Position.X).Length;
                //TextPoint tp = editor.ActiveViewControl.Document.IntPosToPoint(lineOffset);

                if (curWord == null || curWord.Text.Length == 0)
                {
                    // [ is first character on the line
                    // -> Attribute completion
                    editor.ActiveViewControl.ShowCompletionWindow(new AttributesDataProvider(ScriptControl.Parser.ProjectParser.CurrentProjectContent), ch);
                    return true;
                }
            }
            else if (ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled)
            {
                if (InsightRefreshOnComma(editor, ch))
                    return true;
            }
            else if (ch == '=')
            {
                string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
                string documentText = ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName);
                int position = editor.ActiveViewControl.Caret.Offset - 2;

                if (position > 0 && (documentText[position + 1] == '+'))
                {
                    ExpressionResult result = ef.FindFullExpression(documentText, position);

                    if (result.Expression != null)
                    {
                        ResolveResult resolveResult = ScriptControl.Parser.ProjectParser.GetResolver().Resolve(result, editor.ActiveViewControl.Caret.Position.Y + 1, editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.FileName, documentText);
                        if (resolveResult != null && resolveResult.ResolvedType != null)
                        {
                            IClass underlyingClass = resolveResult.ResolvedType.GetUnderlyingClass();
                            if (underlyingClass == null && resolveResult.ResolvedType.FullyQualifiedName.Length > 0)
                            {
                                underlyingClass = ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass(resolveResult.ResolvedType.FullyQualifiedName);
                            }
                            if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.MulticastDelegate")))
                            {
                                //EventHandlerCompletitionDataProvider eventHandlerProvider = new EventHandlerCompletitionDataProvider(result.Expression, resolveResult);
                                //eventHandlerProvider.InsertSpace = true;
                                //editor.ActiveViewControl.ShowCompletionWindow(eventHandlerProvider, ch);
                            }
                        }
                    }
                }
                else if (position > 0)
                {
                    ExpressionResult result = ef.FindFullExpression(documentText, position);
                    if (result.Expression != null)
                    {
                        ResolveResult resolveResult = ScriptControl.Parser.ProjectParser.GetResolver().Resolve(result, editor.ActiveViewControl.Caret.Position.Y + 1, editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.FileName, documentText);
                        if (resolveResult != null && resolveResult.ResolvedType != null)
                        {
                            if (ProvideContextCompletion(editor, resolveResult.ResolvedType, ch))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else if (ch == '\n')
            {
                string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
                // don't return true when inference succeeds, otherwise the ';' won't be added to the document.
                TryDeclarationTypeInference(editor, curLine);
            }

            return base.HandleKeyPress(editor, ch);
        }

        private bool IsInComment(CodeEditorControl editor)
        {
            VBExpressionFinder ef = new VBExpressionFinder();
            int cursor = editor.ActiveViewControl.Caret.Offset - 1;
            return ef.FilterComments(ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor + 1), ref cursor) == null;
        }

        public override bool HandleKeyword(CodeEditorControl editor, string word)
        {
            // TODO: Assistance writing Methods/Fields/Properties/Events:
            // use public/static/etc. as keywords to display a list with other modifiers
            // and possible return types.
            switch (word.ToLowerInvariant())
            {
                case "imports":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CodeCompletionDataProvider(new ExpressionResult("Global", ExpressionContext.Importable)), ' ');
                    return true;

                case "as":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
                    return true;

                case "new":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.ObjectCreation), ' ');
                    return true;

                case "inherits":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
                    return true;

                case "implements":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Interface), ' ');
                    return true;

                case "overrides":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
                    return true;

                case "return":
                    if (IsInComment(editor)) return false;
                    IMember m = GetCurrentMember(editor);
                    if (m != null)
                    {
                        ProvideContextCompletion(editor, m.ReturnType, ' ');
                        return true;
                    }
                    else
                    {
                        goto default;
                    }
                case "option":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new TextCompletionDataProvider("Explicit On",
                                                                               "Explicit Off",
                                                                               "Strict On",
                                                                               "Strict Off",
                                                                               "Compare Binary",
                                                                               "Compare Text"), ' ');
                    return true;

                default:
                    return base.HandleKeyword(editor, word);
            }
        }

        private bool TryDeclarationTypeInference(CodeEditorControl editor, string curLine)
        {
            string lineText = curLine;
            ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new System.IO.StringReader(lineText));
            if (lexer.NextToken().kind != VBTokens.Dim)
                return false;
            if (lexer.NextToken().kind != VBTokens.Identifier)
                return false;
            if (lexer.NextToken().kind != VBTokens.As)
                return false;
            Token t1 = lexer.NextToken();
            if (t1.kind != VBTokens.QuestionMark)
                return false;
            Token t2 = lexer.NextToken();
            if (t2.kind != VBTokens.Assign)
                return false;
            string expr = lineText.Substring(t2.col);

            ResolveResult rr = ScriptControl.Parser.ProjectParser.GetResolver().Resolve(new ExpressionResult(expr),
                                                     editor.ActiveViewControl.Caret.Position.Y + 1,
                                                     t2.col, editor.ActiveViewControl.FileName,
                                                     ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName));
            if (rr != null && rr.ResolvedType != null)
            {
                ClassFinder context = new ClassFinder(editor.ActiveViewControl.FileName, editor.ActiveViewControl.Caret.Position.Y, t1.col);
                if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
                    VBNetAmbience.Instance.ConversionFlags = ConversionFlags.None;
                else
                    VBNetAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
                string typeName = VBNetAmbience.Instance.Convert(rr.ResolvedType);
                int offset = editor.ActiveViewControl.Document.GetRange(new TextRange(0, 0, 0, editor.ActiveViewControl.Caret.Position.Y)).Length;

                editor.ActiveViewControl.Document.InsertText(typeName, offset + t1.col - 1, editor.ActiveViewControl.Caret.Position.Y);
                editor.ActiveViewControl.Caret.SetPos(new TextPoint(editor.ActiveViewControl.Caret.Position.X + typeName.Length - 1, editor.ActiveViewControl.Caret.Position.Y));
                return true;
            }
            return false;
        }
    }
}