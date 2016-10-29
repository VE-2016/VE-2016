using System;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class AssemblyForm : Form
    {
        public AssemblyForm()
        {
            InitializeComponent();
        }

        public utils.assembly assinfo { get; set; }

        private void AssemblyForm_Load(object sender, EventArgs e)
        {
        }

        public void LoadAssemblyInfo()
        {
            if (assinfo == null)
            {
                assinfo = new utils.assembly();
                return;
            }
            textBox1.Text = assinfo.Title;
            textBox2.Text = assinfo.Description;
            textBox3.Text = assinfo.Product;
            textBox4.Text = assinfo.Company;
            textBox5.Text = assinfo.Trademark;
            textBox6.Text = assinfo.Copywrite;

            string s = assinfo.Version;

            if (s == "")
                s = "1.0.0.0";

            System.Version v = new Version(s);

            textBox7.Text = v.Major.ToString();
            textBox8.Text = v.Minor.ToString();
            textBox9.Text = v.Build.ToString();
            textBox10.Text = v.Revision.ToString();

            s = assinfo.FileVersion;

            if (s == "")
                s = "1.0.0.0";

            v = new Version(s);

            textBox14.Text = v.Major.ToString();
            textBox13.Text = v.Minor.ToString();
            textBox12.Text = v.Build.ToString();
            textBox11.Text = v.Revision.ToString();

            textBox16.Text = assinfo.GUID;
        }
    }
}