using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinExplorer.UI.Views
{
    public class ViewList : ListView
    {
        public ViewList()
        {
            this.OwnerDraw = true;
            this.DoubleBuffered = true;

            this.HideSelection = true;

            this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(MyListView_DrawColumnHeader);
            this.DrawItem += new DrawListViewItemEventHandler(MyListView_DrawItem);
            //this.DrawSubItem += new DrawListViewSubItemEventHandler(MyListView_DrawSubItem);
            this.ColumnWidthChanging += ViewList_ColumnWidthChanging;
            this.ColumnClick += ViewList_ColumnClick;
            this.MouseDown += ViewList_MouseDowns;
            this.MouseClick += ViewList_MouseClick;
            this.Leave += ViewList_LostFocus;
        }

        public ContextMenuStrip context { get; set; }

        private void ViewList_LostFocus(object sender, EventArgs e)
        {
            // if(editbox != null && editbox.Focused != true)
            {
                //this.Controls.Remove(editbox);
                //editbox = null;
                //this.SelectedIndices.Clear();
                modelPanel.Unhighlight();
                this.Refresh();
            }
        }

        private void ViewList_MouseDowns(object sender, MouseEventArgs e)
        {
            if (editbox != null)
                this.Controls.Remove(editbox);
            editbox = null;
        }

        private void ViewList_MouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem b = GetItemAt(e.X, e.Y);
            if (b == null)
                return;

            editbox = new TextBox();
            editbox.Location = new Point(24, b.Bounds.Y);
            editbox.Size = new Size(b.Bounds.Width - 24, b.Bounds.Height);
            editbox.Tag = b;
            this.Controls.Add(editbox);
            editbox.Focus();
            editbox.Text = b.SubItems[0].Text;
            editbox.SelectionStart = 0;
            editbox.SelectionLength = editbox.Text.Length;

            this.Refresh();
        }

        public TextBox editbox { get; set; }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.editbox != null && this.editbox.Focused == true)
            {
                if (keyData == Keys.Up)
                {
                    if (SelectedIndices[0] != 0)
                        modelPanel.HandleHighlight(this, keyData);
                }
                else if (keyData == Keys.Down)
                {
                    modelPanel.HandleHighlight(this, keyData);
                }
                else if (keyData == Keys.Enter)
                {
                    ListViewItem b = editbox.Tag as ListViewItem;
                    if (b != null)
                    {
                        b.SubItems[0].Text = editbox.Text;
                        b.Selected = false;
                    }

                    this.Controls.Remove(editbox);
                    this.Refresh();
                    return false;
                }
            }

            //  if (SelectedIndices.Count <= 0)
            //      return false;

            if (keyData == Keys.Up)
            {
                if ((SelectedIndices.Count > 0 && SelectedIndices[0] == 0) || SelectedIndices.Count == 0)
                    modelPanel.HandleArrows(this, keyData);

                return false;
            }
            else if (keyData == Keys.Down)
            {
                if ((SelectedIndices.Count > 0 && SelectedIndices[0] == Items.Count - 1) || SelectedIndices.Count == 0)
                    modelPanel.HandleArrows(this, keyData);

                return false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ViewList_MouseDown(object sender, MouseEventArgs e)
        {
            Point cc = this.PointToClient(MousePosition);

            if ((e.Button == MouseButtons.Right))
            {
                context.Show(this, new Point(50, 20));

                return;
            }

            if (cc.Y < 28)
            {
                mouse = MousePosition;
                moving = true;

                modelPanel.Unhighlight();

                modelPanel.Refresh();

                modelPanel.Visible = true;
                Bitmap image = new Bitmap(this.modelPanel.Size.Width, this.modelPanel.Size.Height);
                modelPanel.DrawToBitmap(image, new Rectangle(0, 0, modelPanel.Size.Width, modelPanel.Size.Height));
                panel.Image = image;
                panel.Bounds = this.modelPanel.Bounds;

                modelPanel.Visible = false;

                panel.Visible = true;

                panel.Refresh();

                panel.Capture = true;

                //modelPanel.BringToFront();

                //this.Refresh();

                return;
            }
        }

        private ImageBox panel { get; set; }

        public void SetModelPanel(ModelPanel p)
        {
            Size sz = new Size(6000, 6000);
            modelPanel = p;
            panel = new ImageBox(sz);
            panel.Bounds = new Rectangle(p.Location.X + 25, p.Location.Y + 2, 140, 14);
            panel.Image = null;
            panel.BackColor = Color.LightGray;
            panel.Visible = true;
            this.modelPanel.control.Controls.Add(panel);
            this.panel.MouseMove += ViewList_MouseMove;
            this.panel.MouseUp += ViewList_MouseUp;
            this.panel.MouseDown += ViewList_MouseDown;
        }

        private void ViewList_MouseUp(object sender, MouseEventArgs e)
        {
            if (moving == false)
                return;
            moving = false;
            panel.Visible = false;
            panel.Capture = false;
            modelPanel.Bounds = panel.Bounds;

            panel.Location = modelPanel.Location;
            panel.BackColor = Color.LightGray;
            panel.Bounds = new Rectangle(modelPanel.Location.X + 25, modelPanel.Location.Y + 2, 140, 14);
            panel.Image = null;
            panel.Visible = true;
            //panel.Visible = true;
            //modelPanel.BringToFront();
            modelPanel.Visible = true;
            modelPanel.Refresh();
        }

        private void ViewList_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving == false)
                return;

            if (e.Button != MouseButtons.Left)
                return;

            Point cc = MousePosition;

            Point dd = mouse;

            mouse = MousePosition;

            //Point de = new Point(cc.X - dd.X, cc.Y - dd.Y);

            if (mouse == null)
                mouse = cc;

            Point delta = new Point(cc.X - dd.X, cc.Y - dd.Y);

            // if (delta.X < 0)
            //     MessageBox.Show("");

            panel.Location = new Point(panel.Location.X + delta.X, panel.Location.Y + delta.Y);

            //modelPanel.Location = new Point(panel.Location.X + delta.X, panel.Location.Y + delta.Y);

            panel.Refresh();

            panel.Focus();
        }

        private Point mouse { get; set; }

        public ModelPanel modelPanel { get; set; }

        public bool moving = false;

        private void ViewList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (panel != null)
                this.panel.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);

            this.active = true;

            if (expanded)
            {
                this.Height = 26;
                this.Scrollable = false;
                expanded = false;
            }
            else
            {
                int c = this.Items.Count;
                this.Height = (c + 1) * 20;
                this.Scrollable = true;
                expanded = true;
            }

            if (modelPanel == null)
                return;
            modelPanel.Measure();
            modelPanel.Refresh();
        }

        public bool expanded = true;

        private void ViewList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }

        private bool active = false;

        public void SetActive(bool c = true)
        {
            active = c;
        }

        public bool IsActive()
        {
            return active;
        }

        private void MyListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            // Not interested in changing the way columns are drawn - this works fine
            //e.DrawDefault = true;
            if (active)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.ActiveCaption)), e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds);
            if (e.Header.ListView.LargeImageList != null)
                e.Graphics.DrawImage(new Bitmap(e.Header.ListView.LargeImageList.Images[e.Header.ImageKey], new Size(17, 17)), 0, 0);

            //e.Graphics.DrawImage(new Bitmap(Properties.ve_resource.ExpandArrow_16x, new Size(15, 15)), e.Bounds.X + e.Bounds.Width - 20, 0);
        }

        private void MyListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawBackground();

            e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), 0, e.Bounds.Y, 20, e.Bounds.Height + 3);
            if (e.Item.Selected)
            {
                if ((editbox != null && MouseButtons == MouseButtons.Left) || editbox == null && MouseButtons != MouseButtons.Left)
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 255)), e.Bounds);
                e.Graphics.DrawString(e.Item.Text, new Font("Arial", 10), new SolidBrush(Color.White), new Rectangle(25, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
            }
            else
                e.Graphics.DrawString(e.Item.Text, new Font("Arial", 10), new SolidBrush(Color.Black), new Rectangle(25, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
        }

        //private void MyListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        //{
        //    if (e.ColumnIndex <= 0)
        //        return;
        //    string searchTerm = "Term";
        //    int index = e.SubItem.Text.IndexOf(searchTerm);
        //    if (index >= 0)
        //    {
        //        string sBefore = e.SubItem.Text.Substring(0, index);

        //        Size bounds = new Size(e.Bounds.Width, e.Bounds.Height);
        //        Size s1 = TextRenderer.MeasureText(e.Graphics, sBefore, this.Font, bounds);
        //        Size s2 = TextRenderer.MeasureText(e.Graphics, searchTerm, this.Font, bounds);

        //        Rectangle rect = new Rectangle(e.Bounds.X + s1.Width, e.Bounds.Y, s2.Width, e.Bounds.Height);
        //        e.Graphics.FillRectangle(new SolidBrush(Color.Yellow), rect);
        //    }

        //    e.DrawText();
        //}
    }

    internal class ImageBox : Panel
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        public ImageBox(Size sz)
        {
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(1000, 1000);
            this.DoubleBuffered = true;
            this.MouseDown += ImageBox_MouseClick;
            this.MouseMove += ImageBox_MouseMove;
            this.MouseUp += ImageBox_MouseUp;
        }

        private void ImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            tracking = false;
        }

        private Point delta { get; set; }

        private void ImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (tracking == false)
                return;

            Point d = new Point(e.X + this.HorizontalScroll.Value, e.Y + this.VerticalScroll.Value);

            Point dd = Point.Subtract(d, new Size(delta));

            cons.MoveSegment(dd);

            SetBitmap(Image);

            Draw();

            cons.Draw(Graphics.FromImage(Image), true);

            this.Refresh();

            delta = new Point(e.X + this.HorizontalScroll.Value, e.Y + this.VerticalScroll.Value);
        }

        public void SetBitmap(Image bmp)
        {
            using (Graphics gfx = Graphics.FromImage(bmp))
            using (SolidBrush brush = new SolidBrush(Color.FromKnownColor(KnownColor.Control)))
            {
                gfx.FillRectangle(brush, 0, 0, 6000, 6000);
            }
        }

        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }
        }

        private Connector cons { get; set; }

        private void ImageBox_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("Mouse clicked");

            Point c = e.Location;

            Point d = new Point(c.X + this.HorizontalScroll.Value, c.Y + this.VerticalScroll.Value);

            Bitmap bmp = Connectors.bmp;

            Color cc = bmp.GetPixel(d.X, d.Y);

            int guid = 256 * cc.G + cc.B;

            //MessageBox.Show("Guid = " + guid.ToString());

            // byte[] bytes = BitmapToByteArray(bmp);

            if (guid <= 0)
                return;

            if (!Connectors.dict.ContainsKey(guid))
                return;

            cons = Connectors.dict[guid];

            cons.Draw(Graphics.FromImage(Image), true);

            this.Refresh();

            delta = new Point(d.X, d.Y);

            tracking = true;
        }

        private bool tracking = false;

        private Image mImage;

        public Image Image
        {
            get { return mImage; }
            set
            {
                mImage = value;
                if (value == null) this.AutoScrollMinSize = new Size(0, 0);
                else
                {
                    var size = value.Size;
                    using (var gr = this.CreateGraphics())
                    {
                        size.Width = (int)(size.Width * gr.DpiX / value.HorizontalResolution);
                        size.Height = (int)(size.Height * gr.DpiY / value.VerticalResolution);
                    }
                    this.AutoScrollMinSize = size;
                }
                this.Invalidate();
            }
        }

        //public Image shadow { get; set; }

        //public void MakeShadow()
        //{
        //    shadow = new Bitmap(Image.Width, Image.Height);
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            //int x = this.HorizontalScroll.Value;
            //int y = this.VerticalScroll.Value;

            e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
            if (mImage != null) e.Graphics.DrawImage(mImage, 0, 0);
            //if (mImage != null) e.Graphics.DrawImage(mImage, this.Bounds, new Rectangle( - this.AutoScrollPosition.X, - this.AutoScrollPosition.Y, this.Width, this.Height), GraphicsUnit.Pixel);
            base.OnPaint(e);
        }

        private Bitmap bmp { get; set; }

        public void ToBitmap()
        {
            int Width = this.AutoScrollMinSize.Width;
            int Height = this.AutoScrollMinSize.Height;
            //Bitmap bmp = new Bitmap(Width, Height);
            //bmp = this.DrawToBitmap(bmp);
            //Image = CaptureWindow(this.Handle, Width, Height);
            // Image = SaveBitmap(Width, Height);
            //Image = Print(Width, Height);
        }

        public void UnloadControls(bool load = false)
        {
            foreach (Control c in this.Controls)
                c.Visible = load;
        }

        private Bitmap DrawToBitmap(Bitmap bmp)
        {
            int Width = this.AutoScrollMinSize.Width;
            int Height = this.AutoScrollMinSize.Height;

            Cursor = Cursors.WaitCursor;         // yes it takes a while
            //Panel p = new Panel();               // the containing panel
            //Point oldLocation = ctl.Location;    //
            //p.Location = Point.Empty;            //
            //this.Controls.Add(p);                //

            int maxWidth = 2000;                 // you may want to try other sizes
            int maxHeight = 2000;                //

            Bitmap bmp2 = new Bitmap(maxWidth, maxHeight);  // the buffer

            //p.Height = maxHeight;               // set up the..
            //p.Width = maxWidth;                 // ..container

            //ctl.Location = new Point(0, 0);     // starting point
            //ctl.Parent = p;                     // inside the container
            //p.Show();                           //
            //p.BringToFront();                   //

            // we'll draw onto the large bitmap with G
            using (Graphics G = Graphics.FromImage(bmp))
                for (int y = 0; y < Height; y += maxHeight)
                {
                    //ctl.Top = -y;                   // move up
                    for (int x = 0; x < Width; x += maxWidth)
                    {
                        //ctl.Left = -x;             // move left
                        this.DrawToBitmap(bmp2, new Rectangle(x, y, maxWidth, maxHeight));
                        G.DrawImage(bmp2, x, y);   // patch together
                    }
                }

            // ctl.Location = p.Location;         // restore..
            // ctl.Parent = this;                 // form layout <<<==== ***
            // p.Dispose();                       // clean up

            Cursor = Cursors.Default;          // done

            return bmp;
        }

        public void DrawControl(Control control, Bitmap bitmap)
        {
            control.DrawToBitmap(bitmap, control.Bounds);
            foreach (Control childControl in control.Controls)
            {
                DrawControl(childControl, bitmap);
            }
        }

        public Bitmap SaveBitmap(int Width, int Height)
        {
            Bitmap bmp = new Bitmap(Width, Height, this.CreateGraphics());

            //this.l.DrawToBitmap(bmp, new Rectangle(0, 0, this.panel.Width, this.panel.Height));
            foreach (Control control in Controls)
            {
                DrawControl(control, bmp);
            }

            return bmp;
        }

        public void Draw()
        {
            Graphics g = Graphics.FromImage(Image);

            foreach (Control c in Controls)
            {
                ModelPanel p = c as ModelPanel;

                if (p == null)
                    continue;

                p.conrs.Draw(g);
            }
        }
    }

    public class ModelPanel : Panel
    {
        public ModelPanel(Size sz) : base()
        {
            panels = new ArrayList();
            this.Paint += ModelPanel_Paint;
            InitConnectors(sz);
        }

        public Connectors conrs { get; set; }

        public void InitConnectors(Size sz)
        {
            conrs = new Connectors(sz);
        }

        public void AddConnector(Connector c)
        {
            conrs.AddConnector(c);
        }

        private void ModelPanel_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.Blue, 3);
            e.Graphics.DrawRectangle(p, e.ClipRectangle);
        }

        public ArrayList panels { get; set; }

        public Control control { get; set; }

        public void AddControl(Control c)
        {
            this.Controls.Add(c);

            panels.Add(c);
            Measure();
        }

        public void Unhighlight()
        {
            foreach (ViewList b in Controls)
            {
                if (b.editbox != null)
                {
                    b.Controls.Remove(b.editbox);
                    b.editbox = null;
                    b.Refresh();
                }
                b.SelectedIndices.Clear();
            }
        }

        public void HandleHighlight(object sender, Keys e)
        {
            ViewList v = sender as ViewList;

            ListViewItem b = v.SelectedItems[0];

            SetActive(v);

            if (e == Keys.Up)
            {
                int index = b.Index;

                if (index == 0)
                    return;

                b.Selected = false;

                v.Items[index - 1].Selected = true;

                v.Controls.Remove(v.editbox);

                v.editbox = null;

                v.Refresh();

                return;
            }
            else if (e == Keys.Down)
            {
                int index = b.Index;

                if (index >= v.Items.Count - 1)
                    return;

                b.Selected = false;

                v.Items[index + 1].Selected = true;

                v.Controls.Remove(v.editbox);

                v.editbox = null;

                v.Refresh();

                return;
            }
        }

        public void HandleArrows(object sender, Keys e)
        {
            ViewList v = sender as ViewList;

            int index = Controls.IndexOf(v);

            if (e == Keys.Up)
            {
                if (index <= 0)
                    return;

                if (v.editbox != null && v.editbox.Focused)
                {
                    v.Items[index - 1].Selected = true;

                    v.Controls.Remove(v.editbox);

                    v.editbox = null;

                    v.Refresh();
                }

                v.Items[0].Selected = false;

                v.SelectedItems.Clear();

                v = Controls[index - 1] as ViewList;

                if (v == null)
                    return;

                v.SelectedItems.Clear();

                if (!v.IsActive())
                {
                    SetActive(v);
                    v.Refresh();
                }
                else
                {
                    v.Items[v.Items.Count - 1].Selected = true;
                    v.Focus();
                }
            }
            else if (e == Keys.Down)
            {
                if (index < 0)
                    return;
                if (index >= Controls.Count - 1)
                    index--;

                v.SelectedItems.Clear();

                if (v.editbox != null && v.editbox.Focused)
                {
                    //v.Items[index - 1].Selected = true;

                    v.Controls.Remove(v.editbox);

                    v.editbox = null;

                    v.Refresh();
                }

                v.Items[v.Items.Count - 1].Selected = false;

                v.Refresh();

                v = Controls[index + 1] as ViewList;

                if (v == null)
                    return;

                v.SelectedItems.Clear();

                if (!v.IsActive())
                {
                    SetActive(v);
                    v.Refresh();
                }
                else
                {
                    v.Items[0].Selected = true;

                    v.Focus();
                }
            }
        }

        public void SetActive(ViewList b)
        {
            foreach (ViewList v in Controls)
            {
                if (v.Equals(b))
                    v.SetActive();
                else v.SetActive(false);
                v.Refresh();
            }
        }

        public void RemoveControl(Control c)
        {
            this.Controls.Remove(c);
            panels.Remove(c);
            Measure();
        }

        public void Measure()
        {
            int w = this.Width - 1;
            int h = 0;

            foreach (Control c in panels)
            {
                Size s = c.Size;

                c.Bounds = new Rectangle(0, h, w, s.Height);

                h += s.Height + 1;
            }

            this.Height = h;
        }

        public static Connector Connect(ModelPanel p0, ModelPanel p1, int guid)
        {
            Size s0 = p0.Size;
            Point b0 = p0.Location;

            Size s1 = p1.Size;
            Point b1 = p1.Location;

            Point a = new Point(b0.X + s0.Width, b0.Y + s0.Height / 2);

            Point b = new Point(b1.X, b1.Y + s1.Height / 2);

            Connector c = Connector.Create(a, b, guid);

            return c;
        }
    }

    public class Connector
    {
        public static Pen pen = new Pen(Color.Blue, 3);

        public static Pen apen = new Pen(Color.Green, 3);

        public List<Point> pts { get; set; }

        public int guid = 0;

        public Point guids { get; set; }

        public Connector(int g)
        {
            pts = new List<Point>();
            guid = g;
            guids = new Point(guid / 256, guid % 256);
            Connectors.dict.Add(guid, this);
        }

        public void Draw(Graphics g, bool active = false)
        {
            if (!active)
                g.DrawLines(pen, pts.ToArray() as Point[]);
            else
                g.DrawLines(apen, pts.ToArray() as Point[]);

            if (!active)
            {
                Pen p = new Pen(Color.FromArgb(255, 0, guids.X, guids.Y), 3);

                Connectors.gr.DrawLines(p, pts.ToArray() as Point[]);
            }
        }

        public void FindSegment(Point p)
        {
        }

        public void MoveSegment(Point delta)
        {
            pts[0] = Point.Add(pts[0], new Size(delta));
            pts[1] = Point.Add(pts[1], new Size(delta));
        }

        public void AddPoint(Point a)
        {
            pts.Add(a);
        }

        public static Connector Create(Point a, Point b, int guid)
        {
            Connector c = new Connector(guid);

            c.AddPoint(a);
            c.AddPoint(b);

            return c;
        }
    }

    public class Connectors
    {
        static public Bitmap bmp { get; set; }

        static public Graphics gr { get; set; }

        static public Dictionary<int, Connector> dict { get; set; }

        public List<Connector> cts { get; set; }

        public Connectors(Size sz)
        {
            cts = new List<Connector>();
            if (bmp == null)
            {
                bmp = new Bitmap(sz.Width, sz.Height);
                gr = Graphics.FromImage(bmp);
                dict = new Dictionary<int, Connector>();
            }
        }

        public void AddConnector(Connector c)
        {
            cts.Add(c);
        }

        public void Draw(Graphics g)
        {
            foreach (Connector c in cts)
                c.Draw(g);
        }
    }
}