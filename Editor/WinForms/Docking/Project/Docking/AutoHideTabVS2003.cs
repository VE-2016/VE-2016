using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    internal class AutoHideTabVS2003 : AutoHideTab
    {
        internal AutoHideTabVS2003(IDockableWindow content) : base(content)
        {
        }

        private int _tabX = 0;
        protected internal int TabX
        {
            get { return _tabX; }
            set { _tabX = value; }
        }

        private int _tabWidth = 0;
        protected internal int TabWidth
        {
            get { return _tabWidth; }
            set { _tabWidth = value; }
        }
    }
}
