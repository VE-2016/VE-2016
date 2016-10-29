using System;
using System.Collections.Generic;
using System.Windows.Forms;

//using ICSharpCode.Core;
using AIMS.Libraries.Scripting.Dom;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class GacReferencePanel : UserControl, IReferencePanel
    {
        private class ColumnSorter : System.Collections.IComparer
        {
            private int _column = 0;
            private bool _asc = true;

            public int CurrentColumn
            {
                get
                {
                    return _column;
                }
                set
                {
                    if (_column == value) _asc = !_asc;
                    else _column = value;
                }
            }

            public int Compare(object x, object y)
            {
                ListViewItem rowA = (ListViewItem)x;
                ListViewItem rowB = (ListViewItem)y;
                int result = String.Compare(rowA.SubItems[CurrentColumn].Text, rowB.SubItems[CurrentColumn].Text);
                if (_asc) return result;
                else return result * -1;
            }
        }

        protected ListView listView;
        private CheckBox _chooseSpecificVersionCheckBox;
        private ISelectReferenceDialog _selectDialog;
        private ColumnSorter _sorter;

        public GacReferencePanel(ISelectReferenceDialog selectDialog)
        {
            listView = new ListView();
            _sorter = new ColumnSorter();
            listView.ListViewItemSorter = _sorter;

            _selectDialog = selectDialog;

            ColumnHeader referenceHeader = new ColumnHeader();
            referenceHeader.Text = "Dialog.SelectReferenceDialog.GacReferencePanel.ReferenceHeader";
            referenceHeader.Width = 180;
            listView.Columns.Add(referenceHeader);

            listView.Sorting = SortOrder.Ascending;

            ColumnHeader versionHeader = new ColumnHeader();
            versionHeader.Text = "Dialog.SelectReferenceDialog.GacReferencePanel.VersionHeader";
            versionHeader.Width = 70;
            listView.Columns.Add(versionHeader);

            ColumnHeader pathHeader = new ColumnHeader();
            pathHeader.Text = "Global.Path";
            pathHeader.Width = 100;
            listView.Columns.Add(pathHeader);

            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.ItemActivate += delegate { AddReference(); };
            listView.ColumnClick += new ColumnClickEventHandler(columnClick);

            listView.Dock = DockStyle.Fill;
            this.Dock = DockStyle.Fill;
            this.Controls.Add(listView);

            _chooseSpecificVersionCheckBox = new CheckBox();
            _chooseSpecificVersionCheckBox.Dock = DockStyle.Top;
            _chooseSpecificVersionCheckBox.Text = "ChooseSpecificAssemblyVersion}";
            this.Controls.Add(_chooseSpecificVersionCheckBox);
            _chooseSpecificVersionCheckBox.CheckedChanged += delegate
            {
                listView.Items.Clear();
                if (_chooseSpecificVersionCheckBox.Checked)
                    listView.Items.AddRange(_fullItemList);
                else
                    listView.Items.AddRange(_shortItemList);
            };

            PrintCache();
        }

        private void columnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column < 2)
            {
                _sorter.CurrentColumn = e.Column;
                listView.Sort();
            }
        }

        public void AddReference()
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                _selectDialog.AddReference(ReferenceType.Gac,
                                          item.Text,
                                          _chooseSpecificVersionCheckBox.Checked ? item.Tag.ToString() : item.Text,
                                          null);
            }
        }

        private ListViewItem[] _fullItemList;
        /// <summary>
        /// Item list where older versions are filtered out.
        /// </summary>
        private ListViewItem[] _shortItemList;

        private void PrintCache()
        {
            List<ListViewItem> itemList = GetCacheContent();

            _fullItemList = itemList.ToArray();
            // Remove all items where a higher version exists
            itemList.RemoveAll(delegate (ListViewItem item)
            {
                return itemList.Exists(delegate (ListViewItem item2)
                {
                    return string.Equals(item.Text, item2.Text, StringComparison.OrdinalIgnoreCase)
                        && new Version(item.SubItems[1].Text) < new Version(item2.SubItems[1].Text);
                });
            });
            _shortItemList = itemList.ToArray();

            listView.Items.AddRange(_shortItemList);
        }

        protected virtual List<ListViewItem> GetCacheContent()
        {
            List<ListViewItem> itemList = new List<ListViewItem>();
            foreach (GacInterop.AssemblyListEntry asm in GacInterop.GetAssemblyList())
            {
                ListViewItem item = new ListViewItem(new string[] { asm.Name, asm.Version });
                item.Tag = asm.FullName;
                itemList.Add(item);
            }
            return itemList;
        }

        #region IReferencePanel Members

        void IReferencePanel.AddReference()
        {
            AddReference();
        }

        #endregion
    }
}
