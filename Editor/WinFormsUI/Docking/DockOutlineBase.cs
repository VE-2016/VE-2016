using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public abstract class DockOutlineBase
    {
        public DockOutlineBase()
        {
            Init();
        }

        private void Init()
        {
            SetValues(Rectangle.Empty, null, DockStyle.None, -1);
            SaveOldValues();
        }

        private Rectangle _oldFloatWindowBounds;
        protected Rectangle OldFloatWindowBounds
        {
            get { return _oldFloatWindowBounds; }
        }

        private Control _oldDockTo;
        protected Control OldDockTo
        {
            get { return _oldDockTo; }
        }

        private DockStyle _oldDock;
        protected DockStyle OldDock
        {
            get { return _oldDock; }
        }

        private int _oldContentIndex;
        protected int OldContentIndex
        {
            get { return _oldContentIndex; }
        }

        protected bool SameAsOldValue
        {
            get
            {
                return FloatWindowBounds == OldFloatWindowBounds &&
                    DockTo == OldDockTo &&
                    Dock == OldDock &&
                    ContentIndex == OldContentIndex;
            }
        }

        private Rectangle _floatWindowBounds;
        public Rectangle FloatWindowBounds
        {
            get { return _floatWindowBounds; }
        }

        private Control _dockTo;
        public Control DockTo
        {
            get { return _dockTo; }
        }

        private DockStyle _dock;
        public DockStyle Dock
        {
            get { return _dock; }
        }

        private int _contentIndex;
        public int ContentIndex
        {
            get { return _contentIndex; }
        }

        public bool FlagFullEdge
        {
            get { return _contentIndex != 0; }
        }

        private bool _flagTestDrop = false;
        public bool FlagTestDrop
        {
            get { return _flagTestDrop; }
            set { _flagTestDrop = value; }
        }

        private void SaveOldValues()
        {
            _oldDockTo = _dockTo;
            _oldDock = _dock;
            _oldContentIndex = _contentIndex;
            _oldFloatWindowBounds = _floatWindowBounds;
        }

        protected abstract void OnShow();

        protected abstract void OnClose();

        private void SetValues(Rectangle floatWindowBounds, Control dockTo, DockStyle dock, int contentIndex)
        {
            _floatWindowBounds = floatWindowBounds;
            _dockTo = dockTo;
            _dock = dock;
            _contentIndex = contentIndex;
            FlagTestDrop = true;
        }

        private void TestChange()
        {
            if (_floatWindowBounds != _oldFloatWindowBounds ||
                _dockTo != _oldDockTo ||
                _dock != _oldDock ||
                _contentIndex != _oldContentIndex)
                OnShow();
        }

        public void Show()
        {
            SaveOldValues();
            SetValues(Rectangle.Empty, null, DockStyle.None, -1);
            TestChange();
        }

        public void Show(DockPane pane, DockStyle dock)
        {
            SaveOldValues();
            SetValues(Rectangle.Empty, pane, dock, -1);
            TestChange();
        }

        public void Show(DockPane pane, int contentIndex)
        {
            SaveOldValues();
            SetValues(Rectangle.Empty, pane, DockStyle.Fill, contentIndex);
            TestChange();
        }

        public void Show(DockPanel dockPanel, DockStyle dock, bool fullPanelEdge)
        {
            SaveOldValues();
            SetValues(Rectangle.Empty, dockPanel, dock, fullPanelEdge ? -1 : 0);
            TestChange();
        }

        public void Show(Rectangle floatWindowBounds)
        {
            SaveOldValues();
            SetValues(floatWindowBounds, null, DockStyle.None, -1);
            TestChange();
        }

        public void Close()
        {
            OnClose();
        }
    }
}
