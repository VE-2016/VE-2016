
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/ClassDef/*'/>
    public abstract class AutoHideStripBase : Control
    {
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Construct[@name="(DockPanel)"]/*'/>
        protected internal AutoHideStripBase(DockContainer panel)
        {
            _dockPanel = panel;
            _panesTop = new AutoHidePaneCollection(panel, DockState.DockTopAutoHide);
            _panesBottom = new AutoHidePaneCollection(panel, DockState.DockBottomAutoHide);
            _panesLeft = new AutoHidePaneCollection(panel, DockState.DockLeftAutoHide);
            _panesRight = new AutoHidePaneCollection(panel, DockState.DockRightAutoHide);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        private DockContainer _dockPanel;
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="DockPanel"]/*'/>
        protected DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        private AutoHidePaneCollection _panesTop;
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="PanesTop"]/*'/>
        protected AutoHidePaneCollection PanesTop
        {
            get { return _panesTop; }
        }

        private AutoHidePaneCollection _panesBottom;
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="PanesBottom"]/*'/>
        protected AutoHidePaneCollection PanesBottom
        {
            get { return _panesBottom; }
        }

        private AutoHidePaneCollection _panesLeft;
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="PanesLeft"]/*'/>
        protected AutoHidePaneCollection PanesLeft
        {
            get { return _panesLeft; }
        }

        private AutoHidePaneCollection _panesRight;
        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="PanesRight"]/*'/>
        protected AutoHidePaneCollection PanesRight
        {
            get { return _panesRight; }
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Method[@name="GetPanes(DockState)"]/*'/>
        protected internal AutoHidePaneCollection GetPanes(DockState dockState)
        {
            if (dockState == DockState.DockTopAutoHide)
                return PanesTop;
            else if (dockState == DockState.DockBottomAutoHide)
                return PanesBottom;
            else if (dockState == DockState.DockLeftAutoHide)
                return PanesLeft;
            else if (dockState == DockState.DockRightAutoHide)
                return PanesRight;
            else
                throw new IndexOutOfRangeException();
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="RectangleTopLeft"]/*'/>
        protected Rectangle RectangleTopLeft
        {
            get
            {
                int height = MeasureHeight();
                return PanesTop.Count > 0 && PanesLeft.Count > 0 ? new Rectangle(0, 0, height, height) : Rectangle.Empty;
            }
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="RectangleTopRight"]/*'/>
        protected Rectangle RectangleTopRight
        {
            get
            {
                int height = MeasureHeight();
                return PanesTop.Count > 0 && PanesRight.Count > 0 ? new Rectangle(Width - height, 0, height, height) : Rectangle.Empty;
            }
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="RectangleBottomLeft"]/*'/>
        protected Rectangle RectangleBottomLeft
        {
            get
            {
                int height = MeasureHeight();
                return PanesBottom.Count > 0 && PanesLeft.Count > 0 ? new Rectangle(0, Height - height, height, height) : Rectangle.Empty;
            }
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Property[@name="RectangleBottomRight"]/*'/>
        protected Rectangle RectangleBottomRight
        {
            get
            {
                int height = MeasureHeight();
                return PanesBottom.Count > 0 && PanesRight.Count > 0 ? new Rectangle(Width - height, Height - height, height, height) : Rectangle.Empty;
            }
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Method[@name="GetTabStripRectangle(DockState)"]/*'/>
        protected internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            try
            {
                int height = MeasureHeight();
                if (dockState == DockState.DockTopAutoHide && PanesTop.Count > 0)
                    return new Rectangle(RectangleTopLeft.Width, 0, Width - RectangleTopLeft.Width - RectangleTopRight.Width, height);
                else if (dockState == DockState.DockBottomAutoHide && PanesBottom.Count > 0)
                    return new Rectangle(RectangleBottomLeft.Width, Height - height, Width - RectangleBottomLeft.Width - RectangleBottomRight.Width, height);
                else if (dockState == DockState.DockLeftAutoHide && PanesLeft.Count > 0)
                    return new Rectangle(0, RectangleTopLeft.Width, height, Height - RectangleTopLeft.Height - RectangleBottomLeft.Height);
                else if (dockState == DockState.DockRightAutoHide && PanesRight.Count > 0)
                    return new Rectangle(Width - height, RectangleTopRight.Width, height, Height - RectangleTopRight.Height - RectangleBottomRight.Height);
                else
                    return Rectangle.Empty;
            }
            catch (Exception ee) { }

            return Rectangle.Empty;
        }

        private GraphicsPath _displayingArea = null;
        private GraphicsPath DisplayingArea
        {
            get
            {
                if (_displayingArea == null)
                    _displayingArea = new GraphicsPath();

                return _displayingArea;
            }
        }

        private void SetRegion()
        {
            DisplayingArea.Reset();
            DisplayingArea.AddRectangle(RectangleTopLeft);
            DisplayingArea.AddRectangle(RectangleTopRight);
            DisplayingArea.AddRectangle(RectangleBottomLeft);
            DisplayingArea.AddRectangle(RectangleBottomRight);
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockTopAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockBottomAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockLeftAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockRightAutoHide));
            Region = new Region(DisplayingArea);
        }

        /// <exclude/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            IDockableWindow content = GetHitTest();
            if (content == null)
                return;

            if (content != DockPanel.ActiveAutoHideContent)
                DockPanel.ActiveAutoHideContent = content;

            if (!content.DockHandler.Pane.IsActivated)
                content.DockHandler.Pane.Activate();
        }

        /// <exclude/>
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            IDockableWindow content = GetHitTest();
            if (content != null && DockPanel.ActiveAutoHideContent != content)
                DockPanel.ActiveAutoHideContent = content;

            // requires further tracking of mouse hover behavior,
            // call TrackMouseEvent
            Win32.TRACKMOUSEEVENTS tme = new Win32.TRACKMOUSEEVENTS(Win32.TRACKMOUSEEVENTS.TME_HOVER, Handle, Win32.TRACKMOUSEEVENTS.HOVER_DEFAULT);
            User32.TrackMouseEvent(ref tme);
        }

        /// <exclude />
        protected override void OnLayout(LayoutEventArgs levent)
        {
            GC.Collect();
            RefreshChanges();
            base.OnLayout(levent);
        }

        internal void RefreshChanges()
        {
            SetRegion();
            OnRefreshChanges();
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Method[@name="OnRefreshChanges()"]/*'/>
        protected virtual void OnRefreshChanges()
        {
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Method[@name="MeasureHeight()"]/*'/>
        protected internal abstract int MeasureHeight();

        private IDockableWindow GetHitTest()
        {
            Point ptMouse = PointToClient(Control.MousePosition);
            return GetHitTest(ptMouse);
        }

        /// <include file='CodeDoc/AutoHideStripBase.xml' path='//CodeDoc/Class[@name="AutoHideStripBase"]/Method[@name="GetHitTest(Point)"]/*'/>
        protected abstract IDockableWindow GetHitTest(Point point);
    }
}
