using ICSharpCode.NRefactory.CSharp.Resolver;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    internal class CSharpCompletionDataFactory
    {
        private object completionContext;
        private CSharpResolver cSharpResolver;

        public CSharpCompletionDataFactory(object completionContext, CSharpResolver cSharpResolver)
        {
            this.completionContext = completionContext;
            this.cSharpResolver = cSharpResolver;
        }
    }
}