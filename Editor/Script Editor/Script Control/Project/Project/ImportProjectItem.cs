using System;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
    /// <summary>
    /// Project item for default namespace import (e.g in VB)
    /// </summary>
    public sealed class ImportProjectItem : ProjectItem
    {
        public ImportProjectItem(IProject project, string include)
            : base(project, ItemType.Import, include)
        {
        }
    }
}
