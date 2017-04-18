using NuGet;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer.Services.NuGet
{
    public partial class NuGetPacketShort : UserControl
    {
        public NuGetPacketShort(NuGetForm.task tasks = NuGetForm.task.none)
        {
            InitializeComponent();

            this.tasks = tasks;

            panel = panel1;
            rb = richTextBox1;
            button = pictureBox1;

            // this.Controls.Remove(panel);
            //this.Controls.Remove(button);

            rb.Resize += Rb_Resize;

            //rb.Controls.Add(button);
            //rb.Controls.Add(panel);

            //panel.Dock = DockStyle.Right;

            HookChildrenEvents(this);

            this.MouseLeave += NuGetPacketShort_MouseLeave;
            this.MouseEnter += NuGetPacketShort_MouseHover;
            this.MouseClick += NuGetPacketShort_MouseClick;

            vLabel = label1;

            uLabel = label2;

            pbInstall = pictureBox3;

            pbUpdate = pictureBox4;

            pbInstall.Visible = false;

            pbUpdate.Visible = false;

            pbInstall.MouseHover += PbInstall_MouseHover;

            pbUpdate.MouseHover += PbUpdate_MouseHover;

            this.Resize += NuGetPacketShort_Resize;

            rb.RightMargin = rb.Width - panel.Width - 5;
            SetHandlers(vLabel);
            SetHandlers(uLabel);
            if (tasks != NuGetForm.task.update)
            {
                LoadNoUpdate();
            }
            else rb.SelectionIndent += 130;

            this.AutoScroll = false;
            this.VScroll = false;
            this.VerticalScroll.Enabled = false;
        }

        public void ResizeShortinfo()
        {
            Rb_Resize(null, null);
        }

        private void LoadNoUpdate()
        {
            this.Controls.Remove(checkBox1);
            button.Location = new Point(0, 0);
            rb.SelectionIndent += 110;
        }

        private NuGetForm.task tasks { get; set; }

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
            rb.RightMargin = rb.Width - panel.Width - 5;
        }

        public delegate void NuGetShortSelected(NuGetPacketShort sender);

        public event NuGetShortSelected nugetShortSelectedEvent;

        private void NuGetPacketShort_MouseClick(object sender, MouseEventArgs e)
        {
            if (nugetShortSelectedEvent != null)
                nugetShortSelectedEvent(this);
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

            rb.SelectionFont = new Font("Verdana", 10, FontStyle.Bold);
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

            cc.BackColor = Color.FromKnownColor(KnownColor.Control);

            if (Parent != null)
            {
                foreach (Control c in cc.Controls)
                {
                    c.BackColor = Color.FromKnownColor(KnownColor.Control);
                    c.Invalidate();
                    c.Refresh();
                }
            }

            button.BackColor = Color.FromKnownColor(KnownColor.Control);
            panel.BackColor = Color.FromKnownColor(KnownColor.Control);
            rb.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.Invalidate();
            this.Refresh();
        }

        private void NuGetPacketShort_MouseHover(object sender, EventArgs e)
        {
            Control c = sender as Control;

            c.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            foreach (Control cc in c.Controls)
                cc.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);

            button.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            panel.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            rb.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
        }
    }
}