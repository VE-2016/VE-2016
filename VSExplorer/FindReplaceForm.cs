using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.ScriptControl;
using ICSharpCode.NRefactory.TypeSystem;
using SolutionFormProject;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VSProvider;
using WinExplorer.UI;

namespace WinExplorer
{
    public partial class FindReplaceForm : Form
    {
        public FindReplaceForm(ExplorerForms ef)
        {
            InitializeComponent();
            SuspendLayout();
            BB = -1;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            LoadLookIn();
            LoadButtons();
            checkBox1.Enabled = false;
            this.Controls.Remove(tabControl1);
            tabControl1.Visible = false;
            panel1.BackColor = Color.White;
            panel1.Dock = DockStyle.Fill;
            panel2.BackColor = Color.White;
            panel2.Dock = DockStyle.Fill;
            statusStrip1.BackColor = Color.White;
            LoadFindPanel();
            this.ef = ef;
            contextMenuStrip1.ItemClicked += ContextMenuStrip1_ItemClicked;
            comboBox2.Text = "Entire Solution";
            comboBox4.Text = "Entire Solution";
            comboBox3.Text = "*.*";
            comboBox7.Text = "*.*";

            panel1.Resize += Panel1_Resize;

            panel2.Resize += Panel2_Resize;

            LoadSettings();

            ResumeLayout();
        }

        private string _theme = "";

        public void LoadSettings()
        {
            if (CodeEditorControl.settings != null)
            {
                Settings s = CodeEditorControl.settings;

                if (s.Theme == "VS2012Light")
                {
                    panel1.BackColor = Color.FromKnownColor(KnownColor.Control);
                    panel2.BackColor = Color.FromKnownColor(KnownColor.Control);
                }
            }
        }

        private void Panel2_Resize(object sender, EventArgs e)
        {
            Point p = groupBox4.Location;

            Size s = groupBox4.Size;

            if (groupBox4.expanded == true)

                panel4.Location = new Point(p.X, p.Y + s.Height + 2);
            else
                panel4.Location = new Point(p.X, p.Y + 30 + 2);
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            Point p = groupBox2.Location;

            Size s = groupBox2.Size;

            if (groupBox2.expanded == true)

                panel3.Location = new Point(p.X, p.Y + s.Height + 2);
            else
                panel3.Location = new Point(p.X, p.Y + 30 + 2);
        }

        private void Panel1Resize()
        {
            Panel1_Resize(null, null);
        }

        private void Panel2Resize()
        {
            Panel2_Resize(null, null);
        }

        private void ContextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem s = e.ClickedItem;
            string[] b = s.Text.Split(" ".ToCharArray());
            comboBox1.Text = b[0];
            comboBox6.Text = b[0];
        }

