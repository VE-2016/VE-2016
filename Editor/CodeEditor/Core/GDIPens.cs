using System;
using System.Drawing;
using AIMS.Libraries.CodeEditor.Win32;

namespace AIMS.Libraries.CodeEditor.Core
{
    public class GDIPen : GDIObject
    {
        public IntPtr hPen;

        public GDIPen(Color color, int width)
        {
            hPen = NativeGdi32Api.CreatePen(0, width, NativeUser32Api.ColorToInt(color));
            Create();
        }

        protected override void Destroy()
        {
            if (hPen != (IntPtr)0)
                NativeGdi32Api.DeleteObject(hPen);
            base.Destroy();
            hPen = (IntPtr)0;
        }

        protected override void Create()
        {
            base.Create();
        }
    }
}