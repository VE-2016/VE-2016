using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class ClipboardForm : Form
    {
        public ClipboardForm()
        {
            InitializeComponent();

            SuspendLayout();

            tv = treeView1;
            sp = splitContainer1;
            sp.Panel2Collapsed = true;
            pg = propertyGrid1;
            this.Width = 300;
            this.BackColor = SystemColors.Control;
            ImageList imgs = CreateView_Solution.CreateImageList();

            tv.ImageList = imgs;

            ResumeLayout();
        }

        private SplitContainer sp { get; set; }

        private PropertyGrid pg { get; set; }
        static public TreeView tv { get; set; }

        static public ArrayList L { get; set; }

        public void DisplayItems(ArrayList LL)
        {
            L = LL;

            tv.Nodes.Clear();

            foreach (string s in L)
            {
            }
        }

        public void DeleteClipboard(TreeNode nodes)
        {
            tv.Nodes.Clear();
            tv.FullRowSelect = true;
            tv.CheckBoxes = true;

            //if (nc == null)
            //    return;
            //tv.Nodes.Add(nc);


            List<TreeNode> n = new List<TreeNode>();
            n.Add(nodes);

            LoadTreeNode(n);

            target = nodes;

            //src = GetSourceType(nc);

            dst = GetSourceType(nodes);

            //if (src == opers.forms)
            {
                MessageBox.Show("Remove items has been selected...");

                //string prefix = "";/// sourceprefix(nc);

                ArrayList L = GetItems(nc);

                VSProjectItem ps = GetVSProjectItem(target);

                ArrayList T = GetTargetItems(L, dst);

                VSProject pp = GetVSProject(nodes);

                pp.RemoveItems(T);

                //pp.LoadMetaData(L);

                //pp.SetItems(T, L);

                //CopyItems(L, T);

                return;
            }
        }

        public TreeNode target { get; set; }

        public void ReloadClipboard(TreeNode nodes)
        {
            tv.Nodes.Clear();
            tv.FullRowSelect = true;
            tv.CheckBoxes = true;

            if (nc == null)
                return;
            tv.Nodes.Add(nc);

            target = nodes;

            src = GetSourceType(nc);

            dst = GetSourceType(nodes);

            if (src == opers.forms)
            {
                MessageBox.Show("The copy of form has been selected...");

                string prefix = sourceprefix(nc);

                ArrayList L = GetItems(nc);



                VSProjectItem ps = GetVSProjectItem(target);

                VSProject pp = GetVSProject(nodes);

                ArrayList T = GetTargetItems(L, prefix, ps.Include);



                pp.LoadMetaData(L);

                pp.SetItems(T, L);

                CopyItems(L, T, pp);

                return;
            }

            if (src == opers.folder)
            {
                if (dst == opers.folder)
                {
                    MessageBox.Show("Two folders have been selected...");
                    //return;

                    string prefix = sourceprefix(nc);

                    ArrayList L = GetItems(nc);

                    VSProjectItem ps = GetVSProjectItem(target);

                    ArrayList T = GetTargetItems(L, prefix, ps.Include);

                    VSProject pp = GetVSProject(nodes);

                    pp.LoadMetaData(L);

                    pp.SetItems(T, L);

                    CopyItems(L, T, pp);

                    return;
                }

                if (dst == opers.project)
                {
                    MessageBox.Show("The copy to project has been selected...");
                    //return;

                    string prefix = sourceprefix(nc);

                    ArrayList L = GetItems(nc);

                    VSProjectItem ps = GetVSProjectItem(target);

                    ArrayList T = GetTargetItems(L, prefix, ps.Include);

                    VSProject pp = GetVSProject(nodes);

                    pp.LoadMetaData(L);

                    pp.SetItems(T, L);

                    CopyItems(L, T);

                    return;
                }
            }

            if (src == opers.file)
            {
                if (dst == opers.folder)
                {
                    string prefix = sourceprefix(nc);

                    ArrayList L = GetItems(nc);

                    VSProjectItem ps = GetVSProjectItem(target);

                    ArrayList T = GetTargetItems(L, prefix, ps.Include);

                    VSProject pp = GetVSProject(nodes);

                    pp.LoadMetaData(L);

                    pp.SetItems(T, L);

                    CopyItems(L, T);

                    return;
                }

                if (dst == opers.project)
                {
                    MessageBox.Show("The copy of the file to project has been selected...");
                    //return;

                    string prefix = sourceprefix(nc);

                    ArrayList L = GetItems(nc);

                    VSProjectItem ps = GetVSProjectItem(target);

                    if (ps.Include == null)
                        ps.Include = "";

                    ArrayList T = GetTargetItems(L, prefix, ps.Include);

                    VSProject pp = GetVSProject(nodes);

                    pp.LoadMetaData(L);

                    pp.SetItems(T, L);

                    CopyItems(L, T);

                    return;
                }
            }
        }

        public opers src { get; set; }

        public opers dst { get; set; }

        public void CopyItems(ArrayList L, ArrayList T)
        {
            VSProject p = GetVSProject(nc);

            string folder = p.GetProjectFolder();

            int N = L.Count;

            int i = 0;
            while (i < N)
            {
                VSProjectItem s = L[i] as VSProjectItem;

                VSProjectItem d = T[i] as VSProjectItem;

                string source = folder + "\\" + s.Include;

                string dest = folder + "\\" + d.Include;

                if (s.Include == null)
                {
                    i++;
                    continue;
                }

                if (File.Exists(source))
                {
                    if (File.Exists(dest) == false)
                        File.Copy(source, dest);
                }
                else if (Directory.Exists(source))
                {
                    if (Directory.Exists(dest) == false)
                        Directory.CreateDirectory(dest);
                }

                i++;
            }
        }
        public void CopyItems(ArrayList L, ArrayList T, VSProject vp)
        {
            VSProject p = GetVSProject(nc);

            string folder = p.GetProjectFolder();

            int N = L.Count;

            int i = 0;
            while (i < N)
            {
                VSProjectItem s = L[i] as VSProjectItem;

                VSProjectItem d = T[i] as VSProjectItem;

                string source = folder + "\\" + s.Include;

                string dest = vp.GetProjectFolder() + "\\" + d.Include;

                if (s.Include == null)
                {
                    i++;
                    continue;
                }

                if (File.Exists(source))
                {
                    if (File.Exists(dest) == false)
                        File.Copy(source, dest);
                }
                else if (Directory.Exists(source))
                {
                    if (Directory.Exists(dest) == false)
                        Directory.CreateDirectory(dest);
                }

                i++;
            }
        }
        public ArrayList GetTargetItems(ArrayList L, opers ops)
        {
            ArrayList T = new ArrayList();

            foreach (VSProjectItem pp in L)
            {
                //VSProjectItem p = new VSProjectItem();
                //p.ItemType = pp.ItemType;
                //p.SubType = pp.SubType;
                //p.Include = target + pp.Include.Replace(prefix, "");
                //if (p.Include.StartsWith("\\") == true)
                //    p.Include = p.Include.Remove(0, 1);

                string name = pp.Include;

                //if (ops == opers.folder)
                //{
                //    if (name.EndsWith("\\") == false)
                //        name = name + "\\";
                //}

                T.Add(name);
            }

            return T;
        }

        public ArrayList GetTargetItems(ArrayList L, string prefix, string target)
        {
            ArrayList T = new ArrayList();

            foreach (VSProjectItem pp in L)
            {
                VSProjectItem p = new VSProjectItem();
                p.ItemType = pp.ItemType;
                p.SubType = pp.SubType;
                if (pp.Include == null)
                    p.Include = "";
                if (pp.Include != "")
                    if (prefix != "")
                        p.Include = target + pp.Include.Replace(prefix, "");

                    else p.Include = target + "\\" + pp.Include;

                if (p.Include != null)
                    if (p.Include.StartsWith("\\") == true)
                        p.Include = p.Include.Remove(0, 1);

                T.Add(p);
            }

            return T;
        }

        public ArrayList GetItems(TreeNode node)
        {
            ArrayList L = new ArrayList();

            VSProjectItem p = GetVSProjectItem(node);

            L.Add(p);

            foreach (TreeNode nodes in node.Nodes)
                GetItems(nodes, L);

            return L;
        }

        public void GetItems(TreeNode node, ArrayList L)
        {
            VSProjectItem p = GetVSProjectItem(node);

            L.Add(p);

            foreach (TreeNode nodes in node.Nodes)
                GetItems(nodes, L);
        }

        public VSProject GetVSProject(TreeNode node)
        {
            CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return null;
            VSProject pp = p.ps;

            return pp;
        }

        public VSProjectItem GetVSProjectItem(TreeNode node)
        {
            CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return null;
            VSProjectItem pp = p.psi;

            return pp;
        }

        public string sourceprefix(TreeNode node)
        {
            string prefix = "";

            VSProjectItem pp = GetVSProjectItem(node);

            string include = pp.Include;

            if (include != null)
            {
                string[] cc = include.Split("\\".ToCharArray());

                if (cc.Length == 1)
                    return prefix;

                string last = cc[cc.Length - 1];

                string name = include.Replace("\\" + last, "");

                prefix = name;
            }

            return prefix;
        }

        public static TreeNode nc { get; set; }

        public static TreeNode ncc { get; set; }

        private VSProject vp { get; set; }

        private TreeNode project_node { get; set; }

        public void LoadTreeNode(List<TreeNode> n)
        {
            TreeNode nn = n[0];

            vp = GetVSProject(nn);

            tv.Nodes.Clear();
            tv.FullRowSelect = true;
            tv.CheckBoxes = true;

            L = new ArrayList();

            foreach (TreeNode ns in n)
            {
                TreeNode ng = new TreeNode();
                ng.Text = ns.Text;
                ng.Checked = true;
                ng.Tag = ns.Tag;
                ng.ImageKey = ns.ImageKey;
                ng.SelectedImageKey = ns.ImageKey;
                tv.Nodes.Add(ng);

                nc = ng;

                textBox1.Text = ng.Text;

                foreach (TreeNode node in ns.Nodes)
                {
                    CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                    if (ps == null)
                        continue;

                    L.Add(ps);

                    TreeNode nodes = new TreeNode();
                    nodes.Text = node.Text;
                    nodes.Tag = ps;
                    nodes.Checked = true;
                    nodes.ImageKey = node.ImageKey;
                    nodes.SelectedImageKey = node.SelectedImageKey;
                    ng.Nodes.Add(nodes);


                    //L.Add(nodes);

                    LoadFromTreeNode(node, nodes);
                }

                project_node = ns;
            }
        }

        public ArrayList RenameItems(ArrayList L, string name)
        {
            ArrayList N = new ArrayList();

            CreateView_Solution.ProjectItemInfo pp = L[0] as CreateView_Solution.ProjectItemInfo;

            VSProjectItem b = pp.psi;

            foreach (CreateView_Solution.ProjectItemInfo p in L)
            {
            }

            return N;
        }

        public void LoadFromTreeNode(TreeNode ns, TreeNode ng)
        {
            foreach (TreeNode node in ns.Nodes)
            {
                CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                if (ps == null)
                    continue;

                L.Add(ps);

                TreeNode nodes = new TreeNode();
                nodes.Text = node.Text;
                nodes.Tag = ps;
                nodes.Checked = true;
                nodes.SelectedImageKey = node.SelectedImageKey;
                nodes.ImageKey = node.ImageKey;
                ng.Nodes.Add(nodes);

                LoadFromTreeNode(node, nodes);
            }
        }

        public void LoadFromTreeNode(TreeNode ns, TreeNode ng, string name, string newname)
        {
            foreach (TreeNode node in ns.Nodes)
            {
                CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                CreateView_Solution.ProjectItemInfo bb = new CreateView_Solution.ProjectItemInfo();
                bb.vs = ps.vs;
                bb.ps = ps.ps;
                VSProjectItem pc = new VSProjectItem();
                pc.SubType = ps.psi.SubType;
                pc.ItemType = ps.psi.ItemType;
                pc.Include = ps.psi.Include.Replace(name, newname);
                bb.psi = pc;

                //if (ps == null)
                //    continue;

                L.Add(ps);

                TreeNode nodes = new TreeNode();
                nodes.Text = node.Text;
                nodes.Tag = bb;
                nodes.Checked = true;
                nodes.SelectedImageKey = node.SelectedImageKey;
                nodes.ImageKey = node.ImageKey;
                ng.Nodes.Add(nodes);

                LoadFromTreeNode(node, nodes, name, newname);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (sp.Panel2Collapsed == true)
                sp.Panel2Collapsed = false;
            else sp.Panel2Collapsed = true;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = tv.SelectedNode;
            if (node == null)
                return;
            CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo; ;

            if (p == null)
                return;

            pg.SelectedObject = p.psi;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            RenameVSProjectItems();
        }

        private void ClipboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tv == null)
                return;
            if (nc == null)
                return;

            tv.Nodes.Remove(nc);
        }
        public string GetName(string c)
        {
            string name = "";

            string[] cc = c.Split("\\".ToCharArray());

            string bb = cc[cc.Length - 1];

            string[] g = bb.Split(".".ToCharArray());

            name = g[0];

            return name;
        }
        public string GetString(string[] cc)
        {
            string name = "";
            int d = cc.Length - 2;
            int i = 0;
            foreach (string c in cc)
            {
                if (i == d)
                    name = name + c + ".";
                else
                    name = name + c + "\\";
                i++;
            }
            name = name.Remove(name.Length - 1);

            return name;
        }

        public void RenameSubFolders(TreeNode nc, string master, string newname, TreeNode ng)
        {
            foreach (TreeNode node in nc.Nodes)
            {
                CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                VSProjectItem pp = null;

                if (ps != null)
                    pp = ps.psi;

                CreateView_Solution.ProjectItemInfo bb = new CreateView_Solution.ProjectItemInfo();
                bb.vs = ps.vs;
                bb.ps = ps.ps;

                VSProjectItem pc = new VSProjectItem();
                pc.SubType = ps.psi.SubType;
                pc.ItemType = ps.psi.ItemType;
                if (ps.psi.ItemType == null)
                    pc.ItemType = ps.psi.SubType;
                bb.psi = pc;

                string inc = ps.psi.Include;

                //string[] d = inc.Split("\\.".ToCharArray());

                inc = inc.Replace(master, newname);

                pc.Include = inc;

                if (vp != null)
                {
                    vp.RenameItem(pc.ItemType, ps.psi.Include, pc.Include, master, newname);
                }

                node.Text = inc;

                TreeNode nodes = new TreeNode();
                //nodes.Text = node.Text;
                nodes.Text = node.Text.Replace(newname, "");
                nodes.SelectedImageKey = node.SelectedImageKey;
                nodes.ImageKey = node.ImageKey;

                nodes.Checked = true;
                ng.Nodes.Add(nodes);

                if (ps != null)
                    if (bb != null)
                    {
                        pc.Include = inc;
                        nodes.Tag = bb;
                    }

                // LoadFromTreeNode(node, nodes, master, newname);

                if (node.Nodes.Count > 0)
                {
                    RenameSubFolders(node, master, newname, nodes);
                }
            }
        }

        public void RenameFolder()
        {
            string name = textBox1.Text;


            tv.BeginUpdate();

            ncc = new TreeNode();



            ncc.Text = name;

            ncc.SelectedImageKey = nc.SelectedImageKey;
            ncc.ImageKey = nc.ImageKey;

            CreateView_Solution.ProjectItemInfo p = nc.Tag as CreateView_Solution.ProjectItemInfo;



            string folder = p.psi.Include;

            string newfolder = "";

            string[] dd = folder.Split("\\".ToCharArray());

            if (dd.Length > 0)
            {
                dd[dd.Length - 1] = name;

                newfolder = "";
                int a = 0;
                foreach (string g in dd)
                {
                    if (a != 0)
                        newfolder += "\\";
                    newfolder += g;
                    a++;
                }
            }

            vp.FolderCreate(newfolder);

            string newname = newfolder + "\\";
            string newnames = newfolder;

            string master = folder + "\\";
            string masters = folder + "\\";

            foreach (TreeNode node in nc.Nodes)
            {
                CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                VSProjectItem pp = null;

                if (ps != null)
                    pp = ps.psi;

                CreateView_Solution.ProjectItemInfo bb = new CreateView_Solution.ProjectItemInfo();
                bb.vs = ps.vs;
                bb.ps = ps.ps;

                VSProjectItem pc = new VSProjectItem();
                pc.SubType = ps.psi.SubType;
                pc.ItemType = ps.psi.ItemType;
                if (ps.psi.ItemType == null)
                    pc.ItemType = ps.psi.SubType;
                bb.psi = pc;

                string inc = ps.psi.Include;

                //string[] d = inc.Split("\\.".ToCharArray());

                inc = inc.Replace(master, newname);

                pc.Include = inc;

                if (vp != null)
                {
                    vp.RenameItem(pc.ItemType, ps.psi.Include, pc.Include, master, newname);
                }

                node.Text = inc;

                TreeNode nodes = new TreeNode();
                nodes.Text = node.Text.Replace(newname, "");
                nodes.SelectedImageKey = node.SelectedImageKey;
                nodes.ImageKey = node.ImageKey;

                nodes.Checked = true;

                ncc.Nodes.Add(nodes);

                if (ps != null)
                    if (bb != null)
                    {
                        pc.Include = inc;
                        nodes.Tag = bb;
                    }

                //LoadFromTreeNode(node, nodes, master, newname);

                if (node.Nodes.Count > 0)
                {
                    RenameSubFolders(node, master, newname, nodes);
                }
            }

            //LoadFromTreeNode(node, nodes, master, newname);



            LoadTreeNodes(ncc);

            tv.EndUpdate();
        }


        public void RenameVSProjectItems()
        {
            string name = textBox1.Text;

            ncc = new TreeNode();

            ncc.Text = name;

            if (project_node != null)
                project_node.Text = name;

            CreateView_Solution.ProjectItemInfo p = nc.Tag as CreateView_Solution.ProjectItemInfo;

            //CreateView_Solution.ProjectItemInfo psp = new CreateView_Solution.ProjectItemInfo();

            if (p.psi != null)
                if (p.psi.ItemType == "Folder")
                {
                    RenameFolder();
                    return;
                }

            VSProjectItem ppp = null;

            if (p != null)
                ppp = p.psi;

            CreateView_Solution.ProjectItemInfo bbb = new CreateView_Solution.ProjectItemInfo();
            bbb.vs = p.vs;
            bbb.ps = p.ps;

            VSProjectItem pcp = new VSProjectItem();
            pcp.SubType = p.psi.SubType;
            pcp.ItemType = p.psi.ItemType;
            bbb.psi = pcp;

            ncc.Tag = bbb;

            if (p == null)
                return;
            if (p.psi == null)
                return;

            string names = p.psi.Include;

            //string[] cc = names.Split("\\".ToCharArray());

            string master;// = GetString(cc);

            // master = cc[cc.Length - 2];

            CreateView_Solution.ProjectItemInfo psp = nc.Tag as CreateView_Solution.ProjectItemInfo;

            string newname = name;

            string incc = p.psi.Include;

            if (psp.psi.Name != null)
            {
                master = GetName(psp.psi.Name);


                newname = name;



                pcp.Include = incc;




                incc = incc.Replace(master, newname);

                pcp.Include = incc;

                if (vp != null)
                {
                    vp.RenameItem(pcp.ItemType, psp.psi.Include, pcp.Include);
                }
            }
            else

                master = GetName(nc.Text);

            foreach (TreeNode node in nc.Nodes)
            {
                CreateView_Solution.ProjectItemInfo ps = node.Tag as CreateView_Solution.ProjectItemInfo;

                VSProjectItem pp = null;

                if (ps != null)
                    pp = ps.psi;

                CreateView_Solution.ProjectItemInfo bb = new CreateView_Solution.ProjectItemInfo();
                bb.vs = ps.vs;
                bb.ps = ps.ps;

                VSProjectItem pc = new VSProjectItem();
                pc.SubType = ps.psi.SubType;
                pc.ItemType = ps.psi.ItemType;
                if (ps.psi.ItemType == null)
                    pc.ItemType = ps.psi.SubType;
                bb.psi = pc;

                string inc = ps.psi.Include;

                //string names = p.psi.Include;

                string[] d = inc.Split("\\.".ToCharArray());

                //master = inc;// GetString(d);

                //if(d.Length > 1)
                //d[d.Length - 2] = name;

                //newname = GetString(d);

                newname = name;

                inc = inc.Replace(master, newname);

                pc.Include = inc;

                if (vp != null)
                {
                    vp.RenameItem(pc.ItemType, ps.psi.Include, pc.Include, master, newname);
                }

                node.Text = inc;

                TreeNode nodes = new TreeNode();
                nodes.Text = node.Text;
                nodes.SelectedImageKey = node.SelectedImageKey;
                nodes.ImageKey = node.ImageKey;

                nodes.Checked = true;
                ncc.Nodes.Add(nodes);

                if (ps != null)
                    if (bb != null)
                    {
                        pc.Include = inc;
                        nodes.Tag = bb;
                    }

                LoadFromTreeNode(node, nodes, master, newname);
            }

            LoadTreeNodes(ncc);
        }

        public void LoadTreeNodes(TreeNode node)
        {
            if (node != null)
                if (node.Parent != null)
                    node.Parent.Nodes.Remove(node);

            tv.Nodes.Clear();
            tv.FullRowSelect = true;
            tv.CheckBoxes = true;

            if (node != null)
                tv.Nodes.Add(node);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            LoadTreeNodes(nc);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            LoadTreeNodes(ncc);
        }

        public opers GetSourceType(TreeNode node)
        {
            VSProjectItem p = GetVSProjectItem(node);



            if (p == null)
                return opers.unknown;

            if (p.SubType == "Form")
                return opers.forms;

            if (p.ItemType == "Forms")
                return opers.forms;
            if (p.ItemType == "Folder")
                return opers.folder;
            else if (p.ItemType == "Compile")
                return opers.file;
            else if (p.ItemType == "File")
                return opers.file;
            else if (p.ItemType == "Project")
                return opers.project;

            return opers.unknown;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
        }
    }

    public enum copies { copy, paste, cut };

    public enum opers { folder, file, forms, project, unknown }
}