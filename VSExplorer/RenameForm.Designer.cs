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

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinExplorer
{
    partial class RenameForm
    {
        private IContainer _components = null;
        private Button _okButton;
        private Button _cancelButton;
        private Label _newNameLabel;
        private TextBox _newNameTextBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            _okButton = new Button();
            _cancelButton = new Button();
            _newNameLabel = new Label();
            _newNameTextBox = new TextBox();

            SuspendLayout();

            _okButton.Location = new Point(126, 83);
            _okButton.Name = "okButton";
            _okButton.Size = new Size(75, 23);
            _okButton.TabIndex = 2;
            _okButton.Text = Resources.RenameButtonOK;
            _okButton.UseVisualStyleBackColor = true;
            _okButton.Click += new System.EventHandler(okButton_Click);

            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.Location = new Point(207, 83);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new Size(75, 23);
            _cancelButton.TabIndex = 3;
            _cancelButton.Text = Resources.RenameButtonCancel;
            _cancelButton.UseVisualStyleBackColor = true;

            _newNameLabel.AutoSize = true;
            _newNameLabel.Location = new Point(12, 9);
            _newNameLabel.Name = "newNameLabel";
            _newNameLabel.TabIndex = 0;
            _newNameLabel.Text = "New name:";

            _newNameTextBox.Location = new Point(15, 25);
            _newNameTextBox.Name = "newNameTextBox";
            _newNameTextBox.Size = new Size(267, 21);
            _newNameTextBox.TabIndex = 1;

            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;

            AcceptButton = _okButton;
            CancelButton = _cancelButton;
            ClientSize = new Size(294, 118);
            Controls.Add(_newNameTextBox);
            Controls.Add(_newNameLabel);
            Controls.Add(_cancelButton);
            Controls.Add(_okButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RenameForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rename";
            Load += new System.EventHandler(RenameForm_Load);

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}