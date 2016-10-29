using Microsoft.Cci;
using System;
using System.Drawing;
using AndersLiu.Reflector.Core;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    internal class AssemblyNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>,
        IWorkspaceUnitNodeTag
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Assembly; }
        }

        public string Title
        {
            get { return this.Assembly.Name.Value; }
        }

        public object Data
        {
            get { return this.Assembly; }
        }

        public object PropertyObject
        {
            get
            {
                return new
                {
                    Name = this.Assembly.Name.Value,
                    this.Assembly.Location,
                    this.Assembly.Culture,
                };
            }
        }

        public string ImageKey
        {
            get { return NodeImages.Keys.AssemblyImage; }
        }

        public Color TextColor
        {
            get { return NodeTextColor.Public; }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        #region IWorkspaceUnitNodeTag

        public WorkspaceUnit WorkspaceUnit { get; private set; }

        #endregion IWorkspaceUnitNodeTag

        /// <summary>
        /// Create a new node tag object.
        /// </summary>
        /// <param name="assembly"></param>
        public AssemblyNodeTag(TUITreeNode treeNode, IAssembly assembly, WorkspaceUnit unit)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Assembly = assembly;
            this.WorkspaceUnit = unit;
        }

        /// <summary>
        /// Get the assembly referenced by the current node.
        /// </summary>
        public IAssembly Assembly { get; private set; }
    }
}