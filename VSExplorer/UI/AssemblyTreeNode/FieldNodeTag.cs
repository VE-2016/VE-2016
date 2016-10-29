using Microsoft.Cci;
using System;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    public class FieldNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Field; }
        }

        public string Title
        {
            get { return this.Field.Name.Value; }
        }

        public object Data
        {
            get { return this.Field; }
        }

        public object PropertyObject
        {
            get
            {
                return this.Field;
            }
        }

        public string ImageKey
        {
            get
            {
                if (this.Field.ContainingTypeDefinition.IsEnum
                    && this.Field.IsCompileTimeConstant)
                    return NodeImages.Keys.EnumItemImage;

                switch (this.Field.Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.FieldPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.FieldProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.FieldInternalImage;

                    default:
                        return NodeImages.Keys.FieldImage;
                }
            }
        }

        public Color TextColor
        {
            get
            {
                switch (this.Field.Visibility)
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

        public FieldNodeTag(TUITreeNode treeNode, IFieldDefinition field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Field = field;
        }

        public IFieldDefinition Field { get; private set; }

        public string GetTypeName()
        {
            return Field.Type.ToString();
        }

        public string GetName()
        {
            return Field.Name.ToString();
        }
    }
}