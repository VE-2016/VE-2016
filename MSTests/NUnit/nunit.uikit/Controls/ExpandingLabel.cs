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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace NUnit.UiKit.Controls
{
    /// <summary>
    /// A special type of label which can display a tooltip-like
    /// window to show the full extent of any text which doesn't 
    /// fit. The window may be placed directly over the label
    /// or immediately beneath it and will expand to fit in
    /// a horizontal, vertical or both directions as needed.
    /// </summary>
    public class ExpandingLabel : System.Windows.Forms.Label
    {
        #region Instance Variables

        /// <summary>
        /// Our window for displaying expanded text
        /// </summary>
        private TipWindow _tipWindow;

        /// <summary>
        /// Direction of expansion
        /// </summary>
        private TipWindow.ExpansionStyle _expansion = TipWindow.ExpansionStyle.Horizontal;

        /// <summary>
        /// True if tipWindow may overlay the label
        /// </summary>
        private bool _overlay = true;

        /// <summary>
        /// Time in milliseconds that the tip window
        /// will remain displayed.
        /// </summary>
        private int _autoCloseDelay = 0;

        /// <summary>
        /// Time in milliseconds that the window stays
        /// open after the mouse leaves the control.
        /// </summary>
        private int _mouseLeaveDelay = 300;

        /// <summary>
        /// If true, a context menu with Copy is displayed which
        /// allows copying contents to the clipboard.
        /// </summary>
        private bool _copySupported = false;

        #endregion

        #region Properties

        [Browsable(false)]
        public bool Expanded
        {
            get { return _tipWindow != null && _tipWindow.Visible; }
        }

        [Category("Behavior"), DefaultValue(TipWindow.ExpansionStyle.Horizontal)]
        public TipWindow.ExpansionStyle Expansion
        {
            get { return _expansion; }
            set { _expansion = value; }
        }

        [Category("Behavior"), DefaultValue(true)]
        [Description("Indicates whether the tip window should overlay the label")]
        public bool Overlay
        {
            get { return _overlay; }
            set { _overlay = value; }
        }

        /// <summary>
        /// Time in milliseconds that the tip window
        /// will remain displayed.
        /// </summary>
        [Category("Behavior"), DefaultValue(0)]
        [Description("Time in milliseconds that the tip is displayed. Zero indicates no automatic timeout.")]
        public int AutoCloseDelay
        {
            get { return _autoCloseDelay; }
            set { _autoCloseDelay = value; }
        }

        /// <summary>
        /// Time in milliseconds that the window stays
        /// open after the mouse leaves the control.
        /// Reentering the control resets this.
        /// </summary>
        [Category("Behavior"), DefaultValue(300)]
        [Description("Time in milliseconds that the tip is displayed after the mouse levaes the control")]
        public int MouseLeaveDelay
        {
            get { return _mouseLeaveDelay; }
            set { _mouseLeaveDelay = value; }
        }

        [Category("Behavior"), DefaultValue(false)]
        [Description("If true, displays a context menu with Copy")]
        public bool CopySupported
        {
            get { return _copySupported; }
            set
            {
                _copySupported = value;
                if (_copySupported)
                    base.ContextMenu = null;
            }
        }

        /// <summary>
        /// Override Text property to set up copy menu if
        /// the val is non-empty.
        /// </summary>
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;

                if (_copySupported)
                {
                    if (value == null || value == string.Empty)
                    {
                        if (this.ContextMenu != null)
                        {
                            this.ContextMenu.Dispose();
                            this.ContextMenu = null;
                        }
                    }
                    else
                    {
                        this.ContextMenu = new System.Windows.Forms.ContextMenu();
                        MenuItem copyMenuItem = new MenuItem("Copy", new EventHandler(CopyToClipboard));
                        this.ContextMenu.MenuItems.Add(copyMenuItem);
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void Expand()
        {
            if (!Expanded)
            {
                _tipWindow = new TipWindow(this);
                _tipWindow.Closed += new EventHandler(tipWindow_Closed);
                _tipWindow.Expansion = this.Expansion;
                _tipWindow.Overlay = this.Overlay;
                _tipWindow.AutoCloseDelay = this.AutoCloseDelay;
                _tipWindow.MouseLeaveDelay = this.MouseLeaveDelay;
                _tipWindow.WantClicks = this.CopySupported;
                _tipWindow.Show();
            }
        }

        public void Unexpand()
        {
            if (Expanded)
            {
                _tipWindow.Close();
            }
        }

        #endregion

        #region Event Handlers

        private void tipWindow_Closed(object sender, EventArgs e)
        {
            _tipWindow = null;
        }

        protected override void OnMouseHover(System.EventArgs e)
        {
            Graphics g = Graphics.FromHwnd(Handle);
            SizeF sizeNeeded = g.MeasureString(Text, Font);
            bool expansionNeeded =
                Width < (int)sizeNeeded.Width ||
                Height < (int)sizeNeeded.Height;

            if (expansionNeeded) Expand();
        }

        /// <summary>
        /// Copy contents to clipboard
        /// </summary>
        private void CopyToClipboard(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.Text);
        }

        #endregion
    }
}
