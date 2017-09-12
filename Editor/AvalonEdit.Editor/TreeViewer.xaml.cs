
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
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

    public partial class TreeViewer : UserControl
    {
        public TreeViewer()
        {
            InitializeComponent();
            TreeListViewItem v = new TreeListViewItem();
            v.Header = "String";


            Source c = new Source();
            c.Name = "test";
            
            
            SourceData d = new SourceData();
            d.Name = "class window";
            
            d.Project = "Project";
            d.File = "file";
            d.Line = "10";
            d.Column = "50";

            List<SourceData> data = new List<SourceData>();
            data.Add(d);

            d = new SourceData();
            d.Name = "class control";
            
            d.Project = "Project-2";
            d.File = "file-2";
            d.Line = "100";
            d.Column = "500";
            data.Add(d);

            c.Data = data;

            Sources.Add(c);

         

            TreeListViewItem p = new TreeListViewItem();
            p.ItemsSource = data;

            treeListView.ItemsSource = Sources;

            ButtonCopy.MouseDown += Button_Click;

            ButtonCopy.Source = Convert(WinExplorers.ve.Copy_32x);
            ButtonPrevList.Source = Convert(WinExplorers.ve.ListMembers_16x);
            ButtonNextList.Source = Convert(WinExplorers.ve.ListMembers_16x);
            ButtonFilterClear.Source = Convert(WinExplorers.ve.Filter_16x);
            ButtonKeepResults.Source = Convert(WinExplorers.ve.Filter_16x);

            this.treeListView.SizeChanged += TreeListView_SizeChanged;

            this.treeListView.SelectedItemChanged += TreeListView_SelectedItemChanged;

            treeList = treeListView;

            treeListView.Height = 200;
            treeListView.Width = 900;
        }

        public TreeListView treeList { get; set; }

        public EditorWindow editorWindow { get; set; }

        private void TreeListView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var s = e.NewValue;

            if (editorWindow == null)
                return;
            var source = s as SourceData;

            if (source == null)
                return;
            editorWindow.OpenFile( source);
        }

        public void LoadData(ISymbol symbol, List<ReferencedSymbol> b, VSSolution vs)
        {

            Dictionary<string, List<SourceData>> dict = new Dictionary<string, List<SourceData>>();


            Sources.Clear();

            Source c = new Source();
            c.Name = symbol.Name;

            List<SourceData> data = new List<SourceData>();

            c.Data = data;

            foreach (var s in b)
            {
                if (s.Locations != null)
                {

                    foreach (var pl in s.Locations)
                    {

                       VSProject vp = vs.GetProjectbyCompileItem(pl.Location.SourceTree.FilePath);
                        string name = Path.GetFileNameWithoutExtension(vp.FileName);

                        if (dict.ContainsKey(name))
                        {
                            data = dict[name];
                        }
                        else
                        {
                            c = new Source();
                            c.Name = name;

                            data = new List<SourceData>();

                            c.Data = data;
                            
                            dict.Add(name, data);

                            Sources.Add(c);
                        }
                        
                        SourceData d = new SourceData();
                        d.Name = s.Definition.Name;
                        d.Project = Path.GetFileName(vp.FileName);
                        d.File = pl.Location.SourceTree.FilePath;
                        int line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        //line = editorWindow.textEditor.Document.GetLineByNumber(line).LineNumberExtended;

                        string cs = vs.GetSyntaxTreeText(pl.Location.SourceTree.FilePath);

                        line -= StringReplace.FindExtended(cs, pl.Location.SourceSpan.Start);

                        //int nw = StringReplace.FindExtended(cs, pl.Location.SourceSpan.Start);

                        d.Line = line.ToString();
                        d.Column = pl.Location.GetLineSpan().StartLinePosition.Character.ToString();
                        d.refs = pl;
                        data.Add(d);
                       
                    }
                }
                
                //d = new SourceData();
                //d.Name = "class control";
                //d.Project = "Project-2";
                //d.File = "file-2";
                //d.Line = "100";
                //d.Column = "500";
                //data.Add(d);

            }

           

            treeListView.ItemsSource = Sources;

        }

        private void TreeListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            var workingWidth = this.treeListView.ActualWidth - 35; // take into account vertical scrollbar
            var col0 = 0.25;
            var col1 = 0.25;
            var col2 = 0.10;
            var col3 = 0.10;
            var col4 = 0.30;
            //var col5 = 0.20;

            treeListView.Columns[0].Width = workingWidth * col0;
            treeListView.Columns[1].Width = workingWidth * col1;
            treeListView.Columns[2].Width = workingWidth * col2;
            treeListView.Columns[3].Width = workingWidth * col3;
            treeListView.Columns[4].Width = workingWidth * col4;
            //treeListView.Columns[5].Width = workingWidth * col5;
        }

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
   

    public class Source
    {
        public string Name { get; set; }

        public string File { get; set; }

        public string Line { get; set; }

        public string Column { get; set; }

        public string Project { get; set; }

        public List<SourceData> Data { get; set; }

    }
    public class SourceData
    {
        public string Name { get; set; }

        public ReferenceLocation? refs { get; set; }

        public string ToName()
        {

            string name = Name;

            string[] dd = Name.Split(" ".ToCharArray());

            if (dd == null || dd.Length <= 1)
                return name;

            name = @"< Span Foreground=""Yellow"" >" + dd[0] + @" </Span>< Span Foreground=""Yellow"" >" + dd[1] + "</Span>";
 
            Name = name;

            return name;

        }

        public string File { get; set; }

        public string Line { get; set; }

        public string Column { get; set; }

        public string Project { get; set; }

    }
}