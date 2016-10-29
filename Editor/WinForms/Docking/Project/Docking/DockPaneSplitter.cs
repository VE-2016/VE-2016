
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class DockPaneSplitter : Control
    {
        private DockPane _pane;

        public DockPaneSplitter(DockPane pane)
        {
            SetStyle(ControlStyles.Selectable, false);
            _pane = pane;
        }

        public DockPane DockPane
        {
            get { return _pane; }
        }

        private DockAlignment _alignment;
        public DockAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                if (_alignment == DockAlignment.Left || _alignment == DockAlignment.Right)
                    Cursor = Cursors.VSplit;
                else if (_alignment == DockAlignment.Top || _alignment == DockAlignment.Bottom)
                    Cursor = Cursors.HSplit;
                else
                    Cursor = Cursors.Default;

                if (DockPane.DockState == DockState.Document)
                    Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DockPane.DockState != DockState.Document)
                return;

            Graphics g = e.Graphics;
            Rectangle rect = ClientRectangle;
            if (Alignment == DockAlignment.Top || Alignment == DockAlignment.Bottom)
                g.DrawLine(SystemPens.ControlDark, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            else if (Alignment == DockAlignment.Left || Alignment == DockAlignment.Right)
                g.DrawLine(SystemPens.ControlDarkDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            _pane.DockPanel.DragHandler.BeginDragPaneSplitter(this);
        }
    }
}
