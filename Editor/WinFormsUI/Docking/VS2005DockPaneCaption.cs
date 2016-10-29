using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal class VS2005DockPaneCaption : DockPaneCaptionBase
    {
        private sealed class InertButton : InertButtonBase
        {
            private Bitmap _image,_imageAutoHide;

            public InertButton(VS2005DockPaneCaption dockPaneCaption, Bitmap image, Bitmap imageAutoHide)
                : base()
            {
                _dockPaneCaption = dockPaneCaption;
                _image = image;
                _imageAutoHide = imageAutoHide;
                RefreshChanges();
            }

            private VS2005DockPaneCaption _dockPaneCaption;
            private VS2005DockPaneCaption DockPaneCaption
            {
                get { return _dockPaneCaption; }
            }

            public bool IsAutoHide
            {
                get { return DockPaneCaption.DockPane.IsAutoHide; }
            }

            public override Bitmap Image
            {
                get { return IsAutoHide ? _imageAutoHide : _image; }
            }

            protected override void OnRefreshChanges()
            {
                if (DockPaneCaption.DockPane.DockPanel != null)
                {
                    if (DockPaneCaption.TextColor != ForeColor)
                    {
                        ForeColor = DockPaneCaption.TextColor;
                        Invalidate();
                    }
                }
            }
        }

        #region consts
        private const int _TextGapTop = 2;
        private const int _TextGapBottom = 0;
        private const int _TextGapLeft = 3;
        private const int _TextGapRight = 3;
        private const int _ButtonGapTop = 2;
        private const int _ButtonGapBottom = 1;
        private const int _ButtonGapBetween = 1;
        private const int _ButtonGapLeft = 1;
        private const int _ButtonGapRight = 2;
        #endregion

        private static Bitmap s_imageButtonClose;
        private static Bitmap ImageButtonClose
        {
            get
            {
                if (s_imageButtonClose == null)
                    s_imageButtonClose = Resources.DockPane_Close;

                return s_imageButtonClose;
            }
        }

        private InertButton _buttonClose;
        private InertButton ButtonClose
        {
            get
            {
                if (_buttonClose == null)
                {
                    _buttonClose = new InertButton(this, ImageButtonClose, ImageButtonClose);
                    _toolTip.SetToolTip(_buttonClose, ToolTipClose);
                    _buttonClose.Click += new EventHandler(Close_Click);
                    Controls.Add(_buttonClose);
                }

                return _buttonClose;
            }
        }

        private static Bitmap s_imageButtonAutoHide;
        private static Bitmap ImageButtonAutoHide
        {
            get
            {
                if (s_imageButtonAutoHide == null)
                    s_imageButtonAutoHide = Resources.DockPane_AutoHide;

                return s_imageButtonAutoHide;
            }
        }

        private static Bitmap s_imageButtonDock;
        private static Bitmap ImageButtonDock
        {
            get
            {
                if (s_imageButtonDock == null)
                    s_imageButtonDock = Resources.DockPane_Dock;

                return s_imageButtonDock;
            }
        }

        private InertButton _buttonAutoHide;
        private InertButton ButtonAutoHide
        {
            get
            {
                if (_buttonAutoHide == null)
                {
                    _buttonAutoHide = new InertButton(this, ImageButtonDock, ImageButtonAutoHide);
                    _toolTip.SetToolTip(_buttonAutoHide, ToolTipAutoHide);
                    _buttonAutoHide.Click += new EventHandler(AutoHide_Click);
                    Controls.Add(_buttonAutoHide);
                }

                return _buttonAutoHide;
            }
        }

        private static Bitmap s_imageButtonOptions;
        private static Bitmap ImageButtonOptions
        {
            get
            {
                if (s_imageButtonOptions == null)
                    s_imageButtonOptions = Resources.DockPane_Option;

                return s_imageButtonOptions;
            }
        }

        private InertButton _buttonOptions;
        private InertButton ButtonOptions
        {
            get
            {
                if (_buttonOptions == null)
                {
                    _buttonOptions = new InertButton(this, ImageButtonOptions, ImageButtonOptions);
                    _toolTip.SetToolTip(_buttonOptions, ToolTipOptions);
                    _buttonOptions.Click += new EventHandler(Options_Click);
                    Controls.Add(_buttonOptions);
                }
                return _buttonOptions;
            }
        }

        private IContainer _components;
        private IContainer Components
        {
            get { return _components; }
        }

        private ToolTip _toolTip;

        public VS2005DockPaneCaption(DockPane pane) : base(pane)
        {
            SuspendLayout();

            _components = new Container();
            _toolTip = new ToolTip(Components);

            ResumeLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Components.Dispose();
            base.Dispose(disposing);
        }

        private static int TextGapTop
        {
            get { return _TextGapTop; }
        }

        public Font TextFont
        {
            get { return DockPane.DockPanel.Skin.DockPaneStripSkin.TextFont; }
        }

        private static int TextGapBottom
        {
            get { return _TextGapBottom; }
        }

        private static int TextGapLeft
        {
            get { return _TextGapLeft; }
        }

        private static int TextGapRight
        {
            get { return _TextGapRight; }
        }

        private static int ButtonGapTop
        {
            get { return _ButtonGapTop; }
        }

        private static int ButtonGapBottom
        {
            get { return _ButtonGapBottom; }
        }

        private static int ButtonGapLeft
        {
            get { return _ButtonGapLeft; }
        }

        private static int ButtonGapRight
        {
            get { return _ButtonGapRight; }
        }

        private static int ButtonGapBetween
        {
            get { return _ButtonGapBetween; }
        }

        private static string s_toolTipClose;
        private static string ToolTipClose
        {
            get
            {
                if (s_toolTipClose == null)
                    s_toolTipClose = Strings.DockPaneCaption_ToolTipClose;
                return s_toolTipClose;
            }
        }

        private static string s_toolTipOptions;
        private static string ToolTipOptions
        {
            get
            {
                if (s_toolTipOptions == null)
                    s_toolTipOptions = Strings.DockPaneCaption_ToolTipOptions;

                return s_toolTipOptions;
            }
        }

        private static string s_toolTipAutoHide;
        private static string ToolTipAutoHide
        {
            get
            {
                if (s_toolTipAutoHide == null)
                    s_toolTipAutoHide = Strings.DockPaneCaption_ToolTipAutoHide;
                return s_toolTipAutoHide;
            }
        }

        private static Blend s_activeBackColorGradientBlend;
        private static Blend ActiveBackColorGradientBlend
        {
            get
            {
                if (s_activeBackColorGradientBlend == null)
                {
                    Blend blend = new Blend(2);

                    blend.Factors = new float[] { 0.5F, 1.0F };
                    blend.Positions = new float[] { 0.0F, 1.0F };
                    s_activeBackColorGradientBlend = blend;
                }

                return s_activeBackColorGradientBlend;
            }
        }

        private Color TextColor
        {
            get
            {
                if (DockPane.IsActivated)
                    return DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor;
                else
                    return DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor;
            }
        }

        private static TextFormatFlags s_textFormat =
            TextFormatFlags.SingleLine |
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.VerticalCenter;
        private TextFormatFlags TextFormat
        {
            get
            {
                if (RightToLeft == RightToLeft.No)
                    return s_textFormat;
                else
                    return s_textFormat | TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }
        }

        protected internal override int MeasureHeight()
        {
            int height = TextFont.Height + TextGapTop + TextGapBottom;

            if (height < ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom)
                height = ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom;

            return height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawCaption(e.Graphics);
        }

        private void DrawCaption(Graphics g)
        {
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
                return;

            if (DockPane.IsActivated)
            {
                Color startColor = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.StartColor;
                Color endColor = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.EndColor;
                LinearGradientMode gradientMode = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.LinearGradientMode;
                using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, startColor, endColor, gradientMode))
                {
                    brush.Blend = ActiveBackColorGradientBlend;
                    g.FillRectangle(brush, ClientRectangle);
                }
            }
            else
            {
                Color startColor = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.StartColor;
                Color endColor = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.EndColor;
                LinearGradientMode gradientMode = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.LinearGradientMode;
                using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, startColor, endColor, gradientMode))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
            }

            Rectangle rectCaption = ClientRectangle;

            Rectangle rectCaptionText = rectCaption;
            rectCaptionText.X += TextGapLeft;
            rectCaptionText.Width -= TextGapLeft + TextGapRight;
            rectCaptionText.Width -= ButtonGapLeft + ButtonClose.Width + ButtonGapRight;
            if (ShouldShowAutoHideButton)
                rectCaptionText.Width -= ButtonAutoHide.Width + ButtonGapBetween;
            if (HasTabPageContextMenu)
                rectCaptionText.Width -= ButtonOptions.Width + ButtonGapBetween;
            rectCaptionText.Y += TextGapTop;
            rectCaptionText.Height -= TextGapTop + TextGapBottom;

            Color colorText;
            if (DockPane.IsActivated)
                colorText = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.ActiveCaptionGradient.TextColor;
            else
                colorText = DockPane.DockPanel.Skin.DockPaneStripSkin.ToolWindowGradient.InactiveCaptionGradient.TextColor;

            TextRenderer.DrawText(g, DockPane.CaptionText, TextFont, DrawHelper.RtlTransform(this, rectCaptionText), colorText, TextFormat);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            SetButtonsPosition();
            base.OnLayout(levent);
        }

        protected override void OnRefreshChanges()
        {
            SetButtons();
            Invalidate();
        }

        private bool CloseButtonEnabled
        {
            get { return (DockPane.ActiveContent != null) ? DockPane.ActiveContent.DockHandler.CloseButton : false; }
        }

        /// <summary>
        /// Determines whether the close button is visible on the content
        /// </summary>
        private bool CloseButtonVisible
        {
            get { return (DockPane.ActiveContent != null) ? DockPane.ActiveContent.DockHandler.CloseButtonVisible : false; }
        }

        private bool ShouldShowAutoHideButton
        {
            get { return !DockPane.IsFloat; }
        }

        private void SetButtons()
        {
            ButtonClose.Enabled = CloseButtonEnabled;
            ButtonClose.Visible = CloseButtonVisible;
            ButtonAutoHide.Visible = ShouldShowAutoHideButton;
            ButtonOptions.Visible = HasTabPageContextMenu;
            ButtonClose.RefreshChanges();
            ButtonAutoHide.RefreshChanges();
            ButtonOptions.RefreshChanges();

            SetButtonsPosition();
        }

        private void SetButtonsPosition()
        {
            // set the size and location for close and auto-hide buttons
            Rectangle rectCaption = ClientRectangle;
            int buttonWidth = ButtonClose.Image.Width;
            int buttonHeight = ButtonClose.Image.Height;
            int height = rectCaption.Height - ButtonGapTop - ButtonGapBottom;
            if (buttonHeight < height)
            {
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);
            int x = rectCaption.X + rectCaption.Width - 1 - ButtonGapRight - _buttonClose.Width;
            int y = rectCaption.Y + ButtonGapTop;
            Point point = new Point(x, y);
            ButtonClose.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));

            // If the close button is not visible draw the auto hide button overtop.
            // Otherwise it is drawn to the left of the close button.
            if (CloseButtonVisible)
                point.Offset(-(buttonWidth + ButtonGapBetween), 0);

            ButtonAutoHide.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            if (ShouldShowAutoHideButton)
                point.Offset(-(buttonWidth + ButtonGapBetween), 0);
            ButtonOptions.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
        }

        private void Close_Click(object sender, EventArgs e)
        {
            DockPane.CloseActiveContent();
        }

        private void AutoHide_Click(object sender, EventArgs e)
        {
            DockPane.DockState = DockHelper.ToggleAutoHideState(DockPane.DockState);
            if (DockHelper.IsDockStateAutoHide(DockPane.DockState))
            {
                DockPane.DockPanel.ActiveAutoHideContent = null;
                DockPane.NestedDockingStatus.NestedPanes.SwitchPaneWithFirstChild(DockPane);
            }
        }

        private void Options_Click(object sender, EventArgs e)
        {
            ShowTabPageContextMenu(PointToClient(Control.MousePosition));
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            PerformLayout();
        }
    }
}
