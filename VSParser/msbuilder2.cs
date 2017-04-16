// VSProject

using GACManagerApi;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using VSParsers;


namespace VSProvider
{
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class VSProject
    {
        private static readonly Type s_ProjectInSolution;
        private static readonly Type s_RootElement;
        private static readonly Type s_ProjectRootElement;
        private static readonly Type s_ProjectRootElementCache;
        private static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        private static readonly PropertyInfo s_ProjectInSolution_ProjectType;
        private static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        private static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        private static readonly PropertyInfo s_ProjectRootElement_Items;
        private static readonly PropertyInfo s_ProjectRootElement_OutputType;
        private static readonly PropertyInfo s_ProjectRootElement_OutDir;

        private static readonly PropertyInfo s_ProjectRootElement_AssemblyName;

        public VSSolution solution;
        private string _projectFileName;
        private object _internalSolutionProject;
        private List<VSProjectItem> _items;

        public Dictionary<string, ITypeDefinition> dicts { get; set; }
        public static Dictionary<string, string> dc { get; set; }

        public string Name { get; set; }

        public string ProjectType { get; private set; }

        public string RelativePath { get; private set; }

        public string ProjectGuid { get; private set; }

        public VSSolution vs { get; set; }

        public Dictionary<string, string> dict { get; set; }

        public CSParsers csd { get; set; }

        public string OutputName { get; set; }

        public string FileName
        {
            get
            {
                
                return _projectFileName;
            }
            set
            {
                _projectFileName = value;
            }
        }

        public static Dictionary<string, Assembly> dcc = new Dictionary<string, Assembly>();

        public bool typeLoadingStarted = false;

        public CSParsers CSParsers()
        {


            if (csd == null)
            {
                if (typeLoadingStarted == false)
                {
                    typeLoadingStarted = true;

                    csd = new VSParsers.CSParsers();
                    ArrayList L = GetCompileItems();
                    ArrayList R = GetReferences();
                    Dictionary<string, AssemblyDescription> dc = GACProject.GACForm.asmd;
                    ArrayList A = new ArrayList();
                    foreach (string s in R)
                    {
                        string[] cc = s.Split(",".ToCharArray());

                        string c = cc[0];// cc[cc.Length - 1];

                        if (dcc.ContainsKey(c) == true)
                        {

                            Assembly asm = dcc[c];

                            A.Add(asm);
                        }

                        else
                        if (dc.ContainsKey(c) == true)
                        {
                            AssemblyDescription d = dc[c];

                            Assembly asm = Assembly.Load(d.DisplayName);

                            A.Add(asm);

                            dcc.Add(c, asm);
                        }
                        else
                        {
                            //string b = AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(s);
                            //if (!File.Exists(b))
                            //    continue;
                            //Assembly asm = Assembly.LoadFile(b);

                            //A.Add(asm);

                            //dcc.Add(c, asm);
                        }
                    }
                    if (dc.ContainsKey("mscorlib") == true)
                    {
                        AssemblyDescription d = dc["mscorlib"];

                        Assembly asm = Assembly.Load(d.DisplayName);

                        A.Add(asm);

                        //dcc.Add("mscorlib", asm);
                    }
                    
                    csd.AddProjectFiles(L, A);
                    csd.addProjectFiles(L, A);
                    ImportProjectTypes();

                    ArrayList P = this.GetProjectReferencesFiles("ProjectReference");

                    foreach (string name in P)
                    {
                        if (vs == null)
                            continue;

                        VSProject vp = vs.GetProjectbyFileName(name);

                        if (vp == null)
                            continue;

                        if (vp.csd == null)
                            vp.CSParsers();

                        var d = vp.csd.cmp.GetAllTypeDefinitions();

                        foreach (ITypeDefinition dd in d)
                        {
                            csd.cmp.Import(dd);
                        }
                    }
                }
                else while (csd == null)
                        Task.Delay(2000);
            }
            return csd;
        }

        public void ImportProjectTypes()
        {
            ArrayList P = this.GetProjectReferences("ProjectReference");

            foreach (string name in P)
            {
                if (vs == null)
                    continue;

                VSProject vp = vs.GetProjectbyName(name);

                if (vp == null)
                    continue;

                if (vp.csd == null)
                    vp.CSParsers();

                ArrayList L = vp.GetCompileItems();
                
                csd.AddProjectFiles(L);

                csd.addProjectFiles(L);
                
                csd.snx.cc = csd.snx.cc.AddReferences(vp.csd.snx.cc.References);

            }
            
        }

        public Dictionary<string, string> GetTypeNames()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            if (dicts == null)
                return d;

            foreach (string s in dicts.Keys)
            {
                string[] b = s.Split(".".ToCharArray());

                if (b.Length <= 0)
                    continue;

                string dd = b[b.Length - 1];

                if (d.ContainsKey(dd) == false)
                    d.Add(b[b.Length - 1], "");
            }
            d.Add("get", "");

            d.Add("set", "");

            return d;
        }

        public string GetProjectFolder()
        {
            string s = Path.GetDirectoryName(FileName);

            if (string.IsNullOrEmpty(s))
                return "";

            s = Path.GetFullPath(s);

            return s;
        }




