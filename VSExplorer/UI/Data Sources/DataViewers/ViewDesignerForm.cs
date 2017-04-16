using AIMS.Libraries.Scripting.ScriptControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class ViewDesignerForm : Form
    {
        public ViewDesignerForm()
        {
            InitializeComponent();
            doc = ExplorerForms.ef.Command_OpenGenericDocument("name");
            doc.TopLevel = false;
            doc.Dock = DockStyle.Fill;
            doc.Show();
            this.Controls.Add(doc);
        }
        public Document doc { get; set; }
    }
}
