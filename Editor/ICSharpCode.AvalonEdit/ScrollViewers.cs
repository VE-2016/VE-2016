using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit
{
    /// <summary>
    ///
    /// </summary>
    public class ScrollViewers : ScrollViewer
    {
        /// <summary>
        ///
        /// </summary>
        public ScrollViewers() : base()
        {
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