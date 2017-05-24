using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Runner.Data;

namespace xunit.runner
{
    public class TestAssemblyViewModel
    {
        private readonly AssemblyAndConfigFile _assembly;
        private bool _isSelected;
        private AssemblyState _state;

        public TestAssemblyViewModel(AssemblyAndConfigFile assembly)
        {
            _assembly = assembly;
        }

        public string FileName => _assembly.AssemblyFileName;
        public string ConfigFileName => Path.GetFileNameWithoutExtension(_assembly.ConfigFileName);
        public string DisplayName => Path.GetFileNameWithoutExtension(_assembly.AssemblyFileName);

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public AssemblyState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }

    public enum AssemblyState
    {
        Ready,
        Loading
    }
    public class AssemblyAndConfigFile
    {
        public string AssemblyFileName { get; }
        public string ConfigFileName { get; }

        public AssemblyAndConfigFile(string assemblyFileName, string configFileName)
        {
            this.AssemblyFileName = Path.GetFullPath(assemblyFileName);
            if (configFileName != null)
            {
                this.ConfigFileName = Path.GetFullPath(configFileName);
            }
        }
    }
    public interface ITestUtil
    {
        /// <summary>
        /// Discover the list of test cases which are available in the specified assembly.
        /// </summary>
        ConcurrentQueue<TestCaseData> Discover(
            string assebmlyFileName,
            Action<IEnumerable<TestCaseData>> testsDiscovered,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Begin a run of all unit tests for the given assembly.
        /// </summary>
        bool RunAll(
            string assemblyFileName,
            Action<IEnumerable<TestResultData>> testsFinished,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Begin a run of specific unit tests for the given assembly.
        /// </summary>
        bool RunSpecific(
            string assemblyFileName,
            ImmutableArray<string> testCasesToRun,
            Action<IEnumerable<TestResultData>> testsFinished,
            CancellationToken cancellationToken = default(CancellationToken));
    }
  
    public partial class TraitViewModel : IEquatable<TraitViewModel>
    {
        private readonly TraitViewModel _parent;
        private bool? _isChecked;
        private bool _isExpanded;
        private string _text;

        public List<TraitViewModel> Children { get; }

        public bool Equals(TraitViewModel other)
        {
            
            if (other == null)
                return false;

            return String.Equals(this.Text, other.Text,
                                StringComparison.OrdinalIgnoreCase);
        }

        public TraitViewModel(string text)
            : this(null, text)
        {
        }

        private TraitViewModel(TraitViewModel parent, string text)
        {
            this._parent = parent;
            this._isChecked = false;
            this._isExpanded = true;
            this._text = text;
            this.Children = new List<TraitViewModel>();
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this._isChecked)
            {
                return;
            }

            this._isChecked = value;

            if (updateChildren && value != null)
            {
                foreach (var child in this.Children)
                {
                    child.SetIsChecked(value, updateChildren: true, updateParent: false);
                }
            }

            if (updateParent && _parent != null)
            {
                _parent.VerifyCheckState();
            }

            //this.RaisePropertyChanged(nameof(IsChecked));
        }

        private void VerifyCheckState()
        {
            bool? state = null;
            var isFirst = true;

            foreach (var child in this.Children)
            {
                if (isFirst)
                {
                    state = child.IsChecked;
                    isFirst = false;
                }
                else if (state != child.IsChecked)
                {
                    state = null;
                    break;
                }
            }

            this.SetIsChecked(state, updateChildren: false, updateParent: true);
        }

        public void AddValues(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                var index = this.Children.FindIndex(a => a.Text == value);
                if (index < 0)
                {
                    this.Children.Insert(~index, new TraitViewModel(this, value));
                }
            }
        }

