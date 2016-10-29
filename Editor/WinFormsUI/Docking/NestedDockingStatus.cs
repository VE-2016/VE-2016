using System;
using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class NestedDockingStatus
    {
        internal NestedDockingStatus(DockPane pane)
        {
            _dockPane = pane;
        }

        private DockPane _dockPane = null;
        public DockPane DockPane
        {
            get { return _dockPane; }
        }

        private NestedPaneCollection _nestedPanes = null;
        public NestedPaneCollection NestedPanes
        {
            get { return _nestedPanes; }
        }

        private DockPane _previousPane = null;
        public DockPane PreviousPane
        {
            get { return _previousPane; }
        }

        private DockAlignment _alignment = DockAlignment.Left;
        public DockAlignment Alignment
        {
            get { return _alignment; }
        }

        private double _proportion = 0.5;
        public double Proportion
        {
            get { return _proportion; }
        }

        private bool _isDisplaying = false;
        public bool IsDisplaying
        {
            get { return _isDisplaying; }
        }

        private DockPane _displayingPreviousPane = null;
        public DockPane DisplayingPreviousPane
        {
            get { return _displayingPreviousPane; }
        }

        private DockAlignment _displayingAlignment = DockAlignment.Left;
        public DockAlignment DisplayingAlignment
        {
            get { return _displayingAlignment; }
        }

        private double _displayingProportion = 0.5;
        public double DisplayingProportion
        {
            get { return _displayingProportion; }
        }

        private Rectangle _logicalBounds = Rectangle.Empty;
        public Rectangle LogicalBounds
        {
            get { return _logicalBounds; }
        }

        private Rectangle _paneBounds = Rectangle.Empty;
        public Rectangle PaneBounds
        {
            get { return _paneBounds; }
        }

        private Rectangle _splitterBounds = Rectangle.Empty;
        public Rectangle SplitterBounds
        {
            get { return _splitterBounds; }
        }

        internal void SetStatus(NestedPaneCollection nestedPanes, DockPane previousPane, DockAlignment alignment, double proportion)
        {
            _nestedPanes = nestedPanes;
            _previousPane = previousPane;
            _alignment = alignment;
            _proportion = proportion;
        }

        internal void SetDisplayingStatus(bool isDisplaying, DockPane displayingPreviousPane, DockAlignment displayingAlignment, double displayingProportion)
        {
            _isDisplaying = isDisplaying;
            _displayingPreviousPane = displayingPreviousPane;
            _displayingAlignment = displayingAlignment;
            _displayingProportion = displayingProportion;
        }

        internal void SetDisplayingBounds(Rectangle logicalBounds, Rectangle paneBounds, Rectangle splitterBounds)
        {
            _logicalBounds = logicalBounds;
            _paneBounds = paneBounds;
            _splitterBounds = splitterBounds;
        }
    }
}
