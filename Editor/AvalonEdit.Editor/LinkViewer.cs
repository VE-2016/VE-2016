﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
//using ICSharpCode.WpfDesign.UIExtensions;

namespace AvalonEdit.Editor
{
    public class LinkItem
    {
        public string Text { get; set; }
        public string Hyperlink { get; set; }

        public LinkItem(string text, string hyperlink)
        {
            Text = text;
            Hyperlink = hyperlink;
        }
    }
    public partial class LinkViewer : Window
    {

        public LinkViewer()
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
        }

        public List<LinkItem> LinkItems { get; set; }

        public void Load(List<LinkItem> v)
        {
            LinkItems = v;
            itemsListBox.ItemsSource = v;
        }

    }
    public class StringReplace
    {
        public static string Replace(string source)
        {
            
            Regex reg = new Regex(@"\  References");
            MatchEvaluator eval = match =>
            {
                switch (match.Value)
                {
                    case "  References": return "//References";
                    default: throw new Exception("Unexpected match!");
                }
            };
            return reg.Replace(source, eval);
            
        }
    }
}