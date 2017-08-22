using AIMS.Libraries.Scripting.ScriptControl;
using AndersLiu.Reflector.Program.UI.AssemblyTreeNode;
using ICSharpCode.NRefactory.CSharp;
using Microsoft.Build.Utilities;

//using ICSharpCode.SharpDevelop.Project;

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using WinExplorer.UI;

namespace WinExplorer
{
    public class TreeViews : System.Windows.Forms.TreeView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
                                                string pszSubIdList);
        bool handlecreated = false;

        //protected override void CreateHandle()
        //{
        //    if (handlecreated)
        //        return;
        //    try
        //    {
        //        base.CreateHandle();
        //        SetWindowTheme(this.Handle, "explorer", null);
        //        handlecreated = true;
        //    }
        //    catch(Exception ex)
        //    {

        //    }
        //}

            public TreeViews()
        {
            SetWindowTheme(this.Handle, "explorer", null);
        }

        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }
        //protected override void OnMouseUp( MouseEventArgs e)
        //{
        //    TreeView treeView = this;

        //    if (context1 == null)
        //        return;

        //    // Show menu only if the right mouse button is clicked.
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        // Point where the mouse is clicked.
        //        Point p = new Point(e.X, e.Y);

        //        // Get the node that the user has clicked.
        //        TreeNode node = treeView.GetNodeAt(p);
        //        if (node != null)
        //        {
        //            // Select the node the user has clicked.
        //            // The node appears selected until the menu is displayed on the screen.
        //            //oldselectednode = treeView.SelectedNode;

        //            if (node.Parent == null)
        //                context2.Show(treeView, p);
        //            else context1.Show(treeView, p);

        //            // Highlight the selected node.
        //            //treeView.SelectedNode = oldselectednode;
        //            //oldselectednode = null;

        //            treeView.Refresh();
        //        }
        //    }
        //}
    }

    public interface IView
    {
        void addview(Control c);

        Control getact();

        string getactview();

        ArrayList getviews();

        string nextview();
    }

    public class Actions
    {
    }

    public class astlogger
    {
        private static StringBuilder s_bb = new StringBuilder();

        static public void logger(string file, SyntaxTree syntax)
        {
            File.Open(file, FileMode.OpenOrCreate).Close();

            if (s_bb == null)
                s_bb = new StringBuilder();

            foreach (AstNode ast in syntax.Children)
            {
                foreach (AstNode astn in ast.Children)
                {
                    string s = astn.Region.ToString();
                    s_bb.AppendLine(s);

                    s = astn.Role.ToString();
                    s_bb.AppendLine(s);

                    s = astn.NodeType.ToString();
                    s_bb.AppendLine(s);
                }
            }

            File.WriteAllText(file, s_bb.ToString());

            s_bb.Clear();
        }
    }

    public class Controller : IView
    {
        public int act = 0;
        public ArrayList W = new ArrayList();

        public Control logger { get; set; }

        public void addview(Control _c)
        {
            W.Add(_c);
        }

        public Control getact()
        {
            if (W.Count <= act)
                return new Control();

            return (Control)W[act];
        }

        public string getactview()
        {
            if (act > W.Count - 1)
                return "e";

            Control c = (Control)W[act];

            return c.Name;
        }

        public Control getlogger()
        {
            return logger;
        }

        public ArrayList getviews()
        {
            return W;
        }

        public string nextview()
        {
            act++;
            if (act == W.Count)
                act = 0;

            Control c = (Control)W[act];

            return c.Name;
        }

        public void registerlogger(Control c)
        {
            logger = c;
        }
    }

    public class CreateView
    {
        public ToolStrip _mainToolStrip = new ToolStrip();

        public ToolStripButton _moveButton;

        public ToolStripButton _parentButton;

        public ToolStripButton _refreshButton;

        public TreeView _LinksTreeView { get; set; }

        public TreeView _maintv { get; set; }

        public ExplorerObject eo { get; set; }

        public Form frm { get; set; }

        public string linkfile { get; set; }

        public void doinit(Form form)
        {
            frm = form;

            _LinksTreeView = new TreeView();

            eo.cont.addview(_LinksTreeView);

            //_mainToolStrip = new ToolStrip();
            _refreshButton = new ToolStripButton();
            //_parentButton = new ToolStripButton();
            _moveButton = new ToolStripButton();

            //_mainToolStrip.Items.Add(_refreshButton);
            //_mainToolStrip.Items.Add(_parentButton);
            //_mainToolStrip.Items.Add(_moveButton);

            _refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_refreshButton.Image = resources_vs.LinkedServer;
            _refreshButton.ImageTransparentColor = Color.Magenta;
            _refreshButton.Name = "Load Links";
            _refreshButton.Text = "Load Links";
            _refreshButton.Click += new System.EventHandler(load_links);//celegate { eo._applicationManager.NotifyFileSystemChange(); };

            //_parentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_parentButton.Image = resources_vs.GoToReference_5542_32;
            //_parentButton.ImageTransparentColor = Color.Magenta;
            //_parentButton.Name = "Constants.UI_TOOLBAR_MOVE_PARENT";
            //_parentButton.Text = "Links";
            //_parentButton.Click += new System.EventHandler(OpenLinksToolStripButton_Click);

            //_moveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_moveButton.Image = ve_resource.OpenFolder;
            //_moveButton.ImageTransparentColor = Color.Magenta;
            //_moveButton.Name = Constants.UI_TOOLBAR_MOVE_NEW;
            //_moveButton.Text = ve_resource.ToolbarMoveRoot;
            //_moveButton.Click += new System.EventHandler(MoveToolStripButton_Click);

            _LinksTreeView.AfterSelect +=
               new TreeViewEventHandler(TreeView_AfterSelect);

            _LinksTreeView.Dock = DockStyle.Fill;

            frm.Controls.Add(_LinksTreeView);

            _LinksTreeView.ResumeLayout(false);
            _LinksTreeView.PerformLayout();

            linkfile = "C:\\a.txt";
        }

        public TreeViews cc { get; set; }

        public void CreateClassViewer(TreeViews _cc)
        {
            cc = _cc;
        }

        public VSSolution vs { get; set; }

        public Dictionary<string, string> dc { get; set; }

        public TreeView LoadSolution(VSSolution _vs)
        {
            vs = _vs;

            TreeView tt = new TreeView();

            //cc.Nodes.Clear();

            foreach (VSProject p in vs.Projects)
            {
                if (Directory.Exists(p.GetProjectExec()) == true)
                    continue;

                TreeNode node = new TreeNode();
                node.Text = p.Name;
                node.Tag = p;

                node.ImageKey = "Folder";
                node.SelectedImageKey = "Folder";

                if (dc != null)
                {
                    string exts = Path.GetExtension(p.FileName);

                    if (dc.ContainsKey(exts))
                    {
                        string dd = (string)dc[exts];
                        if (dd != "")
                        {
                            node.ImageKey = dd;
                            node.SelectedImageKey = dd;
                        }
                    }
                }

                //cc.Nodes.Add(node);

                tt.Nodes.Add(node);

                AddProjectReferences(p, node);

                AddProjectTypes(p, node);
            }

            return tt;
        }

        public TreeView LoadRefTypes(VSProject p)
        {
            TreeView tt = new TreeView();

            // AddProjectReferences(p, node);

            //     AddProjectTypes(p, node);

            return tt;
        }

        public void AddProjectReferences(VSProject p, TreeNode node)
        {
            TreeNode nodes = new TreeNode();
            nodes.Text = "ProjectReferences";

            if (Directory.Exists(p.FileName) == true)
                return;

            nodes.ImageKey = "Folder";
            nodes.SelectedImageKey = "Folder";

            ArrayList L = p.GetReferences("Reference");

            foreach (string s in L)
            {
                TreeNode ns = new TreeNode();
                ns.Text = s;
                nodes.Nodes.Add(ns);
                ns.ImageKey = "reference_16xLG";
                ns.SelectedImageKey = "reference_16xLG";
            }

            node.Nodes.Add(nodes);
        }

        public void AddProjectTypes(VSProject p, TreeNode node)
        {
            string file = p.GetProjectExec();

            TreeNode nodes = TreeNodeBuilder.BuildUnitNodes(file);

            TreeNode ng = RebuildNode(nodes);

            ng.Tag = p;

            node.Nodes.Add(ng);

            //node.Nodes.Add(nodes);
        }

        public void AddTypes(TreeNode node, string file)
        {
            TreeNode nodes = TreeNodeBuilder.BuildUnitNodes(file);

            TreeNode ng = RebuildNode(nodes);

            node.Nodes.Add(ng);

            //node.Nodes.Add(nodes);
        }

        public TreeNode RebuildNode(TreeNode nodes)
        {
            TreeNode ns = new TreeNode();
            ns.Text = nodes.Text;
            ns.ImageKey = "Folder_6222";
            ns.SelectedImageKey = "Folder_6222";

            if (nodes.Nodes.Count <= 0)
                return ns;

            TreeNode ng = nodes.Nodes[0];

            foreach (TreeNode b in ng.Nodes)
            {
                if (b.Text == "")
                    continue;

                ns.Nodes.Add(b);

                foreach (TreeNode g in b.Nodes)
                {
                    ArrayList L = new ArrayList();
                    foreach (TreeNode bb in g.Nodes)
                        L.Add(bb);

                    foreach (TreeNode bb in L)
                        g.Nodes.Remove(bb);

                    TreeNode cc = new TreeNode();
                    cc.Text = "Base Types";
                    cc.ImageKey = "Folder_6222";
                    cc.SelectedImageKey = "Folder_6222";
                    cc.Tag = g.Tag;

                    GetBaseTypes(cc);

                    g.Tag = L;

                    g.Nodes.Add(cc);
                }
            }

            return ns;
        }

        public void GetBaseTypes(TreeNode node)
        {
            AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> T = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

            if (T.B == null)
                return;

            if (T.B.Count <= 0)
                return;

            foreach (string s in T.B)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = s;
                nodes.ImageKey = "Class_489_24";
                nodes.SelectedImageKey = "Class_489_24";
                node.Nodes.Add(nodes);

                Type type = null;

                //try
                //{
                //    //string name = typeof("s").AssemblyQualifiedName;

                //    type = GetGlobalType(s);
                //}
                //catch (Exception e) { };

                string name = s;

                do
                {
                    type = GetGlobalType(name);

                    //string b = type.Name;

                    if (type == null)
                        break;

                    Type bb = type.BaseType;

                    if (bb != null)
                    {
                        nodes = new TreeNode();
                        nodes.Text = bb.Name;
                        nodes.ImageKey = "Class_489_24";
                        nodes.SelectedImageKey = "Class_489_24";
                        node.Nodes.Add(nodes);
                        name = bb.FullName;
                    }
                    Type[] tt = type.GetInterfaces();

                    if (bb != null)
                        foreach (Type ts in tt)
                        {
                            TreeNode ns = new TreeNode();
                            ns.Text = ts.Name;
                            ns.ImageKey = "Interface_612_24";
                            ns.SelectedImageKey = "Interface_612_24";
                            nodes.Nodes.Add(ns);
                            //name = bb.FullName;
                        }

                    if (bb == null) type = null;
                }
                while (type != null);
            }
        }

        public static Type GetGlobalType(string s)
        {
            Type t = null;
            Assembly[] av = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in av)
            {
                try
                {
                    t = Type.GetType(s + "," + a.GetName());
                }
                catch (Exception e) { }
                if (t != null)
                    break;
            }
            return t;
        }

        private void load_links(object sender, EventArgs e)
        {
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = ve_resource.BrowseFolderDialogText;

            //if (fbd.ShowDialog() == DialogResult.OK)
            //    eo.tw.MoveToSelectedFolder(fbd.SelectedPath);

            //string[] f = File.ReadAllLines(linkfile);

            //_LinksTreeView.Nodes.Clear();

            //foreach (string s in f)
            //{
            //    if (s == "")
            //        continue;

            //    _LinksTreeView.Nodes.Add(new TreeNode(Path.GetFullPath(s)));
            //}
        }

        private void OpenLinksToolStripButton_Click(object sender, EventArgs e)
        {
            //Controller cont = eo.cont;

            //ArrayList W = cont.getviews();

            //Control a = cont.getact();
            //a.Visible = false;

            //cont.nextview();
            //Control b = cont.getact();
            //b.Visible = true;

            //foreach (Control c in W)
            //{
            //    if (c != a)
            //        if (c != b)
            //            c.Visible = false;

            //}

            //utils.update_z_order(frm);
        }

        /// <summary>
        /// Handles the AfterSelect event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string s = e.Node.Text;

            //eo.tw.MoveToSelectedFolder(s);
            //  eo.tw.PopulateTreeNode(_maintv.Nodes, s, null);

            eo.tw.selectdir(s, _maintv);
        }
    }

    //}
    public class CreateView_Logger
    {
        public ToolStrip _mainToolStrip = new ToolStrip();

        public ToolStripButton _moveButton;

        public ToolStripButton _parentButton;

        public ToolStripButton _refreshButton;

        public TreeView _LoggerTreeView { get; set; }

        public TreeView _maintv { get; set; }

        public ExplorerObject eo { get; set; }

        public Form frm { get; set; }

        public void doinit(Form form)
        {
            frm = form;

            _LoggerTreeView = new TreeView();

            eo.cont.addview(_LoggerTreeView);

            //_mainToolStrip = new ToolStrip();
            //_refreshButton = new ToolStripButton();
            //_parentButton = new ToolStripButton();
            //_moveButton = new ToolStripButton();

            //_mainToolStrip.Items.Add(_refreshButton);
            //_mainToolStrip.Items.Add(_parentButton);
            //_mainToolStrip.Items.Add(_moveButton);

            //_refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_refreshButton.Image = resources_vs.LinkedServer;
            //_refreshButton.ImageTransparentColor = Color.Magenta;
            //_refreshButton.Name = "Constants.UI_TOOLBAR_REFRESH";
            //_refreshButton.Text = "Load Links";
            //_refreshButton.Click += new System.EventHandler(LoadLinksToolStripButton_Click);//celegate { eo._applicationManager.NotifyFileSystemChange(); };

            //_parentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_parentButton.Image = resources_vs.GoToReference_5542_32;
            //_parentButton.ImageTransparentColor = Color.Magenta;
            //_parentButton.Name = "Constants.UI_TOOLBAR_MOVE_PARENT";
            //_parentButton.Text = "Links";
            //_parentButton.Click += new System.EventHandler(OpenLinksToolStripButton_Click);

            //_moveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //_moveButton.Image = ve_resource.OpenFolder;
            //_moveButton.ImageTransparentColor = Color.Magenta;
            //_moveButton.Name = Constants.UI_TOOLBAR_MOVE_NEW;
            //_moveButton.Text = ve_resource.ToolbarMoveRoot;
            //_moveButton.Click += new System.EventHandler(MoveToolStripButton_Click);

            _LoggerTreeView.AfterSelect +=
               new TreeViewEventHandler(TreeView_AfterSelect);

            _LoggerTreeView.Dock = DockStyle.Fill;

            frm.Controls.Add(_LoggerTreeView);

            _LoggerTreeView.ResumeLayout(false);
            _LoggerTreeView.PerformLayout();

            _LoggerTreeView.Nodes.Add(new TreeNode("Solution Logger"));

            eo.cont.registerlogger(_LoggerTreeView);

            //        linkfile = "C:\\a.txt";
        }

        public void loadsolutioninfo(ArrayList PL)
        {
        }

        public void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node == null)
                return;

            if (node.Tag == null)
                return;

            if (node.Tag.GetType() != typeof(CreateView_Solution.ProjectItemInfo))
                return;

            CreateView_Solution.ProjectItemInfo prs = node.Tag as CreateView_Solution.ProjectItemInfo;

            foreach (VSProjectItem pi in prs.ps.Items)
            {
                TreeNode nods = new TreeNode();
                nods.Text = pi.Name;
                nods.Tag = pi;
                node.Nodes.Add(nods);
            }
        }
    }

    //        if (File.Exists(linkfile) == false)
    //            File.Create(linkfile);
    public class CreateView_Solution
    {
        public ToolStripSplitButton _ddButton;

        public ToolStripButton _prevButton;

        public ToolStripButton _nextButton;

        public ToolStripSeparator _separator;

        public ToolStripDropDownButton _undoButton;

        public ToolStripDropDownButton _redoButton;

        public ToolStripSeparator _separators;

        public ToolStripButton _refreshButton;

        public ToolStripDropDownButton _newButton;

        public ToolStripButton _saveButton;

        public ToolStripButton _saveallButton;

        public ToolStripSeparator _separators2;

        public ToolStripDropDownButton _adjustButton;

        public ToolStripButton _commentButton;

        public ToolStripButton _uncommentButton;

        public ToolStripButton _addButton;

        public ToolStripButton _buildButton;

        public ToolStripButton _buildprojectButton;

        public ToolStripButton _currentsolution;

        public ToolStripComboBoxes _configList;

        public ToolStripComboBoxes _platList;

        public ToolStripComboBoxes _projectList;

        public ToolStrip _mainToolStrip = new ToolStrip();

        public ToolStripButton _opensolutionButton;

        public ToolStripButton _findinfiles;

        public ToolStripButton _parentButton;

        public ToolStripButton _refreshprojectButton;

        public ToolStripButton _removeButton;

        public ToolStripButton _customizeButton;

        public ToolStripButton _resetButton;


        public int baseValue = 0;

        public TreeView _maintv { get; set; }

        public TreeView _SolutionTreeView { get; set; }

        public Dictionary<string, string> ds { get; set; }

        public ExplorerObject eo { get; set; }

        public Form frm { get; set; }

        public ImageList imageList1 { get; set; }

        public ListView imageListView1 { get; set; }

        public string linkfile { get; set; }

        public ListView listView1 { get; set; }

        public ClassMapper mapper { get; set; }

        public ArrayList mappers { get; set; }

        public MainTest mf { get; set; }

        public msbuilder_alls ms { get; set; }

        public OpenFileDialog ofd { get; set; }

        public string slnpath { get; set; }

        private Dictionary<string, object> dict { get; set; }

        private Dictionary<string, object> dict2 { get; set; }

        private Dictionary<string, object> dict3 { get; set; }

        private SaveFileDialog sfd { get; set; }

        private string slnfile { get; set; }

        public History hst { get; set; }

        static public ImageList addImage(Bitmap bmp, string name, ImageList imgL)
        {
            if (imgL == null)
                imgL = new ImageList();

            if (bmp != null)
            {
                imgL.Images.Add(name, (Image)bmp);
                //listView1.BeginUpdate();
                //listView1.Items.Add(imageToLoad, baseValue++);
                //listView1.EndUpdate();
            }

            return imgL;
        }

        static public ImageList getImageResources(string foldername)
        {
            ImageList imgL = new ImageList();

            Dictionary<string, object> dict = new Dictionary<string, object>();

            string s = AppDomain.CurrentDomain.BaseDirectory;

            //if (dict == null)
            {
                dict = Load(s + "\\" + "WinExplorer.exe", "WinExplorer.resources-vs.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imgL);
                }
            }

            //if (dict == null)
            {
                dict = Load(s + "\\" + "WinExplorer.exe", "WinExplorer.resource-vsc.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imgL);
                }
            }

            //if (dict == null)
            {
                dict = Load(s + "\\" + "WinExplorer.exe", "WinExplorer.Resources_refs.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imgL);
                }
            }

            //ImgList_toFolder(foldername, imgL);

            return imgL;
        }

        static public void ImgList_toFolder(string foldername, ImageList imgL)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            s = s + "\\" + foldername;

            if (Directory.Exists(s) == false)
                Directory.CreateDirectory(s);

            foreach (string name in imgL.Images.Keys)
            {
                Image image = (Image)imgL.Images[name];

                image.Save(s + "\\" + name);
            }
        }

        public static Dictionary<string, object> Load(string ase, string res)
        {
            Dictionary<string, object> dicAttributes = new Dictionary<string, object>();

            ArrayList L = new ArrayList();

            ArrayList V = new ArrayList();

            Assembly assem = Assembly.LoadFile(ase);

            string[] rs = assem.GetManifestResourceNames();

            Stream r = assem.GetManifestResourceStream(res/*"WinExplorer.resources-vs.resources"*/);

            System.Resources.ResourceReader mg = new System.Resources.ResourceReader(r);

            string tempFile = "";

            foreach (var item in mg)
            {
                dicAttributes.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString(), ((System.Collections.DictionaryEntry)(item)).Value);

                L.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString());

                V.Add(((System.Collections.DictionaryEntry)(item)).Value);
            }

            return dicAttributes;
        }

        public static string[] Loads(string ase)
        {
            Assembly assem = Assembly.LoadFile(ase);

            // Stream r = null;

            //  Stream r = assem.GetManifestResourceStream("resource_vs");

            string[] rs = assem.GetManifestResourceNames();

            return rs;
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

        public void add_project_to_solution(string projects, string filename)
        {
            //DirectoryInfo project = new DirectoryInfo("");

            //DirectoryInfo folder = new DirectoryInfo("");

            //CopyAll(project, folder);

            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(projects);

            string s = filename;// pc.FullPath + "\\" + filepath;

            if (File.Exists(s) != true)
                File.Create(s);

            IList<Microsoft.Build.Evaluation.ProjectItem> items = pc.AddItem("Compile", Path.GetFileName(filename));

            //pc.ProjectCollection.LoadedProjects.Add(pc);

            pc.Save();

            foreach (Microsoft.Build.Evaluation.ProjectItem prs in items)
            {
                if (prs.ItemType != "Compile")
                    continue;

                if (prs.UnevaluatedInclude == filename)
                    MessageBox.Show("New compile item + " + filename + "has been created");
            }

            pc = null;
        }

        public void add_project_item(string projects, string filename)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(projects);

            string s = filename;// pc.FullPath + "\\" + filepath;

            if (File.Exists(s) != true)
                File.Create(s);

            IList<Microsoft.Build.Evaluation.ProjectItem> items = pc.AddItem("Compile", Path.GetFileName(filename));

            pc.Save();

            foreach (Microsoft.Build.Evaluation.ProjectItem prs in items)
            {
                if (prs.ItemType != "Compile")
                    continue;

                if (prs.UnevaluatedInclude == filename)
                    MessageBox.Show("New compile item + " + filename + "has been created");
            }

            pc = null;
        }

        public void add_and_create_file()
        {
            //ApplicationManager app = ApplicationManager.GetInstance();

            //TreeView tv = _SolutionTreeView;

            //if(tv.SelectedNode == null)
            //    return;

            //TreeNode nodes = tv.SelectedNode;

            //if(nodes.Tag == null)
            //    return;

            ////VSProject project = msbuilder_alls.getvsproject(slnpath, 10);

            //if(nodes.Tag.GetType() != typeof(ProjectItemInfo))
            //    return;

            //ProjectItemInfo project = nodes.Tag as ProjectItemInfo;

            // Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(project.ps.FileName);

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            string path = prs.ps.FileName;

            string projectdir = Path.GetFullPath(path).Replace(Path.GetFileName(path), "");

            sfd = new SaveFileDialog();
            sfd.InitialDirectory = prs.vs.SolutionPath;

            DialogResult r = sfd.ShowDialog();

            if (r != DialogResult.OK)
                return;

            add_project_item(path, sfd.FileName);

            // return;

            // string filepath = sfd.FileName;// "newprojectitem.cs";

            // string s = filepath;// pc.FullPath + "\\" + filepath;

            //if (File.Exists(s) != true)
            //    File.Create(s);

            // IList<Microsoft.Build.Evaluation.ProjectItem> items = pc.AddItem("Compile", Path.GetFileName(filepath));
            // pc.Save();

            // foreach (Microsoft.Build.Evaluation.ProjectItem prs in items)
            // {
            //     if (prs.ItemType != "Compile")
            //         continue;

            //     if (prs.UnevaluatedInclude == filepath)
            //         MessageBox.Show("New compile item + " + filepath + "has been created");

            // }

            // pc = null;
        }

        public void AddSolutionItem_Click(object sender, EventArgs e)
        {
            add_and_create_file();
        }

        public TreeNode MainProjectNode { get; set; }

        public ProjectItemInfo MainProjectItem { get; set; }

        private Font fonts { get; set; }

        public void set_main_project()
        {
            if (font == null)
                font = new Font(_SolutionTreeView.Font, FontStyle.Regular);

            if (fonts == null)
                fonts = new Font(_SolutionTreeView.Font, FontStyle.Bold);

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            _SolutionTreeView.BeginUpdate();

            VSSolution vs = GetVSSolution();

            VSProject vp = GetVSProject();

            vs.MainVSProject = vp;

            MainProjectNode = eo.cvs.getactivenode();

            MainProjectItem = prs;

            MessageBox.Show("Main project name " + prs.ps.Name);

            TreeNode nodes = MainProjectNode;

            while (nodes.Parent != null)
                nodes = nodes.Parent;

            unboldnodes(nodes);

            MainProjectNode.Text = MainProjectNode.Text;
            MainProjectNode.NodeFont = fonts;

            _SolutionTreeView.EndUpdate();
        }
        public void SetVSProject(string mainproject)
        {
            if (font == null)
                font = new Font(_SolutionTreeView.Font, FontStyle.Regular);

            if (fonts == null)
                fonts = new Font(_SolutionTreeView.Font, FontStyle.Bold);

            //WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            //if (prs == null)
            //{
            //    MessageBox.Show("No project has been selected..");
            //    return;
            //}

            _SolutionTreeView.BeginUpdate();

            VSSolution vs = GetVSSolution();

            VSProject vp = vs.GetProjectbyName(mainproject);

            //VSProject vp = GetVSProject();

            vs.MainVSProject = vp;

            MainProjectNode = eo.cvs.getprojectenode(vp.FileName);

            //MainProjectItem = prs;

            //MessageBox.Show("Main project name " + prs.ps.Name);

            TreeNode nodes = MainProjectNode;

            if (nodes != null)
            {

                while (nodes.Parent != null)
                    nodes = nodes.Parent;

                unboldnodes(nodes);

            }

            MainProjectNode.Text = MainProjectNode.Text;
            MainProjectNode.NodeFont = fonts;

            _SolutionTreeView.EndUpdate();
        }
        public void main_project()
        {
            if (font == null)
                font = new Font(_SolutionTreeView.Font, FontStyle.Regular);

            if (fonts == null)
                fonts = new Font(_SolutionTreeView.Font, FontStyle.Bold);

            _SolutionTreeView.BeginInvoke(new Action(() =>
            {
                _SolutionTreeView.BeginUpdate();

                VSSolution vs = GetVSSolution();

                if (vs == null)
                    return;

                VSProject vp = GetVSProject();

                if (vs.MainVSProject != null)
                    MainProjectNode = eo.cvs.getprojectenode(vs.MainVSProject.FileName);

                TreeNode nodes = MainProjectNode;

                if (nodes != null)
                {
                    while (nodes.Parent != null)
                        nodes = nodes.Parent;

                    unboldnodes(nodes);

                    MainProjectNode.Text = MainProjectNode.Text;
                    MainProjectNode.NodeFont = fonts;
                }

                _SolutionTreeView.EndUpdate();
            }));
        }

        public static Font font { get; set; }

        public void unboldnodes(TreeNode node)
        {
            if (font == null)
                font = new Font(_SolutionTreeView.Font, FontStyle.Regular);

            if (node == null)
                return;

            TreeNode ns = node;// node.Parent;

            if (ns == null)
                return;

            foreach (TreeNode ng in ns.Nodes)
            {
                ng.NodeFont = font;

                unboldnodes(ng);
            }
        }

        public void add_create_file(string file)
        {
            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            string path = prs.ps.FileName;

            string projectdir = Path.GetFullPath(path).Replace(Path.GetFileName(path), "");

            //sfd = new SaveFileDialog();
            //sfd.InitialDirectory = prs.vs.SolutionPath;

            //DialogResult r = sfd.ShowDialog();

            //if (r != DialogResult.OK)
            //    return;

            add_project_item(path, projectdir + "\\" + file);
        }

        public TreeNode FindNode(TreeNode node, string file)
        {
            foreach (TreeNode nodes in node.Nodes)
            {
                ProjectItemInfo p = nodes.Tag as ProjectItemInfo;

                if (p != null)
                    if (p.psi != null)
                        if (p.psi.fileName == file)
                            return nodes;

                TreeNode ns = FindNode(nodes, file);

                if (ns != null)
                    return ns;
            }

            return null;
        }


        public TreeNode FindNode(string file)
        {
            TreeView b = _SolutionTreeView;



            foreach (TreeNode node in b.Nodes)
            {
                ProjectItemInfo p = node.Tag as ProjectItemInfo;

                if (p != null)
                    if (p.psi != null)
                        if (p.psi.fileName == file)
                            return node;

                TreeNode nodes = FindNode(node, file);

                if (nodes != null)
                    return nodes;
            }

            return null;
        }

        public void AnalyzeCSharpFile(string path, TreeNode nods, string content)
        {
            StringBuilder b = new StringBuilder();



            CacheManager wm = new CacheManager();

            SyntaxTree syntax = null;

            mapper = new ClassMapper();

            if (mappers == null)
                mappers = new ArrayList();

            mappers.Add(mapper);

            mapper.filename = path;

            string filename = path;


            string sourceText = content;

            CSharpParser parser = new CSharpParser();
            SyntaxTree syntaxTree2 = parser.Parse(sourceText, Path.GetFileNameWithoutExtension(filename));
            var pc = new CSharpProjectContent();
            pc = (CSharpProjectContent)pc.AddOrUpdateFiles(syntaxTree2.ToTypeSystem());
            ICSharpCode.NRefactory.TypeSystem.ICompilation compilation = pc.CreateCompilation();
            var resolver = new ICSharpCode.NRefactory.CSharp.Resolver.CSharpAstResolver(compilation, syntaxTree2, syntaxTree2.ToTypeSystem());

            var classVisitor = new ClassVisitor(resolver);
            classVisitor.filename = path;
            classVisitor.b = b;
            classVisitor.wm = wm;
            classVisitor.mapper = mapper;

            classVisitor.syntax = syntaxTree2;

            try
            {
                syntaxTree2.AcceptVisitor(classVisitor);
            }
            catch (Exception e) { }

            syntax = syntaxTree2;


            string ents = nods.Text;

            ents = mapper.classname;

            IEnumerable<EntityDeclaration> doc = getentity(syntax, ents);

            if (doc == null)
                return;

            TreeNode bb = nods;

            int nc = 0;

            foreach (EntityDeclaration ent in doc)
            {
                AstNode asc = ent as AstNode;

                if (doc.Count() >= 1)
                {
                    if (bb.Nodes.Count > nc)
                        nods = bb.Nodes[nc++];
                    else
                    {
                        nods = new TreeNode();
                        bb.Nodes.Add(nods);
                    }


                    nods.Text = asc.NodeType.ToString();

                    if (asc.GetType() == typeof(DelegateDeclaration))
                    {
                        DelegateDeclaration d = (DelegateDeclaration)asc;

                        nods.Text = d.Name;

                        nods.ImageKey = "delegate";
                        nods.SelectedImageKey = "delegate";
                    }
                    else if (asc.GetType() == typeof(TypeDeclaration))
                    {
                        TypeDeclaration d = (TypeDeclaration)asc;

                        nods.Text = d.Name;

                        if (d.ClassType == ClassType.Interface)
                        {
                            nods.ImageKey = "Interface_Shortcut_617_24";
                            nods.SelectedImageKey = "Interface_Shortcut_617_24";
                        }
                        else if (d.ClassType == ClassType.Enum)
                        {
                            nods.ImageKey = "Enum_582_24";
                            nods.SelectedImageKey = "Enum_582_24";
                        }
                        else
                        {
                            nods.ImageKey = "class";
                            nods.SelectedImageKey = "class";
                        }
                    }
                }

                int nw = 0;

                ArrayList N = ArrayList.Adapter(nods.Nodes);

                foreach (AstNode ast in asc.DescendantsAndSelf)
                {
                    if (ast.NodeType.ToString() == "MemberType" || ast.NodeType.ToString() == "Member" || ast.NodeType.ToString() == "Field")
                    {
                        bool cached = false;

                        ProjectItemInfo pr = new ProjectItemInfo();
                        pr.filepath = mapper.filename;

                        TreeNode nv = new TreeNode(ast.ToString() + "-----" + ast.NodeType.ToString() + "----" + ast.GetType().FullName);

                        if (nods.Nodes.Count > nw)
                        {
                            nv = N[nw] as TreeNode;
                            cached = true;
                        }

                        nw++;

                        Type T = ast.GetType();

                        if (ast.GetType() == typeof(MethodDeclaration))
                        {
                            MethodDeclaration dec2 = (MethodDeclaration)ast;

                            foreach (Statement s in dec2.Body.Statements)
                            {
                                if (s.GetType() == typeof(VariableDeclarationStatement))
                                {
                                    //VariableDeclarationStatement d = (VariableDeclarationStatement)s;
                                    //string types = d.Type.ToString();

                                    //MessageBox.Show("Variable declaration found - " + types);
                                }
                            }

                            nv.Text = dec2.Name;

                            string pp = "( ";

                            int NN = dec2.Parameters.Count - 1;
                            int i = 0;
                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                if (i == NN)
                                    pp += p.Type.ToString();
                                else
                                    pp += p.Type.ToString() + " ,";
                                i++;
                            }

                            nv.Text += " " + pp + ") : " + dec2.ReturnType.ToString();



                            Modifiers m = dec2.Modifiers;
                            if (m == Modifiers.Protected)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.Private)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.Sealed)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.New)
                                nv.ImageKey = "NewMethods_6859_32";
                            else if (m == Modifiers.Abstract)
                                nv.ImageKey = "AbstractClass_8337_32";
                            else if (m == Modifiers.Public)
                                nv.ImageKey = "method";// "PROPERTIES";//Method_636_322";
                            else nv.ImageKey = "method";// "PROPERTIES";//Method_636_322";

                            nv.SelectedImageKey = "method";

                            //Interface_Shortcut_617_24

                            nv.Tag = path + "@" + dec2.Region.BeginLine;
                            if (cached == false)
                                nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec2 = (ConstructorDeclaration)ast;

                            nv.Text = dec2.Name;

                            string pp = "( ";

                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                pp += p.Name + " ,";
                            }

                            nv.Text += " " + pp + ") ";

                            nv.Tag = path + "@" + dec2.Region.BeginLine;

                            nv.ImageKey = "class";
                            nv.SelectedImageKey = "class";
                            if (cached == false)
                                nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(FieldDeclaration))
                        {
                            FieldDeclaration dec3 = (FieldDeclaration)ast;

                            nv.Text = dec3.Name;

                            string pp = "";

                            foreach (VariableInitializer v in dec3.Variables)
                            {
                                pp += v.Name + " ";
                            }

                            nv.Text += " " + pp + " : " + dec3.ReturnType.ToString();

                            nv.Tag = path + "@" + dec3.Region.BeginLine;

                            nv.ImageKey = "field";
                            nv.SelectedImageKey = "field";
                            if (cached == false)
                                nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec4 = (ConstructorDeclaration)ast;

                            nv.Text = dec4.Name;

                            string pp = "";

                            foreach (ParameterDeclaration v in dec4.Parameters)
                            {
                                pp += v.Name + " ";
                            }

                            nv.Text += " " + pp;

                            nv.ImageKey = "Property_Private_505_24";
                        }
                        else if (ast.GetType() == typeof(PropertyDeclaration))
                        {
                            PropertyDeclaration dec5 = (PropertyDeclaration)ast;

                            nv.Text = dec5.Name;

                            string pp = "";

                            Accessor ac = dec5.Getter;
                            nv.Text += " " + ac.Name;

                            ac = dec5.Setter;
                            nv.Text += " " + ac.Name + " : " + dec5.ReturnType.ToString();

                            nv.Tag = path + "@" + dec5.Region.BeginLine;

                            nv.ImageKey = "property";
                            nv.SelectedImageKey = "property";
                            if (cached == false)
                                nods.Nodes.Add(nv);

                            //Property_Private_505_24";
                        }

                        //nods.Nodes.Add(nv);

                        ProjectItemInfo pr0 = new ProjectItemInfo();
                        pr0.mapper = mapper;

                        //nv.Tag = pr;

                        // string bc = getnodebyrole(ast, "TypeMember", nv);
                    }
                }

                while (nw < nods.Nodes.Count)
                {
                    nods.Nodes[nw].Remove();
                }
            }
        }



        public void AnalyzeCSharpFile(string path, TreeNode nods, bool newnode)
        {
            StringBuilder b = new StringBuilder();



            CacheManager wm = new CacheManager();

            SyntaxTree syntax = null;

            mapper = new ClassMapper();

            if (mappers == null)
                mappers = new ArrayList();

            mappers.Add(mapper);

            mapper.filename = path;

            string filename = path;

            if (File.Exists(filename) == false)
                return;

            string sourceText = File.ReadAllText(filename);

            CSharpParser parser = new CSharpParser();
            SyntaxTree syntaxTree2 = null;

            try
            {
                syntaxTree2 = parser.Parse(sourceText, Path.GetFileNameWithoutExtension(filename));
            }
            catch (Exception e)
            {
                return;
            }
            var pc = new CSharpProjectContent();
            pc = (CSharpProjectContent)pc.AddOrUpdateFiles(syntaxTree2.ToTypeSystem());
            ICSharpCode.NRefactory.TypeSystem.ICompilation compilation = pc.CreateCompilation();
            var resolver = new ICSharpCode.NRefactory.CSharp.Resolver.CSharpAstResolver(compilation, syntaxTree2, syntaxTree2.ToTypeSystem());

            var classVisitor = new ClassVisitor(resolver);
            classVisitor.filename = path;
            classVisitor.b = b;
            classVisitor.wm = wm;
            classVisitor.mapper = mapper;

            classVisitor.syntax = syntaxTree2;

            try
            {
                syntaxTree2.AcceptVisitor(classVisitor);
            }
            catch (Exception e) { }

            syntax = syntaxTree2;

            if (newnode == false)
            {
                //CreateSyntaxView(_SolutionTreeView, syntax, nods);
                //  return;
            }

            string ents = nods.Text;

            ents = mapper.classname;

            IEnumerable<EntityDeclaration> doc = getentity(syntax, ents);

            if (doc == null)
                return;

            TreeNode bb = nods;

            foreach (EntityDeclaration ent in doc)
            {
                AstNode asc = ent as AstNode;

                if (doc.Count() >= 1)
                {
                    nods = new TreeNode();
                    nods.Text = asc.NodeType.ToString();

                    bb.Nodes.Add(nods);

                    if (asc.GetType() == typeof(DelegateDeclaration))
                    {
                        DelegateDeclaration d = (DelegateDeclaration)asc;

                        nods.Text = d.Name;

                        nods.ImageKey = "delegate";
                        nods.SelectedImageKey = "delegate";
                    }
                    else if (asc.GetType() == typeof(TypeDeclaration))
                    {
                        TypeDeclaration d = (TypeDeclaration)asc;

                        nods.Text = d.Name;

                        if (d.ClassType == ClassType.Interface)
                        {
                            nods.ImageKey = "Interface_Shortcut_617_24";
                            nods.SelectedImageKey = "Interface_Shortcut_617_24";
                        }
                        else if (d.ClassType == ClassType.Enum)
                        {
                            nods.ImageKey = "Enum_582_24";
                            nods.SelectedImageKey = "Enum_582_24";
                        }
                        else
                        {
                            nods.ImageKey = "class";
                            nods.SelectedImageKey = "class";
                        }
                    }
                }

                foreach (AstNode ast in asc.DescendantsAndSelf)
                {
                    if (ast.NodeType.ToString() == "MemberType" || ast.NodeType.ToString() == "Member" || ast.NodeType.ToString() == "Field")
                    {
                        ProjectItemInfo pr = new ProjectItemInfo();
                        pr.filepath = mapper.filename;

                        TreeNode nv = new TreeNode(ast.ToString() + "-----" + ast.NodeType.ToString() + "----" + ast.GetType().FullName);

                        Type T = ast.GetType();

                        if (ast.GetType() == typeof(MethodDeclaration))
                        {
                            MethodDeclaration dec2 = (MethodDeclaration)ast;

                            foreach (Statement s in dec2.Body.Statements)
                            {
                                if (s.GetType() == typeof(VariableDeclarationStatement))
                                {
                                    //VariableDeclarationStatement d = (VariableDeclarationStatement)s;
                                    //string types = d.Type.ToString();

                                    //MessageBox.Show("Variable declaration found - " + types);
                                }
                            }

                            nv.Text = dec2.Name;

                            string pp = "( ";
                            int N = dec2.Parameters.Count - 1;
                            int i = 0;
                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                if (i == N)
                                    pp += p.Type.ToString();
                                else
                                    pp += p.Type.ToString() + " ,";
                                i++;
                            }

                            nv.Text += " " + pp + ") : " + dec2.ReturnType.ToString();



                            Modifiers m = dec2.Modifiers;
                            if (m == Modifiers.Protected)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.Private)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.Sealed)
                                nv.ImageKey = "method";
                            else if (m == Modifiers.New)
                                nv.ImageKey = "NewMethods_6859_32";
                            else if (m == Modifiers.Abstract)
                                nv.ImageKey = "AbstractClass_8337_32";
                            else if (m == Modifiers.Public)
                                nv.ImageKey = "method";// "PROPERTIES";//Method_636_322";
                            else nv.ImageKey = "method";// "PROPERTIES";//Method_636_322";

                            nv.SelectedImageKey = "method";

                            //Interface_Shortcut_617_24

                            nv.Tag = path + "@" + dec2.Region.BeginLine;

                            nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec2 = (ConstructorDeclaration)ast;

                            nv.Text = dec2.Name;

                            string pp = "( ";

                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                pp += p.Name + " ,";
                            }

                            nv.Text += " " + pp + ") ";

                            nv.ImageKey = "class";
                            nv.SelectedImageKey = "class";

                            nv.Tag = path + "@" + dec2.Region.BeginLine;

                            nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(FieldDeclaration))
                        {
                            FieldDeclaration dec3 = (FieldDeclaration)ast;

                            nv.Text = dec3.Name;

                            string pp = "";

                            foreach (VariableInitializer v in dec3.Variables)
                            {
                                pp += v.Name + " ";
                            }

                            nv.Text += " " + pp + " : " + dec3.ReturnType.ToString();

                            nv.ImageKey = "field";
                            nv.SelectedImageKey = "field";

                            nv.Tag = path + "@" + dec3.Region.BeginLine;

                            nods.Nodes.Add(nv);
                        }
                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec4 = (ConstructorDeclaration)ast;

                            nv.Text = dec4.Name;

                            string pp = "";

                            foreach (ParameterDeclaration v in dec4.Parameters)
                            {
                                pp += v.Name + " ";
                            }

                            nv.Text += " " + pp;

                            nv.ImageKey = "Property_Private_505_24";
                        }
                        else if (ast.GetType() == typeof(PropertyDeclaration))
                        {
                            PropertyDeclaration dec5 = (PropertyDeclaration)ast;

                            nv.Text = dec5.Name;

                            string pp = "";

                            Accessor ac = dec5.Getter;
                            nv.Text += " " + ac.Name;

                            ac = dec5.Setter;
                            nv.Text += " " + ac.Name + " : " + dec5.ReturnType.ToString();

                            nv.ImageKey = "property";
                            nv.SelectedImageKey = "property";

                            nods.Nodes.Add(nv);

                            nv.Tag = path + "@" + dec5.Region.BeginLine;

                            //Property_Private_505_24";
                        }

                        //nods.Nodes.Add(nv);

                        ProjectItemInfo pr0 = new ProjectItemInfo();
                        pr0.mapper = mapper;

                        //nv.Tag = pr;

                        // string bc = getnodebyrole(ast, "TypeMember", nv);
                    }
                }
            }
        }

        public void build_project_solution(string path)
        {
            slnpath = path;

            if (slnpath == "")
                return;
            if (slnpath == null)
                return;

            MessageBox.Show("Build for project from solution " + slnpath);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            BasicFileLogger logger = msbuilder_alls.build_alls_project(slnpath);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void build_project_solution(object sender, EventArgs e)
        {
            //build_project_solution();
        }

        //public QuickSharp.Outputs.OutputForms forms { get; set; }

        public ErrorListForm ef { get; set; }

        public void build_solution2(string file, bool rebuild)
        {
            slnpath = file;

            if (slnpath == null)
                return;

            if (slnpath == "")
                return;

            //MessageBox.Show("Build for solution " + slnpath);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            BasicFileLogger logger = msbuilder_alls.build_alls(slnpath, rebuild);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            //forms.SelectListView();

            ef.ClearOutput();

            ef.Invoke(new Action(() => { ef.LoadCompileResults(logger.Es, logger.Ws, logger.Me); }));
        }



        public void build_solution(string file, bool rebuild, msbuilder_alls.MSBuild msbuilds)
        {
            slnpath = file;

            if (slnpath == null)
                return;

            if (slnpath == "")
                return;

            //MessageBox.Show("Build for solution " + slnpath);

            string b = AppDomain.CurrentDomain.BaseDirectory;


            //msbuilder_alls.MSBuild msbuild = new msbuilder_alls.MSBuild();

            //msbuild.MSBuildFile = file;

            msbuilds.IP = "127.0.0.1";
            msbuilds.Port = "6787";
            msbuilds.LoggerClass = "Loggers.TcpIpLogger";
            msbuilds.LoggerDLL = b + "Loggers.dll";
            msbuilds.msbuildPath = "C:\\Program Files (x86)\\MSBuild\\14.0\\Bin\\MSBuild.exe";

            //var toolsetVersion = ToolLocationHelper.CurrentToolsVersion;
            //var msbuildDir = ToolLocationHelper.GetPathToBuildTools("v140");


            ef.BeginInvoke(new Action(() => { ef.ClearOutput(); }));

            msbuilder_alls.Server server = null;

          
            var c = System.Threading.Tasks.Task.Run(() =>
            {

                msbuilder_alls.ExecuteMsbuildProject(msbuilds);

               

            }
          );
            var t = System.Threading.Tasks.Task.Run(() =>
            {



                server = new msbuilder_alls.Server(ef);
                server.Close();

            }

           
  );

      

            //BasicFileLogger logger = msbuilder_alls.build_alls(slnpath, rebuild);

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            //forms.SelectListView();

            //ef.ClearOutput();
            //ef.Invoke(new Action(() => { ef.LoadCompileResults(logger.Es, logger.Ws, logger.Me); }));

            //server.running = false;
            

            
        }


        // Returns the bounds of the specified node, including the region
        // occupied by the node label and any node tag displayed.
        private Rectangle NodeBounds(TreeNode node)
        {
            // Set the return value to the normal node bounds.
            Rectangle bounds = node.Bounds;
            if (node.Tag != null)
            {
                // Retrieve a Graphics object from the TreeView handle
                // and use it to calculate the display width of the tag.
                Graphics g = _SolutionTreeView.CreateGraphics();
                int tagWidth = (int)g.MeasureString
                    (node.Tag.ToString(), _tagFont).Width + 6;

                // Adjust the node bounds using the calculated value.
                bounds.Offset(tagWidth / 2, 0);
                bounds = Rectangle.Inflate(bounds, tagWidth / 2, 0);
                g.Dispose();
            }

            return bounds;
        }

        // Draws a node.
        private void myTreeView_DrawNode(
            object sender, DrawTreeNodeEventArgs e)
        {
            // Draw the background and node text for a selected node.
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                // Draw the background of the selected node. The NodeBounds
                // method makes the highlight rectangle large enough to
                // include the text of a node tag, if one is present.
                e.Graphics.FillRectangle(Brushes.Green, NodeBounds(e.Node));

                // Retrieve the node font. If the node font has not been set,
                // use the TreeView font.
                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;

                // Draw the node text.
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White,
                    Rectangle.Inflate(e.Bounds, 2, 0));
            }

            // Use the default background and node text.
            else
            {
                e.DrawDefault = true;
            }

            // If a node tag is present, draw its string representation
            // to the right of the label text.
            if (e.Node.Tag != null)
            {
                e.Graphics.DrawString(e.Node.Tag.ToString(), _tagFont,
                    Brushes.Yellow, e.Bounds.Right + 2, e.Bounds.Top);
            }

            // If the node has focus, draw the focus rectangle large, making
            // it large enough to include the text of the node tag, if present.
            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Blue))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = NodeBounds(e.Node);
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
        }

        // Create a Font object for the node tags.
        private Font _tagFont = new Font("Helvetica", 8, FontStyle.Bold);

        public void build_solution(object sender, EventArgs e)
        {
            //build_solution();
        }

        //public string checkfolder(string p)
        //{
        //    string[] dirs = p.Split("\\".ToCharArray());
        //    int i = 0;
        //    while (i < dirs.Length)
        //    {
        //        string seg = dirs[i];

        //        bool result = false;
        //        DirectoryInfo directoryInfo = new DirectoryInfo(p);
        //        foreach (Environment.SpecialFolder suit in Enum.GetValues(typeof(Environment.SpecialFolder)))
        //        {
        //            string name = suit.ToString();

        //            if (seg == name)
        //            {
        //                seg = Environment.GetFolderPath(suit);

        //                string path = seg;

        //                i++;

        //                while (i < dirs.Length)
        //                {
        //                    path += "\\" + dirs[i++];
        //                }

        //                return path;
        //            }
        //        }

        //        i++;
        //    }

        //    return p;
        //}

        public void CreateSyntaxView(TreeView tv, SyntaxTree syntax, TreeNode nods)
        {
            TreeNode node = new TreeNode();
            node.Text = "ParserDescriptor";
            tv.Nodes.Add(node);

            //foreach(AstNode ast in syntax.Children)
            //{
            //    TreeNode ns = new TreeNode(ast.ToString());
            //    node.Nodes.Add(ns);

            //}

            ArrayList ars = new ArrayList();

            TokenRole r = new TokenRole("class");

            TreeNode cls = new TreeNode("classes");

            string classname = "";

            foreach (AstNode ast in syntax.Descendants)
            {
                TreeNode ns = new TreeNode(ast.ToString());

                if (ast.NodeType.ToString() == "TypeDeclaration" || ast.NodeType.ToString() == "Member")
                    node.Nodes.Add(ns);

                if (ast.NodeType.ToString() == "TypeDeclaration" || ast.NodeType.ToString() == "Member")
                {
                    TreeNode nv = new TreeNode("Tokens");

                    ns.Nodes.Add(nv);

                    TreeNode arsc = new TreeNode("token class");

                    //AstNodeCollection<CSharpTokenNode> anc = ast.GetChildrenByRole(r);
                    //foreach (CSharpTokenNode a in anc)
                    //{
                    //    AstNode nst = a.GetNextNode();
                    //    if (nst.Role.ToString() == "Identifier")
                    //    {
                    //        TreeNode nc = new TreeNode(nst.Role.ToString());
                    //        arsc.Nodes.Add(nc);
                    //    }
                    //}
                    //ars.Add(arsc);

                    bool isclass = false;
                    bool isinterface = false;

                    foreach (AstNode nn in ast.Descendants)
                    {
                        TreeNode nt = new TreeNode("Tokens - " + nn.NodeType.ToString() + "  " + nn.Role.ToString());
                        ns.Nodes.Add(nt);

                        if (nn.NodeType.ToString() == "Token" && nn.Role.ToString() == "class")
                        {
                            AstNode next = nn.GetNextNode();

                            AstNode prev = nn.GetPrevNode();

                            if (nn.GetNextNode().Role.ToString() == "Identifier")
                            {
                                TreeNode tk = new TreeNode(nn.ToString());
                                nt.Nodes.Add(tk);

                                TreeNode te = new TreeNode(prev.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(nn.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(next.ToString());
                                classname = next.ToString();

                                arsc.Nodes.Add(te);
                                te = new TreeNode(ast.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(ast.Region.ToString());
                                arsc.Nodes.Add(te);

                                isclass = true;
                            }
                        }
                        if (nn.NodeType.ToString() == "Token" && nn.Role.ToString() == "interface")
                        {
                            AstNode next = nn.GetNextNode();

                            AstNode prev = nn.GetPrevNode();

                            if (nn.GetNextNode().Role.ToString() == "Identifier")
                            {
                                TreeNode tk = new TreeNode(nn.ToString());
                                nt.Nodes.Add(tk);

                                TreeNode te = new TreeNode(prev.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(nn.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(next.ToString());
                                classname = next.ToString();

                                arsc.Nodes.Add(te);
                                te = new TreeNode(ast.ToString());
                                arsc.Nodes.Add(te);
                                te = new TreeNode(ast.Region.ToString());
                                arsc.Nodes.Add(te);

                                isinterface = true;
                            }
                        }
                    }

                    if (isclass == true)
                    {
                        ars.Add(arsc);
                        arsc.Tag = "class";
                    }
                    if (isinterface == true)
                    {
                        ars.Add(arsc);
                        arsc.Tag = "interface";
                    }

                    foreach (AstNode asn in ast.Children)
                    {
                        if (asn.NodeType.ToString() == "Member" || asn.NodeType.ToString() == "TypeDeclaration")
                        {
                            TreeNode ns2 = new TreeNode(asn.NodeType.ToString() + asn.Role.ToString());
                            nv.Nodes.Add(ns2);

                            foreach (AstNode asc in asn.Children)
                            {
                                TreeNode ns3 = new TreeNode(asc.NodeType.ToString() + asc.Role.ToString() + "----" + asc.Region.ToString());
                                ns2.Nodes.Add(ns3);

                                ns3 = new TreeNode(asc.ToString());
                                ns2.Nodes.Add(ns3);
                            }
                        }
                    }
                }
            }

            string cs = "";

            foreach (TreeNode tns in ars)
            {
                cs = "";
                node.Nodes.Add(tns);
                int i = 0;
                foreach (TreeNode ns in tns.Nodes)
                {
                    //if (tns.Tag == null)
                    //    continue;

                    //if (tns.Tag.GetType() != typeof(AstNode))
                    //    continue;

                    //AstNode t = (AstNode)tns.Tag;
                    //tns.Text = t.ToString();

                    cs += ns.Text + "  ";

                    i++;

                    if (i > 2) break;
                }

                TreeNode inf = new TreeNode(cs);

                ProjectItemInfo pr = new ProjectItemInfo();
                pr.filepath = mapper.filename;

                string info = "";

                if (tns.Tag != null)
                    if (tns.Tag.GetType() == typeof(string))
                        info = tns.Tag as string;

                if (info == "class")

                    inf.ImageKey = "Class_5893_24";
                else if (info == "interface")
                    inf.ImageKey = "Interface_Friend_614_24";

                inf.Tag = pr;
                nods.Nodes.Add(inf);
            }

            //astlogger.logger("astnodes.txt", syntax);
        }

        public void createToolStripButons(ArrayList L)
        {
            ArrayList LL = new ArrayList();
        }

        //public void DefaultHandler()
        //{
        //    ExplorerObject eo = ExplorerObject.GetInstance();

        //    if (eo.cvs == null)
        //        return;

        //    Random r = new Random();
        //    int numbers = r.Next(0, 100);

        //    string s = generatecode(numbers.ToString());

        //    WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

        //    string path = prs.ps.FileName;

        //    string projectdir = Path.GetFullPath(path).Replace(Path.GetFileName(path), "");

        //    File.WriteAllText(projectdir + "newcompile.cs", s);

        //    eo.cvs.add_project_item(path, "newcompile.cs");

        //    eo.cvs.build_solution(this, new EventArgs());

        //    eo.cvs.run_solution(this, new EventArgs());

        //}

        public Control master { get; set; }
        public TreeView ts { get; set; }
        public ListView listview { get; set; }
        public ListBox hstb { get; set; }
        public ScriptControl scr { get; set; }

        public void doinit(Form form, SplitContainer sp, Control master, TreeView t, TreeView ps, ListView listviews, PropertyGrid pgs)
        {

            ToolCommands.TFF = new ArrayList();
            ToolCommands.TFF.Add(_mainToolStrip);

            frm = form;
            _SolutionTreeView = t;
            listview = listviews;
            ts = ps;
            hst = new History();

            eo.cont.addview(_SolutionTreeView);

            
            _ddButton = new ToolStripSplitButton();
            _nextButton = new ToolStripButton();
            _separator = new ToolStripSeparator();
            _newButton = new ToolStripDropDownButton();
            _separators = new ToolStripSeparator();
            _saveButton = new ToolStripButton();
            _saveallButton = new ToolStripButton();
            _separators2 = new ToolStripSeparator();
            _undoButton = new ToolStripDropDownButton();
            _redoButton = new ToolStripDropDownButton();

            _commentButton = new ToolStripButton();
            _uncommentButton = new ToolStripButton();

            _refreshButton = new ToolStripButton();
            _parentButton = new ToolStripButton();
            _addButton = new ToolStripButton();
            _removeButton = new ToolStripButton();
            _buildButton = new ToolStripButton();
            _buildprojectButton = new ToolStripButton();
            _refreshprojectButton = new ToolStripButton();
            _opensolutionButton = new ToolStripButton();
            _currentsolution = new ToolStripButton();
            _configList = new ToolStripComboBoxes(new ComboBoxEx());
            _platList = new ToolStripComboBoxes();
            _projectList = new ToolStripComboBoxes();

            _findinfiles = new ToolStripButton();

            //_mainToolStrip.Items.Add(_ddButton);
            // _mainToolStrip.Items.Add(_nextButton);
            //_mainToolStrip.Items.Add(_separator);
            //_mainToolStrip.Items.Add(_newButton);
            

            //_newButton.DropDownItems.Add("New Project");
            //_newButton.DropDownItems.Add("New Web Site");
            //_mainToolStrip.Items.Add(_refreshButton);

            //_mainToolStrip.Items.Add(_saveButton);
            //_mainToolStrip.Items.Add(_saveallButton);
            //_mainToolStrip.Items.Add(_separators);
            //_mainToolStrip.Items.Add(_undoButton);
            //_mainToolStrip.Items.Add(_redoButton);

           
            //_mainToolStrip.Items.Add(_configList);
            //_mainToolStrip.Items.Add(_platList);
            //_mainToolStrip.Items.Add(_projectList);

            //_mainToolStrip.Items.Add(_opensolutionButton);
            //_mainToolStrip.Items.Add(_separators2);
            //_mainToolStrip.Items.Add(_findinfiles);
            //_adjustButton = new ToolStripDropDownButton();
            // _mainToolStrip.Items.Add(_adjustButton);
            //_mainToolStrip.Items.Add(_commentButton);
            //_mainToolStrip.Items.Add(_uncommentButton);


            _ddButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _ddButton.Image = ve_resource.Backward_256x;
            _ddButton.Name = "Navigate Backward";
            _ddButton.ImageTransparentColor = Color.Magenta;
            _ddButton.Click += new EventHandler(HistoryNavigateButtonClick);
            _ddButton.MouseHover += new EventHandler(MouseHover);

            _nextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _nextButton.Name = "Navigate Forward";
            _nextButton.Image = ve_resource.Forward_256x;
            _nextButton.ImageTransparentColor = Color.Magenta;
            _nextButton.Click += new EventHandler(HistoryNavigateBackwardClick);
            _nextButton.MouseHover += _nextButton_MouseHover;
            _nextButton.MouseLeave += _nextButton_MouseLeave;
            s_toolcolor = _nextButton.BackColor;

            _newButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _newButton.Name = "Add Item";
            _newButton.Image = ve_resource.NewItem_16x;
            _newButton.ImageTransparentColor = Color.Magenta;


            _refreshButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _refreshButton.Image = ve_resource.FolderOpen_16x;
            _refreshButton.ImageTransparentColor = Color.Magenta;
            _refreshButton.Name = "Open File";
            _refreshButton.Click += new System.EventHandler(open_file);

            _saveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _saveButton.Name = "Save";
            _saveButton.Image = ve_resource.Save_256x;
            _saveButton.ImageTransparentColor = Color.Magenta;
            _saveButton.Click += new System.EventHandler(save_active);

            _saveallButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _saveallButton.Name = "Save All";
            _saveallButton.Image = ve_resource.SaveAll_256x;
            _saveallButton.ImageTransparentColor = Color.Magenta;
            _saveallButton.Click += new System.EventHandler(save_all);

            _undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _undoButton.Name = "Undo";
            _undoButton.Image = ve_resource.Undo_16x;
            _undoButton.ImageTransparentColor = Color.Magenta;
            _undoButton.Click += new System.EventHandler(undo);

            _redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _redoButton.Name = "Redo";
            _redoButton.Image = ve_resource.Redo_16x;
            _redoButton.ImageTransparentColor = Color.Magenta;
            _redoButton.Click += new System.EventHandler(redo);

            _opensolutionButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            _opensolutionButton.Image = ve_resource.StartWithoutDebug_16x;
            _opensolutionButton.ImageTransparentColor = Color.Magenta;
            _opensolutionButton.Name = "Start";
            _opensolutionButton.Text = "Start";
            _opensolutionButton.Click += new System.EventHandler(run_solution); // open_solution


            _configList.Name = "Solution Configurations";
            _platList.Name = "Solution Platforms";
            _projectList.Name = "Startup Projects";


            _findinfiles.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _findinfiles.Image = ve_resource.FindinFiles_16x;
            _findinfiles.ImageTransparentColor = Color.Magenta;
            _findinfiles.Name = "Find";

            return;
            
            ToolStripDropDownItem b = _adjustButton.DropDownItems.Add("Add or Remove Items") as ToolStripDropDownItem;

            Module module = Module.GetModule("Standard");

            this.module = module;

            b.Tag = module;

            ST = new Dictionary<string, ToolStripItem>();

            GT = module.temp;

            DG = new ArrayList();

            if (module != null)
            {

                foreach (string c in module.dict.Keys)
                {
                    Command cmd = module.dict[c] as Command;
                    ToolStripMenuItem v = b.DropDownItems.Add(append(cmd.Name)) as ToolStripMenuItem;
                    v.Image = cmd.image;
                    v.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    v.Name = cmd.Name;
                    
                    v.Click += new EventHandler(OptionToolbarHandler);
                    
                    ToolStripItem dd = null;

                    foreach(ToolStripItem p in _mainToolStrip.Items)
                    {
                        if(p.Name.Trim() == cmd.Name)
                        {
                            dd = p;
                            break;
                        }
                    }

                    if (dd != null)
                        
                        {
                            dd.Tag = cmd;
                            v.Tag = dd;
                        }


                    ToolStripMenuItem r0 = CreateToolSripItem(cmd.Name, cmd.Name, cmd.image, cmd) as ToolStripMenuItem;
                    

                    if (cmd.keyboard != null)
                    {
                        Keys k = Keys.None;
                        bool first = true;
                        foreach (Keys g in cmd.keyboard)
                        {
                            k |= g;
                            string p = g.ToString();

                            if (g == Keys.Control)
                                p = "Ctrl";
                            else if (g == (Keys)109)
                                p = "-";
                            if (first == false)
                                p = "+" + p;
                            v.ShortcutKeyDisplayString += p + " ";
                            first = false;
                        }
                        v.ShortcutKeys = k;
                        r0.ShortcutKeys = k;
                        r0.ShortcutKeyDisplayString = v.ShortcutKeyDisplayString;
                    }

                    if (cmd.Name != "Gui" && cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) == false)
                        DG.Add(r0);

                    if (cmd.Name == "Module")
                    {
                        gui.Command_Module bb = cmd as gui.Command_Module;

                        v.Text = append(bb.Names);
                        v.Name = bb.Names;


                        r0.Text = bb.Names;// append(bb.Names);
                        r0.Image = bb.image;
                        r0.Tag = bb;

                        foreach (ToolStripItem p in _mainToolStrip.Items)
                        {
                            if (p.Name == bb.Names)
                            {
                                dd = p;
                                break;
                            }
                        }

                        if (dd != null)

                        {
                            dd.Tag = cmd;
                            v.Tag = dd;
                        }


                        Module modules = Module.GetModule(cmd.Module);

                        if (modules != null)
                        {
                            foreach (string cc in modules.dict.Keys)
                            {
                                Command cmds = modules.dict[cc] as Command;
                                ToolStripMenuItem vv = v.DropDownItems.Add(cmds.Name) as ToolStripMenuItem;
                                vv.Image = cmds.image;
                                vv.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                                if (cmds.keyboard != null)
                                {
                                    Keys k = Keys.None;
                                    bool first = true;
                                    foreach (Keys g in cmds.keyboard)
                                    {
                                        k |= g;
                                        string p = g.ToString();
                                        
                                        if (g == Keys.Control)
                                            p = "Ctrl";
                                        else if (g == (Keys)109)
                                            p = "-";
                                        if (first == false)
                                            p = "+" + p;
                                        vv.ShortcutKeyDisplayString += p + " ";
                                        first = false;
                                    }
                                    vv.ShortcutKeys = k;
                                }

                                vv.Checked = true;

                                CreateToolStripDropDown(r0, cmds);

                            }

                        }
                    }
                    if (cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) || cmd.Name == "Gui")
                    {
                        gui.Command_Gui bb = cmd as gui.Command_Gui;
                        v.Text = bb.Names;

                        ToolStripItem p0 = CreateToolComboBox(bb.Names, bb.Names, cmd.image, cmd);

                        bb.Configure(p0 as ToolStripComboBoxes);

                        foreach (ToolStripItem p in _mainToolStrip.Items)
                        {
                            if (p.Name.Trim() == bb.Names)
                            {
                                dd = p;
                                break;
                            }
                        }

                        if (dd != null)

                        {
                            dd.Tag = cmd;
                            v.Tag = dd;
                        }
                        p0.Text = bb.Names;
                        

                        DG.Add(p0);

                    }
                  
                    else if (cmd.Name == "Launcher")
                    {
                        gui.Command_Launcher bb = cmd as gui.Command_Launcher;
                        v.Text = bb.Names;
                        r0.Text = bb.Names;
                    }

                    if (GT.IndexOf(v.Name) >= 0)
                        v.Checked = true;
                    else
                        v.Checked = false;

                    if (GT.IndexOf(v.Text) >= 0)
                        v.Checked = true;

                    if (v.Text == null)
                        v.Text = v.Name;

                    if (ST.ContainsKey(v.Text) == false)
                    ST.Add(v.Text, v);

                    ToolStripItem bc = v.Tag as ToolStripItem;

                    if (bc == null)
                        continue;

                    bc.ToolTipText = v.Name;

                    bc.AutoToolTip = true;

                    if (v.ShortcutKeyDisplayString != "")
                        bc.ToolTipText += "(" + v.ShortcutKeyDisplayString + ")";

                   
                }

                b.DropDownItems.Add(new ToolStripSeparator());

                _customizeButton = new ToolStripButton("Customize");
                _customizeButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                _customizeButton.Click += new EventHandler(customize);
                b.DropDownItems.Add(_customizeButton);
                

                _resetButton = new ToolStripButton("Reset toolbox");
                _resetButton.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                b.DropDownItems.Add(_resetButton);
                
            }

            ns = new ToolStrip();
            ns.Tag = "Standards";
            FilterItems(ns, GT, DG);
            d0 = GetCustomize(DG);
            ChekedStates(GT, d0);
            ns.Items.Add(d0);
            ns.Renderer = new WinExplorers.Debuggers.Renderer(new ProfessionalColorTable());
           

            _commentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _commentButton.Image = ve_resource.CommentCode_16x;
            _commentButton.ImageTransparentColor = Color.Magenta;
            _commentButton.Click += new System.EventHandler(comment);

            _uncommentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            _uncommentButton.Image = ve_resource.UncommentCode_16x;
            _uncommentButton.ImageTransparentColor = Color.Magenta;
            _uncommentButton.Click += new System.EventHandler(uncomment);


            LoadPlatforms();

            _SolutionTreeView.ResumeLayout(false);
            _SolutionTreeView.PerformLayout();

            string s = AppDomain.CurrentDomain.BaseDirectory;

            linkfile = s + "\\" + "a.txt";

            WinExplorers.Debuggers.ProfessionalColorTableExtended pc = new WinExplorers.Debuggers.ProfessionalColorTableExtended();

            _mainToolStrip.Renderer = new WinExplorers.Debuggers.Renderer(pc);

        }

        public void LoadToolbar()
        {



        }


        Module module { get; set; }

        public ToolStrip LoadToolbar(ToolStrip np, string module_name)
        {
            
            
            ToolStripDropDownItem    b = new ToolStripDropDownButton("");
            

            Module module = Module.GetModule(module_name);

            this.module = module;

            b.Tag = module;

            ST = new Dictionary<string, ToolStripItem>();

            GT = module.temp;

            DG = new ArrayList();

            if (module != null)
            {

                foreach (string c in module.dict.Keys)
                {
                    Command cmd = module.dict[c] as Command;

                    //ToolStripMenuItem v = b.DropDownItems.Add(append(cmd.Name)) as ToolStripMenuItem;
                    //v.Image = cmd.image;
                    //v.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    //v.Name = cmd.Name;
                    //v.Click += new EventHandler(OptionToolbarHandler);

                    //ToolStripItem dd = null;

                 


                    ToolStripMenuItem r0 = CreateToolSripItem(cmd.Name, cmd.Name, cmd.image, cmd) as ToolStripMenuItem;


                    if (cmd.keyboard != null)
                    {
                        Keys k = Keys.None;
                        bool first = true;
                        foreach (Keys g in cmd.keyboard)
                        {
                            k |= g;
                            string p = g.ToString();

                            if (g == Keys.Control)
                                p = "Ctrl";
                            else if (g == (Keys)109)
                                p = "-";
                            if (first == false)
                                p = "+" + p;
                            r0.ShortcutKeyDisplayString += p + " ";
                            first = false;
                        }
                        //v.ShortcutKeys = k;
                        r0.ShortcutKeys = k;
                        //r0.ShortcutKeyDisplayString = v.ShortcutKeyDisplayString;
                    }

                    if (cmd.Name != "Gui" && cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) == false)
                        DG.Add(r0);

                    if (cmd.Name == "Module")
                    {
                        gui.Command_Module bb = cmd as gui.Command_Module;

                        //v.Text = append(bb.Names);
                        //v.Name = bb.Names;

                        r0.Text = append(bb.Names);
                        r0.Name = bb.Names;
                        r0.Tag = bb;

                        //foreach (ToolStripItem p in _mainToolStrip.Items)
                        //{
                        //    if (p.Name == bb.Names)
                        //    {
                        //        dd = p;
                        //        break;
                        //    }
                        //}

                        //if (dd != null)

                        //{
                        //    dd.Tag = cmd;
                        //    v.Tag = dd;
                        //}


                        Module modules = Module.GetModule(cmd.Module);

                        if (modules != null)
                        {
                            foreach (string cc in modules.dict.Keys)
                            {
                                Command cmds = modules.dict[cc] as Command;
                                //ToolStripMenuItem vv = v.DropDownItems.Add(cmds.Name) as ToolStripMenuItem;
                                //vv.Image = cmds.image;
                                //vv.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                                if (cmds.keyboard != null)
                                {
                                    Keys k = Keys.None;
                                    bool first = true;
                                    foreach (Keys g in cmds.keyboard)
                                    {
                                        k |= g;
                                        string p = g.ToString();

                                        if (g == Keys.Control)
                                            p = "Ctrl";
                                        else if (g == (Keys)109)
                                            p = "-";
                                        if (first == false)
                                            p = "+" + p;
                                        r0.ShortcutKeyDisplayString += p + " ";
                                        first = false;
                                    }
                                    r0.ShortcutKeys = k;
                                }

                                r0.Checked = true;

                                ToolStripItem b0 = CreateToolStripDropDown(r0, cmds);

                                //r0.DropDownItems.Add(b0);

                            }

                        }
                    }
                    if (cmd.GetType().IsSubclassOf(typeof(gui.Command_Gui)) || cmd.Name == "Gui")
                    {
                        gui.Command_Gui bb = cmd as gui.Command_Gui;
                        //v.Text = bb.Names;

                        ToolStripItem p0 = CreateToolComboBox(bb.Names, bb.Names, cmd.image, cmd);

                        bb.Configure(p0 as ToolStripComboBoxes);
              
                        p0.Text = bb.Names;


                        DG.Add(p0);

                    }
                    else if (cmd.Name == "Launcher")
                    {
                        gui.Command_Launcher bb = cmd as gui.Command_Launcher;
                        r0.Text = bb.Names;
                        r0.Tag = bb;
                    }

                    if (GT.IndexOf(r0.Name) >= 0)
                        r0.Checked = true;
                    else
                        r0.Checked = false;

                    if (GT.IndexOf(r0.Text) >= 0)
                        r0.Checked = true;

                    //ST.Add(v.Text, v);

                    //ToolStripItem bc = v.Tag as ToolStripItem;

                    //if (bc == null)
                    //    continue;

                    //bc.ToolTipText = v.Name;

                    //bc.AutoToolTip = true;

                    //if (v.ShortcutKeyDisplayString != "")
                    //    bc.ToolTipText += "(" + v.ShortcutKeyDisplayString + ")";


                }

                Point pc = np.Location;
                ToolStrip nr = new ToolStrip();
                np.Tag = np.Tag;
                ExplorerForms.ef.Command_ChangeToolstrip(np, AnchorStyles.Top, true);
                FilterItems(nr, GT, DG);
                d0 = GetCustomize(DG);
                ChekedStates(GT, d0);
                nr.Items.Add(d0);
                nr.Renderer = new WinExplorers.Debuggers.Renderer(new ProfessionalColorTable());
                ExplorerForms.ef.Command_ChangeToolstrip(nr, AnchorStyles.Top, false, pc.X, pc.Y);

                

            }
            return ns;
        }

            ArrayList DG { get; set; }

        ToolStripDropDownButton d0 { get; set; }

        public ToolStripMenuItem CreateToolStripDropDown(ToolStripMenuItem v, Command cmds)
        {
            ToolStripMenuItem vv = v.DropDownItems.Add(cmds.Name) as ToolStripMenuItem;
            vv.Image = cmds.image;
            vv.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            if (cmds.keyboard != null)
            {
                Keys k = Keys.None;
                bool first = true;
                foreach (Keys g in cmds.keyboard)
                {
                    k |= g;
                    string p = g.ToString();

                    if (g == Keys.Control)
                        p = "Ctrl";
                    else if (g == (Keys)109)
                        p = "-";
                    if (first == false)
                        p = "+" + p;
                    vv.ShortcutKeyDisplayString += p + " ";
                    first = false;
                }
                vv.ShortcutKeys = k;
            }
            
            vv.Checked = true;

            return vv;
        }


        public ToolStripItem CreateToolSripItem(string text, string name, Image image, Command cmd)
        {
            ToolStripMenuItem r0 = new ToolStripMenuItem();
            r0.Text = text;
            r0.Name = name;
            r0.Image = image;
            r0.Tag = cmd;
            r0.Click += R0_Click;
            return r0;

        }

        private void R0_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem b = sender as ToolStripMenuItem;
            if (b == null)
                return;
            Command cmd = b.Tag as Command;

            if (cmd == null)
                return;

            //MessageBox.Show("Command will be done " + cmd.Name);

            cmd.Execute();
        }

        public ToolStripItem CreateToolComboBox(string text, string name, Image image, Command cmd)
        {
            ToolStripItem r0 = new ToolStripComboBoxes() as ToolStripItem;
            r0.Text = text;
            r0.Name = name;
            r0.Image = image;
            r0.Tag = cmd;
            //r0.Click += R0_Clicked;
            return r0;

        }

        private void R0_Clicked(object sender, EventArgs e)
        {
            ToolStripComboBoxes b = sender as ToolStripComboBoxes;
            if (b == null)
                return;
            gui.Command_Gui cmd = b.Tag as gui.Command_Gui;

            if (cmd == null)
                return;

           // MessageBox.Show("Command will be done " + cmd.Names);
        }

        public void FilterItems(ToolStrip nr, ArrayList G, ArrayList DG)
        {
            nr.ResumeLayout();
            nr.Items.Clear();
            foreach(string name in G)
            {

                if (name == "Separator")
                    nr.Items.Add(new ToolStripSeparator());
                else                    
                foreach(ToolStripItem b in DG)
                {
                        if (b.Text != null)
                        if(b.Text.Trim() == name )
                        {
                            nr.Items.Add(b);
                          
                        }
                    
                }


            }



        }
        public void ChekedStates(ArrayList G, ToolStripDropDownButton d)
        {

            ToolStripDropDownItem b = GetSuggestions(d);


            foreach (ToolStripItem c in b.DropDownItems)
            {
                ToolStripMenuItem s = c as ToolStripMenuItem;
                if(s != null)
                s.Checked = false;
             
            }

            foreach (ToolStripItem p in b.DropDownItems)
            {

                ToolStripMenuItem c = p as ToolStripMenuItem;
                if (c == null)
                    continue;


                foreach (string s in G)
                {
                    if (s == "Separator")
                        continue;
                    if (s.Trim() == c.Text.Trim())
                    {
                        c.Checked = true;
                          

                    }
                }
            }

        }
        public ToolStripDropDownButton GetCustomize(ArrayList DG)
        {

            ToolStripDropDownButton dd = new ToolStripDropDownButton();

            ToolStripDropDownItem b = dd.DropDownItems.Add("Add or Remove Items") as ToolStripDropDownItem;

            foreach (ToolStripItem c in DG)
            {
                if (c.GetType() == typeof(ToolStripSeparator))
                    continue;

                ToolStripMenuItem d = new ToolStripMenuItem();
                d.Text = append(c.Text);
                d.Name = c.Name;
                d.Image = c.Image;
                d.Tag = c.Tag;
                d.Click += OptionToolbarHandlers;
                ToolStripMenuItem bb = c as ToolStripMenuItem;
                if (bb != null) {
                    d.ShortcutKeys = bb.ShortcutKeys;
                    d.ShortcutKeyDisplayString = bb.ShortcutKeyDisplayString;
                    if(bb.DropDownItems != null)
                        if(bb.DropDownItems.Count > 0)
                        {
                            foreach(ToolStripMenuItem p in bb.DropDownItems)
                            {
                                ToolStripMenuItem dc = new ToolStripMenuItem();
                                dc.Text = append(p.Text);
                                dc.Name = p.Name;
                                dc.Image = p.Image;
                                dc.Tag = p.Tag;
                                d.DropDownItems.Add(dc);
                            }
                        }
                }
                
                b.DropDownItems.Add(d);
            }

            b.DropDownItems.Add(new ToolStripSeparator());

            ToolStripButton cc = new ToolStripButton("Customize");
            //cc.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            cc.Click += new EventHandler(customize);
            b.DropDownItems.Add(cc);


            cc = new ToolStripButton("Reset toolbox");
            //cc.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.DropDownItems.Add(cc);
            



            return dd;
        }

        public ToolStripDropDownItem GetSuggestions(ToolStripDropDownButton b)
        {
            if (b.DropDownItems.Count > 0)
                return b.DropDownItems[0] as ToolStripDropDownItem;
            return null;

        }

        public ToolStrip ns { get; set; }

        private void customize(object sender, EventArgs e)
        {
            
            ExplorerForms ef = ExplorerForms.ef;
            if (ef == null)
                return;
            

            string name = "Commands-Toolbar-" + module.Name;

            ef.Command_CustomizeDialog(name);
        }

        private void MouseHover(object sender, EventArgs e)
        {
            ToolStripItem b = sender as ToolStripItem;

            if (b == null)
                return;

            
        }

        public string append(string name)
        {
            return "      " + name;
        }

        public void OptionToolbarHandlers(object sender, EventArgs e)
        {
            ToolStripMenuItem b = sender as ToolStripMenuItem;
            if (b == null)
                return;
            string name = b.Text.Trim();

            int d = GetIndex(GT, DG, name);

            if (b.Checked == true)
            {

                
                GT.Remove(name);
                b.Checked = false;
            }
            else
            {

                if (d < 0)
                    d = 0;
                GT.Insert(d, name);
                b.Checked = true;
            }

            FilterItems(ns, GT, DG);

            ns.Items.Add(d0);

        }

        public void OptionToolbarHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem b = sender as ToolStripMenuItem;
            if (b == null)
                return;
            string name = b.Text.Trim();

            int d = GetIndex(name);

            if (d == -1)
                return;

            ToolStripItem r = b.Tag as ToolStripItem;

            if (r == null)
                return;

            if (b.Checked == true)
            {

                _mainToolStrip.Items.Remove(r);
                GT.Remove(name);
                b.Checked = false;
            }
            else
            {
                
                _mainToolStrip.Items.Insert(d, r);
                GT.Insert(d, name);
                b.Checked = true;
            }
        }

        Dictionary<string, ToolStripItem> ST { get; set; }        

        ArrayList GT { get; set; }

        int GetIndex(string name)
        {

            name = name.Trim();

            int i = GT.IndexOf(name);

            if (i >= 0)
                return i;
            

            foreach(string b in ST.Keys)
            {
                ToolStripMenuItem d = ST[b] as ToolStripMenuItem;

               
                int p = GT.IndexOf(d.Name);
                if ( p >= 0)
                    i = p;
                if (d.Name == name)
                {
                    return i;

                }

            }
            return 0;
        }
        int GetIndex(ArrayList GT, ArrayList DG, string name)
        {

            name = name.Trim();

            int i = GT.IndexOf(name);

            if (i >= 0)
                return i;


            foreach (ToolStripItem b in DG)
            {
                ToolStripItem d = b;


                int p = GT.IndexOf(d.Name);
                if (p >= 0)
                {
                    if (d.Text.Trim() == name)
                    {
                        return p;

                    }
                    else
                        i = p;
                }
                if (d.Text != null)
                if(d.Text.Trim() == name)
                {
                    return i + 1;

                }

            }
            return i;
        }
        public void open_file(object sender, EventArgs e)
        {
            OpenFile();
        }

        public  void OpenFile()
        {

        }
        private void _platList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show("platform changed...");
        }

        private void _configList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show("config platform changed...");
        }

        private void _nextButton_MouseLeave(object sender, EventArgs e)
        {
            _nextButton.BackColor = s_toolcolor;
        }

        private static Color s_toolcolor = Color.FromKnownColor(KnownColor.Control);

        private static Color s_specialyellow = Color.FromArgb(0xFF, 255, 242, 157);

        private void _nextButton_MouseHover(object sender, EventArgs e)
        {
            _nextButton.BackColor = s_specialyellow;
        }

        public void HistoryNavigateClick(object sender, EventArgs e)
        {
            ToolStripMenuItem b = sender as ToolStripMenuItem;

            if (b.Tag == null)
                return;

            history H = b.Tag as history;

            foreach (ToolStripMenuItem c in _ddButton.DropDownItems)
            {
                c.Checked = false;
            }

            b.Checked = true;

            if (H == null)
                return;

            scr.OpenHistory(H);
        }

        public void HistoryNavigateBackwardClick(object sender, EventArgs e)
        {
            ToolStripSplitButton d = _ddButton;

            if (d == null)
                return;

            int i = GetCheckIndex(d);

            i--;

            if (i < 0)
                i = 0;

            if (i >= d.DropDownItems.Count)
                i = d.DropDownItems.Count - 1;

            if (i < 0)
                return;

            ToolStripMenuItem b = d.DropDownItems[i] as ToolStripMenuItem;

            foreach (ToolStripMenuItem c in d.DropDownItems)
            {
                c.Checked = false;
            }

            b.Checked = true;

            if (b.Tag == null)
                return;

            history H = b.Tag as history;

            if (H == null)
                return;

            scr.OpenHistory(H);
        }
        public void NavigateBackward()
        {
            ToolStripSplitButton d = _ddButton;

            if (d == null)
                return;

            int i = GetCheckIndex(d);

            i--;

            if (i < 0)
                i = 0;

            if (i >= d.DropDownItems.Count)
                i = d.DropDownItems.Count - 1;

            if (i < 0)
                return;

            ToolStripMenuItem b = d.DropDownItems[i] as ToolStripMenuItem;

            foreach (ToolStripMenuItem c in d.DropDownItems)
            {
                c.Checked = false;
            }

            b.Checked = true;

            if (b.Tag == null)
                return;

            history H = b.Tag as history;

            if (H == null)
                return;

            scr.OpenHistory(H);
        }
        public int GetCheckIndex(ToolStripSplitButton d)
        {
            int i = 0;
            while (i < d.DropDownItems.Count)

            {
                ToolStripMenuItem c = d.DropDownItems[i] as ToolStripMenuItem;

                if (c.Checked == true)
                    return i;

                i++;
            }

            return -1;
        }

        public void HistoryNavigateButtonClick(object sender, EventArgs e)
        {
            ToolStripSplitButton d = sender as ToolStripSplitButton;

            if (d == null)
                return;

            Command cmd = d.Tag as Command;

            if(cmd != null)
            {

                MessageBox.Show("Command started");

                cmd.Execute();

                return;
            }

            //if (d.IsOnDropDown == false)
            //    return;

            if (d.DropDownItems.Count <= 1)
                return;

            int i = GetCheckIndex(d);

            if (i < 0)
                i = 0;

            i++;

            if (i >= d.DropDownItems.Count)
                i = d.DropDownItems.Count - 1;

            ToolStripMenuItem b = d.DropDownItems[i] as ToolStripMenuItem;

            foreach (ToolStripMenuItem c in d.DropDownItems)
            {
                c.Checked = false;
            }

            b.Checked = true;

            if (b.Tag == null)
                return;

            history H = b.Tag as history;

            if (H == null)
                return;

            scr.OpenHistory(H);
        }
        public void NavigateForward()
        {
            ToolStripSplitButton d = _ddButton;

            if (d == null)
                return;

            //if (d.IsOnDropDown == false)
            //    return;

            if (d.DropDownItems.Count <= 1)
                return;

            int i = GetCheckIndex(d);

            if (i < 0)
                i = 0;

            i++;

            if (i >= d.DropDownItems.Count)
                i = d.DropDownItems.Count - 1;

            ToolStripMenuItem b = d.DropDownItems[i] as ToolStripMenuItem;

            foreach (ToolStripMenuItem c in d.DropDownItems)
            {
                c.Checked = false;
            }

            b.Checked = true;

            if (b.Tag == null)
                return;

            history H = b.Tag as history;

            if (H == null)
                return;

            scr.OpenHistory(H);
        }
        public void HistoryChanged(ArrayList hst)
        {
            _mainToolStrip.SuspendLayout();

            _ddButton.DropDownItems.Clear();

            foreach (history H in hst)
            {
                int d = H.text.Length;

                if (d > 20)
                    d = 20;

                string text = H.text.Substring(0, d);

                ToolStripMenuItem fs = new ToolStripMenuItem();
                string file = Path.GetFileName(H.file);
                fs.Text = file + " : " + " " + text + " " + H.line.ToString();
                //fs.ForeColor = System.Drawing.Color.FromKnownColor(c);
                //fs.BackColor = System.Drawing.Color.FromKnownColor(c);
                fs.Tag = H;
                fs.Click += HistoryNavigateClick;

                _ddButton.DropDownItems.Add(fs);
            }

            _mainToolStrip.ResumeLayout();
        }

        public void LoadPlatforms()
        {
            _configList.Items.Add("Debug");
            _configList.Items.Add("Release");

            _configList.Items.Add("ConfigurationManager");
            _configList.DropDownStyle = ComboBoxStyle.DropDownList;
            _configList.SelectedIndex = 0;
            _configList.BackColor = Color.FromArgb(252, 252, 252);

            _platList.Items.Add("AnyCPU");
            _platList.Items.Add("x86");
            _platList.Items.Add("x64");
            _platList.Items.Add("ConfigurationManager");
            _platList.DropDownStyle = ComboBoxStyle.DropDownList;
            _platList.SelectedIndex = 0;
            _platList.BackColor = Color.FromArgb(245, 245, 245);
        }

        public string GetPlatform()
        {

            int i = _platList.SelectedIndex;
            if (i < 0)
                return "";

            string p = _platList.Items[i].ToString();

            return p;


        }

        public string GetConfiguration()
        {

            int i = _configList.SelectedIndex;
            if (i < 0)
                return "";

            string p = _configList.Items[i].ToString();

            return p;


        }

        public string SetPlatform(string plf)
        {
            
            int i = 0;
            while(i < _platList.Items.Count)
            {

                string c = _platList.Items[i].ToString();

                if(c.ToLower() == plf.ToLower())
                {

                    _platList.BeginUpdate();
                    _platList.SelectedIndex = i;
                    _platList.SelectedItem = _platList.Items[i];
                    _platList.Items[i] = _platList.Items[i].ToString();
                   
                    _platList.EndUpdate();
                    

                    return c;

                }

                i++;
            }

            return "";


        }

        public string SetConfiguration(string cfg)
        {

            int i = 0;
            while (i < _configList.Items.Count)
            {

                string c = _configList.Items[i].ToString();

                if (c.ToLower() == cfg.ToLower())
                {
                    _configList.SelectedIndex = i;

                    return c;

                }

                i++;
            }

            return "";


           

        }

        public void LoadPlatforms(VSSolution vs)
        {
            ArrayList L = ConfigurationManagerForm.GetProjectPlatform(vs);

            //_mainToolStrip.SuspendLayout();

            _configList.BeginUpdate();

            _configList.Items.Clear();

            //_configList.DropDownStyle = ComboBoxStyle.DropDownList;
            //_configList.SelectedIndex = 0;
            //_configList.BackColor = Color.FromArgb(252, 252, 252);
            //_configList.Items.Clear();

            foreach (string s in L)
            {
                _configList.Items.Add(s);
            }

            _configList.Items.Add("ConfigurationManager");

            _configList.EndUpdate();

            _platList.BeginUpdate();

            _platList.Items.Clear();

            //_platList.DropDownStyle = ComboBoxStyle.DropDownList;
            //_platList.SelectedIndex = 0;
            //_platList.BackColor = Color.FromArgb(245, 245, 245);

            L = ConfigurationManagerForm.GetSolutionPlatform(vs);

            foreach (string s in L)
            {
                _platList.Items.Add(s);
            }

            _platList.Items.Add("ConfigurationManager");

            _platList.EndUpdate();

            _configList.SelectedIndex = 0;
            _platList.SelectedIndex = 0;

            //_mainToolStrip.ResumeLayout();

            if (ExplorerForms.ef == null)
                return;
            ExplorerForms.ef.Command_SetPlatforms();

            ExplorerForms.ef.Command_SetConfigurations();

            ExplorerForms.ef.Command_StartupProjects();

        }
        public void LoadConfigurations(VSSolution vs)
        {
            ArrayList L = ConfigurationManagerForm.GetProjectPlatform(vs);

            //_mainToolStrip.SuspendLayout();

            _configList.BeginUpdate();

            _configList.Items.Clear();

            //_configList.DropDownStyle = ComboBoxStyle.DropDownList;
            //_configList.SelectedIndex = 0;
            //_configList.BackColor = Color.FromArgb(252, 252, 252);
            //_configList.Items.Clear();

            foreach (string s in L)
            {
                _configList.Items.Add(s);
            }

            _configList.Items.Add("ConfigurationManager");

            _configList.EndUpdate();

            _platList.BeginUpdate();

            _platList.Items.Clear();

            //_platList.DropDownStyle = ComboBoxStyle.DropDownList;
            //_platList.SelectedIndex = 0;
            //_platList.BackColor = Color.FromArgb(245, 245, 245);

            L = ConfigurationManagerForm.GetSolutionPlatform(vs);

            foreach (string s in L)
            {
                _platList.Items.Add(s);
            }

            _platList.Items.Add("ConfigurationManager");

            _platList.EndUpdate();

            _configList.SelectedIndex = 0;
            _platList.SelectedIndex = 0;

            //_mainToolStrip.ResumeLayout();
        }
        public void LoadProjects(VSSolution vs)
        {
            _projectList.Items.Clear();

            foreach (VSProject p in vs.Projects)
            {
                if (p.IsExecutable() == true)
                    _projectList.Items.Add(p.Name);
            }

            if (_projectList.Items.Count > 0)
                _projectList.SelectedIndex = 0;


            _projectList.DropDownStyle = ComboBoxStyle.DropDownList;
            _projectList.SetDrawMode(false);
        }
        public ArrayList LoadStartupProjects(VSSolution vs)
        {
            ArrayList R = new ArrayList();

            foreach (VSProject p in vs.Projects)
            {
                if (p.IsExecutable() == true)
                    R.Add(p.Name);
            }

            return R;
        }
        public void undo(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.Undo();
        }
        public void undo()
        {
            if (scr == null)
                return;

            scr.Undo();
        }
        public void redo(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.Redo();
        }
        public void redo()
        {
            if (scr == null)
                return;

            scr.Redo();
        }

        public void comment(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.Comment();
        }

        public void uncomment(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.UnComment();
        }

        public void save_active(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.Save();

            //_saveallButton.Enabled = false;
        }
        public void save_active()
        {
            if (scr == null)
                return;

            scr.Save();

            //_saveallButton.Enabled = false;
        }
        public void save_all(object sender, EventArgs e)
        {
            if (scr == null)
                return;

            scr.SaveAll();

            _saveallButton.Enabled = false;
        }
        public void save_all()
        {
            if (scr == null)
                return;

            scr.SaveAll();

            _saveallButton.Enabled = false;
        }
        public void LoadRecent()
        {
            if (hstb == null)
                return;

            hst.ReloadRecentSolutions();
        }

        public IEnumerable<Type> FindSubClassesOf<TBaseType>()
        {
            var baseType = typeof(TBaseType);
            var assembly = baseType.Assembly;

            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public string generatecode(string s)
        {
            // define the class template: spots are marked for replacement (ie %NAME%)
            string baseCode = @"using System;
                                using System.Collections.Generic;
                                using System.IO;
                                using System.Linq;
                                using System.Text;
                                using System.Reflection;
                                using System.Windows.Forms;

                                namespace WinExplorer
                                {
                                    public class %NAME%ToolStripButtonCreator : ToolStripButtonCreator
                                    {
                                            public %NAME%ToolStripButtonCreator() : base()
                                            {
                                            }

                                            public override void DefaultHandler()
                                            {
               if(_appManager == null)
                  return;
                   DockPanel panel = _appManager.MainForm.panel;
                panel.SaveAsXml(""testc.xml"");
              MessageBox.Show(""Dynamic .NET 4.5.1"");
                                            }
                                    }
                                }";

            baseCode = baseCode.Replace("%NAME%", s);

            return baseCode;
        }

        public ProjectItemInfo getactiveproject()
        {
            //ApplicationManager app = ApplicationManager.GetInstance();

            TreeView tv = _SolutionTreeView;

            if (tv.SelectedNode == null)
                return null;

            TreeNode nodes = tv.SelectedNode;

            if (nodes.Tag == null)
                return null;

            //VSProject project = msbuilder_alls.getvsproject(slnpath, 10);

            if (nodes.Tag.GetType() != typeof(ProjectItemInfo))
                return null;

            ProjectItemInfo project = nodes.Tag as ProjectItemInfo;

            return project;
        }

        public TreeNode getrootnode()
        {
            // ApplicationManager app = ApplicationManager.GetInstance();

            TreeView tv = _SolutionTreeView;

            //if (tv.SelectedNode == null)
            //    return null;

            //TreeNode nodes = tv.SelectedNode;

            //if (nodes.Tag == null)
            //    return null;

            //VSProject project = msbuilder_alls.getvsproject(slnpath, 10);

            if (_SolutionTreeView.Nodes.Count <= 0)
                return null;

            return _SolutionTreeView.Nodes[0];
        }

        public TreeNode GetTreeNode(TreeNode nodes, string filename)
        {
            foreach (TreeNode node in nodes.Nodes)
            {
                if (node.Tag != null)
                    if (node.Tag.GetType() == typeof(ProjectItemInfo))
                    {
                        ProjectItemInfo p = node.Tag as ProjectItemInfo;
                        if (p.ps != null)
                            if (p.ps.FileName == filename)
                                return node;
                    }

                TreeNode n = GetTreeNode(node, filename);
            }

            return null;
        }

        public TreeNode getprojectenode(string filename)
        {
            TreeView tv = _SolutionTreeView;

            foreach (TreeNode node in tv.Nodes)
            {
                TreeNode nodes = GetTreeNode(node, filename);

                if (nodes != null)
                    return nodes;
            }

            return null;
        }

        public TreeNode getactivenode()
        {
            //ApplicationManager app = ApplicationManager.GetInstance();

            TreeView tv = _SolutionTreeView;

            if (tv.SelectedNode == null)
                return null;

            TreeNode nodes = tv.SelectedNode;

            if (nodes.Tag == null)
                return null;

            //VSProject project = msbuilder_alls.getvsproject(slnpath, 10);

            return nodes;
        }

        public ArrayList GetProjectList()
        {
            ArrayList L = new ArrayList();

            getactivenode();

            return L;
        }

        public IEnumerable<EntityDeclaration> getentity(SyntaxTree syntax, string ents)
        {
            string classname = ents;

            IEnumerable<EntityDeclaration> doc = syntax.GetTypes(true);

            foreach (EntityDeclaration ent in doc)
            {
                if (ent.Name == classname)
                {
                    //MessageBox.Show("Class - " + ent.Name);

                    return doc;
                }
            }

            return doc;
        }


        public string getnodebyrole(AstNode ast, string role, TreeNode nv)
        {
            foreach (AstNode ent in ast.Children)
            {
            }

            foreach (AstNode nn in ast.Descendants)
            {
                foreach (AstNode ent in nn.Descendants)
                {
                }

                if (nn.NodeType.ToString() == "Token")
                {
                    AstNode next = nn.GetNextNode();

                    AstNode prev = nn.GetPrevNode();

                    TreeNode bb = new TreeNode(nn.Role.ToString());
                    nv.Nodes.Add(bb);
                }
            }

            return "";
        }

        //private BackgroundWorker bw = null;

        //public void startprogress(string file)
        //{
        //    reso = file;

        //    BackgroundWorker bw = new BackgroundWorker();
        //    bw.WorkerReportsProgress = true;
        //    bw.WorkerSupportsCancellation = true;
        //    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
        //    //bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);

        //    bw.RunWorkerAsync();
        //    //bw.CancelAsync();
        //}

    

        public string reso { get; set; }

        public TreeView load_recent_solution(string recent)
        {
            TreeView tv = null;

            if (File.Exists(recent) == false)
            {
                MessageBox.Show("No sln file found...");
                return null;
            }

            _SolutionTreeView.Invoke(new Action(() => { TreeNode node = new TreeNode(Path.GetFileName(recent) + " loading..."); node.ImageKey = "Solution_8308_24"; node.SelectedImageKey = "Solution_8308_24"; _SolutionTreeView.Nodes.Add(node); }));

            load_tree_view(recent);

            _SolutionTreeView.Invoke(new Action(() => { 

            tv = (TreeView)LoadProject(recent);

            
            VSSolution vs = tv.Tag as VSSolution;

            LoadPlatforms(vs);

            // Command.counter--;

            //_SolutionTreeView.Invoke(new Action(() =>
            
            //{
                _ddButton.DropDownItems.Clear(); if (_SolutionTreeView.Nodes.Count > 0) _SolutionTreeView.Nodes[0].Expand(); // LoadPlatforms(vs);
                if (vs != null) LoadProjects(vs);

                _SolutionTreeView.Tag = vs;

               // ExplorerForms.ef.Command_StartupProjects();

                ExplorerForms.ef.Command_SetConfigurations();
                ExplorerForms.ef.Command_SetPlatforms();
                ExplorerForms.ef.Command_StartupProjects();

            }));


            //_SolutionTreeView.Invoke(new Action(() => {
            //_ddButton.DropDownItems.Clear();
            //if (_SolutionTreeView.Nodes.Count > 0) _SolutionTreeView.Nodes[0].Expand();
            //LoadPlatforms(vs);
            //if (vs != null) LoadProjects(vs);
            //Command.running = false;
            //}));

            //_SolutionTreeView.Tag = vs;

            //Command.counter--;

            return tv;
        }

        public TreeView update_project(VSSolution vs, VSProject pp)
        {
            string recent = vs.solutionFileName;

            if (File.Exists(recent) == false)
            {
                MessageBox.Show("No sln file found... Exiting.");
                return null;
            }



            if (pp == null)
                return null;

            TreeView tv = (TreeView)LoadProjects(recent, pp.FileName);



            return tv;
        }

        public TreeNode GetProjectNodes(VSSolution vs, VSProject pp)
        {
            TreeNode node = null;

            string name = pp.Name;

            TreeNode ns = _SolutionTreeView.Nodes[0];

            foreach (TreeNode nodes in ns.Nodes)
            {
                if (nodes.Text == name)
                {
                    SetVS(nodes, vs, pp);
                    return nodes;
                }
            }

            return node;
        }

        public void SetVS(TreeNode node, VSSolution vs, VSProject pp)
        {
            CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (p != null)
            {
                p.vs = vs;
                //p.ps = pp;
            }

            foreach (TreeNode ns in node.Nodes)
                SetVS(ns, vs, pp);
        }

        public TreeNode FindItem(TreeNode node, VSProjectItem pp)
        {
            CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;

            if (p != null)
                if (p.psi != null)
                    if (p.psi.Include == pp.Include)
                        return node;

            foreach (TreeNode ns in node.Nodes)
            {
                TreeNode ng = FindItem(ns, pp);

                if (ng != null)
                    return ng;
            }

            return null;
        }

        public void Loads(object data)
        {
            ArrayList dd = data as ArrayList;

            TreeView _SolutionTreeView = dd[0] as TreeView;
            TreeView tv = dd[1] as TreeView;

            _SolutionTreeView.ImageList = tv.ImageList;

            int N = tv.Nodes.Count;

            ArrayList L = new ArrayList();

            int i = 0;
            while (i < N)
            {
                L.Add(tv.Nodes[i]);

                i++;
            }

            i = 0;
            while (i < N)
            {
                TreeNode ns = L[i] as TreeNode;
                ns.NodeFont = new Font(tv.Font, FontStyle.Bold);
                tv.Nodes.Remove(ns);
                _SolutionTreeView.Nodes.Add(ns);

                i++;
            }
        }

        public void load_current_solution(object sender, EventArgs e)
        {
            //          slnpath = Path.GetFullPath(eo.currentproject);

            slnpath = load_recent_solution();

            load_tree_view(slnpath);

            //startprogress(slnpath);

            //LoadProject(slnpath);
        }

        public void save_recent_solution(string file)
        {
            string d = AppDomain.CurrentDomain.BaseDirectory;

            string fg = d + "\\" + "currentproject.txt";

            if (File.Exists(fg) == false)
                File.Create(fg);

            File.WriteAllText(d + "\\" + "currentproject.txt", file);

            if (hst == null)
                return;
            hst.additem(file);
        }

        public string load_recent_solution()
        {
            string d = AppDomain.CurrentDomain.BaseDirectory;

            string fg = d + "\\" + "currentproject.txt";

            if (File.Exists(fg) == false)
                return fg;

            string file = File.ReadAllText(d + "\\" + "currentproject.txt");

            return file;
        }

        public void load_tree_view(string s)
        {
            ;

            string es = "";

            try
            {
                s = Path.GetDirectoryName(s);

                es = Path.GetFullPath(s);

                selectdir(es, _maintv);
            }
            catch (Exception e)
            {
            }
        }

        public ProgressBar pg { get; set; }

        static public ImageList CreateImageList(string s)
        {
            Dictionary<string, object> dict;

            Dictionary<string, object> dict2;

            Dictionary<string, object> dict3;

            ImageList imageList1 = new ImageList();

            dict = null;
            if (dict == null)
            {
                dict = Load(s + "\\" + "WinExplorer.exe", "WinExplorer.ve-resource.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imageList1);
                }
            }

            dict2 = null;
 
            dict3 = null;
            if (dict3 == null)
            {
 
                addImage(ve_resource.FolderOpen_16x, "Folder", imageList1);
                addImage(ve_resource.FolderOpen_16x, "FolderOpen", imageList1);
                addImage(ve_resource.DataSourceView_16x, "DataSourceView_16x", imageList1);
                


                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.AssemblyImage, ve_resourceAssembly_6212_24);
                //imageList1.Images.Add("Class", ve_resource._class);
                //imageList1.Images.Add("ImageClass", ve_resource.Class_yellow_256x);
                //imageList1.Images.Add("ClassImage", ve_resource.Class_yellow_256x);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ClassInternalImage, ve_resource._class);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ClassPrivateImage, ve_resource._class);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ClassProtectedImage, ve_resource._class);
                ////imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ConstantImage, ve_resource.Constant_495_24);
                ////imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ConstantInternalImage, ve_resource.Constant_495_24);
                ////imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ConstantPrivateImage, ve_resource.Constant_Private_519_24);
                ////imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ConstantProtectedImage, ve_resource.Constant_Protected_508_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.DelegateImage, ve_resource.Delegate_540_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.DelegateInternalImage, ve_resource.Delegate_540_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.DelegatePrivateImage, ve_resource.Delegate_Private_580_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.DelegateProtectedImage, ve_resource.Delegate_Protected_573_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EnumImage, ve_resource.Enum_582_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EnumInternalImage, ve_resource.Enum_582_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EnumItemImage, ve_resource.EnumItem_588_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EnumPrivateImage, ve_resource.EnumItem_Private_592_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EnumProtectedImage, ve_resource.EnumItem_Protected_591_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ErrorImage, ve_resource.Error_6206_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EventImage, ve_resource.Event_594_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EventInternalImage, ve_resource.Event_594_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EventPrivateImage, ve_resource.Event_Private_598_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.EventProtectedImage, ve_resource.Event_Protected_597_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.FieldImage, ve_resource.FieldIcon_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.FieldInternalImage, ve_resource.FieldIcon_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.FieldPrivateImage, ve_resource.Field_Private_545_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.FieldProtectedImage, ve_resource.Field_Protected_544_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.FolderImage, ve_resource.Folder_6221_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.GrayFolderImage, ve_resource.Folder_6221_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.InterfaceImage, ve_resource.Interface_612_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.InterfaceInternalImage, ve_resource.Interface_612_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.InterfacePrivateImage, ve_resource.Interface_Private_616_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.InterfaceProtectedImage, ve_resource.Interface_Protected_615_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.MethodImage, ve_resource.Method_636);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.MethodInternalImage, ve_resource.Method_636);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.MethodPrivateImage, ve_resource.Method_Private_640_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.MethodProtectedImage, ve_resource.Method_Protected_639_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.ModuleImage, ve_resource.Module_Protected_651_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.NamespaceImage, ve_resource.Namespace_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.PropertyImage, ve_resource.PropertyIcon_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.PropertyInternalImage, ve_resource.PropertyIcon_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.PropertyPrivateImage, ve_resource.PropertyIcon_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.PropertyProtectedImage, ve_resource.ComplexProperty_12904_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.StructureImage, ve_resource.Structure_8338_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.StructureInternalImage, ve_resource.Structure_8338_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.StructurePrivateImage, ve_resource.Structure_Private_512_24);
                //imageList1.Images.Add(AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.Keys.StructureProtectedImage, ve_resource.Structure_Protected_511_24);
            }

            return imageList1;
        }

        static public ImageList CreateImageList()
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            return CreateImageList(s);
        }

        public TreeView LoadProject(string sln)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            string file = sln;

            imageList1 = CreateImageList(s);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            s = sln;

            TreeView msv = new TreeView();

            msv.Nodes.Clear();

            msv.ImageList = imageList1;


            ms = new msbuilder_alls();

            string p = Path.GetDirectoryName(file);

            Directory.SetCurrentDirectory(p);

            string g = Path.GetFileName(file);

            loads_vs_dict();

            ms.dc = dc;

            ms.pgs = pg;

            VSParsers.CSParsers.df = new Dictionary<string, ICSharpCode.NRefactory.TypeSystem.IUnresolvedFile>();

            VSSolution vs = ms.tester(msv, g, "");

            slnpath = s;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

          
            foreach (CreateView_Solution.ProjectItemInfo prs in ms.PL)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = prs.ps.Name;

                nodes.Tag = prs;
            }

            foreach (VSProject vp in vs.Projects)
            {
                vp.vs = vs;
            }

            return msv;
        }

        public TreeView LoadProjects(string sln, string thisproject)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            string file = sln;

            imageList1 = CreateImageList(s);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            s = sln;

            TreeView msv = new TreeView();

            msv.Nodes.Clear();

            msv.ImageList = imageList1;

            imageList1.TransparentColor = Color.Fuchsia;

            msv.ImageList.TransparentColor = Color.Fuchsia;

            ms = new msbuilder_alls();

            string p = Path.GetDirectoryName(file);

            Directory.SetCurrentDirectory(p);

            string g = Path.GetFileName(file);

            loads_vs_dict();

            ms.dc = dc;

            ms.pgs = pg;

            ms.tester(msv, g, thisproject);

            slnpath = s;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            // Control c = eo.cont.getlogger();

            // if (ms.PL == null)
            //     return msv;

            // if (c == null)
            //    return msv;
            // if (c.GetType() != typeof(TreeView))
            //     return msv;

            //TreeView tv = c as TreeView;

            //foreach (CreateView_Solution.ProjectItemInfo prs in ms.PL)
            //{
            //    TreeNode nodes = new TreeNode();
            //    nodes.Text = prs.ps.Name;

            //    nodes.Tag = prs;
            //}

            return msv;
        }

        public void SolutionParser(string paths, TreeView ts)
        {
            ts.Nodes.Clear();

            var solution = VSProvider.SolutionParser.Parse(paths);

            foreach (VSProvider.GlobalSection s in solution.Global)
            {
                TreeNode node = new TreeNode();
                node.Text = s.Name;

                ts.Nodes.Add(node);

                foreach (string key in s.Entries.Keys)
                {
                    string v = s.Entries[key];

                    TreeNode b = new TreeNode();
                    b.Text = key + "  " + v;

                    node.Nodes.Add(b);
                }
            }

            VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(paths));

            foreach (VSProject p in vs.Projects)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = p.FileName;

                ts.Nodes.Add(nodes);

                Microsoft.Build.Evaluation.Project pcs = new Microsoft.Build.Evaluation.Project(p.FileName);

                foreach (Microsoft.Build.Evaluation.ProjectProperty pp in pcs.AllEvaluatedProperties)
                {
                    string pv = pcs.GetPropertyValue(pp.Name);

                    TreeNode ns = new TreeNode();
                    ns.Text = pp.Name + "  " + pv;

                    ListViewItem v = new ListViewItem();
                    v.Text = pp.Name;
                    v.SubItems.Add(pv);

                    listview.Items.Add(v);

                    nodes.Nodes.Add(ns);
                }

                // if (pg != null)
                //     pg.SelectedObjects = pcs.AllEvaluatedProperties.ToArray();
            }
        }

        public Dictionary<string, string> dc { get; set; }

        public void loads_vs_dict()
        {
            if (dc == null)
                dc = new Dictionary<string, string>();
            else
                return;

            Dictionary<string, string> d = dc;
            d.Add(".xsd", "DataSourceView_16x");
            d.Add(".xsc", "filesource");
            d.Add(".xss", "filesource");
            d.Add(".vcproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            d.Add(".h", "Include_13490_24");
            d.Add(".txt", "resource_32xMD");
            d.Add(".md", "resource_32xMD");
            d.Add(".rtf", "resource_32xMD");
            d.Add(".targets", "resource_32xMD");
            d.Add(".dll", "Library_6213");
            d.Add(".xml", "XMLFile_828_16x");
            d.Add(".xaml", "XMLFile_828_16x");
            d.Add(".json", "resourcefile");
            d.Add(".cur", "cursor");
            d.Add(".shproj", "shared");
            d.Add(".cd", "classgray");
            d.Add(".resx", "filesource");
            d.Add(".bmp", "images");
            d.Add(".png", "images");
            d.Add(".jpg", "images");
            d.Add(".gif", "images");
            d.Add(".ico", "icon");
            d.Add(".wav", "audio");
            d.Add(".csx", "script");
            d.Add(".sh", "script");
            d.Add(".bat", "script");
            d.Add(".config", "config");
            d.Add(".vsixmanifest", "config");
            d.Add(".ruleset", "rule");
            d.Add(".pfx", "certificate");
            d.Add(".datasource", "datasource");
            d.Add(".cs", "CSFile_16x");
            d.Add(".vb", "vb");


            d.Add("ASSEMBLY", "ASSEMBLY");
            d.Add("CLASS", "CLASS");
            d.Add("CLASS_FRIEND", "CLASS_FRIEND");
            d.Add("CLASS_PRIVATE", "CLASS_PRIVATE");
            d.Add("CLASS_PROTECTED", "CLASS_PROTECTED");
            d.Add("CLASS_SEALED", "CLASS_SEALED");
            d.Add("CONSTANT", "CONSTANT");
            d.Add("CONSTANT_FRIEND", "CONSTANT_FRIEND");
            d.Add("CONSTANT_PRIVATE", "CONSTANT_PRIVATE");
            d.Add("CONSTANT_PROTECTED", "CONSTANT_PROTECTED");
            d.Add("CONSTANT_SEALED", "CONSTANT_SEALED");
            d.Add("DELEGATE", "DELEGATE");
            d.Add("DELEGATE_FRIEND", "DELEGATE_FRIEND");
            d.Add("DELEGATE_PRIVATE", "DELEGATE_PRIVATE");
            d.Add("DELEGATE_PROTECTED", "DELEGATE_PROTECTED");
            d.Add("DELEGATE_SEALED", "DELEGATE_SEALED");
            d.Add("ENUM", "ENUM");
            d.Add("ENUM_FRIEND", "ENUM_FRIEND");
            d.Add("ENUM_PROTECTED", "ENUM_PROTECTED");
            d.Add("ENUM_SEALED", "ENUM_SEALED");
            d.Add("EVENT", "EVENT");
            d.Add("EVENT_FRIEND", "EVENT_FRIEND");
            d.Add("EVENT_PRIVATE", "EVENT_PRIVATE");
            d.Add("EVENT_PROTECTED", "EVENT_PROTECTED");
            d.Add("EVENT_SEALED", "EVENT_SEALED");
            d.Add("FIELD", "FIELD");
            d.Add("FIELD_FRIEND", "FIELD_FRIEND");
            d.Add("FIELD_PRIVATE", "FIELD_PRIVATE");
            d.Add("FIELD_PROTECTED", "FIELD_PROTECTED");
            d.Add("FIELD_SEALED", "FIELD_SEALED");
            d.Add("INTERFACE", "INTERFACE");
            d.Add("INTERFACE_FRIEND", "INTERFACE_FRIEND");
            d.Add("INTERFACE_PRIVATE", "INTERFACE_PRIVATE");
            d.Add("INTERFACE_PROTECTED", "INTERFACE_PROTECTED");
            d.Add("INTERFACE_SEALED", "INTERFACE_SEALED");
            d.Add("METHOD", "METHOD");
            d.Add("METHOD_FRIEND", "METHOD_FRIEND");
            d.Add("METHOD_PRIVATE", "METHOD_PRIVATE");
            d.Add("METHOD_PROTECTED", "METHOD_PROTECTED");
            d.Add("METHOD_SEALED", "METHOD_SEALED");
            d.Add("METHOD_EXTENSION", "METHOD_EXTENSION");
            d.Add("METHODOVERLOAD", "METHODOVERLOAD");
            d.Add("METHODOVERLOAD_FRIEND", "METHODOVERLOAD_FRIEND");
            d.Add("METHODOVERLOAD_PRIVATE", "METHODOVERLOAD_PRIVATE");
            d.Add("METHODOVERLOAD_PROTECTED", "METHODOVERLOAD_PROTECTED");
            d.Add("METHODOVERLOAD_SEALED", "METHODOVERLOAD_SEALED");
            d.Add("NAMESPACE", "NAMESPACE");
            d.Add("NAMESPACE_FRIEND", "NAMESPACE_FRIEND");
            d.Add("NAMESPACE_PRIVATE", "NAMESPACE_PRIVATE");
            d.Add("NAMESPACE_PROTECTED", "NAMESPACE_PROTECTED");
            d.Add("NAMESPACE_SEALED", "NAMESPACE_SEALED");
            d.Add("PROPERTIES", "PROPERTIES");
            d.Add("PROPERTIES_FRIEND", "PROPERTIES_FRIEND");
            d.Add("PROPERTIES_PRIVATE", "PROPERTIES_PRIVATE");
            d.Add("PROPERTIES_PROTECTED", "PROPERTIES_PROTECTED");
            d.Add("PROPERTIES_SEALED", "PROPERTIES_SEALED");
            d.Add("STRUCTURE", "STRUCTURE");
            d.Add("STRUCTURE_FRIEND", "STRUCTURE_FRIEND");
            d.Add("STRUCTURE_PRIVATE", "STRUCTURE_PRIVATE");
            d.Add("STRUCTURE_PROTECTED", "STRUCTURE_PROTECTED");
            d.Add("STRUCTURE_SEALED", "STRUCTURE_SEALED");
            d.Add("VALUETYPE", "VALUETYPE");
            d.Add("VALUETYPE_FRIEND", "VALUETYPE_FRIEND");
            d.Add("VALUETYPE_PRIVATE", "VALUETYPE_PRIVATE");
            d.Add("VALUETYPE_PROTECTED", "VALUETYPE_PROTECTED");
            d.Add("VALUETYPE_SEALED", "VALUETYPE_SEALED");
        }

        static public Dictionary<string, string> GetDC()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add(".xsd", "DataSourceView_16x");
            d.Add(".xsc", "filesource");
            d.Add(".xss", "filesource");
            d.Add(".vcproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            d.Add(".h", "Include_13490_24");
            d.Add(".txt", "resource_32xMD");
            d.Add(".md", "resource_32xMD");
            d.Add(".rtf", "resource_32xMD");
            d.Add(".targets", "resource_32xMD");
            d.Add(".dll", "Library_6213");
            d.Add(".xml", "XMLFile_828_16x");
            d.Add(".xaml", "XMLFile_828_16x");
            d.Add(".json", "resourcefile");
            d.Add(".cur", "cursor");
            d.Add(".shproj", "shared");
            d.Add(".cd", "classgray");
            d.Add(".resx", "filesource");
            d.Add(".bmp", "images");
            d.Add(".png", "images");
            d.Add(".jpg", "images");
            d.Add(".gif", "images");
            d.Add(".ico", "icon");
            d.Add(".wav", "audio");
            d.Add(".csx", "script");
            d.Add(".sh", "script");
            d.Add(".bat", "script");
            d.Add(".config", "config");
            d.Add(".vsixmanifest", "config");
            d.Add(".ruleset", "rule");
            d.Add(".pfx", "certificate");
            d.Add(".datasource", "datasource");
            d.Add(".cs", "CSFile_16x");
            d.Add(".vb", "vb");

            //d.Add("ASSEMBLY", "ASSEMBLY");
            //d.Add("CLASS", "CLASS");
            //d.Add("CLASS_FRIEND", "CLASS_FRIEND");
            //d.Add("CLASS_PRIVATE", "CLASS_PRIVATE");
            //d.Add("CLASS_PROTECTED", "CLASS_PROTECTED");
            //d.Add("CLASS_SEALED", "CLASS_SEALED");
            //d.Add("CONSTANT", "CONSTANT");
            //d.Add("CONSTANT_FRIEND", "CONSTANT_FRIEND");
            //d.Add("CONSTANT_PRIVATE", "CONSTANT_PRIVATE");
            //d.Add("CONSTANT_PROTECTED", "CONSTANT_PROTECTED");
            //d.Add("CONSTANT_SEALED", "CONSTANT_SEALED");
            //d.Add("DELEGATE", "DELEGATE");
            //d.Add("DELEGATE_FRIEND", "DELEGATE_FRIEND");
            //d.Add("DELEGATE_PRIVATE", "DELEGATE_PRIVATE");
            //d.Add("DELEGATE_PROTECTED", "DELEGATE_PROTECTED");
            //d.Add("DELEGATE_SEALED", "DELEGATE_SEALED");
            //d.Add("ENUM", "ENUM");
            //d.Add("ENUM_FRIEND", "ENUM_FRIEND");
            //d.Add("ENUM_PROTECTED", "ENUM_PROTECTED");
            //d.Add("ENUM_SEALED", "ENUM_SEALED");
            //d.Add("EVENT", "EVENT");
            //d.Add("EVENT_FRIEND", "EVENT_FRIEND");
            //d.Add("EVENT_PRIVATE", "EVENT_PRIVATE");
            //d.Add("EVENT_PROTECTED", "EVENT_PROTECTED");
            //d.Add("EVENT_SEALED", "EVENT_SEALED");
            //d.Add("FIELD", "FIELD");
            //d.Add("FIELD_FRIEND", "FIELD_FRIEND");
            //d.Add("FIELD_PRIVATE", "FIELD_PRIVATE");
            //d.Add("FIELD_PROTECTED", "FIELD_PROTECTED");
            //d.Add("FIELD_SEALED", "FIELD_SEALED");
            //d.Add("INTERFACE", "INTERFACE");
            //d.Add("INTERFACE_FRIEND", "INTERFACE_FRIEND");
            //d.Add("INTERFACE_PRIVATE", "INTERFACE_PRIVATE");
            //d.Add("INTERFACE_PROTECTED", "INTERFACE_PROTECTED");
            //d.Add("INTERFACE_SEALED", "INTERFACE_SEALED");
            //d.Add("METHOD", "METHOD");
            //d.Add("METHOD_FRIEND", "METHOD_FRIEND");
            //d.Add("METHOD_PRIVATE", "METHOD_PRIVATE");
            //d.Add("METHOD_PROTECTED", "METHOD_PROTECTED");
            //d.Add("METHOD_SEALED", "METHOD_SEALED");
            //d.Add("METHOD_EXTENSION", "METHOD_EXTENSION");
            //d.Add("METHODOVERLOAD", "METHODOVERLOAD");
            //d.Add("METHODOVERLOAD_FRIEND", "METHODOVERLOAD_FRIEND");
            //d.Add("METHODOVERLOAD_PRIVATE", "METHODOVERLOAD_PRIVATE");
            //d.Add("METHODOVERLOAD_PROTECTED", "METHODOVERLOAD_PROTECTED");
            //d.Add("METHODOVERLOAD_SEALED", "METHODOVERLOAD_SEALED");
            //d.Add("NAMESPACE", "NAMESPACE");
            //d.Add("NAMESPACE_FRIEND", "NAMESPACE_FRIEND");
            //d.Add("NAMESPACE_PRIVATE", "NAMESPACE_PRIVATE");
            //d.Add("NAMESPACE_PROTECTED", "NAMESPACE_PROTECTED");
            //d.Add("NAMESPACE_SEALED", "NAMESPACE_SEALED");
            //d.Add("PROPERTIES", "PROPERTIES");
            //d.Add("PROPERTIES_FRIEND", "PROPERTIES_FRIEND");
            //d.Add("PROPERTIES_PRIVATE", "PROPERTIES_PRIVATE");
            //d.Add("PROPERTIES_PROTECTED", "PROPERTIES_PROTECTED");
            //d.Add("PROPERTIES_SEALED", "PROPERTIES_SEALED");
            //d.Add("STRUCTURE", "STRUCTURE");
            //d.Add("STRUCTURE_FRIEND", "STRUCTURE_FRIEND");
            //d.Add("STRUCTURE_PRIVATE", "STRUCTURE_PRIVATE");
            //d.Add("STRUCTURE_PROTECTED", "STRUCTURE_PROTECTED");
            //d.Add("STRUCTURE_SEALED", "STRUCTURE_SEALED");
            //d.Add("VALUETYPE", "VALUETYPE");
            //d.Add("VALUETYPE_FRIEND", "VALUETYPE_FRIEND");
            //d.Add("VALUETYPE_PRIVATE", "VALUETYPE_PRIVATE");
            //d.Add("VALUETYPE_PROTECTED", "VALUETYPE_PROTECTED");
            //d.Add("VALUETYPE_SEALED", "VALUETYPE_SEALED");

            return d;
        }

        public void projectmapper(string s, TreeNode ns, bool newnode)
        {
            if (mapper == null)
                mapper = new ClassMapper();

            // mf = msbuilder_alls.GetClassDefinition("sln",s, mapper);

            if (File.Exists(s) == false)
                return;

            //string content = File.ReadAllText(s);

            AnalyzeCSharpFile(s, ns, newnode);
        }

        public string SetProperties(string file)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(file);

            if (pc == null)
                return "";

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");

            string s = outdir.EvaluatedValue + "\\" + assname.EvaluatedValue;

            pc.ProjectCollection.UnloadAllProjects();

            return s;
        }

        public Dictionary<string, TreeNode> dic { get; set; }

        public void CreateNodeDict(TreeView tv)
        {
            dic = new Dictionary<string, TreeNode>(0);

            if (tv == null)
                return;

            if (tv.Nodes.Count <= 0)
                return;

            foreach (TreeNode node in tv.Nodes)
            {
                if (node.Tag != null)
                {
                    CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;

                    //if (p != null)
                    //    if(p.pi != null)
                    //        if (p.psi.fileName != null)
                    //            if (dic.ContainsKey(p.psi.fileName) == false)
                    //        dic.Add(p.psi.fileName, node);
                }

                GetNodes(node);
            }
        }

        public void GetNodes(TreeNode node)
        {
            foreach (TreeNode nodes in node.Nodes)
            {
                if (nodes.Tag != null)
                {
                    CreateView_Solution.ProjectItemInfo p = nodes.Tag as CreateView_Solution.ProjectItemInfo;

                    if (p != null)
                        if (p.psi != null)
                            if (p.psi.fileName != null)
                                if (dic.ContainsKey(p.psi.fileName) == false)
                                    dic.Add(p.psi.fileName, nodes);
                }

                GetNodes(nodes);
            }
        }

        public string GetProjectExec(string file)
        {
            string g = Path.GetFileName(file);

            string p = Path.GetFullPath(file).Replace(g, "");

            string s = SetProperties(file);

            string r = p + "\\" + s;

            return r;
        }

        public void remove_project_item(string projects, string filename)
        {
            Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(projects);

            string s = filename;// pc.FullPath + "\\" + filepath;

            if (File.Exists(s) != true)
                File.Create(s);

            //IList<Microsoft.Build.Evaluation.ProjectItem> items = pc.AddItem("Compile", Path.GetFileName(filename));

            foreach (Microsoft.Build.Evaluation.ProjectItem prs in pc.Items)
            {
                if (prs.ItemType != "Compile")
                    continue;

                if (prs.UnevaluatedInclude == filename)
                {
                    MessageBox.Show("Project compile item + " + filename + "has been removed");

                    pc.RemoveItem(prs);

                    pc.Save();

                    pc = null;

                    return;
                }
            }

            MessageBox.Show("Project compile item + " + filename + "has not been found");
        }

        public void RemoveSolutionItem_Click(object sender, EventArgs e)
        {
            //ApplicationManager app = ApplicationManager.GetInstance();

            TreeView tv = _SolutionTreeView;

            if (tv.SelectedNode == null)
                return;

            TreeNode nodes = tv.SelectedNode;

            if (nodes.Tag == null)
                return;

            //VSProject project = msbuilder_alls.getvsproject(slnpath, 10);

            if (nodes.Tag.GetType() != typeof(ProjectItemInfo))
                return;

            ProjectItemInfo project = nodes.Tag as ProjectItemInfo;

            string filename = project.psi.fileName;

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = eo.cvs.getactiveproject();

            string path = prs.ps.FileName;

            string projectdir = Path.GetFullPath(path).Replace(Path.GetFileName(path), "");

            remove_project_item(path, Path.GetFileName(filename));

            //return;

            //Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(project.ps.FileName);

            //sfd = new SaveFileDialog();
            //sfd.InitialDirectory = pc.DirectoryPath;

            //DialogResult r = sfd.ShowDialog();

            //if (r != DialogResult.OK)
            //    return;

            //string filepath = sfd.FileName;// "newprojectitem.cs";

            //string s = filepath;// pc.FullPath + "\\" + filepath;

            //if (File.Exists(s) != true)
            //    File.Create(s);

            //IList<Microsoft.Build.Evaluation.ProjectItem> items = pc.AddItem("Compile", Path.GetFileName(filepath));
            //pc.Save();

            //foreach (Microsoft.Build.Evaluation.ProjectItem prs in items)
            //{
            //    if (prs.ItemType != "Compile")
            //        continue;

            //    if (prs.UnevaluatedInclude == filepath)
            //        MessageBox.Show("New compile item + " + filepath + "has been created");

            //}

            //pc = null;
        }

        //    pc = null;
        public void selectdir(string path, TreeView tv)
        {
            //DirectoryInfo df = new DirectoryInfo(path);

            if (path == "")
                return;

            path = Path.GetFullPath(path);

            string[] fs = path.Split("\\".ToCharArray());

            string act = "";

            bool first = true;

            int i = 0;

            TreeNode ns = null;

            foreach (string f in fs)
            {
                if (act == "")
                    act = f + "\\";
                else if (i > 1)
                    act = act + "\\" + f;
                else act += f;

                string next = act;

                if (i < fs.Length - 1)
                {
                    next = act + "\\" + fs[i + 1];
                }

                if (first == true)
                {
                    if(tv != null)
                    foreach (TreeNode node in tv.Nodes)
                    {
                        string p = node.Tag as string;

                        if (p.ToUpper() == act.ToUpper())
                        {
                            node.Expand();

                            ns = node;

                            break;
                        }

                        //if (!_expandedPaths.Contains(path))
                        //    _expandedPaths.Add(path);
                    }

                    first = false;
                }
                else
                {
                    if (ns != null)
                        foreach (TreeNode node in ns.Nodes)
                        {
                            string p = node.Tag as string;

                            if (p.ToUpper() == act.ToUpper())
                            {
                                node.Expand();

                                ns = node;
                                node.EnsureVisible();

                                tv.SelectedNode = node;

                                break;
                            }

                            //if (!_expandedPaths.Contains(path))
                            //    _expandedPaths.Add(path);
                        }
                }

                i++;
            }

            if (ns == null)
                return;

            foreach (TreeNode n in ns.Nodes)
            {
                if (n.Tag == null)
                    continue;

                if (n.Tag.GetType() != typeof(FileInfo))
                    return;

                FileInfo fg = (FileInfo)n.Tag;

                if (fg.FullName == path)
                {
                    tv.SelectedNode = n;
                    break;
                }
            }
        }

        private void addImage(string imageToLoad)
        {
            if (imageToLoad != "")
            {
                imageList1.Images.Add(Image.FromFile(imageToLoad));
                listView1.BeginUpdate();
                listView1.Items.Add(imageToLoad, baseValue++);
                listView1.EndUpdate();
            }
        }

        private void addImage(Bitmap bmp, string name)
        {
            if (imageList1 == null)
                imageList1 = new ImageList();

            if (bmp != null)
            {
                imageList1.Images.Add(name, (Image)bmp);
                //listView1.BeginUpdate();
                //listView1.Items.Add(imageToLoad, baseValue++);
                //listView1.EndUpdate();
            }
        }

        //static public void addImage(Bitmap bmp, string name,  ImageList imageList1)
        //{
        //    if (imageList1 == null)
        //        imageList1 = new ImageList();

        //    if (bmp != null)
        //    {
        //        imageList1.Images.Add(name, (Image)bmp);
        //        //listView1.BeginUpdate();
        //        //listView1.Items.Add(imageToLoad, baseValue++);
        //        //listView1.EndUpdate();
        //    }
        //}

        private void loadImageLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (ofd.FileNames != null)
                {
                    for (int i = 0; i < ofd.FileNames.Length; i++)
                    {
                        addImage(ofd.FileNames[i]);
                    }
                }
                else
                    addImage(ofd.FileName);
            }
        }

        private void open_solution(object sender, EventArgs e)
        {
            TreeNode node = _maintv.SelectedNode;

            if (node == null)
                return;

            if (node.Tag == null)
                return;

            string s = node.Tag as string;

            slnpath = s;

            LoadProject(s);

            string d = AppDomain.CurrentDomain.BaseDirectory;

            string fg = d + "\\" + "currentproject.txt";

            if (File.Exists(fg) == false)
                File.Create(fg);

            try
            {
                File.WriteAllText(d + "\\" + "currentproject.txt", slnpath);
            }
            catch (Exception ex) { };

            if (hst == null)
                return;
            hst.additem(s);
        }

        private void refresh_solution(object sender, EventArgs e)
        {
            if (slnpath == null || slnpath == "" || File.Exists(slnpath) == false)
            {
                MessageBox.Show("No solution has been loaded...");
                return;
            }

            LoadProject(slnfile);

            //Controller cont = eo.cont;

            //Control a = cont.getact();
            //a.Visible = false;

            //cont.nextview();
            //Control b = cont.getact();
            //b.Visible = true;

            //utils.update_z_order(frm);
        }

        //    }
    

        private void run_solution(object sender, EventArgs e)
        {
            string project = _projectList.Text;

            VSSolution vs = GetVSSolution();

            VSProject p = GetVSProject();

            if (vs != null)
            {
                p = vs.GetProjectbyName(project);
            }

            if (p == null)
                return;

            
            p.RunProject();
        }

        public VSSolution GetVSSolution()
        {
            return _SolutionTreeView.Tag as VSSolution;
        }

        public VSProject GetVSProject()
        {
            TreeNode node = _SolutionTreeView.SelectedNode;
            if (node == null)
                return null;
            CreateView_Solution.ProjectItemInfo s = node.Tag as CreateView_Solution.ProjectItemInfo;
            if (s == null)
                return null; ;
            return s.ps;
        }

        private void SelectView_Click(object sender, EventArgs e)
        {
            Controller cont = eo.cont;

            ArrayList W = cont.getviews();

            Control a = cont.getact();
            a.Visible = false;

            cont.nextview();
            Control b = cont.getact();
            b.Visible = true;

            foreach (Control c in W)
            {
                if (c != a)
                    if (c != b)
                        c.Visible = false;
            }

            utils.update_z_order(frm);
        }

        private void TreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            string s = e.Node.Text;

            TreeNode node = e.Node;

            //node.ImageKey = "Class_5893_24";

            if (e.Node.Tag == null)
                return;

            if (e.Node.Tag.GetType() != typeof(VSProjectItem))
                return;

            VSProjectItem pi = (VSProjectItem)e.Node.Tag;

            string file = pi.fileName;

            //      _applicationManager.MainForm.LoadDocumentIntoWindow(r.fileName, true);

            //eo.tw.MoveToSelectedFolder(s);
            //  eo.tw.PopulateTreeNode(_maintv.Nodes, s, null);

            // eo.tw.selectdir(s, _maintv);

            //LoadProject(s);
        }

        private TreeNode _oldselectednode;

        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }

        //}
        /// <summary>
        /// Handles the AfterSelect event of the treeView1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TreeViewEventArgs"/> instance containing the event data.</param>
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Right)
                return;

            string s = e.Node.Text;

            TreeNode node = e.Node;

            //node.ImageKey = "CSfile_6224_24";

            //node.SelectedImageKey = "CSfile_6224_24";

            //node.SelectedImageIndex = 0;

            string file = "";

            if (e.Node.Tag == null)
                return;

            ProjectItemInfo pr0 = null;

            if (e.Node.Tag.GetType() != typeof(ProjectItemInfo))
                return;

            pr0 = e.Node.Tag as ProjectItemInfo;

            if (pr0.psi == null)
                if (pr0.mapper != null)
                {
                    file = mapper.classname;

                    projectmapper(file, node, false);

                    //node.ImageKey = "CSfile_6224_24";

                    if (File.Exists(file) == true)
                    {
                        // eo._applicationManager.NotifyMainMessage(file);

                        //eo._applicationManager.NotifyImageFolderMessage(Path.GetFullPath(file).Replace(Path.GetFileName(file), ""));

                        //eo._applicationManager.MainForm.LoadDocumentIntoWindow(file, true);
                    }
                    //node.ImageKey = "CSfile_6224_24";
                    return;
                }

            //if (e.Node.Tag.GetType() == typeof(VSProject))
            //    return;

            if (pr0.psi != null)
            //if (e.Node.Tag.GetType() == typeof(VSProjectItem))
            {
                //    return;

                VSProjectItem pi = pr0.psi;// (VSProjectItem)e.Node.Tag;

                file = pi.fileName;

                if (node.Nodes.Count >= 1)
                {
                    if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
                    {
                        //projectmapper(file, node, true);
                        //node.ImageKey = "CSfile_6224_24";
                        return;
                    }
                    else
                    {
                        projectmapper(file, node, false);
                    }
                }
            }
            else //if (e.Node.Tag.GetType() == typeof(ProjectItemInfo))
            {
                ProjectItemInfo pr = pr0;// (ProjectItemInfo)pr0.pi;// e.Node.Tag;

                return;

                //if (pr.filepath != "")
                //{
                //    file = pr.filepath;

                //    projectmapper(file, node, true);
                //    node.ImageKey = "CSfile_6224_24";
                //}
            }

            //            if (node.Nodes.Count <=1)

            //if (node.Nodes.Count == 0)
            //{
            //    if(node.Tag != null)
            //    projectmapper(file, node, false);
            //    else
            //        projectmapper(file, node, true);

            //    return;

            //}

            //else
            //if (node.Nodes[0].GetType() != typeof(TreeViewBase.DummyNode))
            //{
            //    projectmapper(file, node, true);

            //    return;

            //}

            projectmapper(file, node, false);

            if (File.Exists(file) == true)
            {
                //eo._applicationManager.NotifyMainMessage(file);

                //eo._applicationManager.NotifyImageFolderMessage(Path.GetFullPath(file).Replace(Path.GetFileName(file), ""));

                //eo.LoadFile(file, pr0.ps.Name, new Bitmap(ve_resource.CSharpFile_SolutionExplorerNode_24), pr0.ps);

                //eo._applicationManager.MainForm.LoadDocumentIntoWindow(file, true);
            }

            //node.ImageKey = "CSfile_6224_24";

            // e.Node.Parent.ImageKey = "CSfile_6224_24";

            //TreeView se = sender as TreeView;
            //se.Refresh();
        }

        public class ProjectItemInfo : object
        {
            [Category("File")]
            public string filepath { get; set; }
            [Category("File")]
            public ClassMapper mapper { get; set; }
            [Category("File")]
            public VSProvider.Project pg { get; set; }


            [Category("Projects")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VSProject ps { get; set; }
            [Category("Projects")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VSProjectItem psi { get; set; }
            [Category("Projects")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public VSSolution vs { get; set; }

            public bool isItem = true;

            public string SubType { get; set; }

            public bool IsProjectNode { get; set; }

            public string Guid { get; set; }

            public ProjectItemInfo()
            {
                IsProjectNode = false;
            }

            public bool IsItem()
            {
                return isItem;
            }
            public string Include { get; set; }
        }
    }

    public class ExplorerObject
    {
        public bool _applyFileNameFilter;

        public IContainer _components = null;

        public bool _currentDirectoryFollowsRoot;

        public Dictionary<string, string> _documentTypeIconMap;
        public List<string> _expandedPaths;
        public string _rootFolder;

        public Controller cont = new Controller();
        public ArrayList W = new ArrayList();
        static private ExplorerObject s_singleton;

        public AIMS.Libraries.Scripting.ScriptControl.ScriptControl script { get; set; }

        private ExplorerObject()
        {
        }

        public string currentproject { get; set; }

        public CreateView_Solution cvs { get; set; }

        public Control master { get; set; }

        public TreeViewBase tw { get; set; }

        /// <summary>
        /// Provides access to the ApplicationManager singleton.
        /// </summary>
        /// <returns>A reference to the ApplicationManager.</returns>
        public static ExplorerObject GetInstance()
        {
            if (s_singleton == null)
                s_singleton = new ExplorerObject();

            return s_singleton;
        }

        public void addview(TreeView tv)
        {
            W.Add(tv);
        }

        public void LoadFile(string file, Dictionary<string, string> dc, ImageList bmp, VSSolution vs)
        {
            if (script == null)
                return;
            script.OpenDocuments(file, vs);
        }

        public void LoadListTabs(Form f, string r)
        {
            ProjectPropertyForm ppf = new ProjectPropertyForm();
            ppf.Dock = DockStyle.Fill;
            ppf.TopLevel = false;
            ppf.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            f.Controls.Add(ppf);

            ppf.Show();

            WinExplorer.CreateView_Solution.ProjectItemInfo prs = cvs.getactiveproject();

            ppf.Text = Path.GetFileNameWithoutExtension(prs.filepath);

            if (prs == null)
            {
                MessageBox.Show("No project has been selected..");
                return;
            }

            if (prs.ps == null)
                return;

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(prs.ps.FileName);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            if (pc == null)
            {
                MessageBox.Show("Project " + Path.GetFileNameWithoutExtension(prs.ps.FileName) + " could not be opened properly.");

                return;
            }

            ppf.pc = pc;

            ppf.vp = prs.ps;

            ppf.vs = ppf.vp.solution;

            ppf.GetProperties();

            ppf.LoadResourcesView(r);
        }

        public void LoadFile(string file, VSSolution vs,  AutoResetEvent autoEvent = null  )
        {
            if (script == null)
                return;


            //if(vs != null)
            //if (vp.IsResourceEmbedded(file) == true)
            //{
            //    DocumentForm df = OpenDocumentForm(file);

            //    df.FileName = vp.FileName;

            //    df.resource = file;

            //    LoadListTabs(df, file);

            //    return;
            //}

            script.BeginInvoke(new Action(() => { script.OpenDocuments(file, vs, autoEvent);

                //if (autoEvent != null)
                //    autoEvent.Set();

                Command.counter--;

                
            }));
            
        }

        public void LoadFile(string file, string name)
        {
            if (script == null)
                return;
            script.OpenDocuments(file, name, null);
        }

        public void SelectText(int start, int sc, int length)
        {
            if (script == null)
                return;

            script.SelectText(start, sc, length);
        }

        public DocumentForm OpenDocumentForm(string project)
        {
            if (script == null)
                return null;
            DocumentForm df = script.OpenDocumentForm();
            df.FileName = project;
            df.TabText = project;
            return df;
        }

        public WebForm OpenWebForm(string web)
        {
            if (script == null)
                return null;
            WebForm df = script.OpenWebForm(web, web);
            
            return df;

        }
    }

    /// <summary>
    /// abstract base class for the ToolStripButton handlings.
    /// </summary>
    public class InternalEventHandler
    {
        #region Fields

        /// <summary>
        /// the delegate for this event handler
        /// </summary>
        private Delegate _delegate = null;

        /// <summary>
        /// list of object's generating events that this handler is rerouting.
        /// </summary>
        private List<Object> _subscribers = new List<object>();

        #endregion Fields

        /// <summary>
        /// construct the event handler class.
        /// </summary>
        /// <param name="info"></param>
        public InternalEventHandler(EventInfo info)
        {
            Info = info;
        }

        #region Properties



        /// <summary>
        /// gets a delegate to the HandleEvent method implemented in event-handler specific classes derived from this class.
        /// </summary>
        /// <returns></returns>
        public virtual Delegate EventHandlerDelegate
        {
            get
            {
                // keep a reference to the delegate. this allows it to be removed.
                if (_delegate == null)
                    _delegate = Delegate.CreateDelegate(Info.EventHandlerType, this, this.GetType().GetMethod("HandleEvent"));

                // return the delegate to the HandleEvent method
                return _delegate;
            }
        }

        /// <summary>
        /// gets the name of the event for which this handler is created.
        /// </summary>
        public virtual String EventName { get { return Info.Name; } }

        /// <summary>
        /// gets the event for which this handler is created.
        /// </summary>
        public virtual EventInfo Info { get; internal set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// attaches this event-handler to the event on object obj.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void AttachToEventOn(object obj)
        {
            try
            {
                // attach the handler to the event on object obj
                Info.AddEventHandler(obj, EventHandlerDelegate);

                // add to the list of subscribers:
                _subscribers.Add(obj);
            }
            catch (Exception ex1)
            {
                Console.WriteLine("Could Not Bind to Event: " + Info.Name + " on: " + obj.ToString());
            }
        }

        /// <summary>
        /// detatches all event handlers.
        /// </summary>
        public virtual void DetachAll()
        {
            // iterate the list of object's this event handler is attached to:
            foreach (object obj in _subscribers)
            {
                // remove this event handler delegate from the object:
                Info.RemoveEventHandler(obj, EventHandlerDelegate);
            }

            // reset the list of subscribers:
            _subscribers.Clear();
        }

        /// <summary>
        /// removes this event handler from the target object.
        /// </summary>
        /// <param name="obj"></param>
        public virtual void DetachFrom(object obj)
        {
            if (_subscribers.Contains(obj))
            {
                // remove this event handler from the object:
                Info.RemoveEventHandler(obj, EventHandlerDelegate);

                // remove the object from the list:
                _subscribers.Remove(obj);
            }
        }

        #endregion Methods
    }

    public class TreeViewBase
    {
        public Dictionary<string, string> _documentTypeIconMap;

        public TreeView _mainTreeView { get; set; }

        public ExplorerObject eo { get; set; }

        public class DummyNode : TreeNode { }

        #region Helpers




        //}
        public void RefreshView()
        {
            //  if (!Visible) return;

            /*
             * Call pre refresh event handlers.
             */

            //    if (TreeViewPreRefresh != null)
            //        TreeViewPreRefresh();

            /*
             * Save the top visible node for restoration after the refresh.
             * We can't use the TopNode property directly as we will have
             * a completely new set of nodes.
             */

            string topVisibleNodeKey = null;
            string topVisibleNodePath = null;

            if (_mainTreeView.TopNode != null)
            {
                topVisibleNodeKey = _mainTreeView.TopNode.Name;
                topVisibleNodePath = _mainTreeView.TopNode.FullPath;
            }
        }

        public void selectdir(string path, TreeView tv)
        {
            string[] fs = path.Split("\\".ToCharArray());

            string act = "";

            bool first = true;

            int i = 0;

            TreeNode ns = null;

            foreach (string f in fs)
            {
                if (act == "")
                    act = f + "\\";
                else if (i > 1)
                    act = act + "\\" + f;
                else act += f;

                string next = act;

                if (i < fs.Length - 1)
                {
                    next = act + "\\" + fs[i + 1];
                }

                if (first == true)
                {
                    foreach (TreeNode node in tv.Nodes)
                    {
                        string p = node.Tag as string;

                        if (p == act)
                        {
                            node.Expand();

                            ns = node;

                            break;
                        }

                        //if (!_expandedPaths.Contains(path))
                        //    _expandedPaths.Add(path);
                    }

                    first = false;
                }
                else
                {
                }

                i++;
            }

            if (ns == null)
                return;

            foreach (TreeNode n in ns.Nodes)
            {
                if (n.Tag == null)
                    continue;

                if (n.Tag.GetType() != typeof(FileInfo))
                    return;

                FileInfo fg = (FileInfo)n.Tag;

                if (fg.FullName == path)
                {
                    tv.SelectedNode = n;
                    break;
                }
            }
        }

        //public void SelectFolder(string path)
        //{
        //    string savePath = Directory.GetCurrentDirectory();

        //    string[] fs = path.Split("\\".ToCharArray());

        //    string act = "";

        //    foreach (string f in fs)
        //    {
        //        act = f;

        //        DirectoryInfo di = new DirectoryInfo(act);

        //        var directories = di.GetFiles("*", SearchOption.AllDirectories);

        //        foreach (DirectoryInfo d in directories)
        //        {
        //            TreeNode node = new TreeNode(d.Name);

        //        }

        //    }
        //private String GetZipFileName(String sourceDirectory)
        //{
        //    /*
        //     * Check user settings for destination folder.
        //     */

        //    String userHome = eo._settingsManager.BackupDirectory;

        //    if (!Directory.Exists(userHome))
        //        userHome = String.Empty;

        //    /*
        //     * If no path request one from the user.
        //     */

        //    if (userHome == String.Empty)
        //    {
        //        userHome = Environment.GetFolderPath(
        //            Environment.SpecialFolder.MyDocuments);

        //        FolderBrowserDialog fbd = new FolderBrowserDialog();
        //        fbd.SelectedPath = userHome;
        //        fbd.Description = ve_resource.BackupFolderMessage;

        //        if (fbd.ShowDialog() == DialogResult.Cancel)
        //            return null;

        //        userHome = fbd.SelectedPath;
        //    }

        //    /*
        //     * Construct a name from the workspace name, date and time.
        //     */

        //    String baseName = Path.GetFileName(sourceDirectory);
        //    if (String.IsNullOrEmpty(baseName)) return null;

        //    DateTime now = DateTime.Now;

        //    String fileName = String.Format(
        //        "{0}_{1:D4}{2:D2}{3:D2}{4:D2}{5:D2}{6:D2}.zip",
        //        baseName, now.Year, now.Month, now.Day,
        //        now.Hour, now.Minute, now.Second);

        //    return Path.Combine(userHome, fileName);
        //}

        #endregion Helpers
    }
    public class utils
    {
        static public ArrayList IEnumerable_to_ArrayList(IEnumerable e)
        {
            ArrayList L = new ArrayList();

            IEnumerator er = e.GetEnumerator();

            er.Reset();

            int i = 0;
            while (er.MoveNext() == true)
            {
                L.Add(er.Current);

                i++;
            }

            return L;
        }

        static public void update_z_order(Form frm)
        {
            frm.SuspendLayout();

            ArrayList L = new ArrayList();

            foreach (Control c in frm.Controls)
            {
                L.Add(c);
            }
            //foreach (Control c in L)
            //{
            //    Controls.Remove(c);
            //}

            foreach (Control c in L)
            {
                if (c.GetType() == typeof(ToolStripPanel) || c.GetType() == typeof(ToolStrip) || c.GetType() == typeof(StatusStrip))
                    frm.Controls.Remove(c);
                if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true || c.GetType() == typeof(MenuStrip) || c.GetType() == typeof(StatusStrip))
                    frm.Controls.Remove(c);
            }
            foreach (Control c in L)
            {
                if (c.GetType() == typeof(ToolStrip))
                    frm.Controls.Add(c);
                if (c.GetType() == typeof(ToolStripPanel))
                    frm.Controls.Add(c);
                if (c.GetType() == typeof(MenuStrip))
                    frm.Controls.Add(c);
                if (c.GetType() == typeof(StatusStrip))
                    frm.Controls.Add(c);
                if (c.GetType().IsSubclassOf(typeof(MenuStrip)) == true)
                    frm.Controls.Add(c);
            }

            frm.ResumeLayout();
        }
    }

    #region Utility

    /// <summary>
    /// utility methods for generating event handler code.
    /// </summary>
    public static class CodeUtils
    {
        /// <summary>
        /// write an event-handler class for the given event handler type.
        /// </summary>
        /// <param name="eventInfo"></param>
        /// <returns></returns>
        public static String GenerateCode(TypeInfo typeInfo)
        {
            // define the class template: spots are marked for replacement (ie %NAME%)
            string baseCode = @"using System;
                                using System.Collections.Generic;
                                using System.IO;
                                using System.Linq;
                                using System.Text;
                                using System.Reflection;
                                using System.Windows.Forms;

                                namespace WinExplorer
                                {
                                    public class %NAME%ToolStripButtonCreator : ToolStripButtonCreator
                                    {
                                            public %NAME%ToolStripButtonCreator() : base()
                                            {
                                            }

                                            public override void DefaultHandler()
                                            {
               if(_appManager == null)
                  return;
                   DockPanel panel = _appManager.MainForm.panel;
                panel.SaveAsXml(""testc.xml"");
              MessageBox.Show(""Dynamic .NET 4.5.1"");
                                            }
                                    }
                                }";

            // determine the parameters of the event:
            //ParameterInfo[] handlerArgs = typeInfo.MemberType.GetMethod("Invoke").GetParameters();

            // parameter count is a replacement variable:
            //int paramCount = handlerArgs.Length;

            //string parameters = "";     // becommes "Object sender, EventArgs ev";
            //string paramAssign = "";    // becomes args[0]=sender; args[1]=e etc.
            //int i = 0;                  // current parameter id;

            //// enumerate the handler arguments generating the appropriate code:
            //foreach (var p in handlerArgs)
            //{
            //    // handle comma-seperated arguments:
            //    if (parameters.Length > 0)
            //        parameters += ", ";

            //    // this is the list of arguments for the HandleEvent method:
            //    parameters += p.ParameterType.FullName + " " + p.Name;

            //    // this will be the code to assign the values of the arguments to an array:
            //    paramAssign += "args[" + i++ + "]=" + p.Name + ";\r\n\t\t\t";
            //}

            // perform string replacements to create the final class:
            baseCode = baseCode.Replace("%NAME%", typeInfo.Name);
            //baseCode = baseCode.Replace("%PARAMETERS%", parameters);
            //baseCode = baseCode.Replace("%PARAMCOUNT%", paramCount.ToString());
            //baseCode = baseCode.Replace("%PARAMASSIGNMENT%", paramAssign);

            // return the formatted code:
            return baseCode;
        }

        #region Code Dom Util Methods

        /// <summary>
        /// create and return a System.Type from the specified C# code.
        /// </summary>
        /// <param name="codeDefinition"></param>
        /// <returns></returns>
        public static Type CreateTypes(String codeDefinition)
        {
            // create the provider:
            var provider = CodeDomProvider.CreateProvider("CSharp");

            // create the compilere parameters object:
            var parameters = new CompilerParameters(GetAssemblyRefs());

            // set some options: this should all be kept in RAM
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;

            File.WriteAllText("code.cs", codeDefinition);

            // compile the code:
            var results = provider.CompileAssemblyFromSource(parameters, codeDefinition);

            // generate an exeption if the compilation generated errors:
            if (results.Errors.HasErrors)
            {
                Exception e = new Exception("Compiler Encountered Errors!");
                foreach (CompilerError error in results.Errors)
                {
                    if (!error.IsWarning)
                    {
                        e.Data.Add(error.ErrorNumber, error);
                    }
                }
                throw e;
            }
            else
            {
                // get the array of types from the newly compiled assembly:
                Type[] types = results.CompiledAssembly.GetTypes();

                // return the first item in the list:
                if (types.Length > 0)
                    return types[0];
                else
                    throw new Exception("No Types Returned!");
            }
        }

        /// <summary>
        /// gets an array of file-names of the current referenced assemblies of the executing assembly.
        /// </summary>
        /// <returns></returns>
        public static String[] GetAssemblyRefs()
        {
            // empty list:
            List<String> referencedAssemblies = new List<string>();

            // add the filename of each referenced assembly:
            foreach (var assemblyRef in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                referencedAssemblies.Add(System.Reflection.Assembly.Load(assemblyRef).Location);

            // add the current application to the list of referenced assemblies:
            referencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // return the string-array:
            return referencedAssemblies.ToArray();
        }

        #endregion Code Dom Util Methods

        /// <returns></returns>
        static private Type GenerateHandlerType(TypeInfo info)
        {
            //// return the handler type from cache if already generated:
            //if (typeCache.ContainsKey(info.EventHandlerType))
            //    return typeCache[info.EventHandlerType];

            // create the code and compile to a new type:
            string code = CodeUtils.GenerateCode(info);

            Type handlerType = CodeUtils.CreateTypes(code);

            // add into the type cache.
            //typeCache.Add(info.EventHandlerType, handlerType);

            // return the type for the event handler:
            return handlerType;
        }
    }

    #endregion Utility

    public class History
    {
        public ArrayList L { get; set; }

        public string filename { get; set; }

        public string path { get; set; }

        public TreeView tv { get; set; }

        public History()
        {
            L = new ArrayList();

            filename = "vs.lnks";
        }

        public void additem(string s)
        {
            if (L.IndexOf(s) >= 0)
                return;

            L.Add(s);

            if (tv == null)
                return;

            TreeNode node = new TreeNode();
            node.Text = s;

            tv.Nodes.Add(node);

            savelinkerfile();
        }

        public void remitem(string s)
        {
            ArrayList N = new ArrayList();

            foreach (string text in L)
            {
                if (text != s)
                    N.Add(text);
            }

            L = N;

            savelinkerfile();
        }
        public void remitemsall()
        {
            L = new ArrayList();

            savelinkerfile();
        }
        public void ReloadRecentSolutions()
        {
            FileInfo info = new FileInfo(filename);

            if (info.Exists == false)
            {
                FileStream f = File.Create(filename);
                f.Close();

                return;
            }

            if (info.Extension != ".lnks")
                return;

            string[] c = File.ReadAllLines(info.FullName);

            path = info.FullName;
            
            tv.BeginUpdate();

            ImageList imgs = new ImageList();

            imgs.Images.Add(ve_resource.VisualStudioSolution_256x);

            tv.ImageList = imgs;
            
            tv.Nodes.Clear();
            
            foreach (string g in c)
            {
                if (g.Trim() == "")
                    continue;

                TreeNode node = new TreeNode();
                string img = "solution";
                node.ImageKey = img;
                node.SelectedImageKey = img;
                node.StateImageKey = img;
                node.Text = g;

                tv.Nodes.Add(g);

                L.Add(g);
            }

            tv.EndUpdate();
        }
        public List<string> GetRecentSolutions()
        {
            List<string> rs = new List<string>();

            FileInfo info = new FileInfo(filename);

            if (info.Exists == false)
            {
                FileStream f = File.Create(filename);
                f.Close();

                return rs;
            }

            if (info.Extension != ".lnks")
                return rs;

            string[] c = File.ReadAllLines(info.FullName);
            
            foreach (string g in c)
            {
                if (g.Trim() == "")
                    continue;

          
                rs.Add(g);
            }

            return rs;
        }

        public void savelinkerfile()
        {
            int N = L.Count;

            string c = "";

            int i = 0;
            while (i < N)
            {

                if (L[i] != null)
                {


                    string g = L[i].ToString();

                    if (g.Trim() != "")

                        c = c + "\n" + g;
                }
                i++;
            }

            File.WriteAllText(filename, c);
        }
    }
}