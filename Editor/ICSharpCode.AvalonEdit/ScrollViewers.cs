using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            } else
            {
                ScrollToVerticalOffset(cs.VerticalOffset);
                cs = null;
            }
        }

        ScrollChangedEventArgs cs = null;

        DispatcherTimer timer = new DispatcherTimer();
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

            } else
            {
                timer.IsEnabled = true;
                timer.Start();

                cs = null;

                e.Handled = false;
            }
        }
    }
}
