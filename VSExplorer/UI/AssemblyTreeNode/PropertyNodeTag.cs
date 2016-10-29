using Microsoft.Cci;
using System;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    public class PropertyNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Property; }
        }

        public string Title
        {
            get { return this.Property.Name.Value; }
        }

        public object Data
        {
            get { return this.Property; }
        }

        public object PropertyObject
        {
            get
            {
                return this.Property;
            }
        }

        public string ImageKey
        {
            get
            {
                switch (this.Property.Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.PropertyPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.PropertyProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.PropertyInternalImage;

                    default:
                        return NodeImages.Keys.PropertyImage;
                }
            }
        }

        public Color TextColor
        {
            get
            {
                switch (this.Property.Visibility)
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

        public PropertyNodeTag(TUITreeNode treeNode, IPropertyDefinition property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Property = property;
        }

        public IPropertyDefinition Property { get; private set; }

        public string GetTypeName()
        {
            return Property.Type.ToString();
        }

        public string GetName()
        {
            return Property.Name.ToString();
        }
    }
}