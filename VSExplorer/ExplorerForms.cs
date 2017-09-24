using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.Scripting.ScriptControl;
using AIMS.Libraries.Scripting.ScriptControl.Properties;
using AvalonEdit.Editor;
using DockProject;
using GACProject;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Win32;
using NUnit.Engine;
using NUnit.Gui;
using NUnit.Gui.Model;
using NUnit.Gui.Presenters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using utils;
using VSParsers;
using VSProvider;
using WeifenLuo.WinFormsUI.Docking;
using WinExplorer.Environment;
using WinExplorer.Services.Extensions;
using WinExplorer.Services.NuGet;
using WinExplorer.UI;
using WinExplorer.UI.Views;
using xunit.runner;

namespace WinExplorer
{
    /// <summary>
    /// The Visual Explorer main window form.
    /// </summary>
    public partial class ExplorerForms : Form
    {
        private class DummyNode : TreeNode { }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
                                                string pszSubIdList);

        public static ExplorerForms ef { get; set; }

        public static ErrorListForm elf { get; set; }

        static public Dictionary<string, ToolStrip> regToolstrips { get; set; }

        /// <summary>
        /// Main Form Constructor
        /// </summary>
        public ExplorerForms()
        {
            ef = this;

            this.CreateHandle();

            #region Form Setup

            _components = new System.ComponentModel.Container();

            InitializeComponent();
            IntPtr Hicon = WinExplorers.ve.WindowsLogo_16x.GetHicon();
            this.Icon = Icon.FromHandle(Hicon);
            context1 = contextMenuStrip1;
            context2 = contextMenuStrip2;
            context3 = contextMenuStrip3;
            context4 = contextMenuStrip7;
            context5 = contextMenuStrip5;
            context6 = contextMenuStrip6;

            _components = new System.ComponentModel.Container();

            #endregion Form Setup
            
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

            #region Docking Panel

            dock = new DockPanel();

            dock.DocumentTabStripLocation = DocumentTabStripLocation.Top;

            dock.BackColor = Color.White;

            dock.Dock = DockStyle.Fill;

            dock.DocumentStyle = DocumentStyle.DockingWindow;

            dock.SupportDeeplyNestedContent = false;

            this.toolStripContainer1.ContentPanel.Controls.Add(dock);

            #endregion Docking Panel

            ScriptControl.LoadSettings();

            scr = new ScriptControl(this, dock);

            _eo.script = scr;

            string Theme = scr.GetTheme();

            if (Theme == "VS2012Light")
                dock.Theme = new VS2012LightTheme();
            else
                dock.Theme = new VS2013BlueTheme();

            scr.Dock = DockStyle.Fill;

            //if (scr.hst != null && scr.hst.Modules != null)
            //    Module.LoadFromString(scr.hst.Modules);
            //else
                Module.Initialize();

            Module.InitializeTemplates("Standards");

            _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);

            ImageList imgs = CreateView_Solution.CreateImageList();
            VSProject.dc = CreateView_Solution.GetDC();

            CodeEditorControl.AutoListImages = imgs;

            #region Components

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
            LoadWD("Watch", false);
            ResumeLayout();
            LoadBR("Breakpoints", false);
            ResumeLayout();
            LoadNU("NUnit", false);
            ResumeLayout();
            LoadXL("Xml Document", false);
            ResumeLayout();
            LoadTB("Toolbox", false);
            ResumeLayout();
            LoadSF("Server Explorer", false);
            LoadDocumentWindow(false);
            LoadDT("Data Sources", false);
            LoadQE("SQL Server Explorer", false);
            LoadTE("Test Explorer", false);
            LoadHW("References", false);
            LoadHC("Call Hierarchy", false);

            ResumeLayout();

            #endregion Components

            update_z_order(this);

            #region GUI Utils

            loadcurrentproject();

            loadtoolmenu(scr);

            loadcontmenu();

            #endregion GUI Utils

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

            //_sv._SolutionTreeView.MouseDown += new MouseEventHandler(treeView1_MouseDown);

            _sv._SolutionTreeView.MouseDoubleClick += _SolutionTreeView_MouseDoubleClick;

            // _sv._SolutionTreeView.KeyDown += _SolutionTreeView_KeyPress;

            //_selectedNodes = new List<TreeNode>();

            _sv.scr = scr;

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

            //CodeEditorControl.ParserDataChanged += CodeEditorControl_ParserDataChanged;

            update_z_order(this);

            splitContainer1.Visible = false;

            GACForm.GetAssembliesList("");

            if (Module.GetModule("Standards") == null)
                Module.Template("Standards");

            debuggers = new WinExplorers.Debuggers();

            regToolstrips = new Dictionary<string, ToolStrip>();

            regToolstrips.Add("Debug", debuggers.ts);

            regToolstrips.Add("Standard", _sv._mainToolStrip);

            //regToolstrips.Add("Standards", _sv.ns);

            Command_ChangeToolstrip(debuggers.ts, AnchorStyles.Top);

            statusStrip1.BackColor = Color.FromKnownColor(KnownColor.Highlight);

            ve = new Tools();

            ve.regToolstrips = regToolstrips;

            ve.modules = Module.modules;

            ToolStrips ves = ve.CreateToolStrips("Standards");

            ToolStripItem ge = ves.GetItem("Navigate Backward");
            ToolStripSplitButton gb = ge as ToolStripSplitButton;
            if (gb != null)
            {
                _sv._ddButton = gb;
            }

            appLoadedEvent += LoadBreakpoints;

            VSSolution.OpenReferences += VSSolution_OpenReferences;

