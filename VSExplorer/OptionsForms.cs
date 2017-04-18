using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.Scripting.ScriptControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class OptionsForms : Form
    {
        public OptionsForms()
        {
            InitializeComponent();
            tev = treeView1;
            settings = CodeEditorControl.settings;

            sp = splitContainer1;

            tc = tabControl1;

            tr = treeView1;

            propertyGrid1.SelectedObject = settings;

            SetConfigView();

            LoadTreeView();

            tr.AfterSelect += Tr_AfterSelect;

            Load_Page(panel6);

            LoadFontsColors();

            fntb = comboBox5;
            fntb.DrawMode = DrawMode.OwnerDrawFixed;
            fntc = comboBox6;

            fntb.DrawItem += Fntb_DrawItem;

            LoadNuget();
        }

        private void LoadNuget()
        {
            listView2.Resize += ListView2_Resize;
            listView2.CheckBoxes = true;
            listView2.View = View.Details;
            listView2.Columns.Add("");
            listView2.HeaderStyle = ColumnHeaderStyle.None;
            ListViewItem v = new ListViewItem("http:\\nuget.org");
            v.Checked = true;
            listView2.Items.Add(v);
            ListView2_Resize(null, null);
        }

        private void ListView2_Resize(object sender, EventArgs e)
        {
            listView2.Columns[0].Width = listView2.Width - 3;
        }

        private ArrayList C { get; set; }

        private void Fntb_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            string text = cb.Items[e.Index].ToString();
            if (e.Index < 0)
                return;
            int i = e.Index;
            //cb.ItemHeight = 12;

            e.DrawBackground();

            e.DrawFocusRectangle();

            //Rectangle Bounds = new Rectangle(40, 1, 10, 10);

            //To Draw the image in comboBox
            SolidBrush s = new SolidBrush(Color.White);
            if (C != null)
                if (C.Count > i)
                    s = new SolidBrush((Color)C[i]);

            Rectangle bounds = e.Bounds;
            bounds.Width = 14;

            e.Graphics.FillRectangle(s, bounds);

            // draw the text
            using (Brush textBrush = new SolidBrush(e.ForeColor))
            {
                bounds = e.Bounds;

                bounds.X += 15;
                // adjust the bounds so that the text is centered properly.

                // Draw the string vertically centered but on the left
                using (StringFormat format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Near;

                    e.Graphics.DrawString(cb.Items[i].ToString(), cb.Font, textBrush, bounds, format);
                }

                //s.Dispose();
            }
        }

        private ExplorerForms ef { get; set; }

        private ComboBox fntb { get; set; }
        private ComboBox fntc { get; set; }

        private void Tr_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            string text = node.Text;

            if (text == "General")
            {
                Load_Page(panel6);
            }
            if (text == "AutoRecover")
            {
                Load_Autorecover();
            }
            else if (text == "Documents")
            {
                Load_Documents();
            }
            else if (text == "Fonts and Colors")
            {
                Load_Page(panel5);
            }
            else if (text == "Direct Editor tools")
            {
                Load_Page(panel2);
            }
            else if (text == "Startup")
            {
                Load_Page(panel7);
            }
            else if (text == "Task List")
            {
                Load_Page(panel8);
            }
            else if (text == "Web Browser")
            {
                Load_Page(panel9);
            }
            else if (text == "General - projects and solutions")
            {
                Load_Page(panel10);
            }
            else if (text == "General ")
            {
                Load_Page(panel11);
            }
            else if (text == "Package sources")
            {
                Load_Page(panel12);
            }
        }

        public void Load_Autorecover()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(panel3);
            sp.Location = new Point(0, 0);
            ResumeLayout();
        }

        public void Load_Documents()
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(panel4);
            sp.Location = new Point(0, 0);
            ResumeLayout();
        }

        public void Load_Page(Panel panel)
        {
            SuspendLayout();

            sp.Panel2.Controls.Clear();

            sp.Panel2.Controls.Add(panel);
            sp.Location = new Point(0, 0);
            ResumeLayout();
        }

        public SplitContainer sp { get; set; }

        public TreeView tev { get; set; }

        public ScriptControl scr { get; set; }

        private TabControl tc { get; set; }

        private TreeView tr { get; set; }

        public void LoadTreeView()
        {
            TreeNode node = new TreeNode();
            node.Text = "Environment";
            tr.Nodes.Add(node);

            TreeNode nodes = new TreeNode();
            nodes.Text = "General";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "AutoRecover";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Documents";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Extensions and Updates";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Find and Replace";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Fonts and Colors";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Import and Export Settings";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "International Settings";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Startup";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Task List";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Web Browser";
            node.Nodes.Add(nodes);

            node = new TreeNode();
            node.Text = "Project and Solutions";
            tr.Nodes.Add(node);

            nodes = new TreeNode();
            nodes.Text = "General - projects and solutions";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Build and Run";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "DNX projects";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "External Web Tools";
            node.Nodes.Add(nodes);

            node = new TreeNode();
            node.Text = "Text Editor";
            tr.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "Database tools";
            tr.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "GitHub for Visual Explorer";
            tr.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "NuGet Package Manager";
            tr.Nodes.Add(node);

            nodes = new TreeNode();
            nodes.Text = "General ";
            node.Nodes.Add(nodes);

            nodes = new TreeNode();
            nodes.Text = "Package sources";
            node.Nodes.Add(nodes);

            //node = new TreeNode();
            //node.Text = "Project and Solutions";
            //tr.Nodes.Add(node);

            //nodes = new TreeNode();
            //nodes.Text = "General - projects and solutions";
            //node.Nodes.Add(nodes);

            node = new TreeNode();
            node.Text = "Direct Editor tools";
            tr.Nodes.Add(node);

            tr.ExpandAll();
        }

        public void SetConfigView()
        {
            SuspendLayout();

            sp.Panel2.Controls.Remove(tc);

            sp.Panel2.Controls.Add(panel2);
            sp.Location = new Point(0, 0);
            ResumeLayout();
        }

        public Settings settings { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            //settings.ShowLineNumbers = checkBox3.Checked;

            scr.ReloadSettings(true, settings);

            if (checkBox4.Checked == true)
            {
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            scr.ReloadSettings(true, settings);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = comboBox4;

            //if(cb.Text == "Light")
            //    ef.dock.Theme = new VS2012LightTheme();
            //else
            //    if (cb.Text == "Blue")
            //    ef.dock.Theme = new VS2013BlueTheme();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public static Color KnownColorToColor(KnownColor r)
        {
            Color c = Color.FromKnownColor(r);
            return c;
        }

        public ArrayList LoadColors()
        {
            List<string> colors = new List<string>();

            ArrayList L = new ArrayList();

            foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
            {
                //cast the colorName into a KnownColor
                KnownColor knownColor = (KnownColor)Enum.Parse(typeof(KnownColor), colorName);
                //check if the knownColor variable is a System color
                if (knownColor > KnownColor.Transparent)
                {
                    //add it to our list
                    colors.Add(colorName);
                    Color c = KnownColorToColor(knownColor);
                    L.Add(c);
                }
            }
            return L;
        }

        public void LoadFontsColors()
        {
            C = LoadColors();

            ComboBox cb = comboBox5;
            cb.Items.Clear();
            foreach (Color c in C)
            {
                cb.Items.Add(c.Name);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadColorDialog();
        }

        private ColorDialog ccd { get; set; }

        public void LoadColorDialog()
        {
            ccd = new ColorDialog();
            DialogResult r = ccd.ShowDialog();
            if (r == DialogResult.OK)
                return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 0)
                CodeEditorControl.settings.Theme = "VS2012Light";
            else
                CodeEditorControl.settings.Theme = "VS2013Blue";

            Close();
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox7.SelectedIndex;
            if (i < 0)
                return;
            if (i == 1)
            {
                CodeEditorControl.settings.ResumeAtStartup = "yes";
            }
            else

                CodeEditorControl.settings.ResumeAtStartup = "no";
        }

        private void tabPage9_Click(object sender, EventArgs e)
        {
        }
    }
}