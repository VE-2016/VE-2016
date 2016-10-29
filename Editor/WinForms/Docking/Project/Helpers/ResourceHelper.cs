using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class ResourceHelper
    {
        private static ResourceManager s_resourceManager;

        static ResourceHelper()
        {
            s_resourceManager = new ResourceManager("AIMS.Libraries.Forms.Docking.Strings", typeof(DockContainer).Assembly);
        }

        public static Bitmap LoadBitmap(string name)
        {
            Assembly assembly = typeof(ResourceHelper).Assembly;
            string fullNamePrefix = "AIMS.Libraries.Forms.Docking.Resources.";
            return new Bitmap(assembly.GetManifestResourceStream(fullNamePrefix + name));
        }

        public static string GetString(string name)
        {
            return s_resourceManager.GetString(name);
        }
    }
}
