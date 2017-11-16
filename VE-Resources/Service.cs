using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinExplorers;

namespace ves
{
    public class Service
    {
        public static Service s { get; set; }

        static Service()
        {
            LoadDictionary();
            CreateImageList();
        }

        static ImageList imageList { get; set; }

        static public Dictionary<string, string> dc { get; set; }

        static public Bitmap GetBitmapFromExtension(string file)
        {
           if(file == "Start Page")
            {
                return (Bitmap)imageList.Images["startup"];
            }
            string exts = Path.GetExtension(file);
            if (dc.ContainsKey(exts))
                return (Bitmap)imageList.Images[dc[exts]];
            return (Bitmap)imageList.Images[0];
        }

        static public void LoadDictionary()
        {
            if (dc == null)
                dc = new Dictionary<string, string>();
            else
                return;

            Dictionary<string, string> d = dc;
            d.Add(".xsd", "DataSourceView_16x");
            d.Add(".xsc", "filesource");
            d.Add(".xss", "filesource");
            d.Add(".sln", "VSSolutionFile");
            d.Add(".vcproj", "CPPProject_SolutionExplorerNode");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            d.Add(".h", "Include_13490_24");
            d.Add(".txt", "resource_32xMD");
            d.Add(".md", "resource_32xMD");
            d.Add(".rtf", "resource_32xMD");
            d.Add(".targets", "resource_32xMD");
            d.Add(".dll", "Library_6213");
            d.Add(".xml", "XMLFile");
            d.Add(".xaml", "XMLFile");
            d.Add(".json", "resourcefile");
            d.Add(".cur", "cursor");
            d.Add(".shproj", "shared");
            d.Add(".cd", "classgray");
            d.Add(".resx", "filesource");
            d.Add(".bmp", "images");
            d.Add(".png", "images");
            d.Add(".jpg", "images");
            d.Add(".gif", "images");
            d.Add(".ico", "icon");
            d.Add(".wav", "audio");
            d.Add(".csx", "script");
            d.Add(".sh", "script");
            d.Add(".bat", "script");
            d.Add(".config", "config");
            d.Add(".vsixmanifest", "config");
            d.Add(".ruleset", "rule");
            d.Add(".pfx", "certificate");
            d.Add(".datasource", "datasource");
            d.Add(".cs", "CSharpFile_SolutionExplorerNode");
            d.Add(".vb", "vb");
            d.Add(".sp", "startup");

        }

        


    

        static public ImageList addImage(Bitmap bmp, string name, ImageList imgL)
        {
            if (imgL == null)
                imgL = new ImageList();

            if (bmp != null)
            {
                imgL.Images.Add(name, (Image)bmp);
               
            }

            return imgL;
        }

 

        static public ImageList CreateImageList()
        {
         

            imageList = new ImageList();

         
            {

                addImage(ve.Folder_16x, "Folder", imageList);
                addImage(ve.FolderOpen_16x, "FolderOpen", imageList);
                addImage(ve.DataSourceView_16x, "DataSourceView_16x", imageList);

                addImage(ve.CS_16x, "CSharpFile_SolutionExplorerNode", imageList);
                addImage(ve.CS_ProjectSENode_16x, "CSharpProject_SolutionExplorerNode", imageList);
                addImage(ve.FileSource_16x, "filesource", imageList);
                addImage(ve.VisualStudioSolution_16x, "VSSolutionFile", imageList);
                addImage(ve.Property_16x, "property_16x", imageList);
                addImage(ve.Reference_16x, "reference_16x", imageList);
                addImage(ve.Image_16x, "images", imageList);
                addImage(ve.Audio_16x, "audio", imageList);
                addImage(ve.IconFile_16x, "icon", imageList);
                addImage(ve.Cursor_16x, "cursor", imageList);
                addImage(ve.XMLFile_16x, "XMLFile", imageList);
                addImage(ve.ConfigurationFile_16x, "config", imageList);
                addImage(ve.Certificate_16x, "certificate", imageList);
                addImage(ve.Class_yellow_16x, "class", imageList);
                addImage(ve.Delegate_16x, "delegate", imageList);
                addImage(ve.Method_purple_16x, "method", imageList);
                addImage(ve.EnumFriend_16x, "enums", imageList);
                addImage(ve.Property_16x, "property", imageList);
                addImage(ve.Field_blue_16x, "field", imageList);
                addImage(ve.ShowStartPage_256x, "startup", imageList);
            }

            return imageList;
        }
    }


}