//Coded by Rajneesh Noonia 2007

#region using ...

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.IntelliMouse;
using AIMS.Libraries.CodeEditor.Win32;
using AIMS.Libraries.CodeEditor.WinForms.Painter;
using AIMS.Libraries.CodeEditor.WinForms.TextDraw;
using AIMS.Libraries.CodeEditor.Core.Timers;
using AIMS.Libraries.CodeEditor.Core.Globalization;
using ScrollEventArgs = AIMS.Libraries.CodeEditor.WinForms.IntelliMouse.ScrollEventArgs;
using ScrollEventHandler = AIMS.Libraries.CodeEditor.WinForms.IntelliMouse.ScrollEventHandler;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.CodeEditor.WinForms.InsightWindow;
using ICSharpCode.NRefactory;
using System.Threading;
using ICSharpCode.NRefactory.Semantics;
using Mono.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.Resolver;
using VSProvider;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Microsoft.CSharp;
using System.CodeDom.Compiler;


using System.Linq;
using System.Runtime.InteropServices;
using GACProject;
using System.Xml;

#endregion

namespace AIMS.Libraries.CodeEditor.WinForms
{
    [ToolboxItem(false)]
    public sealed class EditViewControl : SplitViewChildWidget
    {
        #region General Declarations

        private int _mouseX = 0;
        private int _mouseY = 0;
        private MouseButtons _mouseButton = 0;

        /// <summary>
        /// The Point in the text where the Autolist was activated.
        /// </summary>
        private TextPoint _AutoListStartPos = null;
        private TextPoint _AutoListPos = null;
        public Selection _Selection;
        private Caret _Caret;
        private double _IntelliScrollPos = 0;

        /// <summary>
        /// The Point in the text where the InfoTip was activated.
        /// </summary>		
        public TextPoint InfoTipStartPos = null;

        private bool _AutoListVisible = false;
        private bool _InfoTipVisible = false;
        private bool _OverWrite = false;
        private bool _KeyDownHandled = false;
        public ViewPoint View = new ViewPoint();
        public IPainter Painter;
        private TextDrawType _TextDrawStyle = 0;

        public bool NeededTypeRedaw = true;

        private string[] _textBorderStyles = new string[]
            {
                "****** * ******* * ******",
                "+---+| | |+-+-+| | |+---+",
                "+---+\u00A6 \u00A6 \u00A6\u00A6-+-\u00A6\u00A6 \u00A6 \u00A6+---+",
                "+---+\u00A6 \u00A6 \u00A6+-+-\u00A6\u00A6 \u00A6 \u00A6+---+"
            };

        #endregion

        #region Internal controls

        private WeakTimer _caretTimer;
        private PictureBox _filler;
        private ToolTip _tooltip;
        private IContainer _components;
        private TextPoint _startOffset = null;
        #region PUBLIC PROPERTY FINDREPLACEDIALOG 

        private FindReplaceForm _FindReplaceDialog;

        public FindReplaceForm FindReplaceDialog
        {
            get
            {
                CreateFindForm();


                return _FindReplaceDialog;
            }
            set { _FindReplaceDialog = value; }
        }

        #endregion

        private IntelliMouseControl _intelliMouse;

        #region PUBLIC PROPERTY AUTOLIST

        private AutoListForm _AutoList;

        public AutoListForm AutoList
        {
            get
            {
                CreateAutoList();

                return _AutoList;
            }
            set { _AutoList = value; }
        }

        #endregion

        #region PUBLIC PROPERTY INFOTIP

        private InfoTipForm _InfoTip;

        public PanelExt InfoTip = new PanelExt();


        //public InfoTipForm InfoTip
        //{
        //	get
        //	{
        //		CreateInfoTip();

        //		return _InfoTip;
        //	}
        //	set { _InfoTip = value; }
        //}

        #endregion

        public bool HasAutoList
        {
            get { return _AutoList != null; }
        }

        public bool HasInfoTip
        {
            get { return _InfoTip != null; }
        }


        private WeakReference _Control = null;

        public CodeEditorControl _CodeEditor
        {
            get
            {
                //try
                //{
                if (_Control != null && _Control.IsAlive)
                    return (CodeEditorControl)_Control.Target;
                else
                    return null;
                //}
                //catch
                //{
                //    return null;
                //}
            }
            set { _Control = new WeakReference(value); }
        }

        #endregion

        #region Public events

        /// <summary>
        /// An event that is fired when the caret has moved.
        /// </summary>
        public event EventHandler CaretChange = null;

        /// <summary>
        /// An event that is fired when the selection has changed.
        /// </summary>
        public event EventHandler SelectionChange = null;

        /// <summary>
        /// An event that is fired when mouse down occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseDown = null;

        /// <summary>
        /// An event that is fired when mouse move occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseMove = null;

        /// <summary>
        /// An event that is fired when mouse up occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseUp = null;

        /// <summary>
        /// An event that is fired when a click occurs on a row
        /// </summary>
        public event RowMouseHandler RowClick = null;

        /// <summary>
        /// An event that is fired when a double click occurs on a row
        /// </summary>
        public event RowMouseHandler RowDoubleClick = null;

        /// <summary>
        /// An event that is fired when the control has updated the clipboard
        /// </summary>
        public event CopyHandler ClipboardUpdated = null;


        public event KeyEventHandler KeyEventHandler;


        #endregion

        private LineMarginRender _LineMarginRender = CodeEditorControl.defaultLineMarginRender;
        private Panel _panel1;
        private ContextMenuStrip _contextMenuStrip1;
        private ToolStripMenuItem _toolStripMenuItem1;
        private ToolStripSeparator _toolStripSeparator1;
        private ToolStripMenuItem _toolStripMenuItem2;
        private ToolStripSeparator _toolStripSeparator2;
        private ToolStripMenuItem _toolStripMenuItem3;
        private ToolStripMenuItem _toolStripMenuItem4;
        private ToolStripSeparator _toolStripSeparator3;
        private ToolStripMenuItem _toolStripMenuItem5;
        private ToolStripMenuItem _toolStripMenuItem6;
        private ToolStripMenuItem _toolStripMenuItem7;
        private IContainer components;

        public CodeEditorControl CodeEditor
        {
            get
            {
                //try
                //{
                if (_Control != null && _Control.IsAlive)
                    return (CodeEditorControl)_Control.Target;
                else
                    return null;
                //}
                //catch
                //{
                //    return null;
                //}
            }
            set { _Control = new WeakReference(value); }
        }

        #region PUBLIC PROPERTY IMEWINDOW

        private IMEWindow _IMEWindow;

        public IMEWindow IMEWindow
        {
            get { return _IMEWindow; }
            set { _IMEWindow = value; }
        }

        #endregion

        public string GetRangeDescription(int selectedItem, int itemCount)
        {
            StringBuilder sb = new StringBuilder(selectedItem.ToString());
            sb.Append(" from ");
            sb.Append(itemCount.ToString());
            return sb.ToString();
        }

        private void CreateAutoList()
        {
            if (this._CodeEditor != null && !this._CodeEditor.DisableAutoList && _AutoList == null)
            {
                //Debug.WriteLine("Creating Autolist");

                this.AutoList = new AutoListForm(this);
                this.AutoList.TopLevel = false;
                NativeUser32Api.SetWindowLong(this.AutoList.Handle,
                                            NativeUser32Api.GWL_STYLE,
                                            (int)NativeUser32Api.WS_CHILD);
                this.AutoList.SendToBack();
                this.AutoList.Visible = false;
                _AutoListVisible = false;
                this.Controls.Add(this.AutoList);
                this.AutoList.VisibleChanged += new EventHandler(AutoList_VisibleChanged);
                this.AutoList.DoubleClick += new EventHandler(this.AutoListDoubleClick);
                this.AutoList.Images = this._CodeEditor.AutoListIcons;
            }
        }

        private void AutoList_VisibleChanged(object sender, EventArgs e)
        {
            _AutoListVisible = this.AutoList.Visible;
        }
        private string _currentFileName = null;
        /// <value>
        /// The current file name
        /// </value>
        [Browsable(false)]
        [ReadOnly(true)]
        public string FileName
        {
            get
            {
                return _currentFileName;
            }
            set
            {
                if (_currentFileName != value)
                {
                    _currentFileName = value;
                    //OnFileNameChanged(EventArgs.Empty);
                }
            }
        }

        private string FindMatchedWord(ArrayList list, string wordToSearch)
        {
            int FirstMatch = 0;
            string NextMatchString = "";
            string FinalWord = "";
            ArrayList nextList = null;
            list.Sort();
            FirstMatch = list.BinarySearch(wordToSearch);
            if (FirstMatch < 0)
                FirstMatch = ~FirstMatch;
            if (FirstMatch > 0)
                try
                {
                    if (FirstMatch < list.Count)
                        nextList = list.GetRange(FirstMatch + 1, 1);
                }
                catch
                {
                    nextList = list.GetRange(FirstMatch, 1);
                    FinalWord = nextList[0].ToString();
                }

            if (nextList != null)
                NextMatchString = nextList[0].ToString();

            if (NextMatchString.IndexOf(wordToSearch, StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (FirstMatch < list.Count)
                    FinalWord = list.GetRange(FirstMatch, 1)[0].ToString();
            }
            return FinalWord;
        }

        private bool TryAutoCompletion(ICompletionData[] completionData)
        {
            if (this.Caret.CurrentWord == null) return false;
            string SearchWord = this.Caret.CurrentWord.Text;
            string FinalWord = "";


            ArrayList list = new ArrayList();
            foreach (ICompletionData c in completionData)
            {
                list.Add(c.Text);
            }


            FinalWord = FindMatchedWord(list, SearchWord);
            if (FinalWord.Length > 0)
            {
                TextRange tr = new TextRange();
                tr.FirstRow = Caret.Position.Y;
                tr.LastRow = Caret.Position.Y;

                tr.FirstColumn = Math.Min(_AutoListStartPos.X, _startOffset.X);
                tr.LastColumn = Math.Max(Caret.Position.X, Caret.CurrentWord.Column + Caret.CurrentWord.Text.Length);

                Document.DeleteRange(tr, true);
                Caret.Position.X = _AutoListStartPos.X;
                this.InsertText(FinalWord);
                SetFocus();
                return true;
            }
            return false;
        }

        public ArrayList ExtractKeywords()
        {
            ArrayList LangKeywords = new ArrayList();

            Language lang = this.Document.Parser.Language;
            //Extract only unique keywords.
            for (int i = 0; i <= lang.Blocks.Length - 1; i++)
            {
                BlockType bt = lang.Blocks[i];
                if (bt.KeywordsList.Count > 0)
                {
                    for (int j = 0; j <= bt.KeywordsList.Count - 1; j++)
                    {
                        PatternList p = bt.KeywordsList[j];
                        foreach (string key in p.SimplePatterns.Keys)
                        {
                            if (!LangKeywords.Contains(key))
                                LangKeywords.Add(key);
                        }
                    }
                }
            }
            return LangKeywords;
        }

        public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider, char ch)
        {
        }

        private InsightWindow.InsightWindow _insightWindow = null;
        public void ShowInsightWindow(IInsightDataProvider insightDataProvider)
        {
        }

        private void CloseInsightWindow(object sender, EventArgs e)
        {
        }

        public bool InsightWindowVisible
        {
            get
            {
                return (_insightWindow != null) && _insightWindow.Visible;
            }
        }

        private void CreateFindForm()
        {
            if (!this._CodeEditor.DisableFindForm && _FindReplaceDialog == null)
            {
                Debug.WriteLine("Creating Findform");
                FindReplaceDialog = new FindReplaceForm(this);
            }
        }

        private void CreateInfoTip()
        {
            if (this._CodeEditor != null && !this._CodeEditor.DisableInfoTip && _InfoTip == null)
            {
                Debug.WriteLine("Creating Infotip");

                this.Controls.Add(InfoTip);
                this.InfoTip.SendToBack();
                this.InfoTip.Visible = false;
            }
        }

        #region Constructor

        /// <summary>
        /// Default constructor for the SyntaxBoxControl
        /// </summary>
        public EditViewControl(CodeEditorControl Parent) : base()
        {
            _CodeEditor = Parent;


            Painter = new Painter_GDI(this);
            _Selection = new Selection(this);
            _Caret = new Caret(this);

            _Caret.Change += new EventHandler(this.CaretChanged);
            _Selection.Change += new EventHandler(this.SelectionChanged);

            vScroll.Scroll += VScroll_Scroll;

            InitializeComponent();

            this.BackColor = Color.White;

            CreateAutoList();

            CreateInfoTip();

            _tooltip = null;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Opaque, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            _contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contexMenu_ItemClicked);

            _toolStripMenuItem2.Click += _toolStripMenuItem2_Click;

            MouseMoveTimer = new System.Threading.Timer(new TimerCallback(TimerProc), null, 1000, 1000);

            
        }

              

