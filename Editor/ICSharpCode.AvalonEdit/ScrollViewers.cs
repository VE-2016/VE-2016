using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit
{
    /// <summary>
    ///
    /// </summary>
    public partial class ScrollViewers : ScrollViewer
    {
        /// <summary>
        ///
        /// </summary>
        public ScrollViewers() : base()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (cs == null)
            {
                timer.IsEnabled = false;
                timer.Stop();
            }
            else
            {
                ScrollToVerticalOffset(cs.VerticalOffset);
                cs = null;
            }
        }

        public event EventHandler<ScrollChangedEventArgs> handler;

        private ScrollChangedEventArgs cs = null;

        private DispatcherTimer timer = new DispatcherTimer();

        public ComboBox zoom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            button = (Image)Template.FindName("button", this);
            panel_bottom = (DockPanel)Template.FindName("panel_bottom", this);
            zoom = (ComboBox)Template.FindName("zoom", this);

        }
        /// <summary>
        /// 
        /// </summary>
        public DockPanel panel_bottom { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Image button { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        public void MakeSplitterButton(bool visible = true)
        {
            if (!visible)
            {
                panel_bottom.Children.Remove(button);
            }
            else if (!panel_bottom.Children.Contains(button))
            {
                DockPanel.SetDock(button, Dock.Top);
                panel_bottom.Children.Insert(0, button);
            }
            //		this.InvalidateVisual();
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnScrollChanged(ScrollChangedEventArgs e)
        {
            if (timer.IsEnabled == true)
            {
                cs = e;

                e.Handled = true;
            }
            else
            {
                timer.IsEnabled = true;
                timer.Start();

                cs = null;

                e.Handled = false;
            }
            if (handler != null)
                handler(this, e);
        }
    }
}