        static VSProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);




            //s_ProjectRootElement_OutDir = s_ProjectInSolution.GetProperty("OutDir", BindingFlags.NonPublic | BindingFlags.Instance);
            //s_ProjectRootElement_AssemblyName = s_ProjectInSolution.GetProperty("AssemblyName", BindingFlags.NonPublic | BindingFlags.Instance);

            s_ProjectRootElement = Type.GetType("Microsoft.Build.Construction.ProjectRootElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_ProjectRootElementCache = Type.GetType("Microsoft.Build.Evaluation.ProjectRootElementCache, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectRootElement_Items = s_ProjectRootElement.GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);


            s_ProjectRootElement_OutputType = s_ProjectRootElement.GetProperty("Properties", BindingFlags.Public | BindingFlags.Instance);
        }

        public string IsPortable { get; set; }

        public IEnumerable<VSProjectItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = (List<VSProjectItem>)value;
            }
        }

        public VSProject()
        {
            _items = new List<VSProjectItem>();
        }

        public void GetDataSources(Dictionary<VSProject, ArrayList> dict)
        {
            Microsoft.Build.Evaluation.Project pc = null;

            if (!File.Exists(FileName))
                return;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch(Exception e)
            {

            }
            if (pc == null)
                return;

            ArrayList D = new ArrayList();

            var c = pc.GetItems("None");
            foreach(ProjectItem p in c)
            {
                if (p.EvaluatedInclude.EndsWith(".datasource"))
                    D.Add(p.EvaluatedInclude);

                if (p.EvaluatedInclude.EndsWith(".xsd"))
                    D.Add(p.EvaluatedInclude);

            }

            dict.Add(this, D);

            pc.ProjectCollection.UnloadAllProjects();
        }

        private ICollection<ProjectPropertyElement> properties { get; set; }

        private string Find(string name)
        {
            foreach (ProjectPropertyElement p in properties)
                if (p.Name == name)
                    return p.Value;
            return "";
        }

        public VSProject(VSSolution solution, object internalSolutionProject)
        {
            this.Name = s_ProjectInSolution_ProjectName.GetValue(internalSolutionProject, null) as string;
            this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(internalSolutionProject, null).ToString();
            this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(internalSolutionProject, null) as string;
            this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(internalSolutionProject, null) as string;


            _internalSolutionProject = internalSolutionProject;

            _projectFileName = Path.Combine(solution.SolutionPath, this.RelativePath);

            _items = new List<VSProjectItem>();

            if (this.ProjectType == "KnownToBeMSBuildFormat")
            {
                this.Parse();
            }
        }

        private void Parse()
        {
            try
            {
                var stream = File.OpenRead(_projectFileName);
                var reader = XmlReader.Create(stream);
                var cache = s_ProjectRootElementCache.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { true });
                ConstructorInfo[] inf = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                var rootElement = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { reader, cache, true });

                stream.Close();

                var collection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);

                foreach (var item in collection)
                {
                    _items.Add(new VSProjectItem(this, item));
                }
                this.properties = s_ProjectRootElement_OutputType.GetValue(rootElement, null) as ICollection<ProjectPropertyElement>;



                string outtype = Find("OutputType");
                string assembly = Find("AssemblyName");
                IsPortable = Find("IsPortable");
                if (IsPortable != null)
                    if (IsPortable == "true")
                        MessageBox.Show("portable");
                OutputName = assembly + ".exe";
                if (outtype == "Library")
                    OutputName = assembly + ".dll";

                properties = null;
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }

        public string Error { get; set; }

        public IEnumerable<VSProjectItem> EDMXModels
        {
            get
            {
                return _items.Where(i => i.ItemType == "EntityDeploy");
            }
        }

        public void Dispose()
        {
        }

        public bool IsExecutable()
        {
            bool exe = false;

            if (!File.Exists(FileName))
                return false;

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return exe;

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");


            if (outtype != null)
                if (outtype.EvaluatedValue == "WinExe" || outtype.EvaluatedValue.ToLower().Contains("exe"))
                    exe = true;

            pc.ProjectCollection.UnloadAllProjects();

            return exe;
        }

        public string GetPlatform()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty s = pc.GetProperty("Platform");

            pc.ProjectCollection.UnloadAllProjects();

            return s.EvaluatedValue;
        }

        public string GetConfiguration()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty s = pc.GetProperty("Configuration");

            pc.ProjectCollection.UnloadAllProjects();

            return s.EvaluatedValue;
        }

        public void CreatePlatform(string config, string platform)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);
            if (pc == null)
                return;
            ProjectRootElement e = pc.Xml;
            ProjectPropertyGroupElement g = e.AddPropertyGroup();
            g.Condition = " '$(Configuration)|$(Platform)' == '" + config + "|" + platform + "' ";
            e.Save();
            pc.Save();
        }
        public void SetPlatformConfig(string config, string platform, Microsoft.Build.Evaluation.Project pc)
        {
            if (pc == null)
                return;

            pc.SetProperty("Configuration", config);

            pc.SetProperty("Platform", platform);

            pc.Save();
        }
        public void SetPlatformConfig(string config, string platform)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);
            if (pc == null)
                return;

            pc.SetProperty("Configuration", config);

            pc.SetProperty("Platform", platform);

            pc.Save();

            pc.ProjectCollection.UnloadAllProjects();
        }

        public bool HasGroupElementProperty(ProjectPropertyGroupElement b, string p, string v, Microsoft.Build.Evaluation.Project pc)
        {
            foreach (ProjectPropertyElement e in b.Properties)

                if (e.Name == p)
                    return true;

            b.AddProperty(p, v);

            return false;
        }

        public ProjectPropertyGroupElement GetGroupElement(string config, string platform, Microsoft.Build.Evaluation.Project pc)
        {
            ICollection<ProjectPropertyGroupElement> e = pc.Xml.PropertyGroups;
            foreach (ProjectPropertyGroupElement b in e)
            {
                //MessageBox.Show(b.Condition);

                string d = b.Condition;

                d = d.Replace("'", "");

                string[] cc = d.Split("==".ToCharArray());

                if (cc.Length < 3)
                    continue;
                //MessageBox.Show(cc[2]);

                string[] c = cc[2].Split("|".ToCharArray());

                string cs = c[0].Trim();
                string ps = c[1].Trim();

                if (cs == config && ps == platform)
                    return b;

                //MessageBox.Show(c[0] + " - " + c[1]);
            }

            return null;
        }


        public string GetPropertyBase(string s)
        {
            Microsoft.Build.Evaluation.Project pc = null;

            if (!File.Exists(FileName))
                return "";

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty d = pc.GetProperty(s);

            pc.ProjectCollection.UnloadAllProjects();

            if (d == null)
                return "";

            return d.EvaluatedValue;
        }
        public string GetTargetFrameworkVersion()
        {
            return GetPropertyBase("TargetFrameworkVersion");
        }
        public void SetTargetFrameworkVersion(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("TargetFrameworkVersion", v, pc);
        }

        public string GetOutputPath()
        {
            return GetPropertyBase("OutputPath");
        }
        public void SetOutputPath(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("OutputPath", v, pc);
        }
        public string TreatWarningsAsErrors()
        {
            return GetPropertyBase("TreatWarningsAsErrors");
        }
        public void SetTreatWarningsAsErrors(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("TreatWarningsAsErrors", v, pc);
        }
        public string GetWarningLevel()
        {
            return GetPropertyBase("WarningLevel");
        }
        public void SetWarningLevel(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("WarningLevel", v, pc);
        }
        public string GetDefineConstants()
        {
            return GetPropertyBase("DefineConstants");
        }
        public void SetDefineConstants(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("DefineConstants", v, pc);
        }
        public string GetUnsafeBlocks()
        {
            return GetPropertyBase("AllowUnsafeBlocks");
        }
        public void SetUnsafeBlocks(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("AllowUnsafeBlocks", v, pc);
        }
        public string GetOptimize()
        {
            return GetPropertyBase("Optimize");
        }
        public void SetOptimize(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("Optimize", v, pc);
        }

        public string GetPrefer32bit()
        {
            return GetPropertyBase("Prefer32bit");
        }

        public void SetPrefer32bit(string v, Microsoft.Build.Evaluation.Project pc)
        {
            SetProperty("Prefer32bit", v, pc);
        }

        public string GetOutDir()
        {
            if (!File.Exists(FileName))
                return "";

            ArrayList L = GetProperties("", "");

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            string outs = "";

            

            foreach (ProjectProperty p in L)
                if (p.Name == "OutDir")
                    outs = p.EvaluatedValue;

            pc.ReevaluateIfNecessary();

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");

            string ext = ".dll";

            if (outtype == null)
                return "";

            if (outtype.EvaluatedValue.ToLower() == "winexe" || outtype.EvaluatedValue.ToLower() == "exe")
                ext = ".exe";

            // string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue + ext;

            string s = outs + "\\" + assname.EvaluatedValue + ext;




            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public string GetOutDirs()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");

            string ext = ".dll";

            if (outtype.EvaluatedValue == "WinExe")
                ext = ".exe";

            string s = Path.GetDirectoryName(FileName) + "\\" + outdir.EvaluatedValue;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public string GetDefNamespace()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("RootNamespace");

            string s = assname.EvaluatedValue;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public string GetProjectExec()
        {
            string g = Path.GetFileName(FileName);

            string p = Path.GetFullPath(FileName).Replace(g, "");

            string s = GetOutDir();

            if (Path.IsPathRooted(s))
                return s;

            string r = p + "\\" + s;

            //string r = s;

            return r;
        }

        public Microsoft.Build.Evaluation.ProjectItem GetReference(string name, string refs)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }
            if (pc == null)
                return null;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                if (p.EvaluatedInclude == refs)
                    return p;
            }

            pc.ProjectCollection.UnloadAllProjects();

            return null;
        }

        public string GetReferenceHint(Microsoft.Build.Evaluation.ProjectItem p)
        {
            if (p.HasMetadata("HintPath") == false)
                return "";

            foreach (Microsoft.Build.Evaluation.ProjectMetadata m in p.Metadata)
            {
                if (m.Name == "HintPath")
                    return m.UnevaluatedValue;
            }

            return "";
        }

        public string GetPathHint(string refs)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }
            if (pc == null)
                return "";

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems("Reference");

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                if (p.EvaluatedInclude == refs)
                {
                  
                    foreach (Microsoft.Build.Evaluation.ProjectMetadata m in p.Metadata)
                    {
                        if (m.Name == "HintPath")
                        {
                            pc.ProjectCollection.UnloadAllProjects();
                            return m.UnevaluatedValue;
                        }
                    }

               }
            }

            pc.ProjectCollection.UnloadAllProjects();
            return "";
      
        }


        public string GetReferenceHint(string refs)
        {
            Microsoft.Build.Evaluation.ProjectItem p = GetReference("Reference", refs);

            if (p == null)
                return "";

            string hint = GetReferenceHint(p);

            

            return hint;
        }

        public ArrayList GetReferences(string name)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                string file = p.EvaluatedInclude;// Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                //string project = Path.GetFileNameWithoutExtension(file);

                //file = p.EvaluatedInclude;

                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public ArrayList GetReferences()
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems("Reference");

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                string file = "";

                if (p.HasMetadata("HintPath"))
                {
                    
                    file = Path.GetFullPath(folder + "\\" + p.GetMetadata("HintPath").UnevaluatedValue);
                }
                else 
                if(!p.HasMetadata("HintPath"))
                    file = p.EvaluatedInclude;

                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public ArrayList GetProjectReferences(string name)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { };
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                string file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                string project = Path.GetFileNameWithoutExtension(file);

                file = project;

                L.Add(file);
            }



            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public ArrayList GetProjectReferencesFiles(string name)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { };
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                string file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                //string project = Path.GetFileNameWithoutExtension(file);

                //file = project;

                L.Add(file);
            }



            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public void RenameItem(string type, string evInclude, string newvalue)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(type);

            if (s.Count() < 0)
                return;

            Microsoft.Build.Evaluation.ProjectItem pp = null;

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                if (p.EvaluatedInclude != evInclude)
                    continue;
                else
                {
                    pp = p;
                    break;
                }
            }

            if (pp != null)
            {
                //pp.SetMetadataValue("", "");

                ArrayList L = new ArrayList();


                IEnumerable<ProjectMetadata> metadata = pp.Metadata;

                foreach (ProjectMetadata b in metadata)
                {
                    if (b.EvaluatedValue.Contains(""))
                    {
                        KeyValuePair<string, string> d = new KeyValuePair<string, string>(b.Name, b.EvaluatedValue);
                        L.Add(d);
                    }
                }

                pc.RemoveItem(pp);

                pc.AddItem(type, newvalue, L as IEnumerable<KeyValuePair<string, string>>);

                pc.Save();
            }

            pc.ProjectCollection.UnloadAllProjects();
        }
        public void RenameItem(string type, string evInclude, string newvalue, string prevform, string newform)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(type);

            if (s.Count() < 0)
                return;

            Microsoft.Build.Evaluation.ProjectItem pp = null;

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                if (p.EvaluatedInclude != evInclude)
                    continue;
                else
                {
                    pp = p;
                    break;
                }
            }

            if (pp != null)
            {
                ArrayList L = new ArrayList();

                IEnumerable<ProjectMetadata> metadata = pp.Metadata;

                foreach (ProjectMetadata b in metadata)
                {
                    if (b.EvaluatedValue.Contains(prevform))
                    {
                        KeyValuePair<string, string> d = new KeyValuePair<string, string>(b.Name, prevform);
                        L.Add(d);
                    }
                    else
                    {
                        KeyValuePair<string, string> d = new KeyValuePair<string, string>(b.Name, b.EvaluatedValue);
                        L.Add(d);
                    }
                }

                pc.RemoveItem(pp);

                pc.AddItem(type, newvalue);

                pc.Save();

                IEnumerable<KeyValuePair<string, string>> e = L as IEnumerable<KeyValuePair<string, string>>;

                s = pc.GetItems(type);

                foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
                {
                    if (p.EvaluatedInclude != newvalue)
                        continue;
                    else
                    {
                        foreach (KeyValuePair<string, string> c in L)
                            p.SetMetadataValue(c.Key, c.Value);
                    }
                }



                MoveItem(evInclude, newvalue);


                pc.Save();
            }

            pc.ProjectCollection.UnloadAllProjects();
        }

        private string[] _names = { "folder", "ui", "data", "forms", "web", "dev", "app", "files", "models", "views", "controllers" };

        public string NewInclude(Microsoft.Build.Evaluation.ProjectItem p)
        {
            Random r = new Random(1234567);

            string name = GetName(p);

            string c = "";

            for (int i = 0; i < 3; i++)
            {
                int b = (int)(r.NextDouble() * (_names.Length - 1));

                if (i == 0)
                    c = _names[b];
                else
                    c += "\\" + _names[b];
            }



            return c;
        }

        public void RenameSubtype(ArrayList S, string prevform, Microsoft.Build.Evaluation.Project pc = null)
        {
            pc = LoadProject(pc);

            string prefix = "";

            foreach (Microsoft.Build.Evaluation.ProjectItem pp in S)
            {
                if (prefix == "")
                    prefix = NewInclude(pp);

                ArrayList L = new ArrayList();

                string name = GetName(pp);

                string newinclude = prefix + "\\" + name;

                IEnumerable<ProjectMetadata> metadata = pp.Metadata;

                foreach (ProjectMetadata b in metadata)
                {
                    if (b.EvaluatedValue.Contains(prevform))
                    {
                        KeyValuePair<string, string> d = new KeyValuePair<string, string>(b.Name, prevform);
                        L.Add(d);
                    }
                    else
                    {
                        KeyValuePair<string, string> d = new KeyValuePair<string, string>(b.Name, b.EvaluatedValue);
                        L.Add(d);
                    }
                }

                pc.RemoveItem(pp);

                IList<ProjectItem> s = pc.AddItem(pp.ItemType, newinclude);

                //pc.Save();

                //IEnumerable<KeyValuePair<string, string>> e = L as IEnumerable<KeyValuePair<string, string>>;

                //s = pc.GetItems(type);

                foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
                {
                    if (p.EvaluatedInclude != newinclude)
                        continue;
                    else
                    {
                        foreach (KeyValuePair<string, string> c in L)
                            p.SetMetadataValue(c.Key, c.Value);
                    }
                }

                //MoveItem(evInclude, newvalue);

                //ProjectRootElement r = pc.Xml;

                ///pc.Save();

                //pc.ProjectCollection.UnloadAllProjects();

                //pc = new Microsoft.Build.Evaluation.Project(r);
            }
        }

        public void FolderCreate(string name)
        {
            string dest = GetProjectFolder() + "\\" + name;


            if (Directory.Exists(dest) == false)
                Directory.CreateDirectory(dest);
        }

        public void MoveItem(string s, string d)
        {
            string source = GetProjectFolder() + "\\" + s;

            string dest = GetProjectFolder() + "\\" + d;

            string folder = Path.GetDirectoryName(dest);

            string[] cc = folder.Split("\\".ToCharArray());

            string dd = "";

            int i = 0;
            foreach (string g in cc)
            {
                if (i != 0)
                    dd += "\\" + cc[i];
                else dd += cc[i];

                //if (Directory.Exists(ddsource) == true)
                if (Directory.Exists(dd) == false)
                {
                    Directory.CreateDirectory(dd);
                }

                i++;
            }

            if (File.Exists(source) == false)
                return;


            File.Move(source, dest);
        }
        public ArrayList GetAllItems(Microsoft.Build.Evaluation.Project pc = null)
        {
            ArrayList L = new ArrayList();

            pc = LoadProject(pc);

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.AllEvaluatedItems;

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                L.Add(p);
            }

            if (pc == null)

                pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public IDictionary<string, Microsoft.Build.Execution.ProjectTargetInstance> GetTargets()
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return null;



            string folder = Path.GetDirectoryName(FileName);

            IDictionary<string, Microsoft.Build.Execution.ProjectTargetInstance> s = pc.Targets;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }
        public ArrayList GetImports()
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;



            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ResolvedImport> s = pc.Imports;

            foreach (Microsoft.Build.Evaluation.ResolvedImport p in s)
            {
                L.Add(p);
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public ArrayList GetItems(string name)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);



            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                L.Add(p);
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public ArrayList GetItems(string name, bool folds)
        {
            ArrayList L = new ArrayList();

            if (!File.Exists(FileName))
                return L;

            Microsoft.Build.Evaluation.Project pc = null;
            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e)
            {
            }
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                string file = "";
                if (folds == false)
                    file = p.EvaluatedInclude;
                else
                {
                    if (Path.IsPathRooted(p.EvaluatedInclude) == true)
                        file = p.EvaluatedInclude;
                    else
                        file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);
                }


                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public ArrayList GetItems(Microsoft.Build.Evaluation.Project pc, string name)
        {
            ArrayList L = new ArrayList();

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                try
                {
                    L.Add(p);
                }
                catch (Exception e) { }
            }

            //pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public ArrayList GetItems(Microsoft.Build.Evaluation.Project pc, string name, bool folds)
        {
            ArrayList L = new ArrayList();

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                try
                {
                    string file = p.EvaluatedInclude;

                    if (File.Exists(file) == false)

                        file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                    if (folds == false)
                        file = p.EvaluatedInclude;

                    L.Add(file);
                }
                catch (Exception e) { }
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public IDictionary<string, List<string>> GetConditionedProperties()
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return null;

            IDictionary<string, List<string>> s = pc.ConditionedProperties;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public ArrayList GetProperties()
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return L;

            ICollection<Microsoft.Build.Evaluation.ProjectProperty> s = pc.AllEvaluatedProperties;// Properties;

            foreach (Microsoft.Build.Evaluation.ProjectProperty p in s)
            {
                L.Add(p);
            }

            //Microsoft.Build.Evaluation.ProjectProperty pp = pc.GetProperty("WarningLevel");

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public ArrayList GetProperties(string proj, string platform)
        {

            if (File.Exists(FileName) == false)
                return new ArrayList();

            ArrayList L = new ArrayList();

            try
            {
                Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

                SetProperty("Configuration", proj, pc);

                SetProperty("Platform", platform, pc);

                pc.ReevaluateIfNecessary();

                if (pc == null)
                    return L;

                ICollection<Microsoft.Build.Evaluation.ProjectProperty> s = pc.AllEvaluatedProperties;// Properties;

                foreach (Microsoft.Build.Evaluation.ProjectProperty p in s)
                {
                    L.Add(p);
                }

                //Microsoft.Build.Evaluation.ProjectProperty pp = pc.GetProperty("WarningLevel");

                pc.ProjectCollection.UnloadAllProjects();
            }
            catch(Exception ex) { }

            return L;
        }

        public IDictionary<string, string> GetGlobalProperties()
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e) { }

            if (pc == null)
                return null;

            IDictionary<string, string> s = pc.GlobalProperties;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public ArrayList GetsItems(Microsoft.Build.Evaluation.Project pc, string name, bool folds)
        {
            ArrayList L = new ArrayList();

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {
                L.Add(p);
            }

            //pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public ArrayList GetsItems(Microsoft.Build.Evaluation.Project pc, string name, string evs, bool folds)
        {
            ArrayList L = new ArrayList();

            if (pc == null)
                return L;

            //string folder = Path.GetDirectoryName(FileName);

            if (name != null)
            {
                ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

                foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
                {
                    if (p.EvaluatedInclude == evs)

                        L.Add(p);
                }

                //pc.ProjectCollection.UnloadAllProjects();
            }

            return L;
        }

        public static string GetRelativePath(string BasePath, string AbsolutePath)
        {
            char Separator = Path.DirectorySeparatorChar;
            if (string.IsNullOrWhiteSpace(BasePath)) BasePath = Directory.GetCurrentDirectory();
            var ReturnPath = "";
            var CommonPart = "";
            var BasePathFolders = BasePath.Split(Separator);
            var AbsolutePathFolders = AbsolutePath.Split(Separator);
            var i = 0;
            while (i < BasePathFolders.Length & i < AbsolutePathFolders.Length)
            {
                if (BasePathFolders[i].ToLower() == AbsolutePathFolders[i].ToLower())
                {
                    CommonPart += BasePathFolders[i] + Separator;
                }
                else
                {
                    break;
                }
                i += 1;
            }
            if (CommonPart.Length > 0)
            {
                var parents = BasePath.Substring(CommonPart.Length - 1).Split(Separator);
                foreach (var ParentDir in parents)
                {
                    if (!string.IsNullOrEmpty(ParentDir))
                        ReturnPath += ".." + Separator;
                }
            }
            ReturnPath += AbsolutePath.Substring(CommonPart.Length);
            return ReturnPath;
        }

        public ArrayList SetProjectReferences(ArrayList L)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetsItems(pc, "ProjectReference", false);

            foreach (Microsoft.Build.Evaluation.ProjectItem pg in R)
            {
                pc.RemoveItem(pg);
            }

            foreach (string p in L)
            {
                string refs = GetRelativePath(s, p);

                //string names = GetRelativePath(pc.ProjectFileLocation.File, solution.solutionFileName);

                int i = R.IndexOf(refs);

                if (i < 0)

                    pc.AddItem("ProjectReference", refs);
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }


        public void SetReference(string file, bool withhint = true)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string libName = Path.GetFileName(file).Replace(".dll", "");

            string s = Path.GetDirectoryName(FileName);

            string r = GetRelativePath(s, file);

            MessageBox.Show("RelativePath is " + r);

            ArrayList R = GetItems(pc, "Reference", false);

            int i = R.IndexOf(libName);


            MessageBox.Show("Reference will be added " + libName);

            IList<ProjectItem> pp = pc.AddItem("Reference", libName);

            if (pp.Count <= 0)
                return;

            ProjectItem b = pp[0];

            b.SetMetadataValue("HintPath", r);

            b.SetMetadataValue("Private", "true");

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }



        public ArrayList SetReferences(ArrayList L)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Reference", false);

            foreach (string p in L)
            {
                //string refs = GetRelativePath(s, p);

                int i = R.IndexOf(p);

                if (i < 0)

                    pc.AddItem("Reference", p);
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }
        public void SetRestorePackages(bool restore)
        {
            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
            }
            catch (Exception e)
            {
            }

            if (pc == null)
                return;

            pc.SetProperty("RestorePackages", "false");

            string s = Path.GetDirectoryName(FileName);

            ProjectProperty p = pc.GetProperty("RestorePackages");

            if (p == null)
            {
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }

        public static ArrayList LoadResource(string ase)
        {
            ArrayList V = new ArrayList();

            ResXResourceReader mg = new ResXResourceReader(ase);

            mg.BasePath = Path.GetFullPath(ase).Replace(Path.GetFileName(ase), "");

            mg.UseResXDataNodes = true;

            IDictionaryEnumerator dict = mg.GetEnumerator();



            while (dict.MoveNext())
            {
                ResXDataNode node = (ResXDataNode)dict.Value;
                if (node.FileRef == null)
                    continue;
                string[] tt = node.FileRef.TypeName.Split(",".ToCharArray());
                string name = node.Name;
                string file = node.FileRef.FileName;

                Tuple<string, string, string> T = new Tuple<string, string, string>(tt[0], name, file);

                V.Add(T);
            }

            mg.Close();

            return V;
        }

        public static void SaveAddResource(string ase, string name, string file, string typename)
        {
            ResXResourceWriter mg = new ResXResourceWriter(ase);

            mg.BasePath = Path.GetFullPath(ase).Replace(Path.GetFileName(ase), "");

            ResXResourceReader mgs = new ResXResourceReader(ase);
            mgs.BasePath = Path.GetFullPath(ase).Replace(Path.GetFileName(ase), "");
            mgs.UseResXDataNodes = true;

            IDictionaryEnumerator dict = mgs.GetEnumerator();

            while (dict.MoveNext())
            {
                ResXDataNode nodes = (ResXDataNode)dict.Value;
                mg.AddResource(nodes);
            }



            ResXDataNode node = (ResXDataNode)new ResXDataNode(name, new ResXFileRef(file, typename));



            mg.AddResource(node);
            mg.Generate();
            mg.Close();
        }

        public ArrayList GetEmbeddedResourcesFiles(Microsoft.Build.Evaluation.Project pc = null)
        {
            ArrayList L = new ArrayList();


            try
            {
                pc = LoadProject(pc);
            }
            catch (Exception e) { };

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetsItems(pc, "EmbeddedResource", true);

            ArrayList r = new ArrayList();

            foreach (ProjectItem p in R)
            {
                if (p.HasMetadata("DependentUpon") == true)
                    continue;

                foreach (ProjectMetadata g in p.Metadata)
                {
                }

                //r.Add(p.EvaluatedInclude);

                string file = pc.DirectoryPath + "\\" + p.EvaluatedInclude;

                r.Add(file);

                //ArrayList V = Load(file);
            }

            //pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return r;
        }

        public bool IsResourceEmbedded(string file)
        {
            ArrayList R = GetEmbeddedResourcesFiles();

            foreach (string p in R)
            {
                if (file == p)
                    return true;
            }

            return false;
        }

        private Dictionary<string, ArrayList> di { get; set; }

        public void GetSubTypes()
        {
        }
        public ArrayList GetCompile(Microsoft.Build.Evaluation.Project pc = null, bool fs = true)
        {
            ArrayList L = new ArrayList();


            try
            {
                pc = LoadProject(pc);
            }
            catch (Exception e) { };

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Compile");

            //pc.ProjectCollection.UnloadAllProjects();

            return R;
        }
        public ArrayList GetCompileItems(bool fs = true)
        {
            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = LoadProject(pc);
            }
            catch (Exception e) { };

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Compile", fs);

            pc.ProjectCollection.UnloadAllProjects(); // uncommented

            return R;
        }

        public string GetFormName(ArrayList L)
        {
            string name = "";

            if (L == null)
                return name;

            foreach (string s in L)
            {
                if (s.ToLower().Contains(".resx") == false)
                    if (s.ToLower().Contains("designer") == false)
                    {
                        name = s;// s.Replace(".cs", "");

                        return name;
                    }
            }

            return name;
        }
        public string GetResourceName(ArrayList L)
        {
            string name = "";

            if (L == null)
                return name;

            foreach (string s in L)
            {
                // if (s.ToLower().Contains(".resx") == false)
                if (s.ToLower().Contains("designer") == false)
                {
                    name = s;// s.Replace(".cs", "");

                    return name;
                }
            }

            return name;
        }

        public string RenameTo(string name)
        {

            

            string[] cc = name.Split(".".ToCharArray());

            string r = cc[0];

            name = cc[0] + "1";

            return name;

        }

        public ArrayList RenameForm(ArrayList L, ArrayList R, string name, Dictionary<string,string> dict)
        {

            ArrayList F = new ArrayList();

            string[] cc = name.Split(".".ToCharArray());

            string r = cc[0];

            string start = name;

            int i = 0;
            while ((R.Contains(name)))
            {
                name = start;
                string[] bb = name.Split(".".ToCharArray());
                bb[0] = bb[0] + i++.ToString();
                name = "";
                int k = 0;
                while (k < bb.Length - 1)
                    name += bb[k++] + ".";
                name += bb[bb.Length - 1];
            }

            string[] dd = name.Split(".".ToCharArray());

            name = dd[0];

            foreach(string s in L)
            {


                
                string d = s.Replace(r, name);

                F.Add(d);

                dict[s] = d;

            }

            return F;

        }

        public Dictionary<string, string> RenameToName(Microsoft.Build.Evaluation.Project pc, ArrayList L)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

           // Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return dict;

            string s = Path.GetDirectoryName(FileName);
           
            ArrayList R = GetItems(pc, "Compile", false);

            string name = GetFormName(L);

            if (name == "")
                name = GetResourceName(L);

            if (name == "")
                return dict;

            string names = "";

            if (R.Contains(name))
            {

                L = RenameForm(L, R, name, dict);
                names = RenameTo(name);
                //name = name.Replace(name, names);

            }

            return dict;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="L"></param>
        /// <param name="folders"></param>
        /// <returns></returns>
        public Dictionary<string, string> AddFormItem(ArrayList L, string folders)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return dict;

            string s = Path.GetDirectoryName(FileName);

            if (folders != "")
                s = s + "\\" + folders;

            ArrayList R = GetItems(pc, "Compile", false);

            string name = GetFormName(L);

            if (name == "")
                name = GetResourceName(L);

            if (name == "")
                return dict;

            string names = "";

            
            if (R.Contains(name))
            {

                L = RenameForm(L, R, name, dict);

                names = RenameTo(name);
                
                //name = name.Replace(name, names);

            }

            if (dict.ContainsKey(name))
                name = dict[name];

            foreach (string p in L)
            {
                string refs = folders + "\\" + p;

                if (folders == "")
                    refs = p;

                IEnumerable<KeyValuePair<string, string>> metadata = Enumerable.Empty<KeyValuePair<string, string>>();

                int i = R.IndexOf(refs);

                if (p.ToLower().Contains(".resx"))
                {
                    List<KeyValuePair<string, string>> list = metadata.ToList();

                    list.Add(new KeyValuePair<string, string>("DependentUpon", name));

                    metadata = (IEnumerable<KeyValuePair<string, string>>)list;

                    if (i < 0)
                        pc.AddItem("EmbeddedResource", refs, metadata);
                }
                else if (p.ToLower().Contains("designer"))
                {
                    List<KeyValuePair<string, string>> list = metadata.ToList();

                    list.Add(new KeyValuePair<string, string>("DependentUpon", name));

                    metadata = (IEnumerable<KeyValuePair<string, string>>)list;

                    if (i < 0)
                        pc.AddItem("Compile", refs, metadata);
                }
                else
                {
                    List<KeyValuePair<string, string>> list = metadata.ToList();

                    //Add new item to list.
                    list.Add(new KeyValuePair<string, string>("SubType", "Form"));

                    //Cast list to IEnumerable
                    metadata = (IEnumerable<KeyValuePair<string, string>>)list;

                    if (i < 0)

                        pc.AddItem("Compile", refs, metadata);
                }

                //  <Compile Include="OptionsForms.cs">
                //  <SubType>Form</SubType>
                //  </Compile>

                //  <Compile Include="OptionsForms.Designer.cs">
                //  <DependentUpon>OptionsForms.cs</DependentUpon>
                //  </Compile>

                //  <EmbeddedResource Include="OptionsForms.resx">
                //  <DependentUpon>OptionsForms.cs</DependentUpon>
                //  </EmbeddedResource>

                
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return dict;

        }

        public void AddDataSource(string name)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            pc.AddItem("None", "Properties\\DataSources\\" + name);

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

        }

        public void AddXSDItem(string name)
        {

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string s = Path.GetDirectoryName(FileName);

            string cs = name + ".Designer.cs";

            string xsd = name + ".xsd";

            string xsc = name + ".xsc";

            string xss = name + ".xss";

            IEnumerable <KeyValuePair<string, string>> metadata = Enumerable.Empty<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> list = metadata.ToList();
            list.Add(new KeyValuePair<string, string>("AutoGen", "True"));
            list.Add(new KeyValuePair<string, string>("DesignTime", "True"));
            list.Add(new KeyValuePair<string, string>("DependentUpon", xsd));
            pc.AddItem("Compile", cs, list);
            pc.Save(FileName);

            metadata = Enumerable.Empty<KeyValuePair<string, string>>();
            list = metadata.ToList();
            list.Add(new KeyValuePair<string, string>("DependentUpon", xsd));
            pc.AddItem("None", xsc, list);
            pc.Save(FileName);

            metadata = Enumerable.Empty<KeyValuePair<string, string>>();
            list = metadata.ToList();
            list.Add(new KeyValuePair<string, string>("DependentUpon", xsd));
            pc.AddItem("None", xss, list);
            pc.Save(FileName);

            metadata = Enumerable.Empty<KeyValuePair<string, string>>();
            list = metadata.ToList();
            list.Add(new KeyValuePair<string, string>("Generator", "MSDataSetGenerator"));
            list.Add(new KeyValuePair<string, string>("LastGenOutput", cs));
            list.Add(new KeyValuePair<string, string>("SubType", "Designer"));
            pc.AddItem("None", xsd, list);
            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

           
        }
        public void LoadMetaData(ArrayList L)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string s = Path.GetDirectoryName(FileName);

            // ArrayList R = GetItems(pc, "Compile", false);

            foreach (VSProjectItem p in L)
            {
                string type = p.ItemType;

                if (type == null)
                    type = p.SubType;

                string include = p.Include;

                if (include == null)
                    continue;

                ArrayList D = GetsItems(pc, type, include, false);

                if (D == null)
                    continue;
                if (D.Count <= 0)
                    continue;
                Microsoft.Build.Evaluation.ProjectItem pp = D[0] as Microsoft.Build.Evaluation.ProjectItem;
                if (pp == null)
                    continue;
                foreach (Microsoft.Build.Evaluation.ProjectMetadata b in pp.Metadata)
                {
                    string name = b.Name;

                    p.mc = b;
                }
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return;
        }


        public bool HasInclude(ICollection<ProjectItem> P, string s)
        {
            foreach (ProjectItem p in P)
                if (p.EvaluatedInclude == s)
                    return true;


            return false;
        }

        public ArrayList SetItems(ArrayList L, ArrayList T)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            // ArrayList R = GetItems(pc, "Compile", false);

            ICollection<ProjectItem> P = pc.GetItems("Compile");
            ICollection<ProjectItem> E = pc.GetItems("EmbeddedResource");



            int N = L.Count;

            int i = 0;
            while (i < N)
            {
                VSProjectItem p = L[i] as VSProjectItem;
                VSProjectItem r = T[i] as VSProjectItem;

                string type = p.ItemType;

                if (type == null)
                    type = p.SubType;

                string include = p.Include;

                if (include == null)
                {
                    i++;
                    continue;
                }

                string replace = "";
                string replacewith = "";

                if (HasInclude(P, include))
                {
                    string[] cc = include.Split(".".ToCharArray());

                    if (cc.Length >= 2)
                    {
                        string b = cc[0] + "0";

                        include = include.Replace(cc[0], b);

                        p.Include = include;

                        replace = cc[0];
                        replacewith = b;
                    }
                }
                if (HasInclude(E, include))
                {
                    string[] cc = include.Split(".".ToCharArray());

                    if (cc.Length >= 2)
                    {
                        string b = cc[0] + "0";

                        include = include.Replace(cc[0], b);

                        p.Include = include;

                        replace = cc[0];
                        replacewith = b;
                    }
                }


                if (type != null)
                {
                    IList<Microsoft.Build.Evaluation.ProjectItem> e = pc.AddItem(type, include);

                    if (r.mc != null)
                    {
                        if (replace == "")
                            e[0].SetMetadataValue(r.mc.Name, r.mc.EvaluatedValue);
                        else
                        {
                            string data = r.mc.EvaluatedValue;

                            data = data.Replace(replace, replacewith);

                            e[0].SetMetadataValue(r.mc.Name, data);
                        }
                    }
                }

                i++;
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public string GetName(Microsoft.Build.Evaluation.ProjectItem p)
        {
            string[] cc = p.EvaluatedInclude.Split("\\".ToCharArray());
            string c = cc[cc.Length - 1];
            return c;
        }



        public void Shuffle(Microsoft.Build.Evaluation.Project pc = null)
        {
            //Microsoft.Build.Evaluation.Project pc = LoadProject(dd);

            ArrayList L = GetCompile(pc);

            Dictionary<string, ArrayList> dict = GetSubtypes(pc);

            foreach (Microsoft.Build.Evaluation.ProjectItem p in L)
            {
                if (p == null)
                    continue;
                string b = GetName(p);
                if (dict.ContainsKey(b) == true)
                {
                    ArrayList N = dict[b];
                    RenameSubtype(N, b, pc);
                }
                else
                {
                    //RenameItem()
                }
            }

            pc.ReevaluateIfNecessary();

            //ProjectRootElement r = pc.Xml;

            //pc.ProjectCollection.UnloadAllProjects();

            //pc.ProjectCollection.UnloadAllProjects();

            //r.ToolsVersion += "1";

            string file = Path.GetFileName(this.FileName).Trim();

            string filename = this.FileName.Trim().Replace(file, "_" + file);

            //pc.ProjectCollection.Dispose();



            //pc = new Microsoft.Build.Evaluation.Project(r);





            pc.Save(filename);

            pc.ProjectCollection.UnloadAllProjects();
        }


        public Dictionary<string, ArrayList> GetSubtypes(Microsoft.Build.Evaluation.Project pc = null)
        {
            di = new Dictionary<string, ArrayList>();

            ArrayList F = GetAllItems(pc);

            foreach (ProjectItem p in F)
            {
                string s = p.EvaluatedInclude;
                if (p.ItemType == "EmbeddedResource")
                {
                    if (p.HasMetadata("DependentUpon") == true)
                    {
                        string de = p.GetMetadataValue("DependentUpon");

                        if (di.ContainsKey(de) == false)
                            di.Add(de, new ArrayList());

                        ArrayList B = di[de];

                        B.Add(p);
                    }
                }
                else if (p.ItemType == "Compile")
                {
                    if (p.HasMetadata("SubType") == true)
                    {
                        string[] cc = p.EvaluatedInclude.Split("\\".ToCharArray());
                        string de = cc[cc.Length - 1];

                        if (di.ContainsKey(de) == false)
                            di.Add(de, new ArrayList());

                        ArrayList B = di[de];

                        B.Add(p);
                    }
                    else
                    if (p.HasMetadata("DependentUpon") == true)
                    {
                        string de = p.GetMetadataValue("DependentUpon");

                        if (di.ContainsKey(de) == false)
                            di.Add(de, new ArrayList());

                        ArrayList B = di[de];

                        B.Add(p);
                    }
                }

                else if (p.ItemType == "EmbeddedResource")
                {
                    if (p.HasMetadata("DependentUpon") == true)
                    {
                        string de = p.GetMetadataValue("DependentUpon");

                        if (di.ContainsKey(de) == false)
                            di.Add(de, new ArrayList());

                        ArrayList B = di[de];

                        B.Add(p);
                    }
                }
            }
            return di;
        }


        public Microsoft.Build.Evaluation.Project LoadProjectToMemory()
        {
            MemoryStream dd = new MemoryStream();
            TextWriter xml = new StreamWriter(dd);
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);
            pc.Save(xml);
            pc.ProjectCollection.UnloadAllProjects();

            MemoryStream bb = new MemoryStream(dd.ToArray());
            XmlReader xmls = new XmlTextReader(bb);
            pc = new Microsoft.Build.Evaluation.Project(xmls);

            return pc;
        }

        public Microsoft.Build.Evaluation.Project LoadProject(Microsoft.Build.Evaluation.Project pc)
        {
            if (!File.Exists(FileName))
                return null;

            if (pc == null)
            {
                pc = new Microsoft.Build.Evaluation.Project(FileName);
                return pc;
            }
            else
            {
                //XmlReader xml = new XmlTextReader(dd);
                //string s = xml.ReadOuterXml();
                //Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(xml);
                return pc;
            }
        }


        public ArrayList SetCompileItems(ArrayList L, Microsoft.Build.Evaluation.Project pc = null)
        {
            pc = LoadProject(pc);

            if (pc == null)
                return L;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Compile", false);

            foreach (string p in L)
            {
                string refs = GetRelativePath(s, p);

                int i = R.IndexOf(refs);

                if (i < 0)

                    pc.AddItem("Compile", refs);
            }

            //  <Compile Include="Controls\UserControl1.cs">
            //  <SubType>UserControl</SubType>
            //  </Compile>
            //  <Compile Include="Controls\UserControl1.Designer.cs">
            //  <DependentUpon>UserControl1.cs</DependentUpon>
            //  </Compile>

            //<Compile Include="Controls\Component1.cs">
            //<SubType>Component</SubType>
            //</Compile>

            //<Compile Include="Controls\Component1.Designer.cs">
            //<DependentUpon>Component1.cs</DependentUpon>
            //</Compile>

            pc.Save(FileName);

            //MemoryStream dd = new MemoryStream();
            //StreamWriter w = new StreamWriter(dd);
            //pc.Save(w);

            //StreamReader r = new StreamReader(dd);
            //XmlReader xml = XmlReader.Create(r);
            //Microsoft.Build.Evaluation.Project pp = new Microsoft.Build.Evaluation.Project(xml);

            //pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public void CreateFolder(string folder, VSProjectItem ps)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Folder", false);

            if (ps == null || ps.ItemType != "Folder")
            {
                string refs = GetRelativePath(s, folder);

                int i = R.IndexOf(refs);

                if (i < 0)

                    pc.AddItem("Folder", refs);
            }
            else
            {
                string g = ps.Include;

                g = g + "\\" + folder;

                string refs = GetRelativePath(s, g);

                int i = R.IndexOf(refs);

                if (i < 0)
                    pc.AddItem("Folder", refs);
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }

        public void RemoveItems(ArrayList L)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            foreach (string s in L)
            {
                if (s == null)
                    continue;

                string name = s.Replace("- Compile", "").Trim();

                ICollection<Microsoft.Build.Evaluation.ProjectItem> c = pc.GetItemsByEvaluatedInclude(name);

                ArrayList F = new ArrayList();

                foreach (Microsoft.Build.Evaluation.ProjectItem pp in c)
                    F.Add(pp);

                foreach (Microsoft.Build.Evaluation.ProjectItem pp in F)
                {
                    if (pp.UnevaluatedInclude == name)
                        pc.RemoveItem(pp);
                }
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }

        public void RemoveReference(VSProjectItem p)
        {
            if (p.ItemType != "Reference")
                return;

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            ICollection<Microsoft.Build.Evaluation.ProjectItem> c = pc.GetItemsByEvaluatedInclude(p.Include);

            foreach (Microsoft.Build.Evaluation.ProjectItem pp in c)
            {
                if (pp.ItemType == "Reference")
                    pc.RemoveItem(pp);
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }
        public void RemoveReference(string file)
        {

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string libName = Path.GetFileName(file).Replace(".dll", "");

            string s = Path.GetDirectoryName(FileName);

            string r = GetRelativePath(s, file);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> c = pc.GetItemsByEvaluatedInclude(libName);

            foreach (Microsoft.Build.Evaluation.ProjectItem pp in c)
            {
                if (pp.ItemType == "Reference")
                    pc.RemoveItem(pp);
            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();
        }
        public void CleanProject()
        {
            string s = GetOutDirs();

            if (s == "")
                return;

            DirectoryInfo d = new DirectoryInfo(s);

            if (d.Exists == true)
            {
                FileInfo[] f = d.GetFiles();

                foreach (FileInfo b in f)
                {
                    try
                    {
                        string exts = Path.GetExtension(b.FullName);
                        if (exts == ".dll" || exts == ".exe")
                            b.Delete();
                    }
                    catch (Exception e) { }
                }
            }
        }

        public string GetProjectExec(string file)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(file);

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");

            string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue + ".exe";

            if (outtype.EvaluatedValue == "Library")
                s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue + ".dll";

            pc.ProjectCollection.UnloadAllProjects();

            string filebase = Path.GetDirectoryName(file);

            return filebase + "\\" + s;
        }

        public void SetProperty(string p, string value)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            pc.SetProperty(p, value);

            //pc.Save();

            pc.ProjectCollection.UnloadAllProjects();
        }

        public void SetProperties(string proj, string platform)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            SetProperty("Configuration", proj, pc);

            SetProperty("Platform", platform, pc);

            pc.ProjectCollection.UnloadAllProjects();
        }

        public void SetProperty(string p, string value, bool unload)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            pc.SetProperty(p, value);

            pc.Save();

            if (unload == true)
                pc.ProjectCollection.UnloadAllProjects();
        }

        public void SetProperty(string p, string value, Microsoft.Build.Evaluation.Project pc)
        {
            // Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            pc.SetProperty(p, value);

            pc.Save();
        }

        public string SetProperties(string file)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(file);

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");

            string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue + ".exe";

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        //static void runCommand()
        //{
        //    //* Create your Process
        //    Process process = new Process();
        //    process.StartInfo.FileName = "cmd.exe";
        //    process.StartInfo.Arguments = "/c DIR";
        //    process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.RedirectStandardError = true;
        //    //* Set your output and error (asynchronous) handlers
        //    process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
        //    process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
        //    //* Start process and handlers
        //    process.Start();
        //    process.BeginOutputReadLine();
        //    process.BeginErrorReadLine();
        //    process.WaitForExit();
        //}
        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }

        public void RunProject()
        {
            string g = Path.GetFileName(FileName);

            string p = Path.GetFullPath(FileName).Replace(g, "");

            string s = SetProperties(FileName);

            string r = p + "\\" + s;

            r = GetProjectExec();

            if (File.Exists(r) == false)
                return;

            cmds.RunExternalExe(r, "");
        }

        public void MergeProject(VSProject p, string folder = "folder2")
        {
            ArrayList L = p.GetCompileItems();

            string project = this.GetProjectFolder();

            string pr = p.GetProjectFolder();

            ArrayList F = new ArrayList();

            foreach (string c in L)
            {
                if (File.Exists(c) == false)
                    continue;

                string file = c.Replace(pr + "\\", "");

                string d = project + "\\" + folder + "\\" + file;

                string b = Path.GetFullPath(d);

                F.Add(b);

                if (File.Exists(d) == true)
                    continue;

                string[] cc = d.Split("\\".ToCharArray());

                string folders = "";

                int i = 0;
                foreach (string s in cc)
                {
                    if (i == 0)
                        folders = s;
                    else
                        folders += "\\" + s;

                    if (Directory.Exists(folders) == false)
                        Directory.CreateDirectory(folders);


                    i++;

                    if (i == cc.Length - 1)
                        break;
                }


                File.Copy(c, d);
            }

            this.SetCompileItems(F);
        }
    }
}