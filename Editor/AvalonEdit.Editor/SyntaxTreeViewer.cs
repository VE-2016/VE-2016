// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using VSProvider;

namespace AvalonEdit.Editor
{
    public partial class SyntaxTreeViewer : UserControl
    {
        public string Text { get; set; }
        public string Hyperlink { get; set; }
        public ReferenceLocation Location { get; set; }

        public SyntaxTree syntaxTree { get; set; }

        public ObservableCollection<Nodes> nodes
        {
            get;

            set;
        }

        public SyntaxTreeViewer()
        {
            InitializeComponent();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;

            treeView.MouseRightButtonDown += TreeView_MouseRightButtonDown;

            legendButton.Click += LegendButton_Click;

            CreateContextMenu();

            treeView.ContextMenu = contextMenu;
        }

        private void TreeView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (treeView.SelectedItem != null)
            {
                contextMenu.IsOpen = true; ;
            }
        }

        public void UpdateProperties(Nodes node)
        {
            SyntaxNode s = node.syntaxNode.AsNode();

            List<DataItem> b = new List<DataItem>();

            if (s != null)
            {
                PropertyInfo[] ps = s.GetType().GetProperties();

                foreach (PropertyInfo p in ps)
                {
                    DataItem dd = new DataItem();
                    dd.Key = p.Name;
                    object obs = p.GetValue(s);
                    if (obs != null)
                    {
                        dd.Value = obs.ToString();
                        b.Add(dd);
                    }
                }
                dg.ItemsSource = b;
                return;
            }
            SyntaxToken t = node.syntaxNode.AsToken();
            if (t != null)
            {
                PropertyInfo[] ps = t.GetType().GetProperties();

                foreach (PropertyInfo p in ps)
                {
                    DataItem dd = new DataItem();
                    dd.Key = p.Name;
                    object obs = p.GetValue(t);
                    if (obs != null)
                    {
                        dd.Value = obs.ToString();
                        b.Add(dd);
                    }
                }
                dg.ItemsSource = b;
                return;
            }

            DataItem d = new DataItem();
            d.Key = "ContainsAnnotations";
            d.Value = node.syntaxNode.ContainsAnnotations.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "ContainsDiagnostics";
            d.Value = node.syntaxNode.ContainsDiagnostics.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "ContainsDirecives";
            d.Value = node.syntaxNode.ContainsDirectives.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "FullSpan";
            d.Value = "[" + node.syntaxNode.SpanStart.ToString() + "..." + (node.syntaxNode.SpanStart + node.syntaxNode.Span.Length).ToString() + "]";
            b.Add(d);

            d = new DataItem();
            d.Key = "HasLeadingTrivia";
            d.Value = node.syntaxNode.HasLeadingTrivia.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "HasTrailingTrivia";
            d.Value = node.syntaxNode.HasTrailingTrivia.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "IsMissing";
            d.Value = node.syntaxNode.IsMissing.ToString();
            b.Add(d);

            d = new DataItem();
            d.Key = "Language";
            d.Value = node.syntaxNode.Language;
            b.Add(d);

            dg.ItemsSource = b;
        }

        private Popup popup { get; set; }

        private void LegendButton_Click(object sender, RoutedEventArgs e)
        {
            popup = new Popup();

            ListView b = new ListView();

            StackPanel s = new StackPanel();
            s.Orientation = Orientation.Horizontal;
            Label c = new Label();
            c.Content = "Blue";
            c.Foreground = Brushes.Blue;
            c.Width = 100;
            c.MouseDown += C_MouseDown;
            Label d = new Label();
            d.Content = "SyntaxNode";
            d.Width = 100;
            d.MouseDown += D_MouseDown;

            s.Children.Add(c);
            s.Children.Add(d);
            b.Items.Add(s);

            s = new StackPanel();
            s.Orientation = Orientation.Horizontal;
            c = new Label();
            c.Content = "Green";
            c.Foreground = Brushes.Green;
            c.Width = 100;
            d = new Label();
            d.Content = "SyntaxToken";
            d.Width = 140;

            s.Children.Add(c);
            s.Children.Add(d);
            b.Items.Add(s);

            s = new StackPanel();
            s.Orientation = Orientation.Horizontal;
            c = new Label();
            c.Content = "Lead:Maroon";
            c.Foreground = Brushes.Maroon;
            c.Width = 100;
            d = new Label();
            d.Content = "Leading Syntax Trivia";
            d.Width = 140;

            s.Children.Add(c);
            s.Children.Add(d);
            b.Items.Add(s);

            s = new StackPanel();
            s.Orientation = Orientation.Horizontal;
            c = new Label();
            c.Content = "Trail:Maroon";
            c.Foreground = Brushes.Maroon;
            c.Width = 100;
            d = new Label();
            d.Content = "Trailing Syntax Trivia";
            d.Width = 140;

            s.Children.Add(c);
            s.Children.Add(d);
            b.Items.Add(s);

            s = new StackPanel();
            s.Orientation = Orientation.Horizontal;
            c = new Label();
            c.Content = "Pink";
            c.Background = Brushes.Pink;
            c.Width = 100;
            d = new Label();
            d.Content = "Has Diagnostic";
            d.Width = 140;

            s.Children.Add(c);
            s.Children.Add(d);
            b.Items.Add(s);

            popup.StaysOpen = false;

            popup.Child = b;

            popup.Placement = PlacementMode.Relative;
            popup.PlacementTarget = legendButton;

            popup.IsOpen = true;
        }

