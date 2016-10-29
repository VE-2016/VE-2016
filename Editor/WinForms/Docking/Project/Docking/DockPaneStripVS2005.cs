
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/ClassDef/*'/>
    [ToolboxItem(false)]
    public class DockPaneStripVS2005 : DockPaneStripBase
    {
        #region consts
        private const int _ToolWindowStripGapLeft = 4;
        private const int _ToolWindowStripGapRight = 3;
        private const int _ToolWindowImageHeight = 16;
        private const int _ToolWindowImageWidth = 16;
        private const int _ToolWindowImageGapTop = 3;
        private const int _ToolWindowImageGapBottom = 1;
        private const int _ToolWindowImageGapLeft = 3;
        private const int _ToolWindowImageGapRight = 2;
        private const int _ToolWindowTextGapRight = 1;
        private const int _ToolWindowTabSeperatorGapTop = 3;
        private const int _ToolWindowTabSeperatorGapBottom = 3;

        private const int _DocumentTabMaxWidth = 200;
        private const int _DocumentButtonGapTop = 5;
        private const int _DocumentButtonGapBottom = 5;
        private const int _DocumentButtonGapBetween = 0;
        private const int _DocumentButtonGapRight = 3;
        private const int _DocumentTabGapTop = 3;
        private const int _DocumentTabGapLeft = 3;
        private const int _DocumentTabGapRight = 10;
        private const int _DocumentTextExtraHeight = 6;
        private const int _DocumentTextExtraWidth = 10;
        private const int _DocumentIconGapLeft = 6;
        private const int _DocumentIconHeight = 16;
        private const int _DocumentIconWidth = 16;

        private const string _ResourceImageCloseEnabled = "DockPaneStrip.CloseEnabled.bmp";
        private const string _ResourceImageCloseDisabled = "DockPaneStrip.CloseDisabled.bmp";
        private const string _ResourceImageScrollLeftEnabled = "DockPaneStrip.ScrollLeftEnabled.bmp";
        private const string _ResourceImageScrollLeftDisabled = "DockPaneStrip.ScrollLeftDisabled.bmp";
        private const string _ResourceImageScrollRightEnabled = "DockPaneStrip.ScrollRightEnabled.bmp";
        private const string _ResourceImageScrollRightDisabled = "DockPaneStrip.ScrollRightDisabled.bmp";
        private const string _ResourceToolTipClose = "DockPaneStrip.ToolTipClose";
        private const string _ResourceToolTipScrollLeft = "DockPaneStrip.ToolTipScrollLeft";
        private const string _ResourceToolTipScrollRight = "DockPaneStrip.ToolTipScrollRight";
        #endregion

        private InertButton _buttonClose,_buttonScrollLeft,_buttonScrollRight;
        private IContainer _components;
        private ToolTip _toolTip;

        /// <exclude/>
        protected IContainer Components
        {
            get { return _components; }
        }

        private int _offsetX = 0;
        private int OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
#if DEBUG
                if (_offsetX > 0)
                    throw new InvalidOperationException();
#endif
            }
        }

        #region Customizable Properties
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowStripGapLeft"]/*'/>
        protected virtual int ToolWindowStripGapLeft
        {
            get { return _ToolWindowStripGapLeft; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowStripGapRight"]/*'/>
        protected virtual int ToolWindowStripGapRight
        {
            get { return _ToolWindowStripGapRight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageHeight"]/*'/>
        protected virtual int ToolWindowImageHeight
        {
            get { return _ToolWindowImageHeight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageWidth"]/*'/>
        protected virtual int ToolWindowImageWidth
        {
            get { return _ToolWindowImageWidth; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapTop"]/*'/>
        protected virtual int ToolWindowImageGapTop
        {
            get { return _ToolWindowImageGapTop; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapBottom"]/*'/>
        protected virtual int ToolWindowImageGapBottom
        {
            get { return _ToolWindowImageGapBottom; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapLeft"]/*'/>
        protected virtual int ToolWindowImageGapLeft
        {
            get { return _ToolWindowImageGapLeft; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowImageGapRight"]/*'/>
        protected virtual int ToolWindowImageGapRight
        {
            get { return _ToolWindowImageGapRight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowTextGapRight"]/*'/>
        protected virtual int ToolWindowTextGapRight
        {
            get { return _ToolWindowTextGapRight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowSeperatorGaptop"]/*'/>
        protected virtual int ToolWindowTabSeperatorGapTop
        {
            get { return _ToolWindowTabSeperatorGapTop; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowSeperatorGapBottom"]/*'/>
        protected virtual int ToolWindowTabSeperatorGapBottom
        {
            get { return _ToolWindowTabSeperatorGapBottom; }
        }

        private static Image s_imageCloseEnabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageCloseEnabled"]/*'/>
        protected virtual Image ImageCloseEnabled
        {
            get
            {
                if (s_imageCloseEnabled == null)
                    s_imageCloseEnabled = ResourceHelper.LoadBitmap(_ResourceImageCloseEnabled);
                return s_imageCloseEnabled;
            }
        }

        private static Image s_imageCloseDisabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageCloseDisabled"]/*'/>
        protected virtual Image ImageCloseDisabled
        {
            get
            {
                if (s_imageCloseDisabled == null)
                    s_imageCloseDisabled = ResourceHelper.LoadBitmap(_ResourceImageCloseDisabled);
                return s_imageCloseDisabled;
            }
        }

        private static Image s_imageScrollLeftEnabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollLeftEnabled"]/*'/>
        protected virtual Image ImageScrollLeftEnabled
        {
            get
            {
                if (s_imageScrollLeftEnabled == null)
                    s_imageScrollLeftEnabled = ResourceHelper.LoadBitmap(_ResourceImageScrollLeftEnabled);
                return s_imageScrollLeftEnabled;
            }
        }

        private static Image s_imageScrollLeftDisabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollLeftDisabled"]/*'/>
        protected virtual Image ImageScrollLeftDisabled
        {
            get
            {
                if (s_imageScrollLeftDisabled == null)
                    s_imageScrollLeftDisabled = ResourceHelper.LoadBitmap(_ResourceImageScrollLeftDisabled);
                return s_imageScrollLeftDisabled;
            }
        }

        private static Image s_imageScrollRightEnabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollRightEnabled"]/*'/>
        protected virtual Image ImageScrollRightEnabled
        {
            get
            {
                if (s_imageScrollRightEnabled == null)
                    s_imageScrollRightEnabled = ResourceHelper.LoadBitmap(_ResourceImageScrollRightEnabled);
                return s_imageScrollRightEnabled;
            }
        }

        private static Image s_imageScrollRightDisabled = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ImageScrollRightDisabled"]/*'/>
        protected virtual Image ImageScrollRightDisabled
        {
            get
            {
                if (s_imageScrollRightDisabled == null)
                    s_imageScrollRightDisabled = ResourceHelper.LoadBitmap(_ResourceImageScrollRightDisabled);
                return s_imageScrollRightDisabled;
            }
        }

        private static string s_toolTipClose = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipClose"]/*'/>
        protected virtual string ToolTipClose
        {
            get
            {
                if (s_toolTipClose == null)
                    s_toolTipClose = ResourceHelper.GetString(_ResourceToolTipClose);
                return s_toolTipClose;
            }
        }

        private static string s_toolTipScrollLeft = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipScrollLeft"]/*'/>
        protected virtual string ToolTipScrollLeft
        {
            get
            {
                if (s_toolTipScrollLeft == null)
                    s_toolTipScrollLeft = ResourceHelper.GetString(_ResourceToolTipScrollLeft);
                return s_toolTipScrollLeft;
            }
        }

        private static string s_toolTipScrollRight = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolTipScrollRight"]/*'/>
        protected virtual string ToolTipScrollRight
        {
            get
            {
                if (s_toolTipScrollRight == null)
                    s_toolTipScrollRight = ResourceHelper.GetString(_ResourceToolTipScrollRight);
                return s_toolTipScrollRight;
            }
        }

        private static StringFormat s_toolWindowTextStringFormat = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ToolWindowTextStringFormat"]/*'/>
        protected virtual StringFormat ToolWindowTextStringFormat
        {
            get
            {
                if (s_toolWindowTextStringFormat == null)
                {
                    s_toolWindowTextStringFormat = new StringFormat(StringFormat.GenericTypographic);
                    s_toolWindowTextStringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    s_toolWindowTextStringFormat.LineAlignment = StringAlignment.Center;
                    s_toolWindowTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;
                }
                return s_toolWindowTextStringFormat;
            }
        }

        private static StringFormat s_documentTextStringFormat = null;
        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTextStringFormat"]/*'/>
        public static StringFormat DocumentTextStringFormat
        {
            get
            {
                if (s_documentTextStringFormat == null)
                {
                    s_documentTextStringFormat = new StringFormat(StringFormat.GenericTypographic);
                    s_documentTextStringFormat.Alignment = StringAlignment.Near;
                    s_documentTextStringFormat.Trimming = StringTrimming.EllipsisCharacter;
                    s_documentTextStringFormat.LineAlignment = StringAlignment.Center;
                    s_documentTextStringFormat.FormatFlags = StringFormatFlags.NoWrap;
                }
                return s_documentTextStringFormat;
            }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabMaxWidth"]/*'/>
        protected virtual int DocumentTabMaxWidth
        {
            get { return _DocumentTabMaxWidth; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapTop"]/*'/>
        protected virtual int DocumentButtonGapTop
        {
            get { return _DocumentButtonGapTop; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapBottom"]/*'/>
        protected virtual int DocumentButtonGapBottom
        {
            get { return _DocumentButtonGapBottom; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapBetween"]/*'/>
        protected virtual int DocumentButtonGapBetween
        {
            get { return _DocumentButtonGapBetween; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentButtonGapRight"]/*'/>
        protected virtual int DocumentButtonGapRight
        {
            get { return _DocumentButtonGapRight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapTop"]/*'/>
        protected virtual int DocumentTabGapTop
        {
            get { return _DocumentTabGapTop; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapLeft"]/*'/>
        protected virtual int DocumentTabGapLeft
        {
            get { return _DocumentTabGapLeft; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTabGapRight"]/*'/>
        protected virtual int DocumentTabGapRight
        {
            get { return _DocumentTabGapRight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTextExtraHeight"]/*'/>
        protected int DocumentTextExtraHeight
        {
            get { return _DocumentTextExtraHeight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentTextExtraWidth"]/*'/>
        protected virtual int DocumentTextExtraWidth
        {
            get { return _DocumentTextExtraWidth; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconGapLeft"]/*'/>
        protected virtual int DocumentIconGapLeft
        {
            get { return _DocumentIconGapLeft; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconWidth"]/*'/>
        protected virtual int DocumentIconWidth
        {
            get { return _DocumentIconWidth; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="DocumentIconHeight"]/*'/>
        protected virtual int DocumentIconHeight
        {
            get { return _DocumentIconHeight; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="OutlineInnerPen"]/*'/>
        protected virtual Pen OutlineInnerPen
        {
            get { return SystemPens.ControlDark; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="OutlineOuterPen"]/*'/>
        protected virtual Pen OutlineOuterPen
        {
            get { return SystemPens.ControlDark; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ActiveBackBrush"]/*'/>
        protected virtual Color ActiveBackColor1
        {
            get
            {
                return SystemColors.Control;
            }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ActiveBackBrush"]/*'/>
        protected virtual Color ActiveBackColor2
        {
            get
            {
                return SystemColors.ControlLightLight;
            }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="ActiveTextBrush"]/*'/>
        protected virtual Brush ActiveTextBrush
        {
            get { return SystemBrushes.ControlText; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="TabSeperatorPen"]/*'/>
        protected virtual Pen TabSeperatorPen
        {
            get { return SystemPens.GrayText; }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Property[@name="InactiveTextBrush"]/*'/>
        protected virtual Brush InactiveTextBrush
        {
            get { return SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark); }
        }
        #endregion

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Construct[@name="(DockPane)"]/*'/>
        protected internal DockPaneStripVS2005(DockPane pane)
            : base(pane)
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            SuspendLayout();

            Font = SystemInformation.MenuFont;
            BackColor = SystemColors.Control;

            _components = new Container();
            _toolTip = new ToolTip(Components);

            _buttonClose = new InertButton(ImageCloseEnabled, ImageCloseDisabled);
            _buttonScrollLeft = new InertButton(ImageScrollLeftEnabled, ImageScrollLeftDisabled);
            _buttonScrollRight = new InertButton(ImageScrollRightEnabled, ImageScrollRightDisabled);

            _buttonClose.ToolTipText = ToolTipClose;
            _buttonClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _buttonClose.Click += new EventHandler(Close_Click);

            _buttonScrollLeft.Enabled = false;
            _buttonScrollLeft.RepeatClick = true;
            _buttonScrollLeft.ToolTipText = ToolTipScrollLeft;
            _buttonScrollLeft.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _buttonScrollLeft.Click += new EventHandler(ScrollLeft_Click);

            _buttonScrollRight.Enabled = false;
            _buttonScrollRight.RepeatClick = true;
            _buttonScrollRight.ToolTipText = ToolTipScrollRight;
            _buttonScrollRight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _buttonScrollRight.Click += new EventHandler(ScrollRight_Click);

            Controls.AddRange(new Control[] {   _buttonClose,
                                                _buttonScrollLeft,
                                                _buttonScrollRight });

            ResumeLayout();
        }

        /// <exclude/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <exclude />
        protected internal override int MeasureHeight()
        {
            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                return MeasureHeight_ToolWindow();
            else
                return MeasureHeight_Document();
        }

        private int MeasureHeight_ToolWindow()
        {
            if (DockPane.IsAutoHide || Tabs.Count <= 1)
                return 0;

            int height = Math.Max(Font.Height, ToolWindowImageHeight)
                + ToolWindowImageGapTop + ToolWindowImageGapBottom;

            // return 0;

            return height;
        }

        private int MeasureHeight_Document()
        {
            int height = Math.Max(Font.Height + DocumentTabGapTop + DocumentTextExtraHeight,
                ImageCloseEnabled.Height + DocumentButtonGapTop + DocumentButtonGapBottom);

            //return 0;

            return height;
        }

        /// <exclude />
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            CalculateTabs();
            DrawTabStrip(e.Graphics);
        }

        /// <exclude />
        protected override void OnRefreshChanges()
        {
            CalculateTabs();
            SetInertButtons();
            Invalidate();
        }

        /// <exclude />
        protected internal override GraphicsPath GetOutlinePath(int index)
        {
            Point[] pts = new Point[8];

            if (Appearance == DockPane.AppearanceStyle.Document)
            {
                Rectangle rectTab = GetTabRectangle(index);
                rectTab.Intersect(TabsRectangle);
                int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Bottom))).Y;
                Rectangle rectPaneClient = DockPane.ClientRectangle;
                pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
                pts[1] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
                pts[2] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
                pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
                pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
                pts[5] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
                pts[6] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Bottom));
                pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Bottom));
            }
            else
            {
                Rectangle rectTab = GetTabRectangle(index);
                rectTab.Intersect(TabsRectangle);
                int y = DockPane.PointToClient(PointToScreen(new Point(0, rectTab.Top))).Y;
                Rectangle rectPaneClient = DockPane.ClientRectangle;
                pts[0] = DockPane.PointToScreen(new Point(rectPaneClient.Left, rectPaneClient.Top));
                pts[1] = DockPane.PointToScreen(new Point(rectPaneClient.Right, rectPaneClient.Top));
                pts[2] = DockPane.PointToScreen(new Point(rectPaneClient.Right, y));
                pts[3] = PointToScreen(new Point(rectTab.Right, rectTab.Top));
                pts[4] = PointToScreen(new Point(rectTab.Right, rectTab.Bottom));
                pts[5] = PointToScreen(new Point(rectTab.Left, rectTab.Bottom));
                pts[6] = PointToScreen(new Point(rectTab.Left, rectTab.Top));
                pts[7] = DockPane.PointToScreen(new Point(rectPaneClient.Left, y));
            }

            GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);
            return path;
        }

        private void CalculateTabs()
        {
            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                CalculateTabs_ToolWindow();
            else
                CalculateTabs_Document();
        }

        private void CalculateTabs_ToolWindow()
        {
            if (Tabs.Count <= 1 || DockPane.IsAutoHide)
                return;

            Rectangle rectTabStrip = ClientRectangle;

            // Calculate tab widths
            int countTabs = Tabs.Count;
            foreach (DockPaneTabVS2005 tab in Tabs)
            {
                tab.MaxWidth = GetTabOriginalWidth(Tabs.IndexOf(tab));
                tab.Flag = false;
            }

            // Set tab whose max width less than average width
            bool anyWidthWithinAverage = true;
            int totalWidth = rectTabStrip.Width - ToolWindowStripGapLeft - ToolWindowStripGapRight;
            int totalAllocatedWidth = 0;
            int averageWidth = totalWidth / countTabs;
            int remainedTabs = countTabs;
            for (anyWidthWithinAverage = true; anyWidthWithinAverage && remainedTabs > 0;)
            {
                anyWidthWithinAverage = false;
                foreach (DockPaneTabVS2005 tab in Tabs)
                {
                    if (tab.Flag)
                        continue;

                    if (tab.MaxWidth <= averageWidth)
                    {
                        tab.Flag = true;
                        tab.TabWidth = tab.MaxWidth;
                        totalAllocatedWidth += tab.TabWidth;
                        anyWidthWithinAverage = true;
                        remainedTabs--;
                    }
                }
                if (remainedTabs != 0)
                    averageWidth = (totalWidth - totalAllocatedWidth) / remainedTabs;
            }

            // If any tab width not set yet, set it to the average width
            if (remainedTabs > 0)
            {
                int roundUpWidth = (totalWidth - totalAllocatedWidth) - (averageWidth * remainedTabs);
                foreach (DockPaneTabVS2005 tab in Tabs)
                {
                    if (tab.Flag)
                        continue;

                    tab.Flag = true;
                    if (roundUpWidth > 0)
                    {
                        tab.TabWidth = averageWidth + 1;
                        roundUpWidth--;
                    }
                    else
                        tab.TabWidth = averageWidth;
                }
            }

            // Set the X position of the tabs
            int x = rectTabStrip.X + ToolWindowStripGapLeft;
            foreach (DockPaneTabVS2005 tab in Tabs)
            {
                tab.TabX = x;
                x += tab.TabWidth;
            }
        }

        private void CalculateTabs_Document()
        {
            Rectangle rectTabStrip = TabsRectangle;

            int totalWidth = 0;
            foreach (DockPaneTabVS2005 tab in Tabs)
            {
                tab.TabWidth = Math.Min(GetTabOriginalWidth(Tabs.IndexOf(tab)), DocumentTabMaxWidth);
                totalWidth += tab.TabWidth;
            }

            if (totalWidth + OffsetX < rectTabStrip.Width && OffsetX < 0)
                OffsetX = Math.Min(0, rectTabStrip.Width - totalWidth);

            int x = rectTabStrip.X + OffsetX;
            foreach (DockPaneTabVS2005 tab in Tabs)
            {
                tab.TabX = x;
                x += tab.TabWidth + 5;
            }
        }

        /// <exclude />
        protected internal override void EnsureTabVisible(IDockableWindow content)
        {
            if (Appearance != DockPane.AppearanceStyle.Document || !Tabs.Contains(content))
                return;

            Rectangle rectTabStrip = TabsRectangle;
            Rectangle rectTab = GetTabRectangle(Tabs.IndexOf(content));

            if (rectTab.Right > rectTabStrip.Right)
            {
                OffsetX -= rectTab.Right - rectTabStrip.Right;
                rectTab.X -= rectTab.Right - rectTabStrip.Right;
            }

            if (rectTab.Left < rectTabStrip.Left)
                OffsetX += rectTabStrip.Left - rectTab.Left;

            OnRefreshChanges();
        }

        private int GetTabOriginalWidth(int index)
        {
            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                return GetTabOriginalWidth_ToolWindow(index);
            else
                return GetTabOriginalWidth_Document(index);
        }

        private int GetTabOriginalWidth_ToolWindow(int index)
        {
            IDockableWindow content = Tabs[index].Content;
            using (Graphics g = CreateGraphics())
            {
                SizeF sizeString = g.MeasureString(content.DockHandler.TabText, Font);
                return ToolWindowImageWidth + (int)sizeString.Width + 1 + ToolWindowImageGapLeft
                    + ToolWindowImageGapRight + ToolWindowTextGapRight;
            }
        }

        private int GetTabOriginalWidth_Document(int index)
        {
            IDockableWindow content = Tabs[index].Content;

            using (Graphics g = CreateGraphics())
            {
                SizeF sizeText;
                if (content == DockPane.ActiveContent && DockPane.IsActiveDocumentPane)
                {
                    using (Font boldFont = new Font(this.Font, FontStyle.Bold))
                    {
                        sizeText = g.MeasureString(content.DockHandler.TabText, boldFont, DocumentTabMaxWidth, DocumentTextStringFormat);
                    }
                }
                else
                    sizeText = g.MeasureString(content.DockHandler.TabText, Font, DocumentTabMaxWidth, DocumentTextStringFormat);

                if (DockPane.DockPanel.ShowDocumentIcon)
                    return (int)sizeText.Width + 1 + DocumentTextExtraWidth + DocumentIconWidth + DocumentIconGapLeft;
                else
                    return (int)sizeText.Width + 1 + DocumentTextExtraWidth;
            }
        }

        private void DrawTabStrip(Graphics g)
        {
            OnBeginDrawTabStrip();

            if (Appearance == DockPane.AppearanceStyle.Document)
                DrawTabStrip_Document(g);
            else
                DrawTabStrip_ToolWindow(g);

            OnEndDrawTabStrip();
        }

        private void DrawTabStrip_Document(Graphics g)
        {
            int count = Tabs.Count;
            if (count == 0)
                return;

            Rectangle rectTabStrip = ClientRectangle;
            g.DrawLine(OutlineOuterPen, rectTabStrip.Left, rectTabStrip.Bottom - 1,
                rectTabStrip.Right, rectTabStrip.Bottom - 1);

            // Draw the tabs
            Rectangle rectTabOnly = TabsRectangle;
            Rectangle rectTab = Rectangle.Empty;
            g.SetClip(rectTabOnly);
            for (int i = 0; i < count; i++)
            {
                rectTab = GetTabRectangle(i);
                if (rectTab.IntersectsWith(rectTabOnly))
                    DrawTab(g, Tabs[i].Content, rectTab);
            }

            if (DockPane.ActiveContent != null)
            {
                rectTab = GetTabRectangle(this.Tabs.IndexOf(DockPane.ActiveContent.DockHandler.DockPaneTab));
                DrawTab(g, DockPane.ActiveContent, rectTab);
            }
        }

        private void DrawTabStrip_ToolWindow(Graphics g)
        {
            Rectangle rectTabStrip = ClientRectangle;

            g.DrawLine(OutlineInnerPen, rectTabStrip.Left, rectTabStrip.Top,
                rectTabStrip.Right, rectTabStrip.Top);

            for (int i = 0; i < Tabs.Count; i++)
                DrawTab(g, Tabs[i].Content, GetTabRectangle(i));
        }

        private Rectangle GetTabRectangle(int index)
        {
            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                return GetTabRectangle_ToolWindow(index);
            else
                return GetTabRectangle_Document(index);
        }

        private Rectangle GetTabRectangle_ToolWindow(int index)
        {
            Rectangle rectTabStrip = ClientRectangle;

            DockPaneTabVS2005 tab = (DockPaneTabVS2005)(Tabs[index]);
            return new Rectangle(tab.TabX, rectTabStrip.Y, tab.TabWidth, rectTabStrip.Height);
        }

        private Rectangle GetTabRectangle_Document(int index)
        {
            Rectangle rectTabStrip = ClientRectangle;
            DockPaneTabVS2005 tab = (DockPaneTabVS2005)Tabs[index];

            if (tab == null)
                return new Rectangle();

            Font font = new Font(this.Font, FontStyle.Bold);

            SizeF size = this.CreateGraphics().MeasureString(tab.Content.DockHandler.TabText, font);

            int w = (int)(size.Width + size.Height / 2) + (int)font.Size;

            w = Math.Min(w, this.DocumentTabMaxWidth + this.DocumentTextExtraWidth * 2);

            if (w > 0)
                tab.TabWidth = w;

            int x = tab.TabX;

            if (index > 0)
            {
                x += ((int)(font.Size / 1.5f) * index);

                //x += (int)font.Size * index;
            }
            // x -= m_offsetX;



            return new Rectangle(x, rectTabStrip.Y + DocumentTabGapTop, w /*tab.TabWidth*/, rectTabStrip.Height - DocumentTabGapTop);
        }

        private void DrawTab(Graphics g, IDockableWindow content, Rectangle rect)
        {
            OnBeginDrawTab(content.DockHandler.DockPaneTab);

            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                DrawTab_ToolWindow(g, content, rect);
            else
                DrawTab_Document(g, content, rect);

            OnEndDrawTab(content.DockHandler.DockPaneTab);
        }

        private void DrawTab_ToolWindow(Graphics g, IDockableWindow content, Rectangle rect)
        {
            try
            {
                GraphicsPath gp = new GraphicsPath();

                gp.AddLine(rect.X, rect.Top - 1, rect.Right - 1, rect.Top - 1);
                gp.AddLine(rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 3);
                gp.AddLine(rect.Right - 3, rect.Bottom - 1, rect.X + 2, rect.Bottom - 1);
                gp.AddLine(rect.X, rect.Bottom - 3, rect.X, rect.Top);
                gp.CloseAllFigures();


                Rectangle rectIcon = new Rectangle(
                    rect.X + ToolWindowImageGapLeft,
                    rect.Y + rect.Height - 1 - ToolWindowImageGapBottom - ToolWindowImageHeight,
                    ToolWindowImageWidth, ToolWindowImageHeight);
                Rectangle rectText = rectIcon;
                rectText.X += rectIcon.Width + ToolWindowImageGapRight;
                rectText.Width = rect.Width - rectIcon.Width - ToolWindowImageGapLeft -
                    ToolWindowImageGapRight - ToolWindowTextGapRight;

                if (DockPane.ActiveContent == content)
                {
                    LinearGradientBrush lnbr = new LinearGradientBrush(rect, this.ActiveBackColor2,
                        this.ActiveBackColor1, LinearGradientMode.Vertical);

                    g.FillPath(lnbr, gp);

                    g.DrawPath(OutlineOuterPen, gp);


                    /* g.FillRectangle(lnbr, rect);

                     g.DrawLine(OutlineOuterPen,
                         rect.X, rect.Y, rect.X, rect.Y + rect.Height - 1);
                     g.DrawLine(OutlineInnerPen,
                         rect.X, rect.Y + rect.Height - 1, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);
                     g.DrawLine(OutlineInnerPen,
                         rect.X + rect.Width - 1, rect.Y, rect.X + rect.Width - 1, rect.Y + rect.Height - 1);*/


                    g.DrawString(content.DockHandler.TabText, Font, ActiveTextBrush, rectText, ToolWindowTextStringFormat);
                }
                else
                {
                    if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(content) + 1)
                        g.DrawLine(TabSeperatorPen,
                            rect.X + rect.Width - 1,
                            rect.Y + ToolWindowTabSeperatorGapTop,
                            rect.X + rect.Width - 1,
                            rect.Y + rect.Height - 1 - ToolWindowTabSeperatorGapBottom);
                    g.DrawString(content.DockHandler.TabText, Font, InactiveTextBrush, rectText, ToolWindowTextStringFormat);
                }

                if (rect.Contains(rectIcon))
                    g.DrawIcon(content.DockHandler.Icon, rectIcon);
            }
            catch (Exception ee) { }
        }

        private void DrawTab_Document(Graphics g, IDockableWindow content, Rectangle rect)
        {
            try
            {
                SmoothingMode mode = g.SmoothingMode;

                g.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath gp = new GraphicsPath();

                gp.AddLine(rect.X, rect.Bottom, rect.X + (rect.Height / 2), rect.Top + 2);

                gp.AddLine(rect.X + (rect.Height / 2) + 4, rect.Top, rect.Right - 2, rect.Top);

                gp.AddLine(rect.Right, rect.Top + 2, rect.Right, rect.Bottom);

                string text = content.DockHandler.TabText;

                if (content.Saved == false)
                    text = text + "*";

                Rectangle rectText = rect;

                if (DockPane.DockPanel.ShowDocumentIcon)
                {
                    rectText.X += DocumentTextExtraWidth / 2 + DocumentIconWidth + DocumentIconGapLeft;
                    rectText.Width -= DocumentTextExtraWidth + DocumentIconWidth + DocumentIconGapLeft;
                }
                else
                {
                    rectText.X += DocumentTextExtraWidth / 2;
                    rectText.Width -= DocumentTextExtraWidth;
                }


                rectText.X += (rect.Height / 2);
                rectText.Width -= (rect.Height / 2);

                if (DockPane.ActiveContent == content)
                {
                    /*g.FillRectangle(ActiveBackBrush, rect);
                    g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
                    g.DrawLine(OutlineOuterPen, rect.X, rect.Y, rect.X + rect.Width - 1, rect.Y);
                    g.DrawLine(OutlineInnerPen,
                        rect.X + rect.Width - 1, rect.Y,
                        rect.X + rect.Width - 1, rect.Y + rect.Height - 1);*/

                    LinearGradientBrush lnbr = new LinearGradientBrush(rect, this.ActiveBackColor2,
                        this.ActiveBackColor1, LinearGradientMode.Vertical);

                    g.FillPath(lnbr, gp);

                    g.DrawPath(OutlineOuterPen, gp);

                    if (DockPane.DockPanel.ShowDocumentIcon)
                    {
                        Icon icon = (content as Form).Icon;
                        Rectangle rectIcon = new Rectangle(
                            rect.X + DocumentIconGapLeft,
                            rect.Y + (rect.Height - DocumentIconHeight) / 2,
                            DocumentIconWidth, DocumentIconHeight);

                        g.DrawIcon((content as Form).Icon, rectIcon);
                    }





                    if (DockPane.IsActiveDocumentPane)
                    {
                        using (Font boldFont = new Font(this.Font, FontStyle.Bold))
                        {
                            g.DrawString(text, boldFont, ActiveTextBrush, rectText, DocumentTextStringFormat);
                        }
                    }
                    else
                        g.DrawString(text, Font, InactiveTextBrush, rectText, DocumentTextStringFormat);

                    g.DrawLine(this.OutlineOuterPen, 0, rect.Bottom - 1, rect.Left, rect.Bottom - 1);
                }
                else
                {
                    LinearGradientBrush lnbr = new LinearGradientBrush(rect, this.ActiveBackColor1,
                             this.ActiveBackColor2, LinearGradientMode.Vertical);

                    g.FillPath(lnbr, gp);

                    g.DrawPath(OutlineOuterPen, gp);

                    //if (Tabs.IndexOf(DockPane.ActiveContent) != Tabs.IndexOf(content) + 1)
                    //    g.DrawLine(TabSeperatorPen,
                    //        rect.X + rect.Width - 1, rect.Y,
                    //        rect.X + rect.Width - 1, rect.Y + rect.Height - 1 - DocumentTabGapTop);

                    if (DockPane.DockPanel.ShowDocumentIcon)
                    {
                        Icon icon = (content as Form).Icon;
                        Rectangle rectIcon = new Rectangle(
                            rect.X + DocumentIconGapLeft,
                            rect.Y + (rect.Height - DocumentIconHeight) / 2,
                            DocumentIconWidth, DocumentIconHeight);

                        g.DrawIcon((content as Form).Icon, rectIcon);
                    }

                    g.DrawString(text, Font, InactiveTextBrush, rectText, DocumentTextStringFormat);

                    g.DrawLine(this.OutlineOuterPen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                }

                g.SmoothingMode = mode;
            }
            catch (Exception ee) { }
        }

        private Rectangle TabsRectangle
        {
            get
            {
                if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                    return ClientRectangle;

                Rectangle rectWindow = ClientRectangle;
                int x = rectWindow.X;
                int y = rectWindow.Y;
                int width = rectWindow.Width;
                int height = rectWindow.Height;

                x += DocumentTabGapLeft;
                width -= DocumentTabGapLeft +
                        DocumentTabGapRight +
                        DocumentButtonGapRight +
                        _buttonClose.Width +
                        _buttonScrollRight.Width +
                        _buttonScrollLeft.Width +
                        2 * DocumentButtonGapBetween;

                return new Rectangle(x, y, width, height);
            }
        }

        private void ScrollLeft_Click(object sender, EventArgs e)
        {
            Rectangle rectTabStrip = TabsRectangle;

            int index;
            for (index = 0; index < Tabs.Count; index++)
                if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
                    break;

            Rectangle rectTab = GetTabRectangle(index);
            if (rectTab.Left < rectTabStrip.Left)
                OffsetX += rectTabStrip.Left - rectTab.Left;
            else if (index == 0)
                OffsetX = 0;
            else
                OffsetX += rectTabStrip.Left - GetTabRectangle(index - 1).Left;

            OnRefreshChanges();
        }

        private void ScrollRight_Click(object sender, EventArgs e)
        {
            Rectangle rectTabStrip = TabsRectangle;

            int index;
            int count = Tabs.Count;
            for (index = 0; index < count; index++)
                if (GetTabRectangle(index).IntersectsWith(rectTabStrip))
                    break;

            if (index + 1 < count)
            {
                OffsetX -= GetTabRectangle(index + 1).Left - rectTabStrip.Left;
                CalculateTabs();
            }

            Rectangle rectLastTab = GetTabRectangle(count - 1);
            if (rectLastTab.Right < rectTabStrip.Right)
                OffsetX += rectTabStrip.Right - rectLastTab.Right;

            OnRefreshChanges();
        }

        private void SetInertButtons()
        {
            // Set the visibility of the inert buttons
            _buttonScrollLeft.Visible = _buttonScrollRight.Visible = _buttonClose.Visible = (DockPane.DockState == DockState.Document);

            _buttonClose.ForeColor = _buttonScrollRight.ForeColor = _buttonScrollLeft.ForeColor = SystemColors.ControlDarkDark;
            _buttonClose.BorderColor = _buttonScrollRight.BorderColor = _buttonScrollLeft.BorderColor = SystemColors.ControlDarkDark;

            // Enable/disable scroll buttons
            int count = Tabs.Count;

            Rectangle rectTabOnly = TabsRectangle;
            Rectangle rectTab = (count == 0) ? Rectangle.Empty : GetTabRectangle(count - 1);
            _buttonScrollLeft.Enabled = (OffsetX < 0);
            _buttonScrollRight.Enabled = rectTab.Right > rectTabOnly.Right;

            // show/hide close button
            if (Appearance == DockPane.AppearanceStyle.ToolWindow)
                _buttonClose.Visible = false;
            else
            {
                bool showCloseButton = DockPane.ActiveContent == null ? true : DockPane.ActiveContent.DockHandler.CloseButton;
                if (_buttonClose.Visible != showCloseButton)
                {
                    _buttonClose.Visible = showCloseButton;
                    PerformLayout();
                }
            }
        }

        /// <exclude/>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            Rectangle rectTabStrip = ClientRectangle;

            // Set position and size of the buttons
            int buttonWidth = ImageCloseEnabled.Width;
            int buttonHeight = ImageCloseEnabled.Height;
            int height = rectTabStrip.Height - DocumentButtonGapTop - DocumentButtonGapBottom;
            if (buttonHeight < height)
            {
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);
            _buttonClose.Size = _buttonScrollLeft.Size = _buttonScrollRight.Size = buttonSize;
            int x = rectTabStrip.X + rectTabStrip.Width - DocumentTabGapLeft
                - DocumentButtonGapRight - buttonWidth;
            int y = rectTabStrip.Y + DocumentButtonGapTop;
            _buttonClose.Location = new Point(x, y);
            Point point = _buttonClose.Location;
            bool showCloseButton = DockPane.ActiveContent == null ? true : DockPane.ActiveContent.DockHandler.CloseButton;
            if (showCloseButton)
                point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
            _buttonScrollRight.Location = point;
            point.Offset(-(DocumentButtonGapBetween + buttonWidth), 0);
            _buttonScrollLeft.Location = point;

            OnRefreshChanges();

            base.OnLayout(levent);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            DockPane.CloseActiveContent();
        }

        /// <exclude/>
        protected internal override int GetHitTest(Point ptMouse)
        {
            Rectangle rectTabStrip = TabsRectangle;

            for (int i = 0; i < Tabs.Count; i++)
            {
                Rectangle rectTab = GetTabRectangle(i);
                rectTab.Intersect(rectTabStrip);
                if (rectTab.Contains(ptMouse))
                    return i;
            }
            return -1;
        }

        /// <exclude/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int index = GetHitTest(PointToClient(Control.MousePosition));
            string toolTip = string.Empty;

            base.OnMouseMove(e);

            if (index != -1)
            {
                Rectangle rectTab = GetTabRectangle(index);
                if (Tabs[index].Content.DockHandler.ToolTipText != null)
                    toolTip = Tabs[index].Content.DockHandler.ToolTipText;
                else if (rectTab.Width < GetTabOriginalWidth(index))
                    toolTip = Tabs[index].Content.DockHandler.TabText;
            }

            if (_toolTip.GetToolTip(this) != toolTip)
            {
                _toolTip.Active = false;
                _toolTip.SetToolTip(this, toolTip);
                _toolTip.Active = true;
            }
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnBeginDrawTabStrip()"]/*'/>
        protected virtual void OnBeginDrawTabStrip()
        {
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnEndDrawTabStrip()"]/*'/>
        protected virtual void OnEndDrawTabStrip()
        {
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnBeginDrawTab(DockPaneTab)"]/*'/>
        protected virtual void OnBeginDrawTab(DockPaneTab tab)
        {
        }

        /// <include file='CodeDoc/DockPaneStripVS2003.xml' path='//CodeDoc/Class[@name="DockPaneStripVS2003"]/Method[@name="OnEndDrawTab(DockPaneTab)"]/*'/>
        protected virtual void OnEndDrawTab(DockPaneTab tab)
        {
        }
    }
}
