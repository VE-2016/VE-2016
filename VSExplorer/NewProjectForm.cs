using AIMS.Libraries.CodeEditor;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class NewProjectForm : Form
    {
        public NewProjectForm()
        {
            InitializeComponent();
            SuspendLayout();
            tv = treeView2;
            tv.FullRowSelect = true;
            tv.ShowLines = false;
            lv = listView1;
            LoadProjects();
            LoadTemplates();
            comboBox4.SelectedIndex = 0;
            splitContainer1.Panel2.Resize += Panel2_Resize;
            Panel2_Resize(this, new EventArgs());
            LoadSettings();
            ResumeLayout();
        }

        private string _theme = "";

        public ExplorerForms ef { get; set; }

        public void LoadSettings()
        {
            if (CodeEditorControl.settings != null)
            {
                Settings s = CodeEditorControl.settings;

                if (s.Theme == "VS2012Light")
                {
                    panel1.BackColor = Color.FromKnownColor(KnownColor.ControlLight);
                    panel2.BackColor = Color.FromKnownColor(KnownColor.Control);
                }
            }
        }

        public void LoadAddProject()
        {
            return;

            SuspendLayout();

            panel3.Visible = false;
            button2.Location = new Point(button2.Location.X, panel3.Location.Y);
            button3.Location = new Point(button3.Location.X, panel3.Location.Y);
            panel2.Size = new Size(panel2.Width, panel2.Height - panel3.Height);
            panel1.Size = new Size(panel1.Width, panel1.Height - panel3.Height);
            splitContainer1.SplitterDistance -= panel1.Height;
            // this.Size = new Size(this.Width, this.Height - panel3.Height);
            ResumeLayout();
        }

        private void Panel2_Resize(object sender, EventArgs e)
        {
            int w = splitContainer1.Panel2.Size.Width;

            lv.Columns[0].Width = 2 * 60;
            lv.Columns[1].Width = w - 2 * 60;
        }

        public string projectname { get; set; }

        public string projectfolder { get; set; }

        public void SetProjectFolder(string s)
        {
            projectfolder = s;

            textBox2.Text = s;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;

            CreateNewProject();

            Close();
        }


        public void CreateNewProject()
        {
            string SolutionFolder = textBox2.Text;

            string SolutionName = textBox4.Text;

            string folder = SolutionFolder + "\\" + SolutionName;


            CreateDefaultSolution(folder, "");
        }

        private string _solutionFile;

        public string GetSolutionFile()
        {
            return _solutionFile;
        }

        public void CreateDefaultSolution(string folder, string name)
        {
            Directory.CreateDirectory(folder);

            string templateSolution = AppDomain.CurrentDomain.BaseDirectory + "\\" + "TemplateSolution";

            FileInfo[] cc = new DirectoryInfo(templateSolution).GetFiles();

            foreach (FileInfo c in cc)
            {
                string f = folder + "\\" + Path.GetFileName(c.FullName);
                if (File.Exists(f))
                    File.Delete(f);
                File.Copy(c.FullName, folder + "\\" + Path.GetFileName(c.FullName));
                _solutionFile = folder + "\\" + Path.GetFileName(c.FullName);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private TreeViews tv { get; set; }

        private ListView lv { get; set; }

        public void LoadProjects()
        {
            TreeNode rec = new TreeNode();
            rec.Text = "Recent";
            tv.Nodes.Add(rec);

            TreeNode node = new TreeNode();
            node.Text = "Installed";
            tv.Nodes.Add(node);

            TreeNode nodes = new TreeNode();
            nodes.Text = "Visual C# Projects";
            node.Nodes.Add(nodes);

            TreeNode ns = new TreeNode();
            ns.Text = "Windows Application";
            nodes.Nodes.Add(ns);

            TreeNode obs = new TreeNode();
            obs.Text = "Online";
            tv.Nodes.Add(obs);
        }

        public void InitializeListView(ListView lv)
        {
            lv.Clear();

            imgs = new ImageList();
            imgs.Images.Add("Form_Win32", resource_vsc.Formtag_5766_32);

            listView1.View = View.Details;
            this.imgs.ImageSize = new Size(32, 32);
            listView1.SmallImageList = this.imgs;

            //lv.View = View.Details;

            lv.CheckBoxes = false;

            lv.FullRowSelect = true;

            lv.Columns.Add("Name");
            lv.Columns.Add("Description");
        }

        public string folders = "TemplateProjects";

        public void LoadTemplates()
        {
            LoadImages();

            InitializeListView(lv);

            DirectoryInfo d = new DirectoryInfo(folders);

            if (d == null)
                return;

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                ListViewItem v = new ListViewItem();
                v.Text = s.Name;

                int i = AddImageIfAny(s);

                if (i < 0)
                    i = 0;

                v.ImageIndex = i;

                v.SubItems.Add(s.FullName);

                lv.Items.Add(v);
            }
        }

        public string GetProjectFile()
        {
            int i = GetSelectedIndex(lv);
            if (i < 0)
                return "";

            ListViewItem v = lv.Items[i];

            string s = v.SubItems[1].Text;

            DirectoryInfo d = new DirectoryInfo(s);

            FileInfo[] f = d.GetFiles();

            foreach (FileInfo g in f)
                if (g.Extension == ".csproj")
                    return g.FullName;

            return "";
        }

        public string GetProjectFolder()
        {
            int i = GetSelectedIndex(lv);
            if (i < 0)
                return "";

            ListViewItem v = lv.Items[i];

            string s = v.SubItems[1].Text;

            return s;
        }

        public string GetProjectName()
        {
            string s = textBox1.Text;

            return s;
        }

        private FolderBrowserDialog bfd { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            bfd = new FolderBrowserDialog();
            DialogResult r = bfd.ShowDialog();
            if (r != System.Windows.Forms.DialogResult.OK)
                return;

            textBox2.Text = bfd.SelectedPath;
        }

        public int GetSelectedIndex(ListView lv)
        {
            if (lv.SelectedIndices == null)
                return -1;

            if (lv.SelectedIndices.Count <= 0)
                return -1;

            return lv.SelectedIndices[0];
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = GetSelectedIndex(lv);
            if (i < 0)
                return;

            ListViewItem v = lv.Items[i];

            string s = v.SubItems[1].Text.ToString();

            //string file = Path.GetFileName(s);

            textBox1.Text = s;
        }

        public ImageList imgs { get; set; }

        private void LoadImages()
        {
            //DirectoryInfo dir = new DirectoryInfo(@"c:\pic");
        }

        public int AddImageIfAny(DirectoryInfo d)
        {
            FileInfo[] fs = d.GetFiles();

            foreach (FileInfo f in fs)
            {
                if (f.Name == "image.bmp")
                {
                    Image image = new Bitmap(f.FullName);

                    this.imgs.Images.Add(d.Name, image);

                    int i = this.imgs.Images.IndexOfKey(d.Name);

                    return i;
                }
            }

            return -1;
        }

        private void NewProjectForm_Load(object sender, EventArgs e)
        {
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}