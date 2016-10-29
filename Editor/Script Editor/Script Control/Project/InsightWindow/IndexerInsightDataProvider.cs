

using System;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.CodeEditor.Syntax;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public class IndexerInsightDataProvider : MethodInsightDataProvider
    {
        /// <summary>
        /// Creates a IndexerInsightDataProvider looking at the caret position.
        /// </summary>
        public IndexerInsightDataProvider() { }

        /// <summary>
        /// Creates a IndexerInsightDataProvider looking at the specified position.
        /// </summary>
        public IndexerInsightDataProvider(int lookupOffset, bool setupOnlyOnce) : base(lookupOffset, setupOnlyOnce) { }

        protected override void SetupDataProvider(string fileName, SyntaxDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
        {
            ResolveResult result = Parser.ProjectParser.GetResolver().Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.Text);
            if (result == null)
                return;
            IReturnType type = result.ResolvedType;
            if (type == null)
                return;
            foreach (IProperty i in type.GetProperties())
            {
                if (i.IsIndexer)
                {
                    methods.Add(i);
                }
            }
        }
    }
}
