using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.Scripting.ScriptControl;
using NUnit;
using DockProject;
using GACProject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VSProvider;
using WeifenLuo.WinFormsUI.Docking;
using WinExplorer.Environment;
using WinExplorer.UI;
using NUnit.Engine;
using NUnit.Gui.Model;
using NUnit.Gui.Presenters;
using NUnit.Gui;
using utils;

namespace WinExplorer
{
    /// <summary>
    /// The Application Studio main window form.
    /// </summary>
    public partial class ExplorerForms : Form
    {
        //this to change.......
        #region Nested Classes

        private class DummyNode : TreeNode { }

        private class FileNamePattern
        {
            public Regex Regex;
            public bool MatchResult = true;
        }

        #endregion Nested Classes

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
                                                string pszSubIdList);

        /// <summary>
        /// Create the explorer window form.
        /// </summary>
        /// <param name="formKey">The GUID key used to identify the form internally.</param>
        public ExplorerForms(string formKey)
        {
            this.CreateHandle();

            #region Form Setup and Events

            _components = new System.ComponentModel.Container();

            InitializeComponent();

            context1 = contextMenuStrip1;
            context2 = contextMenuStrip2;
            context3 = contextMenuStrip3;
            context4 = contextMenuStrip7;
            context5 = contextMenuStrip5;
            context6 = contextMenuStrip6;

            _components = new System.ComponentModel.Container();

            #endregion Form Setup and Events

            #region Document Type Images
            #endregion Document Type Images

            Application.EnableVisualStyles();

            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            CodeEditorControl.settings = new Settings();

            this.DoubleBuffered = true;

            this.BackColor = SystemColors.Control;

            splashForm = new SplashForm();
            splashForm.Show();

            SuspendLayout();

            menuStrip1.Renderer = new MyRenderer();

            toolStrip4.Renderer = new MyRenderer();

            dock = new DockPanel();

            dock.Dock = DockStyle.Fill;

            dock.DocumentStyle = DocumentStyle.DockingWindow;

            dock.SupportDeeplyNestedContent = false;

            this.Controls.Add(dock);

            ScriptControl.LoadSettings();

            scr = new ScriptControl();

            string Theme = scr.GetTheme();

            if (Theme == "VS2012Light")
                dock.Theme = new VS2012LightTheme();
            else
                dock.Theme = new VS2013BlueTheme();

            scr.Dock = DockStyle.Fill;

            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            ImageList imgs = CreateView_Solution.CreateImageList();
            VSProject.dc = CreateView_Solution.GetDC();

            CodeEditorControl.AutoListImages = imgs;


            LoadDocumentWindow(false);

            LoadSE("Explorer", false);
            solutionExplorerToolStripMenuItem.Checked = true;
            LoadCD("Class View", false);
            classViewToolStripMenuItem.Checked = true;
            LoadFD("Folders", false);
            fileExplorerToolStripMenuItem.Checked = true;
            LoadRE("Recent", false);
            recentProjectsToolStripMenuItem.Checked = true;
            LoadPG("Properties", false);
            propertyGridToolStripMenuItem.Checked = false;
            LoadERW("Error List", false);
            errorListToolStripMenuItem.Checked = false;
            LoadORW("Output", false);
            outputWindowToolStripMenuItem.Checked = false;
            LoadRW("Find Replace", false);
            ResumeLayout();
            LoadRF("Find Results 1", false);
            ResumeLayout();
            LoadRF2("Find Results 2", false);
            ResumeLayout();
            LoadAW("Code Analysis", false);
            ResumeLayout();
            //LoadWT("Document Outline", false);
            //ResumeLayout();
            LoadNU("NUnit", false);
            ResumeLayout();

            update_z_order(this);

            loadcurrentproject();

            loadtoolmenu();

            loadcontmenu();

            CreateMenuBasedOnArray();

            _sv.ef = efs;

            _sv._SolutionTreeView.FullRowSelect = true;

            _sv._SolutionTreeView.ShowLines = false;

            treeView1.Visible = true;

            if (!treeView1.IsHandleCreated)
            {
                treeView1.CreateControl();
                MethodInfo ch = treeView1.GetType().GetMethod("CreateHandle", BindingFlags.NonPublic | BindingFlags.Instance);
                ch.Invoke(treeView1, new object[0]);
            }

            CreateProgressBar();

            treeView1.MouseUp += new MouseEventHandler(treeView1_MouseUp);

            treeView2.MouseUp += new MouseEventHandler(treeView2_MouseUp);

            _sv._configList.SelectedIndexChanged += new EventHandler(comboBox1_DrawItem);

            _sv._projectList.SelectedIndexChanged += new EventHandler(comboBox1_DrawItem);

            toolStrip5.GripMargin = Padding.Empty;

            toolStrip5.GripStyle = ToolStripGripStyle.Hidden;

            treeView2.ShowLines = false;

            treeView2.FullRowSelect = true;

            treeView2.AfterSelect += new TreeViewEventHandler(treeView2_AfterSelect);

            _sv._SolutionTreeView.AfterSelect += new TreeViewEventHandler(AfterSelect);

            _sv._SolutionTreeView.MouseDown += new MouseEventHandler(treeView1_MouseDown);


            _sv._SolutionTreeView.MouseDoubleClick += _SolutionTreeView_MouseDoubleClick;

            _sv._SolutionTreeView.KeyDown += _SolutionTreeView_KeyPress;

            _selectedNodes = new List<TreeNode>();

            _sv.scr = scr;

            //ImageList imgs = CreateView_Solution.CreateImageList();
            //VSProject.dc = CreateView_Solution.GetDC();

            //CodeEditorControl.AutoListImages = imgs;

            if (CodeEditorControl.proxy.IsBbound() == false)
                CodeEditorControl.proxy.OpenFile += exitToolStripMenuItem_Click;

            scr.DisplayTopStrip(false);

            scr.SetToolStripLabel(toolStripStatusLabel1, toolStripStatusLabel3);

            scr.DisplayStatusStrip(false);

            scr.HistoryChange += new EventHandler<EventArgs>(HistoryChanged);

            scr.ProjectProperties += Scr_ProjectProperties;

            scr.GetTheme();

            scr.dockContainer1.Dock = DockStyle.Fill;

            scr.Dock = DockStyle.Fill;

            CodeEditorControl.ParserDataChanged += CodeEditorControl_ParserDataChanged;

            update_z_order(this);

            splitContainer1.Visible = false;

            GACForm.GetAssembliesList("");

            statusStrip1.BackColor = Color.FromKnownColor(KnownColor.Highlight);
        }

        private void _SolutionTreeView_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                bool control = (ModifierKeys == Keys.Shift);

