
using System;
using System.Windows.Forms;
//using ICSharpCode.Core;
using AIMS.Libraries.Scripting.ScriptControl.Project;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class COMReferencePanel : ListView, IReferencePanel
    {
        private ISelectReferenceDialog _selectDialog;

        public COMReferencePanel(ISelectReferenceDialog selectDialog)
        {
            _selectDialog = selectDialog;

            this.Sorting = SortOrder.Ascending;


            ColumnHeader nameHeader = new ColumnHeader();
            nameHeader.Text = "Global.Name";
            nameHeader.Width = 240;
            Columns.Add(nameHeader);

            ColumnHeader directoryHeader = new ColumnHeader();
            directoryHeader.Text = "Global.Path";
            directoryHeader.Width = 200;
            Columns.Add(directoryHeader);

            View = View.Details;
            Dock = DockStyle.Fill;
            FullRowSelect = true;

            ItemActivate += delegate { AddReference(); };
        }

        public void AddReference()
        {
            foreach (ListViewItem item in SelectedItems)
            {
                TypeLibrary library = (TypeLibrary)item.Tag;
                _selectDialog.AddReference(ReferenceType.Typelib,
                                          library.Name,
                                          library.Path,
                                          library);
            }
        }

        private bool _populated;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (_populated == false && Visible)
            {
                _populated = true;
                PopulateListView();
            }
        }

        private void PopulateListView()
        {
            foreach (TypeLibrary typeLib in TypeLibrary.Libraries)
            {
                ListViewItem newItem = new ListViewItem(new string[] { typeLib.Description, typeLib.Path });
                newItem.Tag = typeLib;
                Items.Add(newItem);
            }
        }
    }
}
