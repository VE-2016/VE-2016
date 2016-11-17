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

            if(nw == null)
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

        public class gui
        {

            public Dictionary<string, Command> guis { get; set; } 

            static public NewProjectForm npf { get; set; } 


            public gui()
            {
                Init(ExplorerForms.ef);
            }

            public Command GetCommand(string command)
            {
                return guis[command];
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

                    Command.counter = 1 ;

                    string plf = obs as string;

                    ef.BeginInvoke(new Action(() => { ef.Command_Config(plf, "");  Command.counter--; }));

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

            public class Command_OpenForm : Command {


                public Command_OpenForm(ExplorerForms ef) : base(ef)
                {

                }

                NewProjectForm npf { get; set; }

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

                }

                NewProjectForm npf { get; set; }

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

                NewProjectForm npf { get; set; }

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

                NewProjectForm npf { get; set; }

                override public object Execute( object obs = null)
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

                NewProjectForm npf { get; set; }

                override public object Execute( object obs = null)
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
            public class Command_AddProjectItem : Command
            {


                public Command_AddProjectItem(ExplorerForms ef) : base(ef)
                {

                }

                
                override public object Execute(object obs = null)
                {

                    if (ef == null)
                        return ef;

                    if (obs == null)
                        return obs;

                    string name = obs as string;

                    Command.counter = 1;

                    

                    ef.BeginInvoke(new Action(() => {

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

                NewProjectForm npf { get; set; }

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

                }

                NewProjectForm npf { get; set; }

                override public object Execute(object obs = null)
                {

                    if (ef == null)
                        return ef;

                    ef.BeginInvoke(new Action(() => { ef.Command_CloseAllDocuments(); }));


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

                    ef.BeginInvoke(new Action(() => { ef.Command_OpenFile(file, autoEvent);  }));

                    if (autoEvent != null) autoEvent.WaitOne();

                    return obs;
                }

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
            if(lv != null)
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
}