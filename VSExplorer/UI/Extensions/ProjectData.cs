using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace WinExplorer
{
    public class EnumConverters : EnumConverter
    {
        public EnumConverters(Type T) : base(T)
        {
            this.T = T;
        }

        private Type T;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
                                      object value)
        {
            foreach (FieldInfo fi in T.GetFields())
            {
                DescriptionAttribute des = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                            typeof(DescriptionAttribute));
                if ((des != null) && ((string)value == des.Description))
                    return Enum.Parse(T, fi.Name);
            }
            return Enum.Parse(T, (string)value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                   object value, Type destType)
        {
            FieldInfo fi = T.GetField(Enum.GetName(T, value));
            DescriptionAttribute des = (DescriptionAttribute)Attribute.GetCustomAttribute(fi,
                                        typeof(DescriptionAttribute));
            if (des != null)
                return des.Description;
            else
                return value.ToString();
        }
    }

    public enum BuildAction
    {
        [Description("Compile")]
        Compile,

        [Description("Content")]
        Content,

        [Description("Embedded Resource")]
        EmbeddedResource,

        [Description("Additional Files")]
        AdditionalFiles,

        [Description("Code Analysis Dictionary")]
        CodeAnalysisDictionary,

        [Description("Application Definition")]
        ApplicationDefinition,

        [Description("Page")]
        Page,

        [Description("Resource")]
        Resource,

        [Description("Splash Screen")]
        SplashScreen,

        [Description("Design Data")]
        DesignData
    }

    public enum CopyToOutput
    {
        [Description("Copy always")]
        CopyAlways,

        [Description("Do not copy")]
        DoNotCopy,

        [Description("Copy if newer")]
        CopyIfNewer
    }

    public class ProjectFileData : object
    {
        [CategoryAttribute("Advanced")]
        [DisplayName("Build Action")]
        [Description("Build Action")]
        [TypeConverter(typeof(EnumConverters))]
        public BuildAction ba { get; set; }

        [CategoryAttribute("Advanced")]
        [DisplayName("Copy To Output Directory")]
        [Description("Copy To Output Directory")]
        [TypeConverter(typeof(EnumConverters))]
        public CopyToOutput cp { get; set; }

        [CategoryAttribute("Advanced")]
        [DisplayName("Custom Tool")]
        [Description("Custom Tool")]
        public string ts { get; set; }

        [CategoryAttribute("Advanced")]
        [DisplayName("Custom Tool Namespace")]
        [Description("Custom Tool Namespace")]
        public string tn { get; set; }

        [CategoryAttribute("Misc")]
        [DisplayName("File Name")]
        [Description("File Name")]
        public string file { get; set; }

        [CategoryAttribute("Misc")]
        [DisplayName("Full Path")]
        [Description("Full Path")]
        public string filePath { get; set; }
        private object m_data;
        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public ProjectFileData(object obs)
        {
            this.data = obs;
        }
    }


    public class ProjectFile : object
    {
  
        [CategoryAttribute("Misc")]
        [DisplayName("Project File")]
        [Description("Project File")]
        public string file { get; set; }

        [CategoryAttribute("Misc")]
        [DisplayName("Project Folder")]
        [Description("Project Folder")]
        public string filePath { get; set; }
        private object m_data;

        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public ProjectFile(object obs)
        {
            this.data = obs;

            CreateView_Solution.ProjectItemInfo p = obs as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            file = Path.GetFileName(p.ps.FileName);
            filePath = Path.GetDirectoryName(p.ps.FileName);
        }
    }
    public class SolutionFolder : object
    {

        [CategoryAttribute("Misc")]
        [DisplayName("(Name)")]
        [Description("(Name)")]
        public string folderName { get; set; }
        private object m_data;

        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public SolutionFolder(object obs)
        {
            this.data = obs;

            CreateView_Solution.ProjectItemInfo p = obs as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            folderName = Path.GetFileName(p.ps.FileName);
            
        }
    }
    public class ProjectFolder : object
    {

        [CategoryAttribute("Misc")]
        [DisplayName("Folder Name")]
        [Description("Folder Name")]
        public string folderName { get; set; }
        private object m_data;

        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public ProjectFolder(object obs)
        {
            this.data = obs;

            CreateView_Solution.ProjectItemInfo p = obs as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            folderName = Path.GetFileName(p.ps.FileName);

        }
    }
    public class SolutionFile : object
    {

        [CategoryAttribute("Misc")]
        [DisplayName("Solution File")]
        [Description("Solution File")]
        public string file { get; set; }

        [CategoryAttribute("Misc")]
        [DisplayName("Solution Folder")]
        [Description("Solution Folder")]
        public string filePath { get; set; }
        private object m_data;

        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public SolutionFile(object obs)
        {
            this.data = obs;

            CreateView_Solution.ProjectItemInfo p = obs as CreateView_Solution.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            file = Path.GetFileName(p.ps.FileName);
            filePath = Path.GetDirectoryName(p.ps.FileName);
        }
    }
    class ProjectItemInfoConverter : TypeConverter
    {
        // Return true if we need to convert from a string.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(CreateView_Solution.ProjectItemInfo)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        // Return true if we need to convert into a string.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(CreateView_Solution.ProjectItemInfo)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        // Convert from a ProjectItemInfo
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(CreateView_Solution.ProjectItemInfo))
            {
                return value;
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }

        
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(CreateView_Solution.ProjectItemInfo)) return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Return true to indicate that the object supports properties.
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Return a property description collection.
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value);
        }
    }

}