        private void D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var d = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            foreach (MethodDeclarationSyntax es in d)
            {
                string name = es.Identifier.ToString();
                ClassDeclarationSyntax node = es.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                string Name = node.Identifier.ToString();
            }
            var f = syntaxTree.GetRoot().DescendantNodes().OfType<ConstructorDeclarationSyntax>().ToList();
            foreach (ConstructorDeclarationSyntax es in f)
            {
            }
        }

        private ContextMenu contextMenu { get; set; }

        public ContextMenu CreateContextMenu()
        {
            contextMenu = new ContextMenu();
            MenuItem me = new MenuItem();
            me.Header = "View Directed Syntax Graph";
            contextMenu.Items.Add(me);
            contextMenu.Items.Add(new Separator());
            me = new MenuItem();
            me.Header = "View Symbol (if any)";
            contextMenu.Items.Add(me);
            me = new MenuItem();
            me.Header = "View TypeSymbol (if any)";
            contextMenu.Items.Add(me);
            me = new MenuItem();
            me.Header = "View Converted TypeSymbol (if any)";
            contextMenu.Items.Add(me);
            me = new MenuItem();
            me.Header = "View AliasSymbol (if any)";
            contextMenu.Items.Add(me);
            contextMenu.Items.Add(new Separator());
            me = new MenuItem();
            me.Header = "View Constant Value (if any)";
            contextMenu.Items.Add(me);

            return contextMenu;
        }

        private void C_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (syntaxTree == null)
                return;
            var b = syntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();

            foreach (InvocationExpressionSyntax es in b)
            {
                if (es.Expression is MemberAccessExpressionSyntax)
                {
                    MemberAccessExpressionSyntax sa = es.Expression as MemberAccessExpressionSyntax;
                    string name = sa.Name.ToString();
                    string member = sa.Expression.ToString();
                }
            }
            //var c = syntaxTree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>().ToList();

            //foreach (MemberDeclarationSyntax es in c)
            //{
            //}

            XElement xe = Roslyn.SyntaxVisualizer.DgmlHelper.SyntaxDgmlHelper.ToDgml(syntaxTree.GetRoot());

            VSSolution.OpenSyntaxGraphTool(xe);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Nodes node = treeView.SelectedItem as Nodes;
            if (node == null)
                return;
            if (node.syntaxNode.IsNode)
                typeTextBlock.Text = "SyntaxNode";
            else typeTextBlock.Text = "TokenNode";
            kindTextBlock.Text = node.Kind.ToString();
            UpdateProperties(node);
        }

        public void LoadSyntaxTree(SyntaxTree s)
        {
            if (s == null)
                return;

            syntaxTree = s;

            SyntaxNode tree = s.GetRoot();

            var d = tree.DescendantNodesAndTokensAndSelf();

            Nodes ng = SyntaxNodeToNodes(s.GetRoot());

            Nodes node = new Nodes();
            node.Kind = SyntaxKind.CompilationUnit;
            node.Language = node.Kind.ToString() + " [" + 0 + "..." + (s.Length - 1) + "]";
            node.syntaxNode = s.GetRoot();

            node.nodes.Add(ng);
            List<Nodes> b = new List<Nodes>();
            b.Add(ng);

            nodes = new ObservableCollection<Nodes>(b);

            treeView.ItemsSource = nodes;
            treeView.InvalidateVisual();
        }

        private Nodes SyntaxNodeToNodes(SyntaxNode s)
        {
            Nodes node = new Nodes();
            node.Kind = s.Kind();
            node.Language = s.Kind().ToString() + " [" + s.SpanStart + "..." + s.Span.End + "]";
            node.syntaxNode = s;

            if (s.HasLeadingTrivia)
            {
                SyntaxTriviaList d = s.GetLeadingTrivia();
                foreach (SyntaxTrivia t in d)
                {
                    Nodes tr = new Nodes();
                    tr.Language = "Leading: " + t.ToString().Replace("\n", "") + " [" + t.SpanStart + "..." + t.Span.End + "]";
                    node.nodes.Add(tr);
                }
                node.IsLeadingTrivia = true;
            }
            if (s.HasTrailingTrivia)
            {
                SyntaxTriviaList d = s.GetTrailingTrivia();
                foreach (SyntaxTrivia t in d)
                {
                    Nodes tr = new Nodes();
                    tr.Language = "Trailing: " + t.ToString().Replace("\n", "") + " [" + t.SpanStart + "..." + t.Span.End + "]";
                    node.nodes.Add(tr);
                }

                node.IsTrailingTrivia = true;
            }
            foreach (SyntaxNodeOrToken c in s.ChildNodesAndTokens())
            {
                Nodes ng = SyntaxNodeToNodes(c);
                node.nodes.Add(ng);
            }
            return node;
        }

        private Nodes SyntaxNodeToNodes(SyntaxNodeOrToken s)
        {
            Nodes node = new Nodes();
            node.Kind = s.Kind();
            node.Language = s.Kind().ToString() + " [" + s.SpanStart + "..." + s.Span.End + "]";
            node.syntaxNode = s;

            if (s.HasLeadingTrivia)
            {
                SyntaxTriviaList d = s.GetLeadingTrivia();
                foreach (SyntaxTrivia t in d)
                {
                    Nodes tr = new Nodes();
                    tr.Language = "Leading: " + t.ToString() + " [" + t.SpanStart + "..." + t.Span.End + "]";
                    node.nodes.Add(tr);
                }
                node.IsLeadingTrivia = true;
            }
            if (s.HasTrailingTrivia)
            {
                SyntaxTriviaList d = s.GetTrailingTrivia();
                foreach (SyntaxTrivia t in d)
                {
                    Nodes tr = new Nodes();
                    tr.Language = "Trailing: " + t.ToString() + " [" + t.SpanStart + "..." + t.Span.End + "]";
                    node.nodes.Add(tr);
                }

                node.IsTrailingTrivia = true;
            }
            foreach (SyntaxNodeOrToken c in s.ChildNodesAndTokens())
            {
                Nodes ng = SyntaxNodeToNodes(c);
                node.nodes.Add(ng);
            }
            return node;
        }
    }

    public class Nodes
    {
        public Nodes()
        {
            nodes = new ObservableCollection<Nodes>();
        }

        public string Language { get; set; }
        public SyntaxKind Kind { get; set; }
        public ObservableCollection<Nodes> nodes { get; set; }
        public SyntaxNodeOrToken syntaxNode { get; set; }
        public bool IsLeadingTrivia = false;
        public bool IsTrailingTrivia = false;
    }

    public class TextBlocks : TextBlock
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register
       (
            "Data",
            typeof(Nodes),
            typeof(TextBlocks)

       );

        public Nodes Data
        {
            get { return (Nodes)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public TextBlocks()
        {
            this.Initialized += TextBlocks_Initialized;
        }

        private void TextBlocks_Initialized(object sender, EventArgs e)
        {
            if (Data == null)
                return;
            if (Data.syntaxNode == null)
                return;
            if (Data.syntaxNode.IsNode)
            {
                if (Data.IsTrailingTrivia || Data.IsLeadingTrivia)
                    this.Foreground = Brushes.Maroon;
                else
                    this.Foreground = Brushes.Blue;
            }
            else
            {
                if (Data.IsTrailingTrivia || Data.IsLeadingTrivia)
                    this.Foreground = Brushes.Maroon;
                else
                    this.Foreground = Brushes.Green;
            }
        }
    }

    public class DataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}