using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/ClassDef/*'/>
    public abstract class DockPaneStripBase : Control
    {
        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Construct[@name="(DockPane)"]/*'/>
        protected internal DockPaneStripBase(DockPane pane)
        {
            _dockPane = pane;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        private DockPane _dockPane;
        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="DockPane"]/*'/>
        protected DockPane DockPane
        {
            get { return _dockPane; }
        }

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="Appearance"]/*'/>
        protected DockPane.AppearanceStyle Appearance
        {
            get { return DockPane.Appearance; }
        }

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="Tabs"]/*'/>
        protected DockPaneTabCollection Tabs
        {
            get { return DockPane.Tabs; }
        }

        internal void RefreshChanges()
        {
            OnRefreshChanges();
        }

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="OnRefreshChanges()"]/*'/>
        protected virtual void OnRefreshChanges()
        {
        }

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="MeasureHeight()"]/*'/>
        protected internal abstract int MeasureHeight();

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="EnsureTabVisible(IDockContent)"]/*'/>
        protected internal abstract void EnsureTabVisible(IDockableWindow content);

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest"]/*'/>
        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest()"]/*'/>
        protected int GetHitTest()
        {
            return GetHitTest(PointToClient(Control.MousePosition));
        }

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest(Point)"]/*'/>
        protected internal abstract int GetHitTest(Point point);

        /// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetOutlineXorPath(int)"]/*'/>
        protected internal abstract GraphicsPath GetOutlinePath(int index);

        /// <exclude/>
        protected override Size DefaultSize
        {
            get { return Size.Empty; }
        }

        /// <exclude/>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_MOUSEMOVE)
            {
                int index = GetHitTest();
                if (index != -1)
                {
                    IDockableWindow content = Tabs[index].Content;
                    if (DockPane.ActiveContent != content)
                    {
                        DockPane.ActiveContent = content;
                        DockPane.Activate();
                        Update();
                    }
                    if (DockPane.DockPanel.AllowRedocking && DockPane.AllowRedocking && DockPane.ActiveContent.DockHandler.AllowRedocking)
                        DockPane.DockPanel.DragHandler.BeginDragContent(DockPane.ActiveContent);
                }
                else
                    base.WndProc(ref m);
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDOWN)
            {
                int index = GetHitTest();
                if (index != -1)
                {
                    IDockableWindow content = Tabs[index].Content;
                    if (DockPane.ActiveContent != content)
                    {
                        DockPane.ActiveContent = content;
                        DockPane.Activate();
                        Update();
                    }
                    if (DockPane.DockPanel.AllowRedocking && DockPane.AllowRedocking && DockPane.ActiveContent.DockHandler.AllowRedocking)
                        DockPane.DockPanel.DragHandler.BeginDragContent(DockPane.ActiveContent);
                }
                else
                    base.WndProc(ref m);
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_RBUTTONDOWN)
            {
                int index = GetHitTest();
                if (index != -1)
                {
                    IDockableWindow content = Tabs[index].Content;
                    if (DockPane.ActiveContent != content)
                        DockPane.ActiveContent = content;
                }
                base.WndProc(ref m);
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_RBUTTONUP)
            {
                int index = GetHitTest();
                if (index != -1)
                {
                    IDockableWindow content = Tabs[index].Content;
                    if (content.DockHandler.TabPageContextMenuStrip != null)
                        content.DockHandler.TabPageContextMenuStrip.Show(this, this.PointToClient(Control.MousePosition));
                    else if (content.DockHandler.TabPageContextMenu != null)
                        content.DockHandler.TabPageContextMenu.Show(this, this.PointToClient(Control.MousePosition));
                }
                base.WndProc(ref m);
                return;
            }
            else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
            {
                base.WndProc(ref m);

                int index = GetHitTest();
                if (DockPane.DockPanel.AllowRedocking && index != -1)
                {
                    IDockableWindow content = Tabs[index].Content;
                    try { content.DockHandler.IsFloat = !content.DockHandler.IsFloat; }
                    catch { }
                }

                return;
            }

            base.WndProc(ref m);
            return;
        }
    }
}
