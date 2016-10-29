using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal interface IContentFocusManager
    {
        void Activate(IDockContent content);
        void GiveUpFocus(IDockContent content);
        void AddToList(IDockContent content);
        void RemoveFromList(IDockContent content);
    }

    public partial class DockPanel
    {
        private interface IFocusManager
        {
            void SuspendFocusTracking();
            void ResumeFocusTracking();
            bool IsFocusTrackingSuspended { get; }
            IDockContent ActiveContent { get; }
            DockPane ActivePane { get; }
            IDockContent ActiveDocument { get; }
            DockPane ActiveDocumentPane { get; }
        }

        private class FocusManagerImpl : Component, IContentFocusManager, IFocusManager
        {
            private class HookEventArgs : EventArgs
            {
                [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
                public int HookCode;
                [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
                public IntPtr wParam;
                public IntPtr lParam;
            }

            private class LocalWindowsHook : IDisposable
            {
                // Internal properties
                private IntPtr _hHook = IntPtr.Zero;
                private NativeMethods.HookProc _filterFunc = null;
                private Win32.HookType _hookType;

                // Event delegate
                public delegate void HookEventHandler(object sender, HookEventArgs e);

                // Event: HookInvoked 
                public event HookEventHandler HookInvoked;
                protected void OnHookInvoked(HookEventArgs e)
                {
                    if (HookInvoked != null)
                        HookInvoked(this, e);
                }

                public LocalWindowsHook(Win32.HookType hook)
                {
                    _hookType = hook;
                    _filterFunc = new NativeMethods.HookProc(this.CoreHookProc);
                }

                // Default filter function
                public IntPtr CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
                {
                    if (code < 0)
                        return NativeMethods.CallNextHookEx(_hHook, code, wParam, lParam);

                    // Let clients determine what to do
                    HookEventArgs e = new HookEventArgs();
                    e.HookCode = code;
                    e.wParam = wParam;
                    e.lParam = lParam;
                    OnHookInvoked(e);

                    // Yield to the next hook in the chain
                    return NativeMethods.CallNextHookEx(_hHook, code, wParam, lParam);
                }

                // Install the hook
                public void Install()
                {
                    if (_hHook != IntPtr.Zero)
                        Uninstall();

                    int threadId = NativeMethods.GetCurrentThreadId();
                    _hHook = NativeMethods.SetWindowsHookEx(_hookType, _filterFunc, IntPtr.Zero, threadId);
                }

                // Uninstall the hook
                public void Uninstall()
                {
                    if (_hHook != IntPtr.Zero)
                    {
                        NativeMethods.UnhookWindowsHookEx(_hHook);
                        _hHook = IntPtr.Zero;
                    }
                }

                ~LocalWindowsHook()
                {
                    Dispose(false);
                }

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                protected virtual void Dispose(bool disposing)
                {
                    Uninstall();
                }
            }

            // Use a static instance of the windows hook to prevent stack overflows in the windows kernel.
            [ThreadStatic]
            private static LocalWindowsHook t_sm_localWindowsHook;

            private readonly LocalWindowsHook.HookEventHandler _hookEventHandler;

            public FocusManagerImpl(DockPanel dockPanel)
            {
                _dockPanel = dockPanel;
                if (Win32Helper.IsRunningOnMono)
                    return;
                _hookEventHandler = new LocalWindowsHook.HookEventHandler(HookEventHandler);

                // Ensure the windows hook has been created for this thread
                if (t_sm_localWindowsHook == null)
                {
                    t_sm_localWindowsHook = new LocalWindowsHook(Win32.HookType.WH_CALLWNDPROCRET);
                    t_sm_localWindowsHook.Install();
                }

                t_sm_localWindowsHook.HookInvoked += _hookEventHandler;
            }

            private DockPanel _dockPanel;
            public DockPanel DockPanel
            {
                get { return _dockPanel; }
            }

            private bool _disposed = false;
            protected override void Dispose(bool disposing)
            {
                if (!_disposed && disposing)
                {
                    if (!Win32Helper.IsRunningOnMono)
                    {
                        t_sm_localWindowsHook.HookInvoked -= _hookEventHandler;
                    }

                    _disposed = true;
                }

                base.Dispose(disposing);
            }

            private IDockContent _contentActivating = null;
            private IDockContent ContentActivating
            {
                get { return _contentActivating; }
                set { _contentActivating = value; }
            }

            public void Activate(IDockContent content)
            {
                if (IsFocusTrackingSuspended)
                {
                    ContentActivating = content;
                    return;
                }

                if (content == null)
                    return;
                DockContentHandler handler = content.DockHandler;
                if (handler.Form.IsDisposed)
                    return; // Should not reach here, but better than throwing an exception
                if (ContentContains(content, handler.ActiveWindowHandle))
                {
                    if (!Win32Helper.IsRunningOnMono)
                    {
                        NativeMethods.SetFocus(handler.ActiveWindowHandle);
                    }
                }

                if (handler.Form.ContainsFocus)
                    return;

                if (handler.Form.SelectNextControl(handler.Form.ActiveControl, true, true, true, true))
                    return;

                if (Win32Helper.IsRunningOnMono)
                    return;

                // Since DockContent Form is not selectalbe, use Win32 SetFocus instead
                NativeMethods.SetFocus(handler.Form.Handle);
            }

            private List<IDockContent> _listContent = new List<IDockContent>();
            private List<IDockContent> ListContent
            {
                get { return _listContent; }
            }
            public void AddToList(IDockContent content)
            {
                if (ListContent.Contains(content) || IsInActiveList(content))
                    return;

                ListContent.Add(content);
            }

            public void RemoveFromList(IDockContent content)
            {
                if (IsInActiveList(content))
                    RemoveFromActiveList(content);
                if (ListContent.Contains(content))
                    ListContent.Remove(content);
            }

            private IDockContent _lastActiveContent = null;
            private IDockContent LastActiveContent
            {
                get { return _lastActiveContent; }
                set { _lastActiveContent = value; }
            }

            private bool IsInActiveList(IDockContent content)
            {
                return !(content.DockHandler.NextActive == null && LastActiveContent != content);
            }

            private void AddLastToActiveList(IDockContent content)
            {
                IDockContent last = LastActiveContent;
                if (last == content)
                    return;

                DockContentHandler handler = content.DockHandler;

                if (IsInActiveList(content))
                    RemoveFromActiveList(content);

                handler.PreviousActive = last;
                handler.NextActive = null;
                LastActiveContent = content;
                if (last != null)
                    last.DockHandler.NextActive = LastActiveContent;
            }

            private void RemoveFromActiveList(IDockContent content)
            {
                if (LastActiveContent == content)
                    LastActiveContent = content.DockHandler.PreviousActive;

                IDockContent prev = content.DockHandler.PreviousActive;
                IDockContent next = content.DockHandler.NextActive;
                if (prev != null)
                    prev.DockHandler.NextActive = next;
                if (next != null)
                    next.DockHandler.PreviousActive = prev;

                content.DockHandler.PreviousActive = null;
                content.DockHandler.NextActive = null;
            }

            public void GiveUpFocus(IDockContent content)
            {
                DockContentHandler handler = content.DockHandler;
                if (!handler.Form.ContainsFocus)
                    return;

                if (IsFocusTrackingSuspended)
                    DockPanel.DummyControl.Focus();

                if (LastActiveContent == content)
                {
                    IDockContent prev = handler.PreviousActive;
                    if (prev != null)
                        Activate(prev);
                    else if (ListContent.Count > 0)
                        Activate(ListContent[ListContent.Count - 1]);
                }
                else if (LastActiveContent != null)
                    Activate(LastActiveContent);
                else if (ListContent.Count > 0)
                    Activate(ListContent[ListContent.Count - 1]);
            }

            private static bool ContentContains(IDockContent content, IntPtr hWnd)
            {
                Control control = Control.FromChildHandle(hWnd);
                for (Control parent = control; parent != null; parent = parent.Parent)
                    if (parent == content.DockHandler.Form)
                        return true;

                return false;
            }

            private uint _countSuspendFocusTracking = 0;
            public void SuspendFocusTracking()
            {
                if (_disposed)
                    return;

                if (_countSuspendFocusTracking++ == 0)
                {
                    if (!Win32Helper.IsRunningOnMono)
                        if (t_sm_localWindowsHook != null)
                            t_sm_localWindowsHook.HookInvoked -= _hookEventHandler;
                }
            }

            public void ResumeFocusTracking()
            {
                if (_disposed || _countSuspendFocusTracking == 0)
                    return;

                if (--_countSuspendFocusTracking == 0)
                {
                    if (ContentActivating != null)
                    {
                        Activate(ContentActivating);
                        ContentActivating = null;
                    }

                    if (!Win32Helper.IsRunningOnMono)
                        if (t_sm_localWindowsHook != null)
                            t_sm_localWindowsHook.HookInvoked += _hookEventHandler;

                    if (!InRefreshActiveWindow)
                        RefreshActiveWindow();
                }
            }

            public bool IsFocusTrackingSuspended
            {
                get { return _countSuspendFocusTracking != 0; }
            }

            // Windows hook event handler
            private void HookEventHandler(object sender, HookEventArgs e)
            {
                Win32.Msgs msg = (Win32.Msgs)Marshal.ReadInt32(e.lParam, IntPtr.Size * 3);

                if (msg == Win32.Msgs.WM_KILLFOCUS)
                {
                    IntPtr wParam = Marshal.ReadIntPtr(e.lParam, IntPtr.Size * 2);
                    DockPane pane = GetPaneFromHandle(wParam);
                    if (pane == null)
                        RefreshActiveWindow();
                }
                else if (msg == Win32.Msgs.WM_SETFOCUS || msg == Win32.Msgs.WM_MDIACTIVATE)
                    RefreshActiveWindow();
            }

            private DockPane GetPaneFromHandle(IntPtr hWnd)
            {
                Control control = Control.FromChildHandle(hWnd);

                IDockContent content = null;
                DockPane pane = null;
                for (; control != null; control = control.Parent)
                {
                    content = control as IDockContent;
                    if (content != null)
                        content.DockHandler.ActiveWindowHandle = hWnd;

                    if (content != null && content.DockHandler.DockPanel == DockPanel)
                        return content.DockHandler.Pane;

                    pane = control as DockPane;
                    if (pane != null && pane.DockPanel == DockPanel)
                        break;
                }

                return pane;
            }

            private bool _inRefreshActiveWindow = false;
            private bool InRefreshActiveWindow
            {
                get { return _inRefreshActiveWindow; }
            }

            private void RefreshActiveWindow()
            {
                SuspendFocusTracking();
                _inRefreshActiveWindow = true;

                DockPane oldActivePane = ActivePane;
                IDockContent oldActiveContent = ActiveContent;
                IDockContent oldActiveDocument = ActiveDocument;

                SetActivePane();
                SetActiveContent();
                SetActiveDocumentPane();
                SetActiveDocument();
                DockPanel.AutoHideWindow.RefreshActivePane();

                ResumeFocusTracking();
                _inRefreshActiveWindow = false;

                if (oldActiveContent != ActiveContent)
                    DockPanel.OnActiveContentChanged(EventArgs.Empty);
                if (oldActiveDocument != ActiveDocument)
                    DockPanel.OnActiveDocumentChanged(EventArgs.Empty);
                if (oldActivePane != ActivePane)
                    DockPanel.OnActivePaneChanged(EventArgs.Empty);
            }

            private DockPane _activePane = null;
            public DockPane ActivePane
            {
                get { return _activePane; }
            }

            private void SetActivePane()
            {
                DockPane value = Win32Helper.IsRunningOnMono ? null : GetPaneFromHandle(NativeMethods.GetFocus());
                if (_activePane == value)
                    return;

                if (_activePane != null)
                    _activePane.SetIsActivated(false);

                _activePane = value;

                if (_activePane != null)
                    _activePane.SetIsActivated(true);
            }

            private IDockContent _activeContent = null;
            public IDockContent ActiveContent
            {
                get { return _activeContent; }
            }

            internal void SetActiveContent()
            {
                IDockContent value = ActivePane == null ? null : ActivePane.ActiveContent;

                if (_activeContent == value)
                    return;

                if (_activeContent != null)
                    _activeContent.DockHandler.IsActivated = false;

                _activeContent = value;

                if (_activeContent != null)
                {
                    _activeContent.DockHandler.IsActivated = true;
                    if (!DockHelper.IsDockStateAutoHide((_activeContent.DockHandler.DockState)))
                        AddLastToActiveList(_activeContent);
                }
            }

            private DockPane _activeDocumentPane = null;
            public DockPane ActiveDocumentPane
            {
                get { return _activeDocumentPane; }
            }

            private void SetActiveDocumentPane()
            {
                DockPane value = null;

                if (ActivePane != null && ActivePane.DockState == DockState.Document)
                    value = ActivePane;

                if (value == null && DockPanel.DockWindows != null)
                {
                    if (ActiveDocumentPane == null)
                        value = DockPanel.DockWindows[DockState.Document].DefaultPane;
                    else if (ActiveDocumentPane.DockPanel != DockPanel || ActiveDocumentPane.DockState != DockState.Document)
                        value = DockPanel.DockWindows[DockState.Document].DefaultPane;
                    else
                        value = ActiveDocumentPane;
                }

                if (_activeDocumentPane == value)
                    return;

                if (_activeDocumentPane != null)
                    _activeDocumentPane.SetIsActiveDocumentPane(false);

                _activeDocumentPane = value;

                if (_activeDocumentPane != null)
                    _activeDocumentPane.SetIsActiveDocumentPane(true);
            }

            private IDockContent _activeDocument = null;
            public IDockContent ActiveDocument
            {
                get { return _activeDocument; }
            }

            private void SetActiveDocument()
            {
                IDockContent value = ActiveDocumentPane == null ? null : ActiveDocumentPane.ActiveContent;

                if (_activeDocument == value)
                    return;

                _activeDocument = value;
            }
        }

        private IFocusManager FocusManager
        {
            get { return _focusManager; }
        }

        internal IContentFocusManager ContentFocusManager
        {
            get { return _focusManager; }
        }

        internal void SaveFocus()
        {
            DummyControl.Focus();
        }

        [Browsable(false)]
        public IDockContent ActiveContent
        {
            get { return FocusManager.ActiveContent; }
        }

        [Browsable(false)]
        public DockPane ActivePane
        {
            get { return FocusManager.ActivePane; }
        }

        [Browsable(false)]
        public IDockContent ActiveDocument
        {
            get { return FocusManager.ActiveDocument; }
        }

        [Browsable(false)]
        public DockPane ActiveDocumentPane
        {
            get { return FocusManager.ActiveDocumentPane; }
        }

        private static readonly object s_activeDocumentChangedEvent = new object();
        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("DockPanel_ActiveDocumentChanged_Description")]
        public event EventHandler ActiveDocumentChanged
        {
            add { Events.AddHandler(s_activeDocumentChangedEvent, value); }
            remove { Events.RemoveHandler(s_activeDocumentChangedEvent, value); }
        }
        protected virtual void OnActiveDocumentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activeDocumentChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_activeContentChangedEvent = new object();
        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("DockPanel_ActiveContentChanged_Description")]
        public event EventHandler ActiveContentChanged
        {
            add { Events.AddHandler(s_activeContentChangedEvent, value); }
            remove { Events.RemoveHandler(s_activeContentChangedEvent, value); }
        }

        protected void OnActiveContentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activeContentChangedEvent];
            if (handler != null)
                handler(this, e);
        }

        private static readonly object s_documentDraggedEvent = new object();
        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("DockPanel_ActiveContentChanged_Description")]
        public event EventHandler DocumentDragged
        {
            add { Events.AddHandler(s_documentDraggedEvent, value); }
            remove { Events.RemoveHandler(s_documentDraggedEvent, value); }
        }

        internal void OnDocumentDragged()
        {
            EventHandler handler = (EventHandler)Events[s_documentDraggedEvent];
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private static readonly object s_activePaneChangedEvent = new object();
        [LocalizedCategory("Category_PropertyChanged")]
        [LocalizedDescription("DockPanel_ActivePaneChanged_Description")]
        public event EventHandler ActivePaneChanged
        {
            add { Events.AddHandler(s_activePaneChangedEvent, value); }
            remove { Events.RemoveHandler(s_activePaneChangedEvent, value); }
        }
        protected virtual void OnActivePaneChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_activePaneChangedEvent];
            if (handler != null)
                handler(this, e);
        }
    }
}
