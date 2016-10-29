using Microsoft.Cci;
using System;
using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    internal class EventNodeTag<TUITreeNode> :
        IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        #region IAssemblyTreeNode<TUITreeNode>

        public TUITreeNode UITreeNode { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Event; }
        }

        public string Title
        {
            get { return this.Event.Name.Value; }
        }

        public object Data
        {
            get { return this.Event; }
        }

        public object PropertyObject
        {
            get
            {
                return this.Event;
            }
        }

        public string ImageKey
        {
            get
            {
                switch (this.Event.Visibility)
                {
                    case TypeMemberVisibility.Private:
                        return NodeImages.Keys.EventPrivateImage;

                    case TypeMemberVisibility.Family:
                    case TypeMemberVisibility.FamilyOrAssembly:
                        return NodeImages.Keys.EventProtectedImage;

                    case TypeMemberVisibility.Assembly:
                    case TypeMemberVisibility.FamilyAndAssembly:
                        return NodeImages.Keys.EventInternalImage;

                    default:
                        return NodeImages.Keys.EventImage;
                }
            }
        }

        public Color TextColor
        {
            get
            {
                switch (this.Event.Visibility)
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

        public EventNodeTag(TUITreeNode treeNode, IEventDefinition @event)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            this.UITreeNode = treeNode;
            this.Event = @event;
        }

        public IEventDefinition Event { get; private set; }
    }
}