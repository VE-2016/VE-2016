using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProfilerNET
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lb = listBox1;
            ls = listBox2;
            dg = dataGridView1;
            v = treeView1;
            InitializeDG(dg);
            dg.SelectionChanged += Dg_SelectionChanged;
            lb.SelectedIndexChanged += GetRefs;
            LoadSnapshots();
        }

        public void LoadSnapshots()
        {
            ls.Items.Clear();

            string[] dd = Directory.GetFiles("Snapshots");

            foreach (string s in dd)
                ls.Items.Add(s);
        }

        private void GetRefs(object sender, EventArgs e)
        {
            int i = lb.SelectedIndex;
            if (i < 0)
                return;
            string s = lb.Items[i].ToString();

            ulong refs = ulong.Parse(s);

            Node node = Profiler.GetReference(runtime, refs);

            

            v.Nodes.Clear();

            if (node != null)
                MessageBox.Show("Object found...");
            else return;

            TreeNode nodes = new TreeNode(node.Type.Name);
            v.Nodes.Add(nodes);
            nodes.Tag = node;
            while(node.Next != null)
            {

                node = node.Next;

                TreeNode ng = new TreeNode(node.Type.Name);
                nodes.Nodes.Add(ng);
                nodes = ng;
                nodes.Tag = node;

            }


            Profiler.PrintOnePath(node);

            //ClrType type = null;

            //do
            //{

            //    foreach (ClrType b in stats.Keys)
            //    {
            //        Entry entry = stats[b];

            //        foreach (ulong r in entry.refs)
            //        {
            //            if (r == refs)
            //            {

            //                type = objs[r];

            //                TreeNode ng = new TreeNode(type.Name);

            //                nodes.Nodes.Add(ng);

            //                nodes = ng;

            //                break;

            //            }

            //        }

            //    }

            //}
            //while (type != null);            

        
        
        }

        private void Dg_SelectionChanged(object sender, EventArgs e)
        {
            if (dg.SelectedRows == null)
                return;
            if (dg.SelectedRows.Count <= 0)
                return;
            DataGridViewRow row = dg.SelectedRows[0];
            lb.Items.Clear();
            Entry entry = row.Tag as Entry;
            if (entry == null)
                return;
            foreach (ulong r in entry.refs)
                lb.Items.Add(r.ToString());
        }

        DataGridView dg { get; set; }

        ListBox lb { get; set; }

        ListBox ls { get; set; }

        TreeView v { get; set; }

        public ClrRuntime runtime { get; set; }

        public Dictionary<ClrType, Entry> stats {get; set;}

        static public Dictionary<ulong, ulong> objs { get; set; }


        public Dictionary<ClrType, Entry> HeapDumpInfo(string file = "")
        {

            DataTarget target = DataTarget.LoadCrashDump(file);

            //ClrInfo version = target.ClrVersions[0];

            //runtime = target.ClrVersions.First().CreateRuntime();

            ClrInfo runtimeInfo = target.ClrVersions[0];  // just using the first runtime
           runtime = runtimeInfo.CreateRuntime();


            Dictionary<ClrType, Entry> stats = DumpHeap(runtime);


            return stats;




        }


        public Dictionary<ClrType, Entry> HeapInfo()
        {
            
            int pid = Process.GetProcessesByName("WinExplorer")[0].Id;
            //int pid = Process.GetCurrentProcess().Id;

            DataTarget target = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive);

            
            ClrInfo version = target.ClrVersions[0];

            runtime = target.ClrVersions.First().CreateRuntime();

            

            
           
            Dictionary<ClrType, Entry> stats = DumpHeap(runtime);


            return stats;

        }
        static Dictionary<ClrType, Entry> DumpHeap(ClrRuntime runtime)
        {
            Dictionary<ClrType, Entry> stats = new Dictionary<ClrType, Entry>();
            
            

            try
            {
                // Create a ClrRuntime instance from the dump and dac location.  The ClrRuntime
                // object represents a version of CLR loaded in the process.  It contains data
                // such as the managed threads in the process, the AppDomains in the process,
                // the managed heap, and so on.
                //ClrRuntime runtime = CreateRuntime(dump, dac);

                // Walk the entire heap and build heap statistics in "stats".
                
                

                // This is the way to walk every object on the heap:  Get the ClrHeap instance
                // from the runtime.  Walk every segment in heap.Segments, and use
                // ClrSegment.FirstObject and ClrSegment.NextObject to iterate through
                // objects on that segment.
                ClrHeap heap = runtime.GetHeap();
                foreach (ClrSegment seg in heap.Segments)
                {
                    for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                    {
                        // This gets the type of the object.
                        ClrType type = heap.GetObjectType(obj);

                        if (type == null)
                            continue;

                        ulong size = type.GetSize(obj);


                        //objs[obj] = type;


                        // If the user didn't request "-stat", print out the object.

                        //    Console.WriteLine("{0,16:X} {1,12:n0} {2}", obj, size, type.Name);

                        // Add an entry to the dictionary, if one doesn't already exist.
                        Entry entry = null;

                        if (!stats.TryGetValue(type, out entry))
                        {
                            entry = new Entry();
                            entry.Name = type.Name;
                            entry.refs.Add(obj);
                            //entry.obj = obj;
                            stats[type] = entry;
                        }
                        else
                        {
                            //entry.obj = obj;
                            entry.refs.Add(obj);
                        }
                        // Update the statistics for this object.
                        entry.Count++;
                        entry.Size += type.GetSize(obj);


                        //type.EnumerateRefsOfObject(obj, delegate (ulong child, int offset)
                        //{
                        //    objs[obj] = child;
                        //});


                    }
                }

                // Now print out statistics.
                
                //    Console.WriteLine();

                // We'll actually let linq do the heavy lifting.
                var sortedStats = from entry in stats.Values
                                  orderby entry.Size
                                  select entry;

                //Console.WriteLine("{0,12} {1,12} {2}", "Size", "Count", "Type");
                //foreach (var entry in sortedStats)
                //    Console.WriteLine("{0,12:n0} {1,12:n0} {2}", entry.Size, entry.Count, entry.Name);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }
            finally
            {
                
            }
            return stats;
        }

       

      

      
        public void InitializeDG(DataGridView dg)
        {
            dg.Rows.Clear();

            dg.AllowUserToAddRows = false;

            dg.AllowUserToOrderColumns = false;

            dg.AllowUserToResizeRows = false;

            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            
            dg.Columns.Add("Name", "Name");
            dg.Columns.Add("Count", "Count");
            dg.Columns.Add("Size", "Size");
        }

        public void LoadHeapDump(Dictionary<ClrType, Entry> stats)
        {

            dg.Rows.Clear();

            foreach (ClrType c in stats.Keys)
            {

                Entry entry = stats[c];

                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];

                row.Cells[0].Value = c.Name.Trim();
                row.Cells[1].Value = entry.Count;
                row.Cells[2].Value = entry.Size;
                row.Tag = entry;


            }

        }

        public DataGridViewRow FindRow(string name)
        {
            foreach (DataGridViewRow r in dg.Rows)
                if (r.Cells[0].Value.ToString() == name)
                    return r;

            return null;
        }

        public void AppendHeapDump(Dictionary<ClrType, Entry> stats)
        {

            int cc = dg.Columns.Count;

            dg.Columns.Add("Count2", "Count");
            dg.Columns.Add("Size2", "Size");


            foreach (ClrType c in stats.Keys)
            {

                Entry entry = stats[c];


                DataGridViewRow row = FindRow(c.Name.Trim());

                if (row != null)
                {
                    row.Cells[cc].Value = entry.Count;
                    row.Cells[cc + 1].Value = entry.Size;

                }
                else
                {

                    int rowId = dg.Rows.Add();
                    row = dg.Rows[rowId];

                    row.Cells[0].Value = c.Name.Trim();
                    
                    row.Cells[cc].Value = entry.Count;
                    row.Cells[cc+ 1].Value = entry.Size;
                    row.Tag = entry;

                }
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            stats = HeapDumpInfo();

            LoadHeapDump(stats);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Profiler.Profile(runtime);

            stats = DumpHeap(runtime);

            AppendHeapDump(stats);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = v.SelectedNode;
            if (node == null)
                return;
            Node ng = node.Tag as Node;
            if (ng == null)
                return;

            textBox1.Text = ng.Type.Module.Pdb.FileName;

            


        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {

            int i = ls.SelectedIndex;

            if (i < 0)
                return;


            string file = AppDomain.CurrentDomain.BaseDirectory + ls.Items[i].ToString();

            if(runtime == null)
            {
                stats = HeapDumpInfo(file);

                LoadHeapDump(stats);
            }
            else
            {
                stats = HeapDumpInfo(file);

                AppendHeapDump(stats);
            }
        }
    }
    public class Entry
    {
        public ulong obj;
        public string Name;
        public int Count;
        public ulong Size;
        public List<ulong> refs = new List<ulong>();
    }
}
