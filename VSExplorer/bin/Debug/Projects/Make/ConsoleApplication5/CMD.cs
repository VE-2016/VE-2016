using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;

namespace MakeFiles
{
    public class CMD
    {
        private TextWriter outs { get; set; }
        private TextReader ins { get; set; }
        static private Process _process = null;

        private string folder { get; set; }

        static public bool running = false;

        public void runCommand(string cmd = "cmd.exe", string args = "-i")
        {
            
            _process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = cmd;
            if (args != "")
                startInfo.Arguments = args;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.CreateNoWindow = true;
            _process.StartInfo = startInfo;
            _process.Start();

            running = true;

			running = true;	
			
			running = true;	
			
			running = true;	

            Task.Run(new Action(() => { HandleStandardError(); }));

            Task.Run(new Action(() => { HandleStandardOutput(); }));

            _process.WaitForExit();

            running = false;

            
        }
        public string output = "";

        public bool handled = false;

        public string runCommandShell(string cmd = "cmd.exe", string args = "-i", bool window = false)
        {


            //while (true)
            {
                _process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = cmd;
                if (args != "-i")
                    startInfo.Arguments = args;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (window == false)
                {
                    startInfo.CreateNoWindow = true;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                }
                else
                {
                    startInfo.CreateNoWindow = false;
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                }
                _process.StartInfo = startInfo;
                _process.Start();


                Task.Run(new Action(() => { HandleStandardError(); }));

                Task.Run(new Action(() => { HandleStandardOutput(); }));


                //string s = _process.StandardOutput.ReadToEnd();

                //string e = _process.StandardError.ReadToEnd();


                running = true;

                _process.WaitForExit();

            }
            
            return "";

        }
        string ReplaceNonPrintableCharacters(string s, char replaceWith)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                byte b = (byte)c;
                if (b < 32 && b != '\n' && b != ' ')
                    result.Append(replaceWith);
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        public ArrayList R = new ArrayList();

        public void HandleStandardOutput()
        {
            int c = 100000;

            int i = 0;

            char[] buffer = new char[100000];

            int r = -1;

            while (r != 0)
            {
                //Task<int> output = _process.StandardOutput.ReadAsync(buffer, i, c);
                r = _process.StandardOutput.Read(buffer, i, c);
                //r = output.Result;

                //if (rb.IsDisposed == true)
                //    break;

                //rb.Invoke(new Action(() =>
                //{

                    byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(buffer);

                    string b = Encoding.UTF8.GetString(bytes);

                    //b = ReplaceNonPrintableCharacters(b, '\0');

                    //string b = new string(buffer);

                    b = b.Replace("\0", "");

                R.Add(b);

                output += b;

                //if (output.Trim().EndsWith("end"))
                if (output.Trim().Contains("end"))
                    handled = true;

                    //rb.AppendText(b);

                    //string []p = b.Split("\n".ToCharArray());

                    //string p = b.Substring(b.Length - 3);

                    //if (b.TrimEnd().EndsWith(">") == false) rb.AppendText("\n");
                //}));



                Array.Clear(buffer, 0, buffer.Length);

                Array.Clear(bytes, 0, bytes.Length);
            }
        }

        public void HandleStandardError()
        {
            int c = 100000;

            int i = 0;

            char[] buffer = new char[100000];

            int r = -1;

            while (r != 0)
            {
                Task<int> outputs = _process.StandardError.ReadAsync(buffer, i, c);
                // r = _process.StandardError.Read(buffer, i, c);
                r = outputs.Result;

                //if (rb.IsDisposed == true)
                //    break;

                //rb.Invoke(new Action(() =>
                //{

                    byte[] bytes = Encoding.UTF8.GetBytes(buffer);

                    string b = Encoding.UTF8.GetString(bytes);

                    //b = ReplaceNonPrintableCharacters(b, '\0');

                    //string b = new string(buffer);

                    b = b.Replace("\0", "");

                output += b;

                    R.Add(b);

               

                    //rb.AppendText(b);

                    //string []p = b.Split("\n".ToCharArray());

                    //string p = b.Substring(b.Length - 3);

                    //if (b.TrimEnd().EndsWith(">") == false) rb.AppendText("\n");
                //}));

                // process.StandardInput.Flush();

                Array.Clear(buffer, 0, buffer.Length);

                Array.Clear(bytes, 0, bytes.Length);
            }

        }

        public void EndProcess()
        {
            if (running == false)
                return;

            _process.Close();

            _process.Dispose();

            running = false;
        }