        public TraitViewModel GetOrAdd(string text)
        {
            var index = this.Children.FindIndex(a => a.Text == text);

            if (index < 0)
            {
                var viewModel = new TraitViewModel(this, text);
                this.Children.Insert(~index, viewModel);
                return viewModel;
            }

            return this.Children[index];
        }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, updateChildren: true, updateParent: true); }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
    public class TestCaseViewModel
    {
        private TestState _state = TestState.NotRun;

        public string DisplayName { get; }
        public string UniqueID { get; }
        public string SkipReason { get; }
        public string AssemblyFileName { get; }
        public ImmutableArray<TraitViewModel> Traits { get; }
        public bool IsSelected { get; set; }

        public bool HasSkipReason => !string.IsNullOrEmpty(this.SkipReason);

        public TestState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public TestCaseViewModel(string displayName, string uniqueID, string skipReason, string assemblyFileName, IEnumerable<TraitViewModel> traits)
        {
            this.DisplayName = displayName;
            this.UniqueID = uniqueID;
            this.SkipReason = skipReason;
            this.AssemblyFileName = assemblyFileName;
            this.Traits = traits.ToImmutableArray();

            if (!string.IsNullOrEmpty(skipReason))
            {
                _state = TestState.Skipped;
            }
        }
    }
    public class BackgroundWriter<T>
    {
        private readonly ClientWriter _writer;
        private readonly ImmutableArray<T> _data;
        private readonly Action<ClientWriter, T> _writeValue;
        private readonly CancellationToken _cancellationToken;

        internal BackgroundWriter(ClientWriter writer, ImmutableArray<T> data, Action<ClientWriter, T> writeValue, CancellationToken cancellationToken)
        {
            _writer = writer;
            _writeValue = writeValue;
            _data = data;
            _cancellationToken = cancellationToken;
        }

        internal Task WriteAsync()
        {
            return Task.Run(() => GoOnBackground(), _cancellationToken);
        }

        private void GoOnBackground()
        {
            foreach (var item in _data)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _writer.Write(TestDataKind.Value);
                _writeValue(_writer, item);
            }

            _writer.Write(TestDataKind.EndOfData);
        }
    }

    /// <summary>
    /// Utility for reading a collection of <see cref="{T}"/> values from the given 
    /// <see cref="ClientReader"/> value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BackgroundReader<T> where T : class
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly ClientReader _reader;
        private readonly Func<ClientReader, T> _readValue;

        internal ClientReader Reader => _reader;

        internal BackgroundReader(ConcurrentQueue<T> queue, ClientReader reader, Func<ClientReader, T> readValue)
        {
            _queue = queue;
            _reader = reader;
            _readValue = readValue;
        }

        public void ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            GoOnBackground(cancellationToken);
        }

        /// <summary>
        /// This will be called on a background thread to read the results of the test from the 
        /// named pipe client stream.
        /// </summary>
        /// <returns></returns>
        private void GoOnBackground(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var kind = _reader.ReadKind();
                    if (kind != TestDataKind.Value)
                    {
                        break;
                    }

                    var value = _readValue(_reader);
                    _queue.Enqueue(value);
                }
                catch
                {
                    // TODO: Happens when the connection unexpectedly closes on us.  Need to surface this
                    // to the user.
                    break;
                }
            }

            // Signal we are done 
            _queue.Enqueue(null);
        }
    }
    public class Connection : IDisposable
    {
        private NamedPipeClientStream _stream;
        private ClientReader _reader;

        internal NamedPipeClientStream Stream => _stream;

        internal ClientReader Reader => _reader;

        internal Connection(NamedPipeClientStream stream)
        {
            _stream = stream;
            _reader = new ClientReader(stream);
        }

        internal void Dispose()
        {
            if (_stream == null)
            {
                return;
            }

            try
            {
                _stream.WriteAsync(new byte[] { 0 }, 0, 1);
            }
            catch
            {
                // Signal to server we are done with the connection.  Okay to fail because
                // it means the server isn't listening anymore.
            }

            _stream.Close();
            _stream = null;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }


public class BackgroundProducer<T> where T : class
    {
        private const int MaxResultPerTick = 1000;

        public Connection _connection;
        private readonly ConcurrentQueue<T> _queue;
        
        private readonly Action<List<T>> _callback;
        private readonly int _maxPerTick;
        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        public Task Task => _taskCompletionSource.Task;

        public BackgroundProducer(
            Connection connection,
            ConcurrentQueue<T> queue,
            Action<List<T>> callback,
            int maxResultPerTick = MaxResultPerTick,
            TimeSpan? interval = null)
        {
            _connection = connection;
            _queue = queue;
            _maxPerTick = maxResultPerTick;
            _callback = callback;
          
            _taskCompletionSource = new TaskCompletionSource<bool>();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            var i = 0;
            var list = new List<T>();
            var isDone = false;
            T value;
            while (i < _maxPerTick && _queue.TryDequeue(out value))
            {
                if (value == null)
                {
                    isDone = true;
                    break;
                }

                list.Add(value);
            }

            _callback(list);

            if (isDone)
            {
                try
                {
                    
                    _connection.Dispose();
                }
                finally
                {
                    _taskCompletionSource.SetResult(true);
                }
            }
        }
    }

public class RemoteTestUtil : ITestUtil
    {
        private struct ProcessInfo
        {
            internal readonly string PipeName;
            internal readonly Process Process;

            internal ProcessInfo(string pipeName, Process process)
            {
                PipeName = pipeName;
                Process = process;
            }
        }

       

        private ProcessInfo? _processInfo;

        internal RemoteTestUtil()
        {
            
            _processInfo = StartWorkerProcess();
        }

        private Connection CreateConnection(string action, string argument, CancellationToken cancellationToken)
        {
            var pipeName = GetPipeName();

            try
            {
                var stream = new NamedPipeClientStream(pipeName);
                stream.ConnectAsync(cancellationToken);

                var writer = new ClientWriter(stream);
                writer.Write(action);
                writer.Write(argument);

                return new Connection(stream);
            }
            catch
            {
                try
                {
                    _processInfo?.Process.Kill();
                }
                catch
                {
                    // Inherent race condition here.  Just need to make sure the process is 
                    // dead as it can't even handle new connections.
                }

                throw;
            }
        }

        private string GetPipeName()
        {
            var process = _processInfo?.Process;
            if (process != null && !process.HasExited)
            {
                return _processInfo.Value.PipeName;
            }

            _processInfo = StartWorkerProcess();
            return _processInfo.Value.PipeName;
        }

        private static ProcessInfo StartWorkerProcess()
        {
            var pipeName = $"xunit.runner.wpf.pipe.{Guid.NewGuid()}";
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = typeof(Xunit.Runner.Worker.Program).Assembly.Location;
            processStartInfo.Arguments = $"{pipeName} {Process.GetCurrentProcess().Id}";
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            var process = Process.Start(processStartInfo);
            return new ProcessInfo(pipeName, process);
        }

        private void RecycleProcess()
        {
            var process = _processInfo?.Process;
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }

            _processInfo = StartWorkerProcess();
        }

        public ConcurrentQueue<TestCaseData> Discover(string assemblyPath, Action<List<TestCaseData>> callback, CancellationToken cancellationToken)
        {
            var connection = CreateConnection(Constants.ActionDiscover, assemblyPath, cancellationToken);
            ConcurrentQueue<TestCaseData> queue = ProcessResultsCore(connection, r => r.ReadTestCaseData(), callback, cancellationToken);

            RecycleProcess();

            return queue;
        }

        private bool RunCore(string actionName, string assemblyPath, ImmutableArray<string> testCaseDisplayNames, Action<List<TestResultData>> callback, CancellationToken cancellationToken)
        {
            var connection = CreateConnection(actionName, assemblyPath, cancellationToken);

            if (!testCaseDisplayNames.IsDefaultOrEmpty)
            {
                var backgroundWriter = new BackgroundWriter<string>(new ClientWriter(connection.Stream), testCaseDisplayNames, (w, s) => w.Write(s), cancellationToken);
                backgroundWriter.WriteAsync();
            }

            ProcessResultsCore(connection, r => r.ReadTestResultData(), callback, cancellationToken);

            return true;
        }

        public ConcurrentQueue<T> ProcessResultsCore<T>(Connection connection, Func<ClientReader, T> readValue, Action<List<T>> callback, CancellationToken cancellationToken)
            where T : class
        {
            var queue = new ConcurrentQueue<T>();
            var backgroundReader = new BackgroundReader<T>(queue, new ClientReader(connection.Stream), readValue);
            var backgroundProducer = new BackgroundProducer<T>(connection, queue, callback);

            backgroundReader.ReadAsync();
        
            return queue;
        }

        #region ITestUtil

        ConcurrentQueue<TestCaseData> ITestUtil.Discover(string assemblyFileName, Action<IEnumerable<TestCaseData>> testsDiscovered, CancellationToken cancellationToken)
        {
            return Discover(assemblyFileName, testsDiscovered, cancellationToken);
        }

        bool ITestUtil.RunAll(string assemblyFileName, Action<IEnumerable<TestResultData>> testsFinished, CancellationToken cancellationToken)
        {
            return RunCore(Constants.ActionRunAll, assemblyFileName, ImmutableArray<string>.Empty, testsFinished, cancellationToken);
        }

        bool ITestUtil.RunSpecific(string assemblyFileName, ImmutableArray<string> testCases, Action<IEnumerable<TestResultData>> testsFinished, CancellationToken cancellationToken)
        {
            return RunCore(Constants.ActionRunSpecific, assemblyFileName, testCases, testsFinished, cancellationToken);
        }

        #endregion
    }
 
    public class xrunner
    {
        public ITestUtil testUtil;

        public List<TestAssemblyViewModel> Assemblies { get; set; } = new List<TestAssemblyViewModel>();

        public HashSet<string> allTestCaseUniqueIDs = new HashSet<string>();

        public List<TestCaseViewModel> allTestCases = new List<TestCaseViewModel>();

        public List<TraitViewModel> traitCollectionView = new List<TraitViewModel>();

        private CancellationTokenSource filterCancellationTokenSource = new CancellationTokenSource();

        private CancellationTokenSource cancellationTokenSource;

        public int TestsSkipped = 0;

        private void OnTestsDiscovered(IEnumerable<TestCaseData> testCases)
        {
            var traitWorkerList = new List<TraitViewModel>();

            foreach (var testCase in testCases)
            {
                if (this.allTestCaseUniqueIDs.Contains(testCase.UniqueID))
                    continue;

                traitWorkerList.Clear();

                // Get or create traits.
                if (testCase.TraitMap?.Count > 0)
                {
                    foreach (var kvp in testCase.TraitMap)
                    {
                        var name = kvp.Key;
                        var values = kvp.Value;

                        int index = traitCollectionView.FindIndex(a => a.Text == name);

                        if (index < 0)
                        {
                            traitCollectionView.Add(new TraitViewModel(name));
                            index = traitCollectionView.FindIndex(a => a.Text == name);
                        }
                        var parentTraitViewModel = traitCollectionView[index];

                        foreach (var value in values)
                        {
                            var traitViewModel = parentTraitViewModel.GetOrAdd(value);
                            traitWorkerList.Add(traitViewModel);
                        }
                    }
                }

                var testCaseViewModel = new TestCaseViewModel(
                    testCase.DisplayName,
                    testCase.UniqueID,
                    testCase.SkipReason,
                    testCase.AssemblyPath,
                    traitWorkerList);

                if (testCaseViewModel.State == TestState.Skipped)
                {
                    TestsSkipped++;
                }

                this.allTestCaseUniqueIDs.Add(testCase.UniqueID);
                this.allTestCases.Add(testCaseViewModel);
            }
        }
        public List<TestAssemblyViewModel> LoadAssemblies(IEnumerable<AssemblyAndConfigFile> assemblies)
        {

            this.testUtil = new RemoteTestUtil();

            this.cancellationTokenSource = new CancellationTokenSource();

            var taskList = new List<ConcurrentQueue<TestCaseData>>();

            var newAssemblyViewModels = new List<TestAssemblyViewModel>();
            if (!assemblies.Any())
            {
                return newAssemblyViewModels;
            }
            try
            {
                //Task.Run(new Action(() =>
                {
                    
                    foreach (var assembly in assemblies)
                    {

                        ConcurrentQueue<TestCaseData> queue = this.testUtil.Discover(assembly.AssemblyFileName, this.OnTestsDiscovered, this.cancellationTokenSource.Token);
                      taskList.Add( queue);

                        var assemblyViewModel = new TestAssemblyViewModel(assembly);

                        newAssemblyViewModels.Add(assemblyViewModel);
                        this.Assemblies.Add(assemblyViewModel);
                        

                        assemblyViewModel.State = AssemblyState.Loading;
                    }

                    
                }
                //));
            }
            finally
            {
                foreach (var assemblyViewModel in newAssemblyViewModels)
                {
                    assemblyViewModel.State = AssemblyState.Ready;
                }

                
            }
            return newAssemblyViewModels;
        }

    }
}
