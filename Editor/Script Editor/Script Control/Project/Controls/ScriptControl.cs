using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.SyntaxFiles;
using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.Syntax;
using System.Threading;
using System.IO;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.Dom.NRefactoryResolver;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog;
//using AIMS.Libraries.Forms.Docking;
using WeifenLuo.WinFormsUI.Docking;
using AIMS.Libraries.Scripting.ScriptControl;

using WeifenLuo.WinFormsUI;

using VSProvider;


namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class ScriptControl : UserControl
    {
        private ScriptLanguage _scriptLanguage;
        private ErrorList _winErrorList = null;
        private Output _winOutput = null;
        private ProjectExplorer _winProjExplorer = null;
        private static IProject s_AIMSProject = null;
        private WeakReference _selectRefDialog = null;
        private IDictionary<string, object> _RemoteVariables = null;
        public event EventHandler Execute;

        public Bitmap bmp { get; set; }

        public string project { get; set; }

        public DockPanel dockContainer1;

        protected override void OnResize(EventArgs e)
        {
            //     if(dockContainer1 != null)   
            //     if (dockContainer1.ActiveContent != null)
            //         dockContainer1.ActivePane.PerformLayout();

            //if(dockContainer1 !=null)
            //this.dockContainer1.Size = new Size(this.Width, this.Height);
            base.OnResize(e);
        }

        public void ResizeControl(EventArgs e)
        {
            OnResize(e);
        }

        protected virtual void OnExecute()
        {
            if (Execute != null)
            {
                Execute(this, null);
            }
        }

        public void SelectAll()
        {
            Document doc = this.dockContainer1.ActiveDocument as Document;

            if (doc == null)
                return;

            doc.Editor.SelectAll();
        }

        public void SetStatusStrip(StatusStrip p)
        {
            ScriptStatus = p;
        }

        public void DisplayStatusStrip(bool visible)
        {
            ScriptStatus.Visible = false;
        }
        public void DisplayTopStrip(bool visible)
        {
            toolStrip1.Visible = visible;
        }

        public ScriptLanguage ScriptLanguage
        {
            get { return _scriptLanguage; }
            set
            {
                if (value == ScriptLanguage.CSharp)
                {
                    tsbSelectLanguage.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProject_CSCodefile;
                    tsbSelectLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
                }
                else
                {
                    tsbSelectLanguage.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProject_VBCodefile;
                    tsbSelectLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
                }
                ConvertToLanguage(_scriptLanguage, value);
                _scriptLanguage = value;
            }
        }

        private void ConvertToLanguage(ScriptLanguage OldLang, ScriptLanguage NewLanguage)
        {
            ResetParserLanguage(NewLanguage);
            //Disable On Change Event
            foreach (IDockContent docWin in dockContainer1.Contents)
            {
                Document doc = null;
                if (docWin is Document)
                {
                    doc = docWin as Document;
                    DocumentEvents(doc, false);
                    if (OldLang != NewLanguage)
                    {
                        doc.FileName = Path.GetFileNameWithoutExtension(doc.FileName) + (NewLanguage == ScriptLanguage.CSharp ? ".cs" : ".vb");
                        if (NewLanguage == ScriptLanguage.CSharp)
                        {
                            doc.ScriptLanguage = NewLanguage;
                            doc.Contents = Parser.ProjectParser.GetFileContents(doc.FileName);
                        }
                        else
                        {
                            doc.Contents = Parser.ProjectParser.GetFileContents(doc.FileName);
                            doc.ScriptLanguage = NewLanguage;
                        }
                    }

                    DocumentEvents(doc, true);
                }
            }
            //Enable On Change Event
            if (_winErrorList != null)
                _winErrorList.ConvertToLanguage(OldLang, NewLanguage);
            //_winProjExplorer.Language = NewLanguage;
        }

        public IDictionary<string, object> RemoteVariables
        {
            get { return _RemoteVariables; }
        }
        public string DefaultNameSpace
        {
            get { return s_AIMSProject.RootNamespace; }
        }

        public string DefaultClassName
        {
            get { return "Program"; }
        }

        public string StartMethodName
        {
            get { return "Main"; }
        }

        private string GetUserSrcCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("namespace " + this.DefaultNameSpace);
            sb.AppendLine("{");
            sb.AppendLine("\tpublic partial class " + this.DefaultClassName);
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tpublic int " + this.StartMethodName + "()");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t\treturn 0;");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }
        public string OutputAssemblyName
        {
            get { return s_AIMSProject.OutputAssemblyFullPath; }
        }
        private string GetAddObjectSrcCode()
        {
            //Add AddObject & ReturnCode stuff;
            StringBuilder sb = new StringBuilder();
            foreach (string key in _RemoteVariables.Keys)
            {
                object obj = null;
                if (_RemoteVariables.TryGetValue(key, out obj))
                {
                    sb.Append("\t\tpublic static ");
                    sb.Append(obj.GetType().BaseType.FullName);
                    sb.Append(" ");

                    sb.Append(key);
                    sb.Append(" = null;");
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public void AddObject(string Name, object Value)
        {
            try
            {
                _RemoteVariables.Add(Name, Value);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public string GetCurrentSelection()
        {
            string text = "";

            EditViewControl e = GetCurrentView();
            if (e != null)
            {
                text = e.Selection.Text;
                return text;
            }

            return text;
        }
        public TextPoint GetCurrentSelectionStart()
        {
            EditViewControl e = GetCurrentView();
            if (e != null)
            {
                TextPoint p = e.Document.IntPosToPoint(e.Selection.SelStart);
                return p;
            }

            return null;
        }

        public IMember GetCurrentBlock()
        {
            AIMS.Libraries.Scripting.ScriptControl.Document d = dockContainer1.ActiveDocument as AIMS.Libraries.Scripting.ScriptControl.Document;

            if (d == null)
                return null;

            IMember m = d.quickClassBrowserPanel.GetCurrentMember();



            return m;
        }

        public string GetCurrentBlockText(DomRegion r)
        {
            EditViewControl e = GetCurrentView();

            if (e == null)
                return "";

            string text = "";

            int i = r.BeginLine;
            while (i < r.EndLine)
            {
                text += e.Document[i].Text + "\n";


                i++;
            }



            return text;
        }

        private string GetSystemSrcCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#region System Generated Source Code.Please do not change ...");
            sb.AppendLine("namespace " + this.DefaultNameSpace);
            sb.AppendLine("{");
            sb.AppendLine("\tusing System;");
            sb.AppendLine("\tusing System.Collections.Generic;");
            sb.AppendLine("\tusing System.Diagnostics;");
            sb.AppendLine("\tusing System.Reflection;");
            sb.AppendLine("\tpublic partial class " + this.DefaultClassName + " : MarshalByRefObject, IRun");
            sb.AppendLine("\t{");
            sb.AppendLine(GetAddObjectSrcCode());
            sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
            sb.AppendLine("\t\tvoid IRun.Initialize(IDictionary<string, object> Variables)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tobject value = null;");
            sb.AppendLine("\t\t\t\ttry");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tVariables.TryGetValue(name, out value);");
            sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
            sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\tcatch(Exception ex)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tthrow ex;");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("");
            sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
            sb.AppendLine("\t\tobject IRun.Run(string StartMethod, params object[] Parameters)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\ttry");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tMethodInfo methodInfo = this.GetType().GetMethod(StartMethod,BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance);");
            sb.AppendLine("\t\t\t\treturn methodInfo.Invoke(this, Parameters);");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t\tcatch (Exception ex)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tthrow ex;");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("");
            sb.AppendLine("\t\t[DebuggerStepperBoundary()]");
            sb.AppendLine("\t\tvoid IRun.Dispose(IDictionary<string, object> Variables)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\tforeach (string name in Variables.Keys)");
            sb.AppendLine("\t\t\t{");
            sb.AppendLine("\t\t\t\tobject value = null; ;");
            sb.AppendLine("\t\t\t\ttry");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tFieldInfo fInfo = this.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);");
            sb.AppendLine("\t\t\t\t\tfInfo.SetValue(this, value);");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t\tcatch (Exception ex)");
            sb.AppendLine("\t\t\t\t{");
            sb.AppendLine("\t\t\t\t\tthrow ex;");
            sb.AppendLine("\t\t\t\t}");
            sb.AppendLine("\t\t\t}");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            sb.AppendLine("#endregion");
            return sb.ToString();
        }


        public void StartEditor()
        {
            AddRefrence(new ReferenceProjectItem(s_AIMSProject, "System"));
            AddRefrence(new ReferenceProjectItem(s_AIMSProject, "System.Windows.Forms"));
            Parser.ProjectParser.ParseProjectContents("Program.Sys.cs", this.GetSystemSrcCode());
            Document doc = this.AddDocument("Program.cs");
            doc.Contents = this.GetUserSrcCode();
            doc.ParseContentsNow();
            doc.Editor.ActiveViewControl.Caret.Position = new TextPoint(0, 1);
        }

        public void ShowFind()
        {
            Document doc = this.dockContainer1.ActiveDocument as Document;

            if (doc == null)
                return;

            doc.Editor.ShowFind();
        }

        public void ShowReplace()
        {
            Document doc = this.dockContainer1.ActiveDocument as Document;

            if (doc == null)
                return;

            doc.Editor.ShowReplace();
        }

        public ScriptControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            InitializeComponent();
            dockContainer1 = dockPanel1;
            toolStripContainer1.ContentPanel.Controls.Add(this.dockContainer1);
            this.dockContainer1.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.dockContainer1.Dock = System.Windows.Forms.DockStyle.Fill;


            string Theme = GetTheme();


            {
                if (Theme == "VS2012Light")

                    this.dockContainer1.Theme = new VS2012LightTheme();

                else this.dockContainer1.Theme = new VS2013BlueTheme();
            }



            InitilizeDocks();

            Parser.ProjectParser.Initilize(SupportedLanguage.CSharp);
            UpdateCutCopyToolbar();
            s_AIMSProject = new DefaultProject();
            _RemoteVariables = new Dictionary<string, object>();

            hst = new History();

            toolStripContainer1.RightToolStripPanelVisible = false;

            CodeEditorControl.IntErrors = new Intellisense();

            this.BackColor = Color.White;

            //if(CodeEditorControl.proxy.IsBbound() == false)
            //CodeEditorControl.proxy.OpenFile += Proxy_OpenFile;
        }

        public void Proxy_OpenFile(object sender, Proxy.OpenFileEventArgs e)
        {
            string file = e.filename;


            if (e.content != "")

                OpenDocuments(e.content, file, e.vp);

            else

                OpenDocuments2(e.filename, e.vp, e.dr, false);
        }

        private System.Threading.Timer updateTimer { get; set; }

        public void OpenNewFile(object state)
        {
            Proxy.OpenFileEventArgs e = state as Proxy.OpenFileEventArgs;


            this.Invoke(new Action(() => { OpenDocument(e.content, e.vp); }));
        }

        private void ResetParserLanguage(ScriptLanguage lang)
        {
            if (lang == ScriptLanguage.CSharp)
                Parser.ProjectParser.Language = SupportedLanguage.CSharp;
            else
                Parser.ProjectParser.Language = SupportedLanguage.VBNet;
        }

        private void InitilizeDocks()
        {
            ////Error List
            //_winErrorList = new ErrorList(this);
            //_winErrorList.Text = "Error List";
            //_winErrorList.Tag = "ERRORLIST";
            //_winErrorList.HideOnClose = true;
            //_winErrorList.Show(dockContainer1, DockState.DockBottomAutoHide);

            //ProjectExplorer
            //_winProjExplorer = new ProjectExplorer();
            //_winProjExplorer.Text = "Solution Explorer";
            //_winProjExplorer.Tag = "SOLUTIONEXPLORER";
            //_winProjExplorer.HideOnClose = true;
            //_winProjExplorer.Show(dockContainer1, DockState.DockRightAutoHide);

            ////Output
            //_winOutput = new Output();
            //_winOutput.Text = "Output";
            //_winOutput.Tag = "OUTPUT";
            //_winOutput.HideOnClose = true;
            //_winOutput.Show(dockContainer1, DockState.DockBottomAutoHide);
            //_winOutput.Visible = false;

            dockContainer1.ActiveDocumentChanged += new EventHandler(dockContainer1_ActiveDocumentChanged);
            //_winProjExplorer.Visible = false;
            //_winProjExplorer.FileClick += new EventHandler<ExplorerClickEventArgs>(_winProjExplorer_FileClick);
            //_winProjExplorer.FileNameChanged += new EventHandler<ExplorerLabelEditEventArgs>(_winProjExplorer_FileNameChanged);
            //_winProjExplorer.NewItemAdd += new EventHandler(_winProjExplorer_NewItemAdd);
            //_winProjExplorer.FileItemDeleted += new EventHandler(_winProjExplorer_FileItemDeleted);
            //_winErrorList.ItemDoubleClick += new EventHandler<ListViewItemEventArgs>(_winErrorList_ItemDoubleClick);
            //_winProjExplorer.AddRefrenceItem += new EventHandler(_winProjExplorer_AddRefrenceItem);
            //_winProjExplorer.AddWebRefrenceItem += new EventHandler(_winProjExplorer_AddWebRefrenceItem);
        }

        public void AddRefrence(ProjectItem Reference)
        {
            TreeNode refNode = _winProjExplorer.RefrenceNode;
            ArrayList list = new ArrayList();
            list.Add(Reference);
            ConvertCOM(null, list, refNode);
        }

        private void _winProjExplorer_AddWebRefrenceItem(object sender, EventArgs e)
        {
            TreeNode t = (TreeNode)sender;
            StringCollection files = new StringCollection();
            foreach (string Name in Parser.ProjectParser.ProjectFiles.Keys)
            {
                files.Add(Name);
            }
            using (AddWebReferenceDialog refDialog = new AddWebReferenceDialog(s_AIMSProject, _scriptLanguage, files))
            {
                refDialog.NamespacePrefix = s_AIMSProject.RootNamespace;
                if (refDialog.ShowDialog() == DialogResult.OK)
                {
                    // Do not overwrite existing web references.
                    refDialog.WebReference.Name = WebReference.GetReferenceName(refDialog.WebReference.WebReferencesDirectory, refDialog.WebReference.Name);
                    AddWebRefrenceToProject(t, refDialog.WebReference, refDialog.WebReferenceFileName);
                }
            }
        }

        private void AddWebRefrenceToProject(TreeNode node, WebReference webref, string fileName)
        {
            Parser.ProjectParser.ParseProjectContents(fileName, webref.GetSourceCode(), false);
            _winProjExplorer.AddWebReference(fileName);
            ArrayList refItems = new ArrayList();
            foreach (ProjectItem item in webref.Items)
            {
                if (item is ReferenceProjectItem)
                {
                    refItems.Add(item);
                }
            }
            if (refItems.Count > 0)
                ConvertCOM(null, refItems, node);
        }

        private void _winProjExplorer_AddRefrenceItem(object sender, EventArgs e)
        {
            TreeNode t = (TreeNode)sender;
            SelectReferenceDialog selDialog = null;
            if (_selectRefDialog == null)
            {
                _selectRefDialog = new WeakReference(null);
            }
            if (!_selectRefDialog.IsAlive)
            {
                _selectRefDialog.Target = new SelectReferenceDialog(s_AIMSProject);
            }

            selDialog = (SelectReferenceDialog)_selectRefDialog.Target;
            selDialog.ConfigureProject = s_AIMSProject;
            if (selDialog.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                ConvertCOM(null, selDialog.ReferenceInformations, t);
            }

            _selectRefDialog.Target = selDialog;
        }

        private void ConvertCOM(object sender, ArrayList refrences, TreeNode node)
        {
            object[] param = new object[] { sender, (object)refrences, (object)node };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ConvertCOMThread), (object)param);
        }

        private void ConvertCOMThread(Object stateInfo)
        {
            object[] param = (object[])stateInfo;
            object sender = param[0];
            ArrayList refrences = param[1] as ArrayList;
            TreeNode node = param[2] as TreeNode;
            BeginInvoke(new MethodInvoker(delegate { tsMessage.Text = "Please wait..."; }));
            foreach (ReferenceProjectItem reference in refrences)
            {
                try
                {
                    if (reference.ItemType == ItemType.COMReference)
                    {
                        if (Path.IsPathRooted(reference.FileName))
                        {
                            s_AIMSProject.AddProjectItem(reference);
                        }
                        else
                        {
                            ArrayList addedRefs = ImportCom(reference as ComReferenceProjectItem);
                            foreach (ReferenceProjectItem refs in addedRefs)
                            {
                                s_AIMSProject.AddProjectItem(refs);
                                BeginInvoke(new MethodInvoker(delegate
                                {
                                    TreeNode refNode = node.Nodes.Add(refs.Name);
                                    refNode.ImageKey = "Reference.ico";
                                    refNode.Tag = NodeType.Reference;
                                }));
                            }
                        }
                    }
                    else if (reference.ItemType == ItemType.Reference)
                    {
                        s_AIMSProject.AddProjectItem(reference);
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            TreeNode refNode = node.Nodes.Add(reference.Name);
                            refNode.ImageKey = "Reference.ico";
                            refNode.Tag = NodeType.Reference;
                        }));
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                BeginInvoke(new MethodInvoker(delegate { tsMessage.Text = "Ready"; }));
            }
        }

        private ArrayList ImportCom(ComReferenceProjectItem t)
        {
            ArrayList refrences = new ArrayList();
            refrences.Add(t as ReferenceProjectItem);
            BeginInvoke(new MethodInvoker(delegate { tsMessage.Text = "Compiling COM component '" + t.Include + "' ..."; }));
            Converter.TlbImp importer = new Converter.TlbImp(refrences);
            importer.ReportEvent += new EventHandler<Converter.ReportEventEventArgs>(importer_ReportEvent);
            importer.ResolveRef += new EventHandler<Converter.ResolveRefEventArgs>(importer_ResolveRef);
            string outputFolder = Path.GetDirectoryName(s_AIMSProject.OutputAssemblyFullPath);
            string interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", t.Include, ".dll"));
            string asmPath = interopFileName;
            importer.Import(asmPath, t.FilePath, t.Name);
            return refrences;
        }

        private void importer_ResolveRef(object sender, Converter.ResolveRefEventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate { tsMessage.Text = e.Message; }));
            BeginInvoke(new MethodInvoker(delegate { _winOutput.AppendLine(e.Message); }));
        }

        private void importer_ReportEvent(object sender, Converter.ReportEventEventArgs e)
        {
            string msg;
            msg = Environment.NewLine + "COM Importer Event ..." + Environment.NewLine;
            msg += "Kind: " + e.EventKind.ToString() + Environment.NewLine;
            msg += "Code: " + e.EventCode + Environment.NewLine;
            msg += "Message: " + e.EventMsg;
            BeginInvoke(new MethodInvoker(delegate { tsMessage.Text = e.EventMsg; }));
            BeginInvoke(new MethodInvoker(delegate { _winOutput.AppendLine(msg); }));
        }


        private void _winProjExplorer_FileItemDeleted(object sender, EventArgs e)
        {
            TreeNode node = (TreeNode)sender;
            Document doc = GetExistingFile(node.Text);

            if (doc != null)
            {
                doc.Close();
            }
            node.Remove();

            Parser.ProjectParser.RemoveContentFile(node.Text);
        }

        private void _winProjExplorer_NewItemAdd(object sender, EventArgs e)
        {
            AddNewItem();
        }

        private void _winProjExplorer_FileNameChanged(object sender, ExplorerLabelEditEventArgs e)
        {
            if (ValidateFileName(e.NewName))
            {
                string contents = Parser.ProjectParser.GetFileContents(e.OldName);
                Parser.ProjectParser.ProjectFiles.Remove(e.OldName);
                Parser.ProjectParser.ParseProjectContents(e.NewName, contents);
                Document doc = GetExistingFile(e.OldName);
                if (doc != null)
                {
                    doc.Text = Path.GetFileNameWithoutExtension(e.NewName);
                    doc.FileName = e.NewName;
                }
            }
            else
            {
                MessageBox.Show("File Name '" + e.NewName + "' already exists in the project.Please try other name.");
                e.Cancel = true;
            }
        }

        private bool ValidateFileName(string fileName)
        {
            string[] keys = new string[Parser.ProjectParser.ProjectFiles.Keys.Count];
            Parser.ProjectParser.ProjectFiles.Keys.CopyTo(keys, 0);

            for (int count = 0; count <= keys.Length - 1; count++)
            {
                if (keys[count].ToLower() == fileName.ToLower())
                    return false;
            }
            return true;
        }

        private void _winErrorList_ItemDoubleClick(object sender, ListViewItemEventArgs e)
        {
            System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
            tmr.Tick += new EventHandler(ShowIntoView);
            tmr.Interval = 500;
            Document doc = ShowFile(e.FileName);
            tmr.Tag = e;
            tmr.Start();
        }

        private void ShowIntoView(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer tmr = (System.Windows.Forms.Timer)sender;
            tmr.Stop();
            ListViewItemEventArgs et = (ListViewItemEventArgs)tmr.Tag;
            Document doc = ShowFile(et.FileName);
            TextPoint t = new TextPoint(et.ColumnNo, et.LineNo);
            doc.Editor.ScrollIntoView(t);
            doc.Editor.Caret.SetPos(t);
        }

        private Document GetExistingFile(string FileName)
        {
            Document doc = null;
            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is Document)
                {
                    doc = docWin as Document;
                    if (doc.FileName == FileName)
                    {
                        return doc;
                    }
                }
            }
            return null;
        }

        public Document ShowFile(string FileName)
        {
            Document doc = null;
            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is Document)
                {
                    doc = docWin as Document;
                    if (doc.FileName == FileName)
                    {
                        doc.Show(dockContainer1, DockState.Document);
                        return doc;
                    }
                }
            }
            // Not Found

            doc = OpenDocuments(FileName, (VSProject)null);
            if (doc != null)
            {
                doc.Activate();
                doc.Focus();
            }
            return doc;
        }

        public Document FileOpened(string FileName)
        {
            Document doc = null;
            foreach (DockContent docWin in dockContainer1.Contents)
            {
                if (docWin is Document)
                {
                    doc = docWin as Document;
                    if (doc.FileName == FileName)
                    {
                        doc.Show(dockContainer1, DockState.Document);
                        doc.Activate();
                        doc.Focus();
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
        private void _winProjExplorer_FileClick(object sender, ExplorerClickEventArgs e)
        {
            Document doc = ShowFile(e.FileName);
            if (doc != null) doc.ParseContentsNow();
        }


        public Document GetActiveDocument()
        {
            if (dockContainer1.ActiveDocument == null)
                return null;
            return dockContainer1.ActiveDocument as Document;
        }

        public event EventHandler activeDocument;

        private bool _activated = false;

        private void dockContainer1_ActiveDocumentChanged(object sender, EventArgs e)
        {
            if (dockContainer1.ActiveDocument == null)
                return;

            if (dockContainer1.ActiveDocument is Document)
            {
                Document doc = dockContainer1.ActiveDocument as Document;


                if (_activated == false)
                {
                    SetActiveDocument();
                    _activated = true;
                }

                EditViewControl ev = GetCurrentView();

                if (ev != null)
                    ev.CodeEditor.HideIntellisense();
            }
            //UpdateCutCopyToolbar();

            if (activeDocument != null)
                activeDocument(this, new EventArgs());
        }




        public Document AddDocument(string Name)
        {
            return AddDocument(Name, false);
        }

        public Document AddDocument(string Name, bool IsWebReference)
        {
            Document doc = new Document(this);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            doc.ScriptLanguage = _scriptLanguage;
            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);
            if (IsWebReference)
                _winProjExplorer.AddWebReference(Name);
            else
                _winProjExplorer.AddFile(Name);
            Parser.ProjectParser.ParseProjectContents(Name, "");
            return doc;
        }




        public void SetActiveDocument()
        {
            //d.Size = new Size(this.Size.Width - 10, this.Size.Height);

            if (dockContainer1.ActiveDocumentPane == null)
                return;

            this.dockContainer1.ActiveDocumentPane.Size = new Size(this.Size.Width, this.Size.Height);

            this.dockContainer1.ActiveDocumentPane.master = this;

            this.dockContainer1.ActiveDocumentPane.DoResize();
        }

        public DocumentForm OpenDocumentForm()
        {
            DocumentForm doc = new DocumentForm(this);
            //DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);



            return doc;
        }
        public WebForm OpenWebForm(string web, string url)
        {
            WebForm doc = new WebForm(this);
            doc.NavigateTo(url);
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
        public ParseInformation parse { get; set; }


        public Document OpenDocuments(string Name, VSProvider.VSProject pp)
        {
            if (FileOpened(Name) != null)
                return null;

            this.SuspendLayout();

            string contents = "";

            try
            {
                contents = File.ReadAllText(Name);
            }
            catch (Exception e) { };

            if (contents == string.Empty)
                return null;
            Document doc = new Document(this);
            
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            doc.ScriptLanguage = _scriptLanguage;
            doc.vp = pp;
            doc.LoadVSProject(pp, Name);


            //string exts = Path.GetExtension(Name);
            //ImageList imgs = CodeEditorControl.AutoListImages;
            //if (imgs.Images.ContainsKey(exts) == true)
            //    doc.DockHandler.Icon = Icon.FromHandle(((Bitmap)imgs.Images[exts]).GetHicon());
            //else
            //    doc.DockHandler.Icon = Icon.FromHandle(resource._class.GetHicon());

            doc.TabPageContextMenu = new ContextMenu();
            doc.TabPageContextMenu.MenuItems.Add(new MenuItem("test for context menu"));

            if (Name.EndsWith(".cs"))
            {
                doc.Contents = contents;
                doc.ExpandAll();
                doc.ToolTipText = Name;
                DocumentEvents(doc, true);
                dockContainer1.SuspendLayout();
                doc.Show(dockContainer1, DockState.Document);
                dockContainer1.ResumeLayout();

                //doc.ParseContentsNow();

                LoadKeywordsAsync(doc);
            }
            else
            if (Name.EndsWith(".vb"))
            {
                doc.Contents = contents;
                doc.ScriptLanguage = ScriptLanguage.VBNET;
                doc.ExpandAll();
                doc.ToolTipText = Name;
                DocumentEvents(doc, true);
                dockContainer1.SuspendLayout();
                doc.Show(dockContainer1, DockState.Document);
                dockContainer1.ResumeLayout();

                doc.ParseContentsNow();
            }
            else
            {
                doc.ScriptLanguage = ScriptLanguage.XML;
                doc.Contents = contents;
                doc.ExpandAll();
                doc.ToolTipText = Name;
                DocumentEvents(doc, true);
                dockContainer1.SuspendLayout();
                doc.Show(dockContainer1, DockState.Document);
                dockContainer1.ResumeLayout();

                doc.ParseContentsNow();
            }

            this.ResumeLayout();

            return doc;
        }

        private void Doc_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            Document d = sender as Document;
            d.ContextMenu.Show(this.GetCurrentView(), e.Location);
        }

        public Document OpenDocuments(string contents, string name, VSProject vp)
        {
            if (FileOpened(name) != null)
                return null;

            if (contents == string.Empty)
                return null;
            Document doc = new Document(this);
            doc.FileName = name;
            doc.Text = Path.GetFileNameWithoutExtension(name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            doc.ScriptLanguage = _scriptLanguage;
            doc.LoadVSProject(vp, name);

            LoadKeywordsAsync(doc);

            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);
            doc.Contents = contents;

            

            doc.ParseContentsNow();

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
            Document doc = new Document(this);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            doc.ScriptLanguage = _scriptLanguage;
            doc.vp = pp;
            doc.LoadVSProject(pp, Name);
            doc.LoadKeywords(doc);
            doc.Editor.GotoLine(hst.line);
            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);
            doc.Contents = contents;
            doc.ExpandAll();
            doc.TabPageContextMenu = new ContextMenu();
            doc.TabPageContextMenu.MenuItems.Add(new MenuItem("test for context menu"));

            doc.ParseContentsNow();




            return doc;
        }


        public Document OpenDocuments2(string Name, VSProvider.VSProject pp, ICSharpCode.NRefactory.TypeSystem.DomRegion dr, bool selectable)
        {
            Document doc = null;

            doc = FileOpened(Name);

            if (doc == null)
            {
                string contents = "";

                contents = File.ReadAllText(Name);

                if (contents == string.Empty)
                    return null;
                doc = new Document(this);
                doc.FileName = Name;
                doc.Text = Path.GetFileNameWithoutExtension(Name);
                doc.Tag = "USERDOCUMENT";
                doc.HideOnClose = false;
                doc.ScriptLanguage = _scriptLanguage;
                doc.vp = pp;
                doc.Contents = contents;
                doc.LoadVSProject(pp, Name);
                LoadKeywordsAsync(doc);
                DocumentEvents(doc, true);
                doc.Show(dockContainer1, DockState.Document);



                doc.ParseContentsNow();
            }

            doc.Editor.GotoLine(dr.BeginLine);
            if (selectable)
            {
                int offset = doc.Editor.Caret.GetOffset(dr.BeginColumn, dr.BeginLine);
                doc.Editor.GotoLine(dr.BeginColumn); //doc.Editor.Caret.Position = new TextPoint(dr.BeginColumn, dr.BeginLine);
                SelectText(offset, dr.EndColumn - dr.BeginColumn);
            }




            return doc;
        }


        public Document BookmarkAll(string Name, VSProvider.VSProject pp, ArrayList B)
        {
            Document doc = null;

            doc = FileOpened(Name);

            if (doc == null)
            {
                string contents = "";

                contents = File.ReadAllText(Name);

                if (contents == string.Empty)
                    return null;
                doc = new Document(this);
                doc.FileName = Name;
                doc.Text = Path.GetFileNameWithoutExtension(Name);
                doc.Tag = "USERDOCUMENT";
                doc.HideOnClose = false;
                doc.ScriptLanguage = _scriptLanguage;
                doc.vp = pp;
                doc.Contents = contents;
                doc.LoadVSProject(pp, Name);
                LoadKeywordsAsync(doc);
                DocumentEvents(doc, true);
                doc.Show(dockContainer1, DockState.Document);



                doc.ParseContentsNow();
            }


            foreach (Tuple<string, int, int> r in B)
            {
                doc.syntaxDocument1[r.Item3].Bookmarked = true;
            }



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

        public Document Bookmark(string Name, VSProvider.VSProject pp, int l2)
        {
            Document doc = null;

            doc = FileOpened(Name);

            if (doc == null)
            {
                string contents = "";

                contents = File.ReadAllText(Name);

                if (contents == string.Empty)
                    return null;
                doc = new Document(this);
                doc.FileName = Name;
                doc.Text = Path.GetFileNameWithoutExtension(Name);
                doc.Tag = "USERDOCUMENT";
                doc.HideOnClose = false;
                doc.ScriptLanguage = _scriptLanguage;
                doc.vp = pp;
                doc.Contents = contents;
                doc.LoadVSProject(pp, Name);
                doc.LoadKeywords(doc);
                DocumentEvents(doc, true);
                doc.Show(dockContainer1, DockState.Document);



                doc.ParseContentsNow();
            }




            doc.syntaxDocument1[l2].Bookmarked = true;






            return doc;
        }


        public Document OpenDocument(string contents, VSProvider.VSProject pp)
        {
            if (contents == string.Empty)
                return null;
            Document doc = new Document(this);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = true;
            doc.ScriptLanguage = _scriptLanguage;
            doc.vp = pp;
            doc.LoadVSProject(pp, Name);
            DocumentEvents(doc, true);
            doc.Show(dockContainer1, DockState.Document);
            doc.Contents = contents;

            doc.ParseContentsNow();

            doc.Editor.Caret.Position.Y = 1;

            doc.Focus();

            return doc;
        }


        private void DocumentEvents(Document doc, bool Enable)
        {
            if (Enable)
            {
                doc.ParseContent += new EventHandler<ParseContentEventArgs>(doc_ParseContent);
                doc.FormClosing += new FormClosingEventHandler(doc_FormClosing);
                doc.CaretChange += new EventHandler<EventArgs>(doc_CaretChange);
                doc.TextChange += new EventHandler<EventArgs>(doc_TextChange);
                doc.Editor.Selection.Change += new EventHandler(Selection_Change);
                doc.Editor.Comments += new EventHandler(Comments_Change);
                doc.Editor.UnComments += new EventHandler(UnComments_Change);
                //doc.MouseClick += new MouseEventHandler(Doc_MouseClick);


            }
            else
            {
                doc.ParseContent -= doc_ParseContent;
                doc.FormClosing -= doc_FormClosing;
                doc.CaretChange -= doc_CaretChange;
                doc.Editor.Selection.Change -= Selection_Change;
            }
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
            string text = "";
            UpdateCutCopyToolbar();
            string file = ((Document)sender).FileName;
            Caret c = ((Document)sender).Editor.ActiveViewControl.Caret;

            if (c.CurrentRow != null)
                text = c.CurrentRow.Text;
            //tCursorPos.Text = "Ln " + c.Position.Y + ", Col " + c.Position.X;
            if (label != null)
            {
                label.Text = "Ln " + c.Position.Y;
                labels.Text = "Col " + c.Position.X;
            }
            bool changed = hst.AddIfShould(file, c.Position.Y, text);
            if (HistoryChange != null)
                HistoryChange(this, new EventArgs());
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


        //private readonly object _syncRoot = new Object();

        private void doc_ParseContent(object sender, ParseContentEventArgs e)
        {
            //lock (_syncRoot)
            {
                DoParsing(sender, e, true);
            }
        }

        private void DoParsing(object sender, ParseContentEventArgs e, bool IsOpened)
        {
            object[] param = new object[] { sender, (object)e, true };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ParseContentThread), (object)param);
        }

        private readonly object _syncRoot = new Object();

        private void ParseContentThread(Object stateInfo)
        {
            object[] param = (object[])stateInfo;
            object sender = param[0];
            ParseContentEventArgs e = param[1] as ParseContentEventArgs;
            bool IsOpened = (bool)param[2];
            ParseInformation pi = Parser.ProjectParser.ParseProjectContents(e.FileName, e.Content, IsOpened);
            NRefactory.Parser.Errors errors = Parser.ProjectParser.LastParserErrors;
            Document doc = sender as Document;
            BeginInvoke(new MethodInvoker(delegate { UploadParserError(doc, errors); }));
        }

        private void UploadParserError(Document doc, NRefactory.Parser.Errors e)
        {
            if (e != null)
                if (_winErrorList != null)
                    _winErrorList.ProjectErrors(doc, e);
        }

        private void doc_FormClosing(object sender, FormClosingEventArgs e)
        {
            Document doc = sender as Document;

            if (doc.state == Document.State.waiting)
            {
                DQ.Remove(doc);
                e.Cancel = true;
                return;
            }

            if (doc.state == Document.State.working)
            {
                e.Cancel = true;
                return;
            };

            DocumentEvents(doc, false);
            if (Parser.ProjectParser.ProjectFiles.ContainsKey(doc.FileName))
                Parser.ProjectParser.ProjectFiles[doc.FileName].IsOpened = false;
        }
        #region ClickHandlers
        private void cNetToolStripMenuItemCSharp_Click(object sender, EventArgs e)
        {
            if (_winErrorList.ParserErrorCount == 0)
            {
                if (_scriptLanguage != ScriptLanguage.CSharp)
                    this.ScriptLanguage = ScriptLanguage.CSharp;
            }
            else
            {
                MessageBox.Show("Remove Parsing errors before converting");
            }
        }

        private void vBNetToolStripMenuItemVbNet_Click(object sender, EventArgs e)
        {
            if (_winErrorList.ParserErrorCount == 0)
            {
                if (_scriptLanguage != ScriptLanguage.VBNET)
                    this.ScriptLanguage = ScriptLanguage.VBNET;
            }
            else
            {
                MessageBox.Show("Remove Parsing errors before converting");
            }
        }

        private void tsbCut_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null) view.Cut();
            UpdateCutCopyToolbar();
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                if (view.CanCopy)
                    view.Copy();
            }
            UpdateCutCopyToolbar();
        }

        private CodeEditor.WinForms.EditViewControl GetCurrentView()
        {
            Document doc = dockContainer1.ActiveDocument as Document;
            if (doc != null)
                return doc.CurrentView;
            else
                return null;
        }

        private void tsbPaste_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                if (view.CanPaste)
                    view.Paste();
            }
            UpdateCutCopyToolbar();
        }

        private void tsbUndo_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                if (view.CanUndo)
                    view.Undo();
            }
            UpdateCutCopyToolbar();
        }

        private void tsbRedo_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                if (view.CanRedo)
                    view.Redo();
            }
            UpdateCutCopyToolbar();
        }

        private void tsbToggleBookmark_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.ToggleBookmark();
            }
        }

        private void tsbPreBookmark_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.GotoPreviousBookmark();
            }
        }

        private void tsbNextBookmark_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.GotoNextBookmark();
            }
        }

        private void tsbDelAllBookmark_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all of the bookmark(s).", "AIMS Script Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CodeEditor.WinForms.EditViewControl view = GetCurrentView();
                if (view != null)
                {
                    view.Document.ClearBookmarks();
                }
            }
        }

        private void tsbDelallBreakPoints_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove all of the break point(s).", "AIMS Script Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CodeEditor.WinForms.EditViewControl view = GetCurrentView();
                if (view != null)
                {
                    view.Document.ClearBreakpoints();
                }
            }
        }

        private void tsbFind_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.ShowFind();
            }
        }

        public void SelectText(int start, int sc, int length)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                int s = view.GetLineIndex(start);
                view.Selection.SelStart = s + sc;
                view.Selection.SelLength = length;
            }
        }

        public void SelectTextXY(int X, int Y, int length)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                int p = Y - 10;
                if (p < 0)
                    p = 0;

                view.ExpandAll();

                view.Refresh();

                view.ScrollIntoView(p);



                int offset = view.Caret.GetOffset(X, Y);
                //int s = view.GetLineIndex(start);
                view.Selection.SelStart = offset;
                view.Selection.SelLength = length;
            }
        }


        public void SelectText(int start, int length)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                int s = view.GetRowndex(start) - 20;
                if (s < 0)
                    s = 0;

                view.Selection.SelStart = start;
                view.Selection.SelLength = length;

                view.Refresh();

                view.Document[s].EnsureVisible();
            }
        }
        public void SelectTextLine(int start, int length, bool highlighted = false)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.Focus();

                int p = start - 10;
                if (p < 0)
                    p = 0;

                view.ExpandAll();

                view.Refresh();

                view.ScrollIntoView(p);

                System.Threading.Tasks.Task.Delay(300);

                int s = view.Document.PointToIntPos(new TextPoint(0, start));

                if (highlighted == false)
                {
                    view.Selection.SelStart = s;
                    view.Selection.SelLength = length;
                    view.Refresh();
                }
                else
                {
                    view.Focus();

                    //view.Highlight(p);
                    view.Caret.SetPos(new TextPoint(0, start));
                    view.Refresh();
                }

                //view.Document[start].EnsureVisible();
            }
        }


        private void tsbReplace_Click(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                view.ShowReplace();
            }
        }

        private void tsbErrorList_Click(object sender, EventArgs e)
        {
            //  _winErrorList.Show();
            //  _winOutput.Show();
        }

        private void tsbBuild_Click(object sender, EventArgs e)
        {
            CompilerResults results = CompileScript();
            LoadComileErrors(results.Errors);
        }

        private CompilerResults CompileScript()
        {
            CodeDomProvider provider = s_AIMSProject.LanguageProperties.CodeDomProvider;
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = false;
            parameters.IncludeDebugInformation = true;
            parameters.OutputAssembly = s_AIMSProject.OutputAssemblyFullPath;
            foreach (ProjectItem item in s_AIMSProject.Items)
            {
                parameters.ReferencedAssemblies.Add(item.Include + ".dll");
            }
            parameters.ReferencedAssemblies.Add("AIMS.Scripting.ScriptRun.dll");

            string[] sourceCode = new string[Parser.ProjectParser.ProjectFiles.Count];
            int counter = 0;
            string tmpFilePath = Path.Combine(Path.GetDirectoryName(s_AIMSProject.OutputAssemblyFullPath), "Temp");
            if (Directory.Exists(tmpFilePath))
                Directory.Delete(tmpFilePath, true);
            Directory.CreateDirectory(tmpFilePath);
            foreach (Parser.ProjectContentItem pcItem in Parser.ProjectParser.ProjectFiles.Values)
            {
                StreamWriter writer = new StreamWriter(Path.Combine(tmpFilePath, pcItem.FileName), false);
                writer.Write(pcItem.Contents);
                writer.Close();
                sourceCode[counter++] = Path.Combine(tmpFilePath, pcItem.FileName);
            }

            CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourceCode);
            Directory.Delete(tmpFilePath, true);
            return results;
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
            AddNewItem();
        }

        private void AddNewItem()
        {
            dockContainer1.SuspendLayout();
            StringCollection files = new StringCollection();
            foreach (string Name in Parser.ProjectParser.ProjectFiles.Keys)
            {
                files.Add(Name);
            }
            NewFileDialog f = new NewFileDialog(_scriptLanguage, files);
            f.ShowDialog(dockContainer1);
            string fileName = f.FileName;
            if (fileName.Length > 0)
            {
                Document doc = AddDocument(fileName);
                doc.Editor.ActiveViewControl.Document.Text = GetInitialContents(f, fileName);
                doc.ParseContentsNow();
                doc.Editor.ActiveViewControl.Caret.Position = new TextPoint(0, 1);
            }

            dockContainer1.ResumeLayout();
        }
        #endregion

        #region Private Members

        private string GetInitialContents(NewFileDialog f, string fileName)
        {
            string defNameSpace = NewFileDialog.GetDefaultNamespace(s_AIMSProject, fileName);
            string defClassName = NewFileDialog.GenerateValidClassOrNamespaceName(Path.GetFileNameWithoutExtension(fileName), true);

            StringBuilder contents = new StringBuilder();

            if (_scriptLanguage == ScriptLanguage.CSharp)
            {
                contents.AppendLine("#region Usings ...");
                contents.AppendLine("using System;");
                contents.AppendLine("using System.Collections.Generic;");
                contents.AppendLine("using System.Text;");
                contents.AppendLine("#endregion");
                contents.AppendLine("");
                contents.AppendLine("namespace " + defNameSpace);
                contents.AppendLine("{");
                contents.AppendLine("    " + (f.SelectedItemType == SelectedItemType.Class ? "class " : "interface ") + defClassName);
                contents.AppendLine("    {");
                contents.AppendLine("");
                contents.AppendLine("    }");
                contents.AppendLine("}");
            }
            else
            {
                contents.AppendLine("#Region Usings ...");
                contents.AppendLine("Imports System");
                contents.AppendLine("Imports System.Collections.Generic");
                contents.AppendLine("Imports System.Text");
                contents.AppendLine("#End Region");
                contents.AppendLine("");
                contents.AppendLine("Namespace " + defNameSpace);
                contents.AppendLine("    " + (f.SelectedItemType == SelectedItemType.Class ? "Class " : "Interface ") + defClassName);
                contents.AppendLine("");
                contents.AppendLine("    End " + (f.SelectedItemType == SelectedItemType.Class ? "Class" : "Interface"));
                contents.AppendLine("End Namespace");
            }

            return contents.ToString();
        }

        private void UpdateCutCopyToolbar()
        {
            EditViewControl ev = GetCurrentView();
            if (ev != null)
            {
                tsbComment.Enabled = true;
                tsbFind.Enabled = true;
                tsbReplace.Enabled = true;
                tsbToggleBookmark.Enabled = true;
                tsbUnComment.Enabled = true;

                tsbCut.Enabled = (ev.Selection.IsValid ? true : false);
                tsbCopy.Enabled = (ev.Selection.IsValid ? true : false);
                tsbPaste.Enabled = ev.CanPaste;
                tsbRedo.Enabled = ev.CanRedo;
                tsbUndo.Enabled = ev.CanUndo;
            }
            else
            {
                tsbCut.Enabled = false;
                tsbCopy.Enabled = false;
                tsbPaste.Enabled = false;
                tsbRedo.Enabled = false;
                tsbUndo.Enabled = false;

                tsbComment.Enabled = false;
                tsbFind.Enabled = false;
                tsbReplace.Enabled = false;
                tsbToggleBookmark.Enabled = false;
                tsbUnComment.Enabled = false;
            }
        }

        public void LoadComileErrors(System.CodeDom.Compiler.CompilerErrorCollection Errors)
        {
            _winErrorList.ComilerErrors(null, Errors);
        }
        public static AutoListIcons GetIcon(IClass c)
        {
            AutoListIcons imageIndex = AutoListIcons.iClass;

            switch (c.ClassType)
            {
                case ClassType.Delegate:
                    imageIndex = AutoListIcons.iDelegate;
                    break;
                case ClassType.Enum:
                    imageIndex = AutoListIcons.iEnum;
                    break;
                case ClassType.Struct:
                    imageIndex = AutoListIcons.iStructure;
                    break;
                case ClassType.Interface:
                    imageIndex = AutoListIcons.iInterface;
                    break;
            }
            return (AutoListIcons)(int)imageIndex + GetModifierOffset(c.Modifiers);
        }
        public static AutoListIcons GetIcon(IMethod method)
        {
            return (AutoListIcons)((int)AutoListIcons.iMethod + GetModifierOffset(method.Modifiers));
        }
        private static int GetModifierOffset(ModifierEnum modifier)
        {
            if ((modifier & ModifierEnum.Public) == ModifierEnum.Public)
            {
                return 0;
            }
            if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected)
            {
                return 3;
            }
            if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal)
            {
                return 4;
            }
            return 2;
        }
        public static AutoListIcons GetIcon(IField field)
        {
            if (field.IsConst)
            {
                return AutoListIcons.iConstant;
            }
            else if (field.IsParameter)
            {
                return AutoListIcons.iProperties;
            }
            else if (field.IsLocalVariable)
            {
                return AutoListIcons.iField;
            }
            else
            {
                return (AutoListIcons)((int)AutoListIcons.iField + GetModifierOffset(field.Modifiers));
            }
        }
        public static AutoListIcons GetIcon(IProperty property)
        {
            if (property.IsIndexer)
                return (AutoListIcons)((int)AutoListIcons.iProperties + GetModifierOffset(property.Modifiers));
            else
                return (AutoListIcons)((int)AutoListIcons.iProperties + GetModifierOffset(property.Modifiers));
        }
        public static AutoListIcons GetIcon(IEvent evt)
        {
            return (AutoListIcons)((int)AutoListIcons.iEvent + GetModifierOffset(evt.Modifiers));
        }
        #endregion


        internal static IProject GetProject()
        {
            return s_AIMSProject;
        }


        public void Undo()
        {
            EditViewControl ev = GetCurrentView();

            if (ev == null)
                return;

            ev.Undo();
        }

        public void Redo()
        {
            EditViewControl ev = GetCurrentView();

            if (ev == null)
                return;


            ev.Redo();
        }

        public void Comment()
        {
            tsbComment.Enabled = false;
            toolStrip1.Refresh();
            EditViewControl ev = GetCurrentView();
            if (ev != null)
            {
                CommentCode(ev, true);
            }
            tsbComment.Enabled = true;
        }

        private void tsbComment_Click(object sender, EventArgs e)
        {
            tsbComment.Enabled = false;
            toolStrip1.Refresh();
            EditViewControl ev = GetCurrentView();
            if (ev != null)
            {
                CommentCode(ev, true);
            }
            tsbComment.Enabled = true;
        }

        private void CommentCode(EditViewControl ev, bool comment)
        {
            int startRow = Math.Min(ev.Selection.Bounds.FirstRow, ev.Selection.Bounds.LastRow);
            int lastRow = Math.Max(ev.Selection.Bounds.FirstRow, ev.Selection.Bounds.LastRow);
            int lastCol = ev.Document.VisibleRows[lastRow].Count; // No of words
            if (lastCol >= 0)
                lastCol = Math.Max(lastCol, ev.Document.VisibleRows[lastRow].Expansion_EndChar);
            else
                lastCol = 0;
            bool found = false;
            bool Changed = false;
            TextRange tr = new TextRange(0, startRow, lastCol, lastRow);
            TextRange trFinal = new TextRange(0, startRow, lastCol + 2, lastRow);

            string txt = ev.Document.GetRange(tr);
            string[] rows = txt.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] output = new string[rows.Length];
            ev.Selection.Bounds = tr;
            for (int count = 0; count <= lastRow - startRow; count++)
            {
                found = false;
                string rowText = rows[count];
                int startIndex = rowText.Length - rowText.TrimStart(null).Length;
                string wText = rowText.Substring(startIndex);
                if (comment)
                {
                    if (wText.Length > 0)
                    {
                        wText = (_scriptLanguage == ScriptLanguage.CSharp ? "//" : "'") + wText;
                        found = true;
                    }
                }
                else
                {
                    if (_scriptLanguage == ScriptLanguage.CSharp)
                    {
                        if (wText.Length >= 2 && wText.Substring(0, 2) == "//")
                        {
                            if (wText.Length >= 3 && wText.Substring(2, 1) != "/")
                            {
                                wText = wText.Substring(2);
                                found = true;
                            }
                            else
                            {
                                if ((wText.Length > 3 && wText.Substring(0, 3) != "///") || (wText.Length > 4 && wText.Substring(0, 4) == "////"))
                                {
                                    wText = wText.Substring(2);
                                    found = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (wText.Length >= 1 && wText.Substring(0, 1) == "'")
                        {
                            wText = wText.Substring(1);
                            found = true;
                        }
                    }
                }
                if (found && rowText.Length > 0)
                {
                    output[count] = rowText.Substring(0, startIndex) + wText;
                    Changed = true;
                }
                else
                    output[count] = rowText;
            }

            if (Changed)
            {
                string pReplacedData = string.Join("\r\n", output);

                ev.ReplaceSelection(pReplacedData);
                ev.Selection.Bounds = trFinal;
            }
        }

        public void UnComment()
        {
            tsbUnComment.Enabled = false;
            toolStrip1.Refresh();
            EditViewControl ev = GetCurrentView();
            if (ev != null)
            {
                CommentCode(ev, false);
            }
            tsbUnComment.Enabled = true;
        }

        private void tsbUnComment_Click(object sender, EventArgs e)
        {
            tsbUnComment.Enabled = false;
            toolStrip1.Refresh();
            EditViewControl ev = GetCurrentView();
            if (ev != null)
            {
                CommentCode(ev, false);
            }
            tsbUnComment.Enabled = true;
        }
        private void tsbSolutionExplorer_Click(object sender, EventArgs e)
        {
            // _winProjExplorer.Show();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //FileDialog fd = new FileDialog();
            //DialogResult r = fd.ShowDialog();
            //if (r != DialogResult.OK)
            //    return;


        }

        public void OnSave(object sender, EventArgs e)
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                Document ddd = dockContainer1.ActiveDocument as Document;

                string file = ddd.FileName;

                string content = ddd.Contents;

                File.WriteAllText(file, content);

                //ddd.Saved = true;

                view.Selection.Bounds.FirstRow = 5;
                view.Selection.Bounds.FirstColumn = 10;

                view.Selection.Bounds.LastRow = 20;
                view.Selection.Bounds.LastColumn = 10;

                view.SetSelectionBackColor(Color.Blue);

                view.ShowGutter();

                view.Refresh();
            }
        }

        public void Save()
        {
            CodeEditor.WinForms.EditViewControl view = GetCurrentView();
            if (view != null)
            {
                Document ddd = dockContainer1.ActiveDocument as Document;

                string file = ddd.FileName;

                string content = ddd.Contents;

                File.WriteAllText(file, content);

                ddd.ReadyToSave = false;

                ddd.Text = ddd.Text.Replace("*", "");


                view.Refresh();
            }

            dockContainer1.Refresh();
        }


        public void Save(Document ddd)
        {
            string file = ddd.FileName;

            string content = ddd.Contents;

            File.WriteAllText(file, content);

            // ddd.Saved = true;
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
                doc.Editor.LoadSettings(settings);
            }

            dockContainer1.Refresh();
        }

        public string Theme = "";

        public VSSolution vs { get; set; }

        public void SaveSolution()
        {
            if (hst == null)
                hst = new History();

            hst.settings = CodeEditorControl.settings;

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
            }

            hst.Theme = CodeEditorControl.settings.Theme;
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

            if (CodeEditorControl.settings != null)
                CodeEditorControl.settings.Theme = hst.Theme;

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

                    CodeEditorControl.settings = hst.settings;
        }


        public ArrayList LoadSolution(VSSolution vs)
        {
            ArrayList L = new ArrayList();

            hst = History.Deserialize("settings.suo");

            if (hst != null)
                if (hst.settings != null)

                    CodeEditorControl.settings = hst.settings;

            if (hst == null)
            {
                hst = new History();
                this.hst = hst;
            }

            if (hst.acts == null)
                return L;


            this.SuspendLayout();

            int i = 0;

            foreach (string s in hst.acts)
            {
                try
                {
                    if (s.StartsWith("@") == true)
                    {
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
                        Document doc = OpenDocuments(s, p);
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

            hst.acts.Clear();

            this.ResumeLayout();

            return L;
        }

        public void ClearHistory()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;



            if (File.Exists(folder + "settings.suo") == true)
                File.Delete(folder + "settings.suo");

            File.Create("folder + settings.suo");

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
                doc.CloseActiveContent();

                dockContainer1.RemovePane(doc);
            }

            dockContainer1.Refresh();


            hst = new History();
        }

        public void CloseAll()
        {
            ArrayList L = new ArrayList();


            dockContainer1.RemovePanes();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(Document))
                {
                    Document d = doc as Document;
                    L.Add(d.Pane);
                }
                else if (doc.GetType() == typeof(DocumentForm))
                {
                    DocumentForm d = doc as DocumentForm;
                    L.Add(d.Pane);
                }
            }
            foreach (DockPane doc in L)
            {
                doc.CloseActiveContent();

                dockContainer1.RemovePane(doc);
            }

            dockContainer1.Refresh();

            hst = new History();
        }

        public ArrayList GetOpenFiles()
        {
            ArrayList L = new ArrayList();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(Document))
                    L.Add(((Document)doc).FileName);
            }

            return L;
        }
        public ArrayList GetOpenDocuments()
        {
            ArrayList L = new ArrayList();

            foreach (DockContent doc in dockContainer1.Documents)
            {
                if (doc.GetType() == typeof(Document))
                    L.Add(((Document)doc));
            }

            return L;
        }
        public void LoadErrors(ArrayList DD)
        {
            ArrayList B = GetOpenDocuments();

            foreach (Document d in B)
                foreach (Row r in d.syntaxDocument1)
                    foreach (Word w in r)
                        w.HasError = false;

            foreach (IntError e in DD)
            {
                string message = e.e.Message;

                if (message.StartsWith("UnknownIdentifier") == true)
                {
                    string[] cc = e.e.Message.Split(" ".ToCharArray());

                    Document d = GetFileOpened(e.c.FileName);

                    if (d != null)
                    {
                        // MessageBox.Show("File has been found");

                        if (e.e.Region.BeginLine < 0)
                            continue;

                        //d.SuspendLayout();

                        Row r = d.Editor.Document[e.e.Region.BeginLine];

                        foreach (Word w in r)
                        {
                            w.HasError = false;

                            string[] bb = w.Text.Split(";".ToCharArray());

                            //bool changed = false;

                            foreach (string dd in bb)

                                if (dd == cc[1])
                                {
                                    // MessageBox.Show("Identifier found");

                                    w.HasError = true;

                                    //changed = true;

                                    //d.Refresh();
                                }

                            //if (changed == false)
                            //    w.HasError = false;
                        }

                        //d.ResumeLayout();
                    }
                }
            }
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


        public History()
        {
            hst = new ArrayList();
            acts = new ArrayList();
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
            Console.WriteLine("DeSerializing files arraylist");
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
    public class history
    {
        public int type = 0;

        public string file = "";

        public int line = 0;

        public string text = "";
    }
}