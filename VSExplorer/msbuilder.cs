// VSSolution

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Reflection;


namespace VSProvider
{
    public class VSSolution
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        private static readonly Type s_SolutionParser;
        private static readonly MethodInfo s_SolutionParser_parseSolution;
        private static readonly PropertyInfo s_SolutionParser_projects;
        private static readonly PropertyInfo s_SolutionParser_solutionReader;
        
        private List<VSProject> projects;
        public string solutionFileName;

        static VSSolution()
        {
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);

           

        }

        public VSSolution()
        {

        }

        public VSSolution(string solutionFileName)
        {

           

            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }

            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            var streamReader = new StreamReader(solutionFileName);
            {
                s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                s_SolutionParser_parseSolution.Invoke(solutionParser, null);
               
                
            }

            this.solutionFileName = solutionFileName;

            projects = new List<VSProject>();
            var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);

            for (int i = 0; i < array.Length; i++)
            {
                projects.Add(new VSProject(this, array.GetValue(i)));
            }

            streamReader.Close();

            streamReader.Dispose();
        }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(solutionFileName);
            }
        }

        public IEnumerable<VSProject> Projects
        {
            get
            {
                return projects;
            }
            set
            {
                projects = (System.Collections.Generic.List<VSProject>) value;
            }
        }

        public string SolutionPath
        {
            get
            {
                var file = new FileInfo(solutionFileName);

                return file.DirectoryName;
            }
        }

        public void Dispose()
        {
        }

        public string GetProject(string name)
        {

            var solution = VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.Project p in solution.Projects)
            {

                string s = Path.GetDirectoryName(solutionFileName) + "\\" + p.Path; 

                if (s == name)
                {

                    return p.content;

                }

            }

            return "";
        }

        public void RemoveProject(string name)
        {

            string content = GetProject(name);


            string text = File.ReadAllText(solutionFileName);


            text = text.Replace(content, "");

            File.WriteAllText(solutionFileName, text);




        }


   

        public ArrayList GetSolutionPlatforms()
        {
            ArrayList L = new ArrayList();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.GlobalSection se in s.Global)
            {

                if(se.Name == "SolutionConfigurationPlatforms") {


                    foreach(string p in se.Entries.Keys)
                        L.Add(p);

                }


            }

            return L;

        }


        public Dictionary<string, string> GetProjectGuids()
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.Project p in s.Projects)
            {


                if (dict.ContainsKey(p.Guid.ToString()) == true)
                    continue;

                dict.Add(p.Guid.ToString().ToUpper(), p.Name);


                
            }

            return dict;

        }




        public ArrayList GetProjectPlatforms()
        {
            ArrayList L = new ArrayList();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.GlobalSection se in s.Global)
            {

                if (se.Name == "ProjectConfigurationPlatforms")
                {


                    foreach (string p in se.Entries.Keys)
                        L.Add(p);

                }


            }

            return L;

        }


    }


}