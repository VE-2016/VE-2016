

using System;
using System.Collections;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/ClassDef/*'/>>
    public sealed class AutoHideTabCollection : IEnumerable
    {
        #region class AutoHideTabEnumerator
        private class AutoHideTabEnumerator : IEnumerator
        {
            private AutoHideTabCollection _tabs;
            private int _index;

            public AutoHideTabEnumerator(AutoHideTabCollection tabs)
            {
                _tabs = tabs;
                Reset();
            }

            public bool MoveNext()
            {
                _index++;
                return (_index < _tabs.Count);
            }

            public object Current
            {
                get { return _tabs[_index]; }
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
            return new AutoHideTabEnumerator(this);
        }
        #endregion

        internal AutoHideTabCollection(DockPane pane)
        {
            _dockPane = pane;
            _dockPanel = pane.DockPanel;
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="AutoHidePane"]/*'/>>
        public AutoHidePane AutoHidePane
        {
            get { return DockPane.AutoHidePane; }
        }

        private DockPane _dockPane = null;
        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="DockPane"]/*'/>>
        public DockPane DockPane
        {
            get { return _dockPane; }
        }

        private DockContainer _dockPanel = null;
        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="DockPanel"]/*'/>>
        public DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="Count"]/*'/>>
        public int Count
        {
            get { return DockPane.DisplayingContents.Count; }
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Property[@name="Item"]/*'/>>
        public AutoHideTab this[int index]
        {
            get
            {
                IDockableWindow content = DockPane.DisplayingContents[index];
                if (content == null)
                    throw (new IndexOutOfRangeException());
                return content.DockHandler.AutoHideTab;
            }
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains"]/*'/>>
        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains(AutoHideTab)"]/*'/>>
        public bool Contains(AutoHideTab tab)
        {
            return (IndexOf(tab) != -1);
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="Contains(IDockContent)"]/*'/>>
        public bool Contains(IDockableWindow content)
        {
            return (IndexOf(content) != -1);
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf"]/*'/>>
        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf(AutoHideTab)"]/*'/>>
        public int IndexOf(AutoHideTab tab)
        {
            return IndexOf(tab.Content);
        }

        /// <include file='CodeDoc\AutoHideTabCollection.xml' path='//CodeDoc/Class[@name="AutoHideTabCollection"]/Method[@name="IndexOf(IDockContent)"]/*'/>>
        public int IndexOf(IDockableWindow content)
        {
            return DockPane.DisplayingContents.IndexOf(content);
        }
    }
}
