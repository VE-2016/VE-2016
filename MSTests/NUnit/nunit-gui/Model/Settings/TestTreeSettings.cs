﻿// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

using System.Drawing;
using System.Windows.Forms;

namespace NUnit.Gui.Model.Settings
{
    using Engine;

    public class TestTreeSettings : SettingsWrapper
    {
        public TestTreeSettings(ISettings settingsService) : base(settingsService, "Gui.TestTree") { }

        public FixtureListSettings FixtureList
        {
            get { return new FixtureListSettings(SettingsService); }
        }

        public TestListSettings TestList
        {
            get { return new TestListSettings(SettingsService); }
        }

        private const string displayFormatKey = "DisplayFormat";
        public string DisplayFormat
        {
            get { return GetSetting(displayFormatKey, "NUNIT_TREE"); }
            set { SaveSetting(displayFormatKey, value); }
        }

        private const string initialTreeDisplayKey = "InitialTreeDisplay";
        public TreeDisplayStyle InitialTreeDisplay
        {
            get { return GetSetting(initialTreeDisplayKey, TreeDisplayStyle.Auto); }
            set { SaveSetting(initialTreeDisplayKey, value); }
        }

        private string _clearResultsOnReloadKey = "ClearResultsOnReload";
        public bool ClearResultsOnReload
        {
            get { return GetSetting(_clearResultsOnReloadKey, true); }
            set { SaveSetting(_clearResultsOnReloadKey, value); }
        }

        private const string saveVisualStateKey = "SaveVisualState";
        public bool SaveVisualState
        {
            get { return GetSetting(saveVisualStateKey, true); }
            set { SaveSetting(saveVisualStateKey, value); }
        }

        private const string showCheckBoxesKey = "ShowCheckBoxes";
        public bool ShowCheckBoxes
        {
            get { return GetSetting(showCheckBoxesKey, false); }
            set { SaveSetting(showCheckBoxesKey, value); }
        }

        private const string alternateImageSetKey = "AlternateImageSet";
        public string AlternateImageSet
        {
            get { return GetSetting(alternateImageSetKey, "Default"); }
            set { SaveSetting(alternateImageSetKey, value); }
        }

        private const string autoNamespaceSuitesKey = "AutoNamespaceSuites";
        public bool AutoNamespaceSuites
        {
            get { return GetSetting(autoNamespaceSuitesKey, true); }
            set { SaveSetting(autoNamespaceSuitesKey, value); }
        }
    }

    public class FixtureListSettings : SettingsWrapper
    {
        public FixtureListSettings(ISettings settings) : base(settings, "Gui.TestTree.FixtureList") { }

        private string _groupByKey = "GroupBy";
        public string GroupBy
        {
            get { return GetSetting(_groupByKey, "OUTCOME"); }
            set { SaveSetting(_groupByKey, value); }
        }
    }

    public class TestListSettings : SettingsWrapper
    {
        public TestListSettings(ISettings settings) : base(settings, "Gui.TestTree.TestList") { }

        private string _groupByKey = "GroupBy";
        public string GroupBy
        {
            get { return GetSetting(_groupByKey, "OUTCOME"); }
            set { SaveSetting(_groupByKey, value); }
        }
    }
}
