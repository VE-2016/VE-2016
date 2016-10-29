// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using System;
using System.Collections.Generic;
using System.IO;
using CSTokens = AIMS.Libraries.Scripting.NRefactory.Parser.CSharp.Tokens;
using VBTokens = AIMS.Libraries.Scripting.NRefactory.Parser.VB.Tokens;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    /// <summary>
    /// Base class for C# and VB Code Completion Binding.
    /// </summary>
    public abstract class NRefactoryCodeCompletionBinding : DefaultCodeCompletionBinding
    {
        private readonly SupportedLanguage _language;
        private readonly int _eofToken,_commaToken,_openParensToken,_closeParensToken,_openBracketToken,_closeBracketToken,_openBracesToken,_closeBracesToken;
        private readonly LanguageProperties _languageProperties;

        protected NRefactoryCodeCompletionBinding(SupportedLanguage language)
        {
            _language = language;
            if (language == SupportedLanguage.CSharp)
            {
                _eofToken = CSTokens.EOF;
                _commaToken = CSTokens.Comma;
                _openParensToken = CSTokens.OpenParenthesis;
                _closeParensToken = CSTokens.CloseParenthesis;
                _openBracketToken = CSTokens.OpenSquareBracket;
                _closeBracketToken = CSTokens.CloseSquareBracket;
                _openBracesToken = CSTokens.OpenCurlyBrace;
                _closeBracesToken = CSTokens.CloseCurlyBrace;

                _languageProperties = LanguageProperties.CSharp;
            }
            else
            {
                _eofToken = VBTokens.EOF;
                _commaToken = VBTokens.Comma;
                _openParensToken = VBTokens.OpenParenthesis;
                _closeParensToken = VBTokens.CloseParenthesis;
                _openBracketToken = -1;
                _closeBracketToken = -1;
                _openBracesToken = VBTokens.OpenCurlyBrace;
                _closeBracesToken = VBTokens.CloseCurlyBrace;

                _languageProperties = LanguageProperties.VBNet;
            }
        }

        #region Comma Insight refresh

        protected class InspectedCall
        {
            /// <summary>
            /// position of the '('
            /// </summary>
            internal Location start;

            /// <summary>
            /// list of location of the comma tokens.
            /// </summary>
            internal List<Location> commas = new List<Location>();

            /// <summary>
            /// reference back to parent call - used to create a stack of inspected calls
            /// </summary>
            internal InspectedCall parent;

            public InspectedCall(Location start, InspectedCall parent)
            {
                this.start = start;
                this.parent = parent;
            }
        }

        protected IList<ResolveResult> ResolveCallParameters(CodeEditorControl editor, InspectedCall call)
        {
            List<ResolveResult> rr = new List<ResolveResult>();
            int offset = LocationToOffset(editor, call.start);

            string documentText = ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName);
            offset = documentText.LastIndexOf('(');
            int newOffset;
            foreach (Location loc in call.commas)
            {
                newOffset = LocationToOffset(editor, loc);
                if (newOffset < 0) break;
                TextPoint start = editor.ActiveViewControl.Document.IntPosToPoint(offset);
                TextPoint end = editor.ActiveViewControl.Document.IntPosToPoint(newOffset);

                TextRange tr = new TextRange(start.X, start.Y, end.X, end.Y);
                string text = editor.ActiveViewControl.Document.GetRange(tr);//.GetText(offset+1,newOffset-(offset+1));
                rr.Add(ScriptControl.Parser.ProjectParser.GetResolver().Resolve(new ExpressionResult(text), loc.Line, loc.Column, editor.ActiveViewControl.FileName, documentText));
            }
            // the last argument is between the last comma and the caret position
            newOffset = editor.ActiveViewControl.Caret.Offset;
            if (offset < newOffset)
            {
                TextPoint start = editor.ActiveViewControl.Document.IntPosToPoint(offset);
                TextPoint end = editor.ActiveViewControl.Document.IntPosToPoint(newOffset);

                TextRange tr = new TextRange(start.X, start.Y, end.X, end.Y);

                string text = editor.ActiveViewControl.Document.GetRange(tr);//(offset+1,newOffset-(offset+1));
                rr.Add(ScriptControl.Parser.ProjectParser.GetResolver().Resolve(new ExpressionResult(text),
                                             editor.ActiveViewControl.Caret.Position.Y + 1,
                                             editor.ActiveViewControl.Caret.Position.X + 1,
                                             editor.ActiveViewControl.FileName, documentText));
            }
            return rr;
        }

        protected bool InsightRefreshOnComma(CodeEditorControl editor, char ch)
        {
            // Show MethodInsightWindow or IndexerInsightWindow
            NRefactoryResolver r = new NRefactoryResolver(ScriptControl.Parser.ProjectParser.CurrentProjectContent, _languageProperties);
            Location cursorLocation = new Location(editor.ActiveViewControl.Caret.Position.X + 1, editor.ActiveViewControl.Caret.Position.Y + 1);
            if (r.Initialize(editor.ActiveViewControl.FileName, cursorLocation.Y, cursorLocation.X))
            {
                TextReader currentMethod = r.ExtractCurrentMethod(ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName));
                if (currentMethod != null)
                {
                    ILexer lexer = ParserFactory.CreateLexer(_language, currentMethod);
                    Token token;
                    InspectedCall call = new InspectedCall(Location.Empty, null);
                    call.parent = call;
                    while ((token = lexer.NextToken()) != null
                           && token.kind != _eofToken
                           && token.Location < cursorLocation)
                    {
                        if (token.kind == _commaToken)
                        {
                            call.commas.Add(token.Location);
                        }
                        else if (token.kind == _openParensToken || token.kind == _openBracketToken || token.kind == _openBracesToken)
                        {
                            call = new InspectedCall(token.Location, call);
                        }
                        else if (token.kind == _closeParensToken || token.kind == _closeBracketToken || token.kind == _closeBracesToken)
                        {
                            call = call.parent;
                        }
                    }
                    int offset = LocationToOffset(editor, call.start);
                    string docText = ScriptControl.Parser.ProjectParser.GetFileContents(editor.FileName);
                    offset = docText.LastIndexOf('(');
                    //int offset = editor.ActiveViewControl.Document.PointToIntPos(new TextPoint(call.start.X,call.start.Y));//, call.start);
                    if (offset >= 0 && offset < docText.Length)
                    {
                        char c = (char)docText.Substring(offset, 1).ToCharArray(0, 1)[0];
                        if (c == '(')
                        {
                            ShowInsight(editor,
                                        new MethodInsightDataProvider(offset, true),
                                        ResolveCallParameters(editor, call),
                                        ch);
                            return true;
                        }
                        else if (c == '[')
                        {
                            ShowInsight(editor,
                                        new IndexerInsightDataProvider(offset, true),
                                        ResolveCallParameters(editor, call),
                                        ch);
                            return true;
                        }
                        else
                        {
                            //LoggingService.Warn("Expected '(' or '[' at start position");
                        }
                    }
                }
            }
            return false;
        }

        #endregion Comma Insight refresh

        protected bool ProvideContextCompletion(CodeEditorControl editor, IReturnType expected, char charTyped)
        {
            if (expected == null) return false;
            IClass c = expected.GetUnderlyingClass();
            if (c == null) return false;
            if (c.ClassType == ClassType.Enum)
            {
                CtrlSpaceCompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
                cdp.ForceNewExpression = true;
                ContextCompletionDataProvider cache = new ContextCompletionDataProvider(cdp);
                cache.activationKey = charTyped;
                cache.GenerateCompletionData(editor.ActiveViewControl.FileName, editor.ActiveViewControl, charTyped);
                ICompletionData[] completionData = cache.CompletionData;
                Array.Sort(completionData);
                for (int i = 0; i < completionData.Length; i++)
                {
                    CodeCompletionData ccd = completionData[i] as CodeCompletionData;
                    if (ccd != null && ccd.Class != null)
                    {
                        if (ccd.Class.FullyQualifiedName == expected.FullyQualifiedName)
                        {
                            cache.DefaultIndex = i;
                            break;
                        }
                    }
                }

                if (cache.DefaultIndex >= 0)
                {
                    if (charTyped != ' ') cdp.InsertSpace = true;
                    editor.ActiveViewControl.ShowCompletionWindow(cache, charTyped);
                    return true;
                }
            }
            return false;
        }

        private class ContextCompletionDataProvider : CachedCompletionDataProvider
        {
            internal char activationKey;

            internal ContextCompletionDataProvider(ICompletionDataProvider baseProvider) : base(baseProvider)
            {
            }

            public override CompletionDataProviderKeyResult ProcessKey(char key)
            {
                if (key == '=' && activationKey == '=')
                    return CompletionDataProviderKeyResult.BeforeStartKey;
                activationKey = '\0';
                return base.ProcessKey(key);
            }
        }

        protected void ShowInsight(CodeEditorControl editor, MethodInsightDataProvider dp, ICollection<ResolveResult> parameters, char charTyped)
        {
            int paramCount = parameters.Count;
            dp.SetupDataProvider(editor.ActiveViewControl.FileName, editor.ActiveViewControl);
            List<IMethodOrProperty> methods = dp.Methods;
            if (methods.Count == 0) return;
            bool overloadIsSure;
            if (methods.Count == 1)
            {
                overloadIsSure = true;
                dp.DefaultIndex = 0;
            }
            else
            {
                IReturnType[] parameterTypes = new IReturnType[paramCount + 1];
                int i = 0;
                foreach (ResolveResult rr in parameters)
                {
                    if (rr != null)
                    {
                        parameterTypes[i] = rr.ResolvedType;
                    }
                    i++;
                }
                IReturnType[][] tmp;
                int[] ranking = MemberLookupHelper.RankOverloads(methods, parameterTypes, true, out overloadIsSure, out tmp);
                bool multipleBest = false;
                int bestRanking = -1;
                int best = 0;
                for (i = 0; i < ranking.Length; i++)
                {
                    if (ranking[i] > bestRanking)
                    {
                        bestRanking = ranking[i];
                        best = i;
                        multipleBest = false;
                    }
                    else if (ranking[i] == bestRanking)
                    {
                        multipleBest = true;
                    }
                }
                if (multipleBest) overloadIsSure = false;
                dp.DefaultIndex = best;
            }
            editor.ActiveViewControl.ShowInsightWindow(dp);
            if (overloadIsSure)
            {
                IMethodOrProperty method = methods[dp.DefaultIndex];
                if (paramCount < method.Parameters.Count)
                {
                    IParameter param = method.Parameters[paramCount];
                    ProvideContextCompletion(editor, param.ReturnType, charTyped);
                }
            }
        }

        protected int LocationToOffset(CodeEditorControl editor, Location loc)
        {
            if (loc.IsEmpty || loc.Line - 1 >= editor.ActiveViewControl.Document.Lines.Length)
                return -1;
            string seg = editor.ActiveViewControl.Document.Lines[loc.Line - 1];
            TextRange tr = new TextRange(0, 0, 0, loc.Line);
            int tl = editor.ActiveViewControl.Document.GetRange(tr).Length;
            return tl + Math.Min(loc.Column, seg.Length) - 1;
        }

        protected IMember GetCurrentMember(CodeEditorControl editor)
        {
            Caret caret = editor.ActiveViewControl.Caret;
            NRefactoryResolver r = new NRefactoryResolver(ScriptControl.Parser.ProjectParser.CurrentProjectContent, _languageProperties);
            if (r.Initialize(editor.ActiveViewControl.FileName, caret.Position.Y + 1, caret.Position.X + 1))
            {
                return r.CallingMember;
            }
            else
            {
                return null;
            }
        }
    }
}