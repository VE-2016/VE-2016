using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;

namespace AIMS.Libraries.Scripting.ScriptControl.Project
{
    public class TypeLibrary
    {
        private string _name;
        private string _description;
        private string _path;
        private string _guid;
        private string _version;
        private string _lcid;
        private bool _isolated = false;

        public string Guid
        {
            get
            {
                return _guid;
            }
        }

        public bool Isolated
        {
            get
            {
                return _isolated;
            }
        }

        public string Lcid
        {
            get
            {
                return _lcid;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = GetTypeLibName();
                }
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
        }

        public int VersionMajor
        {
            get
            {
                if (_version == null)
                {
                    return -1;
                }
                string[] ver = _version.Split('.');

                return ver.Length == 0 ? -1 : GetVersion(ver[0]);
            }
        }

        public int VersionMinor
        {
            get
            {
                if (_version == null)
                {
                    return -1;
                }
                string[] ver = _version.Split('.');

                return ver.Length < 2 ? -1 : GetVersion(ver[1]);
            }
        }

        public string WrapperTool
        {
            get
            {
                // TODO: which wrapper tool ?
                return "tlbimp";
            }
        }

        public static IEnumerable<TypeLibrary> Libraries
        {
            get
            {
                RegistryKey typeLibsKey = Registry.ClassesRoot.OpenSubKey("TypeLib");
                foreach (string typeLibKeyName in typeLibsKey.GetSubKeyNames())
                {
                    RegistryKey typeLibKey = null;
                    try
                    {
                        typeLibKey = typeLibsKey.OpenSubKey(typeLibKeyName);
                    }
                    catch (System.Security.SecurityException)
                    {
                        // ignore type libraries that cannot be read from the registry
                    }
                    if (typeLibKey == null)
                    {
                        continue;
                    }
                    TypeLibrary lib = Create(typeLibKey);
                    if (lib != null && lib.Description != null && lib.Path != null && lib.Description.Length > 0 && lib.Path.Length > 0)
                    {
                        yield return lib;
                    }
                }
            }
        }

        public static TypeLibrary Create(RegistryKey typeLibKey)
        {
            string[] versions = typeLibKey.GetSubKeyNames();
            if (versions.Length > 0)
            {
                TypeLibrary lib = new TypeLibrary();

                // Use the last version
                lib._version = versions[versions.Length - 1];

                RegistryKey versionKey = typeLibKey.OpenSubKey(lib._version);
                lib._description = (string)versionKey.GetValue(null);
                lib._path = GetTypeLibPath(versionKey, ref lib._lcid);
                lib._guid = System.IO.Path.GetFileName(typeLibKey.Name);

                return lib;
            }
            return null;
        }

        private static string GetTypeLibPath(RegistryKey versionKey, ref string lcid)
        {
            // Get the default value of the (typically) 0\win32 subkey:
            string[] subkeys = versionKey.GetSubKeyNames();

            if (subkeys == null || subkeys.Length == 0)
            {
                return null;
            }
            for (int i = 0; i < subkeys.Length; i++)
            {
                int result;
                if (Int32.TryParse(subkeys[i], out result))
                {
                    lcid = subkeys[i];
                    RegistryKey NullKey = versionKey.OpenSubKey(subkeys[i]);
                    string[] subsubkeys = NullKey.GetSubKeyNames();
                    RegistryKey win32Key = NullKey.OpenSubKey("win32");

                    return win32Key == null || win32Key.GetValue(null) == null ? null : GetTypeLibPath(win32Key.GetValue(null).ToString());
                }
            }
            return null;
        }

        private static int GetVersion(string s)
        {
            int version;
            if (Int32.TryParse(s, out version))
            {
                return version;
            }
            return -1;
        }

        private string GetTypeLibName()
        {
            string name = null;

            int typeLibLcid;
            if (_guid != null && _lcid != null && Int32.TryParse(_lcid, out typeLibLcid))
            {
                Guid typeLibGuid = new Guid(_guid);
                name = GetTypeLibNameFromGuid(ref typeLibGuid, (short)VersionMajor, (short)VersionMinor, typeLibLcid);
            }

            if (name == null)
            {
                name = GetTypeLibNameFromFile(_path);
            }

            if (name != null)
            {
                return name;
            }
            return _description;
        }

        /// <summary>
        /// Removes the trailing part of the type library filename if it
        /// starts with a number.
        /// </summary>
        private static string GetTypeLibPath(string fileName)
        {
            if (fileName != null)
            {
                int index = fileName.LastIndexOf('\\');
                if (index > 0 && index + 1 < fileName.Length)
                {
                    if (Char.IsDigit(fileName[index + 1]))
                    {
                        return fileName.Substring(0, index);
                    }
                }
            }
            return fileName;
        }

        private static string GetTypeLibNameFromFile(string fileName)
        {
            if (fileName != null && fileName.Length > 0 && File.Exists(fileName))
            {
                ITypeLib typeLib;
                if (LoadTypeLibEx(fileName, RegKind.None, out typeLib) == 0)
                {
                    try
                    {
                        return Marshal.GetTypeLibName(typeLib);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(typeLib);
                    }
                }
            }
            return null;
        }

        private static string GetTypeLibNameFromGuid(ref Guid guid, short versionMajor, short versionMinor, int lcid)
        {
            ITypeLib typeLib;
            if (LoadRegTypeLib(ref guid, versionMajor, versionMinor, lcid, out typeLib) == 0)
            {
                try
                {
                    return Marshal.GetTypeLibName(typeLib);
                }
                finally
                {
                    Marshal.ReleaseComObject(typeLib);
                }
            }
            return null;
        }

        private enum RegKind
        {
            Default,
            Register,
            None
        }

        [DllImport("oleaut32.dll")]
        private static extern int LoadTypeLibEx([MarshalAs(UnmanagedType.BStr)] string szFile,
                                        RegKind regkind,
                                        out ITypeLib pptlib);

        [DllImport("oleaut32.dll")]
        private static extern int LoadRegTypeLib(
            ref Guid rguid,
            short wVerMajor,
            short wVerMinor,
            int lcid,
            out ITypeLib pptlib);
    }
}
