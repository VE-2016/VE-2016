using System;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockContentEventArgs : EventArgs
    {
        private IDockContent _content;

        public DockContentEventArgs(IDockContent content)
        {
            _content = content;
        }

        public IDockContent Content
        {
            get { return _content; }
        }
    }
}
