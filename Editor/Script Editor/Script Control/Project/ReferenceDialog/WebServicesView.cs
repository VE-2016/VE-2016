using System;
using System.Web.Services.Description;
using System.Windows.Forms;

//using ICSharpCode.Core;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class WebServicesView : System.Windows.Forms.UserControl
    {
        private const int ServiceDescriptionImageIndex = 0;
        private const int ServiceImageIndex = 1;
        private const int PortImageIndex = 2;
        private ImageList _webServiceImageList;
        private System.ComponentModel.IContainer _components;
        private const int OperationImageIndex = 3;

        public WebServicesView()
        {
            InitializeComponent();
            _webServicesTreeView.ImageList = _webServiceImageList;
        }

        /// <summary>
        /// Removes all web services currently on display.
        /// </summary>
        public void Clear()
        {
            _webServicesListView.Items.Clear();
            _webServicesTreeView.Nodes.Clear();
        }

        public void Add(ServiceDescriptionCollection serviceDescriptions)
        {
            if (serviceDescriptions.Count == 0)
            {
                return;
            }

            _webServicesListView.BeginUpdate();
            try
            {
                foreach (ServiceDescription description in serviceDescriptions)
                {
                    Add(description);
                }
            }
            finally
            {
                _webServicesListView.EndUpdate();
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
            _components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebServicesView));
            _splitContainer = new System.Windows.Forms.SplitContainer();
            _webServicesTreeView = new System.Windows.Forms.TreeView();
            _webServicesListView = new System.Windows.Forms.ListView();
            _propertyColumnHeader = new System.Windows.Forms.ColumnHeader();
            _valueColumnHeader = new System.Windows.Forms.ColumnHeader();
            _webServiceImageList = new System.Windows.Forms.ImageList(_components);
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.Panel2.SuspendLayout();
            _splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            _splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            _splitContainer.Location = new System.Drawing.Point(0, 0);
            _splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            _splitContainer.Panel1.Controls.Add(_webServicesTreeView);
            // 
            // splitContainer.Panel2
            // 
            _splitContainer.Panel2.Controls.Add(_webServicesListView);
            _splitContainer.Size = new System.Drawing.Size(471, 305);
            _splitContainer.SplitterDistance = 156;
            _splitContainer.TabIndex = 1;
            // 
            // webServicesTreeView
            // 
            _webServicesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            _webServicesTreeView.Location = new System.Drawing.Point(0, 0);
            _webServicesTreeView.Name = "webServicesTreeView";
            _webServicesTreeView.Size = new System.Drawing.Size(156, 305);
            _webServicesTreeView.TabIndex = 0;
            _webServicesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.WebServicesTreeViewAfterSelect);
            // 
            // webServicesListView
            // 
            _webServicesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            _propertyColumnHeader,
            _valueColumnHeader});
            _webServicesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            _webServicesListView.Location = new System.Drawing.Point(0, 0);
            _webServicesListView.Name = "webServicesListView";
            _webServicesListView.Size = new System.Drawing.Size(311, 305);
            _webServicesListView.TabIndex = 2;
            _webServicesListView.UseCompatibleStateImageBehavior = false;
            _webServicesListView.View = System.Windows.Forms.View.Details;
            // 
            // propertyColumnHeader
            // 
            _propertyColumnHeader.Text = "Property";
            _propertyColumnHeader.Width = 120;
            // 
            // valueColumnHeader
            // 
            _valueColumnHeader.Text = "Value";
            _valueColumnHeader.Width = 191;
            // 
            // WebServiceImageList
            // 
            _webServiceImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("WebServiceImageList.ImageStream")));
            _webServiceImageList.TransparentColor = System.Drawing.Color.Transparent;
            _webServiceImageList.Images.SetKeyName(0, "Icons.16x16.Library");
            _webServiceImageList.Images.SetKeyName(1, "Icons.16x16.Interface");
            _webServiceImageList.Images.SetKeyName(2, "Icons.16x16.Class");
            _webServiceImageList.Images.SetKeyName(3, "Icons.16x16.Method");
            // 
            // WebServicesView
            // 
            this.Controls.Add(_splitContainer);
            this.Name = "WebServicesView";
            this.Size = new System.Drawing.Size(471, 305);
            _splitContainer.Panel1.ResumeLayout(false);
            _splitContainer.Panel2.ResumeLayout(false);
            _splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.ColumnHeader _propertyColumnHeader;
        private System.Windows.Forms.ColumnHeader _valueColumnHeader;
        private System.Windows.Forms.TreeView _webServicesTreeView;
        private System.Windows.Forms.ListView _webServicesListView;
        private System.Windows.Forms.SplitContainer _splitContainer;

        #endregion

        private void WebServicesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            ListViewItem item;
            _webServicesListView.Items.Clear();

            if (e.Node.Tag is ServiceDescription)
            {
                ServiceDescription desc = (ServiceDescription)e.Node.Tag;
                item = new ListViewItem();
                item.Text = "RetrievalUri";
                item.SubItems.Add(desc.RetrievalUrl);
                _webServicesListView.Items.Add(item);
            }
            else if (e.Node.Tag is Service)
            {
                Service service = (Service)e.Node.Tag;
                item = new ListViewItem();
                item.Text = "Documentation";
                item.SubItems.Add(service.Documentation);
                _webServicesListView.Items.Add(item);
            }
            else if (e.Node.Tag is Port)
            {
                Port port = (Port)e.Node.Tag;

                item = new ListViewItem();
                item.Text = "Documentation";
                item.SubItems.Add(port.Documentation);
                _webServicesListView.Items.Add(item);

                item = new ListViewItem();
                item.Text = "Binding";
                item.SubItems.Add(port.Binding.Name);
                _webServicesListView.Items.Add(item);

                item = new ListViewItem();
                item.Text = "ServiceName";
                item.SubItems.Add(port.Service.Name);
                _webServicesListView.Items.Add(item);
            }
            else if (e.Node.Tag is Operation)
            {
                Operation operation = (Operation)e.Node.Tag;

                item = new ListViewItem();
                item.Text = "Documentation";
                item.SubItems.Add(operation.Documentation);
                _webServicesListView.Items.Add(item);

                item = new ListViewItem();
                item.Text = "Parameters";
                item.SubItems.Add(operation.ParameterOrderString);
                _webServicesListView.Items.Add(item);
            }
        }

        private void Add(ServiceDescription description)
        {
            TreeNode rootNode = new TreeNode(GetName(description));
            rootNode.Tag = description;
            rootNode.ImageIndex = ServiceDescriptionImageIndex;
            rootNode.SelectedImageIndex = ServiceDescriptionImageIndex;
            _webServicesTreeView.Nodes.Add(rootNode);

            foreach (Service service in description.Services)
            {
                // Add a Service node
                TreeNode serviceNode = new TreeNode(service.Name);
                serviceNode.Tag = service;
                serviceNode.ImageIndex = ServiceImageIndex;
                serviceNode.SelectedImageIndex = ServiceImageIndex;
                rootNode.Nodes.Add(serviceNode);

                foreach (Port port in service.Ports)
                {
                    TreeNode portNode = new TreeNode(port.Name);
                    portNode.Tag = port;
                    portNode.ImageIndex = PortImageIndex;
                    portNode.SelectedImageIndex = PortImageIndex;
                    serviceNode.Nodes.Add(portNode);

                    // Get the operations
                    System.Web.Services.Description.Binding binding = description.Bindings[port.Binding.Name];
                    if (binding != null)
                    {
                        PortType portType = description.PortTypes[binding.Type.Name];
                        if (portType != null)
                        {
                            foreach (Operation operation in portType.Operations)
                            {
                                TreeNode operationNode = new TreeNode(operation.Name);
                                operationNode.Tag = operation;
                                operationNode.ImageIndex = OperationImageIndex;
                                operationNode.SelectedImageIndex = OperationImageIndex;
                                portNode.Nodes.Add(operationNode);
                            }
                        }
                    }
                }
            }
            _webServicesTreeView.ExpandAll();
        }

        private string GetName(ServiceDescription description)
        {
            if (description.Name != null)
            {
                return description.Name;
            }
            else if (description.RetrievalUrl != null)
            {
                Uri uri = new Uri(description.RetrievalUrl);
                if (uri.Segments.Length > 0)
                {
                    return uri.Segments[uri.Segments.Length - 1];
                }
                else
                {
                    return uri.Host;
                }
            }
            return String.Empty;
        }
    }
}
