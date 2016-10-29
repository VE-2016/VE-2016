using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
    /// <summary>
    /// A project item whose type is not known by Script Editor.
    /// </summary>
    public sealed class UnknownProjectItem : ProjectItem
    {
        /// <summary>
        /// Constructor for internal use in ProjectDescriptor.
        /// </summary>
        internal UnknownProjectItem(IProject project, string itemType, string include)
            : this(project, itemType, include, false)
        {
        }

        /// <summary>
        /// Constructor for internal use in ProjectDescriptor.
        /// </summary>
        internal UnknownProjectItem(IProject project, string itemType, string include, bool treatIncludeAsLiteral)
            : base(project, new ItemType(itemType), include, treatIncludeAsLiteral)
        {
        }
    }
}
