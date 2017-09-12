using System;
using System.Windows.Controls;
using System.Windows;
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

        #endregion
    }

    public class TreeListViewItem : TreeViewItem
    {


        public TreeListViewItem()
        {
            this.Background = System.Windows.Media.Brushes.AliceBlue;
            // create stack panel
            
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
        }

        private void RichTextBlock_Initialized(object sender, EventArgs e)
        {
            string[] dd = Text.Split(" ".ToCharArray());

            if (dd == null || dd.Length <= 1)
                return;

            this.Inlines.Clear();
            
            Inlines.Add(new Run(dd[0]) { Foreground = Brushes.LightBlue });
            
            Inlines.Add(new Run(" " + dd[1]) { Foreground = Brushes.Blue });
        }
    }
}
