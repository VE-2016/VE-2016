
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

            MethodButton.Source = Convert(WinExplorers.ve.Method_purple_16x);
            FolderButton.Source = Convert(WinExplorers.ve.FolderOpen_16x);
            MethodButtons.Source = Convert(WinExplorers.ve.Method_purple_16x);

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