using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using WinExplorer.Services;

namespace WinExplorer
{
    public partial class TreeViewer_WinformsHost : Form
    {
        public TreeViewer_WinformsHost()
        {
            InitializeComponent();
            v = treeView1;
            ps = panel1;
            pl = panel2;
            st = splitter1;
            SuspendLayout();
            v.Parent = null;
            pl.Parent = null;
            st.Parent = null;

            ImageList g = new ImageList();
            g.Images.Add(ve_resource.StatusInformation_exp_16x);
            g.Images.Add(ve_resource.StatusOK_cyan_16x);
            v.ImageList = g;

            v.Parent = ps;
            v.Dock = DockStyle.Fill;

            st.Parent = ps;
            st.Dock = DockStyle.Bottom;

            pl.Parent = ps;
            pl.Dock = DockStyle.Bottom;

            ResumeLayout();

            font = toolStripLabel1.Font;

            fontstroked = new Font(font, FontStyle.Underline);

            toolStripLabel1.MouseHover += Label_MouseHover;
            toolStripLabel1.MouseLeave += Label_MouseLeave;
            toolStripSplitButton2.MouseHover += Label_MouseHover;
            toolStripSplitButton2.MouseLeave += Label_MouseLeave;
            toolStripSplitButton2.Click += ToolStripSplitButton2_Click;
            toolStripSplitButton1.Click += ToolStripSplitButton2_Click;
            toolStripSplitButton1.MouseHover += Label_MouseHover;
            toolStripSplitButton1.MouseLeave += Label_MouseLeave;
            toolStripSplitButton2.DropDown.ItemClicked += DropDown_ItemClicked;
            toolStripSplitButton1.DropDown.ItemClicked += DropDown_ItemClicked;

            le = labelEx1;
            le.Text = "Copy All";
            le.Visible = false;

            se = labelEx2;
            se.Text = "";
            se.Visible = false;

            se.Click += Se_Click;

            rb = richTextBox1;

            v.AfterSelect += V_AfterSelect;

            v.DoubleClick += V_DoubleClick;
        }

        private void V_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = v.SelectedNode;
            if (node == null)
                return;
            MethodInfo method = node.Tag as MethodInfo;
            if (method == null)
                return;
            var type = method.DeclaringType;
            var p = type.GetProperty("ef");
            var obj = Activator.CreateInstance(type);
            p.SetValue(obj, ExplorerForms.ef);
            Task.Run(async () =>
            {
                method.Invoke(obj, new object[0]);
            });
        }

        private void Se_Click(object sender, EventArgs e)
        {
            string text = se.Text;
            string[] dd = text.Split(" ".ToCharArray());
            ExplorerForms.ef.OpenFileXY(filename, 0, line, 0);
        }

        private RichTextBox rb { get; set; }

        private WinExplorer.UI.LabelEx le { get; set; }

        private WinExplorer.UI.LabelEx se { get; set; }

        private VSProject vpp { get; set; }

        private int line = 0;

        private string filename { get; set; }

        private void V_AfterSelect(object sender, TreeViewEventArgs e)
        {
            rb.Clear();
            rb.AppendText("\n" + e.Node.Text);
            SetAsBold(e.Node.Text, rb);
            le.Visible = true;
            TreeNode node = e.Node;
            if (node == null)
                return;
            VSProject vp = node.Tag as VSProject;
            if (vp == null)
                return;
            vpp = vp;
            string s = FindSource(vp, node.Text.Trim());
            if (string.IsNullOrEmpty(s))
                return;
            se.Text = s;
            rb.AppendText("\n" + "Source: ");
            int index = rb.Text.IndexOf("Source:");
            Point textLocation = richTextBox1.GetPositionFromCharIndex(index + 7);
            textLocation.Y += 2;
            se.Location = textLocation;
            se.Visible = true;
        }

        private string FindSource(VSProject vp, string method)
        {
            VSSolution vs = vp.vs;

            if (vs == null)
                return "";

            Compilation comp = vs.comp[vp.FileName];

            if (comp == null)
                return "";

            //MessageBox.Show("Compilation found for " + comp.SourceModule.Name);

            //var sourceText = SourceText.From(text);
            //Create new document
            //var doc = ws.AddDocument(project.Id, "NewDoc", sourceText);
            //Get the semantic model
            //var model = doc.GetSemanticModelAsync().Result;
            //Get the syntax node for the first invocation to M()
            //var methodInvocation = doc.GetSyntaxRootAsync().Result.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
            //var methodSymbol = model.GetSymbolInfo(methodInvocation).Symbol;
            //Finds all references to M()
            //var referencesToM = SymbolFinder.FindReferencesAsync(methodSymbol, doc.Project.Solution).Result;

            foreach (SyntaxTree syntaxTree in comp.SyntaxTrees)
            {
                var members = syntaxTree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
                foreach (var member in members)
                {
                    var methods = member as MethodDeclarationSyntax;
                    if (methods != null)
                    {
                        if (methods.Identifier.Value.ToString() == method)
                        {
                            filename = syntaxTree.FilePath;
                            return Path.GetFileName(syntaxTree.FilePath) + " line " + methods.GetLocation().SourceSpan.Start;
                        }
                    }
                }
            }
            return "";
        }

