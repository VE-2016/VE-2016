using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/ClassDef/*'/>
    public class NestedDockingStatus
    {
        internal NestedDockingStatus(DockPane pane)
        {
            _dockPane = pane;
        }

        private DockPane _dockPane = null;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="DockPane"]/*'/>
        public DockPane DockPane
        {
            get { return _dockPane; }
        }

        private DockList _dockList = null;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="DockList"]/*'/>
        public DockList DockList
        {
            get { return _dockList; }
        }

        private DockPane _prevPane = null;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="PrevPane"]/*'/>
        public DockPane PrevPane
        {
            get { return _prevPane; }
        }

        private DockAlignment _alignment = DockAlignment.Left;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="Alignment"]/*'/>
        public DockAlignment Alignment
        {
            get { return _alignment; }
        }

        private double _proportion = 0.5;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="Proportion"]/*'/>
        public double Proportion
        {
            get { return _proportion; }
        }

        private bool _isDisplaying = false;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="IsDisplaying"]/*'/>
        public bool IsDisplaying
        {
            get { return _isDisplaying; }
        }

        private DockPane _displayingPrevPane = null;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="DisplayingPrevPane"]/*'/>
        public DockPane DisplayingPrevPane
        {
            get { return _displayingPrevPane; }
        }

        private DockAlignment _displayingAlignment = DockAlignment.Left;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="DisplayingAlignment"]/*'/>
        public DockAlignment DisplayingAlignment
        {
            get { return _displayingAlignment; }
        }

        private double _displayingProportion = 0.5;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="DisplayingProportion"]/*'/>
        public double DisplayingProportion
        {
            get { return _displayingProportion; }
        }

        private Rectangle _logicalBounds = Rectangle.Empty;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="LogicalBounds"]/*'/>
        public Rectangle LogicalBounds
        {
            get { return _logicalBounds; }
        }

        private Rectangle _paneBounds = Rectangle.Empty;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="PaneBounds"]/*'/>
        public Rectangle PaneBounds
        {
            get { return _paneBounds; }
        }

        private Rectangle _splitterBounds = Rectangle.Empty;
        /// <include file='CodeDoc\NestedDockingStatus.xml' path='//CodeDoc/Class[@name="NestedDockingStatus"]/Property[@name="SplitterBounds"]/*'/>
        public Rectangle SplitterBounds
        {
            get { return _splitterBounds; }
        }

        internal void SetStatus(DockList list, DockPane prevPane, DockAlignment alignment, double proportion)
        {
            _dockList = list;
            _prevPane = prevPane;
            _alignment = alignment;
            _proportion = proportion;
        }

        internal void SetDisplayingStatus(bool isDisplaying, DockPane displayingPrevPane, DockAlignment displayingAlignment, double displayingProportion)
        {
            _isDisplaying = isDisplaying;
            _displayingPrevPane = displayingPrevPane;
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
