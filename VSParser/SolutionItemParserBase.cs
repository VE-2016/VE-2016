using System.Collections.Generic;

namespace VSProvider
{
    abstract public class SolutionItemParserBase<T> : ISolutionItemParser<T> where T : ISolutionItem
    {
        protected readonly string SolutionContents;

        protected SolutionItemParserBase(string solutionContents)
        {
            SolutionContents = solutionContents;
        }

        public new IEnumerable<T> Parse()
        {
            throw new System.NotImplementedException();
        }
    }
}