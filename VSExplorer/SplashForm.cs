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
            pb.Image = Resources.logo2;
        }

        private PictureBox pb { get; set; }
    }
}