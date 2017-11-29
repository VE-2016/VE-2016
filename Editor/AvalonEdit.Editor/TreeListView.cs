using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace AvalonEdit.Editor
{
    public class TreeListView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        #region Public Properties

        /// <summary> GridViewColumn List</summary>
        public GridViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new GridViewColumnCollection();
                }

                return _columns;
            }
        }

        private GridViewColumnCollection _columns;

        #endregion Public Properties
    }

    public class TreeListViewItem : TreeViewItem
    {
        public TreeListViewItem()
        {
            this.Background = System.Windows.Media.Brushes.AliceBlue;

            this.MouseEnter += TreeListViewItem_MouseEnter;
            this.MouseLeave += TreeListViewItem_MouseLeave;
            this.MouseDown += TreeListViewItem_MouseDown;
        }

        private void TreeListViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsExpanded)
                this.IsExpanded = false;
            else this.IsExpanded = true;
        }

        private void TreeListViewItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (popup != null)
                popup.IsOpen = false;
        }

        private static Popup popup { get; set; }

        private void TreeListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Header == null)
                return;

            SourceRefData source = this.Header as SourceRefData;

            if (source == null)
                return;

            if (popup == null)
            {
                popup = new Popup();
                popup.Height = 100;
                popup.KeyDown += Popup_KeyDown;
                popup.Placement = PlacementMode.Mouse;
            }

            string lines = source.Lines;
            if (lines == null)
            {
                popup.IsOpen = false;
                e.Handled = false;
                return;
            }
            popup.Child = EditorWindow.staticEditorWindow;
            EditorWindow.LoadStaticContent(lines, TreeViewer.vs);
            EditorWindow.staticEditorWindow.Visibility = Visibility.Visible;
            popup.IsOpen = true;

            e.Handled = false;
        }

        private void Popup_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                popup.IsOpen = false;
            }
        }

        /// <summary>
        /// Item's hierarchy in the tree
        /// </summary>
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    TreeListViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as TreeListViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        private int _level = -1;
    }

    public class RichTextBlock : TextBlock
    {
        public RichTextBlock() : base()
        {
            this.Initialized += RichTextBlock_Initialized;
            TargetUpdated += RichTextBlock_TargetUpdated;
        }

        private void RichTextBlock_TargetUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            e.Handled = true;
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register
            (
                 "Data",
                 typeof(object),
                 typeof(RichTextBlock)

            );

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private void RichTextBlock_Initialized(object sender, EventArgs e)
        {
            object obs = this.Data;

            if (obs == null)
                return;

            if (!(obs is Array))
                return;

            object[] aw = (object[])obs;

            if (aw == null)
                return;

            List<string> kw = (List<string>)aw[0];
            if (kw == null)
                return;
            List<string> tw = (List<string>)aw[1];
            if (tw == null)
                return;

            this.Inlines.Clear();
            string text = Text.Trim();
            Text = "";
            var matches = Regex.Matches(text, @"\w+[^\s]*\w+|\w");

            int prev = 0;

            string s = "";
            foreach (Match match in matches)
            {
                var word = match.Value;
                int start = match.Index;
                int length = match.Length;

                if (prev < start)
                {
                    s = text.Substring(prev, start - prev);
                    Inlines.Add(new Run(s) { Foreground = Brushes.Black });
                }
                if (kw.Contains(word))
                {
                    Inlines.Add(new Run(word) { Foreground = Brushes.LightBlue });
                }
                else if (tw.Contains(word))
                {
                    Inlines.Add(new Run(word) { Foreground = Brushes.Blue });
                }
                else
                {
                    Inlines.Add(new Run(word) { Foreground = Brushes.Black });
                }

                prev = start + length;
            }
            if (prev < text.Length - 1)
            {
                s = text.Substring(prev, text.Length - prev);
                Inlines.Add(new Run(s) { Foreground = Brushes.Black });
            }
        }
    }
}