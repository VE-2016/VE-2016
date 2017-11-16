using System;
using System.Collections;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class SQLServerSelectForm : Form
    {
        public SQLServerSelectForm()
        {
            InitializeComponent();
            v = treeView1;
            v.BorderStyle = BorderStyle.None;
            Load();
            adv = labelEx1;
            adv.Click += Adv_Click;
            lb = labelLine1;
            lb.SetParent(tabPage2);
            lbb = labelLine2;
            lbb.SetParent(tabPage2);
            lbc = labelLine3;
            lbc.SetParent(tabPage2);
            tc = tabControl1;
            this.BackColor = Color.White;
            tc.DrawMode = TabDrawMode.OwnerDrawFixed;

            tc.DrawItem += Tc_DrawItem;
        }

        // The size of the X in each tab's upper right corner.
        private int Xwid = 5;

        private const int tab_margin = 0;

        private void Tc_DrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle r = tabControl1.GetTabRect(tabControl1.TabPages.Count - 1);
            Rectangle rs = new Rectangle(0, r.Y - 5, tabControl1.Width - 3, 6);
            Rectangle rf = new Rectangle(r.X + r.Width, r.Y - 5, tabControl1.Width - (r.X + r.Width) - 3, r.Height + 5 + 5);
            Brush b = Brushes.White;
            e.Graphics.FillRectangle(b, rf);
            e.Graphics.FillRectangle(b, rs);

            Brush txt_brush, box_brush;
            Pen box_pen;
            e.Bounds.Inflate(3, 3);
            // We draw in the TabRect rather than on e.Bounds
            // so we can use TabRect later in MouseDown.
            Rectangle tab_rect = e.Bounds;// tc.GetTabRect(e.Index);

            // Draw the background.
            // Pick appropriate pens and brushes.
            if (e.State == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(Brushes.Orange, tab_rect);
                //e.DrawFocusRectangle();

                txt_brush = Brushes.Black;
                box_brush = Brushes.Black;
                box_pen = Pens.DarkBlue;
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, tab_rect);

                txt_brush = Brushes.Black;
                box_brush = Brushes.Black;
                box_pen = Pens.DarkBlue;
            }

            // Allow room for margins.
            RectangleF layout_rect = new RectangleF(
                tab_rect.Left + tab_margin,
                tab_rect.Y + tab_margin,
                tab_rect.Width - 2 * tab_margin,
                tab_rect.Height - 2 * tab_margin);
            using (StringFormat string_format = new StringFormat())
            {
                Font small_font = this.Font;
                // Draw the tab # in the upper left corner.
                // using (Font small_font = new Font(this.Font.FontFamily,
                //     6, FontStyle.Bold))
                // {
                //    string_format.Alignment = StringAlignment.Near;
                //    string_format.LineAlignment = StringAlignment.Near;
                //    e.Graphics.DrawString(
                //        e.Index.ToString(),
                //        small_font,
                //        txt_brush,
                //        layout_rect,
                //        string_format);
                //}

                // Draw the tab's text centered.
                //using (Font big_font = new Font(this.Font, FontStyle.Bold))
                {
                    string_format.Alignment = StringAlignment.Center;
                    string_format.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString(
                        tc.TabPages[e.Index].Text,
                        small_font,
                        txt_brush,
                        layout_rect,
                        string_format);
                }

                // Draw a line in the bottom.
                Rectangle rect = tab_rect;// tc.GetTabRect(e.Index);

                e.Graphics.DrawLine(box_pen,
                    layout_rect.Left + Xwid,
                    layout_rect.Bottom + Xwid,
                    layout_rect.Right - 2 * Xwid,
                    layout_rect.Bottom + Xwid);
            }
        }

        private void Adv_Click(object sender, EventArgs e)
        {
            AdvancedProperiesForm apf = new AdvancedProperiesForm();
            SqlConnection sql = new SqlConnection();
            SqlConnectionStringBuilder s = new SqlConnectionStringBuilder();
            apf.SetPropertyObject(s);
            apf.ShowDialog();
        }

        private TreeView v { get; set; }

        private LabelEx adv { get; set; }

        private LabelLine lb { get; set; }

        private LabelLine lbb { get; set; }

        private LabelLine lbc { get; set; }

        private TabControl tc { get; set; }

        public DataConnection dc { get; set; }

        public void Load()
        {
            TreeNode node = new TreeNode();
            node.Text = "Local";
            v.Nodes.Add(node);
            ArrayList C = DataSourceWizard.GetConnections();
            foreach (string c in C)
            {
                if (String.IsNullOrEmpty(c))
                    continue;
                TreeNode nodes = new TreeNode();
                nodes.Text = c;
                nodes.ImageKey = "datasource";
                node.Nodes.Add(nodes);
                DataConnection d = new DataConnection();
                d.Load(c);
                nodes.Tag = d;
            }
            node = new TreeNode();
            node.Text = "Network";
            v.Nodes.Add(node);
            node = new TreeNode();
            node.Text = "Remote";
            v.Nodes.Add(node);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode node = v.SelectedNode;
            if (node == null)
                return;
            dc = node.Tag as DataConnection;
            if (dc == null)
                return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
        }
    }

    public class LabelLine : Label
    {
        public LabelLine() : base()
        {
            this.AutoSize = false;
            this.BorderStyle = BorderStyle.Fixed3D;
            this.Height = 2;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        }

        public void SetParent(Control c)
        {
            this.Width = c.Width - 20;
            this.Height = 2;
        }
    }

    public class LabelEx : Label
    {
        public LabelEx() : base()
        {
            font = this.Font;
            this.Font = font;
            fonts = new Font(this.Font, FontStyle.Underline);
            this.ForeColor = Color.Blue;
            this.MouseHover += LabelEx_MouseHover;
            this.MouseLeave += LabelEx_MouseLeave;
        }

        public void SetMouseOverFont(Font fnt)
        {
            fonts = fnt;

            this.Font = font;
            this.Refresh();
        }

        public void SetMouseLeaveFont(Font fnt)
        {
            font = fnt;
        }

        private void LabelEx_MouseLeave(object sender, EventArgs e)
        {
            //if (!this.Font.Equals(font))
            this.Font = font;
            this.Refresh();
        }

        private void LabelEx_MouseHover(object sender, EventArgs e)
        {
            //if (this.Font.Equals(font))
            this.Font = fonts;
            this.Refresh();
        }

        private Font font { get; set; }
        private Font fonts { get; set; }
    }

    public class SearchTextBox : TextBox
    {
        public SearchTextBox()
        {
            SuspendLayout();

            this.Width = 200;

            pb = new PictureBox();
            pb.Bounds = new Rectangle(1, 1, 15, 15);
            pb.Image = new Bitmap(ve_resource.Search_16x, 13,13);
            pb.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            pb.BorderStyle = BorderStyle.None;
            this.Controls.Add(pb);

            tb = new TextBox();
            tb.Bounds = new Rectangle(17, 1, 184, 15);
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            tb.BorderStyle = BorderStyle.None;
            this.Controls.Add(tb);

            tb.Enter += Tb_Enter;

        

            ResumeLayout();
        }

        

        private void Tb_Enter(object sender, EventArgs e)
        {
            tb.Text = "";
        }

        private PictureBox pb { get; set; }

        public TextBox tb { get; set; }

        

        public string GetText()
        {
            return tb.Text;
        }
        public void SetText(string s)
        {
            tb.Text = s;
        }
        public void SetTextColor(Color c)
        {
            tb.ForeColor  = c;
        }
    }
}