using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace AIMS.Libraries.CodeEditor.Core.Timers
{
    #region WeakTimer
    [ToolboxItem(true)]
    public class WeakTimer : Component
    {
        private int _TickCount = 0;

        #region public property Interval


        //member variable
        private int _Interval = 100;

        public int Interval
        {
            get { return _Interval; }

            set { _Interval = value; }
        }

        #endregion //END PROPERTY Interval

        #region public property Enabled


        //member variable
        private bool _Enabled = true;

        public bool Enabled
        {
            get { return _Enabled; }

            set { _Enabled = value; }
        }

        #endregion //END PROPERTY Enabled

        public event EventHandler Tick = null;
        private Container _components = null;

        public WeakTimer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            Init();
        }

        public WeakTimer()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            WeakTimerHelper.Attach(this);
        }

        public void DoTick(object s, EventArgs e)
        {
            _TickCount++;
            if (Enabled)
            {
                if (_TickCount >= (this.Interval / WeakTimerHelper.Speed))
                {
                    _TickCount = 0;

                    try
                    {
                        if (Tick != null)
                            Tick(s, e);
                    }
                    catch
                    {
                    }
                }
            }
            //	GC.KeepAlive(Tick.Target);
            //	GC.KeepAlive(this);
        }

        ~WeakTimer()
        {
            //Console.WriteLine ("killing WeakTimer");
            WeakTimerHelper.Detach(this);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
        }

        #endregion
    }

    #endregion

    #region Helper

    public class WeakTimerHelper
    {
        public static Timer _timer = GetTimer();
        private static ArrayList s_listeners = new ArrayList();
        public const int Speed = 10;


        public static void Attach(WeakTimer t)
        {
            return;

            WeakReference wr = new WeakReference(t);

            s_listeners.Add(wr);
        }

        public static void Detach(WeakTimer t)
        {
            for (int i = s_listeners.Count - 1; i >= 0; i--)
            {
                WeakReference wr = (WeakReference)s_listeners[i];
                try
                {
                    if (wr.Target == t)
                        s_listeners.RemoveAt(i);
                }
                catch
                {
                }
            }
        }

        private static Timer GetTimer()
        {
            Timer t = new Timer();
            t.Interval = Speed;
            t.Tick += new EventHandler(DoTick);
            t.Enabled = true;
            return t;
        }

        private static void DoTick(object s, EventArgs e)
        {
            return;
            foreach (WeakReference wr in s_listeners)
            {
                if (wr.IsAlive)
                {
                    try
                    {
                        ((WeakTimer)wr.Target).DoTick(null, EventArgs.Empty);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    #endregion
}