//using RoughsExtensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class FolderBrowser : UserControl
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        //public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

        public const uint SHGFI_OPENICON = 0x000000002;

        //public const uint SHGFI_SMALLICON = 0x000000001;
        //public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public FolderBrowser()
        {
            InitializeComponent();
            ts = toolStrip1;
            lb = listView1;
            v = treeView1;
            v.AfterSelect += V_AfterSelect;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            rightClickMenuStrip = contextMenuStrip1;
            tb = new TextBox();
            open = toolStripButton1;
            PrepareListView();
            GetDrives();
            LoadCurrentFolder();
        }

        private void V_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (v.SelectedNode == null)
                return;
            if (fd == null)
                return;
            string s = v.SelectedNode.Text;

            string[] g = s.Split("=".ToCharArray());

            if (g.Length > 1)
                s = g[1];

            if (Directory.Exists(s) == false)
                if (File.Exists(s) == false)
                    return;

            fd.LoadFolder(s);
        }

        public FolderLister fd { get; set; }

        private ToolStrip ts { get; set; }

        public ListView lb { get; set; }

        public TreeView v { get; set; }

        public ToolStripButton open { get; set; }

        private ContextMenuStrip rightClickMenuStrip { get; set; }

        public bool sendToFolders = true;

        public void PrepareListView()
        {
            lb.Columns.Add("", 100);

            lb.Dock = DockStyle.Fill;
            //lv.Items.Add(new ListViewItem(new string[] { "Alpha", "Some details" }));
            //lv.Items.Add(new ListViewItem(new string[] { "Bravo", "More details" }));
            lb.View = View.Details;
            lb.FullRowSelect = true;

            lb.SmallImageList = new ImageList();

            lb.HeaderStyle = ColumnHeaderStyle.None;
        }

        public void GetDrives()
        {
            string[] drives = System.IO.Directory.GetLogicalDrives();

            foreach (string str in drives)
            {
                System.Console.WriteLine(str);
                CreateButton(str);
            }
        }

        public void CreateButton(string name)
        {
            ToolStripButton b = new ToolStripButton();
            b.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            b.Image = resource_vsc.harddrive;
            b.Text = name;
            b.Click += new EventHandler(OnDriveSelect);
            b.Tag = name;

            ts.Items.Add(b);
        }

        public void OnDriveSelect(object sender, EventArgs e)
        {
            ToolStripButton b = sender as ToolStripButton;

            if (b == null)
                return;

            if (b.Tag == null)
                return;

            string p = b.Tag as string;

            //string[] filePaths = null;

            //try
            //{
            //    filePaths = Directory.GetFiles(@p);

            //}
            //catch(Exception ex)
            //{
            //    return;
            //}

            lb.Items.Clear();

            ListDirectoryContent(p);
        }

        public void LoadCurrentFolder()
        {
            lb.Items.Clear();

            string p = AppDomain.CurrentDomain.BaseDirectory;

            ListDirectoryContent(p);
        }

        public string pwd { get; set; }

        public void ListDirectoryContent(string path)
        {
            IntPtr hImgSmall; //the handle to the system image list
            IntPtr hImgLarge; //the handle to the system image list

            //label1.Text = path;
            listView1.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(path);

            // first list sub-directories
            DirectoryInfo[] dirs = null;

            try
            {
                dirs = di.GetDirectories();
            }
            catch (Exception ex)
            {
                return;
            }

            ListViewItem bb = new ListViewItem();
            bb.Text = "..";

            bb.Tag = di;
            lb.Items.Add(bb);

            foreach (DirectoryInfo dir in dirs)
            {
                if (lb.SmallImageList.Images.ContainsKey(".dir") == false)
                {
                    // Need to add size check, although errors generated at present!
                    uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | SHGFI_OPENICON;

                    SHFILEINFO shinfo = new SHFILEINFO();

                    var res = SHGetFileInfo(@dir.FullName, FILE_ATTRIBUTE_DIRECTORY, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);

                    // Load the icon from an HICON handle
                    Icon bc = Icon.FromHandle(shinfo.hIcon);

                    // Now clone the icon, so that it can be successfully stored in an ImageList
                    //var c = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();

                    Image bmp = bc.ToBitmap();

                    bc.Dispose();
                    DestroyIcon(shinfo.hIcon);        // Cleanup

                    //lb.SmallImageList.Images.Add(".dir", bmp);
                    lb.SmallImageList.Images.Add(".dir", resource_alls.Folder);
                }

                ListViewItem b = new ListViewItem();
                b.Text = dir.Name;
                b.ImageKey = ".dir";
                b.Tag = dir;

                lb.Items.Add(b);
            }
            // then list the files
            FileInfo[] files = di.GetFiles();
            foreach (FileInfo file in files)
            {
                if (lb.SmallImageList.Images.ContainsKey(file.Extension) == false)
                {
                    SHFILEINFO shinfo = new SHFILEINFO();

                    hImgSmall = SHGetFileInfo(file.FullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);

                    try
                    {
                        // Load the icon from an HICON handle
                        Icon bc = Icon.FromHandle(shinfo.hIcon);

                        //The icon is returned in the hIcon member of the shinfo struct
                        //var c = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();

                        Image bmp = bc.ToBitmap();

                        bc.Dispose();
                        DestroyIcon(shinfo.hIcon);

                        lb.SmallImageList.Images.Add(file.Extension, bmp);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                ListViewItem b = new ListViewItem();
                b.Text = file.Name;
                b.ImageKey = file.Extension;
                b.Tag = file;

                lb.Items.Add(b);
            }

            pwd = path;
        }

        public void Refreshes()
        {
            ListDirectoryContent(pwd);
        }

        public TextBox tb { get; set; }

        public PictureBox pb { get; set; }

        public bool filenameonly = false;

        public bool LoadByClick = false;

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LoadByClick == true)
            {
                listView1_DoubleClick(this, null);
            }

            if (lb.SelectedIndices == null)
                return;
            if (lb.SelectedIndices.Count <= 0)
                return;
            int i = lb.SelectedIndices[0];

            ListViewItem v = lb.Items[i];

            if (v.Tag == null)
                return;

            DirectoryInfo d = v.Tag as DirectoryInfo;

            if (d != null)
            {
                if (tb == null)
                    return;

                string s = d.FullName;

                tb.Text = s;

                return;
            }

            FileInfo f = v.Tag as FileInfo;

            if (f != null)
            {
                if (tb == null)
                    return;

                string s = f.FullName;

                if (filenameonly == true)
                {
                    s = Path.GetFileName(s);

                    tb.ForeColor = Color.DarkBlue;
                }

                tb.Text = s;

                if (pb == null)
                    return;

                string ext = f.Extension;

                if (ext == ".bmp" || ext == ".png" || ext == "jpeg" || ext == "jpg")
                {
                    Image image = Image.FromFile(s);

                    pb.Image = new Bitmap(image);

                    bmp = image;
                }

                return;
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (lb.SelectedIndices == null)
                return;
            if (lb.SelectedIndices.Count <= 0)
                return;
            int i = lb.SelectedIndices[0];

            ListViewItem v = lb.Items[i];

            if (v.Text == "..")
            {
                DirectoryInfo d = v.Tag as DirectoryInfo;

                if (d == null)
                    return;

                DirectoryInfo p = d.Parent;

                if (p == null)
                    return;

                ListDirectoryContent(p.FullName);

                return;
            }
            else
            {
                DirectoryInfo d = v.Tag as DirectoryInfo;

                if (d == null)
                    return;

                DirectoryInfo p = d;

                ListDirectoryContent(p.FullName);
            }
        }

        private void listView1_Resize(object sender, EventArgs e)
        {
            Refresh();

            if (lb == null)
                return;

            Size sz = lb.ClientSize;

            int w = sz.Width;

            lb.Columns[0].Width = w;
        }

        public static string Truncate(string text, int maxLength)
        {
            return text.Substring(0, Math.Min(maxLength, text.Length));
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        rightClickMenuStrip.Show(this, new Point(e.X, e.Y));
                    }
                    break;
            }
        }

        public string SelectedFile()
        {
            ListView vv = listView1;

            if (vv.SelectedIndices == null)
                return "";

            if (vv.SelectedIndices.Count <= 0)
                return "";

            int i = vv.SelectedIndices[0];

            ListViewItem v = vv.Items[i];

            if (v.Text == "..")
            {
                return "";
            }
            else
            {
                DirectoryInfo d = v.Tag as DirectoryInfo;

                if (d != null)

                    return d.FullName;

                FileInfo f = v.Tag as FileInfo;

                if (f == null)
                    return "";

                return f.FullName;
            }
        }

        public Image bmp { get; set; }

        private void importToWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (ws == null)
            //    return;
            //if (bmp == null)
            //    return;
            //ws.Import2Workspace(bmp);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }

        public void OpenSolution()
        {
        }

        public Dictionary<string, ArrayList> dict { get; set; }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (v.Visible == true)
            {
                lb.Visible = true;
                v.Visible = false;
            }
            else
            {
                v.Visible = true;
                lb.Visible = false;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (File.Exists("tmp.tmp") == false)
                return;

            string content = File.ReadAllText("tmp.tmp");

            string[] cc = content.Split("\n".ToCharArray());

            v.Nodes.Clear();

            foreach (string c in cc)
            {
                string[] dd = c.Split(";".ToCharArray());

                if (dd.Length == 1)
                {
                    TreeNode node = new TreeNode();

                    node.Text = c.Replace("\r", "");

                    v.Nodes.Add(node);
                }
                else
                {
                    string[] g = dd[0].Split("=".ToCharArray());

                    if (g.Length == 1)
                        dd[0] = dd[0];
                    else
                        dd[0] = g[0];

                    TreeNode nodes = new TreeNode();
                    nodes.Text = dd[0];
                    v.Nodes.Add(nodes);

                    if (g.Length > 1)
                        dd[0] = g[1];

                    foreach (string d in dd)
                    {
                        TreeNode node = new TreeNode();
                        node.Text = d;
                        nodes.Nodes.Add(node);
                    }
                }
            }
        }
    }
}