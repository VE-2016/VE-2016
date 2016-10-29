using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VSParsers
{
    public class frameworks
    {
        static public ArrayList GetFrameworks()
        {
            ArrayList L = new ArrayList();

            string path = @"SOFTWARE\Microsoft\NET Framework Setup\NDP";
            List<string> display_framwork_name = new List<string>();

            RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(path);
            string[] version_names = installed_versions.GetSubKeyNames();

            for (int i = 1; i <= version_names.Length - 1; i++)
            {
                string temp_name = "Microsoft .NET Framework " + version_names[i].ToString();// +"  SP" + installed_versions.OpenSubKey(version_names[i]).GetValue("SP");
                //lbInstVersions.Items.Add(friendlyName + (version != null ? (" (" + version + ")") : ""));

                //if(version!=null && version.Split(',').Length>=4

                L.Add(temp_name);
            }

            Get45or451FromRegistry(L);

            return L;
        }

        private static void Get45or451FromRegistry(ArrayList L)
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    L.Add(CheckFor45DotVersion(releaseKey));
                }
            }
        }

        // Checking the version using >= will enable forward compatibility,
        // however you should always compile your code on newer versions of
        // the framework to ensure your app works the same.
        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393273)
            {
                return "4.6 RC or later";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2 or later";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1 or later";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
}