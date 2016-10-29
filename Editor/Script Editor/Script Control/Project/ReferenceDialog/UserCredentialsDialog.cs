
using System;
//using ICSharpCode.Core;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class UserCredentialsDialog : System.Windows.Forms.Form
    {
        private string _authenticationType = String.Empty;

        public UserCredentialsDialog(string url, string authenticationType)
        {
            InitializeComponent();
            _url.Text = url;
            _authenticationType = authenticationType;
            AddStringResources();
        }

        public DiscoveryNetworkCredential Credential
        {
            get
            {
                return new DiscoveryNetworkCredential(_userTextBox.Text, _passwordTextBox.Text, _domainTextBox.Text, _authenticationType);
            }
        }

        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            _urlLabel = new System.Windows.Forms.Label();
            _userNameLabel = new System.Windows.Forms.Label();
            _passwordLabel = new System.Windows.Forms.Label();
            _domainLabel = new System.Windows.Forms.Label();
            _userTextBox = new System.Windows.Forms.TextBox();
            _passwordTextBox = new System.Windows.Forms.TextBox();
            _domainTextBox = new System.Windows.Forms.TextBox();
            _url = new System.Windows.Forms.Label();
            _okButton = new System.Windows.Forms.Button();
            _cancelButton = new System.Windows.Forms.Button();
            _infoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // urlLabel
            // 
            _urlLabel.Location = new System.Drawing.Point(10, 59);
            _urlLabel.Name = "urlLabel";
            _urlLabel.Size = new System.Drawing.Size(91, 23);
            _urlLabel.TabIndex = 0;
            _urlLabel.Text = "Url:";
            _urlLabel.UseCompatibleTextRendering = true;
            // 
            // userNameLabel
            // 
            _userNameLabel.Location = new System.Drawing.Point(10, 88);
            _userNameLabel.Name = "userNameLabel";
            _userNameLabel.Size = new System.Drawing.Size(91, 23);
            _userNameLabel.TabIndex = 1;
            _userNameLabel.Text = "&User name:";
            _userNameLabel.UseCompatibleTextRendering = true;
            // 
            // passwordLabel
            // 
            _passwordLabel.Location = new System.Drawing.Point(10, 115);
            _passwordLabel.Name = "passwordLabel";
            _passwordLabel.Size = new System.Drawing.Size(91, 23);
            _passwordLabel.TabIndex = 3;
            _passwordLabel.Text = "&Password:";
            _passwordLabel.UseCompatibleTextRendering = true;
            // 
            // domainLabel
            // 
            _domainLabel.Location = new System.Drawing.Point(10, 142);
            _domainLabel.Name = "domainLabel";
            _domainLabel.Size = new System.Drawing.Size(91, 23);
            _domainLabel.TabIndex = 5;
            _domainLabel.Text = "&Domain:";
            _domainLabel.UseCompatibleTextRendering = true;
            // 
            // userTextBox
            // 
            _userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));
            _userTextBox.Location = new System.Drawing.Point(93, 85);
            _userTextBox.Name = "userTextBox";
            _userTextBox.Size = new System.Drawing.Size(187, 21);
            _userTextBox.TabIndex = 2;
            // 
            // passwordTextBox
            // 
            _passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));
            _passwordTextBox.Location = new System.Drawing.Point(93, 112);
            _passwordTextBox.Name = "passwordTextBox";
            _passwordTextBox.PasswordChar = '*';
            _passwordTextBox.Size = new System.Drawing.Size(187, 21);
            _passwordTextBox.TabIndex = 4;
            // 
            // domainTextBox
            // 
            _domainTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));
            _domainTextBox.Location = new System.Drawing.Point(93, 139);
            _domainTextBox.Name = "domainTextBox";
            _domainTextBox.Size = new System.Drawing.Size(187, 21);
            _domainTextBox.TabIndex = 6;
            // 
            // url
            // 
            _url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));
            _url.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            _url.Location = new System.Drawing.Point(93, 57);
            _url.Name = "url";
            _url.Size = new System.Drawing.Size(187, 21);
            _url.TabIndex = 9;
            _url.UseCompatibleTextRendering = true;
            // 
            // okButton
            // 
            _okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            _okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            _okButton.Location = new System.Drawing.Point(146, 166);
            _okButton.Name = "okButton";
            _okButton.Size = new System.Drawing.Size(64, 26);
            _okButton.TabIndex = 7;
            _okButton.Text = "OK";
            _okButton.UseCompatibleTextRendering = true;
            _okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            _cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelButton.Location = new System.Drawing.Point(216, 166);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new System.Drawing.Size(64, 26);
            _cancelButton.TabIndex = 8;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseCompatibleTextRendering = true;
            _cancelButton.UseVisualStyleBackColor = true;
            // 
            // infoLabel
            // 
            _infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                    | System.Windows.Forms.AnchorStyles.Right)));
            _infoLabel.Location = new System.Drawing.Point(12, 9);
            _infoLabel.Name = "infoLabel";
            _infoLabel.Size = new System.Drawing.Size(267, 48);
            _infoLabel.TabIndex = 10;
            _infoLabel.Text = "Please supply the credentials to access the specified url.";
            _infoLabel.UseCompatibleTextRendering = true;
            // 
            // UserCredentialsDialog
            // 
            this.AcceptButton = _okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = _cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 202);
            this.Controls.Add(_infoLabel);
            this.Controls.Add(_cancelButton);
            this.Controls.Add(_okButton);
            this.Controls.Add(_url);
            this.Controls.Add(_domainTextBox);
            this.Controls.Add(_passwordTextBox);
            this.Controls.Add(_userTextBox);
            this.Controls.Add(_domainLabel);
            this.Controls.Add(_passwordLabel);
            this.Controls.Add(_userNameLabel);
            this.Controls.Add(_urlLabel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 236);
            this.Name = "UserCredentialsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Discovery Credential";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label _infoLabel;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Label _url;
        private System.Windows.Forms.TextBox _domainTextBox;
        private System.Windows.Forms.TextBox _userTextBox;
        private System.Windows.Forms.Label _domainLabel;
        private System.Windows.Forms.Label _passwordLabel;
        private System.Windows.Forms.Label _urlLabel;
        #endregion

        private void AddStringResources()
        {
            Text = "Dialog Title";
            _infoLabel.Text = "Information";
            _urlLabel.Text = "Url";
            _userNameLabel.Text = "User Name";
            _passwordLabel.Text = "Password";
            _domainLabel.Text = "Domain";
            _cancelButton.Text = "Cancel";
            _okButton.Text = "Ok";
        }
    }
}
