using System.Collections;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer.UI
{
    public partial class NewPlatformConForm : Form
    {
        public NewPlatformConForm(ConfigurationManagerForm c)
        {
            InitializeComponent();
            cb = comboBox1;
            mf = c;
        }

        private ComboBox cb { get; set; }

        public ConfigurationManagerForm mf { get; set; }

        public void LoadPlatforms(ArrayList L)
        {
            cb.Items.Clear();
            cb.Items.Add("<Empty>");
            foreach (string s in L)
                cb.Items.Add(s);
        }

        public VSSolution vs;

        public VSProject vp;

        private void button1_Click(object sender, System.EventArgs e)
        {
            string platform = textBox1.Text;

            string data = comboBox1.Text;

            MessageBox.Show("New platform - " + platform + " with " + data + " will be created for project " + vp.FileName);

            vs = mf.vs;

            ArrayList cc = ConfigurationManagerForm.GetSolutionPlatform(vs);

            foreach (string config in cc)
                vp.CreatePlatform(platform, config);
        }
    }
}