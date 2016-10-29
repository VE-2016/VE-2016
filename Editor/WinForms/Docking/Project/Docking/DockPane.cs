
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/ClassDef/*'/>
    [ToolboxItem(false)]
    public class DockPane : UserControl
    {
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/EnumDef/*'/>
        public enum AppearanceStyle
        {
            /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/Member[@name="ToolWindow"]/*'/>
            ToolWindow,
            /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/Member[@name="Document"]/*'/>
            Document
        }

        private enum HitTestArea
        {
            Caption,
            TabStrip,
            Content,
            None
        }

        private struct HitTestResult
        {
            public HitTestArea HitArea;
            public int Index;

            public HitTestResult(HitTestArea hitTestArea)
            {
                HitArea = hitTestArea;
                Index = -1;
            }

            public HitTestResult(HitTestArea hitTestArea, int index)
            {
                HitArea = hitTestArea;
                Index = index;
            }
        }

        public void DoResize()
        {
            this.OnResize(EventArgs.Empty);
        }

        private DockPaneCaptionBase _captionControl;
        private DockPaneCaptionBase CaptionControl
        {
            get { return _captionControl; }
        }

        private DockPaneStripBase _tabStripControl;
        public DockPaneStripBase TabStripControl
        {
            get { return _tabStripControl; }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="Overloads"]/*'/>
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(IDockContent, DockState, bool)"]/*'/>
        public DockPane(IDockableWindow content, DockState visibleState, bool show)
        {
            InternalConstruct(content, visibleState, false, Rectangle.Empty, null, DockAlignment.Right, 0.5, show);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(IDockContent, FloatWindow, bool)"]/*'/>
        public DockPane(IDockableWindow content, FloatWindow floatWindow, bool show)
        {
            InternalConstruct(content, DockState.Float, false, Rectangle.Empty, floatWindow.DockList.GetDefaultPrevPane(this), DockAlignment.Right, 0.5, show);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(IDockContent, DockPane, DockAlignment, double, bool)"]/*'/>
        public DockPane(IDockableWindow content, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
        {
            if (prevPane == null)
                throw (new ArgumentNullException());
            InternalConstruct(content, prevPane.DockState, false, Rectangle.Empty, prevPane, alignment, proportion, show);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(IDockContent, Rectangle, bool)"]/*'/>
        public DockPane(IDockableWindow content, Rectangle floatWindowBounds, bool show)
        {
            InternalConstruct(content, DockState.Float, true, floatWindowBounds, null, DockAlignment.Right, 0.5, show);
        }

        private void InternalConstruct(IDockableWindow content, DockState dockState, bool flagBounds, Rectangle floatWindowBounds, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
        {
            if (dockState == DockState.Hidden || dockState == DockState.Unknown)
                throw new ArgumentException(ResourceHelper.GetString("DockPane.DockState.InvalidState"));

            if (content == null)
                throw new ArgumentNullException(ResourceHelper.GetString("DockPane.Constructor.NullContent"));

            if (content.DockHandler.DockPanel == null)
                throw new ArgumentException(ResourceHelper.GetString("DockPane.Constructor.NullDockPanel"));


            SuspendLayout();
            SetStyle(ControlStyles.Selectable, false);

            _isFloat = (dockState == DockState.Float);

            _contents = new DockContentCollection();
            _displayingContents = new DockContentCollection(this);
            _tabs = new DockPaneTabCollection(this);
            _dockPanel = content.DockHandler.DockPanel;
            _dockPanel.AddPane(this);

            _splitter = new DockPaneSplitter(this);

            _nestedDockingStatus = new NestedDockingStatus(this);

            _autoHidePane = DockPanel.AutoHidePaneFactory.CreateAutoHidePane(this);
            _captionControl = DockPanel.DockPaneCaptionFactory.CreateDockPaneCaption(this);
            _tabStripControl = DockPanel.DockPaneStripFactory.CreateDockPaneStrip(this);
            Controls.AddRange(new Control[] { _captionControl, _tabStripControl });

            DockPanel.SuspendLayout(true);
            if (flagBounds)
                FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this, floatWindowBounds);
            else if (prevPane != null)
                AddToDockList(prevPane.DockListContainer, prevPane, alignment, proportion);

            SetDockState(dockState);
            if (show)
                content.DockHandler.Pane = this;
            else if (this.IsFloat)
                content.DockHandler.FloatPane = this;
            else
                content.DockHandler.PanelPane = this;

            ResumeLayout();
            DockPanel.ResumeLayout(true, true);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            CloseActiveContent();
            if (!DockHelper.IsDockStateAutoHide(DockState) && ActiveContent != null)
                ActiveContent.DockHandler.Activate();
        }

        /// <exclude/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dockState = DockState.Unknown;

                if (DockListContainer != null)
                    DockListContainer.DockList.Remove(this);

                if (DockPanel != null)
                {
                    DockPanel.RemovePane(this);
                    _dockPanel = null;
                }

                Splitter.Dispose();
                AutoHidePane.Dispose();
            }
            base.Dispose(disposing);
        }

        private IDockableWindow _activeContent = null;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="ActiveContent"]/*'/>
        public virtual IDockableWindow ActiveContent
        {
            get { return _activeContent; }
            set
            {
                if (ActiveContent == value)
                    return;

                if (value != null)
                {
                    if (!DisplayingContents.Contains(value))
                        throw (new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
                }
                else
                {
                    if (DisplayingContents.Count != 0)
                        throw (new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
                }

                IDockableWindow oldValue = _activeContent;

                if (DockPanel.ActiveAutoHideContent == oldValue)
                    DockPanel.ActiveAutoHideContent = null;

                _activeContent = value;

                if (DockPanel.DocumentStyle == DocumentStyles.DockingMdi && DockState == DockState.Document)
                {
                    if (_activeContent != null)
                        _activeContent.DockHandler.Form.BringToFront();
                }
                else
                {
                    if (_activeContent != null)
                        _activeContent.DockHandler.SetVisible();
                    if (oldValue != null && DisplayingContents.Contains(oldValue))
                        oldValue.DockHandler.SetVisible();
                }

                if (FloatWindow != null)
                    FloatWindow.SetText();

                RefreshChanges(false);

                if (_activeContent != null)
                    TabStripControl.EnsureTabVisible(_activeContent);
            }
        }

        private bool _allowRedocking = true;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="AllowRedocking"]/*'/>
        public virtual bool AllowRedocking
        {
            get { return _allowRedocking; }
            set { _allowRedocking = value; }
        }

        private DockPaneTabCollection _tabs;
        internal DockPaneTabCollection Tabs
        {
            get { return _tabs; }
        }

        private Rectangle CaptionRectangle
        {
            get
            {
                if (!HasCaption)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;
                int x, y, width;
                x = rectWindow.X;
                y = rectWindow.Y;
                width = rectWindow.Width;
                int height = CaptionControl.MeasureHeight();

                return new Rectangle(x, y, width, height);
            }
        }

        internal Rectangle ContentRectangle
        {
            get
            {
                Rectangle rectWindow = DisplayingRectangle;
                Rectangle rectCaption = CaptionRectangle;
                Rectangle rectTabStrip = TabStripRectangle;

                int x = rectWindow.X;
                int y = rectWindow.Y + (rectCaption.IsEmpty ? 0 : rectCaption.Height) +
                    (DockState == DockState.Document ? rectTabStrip.Height : 0);
                int width = rectWindow.Width;
                int height = rectWindow.Height - rectCaption.Height - rectTabStrip.Height;

                return new Rectangle(x, y, width, height);
            }
        }

        internal Rectangle TabStripRectangle
        {
            get
            {
                if (Appearance == AppearanceStyle.ToolWindow)
                    return TabStripRectangle_ToolWindow;
                else
                    return TabStripRectangle_Document;
            }
        }

        private Rectangle TabStripRectangle_ToolWindow
        {
            get
            {
                if (DisplayingContents.Count <= 1 || IsAutoHide)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;

                int width = rectWindow.Width;
                int height = TabStripControl.MeasureHeight();
                int x = rectWindow.X;
                int y = rectWindow.Bottom - height;
                Rectangle rectCaption = CaptionRectangle;
                if (rectCaption.Contains(x, y))
                    y = rectCaption.Y + rectCaption.Height;



                return new Rectangle(x, y, width, height);
            }
        }

        private Rectangle TabStripRectangle_Document
        {
            get
            {
                if (DisplayingContents.Count == 0)
                    return Rectangle.Empty;

                if (DisplayingContents.Count == 1 && DockPanel.DocumentStyle == DocumentStyles.DockingSdi)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;
                int x = rectWindow.X;
                int y = rectWindow.Y;
                int width = rectWindow.Width;
                int height = TabStripControl.MeasureHeight();

                return new Rectangle(x, y, width, height);
            }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="CaptionText"]/*'/>
        public virtual string CaptionText
        {
            get { return ActiveContent == null ? string.Empty : ActiveContent.DockHandler.TabText; }
        }

        private DockContentCollection _contents;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="Contents"]/*'/>
        public DockContentCollection Contents
        {
            get { return _contents; }
        }

        private DockContentCollection _displayingContents;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DisplayingContents"]/*'/>
        public DockContentCollection DisplayingContents
        {
            get { return _displayingContents; }
        }

        private DockContainer _dockPanel;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockPanel"]/*'/>
        public DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        private bool HasCaption
        {
            get
            {
                if (DockState == DockState.Document ||
                    DockState == DockState.Hidden ||
                    DockState == DockState.Unknown ||
                    (DockState == DockState.Float && FloatWindow.DisplayingList.Count <= 1))
                    return false;
                else
                    return true;
            }
        }

        private bool _isActivated = false;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActivated"]/*'/>
        public bool IsActivated
        {
            get { return _isActivated; }
        }
        internal void SetIsActivated(bool value)
        {
            if (_isActivated == value)
                return;

            _isActivated = value;
            if (DockState != DockState.Document)
                RefreshChanges(false);
            OnIsActivatedChanged(EventArgs.Empty);
        }

        private bool _isActiveDocumentPane = false;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActiveDocumentPane"]/*'/>
        public bool IsActiveDocumentPane
        {
            get { return _isActiveDocumentPane; }
        }
        internal void SetIsActiveDocumentPane(bool value)
        {
            if (_isActiveDocumentPane == value)
                return;

            _isActiveDocumentPane = value;
            if (DockState == DockState.Document)
                RefreshChanges();
            OnIsActiveDocumentPaneChanged(EventArgs.Empty);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="IsDockStateValid(DockState)"]/*'/>
        public bool IsDockStateValid(DockState dockState)
        {
            foreach (IDockableWindow content in Contents)
                if (!content.DockHandler.IsDockStateValid(dockState))
                    return false;

            return true;
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActiveDocumentPane"]/*'/>
        public bool IsAutoHide
        {
            get { return DockHelper.IsDockStateAutoHide(DockState); }
        }

        private DockPaneSplitter _splitter;
        internal DockPaneSplitter Splitter
        {
            get { return _splitter; }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="Appearance"]/*'/>
        public AppearanceStyle Appearance
        {
            get { return (DockState == DockState.Document) ? AppearanceStyle.Document : AppearanceStyle.ToolWindow; }
        }

        internal Rectangle DisplayingRectangle
        {
            get { return ClientRectangle; }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="Activate()"]/*'/>
        public void Activate()
        {
            if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel.ActiveAutoHideContent != ActiveContent)
                DockPanel.ActiveAutoHideContent = ActiveContent;

            if (!IsActivated && ActiveContent != null)
                ActiveContent.DockHandler.Activate();
        }

        internal void AddContent(IDockableWindow content)
        {
            if (Contents.Contains(content))
                return;

            Contents.Add(content);
        }

        internal void Close()
        {
            Dispose();
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="CloseActiveContent()"]/*'/>
        public void CloseActiveContent()
        {
            CloseContent(ActiveContent);
        }

        internal void CloseContent(IDockableWindow content)
        {
            if (content == null)
                return;

            if (!content.DockHandler.CloseButton)
                return;

            if (content.DockHandler.HideOnClose)
                content.DockHandler.Hide();
            else
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // Workaround for .Net Framework bug: removing control from Form may cause form
                // unclosable.
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (Environment.Version.Major == 1 && ContainsFocus)
                {
                    Form form = FindForm();
                    if (form is FloatWindow)
                    {
                        ((FloatWindow)form).DummyControl.Focus();
                        form.ActiveControl = ((FloatWindow)form).DummyControl;
                    }
                    else if (DockPanel != null)
                    {
                        DockPanel.DummyControl.Focus();
                        if (form != null)
                            form.ActiveControl = DockPanel.DummyControl;
                    }
                }
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                content.DockHandler.Close();
            }
        }

        private HitTestResult GetHitTest()
        {
            return GetHitTest(Control.MousePosition);
        }

        private HitTestResult GetHitTest(Point ptMouse)
        {
            HitTestResult hitTestResult = new HitTestResult(HitTestArea.None, -1);

            Point ptMouseClient = PointToClient(ptMouse);

            Rectangle rectCaption = CaptionRectangle;
            if (rectCaption.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Caption, -1);

            Rectangle rectContent = ContentRectangle;
            if (rectContent.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Content, -1);

            Rectangle rectTabStrip = TabStripRectangle;
            if (rectTabStrip.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.TabStrip, TabStripControl.GetHitTest(TabStripControl.PointToClient(ptMouse)));

            return new HitTestResult(HitTestArea.None, -1);
        }

        private bool _isHidden = true;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsHidden"]/*'/>
        public bool IsHidden
        {
            get { return _isHidden; }
        }
        private void SetIsHidden(bool value)
        {
            if (_isHidden == value)
                return;

            _isHidden = value;
            if (DockHelper.IsDockStateAutoHide(DockState))
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }
            else if (DockListContainer != null)
                ((Control)DockListContainer).PerformLayout();
        }

        /// <exclude/>
        protected override void OnLayout(LayoutEventArgs e)
        {
            SetIsHidden(DisplayingContents.Count == 0);
            if (!IsHidden)
            {
                CaptionControl.Bounds = CaptionRectangle;
                TabStripControl.Bounds = TabStripRectangle;

                SetContentBounds();

                foreach (IDockableWindow content in Contents)
                {
                    if (DisplayingContents.Contains(content))
                        if (content.DockHandler.FlagClipWindow && content.DockHandler.Form.Visible)
                            content.DockHandler.FlagClipWindow = false;
                }
            }

            base.OnLayout(e);
        }

        public Control master { get; set; }

        public void SetContentBounds()
        {
            Rectangle rectContent = ContentRectangle;


            if (DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
                rectContent = new Rectangle(DockPanel.PointToMdiClient(PointToScreen(rectContent.Location)), rectContent.Size);


            if (master != null)
                rectContent.Width = master.Width - 15;

            foreach (IDockableWindow content in Contents)
                if (content.DockHandler.Pane == this)
                    content.DockHandler.SetBounds(rectContent);
        }

        internal void RefreshChanges()
        {
            RefreshChanges(true);
        }

        private void RefreshChanges(bool bPerformLayout)
        {
            CaptionControl.RefreshChanges();
            TabStripControl.RefreshChanges();
            if (DockState == DockState.Float)
                FloatWindow.RefreshChanges();
            if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel != null)
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }

            if (bPerformLayout)
                PerformLayout();
        }

        internal void RemoveContent(IDockableWindow content)
        {
            if (!Contents.Contains(content))
                return;

            Contents.Remove(content);
            if (content.DockHandler.Pane == this)
                content.DockHandler.SetPane(null);
            if (Contents.Count == 0)
                Dispose();
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetContentIndex(IDockContent, int)"]/*'/>
        public void SetContentIndex(IDockableWindow content, int index)
        {
            int oldIndex = Contents.IndexOf(content);
            if (oldIndex == -1)
                throw (new ArgumentException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidContent")));

            if (index < 0 || index > Contents.Count - 1)
                if (index != -1)
                    throw (new ArgumentOutOfRangeException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidIndex")));

            if (oldIndex == index)
                return;
            if (oldIndex == Contents.Count - 1 && index == -1)
                return;

            Contents.Remove(content);
            if (index == -1)
                Contents.Add(content);
            else if (oldIndex < index)
                Contents.AddAt(content, index - 1);
            else
                Contents.AddAt(content, index);

            RefreshChanges();
        }

        private void SetParent()
        {
            if (DockState == DockState.Unknown || DockState == DockState.Hidden)
            {
                SetParent(DockPanel.DummyControl);
                Splitter.Parent = DockPanel.DummyControl;
            }
            else if (DockState == DockState.Float)
            {
                SetParent(FloatWindow);
                Splitter.Parent = FloatWindow;
            }
            else if (DockHelper.IsDockStateAutoHide(DockState))
            {
                SetParent(DockPanel.AutoHideWindow);
                Splitter.Parent = DockPanel.DummyControl;
            }
            else
            {
                SetParent(DockPanel.DockWindows[DockState]);
                Splitter.Parent = Parent;
            }
        }

        private void SetParent(Control value)
        {
            if (Parent == value)
                return;

            if (Environment.Version.Major > 1)
            {
                Parent = value;
                return;
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround for .Net Framework bug: removing control from Form may cause form
            // unclosable. Set focus to another dummy control.
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Control oldParent = Parent;

            Form form = FindForm();
            if (ContainsFocus)
            {
                if (form is FloatWindow)
                {
                    ((FloatWindow)form).DummyControl.Focus();
                    form.ActiveControl = ((FloatWindow)form).DummyControl;
                }
                else if (DockPanel != null)
                {
                    DockPanel.DummyControl.Focus();
                    if (form != null)
                        form.ActiveControl = DockPanel.DummyControl;
                }
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            Parent = value;
            _splitter.Parent = value;
        }

        /// <exclude/>
        public new void Show()
        {
            Activate();
        }

        internal void TestDrop(DragHandler dragHandler)
        {
            if (!dragHandler.IsDockStateValid(DockState))
                return;

            Point ptMouse = Control.MousePosition;

            HitTestResult hitTestResult = GetHitTest(ptMouse);
            if (hitTestResult.HitArea == HitTestArea.Caption)
                dragHandler.DockOutline.Show(this, -1);
            else if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
                dragHandler.DockOutline.Show(this, hitTestResult.Index);
        }

        internal void ValidateActiveContent()
        {
            if (ActiveContent == null)
            {
                if (DisplayingContents.Count != 0)
                    ActiveContent = DisplayingContents[0];
                return;
            }

            if (DisplayingContents.IndexOf(ActiveContent) >= 0)
                return;

            IDockableWindow prevVisible = null;
            for (int i = Contents.IndexOf(ActiveContent) - 1; i >= 0; i--)
                if (Contents[i].DockHandler.DockState == DockState)
                {
                    prevVisible = Contents[i];
                    break;
                }

            IDockableWindow nextVisible = null;
            for (int i = Contents.IndexOf(ActiveContent) + 1; i < Contents.Count; i++)
                if (Contents[i].DockHandler.DockState == DockState)
                {
                    nextVisible = Contents[i];
                    break;
                }

            if (prevVisible != null)
                ActiveContent = prevVisible;
            else if (nextVisible != null)
                ActiveContent = nextVisible;
            else
                ActiveContent = null;
        }

        private static readonly object s_dockStateChangedEvent = new object();
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="DockStateChanged"]/*'/>
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(s_dockStateChangedEvent, value); }
            remove { Events.RemoveHandler(s_dockStateChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnDockStateChanged(EventArgs)"]/*'/>
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_dockStateChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_isActivatedChangedEvent = new object();
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="IsActivatedChanged"]/*'/>
        public event EventHandler IsActivatedChanged
        {
            add { Events.AddHandler(s_isActivatedChangedEvent, value); }
            remove { Events.RemoveHandler(s_isActivatedChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnIsActivatedChanged(EventArgs)"]/*'/>
        protected virtual void OnIsActivatedChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_isActivatedChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_isActiveDocumentPaneChangedEvent = new object();
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="IsActiveDocumentPaneChanged"]/*'/>
        public event EventHandler IsActiveDocumentPaneChanged
        {
            add { Events.AddHandler(s_isActiveDocumentPaneChangedEvent, value); }
            remove { Events.RemoveHandler(s_isActiveDocumentPaneChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnIsActiveDocumentPaneChanged(EventArgs)"]/*'/>
        protected virtual void OnIsActiveDocumentPaneChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_isActiveDocumentPaneChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockWindow"]/*'/>
        public DockWindow DockWindow
        {
            get { return (_nestedDockingStatus.DockList == null) ? null : _nestedDockingStatus.DockList.Container as DockWindow; }
            set
            {
                DockWindow oldValue = DockWindow;
                if (oldValue == value)
                    return;

                AddToDockList(value);
            }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="FloatWindow"]/*'/>
        public FloatWindow FloatWindow
        {
            get { return (_nestedDockingStatus.DockList == null) ? null : _nestedDockingStatus.DockList.Container as FloatWindow; }
            set
            {
                FloatWindow oldValue = FloatWindow;
                if (oldValue == value)
                    return;

                AddToDockList(value);
            }
        }

        private NestedDockingStatus _nestedDockingStatus;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="NestedDockingStatus"]/*'/>
        public NestedDockingStatus NestedDockingStatus
        {
            get { return _nestedDockingStatus; }
        }

        private bool _isFloat;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsFloat"]/*'/>
        public bool IsFloat
        {
            get { return _isFloat; }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockListContainer"]/*'/>
        public IDockListContainer DockListContainer
        {
            get
            {
                if (NestedDockingStatus.DockList == null)
                    return null;
                else
                    return NestedDockingStatus.DockList.Container;
            }
        }

        private DockState _dockState = DockState.Unknown;
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockState"]/*'/>
        public DockState DockState
        {
            get { return _dockState; }
            set
            {
                SetDockState(value);
            }
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetDockState(DockState)"]/*'/>
        public DockPane SetDockState(DockState value)
        {
            if (value == DockState.Unknown || value == DockState.Hidden)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.SetDockState.InvalidState"));

            if ((value == DockState.Float) == this.IsFloat)
            {
                InternalSetDockState(value);
                return this;
            }

            if (DisplayingContents.Count == 0)
                return null;

            IDockableWindow firstContent = null;
            for (int i = 0; i < DisplayingContents.Count; i++)
            {
                IDockableWindow content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(value))
                {
                    firstContent = content;
                    break;
                }
            }
            if (firstContent == null)
                return null;

            firstContent.DockHandler.DockState = value;
            DockPane pane = firstContent.DockHandler.Pane;
            DockPanel.SuspendLayout(true);
            for (int i = 0; i < DisplayingContents.Count; i++)
            {
                IDockableWindow content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(value))
                    content.DockHandler.Pane = pane;
            }
            DockPanel.ResumeLayout(true, true);
            return pane;
        }

        private void InternalSetDockState(DockState value)
        {
            if (_dockState == value)
                return;

            DockState oldDockState = _dockState;
            IDockListContainer oldContainer = DockListContainer;

            _dockState = value;

            SuspendRefreshStateChange();
            if (!IsFloat)
                DockWindow = DockPanel.DockWindows[DockState];
            else if (FloatWindow == null)
                FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this);

            ResumeRefreshStateChange(oldContainer, oldDockState);
        }

        private int _countRefreshStateChange = 0;
        private void SuspendRefreshStateChange()
        {
            _countRefreshStateChange++;
            DockPanel.SuspendLayout(true);
        }

        private void ResumeRefreshStateChange()
        {
            _countRefreshStateChange--;
#if DEBUG
            if (_countRefreshStateChange < 0)
                throw new InvalidOperationException();
#endif
            if (_countRefreshStateChange < 0)
                _countRefreshStateChange = 0;
            DockPanel.ResumeLayout(true, true);
        }

        private bool IsRefreshStateChangeSuspended
        {
            get { return _countRefreshStateChange != 0; }
        }

        private void ResumeRefreshStateChange(IDockListContainer oldContainer, DockState oldDockState)
        {
            ResumeRefreshStateChange();
            RefreshStateChange(oldContainer, oldDockState);
        }

        private void RefreshStateChange(IDockListContainer oldContainer, DockState oldDockState)
        {
            lock (this)
            {
                if (IsRefreshStateChangeSuspended)
                    return;

                SuspendRefreshStateChange();
            }

            SetParent();

            if (ActiveContent != null)
                ActiveContent.DockHandler.SetDockState(ActiveContent.DockHandler.IsHidden, DockState, ActiveContent.DockHandler.Pane);
            foreach (IDockableWindow content in Contents)
            {
                if (content.DockHandler.Pane == this)
                    content.DockHandler.SetDockState(content.DockHandler.IsHidden, DockState, content.DockHandler.Pane);
            }

            if (oldContainer != null)
            {
                if (oldContainer.DockState == oldDockState && !oldContainer.IsDisposed)
                    ((Control)oldContainer).PerformLayout();
            }
            if (DockHelper.IsDockStateAutoHide(oldDockState))
                DockPanel.AutoHideWindow.RefreshActiveContent();

            if (DockListContainer.DockState == DockState)
                ((Control)DockListContainer).PerformLayout();
            if (DockHelper.IsDockStateAutoHide(DockState))
                DockPanel.AutoHideWindow.RefreshActiveContent();

            if (DockHelper.IsDockStateAutoHide(oldDockState) ||
                DockHelper.IsDockStateAutoHide(DockState))
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }

            DockPanel.RefreshActiveWindow();
            if (oldDockState != DockState)
                OnDockStateChanged(EventArgs.Empty);

            ResumeRefreshStateChange();
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList"]/*'/>
        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList(IDockListContainer)"]/*'/>
        public DockPane AddToDockList(IDockListContainer container)
        {
            if (container == null)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

            DockAlignment alignment;
            if (container.DockState == DockState.DockLeft || container.DockState == DockState.DockRight)
                alignment = DockAlignment.Bottom;
            else
                alignment = DockAlignment.Right;

            return AddToDockList(container, container.DockList.GetDefaultPrevPane(this), alignment, 0.5);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList(IDockListContainer, DockPane, DockAlignment, double)"]/*'/>
        public DockPane AddToDockList(IDockListContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
        {
            if (container == null)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

            if (container.IsFloat == this.IsFloat)
            {
                InternalAddToDockList(container, prevPane, alignment, proportion);
                return this;
            }

            IDockableWindow firstContent = GetFirstContent(container.DockState);
            if (firstContent == null)
                return null;

            DockPane pane;
            DockPanel.DummyContent.DockPanel = DockPanel;
            if (container.IsFloat)
                pane = DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, (FloatWindow)container, true);
            else
                pane = DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, container.DockState, true);

            pane.AddToDockList(container, prevPane, alignment, proportion);
            SetVisibleContentsToPane(pane);
            DockPanel.DummyContent.DockPanel = null;

            return pane;
        }

        private void SetVisibleContentsToPane(DockPane pane)
        {
            SetVisibleContentsToPane(pane, ActiveContent);
        }

        private void SetVisibleContentsToPane(DockPane pane, IDockableWindow activeContent)
        {
            for (int i = 0; i < DisplayingContents.Count; i++)
            {
                IDockableWindow content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(pane.DockState))
                {
                    content.DockHandler.Pane = pane;
                    i--;
                }
            }

            if (activeContent != null && pane.DisplayingContents.Contains(activeContent))
                pane.ActiveContent = activeContent;
        }

        private void InternalAddToDockList(IDockListContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
        {
            if ((container.DockState == DockState.Float) != IsFloat)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.InvalidContainer"));

            int count = container.DockList.Count;
            if (container.DockList.Contains(this))
                count--;
            if (prevPane == null && count > 0)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullPrevPane"));

            if (prevPane != null && !container.DockList.Contains(prevPane))
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NoPrevPane"));

            if (prevPane == this)
                throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.SelfPrevPane"));

            IDockListContainer oldContainer = DockListContainer;
            DockState oldDockState = DockState;
            container.DockList.Add(this);
            NestedDockingStatus.SetStatus(container.DockList, prevPane, alignment, proportion);

            if (DockHelper.IsDockWindowState(DockState))
                _dockState = container.DockState;

            RefreshStateChange(oldContainer, oldDockState);
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetNestedDockingProportion(double)"]/*'/>
        public void SetNestedDockingProportion(double proportion)
        {
            NestedDockingStatus.SetStatus(NestedDockingStatus.DockList, NestedDockingStatus.PrevPane, NestedDockingStatus.Alignment, proportion);
            if (DockListContainer != null)
                ((Control)DockListContainer).PerformLayout();
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="Float()"]/*'/>
        public DockPane Float()
        {
            IDockableWindow activeContent = ActiveContent;

            DockPane floatPane = GetFloatPaneFromContents();
            if (floatPane == null)
            {
                IDockableWindow firstContent = GetFirstContent(DockState.Float);
                if (firstContent == null)
                    return null;
                floatPane = DockPanel.DockPaneFactory.CreateDockPane(firstContent, DockState.Float, true);
            }
            SetVisibleContentsToPane(floatPane, activeContent);
            floatPane.Activate();

            return floatPane;
        }

        private DockPane GetFloatPaneFromContents()
        {
            DockPane floatPane = null;
            for (int i = 0; i < DisplayingContents.Count; i++)
            {
                IDockableWindow content = DisplayingContents[i];
                if (!content.DockHandler.IsDockStateValid(DockState.Float))
                    continue;

                if (floatPane != null && content.DockHandler.FloatPane != floatPane)
                    return null;
                else
                    floatPane = content.DockHandler.FloatPane;
            }

            return floatPane;
        }

        private IDockableWindow GetFirstContent(DockState dockState)
        {
            for (int i = 0; i < DisplayingContents.Count; i++)
            {
                IDockableWindow content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(dockState))
                    return content;
            }
            return null;
        }

        /// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="RestoreToPanel()"]/*'/>
        public void RestoreToPanel()
        {
            IDockableWindow activeContent = DockPanel.ActiveContent;

            for (int i = DisplayingContents.Count - 1; i >= 0; i--)
            {
                IDockableWindow content = DisplayingContents[i];
                try { content.DockHandler.IsFloat = false; } catch { }
            }

            if (activeContent != null)
                activeContent.DockHandler.Activate();
        }

        private AutoHidePane _autoHidePane;
        internal AutoHidePane AutoHidePane
        {
            get { return _autoHidePane; }
        }

        /// <exclude />
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE)
                Activate();
            else if (m.Msg == (int)Win32.Msgs.WM_WINDOWPOSCHANGING)
            {
                int offset = (int)Marshal.OffsetOf(typeof(Win32.WINDOWPOS), "flags");
                int flags = Marshal.ReadInt32(m.LParam, offset);
                Marshal.WriteInt32(m.LParam, offset, flags | (int)Win32.FlagsSetWindowPos.SWP_NOCOPYBITS);
            }

            base.WndProc(ref m);
        }
    }
}
