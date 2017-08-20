using System.Windows.Forms;

namespace WinExplorer
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
            pb = pictureBox1;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Image = ve_resource.WindowsLogo_16x;
        }

        private PictureBox pb { get; set; }
    }
}