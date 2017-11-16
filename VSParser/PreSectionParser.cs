using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VSProvider
{
    public class PreSectionParser : SolutionItemParserBase<PreSection>
    {
        private static readonly Regex s_globalPattern = new Regex(@"(.*?)Project", RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex s_entryPattern = new Regex(@"^\s*(?<key>.*?)=(?<value>.*?)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        public PreSectionParser(string solutionContents) : base(solutionContents)
        {
        }

        public List<PreSection> Parse()
        {
            var match = s_globalPattern.Match(SolutionContents);
            List<PreSection> sections = new List<PreSection>();
            while (match.Success)
            {
                var section = CreatePreSectionFromMatch(match);
                sections.Add(section);
                PopulateSectionEntries(match, section);
                //return section;
                match = match.NextMatch();
            }
            return sections;
        }

        private static void PopulateSectionEntries(Match match, PreSection section)
        {
            var content = match.Groups[0].Value;
            var entryMatch = s_entryPattern.Match(content);
            while (entryMatch.Success)
            {
                var key = entryMatch.Groups["key"].Value.Trim();
                var value = entryMatch.Groups["value"].Value.Trim();
                section.Entries.Add(key, value);
                entryMatch = entryMatch.NextMatch();
            }
        }

        private static PreSection CreatePreSectionFromMatch(Match match)
        {
            //var sectionType = (match.Groups["type"].Value == "preSolution")
            //                      ? GlobalSectionType.PreSolution
            //                      : GlobalSectionType.PostSolution;
            var section = new PreSection("name");
            return section;
        }
    }
}