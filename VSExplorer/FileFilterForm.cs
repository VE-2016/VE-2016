/*
 * QuickSharp Copyright (C) 2008-2012 Steve Walker.
 *
 * This file is part of QuickSharp.
 *
 * QuickSharp is free software: you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free
 * Software Foundation, either version 3 of the License, or (at your option)
 * any later version.
 *
 * QuickSharp is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with QuickSharp. If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class FileFilterForm : Form
    {
        private List<FileFilter> _filters;
        private string _filterId;

        public FileFilterForm(List<FileFilter> filters, string id)
        {
            InitializeComponent();

            _filters = filters;
            _filterId = id;

            if (id != null)
            {
                FileFilter filter =
                    FileFilter.GetFromList(_filters, _filterId);

                nameTextBox.Text = filter.Name;
                filterTextBox.Text = filter.Filter;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            nameTextBox.Text = nameTextBox.Text.Trim();
            filterTextBox.Text = filterTextBox.Text.Trim();

            // Validate

            if (nameTextBox.Text == String.Empty)
            {
                nameTextBox.BackColor = Color.Yellow;
                return;
            }

            // Save

            if (_filterId == null)
            {
                FileFilter filter = new FileFilter();
                filter.ID = Guid.NewGuid().ToString();
                filter.Name = nameTextBox.Text;
                filter.Filter = filterTextBox.Text;
                _filters.Add(filter);
            }
            else
            {
                FileFilter filter =
                    FileFilter.GetFromList(_filters, _filterId);

                filter.Name = nameTextBox.Text;
                filter.Filter = filterTextBox.Text;
            }

            DialogResult = DialogResult.OK;
        }
    }
}