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

    using System.Diagnostics;


using System.Threading;

using VSParsers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AIMS.Libraries.Scripting.ScriptControl.Properties;
using Microsoft.CodeAnalysis.FindSymbols;

using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit;
//using NUnit.Framework;

namespace VE_Tests
    {

       
        public class Context
        {
        public TestContext testContext { get; set; }
        public string info { get; set; }
        }

    public class VSSolutionStub
    {
        private Dictionary<string, ArrayList> dict { get; set; }


        public void GetSubTypes(Microsoft.Build.Evaluation.Project pc, VSProject vp)
        {
            dict = vp.GetSubtypes(pc);
        }

        
        public void CheckIfCompileItemsArePresentInSolutionExplorerTree(TreeView v)
        {
            Microsoft.Build.Evaluation.Project pc = null;


            TreeNode b = v.Nodes[0];


            VSSolution vs = v.Tag as VSSolution;

           // Assert.IsNotNull(b);

           // Assert.IsNotNull(vs);

            if (vs == null)
                return;

            foreach (VSProject p in vs.projects)
            {

                if (p.ProjectType == "SolutionFolder")
                    continue;

                pc = p.LoadProjectToMemory();
                GetSubTypes(pc, p);

                VSParsers.msbuilder_alls bb = new VSParsers.msbuilder_alls();

                TreeNode node = bb.GetProjectNode(b, p);

             //   Assert.That(node, Is.Not.Null);

                int i = 0;
                foreach (string s in dict.Keys)
                {
                    ArrayList L = dict[s];

                    foreach (Microsoft.Build.Evaluation.ProjectItem pp in L)
                    {
                        string type = pp.ItemType;
                        string include = pp.EvaluatedInclude;

                        TreeNode ng = bb.FindNodesIncludes(node, include);

                        //Assert.That(ng, Is.Not.Null);

                        if (ng != null)
                            i++;
                    }
                }
            }
        }
        public ExplorerForms ef { get; set; }
        public string RandomCompileItem()
        {
            Random r = new Random();
            var b = ef.Command_CompileItemsByProjectName();
            if (b.Count == 0)
                return "";
            int index = (int)(b.Count * r.NextDouble());
            return b[index];
        }
        public void OpenFile(string file)
        {
            CountdownEvent are = new CountdownEvent(1);
            ef.Invoke(new Action(()=> ef.Command_OpenFile(file, are) ));
            //Task.Run(() => are.WaitOne()).Wait();
            //VSSolution vs = ef.GetVSSolution();
            //ManualResetEvent mre = vs.SynchronizedEventForCompileFile(file);
            //Task.Run(() => mre.WaitOne()).Wait();
        }
        async public void OpenFile(string file, CountdownEvent e)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            ef.Invoke(new Action(() => ef.Command_OpenFile(file, e)));
            await Task.Run(() => are.WaitOne());
            VSSolution vs = ef.GetVSSolution();
            ManualResetEvent mre = vs.SynchronizedEventForCompileFile(file);
            Task.Run(() => mre.WaitOne()).Wait();
            
        }
        public bool CheckIfOpened(string file)
        {
            List<AvalonDocument> b = ef.Command_OpenedFiles();
            foreach (var c in b)
                if (c.FileName == file)
                    return true;
            return false;
        }
        public void MethodMembers()
        {
            string file = "";
            
            VSSolution vs = ef.GetVSSolution();
            List<MethodDeclarationSyntax> cs = new List<MethodDeclarationSyntax>();
            while (cs.Count <= 0)
            {
                file = RandomCompileItem();
                if (string.IsNullOrEmpty(file))
                    continue;
                OpenFile(file);
                //if (!CheckIfOpened(file))
                //    return;
                SyntaxTree syntaxTree = vs.GetSyntaxTree(file);
                cs = Syntaxer.MethodMembers(vs, syntaxTree);
            }
            MethodDeclarationSyntax s = cs[0];
            string name = s.Identifier.Text;
            List<LocationSource> d = ef.Command_FindString("." + name, "", "", "", "", SearchDomain.solution);

            foreach (MethodDeclarationSyntax me in cs)
            {
                var symbol = vs.GetSymbolAtLocation(me.Identifier.SpanStart, file);
                VSProject vp = vs.GetProjectbyCompileItem(file);
                var refs = vs.GetAllSymbolReferences(symbol, file, vp).ToList();
                MessageBox.Show("References - " + refs.Count);
                foreach (LocationSource c in d)
                {
                    foreach (ReferencedSymbol se in refs)
                    {

                        foreach (ReferenceLocation b in se.Locations)
                        {
                            if (c.textSpan.Start > b.Location.SourceSpan.Start && c.Source.FilePath == b.Location.SourceTree.FilePath)
                                MessageBox.Show("Location found " + b.Location.ToString());
                        }

                    }
                }
            }
        }
    }


    [TestClass]
    public class VSExplorerSolution_Test
        {

       

        private VSSolution vs { get; set; }

        private TreeView b { get; set; }

        private ArrayList F { get; set; }

        public ExplorerForms ef { get; set; }

        [TestMethod]
        //[Apartment(ApartmentState.STA)]
        async public void Open_Solution_Tests()
        {

            //Context c = new Context();
            //c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            ManualResetEvent mre = new ManualResetEvent(false);
            ef.BeginInvoke(new Action(() => {
                
                ef.Command_OpenSolution(folder, mre);
                
            }));
            await Task.Run(() => mre.WaitOne());

        }
        [TestMethod]
        public CountdownEvent OpenSolutionAndFilesFromProject_Test()
        {
           
            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            ef.Invoke(new Action(() => {
                ef.Command_CloseSolution();
                ef.Command_OpenSolution(folder);
            }));
            var fs = ef.Command_CompileItemsByProjectName("WpfView");
            CountdownEvent e = new CountdownEvent(fs.Count);
            ef.BeginInvoke(new Action(() =>
            {
                foreach (var s in fs)
                    ef.Command_OpenFile(s, e);
            }));
            return e;   
        }
        [TestMethod]
        public void OpenSolutionAndFindMethodsInArbitraryFile()
        {
            
            //string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            
            VSSolutionStub vss = new VSSolutionStub();
            vss.ef = ef;
            vss.MethodMembers();


        }
        [TestMethod]
        public void CheckIfCompileItemsArePresentInSolutionTree()
        {
    
            //string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            //ef.Invoke(new Action(() => {
            //    ef.Command_CloseSolution();
            //    ef.Command_OpenSolution(folder);
            //}));

            //Task.Run(() => mre.WaitOne()).Wait();
            VSSolutionStub vss = new VSSolutionStub();
            TreeView v = ef.Command_SolutionExplorerTreeView();
            vss.CheckIfCompileItemsArePresentInSolutionExplorerTree(v);
            //NUnit.Framework.TestContext.WriteLine("Open solution for " + folder + " and check compile items completed successfully ");
            //mf.BeginInvoke(new Action(() => this.Close()));

        }
        [TestMethod]
       //[Apartment(ApartmentState.STA)]
        public void Open_Solution_Test()
        {

                       
            Context c = new Context();
            //c.testContext = new TestContext(TestExecutionContext.CurrentContext);
            c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //   Application.Run(new TestForm(TestForm.PerformDelegateName.opensolution, c));
            Application.Run(new ExplorerForms());
        }
        [TestMethod]
        //[Apartment(ApartmentState.STA)]
        public void Open_Solution_And_Project_Files_Test()
        {

            Context c = new Context();
            c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm(TestForm.PerformDelegateName.opensolutionandfilesfromproject, c));
            
        }
        [TestMethod]
        //[Apartment(ApartmentState.STA)]
        public void Open_Solution_And_Project_Files_With_Repeat_Test()
        {

            Context c = new Context();
            c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm(TestForm.PerformDelegateName.opensolutionandfilesfromprojectwithrepeat, c));

        }
        [TestMethod]
        //[Apartment(ApartmentState.STA)]
        public void Open_Solution_And_CheckIfCompileItemsArePresentInSolutionTreeView_Test()
        {

            Context c = new Context();
            //c.testContext = new TestContext(TestExecutionContext.CurrentContext);
            c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm(TestForm.PerformDelegateName.opensolutionandcheckifcompileitemsarepresentinsolutiontree, c));

        }
        [TestMethod]
        //[Apartment(ApartmentState.STA)]
        public void Open_Solution_And_FindMethodsInRandomFile_Test()
        {

            Context c = new Context();
          //  c.testContext = new TestContext(TestExecutionContext.CurrentContext);
            c.info = "f:\\ve\\VE-2016\\VE-Tests\\Sources\\Live-Charts-master\\LiveCharts.sln";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm(TestForm.PerformDelegateName.findmethodsinarbitraryfile, c));

        }
    }
    }
