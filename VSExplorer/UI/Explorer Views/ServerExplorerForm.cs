using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class ServerExplorerForm : Form
    {
        public ServerExplorerForm()
        {
            InitializeComponent();
            ts = toolStrip1;
            v = treeView1;
            contextDC = contextMenuStrip1;
            contextSR = contextMenuStrip4;
            contextSC = contextMenuStrip8;
            contextPC = contextMenuStrip9;
            Init();
            v.BeforeExpand += V_BeforeExpand;
            v.AfterSelect += V_AfterSelect;
            v.MouseClick += V_MouseClick;
          
        }

        private void V_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            TreeNode node = v.SelectedNode;
            if (node == null)
                return;
            if(node.Text == "Data Connections")
            {
                contextDC.Show(v, e.Location);
            } else if (node.Text == "Servers")
            {
                contextSR.Show(v, e.Location);
            }
            else if (node.Text == "Services")
            {
                contextSC.Show(v, e.Location);
            }
            else if (node.Text == "Performance Counters")
            {
                contextPC.Show(v, e.Location);
            }
        }

        ContextMenuStrip contextDC { get; set; }
        ContextMenuStrip contextSR { get; set; }
        ContextMenuStrip contextSC { get; set; }
        ContextMenuStrip contextPC { get; set; }
        private void V_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.Tag != null)
            {
                if (node.Tag.GetType() == typeof(DataConnection))
                {

                    DataConnection d = node.Tag as DataConnection;

                    DataSourceWizard.LoadFromConnection(node, d);

                }
                ExplorerForms.ef.SetPropertyGridObject(node.Tag);
            }
           
        }
        private void V_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Text == "Services")
            {
                if (node.Nodes.Count != 1)
                    return;
                node.Nodes.Clear();
                Load_LocalServices(node);
            }
            else if (node.Text == "Performance Counters")
            {
                if (node.Nodes.Count != 1)
                    return;
                node.Nodes.Clear();
                Load_PerformanceCounters(node, System.Environment.MachineName);
            }
            else if (node.Text == "Event Logs")
            {
                if (node.Nodes.Count != 1)
                    return;
                node.Nodes.Clear();
                Load_EventLogs(node);
            }
            else
            {
                
                if (node.Nodes.Count == 1)
                    if (node.Nodes[0].Text == "Oracle")
                    {

                        node.Nodes.Clear();
                        TreeNode ns = node.Parent;
                        if (ns == null)
                            return;
                        ns = ns.Parent;
                        if (ns == null)
                            return;
                        DataConnection dc = ns.Tag as DataConnection;
                        if (dc == null)
                            return;
                        DataSourceWizard.GetColumnsOracle(node, node.Text, dc.GetConnectionStringr());
                    }
            }
        }

        ToolStrip ts { get; set; }

        TreeView v { get; set; }

        ImageList g { get; set; }

        public void Init()
        {
            ts.GripStyle = ToolStripGripStyle.Hidden;

            g = new ImageList();
            g.Images.Add("servers", Resources.ServerSettings_16x);
            g.Images.Add("server", Resources.ServerSettings_16x);
            g.Images.Add("events", Resources.EventLog_32x);
            g.Images.Add("messages", Resources.MessageQueue_32x);
            g.Images.Add("performance", Resources.PerformanceReport_16x);
            g.Images.Add("services", Resources.Services_16x);
            g.Images.Add("datasource", Resources.AddDataSource_16x);
            g.Images.Add("stopped", Resources.Stop_grey_16x);
            



            v.ImageList = g;

            TreeNode node = new TreeNode();
            node.Text = "Hadoop (Not connected)";
            node.ImageKey = "servers";
            v.Nodes.Add(node);

            node = new TreeNode();
            node.Text = "Data Connections";
            node.ImageKey = "datasource";
            v.Nodes.Add(node);
            AddConnections(node);
         
            Servers = new TreeNode();
            Servers.Text = "Servers";
            Servers.ImageKey = "servers";

            v.Nodes.Add(Servers);

            AddLocalServer();


        }

        private void ServerExplorerForm_Load(object sender, EventArgs e)
        {
           
        }

        void AddConnections(TreeNode node)
        {
            ArrayList C = DataSourceWizard.GetConnections();
            foreach (string c in C)
            {
                if (String.IsNullOrEmpty(c))
                    continue;
                TreeNode nodes = new TreeNode();
                nodes.Text = c;
                nodes.ImageKey = "datasource";
                node.Nodes.Add(nodes);
                DataConnection d = new DataConnection();
                d.Load(c);
                nodes.Tag = d;
            }
        }

        TreeNode Servers { get; set; }

        public void AddLocalServer()
        {
            string comp = System.Environment.MachineName;

            TreeNode node = new TreeNode();
            node.Text = comp;
            node.ImageKey = "server";

            Servers.Nodes.Add(node);

                       
            TreeNode nodes = new TreeNode();
            nodes.Text = "Event Logs";
            nodes.ImageKey = "events";
            node.Nodes.Add(nodes);
            TreeNode ns = new TreeNode();
            ns.Text = "void";
            nodes.Nodes.Add(ns);
            
            nodes = new TreeNode();
            nodes.Text = "Message Queues";
            nodes.ImageKey = "messages";
            node.Nodes.Add(nodes);
            nodes = new TreeNode();
            nodes.Text = "Performance Counters";
            nodes.ImageKey = "performance";
            node.Nodes.Add(nodes);

            ns = new TreeNode();
            ns.Text = "void";
            nodes.Nodes.Add(ns);

            nodes = new TreeNode();
            nodes.Text = "Services";
            nodes.ImageKey = "services";
            node.Nodes.Add(nodes);
            ns = new TreeNode();
            ns.Text = "void";
            nodes.Nodes.Add(ns);

        }

        public void Load_LocalServices(TreeNode node)
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();

                     
            foreach (ServiceController scTemp in scServices)
            {
                //if (scTemp.Status == ServiceControllerStatus.Running)
                {
                    // Write the service name and the display name
                    // for each running service.
                    Console.WriteLine();
                    Console.WriteLine("  Service :        {0}", scTemp.ServiceName);
                    Console.WriteLine("    Display name:    {0}", scTemp.DisplayName);

                    // Query WMI for additional information about this service.
                    // Display the start name (LocalSytem, etc) and the service
                    // description.
                    ManagementObject wmiService;
                    wmiService = new ManagementObject("Win32_Service.Name='" + scTemp.ServiceName + "'");
                    wmiService.Get();
                    Console.WriteLine("    Start name:      {0}", wmiService["StartName"]);
                    Console.WriteLine("    Description:     {0}", wmiService["Description"]);

                    TreeNode nodes = new TreeNode();
                    nodes.Text = scTemp.ServiceName;
                    nodes.ImageKey = "stopped";
                    nodes.Tag = scTemp;
                    node.Nodes.Add(nodes);
                }
            }
        }

        public void Load_PerformanceCounters(TreeNode node, string machine)
        {
            string categoryName = "";
            string machineName = machine;
            
            PerformanceCounterCategory pcc;
            PerformanceCounter[] counters = null;

            // Get all the performance counter categories.
            PerformanceCounterCategory[] categories =
              System.Diagnostics.PerformanceCounterCategory.GetCategories();
            foreach (PerformanceCounterCategory c in categories)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = c.CategoryName;
                nodes.ImageKey = "performance";
                node.Nodes.Add(nodes);

                categoryName = c.CategoryName;

                try
                {
                    // Create the appropriate PerformanceCounterCategory object.
                    if (machineName.Length > 0)
                    {
                        pcc = new PerformanceCounterCategory(categoryName, machineName);
                    }
                    else
                    {
                        pcc = new PerformanceCounterCategory(categoryName);
                    }
                  
                    counters = pcc.GetCounters();
                }
                catch (Exception ex)
                {
                   
                }
                if (counters != null)
                {
                    int objX;
                    for (objX = 0; objX < counters.Length; objX++)
                    {
                        TreeNode ns = new TreeNode();
                        ns.Text = counters[objX].CounterName;
                        // Console.WriteLine("{0,4} - {1}", objX + 1, counters[objX].CounterName);
                        ns.Tag = counters[objX];
                        nodes.Nodes.Add(ns);

                    }
                }

            }
            
        }
        public static void Load_EventLogs(TreeNode node)
        {
            // Iterate through the current set of event log files,
            // displaying the property settings for each file.

            Dictionary<string, TreeNode> dict = new Dictionary<string, TreeNode>();


            EventLog[] eventLogs = EventLog.GetEventLogs();
            foreach (EventLog e in eventLogs)
            {

                TreeNode nodes = new TreeNode();
                nodes.Text = e.Log;
                node.Nodes.Add(nodes);
                try
                {
                    foreach (EventLogEntry entry in e.Entries)
                    {
                        string c = entry.Source;
                        if(dict.ContainsKey(c) == false)
                        {
                            TreeNode ng = new TreeNode();
                            ng.Text = c;
                            nodes.Nodes.Add(ng);
                            dict.Add(c, ng);
                        }
                        TreeNode b = dict[c];
                        TreeNode ns = new TreeNode();
                        ns.Text = entry.Message;
                        ns.Tag = entry;
                        b.Nodes.Add(ns);
                    }
                }
                catch(Exception ex) { };
                
            }
        }

        private void contextMenuStrip5_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {

        }
    }
}
