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

            //imgList.Images.Add(Keys.AssemblyImage, ve_resource.AssemblyImage);
            //imgList.Images.Add(Keys.AssemblyImage, ve_resource.AssemblyImage);
            //imgList.Images.Add(Keys.ClassImage, ve_resource.Class_yellow_256x);
            //imgList.Images.Add(Keys.ClassInternalImage, ve_resource.ClassInternalImage);
            //imgList.Images.Add(Keys.ClassPrivateImage, ve_resource.ClassPrivateImage);
            //imgList.Images.Add(Keys.ClassProtectedImage, ve_resource.ClassProtectedImage);
            //imgList.Images.Add(Keys.ConstantImage, ve_resource.ConstantImage);
            //imgList.Images.Add(Keys.ConstantInternalImage, ve_resource.ConstantInternalImage);
            //imgList.Images.Add(Keys.ConstantPrivateImage, ve_resource.ConstantPrivateImage);
            //imgList.Images.Add(Keys.ConstantProtectedImage, ve_resource.ConstantProtectedImage);
            //imgList.Images.Add(Keys.DelegateImage, ve_resource.DelegateImage);
            //imgList.Images.Add(Keys.DelegateInternalImage, ve_resource.DelegateInternalImage);
            //imgList.Images.Add(Keys.DelegatePrivateImage, ve_resource.DelegatePrivateImage);
            //imgList.Images.Add(Keys.DelegateProtectedImage, ve_resource.DelegateProtectedImage);
            //imgList.Images.Add(Keys.EnumImage, ve_resource.EnumImage);
            //imgList.Images.Add(Keys.EnumInternalImage, ve_resource.EnumInternalImage);
            //imgList.Images.Add(Keys.EnumItemImage, ve_resource.EnumItemImage);
            //imgList.Images.Add(Keys.EnumPrivateImage, ve_resource.EnumPrivateImage);
            //imgList.Images.Add(Keys.EnumProtectedImage, ve_resource.EnumProtectedImage);
            //imgList.Images.Add(Keys.ErrorImage, ve_resource.ErrorImage);
            //imgList.Images.Add(Keys.EventImage, ve_resource.EventImage);
            //imgList.Images.Add(Keys.EventInternalImage, ve_resource.EventInternalImage);
            //imgList.Images.Add(Keys.EventPrivateImage, ve_resource.EventPrivateImage);
            //imgList.Images.Add(Keys.EventProtectedImage, ve_resource.EventProtectedImage);
            //imgList.Images.Add(Keys.FieldImage, ve_resource.FieldImage);
            //imgList.Images.Add(Keys.FieldInternalImage, ve_resource.FieldInternalImage);
            //imgList.Images.Add(Keys.FieldPrivateImage, ve_resource.FieldPrivateImage);
            //imgList.Images.Add(Keys.FieldProtectedImage, ve_resource.FieldProtectedImage);
            //imgList.Images.Add(Keys.FolderImage, ve_resource.FolderImage);
            //imgList.Images.Add(Keys.GrayFolderImage, ve_resource.GrayFolderImage);
            //imgList.Images.Add(Keys.InterfaceImage, ve_resource.InterfaceImage);
            //imgList.Images.Add(Keys.InterfaceInternalImage, ve_resource.InterfaceInternalImage);
            //imgList.Images.Add(Keys.InterfacePrivateImage, ve_resource.InterfacePrivateImage);
            //imgList.Images.Add(Keys.InterfaceProtectedImage, ve_resource.InterfaceProtectedImage);
            //imgList.Images.Add(Keys.MethodImage, ve_resource.MethodImage);
            //imgList.Images.Add(Keys.MethodInternalImage, ve_resource.MethodInternalImage);
            //imgList.Images.Add(Keys.MethodPrivateImage, ve_resource.MethodPrivateImage);
            //imgList.Images.Add(Keys.MethodProtectedImage, ve_resource.MethodProtectedImage);
            //imgList.Images.Add(Keys.ModuleImage, ve_resource.ModuleImage);
            //imgList.Images.Add(Keys.NamespaceImage, ve_resource.NamespaceImage);
            //imgList.Images.Add(Keys.PropertyImage, ve_resource.PropertyImage);
            //imgList.Images.Add(Keys.PropertyInternalImage, ve_resource.PropertyInternalImage);
            //imgList.Images.Add(Keys.PropertyPrivateImage, ve_resource.PropertyPrivateImage);
            //imgList.Images.Add(Keys.PropertyProtectedImage, ve_resource.PropertyProtectedImage);
            //imgList.Images.Add(Keys.StructureImage, ve_resource.StructureImage);
            //imgList.Images.Add(Keys.StructureInternalImage, ve_resource.StructureInternalImage);
            //imgList.Images.Add(Keys.StructurePrivateImage, ve_resource.StructurePrivateImage);
            //imgList.Images.Add(Keys.StructureProtectedImage, ve_resource.StructureProtectedImage);

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