using System;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    internal class ErrorNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>

        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Error; }
        }

        public string Title
        {
            get { return this.ErrorMessage; }
        }

        public object Data
        {
            get { return this.ErrorMessage; }
        }

        public object PropertyObject
        {
            get { return null; }
        }

        public string ImageKey
        {
            get { return NodeImages.Keys.ErrorImage; }
        }

        public Color TextColor
        {
            get { return NodeTextColor.Public; }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        #region IWorkspaceUnitNodeTag

        // public WorkspaceUnit WorkspaceUnit { get; private set; }

        #endregion IWorkspaceUnitNodeTag

        public ErrorNodeTag(TUITreeNode treeNode, string errorMessage)
        {
            if (errorMessage == null)
                throw new ArgumentNullException("errorMessage");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}