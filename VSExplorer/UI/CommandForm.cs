using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class Commands : Form
    {
        public Commands(string c)
        {
            InitializeComponent();
            if (this.IsHandleCreated == false)
                this.CreateHandle();
            rb = richTextBox1;
            rb.AcceptsTab = true;
            rb.KeyDown += Rb_KeyDown;

            rb.KeyPress += Rb_KeyPress;
            _cmd.rb = rb;

            bg = SystemColors.ControlLight;

            rb.BackColor = bg;

            rb.ForeColor = Color.Black;

            folder = AppDomain.CurrentDomain.BaseDirectory;
            nav = new Navigators();
            string args = GetStartArgs(c);

            sp = splitContainer1;

            spd = splitContainer2;

            ViewDefined();

            ViewFolders();

            fd = folderLister1;

            fd.open.Click += Open_Click;

            bs = folderBrowser1;

            bs.fd = fd;

            _cmd.runAndInit(args);

            P = new ArrayList();

            LoadLaunchers();

            Focus();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            string file = fd.tb.Text;

            string folder = file;

            if (file == "")
                return;

            if (File.Exists(file) == false)

                folder = Path.GetDirectoryName(file);

            _cmd.SendCommand("cd /d " + folder + "\n");
        }

        private SplitContainer sp { get; set; }

        private SplitContainer spd { get; set; }

        private FolderLister fd { get; set; }

        private FolderBrowser bs { get; set; }

        public void ViewDefined()
        {
            if (sp.Panel1Collapsed == true)
                sp.Panel1Collapsed = false;
            else sp.Panel1Collapsed = true;
        }

        public void ViewFolders()
        {
            if (spd.Panel2Collapsed == true)
                spd.Panel2Collapsed = false;
            else spd.Panel2Collapsed = true;
        }

        public void ViewCmds()
        {
        }

        private void Rb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
                e.Handled = true;
        }

        private Navigators nav { get; set; }

        private void Rb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                string c = nav.Prev();

                //cmd.SendCommand(c);

                AddText(c);

                e.Handled = true;
            }
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Back)
            {
                //string c = nav.Next();

                //cmd.SendCommand(c);

                //AddText(c);

                CharChanged = true;

                int p = rb.SelectionStart;

                //if (rb.Text[p] == '>')
                //    e.Handled = true;
                if (rb.Text[p - 1] == '>')
                    e.Handled = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                string c = nav.Next();

                AddText(c);

                e.Handled = true;
            }
            if (e.KeyCode == Keys.Tab)
            {
                RemoveWhitespaces();

                string s = "";

                if (HasChanged == true)
                {
                    s = GetFolder();
                    HasChanged = false;
                }
                else
                    s = GetNext();

                if (s != "")
                    AddText(s);

                RemoveWhitespaces();

                rb.Select(rb.Text.Length, 0);

                e.Handled = true;
            }
            if (e.KeyCode == Keys.Return)
            {
                _enterPressed = true;

                HasChanged = true;

                CharChanged = false;

                if (_cmd.running == false)
                    return;

                string[] cc = rb.Text.Split("\n".ToCharArray());

                _input = cc[cc.Length - 1];

                if (_input == "")
                    _input = cc[cc.Length - 2];

                cc = _input.Split(">".ToCharArray());

                string cs = "";

                if (cc.Length == 1)
                    cs = cc[0];
                else if (cc.Length > 1)
                    cs = cc[1];
                _cmd.SendCommand(cs);

                if (cs != "")
                    nav.Add(cs);
            }
        }

        public void SendSet()
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;
            _cmd.SendCommand("set > " + s + "tmp.tmp");
        }

        public Navigators navs { get; set; }

        public bool HasChanged = true;

        public bool CharChanged = false;

        public void RemoveWhitespaces()
        {
            rb.Text = rb.Text.TrimEnd();
        }

        public string GetFolder()
        {
            CharChanged = false;

            navs = new Navigators();

            string[] cc = rb.Text.Split("\n".ToCharArray());

            string s = cc[cc.Length - 1];

            cc = s.Split(">".ToCharArray());

            s = cc[0];

            if (cc.Length <= 1)
                return "";

            string b = cc[1];

            if (s == "")
                return "";

            string[] dd = Directory.GetDirectories(s);
            foreach (string g in dd)
            {
                string d = g;
                string[] bb = d.Split("\\".ToCharArray());
                navs.Add(bb[bb.Length - 1]);
            }

            string[] ff = Directory.GetFiles(s);
            foreach (string g in ff)
            {
                string d = Path.GetFileName(g);
                navs.Add(d);
            }

            navs.Sort();

            s = navs.Get(b);

            return s;
        }

        public string GetText()
        {
            string[] cc = rb.Text.Split("\n".ToCharArray());

            string s = cc[cc.Length - 1];

            cc = s.Split(">".ToCharArray());

            s = cc[0];

            string b = cc[1];

            return b;
        }

        public string GetNext()
        {
            string c = "";

            if (CharChanged == true)
            {
                c = GetText();
                if (c == "")
                {
                    CharChanged = false;
                    HasChanged = true;
                }
            }

            string s = navs.Get(c);

            return s;
        }

        public void AddText(string s)
        {
            string[] cc = rb.Text.Split("\n".ToCharArray());

            string c = cc[cc.Length - 1];

            string d = c + s;

            d = d.Replace("\n", "");

            // rb.Text.Replace(c, d);

            while (rb.Text.EndsWith(">") == false)
                rb.Text = rb.Text.Substring(0, rb.Text.Length - 1);

            rb.Text += s;

            if (rb.Text.EndsWith("\t") == true)
                rb.Text = rb.Text.Substring(0, rb.Text.Length - 1);

            rb.Select(rb.Text.Length, 0);
        }

        public void SetFolder(string s)
        {
            string c = Path.GetDirectoryName(s);

            string d = Path.GetPathRoot(c);

            _cmd.SendCommand("cd /d " + d);

            _cmd.SendCommand("cd " + c + "\n");

            Focus();
        }

        public string GetStartArgs(string s)
        {
            string c = Path.GetDirectoryName(s);

            string d = Path.GetPathRoot(c);

            string args = "/K cd /D " + c;

            return args;
        }

        private string _input = "";
        private bool _enterPressed = false;
        private RichTextBox rb { get; set; }
        private CMD _cmd = new CMD();

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string[] cc = rb.Text.Split("\n".ToCharArray());

            _input = cc[cc.Length - 1];

            cc = _input.Split(">".ToCharArray());

            if (cc.Length == 1)
                _cmd.SendCommand(cc[0]);
            else
                _cmd.SendCommand(cc[1]);
        }

        public void FindVSCmds()
        {
            DirectoryInfo d = new DirectoryInfo("C:\\Program Files (x86)");

            DirectoryInfo[] dd = d.GetDirectories("Microsoft Visual Studio*");

            string v2017 = ExplorerForms.GetVisualStudio2017InstalledPath();

            if (!string.IsNullOrEmpty(v2017))
                if (Directory.Exists(v2017))
                {

                    Array.Resize(ref dd, dd.Length + 1);
                    dd[dd.Length - 1] = new DirectoryInfo(v2017);
                }
            foreach (DirectoryInfo c in dd)
            {
                DirectoryInfo[] cc = c.GetDirectories("Common7*");

                foreach (DirectoryInfo ca in cc)
                {
                    DirectoryInfo[] b = ca.GetDirectories("Tools");

                    if (b.Length > 0)
                    {
                        string[] pp = b[0].FullName.Split("\\".ToCharArray());

                        foreach (string s in pp)
                        {
                            if (s.StartsWith("Microsoft"))
                            {
                                string[] rr = s.Split(" ".ToCharArray());

                                AddToolButton(b[0].FullName, rr[rr.Length - 1]);
                            }
                        }

                        //AddToolButton(b[0].FullName);
                    }
                }
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            FindVSCmds();
        }

        public void AddToolButton(string s, string v)
        {
            ToolStripButton b = new ToolStripButton();
            b.Text = v;// Path.GetFileName(s);
            b.DisplayStyle = ToolStripItemDisplayStyle.Text;
            b.Tag = s;
            b.Click += B_Click;

            toolStrip1.Items.Add(b);
        }

        public void AddToolLauncherButton(string s, string v)
        {
            s = s.Replace("$", "");
            ToolStripButton b = new ToolStripButton();
            b.Text = v;// Path.GetFileName(s);
            b.DisplayStyle = ToolStripItemDisplayStyle.Text;
            b.Tag = s;
            b.Click += C_Click;

            toolStrip1.Items.Add(b);
        }

        private void B_Click(object sender, EventArgs e)
        {
            ToolStripButton b = sender as ToolStripButton;
            string s = b.Tag as string;
            //MessageBox.Show("VSCmds at " + s);

            string c = "cd /d c:\\";

            _cmd.SendCommand(c);

            Task.Delay(2000);

            c = "cd " + s;

            _cmd.SendCommand(c);

            Task.Delay(2000);

            c = "VsDevCmd.bat";

            _cmd.SendCommand(c);

            SendSet();
        }

        private void C_Click(object sender, EventArgs e)
        {
            ToolStripButton b = sender as ToolStripButton;
            string s = b.Tag as string;
            //MessageBox.Show("VSCmds at " + s);

            string folder = Path.GetDirectoryName(s);

            folder = folder.Replace("$", "");

            string c = "cd /d " + folder;

            _cmd.SendCommand(c);

            Task.Delay(2000);

            c = s;

            c = c.Replace("$", "");

            _cmd.SendCommand(c);
        }

        private string folder { get; set; }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string drive = Path.GetPathRoot(folder);

            _cmd.SendCommand("cd /d " + drive);

            _cmd.SendCommand("cd " + folder);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            _cmd.runCommands(_input);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            //string drive = "C:\\";

            string folder = "C:\\Program Files\\Git\\bin";

            _cmd.runCommandsExternal(folder + "\\bash.exe");

            //cmd.SendCommand("cd /d " + drive);

            //cmd.SendCommand("cd " + folder);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            string drive = "C:\\";

            string folder = "C:\\Program Files\\Git\\bin";

            _cmd.SendCommand("cd /d " + drive);

            _cmd.SendCommand("cd " + folder);

            HasChanged = true;

            CharChanged = false;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //string drive = "C:\\";

            //string folder = "C:\\WinPython";

            //cmd.SendCommand("cd /d " + drive);

            //cmd.SendCommand("cd " + folder);

            //HasChanged = true;

            //CharChanged = false;

            //cmd.runCommands("C:\\WinPython\\python-3.4.3.amd64\\python.exe");

            _cmd.runCommands("C:\\WinPython\\python-3.4.3.amd64\\python.exe");
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            _cmd.EndProcess();
        }

        private Font font;

        private Color bg = Color.Black;

        private ArrayList R = new ArrayList();

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            R = new ArrayList();

            ConsolePropertyForm pf = new ConsolePropertyForm();
            pf.R = R;
            DialogResult r = pf.ShowDialog();
            if (r != DialogResult.OK)
                return;

            if (R == null)
                return;
            if (R.Count <= 0)
                return;

            try
            {
                font = R[0] as Font;
            }
            catch (Exception ex)
            {
            }

            rb.Font = font;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            ViewDefined();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            ViewFolders();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            LauncherForm lf = new LauncherForm();
            lf.ShowDialog();
            DialogResult r = lf.DialogResult;
            if (r != DialogResult.OK)
                return;
        }

        private ArrayList P { get; set; }

        public void LoadLaunchers()
        {
            if (File.Exists("Launchers.config") == false)
            {
                FileStream fs = File.Create("Launchers.config");
                fs.Close();
            }
            string[] cc = File.ReadAllLines("Launchers.config");

            foreach (ToolStripButton b in P)
                toolStrip1.Items.Remove(b);

            foreach (string c in cc)
            {
                string s = c.Replace("$", "");

                AddToolLauncherButton(s, Path.GetFileName(s));
            }
        }
    }
}