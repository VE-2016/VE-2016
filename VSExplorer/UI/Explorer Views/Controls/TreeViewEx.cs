using LibGit2Sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using VSParsers;
using VSProvider;

namespace WinExplorer.UI
{
    public class TreeViewEx : TreeView
    {

        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }


        public TreeViewEx() : base()
        {
            Init();
            // Enable default double buffering processing (DoubleBuffered returns true)
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            // Disable default CommCtrl painting on non-Vista systems
            //if (Environment.OSVersion.Version.Major < 6)
            //    SetStyle(ControlStyles.UserPaint, true);
            //DoubleBuffered = true;
            this.Validating += TreeViewEx_Validating;
        }
        public bool LostFocusEnabled = true;
        private void TreeViewEx_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!LostFocusEnabled)
                e.Cancel = true;
        }

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
        private int currentMsgCount;
        protected override void WndProc(ref Message m)
        {
            
            
            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL)
            {

                var nfy = m.WParam.ToInt32() & 0xFFFF;
                if (nfy == SB_THUMBTRACK)
                {
                    currentMsgCount++;
                    if (currentMsgCount % skipMsgCount == 0)
                        base.WndProc(ref m);
                    return;
                }
                if (nfy == SB_ENDSCROLL)
                    currentMsgCount = 0;

                base.WndProc(ref m);
            }
            else
                base.WndProc(ref m);
        }
        protected override void OnPaintBackground(    PaintEventArgs pevent)
        {

        }
        //protected override bool DoubleBuffered { get; set; }
        ImageList g = new ImageList();
        void Init()
        {
            
            g.Images.Add("forms", new Bitmap(ve_resource.WindowsForm_256x, new Size(25, 25)));
            this.ImageList = g;

            arrowpen = new Pen(Color.Black, 1);
            arrowfill = Brushes.Black;
        }
        public bool IsSourceControl = false;
        VSSolution vs { get; set; }

        public void ScanForMembers(TreeNode node)
        {
            VSProject vp = ProjectItemInfo.VSProjectIfAny(node);
            string file = ProjectItemInfo.FileNameIfAny(node);
            if(vp != null)
                if (!string.IsNullOrEmpty(file))
                {
                    
                    if(vs.HasMembers(vp, file,""))
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
        Repository repo { get; set; }

        public void ScanForSourceStatus()
        {
            VSSolution vs = Tag as VSSolution;
            if (vs == null)
                return;
            repo = new Repository(vs.SolutionPath);
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
        bool IsBitSet(Int32 b, byte nPos)
        {
            return new BitArray(new[] { b })[(int)Math.Log(nPos, 2)];
        }

        int ColumnWidth = 300;

        Pen arrowpen { get; set; }

        Brush arrowfill { get; set; }

        Brush selectedNotFocused = new SolidBrush(Color.LightGray);

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
                    VisualStyleRenderer renderer = new VisualStyleRenderer((ContainsFocus) ? VisualStyleElement.Button.PushButton.Hot
                                                                                           : VisualStyleElement.Button.PushButton.Normal);
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
            if (IsSourceControl) {
                p.Offset(iconLeft + 5, 0);
                if(projectItemInfo != null)
                {
                    //if (projectItemInfo.status != null)
                    {
                        if (projectItemInfo.status == FileStatus.Added)
                            e.Graphics.DrawImage(this.ImageList.Images["Add_thin"], p);
                        else if (projectItemInfo.status ==  FileStatus.Modified)
                            e.Graphics.DrawImage(this.ImageList.Images["Status_OK"], p);
                        else if (projectItemInfo.status == FileStatus.Unaltered)
                            e.Graphics.DrawImage(this.ImageList.Images["LockCyan_16x"], p);
                    }
                }
                //e.Graphics.DrawImage(this.ImageList.Images["LockCyan_16x"], p);
                p.Offset(13, 0);
            }
            else 
            p.Offset(iconLeft + 20, 0);
            if(e.Node.ImageKey == "" || !this.ImageList.Images.ContainsKey(e.Node.ImageKey))
                e.Graphics.DrawImage(this.g.Images[0], p);
            else
                e.Graphics.DrawImage(this.ImageList.Images[e.Node.ImageKey], p);
            p.X += 18;

            string[] columnTextList = text.Split('|');

            for (int iCol = 0; iCol < columnTextList.GetLength(0); iCol++)
            {
                Rectangle textRect = new Rectangle(textPos.X, textPos.Y, itemRect.Right - textPos.X, itemRect.Bottom - textPos.Y);
                if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, Color.White);
                else
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, SystemColors.ControlDarkDark);
                textPos.X += ColumnWidth;
            }

            // Draw Focussing box around the text
            if (e.State == TreeNodeStates.Focused)
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
            foreach(TreeNode node in Nodes)
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
}