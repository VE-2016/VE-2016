using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Text.RegularExpressions;
//using PowerShellModuleInCSharp.Containers;
using Microsoft.PowerShell.Commands;

using WinExplorer;
using VSParsers;
using System.Windows.Forms;
using System.IO;
using VSProvider;
using System.Collections;

namespace Commandlets
{
    //[Cmdlet(VerbsCommon.Get, nameof(NetworkAdapter))]
    //[OutputType(typeof(NetworkAdapter))]
    //public class GetNetworkAdapterCmdlet : Cmdlet
    //{

    //    [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
    //    public string Name { get; set; }

    //    [Parameter(
    //            Position = 0,
    //            ValueFromPipelineByPropertyName = true,
    //            ValueFromPipeline = true)]
    //    [Alias("mfg", "vendor")]
    //    public string[] Manufacturer { get; set; }

    //    [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
    //    public bool? PhysicalAdapter { get; set; }

    //    [Parameter(Position = 3)]
    //    public int MaxEntries { get; set; } = 100;

    //    private IEnumerable<NetworkAdapter> _wmiResults;

    //    TreeView b { get; set; }

    //    protected override void BeginProcessing()
    //    {
    //        base.BeginProcessing();
    //        _wmiResults = GetWmiResults();
    //    }

    //    protected override void ProcessRecord()
    //    {


    //        string file = Path.GetFullPath("F:\\alls\\test\\WindowsFormsApplication5.sln");

    //        if (!File.Exists(file))
    //            throw new FileNotFoundException("File does not exist.", file);

    //        TreeView b = VSExplorer.LoadProject(file);


    //        var query = _wmiResults;
    //        if (Name != null)
    //        {
    //            query = query.Where(adapter =>
    //                adapter.Name != null && adapter.Name.StartsWith(Name));
    //        }
    //        // Just like "Name" above, this checks for prefix matches
    //        // but on multiple values instead of just a single value.
    //        if (Manufacturer != null)
    //        {
    //            query = query.Where(
    //                adapter =>
    //                    adapter.Manufacturer != null &&
    //                    Regex.IsMatch(adapter.Manufacturer,
    //                        string.Format("^(?:{0})", string.Join("|", Manufacturer))));
    //        }
    //        // Being a Boolean, an exact match is used here.
    //        if (PhysicalAdapter != null)
    //        {
    //            query = query.Where(adapter =>
    //                adapter.PhysicalAdapter == PhysicalAdapter);
    //        }
    //        query.Take(MaxEntries).ToList().ForEach(WriteObject);
    //    }

    //    private IEnumerable<NetworkAdapter> GetWmiResults()
    //    {
    //        const string wmiQuery = "Select * from Win32_NetworkAdapter";
    //        return new ManagementObjectSearcher(wmiQuery).Get()
    //            .Cast<ManagementObject>()
    //            .Select(BuildOutputObject);
    //    }

    //    private NetworkAdapter BuildOutputObject(ManagementBaseObject item)
    //    {

    //        //WriteObject("Hello " + b.Nodes.Count +  "!");

    //        return new NetworkAdapter
    //        {
    //            Name = (string)item[nameof(NetworkAdapter.Name)],
    //            Description = (string)item[nameof(NetworkAdapter.Description)],
    //            DeviceId = int.Parse((string)item[nameof(NetworkAdapter.DeviceId)]),
    //            Manufacturer = (string)item[nameof(NetworkAdapter.Manufacturer)],
    //            NetConnectionId = (string)item[nameof(NetworkAdapter.NetConnectionId)],
    //            PhysicalAdapter = (bool)item[nameof(NetworkAdapter.PhysicalAdapter)]
    //        };
    //    }
    //}

    [Cmdlet(VerbsCommon.Get, "GetProject")]
    [OutputType(typeof(string))]
    public class GetProject : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }



        private TreeView b { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            string file = Path.GetFullPath("F:\\Application\\MSBuildProjects-new\\VStudio.sln");

            if (!File.Exists(file))
                throw new FileNotFoundException("File does not exist.", file);

            TreeView b = VSExplorer.LoadProject(file, Name);

            if (b != null)
                WriteObject("Project contains " + b.Nodes[0].Nodes.Count + " nodes");
            else
                WriteObject("Project did not open properly");

            WriteObject("Completed....");
        }



        private string BuildOutputObject(ManagementBaseObject item)
        {
            return "hello from projects";
        }
    }
    [Cmdlet(VerbsCommon.Get, "ReferenceNugetPackage")]
    [OutputType(typeof(string))]
    public class ReferenceNugetPackage : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Folder { get; set; }

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string SolutionFile { get; set; }

        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
        public string PackageName { get; set; }

        [Parameter(Position = 3, ValueFromPipelineByPropertyName = true)]
        public string ProjectName { get; set; }


        private TreeView b { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            //string file = Path.GetFullPath("F:\\Application\\MSBuildProjects-new\\VStudio.sln");

            string file = Path.GetFullPath(SolutionFile);

            if (!File.Exists(file))
                throw new FileNotFoundException("File does not exist.", file);

            TreeView b = VSExplorer.LoadProject(file, Folder);
            if (b != null)
                WriteObject("Project contains " + b.Nodes[0].Nodes.Count + " nodes");
            else
            {
                WriteObject("Project did not open properly");
                return;
            }
            CreateView_Solution.ProjectItemInfo p = b.Nodes[0].Nodes[0].Tag as CreateView_Solution.ProjectItemInfo;
            if (p == null)
            {
                WriteObject("VSSolution tag not found");
                return;
            }
            else WriteObject("VSSolution tag found");
            VSSolution vs = p.vs;
            if (vs == null)
            {
                WriteObject("VSSolution not found");
                return;
            }
            else WriteObject("VSSolution found");
            VSProject vp = vs.GetProjectbyName(ProjectName);
            if (vp == null)
            {
                WriteObject("VSProject not found");
                return;
            }
            else WriteObject("VSProject found");

            vp.SetReference(PackageName);

            WriteObject("Completed....");
        }



        private string BuildOutputObject(ManagementBaseObject item)
        {
            return "hello from projects";
        }
    }
}
//namespace PowerShellModuleInCSharp.Containers
//{
//    public class NetworkAdapter
//    {
//        public string Name { get; set; }
//        public string Description { get; set; }
//        public int DeviceId { get; set; }
//        public string Manufacturer { get; set; }
//        public string NetConnectionId { get; set; }
//        public bool PhysicalAdapter { get; set; }
//    }
//}