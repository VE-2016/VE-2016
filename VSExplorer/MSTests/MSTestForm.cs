using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;

namespace WinExplorer
{
    public partial class MSTestForm : Form
    {
        public MSTestForm()
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
            g.Images.Add(Resources.StatusInformation_exp_16x);
            g.Images.Add(Resources.StatusOK_cyan_16x);
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
        }

        private void Se_Click(object sender, EventArgs e)
        {
            string text = se.Text;
            string[] dd = text.Split(" ".ToCharArray());
            ExplorerForms.ef.OpenFileXY(filename, "0", line.ToString(), 0);
        }

        RichTextBox rb { get; set; }

        WinExplorer.UI.LabelEx le { get; set; }

        WinExplorer.UI.LabelEx se { get; set; }

        VSProject vpp { get; set; }

        int line = 0;

        string filename { get; set; }

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

        string FindSource(VSProject vp, string method)
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

            foreach(SyntaxTree syntaxTree in comp.SyntaxTrees)
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

        Font font { get; set; }
        Font fontstroked { get; set; }

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

        public void LoadTest(VSProject vp, List<string> s)
        {


            if (s.Count <= 0)
                return;

            TreeNode node = new TreeNode();
            node.Text = "Not Run Tests";

            v.Nodes.Add(node);

            foreach(string c in s)
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
    }
}
