using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using AIMS.Libraries.Forms.Docking;
using AIMS.Libraries.CodeEditor.SyntaxFiles;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.ScriptControl.Converter;
using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.Dom;

using WeifenLuo.WinFormsUI.Docking;

using AIMS.Libraries.CodeEditor;
using VSProvider;
using System.Threading;

namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class Document : DockContent
    {
        private string _FileName = "";
        private string _contents = "";
        private ScriptLanguage _scriptLanguage;
        private ScriptControl _Parent = null;
        public QuickClassBrowserPanel quickClassBrowserPanel = null;
        private Word _lastErrorWord = null;

        public VSProvider.VSProject vp { get; set; }

        public Dictionary<string, string> dc { get; set; }

        //public ImageList bmp { get; set; }

        public Icon icon { get; set; }

        public ParseInformation parse { get; set; }

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


        public void LoadVSProject(VSProvider.VSProject p, string file)
        {
            if (p == null)
                return;

            vp = p;

            CodeEditorCtrl.vp = vp;

            filename = file;

            CodeEditorCtrl.FileName = file;

            string exts = Path.GetExtension(file);

            ImageList imgs = CodeEditorControl.AutoListImages;

            if (VSProject.dc.ContainsKey(exts) == true)
            {
                string b = VSProject.dc[exts];
                DockHandler.Icon = Icon.FromHandle(((Bitmap)imgs.Images[b]).GetHicon());
            }
            else
                DockHandler.Icon = Icon.FromHandle(resource._class.GetHicon());
        }

        public ScriptControl ParentScriptControl
        {
            get { return _Parent; }
        }

        public Document(ScriptControl Parent) : this()
        {
            _Parent = Parent;
        }

        public CodeEditor.CodeEditorControl Editor
        {
            get { return CodeEditorCtrl; }
        }

        public Document()
        {
            InitializeComponent();


            CodeEditorCtrl.Document = this.syntaxDocument1;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            CodeEditorCtrl.Indent = AIMS.Libraries.CodeEditor.WinForms.IndentStyle.LastRow;
            CodeEditorCtrl.LineNumberForeColor = Color.FromArgb(50, CodeEditorCtrl.LineNumberForeColor);
            CodeEditorCtrl.LineNumberBorderColor = Color.FromArgb(50, CodeEditorCtrl.LineNumberBorderColor);
            CodeEditorCtrl.TextChanged += new EventHandler(ActiveViewControl_TextChanged);
            CodeEditorCtrl.ActiveViewControl.KeyDown += new KeyEventHandler(ActiveViewControl_KeyDown);
            CodeEditorCtrl.ActiveViewControl.KeyPress += new KeyPressEventHandler(ActiveViewControl_KeyPress);
            CodeEditorCtrl.CaretChange += new EventHandler(CodeEditorCtrl_CaretChange);
            HostCallBackRegister();
            ShowQuickClassBrowserPanel();
            SetDocument();

            CodeEditorCtrl.ShowLineNumbers = CodeEditorControl.settings.ShowLineNumbers;
        }


        void SetDocument()
        {
            quickClassBrowserPanel.doc = this;
        }

        public void ExpandAll()
        {
            int i = 0;
            while (i < CodeEditorCtrl.Document.Count)
            {
                CodeEditorCtrl.Document[i].Expanded = true;

                i++;
            }
        }

        public void SetVProject(VSProject p)
        {
            vp = p;
            CodeEditorCtrl.vp = p;
        }


        public object obs = new object();

        public AutoResetEvent autoEvent;

        public void LoadKeywords(Document d)
        {
            CodeEditorCtrl.LoadKeywordTypes();

            d.Invoke(new Action(() =>
            {
                
                string s = CodeEditorCtrl.Document.Text;
                CodeEditorCtrl.Document.Text = s;
                if (autoEvent != null)
                    autoEvent.Set();
                
            }));
        }


        public Document(bool b)
        {
            InitializeComponent();



            CodeEditorCtrl.Document = this.syntaxDocument1;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            CodeEditorCtrl.Indent = AIMS.Libraries.CodeEditor.WinForms.IndentStyle.LastRow;
            CodeEditorCtrl.LineNumberForeColor = Color.FromArgb(50, CodeEditorCtrl.LineNumberForeColor);
            CodeEditorCtrl.LineNumberBorderColor = Color.FromArgb(50, CodeEditorCtrl.LineNumberBorderColor);
            CodeEditorCtrl.TextChanged += new EventHandler(ActiveViewControl_TextChanged);
            CodeEditorCtrl.ActiveViewControl.KeyDown += new KeyEventHandler(ActiveViewControl_KeyDown);
            CodeEditorCtrl.ActiveViewControl.KeyPress += new KeyPressEventHandler(ActiveViewControl_KeyPress);
            CodeEditorCtrl.CaretChange += new EventHandler(CodeEditorCtrl_CaretChange);
            HostCallBackRegister();
            //ShowQuickClassBrowserPanel();

            CodeEditorCtrl.ShowLineNumbers = CodeEditorControl.settings.ShowLineNumbers;
            CodeEditorCtrl.BackColor = SystemColors.InactiveBorder;
        }

        public event EventHandler<EventArgs> TextChange;

        private void CodeEditorCtrl_TextChange(object sender, EventArgs e)
        {
            OnTextChange(e);
        }


        public QuickClassBrowserPanel QuickClassBrowserPanel
        {
            get
            {
                return quickClassBrowserPanel;
            }
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

        public void JumpToFilePosition(string fileName, int Line, int Column)
        {
            CodeEditorCtrl.ActiveViewControl.Caret.Position = new TextPoint(Column, Line);
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
        private void ActiveViewControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control & e.KeyCode == Keys.Space)
            {
                CodeEditorCtrl.ActiveViewControl.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(), '\0');
                e.Handled = true;
            }
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

        private bool IsInComment(CodeEditor.WinForms.EditViewControl editor)
        {
            Dom.CSharp.CSharpExpressionFinder ef = new Dom.CSharp.CSharpExpressionFinder(this.FileName);
            int cursor = editor.Caret.Offset - 1;
            return ef.FilterComments(this.Contents, ref cursor) == null;
        }
        public string Contents
        {
            get { return _contents; }
            set { CodeEditorCtrl.ActiveViewControl.Document.Text = value; }
        }
        private void ActiveViewControl_TextChanged(object sender, EventArgs e)
        {
            _contents = CodeEditorCtrl.ActiveViewControl.Document.Text;
            ParseContentEventArgs eInfo = new ParseContentEventArgs(this.FileName, _contents);
            OnParseContent(ref eInfo);
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
                CodeEditorCtrl.ActiveViewControl.FileName = value;
                CodeEditorCtrl.FileName = value;
            }
        }


        public void SetLanguage(Language lg)
        {
            CodeEditorCtrl.Document.Parser.Init(lg);

            GetCompletionBinding();
            CodeEditorCtrl.ScrollIntoView(0);
            OnCaretChange(null);
        }

        public Language lgs { get; set; }

        public ScriptLanguage ScriptLanguage
        {
            get { return _scriptLanguage; }
            set
            {
                if (value == ScriptLanguage.CSharp)
                {
                    lgs = CodeEditorSyntaxLoader.SetSyntax(CodeEditorCtrl, SyntaxLanguage.CSharp);
                }
                else if (value == ScriptLanguage.VBNET)
                {
                    lgs = CodeEditorSyntaxLoader.SetSyntax(CodeEditorCtrl, SyntaxLanguage.VBNET);
                }
                else lgs = CodeEditorSyntaxLoader.SetSyntax(CodeEditorCtrl, SyntaxLanguage.XML); ;

                _scriptLanguage = value;
                //Reset The code Completion bindings
                GetCompletionBinding();
                CodeEditorCtrl.ScrollIntoView(0);
                OnCaretChange(null);
            }
        }

        public void HighlightRemove(int LineNo, int ColNo)
        {
            SyntaxDocument Doc = CodeEditorCtrl.ActiveViewControl.Document;
            Word curWord = Doc.GetWordFromPos(new TextPoint(ColNo, LineNo));
            if (curWord != null)
            {
                curWord.HasError = false;
                curWord.HasWarning = false;
                curWord.InfoTip = "";
            }
            if (_lastErrorWord != null)
            {
                _lastErrorWord.HasError = false;
                _lastErrorWord.HasWarning = false;
                _lastErrorWord.InfoTip = "";
            }
        }

        public void HighlightError(int LineNo, int ColNo, bool IsWarning, string error)
        {
            SyntaxDocument Doc = CodeEditorCtrl.ActiveViewControl.Document;
            Word curWord = Doc.GetWordFromPos(new TextPoint(ColNo, LineNo));
            if (curWord != null)
            {
                if (IsWarning)
                    curWord.HasWarning = true;
                else
                    curWord.HasError = true;
                curWord.InfoTip = error;
            }
            _lastErrorWord = curWord;
        }

        public CodeEditor.WinForms.EditViewControl CurrentView
        {
            get { return CodeEditorCtrl.ActiveViewControl; }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            CodeEditorCtrl.Focus();
            ParseContentsNow();
        }

        public void ParseContentsNow()
        {
            ActiveViewControl_TextChanged(null, null);
            quickClassBrowserPanel.doc = this;
            quickClassBrowserPanel.PopulateCombo();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            CodeEditorCtrl.Focus();
        }

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

        public void ShowQuickClassBrowserPanel()
        {
            if (quickClassBrowserPanel == null)
            {
                quickClassBrowserPanel = new QuickClassBrowserPanel(CodeEditorCtrl);
                Controls.Add(quickClassBrowserPanel);
                quickClassBrowserPanel.BackColor = CodeEditorCtrl.GutterMarginColor;
                CodeEditorCtrl.BorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.None;
                CodeEditorCtrl.ActiveViewControl.BorderColor = CodeEditorCtrl.GutterMarginBorderColor;
                CodeEditorCtrl.ActiveViewControl.BorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.FixedSingle;

                //CodeEditorCtrl.BorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.FixedSingle;
            }
        }
        private void RemoveQuickClassBrowserPanel()
        {
            if (quickClassBrowserPanel != null)
            {
                Controls.Remove(quickClassBrowserPanel);
                quickClassBrowserPanel.Dispose();
                quickClassBrowserPanel = null;
                //CodeEditorCtrl.BorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.None;
            }
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