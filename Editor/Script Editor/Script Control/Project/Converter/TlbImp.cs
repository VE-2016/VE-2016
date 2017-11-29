using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ScriptControl.Converter
{
    public class ReportEventEventArgs : EventArgs
    {
        public ImporterEventKind EventKind;
        public int EventCode;
        public string EventMsg;

        public ReportEventEventArgs(ImporterEventKind eventKind, int eventCode, string eventMsg)
        {
            EventKind = eventKind;
            EventCode = eventCode;
            EventMsg = eventMsg;
        }
    }

    public class ResolveRefEventArgs : EventArgs
    {
        public string Message = "";

        public ResolveRefEventArgs(string message)
        {
            Message = message;
        }
    }

    public class TlbImp
    {
        public enum REGKIND
        {
            REGKIND_DEFAULT = 0,
            REGKIND_REGISTER,
            REGKIND_NONE
        }

        private enum RegKind
        {
            RegKind_Default = 0,
            RegKind_Register = 1,
            RegKind_None = 2
        }

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void LoadTypeLibEx(String strTypeLibName, RegKind regKind, out ITypeLib typeLib);

        public event EventHandler<ReportEventEventArgs> ReportEvent;

        public event EventHandler<ResolveRefEventArgs> ResolveRef;

        public string AsmPath = "";
        public ArrayList References;

        protected virtual void OnReportEvent(ReportEventEventArgs e)
        {
            if (ReportEvent != null)
            {
                ReportEvent(this, e);
            }
        }

        protected virtual void OnResolveRef(ResolveRefEventArgs e)
        {
            if (ResolveRef != null)
            {
                ResolveRef(this, e);
            }
        }

        public void InvokeResolveRef(ResolveRefEventArgs e)
        {
            OnResolveRef(e);
        }

        public void InvokeReportEvent(ReportEventEventArgs e)
        {
            OnReportEvent(e);
        }
    }
}