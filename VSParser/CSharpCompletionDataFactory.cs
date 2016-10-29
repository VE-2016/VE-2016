using ICSharpCode.NRefactory.CSharp.Resolver;

namespace VSParsers
{
    internal class CSharpCompletionDataFactory
    {
        private object _completionContext;
        private CSharpResolver _cSharpResolver;

        public CSharpCompletionDataFactory(object completionContext, CSharpResolver cSharpResolver)
        {
            _completionContext = completionContext;
            _cSharpResolver = cSharpResolver;
        }
    }
}