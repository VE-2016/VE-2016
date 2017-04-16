using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using VSProvider;

using WinExplorer.UI;

namespace WinExplorer
{
    public partial class ConfigurationManagerForm : Form
    {
        public ConfigurationManagerForm()
        {
            InitializeComponent();

            SuspendLayout();

            pc = comboBox1;

            sc = comboBox2;

            dg = dataGridView1;

            dg.RowHeadersVisible = false;

            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dg.GridColor = Color.LightBlue;

            cmb = new DataGridViewComboBoxColumn();
            cmb.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            cmb.HeaderText = "Configuration";
            cmb.Name = "cmbp";
            cmb.Items.Add("<New...>");
            cmb.Items.Add("<Edit...>");
            dg.Columns.RemoveAt(1);
            dg.Columns.Insert(1, cmb);

            cmp = new DataGridViewComboBoxColumn();
            cmp.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            cmp.HeaderText = "Platform";
            cmp.Name = "cmbpl";
            cmp.Items.Add("<New...>");
            cmp.Items.Add("<Edit...>");
            dg.Columns.RemoveAt(2);
            dg.Columns.Insert(2, cmp);

            dg.SelectionChanged += Dg_SelectionChanged;
            dg.CurrentCellDirtyStateChanged += Dg_CurrentCellDirtyStateChanged;
            dg.CellValueChanged += Dg_CellValueChanged;

            sp = splitContainer1;

            pl = panel1;
            startup = panel2;
            deps = panel3;
            code = panel4;
            tc = tabControl1;
            tr = treeView1;

            LoadTreeView();

            tr.AfterSelect += Tr_AfterSelect;

            LoadPage_Configs();

            //LoadPage_MSBuild();

            ResumeLayout();
        }

        private void Tr_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            string text = node.Text;
            if (text == "Startup Projects")
                LoadPage_Startup();
            else if (text == "Project Dependencies")
                LoadPage_Dependencies();
            else if (text == "Code Analysis Settings")
                LoadPage_CodeAnalysis();
            else if (text == "Configurations")
                LoadPage_Configs();
            else if (text == "MSBuild Projects")
                LoadPage_MSBuildProjects();
        }

        public void LoadTreeView()
        {
            TreeNode node = new TreeNode();
            node.Text = "Common Properties";
            tr.Nodes.Add(node);

            TreeNode nodes = new TreeNode();
            nodes.Text = "Startup Projects";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Project Dependencies";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Code Analysis Settings";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Debug Source Files";
            node.Nodes.Add(nodes);

            node = new TreeNode();
            node.Text = "Configuration Properties";
            tr.Nodes.Add(node);

            nodes = new TreeNode();
            nodes.Text = "Configurations";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "MSBuild Projects";
            node.Nodes.Add(nodes);

            tr.ExpandAll();
        }

        private MSBuildProject mbp { get; set; }

        public void LoadPage_MSBuild()
        {
            SuspendLayout();

            //sp.Panel2.Controls.Clear();

            mbp = new MSBuildProject();
            mbp.TopLevel = false;
            mbp.Dock = DockStyle.Fill;
            mbp.FormBorderStyle = FormBorderStyle.None;

            mbp.vs = vs;

            mbp.LoadData();

            panel9.Controls.Add(mbp);

            ResumeLayout();
        }

        public void LoadPage_MSBuildProjects()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(panel9);

            if(mbp != null)

            mbp.Show();

