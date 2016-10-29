using System.Windows.Forms;

namespace WinExplorer
{
    public partial class AboutApplicationForm : Form
    {
        public AboutApplicationForm()
        {
            InitializeComponent();
            pictureBox1.Image = resource_vsc.image.ToBitmap();
            lbs = listBox2;
            lbt = listBox1;
            LoadOpenSource();
            LoadTech();
        }

        private ListBox lbs { get; set; }

        private ListBox lbt { get; set; }

        public void LoadTech()
        {
            lbt.Items.Clear();

            foreach (string s in _tec)
                lbt.Items.Add(s);
        }

        public void LoadOpenSource()
        {
            lbs.Items.Clear();

            foreach (string s in _ops)
                lbs.Items.Add(s);
        }

        private string[] _ops = new string[] { "Docking Panel Suite", "AIMS Library - document editing", "Anders Reflector", "GAC manager", "Mono Cecil decompiler project", "NRefactory CSharp parser" };

        private string[] _tec = new string[] { "Docking Panel structure", "Visual Studio solution opening, viewing, editing", "CSharp files parsing", "Reading .NET executables content" };
    }
}