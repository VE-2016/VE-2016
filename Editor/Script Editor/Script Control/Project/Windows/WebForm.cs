using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ScriptControl
{
    public partial class WebForm : DockContent
    {
        private string _FileName = "";
        private string _contents = "";
        private ScriptLanguage _scriptLanguage;
        private Control _Parent = null;

        public Control ParentScriptControl
        {
            get { return _Parent; }
        }

        public WebForm(Control Parent)
            : this()
        {
            _Parent = Parent;
        }

        public WebForm()
        {
            InitializeComponent();

            // wb = webBrowser1;
        }
    }
}