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
    /// The explorer configuration page for the main options editor.
    /// </summary>
    public class ExplorerOptionsPage : OptionsPage
    {
        private SettingsManager _settingsManager;
        private Label _backupDirectoryLabel;
        private TextBox _backupDirectoryTextBox;
        private Label _backupDirectoryWarningLabel;
        private CheckBox _showHiddenFilesCheckBox;
        private CheckBox _showSystemFilesCheckBox;
        private CheckBox _showFullPathCheckBox;

        #region Form Control Names

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_backupDirectoryLabel = "backupDirectoryLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_backupDirectoryTextBox = "backupDirectoryTextBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        public const string m_backupDirectoryWarningLabel = "backupDirectoryWarningLabel";

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_showHiddenFilesCheckBox = "showHiddenFilesCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_showSystemFilesCheckBox = "showSystemFilesCheckBox";

        /// <summary>
        /// Form control name.
        /// </summary>
        private const string m_showFullPathCheckBox = "showFullPathCheckBox";

        #endregion Form Control Names

        /// <summary>
        /// Create the options page.
        /// </summary>
        public ExplorerOptionsPage()
        {
            _settingsManager = SettingsManager.GetInstance();

            //Name = Constants.UI_OPTIONS_PAGE_EXPLORER;
            //PageText = Resources.OptionsPageTextExplorer;
            //GroupText = Resources.OptionsGroupText;

            #region Form Layout

            _backupDirectoryLabel = new Label();
            _backupDirectoryLabel.AutoSize = true;
            _backupDirectoryLabel.Location = new System.Drawing.Point(0, 0);
            _backupDirectoryLabel.Name = m_backupDirectoryLabel;
            _backupDirectoryLabel.TabIndex = 0;
            _backupDirectoryLabel.Text = Resources.OptionsBackupDirectoryLabel;

            _backupDirectoryTextBox = new TextBox();
            _backupDirectoryTextBox.Location = new System.Drawing.Point(3, 16);
            _backupDirectoryTextBox.Name = m_backupDirectoryTextBox;
            _backupDirectoryTextBox.Size = new System.Drawing.Size(424, 20);
            _backupDirectoryTextBox.TabIndex = 1;

            _backupDirectoryWarningLabel = new Label();
            _backupDirectoryWarningLabel.AutoSize = true;
            _backupDirectoryWarningLabel.Location = new System.Drawing.Point(0, 46);
            _backupDirectoryWarningLabel.Name = m_backupDirectoryWarningLabel;
            _backupDirectoryWarningLabel.TabIndex = 2;
            _backupDirectoryWarningLabel.ForeColor = Color.Red;
            _backupDirectoryWarningLabel.Text = Resources.OptionsBackupDirectoryWarningLabel;
            _backupDirectoryWarningLabel.Visible = false;

            _showHiddenFilesCheckBox = new CheckBox();
            _showHiddenFilesCheckBox.AutoSize = true;
            _showHiddenFilesCheckBox.Location = new Point(3, 70);
            _showHiddenFilesCheckBox.Name = m_showHiddenFilesCheckBox;
            _showHiddenFilesCheckBox.TabIndex = 3;
            _showHiddenFilesCheckBox.Text = Resources.OptionsShowHiddenFiles;

            _showSystemFilesCheckBox = new CheckBox();
            _showSystemFilesCheckBox.AutoSize = true;
            _showSystemFilesCheckBox.Location = new Point(3, 93);
            _showSystemFilesCheckBox.Name = m_showSystemFilesCheckBox;
            _showSystemFilesCheckBox.TabIndex = 4;
            _showSystemFilesCheckBox.Text = Resources.OptionsShowSystemFiles;

            _showFullPathCheckBox = new CheckBox();
            _showFullPathCheckBox.AutoSize = true;
            _showFullPathCheckBox.Location = new Point(3, 116);
            _showFullPathCheckBox.Name = m_showFullPathCheckBox;
            _showFullPathCheckBox.TabIndex = 5;
            _showFullPathCheckBox.Text = Resources.OptionsShowFullPath;

            Controls.Add(_backupDirectoryLabel);
            Controls.Add(_backupDirectoryTextBox);
            Controls.Add(_backupDirectoryWarningLabel);
            Controls.Add(_showHiddenFilesCheckBox);
            Controls.Add(_showSystemFilesCheckBox);

            //if (ApplicationManager.GetInstance().
            //    ClientProfile.HaveFlag(ClientFlags.ExplorerEnableTitleBarUpdate))
            //    Controls.Add(_showFullPathCheckBox);

            #endregion Form Layout

            _backupDirectoryTextBox.Text = _settingsManager.BackupDirectory;

            if (_backupDirectoryTextBox.Text != String.Empty &&
                !Directory.Exists(_backupDirectoryTextBox.Text))
                _backupDirectoryWarningLabel.Visible = true;

            _showHiddenFilesCheckBox.Checked = _settingsManager.ShowHiddenFiles;
            _showSystemFilesCheckBox.Checked = _settingsManager.ShowSystemFiles;
            _showFullPathCheckBox.Checked = _settingsManager.ShowFullPath;
        }

        /// <summary>
        /// Save the settings when requested by the options editor.
        /// </summary>
        public void Save()
        {
            _settingsManager.BackupDirectory = _backupDirectoryTextBox.Text.Trim();
            _settingsManager.ShowHiddenFiles = _showHiddenFilesCheckBox.Checked;
            _settingsManager.ShowSystemFiles = _showSystemFilesCheckBox.Checked;
            _settingsManager.ShowFullPath = _showFullPathCheckBox.Checked;
        }
    }
}