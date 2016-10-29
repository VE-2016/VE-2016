using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.Win32;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Summary description for SplitView.
    /// </summary>
    public class SplitViewWidget : Control
    {
        private Panel _vertical;
        private Panel _horizontal;
        private Panel _center;

        private Control _UpperLeft = null;
        private Control _UpperRight = null;
        private Control _LowerLeft = null;
        private Control _LowerRight = null;
        private Point _prevPos = new Point(0);
        private bool _firstTime = false;


        private SizeAction _action = 0;
        private Point _startPos = new Point(0, 0);

        /// <summary>
        /// an event fired when the split view is resized.
        /// </summary>
        public event EventHandler Resizing = null;

        /// <summary>
        /// an event fired when the top views are hidden.
        /// </summary>
        public event EventHandler HideTop = null;

        /// <summary>
        /// an event fired when the left views are hidden.
        /// </summary>
        public event EventHandler HideLeft = null;

        private void OnResizing()
        {
            if (Resizing != null)
                Resizing(this, new EventArgs());
        }

        private void OnHideLeft()
        {
            if (HideLeft != null)
                HideLeft(this, new EventArgs());
        }

        private void OnHideTop()
        {
            if (HideTop != null)
                HideTop(this, new EventArgs());
        }

        private enum SizeAction
        {
            None = 0,
            SizeH = 1,
            SizeV = 2,
            SizeA = 3
        }


        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        /// <summary>
        /// Default constructor for the splitview control
        /// </summary>
        public SplitViewWidget()
        {
            // This call is required by the Windows.Forms Form Designer.
            this.SetStyle(ControlStyles.ContainerControl, true);
            //			this.SetStyle(ControlStyles.AllPaintingInWmPaint ,false);
            //			this.SetStyle(ControlStyles.DoubleBuffer ,false);
            //			this.SetStyle(ControlStyles.Selectable,true);
            //			this.SetStyle(ControlStyles.ResizeRedraw ,true);
            //			this.SetStyle(ControlStyles.Opaque ,true);			
            //			this.SetStyle(ControlStyles.UserPaint,true);
            //SetStyle(ControlStyles.Selectable ,true);

            InitializeComponent();


            DoResize();
            ReSize(0, 0);
            //this.Refresh ();
            // TODO: Add any initialization after the InitForm call

        }

        //		protected override void OnLoad(EventArgs e)
        //		{
        //			
        //		}

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
#if DEBUG
			try
			{
				Console.WriteLine("disposing splitview");
			}
			catch
			{
			}
#endif
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or Sets the control that should be confined to the upper left view.
        /// </summary>
        public Control UpperLeft
        {
            get { return _UpperLeft; }
            set
            {
                _UpperLeft = value;
                DoResize();
            }
        }

        /// <summary>
        /// Gets or Sets the control that should be confined to the upper right view.
        /// </summary>
        public Control UpperRight
        {
            get { return _UpperRight; }
            set
            {
                _UpperRight = value;
                DoResize();
            }
        }

        /// <summary>
        /// Gets or Sets the control that should be confined to the lower left view.
        /// </summary>
        public Control LowerLeft
        {
            get { return _LowerLeft; }
            set
            {
                _LowerLeft = value;
                DoResize();
            }
        }

        /// <summary>
        /// Gets or Sets the control that should be confined to the lower right view.
        /// </summary>
        public Control LowerRight
        {
            get { return _LowerRight; }
            set
            {
                _LowerRight = value;
                DoResize();
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _vertical = new System.Windows.Forms.Panel();
            _horizontal = new System.Windows.Forms.Panel();
            _center = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Vertical
            // 
            _vertical.BackColor = System.Drawing.SystemColors.Control;
            _vertical.Cursor = System.Windows.Forms.Cursors.VSplit;
            _vertical.Name = "Vertical";
            _vertical.Size = new System.Drawing.Size(4, 264);
            _vertical.TabIndex = 0;
            _vertical.Resize += new System.EventHandler(this.Vertical_Resize);
            _vertical.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseUp);
            _vertical.DoubleClick += new System.EventHandler(this.Vertical_DoubleClick);
            _vertical.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseMove);
            _vertical.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseDown);
            // 
            // Horizontal
            // 
            _horizontal.BackColor = System.Drawing.SystemColors.Control;
            _horizontal.Cursor = System.Windows.Forms.Cursors.HSplit;
            _horizontal.Name = "Horizontal";
            _horizontal.Size = new System.Drawing.Size(320, 4);
            _horizontal.TabIndex = 1;
            _horizontal.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseUp);
            _horizontal.DoubleClick += new System.EventHandler(this.Horizontal_DoubleClick);
            _horizontal.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseMove);
            _horizontal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseDown);
            // 
            // Center
            // 
            _center.BackColor = System.Drawing.SystemColors.Control;
            _center.Cursor = System.Windows.Forms.Cursors.SizeAll;
            _center.Location = new System.Drawing.Point(146, 69);
            _center.Name = "Center";
            _center.Size = new System.Drawing.Size(24, 24);
            _center.TabIndex = 2;
            _center.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Center_MouseUp);
            _center.DoubleClick += new System.EventHandler(this.Center_DoubleClick);
            _center.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Center_MouseMove);
            _center.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Center_MouseDown);
            // 
            // SplitViewControl
            // 
            this.BackColor = System.Drawing.Color.Magenta;
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                {
                    _center,
                    _horizontal,
                    _vertical
                });
            this.Size = new System.Drawing.Size(200, 200);
            this.VisibleChanged += new System.EventHandler(this.SplitViewControl_VisibleChanged);
            this.Enter += new System.EventHandler(this.SplitViewControl_Enter);
            this.Leave += new System.EventHandler(this.SplitViewControl_Leave);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            DoResize();
        }

        private void DoResize()
        {
            //			int OldWidth=Horizontal.Width ;
            //			int OldHeight=Vertical.Height;
            int NewHeight = this.Height;
            int NewWidth = this.Width;

            if (NewHeight != 0 && NewWidth != 0)
            {
                this.SuspendLayout();
                //				Horizontal.Top = (int)(NewHeight*HorizontalPos);
                //				Vertical.Left =(int)(NewWidth*VerticalPos);
                //
                //				int CenterY=(Horizontal.Top+Horizontal.Height /2)-Center.Height/2;
                //				int CenterX=(Vertical.Left+Vertical.Width /2)-Center.Width /2;
                //
                //				Center.Location =new Point (CenterX,CenterY);


                //ReSize (0,0);
                ReSize2();
                OnResizing();

                if (_horizontal.Top < 15)
                {
                    _horizontal.Top = 0 - _horizontal.Height;
                    OnHideTop();
                }

                if (_vertical.Left < 15)
                {
                    _vertical.Left = 0 - _vertical.Width;
                    OnHideLeft();
                }


                _horizontal.Width = this.Width;
                _vertical.Height = this.Height;
                _horizontal.SendToBack();
                _vertical.SendToBack();
                _horizontal.BackColor = SystemColors.Control;
                _vertical.BackColor = SystemColors.Control;


                //this.SendToBack ();
                int RightLeft = _vertical.Left + _vertical.Width;
                int RightLowerTop = _horizontal.Top + _horizontal.Height;
                int RightWidth = this.Width - RightLeft;
                int LowerHeight = this.Height - RightLowerTop;
                int UpperHeight = _horizontal.Top;
                int LeftWidth = _vertical.Left;

                if (LowerRight != null)
                {
                    LowerRight.BringToFront();
                    LowerRight.SetBounds(RightLeft, RightLowerTop, RightWidth, LowerHeight);
                }
                if (UpperRight != null)
                {
                    UpperRight.BringToFront();
                    UpperRight.SetBounds(RightLeft, 0, RightWidth, UpperHeight);
                }


                if (LowerLeft != null)
                {
                    LowerLeft.BringToFront();
                    LowerLeft.SetBounds(0, RightLowerTop, LeftWidth, LowerHeight);
                }
                if (UpperLeft != null)
                {
                    UpperLeft.BringToFront();
                    UpperLeft.SetBounds(0, 0, LeftWidth, UpperHeight);
                }
                this.ResumeLayout(); //ggf
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            this.DoResize();
        }


        private void Vertical_Resize(object sender, EventArgs e)
        {
        }

        private void Horizontal_MouseDown(object sender, MouseEventArgs e)
        {
            _action = SizeAction.SizeH;
            _startPos = new Point(e.X, e.Y);
            _horizontal.BringToFront();
            _horizontal.BackColor = SystemColors.ControlDark;
            _firstTime = true;
        }

        private void Vertical_MouseDown(object sender, MouseEventArgs e)
        {
            _action = SizeAction.SizeV;
            _startPos = new Point(e.X, e.Y);
            _vertical.BringToFront();
            _vertical.BackColor = SystemColors.ControlDark;
            _firstTime = true;
        }

        private void Center_MouseDown(object sender, MouseEventArgs e)
        {
            _action = SizeAction.SizeA;
            _startPos = new Point(e.X, e.Y);
            _vertical.BringToFront();
            _horizontal.BringToFront();
            _vertical.BackColor = SystemColors.ControlDark;
            _horizontal.BackColor = SystemColors.ControlDark;
            _firstTime = true;
        }

        private void Horizontal_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = 0;
            int yDiff = _startPos.Y - e.Y;
            //	StartPos=new Point (e.X,e.Y);
            ReSize(xDiff, yDiff);

            _action = SizeAction.None;
            DoResize();
        }

        private void Vertical_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = _startPos.X - e.X;
            int yDiff = 0;
            //	StartPos=new Point (e.X,e.Y);
            ReSize(xDiff, yDiff);

            _action = SizeAction.None;
            DoResize();
            this.Refresh();
        }

        private void Center_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = _startPos.X - e.X;
            int yDiff = _startPos.Y - e.Y;
            //	StartPos=new Point (e.X,e.Y);
            ReSize(xDiff, yDiff);

            _action = SizeAction.None;
            DoResize();
        }

        private void Horizontal_MouseMove(object sender, MouseEventArgs e)
        {
            if (_action == SizeAction.SizeH && e.Button == MouseButtons.Left)
            {
                Point start;
                int x = e.X;
                int y = e.Y;


                if (y + _horizontal.Top > this.Height - 4)
                    y = this.Height - 4 - _horizontal.Top;
                if (y + _horizontal.Top < 0)
                    y = 0 - _horizontal.Top;

                if (!_firstTime)
                {
                    start = this.PointToScreen(this.Location);
                    start.Y += _prevPos.Y + _horizontal.Location.Y;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), Color.Black);
                }
                else
                    _firstTime = false;

                start = this.PointToScreen(this.Location);
                start.Y += y + _horizontal.Location.Y;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), Color.Black);

                _prevPos = new Point(x, y);
            }
        }

        private void Vertical_MouseMove(object sender, MouseEventArgs e)
        {
            if (_action == SizeAction.SizeV && e.Button == MouseButtons.Left)
            {
                Point start;
                int x = e.X;
                int y = e.Y;

                if (x + _vertical.Left > this.Width - 4)
                    x = this.Width - 4 - _vertical.Left;
                if (x + _vertical.Left < 0)
                    x = 0 - _vertical.Left;


                if (!_firstTime)
                {
                    start = this.PointToScreen(this.Location);
                    start.X += _prevPos.X + _vertical.Location.X;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);
                }
                else
                    _firstTime = false;

                start = this.PointToScreen(this.Location);
                start.X += x + _vertical.Location.X;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);

                _prevPos = new Point(x, y);
            }
        }

        private void Center_MouseMove(object sender, MouseEventArgs e)
        {
            if (_action == SizeAction.SizeA && e.Button == MouseButtons.Left)
            {
                Point start;
                int x = e.X - _center.Width / 2;
                int y = e.Y - _center.Height / 2;

                if (!_firstTime)
                {
                    start = this.PointToScreen(this.Location);
                    start.X += _prevPos.X + _vertical.Location.X;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);

                    start = this.PointToScreen(this.Location);
                    start.Y += _prevPos.Y + _horizontal.Location.Y;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), SystemColors.ControlDark);
                }
                else
                    _firstTime = false;

                start = this.PointToScreen(this.Location);
                start.X += x + _vertical.Location.X;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);

                start = this.PointToScreen(this.Location);
                start.Y += y + _horizontal.Location.Y;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), SystemColors.ControlDark);


                _prevPos = new Point(x, y);
            }
        }


        private void ReSize(int x, int y)
        {
            //if (x==0 && y==0)
            //	return;


            this.SuspendLayout();

            int xx = _vertical.Left - x;
            int yy = _horizontal.Top - y;

            if (xx < 0)
                xx = 0;

            if (yy < 0)
                yy = 0;

            if (yy > this.Height - _horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3)
                yy = this.Height - _horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3;


            if (xx > this.Width - _vertical.Width - SystemInformation.VerticalScrollBarWidth * 3)
                xx = this.Width - _vertical.Width - SystemInformation.VerticalScrollBarWidth * 3;

            if (xx != _vertical.Left)
                _vertical.Left = xx;

            if (yy != _horizontal.Top)
                _horizontal.Top = yy;


            int CenterY = (_horizontal.Top + _horizontal.Height / 2) - _center.Height / 2;
            int CenterX = (_vertical.Left + _vertical.Width / 2) - _center.Width / 2;

            _center.Location = new Point(CenterX, CenterY);
            this.ResumeLayout();
            this.Invalidate();

            try
            {
                if (UpperLeft != null)
                    UpperLeft.Refresh();
                if (UpperLeft != null)
                    UpperLeft.Refresh();
                if (UpperLeft != null)
                    UpperLeft.Refresh();
                if (UpperLeft != null)
                    UpperLeft.Refresh();
            }
            catch
            {
            }
            OnResizing();
            //DoResize();	
            //this.Refresh ();
        }

        private void ReSize2()
        {
            int xx = _vertical.Left;
            int yy = _horizontal.Top;

            if (xx < 0)
                xx = 0;

            if (yy < 0)
                yy = 0;

            if (yy > this.Height - _horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3)
            {
                yy = this.Height - _horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3;
                if (yy != _horizontal.Top)
                    _horizontal.Top = yy;
            }


            if (xx > this.Width - _vertical.Width - SystemInformation.VerticalScrollBarWidth * 3)
            {
                xx = this.Width - _vertical.Width - SystemInformation.VerticalScrollBarWidth * 3;
                if (xx != _vertical.Left)
                    _vertical.Left = xx;
            }

            int CenterY = (_horizontal.Top + _horizontal.Height / 2) - _center.Height / 2;
            int CenterX = (_vertical.Left + _vertical.Width / 2) - _center.Width / 2;

            _center.Location = new Point(CenterX, CenterY);
        }

        private void Center_DoubleClick(object sender, EventArgs e)
        {
            _horizontal.Top = -100;
            _vertical.Left = -100;
            DoResize();
        }

        private void Vertical_DoubleClick(object sender, EventArgs e)
        {
            _vertical.Left = -100;
            DoResize();
        }

        private void Horizontal_DoubleClick(object sender, EventArgs e)
        {
            _horizontal.Top = -100;
            DoResize();
        }

        /// <summary>
        /// Splits the view horiziontally.
        /// </summary>
        public void Split5050h()
        {
            _horizontal.Top = this.Height / 2;
            DoResize();
        }

        /// <summary>
        /// Splits teh view vertically.
        /// </summary>
        public void Split5050v()
        {
            _vertical.Left = this.Width / 2;
            DoResize();
        }

        public int SplitviewV
        {
            get { return _vertical.Left; }
            set
            {
                _vertical.Left = value;
                DoResize();
            }
        }

        public int SplitviewH
        {
            get { return _horizontal.Top; }
            set
            {
                _horizontal.Top = value;
                DoResize();
            }
        }


        public void ResetSplitview()
        {
            _vertical.Left = 0;
            _horizontal.Top = 0;
            DoResize();
        }

        /// <summary>
        /// Start dragging the horizontal splitter.
        /// </summary>
        public void InvokeMouseDownh()
        {
            IntPtr hwnd = _horizontal.Handle;
            NativeUser32Api.SendMessage(hwnd, (int)WindowMessage.WM_LBUTTONDOWN, 0, 0);
        }

        /// <summary>
        /// Start dragging the vertical splitter.
        /// </summary>
        public void InvokeMouseDownv()
        {
            IntPtr hwnd = _vertical.Handle;
            NativeUser32Api.SendMessage(hwnd, (int)WindowMessage.WM_LBUTTONDOWN, 0, 0);
        }

        private void SplitViewControl_Leave(object sender, EventArgs e)
        {
        }

        private void SplitViewControl_Enter(object sender, EventArgs e)
        {
        }


        private void SplitViewControl_VisibleChanged(object sender, EventArgs e)
        {
        }
    }
}