        public void RunAndInit(string args)
        {
            runCommand("cmd.exe", args);
        }
        public void Run(string args)
        {
            runCommand(args);
        }

        public void SetFolder(string s)
        {
            string c = Path.GetDirectoryName(s);

            string d = Path.GetPathRoot(c);

            SendCommand("cd /d " + d);

            SendCommand("cd " + c + "\n");
        }


        public void runCommands(string c)
        {
            start s = Run;
            s.BeginInvoke(c, null, null);
        }
        public void runAndInit(string c)
        {
            start s = RunAndInit;
            s.BeginInvoke(c, null, null);
            // SetFolder(folder);

        }
        public void runCommandsExternal(string c)
        {
            start s = Run;
            s.BeginInvoke(c, null, null);
        }
        public delegate void start(string c);

       // public RichTextBox rb { get; set; }


        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static private object _obs = new object();

        public void SendCommand(string c)
        {
            lock (_obs)
            {
                int i = R.Count;
                _process.StandardInput.Write(c + "\n");
                while (R.Count == i) ;
               

            }
        }
    }

    public enum AppState
    {
        Undefined = 0,
        Running = 1,
        Exiting = 2,
        Exited = 3,
    }

    public static class Win32
    {
        public enum CtrlType
        {
            CtrlCEvent = 0,
            CtrlBreakEvent = 1,
            CtrlCloseEvent = 2,
            CtrlLogoffEvent = 5,
            CtrlShutdownEvent = 6
        }

        public delegate bool ConsoleCtrlEventHandler(CtrlType ctrlType);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCtrlHandler(ConsoleCtrlEventHandler handler, bool add);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool GenerateConsoleCtrlEvent(CtrlType ctrlType, int dwProcessGroupId);

        /// <summary>
        /// allocates a new console for the calling process.
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. 
        /// To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool AllocConsole();

        /// <summary>
        /// Detaches the calling process from its console
        /// </summary>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. 
        /// To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32", SetLastError = true)]
        public static extern bool FreeConsole();

        /// <summary>
        /// Attaches the calling process to the console of the specified process.
        /// </summary>
        /// <param name="dwProcessId">[in] Identifier of the process, usually will be ATTACH_PARENT_PROCESS</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. 
        /// To get extended error information, call Marshal.GetLastWin32Error.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwProcessId);
    }


    public class ConsoleApp
    {
        private Process _process;
        private AutoResetEvent _processEvent;
        private ConcurrentQueue<ConsoleOutputEventArgs> _consoleOutputQueue;

        private Task _processMonitor;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly object _stateLock = new object();
        private readonly Win32.ConsoleCtrlEventHandler _consoleCtrlEventHandler;

        /// <summary>
        /// ConsoleApp constructor
        /// </summary>
        /// <param name="fileName">File name or DOS command</param>
        /// <param name="cmdLine">Command-line arguments</param>
        public ConsoleApp(string fileName, string cmdLine)
        {
            FileName = fileName;
            CmdLine = cmdLine;

            _consoleCtrlEventHandler += ConsoleCtrlHandler;
        }

        /// <summary>
        /// File name of the app, e.g. "cmd.exe"
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Command line of the app, e.g. "/c dir"
        /// </summary>
        public string CmdLine { get; private set; }

        /// <summary>
        /// Current state of the app.
        /// </summary>
        public AppState State { get; private set; }

        /// <summary>
        /// Exit code of the app.
        /// </summary>
        public int? ExitCode { get; private set; }

        /// <summary>
        /// Time the app has exited.
        /// </summary>
        public DateTime? ExitTime { get; private set; }

        /// <summary>
        /// Start the app.
        /// </summary>
        public void Run()
        {
            ThrowIfDisposed();

            lock (_stateLock)
            {
                if (State == AppState.Running)
                    throw new InvalidOperationException("App is already running.");

                if (State == AppState.Exiting)
                    throw new InvalidOperationException("App is exiting.");

                StartProcessAsync();

                State = AppState.Running;
            }
        }

