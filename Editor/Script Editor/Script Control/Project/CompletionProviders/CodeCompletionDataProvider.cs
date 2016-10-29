using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    /// <summary>
    /// Data provider for code completion.
    /// </summary>
    public class CodeCompletionDataProvider : AbstractCodeCompletionDataProvider
    {
        /// <summary>
        /// Initialize a CodeCompletionDataProvider that reads the expression from the text area.
        /// </summary>
        public CodeCompletionDataProvider()
        {
        }

        /// <summary>
        /// Initalize a CodeCompletionDataProvider with a fixed expression.
        /// </summary>
        public CodeCompletionDataProvider(ExpressionResult expression)
        {
            _fixedExpression = expression;
        }

        private ExpressionResult _fixedExpression;

        protected override void GenerateCompletionData(EditViewControl textArea, char charTyped)
        {
            preSelection = null;
            if (_fixedExpression.Expression == null)
                GenerateCompletionData(textArea, GetExpression(textArea));
            else
                GenerateCompletionData(textArea, _fixedExpression);
        }

        protected void GenerateCompletionData(EditViewControl textArea, ExpressionResult expressionResult)
        {
            // allow empty string as expression (for VB 'With' statements)
            if (expressionResult.Expression == null)
            {
                return;
            }
            string textContent = Parser.ProjectParser.GetFileContents(fileName);
            NRefactoryResolver rr = Parser.ProjectParser.GetResolver();
            ResolveResult r1 = rr.Resolve(expressionResult, caretLineNumber, caretColumn, textArea.FileName, Parser.ProjectParser.GetFileContents(fileName));
            AddResolveResults(r1,
                              expressionResult.Context);
        }
    }
}