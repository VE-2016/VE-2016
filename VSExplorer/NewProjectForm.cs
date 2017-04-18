using AIMS.Libraries.CodeEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer
{
    public partial class NewProjectForm : Form
    {
        public static gui gcommands = new gui();

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

            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
            if (lv != null)
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
    }

    public class NewProject
    {
        public string ProjectName { get; set; }

        public string Location { get; set; }

        public string SolutionName { get; set; }

        public string SolutionFile { get; set; }

        public ArrayList TemplateProjects { get; set; }

        public string GetProjectFile()
        {
            string s = ProjectName;

            DirectoryInfo d = new DirectoryInfo(s);

            FileInfo[] f = d.GetFiles();

            foreach (FileInfo g in f)
                if (g.Extension == ".csproj")
                    return g.FullName;

            return "";
        }

        public void CreateNewProject()
        {
            string SolutionFolder = Location;

            string folder = SolutionFolder + "\\" + SolutionName;

            CreateDefaultSolution(folder, "");
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
                SolutionFile = folder + "\\" + Path.GetFileName(c.FullName);
            }
        }
    }

    /// <summary>
    /// The 'Command' abstract class
    /// </summary>
    public abstract class Command
    {
        public static bool running = false;

        public static int counter = 0;

        //protected Receiver receiver;

        public string Name { get; set; }

        public string Names { get; set; }

        public Image image { get; set; }

        public Keys[] keyboard { get; set; }

        public string Module { get; set; }

        public ExplorerForms ef { get; set; }

        public string GuiHint = "";

        static public Command FromName(string name)
        {
            ExplorerForms ef = ExplorerForms.ef;

            if (name == "OpenForm")
                return new gui.Command_OpenForm(ef);
            else if (name == "GetTemplates")
                return new gui.Command_GetTemplates(ef);
            else if (name == "AddTemplate")
                return new gui.Command_AddTemplate(ef);
            else if (name == "GetProject")
                return new gui.Command_GetProject(ef);
            else if (name == "SetProject")
                return new gui.Command_SetProject(ef);
            else if (name == "Close Form")
                return new gui.Command_CloseForm(ef);
            else if (name == "LoadProjectTemplate")
                return new gui.Command_LoadProjectTemplate(ef);
            else if (name == "MSBuild")
                return new gui.Command_MSBuild(ef);
            else if (name == "Config")
                return new gui.Command_Config(ef);
            else if (name == "Platform")
                return new gui.Command_Platform(ef);
            else if (name == "MainProject")
                return new gui.Command_SetMainProject(ef);
            else if (name == "OpenDocument")
                return new gui.Command_OpenDocument(ef);
            else if (name == "LoadSolution")
                return new gui.Command_LoadSolution(ef);
            else if (name == "AddProjectItem")
                return new gui.Command_AddProjectItem(ef);
            else if (name == "CloseAllDocuments")
                return new gui.Command_CloseAllDocuments(ef);
            else if (name == "TakeSnapshot")
                return new gui.Command_TakeSnapshot(ef);
            else if (name == "Customize")
                return new gui.Command_Customize(ef);
            else if (name == "Solution Platforms")
                return new gui.Command_SolutionPlaform(ef);
            else if (name == "Solution Configurations")
                return new gui.Command_SolutionConfiguration(ef);
            else if (name == "Find")
                return new gui.Command_Find(ef);
            else if (name == "Cut")
                return new gui.Command_Cut(ef);
            else if (name == "Copy")
                return new gui.Command_Copy(ef);
            else if (name == "Paste")
                return new gui.Command_Paste(ef);
            else if (name == "Navigate Forward")
                return new gui.Command_NavigateForward(ef);
            else if (name == "Navigate Backward")
                return new gui.Command_NavigateBackward(ef);
            else if (name == "Add Item")
                return new gui.Command_AddProjectItem(ef);
            else if (name == "Open File")
                return new gui.Command_OpenFile(ef);
            else if (name == "Save")
                return new gui.Command_Save(ef);
            else if (name == "Save All")
                return new gui.Command_SaveAll(ef);
            else if (name == "Start Page")
                return new gui.Command_StartupPage(ef);
            else if (name == "Undo")
                return new gui.Command_Undo(ef);
            else if (name == "Redo")
                return new gui.Command_Redo(ef);
            else if (name == "Find In Files")
                return new gui.Command_FindInFiles(ef);
            else if (name == "Debug Target")
                return new gui.Command_Save(ef);
            else if (name == "Startup Projects")
                return new gui.Command_SolutionRun(ef);
            else if (name == "Launcher")
                return new gui.Command_Launcher(ef);
            else if (name == "Solution Explorer")
                return new gui.Command_SolutionExplorer(ef);
            else if (name == "New Project")
                return new gui.Command_NewProject(ef);
            else if (name == "New Web Site")
                return new gui.Command_NewWebSite(ef);
            else if (name == "Get Project")
                return new gui.Command_GetProject(ef);
            else if (name == "Properties")
                return new gui.Command_PropertyWindow(ef);
            else return null;
        }

        // Constructor
        public Command(ExplorerForms ef)
        {
            this.ef = ef;
            //this.receiver = receiver;
        }

        public abstract object Execute(object obs = null);

        public virtual object Configure(object obs)
        {
            return obs;
        }

        public virtual string ToStrings()
        {
            return Name + "\t" + Names + "\t" + Module;
        }

        public virtual void FromStrings(string s)
        {
            string[] bb = s.Split("\t".ToCharArray());

            Name = bb[1];

            if (bb.Length > 2)
                Names = bb[2];
            if (bb.Length > 3)
                Module = bb[3];
        }
    }

    public class gui
    {
        public Dictionary<string, Command> guis { get; set; }

        static public NewProjectForm npf { get; set; }

        public static ArrayList GetCategories()
        {
            ArrayList L = new ArrayList();

            L.Add("Action");
            L.Add("Build");
            L.Add("Debug");
            L.Add("Build");
            L.Add("Edit");
            L.Add("Format");

            return L;
        }

        public static ArrayList CommandList()
        {
            ArrayList L = new ArrayList();

            L.Add("OpenForm");
            L.Add("GetTemplates");
            L.Add("AddTemplate");
            L.Add("GetProject");

            return L;
        }

        public gui()
        {
            Init(ExplorerForms.ef);
        }

        public Command GetCommand(string command)
        {
            return guis[command];
        }

        static public Dictionary<string, Command> Init()
        {
            ExplorerForms ef = ExplorerForms.ef;

            Dictionary<string, Command> guis = new Dictionary<string, Command>();
            guis.Add("OpenForm", new Command_OpenForm(ef));
            guis.Add("GetTemplates", new Command_GetTemplates(ef));
            guis.Add("AddTemplate", new Command_AddTemplate(ef));
            guis.Add("GetProject", new Command_GetProject(ef));
            guis.Add("SetProject", new Command_SetProject(ef));
            guis.Add("Close Form", new Command_CloseForm(ef));
            guis.Add("LoadProjectTemplate", new Command_LoadProjectTemplate(ef));
            guis.Add("MSBuild", new Command_MSBuild(ef));
            guis.Add("Config", new Command_Config(ef));
            guis.Add("Platform", new Command_Platform(ef));
            guis.Add("MainProject", new Command_SetMainProject(ef));
            guis.Add("OpenDocument", new Command_OpenDocument(ef));
            guis.Add("LoadSolution", new Command_LoadSolution(ef));
            guis.Add("AddProjectItem", new Command_AddProjectItem(ef));
            guis.Add("CloseAllDocuments", new Command_CloseAllDocuments(ef));
            guis.Add("TakeSnapshot", new Command_TakeSnapshot(ef));
            guis.Add("Customize", new Command_Customize(ef));
            guis.Add("Solution Platforms", new Command_Customize(ef));
            guis.Add("Solution Configurations", new Command_Customize(ef));
            guis.Add("Find", new Command_Find(ef));
            guis.Add("New Project", new Command_NewProject(ef));
            guis.Add("Cut", new Command_Cut(ef));
            guis.Add("Copy", new Command_Copy(ef));
            guis.Add("Paste", new Command_Paste(ef));
            guis.Add("Navigate Forward", new Command_NavigateForward(ef));
            guis.Add("Navigate Backward", new Command_NavigateBackward(ef));
            guis.Add("Add Item", new Command_Module(ef));
            guis.Add("Open File", new Command_OpenFile(ef));
            guis.Add("Save", new Command_Save(ef));
            guis.Add("Save All", new Command_SaveAll(ef));
            guis.Add("Start Page", new Command_StartupPage(ef));
            guis.Add("Undo", new Command_Undo(ef));
            guis.Add("Redo", new Command_Redo(ef));
            guis.Add("Find In Files", new Command_FindInFiles(ef));
            guis.Add("Debug Target", new Command_Undo(ef));
            guis.Add("Startup Projects", new Command_SolutionRun(ef));
            guis.Add("Launcher", new Command_Launcher(ef));
            guis.Add("Solution Explorer", new Command_SolutionExplorer(ef));
            guis.Add("New Web Site", new Command_NewWebSite(ef));
            guis.Add("AddConnection", new Command_AddConnection(ef));

            return guis;
        }

        public void Init(ExplorerForms ef)
        {
            guis = new Dictionary<string, Command>();
            guis.Add("OpenForm", new Command_OpenForm(ef));
            guis.Add("GetTemplates", new Command_GetTemplates(ef));
            guis.Add("AddTemplate", new Command_AddTemplate(ef));
            guis.Add("GetProject", new Command_GetProject(ef));
            guis.Add("SetProject", new Command_SetProject(ef));
            guis.Add("CloseForm", new Command_CloseForm(ef));
            guis.Add("LoadProjectTemplate", new Command_LoadProjectTemplate(ef));
            guis.Add("MSBuild", new Command_MSBuild(ef));
            guis.Add("Config", new Command_Config(ef));
            guis.Add("Platform", new Command_Platform(ef));
            guis.Add("MainProject", new Command_SetMainProject(ef));
            guis.Add("OpenDocument", new Command_OpenDocument(ef));
            guis.Add("LoadSolution", new Command_LoadSolution(ef));
            guis.Add("AddProjectItem", new Command_AddProjectItem(ef));
            guis.Add("CloseAllDocuments", new Command_CloseAllDocuments(ef));
            guis.Add("TakeSnapshot", new Command_TakeSnapshot(ef));
            guis.Add("Customize", new Command_Customize(ef));
            guis.Add("Solution Platforms", new Command_Customize(ef));
            guis.Add("Solution Configurations", new Command_Customize(ef));
            guis.Add("Find", new Command_Find(ef));
            guis.Add("AddConnection", new Command_AddConnection(ef));
        }

        public class Command_SolutionPlaform : Command_Gui
        {
            public static Command_SolutionPlaform platform { get; set; }

            public Command_SolutionPlaform(ExplorerForms ef) : base(ef)
            {
                Name = "Solution Platforms";
            }

            private ToolStripComboBoxes cb { get; set; }

            public override object Configure(object obs)
            {
                cb = obs as ToolStripComboBoxes;

                if (cb == null)
                    return obs;

                if (ef == null)
                    return obs;

                platform = this;

                cb.DropDownStyle = ComboBoxStyle.DropDownList;

                ef.BeginInvoke(new Action(() => { ArrayList P = ef.Command_LoadPlatforms(); LoadPlatforms(P); }));

                cb.SelectedIndexChanged += Cb_SelectedIndexChanged;

                return base.Configure(obs);
            }

            private void LoadPlatforms(ArrayList P)
            {
                cb.Items.Clear();
                foreach (string s in P)
                    cb.Items.Add(s);
                if (P.Count > 0)
                    cb.SelectedIndex = 0;
            }

            private void Cb_SelectedIndexChanged(object sender, EventArgs e)
            {
                //MessageBox.Show("Solution Platforms Selected Index Changed");

                int i = cb.SelectedIndex;

                if (i < 0)
                    return;

                string s = cb.Items[i].ToString();

                if (s == "Configuration Manager")
                    ef.BeginInvoke(new Action(() => { ef.Command_ConfigurationManager(); }));
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                ef.BeginInvoke(new Action(() => { cb.Items.Clear(); ArrayList P = ef.Command_LoadPlatforms(); LoadPlatforms(P); }));

                return obs;
            }
        }

        public class Command_SolutionConfiguration : Command_Gui
        {
            public static Command_SolutionConfiguration configuration { get; set; }

            public Command_SolutionConfiguration(ExplorerForms ef) : base(ef)
            {
                Name = "Solution Configurations";
            }

            private ToolStripComboBoxes cb { get; set; }

            public override object Configure(object obs)
            {
                cb = obs as ToolStripComboBoxes;

                if (cb == null)
                    return obs;

                if (ef == null)
                    return obs;

                configuration = this;

                cb.DropDownStyle = ComboBoxStyle.DropDownList;

                ef.BeginInvoke(new Action(() => { cb.Items.Clear(); ArrayList P = ef.Command_LoadConfigurations(); LoadPlatforms(P); }));

                return base.Configure(obs);
            }

            private void LoadPlatforms(ArrayList P)
            {
                cb.Items.Clear();
                foreach (string s in P)
                    cb.Items.Add(s);
                if (P.Count > 0)
                    cb.SelectedIndex = 0;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                if (obs == null)
                    obs = "";

                string dialogs = obs as string;

                ef.BeginInvoke(new Action(() => { cb.Items.Clear(); ArrayList P = ef.Command_LoadConfigurations(); LoadPlatforms(P); }));

                return obs;
            }
        }

        public class Command_SolutionRun : Command_Gui
        {
            public static Command_SolutionRun projects { get; set; }

            public Command_SolutionRun(ExplorerForms ef) : base(ef)
            {
                Name = "Startup Projects";
            }

            private ToolStripComboBoxes cb { get; set; }

            public override object Configure(object obs)
            {
                cb = obs as ToolStripComboBoxes;

                if (cb == null)
                    return obs;

                if (ef == null)
                    return obs;

                projects = this;

                cb.DropDownStyle = ComboBoxStyle.DropDownList;

                ef.BeginInvoke(new Action(() => { cb.Items.Clear(); ArrayList P = ef.Command_LoadStartupProjects(); LoadPlatforms(P); }));

                return base.Configure(obs);
            }

            private void LoadPlatforms(ArrayList P)
            {
                cb.Items.Clear();
                foreach (string s in P)
                    cb.Items.Add(s);
                if (P.Count > 0)
                    cb.SelectedIndex = 0;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                if (obs == null)
                    obs = "";

                string dialogs = obs as string;

                ef.BeginInvoke(new Action(() => { cb.Items.Clear(); ArrayList P = ef.Command_LoadStartupProjects(); LoadPlatforms(P); }));

                return obs;
            }
        }

        public class Command_Customize : Command
        {
            public Command_Customize(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                if (obs == null)
                    obs = "";

                string dialogs = obs as string;

                ef.BeginInvoke(new Action(() => { ef.Command_CustomizeDialog(dialogs); }));

                return obs;
            }
        }

        public class Command_AddConnection : Command
        {
            public Command_AddConnection(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                string c = obs as string;

                ef.Command_AddConnection(c);

                return obs;
            }
        }

        public class Command_TakeSnapshot : Command
        {
            public Command_TakeSnapshot(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                if (obs == null)
                    obs = "";

                string snapshot = obs as string;

                ef.BeginInvoke(new Action(() => { ef.Command_TakeSnapshot(snapshot); }));

                return obs;
            }
        }

        public class Command_SetMainProject : Command
        {
            public Command_SetMainProject(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                string mainproject = obs as string;

                ef.BeginInvoke(new Action(() => { ef.SetVSProject(mainproject); }));

                return obs;
            }
        }

        public class Command_Config : Command
        {
            public Command_Config(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                // while (Command.running == true) ;

                string cfg = obs as string;

                ef.BeginInvoke(new Action(() => { ef.Command_Config("", cfg); }));

                return obs;
            }
        }

        public class Command_Platform : Command
        {
            public Command_Platform(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                Command.counter = 1;

                string plf = obs as string;

                ef.BeginInvoke(new Action(() => { ef.Command_Config(plf, ""); Command.counter--; }));

                while (Command.counter > 0) ;

                return obs;
            }
        }

        public class Command_MSBuild : Command
        {
            public Command_MSBuild(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                string project = obs as string;

                if (project == null)
                    project = "";

                Command.counter = 1;

                ef.BeginInvoke(new Action(() => { ef.Command_Rebuild(project); }));

                while (Command.counter > 0) ;

                return obs;
            }
        }

        public class Command_OpenForm : Command
        {
            public Command_OpenForm(ExplorerForms ef) : base(ef)
            {
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                npf = new NewProjectForm();

                //npf.CreateControl();
                MethodInfo ch = npf.GetType().GetMethod("CreateHandle", BindingFlags.NonPublic | BindingFlags.Instance);
                ch.Invoke(npf, new object[0]);

                //while (npf.IsHandleCreated == false) ;

                System.Threading.Tasks.Task.Run(new Action(() =>
                {
                    gui.npf = npf;
                    npf.BeginInvoke(new Action(() => { npf.ShowDialog(); }));
                }));

                return npf;
            }
        }

        public class Command_CloseForm : Command
        {
            public Command_CloseForm(ExplorerForms ef) : base(ef)
            {
                Name = "Close Form";
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                while (gui.npf == null) ;
                //while (gui.npf.Visible == false) ;
                gui.npf.BeginInvoke(new Action(() => { gui.npf.Close(); }));
                return null;
            }
        }

        public class Command_GetTemplates : Command
        {
            public Command_GetTemplates(ExplorerForms ef) : base(ef)
            {
            }

            private NewProjectForm npf { get; set; }

            public ArrayList T { get; set; }

            override public object Execute(object obs = null)
            {
                T = new ArrayList();
                if (gui.npf == null)
                    return T;
                T = gui.npf.GetTemplates("");
                return T;
            }
        }

        public class Command_AddTemplate : Command
        {
            public Command_AddTemplate(ExplorerForms ef) : base(ef)
            {
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (obs == null)
                    return obs;

                string folder = obs as string;

                string[] cc = folder.Split("\\".ToCharArray());

                string d = "TemplateProjects\\" + cc[cc.Length - 1];

                string b = AppDomain.CurrentDomain.BaseDirectory;

                DirectoryInfo source = new DirectoryInfo(folder);

                DirectoryInfo target = new DirectoryInfo(b + d);

                ExplorerForms.CopyAll(source, target);

                return obs;
            }
        }

        public class Command_GetProject : Command
        {
            public Command_GetProject(ExplorerForms ef) : base(ef)
            {
                Name = "Get Project";
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                npf = ef.npf;

                NewProject nw = npf.Project();

                return nw;
            }
        }

        public class Command_SetProject : Command
        {
            public Command_SetProject(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                npf = ef.npf;

                NewProject nw = obs as NewProject;

                npf.Project(nw);

                return nw;
            }
        }

        public class Command_LoadProjectTemplate : Command
        {
            public Command_LoadProjectTemplate(ExplorerForms ef) : base(ef)
            {
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                Command.counter = 2;

                NewProject nw = obs as NewProject;

                ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(true, nw); }));

                while (Command.counter > 0) ;

                return obs;
            }
        }

        public class Command_NavigateForward : Command
        {
            public Command_NavigateForward(ExplorerForms ef) : base(ef)
            {
                Name = "Navigate Forward";
                image = Resources.Forward_256x;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Shift;
                keyboard[2] = Keys.Subtract;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_NavigateForward(); }));

                return obs;
            }
        }

        public class Command_NavigateBackward : Command
        {
            public Command_NavigateBackward(ExplorerForms ef) : base(ef)
            {
                Name = "Navigate Backward";
                image = Resources.Backward_256x;
                GuiHint = "ToolStripDropDownButton";
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_NavigateBackward(); }));

                return obs;
            }
        }

        public class Command_NewProject : Command
        {
            public Command_NewProject(ExplorerForms ef) : base(ef)
            {
                Name = "New Project";
                image = Resources.NewItem_16x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Subtract;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_NewWebSite : Command
        {
            public Command_NewWebSite(ExplorerForms ef) : base(ef)
            {
                Name = "New Web Site";
                image = Resources.OpenWeb_16x;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_OpenProject : Command
        {
            public Command_OpenProject(ExplorerForms ef) : base(ef)
            {
                Name = "Open Project";
                image = Resources.NewFile_6276_32;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.E;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Save : Command
        {
            public Command_Save(ExplorerForms ef) : base(ef)
            {
                Name = "Save";
                image = Resources.Save_256x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.S;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_SaveAll : Command
        {
            public Command_SaveAll(ExplorerForms ef) : base(ef)
            {
                Name = "Save All";
                image = Resources.SaveAll_256x;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Shift;
                keyboard[2] = Keys.S;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_StartupPage : Command_Launcher
        {
            public Command_StartupPage(ExplorerForms ef) : base(ef)
            {
                Name = "Start Page";
                image = Resources.SaveAll_256x;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Shift;
                keyboard[2] = Keys.S;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Cut : Command
        {
            public Command_Cut(ExplorerForms ef) : base(ef)
            {
                Name = "Cut";
                image = Resources.Cut_24x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.X;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Copy : Command
        {
            public Command_Copy(ExplorerForms ef) : base(ef)
            {
                Name = "Copy";
                image = Resources.Copy_32x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.C;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Paste : Command
        {
            public Command_Paste(ExplorerForms ef) : base(ef)
            {
                Name = "Paste";
                image = Resources.Paste_16x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.V;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Undo : Command
        {
            public Command_Undo(ExplorerForms ef) : base(ef)
            {
                Name = "Undo";
                image = Resources.Undo_16x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Z;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Redo : Command
        {
            public Command_Redo(ExplorerForms ef) : base(ef)
            {
                Name = "Redo";
                image = Resources.Redo_16x;
                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Y;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_FindInFiles : Command_Launcher
        {
            public Command_FindInFiles(ExplorerForms ef) : base(ef)
            {
                Name = "Find In Files";
                image = Resources.FindinFiles_16x;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Shift;
                keyboard[2] = Keys.F;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_Launch(Names); }));

                return obs;
            }
        }

        public class Command_Find : Command_Gui
        {
            public Command_Find(ExplorerForms ef) : base(ef)
            {
                Name = "Find";

                Names = "Find";

                keyboard = new Keys[2];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.D;
            }

            private ToolStripComboBoxes cb { get; set; }

            public override object Configure(object obs)
            {
                cb = obs as ToolStripComboBoxes;

                if (cb == null)
                    return obs;

                if (ef == null)
                    return obs;

                cb.SelectedIndexChanged += Cb_SelectedIndexChanged;

                cb.KeyPress += Cb_TextChanged;

                return base.Configure(obs);
            }

            private void Cb_TextChanged(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter)
                    return;

                if (cb.Text == "")
                    return;

                //MessageBox.Show("Find Text Changed");

                string s = cb.Text;

                Execute(s);
            }

            private void Cb_SelectedIndexChanged(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return obs;

                if (obs == null)
                    obs = "";

                string text = obs as string;

                ef.BeginInvoke(new Action(() => { ef.Command_Find(text); }));

                return obs;
            }
        }

        public class Command_Module : Command
        {
            public string ModuleName { get; set; }

            //public string Names { get; set; }

            public Command_Module(ExplorerForms ef) : base(ef)
            {
                Name = "Module";

                Module = "NewItems";

                GuiHint = "ToolStripDropDownButton";

                image = Resources.Backward_256x;
            }

            //NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }
        }

        public class Command_Gui : Command
        {
            public string ModuleName { get; set; }

            public bool Visible { get; set; }

            public Command_Gui(ExplorerForms ef) : base(ef)
            {
                Name = "Gui";

                Names = "Gui";

                Visible = true;
            }

            //NewProjectForm npf { get; set; }

            public virtual object Configure(object obs)
            {
                return base.Configure(obs);
            }

            public Command GetCommand()
            {
                return NewProjectForm.gcommands.GetCommand(Names);
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                //ef.BeginInvoke(new Action(() => { ef.Command_AddNewProject(); }));

                return obs;
            }

            public override string ToStrings()
            {
                return Name + "\t" + Names + "\t" + Module;
            }

            public override void FromStrings(string s)
            {
                string[] bb = s.Split("\t".ToCharArray());

                Name = bb[1];

                if (bb.Length > 2)
                    Names = bb[2];
                if (bb.Length > 3)
                    Module = bb[3];
            }
        }

        public class Command_Launcher : Command
        {
            public string ModuleName { get; set; }

            // public string Names { get; set; }

            public bool Visible { get; set; }

            public Command_Launcher(ExplorerForms ef) : base(ef)
            {
                Name = "Launcher";

                Visible = true;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_Launch(Names); }));

                return obs;
            }
        }

        public class Command_SolutionExplorer : Command_Launcher
        {
            public string ModuleName { get; set; }

            //public string Names { get; set; }

            public bool Visible { get; set; }

            public Command_SolutionExplorer(ExplorerForms ef) : base(ef)
            {
                Name = "Solution Explorer";

                image = Resources.VisualStudioSolution;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Alt;
                keyboard[2] = Keys.L;

                Visible = true;
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_Launch(Name); }));

                return obs;
            }
        }

        public class Command_PropertyWindow : Command_Launcher
        {
            public string ModuleName { get; set; }

            //public string Names { get; set; }

            public bool Visible { get; set; }

            public Command_PropertyWindow(ExplorerForms ef) : base(ef)
            {
                Name = "Properties";

                image = Resources.Property_256x;
                keyboard = new Keys[3];

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.Alt;
                keyboard[2] = Keys.L;

                Visible = true;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_Launch(Names); }));

                return obs;
            }
        }

        public class Command_AddProjectItem : Command_Module
        {
            public Command_AddProjectItem(ExplorerForms ef) : base(ef)
            {
                Name = "Add Item";

                image = Resources.NewItem_16x;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                string name = obs as string;

                Command.counter = 1;

                ef.BeginInvoke(new Action(() =>
                {
                    VSSolution vs = ef.GetVSSolution();

                    VSProject vp = vs.MainVSProject;

                    ef.Command_AddNewProjectItem(name, vp);
                }));

                while (Command.counter > 0) ;

                return obs;
            }
        }

        public class Command_LoadSolution : Command
        {
            public Command_LoadSolution(ExplorerForms ef) : base(ef)
            {
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                string file = obs as string;

                Command.counter = 1;

                NewProject nw = obs as NewProject;

                ef.BeginInvoke(new Action(() => { ef.Command_LoadSolution(file); }));

                while (Command.counter > 0) ;

                return obs;
            }
        }

        public class Command_CloseAllDocuments : Command
        {
            public Command_CloseAllDocuments(ExplorerForms ef) : base(ef)
            {
                Name = "Open All Documents";
            }

            private NewProjectForm npf { get; set; }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                ef.BeginInvoke(new Action(() => { ef.Command_CloseAllDocuments(); }));

                return obs;
            }
        }

        public class Command_OpenFile : Command
        {
            public Command_OpenFile(ExplorerForms ef) : base(ef)
            {
                Name = "Open File";
                keyboard = new Keys[2];
                image = Resources.Open_6529_24;

                keyboard[0] = Keys.Control;
                keyboard[1] = Keys.O;
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                string file = obs as string;

                Command.counter = 1;

                AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);

                ef.BeginInvoke(new Action(() => { ef.Command_OpenFile(file, autoEvent); }));

                if (autoEvent != null) autoEvent.WaitOne();

                return obs;
            }
        }

        public class Command_OpenDocument : Command
        {
            public Command_OpenDocument(ExplorerForms ef) : base(ef)
            {
            }

            override public object Execute(object obs = null)
            {
                if (ef == null)
                    return ef;

                if (obs == null)
                    return obs;

                string file = obs as string;

                Command.counter = 1;

                AutoResetEvent autoEvent = new System.Threading.AutoResetEvent(false);

                ef.BeginInvoke(new Action(() => { ef.Command_OpenFile(file, autoEvent); }));

                if (autoEvent != null) autoEvent.WaitOne();

                return obs;
            }
        }
    }
}