

using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/ClassDef/*'/>
    public class AutoHidePane : IDisposable
    {
        private DockPane _dockPane;

        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Construct[@name="(DockPane)"]/*'/>
        public AutoHidePane(DockPane pane)
        {
            _dockPane = pane;
            _tabs = new AutoHideTabCollection(DockPane);
        }

        /// <exclude/>
        ~AutoHidePane()
        {
            Dispose(false);
        }

        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Property[@name="DockPane"]/*'/>
        public DockPane DockPane
        {
            get { return _dockPane; }
        }

        private AutoHideTabCollection _tabs;
        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Property[@name="Tabs"]/*'/>
        public AutoHideTabCollection Tabs
        {
            get { return _tabs; }
        }

        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Method[@name="Dispose"]/*'/>
        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Method[@name="Dispose()"]/*'/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <include file='CodeDoc/AutoHidePane.xml' path='//CodeDoc/Class[@name="AutoHidePane"]/Method[@name="Dispose(bool)"]/*'/>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
