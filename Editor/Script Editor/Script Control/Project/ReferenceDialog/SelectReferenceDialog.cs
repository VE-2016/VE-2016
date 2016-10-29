using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

//using ICSharpCode.Core;
using AIMS.Libraries.Scripting.ScriptControl.Project;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public interface IReferencePanel
    {
        void AddReference();
    }

    public interface ISelectReferenceDialog
    {
        void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation, object tag);
    }

    public enum ReferenceType
    {
        Assembly,
        Typelib,
        Gac,

        Project
    }

    /// <summary>
    /// Summary description for Form2.
    /// </summary>
    public class SelectReferenceDialog : System.Windows.Forms.Form, ISelectReferenceDialog
    {
        private System.Windows.Forms.ListView _referencesListView;
        private System.Windows.Forms.Button _selectButton;
        private System.Windows.Forms.Button _removeButton;
        private System.Windows.Forms.TabPage _gacTabPage;
        private System.Windows.Forms.TabPage _webTabPage;
        private System.Windows.Forms.TabPage _browserTabPage;
        private System.Windows.Forms.TabPage _comTabPage;
        private System.Windows.Forms.Label _referencesLabel;
        private System.Windows.Forms.ColumnHeader _referenceHeader;
        private System.Windows.Forms.ColumnHeader _typeHeader;
        private System.Windows.Forms.ColumnHeader _locationHeader;
        private System.Windows.Forms.TabControl _referenceTabControl;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _helpButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container _components = null;

        private IProject _configureProject;


        public ArrayList ReferenceInformations
        {
            get
            {
                ArrayList referenceInformations = new ArrayList();
                foreach (ListViewItem item in _referencesListView.Items)
                {
                    System.Diagnostics.Debug.Assert(item.Tag != null);
                    referenceInformations.Add(item.Tag);
                }
                return referenceInformations;
            }
        }

        public IProject ConfigureProject
        {
            get { return _configureProject; }
            set
            {
                _referencesListView.Items.Clear();
                _configureProject = value;
            }
        }
        public SelectReferenceDialog(IProject configureProject)
        {
            _configureProject = configureProject;

            InitializeComponent();
            _gacTabPage.Controls.Add(new GacReferencePanel(this));
            _browserTabPage.Controls.Add(new AssemblyReferencePanel(this));
            _comTabPage.Controls.Add(new COMReferencePanel(this));
        }

        public void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation, object tag)
        {
            foreach (ListViewItem item in _referencesListView.Items)
            {
                if (referenceLocation == item.SubItems[2].Text && referenceName == item.Text)
                {
                    return;
                }
            }

            ListViewItem newItem = new ListViewItem(new string[] { referenceName, referenceType.ToString(), referenceLocation });
            switch (referenceType)
            {
                case ReferenceType.Typelib:
                    newItem.Tag = new ComReferenceProjectItem(_configureProject, (TypeLibrary)tag);
                    break;
                case ReferenceType.Project:
                    newItem.Tag = new ProjectReferenceProjectItem(_configureProject, (IProject)tag);
                    break;
                case ReferenceType.Gac:
                    newItem.Tag = new ReferenceProjectItem(_configureProject, referenceLocation);
                    break;
                case ReferenceType.Assembly:
                    ReferenceProjectItem assemblyReference = new ReferenceProjectItem(_configureProject);
                    assemblyReference.Include = Path.GetFileNameWithoutExtension(referenceLocation);
                    assemblyReference.HintPath = FileUtility.GetRelativePath(_configureProject.Directory, referenceLocation);
                    assemblyReference.SpecificVersion = false;
                    newItem.Tag = assemblyReference;
                    break;
                default:
                    throw new System.NotSupportedException("Unknown reference type:" + referenceType);
            }

            _referencesListView.Items.Add(newItem);
        }

        private void SelectReference(object sender, EventArgs e)
        {
            IReferencePanel refPanel = (IReferencePanel)_referenceTabControl.SelectedTab.Controls[0];
            refPanel.AddReference();
        }

        private void OkButtonClick(object sender, EventArgs e)
        {
            if (_referencesListView.Items.Count == 0)
            {
                SelectReference(sender, e);
            }
        }

        private void RemoveReference(object sender, EventArgs e)
        {
            ArrayList itemsToDelete = new ArrayList();

            foreach (ListViewItem item in _referencesListView.SelectedItems)
            {
                itemsToDelete.Add(item);
            }

            foreach (ListViewItem item in itemsToDelete)
            {
                _referencesListView.Items.Remove(item);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    _components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _referenceTabControl = new System.Windows.Forms.TabControl();
            _gacTabPage = new System.Windows.Forms.TabPage();
            _comTabPage = new System.Windows.Forms.TabPage();
            _webTabPage = new System.Windows.Forms.TabPage();
            _browserTabPage = new System.Windows.Forms.TabPage();
            _referencesListView = new System.Windows.Forms.ListView();
            _referenceHeader = new System.Windows.Forms.ColumnHeader();
            _typeHeader = new System.Windows.Forms.ColumnHeader();
            _locationHeader = new System.Windows.Forms.ColumnHeader();
            _selectButton = new System.Windows.Forms.Button();
            _removeButton = new System.Windows.Forms.Button();
            _referencesLabel = new System.Windows.Forms.Label();
            _okButton = new System.Windows.Forms.Button();
            _cancelButton = new System.Windows.Forms.Button();
            _helpButton = new System.Windows.Forms.Button();
            _referenceTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // referenceTabControl
            // 
            _referenceTabControl.Controls.Add(_gacTabPage);
            _referenceTabControl.Controls.Add(_comTabPage);
            _referenceTabControl.Controls.Add(_webTabPage);
            _referenceTabControl.Controls.Add(_browserTabPage);
            _referenceTabControl.Location = new System.Drawing.Point(8, 8);
            _referenceTabControl.Name = "referenceTabControl";
            _referenceTabControl.SelectedIndex = 0;
            _referenceTabControl.Size = new System.Drawing.Size(472, 224);
            _referenceTabControl.TabIndex = 0;
            // 
            // gacTabPage
            // 
            _gacTabPage.Location = new System.Drawing.Point(4, 22);
            _gacTabPage.Name = "gacTabPage";
            _gacTabPage.Size = new System.Drawing.Size(464, 198);
            _gacTabPage.TabIndex = 0;
            _gacTabPage.Text = ".Net";
            _gacTabPage.UseVisualStyleBackColor = true;
            // 
            // comTabPage
            // 
            _comTabPage.Location = new System.Drawing.Point(4, 22);
            _comTabPage.Name = "comTabPage";
            _comTabPage.Size = new System.Drawing.Size(464, 198);
            _comTabPage.TabIndex = 2;
            _comTabPage.Text = "COM";
            _comTabPage.UseVisualStyleBackColor = true;
            // 
            // webTabPage
            // 
            _webTabPage.Location = new System.Drawing.Point(4, 22);
            _webTabPage.Name = "webTabPage";
            _webTabPage.Size = new System.Drawing.Size(464, 198);
            _webTabPage.TabIndex = 1;
            _webTabPage.Text = "Web";
            _webTabPage.UseVisualStyleBackColor = true;
            // 
            // browserTabPage
            // 
            _browserTabPage.Location = new System.Drawing.Point(4, 22);
            _browserTabPage.Name = "browserTabPage";
            _browserTabPage.Size = new System.Drawing.Size(464, 198);
            _browserTabPage.TabIndex = 2;
            _browserTabPage.Text = "Browse";
            _browserTabPage.UseVisualStyleBackColor = true;
            // 
            // referencesListView
            // 
            _referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            _referenceHeader,
            _typeHeader,
            _locationHeader});
            _referencesListView.FullRowSelect = true;
            _referencesListView.Location = new System.Drawing.Point(8, 256);
            _referencesListView.Name = "referencesListView";
            _referencesListView.Size = new System.Drawing.Size(472, 97);
            _referencesListView.TabIndex = 3;
            _referencesListView.UseCompatibleStateImageBehavior = false;
            _referencesListView.View = System.Windows.Forms.View.Details;
            // 
            // referenceHeader
            // 
            _referenceHeader.Text = "Reference";
            _referenceHeader.Width = 183;
            // 
            // typeHeader
            // 
            _typeHeader.Text = "Type";
            _typeHeader.Width = 57;
            // 
            // locationHeader
            // 
            _locationHeader.Text = "Location";
            _locationHeader.Width = 228;
            // 
            // selectButton
            // 
            _selectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _selectButton.Location = new System.Drawing.Point(488, 32);
            _selectButton.Name = "selectButton";
            _selectButton.Size = new System.Drawing.Size(75, 23);
            _selectButton.TabIndex = 1;
            _selectButton.Text = "Select";
            _selectButton.Click += new System.EventHandler(this.SelectReference);
            // 
            // removeButton
            // 
            _removeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _removeButton.Location = new System.Drawing.Point(488, 256);
            _removeButton.Name = "removeButton";
            _removeButton.Size = new System.Drawing.Size(75, 23);
            _removeButton.TabIndex = 4;
            _removeButton.Text = "Remove";
            _removeButton.Click += new System.EventHandler(this.RemoveReference);
            // 
            // referencesLabel
            // 
            _referencesLabel.Location = new System.Drawing.Point(8, 240);
            _referencesLabel.Name = "referencesLabel";
            _referencesLabel.Size = new System.Drawing.Size(472, 16);
            _referencesLabel.TabIndex = 2;
            _referencesLabel.Text = "References";
            // 
            // okButton
            // 
            _okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            _okButton.Location = new System.Drawing.Point(312, 368);
            _okButton.Name = "okButton";
            _okButton.Size = new System.Drawing.Size(75, 23);
            _okButton.TabIndex = 5;
            _okButton.Text = "Ok";
            // 
            // cancelButton
            // 
            _cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            _cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _cancelButton.Location = new System.Drawing.Point(400, 368);
            _cancelButton.Name = "cancelButton";
            _cancelButton.Size = new System.Drawing.Size(75, 23);
            _cancelButton.TabIndex = 6;
            _cancelButton.Text = "Cancel";
            // 
            // helpButton
            // 
            _helpButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            _helpButton.Location = new System.Drawing.Point(488, 368);
            _helpButton.Name = "helpButton";
            _helpButton.Size = new System.Drawing.Size(75, 23);
            _helpButton.TabIndex = 7;
            _helpButton.Text = "Help";
            // 
            // SelectReferenceDialog
            // 
            this.AcceptButton = _okButton;
            this.CancelButton = _cancelButton;
            this.ClientSize = new System.Drawing.Size(570, 399);
            this.Controls.Add(_helpButton);
            this.Controls.Add(_cancelButton);
            this.Controls.Add(_okButton);
            this.Controls.Add(_referencesLabel);
            this.Controls.Add(_removeButton);
            this.Controls.Add(_selectButton);
            this.Controls.Add(_referencesListView);
            this.Controls.Add(_referenceTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectReferenceDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Reference to AIMS script";
            _referenceTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
