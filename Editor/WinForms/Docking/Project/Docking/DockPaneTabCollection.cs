

using System;
using System.Collections;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/ClassDef/*'/>>
    public sealed class DockPaneTabCollection : IEnumerable
    {
        #region class DockPaneTabEnumerator
        private class DockPaneTabEnumerator : IEnumerator
        {
            private DockPaneTabCollection _tabs;
            private int _index;

            public DockPaneTabEnumerator(DockPaneTabCollection tabs)
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
        /// <exclude/>
        public IEnumerator GetEnumerator()
        {
            return new DockPaneTabEnumerator(this);
        }
        #endregion

        internal DockPaneTabCollection(DockPane pane)
        {
            _dockPane = pane;
        }

        private DockPane _dockPane;
        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="DockPane"]/*'/>>
        public DockPane DockPane
        {
            get { return _dockPane; }
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="Count"]/*'/>>
        public int Count
        {
            get { return DockPane.DisplayingContents.Count; }
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Property[@name="Item"]/*'/>>
        public DockPaneTab this[int index]
        {
            get
            {
                IDockableWindow content = DockPane.DisplayingContents[index];
                if (content == null)
                    return null;// throw (new IndexOutOfRangeException());
                return content.DockHandler.DockPaneTab;
            }
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains"]/*'/>>
        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains(DockPaneTab)"]/*'/>>
        public bool Contains(DockPaneTab tab)
        {
            return (IndexOf(tab) != -1);
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="Contains(IDockContent)"]/*'/>>
        public bool Contains(IDockableWindow content)
        {
            return (IndexOf(content) != -1);
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf"]/*'/>>
        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf(DockPaneTab)"]/*'/>>
        public int IndexOf(DockPaneTab tab)
        {
            return DockPane.DisplayingContents.IndexOf(tab.Content);
        }

        /// <include file='CodeDoc\DockPaneTabCollection.xml' path='//CodeDoc/Class[@name="DockPaneTabCollection"]/Method[@name="IndexOf(IDockContent)"]/*'/>>
        public int IndexOf(IDockableWindow content)
        {
            return DockPane.DisplayingContents.IndexOf(content);
        }
    }
}
