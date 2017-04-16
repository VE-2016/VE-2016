using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer.UI.Views
{
    public partial class DesignerForm : Form
    {
        public DesignerForm()
        {
            InitializeComponent();
            Init();
            v.Resize += V_Resize;
            V_Resize(this, new EventArgs());
            //this.imageBox1.MouseDoubleClick += ImageBox1_MouseDoubleClick;
        }

        //private void ImageBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    this.imageBox1.ToBitmap();
        //    this.imageBox1.UnloadControls(false);
        //}

        private void V_Resize(object sender, EventArgs e)
        {
            v.Columns[0].Width = v.Width - 5;
        }

        ViewList v { get; set; }

        ArrayList models { get; set; }

        ImageList img { get; set; }

        ContextMenuStrip context { get; set; }

        public ViewList AddTestListView()
        {
            ViewList v = new ViewList();
            v.GridLines = true;
            v.View = View.Details;
            v.BackColor = Color.White;
            v.Columns.Add("name");
            v.LabelEdit = true;
            v.FullRowSelect = true;
            v.context = context;

            v.HeaderStyle = ColumnHeaderStyle.Clickable;
            v.Size = new Size(200, 200);
            v.Columns[0].Width = v.Width - 5;
            v.Columns[0].ImageKey = "TableAdapter_16x";
            v.LargeImageList = img;

            v.context = context;

            int i = 0;
            while (i < 6)
            {
                ListViewItem b = new ListViewItem("Text item");
                v.Items.Add(b);
                i++;
            }

            return v;
        }
        public ViewList AddListView(DataTable table)
        {
            ViewList v = new ViewList();
            v.GridLines = true;
            v.View = View.Details;
            v.BackColor = Color.White;
            v.Columns.Add("name");
            v.LabelEdit = true;
            v.FullRowSelect = true;
            v.context = context;

            v.HeaderStyle = ColumnHeaderStyle.Clickable;
            v.Size = new Size(200, 200);
            v.Columns[0].Width = v.Width - 5;
            v.Columns[0].ImageKey = "TableAdapter_16x";
            v.LargeImageList = img;

            v.context = context;

            int i = 0;
            while (i < table.Columns.Count)
            {
                ListViewItem b = new ListViewItem(table.Columns[i].ColumnName);
                v.Items.Add(b);
                i++;
            }

            return v;
        }
        public ViewList AddListViewAdapter(DataTable table)
        {
            ViewList v = new ViewList();
            v.GridLines = true;
            v.View = View.Details;
            v.BackColor = Color.White;
            v.Columns.Add("name");
            v.LabelEdit = true;
            v.FullRowSelect = true;
            v.context = context;

            v.HeaderStyle = ColumnHeaderStyle.Clickable;
            v.Size = new Size(200, 100);
            v.Columns[0].Width = v.Width - 5;
            v.Columns[0].ImageKey = "TableAdapter_16x";
            v.LargeImageList = img;

            v.context = context;

            int i = 0;
            while (i < 1)
            {
                ListViewItem b = new ListViewItem("Get,Fill()");
                v.Items.Add(b);
                i++;
            }

            return v;
        }
        public void Init()
        {

            context = contextMenuStrip1;

            models = new ArrayList();

            imageBox1.Image = new Bitmap(6000, 6000);
            using (TextureBrush brush = new TextureBrush(Resources.designer, WrapMode.Tile))
            {
                using (Graphics g = Graphics.FromImage(imageBox1.Image))
                {
                    g.FillRectangle(brush, 0, 0, imageBox1.Image.Width, imageBox1.Image.Height);
                }
            }

            img = new ImageList();
            img.Images.Add("TableAdapter_16x", Resources.TableAdapter_16x);
            img.Images.Add("DataTable_256x", Resources.DataTable_256x);


            v = new ViewList();
            v.GridLines = true;
            v.View = View.Details;
            v.BackColor = Color.White;
            v.Columns.Add("name");
            v.LabelEdit = true;
            v.FullRowSelect = true;
            v.context = context;

            v.HeaderStyle = ColumnHeaderStyle.Clickable;
            v.Size = new Size(200, 200);
            v.Columns[0].Width = v.Width - 5;
            v.Columns[0].ImageKey = "DataTable_256x";
            v.LargeImageList = img;
            

            int i = 0;
            while (i < 6)
            {
                ListViewItem b = new ListViewItem("Text item");
                v.Items.Add(b);
                i++;
            }


            ModelPanel p = new ModelPanel(new Size(6000,6000));

            p.control = imageBox1;

            p.AddControl(v);

            p.Bounds = new Rectangle(0, 0, 200, 200);

            //v.SetModelPanel( p );

            //this.imageBox1.Controls.Add(p);

          

            //ViewList bb = AddTestListView();

            //bb.modelPanel = p;

            //p.AddControl(bb);

            //bb.Refresh();

            //ModelPanel pc = null;

            //int guid = 0;

            //for(int ax = 0; ax < 30; ax++)
            //    for(int bx = 0; bx < 15; bx++)
            //    {

            //        ModelPanel pp = InitModelPanel();
            //        pp.Location = new Point(ax * 250, bx * 500);

            //        ViewList v = pp.panels[0] as ViewList;

            //        if (pc != null)
            //        {

            //            Connector c = ModelPanel.Connect(pc, pp, guid++);

            //            pp.AddConnector(c);

            //        }

            //        pc = pp;

            //        v.SetModelPanel(pp);
            //        this.imageBox1.Controls.Add(pp);
            //        pp.Refresh();

            //    }

            this.imageBox1.Draw();

        }

        public void LoadModel(DataSet dd)
        {
            

            int ax = 1;
            int bx = 1;

            foreach (DataTable d in dd.Tables)
            {

                ModelPanel pp = InitModelPanel(d);

                pp.Location = new Point(ax * 250, bx * 500);

                //ViewList v = pp.panels[0] as ViewList;

                //if (pc != null)
                //{

                //    Connector c = ModelPanel.Connect(pc, pp, guid++);

                //    pp.AddConnector(c);

                //}

                //pc = pp;

         //pp.AddControl(bb);

         //               ViewList bb = AddListView(d);

         //       bb.modelPanel = pp;

         //      v.SetModelPanel(pp);
                this.imageBox1.Controls.Add(pp);
                pp.Refresh();

                ax += 1;
                if(ax > 6)
                {
                    ax = 1;
                    bx++;
                }

            }
            imageBox1.Draw();
        }

        public ModelPanel InitModelPanel()
        {


            ModelPanel p = new ModelPanel(new Size(600,600));

            p.control = imageBox1;

            //ViewList v = new ViewList();
            //v.GridLines = true;
            //v.View = View.Details;
            //v.BackColor = Color.White;
            //v.Columns.Add("name");
            //v.LabelEdit = true;
            //v.FullRowSelect = true;
            //v.context = context;

            //v.HeaderStyle = ColumnHeaderStyle.Clickable;
            //v.Size = new Size(200, 200);
            //v.Columns[0].Width = v.Width - 5;
            //v.Columns[0].ImageKey = "DataTable_256x";
            //v.LargeImageList = img;

            //int i = 0;
            //while (i < 6)
            //{
            //    ListViewItem b = new ListViewItem("Text item");
            //    v.Items.Add(b);
            //    i++;
            //}

            //p.AddControl(v);

            
            //this.imageBox1.Controls.Add(p);

            ViewList bb = AddTestListView();

            bb.modelPanel = p;

            p.AddControl(bb);

            return p;
        }
        public ModelPanel InitModelPanel(DataTable table)
        {


            ModelPanel p = new ModelPanel(new Size(600, 600));

            p.control = imageBox1;

            ViewList v = AddListView(table);
            v.modelPanel = p;
            p.AddControl(v);

            v = AddListViewAdapter(table);
            v.modelPanel = p;
            p.AddControl(v);
      
           
            
            return p;
        }
    }
}
