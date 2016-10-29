using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    public class LineMarginRender
    {
        protected Brush fillBrush = null;

        private Rectangle _Bounds = Rectangle.Empty;

        public LineMarginRender()
        {
            fillBrush = Brushes.White;
        }

        public virtual Rectangle Bounds
        {
            get
            {
                return _Bounds;
            }
            set
            {
                _Bounds = value;
            }
        }


        public virtual void Render(Graphics gfx)
        {
            if (_Bounds == Rectangle.Empty)
                return;

            gfx.FillRectangle(fillBrush, _Bounds);
        }
    }
}
