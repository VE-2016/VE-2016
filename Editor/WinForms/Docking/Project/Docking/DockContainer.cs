
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Delegate[@name="DeserializeDockContent"]/*'/>
    public delegate IDockableWindow DeserializeDockContent(string persistString);

    /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/ClassDef/*'/>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    [ToolboxBitmap(typeof(DockContainer), "Resources.DockPanel.bmp")]
    public class DockContainer : Panel
    {
        private const int WM_REFRESHACTIVEWINDOW = (int)Win32.Msgs.WM_USER + 1;

        private LocalWindowsHook _localWindowsHook;

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Constructor[@name="()"]/*'/>
        public DockContainer()
        {
            _extender = new DockPanelExtender(this);
            _dragHandler = new DragHandler(this);
            _panes = new DockPaneCollection();
            _floatWindows = new FloatWindowCollection();

            SetStyle(ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint, true);

            SuspendLayout();
            Font = SystemInformation.MenuFont;

            _autoHideWindow = new AutoHideWindow(this);
            _autoHideWindow.Visible = false;

            if (Environment.Version.Major == 1)
            {
                _dummyControl = new DummyControl();
                _dummyControl.Bounds = Rectangle.Empty;
                Controls.Add(_dummyControl);
            }

            _dockWindows = new DockWindowCollection(this);
            Controls.AddRange(new Control[] {
                DockWindows[DockState.Document],
                DockWindows[DockState.DockLeft],
                DockWindows[DockState.DockRight],
                DockWindows[DockState.DockTop],
                DockWindows[DockState.DockBottom]
                });

            _localWindowsHook = new LocalWindowsHook(HookType.WH_CALLWNDPROCRET);
            _localWindowsHook.HookInvoked += new LocalWindowsHook.HookEventHandler(this.HookEventHandler);
            _localWindowsHook.Install();

            _dummyContent = new DockableWindow();
            ResumeLayout();
        }

        private AutoHideStripBase _autoHideStripControl = null;
        private AutoHideStripBase AutoHideStripControl
        {
            get
            {
                if (_autoHideStripControl == null)
                {
                    _autoHideStripControl = AutoHideStripFactory.CreateAutoHideStrip(this);
                    Controls.Add(_autoHideStripControl);
                }
                return _autoHideStripControl;
            }
        }

        private MdiClientController _mdiClientController = null;
        private MdiClientController MdiClientController
        {
            get
            {
                if (_mdiClientController == null)
                {
                    _mdiClientController = new MdiClientController();
                    _mdiClientController.HandleAssigned += new EventHandler(MdiClientHandleAssigned);
                    _mdiClientController.MdiChildActivate += new EventHandler(ParentFormMdiChildActivate);
                    _mdiClientController.Layout += new LayoutEventHandler(MdiClient_Layout);
                }

                return _mdiClientController;
            }
        }

        private void MdiClientHandleAssigned(object sender, EventArgs e)
        {
            SetMdiClient();
            PerformLayout();
        }

        private void MdiClient_Layout(object sender, LayoutEventArgs e)
        {
            if (DocumentStyle != DocumentStyles.DockingMdi)
                return;

            foreach (DockPane pane in Panes)
                if (pane.DockState == DockState.Document)
                    pane.SetContentBounds();

            UpdateWindowRegion();
        }

        private void ParentFormMdiChildActivate(object sender, EventArgs e)
        {
            if (MdiClientController.ParentForm == null)
                return;

            IDockableWindow content = MdiClientController.ParentForm.ActiveMdiChild as IDockableWindow;
            if (content == null)
                return;

            if (content.DockHandler.DockPanel == this && content.DockHandler.Pane != null)
                content.DockHandler.Pane.ActiveContent = content;
        }

        private bool _inRefreshingActiveWindow = false;
        internal bool InRefreshingActiveWindow
        {
            get { return _inRefreshingActiveWindow; }
            set { _inRefreshingActiveWindow = value; }
        }

        // Windows hook event handler
        private void HookEventHandler(object sender, HookEventArgs e)
        {
            if (InRefreshingActiveWindow)
                return;

            Win32.Msgs msg = (Win32.Msgs)Marshal.ReadInt32(e.lParam, IntPtr.Size * 3);

            if (msg == Win32.Msgs.WM_KILLFOCUS)
            {
                IntPtr wParam = Marshal.ReadIntPtr(e.lParam, IntPtr.Size * 2);
                DockPane pane = GetPaneFromHandle(wParam);
                if (pane == null)
                    User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
            }
            else if (msg == Win32.Msgs.WM_SETFOCUS)
                User32.PostMessage(this.Handle, WM_REFRESHACTIVEWINDOW, 0, 0);
        }

        private bool _disposed = false;
        /// <exclude/>
        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!_disposed && disposing)
                {
                    _localWindowsHook.Uninstall();
                    if (_mdiClientController != null)
                    {
                        _mdiClientController.HandleAssigned -= new EventHandler(MdiClientHandleAssigned);
                        _mdiClientController.MdiChildActivate -= new EventHandler(ParentFormMdiChildActivate);
                        _mdiClientController.Layout -= new LayoutEventHandler(MdiClient_Layout);
                        _mdiClientController.Dispose();
                    }
                    FloatWindows.Dispose();
                    Panes.Dispose();
                    DummyContent.Dispose();

                    _disposed = true;
                }

                base.Dispose(disposing);
            }
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveAutoHideContent"]/*' />
        [Browsable(false)]
        public IDockableWindow ActiveAutoHideContent
        {
            get { return AutoHideWindow.ActiveContent; }
            set { AutoHideWindow.ActiveContent = value; }
        }

        private IDockableWindow _activeContent = null;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveContent"]/*' />
        [Browsable(false)]
        public IDockableWindow ActiveContent
        {
            get { return _activeContent; }
        }
        internal void SetActiveContent()
        {
            IDockableWindow value = ActivePane == null ? null : ActivePane.ActiveContent;

            if (_activeContent == value)
                return;

            if (_activeContent != null)
                _activeContent.DockHandler.SetIsActivated(false);

            _activeContent = value;

            if (_activeContent != null)
                _activeContent.DockHandler.SetIsActivated(true);

            OnActiveContentChanged(EventArgs.Empty);
        }

        private DockPane _activePane = null;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActivePane"]/*' />
        [Browsable(false)]
        public DockPane ActivePane
        {
            get { return _activePane; }
        }
        private void SetActivePane()
        {
            DockPane value = GetPaneFromHandle(User32.GetFocus());
            if (_activePane == value)
                return;

            if (_activePane != null)
                _activePane.SetIsActivated(false);

            _activePane = value;

            if (_activePane != null)
                _activePane.SetIsActivated(true);
        }
        private DockPane GetPaneFromHandle(IntPtr hWnd)
        {
            Control control = Control.FromChildHandle(hWnd);

            IDockableWindow content = null;
            DockPane pane = null;
            for (; control != null; control = control.Parent)
            {
                content = control as IDockableWindow;
                if (content != null)
                    content.DockHandler.ActiveWindowHandle = hWnd;

                if (content != null && content.DockHandler.DockPanel == this)
                    return content.DockHandler.Pane;

                pane = control as DockPane;
                if (pane != null && pane.DockPanel == this)
                    break;
            }

            return pane;
        }

        private IDockableWindow _activeDocument = null;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveDocument"]/*' />
        [Browsable(false)]
        public IDockableWindow ActiveDocument
        {
            get { return _activeDocument; }
        }
        private void SetActiveDocument()
        {
            IDockableWindow value = ActiveDocumentPane == null ? null : ActiveDocumentPane.ActiveContent;

            if (_activeDocument == value)
                return;

            _activeDocument = value;

            OnActiveDocumentChanged(EventArgs.Empty);
        }

        private DockPane _activeDocumentPane = null;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ActiveDocumentPane"]/*' />
        [Browsable(false)]
        public DockPane ActiveDocumentPane
        {
            get { return _activeDocumentPane; }
        }
        private void SetActiveDocumentPane()
        {
            DockPane value = null;

            if (ActivePane != null && ActivePane.DockState == DockState.Document)
                value = ActivePane;

            if (value == null)
            {
                if (ActiveDocumentPane == null)
                    value = DockWindows[DockState.Document].DefaultPane;
                else if (ActiveDocumentPane.DockPanel != this || ActiveDocumentPane.DockState != DockState.Document)
                    value = DockWindows[DockState.Document].DefaultPane;
                else
                    value = _activeDocumentPane;
            }

            if (_activeDocumentPane == value)
                return;

            if (_activeDocumentPane != null)
                _activeDocumentPane.SetIsActiveDocumentPane(false);

            _activeDocumentPane = value;

            if (_activeDocumentPane != null)
                _activeDocumentPane.SetIsActiveDocumentPane(true);
        }

        private bool _allowRedocking = true;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="AllowRedocking"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.AllowRedocking.Description")]
        [DefaultValue(true)]
        public bool AllowRedocking
        {
            get { return _allowRedocking; }
            set { _allowRedocking = value; }
        }

        private AutoHideWindow _autoHideWindow;
        internal AutoHideWindow AutoHideWindow
        {
            get { return _autoHideWindow; }
        }

        private DockContentCollection _contents = new DockContentCollection();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Contents"]/*' />
        [Browsable(false)]
        public DockContentCollection Contents
        {
            get { return _contents; }
        }

        private DockableWindow _dummyContent;
        internal DockableWindow DummyContent
        {
            get { return _dummyContent; }
        }

        private bool _showDocumentIcon = false;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="ShowDocumentIcon"]/*' />
        [DefaultValue(false)]
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.ShowDocumentIcon.Description")]
        public bool ShowDocumentIcon
        {
            get { return _showDocumentIcon; }
            set
            {
                if (_showDocumentIcon == value)
                    return;

                _showDocumentIcon = value;
                Refresh();
            }
        }

        private DockPanelExtender _extender;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Extender"]/*' />
        [Browsable(false)]
        public DockPanelExtender Extender
        {
            get { return _extender; }
        }

        internal DockPanelExtender.IDockPaneFactory DockPaneFactory
        {
            get { return Extender.DockPaneFactory; }
        }

        internal DockPanelExtender.IFloatWindowFactory FloatWindowFactory
        {
            get { return Extender.FloatWindowFactory; }
        }

        internal DockPanelExtender.IDockPaneCaptionFactory DockPaneCaptionFactory
        {
            get { return Extender.DockPaneCaptionFactory; }
        }

        internal DockPanelExtender.IDockPaneTabFactory DockPaneTabFactory
        {
            get { return Extender.DockPaneTabFactory; }
        }

        internal DockPanelExtender.IDockPaneStripFactory DockPaneStripFactory
        {
            get { return Extender.DockPaneStripFactory; }
        }

        internal DockPanelExtender.IAutoHideTabFactory AutoHideTabFactory
        {
            get { return Extender.AutoHideTabFactory; }
        }

        internal DockPanelExtender.IAutoHidePaneFactory AutoHidePaneFactory
        {
            get { return Extender.AutoHidePaneFactory; }
        }

        internal DockPanelExtender.IAutoHideStripFactory AutoHideStripFactory
        {
            get { return Extender.AutoHideStripFactory; }
        }

        private DockPaneCollection _panes;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Panes"]/*' />
        [Browsable(false)]
        public DockPaneCollection Panes
        {
            get { return _panes; }
        }

        internal Rectangle DockArea
        {
            get
            {
                return new Rectangle(DockPadding.Left, DockPadding.Top,
                    ClientRectangle.Width - DockPadding.Left - DockPadding.Right,
                    ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom);
            }
        }

        private double _dockBottomPortion = 0.25;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockBottomPortion"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.DockBottomPortion.Description")]
        [DefaultValue(0.25)]
        public double DockBottomPortion
        {
            get { return _dockBottomPortion; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException();

                if (value == _dockBottomPortion)
                    return;

                _dockBottomPortion = value;

                if (_dockTopPortion + _dockBottomPortion > 1)
                    _dockTopPortion = 1 - _dockBottomPortion;

                PerformLayout();
            }
        }

        private double _dockLeftPortion = 0.25;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockLeftPortion"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.DockLeftPortion.Description")]
        [DefaultValue(0.25)]
        public double DockLeftPortion
        {
            get { return _dockLeftPortion; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException();

                if (value == _dockLeftPortion)
                    return;

                _dockLeftPortion = value;

                if (_dockLeftPortion + _dockRightPortion > 1)
                    _dockRightPortion = 1 - _dockLeftPortion;

                PerformLayout();
            }
        }

        private double _dockRightPortion = 0.25;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockRightPortion"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.DockRightPortion.Description")]
        [DefaultValue(0.25)]
        public double DockRightPortion
        {
            get { return _dockRightPortion; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException();

                if (value == _dockRightPortion)
                    return;

                _dockRightPortion = value;

                if (_dockLeftPortion + _dockRightPortion > 1)
                    _dockLeftPortion = 1 - _dockRightPortion;

                PerformLayout();
            }
        }

        private double _dockTopPortion = 0.25;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockTopPortion"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.DockTopPortion.Description")]
        [DefaultValue(0.25)]
        public double DockTopPortion
        {
            get { return _dockTopPortion; }
            set
            {
                if (value <= 0 || value >= 1)
                    throw new ArgumentOutOfRangeException();

                if (value == _dockTopPortion)
                    return;

                _dockTopPortion = value;

                if (_dockTopPortion + _dockBottomPortion > 1)
                    _dockBottomPortion = 1 - _dockTopPortion;

                PerformLayout();
            }
        }

        private DockWindowCollection _dockWindows;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DockWindows"]/*' />
        [Browsable(false)]
        public DockWindowCollection DockWindows
        {
            get { return _dockWindows; }
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="Documents"]/*' />
        [Browsable(false)]
        public IDockableWindow[] Documents
        {
            get { return Contents.Select(DockAreas.Document); }
        }

        private Rectangle DocumentRectangle
        {
            get
            {
                Rectangle rect = DockArea;
                if (DockWindows[DockState.DockLeft].DisplayingList.Count != 0)
                {
                    rect.X += (int)(DockArea.Width * DockLeftPortion);
                    rect.Width -= (int)(DockArea.Width * DockLeftPortion);
                }
                if (DockWindows[DockState.DockRight].DisplayingList.Count != 0)
                    rect.Width -= (int)(DockArea.Width * DockRightPortion);
                if (DockWindows[DockState.DockTop].DisplayingList.Count != 0)
                {
                    rect.Y += (int)(DockArea.Height * DockTopPortion);
                    rect.Height -= (int)(DockArea.Height * DockTopPortion);
                }
                if (DockWindows[DockState.DockBottom].DisplayingList.Count != 0)
                    rect.Height -= (int)(DockArea.Height * DockBottomPortion);

                return rect;
            }
        }

        private DragHandler _dragHandler;
        internal DragHandler DragHandler
        {
            get { return _dragHandler; }
        }

        private Control _dummyControl = null;
        internal Control DummyControl
        {
            get { return _dummyControl; }
        }

        private FloatWindowCollection _floatWindows;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="FloatWindows"]/*' />
        [Browsable(false)]
        public FloatWindowCollection FloatWindows
        {
            get { return _floatWindows; }
        }

        private DocumentStyles _documentStyle = DocumentStyles.DockingMdi;
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Property[@name="DocumentStyle"]/*' />
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockPanel.DocumentStyle.Description")]
        [DefaultValue(DocumentStyles.DockingMdi)]
        public DocumentStyles DocumentStyle
        {
            get { return _documentStyle; }
            set
            {
                if (value == _documentStyle)
                    return;

                if (!Enum.IsDefined(typeof(DocumentStyles), value))
                    throw new InvalidEnumArgumentException();

                if (value == DocumentStyles.SystemMdi && DockWindows[DockState.Document].DisplayingList.Count > 0)
                    throw new InvalidEnumArgumentException();

                _documentStyle = value;

                SuspendLayout(true);

                SetMdiClient();
                UpdateWindowRegion();

                foreach (IDockableWindow content in Contents)
                {
                    if (content.DockHandler.DockState == DockState.Document)
                    {
                        content.DockHandler.SetPane(content.DockHandler.Pane);
                        content.DockHandler.SetVisible();
                    }
                }

                if (MdiClientController.MdiClient != null)
                    MdiClientController.MdiClient.PerformLayout();

                ResumeLayout(true, true);
            }
        }

        /// <exclude/>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            SuspendLayout(true);

            AutoHideStripControl.Bounds = ClientRectangle;

            CalculateDockPadding();

            int width = ClientRectangle.Width - DockPadding.Left - DockPadding.Right;
            int height = ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom;
            int dockLeftSize = (int)(width * _dockLeftPortion);
            int dockRightSize = (int)(width * _dockRightPortion);
            int dockTopSize = (int)(height * _dockTopPortion);
            int dockBottomSize = (int)(height * _dockBottomPortion);

            if (dockLeftSize < MeasurePane.MinSize)
                dockLeftSize = MeasurePane.MinSize;
            if (dockRightSize < MeasurePane.MinSize)
                dockRightSize = MeasurePane.MinSize;
            if (dockTopSize < MeasurePane.MinSize)
                dockTopSize = MeasurePane.MinSize;
            if (dockBottomSize < MeasurePane.MinSize)
                dockBottomSize = MeasurePane.MinSize;

            DockWindows[DockState.DockLeft].Width = dockLeftSize;
            DockWindows[DockState.DockRight].Width = dockRightSize;
            DockWindows[DockState.DockTop].Height = dockTopSize;
            DockWindows[DockState.DockBottom].Height = dockBottomSize;

            AutoHideWindow.Bounds = AutoHideWindowBounds;

            DockWindows[DockState.Document].BringToFront();
            AutoHideWindow.BringToFront();

            base.OnLayout(levent);

            if (DocumentStyle == DocumentStyles.SystemMdi && MdiClientController.MdiClient != null)
            {
                MdiClientController.MdiClient.Bounds = SystemMdiClientBounds;
                UpdateWindowRegion();
            }

            if (Parent != null)
                Parent.ResumeLayout();

            ResumeLayout(true, true);
        }

        internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            return AutoHideStripControl.GetTabStripRectangle(dockState);
        }

        /// <exclude/>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);
        }

        internal void AddContent(IDockableWindow content)
        {
            if (content == null)
                throw (new ArgumentNullException());

            if (!Contents.Contains(content))
            {
                Contents.Add(content);
                OnContentAdded(new DockContentEventArgs(content));
            }
        }

        internal void AddPane(DockPane pane)
        {
            if (Panes.Contains(pane))
                return;

            Panes.Add(pane);
        }

        internal void AddFloatWindow(FloatWindow floatWindow)
        {
            if (FloatWindows.Contains(floatWindow))
                return;

            FloatWindows.Add(floatWindow);
        }

        private void CalculateDockPadding()
        {
            DockPadding.All = 0;

            int height = AutoHideStripControl.MeasureHeight();

            if (AutoHideStripControl.GetPanes(DockState.DockLeftAutoHide).Count > 0)
                DockPadding.Left = height;
            if (AutoHideStripControl.GetPanes(DockState.DockRightAutoHide).Count > 0)
                DockPadding.Right = height;
            if (AutoHideStripControl.GetPanes(DockState.DockTopAutoHide).Count > 0)
                DockPadding.Top = height;
            if (AutoHideStripControl.GetPanes(DockState.DockBottomAutoHide).Count > 0)
                DockPadding.Bottom = height;
        }

        internal Rectangle AutoHideWindowBounds
        {
            get
            {
                DockState state = AutoHideWindow.DockState;
                Rectangle rectDockArea = DockArea;
                if (ActiveAutoHideContent == null)
                    return Rectangle.Empty;

                if (Parent == null)
                    return Rectangle.Empty;

                Rectangle rect = Rectangle.Empty;
                if (state == DockState.DockLeftAutoHide)
                {
                    int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.DockHandler.AutoHidePortion);
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = autoHideSize;
                    rect.Height = rectDockArea.Height;
                }
                else if (state == DockState.DockRightAutoHide)
                {
                    int autoHideSize = (int)(rectDockArea.Width * ActiveAutoHideContent.DockHandler.AutoHidePortion);
                    rect.X = rectDockArea.X + rectDockArea.Width - autoHideSize;
                    rect.Y = rectDockArea.Y;
                    rect.Width = autoHideSize;
                    rect.Height = rectDockArea.Height;
                }
                else if (state == DockState.DockTopAutoHide)
                {
                    int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.DockHandler.AutoHidePortion);
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = rectDockArea.Width;
                    rect.Height = autoHideSize;
                }
                else if (state == DockState.DockBottomAutoHide)
                {
                    int autoHideSize = (int)(rectDockArea.Height * ActiveAutoHideContent.DockHandler.AutoHidePortion);
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y + rectDockArea.Height - autoHideSize;
                    rect.Width = rectDockArea.Width;
                    rect.Height = autoHideSize;
                }

                if (Parent == null)
                    return Rectangle.Empty;
                else
                    return new Rectangle(Parent.PointToClient(PointToScreen(rect.Location)), rect.Size);
            }
        }

        internal void RefreshActiveWindow()
        {
            InRefreshingActiveWindow = true;
            SetActivePane();
            SetActiveContent();
            SetActiveDocumentPane();
            SetActiveDocument();
            AutoHideWindow.RefreshActivePane();
            InRefreshingActiveWindow = false;
        }

        internal void ReCreateAutoHideStripControl()
        {
            if (_autoHideStripControl != null)
            {
                _autoHideStripControl.Dispose();
                _autoHideStripControl = null;
            }
        }

        internal void RefreshAutoHideStrip()
        {
            AutoHideStripControl.RefreshChanges();
        }

        private void OnChangingDocumentStyle(DocumentStyles oldValue)
        {
        }

        public void RemoveContent(IDockableWindow content)
        {
            if (content == null)
                throw (new ArgumentNullException());

            if (Contents.Contains(content))
            {
                Contents.Remove(content);
                OnContentRemoved(new DockContentEventArgs(content));
            }
        }

        public void RemovePanes()
        {
            ArrayList L = new ArrayList();

            Panes.Clear();

            //foreach (DockPane d in Panes)
            //{
            //    L.Add(d);

            //}

            //foreach (DockPane d in L)
            //{
            //  Panes.Remove(d);

            //}
        }

        public void RemovePane(DockPane pane)
        {
            if (!Panes.Contains(pane))
                return;

            Panes.Remove(pane);
        }

        internal void RemoveFloatWindow(FloatWindow floatWindow)
        {
            if (!FloatWindows.Contains(floatWindow))
                return;

            FloatWindows.Remove(floatWindow);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SetPaneIndex(DockPane, int)"]/*'/>
        public void SetPaneIndex(DockPane pane, int index)
        {
            int oldIndex = Panes.IndexOf(pane);
            if (oldIndex == -1)
                throw (new ArgumentException(ResourceHelper.GetString("DockPanel.SetPaneIndex.InvalidPane")));

            if (index < 0 || index > Panes.Count - 1)
                if (index != -1)
                    throw (new ArgumentOutOfRangeException(ResourceHelper.GetString("DockPanel.SetPaneIndex.InvalidIndex")));

            if (oldIndex == index)
                return;
            if (oldIndex == Panes.Count - 1 && index == -1)
                return;

            Panes.Remove(pane);
            if (index == -1)
                Panes.Add(pane);
            else if (oldIndex < index)
                Panes.AddAt(pane, index - 1);
            else
                Panes.AddAt(pane, index);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SuspendLayout(bool)"]/*'/>
        public void SuspendLayout(bool allWindows)
        {
            SuspendLayout();
            if (allWindows)
            {
                AutoHideWindow.SuspendLayout();

                if (MdiClientController.MdiClient != null)
                    MdiClientController.MdiClient.SuspendLayout();

                foreach (DockWindow dockWindow in DockWindows)
                    dockWindow.SuspendLayout();
            }
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="ResumeLayout(bool, bool)"]/*'/>
        public void ResumeLayout(bool performLayout, bool allWindows)
        {
            ResumeLayout(performLayout);
            if (allWindows)
            {
                AutoHideWindow.ResumeLayout(performLayout);

                foreach (DockWindow dockWindow in DockWindows)
                    dockWindow.ResumeLayout(performLayout);

                if (MdiClientController.MdiClient != null)
                    MdiClientController.MdiClient.ResumeLayout(performLayout);
            }
        }

        /// <exclude/>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_REFRESHACTIVEWINDOW)
                RefreshActiveWindow();
            else if (m.Msg == (int)Win32.Msgs.WM_WINDOWPOSCHANGING)
            {
                int offset = (int)Marshal.OffsetOf(typeof(Win32.WINDOWPOS), "flags");
                int flags = Marshal.ReadInt32(m.LParam, offset);
                Marshal.WriteInt32(m.LParam, offset, flags | (int)Win32.FlagsSetWindowPos.SWP_NOCOPYBITS);
            }

            base.WndProc(ref m);
        }

        internal Form ParentForm
        {
            get
            {
                if (!IsParentFormValid())
                    throw new InvalidOperationException();

                return MdiClientController.ParentForm;
            }
        }

        private bool IsParentFormValid()
        {
            if (DocumentStyle == DocumentStyles.DockingSdi || DocumentStyle == DocumentStyles.DockingWindow)
                return true;

            return (MdiClientController.MdiClient != null);
        }

        /// <exclude/>
        protected override void OnParentChanged(EventArgs e)
        {
            AutoHideWindow.Parent = this.Parent;
            MdiClientController.ParentForm = (this.Parent as Form);
            AutoHideWindow.BringToFront();
            base.OnParentChanged(e);
        }

        /// <exclude/>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
                SetMdiClient();
        }

        private Rectangle SystemMdiClientBounds
        {
            get
            {
                if (!IsParentFormValid() || !Visible)
                    return Rectangle.Empty;

                Point location = ParentForm.PointToClient(PointToScreen(DocumentWindowBounds.Location));
                Size size = DocumentWindowBounds.Size;
                return new Rectangle(location, size);
            }
        }

        internal Rectangle DocumentWindowBounds
        {
            get
            {
                Rectangle rectDocumentBounds = DisplayRectangle;
                if (DockWindows[DockState.DockLeft].Visible)
                {
                    rectDocumentBounds.X += DockWindows[DockState.DockLeft].Width;
                    rectDocumentBounds.Width -= DockWindows[DockState.DockLeft].Width;
                }
                if (DockWindows[DockState.DockRight].Visible)
                    rectDocumentBounds.Width -= DockWindows[DockState.DockRight].Width;
                if (DockWindows[DockState.DockTop].Visible)
                {
                    rectDocumentBounds.Y += DockWindows[DockState.DockTop].Height;
                    rectDocumentBounds.Height -= DockWindows[DockState.DockTop].Height;
                }
                if (DockWindows[DockState.DockBottom].Visible)
                    rectDocumentBounds.Height -= DockWindows[DockState.DockBottom].Height;

                return rectDocumentBounds;
            }
        }

        private void SetMdiClient()
        {
            if (this.DocumentStyle == DocumentStyles.DockingMdi)
            {
                MdiClientController.AutoScroll = false;
                MdiClientController.BorderStyle = BorderStyle.None;
                if (MdiClientController.MdiClient != null)
                    MdiClientController.MdiClient.Dock = DockStyle.Fill;
            }
            else if (DocumentStyle == DocumentStyles.DockingSdi || DocumentStyle == DocumentStyles.DockingWindow)
            {
                MdiClientController.AutoScroll = true;
                MdiClientController.BorderStyle = BorderStyle.Fixed3D;
                if (MdiClientController.MdiClient != null)
                    MdiClientController.MdiClient.Dock = DockStyle.Fill;
            }
            else if (this.DocumentStyle == DocumentStyles.SystemMdi)
            {
                MdiClientController.AutoScroll = true;
                MdiClientController.BorderStyle = BorderStyle.Fixed3D;
                if (MdiClientController.MdiClient != null)
                {
                    MdiClientController.MdiClient.Dock = DockStyle.None;
                    MdiClientController.MdiClient.Bounds = SystemMdiClientBounds;
                }
            }
        }

        private void UpdateWindowRegion()
        {
            if (DesignMode)
                return;

            if (this.DocumentStyle == DocumentStyles.DockingMdi)
                UpdateWindowRegion_ClipContent();
            else if (this.DocumentStyle == DocumentStyles.DockingSdi ||
                this.DocumentStyle == DocumentStyles.DockingWindow)
                UpdateWindowRegion_FullDocumentArea();
            else if (this.DocumentStyle == DocumentStyles.SystemMdi)
                UpdateWindowRegion_EmptyDocumentArea();
        }

        private void UpdateWindowRegion_FullDocumentArea()
        {
            SetRegion(null);
        }

        private void UpdateWindowRegion_EmptyDocumentArea()
        {
            Rectangle rect = DocumentWindowBounds;
            SetRegion(new Rectangle[] { rect });
        }

        private void UpdateWindowRegion_ClipContent()
        {
            int count = 0;
            foreach (DockPane pane in this.Panes)
            {
                if (pane.DockState != DockState.Document)
                    continue;

                count++;
            }

            Rectangle[] rects = new Rectangle[count];
            int i = 0;
            foreach (DockPane pane in this.Panes)
            {
                if (pane.DockState != DockState.Document)
                    continue;

                Size size = pane.ContentRectangle.Size;
                Point location = PointToClient(pane.PointToScreen(pane.ContentRectangle.Location));
                rects[i] = new Rectangle(location, size);
                i++;
            }

            SetRegion(rects);
        }

        private Rectangle[] _clipRects = null;
        private void SetRegion(Rectangle[] clipRects)
        {
            if (!IsClipRectsChanged(clipRects))
                return;

            _clipRects = clipRects;

            if (_clipRects == null || _clipRects.GetLength(0) == 0)
                Region = null;
            else
            {
                Region region = new Region(new Rectangle(0, 0, this.Width, this.Height));
                foreach (Rectangle rect in _clipRects)
                    region.Exclude(rect);
                Region = region;
            }
        }

        private bool IsClipRectsChanged(Rectangle[] clipRects)
        {
            if (clipRects == null && _clipRects == null)
                return false;
            else if ((clipRects == null) != (_clipRects == null))
                return true;

            foreach (Rectangle rect in clipRects)
            {
                bool matched = false;
                foreach (Rectangle rect2 in _clipRects)
                {
                    if (rect == rect2)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    return true;
            }

            foreach (Rectangle rect2 in _clipRects)
            {
                bool matched = false;
                foreach (Rectangle rect in clipRects)
                {
                    if (rect == rect2)
                    {
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    return true;
            }
            return false;
        }

        internal Point PointToMdiClient(Point p)
        {
            if (MdiClientController.MdiClient == null)
                return Point.Empty;
            else
                return MdiClientController.MdiClient.PointToClient(p);
        }

        private static readonly object s_activeDocumentChangedEvent = new object();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActiveDocumentChanged"]/*' />
        [LocalizedCategory("Category.PropertyChanged")]
        [LocalizedDescription("DockPanel.ActiveDocumentChanged.Description")]
        public event EventHandler ActiveDocumentChanged
        {
            add { Events.AddHandler(s_activeDocumentChangedEvent, value); }
            remove { Events.RemoveHandler(s_activeDocumentChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActiveDocumentChanged(EventArgs)"]/*' />
        protected virtual void OnActiveDocumentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activeDocumentChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_activeContentChangedEvent = new object();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActiveContentChanged"]/*' />
        [LocalizedCategory("Category.PropertyChanged")]
        [LocalizedDescription("DockPanel.ActiveContentChanged.Description")]
        public event EventHandler ActiveContentChanged
        {
            add { Events.AddHandler(s_activeContentChangedEvent, value); }
            remove { Events.RemoveHandler(s_activeContentChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActiveContentChanged(EventArgs)"]/*' />
        protected virtual void OnActiveContentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activeContentChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_activePaneChangedEvent = new object();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ActivePaneChanged"]/*' />
        [LocalizedCategory("Category.PropertyChanged")]
        [LocalizedDescription("DockPanel.ActivePaneChanged.Description")]
        public event EventHandler ActivePaneChanged
        {
            add { Events.AddHandler(s_activePaneChangedEvent, value); }
            remove { Events.RemoveHandler(s_activePaneChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnActivePaneChanged(EventArgs)"]/*' />
        protected virtual void OnActivePaneChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activePaneChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Delegate[@name="DockContentEventHandler"]/*'/>
        public delegate void DockContentEventHandler(object sender, DockContentEventArgs e);
        private static readonly object s_contentAddedEvent = new object();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ContentAdded"]/*' />
        [LocalizedCategory("Category.DockingNotification")]
        [LocalizedDescription("DockPanel.ContentAdded.Description")]
        public event DockContentEventHandler ContentAdded
        {
            add { Events.AddHandler(s_contentAddedEvent, value); }
            remove { Events.RemoveHandler(s_contentAddedEvent, value); }
        }
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnContentAdded(DockContentEventArgs)"]/*' />
        protected virtual void OnContentAdded(DockContentEventArgs e)
        {
            DockContentEventHandler handler = (DockContentEventHandler)Events[s_contentAddedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_contentRemovedEvent = new object();
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Event[@name="ContentRemoved"]/*' />
        [LocalizedCategory("Category.DockingNotification")]
        [LocalizedDescription("DockPanel.ContentRemoved.Description")]
        public event DockContentEventHandler ContentRemoved
        {
            add { Events.AddHandler(s_contentRemovedEvent, value); }
            remove { Events.RemoveHandler(s_contentRemovedEvent, value); }
        }
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="OnContentRemoved(DockContentEventArgs)"]/*' />
        protected virtual void OnContentRemoved(DockContentEventArgs e)
        {
            DockContentEventHandler handler = (DockContentEventHandler)Events[s_contentRemovedEvent];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml"]/*'/>
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(string)"]/*'/>
        public void SaveAsXml(string filename)
        {
            DockPanelPersist.SaveAsXml(this, filename);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(string, Encoding)"]/*'/>
        public void SaveAsXml(string filename, Encoding encoding)
        {
            DockPanelPersist.SaveAsXml(this, filename, encoding);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(Stream, Encoding)"]/*'/>
        public void SaveAsXml(Stream stream, Encoding encoding)
        {
            DockPanelPersist.SaveAsXml(this, stream, encoding);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="SaveAsXml(Stream, Encoding, bool)"]/*'/>
        public void SaveAsXml(Stream stream, Encoding encoding, bool upstream)
        {
            DockPanelPersist.SaveAsXml(this, stream, encoding, upstream);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml"]/*'/>
        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(string, DeserializeDockContent)"]/*'/>
        public void LoadFromXml(string filename, DeserializeDockContent deserializeContent)
        {
            DockPanelPersist.LoadFromXml(this, filename, deserializeContent);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(Stream, DeserializeDockContent)"]/*'/>
        public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent)
        {
            DockPanelPersist.LoadFromXml(this, stream, deserializeContent);
        }

        /// <include file='CodeDoc\DockPanel.xml' path='//CodeDoc/Class[@name="DockPanel"]/Method[@name="LoadFromXml(Stream, DeserializeDockContent, bool)"]/*'/>
        public void LoadFromXml(Stream stream, DeserializeDockContent deserializeContent, bool closeStream)
        {
            DockPanelPersist.LoadFromXml(this, stream, deserializeContent, closeStream);
        }
    }
}