        public void SetAsBold(string s, RichTextBox rbDesc)
        {
            rbDesc.Find(s, RichTextBoxFinds.MatchCase);

            rbDesc.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Bold);
            rbDesc.SelectionColor = Color.Black;
        }

        private void ToolStripSplitButton2_Click(object sender, EventArgs e)
        {
            ToolStripItem b = sender as ToolStripItem;
            b.ForeColor = Color.Black;
            b.Font = font;
        }

        private void DropDown_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripDropDownMenu b = sender as ToolStripDropDownMenu;
            b.OwnerItem.Text = e.ClickedItem.Text;
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            ToolStripItem b = sender as ToolStripItem;
            b.ForeColor = Color.Black;
            b.Font = font;
        }

        private Font font { get; set; }
        private Font fontstroked { get; set; }

        private void Label_MouseHover(object sender, EventArgs e)
        {
            ToolStripItem b = sender as ToolStripItem;
            b.ForeColor = Color.Blue;
            b.Font = fontstroked;
        }

        public TreeView v { get; set; }

        public Splitter st { get; set; }

        public Panel pl { get; set; }

        public Panel ps { get; set; }

        public void ClearTests()
        {
            v.Nodes.Clear();
        }

        public void LoadTest(List<MethodInfo> methods)
        {
            List<string> m = new List<string>();
            if (methods.Count <= 0)
                return;
            TreeNode node = new TreeNode();
            node.Text = "Not Run Tests";

            v.Nodes.Add(node);

            foreach (MethodInfo c in methods)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = c.Name;
                nodes.ImageKey = "StatusInformation_exp_16x";
                nodes.SelectedImageKey = "StatusInformation_exp_16x";
                nodes.Tag = c;

                node.Nodes.Add(nodes);
            }
        }

        public void LoadTest(VSProject vp, List<string> s)
        {
            if (s.Count <= 0)
                return;
            TreeNode node = new TreeNode();
            node.Text = "Not Run Tests";

            v.Nodes.Add(node);

            foreach (string c in s)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = c;
                nodes.ImageKey = "StatusInformation_exp_16x";
                nodes.SelectedImageKey = "StatusInformation_exp_16x";
                nodes.Tag = vp;
                node.Nodes.Add(nodes);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        public void LoadTestNodes(List<MethodInfo> methods)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadSelfTests();
        }

        public void LoadSelfTests()
        {
            ClearTests();
            VSSolution vs = ExplorerForms.ef.GetVSSolution();
            if (vs == null)
                return;
            // foreach (VSProject vp in vs.Projects)
            {
                //VSProject vp = vs.GetProjectbyName("VE-Tests");
                //   msbuilder_alls.MSTest mstest = new msbuilder_alls.MSTest();
                //   mstest.msTestPath = AppDomain.CurrentDomain.BaseDirectory + "\\Extensions\\_starters-discovery.bat";
                var names = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins\\VE-Tests.dll";

                var types = Reflection.AttributesOfTypes(names, "TestClassAttribute");

                //var methods = Reflection.AttributesOfMethods(names, "VSExplorerSolution_Test", "TestMethodAttribute");

                if (types == null)
                    return;
                //    continue;
                var methods = Reflection.AttributesOfMethodsFromTypes(names, types, "TestMethodAttribute");

                LoadTest(methods);
            }

            //string name = AppDomain.CurrentDomain.BaseDirectory + "" + "Plugins\\VE-Tests.dll";

            ////var types = Reflection.AttributesOfTypes(name, "TestClassAttribute");
            ////var methods = Reflection.AttributesOfMethods(name, "VSExplorerSolution_Test", "TestMethodAttribute");

            ////var methods = Reflection.AttributesOfMethods(name, "VSExplorerSolution_Test", "TestMethodAttribute");
            ////  var type = ats.Select(s => s).Where(s => s.Name == "VSExplorerSolution_Test").FirstOrDefault();
            ////  var attrs = Reflection.Attributes<TestClassAttribute>(ats).Select(s => s).Where(s => s.Name == "VSExplorerSolution_Test").FirstOrDefault();
            ////  var methods = Reflection.AttributesOfMethods<TestMethodAttribute>(type);
            //var p = types[0].GetProperty("ef");
            //var obj = Activator.CreateInstance(types[0]);
            //p.SetValue(obj, this);

            ////var method = methods.Where(s => s.Name == "OpenSolutionAndFilesFromProject_Test").FirstOrDefault();
            ////var method = methods.Where(s => s.Name == "CheckIfCompileItemsArePresentInSolutionTree").FirstOrDefault();
            //var method = methods.Where(s => s.Name == "OpenSolutionAndFindMethodsInArbitraryFile").FirstOrDefault();
        }
    }
}