
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using VSProvider;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class CallHierarchyViewer : UserControl
    {
        public CallHierarchyViewer()
        {
            InitializeComponent();
            

            RefreshButton.Source = Convert(WinExplorers.ve.Refresh_16x);
            CancelButton.Source = Convert(WinExplorers.ve.Cancel_16x);
            ButtonPanel.Source = Convert(WinExplorers.ve.ListMembers_16x);
            ButtonPanel.MouseDown += ButtonPanel_MouseDown;

            //MethodButton.Source = Convert(WinExplorers.ve.Method_purple_16x);
            //FolderButton.Source = Convert(WinExplorers.ve.FolderOpen_16x);
            //MethodButtons.Source = Convert(WinExplorers.ve.Method_purple_16x);

            this.LocationsListView.SizeChanged += TreeListView_SizeChanged;

            grid.SizeChanged += Grid_SizeChanged;

            GridViewColumn column = ((GridView)LocationsListView.View).Columns[0];

 ((System.ComponentModel.INotifyPropertyChanged)column).PropertyChanged += (sender, e) =>
 {
     if (e.PropertyName == "ActualWidth")
     {
         var workingWidth = this.LocationsListView.ActualWidth - 1;
         var col0 = ((GridView)LocationsListView.View).Columns[0].Width;
         var col1 = workingWidth - col0;
         ((GridView)LocationsListView.View).Columns[1].Width = col1;
     }
     
 };
            TreeViewItem v0 = CreateTreeViewItem(kind.method, "method");
            TreeViewItem v1 = CreateTreeViewItem(kind.folder, "folder");
            TreeViewItem v2 = CreateTreeViewItem(kind.none, "item");

            v0.Items.Add(v1);
            v1.Items.Add(v2);

            treeListView.Items.Add(v0);

            LocationsListView.SelectionChanged += LocationsListView_SelectionChanged;
        }

        private void LocationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = LocationsListView.SelectedItem;
            SourceRefData source = s as SourceRefData;
            if (source == null)
                return;
            editorWindow.vs.LoadFileFromProject("", null, source.FileName, null, null, source.Span);
        }

        public enum kind {

            method,
            folder,
            none
        }
        public TreeViewItem CreateTreeViewItem(kind d, string s)
        {
            TreeViewItem v = new TreeViewItem();
            StackPanel p = new StackPanel();
            p.Orientation = Orientation.Horizontal;
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            if(d == kind.method)
            image.Source = Convert(WinExplorers.ve.Method_purple_16x);
            else if(d == kind.folder)
                image.Source = Convert(WinExplorers.ve.FolderOpen_16x);
            else image.Source = Convert(WinExplorers.ve.Method_purple_16x);

            TextBlock b = new TextBlock();
            b.Text = s;
            p.Children.Add(image);
            p.Children.Add(b);

            v.Header = p;

            return v;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LocationsListView.InvalidateVisual();
        }

        int lastVisibleWidth = 0;
        private void ButtonPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                if(grid.ColumnDefinitions[2].Width.Value != 0)
            {
                lastVisibleWidth = (int)grid.ColumnDefinitions[0].ActualWidth;
                grid.ColumnDefinitions[2].Width = new GridLength(0.0);
            }   else
            {
                int w = (int)grid.ActualWidth;

                grid.ColumnDefinitions[0].Width = new GridLength(lastVisibleWidth);
                grid.ColumnDefinitions[2].Width = new GridLength(w - lastVisibleWidth - 3);
                grid.InvalidateVisual();
            }
        }

        private void TreeListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var workingWidth = this.LocationsListView.ActualWidth - 1; // take into account vertical scrollbar
            var col0 = 0.75;
            var col1 = 0.25;

            if (workingWidth <= 0)
                return;

            ((GridView)LocationsListView.View).Columns[0].Width = workingWidth * col0;
            ((GridView)LocationsListView.View).Columns[1].Width = workingWidth * col1;
            

        }

        public void LoadData(ISymbol symbol, List<ReferencedSymbol> b, VSSolution vs)
        {
            TreeViewItem v0 = CreateTreeViewItem(kind.method, "method");
            TreeViewItem v1 = CreateTreeViewItem(kind.folder, "folder");
            foreach (var s in b)
            {
                if (s.Locations != null)
                {
                    foreach (var pl in s.Locations)
                    {
                        if (!pl.Location.IsInSource)
                            continue;
                        SourceRefData d = new SourceRefData();
                        VSProject vp = vs.GetProjectbyCompileItem(pl.Location.SourceTree.FilePath);
                         {
                            int length = 10;
                            string cs = pl.Location.SourceTree.GetRoot().GetText().ToString();
                            var method = pl.Location.SourceTree.GetRoot().FindNode(pl.Location.SourceSpan).FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
                            {
                               
                                if (method != null)
                                {
                                    
                                    if (method is ConstructorDeclarationSyntax)
                                    {
                                        d.Offset = ((ConstructorDeclarationSyntax)method).Identifier.SpanStart;
                                        length = ((ConstructorDeclarationSyntax)method).Identifier.Span.Length;
                                    }
                                    else if (method is DestructorDeclarationSyntax)
                                    {
                                        d.Offset = ((DestructorDeclarationSyntax)method).Identifier.SpanStart;
                                        length = ((DestructorDeclarationSyntax)method).Identifier.Span.Length;
                                    }

                                    else
                                    {
                                        d.Offset = ((MethodDeclarationSyntax)method).Identifier.SpanStart;
                                        length = ((MethodDeclarationSyntax)method).Identifier.Span.Length;
                                    }
                                    var lineNumber = cs.Take(d.Offset).Count(c => c == '\n');
                                    d.name = cs.Split("\n".ToCharArray())[lineNumber].Trim();
                                    d.Line = lineNumber.ToString();
                                    d.Span = new Microsoft.CodeAnalysis.Text.TextSpan(lineNumber, length);
                                }
                                else
                                {

                                    var property = pl.Location.SourceTree.GetRoot().FindNode(pl.Location.SourceSpan).FirstAncestorOrSelf<PropertyDeclarationSyntax>();
                                    if (property != null)
                                    {

                                        d.Offset = ((PropertyDeclarationSyntax)property).Identifier.SpanStart;
                                        var lineNumber = cs.Take(d.Offset).Count(c => c == '\n');
                                        d.name = cs.Split("\n".ToCharArray())[lineNumber].Trim();
                                        d.Line = lineNumber.ToString();
                                        d.Span = new Microsoft.CodeAnalysis.Text.TextSpan(lineNumber, property.Identifier.Span.Length);
                                    }
                                    else
                                        continue;
                                }
                            }
                        }
                        string name = Path.GetFileNameWithoutExtension(vp.FileName);
                        int Line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        var Lines = pl.Location.SourceTree.GetText().Lines;
                        d.Name = pl.Location.SourceTree.GetText().Lines[Line].ToString().Trim();
                        
                        d.Project = Path.GetFileNameWithoutExtension(vp.FileName);
                        d.File = Path.GetFileName(pl.Location.SourceTree.FilePath);
                        d.FileName = pl.Location.SourceTree.FilePath;
                        int line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        string bcs = vs.GetSyntaxTreeText(pl.Location.SourceTree.FilePath);
                        line -= StringReplace.FindExtended(bcs, pl.Location.SourceSpan.Start);
                        d.Line = line.ToString();
                        d.Span = new Microsoft.CodeAnalysis.Text.TextSpan(line, pl.Location.SourceSpan.Length);
                        d.Column = pl.Location.GetLineSpan().StartLinePosition.Character.ToString();
                        d.refs = pl;
                        TreeViewItem v2 = CreateTreeViewItem(kind.none, d.name);
                        TreeViewItem v3 = CreateTreeViewItem(kind.folder, "Folder");
                        TreeViewItem v4 = CreateTreeViewItem(kind.folder, "Searching for calls");
                        v3.Items.Add(v4);
                        v3.Expanded += V3_Expanded;
                        v3.Tag = d;
                        v2.Items.Add(v3);
                        v2.Tag = d;
                        v2.Selected += V2_Selected;
                        v1.Items.Add(v2);
                    }
                }
            }
            v0.Items.Add(v1);
            treeListView.Items.Add(v0);
        }
        public void FindNextCalls(List<ReferencedSymbol> b, TreeViewItem v)
        {
            TreeViewItem v0 = CreateTreeViewItem(kind.method, "method");
            TreeViewItem v1 = CreateTreeViewItem(kind.folder, "folder");
            foreach (var s in b)
            {
                if (s.Locations != null)
                {
                    foreach (var pl in s.Locations)
                    {
                        if (!pl.Location.IsInSource)
                            continue;
                        SourceRefData d = new SourceRefData();
                        {
                            int length = 10;
                            string cs = pl.Location.SourceTree.GetRoot().GetText().ToString();
                            var method = pl.Location.SourceTree.GetRoot().FindNode(pl.Location.SourceSpan).FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
                            {
                                if (method is ConstructorDeclarationSyntax)
                                {
                                    d.Offset = ((ConstructorDeclarationSyntax)method).Identifier.SpanStart;
                                    length = ((ConstructorDeclarationSyntax)method).Identifier.Span.Length;
                                }
                                else if (method is DestructorDeclarationSyntax)
                                {
                                    d.Offset = ((DestructorDeclarationSyntax)method).Identifier.SpanStart;
                                    length = ((DestructorDeclarationSyntax)method).Identifier.Span.Length;
                                }
                                else if (method is MethodDeclarationSyntax)
                                {
                                    d.Offset = ((MethodDeclarationSyntax)method).Identifier.SpanStart;
                                    length = ((MethodDeclarationSyntax)method).Identifier.Span.Length;
                                }
                                else continue;

                                var lineNumber = cs.Take(d.Offset).Count(c => c == '\n');
                                d.name = cs.Split("\n".ToCharArray())[lineNumber].Trim();
                                d.Line = lineNumber.ToString();
                                d.Span = new Microsoft.CodeAnalysis.Text.TextSpan(lineNumber, length);
                            }
                        }
                        int Line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        var Lines = pl.Location.SourceTree.GetText().Lines;
                        d.Name = pl.Location.SourceTree.GetText().Lines[Line].ToString().Trim();
                        
                        d.File = Path.GetFileName(pl.Location.SourceTree.FilePath);
                        d.FileName = pl.Location.SourceTree.FilePath;
                        int line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        string bcs = pl.Location.SourceTree.GetRoot().GetText().ToString();
                        line -= StringReplace.FindExtended(bcs, pl.Location.SourceSpan.Start);
                        d.Line = line.ToString();
                        d.Column = pl.Location.GetLineSpan().StartLinePosition.Character.ToString();
                        d.refs = pl;
                        TreeViewItem v2 = CreateTreeViewItem(kind.none, d.name);
                        TreeViewItem v3 = CreateTreeViewItem(kind.folder, "Folder");
                        TreeViewItem v4 = CreateTreeViewItem(kind.folder, "Searching for calls");
                        v3.Items.Add(v4);
                        v2.Expanded += V3_Expanded;
                        v3.Tag = d;
                        v2.Items.Add(v3);
                        v2.Tag = d;
                        v2.Selected += V2_Selected;
                        v.Items.Add(v2);
                    }
                }
            }
         }

        private void V3_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem b = sender as TreeViewItem;
            if (b == null)
                return;
            SourceRefData source = b.Tag as SourceRefData;
            if (source == null)
                return;
            VSSolution vs = editorWindow.vs;

            var references = vs.FindReferences(source.Offset, source.FileName);
            if (references == null)
                return;

            b.Items.Clear();

            b.Header = "Calls to " + source.name;

            FindNextCalls(references.ToList(), b);

            e.Handled = true;
        }
        
        private void V2_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem v = sender as TreeViewItem;
            if (v == null)
                return;
            SourceRefData c = v.Tag as SourceRefData;
            if (c == null)
                return;
            ListViewItem b = new ListViewItem();

            List<SourceRefData> s = new List<SourceRefData>();
            s.Add(c);
            LocationsListView.ItemsSource = s;

            e.Handled = true;
        }

        public EditorWindow editorWindow { get; set; }

        public static BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            double w = this.ActualWidth;
            double h = this.ActualHeight;

            double wf = 300;
            double hf = 69;

            ff = new FindForm();
            ff.Width = wf;
            ff.Height = hf;
            
            Popup popup = new Popup();
            
            popup.Child = ff;
            popup.Placement = PlacementMode.Relative;
            popup.HorizontalOffset = w - wf - 15 - 2;
            popup.VerticalOffset = 20;
            popup.PlacementTarget = treeListView;
            popup.IsOpen = true;




        }

        FindForm ff { get; set; }

        static public ObservableCollection<Source> Sources = new ObservableCollection<Source>();

    }
   

  
}