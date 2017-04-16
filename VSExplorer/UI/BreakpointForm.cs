using AIMS.Libraries.Scripting.ScriptControl;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class BreakpointForm : Form
    {
        public BreakpointForm()
        {
            InitializeComponent();
        

            Init();

            ScriptControl.br.breakPointEvent += Br_breakPointEvent;

            ScriptControl.br.breakPointStateEvent += Br_breakPointStateEvent;

            
        }

        private void Br_breakPointStateEvent(object sender, Breakpoint e)
        {
            editbox.BeginUpdate();
            editbox.Nodes.Clear();
            v.Items.Clear();
            foreach (Breakpoint b in ScriptControl.br.breakpoints.breakpoints)
                InsertBreakpoint(b);
            editbox.EndUpdate();
        }

        private void Br_breakPointEvent(object sender, Breakpoint e)
        {
            //MessageBox.Show("Breakpoint added at " + e.location.Y);

            //editbox.Nodes.Add(new TreeNode("Breakpoint added at " + e.location.Y));

            bool found = FindBreakpointNode(e);

            if (found)
                return;

            InsertBreakpoint(e);
        }

        public bool FindBreakpointNode(Breakpoint b)
        {

            foreach(TreeNode node in editbox.Nodes)
            {
                ListViewItem c = node.Tag as ListViewItem;

                Breakpoint bc = c.Tag as Breakpoint;

                if (bc.Equals(b))
                {

                    v.Items.Remove(c);
                    editbox.Nodes.Remove(node);
                    return true;


                }
            }
            return false;
            // 150, 58,70
        }

        public void InsertBreakpoint(Breakpoint b)
        {

            int i = editbox.Nodes.Count;

            TreeNode node = new TreeNode("breakpoint node " + i.ToString());
            //int j = 0;
            //while (j < 1)
            //{
            //    TreeNode nodes = new TreeNode("breakpoint line node " + j.ToString());
            //    nodes.Checked = true;
            //    nodes.ImageKey = "breakpoint";
            //    node.Nodes.Add(nodes);
            //    j++;
            //}
            editbox.Nodes.Add(node);

            ListViewItem c = new ListViewItem();
            c.Tag = b;
            node.Tag = c;
            node.Checked = b.enabled;

            foreach(ColumnHeader cc in v.Columns)
            {
                if(cc.Name == "Labels")
                {
                    c.SubItems.Add(b.label);
                }
                else if (cc.Name == "Condition")
                {
                    if (b.conditions == null)
                        c.SubItems.Add("(no condition)");
                    else c.SubItems.Add("conditions");
                }
                else if (cc.Name == "Hit Count")
                {
                    if (b.HitCount == 0)
                        c.SubItems.Add("break always");
                    else c.SubItems.Add("break");
                }
                else if (cc.Name == "File")
                {
                     if (b.file != "")
                        c.SubItems.Add(Path.GetFileName(b.file) + ", line " + b.location.Y + " character " + b.location.X);
                    else c.SubItems.Add("no data");
                }
                else if( cc.Name == "Address")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Data")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Process")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Function")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Language")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Filter")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "When Hit")
                {
                    c.SubItems.Add("");
                }
                
            }
            v.Items.Add(c);
        }

        public void LoadListViewItems(ListViewItem c)
        {
            v.BeginUpdate();
            c.SubItems.Clear();
            Breakpoint b = c.Tag as Breakpoint;
            c.Tag = b;
            
            foreach (ColumnHeader cc in v.Columns)
            {
                if (cc.Name == "Labels")
                {
                    c.SubItems.Add(b.label);
                }
                else if (cc.Name == "Condition")
                {
                    if (b.conditions == null)
                        c.SubItems.Add("(no condition)");
                    else c.SubItems.Add("conditions");
                }
                else if (cc.Name == "Hit Count")
                {
                    if (b.HitCount == 0)
                        c.SubItems.Add("break always");
                    else c.SubItems.Add("break");
                }
                else if (cc.Name == "File")
                {
                    if (b.file != "")
                        c.SubItems.Add(Path.GetFileName(b.file) + ", line " + b.location.Y + " character " + b.location.X);
                    else c.SubItems.Add("no data");
                }
                else if (cc.Name == "Address")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Data")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Process")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Function")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Language")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "Filter")
                {
                    c.SubItems.Add("");
                }
                else if (cc.Name == "When Hit")
                {
                    c.SubItems.Add("");
                }
                //else c.SubItems.Add("");
            }
            v.EndUpdate();
        }

    
        

        

        DoubleClickTreeView editbox { get; set; }


        ListView v { get; set; }

        ContextMenuStrip context { get; set; }
        public void Init()
        {
            context = contextMenuStrip1;

            v = listView1;

            ListView listView = listView1;

            // Initialize the ListView control.
            listView.BackColor = Color.White;
            listView.ForeColor = Color.Black;
            listView.Dock = DockStyle.Fill;
            listView.View = View.Details;
            listView.FullRowSelect = true;

            listView.HideSelection = true;

            // Add columns to the ListView control.
            //listView.Columns.Add("", 15, HorizontalAlignment.Center);
            listView.Columns.Add("Name", "Name", 400);
            listView.Columns.Add("Labels", "Labels",100);
            listView.Columns.Add("Condition", "Condition",100);
            listView.Columns.Add("Hit Count", "Hit Count", 100);
            
            CheckMenuItem("Name", true, false);
            CheckMenuItem("Labels", true);
            CheckMenuItem("Condition", true);
            CheckMenuItem("Hit Count", true);

            listView.GridLines = false;

            editbox = new DoubleClickTreeView();
            editbox.Parent = listView;
            editbox.Bounds = new Rectangle(0,0, 300, 300);
            editbox.Focus();
            editbox.Show();

            LoadTree();

            listView.Resize += ListView_Resize;

            listView.ColumnWidthChanged += ListView_ColumnWidthChanged;

            listView.ColumnWidthChanging += ListView_ColumnWidthChanging;

            listView.SelectedIndexChanged += ListView_SelectedIndexChanged;

            editbox.AfterCheck += Editbox_AfterCheck;

            editbox.NodeMouseClick += Editbox_NodeMouseClick;

            editbox.NodeMouseDoubleClick += Editbox_NodeMouseDoubleClick;
        }

        private void ListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (v.SelectedIndices == null || v.SelectedIndices.Count <= 0)
                return;
            int index = v.SelectedIndices[0];



            

            editbox.Focus();
            editbox.SelectedNode = editbox.Nodes[index];
        }

        private void Editbox_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            ListViewItem c = node.Tag as ListViewItem;
            Breakpoint b = c.Tag as Breakpoint;


            ExplorerForms.ef.Command_OpenFileAndGotoLine(b.file, b.location.Y.ToString());
        }

        private void ListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex > 0)
                return;

            Size s = listView1.Size;
            int w = e.NewWidth;
            editbox.Bounds = new Rectangle(0, 26, w, s.Height - 35);
        }

        private void ListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex > 0)
                return;
            ListView_Resize(null, null);
        }

        public Breakpoint GetBreakpoint(TreeNode node)
        {

            ListViewItem c = node.Tag as ListViewItem;
            if (c == null)
                return null;
            Breakpoint b = c.Tag as Breakpoint;
            return b;

        }

        private void Editbox_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            if (e.Button != MouseButtons.Right)
            {

                //node.Checked = !node.Checked;

                //TreeViewEventArgs ev = new TreeViewEventArgs(node);
                //
                //Editbox_AfterCheck(this, ev);
                return;
            }

            context.Show(editbox, node.Bounds.Right, node.Bounds.Top);
        }

        private void Editbox_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            int state = 4;

            Breakpoint b = null;

            b = (node.Tag as ListViewItem).Tag as Breakpoint;

            if (node.Checked == true)
            {

                node.ImageKey = "breakpoint";
                foreach (TreeNode nodes in node.Nodes)
                {
                    nodes.ImageKey = "breakpoint";
                    nodes.Checked = true;
                   
                }
                b.enabled = true;
            }
            else
            {
                node.ImageKey = "notbreakpoint";
                    foreach (TreeNode nodes in node.Nodes)
                    {
                        nodes.ImageKey = "notbreakpoint";
                        nodes.Checked = false;
                    
                    
                }
                state = 3;
                b.enabled = false;
            }

            ScriptControl.br.LoadBreakpoint(b, state, false);

        }

        private void ListView_Resize(object sender, System.EventArgs e)
        {
            Size s = listView1.Size;
            int w = listView1.Columns[0].Width;
            editbox.Bounds = new Rectangle(0, 24, w, s.Height - 28);
            editbox.BorderStyle = BorderStyle.None;
        }

        public void LoadTree()
        {
            editbox.CheckBoxes = true;
            editbox.Scrollable = false;
            

            ImageList g = new ImageList();
            g.Images.Add("breakpoint", Resources.BreakpointEnable_16x);
            g.Images.Add("notbreakpoint", Resources.BreakpointDisable_16x);
            editbox.ImageList = g;

            //int i = 0;
            //while(i < 5)
            //{
            //    TreeNode node = new TreeNode("breakpoint node " + i.ToString());
            //    int j = 0;
            //    while(j < 3)
            //    {
            //        TreeNode nodes = new TreeNode("breakpoint line node " + j.ToString());
            //        nodes.Checked = true;
            //        nodes.ImageKey = "breakpoint";        
            //        node.Nodes.Add(nodes);
            //        j++;
            //    }
            //    editbox.Nodes.Add(node);
            //    i++;
            //}

        }

        private void toolStripSplitButton2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem b = e.ClickedItem as ToolStripMenuItem;

            if (b.Checked == true)
                b.Checked = false;
            else b.Checked = true;

            LoadColumn(b.Text, b.Checked);


        }
        string[] names =
        {
            "All visible",
            "Name",
            "Labels",
            "Condition",
            "Hit Count",
            "Filter",
            "When Hit",
            "Language",
            "Function",
            "File",
            "Address",
            "Data",
            "Process"

        };

        public void LoadColumn(string name, bool Checked)
        {
            ListView v = listView1;

            if(name == "All visible")
            {


            }


            if(Checked == true)
            {
                foreach (ColumnHeader c in v.Columns)
                    if (c.Name == name)
                        return;

                v.Columns.Add(name, name).Name = name; 
                EnsureColumnOrder();
            }
            else
            {
                ColumnHeader cc = null;
                foreach (ColumnHeader c in v.Columns)
                    if (c.Name == name)
                    {
                        cc = c;
                        break;
                    }

                if (cc == null)
                    return;
                v.Columns.Remove(cc);
                EnsureColumnOrder();
            }
            foreach (ListViewItem c in v.Items)
                LoadListViewItems(c);
        }

        public void EnsureColumnOrder()
        {
            ListView listView = listView1;

            ArrayList L = new ArrayList();

            foreach(string s in names)
            {
                int index = listView.Columns.IndexOfKey(s);
                if (index >= 0)
                    L.Add(listView.Columns[index]);

            }

            listView.Columns.Clear();

            
            foreach(ColumnHeader c in L)
            listView.Columns.Add(c);
           
        }

        public void CheckMenuItem(string name, bool c, bool enabled = true)
        {
            ToolStripSplitButton b = toolStripSplitButton2;

            foreach(ToolStripMenuItem d in b.DropDownItems)
            {
                if(d.Text == name)
                {
                    d.Checked = c;
                    if (enabled == false)
                        d.Enabled = false;
                    return;
                }
            }
        }

        private void toolStripButton6_Click(object sender, System.EventArgs e)
        {
            TreeNode node = editbox.SelectedNode;
            if (node == null)
                return;
            Editbox_NodeMouseDoubleClick(this, new TreeNodeMouseClickEventArgs(node, MouseButtons.Left, 0,0,0));   
        }

        private void toolStripButton3_Click(object sender, System.EventArgs e)
        {
            ScriptControl.br.LoadBreakpoints(3, true);
        }

        private void toolStripButton2_Click(object sender, System.EventArgs e)
        {
            ScriptControl.br.LoadBreakpoints(4, true);
        }

        private void toolStripButton1_Click(object sender, System.EventArgs e)
        {
            TreeNode node = editbox.SelectedNode;
            if (node == null)
                return;
            Breakpoint b = GetBreakpoint(node);
            if (b == null)
                return;
            ScriptControl.br.LoadBreakpoint(b, 0, false);

            editbox.Nodes.Remove(node);
        }

        private void deleteToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            TreeNode node = editbox.SelectedNode;
            if (node == null)
                return;
            Breakpoint b = GetBreakpoint(node);
            if (b == null)
                return;
            ScriptControl.br.LoadBreakpoint(b, 0, false);

            editbox.Nodes.Remove(node);
        }

        private void toolStripButton4_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
                return;
            string file = ofd.FileName;

            string xml = ScriptControl.br.Serialize();

            File.WriteAllText(file, xml);
        }
    }

    public class DoubleClickTreeView : TreeView
    {
        public DoubleClickTreeView() : base()
        {
            // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardClick |
                ControlStyles.StandardDoubleClick, true);
        }
    }

}