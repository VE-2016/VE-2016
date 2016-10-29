// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using AIMS.Libraries.CodeEditor.Util;

namespace AIMS.Libraries.CodeEditor.WinForms.CompletionWindow
{
    public interface IDeclarationViewWindow
    {
        string Description
        {
            get;
            set;
        }
        void ShowDeclarationViewWindow();
        void CloseDeclarationViewWindow();
    }

    public class DeclarationViewWindow : Form, IDeclarationViewWindow
    {
        private string _description = String.Empty;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                if (value == null && Visible)
                {
                    Visible = false;
                }
                else if (value != null)
                {
                    if (!Visible) ShowDeclarationViewWindow();
                    Refresh();
                }
            }
        }

        public bool HideOnClick;

        public DeclarationViewWindow(Form parent)
        {
            SetStyle(ControlStyles.Selectable, false);
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            Owner = parent;
            ShowInTaskbar = false;
            Size = new Size(0, 0);
            base.CreateHandle();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                AbstractCompletionWindow.AddShadowToWindow(p);
                return p;
            }
        }

        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (HideOnClick) Hide();
        }

        public void ShowDeclarationViewWindow()
        {
            Show();
        }

        public void CloseDeclarationViewWindow()
        {
            Close();
            Dispose();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_description != null && _description.Length > 0)
            {
                TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics, Font, null, _description);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
        }
    }
}
