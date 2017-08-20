﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VSProvider
{
    public class ProjectParser : SolutionItemParserBase<Project>
    {
        //        private static readonly Regex ProjectPattern = new Regex(@"Project\(\""(?<typeGuid>.*?)\""\)\s+=\s+\""(?<name>.*?)\"",*\s+\""(?<path>.*?)\"",*\s+\""(?<guid>.*?)\""(?<content>.*?)\bEndProject\b", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        private static readonly Regex s_projectPattern = new Regex(@"Project\(\""(?<typeGuid>.*?)\""\)\s*=\s*\""(?<name>.*?)\"",*\s*\""(?<path>.*?)\"",*\s*\""(?<guid>.*?)\""(?<content>.*?)\bEndProject\b", RegexOptions.ExplicitCapture | RegexOptions.Singleline);

        private static readonly Regex s_sectionPattern = new Regex(@"ProjectSection\((?<name>.*?)\)\s+=\s+(?<type>.*?)\s+(?<entries>.*?)\bEndProjectSection\b", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex s_entryPattern = new Regex(@"^\s*(?<key>.*?)=(?<value>.*?)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);

        public ProjectParser(string solutionContents) : base(solutionContents)
        {
        }

        public new IEnumerable<Project> Parse()
        {
            var match = s_projectPattern.Match(SolutionContents);
            while (match.Success)
            {
                var project = CreateProjectFromMatch(match);
                ParseProjectContent(match, project);
                yield return project;
                match = match.NextMatch();
            }
        }

        private static void ParseProjectContent(Match match, Project project)
        {
            var content = match.Groups["content"].Value.Trim();
            if (string.IsNullOrEmpty(content)) return;
            var sectionMatch = s_sectionPattern.Match(content);
            ProjectSection section = null;// CreateProjectSectionFromMatch(sectionMatch);
            while (sectionMatch.Success)
            {
                if (section == null)
                {
                    section = CreateProjectSectionFromMatch(sectionMatch);
                    project.ProjectSection = section;
                }
                ParseSectionEntries(sectionMatch, section);
                sectionMatch = sectionMatch.NextMatch();
            }
        }

        private static ProjectSection CreateProjectSectionFromMatch(Match sectionMatch)
        {
            var projectType = (sectionMatch.Groups["type"].Value == "preProject")
                                  ? ProjectSectionType.PreProject
                                  : ProjectSectionType.PostProject;
            var section = new ProjectSection(sectionMatch.Groups["name"].Value, projectType);
            return section;
        }

        private static void ParseSectionEntries(Match sectionMatch, ProjectSection section)
        {
            var entries = sectionMatch.Groups["entries"].Value;
            var entryMatch = s_entryPattern.Match(entries);
            while (entryMatch.Success)
            {
                var entryKey = entryMatch.Groups["key"].Value.Trim();
                var entryValue = entryMatch.Groups["value"].Value.Trim();
                section.Entries[entryKey] = entryValue;
                entryMatch = entryMatch.NextMatch();
            }
        }

        private static Project CreateProjectFromMatch(Match match)
        {
            var typeGuid = new Guid(match.Groups["typeGuid"].Value);
            var guid = new Guid(match.Groups["guid"].Value);
            var project = new Project(typeGuid, match.Groups["name"].Value, match.Groups["path"].Value, guid);
            project.s = match.Index;
            project.d = match.Length;
            project.content = match.Value;
            return project;
        }
    }
}