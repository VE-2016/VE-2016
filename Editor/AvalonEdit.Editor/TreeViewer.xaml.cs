
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VSProvider;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic
    /// 
    /// /// </summary>

    public partial class TreeViewer : UserControl
    {

        static public VSSolution vs { get; set; }

        public TreeViewer()
        {
            InitializeComponent();

            treeListView.SelectedItemChanged += TreeListView_SelectedItemChanged;
            
            ButtonCopy.MouseDown += Button_Click;

            ButtonCopy.Source = Convert(WinExplorers.ve.Copy_32x);
            ButtonPrevList.Source = Convert(WinExplorers.ve.ListMembers_16x);
            ButtonNextList.Source = Convert(WinExplorers.ve.ListMembers_16x);
            ButtonFilterClear.Source = Convert(WinExplorers.ve.Filter_16x);
            ButtonKeepResults.Source = Convert(WinExplorers.ve.Filter_16x);

            this.treeListView.SizeChanged += TreeListView_SizeChanged;

            this.treeListView.SelectedItemChanged += TreeListView_SelectedItemChanged;

            treeList = treeListView;
            
        }

        public TreeListView treeList { get; set; }

        public EditorWindow editorWindow { get; set; }

        private void TreeListView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var s = e.NewValue;

            if (editorWindow == null)
                return;
            var source = s as SourceRefData;

            if (source == null)
                return;
            editorWindow.OpenFile( source);

            this.treeListView.Focus();
        }

        public void LoadData(ISymbol symbol, List<ReferencedSymbol> b, VSSolution vs)
        {

            List<string> tw = editorWindow.GetTypesNames();
            List<string> kw = (List<string>)editorWindow.Words;

            object[] aw = new object[2];
            aw[0] = tw;
            aw[1] = kw;


            Dictionary<string, List<SourceData>> dict = new Dictionary<string, List<SourceData>>();


            //Sources.Clear();

            Sources = new ObservableCollection<Source>();

            Source c = new Source();
            c.Name = symbol.Name;

            List<SourceRefData> data = new List<SourceRefData>();

           // c.Data = data;

            foreach (var s in b)
            {
                if (s.Locations != null)
                {

                    foreach (var pl in s.Locations)
                    {
                        
                        if (!pl.Location.IsInSource)
                            continue;
                       VSProject vp = vs.GetProjectbyCompileItem(pl.Location.SourceTree.FilePath);
                        string name = Path.GetFileNameWithoutExtension(vp.FileName);

                        if (dict.ContainsKey(name))
                        {
                            List<SourceData> datasource = dict[name];
                            List<SourceRefData> dataref = datasource[0].Data;
                            data = dataref;// dataref[0].Data;
                        }
                        else
                        {
                            c = new Source();
                            c.Name = name;
                            List<SourceData> datasource = new List<SourceData>();
                            c.Data = datasource;
                            SourceData sourcedata = new SourceData();
                            sourcedata.Name = s.Definition.ToDisplayString();
                            datasource.Add(sourcedata);
                            List<SourceRefData> dataref = new List<SourceRefData>();
                            sourcedata.Data = dataref;



                            data = dataref;

                           

                
                            dict.Add(name, datasource);

                         

                            Sources.Add(c);
                        }
                        
                        SourceRefData d = new SourceRefData();
                        int Line = pl.Location.GetLineSpan().StartLinePosition.Line;
                        var Lines = pl.Location.SourceTree.GetText().Lines;
                        d.Name = pl.Location.SourceTree.GetText().Lines[Line].ToString();
                        List<string> list = new List<string>();
                        int min = Int32.MaxValue;
                        string lines = "";
                        int i = Line - 3;
                        if (i < 0)
                            Line = 0;
                        while(i <= Line)
                        {
                            string loc = Lines[i].ToString();
                            
                            loc = loc.Replace("\t", " ");
                            int pos = loc.TakeWhile(sa => char.IsWhiteSpace(sa)).Count();
                            if (pos < min)
                                min = pos;
                            //lines += Lines[i].ToString().TrimStart() + "\n";
                            list.Add(loc + "\n");
                            i++;
                        }
                        i = Line + 1;
                        while(i <= Line + 3 && i < Lines.Count - 1)
                        {
                            string loc = Lines[i].ToString();
                            loc = loc.Replace("\t", " ");
                            int pos = loc.TakeWhile(sa => char.IsWhiteSpace(sa)).Count();
                            if (pos < min)
                                min = pos;
                            list.Add(loc + "\n");
                            //lines += Lines[i].ToString().TrimStart() + "\n";
                            i++;
                        }
                        foreach (string st in list)
                            lines += " " + st.Substring(min, st.Length - min);
                        d.Lines = lines;
                        d.Project = Path.GetFileNameWithoutExtension(vp.FileName);
                        d.File = Path.GetFileName(pl.Location.SourceTree.FilePath);
                        d.FileName = pl.Location.SourceTree.FilePath;
                        d.Words = (object)aw;
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
                
            

            }

           

            treeListView.ItemsSource = Sources;

            ExpandAll();
        }

        private void ExpandAll(ItemsControl items, bool expand)
        {
            foreach (object obj in items.Items)
            {
                ItemsControl childControl = items.ItemContainerGenerator.ContainerFromItem(obj) as ItemsControl;
                if (childControl != null)
                {
                    ExpandAll(childControl, expand);
                }
                TreeViewItem item = childControl as TreeViewItem;
                if (item != null)
                    item.IsExpanded = true;
            }
        }


        private void ExpandAll()
        {

            foreach (object item in this.treeListView.Items)
            {
                var treeItem = this.treeListView.ItemContainerGenerator.ContainerFromItem(item);
                //if (treeItem != null)
                //    ExpandAll(treeItem, true);
                //treeItem.IsExpanded = true;
            }
        }

        private void TreeListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            var workingWidth = this.treeListView.ActualWidth-5;
            if (workingWidth <= 0)
                return;
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

        private void Expander_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b == null)
                return;
            b.IsChecked = !b.IsChecked;
        }
    }


    //public class Source: INotifyPropertyChanged
    //{
    //    public string Name { get; set; }

    //    public string File { get; set; }

    //    public string Line { get; set; }

    //    public string Column { get; set; }

    //    public string Project { get; set; }

    //private List<Source> data;

    //public List<Source> Data {

    //    get { return data; }
    //    set
    //    {
    //        data = value;
    //        OnPropertyChanged("Data");
    //    }

    //}

    //public event PropertyChangedEventHandler PropertyChanged;
    //protected void OnPropertyChanged(String info)
    //{
    //    PropertyChangedEventHandler handler = PropertyChanged;
    //    if (handler != null)
    //    {
    //        handler(this, new PropertyChangedEventArgs(info));
    //    }
    //}

    //}
    public class Source :  INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Lines { get; set; }

        public ReferenceLocation? refs { get; set; }

        public object words { get; set; }

        public object Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }

        public string File { get; set; }

        public string FileName { get; set; }

        public string Line { get; set; }

        public string Column { get; set; }

        public string Project { get; set; }

        private List<SourceData> data;

        public List<SourceData> Data
        {

            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("Data");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }


    }
    public class SourceData : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Lines { get; set; }

        public ReferenceLocation? refs { get; set; }

        public object words { get; set; }

        public object Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }

        public string File { get; set; }

        public string FileName { get; set; }

        public string Line { get; set; }

        public string Column { get; set; }

        public string Project { get; set; }

        private List<SourceRefData> data;

        public List<SourceRefData> Data
        {

            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("Data");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }


    }
    public class SourceRefData : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string name { get; set; }

        public string Lines { get; set; }

        public int Offset { get; set; }

        
        public ReferenceLocation? refs { get; set; }

        public object words { get; set; }

        public object Words
        {
            get { return words; }
            set
            {
                words = value;
                OnPropertyChanged("Words");
            }
        }

        public string File { get; set; }

        public string FileName { get; set; }

        public string Line { get; set; }

        public TextSpan Span { get; set; }

        public string Column { get; set; }

        public string Project { get; set; }

        private List<SourceRefData> data;

        public List<SourceRefData> Data
        {

            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("Data");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
           
        }


    }

}