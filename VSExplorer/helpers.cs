using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace utils
{
    public class Navigator
    {
        public TreeView v { get; set; }

        public ArrayList N { get; set; }

        public int act = -1;

        public int maxsize = 10;

        public Navigator(TreeView w)
        {
            N = new ArrayList();
            v = w;
        }

        public void Add(TreeNode node)
        {
            N.Add(node);
            act++;
            if (act > maxsize)
            {
                N.RemoveAt(0);
                act--;
            }
        }

        public void Prev()
        {
            if (v == null)
                return;
            if (N == null)
                return;
            if (act <= 0)
                return;

            act--;

            TreeNode node = N[act] as TreeNode;

            v.SelectedNode = node;

            node.EnsureVisible();
        }
        public void Next()
        {
            if (v == null)
                return;
            if (N == null)
                return;
            //if (act <= 0)
            //    return;

            act++;

            if (act >= N.Count)
                act = N.Count - 1;

            if (act < 0)
                act = 0;

            if (N.Count > 0)
            {

                TreeNode node = N[act] as TreeNode;

                v.SelectedNode = node;

                node.EnsureVisible();

            }
        }
    }

    public class testing
    {
        public int iters { get; set; }

        public enum types { simple, extended, minmax, min, max };

        public types ts { get; set; }
    }

    public class dirs
    {
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
    }

    public class launcher
    {
        public static void programlauncher(string exefile)
        {
            // For the example
            //const string ex1 = "C:\\";
            //const string ex2 = "C:\\Dir";

            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exefile;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
        }
    }

    public class stater
    {
        public string[] states = { "vis", "col", "eds" };

        public ArrayList colors = new ArrayList();

        public ArrayList act = new ArrayList();

        public ArrayList tags = new ArrayList();

        public stater()
        {
            colors.Add(Color.Blue);
            colors.Add(Color.LightBlue);
            colors.Add(Color.Green);
            colors.Add(Color.LightGreen);

            tags.Add(0);
        }

        public void addstate()
        {
            int ind = act.Count;

            act.Add(0);
        }

        public int getact(int id)
        {
            return (int)act[id];
        }

        public int setnextstate(int id)
        {
            int ids = (((int)(act[id])) + 1) % 3;

            act[id] = ids;

            return ids;
        }
    }

    //public class controller
    //{
    //    //public ExplorerTreeView.frmDemo expc = null;

    //    //public ICSharpCode.ILSpy.MainWindow mw = null;

    //    public ListBox lb = null;

    //    public SplitContainer sc = null;

    //    public string a = "";

    //    public stater sts = new stater();

    //    public ArrayList inf = null;

    //    public void load_sc(ToolStrip toolStrip1, SplitContainer splitContainer1, Control cc)
    //    {
    //        //if (cc == null)
    //        //    cc = new controller();

    //        //if (cc.sts == null)
    //        //    cc.sts = new stater();

    //        //cc.sts.addstate();
    //        //cc.sts.addstate();

    //        ToolStripButton b0 = new ToolStripButton("a"/*cc.sts.states[0]*/);
    //        b0.Click += new EventHandler(OnBtnClicked);
    //        b0.Dock = DockStyle.Right;
    //        b0.Tag = null;
    //        toolStrip1.Items.Add(b0);

    //        ArrayList L = new ArrayList();

    //        L = utils.helpers.getallcontrols(splitContainer1.GetType(), cc, L);

    //        if (inf == null)
    //            inf = new ArrayList();

    //        int i = 0;
    //        foreach (Control c in L)
    //        {
    //            ToolStripButton b1 = new ToolStripButton("L");
    //            b1.BackColor = (Color)Color.LightGreen;
    //            b1.Click += new EventHandler(OnBtnClicked);
    //            b1.Dock = DockStyle.Right;

    //            toolStrip1.Items.Add(b1);

    //            infobj m = new infobj();
    //            m.c = ((SplitContainer)c).Panel1;
    //            m.p = c;
    //            m.a0 = 0;

    //            inf.Add(m);

    //            b1.Tag = m;

    //            ToolStripButton b2 = new ToolStripButton("R");
    //            b2.BackColor = Color.LightGreen; //(Color)cc.sts.colors[1];
    //            b2.Click += new EventHandler(OnBtnClicked);
    //            b2.Dock = DockStyle.Right;

    //            toolStrip1.Items.Add(b2);

    //            m = new infobj();
    //            m.c = ((SplitContainer)c).Panel2;
    //            m.p = c;
    //            m.a0 = 1;

    //            b2.Tag = m;

    //            inf.Add(m);

    //        }
    //    }

    //    public class infobj
    //    {
    //        public Control c = null;

    //        public Control p = null;

    //        public int a0 = 0;

    //        public int a1 = 0;

    //        public int a2 = 0;

    //        public ToolStrip tsp = null;

    //        public ToolStripButton tsb = null;

    //    }

    //    private Panel p = null;

    //    private ToolStrip tsp = null;

    //    utils.controller cc = null;

    //    private ToolStrip ts = null;

    //    private void OnBtnClicked(object sender, EventArgs e)
    //    {
    //        ToolStripButton btn = sender as ToolStripButton;

    //        // handle the button click

    //        //p = new Panel();
    //        //p.BackColor= Color.Firebrick;
    //        //p.Dock = DockStyle.Fill;

    //        if (cc == null)
    //            return;

    //        string name = btn.Text;

    //        if (btn.Tag == null)
    //        {
    //            //cc.sts.setnextstate(0);

    //            //int ids = cc.sts.getact(0);

    //            //string text = cc.sts.states[ids];

    //            //btn.Text = text;

    //            tsp = null;

    //            ts = null;

    //            return;

    //        }

    //        //   SplitContainer s = null;//(SplitContainer)btn.Tag;

    //        infobj inf = (infobj)btn.Tag;

    //        Control c = (Control)inf.c;

    //        try
    //        {
    //            TabControl tc = null;

    //            SplitContainer s = null;

    //            if (tsp != null)
    //            {
    //                s = (SplitContainer)tsp.Tag;

    //                if (inf.a0 == 0)
    //                {
    //                    // if (name == "0")
    //                    s.Panel1.Controls.Remove(tsp);
    //                    s.Panel2.Controls.Remove(tsp);
    //                    tsp = null;

    //                    if (ts != null)
    //                    {
    //                        s.Panel1.Controls.Remove(ts);
    //                        s.Panel2.Controls.Remove(ts);
    //                    }
    //                    ts = null;

    //                    return;
    //                }
    //                else
    //                {
    //                    s.Panel2.Controls.Remove(tsp);
    //                    s.Panel1.Controls.Remove(tsp);
    //                    tsp = null;
    //                    if (ts != null)
    //                    {
    //                        s.Panel2.Controls.Remove(ts);
    //                        s.Panel1.Controls.Remove(ts);
    //                    }
    //                    ts = null;

    //                    return;

    //                }
    //            }
    //            else if (inf.a0 == 0)
    //            {
    //                s = (SplitContainer)inf.p;

    //                tsp = new ToolStrip();

    //                tsp.Tag = s;

    //                createTSB(tsp, inf);

    //                //if (inf.a0 == 0)

    //                s.Panel1.Controls.Add(tsp);

    //                ts = utils.helpers.add_toolstrip(s.Panel1);

    //                ArrayList L = new ArrayList();

    //                Type[] T = utils.helpers.LoadType("System.Windows.Forms.TabControl");

    //                Control c2 = getallcontrols2(T[0], s.Panel1, L);

    //                if (inf2 == null)
    //                    inf2 = new ArrayList();

    //                tc = (TabControl)c2;

    //                foreach (TabPage p in tc.TabPages)
    //                {
    //                    ToolStripButton b2 = new ToolStripButton("TP");
    //                    //b2.BackColor = Color)cc.sts.colors[1];
    //                    b2.Click += new EventHandler(OnBtnClicked3);
    //                    b2.Dock = DockStyle.Right;

    //                    infobj b = new infobj();
    //                    b.p = tc;
    //                    b.c = p;

    //                    b2.Tag = b;

    //                    ts.Items.Add(b2);

    //                }

    //                ToolStripButton b3 = new ToolStripButton("SH");
    //                //b2.BackColor = Color)cc.sts.colors[1];
    //                b3.Click += new EventHandler(OnBtnClicked5);
    //                b3.Dock = DockStyle.Right;

    //                infobj bb = new infobj();
    //                bb.p = s.Panel1;
    //                bb.c = tsp;

    //                b3.Tag = bb;

    //                ts.Items.Add(b3);

    //                //                        s.Panel2.Controls.Remove(tsp);

    //                //                      tsp = null;
    //                return;
    //            }
    //            else
    //            {
    //                SplitContainer sa = (SplitContainer)inf.p;

    //                //sa.Panel1.Controls.Remove(tsp);
    //                //tsp = null;

    //                tsp = new ToolStrip();
    //                tsp.Tag = sa;

    //                createTSB(tsp, inf);

    //                //if (inf.a0 == 0)

    //                sa.Panel2.Controls.Add(tsp);

    //                ts = utils.helpers.add_toolstrip(sa.Panel2);
    //                ArrayList L = new ArrayList();

    //                Type[] T = utils.helpers.LoadType("System.Windows.Forms.TabControl");

    //                Control c2 = getallcontrols2(T[0], sa.Panel2, L);

    //                if (inf2 == null)
    //                    inf2 = new ArrayList();

    //                tc = (TabControl)c2;

    //                foreach (TabPage p in tc.TabPages)
    //                {
    //                    ToolStripButton b2 = new ToolStripButton("TP");
    //                    //b2.BackColor = Color)cc.sts.colors[1];
    //                    b2.Click += new EventHandler(OnBtnClicked3);
    //                    b2.Dock = DockStyle.Right;

    //                    infobj b = new infobj();
    //                    b.p = tc;
    //                    b.c = p;

    //                    b2.Tag = b;

    //                    ts.Items.Add(b2);

    //                }

    //                ToolStripButton b3 = new ToolStripButton("SH");
    //                //b2.BackColor = Color)cc.sts.colors[1];
    //                b3.Click += new EventHandler(OnBtnClicked5);
    //                b3.Dock = DockStyle.Right;

    //                infobj bb = new infobj();
    //                bb.p = sa.Panel2;
    //                bb.c = tsp;

    //                b3.Tag = bb;

    //                ts.Items.Add(b3);

    //                //sa.Panel2.Controls.Add(tsp);
    //                // set_props(s.Panel2, "Visible", false);
    //            }
    //        }

    //        finally
    //        {
    //        }

    //    }

    //    public Control getallcontrols2(Type T, Control cc, ArrayList L)
    //    {
    //        foreach (Control c in cc.Controls)
    //        {
    //            if (c.GetType() == T)
    //            {
    //                L.Add(c);

    //                return c;
    //            }

    //            Control c2 = getallcontrols2(T, c, L);

    //            if (c2 != null)
    //                return c2;
    //        }

    //        return null;

    //    }

    //    //FormLoader fk = null;

    //    private void OnBtnClicked4(object sender, EventArgs e)
    //    {
    //        ToolStripButton btn = sender as ToolStripButton;
    //        infobj inf = (infobj)btn.Tag;

    //        SplitContainer s = (SplitContainer)(inf.p);

    //        ToolStrip ts = btn.GetCurrentParent();

    //        if (btn.Text == "Panel")
    //        {
    //            btn.Text = "Restore";

    //            fk = new FormLoader();

    //            if (inf.a0 == 0)
    //            {
    //                set_props(s.Panel1, "Visible", false);
    //                btn.GetCurrentParent().Visible = true;
    //                utils.helpers.FormLoaders(fk, null, s.Panel1);
    //            }
    //            else
    //            {
    //                set_props(s.Panel2, "Visible", false);
    //                btn.GetCurrentParent().Visible = true;
    //                utils.helpers.FormLoaders(fk, null, s.Panel2);

    //            }

    //            //if (pdf != null)
    //            //    pdf.Close();

    //            //pdf = null;

    //            //if (pdf == null)
    //            //    pdf = new FMain();

    //            //if (pdf != null)

    //                //fk.load_forms(pdf);

    //                return;

    //        }
    //        else
    //        {
    //            btn.Text = "Panel";

    //            if (inf.a0 == 0)
    //            {
    //                set_props(s.Panel1, "Visible", true);
    //                btn.GetCurrentParent().Visible = true;
    //                if (fk != null)
    //                    fk.Close();
    //                fk = null;
    //            }
    //            else
    //            {
    //                set_props(s.Panel2, "Visible", true);
    //                btn.GetCurrentParent().Visible = true;
    //                if (fk != null)
    //                    fk.Close();
    //                fk = null;

    //            }

    //        }
    //    }

    //    private void OnBtnClicked5(object sender, EventArgs e)
    //    {
    //        ToolStripButton btn = sender as ToolStripButton;
    //        infobj inf = (infobj)btn.Tag;

    //        Control tc = (Control)(inf.p);

    //        ToolStrip ts = (ToolStrip)inf.c;

    //        if (ts == null)
    //            return;

    //        if (tc == null)
    //            return;

    //        if (tc.Controls == null)
    //            return;

    //        if (ts.Visible == true)
    //        {
    //            ts.Visible = false;
    //            tc.Controls.Remove(ts);
    //        }
    //        else
    //        {
    //            ArrayList L = new ArrayList();
    //            L = utils.helpers.getallcontrols(fk.Controls.GetType(), tc, L);

    //            foreach (Control c in L)
    //                tc.Controls.Remove(c);

    //            ts.Visible = true;

    //            tc.Controls.Add(ts);

    //            foreach (Control c in L)
    //                tc.Controls.Add(c);

    //        }
    //    }

    //    public void set_props(Control c, Control cc, string p, object v)
    //    {
    //        ArrayList L = new ArrayList();

    //        L = utils.helpers.getallcontrols(c.GetType(), cc, L);

    //        System.Reflection.PropertyInfo pp = typeof(Control).GetProperty(p);

    //        foreach (Control d in L)
    //        {
    //            pp.SetValue(d, v, null);

    //        }
    //    }

    //    public void set_props(Control c, string p, object v)
    //    {
    //        ArrayList L = new ArrayList();

    //        foreach (Control a in c.Controls)
    //            L.Add(a);

    //        System.Reflection.PropertyInfo pp = typeof(Control).GetProperty(p);

    //        foreach (Control d in L)
    //        {
    //            pp.SetValue(d, v, null);

    //        }
    //    }

    //    private void OnBtnClicked3(object sender, EventArgs e)
    //    {
    //        ToolStripButton btn = sender as ToolStripButton;
    //        infobj inf = (infobj)btn.Tag;

    //        TabControl tc = (TabControl)(inf.p);

    //        TabPage tp = (TabPage)inf.c;

    //        if (tc.Controls.Contains(tp) == true)
    //        {
    //            tc.Controls.Remove(tp);
    //            tc.Refresh();
    //        }

    //        else
    //        {
    //            tc.Controls.Add(tp);
    //            tc.Refresh();
    //            //tp.Visible = true;
    //        }

    //        //if (inf.a0 == 0)
    //        //{
    //        //    if (s.Panel2Collapsed == false)
    //        //        s.Panel2Collapsed = true;
    //        //    else
    //        //        s.Panel2Collapsed = false;
    //        //}
    //        //else
    //        //{
    //        //    if (s.Panel1Collapsed == false)
    //        //        s.Panel1Collapsed = true;
    //        //    else
    //        //        s.Panel1Collapsed = false;
    //        //}
    //    }

    //    private void OnBtnClicked2(object sender, EventArgs e)
    //    {
    //        ToolStripButton btn = sender as ToolStripButton;
    //        infobj inf = (infobj)btn.Tag;

    //        SplitContainer s = (SplitContainer)(inf.p);

    //        if (inf.a0 == 0)
    //        {
    //            if (s.Panel2Collapsed == false)
    //                s.Panel2Collapsed = true;
    //            else
    //                s.Panel2Collapsed = false;
    //        }
    //        else
    //        {
    //            if (s.Panel1Collapsed == false)
    //                s.Panel1Collapsed = true;
    //            else
    //                s.Panel1Collapsed = false;
    //        }
    //    }

    //    public ArrayList inf2 = new ArrayList();

    //    public void createTSB(ToolStrip tsp, infobj f)
    //    {
    //        ToolStripButton b2 = new ToolStripButton("Toggle");
    //        //b2.BackColor = Color)cc.sts.colors[1];
    //        b2.Click += new EventHandler(OnBtnClicked2);
    //        b2.Dock = DockStyle.Right;
    //        b2.Tag = f;

    //        tsp.Items.Add(b2);

    //        ToolStripButton b3 = new ToolStripButton("Panel");
    //        //b2.BackColor = Color)cc.sts.colors[1];
    //        b3.Click += new EventHandler(OnBtnClicked4);
    //        b3.Dock = DockStyle.Right;
    //        b3.Tag = f;

    //        tsp.Items.Add(b3);

    //    }

    //    public SplitContainer add_dir_view(SplitContainer p, bool right)
    //    {
    //        if (sc != null)
    //            return sc;

    //        if (right == true)
    //            sc = helpers.create_sc(p, true);

    //        return sc;
    //    }

    //    public void set_select_tree()
    //    {
    //        WilsonProgramming.ExplorerTreeView tv = expc.treeExplorer;

    //        int index = lb.SelectedIndex;

    //        if (index < 0)
    //            return;

    //        string s = lb.Items[index].ToString();

    //        s = Path.GetFullPath(s);

    //        //tv.findItem("C:\\");

    //        tv.gotointem(s);

    //        if (tv.dns != null)
    //            tv.dns.ExpandAll();

    //    }

    //    public void toggle_view()
    //    {
    //        if (sc == null)
    //            return;

    //        if (sc.Panel2Collapsed == true)
    //            sc.Panel2Collapsed = false;
    //        else sc.Panel2Collapsed = true;

    //    }

    //    public void add_control(Control p, Control c)
    //    {
    //        //c.FormBorderStyle = FormBorderStyle.None;
    //        //c.TopLevel = false;

    //        p.Controls.Add(c);
    //        c.Show();
    //    }

    //    public void add_form_control(Control p, Form c)
    //    {
    //        p = sc;

    //        c.FormBorderStyle = FormBorderStyle.None;
    //        c.TopLevel = false;
    //        c.Dock = DockStyle.Fill;

    //        sc.Panel2.Controls.Add(c);
    //        c.Show();
    //    }

    //}

    //    class ChildControls
    //{
    //private List<object> listChildren;

    //public List<object> GetChildren(Control p_vParent, int p_nLevel)
    //{
    //    if (p_vParent == null)
    //    {
    //        throw new ArgumentNullException("Element {0} is null!", p_vParent.ToString());
    //    }

    //    this.listChildren = new List<object>();

    //    this.GetChildControls(p_vParent, p_nLevel);

    //    return this.listChildren;

    //}

    //private void GetChildControls(Control p_vParent, int p_nLevel)
    //{
    //    int nChildCount = Control.GetChildrenCount(p_vParent);

    //    for (int i = 0; i <= nChildCount - 1; i++)
    //    {
    //        Control v = (Visual)VisualTreeHelper.GetChild(p_vParent, i);

    //        listChildren.Add((object)v);

    //        if (VisualTreeHelper.GetChildrenCount(v) > 0)
    //        {
    //            GetChildControls(v, p_nLevel + 1);
    //        }
    //    }
    //}

    public class helpers
    {
        private TabPage _T1 = null;

        private TabPage _T2 = null;

        private TabControl _TB = null;

        private RichTextBox _RT = null;

        private PictureBox _PB = null;

        public ListBox LB1 = null;

        public string pwd0 = "";

        public string pwd1 = "";

        public ListBox LB2 = null;

        static public void get_control_to_spc(Control c, Control p, ToolStrip ts, SplitContainer spc)
        {
            Panel pls = new Panel();

            pls.Controls.Add(c);

            c.Show();

            p.Controls.Remove(ts);

            utils.helpers.get_into_sp(pls, spc, p, true);

            p.Controls.Add(ts);
        }

        /// <summary>
        /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// Provides a method for performing a deep copy of an object.
        /// Binary Serialization is used to perform the copy.
        /// </summary>
        public static class ObjectCopier
        {
            /// <summary>
            /// Perform a deep Copy of the object.
            /// </summary>
            /// <typeparam name="T">The type of object being copied.</typeparam>
            /// <param name="source">The object instance to copy.</param>
            /// <returns>The copied object.</returns>
            public static T Clone<T>(T source)
            {
                if (!typeof(T).IsSerializable)
                {
                    throw new ArgumentException("The type must be serializable.", "source");
                }

                // Don't serialize a null object, simply return the default for that object
                if (Object.ReferenceEquals(source, null))
                {
                    return default(T);
                }

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new MemoryStream();
                using (stream)
                {
                    formatter.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
        }

        static public string get_string(ListBox lb, ref int index)
        {
            if (lb == null)
                return "";

            index = lb.SelectedIndex;

            if (index < 0)
                return "";

            string s = lb.Items[index].ToString();

            return s;
        }

        static public ArrayList get_upper(Type T, Control c)
        {
            ArrayList L = new ArrayList();

            utils.helpers.getallcontrols2(T, c, L);

            return L;
        }

        static public SplitContainer get_first(ArrayList L)
        {
            SplitContainer sp = null;

            foreach (Control c in L)
                if (c.GetType() == typeof(SplitContainer))
                    return (SplitContainer)c;

            return sp;
        }

        static public void unget_sp(bool right, Control p)
        {
            ArrayList L = utils.helpers.get_upper(typeof(SplitContainer), p);

            SplitContainer sp = utils.helpers.get_first(L);

            unget_sp(right, sp, p);
        }

        static public void unget_sp(bool right, SplitContainer sp, Control p)
        {
            ArrayList L = new ArrayList();

            L = utils.helpers.getallcontrols(sp.Panel1, L, true);

            foreach (Control c in L)
                sp.Panel1.Controls.Remove(c);

            //Control p = sp.Parent;

            p.Controls.Remove(sp);

            foreach (Control c in L)
            {
                if (c.GetType() == typeof(SplitterPanel)) continue;

                p.Controls.Add(c);
            }
        }

        static public void get_into_sp(Control c, Control p, Control r, bool right)
        {
            SplitContainer sp = new SplitContainer();
            sp.Dock = DockStyle.Fill;

            ArrayList L = new ArrayList();

            foreach (Control cc in r.Controls)
            {
                L.Add(cc);
            }

            Panel pls = null;

            if (p != null)
                r.Controls.Remove(p);
            else
            {
                foreach (Control cc in L)

                    if (cc.GetType() != typeof(ToolStrip))
                        r.Controls.Remove(cc);

                foreach (Control cc in L)

                    if (cc.GetType() != typeof(ToolStrip))
                    {
                        sp.Panel1.Controls.Add(cc);
                        cc.Anchor = AnchorStyles.None;
                        cc.Dock = DockStyle.None;
                    }
            }
            if (p != null)
                p.Dock = DockStyle.Fill;

            sp.Panel2.Controls.Add(c);
            c.Dock = DockStyle.Fill;

            r.Controls.Add(sp);

            c.Show();
            sp.Show();
            r.Show();

            sp.Orientation = Orientation.Horizontal;
        }

        static public void create_pls(Control c)
        {
            //Panel pls = new Panel();

            //pls.Width = c.Width;
            //pls.Height = c.Height;

            //return pls;
        }

        static public string[] get_all_strings(ListBox lb)
        {
            if (lb == null)
                return null;

            string[] s = new string[lb.Items.Count];

            int i = 0;
            foreach (string p in s)
            {
                s[i] = lb.Items[i].ToString();
                i++;
            }

            return s;
        }

        public helpers()
        {
        }

        //static public void FormLoaders(FormLoader fp, Control c, Control p)
        //{
        //    fp.setmaster(c);

        //    fp.Dock = DockStyle.Fill;

        //    fp.TopLevel = false;

        //    fp.FormBorderStyle = FormBorderStyle.None;

        //    p.Controls.Add(fp);

        //    if (c != null)
        //        c.Visible = false;

        //    fp.Show();

        //}

        static public void FormLoader(Form fp, Control c, Control p)
        {
            //fp.setmaster(c);

            fp.Dock = DockStyle.Fill;

            fp.TopLevel = false;

            fp.FormBorderStyle = FormBorderStyle.None;

            p.Controls.Add(fp);

            if (c != null)
                c.Visible = false;

            fp.Show();
        }

        static public Type[] LoadType(string typeName)
        {
            return LoadType(typeName, true);
        }

        static public Type[] LoadType(string typeName, bool referenced)
        {
            return LoadType(typeName, referenced, false);
        }

        static private Type[] LoadType(string typeName, bool referenced, bool gac)
        {
            //check for problematic work
            if (string.IsNullOrEmpty(typeName) || !referenced && !gac)
                return new Type[] { };

            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            List<string> assemblyFullnames = new List<string>();
            List<Type> types = new List<Type>();

            if (referenced)
            {            //Check refrenced assemblies
                foreach (AssemblyName assemblyName in currentAssembly.GetReferencedAssemblies())
                {
                    //Load method resolve refrenced loaded assembly
                    Assembly assembly = Assembly.Load(assemblyName.FullName);

                    //Check if type is exists in assembly
                    var type = assembly.GetType(typeName, false, true);

                    if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                    {
                        types.Add(type);
                        assemblyFullnames.Add(assembly.FullName);
                    }
                }
            }

            if (gac)
            {
                //GAC files
                string gacPath = "";// Environment.GetFolderPath(System.Environment.SpecialFolder.Windows) + "\\assembly";
                var files = GetGlobalAssemblyCacheFiles(gacPath);
                foreach (string file in files)
                {
                    try
                    {
                        //reflection only
                        Assembly assembly = Assembly.ReflectionOnlyLoadFrom(file);

                        //Check if type is exists in assembly
                        var type = assembly.GetType(typeName, false, true);

                        if (type != null && !assemblyFullnames.Contains(assembly.FullName))
                        {
                            types.Add(type);
                            assemblyFullnames.Add(assembly.FullName);
                        }
                    }
                    catch
                    {
                        //your custom handling
                    }
                }
            }

            return types.ToArray();
        }

        public static string[] GetGlobalAssemblyCacheFiles(string path)
        {
            List<string> files = new List<string>();

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                files.Add(fi.FullName);
            }

            foreach (DirectoryInfo diChild in di.GetDirectories())
            {
                var files2 = GetGlobalAssemblyCacheFiles(diChild.FullName);
                files.AddRange(files2);
            }

            return files.ToArray();
        }

        static public SplitContainer create_sc(SplitContainer c, bool vert)
        {
            SplitContainer sc = new SplitContainer();
            sc.Dock = DockStyle.Fill;
            sc.Orientation = Orientation.Vertical;

            Control.ControlCollection ic = c.Panel2.Controls;
            foreach (Control b in c.Panel1.Controls)
                c.Controls.Remove(b);
            foreach (Control b in ic)
                sc.Panel1.Controls.Add(b);

            c.Visible = true;

            c.Panel2.Controls.Add(sc);

            return sc;
        }

        static public ToolStrip add_toolstrip(Control c)
        {
            ToolStrip toolStrip2 = new ToolStrip();
            toolStrip2.Items.Add(new ToolStripDropDownButton());
            toolStrip2.Dock = DockStyle.Bottom;
            c.Controls.Add(toolStrip2);

            return toolStrip2;
        }

        static public void save_lb_content(ListBox lb, string file)
        {
            if (File.Exists(file) == false)
                File.Create(file);

            if (lb == null)
                return;

            StringBuilder bb = new StringBuilder();

            int i = 0;
            while (i < lb.Items.Count)
            {
                bb.AppendLine(lb.Items[i].ToString());

                i++;
            }

            File.WriteAllText(file, bb.ToString());
        }

        public static GridItemCollection GetAllGridEntries(PropertyGrid grid)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            object view = grid.GetType().GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(grid);
            return (GridItemCollection)view.GetType().InvokeMember("GetAllGridEntries", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, view, null);
        }

        static public object get_props(PropertyInfo pi, object obs)
        {
            //System.Reflection.PropertyInfo pp = typeof(Control).GetProperty(p);

            //foreach (Control d in L)
            //{
            //    ArrayList C = utils.helpers.getalltabpages((TabControl)d);

            //    foreach (Control cc in C)
            return pi.GetValue(obs, null);

            //}
        }

        static public void get_dir_content(ListBox lb, string p)
        {
            if (Directory.Exists(p) == false)
                Directory.CreateDirectory(p);

            lb.Items.Clear();
            string[] filePaths = Directory.GetFiles(@p, "*.*",
                                         SearchOption.AllDirectories);
            foreach (string s in filePaths)
            {
                if (Directory.Exists(s))
                {
                    get_dir_content(lb, s);
                    return;
                }
                lb.Items.Add(s);
            }
        }

        static public void load_lb_content(ListBox lb, string file)
        {
            if (File.Exists(file) == false)
                File.Create(file);

            if (lb == null)
                return;

            lb.Items.Clear();

            try
            {
                string[] bb = File.ReadAllLines(file);

                int i = 0;
                while (i < bb.Length)
                {
                    lb.Items.Add(bb[i]);

                    i++;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            };
        }

        static public TabPage add_new_tab(TabControl tc)
        {
            TabPage tp = new TabPage();
            tc.TabPages.Add(tp);
            return tp;
        }

        static public ArrayList getalltabcontrols(Control c)
        {
            ArrayList L = new ArrayList();

            Type[] T = utils.helpers.LoadType("System.Windows.Forms.TabControl");

            if (T == null)
                return L;

            if (T.Length <= 0)
                return L;

            L = getallcontrols(T[0], c, L);

            //L.AddRange(L);

            return L;
        }

        static public ArrayList getalltabpages(TabControl c)
        {
            ArrayList L = new ArrayList();

            L.AddRange(c.TabPages);

            return L;
        }

        static public void set_props(ArrayList L, string p, object v)
        {
            System.Reflection.PropertyInfo pp = typeof(Control).GetProperty(p);

            foreach (Control d in L)
            {
                ArrayList C = utils.helpers.getalltabpages((TabControl)d);

                foreach (Control cc in C)
                    pp.SetValue(cc, v, null);
            }
        }

        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(Cursor.Position));
            return null;
        }

        public event EventHandler SomeEvent;

        public static Color bk = Color.Transparent;

        public void OnSomeEvent()
        {
            if (SomeEvent != null) SomeEvent(this, EventArgs.Empty);
        }

        static public void set_event_handler(Control c, string someevent, Color cc, string function)
        {
            pmcont pm = new pmcont();

            EventInfo evt = c.GetType().GetEvent(someevent);
            MethodInfo handler = typeof(utils.helpers).GetMethod(function);
            Delegate del = Delegate.CreateDelegate(
                evt.EventHandlerType, null, handler);

            //MethodInfo handler2 = typeof(utils.pmcont).GetMethod("set_file");

            //Delegate del2 = Delegate.CreateDelegate(
            //    evt.EventHandlerType, null, handler2);

            evt.AddEventHandler(c, del);

            //utils.helpers.bk = cc;

            //obj.OnSomeEvent();
        }

        static public void set_event_handler2(Control c, string someevent, string function, object obj)
        {
            //pmcont pm = new pmcont();

            EventInfo evt = c.GetType().GetEvent(someevent);
            //MethodInfo handler = typeof(utils.helpers).GetMethod(function);
            //Delegate del = Delegate.CreateDelegate(
            //    evt.EventHandlerType, null, handler);

            MethodInfo handler2 = obj.GetType().GetMethod(function);

            Delegate del2 = Delegate.CreateDelegate(
                evt.EventHandlerType, obj, handler2);

            evt.AddEventHandler(c, del2);

            //utils.helpers.bk = cc;

            //obj.OnSomeEvent();
        }

        public static void ColorChanger(object sender, EventArgs args)
        {
            Control c = (Control)sender;
            c.BackColor = utils.helpers.bk;
        }

        public static void ColorChanger2(object sender, EventArgs args)
        {
            Control c = (Control)sender;
            c.BackColor = Color.Transparent;
        }

        public static void ColorChanger3(object sender, EventArgs args)
        {
            Control c = (Control)sender;
            c.BackColor = Color.Transparent;
        }

        static public ArrayList getallcontrols(Type T, Control cc, ArrayList L)
        {
            foreach (Control c in cc.Controls)
            {
                if (c.GetType() == T)
                {
                    L.Add(c);
                }

                getallcontrols(T, c, L);
            }

            return L;
        }

        static public ArrayList getallcontrols(Control c, ArrayList L, TreeView tv, TreeNode node)
        {
            if (node == null)
            {
                node = new TreeNode(c.Name);

                tv.Nodes.Add(node);

                node.Tag = c;

                string s2 = c.Name;
            }

            foreach (Control cc in c.Controls)
            {
                string s = cc.Name;

                if (s == "")
                {
                    s = cc.GetType().FullName;
                }
                TreeNode n = new TreeNode(s);

                node.Nodes.Add(n);

                n.Tag = cc;

                L.Add(cc);

                L = getallcontrols(cc, L, tv, n);
            }

            return L;
        }

        static public ArrayList getallcontrols(Control c, ArrayList L)
        {
            foreach (Control cc in c.Controls)
            {
                L.Add(cc);

                L = getallcontrols(cc, L);
            }

            return L;
        }

        public class fqn
        {
            public string fq { get; set; }

            public Control c { get; set; }

            public fqn(Control cc, string fqq)
            {
                c = cc;
                fq = fqq;
            }

            static public fqn getfqn(ArrayList f, Control c)
            {
                foreach (fqn s in f)
                {
                    if (s.c == c)
                        return s;
                }

                return new fqn(null, "");
            }
        }

        static public ArrayList getallcontrolsfqn(Control c, ArrayList L, fqn fqns)
        {
            string fq = fqns.fq;

            foreach (Control cc in c.Controls)
            {
                string qq = "";

                if (fq != "")
                    qq = fq + "\\";

                fqn g = new fqn(cc, qq + cc.Name);

                L.Add(g);

                L = getallcontrolsfqn(cc, L, g);
            }

            return L;
        }

        static public ArrayList getallcontrols(Control c, ArrayList L, bool r)
        {
            foreach (Control cc in c.Controls)
            {
                L.Add(cc);

                //L = getallcontrols(cc, L);
            }

            return L;
        }

        static public Control getallcontrols2(Type T, Control cc, ArrayList L)
        {
            foreach (Control c in cc.Controls)
            {
                if (c.GetType() == T)
                {
                    L.Add(c);

                    return c;
                }

                Control c2 = getallcontrols2(T, c, L);

                if (c2 != null)
                    return c2;
            }

            return null;
        }

        private static WebBrowser create_new_wb_tab(TabControl tc)
        {
            WebBrowser wb = new WebBrowser();
            wb.Dock = DockStyle.Fill;
            TabPage tp = helpers.add_new_tab(tc);

            ComboBox cb = new ComboBox();

            return wb;
        }

        static public void set_pb(TabPage tb, RichTextBox rt, PictureBox pb, bool pv)
        {
            if (pv == true)
            {
                rt.Visible = false;
                pb.Visible = true;
            }
            else
            {
                rt.Visible = true;
                pb.Visible = false;
            }
        }

        static public Control addControl(Control c, Control p)
        {
            PropertyGrid pg = new PropertyGrid();
            pg.Dock = DockStyle.Fill;

            p.Controls.Add(pg);

            pg.Show();

            return pg;
        }

        static public void get_strs_contents(ListBox lb, string[] g)
        {
            lb.Items.Clear();

            //string[] g = reflect.get_full_names2(a);

            foreach (string s in g)
            {
                lb.Items.Add(s);
            }
        }



        static public string[] get_lb_items(ListBox lb)
        {
            int N = lb.Items.Count;

            if (N <= 0)
                return null;

            string[] p = new string[N];

            int i = 0;
            while (i < N)
                p[i] = lb.Items[i++].ToString();

            return p;
        }


        static public void do_logger_select(ListBox lb, string logs)
        {
            if (lb == null)
                return;

            lb.Items.Add(logs);
        }

        static public void load_file_content(RichTextBox txb, string p)
        {
            txb.Text = "";
            string f = File.ReadAllText(p);
            txb.Text = f;
        }

        static public void load_text_content(RichTextBox txb, string p)
        {
            txb.Text = "";
            //string f = File.ReadAllText(p);
            txb.Text = p;
        }

        public static bool isenum(object objects)
        {
            var t = objects.GetType(); // to be compatible with the question

            bool isIEnumerable = false;
            foreach (var i in t.GetInterfaces())
            {
                if (i == typeof(IEnumerable))
                {
                    isIEnumerable = true;
                    break;
                }
            }

            return isIEnumerable;
        }

        public static string ToHtmlTable<T>(IEnumerable<T> list, string tableSyle, string headerStyle, string rowStyle, string alternateRowStyle, string t)
        {
            var result = new StringBuilder();

            result.AppendLine("<p><H2>Block info 0</H2></p>");

            if (String.IsNullOrEmpty(tableSyle))
            {
                result.Append("<table id=\"" + typeof(T).Name + "Table\">");
            }
            else
            {
                result.Append("<table id=\"" + typeof(T).Name + "Table\" class=\"" + tableSyle + "\">");
            }

            var propertyArray = list.GetType().GetProperties();

            //var propertyArray = typeof(T).GetMembers();

            foreach (var prop in propertyArray)
            {
                if (utils.helpers.isenum(prop))
                {
                    result.Append(utils.helpers.ToHtmlTable<T>(((IEnumerable<T>)prop).ToArray(), "", "", "", "", "block"));
                    continue;
                }
                if (String.IsNullOrEmpty(headerStyle))
                {
                    result.AppendFormat("<th>{0}</th>", prop.Name);
                }
                else
                {
                    result.AppendFormat("<th class=\"{0}\">{1}</th>", headerStyle, prop.Name);
                }
            }

            for (int i = 0; i < list.Count(); i++)
            {
                if (!String.IsNullOrEmpty(rowStyle) && !String.IsNullOrEmpty(alternateRowStyle))
                {
                    result.AppendFormat("<tr class=\"{0}\">", i % 2 == 0 ? rowStyle : alternateRowStyle);
                }
                else
                {
                    result.AppendFormat("<tr>");
                }

                list.ElementAt(0);

                foreach (var prop in propertyArray)
                {
                    if (utils.helpers.isenum(prop))
                    {
                        object value = prop.GetValue(list.ElementAt(i), null);
                        result.AppendFormat("<td>{0}</td>", value ?? String.Empty);
                    }
                    else result.AppendFormat("<td>{0}</td>", list.ElementAt(i).ToString() ?? String.Empty);
                }
                result.AppendLine("</tr>");
            }

            result.Append("</table>");

            return result.ToString();
        }

        public static string ToHtmlTable_fromAL(ArrayList list, ArrayList reg, string tableSyle, string headerStyle, string rowStyle, string alternateRowStyle, string info)
        {
            var result = new StringBuilder();

            result.AppendLine("<p><H2>" + info + "</H2></p>");

            //int id = 0;

            //if (String.IsNullOrEmpty(tableSyle))
            {
                result.AppendLine("<table class=\"tg\">");

                result.Append("<tr>");
            }

            int c = 0;

            foreach (var p in list)
            {
                if (utils.helpers.isenum(p) == false)
                {
                }
                else

                    if (String.IsNullOrEmpty(headerStyle))
                {
                    result.Append("<td  class=\"tg-031e\">" + (string)reg[c] + "</td>");
                }
                else
                {
                    // result.AppendFormat("<th  class=\"tg-031e\">{1}</th>", headerStyle, "m");
                }

                c++;
            }

            result.Append("</tr>");

            ArrayList L2 = (ArrayList)list[0];

            int C = L2.Count;

            for (int i = 0; i < C; i++)
            {
                //ArrayList B = (ArrayList) list[i];

                result.AppendFormat("\n<tr  class=\"tg-031e\">");

                for (int j = 0; j < list.Count; j++)
                {
                    ArrayList B = (ArrayList)list[j];

                    var r2 = (uint)B[i];

                    if (!String.IsNullOrEmpty(rowStyle) && !String.IsNullOrEmpty(alternateRowStyle))
                    {
                        // result.AppendFormat("<td  class=\"tg-031e\">", i%2 == 0 ? rowStyle : alternateRowStyle);
                    }
                    else
                    {
                        //result.AppendFormat("<td  class=\"tg-031e\">");
                    }

                    //foreach (var ros in r2)
                    {
                        result.AppendFormat("<td class=\"tg-031e\">{0}</td>", r2.ToString() ?? String.Empty);
                    }
                }

                result.AppendLine("</tr>");
            }
            result.Append("</table>");

            return result.ToString();
        }

        public class layouts
        {
            private ArrayList _L = new ArrayList();

            public void register(Control p)
            {
                p.Resize += new System.EventHandler(resize);
            }

            public void load(Control cc)
            {
                if (_L == null)
                    _L = new ArrayList();

                _L.Add(cc);
            }

            public void resize(object sender, EventArgs e)
            {
                Control p = (Control)sender;

                int N = _L.Count;

                int w1 = p.ClientSize.Width;
                int h1 = p.ClientSize.Height;

                int w = (w1 / N) - 8;
                int h = h1 - 10;

                int d = 0;

                foreach (Control c in _L)
                {
                    c.Location = new Point(d, 5);
                    c.Size = new Size(w, h);
                    d += w + 2;
                }
            }
        }
    }

    public class sshc
    {
    }

    public class pmcont : object
    {
        public string file = "";

        public TextBox tb = null;

        public Control c = null;

        public Buffer b = new Buffer();

        public void add_to_Buffer(string s, int i)
        {
            if (b == null)
                return;

            if (b.buffer == null)
                b.init();

            b.add_to_Buffer(s, i);
        }

        public void add_to_Buffer(string[] s)
        {
            if (b == null)
                return;

            if (b.buffer == null)
                b.init();

            b.add_to_Buffer(s);
        }

        public void reload()
        {
            ListBox lb = (ListBox)c;

            utils.helpers.get_dir_content(lb, @"Files");
        }

        public void FileChanged(object sender, EventArgs args)
        {
            Control c = (Control)sender;

            ListBox lb = (ListBox)c;

            //c.BackColor = Color.Transparent;

            int index = lb.SelectedIndex;

            if (index < 0)
                return;

            string f = lb.Items[index].ToString();

            set_file(f);
        }

        public void set_filer(TextBox _tb)
        {
            tb = _tb;
        }

        public void set_file(string s)
        {
            if (tb == null)
                return;

            tb.Text = s;
        }

        public void register(Control _c)
        {
            c = _c;

            //tb = (TextBox)c;

            utils.helpers.set_event_handler2(c, "SelectedIndexChanged", "FileChanged", this);
        }
    }

    public class fmcont
    {
        public pmcont p0 = null;

        public pmcont p1 = null;

        public void function_Copy()
        {
        }
    }

    public class Buffer
    {
        public ArrayList buffer = null;

        public ArrayList indices = null;

        public void init()
        {
            buffer = new ArrayList();
            indices = new ArrayList();
        }

        public void add(object a)
        {
            if (buffer == null)
                buffer = new ArrayList();
            buffer.Add(a);
        }

        public void add(object[] a)
        {
            if (buffer == null)
                buffer = new ArrayList();
            buffer.AddRange(a);
        }

        public void add_to_Buffer(string s, int i)
        {
            if (indices.IndexOf(i) < 0)
            {
                buffer.Add(s);
                indices.Add(i);
            }
            else
            {
                buffer.Remove(s);
                indices.Remove(i);
            }

            indices.Sort();
        }

        public void add_to_Buffer(string[] s)
        {
            buffer.Clear();
            indices.Clear();

            int i = 0;

            foreach (string p in s)
            {
                buffer.Add(p);
                indices.Add(i);
                i++;
            }

            indices.Sort();
        }
    }

    public class chunker
    {
        static public void dochunks()
        {
            String[] arrayString = new string[] { "", "", "", "", "" };
            List<string[]> splitted = new List<string[]>();//This list will contain all the splitted arrays.
            int lengthToSplit = 3;

            int arrayLength = arrayString.Length;

            for (int i = 0; i < arrayLength; i = i + lengthToSplit)
            {
                string[] val = new string[lengthToSplit];

                if (arrayLength < i + lengthToSplit)
                {
                    lengthToSplit = arrayLength - i;
                }
                Array.Copy(arrayString, i, val, 0, lengthToSplit);
                splitted.Add(val);
            }
        }

        static public void dochunks(string inputFile, string outputFile)
        {
            byte[] sourceBuffer = File.ReadAllBytes(inputFile);
            byte[] destBuffer = new byte[sourceBuffer.Length];
            byte[] toEncrypt = new byte[16];
            // Encrypt each block.
            for (int offset = 0; offset < sourceBuffer.Length; offset += 16)
            {
                Array.Copy(sourceBuffer, offset, toEncrypt, 0, 16); // copying from the source into 16 byte array size
                Array.Copy(destBuffer/*EncryptByte(toEncrypt)*/, 0, destBuffer, offset, 16);
            }
            System.IO.File.WriteAllBytes(outputFile, destBuffer);
        }

        public byte[][] LargeByteArrayToChunks(byte[] original_data, int max_chunk_size)
        {
            List<byte> buffer = new List<byte>(original_data);
            List<byte[]> result = new List<byte[]>();

            while (buffer.Count > max_chunk_size)
            {
                result.Add(buffer.GetRange(0, max_chunk_size).ToArray());
                buffer.RemoveRange(0, max_chunk_size);
            }

            if (buffer.Count > 0) // remember to check if there is any residual
                result.Add(buffer.ToArray());

            return result.ToArray();
        }
    }

    public interface ICommand
    {
        void cmd_CreateCmd();

        void cmd_ExecuteCmd();
    }

    public class cmd_CreateNewDir : ICommand
    {
        public void cmd_CreateCmd()
        {
        }

        public void cmd_ExecuteCmd()
        {
        }
    }

    public class mvc_sel
    {
        public ArrayList F = null;

        public ArrayList T = null;

        static public void unget_sp()
        {
        }

        internal class gui
        {
            private ListBox _fb = null;

            private ListBox _tb = null;
        }
    }

    public class mvc_panels
    {
        public ArrayList P = null;
    }

    public class Placer
    {
        private static int FirstClickDownPosX { get; set; }

        private static int FirstClickDownPosY { get; set; }

        private static int MouseDragToPosX { get; set; }

        private static int MouseDragToPosY { get; set; }

        private static Control RepositionableObject { get; set; }

        private void MoveWithCursor_MouseDown(object sender, MouseEventArgs e)
        {
            FirstClickDownPosX = e.X;
            FirstClickDownPosY = e.Y;
        }

        private void MoveWithCursor_MouseMove(object sender, MouseEventArgs e)
        {
            MouseDragToPosX = e.X;
            MouseDragToPosY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                RepositionableObject.Left += (MouseDragToPosX - FirstClickDownPosX);
                RepositionableObject.Top += (MouseDragToPosY - FirstClickDownPosY);
            }
        }

        internal void MoveWithCursor(Control newObject)
        {
            RepositionableObject = newObject;
            RepositionableObject.MouseDown += new MouseEventHandler(MoveWithCursor_MouseDown);
            RepositionableObject.MouseMove += new MouseEventHandler(MoveWithCursor_MouseMove);
        }
    }

    public class Tooler
    {
        public ToolStrip ts = null;

        public void createToolStrip(Control c)
        {
            ts = new ToolStrip();

            ts.Dock = DockStyle.Top;

            ts.LayoutStyle = ToolStripLayoutStyle.Table;
            var layoutSettings = (ts.LayoutSettings as TableLayoutSettings);
            layoutSettings.ColumnCount = 3;
            layoutSettings.RowCount = 3;

            c.Controls.Add(ts);

            for (int i = 0; i < 9; i++)
            {
                var item = new ToolStripMenuItem("a" + i.ToString());
                ts.Items.Add(item);
            }

            ts.Show();
        }

        private ArrayList _tabControls = null;

        public void createtabber(Control p, Control c)
        {
            ArrayList L = utils.helpers.getalltabcontrols(p);

            _tabControls = new ArrayList();

            foreach (TabControl b in L)
            {
                ToolStrip d = utils.helpers.add_toolstrip(c);
                d.Tag = b;

                _tabControls.Add(d);
            }

            foreach (ToolStrip d in _tabControls)
            {
                TabControl e = (TabControl)d.Tag;

                ArrayList g = utils.helpers.getalltabpages(e);

                foreach (TabPage t in g)
                {
                    ToolStripButton b = toolbutton(t.Text);
                    b.Tag = t;
                    d.Items.Add(b);
                }
            }
        }

        public ToolStripButton toolbutton(string text)
        {
            ToolStripButton b = new ToolStripButton(text);
            b.Click += new EventHandler(tabpage_click);

            return b;
        }

        public void tabpage_click(object sender, EventArgs args)
        {
            ToolStripButton b = (ToolStripButton)sender;

            MessageBox.Show("ToolStripButton clicked!");
        }
    }
}
namespace SolutionFormProject
{
    public class GroupBoxEx : GroupBox
    {
        public Button b { get; set; }

