using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.SyntaxFiles;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.ScriptControl.Converter;
using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.Dom;
using WeifenLuo.WinFormsUI.Docking;

namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class FileForm : DockContent
    {
        private string _FileName = "";
        private string _contents = "";
        private ScriptLanguage _scriptLanguage;
        private Control _Parent = null;
        //QuickClassBrowserPanel quickClassBrowserPanel = null;
        private Word _lastErrorWord = null;
        public Control ParentScriptControl
        {
            get { return _Parent; }
        }

        public FileForm(Control Parent)
            : this()
        {
            _Parent = Parent;
        }



        public FileForm()
        {
            InitializeComponent();

            wb = webBrowser1;

            HostCallBackRegister();
        }

        public void LoadFile(string file)
        {
            RichTextBox r = richTextBox1;

            r.Clear();

            if (File.Exists(file) == false)
                return;

            string content = File.ReadAllText(file);

            r.AppendText(content);
        }


        public WebBrowser wb { get; set; }



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
        private ICodeCompletionBinding[] GetCompletionBinding()
        {
            ICodeCompletionBinding[] bindings = null;
            if (_scriptLanguage == ScriptLanguage.CSharp)
                bindings = new ICodeCompletionBinding[] { new CSharpCompletionBinding() };
            else
                bindings = new ICodeCompletionBinding[] { new VBNetCompletionBinding() };
            return bindings;
        }

        private ICodeCompletionBinding[] _codeCompletionBindings;
        public ICodeCompletionBinding[] CodeCompletionBindings
        {
            get
            {
                if (_codeCompletionBindings == null)
                {
                    try
                    {
                        _codeCompletionBindings = GetCompletionBinding();
                    }
                    catch
                    {
                        _codeCompletionBindings = new ICodeCompletionBinding[] { };
                    }
                }
                return _codeCompletionBindings;
            }
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


        public void SetLanguage(Language lg)
        {
        }

        public Language lgs { get; set; }






        public void HostCallBackRegister()
        {
            // Must be implemented. Gets the parse information for the specified file.
            HostCallback.GetParseInformation = delegate (string fileName)
            {
                //if (fileName != this.FileName)
                //    throw new Exception("Unknown file");
                return Parser.ProjectParser.GetParseInformation(this.FileName);
            };

            // Must be implemented. Gets the project content of the active project.
            HostCallback.GetCurrentProjectContent = delegate
            {
                return Parser.ProjectParser.CurrentProjectContent;
            };

            // The default implementation just logs to Log4Net. We want to display a MessageBox.
            // Note that we use += here - in this case, we want to keep the default Log4Net implementation.
            HostCallback.ShowError += delegate (string message, Exception ex)
            {
                MessageBox.Show(message + Environment.NewLine + ex.ToString());
            };
            HostCallback.ShowMessage += delegate (string message)
            {
                MessageBox.Show(message);
            };
            HostCallback.ShowAssemblyLoadError += delegate (string fileName, string include, string message)
            {
                MessageBox.Show("Error loading code-completion information for "
                                + include + " from " + fileName
                                + ":\r\n" + message + "\r\n");
            };
        }

        public void HostCallBackUnRegister()
        {
            // Must be implemented. Gets the parse information for the specified file.
            HostCallback.GetParseInformation = null;
            // Must be implemented. Gets the project content of the active project.
            HostCallback.GetCurrentProjectContent = null;
            // The default implementation just logs to Log4Net. We want to display a MessageBox.
            // Note that we use += here - in this case, we want to keep the default Log4Net implementation.
            HostCallback.ShowError = null;
            HostCallback.ShowMessage = null;
            HostCallback.ShowAssemblyLoadError = null;
        }
    }
}