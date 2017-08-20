using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//using AndersLiu.Reflector.Program.UI.AssemblyTreeNode;

using VSProvider;

namespace AIMS.Libraries.CodeEditor
{
    public partial class TypeForm : Form
    {
        public TypeForm()
        {
            InitializeComponent();

            tw = treeView1;

            lvs = listView1;
        }

        public string SelectedType { get; set; }

        public TreeView tw { get; set; }

        public TypeBuilder b { get; set; }

        public ListView lvs { get; set; }

        public void SelectType(string s)
        {
            textBox1.Text = s;

            SelectedType = s;
        }

        public TreeNode GetNamespace(string s)
        {
            TreeNode nodes = null;

            foreach (TreeNode node in tw.Nodes)
            {
                if (node.Text == "CustomControls")
                    nodes = node;
            }

            if (nodes == null)
                return null;

            foreach (TreeNode node in nodes.Nodes)
            {
                if (node.Text == "ProjectReferences")
                    nodes = node;
            }

            if (nodes == null)
                return null;

            foreach (TreeNode node in nodes.Nodes)
            {
                TreeNode ns = GetNode(node, s);

                if (ns != null)
                    return ns;
            }

            return null;
        }


        public TreeNode GetNode(TreeView tw, string text)
        {
            foreach (TreeNode nodes in tw.Nodes)
            {
                TreeNode ns = GetNode(nodes, text);

                if (ns != null)
                    return ns;
            }

            return null;
        }


        private TreeNode GetNode(TreeNode node, string text)
        {
            if (node.Text == text)
                return node;

            foreach (TreeNode nodes in node.Nodes)
            {
                TreeNode ns = GetNode(nodes, text);

                if (ns != null)
                    return ns;
            }

            return null;
        }

        public TreeNode GetReferenceNode()
        {
            TreeNode nodes = null;

            foreach (TreeNode node in tw.Nodes)
            {
                if (node.Text == "CustomControls")
                    nodes = node;
            }

            if (nodes == null)
                return null;

            foreach (TreeNode node in nodes.Nodes)
            {
                if (node.Text == "ProjectReferences")
                    return node;
            }

            return null;
        }

        public string GetBaseType(string s)
        {
            SelectType(s);

            string bs = b.FindBaseType(s);

            return bs;
        }

        public ClassMapper GetBaseTypes(string s)
        {
            string c = "";

            if (b.dict.ContainsKey(s) == true)
                c = b.dict[s];
            else if (b.maps.ContainsKey(s) == false)
            {
                return null;
            }
            else c = s;


            if (b.maps.ContainsKey(c) == false)
                return null;

            ClassMapper m = b.maps[c] as ClassMapper;

            return m;
        }

        public void InitializeListView(ListView lv)
        {
            SuspendLayout();


            lv.Clear();

            lv.View = View.Details;


            lv.CheckBoxes = true;

            lv.FullRowSelect = true;



            lv.Columns.Add("Type");
            lv.Columns.Add("Class Mapper");


            ResumeLayout();
        }


        public Dictionary<string, ClassMapper> maps { get; set; }
        private Dictionary<string, string> dict { get; set; }

        private Dictionary<string, string> dlls { get; set; }

        public void LoadTypeMappings(Dictionary<string, string> d)
        {
        }


        public void LoadTypeMapping()
        {
            if (maps == null)
                return;

            InitializeListView(lvs);

            foreach (string key in maps.Keys)
            {
                ListViewItem v = new ListViewItem();
                v.Text = key;

                ClassMapper m = maps[key] as ClassMapper;

                v.SubItems.Add(m.Namespace);

                lvs.Items.Add(v);
            }

            CreateProjects(treeView2);
        }

        public TreeNode GetNode(string name, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Name.Equals(name)) return node;
                TreeNode next = GetNode(name, node);
                if (next != null) return next;
            }
            return null;
        }

        private TreeView tvs { get; set; }

        public void CreateProjects(TreeView tvs)
        {
            //tvs.ImageList = AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.CreateImageList();         


            foreach (string key in maps.Keys)
            {
                ListViewItem v = new ListViewItem();
                v.Text = key;

                ClassMapper m = maps[key] as ClassMapper;

                string project = Path.GetFileNameWithoutExtension(m.project);

                TreeNode[] ns = tvs.Nodes.Find(project, false);

                TreeNode node;

                if (ns == null || ns.Length <= 0)
                {
                    node = new TreeNode();
                    node.Text = project;
                    node.Name = project;

                    tvs.Nodes.Add(node);
                }
                else node = ns[0];

                string name = m.Namespace;

                TreeNode nodes;

                TreeNode[] ng = node.Nodes.Find(name, true);

                if (ng == null || ng.Length <= 0)
                {
                    nodes = new TreeNode();
                    nodes.Text = name;
                    nodes.Name = name;
                    node.Nodes.Add(nodes);
                    // node.ImageKey = NodeImages.Keys.AssemblyImage;

                    TreeNode b = new TreeNode();
                    b.Text = m.classname;
                    b.Name = m.classname;
                    //b.ImageKey = NodeImages.Keys.ClassImage;
                    b.Tag = m;


                    nodes.Nodes.Add(b);
                }
                else
                {
                    nodes = ng[0];

                    TreeNode b = new TreeNode();
                    b.Text = m.classname;
                    b.Name = m.classname;
                    //b.ImageKey = NodeImages.Keys.ClassImage;
                    b.Tag = m;

                    nodes.Nodes.Add(b);
                }
            }
        }
    }
}
