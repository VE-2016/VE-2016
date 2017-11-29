using ScriptControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace WinExplorer
{
    public partial class NewWebSiteForm : Form
    {
        public static gui gcommands = new gui();

        public NewWebSiteForm(string filename = "")
        {
            InitializeComponent();
            SuspendLayout();
            tv = treeView2;
            tv.FullRowSelect = true;
            tv.ShowLines = false;
            lv = listView1;
            LoadProjects();
            LoadVSTemplates();
            LoadTemplates(filename);
            comboBox4.SelectedIndex = 0;
            splitContainer1.Panel2.Resize += Panel2_Resize;
            Panel2_Resize(this, new EventArgs());
            LoadSettings();
            if (!string.IsNullOrEmpty(filename))
                SelectTemplate(filename);
            ResumeLayout();
            searchTextBox1.Text = "Search Installed Templates (Ctrl + E)";
            tv.AfterSelect += Tv_AfterSelect;
            rb = richTextBox1;
        }

        public RichTextBox rb { get; set; }

        private void Tv_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            PreviewInfo p = node.Tag as PreviewInfo;
            if (p == null)
                return;
            lv.Items.Clear();

            lv.Items.AddRange(p.v.ToArray());

            richTextBox1.Text = "Type \n\r " + p.description;
            SetAsBold("Type ", rb);
        }

        public void SetAsBold(string s, RichTextBox rb)
        {
            rb.Find(s, RichTextBoxFinds.MatchCase);

            rb.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Bold);
            rb.SelectionColor = SystemColors.HotTrack;
        }

        private void SelectTemplate(string filename)
        {
            string file = Path.GetFileName(filename);
            ListViewItem v = lv.FindItemWithText(file);
            if (v == null)
                return;
            v.Selected = true;
            lv.Select();
        }

        public NewProject Project(NewProject nw = null)
        {
            if (nw == null)
            {
                nw.ProjectName = textBox1.Text;
                nw.Location = textBox2.Text;
                nw.SolutionName = textBox4.Text;
                return nw;
            }

            textBox1.Text = nw.ProjectName;
            textBox2.Text = nw.Location;
            textBox4.Text = nw.SolutionName;

            return nw;
        }

        public class Category
        {
            public string Name { get; set; }

            public ArrayList C { get; set; }

            public Category()
            {
                C = new ArrayList();
            }
        }

        private string _theme = "";

        public ExplorerForms ef { get; set; }

        public void LoadSettings()
        {
            if (ScriptControl.ScriptControl.settings != null)
            {
                Settings s = ScriptControl.ScriptControl.settings;

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

            TreeNode tmp = new TreeNode();
            tmp.Text = "Templates";
            obs.Nodes.Add(tmp);

            TreeNode vsc = new TreeNode();
            vsc.Text = "Visual C#";
            tmp.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Other";
            tmp.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Visual F#";
            tmp.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Visual C++";
            tmp.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Visual Basic";
            tmp.Nodes.Add(vsc);
        }

        public void LoadVSTemplates()
        {
            TreeNode node = new TreeNode();
            node.Text = "Installed-2";
            tv.Nodes.Add(node);

            TreeNode vsc = new TreeNode();
            vsc.Text = "Visual C#";
            node.Nodes.Add(vsc);

            LoadCSharpTemplates(vsc);

            vsc = new TreeNode();
            vsc.Text = "Visual Basic";
            node.Nodes.Add(vsc);

            LoadVisualBasicTemplates(vsc);

            //vsc = new TreeNode();
            //vsc.Text = "Visual C++";
            //node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Visual F#";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "SQL Server";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "R";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Azure Data Lake";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "JavaScript";
            node.Nodes.Add(vsc);
            LoadJavascriptTemplates(vsc);

            vsc = new TreeNode();
            vsc.Text = "Python";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "TypeScript";
            node.Nodes.Add(vsc);

            vsc = new TreeNode();
            vsc.Text = "Other Project Types";
            node.Nodes.Add(vsc);

            TreeNode vs = new TreeNode();
            vs.Text = "Visual Studio Solution";
            vsc.Nodes.Add(vs);
        }

        public string templates = "Templates-Data";

        public void LoadJavascriptTemplates(TreeNode node)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + templates + "\\ProjectTemplates\\Javascript";

            string[] d = Directory.GetDirectories(folder);

            PreviewInfo p = new PreviewInfo();

            PreviewInfo pp = new PreviewInfo();

            TreeNode vsc = new TreeNode();

            foreach (string s in d)
            {
                pp = new PreviewInfo();

                vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);

                if (s.Contains("Windows 10"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Mobile Apps"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else continue;

                node.Nodes.Add(vsc);

                vsc.Tag = pp;

                p.v.AddRange(pp.v);
            }

            node.Tag = p;
        }

        public void LoadCSharpTemplates(TreeNode node)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + templates + "\\ProjectTemplates\\CSharp";

            string[] d = Directory.GetDirectories(folder);

            PreviewInfo p = new PreviewInfo();

            PreviewInfo pp = new PreviewInfo();

            TreeNode vsc = new TreeNode();

            foreach (string s in d)
            {
                pp = new PreviewInfo();

                vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);

                if (s.Contains("Windows UAP"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Windows"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                    vsc.Text = "Windows Classic Desktop";
                }
                else if (s.EndsWith("Web"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith(".NET Core"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Cloud"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Extensibility"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("WCF"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Test"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else continue;

                node.Nodes.Add(vsc);

                vsc.Tag = pp;

                p.v.AddRange(pp.v);
            }

            pp = new PreviewInfo();

            vsc = new TreeNode();
            vsc.Text = "Android";
            node.Nodes.Add(vsc);
            LoadAndroid(vsc, "", pp);
            vsc.Tag = pp;
            p.v.AddRange(pp.v);

            node.Tag = p;
        }

        public void LoadVisualBasicTemplates(TreeNode node)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + templates + "\\ProjectTemplates\\VisualBasic";

            string[] d = Directory.GetDirectories(folder);

            PreviewInfo p = new PreviewInfo();

            PreviewInfo pp = new PreviewInfo();

            TreeNode vsc = new TreeNode();

            foreach (string s in d)
            {
                pp = new PreviewInfo();

                vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);

                if (s.Contains("Windows UAP"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Windows"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                    vsc.Text = "Windows Classic Desktop";
                }
                else if (s.EndsWith("Web"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                //else if (s.EndsWith(".NET Core"))
                //{
                //    LoadWindowsUniversal(vsc, s, pp);

                //}
                else if (s.EndsWith("Cloud"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Extensibility"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("WCF"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else if (s.EndsWith("Test"))
                {
                    LoadWindowsUniversal(vsc, s, pp);
                }
                else continue;

                node.Nodes.Add(vsc);

                vsc.Tag = pp;

                p.v.AddRange(pp.v);
            }

            //pp = new PreviewInfo();

            //vsc = new TreeNode();
            //vsc.Text = "Android";
            //node.Nodes.Add(vsc);
            //LoadAndroid(vsc, "", pp);
            //vsc.Tag = pp;
            //p.v.AddRange(pp.v);

            node.Tag = p;
        }

        public void LoadAndroid(TreeNode node, string folder, PreviewInfo p)
        {
            string folders = AppDomain.CurrentDomain.BaseDirectory + templates + "\\Extensions\\Xamarin.VisualStudio\\T\\PT\\CSharp\\Android";

            string[] d = Directory.GetFiles(folders);

            foreach (string s in d)
            {
                TreeNode vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);
                //node.Nodes.Add(vsc);

                PreviewInfo c = LoadTemplateXml(vsc, s);

                ListViewItem v = LoadListViewItems(s, c);

                p.v.Add(v);
            }
        }

        public void LoadWindowsUniversal(TreeNode node, string folder, PreviewInfo p)
        {
            string folders = folder + "\\1033";

            string[] d = Directory.GetDirectories(folders);

            foreach (string s in d)
            {
                TreeNode vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);
                //node.Nodes.Add(vsc);

                PreviewInfo c = LoadTemplateXml(vsc, s);

                ListViewItem v = LoadListViewItems(s, c);

                p.v.Add(v);
            }
        }

        public void LoadWindowsClassic(TreeNode node, string folder, PreviewInfo p)
        {
            string folders = folder + "\\1033";

            string[] d = Directory.GetDirectories(folders);

            foreach (string s in d)
            {
                TreeNode vsc = new TreeNode();
                vsc.Text = Path.GetFileName(s);
                //node.Nodes.Add(vsc);

                PreviewInfo c = LoadTemplateXml(vsc, s);

                ListViewItem v = LoadListViewItems(s, c);

                p.v.Add(v);
            }
        }

        private PreviewInfo LoadTemplateXml(TreeNode node, string folder)
        {
            PreviewInfo p = new PreviewInfo();

            p.folder = folder;

            string file = new DirectoryInfo(folder).Name;

            string filename = folder + "\\" + file + ".vstemplate";

            if (!File.Exists(filename))
                return p;

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);

            XmlNode xml = FindNode(xmlDocument.ChildNodes, "TemplateData");

            if (xml != null)
            {
                XmlNode a = FindNode(xml.ChildNodes, "PreviewImage");

                XmlNode b = FindNode(xml.ChildNodes, "Description");
                if (a != null)
                    p.image = a.InnerText;
                if (b != null)
                    p.description = b.InnerText;
            }

            node.Tag = p;

            return p;
        }

        public ListViewItem LoadListViewItems(string file, PreviewInfo p)
        {
            ListViewItem v = new ListViewItem();
            v.Text = Path.GetFileName(file);

            int i = AddImageIfAny(new DirectoryInfo(file));

            if (i < 0)
                i = 0;

            v.ImageIndex = i;

            v.SubItems.Add(file);

            v.Tag = p;

            return v;
        }

        public XmlNode FindNode(XmlNodeList list, string nodeName)
        {
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    if (node.Name.Equals(nodeName)) return node;
                    if (node.HasChildNodes)
                    {
                        XmlNode nodes = FindNode(node.ChildNodes, nodeName);

                        if (nodes != null)
                            return nodes;
                    }
                }
            }
            return null;
        }

        public void InitializeListView(ListView lv)
        {
            lv.Clear();

            imgs = new ImageList();
            imgs.Images.Add("Form_Win32", ve_resource.WindowsForm_256x);

            listView1.View = View.Details;
            this.imgs.ImageSize = new Size(32, 32);
            listView1.SmallImageList = this.imgs;

            //lv.View = View.Details;

            lv.CheckBoxes = false;

            lv.FullRowSelect = true;

            lv.Columns.Add("Name");
            lv.Columns.Add("Description");

            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        static public string folders = "TemplateProjects";

        public static List<string> GetProjectTemplates()
        {
            string c = AppDomain.CurrentDomain.BaseDirectory;

            List<string> tmps = new List<string>();

            ArrayList L = GetProjectTemplates(c + folders);

            foreach (string s in L)
                tmps.Add(s);

            return tmps;
        }

        public void LoadTemplates(string filename = "")
        {
            string c = AppDomain.CurrentDomain.BaseDirectory;

            LoadImages();

            InitializeListView(lv);

            lv.SelectedIndexChanged += Lv_SelectedIndexChanged;

            DirectoryInfo d = new DirectoryInfo(c + folders);

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

        private void Lv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv.SelectedItems == null || lv.SelectedItems.Count <= 0)
                return;
            ListViewItem b = lv.SelectedItems[0];
            if (b == null)
                return;
            PreviewInfo p = b.Tag as PreviewInfo;
            if (p == null)
                return;
            richTextBox1.Text = p.description;
        }

        public ArrayList GetTemplates(string folder)
        {
            ArrayList L = new ArrayList();

            DirectoryInfo d = new DirectoryInfo(folders);

            if (d == null)
                return L;

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                L.Add(s.FullName);
            }

            return L;
        }

        static public ArrayList GetProjectTemplates(string folder)
        {
            ArrayList L = new ArrayList();

            DirectoryInfo d = new DirectoryInfo(folders);

            if (d == null)
                return L;

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                L.Add(s.FullName);
            }

            return L;
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

            s = s.Replace("{", "");

            s = s.Replace("}", "");

            string[] cc = s.Split("\\".ToCharArray());

            textBox1.Text = cc[cc.Length - 1];
        }

        public ImageList imgs { get; set; }

        private void LoadImages()
        {
            //DirectoryInfo dir = new DirectoryInfo(@"c:\pic");
        }

        public int AddImageIfAny(DirectoryInfo d)
        {
            if (!d.Exists)
                return -1;

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

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (lv != null)
                    if (lv.Columns.Count >= 1)
                        lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
            }
        }
    }
}