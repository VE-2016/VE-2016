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

using NUnit.UiKit.Controls;

namespace NUnit.Gui.Presenters
{
    using Model;
    using Views;

    public class ProgressBarPresenter
    {
        private IProgressBarView _progressBar;
        private ITestModel _model;

        public ProgressBarPresenter(IProgressBarView progressBar, ITestModel model)
        {
            _progressBar = progressBar;
            //_progressBar.Initialize(100);
            _model = model;

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            _model.TestLoaded += (ea) => { Initialize(100); };
            _model.TestUnloaded += (ea) => { Initialize(100); };
            _model.TestReloaded += (ea) => { Initialize(100); };
            _model.RunStarting += (ea) => { Initialize(ea.TestCount); };
            _model.TestFinished += (ea) => { ReportTestOutcome(ea.Result); };
            _model.SuiteFinished += (ea) => { ReportTestOutcome(ea.Result); };
        }

        public void Initialize(int max)
        {
            _progressBar.Initialize(max);
        }

        public void ReportTestOutcome(ResultNode result)
        {
            UpdateProgress(result);
            UpdateStatus(result.Outcome);
        }

        private void UpdateStatus(ResultState result)
        {
            // Status can only get worse during a run, so we can avoid
            // unnecessary calls to Invoke by checking current status.
            var currentStatus = _progressBar.Status;
            if (currentStatus != TestProgressBarStatus.Failure)
                switch (result.Status)
                {
                    case TestStatus.Failed:
                        _progressBar.Status = TestProgressBarStatus.Failure;
                        break;
                    case TestStatus.Skipped:
                        if (result.Label == "Invalid")
                            _progressBar.Status = TestProgressBarStatus.Failure;
                        else if (result.Label == "Ignored" && currentStatus != TestProgressBarStatus.Warning)
                            _progressBar.Status = TestProgressBarStatus.Warning;
                        break;
                }
        }

        private void UpdateProgress(TestNode result)
        {
            if (!result.IsSuite)
                _progressBar.Progress++;
        }

        //private void InvokeIfRequired(MethodInvoker _delegate)
        //{
        //    if (_progressBar.InvokeRequired)
        //        _progressBar.Invoke(_delegate);
        //    else
        //        _delegate();
        //}
    }
}
