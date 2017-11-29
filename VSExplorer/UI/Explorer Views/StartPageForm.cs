using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WinExplorer.UI
{
    public partial class StartPageForm : Form
    {
        public StartPageForm()
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.Dock = DockStyle.None;

            sp = splitContainer1;
            sp.FixedPanel = FixedPanel.Panel2;
            sp.Panel1.BackColor = SystemColors.Control;
            sp.Panel2.BackColor = SystemColors.Control;
            sp.BackColor = Color.LightGray;
            sp.IsSplitterFixed = true;

            pl = sp.Panel1;
            pr = sp.Panel2;
            pc = panel1;
            pd = panel3;
            pd.AutoScroll = true;

            lbh = labelEx1;
            lbh.Text = "Developer News";
            lbh.ForeColor = SystemColors.ActiveCaption;

            HFont = new Font(this.Font.FontFamily, 15.0f);
            HLFont = new Font(this.Font.FontFamily, 18.0f);
            HLVFont = new Font(this.Font.FontFamily, 21.0f);

            lbh.SetMouseLeaveFont(HFont);
            lbh.SetMouseOverFont(HFont);

            lbr = labelEx2;
            lbr.Text = "Read more...";
            lbr.MouseClick += Lbr_MouseClick;

            lbs = labelEx3;
            lbs.Text = "Get Started";
            lbs.ForeColor = Color.DarkViolet;
            lbs.SetMouseLeaveFont(HLVFont);
            lbs.SetMouseOverFont(HLVFont);

            lbn = labelEx5;
            lbn.Text = "Checkout code samples";
            lbn.ForeColor = SystemColors.ActiveCaption;
            lbn.Click += Lbi_Click;
            lbi = labelEx6;
            lbi.Text = "Explore this IDE";
            lbi.ForeColor = SystemColors.ActiveCaption;
            lbi.Click += Lbi_Clicked;

            lbo = labelEx4;
            lbo.Text = "Open";
            lbo.ForeColor = Color.DarkViolet;
            lbo.SetMouseLeaveFont(HLFont);
            lbo.SetMouseOverFont(HLFont);

            lbc = labelEx8;
            lbc.Text = "New project";
            lbc.ForeColor = Color.DarkViolet;
            lbc.SetMouseLeaveFont(HLFont);
            lbc.SetMouseOverFont(HLFont);

            lbnp = labelEx9;
            lbnp.Text = "Create New Project...";
            lbnp.Click += Lbnp_Click;

            labelLine1.Text = "";
            labelLine1.Height = 2;

            tbo = textBox1;
            tbo.BorderStyle = BorderStyle.None;
            tbo.Multiline = true;
            tbo.AppendText("Get code from source control or from external source or local drive.\r\n\r\n:");
            tbo.AppendText("Checkout from: \r\n");
            lbe = labelEx7;
            lbe.Text = "Recent";
            lbe.ForeColor = Color.DarkViolet;
            lbe.SetMouseLeaveFont(HLVFont);
            lbe.SetMouseOverFont(HLVFont);

            this.Resize += Sp_Resize;
            pd.Resize += Pd_Resize;
            pa = panel4;
            pa.AutoScroll = true;
            pp = panel5;
            pb = panel6;
            ptfs = panel7;

            SuspendLayout();
            this.Controls.Remove(sp);
            sp.Location = new Point();
            this.AutoSize = false;
            sp.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.DarkGreen;
            this.Controls.Remove(sp);
            this.Controls.Add(sp);

            ResumeLayout();
            button = pictureBox1;
            button.Image = Rotate(ve_resource.Expander_24x);
            button.Click += Button_Click;
            button.Visible = false;
            LoadRecent();
            LoadTfs();
            LoadProject();
            LoadProjectTemplate();

            source = new CancellationTokenSource();
            cs = source.Token;
            StartTimer(cs);
            this.FormClosing += StartPageForm_FormClosing;
        }

        private CancellationTokenSource source { get; set; }

        private void StartPageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            source.Cancel();
        }

        private CancellationToken cs { get; set; }

        public async Task StartTimer(CancellationToken c)
        {
            while (true)
            {
                MouseInPanel();
                await Task.Delay(1500, c);
                if (c.IsCancellationRequested)
                    break;
            }
        }

        private bool showed = false;

        private void MouseInPanel()
        {
            Point s = System.Windows.Forms.Control.MousePosition;
            Rectangle r = this.RectangleToScreen(pa.Bounds);
            if (s.X > r.X + 95)
                if (s.X < r.X + r.Width + 95)
                    if (s.Y > r.Y)
                        if (s.Y < r.Y + r.Height)
                        {
                            ShowHideScrollbars(pa, true);
                            return;
                        }
            ShowHideScrollbars(pa, false);
        }

        private void Lbi_Clicked(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ve-2016");
        }

        private void Lbi_Click(object sender, EventArgs e)
        {
            Process.Start("https://docs.microsoft.com/en-US/visualstudio/ide/get-started-with-visual-studio");
        }

        private void Lbnp_Click(object sender, EventArgs e)
        {
            ExplorerForms.ef.Command_AddNewProject();
        }

        private int pw;

        private void Button_Click(object sender, EventArgs e)
        {
            //if (sp.Panel2.Width > 60)
            //{
            //    pw = sp.SplitterDistance;
            //    sp.SplitterDistance = sp.Width - 60;
            //    button.Location = new Point(sp.Width - 60, button.Location.Y);
            //    pd.Visible = false;
            //}
            //else
            //{
            //    sp.SplitterDistance = pw;
            //    pd.Visible = true;
            //    button.Location = new Point(pw, button.Location.Y);
            //}
            //Sp_Resize(null, null);
        }

        private PictureBox button { get; set; }

        private static Color InvertColor(Color ColourToInvert)
        {
            return Color.FromArgb((byte)~ColourToInvert.R, (byte)~ColourToInvert.G, (byte)~ColourToInvert.B);
        }

        static public Bitmap Rotate(Image imgs)
        {
            Bitmap img = new Bitmap(imgs);
            {
                img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }

            return img;
        }

        private PanelEx pa { get; set; }
        private Panel pp { get; set; }
        private Panel pb { get; set; }
        private Panel ptfs { get; set; }

        private string[] d = { "Open Project / Solution", "Open Folder", "Open Website" };
        private Bitmap[] bmps = { ve_resource.VisualStudioSolution_256x, ve_resource.FolderOpen_16x, ve_resource.WebURL_32x };

        public void LoadRecent()
        {
            if (ExplorerForms.ef == null)
                return;
            List<string> rs = ExplorerForms.ef.Command_RecentSolutions();

            OpenItemGroup g = new OpenItemGroup();
            g.Location = new Point(0, 0);
            g.Width = pa.Width - 25;
            g.Height = 20;
            pa.Controls.Add(g);
            int h = 23;
            foreach (string c in rs)
            {
                try
                {
                    string name = Path.GetFileName(c);
                    OpenItemShort s = new OpenItemShort();
                    s.filename = c;
                    s.Location = new Point(0, h);
                    s.Width = pa.Width - 25;
                    s.Height = 47;
                    s.SetMainText(name + "\n" + c);
                    s.SetAsBold(name);
                    pa.Controls.Add(s);
                    h += 47;
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void ShowHideScrollbars(PanelEx panel, bool show = true)
        {
            if (show)
            {
                {
                    PanelEx.ShowScrollBar(panel.Handle, 1, show);
                    panel.AutoScroll = true;
                    panel.VerticalScroll.Enabled = true;
                    panel.VerticalScroll.Visible = true;
                    panel.Refresh();
                }
            }
            else
            {
                {
                    PanelEx.ShowScrollBar(panel.Handle, 1, show);
                    //panel.AutoScroll = false;
                    panel.VerticalScroll.Enabled = false;
                    panel.VerticalScroll.Visible = false;
                    panel.Refresh();
                }
            }
        }

        public void LoadProject()
        {
            int h = 5;
            int i = 0;
            foreach (string c in d)
            {
                OpenProjectShort s = new OpenProjectShort();
                s.Location = new Point(0, h);
                s.Width = pa.Width;
                s.Height = 27;
                s.SetMainText(c);
                s.LoadBitmap(bmps[i++]);
                if (i == 1)
                    s.GetAndClearLink().Click += StartPageForm_Click;
                else if (i == 2)
                    s.GetAndClearLink().Click += StartPageForm_FolderClick;
                s.SetTipInfoText(c);
                pp.Controls.Add(s);
                h += 27;
            }
        }

        private void StartPageForm_FolderClick(object sender, EventArgs e)
        {
            ExplorerForms.ef.Command_OpenFolder();
        }

        private void StartPageForm_Click(object sender, EventArgs e)
        {
            ExplorerForms.ef.Command_OpenSolution();
        }

        public void LoadTfs()
        {
            int h = 1;
            //foreach (string c in d)
            {
                OpenProjectShort s = new OpenProjectShort();
                s.Location = new Point(0, h);
                s.Width = pa.Width;
                s.Height = 27;
                s.SetMainText("Visual Studio Team Services");
                s.SetLink("https://www.visualstudio.com/tfs/");
                ptfs.Controls.Add(s);
                h += 27;
            }
        }

        public void LoadProjectTemplate()
        {
            List<string> tmps = NewProjectForm.GetProjectTemplates();

            int h = 5;
            foreach (string c in tmps)
            {
                OpenProjectShort s = new OpenProjectShort();
                s.Location = new Point(0, h);
                s.Width = pa.Width;
                s.Height = 27;
                s.SetLink(c);
                s.SetMainText(Path.GetFileName(c));
                s.GetAndClearLink().Click += StartPageForm_Click1;
                pb.Controls.Add(s);
                h += 27;
            }
        }

        private void StartPageForm_Click1(object sender, EventArgs e)
        {
            LabelEx lb = sender as LabelEx;
            if (lb == null)
                return;
            OpenProjectShort s = lb.Parent as OpenProjectShort;
            if (s == null)
                return;
            string filename = s.urls;

            ExplorerForms.ef.Command_AddNewProject(filename);
        }

        private void Lbr_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("https://blogs.msdn.microsoft.com/visualstudio/");
        }

        public void SetAsBold(string s, RichTextBox rb)
        {
            rb.Find(s, RichTextBoxFinds.MatchCase);

            rb.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Bold);
            rb.SelectionColor = SystemColors.HotTrack;
        }

        public void SetAsGrayed(string s, RichTextBox rb)
        {
            rb.Find(s, RichTextBoxFinds.MatchCase);

            rb.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Regular);
            rb.SelectionColor = SystemColors.GrayText;
        }

        public void SetAsRed(string s, RichTextBox rb)
        {
            rb.Find(s, RichTextBoxFinds.MatchCase);

            rb.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Regular);
            rb.SelectionColor = Color.Red;
        }

        private void Pd_Resize(object sender, EventArgs e)
        {
            foreach (Control c in pd.Controls)
                c.Width = pd.Width - 25;
        }

        private DevNewsPanel p = null;

        public void LoadDevs()
        {
            //p = new DevNewsPanel();
            //pd.Controls.Add(p);
            //GetFeed();
        }

        public void GetFeed()
        {
            Panel p = new Panel();
            p.Location = new Point();
            p.Dock = DockStyle.Fill;
            Label b = new Label();
            b.Location = new Point(30, 30);
            b.ForeColor = Color.Blue;
            b.Text = "Loading Developer News...";
            p.Controls.Add(b);
            sp.Panel2.Controls.Add(p);
            sp.Refresh();

            string url = "https://vsstartpage.blob.core.windows.net/news/vs";
            try
            {
                XmlReader reader = XmlReader.Create(url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();
                foreach (SyndicationItem item in feed.Items)
                {
                    String subject = item.Title.Text.Trim();
                    String summary = item.Summary.Text.Trim();
                    String date = item.PublishDate.ToString().Trim();
                    String urls = item.Links[0].Uri.AbsoluteUri.Trim();
                    DevNewsPanel ps = AddFeedPanel();
                    ps.url = urls;
                    ps.rb.AppendText(subject);
                    ps.rb.AppendText("\n" + summary);
                    ps.rb.AppendText("\n" + "NEW");
                    ps.rb.AppendText(" " + date);
                    SetAsBold(subject, ps.rb);
                    SetAsGrayed(date, ps.rb);
                    SetAsRed("NEW", ps.rb);
                }
            }
            catch (Exception e)
            {
            }

            sp.Panel2.Controls.Remove(p);
        }

        private DevNewsPanel AddFeedPanel()
        {
            DevNewsPanel d = new DevNewsPanel();
            int h = 15;
            foreach (Control c in pd.Controls)
                h += 160;
            d.Location = new Point(0, h);
            d.Width = pd.Width - 25;
            pd.Controls.Add(d);
            return d;
        }

        private int mw = 700;

        private void Sp_Resize(object sender, EventArgs e)
        {
            sp.Location = new Point(0, 0);

            sp.Size = this.ClientSize;

            if (this.Width < 700 + 305)
            {
                sp.Width = 700 + 305;
                sp.Height = this.Size.Height - 30;
            }

            int h = pl.Height;
            int w = mw;
            int x = 0;
            int y = 0;
            if (pl.Width > mw)
                x = (int)((pl.Width - mw) * 0.5);
            pc.Location = new Point(x, y);
            pc.Size = new Size(w, h);
        }

        private Font HFont { get; set; }
        private Font HLFont { get; set; }
        private Font HLVFont { get; set; }
        // https://vsstartpage.blob.core.windows.net/news/vs

        private LabelEx lbh = null;
        private LabelEx lbr = null;
        private LabelEx lbs = null;
        private LabelEx lbo = null;

        private TextBox tbo = null;

        private LabelEx lbn = null;

        private LabelEx lbi = null;

        private LabelEx lbe = null;

        private LabelEx lbc = null;

        private LabelEx lbnp = null;

        private LabelEx lbcp = null;

        private SplitContainer sp = null;

        private Panel pl { get; set; }
        private Panel pr { get; set; }
        private Panel pc { get; set; }
        private Panel pd { get; set; }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }

    public class PanelEx : Panel
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //ShowScrollBar(this.Handle, (int)ScrollBarDirection.SB_BOTH, false);
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }
    }
}