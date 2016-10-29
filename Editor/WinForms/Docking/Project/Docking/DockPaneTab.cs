

using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/ClassDef/*'/>
    public class DockPaneTab : IDisposable
    {
        private IDockableWindow _content;

        /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/Construct[@name="(IDockContent)"]/*'/>
        public DockPaneTab(IDockableWindow content)
        {
            _content = content;
        }

        /// <exclude/>
        ~DockPaneTab()
        {
            Dispose(false);
        }

        /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/Property[@name="Content"]/*'/>
        public IDockableWindow Content
        {
            get { return _content; }
        }

        /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/Method[@name="Dispose"]/*'/>
        /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/Method[@name="Dispose()"]/*'/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <include file='CodeDoc/DockPaneTab.xml' path='//CodeDoc/Class[@name="DockPaneTab"]/Method[@name="Dispose(bool)"]/*'/>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