        public ExplorerForms ef { get; set; }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadReplacePanels();
        }

        private string[] _lk = { "Current Block", "All Open Documents", "Current Document", "Current Project", "Entire Solution", "Entire Solution (Including External Items)", "Current Project (Including External Items", "Current Selection" };

        private string[] _ext0 = { "*.cs; *resx; *.resw; *.xsd; *.wsdl; *.xaml; *.xml; *.htm; *.html; *.css;", "*.vb, *.resx, *.resw, *.xsd, *.wsdl,*.xaml, *.xml; *.htm; *.html; *.css;", "*.c; *.cpp; *.cc; *.cxx; *.tli; *.tlh;", "*.srf; *.htm; *.html; *.xml; *.gif; *.jpg; *.png; *.css; *.disco", "*.txt", "*.*" };

        private Button[] bs { get; set; }

        public void LoadButtons()
        {
            GroupBox[] g = { groupBox1, groupBox2, groupBox3, groupBox4 };

            bs = new Button[4];

            int i = 0;
            while (i < 4)
            {
                Button b = new Button();

                b.TabStop = false;
                //b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.Text = "+";
                b.Size = new Size(20, 20);
                b.Click += B_Click;
                // b.Location = new Point(g[i].Location.X - 10, g[i].Location.Y - g[i].Height - 7);
                b.Location = new Point(0, 0);
                b.BackColor = SystemColors.ButtonFace;
                b.Visible = true;
                g[i].Controls.Add(b);
                //[i].b = b;
                g[i].Controls.SetChildIndex(b, 0);

                bs[i] = b;

                i++;
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;

            GroupBoxEx[] g = { groupBox1, groupBox2, groupBox3, groupBox4 };

            if (b == bs[0])
            {
                toggle(g[0], g[1]);
                panel3.Location = new Point(g[1].Location.X, g[1].Location.Y + g[1].Height + 5);
            }

            if (b == bs[1])
            {
                toggle(g[1]);
                panel3.Location = new Point(g[1].Location.X, g[1].Location.Y + g[1].Height + 5);
            }

            if (b == bs[2])
            {
                toggle(g[2], g[3]);
                panel4.Location = new Point(g[3].Location.X, g[3].Location.Y + g[3].Height + 5);
                panel4.Width = g[3].Width;
            }

            if (b == bs[3])
            {
                toggle(g[3]);
                panel4.Location = new Point(g[3].Location.X, g[3].Location.Y + g[3].Height + 5);
                panel4.Width = g[3].Width;
            }
        }

        public void toggle(GroupBoxEx g)
        {
            Size s = this.Size;
            if (g.expanded == true)
            {
                g.exp = new Size(s.Width - 10, g.Height);
                g.Size = new Size(s.Width - 10, 20);
                g.expanded = false;
            }
            else
            {
                //g.Size = g.exp;
                g.Size = new Size(s.Width - 10, g.exp.Height);
                g.expanded = true;
                //panel3.Location = new Point(g.Location.X, g.Location.Y + g.Height + 15);
            }
        }

        public void toggle(GroupBoxEx gu, GroupBoxEx gl)
        {
            toggle(gu);

            Point p = gu.Location;

            Size s = gu.Size;

            gl.Location = new Point(p.X, p.Y + s.Height + 15);

            panel3.Location = new Point(p.X, p.Y + s.Height + 15 + gl.Height);
        }

        public void LoadLookIn()
        {
            ComboBox cb = comboBox2;

            cb.Items.Clear();

            foreach (string s in _lk)
                cb.Items.Add(s);

            cb = comboBox3;

            cb.Items.Clear();

            foreach (string s in _ext0)
                cb.Items.Add(s);

            cb = comboBox7;

            cb.Items.Clear();

            foreach (string s in _ext0)
                cb.Items.Add(s);
        }

        public void LoadFindPanel()
        {
            this.Controls.Clear();
            this.Controls.Remove(panel1);
            this.Controls.Add(panel2);
            this.Controls.Add(toolStrip1);
            this.Controls.Add(statusStrip1);
            Panel2Resize();
            panel2.Refresh();
        }

        public void LoadFindPanels()
        {
            SuspendLayout();
            LoadFindPanel();
            // LoadReplacePanel();
            // LoadFindPanel();
            ResumeLayout();
        }

        public void LoadReplacePanel()
        {
            this.Controls.Clear();
            this.Controls.Remove(panel2);
            this.Controls.Add(panel1);
            this.Controls.Add(toolStrip1);
            this.Controls.Add(statusStrip1);
            Panel1Resize();
            panel1.Refresh();
        }

        public void LoadReplacePanels()
        {
            SuspendLayout();
            LoadReplacePanel();
            //LoadFindPanel();
            //LoadReplacePanel();
            ResumeLayout();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadFindPanels();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Results == null)

                Results = FindAll();
            else
            {
                //FindAll();
                act++;
            }

            if (Results != null)
            {
                FindResultsForm rf = ef.rfs;

                rf.HighlightLine(act, Color.Blue);

                UpdateButtons();
            }

            //if (Results.Count <= 0)
            //{
            //    button3.Enabled = false;
            //    button7.Enabled = false;

            //}
            //else
            //{
            //    if (act >= Results.Count)
            //    {
            //        act = Results.Count - 1;
            //        button7.Enabled = false;
            //        //if (act <= 0)
            //        //    button3.Enabled = false;
            //    }
            //    else button7.Enabled = true;
            //    if (act < 0)
            //    {
            //        act = 0;
            //        button3.Enabled = false;
            //    }
            //    else button3.Enabled = true;
            //}
        }

        public void UpdateButtons()
        {
            if (Results == null)
                Results = new ArrayList();

            if (Results.Count <= 0)

            {
                button3.Enabled = false;
                button7.Enabled = false;
            }
            else
            {
                if (act >= Results.Count)
                {
                    act = Results.Count - 1;
                    button7.Enabled = false;
                    //if (act <= 0)
                    //    button3.Enabled = false;
                }
                else button7.Enabled = true;
                if (act <= 0)
                {
                    act = 0;
                    button3.Enabled = false;
                }
                else button3.Enabled = true;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
        }

        private void checkBox5_CheckStateChanged(object sender, EventArgs e)
        {
            CheckBox cb = checkBox5;
            if (cb.Checked == true)
            {
                button5.Enabled = true;
                button6.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
                button6.Enabled = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = checkBox4;
            if (cb.Checked == true)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Button b = button6;
            Point p = b.Location;
            contextMenuStrip1.Show(this, p.X + 20, p.Y);
        }

        private void button13_Click(object sender, EventArgs e)
        {
        }

        private void button12_Click(object sender, EventArgs e)
        {
        }

        private void button11_Click(object sender, EventArgs e)
        {
        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button b = button1;
            Point p = b.Location;
            contextMenuStrip1.Show(this, p.X + 20, p.Y);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ChooseSearchFolders csf = new ChooseSearchFolders();
            DialogResult r = csf.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChooseSearchFolders csf = new ChooseSearchFolders();
            DialogResult r = csf.ShowDialog();
        }

        private ArrayList results { get; set; }

        private string filenamedoc { get; set; }

        private void button8_Click(object sender, EventArgs e)
        {
            int bc = comboBox2.SelectedIndex;

            string s = comboBox1.Text;

            if (BB != bc || patterns != s)
            {
                results = FindAll();
            }
            else if (bc == 2)
            {
                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                if (filename != filenamedoc)

                    results = FindAll();
                else results = Results;
            }
            else results = Results;

            if (results == null)
                return;

            Results = results;

            FindResultsForm rf = ef.rfs;

            if (radioButton2.Checked == true)
                rf = ef.rfs2;

            if (rf == null)
                return;

            if (rf.results == null)
                rf.results = new ArrayList();

            if (checkBox9.Checked == false)
            {
                rf.ClearLog();
            }

            string bb = "";

            rf.AddOrUpdateOutputs(s);

            foreach (string b in results)
                rf.LogLine(b);

            if (checkBox9.Checked == true)
            {
                rf.results.AddRange(results);
            }
            else
                rf.results = results;

            rf.text = comboBox1.Text;

            UpdateButtons();
        }

        public ArrayList Results { get; set; }

        public int act;

        public string patterns { get; set; }

        public int BB { get; set; }

        private AIMS.Libraries.Scripting.Dom.IMember mb { get; set; }

        public ArrayList FindAll()
        {
            ArrayList results = new ArrayList();

            SearchEngine se = new SearchEngine();

            bool mc = true;

            if (checkBox2.Checked == false)
                mc = false;

            string s = comboBox1.Text;

            //MessageBox.Show("Find " + s);

            VSSolution vs = ef.GetVSSolution();

            if (vs == null)
                return results;

            int bb = comboBox2.SelectedIndex;

            if (bb == 1)
            {
                MessageBox.Show("Find for All Open Documents");

                ScriptControl scr = ef.scr;

                VSProject vp = vs.MainVSProject;

                ArrayList L = scr.GetOpenFiles();

                if (BB != bb || patterns != s)
                    Results = null;

                BB = bb;

                patterns = s;

                if (Results == null)
                {
                    {
                        {
                            Finder finders = new Finder();

                            Results = new ArrayList();

                            foreach (string f in L)
                            {
                                string text = File.ReadAllText(f);

                                if (mc == false)
                                {
                                    text = text.ToLower();

                                    s = s.ToLower();
                                }

                                char[] texts = text.ToCharArray();

                                char[] pattern = s.ToCharArray();

                                ArrayList found = null;

                                if (checkBox4.Checked == true)
                                {
                                    found = RegexMatch(text, s, true);
                                }
                                else
                                {
                                    found = finders.TW(pattern, pattern.Length, texts, texts.Length);
                                }

                                if (found.Count == 0)
                                    continue;

                                string[] lines = text.Split("\n".ToCharArray());

                                int fp = 0;

                                int pos = (int)found[fp];

                                int r = 0;

                                int n = 0;

                                int i = 0;
                                while (i < lines.Length)
                                {
                                    string line = lines[i];

                                    n = r + line.Length + 1;

                                    if (pos >= r && pos <= n)
                                    {
                                        do
                                        {
                                            if (checkBox3.Checked == true)
                                            {
                                                if (Char.IsLetterOrDigit(texts[pos + patterns.Length]) == false)
                                                {
                                                    if (pos > 0)
                                                        if (Char.IsLetterOrDigit(texts[pos - 1]) == false)
                                                            Results.Add(f + "\t\t" + (pos - r) + " \t" + i);
                                                }
                                            }
                                            else
                                                Results.Add(f + "\t\t" + (pos - r) + " \t" + i);

                                            fp++;

                                            if (fp < found.Count)
                                                pos = (int)found[fp];
                                            else break;
                                        }
                                        while (pos <= n);
                                    }

                                    r = n;

                                    i++;
                                }
                            }
                            act = 0;
                            return Results;
                        }
                    }
                }

                if (Results == null)
                    return null;

                if (act >= Results.Count)
                    return null;

                string dd = Results[act] as string;

                string[] cc = dd.Split("\t".ToCharArray());

                string file = cc[0];

                int l2 = Convert.ToInt32(cc[2]);

                int c2 = Convert.ToInt32(cc[1]);

                scr = ef.scr;

                string filename = file;

                //VSProject vp = vs.GetProjectbyFileName(filename);

                DomRegion dr = new DomRegion(l2, c2, l2, c2 + patterns.Length);

                scr.OpenDocuments2(filename, vp, dr, true);

                return null;
            }
            else if (bb == 0)
            {
                MessageBox.Show("Find for Current Block");

                ScriptControl scr = ef.scr;

                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                AIMS.Libraries.Scripting.Dom.IMember m = scr.GetCurrentBlock();

                if (m == null)
                {
                    Results = null;
                    return null;
                }

                string text = scr.GetCurrentBlockText(m.BodyRegion);

                TextPoint p = new TextPoint(0, m.Region.BeginLine);

                //if (BB != bb || patterns != s || filename != filenamedoc)

                if (m != mb)

                    Results = null;

                mb = m;

                filenamedoc = filename;

                BB = bb;

                patterns = s;

                if (Results == null)
                {
                    {
                        {
                            Finder finders = new Finder();

                            Results = new ArrayList();

                            if (mc == false)
                            {
                                text = text.ToLower();

                                s = s.ToLower();
                            }

                            char[] texts = text.ToCharArray();

                            char[] pattern = s.ToCharArray();

                            ArrayList found = null;

                            if (checkBox4.Checked == true)
                            {
                                found = RegexMatch(text, s, true);
                            }
                            else
                            {
                                found = finders.TW(pattern, pattern.Length, texts, texts.Length);
                            }

                            string[] lines = text.Split("\n".ToCharArray());

                            int fp = 0;

                            int pos = (int)found[fp];

                            int r = 0;

                            int n = 0;

                            int i = 0;
                            while (i < lines.Length)
                            {
                                string line = lines[i];

                                n = r + line.Length + 1;

                                if (pos >= r && pos <= n)
                                {
                                    Results.Add(filename + "\t\t" + (pos - r) + " \t" + (p.Y + i));

                                    fp++;

                                    if (fp < found.Count)
                                        pos = (int)found[fp];
                                    else break;
                                }

                                r = n;

                                i++;
                            }
                        }

                        act = 0;

                        return Results;
                    }
                }

                if (Results == null)
                    return null;

                if (act >= Results.Count)
                    return null;

                string dd = Results[act] as string;

                string[] cc = dd.Split("\t".ToCharArray());

                int l2 = Convert.ToInt32(cc[2]);

                int c2 = Convert.ToInt32(cc[1]);

                scr = ef.scr;

                filename = doc.FileName;

                VSProject vp = vs.GetProjectbyFileName(filename);

                DomRegion dr = new DomRegion(l2, c2, l2, c2 + patterns.Length);

                scr.OpenDocuments2(filename, vp, dr, true);

                return null;
            }

            if (bb == 7)
            {
                MessageBox.Show("Find for Current Selection");

                ScriptControl scr = ef.scr;

                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                string text = scr.GetCurrentSelection();

                TextPoint p = scr.GetCurrentSelectionStart();

                if (BB != bb || patterns != s || filename != filenamedoc)
                    Results = null;

                filenamedoc = filename;

                BB = bb;

                patterns = s;

                if (Results == null)
                {
                    {
                        {
                            Finder finders = new Finder();

                            Results = new ArrayList();

                            if (mc == false)
                            {
                                text = text.ToLower();

                                s = s.ToLower();
                            }

                            char[] texts = text.ToCharArray();

                            char[] pattern = s.ToCharArray();

                            ArrayList found = null;

                            if (checkBox4.Checked == true)
                            {
                                found = RegexMatch(text, s, true);
                            }
                            else
                            {
                                found = finders.TW(pattern, pattern.Length, texts, texts.Length);
                            }

                            string[] lines = text.Split("\n".ToCharArray());

                            int fp = 0;

                            int pos = (int)found[fp];

                            int r = 0;

                            int n = 0;

                            int i = 0;
                            while (i < lines.Length)
                            {
                                string line = lines[i];

                                n = r + line.Length + 1;

                                if (pos >= r && pos <= n)
                                {
                                    Results.Add(filename + "\t\t" + (pos - r) + " \t" + (p.Y + i));

                                    fp++;

                                    if (fp < found.Count)
                                        pos = (int)found[fp];
                                    else break;
                                }

                                r = n;

                                i++;
                            }
                        }

                        act = 0;

                        return Results;
                    }
                }

                if (Results == null)
                    return null;

                if (act >= Results.Count)
                    return null;

                string dd = Results[act] as string;

                string[] cc = dd.Split("\t".ToCharArray());

                int l2 = Convert.ToInt32(cc[2]);

                int c2 = Convert.ToInt32(cc[1]);

                scr = ef.scr;

                filename = doc.FileName;

                VSProject vp = vs.GetProjectbyFileName(filename);

                DomRegion dr = new DomRegion(l2, c2, l2, c2 + patterns.Length);

                scr.OpenDocuments2(filename, vp, dr, true);

                return null;
            }

            if (bb == 3)
            {
                //MessageBox.Show("Find for Project");

                ScriptControl scr = ef.scr;

                //string filename = doc.FileName;

                VSProject vp = vs.MainVSProject;

                if (vp == null)
                {
                    MessageBox.Show("No project has been selected");
                    Results = new ArrayList();
                    return Results;
                }

                ArrayList L = vp.GetCompileItems();

                if (BB != bb || patterns != s)
                    Results = null;

                BB = bb;

                patterns = s;

                if (Results == null)
                {
                    {
                        {
                            Finder finders = new Finder();

                            Results = new ArrayList();

                            foreach (string f in L)
                            {
                                string text = File.ReadAllText(f);

                                if (mc == false)
                                {
                                    text = text.ToLower();

                                    s = s.ToLower();
                                }

                                char[] texts = text.ToCharArray();

                                char[] pattern = s.ToCharArray();

                                ArrayList found = null;

                                if (checkBox4.Checked == true)
                                {
                                    found = RegexMatch(text, s, true);
                                }
                                else
                                {
                                    found = finders.TW(pattern, pattern.Length, texts, texts.Length);
                                }

                                if (found.Count == 0)
                                    continue;

                                string[] lines = text.Split("\n".ToCharArray());

                                int fp = 0;

                                int pos = (int)found[fp];

                                int r = 0;

                                int n = 0;

                                int i = 0;
                                while (i < lines.Length)
                                {
                                    string line = lines[i];

                                    n = r + line.Length + 1;

                                    if (pos >= r && pos <= n)
                                    {
                                        Results.Add(f + "\t" + (pos - r) + " \t" + i);

                                        fp++;

                                        if (fp < found.Count)
                                            pos = (int)found[fp];
                                        else break;
                                    }

                                    r = n;

                                    i++;
                                }
                            }
                            act = 0;
                            return Results;
                        }
                    }
                }

                if (Results == null)
                    return null;

                if (act >= Results.Count)
                    return null;

                string dd = Results[act] as string;

                string[] cc = dd.Split("\t".ToCharArray());

                string file = cc[0];

                int l2 = Convert.ToInt32(cc[2]);

                int c2 = Convert.ToInt32(cc[1]);

                scr = ef.scr;

                string filename = file;

                //VSProject vp = vs.GetProjectbyFileName(filename);

                DomRegion dr = new DomRegion(l2, c2, l2, c2 + patterns.Length);

                scr.OpenDocuments2(filename, vp, dr, true);

                return null;
            }
            else if (bb == 2)
            {
                //MessageBox.Show("Find for Current Document");

                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                string text = doc.syntaxDocument1.Text;

                if (BB != bb || patterns != s || filename != filenamedoc)
                    Results = null;

                filenamedoc = filename;

                BB = bb;

                patterns = s;

                if (Results == null)
                {
                    {
                        {
                            Finder finders = new Finder();

                            Results = new ArrayList();

                            //text = File.ReadAllText(f);

                            if (mc == false)
                            {
                                text = text.ToLower();

                                s = s.ToLower();
                            }

                            char[] texts = text.ToCharArray();

                            char[] pattern = s.ToCharArray();

                            ArrayList found = null;

                            if (checkBox4.Checked == true)
                            {
                                found = RegexMatch(text, s, true);
                            }
                            else
                            {
                                found = finders.TW(pattern, pattern.Length, texts, texts.Length);
                            }

                            if (found.Count > 0)
                            {
                                string[] lines = text.Split("\n".ToCharArray());

                                int fp = 0;

                                int pos = (int)found[fp];

                                int r = 0;

                                int n = 0;

                                int i = 0;
                                while (i < lines.Length)
                                {
                                    string line = lines[i];

                                    n = r + line.Length + 1;

                                    if (pos >= r && pos <= n)
                                    {
                                        do
                                        {
                                            Results.Add(filename + "\t" + (pos - r) + " \t" + i);

                                            fp++;

                                            if (fp < found.Count)
                                                pos = (int)found[fp];
                                            else break;
                                        }
                                        while (pos <= n);
                                    }

                                    r = n;

                                    i++;
                                }
                            }
                        }
                        act = 0;

                        return Results;
                    }
                }

                if (Results == null)
                    return null;

                if (act >= Results.Count)
                    return null;

                string dd = Results[act] as string;

                string[] cc = dd.Split("\t".ToCharArray());

                int l2 = Convert.ToInt32(cc[2]);

                int c2 = Convert.ToInt32(cc[1]);

                ScriptControl scr = ef.scr;

                filename = doc.FileName;

                VSProject vp = vs.GetProjectbyFileName(filename);

                DomRegion dr = new DomRegion(l2, c2, l2, c2 + patterns.Length);

                scr.OpenDocuments2(filename, vp, dr, true);

                return null;
            }
            Finder finder = new Finder();

            foreach (VSProject p in vs.Projects)
            {
                ArrayList L = p.GetCompileItems();

                foreach (string f in L)
                {
                    string text = File.ReadAllText(f);

                    if (mc == false)
                    {
                        text = text.ToLower();

                        s = s.ToLower();
                    }

                    char[] texts = text.ToCharArray();

                    char[] pattern = s.ToCharArray();

                    ArrayList found = null;

                    if (checkBox4.Checked == true)
                    {
                        found = RegexMatch(text, s, true);
                    }
                    else
                    {
                        found = finder.TW(pattern, pattern.Length, texts, texts.Length);
                    }

                    if (found.Count == 0)
                        continue;

                    string[] lines = text.Split("\n".ToCharArray());

                    int fp = 0;

                    int pos = (int)found[fp];

                    int r = 0;

                    int n = 0;

                    int i = 0;
                    while (i < lines.Length)
                    {
                        string line = lines[i];

                        n = r + line.Length + 1;

                        if (pos >= r && pos <= n)
                        {
                            results.Add(f + "\t" + (pos - r) + " \t" + i);

                            fp++;

                            if (fp < found.Count)
                                pos = (int)found[fp];
                            else break;
                        }

                        r = n;

                        i++;
                    }
                }
            }

            if (checkBox3.Checked == true)
            {
                string pp = comboBox1.Text;

                char[] separators = { '\t', ' ', '\n', '.', '-', '(' };

                ArrayList R = new ArrayList();

                foreach (string sw in results)
                {
                    string[] d = sw.Split(separators);

                    foreach (string b in d)
                        if (b.Trim().ToLower() == pp.Trim().ToLower())
                            R.Add(sw);
                }

                results = R;
            }

            if (checkBox10.Checked == true)
            {
                ArrayList R = new ArrayList();
                foreach (string sw in results)
                {
                    string[] d = sw.Split(" ".ToCharArray());
                    R.Add(d[0] + "\n");
                }

                results = R;
            }

            return results;
        }

        public ArrayList RegexMatch(string s, string p, bool IgnoreCase)
        {
            ArrayList R = new ArrayList();
            //Regex regex = new Regex();

            //Match result = Regex.Match(s, @"([a-zA-Z]+) (\d+)", RegexOptions.IgnoreCase);

            Match result = Regex.Match(s, p, RegexOptions.IgnoreCase);

            while (result.Success)
            {
                R.Add(result.Index);
                Console.WriteLine("Match: {0}-{1}-{2}", result.Value, result.Index, result.Length);
                result = result.NextMatch();
            }
            return R;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Results == null)

                Results = FindAll();
            else
            {
                act--;

                if (act < 0)
                    act = 0;
            }

            if (Results != null)
            {
                FindResultsForm rf = ef.rfs;

                rf.HighlightLine(act, Color.Blue);

                UpdateButtons();
            }
        }

        private ArrayList ToLines(ArrayList R)
        {
            ArrayList L = new ArrayList();
            foreach (string s in R)
            {
                string[] cc = s.Split("\t".ToCharArray());

                int l2 = Convert.ToInt32(cc[1]);

                L.Add(l2);
            }

            return L;
        }

        private ArrayList ToLinesProject(ArrayList R)
        {
            ArrayList L = new ArrayList();
            foreach (string s in R)
            {
                string[] cc = s.Split("\t".ToCharArray());

                string file = cc[0];

                int c2 = Convert.ToInt32(cc[1]);

                int l2 = Convert.ToInt32(cc[2]);

                System.Tuple<string, int, int> tt = new System.Tuple<string, int, int>(file, c2, l2);

                L.Add(tt);
            }

            return L;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (Results == null)
                return;
            if (ef == null)
                return;
            if (ef.scr == null)
                return;

            int bb = comboBox2.SelectedIndex;

            if (bb == 2)
            {
                ScriptControl scr = ef.scr;

                ArrayList L = ToLinesProject(Results);

                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                VSSolution vs = ef.GetVSSolution();

                VSProject vp = vs.GetProjectbyFileName(filename);

                scr.BookmarkAll(filename, vp, L);
            }
            else if (bb == 3)
            {
                ScriptControl scr = ef.scr;

                ArrayList L = ToLinesProject(Results);

                Document doc = ef.scr.GetActiveDocument();

                string filename = doc.FileName;

                VSSolution vs = ef.GetVSSolution();

                VSProject vp = vs.MainVSProject;

                scr.BookmarkAll(vp, L);
            }
        }
    }
}