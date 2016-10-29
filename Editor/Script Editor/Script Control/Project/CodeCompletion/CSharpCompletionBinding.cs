using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using AIMS.Libraries.Scripting.ScriptControl;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AST = AIMS.Libraries.Scripting.NRefactory.Ast;
using CSTokens = AIMS.Libraries.Scripting.NRefactory.Parser.CSharp.Tokens;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    public class CSharpCompletionBinding : NRefactoryCodeCompletionBinding
    {
        public CSharpCompletionBinding() : base(SupportedLanguage.CSharp)
        {
        }

        public override bool HandleKeyPress(CodeEditorControl editor, char ch)
        {
            CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
            int cursor = editor.ActiveViewControl.Caret.Offset;
            ExpressionContext context = null;
            if (ch == '(')
            {
                if (CodeCompletionOptions.KeywordCompletionEnabled)
                {
                    switch (editor.ActiveViewControl.Caret.CurrentWord.Text.Trim())
                    {
                        case "for":
                        case "lock":
                            context = ExpressionContext.Default;
                            break;

                        case "using":
                            context = ExpressionContext.TypeDerivingFrom(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.IDisposable"), false);
                            break;

                        case "catch":
                            context = ExpressionContext.TypeDerivingFrom(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.Exception"), false);
                            break;

                        case "foreach":
                        case "typeof":
                        case "sizeof":
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
            else if (ch == '[')
            {
                Word curWord = editor.ActiveViewControl.Caret.CurrentWord;
                //Word preWord =  curWord.Row.FormattedWords[curWord.Index-1];
                //int lineOffset = editor.ActiveViewControl.Document.Text.Substring(0, editor.ActiveViewControl.Caret.Offset - editor.ActiveViewControl.Caret.Position.X).Length;
                //TextPoint tp = editor.ActiveViewControl.Document.IntPosToPoint(lineOffset);
                if (curWord.Text.Length == 0)
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
                            //if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.MulticastDelegate"))) {
                            if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass("System.Delegate")))
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
            else if (ch == ';')
            {
                string curLine = editor.ActiveViewControl.Caret.CurrentRow.Text;
                // don't return true when inference succeeds, otherwise the ';' won't be added to the document.
                TryDeclarationTypeInference(editor, curLine);
            }

            return base.HandleKeyPress(editor, ch);
        }

        private bool TryDeclarationTypeInference(CodeEditorControl editor, string curLine)
        {
            string lineText = curLine;
            ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.CSharp, new System.IO.StringReader(lineText));
            Token typeToken = lexer.NextToken();
            if (typeToken.kind == CSTokens.Question)
            {
                if (lexer.NextToken().kind == CSTokens.Identifier)
                {
                    Token t = lexer.NextToken();
                    if (t.kind == CSTokens.Assign)
                    {
                        string expr = lineText.Substring(t.col);
                        //LoggingService.Debug("DeclarationTypeInference: >" + expr + "<");
                        ResolveResult rr = ScriptControl.Parser.ProjectParser.GetResolver().Resolve(new ExpressionResult(expr),
                                                                 editor.ActiveViewControl.Caret.Position.Y + 1,
                                                                 t.col, editor.ActiveViewControl.FileName,
                                                                 ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName));

                        if (rr != null && rr.ResolvedType != null)
                        {
                            ClassFinder context = new ClassFinder(editor.ActiveViewControl.FileName, editor.ActiveViewControl.Caret.Position.Y, t.col);
                            if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
                                CSharpAmbience.Instance.ConversionFlags = ConversionFlags.None;
                            else
                                CSharpAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
                            string typeName = CSharpAmbience.Instance.Convert(rr.ResolvedType);

                            //editor.Document.Replace(curLine.Offset + typeToken.col - 1, 1, typeName);
                            int offset = editor.ActiveViewControl.Document.GetRange(new TextRange(0, 0, 0, editor.ActiveViewControl.Caret.Position.Y)).Length;

                            editor.ActiveViewControl.Document.InsertText(typeName, offset + typeToken.col - 1, editor.ActiveViewControl.Caret.Position.Y);
                            editor.ActiveViewControl.Caret.SetPos(new TextPoint(editor.ActiveViewControl.Caret.Position.X + typeName.Length - 1, editor.ActiveViewControl.Caret.Position.Y));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool IsInComment(CodeEditorControl editor)
        {
            CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
            int cursor = editor.ActiveViewControl.Caret.Offset - 1;
            return ef.FilterComments(ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor + 1), ref cursor) == null;
        }

        public override bool HandleKeyword(CodeEditorControl editor, string word)
        {
            // TODO: Assistance writing Methods/Fields/Properties/Events:
            // use public/static/etc. as keywords to display a list with other modifiers
            // and possible return types.
            switch (word)
            {
                case "using":
                    if (IsInComment(editor)) return false;

                    // TODO: check if we are inside class/namespace
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Namespace), ' ');
                    return true;

                case "as":
                case "is":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
                    return true;

                case "override":
                    if (IsInComment(editor)) return false;
                    editor.ActiveViewControl.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
                    return true;

                case "new":
                    return ShowNewCompletion(editor);

                case "case":
                    if (IsInComment(editor)) return false;
                    return DoCaseCompletion(editor);

                case "return":
                    if (IsInComment(editor)) return false;
                    IMember m = GetCurrentMember(editor);
                    if (m != null)
                    {
                        return ProvideContextCompletion(editor, m.ReturnType, ' ');
                    }
                    else
                    {
                        goto default;
                    }
                default:
                    return base.HandleKeyword(editor, word);
            }
        }

        private bool ShowNewCompletion(CodeEditorControl editor)
        {
            CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.ActiveViewControl.FileName);
            int cursor = editor.ActiveViewControl.Caret.Offset;
            ExpressionContext context = ef.FindExpression(ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName).Substring(0, cursor) + " T.", cursor + 2).Context;
            if (context.IsObjectCreation)
            {
                editor.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ' ');
                return true;
            }
            return false;
        }

        #region "case"-keyword completion

        private bool DoCaseCompletion(CodeEditorControl editor)
        {
            Caret caret = editor.ActiveViewControl.Caret;
            NRefactoryResolver r = new NRefactoryResolver(ScriptControl.Parser.ProjectParser.CurrentProjectContent, LanguageProperties.CSharp);
            if (r.Initialize(editor.ActiveViewControl.FileName, caret.Position.Y + 1, caret.Position.X + 1))
            {
                AST.INode currentMember = r.ParseCurrentMember(ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName));
                if (currentMember != null)
                {
                    CaseCompletionSwitchFinder ccsf = new CaseCompletionSwitchFinder(caret.Position.Y + 1, caret.Position.X + 1);
                    currentMember.AcceptVisitor(ccsf, null);
                    if (ccsf.bestStatement != null)
                    {
                        r.RunLookupTableVisitor(currentMember);
                        ResolveResult rr = r.ResolveInternal(ccsf.bestStatement.SwitchExpression, ExpressionContext.Default);
                        if (rr != null && rr.ResolvedType != null)
                        {
                            return ProvideContextCompletion(editor, rr.ResolvedType, ' ');
                        }
                    }
                }
            }
            return false;
        }

        private class CaseCompletionSwitchFinder : AbstractAstVisitor
        {
            private Location _caretLocation;
            internal AST.SwitchStatement bestStatement;

            public CaseCompletionSwitchFinder(int caretLine, int caretColumn)
            {
                _caretLocation = new Location(caretColumn, caretLine);
            }

            public override object VisitSwitchStatement(AST.SwitchStatement switchStatement, object data)
            {
                if (switchStatement.StartLocation < _caretLocation && _caretLocation < switchStatement.EndLocation)
                {
                    bestStatement = switchStatement;
                }
                return base.VisitSwitchStatement(switchStatement, data);
            }
        }

        #endregion "case"-keyword completion
    }
}