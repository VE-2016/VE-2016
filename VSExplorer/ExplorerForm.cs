/*
 * QuickSharp Copyright (C) 2008-2012 Steve Walker.
 *
 *
 */

using QuickSharp.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;

using AIMS.Libraries.CodeEditor;

using AIMS.Libraries.Scripting.ScriptControl;

using GACProject;

using DecompilerProject;

namespace WinExplorer
{
    /// <summary>
    /// The explorer window form.
    /// </summary>
    public partial class ExplorerForm : Form
    											{
        #region Nested Classes

        private class DummyNode : TreeNode { }

        private class FileNamePattern
				        {
            public Regex Regex;
            public bool MatchResult = true;
        				}

        #endregion Nested Classes

        private ApplicationManager _applicationManager;
        private SettingsManager _settingsManager;
        private string _rootFolder;
        private Dictionary<string, string> _documentTypeIconMap;
        private List<string> _expandedPaths;
        private bool _enableTitleBarUpdate;
        private bool _currentDirectoryFollowsRoot;
        private bool _applyFileNameFilter;
        private List<FileNamePattern> _allowedFileNamePatterns;

        /// <summary>
        /// Event raised when the tree view is about to be refreshed.
        /// </summary>
        public event MessageHandler TreeViewPreRefresh;

        /// <summary>
        /// Event raised when the tree view has been refreshed.
        /// </summary>
        public event MessageHandler TreeViewPostRefresh;

        /// <summary>
        /// Event raised when a tree node has been updated.
        /// </summary>
        public event TreeViewNodeUpdateHandler TreeViewNodeUpdate;


        //private const int WM_NCPAINT = 0x0085;
        //[DllImport("user32.dll")]
        //static extern IntPtr GetWindowDC(IntPtr hWnd);
        //protected override void WndProc(ref Message message)
        //{

        //    base.WndProc(ref message);

        //    if (message.Msg == WM_NCPAINT)
        //    {
        //        IntPtr hDC = GetWindowDC(message.HWnd);
        //        Graphics gfx = Graphics.FromHdc(hDC);
        //        gfx.DrawString("c# forum", this.Font, new SolidBrush(Color.Black), new Point(10, 6));
        //        gfx.Dispose();
        //        return;
        //    }

           

        //}

        /// <summary>
        /// Create the explorer window form.
        /// </summary>
        /// <param name="formKey">The GUID key used to identify the form internally.</param>
        public ExplorerForm(string formKey) //: base(formKey)
        {
    
            if (!Directory.Exists(_rootFolder))
                _rootFolder = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(_rootFolder);

            
    

            #region Form Setup and Events

            this._components = new System.ComponentModel.Container();

            InitializeComponent();

            context1 = contextMenuStrip1;
            context2 = contextMenuStrip2;
            context3 = contextMenuStrip3;
            context4 = contextMenuStrip4;
            context5 = contextMenuStrip5;


            this._components = new System.ComponentModel.Container();

            Text = Resources.ExplorerWindowTitle;
            //Icon = Resources.ExplorerIcon;

            VisibleChanged += delegate { RefreshView(); };

            _mainTreeView.AllowDrop = true;

            _mainTreeView.ItemDrag +=
                new ItemDragEventHandler(TreeView_ItemDrag);

            _mainTreeView.DragOver +=
                new DragEventHandler(TreeView_DragOver);

            _mainTreeView.AfterSelect +=
                new TreeViewEventHandler(TreeView_AfterSelect);

            _mainTreeView.DragDrop +=
                new DragEventHandler(TreeView_DragDrop);

            _mainTreeView.BeforeExpand +=
                new TreeViewCancelEventHandler(TreeView_BeforeExpand);

            _mainTreeView.BeforeCollapse +=
                new TreeViewCancelEventHandler(TreeView_BeforeCollapse);

            _mainTreeView.MouseClick +=
                new MouseEventHandler(TreeView_MouseClick);

            _mainTreeView.MouseDoubleClick +=
                new MouseEventHandler(TreeView_MouseDoubleClick);

            _mainTreeView.KeyDown +=
                new KeyEventHandler(TreeView_KeyDown);

   

            if (_enableTitleBarUpdate)
            {
                VisibleChanged += delegate
                {
                    //if (!Visible)
                    //    mainForm.Text = _applicationManager.
                    //        ClientProfile.ClientTitle;
                };
            }

            #endregion Form Setup and Events

            /*
             * The explorer shows a view of the file system so register
             * to be notified if any plugin makes file system changes.
             */

            //        _applicationManager.FileSystemChange +=
            //            new MessageHandler(RefreshView);

            #region Document Type Images

            /*
             * Associate the appropriate filetype image with appropriate
             * supported file extensions.
             */

            _documentTypeIconMap = new Dictionary<String, String>();
            _documentTypeIconMap.Add(".asax", "ASHX");
            _documentTypeIconMap.Add(".ascx", "ASCX");
            _documentTypeIconMap.Add(".ashx", "ASHX");
            _documentTypeIconMap.Add(".asmx", "ASHX");
            _documentTypeIconMap.Add(".aspx", "ASPX");
            _documentTypeIconMap.Add(".bat", "SCRIPT");
            _documentTypeIconMap.Add(".bmp", "IMAGE");
            _documentTypeIconMap.Add(".c", "C");
            _documentTypeIconMap.Add(".cc", "CPP");
            _documentTypeIconMap.Add(".class", "OBJ");
            _documentTypeIconMap.Add(".cmd", "SCRIPT");
            _documentTypeIconMap.Add(".conf", "TEXT");
            _documentTypeIconMap.Add(".config", "XML");
            _documentTypeIconMap.Add(".cpp", "CPP");
            _documentTypeIconMap.Add(".cs", "CSHARP");
            _documentTypeIconMap.Add(".css", "CSS");
            _documentTypeIconMap.Add(".cxx", "CPP");
            _documentTypeIconMap.Add(".dbml", "DBML");
            _documentTypeIconMap.Add(".dll", "DLL");
            _documentTypeIconMap.Add(".erb", "HTML");
            _documentTypeIconMap.Add(".exe", "EXE");
            _documentTypeIconMap.Add(".gif", "IMAGE");
            _documentTypeIconMap.Add(".h", "H");
            _documentTypeIconMap.Add(".hh", "H");
            _documentTypeIconMap.Add(".hpp", "H");
            _documentTypeIconMap.Add(".htm", "HTML");
            _documentTypeIconMap.Add(".html", "HTML");
            _documentTypeIconMap.Add(".ico", "IMAGE");
            _documentTypeIconMap.Add(".il", "MSIL");
            _documentTypeIconMap.Add(".ini", "TEXT");
            _documentTypeIconMap.Add(".java", "JAVA");
            _documentTypeIconMap.Add(".jpeg", "IMAGE");
            _documentTypeIconMap.Add(".jpg", "IMAGE");
            _documentTypeIconMap.Add(".js", "SCRIPT");
            _documentTypeIconMap.Add(".lua", "SCRIPT");
            _documentTypeIconMap.Add(".master", "ASPX");
            _documentTypeIconMap.Add(".o", "OBJ");
            _documentTypeIconMap.Add(".obj", "OBJ");
            _documentTypeIconMap.Add(".pdb", "PDB");
            _documentTypeIconMap.Add(".php", "HTML");
            _documentTypeIconMap.Add(".pl", "SCRIPT");
            _documentTypeIconMap.Add(".png", "IMAGE");
            _documentTypeIconMap.Add(".proj", "PROJ");
            _documentTypeIconMap.Add(".py", "SCRIPT");
            _documentTypeIconMap.Add(".rb", "SCRIPT");
            _documentTypeIconMap.Add(".resx", "RESOURCE");
            _documentTypeIconMap.Add(".rhtml", "HTML");
            _documentTypeIconMap.Add(".sdf", "DATABASE");
            _documentTypeIconMap.Add(".sitemap", "XML");
            _documentTypeIconMap.Add(".sql", "SQL");
            _documentTypeIconMap.Add(".sqlite", "DATABASE");
            _documentTypeIconMap.Add(".txt", "TEXT");
            _documentTypeIconMap.Add(".vb", "VBNET");
            _documentTypeIconMap.Add(".wsdl", "XML");
            _documentTypeIconMap.Add(".xaml", "XAML");
            _documentTypeIconMap.Add(".xap", "CAB");
            _documentTypeIconMap.Add(".xml", "XML");
            _documentTypeIconMap.Add(".xsd", "XSD");
            _documentTypeIconMap.Add(".xsl", "XML");
            _documentTypeIconMap.Add(".yml", "TEXT");
            _documentTypeIconMap.Add(".zip", "CAB");

            #endregion Document Type Images

            /*
             * Keep a list of any expanded nodes for repopulating on a refresh
             */

            _expandedPaths = new List<string>();

            Application.EnableVisualStyles();

            RefreshView();

            addview(scriptControl1);

            //LoadListView();

            //addview(listview);

            //LoadListTab();

            //addview(bf);

            LoadProjectForm();

            addview(pf);

            _currentDirectoryFollowsRoot = true;

            loadcurrentproject();

            loadtoolmenu();

            loadcontmenu();

            _currentDirectoryFollowsRoot = true;


            CreateOutputWindow();

           
            forms.lv.MouseDoubleClick += new MouseEventHandler(ListViewListView_MouseDoubleClick);

           
            sv._SolutionTreeView.ContextMenuStrip = contextMenuStrip1;

            sv._SolutionTreeView.FullRowSelect = true;

            sv._SolutionTreeView.ShowLines = false;

            //sv.context = context;

            toolStrip4.BringToFront();

            CreateProgressBar();

            

            //CompletedChange += new EventHandler(OnCompletedChange);

           // OnCompletedChange(this, new EventArgs());

            treeView1.MouseUp += new MouseEventHandler(treeView1_MouseUp);

            treeView2.MouseUp += new MouseEventHandler(treeView2_MouseUp);

            sv._configList.SelectedIndexChanged += new EventHandler(comboBox1_DrawItem);

            toolStrip5.GripMargin = Padding.Empty;

            toolStrip5.GripStyle = ToolStripGripStyle.Hidden;

            cc.CreateClassViewer(treeView2);

            treeView2.ShowLines = false;

            sv._SolutionTreeView.AfterSelect += new TreeViewEventHandler(AfterSelect);

            scr = scriptControl1;

            sv.scr = scr;

            ImageList imgs = CreateView_Solution.CreateImageList();
            VSProject.dc = CreateView_Solution.GetDC();

            CodeEditorControl.AutoListImages = imgs;

            CodeEditorControl.settings = new Settings();

            scr.DisplayTopStrip(false);

            toolStripStatusLabel1.Width = 100;
            toolStripStatusLabel3.Width = 100;

            scr.SetToolStripLabel(toolStripStatusLabel1, toolStripStatusLabel3);

            scr.DisplayStatusStrip(false);

            scr.HistoryChange += new EventHandler<EventArgs>(HistoryChanged);

            tabControl1.SelectedIndex = 2;

            GACForm.GetAssembliesList();

            ResumeAtStartUp();
        }

        ScriptControl scr { get; set; }

        public void HistoryChanged(object sender, EventArgs e)
        {
            sv.HistoryChanged(scr.hst.hst);
        }

        private void ListViewListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;
            
            if (lv.SelectedItems.Count == 0) return;

            ListViewItem v = lv.SelectedItems[0];

            if(v.Tag == null)
                return;

            Microsoft.Build.Framework.BuildWarningEventArgs ws = v.Tag as Microsoft.Build.Framework.BuildWarningEventArgs;

