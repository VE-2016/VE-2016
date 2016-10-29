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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;


namespace WinExplorer
{
    partial class FileFilterForm
    {
        private IContainer components = null;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label filterLabel;
        private TextBox filterTextBox;
        private Button okButton;
        private Button cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            nameLabel = new Label();
            nameTextBox = new TextBox();
            filterLabel = new Label();
            filterTextBox = new TextBox();
            okButton = new Button();
            cancelButton = new Button();
            SuspendLayout();

            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(10, 10);
            nameLabel.Name = "nameLabel";
            nameLabel.TabIndex = 0;
            nameLabel.Text = Resources.FilterFormNameLabel;

            nameTextBox.Location = new Point(13, 26);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(380, 21);
            nameTextBox.TabIndex = 1;

            filterLabel.AutoSize = true;
            filterLabel.Location = new Point(10, 50);
            filterLabel.Name = "filterLabel";
            filterLabel.TabIndex = 2;
            filterLabel.Text = Resources.FilterFormFilterLabel;

            filterTextBox.Location = new Point(13, 66);
            filterTextBox.Name = "filterTextBox";
            filterTextBox.Size = new Size(380, 21);
            filterTextBox.TabIndex = 3;

            okButton.Location = new Point(238, 112);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 4;
            okButton.Text = Resources.FilterFormOK;
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += new EventHandler(okButton_Click);

            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(319, 112);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.TabIndex = 5;
            cancelButton.Text = Resources.FilterFormCancel;
            cancelButton.UseVisualStyleBackColor = true;

            Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;

            AcceptButton = okButton;
            CancelButton = cancelButton;
            ClientSize = new Size(408, 147);
            Controls.Add(cancelButton);
            Controls.Add(okButton);
            Controls.Add(filterTextBox);
            Controls.Add(filterLabel);
            Controls.Add(nameTextBox);
            Controls.Add(nameLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FileFilterForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = Resources.FilterFormTitle;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}