


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class DockOutline : DockOutlineBase
    {
        public DockOutline()
        {
            _dragForm = new DragForm();
            SetDragForm(Rectangle.Empty);
            DragForm.BackColor = Color.FromArgb(96, 128, 186);// Color.Blue;
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
            else if (DockTo is DockContainer)
                SetOutline(DockTo as DockContainer, Dock, (ContentIndex != 0));
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

        private void SetOutline(DockContainer dockPanel, DockStyle dock, bool fullPanelEdge)
        {
            Rectangle rect = fullPanelEdge ? dockPanel.DockArea : dockPanel.DocumentWindowBounds;
            rect.Location = dockPanel.PointToScreen(rect.Location);
            if (dock == DockStyle.Top)
            {
                int height = (int)(rect.Height * dockPanel.DockBottomPortion);
                rect = new Rectangle(rect.X, rect.Y, rect.Width, height);
            }
            else if (dock == DockStyle.Bottom)
            {
                int height = (int)(rect.Height * dockPanel.DockBottomPortion);
                rect = new Rectangle(rect.X, rect.Bottom - height, rect.Width, height);
            }
            else if (dock == DockStyle.Left)
            {
                int width = (int)(rect.Width * dockPanel.DockLeftPortion);
                rect = new Rectangle(rect.X, rect.Y, width, rect.Height);
            }
            else if (dock == DockStyle.Right)
            {
                int width = (int)(rect.Width * dockPanel.DockRightPortion);
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
                using (GraphicsPath path = pane.TabStripControl.GetOutlinePath(contentIndex))
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
                DragForm.Region = new Region(Rectangle.Empty);
            else if (DragForm.Region != null)
                DragForm.Region = null;
        }

        private void SetDragForm(Rectangle rect, Region region)
        {
            DragForm.Bounds = rect;
            DragForm.Region = region;
        }
    }
}