        private void _toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CodeEditor.FormatCode();
        }

        private void contexMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;

            if (item.Text == "Copy")
            {
                MessageBox.Show("Copy data requested");
                CopyText();
            }
            else if (item.Text == "Paste")
            {
                MessageBox.Show("Paste data requested");
                PasteText();
            }
            else if (item.Text == "Cut")
            {
                MessageBox.Show("Cut data requested");
            }
        }

        private System.Threading.Timer MouseMoveTimer { get; set; }

        private void EditViewControl_Paint(object sender, PaintEventArgs e)
        {
        }

        private void VScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            CodeEditor.Scroll();
        }

        #endregion

        #region DISPOSE()

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            RemoveFocus();

            if (disposing)
            {
                if (_components != null)
                    _components.Dispose();

                if (this.Painter != null)
                    this.Painter.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        ~EditViewControl()
        {
        }

        #region Private/Protected/public Properties

        public int PixelTabSize
        {
            get { return _CodeEditor.TabSize * View.CharWidth; }
        }

        #endregion

        public LineMarginRender LineMarginRender
        {
            get
            {
                return _LineMarginRender;
            }
            set
            {
                _LineMarginRender = value;
            }
        }

        #region Private/Protected/Internal Methods

        private void DoResize()
        {
            this.SuspendLayout();
            if (this.Visible && this.Width > 0 && this.Height > 0 && this.IsHandleCreated)
            {
                if (_filler == null)
                    return;

                TopThumb.Width = SystemInformation.VerticalScrollBarWidth;
                LeftThumb.Height = SystemInformation.HorizontalScrollBarHeight;
                vScroll.Width = SystemInformation.VerticalScrollBarWidth;
                hScroll.Height = SystemInformation.HorizontalScrollBarHeight;

                if (TopThumbVisible)
                {
                    vScroll.Top = TopThumb.Height;
                    if (hScroll.Visible)
                        vScroll.Height = this.ClientHeight - hScroll.Height - TopThumb.Height;
                    else
                        vScroll.Height = this.ClientHeight - TopThumb.Height;
                }
                else
                {
                    if (hScroll.Visible)
                        vScroll.Height = this.ClientHeight - hScroll.Height;
                    else
                        vScroll.Height = this.ClientHeight;

                    vScroll.Top = 0;
                }

                if (LeftThumbVisible)
                {
                    hScroll.Left = LeftThumb.Width;
                    if (vScroll.Visible)
                        hScroll.Width = this.ClientWidth - vScroll.Width - LeftThumb.Width;
                    else
                        hScroll.Width = this.ClientWidth - LeftThumb.Width;
                }
                else
                {
                    if (vScroll.Visible)
                        hScroll.Width = this.ClientWidth - vScroll.Width;
                    else
                        hScroll.Width = this.ClientWidth;

                    hScroll.Left = 0;
                }


                if (this.Width != _oldWidth && this.Width > 0)
                {
                    _oldWidth = this.Width;
                    if (Painter != null)
                        Painter.Resize();
                }


                vScroll.Left = this.ClientWidth - vScroll.Width;
                hScroll.Top = this.ClientHeight - hScroll.Height;

                LeftThumb.Left = 0;
                LeftThumb.Top = hScroll.Top;

                TopThumb.Left = vScroll.Left;
                ;
                TopThumb.Top = 0;


                _filler.Left = vScroll.Left;
                _filler.Top = hScroll.Top;
                _filler.Width = vScroll.Width;
                _filler.Height = hScroll.Height;
            }
            if (CodeEditor != null)
                CodeEditor.Scroll();

            this.ResumeLayout(false);
        }

        public void InsertText(string text)
        {
            Caret.CropPosition();
            if (Selection.IsValid)
            {
                Selection.DeleteSelection();
                InsertText(text);
            }
            else
            {
                if (!_OverWrite || text.Length > 1)
                {
                    Row xtr = Caret.CurrentRow;
                    TextPoint p = Document.InsertText(text, Caret.Position.X, Caret.Position.Y);
                    Caret.CurrentRow.Parse(true);
                    if (text.Length == 1) { 
                    
                        if(text != "\t")
                        Document.ResetVisibleRows();
                        Caret.SetPos(p);
                        Caret.CaretMoved(false);
                    }
                    else
                    {
                        //Document.i = true;

                        Document.ResetVisibleRows();
                        Caret.SetPos(p);
                        Caret.CaretMoved(false);
                    }
                }
                else
                {
                    TextRange r = new TextRange();
                    r.FirstColumn = Caret.Position.X;
                    r.FirstRow = Caret.Position.Y;
                    r.LastColumn = Caret.Position.X + 1;
                    r.LastRow = Caret.Position.Y;
                    UndoBlockCollection ag = new UndoBlockCollection();
                    UndoBlock b;
                    b = new UndoBlock();
                    b.Action = UndoAction.DeleteRange;
                    b.Text = Document.GetRange(r);
                    b.Position = Caret.Position;
                    ag.Add(b);
                    Document.DeleteRange(r, false);
                    b = new UndoBlock();
                    b.Action = UndoAction.InsertRange;
                    string NewChar = text;
                    b.Text = NewChar;
                    b.Position = Caret.Position;
                    ag.Add(b);
                    Document.AddToUndoList(ag);
                    Document.InsertText(NewChar, Caret.Position.X, Caret.Position.Y, false);
                    Caret.CurrentRow.Parse(true);

                    Caret.MoveRight(false);
                }
            }
            //	this.ScrollIntoView ();

        }

        private void InsertEnter()
        {
            Caret.CropPosition();
            if (Selection.IsValid)
            {
                Selection.DeleteSelection();
                InsertEnter();
            }
            else
            {
                switch (this.Indent)
                {
                    case IndentStyle.None:
                        {
                            Row xtr = Caret.CurrentRow;
                            Document.InsertText("\n", Caret.Position.X, Caret.Position.Y);
                            //depends on what sort of indention we are using....
                            Caret.CurrentRow.Parse();
                            Caret.MoveDown(false);
                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);

                            Caret.Position.X = 0;
                            Caret.SetPos(Caret.Position);
                            break;
                        }
                    case IndentStyle.LastRow:
                        {
                            Row xtr = Caret.CurrentRow;
                            string indent = xtr.GetLeadingWhitespace();
                            int Max = Math.Min(indent.Length, Caret.Position.X);
                            string split = "\n" + indent.Substring(0, Max);
                            Document.InsertText(split, Caret.Position.X, Caret.Position.Y);
                            Document.ResetVisibleRows();
                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);
                            Caret.MoveDown(false);
                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);

                            Caret.Position.X = indent.Length;
                            Caret.SetPos(Caret.Position);
                            //BOO xtr.Parse(false);
                            xtr.Parse(true);
                            //BOO xtr.NextRow.Parse(false);
                            xtr.NextRow.Parse(true);

                            break;
                        }
                    case IndentStyle.Scope:
                        {
                            Row xtr = Caret.CurrentRow;
                            xtr.Parse(true);
                            if (xtr.ShouldOutdent)
                            {
                                OutdentEndRow();
                            }

                            Document.InsertText("\n", Caret.Position.X, Caret.Position.Y);
                            //depends on what sort of indention we are using....
                            Caret.CurrentRow.Parse();
                            Caret.MoveDown(false);
                            Caret.CurrentRow.Parse(false);

                            string indent = new String('\t', Caret.CurrentRow.Depth);
                            Document.InsertText(indent, 0, Caret.Position.Y);

                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);

                            Caret.Position.X = indent.Length;
                            Caret.SetPos(Caret.Position);
                            Caret.CropPosition();
                            Selection.ClearSelection();

                            //BOO xtr.Parse(false);
                            xtr.Parse(true);
                            //BOO xtr.NextRow.Parse(false);
                            xtr.NextRow.Parse(true);

                            break;
                        }
                    case IndentStyle.Smart:
                        {
                            Row xtr = Caret.CurrentRow;
                            if (xtr.ShouldOutdent)
                            {
                                OutdentEndRow();
                            }
                            Document.InsertText("\n", Caret.Position.X, Caret.Position.Y);
                            Caret.MoveDown(false);
                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);
                            //BOO Caret.CurrentRow.StartSegment.StartRow.Parse(false);
                            Caret.CurrentRow.StartSegment.StartRow.Parse(true);

                            string prev = "\t" + Caret.CurrentRow.StartSegment.StartRow.GetVirtualLeadingWhitespace();

                            string indent = Caret.CurrentRow.PrevRow.GetLeadingWhitespace();
                            if (indent.Length < prev.Length)
                                indent = prev;

                            string ts = "\t" + new String(' ', this.TabSize);
                            while (indent.IndexOf(ts) >= 0)
                            {
                                indent = indent.Replace(ts, "\t\t");
                            }

                            Document.InsertText(indent, 0, Caret.Position.Y);

                            //BOO Caret.CurrentRow.Parse(false);
                            Caret.CurrentRow.Parse(true);

                            Caret.Position.X = indent.Length;
                            Caret.SetPos(Caret.Position);

                            Caret.CropPosition();
                            Selection.ClearSelection();
                            //BOO xtr.Parse(false);
                            xtr.Parse(true);
                            //BOO xtr.NextRow.Parse(false);
                            xtr.NextRow.Parse(true);
                            break;
                        }
                }
                ScrollIntoView();
            }
        }

        private void OutdentEndRow()
        {
            //try
            //{
            if (this.Indent == IndentStyle.Scope)
            {
                Row xtr = Caret.CurrentRow;
                string ct = xtr.Text.Substring(0, xtr.GetLeadingWhitespace().Length);
                string indent1 = new String('\t', Caret.CurrentRow.Depth);
                TextRange tr = new TextRange();
                tr.FirstColumn = 0;
                tr.LastColumn = xtr.GetLeadingWhitespace().Length;
                tr.FirstRow = xtr.Index;
                tr.LastRow = xtr.Index;
                this.Document.DeleteRange(tr);
                this.Document.InsertText(indent1, 0, xtr.Index, true);

                int diff = indent1.Length - tr.LastColumn;
                Caret.Position.X += diff;
                Caret.SetPos(Caret.Position);
                Caret.CropPosition();
                Selection.ClearSelection();
                //BOO Caret.CurrentRow.Parse(false);
                Caret.CurrentRow.Parse(true);
            }
            else if (this.Indent == IndentStyle.Smart)
            {
                Row xtr = Caret.CurrentRow;

                if (xtr.FirstNonWsWord == xtr.Expansion_EndSegment.EndWord)
                {
                    string ct = xtr.Text.Substring(0, xtr.GetLeadingWhitespace().Length);
                    //int j=xtr.Expansion_StartRow.StartWordIndex;
                    string indent1 = xtr.StartSegment.StartWord.Row.GetVirtualLeadingWhitespace();
                    TextRange tr = new TextRange();
                    tr.FirstColumn = 0;
                    tr.LastColumn = xtr.GetLeadingWhitespace().Length;
                    tr.FirstRow = xtr.Index;
                    tr.LastRow = xtr.Index;
                    this.Document.DeleteRange(tr);
                    string ts = "\t" + new String(' ', this.TabSize);
                    while (indent1.IndexOf(ts) >= 0)
                    {
                        indent1 = indent1.Replace(ts, "\t\t");
                    }
                    this.Document.InsertText(indent1, 0, xtr.Index, true);

                    int diff = indent1.Length - tr.LastColumn;
                    Caret.Position.X += diff;
                    Caret.SetPos(Caret.Position);
                    Caret.CropPosition();
                    Selection.ClearSelection();
                    //BOO   Caret.CurrentRow.Parse(false);
                    Caret.CurrentRow.Parse(true);
                }
            }
            //}
            //catch
            //{
            //}
        }

        private void DeleteForward()
        {
            Caret.CropPosition();
            if (Selection.IsValid)
                Selection.DeleteSelection();
            else
            {
                Row xtr = Caret.CurrentRow;
                if (Caret.Position.X == xtr.Text.Length)
                {
                    if (Caret.Position.Y <= Document.Count - 2)
                    {
                        TextRange r = new TextRange();
                        r.FirstColumn = Caret.Position.X;
                        r.FirstRow = Caret.Position.Y;
                        r.LastRow = r.FirstRow + 1;
                        r.LastColumn = 0;

                        Document.DeleteRange(r);
                        Document.ResetVisibleRows();
                    }
                }
                else
                {
                    TextRange r = new TextRange();
                    r.FirstColumn = Caret.Position.X;
                    r.FirstRow = Caret.Position.Y;
                    r.LastRow = r.FirstRow;
                    r.LastColumn = r.FirstColumn + 1;
                    Document.DeleteRange(r);
                    Document.ResetVisibleRows();
                    //BOO Caret.CurrentRow.Parse(false);
                    Caret.CurrentRow.Parse(true);
                }
            }
        }

        private void DeleteBackwards()
        {
            Caret.CropPosition();
            if (Selection.IsValid)
                Selection.DeleteSelection();
            else
            {
                Row xtr = Caret.CurrentRow;
                if (Caret.Position.X == 0)
                {
                    if (Caret.Position.Y > 0)
                    {
                        Caret.Position.Y--;
                        Caret.MoveEnd(false);
                        DeleteForward();
                        //Caret.CurrentRow.Parse ();
                        Document.ResetVisibleRows();
                    }
                }
                else
                {
                    if (Caret.Position.X >= xtr.Text.Length)
                    {
                        TextRange r = new TextRange();
                        r.FirstColumn = Caret.Position.X - 1;
                        r.FirstRow = Caret.Position.Y;
                        r.LastRow = r.FirstRow;
                        r.LastColumn = r.FirstColumn + 1;
                        Document.DeleteRange(r);
                        Document.ResetVisibleRows();
                        Caret.MoveEnd(false);
                        Caret.CurrentRow.Parse();
                    }
                    else
                    {
                        TextRange r = new TextRange();
                        r.FirstColumn = Caret.Position.X - 1;
                        r.FirstRow = Caret.Position.Y;
                        r.LastRow = r.FirstRow;
                        r.LastColumn = r.FirstColumn + 1;
                        Document.DeleteRange(r);
                        Document.ResetVisibleRows();
                        Caret.MoveLeft(false);
                        Caret.CurrentRow.Parse();
                    }
                }
            }
        }

        private void ScrollScreen(int Amount)
        {
            ScrollScreen(Amount, 2);
        }

        private void ScrollScreen(int Amount, int speed)
        {
            //try
            //{
            // this.tooltip.RemoveAll();

            int newval = vScroll.Value + Amount;

            newval = Math.Max(newval, vScroll.Minimum);
            newval = Math.Min(newval, vScroll.Maximum);

            if (newval >= vScroll.Maximum - 2)
                newval = vScroll.Maximum - 2;

            vScroll.Value = newval;
            this.Redraw();
            //}
            //catch
            //{
            //}
        }



        private void PasteText()
        {
            //try
            //{
            IDataObject iData = Clipboard.GetDataObject();

            if (iData.GetDataPresent(DataFormats.UnicodeText))
            {
                // Yes it is, so display it in a text box.
                string s = (string)iData.GetData(DataFormats.UnicodeText);

                InsertText(s);
                if (ParseOnPaste)
                    this.Document.ParseAll(true);
            }
            else if (iData.GetDataPresent(DataFormats.Text))
            {
                // Yes it is, so display it in a text box.
                string s = (string)iData.GetData(DataFormats.Text);

                InsertText(s);
                if (ParseOnPaste)
                    this.Document.ParseAll(true);
            }
            //}
            //catch
            //{
            //    //ignore
            //}
        }


        private void BeginDragDrop()
        {
            this.DoDragDrop(Selection.Text, DragDropEffects.All);
        }

        private void Redraw()
        {
            this.Invalidate();
        }
        private void Redraw(bool neededTypeRedaw)
        {
            this.NeededTypeRedaw = neededTypeRedaw;
            this.Invalidate();
        }

        private void RedrawCaret()
        {
            using (Graphics gfx = this.CreateGraphics())
            {
                Painter.RenderCaret(gfx);
            }
        }

        private void SetMouseCursor(int x, int y)
        {
            if (_CodeEditor.LockCursorUpdate)
            {
                this.Cursor = _CodeEditor.Cursor;
                return;
            }

            if (View.Action == XTextAction.xtDragText)
            {
                this.Cursor = System.Windows.Forms.Cursors.Hand;
                //Cursor.Current = Cursors.Hand;
            }
            else
            {
                if (x < View.TotalMarginWidth)
                {
                    if (x < View.GutterMarginWidth)
                    {
                        this.Cursor = System.Windows.Forms.Cursors.Arrow;
                    }
                    else
                    {
                        Assembly assembly = GetType().Assembly;

                        Stream strm = assembly.GetManifestResourceStream("AIMS.Libraries.CodeEditor.FlippedCursor.cur");
                        this.Cursor = new Cursor(strm);
                    }
                }
                else
                {
                    if (x > View.TextMargin - 8)
                    {
                        if (IsOverSelection(x, y))
                            this.Cursor = System.Windows.Forms.Cursors.Arrow;
                        else
                        {
                            TextPoint tp = this.Painter.CharFromPixel(x, y);
                            Word w = this.Document.GetWordFromPos(tp);
                            if (w != null && w.Pattern != null && w.Pattern.Category != null)
                            {
                                WordMouseEventArgs e = new WordMouseEventArgs();
                                e.Pattern = w.Pattern;
                                e.Button = MouseButtons.None;
                                e.Cursor = System.Windows.Forms.Cursors.Hand;
                                e.Word = w;

                                this._CodeEditor.OnWordMouseHover(ref e);

                                this.Cursor = e.Cursor;
                            }
                            else
                                this.Cursor = System.Windows.Forms.Cursors.IBeam;
                        }
                    }
                    else
                    {
                        this.Cursor = System.Windows.Forms.Cursors.Arrow;
                    }
                }
            }
        }

        private void CopyText()
        {
            //no freaking vs.net copy empty selection 
            if (!Selection.IsValid)
                return;

            //if (this._CodeEditor.CopyAsRTF)
            //{
            //    this.CopyAsRTF();


            //}
            //else
            {
                //try
                //{
                try
                {
                    if (this.Selection != null)
                    {
                        string t = Selection.Text;
                        Clipboard.SetDataObject(t, true);
                        CopyEventArgs ea = new CopyEventArgs();
                        ea.Text = t;
                        OnClipboardUpdated(ea);
                    }
                }
                catch
                {
                }
                //}
                //catch
                //{
                //    try
                //    {
                //        string t = Selection.Text;
                //        Clipboard.SetDataObject(t, true);
                //        CopyEventArgs ea = new CopyEventArgs();
                //        ea.Text = t;
                //        OnClipboardUpdated(ea);
                //    }
                //    catch
                //    {
                //    }
                //}
            }
        }

        /// <summary>
        /// For public use only
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                case Keys.Tab:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Enter:
                    return true;
            }
            return true; //base.IsInputKey(key);			
        }

        protected override bool IsInputChar(char c)
        {
            return true;
        }

        private void TextDraw(TextDrawDirectionType Direction)
        {
            TextRange r = new TextRange();
            r.FirstColumn = Caret.Position.X;
            r.FirstRow = Caret.Position.Y;
            r.LastColumn = Caret.Position.X + 1;
            r.LastRow = Caret.Position.Y;

            int Style = (int)this.TextDrawStyle;
            string OldChar = Document.GetRange(r);
            string BorderString = _textBorderStyles[Style];
            //TextBorderChars OldCharType=0;

            if (OldChar == "")
                OldChar = " ";


            UndoBlockCollection ag = new UndoBlockCollection();
            UndoBlock b;
            b = new UndoBlock();
            b.Action = UndoAction.DeleteRange;
            b.Text = Document.GetRange(r);
            b.Position = Caret.Position;
            ag.Add(b);
            Document.DeleteRange(r, false);

            b = new UndoBlock();
            b.Action = UndoAction.InsertRange;


            string NewChar = "*";


            b.Text = NewChar;
            b.Position = Caret.Position;
            ag.Add(b);
            Document.AddToUndoList(ag);
            Document.InsertText(NewChar, Caret.Position.X, Caret.Position.Y, false);
        }


        public void RemoveFocus()
        {
            if (this.InfoTip == null || this.AutoList == null)
                return;

            if (!this.ContainsFocus && !InfoTip.ContainsFocus && !AutoList.ContainsFocus)
            {
                _caretTimer.Enabled = false;
                this.Caret.Blink = false;
                //Removed for Debugging Rajneesh
                // this.AutoListVisible = false;
                _InfoTipVisible = false;
            }
            this.Redraw();
        }

        private void SelectCurrentWord()
        {
            Row xtr = Caret.CurrentRow;
            if (xtr.Text == "")
                return;

            if (Caret.Position.X >= xtr.Text.Length)
                return;

            string Char = xtr.Text.Substring(Caret.Position.X, 1);
            int Type = CharType(Char);

            int left = Caret.Position.X;
            int right = Caret.Position.X;

            while (left >= 0 && CharType(xtr.Text.Substring(left, 1)) == Type)
                left--;

            while (right <= xtr.Text.Length - 1 && CharType(xtr.Text.Substring(right, 1)) == Type)
                right++;

            Selection.Bounds.FirstRow = Selection.Bounds.LastRow = xtr.Index;
            Selection.Bounds.FirstColumn = left + 1;
            Selection.Bounds.LastColumn = right;
            Caret.Position.X = right;
            Caret.SetPos(Caret.Position);
            this.Redraw();
        }

        private int CharType(string s)
        {
            string g1 = " \t";
            string g2 = ".,-+'?\u00B4=)(/&%\u00A4#!\"\\<>[]$\u00A3@*:;{}";

            if (g1.IndexOf(s) >= 0)
                return 1;

            if (g2.IndexOf(s) >= 0)
                return 2;

            return 3;
        }

        private void SelectPattern(int RowIndex, int Column, int Length)
        {
            this.Selection.Bounds.FirstColumn = Column;
            this.Selection.Bounds.FirstRow = RowIndex;
            this.Selection.Bounds.LastColumn = Column + Length;
            this.Selection.Bounds.LastRow = RowIndex;
            this.Caret.Position.X = Column + Length;
            this.Caret.Position.Y = RowIndex;
            this.Caret.CurrentRow.EnsureVisible();
            this.ScrollIntoView();
            this.Redraw();
        }

        public void InitVars()
        {
            //setup viewpoint data


            if (View.RowHeight == 0)
                View.RowHeight = 48;

            if (View.CharWidth == 0)
                View.CharWidth = 16;

            //View.RowHeight=16;
            //View.CharWidth=8;

            View.FirstVisibleColumn = hScroll.Value;
            View.FirstVisibleRow = vScroll.Value;
            //	View.yOffset =_yOffset;

            View.VisibleRowCount = 0;
            if (hScroll.Visible)
                View.VisibleRowCount = (this.Height - hScroll.Height) / View.RowHeight + 1;
            else
                View.VisibleRowCount = (this.Height - hScroll.Height) / View.RowHeight + 2;

            if (this.ShowGutterMargin)
                View.GutterMarginWidth = this.GutterMarginWidth;
            else
                View.GutterMarginWidth = 0;

            if (this.ShowLineNumbers)
            {
                int chars = (Document.Count).ToString().Length;
                string s = new String('9', chars);
                View.LineNumberMarginWidth = 10 + this.Painter.MeasureString(s).Width;
            }
            else
                View.LineNumberMarginWidth = 0;




            View.TotalMarginWidth = View.GutterMarginWidth + View.LineNumberMarginWidth;
            if (Document.Folding)
                View.TextMargin = View.TotalMarginWidth + 20;
            else
                View.TextMargin = View.TotalMarginWidth + 7;


            View.ClientAreaWidth = this.Width - vScroll.Width - View.TextMargin;
            View.ClientAreaStart = View.FirstVisibleColumn * View.CharWidth;
        }

        private int _maxCharWidth = 8;

        public Point GetDrawingPos(TextPoint tp)
        {
            return this.Painter.GetDrawingPos(tp);
        }
        public void CalcMaxCharWidth()
        {
            _maxCharWidth = this.Painter.GetMaxCharWidth();
        }

        public void SetMaxHorizontalScroll()
        {
            CalcMaxCharWidth();
            int CharWidth = this.View.CharWidth;
            if (CharWidth == 0)
                CharWidth = 1;

            if (this.View.ClientAreaWidth / CharWidth < 0)
            {
                this.hScroll.Maximum = 1000;
                return;
            }

            this.hScroll.LargeChange = this.View.ClientAreaWidth / CharWidth;

            try
            {
                int max = 0;
                for (int i = this.View.FirstVisibleRow; i < this.Document.VisibleRows.Count; i++)
                {
                    if (i >= this.View.VisibleRowCount + this.View.FirstVisibleRow)
                        break;

                    string l = "";

                    if (this.Document.VisibleRows[i].IsCollapsed)
                        l = this.Document.VisibleRows[i].VirtualCollapsedRow.Text;
                    else
                        l = this.Document.VisibleRows[i].Text;

                    l = l.Replace("\t", new string(' ', this.TabSize));
                    if (l.Length > max)
                        max = l.Length;
                }

                int pixels = max * _maxCharWidth;
                int chars = pixels / CharWidth;


                if (this.hScroll.Value <= chars)
                    this.hScroll.Maximum = chars;
            }
            catch
            {
                this.hScroll.Maximum = 1000;
            }

            if (hScroll.Maximum > hScroll.LargeChange)
                hScroll.Show();
            else
                hScroll.Hide();
        }

        public void InitScrollbars()
        {
            if (Document.VisibleRows.Count > 0)
            {
                vScroll.Maximum = Document.VisibleRows.Count + 1; //+this.View.VisibleRowCount-2;// - View.VisibleRowCount  ;
                vScroll.LargeChange = this.View.VisibleRowCount;
                SetMaxHorizontalScroll();
            }
            else
                vScroll.Maximum = 1;

            if (vScroll.Maximum > vScroll.LargeChange)
                vScroll.Show();
            else
                vScroll.Hide();
        }


        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditViewControl));
            this._filler = new System.Windows.Forms.PictureBox();
            this._caretTimer = new AIMS.Libraries.CodeEditor.Core.Timers.WeakTimer(this.components);
            this._tooltip = new System.Windows.Forms.ToolTip(this.components);
            this._intelliMouse = new AIMS.Libraries.CodeEditor.WinForms.IntelliMouse.IntelliMouseControl();
            this._panel1 = new System.Windows.Forms.Panel();
            this._contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this._filler)).BeginInit();
            this._contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LeftThumb
            // 
            this.LeftThumb.Location = new System.Drawing.Point(0, 197);
            // 
            // TopThumb
            // 
            this.TopThumb.Location = new System.Drawing.Point(221, 0);
            // 
            // hScroll
            // 
            this.hScroll.Cursor = System.Windows.Forms.Cursors.Default;
            this.hScroll.Location = new System.Drawing.Point(8, 197);
            this.hScroll.Size = new System.Drawing.Size(213, 17);
            this.hScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScroll_Scroll);
            // 
            // vScroll
            // 
            this.vScroll.Cursor = System.Windows.Forms.Cursors.Default;
            this.vScroll.Location = new System.Drawing.Point(221, 8);
            this.vScroll.Size = new System.Drawing.Size(17, 189);
            this.vScroll.Visible = false;
            this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScroll_Scroll);
            // 
            // _filler
            // 
            this._filler.Location = new System.Drawing.Point(0, 0);
            this._filler.Name = "_filler";
            this._filler.Size = new System.Drawing.Size(100, 50);
            this._filler.TabIndex = 0;
            this._filler.TabStop = false;
            // 
            // _caretTimer
            // 
            this._caretTimer.Enabled = true;
            this._caretTimer.Interval = 500;
            this._caretTimer.Tick += new System.EventHandler(this.CaretTimer_Tick);
            // 
            // _tooltip
            // 
            this._tooltip.AutoPopDelay = 50000;
            this._tooltip.InitialDelay = 0;
            this._tooltip.ReshowDelay = 1000;
            this._tooltip.ShowAlways = true;
            // 
            // _intelliMouse
            // 
            this._intelliMouse.Image = ((System.Drawing.Bitmap)(resources.GetObject("_intelliMouse.Image")));
            this._intelliMouse.Location = new System.Drawing.Point(197, 157);
            this._intelliMouse.Name = "_intelliMouse";
            this._intelliMouse.Size = new System.Drawing.Size(28, 28);
            this._intelliMouse.TabIndex = 4;
            this._intelliMouse.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this._intelliMouse.Visible = false;
            this._intelliMouse.BeginScroll += new System.EventHandler(this.IntelliMouse_BeginScroll);
            this._intelliMouse.EndScroll += new System.EventHandler(this.IntelliMouse_EndScroll);
            this._intelliMouse.Scroll += new AIMS.Libraries.CodeEditor.WinForms.IntelliMouse.ScrollEventHandler(this.IntelliMouse_Scroll);
            // 
            // _panel1
            // 
            this._panel1.Location = new System.Drawing.Point(0, 0);
            this._panel1.Name = "_panel1";
            this._panel1.Size = new System.Drawing.Size(200, 100);
            this._panel1.TabIndex = 0;
            this._panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // _contextMenuStrip1
            // 
            this._contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItem1,
            this._toolStripSeparator1,
            this._toolStripMenuItem2,
            this._toolStripSeparator2,
            this._toolStripMenuItem3,
            this._toolStripMenuItem4,
            this._toolStripSeparator3,
            this._toolStripMenuItem5,
            this._toolStripMenuItem6,
            this._toolStripMenuItem7});
            this._contextMenuStrip1.Name = "contextMenuStrip1";
            this._contextMenuStrip1.Size = new System.Drawing.Size(192, 176);
            this._contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // _toolStripMenuItem1
            // 
            this._toolStripMenuItem1.Name = "_toolStripMenuItem1";
            this._toolStripMenuItem1.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem1.Text = "Rename";
            // 
            // _toolStripSeparator1
            // 
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            this._toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // _toolStripMenuItem2
            // 
            this._toolStripMenuItem2.Name = "_toolStripMenuItem2";
            this._toolStripMenuItem2.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem2.Text = "Format";
            // 
            // _toolStripSeparator2
            // 
            this._toolStripSeparator2.Name = "_toolStripSeparator2";
            this._toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // _toolStripMenuItem3
            // 
            this._toolStripMenuItem3.Name = "_toolStripMenuItem3";
            this._toolStripMenuItem3.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem3.Text = "Go to Definition";
            // 
            // _toolStripMenuItem4
            // 
            this._toolStripMenuItem4.Name = "_toolStripMenuItem4";
            this._toolStripMenuItem4.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem4.Text = "Go to Implementation";
            // 
            // _toolStripSeparator3
            // 
            this._toolStripSeparator3.Name = "_toolStripSeparator3";
            this._toolStripSeparator3.Size = new System.Drawing.Size(188, 6);
            // 
            // _toolStripMenuItem5
            // 
            this._toolStripMenuItem5.Name = "_toolStripMenuItem5";
            this._toolStripMenuItem5.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem5.Text = "Copy";
            // 
            // _toolStripMenuItem6
            // 
            this._toolStripMenuItem6.Name = "_toolStripMenuItem6";
            this._toolStripMenuItem6.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem6.Text = "Paste";
            // 
            // _toolStripMenuItem7
            // 
            this._toolStripMenuItem7.Name = "_toolStripMenuItem7";
            this._toolStripMenuItem7.Size = new System.Drawing.Size(191, 22);
            this._toolStripMenuItem7.Text = "Delete";
            // 
            // EditViewControl
            // 
            this.Controls.Add(this._intelliMouse);
            this.LeftThumbVisible = true;
            this.Size = new System.Drawing.Size(240, 216);
            this.TopThumbVisible = true;
            this.GotFocus += new System.EventHandler(this.EditViewControl_Enter);
            this.LostFocus += new System.EventHandler(this.EditViewControl_Leave);
            this.Controls.SetChildIndex(this._intelliMouse, 0);
            this.Controls.SetChildIndex(this.hScroll, 0);
            this.Controls.SetChildIndex(this.vScroll, 0);
            this.Controls.SetChildIndex(this.TopThumb, 0);
            this.Controls.SetChildIndex(this.LeftThumb, 0);
            ((System.ComponentModel.ISupportInitialize)(this._filler)).EndInit();
            this._contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        public void InsertAutolistText()
        {
            TextRange tr = new TextRange();
            tr.FirstRow = Caret.Position.Y;
            tr.LastRow = Caret.Position.Y;

            tr.FirstColumn = Math.Min(_AutoListStartPos.X, _startOffset.X);
            tr.LastColumn = Math.Max(Caret.Position.X, Caret.CurrentWord.Column + Caret.CurrentWord.Text.Length);
            //this.Selection.Bounds = tr;
            Document.DeleteRange(tr, true);
            Caret.Position.X = _AutoListStartPos.X;
            ICompletionData i = AutoList.SelectedListItem.CodeCompletionData;
            if (i == null)
                this.InsertText(AutoList.SelectedText);
            else
                i.InsertAction(this, '\0');
            //this.InsertText(AutoList.SelectedText);

            SetFocus();
        }


        private void MoveCaretToNextWord(bool Select)
        {
            int x = Caret.Position.X;
            int y = Caret.Position.Y;
            string StartChar = "";
            int StartType = 0;
            bool found = false;
            if (x == Caret.CurrentRow.Text.Length)
            {
                StartType = 1;
            }
            else
            {
                StartChar = Document[y].Text.Substring(Caret.Position.X, 1);
                StartType = CharType(StartChar);
            }


            while (y < Document.Count)
            {
                while (x < Document[y].Text.Length)
                {
                    string Char = Document[y].Text.Substring(x, 1);
                    int Type = CharType(Char);
                    if (Type != StartType)
                    {
                        if (Type == 1)
                        {
                            StartType = 1;
                        }
                        else
                        {
                            found = true;
                            break;
                        }
                    }
                    x++;
                }
                if (found)
                    break;
                x = 0;
                y++;
            }

            if (y >= this.Document.Count - 1)
            {
                y = this.Document.Count - 1;

                if (x >= this.Document[y].Text.Length)
                    x = this.Document[y].Text.Length - 1;

                if (x == -1)
                    x = 0;
            }

            Caret.SetPos(new TextPoint(x, y));
            if (!Select)
                Selection.ClearSelection();
            if (Select)
            {
                Selection.MakeSelection();
            }


            ScrollIntoView();
        }

        public void InitGraphics()
        {
            this.Painter.InitGraphics();
        }


        private void MoveCaretToPrevWord(bool Select)
        {
            int x = Caret.Position.X;
            int y = Caret.Position.Y;
            string StartChar = "";
            int StartType = 0;
            bool found = false;
            if (x == Caret.CurrentRow.Text.Length)
            {
                StartType = 1;
                x = Caret.CurrentRow.Text.Length - 1;
            }
            else
            {
                StartChar = Document[y].Text.Substring(Caret.Position.X, 1);
                StartType = CharType(StartChar);
            }


            while (y >= 0)
            {
                while (x >= 0 && x < Document[y].Text.Length)
                {
                    string Char = Document[y].Text.Substring(x, 1);
                    int Type = CharType(Char);
                    if (Type != StartType)
                    {
                        found = true;

                        string Char2 = Document[y].Text.Substring(x, 1);
                        int Type2 = Type;
                        while (x > 0)
                        {
                            Char2 = Document[y].Text.Substring(x, 1);
                            Type2 = CharType(Char2);
                            if (Type2 != Type)
                            {
                                x++;
                                break;
                            }

                            x--;
                        }

                        break;
                    }
                    x--;
                }
                if (found)
                    break;

                if (y == 0)
                {
                    x = 0;
                    break;
                }
                y--;
                x = Document[y].Text.Length - 1;
            }


            Caret.SetPos(new TextPoint(x, y));
            if (!Select)
                Selection.ClearSelection();

            if (Select)
            {
                Selection.MakeSelection();
            }

            ScrollIntoView();
        }


        private void SetFocus()
        {
            this.Focus();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays the GotoLine dialog.
        /// </summary>
        public void ShowGotoLine()
        {
            GotoLineForm go = new GotoLineForm(this, this.Document.Count);
            //			if (this.TopLevelControl is Form)
            //				go.Owner=(Form)this.TopLevelControl;

            go.ShowDialog(this.TopLevelControl);
        }

        /// <summary>
        /// -
        /// </summary>
        public void ShowSettings()
        {
            //SettingsForm se = new SettingsForm(this);
            //se.ShowDialog();
        }

        /// <summary>
        /// Places the caret on a specified line and scrolls the caret into view.
        /// </summary>
        /// <param name="RowIndex">the zero based index of the line to jump to</param>
        public void GotoLine(int RowIndex)
        {
            if (RowIndex >= this.Document.Count)
                RowIndex = this.Document.Count - 1;

            if (RowIndex < 0)
                RowIndex = 0;

            this.Caret.Position.Y = RowIndex;
            this.Caret.Position.X = 0;
            this.Caret.CurrentRow.EnsureVisible();
            ClearSelection();
            ScrollIntoView();
            this.Redraw();
        }


        /// <summary>
        /// Clears the active selection.
        /// </summary>
        public void ClearSelection()
        {
            this.Selection.ClearSelection();
            this.Redraw();
        }

        /// <summary>
        /// Returns if a specified pixel position is over the current selection.
        /// </summary>
        /// <param name="x">X Position in pixels</param>
        /// <param name="y">Y Position in pixels</param>
        /// <returns>true if over selection othewise false</returns>
        public bool IsOverSelection(int x, int y)
        {
            TextPoint p = Painter.CharFromPixel(x, y);

            if (p.Y >= this.Selection.LogicalBounds.FirstRow && p.Y <= this.Selection.LogicalBounds.LastRow && this.Selection.IsValid)
            {
                if (p.Y > this.Selection.LogicalBounds.FirstRow && p.Y < this.Selection.LogicalBounds.LastRow && this.Selection.IsValid)
                    return true;
                else
                {
                    if (p.Y == this.Selection.LogicalBounds.FirstRow && this.Selection.LogicalBounds.FirstRow == this.Selection.LogicalBounds.LastRow)
                    {
                        if (p.X >= this.Selection.LogicalBounds.FirstColumn && p.X <= this.Selection.LogicalBounds.LastColumn)
                            return true;
                        else
                            return false;
                    }
                    else if (p.X >= this.Selection.LogicalBounds.FirstColumn && p.Y == this.Selection.LogicalBounds.FirstRow)
                        return true;
                    else if (p.X <= this.Selection.LogicalBounds.LastColumn && p.Y == this.Selection.LogicalBounds.LastRow)
                        return true;
                    else
                        return false;
                }
            }
            else
                return false; //no chance we are over Selection.LogicalBounds
        }

        /// <summary>
        /// Scrolls a given position in the text into view.
        /// </summary>
        /// <param name="Pos">Position in text</param>
        public void ScrollIntoView(TextPoint Pos)
        {
            TextPoint tmp = this.Caret.Position;
            this.Caret.Position = Pos;
            if (this.Caret.CurrentRow != null) this.Caret.CurrentRow.EnsureVisible();
            this.ScrollIntoView();
            this.Caret.Position = tmp;
            this.Invalidate();
        }

        public void ScrollIntoView(int RowIndex)
        {
            Row r = Document[RowIndex];
            if (r == null)
                return;
            r.EnsureVisible();

            this.vScroll.Value = r.VisibleIndex;
            this.Invalidate();
        }

        /// <summary>
        /// Scrolls the caret into view.
        /// </summary>
        public void ScrollIntoView()
        {
            InitScrollbars();

            Caret.CropPosition();
            try
            {
                Row xtr2 = Caret.CurrentRow;
                if (xtr2.VisibleIndex >= View.FirstVisibleRow + View.VisibleRowCount - 2)
                {
                    int Diff = Caret.CurrentRow.VisibleIndex - (View.FirstVisibleRow + View.VisibleRowCount - 2) + View.FirstVisibleRow;
                    if (Diff > Document.VisibleRows.Count - 1)
                        Diff = Document.VisibleRows.Count - 1;

                    Row r = Document.VisibleRows[Diff];
                    int index = r.VisibleIndex;
                    if (index != -1)
                        vScroll.Value = index;
                }
            }
            catch
            {
            }


            if (Caret.CurrentRow.VisibleIndex < View.FirstVisibleRow)
            {
                Row r = Caret.CurrentRow;
                int index = r.VisibleIndex;
                if (index != -1)
                    vScroll.Value = index;
            }

            Row xtr = Caret.CurrentRow;


            int x = 0;
            if (Caret.CurrentRow.IsCollapsedEndPart)
            {
                x = Painter.MeasureRow(xtr, Caret.Position.X).Width + Caret.CurrentRow.Expansion_PixelStart;
                x -= Painter.MeasureRow(xtr, xtr.Expansion_StartChar).Width;

                if (x >= View.ClientAreaWidth + View.ClientAreaStart)
                    hScroll.Value = Math.Min(hScroll.Maximum, ((x - View.ClientAreaWidth) / View.CharWidth) + 15);

                if (x < View.ClientAreaStart + 10)
                    hScroll.Value = Math.Max(hScroll.Minimum, ((x) / View.CharWidth) - 15);
            }
            else
            {
                x = Painter.MeasureRow(xtr, Caret.Position.X).Width;

                if (x >= View.ClientAreaWidth + View.ClientAreaStart)
                    hScroll.Value = Math.Min(hScroll.Maximum, ((x - View.ClientAreaWidth) / View.CharWidth) + 15);

                if (x < View.ClientAreaStart)
                    hScroll.Value = Math.Max(hScroll.Minimum, ((x) / View.CharWidth) - 15);
            }
        }

        /// <summary>
        /// Moves the caret to the next line that has a bookmark.
        /// </summary>
        public void GotoNextBookmark()
        {
            int index = 0;
            index = Document.GetNextBookmark(Caret.Position.Y);
            Caret.SetPos(new TextPoint(0, index));
            ScrollIntoView();
            this.Redraw();
        }


        /// <summary>
        /// Moves the caret to the previous line that has a bookmark.
        /// </summary>
        public void GotoPreviousBookmark()
        {
            int index = 0;
            index = Document.GetPreviousBookmark(Caret.Position.Y);
            Caret.SetPos(new TextPoint(0, index));
            ScrollIntoView();
            this.Redraw();
        }

        /// <summary>
        /// Selects next occurance of the given pattern.
        /// </summary>
        /// <param name="Pattern">Pattern to find</param>
        /// <param name="MatchCase">Case sensitive</param>
        /// <param name="WholeWords">Match whole words only</param>
        public bool SelectNext(string Pattern, bool MatchCase, bool WholeWords, bool UseRegEx)
        {
            string pattern = Pattern;
            int StartCol = this.Caret.Position.X;
            int StartRow = this.Caret.Position.Y;

            for (int i = this.Caret.Position.Y; i < this.Document.Count; i++)
            {
                Row r = Document[i];

                if (UseRegEx)
                {
                    Regex regex = new Regex(Pattern);
                    int MatchCol = StartCol;
                    if (i == StartRow)
                    {
                        if (StartCol < r.Text.Length)
                            continue;
                    }
                    else
                        MatchCol = 0;

                    Match match = regex.Match(r.Text, MatchCol);
                    if (match.Success)
                    {
                        SelectPattern(i, match.Index, match.Length);
                        return true;
                    }
                }
                else
                {
                    int Col = -1;
                    string s = "";
                    string t = r.Text;
                    if (WholeWords)
                    {
                        s = " " + r.Text + " ";
                        t = "";
                        pattern = " " + Pattern + " ";
                        foreach (char c in s)
                        {
                            if (".,+-*^\\/()[]{}@:;'?\u00A3$#%& \t=<>".IndexOf(c) >= 0)
                                t += " ";
                            else
                                t += c;
                        }
                    }

                    if (!MatchCase)
                    {
                        t = t.ToLower();
                        pattern = pattern.ToLower();
                    }

                    Col = t.IndexOf(pattern);

                    if ((Col >= StartCol) || (i > StartRow && Col >= 0))
                    {
                        SelectPattern(i, Col, Pattern.Length);
                        return true;
                    }
                }
            }
            return false;
        }

        //public bool ReplaceSelection(string text)
        //{
        //    if (!Selection.IsValid)
        //        return false;

        //    int x = Selection.LogicalBounds.FirstColumn;
        //    int y = Selection.LogicalBounds.FirstRow;

        //    this.Selection.DeleteSelection();

        //    Caret.Position.X = x;
        //    Caret.Position.Y = y;

        //    InsertText(text);


        //    Selection.Bounds.FirstRow = y;
        //    Selection.Bounds.FirstColumn = x + text.Length;

        //    Selection.Bounds.LastRow = y;
        //    Selection.Bounds.LastColumn = x + text.Length;

        //    Caret.Position.X = x + text.Length;
        //    Caret.Position.Y = y;
        //    return true;
        //}

        public bool ReplaceSelection(string text)
        {
            if (!Selection.IsValid)
                return false;

            int x = Selection.LogicalBounds.FirstColumn;
            int y = Selection.LogicalBounds.FirstRow;

            TextRange t = new TextRange(Selection.Bounds.FirstColumn, Selection.Bounds.FirstRow, Selection.Bounds.LastColumn, Selection.Bounds.LastRow);


            TextRange newRange = this.Document.ReplaceRange(t, text, true);
            Selection.Bounds = newRange;
            Caret.Position.X = newRange.LastColumn;
            Caret.Position.Y = newRange.LastRow;
            return true;
        }

        /// <summary>
        /// Toggles a bookmark on/off on the active row.
        /// </summary>
        public void ToggleBookmark()
        {
            Document[Caret.Position.Y].Bookmarked = !Document[Caret.Position.Y].Bookmarked;
            this.Redraw();
        }


        /// <summary>
        /// Deletes selected text if possible otherwise deletes forward. (delete key)
        /// </summary>
        public void Delete()
        {
            DeleteForward();
            this.Refresh();
        }

        /// <summary>
        /// Selects all text in the active document. (control + a)
        /// </summary>
        public void SelectAll()
        {
            Selection.SelectAll();
            this.Redraw();
        }


        /// <summary>
        /// Paste text from clipboard to current caret position. (control + v)
        /// </summary>
        public void Paste()
        {
            PasteText();
            this.Refresh();
        }

        /// <summary>
        /// Copies selected text to clipboard. (control + c)
        /// </summary>
        public void Copy()
        {
            CopyText();
        }

        /// <summary>
        /// Cuts selected text to clipboard. (control + x)
        /// </summary>
        public void Cut()
        {
            CopyText();
            Selection.DeleteSelection();
        }

        /// <summary>
        /// Removes the current row
        /// </summary>
        public void RemoveCurrentRow()
        {
            if (Caret.CurrentRow != null && this.Document.Count > 1)
            {
                this.Document.Remove(Caret.CurrentRow.Index, true);
                this.Document.ResetVisibleRows();
                this.Caret.CropPosition();
                this.Caret.CurrentRow.Text = this.Caret.CurrentRow.Text;
                this.Caret.CurrentRow.Parse(true);
                this.Document.ResetVisibleRows();
                this.ScrollIntoView();
                //this.Refresh ();

            }
        }

        public void CutClear()
        {
            if (Selection.IsValid)
                Cut();
            else
                RemoveCurrentRow();
        }

        /// <summary>
        /// Redo last undo action. (control + y)
        /// </summary>
        public void Redo()
        {
            TextPoint p = this.Document.Redo();
            if (p.X != -1 && p.Y != -1)
            {
                Caret.Position = p;
                Selection.ClearSelection();
                ScrollIntoView();
            }
        }

        /// <summary>
        /// Undo last edit action. (control + z)
        /// </summary>
        public void Undo()
        {
            TextPoint p = this.Document.Undo();
            if (p.X != -1 && p.Y != -1)
            {
                Caret.Position = p;
                Selection.ClearSelection();
                ScrollIntoView();
            }
        }

        /// <summary>
        /// Returns a point where x is the column and y is the row from a given pixel position.
        /// </summary>
        /// <param name="x">X Position in pixels</param>
        /// <param name="y">Y Position in pixels</param>
        /// <returns>Column and Rowindex</returns>
        public TextPoint CharFromPixel(int x, int y)
        {
            return this.Painter.CharFromPixel(x, y);
        }

        public void ShowFind()
        {
            if (FindReplaceDialog != null)
            {
                FindReplaceDialog.TopLevel = true;
                if (this.TopLevelControl is Form)
                {
                    FindReplaceDialog.Owner = (Form)this.TopLevelControl;
                }
                FindReplaceDialog.ShowFind();
            }
        }

        public void ShowReplace()
        {
            if (FindReplaceDialog != null)
            {
                FindReplaceDialog.TopLevel = true;
                if (this.TopLevelControl is Form)
                {
                    FindReplaceDialog.Owner = (Form)this.TopLevelControl;
                }
                FindReplaceDialog.ShowReplace();
            }
        }

        public void AutoListBeginLoad()
        {
            this.AutoList.BeginLoad();
        }

        public void AutoListEndLoad()
        {
            this.AutoList.EndLoad();
        }

        public void FindNext()
        {
            this.FindReplaceDialog.FindNext();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if the control is in overwrite mode.
        /// </summary>
        [Browsable(false)]
        public bool OverWrite
        {
            get { return _OverWrite; }
        }

        /// <summary>
        /// Returns True if the control contains a selected text.
        /// </summary>
        [Browsable(false)]
        public bool CanCopy
        {
            get { return this.Selection.IsValid; }
        }

        /// <summary>
        /// Returns true if there is any valid text data inside the Clipboard.
        /// </summary>
        [Browsable(false)]
        public bool CanPaste
        {
            get
            {
                string s = "";
                //try
                //{
                //    IDataObject iData = Clipboard.GetDataObject();

                //    if (iData.GetDataPresent(DataFormats.Text))
                //    {
                //        // Yes it is, so display it in a text box.
                //        s = (String)iData.GetData(DataFormats.Text);
                //    }

                //    if (s != null)
                //        return true;
                //}
                //catch
                //{
                //}
                return false;
            }
        }

        /// <summary>
        /// Returns true if the undobuffer contains one or more undo actions.
        /// </summary>
        [Browsable(false)]
        public bool CanUndo
        {
            get { return (this.Document.UndoStep > 0); }
        }

        /// <summary>
        /// Returns true if the control can redo the last undo action/s
        /// </summary>
        [Browsable(false)]
        public bool CanRedo
        {
            get
            {
                if (this.Document.UndoStep >= this.Document.UndoBuffer.Count)
                    return false;
                else
                    return true;
            }
        }


        /// <summary>
        /// Gets the size (in pixels) of the font to use when rendering the the content.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public float FontSize
        {
            get { return _CodeEditor.FontSize; }
        }


        /// <summary>
        /// Gets the indention style to use when inserting new lines.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public IndentStyle Indent
        {
            get { return _CodeEditor.Indent; }
        }

        /// <summary>
        /// Gets the SyntaxDocument the control is currently attatched to.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        [Category("Content")]
        [Description("The SyntaxDocument that is attatched to the control")]
        public SyntaxDocument Document
        {
            get
            {
                return _CodeEditor.Document;
            }
        }

        /// <summary>
        /// Gets the delay in MS before the tooltip is displayed when hovering a collapsed section.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int TooltipDelay
        {
            get
            {
                return _CodeEditor.TooltipDelay;
            }
        }


        /// <summary>
        /// Gets if the control is readonly.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ReadOnly
        {
            get { return _CodeEditor.ReadOnly; }
        }


        /// <summary>
        /// Gets the name of the font to use when rendering the control.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public string FontName
        {
            get { return _CodeEditor.FontName; }
        }

        /// <summary>
        /// Determines the style to use when painting with alt+arrow keys.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines what type of chars to use when painting with ALT+arrow keys")]
        public TextDrawType TextDrawStyle
        {
            get { return _TextDrawStyle; }
            set { _TextDrawStyle = value; }
        }

        /// <summary>
        /// Gets if the control should render bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool BracketMatching
        {
            get
            {
                return _CodeEditor.BracketMatching;
            }
        }


        /// <summary>
        /// Gets if the control should render whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool VirtualWhitespace
        {
            get { return _CodeEditor.VirtualWhitespace; }
        }


        /// <summary>
        /// Gets the Color of the horizontal separators (a'la visual basic 6).
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SeparatorColor
        {
            get { return _CodeEditor.SeparatorColor; }
        }


        /// <summary>
        /// Gets the text color to use when rendering bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BracketForeColor
        {
            get { return _CodeEditor.BracketForeColor; }
        }


        /// <summary>
        /// Gets the back color to use when rendering bracket matching.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BracketBackColor
        {
            get { return _CodeEditor.BracketBackColor; }
        }


        /// <summary>
        /// Gets the back color to use when rendering the selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SelectionBackColor
        {
            get { return _CodeEditor.SelectionBackColor; }
        }

        public void SetSelectionBackColor(Color c)
        {
            _CodeEditor.SelectionBackColor = c;
        }

        /// <summary>
        /// Gets the text color to use when rendering the selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color SelectionForeColor
        {
            get { return _CodeEditor.SelectionForeColor; }
        }

        /// <summary>
        /// Gets the back color to use when rendering the inactive selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color InactiveSelectionBackColor
        {
            get { return _CodeEditor.InactiveSelectionBackColor; }
        }


        /// <summary>
        /// Gets the text color to use when rendering the inactive selected text.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color InactiveSelectionForeColor
        {
            get { return _CodeEditor.InactiveSelectionForeColor; }
        }


        /// <summary>
        /// Gets the color of the border between the gutter area and the line number area.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color GutterMarginBorderColor
        {
            get { return _CodeEditor.GutterMarginBorderColor; }
        }


        /// <summary>
        /// Gets the color of the border between the line number area and the folding area.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color LineNumberBorderColor
        {
            get { return _CodeEditor.LineNumberBorderColor; }
        }


        /// <summary>
        /// Gets the text color to use when rendering breakpoints.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BreakPointForeColor
        {
            get { return _CodeEditor.BreakPointForeColor; }
        }

        /// <summary>
        /// Gets the back color to use when rendering breakpoints.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color BreakPointBackColor
        {
            get { return _CodeEditor.BreakPointBackColor; }
        }

        /// <summary>
        /// Gets the text color to use when rendering line numbers.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color LineNumberForeColor
        {
            get { return _CodeEditor.LineNumberForeColor; }
        }

        /// <summary>
        /// Gets the back color to use when rendering line number area.
        /// </summary>
        public Color LineNumberBackColor
        {
            get { return _CodeEditor.LineNumberBackColor; }
        }

        /// <summary>
        /// Gets the color of the gutter margin.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color GutterMarginColor
        {
            get { return _CodeEditor.GutterMarginColor; }
        }

        /// <summary>
        /// Gets or Sets the background Color of the client area.
        /// </summary>
        [Category("Appearance")]
        [Description("The background color of the client area")]
        new public Color BackColor
        {
            get { return _CodeEditor.BackColor; }
            set { _CodeEditor.BackColor = value; }
        }

        /// <summary>
        /// Gets the back color to use when rendering the active line.
        /// </summary>
        public Color HighLightedLineColor
        {
            get { return _CodeEditor.HighLightedLineColor; }
        }

        /// <summary>
        /// Get if the control should highlight the active line.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool HighLightActiveLine
        {
            get { return _CodeEditor.HighLightActiveLine; }
        }

        /// <summary>
        /// Get if the control should render whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowWhitespace
        {
            get { return _CodeEditor.ShowWhitespace; }
        }


        /// <summary>
        /// Get if the line number margin is visible or not.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return _CodeEditor.ShowLineNumbers; }
        }


        /// <summary>
        /// Get if the gutter margin is visible or not.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowGutterMargin
        {
            get { return _CodeEditor.ShowGutterMargin; }
        }

        /// <summary>
        /// Get the Width of the gutter margin (in pixels)
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int GutterMarginWidth
        {
            get { return _CodeEditor.GutterMarginWidth; }
        }


        public void ShowGutter()
        {
            _CodeEditor.GutterMarginWidth = 0;
            _CodeEditor.ShowLineNumbers = false;
        }

        /// <summary>
        /// Get the numbers of space chars in a tab.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int TabSize
        {
            get { return _CodeEditor.TabSize; }
        }

        /// <summary>
        /// Get if the control should render 'Tab guides'
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ShowTabGuides
        {
            get { return _CodeEditor.ShowTabGuides; }
        }

        /// <summary>
        /// Gets the color to use when rendering whitespace chars.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color WhitespaceColor
        {
            get { return _CodeEditor.WhitespaceColor; }
        }

        /// <summary>
        /// Gets the color to use when rendering tab guides.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color TabGuideColor
        {
            get { return _CodeEditor.TabGuideColor; }
        }

        /// <summary>
        /// Get the color to use when rendering bracket matching borders.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        /// <remarks>
        /// NOTE: Use the Color.Transparent turn off the bracket match borders.
        /// </remarks>
        public Color BracketBorderColor
        {
            get { return _CodeEditor.BracketBorderColor; }
        }


        /// <summary>
        /// Get the color to use when rendering Outline symbols.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public Color OutlineColor
        {
            get { return _CodeEditor.OutlineColor; }
        }

        [Browsable(false)]
        public TextPoint AutoListStartPos
        {
            get { return _AutoListStartPos; }
            set { _AutoListStartPos = value; }
        }

        /// <summary>
        /// Positions the AutoList
        /// </summary>
        /// 
        [Category("Behavior")]
        public TextPoint AutoListPosition
        {
            get { return _AutoListPos; }
            set { _AutoListPos = value; }
        }

        /// <summary>
        /// Positions the InfoTip
        /// </summary>
        [Category("Behavior")]
        public TextPoint InfoTipPosition
        {
            get { return InfoTipStartPos; }
            set { InfoTipStartPos = value; }
        }


        /// <summary>
        /// Gets or Sets if the intellisense list is visible.
        /// </summary>
        [Category("Behavior")]
        public bool AutoListVisible
        {
            set
            {
                CreateAutoList();
                if (this.AutoList == null)
                {
                    _AutoListVisible = false;
                    return;
                }

                if (value)
                {
                    AutoList.TopLevel = true;
                    AutoList.BringToFront();
                    InfoTip.BringToFront();
                    if (this.TopLevelControl is Form)
                    {
                        AutoList.Owner = (Form)this.TopLevelControl;
                    }
                    _AutoListVisible = value;
                }
                else
                {
                    AutoList.Visible = value;
                    //AutoList.Close();
                }
                _AutoListVisible = value;
                InfoTip.BringToFront();

                this.Redraw();
            }
            get
            {
                return _AutoListVisible;
            }
        }


        ///// <summary>
        ///// Gets or Sets if the infotip is visible
        ///// </summary>
        //[Category("Behavior")]
        //public bool InfoTipVisible
        //{
        //	set
        //	{
        //		CreateInfoTip();
        //		if (this.InfoTip == null)
        //			return;

        //		if (value)
        //		{
        //			//	InfoTipStartPos=new TextPoint (Caret.Position.X,Caret.Position.Y);
        //			InfoTip.TopLevel = true;
        //			AutoList.BringToFront();
        //			if (this.TopLevelControl is Form)
        //			{
        //				InfoTip.Owner = (Form) this.TopLevelControl;
        //			}
        //		}
        //		//else
        //		//	InfoTipStartPos=new TextPoint (0,0);

        //		InfoTip.BringToFront();

        //		_InfoTipVisible = value;
        //              InfoTip.Visible = value;
        //		if (this.InfoTip != null && value == true)
        //		{
        //			this.InfoTip.Init();
        //		}
        //		this.Redraw();
        //	}
        //	get { return _InfoTipVisible; }
        //}

        /// <summary>
        /// Get if the control should use smooth scrolling.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool SmoothScroll
        {
            get { return _CodeEditor.SmoothScroll; }
        }

        /// <summary>
        /// Get the number of pixels the screen should be scrolled per frame when using smooth scrolling.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public int SmoothScrollSpeed
        {
            get { return _CodeEditor.SmoothScrollSpeed; }
        }


        /// <summary>
        /// Get if the control should parse all text when text is pasted from the clipboard.
        /// The value is retrived from the owning Syntaxbox control.
        /// </summary>
        public bool ParseOnPaste
        {
            get { return _CodeEditor.ParseOnPaste; }
        }


        /// <summary>
        /// Gets the Caret object.
        /// </summary>
        public Caret Caret
        {
            get { return _Caret; }
        }

        /// <summary>
        /// Gets the Selection object.
        /// </summary>
        public Selection Selection
        {
            get { return _Selection; }
        }

        #endregion

        #region eventhandlers

        private void OnClipboardUpdated(CopyEventArgs e)
        {
            if (ClipboardUpdated != null)
                ClipboardUpdated(this, e);
        }


        private void OnRowMouseDown(RowMouseEventArgs e)
        {
            if (RowMouseDown != null)
                RowMouseDown(this, e);
        }

        private void OnRowMouseMove(RowMouseEventArgs e)
        {
            if (RowMouseMove != null)
                RowMouseMove(this, e);
        }

        private void OnRowMouseUp(RowMouseEventArgs e)
        {
            if (RowMouseUp != null)
                RowMouseUp(this, e);
        }

        private void OnRowClick(RowMouseEventArgs e)
        {
            if (RowClick != null)
                RowClick(this, e);
        }

        private void OnRowDoubleClick(RowMouseEventArgs e)
        {
            if (RowDoubleClick != null)
                RowDoubleClick(this, e);
        }


        protected override void OnLoad(EventArgs e)
        {
            this.DoResize();
            this.Refresh();
        }


        public void OnParse()
        {
            this.Redraw();
        }

        public void OnChange()
        {
            if (this.Caret.Position.Y > this.Document.Count - 1)
            {
                this.Caret.Position.Y = this.Document.Count - 1;
                //this.Caret.MoveAbsoluteHome (false);
                ScrollIntoView();
            }

            //try
            //{
            if (this.VirtualWhitespace == false && Caret.CurrentRow != null && Caret.Position.X > Caret.CurrentRow.Text.Length)
            {
                Caret.Position.X = Caret.CurrentRow.Text.Length;
                this.Redraw();
            }
            //}
            //catch
            //{
            //}


            if (!this.ContainsFocus)
            {
                this.Selection.ClearSelection();
            }


            if (this.Selection.LogicalBounds.FirstRow > this.Document.Count)
            {
                this.Selection.Bounds.FirstColumn = Caret.Position.X;
                this.Selection.Bounds.LastColumn = Caret.Position.X;
                this.Selection.Bounds.FirstRow = Caret.Position.Y;
                this.Selection.Bounds.LastRow = Caret.Position.Y;
            }

            if (this.Selection.LogicalBounds.LastRow > this.Document.Count)
            {
                this.Selection.Bounds.FirstColumn = Caret.Position.X;
                this.Selection.Bounds.LastColumn = Caret.Position.X;
                this.Selection.Bounds.FirstRow = Caret.Position.Y;
                this.Selection.Bounds.LastRow = Caret.Position.Y;
            }

            this.Redraw(false);
        }

        /// <summary>
        /// Overrides the default OnKeyDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _KeyDownHandled = e.Handled;

            //if (e.KeyCode == Keys.Space && e.Control)
            //{
            //    InfoTipVisible = true;
            //    AutoListVisible = true;
            //    InfoTip.Visible = true;
            //    e.Handled = true;
            //    _KeyDownHandled=true;


            //}

            //if (e.KeyCode == Keys.Escape && (InsightWindowVisible || AutoListVisible))
            //{
            //    CloseInsightWindow(null, null);
            //    AutoListVisible = false;
            //    e.Handled = true;
            //    this.Redraw();
            //    return;
            //}

            //if (!e.Handled && InsightWindowVisible )
            //{
            //   e.Handled= insightWindow.ProcessTextAreaKey(e.KeyData);
            //    insightWindow.SetLocation();

            //}
            //if (AutoListVisible)
            //{
            //    if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown))
            //    {
            //        AutoList.SendKey((int)e.KeyCode);
            //        e.Handled = true;
            //        return;
            //    }

            //    if (((e.KeyCode == Keys.Space) && ! (e.Control || e.Alt || e.Shift)) || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            //    {
            //        string s = AutoList.SelectedText;
            //        if (s != "")
            //            InsertAutolistText();
            //        AutoListVisible = false;
            //        e.Handled = true;
            //        if (e.KeyCode == Keys.Space)
            //            e.SuppressKeyPress = true;
            //        this.Redraw();
            //        return;
            //    }


            //}

            //if (AutoListVisible)
            //{
            //    if (e.Handled == false)
            //    {
            //        AutoList.SendKey((int)e.KeyCode);
            //        e.Handled = true;

            //        return;
            //    }
            //}


            //if (!e.Handled && AutoListVisible)
            //{
            //    //move autolist selection


            if (e.KeyCode == Keys.Escape)
            {
                CodeEditor.HideIntellisense();
            }

            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown))
            {
                if (CodeEditor.SendKeys((int)e.KeyCode) == true)
                {
                    e.Handled = true;
                    this.Redraw(false);
                    return;
                }

                //                ShadowedWords();
            }

            //    //inject inser text from autolist
            //    if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            //    {
            //        string s = AutoList.SelectedText;
            //        if (s != "")
            //            InsertAutolistText();
            //        AutoListVisible = false;
            //        e.Handled = true;
            //        this.Redraw();
            //        return;
            //    }
            //}


            if (!e.Handled)
            {
                //do keyboard actions
                foreach (KeyboardAction ka in this._CodeEditor.KeyboardActions)
                {
                    if (!this.ReadOnly || ka.AllowReadOnly)
                    {
                        if ((ka.Key == (Keys)(int)e.KeyCode)
                            && ka.Alt == e.Alt
                            && ka.Shift == e.Shift
                            && ka.Control == e.Control)
                            ka.Action(); //if keys match , call action delegate
                    }
                }


                //------------------------------------------------------------------------------------------------------------

                if (e.Control || e.Alt)
                    return;

                switch ((Keys)(int)e.KeyCode)
                {
                    case Keys.ShiftKey:
                    case Keys.ControlKey:
                    case Keys.Alt:
                        return;

                    case Keys.Down:
                        if (e.Control)
                            ScrollScreen(1);
                        else
                        {
                            if (e.Alt)
                                TextDraw(TextDrawDirectionType.Down);
                            Caret.MoveDown(e.Shift);
                            this.Redraw(false);
                        }
                        break;
                    case Keys.Up:
                        if (e.Control)
                            ScrollScreen(-1);
                        else
                        {
                            if (e.Alt)
                                TextDraw(TextDrawDirectionType.Up);
                            Caret.MoveUp(e.Shift);
                        }
                        this.Redraw(false);
                        break;
                    case Keys.Left:
                        {
                            if (e.Control)
                            {
                                MoveCaretToPrevWord(e.Shift);
                            }
                            else
                            {
                                if (e.Alt)
                                    TextDraw(TextDrawDirectionType.Left);
                                Caret.MoveLeft(e.Shift);
                            }
                        }


                        //Painter.RenderCaret(this.CreateGraphics());

                        this.Redraw(false);
                        break;
                    case Keys.Right:
                        {
                            if (e.Control)
                            {
                                MoveCaretToNextWord(e.Shift);
                            }
                            else
                            {
                                if (e.Alt)
                                    TextDraw(TextDrawDirectionType.Right);

                                Caret.MoveRight(e.Shift);
                            }
                        }
                        //Painter.RenderCaret(this.CreateGraphics());
                        this.Redraw(false);
                        break;
                    case Keys.End:
                        if (e.Control)
                            Caret.MoveAbsoluteEnd(e.Shift);
                        else
                            Caret.MoveEnd(e.Shift);
                        this.Redraw(false);
                        break;
                    case Keys.Home:
                        if (e.Control)
                            Caret.MoveAbsoluteHome(e.Shift);
                        else
                            Caret.MoveHome(e.Shift);
                        this.Redraw(false);
                        break;
                    case Keys.PageDown:
                        Caret.MoveDown(View.VisibleRowCount - 2, e.Shift);
                        this.Redraw(false);
                        break;
                    case Keys.PageUp:
                        Caret.MoveUp(View.VisibleRowCount - 2, e.Shift);
                        this.Redraw(false);
                        break;

                    default:
                        break;
                }


                //dont do if readonly
                if (!this.ReadOnly)
                {
                    switch ((Keys)(int)e.KeyCode)
                    {
                        case Keys.Enter:
                            {
                                if (e.Control)
                                {
                                    if (Caret.CurrentRow.CanFold)
                                    {
                                        Caret.MoveHome(false);
                                        Document.ToggleRow(Caret.CurrentRow);
                                        this.Redraw();
                                    }
                                }
                                else
                                {
                                    if (CodeEditor.EnterHandled(e) == false)
                                        InsertEnter();
                                }
                                break;
                            }
                        case Keys.Back:
                            if (!e.Control)
                                this.DeleteBackwards();
                            else
                            {
                                if (this.Selection.IsValid)
                                    this.Selection.DeleteSelection();
                                else
                                {
                                    this.Selection.ClearSelection();
                                    MoveCaretToPrevWord(true);
                                    this.Selection.DeleteSelection();
                                }
                                this.Caret.CurrentRow.Parse(true);
                            }

                            break;
                        case Keys.Delete:
                            {
                                if (!e.Control && !e.Alt && !e.Shift)
                                {
                                    this.Delete();
                                }
                                else if (e.Control && !e.Alt && !e.Shift)
                                {
                                    if (this.Selection.IsValid)
                                    {
                                        Cut();
                                    }
                                    else
                                    {
                                        this.Selection.ClearSelection();
                                        MoveCaretToNextWord(true);
                                        this.Selection.DeleteSelection();
                                    }
                                    this.Caret.CurrentRow.Parse(true);
                                }
                                break;
                            }
                        case Keys.Insert:
                            {
                                if (!e.Control && !e.Alt && !e.Shift)
                                {
                                    _OverWrite = !_OverWrite;
                                }
                                break;
                            }
                        case Keys.Tab:
                            {
                                if (!Selection.IsValid)
                                    InsertText("\t");

                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

                //if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
                //{
                //    //updateTimer = new System.Threading.Timer(ShadowsUpdate, null, 1000, -1);

                //}
                //if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Right))
                //{
                //    // string word = Selection.GetCaretWord();

                //    // if (word != shadowed)

                //    //  updateTimer = new System.Threading.Timer(ShadowsUpdates, null, 1000, -1);

                //    //  else updateTimer = null;

                //}
                Caret.Blink = true;
                //this.Redraw ();
            }
        }

        private string word { get; set; }

        public void ShadowsUpdate(object state)
        {
            //if (shadowed == Selection.GetCaretWord())
            ShadowedWords();
            if (_haswords)
                this.Invoke(new Action(() => { Refresh(); }));
        }
        public void ShadowsUpdates(object state)
        {
            if (shadowed == Selection.GetCaretWord())
                return;
            ShadowedWords();
            if (_haswords)
                this.Invoke(new Action(() => { Refresh(); }));
        }


        private System.Threading.Timer updateTimer { get; set; }

        public bool textchanged = false;


        /// <summary>
        /// Overrides the default OnKeyPress
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);


            if (!e.Handled && !_KeyDownHandled && e.KeyChar != (char)127 && (int)e.KeyChar != 8)
            {
                if (((int)e.KeyChar) < 32)
                    return;

                if (!this.ReadOnly)
                {
                    switch ((Keys)(int)e.KeyChar)
                    {
                        default:
                            {
                                InsertText(e.KeyChar.ToString());

                                textchanged = true;

                                if (this.Indent == IndentStyle.Scope || this.Indent == IndentStyle.Smart)
                                {
                                    if (Caret.CurrentRow.ShouldOutdent)
                                    {
                                        OutdentEndRow();
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            //else 
            //if (AutoListVisible && !e.Handled && _CodeEditor.AutoListAutoSelect)
            {
                // CodeEditor.FindWord();

                // await this.CodeEditor.FindWord();

                FindWords w = new FindWords();
                w.dd = this.CodeEditor;
                w.Runs();


            }
        }

        public class FindWords
        {
            public CodeEditorControl dd { get; set; }

            public void Runs()
            {
                Thread d = new Thread(Run);
                d.IsBackground = true;
                d.Start();
            }


            public void Run()
            {
                //dd.BeginInvoke(new Action(() => { FindWord(); } ));

                FindWord();
            }
            public void FindWord()
            {

                dd.BeginInvoke(new Action(() => {  


                if (dd.csd == null)
                {
                    dd.csd = dd.vp.CSParsers();
                }

                dd._shouldwait++;

                if (CodeEditorControl.ins == null)
                {
                    CodeEditorControl.ins = new PanelIns();
                    CodeEditorControl.ins.cec = dd;

                    CodeEditorControl.ins.LoadTypes(dd.csd);

                    if (CodeEditorControl.form == null)
                    {
                        CodeEditorControl.form = new TopForm();
                        CodeEditorControl.form.Load(CodeEditorControl.ins);
                        CodeEditorControl.ins.form = CodeEditorControl.form;

                    }

                    CodeEditorControl.ins.Hide();
                    CodeEditorControl.form.Hide();

                    dd._ActiveView.Refresh();
                }


                Point p = dd.Selection.GetCursor();


                if (dd.txt != null)
                    if (p.Y + 1 != dd.txt.Line)
                        dd._runnings = false;

                dd.txt = new TextLocation(p.Y + 1, p.X);

                }));

                if (dd._runnings == false)
                {
                   // dd.BeginInvoke(new Action(() => { dd.TypesWithDelay(); }));
                    //TypesWithDelay();
                }


                if( dd._running == false)
                {
                    dd.BeginInvoke(new Action(() => { dd.SendWithDelay(); }));
                    //dd.SendWithDelay();
                }


                return;
            }

            //public bool _runnings = false;

            public async void TypesWithDelay()
            {

                PanelIns ins = null;

                    double db;

                string bb = "";

                int offset = 0;

                VSParsers.CSParsers csd = null;

                //bool afterdot = false;

                bool dotdeleted = false;

                string w = "";

                string FileName = "";

                string Text = "";

                bool _afterdot = false;

                bool hasdots = false;

                VSProject vp = null;

                Point p = new Point(0,0);

                TopForm form = CodeEditorControl.form;


                dd.Invoke(new Action(() =>
                {

                    dd._runnings = true;

                                        //
                    p = dd.Selection.GetCursor();
                //
                offset = dd.Caret.Offset;
                //
                Row r = dd.Document[p.Y];
                //
                w = dd.Selection.GetCaretWord();

                dotdeleted = false;

                    FileName = "";

                    

                    

                    if (w == ".")
                    dd._afterdot = true;
                else
                {   //
                    string[] pw = dd.Selection.GetPrevCaretWord();
                    if (pw == null || pw[0] != ".")
                    {
                        if (dd._afterdot == true)
                            dotdeleted = true;
                        dd._afterdot = false;
                    }
                }

                CodeEditorControl.ins.afterdot = dd._afterdot;

                if (w == " ")
                    CodeEditorControl.ins.offset = 0;


                 

                    Word ww = dd.Selection.getCaretWord();

                    bool isnumber = double.TryParse(w, out db);

                    if (dotdeleted == true)
                    {

                    }
                    //

                    //
                    var ws = r.FormattedWords;
                    //
                    int cc = ws.IndexOf(ww);
                    //
                    bb = r.PrevNonWs(ww);
                    //
                    hasdots = r.HasDots(ww);

                    csd = dd.csd;

                ins = CodeEditorControl.ins;

                Point pp = dd.Selection.GetCursor();

                pp.X = pp.X * dd.ActiveViewControl.View.CharWidth;

                pp.Y = (dd.ActiveViewControl.Document[p.Y].VisibleIndex - dd.ActiveViewControl.View.FirstVisibleRow + 1) * dd.ActiveViewControl.View.RowHeight + 3;

                FileName = dd.FileName;

                _afterdot = dd._afterdot;

                Text = dd.Document.Text;

                vp = dd.vp;

                if (w.Length <= 1)
                {


                    CodeEditorControl.form.Location = dd.PointToScreen(pp);
                }

                if (CodeEditorControl.form.Visible == true)
                    CodeEditorControl.form.SetTopMost();

                dd.Focus();

                dd._runnings = false;


                    if (w.Length > 2)
                        ins.Find(w, offset, hasdots);

                    ins.resize(400, 207);

                    //ins.v.ResumeLayout();

                    ins.SetVirtualMode();

                }));



                                  

                    if (bb != "")
                    {
                        //MessageBox.Show("Pattern detected - " + bb);

                        ArrayList T = csd.GetAllTypes();

                        foreach (IMember d in CodeEditorControl.ins.LL)
                        {

                            if (d != null)

                                if (d.Name == bb)
                                {
                                    //MessageBox.Show("Pattern has been found - " + d.Name);

                                    IType rb = d.ReturnType;

                                    foreach (ICSharpCode.NRefactory.TypeSystem.ITypeDefinition dd in T)
                                    {
                                        if (dd != null)
                                            if (dd.Name == rb.Name)
                                            {
                                                //MessageBox.Show("Pattern has been found of type " + rb.Name);

                                                CodeEditorControl.ins.Find(dd.Name, offset, false);

                                                ins.resize(300, 400);

                                                return;
                                            }
                                    }
                                }
                        }
                    }

                    //ins.v.SuspendLayout();

                    if ((w.Length < 2 && _afterdot == false && hasdots == false) || w == "." || w == "(" || dotdeleted)
                    {
                        ins.ClearLocalItems();
                        ins.cc.Clear();
                        ins.CC.Clear();
                        //
                        var data = csd.GetCodeComplete(FileName, Text, offset, p.X, p.Y);

                        if (data != null)
                            foreach (var d in data)
                            {
                                VSParsers.EntityCompletionData dd = d as VSParsers.EntityCompletionData;

                                if (dd != null)
                                {
                                    int kind = 0;
                                    if (dd.Entity.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Method)
                                        kind = 1;
                                    else if (dd.Entity.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Property)
                                        kind = 3;
                                    else if (dd.Entity.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Field)
                                        kind = 2;
                                    else if (dd.Entity.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Event)
                                        kind = 4;
                                    else
                                        kind = 5;

                                    ListViewItem v = new ListViewItem(dd.Entity.Name.ToString(), kind);
                                    v.ImageIndex = kind;
                                    ins.cc.Add(v);
                                    ins.CC.Add(v);
                                }
                                else
                                {
                                    VariableCompletionData dv = d as VariableCompletionData;

                                    if (dv != null)
                                    {
                                        if (dv.Variable.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Variable || dv.Variable.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Parameter)
                                        {
                                            ListViewItem v = new ListViewItem(dv.Text.ToString(), 3);
                                            v.ImageIndex = 3;
                                            ins.cc.Add(v);
                                            ins.CC.Add(v);
                                        }
                                    }
                                    else
                                    {
                                        ListViewItem v = new ListViewItem(d.DisplayText.ToString(), 0);
                                        v.ImageIndex = 5;
                                        ins.cc.Add(v);
                                        ins.CC.Add(v);
                                    }
                                }
                            }

                        if (dotdeleted)
                        {
                            ins.LoadTypes(csd);
                            ins.ReloadTypes();
                        }
                    }
                    else
                    if ((w.Length < 2 && _afterdot == false && hasdots == false) || w == ".")

                    {
                        ins.cc.Clear();
                        ins.CC.Clear();

                        var datas = csd.GetCurrentMembers(FileName, Text, offset, p.X, p.Y);

                        ins.v.Items.Clear();

                        if (datas.GetEnumerator().MoveNext() == false)
                        {
                            form.Hide();
                            ins.Hide();

                            Intellisense rr = CodeEditorControl.IntErrors;

                            ArrayList EE = csd.GetErrors();

                            vp.ImportProjectTypes();

                            ArrayList ES = csd.ResolveAt(new TextLocation(p.Y + 1, p.X), Text, FileName, vp);

                            EE.AddRange(ES);

                            rr.LoadErrors(EE, vp);

                            //ins.v.ResumeLayout();

                            return;
                        }

                        foreach (var d in datas)
                        {
                            ISymbol dd = d as ISymbol;

                            if (dd != null)
                            {
                                int kind = 0;
                                if (dd.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Method)
                                    kind = 1;
                                else if (dd.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Property)
                                    kind = 3;
                                else if (dd.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Field)
                                    kind = 2;
                                else if (dd.SymbolKind == ICSharpCode.NRefactory.TypeSystem.SymbolKind.Event)
                                    kind = 4;
                                else
                                    kind = 5;

                                ListViewItem v = new ListViewItem(dd.Name.ToString(), kind);

                                v.ImageIndex = kind;
                                ins.cc.Add(v);
                                ins.CC.Add(v);
                            }
                        }

                        ins.LL = datas;



                        if (w != ".")
                            ins.ReloadTypes();

                        ins.Y = ins.cc.Count;
                    }

                 



                //if (w.Length > 2)
                //    ins.Find(w, offset, hasdots);

                //ins.resize(400, 207);

                //if (w.Length <= 1)
                //{
                //    Point pp = Selection.GetCursor();

                //    pp.X = pp.X * ActiveViewControl.View.CharWidth;

                //    pp.Y = (ActiveViewControl.Document[p.Y].VisibleIndex - ActiveViewControl.View.FirstVisibleRow + 1) * ActiveViewControl.View.RowHeight + 3;

                //    form.Location = this.PointToScreen(pp);
                //}


                //if (form.Visible == true)
                //    form.SetTopMost();

                //this.Focus();

                //_runnings = false;
                //}));

                return;
            }

            

        }

        private void SelectAutolistWord()
        {
            string s = Caret.CurrentRow.Text;
            //try
            //{
            if (_AutoListStartPos == null)
            {
                Word w = Caret.CurrentWord;

                if (w.Text == ".")
                    _AutoListStartPos = new TextPoint(w.Column + 1, Caret.Position.Y);
                else
                    _AutoListStartPos = new TextPoint(w.Column, Caret.Position.Y);
            }
            _CodeEditor.AutoListPosition = new TextPoint(Caret.Position.X + 2, Caret.Position.Y);
            if (Caret.Position.X - _AutoListStartPos.X >= 0 && _AutoListStartPos.X > 0)
            {
                s = s.Substring(_AutoListStartPos.X - 1, Caret.Position.X - _AutoListStartPos.X + 1);
                if (Caret.CurrentWord.Text.Length > 0)
                    s = Caret.CurrentWord.Text;
                s = s.Replace('.', ' ').Trim();

                AutoList.SelectItem(s);
            }
        }
        /// <summary>
        /// Overrides the default OnKeyUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (AutoListVisible)
            {
                //if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                //{
                //    if (AutoListVisible && !e.Handled && _CodeEditor.AutoListAutoSelect)
                //    {
                //        SelectAutolistWord();
                //        return;
                //    }
                //}

                //if (!e.Handled)
                //{
                //    int offset = this.Caret.Offset;
                //    Word w = this.Caret.CurrentWord;
                //    string strWord = "";
                //    if (w != null)
                //        strWord = w.Text;
                //    if ((offset < this.Document.PointToIntPos(startOffset)) || (_AutoListStartPos.Y != this.Caret.Position.Y))
                //    {
                //        CloseInsightWindow(null, null);
                //        AutoListVisible = false;
                //        e.Handled = true;
                //        this.Redraw();
                //        return;
                //    }

                //}
            }
        }

        /// <summary>
        /// Overrides the default OnMouseDown
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // this.Select();
            _mouseX = e.X;
            _mouseY = e.Y;
            _mouseButton = e.Button;

            SetFocus();
            base.OnMouseDown(e);
            TextPoint pos = Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if (pos.Y >= 0 && pos.Y < this.Document.Count)
                row = this.Document[pos.Y];

            if (_mouseButton == MouseButtons.Right)
            {
                ContextMenuStrip c = _contextMenuStrip1;

                c.Show(this, e.Location);

                return;
            }


            #region RowEvent

            RowMouseEventArgs rea = new RowMouseEventArgs();
            rea.Row = row;
            rea.Button = e.Button;
            rea.MouseX = _mouseX;
            rea.MouseY = _mouseY;
            if (e.X >= this.View.TextMargin - 7)
            {
                rea.Area = RowArea.TextArea;
            }
            else if (e.X < this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.GutterArea;
            }
            else if (e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.LineNumberArea;
            }
            else if (e.X < this.View.TextMargin - 7)
            {
                rea.Area = RowArea.FoldingArea;
            }

            this.OnRowMouseDown(rea);

            #endregion

            //try
            //{
            Row r2 = Document[pos.Y];
            if (r2 != null)
            {
                if (e.X >= r2.Expansion_PixelEnd && r2.IsCollapsed)
                {
                    if (r2.Expansion_StartSegment != null)
                    {
                        if (r2.Expansion_StartSegment.StartRow != null && r2.Expansion_StartSegment.EndRow != null && r2.Expansion_StartSegment.Expanded == false)
                        {
                            if (!IsOverSelection(e.X, e.Y))
                            {
                                this.Caret.Position.X = pos.X;
                                this.Caret.Position.Y = pos.Y;
                                this.Selection.ClearSelection();

                                Row r3 = r2.Expansion_EndRow;
                                int x3 = r3.Expansion_StartChar;

                                this.Caret.Position.X = x3;
                                this.Caret.Position.Y = r3.Index;
                                this.Selection.MakeSelection();

                                this.Redraw();
                                View.Action = XTextAction.xtSelect;

                                return;
                            }
                        }
                    }
                }
            }
            //}
            //catch
            //{
            //    //this is untested code...
            //}

            bool shift = NativeUser32Api.IsKeyPressed(Keys.ShiftKey);

            if (e.X > View.TotalMarginWidth)
            {
                if (e.X > View.TextMargin - 8)
                {
                    if (!IsOverSelection(e.X, e.Y))
                    {
                        //selecting
                        if (e.Button == MouseButtons.Left)
                        {
                            if (!shift)
                            {
                                TextPoint tp = pos;
                                Word w = this.Document.GetWordFromPos(tp);
                                if (w != null && w.Pattern != null && w.Pattern.Category != null)
                                {
                                    WordMouseEventArgs pe = new WordMouseEventArgs();
                                    pe.Pattern = w.Pattern;
                                    pe.Button = e.Button;
                                    pe.Cursor = System.Windows.Forms.Cursors.Hand;
                                    pe.Word = w;

                                    this._CodeEditor.OnWordMouseDown(ref pe);

                                    this.Cursor = pe.Cursor;
                                    return;
                                }

                                View.Action = XTextAction.xtSelect;
                                Caret.SetPos(pos);
                                Selection.ClearSelection();
                                Caret.Blink = true;
                                this.Redraw();
                            }
                            else
                            {
                                View.Action = XTextAction.xtSelect;
                                Caret.SetPos(pos);
                                Selection.MakeSelection();
                                Caret.Blink = true;
                                this.Redraw();
                            }
                        }
                    }
                }
                else
                {
                    if (row.Expansion_StartSegment != null)
                    {
                        Caret.SetPos(new TextPoint(0, pos.Y));
                        Selection.ClearSelection();
                        this.Document.ToggleRow(row);
                        this.Redraw();
                    }
                }
            }
            else
            {
                if (e.X < View.GutterMarginWidth)
                {
                    if (_CodeEditor.AllowBreakPoints)
                    {
                        Row r = Document[Painter.CharFromPixel(e.X, e.Y).Y];
                        r.Breakpoint = !r.Breakpoint;
                        this.Redraw();
                    }
                    else
                    {
                        Row r = Document[Painter.CharFromPixel(e.X, e.Y).Y];
                        r.Breakpoint = false;
                        this.Redraw();
                    }
                }
                else
                {
                    View.Action = XTextAction.xtSelect;
                    Caret.SetPos(Painter.CharFromPixel(0, e.Y));
                    Selection.ClearSelection();
                    Caret.MoveDown(true);

                    this.Redraw();
                }
            }
            SetMouseCursor(e.X, e.Y);
        }

        public bool hovered = false;

        public int hovered_exp = -1;

        public string shadowed = "";

        public bool leftright = false;

        public void arrowchanged()
        {
        }

        public void ExpandAll()
        {
            int i = 0;
            while (i < Document.Count)
            {
                Document[i].Expanded = true;

                i++;
            }
        }

        public void ShadowedWords()
        {
            string word = Selection.GetCaretWord();

            if (word == "" || word.Length <= 1)
            {
                _haswords = false;
                return;
            }


            shadowed = word;

            Graphics gg = this.CreateGraphics();
            Color c = Color.FromKnownColor(KnownColor.Gray);
            c = Color.FromArgb(60, c);
            SolidBrush bb = new SolidBrush(c);

            int i = View.FirstVisibleRow;
            while (i >= 0)
            {
                i = Document.Text.IndexOf(word, i);

                if (i < 0)
                    break;

                int g = GetRowndex(i);

                int s = GetRowIndex(i);

                int b = i - s + 2;

                int be = (g - View.FirstVisibleRow) * View.RowHeight;
                Rectangle rec = new Rectangle(10 + 17 + b * View.CharWidth, be, word.Length * View.CharWidth, View.RowHeight);
                gg.FillRectangle(bb, rec);
                if (g - View.FirstVisibleRow > View.VisibleRowCount)
                    break;
                i++;
            }

            _haswords = true;
        }

        public void ShadowedWords2()
        {
            string word = Selection.GetCaretWord();

            int taborig = -View.FirstVisibleColumn * View.CharWidth;

            if (word == "" || word.Length <= 1)
            {
                _haswords = false;
                return;
            }


            shadowed = word;

            Graphics gg = this.CreateGraphics();
            Color c = Color.FromKnownColor(KnownColor.Gray);
            c = Color.FromArgb(60, c);
            SolidBrush bb = new SolidBrush(c);

            int i = View.FirstVisibleRow;

            int cc = View.FirstVisibleRow + View.VisibleRowCount;


            while (i < cc)
            {
                if (i < Document.VisibleRows.Count)
                {
                    Row row = Document.VisibleRows[i];

                    int wdh = 0;


                    int nt = 0;

                    foreach (Word w in row)
                    {
                        if (w.Type == WordType.xtTab)
                        {
                            wdh += PixelTabSize;
                            nt++;
                        }

                        if (w.Text == word)
                        {
                            int b = w.Column - nt;

                            int be = (i - View.FirstVisibleRow) * View.RowHeight;
                            //int be = i * View.RowHeight;

                            int tt = View.TotalMarginWidth + 3 * View.CharWidth;// + View.TextMargin;

                            Rectangle rec = new Rectangle(taborig + wdh + tt + b * View.CharWidth, be, word.Length * View.CharWidth, View.RowHeight);
                            gg.FillRectangle(bb, rec);
                            //Brush zigzagBrush = new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.ZigZag, Color.Red);
                            //gg.FillRectangle(zigzagBrush, taborig + wdh + tt + b * View.CharWidth, be + View.RowHeight + 1, word.Length * View.CharWidth, (int)(View.RowHeight * 0.2));

                        }
                    }

                    //i = Document.Text.IndexOf(word, i);

                    //if (i < 0)
                    //    break;

                    //int g = GetRowndex(i);

                    //int s = GetRowIndex(i);

                    //int b = i - s + 2;

                    //int be = (g - View.FirstVisibleRow) * View.RowHeight;
                    //Rectangle rec = new Rectangle(10 + 17 + b * View.CharWidth, be, word.Length * View.CharWidth, View.RowHeight);
                    //gg.FillRectangle(bb, rec);
                    //if (g - View.FirstVisibleRow > View.VisibleRowCount)
                    //    break;
                }
                i++;
            }

            _haswords = true;
        }


        public void FirstLine(int line)
        {
            View.FirstVisibleRow = line;

            this.Refresh();
        }

        public void Highlight(int line)
        {
            if (Document.Count <= line)
                return;

            int be = (Document[line].VisibleIndex - View.FirstVisibleRow) * View.RowHeight;

            Graphics g = this.CreateGraphics();
            Color c = Color.FromKnownColor(KnownColor.Control);
            c = Color.FromArgb(255, c);
            SolidBrush b = new SolidBrush(c);
            Rectangle rec = new Rectangle(5, be, View.ClientAreaWidth + 20, 3 * View.RowHeight);
            g.FillRectangle(b, rec);
        }


        private bool _hashover = false;

        private bool _haswords = false;

        /// <summary>
        /// Overrides the default OnMouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            _mouseButton = e.Button;

            TextPoint pos = Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if (pos.Y >= 0 && pos.Y < this.Document.Count)
                //row = this.Document.VisibleRows[pos.Y];
                row = this.Document[pos.Y];
            #region RowEvent

            RowMouseEventArgs rea = new RowMouseEventArgs();
            rea.Row = row;
            rea.Button = e.Button;
            rea.MouseX = _mouseX;
            rea.MouseY = _mouseY;
            if (e.X >= this.View.TextMargin - 7)
            {
                rea.Area = RowArea.TextArea;
            }
            else if (e.X < this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.GutterArea;
            }
            else if (e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.LineNumberArea;
            }
            else if (e.X < this.View.TextMargin - 7)
            {
                rea.Area = RowArea.FoldingArea;
            }

            this.OnRowMouseMove(rea);

            #endregion

            //try
            //{
            if (Document != null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (View.Action == XTextAction.xtSelect)
                    {
                        //Selection ACTIONS!!!!!!!!!!!!!!
                        Caret.Blink = true;
                        Caret.SetPos(pos);
                        if (e.X <= View.TotalMarginWidth)
                            Caret.MoveDown(true);
                        Caret.CropPosition();
                        Selection.MakeSelection();
                        ScrollIntoView();
                        this.Redraw();
                    }
                    //else if (View.Action == XTextAction.xtNone)
                    //{
                    //    if (IsOverSelection(e.X, e.Y))
                    //    {
                    //        BeginDragDrop();
                    //    }
                    //}
                    //else if (View.Action == XTextAction.xtDragText)
                    //{
                    //}

                }
                else
                {
                    TextPoint p = pos;
                    Row r = Document[p.Y];

                    if (r != null)
                    {
                        Row rr = this.Document[p.Y];

                        r = rr;


                        Row se = null;
                        Row ee = null;

                        if (e.X - this.View.TotalMarginWidth - 17 <= 1)
                        {
                            //   MessageBox.Show("left mouse hover...");
                            Row t = rr;//row;
                            int i = r.VisibleIndex;
                            int gs = i;
                            while (i >= 0)
                            {
                                // if (r.Expanded == false)
                                while (i > 0 && (t.Expanded != true))
                                {
                                    t = t.PrevVisibleRow;
                                    i--;
                                }

                                if (t.IsCollapsed == false)
                                {
                                    rr = t;// this.Document[i];


                                    se = rr.Expansion_StartRow;
                                    ee = rr.Expansion_EndRow;

                                    if (se.VisibleIndex <= gs)
                                        if (ee.VisibleIndex >= gs)
                                            break;
                                }
                                t = t.PrevVisibleRow;
                                i--;
                            }

                            //Row rr = this.Document[i];

                            //Row rr =  this.Document.VisibleRows[i];


                            //Row se = rr.Expansion_StartRow;
                            //Row ee = rr.Expansion_EndRow;

                            int be = (se.VisibleIndex - View.FirstVisibleRow) * View.RowHeight;
                            int ce = (ee.VisibleIndex - View.FirstVisibleRow) * View.RowHeight;

                            if (be != hovered_exp)
                            {
                                this.Refresh();
                                Graphics g = this.CreateGraphics();
                                Color c = Color.FromKnownColor(KnownColor.Control);
                                c = Color.FromArgb(100, c);
                                SolidBrush b = new SolidBrush(c);
                                Rectangle rec = new Rectangle(5, be, View.ClientAreaWidth + 20, ce - be + View.RowHeight);
                                g.FillRectangle(b, rec);
                                int cc = this.Selection.GetCursorLine();
                                if (be <= cc && ce >= cc)
                                    hovered = true;
                                else
                                {
                                    hovered = false;
                                }

                                _hashover = true;

                                hovered_exp = be;
                            }
                        }
                        else
                        {
                            hovered_exp = -1;
                            if (hovered == true)
                            {
                                hovered = false;
                            }
                            else
                                hovered = false;
                            if (_hashover)
                            {
                                _hashover = false;
                                //this.Refresh();
                                this.Redraw(false);
                            }
                        }

                        if (e.X >= r.Expansion_PixelEnd && r.IsCollapsed)
                        {
                            if (r.Expansion_StartSegment != null)
                            {
                                if (r.Expansion_StartSegment.StartRow != null && r.Expansion_StartSegment.EndRow != null && r.Expansion_StartSegment.Expanded == false)
                                {
                                    string t = "";
                                    int j = 0;
                                    for (int i = r.Expansion_StartSegment.StartRow.Index; i <= r.Expansion_StartSegment.EndRow.Index; i++)
                                    {
                                        if (j > 0)
                                            t += "\n";
                                        Row tmp = Document[i];
                                        string tmpstr = tmp.Text.Replace("\t", "    ");
                                        t += tmpstr;
                                        if (j > 20)
                                        {
                                            t += "...";
                                            break;
                                        }

                                        j++;
                                    }
                                }
                            }
                        }
                        //else
                        {
                            //if (refresher == false)
                            //{
                            //TextPoint ppp = Painter.CharFromPixel(e.X, e.Y);
                            Word w = this.Document.GetWordFromPos(pos);
                            //Word w = this.Document.GetFormatWordFromPos(pos);
                            //if(Painter.IsMouseOverWord(pos.Y, e.X))
                            if (w != null)
                            {
                                // MessageBox.Show("Entered word - " + w.Text);

                                if (mousemoveword != w.Text || mousemoveword == "")
                                {
                                    mousemoveword = w.Text;

                                    if (CodeEditor.csd != null)
                                    {
                                        //if (CodeEditor.vp != null)
                                        //{
                                        //    ArrayList L = CodeEditor.vp.GetCompileItems();
                                        //    CodeEditor.csd = CodeEditor.vp.CSParsers(); 
                                        //    CodeEditor.csd .AddProjectFiles(L);
                                        //    InfoTip.vp = CodeEditor.vp;
                                        //    InfoTip.keywords = ExtractKeywords();
                                        //}


                                        if (InfoTip.vp == null)
                                        {
                                            InfoTip.vp = CodeEditor.vp;
                                            InfoTip.keywords = ExtractKeywords();
                                        }

                                        //if (w.InfoTip != null)
                                        {
                                            string s = "";

                                            if (w.Text != "")
                                            {
                                                TextLocation pp = new TextLocation(pos.Y + 1, pos.X + 1);

                                                //InfoTip.SetText(CodeEditor.csd.Resolve(pp, Document.Text, FileName), w.Text);

                                                int a = e.X + 1;// View.CharWidth * w.Column  + View.GutterMarginWidth + 100;
                                                int b = (Document[p.Y].VisibleIndex - View.FirstVisibleRow) * View.RowHeight;
                                                int c = row.Expansion_EndChar * View.CharWidth + View.GutterMarginWidth + 100;
                                                if (_mouseX <= c)
                                                {
                                                    InfoTip.Location = new Point(a, b + 20);
                                                    if (CodeEditor.csd != null)
                                                    {
                                                        ResolveResult res = CodeEditor.csd.Resolve(pp, Document.Text, FileName);

                                                        s = InfoTip.SetText(res, w.Text);
                                                    }
                                                    if (s != "")
                                                        InfoTip.Show();
                                                    else InfoTip.Hide();
                                                }
                                                else
                                                {
                                                    mousemoveword = "";
                                                    if (s == "") InfoTip.Hide();
                                                }
                                            }
                                            else InfoTip.Hide();
                                        }
                                    }
                                     else InfoTip.Hide();
                                }
                                 //else InfoTip.Hide();
                            }
                            else
                            {
                                mousemoveword = "";
                                InfoTip.Hide();
                            }

                            //refresher = true;
                        }
                    }
                }


                string word = Selection.GetCaretWord();
                if (hovered == false)
                    if (word != shadowed)
                        ShadowedWords();

                SetMouseCursor(e.X, e.Y);



                base.OnMouseMove(e);
            }

            //}
            //catch
            //{
            //}
        }

        private bool _refresher = false;

        private string mousemoveword { get; set; }

        private void TimerProc(object state)
        {
            _refresher = false;
        }

        //public void MouseMoveResolver()
        //{

        //    while (1 == 1)
        //    {



        //        this.Invoke(new Action(() => {


        //            //MouseMoveResolve();

        //            System.Threading.Thread.Sleep(1000);

        //        }));

        //    }
        //}

        //public void MouseMoveResolve()
        //{

        //    //while (1 == 1)
        //    {



        //      //  this.Invoke(new Action(() => {

        //            this.Cursor = Cursors.Default;

        //            Point e = this.PointToClient(Cursor.Position);
        //        TextPoint p = Painter.CharFromPixel(e.X, e.Y);
        //        Word w = this.Document.GetFormatWordFromPos(p);
        //        if (w != null)
        //        {
        //            if (CodeEditor.csd == null)
        //            {

        //                ArrayList L = CodeEditor.vp.GetCompileItems();
        //                CodeEditor.csd.AddProjectFiles(L);
        //            }


        //            {


        //                if (w.Text != "")
        //                {

        //                    TextLocation pp = new TextLocation(p.Y + 1, p.X + 1);

        //                    InfoTip.SetText(CSDemo.Resolve(pp, Document.Text, FileName));




        //                    int a = View.CharWidth * w.Column;
        //                    int b = (p.Y - View.FirstVisibleRow) * View.RowHeight;
        //                    int c = Document[p.Y].Expansion_EndChar * View.CharWidth + View.GutterMarginWidth;
        //                    if (MouseX <= c)
        //                    {
        //                        InfoTip.Location = new Point(a, b + 20);
        //                        InfoTip.SetText(CSDemo.Resolve(pp, Document.Text, FileName));
        //                        InfoTip.Show();
        //                            this.Refresh();
        //                    }
        //                }
        //                else InfoTip.Hide();
        //            }


        //        }
        //        else InfoTip.Hide();



        //    //    System.Threading.Thread.Sleep(1000);

        //    //}));

        //    }
        //}


        private bool _shadows = false;

        /// <summary>
        /// Overrides the default OnMouseUp
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
            _mouseButton = e.Button;

            TextPoint pos = Painter.CharFromPixel(e.X, e.Y);
            Row row = null;
            if (pos.Y >= 0 && pos.Y < this.Document.Count)
                row = this.Document[pos.Y];

            #region RowEvent

            RowMouseEventArgs rea = new RowMouseEventArgs();
            rea.Row = row;
            rea.Button = e.Button;
            rea.MouseX = _mouseX;
            rea.MouseY = _mouseY;
            if (e.X >= this.View.TextMargin - 7)
            {
                rea.Area = RowArea.TextArea;
            }
            else if (e.X < this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.GutterArea;
            }
            else if (e.X < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.LineNumberArea;
            }
            else if (e.X < this.View.TextMargin - 7)
            {
                rea.Area = RowArea.FoldingArea;
            }

            this.OnRowMouseUp(rea);

            #endregion

            if (View.Action == XTextAction.xtNone)
            {
                if (e.X > View.TotalMarginWidth)
                {
                    if (IsOverSelection(e.X, e.Y) && e.Button == MouseButtons.Left)
                    {
                        View.Action = XTextAction.xtSelect;
                        Caret.SetPos(Painter.CharFromPixel(e.X, e.Y));
                        Selection.ClearSelection();
                        this.Redraw();
                    }
                }
            }

            View.Action = XTextAction.xtNone;
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Overrides the default OnMouseWheel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int l = SystemInformation.MouseWheelScrollLines;
            ScrollScreen(-(e.Delta / 120) * l, 2);

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Overrides the default OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Document != null && this.Width > 0 && this.Height > 0)
            {
                Painter.RenderAll(e.Graphics);

                ShadowedWords2();
            }
        }

        /// <summary>
        /// Overrides the default OnHandleCreated
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
        }


        private int _oldWidth = 0;

        /// <summary>
        /// Overrides the default OnResize
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DoResize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Overrides the default OnDragOver
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (!this.ReadOnly)
            {
                if (Document != null)
                {
                    View.Action = XTextAction.xtDragText;

                    Point pt = this.PointToClient(new Point(drgevent.X, drgevent.Y));

                    int x = pt.X;
                    int y = pt.Y;

                    //	drgevent.Effect = DragDropEffects.All  ;
                    //Caret.Position = Painter.CharFromPixel(x,y);

                    if ((drgevent.KeyState & 8) == 8)
                    {
                        drgevent.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        drgevent.Effect = DragDropEffects.Move;
                    }
                    Caret.SetPos(Painter.CharFromPixel(x, y));
                    this.Redraw();
                }
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }

            base.OnDragOver(drgevent);
        }

        /// <summary>
        /// Overrides the default OnDragDrop
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!this.ReadOnly)
            {
                if (Document != null)
                {
                    View.Action = XTextAction.xtNone;
                    int SelStart = Selection.LogicalSelStart;
                    int DropStart = Document.PointToIntPos(Caret.Position);


                    string s = drgevent.Data.GetData(typeof(string)).ToString();
                    //int SelLen=s.Replace ("\r\n","\n").Length ;
                    int SelLen = s.Length;


                    if (DropStart >= SelStart && DropStart <= SelStart + Math.Abs(Selection.SelLength))
                        DropStart = SelStart;
                    else if (DropStart >= SelStart + SelLen)
                        DropStart -= SelLen;


                    this.Document.StartUndoCapture();
                    if ((drgevent.KeyState & 8) == 0)
                    {
                        this._CodeEditor.Selection.DeleteSelection();
                        this.Caret.Position = Document.IntPosToPoint(DropStart);
                    }

                    TextPoint p = Document.InsertText(s, Caret.Position.X, Caret.Position.Y);
                    this.Document.EndUndoCapture();

                    Selection.SelStart = Document.PointToIntPos(Caret.Position);
                    Selection.SelLength = SelLen;
                    Document.ResetVisibleRows();
                    ScrollIntoView();
                    this.Redraw();
                    drgevent.Effect = DragDropEffects.All;

                    if (ParseOnPaste)
                        this.Document.ParseAll(true);

                    View.Action = XTextAction.xtNone;
                }
            }

            base.OnDragDrop(drgevent);
        }


        /// <summary>
        ///  Overrides the default OnDragEnter
        /// </summary>
        /// <param name="drgevent"></param>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
        }

        /// <summary>
        ///  Overrides the default OnDragLeave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragLeave(EventArgs e)
        {
            View.Action = XTextAction.xtNone;
            base.OnDragLeave(e);
        }

        /// <summary>
        ///  Overrides the default OnDoubleClick
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            TextPoint pos = Painter.CharFromPixel(_mouseX, _mouseY);
            Row row = null;
            if (pos.Y >= 0 && pos.Y < this.Document.Count)
                row = this.Document[pos.Y];

            #region RowEvent

            RowMouseEventArgs rea = new RowMouseEventArgs();
            rea.Row = row;
            rea.Button = MouseButtons.None;
            rea.MouseX = _mouseX;
            rea.MouseY = _mouseY;
            if (_mouseX >= this.View.TextMargin - 7)
            {
                rea.Area = RowArea.TextArea;
            }
            else if (_mouseX < this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.GutterArea;
            }
            else if (_mouseX < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.LineNumberArea;
            }
            else if (_mouseX < this.View.TextMargin - 7)
            {
                rea.Area = RowArea.FoldingArea;
            }

            this.OnRowDoubleClick(rea);

            #endregion

            //try
            //{
            Row r2 = Document[pos.Y];
            if (r2 != null)
            {
                if (_mouseX >= r2.Expansion_PixelEnd && r2.IsCollapsed)
                {
                    if (r2.Expansion_StartSegment != null)
                    {
                        if (r2.Expansion_StartSegment.StartRow != null && r2.Expansion_StartSegment.EndRow != null && r2.Expansion_StartSegment.Expanded == false)
                        {
                            r2.Expanded = true;
                            this.Document.ResetVisibleRows();
                            this.Redraw();
                            return;
                        }
                    }
                }
            }
            //}
            //catch
            //{
            //    //this is untested code...
            //}

            if (_mouseX > this.View.TotalMarginWidth)
                SelectCurrentWord();
        }

        protected override void OnClick(EventArgs e)
        {
            TextPoint pos = Painter.CharFromPixel(_mouseX, _mouseY);
            Row row = null;
            if (pos.Y >= 0 && pos.Y < this.Document.Count)
                row = this.Document[pos.Y];

            #region RowEvent

            RowMouseEventArgs rea = new RowMouseEventArgs();
            rea.Row = row;
            rea.Button = MouseButtons.None;
            rea.MouseX = _mouseX;
            rea.MouseY = _mouseY;
            if (_mouseX >= this.View.TextMargin - 7)
            {
                rea.Area = RowArea.TextArea;
            }
            else if (_mouseX < this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.GutterArea;
            }
            else if (_mouseX < this.View.LineNumberMarginWidth + this.View.GutterMarginWidth)
            {
                rea.Area = RowArea.LineNumberArea;
            }
            else if (_mouseX < this.View.TextMargin - 7)
            {
                rea.Area = RowArea.FoldingArea;
            }

            this.OnRowClick(rea);

            #endregion
        }

        private void vScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            SetMaxHorizontalScroll();
            _InfoTipVisible = false;
            AutoListVisible = false;

            SetFocus();


            int diff = e.NewValue - vScroll.Value;
            if ((diff == -1 || diff == 1) && (e.Type == ScrollEventType.SmallDecrement || e.Type == ScrollEventType.SmallIncrement))
            {
                ScrollScreen(diff);
            }
            else
            {
                Invalidate();
            }
        }

        private void hScroll_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {
            _InfoTipVisible = false;
            AutoListVisible = false;

            SetFocus();
            Invalidate();
        }

        private void CaretTimer_Tick(object sender, EventArgs e)
        {
            Caret.Blink = !Caret.Blink;
            this.RedrawCaret();
        }


        private void AutoListDoubleClick(object sender, EventArgs e)
        {
            string s = AutoList.SelectedText;
            if (s != "")
                InsertAutolistText();
            AutoListVisible = false;
            this.Redraw();
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_tooltip != null)
                _tooltip.RemoveAll();
        }

        private void CaretChanged(object s, EventArgs e)
        {
            OnCaretChange();
        }

        private void EditViewControl_Leave(object sender, EventArgs e)
        {
            RemoveFocus();
        }

        private void EditViewControl_Enter(object sender, EventArgs e)
        {
            _caretTimer.Enabled = true;
        }

        private void SelectionChanged(object s, EventArgs e)
        {
            OnSelectionChange();
        }

        private void OnCaretChange()
        {
            if (CaretChange != null)
                CaretChange(this, null);
        }

        private void OnSelectionChange()
        {
            if (SelectionChange != null)
                SelectionChange(this, null);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible == false)
                RemoveFocus();

            base.OnVisibleChanged(e);
            DoResize();
        }

        #endregion

        private void IntelliMouse_BeginScroll(object sender, EventArgs e)
        {
            _IntelliScrollPos = 0;
            this.View.YOffset = 0;
        }

        private void IntelliMouse_EndScroll(object sender, EventArgs e)
        {
            this.View.YOffset = 0;
            this.Redraw();
        }

        private void IntelliMouse_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.DeltaY < 0 && vScroll.Value == 0)
            {
                this.View.YOffset = 0;
                this.Redraw();
                return;
            }

            if (e.DeltaY > 0 && vScroll.Value >= vScroll.Maximum - this.View.VisibleRowCount + 1)
            {
                this.View.YOffset = 0;
                this.Redraw();
                return;
            }

            _IntelliScrollPos += (double)e.DeltaY / (double)8;

            int scrollrows = (int)(_IntelliScrollPos) / this.View.RowHeight;
            if (scrollrows != 0)
            {
                _IntelliScrollPos -= scrollrows * this.View.RowHeight;
            }
            this.View.YOffset = -(int)_IntelliScrollPos;
            this.ScrollScreen(scrollrows);
        }



        private static void compile(string source, VSProject vp)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            //var asm = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "foo.exe", true);
            //string[] asm = GetAssemblyNames(source);
            string[] asm = new[] { "mscorlib.dll", "System.dll", "System.Xml.dll", "System.Linq.dll", "System.Drawing.dll", "System.Reflection.dll", "System.ComponentModel.dll", "System.Windows.Forms.dll" };
            var parameters = new CompilerParameters(asm, "foo.exe", true);
            parameters.GenerateExecutable = false;

            ArrayList P = new ArrayList();

            ArrayList L = vp.GetCompileItems();
            foreach (string s in L)
            {
                string d = File.ReadAllText(s);

                P.Add(d);
            }


            string[] sources = P.ToArray(typeof(string)) as string[];

            CompilerResults results = csc.CompileAssemblyFromSource(parameters, sources);
            //@"using System.Linq;
            //class Program {
            //  public static void Main(string[] args) {
            //    var q = from i in Enumerable.Range(1,100)
            //              where i % 2 == 0
            //              select i;
            //  }
            //}");
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }

        private static string[] GetAssemblyNames(string source)
        {
            ArrayList L = new ArrayList();

            string[] lines = source.Split("\r\n".ToCharArray());

            foreach (string s in lines)
            {
                if (s.StartsWith("using") == false)
                    continue;

                string[] d = s.Split(" \t".ToCharArray());

                string dd = d[1].Replace(";", "") + ".dll";

                L.Add(dd);
            }



            return L.ToArray(typeof(string)) as string[];
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowMessage.WM_DESTROY)
            {
                try
                {
                    if (FindReplaceDialog != null)
                        FindReplaceDialog.Close();

                    if (AutoList != null)
                        AutoList.Close();

                    //if (InfoTip != null)
                    //	InfoTip.Close();
                }
                catch
                {
                }
            }

            base.WndProc(ref m);
        }

        private void CopyAsRTF()
        {
            TextStyle[] styles = this.Document.Parser.Language.Styles;
            this.Document.ParseAll(true);
            int r1 = Selection.LogicalBounds.FirstRow;
            int r2 = Selection.LogicalBounds.LastRow;
            int c1 = Selection.LogicalBounds.FirstColumn;
            int c2 = Selection.LogicalBounds.LastColumn;

            StringBuilder sb = new StringBuilder();
            sb.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1053{\fonttbl{\f0\fmodern\fprq1\fcharset0 " + this.FontName + @";}}");
            sb.Append(@"{\colortbl ;");

            foreach (TextStyle ts in styles)
            {
                sb.AppendFormat("\\red{0}\\green{1}\\blue{2};", ts.ForeColor.R, ts.ForeColor.G, ts.ForeColor.B);
                sb.AppendFormat("\\red{0}\\green{1}\\blue{2};", ts.BackColor.R, ts.BackColor.G, ts.BackColor.B);
            }

            sb.Append(@";}");
            sb.Append(@"\viewkind4\uc1\pard\f0\fs20");


            bool Done = false;
            for (int i = r1; i <= r2; i++)
            {
                Row row = this.Document[i];


                foreach (Word w in row)
                {
                    if (i == r1 && w.Column + w.Text.Length < c1)
                        continue;

                    bool IsFirst = (i == r1 && w.Column <= c1 && w.Column + w.Text.Length > c1);
                    bool IsLast = (i == r2 && w.Column < c2 && w.Column + w.Text.Length > c2);


                    if (w.Type == WordType.xtWord && w.Style != null)
                    {
                        int clrindex = Array.IndexOf(styles, w.Style);
                        clrindex *= 2;
                        clrindex++;

                        sb.Append("{\\cf" + clrindex.ToString());
                        if (!w.Style.Transparent)
                        {
                            sb.Append("\\highlight" + (clrindex + 1).ToString());
                        }
                        sb.Append(" ");
                    }

                    if (w.Style != null)
                    {
                        if (w.Style.Bold)
                            sb.Append(@"\b ");
                        if (w.Style.Underline)
                            sb.Append(@"\ul ");
                        if (w.Style.Italic)
                            sb.Append(@"\i ");
                    }
                    string wordtext = w.Text;

                    if (IsLast)
                        wordtext = wordtext.Substring(0, c2 - w.Column);

                    if (IsFirst)
                        wordtext = wordtext.Substring(c1 - w.Column);


                    wordtext = wordtext.Replace(@"\", @"\\").Replace(@"}", @"\}").Replace(@"{", @"\{");

                    sb.Append(wordtext);

                    if (w.Style != null)
                    {
                        if (w.Style.Bold)
                            sb.Append(@"\b0 ");
                        if (w.Style.Underline)
                            sb.Append(@"\ul0 ");
                        if (w.Style.Italic)
                            sb.Append(@"\i0 ");
                    }

                    if (w.Type == WordType.xtWord && w.Style != null)
                    {
                        sb.Append("}");
                    }

                    if (IsLast)
                    {
                        Done = true;
                        break;
                    }
                }
                if (Done)
                    break;

                sb.Append(@"\par");
            }


            DataObject da;


            da = new DataObject();
            da.SetData(DataFormats.Rtf, sb.ToString());
            string s = this.Selection.Text;
            da.SetData(DataFormats.Text, s);
            Clipboard.SetDataObject(da);

            CopyEventArgs ea = new CopyEventArgs();
            ea.Text = s;
            OnClipboardUpdated(ea);
        }


        /// <summary>
        /// Scrolls a given position in the text to the center of the view.
        /// </summary>
        /// <param name="Pos">Position in text</param>
        public void CenterInView(int rowIndex)
        {
            //this method is a contribution of Marcel Isler goodfun (at) goodfun (dot) org 
            if (this.Document == null && rowIndex > this.Document.Count
                && rowIndex < 0)
                return;

            Row row = this.Document[rowIndex];

            int topRow = 0;
            int visibleLines = 0;

            if (row != null)
            {
                InitScrollbars();
                row.EnsureVisible();

                visibleLines = View.VisibleRowCount / 2;
                if (row.VisibleIndex < visibleLines)
                {
                    topRow = 0;
                }
                else
                {
                    topRow = row.VisibleIndex - visibleLines;
                }
                this.vScroll.Value = topRow;
                this.Invalidate();
            }
        }

        public int GetLineIndex(int start)
        {
            int i = 0;

            int cs = 0;

            start--;

            Row row = this.Document[i];

            //int pp = row.Index;

            while (i <= start)
            {
                cs += row.Text.Length + 1;
                //cs += row.Expansion_EndChar;
                row = this.Document[++i];
            }

            return cs;
        }
        public int GetRowndex(int start)
        {
            int cs = 0;


            int i = 0;

            while (i < this.Document.Count)
            {
                Row row = this.Document[i];

                cs += row.Text.Length + 2;
                if (cs > start)
                    return i;
                i++;
            }

            return 0;
        }

        public int GetRowIndex(int start)
        {
            int cs = 0;


            int i = 0;

            while (i < this.Document.Count)
            {
                Row row = this.Document[i];
                cs += row.Text.Length + 2;
                if (cs > start)
                    return cs - (row.Text.Length + 2);
                i++;
            }

            return 0;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }
    }

    public class PanelExt : Panel
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr s_HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        public Label label { get; set; }

        public VSProject vp { get; set; }

        public ArrayList keywords { get; set; }

        public PanelExt() : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);

            this.BackColor = Color.FromKnownColor(KnownColor.Control);

            //label = new Label();

            //this.BackColor = SystemColors.ControlLight;

            //this.Controls.Add(label);

            this.BorderStyle = BorderStyle.None;

            this.BorderColor = SystemColors.InactiveCaption;

            // label.Visible = false;

            SetTopMost();
        }

        private SyntaxDocument _doc = new SyntaxDocument();

        private Dictionary<string, string> d { get; set; }

        public void SetTopMost()
        {
            SetWindowPos(this.Handle, s_HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        private enum chars { r, l, p }

        private class Words
        {
            public chars c;

            public string b;

            public int start = 0;
        }

        private string documentation { get; set; }

        public ArrayList Parse(string word)
        {
            documentation = "";

            ArrayList L = new ArrayList();

            word = word.Replace("\r", "");

            string[] bb = word.Split("\n".ToCharArray());

            if (bb.Length >= 2)
            {
                string doc = bb[1];

                documentation = doc;

                word = bb[0];
            }

            string[] b = word.Split(".(); \t".ToCharArray());

            Words w;

            int i = 0;
            foreach (string c in b)
            {
                if (c == "" || c == " ")
                    continue;

                i = word.IndexOf(c, i);

                if (i > 0 && c != ".")
                {
                    w = new Words();
                    w.b = word[i - 1].ToString();
                    w.start = i - 1;
                    L.Add(w);
                }

                w = new Words();
                w.b = c.Trim();
                w.start = i;
                L.Add(w);

                i += c.Length;
            }
            if (b.Length > 1)
                if (word[word.Length - 2] == ')' || word[word.Length - 2] == '(')
                {
                    i = word.IndexOf(b[b.Length - 2]);
                    w = new Words();
                    w.b = word[word.Length - 2].ToString();
                    w.start = i;
                    L.Add(w);
                }
            if (b.Length > 0)
                if (word[word.Length - 1] == ')' || word[word.Length - 1] == '(')
                {
                    i = word.IndexOf(b[b.Length - 1]);
                    w = new Words();
                    w.b = word[word.Length - 1].ToString();
                    w.start = i;
                    L.Add(w);
                }
                else
                {
                    w = new Words();
                    w.b = " ";
                    w.start = i;
                    L.Add(w);
                }

            return L;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawRectangle(
                new Pen(
                    new SolidBrush(BorderColor), 2),
                    e.ClipRectangle);

            if (texts == null)
                return;

            if (texts == "")
                return;

            if (keywords == null)
                keywords = new ArrayList();

            Graphics g = e.Graphics;

            //string[] b = texts.Split("() .[]".ToCharArray());

            SizeF s = g.MeasureString("c", this.Font, 1000);

            int p = 5;

            if (d == null)
                d = vp.GetTypeNames();

            Rectangle rect = new Rectangle(5, 2, 15, 15);

            ArrayList L = Parse(texts);

            int bb = 0;
            foreach (Words ws in L)
            {
                bool shouldbeblue = false;

                string w = ws.b;

                switch (w)
                {
                    case "class":
                        g.DrawImage(resource._class, rect);
                        p = 25;
                        shouldbeblue = true;
                        break;
                    case "struct":
                        g.DrawImage(resource.structure, rect);
                        p = 25;
                        shouldbeblue = true;
                        break;
                    case "interface":
                        g.DrawImage(resource._interface, rect);
                        p = 25;
                        break;
                    case "delegate":
                        g.DrawImage(resource._delegate, rect);
                        p = 25;
                        break;
                    case "event":
                        g.DrawImage(resource._event, rect);
                        p = 25;
                        continue;
                    //break;
                    case "field":
                        g.DrawImage(resource.field, rect);
                        p = 25;
                        break;
                    case "variable":
                        g.DrawImage(resource.field, rect);
                        p = 25;
                        break;
                    case "local-variable":
                        g.DrawImage(resource.field, rect);
                        p = 25;
                        break;
                    case "parameter":
                        g.DrawImage(resource.field, rect);
                        p = 25;
                        break;
                    case "enum":
                        g.DrawImage(resource.field, rect);
                        p = 25;
                        break;
                    case "property":
                        g.DrawImage(resource.property, rect);
                        p = 25;
                        break;
                    case "namespace":
                        g.DrawImage(resource._namespace, rect);
                        p = 25;
                        break;
                    case "method":
                        if (bb == 0)
                        {
                            g.DrawImage(resource.methods, rect);
                            p = 25;
                            continue;
                        }
                        break;
                }

                bb++;

                if (ws.b == "")
                    continue;

                if (ws.b == " ")
                {
                    g.DrawString("  ", this.Font, Brushes.Blue, new PointF(p, 5));

                    s = e.Graphics.MeasureString("  ",
                                   this.Font,
                                   new SizeF(Width, 15f),
                                   StringFormat.GenericDefault);
                    p += (int)s.Width + 2;

                    continue;
                }

                if (d.ContainsKey(ws.b) || d.ContainsKey(ws.b.ToLower()))
                {
                    if (keywords.Contains(ws.b.ToLower()))

                        g.DrawString(ws.b.ToLower(), this.Font, Brushes.Blue, new PointF(p, 5));
                    else
                        g.DrawString(ws.b, this.Font, Brushes.LightBlue, new PointF(p, 5));

                    //s = TextRenderer.MeasureText(ws.b, this.Font);
                    //s = g.MeasureString(ws.b, this.Font);
                    s = e.Graphics.MeasureString(ws.b,
                                      this.Font,
                                      new SizeF(Width, 15f),
                                      StringFormat.GenericTypographic);
                    p += (int)s.Width + 1;
                }
                else

                //
                //                   foreach (Words w in L)
                //                   {


                //if (keywords.Contains(w.b.ToLower()))
                //{
                //    g.DrawString(w.b.ToLower(), this.Font, Brushes.Blue, new PointF(p, 5));
                //    s = g.MeasureString(w.b, this.Font, 1000);
                //    p += (int)s.Width;
                //}
                //else
                {
                    if (shouldbeblue == true)
                        g.DrawString(ws.b, this.Font, Brushes.Blue, new PointF(p, 5));
                    else
                        g.DrawString(ws.b, this.Font, Brushes.Black, new PointF(p, 5));
                    //s = TextRenderer.MeasureText(ws.b, this.Font);
                    s = e.Graphics.MeasureString(ws.b,
                                      this.Font,
                                      new SizeF(Width, 15f),
                                      StringFormat.GenericTypographic);
                    //s = g.MeasureString(ws.b, this.Font);
                    p += (int)s.Width + 1;
                }
            }

            if (documentation == "")
                return;

            p = 5;

            g.DrawString(documentation, this.Font, Brushes.Black, new PointF(p, 35));
        }
        public Color BorderColor { get; set; }

        private ResolveResult b { get; set; }

        public string texts { get; set; }



        public string ResolveText(ResolveResult r)
        {
            string text = "";

            if (r == null)
                return "";

            if (r.GetType() == typeof(NamespaceResolveResult))
            {
                NamespaceResolveResult ns = (NamespaceResolveResult)r;

                text = "namespace " + ns.Namespace.FullName;

                return text;
            }

            else if (r.GetType() == typeof(ThisResolveResult))
            {
                //text = r.Type.FullName;

                text = "class " + r.Type.FullName;


                return text;
            }
            else if (r.GetType() == typeof(ConstantResolveResult))
            {
                ConstantResolveResult rr = r as ConstantResolveResult;

                string s = rr.ConstantValue.ToString();

                int w = s.Length;

                if (w > 25)
                    w = 25;

                w--;

                if (w <= 0)
                    w = 0;

                text = rr.Type.Name + " : ";
                if (s != "")
                    text += s.Substring(0, w + 1);



                return text;
            }
            else if (r.GetType() == typeof(CSharpInvocationResolveResult))
            {
                CSharpInvocationResolveResult rr = r as CSharpInvocationResolveResult;

                if (rr.Member != null)
                {
                    IMember m = rr.Member;

                    text = "method " + m.ReturnType.Name + " " + m.FullName + "(";


                    if (m.GetType() == typeof(DefaultResolvedMethod))
                    {
                        DefaultResolvedMethod dd = (DefaultResolvedMethod)m;

                        if (dd.IsConstructor == true)

                            text = "method " + dd.ReturnType.Name + " " + r.Type.FullName + "(";
                    }



                    if (rr.Arguments != null)
                    {
                        int i = 0;
                        foreach (ResolveResult re in rr.Arguments)
                        {
                            if (i == 0)
                                text += re.Type.Name + " " + "method";
                            else
                                text += ", " + re.Type.Name + " " + "method";
                            i++;
                        }
                    }

                    text += ")";

                    string doc = "";

                    if (m.Documentation != null)
                    {
                        doc = m.Documentation.ToString().Replace("\n", "");
                        doc = doc.Replace("<summary>", "");
                        doc = doc.Replace("</summary>", "");
                    }
                    else
                    {
                        if (m.ParentAssembly != null)
                            if (m.ParentAssembly.AssemblyName != null)
                                doc = GACForm.GetFrameworkDoc(m.ParentAssembly.AssemblyName, m.FullName);
                        doc = doc.Replace("<summary>", "");
                        doc = doc.Replace("</summary>", "");
                    }

                    if (doc != "")
                        text += "\n" + doc;

                    return text;
                }
                else
                {
                    text = rr.Type.FullName + "(";

                    if (rr.Arguments != null)
                    {
                        int i = 0;
                        foreach (ResolveResult re in rr.Arguments)
                        {
                            if (i == 0)
                                text += re.Type.Name + " " + "method";
                            else
                                text += ", " + re.Type.Name + " " + "method";
                            i++;
                        }

                        text += ")";
                    }

                    return text;
                }
                // else return r.ToString();


            }
            else

            if (r.GetType() == typeof(TypeResolveResult))
            {
                TypeResolveResult rr = r as TypeResolveResult;

                DefaultResolvedTypeDefinition dtd = null;

                string doc = "";

                if (rr.Type.GetType() == typeof(DefaultResolvedTypeDefinition))
                {
                    dtd = (DefaultResolvedTypeDefinition)rr.Type;

                    if (dtd.Documentation != null)
                    {
                        doc = dtd.Documentation.ToString().Replace("\n", "");
                        doc = doc.Replace("<summary>", "");
                        doc = doc.Replace("</summary>", "");
                    }
                    else
                    {
                        if (dtd.ParentAssembly != null)
                            if (dtd.ParentAssembly.AssemblyName != null)
                                doc = GACForm.GetFrameworkDoc(dtd.ParentAssembly.AssemblyName, dtd.FullName, "T:");
                        doc = doc.Replace("<summary>", "");
                        doc = doc.Replace("</summary>", "");
                    }

                    //if(dtd.ParentAssembly != null)
                    //    if (dtd.ParentAssembly.AssemblyName != null)
                    //        if (GACForm.dicts.ContainsKey(dtd.ParentAssembly.AssemblyName) == true)
                    //{
                    //   // MessageBox.Show("Assembly found");

                    //    string file = GACForm.dicts[dtd.ParentAssembly.AssemblyName] as string;

                    //    string docuPath = file.Substring(0, file.LastIndexOf(".")) + ".XML";

                    //    string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

                    //    string s = "Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5.2";

                    //    string filename = programFilesX86 + "\\" + s + "\\" + Path.GetFileName(docuPath);

                    //    if (File.Exists(filename))
                    //    {
                    //        XmlDocument dd = new XmlDocument();
                    //        dd.Load(filename);
                    //        XmlElement matchedElement = null;

                    //        foreach (XmlElement xmlElement in dd["doc"]["members"])
                    //        {
                    //           // if(xmlElement.Attributes.Count > 0)
                    //            if (xmlElement.Attributes["name"].Value.Equals("T:" + dtd.FullName))
                    //            {

                    //                matchedElement = xmlElement;

                    //                doc = matchedElement.InnerText;
                    //            }
                    //        }



                    //     }

                    //   }
                    //   }
                }


                if (rr.Type.Kind == TypeKind.Enum)
                {
                    text = "enum " + rr.Type.FullName;

                    if (dtd != null)
                        text += "\n" + doc;

                    return text;
                }
                else if (rr.Type.Kind == TypeKind.Struct)
                {
                    text = "struct " + rr.Type.FullName;

                    if (dtd != null)
                        text += "\n" + doc;

                    return text;
                }
                else if (rr.Type.Kind == TypeKind.Delegate)
                {
                    text = "delegate " + rr.Type.FullName;

                    if (dtd != null)
                        text += "\n" + doc;

                    return text;
                }
                else if (rr.Type.Kind == TypeKind.Interface)
                {
                    text = "interface " + rr.Type.FullName;

                    if (dtd != null)
                        text += "\n" + doc;

                    return text;
                }

                else if (rr.Type.Kind == TypeKind.Class)
                {
                    text = "class " + rr.Type.FullName;

                    if (dtd != null)
                        text += "\n" + doc;

                    return text;
                }

                else return r.Type.FullName;
            }
            else if (r.GetType() == typeof(MemberResolveResult))
            {
                MemberResolveResult rr = r as MemberResolveResult;



                IMember m = rr.Member;

                string prefix = "";

                if (m.SymbolKind == SymbolKind.Property)
                {
                    prefix = "P";

                    IProperty ip = (IProperty)m;


                    string getter = "";
                    string setter = "";

                    if (ip.Getter != null)
                        getter = "get;";
                    if (ip.Setter != null)
                        setter = "set;";

                    text = "property " + ip.ReturnType.Name + " " + ip.DeclaringType.Name + "." + ip.Name;

                    if (getter == "" && setter == "")
                        return text;

                    text += " { " + getter + " " + setter + "} ";

                    if (m.Documentation != null)
                        text += "\n" + m.Documentation.ToString().Replace("<summary>", "");



                    return text;
                }
                else if (m.SymbolKind == SymbolKind.Field)
                {
                    prefix = "F";

                    IField ip = (IField)m;

                    text = "field " + ip.ReturnType.Name + " " + ip.DeclaringType.Name + "." + ip.Name;

                    if (m.Documentation != null)
                        text += "\n" + m.Documentation.ToString().Replace("<summary>", "");

                    return text;
                }
                else if (m.SymbolKind == SymbolKind.Event)
                {
                    prefix = "E";

                    IEvent ip = (IEvent)m;

                    text = "event " + ip.ReturnType.Name + " " + ip.DeclaringType.Name + "." + ip.Name;

                    if (m.Documentation != null)
                        text += "\n" + m.Documentation.ToString().Replace("<summary>", "");

                    return text;
                }
                else if (m.SymbolKind == SymbolKind.Method)
                {
                    prefix = "M";

                    IMethod ip = (IMethod)m;

                    text = "method " + ip.ReturnType.Name + " " + ip.DeclaringType.Name + "." + ip.Name + "(";

                    int i = 0;
                    foreach (IParameter p in ip.Parameters)
                    {
                        if (i == 0)
                            text += p.Type.Name + " " + p.Name;
                        else
                            text += ", " + p.Type.Name + " " + p.Name;
                        i++;
                    }

                    text += ")";
                    if (m.Documentation != null)
                        text += "\n" + m.Documentation.ToString().Replace("<summary>", "");

                    return text;
                }
                else if (m.SymbolKind == SymbolKind.Constructor)
                {
                    prefix = "M";

                    //IMethod ip = (IMethod)m;

                    DefaultResolvedMethod ip = (DefaultResolvedMethod)m;

                    //text =  " " + ip.DeclaringType.Name + "." + ip.Name + "(";

                    text = "method " + ip.ReturnType.Name + " " + r.Type.FullName + "(";

                    int i = 0;
                    foreach (IParameter p in ip.Parameters)
                    {
                        if (i == 0)
                            text += p.Type.Name + " " + p.Name;
                        else
                            text += ", " + p.Type.Name + " " + p.Name;
                        i++;
                    }

                    text += ")";


                    return text;
                }
                else return r.ToString();
            }
            if (r.GetType() == typeof(LocalResolveResult))
            {
                LocalResolveResult rr = (LocalResolveResult)r;

                if (rr.IsParameter)
                {
                    text = "parameter " + rr.Type.Name + " " + rr.GetSymbol().Name;
                    return text;
                }
                else
                if (rr.Variable != null)
                {
                    text = "local-variable " + rr.Type.Name + " " + rr.GetSymbol().Name;

                    return text;
                }

                else

                    return r.ToString();
            }



            else if (r.GetType() == typeof(ConversionResolveResult) || r.GetType().FullName == "ICSharpCode.NRefactory.CSharp.Resolver.CastResolveResult")
            {
                ConversionResolveResult rr = (ConversionResolveResult)r;


                if (rr.GetSymbol() != null)
                {
                    if (rr.GetSymbol().SymbolKind == SymbolKind.Event)
                    {
                        text = "event " + rr.Type.Name + " " + rr.GetSymbol().Name;
                        return text;
                    }
                }

                {
                    if (rr.Type.Kind == TypeKind.Delegate)
                    {
                        text = "delegate " + rr.Type.FullName + "( ";
                        if (rr.Conversion.Method != null)
                        {
                            int i = 0;
                            foreach (IParameter p in rr.Conversion.Method.Parameters)
                            {
                                if (i == 0)
                                    text += p.Type.Name + " " + p.Name;
                                else
                                    text += ", " + p.Type.Name + " " + p.Name;
                                i++;
                            }

                            text += ")";
                        }

                        return text;
                    }







                    else

                        return r.ToString();
                }
            }

            return r.ToString();
        }


        public string SetText(ResolveResult r, string word)
        {
            string text = "";

            text = ResolveText(r);

            if (text == "")
            {
                if (d == null)
                    d = vp.GetTypeNames();
                if (d.ContainsKey(word))
                    text = word;
            }

            //if (text == label.Text)
            //    return;

            Graphics g = this.CreateGraphics();

            SizeF s = g.MeasureString(text, this.Font, 1000);

            string[] dd = text.Trim().Split("\n\r".ToCharArray());

            if (dd.Length == 1)
                this.Size = new Size((int)s.Width + 20 + 15 + 15, 30);
            else
                this.Size = new Size((int)s.Width + 20 + 15 + 15, 60);
            //label.Location = new Point(2, 2);

            //label.Size = new Size(this.Size.Width - 4, this.Size.Height - 4);

            //label.Text = text;

            texts = text;

            this.Refresh();

            return text;
        }
    }
}
