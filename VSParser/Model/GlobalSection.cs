using System.Collections.Generic;

namespace VSProvider
{
    public class PreSection : ISolutionItem
    {
        public PreSection(string name)
        {
            Name = name;
            //Type = sectionType;
            Entries = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public PreSectionType Type { get; set; }
        public IDictionary<string, string> Entries { get; set; }
    }

    public class GlobalSection : ISolutionItem
    {
        public GlobalSection(string name, GlobalSectionType sectionType)
        {
            Name = name;
            Type = sectionType;
            Entries = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public GlobalSectionType Type { get; set; }
        public IDictionary<string, string> Entries { get; set; }
    }

    public enum GlobalSectionType
    {
        PostSolution,
        PreSolution
    }

    public enum PreSectionType
    {
        PostSolution,
        PreSolution
    }
}