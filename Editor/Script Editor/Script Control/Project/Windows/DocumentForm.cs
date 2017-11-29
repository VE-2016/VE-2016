using System;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace ScriptControl
{
    public partial class DocumentForm : DockContent
    {
        private string _FileName = "";
        private string _contents = "";
        private ScriptLanguage _scriptLanguage;
        private Control _Parent = null;

        public Control ParentScriptControl
        {
            get { return _Parent; }
        }

        public DocumentForm(Control Parent)
            : this()
        {
            _Parent = Parent;
        }

        public DocumentForm()
        {
            InitializeComponent();

            this.ShowIcon = false;
            this.ShowInTaskbar = false;
        }

        public event EventHandler<EventArgs> CaretChange;

        private void CodeEditorCtrl_CaretChange(object sender, EventArgs e)
        {
            OnCaretChange(e);
        }

        protected virtual void OnCaretChange(EventArgs e)
        {
            if (CaretChange != null)
            {
                CaretChange(this, e);
            }
        }

        public void JumpToFilePosition(string fileName, int Line, int Column)
        {
        }

        private void ActiveViewControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = HandleKeyPress(e.KeyChar);
        }

        private bool IsVariable(string varName)
        {
            return false;
        }

        private void ActiveViewControl_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private bool _inHandleKeyPress;

        private bool HandleKeyPress(char ch)
        {
            if (_inHandleKeyPress)
                return false;
            _inHandleKeyPress = true;
            return false;
        }

        private void ActiveViewControl_TextChanged(object sender, EventArgs e)
        {
        }

        #region Events

        public event EventHandler<ParseContentEventArgs> ParseContent;

        protected virtual void OnParseContent(ref ParseContentEventArgs e)
        {
            if (ParseContent != null)
            {
                ParseContent(this, e);
            }
        }

        # endregion

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
            }
        }

        public string resource { get; set; }
    }
}