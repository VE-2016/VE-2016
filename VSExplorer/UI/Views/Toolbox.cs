using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer.UI.Views
{
    public partial class Toolbox : Form
    {
        public Toolbox()
        {
            InitializeComponent();
            v = treeView1;
            rb = richTextBox1;
            rb.MouseClick += Rb_MouseClick;
            context = contextMenuStrip1;
            ExplorerForms.ef.event_SelectedSolutionChanged += Ef_event_SelectedSolutionChanged;
        }

        private void Rb_MouseClick(object sender, MouseEventArgs e)
        {
            context.Show(this, e.Location);
        }

        private RichTextBox rb { get; set; }

        private ContextMenuStrip context { get; set; }

        private void Ef_event_SelectedSolutionChanged(object sender, VSSolution vs)
        {
            if (vs == null)
                return;
            this.BeginInvoke(new Action(() => { /*LoadComponents();*/ }));
        }

        private TreeView v { get; set; }
        private VSSolution vs { get; set; }

        public static ArrayList LoadNETFrameworkComponents(ArrayList F)
        {
            ArrayList C = new ArrayList();
            foreach (GACManagerApi.AssemblyDescription d in F)
            {
                string s = d.Path;
                if (!File.Exists(s))
                    continue;
                string name = Path.GetFileNameWithoutExtension(s);

                Type[] types = GetComponents(s);

                if (types == null || types.Length <= 0)
                    continue;

                foreach (Type T in types)
                {
                    C.Add(T);
                }
            }
            return C;
        }

        public static ArrayList LoadWPFFrameworkComponents(ArrayList F)
        {
            Assembly asm = Assembly.LoadFile("C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\WPF\\PresentationFramework.dll");
            Type TT = asm.GetType("System.Windows.FrameworkElement");

            ArrayList C = new ArrayList();
            foreach (GACManagerApi.AssemblyDescription d in F)
            {
                string s = d.Path;
                if (!File.Exists(s))
                    continue;
                string name = Path.GetFileNameWithoutExtension(s);

                Type[] types = GetWPFComponents(s, TT);

                if (types == null || types.Length <= 0)
                    continue;

                foreach (Type T in types)
                {
                    C.Add(T);
                }
            }
            return C;
        }

        public void LoadComponents()
        {
            vs = ExplorerForms.ef.GetVSSolution();
            if (vs == null)
                return;
            ArrayList E = vs.GetExecutables(VSSolution.OutputType.both);
            v.Nodes.Clear();
            foreach (string s in E)
            {
                if (!File.Exists(s))
                    continue;
                string name = Path.GetFileNameWithoutExtension(s);

                Type[] types = GetComponents(s);

                if (types == null || types.Length <= 0)
                    continue;

                TreeNode node = new TreeNode();
                node.Text = name;
                v.Nodes.Add(node);

                foreach (Type T in types)
                {
                    TreeNode nodes = new TreeNode();
                    nodes.Text = T.Name;
                    if (node.Nodes.Cast<TreeNode>().Any(n => n.Text == nodes.Text)) continue;
                    node.Nodes.Add(nodes);
                }
            }
        }

        public static ArrayList LoadCOMComponents(ArrayList F)
        {
            ArrayList C = new ArrayList();
            foreach (GACManagerApi.AssemblyDescription d in F)
            {
                string s = d.Path;

                if (!File.Exists(s))
                {
                    continue;
                }

                // string output = TlbImport(s);

                Type[] types = GetTLBComponents(s);

                if (types == null || types.Length <= 0)
                    continue;

                foreach (Type T in types)
                {
                    C.Add(T);
                }
            }
            return C;
        }

        public static string TlbImport(string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);

            name = name + ".dll";

            string output = AppDomain.CurrentDomain.BaseDirectory + "temp\\" + name;

            ExplorerForms.ef.Command_ImportTlb(file, output);

            return output;
        }

        public static Type[] GetComponents(string file)
        {
            try
            {
                var types = Assembly.LoadFile(file)
                    .GetTypes()
                    .Where(t => typeof(Component).IsAssignableFrom(t) &&
                    t != typeof(Component) && !typeof(Form).IsAssignableFrom(t))
                    .ToArray();
                return types;
            }
            catch (Exception ex)
            {
            }
            return new Type[0];
        }

        public static Type[] GetWPFComponents(string file, Type T)
        {
            try
            {
                var types = Assembly.LoadFile(file)
                    .GetTypes()
                    .Where(t => T.IsAssignableFrom(t) &&
                    t != T)
                    .ToArray();
                return types;
            }
            catch (Exception ex)
            {
            }
            return new Type[0];
        }

        public static Type[] GetTLBComponents(string file)
        {
            try
            {
                var types = Assembly.LoadFile(file)
                    .GetTypes()
                    // .Where(t => typeof(Component).IsAssignableFrom(t) &&
                    // t != typeof(Component) && !typeof(Form).IsAssignableFrom(t))
                    .ToArray();
                return types;
            }
            catch (Exception ex)
            {
            }
            return new Type[0];
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChooseToolboxItemsForm ch = new ChooseToolboxItemsForm();
            DialogResult r = ch.ShowDialog();
        }
    }
}