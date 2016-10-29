// VSProjectItem

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

//using AbstractX.Contracts;

namespace VSProvider
{
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class VSProjectItem
    {
        private static readonly Type s_ProjectItemElement;
        private static readonly PropertyInfo s_ProjectItemElement_ItemType;
        private static readonly PropertyInfo s_ProjectItemElement_Include;

        private VSProject project;
        private object internalProjectItem;
        public string fileName;

        static VSProjectItem()
        {
            s_ProjectItemElement = Type.GetType("Microsoft.Build.Construction.ProjectItemElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectItemElement_ItemType = s_ProjectItemElement.GetProperty("ItemType", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_Include = s_ProjectItemElement.GetProperty("Include", BindingFlags.Public | BindingFlags.Instance);
        }

        public string ItemType { get; set; }

        public string Include { get; set; }


        public VSProjectItem()
        {

        }

        public VSProjectItem(VSProject project, object internalProjectItem)
        {
            this.ItemType = s_ProjectItemElement_ItemType.GetValue(internalProjectItem, null) as string;
            this.Include = s_ProjectItemElement_Include.GetValue(internalProjectItem, null) as string;
            this.project = project;
            this.internalProjectItem = internalProjectItem;

            // todo - expand this

            if (this.ItemType == "Compile" || this.ItemType == "EntityDeploy")
            {
                var file = new FileInfo(project.FileName);

                fileName = Path.Combine(file.DirectoryName, this.Include);
            }
        }

        public byte[] FileContents
        {
            get
            {
                return File.ReadAllBytes(fileName);
            }
        }

        public string Name
        {
            get
            {
                if (fileName != null)
                {
                    var file = new FileInfo(fileName);

                    return file.Name;
                }
                else
                {
                    return this.Include;
                }
            }
        }
    }
}