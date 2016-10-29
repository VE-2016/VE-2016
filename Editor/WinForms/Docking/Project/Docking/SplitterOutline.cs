using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class SplitterOutline
    {
        public SplitterOutline()
        {
            _dragForm = new DragForm();
            SetDragForm(Rectangle.Empty);
            DragForm.BackColor = Color.Black;
            DragForm.Opacity = 0.7;
            DragForm.Show(false);
        }

        private DragForm _dragForm;
        private DragForm DragForm
        {
            get { return _dragForm; }
        }

        public void Show(Rectangle rect)
        {
            SetDragForm(rect);
        }

        public void Close()
        {
            DragForm.Close();
        }

        private void SetDragForm(Rectangle rect)
        {
            DragForm.Bounds = rect;
            if (rect == Rectangle.Empty)
                DragForm.Region = new Region(Rectangle.Empty);
            else if (DragForm.Region != null)
                DragForm.Region = null;
        }
    }
}
