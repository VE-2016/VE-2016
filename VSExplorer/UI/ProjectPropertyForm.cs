using GACProject;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer.UI
{
    public partial class ProjectPropertyForm : Form
    {
        public ProjectPropertyForm()
        {
            InitializeComponent();

            panel1.Paint += Panel1_Paint;

            label12.Visible = false;

            panel2.Paint += Panel2_Paint;

            label14.Visible = false;
            label18.Visible = false;
            label21.Visible = false;
            label25.Visible = false;
            label27.Visible = false;

            panel3.Paint += Panel3_Paint;
            label37.Visible = false;
            label40.Visible = false;
            label42.Visible = false;
            label46.Visible = false;

            panel5.Paint += Panel5_Paint;
            label53.Visible = false;

            panel8.Paint += Panel8_Paint;
            label75.Visible = false;

            panel10.Paint += Panel10_Paint;
            label81.Visible = false;

            ts = toolStrip1;

            ts.GripStyle = ToolStripGripStyle.Hidden;

            toolStripComboBox1.SelectedIndex = 1;

            panel4.Paint += Panel4_Paint;
            label48.Visible = false;

            comboBox1.SelectedIndex = 0;

            comboBox8.SelectedIndex = 0;

            dg = dataGridView1;

            img = new ImageList();

            lv = listView1;
            lv.Visible = false;

            InitListView();

            toggledg();

            // LoadResources();

            tabControl1.Selecting += TabControl1_Selecting;
        }

        private void TabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 13)
            {
                SaveProject();
                e.Cancel = true;
            }
        }

        public void SaveProject()
        {
            SetProperties();
        }

        private bool _notloaded = true;

        private OpenFileDialog ofd { get; set; }

        private void Panel10_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(label81, e);
        }

        private void Panel8_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(label75, e);
        }

        private void Panel5_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(label53, e);
        }

        private ImageList img { get; set; }

        private DataGridView dg { get; set; }

        private ListView lv { get; set; }

        private void Panel4_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(label48, e);
        }

        private ToolStrip ts { get; set; }

        private void Panel3_Paint(object sender, PaintEventArgs e)
        {
            DrawLine(label37, e);
            DrawLine(label40, e);
            DrawLine(label42, e);
            DrawLine(label46, e);
        }

        private void DrawLine(Label b, PaintEventArgs e)
        {
            var g = e.Graphics;

            Point cc = b.Location;

            Size sz = tabPage1.Size;

            g.DrawLine(Pens.Gray, cc.X + 2, cc.Y + 2, sz.Width - cc.X - 50, cc.Y + 2);
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            var g = e.Graphics;

            Point cc = label14.Location;

            Size sz = tabPage1.Size;

            g.DrawLine(Pens.Gray, cc.X + 2, cc.Y + 2, sz.Width - cc.X - 50, cc.Y + 2);

            DrawLine(label18, e);
            DrawLine(label21, e);
            DrawLine(label25, e);
            DrawLine(label27, e);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            var g = e.Graphics;

            Point cc = label12.Location;

            //Point s = label12.Size;

            Size sz = tabPage1.Size;

            g.DrawLine(Pens.Gray, cc.X + 2, cc.Y + 2, sz.Width - cc.X - 50, cc.Y + 2);
        }

        public Microsoft.Build.Evaluation.Project pc { get; set; }

        public VSProject vp { get; set; }

        public VSSolution vs { get; set; }

        private void tabControl1_DrawItem(Object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;

            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];

            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.White);
                g.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                g.FillRectangle(SystemBrushes.Control, _tabBounds/*e.Bounds*/);
                //e.DrawBackground();
            }

            // Use our own font.
            Font _tabFont = new Font("Arial", (float)10.0, FontStyle.Bold, GraphicsUnit.Pixel);

            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl1.TabCount - 1; i++)
            {
                if (tabControl1.GetTabRect(i).Contains(e.X, e.Y))
                {
                    tabControl1.SelectedIndex = i;
                }
            }
        }

        public void GetProperties()
        {
            if (pc == null)
                return;

            label3.Enabled = false;
            comboBox6.Text = "N/A";
            comboBox6.Enabled = false;

            label10.Enabled = false;
            comboBox7.Text = "N/A";
            comboBox7.Enabled = false;

            try
            {
                ArrayList L = new ArrayList();

                ArrayList Fs = GACForm.frameworks;

                if (Fs != null)
                {
                    foreach (DirectoryInfo d in Fs)
                        L.Add(".NET Framework " + d.Name);
                }
                else
                    L = VSParsers.frameworks.GetFrameworks();

                foreach (string s in L)
                {
                    comboBox4.Items.Add(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            };

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");
            Microsoft.Build.Evaluation.ProjectProperty rootassname = pc.GetProperty("RootNamespace");
            Microsoft.Build.Evaluation.ProjectProperty OutputPath = pc.GetProperty("OutputPath");
            Microsoft.Build.Evaluation.ProjectProperty TargetFramework = pc.GetProperty("TargetFrameworkVersion");

            textBox3.Text = rootassname.EvaluatedValue;

            comboBox3.Text = assname.EvaluatedValue;

            string outtypes = outtype.EvaluatedValue;

            if (outtype.EvaluatedValue == "Library")
                outtypes = "Class Library";

            comboBox5.Text = outtypes;

            textBox5.Text = OutputPath.EvaluatedValue;

            textBox9.Text = outdir.EvaluatedValue;

            comboBox2.Text = "(Not Set)";

            string b = TargetFramework.EvaluatedValue;

            b = ".NET Framework " + b;

            int i = comboBox4.FindStringExact(b);

            if (i >= 0)
                comboBox4.SelectedIndex = i;

            pc.ProjectCollection.UnloadAllProjects();

            string w = vp.GetWarningLevel();
            comboBox13.Text = w;

            w = vp.TreatWarningsAsErrors();
            if (w == "false")
            {
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
            }
            else if (w == "true")
            {
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
            }

            InitDefineConstants();

            LoadSolutionPlatforms();

            string ac = pc.DirectoryPath + "\\" + "Properties\\AssemblyInfo.cs";

            assinfo = utils.Assembly.Read(ac);

            LoadNA(comboBox6);
            LoadNA(comboBox7);
            LoadNA(comboBox16);
            LoadNA(comboBox15);
            LoadNA(comboBox20);
            LoadNA(comboBox21);
            LoadNA(comboBox31);
            LoadNA(comboBox32);
            LoadNA(comboBox33);
            LoadNA(comboBox34);
            LoadNA(comboBox25);
            LoadNA(comboBox26);
            LoadNA(comboBox29);
            LoadNA(comboBox30);

            string sc = vp.GetPlatform();
            sc = "Active(" + sc + ")";
            InitPlatform(comboBox10, sc);
            InitPlatform(comboBox12, sc);
            InitPlatform(comboBox18, sc);
            InitPlatform(comboBox22, sc);

            //if (comboBox10.Items.Contains(sc) == false)
            //    comboBox10.Items.Add(sc);
            //comboBox10.Text = sc;

            sc = vp.GetConfiguration();
            sc = "Active(" + sc + ")";
            InitPlatform(comboBox11, sc);
            InitPlatform(comboBox19, sc);
            InitPlatform(comboBox23, sc);

            //if (comboBox11.Items.Contains(sc) == false)
            //    comboBox11.Items.Add(sc);
            //comboBox11.Text = sc;

            comboBox10.Items.Add("x86");

            _notloaded = false;
        }

        public void GetProperties(string config, string platform)
        {
            if (pc == null)
                return;

            SuspendLayout();

            //vp.SetProperty("Configuration", config, false);

            //vp.SetProperty("Platform", platform, false);

            // vp.SetProperties(config, platform);

            label3.Enabled = false;
            comboBox6.Text = "N/A";
            comboBox6.Enabled = false;

            label10.Enabled = false;
            comboBox7.Text = "N/A";
            comboBox7.Enabled = false;

            try
            {
                ArrayList L = VSParsers.frameworks.GetFrameworks();

                foreach (string s in L)
                {
                    comboBox4.Items.Add(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            };

            Microsoft.Build.Evaluation.ProjectProperty outtype = pc.GetProperty("OutputType");
            Microsoft.Build.Evaluation.ProjectProperty outdir = pc.GetProperty("OutDir");
            Microsoft.Build.Evaluation.ProjectProperty assname = pc.GetProperty("AssemblyName");
            Microsoft.Build.Evaluation.ProjectProperty rootassname = pc.GetProperty("RootNamespace");

            textBox3.Text = rootassname.EvaluatedValue;

            comboBox3.Text = assname.EvaluatedValue;

            string outtypes = outtype.EvaluatedValue;

            if (outtype.EvaluatedValue == "Library")
                outtypes = "Class Library";

            comboBox5.Text = outtypes;

            textBox9.Text = outdir.EvaluatedValue;

            comboBox2.Text = "(Not Set)";

            string w = vp.GetWarningLevel();
            comboBox13.Text = w;

            w = vp.TreatWarningsAsErrors();
            if (w == "false")
            {
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
            }
            else if (w == "true")
            {
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
            }

            pc.ProjectCollection.UnloadAllProjects();

            //vp.SetProperty("Configuration", config, false);

            pc.ReevaluateIfNecessary();

            InitDefineConstants();

            pc.ProjectCollection.UnloadAllProjects();

            //LoadSolutionPlatforms();

            string ac = pc.DirectoryPath + "\\" + "Properties\\AssemblyInfo.cs";

            //assinfo = utils.Assembly.Read(ac);

            //LoadNA(comboBox6);
            //LoadNA(comboBox7);
            //LoadNA(comboBox16);
            //LoadNA(comboBox15);
            //LoadNA(comboBox20);
            //LoadNA(comboBox21);
            //LoadNA(comboBox31);
            //LoadNA(comboBox32);
            //LoadNA(comboBox33);
            //LoadNA(comboBox34);
            //LoadNA(comboBox25);
            //LoadNA(comboBox26);
            //LoadNA(comboBox29);
            //LoadNA(comboBox30);

            //string sc = vp.GetPlatform();
            //sc = "Active(" + sc + ")";
            //InitPlatform(comboBox10, sc);
            //InitPlatform(comboBox12, sc);
            //InitPlatform(comboBox18, sc);
            //InitPlatform(comboBox22, sc);

            //sc = vp.GetConfiguration();
            //sc = "Active(" + sc + ")";
            //InitPlatform(comboBox11, sc);
            //InitPlatform(comboBox19, sc);
            //InitPlatform(comboBox23, sc);

            ResumeLayout();
        }

        public void InitDefineConstants()
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;

            string w = vp.GetDefineConstants();
            string[] s = w.Split(";".ToCharArray());

            foreach (string d in s)
            {
                if (d == "DEBUG")
                    checkBox1.Checked = true;
                else if (d == "TRACE")
                    checkBox2.Checked = true;
            }
            w = vp.GetPrefer32bit();
            if (w == "true")
                checkBox3.Checked = true;
            w = vp.GetUnsafeBlocks();
            if (w == "true")
                checkBox4.Checked = true;
            w = vp.GetOptimize();
            if (w == "true")
                checkBox5.Checked = true;
        }

        public void SaveDefineConstants()
        {
            //checkBox1.Checked = false;
            //checkBox2.Checked = false;
            //checkBox3.Checked = false;
            //checkBox4.Checked = false;
            //checkBox5.Checked = false;

            //string w = vp.GetDefineConstants();
            //string[] s = w.Split(";".ToCharArray());

            //foreach (string d in s)
            //{
            //    if (d == "DEBUG")
            //        checkBox1.Checked = true;
            //    else if (d == "TRACE")
            //        checkBox2.Checked = true;

            //}

            string w = "";

            if (checkBox1.Checked == true)
                w = "DEBUG";
            if (checkBox2.Checked == true)
            {
                if (w == "DEBUG")
                    w += ";";
                w += "TRACE";
            }
            vp.SetDefineConstants(w, pc);

            w = "false";
            if (checkBox3.Checked == true)
                w = "true";
            vp.SetPrefer32bit(w, pc);

            w = "false";
            if (checkBox4.Checked == true)
                w = "true";
            vp.SetUnsafeBlocks(w, pc);

            w = "false";
            if (checkBox5.Checked == true)
                w = "true";
            vp.SetOptimize(w, pc);

            w = "false";
            if (checkBox5.Checked == true)
                w = "true";
            vp.SetOptimize(w, pc);

            w = comboBox13.Text;
            vp.SetWarningLevel(w, pc);

            w = vp.TreatWarningsAsErrors();
            if (radioButton3.Checked == true)
            {
                w = "false";
            }
            else if (radioButton4.Checked == true)
            {
                w = "true";
            }

            vp.SetTreatWarningsAsErrors(w, pc);

            w = textBox5.Text;
            vp.SetOutputPath(w, pc);

            w = comboBox4.Text;

            string[] s = w.Split(" ".ToCharArray());

            if (s.Length >= 3)
            {
                //MessageBox.Show(s[2]);

                vp.SetTargetFrameworkVersion(s[2], pc);
            }
        }

        public void InitPlatform(ComboBox cb, string sc)
        {
            ArrayList L = new ArrayList();

            int i = 0;
            while (i < cb.Items.Count)
            {
                if (cb.Items[i].ToString().StartsWith("A"))
                    cb.Items.RemoveAt(i);
                else i++;
            }
            if (cb.Items.Contains(sc) == false)
                cb.Items.Add(sc);

            cb.Text = sc;
        }

        private utils.assembly assinfo { get; set; }

        public void LoadSolutionPlatforms()
        {
            ArrayList L = ConfigurationManagerForm.GetSolutionPlatform(vs);

            ArrayList P = ConfigurationManagerForm.GetProjectPlatform(vs);

            LoadProjectPlatforms(comboBox12, P);
            LoadSolutionPlatforms(comboBox11, L);
            LoadProjectPlatforms(comboBox15, P);
            LoadSolutionPlatforms(comboBox16, L);
            LoadProjectPlatforms(comboBox18, P);
            LoadSolutionPlatforms(comboBox19, L);
        }

        public void LoadNA(ComboBox cb)
        {
            if (cb.Items.Contains("N\\A") == false)
                cb.Items.Add("N\\A");
            cb.Text = "N\\A";
            cb.Enabled = false;
        }

        public void LoadSolutionPlatforms(ComboBox cb, ArrayList L)
        {
            cb.Items.Clear();
            foreach (string s in L)
                cb.Items.Add(s);
        }

        public void LoadProjectPlatforms(ComboBox cb, ArrayList L)
        {
            cb.Items.Clear();
            foreach (string s in L)
                cb.Items.Add(s);
        }

        public void SetProperties()
        {
            if (pc == null)
                return;

            string config = GetFormConfig();
            config = config.Replace("Active", "");
            config = config.Replace("(", "");
            config = config.Replace(")", "");

            string platform = GetFormPlatform();
            platform = platform.Replace("Active", "");
            platform = platform.Replace("(", "");
            platform = platform.Replace(")", "");

            vp.SetPlatformConfig(config, platform, pc);

            string rootassname = textBox3.Text;
            string outtype = comboBox5.Text;
            string outdir = textBox5.Text;
            string assname = comboBox3.Text;

            if (outtype == "Class Library")
                outtype = "Library";

            pc.SetProperty("RootNamespace", rootassname);
            pc.SetProperty("AssemblyName", assname);
            pc.SetProperty("OutDir", outdir);
            pc.SetProperty("OutputType", outtype);
            pc.Save();

            SaveDefineConstants();

            //vp.GetCompileItems();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label26_Click(object sender, EventArgs e)
        {
        }

        private void label25_Click(object sender, EventArgs e)
        {
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            //LoadResources();
        }

        private string[] _aud = { ".wav" };

        private bool isAudio(string file)
        {
            string ext = Path.GetExtension(file);
            foreach (string s in _aud)
                if (s == ext)
                    return true;

            return false;
        }

        private void LoadResources(string ext = "")
        {
            dg.Columns.Clear();
            dg.Columns.Add("Name", "Name");
            dg.Columns.Add("Value", "Value");
            dg.Columns.Add("Comment", "Comment");
            dg.Rows.Clear();

            if (vp == null)
                return;

            ArrayList L = vp.GetEmbeddedResourcesFiles();
            if (L == null || L.Count <= 0)
                return;
            string file = L[0] as string;
            resourcefile = file;
            ArrayList V = VSProject.LoadResource(file);

            if (ext == "")
                foreach (Tuple<string, string, string> T in V)
                {
                    int id = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[id];
                    row.Cells[0].Value = T.Item1;
                    row.Cells[1].Value = T.Item2;
                    row.Cells[2].Value = T.Item3;
                }
            else if (ext == "audio")
            {
                foreach (Tuple<string, string, string> T in V)
                {
                    if (isAudio(T.Item3) == false)
                        continue;

                    int id = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[id];
                    row.Cells[0].Value = T.Item1;
                    row.Cells[1].Value = T.Item2;
                    row.Cells[2].Value = T.Item3;
                }
            }
            else if (ext == "ico")
            {
                foreach (Tuple<string, string, string> T in V)
                {
                    if (T.Item3.EndsWith(ext) == false)
                        continue;

                    int id = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[id];
                    row.Cells[0].Value = T.Item1;
                    row.Cells[1].Value = T.Item2;
                    row.Cells[2].Value = T.Item3;
                }
            }
            else
            {
                foreach (Tuple<string, string, string> T in V)
                {
                    if (T.Item3.EndsWith(ext) == false)
                        continue;

                    int id = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[id];
                    row.Cells[0].Value = T.Item1;
                    row.Cells[1].Value = T.Item2;
                    row.Cells[2].Value = T.Item3;
                }
            }
        }

        public void InitListView()
        {
            lv.Items.Clear();

            lv.Columns.Add("Name");
            lv.Columns.Add("Filename");
            lv.Columns.Add("Type");
            lv.Columns.Add("Size");
            lv.Columns.Add("Comment");

            lv.HeaderStyle = ColumnHeaderStyle.None;
            lv.View = View.SmallIcon;
            lv.SmallImageList = img;
            lv.LargeImageList = img;
            lv.Scrollable = true;
            //lv.VirtualMode = true;
        }

        public void togglelist()
        {
            lv.Size = dg.Size;
            lv.Location = dg.Location;
            lv.HeaderStyle = ColumnHeaderStyle.Clickable;
            lv.Columns[0].Width = 200;
            lv.Columns[1].Width = 200;
            //lv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lv.Visible = true;
            lv.Dock = DockStyle.Fill;

            panel11.Controls.Remove(dg);
            panel11.Controls.Add(lv);

            lv.Visible = true;
            lv.Dock = DockStyle.Fill;
        }

        public void toggledg()
        {
            dg.Size = lv.Size;
            dg.Location = lv.Location;
            //dg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dg.Dock = DockStyle.Fill;
            lv.Visible = false;

            dg.Visible = true;

            panel11.Controls.Remove(lv);
            panel11.Controls.Add(dg);
        }

        private string resourcefile { get; set; }

        private ArrayList items { get; set; }

        public void addImages(string ext)
        {
            if (cache != null)
            {
                togglelist();

                listView1.BeginUpdate();

                listView1.VirtualMode = false;

                listView1.Items.Clear();

                if (ext == ".icon")
                    addIcon(cache);
                else addImage(cache);

                listView1.VirtualMode = true;
                listView1.VirtualListSize = items.Count;

                listView1.EndUpdate();

                ResumeLayout();

                return;
            }

            items = new ArrayList();

            cache = new ArrayList();

            SuspendLayout();
            togglelist();

            if (lv.Items.Count > 0)
                return;

            ArrayList L = vp.GetEmbeddedResourcesFiles();
            if (L.Count > 0)
            {
                string file = L[0] as string;
                resourcefile = file;
                ArrayList V = VSProject.LoadResource(file);

                listView1.BeginUpdate();
                listView1.ListViewItemSorter = null;
                listView1.Items.Clear();
                listView1.View = View.Details;

                foreach (Tuple<string, string, string> T in V)
                {
                    addImage(T.Item3);
                }

                listView1.VirtualMode = true;
                listView1.VirtualListSize = items.Count;

                listView1.EndUpdate();
            }

            ResumeLayout();
        }

        public void addIcons()
        {
            SuspendLayout();
            togglelist();

            if (lv.Items.Count > 0)
                return;

            ArrayList L = vp.GetEmbeddedResourcesFiles();
            string file = L[0] as string;
            ArrayList V = VSProject.LoadResource(file);

            //listView1.BeginUpdate();

            listView1.BeginUpdate();
            listView1.ListViewItemSorter = null;
            listView1.Items.Clear();
            listView1.View = View.Details;

            foreach (Tuple<string, string, string> T in V)
            {
                if (T.Item1.ToLower().EndsWith("icon"))
                    addImage(T.Item3);
            }

            listView1.EndUpdate();

            ResumeLayout();
        }

        private string[] _ims = { ".bmp", ".png", ".gif", ".tiff", ".jpg", ".jpeg", ".ico" };

        private bool isImage(string file)
        {
            string ext = Path.GetExtension(file);
            foreach (string s in _ims)
                if (s == ext)
                    return true;

            return false;
        }

        public void addImage(string imageToLoad)
        {
            if (imageToLoad != "")
            {
                if (isImage(imageToLoad) == false)
                    return;
                Image image = Image.FromFile(imageToLoad);
                img.Images.Add(imageToLoad, image);

                //ListViewItem v = listView1.Items.Add(Path.GetFileNameWithoutExtension(imageToLoad), items.Count);//imageToLoad
                ListViewItem v = new ListViewItem(Path.GetFileName(imageToLoad), items.Count);
                v.SubItems.Add(Path.GetFileNameWithoutExtension(imageToLoad));
                v.SubItems.Add(image.PixelFormat.ToString());
                v.SubItems.Add(image.Width.ToString() + "x" + image.Height.ToString());
                v.SubItems.Add("-");
                if (items != null)
                    items.Add(v);
                if (cache != null)
                    cache.Add(v);
            }
        }

        private ArrayList cache { get; set; }

        public void addImage(ArrayList b)
        {
            items = new ArrayList();

            string[] exts = { ".png", ".bmp", "jpeg", ".icon" };

            foreach (ListViewItem v in cache)
            {
                string name = v.Text;

                string ext = Path.GetExtension(name);

                foreach (string e in exts)
                    if (e == ext)
                        items.Add(v);
            }
        }

        public void addIcon(ArrayList b)
        {
            items = new ArrayList();

            string[] exts = { ".icon", ".ico" };

            foreach (ListViewItem v in cache)
            {
                string name = v.Text;

                string ext = Path.GetExtension(name);

                foreach (string e in exts)
                    if (e.ToLower() == ext)
                        items.Add(v);
            }
        }

        private void imagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addImages(".img");
        }

        private void viewDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lv.View = View.Details;
            lv.HeaderStyle = ColumnHeaderStyle.Clickable;
        }

        private void viewInListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lv.View = View.List;
            lv.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void viewAsThumbnailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lv.View = View.LargeIcon;
            lv.HeaderStyle = ColumnHeaderStyle.None;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AssemblyForm asf = new AssemblyForm();
            asf.assinfo = assinfo;
            asf.LoadAssemblyInfo();
            asf.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.ShowDialog();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton r = radioButton1;
            if (r.Checked == true)
            {
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BuildSettingsForm bsf = new BuildSettingsForm();
            bsf.ShowDialog();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton b = radioButton5;

            if (b.Checked == true)
            {
                textBox4.Enabled = true;
            }
            else
            {
                textBox4.Enabled = false;
            }
        }

        private void existingFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();
            if (r != DialogResult.OK)
                return;
            string file = ofd.FileName;
            string name = Path.GetFileName(file);
            string b = Path.GetDirectoryName(vp.FileName);

            if (Directory.Exists(b + "\\" + "Resources") == false)
                Directory.CreateDirectory(b + "\\" + "Resources");

            string c = b + "\\" + "Resources" + "\\" + name;

            if (File.Exists(c) == false)
                File.Copy(file, c);

            if (resourcefile == null || resourcefile == "")
            {
                string source = AppDomain.CurrentDomain.BaseDirectory + "TemplateFiles\\resourcefile.resx";

                File.Copy(source, b + "\\" + "resource.resx", true);

                resourcefile = b + "\\" + "resource.resx";
            }

            Assembly asm = typeof(Bitmap).Assembly;
            Type T = asm.GetType("System.Drawing.Bitmap");
            VSProject.SaveAddResource(resourcefile, name, c, T.AssemblyQualifiedName);

            addImage(file);
        }

        private void stringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            lv.Visible = false;
            toggledg();
            LoadResources();
            ResumeLayout();
        }

        private void iconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadResources("ico");
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int i = e.ItemIndex;
            if (i < 0)
                return;
            if (i >= items.Count)
                return;
            e.Item = items[i] as ListViewItem;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_notloaded == true)
                return;

            string config = GetFormConfig();
            config = config.Replace("Active", "");
            config = config.Replace("(", "");
            config = config.Replace(")", "");

            string platform = GetFormPlatform();
            platform = platform.Replace("Active", "");
            platform = platform.Replace("(", "");
            platform = platform.Replace(")", "");

            _notloaded = true;

            vp.SetPlatformConfig(config, platform, pc);

            pc.ProjectCollection.UnloadAllProjects();

            GetProperties(config, platform);

            _notloaded = false;
        }

        public string GetFormPlatform()
        {
            int i = comboBox10.SelectedIndex;

            if (i < 0)
                return "";

            string platform = comboBox10.Items[i].ToString();

            return platform;
        }

        public string GetFormConfig()
        {
            int i = comboBox11.SelectedIndex;
            if (i < 0)
                return "";
            string config = comboBox11.Items[i].ToString();

            return config;
        }

        private void bMPImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewResource anr = new AddNewResource();
            DialogResult r = anr.ShowDialog();
            if (r != DialogResult.OK)
                return;
        }

        public void LoadResourcesView(string r)
        {
            SuspendLayout();
            this.Controls.Remove(tabControl1);
            this.Controls.Add(panel12);
            panel12.Dock = DockStyle.Fill;
            ResumeLayout();
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void pNGImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewResource anr = new AddNewResource();
            DialogResult r = anr.ShowDialog();
            if (r != DialogResult.OK)
                return;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenCodeAnalysis();
        }

        public void OpenCodeAnalysis()
        {
            OpenStyleAnalysisCode();
        }

        private StyleCop.StyleCopCore core { get; set; }

        private StyleCop.PropertyDialog ppd { get; set; }

        public void OpenStyleAnalysisCode()
        {
            //            VSSolution vs = GetVSSolution();
            //            VSProject vp = GetVSProject();
            //            DocumentForm df = sv.eo.OpenDocumentForm(vp.FileName);
            //            df.FileName = vp.FileName;

            StyleCop.StyleCopCore core = new StyleCop.StyleCopCore(null, null);
            core.Initialize(null, true);
            core.WriteResultsCache = false;
            core.DisplayUI = true;
            core.ShowSettings("Settings.StyleCop");

            ppd = core.properties;
            ppd.Dock = DockStyle.Fill;
            ppd.TopLevel = false;
            ppd.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            tabControl1.SelectedTab = tabPage14;

            tabPage14.Controls.Add(ppd);

            ppd.Show();
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadResources("audio");
        }
    }
}