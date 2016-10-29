

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/ClassDef/*'/>
    [ToolboxItem(false)]
    public class DockWindow : Panel, IDockListContainer
    {
        private DockContainer _dockPanel;
        private DockState _dockState;
        private DockWindowSplitter _splitter;
        private DockList _dockList;

        internal DockWindow(DockContainer dockPanel, DockState dockState)
        {
            _dockList = new DockList(this);
            _dockPanel = dockPanel;
            _dockState = dockState;
            Visible = false;

            SuspendLayout();

            if (DockState == DockState.DockLeft || DockState == DockState.DockRight ||
                DockState == DockState.DockTop || DockState == DockState.DockBottom)
            {
                _splitter = new DockWindowSplitter();
                Controls.Add(_splitter);
            }

            if (DockState == DockState.DockLeft)
            {
                Dock = DockStyle.Left;
                _splitter.Dock = DockStyle.Right;
            }
            else if (DockState == DockState.DockRight)
            {
                Dock = DockStyle.Right;
                _splitter.Dock = DockStyle.Left;
            }
            else if (DockState == DockState.DockTop)
            {
                Dock = DockStyle.Top;
                _splitter.Dock = DockStyle.Bottom;
            }
            else if (DockState == DockState.DockBottom)
            {
                Dock = DockStyle.Bottom;
                _splitter.Dock = DockStyle.Top;
            }
            else if (DockState == DockState.Document)
            {
                Dock = DockStyle.Fill;
            }

            ResumeLayout();
        }

        /// <exclude/>
        protected override Size DefaultSize
        {
            // Set the default size to empty to reduce the screen flicker
            get { return Size.Empty; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="DisplayingList"]/*'/>
        public DisplayingDockList DisplayingList
        {
            get { return DockList.DisplayingList; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="DockList"]/*'/>
        public DockList DockList
        {
            get { return _dockList; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="DockPanel"]/*'/>
        public DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="DockState"]/*'/>
        public DockState DockState
        {
            get { return _dockState; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="IsFloat"]/*'/>
        public bool IsFloat
        {
            get { return DockState == DockState.Float; }
        }

        internal DockPane DefaultPane
        {
            get { return DisplayingList.Count == 0 ? null : DisplayingList[0]; }
        }

        /// <include file='CodeDoc\DockWindow.xml' path='//CodeDoc/Class[@name="DockWindow"]/Property[@name="DisplayingRectangle"]/*'/>
        public virtual Rectangle DisplayingRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;
                // if DockWindow is document, exclude the border
                if (DockState == DockState.Document)
                {
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width -= 2;
                    rect.Height -= 2;
                }
                // exclude the splitter
                else if (DockState == DockState.DockLeft)
                    rect.Width -= Measures.SplitterSize;
                else if (DockState == DockState.DockRight)
                {
                    rect.X += Measures.SplitterSize;
                    rect.Width -= Measures.SplitterSize;
                }
                else if (DockState == DockState.DockTop)
                    rect.Height -= Measures.SplitterSize;
                else if (DockState == DockState.DockBottom)
                {
                    rect.Y += Measures.SplitterSize;
                    rect.Height -= Measures.SplitterSize;
                }

                return rect;
            }
        }

        /// <exclude/>
        protected override void OnPaint(PaintEventArgs e)
        {
            // if DockWindow is document, draw the border
            if (DockState == DockState.Document)
                e.Graphics.DrawRectangle(SystemPens.ControlDark, ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);

            base.OnPaint(e);
        }

        /// <exclude/>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            DisplayingList.Refresh();
            if (DisplayingList.Count == 0)
            {
                if (Visible)
                    Visible = false;
            }
            else if (!Visible)
            {
                Visible = true;
                DisplayingList.Refresh();
            }

            base.OnLayout(levent);
        }

        /// <exclude/>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_WINDOWPOSCHANGING)
            {
                int offset = (int)Marshal.OffsetOf(typeof(Win32.WINDOWPOS), "flags");
                int flags = Marshal.ReadInt32(m.LParam, offset);
                Marshal.WriteInt32(m.LParam, offset, flags | (int)Win32.FlagsSetWindowPos.SWP_NOCOPYBITS);
            }

            base.WndProc(ref m);
        }
    }
}
