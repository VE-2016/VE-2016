

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AIMS.Libraries.Forms.Docking
{
    /// <summary>
    /// DragHandlerBase is the base class for drag handlers. The derived class should:
    ///   1. Define its public method BeginDrag. From within this public BeginDrag method,
    ///      DragHandlerBase.BeginDrag should be called to initialize the mouse capture
    ///      and message filtering.
    ///   2. Override the OnDragging and OnEndDrag methods.
    /// </summary>
    internal class DragHandlerBase : IMessageFilter
    {
        private Control _dragControl = null;

        private IntPtr _hWnd;
        private WndProcCallBack _wndProcCallBack;
        private IntPtr _prevWndFunc;
        private delegate IntPtr WndProcCallBack(IntPtr hwnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("User32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WndProcCallBack wndProcCallBack);
        [DllImport("User32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr wndFunc);
        [DllImport("User32.dll")]
        private static extern IntPtr CallWindowProc(IntPtr prevWndFunc, IntPtr hWnd, int iMsg, IntPtr wParam, IntPtr lParam);

        public DragHandlerBase()
        {
            _wndProcCallBack = new WndProcCallBack(this.WndProc);
        }

        private void AssignHandle(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _prevWndFunc = SetWindowLong(hWnd, -4, _wndProcCallBack); // GWL_WNDPROC = -4
        }

        private void ReleaseHandle()
        {
            SetWindowLong(_hWnd, -4, _prevWndFunc);   // GWL_WNDPROC = -4
            _hWnd = IntPtr.Zero;
            _prevWndFunc = IntPtr.Zero;
        }

        internal Control DragControl
        {
            get { return _dragControl; }
        }

        private Point _startMousePosition = Point.Empty;
        protected Point StartMousePosition
        {
            get { return _startMousePosition; }
        }

        protected bool BeginDrag(Control c)
        {
            // Avoid re-entrance;
            lock (this)
            {
                if (_dragControl != null)
                    return false;

                _startMousePosition = Control.MousePosition;

                if (!User32.DragDetect(c.Handle, StartMousePosition))
                    return false;

                _dragControl = c;
                c.FindForm().Capture = true;
                AssignHandle(c.FindForm().Handle);
                Application.AddMessageFilter(this);
                return true;
            }
        }

        protected virtual void OnDragging()
        {
        }

        protected virtual void OnEndDrag(bool abort)
        {
        }

        private void EndDrag(bool abort)
        {
            ReleaseHandle();
            Application.RemoveMessageFilter(this);
            _dragControl.FindForm().Capture = false;

            OnEndDrag(abort);

            _dragControl = null;
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_MOUSEMOVE)
                OnDragging();
            else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONUP)
                EndDrag(false);
            else if (m.Msg == (int)Win32.Msgs.WM_CAPTURECHANGED)
                EndDrag(true);
            else if (m.Msg == (int)Win32.Msgs.WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape)
                EndDrag(true);

            return OnPreFilterMessage(ref m);
        }

        protected virtual bool OnPreFilterMessage(ref Message m)
        {
            return false;
        }

        private IntPtr WndProc(IntPtr hWnd, int iMsg, IntPtr wParam, IntPtr lParam)
        {
            if (iMsg == (int)Win32.Msgs.WM_CANCELMODE || iMsg == (int)Win32.Msgs.WM_CAPTURECHANGED)
                EndDrag(true);

            return CallWindowProc(_prevWndFunc, hWnd, iMsg, wParam, lParam);
        }
    }
}