using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using WinExplorer;

using Microsoft.Build.Evaluation;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace VE_Tests
{
   
    [TestClass]
    public class VSExplorerTest
    {
        private VSSolution vs { get; set; }

        private TreeView b { get; set; }

        private ArrayList F { get; set; }


        ArrayList FolderSearch(string folder, string ext)
        {
            try
            {
                F = new ArrayList();

                foreach (string d in Directory.GetDirectories(folder))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        if (f.EndsWith(ext))
                            F.Add(f);

                    }
                    FolderSearch(d, ext);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return F;
        }

     
        [TestMethod]
        public void LoadVESolution()
        {

            string folder = AppDomain.CurrentDomain.BaseDirectory;

            string folders = folder + "\\..\\..\\..\\VStudio.sln";

            folders = "C:\\Bin\\roslyn-master\\Roslyn.sln";


            b = VSExplorer.LoadProject(folders);

            Assert.IsNotNull(b);

            vs = b.Tag as VSSolution;



            F = new ArrayList();
            F.Add(vs);
                Assert.IsNotNull(vs);

            Directory.SetCurrentDirectory(folder);

        }
        public void MockProject()
        {
            ArrayList S = VSSolution.GetVSProjects(vs.solutionFileName);

            string content = File.ReadAllText(vs.solutionFileName);

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

            string file = vs.solutionFileName;

            string name = Path.GetFileName(file).Trim();

            file = file.Replace(name, "_" + name);

            File.WriteAllText(file, content);
        }

        public void ChangeSolution()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            if (vs == null)
                return;

            foreach (VSProject p in vs.projects)
            {
                if (File.Exists(p.FileName) == false)
                    continue;

                pc = p.LoadProjectToMemory();

                p.Shuffle(pc);
            }

            MockProject();
        }

        private Dictionary<string, ArrayList> dict { get; set; }


        public void GetSubTypes(Microsoft.Build.Evaluation.Project pc, VSProject vp)
        {
            dict = vp.GetSubtypes(pc);
        }
        public ArrayList GetAllItems(Microsoft.Build.Evaluation.Project pc, VSProject vp)
        {
            
            return vp.GetAllItems(pc);
        }
        public Dictionary<string, ArrayList> ReadAllProjectItemsFromFile(string file)
        {
            Dictionary<string, ArrayList> dict = new Dictionary<string, ArrayList>();

            VSSolution vs = new VSSolution(Path.GetFullPath(file));
            
            foreach(VSProject p in vs.projects)
            {
                ArrayList P = new ArrayList();

                foreach(VSProjectItem pp in p.Items)
                {
                    P.Add(pp);
                }
                dict.Add(p.FileName, P);
            }
            return dict;
        }
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        [TestMethod()]
        public void CheckSolutionFormSubtypes()
        {

            LoadVESolution();

            Microsoft.Build.Evaluation.Project pc = null;

            if (vs == null)
                return;
            int c = 0;
            foreach (VSProject p in vs.projects)
            {
                pc = (Microsoft.Build.Evaluation.Project)p.LoadProjectToMemory();

                if (pc == null)
                    continue;
                this.GetSubTypes(pc, p);

                msbuilder_alls bb = new msbuilder_alls();

                TreeNode node = bb.GetProjectNode(b, p);

                Assert.IsNotNull(node);

                int i = 0;
                foreach (string s in dict.Keys)
                {
                    ArrayList L = dict[s];

                    foreach (Microsoft.Build.Evaluation.ProjectItem pp in L)
                    {
                        string type = pp.ItemType;
                        string include = pp.EvaluatedInclude;

                        TreeNode ng = bb.FindNodesIncludes(node, include);

                        Assert.IsNotNull(ng);

                        if (ng != null)
                            i++;
                        c++;
                        
                    }

                }
                Console.WriteLine("Projects " + p.Name + " - " + i.ToString() + " items have been checked");
            }
            Console.WriteLine("Test results\\n" + "Projects " + c.ToString() + " have been checked");

//            TestContext.WriteLine("Projects " + c.ToString() + " have been checked" );
        }
    
    //[TestMethod()]
    public void CheckSolutionAllItems()
    {

        LoadVESolution();

        Microsoft.Build.Evaluation.Project pc = null;

        if (vs == null)
            return;
        int c = 0;
        foreach (VSProject p in vs.projects)
        {
            pc = (Microsoft.Build.Evaluation.Project)p.LoadProjectToMemory();

            if (pc == null)
                continue;
            ArrayList L = this.GetAllItems(pc, p);

            msbuilder_alls bb = new msbuilder_alls();

            TreeNode node = bb.GetProjectNode(b, p);

            Assert.IsNotNull(node);

            int i = 0;
            //foreach (string s in dict.Keys)
            {
                //ArrayList L = dict[s];

                foreach (Microsoft.Build.Evaluation.ProjectItem pp in L)
                {
                    string type = pp.ItemType;
                    string include = pp.EvaluatedInclude;

                    TreeNode ng = bb.FindNodesIncludes(node, include);

                    Assert.IsNotNull(ng);

                    if (ng != null)
                        i++;
                    c++;

                }

            }
            Console.WriteLine("Projects " + p.Name + " - " + i.ToString() + " items have been checked");
        }
        Console.WriteLine("Test results: " + "Projects " + c.ToString() + " have been checked");

        
    }
        [TestMethod()]
        public void CheckSolutionAllItemsFromFile()
        {

            string folder = AppDomain.CurrentDomain.BaseDirectory;

            string folders = folder + "\\..\\..\\..\\VStudio.sln";

            LoadVESolution();

            //dict = ReadAllProjectItemsFromFile(folders);

            //Microsoft.Build.Evaluation.Project pc = null;

            if (vs == null)
                return;
            Console.WriteLine("Projects " + vs.projects.Count + " have been loaded");
            int c = 0;
            foreach (VSProject p in vs.projects)
            {
                msbuilder_alls bb = new msbuilder_alls();
                TreeNode node = bb.GetProjectNode(b, p);
                if(node == null)
                {
                    c++;
                    continue;
                }
                //Assert.IsNotNull(node);
                int i = 0;
                {
                    foreach (VSProjectItem pp in p.Items)
                    {
                        string type = pp.ItemType;
                        string include = pp.Include;
                        if (include.EndsWith("\\"))
                            include = include.Substring(0, include.Length - 1);
                        TreeNode ng = bb.FindNodesIncludes(node, include);
                        if (ng == null)
                        {
                            Console.WriteLine("Error: " + p.FileName);
                            Console.WriteLine("Type: " + pp.ItemType );
                            Console.WriteLine("Include: " + pp.Include);
                            Assert.IsNotNull(ng);
                        }
                        if (ng != null)
                            i++;
                        c++;
                    }
                }
                Console.WriteLine("Projects " + p.Name + " - " + i.ToString() + " items have been checked");
            }
            Console.WriteLine("Test results: " + "Projects " + c.ToString() + " have been checked");
        }
    }
}
