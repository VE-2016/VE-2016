using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.CodeEditor.Syntax
{
    /// <summary>
    /// Summary description for TextStyleDesignerDialog.
    /// </summary>
    public class TextStyleDesignerDialog : Form
    {
        public static event EventHandler Change;

        protected static void OnChange()
        {
            if (Change != null)
                Change(null, EventArgs.Empty);
        }


        private TextStyle _Style = null;
        private TextStyle _TmpStyle = null;
        private Panel _panel2;
        private Button _btnCancel;
        private Button _btnOK;
        private PropertyGrid _pgStyles;
        private Panel _panel1;
        private Panel _panel3;
        private Label _lblCaption;
        private Label _lblPreview;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        public TextStyleDesignerDialog(TextStyle Style)
        {
            _Style = Style;
            _TmpStyle = (TextStyle)Style.Clone();

            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _pgStyles.SelectedObject = _TmpStyle;
            _lblCaption.Text = _Style.ToString();
            PreviewStyle();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _panel2 = new System.Windows.Forms.Panel();
            _btnCancel = new System.Windows.Forms.Button();
            _btnOK = new System.Windows.Forms.Button();
            _pgStyles = new System.Windows.Forms.PropertyGrid();
            _panel1 = new System.Windows.Forms.Panel();
            _panel3 = new System.Windows.Forms.Panel();
            _lblCaption = new System.Windows.Forms.Label();
            _lblPreview = new System.Windows.Forms.Label();
            _panel2.SuspendLayout();
            _panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            _panel2.Controls.AddRange(new System.Windows.Forms.Control[]
                {
                    _lblPreview,
                    _btnCancel,
                    _btnOK
                });
            _panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            _panel2.Location = new System.Drawing.Point(4, 255);
            _panel2.Name = "panel2";
            _panel2.Size = new System.Drawing.Size(354, 80);
            _panel2.TabIndex = 8;
            // 
            // btnCancel
            // 
            _btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnCancel.Location = new System.Drawing.Point(279, 48);
            _btnCancel.Name = "btnCancel";
            _btnCancel.TabIndex = 4;
            _btnCancel.Text = "Cancel";
            _btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            _btnOK.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            _btnOK.Location = new System.Drawing.Point(200, 48);
            _btnOK.Name = "btnOK";
            _btnOK.TabIndex = 3;
            _btnOK.Text = "OK";
            _btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pgStyles
            // 
            _pgStyles.CommandsVisibleIfAvailable = true;
            _pgStyles.Dock = System.Windows.Forms.DockStyle.Fill;
            _pgStyles.LargeButtons = false;
            _pgStyles.LineColor = System.Drawing.SystemColors.ScrollBar;
            _pgStyles.Location = new System.Drawing.Point(4, 26);
            _pgStyles.Name = "pgStyles";
            _pgStyles.Size = new System.Drawing.Size(354, 221);
            _pgStyles.TabIndex = 6;
            _pgStyles.Text = "propertyGrid1";
            _pgStyles.ToolbarVisible = false;
            _pgStyles.ViewBackColor = System.Drawing.SystemColors.Window;
            _pgStyles.ViewForeColor = System.Drawing.SystemColors.WindowText;
            _pgStyles.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgStyles_PropertyValueChanged);
            // 
            // panel1
            // 
            _panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            _panel1.Location = new System.Drawing.Point(4, 247);
            _panel1.Name = "panel1";
            _panel1.Size = new System.Drawing.Size(354, 8);
            _panel1.TabIndex = 9;
            // 
            // panel3
            // 
            _panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            _panel3.Controls.AddRange(new System.Windows.Forms.Control[]
                {
                    _lblCaption
                });
            _panel3.Dock = System.Windows.Forms.DockStyle.Top;
            _panel3.Location = new System.Drawing.Point(4, 2);
            _panel3.Name = "panel3";
            _panel3.Size = new System.Drawing.Size(354, 24);
            _panel3.TabIndex = 10;
            // 
            // lblCaption
            // 
            _lblCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            _lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            _lblCaption.ForeColor = System.Drawing.SystemColors.Window;
            _lblCaption.Name = "lblCaption";
            _lblCaption.Size = new System.Drawing.Size(354, 24);
            _lblCaption.TabIndex = 0;
            _lblCaption.Text = "-";
            _lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPreview
            // 
            _lblPreview.BackColor = System.Drawing.SystemColors.Window;
            _lblPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            _lblPreview.Dock = System.Windows.Forms.DockStyle.Top;
            _lblPreview.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            _lblPreview.Name = "lblPreview";
            _lblPreview.Size = new System.Drawing.Size(354, 40);
            _lblPreview.TabIndex = 8;
            _lblPreview.Text = "The quick brown fox jumped over the lazy dog.        ";
            _lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TextStyleDesignerDialog
            // 
            this.AcceptButton = _btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = _btnCancel;
            this.ClientSize = new System.Drawing.Size(362, 335);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                {
                    _pgStyles,
                    _panel3,
                    _panel1,
                    _panel2
                });
            this.DockPadding.Left = 4;
            this.DockPadding.Right = 4;
            this.DockPadding.Top = 2;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TextStyleDesignerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Style Designer";
            this.Load += new System.EventHandler(this.TextStyleDesignerDialog_Load);
            _panel2.ResumeLayout(false);
            _panel3.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void pgStyles_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PreviewStyle();
        }

        private void PreviewStyle()
        {
            TextStyle s = _TmpStyle;

            _lblPreview.ForeColor = s.ForeColor;
            if (s.BackColor != Color.Transparent)
                _lblPreview.BackColor = s.BackColor;
            else
                _lblPreview.BackColor = Color.White;


            FontStyle fs = FontStyle.Regular;
            if (s.Bold)
                fs |= FontStyle.Bold;
            if (s.Italic)
                fs |= FontStyle.Italic;
            if (s.Underline)
                fs |= FontStyle.Underline;

            _lblPreview.Font = new Font("Courier New", 11f, fs);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _Style.BackColor = _TmpStyle.BackColor;
            _Style.ForeColor = _TmpStyle.ForeColor;
            _Style.Bold = _TmpStyle.Bold;
            _Style.Italic = _TmpStyle.Italic;
            _Style.Underline = _TmpStyle.Underline;
            OnChange();
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void TextStyleDesignerDialog_Load(object sender, EventArgs e)
        {
        }
    }
}