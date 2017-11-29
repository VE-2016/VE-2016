using AvalonEdit.Editor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ScriptControl.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using VSParsers;
using VSProvider;
using WeifenLuo.WinFormsUI.Docking;

namespace ScriptControl
{
    public partial class ScriptControl : UserControl
    {
        private ScriptLanguage _scriptLanguage;

        private IDictionary<string, object> _RemoteVariables = null;

        public event EventHandler Execute;

        public Bitmap bmp { get; set; }

        public string project { get; set; }

        public DockPanel dockContainer1;

        public static Breakpointer br { get; set; }

        //protected override void OnResize(EventArgs e)
        //{
        //    if (this.Parent != null)
        //        this.Parent.Dock = DockStyle.Fill;
        //    base.OnResize(e);
        //}

        //public void ResizeControl(EventArgs e)
        //{
        //    OnResize(e);
        //}

        protected virtual void OnExecute()
        {
            if (Execute != null)
            {
                Execute(this, null);
            }
        }

        public AvalonDocument AvalonDocumentToPreview { get; set; }

        public void SelectAll()
        {
            Document doc = this.dockContainer1.ActiveDocument as Document;

            if (doc == null)
                return;
        }

        public void SetStatusStrip(StatusStrip p)
        {
            // ScriptStatus = p;
        }

        public void DisplayStatusStrip(bool visible)
        {
            // ScriptStatus.Visible = false;
        }

        public void DisplayTopStrip(bool visible)
        {
            // toolStrip1.Visible = visible;
        }

        public void AvalonFileModified(EditorWindow ew)
        {
            foreach (IDockContent ve in dockContainer1.Documents)
            {
                if (!(ve is AvalonDocument))
                    continue;
                AvalonDocument av = ve as AvalonDocument;
                if (av.Editor.dv == ew)
                {
                    if (!av.Text.Contains("*"))
                        av.Text = av.Text + "*";
                    return;
                }
            }
        }

        public string GetCurrentSelection()
        {
            string text = "";

            if (LastActiveAvalonDocument == null)
                return text;

            text = LastActiveAvalonDocument.Editor.dv.GetSelectedText();

            return text;
        }

        private Form form { get; set; }

        public AvalonDocument LastActiveAvalonDocument = null;

        public ScriptControl(Form form, DockPanel dockPanel = null)
        {
            this.CreateControl();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            //InitializeComponent();
            this.form = form;
            //if (dockPanel == null)
            //dockContainer1 = dockPanel1;
            //else
            {
                dockContainer1 = dockPanel;
                // dockPanel1 = dockPanel;
            }

            string Theme = GetTheme();

            {
                if (Theme == "VS2012Light")

                    this.dockContainer1.Theme = new VS2012LightTheme();
                else this.dockContainer1.Theme = new VS2013BlueTheme();
            }

            _RemoteVariables = new Dictionary<string, object>();

            br = new Breakpointer();

            br.scr = this;

            hst = new History();

            this.BackColor = Color.White;

            VSSolution.OpenFile += VSSolution_OpenFile;

            dockContainer1.Layout += DockContainer1_SizeChanged;

            dockContainer1.ActiveDocumentChanged += DockContainer1_ActiveDocumentChanged;

            //VSSolution.Errors += VSSolution_Errors;
        }

        private void DockContainer1_SizeChanged(object sender, EventArgs e)
        {
            if (bounders != null)
                bounders.Invoke(dockContainer1.DocumentWindowBounds);
        }

        ~ScriptControl()
        {
        }

        public void Dispose()
        {
        }

        public event EventHandler<AvalonDocument> handler;

        private void DockContainer1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (LastActiveAvalonDocument != null)
            {
                LastActiveAvalonDocument.OnActivated(false);
            }
            if (AvalonDocumentPreview != null)
            {
                if (!dockContainer1.Documents.ToList().Contains(AvalonDocumentPreview))
                    AvalonDocumentPreview = null;
            }
            if (!(dockContainer1.ActiveDocument is AvalonDocument))
                return;

            LastActiveAvalonDocument = dockContainer1.ActiveDocument as AvalonDocument;

            LastActiveAvalonDocument.OnActivated(true);

            if (handler != null)
                handler(this, LastActiveAvalonDocument);
        }

        //private void VSSolution_Errors(object sender, OpenFileEventArgs e)
        //{
        //    //MessageBox.Show(" Errors found - " +  e.Errors.Count));
        //}

        private void VSSolution_OpenFile(object sender, OpenFileEventArgs e)
        {
            //MessageBox.Show("Open content for " + e.content);
            if (!string.IsNullOrEmpty(e.filename))
            {
                OpenDocuments(e.filename, vs, null, e);
            }
            else
            {
                if (e.symbol != null)
                    OpenDocumentsWithContentForPreview(e.content, vs, null, null, e.symbol);
            }
        }

        private System.Threading.Timer MouseMoveTimer { get; set; }

        private System.Threading.Timer updateTimer { get; set; }

