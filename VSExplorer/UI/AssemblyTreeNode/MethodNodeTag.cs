using Microsoft.Cci;
using System;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    public class MethodNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Method; }
        }

        public string Title
        {
            get { return this.Method.Name.Value; }
        }

        public object Data
        {
            get { return this.Method; }
        }

        public object PropertyObject
        {
            get
            {
                return this.Method;
            }
        }

        public string ImageKey
        {
            get
            {
                switch (this.Method.Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.MethodPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.MethodProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.MethodInternalImage;

                    default:
                        return NodeImages.Keys.MethodImage;
                }
            }
        }

        public Color TextColor
        {
            get
            {
                switch (this.Method.Visibility)
                {
                    case TypeMemberVisibility.Public:
                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeTextColor.Public;

                    default:
                        return NodeTextColor.NonPublic;
                }
            }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        public MethodNodeTag(TUITreeNode treeNode, IMethodDefinition method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Method = method;
        }

        public IMethodDefinition Method { get; private set; }

        public string GetTypeName()
        {
            return Method.Type.ToString();
        }

        public string GetName()
        {
            return Method.Name.ToString();
        }
    }
}