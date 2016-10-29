//Coded by Rajneesh Noonia 2007
//MODIFIED BY Rajneesh Noonia 2007
//Added localization support

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Summary description for FindReplace.
    /// </summary>
    public class FindReplaceForm : Form
    {
        private Panel _pnlButtons;
        private Panel _pnlFind;
        private Panel _pnlReplace;
        private Panel _pnlSettings;
        private Label _lblFindWhat;
        private Button _btnRegex1;
        private Button _button1;
        private Label _lblReplaceWith;
        private Panel _panel1;
        private Panel _panel3;
        private Panel _pnlReplaceButtons;
        private GroupBox _groupBox1;
        private ComboBox _cboFind;
        private ComboBox _cboReplace;
        private Button _btnReplace;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        private Button _btnFind;
        private Button _btnClose;
        private CheckBox _chkWholeWord;
        private CheckBox _chkMatchCase;
        private Button _btnReplaceAll;
        private CheckBox _chkRegEx;
        private Button _btnMarkAll;

        private WeakReference _Control = null;

        private EditViewControl mOwner
        {
            get
            {
                if (_Control != null)
                    return (EditViewControl)_Control.Target;
                else
                    return null;
            }
            set { _Control = new WeakReference(value); }
        }


        private Button _btnDoReplace;
        private string _Last = "";

        /// <summary>
        /// Default constructor for the FindReplaceForm.
        /// </summary>
        public FindReplaceForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _btnFind.Text = Localizations.FindNextButtonText;
            _btnReplace.Text = Localizations.FindReplaceButtonText;
            _btnMarkAll.Text = Localizations.FindMarkAllButtonText;
            _btnReplaceAll.Text = Localizations.FindReplaceAllButtonText;
            _btnClose.Text = Localizations.FindCloseButtonText;

            _lblFindWhat.Text = Localizations.FindWhatLabelText;
            _lblReplaceWith.Text = Localizations.FindReplaceWithLabelText;

            _chkMatchCase.Text = Localizations.FindMatchCaseLabel;
            _chkWholeWord.Text = Localizations.FindMatchWholeWordLabel;
            _chkRegEx.Text = Localizations.FindUseRegExLabel;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //				unchecked
                //				{
                //					int i= (int)0x80000000;
                //					cp.Style |=i;
                //				}
                return cp;
            }
        }

        /// <summary>
        /// Creates a FindReplaceForm that will be assigned to a specific Owner control.
        /// </summary>
        /// <param name="Owner">The SyntaxBox that will use the FindReplaceForm</param>
        public FindReplaceForm(EditViewControl Owner)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            mOwner = Owner;
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

        /// <summary>
        /// Displays the FindReplaceForm and sets it in "Find" mode.
        /// </summary>
        public void ShowFind()
        {
            _pnlReplace.Visible = false;
            _pnlReplaceButtons.Visible = false;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Text = Localizations.FindDialogText;
            this.Show();
            this.Height = 160;
            _btnDoReplace.Visible = false;
            _btnReplace.Visible = true;
            _Last = "";
            _cboFind.Focus();
        }

        /// <summary>
        /// Displays the FindReplaceForm and sets it in "Replace" mode.
        /// </summary>
        public void ShowReplace()
        {
            _pnlReplace.Visible = true;
            _pnlReplaceButtons.Visible = true;
            this.Text = Localizations.ReplaceDialogText;
            this.Show();
            this.Height = 200;
            _btnDoReplace.Visible = true;
            _btnReplace.Visible = false;
            _Last = "";
            _cboFind.Focus();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindReplaceForm));
            _pnlButtons = new System.Windows.Forms.Panel();
            _panel3 = new System.Windows.Forms.Panel();
            _btnClose = new System.Windows.Forms.Button();
            _btnMarkAll = new System.Windows.Forms.Button();
            _pnlReplaceButtons = new System.Windows.Forms.Panel();
            _btnReplaceAll = new System.Windows.Forms.Button();
            _panel1 = new System.Windows.Forms.Panel();
            _btnDoReplace = new System.Windows.Forms.Button();
            _btnReplace = new System.Windows.Forms.Button();
            _btnFind = new System.Windows.Forms.Button();
            _pnlFind = new System.Windows.Forms.Panel();
            _cboFind = new System.Windows.Forms.ComboBox();
            _lblFindWhat = new System.Windows.Forms.Label();
            _btnRegex1 = new System.Windows.Forms.Button();
            _pnlReplace = new System.Windows.Forms.Panel();
            _cboReplace = new System.Windows.Forms.ComboBox();
            _lblReplaceWith = new System.Windows.Forms.Label();
            _button1 = new System.Windows.Forms.Button();
            _pnlSettings = new System.Windows.Forms.Panel();
            _groupBox1 = new System.Windows.Forms.GroupBox();
            _chkRegEx = new System.Windows.Forms.CheckBox();
            _chkWholeWord = new System.Windows.Forms.CheckBox();
            _chkMatchCase = new System.Windows.Forms.CheckBox();
            _pnlButtons.SuspendLayout();
            _panel3.SuspendLayout();
            _pnlReplaceButtons.SuspendLayout();
            _panel1.SuspendLayout();
            _pnlFind.SuspendLayout();
            _pnlReplace.SuspendLayout();
            _pnlSettings.SuspendLayout();
            _groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            _pnlButtons.Controls.Add(_panel3);
            _pnlButtons.Controls.Add(_pnlReplaceButtons);
            _pnlButtons.Controls.Add(_panel1);
            _pnlButtons.Dock = System.Windows.Forms.DockStyle.Right;
            _pnlButtons.Location = new System.Drawing.Point(400, 0);
            _pnlButtons.Name = "pnlButtons";
            _pnlButtons.Size = new System.Drawing.Size(96, 178);
            _pnlButtons.TabIndex = 0;
            // 
            // panel3
            // 
            _panel3.Controls.Add(_btnClose);
            _panel3.Controls.Add(_btnMarkAll);
            _panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            _panel3.Location = new System.Drawing.Point(0, 96);
            _panel3.Name = "panel3";
            _panel3.Size = new System.Drawing.Size(96, 82);
            _panel3.TabIndex = 4;
            // 
            // btnClose
            // 
            _btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _btnClose.Location = new System.Drawing.Point(8, 40);
            _btnClose.Name = "btnClose";
            _btnClose.Size = new System.Drawing.Size(80, 24);
            _btnClose.TabIndex = 3;
            _btnClose.Text = "Close";
            _btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnMarkAll
            // 
            _btnMarkAll.Location = new System.Drawing.Point(8, 8);
            _btnMarkAll.Name = "btnMarkAll";
            _btnMarkAll.Size = new System.Drawing.Size(80, 24);
            _btnMarkAll.TabIndex = 2;
            _btnMarkAll.Text = "Mark all";
            _btnMarkAll.Click += new System.EventHandler(this.btnMarkAll_Click);
            // 
            // pnlReplaceButtons
            // 
            _pnlReplaceButtons.Controls.Add(_btnReplaceAll);
            _pnlReplaceButtons.Dock = System.Windows.Forms.DockStyle.Top;
            _pnlReplaceButtons.Location = new System.Drawing.Point(0, 64);
            _pnlReplaceButtons.Name = "pnlReplaceButtons";
            _pnlReplaceButtons.Size = new System.Drawing.Size(96, 32);
            _pnlReplaceButtons.TabIndex = 3;
            _pnlReplaceButtons.Visible = false;
            // 
            // btnReplaceAll
            // 
            _btnReplaceAll.Location = new System.Drawing.Point(8, 8);
            _btnReplaceAll.Name = "btnReplaceAll";
            _btnReplaceAll.Size = new System.Drawing.Size(80, 24);
            _btnReplaceAll.TabIndex = 2;
            _btnReplaceAll.Text = "Replace All";
            _btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // panel1
            // 
            _panel1.Controls.Add(_btnDoReplace);
            _panel1.Controls.Add(_btnReplace);
            _panel1.Controls.Add(_btnFind);
            _panel1.Dock = System.Windows.Forms.DockStyle.Top;
            _panel1.Location = new System.Drawing.Point(0, 0);
            _panel1.Name = "panel1";
            _panel1.Size = new System.Drawing.Size(96, 64);
            _panel1.TabIndex = 2;
            // 
            // btnDoReplace
            // 
            _btnDoReplace.DialogResult = System.Windows.Forms.DialogResult.OK;
            _btnDoReplace.Location = new System.Drawing.Point(8, 40);
            _btnDoReplace.Name = "btnDoReplace";
            _btnDoReplace.Size = new System.Drawing.Size(80, 24);
            _btnDoReplace.TabIndex = 4;
            _btnDoReplace.Text = "Replace";
            _btnDoReplace.Click += new System.EventHandler(this.btnDoReplace_Click);
            // 
            // btnReplace
            // 
            _btnReplace.Image = ((System.Drawing.Image)(resources.GetObject("btnReplace.Image")));
            _btnReplace.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            _btnReplace.Location = new System.Drawing.Point(8, 40);
            _btnReplace.Name = "btnReplace";
            _btnReplace.Size = new System.Drawing.Size(80, 24);
            _btnReplace.TabIndex = 3;
            _btnReplace.Text = "Replace";
            _btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnFind
            // 
            _btnFind.DialogResult = System.Windows.Forms.DialogResult.OK;
            _btnFind.Location = new System.Drawing.Point(8, 8);
            _btnFind.Name = "btnFind";
            _btnFind.Size = new System.Drawing.Size(80, 24);
            _btnFind.TabIndex = 2;
            _btnFind.Text = "&FindNext";
            _btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // pnlFind
            // 
            _pnlFind.Controls.Add(_cboFind);
            _pnlFind.Controls.Add(_lblFindWhat);
            _pnlFind.Controls.Add(_btnRegex1);
            _pnlFind.Dock = System.Windows.Forms.DockStyle.Top;
            _pnlFind.Location = new System.Drawing.Point(0, 0);
            _pnlFind.Name = "pnlFind";
            _pnlFind.Size = new System.Drawing.Size(400, 40);
            _pnlFind.TabIndex = 1;
            // 
            // cboFind
            // 
            _cboFind.Location = new System.Drawing.Point(104, 8);
            _cboFind.Name = "cboFind";
            _cboFind.Size = new System.Drawing.Size(288, 21);
            _cboFind.TabIndex = 1;
            // 
            // lblFindWhat
            // 
            _lblFindWhat.Location = new System.Drawing.Point(8, 8);
            _lblFindWhat.Name = "lblFindWhat";
            _lblFindWhat.Size = new System.Drawing.Size(96, 24);
            _lblFindWhat.TabIndex = 0;
            _lblFindWhat.Text = "Fi&nd what:";
            // 
            // btnRegex1
            // 
            _btnRegex1.Image = ((System.Drawing.Image)(resources.GetObject("btnRegex1.Image")));
            _btnRegex1.Location = new System.Drawing.Point(368, 8);
            _btnRegex1.Name = "btnRegex1";
            _btnRegex1.Size = new System.Drawing.Size(21, 21);
            _btnRegex1.TabIndex = 2;
            _btnRegex1.Visible = false;
            // 
            // pnlReplace
            // 
            _pnlReplace.Controls.Add(_cboReplace);
            _pnlReplace.Controls.Add(_lblReplaceWith);
            _pnlReplace.Controls.Add(_button1);
            _pnlReplace.Dock = System.Windows.Forms.DockStyle.Top;
            _pnlReplace.Location = new System.Drawing.Point(0, 40);
            _pnlReplace.Name = "pnlReplace";
            _pnlReplace.Size = new System.Drawing.Size(400, 40);
            _pnlReplace.TabIndex = 2;
            _pnlReplace.Visible = false;
            // 
            // cboReplace
            // 
            _cboReplace.Location = new System.Drawing.Point(106, 8);
            _cboReplace.Name = "cboReplace";
            _cboReplace.Size = new System.Drawing.Size(286, 21);
            _cboReplace.TabIndex = 4;
            // 
            // lblReplaceWith
            // 
            _lblReplaceWith.Location = new System.Drawing.Point(10, 8);
            _lblReplaceWith.Name = "lblReplaceWith";
            _lblReplaceWith.Size = new System.Drawing.Size(96, 24);
            _lblReplaceWith.TabIndex = 3;
            _lblReplaceWith.Text = "Re&place with:";
            // 
            // button1
            // 
            _button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            _button1.Location = new System.Drawing.Point(368, 8);
            _button1.Name = "button1";
            _button1.Size = new System.Drawing.Size(21, 21);
            _button1.TabIndex = 5;
            _button1.Visible = false;
            // 
            // pnlSettings
            // 
            _pnlSettings.Controls.Add(_groupBox1);
            _pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            _pnlSettings.Location = new System.Drawing.Point(0, 80);
            _pnlSettings.Name = "pnlSettings";
            _pnlSettings.Size = new System.Drawing.Size(400, 98);
            _pnlSettings.TabIndex = 3;
            // 
            // groupBox1
            // 
            _groupBox1.Controls.Add(_chkRegEx);
            _groupBox1.Controls.Add(_chkWholeWord);
            _groupBox1.Controls.Add(_chkMatchCase);
            _groupBox1.Location = new System.Drawing.Point(8, 0);
            _groupBox1.Name = "groupBox1";
            _groupBox1.Size = new System.Drawing.Size(384, 88);
            _groupBox1.TabIndex = 0;
            _groupBox1.TabStop = false;
            _groupBox1.Text = "Search";
            // 
            // chkRegEx
            // 
            _chkRegEx.Location = new System.Drawing.Point(8, 64);
            _chkRegEx.Name = "chkRegEx";
            _chkRegEx.Size = new System.Drawing.Size(144, 16);
            _chkRegEx.TabIndex = 2;
            _chkRegEx.Text = "Use Regular expressions";
            // 
            // chkWholeWord
            // 
            _chkWholeWord.Location = new System.Drawing.Point(8, 40);
            _chkWholeWord.Name = "chkWholeWord";
            _chkWholeWord.Size = new System.Drawing.Size(144, 16);
            _chkWholeWord.TabIndex = 1;
            _chkWholeWord.Text = "Match &whole word";
            // 
            // chkMatchCase
            // 
            _chkMatchCase.Location = new System.Drawing.Point(8, 16);
            _chkMatchCase.Name = "chkMatchCase";
            _chkMatchCase.Size = new System.Drawing.Size(144, 16);
            _chkMatchCase.TabIndex = 0;
            _chkMatchCase.Text = "Match &case";
            // 
            // FindReplaceForm
            // 
            this.AcceptButton = _btnFind;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = _btnClose;
            this.ClientSize = new System.Drawing.Size(496, 178);
            this.Controls.Add(_pnlSettings);
            this.Controls.Add(_pnlReplace);
            this.Controls.Add(_pnlFind);
            this.Controls.Add(_pnlButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FindReplaceForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FindReplace_Closing);
            _pnlButtons.ResumeLayout(false);
            _panel3.ResumeLayout(false);
            _pnlReplaceButtons.ResumeLayout(false);
            _panel1.ResumeLayout(false);
            _pnlFind.ResumeLayout(false);
            _pnlReplace.ResumeLayout(false);
            _pnlSettings.ResumeLayout(false);
            _groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private void btnReplace_Click(object sender, EventArgs e)
        {
            ShowReplace();
        }

        private void FindReplace_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            this.FindNext();
            _cboFind.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            mOwner.Focus();
            this.Hide();
        }

        private void btnDoReplace_Click(object sender, EventArgs e)
        {
            this.mOwner.ReplaceSelection(_cboReplace.Text);
            btnFind_Click(null, null);
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            string text = _cboFind.Text;
            if (text == "")
                return;

            bool found = false;
            foreach (string s in _cboFind.Items)
            {
                if (s == text)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                _cboFind.Items.Add(text);

            int x = mOwner.Caret.Position.X;
            int y = mOwner.Caret.Position.Y;
            mOwner.Caret.Position.X = 0;
            mOwner.Caret.Position.Y = 0;
            while (mOwner.SelectNext(_cboFind.Text, _chkMatchCase.Checked, _chkWholeWord.Checked, _chkRegEx.Checked))
            {
                this.mOwner.ReplaceSelection(_cboReplace.Text);
            }

            mOwner.Selection.ClearSelection();
            //	mOwner.Caret.Position.X=x;
            //	mOwner.Caret.Position.Y=y;
            //	mOwner.ScrollIntoView ();

            _cboFind.Focus();
        }

        private void btnMarkAll_Click(object sender, EventArgs e)
        {
            string text = _cboFind.Text;
            if (text == "")
                return;

            bool found = false;
            foreach (string s in _cboFind.Items)
            {
                if (s == text)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                _cboFind.Items.Add(text);

            int x = mOwner.Caret.Position.X;
            int y = mOwner.Caret.Position.Y;
            mOwner.Caret.Position.X = 0;
            mOwner.Caret.Position.Y = 0;
            while (mOwner.SelectNext(_cboFind.Text, _chkMatchCase.Checked, _chkWholeWord.Checked, _chkRegEx.Checked))
            {
                this.mOwner.Caret.CurrentRow.Bookmarked = true;
            }

            mOwner.Selection.ClearSelection();
            //	mOwner.Caret.Position.X=x;
            //	mOwner.Caret.Position.Y=y;
            //	mOwner.ScrollIntoView ();

            _cboFind.Focus();
        }

        public void FindNext()
        {
            string text = _cboFind.Text;

            if (_Last != "" && _Last != text)
            {
                this.mOwner.Caret.Position.X = 0;
                this.mOwner.Caret.Position.Y = 0;
                this.mOwner.ScrollIntoView();
            }

            _Last = text;

            if (text == "")
                return;

            bool found = false;
            foreach (string s in _cboFind.Items)
            {
                if (s == text)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                _cboFind.Items.Add(text);

            mOwner.SelectNext(_cboFind.Text, _chkMatchCase.Checked, _chkWholeWord.Checked, _chkRegEx.Checked);
        }
    }
}
