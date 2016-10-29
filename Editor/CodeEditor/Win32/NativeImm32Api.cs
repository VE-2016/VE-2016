using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.CodeEditor.Win32
{
    public static class NativeImm32Api
    {
        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);
    }
}