            if (ws != null)
            {

                string s = Path.GetDirectoryName(ws.ProjectFile);

                eo.LoadFile(s + "\\" + ws.File, null, null, null );

                

                eo.SelectText(ws.LineNumber, ws.ColumnNumber, 10);

            }
            else
            {
                Microsoft.Build.Framework.BuildErrorEventArgs es = v.Tag as Microsoft.Build.Framework.BuildErrorEventArgs;

                string s = Path.GetDirectoryName(es.ProjectFile);

                eo.LoadFile(s + "\\" + es.File, null, null, null);

                eo.SelectText(es.LineNumber, es.ColumnNumber, 10);
            }
            

        }

        private void comboBox1_DrawItem(object sender, EventArgs e)
        {
            ToolStripComboBox c = sender as ToolStripComboBox;

            int i = c.SelectedIndex;

            if (i == 2)

                LoadConfigurationManager();


        }

        ConfigurationManagerForm cmf { get; set; }
        public void LoadConfigurationManager()
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            if (vs == null)
                return;

            cmf = new ConfigurationManagerForm();
            cmf.LoadConfigs(vs);
            cmf.ShowDialog();
        }

        ContextMenuStrip context1 { get; set; }
        ContextMenuStrip context2 { get; set; }
        ContextMenuStrip context3 { get; set; }
        ContextMenuStrip context4 { get; set; }

        ContextMenuStrip context5 { get; set; }


        QuickSharp.Outputs.OutputForms forms { get; set; }

        public void CreateOutputWindow()
        {

            //ApplicationManager app = ApplicationManager.GetInstance();

            
            forms = new QuickSharp.Outputs.OutputForms();

            forms.TopLevel = false;

            forms.Dock = DockStyle.Fill;

            forms.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            //forms.Show();

            splitContainer2.Panel2.Controls.Add(forms);

            forms.Show();

            splitContainer2.SplitterDistance = (int)((double)splitContainer2.Height * 0.8);

                

        }

        public void ClearOutput()
        {

            forms.ClearOutput();
        }

        public SolutionFormProject.ProjectForm pf { get; set; }

        public void LoadProjectForm()
        {
            pf = new SolutionFormProject.ProjectForm();
            pf.Dock = DockStyle.Fill;
            pf.TopLevel = false;
            pf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            splitContainer1.Panel2.Controls.Add(pf);

            pf.Show();


        }

        public string currentproject { get; set; }

        public ListView listview { get; set; }

        public void LoadListView()
        {

            listview = new ListView();

            listview.Dock = DockStyle.Fill;

            listview.View = View.Details;

            listview.FullRowSelect = true;

            listview.Columns.Add("Name");

            listview.Columns.Add("Value");

            splitContainer1.Panel2.Controls.Add(listview);

            update_z_order(this);

        }

       // Cyotek.Windows.Forms.Demo.MainTabForm bf { get; set; }

        //public void LoadListTab(Form f)
        //{

        //    bf = new Cyotek.Windows.Forms.Demo.MainTabForm();
        //    bf.Dock = DockStyle.Fill;
        //    bf.TopLevel = false;
        //    bf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

        //    f.Controls.Add(bf);

        //    bf.Show();

        //    WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

        //    if (prs == null)
        //    {
        //        MessageBox.Show("No project has been selected..");
        //        return;

        //    }



        //    //MainProjectNode = eo.cvs.getactivenode();

        //    if (prs.ps == null)
        //        return;

        //    Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(prs.ps.FileName);


        //    bf.pc = pc;

        //    bf.vp = prs.ps;

        //    bf.GetProperties();

        //}


        public void loadcurrentproject()
        {
            try
            {
                string d = AppDomain.CurrentDomain.BaseDirectory;

                string s = File.ReadAllText(d + "\\" + "currentproject.txt");

                eo.currentproject = s;
            }
            catch (Exception e) { };
        }

        public void loadcontmenu()
        {
            eo._applicationManager = _applicationManager;
            eo._applyFileNameFilter = _applyFileNameFilter;
            eo._currentDirectoryFollowsRoot = _currentDirectoryFollowsRoot;
            eo._documentTypeIconMap = _documentTypeIconMap;
            eo._expandedPaths = _expandedPaths;
            eo._rootFolder = _rootFolder;
            eo._settingsManager = _settingsManager;
            eo._components = _components;
            eo.master = this;
            eo.tw = tw;

            
            tc.eo = eo;
            tc._mainTreeView = _mainTreeView;
            tc.doinit();
        }

        public void loadtoolmenu()
        {
            eo._applicationManager = _applicationManager;
            eo._applyFileNameFilter = _applyFileNameFilter;
            eo._currentDirectoryFollowsRoot = true;// _currentDirectoryFollowsRoot;
            eo._documentTypeIconMap = _documentTypeIconMap;
            eo._expandedPaths = _expandedPaths;
            eo._rootFolder = _rootFolder;
            eo._settingsManager = _settingsManager;
            eo._components = _components;
            eo.tw = tw;
            eo.script = scriptControl1;

            tw.eo = eo;
            ts.eo = eo;
            ts._mainTreeView = _mainTreeView;
            ts.toolStrip1 = toolStrip1;
            ts.doinit(this);

            cc.eo = eo;
            cc._maintv = ts._mainTreeView;// _mainTreeView;
            cc._mainToolStrip = toolStrip4;
            cc.doinit(this);

            sv.eo = eo;
            sv._maintv = ts._mainTreeView;// _mainTreeView;
            sv._mainToolStrip = toolStrip4;
            sv.doinit(this, splitContainer1, tabPage3, treeView1, treeView2, listview, null);
            sv.hst.tv = treeView3;
            sv.hst.loaddefaults();
            sv.context1 = contextMenuStrip1;
            sv.context2 = contextMenuStrip2;

            //sv.eo = eo;
            //sv._maintv = ts._mainTreeView;// _mainTreeView;
            //sv._mainToolStrip = ts._mainToolStrip;
            //sv.doinit(this);

            ls.eo = eo;
            ls._maintv = ts._mainTreeView;// _mainTreeView;
            ls._mainToolStrip = toolStrip4;
            ls.doinit(this);
            

            eo.cvs = sv;

            update_z_order(tabPage1);
        }

        private WinExplorer.ExplorerObject eo = ExplorerObject.GetInstance();

        private WinExplorer.TreeViewBase tw = new TreeViewBase();

        private WinExplorer.ToolStripCreator tc = new ToolStripCreator();

        private WinExplorer.ToolStripCreators ts = new ToolStripCreators();

        private WinExplorer.CreateView cc = new CreateView();

        private WinExplorer.CreateView_Solution sv = new CreateView_Solution();

        private WinExplorer.CreateView_Logger ls = new CreateView_Logger();

        /// <summary>
        /// Set the form's initial state when there is no saved configuration
        /// to be restored from the previous session.
        /// </summary>
        //protected override void SetFormDefaultState()
        //{
        //    ClientProfile profile = _applicationManager.ClientProfile;

        //    if (profile.HaveFlag(ClientFlags.ExplorerDockRightByDefault))
        //        DockState = DockState.DockRight;
        //    else
        //        DockState = DockState.DockLeft;

        //    if (profile.HaveFlag(ClientFlags.ExplorerHideByDefault))
        //        Hide();
        //    else
        //        Show();

        //}

        private void UpdateUI()
        {
            //SetFileNameFilter();
            RefreshView();
        }

        #region Toolbar Events

        private void MoveToolStripButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = Resources.BrowseFolderDialogText;

            if (fbd.ShowDialog() == DialogResult.OK)
                MoveToSelectedFolder(fbd.SelectedPath);
        }

        #endregion Toolbar Events

        #region Form Events

        private void MainForm_ThemeActivated()
        {
            ThemeFlags flags = _applicationManager.
                ClientProfile.ThemeFlags;

            if (flags != null)
            {
                if (flags.MainBackColor != Color.Empty)
                    BackColor = flags.MainBackColor;

                if (flags.ViewBackColor != Color.Empty)
                    TreeView.BackColor = flags.ViewBackColor;

                if (flags.ViewForeColor != Color.Empty)
                    TreeView.ForeColor = flags.ViewForeColor;

                if (flags.ViewShowBorder == false)
                    _mainTreeView.BorderStyle = BorderStyle.None;

                if (flags.MenuForeColor != Color.Empty)
                    MenuTools.ContextMenuStripItemsSetForeColor(
                        TreeViewContextMenu,
                        flags.MenuForeColor,
                        flags.MenuHideImages);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                _settingsManager.ApplyFilter = _applyFileNameFilter;
                _settingsManager.RootFolder = _rootFolder;
                _settingsManager.Save();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion Form Events

        #region Public UI Elements

        /// <summary>
        /// The root folder of the explorer window tree view.
        /// </summary>
        public string RootFolder
        {
            get { return _rootFolder; }
            set { _rootFolder = value; }
        }

        /// <summary>
        /// The explorer window toolbar.
        /// </summary>
        public ToolStrip Toolbar
        {
            get { return _mainToolStrip; }
        }

        /// <summary>
        /// The explorer window tree view.
        /// </summary>
        public TreeView TreeView
        {
            get { return _mainTreeView; }
        }

        /// <summary>
        /// The explorer window tree view's context menu.
        /// </summary>
        public ContextMenuStrip TreeViewContextMenu
        {
            get { return new ContextMenuStrip(); }

            //get { return _treeViewMenu; }
        }

        /// <summary>
        /// The image list used to provide images to the explorer window tree view.
        /// </summary>
        //public ImageList DocumentImageList
        //{
        //    get { return _treeViewImageList; }
        //}

        /// <summary>
        /// A dictionary mapping file types to the names of the images used to represent them.
        /// The names are the keys of the images in the image list.
        /// </summary>
        public Dictionary<String, String> DocumentTypeIconMap
        {
            get { return _documentTypeIconMap; }
        }

        #endregion Public UI Elements

        #region Drag and Drop

        private void TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            /*
             * Allow drag and drop with left mouse button.
             * Actual effect is selected in DragOver.
             */

            if (e.Button == MouseButtons.Left)
                DoDragDrop(e.Item, DragDropEffects.All);
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            /*
             * Get the source and target nodes.
             */

            TreeNode targetNode = _mainTreeView.GetNodeAt(
                _mainTreeView.PointToClient(new Point(e.X, e.Y)));

            TreeNode draggedNode =
                (TreeNode)e.Data.GetData(typeof(TreeNode));

            /*
             * Items may only be moved to folders. Files
             * may be moved or copied. Folders may only be
             * moved. Copy if Ctrl key pressed, otherwise move.
             */

            if (NodeIsFolder(targetNode))
            {
                if ((e.KeyState & 8) == 8)
                {
                    if (NodeIsFolder(draggedNode))
                        e.Effect = DragDropEffects.None;
                    else
                        e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            else
                e.Effect = DragDropEffects.None;

            /*
             * Select the target node as the mouse is dragged
             * over it.
             */

            _mainTreeView.SelectedNode = targetNode;
        }

        private void TreeView_DragDrop(object sender, DragEventArgs e)
        {
            /*
             * Get the source and target nodes.
             */

            TreeNode targetNode = _mainTreeView.GetNodeAt(
                _mainTreeView.PointToClient(new Point(e.X, e.Y)));

            TreeNode draggedNode =
                (TreeNode)e.Data.GetData(typeof(TreeNode));

            /*
             * Check that the node at the drop location is not
             * the dragged node or its descendant.
             */

            if (draggedNode.Equals(targetNode)) return;
            if (NodeContainsNode(draggedNode, targetNode)) return;

            string sourcePath = draggedNode.Tag as string;
            string targetFolder = targetNode.Tag as string;
            string targetPath = Path.Combine(
                targetFolder, Path.GetFileName(sourcePath));

            /*
             * Move or copy the source node to the target node.
             */

            if (e.Effect == DragDropEffects.Move)
            {
                try
                {
                    if (NodeIsFolder(draggedNode))
                    {
                        /*
                         * Can't move a folder if the current directory is
                         * within the path being moved so set the current
                         * directory to the root as a precaution.
                         */

                        Directory.SetCurrentDirectory(_rootFolder);
                        Directory.Move(sourcePath, targetPath);
                    }
                    else
                    {
                        File.Move(sourcePath, targetPath);
                    }

                    /*
                     * Update any open editors with new filepaths after move.
                     */

                    //List<Document> documents = new List<Document>();

                    //foreach (Document d in mainForm.ClientWindow.Documents)
                    //{
                    //    if (d.FilePath.ToLower().StartsWith(sourcePath,
                    //            StringComparison.CurrentCultureIgnoreCase))
                    //        d.FilePath = targetPath +
                    //            d.FilePath.Remove(0, sourcePath.Length);
                    //}

                    /*
                     * Mark target folder for expansion on refresh so we can
                     * see the moved file/folder.
                     */

                    if (!_expandedPaths.Contains(targetFolder))
                        _expandedPaths.Add(targetFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.MoveErrorMessage, ex.Message),
                        Resources.MoveErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (e.Effect == DragDropEffects.Copy)
            {
                try
                {
                    File.Copy(sourcePath, targetPath);

                    /*
                     * Mark target folder for expansion on refresh so we can
                     * see the new file.
                     */

                    if (!_expandedPaths.Contains(targetFolder))
                        _expandedPaths.Add(targetFolder);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("{0}\r\n{1}",
                            Resources.CopyErrorMessage, ex.Message),
                        Resources.CopyErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //_applicationManager.NotifyFileSystemChange();
        }

        #endregion Drag and Drop

        #region Visited Folder List

        /// <summary>
        /// Refresh the list of visited folders in the explorer window toolbar.
        /// </summary>
        public void RefreshVisitedFolderList()
        {
            //_visitedFolderSelectButton.DropDownItems.Clear();
            //_visitedFolderSelectButton.Enabled = false;

            //// Create a new list of only the folders that exist.
            //List<string> validFoldersList = new List<string>();

            //ThemeFlags flags = _applicationManager.
            //    ClientProfile.ThemeFlags;

            //bool applyTheme = flags != null &&
            //    flags.MenuForeColor != Color.Empty;

            //foreach (string folder in _settingsManager.VisitedFoldersList)
            //{
            //    if (!Directory.Exists(folder)) continue;

            //    ToolStripMenuItem item = new ToolStripMenuItem();
            //    item.Text = Path.GetFileName(folder);
            //    if (item.Text == String.Empty) item.Text = folder;
            //    item.ToolTipText = folder;
            //    item.Click += new EventHandler(
            //        VisitedFolderDropDownItem_Click);

            //    // Apply theme color if available.
            //    if (applyTheme) item.ForeColor = flags.MenuForeColor;

            //    // Insert so that latest file appears first.
            //    _visitedFolderSelectButton.DropDownItems.Insert(0, item);

            //    validFoldersList.Add(folder);
            //}

            //if (_visitedFolderSelectButton.DropDownItems.Count > 0)
            //    _visitedFolderSelectButton.Enabled = true;

            //// Retain the updated list.
            //_settingsManager.VisitedFoldersList = validFoldersList;
        }

        private void VisitedFolderDropDownItem_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null) return;

            string path = item.ToolTipText;

            if (Directory.Exists(path))
            {
                MoveToSelectedFolder(path);
            }
            else
            {
                _settingsManager.RemoveFolderFromVisitedList(path);
                //if (_settingsManager.VisitedFoldersList.Count == 0)
                //    _visitedFolderSelectButton.Enabled = false;
            }
        }

        #endregion Visited Folder List

        #region File Name Filters

        /*
         * Entry point - the tree refresh view calls this method
         */

        private bool ApplyNameFileFilter(string name)
        {
            if (!_applyFileNameFilter || _allowedFileNamePatterns == null)
                return true;

            /*
             * Each file starts hidden and each filter can make it visible.
             * If none do so the file remains hidden. Once a file is visible
             * it can't be hidden again by the next filter. To use a negative
             * match preceed the pattern with "!". This will return false for
             * a match instead of true. Remember the filters "opt in" so if
             * you have a negative filter it has to be followed by something
             * to opt the remaining files back in (use "(?i:\.+)" as a
             * universal match). The filters are applied in the order they
             * are listed in the pattern, the first to create a match
             * determines the visibility of the file.
             */

            foreach (FileNamePattern pattern in _allowedFileNamePatterns)
                if (pattern.Regex.Match(name).Success == true)
                    return pattern.MatchResult;

            return false;
        }

        /*
         * Update the list of file patterns and the filter status.
         */

        //private void SetFileNameFilter()
        //{
        //    string selectedFilter =
        //        _settingsManager.SelectedFilter;

        //    // Validate the selected filter key
        //    if (String.IsNullOrEmpty(selectedFilter) ||
        //        !_settingsManager.FileFilters.ContainsKey(selectedFilter))
        //    {
        //        _settingsManager.SelectedFilter = String.Empty;
        //        _allowedFileNamePatterns = null;
        //        _applyFileNameFilter = false;
        //        _filterButton.Checked = false;
        //        _filterButton.Enabled = false;
        //        _filterButton.ToolTipText = Resources.ToolbarFilter;
        //        return;
        //    }

        //    // Get the filter
        //    FileFilter filter = _settingsManager.
        //        FileFilters[selectedFilter];

        //    // Update the button
        //    _filterButton.ToolTipText = String.Format("{0}: {1}",
        //        Resources.ToolbarFilter, filter.Name);

        //    // Convert to a list of file patterns
        //    List<FileNamePattern> list = new List<FileNamePattern>();

        //    string[] split = filter.Filter.Split(' ');

        //    if (split.Length > 0)
        //    {
        //        foreach (string s in split)
        //        {
        //            if (s == String.Empty)
        //                continue;

        //            try
        //            {
        //                FileNamePattern pattern = new FileNamePattern();
        //                string re = s;

        //                if (re.StartsWith("!"))
        //                {
        //                    pattern.MatchResult = false;
        //                    re = re.Substring(1);
        //                }

        //                // Pseudo pattern - global match
        //                if (re == Constants.PSEUDO_PATTERN_GLOBAL)
        //                    re = "(?!:.+)";

        //                pattern.Regex = new Regex(re);

        //                list.Add(pattern);
        //            }
        //            catch
        //            {
        //                /*
        //                 * Ignore errors here, just carry on with what's left.
        //                 * Regex is validated in management UI - presenting errors
        //                 * here causes dialogs to popup too frequently with no
        //                 * real clue as to why.
        //                 */
        //            }
        //        }

        //        _allowedFileNamePatterns = list;
        //    }
        //}

        ///*
        // * Toggle the filter via the toolbar button.
        // */

        //private void ToggleFileFilter()
        //{
        //    _filterButton.Checked = !_filterButton.Checked;
        //    _applyFileNameFilter = _filterButton.Checked;

        //    UpdateUI();
        //}

        ///*
        // * Update the drop-down list when opened.
        // */

        //private void RefreshFileFilterList()
        //{
        //    /*
        //     * Enable/disable the buttons.
        //     */

        //    if (_settingsManager.FileFilters.Keys.Count == 0)
        //    {
        //        _filterButton.Enabled = false;
        //        _filterButton.Checked = false;
        //        _filterSelectButton.Enabled = false;
        //        return;
        //    }

        //    _filterButton.Enabled = _settingsManager.
        //        SelectedFilter != String.Empty;

        //    _filterSelectButton.Enabled = true;

        //    /*
        //     * Refresh the menu.
        //     */

        //    _filterSelectButton.DropDownItems.Clear();

        //    ThemeFlags flags = _applicationManager.
        //        ClientProfile.ThemeFlags;

        //    bool applyTheme = flags != null &&
        //        flags.MenuForeColor != Color.Empty;

        //    /*
        //     * Sort the filters.
        //     */

        //    List<FileFilter> filters = _settingsManager.FileFilters.
        //        Values.ToList<FileFilter>();

        //    filters.Sort(new FileFilterComparer());

        //    foreach (FileFilter filter in filters)
        //    {
        //        ToolStripMenuItem item = new ToolStripMenuItem();
        //        item.Name = filter.ID;
        //        item.Text = filter.Name;
        //        item.Tag = filter.Filter;
        //        item.Click += delegate
        //        {
        //            _settingsManager.SelectedFilter = item.Name;
        //            UpdateUI();
        //        };

        //        if (_settingsManager.SelectedFilter == filter.ID)
        //            item.Checked = true;

        //        // Apply theme color if available.
        //        if (applyTheme) item.ForeColor = flags.MenuForeColor;

        //        _filterSelectButton.DropDownItems.Add(item);
        //    }
        //}

        /// <summary>
        /// Handles the AfterSelect event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null)
                return;

            object tag = e.Node.Tag;

            if (tag.GetType() != typeof(VSProjectItem))
                return;

            VSProjectItem r = ((VSProjectItem)tag);

            if (r.fileName == null)
                return;

            //scriptControl1.OpenDocument(r.fileName,"", null);

            //_applicationManager.MainForm.LoadDocumentIntoWindow(r.fileName, true);



            //// tec.LoadFile(r.fileName);

            // projectfilename = r.fileName;

            //// if (hs == null)
            ////     return;

            // ClassMapper mapper = null;

            // if (ms == null)
            //     return;

            // mapper = getmapperclass(r.fileName);

            // if (mapper == null)
            //     return;

            // if (mapper.methods == null)
            //     return;

            // //if (gc == null)
            // //    return;

            // //ComboBox cb = gc.cb1;

            // //cb.Items.Clear();

            // //foreach (MethodCreator m in mapper.methods)
            // //{
            // //    string s = m.name.Replace(mapper.Namespace, "");

            // //    cb.Items.Add(s);
            // //}
        }

        #endregion File Name Filters

        #region Tree View

        /// <summary>
        /// Refresh the explorer window tree view.
        /// </summary>
        public void RefreshView()
        {
            if (!Visible) return;

            /*
             * Call pre refresh event handlers.
             */

            if (TreeViewPreRefresh != null)
                TreeViewPreRefresh();

            /*
             * Save the top visible node for restoration after the refresh.
             * We can't use the TopNode property directly as we will have
             * a completely new set of nodes.
             */

            string topVisibleNodeKey = null;
            string topVisibleNodePath = null;

            if (_mainTreeView.TopNode != null)
            {
                topVisibleNodeKey = _mainTreeView.TopNode.Name;
                topVisibleNodePath = _mainTreeView.TopNode.FullPath;
            }

            /*
             * Save the currently selected node so that it can be
             * restored after the refresh. We can't save the node
             * itself as the refresh creates an entirely new set
             * of nodes.
             */

            string selectedNodePath = null;

            if (_mainTreeView.SelectedNode != null)
                selectedNodePath =
                    _mainTreeView.SelectedNode.Tag as String;

            /*
             * Update the title bar with the current location.
             */

            if (_enableTitleBarUpdate)
            {
                string folder = _rootFolder;

                if (!_settingsManager.ShowFullPath)
                {
                    folder = Path.GetFileName(_rootFolder);
                    if (folder == String.Empty)
                        folder = Path.GetPathRoot(_rootFolder);
                }

                //mainForm.Text = String.Format("{0} - {1}",
                //    folder, _applicationManager.ClientProfile.ClientTitle);
            }

            /*
             * Create the root node.
             */

            string rootName = Path.GetFileName(_rootFolder);
            if (rootName == String.Empty)
                rootName = Path.GetPathRoot(_rootFolder);

            TreeNode rootNode = new TreeNode();
            rootNode.Text = rootNode.Name = rootName;
            rootNode.Tag = _rootFolder;

            if (_rootFolder == Directory.GetCurrentDirectory())
                rootNode.ImageKey = rootNode.SelectedImageKey =
                    Constants.OPENED_FOLDER_IMAGE;
            else
                rootNode.ImageKey = rootNode.SelectedImageKey =
                    Constants.CLOSED_FOLDER_IMAGE;

            if (TreeViewNodeUpdate != null)
                TreeViewNodeUpdate(ref rootNode);

            if (rootNode == null) return;

            _mainTreeView.BeginUpdate();
            _mainTreeView.Nodes.Clear();
            _mainTreeView.Nodes.Add(rootNode);

            /*
             * populate the tree.
             */

            PopulateTreeNode(rootNode.Nodes, _rootFolder, selectedNodePath);

            rootNode.Expand();

            _mainTreeView.EndUpdate();

            if (_mainTreeView.SelectedNode == null)
                _mainTreeView.SelectedNode = rootNode;

            /*
             * Restore the top node if still present in the tree.
             */

            if (topVisibleNodeKey != null && topVisibleNodePath != null)
            {
                TreeNode[] nodes =
                    _mainTreeView.Nodes.Find(topVisibleNodeKey, true);

                foreach (TreeNode node in nodes)
                {
                    if (node.FullPath == topVisibleNodePath)
                    {
                        _mainTreeView.TopNode = node;
                        break;
                    }
                }
            }

            /*
             * Update the visited folder list button state.
             */

            //_visitedFolderSelectButton.Enabled =
            //    _settingsManager.VisitedFoldersList.Count > 0;

            ///*
            // * Update the file filters button state.
            // */

            //_filterButton.Enabled =
            //    _settingsManager.FileFilters.Count > 0 &&
            //    _settingsManager.SelectedFilter != String.Empty;

            //_filterSelectButton.Enabled =
            //    _settingsManager.FileFilters.Count > 0;

            /*
             * Call post refresh event handlers.
             */

            if (TreeViewPostRefresh != null)
                TreeViewPostRefresh();
        }

        private void PopulateTreeNode(TreeNodeCollection root,
            String path, String selectedNodePath)
        {
            //bool hideHiddenFiles = !_settingsManager.ShowHiddenFiles;
            //bool hideSystemFiles = !_settingsManager.ShowSystemFiles;

            root.Clear();

            foreach (string folder in Directory.GetDirectories(path))
            {
                DirectoryInfo di = new DirectoryInfo(folder);

                //if (hideHiddenFiles &&
                //    ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                //    continue;

                //if (hideSystemFiles &&
                //    ((di.Attributes & FileAttributes.System) == FileAttributes.System))
                //    continue;

                TreeNode node = new TreeNode();
                node.Text = node.Name = Path.GetFileName(folder);
                node.Tag = folder;

                if (folder == Directory.GetCurrentDirectory())
                    node.ImageKey = node.SelectedImageKey =
                        Constants.OPENED_FOLDER_IMAGE;
                else
                    node.ImageKey = node.SelectedImageKey =
                        Constants.CLOSED_FOLDER_IMAGE;

                if (TreeViewNodeUpdate != null)
                    TreeViewNodeUpdate(ref node);

                if (node != null)
                {
                    root.Add(node);

                    if (selectedNodePath != null &&
                        selectedNodePath == node.Tag as String)
                        _mainTreeView.SelectedNode = node;

                    if (_expandedPaths.Contains(folder))
                    {
                        PopulateTreeNode(node.Nodes, folder, selectedNodePath);
                        node.Expand();
                    }
                    else
                        node.Nodes.Add(new DummyNode());
                }
            }

            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(file);

                //if (hideHiddenFiles &&
                //    ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                //    continue;

                //if (hideSystemFiles &&
                //    ((fi.Attributes & FileAttributes.System) == FileAttributes.System))
                //    continue;

                TreeNode node = new TreeNode();
                node.Text = node.Name = Path.GetFileName(file);
                node.Tag = file;

                DocumentType documentType = new DocumentType(fi.Extension);
                if (_documentTypeIconMap.ContainsKey(documentType.ToString()))
                    node.ImageKey = node.SelectedImageKey =
                        _documentTypeIconMap[documentType.ToString()];
                else
                    node.ImageKey = node.SelectedImageKey =
                        Constants.DOCUMENT_IMAGE;

                if (TreeViewNodeUpdate != null)
                    TreeViewNodeUpdate(ref node);

                if (node != null && ApplyNameFileFilter(node.Name))
                {
                    root.Add(node);

                    if (selectedNodePath != null &&
                        selectedNodePath == node.Tag as String)
                        _mainTreeView.SelectedNode = node;
                }
            }
        }

        #endregion Tree View

        #region Tree View Events

        private void TreeView_BeforeExpand(
            object sender, TreeViewCancelEventArgs e)
        {
            string path = e.Node.Tag as string;

            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0] is DummyNode)
                PopulateTreeNode(e.Node.Nodes, path, null);

            if (!_expandedPaths.Contains(path))
                _expandedPaths.Add(path);
        }

        private void TreeView_BeforeCollapse(
            object sender, TreeViewCancelEventArgs e)
        {
            string path = e.Node.Tag as string;

            if (_expandedPaths.Contains(path))
                _expandedPaths.Remove(path);
        }

        private void TreeView_MouseClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = _mainTreeView.GetNodeAt(e.Location);
                if (node == null) return;

                _mainTreeView.SelectedNode = node;
            }
        }

        private void TreeView_MouseDoubleClick(
            object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                /*
                 * Deliberately NOT using NodeMouseClick or e.Node
                 * here. Node returned is incorrect if the control
                 * scrolls as a result of the double click.
                 */

                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                if (!NodeIsFolder(node))
                {
                    /*
                     * Don't use OpenNode - we don't want to
                     * open folders by double clicking as this
                     * conflicts with expanding/collapsing the node.
                     */

                    string path = node.Tag as string;

                    //if (path != null)
                    //    mainForm.LoadDocumentIntoWindow(path, true);
                }
            }
        }

        private void TreeView_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                OpenNode(node);
            }

            if (e.KeyCode == Keys.Delete)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                DeleteNode(node);
            }

            if (e.KeyCode == Keys.F2)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                RenameNode(node);
            }

            if (e.KeyCode == Keys.Back)
                MoveToParent();
        }

        #endregion Tree View Events

        #region Tree View Menu Events

        private void treeViewMenu_Opening(object sender, CancelEventArgs e)
        {
            //UI_TREE_MENU_OPEN.Enabled = false;
            //UI_TREE_MENU_RENAME.Enabled = false;
            //UI_TREE_MENU_CLONE.Enabled = false;
            //UI_TREE_MENU_DELETE.Enabled = false;
            //UI_TREE_MENU_CREATE_FOLDER.Enabled = false;
            //UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = false;

            //if (_mainTreeView.Nodes.Count == 0) return;
            //if (_mainTreeView.SelectedNode == null) return;

            //UI_TREE_MENU_OPEN.Enabled = true;
            //UI_TREE_MENU_RENAME.Enabled = true;
            //UI_TREE_MENU_DELETE.Enabled = true;

            //TreeNode node = _mainTreeView.SelectedNode;
            //if (node == null) return;

            //if (NodeIsFolder(node))
            //{
            //    UI_TREE_MENU_CREATE_FOLDER.Enabled = true;
            //    UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = true;

            //    /*
            //     * Don't allow the root node to be deleted or renamed.
            //     */

            //    if (node.Parent == null)
            //    {
            //        UI_TREE_MENU_RENAME.Enabled = false;
            //        UI_TREE_MENU_DELETE.Enabled = false;
            //    }
            //}
            //else
            //{
            //    UI_TREE_MENU_CLONE.Enabled = true;
            //}
        }

        private void UI_TREE_MENU_OPEN_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            OpenNode(node);
        }

        private void UI_TREE_MENU_RENAME_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;
            if (node.Parent == null) return;

            RenameNode(node);
        }

        public msbuilder_alls ms { get; set; }

        public string slnpath { get; set; }

        public void LoadProject()
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            string c = b + "\\Plugins\\Solution\\TEWI-Platform\\";

            string s = Path.GetFullPath(c + "Rough_Sets_Segmentation.sln");

            TreeView msv = TreeView;

            VSProvider.VSSolution vs = new VSProvider.VSSolution(s);

            ms = new msbuilder_alls();

            string p = Path.GetFullPath(c/*"Plugins\\Solution\\TEWI-Platform"*/);

            Directory.SetCurrentDirectory(p);

            ms.tester(msv, "Rough_Sets_Segmentation.sln","");

            slnpath = c;

            //"\\Plugins\\Solution\\TEWI-Platform\\Rough_Sets_Segmentation.sln";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void LoadProject(string sln)
        {
            TreeNode node = _mainTreeView.SelectedNode;

            sln = node.Tag as string;

            string b = AppDomain.CurrentDomain.BaseDirectory;

            //string c = b + "\\Plugins\\Solution\\TEWI-Platform\\";

            //string s = Path.GetFullPath(c + "Rough_Sets_Segmentation.sln");

            string s = sln;

            TreeView msv = TreeView;

            VSProvider.VSSolution vs = new VSProvider.VSSolution(s);

            ms = new msbuilder_alls();

            string p = Path.GetDirectoryName(s/*"Plugins\\Solution\\TEWI-Platform"*/);

            Directory.SetCurrentDirectory(p);

            string g = Path.GetFileName(s);

            ms.tester(msv, g/*"Rough_Sets_Segmentation.sln"*/,"");

            slnpath = s;

            //"\\Plugins\\Solution\\TEWI-Platform\\Rough_Sets_Segmentation.sln";

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void UI_TREE_MENU_LOAD_PROJECT_Click(object sender, EventArgs e)
        {
            LoadProject();

            return;

            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            string path = node.Tag as string;
            if (path == null) return;

            CreateFolder(path);
        }

        private void UI_TREE_MENU_LOAD_PROJECT_DIR_Click(object sender, EventArgs e)
        {
            loadcontmenu();

            LoadProject(slnpath);

            return;

            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            string path = node.Tag as string;
            if (path == null) return;

            CreateFolder(path);
        }

        private void UI_TREE_MENU_BUILD_PROJECT_Click(object sender, EventArgs e)
        {
            if (slnpath == "")
                return;
            if (slnpath == null)
                return;

            string b = AppDomain.CurrentDomain.BaseDirectory;

            string c = b + "\\Plugins\\Solution\\TEWI-Platform\\";

            string s = Path.GetFullPath(c + "Rough_Sets_Segmentation.sln");

            //slnpath = s;

            //"\\Plugins\\Solution\\TEWI-Platform\\Rough_Sets_Segmentation.sln";

            msbuilder_alls.build_alls(slnpath);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            return;

            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            string path = node.Tag as string;
            if (path == null) return;

            CreateFolder(path);
        }

        private void UI_TREE_MENU_RUN_PROJECT_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;

            string sln = node.Tag as string;
            string b = AppDomain.CurrentDomain.BaseDirectory;

            ProcessStartInfo start = new ProcessStartInfo();
            //start.arguments = new string[2];
            start.FileName = sln;
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = false;
            using (Process proc = Process.Start(start))
            {
                //proc.WaitForExit();
                //int exitCode = proc.ExitCode;
            }

            //string c = b + "\\Plugins\\Solution\\TEWI-Platform\\";

            //string s = Path.GetFullPath(c + "Rough_Sets_Segmentation.sln");

            ////slnpath = s;

            ////"\\Plugins\\Solution\\TEWI-Platform\\Rough_Sets_Segmentation.sln";

            //msbuilder_alls.build_alls(slnpath);

            //Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            //return;

            //TreeNode node = _mainTreeView.SelectedNode;
            //if (node == null) return;

            //string path = node.Tag as string;
            //if (path == null) return;

            //CreateFolder(path);
        }

        private void UI_TREE_MENU_CLONE_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            CloneNode(node);
        }

        private void UI_TREE_MENU_DELETE_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;
            if (node.Parent == null) return;

            DeleteNode(node);
        }

        private void UI_TREE_MENU_CREATE_FOLDER_Click(object sender, EventArgs e)
        {
            TreeNode node = _mainTreeView.SelectedNode;
            if (node == null) return;

            string path = node.Tag as string;
            if (path == null) return;

            CreateFolder(path);
        }

        private void UI_TREE_MENU_SET_AS_CURRENT_DIR_Click(object sender, EventArgs e)
        {
            string path = null;

            TreeNode node = _mainTreeView.SelectedNode;

            if (node == null)
                path = _rootFolder;
            else if (NodeIsFolder(node))
                path = node.Tag as string;

            if (path == null) return;

            Directory.SetCurrentDirectory(path);
            _applicationManager.NotifyFileSystemChange();
        }

        #endregion Tree View Menu Events

        #region File System Operations

        /// <summary>
        /// Notify the explorer window that the root folder has changed; the
        /// view will be updated according to the settings in effect.
        /// </summary>
        public void RootChangeUpdate()
        {
            if (_currentDirectoryFollowsRoot &&
                Directory.Exists(_rootFolder))
            {
                Directory.SetCurrentDirectory(_rootFolder);
                _applicationManager.NotifyFileSystemChange();
            }
        }

        public void SelectDirs(string path)
        {
        }

        private void MoveToSelectedFolder(string path)
        {
            /*
             * Test the new folder can be opened.
             */

            string savePath = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(path);
                Directory.SetCurrentDirectory(savePath);

                _rootFolder = path;

                _settingsManager.AddFolderToVisitedList(path);

                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.MoveRootErrorMessage,
                        ex.Message),
                    Resources.MoveRootErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MoveToParent()
        {
            DirectoryInfo newRoot = Directory.GetParent(_rootFolder);

            if (newRoot != null)
            {
                _rootFolder = newRoot.FullName;
                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
        }

        private void OpenNode(TreeNode node)
        {
            string path = node.Tag as string;
            if (path == null) return;

            if (NodeIsFolder(node))
            {
                _rootFolder = path;
                _applicationManager.NotifyFileSystemChange();
                _expandedPaths.Clear();

                RootChangeUpdate();
            }
            else
            {
                //mainForm.LoadDocumentIntoWindow(path, true);
            }
        }

        private void RenameNode(TreeNode node)
        {
            //RenameForm rf = new RenameForm();
            //rf.NewName = node.Name;

            //if (rf.ShowDialog() == DialogResult.OK)
            //{
            //    String newName = rf.NewName.Trim();
            //    if (newName == String.Empty) return;
            //    if (newName == node.Name) return;

            //    string oldPath = node.Tag as string;
            //    if (oldPath == null) return;

            //    string newPath = String.Empty;

            //    try
            //    {
            //        if (NodeIsFolder(node))
            //        {
            //            /*
            //             * We cant't rename a folder if it's the current
            //             * directory or a child of the current directory so we
            //             * need to move the current directory to the root
            //             * folder so that it's out of the way of the rename
            //             * operation. We can restore it afterwards if it still
            //             * exists (i.e. was not originally within the path
            //             * of the renamed folder).
            //             */

            //            string currentDir = Directory.GetCurrentDirectory();
            //            Directory.SetCurrentDirectory(_rootFolder);

            //            newPath = FileTools.ChangeDirectoryName(
            //                oldPath, newName);

            //            /*
            //             * Update the paths of any open editors.
            //             */

            //            //List<Document> documents = new List<Document>();

            //            string oldPathLC = oldPath.ToLower();

            //            //foreach (Document d in mainForm.ClientWindow.Documents)
            //            //{
            //            //    if (d.FilePath != null &&
            //            //        d.FilePath.ToLower().StartsWith(oldPathLC,
            //            //            StringComparison.CurrentCultureIgnoreCase))
            //            //    {
            //            //        d.FilePath = newPath + d.FilePath.Substring(oldPath.Length);
            //            //    }
            //            //}

            //            /*
            //             * If the old node was expanded make the new one
            //             * expanded. If any child folders were expanded
            //             * they will remain in the list but will no longer
            //             * be valid paths.
            //             */

            //            if (_expandedPaths.Contains(oldPath))
            //            {
            //                _expandedPaths.Remove(oldPath);
            //                _expandedPaths.Add(newPath);
            //            }

            //            if (Directory.Exists(currentDir))
            //                Directory.SetCurrentDirectory(currentDir);
            //        }
            //        else
            //        {
            //            newPath = FileTools.ChangeFileName(
            //                oldPath, newName);

            //            /*
            //             * Update the filename in any open editors.
            //             */

            //            //List<Document> documents = new List<Document>();

            //            //foreach (Document d in mainForm.ClientWindow.Documents)
            //            //{
            //            //    if (d.FilePath != null &&
            //            //        FileTools.MatchPaths(d.FilePath, oldPath))
            //            //    {
            //            //        d.FileName = newName;
            //            //        d.FilePath = newPath;
            //            //    }
            //            //}
            //        }

            //        _applicationManager.NotifyFileSystemChange();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(String.Format("{0}\r\n{1}",
            //                Resources.RenameErrorMessage,
            //                ex.Message),
            //            Resources.RenameErrorTitle,
            //            MessageBoxButtons.OK,
            //            MessageBoxIcon.Error);
            //    }
            //}
        }

        private void CloneNode(TreeNode node)
        {
            try
            {
                string source = node.Tag as string;
                if (source == null) return;

                int i = 1;
                string folder = Path.GetDirectoryName(source);
                string filebase = Path.GetFileNameWithoutExtension(source);
                string extension = Path.GetExtension(source);

                string destination = source;

                while (File.Exists(destination))
                {
                    string filename =
                        String.Format("{0}_{1}{2}", filebase, i++, extension);

                    destination = Path.Combine(folder, filename);
                }

                File.Copy(source, destination);
                _applicationManager.NotifyFileSystemChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.CloneErrorMessage,
                        ex.Message),
                    Resources.CloneErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DeleteNode(TreeNode node)
        {
            try
            {
                string path = node.Tag as string;
                if (path == null) return;

                if (NodeIsFolder(node))
                {
                    /*
                     * Can't delete folder if current directory or it's
                     * child. Need to move the current dir to the root
                     * folder and restore it after the delete where possible.
                     * As with the folder rename the current directory is
                     * moved regardless of where it is - this is easier than
                     * working out if the folder to be deleted is the current
                     * directory or one of it's children. If it's not it will
                     * be restored after the delete.
                     */

                    string currentDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(_rootFolder);

                    FileTools.DeleteWithUndo(path);

                    if (_expandedPaths.Contains(path))
                        _expandedPaths.Remove(path);

                    if (Directory.Exists(currentDir))
                        Directory.SetCurrentDirectory(currentDir);
                }
                else
                {
                    FileTools.DeleteWithUndo(path);

                    /*
                     * See if we have the file in any open editors.
                     */

                    //List<Document> documents = new List<Document>();

                    //foreach (Document d in mainForm.ClientWindow.Documents)
                    //    if (d.FilePath != null &&
                    //        FileTools.MatchPaths(d.FilePath, path))
                    //        documents.Add(d);

                    //if (documents.Count > 0)
                    //{
                    //    if (MessageBox.Show(
                    //        String.Format(
                    //            Resources.DeleteFileOpenMessage,
                    //            documents[0].FileName),
                    //        Resources.DeleteErrorTitle,
                    //        MessageBoxButtons.YesNo,
                    //        MessageBoxIcon.Information) == DialogResult.Yes)
                    //    {
                    //        foreach (Document d in documents)
                    //            d.Close();
                    //    }
                    //}
                }

                _applicationManager.NotifyFileSystemChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}\r\n{1}",
                        Resources.DeleteErrorMessage,
                        ex.Message),
                    Resources.DeleteErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CreateFolder(string parentFolder)
        {
            //string name = Resources.NewFolderName;
            //int i = 1;
            //while (Directory.Exists(name))
            //    name = String.Format("{0} ({1})",
            //        Resources.NewFolderName, i++);

            //RenameForm rf = new RenameForm();
            //rf.NewName = name;
            //rf.Title = Resources.CreateFolderTitle;

            //if (rf.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            //        String folderName = Path.Combine(
            //            parentFolder, rf.NewName.Trim());

            //        Directory.CreateDirectory(folderName);

            //        if (!_expandedPaths.Contains(parentFolder))
            //            _expandedPaths.Add(parentFolder);

            //        _applicationManager.NotifyFileSystemChange();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(String.Format("{0}\r\n{1}",
            //                Resources.NewFolderErrorMessage,
            //                ex.Message),
            //            Resources.NewFolderErrorTitle,
            //            MessageBoxButtons.OK,
            //            MessageBoxIcon.Error);
            //    }
            //}
        }

        #endregion File System Operations

        #region File Backup

        /// <summary>
        /// Backup the contents of the root folder to a zip archive.
        /// </summary>
        public void BackupExplorer()
        {
            String sourceDirectory = RootFolder;

            /*
             * Validate the source directory - user's need to use
             * common sense, i.e. not selecting huge folder structures
             * for backing up. We can trap the most obvious case
             * of trying to backup a whole disk.
             */

            if (String.IsNullOrEmpty(Path.GetFileName(sourceDirectory)))
            {
                MessageBox.Show(
                    Resources.BackupDialogNoDisk,
                    Resources.BackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            /*
             * Get the archive file name.
             */

            String zipFileName = GetZipFileName(sourceDirectory);
            if (zipFileName == null) return;

            try
            {
                //mainForm.Cursor = Cursors.WaitCursor;

                // FastZip zip = null;
                //    new FastZip();
                //zip.CreateEmptyDirectories = true;
                //zip.CreateZip(zipFileName, sourceDirectory, true, ".");

                _applicationManager.NotifyFileSystemChange();

                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.BackupDialogSuccess,
                        zipFileName),
                    Resources.BackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    if (File.Exists(zipFileName))
                        File.Delete(zipFileName);
                }
                catch
                {
                    // Suppress this - not much to do to correct and
                    // user will already get an error message from the
                    // outer exception.
                }

                MessageBox.Show(
                    String.Format("{0}:\r\n{1}",
                        Resources.BackupDialogError,
                        ex.Message),
                    Resources.BackupDialogTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // mainForm.Cursor = Cursors.Default;
            }
        }

        private String GetZipFileName(String sourceDirectory)
        {
            /*
             * Check user settings for destination folder.
             */

            String userHome = _settingsManager.BackupDirectory;

            if (!Directory.Exists(userHome))
                userHome = String.Empty;

            /*
             * If no path request one from the user.
             */

            if (userHome == String.Empty)
            {
                userHome = Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments);

                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = userHome;
                fbd.Description = Resources.BackupFolderMessage;

                if (fbd.ShowDialog() == DialogResult.Cancel)
                    return null;

                userHome = fbd.SelectedPath;
            }

            /*
             * Construct a name from the workspace name, date and time.
             */

            String baseName = Path.GetFileName(sourceDirectory);
            if (String.IsNullOrEmpty(baseName)) return null;

            DateTime now = DateTime.Now;

            String fileName = String.Format(
                "{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}.zip",
                baseName, now.Year, now.Month, now.Day,
                now.Hour, now.Minute, now.Second);

            return Path.Combine(userHome, fileName);
        }

        #endregion File Backup

        #region Helpers

        private bool NodeContainsNode(TreeNode node1, TreeNode node2)
        {
            if (node2.Parent == null) return false;
            if (node2.Parent.Equals(node1)) return true;

            return NodeContainsNode(node1, node2.Parent);
        }

        private bool NodeIsFolder(TreeNode node)
        {
            if (node == null) return false;
            if (node.ImageKey == Constants.CLOSED_FOLDER_IMAGE) return true;
            if (node.ImageKey == Constants.OPENED_FOLDER_IMAGE) return true;
            return false;
        }

        private void LaunchExplorer()
        {
            string explorerPath = Path.Combine(
                Environment.SystemDirectory, "..\\explorer.exe");

            string explorerArgs = _rootFolder;

            if (!File.Exists(explorerPath))
            {
                MessageBox.Show(String.Format(
                        Resources.LaunchExplorerErrorMessage,
                        "\r\n", explorerPath),
                    Resources.LaunchExplorerErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            FileTools.LaunchApplication(
                false, explorerPath, explorerArgs);
        }

        private void LaunchShell()
        {
            string shellPath = Path.Combine(
                Environment.SystemDirectory, "cmd.exe");

            string shellArgs = "/D/K PROMPT $N:$G&&CD";

            if (!File.Exists(shellPath))
            {
                MessageBox.Show(String.Format(
                        Resources.LaunchShellErrorMessage,
                        "\r\n", shellPath),
                    Resources.LaunchShellErrorTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            //FileTools.LaunchApplication(
            //    true, shellPath, shellArgs, _rootFolder);
        }

        #endregion Helpers

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        public void LoadParser()
        {





        }

        public static void toggle_sp(SplitContainer sp)
        {

            if (sp.Panel1Collapsed == true)
            {

                sp.Panel1Collapsed = false;
                sp.Panel2Collapsed = false;

            }
            else if (sp.Panel2Collapsed == false)
            {

                sp.Panel2Collapsed = true;
                sp.Panel1Collapsed = false;

            }
            else
            {

                sp.Panel1Collapsed = true;
                sp.Panel2Collapsed = false;


            }



        }

        public int act = -1;

        public ArrayList views { get; set; }

        public void next(ArrayList views, Control p)
        {

            if (act >= views.Count - 1)
                act = 0;
            else
                act++;

            Control c = views[act] as Control;

            int i = 0;

            foreach (Control cc in views)
            {
                if (cc == c)
                {

                    p.Controls.Add(cc);
                    act = i;
                }
                else
                {
                    p.Controls.Remove(cc);

                }

                i++;
            }

        }

        public void addview(Control c)
        {

            if (views == null)
            {
                act = 0;
                views = new ArrayList();
            }

            views.Add(c);
            act = views.Count - 1;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //next(views, splitContainer1.Panel2);
            OnCompletedChange(this, new EventArgs());
        }

        public void update_z_order(Control p)
        {
            p.SuspendLayout();

            ArrayList L = new ArrayList();

            //foreach (Control c in Controls)
            //{
            //    L.Add(c);
            //}
            foreach (Control c in p.Controls)
            {
                L.Add(c);
            }

            foreach (Control c in L)
            {
                if (c.GetType() == typeof(ToolStripPanel) || c.GetType() == typeof(ToolStrip))
                    p.Controls.Remove(c);
                if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true || c.GetType() == typeof(MenuStrip))
                    p.Controls.Remove(c);
            }
            foreach (Control c in L)
            {
                //if (c.GetType() == typeof(ToolStrip))
                //    p.Controls.Add(c);
                //if (c.GetType() == typeof(ToolStripPanel))
                //    p.Controls.Add(c);
                if (c.GetType() == typeof(MenuStrip))
                    p.Controls.Add(c);
                if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true)
                    p.Controls.Add(c);
            }

            foreach (Control c in L)
            {
                if (c.GetType() == typeof(ToolStrip))
                    p.Controls.Add(c);
                if (c.GetType() == typeof(ToolStripPanel))
                    p.Controls.Add(c);
                //if (c.GetType() == typeof(MenuStrip))
                //    p.Controls.Add(c);
                //if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true)
                //    p.Controls.Add(c);
            }

            p.ResumeLayout();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            sv.AddSolutionItem_Click(this, new EventArgs());
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //string file = "";

            //sv.add_create_file(file);

            //sv.remove_project_item(file);

            string s = "Script control - " + scriptControl1.Size.Width + " - " + scriptControl1.Size.Height + "\n";
            richTextBox1.AppendText(s);

            if (scriptControl1.dockContainer1 != null)
            {
                if (scriptControl1.dockContainer1.ActiveDocumentPane != null)
                {

                    s = "Script control DC - " + scriptControl1.dockContainer1.Size.Width + " - " + scriptControl1.dockContainer1.Size.Height + "\n";
                    richTextBox1.AppendText(s);

                    s = "Script control DC Pane- " + scriptControl1.dockContainer1.ActiveDocumentPane.Size.Width + " - " + scriptControl1.dockContainer1.ActiveDocumentPane.Height + "\n";
                    richTextBox1.AppendText(s);

                    AIMS.Libraries.Scripting.ScriptControl.Document d = scriptControl1.dockContainer1.ActiveDocument as AIMS.Libraries.Scripting.ScriptControl.Document;

                    d.ShowQuickClassBrowserPanel();

                    s = "Script control DC Pane- " + d.Size.Width + " - " + d.Height + "\n";
                    richTextBox1.AppendText(s);

                    d.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

                    scriptControl1.dockContainer1.ActiveDocumentPane.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

                    scriptControl1.dockContainer1.ActiveDocumentPane.master = scriptControl1;

                }
            }



            s = "SP - " + splitContainer1.Size.Width + " - " + splitContainer1.Size.Height + "\n";
            richTextBox1.AppendText(s);

            s = "Panel2 - " + splitContainer1.Panel2.Size.Width + " - " + splitContainer1.Panel2.Size.Height + "\n";
            richTextBox1.AppendText(s);






        }

        public void SetActiveDocument()
        {

            string s = "";

            if (scriptControl1.dockContainer1 != null)
            {
                if (scriptControl1.dockContainer1.ActiveDocumentPane != null)
                {

                    s = "Script control DC - " + scriptControl1.dockContainer1.Size.Width + " - " + scriptControl1.dockContainer1.Size.Height + "\n";
                    richTextBox1.AppendText(s);

                    s = "Script control DC Pane- " + scriptControl1.dockContainer1.ActiveDocumentPane.Size.Width + " - " + scriptControl1.dockContainer1.ActiveDocumentPane.Height + "\n";
                    richTextBox1.AppendText(s);

                    AIMS.Libraries.Scripting.ScriptControl.Document d = scriptControl1.dockContainer1.ActiveDocument as AIMS.Libraries.Scripting.ScriptControl.Document;

                    d.ShowQuickClassBrowserPanel();

                    s = "Script control DC Pane- " + d.Size.Width + " - " + d.Height + "\n";
                    richTextBox1.AppendText(s);

                    d.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

                    scriptControl1.dockContainer1.ActiveDocumentPane.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

                    scriptControl1.dockContainer1.ActiveDocumentPane.master = scriptControl1;

                }
            }



        }


        BackgroundWorker bw = null;

        public void startprogress(){


        BackgroundWorker bw = new BackgroundWorker();
        bw.WorkerReportsProgress = true;
        bw.WorkerSupportsCancellation = true;
        bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);


            //bw.RunWorkerAsync();
            //bw.CancelAsync();
        }

void bw_DoWork(object sender, DoWorkEventArgs e)
{
    //Task in background
    //

     //inside loop
    bw.ReportProgress(50); //50 is percentage
}

void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
{
    //make sure ProgressBar is not continuous, and has a max value
    progressBar1.Value = e.ProgressPercentage;
}

        

        Font font {get; set;}

        public void Loads2(TreeView tv, TreeView s)
        {
            //sv._SolutionTreeView.ImageList = tv.ImageList;

           font = new Font(s.Font, FontStyle.Bold);

           s.Nodes.Clear();

           s.BeginUpdate();

            s.ImageList = tv.ImageList;

            s.Tag = tv.Tag;

            s.EndUpdate();

            int N = tv.Nodes.Count;

            ArrayList L = new ArrayList();

            int i = 0;
            while (i < N)
            {
                L.Add(tv.Nodes[i]);

                i++;
            }

            i = 0;
            while (i < N)
            {
                TreeNode ns = L[i] as TreeNode;
                object[] obs = new object[3];
                obs[0] = s;
                obs[1] = tv;
                obs[2] = ns;
                this.BeginInvoke(new PopulateTreeNodes(populateTreeNode), obs);

                //ns.NodeFont = font;
                //tv.Nodes.Remove(ns);
                //s.Nodes.Add(ns);

                i++;
            }

           

        }

        public delegate void workerSelectDelegate(TreeNode node);

        public delegate void workerBuildDelegate(string recent);

        public delegate void workerFunctionDelegate(string recent);

        public delegate void workerTypeViewerDelegate(VSSolution vs);

        public delegate void PopulateTreeNodes(TreeView s, TreeView tv, TreeNode ns);

        void workerFunction(string recent)
        {


            TreeView tv = sv.load_recent_solution(recent);

            Loads2(tv, sv._SolutionTreeView);

            this.Text = Path.GetFileName(recent) + " - Application Studio";

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            sv._SolutionTreeView.BeginInvoke(new Action(() => { sv._SolutionTreeView.Nodes[0].Expand(); sv.LoadPlatforms(vs); sv.LoadProjects(vs); }));

            

            if(pg != null)
            pg.Invoke(new Action(() => { pg.Value = 0; }));


            if (scr != null)
                scr.BeginInvoke(new Action(() => { scr.LoadSolution(vs); }));

            started = false;


            return;
            
            //ImageList img = CreateView_Solution.CreateImageList();
            //cc.dc = CreateView_Solution.GetDC();
            //treeView2.ImageList = img;
            //img.TransparentColor = Color.Fuchsia;
            //workerTypeViewerDelegate w = workerTypeViewer;
            //IAsyncResult r = w.BeginInvoke(vs, null, null);
            

            //w.EndInvoke(r);

            //this.BeginInvoke(new Action(() => { sv.LoadPlatforms(vs); }));

            //CompletedChange(this, new EventArgs());

        }


        void workerTypeViewer(VSSolution vs)
        {

            //VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;
            //cc.dc = sv.dc;
            //cc.cc.ImageList = sv._SolutionTreeView.ImageList;

            

            TreeView tt = cc.LoadSolution(vs);

            //string s = "F:\\Application-Studio\\AdminConsoleProjects-alls\\LinkerProject\\DataViewerProjects\\CustomControls\\CustomTabControl.cs";

            this.BeginInvoke(new Action(() => { treeView2.Nodes.Clear(); treeView2.ImageList.TransparentColor = Color.Fuchsia; treeView2.ImageList = CreateView_Solution.CreateImageList(); ArrayList L = new ArrayList(); foreach (TreeNode node in tt.Nodes) L.Add(node); foreach (TreeNode node in L) { tt.Nodes.Remove(node); treeView2.Nodes.Add(node); }; }));

       

        }

        void populateTreeNode(TreeView s, TreeView tv, TreeNode ns)
        {
            s.BeginUpdate();
            //ns.NodeFont = font;
            tv.Nodes.Remove(ns);
            s.Nodes.Add(ns);
            s.EndUpdate();
        }



        private void reload_solution(string recent)
        {


            sv._SolutionTreeView.Nodes.Clear();


            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);


        }


         private void toolStripButton5_Click(object sender, EventArgs e)
        {

            if (treeView3.SelectedNode == null)
                return;

            scr.ClearHistory();

            ClearOutput();

            string recent = treeView3.SelectedNode.Text;

            sv._SolutionTreeView.Nodes.Clear();

            sv.save_recent_solution(recent);



            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);


        }

 

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if(scriptControl1.ActiveControl != null)
            scriptControl1.ActiveControl.Size = splitContainer1.Panel2.Size;


           // scriptControl1.ResizeControl(e, sizeof);

        }

        //WindowsFormsSynchronizationContext mUiContext = new WindowsFormsSynchronizationContext();

        // private Task<double> JobAsync(double value)
        //{
        //    return Task.Factory.StartNew(() =>
        //    {

        //        mUiContext.Post(UpdateGUI, new object());

              
               
        //        return 0.0;
        //    });
        //}


        // void UpdateGUI(object userData)
        // {
        //     //update your gui controls here

        //     string recent = treeView3.SelectedNode.Text;
        //     sv.load_recent_solution(recent);

        // }

        private void treeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {
            sv.slnpath = e.Node.Text;
            textBox3.Text = sv.slnpath;
            textBox3.Enabled = false;
        }

       // public SVNForm sfm { get; set; }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            //sfm = new SVNForm();
            //sfm.Show();

            TreeNode node = treeView3.SelectedNode;

            if (node == null)
                return;

            string text = node.Text;

            sv.hst.remitem(text);

            sv.hst.loaddefaults();


        }

        private void setAsMainProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sv.set_main_project();
        }

        SplitContainer sp { get; set; }

        SplitContainer sc { get; set; }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {

            

        }

        public void LoadOutput()
        {

            sc = splitContainer2;

            if (sc.Panel2Collapsed == true)
                sc.Panel2Collapsed = false;
            sc.Panel2Collapsed = true;

        }

        private void builds_alls()
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;
            if (vs == null)
                return;

            workerBuildDelegate b = build_alls;
            b.BeginInvoke(vs.solutionFileName, null, null);
        }

        public void build_alls(string recent)
        {
            sv.build_solution(recent);
        }

        public void build_project(string recent)
        {

            sv.build_project_solution(recent);
        }

        ProgressBar pg { get; set; }

        public void CreateProgressBar()
        {
            if (pg == null)
            {
                pg = new ProgressBar();
                pg.Dock = DockStyle.Bottom;
                forms.Controls.Add(pg);
                pg.Show();
            }

            

            pg.Visible = true;

            sv.pg = pg;

        }

        bool started = false;
        private void toolStripButton9_Click(object sender, EventArgs e)
        {

            if (started == true)
                return;

            string recent = sv.load_recent_solution();

            sv._SolutionTreeView.Nodes.Clear();

            sv.save_recent_solution(recent);

            //CreateProgressBar();



            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);

            

        }

        public void ResumeAtStartUp()
        {


            if (started == true)
                return;

            string recent = sv.load_recent_solution();

            sv._SolutionTreeView.Nodes.Clear();

            sv.save_recent_solution(recent);

            

            //CreateProgressBar();

            //sv._configList.SetDrawMode(false);

            //sv._platList.SetDrawMode(false);


            workerFunctionDelegate w = workerFunction;
            IAsyncResult r = w.BeginInvoke(recent, null, null);

            //w.EndInvoke(r);


            

        }

        public event EventHandler CompletedChange = null;

        public void OnCompletedChange(object sender, EventArgs e)
        {

            sv._configList.Enabled = false;
            sv._platList.Enabled = false;


            sv._configList.SetDrawMode(false);
            sv._platList.SetDrawMode(false);


            sv.LoadPlatforms(GetVSSolution());

            sv._configList.Enabled = true;
            sv._platList.Enabled = true;

           

            sv._configList.SetDrawMode(true);
            sv._platList.SetDrawMode(true);

            sv._mainToolStrip.Refresh();
            

        }

        AIMS.Libraries.Scripting.ScriptControl.DocumentForm df { get; set; }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (sv == null)
                return;
            if (sv.eo == null)
                return;

            df = sv.eo.OpenDocumentForm("test");

           // LoadListTab(df);

        }

         

        private void toolStripButton11_Click(object sender, EventArgs e)
        {

            if (_mainTreeView.SelectedNode == null)
                return;

            TreeNode node = _mainTreeView.SelectedNode;

            if (node.Tag == null)
                return;

            string recent = node.Tag as string;


            if (scr != null)
                scr.ClearHistory();

            ClearOutput();

            //string recent = treeView3.SelectedNode.Text;

            sv._SolutionTreeView.Nodes.Clear();

            sv.save_recent_solution(recent);


            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);


        

            //sv._SolutionTreeView.Nodes.Clear();

            //sv.save_recent_solution(recent);

            //workerFunctionDelegate w = workerFunction;
            //IAsyncResult r =  w.BeginInvoke(recent, null, null);

            //w.EndInvoke(r);

            //sv._configList.Enabled = true;
            //sv._platList.Enabled = true;


            //sv._configList.SetDrawMode(true);
            //sv._platList.SetDrawMode(true);


            //sv._mainToolStrip.Refresh();

        }

        private void projectPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

 

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }

            

            TreeNode MainProjectNode = eo.cvs.getactivenode();


            //MainProjectItem = prs;

            MessageBox.Show("Main project properties " + prs.ps.Name);

            //unboldnodes(MainProjectNode);

            //MainProjectNode.NodeFont = font;

            if (sv == null)
                return;
            if (sv.eo == null)
                return;

            df = sv.eo.OpenDocumentForm(prs.ps.Name);




            //LoadListTab(df);

            

        
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            //MainProjectNode = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            string r = sv.GetProjectExec(prs.ps.FileName);

            cmds.RunExternalExe(r, "");


        }


        //public void startapache()
        //{

        //    string apache = AppDomain.CurrentDomain.BaseDirectory + "\\" + "xampp\\apache_start.bat";


        //    cmds.RunExternalExe(apache, "");

        //}

        //RefManagerForm rmf { get; set; }

        GACForm gf { get; set; }


        private void addReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }

            

            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;


            VSSolution vs = prs.vs;

            VSProject pp = prs.ps;

            gf = new GACForm();
            
            gf.GetAssemblies();
            gf.Text = "Reference manager - " + prs.ps.Name;

            ArrayList L  = pp.GetItems("Reference", false);

            //foreach(VSProjectItem p in prs.ps.Items){

            //    if (p.ItemType == "Reference")
            //    {

            //        L.Add(p.Name);

            //    }

            //}


            gf.refs = L;

            gf.CheckReferences(L);

         

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (VSProject p in vs.Projects)
                dict.Add(p.Name, p.FileName);

            gf.LoadProjects(dict);



            if (pp == null)
                return;

            ArrayList P = pp.GetItems("ProjectReference", true);

            gf.projs = P;

            gf.CheckProjects(P);

            gf.sel = GACForm.Sel.projects;

            DialogResult r = gf.ShowDialog();

            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            P = gf.projs;

            L = gf.refs;

            pp.SetProjectReferences(P);

            pp.SetReferences(L);

            reload_solution(vs.solutionFileName);

        }

        NewProjectForm npf { get; set; }
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            npf = new NewProjectForm();
            npf.SetProjectFolder(sln);
            DialogResult r = npf.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;


            string pf = npf.GetProjectFolder();

            string pr = npf.GetProjectFile();

            string prs = Path.GetFileName(pr);

            string folds = Path.GetDirectoryName(pr);

            string[] dd = folds.Split("\\".ToCharArray());

            string fds = dd[dd.Length - 1];

            //file = file + "s";


            string content = File.ReadAllText(file);

            int p = content.IndexOf("Global");

            string dir = fds;// "LinkerProjectProject";

            string proj = prs;// "LinkerProjectProject.csproj";

            string pb = dir + "\\" + proj;

            Guid guid = Guid.NewGuid();

            string pp = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + dir + "\",\"" + pb + "\"," + "\"{" + guid.ToString() + "}\"\nEndProject\n\n\t";

            content = content.Insert(p, pp);

            File.WriteAllText(file, content);

            DirectoryInfo project = new DirectoryInfo(folds);

            DirectoryInfo folder = new DirectoryInfo(sln + "\\" + fds);

            CopyAll(project, folder);

            reload_solution(file);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists; if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        private void viewSolutionFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            eo.LoadFile(vs.solutionFileName, null, null,null);
        }

        private void viewProjectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }

            VSProject project = prs.ps;

            string file = project.FileName;

            eo.LoadFile(file, null, null,null);


        }

        OpenFileDialog ofd { get; set; }
        private void existingProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            //npf = new NewProjectForm();
            //npf.SetProjectFolder(sln);
            //DialogResult r = npf.ShowDialog();
            //if (r != System.Windows.Forms.DialogResult.OK)
            //    return;

            ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;


            string filefolder = ofd.FileName;

            string pf = Path.GetDirectoryName(filefolder);

            string pr = filefolder;// npf.GetProjectFile();

            string prs = Path.GetFileName(pr);

            string folds = Path.GetDirectoryName(pr);

            string[] dd = folds.Split("\\".ToCharArray());

            string fds = dd[dd.Length - 1];

            //file = file + "s";


            string content = File.ReadAllText(file);

            int p = content.IndexOf("Global");

            string dir = fds;// "LinkerProjectProject";

            string proj = prs;// "LinkerProjectProject.csproj";

            string pb = dir + "\\" + proj;

            Guid guid = Guid.NewGuid();

            string pp = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + dir + "\",\"" + pb + "\"," + "\"{" + guid.ToString() + "}\"\nEndProject\n\n\t";

            content = content.Insert(p, pp);

            File.WriteAllText(file, content);

            //DirectoryInfo project = new DirectoryInfo(folds);

            //DirectoryInfo folder = new DirectoryInfo(sln + "\\" + fds);

            //CopyAll(project, folder);

            reload_solution(file);

        }

 
        //private void treeView2_MouseUp(object sender, MouseEventArgs e)
        //{
            
            

        //    if (context1 == null)
        //        return;

        //    // Show menu only if the right mouse button is clicked.
        //    if (e.Button == MouseButtons.Right)
        //    {

        //        // Point where the mouse is clicked.
             
        //        {

        //            // Select the node the user has clicked.
        //            // The node appears selected until the menu is displayed on the screen.
        //            //oldselectednode = treeView.SelectedNode;
        //            //treeView.SelectedNode = node;


                    
        //            context1.Show();

        //            context1.Refresh();

        //            // Highlight the selected node.
        //            //treeView.SelectedNode = oldselectednode;
        //            //oldselectednode = null;

        //            //treeView.Refresh();
        //        }
        //    }
        //}

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView treeView = treeView1;

            if (context1 == null)
                return;

            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = treeView.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    //oldselectednode = treeView.SelectedNode;
                    
                    
                    if(node.Parent == null)
                        context2.Show(treeView, p);
                    else if(node.Parent.Text == "Reference")
                        context3.Show(treeView, p);
                    else if(node.ImageKey == "Folder_6222")
                        context4.Show(treeView, p);
                    else context1.Show(treeView, p);



                    // Highlight the selected node.
                    //treeView.SelectedNode = oldselectednode;
                    //oldselectednode = null;

                    treeView.Refresh();
                }
            }
        }

        private void treeView2_MouseUp(object sender, MouseEventArgs e)
        {
            TreeView treeView = treeView2;

            if (context1 == null)
                return;

            // Show menu only if the right mouse button is clicked.
            if (e.Button == MouseButtons.Right)
            {

                // Point where the mouse is clicked.
                Point p = new Point(e.X, e.Y);

                // Get the node that the user has clicked.
                TreeNode node = treeView.GetNodeAt(p);
                if (node != null)
                {

                    // Select the node the user has clicked.
                    // The node appears selected until the menu is displayed on the screen.
                    //oldselectednode = treeView.SelectedNode;


                    //if (node.Parent == null)
                    //    context2.Show(treeView, p);
                    //else if (node.Parent.Text == "Reference")
                    //    context3.Show(treeView, p);
                    //else if (node.ImageKey == "Folder_6222")
                    //    context4.Show(treeView, p);
                    //else context1.Show(treeView, p);



                    context5.Show(treeView2, p);



                    // Highlight the selected node.
                    //treeView.SelectedNode = oldselectednode;
                    //oldselectednode = null;

                    treeView.Refresh();
                }
            }
        }


        NewProjectItem npn { get; set; }
        private void addNewItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;


            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            DialogResult r = npn.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

           
            string pf = npn.GetProjectFolder();

            ArrayList L = npn.GetProjectFiles();

            string folds = Path.GetDirectoryName(pp.FileName);// Path.GetDirectoryName(pr);

            pp.SetCompileItems(L);



            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            CopyAll(project, folder);

            reload_solution(file);

        }

        // project remove from solution
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

       
            vs.RemoveProject(pp.FileName);

            reload_solution(file);

        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }

            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            if (pp == null)
                return;

            DataForm df = new DataForm();
            DialogResult r = df.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            string folder = df.GetFolderName();

            pp.CreateFolder(folder, prs.psi);

            reload_solution(file);

        }

        private void controlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;


            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            DialogResult r = npn.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;


            string pf = npn.GetProjectFolder();

            ArrayList L = npn.GetProjectFiles();

            string folds = Path.GetDirectoryName(pp.FileName);// Path.GetDirectoryName(pr);

            pp.SetCompileItems(L);



            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            CopyAll(project, folder);

            reload_solution(file);

        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {


            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;


            pp.RemoveReference(prs.psi);

            reload_solution(file);


        }

        private void viewInFileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pr = prs.ps;

            if (eo.tw == null)
                return;

            VSProjectItem pp = prs.psi;

            string folder = Path.GetDirectoryName(pp.fileName);

            eo.tw.MoveToSelectedFolder(folder);

        }

        DependencyForms dfs { get; set; }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {


            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;

            }



            TreeNode node = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pr = prs.ps;


            dfs = new DependencyForms();
            dfs.LoadProjects(vs);
            dfs.ShowDialog();

            reload_solution(vs.solutionFileName);

        }

        OptionsForms opsf { get; set; }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            opsf = new OptionsForms();
            opsf.scr = scr;
            opsf.Show();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            //VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;
            //cc.dc = sv.dc;
            //cc.cc.ImageList = CreateView_Solution.CreateImageList();
            //cc.cc.Nodes.Clear();
          
            //workerTypeViewerDelegate w = workerTypeViewer;
            //w.BeginInvoke( vs, null, null);

            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;
            ImageList img = CreateView_Solution.CreateImageList();
            cc.dc = VSProject.dc;// CreateView_Solution.GetDC();
            treeView2.ImageList = img;
            img.TransparentColor = Color.Fuchsia;
            treeView2.ImageList.TransparentColor = Color.Fuchsia;
            //treeView2.ImageList.ColorDepth = ColorDepth.Depth24Bit;

            treeView2.Nodes.Clear();
            workerTypeViewerDelegate w = workerTypeViewer;
            w.BeginInvoke(vs, null, null);

        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {

            sv.CreateNodeDict(sv._SolutionTreeView);

            //VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            //string sln = vs.SolutionPath;

            //string file = vs.solutionFileName;

           

            //WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            //if (prs == null)
            //{
            //    MessageBox.Show("No project has been selected..");
            //    return;

            //}

            //VSProject p = prs.ps;

            //string name = p.GetDefNamespace();


            //MainTest mt = msbuilder_alls.GetProjectDefinitions(prs.ps.FileName);

            //Dictionary<object, object> dict = mt.wm.sharedDict;

            //mt.wm.AddShared(name, dict);
        }

        private void treeView2_DoubleClick(object sender, EventArgs e)
        {

        }

        private void treeView2_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            string text = node.Text;


            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            VSProject p = GetVSProject(treeView2);

            if (p == null)
                return;

 
            string name = p.GetDefNamespace();

            MainTest mf = msbuilder_alls.mf;

            Dictionary<object, object> dict = null;


            if (mf == null)
            {

                mf = msbuilder_alls.GetProjectDefinitions(p.FileName);

                dict = mf.wm.sharedDict;

                mf.wm.AddShared(name, dict);

            }

            dict = mf.wm.FindShared(name);

            if (dict == null)
            {
                mf = msbuilder_alls.GetProjectDefinitions(p.FileName);

                dict = mf.wm.sharedDict;

                mf.wm.AddShared(name, dict);
            }

            if(dict.ContainsKey(text)){


                Dictionary<string, object> d = dict[text] as Dictionary<string, object>;

                string files = d["FullFileName"] as string;

                eo.LoadFile(files, null, null,null);

                if (sv.dic != null)
                {

                    if (sv.dic.ContainsKey(files) == false)
                        return;

                    TreeNode nodes = sv.dic[files];

                    nodes.EnsureVisible();

                    sv._SolutionTreeView.SelectedNode = nodes;

                }
            }

             

        }

        public void OpenFileFromProject(string files)
        {

            sv.CreateNodeDict(sv._SolutionTreeView);


            if (sv.dic != null)
            {

                if (sv.dic.ContainsKey(files) == false)
                    return;

                TreeNode nodes = sv.dic[files];

                nodes.EnsureVisible();

                sv._SolutionTreeView.SelectedNode = nodes;

                CreateView_Solution.ProjectItemInfo pr0 = nodes.Tag as CreateView_Solution.ProjectItemInfo;


                ImageList img = CreateView_Solution.CreateImageList();
                Dictionary<string, string> dc = CreateView_Solution.GetDC();

                eo.LoadFile(files, dc, img, pr0.ps);

            }




        }


        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;


            ArrayList L = node.Tag as ArrayList;


            if(node.Parent != null)
                if (node.Parent.Text == "ProjectReferences")
                {

                    VSProject p = node.Parent.Parent.Tag as VSProject;

                    if (p == null)
                        return;

                    Dictionary<string, string> d = GACForm.dicts;

                    string asm = node.Text;

                    string file = Path.GetDirectoryName(p.FileName);

                    if (d.ContainsKey(asm))
                    {

                        file = d[asm];
                     
                    }
                    else
                    {
                        string hint = p.GetReferenceHint(asm);

                        if (hint == "")
                            return;

                        file = file + "\\" + hint;

                    }

                    cc.AddTypes(node, file);

                }

            if (L == null) {


                //AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> T = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

                //Type TT = T.PropertyObject.GetType();

                //Type B = TT.BaseType;

                return;
            
            }
                

            if (treeView4.ImageList == null)
            {
                ImageList img = CreateView_Solution.CreateImageList();
                img.TransparentColor = Color.Fuchsia;
                treeView4.ImageList =img;
                //treeView4.ImageList.TransparentColor = Color.Fuchsia;

            }
            treeView4.Nodes.Clear();

            foreach (TreeNode nodes in L)
                treeView4.Nodes.Add(nodes);

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {

            TreeNode node = treeView1.SelectedNode;

            if (node == null)
                return;

            CreateView_Solution.ProjectItemInfo prs = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (prs == null)
                return;

            VSProject p = prs.ps;

            if (p == null)
                return;

            workerBuildDelegate b = build_project;
            b.BeginInvoke(p.FileName, null, null);

        }


        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            workerSelectDelegate s = AfterSelects;
            s.BeginInvoke(node, null,null);
        }

        private void AfterSelects(TreeNode node)
        {

            if (Control.MouseButtons == MouseButtons.Right)
                return;
            
            string s = node.Text;

            //TreeNode node = e.Node;

            string file = "";

            if (node.Tag == null)
                return;

            CreateView_Solution.ProjectItemInfo pr0 = null;

            if (node.Tag.GetType() != typeof(CreateView_Solution.ProjectItemInfo))
                return;

            pr0 = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (pr0.psi == null)
                if (pr0.mapper != null)
                {
                    file = sv.mapper.classname;

                    sv.projectmapper(file, node, false);

                    return;
                }
  
            if (pr0.psi != null)
       
            {

                VSProjectItem pi = pr0.psi;

                file = pi.fileName;

                if (node.Nodes.Count >= 1)
                {
                    if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
                    {
                        
                        return;
                    }
                    else
                    {
                        sv.projectmapper(file, node, false);
                    }
                }
            }
            else //if (e.Node.Tag.GetType() == typeof(ProjectItemInfo))
            {
                CreateView_Solution.ProjectItemInfo pr = pr0;// (ProjectItemInfo)pr0.pi;// e.Node.Tag;

                return;

 
            }




            this.BeginInvoke(new Action(() => { sv.projectmapper(file, node, false); }));

            if (File.Exists(file) == true)
            {

                VSProject pp = pr0.ps;// GetVSProject();

                

                  if (CodeEditorControl.AutoListImages == null)
                  {

                        ImageList imgs = CreateView_Solution.CreateImageList();
            VSProject.dc = CreateView_Solution.GetDC();

            CodeEditorControl.AutoListImages = imgs;


                  }

                this.BeginInvoke(new Action(() => { eo.LoadFile(file, pp); }));

                

                //eo.LoadFile(file, pr0.ps.Name, new Bitmap(resource_vsc.CSharpFile_SolutionExplorerNode_24), pr0.ps);

                
            }

     

        }



        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            VSProject p = GetVSProject();
            if (p == null)
                return;
            MessageBox.Show("Project " + p.Name + " will be cleaned.");

            p.CleanProject();
        }


        public VSSolution GetVSSolution()
        {
            return sv._SolutionTreeView.Tag as VSSolution;
        }

        public VSProject GetVSProject()
        {
            TreeNode node = sv._SolutionTreeView.SelectedNode;
            if (node == null)
                return null;
            CreateView_Solution.ProjectItemInfo s = node.Tag as CreateView_Solution.ProjectItemInfo;
            if (s == null)
                return null; ;
            return s.ps;
                
        }

        public VSProject GetVSProject(TreeView tv)
        {
            TreeNode node = tv.SelectedNode;
            if (node == null)
                return null;

            if(node.Parent != null)

            while (node.Parent != null)
                node = node.Parent;

            VSProject p = node.Tag as VSProject;

            return p;

        }

        private void rebuildAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;
            if (vs == null)
                return;

            workerBuildDelegate b = build_alls;
            b.BeginInvoke(vs.solutionFileName, null, null);
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSProject p = GetVSProject();
            if (p == null)
                return;
            p.RunProject();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {

        }

        private void scriptControl1_Load(object sender, EventArgs e)
        {

        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.ShowFind();
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.ShowReplace();

            
        }

        private void commentSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.Comment();
        }

        private void uncommentSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.UnComment();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
        if (scr == null)
                return;

            scr.SelectAll();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;
            scr.Undo();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;
            scr.Redo();
        }

        private void decompileTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DecompileType();
        }

        public void DecompileType()
        {

            TreeNode node = treeView2.SelectedNode;

            if (node == null)
                return;

           string text = node.Text;


            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            VSProject p = GetVSProject(treeView2);

            if (p == null)
                return;

            AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> bg = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

            if (bg == null)
                return;

            MessageBox.Show("Type node selected");

            //DecompilerProject.DecompilerForm.LoadLibrary4Decompile("s");

            string c = DecompilerForm.LoadLibrary4Method( p.GetProjectExec(p.FileName), bg.GetTypeFullName());

            eo.LoadFile(c, bg.GetTypeFullName());

        }

        private void typeInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView2.SelectedNode;

            if (node == null)
                return;

            string text = node.Text;


            VSSolution vs = sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;


            VSProject p = GetVSProject(treeView2);

            if (p == null)
                return;

            AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> bg = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

            if (bg == null)
                return;

            MessageBox.Show("Type node selected");

        


            string c = bg.GetTypeSignature();


            eo.LoadFile(c, p);

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
           
        }

        private void ExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (scr == null)
                return;
            scr.Invoke(new Action(() => { scr.SaveSolution(); }));
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            if (scr == null)
                return;
            scr.CloseAll();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            scr.CloseAll();
            sv._SolutionTreeView.Nodes.Clear();
            treeView2.Nodes.Clear();
            ClearOutput();
        }

        private void outputWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
    public class FileCreator
    {

        public string[] files = new string[] { "code cs file"};

    }

 

}