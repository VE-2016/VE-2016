using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using System.Resources.Tools;
using VSProvider;
using System.Globalization;

namespace WinExplorer
{

    public class EnumConverters : EnumConverter
    {

        public EnumConverters(Type T) : base(T)
        {
            this.T = T;
        }

        Type T;

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

    public class ProjectFileData
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

        [CategoryAttribute("Misc")]
        [DisplayName("Data")]
        [Description("Data")]
        public object data { get; set; }

        public ProjectFileData(object obs)
        {
            this.data = obs;

        }
    }
}