        public AvalonDocument ShowFile(string FileName)
        {
            AvalonDocument doc = null;
            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is AvalonDocument)
                {
                    doc = docWin as AvalonDocument;
                    if (doc.FileName == FileName)
                    {
                        doc.Show(dockContainer1, DockState.Document);
                        return doc;
                    }
                }
            }
            // Not Found

            doc = OpenDocuments(FileName, (VSSolution)null);

            if (doc != null)
            {
                doc.Activate();
                doc.Focus();
            }
            return doc;
        }

        public List<DocumentForm> DocumentOpened()
        {
            List<DocumentForm> ns = new List<DocumentForm>();

            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is DocumentForm)
                {
                    ns.Add(docWin as DocumentForm);
                }
            }
            return ns;
        }

        public AvalonDocument FileOpened(string FileName, bool shouldactivate = true)
        {
            AvalonDocument doc = null;

            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is AvalonDocument)
                {
                    doc = docWin as AvalonDocument;
                    if (doc.FileName == FileName)
                    {
                        if (shouldactivate)
                        {
                            doc.Show(dockContainer1, DockState.Document);
                            doc.Activate();
                            doc.Focus();
                        }
                        return doc;
                    }
                }
            }
            return null;
        }

        public Document GetFileOpened(string FileName)
        {
            Document doc = null;
            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is Document)
                {
                    doc = docWin as Document;
                    if (doc.FileName == FileName)
                    {
                        //doc.Show(dockContainer1, DockState.Document);
                        //doc.Activate();
                        //doc.Focus();
                        return doc;
                    }
                }
            }
            return null;
        }

        public AvalonDocument GetActiveDocument(bool focused = false)
        {
            if (dockContainer1.ActiveDocument == null)
                return null;
            return dockContainer1.ActiveDocument as AvalonDocument;
        }

        private object obs = new object();

        public void Cut()
        {
        }

        public void Copy()
        {
        }

        public void Paste()
        {
        }

        public void HideAndRedraw()
        {
        }

        public void ShowAndRedraw()
        {
        }

        public event EventHandler activeDocument;

        private bool _activated = false;

        public Document AddDocument(string Name)
        {
            return AddDocument(Name, false);
        }

        public Document AddDocument(string Name, bool IsWebReference)
        {
            Document doc = new Document(form);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.HideOnClose = false;

            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.DockTop);
            //    Parser.ProjectParser.ParseProjectContents(Name, "");
            return doc;
        }

        public AvalonDocument AddGenericDocument(string Name)
        {
            AvalonDocument doc = new AvalonDocument(vs);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            return doc;
        }

        public static Func<Rectangle, string> bounders { get; set; }

        public void SetActiveDocument()
        {
        }

        public DocumentForm OpenDocumentForm()
        {
            DocumentForm doc = new DocumentForm(this);

            doc.Show(dockContainer1, DockState.Document);
            return doc;
        }

        private void ActivePane_IsActivatedChanged(object sender, EventArgs e)
        {
        }

        public WebForm OpenWebForm(string web, string url)
        {
            WebForm doc = new WebForm(this);
            //.NavigateTo(url);
            //DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);

            doc.Text = web;

            return doc;
        }

        public FileForm OpenFileForm(string file)
        {
            FileForm doc = new FileForm(this);

            doc.LoadFile(file);

            doc.Show(dockContainer1, DockState.Document);

            doc.Text = file;

            return doc;
        }

        public AvalonDocument OpenDocuments(string Name, VSProvider.VSSolution vs, CountdownEvent autoEvent = null, OpenFileEventArgs ofa = null/*ReferenceLocation? Location = null*/)
        {
            var d = FileOpened(Name);
            if (d != null)
            {
                d.Activate();
                return d;
            }
            this.SuspendLayout();
            AvalonDocument doc = new AvalonDocument(vs, Name);
            doc.Text = Path.GetFileName(Name);
            doc.FileName = Name;
            doc.ToolTipText = Name;
            doc.DockAlignment = DockAlign.DockToLeft;
            doc.Show(dockContainer1, DockState.Document);
            doc.Activate();

            dockContainer1.Refresh();

            if (vs != null)
                doc.Editor.dv.LoadFromProject(vs, false);

            this.ResumeLayout();

            if (autoEvent != null)
                autoEvent.Signal();

            if (ofa == null)
                return doc;

            if (!string.IsNullOrEmpty(ofa.Subtype))
                doc.DockHandler.Bitmap = ves.Service.GetBitmapFromSubtype(ofa.Subtype);
            if (ofa.Location != null)
            {
                int start = ofa.Location.Value.Location.SourceSpan.Start;
                int length = ofa.Location.Value.Location.SourceSpan.Length;
                doc.Editor.dv.SelectText(start, length);
            }
            else if (ofa.Span != null)
            {
                TextSpan? Span = ofa.Span;
                int start = Span.Value.Start;
                int length = ofa.Span.Value.Length;
                doc.Editor.dv.SelectText(start, length);
            }

            return doc;
        }

        private AvalonDocument AvalonDocumentPreview { get; set; }

        public AvalonDocument OpenDocumentsForPreview(string Name, VSProvider.VSSolution vs, CountdownEvent autoEvent = null, OpenFileEventArgs ofa = null/*ReferenceLocation? Location = null*/)
        {
            var d = FileOpened(Name);
            if (d != null)
            {
                d.Activate();
                //dockContainer1.Refresh();
                return d;
            }
            if (!File.Exists(Name))
                return null;
            AvalonDocument doc = AvalonDocumentPreview;
            if (doc != null)
            {
                doc.Text = Path.GetFileName(Name);
                doc.FileName = Name;
                doc.DockAlignment = DockAlign.DockToRight;
                doc.Editor.Load(Name);
                doc.Activate();
                //dockContainer1.Refresh();
            }
            else
            {
                this.SuspendLayout();
                doc = new AvalonDocument(vs, Name);
                doc.Text = Path.GetFileName(Name);
                doc.FileName = Name;
                doc.DockAlignment = DockAlign.DockToRight;
                doc.Show(dockContainer1, DockState.Document);
                doc.Activate();

                //dockContainer1.Refresh();
                this.ResumeLayout();
            }
            if (vs != null)
                doc.Editor.dv.LoadFromProject(vs, false);

            AvalonDocumentPreview = doc;

            if (autoEvent != null)
                autoEvent.Signal();

            if (ofa == null)
                return doc;
            if (!string.IsNullOrEmpty(ofa.Subtype))
                doc.DockHandler.Bitmap = ves.Service.GetBitmapFromSubtype(ofa.Subtype);
            if (ofa.Location != null)
            {
                int start = ofa.Location.Value.Location.SourceSpan.Start;
                int length = ofa.Location.Value.Location.SourceSpan.Length;
                doc.Editor.dv.SelectText(start, length);
            }
            else if (ofa.Span != null)
            {
                TextSpan? Span = ofa.Span;
                int start = Span.Value.Start;
                int length = ofa.Span.Value.Length;
                doc.Editor.dv.SelectText(start, length);
            }

            return doc;
        }

        public AvalonDocument OpenDocumentsWithContentForPreview(string content, VSProvider.VSSolution vs, CountdownEvent autoEvent = null, OpenFileEventArgs ofa = null/*ReferenceLocation? Location = null*/, ISymbol symbol = null)
        {
            var d = FileOpened(Name);
            if (d != null)
            {
                d.Activate();
                dockContainer1.Refresh();
                return d;
            }

            AvalonDocument doc = AvalonDocumentPreview;
            if (doc != null)
            {
                doc.Text = symbol.Name;
                doc.FileName = symbol.Name;
                doc.DockAlignment = DockAlign.DockToRight;
                doc.LoadText(content);
                doc.Activate();
                dockContainer1.Refresh();
            }
            else
            {
                this.SuspendLayout();
                doc = new AvalonDocument(vs);
                doc.Text = symbol.Name;
                doc.FileName = symbol.Name;
                doc.LoadText(content);
                doc.DockAlignment = DockAlign.DockToRight;
                doc.Show(dockContainer1, DockState.Document);
                doc.Activate();
                dockContainer1.Refresh();
                this.ResumeLayout();
            }
            if (vs != null)
                doc.Editor.dv.Dispatcher.BeginInvoke(new Action(() =>
                {
                    doc.Editor.dv.LoadFromProject(vs, true, symbol);
                    doc.Activate();
                    doc.Editor.Focus();
                    doc.Editor.dv.Focus();
                }));

            AvalonDocumentPreview = doc;

            if (autoEvent != null)
                autoEvent.Signal();

            if (ofa == null)
                return doc;

            if (ofa.Location != null)
            {
                int start = ofa.Location.Value.Location.SourceSpan.Start;
                int length = ofa.Location.Value.Location.SourceSpan.Length;
                doc.Editor.dv.SelectText(start, length);
            }
            else if (ofa.Span != null)
            {
                TextSpan? Span = ofa.Span;
                int start = Span.Value.Start;
                int length = ofa.Span.Value.Length;
                doc.Editor.dv.SelectText(start, length);
            }

            return doc;
        }

        public AvalonDocument OpenDocumentsWithContent(string content, VSProvider.VSSolution vs, AutoResetEvent autoEvent = null, ISymbol symbol = null)
        {
            var d = FileOpened(Name);
            if (d != null)
            {
                d.Activate();
                return d;
            }
            this.SuspendLayout();
            AvalonDocument doc = new AvalonDocument(vs);
            doc.LoadText(content);
            if (symbol != null)
            {
                doc.Text = symbol.Name;
                doc.FileName = symbol.Name;
            }
            doc.Activate();
            doc.Show(dockContainer1, DockState.Document);
            this.ResumeLayout();
            if (vs != null)
                doc.Editor.dv.Dispatcher.BeginInvoke(new Action(() =>
                {
                    doc.Editor.dv.LoadFromProject(vs, true, symbol);
                    doc.Activate();
                    doc.Editor.Focus();
                    doc.Editor.dv.Focus();
                }));
            return doc;
        }

        //public void BreakPointAdded(object sender, RowEventArgs e)
        //{
        //    VSProject vp = d.CodeEditor.vp;
        //    VSSolution vs = vp.vs;
        //    br.AddBreakpoint(d.FileName, new Point(0, e.Row.Index), vs.solutionFileName, vp.FileName);
        //}

        public Document OpenDocuments(string contents, string name, VSProject vp)
        {
            if (FileOpened(name) != null)
                return null;

            if (contents == string.Empty)
                return null;
            Document doc = new Document(form);
            doc.FileName = name;
            doc.Text = Path.GetFileNameWithoutExtension(name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;

            LoadKeywordsAsync(doc);

            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);

            return doc;
        }

        public Document OpenDocuments(string Name, VSProvider.VSProject pp, history hst)
        {
            if (FileOpened(Name) != null)
                return null;

            string contents = "";

            if (File.Exists(Name))

                contents = File.ReadAllText(Name);

            if (contents == string.Empty)
                return null;
            Document doc = new Document(form);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;

            doc.vp = pp;

            doc.LoadKeywords(doc);

            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);

            doc.ExpandAll();
            doc.TabPageContextMenu = new ContextMenu();
            doc.TabPageContextMenu.MenuItems.Add(new MenuItem("test for context menu"));

            return doc;
        }

        public AvalonDocument OpenDocuments2(string Name, VSProvider.VSProject pp, ICSharpCode.NRefactory.TypeSystem.DomRegion dr, bool selectable)
        {
            AvalonDocument doc = null;

            doc = FileOpened(Name);

            //if (doc == null)
            //{
            //    string contents = "";

            //    contents = File.ReadAllText(Name);

            //    if (contents == string.Empty)
            //        return null;
            //    doc = new Document(form);
            //    doc.FileName = Name;
            //    doc.Text = Path.GetFileNameWithoutExtension(Name);
            //    doc.Tag = "USERDOCUMENT";
            //    doc.HideOnClose = false;
            //    doc.ScriptLanguage = _scriptLanguage;
            //    doc.vp = pp;
            //    doc.Contents = contents;
            //    doc.LoadVSProject(pp, Name);
            //    LoadKeywordsAsync(doc);
            //    DocumentEvents(doc, true);
            //    doc.Show(dockContainer1, DockState.Document);

            //    doc.ParseContentsNow();
            //}

            //doc.Editor.GotoLine(dr.BeginLine);
            //if (selectable)
            //{
            //    int offset = doc.Editor.Caret.GetOffset(dr.BeginColumn, dr.BeginLine);
            //    doc.Editor.GotoLine(dr.BeginColumn); //doc.Editor.Caret.Position = new TextPoint(dr.BeginColumn, dr.BeginLine);
            //    SelectText(offset, dr.EndColumn - dr.BeginColumn);
            //}

            return doc;
        }

        public AvalonDocument BookmarkAll(string Name, VSProvider.VSProject pp, ArrayList B)
        {
            AvalonDocument doc = null;

            doc = FileOpened(Name);

            //if (doc == null)
            //{
            //    string contents = "";

            //    contents = File.ReadAllText(Name);

            //    if (contents == string.Empty)
            //        return null;
            //    doc = new Document(form);
            //    doc.FileName = Name;
            //    doc.Text = Path.GetFileNameWithoutExtension(Name);
            //    doc.Tag = "USERDOCUMENT";
            //    doc.HideOnClose = false;
            //    doc.ScriptLanguage = _scriptLanguage;
            //    doc.vp = pp;
            //    doc.Contents = contents;
            //    doc.LoadVSProject(pp, Name);
            //    LoadKeywordsAsync(doc);
            //    DocumentEvents(doc, true);
            //    doc.Show(dockContainer1, DockState.Document);

            //    doc.ParseContentsNow();
            //}

            //foreach (Tuple<string, int, int> r in B)
            //{
            //    doc.syntaxDocument1[r.Item3].Bookmarked = true;
            //}

            return doc;
        }

        public void BookmarkAll(VSProvider.VSProject pp, ArrayList B)
        {
            foreach (Tuple<string, int, int> tt in B)
            {
                Bookmark(tt.Item1, pp, tt.Item3);
            }
        }

        public delegate void starter(Document doc);

        public void LoadKeywordsAsync(Document doc)
        {
            //BeginInvoke(new Action(() => { doc.LoadKeywords(doc); }));
            starter start = Keywords;
            start.BeginInvoke(doc, NewProcess, "null");
        }

        public void NewProcess(IAsyncResult ar)
        {
        }

        private object _obs = new object();

        private object _ob = new object();

        public ArrayList DQ = new ArrayList();

        private bool _block = false;

        public void Keywords(Document d)
        {
            d.state = Document.State.waiting;

            DQ.Add(d);

            lock (_obs)
            {
                while (_block)
                {
                    Monitor.Wait(_obs, 500);
                    if (DQ.IndexOf(d) < 0)
                    {
                        d.state = Document.State.none;
                        return;
                    }
                }
                _block = true;
            }

            lock (_ob)
            {
                int dd = DQ.IndexOf(d);
                if (dd < 0)
                {
                    d.state = Document.State.none;
                    return;
                }
                d.state = Document.State.working;
                d.LoadKeywords(d);

                DQ.Remove(dd);
                d.state = Document.State.none;

                _block = false;
                Monitor.Pulse(_ob);
            }
        }

        public AvalonDocument Bookmark(string Name, VSProvider.VSProject pp, int l2)
        {
            AvalonDocument doc = null;

            doc = FileOpened(Name);

            //if (doc == null)
            //{
            //    string contents = "";

            //    contents = File.ReadAllText(Name);

            //    if (contents == string.Empty)
            //        return null;
            //    doc = new Document(form);
            //    doc.FileName = Name;
            //    doc.Text = Path.GetFileNameWithoutExtension(Name);
            //    doc.Tag = "USERDOCUMENT";
            //    doc.HideOnClose = false;
            //    doc.ScriptLanguage = _scriptLanguage;
            //    doc.vp = pp;
            //    doc.Contents = contents;
            //    doc.LoadVSProject(pp, Name);
            //    doc.LoadKeywords(doc);
            //    DocumentEvents(doc, true);
            //    doc.Show(dockContainer1, DockState.Document);

            //    doc.ParseContentsNow();
            //}

            //doc.syntaxDocument1[l2].Bookmarked = true;

            return doc;
        }

        public Document OpenDocument(string contents, VSProvider.VSProject pp)
        {
            if (contents == string.Empty)
                return null;
            Document doc = new Document(form);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = true;

            doc.vp = pp;

            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.DockTop);

            doc.Focus();

            return doc;
        }

        private void DocumentEvents(Document doc, bool Enable)
        {
        }

        public event EventHandler<EventArgs> HistoryChange;

        private void Comments_Change(object sender, EventArgs e)
        {
            Comment();
        }

        private void UnComments_Change(object sender, EventArgs e)
        {
            UnComment();
        }

        private void Selection_Change(object sender, EventArgs e)
        {
            UpdateCutCopyToolbar();
        }

        public ToolStripStatusLabel label { get; set; }

        public ToolStripStatusLabel labels { get; set; }

        public void SetToolStripLabel(ToolStripStatusLabel b, ToolStripStatusLabel c)
        {
            label = b;
            labels = c;
        }

        private void doc_CaretChange(object sender, EventArgs e)
        {
            //string text = "";
            //UpdateCutCopyToolbar();
            //string file = ((Document)sender).FileName;
            //Caret c = ((Document)sender).Editor.ActiveViewControl.Caret;

            //if (c.CurrentRow != null)
            //    text = c.CurrentRow.Text;
            ////tCursorPos.Text = "Ln " + c.Position.Y + ", Col " + c.Position.X;
            //if (label != null)
            //{
            //    label.Text = "Ln " + c.Position.Y;
            //    labels.Text = "Col " + c.Position.X;
            //}
            //bool changed = hst.AddIfShould(file, c.Position.Y, text);
            //if (HistoryChange != null)
            //    HistoryChange(this, new EventArgs());
        }

        private void doc_TextChange(object sender, EventArgs e)
        {
            Document doc = ((Document)sender);

            if (doc != null)
                if (doc.Text != null)
                    if (doc.Text.EndsWith("*") == false)
                        doc.Text += "*";

            doc.ReadyToSave = true;
        }

        public void OpenHistory(history H)
        {
            string file = H.file;

            VSProject vp = vs.GetProjectbyCompileItem(file);

            OpenDocuments(file, vp, H);
        }

        private readonly object _syncRoot = new Object();

        #region ClickHandlers

        private void cNetToolStripMenuItemCSharp_Click(object sender, EventArgs e)
        {
        }

        private void vBNetToolStripMenuItemVbNet_Click(object sender, EventArgs e)
        {
        }

        private void tsbCut_Click(object sender, EventArgs e)
        {
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
        }

        public void SelectTextXY(AvalonDocument doc, int X, int Y)
        {
            //CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            //if (view != null)
            {
                doc.Editor.dv.SetAllFoldings();

                int p = Y - 10;
                if (p < 0)
                    p = 0;

                doc.Editor.dv.SelectText(X, Y);

                //doc.Editor.dv.  ExpandAll();

                //view.Refresh();

                //view.ScrollIntoView(p);

                //if (view.Document.Count <= Y)
                //    Task.Delay(1000);

                //if (view.Document.Count <= Y)
                //    return;

                //length = view.Document[Y].Text.Length - X;

                //int offset = view.Caret.GetOffset(X, Y);
                ////int s = view.GetLineIndex(start);
                //view.Selection.SelStart = offset;
                //view.Selection.SelLength = length;
            }
        }

        private void tsbBuild_Click(object sender, EventArgs e)
        {
        }

        private void tsbRun_Click(object sender, EventArgs e)
        {
            OnExecute();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            this.OnSave(sender, e);
        }

        private void tsbNew_Click(object sender, EventArgs e)
        {
        }

        #endregion ClickHandlers

        #region Private Members

        private void UpdateCutCopyToolbar()
        {
        }

        #endregion Private Members

        public void Undo()
        {
        }

        public void Redo()
        {
        }

        public void Comment()
        {
        }

        private void tsbComment_Click(object sender, EventArgs e)
        {
        }

        public void UnComment()
        {
        }

        private void tsbUnComment_Click(object sender, EventArgs e)
        {
        }

        private void tsbSolutionExplorer_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }

        public void OnSave(object sender, EventArgs e)
        {
        }

        public string Save()
        {
            AvalonDocument doc = dockContainer1.ActiveDocument as AvalonDocument;
            if (doc == null)
                return "";
            doc.Save();
            doc.Text = doc.Text.Replace("*", "");
            dockContainer1.Refresh();
            return doc.FileName;
        }

        public void Save(Document ddd)
        {
        }

        public SourceControl GetSourceControl()
        {
            SourceControl sourceControl = hst.sourceControl;
            if (sourceControl == null)
            {
                sourceControl = new SourceControl();
                hst.sourceControl = sourceControl;
            }
            return sourceControl;
        }

        public void SaveAll()
        {
            foreach (IDockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(Document))
                    Save((Document)doc);
            }

            dockContainer1.Refresh();
        }

        public History hst { get; set; }

        public event EventHandler ProjectProperties;

        public void ReloadSettings(bool forall, Settings settings)
        {
            settings.HighLightedLineColor = Color.White;
            foreach (Document doc in dockContainer1.Documents)
            {
                //doc.Editor.LoadSettings(settings);
            }

            dockContainer1.Refresh();
        }

        public string Theme = "";

        public VSSolution vs { get; set; }

        public static Settings settings { get; set; }

        public void SaveSolution()
        {
            if (hst == null)
                hst = new History();

            hst.settings = settings;

            hst.breakpointer = br.Serialize();

            hst.acts = new ArrayList();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(Document))
                {
                    Document d = doc as Document;

                    hst.acts.Add(d.FileName);
                }
                else if (doc.GetType() == typeof(DocumentForm))
                {
                    DocumentForm d = doc as DocumentForm;
                    if (d.resource != "" && d.resource != null)
                        hst.acts.Add("@" + d.FileName + "@" + d.resource);
                    else hst.acts.Add("@" + d.FileName);
                }
                else if (doc.GetType() == typeof(AvalonDocument))
                {
                    AvalonDocument d = doc as AvalonDocument;

                    hst.acts.Add(d.FileName);
                }
            }
            hst.Theme = settings.Theme;
            if (vs != null)
                if (vs.MainVSProject != null)
                    hst.MainProjectFile = vs.MainVSProject.FileName;

            hst.Serialize("settings.suo");
        }

        public string GetTheme()
        {
            hst = History.Deserialize("settings.suo");

            if (hst == null)
                return "";

            if (settings != null)
                settings.Theme = hst.Theme;

            return hst.Theme;
        }

        public void SetTheme(string theme)
        {
            Theme = theme;
        }

        public static void LoadSettings()
        {
            History hst = History.Deserialize("settings.suo");

            if (hst != null)
                if (hst.settings != null)

                    ScriptControl.settings = hst.settings;
        }

        async public Task<int> LoadFromProjectAsync(VSSolution vs)
        {
            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(AvalonDocument))
                {
                    AvalonDocument d = doc as AvalonDocument;
                    if (d.Editor.dv != null)
                        d.Editor.dv.LoadFromProjectAsync(vs);
                }
            }
            return 0;
        }

        public ArrayList LoadSolutionFiles(VSSolution vs, TreeView v)
        {
            ArrayList L = new ArrayList();

            hst = History.Deserialize("settings.suo");

            if (hst != null)
                if (hst.settings != null)

                    settings = hst.settings;

            if (hst == null)
            {
                hst = new History();
                this.hst = hst;
            }

            if (hst.acts == null)
                return L;

            Breakpointer b = Breakpointer.Deserialize(hst.breakpointer);

            br.Load(b);

            br.scr = this;

            this.SuspendLayout();

            int i = 0;

            foreach (string s in hst.acts)
            {
                try
                {
                    if (s.StartsWith("@") == true)
                    {
                        if (GetOpenDocumentForm(s.Replace("@", "")) != null)
                            continue;
                        string sa = s.Substring(1);
                        //VSProject p = vs.GetVSProject(sa);
                        //DocumentForm df = OpenDocumentForm();
                        //df.TabText = Path.GetFileName(sa);
                        //df.FileName = sa;
                        //df.Show();

                        //ProjectProperties(df, null);
                    }
                    else
                    if (s.StartsWith("@") == false)
                    {
                        var d = FileOpened(s);
                        if (d != null)
                        {
                            continue;
                        }
                        //VSProject p = vs.GetVSProject(s);
                        AvalonDocument doc = new AvalonDocument(vs, s);
                        doc.Text = Path.GetFileName(s);
                        doc.ToolTipText = s;
                        ProjectItemInfo p = ProjectItemInfo.ToProjectItemInfo(v, s);
                        if (p != null)
                        {
                            var subtype = p.SubTypes();
                            doc.DockHandler.Bitmap = ves.Service.GetBitmapFromSubtype(subtype);
                        }
                        //if (vs != null)
                        //    doc.Editor.dv.LoadFromProject(vs, false);
                        doc.Show(dockContainer1, DockState.Document);
                        dockContainer1.Refresh();
                    }

                    L.Add(s);
                }
                catch (Exception e) { }

                // this.BeginInvoke(new Action(() =>
                //{
                //if (vs != null)
                //    if (hst.MainProjectFile != null)
                //        vs.MainVSProject = vs.GetProjectbyFileName(hst.MainProjectFile);
                // }));

                //this.vs = vs;
            }

            br.LoadBreakpoints();

            hst.acts.Clear();

            this.ResumeLayout();

            return L;
        }

        public static ImageList AutoListImages { get; set; }

        public ArrayList LoadSolution(VSSolution vs)
        {
            ArrayList L = new ArrayList();

            hst = History.Deserialize("settings.suo");

            if (hst != null)
                if (hst.settings != null)

                    settings = hst.settings;

            if (hst == null)
            {
                hst = new History();
                this.hst = hst;
            }

            if (hst.acts == null)
                return L;

            Breakpointer b = Breakpointer.Deserialize(hst.breakpointer);

            br.Load(b);

            br.scr = this;

            this.SuspendLayout();

            int i = 0;

            foreach (string s in hst.acts)
            {
                try
                {
                    if (s.StartsWith("@") == true)
                    {
                        if (GetOpenDocumentForm(s.Replace("@", "")) != null)
                            continue;
                        string sa = s.Substring(1);
                        VSProject p = vs.GetVSProject(sa);
                        DocumentForm df = OpenDocumentForm();
                        df.TabText = Path.GetFileName(sa);
                        df.FileName = sa;
                        df.Show();

                        ProjectProperties(df, null);
                    }
                    else
                    if (s.StartsWith("@") == false)
                    {
                        VSProject p = vs.GetVSProject(s);
                        AvalonDocument doc = OpenDocuments(s, vs);
                        dockContainer1.Refresh();
                    }

                    L.Add(s);
                }
                catch (Exception e) { }

                this.BeginInvoke(new Action(() =>
                {
                    if (vs != null)
                        if (hst.MainProjectFile != null)
                            vs.MainVSProject = vs.GetProjectbyFileName(hst.MainProjectFile);
                }));

                this.vs = vs;
            }

            br.LoadBreakpoints();

            hst.acts.Clear();

            this.ResumeLayout();

            return L;
        }

        private object objects = new object();

        public void ClearHistory()
        {
            //lock (objects)
            {
                string folder = AppDomain.CurrentDomain.BaseDirectory;

                if (File.Exists(folder + "\\folder + settings.suo") == true)
                    File.Delete(folder + "\\folder + settings.suo");

                FileStream fs = File.Create(folder + "\\folder + settings.suo");

                fs.Close();

                ArrayList L = new ArrayList();

                // dockContainer1.RemovePanes();

                foreach (IDockContent dc in dockContainer1.Documents)
                {
                    if (dc.GetType() == typeof(Document))
                    {
                        Document doc = dc as Document;
                        L.Add(doc.Pane);
                    }
                }
                foreach (DockPane doc in L)
                {
                    //doc.CloseActiveContent();

                    //dockContainer1.RemovePane(doc);
                }

                dockContainer1.Refresh();

                hst = new History();
            }
        }

        public void CloseAll()
        {
            ArrayList L = new ArrayList();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(AvalonDocument))
                {
                    AvalonDocument d = doc as AvalonDocument;
                    //d.syntaxDocument1.Dispose();
                    //d.Editor.Document.Parser.Language.BlockTypeDispose();
                    //d.Editor.Dispose();

                    d.Editor.dv.Dispose();
                    d.Editor.dv = null;
                    d.Editor.Dispose();

                    L.Add(d/*.Pane*/);
                }
                else if (doc.GetType() == typeof(DocumentForm))
                {
                    DocumentForm d = doc as DocumentForm;
                    L.Add(d/*.Pane*/);
                }
                //dockContainer1.Refresh();
            }
            //MessageBox.Show("Found AvalonDocuments " + L.Count.ToString());
            foreach (DockContent doc in L)
            {
                doc.Close();

                doc.Dispose();
            }

            dockContainer1.Refresh();

            hst = new History();

            if (vs == null)
                return;

            foreach (VSProject p in vs.Projects)
            {
                if (VSParsers.CSParsers.df != null)
                    VSParsers.CSParsers.df.Clear();
                if (VSParsers.CSParsers.dd != null)
                    VSParsers.CSParsers.dd.Clear();
            }
            if (VSProject.dc != null)
                VSProject.dc.Clear();
            if (VSProject.dcc != null)
                VSProject.dcc.Clear();

            GC.Collect();
        }

        public List<string> GetOpenFiles()
        {
            List<string> list = new List<string>();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(AvalonDocument))
                    list.Add(((AvalonDocument)doc).FileName);
            }

            return list;
        }

        public ArrayList GetOpenDocuments()
        {
            ArrayList L = new ArrayList();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(AvalonDocument))
                    L.Add(((AvalonDocument)doc));
            }

            return L;
        }

        public DocumentForm GetOpenDocumentForm(string name)
        {
            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(DocumentForm))
                    if (doc.Name == name)
                        return (DocumentForm)doc;
            }

            return null;
        }

        public void LoadErrors(ArrayList DD)
        {
        }
    }

    public enum ScriptLanguage
    {
        CSharp,
        VBNET,
        XML
    }

    [Serializable]
    public class History
    {
        public ArrayList hst { get; set; }

        public ArrayList acts { get; set; }

        public string Theme { get; set; }

        public string MainProjectFile { get; set; }

        public Settings settings { get; set; }

        public string breakpointer { get; set; }

        public string Modules { get; set; }

        public DataSourceItems dataSources { get; set; }

        public SourceControl sourceControl { get; set; }

        public History()
        {
            hst = new ArrayList();
            acts = new ArrayList();
            dataSources = new DataSourceItems();
            Theme = "";
        }

        public void Insert(history H)
        {
            hst.Add(H);
        }

        public bool ShouldBeAdded()
        {
            return false;
        }

        public bool AddIfShould(string file, int line, string text)
        {
            if (hst.Count > 30)
            {
                hst.RemoveRange(20, 10);
            }

            if (hst.Count <= 0)
            {
                history H = new history();
                H.file = file;
                H.line = line;
                H.text = text;

                hst.Add(H);
                return true;
            }

            history HH = hst[0] as history;

            if (HH.file != file)
            {
                history H = new history();
                H.file = file;
                H.line = line;
                H.text = text;

                hst.Insert(0, H);

                return true;
            }
            else if (Math.Abs(HH.line - line) > 80)
            {
                history H = new history();
                H.file = file;
                H.line = line;
                H.text = text;

                hst.Insert(0, H);
                return true;
            }

            return false;
        }

        public void Serialize(string file)
        {
            dataSources.ToDictionary();

            // Persist to file
            string files = AppDomain.CurrentDomain.BaseDirectory + file;
            FileStream stream = File.Create(files);
            BinaryFormatter formatter = new BinaryFormatter();
            Console.WriteLine("Serializing files arraylist");
            formatter.Serialize(stream, this);
            stream.Close();
        }

        static public History Deserialize(string file)
        {
            History hst = null;

            string files = AppDomain.CurrentDomain.BaseDirectory + file;

            if (File.Exists(files) == false)
                return new History();

            FileStream stream = File.Open(files, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            //Console.WriteLine("DeSerializing files arraylist");
            try
            {
                hst = formatter.Deserialize(stream) as History;
            }
            catch (Exception e) { };
            stream.Close();

            return hst;
        }
    }

    [Serializable]
    public class DataSourceItem
    {
        public string name { get; set; }

        public string FileName { get; set; }

        public string TypeName { get; set; }

        public string Namespace { get; set; }

        public string AssemblyQualifiedName { get; set; }

        public string FullName { get; set; }

        [NonSerialized]
        public VSProject vp;

        [NonSerialized]
        public VSSolution vs;

        public string Dataset { get; set; }
    }

    [Serializable]
    public class DataSourceItems
    {
        [NonSerialized]
        public Dictionary<VSProject, ArrayList> dict;

        public Dictionary<string, ArrayList> dicts { get; set; }

        public DataSourceItems()
        {
            dict = new Dictionary<VSProject, ArrayList>();

            dicts = new Dictionary<string, ArrayList>();
        }

        public Dictionary<string, ArrayList> ToDictionary()
        {
            Dictionary<string, ArrayList> d = new Dictionary<string, ArrayList>();
            if (dict == null)
                return d;
            foreach (VSProject p in dict.Keys)
            {
                d.Add(p.Name, dict[p]);
            }
            dicts = d;
            return d;
        }

        public void FromDictionary(VSSolution vs)
        {
            dict = new Dictionary<VSProject, ArrayList>();
            Dictionary<string, ArrayList> d = dicts;
            if (vs == null)
                return;
            foreach (string s in d.Keys)
                if (d[s] != null)
                    dict.Add(vs.GetProjectbyName(s), d[s]);
        }
    }

    [Serializable]
    public class history
    {
        public int type = 0;

        public string file = "";

        public int line = 0;

        public string text = "";
    }

    public class Condition
    {
        public string name { get; set; }

        public object condition { get; set; }
    }

    public class Breakpoint : object
    {
        public string label { get; set; }

        public Point location { get; set; }

        public string file { get; set; }

        public string project { get; set; }

        public string solution { get; set; }

        public int HitCount { get; set; }

        public bool enabled = true;

        public ArrayList conditions { get; set; }

        public Breakpoint()
        {
            enabled = true;
        }

        public Breakpoint(string file, Point c, string solution, string project, string label = "")
        {
            this.enabled = true;
            this.file = file;
            this.location = c;
            this.solution = solution;
            this.project = project;
            this.label = label;
        }
    }

    public class Breakpoints
    {
        public string solution { get; set; }

        public string project { get; set; }

        public ArrayList breakpoints { get; set; }

        public Breakpoints()
        {
            breakpoints = new ArrayList();
        }

        public Breakpoint AddBreakpoint(string file, Point c, string solution, string project, string label = "")
        {
            Breakpoint b = new Breakpoint(file, c, solution, project, label);

            breakpoints.Add(b);

            return b;
        }
    }

    public class Breakpointer
    {
        public delegate void BreakPointEventHandler(object sender, Breakpoint e);

        public event BreakPointEventHandler breakPointEvent;

        public delegate void BreakPointStateEventHandler(object sender, Breakpoint e);

        public event BreakPointStateEventHandler breakPointStateEvent;

        [XmlIgnore]
        public ScriptControl scr { get; set; }

        public Breakpoints breakpoints { get; set; }

        public Breakpointer()
        {
            breakpoints = new Breakpoints();
        }

        public void AddBreakpoint(string file, Point c, string solution, string project, string label = "")
        {
            Breakpoint b = FindBreakpoint(c, file);

            if (b == null)
            {
                //b = new Breakpoint(file, c, solution, project, label);
                b = breakpoints.AddBreakpoint(file, c, solution, project, label);
            }
            else
            {
                RemoveBreakpoint(b);
            }

            if (breakPointEvent != null)
                breakPointEvent(this, b);
        }

        public Breakpoint FindBreakpoint(Point TextPosition, string file)
        {
            foreach (Breakpoint b in breakpoints.breakpoints)
            {
                if (b.location.Y == TextPosition.Y)
                    if (b.file == file)
                        return b;
            }
            return null;
        }

        public void RemoveBreakpoint(Breakpoint b)
        {
            breakpoints.breakpoints.Remove(b);
        }

        public void Load(Breakpointer b)
        {
            this.breakpoints = b.breakpoints;
        }

        public void LoadBreakpoint(Breakpoint b, int state, bool notify = true)
        {
            if (scr == null)
            {
                return;
            }

            Document d = scr.GetFileOpened(b.file);
            if (d == null)
            {
                return;
            }

            d.Focus();

            d.SetBreakpoint(b.location, state);

            if (notify)
                if (breakPointStateEvent != null)
                    breakPointStateEvent(this, null);
        }

        public void LoadBreakpoints(int state = 1, bool notify = true)
        {
            if (scr == null)
            {
                //MessageBox.Show("no file sc");
                return;
            }

            foreach (Breakpoint b in breakpoints.breakpoints)
            {
                Document d = scr.GetFileOpened(b.file);
                if (d == null)
                {
                    //MessageBox.Show("no file d");

                    continue;
                }

                if (state == 4)
                    b.enabled = true;
                else if (state == 3)
                    b.enabled = false;

                d.Focus();

                d.SetBreakpoint(b.location, state);
            }
            if (notify)
                if (breakPointStateEvent != null)
                    breakPointStateEvent(this, null);
        }

        public string Serialize()
        {
            Type[] types = { typeof(Breakpoint), typeof(Breakpoints) };

            XmlSerializer s = new XmlSerializer(typeof(Breakpointer), types);

            using (StringWriter textWriter = new StringWriter())
            {
                s.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        public static Breakpointer Deserialize(string xml)
        {
            if (xml == null)
                return new Breakpointer();

            Type[] types = { typeof(Breakpoint), typeof(Breakpoints) };

            XmlSerializer s = new XmlSerializer(typeof(Breakpointer), types);

            StringReader textReader = new StringReader(xml);
            return (Breakpointer)s.Deserialize(textReader);
        }
    }

    public enum SourceControlled
    {
        none,
        git,
        tfs,
        other
    }

    [Serializable]
    public class SourceControl
    {
        public SourceControl()
        {
            sourceControlled = SourceControlled.none;
        }

        public SourceControlled sourceControlled { get; set; }
    }

    [Serializable]
    public class Settings
    {
        public string ResumeAtStartup { get; set; }

        public string Theme { get; set; }

        private int _GutterMarginWidth = 0;

        public int GutterMarginWidth
        {
            get { return _GutterMarginWidth; }
            set { _GutterMarginWidth = value; }
        }

        private int _SmoothScrollSpeed = 2;

        public int SmoothScrollSpeed
        {
            get { return _SmoothScrollSpeed; }
            set { _SmoothScrollSpeed = value; }
        }

        private int _RowPadding = 0;

        public int RowPadding
        {
            get { return _RowPadding; }
            set { _RowPadding = value; }
        }

        private long _ticks = 0;

        public long Ticks
        {
            get { return _ticks; }
            set { _ticks = value; }
        }

        private bool _ShowWhitespace = false;

        public bool ShowWhitespace
        {
            get { return _ShowWhitespace; }
            set { _ShowWhitespace = value; }
        }

        private bool _ShowTabGuides = false;

        public bool ShowTabGuides
        {
            get { return _ShowTabGuides; }
            set { _ShowTabGuides = value; }
        }

        private bool _ShowLineNumbers = false;

        public bool ShowLineNumbers
        {
            get { return _ShowLineNumbers; }
            set { _ShowLineNumbers = value; }
        }

        private bool _ShowGutterMargin = true;

        public bool ShowGutterMargin
        {
            get { return _ShowGutterMargin; }
            set { _ShowGutterMargin = value; }
        }

        private bool _ReadOnly = false;

        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        private bool _HighLightActiveLine = true;

        public bool HighLightActiveLine
        {
            get { return _HighLightActiveLine; }
            set { _HighLightActiveLine = value; }
        }

        private bool _VirtualWhitespace = false;

        public bool VirtualWhitespace
        {
            get { return _VirtualWhitespace; }
            set { _VirtualWhitespace = value; }
        }

        private bool _BracketMatching = true;

        public bool BracketMatching
        {
            get { return _BracketMatching; }
            set { _BracketMatching = value; }
        }

        private bool _OverWrite = false;

        public bool OverWrite
        {
            get { return _OverWrite; }
            set { _OverWrite = value; }
        }

        private bool _ParseOnPaste = false;

        public bool ParseOnPaste
        {
            get { return _ParseOnPaste; }
            set { _ParseOnPaste = value; }
        }

        private bool _SmoothScroll = false;

        public bool SmoothScroll
        {
            get { return _SmoothScroll; }
            set { _SmoothScroll = value; }
        }

        private bool _AllowBreakPoints = true;

        public bool AllowBreakPoints
        {
            get { return _AllowBreakPoints; }
            set { _AllowBreakPoints = value; }
        }

        private bool _LockCursorUpdate = false;

        public bool LockCursorUpdate
        {
            get { return _LockCursorUpdate; }
            set { _LockCursorUpdate = value; }
        }

        private Color _BracketBorderColor = Color.DarkBlue;

        public Color BracketBorderColor
        {
            get { return _BracketBorderColor; }
            set { _BracketBorderColor = value; }
        }

        private Color _TabGuideColor = ControlPaint.Light(SystemColors.ControlLight);

        public Color TabGuideColor
        {
            get { return _TabGuideColor; }
            set { _TabGuideColor = value; }
        }

        private Color _OutlineColor = SystemColors.ControlDarkDark;

        public Color OutlineColor
        {
            get { return _OutlineColor; }
            set { _OutlineColor = value; }
        }

        private Color _WhitespaceColor = SystemColors.ControlDark;

        public Color WhitespaceColor
        {
            get { return _WhitespaceColor; }
            set { _WhitespaceColor = value; }
        }

        private Color _SeparatorColor = SystemColors.Control;

        public Color SeparatorColor
        {
            get { return _SeparatorColor; }
            set { _SeparatorColor = value; }
        }

        private Color _SelectionBackColor = SystemColors.Highlight;

        public Color SelectionBackColor
        {
            get { return _SelectionBackColor; }
            set { _SelectionBackColor = value; }
        }

        private Color _SelectionForeColor = SystemColors.HighlightText;

        public Color SelectionForeColor
        {
            get { return _SelectionForeColor; }
            set { _SelectionForeColor = value; }
        }

        private Color _InactiveSelectionBackColor = SystemColors.ControlDark;

        public Color InactiveSelectionBackColor
        {
            get { return _InactiveSelectionBackColor; }
            set { _InactiveSelectionBackColor = value; }
        }

        private Color _InactiveSelectionForeColor = SystemColors.ControlLight;

        public Color InactiveSelectionForeColor
        {
            get { return _InactiveSelectionForeColor; }
            set { _InactiveSelectionForeColor = value; }
        }

        private Color _BreakPointBackColor = Color.DarkRed;

        public Color BreakPointBackColor
        {
            get { return _BreakPointBackColor; }
            set { _BreakPointBackColor = value; }
        }

        private Color _BreakPointForeColor = Color.White;

        public Color BreakPointForeColor
        {
            get { return _BreakPointForeColor; }
            set { _BreakPointForeColor = value; }
        }

        private Color _BackColor = Color.White;

        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }

        private Color _HighLightedLineColor = Color.LightBlue;

        public Color HighLightedLineColor
        {
            get { return _HighLightedLineColor; }
            set { _HighLightedLineColor = value; }
        }

        private Color _GutterMarginColor = SystemColors.Control;

        public Color GutterMarginColor
        {
            get { return _GutterMarginColor; }
            set { _GutterMarginColor = value; }
        }

        private Color _LineNumberBackColor = SystemColors.Window;

        public Color LineNumberBackColor
        {
            get { return _LineNumberBackColor; }
            set { _LineNumberBackColor = value; }
        }

        private Color _LineNumberForeColor = Color.Teal;

        public Color LineNumberForeColor
        {
            get { return _LineNumberForeColor; }
            set { _LineNumberForeColor = value; }
        }

        private Color _GutterMarginBorderColor = SystemColors.ControlDark;

        public Color GutterMarginBorderColor
        {
            get { return _GutterMarginBorderColor; }
            set { _GutterMarginBorderColor = value; }
        }

        private Color _LineNumberBorderColor = Color.Teal;

        public Color LineNumberBorderColor
        {
            get { return _LineNumberBorderColor; }
            set { _LineNumberBorderColor = value; }
        }

        private Color _BracketForeColor = Color.Black;

        public Color BracketForeColor
        {
            get { return _BracketForeColor; }
            set { _BracketForeColor = value; }
        }

        private Color _BracketBackColor = Color.LightSteelBlue;

        public Color BracketBackColor
        {
            get { return _BracketBackColor; }
            set { _BracketBackColor = value; }
        }

        private Color _ScopeBackColor = Color.Transparent;

        public Color ScopeBackColor
        {
            get { return _ScopeBackColor; }
            set { _ScopeBackColor = value; }
        }

        private Color _ScopeIndicatorColor = Color.Transparent;

        public Color ScopeIndicatorColor
        {
            get { return _ScopeIndicatorColor; }
            set { _ScopeIndicatorColor = value; }
        }

        private string _FontName = "Courier New";

        public string FontName
        {
            get { return _FontName; }
            set { _FontName = value; }
        }

        private float _FontSize = 10f;

        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
    }
}