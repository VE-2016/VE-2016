using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GACManagerApi;

namespace GACProject
{
    public partial class GACForm : Form
    {
        public GACForm()
        {
            InitializeComponent();

            //GetAssembliesList("");


            //var parent = Registry.ClassesRoot.OpenSubKey("CLSID");

            //var parent = Registry.ClassesRoot.OpenSubKey("TypeLib");


            //var subKeys = parent.GetSubKeyNames();
            //foreach (var subKey in subKeys)
            //{

            //    var sub = parent.OpenSubKey(subKey);
            //    if (sub != null)
            //    {


            //        GetSubKeys(sub, "win32");

            //        //var inProc = sub.GetSubKeyNames();
            //        //if (inProc != null)
            //        //{
            //        //    var val = inProc[0];

            //        //    var b = sub.OpenSubKey(val);
            //        //    if (b != null)
            //        //    {
            //        //        var name = b.GetValue(null).ToString();
            //        //        if (!string.IsNullOrWhiteSpace(name))
            //        //            Console.WriteLine(name);
            //        //    }
            //        //}
            //    }
            //}


            GetAssembliesList("");

            sp = splitContainer1;

            //sp.Panel1Collapsed = true;

            lv = listView1;

            lv.SelectedIndexChanged += new EventHandler(SelectedIndexChanged);

            LoadTreeView();

            GetFrameworks();
        }

        private static ArrayList C { get; set; }

        private static void GetSubKeys(RegistryKey SubKey, string b, string name)
        {
            foreach (string sub in SubKey.GetSubKeyNames())
            {
                // MessageBox.Show(sub);
                // RegistryKey local = Registry.Users;
                RegistryKey local = SubKey.OpenSubKey(sub);
                if (local.GetValue(null) != null)
                {
                    var v = local.GetValue(null);

                    if (sub.ToString() == b)
                    {
                        AssemblyDescription asd = null;

                        string c = local.Name.ToString();

                        //RegistryKey r = local.OpenSubKey(local.GetValue(null).ToString());

                        //if(v.ToString().EndsWith(".dll") == true)

                        asd = new GACManagerApi.AssemblyDescription(v.ToString(), "", true);

                        asd.Name = name;

                        C.Add(asd);
                    }
                }
                GetSubKeys(local, b, name); // By recalling itself it makes sure it get all the subkey names
            }
        }

        private static ArrayList E { get; set; }

        public TreeViews tv { get; set; }

        public void LoadTreeView()
        {
            tv = treeView1;

            tv.ShowLines = false;

            tv.FullRowSelect = true;

            TreeNode node = new TreeNode();
            node.Text = "Assemblies";

            TreeNode n = new TreeNode();
            n.Text = "Framework";
            node.Nodes.Add(n);

            n = new TreeNode();
            n.Text = "Extensions";
            node.Nodes.Add(n);

            n = new TreeNode();
            n.Text = "Recent";
            node.Nodes.Add(n);

            tv.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "Solution";

            n = new TreeNode();
            n.Text = "Projects";
            node.Nodes.Add(n);

            tv.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "COM";

            n = new TreeNode();
            n.Text = "TypeLibraries";
            node.Nodes.Add(n);

            tv.Nodes.Add(node);


            node = new TreeNode();
            node.Text = "Browse";

            n = new TreeNode();
            n.Text = "Recent";
            node.Nodes.Add(n);

            tv.Nodes.Add(node);
        }

        public void GetFrameworks()
        {
            FindFrameworks();

            FilterAssemblies();
        }


        public void FilterTypeLibraries()
        {
            GetTLBs();

            // LoadTypeLibraries(asm);
        }

        public void LoadDefaults()
        {
            GetAssemblies();

            FilterAssemblies();
        }

        public static ArrayList GetTLB()
        {
            C = new ArrayList();

            var parent = Registry.ClassesRoot.OpenSubKey("TypeLib");


            var subKeys = parent.GetSubKeyNames();
            foreach (var subKey in subKeys)
            {
                string v = "";

                var sub = parent.OpenSubKey(subKey);
                if (sub != null)
                {
                    string[] sc = sub.GetSubKeyNames();
                    if (sc.Length > 0)
                    {
                        var k = sub.OpenSubKey(sc[0]);
                        object obs = k.GetValue(null);

                        if (obs != null)
                            v = obs.ToString();
                    }

                    sub.Close();
                }
                sub = parent.OpenSubKey(subKey);
                if (sub != null)
                {
                    GetSubKeys(sub, "win32", v);
                    GetSubKeys(sub, "win64", v);
                }
            }
            return C;
        }


