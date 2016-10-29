
using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    internal class DockPaneTabVS2005 : DockPaneTab
    {
        internal DockPaneTabVS2005(IDockableWindow content)
            : base(content)
        {
        }

        private int _tabX;
        protected internal int TabX
        {
            get { return _tabX; }
            set { _tabX = value; }
        }

        private int _tabWidth;
        protected internal int TabWidth
        {
            get { return _tabWidth; }
            set { _tabWidth = value; }
        }

        private int _maxWidth;
        protected internal int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }

        private bool _flag;
        protected internal bool Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
    }
}
