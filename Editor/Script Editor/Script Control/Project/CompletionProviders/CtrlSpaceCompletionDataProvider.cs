using System;
using System.Collections;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public class CtrlSpaceCompletionDataProvider : CodeCompletionDataProvider
    {
        public CtrlSpaceCompletionDataProvider()
        {
        }

        public CtrlSpaceCompletionDataProvider(ExpressionContext overrideContext)
        {
            this.overrideContext = overrideContext;
        }

        private bool _forceNewExpression;

        /// <summary>
        /// Gets/Sets whether the CtrlSpaceCompletionDataProvider creates a new completion
        /// dropdown instead of completing an old expression.
        /// Default value is false.
        /// </summary>
        public bool ForceNewExpression
        {
            get
            {
                return _forceNewExpression;
            }
            set
            {
                _forceNewExpression = value;
            }
        }

        protected override void GenerateCompletionData(EditViewControl textArea, char charTyped)
        {
            if (_forceNewExpression)
            {
                preSelection = "";
                if (charTyped != '\0')
                {
                    preSelection = null;
                }
                ExpressionContext context = overrideContext;
                if (context == null) context = ExpressionContext.Default;
                NRefactoryResolver rr = Parser.ProjectParser.GetResolver();
                AddResolveResults(rr.CtrlSpace(caretLineNumber, caretColumn, fileName, Parser.ProjectParser.GetFileContents(fileName), context), context);
                return;
            }

            ExpressionResult expressionResult = GetExpression(textArea);
            string expression = expressionResult.Expression;
            preSelection = null;
            if (expression == null || expression.Length == 0)
            {
                preSelection = "";
                if (charTyped != '\0')
                {
                    preSelection = null;
                }
                NRefactoryResolver rr = Parser.ProjectParser.GetResolver();
                AddResolveResults(rr.CtrlSpace(caretLineNumber, caretColumn, fileName, Parser.ProjectParser.GetFileContents(fileName), expressionResult.Context), expressionResult.Context);
                return;
            }

            int idx = expression.LastIndexOf('.');
            if (idx > 0)
            {
                preSelection = expression.Substring(idx + 1);
                expressionResult.Expression = expression.Substring(0, idx);
                if (charTyped != '\0')
                {
                    preSelection = null;
                }
                GenerateCompletionData(textArea, expressionResult);
            }
            else
            {
                preSelection = expression;
                if (charTyped != '\0')
                {
                    preSelection = null;
                }

                ArrayList results = Parser.ProjectParser.GetResolver().CtrlSpace(caretLineNumber, caretColumn, fileName, Parser.ProjectParser.GetFileContents(fileName), expressionResult.Context);
                AddResolveResults(results, expressionResult.Context);
            }
        }
    }
}
