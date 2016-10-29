//Coded by Rajneesh Noonia 2007

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Summary description for GotoLine.
    /// </summary>
    public class GotoLineForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        private Button _btnCancel;
        private Button _btnOK;
        private Label _lblLines;
        private TextBox _txtRow;
        private EditViewControl _mOwner = null;

        /// <summary>
        /// Default constructor for the GotoLineForm.
        /// </summary>
        public GotoLineForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Creates a GotoLineForm that will be assigned to a specific Owner control.
        /// </summary>
        /// <param name="Owner">The SyntaxBox that will use the GotoLineForm</param>
        /// <param name="RowCount">The number of lines in the owner control</param>
        public GotoLineForm(EditViewControl Owner, int RowCount)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            _lblLines.Text = "Line number (1-" + (RowCount).ToString() + "):";
            _mOwner = Owner;
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
            _btnCancel = new System.Windows.Forms.Button();
            _btnOK = new System.Windows.Forms.Button();
            _txtRow = new System.Windows.Forms.TextBox();
            _lblLines = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            _btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnCancel.Location = new System.Drawing.Point(160, 48);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new System.Drawing.Size(72, 24);
            _btnCancel.TabIndex = 0;
            _btnCancel.Text = "Cancel";
            _btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            _btnOK.Location = new System.Drawing.Point(80, 48);
            _btnOK.Name = "btnOK";
            _btnOK.Size = new System.Drawing.Size(72, 24);
            _btnOK.TabIndex = 1;
            _btnOK.Text = "OK";
            _btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtRow
            // 
            _txtRow.Location = new System.Drawing.Point(8, 24);
            _txtRow.Name = "txtRow";
            _txtRow.Size = new System.Drawing.Size(224, 20);
            _txtRow.TabIndex = 2;
            _txtRow.Text = "";
            // 
            // lblLines
            // 
            _lblLines.Location = new System.Drawing.Point(8, 8);
            _lblLines.Name = "lblLines";
            _lblLines.Size = new System.Drawing.Size(128, 16);
            _lblLines.TabIndex = 3;
            _lblLines.Text = "-";
            // 
            // GotoLineForm
            // 
            this.AcceptButton = _btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = _btnCancel;
            this.ClientSize = new System.Drawing.Size(242, 82);
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                {
                    _lblLines,
                    _txtRow,
                    _btnOK,
                    _btnCancel
                });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GotoLineForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Go To Line";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.GotoLine_Closing);
            this.Activated += new System.EventHandler(this.GotoLine_Activated);
            this.ResumeLayout(false);
        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                int row = int.Parse(_txtRow.Text) - 1;
                _mOwner.GotoLine(row);
            }
            catch
            {
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GotoLine_Closing(object sender, CancelEventArgs e)
        {
            //e.Cancel =true;
            //this.Hide ();
        }

        private void GotoLine_Activated(object sender, EventArgs e)
        {
            _txtRow.Focus();
        }
    }
}