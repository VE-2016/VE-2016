using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SE_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadSE("", true);
            SELoader se = new SELoader();
            TreeView w = se.LoadProject("C:\\MSBuildProjects-beta\\VStudio.sln");
            List<TreeNode> ns = new List<TreeNode>();
            foreach (TreeNode node in w.Nodes)
                ns.Add(node);
            w.Nodes.Clear();
            foreach (TreeNode node in ns)
                v.Nodes.Add(node);
        }
        

        public ToolStrip ses { get; set; }

        public ToolStripButton sesf { get; set; }

        public ToolStripButton sesb { get; set; }

        public ToolStripSeparator sess { get; set; }

        public ToolStripButton sest { get; set; }

        public ToolStripButton sesc { get; set; }

        public ToolStripButton sesa { get; set; }

        public Navigator nav { get; set; }

        TreeViewEx v { get; set; }

        public void LoadSE(string name, bool load)
        {
           
            {

                v = new TreeViewEx();
                v.Dock = DockStyle.Fill;
              
                nav = new Navigator(v);

                ses = new ToolStrip();

                //this.Controls.Add(v);

                this.Controls.Add(ses);

                

                ses.GripStyle = ToolStripGripStyle.Hidden;

                sesb = new ToolStripButton();
                sesb.DisplayStyle = ToolStripItemDisplayStyle.Image;
                //sesb.Image = resource_alls.NavigateBackwards_62701;
                sesb.ToolTipText = "Backward";
                sesb.Click += Sest_Click;
                ses.Items.Add(sesb);

                sesf = new ToolStripButton();
                sesf.DisplayStyle = ToolStripItemDisplayStyle.Image;
                //sesf.Image = resource_alls.NavigateForward_62711;
                sesf.ToolTipText = "Forward";
                sesf.Click += Sesf_Click;
                ses.Items.Add(sesf);

                sess = new ToolStripSeparator();

                ses.Items.Add(sess);

                sest = new ToolStripButton();
                sest.DisplayStyle = ToolStripItemDisplayStyle.Image;
                //sest.Image = resource_alls.synchronize;
                sest.Click += Sest_Click;
                ses.Items.Add(sest);

                sesc = new ToolStripButton();
                sesc.DisplayStyle = ToolStripItemDisplayStyle.Image;
                //sesc.Image = resource_alls.collapse;
                sesc.ToolTipText = "Collapse";
                sesc.Click += Sesc_Click;
                ses.Items.Add(sesc);

                sesa = new ToolStripButton();
                sesa.DisplayStyle = ToolStripItemDisplayStyle.Image;
                //sesa.Image = resource_alls.showall;
                sesa.ToolTipText = "Show All";
                sesa.Click += Sesa_Click;
                ses.Items.Add(sesa);
                
                v.DrawMode = TreeViewDrawMode.OwnerDrawAll;
                v.DrawNode += V_DrawNode;
                v.ShowLines = false;
                v.HideSelection = false;

              
            }

          
        }
        private void V_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            v.OnDrawTreeNode(sender, e);
        }
        private void Sesf_Click(object sender, EventArgs e)
        {
            nav.Next();
        }

        private void Sesc_Click(object sender, EventArgs e)
        {
            v.CollapseAll();
        }

        private void Sesa_Click(object sender, EventArgs e)
        {
            v.ExpandAll();
        }

        private void Sest_Click(object sender, EventArgs e)
        {
            nav.Prev();
        }

        public static System.Drawing.Bitmap CombineBitmap(string[] files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

    }
}
