// VSProjectItem

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VSProvider
{
    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class VSProjectItem : object
    {
        private static readonly Type s_ProjectItemElement;
        private static readonly PropertyInfo s_ProjectItemElement_ItemType;
        private static readonly PropertyInfo s_ProjectItemElement_Include;

        public VSProject project;
        private object _internalProjectItem;
        public string fileName;
        public Microsoft.Build.Evaluation.ProjectMetadata mc;

        static VSProjectItem()
        {
            //s_ProjectItemElement = Type.GetType("Microsoft.Build.Construction.ProjectItemElement, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_ProjectItemElement = Type.GetType("Microsoft.Build.Construction.ProjectItemElement, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectItemElement_ItemType = s_ProjectItemElement.GetProperty("ItemType", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_Include = s_ProjectItemElement.GetProperty("Include", BindingFlags.Public | BindingFlags.Instance);
        }

        public string ItemType { get; set; }

        public string Include { get; set; }

        public string SubType { get; set; }

        public VSProjectItem()
        {
        }

        public VSProjectItem(VSProject project, object internalProjectItem)
        {
            this.ItemType = s_ProjectItemElement_ItemType.GetValue(internalProjectItem, null) as string;
            this.Include = s_ProjectItemElement_Include.GetValue(internalProjectItem, null) as string;
            this.project = project;
            _internalProjectItem = internalProjectItem;

            // todo - expand this

            if (this.ItemType == "Compile" || this.ItemType == "EntityDeploy")
            {
                var file = new FileInfo(project.FileName);

                fileName = Path.Combine(file.DirectoryName, this.Include);
            }
        }

        public string SubTypes { get; set; }

        public string Info { get; set; }

        public string GetSubType(out string dependentupon)
        {
            Microsoft.Build.Construction.ProjectItemElement c = (Microsoft.Build.Construction.ProjectItemElement)_internalProjectItem;
            
            dependentupon = "";

            string subtype = "";

            SubTypes = "";

            if (c != null)
                if (c.HasMetadata == true)
                {
                    foreach (Microsoft.Build.Construction.ProjectMetadataElement e in c.Metadata)
                    {
                        if (e.Name == "SubType")
                        {
                            SubTypes = e.Value;
                            subtype = e.Value;
                        }

                        if (e.Name == "Link")
                        {
                            Info = e.Value;
                            subtype = e.Value;
                        }
                        else if (e.Name.ToLower() == "dependentupon")
                        {
                            dependentupon = e.Value;
                        }
                    }
                }

            return subtype;
        }

        public byte[] FileContents
        {
            get
            {
                //if (File.Exists(fileName) == false)
                    return null;
                //return File.ReadAllBytes(fileName);
            }
        }

        public string Name
        {
            get
            {
                if (fileName != null)
                {
                    try
                    {
                        var file = new FileInfo(fileName);

                        return file.Name;
                    }
                    catch (Exception e)
                    {
                    }
                    return this.Include;
                }
                else
                {
                    return this.Include;
                }
            }
        }
    }
}