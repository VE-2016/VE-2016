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

    /// <summary>
    /// SettingsWrapper wraps an ISettings interface
    /// and is the base of all the classes that form
    /// the SettingsModel. SettingsWrapper may be used
    /// directly to take advantage of the fact that
    /// it encapsulates the prefix used for a given
    /// group of settings or through derived classes
    /// that provide properties for type-safe access
    /// to particular settings.
    /// </summary>
    public class SettingsWrapper
    {
        private string _prefix;

        public SettingsWrapper(ISettings settingsService, string prefix)
        {
            this.SettingsService = settingsService;
            _prefix = prefix;

            if (_prefix == null)
                _prefix = string.Empty;
            if (_prefix != string.Empty && !prefix.EndsWith("."))
                _prefix += ".";
        }

        protected ISettings SettingsService { get; private set; }

        public object GetSetting(string name)
        {
            return SettingsService.GetSetting(_prefix + name);
        }

        public T GetSetting<T>(string name, T defaultValue)
        {
            return SettingsService.GetSetting(_prefix + name, defaultValue);
        }

        public void SaveSetting(string name, object value)
        {
            name = _prefix + name;
            if (value == null)
                SettingsService.RemoveSetting(name);
            else
                SettingsService.SaveSetting(name, value);
        }

        public void RemoveSetting(string name)
        {
            SettingsService.RemoveSetting(_prefix + name);
        }
    }
}
