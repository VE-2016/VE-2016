

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class DockWindowSplitter : SplitterBase
    {
        protected override int SplitterSize
        {
            get { return Measures.SplitterSize; }
        }

        protected override void StartDrag()
        {
            DockWindow window = Parent as DockWindow;
            if (window == null)
                return;

            window.DockPanel.DragHandler.BeginDragDockWindowSplitter(window, this.Location);
        }
    }
}
