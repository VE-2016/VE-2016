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
    public partial class ViewCodeForm : Form
    {
        public ViewCodeForm(string content = "")
        {
            InitializeComponent();
            ts = toolStrip1;
            this.Controls.Remove(ts);
            doc = ExplorerForms.ef.Command_OpenGenericDocument("name");
            doc.TopLevel = false;
            doc.Dock = DockStyle.Fill;
            doc.Contents = content;
            doc.Show();
            this.Controls.Add(doc);
            this.Controls.Add(ts);
        }
        ToolStrip ts { get; set; }
        public Document doc { get; set; }
    }
}
