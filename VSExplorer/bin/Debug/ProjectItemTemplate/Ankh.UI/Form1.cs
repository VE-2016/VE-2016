using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ankh.UI;

namespace Ankh.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Ankh.UI.RepositoryExplorer.RepositoryExplorerControl Rc { get; set; }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Rc = new Ankh.UI.RepositoryExplorer.RepositoryExplorerControl();
            splitContainer1.Panel2.Controls.Add(Rc);
            Rc.Dock = DockStyle.Fill;
            Rc.Show();
        }
    }
}
