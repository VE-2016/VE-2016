using System;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class ProjectTemplateMain : UserControl
    {
        public ProjectTemplateMain()
        {
            InitializeComponent();
            cb = comboBox1;
        }

        public ComboBox cb { get; set; }

        public int GetTemplate()
        {
            if (radioButton1.Checked == true)
                return 0;
            else if (radioButton2.Checked == true)
                return 1;

            return -1;
        }

        public VSSolution vs { get; set; }

        public VSProject pp { get; set; }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cb.SelectedIndex;

            if (vs == null)
                return;

            string name = cb.Items[i].ToString();

            pp = vs.GetProjectbyName(name);

            if (pp == null)
                return;
        }
    }
}