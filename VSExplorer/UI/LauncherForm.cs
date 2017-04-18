using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class LauncherForm : Form
    {
        public LauncherForm()
        {
            InitializeComponent();

            lb = listBox1;

            lb.SelectedIndexChanged += Lb_SelectedIndexChanged;

            exe = textBox1;

            args = textBox2;

            fd = folderLister1;

            fd.LoadByClick = true;

            fd.open.Click += Open_Click;
        }

        private void Open_Click(object sender, EventArgs e)
        {
            string c = fd.tb.Text;

            exe.Text = c;
        }

        private void Lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = lb.SelectedIndex;
            if (i < 0)
                return;
            string c = lb.Items[i].ToString();

            string[] dd = c.Split("$".ToCharArray());

            if (dd.Length > 0)
                exe.Text = dd[0];
            if (dd.Length > 1)
                args.Text = dd[1];
        }

        public ArrayList P { get; set; }

        private ListBox lb { get; set; }

        private TextBox exe { get; set; }

        private TextBox args { get; set; }

        private FolderLister fd { get; set; }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists("Launchers.config") == false)
            {
                FileStream fs = File.Create("Launchers.config");
                fs.Close();
            }

            string file = textBox1.Text;

            string prms = textBox2.Text;

            string cc = File.ReadAllText("Launchers.config");

            string c = file + "$" + prms;

            cc = cc + "\n" + c;

            File.WriteAllText("Launchers.config", cc);

            LoadLaunchers();
        }

        public void LoadLaunchers()
        {
            if (File.Exists("Launchers.config") == false)
            {
                FileStream fs = File.Create("Launchers.config");
                fs.Close();
            }
            string[] cc = File.ReadAllLines("Launchers.config");

            lb.Items.Clear();

            foreach (string c in cc)
                lb.Items.Add(c);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists("Launchers.config") == false)
            {
                FileStream fs = File.Create("Launchers.config");
                fs.Close();
            }
            string[] cc = File.ReadAllLines("Launchers.config");

            int i = lb.SelectedIndex;

            if (i < 0)
                return;

            string c = lb.Items[i].ToString();

            ArrayList L = new ArrayList();

            foreach (string d in cc)
            {
                if (d != c)
                    L.Add(d);
            }
            string s = "";
            foreach (string d in L)
            {
                s += d + "\n";
            }
            File.WriteAllText("Launchers.config", s);
            lb.Items.Clear();

            LoadLaunchers();
        }
    }
}