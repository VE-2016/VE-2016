﻿// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Collections.Generic;
using NUnit.Engine;

namespace NUnit.Gui.Model
{
    /// <summary>
    /// Delegates for all events related to the model
    /// </summary>
    public delegate void TestEventHandler(TestEventArgs args);
    public delegate void RunStartingEventHandler(RunStartingEventArgs args);
    public delegate void TestNodeEventHandler(TestNodeEventArgs args);
    public delegate void TestResultEventHandler(TestResultEventArgs args);
    public delegate void TestItemEventHandler(TestItemEventArgs args);

    public interface ITestModel : IServiceLocator, System.IDisposable
    {
        #region Events

        // Events related to loading and unloading tests.
        event TestNodeEventHandler TestLoaded;
        event TestNodeEventHandler TestReloaded;
        event TestEventHandler TestUnloaded;

        // Events related to running tests
        event RunStartingEventHandler RunStarting;
        event TestNodeEventHandler SuiteStarting;
        event TestNodeEventHandler TestStarting;

        event TestResultEventHandler RunFinished;
        event TestResultEventHandler SuiteFinished;
        event TestResultEventHandler TestFinished;

        // Event used to broadcast a change in the selected
        // item, so that all presenters may be notified.
        event TestItemEventHandler SelectedItemChanged;

        #endregion

        #region Properties

        CommandLineOptions Options { get; }

        IRecentFiles RecentFiles { get; }

        bool IsPackageLoaded { get; }

        // TestNode hierarchy representing the discovered tests
        TestNode Tests { get; }

        // See if tests are available
        bool HasTests { get; }

        // See if a test is running
        bool IsTestRunning { get; }

        bool HasResults { get; }

        Settings.SettingsModel Settings { get; }

        IList<RuntimeFramework> AvailableRuntimes { get; }

        #endregion

        #region Methods

        // Perform initial actions on startup, loading and possibly running tests
        void OnStartup();

        // Create a new empty project using a default name
        void NewProject();

        // Create a new project given a filename
        void NewProject(string filename);

        void SaveProject();

        // Load a TestPackage
        void LoadTests(IList<string> files);

        // Unload current TestPackage
        void UnloadTests();

        // Reload current TestPackage
        void ReloadTests();

        // Reload current TestPackage using specified runtime
        void ReloadTests(string runtime);

        // Run all the tests
        void RunAllTests();

        // Run just the specified ITestItem
        void RunTests(ITestItem testItem);

        // Cancel the running test
        void CancelTestRun();

        // Get the result for a test if available
        ResultNode GetResultForTest(TestNode testNode);

        // Broadcast event when SelectedTestItem changes
        void NotifySelectedItemChanged(ITestItem testItem);

        #endregion
    }
}
