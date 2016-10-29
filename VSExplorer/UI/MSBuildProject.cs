using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer.UI
{
    public partial class MSBuildProject : Form
    {
        public MSBuildProject()
        {
            InitializeComponent();
            dg = dataGridView1;
            lb = listBox1;
        }

        public VSSolution vs { get; set; }

        public VSProject vp { get; set; }

        public DataGridView dg { get; set; }

        public ListBox lb { get; set; }

        public string config { get; set; }

        public string platform { get; set; }

        public void SetConfig(string p)
        {
            config = p;
            textBox2.Text = config;
        }

        public void SetPlatform(string p)
        {
            platform = p;
            textBox3.Text = platform;
        }

        public void Initialize()
        {
            dg.ColumnCount = 2;
            dg.Columns[0].Name = "Property";
            dg.Columns[1].Name = "Value";
            dg.Rows.Clear();
        }

        public void LoadData()
        {
            if (vs == null)
                return;
            lb.Items.Clear();
            foreach (VSProject p in vs.Projects)
                lb.Items.Add(p.Name);
        }

        public void LoadProject()
        {
            Initialize();

            if (vp == null)
                return;

            //vp.SetProperty("Configuration", textBox2.Text);
            //vp.SetProperty("Platform", textBox3.Text);

            string proj = textBox2.Text.Replace(" ", "");
            string platform = textBox3.Text.Replace(" ", "");

            ArrayList L = vp.GetProperties(proj, platform);

            foreach (Microsoft.Build.Evaluation.ProjectProperty p in L)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p.Name;
                row.Cells[1].Value = p.EvaluatedValue;
            }
        }

        public void LoadConditionedProperties()
        {
            Initialize();
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
            IDictionary<string, string> d = vp.GetGlobalProperties();
            foreach (string p in d.Keys)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];
                row.Cells[0].Value = p;
                row.Cells[1].Value = d[p];
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadConditionedProperties();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadProject();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            LoadGlobalProperties();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lb.SelectedIndex;
            if (i < 0)
                return;
            string name = lb.Items[i].ToString();
            VSProject pp = vs.GetProjectbyName(name);
            if (pp == null)
                return;
            vp = pp;
            textBox1.Text = vp.Name;
            textBox1.Enabled = false;
        }
    }
}