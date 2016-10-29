// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NUnit.UiKit.Controls
{
    public class TipWindow : Form
    {
        /// <summary>
        /// Direction in which to expand
        /// </summary>
        public enum ExpansionStyle
        {
            Horizontal,
            Vertical,
            Both
        }

        #region Instance Variables

        /// <summary>
        /// Text we are displaying
        /// </summary>
        private string _tipText;

        /// <summary>
        /// The control for which we are showing expanded text
        /// </summary>
        private Control _control;

        /// <summary>
        /// Rectangle representing bounds to overlay. For a listbox, this
        /// is a single item rectangle. For other controls, it is usually
        /// the entire client area.
        /// </summary>
        private Rectangle _itemBounds;

        /// <summary>
        /// True if we may overlay control or item
        /// </summary>
        private bool _overlay = true;

        /// <summary>
        /// Directions we are allowed to expand
        /// </summary>
        private ExpansionStyle _expansion = ExpansionStyle.Horizontal;

        /// <summary>
        /// Time before automatically closing
        /// </summary>
        private int _autoCloseDelay = 0;

        /// <summary>
        /// Timer used for auto-close
        /// </summary>
        private System.Windows.Forms.Timer _autoCloseTimer;

        /// <summary>
        /// Time to wait for after mouse leaves
        /// the window or the label before closing.
        /// </summary>
        private int _mouseLeaveDelay = 300;

        /// <summary>
        /// Timer used for mouse leave delay
        /// </summary>
        private System.Windows.Forms.Timer _mouseLeaveTimer;

        /// <summary>
        /// Rectangle used to display text
        /// </summary>
        private Rectangle _textRect;

        /// <summary>
        /// Indicates whether any clicks should be passed to the underlying control
        /// </summary>
        private bool _wantClicks = false;

        #endregion

        #region Construction and Initialization

        public TipWindow(Control control)
        {
            InitializeComponent();
            InitControl(control);

            // Note: This causes an error if called on a listbox
            // with no item as yet selected, therefore, it is handled
            // differently in the constructor for a listbox.
            _tipText = control.Text;
        }

        public TipWindow(ListBox listbox, int index)
        {
            InitializeComponent();
            InitControl(listbox);

            _itemBounds = listbox.GetItemRectangle(index);
            _tipText = listbox.Items[index].ToString();
        }

        private void InitControl(Control control)
        {
            _control = control;
            this.Owner = control.FindForm();
            _itemBounds = control.ClientRectangle;

            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.LightYellow;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            this.Font = control.Font;
        }

        private void InitializeComponent()
        {
            // 
            // TipWindow
            // 
            this.BackColor = System.Drawing.Color.LightYellow;
            this.ClientSize = new System.Drawing.Size(292, 268);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TipWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            // At this point, further changes to the properties
            // of the label will have no effect on the tip.
            Point origin = _control.Parent.PointToScreen(_control.Location);
            origin.Offset(_itemBounds.Left, _itemBounds.Top);
            if (!_overlay) origin.Offset(0, _itemBounds.Height);
            this.Location = origin;

            Graphics g = Graphics.FromHwnd(Handle);
            Screen screen = Screen.FromControl(_control);
            SizeF layoutArea = new SizeF(screen.WorkingArea.Width - 40, screen.WorkingArea.Height - 40);
            if (_expansion == ExpansionStyle.Vertical)
                layoutArea.Width = _itemBounds.Width;
            else if (_expansion == ExpansionStyle.Horizontal)
                layoutArea.Height = _itemBounds.Height;

            Size sizeNeeded = Size.Ceiling(g.MeasureString(_tipText, Font, layoutArea));

            this.ClientSize = sizeNeeded;
            this.Size = sizeNeeded + new Size(2, 2);
            _textRect = new Rectangle(1, 1, sizeNeeded.Width, sizeNeeded.Height);

            // Catch mouse leaving the control
            _control.MouseLeave += new EventHandler(control_MouseLeave);

            // Catch the form that holds the control closing
            _control.FindForm().Closed += new EventHandler(control_FormClosed);

            if (this.Right > screen.WorkingArea.Right)
            {
                this.Left = Math.Max(
                    screen.WorkingArea.Right - this.Width - 20,
                    screen.WorkingArea.Left + 20);
            }

            if (this.Bottom > screen.WorkingArea.Bottom - 20)
            {
                if (_overlay)
                    this.Top = Math.Max(
                        screen.WorkingArea.Bottom - this.Height - 20,
                        screen.WorkingArea.Top + 20);

                if (this.Bottom > screen.WorkingArea.Bottom - 20)
                    this.Height = screen.WorkingArea.Bottom - 20 - this.Top;
            }

            if (_autoCloseDelay > 0)
            {
                _autoCloseTimer = new System.Windows.Forms.Timer();
                _autoCloseTimer.Interval = _autoCloseDelay;
                _autoCloseTimer.Tick += new EventHandler(OnAutoClose);
                _autoCloseTimer.Start();
            }
        }

        #endregion

        #region Properties

        public bool Overlay
        {
            get { return _overlay; }
            set { _overlay = value; }
        }

        public ExpansionStyle Expansion
        {
            get { return _expansion; }
            set { _expansion = value; }
        }

        public int AutoCloseDelay
        {
            get { return _autoCloseDelay; }
            set { _autoCloseDelay = value; }
        }

        public int MouseLeaveDelay
        {
            get { return _mouseLeaveDelay; }
            set { _mouseLeaveDelay = value; }
        }

        public string TipText
        {
            get { return _tipText; }
            set { _tipText = value; }
        }

        public Rectangle ItemBounds
        {
            get { return _itemBounds; }
            set { _itemBounds = value; }
        }

        public bool WantClicks
        {
            get { return _wantClicks; }
            set { _wantClicks = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle outlineRect = this.ClientRectangle;
            outlineRect.Inflate(-1, -1);
            g.DrawRectangle(Pens.Black, outlineRect);
            g.DrawString(_tipText, Font, Brushes.Black, _textRect);
        }

        private void OnAutoClose(object sender, System.EventArgs e)
        {
            this.Close();
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            if (_mouseLeaveTimer != null)
            {
                _mouseLeaveTimer.Stop();
                _mouseLeaveTimer.Dispose();
                System.Diagnostics.Debug.WriteLine("Entered TipWindow - stopped mouseLeaveTimer");
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            if (_mouseLeaveDelay > 0)
            {
                _mouseLeaveTimer = new System.Windows.Forms.Timer();
                _mouseLeaveTimer.Interval = _mouseLeaveDelay;
                _mouseLeaveTimer.Tick += new EventHandler(OnAutoClose);
                _mouseLeaveTimer.Start();
                System.Diagnostics.Debug.WriteLine("Left TipWindow - started mouseLeaveTimer");
            }
        }

        /// <summary>
        /// The form our label is on closed, so we should. 
        /// </summary>
        private void control_FormClosed(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// The mouse left the label. We ignore if we are
        /// overlaying the label but otherwise start a
        /// delay for closing the window
        /// </summary>
        private void control_MouseLeave(object sender, System.EventArgs e)
        {
            if (_mouseLeaveDelay > 0 && !_overlay)
            {
                _mouseLeaveTimer = new System.Windows.Forms.Timer();
                _mouseLeaveTimer.Interval = _mouseLeaveDelay;
                _mouseLeaveTimer.Tick += new EventHandler(OnAutoClose);
                _mouseLeaveTimer.Start();
                System.Diagnostics.Debug.WriteLine("Left Control - started mouseLeaveTimer");
            }
        }

        #endregion

        [DllImport("user32.dll")]
        private static extern uint SendMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wparam,
            IntPtr lparam
            );

        protected override void WndProc(ref Message m)
        {
            uint WM_LBUTTONDOWN = 0x201;
            uint WM_RBUTTONDOWN = 0x204;
            uint WM_MBUTTONDOWN = 0x207;

            if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN)
            {
                if (m.Msg != WM_LBUTTONDOWN)
                    this.Close();
                SendMessage(_control.Handle, m.Msg, m.WParam, m.LParam);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
