using System.Collections;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class EditProjectForm : Form
    {
        public EditProjectForm()
        {
            InitializeComponent();
            lb = listBox1;
        }

        private ListBox lb { get; set; }

        public void LoadPlatforms(ArrayList L)
        {
            lb.Items.Clear();

            foreach (string s in L)
                lb.Items.Add(s);
        }
    }
}