            ResumeLayout();
        }

        public void SetConfigView()
        {
            SuspendLayout();
            this.Controls.Remove(panel6);
            sp.Panel2.Controls.Remove(tc);
            sp.Panel1Collapsed = true;
            sp.Panel2.Controls.Add(pl);
            sp.Location = new Point(0, 0);
            ResumeLayout();
        }

        public void LoadPage_Configs()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(pl);
            ResumeLayout();
        }

        public void LoadPage_Startup()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(startup);
            ResumeLayout();
        }

        public void LoadPage_Dependencies()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(deps);
            ResumeLayout();
        }

        public void LoadPage_CodeAnalysis()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(code);
            ResumeLayout();
        }

        private ToolStrip ts { get; set; }

        private SplitContainer sp { get; set; }

        private TabControl tc { get; set; }

        private Panel pl { get; set; }

        private Panel startup { get; set; }

        private Panel code { get; set; }

        private Panel deps { get; set; }

        private TreeView tr { get; set; }

        private DataGridViewComboBoxColumn cmb { get; set; }
        private DataGridViewComboBoxColumn cmp { get; set; }

        private void Dg_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dg.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                dg.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void Dg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)dg.Rows[e.RowIndex].Cells[1];
                if (cb.Value != null)
                {
                    if (cb.Value.ToString() == "<Edit...>")
                    {
                        ArrayList L = ConfigurationManagerForm.GetSolutionPlatform(vs);
                        EditProjectForm epf = new EditProjectForm();
                        epf.Text = "Edit Project Configurations";
                        epf.LoadPlatforms(L);
                        DialogResult r = epf.ShowDialog();
                        cb.Value = cmb.Items[2];
                    }
                    else if (cb.Value.ToString() == "<New...>")
                    {
                        ArrayList L = ConfigurationManagerForm.GetSolutionPlatform(vs);
                        NewProjectConForm epf = new NewProjectConForm();
                        epf.Text = "New Project Configurations";
                        epf.LoadProject(L);
                        DialogResult r = epf.ShowDialog();
                        cb.Value = cmb.Items[2];
                    }
                }
            }
            else if (e.ColumnIndex == 2)
            {
                DataGridViewComboBoxCell cd = (DataGridViewComboBoxCell)dg.Rows[e.RowIndex].Cells[2];
                if (cd.Value != null)
                {
                    if (cd.Value.ToString() == "<Edit...>")
                    {
                        ArrayList L = ConfigurationManagerForm.GetProjectPlatform(vs);
                        EditProjectForm epf = new EditProjectForm();
                        epf.Text = "Edit Platform Configurations";
                        epf.LoadPlatforms(L);
                        DialogResult r = epf.ShowDialog();
                        cd.Value = cmp.Items[2];
                    }
                    else if (cd.Value.ToString() == "<New...>")
                    {
                        int c = e.RowIndex;
                        string p = dg.Rows[c].Tag as string;
                        VSProject vp = vs.GetProjectbyName(p);
                        ArrayList L = ConfigurationManagerForm.GetProjectPlatform(vs);
                        NewPlatformConForm epf = new NewPlatformConForm(this);
                        epf.vp = vp;
                        epf.Text = "New Platform Configurations";
                        epf.LoadPlatforms(L);
                        DialogResult r = epf.ShowDialog();
                        cd.Value = cmp.Items[2];
                    }
                }
            }
        }

        private void Dg_SelectionChanged(object sender, EventArgs e)
        {
        }

        public void LoadConfigItems(ArrayList L)
        {
            DataGridViewComboBoxColumn cmb = dg.Columns[2] as DataGridViewComboBoxColumn;

            foreach (string s in L)
            {
                if (cmb.Items.Contains(s) == false)
                    cmb.Items.Add(s);
            }

            ComboBox cb = comboBox8;
            cb.Items.Clear();
            foreach (string s in L)
            {
                cb.Items.Add(s);
            }
        }

        public void LoadProjectItems(ArrayList L)
        {
            DataGridViewComboBoxColumn cmb = dg.Columns[1] as DataGridViewComboBoxColumn;

            foreach (string s in L)
            {
                if (cmb.Items.Contains(s) == false)
                    cmb.Items.Add(s);
            }
            ComboBox cb = comboBox7;
            cb.Items.Clear();
            foreach (string s in L)
            {
                cb.Items.Add(s);
            }
        }

        public DataGridView dg { get; set; }

        public ComboBox pc { get; set; }

        public ComboBox sc { get; set; }

        public VSSolution vs { get; set; }

        public static ArrayList GetSolutionPlatform(VSSolution vs)
        {
            ArrayList L = new ArrayList();

            if (vs == null)
                return L;



            L = vs.GetSolutionPlatforms();

            ArrayList P = new ArrayList();

            foreach (string s in L)
            {
                string[] cc = s.Split("|".ToCharArray());

                if (cc.Length < 1)
                    continue;

                if (P.IndexOf(cc[0]) < 0)
                    P.Add(cc[0]);
            }

            return P;
        }

        public static ArrayList GetProjectPlatform(VSSolution vs)
        {
            ArrayList L = new ArrayList();

            if (vs == null)
                return L;

            L = vs.GetSolutionPlatforms();

            ArrayList P = new ArrayList();

            foreach (string s in L)
            {
                string[] cc = s.Split("|".ToCharArray());

                if (cc.Length < 1)
                    continue;
                if (cc.Length == 2)
                    if (P.IndexOf(cc[1]) < 0)
                        P.Add(cc[1]);
            }

            P.Add("Configuration Manager");

            return P;
        }

        public void LoadProjects(ComboBox cb)
        {
            cb.Items.Clear();
            foreach (VSProject p in vs.projects)
                cb.Items.Add(p.Name);
        }

        public void LoadProjects(CheckedListBox lb)
        {
            lb.Items.Clear();
            foreach (VSProject p in vs.projects)
                lb.Items.Add(p.Name);
        }

        public void UnloadConfigPanel()
        {
            tabPage1.Controls.Remove(panel7);
            panel8.Dock = DockStyle.Fill;
        }

        public void LoadConfigs(VSSolution _vs)
        {
            vs = _vs;

            LoadProjects(comboBox4);
            LoadProjects(checkedListBox1);

            ArrayList L = vs.GetSolutionPlatforms();

            pc.Items.Clear();
            sc.Items.Clear();

            foreach (string s in L)
            {
                string[] cc = s.Split("|".ToCharArray());

                if (cc.Length < 1)
                    continue;

                if (pc.Items.Contains(cc[0]) == false)
                    pc.Items.Add(cc[0]);

                if (cc.Length == 2)

                    if (sc.Items.Contains(cc[1]) == false)
                        sc.Items.Add(cc[1]);
            }

            pc.Items.Add("New");
            pc.Items.Add("Edit");

            L = vs.GetProjectPlatforms();

            ArrayList C = ConfigurationManagerForm.GetProjectPlatform(vs);

            LoadConfigItems(C);

            ArrayList P = ConfigurationManagerForm.GetSolutionPlatform(vs);

            LoadProjectItems(P);

            Dictionary<string, string> dict = vs.GetProjectGuids();

            foreach (string s in L)
            {
                string[] cc = s.Split(".".ToCharArray());

                string project = cc[0];

                project = project.Replace("{", "");
                project = project.Replace("}", "");

                project = project.Trim();

                if (dict.ContainsKey(project))
                    dict.TryGetValue(project, out project);

                string[] dd = cc[1].Split("|".ToCharArray());

                if (C.Contains(dd[1]) == false)
                    continue;
                if (P.Contains(dd[0]) == false)
                    continue;

                DataGridViewRow row = (DataGridViewRow)dg.RowTemplate.Clone();
                row.CreateCells(dg, project, dd[0], dd[1], true, true);

                row.Tag = project;

                dg.Rows.Add(row);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;
            if (i < 0)
                return;
            string text = comboBox1.Items[i].ToString();

            if (text == "Edit")
                return;

            FilterByProject(text);
        }

        public void FilterByProject(string text)
        {
            dg.Rows.Clear();

            Dictionary<string, string> dict = vs.GetProjectGuids();

            ArrayList L = vs.GetProjectPlatforms();

            foreach (string s in L)
            {
                string[] cc = s.Split(".".ToCharArray());

                string project = cc[0];

                project = project.Replace("{", "");
                project = project.Replace("}", "");

                project = project.Trim();

                if (dict.ContainsKey(project))
                    dict.TryGetValue(project, out project);

                string[] dd = cc[1].Split("|".ToCharArray());

                if (dd[0] == text)
                {
                    DataGridViewRow row = (DataGridViewRow)dg.RowTemplate.Clone();
                    row.CreateCells(dg, project, dd[0], dd[1], true, true);

                    dg.Rows.Add(row);
                }
            }
        }

        public void FilterByPlatform(string text)
        {
            dg.Rows.Clear();

            Dictionary<string, string> dict = vs.GetProjectGuids();

            ArrayList L = vs.GetProjectPlatforms();

            foreach (string s in L)
            {
                string[] cc = s.Split(".".ToCharArray());

                string project = cc[0];

                project = project.Replace("{", "");
                project = project.Replace("}", "");

                project = project.Trim();

                if (dict.ContainsKey(project))
                    dict.TryGetValue(project, out project);

                string[] dd = cc[1].Split("|".ToCharArray());

                if (dd[1] == text)
                {
                    DataGridViewRow row = (DataGridViewRow)dg.RowTemplate.Clone();
                    row.CreateCells(dg, project, dd[0], dd[1], true, true);

                    dg.Rows.Add(row);
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox2.SelectedIndex;
            if (i < 0)
                return;
            string text = comboBox2.Items[i].ToString();

            if (text == "Edit")
                return;

            FilterByPlatform(text);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox7.SelectedIndex;
            if (i < 0)
                return;
            string project = comboBox7.Items[i].ToString();
            if (mbp == null)
                return;
            mbp.SetConfig(project);
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox8.SelectedIndex;
            if (i < 0)
                return;
            string project = comboBox8.Items[i].ToString();
            if (mbp == null)
                return;
            mbp.SetPlatform(project);
        }
    }
}