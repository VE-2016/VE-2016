using System.Collections.Generic;
using System.Linq;

namespace VSProvider
{
    public class Solution : ISolution
    {
        public PreSection preSection { get; set; }
        public List<PreSection> preSections { get; set; }
        public IEnumerable<GlobalSection> Global { get; set; }
        public IEnumerable<Project> Projects { get; set; }

        public PreSection GetPresection(string presection, string sections)
        {
            foreach (PreSection section in preSections)
            {
                if (section.Entries.Count <= 0)
                    continue;
                if (section.Entries.Any(s => s.Key.Contains(presection) && s.Value == sections))
                    return section;
            }
            return null;
        }
    }
}