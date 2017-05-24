﻿// ***********************************************************************
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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NUnit.UiKit.Controls
{
    public enum TestProgressBarStatus
    {
        Success = 0,
        Warning = 1,
        Failure = 2
    }

    public class NUnitProgressBar : ProgressBar
    {
        public readonly static Color[][] BrushColors =
        {
              new Color[] { Color.FromArgb(32, 205, 32), Color.FromArgb(16, 64, 16) },  // Success
              new Color[] { Color.FromArgb(255, 255, 0), Color.FromArgb(255, 192, 0) }, // Warning
              new Color[] { Color.FromArgb(255, 0, 0), Color.FromArgb(150, 0, 0) }      // Failure
        };

        private Brush _brush;

        public NUnitProgressBar()
        {
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            _status = TestProgressBarStatus.Success;
            _brush = CreateBrush(_status);
        }

        #region Properties

        private TestProgressBarStatus _status = TestProgressBarStatus.Success;
        public TestProgressBarStatus Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;

                    if (_brush != null)
                        _brush.Dispose();
                    _brush = CreateBrush(value);
                }
            }
        }

        #endregion

        #region Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = this.ClientRectangle;
            rec.Inflate(-2, -2);
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rec);
            rec.Inflate(-1, -1);
            rec.Width = (int)(rec.Width * ((double)Value / Maximum));
            e.Graphics.FillRectangle(_brush, rec); //2, 2, rec.Width, rec.Height);
        }

        private Brush CreateBrush(TestProgressBarStatus status)
        {
            Color[] colors = BrushColors[(int)status];
            return new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, this.ClientSize.Height - 3),
                colors[0],
                colors[1]);
        }

        #endregion
    }
}