        public bool expanded = true;

        private SizeF _sizeOfText;

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            CalculateTextSize();
        }

        public Size exp { get; set; }

        public Point prev { get; set; }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalculateTextSize();
        }

        protected void CalculateTextSize()
        {
            // measure the string:
            using (Graphics g = this.CreateGraphics())
            {
                _sizeOfText = g.MeasureString(Text, Font);
            }
            _linePen = new Pen(SystemColors.ButtonShadow);// new Pen(Color.FromArgb(86, 136, 186), sizeOfText.Height * 0.1F);
        }

        private Pen _linePen;

        protected override void OnPaint(PaintEventArgs e)
        {
            //this.Padding = new Padding(20, 0, 0, 0);
            // base.OnPaint(e);
            //return;
            //;
            //if (b != null)
            //{
            //    e.Graphics.DrawRectangle(Pens.Green, this.Location.X, this.Location.Y, 15, 15);
            //    this.Controls.SetChildIndex(b, 0);
            //    b.Size = new Size(600, 600);
            //    b.Location = new Point(this.Location.X, this.Location.Y);
            //    b.Refresh();
            //}

            // Draw the string, we now have complete control over where:

            Rectangle r = new Rectangle(ClientRectangle.Left,  // + Margin.Left,
                ClientRectangle.Top + Margin.Top,
                ClientRectangle.Width - Margin.Left - Margin.Right + 3,
                ClientRectangle.Height - Margin.Top - Margin.Bottom);

            const int gapInLine = 0;
            const int textMarginLeft = 25, textMarginTop = -1;

            // Top line:
            e.Graphics.DrawLine(_linePen, r.Left + 2, r.Top + 3, r.Left + textMarginLeft - gapInLine, r.Top + 3);
            e.Graphics.DrawLine(_linePen, r.Left + textMarginLeft + _sizeOfText.Width, r.Top + 3, r.Right, r.Top + 3);

            // Now, draw the string at the desired location:
            e.Graphics.DrawString(Text, Font, Brushes.Black, new Point(this.ClientRectangle.Left + textMarginLeft, this.ClientRectangle.Top - textMarginTop));

            if (expanded == false)
                return;

            // and so on...
            e.Graphics.DrawLine(_linePen, r.Left, r.Top + 3 + 10, r.Left, r.Bottom - 1);
            e.Graphics.DrawLine(_linePen, r.Right, r.Top + 3, r.Right, r.Bottom - 1);
            e.Graphics.DrawLine(_linePen, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
        }
    }

    public class Attributes : Attribute
    {
        public string SomeProperty { get; set; }
    }

    public class utils
    {
        //public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
        //{
        //    var atts = provider.GetCustomAttributes(typeof(T), true);
        //    return atts.Length > 0;
        //}

        [Attributes]
        static public void LoadListView(ListView listview)
        {
            MethodInfo[] me = (MethodInfo[])typeof(utils).GetMethods().Where(p => p.GetCustomAttributes(typeof(Attributes), true).Length != 0).ToArray();

            //listview = new ListView();

            listview.Dock = DockStyle.Fill;

            listview.View = View.Details;

            listview.FullRowSelect = true;

            listview.Columns.Add("Name");

            listview.Columns.Add("Value");

            //splitContainer1.Panel2.Controls.Add(listview);
        }

        static public void SolutionParser(string paths, TreeView ts, ListView listview)
        {
            ts.Nodes.Clear();

            //var solution = Onion.SolutionParser.Parser.SolutionParser.Parse(paths);

            //foreach (Onion.SolutionParser.Parser.Model.GlobalSection s in solution.Global)
            //{
            //    TreeNode node = new TreeNode();
            //    node.Text = s.Name;

            //    ts.Nodes.Add(node);

            //    foreach (string key in s.Entries.Keys)
            //    {
            //        string v = s.Entries[key];

            //        TreeNode b = new TreeNode();
            //        b.Text = key + "  " + v;

            //        node.Nodes.Add(b);
            //    }
            //}

            VSSolution vs = new VSSolution(Path.GetFullPath(paths));

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
                    ns.Tag = pv;

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

        public static void SelectProject(string s, ListView tv)
        {
            tv.Items.Clear();

            Microsoft.Build.Evaluation.Project pcs = new Microsoft.Build.Evaluation.Project(s);

            foreach (Microsoft.Build.Evaluation.ProjectProperty pp in pcs.AllEvaluatedProperties)
            {
                string pv = pcs.GetPropertyValue(pp.Name);

                //TreeNode ns = new TreeNode();
                //ns.Text = pp.Name + "  " + pv;
                //ns.Tag = pv;

                ListViewItem v = new ListViewItem();
                v.Text = pp.Name;
                v.SubItems.Add(pv);

                tv.Items.Add(v);

                //nodes.Nodes.Add(ns);
            }
        }

        public class VSSolution
        {
            //internal class SolutionParser
            //Name: Microsoft.Build.Construction.SolutionParser
            //Assembly: Microsoft.Build, Version=4.0.0.0

            private static readonly Type s_SolutionParser;
            private static readonly MethodInfo s_SolutionParser_parseSolution;
            private static readonly PropertyInfo s_SolutionParser_projects;
            private static readonly PropertyInfo s_SolutionParser_solutionReader;
            private List<VSProject> _projects;
            private string _solutionFileName;

            static VSSolution()
            {
                s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
                s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            public VSSolution(string solutionFileName)
            {
                if (s_SolutionParser == null)
                {
                    throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
                }

                var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

                using (var streamReader = new StreamReader(solutionFileName))
                {
                    s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                    s_SolutionParser_parseSolution.Invoke(solutionParser, null);
                }

                _solutionFileName = solutionFileName;

                _projects = new List<VSProject>();
                var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);

                for (int i = 0; i < array.Length; i++)
                {
                    _projects.Add(new VSProject(this, array.GetValue(i)));
                }
            }

            public string Name
            {
                get
                {
                    return Path.GetFileNameWithoutExtension(_solutionFileName);
                }
            }

            public IEnumerable<VSProject> Projects
            {
                get
                {
                    return _projects;
                }
            }

            public string SolutionPath
            {
                get
                {
                    var file = new FileInfo(_solutionFileName);

                    return file.DirectoryName;
                }
            }

            public void Dispose()
            {
            }
        }

        //[DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
        public class VSProject
        {
            private static readonly Type s_ProjectInSolution;
            private static readonly Type s_RootElement;
            private static readonly Type s_ProjectRootElement;
            private static readonly Type s_ProjectRootElementCache;
            private static readonly PropertyInfo s_ProjectInSolution_ProjectName;
            private static readonly PropertyInfo s_ProjectInSolution_ProjectType;
            private static readonly PropertyInfo s_ProjectInSolution_RelativePath;
            private static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
            private static readonly PropertyInfo s_ProjectRootElement_Items;

            private VSSolution _solution;
            private string _projectFileName;
            private object _internalSolutionProject;
            private List<VSProjectItem> _items;

            public string Name { get; private set; }

            public string ProjectType { get; private set; }

            public string RelativePath { get; private set; }

            public string ProjectGuid { get; private set; }

            public string FileName
            {
                get
                {
                    return _projectFileName;
                }
            }

            static VSProject()
            {
                s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

                s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);

                s_ProjectRootElement = Type.GetType("Microsoft.Build.Construction.ProjectRootElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
                s_ProjectRootElementCache = Type.GetType("Microsoft.Build.Evaluation.ProjectRootElementCache, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

                s_ProjectRootElement_Items = s_ProjectRootElement.GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);
            }

            public IEnumerable<VSProjectItem> Items
            {
                get
                {
                    return _items;
                }
            }

            public VSProject(VSSolution solution, object internalSolutionProject)
            {
                this.Name = s_ProjectInSolution_ProjectName.GetValue(internalSolutionProject, null) as string;
                this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(internalSolutionProject, null).ToString();
                this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(internalSolutionProject, null) as string;
                this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(internalSolutionProject, null) as string;

                _solution = solution;
                _internalSolutionProject = internalSolutionProject;

                _projectFileName = Path.Combine(solution.SolutionPath, this.RelativePath);

                _items = new List<VSProjectItem>();

                if (this.ProjectType == "KnownToBeMSBuildFormat")
                {
                    this.Parse();
                }
            }

            private void Parse()
            {
                try
                {
                    //var stream = File.OpenRead(projectFileName);
                    //var reader = XmlReader.Create(stream);
                    //var cache = s_ProjectRootElementCache.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { true });
                    //ConstructorInfo[] inf = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    //var rootElement = s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { reader, cache, true });

                    //stream.Close();

                    //var collection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);

                    //foreach (var item in collection)
                    //{
                    //    items.Add(new VSProjectItem(this, item));
                    //}
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            public IEnumerable<VSProjectItem> EDMXModels
            {
                get
                {
                    return _items.Where(i => i.ItemType == "EntityDeploy");
                }
            }

            public void Dispose()
            {
            }
        }

        // [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
        public class VSProjectItem
        {
            private static readonly Type s_ProjectItemElement;
            private static readonly PropertyInfo s_ProjectItemElement_ItemType;
            private static readonly PropertyInfo s_ProjectItemElement_Include;

            private VSProject _project;
            private object _internalProjectItem;
            public string fileName;

            static VSProjectItem()
            {
                s_ProjectItemElement = Type.GetType("Microsoft.Build.Construction.ProjectItemElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

                s_ProjectItemElement_ItemType = s_ProjectItemElement.GetProperty("ItemType", BindingFlags.Public | BindingFlags.Instance);
                s_ProjectItemElement_Include = s_ProjectItemElement.GetProperty("Include", BindingFlags.Public | BindingFlags.Instance);
            }

            public string ItemType { get; private set; }

            public string Include { get; private set; }

            public VSProjectItem(VSProject project, object internalProjectItem)
            {
                this.ItemType = s_ProjectItemElement_ItemType.GetValue(internalProjectItem, null) as string;
                this.Include = s_ProjectItemElement_Include.GetValue(internalProjectItem, null) as string;
                _project = project;
                _internalProjectItem = internalProjectItem;

                // todo - expand this

                if (this.ItemType == "Compile" || this.ItemType == "EntityDeploy")
                {
                    var file = new FileInfo(project.FileName);

                    fileName = Path.Combine(file.DirectoryName, this.Include);
                }
            }

            public byte[] FileContents
            {
                get
                {
                    return File.ReadAllBytes(fileName);
                }
            }

            public string Name
            {
                get
                {
                    if (fileName != null)
                    {
                        var file = new FileInfo(fileName);

                        return file.Name;
                    }
                    else
                    {
                        return this.Include;
                    }
                }
            }
        }
    }
}