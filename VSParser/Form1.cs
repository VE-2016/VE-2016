using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSParsers;
using VSProvider;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            CompileSolution();
        }
        private MSBuildWorkspace workspace { get; set; }

        Dictionary<string, Compilation> comp { get; set; }

        Dictionary<string, List<INamespaceOrTypeSymbol>> named { get; set; }

        public async void CompileSolution()
        {

            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

            string solutionFileName = "C:\\MSBuildProjects-beta\\VStudio.sln";


            VSSolutionLoader rw = new VSSolutionLoader();
            System.Windows.Forms.TreeView vv = rw.LoadProject(solutionFileName);

            VSSolution vs = vv.Tag as VSSolution;

            MessageBox.Show("VSolution loaded.." + vs.Name);

            comp = new Dictionary<string, Compilation>();

            named = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

            workspace = null;
            
            ProjectDependencyGraph projectGraph = null;

            Microsoft.CodeAnalysis.Solution solution = null;
            workspace = MSBuildWorkspace.Create();
            solution =await  workspace.OpenSolutionAsync(solutionFileName);

            projectGraph = solution.GetProjectDependencyGraph();

            foreach (ProjectId projectId in projectGraph.GetTopologicallySortedProjects())
            {
                Stopwatch sw = Stopwatch.StartNew();

                Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;

                sw.Stop();

                System.Diagnostics.Debug.WriteLine("Time taken compilation creation: {0}ms", sw.Elapsed.TotalMilliseconds);

                Microsoft.CodeAnalysis.Project project = solution.GetProject(projectId);

                comp.Add(project.FilePath, projectCompilation);

               // List<INamespaceOrTypeSymbol> ns = GetAllTypes(project.FilePath);

               // named.Add(project.FilePath, ns);
            }
        }

    }
}
