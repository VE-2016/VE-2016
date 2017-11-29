using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinExplorer;

namespace VE_Tests
{
    public partial class TestForm : Form
    {
        private WinExplorer.ExplorerForms mf { get; set; }

        public TestForm(PerformDelegateName name, object obs)
        {
            InitializeComponent();
            SuspendLayout();
            mf = new ExplorerForms();
            mf.Dock = DockStyle.Fill;
            mf.TopLevel = false;
            this.Controls.Add(mf);
            scr = mf.scr;
            GC.SuppressFinalize(scr);
            ResumeLayout();
            mf.Show();
            var t = new Task(() =>
            {
                if (name == PerformDelegateName.opensolution)
                    perform = OpenSolution;
                else if (name == PerformDelegateName.opensolutionandfilesfromproject)
                    perform = OpenSolutionAndFilesFromProject;
                else if (name == PerformDelegateName.opensolutionandfilesfromprojectwithrepeat)
                    perform = OpenSolutionAndFilesFromProjectWithRepeat;
                else if (name == PerformDelegateName.opensolutionandcheckifcompileitemsarepresentinsolutiontree)
                    perform = OpenSolutionAndCheckIfCompileItemsArePresentInSolutionTree;
                else if (name == PerformDelegateName.findmethodsinarbitraryfile)
                    perform = OpenSolutionAndFindMethodsInArbitraryFile;
                perform(obs);
            }
            );
            t.Start();
        }

        private ScriptControl.ScriptControl scr { get; set; }

        public enum PerformDelegateName
        {
            opensolution,
            opensolutionandfilesfromproject,
            opensolutionandfilesfromprojectwithrepeat,
            opensolutionandcheckifcompileitemsarepresentinsolutiontree,
            findmethodsinarbitraryfile
        }

        private Perform perform;

        public delegate void Perform(object obs);

        public void OpenSolution(object obs)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Context c = obs as Context;

            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            c.info = folder;
            mf.Invoke(new Action(() =>
            {
                //mf.CommandToPerform("LoadSolution", folder);
                mf.Command_OpenSolution(folder, mre);
            }));
            Task.Run(() => mre.WaitOne()).Wait();
            //NUnit.Framework.TestContext.WriteLine("Open solution for " + folder + " completed successfully ");
            this.Close();
        }

        public void OpenSolutionAndFilesFromProject(object obs)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Context c = obs as Context;

            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            c.info = folder;
            mf.Invoke(new Action(() =>
            {
                mf.Command_OpenSolution(folder, mre);
            }));
            Task.Run(() => mre.WaitOne()).Wait();
            var fs = mf.Command_CompileItemsByProjectName("WpfView");
            mf.BeginInvoke(new Action(() =>
            {
                foreach (var s in fs)
                    mf.Command_OpenFile(s);
            }));
        }

        public void OpenSolutionAndFilesFromProjectWithRepeat(object obs)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Context c = obs as Context;
            int i = 0;
            //while (i < 10)
            //{
            mre.Reset();
            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            c.info = folder;
            AppStatus appStatus = new AppStatus();
            mf.Invoke(new Action(() =>
            {
                GC.KeepAlive(mf.scr);
                mf.Command_OpenSolution(folder, mre);
                GC.KeepAlive(mf.scr);
            }));
            Task.Run(() => mre.WaitOne()).Wait();
            //while (i < 10) {
            AutoResetEvent are = new AutoResetEvent(false);
            var fs = mf.Command_CompileItemsByProjectName("WpfView");
            mf.BeginInvoke(new Action(() =>
            {
                GC.KeepAlive(mf.scr);
                foreach (var s in fs)
                {
                    mf.Command_OpenFile(s);
                        // Task.Run(() => Task.Delay(5000)).Wait();
                    }
                    //Task.Run(() => Task.Delay(10000)).Wait();
                    //mf.Command_CloseAllDocuments();
                    // mf.Command_CloseSolution();
                    mf.Command_ApplicationStatus(appStatus);
                    //NUnit.Framework.TestContext.WriteLine("Memory usage at closed state " + appStatus.ProcessMemoryUsed.ToString());
                    GC.KeepAlive(mf.scr);
                are.Set();
            }));
            Task.Run(() => are.WaitOne()).Wait();

            i++;
            //}
        }

        public void OpenSolutionAndCheckIfCompileItemsArePresentInSolutionTree(object obs)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Context c = obs as Context;

            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            c.info = folder;
            mf.Invoke(new Action(() =>
            {
                mf.Command_OpenSolution(folder, mre);
            }));
            Task.Run(() => mre.WaitOne()).Wait();
            VSSolutionStub vss = new VSSolutionStub();
            TreeView v = mf.Command_SolutionExplorerTreeView();
            vss.CheckIfCompileItemsArePresentInSolutionExplorerTree(v);
            //NUnit.Framework.TestContext.WriteLine("Open solution for " + folder + " and check compile items completed successfully ");
            mf.BeginInvoke(new Action(() => this.Close()));
        }

        public void OpenSolutionAndFindMethodsInArbitraryFile(object obs)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            Context c = obs as Context;

            string folder = AppDomain.CurrentDomain.BaseDirectory + "\\Sources\\Live-Charts-master\\LiveCharts.sln";
            c.info = folder;
            mf.BeginInvoke(new Action(() =>
            {
                mf.Command_OpenSolution(folder, mre);
            }));
            Task.Run(() => mre.WaitOne()).Wait();
            VSSolutionStub vss = new VSSolutionStub();
            vss.ef = mf;
            //vss.MethodMembers();
        }
    }
}