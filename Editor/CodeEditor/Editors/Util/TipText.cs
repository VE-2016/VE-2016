
using System;
using System.Drawing;

namespace AIMS.Libraries.CodeEditor.Util
{
    internal class CountTipText : TipText
    {
        private float _triHeight = 10;
        private float _triWidth = 10;

        public CountTipText(Graphics graphics, Font font, string text) : base(graphics, font, text)
        {
        }

        private void DrawTriangle(float x, float y, bool flipped)
        {
            Brush brush = BrushRegistry.GetBrush(Color.FromArgb(192, 192, 192));
            base.Graphics.FillRectangle(brush, new RectangleF(x, y, _triHeight, _triHeight));
            float triHeight2 = _triHeight / 2;
            float triHeight4 = _triHeight / 4;
            brush = Brushes.Black;
            if (flipped)
            {
                base.Graphics.FillPolygon(brush, new PointF[] {
                                              new PointF(x,                y + triHeight2 - triHeight4),
                                              new PointF(x + _triWidth / 2, y + triHeight2 + triHeight4),
                                              new PointF(x + _triWidth,     y + triHeight2 - triHeight4),
                                          });
            }
            else
            {
                base.Graphics.FillPolygon(brush, new PointF[] {
                                              new PointF(x,                y +  triHeight2 + triHeight4),
                                              new PointF(x + _triWidth / 2, y +  triHeight2 - triHeight4),
                                              new PointF(x + _triWidth,     y +  triHeight2 + triHeight4),
                                          });
            }
        }

        public Rectangle DrawingRectangle1;
        public Rectangle DrawingRectangle2;

        public override void Draw(PointF location)
        {
            if (tipText != null && tipText.Length > 0)
            {
                base.Draw(new PointF(location.X + _triWidth + 4, location.Y));
                DrawingRectangle1 = new Rectangle((int)location.X + 2,
                                                  (int)location.Y + 2,
                                                  (int)(_triWidth),
                                                  (int)(_triHeight));
                DrawingRectangle2 = new Rectangle((int)(location.X + base.AllocatedSize.Width - _triWidth - 2),
                                                  (int)location.Y + 2,
                                                  (int)(_triWidth),
                                                  (int)(_triHeight));
                DrawTriangle(location.X + 2, location.Y + 2, false);
                DrawTriangle(location.X + base.AllocatedSize.Width - _triWidth - 2, location.Y + 2, true);
            }
        }

        protected override void OnMaximumSizeChanged()
        {
            if (IsTextVisible())
            {
                SizeF tipSize = Graphics.MeasureString
                    (tipText, tipFont, MaximumSize,
                     GetInternalStringFormat());
                tipSize.Width += _triWidth * 2 + 8;
                SetRequiredSize(tipSize);
            }
            else
            {
                SetRequiredSize(SizeF.Empty);
            }
        }
    }

    internal class TipText : TipSection
    {
        protected StringAlignment horzAlign;
        protected StringAlignment vertAlign;
        protected Color tipColor;
        protected Font tipFont;
        protected StringFormat tipFormat;
        protected string tipText;

        public TipText(Graphics graphics, Font font, string text) :
            base(graphics)
        {
            tipFont = font; tipText = text;
            if (text != null && text.Length > short.MaxValue)
                throw new ArgumentException("TipText: text too long (max. is " + short.MaxValue + " characters)", "text");

            Color = SystemColors.InfoText;
            HorizontalAlignment = StringAlignment.Near;
            VerticalAlignment = StringAlignment.Near;
        }

        public override void Draw(PointF location)
        {
            if (IsTextVisible())
            {
                RectangleF drawRectangle = new RectangleF(location, AllocatedSize);

                Graphics.DrawString(tipText, tipFont,
                                    BrushRegistry.GetBrush(Color),
                                    drawRectangle,
                                    GetInternalStringFormat());
            }
        }

        protected StringFormat GetInternalStringFormat()
        {
            if (tipFormat == null)
            {
                tipFormat = CreateTipStringFormat(horzAlign, vertAlign);
            }

            return tipFormat;
        }

        protected override void OnMaximumSizeChanged()
        {
            base.OnMaximumSizeChanged();

            if (IsTextVisible())
            {
                SizeF tipSize = Graphics.MeasureString
                    (tipText, tipFont, MaximumSize,
                     GetInternalStringFormat());

                SetRequiredSize(tipSize);
            }
            else
            {
                SetRequiredSize(SizeF.Empty);
            }
        }

        private static StringFormat CreateTipStringFormat(StringAlignment horizontalAlignment, StringAlignment verticalAlignment)
        {
            StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone();
            format.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
            // note: Align Near, Line Center seemed to do something before

            format.Alignment = horizontalAlignment;
            format.LineAlignment = verticalAlignment;

            return format;
        }

        protected bool IsTextVisible()
        {
            return tipText != null && tipText.Length > 0;
        }

        public Color Color
        {
            get
            {
                return tipColor;
            }
            set
            {
                tipColor = value;
            }
        }

        public StringAlignment HorizontalAlignment
        {
            get
            {
                return horzAlign;
            }
            set
            {
                horzAlign = value;
                tipFormat = null;
            }
        }

        public StringAlignment VerticalAlignment
        {
            get
            {
                return vertAlign;
            }
            set
            {
                vertAlign = value;
                tipFormat = null;
            }
        }
    }
}
