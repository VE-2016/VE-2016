
using System;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.Forms.Docking
{
    internal class HookEventArgs : EventArgs
    {
        public int HookCode;    // Hook code
        public IntPtr wParam;   // WPARAM argument
        public IntPtr lParam;   // LPARAM argument
    }


    // Hook Types  
    internal enum HookType : int
    {
        WH_JOURNALRECORD = 0,
        WH_JOURNALPLAYBACK = 1,
        WH_KEYBOARD = 2,
        WH_GETMESSAGE = 3,
        WH_CALLWNDPROC = 4,
        WH_CBT = 5,
        WH_SYSMSGFILTER = 6,
        WH_MOUSE = 7,
        WH_HARDWARE = 8,
        WH_DEBUG = 9,
        WH_SHELL = 10,
        WH_FOREGROUNDIDLE = 11,
        WH_CALLWNDPROCRET = 12,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    internal class LocalWindowsHook
    {
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        // Internal properties
        private IntPtr _hhook = IntPtr.Zero;
        private HookProc _filterFunc = null;
        private HookType _hookType;

        // Event delegate
        public delegate void HookEventHandler(object sender, HookEventArgs e);

        // Event: HookInvoked 
        public event HookEventHandler HookInvoked;
        protected void OnHookInvoked(HookEventArgs e)
        {
            if (HookInvoked != null)
                HookInvoked(this, e);
        }

        // Class constructor(s)
        public LocalWindowsHook(HookType hook)
        {
            _hookType = hook;
            _filterFunc = new HookProc(this.CoreHookProc);
        }

        public LocalWindowsHook(HookType hook, HookProc func)
        {
            _hookType = hook;
            _filterFunc = func;
        }

        // Default filter function
        public int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(_hhook, code, wParam, lParam);

            // Let clients determine what to do
            HookEventArgs e = new HookEventArgs();
            e.HookCode = code;
            e.wParam = wParam;
            e.lParam = lParam;
            OnHookInvoked(e);

            // Yield to the next hook in the chain
            return CallNextHookEx(_hhook, code, wParam, lParam);
        }

        // Install the hook
        public void Install()
        {
            int threadId = Kernel32.GetCurrentThreadId();
            _hhook = SetWindowsHookEx(_hookType, _filterFunc, IntPtr.Zero, threadId);
        }

        // Uninstall the hook
        public void Uninstall()
        {
            UnhookWindowsHookEx(_hhook);
        }
    }
}
