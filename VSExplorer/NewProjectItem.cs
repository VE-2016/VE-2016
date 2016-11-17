using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class NewProjectItem : Form
    {
        public NewProjectItem()
        {
            InitializeComponent();
            tv = treeView2;
            tv.FullRowSelect = true;
            tv.ShowLines = false;
            lv = listView1;
            LoadProjects();
            LoadTemplates();
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
            Close();
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
            TreeNode node = new TreeNode();
            node.Text = "Installed";
            tv.Nodes.Add(node);

            TreeNode nodes = new TreeNode();
            nodes.Text = "Visual C# Items";
            node.Nodes.Add(nodes);

            TreeNode ns = new TreeNode();
            ns.Text = "Windows Form";
            nodes.Nodes.Add(ns);

            ns = new TreeNode();
            ns.Text = "C# class";
            nodes.Nodes.Add(ns);
        }

        private ImageList imgs { get; set; }

        public void InitializeListView(ListView lv)
        {
            lv.Clear();

            imgs = new ImageList();
            imgs.Images.Add("Form_Win32", resource_vsc.Formtag_5766_32);

            listView1.View = View.Details;
            this.imgs.ImageSize = new Size(32, 32);
            listView1.SmallImageList = this.imgs;

            lv.CheckBoxes = false;

            lv.FullRowSelect = true;

            

            lv.Columns.Add("Name");
            lv.Columns.Add("Description");
        }

        static public string folders = "TemplateProjectItems";

        public void LoadTemplates()
        {
            InitializeListView(lv);

            DirectoryInfo d = new DirectoryInfo(folders);

            if (d == null)
                return;

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                ListViewItem v = new ListViewItem();
                v.Text = s.Name;

                v.SubItems.Add(s.FullName);

                v.ImageIndex = 0;

                if (s.FullName.EndsWith("WindowsForm"))
                    v.Tag = "WindowsForm";
                else if (s.FullName.EndsWith("UserControl"))
                    v.Tag = "UserControl";
                else if (s.FullName.EndsWith("Component"))
                    v.Tag = "Component";
                else if (s.FullName.EndsWith("Resources"))
                    v.Tag = "Resource";

                lv.Items.Add(v);
            }

            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        static public Dictionary<string, string> Templates()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            DirectoryInfo d = new DirectoryInfo(folders);

            if (d == null)
                return dict;

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                string v = "";
                v  = s.Name;

                v += "$" + s.FullName;

                string tag = "";
                if (s.FullName.EndsWith("WindowsForm"))
                    tag = "WindowsForm";
                else if (s.FullName.EndsWith("UserControl"))
                    tag = "UserControl";
                else if (s.FullName.EndsWith("Component"))
                    tag = "Component";
                else if (s.FullName.EndsWith("Resources"))
                    tag = "Resource";

                v += "$" + tag;

                dict.Add(s.Name, v);
            }
            return dict;
        }

        static public ArrayList ProjectFiles(string name, Dictionary<string, string> dict)
        {
            ArrayList L = new ArrayList();

            foreach (string s in dict.Keys)
            {
                if (s != name)
                    continue;

                string[] cc = dict[s].Split("$".ToCharArray());

                string p = cc[1];

                DirectoryInfo d = new DirectoryInfo(p);

                FileInfo[] f = d.GetFiles();

                foreach (FileInfo g in f)
                    L.Add(g.Name);

                return L;
            }

            return L;
        }
        static public string ProjectFolder(string name, Dictionary<string, string> dict)
        {
            

            foreach (string s in dict.Keys)
            {
                if (s != name)
                    continue;

                string[] cc = dict[s].Split("$".ToCharArray());

                string p = cc[1];

                return p;
            }

            return "";
        }
        static public string GetSubType(string name, Dictionary<string, string> dict)
        {
            

            foreach (string s in dict.Keys)
            {
                if (s != name)
                    continue;

                string[] cc = dict[s].Split("$".ToCharArray());

                if (cc.Length <= 2)
                    return "";

                string p = cc[2];

                return p;
            }

            return "";
        }
        public ArrayList GetProjectFiles(string name)
        {
            ArrayList L = new ArrayList();

            foreach (ListViewItem v in lv.Items)
            {
                if (v.SubItems[0].Text != name)
                    continue;

                string s = v.SubItems[1].Text;

                DirectoryInfo d = new DirectoryInfo(s);

                FileInfo[] f = d.GetFiles();

                foreach (FileInfo g in f)
                    L.Add(g.Name);

                return L;
            }

            return L;
        }

        public ArrayList GetProjectFiles()
        {
            ArrayList L = new ArrayList();

            int i = GetSelectedIndex(lv);
            if (i < 0)
                return L;

            ListViewItem v = lv.Items[i];

            string s = v.SubItems[1].Text;

            if (Directory.Exists(s) == true)
            {
                DirectoryInfo d = new DirectoryInfo(s);

                FileInfo[] f = d.GetFiles();

                foreach (FileInfo g in f)
                    L.Add(g.Name);
            }
            else L.Add(s);

            return L;
        }

        public static string GetFormTemplate()
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            b = b + folders + "\\" + "WindowsForm";

            return b;
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

        public string GetProjectFolder(string name)
        {
            string names = "";

            foreach (ListViewItem v in lv.Items)
            {
                if (v.SubItems[0].Text != name)
                    continue;

                string s = v.SubItems[1].Text;

                return s;
            }

            return names;
        }

        public string GetSubType()
        {
            int i = GetSelectedIndex(lv);
            if (i < 0)
                return "";

            ListViewItem v = lv.Items[i];

            return v.Tag as string;
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

            string s = v.SubItems[1].ToString();

            s = s.Replace("{", "");

            s = s.Replace("}", "");

            string[] cc = s.Split("\\".ToCharArray());

            textBox1.Text = cc[cc.Length - 1];
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
    }
}