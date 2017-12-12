using LibGit2Sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSParsers;
using VSProvider;

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Text;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Forms.VisualStyles;
using Input = System.Windows.Input;
using System.Drawing.Design;
namespace WinExplorer.UI
{
    public class TreeViewEx : TreeView
    {
        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }

        private const int WM_PARENT_NOTIFY = 0x210;

        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        
        public TreeViewEx() : base()
        {
            Init();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

           
            //this.LostFocus += TreeViewEx_LostFocus;

            //this.Enter += TreeViewEx_Enter;

            
        }

        //async private void TreeViewEx_Enter(object sender, EventArgs e)
        //{
        //    //RefreshNeeded = true;
        //    //await Task.Run(() =>
        //    //{
        //    //    Task.Delay(3000);
        //    //    RefreshNeeded = true;
        //    //});
        //}

        //async private void TreeViewEx_LostFocus(object sender, EventArgs e)
        //{
        //    //RefreshNeeded = true;
        //    //await Task.Run(() =>
        //    //{
        //    //    Task.Delay(3000);

        //    //    RefreshNeeded = false;
        //    //});
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                Message m = new Message();
                m.HWnd = Handle;
                m.Msg = WM_PRINTCLIENT;
                m.WParam = e.Graphics.GetHdc();
                m.LParam = (IntPtr)PRF_CLIENT;
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }

        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;
        private const int SB_THUMBTRACK = 5;
        private const int SB_ENDSCROLL = 8;
        private const int WM_PRINTCLIENT = 0x318;
        private const int PRF_CLIENT = 0x00000004;
        private const int WM_ERASEBKGND = 0x000000014;

        private const int skipMsgCount = 2;
        

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public int code;
        }

        
        public string searchPattern { get; set; }

        public bool searchPatternEnabled = false;

        public Color searchBackColor = Color.Cyan;

        protected override void WndProc(ref Message m)
        {

            ShowScrollBar(Handle, 3, 0);

            //if (m.Msg == 0x204E)
            //{
            //    WM_REFLECT_NOTIFY
            //   NMHDR nmhdr = (NMHDR)m.GetLParam(typeof(NMHDR));
            //    if (nmhdr.code == -12)
            //    { // NM_CUSTOMDRAW
            //        if (this.Focused || RefreshNeeded/* || ExplorerForms.ef.pgp.mouseDown*/)
            //        {
            //            base.WndProc(ref m);

            //        }
            //        else m.Result = (IntPtr)0x0020;
            //    }
            //    else
            //    {
            //        if (this.Focused || RefreshNeeded)
            //        {
            //            base.WndProc(ref m);
            //        }
            //    }
            //    return;
            //}
            //if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
            //{
            //    var nfy = m.WParam.ToInt32() & 0xFFFF;
            //    if (nfy == SB_THUMBTRACK)
            //    {
            //        currentMsgCount++;

            //        if (currentMsgCount % skipMsgCount == 0)
            //            base.WndProc(ref m);
            //        else if (m.LParam == null)
            //            base.WndProc(ref m);
            //        return;
            //    }
            //    if (nfy == SB_ENDSCROLL)
            //        currentMsgCount = 0;

            //    base.WndProc(ref m);
            //}
            //else
                base.WndProc(ref m);
        }

        private ImageList g = new ImageList();

        private void Init()
        {
            g.Images.Add("forms", new Bitmap(ve_resource.WindowsForm_256x, new Size(25, 25)));
            this.ImageList = g;

            arrowpen = new Pen(Color.Black, 1);
            arrowfill = Brushes.Black;
        }

        public bool IsSourceControl = false;
        private VSSolution vs { get; set; }

        public void ScanForMembers(TreeNode node)
        {
            VSProject vp = ProjectItemInfo.VSProjectIfAny(node);
            string file = ProjectItemInfo.FileNameIfAny(node);
            if (vp != null)
                if (!string.IsNullOrEmpty(file))
                {
                    if (vs.HasMembers(vp, file, ""))
                    {
                        TreeNode nodes = new TreeViewBase.DummyNode();
                        node.Nodes.Add(nodes);
                    }
                }
            foreach (TreeNode ns in node.Nodes)
                ScanForMembers(ns);
        }

        public void ScanForMembers()
        {
            VSSolution vs = Tag as VSSolution;
            if (vs == null)
                return;
            this.BeginInvoke(new Action(() =>
            {
                this.vs = vs;
                foreach (TreeNode node in Nodes)
                {
                    ScanForMembers(node);
                }
                this.Refresh();
            }));
        }

        /// <summary>
        /// Searches given node for Include string
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public TreeNode ScanForNode(TreeNode nodes, string file)
        {
            var files = ProjectItemInfo.FileNameIfAny(nodes);
            if (file == files)
                return nodes;
            foreach (TreeNode node in nodes.Nodes)
            {
                var ns = ScanForNode(node, file);
                if (ns != null)
                    return ns;
            }
            return null;
        }

        /// <summary>
        ///  Searches for node with given Include value
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public TreeNode ScanForNode(string file)
        {
            VSSolution vs = Tag as VSSolution;
            if (vs == null)
                return null;
            this.vs = vs;
            foreach (TreeNode node in Nodes)
            {
                var ns = ScanForNode(node, file);
                if (ns != null)
                    return ns;
            }
            return null;
        }

        public void UpdateNode(string file)
        {
            TreeNode node = ScanForNode(file);
            if (node == null)
                return;
            UpdateSourceStatus(node);
            Refresh();
        }

        public void ScanForSourceStatus(TreeNode node)
        {
            //VSProject vp = ProjectItemInfo.VSProjectIfAny(node);
            string file = ProjectItemInfo.FileNameIfAny(node);
            if (!Directory.Exists(file))
            {
                ProjectItemInfo p = ProjectItemInfo.ToProjectItemInfo(node);
                if (p != null)
                    if (!string.IsNullOrEmpty(file))
                    {
                        if (File.Exists(file))
                        {
                            var changes = repo.Index.RetrieveStatus(file);

                            p.status = changes;
                        }
                    }
            }
            foreach (TreeNode ns in node.Nodes)
                ScanForSourceStatus(ns);
        }

        public void UpdateSourceStatus(TreeNode node)
        {
            ProjectItemInfo p = ProjectItemInfo.ToProjectItemInfo(node);
            if (p != null)
                p.status = FileStatus.Modified;
        }

        private Repository repo { get; set; }

        public void ScanForSourceStatus()
        {
            VSSolution vs = Tag as VSSolution;
            if (vs == null)
                return;
            try
            {
                repo = new Repository(vs.SolutionPath);
            }
            catch (Exception ex)
            {
                return;
            }
            this.BeginInvoke(new Action(() =>
            {
                this.vs = vs;
                foreach (TreeNode node in Nodes)
                {
                    ScanForSourceStatus(node);
                }
                this.Refresh();
            }));
        }

        private bool IsBitSet(Int32 b, byte nPos)
        {
            return new BitArray(new[] { b })[(int)Math.Log(nPos, 2)];
        }

        private int ColumnWidth = 300;

        private Pen arrowpen { get; set; }

        private Brush arrowfill { get; set; }

        private Brush selectedNotFocused = new SolidBrush(Color.LightGray);

        public virtual void OnDrawTreeNode(object sender, DrawTreeNodeEventArgs e)
        {
            string text = e.Node.Text;
            Rectangle itemRect = e.Bounds;
            if (e.Bounds.Height < 1 || e.Bounds.Width < 1)
                return;
            int cIndentBy = 19;
            int cMargin = 6;
            int cTwoMargins = cMargin * 2;
            int indent = (e.Node.Level * cIndentBy) + cMargin;
            int iconLeft = indent;
            int textLeft = iconLeft + 16;
            Color leftColour = e.Node.BackColor;
            Color textColour = e.Node.ForeColor;
            if (IsBitSet((int)e.State, (int)TreeNodeStates.Grayed))
                textColour = Color.FromArgb(255, 128, 128, 128);
            Brush backBrush = new SolidBrush(leftColour);
            if (e.State != TreeNodeStates.Focused && e.State != TreeNodeStates.Selected)
                e.Graphics.FillRectangle(backBrush, itemRect);
            Color separatorColor = Color.Gray;
            Pen separatorPen = new Pen(separatorColor);
            Color cs = Color.FromArgb(255, 51, 153, 255);
            ProjectItemInfo projectItemInfo = ProjectItemInfo.ToProjectItemInfo(e.Node);
            if (!HideSelection)
            {
                if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused) || IsBitSet((int)e.State, (int)TreeNodeStates.Selected/*Focused*/))
                {
                    Rectangle selRect = new Rectangle(textLeft, itemRect.Top, itemRect.Right - textLeft, itemRect.Height);
                    //    VisualStyleRenderer renderer = new VisualStyleRenderer((ContainsFocus) ? VisualStyleElement.Button.PushButton.Hot
                    //                                                                           : VisualStyleElement.Button.PushButton.Normal);
                    //         renderer.DrawBackground(e.Graphics, e.Bounds);
                    Brush bodge = new SolidBrush(Color.FromArgb((IsBitSet((int)e.State, (int)TreeNodeStates.Hot)) ? 255 : 128, 255, 255, 255));
                    if (!IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                        bodge = selectedNotFocused;
                    else
                        bodge = new SolidBrush(cs);
                    e.Graphics.FillRectangle(bodge, e.Bounds);
                }
            }
            Pen dotPen = new Pen(Color.FromArgb(128, 128, 128));
            dotPen.DashStyle = DashStyle.Dot;
            int midY = (itemRect.Top + itemRect.Bottom) / 2;
            if (ShowLines)
            {
                int x = cMargin * 2;
                if (e.Node.Level == 0 && e.Node.PrevNode == null)
                {
                    e.Graphics.DrawLine(dotPen, x, midY, x, itemRect.Bottom);
                }
                else
                {
                    TreeNode testNode = e.Node;
                    for (int iLine = e.Node.Level; iLine >= 0; iLine--)
                    {
                        if (testNode.NextNode != null)
                        {
                            x = (iLine * cIndentBy) + (cMargin * 2);
                            e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, itemRect.Bottom);
                        }
                        testNode = testNode.Parent;
                    }
                    x = (e.Node.Level * cIndentBy) + cTwoMargins;
                    e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, midY);
                }
                e.Graphics.DrawLine(dotPen, iconLeft + cMargin, midY, iconLeft + cMargin + 10, midY);
            }
            if (ShowPlusMinus && e.Node.Nodes.Count > 0)
            {
                Rectangle expandRect = new Rectangle(iconLeft - 1, midY - 7, 16, 16);
                Point pe = expandRect.Location;

                Point a = new Point(new Size(pe));
                a.Offset(e.Bounds.X + 3, 3);
                Point b = new Point(new Size(pe));
                b.Offset(e.Bounds.X + 3, 11);
                Point c = new Point(new Size(pe));
                c.Offset(e.Bounds.X + 7, 7);
                Point d = new Point(new Size(pe));
                d.Offset(e.Bounds.X + 1, 9);
                Point f = new Point(new Size(pe));
                f.Offset(e.Bounds.X + 7, 3);
                Point g = new Point(new Size(pe));
                g.Offset(e.Bounds.X + 7, 9);
                if (e.Node.IsExpanded)
                {
                    List<Point> s = new List<Point>();
                    s.Add(d);
                    s.Add(f);
                    s.Add(g);
                    s.Add(d);
                    e.Graphics.FillPolygon(arrowfill, s.ToArray());
                }
                else
                {
                    List<Point> s = new List<Point>();
                    s.Add(a);
                    s.Add(b);
                    s.Add(c);
                    s.Add(a);
                    e.Graphics.DrawLines(arrowpen, s.ToArray());
                }
            }
            Point textStartPos = new Point(itemRect.Left + textLeft, itemRect.Top);
            Point textPos = new Point(textStartPos.X, textStartPos.Y);

            Font textFont = e.Node.NodeFont;
            if (textFont == null)
                textFont = Font;

            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Near;
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.FormatFlags = StringFormatFlags.NoWrap;

            Point p = new Point(e.Bounds.Location.X, e.Bounds.Location.Y);
            if (IsSourceControl)
            {
                p.Offset(iconLeft + 5, 0);
                if (projectItemInfo != null)
                {
                    //if (projectItemInfo.status != null)
                    {
                        if (projectItemInfo.status == FileStatus.Added)
                            e.Graphics.DrawImage(this.ImageList.Images["Add_thin"], p);
                        else if (projectItemInfo.status == FileStatus.Modified)
                            e.Graphics.DrawImage(this.ImageList.Images["Status_Offline"], p);
                        else if (projectItemInfo.status == FileStatus.Unaltered)
                            e.Graphics.DrawImage(this.ImageList.Images["LockCyan_16x"], p);
                    }
                }
                //e.Graphics.DrawImage(this.ImageList.Images["LockCyan_16x"], p);
                p.Offset(13, 0);
            }
            else
                p.Offset(iconLeft + 20, 0);
            if (e.Node.ImageKey == "" || !this.ImageList.Images.ContainsKey(e.Node.ImageKey))
                e.Graphics.DrawImage(this.g.Images[0], p);
            else
                e.Graphics.DrawImage(this.ImageList.Images[e.Node.ImageKey], p);
            p.X += 18;

            //string[] columnTextList = text.Split('|');

            //    for (int iCol = 0; iCol < columnTextList.GetLength(0); iCol++)
            {
                Rectangle textRect = new Rectangle(textPos.X, textPos.Y, itemRect.Right - textPos.X, itemRect.Bottom - textPos.Y);

                if (searchPatternEnabled)
                    if (!string.IsNullOrEmpty(searchPattern))
                    {
                        int i = text.ToLower().IndexOf(searchPattern);
                        if (i >= 0)
                        {
                            e.Graphics.FillRectangle(Brushes.LightCyan, new Rectangle(p.X + i * 5, p.Y, 20, e.Bounds.Height));
                        }
                    }

                if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, Color.White);
                else
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, SystemColors.ControlDarkDark);

                textPos.X += ColumnWidth;
            }

            // Draw Focussing box around the text
            if (e.State == TreeNodeStates.Focused || e.State == TreeNodeStates.Selected)
            {
                SizeF size = e.Graphics.MeasureString(text, textFont);
                size.Width = (ClientRectangle.Width - 2) - textStartPos.X;
                size.Height += 1;
                Rectangle rect = new Rectangle(textStartPos, size.ToSize());
                e.Graphics.DrawRectangle(dotPen, rect);
            }
        }

        public void ExpandAllWithoutDummy()
        {
            foreach (TreeNode node in Nodes)
            {
                var ns = node.Nodes.OfType<TreeViewBase.DummyNode>().ToList();
                if (ns.Count <= 0)
                    ExpandAllWithoutDummy(node);
            }
        }

        public void ExpandAllWithoutDummy(TreeNode nodes)
        {
            nodes.Expand();
            foreach (TreeNode node in nodes.Nodes)
            {
                var ns = node.Nodes.OfType<TreeViewBase.DummyNode>().ToList();
                if (ns.Count <= 0)
                    ExpandAllWithoutDummy(node);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 515)
            { /* WM_LBUTTONDBLCLK */
            }
            else
                base.DefWndProc(ref m);
        }
    }


