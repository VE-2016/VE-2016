// VSProject

using System;

//using AbstractX.Contracts;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

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

        private VSSolution solution;
        private string projectFileName;
        private object internalSolutionProject;
        private List<VSProjectItem> items;

        public string Name { get; set; }

        public string ProjectType { get; private set; }

        public string RelativePath { get; private set; }

        public string ProjectGuid { get; private set; }

        public string FileName
        {
            get
            {
                return projectFileName;
            }
            set
            {
                projectFileName = value;

            }
        }

        static VSProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);

            s_ProjectRootElement = Type.GetType("Microsoft.Build.Construction.ProjectRootElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_ProjectRootElementCache = Type.GetType("Microsoft.Build.Evaluation.ProjectRootElementCache, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectRootElement_Items = s_ProjectRootElement.GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);
        }

        public IEnumerable<VSProjectItem> Items
        {
            get
            {
                return items;
            }
            set
            {

                items =(List<VSProjectItem>) value;
            }
        }

        public VSProject()
        {

            items = new List<VSProjectItem>();

        }

        public VSProject(VSSolution solution, object internalSolutionProject)
        {
            this.Name = s_ProjectInSolution_ProjectName.GetValue(internalSolutionProject, null) as string;
            this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(internalSolutionProject, null).ToString();
            this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(internalSolutionProject, null) as string;
            this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(internalSolutionProject, null) as string;

            this.solution = solution;
            this.internalSolutionProject = internalSolutionProject;

            this.projectFileName = Path.Combine(solution.SolutionPath, this.RelativePath);

            items = new List<VSProjectItem>();

            if (this.ProjectType == "KnownToBeMSBuildFormat")
            {
                this.Parse();
            }
        }

        private void Parse()
        {
            try
            {
                var stream = File.OpenRead(projectFileName);
                var reader = XmlReader.Create(stream);
                var cache = s_ProjectRootElementCache.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { true });
                ConstructorInfo[] inf = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                var rootElement = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { reader, cache, true });

                stream.Close();

                var collection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);

                foreach (var item in collection)
                {
                    items.Add(new VSProjectItem(this, item));
                }
            }
            catch (Exception e) { }
        }

        public IEnumerable<VSProjectItem> EDMXModels
        {
            get
            {
                return this.items.Where(i => i.ItemType == "EntityDeploy");
            }
        }

        public void Dispose()
        {
        }

        public string GetOutDir()
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

            if(outtype.EvaluatedValue == "WinExe")
                ext = ".exe";


            string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue + ext;


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

            string r = p + "\\" + s;

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

            
            foreach(Microsoft.Build.Evaluation.ProjectMetadata m in p.Metadata){

                if (m.Name == "HintPath")
                    return m.UnevaluatedValue;

            }


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
            catch (Exception e) {  }
            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);




            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {

                string file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                string project = Path.GetFileNameWithoutExtension(file);


                file = p.EvaluatedInclude;

                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }


        public ArrayList GetProjectReferences(string name)
        {

            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

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


        public ArrayList GetItems(string name, bool folds)
        {

            ArrayList L = new ArrayList();

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return L;

            string folder = Path.GetDirectoryName(FileName);

            ICollection<Microsoft.Build.Evaluation.ProjectItem> s = pc.GetItems(name);

                


            foreach (Microsoft.Build.Evaluation.ProjectItem p in s)
            {

                string file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                if (folds == false)
                    file = p.EvaluatedInclude;

                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

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

                string file = Path.GetFullPath(folder + "\\" + p.EvaluatedInclude);

                if (folds == false)
                    file = p.EvaluatedInclude;

                L.Add(file);
            }

            pc.ProjectCollection.UnloadAllProjects();

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

            ArrayList R = GetItems(pc, "ProjectReference", false);

            foreach (string p in L)
            {

                string refs = GetRelativePath(s, p);

                int i = R.IndexOf(refs);

                if(i < 0)

                   pc.AddItem("ProjectReference", refs);
  

            }

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return L;
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


        public ArrayList SetCompileItems(ArrayList L)
        {

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

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

            pc.Save(FileName);

            pc.ProjectCollection.UnloadAllProjects();

            return L;
        }

        public void CreateFolder(string folder, VSProjectItem ps)
        {

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(FileName);

            if (pc == null)
                return;

            string s = Path.GetDirectoryName(FileName);

            ArrayList R = GetItems(pc, "Folder", false);

            if(ps == null || ps.ItemType != "Folder")
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

        public void CleanProject()
        {

            string s = GetOutDirs();

            DirectoryInfo d = new DirectoryInfo(s);

            FileInfo[] f = d.GetFiles();

            foreach(FileInfo b in f){

                try
                {
                    b.Delete();
                }
                catch (Exception e) { }

            }

        }

        public string SetProperties(string file)
        {




            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(file);


            if (pc == null)
                return "";



            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");


            string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue;


            pc.ProjectCollection.UnloadAllProjects();

            return s;




        }


    
        public void RunProject()
        {

            string g = Path.GetFileName(FileName);

            string p = Path.GetFullPath(FileName).Replace(g, "");

            string s = SetProperties(FileName);

            string r = p + "\\" + s;

            WinExplorer.cmds.RunExternalExe(r, "");

        }
    }
}