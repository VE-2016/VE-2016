
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace AIMS.Libraries.Forms.Docking
{
    internal class DockIndicator : DragForm
    {
        #region IHitTest
        private interface IHitTest
        {
            DockStyle HitTest(Point pt);
            DockStyle Status { get; set; }
        }
        #endregion

        #region PanelIndicator
        private class PanelIndicator : PictureBox, IHitTest
        {
            private const string _ResourceImagePanelLeft = "DockIndicator.PanelLeft.bmp";
            private const string _ResourceImagePanelLeftActive = "DockIndicator.PanelLeft.Active.bmp";
            private const string _ResourceImagePanelRight = "DockIndicator.PanelRight.bmp";
            private const string _ResourceImagePanelRightActive = "DockIndicator.PanelRight.Active.bmp";
            private const string _ResourceImagePanelTop = "DockIndicator.PanelTop.bmp";
            private const string _ResourceImagePanelTopActive = "DockIndicator.PanelTop.Active.bmp";
            private const string _ResourceImagePanelBottom = "DockIndicator.PanelBottom.bmp";
            private const string _ResourceImagePanelBottomActive = "DockIndicator.PanelBottom.Active.bmp";
            private const string _ResourceImagePanelFill = "DockIndicator.PanelFill.bmp";
            private const string _ResourceImagePanelFillActive = "DockIndicator.PanelFill.Active.bmp";

            private static Image s_imagePanelLeft;
            private static Image s_imagePanelRight;
            private static Image s_imagePanelTop;
            private static Image s_imagePanelBottom;
            private static Image s_imagePanelFill;
            private static Image s_imagePanelLeftActive = null;
            private static Image s_imagePanelRightActive = null;
            private static Image s_imagePanelTopActive = null;
            private static Image s_imagePanelBottomActive = null;
            private static Image s_imagePanelFillActive = null;

            static PanelIndicator()
            {
                s_imagePanelLeft = ResourceHelper.LoadBitmap(_ResourceImagePanelLeft);
                s_imagePanelRight = ResourceHelper.LoadBitmap(_ResourceImagePanelRight);
                s_imagePanelTop = ResourceHelper.LoadBitmap(_ResourceImagePanelTop);
                s_imagePanelBottom = ResourceHelper.LoadBitmap(_ResourceImagePanelBottom);
                s_imagePanelFill = ResourceHelper.LoadBitmap(_ResourceImagePanelFill);
                s_imagePanelLeftActive = ResourceHelper.LoadBitmap(_ResourceImagePanelLeftActive);
                s_imagePanelRightActive = ResourceHelper.LoadBitmap(_ResourceImagePanelRightActive);
                s_imagePanelTopActive = ResourceHelper.LoadBitmap(_ResourceImagePanelTopActive);
                s_imagePanelBottomActive = ResourceHelper.LoadBitmap(_ResourceImagePanelBottomActive);
                s_imagePanelFillActive = ResourceHelper.LoadBitmap(_ResourceImagePanelFillActive);
            }

            public PanelIndicator(DockStyle dockStyle)
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
                return ClientRectangle.Contains(PointToClient(pt)) ? DockStyle : DockStyle.None;
            }
        }
        #endregion PanelIndicator

        #region PaneIndicator
        private class PaneIndicator : PictureBox, IHitTest
        {
            private struct HotSpotIndex
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

            private const string _ResourceBitmapPaneDiamond = "DockIndicator.PaneDiamond.bmp";
            private const string _ResourceBitmapPaneDiamondLeft = "DockIndicator.PaneDiamond.Left.bmp";
            private const string _ResourceBitmapPaneDiamondRight = "DockIndicator.PaneDiamond.Right.bmp";
            private const string _ResourceBitmapPaneDiamondTop = "DockIndicator.PaneDiamond.Top.bmp";
            private const string _ResourceBitmapPaneDiamondBottom = "DockIndicator.PaneDiamond.Bottom.bmp";
            private const string _ResourceBitmapPaneDiamondFill = "DockIndicator.PaneDiamond.Fill.bmp";
            private const string _ResourceBitmapPaneDiamondHotSpot = "DockIndicator.PaneDiamond.HotSpot.bmp";
            private const string _ResourceBitmapPaneDiamondHotSpotIndex = "DockIndicator.PaneDiamond.HotSpotIndex.bmp";

            private static Bitmap s_bitmapPaneDiamond;
            private static Bitmap s_bitmapPaneDiamondLeft;
            private static Bitmap s_bitmapPaneDiamondRight;
            private static Bitmap s_bitmapPaneDiamondTop;
            private static Bitmap s_bitmapPaneDiamondBottom;
            private static Bitmap s_bitmapPaneDiamondFill;
            private static Bitmap s_bitmapPaneDiamondHotSpot;
            private static Bitmap s_bitmapPaneDiamondHotSpotIndex;
            private static HotSpotIndex[] s_hotSpots;
            private static GraphicsPath s_displayingGraphicsPath;

            static PaneIndicator()
            {
                s_bitmapPaneDiamond = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamond);
                s_bitmapPaneDiamondLeft = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondLeft);
                s_bitmapPaneDiamondRight = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondRight);
                s_bitmapPaneDiamondTop = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondTop);
                s_bitmapPaneDiamondBottom = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondBottom);
                s_bitmapPaneDiamondFill = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondFill);
                s_bitmapPaneDiamondHotSpot = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondHotSpot);
                s_bitmapPaneDiamondHotSpotIndex = ResourceHelper.LoadBitmap(_ResourceBitmapPaneDiamondHotSpotIndex);
                s_hotSpots = new HotSpotIndex[]
                    {
                        new HotSpotIndex(1, 0, DockStyle.Top),
                        new HotSpotIndex(0, 1, DockStyle.Left),
                        new HotSpotIndex(1, 1, DockStyle.Fill),
                        new HotSpotIndex(2, 1, DockStyle.Right),
                        new HotSpotIndex(1, 2, DockStyle.Bottom)
                    };
                s_displayingGraphicsPath = DrawHelper.CalculateGraphicsPathFromBitmap(s_bitmapPaneDiamond);
            }

            public PaneIndicator()
            {
                SizeMode = PictureBoxSizeMode.AutoSize;
                Image = s_bitmapPaneDiamond;
                Region = new Region(DisplayingGraphicsPath);
            }

            public GraphicsPath DisplayingGraphicsPath
            {
                get { return s_displayingGraphicsPath; }
            }

            public DockStyle HitTest(Point pt)
            {
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

        #region consts
        private int _PanelIndicatorMargin = 10;
        #endregion

        public DockIndicator(DragHandler dragHandler)
        {
            _dragHandler = dragHandler;
            Controls.AddRange(new Control[]
                {
                    PaneDiamond,
                    PanelLeft,
                    PanelRight,
                    PanelTop,
                    PanelBottom,
                    PanelFill
                });
            Bounds = GetAllScreenBounds();
            Region = new Region(Rectangle.Empty);
        }

        private PaneIndicator _paneDiamond = null;
        private PaneIndicator PaneDiamond
        {
            get
            {
                if (_paneDiamond == null)
                    _paneDiamond = new PaneIndicator();

                return _paneDiamond;
            }
        }

        private PanelIndicator _panelLeft = null;
        private PanelIndicator PanelLeft
        {
            get
            {
                if (_panelLeft == null)
                    _panelLeft = new PanelIndicator(DockStyle.Left);

                return _panelLeft;
            }
        }

        private PanelIndicator _panelRight = null;
        private PanelIndicator PanelRight
        {
            get
            {
                if (_panelRight == null)
                    _panelRight = new PanelIndicator(DockStyle.Right);

                return _panelRight;
            }
        }

        private PanelIndicator _panelTop = null;
        private PanelIndicator PanelTop
        {
            get
            {
                if (_panelTop == null)
                    _panelTop = new PanelIndicator(DockStyle.Top);

                return _panelTop;
            }
        }

        private PanelIndicator _panelBottom = null;
        private PanelIndicator PanelBottom
        {
            get
            {
                if (_panelBottom == null)
                    _panelBottom = new PanelIndicator(DockStyle.Bottom);

                return _panelBottom;
            }
        }

        private PanelIndicator _panelFill = null;
        private PanelIndicator PanelFill
        {
            get
            {
                if (_panelFill == null)
                    _panelFill = new PanelIndicator(DockStyle.Fill);

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

        private DragHandler _dragHandler;
        public DragHandler DragHandler
        {
            get { return _dragHandler; }
        }

        public DockContainer DockPanel
        {
            get { return DragHandler.DockPanel; }
        }

        private DockPane _dockPane = null;
        public DockPane DockPane
        {
            get { return _dockPane; }
            set
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

            rectDockArea.Location = DockPanel.PointToScreen(rectDockArea.Location);
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
                Rectangle rectDocumentWindow = DockPanel.DocumentWindowBounds;
                rectDocumentWindow.Location = DockPanel.PointToScreen(rectDocumentWindow.Location);
                PanelFill.Location = new Point(rectDocumentWindow.X + (rectDocumentWindow.Width - PanelFill.Width) / 2, rectDocumentWindow.Y + (rectDocumentWindow.Height - PanelFill.Height) / 2);
                PanelFill.Visible = true;
                region.Union(PanelFill.Bounds);
            }
            else
                PanelFill.Visible = false;

            if (ShouldPaneDiamondVisible())
            {
                Rectangle rect = DockPane.ClientRectangle;
                rect.Location = DockPane.PointToScreen(rect.Location);
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

            return DragHandler.IsDockStateValid(dockState);
        }

        private bool ShouldPaneDiamondVisible()
        {
            if (DockPane == null)
                return false;

            if (DragHandler.DragControl == DockPane)
                return false;

            if (DragHandler.DragControl == DockPane.DockListContainer)
                return false;

            IDockableWindow content = DragHandler.DragControl as IDockableWindow;
            if (content != null && DockPane.Contents.Contains(content) && DockPane.DisplayingContents.Count == 1)
                return false;

            return DragHandler.IsDockStateValid(DockPane.DockState);
        }

        public override void Show(bool bActivate)
        {
            base.Show(bActivate);
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
                if (HitTestResult is PaneIndicator)
                    DragHandler.DockOutline.Show(DockPane, HitTestResult.Status);
                else
                    DragHandler.DockOutline.Show(DockPanel, HitTestResult.Status, FullPanelEdge);
            }
        }

        private DockStyle TestDrop(IHitTest hitTest, Point pt)
        {
            return hitTest.Status = hitTest.HitTest(pt);
        }

        private Rectangle GetAllScreenBounds()
        {
            Rectangle rect = new Rectangle(0, 0, 0, 0);
            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle rectScreen = screen.Bounds;
                if (rectScreen.Left < rect.Left)
                {
                    rect.Width += (rect.Left - rectScreen.Left);
                    rect.X = rectScreen.X;
                }
                if (rectScreen.Right > rect.Right)
                    rect.Width += (rectScreen.Right - rect.Right);
                if (rectScreen.Top < rect.Top)
                {
                    rect.Height += (rect.Top - rectScreen.Top);
                    rect.Y = rectScreen.Y;
                }
                if (rectScreen.Bottom > rect.Bottom)
                    rect.Height += (rectScreen.Bottom - rect.Bottom);
            }

            return rect;
        }
    }
}
