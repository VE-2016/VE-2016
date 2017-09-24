using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer
{
    public partial class VSExplorerForm : Form
    {
        public VSExplorerForm()
        {
            InitializeComponent();

            dg = dataGridView1;

            ng = treeView2;

            ns = treeView1;

            //ns.DrawMode = TreeViewDrawMode.OwnerDrawText;

            //ns.DrawNode += Ns_DrawNode;

            np = treeView3;

            sn = ns;

            fs = folderLister1;

            ns.AfterSelect += Ns_AfterSelect;

            Pages();
        }

        private string search { get; set; }

        //private void Ns_DrawNode(object sender, DrawTreeNodeEventArgs e)
        //{
        //    // Draw the background and node text for a selected node.
        //    if (search != null &&  search != "" && e.Node.Text.ToLower().Contains(search.ToLower()))
        //    {
        //        // Draw the background of the selected node. The NodeBounds
        //        // method makes the highlight rectangle large enough to
        //        // include the text of a node tag, if one is present.
        //        e.Graphics.FillRectangle(Brushes.Green, NodeBounds(e.Node));

        //        // Retrieve the node font. If the node font has not been set,
        //        // use the TreeView font.
        //        Font nodeFont = e.Node.NodeFont;
        //        if (nodeFont == null) nodeFont = ((TreeView)sender).Font;

        //        // Draw the node text.
        //        e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.Black,
        //            Rectangle.Inflate(e.Bounds, 2, 0));

        //        e.DrawDefault = false;
        //    }

        //    // Use the default background and node text.
        //    else
        //    {
        //        e.DrawDefault = true;
        //    }

        //}
        //// Returns the bounds of the specified node, including the region
        //// occupied by the node label and any node tag displayed.
        //private Rectangle NodeBounds(TreeNode node)
        //{
        //    // Set the return value to the normal node bounds.
        //    Rectangle bounds = node.Bounds;
        //    if (node.Tag != null)
        //    {
        //        // Retrieve a Graphics object from the TreeView handle
        //        // and use it to calculate the display width of the tag.
        //        Graphics g = ns.CreateGraphics();
        //        int tagWidth = (int)g.MeasureString
        //            (node.Tag.ToString(), ns.Font).Width + 6;

        //        // Adjust the node bounds using the calculated value.
        //        bounds.Offset(tagWidth / 2, 0);
        //        bounds = Rectangle.Inflate(bounds, tagWidth / 2, 0);
        //        g.Dispose();
        //    }

        //    return bounds;

        //}

        private void Ns_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = ns.SelectedNode;

            if (node == null)
                return;

            if (pg == null)
                return;

            pg.SelectedObject = node.Tag;
        }

        private DataGridView dg { get; set; }

        private TreeView ng { get; set; }

        private TreeView ns { get; set; }

        private TreeView np { get; set; }

        private FolderLister fs { get; set; }

        private VSSolution vs { get; set; }

        private VSProject vp { get; set; }

        private void folderLister1_Load(object sender, EventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CheckProjectItems();
        }

        public void Initialize()
        {
            dg.ColumnCount = 2;
            dg.Columns[0].Name = "Property";
            dg.Columns[1].Name = "Value";
            dg.Rows.Clear();

            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public void LoadAllProjectProperties()
        {
            Initialize();

            if (vp == null)
                return;

            //vp.SetProperty("Configuration", textBox2.Text);
            //vp.SetProperty("Platform", textBox3.Text);

            string proj = "";// textBox2.Text.Replace(" ", "");
            string platform = "";// textBox3.Text.Replace(" ", "");

            ArrayList L = vp.GetProperties(proj, platform);

            foreach (Microsoft.Build.Evaluation.ProjectProperty p in L)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p.Name;
                row.Cells[1].Value = p.EvaluatedValue;
            }
        }

        public void LoadAllProjectItems()
        {
            Initialize();

            GetVSProject(sn);

            if (vp == null)
                return;

            //vp.SetProperty("Configuration", textBox2.Text);
            //vp.SetProperty("Platform", textBox3.Text);

            string proj = "";// textBox2.Text.Replace(" ", "");
            string platform = "";// textBox3.Text.Replace(" ", "");

            ArrayList L = vp.GetAllItems();

            foreach (Microsoft.Build.Evaluation.ProjectItem p in L)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p.ItemType;
                row.Cells[1].Value = p.EvaluatedInclude;
            }
        }

        public void LoadProjectTargets()
        {
            Initialize();

            GetVSProject(sn);

            if (vp == null)
                return;

            IDictionary<string, Microsoft.Build.Execution.ProjectTargetInstance> s = vp.GetTargets();

            foreach (string p in s.Keys)
            {
                Microsoft.Build.Execution.ProjectTargetInstance c = s[p];
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p;
                row.Cells[1].Value = c.Name;
            }
        }

        public void LoadProjectImports()
        {
            Initialize();

            GetVSProject(sn);

            if (vp == null)
                return;

            //vp.SetProperty("Configuration", textBox2.Text);
            //vp.SetProperty("Platform", textBox3.Text);

            string proj = "";// textBox2.Text.Replace(" ", "");
            string platform = "";// textBox3.Text.Replace(" ", "");

            ArrayList L = vp.GetImports();

            foreach (Microsoft.Build.Evaluation.ResolvedImport p in L)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p.ImportingElement.Project;
                row.Cells[1].Value = p.ImportedProject.InitialTargets;
            }
        }

        public void LoadProjectItems()
        {
            Initialize();

            GetVSProject(sn);

            if (vp == null)
                return;

            //vp.SetProperty("Configuration", textBox2.Text);
            //vp.SetProperty("Platform", textBox3.Text);

            string proj = "";// textBox2.Text.Replace(" ", "");
            string platform = "";// textBox3.Text.Replace(" ", "");

            ArrayList L = vp.GetItems("Compile");

            foreach (Microsoft.Build.Evaluation.ProjectItem p in L)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p.ItemType;
                row.Cells[1].Value = p.UnevaluatedInclude;
            }
        }

        public void LoadConditionedProperties()
        {
            Initialize();

            GetVSProject(sn);

            IDictionary<string, List<string>> d = vp.GetConditionedProperties();
            foreach (string s in d.Keys)
            {
                List<string> b = d[s];

                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = s;
                row.Cells[1].Value = "";

                foreach (string c in b)
                {
                    rowId = dg.Rows.Add();
                    row = dg.Rows[rowId];
                    row.Cells[0].Value = "-";
                    row.Cells[1].Value = c;
                }
            }
        }

        public void LoadGlobalProperties()
        {
            Initialize();

            GetVSProject(sn);

            IDictionary<string, string> d = vp.GetGlobalProperties();
            foreach (string p in d.Keys)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p;
                row.Cells[1].Value = d[p];
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SplitContainer sp = splitContainer1;

            if (fs.Visible == true)
            {
                sp.Panel1.Controls.Remove(fs);
                sp.Panel1.Controls.Add(ng);
                ng.Dock = DockStyle.Fill;
                fs.Visible = false;
                ng.Visible = true;
            }
            else
            {
                sp.Panel1.Controls.Remove(ng);
                sp.Panel1.Controls.Add(fs);
                fs.Dock = DockStyle.Fill;
                fs.Visible = true;
                ng.Visible = false;
            }
        }

        public void Pages()
        {
            SplitContainer sp = splitContainer1;
            sp.Panel1.Controls.Remove(ng);
            sp.Panel1.Controls.Add(fs);
            fs.Dock = DockStyle.Fill;
            fs.Visible = true;
            ng.Visible = false;

            ng.BackColor = SystemColors.Control;
            ng.Nodes.Add("All project items");
            ng.Nodes.Add("Compile project items");
            ng.Nodes.Add("project");
            ng.Nodes.Add("All project properties");
            ng.Nodes.Add("Conditioned project properties");
            ng.Nodes.Add("Global project properties");
            ng.Nodes.Add("Project imports");
            ng.Nodes.Add("Project targets");

            ns.HideSelection = false;
            np.HideSelection = false;

            fs.open.Click += Open_Click;

            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dg.SelectionChanged += Dg_SelectionChanged;

            LoadPanel(panel1);
        }

        private void Dg_SelectionChanged(object sender, EventArgs e)
        {
            if (dg.SelectedRows == null)
                return;
            if (dg.SelectedRows.Count <= 0)
                return;
            DataGridViewRow r = dg.SelectedRows[0];

            string type = r.Cells[0].Value as string;
            string include = r.Cells[1].Value as string;
            if (include == null)
                return;

            msbuilder_alls b = new msbuilder_alls();

            TreeNode d = ns.SelectedNode;

            if (pg != null)
                pg.SelectedObject = d.Tag;

            TreeNode ng = b.FindNodesIncludes(d, include);

            if (ng == null)
                return;
            MessageBox.Show("Node found - " + ng.Text);
        }

        public void CheckProjectItems()
        {
            Initialize();

            vp = GetVSProject(ns);

            if (vp == null)
                return;

            Dictionary<string, ArrayList> dict = vp.GetSubtypes(pc);

            foreach (string d in dict.Keys)
            {
                ArrayList L = dict[d];

                foreach (Microsoft.Build.Evaluation.ProjectItem p in L)
                {
                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];
                    row.Cells[0].Value = p.ItemType;
                    row.Cells[1].Value = p.EvaluatedInclude;
                }
            }
        }

        public void LoadPanel(Panel p)
        {
            SplitContainer sp = splitContainer1;

            sp.Panel2.Controls.Clear();

            tabControl1.Visible = false;

            tabPage1.Controls.Clear();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(p);

            p.Dock = DockStyle.Fill;

            p.Visible = true;

            dg.Visible = true;
        }

        public VSProject GetVSProject(TreeView b)
        {
            TreeNode node = b.SelectedNode;
            if (node == null)
                return null;
            VSParsers.ProjectItemInfo s = node.Tag as VSParsers.ProjectItemInfo;
            if (s == null)
                return null;

            vp = s.ps;

            return s.ps;
        }

        public VSSolution GetVSSolution(TreeNode b)
        {
            foreach (TreeNode node in b.Nodes)
            {
                VSParsers.ProjectItemInfo s = node.Tag as VSParsers.ProjectItemInfo;
                if (s == null)
                    continue;

                return s.ps.vs;
            }

            return null;
        }

        public VSSolution GetVSSolution(TreeView b)
        {
            foreach (TreeNode node in b.Nodes)
            {
                VSParsers.ProjectItemInfo s = node.Tag as VSParsers.ProjectItemInfo;
                if (s != null)

                    return s.ps.vs;

                VSSolution v = GetVSSolution(node);

                if (v != null)
                    return v;
            }

            return null;
        }

        private void Open_Click(object sender, EventArgs e)
        {
            string file = folderLister1.tb.Text;
            TreeView b = VSExplorer.LoadProject(file);
            VSExplorer.Loads2(b, sn);

            vs = b.Tag as VSSolution;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MergeProjectItems();
        }

        public void MergeProjectItems()
        {
            VSProject p0 = GetVSProject(ns);

            //VSParsers.ProjectItemInfo p0 = ns.Tag as VSParsers.ProjectItemInfo;

            string projectfile = p0.FileName;

            VSProject p1 = GetVSProject(np);

            //VSParsers.ProjectItemInfo pm = np.Tag as VSParsers.ProjectItemInfo;

            string projectmerge = p1.FileName;

            MessageBox.Show(projectfile);

            MessageBox.Show(projectmerge);

            p0.MergeProject(p1, "");
        }

        private void treeView2_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = ng.SelectedNode;
            if (node == null)
                return;
            string text = node.Text;
            if (text == "All project items")
                LoadAllProjectItems();
            else if (text == "Compile project items")
                LoadProjectItems();
            else if (text == "All project properties")
                LoadAllProjectProperties();
            else if (text == "Conditioned project properties")
                LoadConditionedProperties();
            else if (text == "Global project properties")
                LoadGlobalProperties();
            else if (text == "Project imports")
                LoadProjectImports();
            else if (text == "Project targets")
                LoadProjectTargets();
        }

        private void splitContainer3_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private TreeView sn { get; set; }

        private void treeView1_Click(object sender, EventArgs e)
        {
            ns.BackColor = SystemColors.Control;
            np.BackColor = Color.White;
            sn = ns;
        }

        private void treeView3_Click(object sender, EventArgs e)
        {
            np.BackColor = SystemColors.Control;
            ns.BackColor = Color.White;
            sn = np;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ns.BackColor = SystemColors.Control;
            np.BackColor = Color.White;
            sn = ns;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            np.BackColor = SystemColors.Control;
            ns.BackColor = Color.White;
            sn = np;
        }

        private Microsoft.Build.Evaluation.Project pc { get; set; }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Initialize();

            VSSolution vs = GetVSSolution(ns);

            if (vs == null)
                return;

            if (pc != null)
                pc.ProjectCollection.UnloadAllProjects();

            foreach (VSProject p in vs.projects)
            {
                if (File.Exists(p.FileName) == false)
                    continue;

                pc = p.LoadProjectToMemory();

                p.Shuffle(pc);
            }
            MockProject();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            RichTextBox r = richTextBox1;

            LoadPanel(panel2);

            VSSolution vs = GetVSSolution(ns);

            ArrayList S = VSSolution.GetVSProjects(vs.solutionFileName);

            foreach (vsproject v in S)
            {
                string s = v.content;

                string[] cc = s.Split("\n".ToCharArray());

                r.AppendText(cc[0] + "\n");

                string n = v.name.Replace("\"", "");

                n = n.Trim();

                v.name = v.name.Replace(n, '_' + n);

                v.file = v.file.Replace(n + ".", '_' + n + ".");

                string g = v.GetProject();

                r.AppendText(g + "\n");

                g = v.ModifyProject(g);

                r.AppendText(g + "\n");
            }
        }

        public void MockProject()
        {
            VSSolution vs = GetVSSolution(ns);

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

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            TreeNode d = ns.SelectedNode;

            msbuilder_alls b = new msbuilder_alls();

            int N = dg.Rows.Count;

            int i = 0;
            foreach (DataGridViewRow r in dg.Rows)
            {
                string type = r.Cells[0].Value as string;
                string include = r.Cells[1].Value as string;
                if (include == null)
                    continue;

                TreeNode ng = b.FindNodesIncludes(d, include);

                if (ng == null)
                    MessageBox.Show("Node has not been found - " + d.Text);
                else i++;
            }

            MessageBox.Show("Total nodes " + i.ToString() + " from existing " + N.ToString() + " have been found");
        }

        private PropertyGrid pg { get; set; }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            SplitContainer sp = splitContainer3;

            pg = new PropertyGrid();

            pg.Show();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(pg);

            pg.Dock = DockStyle.Fill;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Initialize();

            foreach (TreeNode node in ns.Nodes)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];

                row.HeaderCell.Value = rowId.ToString();

                row.Cells[0].Value = ns.Text;

                VSParsers.ProjectItemInfo p = node.Tag as VSParsers.ProjectItemInfo;

                if (p != null)
                {
                    if (p.psi != null)
                    {
                        string s = p.psi.Include;

                        row.Cells[1].Value = s;
                    }
                }

                GeTNodes(node);
            }
        }

        public void GeTNodes(TreeNode node)
        {
            foreach (TreeNode ns in node.Nodes)
            {
                int rowId = dg.Rows.Add();

                DataGridViewRow row = dg.Rows[rowId];

                row.HeaderCell.Value = rowId.ToString();

                row.Cells[0].Value = ns.Text;

                VSParsers.ProjectItemInfo p = ns.Tag as VSParsers.ProjectItemInfo;

                if (p != null)
                {
                    if (p.psi != null)
                    {
                        string s = p.psi.Include;

                        row.Cells[1].Value = s;
                    }
                }

                GeTNodes(ns);
            }
        }

        private TreeView bb { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            ns.BeginUpdate();

            ts = new TreeView();

            bb = new TreeView();

            CopyTreeNodes(ns, ts);

            CopyTreeNodes(ns, bb);

            //ns = ts;

            ArrayList N = Find(ts);

            ViewFind(N);
        }

        public ArrayList Find(TreeView ns)
        {
            string text = textBox1.Text.ToLower();

            search = text;

            ArrayList N = new ArrayList();

            foreach (TreeNode node in ns.Nodes)
            {
                if (node.Text.ToLower().Contains(text))
                    N.Add(node);

                FindNodes(node, N, text);
            }
            return N;
        }

        private TreeView ts { get; set; }

        public void FindNodes(TreeNode ns, ArrayList N, string text)
        {
            foreach (TreeNode node in ns.Nodes)
            {
                if (node.Text.ToLower().Contains(text))
                    N.Add(node);

                FindNodes(node, N, text);
            }
        }

        private TreeNode master { get; set; }

        public void ViewFind(ArrayList N)
        {
            //ns.BeginUpdate();

            ArrayList PP = new ArrayList();

            foreach (TreeNode node in N)
            {
                ArrayList P = GetNodePath(node);
                PP.Add(P);
            }

            ArrayList R = GetAllNodes(ts);

            ClearAllNodes(R);

            ns.Nodes.Clear();

            //ns = new TreeView();

            foreach (ArrayList P in PP)
            {
                P.Reverse();

                TreeNode nodes = P[0] as TreeNode;

                if (ns.Nodes.Contains(nodes) == false)
                {
                    ns.Nodes.Add(nodes);
                }
                P.Remove(nodes);

                foreach (TreeNode ng in P)
                {
                    if (nodes.Nodes.Contains(ng) == false)
                        nodes.Nodes.Add(ng);
                    nodes = ng;
                }
            }

            // MessageBox.Show("ns -" + ns.Nodes.Count);

            ns.EndUpdate();
        }

        private ArrayList GetNodePath(TreeNode node)
        {
            ArrayList P = new ArrayList();

            P.Add(node);
            while (node.Parent != null)
            {
                P.Add(node.Parent);
                node = node.Parent;
            }

            return P;
        }

        private ArrayList GetAllNodes(TreeView v)
        {
            ArrayList D = new ArrayList();
            foreach (TreeNode nodes in v.Nodes)
            {
                D.Add(nodes);
                GetAllNodes(nodes, D);
            }

            return D;
        }

        private void GetAllNodes(TreeNode node, ArrayList D)
        {
            foreach (TreeNode nodes in node.Nodes)
            {
                D.Add(nodes);
                GetAllNodes(nodes, D);
            }
        }

        private void ClearAllNodes(ArrayList D)
        {
            foreach (TreeNode node in D)
            {
                ArrayList L = new ArrayList();
                foreach (TreeNode ng in node.Nodes)
                    L.Add(ng);
                foreach (TreeNode nodes in L)
                    node.Nodes.Remove(nodes);
            }
        }

        private void ClearAllNodes(TreeView v)
        {
            foreach (TreeNode node in v.Nodes)
            {
                ArrayList L = new ArrayList();
                foreach (TreeNode ng in node.Nodes)
                    L.Add(ng);
                foreach (TreeNode nodes in L)
                    node.Nodes.Remove(nodes);
                ClearAllNodes(node);
            }
        }

        private void ClearAllNodes(TreeNode b)
        {
            foreach (TreeNode node in b.Nodes)
            {
                ArrayList L = new ArrayList();
                foreach (TreeNode ng in node.Nodes)
                    L.Add(ng);
                foreach (TreeNode nodes in L)
                    node.Nodes.Remove(nodes);
                ClearAllNodes(node);
            }
        }

        public void CopyTreeNodes(TreeView treeview1, TreeView treeview2)
        {
            treeView2.ImageList = CreateView_Solution.CreateImageList();

            //treeView2.ImageList = new ImageList();
            //foreach(Image b in treeView1.ImageList.Images)
            //    treeView2.ImageList.Images.Add(b);
            TreeNode newTn;
            foreach (TreeNode tn in treeview1.Nodes)
            {
                newTn = new TreeNode(tn.Text);
                newTn.ImageKey = tn.ImageKey;
                newTn.SelectedImageKey = tn.SelectedImageKey;
                newTn.Tag = tn.Tag;
                treeview2.Nodes.Add(newTn);
                CopyChildren(newTn, tn);
            }
        }

        public void CopyChildren(TreeNode parent, TreeNode original)
        {
            TreeNode newTn;
            foreach (TreeNode tn in original.Nodes)
            {
                newTn = new TreeNode(tn.Text);
                newTn.ImageKey = tn.ImageKey;
                newTn.SelectedImageKey = tn.SelectedImageKey;
                newTn.Tag = tn.Tag;
                parent.Nodes.Add(newTn);
                CopyChildren(newTn, tn);
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            foreach (TreeNode node in bb.Nodes)
                treeView1.Nodes.Add(node);
            treeView1.EndUpdate();
        }
    }
}