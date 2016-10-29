using System.Collections.Generic;

namespace VSProvider
{
    public interface ISolutionItemParser<out T> where T : ISolutionItem
    {
        IEnumerable<T> Parse();
    }
}