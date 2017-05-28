using NuGet;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class OpenProjectShort : UserControl
    {
        public OpenProjectShort()
        {
            InitializeComponent();

           
            
            button = pictureBox1;


            textLink = labelEx1;

            textLink.Click += TextLink_Click;

            textLink.MouseHover += TextLink_MouseHover;

            this.BackColor = Color.White;

            this.Resize += NuGetPacketShort_Resize;


            button.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = Resources.Cloud_256x;
           
            this.AutoScroll = false;
            this.VScroll = false;
            this.VerticalScroll.Enabled = false;
        }

        public string TipInfoText { get; set; }

        private void TextLink_MouseHover(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TipInfoText))
                return;
            ToolTip1.SetToolTip(this.textLink, TipInfoText);
        }

        public void SetTipInfoText(string tipstring)
        {
            TipInfoText = tipstring;
        }

        System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
        
        private void TextLink_Click(object sender, EventArgs e)
        {
            RunLink();
        }

        public string urls { get; set; }

        public void SetLink(string url)
        {
            urls = url;
        }

        public LabelEx GetAndClearLink()
        {
            textLink.Click -= TextLink_Click;

            return textLink;
        }


        public void RunLink()
        {
            if(!string.IsNullOrEmpty(urls))
            Process.Start(urls);
        }

        public void LoadBitmap(Bitmap bmp)
        {
            button.Image = bmp;
        }

        public void ResizeShortinfo()
        {
            Rb_Resize(null, null);
        }

      

        

    
        private void Rb_Resize(object sender, EventArgs e)
        {
           
           
           
        }

     

        private void SetHandlers(Control c)
        {
            c.MouseHover += NuGetPacketShort_MouseHover;
            c.MouseEnter += NuGetPacketShort_MouseHover;
        }

        private void NuGetPacketShort_Resize(object sender, EventArgs e)
        {
           
            
        }

        

        

        private void NuGetPacketShort_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

      
        private PictureBox button { get; set; }

        private LabelEx textLink { get; set; }

        private Panel panel { get; set; }

        private string MainText { get; set; }

        public void SetMainText(string text)
        {
            textLink.Text = text;
            MainText = text;
        }

  
        public void SetAsBold(string s)
        {
           
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
           // panel.BackColor = Color.FromKnownColor(KnownColor.Control);
            textLink.BackColor = Color.FromKnownColor(KnownColor.Control);
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
           // panel.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
            textLink.BackColor = Color.FromKnownColor(KnownColor.AliceBlue);
        }
    }
}