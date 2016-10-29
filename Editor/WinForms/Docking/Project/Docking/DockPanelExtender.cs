using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/ClassDef/*'/>
    public sealed class DockPanelExtender
    {
        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/InterfaceDef/*'/>
        public interface IDockPaneFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane"]/*'/>
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, DockState, bool)"]/*'/>
            DockPane CreateDockPane(IDockableWindow content, DockState visibleState, bool show);
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, FloatWindow, bool)"]/*'/>
            DockPane CreateDockPane(IDockableWindow content, FloatWindow floatWindow, bool show);
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, DockPane, DockAlignment, double, bool)"]/*'/>
            DockPane CreateDockPane(IDockableWindow content, DockPane prevPane, DockAlignment alignment, double proportion, bool show);
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneFactory"]/Method[@name="CreateDockPane(IDockContent, Rectangle, bool)"]/*'/>
            DockPane CreateDockPane(IDockableWindow content, Rectangle floatWindowBounds, bool show);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/InterfaceDef/*'/>
        public interface IFloatWindowFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow"]/*'/>
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane)"]/*'/>
            FloatWindow CreateFloatWindow(DockContainer dockPanel, DockPane pane);
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IFloatWindowFactory"]/Method[@name="CreateFloatWindow(DockPanel, DockPane, Rectangle)"]/*'/>
            FloatWindow CreateFloatWindow(DockContainer dockPanel, DockPane pane, Rectangle bounds);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneCaptionFactory"]/InterfaceDef/*'/>
        public interface IDockPaneCaptionFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneCaptionFactory"]/Method[@name="CreateDockPaneCaption(DockPane)"]/*'/>
            DockPaneCaptionBase CreateDockPaneCaption(DockPane pane);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneStripFactory"]/InterfaceDef/*'/>
        public interface IDockPaneStripFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneStripFactory"]/Method[@name="CreateDockPaneStrip(DockPane)"]/*'/>
            DockPaneStripBase CreateDockPaneStrip(DockPane pane);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideStripFactory"]/InterfaceDef/*'/>
        public interface IAutoHideStripFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideStripFactory"]/Method[@name="CreateAutoHideStrip(DockPanel)"]/*'/>
            AutoHideStripBase CreateAutoHideStrip(DockContainer panel);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideTabFactory"]/InterfaceDef/*'/>
        public interface IAutoHideTabFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHideTabFactory"]/Method[@name="CreateAutoHideTab(IDockContent)"]/*'/>
            AutoHideTab CreateAutoHideTab(IDockableWindow content);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHidePaneFactory"]/InterfaceDef/*'/>
        public interface IAutoHidePaneFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IAutoHidePaneFactory"]/Method[@name="CreateAutoHidePane(DockPane)"]/*'/>
            AutoHidePane CreateAutoHidePane(DockPane pane);
        }

        /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneTabFactory"]/InterfaceDef/*'/>
        public interface IDockPaneTabFactory
        {
            /// <include file='CodeDoc\DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Interface[@name="IDockPaneTabFactory"]/Method[@name="CreateDockPaneTab(IDockContent)"]/*'/>
            DockPaneTab CreateDockPaneTab(IDockableWindow content);
        }

        #region DefaultDockPaneFactory
        private class DefaultDockPaneFactory : IDockPaneFactory
        {
            public DockPane CreateDockPane(IDockableWindow content, DockState visibleState, bool show)
            {
                return new DockPane(content, visibleState, show);
            }

            public DockPane CreateDockPane(IDockableWindow content, FloatWindow floatWindow, bool show)
            {
                return new DockPane(content, floatWindow, show);
            }

            public DockPane CreateDockPane(IDockableWindow content, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
            {
                return new DockPane(content, prevPane, alignment, proportion, show);
            }

            public DockPane CreateDockPane(IDockableWindow content, Rectangle floatWindowBounds, bool show)
            {
                return new DockPane(content, floatWindowBounds, show);
            }
        }
        #endregion

        #region DefaultFloatWindowFactory
        private class DefaultFloatWindowFactory : IFloatWindowFactory
        {
            public FloatWindow CreateFloatWindow(DockContainer dockPanel, DockPane pane)
            {
                return new FloatWindow(dockPanel, pane);
            }

            public FloatWindow CreateFloatWindow(DockContainer dockPanel, DockPane pane, Rectangle bounds)
            {
                return new FloatWindow(dockPanel, pane, bounds);
            }
        }
        #endregion

        #region DefaultDockPaneCaptionFactory
        private class DefaultDockPaneCaptionFactory : IDockPaneCaptionFactory
        {
            public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
            {
                return new DockPaneCaptionVS2005(pane);
            }
        }
        #endregion

        #region DefaultDockPaneTabFactory
        private class DefaultDockPaneTabFactory : IDockPaneTabFactory
        {
            public DockPaneTab CreateDockPaneTab(IDockableWindow content)
            {
                return new DockPaneTabVS2005(content);
            }
        }
        #endregion

        #region DefaultDockPaneTabStripFactory
        private class DefaultDockPaneStripFactory : IDockPaneStripFactory
        {
            public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
            {
                return new DockPaneStripVS2005(pane);
            }
        }
        #endregion

        #region DefaultAutoHidePaneFactory
        private class DefaultAutoHidePaneFactory : IAutoHidePaneFactory
        {
            public AutoHidePane CreateAutoHidePane(DockPane pane)
            {
                return new AutoHidePane(pane);
            }
        }
        #endregion

        #region DefaultAutoHideTabFactory
        private class DefaultAutoHideTabFactory : IAutoHideTabFactory
        {
            public AutoHideTab CreateAutoHideTab(IDockableWindow content)
            {
                return new AutoHideTabVS2005(content);
            }
        }
        #endregion

        #region DefaultAutoHideStripFactory
        private class DefaultAutoHideStripFactory : IAutoHideStripFactory
        {
            public AutoHideStripBase CreateAutoHideStrip(DockContainer panel)
            {
                return new AutoHideStripVS2005(panel);
            }
        }
        #endregion

        internal DockPanelExtender(DockContainer dockPanel)
        {
            _dockPanel = dockPanel;
        }

        private DockContainer _dockPanel;
        private DockContainer DockPanel
        {
            get { return _dockPanel; }
        }

        private IDockPaneFactory _dockPaneFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneFactory"]/*'/>
        public IDockPaneFactory DockPaneFactory
        {
            get
            {
                if (_dockPaneFactory == null)
                    _dockPaneFactory = new DefaultDockPaneFactory();

                return _dockPaneFactory;
            }
            set
            {
                if (DockPanel.Panes.Count > 0)
                    throw new InvalidOperationException();

                _dockPaneFactory = value;
            }
        }

        private IFloatWindowFactory _floatWindowFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="FloatWindowFactory"]/*'/>
        public IFloatWindowFactory FloatWindowFactory
        {
            get
            {
                if (_floatWindowFactory == null)
                    _floatWindowFactory = new DefaultFloatWindowFactory();

                return _floatWindowFactory;
            }
            set
            {
                if (DockPanel.FloatWindows.Count > 0)
                    throw new InvalidOperationException();

                _floatWindowFactory = value;
            }
        }

        private IDockPaneCaptionFactory _dockPaneCaptionFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneCaptionFactory"]/*'/>
        public IDockPaneCaptionFactory DockPaneCaptionFactory
        {
            get
            {
                if (_dockPaneCaptionFactory == null)
                    _dockPaneCaptionFactory = new DefaultDockPaneCaptionFactory();

                return _dockPaneCaptionFactory;
            }
            set
            {
                if (DockPanel.Panes.Count > 0)
                    throw new InvalidOperationException();

                _dockPaneCaptionFactory = value;
            }
        }

        private IDockPaneTabFactory _dockPaneTabFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneTabFactory"]/*'/>
        public IDockPaneTabFactory DockPaneTabFactory
        {
            get
            {
                if (_dockPaneTabFactory == null)
                    _dockPaneTabFactory = new DefaultDockPaneTabFactory();

                return _dockPaneTabFactory;
            }
            set
            {
                if (DockPanel.Contents.Count > 0)
                    throw new InvalidOperationException();

                _dockPaneTabFactory = value;
            }
        }

        private IDockPaneStripFactory _dockPaneStripFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="DockPaneStripFactory"]/*'/>
        public IDockPaneStripFactory DockPaneStripFactory
        {
            get
            {
                if (_dockPaneStripFactory == null)
                    _dockPaneStripFactory = new DefaultDockPaneStripFactory();

                return _dockPaneStripFactory;
            }
            set
            {
                if (DockPanel.Contents.Count > 0)
                    throw new InvalidOperationException();

                _dockPaneStripFactory = value;
            }
        }

        private IAutoHidePaneFactory _autoHidePaneFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHidePaneFactory"]/*'/>
        public IAutoHidePaneFactory AutoHidePaneFactory
        {
            get
            {
                if (_autoHidePaneFactory == null)
                    _autoHidePaneFactory = new DefaultAutoHidePaneFactory();

                return _autoHidePaneFactory;
            }
            set
            {
                if (DockPanel.Contents.Count > 0)
                    throw new InvalidOperationException();

                _autoHidePaneFactory = value;
            }
        }

        private IAutoHideTabFactory _autoHideTabFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHideTabFactory"]/*'/>
        public IAutoHideTabFactory AutoHideTabFactory
        {
            get
            {
                if (_autoHideTabFactory == null)
                    _autoHideTabFactory = new DefaultAutoHideTabFactory();

                return _autoHideTabFactory;
            }
            set
            {
                if (DockPanel.Contents.Count > 0)
                    throw new InvalidOperationException();

                _autoHideTabFactory = value;
            }
        }

        private IAutoHideStripFactory _autoHideStripFactory = null;
        /// <include file='CodeDoc/DockPanelExtender.xml' path='//CodeDoc/Class[@name="DockPanelExtender"]/Property[@name="AutoHideStripFactory"]/*'/>
        public IAutoHideStripFactory AutoHideStripFactory
        {
            get
            {
                if (_autoHideStripFactory == null)
                    _autoHideStripFactory = new DefaultAutoHideStripFactory();

                return _autoHideStripFactory;
            }
            set
            {
                if (DockPanel.Contents.Count > 0)
                    throw new InvalidOperationException();

                if (_autoHideStripFactory == value)
                    return;

                _autoHideStripFactory = value;
                DockPanel.ReCreateAutoHideStripControl();
            }
        }
    }
}
