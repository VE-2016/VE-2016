using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WinExplorer.UI
{
    public class TreeViewEx : TreeView
    {

        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }


        public TreeViewEx() : base()
        {
            Init();
        }
        ImageList g = new ImageList();
        void Init()
        {
            
            g.Images.Add("forms", new Bitmap(ve_resource.WindowsForm_256x, new Size(25, 25)));
            this.ImageList = g;

            arrowpen = new Pen(Color.Black, 1);
            arrowfill = Brushes.Black;
        }

        bool IsBitSet(Int32 b, byte nPos)
        {
            return new BitArray(new[] { b })[(int)Math.Log(nPos, 2)];
        }

        int ColumnWidth = 300;

        Pen arrowpen { get; set; }

        Brush arrowfill { get; set; }

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
            if (e.State != TreeNodeStates.Focused)
                e.Graphics.FillRectangle(backBrush, itemRect);
            Color separatorColor = Color.Gray;
            Pen separatorPen = new Pen(separatorColor);
            Color cs = Color.FromArgb(255, 51, 153, 255);
            if (!HideSelection)
            {
                if (/*IsBitSet((int)e.State, (int)TreeNodeStates.Selected) || */IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                {
                    Rectangle selRect = new Rectangle(textLeft, itemRect.Top, itemRect.Right - textLeft, itemRect.Height);
                    VisualStyleRenderer renderer = new VisualStyleRenderer((ContainsFocus) ? VisualStyleElement.Button.PushButton.Hot
                                                                                           : VisualStyleElement.Button.PushButton.Normal);
                    //renderer.DrawBackground(e.Graphics, e.Bounds);
                    Brush bodge = new SolidBrush(Color.FromArgb((IsBitSet((int)e.State, (int)TreeNodeStates.Hot)) ? 255 : 128, 255, 255, 255));
                    if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
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
                a.Offset(3, 3);
                Point b = new Point(new Size(pe));
                b.Offset(3, 11);
                Point c = new Point(new Size(pe));
                c.Offset(7, 7);
                Point d = new Point(new Size(pe));
                d.Offset(1, 9);
                Point f = new Point(new Size(pe));
                f.Offset(7, 3);
                Point g = new Point(new Size(pe));
                g.Offset(7, 9);
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
            p.Offset(iconLeft + 1 + 16/* + e.Node.Level * 30*/, 0);
            if(e.Node.ImageKey == "" || !this.ImageList.Images.ContainsKey(e.Node.ImageKey))
                e.Graphics.DrawImage(this.g.Images[0], p);
            else
                e.Graphics.DrawImage(this.ImageList.Images[e.Node.ImageKey], p);
            p.X += 20;

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

    }
}