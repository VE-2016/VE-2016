using System.Collections.Generic;

namespace VSProvider
{
    public class Solution : ISolution
    {
        public PreSection preSection { get; set; }
        public IEnumerable<GlobalSection> Global { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}