
namespace WinExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using PowerShell = System.Management.Automation.PowerShell;
    using System.Threading.Tasks;
    using System.Collections;
    /// <summary>
    /// This sample shows how to implement a basic read-evaluate-print 
    /// loop (or 'listener') that allowing you to interactively work 
    /// with the Windows PowerShell engine.
    /// </summary>
    public class PSListenerConsoleSample
    {
        /// <summary>
        /// Used to read user input.
        /// </summary>
        public ConsoleReadLine consoleReadLine { get; set; }




        /// <summary>
        /// Holds a reference to the runspace for this interpeter.
        /// </summary>
        internal Runspace myRunSpace;

        /// <summary>
        /// Indicator to tell the host application that it should exit.
        /// </summary>
        private bool _shouldExit;

        /// <summary>
        /// The exit code that the host application will use to exit.
        /// </summary>
        private int _exitCode;

        /// <summary>
        /// Holds a reference to the PSHost implementation for this interpreter.
        /// </summary>
        private MyHost _myHost;

        /// <summary>
        /// Holds a reference to the currently executing pipeline so that it can be
        /// stopped by the control-C handler.
        /// </summary>
        private PowerShell _currentPowerShell;

        /// <summary>
        /// Used to serialize access to instance data.
        /// </summary>
        private object _instanceLock = new object();

        /// <summary>
        /// Gets or sets a value indicating whether the host application 
        /// should exit.
        /// </summary>
        public bool ShouldExit
        {
            get { return _shouldExit; }
            set { _shouldExit = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the host application 
        /// should exit.
        /// </summary>
        public int ExitCode
        {
            get { return _exitCode; }
            set { _exitCode = value; }
        }

        public ConsoleControls console { get; set; }

        /// <summary>
        /// Creates and initiates the listener instance.
        /// </summary>
        /// <param name="args">This parameter is not used.</param>
        public void start(string args)
        {
            // console = new ConsoleControls();

            consoleReadLine = new ConsoleReadLine(console);

            // Display the welcome message.
            //console.Title = "PowerShell Console Host Sample Application";
            //ConsoleColor oldFg = console.ForeColor;
            //console.ForeColor = Color.Cyan;
            console.WriteLine("    Windows PowerShell Console Host Application Sample");
            console.WriteLine("    ==================================================");
            console.WriteLine(string.Empty);
            console.WriteLine("This is an example of a simple interactive console host uses ");
            console.WriteLine("the Windows PowerShell engine to interpret commands.");
            console.WriteLine("Type 'exit' to exit.");
            console.WriteLine(string.Empty);
            //console.ForeColor = oldFg;

            // Create the listener and run it. This method never returns.
            this.listener = new PSListenerConsoleSample(console);
            listener.Run(args);
        }

        private PSListenerConsoleSample listener { get; set; }
        /// <summary>
        /// Initializes a new instance of the PSListenerConsoleSample class.
        /// </summary>
        public PSListenerConsoleSample(ConsoleControls c)
        {
            // Create the host and runspace instances for this interpreter. 
            // Note that this application does not support console files so 
            // only the default snap-ins will be available.

            _myHost = new MyHost(this, c);
            this.myRunSpace = RunspaceFactory.CreateRunspace(_myHost);
            this.myRunSpace.Open();
            this.console = c;
            this.consoleReadLine = new ConsoleReadLine(c);

            // Create a PowerShell object to run the commands used to create 
            // $profile and load the profiles.
            lock (_instanceLock)
            {
                _currentPowerShell = PowerShell.Create();
            }

            try
            {
                _currentPowerShell.Runspace = this.myRunSpace;

                //PSCommand[] profileCommands = Microsoft.Samples.PowerShell.Host.HostUtilities.GetProfileCommands("SampleHost06");
                //foreach (PSCommand command in profileCommands)
                //{
                //    this.currentPowerShell.Commands = command;
                //    this.currentPowerShell.Invoke();
                //}
            }
            finally
            {
                // Dispose the PowerShell object and set currentPowerShell 
                // to null. It is locked because currentPowerShell may be 
                // accessed by the ctrl-C handler.
                lock (_instanceLock)
                {
                    _currentPowerShell.Dispose();
                    _currentPowerShell = null;
                }
            }
        }

        /// <summary>
        /// A helper class that builds and executes a pipeline that writes 
        /// to the default output path. Any exceptions that are thrown are 
        /// just passed to the caller. Since all output goes to the default 
        /// outter, this method does not return anything.
        /// </summary>
        /// <param name="cmd">The script to run.</param>
        /// <param name="input">Any input arguments to pass to the script. 
        /// If null then nothing is passed in.</param>
        private void executeHelper(string cmd, object input)
        {
            // Ignore empty command lines.
            if (String.IsNullOrEmpty(cmd))
            {
                return;
            }

            // Create the pipeline object and make it available to the
            // ctrl-C handle through the currentPowerShell instance
            // variable.
            lock (_instanceLock)
            {
                _currentPowerShell = PowerShell.Create();
            }

            // Add a script and command to the pipeline and then run the pipeline. Place 
            // the results in the currentPowerShell variable so that the pipeline can be 
            // stopped.
            try
            {
                _currentPowerShell.Runspace = this.myRunSpace;

                _currentPowerShell.AddScript(cmd);

                // Add the default outputter to the end of the pipe and then call the 
                // MergeMyResults method to merge the output and error streams from the 
                // pipeline. This will result in the output being written using the PSHost
                // and PSHostUserInterface classes instead of returning objects to the host
                // application.
                _currentPowerShell.AddCommand("out-default");
                _currentPowerShell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

                // If there is any input pass it in, otherwise just invoke the
                // the pipeline.
                if (input != null)
                {
                    _currentPowerShell.Invoke(new object[] { input });
                }
                else
                {
                    _currentPowerShell.Invoke();
                }
            }
            finally
            {
                // Dispose the PowerShell object and set currentPowerShell to null. 
                // It is locked because currentPowerShell may be accessed by the 
                // ctrl-C handler.
                lock (_instanceLock)
                {
                    _currentPowerShell.Dispose();
                    _currentPowerShell = null;
                }
            }
        }

        /// <summary>
        /// To display an exception using the display formatter, 
        /// run a second pipeline passing in the error record.
        /// The runtime will bind this to the $input variable,
        /// which is why $input is being piped to the Out-String
        /// cmdlet. The WriteErrorLine method is called to make sure 
        /// the error gets displayed in the correct error color.
        /// </summary>
        /// <param name="e">The exception to display.</param>
        private void ReportException(Exception e)
        {
            if (e != null)
            {
                object error;
                IContainsErrorRecord icer = e as IContainsErrorRecord;
                if (icer != null)
                {
                    error = icer.ErrorRecord;
                }
                else
                {
                    error = (object)new ErrorRecord(e, "Host.ReportException", ErrorCategory.NotSpecified, null);
                }

                lock (_instanceLock)
                {
                    _currentPowerShell = PowerShell.Create();
                }

                _currentPowerShell.Runspace = this.myRunSpace;

                try
                {
                    _currentPowerShell.AddScript("$input").AddCommand("out-string");

                    // Do not merge errors, this function will swallow errors.
                    Collection<PSObject> result;
                    PSDataCollection<object> inputCollection = new PSDataCollection<object>();
                    inputCollection.Add(error);
                    inputCollection.Complete();
                    result = _currentPowerShell.Invoke(inputCollection);

                    if (result.Count > 0)
                    {
                        string str = result[0].BaseObject as string;
                        if (!string.IsNullOrEmpty(str))
                        {
                            // Remove \r\n, which is added by the Out-String cmdlet.
                            _myHost.UI.WriteErrorLine(str.Substring(0, str.Length - 2));
                        }
                    }
                }
                finally
                {
                    // Dispose of the pipeline and set it to null, locking it  because 
                    // currentPowerShell may be accessed by the ctrl-C handler.
                    lock (_instanceLock)
                    {
                        _currentPowerShell.Dispose();
                        _currentPowerShell = null;
                    }
                }
            }
        }

        /// <summary>
        /// Basic script execution routine. Any runtime exceptions are
        /// caught and passed back to the Windows PowerShell engine to 
        /// display.
        /// </summary>
        /// <param name="cmd">Script to run.</param>
        private void Execute(string cmd)
        {
            try
            {
                // Run the command with no input.
                this.executeHelper(cmd, null);
            }
            catch (RuntimeException rte)
            {
                this.ReportException(rte);
            }
        }

        /// <summary>
        /// Method used to handle control-C's from the user. It calls the
        /// pipeline Stop() method to stop execution. If any exceptions occur
        /// they are printed to the console but otherwise ignored.
        /// </summary>
        /// <param name="sender">See sender property documentation of  
        /// ConsoleCancelEventHandler.</param>
        /// <param name="e">See e property documentation of 
        /// ConsoleCancelEventHandler.</param>
        private void HandleControlC(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                lock (_instanceLock)
                {
                    if (_currentPowerShell != null && _currentPowerShell.InvocationStateInfo.State == PSInvocationState.Running)
                    {
                        _currentPowerShell.Stop();
                    }
                }

                e.Cancel = true;
            }
            catch (Exception exception)
            {
                _myHost.UI.WriteErrorLine(exception.ToString());
            }
        }

        /// <summary>
        /// Implements the basic listener loop. It sets up the ctrl-C handler, then
        /// reads a command from the user, executes it and repeats until the ShouldExit
        /// flag is set.
        /// </summary>
        private void Run(string cmds = "")
        {
            if (cmds != "")
            {
                console.WritePrompt();
                this.Execute(cmds);
            }

            // Set up the control-C handler.
            // Console.CancelKeyPress += new ConsoleCancelEventHandler(this.HandleControlC);
            // Console.TreatControlCAsInput = false;

            // Read commands and run them until the ShouldExit flag is set by
            // the user calling "exit".
            while (!this.ShouldExit)
            {
                if (cmds == "")
                {
                    console.WriteLine("\n");
                    console.WritePrompt();
                }
                else cmds = "";

                Task.Delay(1800);

                string prompt;
                if (_myHost.IsRunspacePushed)
                {
                    prompt = string.Format("\n[{0}] PSConsoleSample: ", this.myRunSpace.ConnectionInfo.ComputerName);
                }
                else
                {
                    prompt = "\nPSConsoleSample: ";
                }

                //this.myHost.UI.Write(ConsoleColor.Cyan, ConsoleColor.Black, prompt);
                string cmd = console.ReadLine();
                this.Execute(cmd);
            }

            // Exit with the desired exit code that was set by the exit command.
            // The exit code is set in the host by the MyHost.SetShouldExit() method.
            //Environment.Exit(this.ExitCode);
        }
    }

    /// <summary>
    /// This class is used to read the command line and color the text as 
    /// it is entered. Tokens are determined using the PSParser.Tokenize
    /// method.
    /// </summary>
    public class ConsoleReadLine
    {
        /// <summary>
        /// The buffer used to edit.
        /// </summary>
        private StringBuilder _buffer = new StringBuilder();

        /// <summary>
        /// The position of the cursor within the buffer.
        /// </summary>
        private int _current;

        /// <summary>
        /// The count of characters in buffer rendered.
        /// </summary>
        private int _rendered;

        /// <summary>
        /// Store the anchor and handle cursor movement
        /// </summary>
        private Cursor _cursor;

        /// <summary>
        /// The array of colors for tokens, indexed by PSTokenType
        /// </summary>
        private ConsoleColor[] _tokenColors;

        /// <summary>
        /// We do not pick different colors for every token, those tokens
        /// use this default.
        /// </summary>
        private ConsoleColor _defaultColor = Console.ForegroundColor;

        public ConsoleControls console { get; set; }

        /// <summary>
        /// Initializes a new instance of the ConsoleReadLine class.
        /// </summary>
        public ConsoleReadLine(ConsoleControls c)
        {
            console = c;
            _tokenColors = new ConsoleColor[]
            {
        _defaultColor,       // Unknown
        ConsoleColor.Yellow,     // Command
        ConsoleColor.Green,      // CommandParameter
        ConsoleColor.Cyan,       // CommandArgument
        ConsoleColor.Cyan,       // Number
        ConsoleColor.Cyan,       // String
        ConsoleColor.Green,      // Variable
        _defaultColor,            // Member
        _defaultColor,            // LoopLabel
        ConsoleColor.DarkYellow, // Attribute
        ConsoleColor.DarkYellow, // Type
        ConsoleColor.DarkCyan,   // Operator
        _defaultColor,            // GroupStart
        _defaultColor,            // GroupEnd
        ConsoleColor.Magenta,    // Keyword
        ConsoleColor.Red,        // Comment
        ConsoleColor.DarkCyan,   // StatementSeparator
                _defaultColor,            // NewLine
                _defaultColor,            // LineContinuation
                _defaultColor,            // Position            
            };
        }

        /// <summary>
        /// Read a line of text, colorizing while typing.
        /// </summary>
        /// <returns>The command line read</returns>
        public string Read()
        {
            this.Initialize();

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        this.OnBackspace();
                        break;
                    case ConsoleKey.Delete:
                        this.OnDelete();
                        break;
                    case ConsoleKey.Enter:
                        return this.OnEnter();
                    case ConsoleKey.RightArrow:
                        this.OnRight(key.Modifiers);
                        break;
                    case ConsoleKey.LeftArrow:
                        this.OnLeft(key.Modifiers);
                        break;
                    case ConsoleKey.Escape:
                        this.OnEscape();
                        break;
                    case ConsoleKey.Home:
                        this.OnHome();
                        break;
                    case ConsoleKey.End:
                        this.OnEnd();
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftWindows:
                    case ConsoleKey.RightWindows:
                        // ignore these
                        continue;

                    default:
                        if (key.KeyChar == '\x0D')
                        {
                            goto case ConsoleKey.Enter;      // Ctrl-M
                        }

                        if (key.KeyChar == '\x08')
                        {
                            goto case ConsoleKey.Backspace;  // Ctrl-H
                        }

                        this.Insert(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes the buffer.
        /// </summary>
        private void Initialize()
        {
            _buffer.Length = 0;
            _current = 0;
            _rendered = 0;
            _cursor = new Cursor();
        }

        /// <summary>
        /// Inserts a key.
        /// </summary>
        /// <param name="key">The key to insert.</param>
        private void Insert(ConsoleKeyInfo key)
        {
            _buffer.Insert(_current, key.KeyChar);
            _current++;
            this.Render();
        }

        /// <summary>
        /// The End key was enetered..
        /// </summary>
        private void OnEnd()
        {
            _current = _buffer.Length;
            _cursor.Place(_rendered);
        }

        /// <summary>
        /// The Home key was eneterd.
        /// </summary>
        private void OnHome()
        {
            _current = 0;
            _cursor.Reset();
        }

        /// <summary>
        /// The Escape key was enetered.
        /// </summary>
        private void OnEscape()
        {
            _buffer.Length = 0;
            _current = 0;
            this.Render();
        }

        /// <summary>
        /// Moves to the left of the cursor position.
        /// </summary>
        /// <param name="consoleModifiers">Enumeration for Alt, Control, 
        /// and Shift keys.</param>
        private void OnLeft(ConsoleModifiers consoleModifiers)
        {
            if ((consoleModifiers & ConsoleModifiers.Control) != 0)
            {
                // Move back to the start of the previous word.
                if (_buffer.Length > 0 && _current != 0)
                {
                    bool nonLetter = IsSeperator(_buffer[_current - 1]);
                    while (_current > 0 && (_current - 1 < _buffer.Length))
                    {
                        this.MoveLeft();

                        if (IsSeperator(_buffer[_current]) != nonLetter)
                        {
                            if (!nonLetter)
                            {
                                this.MoveRight();
                                break;
                            }

                            nonLetter = false;
                        }
                    }
                }
            }
            else
            {
                this.MoveLeft();
            }
        }

        /// <summary>
        /// Determines if a character is a seperator.
        /// </summary>
        /// <param name="ch">Character to investigate.</param>
        /// <returns>A value that indicates whether the character 
        /// is a seperator.</returns>
        private static bool IsSeperator(char ch)
        {
            return !Char.IsLetter(ch);
        }

        /// <summary>
        /// Moves to what is to the right of the cursor position.
        /// </summary>
        /// <param name="consoleModifiers">Enumeration for Alt, Control, 
        /// and Shift keys.</param>
        private void OnRight(ConsoleModifiers consoleModifiers)
        {
            if ((consoleModifiers & ConsoleModifiers.Control) != 0)
            {
                // Move to the next word.
                if (_buffer.Length != 0 && _current < _buffer.Length)
                {
                    bool nonLetter = IsSeperator(_buffer[_current]);
                    while (_current < _buffer.Length)
                    {
                        this.MoveRight();

                        if (_current == _buffer.Length)
                        {
                            break;
                        }

                        if (IsSeperator(_buffer[_current]) != nonLetter)
                        {
                            if (nonLetter)
                            {
                                break;
                            }

                            nonLetter = true;
                        }
                    }
                }
            }
            else
            {
                this.MoveRight();
            }
        }

        /// <summary>
        /// Moves the cursor one character to the right.
        /// </summary>
        private void MoveRight()
        {
            if (_current < _buffer.Length)
            {
                char c = _buffer[_current];
                _current++;
                Cursor.Move(1);
            }
        }

        /// <summary>
        /// Moves the cursor one character to the left.
        /// </summary>
        private void MoveLeft()
        {
            if (_current > 0 && (_current - 1 < _buffer.Length))
            {
                _current--;
                char c = _buffer[_current];
                Cursor.Move(-1);
            }
        }

        /// <summary>
        /// The Enter key was entered.
        /// </summary>
        /// <returns>A newline character.</returns>
        private string OnEnter()
        {
            console.WriteLine("\n");
            return _buffer.ToString();
        }

        /// <summary>
        /// The delete key was entered.
        /// </summary>
        private void OnDelete()
        {
            if (_buffer.Length > 0 && _current < _buffer.Length)
            {
                _buffer.Remove(_current, 1);
                this.Render();
            }
        }

        /// <summary>
        /// The Backspace key was entered.
        /// </summary>
        private void OnBackspace()
        {
            if (_buffer.Length > 0 && _current > 0)
            {
                _buffer.Remove(_current - 1, 1);
                _current--;
                this.Render();
            }
        }

        /// <summary>
        /// Displays the line.
        /// </summary>
        private void Render()
        {
            string text = _buffer.ToString();

            // The PowerShell tokenizer is used to decide how to colorize
            // the input.  Any errors in the input are returned in 'errors',
            // but we won't be looking at those here.
            Collection<PSParseError> errors = null;
            Collection<PSToken> tokens = PSParser.Tokenize(text, out errors);

            if (tokens.Count > 0)
            {
                // We can skip rendering tokens that end before the cursor.
                int i;
                for (i = 0; i < tokens.Count; ++i)
                {
                    if (_current >= tokens[i].Start)
                    {
                        break;
                    }
                }

                // Place the cursor at the start of the first token to render.  The
                // last edit may require changes to the colorization of characters
                // preceding the cursor.
                _cursor.Place(tokens[i].Start);

                for (; i < tokens.Count; ++i)
                {
                    // Write out the token.  We don't use tokens[i].Content, instead we
                    // use the actual text from our input because the content sometimes
                    // excludes part of the token, e.g. the quote characters of a string.
                    //console.ForegroundColor = this.tokenColors[(int)tokens[i].Type];
                    console.WriteLine(text.Substring(tokens[i].Start, tokens[i].Length));

                    // Whitespace doesn't show up in the array of tokens.  Write it out here.
                    if (i != (tokens.Count - 1))
                    {
                        Console.ForegroundColor = _defaultColor;
                        for (int j = (tokens[i].Start + tokens[i].Length); j < tokens[i + 1].Start; ++j)
                        {
                            console.WriteLine(text[j]);
                        }
                    }
                }

                // It's possible there is text left over to output.  This happens when there is
                // some error during tokenization, e.g. a string literal is missing a closing quote.
                Console.ForegroundColor = _defaultColor;
                for (int j = tokens[i - 1].Start + tokens[i - 1].Length; j < text.Length; ++j)
                {
                    console.WriteLine(text[j]);
                }
            }
            else
            {
                // If tokenization completely failed, just redraw the whole line.  This
                // happens most frequently when the first token is incomplete, like a string
                // literal missing a closing quote.
                _cursor.Reset();
                console.WriteLine(text);
            }

            // If characters were deleted, we must write over previously written characters
            if (text.Length < _rendered)
            {
                console.WriteLine(new string(' ', _rendered - text.Length));
            }

            _rendered = text.Length;
            _cursor.Place(_current);
        }

        /// <summary>
        /// A helper class for maintaining the cursor while editing the command line.
        /// </summary>
        internal class Cursor
        {
            /// <summary>
            /// The top anchor for reposition the cursor.
            /// </summary>
            private int _anchorTop;

            /// <summary>
            /// The left anchor for repositioning the cursor.
            /// </summary>
            private int _anchorLeft;

            /// <summary>
            /// Initializes a new instance of the Cursor class.
            /// </summary>
            public Cursor()
            {
                //this.anchorTop = Console.CursorTop;
                //this.anchorLeft = Console.CursorLeft;
            }

            /// <summary>
            /// Moves the cursor.
            /// </summary>
            /// <param name="delta">The number of characters to move.</param>
            internal static void Move(int delta)
            {
                int position = Console.CursorTop * Console.BufferWidth + Console.CursorLeft + delta;

                Console.CursorLeft = position % Console.BufferWidth;
                Console.CursorTop = position / Console.BufferWidth;
            }

            /// <summary>
            /// Resets the cursor position.
            /// </summary>
            internal void Reset()
            {
                Console.CursorTop = _anchorTop;
                Console.CursorLeft = _anchorLeft;
            }

            /// <summary>
            /// Moves the cursor to a specific position.
            /// </summary>
            /// <param name="position">The new position.</param>
            internal void Place(int position)
            {
                Console.CursorLeft = (_anchorLeft + position) % Console.BufferWidth;
                int cursorTop = _anchorTop + (_anchorLeft + position) / Console.BufferWidth;
                if (cursorTop >= Console.BufferHeight)
                {
                    _anchorTop -= cursorTop - Console.BufferHeight + 1;
                    cursorTop = Console.BufferHeight - 1;
                }

                Console.CursorTop = cursorTop;
            }
        } // End Cursor
    }
    /// <summary>
    /// This is a sample implementation of the PSHost abstract class for 
    /// console applications. Not all members are implemented. Those that 
    /// are not implemented throw a NotImplementedException exception or 
    /// return nothing.
    /// </summary>
    internal class MyHost : PSHost, IHostSupportsInteractiveSession
    {
        public MyHost(PSListenerConsoleSample program, ConsoleControls c)
        {
            _program = program;
            myHostUserInterface = new MyHostUserInterface(c);
        }

        /// <summary>
        /// A reference to the PSHost implementation.
        /// </summary>
        private PSListenerConsoleSample _program;

        /// <summary>
        /// The culture information of the thread that created
        /// this object.
        /// </summary>
        private CultureInfo _originalCultureInfo =
            System.Threading.Thread.CurrentThread.CurrentCulture;

        /// <summary>
        /// The UI culture information of the thread that created
        /// this object.
        /// </summary>
        private CultureInfo _originalUICultureInfo =
            System.Threading.Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// The identifier of this PSHost implementation.
        /// </summary>
        private static Guid s_instanceId = Guid.NewGuid();

        /// <summary>
        /// A reference to the implementation of the PSHostUserInterface
        /// class for this application.
        /// </summary>
        private MyHostUserInterface myHostUserInterface { get; set; }



        /// <summary>
        /// A reference to the runspace used to start an interactive session.
        /// </summary>
        public Runspace pushedRunspace = null;


        /// <summary>
        /// Gets the culture information to use. This implementation 
        /// returns a snapshot of the culture information of the thread 
        /// that created this object.
        /// </summary>
        public override CultureInfo CurrentCulture
        {
            get { return _originalCultureInfo; }
        }

        /// <summary>
        /// Gets the UI culture information to use. This implementation 
        /// returns a snapshot of the UI culture information of the thread 
        /// that created this object.
        /// </summary>
        public override CultureInfo CurrentUICulture
        {
            get { return _originalUICultureInfo; }
        }

        /// <summary>
        /// Gets an identifier for this host. This implementation always 
        /// returns the GUID allocated at instantiation time.
        /// </summary>
        public override Guid InstanceId
        {
            get { return s_instanceId; }
        }

        /// <summary>
        /// Gets a string that contains the name of this host implementation. 
        /// Keep in mind that this string may be used by script writers to
        /// identify when your host is being used.
        /// </summary>
        public override string Name
        {
            get { return "MySampleConsoleHostImplementation"; }
        }

        /// <summary>
        /// Gets an instance of the implementation of the PSHostUserInterface
        /// class for this application. This instance is allocated once at startup time
        /// and returned every time thereafter.
        /// </summary>
        public override PSHostUserInterface UI
        {
            get { return this.myHostUserInterface; }
        }

        /// <summary>
        /// Gets the version object for this application. Typically this 
        /// should match the version resource in the application.
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        #region IHostSupportsInteractiveSession Properties

        /// <summary>
        /// Gets a value indicating whether a request 
        /// to open a PSSession has been made.
        /// </summary>
        public bool IsRunspacePushed
        {
            get { return this.pushedRunspace != null; }
        }

        /// <summary>
        /// Gets or sets the runspace used by the PSSession.
        /// </summary>
        public Runspace Runspace
        {
            get { return _program.myRunSpace; }
            internal set { _program.myRunSpace = value; }
        }
        #endregion IHostSupportsInteractiveSession Properties

        /// <summary>
        /// This API Instructs the host to interrupt the currently running 
        /// pipeline and start a new nested input loop. In this example this 
        /// functionality is not needed so the method throws a 
        /// NotImplementedException exception.
        /// </summary>
        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException(
                  "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API instructs the host to exit the currently running input loop. 
        /// In this example this functionality is not needed so the method 
        /// throws a NotImplementedException exception.
        /// </summary>
        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException(
                  "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API is called before an external application process is 
        /// started. Typically it is used to save state so that the parent  
        /// can restore state that has been modified by a child process (after 
        /// the child exits). In this example this functionality is not  
        /// needed so the method returns nothing.
        /// </summary>
        public override void NotifyBeginApplication()
        {
            return;
        }

        /// <summary>
        /// This API is called after an external application process finishes.
        /// Typically it is used to restore state that a child process has
        /// altered. In this example, this functionality is not needed so  
        /// the method returns nothing.
        /// </summary>
        public override void NotifyEndApplication()
        {
            return;
        }

        /// <summary>
        /// Indicate to the host application that exit has
        /// been requested. Pass the exit code that the host
        /// application should use when exiting the process.
        /// </summary>
        /// <param name="exitCode">The exit code that the 
        /// host application should use.</param>
        public override void SetShouldExit(int exitCode)
        {
            _program.ShouldExit = true;
            _program.ExitCode = exitCode;
        }

        #region IHostSupportsInteractiveSession Methods

        /// <summary>
        /// Requests to close a PSSession.
        /// </summary>
        public void PopRunspace()
        {
            Runspace = this.pushedRunspace;
            this.pushedRunspace = null;
        }

        /// <summary>
        /// Requests to open a PSSession.
        /// </summary>
        /// <param name="runspace">Runspace to use.</param>
        public void PushRunspace(Runspace runspace)
        {
            this.pushedRunspace = Runspace;
            Runspace = runspace;
        }

        #endregion IHostSupportsInteractiveSession Methods
    }
    /// <summary>
    /// A sample implementation of the PSHostUserInterface abstract class for 
    /// console applications. Not all members are implemented. Those that are 
    /// not implemented throw a NotImplementedException exception or return 
    /// nothing. Members that are implemented include those that map easily to 
    /// Console APIs and a basic implementation of the prompt API provided. 
    /// </summary>
    internal class MyHostUserInterface : PSHostUserInterface, IHostUISupportsMultipleChoiceSelection
    {
        /// <summary>
        /// A reference to the PSRawUserInterface implementation.
        /// </summary>
        private MyRawUserInterface _myRawUi = new MyRawUserInterface();

        public ConsoleControls console { get; set; }

        /// <summary>
        /// Gets an instance of the PSRawUserInterface class for this host
        /// application.
        /// </summary>
        public override PSHostRawUserInterface RawUI
        {
            get { return _myRawUi; }
        }

        public MyHostUserInterface(ConsoleControls c)
        {
            console = c;
        }

        /// <summary>
        /// Prompts the user for input. 
        /// <param name="caption">The caption or title of the prompt.</param>
        /// <param name="message">The text of the prompt.</param>
        /// <param name="descriptions">A collection of FieldDescription objects  
        /// that describe each field of the prompt.</param>
        /// <returns>A dictionary object that contains the results of the user 
        /// prompts.</returns>
        public override Dictionary<string, PSObject> Prompt(
                                  string caption,
                                  string message,
                                  Collection<FieldDescription> descriptions)
        {
            this.Write(
                       ConsoleColor.DarkCyan,
                       ConsoleColor.Black,
                       caption + "\n" + message + " ");
            Dictionary<string, PSObject> results =
                     new Dictionary<string, PSObject>();
            foreach (FieldDescription fd in descriptions)
            {
                string[] label = GetHotkeyAndLabel(fd.Label);
                this.WriteLine(label[1]);
                string userData = console.ReadLine();
                if (userData == null)
                {
                    return null;
                }

                results[fd.Name] = PSObject.AsPSObject(userData);
            }

            return results;
        }

        /// <summary>

        /// Provides a set of choices that enable the user to choose a 
        /// single option from a set of options. 
        /// </summary>
        /// <param name="caption">Text that proceeds (a title) the choices.</param>
        /// <param name="message">A message that describes the choice.</param>
        /// <param name="choices">A collection of ChoiceDescription objects that  
        /// describ each choice.</param>
        /// <param name="defaultChoice">The index of the label in the Choices  
        /// parameter collection. To indicate no default choice, set to -1.</param>
        /// <returns>The index of the Choices parameter collection element that 
        /// corresponds to the option that is selected by the user.</returns>
        public override int PromptForChoice(
                                            string caption,
                                            string message,
                                            Collection<ChoiceDescription> choices,
                                            int defaultChoice)
        {
            // Write the caption and message strings in Blue.
            this.WriteLine(
                           ConsoleColor.Blue,
                           ConsoleColor.Black,
                           caption + "\n" + message + "\n");

            // Convert the choice collection into something that is
            // easier to work with. See the BuildHotkeysAndPlainLabels 
            // method for details.
            string[,] promptData = BuildHotkeysAndPlainLabels(choices);

            // Format the overall choice prompt string to display.
            StringBuilder sb = new StringBuilder();
            for (int element = 0; element < choices.Count; element++)
            {
                sb.Append(String.Format(
                                        CultureInfo.CurrentCulture,
                                        "|{0}> {1} ",
                                        promptData[0, element],
                                        promptData[1, element]));
            }

            sb.Append(String.Format(
                                    CultureInfo.CurrentCulture,
                                    "[Default is ({0}]",
                                    promptData[0, defaultChoice]));

            // Read prompts until a match is made, the default is
            // chosen, or the loop is interrupted with ctrl-C.
            while (true)
            {
                this.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, sb.ToString());
                string data = console.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

                // If the choice string was empty, use the default selection.
                if (data.Length == 0)
                {
                    return defaultChoice;
                }

                // See if the selection matched and return the
                // corresponding index if it did.
                for (int i = 0; i < choices.Count; i++)
                {
                    if (promptData[0, i] == data)
                    {
                        return i;
                    }
                }

                this.WriteErrorLine("Invalid choice: " + data);
            }
        }

        #region IHostUISupportsMultipleChoiceSelection Members

        /// <summary>
        /// Provides a set of choices that enable the user to choose a one or 
        /// more options from a set of options. 
        /// </summary>
        /// <param name="caption">Text that proceeds (a title) the choices.</param>
        /// <param name="message">A message that describes the choice.</param>
        /// <param name="choices">A collection of ChoiceDescription objects that  
        /// describ each choice.</param>
        /// <param name="defaultChoices">The index of the label in the Choices  
        /// parameter collection. To indicate no default choice, set to -1.</param>
        /// <returns>The index of the Choices parameter collection element that 
        /// corresponds to the option that is selected by the user.</returns>
        public Collection<int> PromptForChoice(
                                               string caption,
                                               string message,
                                               Collection<ChoiceDescription> choices,
                                               IEnumerable<int> defaultChoices)
        {
            // Write the caption and message strings in Blue.
            this.WriteLine(
                           ConsoleColor.Blue,
                           ConsoleColor.Black,
                           caption + "\n" + message + "\n");

            // Convert the choice collection into something that is
            // easier to work with. See the BuildHotkeysAndPlainLabels 
            // method for details.
            string[,] promptData = BuildHotkeysAndPlainLabels(choices);

            // Format the overall choice prompt string to display.
            StringBuilder sb = new StringBuilder();
            for (int element = 0; element < choices.Count; element++)
            {
                sb.Append(String.Format(
                                        CultureInfo.CurrentCulture,
                                        "|{0}> {1} ",
                                        promptData[0, element],
                                        promptData[1, element]));
            }

            Collection<int> defaultResults = new Collection<int>();
            if (defaultChoices != null)
            {
                int countDefaults = 0;
                foreach (int defaultChoice in defaultChoices)
                {
                    ++countDefaults;
                    defaultResults.Add(defaultChoice);
                }

                if (countDefaults != 0)
                {
                    sb.Append(countDefaults == 1 ? "[Default choice is " : "[Default choices are ");
                    foreach (int defaultChoice in defaultChoices)
                    {
                        sb.AppendFormat(
                                        CultureInfo.CurrentCulture,
                                        "\"{0}\",",
                                        promptData[0, defaultChoice]);
                    }

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
            }

            this.WriteLine(
                           ConsoleColor.Cyan,
                           ConsoleColor.Black,
                           sb.ToString());
            // Read prompts until a match is made, the default is
            // chosen, or the loop is interrupted with ctrl-C.
            Collection<int> results = new Collection<int>();
            while (true)
            {
            ReadNext:
                string prompt = string.Format(CultureInfo.CurrentCulture, "Choice[{0}]:", results.Count);
                this.Write(ConsoleColor.Cyan, ConsoleColor.Black, prompt);
                string data = Console.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

                // If the choice string was empty, no more choices have been made.
                // If there were no choices made, return the defaults
                if (data.Length == 0)
                {
                    return (results.Count == 0) ? defaultResults : results;
                }

                // See if the selection matched and return the
                // corresponding index if it did.
                for (int i = 0; i < choices.Count; i++)
                {
                    if (promptData[0, i] == data)
                    {
                        results.Add(i);
                        goto ReadNext;
                    }
                }

                this.WriteErrorLine("Invalid choice: " + data);
            }
        }

        #endregion

        /// <summary>
        /// Prompts the user for credentials with a specified prompt window 
        /// caption, prompt message, user name, and target name. In this 
        /// example this functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be 
        /// prompted for.</param>
        /// <param name="targetName">The name of the target for which the 
        /// credential is collected.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override PSCredential PromptForCredential(
                                                         string caption,
                                                         string message,
                                                         string userName,
                                                         string targetName)
        {
            throw new NotImplementedException(
                                 "The method or operation is not implemented.");
        }

        /// <summary>
        /// Prompts the user for credentials by using a specified prompt window 
        /// caption, prompt message, user name and target name, credential 
        /// types allowed to be returned, and UI behavior options. In this 
        /// example this functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <param name="caption">The caption for the message window.</param>
        /// <param name="message">The text of the message.</param>
        /// <param name="userName">The user name whose credential is to be 
        /// prompted for.</param>
        /// <param name="targetName">The name of the target for which the 
        /// credential is collected.</param>
        /// <param name="allowedCredentialTypes">A PSCredentialTypes constant  
        /// that identifies the type of credentials that can be returned.</param>
        /// <param name="options">A PSCredentialUIOptions constant that 
        /// identifies the UI behavior when it gathers the credentials.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override PSCredential PromptForCredential(
                                           string caption,
                                           string message,
                                           string userName,
                                           string targetName,
                                           PSCredentialTypes allowedCredentialTypes,
                                           PSCredentialUIOptions options)
        {
            throw new NotImplementedException(
                                    "The method or operation is not implemented.");
        }

        /// <summary>
        /// Reads characters that are entered by the user until a newline 
        /// (carriage return) is encountered.
        /// </summary>
        /// <returns>The characters that are entered by the user.</returns>
        public override string ReadLine()
        {
            return console.ReadLine().Replace(":\\>", "");
        }

        /// <summary>
        /// Reads characters entered by the user until a newline (carriage return) 
        /// is encountered and returns the characters as a secure string. In this 
        /// example this functionality is not needed so the method throws a 
        /// NotImplementException exception.
        /// </summary>
        /// <returns>Throws a NotImplemented exception.</returns>
        public override System.Security.SecureString ReadLineAsSecureString()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Writes characters to the output display of the host.
        /// </summary>
        /// <param name="value">The characters to be written.</param>
        public override void Write(string value)
        {
            console.WriteLine(value);
        }

        /// <summary>
        /// Writes characters to the output display of the host with possible 
        /// foreground and background colors. 
        /// </summary>
        /// <param name="foregroundColor">The color of the characters.</param>
        /// <param name="backgroundColor">The backgound color to use.</param>
        /// <param name="value">The characters to be written.</param>
        public override void Write(
                                   ConsoleColor foregroundColor,
                                   ConsoleColor backgroundColor,
                                   string value)
        {
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            console.WriteLine(value);
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }


        /// <summary>
        /// Writes a line of characters to the output display of the host 
        /// with foreground and background colors and appends a newline (carriage return). 
        /// </summary>
        /// <param name="foregroundColor">The forground color of the display. </param>
        /// <param name="backgroundColor">The background color of the display. </param>
        /// <param name="value">The line to be written.</param>
        public override void WriteLine(
                                       ConsoleColor foregroundColor,
                                       ConsoleColor backgroundColor,
                                       string value)
        {
            ConsoleColor oldFg = Console.ForegroundColor;
            ConsoleColor oldBg = Console.BackgroundColor;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            console.WriteLine(value);
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }

        /// <summary>
        /// Writes a debug message to the output display of the host.
        /// </summary>
        /// <param name="message">The debug message that is displayed.</param>
        public override void WriteDebugLine(string message)
        {
            this.WriteLine(
                           ConsoleColor.DarkYellow,
                           ConsoleColor.Black,
                           String.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
        }


        /// <summary>
        /// Writes an error message to the output display of the host.
        /// </summary>
        /// <param name="value">The error message that is displayed.</param>
        public override void WriteErrorLine(string value)
        {
            this.WriteLine(
                           ConsoleColor.Red,
                           ConsoleColor.Black,
                           value);
        }

        /// <summary>
        /// Writes a newline character (carriage return) 
        /// to the output display of the host. 
        /// </summary>
        public override void WriteLine()
        {
            console.WriteLine("\n");
        }

        /// <summary>
        /// Writes a line of characters to the output display of the host 
        /// and appends a newline character(carriage return). 
        /// </summary>
        /// <param name="value">The line to be written.</param>
        public override void WriteLine(string value)
        {
            console.WriteLine(value);
        }

        /// <summary>
        /// Writes a progress report to the output display of the host.
        /// </summary>
        /// <param name="sourceId">Unique identifier of the source of the record. </param>
        /// <param name="record">A ProgressReport object.</param>
        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        /// <summary>
        /// Writes a verbose message to the output display of the host.
        /// </summary>
        /// <param name="message">The verbose message that is displayed.</param>
        public override void WriteVerboseLine(string message)
        {
            this.WriteLine(
                           ConsoleColor.Green,
                           ConsoleColor.Black,
                           String.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
        }

        /// <summary>
        /// Writes a warning message to the output display of the host.
        /// </summary>
        /// <param name="message">The warning message that is displayed.</param>
        public override void WriteWarningLine(string message)
        {
            this.WriteLine(
                           ConsoleColor.Yellow,
                           ConsoleColor.Black,
                           String.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
        }


        /// <summary>
        /// Parse a string containing a hotkey character.
        /// Take a string of the form
        ///    Yes to &amp;all
        /// and returns a two-dimensional array split out as
        ///    "A", "Yes to all".
        /// </summary>
        /// <param name="input">The string to process</param>
        /// <returns>
        /// A two dimensional array containing the parsed components.
        /// </returns>
        private static string[] GetHotkeyAndLabel(string input)
        {
            string[] result = new string[] { String.Empty, String.Empty };
            string[] fragments = input.Split('&');
            if (fragments.Length == 2)
            {
                if (fragments[1].Length > 0)
                {
                    result[0] = fragments[1][0].ToString().
                    ToUpper(CultureInfo.CurrentCulture);
                }

                result[1] = (fragments[0] + fragments[1]).Trim();
            }
            else
            {
                result[1] = input;
            }

            return result;
        }

        /// <summary>
        /// This is a private worker function splits out the
        /// accelerator keys from the menu and builds a two
        /// dimentional array with the first access containing the
        /// accelerator and the second containing the label string
        /// with the &amp; removed.
        /// </summary>
        /// <param name="choices">The choice collection to process</param>
        /// <returns>
        /// A two dimensional array containing the accelerator characters
        /// and the cleaned-up labels</returns>
        private static string[,] BuildHotkeysAndPlainLabels(
             Collection<ChoiceDescription> choices)
        {
            // Allocate the result array
            string[,] hotkeysAndPlainLabels = new string[2, choices.Count];

            for (int i = 0; i < choices.Count; ++i)
            {
                string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
                hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
                hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
            }

            return hotkeysAndPlainLabels;
        }
    }
    /// <summary>
    /// A sample implementation of the PSHostRawUserInterface for console
    /// applications. Members of this class that easily map to the .NET 
    /// console class are implemented. More complex methods are not 
    /// implemented and throw a NotImplementedException exception.
    /// </summary>
    internal class MyRawUserInterface : PSHostRawUserInterface
    {
        /// <summary>
        /// Gets or sets the background color of text to be written.
        /// This maps to the corresponding Console.Background property.
        /// </summary>
        public override ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }

        public ConsoleControls consoles { get; set; }

        /// <summary>
        /// Gets or sets the host buffer size adapted from the Console buffer 
        /// size members.
        /// </summary>
        public override Size BufferSize
        {
            get { return new Size(80, 25); }
            set { }
        }

        /// <summary>
        /// Gets or sets the cursor position. In this example this 
        /// functionality is not needed so the property throws a 
        /// NotImplementException exception.
        /// </summary>
        public override Coordinates CursorPosition
        {
            get
            {
                throw new NotImplementedException(
                     "The method or operation is not implemented.");
            }
            set
            {
                throw new NotImplementedException(
                     "The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Gets or sets the cursor size taken directly from the 
        /// Console.CursorSize property.
        /// </summary>
        public override int CursorSize
        {
            get { return Console.CursorSize; }
            set { Console.CursorSize = value; }
        }

        /// <summary>
        /// Gets or sets the foreground color of the text to be written.
        /// This maps to the corresponding Console.ForgroundColor property.
        /// </summary>
        public override ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        /// <summary>
        /// Gets a value indicating whether a key is available. This maps to  
        /// the corresponding Console.KeyAvailable property.
        /// </summary>
        public override bool KeyAvailable
        {
            get { return Console.KeyAvailable; }
        }

        /// <summary>
        /// Gets the maximum physical size of the window adapted from the  
        /// Console.LargestWindowWidth and Console.LargestWindowHeight 
        /// properties.
        /// </summary>
        public override Size MaxPhysicalWindowSize
        {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
        }

        /// <summary>
        /// Gets the maximum window size adapted from the 
        /// Console.LargestWindowWidth and console.LargestWindowHeight 
        /// properties.
        /// </summary>
        public override Size MaxWindowSize
        {
            get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
        }

        /// <summary>
        /// Gets or sets the window position adapted from the Console window position 
        /// members.
        /// </summary>
        public override Coordinates WindowPosition
        {
            get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
            set { Console.SetWindowPosition(value.X, value.Y); }
        }

        /// <summary>
        /// Gets or sets the window size adapted from the corresponding Console 
        /// calls.
        /// </summary>
        public override Size WindowSize
        {
            get { return new Size(Console.WindowWidth, Console.WindowHeight); }
            set { Console.SetWindowSize(value.Width, value.Height); }
        }

        /// <summary>
        /// Gets or sets the title of the window mapped to the Console.Title 
        /// property.
        /// </summary>
        public override string WindowTitle
        {
            get { return Console.Title; }
            set { Console.Title = value; }
        }

        /// <summary>
        /// This API resets the input buffer. In this example this 
        /// functionality is not needed so the method returns nothing.
        /// </summary>
        public override void FlushInputBuffer()
        {
        }

        /// <summary>
        /// This API returns a rectangular region of the screen buffer. In 
        /// this example this functionality is not needed so the method throws 
        /// a NotImplementException exception.
        /// </summary>
        /// <param name="rectangle">Defines the size of the rectangle.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException(
                     "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API Reads a pressed, released, or pressed and released keystroke 
        /// from the keyboard device, blocking processing until a keystroke is 
        /// typed that matches the specified keystroke options. In this example 
        /// this functionality is not needed so the method throws a
        /// NotImplementException exception.
        /// </summary>
        /// <param name="options">Options, such as IncludeKeyDown,  used when 
        /// reading the keyboard.</param>
        /// <returns>Throws a NotImplementedException exception.</returns>
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException(
                      "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API crops a region of the screen buffer. In this example 
        /// this functionality is not needed so the method throws a
        /// NotImplementException exception.
        /// </summary>
        /// <param name="source">The region of the screen to be scrolled.</param>
        /// <param name="destination">The region of the screen to receive the 
        /// source region contents.</param>
        /// <param name="clip">The region of the screen to include in the operation.</param>
        /// <param name="fill">The character and attributes to be used to fill all cell.</param>
        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException(
                      "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API copies an array of buffer cells into the screen buffer 
        /// at a specified location. In this example this  functionality is 
        /// not needed si the method  throws a NotImplementedException exception.
        /// </summary>
        /// <param name="origin">The parameter is not used.</param>
        /// <param name="contents">The parameter is not used.</param>
        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException(
                      "The method or operation is not implemented.");
        }

        /// <summary>
        /// This API Copies a given character, foreground color, and background 
        /// color to a region of the screen buffer. In this example this 
        /// functionality is not needed so the method throws a
        /// NotImplementException exception./// </summary>
        /// <param name="rectangle">Defines the area to be filled. </param>
        /// <param name="fill">Defines the fill character.</param>
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException(
                      "The method or operation is not implemented.");
        }
    }

    public class NavigatorExt
    {
        public ArrayList N { get; set; }

        public int act = -1;

        public int maxsize = 10;

        public NavigatorExt()
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
            if (act <= 0)
                return "";

            act++;

            if (act >= N.Count)
                act = N.Count - 1;

            if (act < 0)
                act = 0;

            string s = N[act] as string;

            return s;
        }
    }
}