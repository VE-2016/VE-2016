
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


namespace AIMS.Libraries.Forms.Docking
{
    internal class DockAreasEditor : UITypeEditor
    {
        private class DockAreasEditorControl : System.Windows.Forms.UserControl
        {
            private IWindowsFormsEditorService _edSvc;
            private CheckBox _checkBoxFloat;
            private CheckBox _checkBoxDockLeft;
            private CheckBox _checkBoxDockRight;
            private CheckBox _checkBoxDockTop;
            private CheckBox _checkBoxDockBottom;
            private CheckBox _checkBoxDockFill;
            private DockAreas _oldDockAreas;

            public DockAreas DockAreas
            {
                get
                {
                    DockAreas dockAreas = 0;
                    if (_checkBoxFloat.Checked)
                        dockAreas |= DockAreas.Float;
                    if (_checkBoxDockLeft.Checked)
                        dockAreas |= DockAreas.DockLeft;
                    if (_checkBoxDockRight.Checked)
                        dockAreas |= DockAreas.DockRight;
                    if (_checkBoxDockTop.Checked)
                        dockAreas |= DockAreas.DockTop;
                    if (_checkBoxDockBottom.Checked)
                        dockAreas |= DockAreas.DockBottom;
                    if (_checkBoxDockFill.Checked)
                        dockAreas |= DockAreas.Document;

                    if (dockAreas == 0)
                        return _oldDockAreas;
                    else
                        return dockAreas;
                }
            }

            public DockAreasEditorControl()
            {
                _checkBoxFloat = new CheckBox();
                _checkBoxDockLeft = new CheckBox();
                _checkBoxDockRight = new CheckBox();
                _checkBoxDockTop = new CheckBox();
                _checkBoxDockBottom = new CheckBox();
                _checkBoxDockFill = new CheckBox();

                SuspendLayout();

                _checkBoxFloat.Appearance = Appearance.Button;
                _checkBoxFloat.Dock = DockStyle.Top;
                _checkBoxFloat.Height = 24;
                _checkBoxFloat.Text = "(Float)";
                _checkBoxFloat.TextAlign = ContentAlignment.MiddleCenter;
                _checkBoxFloat.FlatStyle = FlatStyle.System;

                _checkBoxDockLeft.Appearance = System.Windows.Forms.Appearance.Button;
                _checkBoxDockLeft.Dock = System.Windows.Forms.DockStyle.Left;
                _checkBoxDockLeft.Width = 24;
                _checkBoxDockLeft.FlatStyle = FlatStyle.System;

                _checkBoxDockRight.Appearance = System.Windows.Forms.Appearance.Button;
                _checkBoxDockRight.Dock = System.Windows.Forms.DockStyle.Right;
                _checkBoxDockRight.Width = 24;
                _checkBoxDockRight.FlatStyle = FlatStyle.System;

                _checkBoxDockTop.Appearance = System.Windows.Forms.Appearance.Button;
                _checkBoxDockTop.Dock = System.Windows.Forms.DockStyle.Top;
                _checkBoxDockTop.Height = 24;
                _checkBoxDockTop.FlatStyle = FlatStyle.System;

                _checkBoxDockBottom.Appearance = System.Windows.Forms.Appearance.Button;
                _checkBoxDockBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
                _checkBoxDockBottom.Height = 24;
                _checkBoxDockBottom.FlatStyle = FlatStyle.System;

                _checkBoxDockFill.Appearance = System.Windows.Forms.Appearance.Button;
                _checkBoxDockFill.Dock = System.Windows.Forms.DockStyle.Fill;
                _checkBoxDockFill.FlatStyle = FlatStyle.System;

                this.Controls.AddRange(new Control[] {
                                                         _checkBoxDockFill,
                                                         _checkBoxDockBottom,
                                                         _checkBoxDockTop,
                                                         _checkBoxDockRight,
                                                         _checkBoxDockLeft,
                                                         _checkBoxFloat});

                Size = new System.Drawing.Size(160, 144);
                BackColor = SystemColors.Control;
                ResumeLayout();
            }

            public void SetStates(IWindowsFormsEditorService edSvc, DockAreas dockAreas)
            {
                _edSvc = edSvc;
                _oldDockAreas = dockAreas;
                if ((dockAreas & DockAreas.DockLeft) != 0)
                    _checkBoxDockLeft.Checked = true;
                if ((dockAreas & DockAreas.DockRight) != 0)
                    _checkBoxDockRight.Checked = true;
                if ((dockAreas & DockAreas.DockTop) != 0)
                    _checkBoxDockTop.Checked = true;
                if ((dockAreas & DockAreas.DockTop) != 0)
                    _checkBoxDockTop.Checked = true;
                if ((dockAreas & DockAreas.DockBottom) != 0)
                    _checkBoxDockBottom.Checked = true;
                if ((dockAreas & DockAreas.Document) != 0)
                    _checkBoxDockFill.Checked = true;
                if ((dockAreas & DockAreas.Float) != 0)
                    _checkBoxFloat.Checked = true;
            }
        }

        private DockAreasEditor.DockAreasEditorControl _ui = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider sp, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)sp.GetService(typeof(IWindowsFormsEditorService));

            if (_ui == null)
                _ui = new DockAreasEditor.DockAreasEditorControl();

            _ui.SetStates(edSvc, (DockAreas)value);
            edSvc.DropDownControl(_ui);

            return _ui.DockAreas;
        }
    }
}
