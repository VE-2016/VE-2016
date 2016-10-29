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

using QuickSharp.Core;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer
{
    /// <summary>
    /// The explorer window rename file/folder form.
    /// </summary>
    public partial class RenameForm : Form
    {
        /// <summary>
        /// Create a new form.
        /// </summary>
        public RenameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The new name for the item to be renamed. Sets the initial
        /// value and provides the new name on exit.
        /// </summary>
        public string NewName
        {
            get { return _newNameTextBox.Text; }
            set { _newNameTextBox.Text = value; }
        }

        /// <summary>
        /// The form title.
        /// </summary>
        public string Title
        {
            set { Text = value; }
        }

        private void RenameForm_Load(object sender, EventArgs e)
        {
            string filename = Path.GetFileNameWithoutExtension(
                _newNameTextBox.Text);

            _newNameTextBox.SelectionStart = 0;
            _newNameTextBox.SelectionLength = filename.Length;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _newNameTextBox.Text = _newNameTextBox.Text.Trim();

            if (FileTools.FilenameIsInvalid(_newNameTextBox.Text))
            {
                _newNameTextBox.BackColor = Color.Yellow;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}