        public SplitContainer sp { get; set; }

        private ListView lv { get; set; }

        // public LinkerForms.PanelControl pc { get; set; }


        private GACManagerApi.AssemblyDescription _d;





        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetAssemblies();

            //LoadAssemblies();
        }

        public ArrayList asm { get; set; }

        public ArrayList A { get; set; }

        public void LoadAssemblies(ArrayList asm)
        {
            InitializeListView(lv);

            A = new ArrayList();

            foreach (object obs in asm)
            {
                if (obs == null)
                    continue;



                GACManagerApi.AssemblyDescription ase = obs as GACManagerApi.AssemblyDescription;

                if (ase.Name == null)
                    continue;


                if (A.IndexOf(ase.Name) >= 0)
                    continue;
                else
                    A.Add(ase.Name);

                ListViewItem v = new ListViewItem();
                v.Text = "";

                lv.Columns[0].TextAlign = HorizontalAlignment.Center;

                v.SubItems.Add(ase.Name.ToString());




                v.SubItems.Add(ase.Path);

                v.SubItems.Add(ase.ProcessorArchitecture);




                // v.SubItems.Add(ase.ReflectionProperties.ToString());

                v.SubItems.Add(ase.Version);

                v.SubItems.Add(ase.DisplayName.ToString());

                lv.Items.Add(v);
            }
        }



        public void LoadTypeLibraries(ArrayList asm)
        {
            InitializeListView(lv);

            A = new ArrayList();

            foreach (object obs in asm)
            {
                if (obs == null)
                    continue;

                GACManagerApi.AssemblyDescription ase = obs as GACManagerApi.AssemblyDescription;

                if (A.IndexOf(ase.Name) >= 0)
                    continue;
                else
                    A.Add(ase.Name);

                if (ase.ProcessorArchitecture != "" && ase.ProcessorArchitecture != null)
                    continue;

                ListViewItem v = new ListViewItem();
                v.Text = "";

                lv.Columns[0].TextAlign = HorizontalAlignment.Center;

                v.SubItems.Add(ase.Name.ToString());

                v.SubItems.Add(ase.Path);

                v.SubItems.Add(ase.ProcessorArchitecture);

                // v.SubItems.Add(ase.ReflectionProperties.ToString());

                v.SubItems.Add(ase.Version);

                //v.SubItems.Add(ase.DisplayName.ToString());

                lv.Items.Add(v);
            }
        }


        public void InitializeListView(ListView lv)
        {
            lv.Clear();

            lv.View = View.Details;


            lv.CheckBoxes = true;

            lv.FullRowSelect = true;


            lv.Columns.Add("");
            lv.Columns.Add("Name");
            lv.Columns.Add("Path");
            lv.Columns.Add("Arch");
            // lv.Columns.Add("Refl");
            lv.Columns.Add("Version");
            lv.Columns.Add("Display");
        }


        public void InitializeListView(ListView lv, string project)
        {
            lv.Clear();

            lv.View = View.Details;


            lv.CheckBoxes = true;

            lv.FullRowSelect = true;


            lv.Columns.Add("");
            lv.Columns.Add("Name");
            lv.Columns.Add("Path");
        }


        public void SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (pc == null)
            //    return;

            if (lv.SelectedIndices == null)
                return;

            if (lv.SelectedIndices.Count <= 0)
                return;

            int i = lv.SelectedIndices[0];

            string s = "";


            ListViewItem v = lv.Items[i];

            s = v.SubItems[1].Text;

            //pc.teb.Text = s;

            //s = Path.GetDirectoryName(s);

