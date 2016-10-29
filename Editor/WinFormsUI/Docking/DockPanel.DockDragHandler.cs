using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public partial class DockPanel
    {
        #region PaneIndicator

        public interface IPaneIndicator : IHitTest
        {
            Point Location { get; set; }
            bool Visible { get; set; }
            int Left { get; }
            int Top { get; }
            int Right { get; }
            int Bottom { get; }
            Rectangle ClientRectangle { get; }
            int Width { get; }
            int Height { get; }
            GraphicsPath DisplayingGraphicsPath { get; }
        }

        public struct HotSpotIndex
        {
            public HotSpotIndex(int x, int y, DockStyle dockStyle)
            {
                _x = x;
                _y = y;
                _dockStyle = dockStyle;
            }

            private int _x;
            public int X
            {
                get { return _x; }
            }

            private int _y;
            public int Y
            {
                get { return _y; }
            }

            private DockStyle _dockStyle;
            public DockStyle DockStyle
            {
                get { return _dockStyle; }
            }
        }

        internal class DefaultPaneIndicator : PictureBox, IPaneIndicator
        {
            private static Bitmap s_bitmapPaneDiamond = Resources.DockIndicator_PaneDiamond;
            private static Bitmap s_bitmapPaneDiamondLeft = Resources.DockIndicator_PaneDiamond_Left;
            private static Bitmap s_bitmapPaneDiamondRight = Resources.DockIndicator_PaneDiamond_Right;
            private static Bitmap s_bitmapPaneDiamondTop = Resources.DockIndicator_PaneDiamond_Top;
            private static Bitmap s_bitmapPaneDiamondBottom = Resources.DockIndicator_PaneDiamond_Bottom;
            private static Bitmap s_bitmapPaneDiamondFill = Resources.DockIndicator_PaneDiamond_Fill;
            private static Bitmap s_bitmapPaneDiamondHotSpot = Resources.DockIndicator_PaneDiamond_HotSpot;
            private static Bitmap s_bitmapPaneDiamondHotSpotIndex = Resources.DockIndicator_PaneDiamond_HotSpotIndex;
            private static HotSpotIndex[] s_hotSpots = new[]
            {
                new HotSpotIndex(1, 0, DockStyle.Top),
                new HotSpotIndex(0, 1, DockStyle.Left),
                new HotSpotIndex(1, 1, DockStyle.Fill),
                new HotSpotIndex(2, 1, DockStyle.Right),
                new HotSpotIndex(1, 2, DockStyle.Bottom)
            };

            private GraphicsPath _displayingGraphicsPath = DrawHelper.CalculateGraphicsPathFromBitmap(s_bitmapPaneDiamond);

            public DefaultPaneIndicator()
            {
                SizeMode = PictureBoxSizeMode.AutoSize;
                Image = s_bitmapPaneDiamond;
                Region = new Region(DisplayingGraphicsPath);
            }

            public GraphicsPath DisplayingGraphicsPath
            {
                get { return _displayingGraphicsPath; }
            }

            public DockStyle HitTest(Point pt)
            {
                if (!Visible)
                    return DockStyle.None;

                pt = PointToClient(pt);
                if (!ClientRectangle.Contains(pt))
                    return DockStyle.None;

                for (int i = s_hotSpots.GetLowerBound(0); i <= s_hotSpots.GetUpperBound(0); i++)
                {
                    if (s_bitmapPaneDiamondHotSpot.GetPixel(pt.X, pt.Y) == s_bitmapPaneDiamondHotSpotIndex.GetPixel(s_hotSpots[i].X, s_hotSpots[i].Y))
                        return s_hotSpots[i].DockStyle;
                }

                return DockStyle.None;
            }

            private DockStyle _status = DockStyle.None;
            public DockStyle Status
            {
                get { return _status; }
                set
                {
                    _status = value;
                    if (_status == DockStyle.None)
                        Image = s_bitmapPaneDiamond;
                    else if (_status == DockStyle.Left)
                        Image = s_bitmapPaneDiamondLeft;
                    else if (_status == DockStyle.Right)
                        Image = s_bitmapPaneDiamondRight;
                    else if (_status == DockStyle.Top)
                        Image = s_bitmapPaneDiamondTop;
                    else if (_status == DockStyle.Bottom)
                        Image = s_bitmapPaneDiamondBottom;
                    else if (_status == DockStyle.Fill)
                        Image = s_bitmapPaneDiamondFill;
                }
            }
        }
        #endregion PaneIndicator

        #region IHitTest
        public interface IHitTest
        {
            DockStyle HitTest(Point pt);
            DockStyle Status { get; set; }
        }
        #endregion

        #region PanelIndicator

        public interface IPanelIndicator : IHitTest
        {
            Point Location { get; set; }
            bool Visible { get; set; }
            Rectangle Bounds { get; }
            int Width { get; }
            int Height { get; }
        }

        internal class DefaultPanelIndicator : PictureBox, IPanelIndicator
        {
            private static Image s_imagePanelLeft = Resources.DockIndicator_PanelLeft;
            private static Image s_imagePanelRight = Resources.DockIndicator_PanelRight;
            private static Image s_imagePanelTop = Resources.DockIndicator_PanelTop;
            private static Image s_imagePanelBottom = Resources.DockIndicator_PanelBottom;
            private static Image s_imagePanelFill = Resources.DockIndicator_PanelFill;
            private static Image s_imagePanelLeftActive = Resources.DockIndicator_PanelLeft_Active;
            private static Image s_imagePanelRightActive = Resources.DockIndicator_PanelRight_Active;
            private static Image s_imagePanelTopActive = Resources.DockIndicator_PanelTop_Active;
            private static Image s_imagePanelBottomActive = Resources.DockIndicator_PanelBottom_Active;
            private static Image s_imagePanelFillActive = Resources.DockIndicator_PanelFill_Active;

            public DefaultPanelIndicator(DockStyle dockStyle)
            {
                _dockStyle = dockStyle;
                SizeMode = PictureBoxSizeMode.AutoSize;
                Image = ImageInactive;
            }

            private DockStyle _dockStyle;
            private DockStyle DockStyle
            {
                get { return _dockStyle; }
            }

            private DockStyle _status;
            public DockStyle Status
            {
                get { return _status; }
                set
                {
                    if (value != DockStyle && value != DockStyle.None)
                        throw new InvalidEnumArgumentException();

                    if (_status == value)
                        return;

                    _status = value;
                    IsActivated = (_status != DockStyle.None);
                }
            }

            private Image ImageInactive
            {
                get
                {
                    if (DockStyle == DockStyle.Left)
                        return s_imagePanelLeft;
                    else if (DockStyle == DockStyle.Right)
                        return s_imagePanelRight;
                    else if (DockStyle == DockStyle.Top)
                        return s_imagePanelTop;
                    else if (DockStyle == DockStyle.Bottom)
                        return s_imagePanelBottom;
                    else if (DockStyle == DockStyle.Fill)
                        return s_imagePanelFill;
                    else
                        return null;
                }
            }

            private Image ImageActive
            {
                get
                {
                    if (DockStyle == DockStyle.Left)
                        return s_imagePanelLeftActive;
                    else if (DockStyle == DockStyle.Right)
                        return s_imagePanelRightActive;
                    else if (DockStyle == DockStyle.Top)
                        return s_imagePanelTopActive;
                    else if (DockStyle == DockStyle.Bottom)
                        return s_imagePanelBottomActive;
                    else if (DockStyle == DockStyle.Fill)
                        return s_imagePanelFillActive;
                    else
                        return null;
                }
            }

            private bool _isActivated = false;
            private bool IsActivated
            {
                get { return _isActivated; }
                set
                {
                    _isActivated = value;
                    Image = IsActivated ? ImageActive : ImageInactive;
                }
            }

            public DockStyle HitTest(Point pt)
            {
                return this.Visible && ClientRectangle.Contains(PointToClient(pt)) ? DockStyle : DockStyle.None;
            }
        }
        #endregion PanelIndicator

        internal class DefaultDockOutline : DockOutlineBase
        {
            public DefaultDockOutline()
            {
                _dragForm = new DragForm();
                SetDragForm(Rectangle.Empty);
                DragForm.BackColor = SystemColors.ActiveCaption;
                DragForm.Opacity = 0.5;
                DragForm.Show(false);
            }

            private DragForm _dragForm;
            private DragForm DragForm
            {
                get { return _dragForm; }
            }

            protected override void OnShow()
            {
                CalculateRegion();
            }

            protected override void OnClose()
            {
                DragForm.Close();
            }

            private void CalculateRegion()
            {
                if (SameAsOldValue)
                    return;

                if (!FloatWindowBounds.IsEmpty)
                    SetOutline(FloatWindowBounds);
                else if (DockTo is DockPanel)
                    SetOutline(DockTo as DockPanel, Dock, (ContentIndex != 0));
                else if (DockTo is DockPane)
                    SetOutline(DockTo as DockPane, Dock, ContentIndex);
                else
                    SetOutline();
            }

            private void SetOutline()
            {
                SetDragForm(Rectangle.Empty);
            }

            private void SetOutline(Rectangle floatWindowBounds)
            {
                SetDragForm(floatWindowBounds);
            }

            private void SetOutline(DockPanel dockPanel, DockStyle dock, bool fullPanelEdge)
            {
                Rectangle rect = fullPanelEdge ? dockPanel.DockArea : dockPanel.DocumentWindowBounds;
                rect.Location = dockPanel.PointToScreen(rect.Location);
                if (dock == DockStyle.Top)
                {
                    int height = dockPanel.GetDockWindowSize(DockState.DockTop);
                    rect = new Rectangle(rect.X, rect.Y, rect.Width, height);
                }
                else if (dock == DockStyle.Bottom)
                {
                    int height = dockPanel.GetDockWindowSize(DockState.DockBottom);
                    rect = new Rectangle(rect.X, rect.Bottom - height, rect.Width, height);
                }
                else if (dock == DockStyle.Left)
                {
                    int width = dockPanel.GetDockWindowSize(DockState.DockLeft);
                    rect = new Rectangle(rect.X, rect.Y, width, rect.Height);
                }
                else if (dock == DockStyle.Right)
                {
                    int width = dockPanel.GetDockWindowSize(DockState.DockRight);
                    rect = new Rectangle(rect.Right - width, rect.Y, width, rect.Height);
                }
                else if (dock == DockStyle.Fill)
                {
                    rect = dockPanel.DocumentWindowBounds;
                    rect.Location = dockPanel.PointToScreen(rect.Location);
                }

                SetDragForm(rect);
            }

            private void SetOutline(DockPane pane, DockStyle dock, int contentIndex)
            {
                if (dock != DockStyle.Fill)
                {
                    Rectangle rect = pane.DisplayingRectangle;
                    if (dock == DockStyle.Right)
                        rect.X += rect.Width / 2;
                    if (dock == DockStyle.Bottom)
                        rect.Y += rect.Height / 2;
                    if (dock == DockStyle.Left || dock == DockStyle.Right)
                        rect.Width -= rect.Width / 2;
                    if (dock == DockStyle.Top || dock == DockStyle.Bottom)
                        rect.Height -= rect.Height / 2;
                    rect.Location = pane.PointToScreen(rect.Location);

                    SetDragForm(rect);
                }
                else if (contentIndex == -1)
                {
                    Rectangle rect = pane.DisplayingRectangle;
                    rect.Location = pane.PointToScreen(rect.Location);
                    SetDragForm(rect);
                }
                else
                {
                    using (GraphicsPath path = pane.TabStripControl.GetOutline(contentIndex))
                    {
                        RectangleF rectF = path.GetBounds();
                        Rectangle rect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
                        using (Matrix matrix = new Matrix(rect, new Point[] { new Point(0, 0), new Point(rect.Width, 0), new Point(0, rect.Height) }))
                        {
                            path.Transform(matrix);
                        }
                        Region region = new Region(path);
                        SetDragForm(rect, region);
                    }
                }
            }

            private void SetDragForm(Rectangle rect)
            {
                DragForm.Bounds = rect;
                if (rect == Rectangle.Empty)
                {
                    if (DragForm.Region != null)
                    {
                        DragForm.Region.Dispose();
                    }

                    DragForm.Region = new Region(Rectangle.Empty);
                }
                else if (DragForm.Region != null)
                {
                    DragForm.Region.Dispose();
                    DragForm.Region = null;
                }
            }

            private void SetDragForm(Rectangle rect, Region region)
            {
                DragForm.Bounds = rect;
                DragForm.Region = region;
            }
        }

        private sealed class DockDragHandler : DragHandler
        {
            public class DockIndicator : DragForm
            {
                #region consts
                private int _PanelIndicatorMargin = 10;
                #endregion

                private DockDragHandler _dragHandler;

                public DockIndicator(DockDragHandler dragHandler)
                {
                    _dragHandler = dragHandler;
                    Controls.AddRange(new[] {
                        (Control)PaneDiamond,
                        (Control)PanelLeft,
                        (Control)PanelRight,
                        (Control)PanelTop,
                        (Control)PanelBottom,
                        (Control)PanelFill
                        });
                    Region = new Region(Rectangle.Empty);
                }

                private IPaneIndicator _paneDiamond = null;
                private IPaneIndicator PaneDiamond
                {
                    get
                    {
                        if (_paneDiamond == null)
                            _paneDiamond = _dragHandler.DockPanel.Extender.PaneIndicatorFactory.CreatePaneIndicator();

                        return _paneDiamond;
                    }
                }

                private IPanelIndicator _panelLeft = null;
                private IPanelIndicator PanelLeft
                {
                    get
                    {
                        if (_panelLeft == null)
                            _panelLeft = _dragHandler.DockPanel.Extender.PanelIndicatorFactory.CreatePanelIndicator(DockStyle.Left);

                        return _panelLeft;
                    }
                }

                private IPanelIndicator _panelRight = null;
                private IPanelIndicator PanelRight
                {
                    get
                    {
                        if (_panelRight == null)
                            _panelRight = _dragHandler.DockPanel.Extender.PanelIndicatorFactory.CreatePanelIndicator(DockStyle.Right);

                        return _panelRight;
                    }
                }

                private IPanelIndicator _panelTop = null;
                private IPanelIndicator PanelTop
                {
                    get
                    {
                        if (_panelTop == null)
                            _panelTop = _dragHandler.DockPanel.Extender.PanelIndicatorFactory.CreatePanelIndicator(DockStyle.Top);

                        return _panelTop;
                    }
                }

                private IPanelIndicator _panelBottom = null;
                private IPanelIndicator PanelBottom
                {
                    get
                    {
                        if (_panelBottom == null)
                            _panelBottom = _dragHandler.DockPanel.Extender.PanelIndicatorFactory.CreatePanelIndicator(DockStyle.Bottom);

                        return _panelBottom;
                    }
                }

                private IPanelIndicator _panelFill = null;
                private IPanelIndicator PanelFill
                {
                    get
                    {
                        if (_panelFill == null)
                            _panelFill = _dragHandler.DockPanel.Extender.PanelIndicatorFactory.CreatePanelIndicator(DockStyle.Fill);

                        return _panelFill;
                    }
                }

                private bool _fullPanelEdge = false;
                public bool FullPanelEdge
                {
                    get { return _fullPanelEdge; }
                    set
                    {
                        if (_fullPanelEdge == value)
                            return;

                        _fullPanelEdge = value;
                        RefreshChanges();
                    }
                }

                public DockDragHandler DragHandler
                {
                    get { return _dragHandler; }
                }

                public DockPanel DockPanel
                {
                    get { return DragHandler.DockPanel; }
                }

                private DockPane _dockPane = null;
                public DockPane DockPane
                {
                    get { return _dockPane; }
                    internal set
                    {
                        if (_dockPane == value)
                            return;

                        DockPane oldDisplayingPane = DisplayingPane;
                        _dockPane = value;
                        if (oldDisplayingPane != DisplayingPane)
                            RefreshChanges();
                    }
                }

                private IHitTest _hitTest = null;
                private IHitTest HitTestResult
                {
                    get { return _hitTest; }
                    set
                    {
                        if (_hitTest == value)
                            return;

                        if (_hitTest != null)
                            _hitTest.Status = DockStyle.None;

                        _hitTest = value;
                    }
                }

                private DockPane DisplayingPane
                {
                    get { return ShouldPaneDiamondVisible() ? DockPane : null; }
                }

                private void RefreshChanges()
                {
                    Region region = new Region(Rectangle.Empty);
                    Rectangle rectDockArea = FullPanelEdge ? DockPanel.DockArea : DockPanel.DocumentWindowBounds;

                    rectDockArea = RectangleToClient(DockPanel.RectangleToScreen(rectDockArea));
                    if (ShouldPanelIndicatorVisible(DockState.DockLeft))
                    {
                        PanelLeft.Location = new Point(rectDockArea.X + _PanelIndicatorMargin, rectDockArea.Y + (rectDockArea.Height - PanelRight.Height) / 2);
                        PanelLeft.Visible = true;
                        region.Union(PanelLeft.Bounds);
                    }
                    else
                        PanelLeft.Visible = false;

                    if (ShouldPanelIndicatorVisible(DockState.DockRight))
                    {
                        PanelRight.Location = new Point(rectDockArea.X + rectDockArea.Width - PanelRight.Width - _PanelIndicatorMargin, rectDockArea.Y + (rectDockArea.Height - PanelRight.Height) / 2);
                        PanelRight.Visible = true;
                        region.Union(PanelRight.Bounds);
                    }
                    else
                        PanelRight.Visible = false;

                    if (ShouldPanelIndicatorVisible(DockState.DockTop))
                    {
                        PanelTop.Location = new Point(rectDockArea.X + (rectDockArea.Width - PanelTop.Width) / 2, rectDockArea.Y + _PanelIndicatorMargin);
                        PanelTop.Visible = true;
                        region.Union(PanelTop.Bounds);
                    }
                    else
                        PanelTop.Visible = false;

                    if (ShouldPanelIndicatorVisible(DockState.DockBottom))
                    {
                        PanelBottom.Location = new Point(rectDockArea.X + (rectDockArea.Width - PanelBottom.Width) / 2, rectDockArea.Y + rectDockArea.Height - PanelBottom.Height - _PanelIndicatorMargin);
                        PanelBottom.Visible = true;
                        region.Union(PanelBottom.Bounds);
                    }
                    else
                        PanelBottom.Visible = false;

                    if (ShouldPanelIndicatorVisible(DockState.Document))
                    {
                        Rectangle rectDocumentWindow = RectangleToClient(DockPanel.RectangleToScreen(DockPanel.DocumentWindowBounds));
                        PanelFill.Location = new Point(rectDocumentWindow.X + (rectDocumentWindow.Width - PanelFill.Width) / 2, rectDocumentWindow.Y + (rectDocumentWindow.Height - PanelFill.Height) / 2);
                        PanelFill.Visible = true;
                        region.Union(PanelFill.Bounds);
                    }
                    else
                        PanelFill.Visible = false;

                    if (ShouldPaneDiamondVisible())
                    {
                        Rectangle rect = RectangleToClient(DockPane.RectangleToScreen(DockPane.ClientRectangle));
                        PaneDiamond.Location = new Point(rect.Left + (rect.Width - PaneDiamond.Width) / 2, rect.Top + (rect.Height - PaneDiamond.Height) / 2);
                        PaneDiamond.Visible = true;
                        using (GraphicsPath graphicsPath = PaneDiamond.DisplayingGraphicsPath.Clone() as GraphicsPath)
                        {
                            Point[] pts = new Point[]
                                {
                                    new Point(PaneDiamond.Left, PaneDiamond.Top),
                                    new Point(PaneDiamond.Right, PaneDiamond.Top),
                                    new Point(PaneDiamond.Left, PaneDiamond.Bottom)
                                };
                            using (Matrix matrix = new Matrix(PaneDiamond.ClientRectangle, pts))
                            {
                                graphicsPath.Transform(matrix);
                            }

                            region.Union(graphicsPath);
                        }
                    }
                    else
                        PaneDiamond.Visible = false;

                    Region = region;
                }

                private bool ShouldPanelIndicatorVisible(DockState dockState)
                {
                    if (!Visible)
                        return false;

                    if (DockPanel.DockWindows[dockState].Visible)
                        return false;

                    return DragHandler.DragSource.IsDockStateValid(dockState);
                }

                private bool ShouldPaneDiamondVisible()
                {
                    if (DockPane == null)
                        return false;

                    if (!DockPanel.AllowEndUserNestedDocking)
                        return false;

                    return DragHandler.DragSource.CanDockTo(DockPane);
                }

                public override void Show(bool bActivate)
                {
                    base.Show(bActivate);
                    Bounds = SystemInformation.VirtualScreen;
                    RefreshChanges();
                }

                public void TestDrop()
                {
                    Point pt = Control.MousePosition;
                    DockPane = DockHelper.PaneAtPoint(pt, DockPanel);

                    if (TestDrop(PanelLeft, pt) != DockStyle.None)
                        HitTestResult = PanelLeft;
                    else if (TestDrop(PanelRight, pt) != DockStyle.None)
                        HitTestResult = PanelRight;
                    else if (TestDrop(PanelTop, pt) != DockStyle.None)
                        HitTestResult = PanelTop;
                    else if (TestDrop(PanelBottom, pt) != DockStyle.None)
                        HitTestResult = PanelBottom;
                    else if (TestDrop(PanelFill, pt) != DockStyle.None)
                        HitTestResult = PanelFill;
                    else if (TestDrop(PaneDiamond, pt) != DockStyle.None)
                        HitTestResult = PaneDiamond;
                    else
                        HitTestResult = null;

                    if (HitTestResult != null)
                    {
                        if (HitTestResult is IPaneIndicator)
                            DragHandler.Outline.Show(DockPane, HitTestResult.Status);
                        else
                            DragHandler.Outline.Show(DockPanel, HitTestResult.Status, FullPanelEdge);
                    }
                }

                private static DockStyle TestDrop(IHitTest hitTest, Point pt)
                {
                    return hitTest.Status = hitTest.HitTest(pt);
                }
            }

            public DockDragHandler(DockPanel panel)
                : base(panel)
            {
            }

            public new IDockDragSource DragSource
            {
                get { return base.DragSource as IDockDragSource; }
                set { base.DragSource = value; }
            }

            private DockOutlineBase _outline;
            public DockOutlineBase Outline
            {
                get { return _outline; }
                private set { _outline = value; }
            }

            private DockIndicator _indicator;
            private DockIndicator Indicator
            {
                get { return _indicator; }
                set { _indicator = value; }
            }

            private Rectangle _floatOutlineBounds;
            private Rectangle FloatOutlineBounds
            {
                get { return _floatOutlineBounds; }
                set { _floatOutlineBounds = value; }
            }

            public void BeginDrag(IDockDragSource dragSource)
            {
                DragSource = dragSource;

                if (!BeginDrag())
                {
                    DragSource = null;
                    return;
                }

                Outline = DockPanel.Extender.DockOutlineFactory.CreateDockOutline();
                Indicator = new DockIndicator(this);
                Indicator.Show(false);

                FloatOutlineBounds = DragSource.BeginDrag(StartMousePosition);
            }

            protected override void OnDragging()
            {
                TestDrop();
            }

            protected override void OnEndDrag(bool abort)
            {
                DockPanel.SuspendLayout(true);

                Outline.Close();
                Indicator.Close();

                EndDrag(abort);

                // Queue a request to layout all children controls
                DockPanel.PerformMdiClientLayout();

                DockPanel.ResumeLayout(true, true);

                DragSource.EndDrag();

                DragSource = null;

                // Fire notification
                DockPanel.OnDocumentDragged();
            }

            private void TestDrop()
            {
                Outline.FlagTestDrop = false;

                Indicator.FullPanelEdge = ((Control.ModifierKeys & Keys.Shift) != 0);

                if ((Control.ModifierKeys & Keys.Control) == 0)
                {
                    Indicator.TestDrop();

                    if (!Outline.FlagTestDrop)
                    {
                        DockPane pane = DockHelper.PaneAtPoint(Control.MousePosition, DockPanel);
                        if (pane != null && DragSource.IsDockStateValid(pane.DockState))
                            pane.TestDrop(DragSource, Outline);
                    }

                    if (!Outline.FlagTestDrop && DragSource.IsDockStateValid(DockState.Float))
                    {
                        FloatWindow floatWindow = DockHelper.FloatWindowAtPoint(Control.MousePosition, DockPanel);
                        if (floatWindow != null)
                            floatWindow.TestDrop(DragSource, Outline);
                    }
                }
                else
                    Indicator.DockPane = DockHelper.PaneAtPoint(Control.MousePosition, DockPanel);

                if (!Outline.FlagTestDrop)
                {
                    if (DragSource.IsDockStateValid(DockState.Float))
                    {
                        Rectangle rect = FloatOutlineBounds;
                        rect.Offset(Control.MousePosition.X - StartMousePosition.X, Control.MousePosition.Y - StartMousePosition.Y);
                        Outline.Show(rect);
                    }
                }

                if (!Outline.FlagTestDrop)
                {
                    Cursor.Current = Cursors.No;
                    Outline.Show();
                }
                else
                    Cursor.Current = DragControl.Cursor;
            }

            private void EndDrag(bool abort)
            {
                if (abort)
                    return;

                if (!Outline.FloatWindowBounds.IsEmpty)
                    DragSource.FloatAt(Outline.FloatWindowBounds);
                else if (Outline.DockTo is DockPane)
                {
                    DockPane pane = Outline.DockTo as DockPane;
                    DragSource.DockTo(pane, Outline.Dock, Outline.ContentIndex);
                }
                else if (Outline.DockTo is DockPanel)
                {
                    DockPanel panel = Outline.DockTo as DockPanel;
                    panel.UpdateDockWindowZOrder(Outline.Dock, Outline.FlagFullEdge);
                    DragSource.DockTo(panel, Outline.Dock);
                }
            }
        }

        private DockDragHandler _dockDragHandler = null;
        private DockDragHandler GetDockDragHandler()
        {
            if (_dockDragHandler == null)
                _dockDragHandler = new DockDragHandler(this);
            return _dockDragHandler;
        }

        internal void BeginDrag(IDockDragSource dragSource)
        {
            GetDockDragHandler().BeginDrag(dragSource);
        }
    }
}
