using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    public class EventHandlerCompletitionDataProvider //: AbstractCompletionDataProvider
    {
        private string _expression;
        private ResolveResult _resolveResult;
        private IClass _resolvedClass;

        public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
        {
            _expression = expression;
            _resolveResult = resolveResult;
            _resolvedClass = resolveResult.ResolvedType.GetUnderlyingClass();
            if (_resolvedClass == null && resolveResult.ResolvedType.FullyQualifiedName.Length > 0)
            {
                _resolvedClass = ScriptControl.Parser.ProjectParser.CurrentProjectContent.GetClass(resolveResult.ResolvedType.FullyQualifiedName);
            }
        }

    
        private class DelegateCompletionData// : DefaultCompletionData
        {
            private int _cursorOffset;

    
        }
    }
}