            //pc.tea.Text = s;
        }

        public static Dictionary<string, string> dicts { get; set; }

        public static Dictionary<string, string> tlds { get; set; }

        public static Dictionary<string, AssemblyDescription> asmd { get; set; }

        public static ArrayList asms { get; set; }

        async static public Task<ArrayList> GetAssembliesList()
        {
            asms = await utils.EnumerateAssemblies();

            asmd = new Dictionary<string, AssemblyDescription>();

            foreach (AssemblyDescription d in asms)
            {
                asmd.Add(d.Name, d);
            }

            dicts = GetAsmDict(asms);

            GetTLBList();

            return asms;
        }

        public static ArrayList frameworks { get; set; }

        async static public Task<ArrayList> GetAssembliesList(string s)
        {
            frameworks = GacUtil.GetFrameworks();

            asms = await GacUtil.GetFramework(s);

            asmd = new Dictionary<string, AssemblyDescription>();

            foreach (AssemblyDescription d in asms)
            {
                if (d.Name != null)
                    asmd.Add(d.Name, d);
            }

            dicts = GetAsmDict(asms);

            return asms;
        }

        public static string GetFrameworkDoc(string assembly, string data, string prefix = "")
        {
            if (asmd == null)
                return "";

            if (asmd.ContainsKey(assembly) == false)
                return "";

            AssemblyDescription d = asmd[assembly];

            string doc = d.GetDocument(data, prefix);

            return doc;
        }

        public static Dictionary<string, AssemblyDescription> tlb { get; set; }

        async static public Task<ArrayList> GetTLBList()
        {
            ArrayList asms = GetTLB();

            tlb = new Dictionary<string, AssemblyDescription>();

            foreach (AssemblyDescription d in asms)
            {
                if (d.Name != null)
                    tlb.Add(d.Name, d);
            }

            dicts = GetTLBDict(asms);

            return asms;
        }

        //async static public Task<ArrayList> GetExtensionsList()
        //{

        //    ArrayList asms = GetExtension();

        //    tlb = new Dictionary<string, AssemblyDescription>();

        //    foreach (AssemblyDescription d in asms)
        //    {
        //        if (d.Name != null)
        //            tlb.Add(d.Name, d);
        //    }

        //    dicts = GetTLBDict(asms);

        //    return asms;

        //}

        static public Dictionary<string, string> GetAsmDict(ArrayList L)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (GACManagerApi.AssemblyDescription ase in L)
            {
                if (ase == null)
                    continue;

                if (ase.Name == null)
                    continue;


                if (dict.ContainsKey(ase.Name))
                    continue;
                try
                {
                    if (ase.Name != null)
                        if (ase.Path != null)

                            dict.Add(ase.Name, ase.Path);
                }
                catch (Exception e)
                {
                }
            }

            return dict;
        }
        static public Dictionary<string, string> GetTLBDict(ArrayList L)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (GACManagerApi.AssemblyDescription ase in L)
            {
                if (ase == null)
                    continue;

                if (ase.Name == null)
                    continue;


                if (dict.ContainsKey(ase.Name))
                    continue;
                try
                {
                    if (ase.Name != null)
                        if (ase.Path != null)

                            dict.Add(ase.Name, ase.Path);
                }
                catch (Exception e)
                {
                }
            }

            return dict;
        }

        async public void GetAssemblies()
        {
            if (GACForm.asms != null)
            {
                asm = asms;
            }
            else asm = await utils.EnumerateAssemblies();

            LoadAssemblies(asm);
        }
        async public void GetExtensions()
        {
            if (GACForm.E != null)
            {
                // asm = asms;

            }
            else E = await GacUtil.GetExtensions("");

            LoadAssemblies(E);
        }
        async public void GetTLBs()
        {
            if (GACForm.C != null)
            {
                //asm = asms;

            }
            else C = GetTLB();

            LoadTypeLibraries(C);
        }

        //async public  void GetExtension()
        //{

        //    if (GACForm.E != null)
        //    {


        //    }
        //    else E = GetExtensions();

        //    LoadTypeLibraries(C);

        //}

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            LoadAssemblies(asm);
        }

        private ArrayList L { get; set; }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            L = new ArrayList();

            var ac = new AssemblyCacheEnumerator();

            int i = 0;
            while (i < 2000)
            {
                GACManagerApi.Fusion.IAssemblyName name = ac.GetNextAssembly();

                if (name == null)
                    break;

                //  Get the first assembly in the global assembly cache.
                var someAssembly = new AssemblyDescription(name);

                if (someAssembly == null)
                    break;

                if (someAssembly.Name == null)
                    break;

                //  Show the Runtime Version.
                System.Diagnostics.Trace.WriteLine(someAssembly.ReflectionProperties.RuntimeVersion);
                //richTextBox1.AppendText("\n\n\n" + someAssembly.ReflectionProperties.RuntimeVersion);
                //richTextBox1.AppendText("\n\n\n" + someAssembly.ReflectionProperties.ToString());

                int p = L.IndexOf(someAssembly.ReflectionProperties.RuntimeVersion);

                if (p < 0)
                    L.Add(someAssembly.ReflectionProperties.RuntimeVersion);

                i++;
            }

            ToolStripComboBox c = toolStripComboBox1;

            c.DropDownStyle = ComboBoxStyle.DropDownList;

            c.Items.Clear();

            foreach (string s in L)
                c.Items.Add(s);

            if (c.Items.Count > 0)
                c.SelectedIndex = c.Items.Count - 1;
        }

        private void FindFrameworks()
        {
            if (frameworks == null || frameworks.Count < 0)
            {
                L = new ArrayList();

                var ac = new AssemblyCacheEnumerator();

                int i = 0;
                while (i < 2000)
                {
                    GACManagerApi.Fusion.IAssemblyName name = ac.GetNextAssembly();

                    if (name == null)
                        break;

                    //  Get the first assembly in the global assembly cache.
                    var someAssembly = new AssemblyDescription(name);

                    if (someAssembly == null)
                        break;

                    if (someAssembly.Name == null)
                        break;

                    //  Show the Runtime Version.
                    System.Diagnostics.Trace.WriteLine(someAssembly.ReflectionProperties.RuntimeVersion);
                    //richTextBox1.AppendText("\n\n\n" + someAssembly.ReflectionProperties.RuntimeVersion);
                    //richTextBox1.AppendText("\n\n\n" + someAssembly.ReflectionProperties.ToString());

                    int p = L.IndexOf(someAssembly.ReflectionProperties.RuntimeVersion);

                    if (p < 0)
                        L.Add(someAssembly.ReflectionProperties.RuntimeVersion);

                    i++;
                }
            }

            else
            {
                L = new ArrayList();

                foreach (DirectoryInfo d in frameworks)

                    L.Add(d.Name);
            }
            ToolStripComboBox c = toolStripComboBox1;

            c.DropDownStyle = ComboBoxStyle.DropDownList;

            c.Items.Clear();

            foreach (string s in L)
                c.Items.Add(s);

            if (c.Items.Count > 0)
                c.SelectedIndex = c.Items.Count - 1;
        }


        private ArrayList F { get; set; }

        public void FilterAssemblies()
        {
            F = new ArrayList();


            int p = toolStripComboBox1.SelectedIndex;

            if (p < 0)
                return;



            string filtered = toolStripComboBox1.Items[p].ToString();

            if (asm == null)
                return;

            foreach (AssemblyDescription d in asm)
            {
                if (d.ReflectionProperties.RuntimeVersion == filtered)

                    F.Add(d);
            }


            LoadAssemblies(F);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            FilterAssemblies();
        }

        public static void GetFrameworkVersion()
        {
            Dictionary<int, String> mappings = new Dictionary<int, string>();

            mappings[378389] = "4.5";
            mappings[378675] = "4.5.1 on Windows 8.1";
            mappings[378758] = "4.5.1 on Windows 8, Windows 7 SP1, and Vista";
            mappings[379893] = "4.5.2";
            mappings[393295] = "4.6 on Windows 10";
            mappings[393297] = "4.6 on Windows not 10";

            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    Console.WriteLine("Version: " + mappings[releaseKey]);
                }
            }
            int a = Console.Read();
        }

        public ArrayList N { get; set; }

        private void GetVersionFromRegistry()
        {
            N = new ArrayList();

            // Opens the registry key for the .NET Framework entry. 
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5  
                // or later, you can use: 
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,  
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            Console.WriteLine(versionKeyName + "  " + name);
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                Console.WriteLine(versionKeyName + "  " + name + "  SP" + sp);
                            }
                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "")
                            { //no install info, must be later.
                                Console.WriteLine(versionKeyName + "  " + name);
                                N.Add(versionKeyName + "  " + name);
                            }
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name + "  SP" + sp);
                                    N.Add("  " + subKeyName + "  " + name + "  SP" + sp);
                                }
                                else if (install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name);
                                    N.Add("  " + subKeyName + "  " + name);
                                }
                            }
                        }
                    }
                }

                //foreach(string s in N)
                //richTextBox1.AppendText("\n" + s);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            GetVersionFromRegistry();
        }

        private int _sortColumn = -1;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //C#

            if (lv.ListViewItemSorter == null)
            {
                // Set the ListViewItemSorter property to a new ListViewItemComparer
                // object.
                lv.ListViewItemSorter = new ListViewItemComparer(e.Column, SortOrder.Ascending);
                // Call the sort method to manually sort.
                ///lv.Sort();

            }

            // Determine whether the column is the same as the last column clicked.
            if (e.Column != _sortColumn)
            {
                // Set the sort column to the new column.
                _sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (lv.Sorting == SortOrder.Ascending)
                    lv.Sorting = SortOrder.Descending;
                else
                    lv.Sorting = SortOrder.Ascending;
            }


            this.lv.ListViewItemSorter = new ListViewItemComparer(e.Column, lv.Sorting);
            // Call the sort method to manually sort.
            lv.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.

        }

        public void ShortVersion()
        {
            lv.Columns[2].Width = 0;
            lv.Columns[3].Width = 0;
            lv.Columns[5].Width = 0;

            //lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ShortVersion();
        }

        public ListViewItem FindItem(string name)
        {
            foreach (ListViewItem v in lv.Items)
            {
                if (v.SubItems[1].Text == name)
                    return v;
            }

            return null;
        }

        public ArrayList refs { get; set; }

        public void CheckReferences(ArrayList L)
        {
            if (L == null)
                return;

            foreach (string s in L)
            {
                ListViewItem v = FindItem(s);

                if (v == null)
                    continue;

                v.Checked = true;
            }
        }

        private Dictionary<string, string> ps { get; set; }

        //public void LoadProjects(Dictionary<string, string> d){


        //    ps = d;



        //    foreach (string s in L)
        //    {

        //        ListViewItem v = FindItem(s);

        //        if (v == null)
        //            continue;

        //        v.Checked = true;

        //    }

        //}


        public enum Sel { framework, extensions, recent, projects, COM, browse };

        public Sel sel { get; set; }

        public void LoadProjects()
        {
            if (ps == null)
                return;

            LoadProjects(ps);

            sel = Sel.projects;
        }

        public ArrayList projs { get; set; }

        public void CheckProjects(ArrayList L)
        {
            if (L == null)
                return;


            foreach (ListViewItem v in lv.Items)
            {
                string text = v.SubItems[2].Text;



                int i = L.IndexOf(text);

                if (i < 0)
                    v.Checked = false;
                else
                    v.Checked = true;
            }
        }


        public void LoadProjects(Dictionary<string, string> d)
        {
            ps = d;

            InitializeListView(lv, "projects");

            A = new ArrayList();

            foreach (string s in d.Keys)
            {
                ListViewItem v = new ListViewItem();
                v.Text = "";

                lv.Columns[0].TextAlign = HorizontalAlignment.Center;

                v.SubItems.Add(s);




                v.SubItems.Add(d[s]);



                lv.Items.Add(v);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.Text == "Assemblies" || node.Text == "Framework")
            {
                SuspendLayout();
                UpdateChecks();
                if (sel != Sel.framework)
                {
                    sel = Sel.framework;
                    GetAssemblies();
                }
                CheckReferences(refs);
                UpdateChecks();
                ResumeLayout();
            }
            else
            if (node.Text == "Extensions")
            {
                SuspendLayout();
                UpdateChecks();
                if (sel != Sel.extensions)
                {
                    sel = Sel.extensions;
                    //GetFrameworks();
                    GetExtensions();
                }
                CheckReferences(refs);
                UpdateChecks();
                sel = Sel.extensions;
                ResumeLayout();
            }
            else
                if (node.Text == "Solution" || node.Text == "Projects")
            {
                SuspendLayout();
                UpdateChecks();
                //if (sel != Sel.projects)
                {
                    sel = Sel.projects;
                    LoadProjects();
                }
                CheckProjects(projs);
                UpdateChecks();

                ResumeLayout();
            }
            else
            if (node.Text == "COM" || node.Text == "TypeLibraries")
            {
                SuspendLayout();
                UpdateChecks();
                if (sel != Sel.COM)

                {
                    sel = Sel.COM;
                    FilterTypeLibraries();
                }
                UpdateChecks();
                ResumeLayout();
            }
        }

        private void UpdateChecks()
        {
            if (sel == Sel.projects)
                if (projs != null)
                    projs.Clear();

            foreach (ListViewItem v in lv.Items)
            {
                if (sel == Sel.projects)
                {
                    if (projs == null)
                        return;

                    if (v.Checked == true)
                    {
                        string text = v.SubItems[2].Text;

                        projs.Add(text);
                    }
                }
                else if (sel == Sel.framework || sel == Sel.extensions)
                {
                    if (refs == null)
                        return;

                    if (v.Checked == true)
                    {
                        string text = v.SubItems[1].Text;

                        int i = refs.LastIndexOf(text);

                        if (i < 0)
                            refs.Add(text);
                    }
                    else
                    {
                        string text = v.SubItems[1].Text;

                        int i = refs.LastIndexOf(text);

                        if (i > 0)
                            refs.Remove(text);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateChecks();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult r = ofd.ShowDialog();
        }
    }
}
