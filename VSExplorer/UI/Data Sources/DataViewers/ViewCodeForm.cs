using ScriptControl.Properties;
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
            //doc.Contents = content;
            doc.Show();
            this.Controls.Add(doc);
            this.Controls.Add(ts);
        }

        private ToolStrip ts { get; set; }
        public AvalonDocument doc { get; set; }
    }
}