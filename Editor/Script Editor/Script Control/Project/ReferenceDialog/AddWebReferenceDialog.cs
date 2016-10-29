
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows.Forms;

//using ICSharpCode.Core;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using Microsoft.Win32;
using System.Collections.Specialized;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class AddWebReferenceDialog : System.Windows.Forms.Form
    {
        private WebServiceDiscoveryClientProtocol _discoveryClientProtocol;
        private CredentialCache _credentialCache = new CredentialCache();
        private string _namespacePrefix = String.Empty;
        private Uri _discoveryUri;
        private IProject _project;
        private WebReference _webReference;
        private ScriptLanguage _Language = ScriptLanguage.CSharp;
        private Label _label2;
        private TextBox _txtName;
        private StringCollection _CreatedFileNames = null;

        private delegate DiscoveryDocument DiscoverAnyAsync(string url);
        private delegate void DiscoveredWebServicesHandler(DiscoveryClientProtocol protocol);
        private delegate void AuthenticationHandler(Uri uri, string authenticationType);

        public AddWebReferenceDialog(IProject project, ScriptLanguage Lang, StringCollection CreatedFileNames)
        {
            _Language = Lang;
            _CreatedFileNames = CreatedFileNames;
            InitializeComponent();
            _txtName.Text = GenerateValidFileName();
            AddMruList();

            // fixes forum-16247: Add Web Reference dialog missing URL on 120 DPI
            AddWebReferenceDialogResize(null, null);
            _project = project;
        }

        /// <summary>
        /// The prefix that will be added to the web service's namespace
        /// (typically the project's namespace).
        /// </summary>
        public string NamespacePrefix
        {
            get
            {
                return _namespacePrefix;
            }
            set
            {
                _namespacePrefix = value;
            }
        }

        /// <summary>
        /// The discovered web reference to add to the project.
        /// </summary>
        public WebReference WebReference
        {
            get
            {
                return _webReference;
            }
        }

        public string WebReferenceFileName
        {
            get { return _txtName.Text; }
        }
        #region Windows Forms Designer generated code
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        /// 
        private void InitializeComponent()
        {
            _toolStrip = new System.Windows.Forms.ToolStrip();
            _backButton = new System.Windows.Forms.ToolStripButton();
            _forwardButton = new System.Windows.Forms.ToolStripButton();
            _refreshButton = new System.Windows.Forms.ToolStripButton();
            _stopButton = new System.Windows.Forms.ToolStripButton();
            _urlComboBox = new System.Windows.Forms.ToolStripComboBox();
            _goButton = new System.Windows.Forms.ToolStripButton();
            _tabControl = new System.Windows.Forms.TabControl();
            _webBrowserTabPage = new System.Windows.Forms.TabPage();
            _webBrowser = new System.Windows.Forms.WebBrowser();
            _webServicesTabPage = new System.Windows.Forms.TabPage();
            _webServicesView = new AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog.WebServicesView();
            _referenceNameLabel = new System.Windows.Forms.Label();
            _referenceNameTextBox = new System.Windows.Forms.TextBox();
            _addButton = new System.Windows.Forms.Button();
            _cancelButton = new System.Windows.Forms.Button();
            _namespaceTextBox = new System.Windows.Forms.TextBox();
            _namespaceLabel = new System.Windows.Forms.Label();
            _label2 = new System.Windows.Forms.Label();
            _txtName = new System.Windows.Forms.TextBox();
            _toolStrip.SuspendLayout();
            _tabControl.SuspendLayout();
            _webBrowserTabPage.SuspendLayout();
            _webServicesTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            _toolStrip.CanOverflow = false;
            _toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _backButton,
            _forwardButton,
            _refreshButton,
            _stopButton,
            _urlComboBox,
            _goButton});
            _toolStrip.Location = new System.Drawing.Point(0, 0);
            _toolStrip.Name = "toolStrip";
            _toolStrip.Size = new System.Drawing.Size(543, 25);
            _toolStrip.Stretch = true;
            _toolStrip.TabIndex = 0;
            _toolStrip.Text = "toolStrip";
            _toolStrip.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ToolStripPreviewKeyDown);
            _toolStrip.Enter += new System.EventHandler(this.ToolStripEnter);
            _toolStrip.Leave += new System.EventHandler(this.ToolStripLeave);
            // 
            // backButton
            // 
            _backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _backButton.Enabled = false;
            _backButton.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Icons_16x16_BrowserBefore;
            _backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _backButton.Name = "backButton";
            _backButton.Size = new System.Drawing.Size(23, 22);
            _backButton.Text = "Back";
            _backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // forwardButton
            // 
            _forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _forwardButton.Enabled = false;
            _forwardButton.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Icons_16x16_BrowserAfter;
            _forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _forwardButton.Name = "forwardButton";
            _forwardButton.Size = new System.Drawing.Size(23, 22);
            _forwardButton.Text = "forward";
            _forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
            // 
            // refreshButton
            // 
            _refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _refreshButton.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Icons_16x16_BrowserRefresh;
            _refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _refreshButton.Name = "refreshButton";
            _refreshButton.Size = new System.Drawing.Size(23, 22);
            _refreshButton.Text = "Refresh";
            _refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
            // 
            // stopButton
            // 
            _stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            _stopButton.Enabled = false;
            _stopButton.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Icons_16x16_BrowserCancel;
            _stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _stopButton.Name = "stopButton";
            _stopButton.Size = new System.Drawing.Size(23, 22);
            _stopButton.Text = "Stop";
            _stopButton.ToolTipText = "Stop";
            _stopButton.Click += new System.EventHandler(this.StopButtonClick);
            // 
            // urlComboBox
            // 
            _urlComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            _urlComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            _urlComboBox.AutoSize = false;
            _urlComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            _urlComboBox.Name = "urlComboBox";
            _urlComboBox.Size = new System.Drawing.Size(361, 21);
            _urlComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UrlComboBoxKeyDown);
            _urlComboBox.SelectedIndexChanged += new System.EventHandler(this.UrlComboBoxSelectedIndexChanged);
            // 
            // goButton
            // 
            _goButton.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Icons_16x16_RunProgramIcon;
            _goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            _goButton.Name = "goButton";
            _goButton.Size = new System.Drawing.Size(40, 22);
            _goButton.Text = "Go";
            _goButton.Click += new System.EventHandler(this.GoButtonClick);
            // 
            // tabControl
            // 
            _tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            _tabControl.Controls.Add(_webBrowserTabPage);
            _tabControl.Controls.Add(_webServicesTabPage);
            _tabControl.Location = new System.Drawing.Point(0, 28);
            _tabControl.Name = "tabControl";
            _tabControl.SelectedIndex = 0;
            _tabControl.Size = new System.Drawing.Size(543, 181);
            _tabControl.TabIndex = 1;
            // 
            // webBrowserTabPage
            // 
            _webBrowserTabPage.Controls.Add(_webBrowser);
            _webBrowserTabPage.Location = new System.Drawing.Point(4, 22);
            _webBrowserTabPage.Name = "webBrowserTabPage";
            _webBrowserTabPage.Padding = new System.Windows.Forms.Padding(3);
            _webBrowserTabPage.Size = new System.Drawing.Size(535, 155);
            _webBrowserTabPage.TabIndex = 0;
            _webBrowserTabPage.Text = "WSDL";
            _webBrowserTabPage.UseVisualStyleBackColor = true;
            // 
            // webBrowser
            // 
            _webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            _webBrowser.Location = new System.Drawing.Point(3, 3);
            _webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            _webBrowser.Name = "webBrowser";
            _webBrowser.Size = new System.Drawing.Size(529, 149);
            _webBrowser.TabIndex = 0;
            _webBrowser.TabStop = false;
            _webBrowser.CanGoForwardChanged += new System.EventHandler(this.WebBrowserCanGoForwardChanged);
            _webBrowser.CanGoBackChanged += new System.EventHandler(this.WebBrowserCanGoBackChanged);
            _webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.WebBrowserNavigated);
            _webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
            // 
            // webServicesTabPage
            // 
            _webServicesTabPage.Controls.Add(_webServicesView);
            _webServicesTabPage.Location = new System.Drawing.Point(4, 22);
            _webServicesTabPage.Name = "webServicesTabPage";
            _webServicesTabPage.Padding = new System.Windows.Forms.Padding(3);
            _webServicesTabPage.Size = new System.Drawing.Size(535, 155);
            _webServicesTabPage.TabIndex = 1;
            _webServicesTabPage.Text = "Available Web Services";
            _webServicesTabPage.UseVisualStyleBackColor = true;
            // 
            // webServicesView
            // 
            _webServicesView.Dock = System.Windows.Forms.DockStyle.Fill;
            _webServicesView.Location = new System.Drawing.Point(3, 3);
            _webServicesView.Name = "webServicesView";
            _webServicesView.Size = new System.Drawing.Size(529, 149);
            _webServicesView.TabIndex = 0;
            // 
            // referenceNameLabel
            // 
            _referenceNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            _referenceNameLabel.Location = new System.Drawing.Point(-26, 263);
            _referenceNameLabel.Name = "referenceNameLabel";
            _referenceNameLabel.Size = new System.Drawing.Size(20, 13);
            _referenceNameLabel.TabIndex = 2;
            _referenceNameLabel.Text = "&Reference Name:";
            _referenceNameLabel.UseCompatibleTextRendering = true;
            _referenceNameLabel.Visible = false;
            // 
            // referenceNameTextBox
            // 
            _referenceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            _referenceNameTextBox.Location = new System.Drawing.Point(0, 300);
            _referenceNameTextBox.Name = "referenceNameTextBox";
            _referenceNameTextBox.Size = new System.Drawing.Size(12, 20);
            _referenceNameTextBox.TabIndex = 4;
            _referenceNameTextBox.Visible = false;
            // 
            // addButton
            // 
            _addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            _addButton.Enabled = false;
            _addButton.Location = new System.Drawing.Point(468, 226);
            _addButton.Name = "addButton";
            _addButton.Size = new System.Drawing.Size(73, 21);
            _addButton.TabIndex = 6;
            _addButton.Text = "&Add";
            _addButton.UseCompatibleTextRendering = true;
            _addButton.UseVisualStyleBackColor = true;
            _addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // cancelButton
            // 
            _cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelButton.Location = new System.Drawing.Point(468, 251);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new System.Drawing.Size(73, 21);
            _cancelButton.TabIndex = 7;
            _cancelButton.Text = "Cancel";
            _cancelButton.UseCompatibleTextRendering = true;
            _cancelButton.UseVisualStyleBackColor = true;
            _cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // namespaceTextBox
            // 
            _namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            _namespaceTextBox.Location = new System.Drawing.Point(129, 253);
            _namespaceTextBox.Name = "namespaceTextBox";
            _namespaceTextBox.Size = new System.Drawing.Size(333, 20);
            _namespaceTextBox.TabIndex = 5;
            // 
            // namespaceLabel
            // 
            _namespaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _namespaceLabel.Location = new System.Drawing.Point(11, 252);
            _namespaceLabel.Name = "namespaceLabel";
            _namespaceLabel.Size = new System.Drawing.Size(128, 20);
            _namespaceLabel.TabIndex = 3;
            _namespaceLabel.Text = "&Namespace:";
            _namespaceLabel.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            _label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _label2.Location = new System.Drawing.Point(11, 227);
            _label2.Name = "label2";
            _label2.Size = new System.Drawing.Size(128, 20);
            _label2.TabIndex = 9;
            _label2.Text = "&File Name:";
            _label2.UseCompatibleTextRendering = true;
            // 
            // txtName
            // 
            _txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            _txtName.Location = new System.Drawing.Point(129, 226);
            _txtName.Name = "txtName";
            _txtName.Size = new System.Drawing.Size(333, 20);
            _txtName.TabIndex = 3;
            // 
            // AddWebReferenceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 285);
            this.Controls.Add(_txtName);
            this.Controls.Add(_label2);
            this.Controls.Add(_cancelButton);
            this.Controls.Add(_namespaceTextBox);
            this.Controls.Add(_namespaceLabel);
            this.Controls.Add(_addButton);
            this.Controls.Add(_referenceNameTextBox);
            this.Controls.Add(_referenceNameLabel);
            this.Controls.Add(_tabControl);
            this.Controls.Add(_toolStrip);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "AddWebReferenceDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Web Reference";
            this.Resize += new System.EventHandler(this.AddWebReferenceDialogResize);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddWebReferenceDialogFormClosing);
            _toolStrip.ResumeLayout(false);
            _toolStrip.PerformLayout();
            _tabControl.ResumeLayout(false);
            _webBrowserTabPage.ResumeLayout(false);
            _webServicesTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.Label _namespaceLabel;
        private System.Windows.Forms.TextBox _namespaceTextBox;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _addButton;
        private System.Windows.Forms.TextBox _referenceNameTextBox;
        private System.Windows.Forms.Label _referenceNameLabel;
        private System.Windows.Forms.TabPage _webBrowserTabPage;
        private System.Windows.Forms.TabPage _webServicesTabPage;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.WebBrowser _webBrowser;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.ToolStripButton _goButton;
        private System.Windows.Forms.ToolStripComboBox _urlComboBox;
        private System.Windows.Forms.ToolStripButton _stopButton;
        private System.Windows.Forms.ToolStripButton _refreshButton;
        private System.Windows.Forms.ToolStripButton _forwardButton;
        private System.Windows.Forms.ToolStripButton _backButton;
        private AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog.WebServicesView _webServicesView;
        #endregion

        private void AddMruList()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
                if (key != null)
                {
                    foreach (string name in key.GetValueNames())
                    {
                        _urlComboBox.Items.Add((string)key.GetValue(name));
                    }
                }
            }
            catch (Exception) { };
        }

        private string GenerateValidFileName()
        {
            string baseName = "";
            baseName = "WebReference";
            bool found = false;
            int filecounter = 1;
            string filename = "";
            while (found == false)
            {
                filename = baseName + filecounter.ToString() + (_Language == ScriptLanguage.CSharp ? ".cs" : ".vb");
                if (_CreatedFileNames != null)
                {
                    if (_CreatedFileNames.Contains(filename) == false)
                        break;
                }
                else
                    break;
                filecounter++;
            }
            return filename;
        }

        /// <summary>
        /// If the user presses the tab key, and the currently selected toolstrip
        /// item is at the end or the beginning of the toolstip, then force the
        /// tab to move to another control instead of staying on the toolstrip.
        /// </summary>
        private void ToolStripPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                if (_goButton.Selected && e.Modifiers != Keys.Shift)
                {
                    _toolStrip.TabStop = true;
                }
                else if (_backButton.Selected && e.Modifiers == Keys.Shift)
                {
                    _toolStrip.TabStop = true;
                }
            }
        }

        private void ToolStripEnter(object sender, EventArgs e)
        {
            _toolStrip.TabStop = false;
        }

        private void ToolStripLeave(object sender, EventArgs e)
        {
            _toolStrip.TabStop = true;
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            try
            {
                _webBrowser.GoBack();
            }
            catch (Exception) { }
        }

        private void ForwardButtonClick(object sender, System.EventArgs e)
        {
            try
            {
                _webBrowser.GoForward();
            }
            catch (Exception) { }
        }

        private void StopButtonClick(object sender, System.EventArgs e)
        {
            _webBrowser.Stop();
            StopDiscovery();
            _addButton.Enabled = false;
        }

        private void RefreshButtonClick(object sender, System.EventArgs e)
        {
            _webBrowser.Refresh();
        }

        private void GoButtonClick(object sender, System.EventArgs e)
        {
            BrowseUrl(_urlComboBox.Text);
        }

        private void BrowseUrl(string url)
        {
            _webBrowser.Focus();
            _webBrowser.Navigate(url);
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            _stopButton.Enabled = true;
            _webServicesView.Clear();
        }

        private void WebBrowserNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Cursor = Cursors.Default;
            _stopButton.Enabled = false;
            _urlComboBox.Text = _webBrowser.Url.ToString();
            StartDiscovery(e.Url);
        }

        private void WebBrowserCanGoForwardChanged(object sender, EventArgs e)
        {
            _forwardButton.Enabled = _webBrowser.CanGoForward;
        }

        private void WebBrowserCanGoBackChanged(object sender, EventArgs e)
        {
            _backButton.Enabled = _webBrowser.CanGoBack;
        }

        /// <summary>
        /// Gets the namespace to be used with the generated web reference code.
        /// </summary>
        private string GetDefaultNamespace()
        {
            if (_namespacePrefix.Length > 0 && _discoveryUri != null)
            {
                return String.Concat(_namespacePrefix, ".", _discoveryUri.Host);
            }
            else if (_discoveryUri != null)
            {
                return _discoveryUri.Host;
            }
            return String.Empty;
        }

        private string GetReferenceName()
        {
            if (_discoveryUri != null)
            {
                return _discoveryUri.Host;
            }
            return String.Empty;
        }

        private bool IsValidNamespace
        {
            get
            {
                bool valid = false;

                if (_namespaceTextBox.Text.Length > 0)
                {
                    // Can only begin with a letter or '_'
                    char ch = _namespaceTextBox.Text[0];
                    if (Char.IsLetter(ch) || (ch == '_'))
                    {
                        valid = true;
                        for (int i = 1; i < _namespaceTextBox.Text.Length; ++i)
                        {
                            ch = _namespaceTextBox.Text[i];
                            // Can only contain letters, digits or '_'
                            if (!Char.IsLetterOrDigit(ch) && (ch != '.') && (ch != '_'))
                            {
                                valid = false;
                                break;
                            }
                        }
                    }
                }

                return valid;
            }
        }

        private bool IsValidReferenceName
        {
            get
            {
                if (_referenceNameTextBox.Text.Length > 0)
                {
                    if (_referenceNameTextBox.Text.IndexOf('\\') == -1)
                    {
                        if (!ContainsInvalidDirectoryChar(_referenceNameTextBox.Text))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private bool ContainsInvalidDirectoryChar(string item)
        {
            foreach (char ch in Path.GetInvalidPathChars())
            {
                if (item.IndexOf(ch) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Starts the search for web services at the specified url.
        /// </summary>
        private void StartDiscovery(Uri uri)
        {
            StartDiscovery(uri, new DiscoveryNetworkCredential(CredentialCache.DefaultNetworkCredentials, DiscoveryNetworkCredential.DefaultAuthenticationType));
        }

        private void StartDiscovery(Uri uri, DiscoveryNetworkCredential credential)
        {
            // Abort previous discovery.
            StopDiscovery();

            // Start new discovery.
            _discoveryUri = uri;
            DiscoverAnyAsync asyncDelegate = new DiscoverAnyAsync(_discoveryClientProtocol.DiscoverAny);
            AsyncCallback callback = new AsyncCallback(DiscoveryCompleted);
            _discoveryClientProtocol.Credentials = credential;
            IAsyncResult result = asyncDelegate.BeginInvoke(uri.AbsoluteUri, callback, new AsyncDiscoveryState(_discoveryClientProtocol, uri, credential));
        }

        /// <summary>
        /// Called after an asynchronous web services search has
        /// completed.
        /// </summary>
        private void DiscoveryCompleted(IAsyncResult result)
        {
            AsyncDiscoveryState state = (AsyncDiscoveryState)result.AsyncState;
            WebServiceDiscoveryClientProtocol protocol = state.Protocol;

            // Check that we are still waiting for this particular callback.
            bool wanted = false;
            lock (this)
            {
                wanted = Object.ReferenceEquals(_discoveryClientProtocol, protocol);
            }

            if (wanted)
            {
                DiscoveredWebServicesHandler handler = new DiscoveredWebServicesHandler(DiscoveredWebServices);
                try
                {
                    DiscoverAnyAsync asyncDelegate = (DiscoverAnyAsync)((AsyncResult)result).AsyncDelegate;
                    DiscoveryDocument doc = asyncDelegate.EndInvoke(result);
                    if (!state.Credential.IsDefaultAuthenticationType)
                    {
                        AddCredential(state.Uri, state.Credential);
                    }
                    Invoke(handler, new object[] { protocol });
                }
                catch
                {
                    if (protocol.IsAuthenticationRequired)
                    {
                        HttpAuthenticationHeader authHeader = protocol.GetAuthenticationHeader();
                        AuthenticationHandler authHandler = new AuthenticationHandler(AuthenticateUser);
                        Invoke(authHandler, new object[] { state.Uri, authHeader.AuthenticationType });
                    }
                    else
                    {
                        //LoggingService.Error("DiscoveryCompleted", ex);
                        Invoke(handler, new object[] { null });
                    }
                }
            }
        }

        /// <summary>
        /// Stops any outstanding asynchronous discovery requests.
        /// </summary>
        private void StopDiscovery()
        {
            lock (this)
            {
                if (_discoveryClientProtocol != null)
                {
                    try
                    {
                        _discoveryClientProtocol.Abort();
                    }
                    catch (NotImplementedException)
                    {
                    }
                    catch (ObjectDisposedException)
                    {
                        // Receive this error if the url pointed to a file.
                        // The discovery client will already have closed the file
                        // so the abort fails.
                    }
                    _discoveryClientProtocol.Dispose();
                }
                _discoveryClientProtocol = new WebServiceDiscoveryClientProtocol();
            }
        }

        private void AddWebReferenceDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            StopDiscovery();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _urlComboBox.Focus();
        }

        private ServiceDescriptionCollection GetServiceDescriptions(DiscoveryClientProtocol protocol)
        {
            ServiceDescriptionCollection services = new ServiceDescriptionCollection();
            protocol.ResolveOneLevel();

            foreach (DictionaryEntry entry in protocol.References)
            {
                ContractReference contractRef = entry.Value as ContractReference;
                if (contractRef != null)
                {
                    services.Add(contractRef.Contract);
                }
            }
            return services;
        }

        private void DiscoveredWebServices(DiscoveryClientProtocol protocol)
        {
            if (protocol != null)
            {
                _addButton.Enabled = true;
                _namespaceTextBox.Text = GetDefaultNamespace();
                _referenceNameTextBox.Text = GetReferenceName();
                _webServicesView.Add(GetServiceDescriptions(protocol));
                _webReference = new WebReference(_project, _discoveryUri.AbsoluteUri, _referenceNameTextBox.Text, _namespaceTextBox.Text, protocol);
                _tabControl.SelectedTab = _webServicesTabPage;
            }
            else
            {
                _webReference = null;
                _addButton.Enabled = false;
                _webServicesView.Clear();
            }
        }

        private void UrlComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            BrowseUrl(_urlComboBox.Text);
        }

        private void UrlComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && _urlComboBox.Text.Length > 0)
            {
                BrowseUrl(_urlComboBox.Text);
            }
        }

        private void AddWebReferenceDialogResize(object sender, EventArgs e)
        {
            int width = _toolStrip.ClientSize.Width;
            foreach (ToolStripItem item in _toolStrip.Items)
            {
                if (item != _urlComboBox)
                    width -= item.Width + 8;
            }
            _urlComboBox.Width = width;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (!IsValidReferenceName)
                {
                    MessageBox.Show("Invalid Reference Name Error");
                    return;
                }

                if (!IsValidNamespace)
                {
                    MessageBox.Show("Invalid Namespace Error");
                    return;
                }

                if (_CreatedFileNames != null)
                {
                    if (_CreatedFileNames.Contains(_txtName.Text) == true)
                    {
                        MessageBox.Show(this, "A file with the name " + _txtName.Text + " already exists in the current project. Please give a unique name to the item you are adding , or delete the existing item first.", "AIMS Script Editor");
                        return;
                    }
                }

                _webReference.Name = _referenceNameTextBox.Text;
                _webReference.ProxyNamespace = _namespaceTextBox.Text;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void AuthenticateUser(Uri uri, string authenticationType)
        {
            DiscoveryNetworkCredential credential = (DiscoveryNetworkCredential)_credentialCache.GetCredential(uri, authenticationType);
            if (credential != null)
            {
                StartDiscovery(uri, credential);
            }
            else
            {
                using (UserCredentialsDialog credentialsForm = new UserCredentialsDialog(uri.ToString(), authenticationType))
                {
                    if (DialogResult.OK == credentialsForm.ShowDialog())
                    {
                        StartDiscovery(uri, credentialsForm.Credential);
                    }
                }
            }
        }

        private void AddCredential(Uri uri, DiscoveryNetworkCredential credential)
        {
            NetworkCredential matchedCredential = _credentialCache.GetCredential(uri, credential.AuthenticationType);
            if (matchedCredential != null)
            {
                _credentialCache.Remove(uri, credential.AuthenticationType);
            }
            _credentialCache.Add(uri, credential.AuthenticationType, credential);
        }
    }
}
