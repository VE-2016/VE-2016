using Microsoft.Cci;
using System;
using System.Drawing;
using AndersLiu.Reflector.Core;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    internal class ModuleNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>,
        IWorkspaceUnitNodeTag
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Module; }
        }

        public string Title
        {
            get { return this.Module.ModuleName.Value; }
        }

        public object Data
        {
            get { return this.Module; }
        }

        public object PropertyObject
        {
            get
            {
                return new
                {
                    Name = this.Module.Name.Value,
                    this.Module.Location,
                };
            }
        }

        public string ImageKey
        {
            get { return NodeImages.Keys.ModuleImage; }
        }

        public Color TextColor
        {
            get { return NodeTextColor.Public; }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        #region IWorkspaceUnitNodeTag

        public WorkspaceUnit WorkspaceUnit { get; private set; }

        #endregion IWorkspaceUnitNodeTag

        public ModuleNodeTag(TUITreeNode treeNode, IModule module, WorkspaceUnit unit)
        {
            if (module == null)
                throw new ArgumentNullException("module");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Module = module;
            this.WorkspaceUnit = unit;
        }

        public IModule Module { get; private set; }
    }
}