using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using VSProvider;
using WeifenLuo.WinFormsUI.Docking;

namespace ScriptControl
{
    public partial class Document : DockContent
    {
        private string _FileName = "";
        private string _contents = "";
        private ScriptLanguage _scriptLanguage;
        private Form _Parent = null;

        public VSProvider.VSProject vp { get; set; }

        public Dictionary<string, string> dc { get; set; }

        //public ImageList bmp { get; set; }

        public Icon icon { get; set; }

        public string filename { get; set; }

        public bool ReadyToSave = false;

        public enum State
        {
            waiting,
            working,
            exiting,
            none
        }

        public State state = State.none;

        public Form ParentScriptControl
        {
            get { return _Parent; }
        }

        public Document(Form Parent) : this()
        {
            _Parent = Parent;

            //this.CodeEditorCtrl.ActiveViewControl.BreakPointAdded += Parent.BreakPointAdded;
        }

        public Document()
        {
            InitializeComponent();

            //CodeEditorCtrl.Document = this.syntaxDocument1;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
        }

        private void SetDocument()
        {
        }

        public void ExpandAll()
        {
        }

        public void SetVProject(VSProject p)
        {
        }

        public object obs = new object();

        public AutoResetEvent autoEvent;

        public void LoadKeywords(Document d)
        {
        }

        public Document(bool b)
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> TextChange;

        private void CodeEditorCtrl_TextChange(object sender, EventArgs e)
        {
            OnTextChange(e);
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

        protected virtual void OnTextChange(EventArgs e)
        {
            if (TextChange != null)
            {
                TextChange(this, e);
            }
        }

        private void ActiveViewControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnTextChange(new EventArgs());

            e.Handled = HandleKeyPress(e.KeyChar);

            //if (e.Handled == false && e.KeyChar == '.' &&  this.CodeEditorCtrl.AutoListVisible == false )
            //{
            //    string SearchWord = this.Editor.ActiveViewControl.Caret.CurrentWord.Text;
            //    string FoundWord="";

            //    AIMS.Libraries.CodeEditor.WinForms.CompletionWindow.ICompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
            //    AIMS.Libraries.CodeEditor.WinForms.CompletionWindow.ICompletionData[] completionData = cdp.GenerateCompletionData(this.FileName, this.Editor.ActiveViewControl,(char) e.KeyChar);

            //    foreach (AIMS.Libraries.CodeEditor.WinForms.CompletionWindow.ICompletionData data in completionData)
            //    {
            //        if (SearchWord.ToLower() == data.Text.ToLower())
            //        {
            //            FoundWord = data.Text;
            //            break;
            //        }

            //    }

            //    if (FoundWord.Length > 0)
            //    {
            //        this.Editor.ActiveViewControl.Caret.CurrentWord.Text = FoundWord;
            //        e.Handled = HandleKeyPress(e.KeyChar);
            //        return ;
            //    }
            //}
        }

        private bool IsVariable(string varName)
        {
            return false;
        }

        private bool _inHandleKeyPress;

        private bool HandleKeyPress(char ch)
        {
            if (_inHandleKeyPress)
                return false;
            _inHandleKeyPress = true;
            try
            {
                //if (CodeCompletionOptions.EnableCodeCompletion && Editor.AutoListVisible == false)
                //{
                //    foreach (ICodeCompletionBinding ccBinding in CodeCompletionBindings)
                //    {
                //        if (ccBinding.HandleKeyPress(CodeEditorCtrl, ch))
                //            return false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _inHandleKeyPress = false;
            }
            return false;
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

        protected override void OnGotFocus(EventArgs e)
        {
        }

        public void HostCallBackRegister()
        {
        }

        public void HostCallBackUnRegister()
        {
        }
    }

    public class ParseContentEventArgs : EventArgs
    {
        public string FileName = "";
        public string Content = "";
        public int Column = 0;
        public int Line = 0;

        public ParseContentEventArgs(string fileName, string content)
        {
            FileName = fileName;
            Content = content;
        }
    }
}