//
// Based and modified upon Author: Copyright: Home Page: http://www.proxoft.com
//


        /// <summary>
        /// 
        /// </summary>
        public class ScrollBarEx : UserControl
        {
            #region Private fields

            private Input.MouseButtonState MouseUpDownStatus = Input.MouseButtonState.Released;
            EnhancedScrollBarMouseLocation MouseScrollBarArea = EnhancedScrollBarMouseLocation.OutsideScrollBar;
            private Point MouseActivePointPoint;
            private int MouseRelativeYFromThumbTop = 0;

            decimal HotValue = -1;

            decimal PreviousValue = -1;

            private decimal m_Minimum = 0;
            private decimal m_Maximum = 100;
            private decimal m_Value = 0;

            private bool m_disposed;

            private ToolTip toolTip1;
            private Timer timerMouseDownRepeater;

            Dictionary<Color, Brush> m_BrushesCache = new Dictionary<Color, Brush>();
            Dictionary<Color, Pen> m_PensCache = new Dictionary<Color, Pen>();

            private Orientation m_Orientation = Orientation.Vertical;

            //Every time mouse is pressed down this is populated to be used by the repeat timer
            MouseEventArgs m_MouseDownArgs = null;

            #endregion

            #region Public Events

            /// <summary>
            /// Fires every time mouse is clicked over track area.
            /// </summary>
            [Description("Fires every time mouse is clicked over track area.")]
            public new event EventHandler<EnhancedMouseEventArgs> MouseClick = null;

            /// <summary>
            /// Fires every time mouse moves over track area.
            /// </summary>
            [Description("Fires every time mouse moves over track area.")]
            public new event EventHandler<EnhancedMouseEventArgs> MouseMove = null;

            /// <summary>
            /// Occurs each time scrollbar orientation has changed.
            /// </summary>
            [Description("Occurs each time scrollbar orientation has changed.")]
            public event EventHandler OrientationChanged = null;

            /// <summary>
            /// Occurs every time scrollbar orientation is about to change.
            /// </summary>
            [Description("Occurs every time scrollbar orientation is about to change.")]
            public event EventHandler<CancelEventArgs> OrientationChanging = null;

            /// <summary>
            /// 
            /// </summary>
            public new event EventHandler<EnhancedScrollEventArgs> Scroll = null;

            /// <summary>
            /// Fired every time <c>Value</c> of the ScrollBar changes.
            /// </summary>
            [Description("Occurs every time scrollbar value changes.")]
            public event EventHandler ValueChanged = null;

            #endregion

            #region Constructor and related

            /// <summary>
            /// Constructor. Initialize properties.
            /// </summary>
            public ScrollBarEx()
            {

                InitializeComponent();
                SetStyle(ControlStyles.ResizeRedraw, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);

                ShowTooltipOnMouseMove = false;
                InitialDelay = 300;
                RepeatRate = 62;
                LargeChange = 10;
                SmallChange = 1;

                timerMouseDownRepeater.Tick += new EventHandler(timerMouseDownRepeater_Tick);

                Dock = DockStyle.None;
                this.Width = SystemInformation.VerticalScrollBarWidth;

                Orientation = Orientation.Vertical;

                timer.Interval = 200;

                timer.Tick += Timer_Tick;

                timer.Start();
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                if (eas == null)
                    return;

                OnScroll(eas.NewValue, eas.OldValue, ScrollEventType.ThumbTrack);

                eas = null;
            }

            Timer timer = new Timer();
            /// <summary>
            /// Generates repeat events when mouse is pressed and hold.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void timerMouseDownRepeater_Tick(object sender, EventArgs e)
            {
                base.OnMouseDown(m_MouseDownArgs);

                DoMouseDown(m_MouseDownArgs);

                if (timerMouseDownRepeater.Enabled)
                    timerMouseDownRepeater.Interval = RepeatRate;
                else
                    timerMouseDownRepeater.Interval = InitialDelay;

                //Do this only if not dragging thumb
                if (MouseScrollBarArea != EnhancedScrollBarMouseLocation.Thumb)
                    timerMouseDownRepeater.Enabled = true;
                else
                    timerMouseDownRepeater.Enabled = false;
            }

            private IContainer components;
            private void InitializeComponent()
            {
                this.components = new System.ComponentModel.Container();
                this.timerMouseDownRepeater = new System.Windows.Forms.Timer(this.components);
                this.SuspendLayout();
                this.ResumeLayout(false);

            }

            /// <summary>
            /// Dispose overridden method. When called from the host <c>disposing</c> parameter is <b>true</b>.
            /// When called from the finalize parameter is <b>false</b>.
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (!m_disposed)
                {
                    if (disposing)
                    {
                        if (components != null)
                        {
                            components.Dispose();
                        }
                    }
                    foreach (Brush brush in m_BrushesCache.Values)
                        brush.Dispose();

                    foreach (Pen pen in m_PensCache.Values)
                        pen.Dispose();

                    m_disposed = true;

                    base.Dispose();

                }
            }

 
            #endregion

            #region Public Properties

            //We don't need BackColor to be exposed as property, so let's try to hide it form property list
            /// <summary>
            /// BackColor doesn't have any meaning for the ScrollBar.
            /// </summary> 
            [Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never)]
            public new Color BackColor { get; set; }


            /// <summary>
            /// Delay in milliseconds to start autorepeat behavior when mouse is pressed down and hold.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(400), Category("Enhanced")]
            [Description("Delay in milliseconds to start autorepeat behavior when mouse is pressed down and hold.")]
            public int InitialDelay { set; get; }

            /// <summary>
            /// Large scrollbar change.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(10), Category("Enhanced")]
            [Description("Large scrollbar change.")]
            public decimal LargeChange { set; get; }

            /// <summary>
            /// "Maximum scrollbar value.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(100), Category("Enhanced")]
            [Description("Maximum scrollbar value.")]
            public decimal Maximum
            {
                get { return m_Maximum; }
                set
                {
                    if (value < Minimum)
                        throw new ArgumentException("Minimum has to be less or equal Maximum", "Minimum");

                    //The following line will throw exception is range is more than decimal.MaxValue
                    decimal rangeTestValue = value - Minimum;

                    m_Maximum = value;
                    Invalidate();
                }
            }

            /// <summary>
            /// Minimum scrollbar value.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(0), Category("Enhanced")]
            [Description("Minimum scrollbar value.")]
            public decimal Minimum
            {
                get { return m_Minimum; }
                set
                {
                    if (Maximum < value)
                        throw new ArgumentException("Minimum has to be less or equal Maximum", "Minimum");

                    //The following line will throw exception is range is more than decimal.MaxValue
                    decimal rangeTestValue = Maximum - value;

                    m_Minimum = value;
                    Invalidate();
                }
            }

            /// <summary>
            /// ScrollBar orientation.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(Orientation.Vertical), Category("Enhanced")]
            [Description("ScrollBar orientation.")]
            public Orientation Orientation
            {
                set
                {
                    if (OrientationChanging != null)
                    {
                        CancelEventArgs ea = new CancelEventArgs(false);
                        OrientationChanging(this, ea);
                        if (ea.Cancel)
                            return;
                    }
                    if (m_Orientation != value)
                    {
                        m_Orientation = value;

                        //Switch width with height
                        int tmpWidth = this.Width;
                        this.Width = this.Height;
                        this.Height = tmpWidth;


                        if (Orientation == Orientation.Vertical)
                        {
                            base.MinimumSize = new Size(0, 2 * SystemInformation.VerticalScrollBarArrowHeight + ThumbLength);
                            switch (this.Dock)
                            {
                                case DockStyle.Bottom:
                                    Dock = DockStyle.Right; break;
                                case DockStyle.Top:
                                    Dock = DockStyle.Left; break;
                            }
                        }
                        else
                        {
                            base.MinimumSize = new Size(2 * SystemInformation.HorizontalScrollBarArrowWidth + ThumbLength, 0);
                            switch (this.Dock)
                            {
                                case DockStyle.Right:
                                    Dock = DockStyle.Bottom; break;
                                case DockStyle.Left:
                                    Dock = DockStyle.Top; break;
                            }

                        }
                        if (OrientationChanged != null)
                        {
                            OrientationChanged(this, EventArgs.Empty);
                        }
                    }

                }
                get
                {
                    return m_Orientation;
                }
            }

            /// <summary>
            /// "When <b>true</b>, clicking on bookmark image, changes scrollbar value to the bookmark value 
            /// (moves thumb position to bookmark value).
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(true), Category("Enhanced")]
            [Description("When 'true', clicking on bookmark image, changes scrollbar value to the bookmark value.")]
            public bool QuickBookmarkNavigation
            { set; get; }

            /// <summary>
            /// Delay in milliseconds between autorepeat MouseDown events when mouse is pressed down and hold.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(62), Category("Enhanced")]
            [Description("Delay in milliseconds between autorepeat MouseDown events when mouse is pressed down and hold.")]
            public int RepeatRate { set; get; }

            /// <summary>
            /// When set to <b>true</b>, allows to show ToolTip when mouse moves over scrollbar area.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Enhanced")]
            [Description("When set to 'true', allows to show ToolTip when mouse moves over scrollbar area.")]
            public bool ShowTooltipOnMouseMove { set; get; }

            /// <summary>
            /// Small bookmark change.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(1), Category("Enhanced")]
            [Description("Small change.")]
            public decimal SmallChange { set; get; }

            /// <summary>
            /// Bookmark value. Determines current thumb position.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(0), Category("Enhanced")]
            [Description("Value")]
            public decimal Value
            {
                get { return m_Value; }
                set
                {
                    if (value < Minimum)
                        m_Value = Minimum;
                    else if (value > Maximum)
                        m_Value = Maximum;
                    else
                        m_Value = value;

                    OnValueChanged();
                    Invalidate();
                }
            }

            #endregion

            #region Overridden events

            /// <summary>
            /// What should happen here:
            /// 1. Save information that mouse is down
            /// 2. Call timer event handler (it will repeat periodically MouseDown events as long as mouse is down)
            /// </summary>
            /// <param name="e">Standard <c>MouseEventArgs</c>.</param>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                //Save arguments passed to mouse down
                m_MouseDownArgs = e;
                MouseUpDownStatus = Input.MouseButtonState.Pressed;

                //Make sure timer is disabled
                timerMouseDownRepeater.Enabled = false;

                //Call timer event; it will call DoMouseDown every RepeatRate interval
                timerMouseDownRepeater_Tick(null, EventArgs.Empty);
            }

            /// <summary>
            /// This private methods called from repeater timer event handler
            /// </summary>
            /// <param name="e"></param>
            private void DoMouseDown(MouseEventArgs e)
            {
                //1. Save info about fact that mouse is presses
                //2. Save scrollbar area where mouse was pressed 
                if (Orientation == Orientation.Vertical)
                    MouseScrollBarArea = MouseLocation(e.Y, out MouseRelativeYFromThumbTop);
                else
                    MouseScrollBarArea = MouseLocation(e.X, out MouseRelativeYFromThumbTop);

                //3. Save exact location of mouse press 
                MouseActivePointPoint = e.Location;

                //4. Calculate ScrollEvent arguments and adjust Value if needed
                decimal NewValue = Value;
                ScrollEventType et;

                {
                    switch (MouseScrollBarArea)
                    {
                        case EnhancedScrollBarMouseLocation.BottomOrRightArrow:
                        case EnhancedScrollBarMouseLocation.BottomOrRightTrack:
                            if (MouseScrollBarArea == EnhancedScrollBarMouseLocation.BottomOrRightArrow)
                            {
                                NewValue += SmallChange;
                                et = ScrollEventType.SmallIncrement;
                            }
                            else    // EnhancedScrollBarMouseLocation.bottomTrack
                            {
                                NewValue += LargeChange;
                                et = ScrollEventType.LargeIncrement;
                            }
                            if (NewValue >= Maximum)
                            {
                                NewValue = Maximum;
                                et = ScrollEventType.Last;
                            }
                            OnScroll(NewValue, Value, et);
                            break;
                        case EnhancedScrollBarMouseLocation.Thumb:
                            OnScroll(Value, Value, ScrollEventType.ThumbTrack);
                            break;
                        case EnhancedScrollBarMouseLocation.TopOrLeftArrow:
                        case EnhancedScrollBarMouseLocation.TopOrLeftTrack:
                            if (MouseScrollBarArea == EnhancedScrollBarMouseLocation.TopOrLeftArrow)
                            {
                                NewValue -= SmallChange;
                                et = ScrollEventType.SmallDecrement;
                            }
                            else
                            {
                                NewValue -= LargeChange;
                                et = ScrollEventType.LargeIncrement;
                            }
                            if (NewValue <= Minimum)
                            {
                                NewValue = Minimum;
                                et = ScrollEventType.First;
                            }
                            OnScroll(NewValue, Value, et);
                            break;
                    }
                    Value = NewValue;
                }

                //3. Repaint
                Invalidate();

            }

            /// <summary>
            /// Captures mouse wheel actions and translates them to small decrement events.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseWheel(MouseEventArgs e)
            {
                int ScrollStep = e.Delta / System.Windows.Input.Mouse.MouseWheelDeltaForOneLine;

                //Calculate new value
                decimal NewValue = Value - SmallChange * ScrollStep;

                if (NewValue < Value)
                    OnScroll(NewValue, Value, ScrollEventType.SmallDecrement);
                else
                    OnScroll(NewValue, Value, ScrollEventType.SmallIncrement);

                Value = NewValue;
                OnScroll(Value, Value, ScrollEventType.EndScroll);

                base.OnMouseWheel(e);
            }

            /// <summary>
            /// MouseClick override. Calls MouseClick event handled with enhanced arguments. 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);
                //enrich arguments and call back the host
                OnMouseClick(e, Value);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseMove(MouseEventArgs e)
            {

                base.OnMouseMove(e);

                //Mouse is down and is over thumb
                if ((MouseUpDownStatus == Input.MouseButtonState.Pressed) && (MouseScrollBarArea == EnhancedScrollBarMouseLocation.Thumb))
                {
                    //Dragging thumb button

                    //Calculate Value based on new e.Y 
                    decimal NewValue;
                    if (Orientation == Orientation.Vertical)
                        NewValue = ThumbTopPosition2Value(e.Y - MouseRelativeYFromThumbTop);
                    else
                        NewValue = ThumbTopPosition2Value(e.X - MouseRelativeYFromThumbTop);

                    if (NewValue < Minimum) NewValue = Minimum;
                    if (NewValue > Maximum) NewValue = Maximum;

                    //Call onScroll event 
                    //OnScroll(NewValue, Value, ScrollEventType.ThumbTrack);

                    eas = new EnhancedScrollEventArgs(Value, NewValue, ScrollEventType.ThumbTrack, ScrollOrientation.VerticalScroll);

                    //Assign new value
                    Value = NewValue;  //New value will move scrollbar to proper position

                    //Refresh display
                    this.Invalidate();
                }
                else
                {
                    string toolTip = "";
                    //Moving mouse over different areas

                    //1. Save current (before mouse move) mouse location
                    EnhancedScrollBarMouseLocation oldLocation = MouseScrollBarArea;

                    int tmpInt;
                    EnhancedScrollBarMouseLocation newLocation;

                    int TrackPosition;
                    if (Orientation == Orientation.Vertical)
                    {
                        newLocation = MouseLocation(e.Y, out tmpInt);
                        TrackPosition = e.Y - SystemInformation.VerticalScrollBarArrowHeight;
                        switch (newLocation)
                        {
                            case EnhancedScrollBarMouseLocation.TopOrLeftArrow:
                                TrackPosition = 0;
                                break;
                            case EnhancedScrollBarMouseLocation.BottomOrRightArrow:
                                TrackPosition = ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight;
                                break;

                        }
                    }
                    else
                    {
                        newLocation = MouseLocation(e.X, out tmpInt);

                        TrackPosition = e.X - SystemInformation.VerticalScrollBarArrowHeight;
                        switch (newLocation)
                        {
                            case EnhancedScrollBarMouseLocation.TopOrLeftArrow:
                                TrackPosition = 0;
                                break;
                            case EnhancedScrollBarMouseLocation.BottomOrRightArrow:
                                TrackPosition = ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth;
                                break;

                        }
                    }
                    HotValue = (Maximum - Minimum) * ((decimal)TrackPosition / TrackLength) + Minimum;
                    MouseScrollBarArea = newLocation;

                    if ((TrackPosition < 0) || (TrackPosition > TrackLength))
                        toolTip1.Hide(this);
                    else
                    {

                        //Call the host to notify about the MouseMove event		
                        OnMouseMove(e, HotValue);


                        //If moving over different area- refresh display
                        if (oldLocation != MouseScrollBarArea)
                            this.Invalidate();
                    }
                }
            }

            /// <summary>
            /// Forces repaint of ScrollBar when mouse moves outside of ScrollBar area.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseLeave(EventArgs e)
            {
                MouseUpDownStatus = Input.MouseButtonState.Released;
                MouseScrollBarArea = EnhancedScrollBarMouseLocation.OutsideScrollBar;
                timerMouseDownRepeater.Enabled = false;

                base.OnMouseLeave(e);
                this.Invalidate();
            }

            /// <summary>
            /// Fires <c>Scroll</c> events and refreshes ScrollBar display.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                timerMouseDownRepeater.Enabled = false;
                MouseUpDownStatus = Input.MouseButtonState.Released;

                switch (MouseScrollBarArea)
                {
                    case EnhancedScrollBarMouseLocation.BottomOrRightArrow:
                    case EnhancedScrollBarMouseLocation.TopOrLeftArrow:
                    case EnhancedScrollBarMouseLocation.BottomOrRightTrack:
                    case EnhancedScrollBarMouseLocation.TopOrLeftTrack:
                        OnScroll(Value, Value, ScrollEventType.EndScroll);
                        break;
                    case EnhancedScrollBarMouseLocation.Thumb:
                        OnScroll(Value, Value, ScrollEventType.ThumbPosition);
                        OnScroll(Value, Value, ScrollEventType.EndScroll);
                        break;
                }
                Invalidate();
            }

            #endregion

            #region Private helpers/wrappers of public events

            private void OnValueChanged()
            {
                if ((ValueChanged != null) && (this.Value != PreviousValue))
                {
                    PreviousValue = this.Value;
                    ValueChanged(this, new EventArgs());
                }
            }

            private void OnMouseClick(MouseEventArgs mouseArgs, decimal Value)
            {
                if (MouseClick != null)
                {
                    EnhancedMouseEventArgs e = new EnhancedMouseEventArgs(HotValue, mouseArgs, MouseScrollBarArea);
                    MouseClick(this, e);
                }
            }

            private void OnMouseMove(MouseEventArgs ea, decimal HotValue)
            {
                if (MouseMove != null)
                {
                    //Call event handler
                    EnhancedMouseEventArgs e = new EnhancedMouseEventArgs(HotValue, ea, MouseScrollBarArea);
                    MouseMove(this, e);
                }
            }

            EnhancedScrollEventArgs eas = null;

            private void OnScroll(decimal newVal, decimal oldVal, ScrollEventType scrollEventType)
            {
                if (Scroll != null)
                {
                    ScrollOrientation ScrollOrientation;
                    if (Orientation == Orientation.Horizontal)
                        ScrollOrientation = ScrollOrientation.HorizontalScroll;
                    else
                        ScrollOrientation = ScrollOrientation.VerticalScroll;

                    //Make sure NewVal is within valid range
                    newVal = Math.Max(Math.Min(Maximum, newVal), Minimum);
                    EnhancedScrollEventArgs ea = new EnhancedScrollEventArgs(oldVal, newVal, scrollEventType, ScrollOrientation);

                    Scroll(this, ea);
                }
            }

            #endregion

            #region OnPaint override



            private int ArrowLegth()
            {
                return Orientation == Orientation.Vertical ? SystemInformation.VerticalScrollBarArrowHeight : SystemInformation.HorizontalScrollBarArrowWidth;
            }



            /// <summary>
            /// Overridden OnPaint. Draws all EnhancedScrollBar elements.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {

                //Draw top arrow
                Rectangle rec;
                ScrollBarState state = GetScrollBarAreaState(EnhancedScrollBarMouseLocation.TopOrLeftArrow);
                if (Orientation == Orientation.Vertical)
                {
                    rec = new Rectangle(0, 0, ClientSize.Width, ArrowLegth());
                    switch (state)
                    {
                        case ScrollBarState.Disabled:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.UpDisabled); break;
                        case ScrollBarState.Pressed:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.UpPressed); break;
                        case ScrollBarState.Normal:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.UpNormal); break;
                        case ScrollBarState.Hot:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.UpHot); break;
                    }
                }
                else
                {
                    rec = new Rectangle(0, 0, ArrowLegth(), ClientSize.Height);
                    switch (state)
                    {
                        case ScrollBarState.Disabled:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.LeftDisabled); break;
                        case ScrollBarState.Pressed:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.LeftPressed); break;
                        case ScrollBarState.Normal:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.LeftNormal); break;
                        case ScrollBarState.Hot:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.LeftHot); break;
                    }
                }

                //Draw top track
                int ThumbPos = Value2ThumbTopPosition(Value);
                state = GetScrollBarAreaState(EnhancedScrollBarMouseLocation.TopOrLeftTrack);
                if (Orientation == Orientation.Vertical)
                {
                    rec = new Rectangle(0, ArrowLegth(), ClientSize.Width, ThumbPos - ArrowLegth() + ThumbLength);
                    ScrollBarRenderer.DrawUpperVerticalTrack(e.Graphics, rec, state);
                }
                else
                {
                    rec = new Rectangle(ArrowLegth() + 1, 0, ThumbPos - ArrowLegth() + ThumbLength, ClientSize.Height + ThumbLength);
                    ScrollBarRenderer.DrawLeftHorizontalTrack(e.Graphics, rec, state);
                }

                //draw thumb
                int nThumbLength = ThumbLength;
                DrawThumb(e.Graphics, nThumbLength, ThumbPos);

                //Draw bottom track
                state = GetScrollBarAreaState(EnhancedScrollBarMouseLocation.BottomOrRightTrack);
                if (Orientation == Orientation.Vertical)
                {
                    rec = new Rectangle(1, ThumbPos + nThumbLength, ClientSize.Width, TrackLength + ArrowLegth() - (ThumbPos + nThumbLength));
                    ScrollBarRenderer.DrawLowerVerticalTrack(e.Graphics, rec, state);
                }
                else
                {
                    rec = new Rectangle(ThumbPos + nThumbLength, 0, TrackLength + ArrowLegth() - (ThumbPos + nThumbLength), ClientSize.Height);
                    ScrollBarRenderer.DrawRightHorizontalTrack(e.Graphics, rec, state);
                }

                //Draw bottom arrow 
                state = GetScrollBarAreaState(EnhancedScrollBarMouseLocation.BottomOrRightArrow);
                if (Orientation == Orientation.Vertical)
                {
                    rec = new Rectangle(0, ClientSize.Height - ArrowLegth(), ClientSize.Width, ArrowLegth());
                    switch (state)
                    {
                        case ScrollBarState.Disabled:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.DownDisabled); break;
                        case ScrollBarState.Pressed:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.DownPressed); break;
                        case ScrollBarState.Normal:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.DownNormal); break;
                        case ScrollBarState.Hot:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.DownHot); break;
                    }
                }
                else
                {
                    rec = new Rectangle(ClientSize.Width - ArrowLegth(), 0, ArrowLegth(), ClientSize.Height);
                    switch (state)
                    {
                        case ScrollBarState.Disabled:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.RightDisabled); break;
                        case ScrollBarState.Pressed:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.RightPressed); break;
                        case ScrollBarState.Normal:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.RightNormal); break;
                        case ScrollBarState.Hot:
                            ScrollBarRenderer.DrawArrowButton(e.Graphics, rec, ScrollBarArrowButtonState.RightHot); break;
                    }
                }



            }


            private void DrawThumb(Graphics graphics, int thumbLength, int thumbPosition)
            {
                ScrollBarState state = GetScrollBarAreaState(EnhancedScrollBarMouseLocation.Thumb);
                Rectangle rec;
                if (Orientation == Orientation.Vertical)
                {
                    rec = new Rectangle(2, thumbPosition, ClientSize.Width - 4, thumbLength);
                    ScrollBarRenderer.DrawVerticalThumb(graphics, rec, state);
                    //draw thumb grip
                    if (thumbLength >= SystemInformation.VerticalScrollBarThumbHeight)
                        ScrollBarRenderer.DrawVerticalThumbGrip(graphics, rec, ScrollBarState.Normal);
                }
                else
                {
                    rec = new Rectangle(thumbPosition, 2, thumbLength, ClientSize.Height - 4);
                    ScrollBarRenderer.DrawHorizontalThumb(graphics, rec, state);
                    //draw thumb grip
                    if (thumbLength >= SystemInformation.HorizontalScrollBarThumbWidth)
                        ScrollBarRenderer.DrawHorizontalThumbGrip(graphics, rec, ScrollBarState.Normal);
                }
            }

            private ScrollBarState GetScrollBarAreaState(EnhancedScrollBarMouseLocation mouseHotLocation)
            {
                if (this.Enabled)
                {
                    if (MouseScrollBarArea == mouseHotLocation)
                        return MouseUpDownStatus == Input.MouseButtonState.Pressed ? ScrollBarState.Pressed : ScrollBarState.Hot;
                    else
                        return ScrollBarState.Normal;
                }
                else
                    return ScrollBarState.Disabled;

            }

            #endregion

            #region private helper methods

            private void NavigateTo(decimal NewValue)
            {
                OnScroll(Value, Value, ScrollEventType.ThumbTrack);
                OnScroll(Value, NewValue, ScrollEventType.ThumbTrack);
                Value = NewValue;
                OnScroll(Value, Value, ScrollEventType.ThumbPosition);
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private int TrackLength
            {
                get
                {
                    if (Orientation == Orientation.Vertical)
                        return this.ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight;
                    else
                        return this.ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth;
                }
            }
            decimal v_ratio = 0;
            decimal w_ratio = 0;
            private int ThumbLength
            {
                get
                {
                    if (Minimum == Maximum) return TrackLength;

                    int nThumbLength = (int)((TrackLength / (Maximum - Minimum + 1)) * TrackLength);



                    if (Orientation == Orientation.Vertical)
                    {
                    nThumbLength = (int)((TrackLength / (10*(Maximum - Minimum + 1))) * TrackLength);
                    if (nThumbLength < SystemInformation.VerticalScrollBarThumbHeight)
                            nThumbLength = SystemInformation.VerticalScrollBarThumbHeight;
                        if (this.ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight - nThumbLength != 0)
                            v_ratio = (this.ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight - nThumbLength) / Maximum;
                        else v_ratio = 1;

                    return nThumbLength;
                }
                    else
                    {
                        if (nThumbLength < SystemInformation.HorizontalScrollBarThumbWidth)
                            nThumbLength = SystemInformation.HorizontalScrollBarThumbWidth;
                        if (this.ClientSize.Width - 2 * SystemInformation.VerticalScrollBarArrowHeight - nThumbLength != 0)
                        {
                            v_ratio = (this.ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth - nThumbLength) / Maximum;
                           // w_ratio = (this.ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth - nThumbLength - 20) / Maximum;
                        }
                        else v_ratio = 1;
                    }

                    return nThumbLength;
                }
            }
            public int windowPosition { get; set; }

            private int Value2ThumbTopPosition(decimal value)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (Maximum == Minimum) return SystemInformation.VerticalScrollBarArrowHeight;

                    decimal ratio = (decimal)(ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight - ThumbLength) / (Maximum - Minimum);
                    return (int)((value - Minimum) * ratio) + SystemInformation.VerticalScrollBarArrowHeight;
                }
                else
                {
                    if (Maximum == Minimum) return SystemInformation.HorizontalScrollBarArrowWidth;

                    decimal ratio = (decimal)(ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth - ThumbLength) / (Maximum - Minimum);
                    return (int)((value - Minimum) * ratio) + SystemInformation.HorizontalScrollBarArrowWidth;
                }
            }

            private decimal ThumbTopPosition2Value(int y)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight - ThumbLength == 0)
                        return 0;
                    else
                    {
                        decimal ratio = (decimal)((y - SystemInformation.VerticalScrollBarArrowHeight)) / (ClientSize.Height - 2 * SystemInformation.VerticalScrollBarArrowHeight - ThumbLength);
                        ratio = y / v_ratio;
                    ratio = (y - SystemInformation.HorizontalScrollBarArrowWidth) / v_ratio;
                    return ratio;// (Maximum - Minimum) * ratio + Minimum;
                    }
                }
                else
                {
                    if (ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth - ThumbLength == 0)
                        return 0;
                    else
                    {
                        decimal ratio = (decimal)((y - SystemInformation.HorizontalScrollBarArrowWidth)) / (ClientSize.Width - 2 * SystemInformation.HorizontalScrollBarArrowWidth - ThumbLength);
                        ratio = (y - SystemInformation.HorizontalScrollBarArrowWidth) / v_ratio;
                        //windowPosition = (int)((y - SystemInformation.HorizontalScrollBarArrowWidth - 20) / w_ratio);

                        return ratio;// (Maximum - Minimum) * ratio + Minimum;
                    }
                }
            }

            private EnhancedScrollBarMouseLocation MouseLocation(int absolutePosition, out int relativeY)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (absolutePosition <= SystemInformation.VerticalScrollBarArrowHeight)
                    {
                        relativeY = absolutePosition;
                        return EnhancedScrollBarMouseLocation.TopOrLeftArrow;
                    }
                    else if (absolutePosition > ClientSize.Height - SystemInformation.VerticalScrollBarArrowHeight)
                    {
                        relativeY = absolutePosition - ClientSize.Height + SystemInformation.VerticalScrollBarArrowHeight;
                        return EnhancedScrollBarMouseLocation.BottomOrRightArrow;
                    }
                    else
                    {
                        int thumbTop = Value2ThumbTopPosition(Value);
                        if (absolutePosition < thumbTop)
                        {
                            relativeY = absolutePosition - SystemInformation.VerticalScrollBarArrowHeight;
                            return EnhancedScrollBarMouseLocation.TopOrLeftTrack;
                        }
                        else if (absolutePosition < thumbTop + ThumbLength)
                        {
                            relativeY = absolutePosition - thumbTop;
                            return EnhancedScrollBarMouseLocation.Thumb;
                        }
                        else
                        {
                            relativeY = absolutePosition - thumbTop - ThumbLength;
                            return EnhancedScrollBarMouseLocation.BottomOrRightTrack;
                        }
                    }
                }
                else
                {
                    if (absolutePosition <= SystemInformation.HorizontalScrollBarArrowWidth)
                    {
                        relativeY = absolutePosition;
                        return EnhancedScrollBarMouseLocation.TopOrLeftArrow;
                    }
                    else if (absolutePosition > ClientSize.Width - SystemInformation.HorizontalScrollBarArrowWidth)
                    {
                        relativeY = absolutePosition - ClientSize.Width + SystemInformation.HorizontalScrollBarArrowWidth;
                        return EnhancedScrollBarMouseLocation.BottomOrRightArrow;
                    }
                    else
                    {
                        int thumbTop = Value2ThumbTopPosition(Value);
                        if (absolutePosition < thumbTop)
                        {
                            relativeY = absolutePosition - SystemInformation.HorizontalScrollBarArrowWidth;
                            return EnhancedScrollBarMouseLocation.TopOrLeftTrack;
                        }
                        else if (absolutePosition < thumbTop + ThumbLength)
                        {
                            relativeY = absolutePosition - thumbTop;
                            return EnhancedScrollBarMouseLocation.Thumb;
                        }
                        else
                        {
                            relativeY = absolutePosition - thumbTop - ThumbLength;
                            return EnhancedScrollBarMouseLocation.BottomOrRightTrack;
                        }
                    }
                }
            }


            #endregion

            #region Context menu event handlers

            private void topToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Minimum, Value, ScrollEventType.First);
                Value = Minimum;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Maximum, Value, ScrollEventType.Last);
                Value = Maximum;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private void scrollHereToolStripMenuItem_Click(object sender, EventArgs e)
            {
                NavigateTo(HotValue);
            }

            private void pageUpToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Value - LargeChange, Value, ScrollEventType.LargeDecrement);
                Value -= LargeChange;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private void pageDownToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Value + LargeChange, Value, ScrollEventType.LargeIncrement);
                Value += LargeChange;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private void scrollUpToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Value - SmallChange, Value, ScrollEventType.SmallDecrement);
                Value -= SmallChange;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            private void scrollDownToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OnScroll(Value + SmallChange, Value, ScrollEventType.SmallIncrement);
                Value += SmallChange;
                OnScroll(Value, Value, ScrollEventType.EndScroll);
            }

            #endregion

        }

        #region Enhanced event argument definitions

        /// <summary>
        /// Arguments for EnhancedScrollEvent.
        /// </summary>
        public class EnhancedScrollEventArgs : EventArgs
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            public EnhancedScrollEventArgs()
            {
                this.NewValue = 0;
                this.OldValue = 0;
                this.Type = ScrollEventType.EndScroll;
                this.ScrollOrientation = ScrollOrientation.VerticalScroll;

            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="OldValue">Previous EnhancedScrollBar value.</param>
            /// <param name="NewValue">New EnhancedScrollBar value.</param>
            /// <param name="Type">Type of the scroll event.</param>
            /// <param name="ScrollOrientation">EnhancedScrollBar orientation.</param>
            public EnhancedScrollEventArgs(decimal OldValue, decimal NewValue, ScrollEventType Type, ScrollOrientation ScrollOrientation)
            {
                this.NewValue = NewValue;
                this.OldValue = OldValue;
                this.ScrollOrientation = ScrollOrientation;
                this.Type = Type;
            }

            /// <summary>
            /// Previous EnhancedScrollBar value.
            /// </summary>
            public decimal OldValue { private set; get; }

            /// <summary>
            /// New EnhancedScrollBar value.
            /// </summary>
            public decimal NewValue { private set; get; }

            /// <summary>
            /// EnhancedScrollBar orientation.
            /// </summary>
            public ScrollOrientation ScrollOrientation { private set; get; }

            /// <summary>
            /// Type of the scroll event.
            /// </summary>
            public ScrollEventType Type { private set; get; }
        }

        /// <summary>
        /// Arguments for mouse related events.
        /// </summary>
        public class EnhancedMouseEventArgs : MouseEventArgs
        {

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="Value">ScrollBar <c>Value when event occurred.</c></param>
            /// <param name="MouseArgs">Original <c>MouseArgs</c>.</param>
            /// <param name="ScrollBarSection">Section of the EnhancedScrollBar where mouse pointer is located.</param>
            public EnhancedMouseEventArgs(decimal Value, MouseEventArgs MouseArgs, EnhancedScrollBarMouseLocation ScrollBarSection) : base(MouseArgs.Button, MouseArgs.Clicks, MouseArgs.X, MouseArgs.Y, MouseArgs.Delta)
            {

                this.Value = Value;
                this.ScrollBarSection = ScrollBarSection;

            }

            /// <summary>
            /// ScrollBar <c>Value</c> when event occurred.
            /// </summary>
            public decimal Value { private set; get; }


            /// <summary>
            /// Section of the EnhancedScrollBar where mouse pointer is located.
            /// </summary>
            public EnhancedScrollBarMouseLocation ScrollBarSection { private set; get; }
        }

        #endregion

        #region Public Enumerators

        /// <summary>
        /// Area of ScrollBar definitions. Used to describe relation of mouse pointer location
        /// to the distinct part of ScrollBar.
        /// </summary>
        public enum EnhancedScrollBarMouseLocation
        {
            /// <summary>
            /// Located outside of the ScrollBar.
            /// </summary>
            OutsideScrollBar,

            /// <summary>
            /// Located over top (for vertical ScrollBar) or 
            /// over left hand side arrow (for horizontal ScrollBar).
            /// </summary>
            TopOrLeftArrow,

            /// <summary>
            /// Located over top (for vertical Scrollbar) or
            /// over left hand side track (for horizontal ScrollBar).
            /// Track is the area between arrow and thumb images.
            /// </summary>
            TopOrLeftTrack,

            /// <summary>
            /// Located over ScrollBar thumb. Thumb is movable portion of the ScrollBar.
            /// </summary>
            Thumb,

            /// <summary>
            /// Located over bottom (for vertical Scrollbar) or
            /// over right hand side track (for horizontal ScrollBar).
            /// Track is the area between arrow and thumb images.
            /// </summary>
            BottomOrRightTrack,

            /// <summary>
            /// Located over bottom (for vertical ScrollBar) or 
            /// over right hand side arrow (for horizontal ScrollBar).
            /// </summary>
            BottomOrRightArrow
        }

        #endregion

        public class TreePanel : Panel
        {

            [DllImport("user32.dll")]

            private static extern int SendMessage(IntPtr hWnd,

               int msg, int wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);
            [DllImport("user32.dll")]
            static extern int SetScrollPos(IntPtr hWnd, int nBar,
                    int nPos, bool bRedraw);

            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int SB_THUMBPOSITION = 4;

            Dictionary<int, TreeNode> dict { get; set; }

            public bool expandEnabled = true;

            public TreePanel(TreeViewEx vv)
            {
            v = vv;
            this.BackColor = Color.White;
            //Controls.Add(v);

          
        }
        public void ScrollBarEx(ScrollBarEx s)
        {
            
            s.LargeChange = new decimal(new int[] {
            10,
            0,
            0,
            0});
            
            s.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            s.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            
            s.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            
            s.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            
        }
        public void Initialize()
        {
            ve = new ScrollBarEx();
            ve.Orientation = Orientation.Vertical;
            ScrollBarEx(ve);
            Controls.Add(ve);
            ve.Dock = DockStyle.Right;
            pe = new ScrollBarEx();
            pe.Orientation = Orientation.Horizontal;
            ScrollBarEx(pe);
            Controls.Add(pe);
            pe.Dock = DockStyle.Bottom;
            Controls.Add(v);
            LoadHandlers();
            //Arrange();
            AdjustVisibility();
        }
            public ScrollBarEx ve { get; set; }
            public ScrollBarEx pe { get; set; }
            public TreeViewEx v { get; set; }
            bool vve = true;
            bool pev = true;
            public void LoadHandlers()
            {
                v.Scrollable = true;
                ve.Scroll += Ve_Scroll;
                pe.Scroll += Pe_Scroll;
                
                v.AfterSelect += V_AfterSelect;

                v.AfterExpand += V_AfterExpand;

                v.AfterCollapse += V_AfterCollapse;

            v.SizeChanged += V_SizeChanged;
            }

        private void V_SizeChanged(object sender, EventArgs e)
        {
            AdjustVisibility();
        }

        private void Pe_Scroll(object sender, EnhancedScrollEventArgs e)
            {   
                SendMessage(v.Handle, WM_HSCROLL, (int)e.NewValue * 0x10000 + 4, new IntPtr(0));
                v.Refresh();
            }

            public void LoadExpandHandlers(bool load)
            {
                if (load)
                {
                    v.AfterExpand += V_AfterExpand;
                    v.AfterCollapse += V_AfterCollapse;
                }
                else
                {
                    v.AfterExpand -= V_AfterExpand;
                    v.AfterCollapse -= V_AfterCollapse;
                }
            }
            private void V_AfterCollapse(object sender, TreeViewEventArgs e)
            {
                if (!expandEnabled)
                    return;

                TreeNode node = e.Node;

                MeasureHeightForCollapse(node);

                ve.Minimum = 1;
                ve.Maximum = mw;
                pe.Maximum = mh;
            AdjustVisibility();
            }

            private void V_AfterExpand(object sender, TreeViewEventArgs e)
            {
                if (!expandEnabled)
                    return;
                //if (e.Action == TreeViewAction.Expand)
                //    return;
                MeasureHeightForExpand(v);
                ve.Minimum = 1;
                ve.Maximum = mw;
                pe.Maximum = mh;
                AdjustVisibility();
            }

            private void V_AfterSelect(object sender, TreeViewEventArgs e)
            {
                TreeNode node = v.SelectedNode;
                if (node == null)
                    return;
                if (!(node.Tag is int))
                    return;

                int index = (int)node.Tag;

                ve.Value = index;

            }

            private void Ve_Scroll(object sender, EnhancedScrollEventArgs e)
            {

                if (e == null)
                    return;

                int value = (int)e.NewValue;
                var node = GetNodeAt(/*0, */value/*, v*/);

                if (node == null)
                    return;
                node.EnsureVisible();
            }

            public void Arrange()
            {
                v.Scrollable = true;

                int w = ve.Width;
                if (!vve)
                    w = 0;
                int ww = Width - w;
                pe.Height = ve.Width;
                pe.Width = ww;
                int h = pe.Height;
                if (!pev)
                    h = 0;
                int hh = Height - h;
            v.Location = new Point(0, 0);
            v.Width = ww;
            v.Height = hh;
            ve.Height = hh;
                
                
                pe.Location = new Point(0, v.Height);
                ve.Location = new Point(v.Width, 0);
            ve.Visible = true;
            pe.Visible = true;

            }

            void AdjustVisibility()
        {
            ve.Height = v.Height;
            pe.Width = v.Width;

            if (mw * 20 < ve.Height)
            {
                Controls.Remove(ve);
            }
            else Controls.Add(ve);
            if (mh < pe.Width)
            {
                Controls.Remove(pe);
            }
            else Controls.Add(pe);
        }

            int mw = 0;
            int mh = 0;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeight(TreeView v)
            {
                dict = new Dictionary<int, TreeNode>();
                mw = 0;
                mh = 0;
                
                foreach (TreeNode node in v.Nodes)
                {

                    mw += 1;
                    var w = node.Bounds.Right;
                    if (w > mh)
                        mh = w;
                    dict.Add(mw, node);
                    if (node.IsExpanded)
                    {
                        //    mw += 1;
                        MeasureHeight(node);

                    }
                }
            }
          
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeight(TreeNode v)
            {


                foreach (TreeNode node in v.Nodes)
                {
                    mw += 1;
                    var w = node.Bounds.Right; /*/MeasureTextByGraphics(node.Text.Trim())*/;
                    if (w > mh)
                        mh = w;
                    node.Tag = mw;
                    dict.Add(mw, node);
                    if (node.IsExpanded)
                    {
                        MeasureHeight(node);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeightForExpand(TreeView v)
            {
                mw = 0;
                mh = 0;
                dict = new Dictionary<int, TreeNode>();

                foreach (TreeNode node in v.Nodes)
                {
                    mw += 1;
                    var w = node.Bounds.Right;
                    if (w > mh)
                        mh = w;
                dict.Add(mw, node);
                    if (node.IsExpanded)
                    {

                        MeasureHeightForExpand(node);

                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeightForExpand(TreeNode v)
            {
                foreach (TreeNode node in v.Nodes)
                {
                    mw += 1;
                var w = node.Bounds.Right;
                if (w > mh)
                    mh = w;
                dict.Add(mw, node);
                    if (node.IsExpanded)
                    {

                        MeasureHeightForExpand(node);

                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeightForCollapse(TreeView v)
            {
                mw = 0;
                mh = 0;
                dict = new Dictionary<int, TreeNode>();
                foreach (TreeNode node in v.Nodes)
                {
                    mw += 1;
                var w = node.Bounds.Right;
                if (w > mh)
                    mh = w;
                dict.Add(mw, node);
                    if (node.IsExpanded)
                    {
                        MeasureHeightForCollapse(node);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            public void MeasureHeightForCollapse(TreeNode v)
            {
                foreach (TreeNode node in v.Nodes)
                {
                    mw += 1;
                var w = node.Bounds.Right;
                if (w > mh)
                    mh = w;
                dict.Add(mw, node);
                    if (node.IsExpanded)
                    {
                        MeasureHeightForCollapse(node);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="y"></param>
            /// <returns></returns>
            public TreeNode GetNodeAt(int y)
            {
            if (dict == null)
                return null;
                if (dict.ContainsKey(y))
                    return dict[y];
                else return null;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="v"></param>
            /// <returns></returns>
            public TreeNode GetNodeAt(int x, int y, TreeView v)
            {
                foreach (TreeNode node in v.Nodes)
                {
                    //    if (node.Bounds.Top <= y && node.Bounds.Bottom >= y)
                    if (node.Tag != null && ((int)node.Tag) == y)
                        return node;
                    TreeNode ng = GetNodeAt(x, y, node);
                    if (ng != null)
                        return ng;
                }
                return null;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="node"></param>
            /// <returns></returns>
            public TreeNode GetNodeAt(int x, int y, TreeNode node)
            {
                foreach (TreeNode nodes in node.Nodes)
                {
                    //if (nodes.Bounds.Top <= y && nodes.Bounds.Bottom >= y)
                    if (nodes.Tag != null && ((int)nodes.Tag) == y)
                        return nodes;
                    TreeNode ng = GetNodeAt(x, y, nodes);
                    if (ng != null)
                        return ng;

                }
                return null;
            }
            /// <summary>
            /// 
            /// </summary>
            public void Scrollbars()
            {
                MeasureHeight(v);
                ve.Minimum = 1;
                ve.Maximum = mw;
                pe.Maximum = mh;

            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //public class TreeViewEx : TreeView
        //{
        //    [DllImport("user32.dll")]
        //    private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        //    const int WM_VSCROLL = 0x115;
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="m"></param>
        //    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        //    protected override void WndProc(ref Message m)
        //    {
        //        ShowScrollBar(Handle, 3, 0);

        //        base.WndProc(ref m);
        //    }
        //}

    }