        /// <summary>
        /// Stop the app.
        /// </summary>
        /// <param name="closeKey">Special key to send to close the app [default=Ctrl-C]</param>
        /// <param name="forceCloseMillisecondsTimeout">Timeout to wait before closing the app forcefully [default=infinite]</param>
        public void Stop(ConsoleSpecialKey closeKey = ConsoleSpecialKey.ControlC, int forceCloseMillisecondsTimeout = Timeout.Infinite)
        {
            ThrowIfDisposed();

            lock (_stateLock)
            {
                if (State == AppState.Undefined || State == AppState.Exited)
                    throw new InvalidOperationException("App is not running.");

                if (State == AppState.Exiting)
                    throw new InvalidOperationException("App is already exiting.");

                State = AppState.Exiting;

                Task.Factory.StartNew(() => CloseConsole(closeKey, forceCloseMillisecondsTimeout),
                    TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Wait until the app exits.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout to wait until the app is exited [default=infinite]</param>
        /// <returns>True if exited or False if timeout elapsed</returns>
        public bool WaitForExit(int millisecondsTimeout = Timeout.Infinite)
        {
            ThrowIfDisposed();

            if (State == AppState.Undefined || _processMonitor == null)
            {
                Trace.TraceWarning("App hasn't started yet");
                return true;
            }

            if (_processMonitor.IsCompleted)
                return true;

            Trace.TraceInformation("Waiting for app exit: Timeout={0}", millisecondsTimeout);
            return _processMonitor.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Fires when the app exits.
        /// </summary>
        public event EventHandler<EventArgs> Exited;

        /// <summary>
        /// Fires when the console outputs a new line or error.
        /// </summary>
        /// <remarks>The lines are queued and guaranteed to follow the output order</remarks>
        public event EventHandler<ConsoleOutputEventArgs> ConsoleOutput;

        private void StartProcessAsync()
        {
            _process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = FileName,
                    Arguments = CmdLine,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                },
            };

            try
            {
                Trace.TraceInformation("Starting app: '{0} {1}'", FileName, CmdLine);

                _processEvent = new AutoResetEvent(false);
                _consoleOutputQueue = new ConcurrentQueue<ConsoleOutputEventArgs>();

                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                _processMonitor = new Task(MonitoringHandler, cancellationToken, TaskCreationOptions.LongRunning);
                _processMonitor.Start();

                _process.OutputDataReceived += OnOutputLineReceived;
                _process.ErrorDataReceived += OnErrorLineReceived;
                _process.Exited += OnProcessExited;

                _process.Start();

                while (true)
                {
                    _process.BeginErrorReadLine();
                    _process.BeginOutputReadLine();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not start app: '{0} {1}', Error={2}", FileName, CmdLine, ex);

                FreeProcessResources();
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _processMonitor = null;
                }
                throw;
            }
        }

        private void MonitoringHandler(object obj)
        {
            var cancellationToken = (CancellationToken)obj;
            var supportedEvents = new[] { _processEvent, cancellationToken.WaitHandle };

            while (!cancellationToken.IsCancellationRequested)
            {
                WaitHandle.WaitAny(supportedEvents);

                // always dispatch output is case more than one event becomes signaled
                DispatchProcessOutput();
            }

            HandleProcessExit();
        }

        private void DispatchProcessOutput()
        {
            ConsoleOutputEventArgs args;
            while (_consoleOutputQueue.TryDequeue(out args))
            {
                try
                {
                    OnConsoleOutput(args);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("OnConsoleOutput exception ignored: Error={0}", ex.ToString());
                }
            }
        }

        private void HandleProcessExit()
        {
            if (_process == null)
                return;

            lock (_stateLock)
            {
                ExitCode = _process.ExitCode;
                ExitTime = _process.ExitTime;

                FreeProcessResources();
                State = AppState.Exited;
            }
            try
            {
                OnExited(new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.TraceError("OnExited exception ignored: Error={0}", ex.ToString());
            }
        }

        private void CloseConsole(ConsoleSpecialKey closeKey, int forceCloseMillisecondsTimeout)
        {
            if (_process == null || _process.HasExited)
                return;

            Trace.TraceInformation("Closing app input by sending Ctrl-Z signal");
            _process.StandardInput.Close();

            if (_process == null || _process.HasExited)
                return;

            Trace.TraceInformation("Trying to close the app gracefully by sending " + closeKey);
            Win32.AttachConsole((uint)_process.Id);
            Win32.SetConsoleCtrlHandler(_consoleCtrlEventHandler, true);
            var ctrlType = closeKey == ConsoleSpecialKey.ControlC ? Win32.CtrlType.CtrlCEvent : Win32.CtrlType.CtrlBreakEvent;
            Win32.GenerateConsoleCtrlEvent(ctrlType, 0);

            if (_process == null || _process.HasExited)
                return;

            // close console forcefully if not finished within allowed timeout
            Trace.TraceInformation("Waiting for voluntary app exit: Timeout={0}", forceCloseMillisecondsTimeout);
            var exited = _process.WaitForExit(forceCloseMillisecondsTimeout);
            if (!exited)
            {
                Trace.TraceWarning("Closing the app forcefully");
                _process.Kill();
            }
        }

        private static bool ConsoleCtrlHandler(Win32.CtrlType ctrlType)
        {
            const bool ignore = true;
            return ignore;
        }

        private void FreeProcessResources()
        {
            if (_process != null)
            {
                _process.OutputDataReceived -= OnOutputLineReceived;
                _process.ErrorDataReceived -= OnErrorLineReceived;
                _process.Exited -= OnProcessExited;

                _process.Close();
                _process.Dispose();
                _process = null;
            }
        }

        private void OnOutputLineReceived(object sender, DataReceivedEventArgs e)
        {
            _consoleOutputQueue.Enqueue(new ConsoleOutputEventArgs(e.Data, false));
            _processEvent.Set();
        }

        private void OnErrorLineReceived(object sender, DataReceivedEventArgs e)
        {
            _consoleOutputQueue.Enqueue(new ConsoleOutputEventArgs(e.Data, true));
            _processEvent.Set();
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _processEvent.Set();
        }

        protected virtual void OnConsoleOutput(ConsoleOutputEventArgs e)
        {
            if (e.Line == null)
                return;

            Trace.TraceInformation("{0}> {1}", e.IsError ? "stderr" : "stdout", e.Line);

            var handler = ConsoleOutput;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnExited(EventArgs e)
        {
            Trace.TraceInformation("App exited: '{0} {1}', ExitCode={2}, ExitTime='{3}'",
                FileName, CmdLine, ExitCode, ExitTime);

            var handler = Exited;
            if (handler != null)
                handler(this, e);
        }


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseConsole(ConsoleSpecialKey.ControlBreak, 500);
                    WaitForExit(500);
                    FreeProcessResources();
                }

                _disposed = true;
            }
        }

        ~ConsoleApp()
        {
            Dispose(false);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");
        }

        #endregion


        #region Static utility methods

        public class Result
        {
            public int ExitCode;
            public string Output;
        }

        /// <summary>
        /// Run console app synchronously and capture all its output including standard error stream.
        /// </summary>
        /// <param name="fileName">File name or DOS command</param>
        /// <param name="cmdLine">Command-line arguments</param>
        /// <returns>Execution result</returns>
        public static Result Run(string fileName, string cmdLine = null)
        {
            var app = new ConsoleApp(fileName, cmdLine);
            {
                StringBuilder outputStringBuilder = new StringBuilder();
                app.ConsoleOutput += (o, args) => outputStringBuilder.AppendLine(args.Line);

                app.Run();
                app.WaitForExit();

                var result = new Result
                {
                    ExitCode = app.ExitCode.GetValueOrDefault(),
                    Output = outputStringBuilder.ToString()
                };

                return result;
            }
        }

        #endregion
    }
    public class ConsoleOutputEventArgs : EventArgs
    {
        public ConsoleOutputEventArgs(string line, bool isError)
        {
            Line = line;
            IsError = isError;
        }

