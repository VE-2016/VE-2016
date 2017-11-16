using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using VSProvider;

namespace WinExplorer
{
    public class EnumConverters : EnumConverter
    {
        public EnumConverters(Type T) : base(T)
        {
            this.Te = T;
        }

        public Type Te { get; set; }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,
                                      object value)
        {
            foreach (FieldInfo fi in Te.GetFields())
            {
                DescriptionAttribute des = (DescriptionAttribute)System.Attribute.GetCustomAttribute(fi,
                                            typeof(DescriptionAttribute));
                if ((des != null) && ((string)value == des.Description))
                    return Enum.Parse(Te, fi.Name);
            }
            return Enum.Parse(Te, (string)value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
                                   object value, Type destType)
        {
            FieldInfo fi = Te.GetField(Enum.GetName(Te, value));
            DescriptionAttribute des = (DescriptionAttribute)System.Attribute.GetCustomAttribute(fi,
                                        typeof(DescriptionAttribute));
            if (des != null)
                return des.Description;
            else
                return value.ToString();
        }
        private bool ShouldSerializeTe() { return false; }
        private void ResetTe() {  }
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

        [DisplayName("Build Action")]
        [Description("How the file relates to the build and deployment processes")]
        [TypeConverter(typeof(EnumConverters))]
        public BuildAction ba { get; set; }


        [DisplayName("Copy To Output Directory")]
        [Description("Specifies the source file will be copied to the output directory")]
        [TypeConverter(typeof(EnumConverters))]
        public CopyToOutput cp { get; set; }


        [DisplayName("Custom Tool")]
        [Description("Specifies the tool that transforms a file at design time and places the output of that transformation into another For example a dataset (.xsd) file comes with default custom tool")]
        public string ts { get; set; }


        [DisplayName("Custom Tool Namespace")]
        [Description("The namespace into which the output of the custom tool is placed")]
        public string tn { get; set; }

        [DisplayName("File Name")]
        [Description("Name od the file or folder")]
        public string file { get; }

