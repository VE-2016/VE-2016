using System.Collections.Generic;

namespace VSProvider
{
    public interface ISolution
    {
        PreSection preSection { get; }

        IEnumerable<GlobalSection> Global { get; }
        IEnumerable<Project> Projects { get; }
    }
}