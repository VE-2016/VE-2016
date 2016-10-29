using System.Collections;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class NewProjectConForm : Form
    {
        public NewProjectConForm()
        {
            InitializeComponent();
            cb = comboBox1;
        }

        private ComboBox cb { get; set; }

        public void LoadProject(ArrayList L)
        {
            cb.Items.Clear();
            cb.Items.Add("<Empty>");
            foreach (string s in L)
                cb.Items.Add(s);
        }
    }
}