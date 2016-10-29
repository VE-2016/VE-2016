using System.Drawing;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    /// <summary>
    /// Provide color for node text.
    /// </summary>
    internal class NodeTextColor
    {
        /// <summary>
        /// Get color for the public members.
        /// </summary>
        public static Color Public { get { return SystemColors.ControlText; } }

        /// <summary>
        /// Get color for the non public members.
        /// </summary>
        public static Color NonPublic { get { return SystemColors.GrayText; } }
    }
}