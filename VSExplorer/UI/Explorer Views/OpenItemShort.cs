using NuGet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class OpenItemShort : UserControl
    {
        public OpenItemShort()
        {
            InitializeComponent();

           
            rb = richTextBox1;
            rb.ReadOnly = true;
            rb.WordWrap = false;
            button = pictureBox1;

            // this.Controls.Remove(panel);
            //this.Controls.Remove(button);

           // rb.Resize += Rb_Resize;

            //rb.Controls.Add(button);
            //rb.Controls.Add(panel);

            //panel.Dock = DockStyle.Right;

            HookChildrenEvents(this);

            this.MouseLeave += NuGetPacketShort_MouseLeave;
            this.MouseEnter += NuGetPacketShort_MouseHover;
            this.MouseClick += NuGetPacketShort_MouseClick;

            rb.BackColor = Color.White;// FromKnownColor(KnownColor.Control);

            this.BackColor = Color.White;// FromKnownColor(KnownColor.Control);

            this.Resize += NuGetPacketShort_Resize;

            //rb.RightMargin = rb.Width - 5;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = ve_resource.VisualStudioSolution_256x;

            rb.MouseDoubleClick += Rb_MouseDoubleClick;

            this.AutoScroll = false;
            this.VScroll = false;
            this.VerticalScroll.Enabled = false;
        }

        private void Rb_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ExplorerForms.ef.Command_LoadSolution(filename);
        }

        public string filename { get; set; }

        public void ResizeShortinfo()
        {
            Rb_Resize(null, null);
        }

      

        

        private void GetDescription(int c)
        {
            if (c > MainText.Length - 1)
                c = MainText.Length - 1;
            string texts = MainText.Substring(0, c);
            rb.Clear();
            rb.Text = texts;
            SetAsBold(package.GetFullName());
        }

        private void Rb_Resize(object sender, EventArgs e)
        {
            if (package == null)
                return;
            int i = rb.GetFirstCharIndexFromLine(5);
            if (i < 0)
                i = MainText.Length - 1;
            //if (i > rb.Text.Length - 2)
            //    return;
            GetDescription(i);
        }

        private void PbUpdate_MouseHover(object sender, EventArgs e)
        {
            pbUpdate.Visible = true;
            pbUpdate.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            pbUpdate.Refresh();
        }

        private void PbInstall_MouseHover(object sender, EventArgs e)
        {
            pbInstall.Visible = true;
            pbInstall.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            pbInstall.Refresh();
        }

        private void SetHandlers(Control c)
        {
            c.MouseHover += NuGetPacketShort_MouseHover;
            c.MouseEnter += NuGetPacketShort_MouseHover;
        }

        private void NuGetPacketShort_Resize(object sender, EventArgs e)
        {
            //if(this.Parent == null)
            //    return;
            //this.Width = Parent.ClientRectangle.Width - 1;
            rb.RightMargin = rb.Width  - 5;
        }

        

        

        private void NuGetPacketShort_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private Label vLabel { get; set; }

        private Label uLabel { get; set; }

        public IPackage package { get; set; }

        private PictureBox pbInstall { get; set; }

        private PictureBox pbUpdate { get; set; }

        public void SetPackage(IPackage package)
        {
            this.package = package;
        }

        private PictureBox button { get; set; }

        private RichTextBox rb { get; set; }

        private Panel panel { get; set; }

        private string MainText { get; set; }

        public void SetMainText(string text)
        {
            rb.Text = text;
            MainText = text;
        }

        public void SetVersion(string text)
        {
            vLabel.Text = text;
        }

        public void SetUpdateVersion(string text)
        {
            uLabel.Text = text;
        }

        public void SetAsBold(string s)
        {
            rb.Find(s, RichTextBoxFinds.MatchCase);

            rb.SelectionFont = new Font("Verdana", 9, FontStyle.Bold);
            rb.SelectionColor = Color.Black;
        }

        public void SetIcon(Bitmap icon)
        {
            button.Image = new Bitmap(icon, 64, 64);
        }

        private void HookChildrenEvents(Control control, int level = 0)
        {
            level++;
            foreach (Control child in control.Controls)
            {
                if (child.Controls.Count > 0)
                {
                    HookChildrenEvents(child, level);
                }

                child.MouseEnter += NuGetPacketShort_MouseHover;
                child.MouseHover += NuGetPacketShort_MouseHover;
                if (level < 3)
                    child.MouseLeave += NuGetPacketShort_MouseLeave;
                child.MouseClick += NuGetPacketShort_MouseClick;
            }
        }

        private void NuGetPacketShort_MouseLeave(object sender, EventArgs e)
        {
            if (sender.Equals(vLabel))
                return;
            if (sender.Equals(uLabel))
                return;

            Control cc = sender as Control;

            cc.BackColor = Color.White;// FromKnownColor(KnownColor.Control);
            this.BackColor = Color.White;
            if (Parent != null)
            {
                foreach (Control c in cc.Controls)
                {
                    c.BackColor = Color.White;// FromKnownColor(KnownColor.Control);
                    c.Invalidate();
                    c.Refresh();
                }
            }

            button.BackColor = Color.White;//.FromKnownColor(KnownColor.Control);
                                           // panel.BackColor = Color.FromKnownColor(KnownColor.Control);
            rb.BackColor = Color.White;//.FromKnownColor(KnownColor.Control);
            this.Invalidate();
            this.Refresh();
        }

        private void NuGetPacketShort_MouseHover(object sender, EventArgs e)
        {
            Control c = sender as Control;

            this.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            c.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            foreach (Control cc in c.Controls)
                cc.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);

            //if (c.Parent != null)
            //{
                
            //    foreach (Control cs in c.Parent.Controls)
            //    {
            //        cs.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            //        cs.Invalidate();
            //        cs.Refresh();
            //    }
            //}


            button.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
           // panel.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            rb.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
        }
    }

    public class RichTextBoxEx : RichTextBox
    {
        public RichTextBoxEx()
        {
            Selectable = false;
        }
        const int WM_SETFOCUS = 0x0007;
        const int WM_KILLFOCUS = 0x0008;
        [DefaultValue(true)]
        public bool Selectable { get; set; }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SETFOCUS && !Selectable)
                m.Msg = WM_KILLFOCUS;

            base.WndProc(ref m);
        }
    }

}