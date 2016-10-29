
using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/ClassDef/*'/>
    public class AutoHideTab : IDisposable
    {
        private IDockableWindow _content;

        /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/Construct[@name="(IDockContent)"]/*'/>
        public AutoHideTab(IDockableWindow content)
        {
            _content = content;
        }

        /// <exclude/>
        ~AutoHideTab()
        {
            Dispose(false);
        }

        /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/Property[@name="Content"]/*'/>
        public IDockableWindow Content
        {
            get { return _content; }
        }

        /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/Method[@name="Dispose"]/*'/>
        /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/Method[@name="Dispose()"]/*'/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <include file='CodeDoc/AutoHideTab.xml' path='//CodeDoc/Class[@name="AutoHideTab"]/Method[@name="Dispose(bool)"]/*'/>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
