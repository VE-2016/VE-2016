
using System;
using System.Drawing;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.Syntax;

namespace AIMS.Libraries.CodeEditor.WinForms.CompletionWindow
{
    /// <summary>
    /// Description of AbstractCompletionWindow.
    /// </summary>
    public abstract class AbstractCompletionWindow : System.Windows.Forms.Form
    {
        protected CodeEditorControl control;
        protected Size drawingSize;
        private Rectangle _workingScreen;
        private Form _parentForm;

        protected AbstractCompletionWindow(Form parentForm, CodeEditorControl control)
        {
            if (parentForm == null)
                _workingScreen = Screen.GetWorkingArea(control);
            else
                _workingScreen = Screen.GetWorkingArea(parentForm);
            //			SetStyle(ControlStyles.Selectable, false);
            _parentForm = parentForm;
            this.control = control;


            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            MinimumSize = new Size(1, 1);
            Size = new Size(1, 1);
            SetLocation();
        }

        public virtual void SetLocation()
        {
            EditViewControl textArea = control.ActiveViewControl;
            TextPoint caretPos = textArea.Caret.Position;
            Owner = _parentForm;


            Point pos = control.ActiveViewControl.GetDrawingPos(new TextPoint(caretPos.X + 2, caretPos.Y + 1));

            Point location = control.PointToScreen(pos);
            Rectangle bounds = new Rectangle(location, drawingSize);

            if (!_workingScreen.Contains(bounds))
            {
                if (bounds.Right > _workingScreen.Right)
                {
                    bounds.X = _workingScreen.Right - bounds.Width;
                }
                if (bounds.Left < _workingScreen.Left)
                {
                    bounds.X = _workingScreen.Left;
                }
                if (bounds.Top < _workingScreen.Top)
                {
                    bounds.Y = _workingScreen.Top;
                }
                if (bounds.Bottom > _workingScreen.Bottom)
                {
                    bounds.Y = bounds.Y - bounds.Height - control.ActiveViewControl.Font.Height;
                    if (bounds.Bottom > _workingScreen.Bottom)
                    {
                        bounds.Y = _workingScreen.Bottom - bounds.Height;
                    }
                }
            }
            Bounds = bounds;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                AddShadowToWindow(p);
                return p;
            }
        }

        private static int s_shadowStatus;

        /// <summary>
        /// Adds a shadow to the create params if it is supported by the operating system.
        /// </summary>
        public static void AddShadowToWindow(CreateParams createParams)
        {
            if (s_shadowStatus == 0)
            {
                // Test OS version
                s_shadowStatus = -1; // shadow not supported
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    Version ver = Environment.OSVersion.Version;
                    if (ver.Major > 5 || ver.Major == 5 && ver.Minor >= 1)
                    {
                        s_shadowStatus = 1;
                    }
                }
            }
            if (s_shadowStatus == 1)
            {
                createParams.ClassStyle |= 0x00020000; // set CS_DROPSHADOW
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        protected void ShowCompletionWindow()
        {
            Owner = _parentForm;
            Enabled = true;
            this.Show();
            control.Focus();

            if (_parentForm != null)
            {
                _parentForm.LocationChanged += new EventHandler(this.ParentFormLocationChanged);
            }

            //control.ActiveViewControl.VScrollBar.ValueChanged     += new EventHandler(ParentFormLocationChanged);
            //control.ActiveViewControl.HScrollBar.ValueChanged     += new EventHandler(ParentFormLocationChanged);
            //control.ActiveViewControl.TextArea.DoProcessDialogKey += new DialogKeyProcessor(ProcessTextAreaKey);
            control.ActiveViewControl.Caret.Change += new EventHandler(CaretOffsetChanged);
            control.ActiveViewControl.LostFocus += new EventHandler(this.TextEditorLostFocus);
            control.Resize += new EventHandler(ParentFormLocationChanged);

            foreach (Control c in Controls)
            {
                c.MouseMove += ControlMouseMove;
            }
        }

        private void ParentFormLocationChanged(object sender, EventArgs e)
        {
            SetLocation();
        }

        public virtual bool ProcessKeyEvent(char ch)
        {
            return false;
        }

        public virtual bool ProcessTextAreaKey(Keys keyData)
        {
            if (!Visible)
            {
                return false;
            }
            switch (keyData)
            {
                case Keys.Escape:
                    Close();
                    return true;
            }
            return false;
        }

        protected virtual void CaretOffsetChanged(object sender, EventArgs e)
        {
            SetLocation();
        }

        protected void TextEditorLostFocus(object sender, EventArgs e)
        {
            if (!control.ActiveViewControl.Focused && !this.ContainsFocus)
            {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // take out the inserted methods
            if (_parentForm != null)
                _parentForm.LocationChanged -= new EventHandler(ParentFormLocationChanged);

            foreach (Control c in Controls)
            {
                c.MouseMove -= ControlMouseMove;
            }

            //if (control.ActiveViewControl.VScrollBar != null) {
            //	control.ActiveViewControl.VScrollBar.ValueChanged -= new EventHandler(ParentFormLocationChanged);
            //}
            //if (control.ActiveViewControl.HScrollBar != null) {
            //	control.ActiveViewControl.HScrollBar.ValueChanged -= new EventHandler(ParentFormLocationChanged);
            //}

            control.ActiveViewControl.LostFocus -= new EventHandler(this.TextEditorLostFocus);
            control.ActiveViewControl.Caret.Change -= new EventHandler(CaretOffsetChanged);
            //control.ActiveViewControl.TextArea.DoProcessDialogKey -= new DialogKeyProcessor(ProcessTextAreaKey);
            control.Resize -= new EventHandler(ParentFormLocationChanged);
            Dispose();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            ControlMouseMove(this, e);
        }

        /// <summary>
        /// Invoked when the mouse moves over this form or any child control.
        /// Shows the mouse cursor on the text area if it has been hidden.
        /// </summary>
        /// <remarks>
        /// Derived classes should attach this handler to the MouseMove event
        /// of all created controls which are not added to the Controls
        /// collection.
        /// </remarks>
        protected void ControlMouseMove(object sender, MouseEventArgs e)
        {
            //control.ActiveViewControl.TextArea.ShowHiddenCursor(false);
        }
    }
}
