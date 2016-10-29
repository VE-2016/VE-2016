using Microsoft.Cci;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    public class TypeNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Type; }
        }

        public string Title
        {
            get { return this.Type.Name.Value; }
        }

        public object Data
        {
            get { return this.Type; }
        }

        public object PropertyObject
        {
            get
            {
                return this.Type;
            }
        }

        public string ImageKey
        {
            get
            {
                if (this.Type.IsInterface)
                    return GetInterfaceImageKey(this.Type);
                else if (this.Type.IsEnum)
                    return GetEnumImageKey(this.Type);
                else if (this.Type.IsDelegate)
                    return GetDelegateImageKey(this.Type);
                else if (this.Type.IsStruct)
                    return GetStructureImageKey(this.Type);
                else
                    return GetClassImageKey(this.Type);
            }
        }

        public Color TextColor
        {
            get
            {
                if (this.Type is INamespaceTypeDefinition)
                {
                    return (this.Type as INamespaceTypeDefinition).IsPublic
                        ? SystemColors.ControlText
                        : SystemColors.GrayText;
                }
                else if (this.Type is INestedTypeDefinition)
                {
                    var t = this.Type as INestedTypeDefinition;
                    switch (t.Visibility)
                    {
                        case TypeMemberVisibility.Public:
                        case TypeMemberVisibility.Family:
                        case TypeMemberVisibility.FamilyOrAssembly:
                            return NodeTextColor.Public;

                        default:
                            return NodeTextColor.NonPublic;
                    }
                }
                else
                {
                    return SystemColors.ControlText;
                }
            }
        }

        #endregion IAssemblyTreeNode<TUITreeNode>

        public TypeNodeTag(TUITreeNode treeNode, INamedTypeDefinition type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            bs = type.BaseClasses;

            B = new ArrayList();

            foreach (ITypeReference r in bs)
            {
                B.Add(r.ToString());

                //if (r.ResolvedType.BaseClasses != null)
                //{
                //    foreach (ITypeReference rr in r.ResolvedType.BaseClasses)
                //        B.Add(rr.ToString());
                //}
            }

            this.UITreeNode = treeNode;
            this.Type = type;
        }

        public ArrayList B { get; set; }

        public INamedTypeDefinition Type { get; private set; }

        public string GetTypeFullName()
        {
            string s = Type.ToString();

            return s;
        }

        public string GetTypeSignature()
        {
            StringBuilder b = new StringBuilder();

            string cc = Type.Name.ToString();

            b.Append("class ");
            b.AppendLine(cc + " () ");
            b.AppendLine("{");

            foreach (ITypeDefinitionMember d in Type.Members)
            {
                string a = d.Name.ToString();

                b.AppendLine();
                b.AppendLine(d + " () {...}");
                //b.AppendLine("{");

                b.AppendLine();
                //b.AppendLine("}");
            }

            b.AppendLine();
            b.AppendLine("}");

            return b.ToString();
        }

        public string GetTypeName()
        {
            string s = Type.Name.Value;

            return s;
        }

        private string GetInterfaceImageKey(INamedTypeDefinition type)
        {
            if (type is INamespaceTypeDefinition)
            {
                return (type as INamespaceTypeDefinition).IsPublic
                    ? NodeImages.Keys.InterfaceImage
                    : NodeImages.Keys.InterfaceInternalImage;
            }
            else if (type is INestedTypeDefinition)
            {
                switch ((type as INestedTypeDefinition).Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.InterfacePrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.InterfaceProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.InterfaceInternalImage;

                    default:
                        return NodeImages.Keys.InterfaceImage;
                }
            }
            else
            {
                return NodeImages.Keys.InterfaceImage;
            }
        }

        private string GetEnumImageKey(INamedTypeDefinition type)
        {
            if (type is INamespaceTypeDefinition)
            {
                return (type as INamespaceTypeDefinition).IsPublic
                    ? NodeImages.Keys.EnumImage
                    : NodeImages.Keys.EnumInternalImage;
            }
            else if (type is INestedTypeDefinition)
            {
                switch ((type as INestedTypeDefinition).Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.EnumPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.EnumProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.EnumInternalImage;

                    default:
                        return NodeImages.Keys.EnumImage;
                }
            }
            else
            {
                return NodeImages.Keys.EnumImage;
            }
        }

        private string GetDelegateImageKey(INamedTypeDefinition type)
        {
            if (type is INamespaceTypeDefinition)
            {
                return (type as INamespaceTypeDefinition).IsPublic
                    ? NodeImages.Keys.DelegateImage
                    : NodeImages.Keys.DelegateInternalImage;
            }
            else if (type is INestedTypeDefinition)
            {
                switch ((type as INestedTypeDefinition).Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.DelegatePrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.DelegateProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.DelegateInternalImage;

                    default:
                        return NodeImages.Keys.DelegateImage;
                }
            }
            else
            {
                return NodeImages.Keys.DelegateImage;
            }
        }

        private string GetStructureImageKey(INamedTypeDefinition type)
        {
            if (type is INamespaceTypeDefinition)
            {
                return (type as INamespaceTypeDefinition).IsPublic
                    ? NodeImages.Keys.StructureImage
                    : NodeImages.Keys.StructureInternalImage;
            }
            else if (type is INestedTypeDefinition)
            {
                switch ((type as INestedTypeDefinition).Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.StructurePrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.StructureProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.StructureInternalImage;

                    default:
                        return NodeImages.Keys.StructureImage;
                }
            }
            else
            {
                return NodeImages.Keys.StructureImage;
            }
        }

        public IEnumerable<ITypeReference> bs { get; set; }

        private string GetClassImageKey(INamedTypeDefinition type)
        {
            if (type is INamespaceTypeDefinition)
            {
                return (type as INamespaceTypeDefinition).IsPublic
                    ? NodeImages.Keys.ClassImage
                    : NodeImages.Keys.ClassInternalImage;
            }
            else if (type is INestedTypeDefinition)
            {
                switch ((type as INestedTypeDefinition).Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.ClassPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.ClassProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.ClassInternalImage;

                    default:
                        return NodeImages.Keys.ClassImage;
                }
            }
            else
            {
                return NodeImages.Keys.ClassImage;
            }
        }
    }
}