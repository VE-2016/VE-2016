
using System;
using System.Collections;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/ClassDef/*'/>>
    public sealed class AutoHidePaneCollection : IEnumerable
    {
        #region class AutoHidePaneEnumerator
        private class AutoHidePaneEnumerator : IEnumerator
        {
            private AutoHidePaneCollection _panes;
            private int _index;

            public AutoHidePaneEnumerator(AutoHidePaneCollection panes)
            {
                _panes = panes;
                Reset();
            }

            public bool MoveNext()
            {
                _index++;
                return (_index < _panes.Count);
            }

            public object Current
            {
                get { return _panes[_index]; }
            }

            public void Reset()
            {
                _index = -1;
            }
        }
        #endregion

        #region IEnumerable Members
        /// <exclude />
        public IEnumerator GetEnumerator()
        {
            return new AutoHidePaneEnumerator(this);
        }
        #endregion

        internal AutoHidePaneCollection(DockContainer panel, DockState dockState)
        {
            _dockPanel = panel;
            _states = new AutoHideStateCollection();
            States[DockState.DockTopAutoHide].Selected = (dockState == DockState.DockTopAutoHide);
            States[DockState.DockBottomAutoHide].Selected = (dockState == DockState.DockBottomAutoHide);
            States[DockState.DockLeftAutoHide].Selected = (dockState == DockState.DockLeftAutoHide);
            States[DockState.DockRightAutoHide].Selected = (dockState == DockState.DockRightAutoHide);
        }

        private DockContainer _dockPanel;
        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="DockPanel"]/*'/>>
        public DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        private AutoHideStateCollection _states;
        private AutoHideStateCollection States
        {
            get { return _states; }
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="Count"]/*'/>>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (DockPane pane in DockPanel.Panes)
                {
                    if (States.ContainsPane(pane))
                        count++;
                }

                return count;
            }
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Property[@name="Item"]/*'/>>
        public AutoHidePane this[int index]
        {
            get
            {
                int count = 0;
                foreach (DockPane pane in DockPanel.Panes)
                {
                    if (!States.ContainsPane(pane))
                        continue;

                    if (count == index)
                        return pane.AutoHidePane;

                    count++;
                }
                throw new IndexOutOfRangeException();
            }
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains"]/*'/>>
        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains(AutoHidePane)"]/*'/>>
        public bool Contains(AutoHidePane autoHidePane)
        {
            return (IndexOf(autoHidePane) != -1);
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="Contains(DockPane)"]/*'/>>
        public bool Contains(DockPane pane)
        {
            return (IndexOf(pane) != -1);
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf"]/*'/>>
        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf(AutoHidePane)"]/*'/>>
        public int IndexOf(AutoHidePane autoHidePane)
        {
            return IndexOf(autoHidePane.DockPane);
        }

        /// <include file='CodeDoc\AutoHidePaneCollection.xml' path='//CodeDoc/Class[@name="AutoHidePaneCollection"]/Method[@name="IndexOf(DockPane)"]/*'/>>
        public int IndexOf(DockPane pane)
        {
            int index = 0;
            foreach (DockPane dockPane in DockPanel.Panes)
            {
                if (!States.ContainsPane(pane))
                    continue;

                if (pane == dockPane)
                    return index;

                index++;
            }
            return -1;
        }
    }
}
