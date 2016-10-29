
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AIMS.Libraries.CodeEditor.Util
{
    /// <summary>
    /// Contains brushes/pens for the text editor to speed up drawing. Re-Creation of brushes and pens
    /// seems too costly.
    /// </summary>
    public class BrushRegistry
    {
        private static Hashtable s_brushes = new Hashtable();
        private static Hashtable s_pens = new Hashtable();
        private static Hashtable s_dotPens = new Hashtable();

        public static Brush GetBrush(Color color)
        {
            if (!s_brushes.Contains(color))
            {
                Brush newBrush = new SolidBrush(color);
                s_brushes.Add(color, newBrush);
                return newBrush;
            }
            return s_brushes[color] as Brush;
        }

        public static Pen GetPen(Color color)
        {
            if (!s_pens.Contains(color))
            {
                Pen newPen = new Pen(color);
                s_pens.Add(color, newPen);
                return newPen;
            }
            return s_pens[color] as Pen;
        }

        public static Pen GetDotPen(Color bgColor, Color fgColor)
        {
            bool containsBgColor = s_dotPens.Contains(bgColor);
            if (!containsBgColor || !((Hashtable)s_dotPens[bgColor]).Contains(fgColor))
            {
                if (!containsBgColor)
                {
                    s_dotPens[bgColor] = new Hashtable();
                }

                HatchBrush hb = new HatchBrush(HatchStyle.Percent50, bgColor, fgColor);
                Pen newPen = new Pen(hb);
                ((Hashtable)s_dotPens[bgColor])[fgColor] = newPen;
                return newPen;
            }
            return ((Hashtable)s_dotPens[bgColor])[fgColor] as Pen;
        }
    }
}
