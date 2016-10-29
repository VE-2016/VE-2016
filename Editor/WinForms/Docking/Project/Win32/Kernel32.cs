using System;
using System.Drawing;
using System.Runtime.InteropServices;
using AIMS.Libraries.Forms.Docking.Win32;

namespace AIMS.Libraries.Forms.Docking
{
    internal class Kernel32
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetCurrentThreadId();
    }
}