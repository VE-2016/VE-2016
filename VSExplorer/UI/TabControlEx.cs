using System.Runtime.InteropServices;

using System.Windows.Forms;

namespace WinExplorer.UI
{
    public class TabControlEx : TabControl
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1300 + 40)
            {
                RECT rc = (RECT)m.GetLParam(typeof(RECT));
                rc.Left -= 0;
                rc.Right += 3;
                rc.Top -= 0;
                rc.Bottom += 3;
                Marshal.StructureToPtr(rc, m.LParam, true);
            }
            //if (m.Msg == 0x1328 && !this.DesignMode)
            //    m.Result = new IntPtr(1);
            //else
            //    base.WndProc(ref m);
            base.WndProc(ref m);
        }
    }

    internal struct RECT { public int Left, Top, Right, Bottom; }
}