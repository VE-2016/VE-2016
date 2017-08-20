// VSSolution

using Microsoft.Build.Construction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VSProvider
{
    public class VSSolution
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        private static readonly Type s_SolutionParser;
        private static readonly MethodInfo s_SolutionParser_parseSolution;
        //private static readonly PropertyInfo s_SolutionParser_projects;
        private static readonly FieldInfo s_SolutionParser_projects;
        private static readonly PropertyInfo s_SolutionParser_solutionReader;

        public List<VSProject> projects;
        public string solutionFileName;

        public Dictionary<string, VSProject> dd { get; set; }

        static VSSolution()
        {

            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            //s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            

            
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionFile, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetField("_projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
        }

        public VSSolution()
        {
        }

        public VSProject GetProjectbyName(string name)
        {
            foreach (VSProject p in projects)
                if (p.Name == name)
                    return p;

            return null;
        }

        public VSProject GetProjectbyExec(string name)
        {
            foreach (VSProject p in projects)
                if (p.GetProjectExec() == name)
                    return p;

            return null;
        }
        public string GetProjectExec(string name)
        {
            VSProject p = GetProjectbyName(name);
            if(p != null)
            return p.GetProjectExec();

            
            return "";
        }
        public VSProject GetProjectbyFileName(string name)
        {
            foreach (VSProject p in projects)
                if (p.FileName == name)
                    return p;

            return null;
        }

        public VSProject GetProjectbyCompileItem(string name)
        {
            foreach (VSProject p in projects)
            {
                ArrayList C = p.GetCompileItems();

                foreach (string file in C)
                    if (file == name)
                        return p;
            }

            return null;
        }

        public VSProject MainVSProject { get; set; }

        public SolutionFile sf { get; set; }

        public VSSolution(string solutionFileName)
        {
            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }

            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            //var streamReader = new StreamReader(solutionFileName);
            //{
            //    s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
            //    s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            //}

            sf = (SolutionFile)s_SolutionParser_parseSolution.Invoke(solutionParser, new[] { solutionFileName });

            this.solutionFileName = solutionFileName;

            projects = new List<VSProject>();
            //var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
            //var array = (Array)s_SolutionParser_projects.GetValue(sf);
            var array = (Dictionary<string, ProjectInSolution>)s_SolutionParser_projects.GetValue(sf);
            //for (int i = 0; i < array.Keys.Count; i++)
            //{
            //    projects.Add(new VSProject(this, array.GetValue(i)));
            //}


            foreach (string s in array.Keys)
            {
                projects.Add(new VSProject(this, array[s]));
            }
            //streamReader.Close();
            //streamReader.Dispose();
        }

        public VSProject GetVSProject(string file)
        {
            file = file.Replace("\\\\", "\\");

            foreach (VSProject p in this.Projects)
            {
                if (file == p.FileName)
                    return p;

                ArrayList L = p.GetCompileItems();

                int i = L.IndexOf(file);

                if (i >= 0)
                    return p;
            }

            return null;
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
                projects = (System.Collections.Generic.List<VSProject>)value;
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

     

   
   
       

       

       

  
        public Dictionary<VSProject, ArrayList> GetDataSources()
        {
            Dictionary<VSProject, ArrayList> dict = new Dictionary<VSProject, ArrayList>();
            foreach (VSProject vp in this.projects)
            {
                vp.GetDataSources(dict);
            }

            return dict;
        }

        public void Dispose()
        {
        }

        public void RestorePackages(bool restore)
        {
            foreach (VSProject vp in this.projects)
            {
                vp.SetRestorePackages(restore);
            }
        }

        public enum OutputType
        {
            exec,
            dlls,
            both
        }

        public ArrayList GetExecutables(OutputType output = OutputType.dlls)
        {
            ArrayList E = new ArrayList();
            foreach (VSProject p in projects)
            {
                string s = p.GetProjectExec();

                if (Directory.Exists(s) == false)
                {
                    if (output == OutputType.dlls)
                    {
                        if (Path.GetExtension(s) != ".exe")
                            if (s != "")
                                E.Add(s);
                    }
                    else if (output == OutputType.exec)
                    {
                        if (Path.GetExtension(s) == ".exe")
                            if (s != "")
                                E.Add(s);
                    }
                    else
                    {
                        if (s != "")
                            E.Add(s);
                    }
                }
            }

            return E;
        }

        public ArrayList GetDependencyOrder(ArrayList T)
        {
            ArrayList L = new ArrayList();

            foreach (VSProject p in this.projects)
            {
                bool added = false;

                ArrayList D = p.GetProjectReferences("ProjectReference");

                L.Add(D);

                T.Add(D);

                int i = 0;
                foreach (ArrayList d in T)
                {
                    if (d.Count <= D.Count)
                    {
                        T.Insert(i, D);
                        added = true;
                        break;
                    }
                    i++;
                }
                if (added == false)
                    T.Add(D);
            }

            return L;
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

            if (content != "")

                text = text.Replace(content, "");

            File.WriteAllText(solutionFileName, text);
        }

        public ArrayList GetSolutionPlatforms()
        {
            ArrayList L = new ArrayList();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.GlobalSection se in s.Global)
            {
                if (se.Name == "SolutionConfigurationPlatforms")
                {
                    foreach (string p in se.Entries.Keys)
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

        public static ArrayList GetSubstringByString(int start, string a, string b, string c)
        {
            ArrayList S = new ArrayList();

            int s = 0;

            while (s >= 0)
            {
                s = c.IndexOf(a, start);

                int g = (c.IndexOf(b, start) - c.IndexOf(a, start) - a.Length);

                if (s < 0)
                    return S;
                if (g < 0)
                    return S;

                string d = c.Substring((s + a.Length), g);

                start = s + g + b.Length;

                S.Add(d);
            }

            return S;
        }

        public static string GetSubstring(ref int start, string a, string b, string c)
        {
            string w = "";

            int s = 0;

            s = c.IndexOf(a, start);

            if (s < 0)
                return w;

            int g = (c.IndexOf(b, s + 2) - c.IndexOf(a, start) - a.Length);

            if (g < 0)
                return w;

            string d = c.Substring((s + a.Length), g);

            start = s + g + b.Length;

            return d;
        }

        public static ArrayList GetVSProjects(string file)
        {
            ArrayList C = new ArrayList();

            string content = File.ReadAllText(file);

            ArrayList S = GetSubstringByString(0, "Project(", "EndProject\r", content);

            ArrayList F = new ArrayList();

            foreach (string s in S)
            {
                string[] cc = s.Split("\n".ToCharArray());

                string c = cc[0];

                vsproject v = new vsproject();

                v.content = content;

                v.project = s;

                v.Load(c);

                F.Add(v);
            }

            return F;
        }

        public static void CreateVSolutionFile(string file, string sf)
        {
            string content = File.ReadAllText(file);

            FileStream b = File.Create(sf);
            b.Close();

            File.WriteAllText(sf, content);
        }

        public void MockProject()
        {
            ArrayList S = VSSolution.GetVSProjects(solutionFileName);

            string content = File.ReadAllText(solutionFileName);

            foreach (vsproject v in S)
            {
                v.content = content;

                string n = v.name.Replace("\"", "");

                n = n.Trim();

                v.name = v.name.Replace(n, '_' + n);

                v.file = v.file.Replace(n + ".", '_' + n + ".");

                string g = v.GetProject();

                g = v.ModifyProject(g);

                content = g;
            }

            string file = solutionFileName;

            string name = Path.GetFileName(file).Trim();

            file = file.Replace(name, "_" + name);

            File.WriteAllText(file, content);
        }
    }

    public class vsproject
    {
        public static string pattern = "Project(\"{%g}\") = \"%n\", \"%f\", \"{%s}\"";

        public string content { get; set; }

        public string project { get; set; }

        public string guid { get; set; }
        public string name { get; set; }
        public string file { get; set; }
        public string guids { get; set; }

        public string Extract(string g)
        {
            g = g.Replace("\"", "");
            g = g.Replace("{", "");
            g = g.Replace("}", "");
            g = g.Replace("(", "");
            g = g.Replace(")", "");
            g = g.Replace("Project", "");

            return g.Trim();
        }

        public void Load(string s)
        {
            project = s;

            string[] cc = s.Split("\n".ToCharArray());

            string c = cc[0];

            cc = c.Split("=".ToCharArray());

            string g = Extract(cc[0]);

            //g = g.Replace("\"", "");
            //g = g.Replace("{", "");
            //g = g.Replace("}", "");
            //g = g.Replace("(", "");
            //g = g.Replace(")", "");
            //g = g.Replace("Project", "");

            guid = g;

            string[] dd = cc[1].Split(",".ToCharArray());
            name = Extract(dd[0]);
            file = Extract(dd[1]);
            guids = Extract(dd[2]);
        }

        public void RemoveProject()
        {
            content = content.Replace(project, "");
        }

        public string GetProject()
        {
            string s = pattern;
            s = s.Replace("%g", guid);
            s = s.Replace("%n", name);
            s = s.Replace("%f", file);
            s = s.Replace("%s", guids);

            return s;
        }

        public string ModifyProject(string s)
        {
            content = content.Replace(project, s);

            return content;
        }
    }

    public class projects
    {
        public ArrayList P { get; set; }
    }
}