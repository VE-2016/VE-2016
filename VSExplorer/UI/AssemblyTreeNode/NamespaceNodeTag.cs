using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    internal class NamespaceNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Namespace; }
        }

        public string Title
        {
            get { return this.Name; }
        }

        public object Data
        {
            get { return this.Name; }
        }

        public object PropertyObject
        {
            get { return new { this.Name }; }
        }

        public string ImageKey
        {
            get { return NodeImages.Keys.NamespaceImage; }
        }

        public Color TextColor
        {
            get { return NodeTextColor.Public; }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        public NamespaceNodeTag(TUITreeNode treeNode, string name,
            IEnumerable<INamedTypeDefinition> types)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (types == null)
                throw new ArgumentNullException("types");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Name = name;
            this.Types = types;
        }

        public string Name { get; private set; }

        public IEnumerable<INamedTypeDefinition> Types { get; private set; }
    }
}