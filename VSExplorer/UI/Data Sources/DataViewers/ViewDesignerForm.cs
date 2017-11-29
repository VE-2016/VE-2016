using ScriptControl.Properties;
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

        public AvalonDocument doc { get; set; }
    }
}