        public string Line { get; private set; }
        public bool IsError { get; private set; }
    }

    public class Navigators
    {
        public ArrayList N { get; set; }

        public int act = -1;

        public int maxsize = 1000;

        public Navigators()
        {
            N = new ArrayList();
        }

        public void Add(string c)
        {
            N.Add(c);
            act++;
            if (act > maxsize)
            {
                N.RemoveAt(0);
                act--;
            }
        }

        public void Sort()
        {
            act = -1;
            N.Sort();
        }

        public string Find(string s)
        {
            foreach (string b in N)
                if (b.StartsWith(s) == true)
                    return b;
            return "";
        }

        private ArrayList S { get; set; }

        public string Get(string c)
        {
            if (c == "")
            {
                if (act >= N.Count - 1)
                    act = -1;

                return Next();
            }

            return Find(c);
        }
        public string Prev()
        {
            if (N == null)
                return "";

            if (act <= 0)
                return "";

            act--;

            string c = N[act] as string;

            return c;
        }
        public string Next()
        {
            if (N == null)
                return "";
            //if (act <= 0)
            //    return "";

            act++;

            if (act >= N.Count)
                act = N.Count - 1;

            if (act < 0)
                act = 0;

            if (act >= N.Count)
                return "";

            string s = N[act] as string;

            return s;
        }
    }
    public class VBoxKeyboardHelper
    {
        //public IKeyboard Keyboard { get; private set; }

