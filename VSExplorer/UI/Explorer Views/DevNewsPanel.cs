using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WinExplorer.UI
{
    public partial class DevNewsPanel : UserControl
    {
        public DevNewsPanel()
        {
            InitializeComponent();
            rb = richTextBox1;
            // rb.ContentsResized += Rb_ContentsResized;
            rb.MouseMove += Rb_MouseHover;
            rb.MouseClick += Rb_MouseClick;
            rb.MouseEnter += Rb_MouseEnter;
            rb.MouseLeave += Rb_MouseLeave;
            rb.ReadOnly = true;
            
            
        }

        private void Rb_MouseLeave(object sender, EventArgs e)
        {
            rb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            rb.Refresh();
        }

        private void Rb_MouseEnter(object sender, EventArgs e)
        {
            rb.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Both;
            rb.Refresh();
        }

        private void Rb_MouseClick(object sender, MouseEventArgs e)
        {
            int c = rb.GetCharIndexFromPosition(new Point(e.X, e.Y));
            rb.Select(c, 1);
            if (rb.SelectionFont.Bold)
            {
                Process.Start(url.ToString());
            }
        }

        private void Rb_MouseHover(object sender, MouseEventArgs e)
        {
            int c = rb.GetCharIndexFromPosition(new Point(e.X, e.Y));
            rb.Select(c, 1);
            if (rb.SelectionFont.Bold)
            {
                if(rb.Cursor != Cursors.Hand)
                rb.Cursor = Cursors.Hand;
            }
            else
            {
                if (rb.Cursor != Cursors.Default)
                    rb.Cursor = Cursors.Default;
            }
            rb.DeselectAll();
        }

        private void Rb_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            var richTextBox = (RichTextBox)sender;
            //richTextBox.Width = e.NewRectangle.Width;
            richTextBox.Height = e.NewRectangle.Height;
        }
        public string url { get; set; }
        public RichTextBox rb { get; set; }
    }
}
