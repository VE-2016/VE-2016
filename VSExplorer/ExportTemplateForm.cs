using System;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class ExportTemplateForm : Form
    {
        public ExportTemplateForm()
        {
            InitializeComponent();
            pictureBox1.Image = resource_vsc.template;

            panel = panel1;

            ptt = new ProjectTemplate();

            mtt = new ProjectTemplateMain();

            itt = new ProjectItemTemplate();

            LoadMainTemplate();

            cb = mtt.cb;
            cb.DropDownStyle = ComboBoxStyle.DropDownList;

            button1.Enabled = false;
        }

        private Panel panel { get; set; }

        public void LoadMainTemplate()
        {
            panel.Controls.Clear();

            panel.Controls.Add(mtt);

            mtt.Dock = DockStyle.Fill;
        }

        public void LoadProjectTemplate()
        {
            panel.Controls.Clear();

            panel.Controls.Add(ptt);

            ptt.Dock = DockStyle.Fill;

            if (mtt.pp == null)
                return;

            ptt.SetProjectTemplate(mtt.pp);
        }

        public void LoadItemTemplate()
        {
            panel.Controls.Clear();

            panel.Controls.Add(itt);

            itt.Dock = DockStyle.Fill;

            if (mtt.pp == null)
                return;

            itt.LoadProjectItems(mtt.pp);
        }

        public ComboBox cb { get; set; }

        public void LoadSolution(VSSolution _vs)
        {
            vs = _vs;

            if (vs == null)
                return;

            foreach (VSProject p in vs.Projects)
            {
                string name = p.Name;

                cb.Items.Add(name);
            }

            mtt.vs = vs;
        }

        public ProjectTemplate ptt { get; set; }

        public ProjectTemplateMain mtt { get; set; }

        public ProjectItemTemplate itt { get; set; }

        public VSSolution vs { get; set; }

        public VSProject pp { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            int i = mtt.GetTemplate();

            if (i == 0)
                LoadProjectTemplate();
            else if (i == 1)
                LoadItemTemplate();

            button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadMainTemplate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i = mtt.GetTemplate();

            if (i == 0)
                ptt.MakeProjectTemplate();
            else if (i == 1)
                itt.MakeItemTemplate();
        }
    }
}