        [DisplayName("Full Path")]
        [Description("Location of the file")]
        public string filePath { get; }
        private object m_data;

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
            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            file = Path.GetFileName(p.ps.FileName);
            filePath = Path.GetDirectoryName(p.ps.FileName);
        }
        private void ResetfilePath(){ }
        private bool ShouldSerializefilePath(){ return false; }
        private void Resetfile(){ }
        private bool ShouldSerializefile(){ return false; }
        private void ResetCopyba() { }
        private bool ShouldSerializeba() { return false; }
        private void ResetCopycp(){ }
        private bool ShouldSerializecp(){ return false; }
    }

    public class ProjectReferenceItem : object
    {

        [DisplayName("(Name)")]
        [Description("Display name of the reference.")]
        public string Name { get; set; }

        [DisplayName("Aliases")]
        [Description("Comma separated name of the aliases.")]
        public string Aliases { get; set; }

        [DisplayName("Copy Local")]
        [Description("Copy local.")]
        public bool CopyLocal { get; set; }

        [DisplayName("Culture")]
        [Description("Culture.")]
        public string Culture { get; set; }

        [DisplayName("Description")]
        [Description("Description.")]
        public string Description { get; set; }

        [DisplayName("Embed Interop Types")]
        [Description("Interop.")]
        public bool Interop { get; set; }

        [DisplayName("File Type")]
        [Description("File Type.")]
        public string FileType { get; set; }

        [DisplayName("Identity")]
        [Description("Identity.")]
        public string Identity { get; set; }

        [DisplayName("Path")]
        [Description("Path.")]
        public string Path { get; set; }

        [DisplayName("Resloved")]
        [Description("Resolved.")]
        public bool Resolved { get; set; }

        [DisplayName("Runtime version")]
        [Description("Runtime version.")]
        public string Runtime { get; set; }

        [DisplayName("Specific version")]
        [Description("Specific version.")]
        public bool SpecificVersion { get; set; }

        [DisplayName("Strong Name")]
        [Description("Strong Name.")]
        public bool StrongName { get; set; }

        [DisplayName("Version")]
        [Description("version.")]
        public bool Version { get; set; }

        private object m_data;
        [DisplayName("Data")]
        [Description("Data")]
        [TypeConverter(typeof(ProjectItemInfoConverter))]
        public object data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        public ProjectReferenceItem(object obs)
        {
            this.data = obs;

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            Name = p.Include;
           
        }
        private void ResetfileName() { }
        private bool ShouldSerializeName() { return false; }
        private void ResetAliases() { }
        private bool ShouldSerializeAliases() { return false; }
        private void ResetfileCopyLocal() { }
        private bool ShouldSerializeCopyLocal() { return false; }
        private void ResetCulture() { }
        private bool ShouldSerializeCulture() { return false; }
        private void ResetfileDescription() { }
        private bool ShouldSerializeDescription() { return false; }
        private void ResetInterop() { }
        private bool ShouldSerializeInterop() { return false; }
        private void ResetfileFileType() { }
        private bool ShouldSerializeFileType() { return false; }
        private void ResetIdentity() { }
        private bool ShouldSerializeIdentity() { return false; }
        private void ResetfilePath() { }
        private bool ShouldSerializePath() { return false; }
        private void ResetResolved() { }
        private bool ShouldSerializeResolved() { return false; }
        private void ResetfileRuntime() { }
        private bool ShouldSerializeRuntime() { return false; }
        private void ResetSpecificVersion() { }
        private bool ShouldSerializeSpecificVersion() { return false; }
        private void ResetfileStrongName() { }
        private bool ShouldSerializeStrongName() { return false; }
        private void ResetVersion() { }
        private bool ShouldSerializeVersion() { return false; }
        private void ResetData() { }
        private bool ShouldSerializeData() { return false; }

    }
    public class ProjectFile : object
    {
  
        [DisplayName("Project File")]
        [Description("The name of the file containing build, configuration, and other information about the project")]
        public string file { get; set; }

        [DisplayName("Project Folder")]
        [Description("The location of the project file")]
        public string filePath { get; set; }
        private object m_data;

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

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            file = Path.GetFileName(p.ps.FileName);
            filePath = Path.GetDirectoryName(p.ps.FileName);
        }
        private void ResetfilePath() { }
        private bool ShouldSerializefilePath() { return false; }
        private void Resetfile() { }
        private bool ShouldSerializefile() { return false; }
    }
    public class ProjectTypeFromFile : object
    {

        [DisplayName("Name")]
        [Description("The name of this item")]
        public string Name { get; set; }

        public ProjectTypeFromFile(object obs)
        {
            

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            EntityDeclaration e = p.mapper;
            if (e == null)
                return;
            Name = e.Name;
            if(e is FieldDeclaration)
            {
                FieldDeclaration f = e as FieldDeclaration;
                Name = f.Name;
                if (string.IsNullOrEmpty(Name))
                    if (f.Variables != null)
                        if (f.Variables.Count > 0)
                            Name = f.Variables.ToList()[0].Name;
            }
            
            
        }
        public static string Description(EntityDeclaration e)
        {
            if (e == null)
                return "";
            var t = e.GetType();
            if (e is MethodDeclaration)
                return "Method";
            else if (e is ConstructorDeclaration)
                return "Constructor";
            else if (e is PropertyDeclaration)
                return "Property";
            else if (e is DelegateDeclaration)
                return "Delegate";
            else if (e is FieldDeclaration)
                return "Field";

            if (t.IsClass)
                return "Class";
            else if (t.IsEnum)
                return "Enum";
            else if (t.IsInterface)
                return "Interface";
            
            else return "";


        }
        private void ResetName() { }
        private bool ShouldSerializeName() { return false; }
    }

    public class SolutionFolder : object
    {
        
        [DisplayName("(Name)")]
        [Description("(The name of the Solution Folder")]
        public string folderName { get; set; }
        private object m_data;
        
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

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            folderName = Path.GetFileName(p.ps.FileName);
            
        }
        private void ResetfolderName() { }
        private bool ShouldSerializefolderName() { return false; }
     }
    public class ProjectFolder : object
    {
        [DisplayName("Folder Name")]
        [Description("Name of this folder")]
        public string folderName { get; set; }
        private object m_data;

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

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps == null)
                return;
            folderName = p.Include;

        }
        private void ResetfolderName() { }
        private bool ShouldSerializefolderName() { return false; }

    }
    public class SolutionFile : object
    {

        //[CategoryAttribute("Misc")]
        [DisplayName("(Name)")]
        [Description("The name of the solution file")]
        public string file { get; set; }

              
        [DisplayName("Active config")]
        [Description("The configuration to build for the soluton. Access the Property Pages dialog box for the solution to modify the solutions's configurations")]
        public DropDownListProperty conf { get; set; }
        private object m_data;

        
        [DisplayName("Description")]
        [Description("Text that will be placed in the solution file that describes the solution")]
        
        public string description { get; set; }
        private object m_desc;

        
        [DisplayName("Lightweight load")]
        [Description("Load projects as necessary")]
        public string load { get; }

        
        [DisplayName("Path")]
        
        [Description("The path to the solution file")]
        public String/*PropertyDescriptor*/ filePath { get; set; }
        private object m_path;

        
        [DisplayName("Startup project")]
        [Description("Specify which project will start when you run debugger")]
        [DefaultValue("")]
        public string startup { get; set; }
        
        
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

            VSParsers.ProjectItemInfo p = obs as VSParsers.ProjectItemInfo;
            if (p == null)
                return;
            if (p.ps != null)
            {
                file = Path.GetFileName(p.ps.FileName);
                filePath = Path.GetDirectoryName(p.ps.FileName);//new StringPropertyDescriptor(Path.GetDirectoryName(p.ps.FileName), null);
            }
            VSSolution vs = p.vs;
            if (vs == null)
                return;
            Solution c = vs.SolutionParsed;
            var section = c.GetPresection("SolutionConfigurationPlatforms", "preSolution");
            if(section != null)
            {
                List<string> b = new List<string>();
                foreach (var s in section.Entries)
                    if(s.Value != "preSolution")
                    b.Add(s.Key);
                conf = new DropDownListProperty(b);
            }
            file = vs.Name;
            filePath = Path.GetDirectoryName(vs.solutionFileName);// new StringPropertyDescriptor(vs.solutionFileName,null);
            load = "Default";
            startup = ExplorerForms.ef.Command_GetStartupProject();
        }
        private void ResetfilePath(){ }
        private bool ShouldSerializefilePath() { return false; }
        private void Resetload(){ }
        private bool ShouldSerializeload(){ return false; }
        private void Resetfile() { }
        private bool ShouldSerializefile() { return false; }
        private void Resetconf() { }
        private bool ShouldSerializeconf() { return false; }
    }
  
    [Editor(typeof(DropDownListPropertyEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class DropDownListProperty
    {
        private List<string> _values = new List<string>();

        public DropDownListProperty()
        {
            SelectedItem = "None";
        }

        public DropDownListProperty(List<String> values)
        {
            if (values.Count > 0)
                SelectedItem = values[0];
            else
                SelectedItem = "None";

            Values = values;
        }

        public List<string> Values
        {
            get
            {
                if (_values == null)
                    _values = new List<String>();

                return _values;
            }
            set
            {
                if (value != null)
                    _values = value;
            }
        }

        [Browsable(false)]
        public string SelectedItem { get; set; }

        /// <summary>
        /// The value that we return here will be shown in the property grid.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SelectedItem;
        }
    }

    /// <summary>
    /// Provides a user interface for selecting a state property.
    /// </summary>
    public class DropDownListPropertyEditor : UITypeEditor
    {
        #region Members

        private IWindowsFormsEditorService _service = null;

        #endregion

        /// <summary>
        /// Displays a list of available values for the specified component than sets the value.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
        /// <param name="provider">A service provider object through which editing services may be obtained.</param>
        /// <param name="value">An instance of the value being edited.</param>
        /// <returns>The new value of the object. If the value of the object hasn't changed, this method should return the same object it was passed.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                // This service is in charge of popping our ListBox.
                _service = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (_service != null && value is DropDownListProperty)
                {
                    var property = (DropDownListProperty)value;

                    var list = new ListBoxExtended();
                    
                    list.Click += ListBox_Click;
                    list.b.Click += B_Click; 

                    foreach (string item in property.Values)
                    {
                        list.Items.Add(item);
                    }

                    // Drop the list control.
                    _service.DropDownControl(list);

                    if (list.SelectedItem != null && list.SelectedIndex >= 0)
                    {
                        property.SelectedItem = list.SelectedItem.ToString();
                        value = property;
                    }
                }
            }

            return value;
        }

        private void B_Click(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }

     

        private void ListBox_Click(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }

        public class ListBoxExtended : UserControl
        {
            public ListBox b { get; set; }
            public ListBoxExtended()
            {
                b = new ListBox();
                b.Margin = new Padding(0, 0, 0, 40);
                b.Dock = DockStyle.Fill;
                b.Font = new System.Drawing.Font(b.Font.FontFamily, 10.5f);
                this.Controls.Add(b);
            }
            public ListBox.ObjectCollection Items
            {
                get
                {
                    return b.Items;
                }
                set
                {
                    b.Items.AddRange( value);
                    Items = value;
                }
            }
            public object SelectedItem
            {
                get
                {
                    return b.SelectedItem;
                }
                set
                {
                    b.SelectedItem = value;
                    SelectedItem = value;
                }
            }
            public int SelectedIndex
            {
                get
                {
                    return b.SelectedIndex;
                }
                set
                {
                    b.SelectedIndex = value;
                    SelectedIndex = value;
                }
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button == MouseButtons.Left)
                {
                    bIsResizing = true;
                    Cursor = Cursors.SizeAll;
                    oldPoint = e.Location;
                    oldSize = Size;
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (bIsResizing)
                {
                    Height = oldSize.Height + (e.Location.Y - oldPoint.Y);
                    Width = oldSize.Width + (-e.Location.X + oldPoint.X);
                }
            }
            public bool afterResize = false;

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (e.Button == MouseButtons.Left)
                {
                    bIsResizing = false;
                }
            }

            private bool bIsResizing;
            private Point oldPoint;
            private Size oldSize;
        }
        /// <summary>
        /// Gets the editing style of the <see cref="EditValue"/> method.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that can be used to gain additional context information.</param>
        /// <returns>Returns the DropDown style, since this editor uses a drop down list.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            // We're using a drop down style UITypeEditor.
            return UITypeEditorEditStyle.DropDown;
        }
    }
    class ProjectItemInfoConverter : TypeConverter
    {
        // Return true if we need to convert from a string.
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(VSParsers.ProjectItemInfo)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        // Return true if we need to convert into a string.
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(VSParsers.ProjectItemInfo)) return true;
            return base.CanConvertTo(context, destinationType);
        }

        // Convert from a ProjectItemInfo
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(VSParsers.ProjectItemInfo))
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
            if (destinationType == typeof(VSParsers.ProjectItemInfo)) return value.ToString();
            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Return true to indicate that the object supports properties.
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Return a property description collection.
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, System.Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value);
        }
    }

}