        //public VBoxKeyboardHelper(IKeyboard kb)
        //{
        //    Keyboard = kb;
        //}

        //public void SendKeyCode(Keys keycode, bool down)
        //{
        //    int code;
        //    //Console.WriteLine("VBoxKeyboardHelper.SendKeyCode({1}={2},{0})", down, keycode, (int)keycode);
        //    int[] codes_az = { 0x1e, 0x30, 0x2e, 0x20, 0x12, 0x21, 0x22, 0x23, 0x17, 0x24, 0x25, 0x26, 0x32, 0x31, 0x18, 0x19, 0x10, 0x13, 0x1f, 0x14, 0x16, 0x2f, 0x11, 0x2d, 0x15, 0x2c }; //a-z
        //    int[] codes_num = { 0x0b, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a }; //0123456789
        //    int[] codes_Fkeys = { 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44, 0x57, 0x58 }; //F123456789,10,11,12
        //    if (keycode >= Keys.A && keycode <= Keys.Z)
        //    {
        //        code = codes_az[keycode - Keys.A];
        //    }
        //    else if (keycode >= Keys.D0 && keycode <= Keys.D9)
        //    {
        //        code = codes_num[keycode - Keys.D0];
        //    }
        //    else if (keycode >= Keys.F1 && keycode <= Keys.F12)
        //    {
        //        code = codes_Fkeys[keycode - Keys.F1];
        //    }
        //    else
        //    {
        //        switch (keycode)
        //        {
        //            case Keys.Space: code = 0x0239; break;
        //            case Keys.Left: code = 0xe04b; break;
        //            case Keys.Up: code = 0xe048; break;
        //            case Keys.Right: code = 0xe04d; break;
        //            case Keys.Down: code = 0xe050; break;
        //            case Keys.Enter: code = 0x1c; break;
        //            case Keys.LShiftKey:
        //            case Keys.RShiftKey:
        //            case Keys.ShiftKey: code = 0x2a; break;
        //            case Keys.Tab: code = 0x0f; break;
        //            case Keys.Back: code = 0x0e; break;
        //            case Keys.Escape: code = 0x01; break;
        //            case Keys.Home: code = 0xe047; break;
        //            case Keys.PageUp: code = 0xe049; break;
        //            case Keys.PageDown: code = 0xe051; break;
        //            case Keys.End: code = 0xe04f; break;
        //            case Keys.Insert: code = 0xe052; break;
        //            case Keys.ControlKey:
        //            case Keys.LControlKey: code = 0x1d; break;
        //            case Keys.RControlKey: code = 0xe01d; break;
        //            case Keys.Menu:
        //            case Keys.LMenu: code = 0x38; break; //Left Alt
        //            case Keys.RMenu: code = 0xe038; break; //Right Alt
        //            case Keys.LWin: code = 0xe05b; break; //Left windows key
        //            case Keys.RWin: code = 0xe05c; break; //Right windows key
        //            case Keys.Delete: code = 0xe053; break;
        //            case Keys.OemQuotes: code = 0x28; break;
        //            case Keys.OemQuestion: code = 0x35; break;
        //            case Keys.OemPeriod: code = 0x34; break;
        //            case Keys.OemMinus: code = 0x0c; break;
        //            case Keys.Oemplus: code = 0x0d; break;
        //            case Keys.Oemcomma: code = 0x33; break;
        //            case Keys.OemSemicolon: code = 0x27; break;
        //            case Keys.Oemtilde: code = 0x29; break;
        //            case Keys.OemCloseBrackets: code = 0x1b; break;
        //            case Keys.OemBackslash: code = 0x2b; break;
        //            case Keys.OemOpenBrackets: code = 0x1a; break;
        //            default:
        //                Console.Error.WriteLine("VBoxKeyboardHelper.SendKeyCode({1}={2},{0}) unknown key", down, keycode, (int)keycode);
        //                return;
        //        }
        //    }
        //    if ((code & 0xff00) != 0) kbdPutCode((code >> 8) & 0xff);
        //    kbdPutCode((code & 0xff) | (down ? 0 : 0x80));
        //}
        private void kbdPutCode(int code)
        {
            try
            {
                //Keyboard.PutScancode(code);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("VBoxKeyboardHelper.PutScancode Exception: {0}", ex.ToString());
            }
        }
    }
}


