
using System;

namespace AIMS.Libraries.Forms.Docking
{
    internal class AutoHideState
    {
        public DockState m_dockState;
        public bool m_selected;

        public AutoHideState(DockState dockState)
        {
            m_dockState = dockState;
            m_selected = false;
        }

        public DockState DockState
        {
            get { return m_dockState; }
        }

        public bool Selected
        {
            get { return m_selected; }
            set { m_selected = value; }
        }
    }

    internal class AutoHideStateCollection
    {
        private AutoHideState[] _states;

        public AutoHideStateCollection()
        {
            _states = new AutoHideState[]  {
                                                new AutoHideState(DockState.DockTopAutoHide),
                                                new AutoHideState(DockState.DockBottomAutoHide),
                                                new AutoHideState(DockState.DockLeftAutoHide),
                                                new AutoHideState(DockState.DockRightAutoHide)
                                            };
        }

        public int Count
        {
            get { return _states.Length; }
        }

        public AutoHideState this[DockState dockState]
        {
            get
            {
                for (int i = 0; i < _states.Length; i++)
                {
                    if (_states[i].DockState == dockState)
                        return _states[i];
                }
                throw new IndexOutOfRangeException();
            }
        }

        public bool ContainsPane(DockPane pane)
        {
            if (pane.IsHidden)
                return false;

            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].DockState == pane.DockState && _states[i].Selected)
                    return true;
            }
            return false;
        }

        public void DeselectAll()
        {
            for (int i = 0; i < _states.Length; i++)
                _states[i].Selected = false;
        }
    }
}