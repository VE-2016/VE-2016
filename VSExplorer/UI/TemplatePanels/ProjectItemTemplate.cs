using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class ProjectItemTemplate : UserControl
    {
        public ProjectItemTemplate()
        {
            InitializeComponent();
            tvs = treeView1;
        }

        private TreeViews tvs
        {
            get;
            set;
        }

        public VSProject pp { get; set; }

        public bool ProjectLoaded()
        {
            return true;
        }

        public void LoadProjectItems(VSProject p)
        {
            if (p == pp)
                return;

            pp = p;

            tvs.CheckBoxes = true;
            tvs.Nodes.Clear();

            ArrayList L = p.GetCompileItems();

            foreach (string s in L)
            {
                string name = Path.GetFileName(s);
                TreeNode node = new TreeNode();
                node.Text = name;
                node.Tag = s;

                tvs.Nodes.Add(node);
            }
        }

        public void MakeItemTemplate()
        {
            if (pp == null)
                return;

            ArrayList L = GetSelectedItems();

            SaveItemTemplate("", L);
        }

        public ArrayList GetSelectedItems()
        {
            ArrayList L = new ArrayList();

            foreach (TreeNode node in tvs.Nodes)
            {
                string file = node.Tag as string;

                L.Add(file);
            }

            return L;
        }

        private string _loc = "ProjectItemTemplate";

        public void SaveItemTemplate(string folder, ArrayList L)
        {
            folder = pp.Name;

            string folders = AppDomain.CurrentDomain.BaseDirectory;

            string target = folders + _loc + "\\" + folder;

            string source = Path.GetDirectoryName(pp.FileName);

            if (Directory.Exists(target))
                return;
            Directory.CreateDirectory(target);

            foreach (string s in L)
            {
                string name = Path.GetFileName(s);
                File.Copy(s, target + "\\" + name);
            }
        }
    }
}