            VSSolution.OpenCallHierarchy += VSSolution_OpenCallHierarchy;

        }

        private void VSSolution_OpenCallHierarchy(object sender, OpenReferenceEventArgs e)
        {
            if (hc == null)
                return;
            hc.Show(dock, DockState.DockBottom);

            EditorWindow.hierarchyViewer = hcc.dv;
        }

        private void VSSolution_OpenReferences(object sender, OpenReferenceEventArgs e)
        {
            if (hw == null)
                return;
            hw.Show(dock, DockState.DockBottom);

            EditorWindow.treeViewer = hwf.dv;

            TreeViewer.vs = GetVSSolution();
        }

        //protected override void WndProc(ref Message m)
        //{
        //    // WM_SYSCOMMAND
        //    if (m.Msg == 0x0112)
        //    {
        //        if (m.WParam == new IntPtr(0xF030) // Maximize event - SC_MAXIMIZE from Winuser.h
        //            || m.WParam == new IntPtr(0xF120)
        //            || m.WParam == new IntPtr(0xF020)) // Restore event - SC_RESTORE from Winuser.h
        //        {
        //            if (scr == null)
        //                return;

        //            scr.HideAndRedraw();
        //        }
        //    }
        //    base.WndProc(ref m);
        //    if (m.Msg == 0x0112)
        //    {
        //        if (m.WParam == new IntPtr(0xF030) // Maximize event - SC_MAXIMIZE from Winuser.h
        //            || m.WParam == new IntPtr(0xF120)
        //            || m.WParam == new IntPtr(0xF020)) // Restore event - SC_RESTORE from Winuser.h
        //        {
        //            if (scr == null)
        //                return;

        //            scr.ShowAndRedraw();
        //        }
        //    }
        //}
        public Tools ve { get; set; }

        public WinExplorers.Debuggers debuggers { get; set; }

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

            VSParsers.ProjectItemInfo pr0 = null;

            if (node.Tag.GetType() != typeof(VSParsers.ProjectItemInfo))
                return;

            pr0 = node.Tag as VSParsers.ProjectItemInfo;

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
                    if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode)) // if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
                    {
                        //return;
                    }
                    else
                    {
                        _sv.projectmapper(file, node, false);
                    }
                }
            }
            else
            {
                VSParsers.ProjectItemInfo pr = pr0;

                return;
            }

            if (file != null)
            {
                if (file.EndsWith(".cs") == true)
                    this.BeginInvoke(new Action(() => { _sv.projectmapper(file, node, false); }));
            }

            if (File.Exists(file) == true)
            {
                if (CodeEditorControl.AutoListImages == null)
                {
                    ImageList imgs = CreateView_Solution.CreateImageList();
                    VSProject.dc = CreateView_Solution.GetDC();

                    CodeEditorControl.AutoListImages = imgs;
                }


                AvalonDelegate w = Avalon;
                IAsyncResult r = w.BeginInvoke(file, null, null);

                this.BeginInvoke(new Action(() =>
                {
                    //_eo.LoadFile(file, pp);
                    scr.BeginInvoke(new Action(() =>
                    {
        //                scr.OpenDocuments(file, null);
                    }));
                }));
            }
        }

        public void Avalon(string file)
        {
            this.BeginInvoke(new Action(() =>
            {
                //_eo.LoadFile(file, pp);
                scr.BeginInvoke(new Action(() =>
                {
                    scr.OpenDocuments(file, GetVSSolution());
                }));
            }));
        }

        

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
            LoadWD("Watch", true);
            ResumeLayout();
        }

        public FindReplaceForm fr { get; set; }

        public ToolWindow rw { get; set; }

        public void LoadRW(string name, bool load)
        {
            if (rw == null)
            {
                rw = new ToolWindow();

                rw.Text = name;
                rw.FormBorderStyle = FormBorderStyle.None;
                rw.TabText = name;
                rw.TopLevel = true;
                rw.HideOnClose = true;

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
            }
            if (load)
                rw.Show(dock, DockState.DockLeft);
        }

        public ToolWindow ows { get; set; }

        public OutputForm ofm { get; set; }

        private RichTextBox rbb { get; set; }

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
            ews.FormBorderStyle = FormBorderStyle.None;
            ews.TabText = name;
            ews.TopLevel = true;
            ews.HideOnClose = true;

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

            elf = efs;
            CodeEditorControl.dg = efs.dg;
        }

        public FindResultsForm rfs { get; set; }

        public ToolWindow rws { get; set; }

        public void LoadRF(string name, bool load)
        {
            rws = new ToolWindow();

            rws.Text = name;
            rws.HideOnClose = true;
            //tw.Controls.Add(scr);
            rws.FormBorderStyle = FormBorderStyle.None;
            //ww.ControlBox = false;
            rws.TabText = name;
            rws.TopLevel = true;
            rws.HideOnClose = true;

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
            rws2.FormBorderStyle = FormBorderStyle.None;

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

        public ToolWindow qe { get; set; }
        public DatabaseInspectorForm qdtf { get; set; }

        public void LoadQE(string name, bool load)
        {
            qe = new ToolWindow();
            qe.Text = name;
            qe.HideOnClose = true;
            qe.FormBorderStyle = FormBorderStyle.None;
            qe.TabText = name;
            qe.TopLevel = false;

            if (qdtf == null)
            {
                qdtf = new DatabaseInspectorForm();
                qdtf.TopLevel = false;
                qdtf.FormBorderStyle = FormBorderStyle.None;
                qdtf.Dock = DockStyle.Fill;
                qdtf.Show();
            }

            qe.Controls.Add(qdtf);

            qe.types = "qe";

            if (load)
                qe.Show(dock, DockState.DockLeft);
        }
        public ToolWindow te { get; set; }
        public TreeViewer_WinformsHost tef { get; set; }

        public void LoadTE(string name, bool load)
        {
            te = new ToolWindow();
            te.Text = name;
            te.HideOnClose = true;
            te.FormBorderStyle = FormBorderStyle.None;
            te.TabText = name;
            te.TopLevel = false;

            if (tef == null)
            {
                tef = new TreeViewer_WinformsHost();
                tef.TopLevel = false;
                tef.FormBorderStyle = FormBorderStyle.None;
                tef.Dock = DockStyle.Fill;
                tef.Show();
            }

            te.Controls.Add(tef);

            te.types = "te";

            if (load)
                te.Show(dock, DockState.DockLeft);
        }
        public ToolWindow dt { get; set; }
        public DataSourceForm dtf { get; set; }

        public void LoadDT(string name, bool load)
        {
            dt = new ToolWindow();
            dt.Text = name;
            dt.HideOnClose = true;
            dt.FormBorderStyle = FormBorderStyle.None;
            dt.TabText = name;
            dt.TopLevel = false;

            if (dtf == null)
            {
                dtf = new DataSourceForm();
                dtf.TopLevel = false;
                dtf.FormBorderStyle = FormBorderStyle.None;
                dtf.Dock = DockStyle.Fill;
                dtf.Show();
            }

            dt.Controls.Add(dtf);

            dt.types = "dt";

            if (load)
                dt.Show(dock, DockState.DockLeft);
        }

        public ToolWindow xt { get; set; }
        public XmlDocumentForm xfm { get; set; }

        public void LoadXL(string name, bool load)
        {
            xt = new ToolWindow();
            xt.Text = name;
            xt.HideOnClose = true;
            xt.FormBorderStyle = FormBorderStyle.None;
            xt.TabText = name;
            xt.TopLevel = false;

            if (xfm == null)
            {
                xfm = new XmlDocumentForm();
                xfm.TopLevel = false;
                xfm.FormBorderStyle = FormBorderStyle.None;
                xfm.Dock = DockStyle.Fill;
                xfm.Show();
            }

            xt.Controls.Add(xfm);

            xt.types = "xt";

            if (load)
                xt.Show(dock, DockState.DockLeft);
        }

        public ToolWindow tx { get; set; }
        public Toolbox txf { get; set; }

        public void LoadTB(string name, bool load)
        {
            tx = new ToolWindow();
            tx.Text = name;
            tx.HideOnClose = true;
            tx.FormBorderStyle = FormBorderStyle.None;
            tx.TabText = name;
            tx.TopLevel = false;

            if (txf == null)
            {
                txf = new Toolbox();
                txf.TopLevel = false;
                txf.FormBorderStyle = FormBorderStyle.None;
                txf.Dock = DockStyle.Fill;
                txf.Show();
            }

            tx.Controls.Add(txf);

            tx.types = "tx";

            if (load)
                tx.Show(dock, DockState.DockLeft);
        }
        public ToolWindow hw { get; set; }
        public AvalonEdit.Host.TreeViewer_WinformsHost hwf { get; set; }

        public void LoadHW(string name, bool load)
        {
            hw = new ToolWindow();
            hw.Text = name;
            hw.HideOnClose = true;
            hw.FormBorderStyle = FormBorderStyle.None;
            hw.TabText = name;
            hw.TopLevel = false;
            if (hwf == null)
            {
                hwf = new AvalonEdit.Host.TreeViewer_WinformsHost();
                
                hwf.Dock = DockStyle.Fill;
                hwf.Show();
            }
            hw.Controls.Add(hwf);
            hw.types = "hw";
            if (load)
                hw.Show(dock, DockState.DockBottom);
        }
        public ToolWindow hc { get; set; }
        public AvalonEdit.Host.CallHierarchyViewer_WinformsHost hcc { get; set; }

        public void LoadHC(string name, bool load)
        {
            hc = new ToolWindow();
            hc.Text = name;
            hc.HideOnClose = true;
            hc.FormBorderStyle = FormBorderStyle.None;
            hc.TabText = name;
            hc.TopLevel = false;
            if (hcc == null)
            {
                hcc = new AvalonEdit.Host.CallHierarchyViewer_WinformsHost();

                hcc.Dock = DockStyle.Fill;
                hcc.Show();
            }
            hc.Controls.Add(hcc);
            hc.types = "hc";
            if (load)
                hc.Show(dock, DockState.DockBottom);
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

        public ToolWindow wd { get; set; }

        public ListViewOwnerDraw wdf { get; set; }

        public void LoadWD(string name, bool load)
        {
            wd = new ToolWindow();
            wd.Text = name;
            wd.HideOnClose = true;
            wd.FormBorderStyle = FormBorderStyle.None;
            wd.TabText = name;
            wd.TopLevel = true;

            wdf = new ListViewOwnerDraw();
            wdf.FormBorderStyle = FormBorderStyle.None;
            wdf.TopLevel = false;
            wdf.Dock = DockStyle.Fill;

            wd.Controls.Add(wdf);
            wdf.Show();

            wd.types = "wd";

            if (load)
                wd.Show(dock, DockState.DockBottom);
        }

        public ToolWindow wbr { get; set; }

        public BreakpointForm bdf { get; set; }

        public void LoadBR(string name, bool load)
        {
            wbr = new ToolWindow();
            wbr.Text = name;
            wbr.HideOnClose = true;
            wbr.FormBorderStyle = FormBorderStyle.None;
            wbr.TabText = name;
            wbr.TopLevel = true;

            bdf = new BreakpointForm();
            bdf.FormBorderStyle = FormBorderStyle.None;
            bdf.TopLevel = false;
            bdf.Dock = DockStyle.Fill;

            wbr.Controls.Add(bdf);
            bdf.Show();

            wbr.types = "wbr";

            if (load)
                wbr.Show(dock, DockState.DockBottom);
        }

        public ToolWindow cd { get; set; }

        public void LoadCD(string name, bool load)
        {
            cd = new ToolWindow();
            cd.Text = name;
            cd.HideOnClose = true;
            cd.FormBorderStyle = FormBorderStyle.None;
            cd.TabText = name;
            cd.TopLevel = true;

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

        public ToolWindow sf { get; set; }

        public ServerExplorerForm sef { get; set; }

        public void LoadSF(string name, bool load)
        {
            sf = new ToolWindow();
            sf.Text = name;
            sf.HideOnClose = true;
            sf.FormBorderStyle = FormBorderStyle.None;
            sf.TabText = name;
            sf.TopLevel = true;

            sef = new ServerExplorerForm();
            sef.FormBorderStyle = FormBorderStyle.None;
            sef.TopLevel = false;
            sef.Dock = DockStyle.Fill;

            sf.Controls.Add(sef);
            sef.Show();

            sf.types = "sf";

            if (load)
                sf.Show(dock, DockState.DockLeft);
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

        public void SetPropertyGridObject(object obs)
        {
            pgp.SelectedObject = obs;
        }

        private PropertyGrid pgp { get; set; }
        public ToolWindow pw { get; set; }

        public void LoadPG(string name, bool load)
        {
            if (pgp == null)
            {
                pgp = new PropertyGrid();
                pgp.Dock = DockStyle.Fill;

                if (pw == null)
                {
                    pw = new ToolWindow();
                    pw.Text = name;
                }
                pw.FormBorderStyle = FormBorderStyle.None;

                pw.TabText = name;
                pw.TopLevel = false;
                pw.HideOnClose = true;

                pw.Controls.Add(pgp);

                pw.types = "pw";
            }

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
            if (se == null)
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
                sesb.Image = ve_resource.Backward_256x;
                sesb.ToolTipText = "Backward";
                sesb.Click += Sest_Click;
                ses.Items.Add(sesb);

                sesf = new ToolStripButton();
                sesf.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sesf.Image = ve_resource.Forward_256x;
                sesf.ToolTipText = "Forward";
                sesf.Click += Sesf_Click;
                ses.Items.Add(sesf);

                sess = new ToolStripSeparator();

                ses.Items.Add(sess);

                sest = new ToolStripButton();
                sest.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sest.Image = ve_resource.Synchronize_16x;
                sest.Click += Sest_Click;
                ses.Items.Add(sest);

                sesc = new ToolStripButton();
                sesc.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sesc.Image = ve_resource.VSO_CollapseBoard_16x;
                sesc.ToolTipText = "Collapse";
                sesc.Click += Sesc_Click;
                ses.Items.Add(sesc);

                sesa = new ToolStripButton();
                sesa.DisplayStyle = ToolStripItemDisplayStyle.Image;
                sesa.Image = ve_resource.ShowAllCode_16x;
                sesa.ToolTipText = "Show All";
                sesa.Click += Sesa_Click;
                ses.Items.Add(sesa);

                TreeViewEx v = treeView1;

                v.DrawMode = TreeViewDrawMode.OwnerDrawAll;
                v.DrawNode += V_DrawNode;
                v.ShowLines = false;
                v.HideSelection = false;

                se.Controls.Add(v);

                se.Controls.Add(ses);

                se.types = "se";
            }

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
        private void V_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            treeView1.OnDrawTreeNode(sender, e);
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
            ww.FormBorderStyle = FormBorderStyle.None;
            ww.TabText = name;
            ww.TopLevel = false;
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
                else if (parsedStrings[1] == "wd")
                    return wd;
                else if (parsedStrings[1] == "wbr")
                    return wbr;
                else if (parsedStrings[1] == "dt")
                    return dt;
                else if (parsedStrings[1] == "sf")
                    return sf;
                else if (parsedStrings[1] == "qe")
                    return qe;
                else if (parsedStrings[1] == "xt")
                    return xt;
                else if (parsedStrings[1] == "tx")
                    return tx;
                else if (parsedStrings[1] == "te")
                    return te;
                else if (parsedStrings[1] == "hw")
                    return hw;
                else if (parsedStrings[1] == "hc")
                    return hc;
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
                        // MessageBox.Show("Active project - " + vp.Name);
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
            forms = new FolderLister();

            forms.Dock = DockStyle.Fill;

            forms.Show();

            return forms;
        }

        public void ClearOutput()
        {
            if (efs != null)
                efs.ClearOutput();
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

        public void loadtoolmenu(ScriptControl scr)
        {
            this.scr = scr;

            _eo._currentDirectoryFollowsRoot = true;

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

            _ls.eo = _eo;
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



        #region Form Events

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        #endregion Form Events

        #region Public UI Elements

        ///// <summary>
        ///// The explorer window toolbar.
        ///// </summary>
        //public ToolStrip Toolbar
        //{
        //    get { return _mainToolStrip; }
        //}

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
        }

        #endregion Public UI Elements

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

        //public void SetActiveDocument()
        //{
        //    string s = "";

        //    if (scriptControl1.dockContainer1 != null)
        //    {
        //        if (scriptControl1.dockContainer1.ActiveDocumentPane != null)
        //        {
        //            s = "Script control DC - " + scriptControl1.dockContainer1.Size.Width + " - " + scriptControl1.dockContainer1.Size.Height + "\n";
        //            richTextBox1.AppendText(s);

        //            s = "Script control DC Pane- " + scriptControl1.dockContainer1.ActiveDocumentPane.Size.Width + " - " + scriptControl1.dockContainer1.ActiveDocumentPane.Height + "\n";
        //            richTextBox1.AppendText(s);

        //            AIMS.Libraries.Scripting.ScriptControl.Document d = scriptControl1.dockContainer1.ActiveDocument as AIMS.Libraries.Scripting.ScriptControl.Document;

        //            d.ShowQuickClassBrowserPanel();

        //            s = "Script control DC Pane- " + d.Size.Width + " - " + d.Height + "\n";
        //            richTextBox1.AppendText(s);

        //            d.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

        //            scriptControl1.dockContainer1.ActiveDocumentPane.Size = new Size(scriptControl1.Size.Width - 10, scriptControl1.Size.Height);

        //            // scriptControl1.dockContainer1.ActiveDocumentPane.master = scriptControl1;
        //        }
        //    }
        //}

        private BackgroundWorker _bw = null;

        public void startprogress()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
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
            //s.ImageKey = tv.ImageKey;
            //s.SelectedImageKey = tv.SelectedImageKey;

            font = new Font(s.Font, FontStyle.Bold);

            if (tv == null)
                return;

            //s.BeginInvoke(new Action(() =>
            //{
                s.Nodes.Clear();

                s.BeginUpdate();

                s.ImageList = tv.ImageList;

                s.Tag = tv.Tag;

                s.EndUpdate();
            //}));

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
                this.Invoke(new PopulateTreeNodes(populateTreeNode), obs);

                //ns.NodeFont = font;
                //tv.Nodes.Remove(ns);
                //s.Nodes.Add(ns);

                i++;
            }
        }

        public delegate void AvalonDelegate(string filent);

        public delegate void workerSelectDelegate(TreeNode node);

        public delegate void workerBuildDelegate(string recent, msbuilder_alls.MSBuild msbuilds);

        public delegate void workerFunctionDelegate(string recent);

        public delegate void workerTypeViewerDelegate(VSSolution vs);

        public delegate void PopulateTreeNodes(TreeView s, TreeView tv, TreeNode ns);

        public delegate void workerFunctionTestDelegate(string recent);

        private async void workerFunction(string recent)
        {
            //this.BeginInvoke(new Action(() => { SolutionClose(); }));

            TreeView vv = _sv.load_recent_solution(recent);

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            if (CodeEditorControl.AutoListImages == null)
            {
                ImageList imgs = CreateView_Solution.CreateImageList();
                VSProject.dc = CreateView_Solution.GetDC();

                CodeEditorControl.AutoListImages = imgs;
            }

            ArrayList L = null;

            this.Invoke(new Action(() => { Loads2(vv, _sv._SolutionTreeView); this.Text = Path.GetFileName(recent) + " - Visual Explorer"; }));

            this.BeginInvoke(new Action(async () =>
            {

                 //this.Invoke(new Action(() => { Loads2(vv, _sv._SolutionTreeView); this.Text = Path.GetFileName(recent) + " - Visual Explorer"; }));

                L = scr.LoadSolutionFiles();

                int index = _sv._SolutionTreeView.ImageList.Images.Keys.IndexOf("VSSolutionFile");

                if (splashForm != null)
                {
                    splashForm.Hide();
                    splashForm = null;
                }
                this.Show();
                Visible = true;
                Opacity = 100;
                this.Show();
                Command.counter--;
                this.Focus();

                if (!File.Exists(recent))
                    return;


                // editorWindow.Dispatcher.BeginInvoke((Action)(() => {
                //string file = "C:\\MSBuildProjects-beta\\VStudio.sln";

                VSSolutionLoader rw = new VSSolutionLoader();

                vs = null;

                Task vs_task = new Task(delegate
                {

                    //vv = rw.LoadProject(recent);

                    //this.Invoke(new Action(() => { Loads2(vv, _sv._SolutionTreeView); this.Text = Path.GetFileName(recent) + " - Visual Explorer"; }));

                    vs = vv.Tag as VSSolution;

                });

                vs_task.Start();

                Task rs_task = new Task(delegate
                {

                    //vs.CompileSolution();

                    rw.CompileSolution(recent);
                });
                rs_task.Start();
                await Task.WhenAll(new Task[] { vs_task, rs_task });

                vs.comp = rw.comp;
                vs.nts = rw.nts;
                vs.named = rw.named;
                vs.workspace = rw.workspace;
                vs.solution = rw.solution;

                scr.vs = vs;

                scr.LoadFromProject(vs);

                if (appLoadedEvent != null)
                    appLoadedEvent(this, new EventArgs());


                if (event_SelectedSolutionChanged != null)
                    event_SelectedSolutionChanged(this, vs);


            }));


            //this.BeginInvoke(new Action(() =>
            //{
            //    Loads2(tv, _sv._SolutionTreeView); this.Text = Path.GetFileName(recent) + " - Visual Explorer";
            //    if (scr != null)
            //    {
            //        vs = GetVSSolution();
            //        scr.BeginInvoke(new Action(() =>
            //        {
            //            L = scr.LoadSolutionFiles();
            //            _sv.main_project();
            //            CreateRecentWindows(L);

            //            if (splashForm != null)
            //            {
            //                splashForm.Hide();
            //                splashForm = null;
            //            }
            //            this.Show();
            //            Visible = true;
            //            Opacity = 100;
            //            this.Show();
            //            Command.counter--;
            //            this.Focus();
            //            if (appLoadedEvent != null)
            //                appLoadedEvent(this, new EventArgs());
            //            //Command_SetConfigurations();
            //            //Command_SetPlatforms();
            //            //Command_StartupProjects();
            //            if (vs != null)
            //            vs.CompileSolution();
            //        }));
            //        if (event_SelectedSolutionChanged != null)
            //            event_SelectedSolutionChanged(this, vs);
            //    }

            //    //Command.running = false;
            //}));

            //            Command.counter--;
        }

        public delegate void ApplicationLoaded(object sender, EventArgs e);

        public event ApplicationLoaded appLoadedEvent;

        public void LoadBreakpoints(object sender, EventArgs e)
        {
            //this.Focus(); ScriptControl.br.LoadBreakpoints(); /*MessageBox.Show("Starting..." + Row.counter);*/
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

                this.Invoke(new Action(() => { ResumeAtStartUp(); }));

                ScriptControl.br.LoadBreakpoints();

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
            s.Invoke(new Action(() =>
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

        public void Command_LoadSolution(string recent)
        {
            scr.ClearHistory();

            if (efs != null)
                efs.Invoke(new Action(() => { efs.ClearOutput(); }));

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        public void Command_SetPlatforms()
        {
            if (gui.Command_SolutionPlaform.platform == null)
                return;
            gui.Command_SolutionPlaform.platform.Execute();
        }

        public ArrayList Command_LoadPlatforms()
        {
            VSSolution vs = GetVSSolution();
            ArrayList L = ConfigurationManagerForm.GetProjectPlatform(vs);

            return L;
        }

        public ArrayList Command_LoadConfigurations()
        {
            VSSolution vs = GetVSSolution();
            ArrayList L = ConfigurationManagerForm.GetSolutionPlatform(vs);

            return L;
        }

        public void Command_StartPage()
        {
            //string web = "Start Page";
            //scr.OpenWebForm(web, "https://www.nuget.org/packages");

            DocumentForm df = scr.OpenDocumentForm();
            df.Text = "Start Page";
            df.FileName = "Start Page";
            df.Name = "Start Page";
            StartPageForm spf = new StartPageForm();
            spf.FormBorderStyle = FormBorderStyle.None;
            spf.TopLevel = false;
            spf.Dock = DockStyle.Fill;
            df.Controls.Add(spf);
            spf.Show();
            spf.GetFeed();
        }

        public ArrayList Command_LoadStartupProjects()
        {
            VSSolution vs = GetVSSolution();
            if (vs == null)
                return new ArrayList();
            ArrayList L = _sv.LoadStartupProjects(vs);

            return L;
        }

        public void Command_SetConfigurations()
        {
           
            if (gui.Command_SolutionConfiguration.configuration == null)
                return;
            gui.Command_SolutionConfiguration.configuration.Execute();
        }

        public void Command_CloseAllDocuments()
        {
            scr.ClearHistory();

            scr.CloseAll();
        }

        public void Command_AddConnection(string provider = "")
        {
            if (provider == "Oracle")
            {
                AddConnectionForm_Oracle asd = new AddConnectionForm_Oracle();
                asd.ShowDialog();
            }
            else
            {
                AddConnectionForm asd = new AddConnectionForm();
                asd.ShowDialog();
            }
        }

        public void Command_RunProgram(string d)
        {

            cmds.RunExternalExes(d, "");
            
        }

        public void Command_TakeSnapshot(string process = "")
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            string d = b + "Extensions\\procdump.exe";

            if (Directory.Exists("Snapshots") == false)
            {
                Directory.CreateDirectory("Snapshots");
            }

            int c = Directory.GetFiles("Snapshots").Length;

            string file = b + "Snapshots\\snapshot" + c + ".dmp";

            cmds.RunExternalExes(d, " -ma " + process + " " + file);
        }

        public DocumentForm Command_OpenDocumentForm(string name)
        {
            return _sv.eo.OpenDocumentForm(name);
        }

        public Document Command_OpenGenericDocument(string name)
        {
            return scr.AddGenericDocument(name);
        }

        public void Command_TakeTypedDataset(string filename)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            string d = b + "Extensions\\xsd.exe";

            string folder = Path.GetDirectoryName(filename);

            string filewithoutextension = Path.GetFileNameWithoutExtension(filename);

            string cmd = filename + " /DataSet /language:CS /out:" + folder;

            cmds.RunExternalExes(d, cmd);

            string designer = filewithoutextension + ".Designer.cs";

            designer = folder + "\\" + designer;

            string cs = folder + "\\" + filewithoutextension + ".cs";

            if (File.Exists(designer))
                File.Delete(designer);

            File.Copy(cs, designer);

            File.Delete(cs);

            string[] dd = Directory.GetFiles(b + "Templates\\xsd");

            foreach (string s in dd)
            {
                string c = folder + "\\" + Path.GetFileName(s).Replace("databaseDataSet", filewithoutextension);
                if (File.Exists(c))
                    File.Delete(c);
                File.Copy(s, c);
            }
        }

        public void Command_ImportTlb(string filename, string output)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            string d = b + "Extensions\\tlbimp.exe";

            string folder = Path.GetDirectoryName(filename);

            string filewithoutextension = Path.GetFileNameWithoutExtension(filename);

            string cmd = "\"" + filename + "\" " + "/out:" + output;

            cmds.RunExternalExes(d, cmd);
        }

        public void Command_ConfigurationManager()
        {
            ConfigurationManagerForm c = new ConfigurationManagerForm();
            VSSolution vs = GetVSSolution();
            c.LoadConfigs(vs);
            c.SetConfigView();
            c.LoadPage_MSBuild();
            c.ShowDialog();
        }

        public void Command_ReloadToolbar(ToolStrip ts, string module_name)
        {
            // ToolStrip ns = _sv.LoadToolbar(ts, module_name);

            ToolStrip ns = ve.LoadToolbar(ts, module_name);

            ExplorerForms.regToolstrips[module_name] = ns;
        }

        public void Command_ChangeToolstrip(ToolStrip c, AnchorStyles p, bool remove = false, int x = 900, int y = 0)
        {
            if (remove == true)
            {
                c.Parent.Controls.Remove(c);
                return;
            }

            Point pc = new Point(x, y);

            if (c.Parent != null)
                c.Parent.Controls.Remove(c);
            if (p == AnchorStyles.Left)
                toolStripContainer1.LeftToolStripPanel.Controls.Add(c);
            else if (p == AnchorStyles.Right)
                toolStripContainer1.RightToolStripPanel.Controls.Add(c);
            else if (p == AnchorStyles.Top)
            {
                toolStripContainer1.TopToolStripPanel.SuspendLayout();
                c.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
                int b = toolStripContainer1.TopToolStripPanel.Controls.Count;
                toolStripContainer1.TopToolStripPanel.Join(c, pc);
                //toolStripContainer1.TopToolStripPanel.Controls.SetChildIndex(c, 0);
                toolStripContainer1.TopToolStripPanel.ResumeLayout();
            }
            else if (p == AnchorStyles.Bottom)
                toolStripContainer1.BottomToolStripPanel.Controls.Add(c);
        }

        public void Command_CustomizeDialog(string c)
        {
            CustomizeForm cc = new CustomizeForm();
            cc.LoadView(c);
            cc.ShowDialog();
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

        public void SetVSProject(string mainproject)
        {
            _sv.SetVSProject(mainproject);
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
            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }

        public void ResumeAtStartUp()
        {
            if (_started == true)
                return;

            _started = true;

    
            string recent = _sv.load_recent_solution();
            _sv._SolutionTreeView.Nodes.Clear();
            _sv.save_recent_solution(recent);
            workerFunctionDelegate w = workerFunction;
            IAsyncResult r = w.BeginInvoke(recent, null, null);

    
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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
            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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
            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

        public NewProjectForm npf { get; set; }

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

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target, Dictionary<string, string> dict)
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
                fi.CopyTo(Path.Combine(target.FullName, dict[fi.Name]), true);
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
            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

                    VSParsers.ProjectItemInfo pp = node.Tag as VSParsers.ProjectItemInfo;

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
                    else if (node.Parent.Text == "Reference" || node.Text == "Reference")
                    {
                        context5.Show(treeView, p);
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            //VSParsers.ProjectItemInfo prs = eo.cvs.getactiveproject();

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

        public void Command_OpenFile(string file, AutoResetEvent autoEvent = null)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            _eo.LoadFile(file, vs, autoEvent);
        }

        public void Command_OpenFileAndGotoLine(string file, string line)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            //MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, vs);

            int s = Convert.ToInt32(line);

            scr.SelectText(s, 0);
        }

        //public void OpenFile(string file, string line)
        //{
        //    VSSolution vs = GetVSSolution();

        //    VSProject pp = vs.GetVSProject(file);

        //    MessageBox.Show("File " + file + " at line " + line);

        //    _eo.LoadFile(file, pp);

        //    int s = Convert.ToInt32(line);

        //    scr.SelectText(s, 0);
        //}

        public void OpenFile(string file, string line, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, vs);

            int s = Convert.ToInt32(line);

            scr.SelectText(s, length);
        }

        public void OpenFileXY(string file, int X, int Y, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            // MessageBox.Show("File " + file + " at line " + line);

            _eo.LoadFile(file, vs);

           AvalonDocument doc =  scr.OpenDocuments(file, vs);

           // int x = Convert.ToInt32(X);

           // int y = Convert.ToInt32(Y);

            scr.SelectTextXY(doc, X, Y);
        }

        public void OpenFileLine(string file, string line, int length)
        {
            VSSolution vs = GetVSSolution();

            VSProject pp = vs.GetVSProject(file);

            //MessageBox.Show("File " + file + " at line " + line);

            if (File.Exists(file) == false)
                return;

            _eo.LoadFile(file, vs);

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

            _eo.LoadFile(file, vs);

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

                VSParsers.ProjectItemInfo pr0 = nodes.Tag as VSParsers.ProjectItemInfo;

                ImageList img = CreateView_Solution.CreateImageList();
                Dictionary<string, string> dc = CreateView_Solution.GetDC();

                //_eo.LoadFile(files, dc, img, vs);
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

            VSParsers.ProjectItemInfo prs = node.Tag as VSParsers.ProjectItemInfo;

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

        public VSProject project_VSProject { get; set; }

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

            VSParsers.ProjectItemInfo pr0 = null;

            if (node.Tag.GetType() != typeof(VSParsers.ProjectItemInfo))
                return;

            pr0 = node.Tag as VSParsers.ProjectItemInfo;

            //if (pgp != null)
            this.BeginInvoke(new Action(() =>
            {
                if (pr0.SubType == "SolutionFolder")
                    pgp.SelectedObject = new WinExplorer.SolutionFolder(pr0);
                else if (pr0.psi != null && pr0.psi.SubType == "Folder")
                    pgp.SelectedObject = new WinExplorer.ProjectFolder(pr0); 
                else if (pr0.SubType == "SolutionNode")
                    pgp.SelectedObject = new WinExplorer.SolutionFile(pr0);
                else if (pr0.IsProjectNode)
                    pgp.SelectedObject = new WinExplorer.ProjectFile(pr0);
                else 
                    pgp.SelectedObject = new WinExplorer.ProjectFileData(pr0);
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

            if (pp != project_VSProject)
            {
                project_VSProject = pp;
                if (event_SelectedProjectChanged != null)
                    event_SelectedProjectChanged(this, project_VSProject);
            }

            if (pr0.psi != null)

            {
                VSProjectItem pi = pr0.psi;

                file = pi.fileName;

                if (file == null)
                    if (pp != null)
                        file = Path.GetDirectoryName(pp.FileName) + "\\" + pi.Include;

                if (event_SelectedProjectItemChanged != null)
                    event_SelectedProjectItemChanged(this, project_VSProject, file);

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
                VSParsers.ProjectItemInfo pr = pr0;

                return;
            }
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

            VSParsers.ProjectItemInfo prs = node.Tag as VSParsers.ProjectItemInfo;

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
            VSParsers.ProjectItemInfo pr0 = _sv._SolutionTreeView.Tag as VSParsers.ProjectItemInfo;

            if (pr0 != null)
                return pr0.vs;

            return _sv._SolutionTreeView.Tag as VSSolution;
        }

        public VSProject GetVSProject()
        {
            if (_sv == null)
                return null;
            if (_sv._SolutionTreeView == null)
                return null;
            TreeNode node = _sv._SolutionTreeView.SelectedNode;
            if (node == null)
                return null;
            VSParsers.ProjectItemInfo s = node.Tag as VSParsers.ProjectItemInfo;
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
            if (scr.hst != null)
                scr.hst.Modules = Module.ToString();

            scr.SaveSolution();

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

        private bool starting = true;

        public void SolutionClose()
        {
            if (starting)
                return;
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

            Command_StartPage();

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
            starting = false;
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
            if (_col != null)
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            string names = "";

            string dg = "";

            Dictionary<string, string> dict = null;

            if (SubType == "WindowsForm" || SubType == "Resource")
            {
                dg = pp.GetFormName(L);
                string[] cc = dg.Split(".".ToCharArray());

                dg = cc[0];
                dict = pp.AddFormItem(L, "");
            }
            else
                pp.SetCompileItems(L);

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            if (names == "")
                CopyAll(project, folder);
            else CopyAll(project, folder, dict);

            //if (SubType == "WindowsForm" || SubType == "Resource")

            //    pp.AddFormItem(L, "");
            //else
            //    pp.SetCompileItems(L);

            //DirectoryInfo project = new DirectoryInfo(pf);

            //DirectoryInfo folder = new DirectoryInfo(folds);

            //CopyAll(project, folder);

            reload_solution(file);
        }

        public void Command_AddNewProjectItem(string name, VSProject pp)
        {
            VSSolution vs = GetVSSolution();

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            Dictionary<string, string> dict = NewProjectItem.Templates();

            ArrayList L = NewProjectItem.ProjectFiles(name, dict);

            string pf = NewProjectItem.ProjectFolder(name, dict);

            if (pf == "")
                return;

            string SubType = NewProjectItem.GetSubType(name, dict);

            string folds = Path.GetDirectoryName(pp.FileName);

            string names = "";

            string dg = "";

            Dictionary<string, string> dd = new Dictionary<string, string>();

            if (SubType == "WindowsForm" || SubType == "Resource")
            {
                dg = pp.GetFormName(L);
                string[] cc = dg.Split(".".ToCharArray());

                dg = cc[0];
                dd = pp.AddFormItem(L, "");
            }
            else
                pp.SetCompileItems(L);

            DirectoryInfo project = new DirectoryInfo(pf);

            DirectoryInfo folder = new DirectoryInfo(folds);

            if (dict.Count <= 0)
                CopyAll(project, folder);
            CopyAll(project, folder, dd);

            reload_solution(file);
        }

        public List<string> Command_RecentSolutions()
        {
            return _sv.hst.GetRecentSolutions();
        }

        public void Command_NavigateForward()
        {
            _sv.NavigateForward();
        }

        public void Command_NavigateBackward()
        {
            _sv.NavigateBackward();
        }

        public void Command_OpenFolder(string foldername = "")
        {
            if (string.IsNullOrEmpty(foldername))
            {
                
                FolderBrowserDialog ofb = new FolderBrowserDialog();
                DialogResult r = ofb.ShowDialog();
                if (r != DialogResult.OK)
                    return;
                foldername = ofb.SelectedPath;
            }
            if (forms == null)
                return;
            forms.ListDirectoryContent(foldername);

        }

        public void Command_OpenSolution(string filename = "")
        {

            if (string.IsNullOrEmpty(filename))
            {
                OpenFileDialog ofd = new OpenFileDialog();
                DialogResult r = ofd.ShowDialog();
                if (r != DialogResult.OK)
                    return;
                filename = ofd.FileName;
            }
            
            scr.ClearHistory();

            ClearOutput();

            string recent = filename;

            _sv._SolutionTreeView.Nodes.Clear();

            _sv.save_recent_solution(recent);

            workerFunctionDelegate w = workerFunction;
            w.BeginInvoke(recent, null, null);
        }
        
        public void Command_OpenFile()
        {
            _sv.OpenFile();
        }

        public void Command_SaveFile()
        {
            _sv.save_active();
        }

        public void Command_SaveAllFiles()
        {
            _sv.save_active();
        }

        public void Command_Undo()
        {
            _sv.undo();
        }

        public void Command_Redo()
        {
            _sv.redo();
        }

        public void Command_AddItem()
        {
        }

        public void Command_Copy()
        {
            if (scr != null)
                scr.Copy();
        }

        public void Command_RunProject()
        {

        }

        public void Command_SetPropertyGrid(object obs)
        {
            pgp.BeginInvoke(new Action(() => { pgp.SelectedObject = obs; }));
        }

        public void Command_SolutionExplorer()
        {
        }

        public void Command_Launch(string s)
        {
            if (s == "Properties")
            {
                LoadPG("Properties", true);
                propertyGridToolStripMenuItem.Checked = true;
            }
            else if (s == "Find In Files")
            {
                LoadRW("Find Replace", true);
                findAndReplaceToolStripMenuItem.Checked = true;
            }
            else if (s == "Solution Explorer")
            {
                LoadSE("Solution Explorer", true);
                solutionExplorerToolStripMenuItem.Checked = true;
            }
        }

        public void Command_PropertiesWindows()
        {
        }

        public void Command_OtherWindows()
        {
        }

        public void Command_NewConnection()
        {
            AddConnectionForm ac = new AddConnectionForm();
            ac.ShowDialog();
        }

        public Dictionary<VSProject, ArrayList> Command_Get_DataSources()
        {
            VSSolution vs = GetVSSolution();

            if (vs == null)
                return null;

            return vs.GetDataSources();
        }

        public void Command_Paste()
        {
            if (scr != null)
                scr.Paste();
        }

        public void Command_Cut()
        {
            if (scr != null)
                scr.Cut();
        }

        private FindResults r { get; set; }

        public void Command_Find(string name)
        {
            //MessageBox.Show("Search for " + name);

            Document doc = scr.GetActiveDocument();

            if (r == null || doc.FileName != r.filename)
            {
                ArrayList Results = FindReplaceForm.FindInActiveDocument(name);

                r = new FindResults();

                r.pattern = name;

                r.filename = doc.FileName;

                r.Results = Results;

                r.HighLightWords();
            }
            else r.HighLightNext();
        }

        public void Command_SolutionConfigurations(string cfg)
        {
            _sv.SetConfiguration(cfg);
        }

        public void Command_SolutionPlatforms(string plf)
        {
            _sv.SetPlatform(plf);
        }

        public void Command_StartupProjects()
        {
            if(gui.Command_SolutionRun.projects == null) {

                return;

            }
            gui.Command_SolutionRun.projects.Execute();
        }
        public void Command_StartupProject()
        {
            if (gui.Command_SolutionRun.projects == null)
            {

                return;

            }
            gui.Command_SolutionRun.projects.Execute();
        }
        public void DebugTarget()
        {
        }

        public void DebugType()
        {
        }

        public void Command_AddNewFileProjectItem(string name, VSProject pp)
        {
            VSSolution vs = GetVSSolution();

            string sln = vs.SolutionPath;

            string file = vs.solutionFileName;

            // Dictionary<string, string> dict = NewProjectItem.Templates();

            ArrayList L = new ArrayList();

            L.Add(name);

            pp.SetCompileItems(L);

            reload_solution(file);
        }

        private void AddNewProject(bool addproject, string filename = "")
        {
            npf = new NewProjectForm(filename);
            npf.LoadAddProject();
            if (addproject == true)
                npf.SetProjectFolder("sln");
            DialogResult r = npf.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

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

            string prf = pr.Replace(bs + "\\", "");

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

            string projectfile = sln + "\\" + pn + "\\" + Path.GetFileName(proj);

            File.Move(tempfile, projectfile);

            reload_solution(file);
        }

        public NewWebSiteForm nwp { get; set; }

        private void AddNewWebSite(bool addproject, string filename = "")
        {
            nwp = new NewWebSiteForm(filename);
            nwp.LoadAddProject();
            if (addproject == true)
                nwp.SetProjectFolder("sln");
            DialogResult r = nwp.ShowDialog();
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

            string prf = pr.Replace(bs + "\\", "");

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

            string projectfile = sln + "\\" + pn + "\\" + Path.GetFileName(proj);

            File.Move(tempfile, projectfile);

            reload_solution(file);
        }


        public void Command_AddNewProject(string filename = "")
        {
            AddNewProject(false, filename);
        }

        public void Command_AddNewWebSite(string filename = "")
        {
            AddNewWebSite(false, filename);
        }

        public void Command_AddNewProject(bool addproject, NewProject nw)
        {
            Command.counter = 2;

            string bs = AppDomain.CurrentDomain.BaseDirectory + "TemplateProjects";

            nw.CreateNewProject();

            string file = nw.SolutionFile;

            string recent = file;

            scr.ClearHistory();
            ClearOutput();
            SolutionClose();
            _sv._SolutionTreeView.Nodes.Clear();
            _sv.save_recent_solution(recent);
            workerFunctionDelegate w = workerFunction;
            w.Invoke(recent);

            VSSolution vs = _sv._SolutionTreeView.Tag as VSSolution;

            string sln = vs.SolutionPath;

            string pn = nw.ProjectName;

            string pr = nw.GetProjectFile();

            string prf = pr.Replace(bs + "\\", "");

            string prc = pn.Replace(bs + "\\", "");

            string prs = Path.GetFileName(pr);

            string folds = Path.GetDirectoryName(pr);

            string[] dd = folds.Split("\\".ToCharArray());

            string fds = dd[dd.Length - 1];

            string content = File.ReadAllText(file);

            int p = content.IndexOf("Global");

            string dir = fds;

            dir = pn;

            string proj = prs;

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

            string projectfile = sln + "\\" + pn + "\\" + Path.GetFileName(proj);

            File.Move(tempfile, projectfile);

            //           while(Command.running == true);

            reload_solution(file);

            //         ThreadWorker worker = new ThreadWorker();

            //         Thread thread1 = new Thread(worker.Run);
            //         thread1.Start();

            // this.Invoke(new Action(() => { while (Command.counter > 0) ; } ));
            //         thread1.Join();
        }

        public VSParsers.ProjectItemInfo GetProjectInfo()
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null)
                return null;

            if (node.Tag == null)
                return null;

            VSParsers.ProjectItemInfo prs = node.Tag as VSParsers.ProjectItemInfo;

            return prs;
        }

        //private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    TreeNode node = treeView1.SelectedNode;
        //    if (node == null)
        //        return;

        //    ClipboardForm c = new ClipboardForm();
        //    if (_selectedNodes.Count > 0)
        //        c.LoadTreeNode(_selectedNodes);
        //    else
        //    {
        //        List<TreeNode> n = new List<TreeNode>();
        //        n.Add(node);
        //        c.LoadTreeNode(n);
        //    }
        //    c.ShowDialog();

        //    // ReloadVSProject();
        //}

        public VSProjectItem bc { get; set; }

        public TreeNode ncp { get; set; }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TreeNode node = treeView1.SelectedNode;
            //if (node == null)
            //    return;
            //ClipboardForm bb = new ClipboardForm();
            //if (_selectedNodes.Count > 0)
            //    bb.LoadTreeNode(_selectedNodes);
            //else
            //{
            //    List<TreeNode> n = new List<TreeNode>();
            //    n.Add(node);
            //    bb.LoadTreeNode(n);
            //}

            //bb.ShowDialog();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;
            ClipboardForm bb = new ClipboardForm();
            bb.DeleteClipboard(node);
            //bb.ShowDialog();

            ReloadVSProject();
        }

        private void newItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewProjectItem();
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            VSProject p = GetVSProject();

            VSParsers.ProjectItemInfo prs = GetProjectInfo();

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
                VSParsers.ProjectItemInfo p = nodes.Tag as VSParsers.ProjectItemInfo;

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

            ExtensionManager em = new ExtensionManager();
            RootObject r = em.GetExtensions();


            return;

            VSSolution vs = GetVSSolution();
            var ex = vs.GetExecutables(VSSolution.OutputType.both).ToArray();
            var b = System.Array.ConvertAll(ex, x => x.ToString());

            var assemblies = b.Select(x => new AssemblyAndConfigFile(x, configFileName: null));
            

            xrunner runner = new xrunner();
            runner.LoadAssemblies(assemblies);

            return;

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

            VSParsers.ProjectItemInfo prs = node.Tag as VSParsers.ProjectItemInfo;

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

        public void Command_Rebuild(string project = "")
        {
            //while (Command.running == true) ;

            //Command.running = true;

            VSSolution vs = GetVSSolution();

            if (vs == null)
                return;

            VSProject vp = vs.GetProjectbyName(project);

            project = vp.FileName;

            if (efs != null)
                efs.ClearOutput();

            string p = _sv.GetPlatform();

            string c = _sv.GetConfiguration();

            msbuilder_alls.MSBuild msbuilds = new msbuilder_alls.MSBuild();

            msbuilds.build = msbuilder_alls.Build.rebuild;

            msbuilds.Platform = p;

            msbuilds.Configuration = c;

            //string sf = vs.solutionFileName;// project.FileName;

            msbuilds.MSBuildFile = project;

            if (project == "")

                msbuilds.MSBuildFile = vs.solutionFileName;

            workerBuildDelegate b = build_alls;

            b.BeginInvoke(vs.solutionFileName, msbuilds, null, null);

            //Command.running = false;
        }

        public void Command_Config(string plf, string cfg)
        {
            if (plf != "")
                _sv.SetPlatform(plf);

            if (cfg != "")
                _sv.SetConfiguration(cfg);

            this.BeginInvoke(new Action(() => { this.Refresh(); }));
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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
            //TreeNode node = treeView1.SelectedNode;
            //if (node == null)
            //    return;

            //ClipboardForm c = new ClipboardForm();
            //if (_selectedNodes.Count > 0)
            //    c.LoadTreeNode(_selectedNodes);
            //else
            //{
            //    List<TreeNode> n = new List<TreeNode>();
            //    n.Add(node);
            //    c.LoadTreeNode(n);
            //}

            //c.ShowDialog();

            //ReloadVSProject();
        }

        //public void RenameFolder()
        //{
        //    VSProject p = GetVSProject();

        //    VSParsers.ProjectItemInfo prs = GetProjectInfo();

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

            VSParsers.ProjectItemInfo prs = GetProjectInfo();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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
            //TreeNode node = treeView1.SelectedNode;
            //if (node == null)
            //    return;
            //ClipboardForm bb = new ClipboardForm();
            //if (_selectedNodes.Count > 0)
            //    bb.LoadTreeNode(_selectedNodes);
            //else
            //{
            //    List<TreeNode> n = new List<TreeNode>();
            //    n.Add(node);
            //    bb.LoadTreeNode(n);
            //}
            //bb.ShowDialog();
        }

        private void runTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {


            var c = System.Threading.Tasks.Task.Run(() =>
            {
                tef.ClearTests();
                VSSolution vs = GetVSSolution();
                foreach (VSProject vp in vs.Projects)
                {
                    //VSProject vp = vs.GetProjectbyName("VE-Tests");
                    msbuilder_alls.MSTest mstest = new msbuilder_alls.MSTest();
                    mstest.msTestPath = AppDomain.CurrentDomain.BaseDirectory + "\\Extensions\\_starters-discovery.bat";
                    mstest.SetTestLibrary(vp.GetProjectExec());


                    msbuilder_alls.ExecuteMsTests(mstest);


                    List<string> b = mstest.GetDiscoveredTests();

                    if (te == null)
                        return;
                    tef.Invoke(new Action(() => { tef.LoadTest(vp, b); }));


                }

            }
        );
            var t = System.Threading.Tasks.Task.Run(() =>
            {



                MSTestServer server = new MSTestServer();

            }

            );


            //workerFunctionTestDelegate w = TestForm;
            //w.BeginInvoke("recent", null, null);
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

            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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
            //TreeNode node = _sv._SolutionTreeView.SelectedNode;
            //if (node == null)
            //    return;
            //ClipboardForm bb = new ClipboardForm();
            //if (_selectedNodes.Count > 0)
            //    bb.LoadTreeNode(_selectedNodes);
            //else
            //{
            //    List<TreeNode> n = new List<TreeNode>();
            //    n.Add(node);
            //    bb.LoadTreeNode(n);
            //}

            //bb.ShowDialog();
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
            //  WinExplorer.UI.ProjectPropertyForm ppf = new WinExplorer.UI.ProjectPropertyForm();
            //  ppf.Show();
            //WinExplorer.UI.ListViewOwnerDraw od = new ListViewOwnerDraw();
            //od.Show();
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
        private object ExtensionsManager;

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
            //string web = "Start Page";
            //scr.OpenWebForm(web, "https://www.nuget.org/packages");

            DocumentForm df = scr.OpenDocumentForm();
            df.Text = "Start Page";
            StartPageForm spf = new StartPageForm();
            spf.FormBorderStyle = FormBorderStyle.None;
            spf.TopLevel = false;
            spf.Dock = DockStyle.Fill;
            df.Controls.Add(spf);
            spf.Show();
            spf.GetFeed();

        }

        public void OpenNuGetPage()
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
            //TreeNode node = treeView1.SelectedNode;
            //if (node == null)
            //    return;

            //ClipboardForm c = new ClipboardForm();
            //if (_selectedNodes.Count > 0)
            //    c.LoadTreeNode(_selectedNodes);
            //else
            //{
            //    List<TreeNode> n = new List<TreeNode>();
            //    n.Add(node);
            //    c.LoadTreeNode(n);
            //}
            //c.ShowDialog();

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

        public static string GetVisualStudio2017InstalledPath()
        {
            var visualStudioInstalledPath = string.Empty;
            var visualStudioRegistryPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\SxS\VS7");
            if (visualStudioRegistryPath != null)
            {
                visualStudioInstalledPath = visualStudioRegistryPath.GetValue("15.0", string.Empty) as string;
            }

           

            return visualStudioInstalledPath;
        }

        private void openCMD()
        {
            VSParsers.ProjectItemInfo prs = _eo.cvs.getactiveproject();

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

        private static void HeapInfo()
        {
            // Create the DataTarget.  This can be done through a crash dump, a live process pid, or
            // via an IDebugClient pointer.
            //int pid = Process.GetProcessesByName("HelloWorld")[0].Id;
            int pid = Process.GetCurrentProcess().Id;

            string name = Process.GetCurrentProcess().ProcessName;

            DataTarget target = DataTarget.AttachToProcess(pid, 5000);

            // DataTarget.ClrVersions lists the versions of CLR loaded in the process (this may be
            // v2 and v4 in the Side-By-Side case.
            ClrInfo version = target.ClrVersions[0];

            // CLRVersionInfo contains information on the correct Dac dll to load.  This includes
            // the long named dac, the version of clr, etc.  This is enough information to request
            // the dac from the symbol server (though we do not provide an API to do this).  Also,
            // if the version you are debugging is actually installed on your machine, DacLocation
            // will contain the full path to the dac.
            //string dacLocation = version.TryGetDacLocation();

            // If we don't have the dac installed, we will use the long-name dac in the same folder.
            //if (!string.IsNullOrEmpty(dacLocation))
            //    dacLocation = version.DacInfo.FileName;

            // Now create a CLRRuntime instance.  This is the "root" object of the API. It allows
            // you to do things like Enumerate memory regions in CLR, enumerate managed threads,
            // enumerate AppDomains, etc.
            ClrRuntime runtime = target.ClrVersions[0].CreateRuntime();

            // Print out some basic information like:  Number of managed threads, number of AppDomains,
            // number of objects on the heap, and so on.  Note you can walk the AppDomains in the process
            // with:  foreach (CLRAppDomain domain in runtime.AppDomains)
            // Same for runtime.Threads to walk the threads in the process.  You can walk callstacks
            // (similar to !clrstack) with:
            //    foreach (CLRStackFrame frame in runtime.Threads[i].StackTrace)
            Console.WriteLine("AppDomains:        {0:n0}", runtime.AppDomains.Count);
            Console.WriteLine("Managed Threads:   {0:n0}", runtime.Threads.Count);

            // Now let's walk the heap and count objects, and total size of all objects.
            ulong objSize = 0;
            uint objCount = 0;

            // To operate on the heap, you need a GCHeap object.
            ClrHeap heap = runtime.GetHeap();

            // Walk the heap.
            foreach (ulong obj in heap.EnumerateObjectAddresses())
            {
                // The type of the object lets you do things like get the size of the object,
                // walk the fields of the object, etc.
                ClrType type = heap.GetObjectType(obj);

                // Type would only be null if there is heap corruption.
                if (type == null)
                    continue;

                // Note you can do a lot with obj right now.  Collect per-type statistics for example.
                // GCHeapType.Name also holds the type name, and so on.  Note there is currently not
                // a way to get the MethodTable (yet!) but hopefully you should be able to do everything
                // you need with GCHeapType.
                objCount++;
                objSize += type.GetSize(obj);
            }

            Console.WriteLine("Object Count:      {0:n0}", objCount);
            Console.WriteLine("Total Object Size: {0:n0}", objSize);
        }

        private void guiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //NuGetForm ngf = new NuGetForm(project_VSProject);
            //ngf.ShowDialog();
            //return;

            //WinExplorer.Services.Syntax.Syntaxer.Execute(new RichTextBox());

            //NewProjectForm.gcommands.GetCommand("AddConnection").Execute("Oracle");

            //NewProjectForm.gcommands.GetCommand("Customize").Execute("Commands-Toolbar-Standard");

            return;

            //string b = AppDomain.CurrentDomain.BaseDirectory;
            //npf = NewProjectForm.gcommands.GetCommand("OpenForm").Execute() as NewProjectForm;
            //ArrayList T = NewProjectForm.gcommands.GetCommand("GetTemplates").Execute() as ArrayList;
            ////MessageBox.Show(T.Count + " templates found");
            //NewProjectForm.gcommands.GetCommand("AddTemplate").Execute(b + "Sources\\ConsoleApplication5");
            //T = NewProjectForm.gcommands.GetCommand("GetTemplates").Execute() as ArrayList;
            ////MessageBox.Show(T.Count + " templates found");
            //NewProjectForm.gcommands.GetCommand("CloseForm").Execute();

            //NewProject nw = new NewProject();
            //nw.ProjectName = b + "TemplateProjects\\ConsoleApplication5";
            //nw.Location = b + "Projects";
            //nw.SolutionName = "MakeProject";

            //this.Invoke(new Action(() => { NewProjectForm.gcommands.GetCommand("LoadProjectTemplate").Execute(nw); }));

            NewProjectForm.gcommands.GetCommand("Platform").Execute("Release");

            //NewProjectForm.gcommands.GetCommand("Config").Execute("Any CPU");

            //NewProjectForm.gcommands.GetCommand("MSBuild").Execute();

            ThreadWorker worker = new ThreadWorker();
            worker.ef = ef;
            Thread thread = new Thread(worker.Runs);
            thread.Start();

            // Thread thread_gui = new Thread(worker.Run);
            //  thread_gui.Start();
        }

        private void existingItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
                return;

            string file = ofd.FileName;

            VSProject vp = GetVSProject();

            Command_AddNewFileProjectItem(file, vp);
        }

        private void ExplorerForms_ResizeBegin(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.HideAndRedraw();
        }

        private void ExplorerForms_ResizeEnd(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.ShowAndRedraw();
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Command_CustomizeDialog("");
        }

        private void toolStripMenuItem73_Click(object sender, EventArgs e)
        {
        }

        private void watchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wd == null)
                LoadWD("Watch", false);
            else if (wd.Visible == true)
            {
                wd.Hide();
                watchToolStripMenuItem.Checked = false;
            }
            else
            {
                wd.Show(dock, DockState.DockBottom);
                //ews.Show();
                watchToolStripMenuItem.Checked = true;
            }
        }

        private void breakpointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wbr == null)
                LoadBR("Breakpoints", false);
            else if (wbr.Visible == true)
            {
                wbr.Hide();
                breakpointsToolStripMenuItem.Checked = false;
            }
            else
            {
                wbr.Show(dock, DockState.DockBottom);
                breakpointsToolStripMenuItem.Checked = true;
            }
        }

        private void dataSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dt == null)
                LoadDT("Data Sources", false);
            else if (dt.Visible == true)
            {
                dt.Hide();
                dataSourcesToolStripMenuItem.Checked = false;
            }
            else
            {
                dt.Show(dock, DockState.DockBottom);
                dataSourcesToolStripMenuItem.Checked = true;
            }
        }

        private void toolStripMenuItem81_Click(object sender, EventArgs e)
        {
            //if (sf == null)
            //    LoadWD("Server Explorer", false);
            //else if (sf.Visible == true)
            //{
            //    sf.Hide();
            //    toolStripMenuItem81.Checked = false;
            //}
            //else
            //{
            //    sf.Show(dock, DockState.DockLeft);
            //    toolStripMenuItem83.Checked = true;
            //}
        }

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    // WM_SYSCOMMAND
        //    if (m.Msg == 0x0112)
        //    {
        //        if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
        //            // || m.WParam == new IntPtr(0xF120)) // Restore event - SC_RESTORE from Winuser.h
        //        {
        //            if (scr == null)
        //                return;

        //            scr.HideAndRedraw();
        //        }
        //    }
        //   // base.WndProc(ref m);
        //}

        //FormWindowState? LastWindowState = null;
        //private void ExplorerForms_Resize(object sender, EventArgs e)
        //{
        //    if (WindowState != LastWindowState)
        //    {
        //        if (WindowState == FormWindowState.Maximized)
        //        {
        //            if (scr == null)
        //                return;

        //            scr.ShowAndRedraw();
        //        }
        //        if (WindowState == FormWindowState.Normal)
        //        {
        //        }
        //        LastWindowState = WindowState;
        //    }
        //}

        #region Events

        public delegate void SelectedSolutionChange(object sender, VSSolution vs);

        public event SelectedSolutionChange event_SelectedSolutionChanged;

        public delegate void SelectedProjectChange(object sender, VSProject vp);

        public event SelectedProjectChange event_SelectedProjectChanged;

        public delegate void SelectedProjectItemChange(object sender, VSProject vp, string file);

        public event SelectedProjectItemChange event_SelectedProjectItemChanged;

        #endregion Events

        private void toolStripMenuItem83_Click(object sender, EventArgs e)
        {
            if (sf == null)
                LoadSF("Server Explorer", false);
            else if (sf.Visible == true)
            {
                sf.Hide();
                toolStripMenuItem83.Checked = false;
            }
            else
            {
                sf.Show(dock, DockState.DockLeft);
                toolStripMenuItem83.Checked = true;
            }
        }

        private void toolStripMenuItem82_Click(object sender, EventArgs e)
        {
            if (qe == null)
                LoadQE("SQL Server Explorer", false);
            else if (qe.Visible == true)
            {
                qe.Hide();
                toolStripMenuItem82.Checked = false;
            }
            else
            {
                qe.Show(dock, DockState.DockLeft);
                toolStripMenuItem82.Checked = true;
            }
        }

        private void xmlDocumentOutlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (xt == null)
                LoadXL("XML Document", false);
            else if (xt.Visible == true)
            {
                xt.Hide();
                xmlDocumentOutlineToolStripMenuItem.Checked = false;
            }
            else
            {
                xt.Show(dock, DockState.DockLeft);
                xmlDocumentOutlineToolStripMenuItem.Checked = true;
            }
        }

        private void toolStripMenuItem84_Click(object sender, EventArgs e)
        {
            if (tx == null)
                LoadTB("Toolbox", false);
            else if (tx.Visible == true)
            {
                tx.Hide();
                toolStripMenuItem84.Checked = false;
            }
            else
            {
                tx.Show(dock, DockState.DockLeft);
                toolStripMenuItem84.Checked = true;
            }
        }

        private void managedNuGetPackagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // OpenNuGetPage();
            //NuGets nuget = new NuGets();
            //VSSolution vs = GetVSSolution();
            //string file = vs.SolutionPath + "\\" + "packages\\NuGet.Core.2.14.0\\NuGet.Core.2.14.0.nupkg";
            //nuget.ReadNuSpec(file);

            NuGetForm ngf = new NuGetForm(project_VSProject);
            ngf.ShowDialog();
        }

        private void testExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (te == null)
                LoadTE("Test Explorer", false);
            else if (te.Visible == true)
            {
                te.Hide();
                testExplorerToolStripMenuItem.Checked = false;
            }
            else
            {
                te.Show(dock, DockState.DockLeft);
                testExplorerToolStripMenuItem.Checked = true;
            }
        }

        private void hELPToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void commandWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openCMD();
        }

        private void _mainTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void toolStripMenuItem87_Click(object sender, EventArgs e)
        {
            //string web = "Start Page";
            //scr.OpenWebForm(web, "https://www.nuget.org/packages");

            DocumentForm df = scr.OpenDocumentForm();
            df.Text = "Start Page";
            StartPageForm spf = new StartPageForm();
            spf.FormBorderStyle = FormBorderStyle.None;
            spf.TopLevel = false;
            spf.Dock = DockStyle.Fill;
            df.Controls.Add(spf);
            spf.Show();
            spf.GetFeed();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VSSolution vs = GetVSSolution();
            VSProject vp = GetVSProject();

            if (vp == null)
                return;

            scr.OpenDocumentsWithContent("", vs);
        }
    }

    internal class ThreadWorker
    {
        public ExplorerForms ef { get; set; }

        public void Runs(object state)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;
            NewProject nw = new NewProject();
            nw.ProjectName = b + "TemplateProjects\\ConsoleApplication5";
            nw.Location = b + "Projects";
            nw.SolutionName = "MakeProject";

            NewProjectForm.gcommands.GetCommand("LoadSolution").Execute(b + "Projects\\SharpDisasm\\SharpDisasm.sln");

            NewProjectForm.gcommands.GetCommand("MainProject").Execute("SharpDisasm");

            NewProjectForm.gcommands.GetCommand("TakeSnapshot").Execute("WinExplorer");

            return;

            //VSProject vp = null;
            //ArrayList F = null;
            //ef.Invoke(new Action(() => { VSSolution vs = ef.GetVSSolution(); vp = vs.MainVSProject; F = vp.GetCompileItems(); }));

            //foreach (string file in F)
            //{
            //    NewProjectForm.gcommands.GetCommand("OpenDocument").Execute(file);
            //    break;
            //}

            //NewProjectForm.gcommands.GetCommand("AddProjectItem").Execute("WindowsForm");

            int i = 0;
            while (true)
            {
                //string b = AppDomain.CurrentDomain.BaseDirectory;
                //NewProject nw = new NewProject();
                //nw.ProjectName = b + "TemplateProjects\\ConsoleApplication5";
                //nw.Location = b + "Projects";
                //nw.SolutionName = "MakeProject";

                //NewProjectForm.gcommands.GetCommand("LoadSolution").Execute(b + "Projects\\SharpDisasm\\SharpDisasm.sln");

                //NewProjectForm.gcommands.GetCommand("LoadProjectTemplate").Execute(nw);

                //NewProjectForm.gcommands.GetCommand("MainProject").Execute("ConsoleApplication5");

                NewProjectForm.gcommands.GetCommand("MainProject").Execute("SharpDisasm");

                NewProjectForm.gcommands.GetCommand("Platform").Execute("Net45Release");

                NewProjectForm.gcommands.GetCommand("Config").Execute("Any CPU");

                //NewProjectForm.gcommands.GetCommand("MSBuild").Execute("ConsoleApplication5");

                NewProjectForm.gcommands.GetCommand("MSBuild").Execute("SharpDisasm");

                NewProjectForm.gcommands.GetCommand("Platform").Execute("Net45Debug");

                //NewProjectForm.gcommands.GetCommand("MSBuild").Execute("ConsoleApplication5");

                NewProjectForm.gcommands.GetCommand("MSBuild").Execute("SharpDisasm");

                VSProject vpb = null;
                ArrayList Fb = null;
                ef.Invoke(new Action(() => { VSSolution vs = ef.GetVSSolution(); vpb = vs.MainVSProject; Fb = vpb.GetCompileItems(); }));

                foreach (string file in Fb)
                    NewProjectForm.gcommands.GetCommand("OpenDocument").Execute(file);

                //NewProjectForm.gcommands.GetCommand("AddProjectItem").Execute("WindowsForm");

                NewProjectForm.gcommands.GetCommand("CloseAllDocuments").Execute("");

                System.GC.Collect();

                System.GC.Collect();

                System.GC.Collect();

                //NewProjectForm.gcommands.GetCommand("TakeSnapshot").Execute("WinExplorer.exe");
            }
        }

        public void Run(object state)
        {
            Random r = new Random();

            int x = 600;

            int y = 400;

            while (true)
            {
                Point p = ef.Location;

                double d0 = r.NextDouble();

                if (d0 > 0.5)
                    d0 = 1;
                else
                    d0 = -1;

                double dx = r.NextDouble() * (1900 - 500);

                double dy = r.NextDouble() * (1000 - 300);

                ef.Invoke(new Action(() => { ef.Location = new Point((int)dx, (int)dy); }));

                Thread.Sleep(1000);

                ef.Invoke(new Action(() =>
                {
                    x += 30;

                    if (x > 1000)
                        x = 600;

                    if (x < 600)
                        x = 600;

                    y += 30;

                    if (y > 700)
                        y = 500;

                    if (y < 500)
                        y = 500;

                    ef.Size = new Size(x, y); ef.Refresh();
                }));

                Thread.Sleep(1000);
            }
        }
    }

    public class Tools
    {
        public Dictionary<string, Module> modules { get; set; }
        public string Modules { get; set; }

        public Dictionary<string, ToolStrip> regToolstrips { get; set; }

        public Dictionary<string, ToolStrips> np { get; set; }

        public Tools()
        {
            regToolstrips = new Dictionary<string, ToolStrip>();

            np = new Dictionary<string, ToolStrips>();
        }

        public ToolStrip GetToolStrip(string name)
        {
            if (regToolstrips.ContainsKey(name))
                return regToolstrips[name];
            return null;
        }

        public Module GetModule(string name)
        {
            if (modules.ContainsKey(name))
                return modules[name];
            return null;
        }

        public ArrayList GetModuleItems(string name)
        {
            if (modules.ContainsKey(name))
                return modules[name].temp;
            return null;
        }

        public ToolStrips GetToolStrips(string name)
        {
            if (np.ContainsKey(name))
                return np[name];
            return null;
        }

        public void AddToolStrip(string name, ToolStrip ns)
        {
            regToolstrips.Add(name, ns);
        }

        public ToolStrips CreateToolStrips(string name)
        {
            ToolStrips s = new ToolStrips();

            s.LoadToolbar(name);

            np.Add(name, s);

            AddToolStrip(name, s.GetToolStrip());

            return s;
        }

        public ToolStrip LoadToolbar(ToolStrip ts, string module_name)
        {
            ToolStrips s = GetToolStrips(module_name);

            s.LoadToolbar(module_name, ts);

            return s.GetToolStrip();
        }
    }

    public class ToolStrips
    {
        public ToolStrips()
        {
        }

        public ToolStrip GetToolStrip()
        {
            return ns;
        }

        public Module module { get; set; }

        public ToolStrip LoadToolbar(string module_name, ToolStrip np = null)
        {
            ToolStripDropDownItem b = new ToolStripDropDownButton("");

            module = Module.GetModule(module_name);

            this.module = module;

            b.Tag = module;

            ST = new Dictionary<string, ToolStripItem>();

            GT = module.temp;

            DG = new ArrayList();

            if (module != null)
            {
                foreach (string c in module.dict.Keys)
                {
                    Command cmd = module.dict[c] as Command;

                    //ToolStripMenuItem v = b.DropDownItems.Add(append(cmd.Name)) as ToolStripMenuItem;
                    //v.Image = cmd.image;
                    //v.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    //v.Name = cmd.Name;
                    //v.Click += new EventHandler(OptionToolbarHandler);

                    //ToolStripItem dd = null;

                    ToolStripItem r0 = CreateToolStripItem(cmd.Name, cmd.Name, cmd.image, cmd) as ToolStripItem;

                    ToolStripMenuItem r1 = r0 as ToolStripMenuItem;

                    if (cmd.keyboard != null)
                    {
                        if (r1 != null)
                        {
                            Keys k = Keys.None;
                            bool first = true;
                            foreach (Keys g in cmd.keyboard)
                            {
                                k |= g;
                                string p = g.ToString();

                                if (g == Keys.Control)
                                    p = "Ctrl";
                                else if (g == (Keys)109)
                                    p = "-";
                                if (first == false)
                                    p = "+" + p;
                                r1.ShortcutKeyDisplayString += p + " ";
                                first = false;
                            }
                            //v.ShortcutKeys = k;
                            r1.ShortcutKeys = k;
                            //r0.ShortcutKeyDisplayString = v.ShortcutKeyDisplayString;
                        }
                    }
                    if (cmd.Name != "Gui" && cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) == false)
                        DG.Add(r0);
                    if (cmd.Name == "Module")
                    {
                        gui.Command_Module bb = cmd as gui.Command_Module;
                        r0.Text = append(bb.Names);
                        
                        r0.Name = bb.Names;
                        r0.Tag =  bb;
                        Module modules = Module.GetModule(cmd.Module);

                        if (modules != null)
                        {
                            ToolStripSplitButton bc = r0 as ToolStripSplitButton;
                            
                            if(bc != null)
                            foreach (string cc in modules.dict.Keys)
                            {
                                Command cmds = modules.dict[cc] as Command;
                                ToolStripMenuItem vv = new ToolStripMenuItem(cmds.Name) as ToolStripMenuItem;
                                vv.Image = cmds.image;
                                vv.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                                vv.Tag = cmds;
                                vv.Click += R0_Click;


                                if (r1 != null)
                                {
                                    if (cmds.keyboard != null)
                                    {
                                        Keys k = Keys.None;
                                        bool first = true;
                                        foreach (Keys g in cmds.keyboard)
                                        {
                                            k |= g;
                                            string p = g.ToString();

                                            if (g == Keys.Control)
                                                p = "Ctrl";
                                            else if (g == (Keys)109)
                                                p = "-";
                                            if (first == false)
                                                p = "+" + p;
                                            r1.ShortcutKeyDisplayString += p + " ";
                                            first = false;
                                        }
                                        r1.ShortcutKeys = k;
                                    }

                                    r1.Checked = true;
                                }

                                if (bc == null)
                                    continue;
                                bc.DropDown.Items.Add(vv);

                            }
                            
                            
                        }
                    }
                    if (cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) || cmd.Name == "Gui")
                    {
                        gui.Command_Gui bb = cmd as gui.Command_Gui;
                        //v.Text = bb.Names;

                        ToolStripItem p0 = CreateToolComboBox(bb.Names, bb.Names, cmd.image, cmd);

                        bb.Configure(p0 as ToolStripComboBoxes);

                        p0.Text = bb.Names;

                        DG.Add(p0);
                    }
                    else if (cmd.Name == "Launcher")
                    {
                        gui.Command_Launcher bb = cmd as gui.Command_Launcher;
                        r0.Text = bb.Names;
                        r0.Tag = bb;
                    }

                    if (r1 != null)
                    {
                        if (GT.IndexOf(r0.Name) >= 0)
                            r1.Checked = true;
                        else
                            r1.Checked = false;

                        if (GT.IndexOf(r0.Text) >= 0)
                            r1.Checked = true;
                    }
                }

                ToolStrip nr = new ToolStrip();

                Point pc = new Point(900, 0);

                if (np != null)
                {
                    pc = np.Location;
                    np.Tag = np.Tag;
                }

                if (np != null)
                    ExplorerForms.ef.Command_ChangeToolstrip(np, AnchorStyles.Top, true);
                FilterItems(nr, GT, DG);
                d0 = GetCustomize(DG);
                ChekedStates(GT, d0);
                nr.Items.Add(d0);
                nr.Renderer = new WinExplorers.Debuggers.Renderer(new ProfessionalColorTable());
                ExplorerForms.ef.Command_ChangeToolstrip(nr, AnchorStyles.Top, false, pc.X, pc.Y);

                ns = nr;
            }
            return ns;
        }

        private ArrayList DG { get; set; }

        private ToolStripDropDownButton d0 { get; set; }

        public ToolStripMenuItem CreateToolStripDropDown(ToolStripMenuItem v, Command cmds)
        {
            ToolStripMenuItem vv = v.DropDownItems.Add(cmds.Name) as ToolStripMenuItem;
            vv.Image = cmds.image;
            vv.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            if (cmds.keyboard != null)
            {
                Keys k = Keys.None;
                bool first = true;
                foreach (Keys g in cmds.keyboard)
                {
                    k |= g;
                    string p = g.ToString();

                    if (g == Keys.Control)
                        p = "Ctrl";
                    else if (g == (Keys)109)
                        p = "-";
                    if (first == false)
                        p = "+" + p;
                    vv.ShortcutKeyDisplayString += p + " ";
                    first = false;
                }
                vv.ShortcutKeys = k;
            }

            vv.Checked = true;

            return vv;
        }

        public ToolStripItem CreateToolStripItem(string text, string name, Image image, Command cmd)
        {
            ToolStripMenuItem r0 = new ToolStripMenuItem();
            if (cmd.GuiHint == "ToolStripDropDownButton")
            {
                ToolStripSplitButton r1 = new ToolStripSplitButton("", cmd.image);
                r1.DisplayStyle = ToolStripItemDisplayStyle.Image;
                r1.DropDown = new ToolStripDropDown();
                r1.Text = text;
                r1.Name = name;
               // r1.Image = image;
                r1.Tag = cmd;
                r1.Click += R0_Click;
                return r1;
            }
            r0.Text = text;
            r0.Name = name;
            r0.Image = image;
            r0.Tag = cmd;
            r0.Click += R0_Click;
            return r0;
        }

        private void R0_Click(object sender, EventArgs e)
        {
            Command cmd = null;

            ToolStripMenuItem b = sender as ToolStripMenuItem;
            if (b == null)
            {
                ToolStripSplitButton c = sender as ToolStripSplitButton;
                if(c == null)
                    return;

                if (c.DropDownButtonPressed)
                    return;

                if (c.DropDown.Items.Count <= 0)
                    return;
                ToolStripItem d = c.DropDown.Items[0];
                if (d == null)
                    return;

                cmd = d.Tag as Command;
            }
            else 
                cmd = b.Tag as Command;

            if (cmd == null)
                return;
            
            cmd.Execute();
        }

        public ToolStripItem CreateToolComboBox(string text, string name, Image image, Command cmd)
        {
            ToolStripItem r0 = new ToolStripComboBoxes() as ToolStripItem;
            r0.Text = text;
            r0.Name = name;
            r0.Image = image;
            r0.Tag = cmd;
            //r0.Click += R0_Clicked;
            return r0;
        }

        private void R0_Clicked(object sender, EventArgs e)
        {
            ToolStripComboBoxes b = sender as ToolStripComboBoxes;
            if (b == null)
                return;
            gui.Command_Gui cmd = b.Tag as gui.Command_Gui;

            if (cmd == null)
                return;

            // MessageBox.Show("Command will be done " + cmd.Names);
        }

        public void FilterItems(ToolStrip nr, ArrayList G, ArrayList DG)
        {
            nr.ResumeLayout();
            nr.Items.Clear();
            foreach (string name in G)
            {
                if (name == "Separator")
                    nr.Items.Add(new ToolStripSeparator());
                else
                    foreach (ToolStripItem b in DG)
                    {
                        if (b.Text != null)
                            if (b.Text.Trim() == name.Replace("@", ""))
                            {
                                if (name.Contains("@@"))
                                    b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                                else if (name.Contains("@"))
                                    b.DisplayStyle = ToolStripItemDisplayStyle.Text;
                                else b.DisplayStyle = ToolStripItemDisplayStyle.Image;
                                b.Text = b.Text.Trim();
                                nr.Items.Add(b);
                                Command c = b.Tag as Command;
                                if (c == null)
                                    continue;
                                if (c.shouldDisplayText)
                                    b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            }
                    }
            }
        }

        public void ChekedStates(ArrayList G, ToolStripDropDownButton d)
        {
            ToolStripDropDownItem b = GetSuggestions(d);

            foreach (ToolStripItem c in b.DropDownItems)
            {
                ToolStripMenuItem s = c as ToolStripMenuItem;
                if (s != null)
                    s.Checked = false;
            }

            foreach (ToolStripItem p in b.DropDownItems)
            {
                ToolStripMenuItem c = p as ToolStripMenuItem;
                if (c == null)
                    continue;

                foreach (string s in G)
                {
                    if (s == "Separator")
                        continue;
                    if (s.Trim() == c.Text.Trim())
                    {
                        c.Checked = true;
                    }
                }
            }
        }

        public ToolStripDropDownButton GetCustomize(ArrayList DG)
        {
            ToolStripDropDownButton dd = new ToolStripDropDownButton();

            ToolStripDropDownItem b = dd.DropDownItems.Add("Add or Remove Items") as ToolStripDropDownItem;

            foreach (ToolStripItem c in DG)
            {
                if (c.GetType() == typeof(ToolStripSeparator))
                    continue;

                ToolStripMenuItem d = new ToolStripMenuItem();
                d.Text = append(c.Text);
                d.Name = c.Name;
                d.Image = c.Image;
                d.Tag = c.Tag;
                d.Click += OptionToolbarHandlers;
                if (Module.GetTemplateItem(module.Name, c.Text) == false)
                    d.Enabled = false;
                ToolStripMenuItem bb = c as ToolStripMenuItem;
                if (bb != null)
                {
                    d.ShortcutKeys = bb.ShortcutKeys;
                    d.ShortcutKeyDisplayString = bb.ShortcutKeyDisplayString;
                    if (bb.DropDownItems != null)
                        if (bb.DropDownItems.Count > 0)
                        {
                            foreach (ToolStripMenuItem p in bb.DropDownItems)
                            {
                                ToolStripMenuItem dc = new ToolStripMenuItem();
                                dc.Text = append(p.Text);
                                dc.Name = p.Name;
                                dc.Image = p.Image;
                                dc.Tag = p.Tag;
                                d.DropDownItems.Add(dc);
                            }
                        }
                }

                b.DropDownItems.Add(d);
            }

            b.DropDownItems.Add(new ToolStripSeparator());

            ToolStripButton cc = new ToolStripButton("Customize");
            //cc.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            cc.Click += new EventHandler(customize);
            b.DropDownItems.Add(cc);

            cc = new ToolStripButton("Reset toolbox");
            //cc.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.DropDownItems.Add(cc);

            return dd;
        }

        public ToolStripDropDownItem GetSuggestions(ToolStripDropDownButton b)
        {
            if (b.DropDownItems.Count > 0)
                return b.DropDownItems[0] as ToolStripDropDownItem;
            return null;
        }

        public ToolStrip ns { get; set; }

        private void customize(object sender, EventArgs e)
        {
            ExplorerForms ef = ExplorerForms.ef;
            if (ef == null)
                return;

            string name = "Commands-Toolbar-" + module.Name;

            ef.Command_CustomizeDialog(name);
        }

        private void MouseHover(object sender, EventArgs e)
        {
            ToolStripItem b = sender as ToolStripItem;

            if (b == null)
                return;
        }

        public string append(string name)
        {
            return "      " + name;
        }

        public void OptionToolbarHandlers(object sender, EventArgs e)
        {
            ToolStripMenuItem b = sender as ToolStripMenuItem;
            if (b == null)
                return;
            string name = b.Text.Trim();

            int d = GetIndex(GT, DG, name);

            if (b.Checked == true)
            {
                GT.Remove(name);
                b.Checked = false;
            }
            else
            {
                if (d < 0)
                    d = 0;
                GT.Insert(d, name);
                b.Checked = true;
            }

            FilterItems(ns, GT, DG);

            ns.Items.Add(d0);
        }

        private Dictionary<string, ToolStripItem> ST { get; set; }

        private ArrayList GT { get; set; }

        private int GetIndex(string name)
        {
            name = name.Trim();

            int i = GT.IndexOf(name);

            if (i >= 0)
                return i;

            foreach (string b in ST.Keys)
            {
                ToolStripMenuItem d = ST[b] as ToolStripMenuItem;

                int p = GT.IndexOf(d.Name);
                if (p >= 0)
                    i = p;
                if (d.Name == name)
                {
                    return i;
                }
            }
            return 0;
        }

        private int GetIndex(ArrayList GT, ArrayList DG, string name)
        {
            name = name.Trim();

            int i = GT.IndexOf(name);

            if (i >= 0)
                return i;

            foreach (ToolStripItem b in DG)
            {
                ToolStripItem d = b;

                int p = GT.IndexOf(d.Name);
                if (p >= 0)
                {
                    if (d.Text.Trim() == name)
                    {
                        return p;
                    }
                    else
                        i = p;
                }
                if (d.Text != null)
                    if (d.Text.Trim() == name)
                    {
                        return i + 1;
                    }
            }
            return i;
        }

        public void open_file(object sender, EventArgs e)
        {
            OpenFile();
        }

        public void OpenFile()
        {
        }

        public ToolStripItem GetItem(string name)
        {
            if (ns == null)
                return null;
            foreach (ToolStripItem s in ns.Items)
                if (s.Name == name)
                    return s;
            return null;
        }
    }
}