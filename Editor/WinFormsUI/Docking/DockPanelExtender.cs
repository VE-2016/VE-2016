using System;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class DockPanelExtender
    {
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneFactory
        {
            DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show);

            [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
            DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show);

            DockPane CreateDockPane(IDockContent content, DockPane previousPane, DockAlignment alignment,
                                    double proportion, bool show);

            [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
            DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show);
        }

        public interface IDockPaneSplitterControlFactory
        {
            DockPane.SplitterControlBase CreateSplitterControl(DockPane pane);
        }

        public interface IDockWindowSplitterControlFactory
        {
            SplitterBase CreateSplitterControl();
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IFloatWindowFactory
        {
            FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane);
            FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds);
        }

        public interface IDockWindowFactory
        {
            DockWindow CreateDockWindow(DockPanel dockPanel, DockState dockState);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneCaptionFactory
        {
            DockPaneCaptionBase CreateDockPaneCaption(DockPane pane);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneStripFactory
        {
            DockPaneStripBase CreateDockPaneStrip(DockPane pane);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IAutoHideStripFactory
        {
            AutoHideStripBase CreateAutoHideStrip(DockPanel panel);
        }

        public interface IAutoHideWindowFactory
        {
            DockPanel.AutoHideWindowControl CreateAutoHideWindow(DockPanel panel);
        }

        public interface IPaneIndicatorFactory
        {
            DockPanel.IPaneIndicator CreatePaneIndicator();
        }

        public interface IPanelIndicatorFactory
        {
            DockPanel.IPanelIndicator CreatePanelIndicator(DockStyle style);
        }

        public interface IDockOutlineFactory
        {
            DockOutlineBase CreateDockOutline();
        }

        #region DefaultDockPaneFactory

        private class DefaultDockPaneFactory : IDockPaneFactory
        {
            public DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show)
            {
                return new DockPane(content, visibleState, show);
            }

            public DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show)
            {
                return new DockPane(content, floatWindow, show);
            }

            public DockPane CreateDockPane(IDockContent content, DockPane prevPane, DockAlignment alignment,
                                           double proportion, bool show)
            {
                return new DockPane(content, prevPane, alignment, proportion, show);
            }

            public DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show)
            {
                return new DockPane(content, floatWindowBounds, show);
            }
        }

        #endregion

        #region DefaultDockPaneSplitterControlFactory

        private class DefaultDockPaneSplitterControlFactory : IDockPaneSplitterControlFactory
        {
            public DockPane.SplitterControlBase CreateSplitterControl(DockPane pane)
            {
                return new DockPane.DefaultSplitterControl(pane);
            }
        }

        #endregion

        #region DefaultDockWindowSplitterControlFactory

        private class DefaultDockWindowSplitterControlFactory : IDockWindowSplitterControlFactory
        {
            public SplitterBase CreateSplitterControl()
            {
                return new DockWindow.DefaultSplitterControl();
            }
        }

        #endregion

        #region DefaultFloatWindowFactory

        private class DefaultFloatWindowFactory : IFloatWindowFactory
        {
            public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
            {
                return new FloatWindow(dockPanel, pane);
            }

            public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
            {
                return new FloatWindow(dockPanel, pane, bounds);
            }
        }

        #endregion

        #region DefaultDockWindowFactory

        private class DefaultDockWindowFactory : IDockWindowFactory
        {
            public DockWindow CreateDockWindow(DockPanel dockPanel, DockState dockState)
            {
                return new DefaultDockWindow(dockPanel, dockState);
            }
        }

        #endregion

        #region DefaultDockPaneCaptionFactory

        private class DefaultDockPaneCaptionFactory : IDockPaneCaptionFactory
        {
            public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
            {
                return new VS2005DockPaneCaption(pane);
            }
        }

        #endregion

        #region DefaultDockPaneTabStripFactory

        private class DefaultDockPaneStripFactory : IDockPaneStripFactory
        {
            public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
            {
                return new VS2005DockPaneStrip(pane);
            }
        }

        #endregion

        #region DefaultAutoHideStripFactory

        private class DefaultAutoHideStripFactory : IAutoHideStripFactory
        {
            public AutoHideStripBase CreateAutoHideStrip(DockPanel panel)
            {
                return new VS2005AutoHideStrip(panel);
            }
        }

        #endregion

        #region DefaultAutoHideWindowFactory

        public class DefaultAutoHideWindowFactory : IAutoHideWindowFactory
        {
            public DockPanel.AutoHideWindowControl CreateAutoHideWindow(DockPanel panel)
            {
                return new DockPanel.DefaultAutoHideWindowControl(panel);
            }
        }

        #endregion

        public class DefaultPaneIndicatorFactory : IPaneIndicatorFactory
        {
            public DockPanel.IPaneIndicator CreatePaneIndicator()
            {
                return new DockPanel.DefaultPaneIndicator();
            }
        }

        public class DefaultPanelIndicatorFactory : IPanelIndicatorFactory
        {
            public DockPanel.IPanelIndicator CreatePanelIndicator(DockStyle style)
            {
                return new DockPanel.DefaultPanelIndicator(style);
            }
        }

        public class DefaultDockOutlineFactory : IDockOutlineFactory
        {
            public DockOutlineBase CreateDockOutline()
            {
                return new DockPanel.DefaultDockOutline();
            }
        }

        internal DockPanelExtender(DockPanel dockPanel)
        {
            _dockPanel = dockPanel;
        }

        private DockPanel _dockPanel;

        private DockPanel DockPanel
        {
            get { return _dockPanel; }
        }

        private IDockPaneFactory _dockPaneFactory = null;

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

        private IDockPaneSplitterControlFactory _dockPaneSplitterControlFactory;

        public IDockPaneSplitterControlFactory DockPaneSplitterControlFactory
        {
            get
            {
                return _dockPaneSplitterControlFactory ??
                       (_dockPaneSplitterControlFactory = new DefaultDockPaneSplitterControlFactory());
            }

            set
            {
                if (DockPanel.Panes.Count > 0)
                {
                    throw new InvalidOperationException();
                }

                _dockPaneSplitterControlFactory = value;
            }
        }

        private IDockWindowSplitterControlFactory _dockWindowSplitterControlFactory;

        public IDockWindowSplitterControlFactory DockWindowSplitterControlFactory
        {
            get
            {
                return _dockWindowSplitterControlFactory ??
                       (_dockWindowSplitterControlFactory = new DefaultDockWindowSplitterControlFactory());
            }

            set
            {
                _dockWindowSplitterControlFactory = value;
                DockPanel.ReloadDockWindows();
            }
        }

        private IFloatWindowFactory _floatWindowFactory = null;

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

        private IDockWindowFactory _dockWindowFactory;

        public IDockWindowFactory DockWindowFactory
        {
            get { return _dockWindowFactory ?? (_dockWindowFactory = new DefaultDockWindowFactory()); }
            set
            {
                _dockWindowFactory = value;
                DockPanel.ReloadDockWindows();
            }
        }

        private IDockPaneCaptionFactory _dockPaneCaptionFactory = null;

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

        private IDockPaneStripFactory _dockPaneStripFactory = null;

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

        private IAutoHideStripFactory _autoHideStripFactory = null;

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
                DockPanel.ResetAutoHideStripControl();
            }
        }

        private IAutoHideWindowFactory _autoHideWindowFactory;

        public IAutoHideWindowFactory AutoHideWindowFactory
        {
            get { return _autoHideWindowFactory ?? (_autoHideWindowFactory = new DefaultAutoHideWindowFactory()); }
            set
            {
                if (DockPanel.Contents.Count > 0)
                {
                    throw new InvalidOperationException();
                }

                if (_autoHideWindowFactory == value)
                {
                    return;
                }

                _autoHideWindowFactory = value;
                DockPanel.ResetAutoHideStripWindow();
            }
        }

        private IPaneIndicatorFactory _paneIndicatorFactory;

        public IPaneIndicatorFactory PaneIndicatorFactory
        {
            get { return _paneIndicatorFactory ?? (_paneIndicatorFactory = new DefaultPaneIndicatorFactory()); }
            set { _paneIndicatorFactory = value; }
        }

        private IPanelIndicatorFactory _panelIndicatorFactory;

        public IPanelIndicatorFactory PanelIndicatorFactory
        {
            get { return _panelIndicatorFactory ?? (_panelIndicatorFactory = new DefaultPanelIndicatorFactory()); }
            set { _panelIndicatorFactory = value; }
        }

        private IDockOutlineFactory _dockOutlineFactory;

        public IDockOutlineFactory DockOutlineFactory
        {
            get { return _dockOutlineFactory ?? (_dockOutlineFactory = new DefaultDockOutlineFactory()); }
            set { _dockOutlineFactory = value; }
        }
    }
}
