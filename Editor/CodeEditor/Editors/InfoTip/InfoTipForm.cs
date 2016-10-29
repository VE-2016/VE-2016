//Coded by Rajneesh Noonia 2007

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.WinForms;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Summary description for InfoTip.
    /// </summary>
    public class InfoTipForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int message, int _data, int _id);


        private int _SelectedIndex = 0;
        private int _Count = 1;

        private WeakReference _Control = null;

        private Control ParentControl
        {
            get
            {
                if (_Control != null)
                    return (Control)_Control.Target;
                else
                    return null;
            }
            set { _Control = new WeakReference(value); }
        }


        private Panel _panel2;
        private FormatLabelControl _infoText;
        private PictureBox _btnPrev;
        private PictureBox _btnNext;
        private Panel _pnlSelect;
        private Label _lblIndex;
        private Panel _pnlImage;
        private PictureBox _picIcon;
        private Panel _panel1;

        public event EventHandler SelectedIndexChanged = null;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container _components = null;

        /// <summary>
        /// 
        /// </summary>
        public InfoTipForm()
        {
            InitializeComponent();
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(InfoTipForm_KeyDown);
        }

        private void InfoTipForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                btnPrev_Click(sender, null);
            else if (e.KeyCode == Keys.Up)
                btnNext_Click(sender, null);
        }


        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                if (value > _Count)
                    value = 1;
                if (value < 1)
                    value = _Count;

                _SelectedIndex = value;
                OnSelectedIndexChanged();
                SetPos();
            }
        }

        public void SetBackground(Color c)
        {
            _panel2.BackColor = c;
            _pnlSelect.BackColor = c;
            _pnlImage.BackColor = c;
        }


        public int Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public InfoTipForm(Control parent)
        {
            this.ParentControl = parent;
            this.CreateParams.ClassName = "tooltips_class32";
            //		//	this.CreateParams.Parent =ParentControl.Handle;
            //			this.RecreateHandle ();


            InitializeComponent();
        }

        public void SetText(string s)
        {
            _infoText.Text = s;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoTipForm));
            _pnlSelect = new System.Windows.Forms.Panel();
            _btnNext = new System.Windows.Forms.PictureBox();
            _btnPrev = new System.Windows.Forms.PictureBox();
            _lblIndex = new System.Windows.Forms.Label();
            _panel2 = new System.Windows.Forms.Panel();
            _infoText = new AIMS.Libraries.CodeEditor.WinForms.FormatLabelControl();
            _pnlImage = new System.Windows.Forms.Panel();
            _picIcon = new System.Windows.Forms.PictureBox();
            _panel1 = new System.Windows.Forms.Panel();
            _pnlSelect.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_btnNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(_btnPrev)).BeginInit();
            _panel2.SuspendLayout();
            _pnlImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_picIcon)).BeginInit();
            _panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSelect
            // 
            _pnlSelect.BackColor = System.Drawing.SystemColors.Control;
            _pnlSelect.Controls.Add(_btnNext);
            _pnlSelect.Controls.Add(_btnPrev);
            _pnlSelect.Controls.Add(_lblIndex);
            _pnlSelect.Dock = System.Windows.Forms.DockStyle.Left;
            _pnlSelect.Location = new System.Drawing.Point(32, 0);
            _pnlSelect.Name = "pnlSelect";
            _pnlSelect.Padding = new System.Windows.Forms.Padding(4);
            _pnlSelect.Size = new System.Drawing.Size(80, 35);
            _pnlSelect.TabIndex = 0;
            // 
            // btnNext
            // 
            _btnNext.BackColor = System.Drawing.SystemColors.Control;
            _btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
            _btnNext.Location = new System.Drawing.Point(68, 6);
            _btnNext.Name = "btnNext";
            _btnNext.Size = new System.Drawing.Size(9, 11);
            _btnNext.TabIndex = 1;
            _btnNext.TabStop = false;
            _btnNext.Click += new System.EventHandler(this.btnNext_Click);
            _btnNext.DoubleClick += new System.EventHandler(this.btnNext_DoubleClick);
            // 
            // btnPrev
            // 
            _btnPrev.BackColor = System.Drawing.SystemColors.Control;
            _btnPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev.Image")));
            _btnPrev.Location = new System.Drawing.Point(4, 6);
            _btnPrev.Name = "btnPrev";
            _btnPrev.Size = new System.Drawing.Size(9, 11);
            _btnPrev.TabIndex = 0;
            _btnPrev.TabStop = false;
            _btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            _btnPrev.DoubleClick += new System.EventHandler(this.btnPrev_DoubleClick);
            // 
            // lblIndex
            // 
            _lblIndex.BackColor = System.Drawing.SystemColors.Control;
            _lblIndex.Dock = System.Windows.Forms.DockStyle.Top;
            _lblIndex.Location = new System.Drawing.Point(4, 4);
            _lblIndex.Name = "lblIndex";
            _lblIndex.Size = new System.Drawing.Size(72, 23);
            _lblIndex.TabIndex = 2;
            _lblIndex.Text = "20 of 20";
            _lblIndex.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            _panel2.BackColor = System.Drawing.SystemColors.Control;
            _panel2.Controls.Add(_infoText);
            _panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            _panel2.Location = new System.Drawing.Point(112, 0);
            _panel2.Name = "panel2";
            _panel2.Padding = new System.Windows.Forms.Padding(4);
            _panel2.Size = new System.Drawing.Size(126, 35);
            _panel2.TabIndex = 1;
            // 
            // InfoText
            // 
            _infoText.AutoSizeHorizontal = true;
            _infoText.AutoSizeVertical = true;
            _infoText.BackColor = System.Drawing.SystemColors.Control;
            _infoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _infoText.ImageList = null;
            _infoText.LabelMargin = 0;
            _infoText.Link_Color = System.Drawing.Color.Blue;
            _infoText.Link_Color_Hover = System.Drawing.Color.Blue;
            _infoText.Link_UnderLine = false;
            _infoText.Link_UnderLine_Hover = true;
            _infoText.Location = new System.Drawing.Point(2, 4);
            _infoText.Name = "InfoText";
            _infoText.ScrollBars = System.Windows.Forms.ScrollBars.None;
            _infoText.Size = new System.Drawing.Size(59, 13);
            _infoText.TabIndex = 0;
            _infoText.Text = "format <b>label</b>";
            _infoText.WordWrap = false;
            _infoText.Enter += new System.EventHandler(this.InfoText_Enter);
            _infoText.Resize += new System.EventHandler(this.InfoText_Resize);
            // 
            // pnlImage
            // 
            _pnlImage.BackColor = System.Drawing.SystemColors.Control;
            _pnlImage.Controls.Add(_picIcon);
            _pnlImage.Dock = System.Windows.Forms.DockStyle.Left;
            _pnlImage.Location = new System.Drawing.Point(0, 0);
            _pnlImage.Name = "pnlImage";
            _pnlImage.Size = new System.Drawing.Size(32, 35);
            _pnlImage.TabIndex = 2;
            _pnlImage.Visible = false;
            // 
            // picIcon
            // 
            _picIcon.Location = new System.Drawing.Point(5, 3);
            _picIcon.Name = "picIcon";
            _picIcon.Size = new System.Drawing.Size(19, 20);
            _picIcon.TabIndex = 1;
            _picIcon.TabStop = false;
            // 
            // panel1
            // 
            _panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _panel1.Controls.Add(_panel2);
            _panel1.Controls.Add(_pnlSelect);
            _panel1.Controls.Add(_pnlImage);
            _panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            _panel1.Location = new System.Drawing.Point(0, 0);
            _panel1.Name = "panel1";
            _panel1.Size = new System.Drawing.Size(240, 37);
            _panel1.TabIndex = 3;
            // 
            // InfoTipForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(240, 37);
            this.ControlBox = false;
            this.Controls.Add(_panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "InfoTipForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Enter += new System.EventHandler(this.InfoTipForm_Enter);
            _pnlSelect.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_btnNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(_btnPrev)).EndInit();
            _panel2.ResumeLayout(false);
            _pnlImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_picIcon)).EndInit();
            _panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        //	public void ShowInfo(int x,int y,string text)
        //	{
        //		this.Show ();
        //		this.Location =new Point (x,y);
        //	}
        private void InfoText_Resize(object sender, EventArgs e)
        {
            DoResize();
        }

        private void DoResize()
        {
            int w = _infoText.Left + _infoText.Width + 8;
            if (Count > 1)
            {
                w += _pnlSelect.Width;
            }
            if (_picIcon.Image != null)
            {
                w += _pnlImage.Width;
            }


            int h = _infoText.Top + _infoText.Height + 6;
            if (this.Image != null && this.Image.Height + _picIcon.Top * 2 > h)
                h = this.Image.Height + _picIcon.Top * 2;

            this.ClientSize = new Size(w, h);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            SelectedIndex = 1;
            SetPos();
        }

        public Image Image
        {
            get { return _picIcon.Image; }
            set
            {
                _picIcon.Image = value;
                if (value == null)
                {
                    _pnlImage.Visible = false;
                }
                else
                {
                    _pnlImage.Visible = true;
                    _pnlImage.Width = this.Image.Width + 6;
                    _picIcon.Size = Image.Size;
                }
                DoResize();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SelectedIndex++;
            SetPos();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            SelectedIndex--;
            SetPos();
        }

        private void btnPrev_DoubleClick(object sender, EventArgs e)
        {
            SelectedIndex--;
            SetPos();
        }

        private void btnNext_DoubleClick(object sender, EventArgs e)
        {
            SelectedIndex++;
            SetPos();
        }

        private void SetPos()
        {
            if (Count == 1)
            {
                _pnlSelect.Visible = false;
            }
            else
            {
                _pnlSelect.Visible = true;
            }
            DoResize();

            _lblIndex.Text = SelectedIndex.ToString() + " of " + Count.ToString();

            if (ParentControl != null)
                ParentControl.Focus();
        }

        private void InfoTipForm_Enter(object sender, EventArgs e)
        {
            ParentControl.Focus();
        }

        private void InfoText_Enter(object sender, EventArgs e)
        {
            ParentControl.Focus();
        }

        public string Data
        {
            get { return _infoText.Text; }
            set { _infoText.Text = value; }
        }

        private void OnSelectedIndexChanged()
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, null);
        }
    }
}