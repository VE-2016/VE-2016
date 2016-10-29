using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    /// <summary>
    /// Base interface of objects that associated with the tree node tag.
    /// </summary>
    /// <typeparam name="TUITreeNode">
    /// Type of the relative UI tree node.
    /// </typeparam>
    public interface IAssemblyTreeNode<TUITreeNode>
        where TUITreeNode : class
    {
        /// <summary>
        /// Get the relative UI tree node.
        /// </summary>
        TUITreeNode UITreeNode { get; }

        /// <summary>
        /// Get the type of the node.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        /// Get the node title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Get the data (such as assembly, module, class and so on)
        /// associated with the node. Type of the data depends on
        /// the value of the <see cref="Type"/> property.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Get the object contains the properties of the node,
        /// will be shown in the property grid control.
        /// </summary>
        /// <returns></returns>
        object PropertyObject { get; }

        /// <summary>
        /// Get the image key that will affect the display image
        /// </summary>
        /// <returns></returns>
        string ImageKey { get; }

        /// <summary>
        /// Get the text color of the tree node.
        /// </summary>
        /// <returns></returns>
        Color TextColor { get; }
    }
}