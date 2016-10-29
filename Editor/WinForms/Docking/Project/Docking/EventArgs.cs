using System;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/ClassDef/*'/>
    public class DockContentEventArgs : EventArgs
    {
        private IDockableWindow _content;

        /// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/Constructor[@name="(IDockContent)"]/*'/>
        public DockContentEventArgs(IDockableWindow content)
        {
            _content = content;
        }

        /// <include file='CodeDoc\EventArgs.xml' path='//CodeDoc/Class[@name="DockContentEventArgs"]/Property[@name="Content"]/*'/>
        public IDockableWindow Content
        {
            get { return _content; }
        }
    }
}
