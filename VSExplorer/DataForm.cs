using System;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class DataForm : Form
    {
        public DataForm()
        {
            InitializeComponent();
        }

        public string GetFolderName()
        {
            return textBox1.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}