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
using System.IO;
using System.Text;
using System.Xml;

using Mono.Cecil;

namespace NUnit.Gui.Presenters
{
    using Model;
    using Views;

    public class XmlPresenter
    {
        private readonly IXmlView _view;
        private readonly ITestModel _model;

        private ITestItem _selectedItem;

        public XmlPresenter(IXmlView view, ITestModel model)
        {
            _view = view;
            _model = model;

            _view.Visible = false;

            WireUpEvents();
        }

        private void WireUpEvents()
        {
            _model.TestLoaded += (ea) => _view.Visible = true;
            _model.TestReloaded += (ea) => _view.Visible = true;
            _model.TestUnloaded += (ea) => _view.Visible = false;
            _model.RunFinished += (ea) => DisplayXml();
            _model.SelectedItemChanged += (ea) => OnSelectedItemChanged(ea.TestItem);

            _view.SelectAllCommand += () =>
            {
                _view.XmlTextBox.Control.Focus();
                _view.XmlTextBox.Control.SelectAll();
            };
            _view.SelectionChanged += () =>
            {
                _view.CopyToolStripMenuItem.ToolStripItem.Enabled = !string.IsNullOrEmpty(_view.XmlTextBox.Control.SelectedText);
            };
            _view.WordWrapChanged += () =>
            {
                _view.XmlTextBox.Control.WordWrap = _view.WordWrapToolStripMenuItem.ToolStripItem.Checked;
            };
            _view.CopyCommand += () =>
            {
                _view.XmlTextBox.Control.Copy();
            };
        }

        private void OnSelectedItemChanged(ITestItem testItem)
        {
            _selectedItem = testItem;
            DisplayXml();
        }

        private void DisplayXml()
        {
            var testNode = _selectedItem as TestNode;

            _view.XmlPanel.Visible = testNode != null;

            if (testNode != null)
            {
                _view.SuspendLayout();
                _view.Header = testNode.Name;
                _view.TestXml = GetFullXml(testNode);
                _view.ResumeLayout();
            }
            else if (_selectedItem != null)
            {
                _view.Header = _selectedItem.Name;
            }
        }

        private XmlNode GetFullXml(TestNode testNode)
        {
            ResultNode resultNode = _model.GetResultForTest(testNode);
            XmlNode currentXml;
            if (resultNode != null)
            {
                currentXml = resultNode.Xml.Clone();
                foreach (TestNode child in testNode.Children)
                {
                    XmlNode childXml = GetFullXml(child);
                    XmlNode importedChildXml = currentXml.OwnerDocument.ImportNode(childXml, true);
                    currentXml.AppendChild(importedChildXml);
                }
            }
            else
            {
                currentXml = testNode.Xml.Clone();
                foreach (TestNode child in testNode.Children)
                {
                    XmlNode childXml = GetFullXml(child);
                    XmlNode importedChildXml = currentXml.OwnerDocument.ImportNode(childXml, true);
                    var oldChild = FindXmlNode(currentXml, child);
                    if (oldChild != null)
                        currentXml.ReplaceChild(importedChildXml, oldChild);
                    else
                        currentXml.AppendChild(importedChildXml);
                }
            }
            return currentXml;
        }

        private static XmlNode FindXmlNode(XmlNode currentXml, TestNode testNodeChild)
        {
            foreach (XmlNode child in currentXml.ChildNodes)
            {
                if ((child.LocalName == "test-case" || child.LocalName == "test-suite")
                    && testNodeChild.FullName == child.Attributes["fullname"].Value)
                    return child;
            }
            return null;
        }
    }
}