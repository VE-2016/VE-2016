using AndersLiu.Reflector.Core;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AndersLiu.Reflector.Core
{
    public class WorkspaceUnit
    {
        /// <summary>
        /// Create a new workspace unit.
        /// </summary>
        /// <param name="location">Location of the unit.</param>
        public WorkspaceUnit(string location)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            this.Location = location;
        }

        /// <summary>
        /// Get the location of the unit.
        /// </summary>
        public string Location { get; set; }

        public bool IsAdded { get; set; }
    }
}

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    /// <summary>
    /// Provides functions for building tree node.
    /// </summary>
    public class TreeNodeBuilder
    {
        public TreeNodeBuilder()
        {
            //_host = new ReflectorHostEnvironment();
            //_host.Errors += HostErrorsHandler;
        }

        //private void HostErrorsHandler(object sender, ErrorEventArgs e)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendFormat("Error Report: {0}", e.ErrorReporter);
        //    sb.AppendLine();
        //    sb.AppendLine("Errors:");

        //    foreach (var err in e.Errors)
        //    {
        //        sb.AppendFormat("  Location: {0}", err.Location.Document.Location);
        //        sb.AppendLine();
        //        sb.AppendFormat("  Code: {0}", err.Code);
        //        sb.AppendLine();
        //        sb.AppendFormat("  Message: {0}", err.Message);
        //        sb.AppendLine();
        //    }

        //    sb.AppendFormat("Location: {0}", e.Location.Document.Location);

        //    MessageBox.Show(sb.ToString());
        //}

        static public TreeNode BuildUnitNodes(string file)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            string g = s + "temp";

            string b = g + Path.GetFileName(file);

            if (Directory.Exists(g) == false)
                Directory.CreateDirectory(g);

            try
            {
                if (File.Exists(b) == true)
                    File.Delete(b);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                if (File.Exists(file))
                    File.Copy(file, b);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            WorkspaceUnit w = new WorkspaceUnit(b);

            TreeNode node = BuildUnitNodes(w);

            return node;
        }

        static public TreeNode BuildUnitNodes(WorkspaceUnit unit)
        {
            var node = (TreeNode)null;

            ReflectorHostEnvironment host = new ReflectorHostEnvironment();

            var iunit = host.LoadUnitFrom(unit.Location);

            if (iunit == null || iunit == Dummy.Assembly || iunit == Dummy.Module)
                node = BuildErrorNodes(unit.Location, unit);
            else if (iunit is IAssembly)
                node = BuildAssemblyNodes(iunit as IAssembly, unit);
            else if (iunit is IModule)
                node = BuildModuleNodes(iunit as IModule, unit);
            else  // should never goes here
                throw new NotSupportedException();

            if (unit.IsAdded)
                node.Text = "[+] " + node.Text;

            host.CloseReader();

            return node;
        }

        /// <summary>
        /// Build a unit node.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public TreeNode BuildUnitNode(WorkspaceUnit unit)
        {
            var node = (TreeNode)null;
            var iunit = _host.LoadUnitFrom(unit.Location);

            if (iunit == null || iunit == Dummy.Assembly || iunit == Dummy.Module)
                node = BuildErrorNode(unit.Location, unit);
            else if (iunit is IAssembly)
                node = BuildAssemblyNode(iunit as IAssembly, unit);
            else if (iunit is IModule)
                node = BuildModuleNode(iunit as IModule, unit);
            else  // should never goes here
                throw new NotSupportedException();

            if (unit.IsAdded)
                node.Text = "[+] " + node.Text;

            return node;
        }

        private ReflectorHostEnvironment _host;

        public TreeNode BuildErrorNode(string location, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            //CreateTreeNode(new ErrorNodeTag<TreeNode>(node, location, unit));

            return node;
        }

        static public TreeNode BuildErrorNodes(string location, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            // CreateTreeNodes(new ErrorNodeTag<TreeNode>(node, location, unit));

            return node;
        }

        /// <summary>
        /// Build an assembly node.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public TreeNode BuildAssemblyNode(IAssembly assembly, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            CreateTreeNode(new AssemblyNodeTag<TreeNode>(node, assembly, unit));

            node.Nodes.Add(BuildModuleNode(assembly, null));
            foreach (var module in assembly.MemberModules)
            {
                node.Nodes.Add(BuildModuleNode(module, null));
            }

            return node;
        }

        static public TreeNode BuildAssemblyNodes(IAssembly assembly, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            CreateTreeNodes(new AssemblyNodeTag<TreeNode>(node, assembly, unit));

            node.Nodes.Add(BuildModuleNodes(assembly, null));
            foreach (var module in assembly.MemberModules)
            {
                node.Nodes.Add(BuildModuleNodes(module, null));
            }

            return node;
        }

        static public TreeNode BuildAssemblyInfo(IAssembly assembly, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            CreateTreeNodes(new AssemblyNodeTag<TreeNode>(node, assembly, unit));

            node.Nodes.Add(BuildModuleNodes(assembly, null));
            foreach (var module in assembly.MemberModules)
            {
                node.Nodes.Add(BuildModuleNodes(module, null));
            }

            return node;
        }

        public TreeNode BuildModuleNode(IModule module, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            CreateTreeNode(new ModuleNodeTag<TreeNode>(node, module, unit));

            // Add namespace nodes.
            var types = module.GetAllTypes();

            var topTypes = new List<INamedTypeDefinition>();
            foreach (var t in types)
                if (t is INamespaceTypeDefinition)
                    topTypes.Add(t);

            var namespaceTypes = new Dictionary<string, List<INamedTypeDefinition>>();
            foreach (var t in topTypes)
            {
                var ns = TypeHelper.GetNamespaceName(
                    (t as INamespaceTypeDefinition).ContainingUnitNamespace,
                    NameFormattingOptions.None);

                if (!namespaceTypes.ContainsKey(ns))
                    namespaceTypes.Add(ns, new List<INamedTypeDefinition>());

                namespaceTypes[ns].Add(t);
            }

            foreach (var ns in namespaceTypes.Keys)
                node.Nodes.Add(BuildNamespaceNode(ns, namespaceTypes[ns]));

            return node;
        }

        static public TreeNode BuildModuleNodes(IModule module, WorkspaceUnit unit)
        {
            var node = new TreeNode();
            CreateTreeNodes(new ModuleNodeTag<TreeNode>(node, module, unit));

            // Add namespace nodes.
            var types = module.GetAllTypes();

            var topTypes = new List<INamedTypeDefinition>();
            foreach (var t in types)
                if (t is INamespaceTypeDefinition)
                    topTypes.Add(t);

            var namespaceTypes = new Dictionary<string, List<INamedTypeDefinition>>();
            foreach (var t in topTypes)
            {
                var ns = TypeHelper.GetNamespaceName(
                    (t as INamespaceTypeDefinition).ContainingUnitNamespace,
                    NameFormattingOptions.None);

                if (!namespaceTypes.ContainsKey(ns))
                    namespaceTypes.Add(ns, new List<INamedTypeDefinition>());

                namespaceTypes[ns].Add(t);
            }

            foreach (var ns in namespaceTypes.Keys)
                node.Nodes.Add(BuildNamespaceNodes(ns, namespaceTypes[ns]));

            return node;
        }

        public TreeNode BuildNamespaceNode(string ns, IEnumerable<INamedTypeDefinition> types)
        {
            var node = new TreeNode();
            CreateTreeNode(new NamespaceNodeTag<TreeNode>(node, ns, types));

            foreach (var t in types)
                node.Nodes.Add(BuildTypeNode(t));

            return node;
        }

        static public TreeNode BuildNamespaceNodes(string ns, IEnumerable<INamedTypeDefinition> types)
        {
            var node = new TreeNode();
            CreateTreeNodes(new NamespaceNodeTag<TreeNode>(node, ns, types));

            foreach (var t in types)
                node.Nodes.Add(BuildTypeNodes(t));

            return node;
        }

        public TreeNode BuildTypeNode(INamedTypeDefinition type)
        {
            var node = new TreeNode();
            CreateTreeNode(new TypeNodeTag<TreeNode>(node, type));

            // Add nested types nodes.
            foreach (var t in type.NestedTypes)
                node.Nodes.Add(BuildTypeNode(t));

            var methods = new List<IMethodReference>();
            foreach (var m in type.Methods)
                methods.Add(m);

            // fields
            foreach (var f in type.Fields)
                node.Nodes.Add(BuildFieldNode(f));

            // events
            foreach (var e in type.Events)
            {
                node.Nodes.Add(BuildEventNode(e));

                foreach (var m in e.Accessors)
                    methods.Remove(m);
            }

            // properties
            foreach (var p in type.Properties)
            {
                node.Nodes.Add(BuildPropertyNode(p));

                foreach (var m in p.Accessors)
                    methods.Remove(m);
            }

            // methods
            foreach (var m in methods)
                node.Nodes.Add(BuildMethodNode(m.ResolvedMethod));

            return node;
        }

        static public TreeNode BuildTypeNodes(INamedTypeDefinition type)
        {
            var node = new TreeNode();
            CreateTreeNodes(new TypeNodeTag<TreeNode>(node, type));

            // Add nested types nodes.
            foreach (var t in type.NestedTypes)
                node.Nodes.Add(BuildTypeNodes(t));

            var methods = new List<IMethodReference>();
            foreach (var m in type.Methods)
                methods.Add(m);

            // fields
            foreach (var f in type.Fields)
                node.Nodes.Add(BuildFieldNodes(f));

            // events
            foreach (var e in type.Events)
            {
                node.Nodes.Add(BuildEventNodes(e));

                foreach (var m in e.Accessors)
                    methods.Remove(m);
            }

            // properties
            foreach (var p in type.Properties)
            {
                node.Nodes.Add(BuildPropertyNodes(p));

                foreach (var m in p.Accessors)
                    methods.Remove(m);
            }

            // methods
            foreach (var m in methods)
                node.Nodes.Add(BuildMethodNodes(m.ResolvedMethod));

            return node;
        }

        public TreeNode BuildFieldNode(IFieldDefinition field)
        {
            var node = new TreeNode();
            CreateTreeNode(new FieldNodeTag<TreeNode>(node, field));

            return node;
        }

        public TreeNode BuildEventNode(IEventDefinition @event)
        {
            var node = new TreeNode();
            CreateTreeNode(new EventNodeTag<TreeNode>(node, @event));

            foreach (var m in @event.Accessors)
                node.Nodes.Add(BuildMethodNode(m.ResolvedMethod));

            return node;
        }

        public TreeNode BuildPropertyNode(IPropertyDefinition property)
        {
            var node = new TreeNode();
            CreateTreeNode(new PropertyNodeTag<TreeNode>(node, property));

            foreach (var m in property.Accessors)
                node.Nodes.Add(BuildMethodNode(m.ResolvedMethod));

            return node;
        }

        public TreeNode BuildMethodNode(IMethodDefinition method)
        {
            var node = new TreeNode();
            CreateTreeNode(new MethodNodeTag<TreeNode>(node, method));

            return node;
        }

        static public TreeNode BuildFieldNodes(IFieldDefinition field)
        {
            var node = new TreeNode();
            CreateTreeNodes(new FieldNodeTag<TreeNode>(node, field));

            return node;
        }

        static public TreeNode BuildEventNodes(IEventDefinition @event)
        {
            var node = new TreeNode();
            CreateTreeNodes(new EventNodeTag<TreeNode>(node, @event));

            foreach (var m in @event.Accessors)
                node.Nodes.Add(BuildMethodNodes(m.ResolvedMethod));

            return node;
        }

        static public TreeNode BuildPropertyNodes(IPropertyDefinition property)
        {
            var node = new TreeNode();
            CreateTreeNodes(new PropertyNodeTag<TreeNode>(node, property));

            foreach (var m in property.Accessors)
                node.Nodes.Add(BuildMethodNodes(m.ResolvedMethod));

            return node;
        }

        static public TreeNode BuildMethodNodes(IMethodDefinition method)
        {
            var node = new TreeNode();
            CreateTreeNodes(new MethodNodeTag<TreeNode>(node, method));

            return node;
        }

        /// <summary>
        /// Create a tree node with the specified tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void CreateTreeNode(IAssemblyTreeNode<TreeNode> node)
        {
            if (node == null)
                throw new ArgumentNullException("tag");

            node.UITreeNode.Text = node.Title;
            node.UITreeNode.ImageKey = node.ImageKey;
            node.UITreeNode.SelectedImageKey = node.ImageKey;
            node.UITreeNode.ForeColor = node.TextColor;
            node.UITreeNode.Tag = node;
        }

        static public void CreateTreeNodes(IAssemblyTreeNode<TreeNode> node)
        {
            if (node == null)
                throw new ArgumentNullException("tag");

            node.UITreeNode.Text = node.Title;
            node.UITreeNode.ImageKey = node.ImageKey;
            node.UITreeNode.SelectedImageKey = node.ImageKey;
            node.UITreeNode.ForeColor = node.TextColor;
            node.UITreeNode.Tag = node;
        }
    }
}