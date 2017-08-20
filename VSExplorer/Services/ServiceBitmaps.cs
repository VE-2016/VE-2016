using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace WinExplorer.Services
{
    public class ServiceBitmaps
    {
        private ImageList g { get; set; }

        public ServiceBitmaps()
        {
            g = ServiceBitmaps.BitmapResourceToImageList();
            ArrayList G = ServiceBitmaps.GetClassBitmapNames();
            ServiceBitmaps.GetStaticOverlay(g, G);
        }

        public Image GetImage(string key)
        {
            return g.Images[key];
        }

        public string GetKeyForField(FieldInfo f)
        {
            string key = "";

            if (f.IsPublic)
            {
                if (f.IsStatic)
                    return "s_Field_blue_16x";

                return "Field_blue_16x";
            }
            else
            {
                if (f.IsStatic)
                    return "s_Field_blue_16x";

                if (f.IsPrivate)
                    return "FieldPrivate_16x";
                else return "FieldProtect_16x";
            }
        }

        public string GetKeyForProperty(PropertyInfo f, string modifier = "")
        {
            string key = "";

            if (modifier == "static")
            {
                return "s_Property_16x";
            }

            key = "Property_16x";

            return key;
        }

        public Dictionary<string, Bitmap> GetBitmapDictionary()
        {
            Dictionary<string, Bitmap> dict = new Dictionary<string, Bitmap>();

            return dict;
        }

        public static ArrayList GetClassBitmapNames()
        {
            string[] s = { "Field_blue_16x", "FieldPrivate_16x", "FieldProtect_16x", "FieldSealed_16x", "FieldFriend_16x", "Property_16x" };

            ArrayList G = new ArrayList();
            foreach (string c in s)
                G.Add(c);

            return G;
        }

        public static ImageList BitmapResourceToImageList()
        {
            var resourceSet = ve_resource.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, false, false);

            ImageList g = new ImageList();

            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                object resource = entry.Value;
                if (resource.GetType() == typeof(Bitmap))
                    g.Images.Add(resourceKey, resource as Image);
            }
            return g;
        }

        public static Image GetOverlay(Image imageBackground, Image imageOverlay)
        {
            Image img = new Bitmap(imageBackground.Width, imageBackground.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(imageBackground, new Point(0, 0));
                gr.DrawImage(imageOverlay, new Point(0, 0));
            }
            return img;
        }

        public static Image GetStaticImageOverlay(ImageList g)
        {
            return g.Images["OverlayStatic_16x"];
        }

        public static Image GetProtectedImageOverlay(ImageList g)
        {
            return g.Images["OverlayStatic_16x"];
        }

        public static ImageList GetStaticOverlay(ImageList g, ArrayList G)
        {
            Image imageOverlay = GetStaticImageOverlay(g);

            foreach (string key in G)
            {
                Image imageBackground = g.Images[key];
                g.Images.Add("s_" + key, GetOverlay(imageBackground, imageOverlay));
            }
            return g;
        }
    }
}