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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NUnit.UiKit.Controls;
using NUnit.UiKit.Elements;

namespace NUnit.Gui.Views
{
    public interface ITestPropertiesView : IView
    {
        event CommandHandler DisplayHiddenPropertiesChanged;

        bool Visible { get; set; }
        string Header { get; set; }
        IViewElement TestPanel { get; }
        IViewElement ResultPanel { get; }

        string TestType { get; set; }
        string FullName { get; set; }
        string Description { get; set; }
        string Categories { get; set; }
        string TestCount { get; set; }
        string RunState { get; set; }
        string SkipReason { get; set; }
        bool DisplayHiddenProperties { get; }
        string Properties { get; set; }
        string Outcome { get; set; }
        string ElapsedTime { get; set; }
        string AssertCount { get; set; }
        string Message { get; set; }
        string StackTrace { get; set; }
        string Output { get; set; }
    }

    public partial class TestPropertiesView : UserControl, ITestPropertiesView
    {
        public event CommandHandler DisplayHiddenPropertiesChanged;

        public TestPropertiesView()
        {
            InitializeComponent();

            this.TestPanel = new ControlElement<Panel>(testPanel);
            this.ResultPanel = new ControlElement<Panel>(resultPanel);

            displayHiddenProperties.CheckedChanged += (s, e) =>
            {
                if (DisplayHiddenPropertiesChanged != null)
                    DisplayHiddenPropertiesChanged();
            };
        }

        public string Header
        {
            get { return header.Text; }
            set { InvokeIfRequired(() => { header.Text = value; }); }
        }

        public IViewElement TestPanel { get; private set; }
        public IViewElement ResultPanel { get; private set; }

        public string TestType
        {
            get { return testType.Text; }
            set { InvokeIfRequired(() => { testType.Text = value; }); }
        }

        public string FullName
        {
            get { return fullName.Text; }
            set { InvokeIfRequired(() => { fullName.Text = value; }); }
        }

        public string Description
        {
            get { return description.Text; }
            set { InvokeIfRequired(() => { description.Text = value; }); }
        }

        public string Categories
        {
            get { return categories.Text; }
            set { InvokeIfRequired(() => { categories.Text = value; }); }
        }

        public string TestCount
        {
            get { return testCount.Text; }
            set { InvokeIfRequired(() => { testCount.Text = value; }); }
        }

        public string RunState
        {
            get { return runState.Text; }
            set { InvokeIfRequired(() => { runState.Text = value; }); }
        }

        public string SkipReason
        {
            get { return skipReason.Text; }
            set { InvokeIfRequired(() => { skipReason.Text = value; }); }
        }

        public bool DisplayHiddenProperties
        {
            get { return displayHiddenProperties.Checked; }
        }

        public string Properties
        {
            get { return properties.Text; }
            set { properties.Text = value; }
        }

        public string Outcome
        {
            get { return outcome.Text; }
            set { InvokeIfRequired(() => { outcome.Text = value; }); }
        }

        public string ElapsedTime
        {
            get { return elapsedTime.Text; }
            set { InvokeIfRequired(() => { elapsedTime.Text = value; }); }
        }

        public string AssertCount
        {
            get { return assertCount.Text; }
            set { InvokeIfRequired(() => { assertCount.Text = value; }); }
        }

        public string Message
        {
            get { return message.Text; }
            set { InvokeIfRequired(() => { message.Text = value; }); }
        }

        public string StackTrace
        {
            get { return stackTrace.Text; }
            set { InvokeIfRequired(() => { stackTrace.Text = value; }); }
        }

        public string Output
        {
            get { return output.Text; }
            set { InvokeIfRequired(() => { output.Text = value; }); }
        }

        public SplitContainer sp { get; set; }

        #region Helper Methods

        private void InvokeIfRequired(MethodInvoker _delegate)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(_delegate);
            else
                _delegate();
        }

        #endregion

        public void ApplyTheme()
        {
            sp = splitContainer1;
            output.BackColor = Color.White;
            properties.BackColor = Color.White;
            message.BackColor = Color.White;
            stackTrace.BackColor = Color.White;
            sp.Panel1.BackColor = Color.White;
            sp.Panel2.BackColor = Color.White;
        }
    }
}