                if (control == true)
                {
                    TreeNode node = treeView1.SelectedNode;

                    if (_selectedNodes.Contains(node) == true)
                    {
                        _selectedNodes.Remove(node);
                        drawSelection(node, false);
                    }
                    else
                    {
                        _selectedNodes.Add(node);
                        drawSelection(node, true);
                    }

                    treeView1.Refresh();
                }
            }

            e.Handled = false;
        }

        //TreeNode _selectedNode;

        public void AfterSelectActivate(bool activate)
        {
            if (activate == true)
                treeView2.AfterSelect += new TreeViewEventHandler(treeView2_AfterSelect);
            else
                treeView2.AfterSelect -= new TreeViewEventHandler(treeView2_AfterSelect);
        }

        private void _SolutionTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = _sv._SolutionTreeView.SelectedNode;

            if (node == null)
                return;

            string s = node.Tag as string;

            if (s != null)
            {
                string[] cc = s.Split("@".ToCharArray());

                if (cc.Length > 1)
                {
                    OpenFileLine(cc[0], cc[1], 0);
                    return;
                }

                return;
            }
            if (Control.MouseButtons == MouseButtons.Right)
                return;

            s = node.Text;


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
                    file = _sv.mapper.classname;

                    _sv.projectmapper(file, node, false);

                    return;
                }

            VSProject pp = pr0.ps;

            if (pr0.psi != null)

            {
                VSProjectItem pi = pr0.psi;

                file = pi.fileName;

                if (file == null)
                    if (pp != null)
                        file = Path.GetDirectoryName(pp.FileName) + "\\" + pi.Include;

                if (node.Nodes.Count >= 1)
                {
                    if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
                    {
                        return;
                    }
                    else
                    {
                        _sv.projectmapper(file, node, false);
                    }
                }
            }
            else
            {
                CreateView_Solution.ProjectItemInfo pr = pr0;

                return;
            }

            if (file != null)
            {
                if (file.EndsWith(".cs") == true)
                    this.BeginInvoke(new Action(() => { _sv.projectmapper(file, node, false); }));
            }


            if (File.Exists(file) == true)
            {
                // GetVSProject();

                if (CodeEditorControl.AutoListImages == null)
                {
                    ImageList imgs = CreateView_Solution.CreateImageList();
                    VSProject.dc = CreateView_Solution.GetDC();

                    CodeEditorControl.AutoListImages = imgs;
                }

                this.Invoke(new Action(() =>
                {
                    //if (file.EndsWith(".cs"))

                    _eo.LoadFile(file, pp);

                    //else

                    //    scr.OpenFileForm(file);
                }));
            }
        }

        #region Handle node selection

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private List<TreeNode> _selectedNodes;
        private TreeNode _previousNode;

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode currentNode = treeView1.GetNodeAt(e.Location);
            if (currentNode == null)
                return;
            currentNode.BackColor = treeView1.BackColor;
            currentNode.ForeColor = treeView1.ForeColor;

            bool control = (ModifierKeys == Keys.Control);
            bool shift = (ModifierKeys == Keys.Shift);

            if (control)
            {
                // the node clicked with control button pressed:
                //  invert selection of the current node
                List<TreeNode> addedNodes = new List<TreeNode>();
                List<TreeNode> removedNodes = new List<TreeNode>();
                if (!_selectedNodes.Contains(currentNode))
                {
                    addedNodes.Add(currentNode);
                    _previousNode = currentNode;
                }
                else
                {
                    removedNodes.Add(currentNode);
                }
                changeSelection(addedNodes, removedNodes);
            }
            else if (shift && _previousNode != null)
            {
                if (currentNode.Parent == _previousNode.Parent)
                {
                    // the node clicked with shift button pressed:
                    //  if current node and previously selected node 
                    //  belongs to the same parent,
                    //  select range of nodes between these two
                    List<TreeNode> addedNodes = new List<TreeNode>();
                    List<TreeNode> removedNodes = new List<TreeNode>();
                    bool selection = false;
                    bool selectionEnd = false;

                    TreeNodeCollection nodes = null;
                    if (_previousNode.Parent == null)
                    {
                        nodes = treeView1.Nodes;
                    }
                    else
                    {
                        nodes = _previousNode.Parent.Nodes;
                    }
                    foreach (TreeNode n in nodes)
                    {
                        if (n == currentNode || n == _previousNode)
                        {
                            if (selection)
                            {
                                selectionEnd = true;
                            }
                            if (!selection)
                            {
                                selection = true;
                            }
                        }
                        if (selection && !_selectedNodes.Contains(n))
                        {
                            addedNodes.Add(n);
                        }
                        if (selectionEnd)
                        {
                            break;
                        }
                    }

                    if (addedNodes.Count > 0)
                    {
                        changeSelection(addedNodes, removedNodes);
                    }
                }
            }
            else
            {
                //if (!currentNode.NodeFont.Bold)
                //{
                //    // single click: 
                //    //  remove all selected nodes
                //    //  and add current node
                //    List<TreeNode> addedNodes = new List<TreeNode>();
                //    List<TreeNode> removedNodes = new List<TreeNode>();
                //    removedNodes.AddRange(selectedNodes);
                //    if (removedNodes.Contains(currentNode))
                //    {
                //        removedNodes.Remove(currentNode);
                //    }
                //    else
                //    {
                //        addedNodes.Add(currentNode);
                //    }
                //    changeSelection(addedNodes, removedNodes);
                //    previousNode = currentNode;
                //}
            }
        }

        protected void changeSelection(List<TreeNode> addedNodes, List<TreeNode> removedNodes)
        {
            foreach (TreeNode n in addedNodes)
            {
                //if (!n.NodeFont.Bold)
                {
                    //if (treeView1.SelectedNode != null)
                    {
                        n.BackColor = SystemColors.GradientInactiveCaption;// treeView1.SelectedNode.BackColor;
                        n.ForeColor = treeView1.ForeColor;
                    }
                    _selectedNodes.Add(n);
                }
            }
            foreach (TreeNode n in removedNodes)
            {
                n.BackColor = treeView1.BackColor;
                n.ForeColor = treeView1.ForeColor;
                _selectedNodes.Remove(n);
            }
        }

        private TreeNode prevSelected { get; set; }

        protected void drawSelection(TreeNode node, bool added)
        {
            if (prevSelected != null)
            {
            }


            foreach (TreeNode n in _selectedNodes)
            {
                //if (treeView1.SelectedNode != null)
                {
                    n.BackColor = SystemColors.GradientInactiveCaption;
                    n.ForeColor = treeView1.ForeColor;
                }
                // selectedNodes.Add(n);

            }



            if (added == false)
            {
                node.BackColor = treeView1.BackColor;
                node.ForeColor = treeView1.ForeColor;
            }
            else
            {
                node.BackColor = SystemColors.GradientInactiveCaption;
                node.ForeColor = treeView1.ForeColor;
            }
        }

        #endregion Handle node selection



        public bool running = false;

        public ArrayList Queue = new ArrayList();

        private void CodeEditorControl_ParserDataChanged(object sender, EventArgs e)
        {
            if (running == true)
            {
                if (Queue.Count == 0)
                    Queue.Add(sender);
                else Queue[0] = sender;
            }

            if (running == false)

                this.BeginInvoke(new Action(() =>
                {
                    running = true;

                    CodeEditorControl c = sender as CodeEditorControl;

                    string file = c.FileName;

                    TreeNode node = _sv.FindNode(file);

                    if (node == null || node.IsExpanded == false)
                    {
                        running = false;
                        return;
                    }

                    node.EnsureVisible();

                    _sv._SolutionTreeView.SelectedNode = node;

                    _sv._SolutionTreeView.BeginUpdate();

                    _sv.AnalyzeCSharpFile(file, node, c.Document.Text);

                    _sv._SolutionTreeView.EndUpdate();

                    running = false;

                    LoadQueueTask();
                }));
        }

        public void LoadQueueTask()
        {
            object sender;

            if (Queue.Count <= 0)
                return;

            sender = Queue[0];

            Queue.Clear();

            this.BeginInvoke(new Action(() =>
            {
                running = true;

                CodeEditorControl c = sender as CodeEditorControl;

                string file = c.FileName;

                TreeNode node = _sv.FindNode(file);

                if (node == null || node.IsExpanded == false)
                {
                    running = false;
                    return;
                }


                node.EnsureVisible();

                _sv._SolutionTreeView.SelectedNode = node;

                _sv._SolutionTreeView.BeginUpdate();

                _sv.AnalyzeCSharpFile(file, node, c.Document.Text);

                _sv._SolutionTreeView.EndUpdate();

                running = false;
            }));
        }


        private SplashForm splashForm { get; set; }

        private void Proxy_OpenFile1(object sender, Proxy.OpenFileEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Proxy_OpenFile(object sender, Proxy.OpenFileEventArgs e)
        {
            throw new NotImplementedException();
        }

        private class MyRenderer : ToolStripProfessionalRenderer
        {
            private static Color s_specialyellow = Color.FromArgb(0xFF, 255, 242, 157);

            public MyRenderer() : base(new MyColors())
            {
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                {
                    base.OnRenderButtonBackground(e);
                }
                else
                {
                    Rectangle rectangle = new Rectangle(0, 0, e.Item.Size.Width - 1, e.Item.Size.Height - 1);
                    e.Graphics.FillRectangle(new SolidBrush(s_specialyellow), rectangle);
                    e.Graphics.DrawRectangle(new Pen(s_specialyellow), rectangle);
                }
            }
        }

        private class MyColors : ProfessionalColorTable
        {
            private static Color s_specialyellow = Color.FromArgb(0xFF, 255, 242, 157);

            public override Color ButtonSelectedHighlight
            {
                get { return s_specialyellow; }
            }

            public override Color MenuItemBorder
            {
                get { return s_specialyellow; }
            }

            public override Color MenuItemSelected
            {
                get { return s_specialyellow; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return s_specialyellow; }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get { return s_specialyellow; }
            }
        }

        public class clsColor
        {
            public static Color clrHorBG_GrayBlue = Color.FromArgb(255, 233, 236, 250);
            public static Color clrHorBG_White = Color.FromArgb(255, 244, 247, 252);
            public static Color clrSubmenuBG = Color.FromArgb(255, 240, 240, 240);
            public static Color clrImageMarginBlue = Color.FromArgb(255, 212, 216, 230);
            public static Color clrImageMarginWhite = Color.FromArgb(255, 244, 247, 252);
            public static Color clrImageMarginLine = Color.FromArgb(255, 160, 160, 180);
            public static Color clrSelectedBG_Blue = Color.FromArgb(255, 186, 228, 246);

            public static Color clrSelectedBG_Header_Blue =
                        Color.FromArgb(255, 146, 202, 230);

            public static Color clrSelectedBG_White = Color.FromArgb(255, 241, 248, 251);
            public static Color clrSelectedBG_Border = Color.FromArgb(255, 150, 217, 249);
            public static Color clrSelectedBG_Drop_Blue = Color.FromArgb(255, 139, 195, 225);
            public static Color clrSelectedBG_Drop_Border = Color.FromArgb(255, 48, 127, 177);
            public static Color clrMenuBorder = Color.FromArgb(255, 160, 160, 160);
            public static Color clrCheckBG = Color.FromArgb(255, 206, 237, 250);

            public static Color clrVerBG_GrayBlue = Color.FromArgb(255, 196, 203, 219);
            public static Color clrVerBG_White = Color.FromArgb(255, 250, 250, 253);
            public static Color clrVerBG_Shadow = Color.FromArgb(255, 181, 190, 206);

            public static Color clrToolstripBtnGrad_Blue = Color.FromArgb(255, 129, 192, 224);

            public static Color clrToolstripBtnGrad_White =
                        Color.FromArgb(255, 237, 248, 253);

            public static Color clrToolstripBtn_Border = Color.FromArgb(255, 41, 153, 255);

            public static Color clrToolstripBtnGrad_Blue_Pressed =
                        Color.FromArgb(255, 124, 177, 204);

            public static Color clrToolstripBtnGrad_White_Pressed =
                        Color.FromArgb(255, 228, 245, 252);

            public static void DrawRoundedRectangle(Graphics g, int x, int y,
                int width, int height, int m_diameter, Color color)
            {
                using (Pen pen = new Pen(color))
                {
                    //Dim g As Graphics
                    var BaseRect = new RectangleF(x, y, width, height);
                    var ArcRect = new RectangleF(BaseRect.Location,
                    new SizeF(m_diameter, m_diameter));
                    //top left Arc
                    g.DrawArc(pen, ArcRect, 180, 90);
                    g.DrawLine(pen, x + Convert.ToInt32(m_diameter / 2),
                y, x + width - Convert.ToInt32(m_diameter / 2), y);

                    // top right arc
                    ArcRect.X = BaseRect.Right - m_diameter;
                    g.DrawArc(pen, ArcRect, 270, 90);
                    g.DrawLine(pen, x + width, y + Convert.ToInt32(m_diameter / 2),
                x + width, y + height - Convert.ToInt32(m_diameter / 2));

                    // bottom right arc
                    ArcRect.Y = BaseRect.Bottom - m_diameter;
                    g.DrawArc(pen, ArcRect, 0, 90);
                    g.DrawLine(pen, x + Convert.ToInt32(m_diameter / 2),
            y + height, x + width - Convert.ToInt32(m_diameter / 2), y + height);

                    // bottom left arc
                    ArcRect.X = BaseRect.Left;
                    g.DrawArc(pen, ArcRect, 90, 90);
                    g.DrawLine(pen, x, y + Convert.ToInt32(m_diameter / 2),
                x, y + height - Convert.ToInt32(m_diameter / 2));
                }
            }
        }

        public class VS2008ToolStripRenderer : ToolStripProfessionalRenderer
        {
            // Render custom background gradient
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                base.OnRenderToolStripBackground(e);

                using (var b = new LinearGradientBrush(e.AffectedBounds,
                    clsColor.clrVerBG_White, clsColor.clrVerBG_GrayBlue,
                    LinearGradientMode.Vertical))
                {
                    using (var shadow = new SolidBrush(clsColor.clrVerBG_Shadow))
                    {
                        var rect = new Rectangle
                (0, e.ToolStrip.Height - 2, e.ToolStrip.Width, 1);
                        e.Graphics.FillRectangle(b, e.AffectedBounds);
                        e.Graphics.FillRectangle(shadow, rect);
                    }
                }
            }

            // Render button selected and pressed state
            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderButtonBackground(e);
                var rectBorder = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                var rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);

                if (e.Item.Selected == true || (e.Item as ToolStripButton).Checked)
                {
                    using (var b = new LinearGradientBrush
                (rect, clsColor.clrToolstripBtnGrad_White,
                        clsColor.clrToolstripBtnGrad_Blue, LinearGradientMode.Vertical))
                    {
                        using (var b2 = new SolidBrush(clsColor.clrToolstripBtn_Border))
                        {
                            e.Graphics.FillRectangle(b2, rectBorder);
                            e.Graphics.FillRectangle(b, rect);
                        }
                    }
                }
                if (e.Item.Pressed)
                {
                    using (var b = new LinearGradientBrush
                (rect, clsColor.clrToolstripBtnGrad_White_Pressed,
                        clsColor.clrToolstripBtnGrad_Blue_Pressed,
                    LinearGradientMode.Vertical))
                    {
                        using (var b2 = new SolidBrush(clsColor.clrToolstripBtn_Border))
                        {
                            e.Graphics.FillRectangle(b2, rectBorder);
                            e.Graphics.FillRectangle(b, rect);
                        }
                    }
                }
            }
        }

        private void Scr_ProjectProperties(object sender, EventArgs e)
        {
            DocumentForm df = sender as DocumentForm;
            LoadListTabsReload(df);
        }

        public void LoadDefaults()
        {
            SuspendLayout();

            LoadDocumentWindow(true);

            LoadSE("Explorer", true);
            solutionExplorerToolStripMenuItem.Checked = true;
            LoadCD("Class View", true);
            classViewToolStripMenuItem.Checked = true;
            //           CreateOutputWindow();
            //           LoadOW("Output", true);
            //           outputWindowToolStripMenuItem.Checked = true;
            LoadFD("Folders", true);
            fileExplorerToolStripMenuItem.Checked = true;
            LoadRE("Recent", true);
            recentProjectsToolStripMenuItem.Checked = true;
            LoadPG("Properties", true);
            propertyGridToolStripMenuItem.Checked = true;
            LoadERW("ErrorList", true);
            outputWindowToolStripMenuItem.Checked = true;
            LoadORW("Outputs", true);
            LoadRW("Find Replace", true);
            ResumeLayout();
        }

        public FindReplaceForm fr { get; set; }

        public ToolWindow rw { get; set; }

        public void LoadRW(string name, bool load)
        {
            rw = new ToolWindow();

            rw.Text = name;
            rw.FormBorderStyle = FormBorderStyle.Sizable;
            rw.TabText = name;
            rw.TopLevel = true;

            if (fr == null)
            {
                fr = new FindReplaceForm(this);
                fr.TopLevel = false;
                fr.FormBorderStyle = FormBorderStyle.None;
                fr.Dock = DockStyle.Fill;
                fr.Show();
            }
            rw.Controls.Add(fr);

            rw.types = "rw";

            if (load)
                rw.Show(dock, DockState.DockBottom);
        }

        public ToolWindow ows { get; set; }

        public OutputForm ofm { get; set; }

        RichTextBox rbb { get; set; }

        public void LoadORW(string name, bool load)
        {
            ows = new ToolWindow();

            ows.Text = name;
            ows.FormBorderStyle = FormBorderStyle.Sizable;
            ows.TabText = name;
            ows.TopLevel = true;

            if (ofm == null)
            {
                ofm = new OutputForm();
                ofm.TopLevel = false;
                ofm.FormBorderStyle = FormBorderStyle.None;
                ofm.Dock = DockStyle.Fill;
                ofm.Show();
            }

            rbb = ofm.rb;
            ows.Controls.Add(ofm);

            VSProvider.cmds.rb = rbb;

            OutputForm.ofm = ofm;


            ows.types = "ou";

            if (load)
                ows.Show(dock, DockState.DockBottom);


     



        }

        public ErrorListForm efs { get; set; }

        public ToolWindow ews { get; set; }

        public void LoadERW(string name, bool load)
        {
            ews = new ToolWindow();

            ews.Text = name;
            ews.FormBorderStyle = FormBorderStyle.Sizable;
            ews.TabText = name;
            ews.TopLevel = true;

            if (efs == null)
            {
                efs = new ErrorListForm();
                efs.LoadEF(this);
                efs.TopLevel = false;
                efs.FormBorderStyle = FormBorderStyle.None;
                efs.Dock = DockStyle.Fill;
                efs.Show();
            }
            ews.Controls.Add(efs);
            ews.types = "ef";

            if (load)
                ews.Show(dock, DockState.DockBottom);
        }

        public FindResultsForm rfs { get; set; }

        public ToolWindow rws { get; set; }

        public void LoadRF(string name, bool load)
        {
            rws = new ToolWindow();

            rws.Text = name;
            rws.HideOnClose = true;
            //tw.Controls.Add(scr);
            rws.FormBorderStyle = FormBorderStyle.Sizable;
            //ww.ControlBox = false;
            rws.TabText = name;
            rws.TopLevel = true;

            if (rfs == null)
            {
                rfs = new FindResultsForm();
                rfs.ef = this;
                rfs.TopLevel = false;
                rfs.FormBorderStyle = FormBorderStyle.None;
                rfs.Dock = DockStyle.Fill;
                rfs.Show();
            }
            rws.Controls.Add(rfs);
            rws.types = "rr";

            //rws.Show();

            if (load)
                rws.Show(dock, DockState.DockBottom);

            //    rw.Closed += new EventHandler(closed_ow);
        }

        public FindResultsForm rfs2 { get; set; }

        public ToolWindow rws2 { get; set; }

        public void LoadRF2(string name, bool load)
        {
            rws2 = new ToolWindow();

            rws2.Text = name;
            rws2.HideOnClose = true;
            rws2.FormBorderStyle = FormBorderStyle.Sizable;

            rws2.TabText = name;
            rws2.TopLevel = true;

            if (rfs2 == null)
            {
                rfs2 = new FindResultsForm();
                rfs2.ef = this;
                rfs2.TopLevel = false;
                rfs2.FormBorderStyle = FormBorderStyle.None;
                rfs2.Dock = DockStyle.Fill;
                rfs2.Show();
            }
            rws2.Controls.Add(rfs2);
            rws2.types = "rr2";

            if (load)
                rws2.Show(dock, DockState.DockBottom);
        }

        public ToolWindow ow { get; set; }

        public void LoadOW(string name, bool load)
        {
            ow = new ToolWindow();
            ow.Text = name;
            se.HideOnClose = true;
            ow.FormBorderStyle = FormBorderStyle.None;
            //ww.ControlBox = false;
            ow.TabText = name;
            ow.TopLevel = false;
            //ww.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            ow.Controls.Add(forms);

            //update_z_order(ow);

            ow.types = "ow";

            if (load)
                ow.Show(dock, DockState.DockBottom);

            ow.Closed += new EventHandler(closed_ow);
        }

        public ToolWindow aw { get; set; }

        public StyleListForm slf { get; set; }

        public void LoadAW(string name, bool load)
        {
            aw = new ToolWindow();
            aw.Text = name;
            aw.HideOnClose = true;
            aw.FormBorderStyle = FormBorderStyle.None;
            aw.TabText = name;
            aw.TopLevel = false;

            if (slf == null)
            {
                slf = new StyleListForm();
                slf.ef = this;
                slf.TopLevel = false;
                slf.FormBorderStyle = FormBorderStyle.None;
                slf.Dock = DockStyle.Fill;
                slf.Show();
            }

            aw.Controls.Add(slf);

            aw.types = "aw";

            if (load)
                aw.Show(dock, DockState.DockBottom);
        }

        public ToolWindow wt { get; set; }

     
        private static NUnit.Gui.Views.MainForm nunit()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);



            var testEngine = TestEngineActivator.CreateInstance(true);
            CommandLineOptions options = new CommandLineOptions();

            var model = new TestModel(testEngine, options);

            var form = new NUnit.Gui.Views.MainForm();
            MainPresenter mp = new MainPresenter(form, model);

            form.mp = mp;

            new ProgressBarPresenter(form.ProgressBarView, model);
            new TreeViewPresenter(form.TestTreeView, model);
            new StatusBarPresenter(form.StatusBarView, model);
            new TestPropertiesPresenter(form.PropertiesView, model);
            new XmlPresenter(form.XmlView, model);

            //new RecentFiles(settingsServiceServiceService._settings);
            //new RecentFilesPresenter(form, settingsServiceServiceService);

            return form;
        }
        public ToolWindow nt { get; set; }

        public NUnit.Gui.Views.MainForm gunit { get; set; }



        public void LoadNU(string name, bool load)
        {
            nt = new ToolWindow();
            nt.Text = name;
            nt.HideOnClose = true;
            nt.FormBorderStyle = FormBorderStyle.None;
            nt.TabText = name;
            nt.TopLevel = false;

            if (gunit == null)
            {
                gunit = nunit();
                gunit.TopLevel = false;
                gunit.FormBorderStyle = FormBorderStyle.None;
                gunit.Dock = DockStyle.Fill;
                gunit.Show();
            }

            nt.Controls.Add(gunit);

            nt.types = "nt";

            gunit.ttv.loadButton.Click += LoadButton_Click;

            if (load)
                nt.Show(dock, DockState.DockBottom);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (gunit == null)
                return;

            gunit.mp.Files = vs.GetExecutables();

            gunit.mp.OpenFiles();
        }

        public void closed_ow(object sender, EventArgs e)
        {
            ow.Hide();
            outputWindowToolStripMenuItem.Checked = false;
        }

        public ToolWindow cd { get; set; }

        public void LoadCD(string name, bool load)
        {
            cd = new ToolWindow();
            cd.Text = name;
            cd.HideOnClose = true;
            cd.FormBorderStyle = FormBorderStyle.None;
            cd.TabText = name;
            cd.TopLevel = false;

            cd.Controls.Add(toolStrip5);
            cd.Controls.Add(textBox4);
            SetControls(textBox4, cd);
            cd.Controls.Add(splitContainer3);
            SetControls(splitContainer3, cd, true);
            splitContainer3.Anchor = splitContainer3.Anchor | AnchorStyles.Bottom;

            cd.types = "cd";

            if (load)
                cd.Show(dock, DockState.DockRight);
        }

        public ToolWindow re { get; set; }



        public void LoadRE(string name, bool load)
        {
            re = new ToolWindow();
            re.Text = name;
            re.HideOnClose = true;
            re.FormBorderStyle = FormBorderStyle.None;

            re.TabText = name;
            re.TopLevel = false;

            re.Controls.Add(toolStrip3);
            re.Controls.Add(textBox3);
            SetControls(textBox3, re);
            re.Controls.Add(treeView3);
            ImageList imgs = CreateView_Solution.CreateImageList();
            treeView3.ImageList = imgs;

            SetControls(treeView3, re, true);
            treeView3.Anchor = treeView3.Anchor | AnchorStyles.Bottom;

            re.types = "re";

            if (load)
                re.Show(dock, DockState.DockRight);
        }

        public ToolWindow fd { get; set; }

        public void LoadFD(string name, bool load)
        {
            fd = new ToolWindow();
            fd.Text = name;
            fd.HideOnClose = true;

            fd.FormBorderStyle = FormBorderStyle.None;

            fd.TabText = name;
            fd.TopLevel = false;

            forms = new FolderLister();
            forms.Dock = DockStyle.Fill;
            forms.open.Click += Open_Click;
            fd.Controls.Add(forms);

            update_z_order(fd);

            fd.types = "fd";

            if (load)
                fd.Show(dock, DockState.DockRight);
        }

        private void Open_Click(object sender, EventArgs e)
        {
            string recent = forms.tb.Text;

            if (recent == "")
                return;

            if (File.Exists(recent) == false)
                return;

            if (scr != null)
                scr.ClearHistory();

            ClearOutput();

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        private PropertyGrid pgp { get; set; }
        public ToolWindow pw { get; set; }

        public void LoadPG(string name, bool load)
        {
            pgp = new PropertyGrid();
            pgp.Dock = DockStyle.Fill;

            pw = new ToolWindow();
            pw.Text = name;

            pw.FormBorderStyle = FormBorderStyle.None;

            pw.TabText = name;
            pw.TopLevel = false;

            pw.Controls.Add(pgp);

            pw.types = "pw";

            if (load)
                pw.Show(dock, DockState.DockRight);
        }

        public ToolWindow se { get; set; }

        public ToolStrip ses { get; set; }

        public ToolStripButton sesf { get; set; }

        public ToolStripButton sesb { get; set; }

        public ToolStripSeparator sess { get; set; }

        public ToolStripButton sest { get; set; }

        public ToolStripButton sesc { get; set; }

        public ToolStripButton sesa { get; set; }

        public Navigator nav { get; set; }

        public void LoadSE(string name, bool load)
        {
            se = new ToolWindow();
            se.Text = name;
            se.HideOnClose = true;
            //tw.Controls.Add(scr);
            se.FormBorderStyle = FormBorderStyle.None;
            //ww.ControlBox = false;
            se.TabText = name;
            se.TopLevel = false;
            //ww.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            nav = new Navigator(treeView1);

            ses = new ToolStrip();

            ses.GripStyle = ToolStripGripStyle.Hidden;

            sesb = new ToolStripButton();
            sesb.DisplayStyle = ToolStripItemDisplayStyle.Image;
            sesb.Image = resource_alls.NavigateBackwards_62701;
            sesb.ToolTipText = "Backward";
            sesb.Click += Sest_Click;
            ses.Items.Add(sesb);

            sesf = new ToolStripButton();
            sesf.DisplayStyle = ToolStripItemDisplayStyle.Image;
            sesf.Image = resource_alls.NavigateForward_62711;
            sesf.ToolTipText = "Forward";
            sesf.Click += Sesf_Click;
            ses.Items.Add(sesf);

            sess = new ToolStripSeparator();

            ses.Items.Add(sess);




            sest = new ToolStripButton();
            sest.DisplayStyle = ToolStripItemDisplayStyle.Image;
            sest.Image = resource_alls.synchronize;
            sest.Click += Sest_Click;
            ses.Items.Add(sest);

            sesc = new ToolStripButton();
            sesc.DisplayStyle = ToolStripItemDisplayStyle.Image;
            sesc.Image = resource_alls.collapse;
            sesc.ToolTipText = "Collapse";
            sesc.Click += Sesc_Click;
            ses.Items.Add(sesc);

            sesa = new ToolStripButton();
            sesa.DisplayStyle = ToolStripItemDisplayStyle.Image;
            sesa.Image = resource_alls.showall;
            sesa.ToolTipText = "Show All";
            sesa.Click += Sesa_Click;
            ses.Items.Add(sesa);

            se.Controls.Add(treeView1);

            se.Controls.Add(ses);

            se.types = "se";

            if (load)
                se.Show(dock, DockState.DockRight);

            if (CodeEditorControl.settings != null)
            {
                Settings s = CodeEditorControl.settings;

                if (s.Theme == "VS2012Light")
                {
                    treeView1.BackColor = Color.FromKnownColor(KnownColor.Control);
                    treeView1.BackColor = Color.FromKnownColor(KnownColor.Control);
                }
            }
        }

        private void Sesf_Click(object sender, EventArgs e)
        {
            nav.Next();
        }

        private void Sesc_Click(object sender, EventArgs e)
        {
            _sv._SolutionTreeView.CollapseAll();
        }

        private void Sesa_Click(object sender, EventArgs e)
        {
            _sv._SolutionTreeView.ExpandAll();
        }

        private void Sest_Click(object sender, EventArgs e)
        {
            nav.Prev();
        }

        public static System.Drawing.Bitmap CombineBitmap(string[] files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

        public ToolWindow ww { get; set; }

        public void LoadTW(string name, bool load)
        {
            ww = new ToolWindow();
            ww.Text = name;
            //tw.Controls.Add(scr);
            ww.FormBorderStyle = FormBorderStyle.None;
            //ww.ControlBox = false;
            ww.TabText = name;
            ww.TopLevel = false;
            //ww.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            ww.types = "ww";

            if (load)
                ww.Show(dock, DockState.DockRight);
        }

        public ToolWindow tws { get; set; }

        public void LoadDocumentWindow(bool load)
        {
            tws = new ToolWindow();
            tws.Controls.Add(scr);
            tws.FormBorderStyle = FormBorderStyle.None;
            //tw.ControlBox = false;
            tws.Text = String.Empty;
            tws.TopLevel = false;
            //((DockContent)tw).AllowEndUserDocking = false;
            //tw.DockAreas = DockAreas.DockTop;
            scr.Dock = DockStyle.Fill;

            tws.TabText = "Documents";

            tws.Dock = DockStyle.Fill;

            tws.types = "tws";

            if (load)
                tws.Show(dock, DockState.Document);
        }

        public void SetAnchor(Control c)
        {
            c.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
        }

        public void SetControls(Control c, Control p)
        {
            int w = p.Width;

            c.Width = w - 4;

            c.Location = new Point(2, c.Location.Y);

            SetAnchor(c);
        }

        public void SetControls(Control c, Control p, bool b)
        {
            int w = p.Width;

            int h = p.Height;

            c.Width = w - 4;

            c.Height = h - c.Location.Y - 4;

            c.Location = new Point(2, c.Location.Y);

            SetAnchor(c);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            {
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 2)
                    return null;

                if (parsedStrings[0] != typeof(ToolWindow).ToString())
                    return null;

                if (parsedStrings[1] == "se")
                    return se;
                else if (parsedStrings[1] == "re")
                    return re;
                //else if (parsedStrings[1] == "ow")
                //    return ow;
                else if (parsedStrings[1] == "fd")
                    return fd;
                else if (parsedStrings[1] == "tws")
                    return tws;
                else if (parsedStrings[1] == "cd")
                    return cd;
                else if (parsedStrings[1] == "pw")
                    return pw;
                else if (parsedStrings[1] == "ef")
                    return ews;
                else if (parsedStrings[1] == "ou")
                    return ows;
                else if (parsedStrings[1] == "rw")
                    return rw;
                else if (parsedStrings[1] == "rr")
                    return rws;
                else if (parsedStrings[1] == "rr2")
                    return rws2;
                else if (parsedStrings[1] == "aw")
                    return aw;
                else if (parsedStrings[1] == "wt")
                    return wt;
                else if (parsedStrings[1] == "nt")
                    return nt;
                return null;
            }
        }

        public DockPanel dock { get; set; }

        public ScriptControl scr { get; set; }

        private DeserializeDockContent _deserializeDockContent;

        public void HistoryChanged(object sender, EventArgs e)
        {
            _sv.HistoryChanged(scr.hst.hst);
        }

        private void ListViewListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView lv = sender as ListView;

            if (lv.SelectedItems.Count == 0) return;

            ListViewItem v = lv.SelectedItems[0];

            if (v.Tag == null)
                return;

            Microsoft.Build.Framework.BuildWarningEventArgs ws = v.Tag as Microsoft.Build.Framework.BuildWarningEventArgs;

            if (ws != null)
            {
                string s = Path.GetDirectoryName(ws.ProjectFile);

                _eo.LoadFile(s + "\\" + ws.File, null, null, null);

                _eo.SelectText(ws.LineNumber, ws.ColumnNumber, 10);
            }
            else
            {
                Microsoft.Build.Framework.BuildErrorEventArgs es = v.Tag as Microsoft.Build.Framework.BuildErrorEventArgs;

                string s = Path.GetDirectoryName(es.ProjectFile);

                _eo.LoadFile(s + "\\" + es.File, null, null, null);

                _eo.SelectText(es.LineNumber, es.ColumnNumber, 10);
            }
        }

        private void comboBox1_DrawItem(object sender, EventArgs e)
        {
            try
            {
                ToolStripComboBox c = sender as ToolStripComboBox;

                int i = c.SelectedIndex;

                if (i >= c.Items.Count)
                    return;

                VSSolution vs = GetVSSolution();

                if (vs != null)
                {
                    VSProject vp = vs.MainVSProject;

                    if (vp != null)
                    {
                        MessageBox.Show("Active project - " + vp.Name);
                    }

                    vp.SetPlatformConfig("", "");
                }

                string text = c.Items[i].ToString();

                if (text == "Configuration Manager")

                    LoadConfigurationManager();
            }
            catch (Exception ee) { }
        }

        private ConfigurationManagerForm cmf { get; set; }

        public void LoadConfigurationManager()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            if (vs == null)
                return;

            cmf = new ConfigurationManagerForm();
            cmf.LoadConfigs(vs);
            cmf.SetConfigView();
            cmf.LoadPage_MSBuild();
            cmf.ShowDialog();
        }

        private ContextMenuStrip context1 { get; set; }
        private ContextMenuStrip context2 { get; set; }
        private ContextMenuStrip context3 { get; set; }
        private ContextMenuStrip context4 { get; set; }
        private ContextMenuStrip context5 { get; set; }
        private ContextMenuStrip context6 { get; set; }

        //QuickSharp.Outputs.OutputForms forms { get; set; }

        private FolderLister forms { get; set; }

        public FolderLister CreateOutputWindow()
        {
            //ApplicationManager app = ApplicationManager.GetInstance();

            // forms = new QuickSharp.Outputs.OutputForms();

            forms = new FolderLister();

            // forms.TopLevel = false;

            forms.Dock = DockStyle.Fill;

            // forms.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            forms.Show();

            return forms;

            //forms.Show();

            //splitContainer2.Panel2.Controls.Add(forms);

            //forms.Show();

            splitContainer2.SplitterDistance = (int)((double)splitContainer2.Height * 0.8);
        }

        public void ClearOutput()
        {
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

        private PSForm psf { get; set; }

        public void LoadCMD(Form f, string FileName)
        {
            cmdf = new Commands(FileName);
            cmdf.Dock = DockStyle.Fill;
            cmdf.TopLevel = false;
            cmdf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            f.Controls.Add(cmdf);

            cmdf.Show();
        }

        private Commands cmdf { get; set; }

        public void LoadPS(Form f, string FileName)
        {
            psf = new PSForm();
            psf.Dock = DockStyle.Fill;
            psf.TopLevel = false;
            psf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            f.Controls.Add(psf);

            psf.Show();
        }


        private ProjectPropertyForm ppf { get; set; }

        public void LoadListTabs(Form f)
        {
            ppf = new ProjectPropertyForm();
            ppf.Dock = DockStyle.Fill;
            ppf.TopLevel = false;
            ppf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            f.Controls.Add(ppf);

            ppf.Show();

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            ppf.Text = Path.GetFileNameWithoutExtension(prs.filepath);

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            if (prs.ps == null)
                return;

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(prs.ps.FileName);
            }
            catch (Exception e) { }

            if (pc == null)
            {
                MessageBox.Show("Project " + Path.GetFileNameWithoutExtension(prs.ps.FileName) + " could not be opened properly.");

                return;
            }

            ppf.pc = pc;

            ppf.vp = prs.ps;

            ppf.vs = GetVSSolution();

            ppf.GetProperties();
        }

        public void LoadListTabsReload(DocumentForm f)
        {
            ppf = new ProjectPropertyForm();
            ppf.Dock = DockStyle.Fill;
            ppf.TopLevel = false;
            ppf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            f.Controls.Add(ppf);

            ppf.Show();

            string[] d = f.FileName.Split("@".ToCharArray());

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            ppf.Text = Path.GetFileNameWithoutExtension(Path.GetFileName(d[0]));

            f.FileName = Path.GetFileNameWithoutExtension(Path.GetFileName(d[0]));

            f.FileName = d[0];

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(d[0]);
            }
            catch (Exception e) { }

            if (pc == null)
            {
                MessageBox.Show("Project " + Path.GetFileNameWithoutExtension(prs.ps.FileName) + " could not be opened properly.");

                return;
            }

            ppf.pc = pc;

            ppf.vs = GetVSSolution();

            ppf.vp = ppf.vs.GetProjectbyFileName(d[0]);

            ppf.GetProperties();

            if (d.Length > 1)
                if (d[1].EndsWith(".resx"))
                {
                    ppf.LoadResourcesView(d[1]);
                    f.resource = d[1];
                }
        }

        public void loadcurrentproject()
        {
            try
            {
                string d = AppDomain.CurrentDomain.BaseDirectory;

                string s = File.ReadAllText(d + "\\" + "currentproject.txt");

                _eo.currentproject = s;
            }
            catch (Exception e) { };
        }

        public void loadcontmenu()
        {
            _eo._components = _components;
            _eo.master = this;
            _eo.tw = _tw;
        }

        public void loadtoolmenu()
        {
            _eo._currentDirectoryFollowsRoot = true;// _currentDirectoryFollowsRoot;

            _eo._components = _components;
            _eo.tw = _tw;
            _eo.script = scr;// scriptControl1;

            _tw.eo = _eo;

            _cc.eo = _eo;

            _cc._mainToolStrip = toolStrip4;
            _cc.doinit(this);

            _sv.eo = _eo;

            _sv._mainToolStrip = toolStrip4;
            _sv.doinit(this, splitContainer1, tabPage3, treeView1, treeView2, listview, null);
            _sv.hst.tv = treeView3;
            _sv.hst.ReloadRecentSolutions();
            _sv.context1 = contextMenuStrip1;
            _sv.context2 = contextMenuStrip2;

            //sv.eo = eo;
            //sv._maintv = ts._mainTreeView;// _mainTreeView;
            //sv._mainToolStrip = ts._mainToolStrip;
            //sv.doinit(this);

            _ls.eo = _eo;
            //ls._maintv = ts._mainTreeView;// _mainTreeView;
            _ls._mainToolStrip = toolStrip4;
            _ls.doinit(this);

            _eo.cvs = _sv;

            update_z_order(tabPage1);
        }

        private WinExplorer.ExplorerObject _eo = ExplorerObject.GetInstance();

        private WinExplorer.TreeViewBase _tw = new TreeViewBase();

        private WinExplorer.CreateView _cc = new CreateView();

        private WinExplorer.CreateView_Solution _sv = new CreateView_Solution();

        private WinExplorer.CreateView_Logger _ls = new CreateView_Logger();

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

        #region Toolbar Events



        #endregion Toolbar Events

        #region Form Events

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        #endregion Form Events

        #region Public UI Elements



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



        #endregion Public UI Elements


        #region Helpers



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
                if (c.GetType() == typeof(ToolStripPanel) || c.GetType() == typeof(ToolStrip) || c.GetType() == typeof(StatusStrip))
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
                //if (c.GetType() == typeof(MenuStrip))
                //    p.Controls.Add(c);
                //if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true)
                //    p.Controls.Add(c);
            }

            foreach (Control c in L)
            {
                if (c.GetType() == typeof(ToolStrip))
                    p.Controls.Add(c);
                if (c.GetType() == typeof(ToolStripPanel))
                    p.Controls.Add(c);
                if (c.GetType() == typeof(StatusStrip))
                    p.Controls.Add(c);

                if (c.GetType() == typeof(MenuStrip))
                    p.Controls.Add(c);
                if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true)
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
            _sv.AddSolutionItem_Click(this, new EventArgs());
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

                    //scriptControl1.dockContainer1.ActiveDocumentPane.master = scriptControl1;
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

                    // scriptControl1.dockContainer1.ActiveDocumentPane.master = scriptControl1;
                }
            }
        }

        private BackgroundWorker _bw = null;

        public void startprogress()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

            //bw.RunWorkerAsync();
            //bw.CancelAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            _bw.ReportProgress(50);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //make sure ProgressBar is not continuous, and has a max value
            progressBar1.Value = e.ProgressPercentage;
        }

        private Font font { get; set; }

        public void Loads2(TreeView tv, TreeView s)
        {
            //sv._SolutionTreeView.ImageList = tv.ImageList;

            font = new Font(s.Font, FontStyle.Bold);

            if (tv == null)
                return;


            s.BeginInvoke(new Action(() =>
            {
                s.Nodes.Clear();

                s.BeginUpdate();

                s.ImageList = tv.ImageList;

                s.Tag = tv.Tag;

                s.EndUpdate();
            }));

            if (tv == null)
                return;

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

        public delegate void workerBuildDelegate(string recent, msbuilder_alls.MSBuild msbuilds);

        public delegate void workerFunctionDelegate(string recent);

        public delegate void workerTypeViewerDelegate(VSSolution vs);

        public delegate void PopulateTreeNodes(TreeView s, TreeView tv, TreeNode ns);

        public delegate void workerFunctionTestDelegate(string recent);

        private void workerFunction(string recent)
        {
            this.BeginInvoke(new Action(() => { SolutionClose(); }));

            TreeView tv = _sv.load_recent_solution(recent);


            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            if (CodeEditorControl.AutoListImages == null)
            {
                ImageList imgs = CreateView_Solution.CreateImageList();
                VSProject.dc = CreateView_Solution.GetDC();

                CodeEditorControl.AutoListImages = imgs;
            }

            ArrayList L = null;


            this.Invoke(new Action(() =>
            {
                Loads2(tv, _sv._SolutionTreeView); this.Text = Path.GetFileName(recent) + " - Explorer Studio";
                if (scr != null)
                {
                    vs = GetVSSolution();
                    scr.BeginInvoke(new Action(() =>
                    {
                        L = scr.LoadSolution(vs);
                        _sv.main_project();
                        CreateRecentWindows(L);

                        if (splashForm != null)
                        {
                            splashForm.Hide();
                            splashForm = null;
                        }
                        this.Show();
                        Visible = true;
                        Opacity = 100;
                        this.Show();
                    }));
                }
            }));
        }

        private void TestForm(string s)
        {
            Random r = new Random();

            int N = 1;

            if (tsf != null)
                if (tsf.ts != null)
                    N = tsf.ts.iters;

            int i = 0;
            while (i < N)
            {
                int X = (int)(r.NextDouble() * 1900.0);
                int Y = (int)(r.NextDouble() * 1000.0);

                int W = 400 + (int)(r.NextDouble() * 1000.0);
                int H = 400 + (int)(r.NextDouble() * 1000.0);

                int dx = 1;
                if (X > 1000)
                    dx = -1;

                int dy = 1;
                if (Y > 500)
                    dy = -1;

                Size sz = new Size(W, H);
                Point p = new Point(X, Y);

                int j = 0;
                while (j < 20)
                {
                    //WindowState = FormWindowState.Minimized;

                    sz.Width += dy * 350;
                    sz.Height += dx * 350;

                    if (sz.Width < 100)
                        sz.Width = 400;
                    if (sz.Height < 150)
                        sz.Height = 400;

                    p.X += dx * 55;
                    p.Y += dy * 55;

                    if (p.X > 1900)
                        p.X = 150;

                    if (p.Y > 500)
                        p.Y = 150;

                    if (p.X + sz.Width > 2000)
                    {
                        sz.Width = 500;
                        p.X = 0;
                    }
                    if (p.Y + sz.Height > 1000)
                    {
                        sz.Height = 500;
                        p.Y = 0;
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        this.Size = sz;
                        this.Location = p;

                        this.Refresh();
                    }));

                    j++;
                }

                this.BeginInvoke(new Action(() => { ResumeAtStartUp(); /*Thread.Sleep(10000); */}));

                //this.WindowState = FormWindowState.Maximized;

                i++;
            }
        }

        private void workerTypeViewer(VSSolution vs)
        {
            TreeView tt = _cc.LoadSolution(vs);

            msbuilder_alls.SortProjectNodes(tt);

            this.BeginInvoke(new Action(() =>
            {
                treeView2.Nodes.Clear();
                treeView2.ImageList.TransparentColor = Color.Fuchsia;
                treeView2.ImageList = CreateView_Solution.CreateImageList();
                ArrayList L = new ArrayList();
                foreach (TreeNode node in tt.Nodes)
                    L.Add(node);
                foreach (TreeNode node in L)
                {
                    tt.Nodes.Remove(node);
                    treeView2.Nodes.Add(node);
                };
            }));
        }

        private void populateTreeNode(TreeView s, TreeView tv, TreeNode ns)
        {
            s.BeginInvoke(new Action(() =>
            {
                s.BeginUpdate();
                tv.Nodes.Remove(ns);
                s.Nodes.Add(ns);
                s.EndUpdate();
            }));
        }

        private void reload_solution(string recent)
        {
            SolutionClose();
            _sv._SolutionTreeView.Nodes.Clear();

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

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }


        public void CloseCurrentSolution()
        {
            scr.ClearHistory();

            ClearOutput();

            if (treeView3.SelectedNode != null)
            {
                string recent = treeView3.SelectedNode.Text;

                _sv._SolutionTreeView.Nodes.Clear();

                _sv.save_recent_solution(recent);
            }

            SolutionClose();
        }

        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);

                //   if (scriptControl1.ActiveControl != null)
                //       scriptControl1.ActiveControl.Size = splitContainer1.Panel2.Size;
            }
            catch (Exception ee) { }

            GC.Collect();

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
            _sv.slnpath = e.Node.Text;
            textBox3.Text = _sv.slnpath;
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

            _sv.hst.remitem(text);

            _sv.hst.ReloadRecentSolutions();
        }

        private void setAsMainProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _sv.set_main_project();
        }

        private SplitContainer sp { get; set; }

        private SplitContainer sc { get; set; }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
        }

        //public void LoadOutput()
        //{
        //    sc = splitContainer2;

        //    if (sc.Panel2Collapsed == true)
        //        sc.Panel2Collapsed = false;
        //    sc.Panel2Collapsed = true;
        //}

        private void builds_alls()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;
            if (vs == null)
                return;

            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            workerBuildDelegate b = build_alls;
            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);
        }

        public void rebuild_alls(string recent, msbuilder_alls.MSBuild msbuilds)
        {
            _sv.build_solution(recent, true, msbuilds);
        }

        public void build_alls(string recent, msbuilder_alls.MSBuild msbuilds)
        {
            _sv.build_solution(recent, false, msbuilds);
        }

        public void build_project(string recent, msbuilder_alls.MSBuild msbuilds)
        {
            _sv.build_project_solution(recent);
        }

        private ProgressBar pg { get; set; }

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

            _sv.pg = pg;
        }

        private bool _started = false;

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (_started == true)
                return;

            string recent = _sv.load_recent_solution();

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            //CreateProgressBar();

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        public void ResumeAtStartUp()
        {
            if (_started == true)
                return;

            _started = true;

            //SolutionClose();

            string recent = _sv.load_recent_solution();

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

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
            _sv._configList.Enabled = false;
            _sv._platList.Enabled = false;

            _sv._configList.SetDrawMode(false);
            _sv._platList.SetDrawMode(false);

            _sv.LoadPlatforms(GetVSSolution());

            _sv._configList.Enabled = true;
            _sv._platList.Enabled = true;

            _sv._configList.SetDrawMode(true);
            _sv._platList.SetDrawMode(true);

            _sv._mainToolStrip.Refresh();

            _sv._projectList.SetDrawMode(true);
        }

        private AIMS.Libraries.Scripting.ScriptControl.DocumentForm df { get; set; }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (_sv == null)
                return;
            if (_sv.eo == null)
                return;

            df = _sv.eo.OpenDocumentForm("test");
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

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

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
            //OpenStyleAnalysisCode();
            //return;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode MainProjectNode = _eo.cvs.getactivenode();

            MessageBox.Show("Main project properties " + prs.ps.Name);

            if (_sv == null)
                return;
            if (_sv.eo == null)
                return;

            df = _sv.eo.OpenDocumentForm(prs.ps.Name);

            df.FileName = prs.ps.FileName;

            LoadListTabs(df);
        }

        private StyleCop.StyleCopCore core { get; set; }

        private StyleCop.PropertyDialog ppd { get; set; }

        public void OpenStyleAnalysisCode()
        {
            VSSolution vs = GetVSSolution();
            VSProject vp = GetVSProject();
            DocumentForm df = _sv.eo.OpenDocumentForm(vp.FileName);
            df.FileName = vp.FileName;

            StyleCop.StyleCopCore core = new StyleCop.StyleCopCore(null, null);
            core.Initialize(null, true);
            core.WriteResultsCache = false;
            core.DisplayUI = true;
            core.ShowSettings("Settings.StyleCop");

            ppd = core.properties;
            ppd.Dock = DockStyle.Fill;
            ppd.TopLevel = false;
            ppd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            df.Controls.Add(ppd);

            ppd.Show();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            //MainProjectNode = eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            string r = _sv.GetProjectExec(prs.ps.FileName);

            cmds.RunExternalExe(r, "");
        }

        //public void startapache()
        //{
        //    string apache = AppDomain.CurrentDomain.BaseDirectory + "\\" + "xampp\\apache_start.bat";

        //    cmds.RunExternalExe(apache, "");

        //}

        //RefManagerForm rmf { get; set; }

        private GACForm gf { get; set; }

        private void addReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSSolution vs = prs.vs;

            VSProject pp = prs.ps;

            gf = new GACForm();

            gf.GetAssemblies();
            gf.Text = "Reference manager - " + prs.ps.Name;

            ArrayList L = pp.GetItems("Reference", false);

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

        private NewProjectForm npf { get; set; }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();

            

            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = vs.solutionFileName;

            msbuilds.MSBuildFile = sf;

            msbuilds.build = msbuilder_alls.Build.rebuild;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);
        }

        private void NewProjectLoad()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

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
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            _eo.LoadFile(vs.solutionFileName, null, null, null);
        }

        private void viewProjectFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            VSProject project = prs.ps;

            string file = project.FileName;

            _eo.LoadFile(file, null, null, null);
        }

        private OpenFileDialog ofd { get; set; }

        private void existingProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

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
                    TreeNode oldselectednode = treeView.SelectedNode;

                    CreateView_Solution.ProjectItemInfo pp = node.Tag as CreateView_Solution.ProjectItemInfo;

                    VSSolution vs = GetVSSolution();
                    if (vs != null)
                    {
                        VSProject vp = vs.GetProjectbyName(node.Text);

                        if (vp != null)
                        {
                            context1.Show(treeView, p);
                            return;
                        }
                    }



                    if (node.Parent == null)
                    {
                        context2.Show(treeView, p);
                        return;
                    }
                    else if (node.Parent.Text == "Reference")
                    {
                        context3.Show(treeView, p);
                        return;
                    }
                    else if (node.ImageKey == "Folder_6222")
                    {
                        context4.Show(treeView, p);
                        return;
                    }

                    if (pp != null)
                    {
                        if (pp.IsItem() == true)
                        {
                            context6.Show(treeView, p);
                            treeView.Refresh();
                            return;
                        }
                    }
                    else
                    {
                        context6.Show(treeView, p);
                        treeView.Refresh();
                        return;
                    }

                    context1.Show(treeView, p);

                    // Highlight the selected node.
                    treeView.SelectedNode = oldselectednode;
                    oldselectednode = null;

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
                    context5.Show(treeView2, p);


                    treeView.Refresh();
                }
            }
        }

        private NewProjectItem npn { get; set; }

        private void addNewItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

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
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            vs.RemoveProject(pp.FileName);

            reload_solution(file);
        }

        private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

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
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

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
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            pp.RemoveReference(prs.psi);

            reload_solution(file);
        }

        private void viewInFileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pr = prs.ps;

            if (_eo.tw == null)
                return;

            VSProjectItem pp = prs.psi;

            string folder = Path.GetDirectoryName(pp.fileName);
        }

        private DependencyForms dfs { get; set; }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pr = prs.ps;

            dfs = new DependencyForms();
            dfs.LoadProjects(vs);
            dfs.ShowDialog();

            //reload_solution(vs.solutionFileName);
        }

        private OptionsForms opsf { get; set; }

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

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;
            ImageList img = CreateView_Solution.CreateImageList();
            _cc.dc = VSProject.dc;// CreateView_Solution.GetDC();
            treeView2.ImageList = img;
            img.TransparentColor = Color.Fuchsia;
            //treeView2.ImageList.TransparentColor = Color.Fuchsia;
            //treeView2.ImageList.ColorDepth = ColorDepth.Depth24Bit;

            treeView2.Nodes.Clear();
            workerTypeViewerDelegate w = workerTypeViewer;
            w.BeginInvoke(vs, null, null);
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            _sv.CreateNodeDict(_sv._SolutionTreeView);

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

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

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

            if (dict.ContainsKey(text))
            {
                Dictionary<string, object> d = dict[text] as Dictionary<string, object>;

                string files = d["FullFileName"] as string;

                _eo.LoadFile(files, null, null, null);

                if (_sv.dic != null)
                {
                    if (_sv.dic.ContainsKey(files) == false)
                        return;

                    TreeNode nodes = _sv.dic[files];

                    nodes.EnsureVisible();

                    _sv._SolutionTreeView.SelectedNode = nodes;
                }
            }
        }

        public void OpenFile(string file, string line)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, pp);

            int s = Convert.ToInt32(line);

            scr.SelectText(s, 0);
        }

        public void OpenFile(string file, string line, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, pp);

            int s = Convert.ToInt32(line);

            scr.SelectText(s, length);
        }

        public void OpenFileXY(string file, string X, string Y, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            // MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, pp);

            int x = Convert.ToInt32(X);

            int y = Convert.ToInt32(Y);



            scr.SelectTextXY(x, y, length);
        }

        public void OpenFileLine(string file, string line, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            //MessageBox.Show("File " + file + " at line " + line);

            if (File.Exists(file) == false)
                return;

            _eo.LoadFile(file, pp);

            int s = Convert.ToInt32(line);

            scr.SelectTextLine(s, length);
        }
        public void OpenFileLineHighlight(string file, string line, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            //MessageBox.Show("File " + file + " at line " + line);

            if (File.Exists(file) == false)
                return;

            _eo.LoadFile(file, pp);

            int s = Convert.ToInt32(line);

            scr.BeginInvoke(new Action(() => { scr.SelectTextLine(s, length, true); }));
        }
        public void OpenFileFromProject(string files)
        {
            _sv.CreateNodeDict(_sv._SolutionTreeView);

            if (_sv.dic != null)
            {
                if (_sv.dic.ContainsKey(files) == false)
                    return;

                TreeNode nodes = _sv.dic[files];

                nodes.EnsureVisible();

                _sv._SolutionTreeView.SelectedNode = nodes;

                CreateView_Solution.ProjectItemInfo pr0 = nodes.Tag as CreateView_Solution.ProjectItemInfo;

                ImageList img = CreateView_Solution.CreateImageList();
                Dictionary<string, string> dc = CreateView_Solution.GetDC();

                _eo.LoadFile(files, dc, img, pr0.ps);
            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            VSProject pp = node.Tag as VSProject;

            if (pp != null)
            {
                MessageBox.Show("Project ready for parsing...");

                VSProvider.Parser.BuildMaps(pp, node);

                return;
            }

            ArrayList L = node.Tag as ArrayList;

            if (node.Parent != null)
                if (node.Parent.Text == "ProjectReferences")
                {
                    VSProject p = node.Parent.Parent.Tag as VSProject;

                    if (p == null)
                        return;

                    Dictionary<string, string> d = GACForm.dicts;

                    string asm = node.Text;

                    string[] asms = asm.Split(",".ToCharArray());

                    asm = asms[0];

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

                    _cc.AddTypes(node, file);
                }

            if (L == null)
            {
                //AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> T = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

                //Type TT = T.PropertyObject.GetType();

                //Type B = TT.BaseType;

                return;
            }

            if (treeView4.ImageList == null)
            {
                ImageList img = CreateView_Solution.CreateImageList();
                img.TransparentColor = Color.Fuchsia;
                treeView4.ImageList = img;
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

            VSProject project = prs.ps;

            if (project == null)
                return;

            //msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            //workerBuildDelegate b = build_project;
            //b.BeginInvoke(p.FileName, msbuilds, null, null);


            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();



            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.build;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = project.FileName;

            msbuilds.MSBuildFile = sf;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);



        }

        private void AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            workerSelectDelegate s = AfterSelects;
            s.BeginInvoke(node, null, null);
        }

        private void AfterSelects(TreeNode node)
        {
            if (Control.MouseButtons == MouseButtons.Right)
                return;

            nav.Add(node);

            string s = node.Text;


            string file = "";

            if (node.Tag == null)
            {
                this.BeginInvoke(new Action(() =>
                {
                    pgp.SelectedObject = null;
                    ExpandGroup(pgp, "Projects");
                }));

                return;
            }

            CreateView_Solution.ProjectItemInfo pr0 = null;

            if (node.Tag.GetType() != typeof(CreateView_Solution.ProjectItemInfo))
                return;

            pr0 = node.Tag as CreateView_Solution.ProjectItemInfo;

            //if (pgp != null)
            this.BeginInvoke(new Action(() =>
            {
                pgp.SelectedObject = pr0;
                ExpandGroup(pgp, "Projects");
            }));

            if (pr0.psi == null)
                if (pr0.mapper != null)
                {
                    file = _sv.mapper.classname;

                    _sv.projectmapper(file, node, false);

                    return;
                }

            VSProject pp = pr0.ps;

            if (pr0.psi != null)

            {
                VSProjectItem pi = pr0.psi;

                file = pi.fileName;

                if (file == null)
                    if (pp != null)
                        file = Path.GetDirectoryName(pp.FileName) + "\\" + pi.Include;

                if (node.Nodes.Count >= 1)
                {
                    if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
                    {
                        return;
                    }
                    else
                    {
                        _sv.projectmapper(file, node, false);
                    }
                }
            }
            else
            {
                CreateView_Solution.ProjectItemInfo pr = pr0;

                return;
            }

            //if (pgp != null)
            //    this.BeginInvoke(new Action(() =>
            //    {
            //        pgp.SelectedObject = pr0;
            //        ExpandGroup(pgp, "Projects");
            //    }));
        }

        private static void ExpandGroup(PropertyGrid propertyGrid, string groupName)
        {
            GridItem root = propertyGrid.SelectedGridItem;

            if (root == null)
                return;
            //Get the parent
            while (root.Parent != null)
                root = root.Parent;

            if (root != null)
            {
                foreach (GridItem g in root.GridItems)
                {
                    if (g.GridItemType == GridItemType.Category && g.Label == groupName)
                    {
                        g.Expanded = true;

                        foreach (GridItem gg in g.GridItems)
                            gg.Expanded = true;
                    }
                }
            }
        }



        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null)
                return;

            CreateView_Solution.ProjectItemInfo prs = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (prs == null)
                return;

            VSProject project = prs.ps;

            if (project == null)
                return;

            //msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            //workerBuildDelegate b = build_project;
            //b.BeginInvoke(p.FileName, msbuilds, null, null);


            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();



            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.clean;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = project.FileName;

            msbuilds.MSBuildFile = sf;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);

        }

        public VSSolution GetVSSolution()
        {
            return _sv._SolutionTreeView.Tag as VSSolution;
        }

        public VSProject GetVSProject()
        {
            TreeNode node = _sv._SolutionTreeView.SelectedNode;
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

            if (node.Parent != null)

                while (node.Parent != null)
                    node = node.Parent;

            VSProject p = node.Tag as VSProject;

            return p;
        }

        //MSBuild.exe sample.sln /t:Rebuild /p:Configuration= Release; Platform=Any CPU
        // MSBuild.exe sample.sln /t:Build /p:Configuration= Release; Platform=Any CPU
        //MSBuild.exe sample.sln /t:Clean /p:Configuration= Release; Platform=Any CPU

        private void rebuildAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;
            if (vs == null)
                return;

 
            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();
            workerBuildDelegate b = build_alls;
            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);

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
            if (rw == null)
                LoadRW("Find Replace", true);
            else if (rw.Visible == true)
            {
                rw.Hide();

                findAndReplaceToolStripMenuItem.Checked = false;
            }
            else
            {
                rw.Show();
                rw.Show(dock, DockState.DockBottom);
                findAndReplaceToolStripMenuItem.Checked = true;
            }
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

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

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

            string name = p.GetProjectExec(p.FileName);

            string folder = Path.GetDirectoryName(name);

            string dirs = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(folder);

            string type = bg.GetTypeFullName();

            string c = "";//DecompilerForm.LoadLibrary4Method( p.GetProjectExec(p.FileName), bg.GetTypeFullName());

            Directory.SetCurrentDirectory(dirs);

            _eo.LoadFile(c, type + ".cs");
        }

        private void typeInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView2.SelectedNode;

            if (node == null)
                return;

            string text = node.Text;

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

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

            _eo.LoadFile(c, bg.Title);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
        }

        private void ExplorerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (scr == null)
                return;
            scr.Invoke(new Action(() => { scr.SaveSolution(); }));

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            dock.SaveAsXml(configFile);
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
            _sv._SolutionTreeView.Nodes.Clear();
            treeView2.Nodes.Clear();
            ClearOutput();
        }

        public void SolutionClose()
        {
            if (scr != null)
                scr.CloseAll();
            if (_sv != null)
                _sv._SolutionTreeView.Nodes.Clear();
            treeView2.Nodes.Clear();
            ClearOutput();
        }

        private void solutionExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (se == null)
                return;

            if (se.Visible == true)
            {
                se.Hide();
                solutionExplorerToolStripMenuItem.Checked = false;
            }
            else
            {
                se.Show();
                se.Show(dock, DockState.DockRight);
                solutionExplorerToolStripMenuItem.Checked = true;
            }
        }

        private void recentProjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (re.Visible == true)
            {
                re.Hide();
                recentProjectsToolStripMenuItem.Checked = false;
            }
            else
            {
                re.Show();
                recentProjectsToolStripMenuItem.Checked = true;
            }
        }

        private void fileExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fd == null)
                LoadFD("File Explorer", false);
            if (fd.Visible == true)
            {
                fd.Hide();
                fileExplorerToolStripMenuItem.Checked = false;
            }
            else
            {
                fd.Show(dock, DockState.DockRight);
                fileExplorerToolStripMenuItem.Checked = true;
            }
        }

        private void outputWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ows.Visible == true)
            {
                ows.Hide();
                outputWindowToolStripMenuItem.Checked = false;
            }
            else
            {
                ows.Show();
                outputWindowToolStripMenuItem.Checked = true;
            }
        }

        private void classViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cd.Visible == true)
            {
                cd.Hide();
                classViewToolStripMenuItem.Checked = false;
            }
            else
            {
                cd.Show();
                classViewToolStripMenuItem.Checked = true;
            }
        }

        private void ExplorerForms_Load(object sender, EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            Opacity = 0;

            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

            if (File.Exists(configFile))
                dock.LoadFromXml(configFile, _deserializeDockContent);
            else LoadDefaults();

            CheckStates();




            Settings s = CodeEditorControl.settings;

            if (s.ResumeAtStartup == "yes")
            {
                ResumeAtStartUp();

                if (splashForm != null)
                {
                    splashForm.Show();
                }
            }
            else
            {
                if (splashForm != null)
                {
                    splashForm.Hide();
                    splashForm = null;
                }

                this.Show();
                Visible = true;
                Opacity = 100;
                this.Show();
            }
        }

        public void CheckStates()
        {
            CheckState(solutionExplorerToolStripMenuItem, se);
            //  CheckState(outputWindowToolStripMenuItem, ow);
            CheckState(classViewToolStripMenuItem, cd);
            CheckState(fileExplorerToolStripMenuItem, fd);
            CheckState(recentProjectsToolStripMenuItem, re);
            CheckState(propertyGridToolStripMenuItem, pw);
        }

        public void CheckState(ToolStripMenuItem s, ToolWindow w)
        {
            if (w.IsHidden == false)
                s.Checked = true;
            else s.Checked = false;
        }

        private void propertyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pw == null)
                LoadERW("PropertyGrid", false);
            else if (pw.Visible == true)
            {
                pw.Hide();
                propertyGridToolStripMenuItem.Checked = false;
            }
            else
            {
                pw.Show(dock, DockState.DockRight);
                propertyGridToolStripMenuItem.Checked = true;
            }
        }

        private ExportTemplateForm etf { get; set; }

        private void exportTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = GetVSSolution();

            etf = new ExportTemplateForm();

            etf.LoadSolution(vs);

            etf.ShowDialog();
        }

        // Create an array of ToolStripMenuItems
        private ToolStripMenuItem[] _col = null;

        private void CreateMenuBasedOnArray()
        {
            MenuStrip strip = new MenuStrip();

            //ToolStripMenuItem fileItem = new ToolStripMenuItem("&File");

            CreateRecentProjects();

            // Create menu item containing icon and a collection
            recentProjectsAndSolutionsToolStripMenuItem.DropDownItems.Clear();
            if(_col != null)
            recentProjectsAndSolutionsToolStripMenuItem.DropDownItems.AddRange(_col);
        }

        private ToolStripMenuItem[] _wnds = null;

        public void CreateRecentProjects()
        {
            ArrayList L = new ArrayList();

            foreach (TreeNode node in treeView3.Nodes)
            {
                try
                {
                    string file = Path.GetFileNameWithoutExtension(node.Text);
                    L.Add(file);
                }
                catch (Exception e) { }
            }

            if (L.Count <= 0)
                return;

            int i = 0;

            int N = L.Count;

            _col = new ToolStripMenuItem[N];

            foreach (string file in L)
            {
                ToolStripMenuItem m = new ToolStripMenuItem();
                m.Click += new EventHandler(recentfileselected);
                m.Text = file;

                _col[i] = m;

                i++;
            }
        }

        public void CreateRecentWindows(ArrayList L)
        {
            if (L.Count <= 0)
                return;

            int i = 0;

            int N = L.Count;

            _wnds = new ToolStripMenuItem[N];

            foreach (string file in L)
            {
                string f = Path.GetFileName(file);

                ToolStripMenuItem m = new ToolStripMenuItem();
                //m.Click += new EventHandler(recentfileselected);
                m.Text = f;

                _wnds[i] = m;

                i++;
            }

            wINDOWToolStripMenuItem.DropDownItems.AddRange(_wnds);
        }

        public string GetFileFromName(string name)
        {
            string file = "";

            foreach (TreeNode node in treeView3.Nodes)
            {
                string s = node.Text;

                if (s.Contains(name))
                    return s;
            }

            return file;
        }

        public void recentfileselected(object sender, EventArgs e)
        {
            ToolStripMenuItem m = sender as ToolStripMenuItem;
            if (m == null)
                return;
            string name = m.Text;
            string file = GetFileFromName(name);
            if (file == "")
                return;

            scr.ClearHistory();

            ClearOutput();

            string recent = file;

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        private void newProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddNewProject(false);
        }

        public void AddNewProjectItem()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            DialogResult r = npn.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            string pf = npn.GetProjectFolder();

            string SubType = npn.GetSubType();

            ArrayList L = npn.GetProjectFiles();

            string folds = Path.GetDirectoryName(pp.FileName);// Path.GetDirectoryName(pr);

            if (SubType == "WindowsForm" || SubType == "Resource")

                pp.AddFormItem(L, "");
            else
                pp.SetCompileItems(L);

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            CopyAll(project, folder);

            reload_solution(file);
        }

        private void AddNewProject(bool addproject)
        {
            npf = new NewProjectForm();
            npf.LoadAddProject();
            if (addproject == true)
                npf.SetProjectFolder("sln");
            DialogResult r = npf.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            //CloseCurrentSolution();

            string bs = AppDomain.CurrentDomain.BaseDirectory + "TemplateProjects";

            

            string file = npf.GetSolutionFile();

            scr.ClearHistory();

            ClearOutput();

            SolutionClose();

            string recent = file;

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;

            w.Invoke(recent);


            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            // string file = vs.solutionFileName;

            string pn = npf.GetProjectName();

            string pf = npf.GetProjectFolder();

            string pr = npf.GetProjectFile();

            string prf = pr.Replace(bs + "\\","");

            string prc = pn.Replace(bs + "\\", "");

            string prs = Path.GetFileName(pr);

            string folds = Path.GetDirectoryName(pr);

            string[] dd = folds.Split("\\".ToCharArray());

            string fds = dd[dd.Length - 1];

            //file = file + "s";

            string content = File.ReadAllText(file);

            int p = content.IndexOf("Global");

            string dir = fds;// "LinkerProjectProject";

            dir = pn;

            string proj = prs;// "LinkerProjectProject.csproj";

            string projs = prs;

            proj = pn + ".csproj";

            string pb = Path.GetDirectoryName(dir) + "\\" + Path.GetFileName(proj);

            Guid guid = Guid.NewGuid();

            //string pp = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + dir + "\",\"" + pb + "\"," + "\"{" + guid.ToString() + "}\"\nEndProject\n\n\t";

            string pp = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + prc + "\",\"" + prf + "\"," + "\"{" + guid.ToString() + "}\"\nEndProject\n\n\t";

            content = content.Insert(p, pp);

            File.WriteAllText(file, content);

            DirectoryInfo project = new DirectoryInfo(folds);

            DirectoryInfo folder = new DirectoryInfo(sln + "\\" + /*pn*/fds);

            pn = fds;


            CopyAll(project, folder);


            string tempfile = sln + "\\" + pn + "\\" + Path.GetFileName(projs);

            string projectfile = sln + "\\" + pn + "\\" +Path.GetFileName(proj);

            File.Move(tempfile, projectfile);

            reload_solution(file);
        }

        public CreateView_Solution.ProjectItemInfo GetProjectInfo()
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null)
                return null;

            if (node.Tag == null)
                return null;

            CreateView_Solution.ProjectItemInfo prs = node.Tag as CreateView_Solution.ProjectItemInfo;

            return prs;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;

            ClipboardForm c = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                c.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                c.LoadTreeNode(n);
            }
            c.ShowDialog();

            // ReloadVSProject();
        }

        public VSProjectItem bc { get; set; }

        public TreeNode ncp { get; set; }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                bb.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                bb.LoadTreeNode(n);
            }

            bb.ShowDialog();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            bb.DeleteClipboard(node);
            bb.ShowDialog();

            ReloadVSProject();
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewProjectItem();
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            VSProject p = GetVSProject();

            CreateView_Solution.ProjectItemInfo prs = GetProjectInfo();

            if (prs == null)
                return;

            //if (bc == null)
            //    return;

            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;

            treeView1.SelectedNode = null;

            ClipboardForm bb = new ClipboardForm();
            bb.ReloadClipboard(node);
            bb.ShowDialog();

            ReloadVSProject();

            treeView1.SelectedNode = node;
        }

        public void ReloadVSProject()
        {
            _sv._SolutionTreeView.SuspendLayout();

            _sv._SolutionTreeView.BeginUpdate();

            AfterSelectActivate(false);

            VSProject pp = GetVSProject();

            VSSolution vs = GetVSSolution();

            TreeView bbb = _sv.update_project(vs, pp);

            TreeNode cc = bbb.Nodes[0];

            //TreeNode first = sv._SolutionTreeView.Nodes[0];

            TreeNode nodes = _sv.GetProjectNodes(vs, pp);

            //TreeNode nc = sv.FindItem(nodes, ps);

            if (nodes == null)
            {
                _sv._SolutionTreeView.ResumeLayout();



                AfterSelectActivate(true);

                return;
            }

            ArrayList N = new ArrayList();

            foreach (TreeNode n in nodes.Nodes)
                N.Add(n);
            foreach (TreeNode n in N)
                nodes.Nodes.Remove(n);

            ArrayList L = new ArrayList();
            foreach (TreeNode ns in cc.Nodes)
            {
                L.Add(ns);
            }



            foreach (TreeNode ns in L)
            {
                cc.Nodes.Remove(ns);
                nodes.Nodes.Add(ns);
                cc.Expand();
            }
            _sv._SolutionTreeView.EndUpdate();

            _sv._SolutionTreeView.ResumeLayout();

            _sv._SolutionTreeView.Refresh();

            AfterSelectActivate(true);
        }

        public ArrayList GetSubNodes(TreeNode node)
        {
            ArrayList L = new ArrayList();

            foreach (TreeNode nodes in node.Nodes)
            {
                CreateView_Solution.ProjectItemInfo p = nodes.Tag as CreateView_Solution.ProjectItemInfo;

                if (p != null)
                    L.Add(p);
            }

            return L;
        }

        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            LoadConfigurationManager();
        }

        private void aboutApplicationStudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutApplicationForm aaf = new AboutApplicationForm();
            aaf.ShowDialog();
        }
        
        private void viewHelpFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewHelpFiles();

            //System.Windows.Forms.Help.ShowHelp(this, "CHM\\index.chm");
        }

        public WebForm wf { get; set; }

        public void ViewHelpFiles()
        {
            if (_sv == null)
                return;
            if (_sv.eo == null)
                return;

            string s = AppDomain.CurrentDomain.BaseDirectory;

            wf = _sv.eo.OpenWebForm("file://" + s + "Documentation\\index.html");

            System.Threading.Tasks.Task.Run(new Action(() =>
            {

                this.Invoke(new Action(() => { wf.wb.Document.ExecCommand("fontSize", false, "4pt"); }));
               
            }));

            //wf.wb.Navigate("file://" + s + "Documentation\\Documentation.chm");

            //LoadListTab(df);
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            //VSSolution vs = GetVSSolution();

            //if (vs == null)
            //    return;
            //msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();
            //workerBuildDelegate b = build_alls;
            //b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);

            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();



            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.build;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = vs.solutionFileName;

            msbuilds.MSBuildFile = sf;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);




        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            //VSSolution vs = GetVSSolution();
            //foreach (VSProject p in vs.Projects)
            //{
            //    p.CleanProject();
            //}
            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();



            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.clean;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = vs.solutionFileName;

            msbuilds.MSBuildFile = sf;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);


        }

        private void buildSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null)
                return;

            CreateView_Solution.ProjectItemInfo prs = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (prs == null)
                return;

            VSProject project = prs.ps;

            if (project == null)
                return;

            //msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            //workerBuildDelegate b = build_project;
            //b.BeginInvoke(p.FileName, msbuilds, null, null);


            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();

            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.rebuild;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            string sf = project.FileName;

            msbuilds.MSBuildFile = sf;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);

        }

        private void formToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFormTemplate();
        }

        public void AddFormTemplate()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            VSProjectItem psi = prs.psi;

            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            //DialogResult r = npn.ShowDialog();
            //if (r != System.Windows.Forms.DialogResult.OK)
            //    return;

            string pf = npn.GetProjectFolder("WindowsForm");

            string SubType = "Form";

            ArrayList L = npn.GetProjectFiles("WindowsForm");

            string folds = Path.GetDirectoryName(pp.FileName);

            if (psi != null)
            {
                if (psi.ItemType == "Folder")
                {
                    folds = folds + "\\" + psi.Include;

                    ArrayList LL = new ArrayList();
                    foreach (string s in L)
                    {
                        string name = psi.Include + "\\" + s;
                        LL.Add(name);
                    }
                    L = LL;
                }
            }

            if (SubType == "Form")

                pp.AddFormItem(L, "");

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            if (folder.Exists == false)
                folder.Create();

            CopyAll(project, folder);

            reload_solution(file);
        }

        public void RemoveProjectItem()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project info associated with this item. Exiting...");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            VSProjectItem ps = prs.psi;

            if (ps == null)
                return;

            MessageBox.Show("Project item will be removed...");

            ArrayList L = new ArrayList();

            foreach (TreeNode ns in node.Nodes)
            {
                string s = ns.Text;
                L.Add(s);
            }

            pp.RemoveItems(L);

            reload_solution(vs.solutionFileName);
        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
        }

        private void newFolderToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project info associated with this item. Exiting...");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            VSProjectItem ps = prs.psi;

            if (ps == null)
                return;

            DataForm df = new DataForm();
            DialogResult r = df.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            string folder = df.GetFolderName();

            pp.CreateFolder(folder, ps);

            TreeView bb = _sv.update_project(vs, pp);

            TreeNode cc = bb.Nodes[0];

            TreeNode first = _sv._SolutionTreeView.Nodes[0];

            TreeNode nodes = _sv.GetProjectNodes(vs, pp);

            TreeNode nc = _sv.FindItem(nodes, ps);

            if (nodes == null)
                return;

            nodes.Nodes.Clear();

            ArrayList L = new ArrayList();
            foreach (TreeNode ns in cc.Nodes)
            {
                L.Add(ns);
            }

            foreach (TreeNode ns in L)
            {
                cc.Nodes.Remove(ns);
                nodes.Nodes.Add(ns);
            }

            cc.Expand();

            nc.EnsureVisible();
        }

        private void windowsFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AddFormTemplate();

            AddFormItem2Folder();
        }

        private void toolStripMenuItem39_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;

            ClipboardForm c = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                c.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                c.LoadTreeNode(n);
            }

            c.ShowDialog();

            //ReloadVSProject();
        }

        //public void RenameFolder()
        //{
        //    VSProject p = GetVSProject();

        //    CreateView_Solution.ProjectItemInfo prs = GetProjectInfo();

        //    if (prs == null)
        //        return;

        //    VSProjectItem pp = prs.psi;

        //    if (pp == null)
        //        return;

        //    if (prs.psi.SubType != "Folder")
        //        return;

        //    MessageBox.Show("Folder will be renamed...");

        //    RenameForm rf = new RenameForm();
        //    DialogResult r = rf.ShowDialog();
        //    if (r != System.Windows.Forms.DialogResult.OK)
        //        return;
        //    string s = rf.GetFolderName();

        //    //string v = bc.Include.Replace(".cs", "");

        //    //string s = v + " " + "Copy.cs";

        //    string folder = p.GetProjectFolder();

        //    string source = folder + "\\" + pp.Include;

        //    string newfolder = folder + "\\" + s;

        //    if (Directory.Exists(newfolder) == false)
        //        Directory.CreateDirectory(newfolder);

        //    DirectoryInfo src = new DirectoryInfo(source);

        //    DirectoryInfo target = new DirectoryInfo(newfolder);

        //    CopyAll(src, target);

        //    p.RenameItem(pp.ItemType, pp.Include, s);

        //    reload_solution(prs.vs.solutionFileName);
        //}

        public void RenameItem()
        {
            VSProject p = GetVSProject();

            CreateView_Solution.ProjectItemInfo prs = GetProjectInfo();

            if (prs == null)
                return;

            VSProjectItem pp = prs.psi;

            if (pp == null)
                return;

            //if (prs.psi.SubType != "Folder")
            //    return;

            MessageBox.Show("Item will be renamed...");

            RenameForm rf = new RenameForm();
            DialogResult r = rf.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;
            string s = rf.GetFolderName();

            string folder = p.GetProjectFolder();

            string source = folder + "\\" + pp.Include;

            string newfile = folder + "\\" + s;

            if (File.Exists(newfile) == true)
            {
                MessageBox.Show("File exists. Exiting...");
                return;
            }
            File.Copy(source, newfile);

            p.RenameItem(pp.ItemType, pp.Include, s);

            reload_solution(prs.vs.solutionFileName);
        }

        public void AddNewProjectItem2Folder()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            if (prs.psi == null)
                return;

            VSProjectItem psi = prs.psi;

            if (psi.SubType != "Folder")
            {
                MessageBox.Show("No folder selected. Exiting...");
                return;
            }

            string Folder = psi.Include;

            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            DialogResult r = npn.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            string pf = npn.GetProjectFolder();

            string SubType = npn.GetSubType();

            ArrayList L = npn.GetProjectFiles();

            string folds = Path.GetDirectoryName(pp.FileName) + "\\" + Folder;

            if (SubType == "WindowsForm")

                pp.AddFormItem(L, Folder);

            //pp.SetCompileItems(L);

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            CopyAll(project, folder);

            reload_solution(file);
        }

        private void newItemToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddNewProjectItem2Folder();
        }

        public void AddFormItem2Folder()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            if (prs.psi == null)
                return;

            VSProjectItem psi = prs.psi;

            if (psi.SubType != "Folder")
            {
                MessageBox.Show("No folder selected. Exiting...");
                return;
            }

            string Folder = psi.Include;

            npn = new NewProjectItem();
            npn.SetProjectFolder(sln);
            //DialogResult r = npn.ShowDialog();
            //if (r != System.Windows.Forms.DialogResult.OK)
            //    return;

            //string pf = npn.GetProjectFolder("WindowsForm");

            //string SubType = "Form";

            //ArrayList L = npn.GetProjectFiles("WindowsForm");

            //string folds = Path.GetDirectoryName(pp.FileName);

            string pf = npn.GetProjectFolder("WindowsForm");

            string SubType = "Form";

            ArrayList L = npn.GetProjectFiles("WindowsForm");

            string folds = Path.GetDirectoryName(pp.FileName) + "\\" + Folder;

            if (SubType == "Form")

                pp.AddFormItem(L, Folder);

            //pp.SetCompileItems(L);

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            CopyAll(project, folder);

            //reload_solution(file);

            ReloadVSProject();
        }

        private void toolStripMenuItem37_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                bb.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                bb.LoadTreeNode(n);
            }
            bb.ShowDialog();
        }

        private void runTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            workerFunctionTestDelegate w = TestForm;
            w.BeginInvoke("recent", null, null);
        }

        ///////////////////////////////
        // copy items to the project item (folder)
        /// <summary>
        /// ///////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem43_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            bb.ReloadClipboard(node);
            bb.ShowDialog();

            ReloadVSProject();
        }

        private void unloadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSProject p = GetVSProject();

            if (p == null)
                return;
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            AddNewFolder();
        }

        public void AddNewFolder()
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project info associated with this item. Exiting...");
                return;
            }

            TreeNode node = _eo.cvs.getactivenode();

            if (prs.ps == null)
                return;

            VSProject pp = prs.ps;

            //VSProjectItem ps = prs.psi;

            //if (ps == null)
            //    return;

            DataForm df = new DataForm();
            DialogResult r = df.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            string folder = df.GetFolderName();

            pp.CreateFolder(folder, null);

            TreeView bb = _sv.update_project(vs, pp);

            TreeNode cc = bb.Nodes[0];

            TreeNode first = _sv._SolutionTreeView.Nodes[0];

            TreeNode nodes = _sv.GetProjectNodes(vs, pp);

            //TreeNode nc = sv.FindItem(nodes, ps);

            if (nodes == null)
                return;

            nodes.Nodes.Clear();

            ArrayList L = new ArrayList();
            foreach (TreeNode ns in cc.Nodes)
            {
                L.Add(ns);
            }

            foreach (TreeNode ns in L)
            {
                cc.Nodes.Remove(ns);
                nodes.Nodes.Add(ns);
            }

            cc.Expand();

            //nc.EnsureVisible();
        }

        private static Testing_Suite tsf { get; set; }

        private void testSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tsf == null)
                tsf = new Testing_Suite();
            tsf.Show();
        }

        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            TreeNode node = _sv._SolutionTreeView.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            bb.DeleteClipboard(node);
            bb.ShowDialog();

            ReloadVSProject();
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
            TreeNode node = _sv._SolutionTreeView.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                bb.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                bb.LoadTreeNode(n);
            }

            bb.ShowDialog();
        }

        private void outputWindowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ows == null) 
                LoadORW("Outputs", false);
            if (ows.Visible == true)
            {
                ows.Hide();
                outputWindowToolStripMenuItem.Checked = false;
            }
            else
            {
                ows.Show(dock, DockState.DockBottom);
                outputWindowToolStripMenuItem.Checked = true;
            }


            


        }

        private void errorListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ews == null)
                LoadERW("Error List", false);
            else if (ews.Visible == true)
            {
                ews.Hide();
                errorListToolStripMenuItem.Checked = false;
            }
            else
            {
                ews.Show(dock, DockState.DockBottom);
                //ews.Show();
                errorListToolStripMenuItem.Checked = true;
            }
        }

        private void findResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStrip4_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void projectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //VSSolution vs = GetVSSolution();// sv._SolutionTreeView.Tag as VSSolution;

            //sv._SolutionTreeView.BeginInvoke(new Action(() => { sv._ddButton.DropDownItems.Clear(); sv._SolutionTreeView.Nodes[0].Expand(); sv.LoadPlatforms(vs); if (vs != null) sv.LoadProjects(vs); }));

            //  WinExplorer.UI.ProjectPropertyForm ppf = new WinExplorer.UI.ProjectPropertyForm();
            //  ppf.Show();

            WinExplorer.UI.ListViewOwnerDraw od = new ListViewOwnerDraw();
            od.Show();
        }

        private MSBuildProject mbp { get; set; }

        private void mSBuildProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = GetVSSolution();
            VSProject vp = GetVSProject();

            mbp = new MSBuildProject();
            mbp.vs = vs;
            mbp.vp = vp;
            mbp.LoadProject();
            mbp.Show();
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            if (vs == null)
                return;

            cmf = new ConfigurationManagerForm();
            cmf.Text = "Solution " + vs.Name + " Property Pages";
            cmf.vs = GetVSSolution();
            cmf.UnloadConfigPanel();

            cmf.LoadConfigs(vs);

            cmf.LoadPage_MSBuild();

            cmf.ShowDialog();
        }

        private void findResults2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rws2 == null)
                LoadRF2("Find Results 2", false);
            else if (rws2.Visible == true)
            {
                rws2.Hide();
                findResults1ToolStripMenuItem.Checked = false;
            }
            else
            {
                //rws.Show();
                rws2.Show(dock, DockState.DockBottom);
                findResults1ToolStripMenuItem.Checked = true;
            }
        }

        private void findResults1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rws == null)
                LoadRF("Find Results 1", false);
            else if (rws.Visible == true)
            {
                rws.Hide();
                findResultsToolStripMenuItem.Checked = false;
            }
            else
            {
                //rws.Show();
                rws.Show(dock, DockState.DockBottom);
                findResultsToolStripMenuItem.Checked = true;
            }
        }

        public delegate void CodeAnalysisThread(string s);

        private void analyzeSourceCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string settings = "Settings.StyleCop";
            // ProcessSolution(settings);
            CodeAnalysisThread a = ProcessSolution;
            IAsyncResult s = a.BeginInvoke(settings, null, null);
        }

        private void codeAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (aw == null)
                return;

            if (aw.Visible == true)
            {
                aw.Hide();
                codeAnalysisToolStripMenuItem.Checked = false;
            }
            else
            {
                if (dock.Contains(slf) == false)
                    aw.Show(dock, DockState.DockBottom);
                aw.Show();
                codeAnalysisToolStripMenuItem.Checked = true;
            }
        }

        private ArrayList Vs { get; set; }

        private void ProcessSolution(string settings)
        {
            VSSolution vs = null;
            this.Invoke(new Action(() => { vs = GetVSSolution(); }));
            VSProject vp = null;
            this.Invoke(new Action(() => { vp = GetVSProject(); }));

            Vs = new ArrayList();

            string projectPath = vs.solutionFileName;

            var console = new StyleCop.StyleCopConsole(settings, false, null, null, true);
            var project = new StyleCop.CodeProject(0, projectPath, new StyleCop.Configuration(null));

            foreach (VSProject p in vs.Projects)
            {
                foreach (string s in p.GetCompileItems())

                    if (File.Exists(s) == true)
                    {
                        console.Core.Environment.AddSourceCode(project, s, null);
                    }
            }

            //console.OutputGenerated += OnOutputGenerated;
            console.ViolationEncountered += OnViolationEncountered;
            // this.BeginInvoke(new Action(() => { console.Start(new[] { project }, true); }));
            console.Start(new[] { project }, true);
            //console.OutputGenerated -= OnOutputGenerated;
            console.ViolationEncountered -= OnViolationEncountered;

            slf.LoadLogItem(Vs);
        }

        private void OnOutputGenerated(object sender, StyleCop.OutputEventArgs e)
        {
            //System.Console.WriteLine(e.Output);
        }

        private int _encounteredViolations = 0;

        private void OnViolationEncountered(object sender, StyleCop.ViolationEventArgs e)
        {
            _encounteredViolations++;
            // WriteLineViolationMessage(string.Format("  Line {0}: {1} ({2})", e.LineNumber, e.Message,
            //     e.Violation.Rule.CheckId));
            Vs.Add(e);
            //slf.LoadLogItem(e);
        }

        private void WriteLineViolationMessage(string message)
        {
            //System.Console.ForegroundColor = ConsoleColor.DarkRed;
            //System.Console.WriteLine(message);
            //System.Console.ResetColor();
        }

        private void toolStripMenuItem69_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem72_Click(object sender, EventArgs e)
        {
        }

        private void outlineWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void projectSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
                return;

            string recent = ofd.FileName;

            if (scr != null)
                scr.ClearHistory();

            ClearOutput();

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private CustomizeForm cup { get; set; }

        private void custToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cup = new CustomizeForm();
            cup.ShowDialog();
        }

        private ExternalToolsForm etfs { get; set; }

        private void externalToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            etfs = new ExternalToolsForm();
            etfs.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, Proxy.OpenFileEventArgs p)
        {
            //Proxy.OpenFileEventArgs p = new Proxy.OpenFileEventArgs();
            //p.content = "";
            //scr.OpenDocuments("this", "p");
            scr.Proxy_OpenFile(this, p);
        }

        private void toolStripMenuItem63_Click(object sender, EventArgs e)
        {
            string web = "Start Page";
            scr.OpenWebForm(web, "https://www.nuget.org/packages");
        }

        private void projectToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AddNewProject(false);
        }

        private void openWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWithForm owf = new OpenWithForm();
            DialogResult r = owf.ShowDialog(this);
            if (r != DialogResult.OK)
                return;
        }

        private void newItemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem77_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            bb.ReloadClipboard(node);
            bb.ShowDialog();

            ReloadVSProject();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void vSExplorerTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSExplorerForm ve = new VSExplorerForm();
            ve.Show();
        }

        private void toolStripMenuItem51_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;

            ClipboardForm c = new ClipboardForm();
            if (_selectedNodes.Count > 0)
                c.LoadTreeNode(_selectedNodes);
            else
            {
                List<TreeNode> n = new List<TreeNode>();
                n.Add(node);
                c.LoadTreeNode(n);
            }
            c.ShowDialog();

            //ReloadVSProject();
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
        }

        private void nUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nt == null)
                return;

            if (nt.Visible == true)
            {
                nt.Hide();
                nUnitToolStripMenuItem.Checked = false;
            }
            else
            {
                if (dock.Contains(gunit) == false)
                    nt.Show(dock, DockState.DockRight);
                nt.Show();
                nUnitToolStripMenuItem.Checked = true;
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            _sv.hst.ReloadRecentSolutions();
        }

        private void toolStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            _sv.hst.remitemsall();

            _sv.hst.ReloadRecentSolutions();
        }

        private void openCMDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = _eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            TreeNode MainProjectNode = _eo.cvs.getactivenode();

            //MessageBox.Show("Main project CMD " + prs.ps.Name);

            if (_sv == null)
                return;
            if (_sv.eo == null)
                return;

            df = _sv.eo.OpenDocumentForm(prs.ps.Name);

            df.FileName = prs.ps.FileName;

            LoadCMD(df, df.FileName);
        }

        private void restoreNugetPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // < RestorePackages > true </ RestorePackages >

            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            vs.RestorePackages(true);
        }

        private void powerShellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSProject vp = GetVSProject();

            df = _sv.eo.OpenDocumentForm(vp.FileName);

            df.FileName = vp.FileName;

            LoadPS(df, df.FileName);
        }

        private void powerShellWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSProject vp = GetVSProject();

            df = _sv.eo.OpenDocumentForm(vp.FileName);

            df.FileName = vp.FileName;

            LoadPS(df, df.FileName);
        }
    }
}