using System;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class CustomizeForm : Form
    {
        public CustomizeForm()
        {
            InitializeComponent();
        }

        private AddCommandForm adf { get; set; }

        private void button5_Click(object sender, EventArgs e)
        {
            adf = new AddCommandForm();
            DialogResult r = adf.ShowDialog();
            if (r != DialogResult.OK)
                return;
        }
    }
}