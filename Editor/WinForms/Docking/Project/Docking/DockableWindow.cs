

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/ClassDef/*'/>
    public class DockableWindow : Form, IDockableWindow
    {
        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockHContent"]/Constructor[@name="()"]/*'/>
        public DockableWindow()
        {
            _dockHandler = new DockContentHandler(this, new GetPersistStringDelegate(GetPersistString));
            _dockHandler.DockStateChanged += new EventHandler(DockHandler_DockStateChanged);
            Saved = true;
        }

        public bool Saved { get; set; }

        private DockContentHandler _dockHandler = null;
        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="DockHandler"]/*'/>
        [Browsable(false)]
        public DockContentHandler DockHandler
        {
            get { return _dockHandler; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="AllowRedocking"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.AllowRedocking.Description")]
        [DefaultValue(true)]
        public bool AllowRedocking
        {
            get { return DockHandler.AllowRedocking; }
            set { DockHandler.AllowRedocking = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="DockableAreas"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.DockableAreas.Description")]
        [DefaultValue(DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float)]
        public DockAreas DockableAreas
        {
            get { return DockHandler.DockableAreas; }
            set { DockHandler.DockableAreas = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="AutoHidePortion"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.AutoHidePortion.Description")]
        [DefaultValue(0.25)]
        public double AutoHidePortion
        {
            get { return DockHandler.AutoHidePortion; }
            set { DockHandler.AutoHidePortion = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="TabText"]/*'/>
        [Localizable(true)]
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.TabText.Description")]
        [DefaultValue(null)]
        public string TabText
        {
            get { return DockHandler.TabText; }
            set { DockHandler.TabText = value; }
        }
        private bool ShouldSerializeTabText()
        {
            return (DockHandler.TabText != null);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="CloseButton"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.CloseButton.Description")]
        [DefaultValue(true)]
        public bool CloseButton
        {
            get { return DockHandler.CloseButton; }
            set { DockHandler.CloseButton = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="DockPanel"]/*'/>
        [Browsable(false)]
        public DockContainer DockPanel
        {
            get { return DockHandler.DockPanel; }
            set { DockHandler.DockPanel = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="DockState"]/*'/>
        [Browsable(false)]
        public DockState DockState
        {
            get { return DockHandler.DockState; }
            set { DockHandler.DockState = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="Pane"]/*'/>
        [Browsable(false)]
        public DockPane Pane
        {
            get { return DockHandler.Pane; }
            set { DockHandler.Pane = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="IsHidden"]/*'/>
        [Browsable(false)]
        public bool IsHidden
        {
            get { return DockHandler.IsHidden; }
            set { DockHandler.IsHidden = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="VisibleState"]/*'/>
        [Browsable(false)]
        public DockState VisibleState
        {
            get { return DockHandler.VisibleState; }
            set { DockHandler.VisibleState = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="IsFloat"]/*'/>
        [Browsable(false)]
        public bool IsFloat
        {
            get { return DockHandler.IsFloat; }
            set { DockHandler.IsFloat = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="PanelPane"]/*'/>
        [Browsable(false)]
        public DockPane PanelPane
        {
            get { return DockHandler.PanelPane; }
            set { DockHandler.PanelPane = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="FloatPane"]/*'/>
        [Browsable(false)]
        public DockPane FloatPane
        {
            get { return DockHandler.FloatPane; }
            set { DockHandler.FloatPane = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="GetPersistString()"]/*'/>
        protected virtual string GetPersistString()
        {
            return GetType().ToString();
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="HideOnClose"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.HideOnClose.Description")]
        [DefaultValue(false)]
        public bool HideOnClose
        {
            get { return DockHandler.HideOnClose; }
            set { DockHandler.HideOnClose = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="ShowHint"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.ShowHint.Description")]
        [DefaultValue(DockState.Unknown)]
        public DockState ShowHint
        {
            get { return DockHandler.ShowHint; }
            set { DockHandler.ShowHint = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="IsActivated"]/*'/>
        [Browsable(false)]
        public bool IsActivated
        {
            get { return DockHandler.IsActivated; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="IsDockStateValid(DockState)"]/*'/>
        public bool IsDockStateValid(DockState dockState)
        {
            return DockHandler.IsDockStateValid(dockState);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="TabPageContextMenu"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.TabPageContextMenu.Description")]
        [DefaultValue(null)]
        public ContextMenu TabPageContextMenu
        {
            get { return DockHandler.TabPageContextMenu; }
            set { DockHandler.TabPageContextMenu = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="TabPageContextMenuStrip"]/*'/>
        [LocalizedCategory("Category.Docking")]
        [LocalizedDescription("DockHandler.TabPageContextMenuStrip.Description")]
        [DefaultValue(null)]
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return DockHandler.TabPageContextMenuStrip; }
            set { DockHandler.TabPageContextMenuStrip = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Property[@name="ToolTipText"]/*'/>
        [Localizable(true)]
        [Category("Appearance")]
        [LocalizedDescription("DockHandler.ToolTipText.Description")]
        [DefaultValue(null)]
        public string ToolTipText
        {
            get { return DockHandler.ToolTipText; }
            set { DockHandler.ToolTipText = value; }
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Activate()"]/*'/>
        public new void Activate()
        {
            DockHandler.Activate();
        }

        /// <exclude/>
        public new void Hide()
        {
            DockHandler.Hide();
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show()"]/*'/>
        public new void Show()
        {
            DockHandler.Show();
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show(DockPanel)"]/*'/>
        public void Show(DockContainer dockPanel)
        {
            DockHandler.Show(dockPanel);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show(DockPanel, DockState)"]/*'/>
        public void Show(DockContainer dockPanel, DockState dockState)
        {
            DockHandler.Show(dockPanel, dockState);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show(DockPanel, Rectangle)"]/*'/>
        public void Show(DockContainer dockPanel, Rectangle floatWindowBounds)
        {
            DockHandler.Show(dockPanel, floatWindowBounds);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show(DockPane, DockHandler)"]/*'/>
        public void Show(DockPane pane, IDockableWindow beforeContent)
        {
            DockHandler.Show(pane, beforeContent);
        }

        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="DockContent"]/Method[@name="Show(DockPane, DockAlignment, double)"]/*'/>
        public void Show(DockPane prevPane, DockAlignment alignment, double proportion)
        {
            DockHandler.Show(prevPane, alignment, proportion);
        }

        #region Events
        private void DockHandler_DockStateChanged(object sender, EventArgs e)
        {
            OnDockStateChanged(e);
        }

        private static readonly object s_dockStateChangedEvent = new object();
        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="IDockContent"]/Event[@name="DockStateChanged"]/*'/>
        [LocalizedCategory("Category.PropertyChanged")]
        [LocalizedDescription("Pane.DockStateChanged.Description")]
        public event EventHandler DockStateChanged
        {
            add { Events.AddHandler(s_dockStateChangedEvent, value); }
            remove { Events.RemoveHandler(s_dockStateChangedEvent, value); }
        }
        /// <include file='CodeDoc\DockContent.xml' path='//CodeDoc/Class[@name="IDockContent"]/Method[@name="OnDockStateChanged(EventArgs)"]/*'/>
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_dockStateChangedEvent];
            if (handler != null)
                handler(this, e);
        }
        #endregion
    }
}
