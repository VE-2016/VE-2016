using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        Bitmap Bm  {get; set;}

        string input = "file.png";

        string output = "file.png";

        PictureBox pb { get; set; }



        // Draw the picture.
        private void picImage_Paint(object sender, PaintEventArgs e)
        {
            if (Bm == null) return;
            e.Graphics.DrawImage(Bm, Offset, Offset);
        }

        // Set the transparent pixel.
        private void picImage_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the color clicked.
            Color color = Bm.GetPixel(e.X - Offset, e.Y - Offset);
            // Make that color transparent.
            Bm.MakeTransparent(color);
            // Show the result.
            pb.Refresh();
        }



        // Save the file.
        private void mnuFileSave_Click(object sender, EventArgs e)
        {

            

            //if (sfdFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bm.Save(sfdFile.FileName, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

    }
}
