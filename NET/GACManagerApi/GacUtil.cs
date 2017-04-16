using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GACManagerApi
{
    /// <summary>
    /// The GacUtil class wraps functionality provided by the gacutil.exe program.
    /// </summary>
    public class GacUtil
    {
        /// <summary>
        /// Determines whether the gacutil.exe file can be found.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can find executable]; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanFindExecutable()
        {
            return GetGacUtilPath() != null;
        }

        /// <summary>
        /// Gets the gac util path.
        /// </summary>
        public static string GetGacUtilPath()
        {
            //  Define all search paths here.
            var searchPaths = new[]
            {
                @"%PROGRAMFILES%\Microsoft SDKs\Windows\v6.0A\bin\gacutil.exe",
                @"%PROGRAMFILES%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe",
                @"%PROGRAMFILES%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe",
                @"%PROGRAMFILES%\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\gacutil.exe",
                @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v6.0A\bin\gacutil.exe",
                @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe",
                @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v7.0A\Bin\gacutil.exe",
                @"%PROGRAMFILES(X86)%\Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\gacutil.exe",
            };

            //  Expand the environment variables.
            var paths = searchPaths.Select(Environment.ExpandEnvironmentVariables);

            //  Go through every search path until we find the gacutil file.
            return paths.FirstOrDefault(System.IO.File.Exists);
        }

        public static void ListAssemblies(string search)
        {
        }

        public static ArrayList LoadFramework(DirectoryInfo d)
        {
            ArrayList L = new ArrayList();

            string folder = d.FullName;

            FileInfo[] g = d.GetFiles();

            foreach (FileInfo c in g)
            {
                if (c.Extension != ".dll")
                    continue;


                AssemblyDescription desc = new AssemblyDescription(c.FullName, "");

                L.Add(desc);
            }

            return L;
        }

        public static ArrayList GetInstalledSDKs()
        {
            ArrayList L = new ArrayList();

            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

            string s = "Reference Assemblies\\Microsoft\\Framework\\.NETFramework";

            DirectoryInfo d = new DirectoryInfo(programFilesX86 + "\\" + s);

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo c in dd)
                L.Add(c);


            return L;
        }

        public static ArrayList GetFrameworks()
        {
            ArrayList L = new ArrayList();

            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

            string s = "Reference Assemblies\\Microsoft\\Framework\\.NETFramework";

            DirectoryInfo d = new DirectoryInfo(programFilesX86 + "\\" + s);

            DirectoryInfo[] dd = d.GetDirectories();

            foreach (DirectoryInfo c in dd)
                L.Add(c);


            return L;
        }

        public static ArrayList F { get; set; }

        public static async Task<ArrayList> GetFramework(string s)
        {
            F = GetFrameworks();

            DirectoryInfo d = F[1] as DirectoryInfo;

            ArrayList L = LoadFramework(d);

            return L;
        }

        public static ArrayList GetExtensionFiles(DirectoryInfo d, bool subfolders = true)
        {
            ArrayList L = new ArrayList();

            DirectoryInfo[] dd = new DirectoryInfo[] { d };
            if(subfolders)
             d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                DirectoryInfo[] c = s.GetDirectories();

                if (c.Length <= 0)
                    continue;

                DirectoryInfo cc = c[0];

                FileInfo[] f = cc.GetFiles();

                foreach (FileInfo g in f)
                {
                    if (g.FullName.EndsWith(".dll") == false)
                        continue;

                    AssemblyDescription desc = new AssemblyDescription(g.FullName, "");

                    L.Add(desc);
                }
            }

            return L;
        }
        public static ArrayList GetWPFFiles(DirectoryInfo d, bool subfolders = true)
        {
            ArrayList L = new ArrayList();

            DirectoryInfo[] dd = new DirectoryInfo[] { d };
            if (subfolders)
                d.GetDirectories();

            foreach (DirectoryInfo s in dd)
            {
                //DirectoryInfo[] c = s.GetDirectories();

                //if (c.Length <= 0)
                //    continue;

                DirectoryInfo cc = s;

                FileInfo[] f = cc.GetFiles();

                foreach (FileInfo g in f)
                {
                    if (g.FullName.EndsWith(".dll") == false)
                        continue;

                    AssemblyDescription desc = new AssemblyDescription(g.FullName, "");

                    L.Add(desc);
                }
            }

            return L;
        }
        public static async Task<ArrayList> GetExtensions(string s)
        {
            // F = GetFrameworks();

            string system = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName;

            DirectoryInfo d = new DirectoryInfo(system + "\\" + "assembly\\GAC");

            ArrayList EG = GetExtensionFiles(d);

            DirectoryInfo e = new DirectoryInfo(system + "\\" + "assembly\\GAC_MSIL");

            ArrayList EM = GetExtensionFiles(e);

            ArrayList L = new ArrayList();

            L.AddRange(EG);

            L.AddRange(EM);

            return L;
        }
        public static ArrayList GetWPF()
        {
            string system = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).FullName;

            DirectoryInfo d = new DirectoryInfo(system + "\\" + "Microsoft.NET\\Framework64");

            DirectoryInfo[] folders = d.GetDirectories();

            DirectoryInfo wpf = folders[folders.Length - 1];

            string s = wpf.FullName + "\\WPF";

            d = new DirectoryInfo(s);

            ArrayList WPF = GetWPFFiles(d, false);

            return WPF;
        }

        //public string GetDocumentation(string assemblyName, string data)
        //{

        //    string file = GACForm.dicts[dtd.ParentAssembly.AssemblyName] as string;

        //    string docuPath = file.Substring(0, file.LastIndexOf(".")) + ".XML";

        //    string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");

        //    string s = "Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.5.2";

        //    string filename = programFilesX86 + "\\" + s + "\\" + Path.GetFileName(docuPath);

        //    if (File.Exists(filename))
        //    {
        //        XmlDocument dd = new XmlDocument();
        //        dd.Load(filename);
        //        XmlElement matchedElement = null;

        //        foreach (XmlElement xmlElement in dd["doc"]["members"])
        //        {
        //            // if(xmlElement.Attributes.Count > 0)
        //            if (xmlElement.Attributes["name"].Value.Equals("T:" + dtd.FullName))
        //            {

        //                matchedElement = xmlElement;

        //                doc = matchedElement.InnerText;
        //            }
        //        }



        //    }

        //    return "";
        //}
    }
}
