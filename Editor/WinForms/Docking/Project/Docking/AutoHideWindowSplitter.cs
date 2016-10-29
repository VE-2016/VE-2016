


using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    internal class AutoHideWindowSplitter : SplitterBase
    {
        protected override int SplitterSize
        {
            get { return Measures.SplitterSize; }
        }

        protected override void StartDrag()
        {
            AutoHideWindow window = Parent as AutoHideWindow;
            if (window == null)
                return;

            window.DockPanel.DragHandler.BeginDragAutoHideWindowSplitter(window, this.Location);
        }
    }
}
