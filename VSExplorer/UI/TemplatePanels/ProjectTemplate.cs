using System;
using System.IO;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class ProjectTemplate : UserControl
    {
        public ProjectTemplate()
        {
            InitializeComponent();
            SetOutputLocation();
        }

        public void SetOutputLocation()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            textBox3.Text = name;
            textBox3.Enabled = false;
            textBox2.Text = "No description given";
        }

        public VSProject pp { get; set; }

        public void SetProjectTemplate(VSProject p)
        {
            pp = p;

            if (pp == null)
                return;

            textBox1.Text = pp.Name;
        }

        private string _loc = "TemplateProjects";

        public void MakeProjectTemplate()
        {
            string folder = textBox1.Text;

            string folders = AppDomain.CurrentDomain.BaseDirectory;

            string target = folders + _loc + "\\" + folder;

            string source = Path.GetDirectoryName(pp.FileName);

            CopyAll(new DirectoryInfo(source), new DirectoryInfo(target));
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists; if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}