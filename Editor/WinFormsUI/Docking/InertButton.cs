using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class InertButton : Button
    {
        private enum RepeatClickStatus
        {
            Disabled,
            Started,
            Repeating,
            Stopped
        }

        private class RepeatClickEventArgs : EventArgs
        {
            private static RepeatClickEventArgs s_empty;

            static RepeatClickEventArgs()
            {
                s_empty = new RepeatClickEventArgs();
            }

            public new static RepeatClickEventArgs Empty
            {
                get { return s_empty; }
            }
        }

        private IContainer _components = new Container();
        private int _borderWidth = 1;
        private bool _mouseOver = false;
        private bool _mouseCapture = false;
        private bool _isPopup = false;
        private Image _imageEnabled = null;
        private Image _imageDisabled = null;
        private int _imageIndexEnabled = -1;
        private int _imageIndexDisabled = -1;
        private bool _monochrom = true;
        private ToolTip _toolTip = null;
        private string _toolTipText = "";
        private Color _borderColor = Color.Empty;

        public InertButton()
        {
            InternalConstruct(null, null);
        }

        public InertButton(Image imageEnabled)
        {
            InternalConstruct(imageEnabled, null);
        }

        public InertButton(Image imageEnabled, Image imageDisabled)
        {
            InternalConstruct(imageEnabled, imageDisabled);
        }

        private void InternalConstruct(Image imageEnabled, Image imageDisabled)
        {
            // Remember parameters
            ImageEnabled = imageEnabled;
            ImageDisabled = imageDisabled;

            // Prevent drawing flicker by blitting from memory in WM_PAINT
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            // Prevent base class from trying to generate double click events and
            // so testing clicks against the double click time and rectangle. Getting
            // rid of this allows the user to press then release button very quickly.
            //SetStyle(ControlStyles.StandardDoubleClick, false);

            // Should not be allowed to select this control
            SetStyle(ControlStyles.Selectable, false);

            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Tick += new EventHandler(Timer_Tick);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                    _components.Dispose();
            }
            base.Dispose(disposing);
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeBorderColor()
        {
            return (_borderColor != Color.Empty);
        }

        public int BorderWidth
        {
            get { return _borderWidth; }

            set
            {
                if (value < 1)
                    value = 1;
                if (_borderWidth != value)
                {
                    _borderWidth = value;
                    Invalidate();
                }
            }
        }

        public Image ImageEnabled
        {
            get
            {
                if (_imageEnabled != null)
                    return _imageEnabled;

                try
                {
                    if (ImageList == null || ImageIndexEnabled == -1)
                        return null;
                    else
                        return ImageList.Images[_imageIndexEnabled];
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                if (_imageEnabled != value)
                {
                    _imageEnabled = value;
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializeImageEnabled()
        {
            return (_imageEnabled != null);
        }

        public Image ImageDisabled
        {
            get
            {
                if (_imageDisabled != null)
                    return _imageDisabled;

                try
                {
                    if (ImageList == null || ImageIndexDisabled == -1)
                        return null;
                    else
                        return ImageList.Images[_imageIndexDisabled];
                }
                catch
                {
                    return null;
                }
            }

            set
            {
                if (_imageDisabled != value)
                {
                    _imageDisabled = value;
                    Invalidate();
                }
            }
        }

        public int ImageIndexEnabled
        {
            get { return _imageIndexEnabled; }
            set
            {
                if (_imageIndexEnabled != value)
                {
                    _imageIndexEnabled = value;
                    Invalidate();
                }
            }
        }

        public int ImageIndexDisabled
        {
            get { return _imageIndexDisabled; }
            set
            {
                if (_imageIndexDisabled != value)
                {
                    _imageIndexDisabled = value;
                    Invalidate();
                }
            }
        }

        public bool IsPopup
        {
            get { return _isPopup; }

            set
            {
                if (_isPopup != value)
                {
                    _isPopup = value;
                    Invalidate();
                }
            }
        }

        public bool Monochrome
        {
            get { return _monochrom; }
            set
            {
                if (value != _monochrom)
                {
                    _monochrom = value;
                    Invalidate();
                }
            }
        }

        public bool RepeatClick
        {
            get { return (ClickStatus != RepeatClickStatus.Disabled); }
            set { ClickStatus = RepeatClickStatus.Stopped; }
        }

        private RepeatClickStatus _clickStatus = RepeatClickStatus.Disabled;
        private RepeatClickStatus ClickStatus
        {
            get { return _clickStatus; }
            set
            {
                if (_clickStatus == value)
                    return;

                _clickStatus = value;
                if (ClickStatus == RepeatClickStatus.Started)
                {
                    Timer.Interval = RepeatClickDelay;
                    Timer.Enabled = true;
                }
                else if (ClickStatus == RepeatClickStatus.Repeating)
                    Timer.Interval = RepeatClickInterval;
                else
                    Timer.Enabled = false;
            }
        }

        private int _repeatClickDelay = 500;
        public int RepeatClickDelay
        {
            get { return _repeatClickDelay; }
            set { _repeatClickDelay = value; }
        }

        private int _repeatClickInterval = 100;
        public int RepeatClickInterval
        {
            get { return _repeatClickInterval; }
            set { _repeatClickInterval = value; }
        }

        private Timer _timer;
        private Timer Timer
        {
            get { return _timer; }
        }

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                if (_toolTipText != value)
                {
                    if (_toolTip == null)
                        _toolTip = new ToolTip(_components);
                    _toolTipText = value;
                    _toolTip.SetToolTip(this, value);
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_mouseCapture && _mouseOver)
                OnClick(RepeatClickEventArgs.Empty);
            if (ClickStatus == RepeatClickStatus.Started)
                ClickStatus = RepeatClickStatus.Repeating;
        }

        /// <exclude/>
        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);

            if (mevent.Button != MouseButtons.Left)
                return;

            if (_mouseCapture == false || _mouseOver == false)
            {
                _mouseCapture = true;
                _mouseOver = true;

                //Redraw to show button state
                Invalidate();
            }

            if (RepeatClick)
            {
                OnClick(RepeatClickEventArgs.Empty);
                ClickStatus = RepeatClickStatus.Started;
            }
        }

        /// <exclude/>
        protected override void OnClick(EventArgs e)
        {
            if (RepeatClick && !(e is RepeatClickEventArgs))
                return;

            base.OnClick(e);
        }

        /// <exclude/>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            if (mevent.Button != MouseButtons.Left)
                return;

            if (_mouseOver == true || _mouseCapture == true)
            {
                _mouseOver = false;
                _mouseCapture = false;

                // Redraw to show button state
                Invalidate();
            }

            if (RepeatClick)
                ClickStatus = RepeatClickStatus.Stopped;
        }

        /// <exclude/>
        protected override void OnMouseMove(MouseEventArgs mevent)
        {
            base.OnMouseMove(mevent);

            // Is mouse point inside our client rectangle
            bool over = this.ClientRectangle.Contains(new Point(mevent.X, mevent.Y));

            // If entering the button area or leaving the button area...
            if (over != _mouseOver)
            {
                // Update state
                _mouseOver = over;

                // Redraw to show button state
                Invalidate();
            }
        }

        /// <exclude/>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Update state to reflect mouse over the button area
            if (!_mouseOver)
            {
                _mouseOver = true;

                // Redraw to show button state
                Invalidate();
            }

            base.OnMouseEnter(e);
        }

        /// <exclude/>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Update state to reflect mouse not over the button area
            if (_mouseOver)
            {
                _mouseOver = false;

                // Redraw to show button state
                Invalidate();
            }

            base.OnMouseLeave(e);
        }

        /// <exclude/>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            DrawBackground(pevent.Graphics);
            DrawImage(pevent.Graphics);
            DrawText(pevent.Graphics);
            DrawBorder(pevent.Graphics);
        }

        private void DrawBackground(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                g.FillRectangle(brush, ClientRectangle);
            }
        }

        private void DrawImage(Graphics g)
        {
            Image image = this.Enabled ? ImageEnabled : ((ImageDisabled != null) ? ImageDisabled : ImageEnabled);
            ImageAttributes imageAttr = null;

            if (null == image)
                return;

            if (_monochrom)
            {
                imageAttr = new ImageAttributes();

                // transform the monochrom image
                // white -> BackColor
                // black -> ForeColor
                ColorMap[] colorMap = new ColorMap[2];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.White;
                colorMap[0].NewColor = this.BackColor;
                colorMap[1] = new ColorMap();
                colorMap[1].OldColor = Color.Black;
                colorMap[1].NewColor = this.ForeColor;
                imageAttr.SetRemapTable(colorMap);
            }

            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            if ((!Enabled) && (null == ImageDisabled))
            {
                using (Bitmap bitmapMono = new Bitmap(image, ClientRectangle.Size))
                {
                    if (imageAttr != null)
                    {
                        using (Graphics gMono = Graphics.FromImage(bitmapMono))
                        {
                            gMono.DrawImage(image, new Point[3] { new Point(0, 0), new Point(image.Width - 1, 0), new Point(0, image.Height - 1) }, rect, GraphicsUnit.Pixel, imageAttr);
                        }
                    }
                    ControlPaint.DrawImageDisabled(g, bitmapMono, 0, 0, this.BackColor);
                }
            }
            else
            {
                // Three points provided are upper-left, upper-right and 
                // lower-left of the destination parallelogram. 
                Point[] pts = new Point[3];
                pts[0].X = (Enabled && _mouseOver && _mouseCapture) ? 1 : 0;
                pts[0].Y = (Enabled && _mouseOver && _mouseCapture) ? 1 : 0;
                pts[1].X = pts[0].X + ClientRectangle.Width;
                pts[1].Y = pts[0].Y;
                pts[2].X = pts[0].X;
                pts[2].Y = pts[1].Y + ClientRectangle.Height;

                if (imageAttr == null)
                    g.DrawImage(image, pts, rect, GraphicsUnit.Pixel);
                else
                    g.DrawImage(image, pts, rect, GraphicsUnit.Pixel, imageAttr);
            }
        }

        private void DrawText(Graphics g)
        {
            if (Text == string.Empty)
                return;

            Rectangle rect = ClientRectangle;

            rect.X += BorderWidth;
            rect.Y += BorderWidth;
            rect.Width -= 2 * BorderWidth;
            rect.Height -= 2 * BorderWidth;

            StringFormat stringFormat = new StringFormat();

            if (TextAlign == ContentAlignment.TopLeft)
            {
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Near;
            }
            else if (TextAlign == ContentAlignment.TopCenter)
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Near;
            }
            else if (TextAlign == ContentAlignment.TopRight)
            {
                stringFormat.Alignment = StringAlignment.Far;
                stringFormat.LineAlignment = StringAlignment.Near;
            }
            else if (TextAlign == ContentAlignment.MiddleLeft)
            {
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Center;
            }
            else if (TextAlign == ContentAlignment.MiddleCenter)
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
            }
            else if (TextAlign == ContentAlignment.MiddleRight)
            {
                stringFormat.Alignment = StringAlignment.Far;
                stringFormat.LineAlignment = StringAlignment.Center;
            }
            else if (TextAlign == ContentAlignment.BottomLeft)
            {
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Far;
            }
            else if (TextAlign == ContentAlignment.BottomCenter)
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Far;
            }
            else if (TextAlign == ContentAlignment.BottomRight)
            {
                stringFormat.Alignment = StringAlignment.Far;
                stringFormat.LineAlignment = StringAlignment.Far;
            }

            using (Brush brush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, Font, brush, rect, stringFormat);
            }
        }

        private void DrawBorder(Graphics g)
        {
            ButtonBorderStyle bs;

            // Decide on the type of border to draw around image
            if (!this.Enabled)
                bs = IsPopup ? ButtonBorderStyle.Outset : ButtonBorderStyle.Solid;
            else if (_mouseOver && _mouseCapture)
                bs = ButtonBorderStyle.Inset;
            else if (IsPopup || _mouseOver)
                bs = ButtonBorderStyle.Outset;
            else
                bs = ButtonBorderStyle.Solid;

            Color colorLeftTop;
            Color colorRightBottom;
            if (bs == ButtonBorderStyle.Solid)
            {
                colorLeftTop = this.BackColor;
                colorRightBottom = this.BackColor;
            }
            else if (bs == ButtonBorderStyle.Outset)
            {
                colorLeftTop = _borderColor.IsEmpty ? this.BackColor : _borderColor;
                colorRightBottom = this.BackColor;
            }
            else
            {
                colorLeftTop = this.BackColor;
                colorRightBottom = _borderColor.IsEmpty ? this.BackColor : _borderColor;
            }
            ControlPaint.DrawBorder(g, this.ClientRectangle,
                colorLeftTop, _borderWidth, bs,
                colorLeftTop, _borderWidth, bs,
                colorRightBottom, _borderWidth, bs,
                colorRightBottom, _borderWidth, bs);
        }

        /// <exclude/>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (Enabled == false)
            {
                _mouseOver = false;
                _mouseCapture = false;
                if (RepeatClick && ClickStatus != RepeatClickStatus.Stopped)
                    ClickStatus = RepeatClickStatus.Stopped;
            }
            Invalidate();
        }
    }
}