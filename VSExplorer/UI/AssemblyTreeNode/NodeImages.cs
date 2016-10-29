using AndersLiu.Reflector.Core;
using System.Drawing;
using System.Windows.Forms;

using WinExplorer;

namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    public class NodeImages
    {
        public static ImageList CreateImageList()
        {
            var imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth24Bit,
                ImageSize = new Size(16, 16),
                TransparentColor = Color.Magenta,
            };

            imgList.Images.Add(Keys.AssemblyImage, Resources2.AssemblyImage);
            imgList.Images.Add(Keys.AssemblyImage, Resources2.AssemblyImage);
            imgList.Images.Add(Keys.ClassImage, Resources.Class_yellow_256x);
            imgList.Images.Add(Keys.ClassInternalImage, Resources2.ClassInternalImage);
            imgList.Images.Add(Keys.ClassPrivateImage, Resources2.ClassPrivateImage);
            imgList.Images.Add(Keys.ClassProtectedImage, Resources2.ClassProtectedImage);
            imgList.Images.Add(Keys.ConstantImage, Resources2.ConstantImage);
            imgList.Images.Add(Keys.ConstantInternalImage, Resources2.ConstantInternalImage);
            imgList.Images.Add(Keys.ConstantPrivateImage, Resources2.ConstantPrivateImage);
            imgList.Images.Add(Keys.ConstantProtectedImage, Resources2.ConstantProtectedImage);
            imgList.Images.Add(Keys.DelegateImage, Resources2.DelegateImage);
            imgList.Images.Add(Keys.DelegateInternalImage, Resources2.DelegateInternalImage);
            imgList.Images.Add(Keys.DelegatePrivateImage, Resources2.DelegatePrivateImage);
            imgList.Images.Add(Keys.DelegateProtectedImage, Resources2.DelegateProtectedImage);
            imgList.Images.Add(Keys.EnumImage, Resources2.EnumImage);
            imgList.Images.Add(Keys.EnumInternalImage, Resources2.EnumInternalImage);
            imgList.Images.Add(Keys.EnumItemImage, Resources2.EnumItemImage);
            imgList.Images.Add(Keys.EnumPrivateImage, Resources2.EnumPrivateImage);
            imgList.Images.Add(Keys.EnumProtectedImage, Resources2.EnumProtectedImage);
            imgList.Images.Add(Keys.ErrorImage, Resources2.ErrorImage);
            imgList.Images.Add(Keys.EventImage, Resources2.EventImage);
            imgList.Images.Add(Keys.EventInternalImage, Resources2.EventInternalImage);
            imgList.Images.Add(Keys.EventPrivateImage, Resources2.EventPrivateImage);
            imgList.Images.Add(Keys.EventProtectedImage, Resources2.EventProtectedImage);
            imgList.Images.Add(Keys.FieldImage, Resources2.FieldImage);
            imgList.Images.Add(Keys.FieldInternalImage, Resources2.FieldInternalImage);
            imgList.Images.Add(Keys.FieldPrivateImage, Resources2.FieldPrivateImage);
            imgList.Images.Add(Keys.FieldProtectedImage, Resources2.FieldProtectedImage);
            imgList.Images.Add(Keys.FolderImage, Resources2.FolderImage);
            imgList.Images.Add(Keys.GrayFolderImage, Resources2.GrayFolderImage);
            imgList.Images.Add(Keys.InterfaceImage, Resources2.InterfaceImage);
            imgList.Images.Add(Keys.InterfaceInternalImage, Resources2.InterfaceInternalImage);
            imgList.Images.Add(Keys.InterfacePrivateImage, Resources2.InterfacePrivateImage);
            imgList.Images.Add(Keys.InterfaceProtectedImage, Resources2.InterfaceProtectedImage);
            imgList.Images.Add(Keys.MethodImage, Resources2.MethodImage);
            imgList.Images.Add(Keys.MethodInternalImage, Resources2.MethodInternalImage);
            imgList.Images.Add(Keys.MethodPrivateImage, Resources2.MethodPrivateImage);
            imgList.Images.Add(Keys.MethodProtectedImage, Resources2.MethodProtectedImage);
            imgList.Images.Add(Keys.ModuleImage, Resources2.ModuleImage);
            imgList.Images.Add(Keys.NamespaceImage, Resources2.NamespaceImage);
            imgList.Images.Add(Keys.PropertyImage, Resources2.PropertyImage);
            imgList.Images.Add(Keys.PropertyInternalImage, Resources2.PropertyInternalImage);
            imgList.Images.Add(Keys.PropertyPrivateImage, Resources2.PropertyPrivateImage);
            imgList.Images.Add(Keys.PropertyProtectedImage, Resources2.PropertyProtectedImage);
            imgList.Images.Add(Keys.StructureImage, Resources2.StructureImage);
            imgList.Images.Add(Keys.StructureInternalImage, Resources2.StructureInternalImage);
            imgList.Images.Add(Keys.StructurePrivateImage, Resources2.StructurePrivateImage);
            imgList.Images.Add(Keys.StructureProtectedImage, Resources2.StructureProtectedImage);

            return imgList;
        }

        public class Keys
        {
            public const string AssemblyImage = "AssemblyImage";
            public const string ClassImage = "ClassImage";
            public const string ClassInternalImage = "ClassInternalImage";
            public const string ClassPrivateImage = "ClassPrivateImage";
            public const string ClassProtectedImage = "ClassProtectedImage";
            public const string ConstantImage = "ConstantImage";
            public const string ConstantInternalImage = "ConstantInternalImage";
            public const string ConstantPrivateImage = "ConstantPrivateImage";
            public const string ConstantProtectedImage = "ConstantProtectedImage";
            public const string DelegateImage = "DelegateImage";
            public const string DelegateInternalImage = "DelegateInternalImage";
            public const string DelegatePrivateImage = "DelegatePrivateImage";
            public const string DelegateProtectedImage = "DelegateProtectedImage";
            public const string EnumImage = "EnumImage";
            public const string EnumInternalImage = "EnumInternalImage";
            public const string EnumItemImage = "EnumItemImage";
            public const string EnumPrivateImage = "EnumPrivateImage";
            public const string EnumProtectedImage = "EnumProtectedImage";
            public const string ErrorImage = "ErrorImage";
            public const string EventImage = "EventImage";
            public const string EventInternalImage = "EventInternalImage";
            public const string EventPrivateImage = "EventPrivateImage";
            public const string EventProtectedImage = "EventProtectedImage";
            public const string FieldImage = "FieldImage";
            public const string FieldInternalImage = "FieldInternalImage";
            public const string FieldPrivateImage = "FieldPrivateImage";
            public const string FieldProtectedImage = "FieldProtectedImage";
            public const string FolderImage = "FolderImage";
            public const string GrayFolderImage = "GrayFolderImage";
            public const string InterfaceImage = "InterfaceImage";
            public const string InterfaceInternalImage = "InterfaceInternalImage";
            public const string InterfacePrivateImage = "InterfacePrivateImage";
            public const string InterfaceProtectedImage = "InterfaceProtectedImage";
            public const string MethodImage = "MethodImage";
            public const string MethodInternalImage = "MethodInternalImage";
            public const string MethodPrivateImage = "MethodPrivateImage";
            public const string MethodProtectedImage = "MethodProtectedImage";
            public const string ModuleImage = "ModuleImage";
            public const string NamespaceImage = "NamespaceImage";
            public const string PropertyImage = "PropertyImage";
            public const string PropertyInternalImage = "PropertyInternalImage";
            public const string PropertyPrivateImage = "PropertyPrivateImage";
            public const string PropertyProtectedImage = "PropertyProtectedImage";
            public const string StructureImage = "StructureImage";
            public const string StructureInternalImage = "StructureInternalImage";
            public const string StructurePrivateImage = "StructurePrivateImage";
            public const string StructureProtectedImage = "StructureProtectedImage";
        }
    }
}