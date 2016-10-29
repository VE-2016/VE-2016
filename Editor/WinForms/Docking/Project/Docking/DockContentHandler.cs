

using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Delegate[@name="GetPersistStringDelegate"]/*'/>
    public delegate string GetPersistStringDelegate();

    /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/ClassDef/*'/>
    public class DockContentHandler : IDisposable
    {
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="Overloads"]/*'/>
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="(Form)"]/*'/>
        public DockContentHandler(Form form) : this(form, null)
        {
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="(Form, GetPersistStringDelegate)"]/*'/>
        public DockContentHandler(Form form, GetPersistStringDelegate getPersistStringDelegate)
        {
            if (!(form is IDockableWindow))
                throw new ArgumentException();

            _form = form;
            _getPersistStringDelegate = getPersistStringDelegate;

            _events = new EventHandlerList();
            Form.Disposed += new EventHandler(Form_Disposed);
            Form.TextChanged += new EventHandler(Form_TextChanged);
        }

        /// <exclude />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <exclude />
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    DockPanel = null;
                    if (AutoHideTab != null)
                        AutoHideTab.Dispose();
                    if (DockPaneTab != null)
                        DockPaneTab.Dispose();

                    Form.Disposed -= new EventHandler(Form_Disposed);
                    Form.TextChanged -= new EventHandler(Form_TextChanged);
                    Events.Dispose();
                }
            }
        }

        private Form _form;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Form"]/*'/>
        public Form Form
        {
            get { return _form; }
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Form"]/*'/>
        public IDockableWindow Content
        {
            get { return Form as IDockableWindow; }
        }

        private EventHandlerList _events;
        private EventHandlerList Events
        {
            get { return _events; }
        }

        private bool _allowRedocking = true;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="AllowRedocking"]/*'/>
        public bool AllowRedocking
        {
            get { return _allowRedocking; }
            set { _allowRedocking = value; }
        }

        private double _autoHidePortion = 0.25;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="AutoHidePortion"]/*'/>
        public double AutoHidePortion
        {
            get { return _autoHidePortion; }
            set
            {
                if (value <= 0 || value > 1)
                    throw (new ArgumentOutOfRangeException(ResourceHelper.GetString("IDockContent.AutoHidePortion.OutOfRange")));

                if (_autoHidePortion == value)
                    return;

                _autoHidePortion = value;

                if (DockPanel == null)
                    return;

                if (DockPanel.ActiveAutoHideContent == this.Form as IDockableWindow)
                    DockPanel.PerformLayout();
            }
        }

        private bool _closeButton = true;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="CloseButton"]/*'/>
        public bool CloseButton
        {
            get { return _closeButton; }
            set
            {
                if (_closeButton == value)
                    return;

                _closeButton = value;
                if (Pane != null)
                    if (Pane.ActiveContent == this)
                        Pane.RefreshChanges();
            }
        }

        private DockState DefaultDockState
        {
            get
            {
                if (ShowHint != DockState.Unknown && ShowHint != DockState.Hidden)
                    return ShowHint;

                if ((DockableAreas & DockAreas.Document) != 0)
                    return DockState.Document;
                if ((DockableAreas & DockAreas.DockRight) != 0)
                    return DockState.DockRight;
                if ((DockableAreas & DockAreas.DockLeft) != 0)
                    return DockState.DockLeft;
                if ((DockableAreas & DockAreas.DockBottom) != 0)
                    return DockState.DockBottom;
                if ((DockableAreas & DockAreas.DockTop) != 0)
                    return DockState.DockTop;

                return DockState.Unknown;
            }
        }

        private DockState DefaultShowState
        {
            get
            {
                if (ShowHint != DockState.Unknown)
                    return ShowHint;

                if ((DockableAreas & DockAreas.Document) != 0)
                    return DockState.Document;
                if ((DockableAreas & DockAreas.DockRight) != 0)
                    return DockState.DockRight;
                if ((DockableAreas & DockAreas.DockLeft) != 0)
                    return DockState.DockLeft;
                if ((DockableAreas & DockAreas.DockBottom) != 0)
                    return DockState.DockBottom;
                if ((DockableAreas & DockAreas.DockTop) != 0)
                    return DockState.DockTop;
                if ((DockableAreas & DockAreas.Float) != 0)
                    return DockState.Float;

                return DockState.Unknown;
            }
        }

        private DockAreas _allowedAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockableAreas"]/*'/>
        public DockAreas DockableAreas
        {
            get { return _allowedAreas; }
            set
            {
                if (_allowedAreas == value)
                    return;

                if (!DockHelper.IsDockStateValid(DockState, value))
                    throw (new InvalidOperationException(ResourceHelper.GetString("IDockContent.DockableAreas.InvalidValue")));

                _allowedAreas = value;

                if (!DockHelper.IsDockStateValid(ShowHint, _allowedAreas))
                    ShowHint = DockState.Unknown;
            }
        }

        private DockState _dockState = DockState.Unknown;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockState"]/*'/>
        public DockState DockState
        {
            get { return _dockState; }
            set
            {
                if (_dockState == value)
                    return;

                if (value == DockState.Hidden)
                    IsHidden = true;
                else
                    SetDockState(false, value, Pane);
            }
        }

        private DockContainer _dockPanel = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockPanel"]/*'/>
        public DockContainer DockPanel
        {
            get { return _dockPanel; }
            set
            {
                if (_dockPanel == value)
                    return;

                Pane = null;

                if (_dockPanel != null)
                    _dockPanel.RemoveContent(Content);

                if (_dockPaneTab != null)
                {
                    _dockPaneTab.Dispose();
                    _dockPaneTab = null;
                }

                if (_autoHideTab != null)
                {
                    _autoHideTab.Dispose();
                    _autoHideTab = null;
                }

                _dockPanel = value;

                if (_dockPanel != null)
                {
                    _dockPanel.AddContent(Content);
                    Form.TopLevel = false;
                    Form.FormBorderStyle = FormBorderStyle.None;
                    Form.ShowInTaskbar = false;
                    User32.SetWindowPos(Form.Handle, IntPtr.Zero, 0, 0, 0, 0,
                        Win32.FlagsSetWindowPos.SWP_NOACTIVATE |
                        Win32.FlagsSetWindowPos.SWP_NOMOVE |
                        Win32.FlagsSetWindowPos.SWP_NOSIZE |
                        Win32.FlagsSetWindowPos.SWP_NOZORDER |
                        Win32.FlagsSetWindowPos.SWP_NOOWNERZORDER |
                        Win32.FlagsSetWindowPos.SWP_FRAMECHANGED);
                    _dockPaneTab = DockPanel.DockPaneTabFactory.CreateDockPaneTab(Content);
                    _autoHideTab = DockPanel.AutoHideTabFactory.CreateAutoHideTab(Content);
                }
            }
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Icon"]/*'/>
        public Icon Icon
        {
            get { return Form.Icon; }
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Pane"]/*'/>
        public DockPane Pane
        {
            get { return IsFloat ? FloatPane : PanelPane; }
            set
            {
                if (Pane == value)
                    return;

                DockPane oldPane = Pane;

                SuspendSetDockState();
                FloatPane = (value == null ? null : (value.IsFloat ? value : FloatPane));
                PanelPane = (value == null ? null : (value.IsFloat ? PanelPane : value));
                ResumeSetDockState(IsHidden, value != null ? value.DockState : DockState.Unknown, oldPane);
            }
        }

        private bool _isHidden = true;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsHidden"]/*'/>
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (_isHidden == value)
                    return;

                SetDockState(value, VisibleState, Pane);
            }
        }

        private string _tabText = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="TabText"]/*'/>
        public string TabText
        {
            get { return _tabText == null ? Form.Text : _tabText; }
            set
            {
                if (_tabText == value)
                    return;

                _tabText = value;
                if (Pane != null)
                    Pane.RefreshChanges();
            }
        }

        private DockState _visibleState = DockState.Unknown;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="VisibleState"]/*'/>
        public DockState VisibleState
        {
            get { return _visibleState; }
            set
            {
                if (_visibleState == value)
                    return;

                SetDockState(IsHidden, value, Pane);
            }
        }

        private bool _isFloat = false;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsFloat"]/*'/>
        public bool IsFloat
        {
            get { return _isFloat; }
            set
            {
                DockState visibleState;

                if (_isFloat == value)
                    return;

                DockPane oldPane = Pane;

                if (value)
                {
                    if (!IsDockStateValid(DockState.Float))
                        throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.IsFloat.InvalidValue"));
                    visibleState = DockState.Float;
                }
                else
                    visibleState = (PanelPane != null) ? PanelPane.DockState : DefaultDockState;

                if (visibleState == DockState.Unknown)
                    throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.IsFloat.InvalidValue"));

                SetDockState(IsHidden, visibleState, oldPane);
            }
        }

        private DockPane _panelPane = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="PanelPane"]/*'/>
        public DockPane PanelPane
        {
            get { return _panelPane; }
            set
            {
                if (_panelPane == value)
                    return;

                if (value != null)
                {
                    if (value.IsFloat || value.DockPanel != DockPanel)
                        throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.DockPane.InvalidValue"));
                }

                DockPane oldPane = Pane;

                if (_panelPane != null)
                    _panelPane.RemoveContent(Content);

                _panelPane = value;
                if (_panelPane != null)
                {
                    _panelPane.AddContent(Content);
                    SetDockState(IsHidden, IsFloat ? DockState.Float : _panelPane.DockState, oldPane);
                }
                else
                    SetDockState(IsHidden, DockState.Unknown, oldPane);
            }
        }

        private DockPane _floatPane = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="FloatPane"]/*'/>
        public DockPane FloatPane
        {
            get { return _floatPane; }
            set
            {
                if (_floatPane == value)
                    return;

                if (value != null)
                {
                    if (!value.IsFloat || value.DockPanel != DockPanel)
                        throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.FloatPane.InvalidValue"));
                }

                DockPane oldPane = Pane;

                if (_floatPane != null)
                    _floatPane.RemoveContent(Content);

                _floatPane = value;
                if (_floatPane != null)
                {
                    _floatPane.AddContent(Content);
                    SetDockState(IsHidden, IsFloat ? DockState.Float : VisibleState, oldPane);
                }
                else
                    SetDockState(IsHidden, DockState.Unknown, oldPane);
            }
        }

        private int _countSetDockState = 0;
        private void SuspendSetDockState()
        {
            _countSetDockState++;
        }

        private void ResumeSetDockState()
        {
            _countSetDockState--;
            if (_countSetDockState < 0)
                _countSetDockState = 0;
        }

        internal bool IsSuspendSetDockState
        {
            get { return _countSetDockState != 0; }
        }

        private void ResumeSetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
        {
            ResumeSetDockState();
            SetDockState(isHidden, visibleState, oldPane);
        }

        internal void SetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
        {
            if (IsSuspendSetDockState)
                return;

            if (DockPanel == null && visibleState != DockState.Unknown)
                throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.SetDockState.NullPanel"));

            if (visibleState == DockState.Hidden || (visibleState != DockState.Unknown && !IsDockStateValid(visibleState)))
                throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.SetDockState.InvalidState"));

            SuspendSetDockState();

            DockState oldDockState = DockState;

            if (_isHidden != isHidden || oldDockState == DockState.Unknown)
            {
                _isHidden = isHidden;
            }
            _visibleState = visibleState;
            _dockState = isHidden ? DockState.Hidden : visibleState;

            if (visibleState == DockState.Unknown)
                Pane = null;
            else
            {
                _isFloat = (_visibleState == DockState.Float);

                if (Pane == null)
                    Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, visibleState, true);
                else if (Pane.DockState != visibleState)
                {
                    if (Pane.Contents.Count == 1)
                        Pane.SetDockState(visibleState);
                    else
                        Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, visibleState, true);
                }
            }

            SetPane(Pane);
            SetVisible();

            if (oldPane != null && !oldPane.IsDisposed && oldDockState == oldPane.DockState)
                RefreshDockPane(oldPane);

            if (Pane != null && DockState == Pane.DockState)
            {
                if ((Pane != oldPane) ||
                    (Pane == oldPane && oldDockState != oldPane.DockState))
                    RefreshDockPane(Pane);
            }

            if (oldDockState != DockState)
                OnDockStateChanged(EventArgs.Empty);

            ResumeSetDockState();
        }

        private void RefreshDockPane(DockPane pane)
        {
            pane.RefreshChanges();
            pane.ValidateActiveContent();
        }

        internal string PersistString
        {
            get { return GetPersistStringDelegate == null ? Form.GetType().ToString() : GetPersistStringDelegate(); }
        }

        private GetPersistStringDelegate _getPersistStringDelegate = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="GetPersistStringDelegate"]/*'/>
        public GetPersistStringDelegate GetPersistStringDelegate
        {
            get { return _getPersistStringDelegate; }
            set { _getPersistStringDelegate = value; }
        }


        private bool _hideOnClose = false;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="HideOnClose"]/*'/>
        public bool HideOnClose
        {
            get { return _hideOnClose; }
            set { _hideOnClose = value; }
        }

        private DockState _showHint = DockState.Unknown;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="ShowHint"]/*'/>
        public DockState ShowHint
        {
            get { return _showHint; }
            set
            {
                if (!DockHelper.IsDockStateValid(value, DockableAreas))
                    throw (new InvalidOperationException(ResourceHelper.GetString("IDockContent.ShowHint.InvalidValue")));

                if (_showHint == value)
                    return;

                _showHint = value;
            }
        }

        private bool _isActivated = false;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsActivated"]/*'/>
        public bool IsActivated
        {
            get { return _isActivated; }
        }
        internal void SetIsActivated(bool value)
        {
            if (_isActivated == value)
                return;

            _isActivated = value;
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="IsDockStateValid(DockState)"]/*'/>
        public bool IsDockStateValid(DockState dockState)
        {
            if (DockPanel != null && dockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.SystemMdi)
                return false;
            else
                return DockHelper.IsDockStateValid(dockState, DockableAreas);
        }

        private ContextMenu _tabPageContextMenu = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="TabPageContextMenu"]/*'/>
        public ContextMenu TabPageContextMenu
        {
            get { return _tabPageContextMenu; }
            set { _tabPageContextMenu = value; }
        }

        private string _toolTipText = null;
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="ToolTipText"]/*'/>
        public string ToolTipText
        {
            get { return _toolTipText; }
            set { _toolTipText = value; }
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Activate()"]/*'/>
        public void Activate()
        {
            if (DockPanel == null)
                Form.Activate();
            else if (Pane == null)
                Show(DockPanel);
            else
            {
                IsHidden = false;
                Pane.ActiveContent = Content;
                if (DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
                    Form.Activate();
                else if (!Form.ContainsFocus)
                {
                    if (Contains(ActiveWindowHandle))
                        User32.SetFocus(ActiveWindowHandle);

                    if (!Form.ContainsFocus)
                    {
                        if (!Form.SelectNextControl(Form.ActiveControl, true, true, true, true))
                            // Since DockContent Form is not selectalbe, use Win32 SetFocus instead
                            User32.SetFocus(Form.Handle);
                    }
                }
            }
        }

        private bool Contains(IntPtr hWnd)
        {
            Control control = Control.FromChildHandle(hWnd);
            for (Control parent = control; parent != null; parent = parent.Parent)
                if (parent == Form)
                    return true;

            return false;
        }

        private IntPtr _activeWindowHandle = IntPtr.Zero;
        internal IntPtr ActiveWindowHandle
        {
            get { return _activeWindowHandle; }
            set { _activeWindowHandle = value; }
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Hide()"]/*'/>
        public void Hide()
        {
            IsHidden = true;
        }

        internal void SetPane(DockPane pane)
        {
            if (pane != null && pane.DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
            {
                if (Form.Parent is DockPane)
                    SetParent(null);
                if (Form.MdiParent != DockPanel.ParentForm)
                {
                    FlagClipWindow = true;
                    Form.MdiParent = DockPanel.ParentForm;
                }
            }
            else
            {
                FlagClipWindow = true;
                if (Form.MdiParent != null)
                    Form.MdiParent = null;
                if (Form.TopLevel)
                    Form.TopLevel = false;
                SetParent(pane);
            }
        }

        internal void SetVisible()
        {
            bool visible;

            if (IsHidden)
                visible = false;
            else if (Pane != null && Pane.DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
                visible = true;
            else if (Pane != null && Pane.ActiveContent == Form as IDockableWindow)
                visible = true;
            else if (Pane != null && Pane.ActiveContent != Form as IDockableWindow)
                visible = false;
            else
                visible = Form.Visible;

            Form.Visible = visible;
        }

        private void SetParent(Control value)
        {
            if (Form.Parent == value)
                return;

            if (Environment.Version.Major > 1)
            {
                Form.Parent = value;
                return;
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround for .Net Framework bug: removing control from Form may cause form
            // unclosable. Set focus to another dummy control.
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Control oldParent = Form.Parent;

            Form form = null;
            if (Form.Parent is DockPane)
                form = ((DockPane)Form.Parent).FindForm();
            if (Form.ContainsFocus)
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

            Form.Parent = value;
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show"]/*'/>
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show()"]/*'/>
        public void Show()
        {
            if (DockPanel == null)
                Form.Show();
            else
                Show(DockPanel);
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel)"]/*'/>
        public void Show(DockContainer dockPanel)
        {
            if (dockPanel == null)
                throw (new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

            if (DockState == DockState.Unknown)
                Show(dockPanel, DefaultShowState);
            else
                Activate();
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel, DockState)"]/*'/>
        public void Show(DockContainer dockPanel, DockState dockState)
        {
            if (dockPanel == null)
                throw (new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

            if (dockState == DockState.Unknown || dockState == DockState.Hidden)
                throw (new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidDockState")));

            DockPanel = dockPanel;

            if (dockState == DockState.Float && FloatPane == null)
                Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, DockState.Float, true);
            else if (PanelPane == null)
            {
                DockPane paneExisting = null;
                foreach (DockPane pane in DockPanel.Panes)
                    if (pane.DockState == dockState)
                    {
                        paneExisting = pane;
                        break;
                    }

                if (paneExisting == null)
                    Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, dockState, true);
                else
                    Pane = paneExisting;
            }

            DockState = dockState;
            Activate();
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel, Rectangle)"]/*'/>
        public void Show(DockContainer dockPanel, Rectangle floatWindowBounds)
        {
            if (dockPanel == null)
                throw (new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

            DockPanel = dockPanel;
            if (FloatPane == null)
            {
                IsHidden = true;    // to reduce the screen flicker
                FloatPane = DockPanel.DockPaneFactory.CreateDockPane(Content, DockState.Float, false);
                FloatPane.FloatWindow.StartPosition = FormStartPosition.Manual;
            }

            FloatPane.FloatWindow.Bounds = floatWindowBounds;

            Show(dockPanel, DockState.Float);
            Activate();
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPane, IDockContent)"]/*'/>
        public void Show(DockPane pane, IDockableWindow beforeContent)
        {
            if (pane == null)
                throw (new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullPane")));

            if (beforeContent != null && pane.Contents.IndexOf(beforeContent) == -1)
                throw (new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidBeforeContent")));

            DockPanel = pane.DockPanel;
            Pane = pane;
            pane.SetContentIndex(Content, pane.Contents.IndexOf(beforeContent));
            Show();
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPane, DockAlignment, double)"]/*'/>
        public void Show(DockPane prevPane, DockAlignment alignment, double proportion)
        {
            if (prevPane == null)
                throw (new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidPrevPane")));

            if (DockHelper.IsDockStateAutoHide(prevPane.DockState))
                throw (new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidPrevPane")));

            DockPanel = prevPane.DockPanel;
            DockPanel.DockPaneFactory.CreateDockPane(Content, prevPane, alignment, proportion, true);
            Show();
        }

        internal void SetBounds(Rectangle bounds)
        {
            Form.SuspendLayout();
            Form.Bounds = bounds;
            Form.ResumeLayout();

            Form.Refresh();
        }

        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Close()"]/*'/>
        public void Close()
        {
            DockContainer dockPanel = DockPanel;
            if (dockPanel != null)
                dockPanel.SuspendLayout(true);
            Form.Close();
            if (dockPanel != null)
                dockPanel.ResumeLayout(true, true);
        }

        private DockPaneTab _dockPaneTab = null;
        internal DockPaneTab DockPaneTab
        {
            get { return _dockPaneTab; }
        }

        private AutoHideTab _autoHideTab = null;
        internal AutoHideTab AutoHideTab
        {
            get { return _autoHideTab; }
        }

        #region Events
        private static readonly object s_dockStateChangedEvent = new object();
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Event[@name="DockStateChanged"]/*'/>
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(s_dockStateChangedEvent, value); }
            remove { Events.RemoveHandler(s_dockStateChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="OnDockStateChanged(EventArgs)"]/*'/>
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_dockStateChangedEvent];
            if (handler != null)
                handler(this, e);
        }
        #endregion

        private void Form_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        private void Form_TextChanged(object sender, EventArgs e)
        {
            if (DockHelper.IsDockStateAutoHide(DockState))
                DockPanel.RefreshAutoHideStrip();
            else if (Pane != null)
            {
                if (Pane.FloatWindow != null)
                    Pane.FloatWindow.SetText();
                Pane.RefreshChanges();
            }
        }

        private bool _flagClipWindow = false;
        internal bool FlagClipWindow
        {
            get { return _flagClipWindow; }
            set
            {
                if (_flagClipWindow == value)
                    return;

                _flagClipWindow = value;
                if (_flagClipWindow)
                    Form.Region = new Region(Rectangle.Empty);
                else
                    Form.Region = null;
            }
        }

        private ContextMenuStrip _tabPageContextMenuStrip = null;
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return _tabPageContextMenuStrip; }
            set { _tabPageContextMenuStrip = value; }
        }
    }
}
