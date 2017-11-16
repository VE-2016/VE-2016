﻿using System.IO;

namespace VSProvider
{
    public class SolutionParser : ISolutionParser
    {
        private readonly string _solutionContents;

        public SolutionParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("Solution file {0} does not exist", path));
            using (var reader = new StreamReader(path))
            {
                _solutionContents = reader.ReadToEnd();

                reader.Close();
            }
        }

        public ISolution Parse()
        {
            return new Solution
            {
                preSections = (new PreSectionParser(_solutionContents)).Parse(),
                Global = (new GlobalSectionParser(_solutionContents)).Parse(),
                Projects = (new ProjectParser(_solutionContents)).Parse()
            };
        }

        public static ISolution Parse(string path)
        {
            var parser = new SolutionParser(path);
            return parser.Parse();
        }
    }
}