namespace WinExplorer.UI
{
    partial class DatabaseInspectorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseInspectorForm));
            this.InspectorContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InspectorImageList = new System.Windows.Forms.ImageList(this.components);
            this.TableNodeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DatabaseTreeView = new System.Windows.Forms.TreeView();
            this.ColumnNameContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dataComparisonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.scriptAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cREATEToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQueryWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dROPToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQueryWindowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.clipboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cREATEAndDROPToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newQueryWindowToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.clipboardToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDesignerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewPermissionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InspectorContextMenuStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // InspectorContextMenuStrip
            // 
            this.InspectorContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem});
            this.InspectorContextMenuStrip.Name = "InspectorContextMenuStrip";
            this.InspectorContextMenuStrip.Size = new System.Drawing.Size(160, 26);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.loadToolStripMenuItem.Text = "&Load Meta-Data";
            // 
            // InspectorImageList
            // 
            this.InspectorImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.InspectorImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.InspectorImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TableNodeContextMenuStrip
            // 
            this.TableNodeContextMenuStrip.Name = "TableNodeContextMenuStrip";
            this.TableNodeContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // DatabaseTreeView
            // 
            this.DatabaseTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DatabaseTreeView.ContextMenuStrip = this.InspectorContextMenuStrip;
            this.DatabaseTreeView.ImageIndex = 0;
            this.DatabaseTreeView.ImageList = this.InspectorImageList;
            this.DatabaseTreeView.Location = new System.Drawing.Point(0, 28);
            this.DatabaseTreeView.Name = "DatabaseTreeView";
            this.DatabaseTreeView.SelectedImageIndex = 0;
            this.DatabaseTreeView.ShowNodeToolTips = true;
            this.DatabaseTreeView.Size = new System.Drawing.Size(342, 484);
            this.DatabaseTreeView.TabIndex = 2;
            this.DatabaseTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.DatabaseTreeView_BeforeExpand);
            this.DatabaseTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.DatabaseTreeView_AfterSelect);
            this.DatabaseTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.DatabaseTreeView_NodeMouseClick);
            // 
            // ColumnNameContextMenuStrip
            // 
            this.ColumnNameContextMenuStrip.Name = "ColumnNameContextMenuStrip";
            this.ColumnNameContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.toolStripButton3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(342, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::WinExplorer.Resources.Refresh_16x;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::WinExplorer.Resources.Cancel_256x;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = global::WinExplorer.Resources.AddDatabase_16x;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataComparisonToolStripMenuItem,
            this.toolStripMenuItem1,
            this.scriptAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.viewCodeToolStripMenuItem,
            this.viewDesignerToolStripMenuItem,
            this.viewPermissionsToolStripMenuItem,
            this.viewDataToolStripMenuItem,
            this.toolStripMenuItem3,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripMenuItem4,
            this.refreshToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 270);
            // 
            // dataComparisonToolStripMenuItem
            // 
            this.dataComparisonToolStripMenuItem.Name = "dataComparisonToolStripMenuItem";
            this.dataComparisonToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.dataComparisonToolStripMenuItem.Text = "Data Comparison";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 6);
            // 
            // scriptAsToolStripMenuItem
            // 
            this.scriptAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cREATEToToolStripMenuItem,
            this.dROPToToolStripMenuItem,
            this.cREATEAndDROPToToolStripMenuItem});
            this.scriptAsToolStripMenuItem.Name = "scriptAsToolStripMenuItem";
            this.scriptAsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.scriptAsToolStripMenuItem.Text = "Script As";
            // 
            // cREATEToToolStripMenuItem
            // 
            this.cREATEToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newQueryWindowToolStripMenuItem,
            this.clipboardToolStripMenuItem,
            this.fileToolStripMenuItem});
            this.cREATEToToolStripMenuItem.Name = "cREATEToToolStripMenuItem";
            this.cREATEToToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.cREATEToToolStripMenuItem.Text = "CREATE To";
            // 
            // newQueryWindowToolStripMenuItem
            // 
            this.newQueryWindowToolStripMenuItem.Name = "newQueryWindowToolStripMenuItem";
            this.newQueryWindowToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.newQueryWindowToolStripMenuItem.Text = "New Query Window";
            this.newQueryWindowToolStripMenuItem.Click += new System.EventHandler(this.newQueryWindowToolStripMenuItem_Click);
            // 
            // clipboardToolStripMenuItem
            // 
            this.clipboardToolStripMenuItem.Name = "clipboardToolStripMenuItem";
            this.clipboardToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.clipboardToolStripMenuItem.Text = "Clipboard";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // dROPToToolStripMenuItem
            // 
            this.dROPToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newQueryWindowToolStripMenuItem1,
            this.clipboardToolStripMenuItem1,
            this.fileToolStripMenuItem1});
            this.dROPToToolStripMenuItem.Name = "dROPToToolStripMenuItem";
            this.dROPToToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.dROPToToolStripMenuItem.Text = "DROP To";
            // 
            // newQueryWindowToolStripMenuItem1
            // 
            this.newQueryWindowToolStripMenuItem1.Name = "newQueryWindowToolStripMenuItem1";
            this.newQueryWindowToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.newQueryWindowToolStripMenuItem1.Text = "New Query Window";
            // 
            // clipboardToolStripMenuItem1
            // 
            this.clipboardToolStripMenuItem1.Name = "clipboardToolStripMenuItem1";
            this.clipboardToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.clipboardToolStripMenuItem1.Text = "Clipboard";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.fileToolStripMenuItem1.Text = "File";
            // 
            // cREATEAndDROPToToolStripMenuItem
            // 
            this.cREATEAndDROPToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newQueryWindowToolStripMenuItem2,
            this.clipboardToolStripMenuItem2,
            this.fileToolStripMenuItem2});
            this.cREATEAndDROPToToolStripMenuItem.Name = "cREATEAndDROPToToolStripMenuItem";
            this.cREATEAndDROPToToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.cREATEAndDROPToToolStripMenuItem.Text = "CREATE And DROP To";
            // 
            // newQueryWindowToolStripMenuItem2
            // 
            this.newQueryWindowToolStripMenuItem2.Name = "newQueryWindowToolStripMenuItem2";
            this.newQueryWindowToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.newQueryWindowToolStripMenuItem2.Text = "New Query Window";
            // 
            // clipboardToolStripMenuItem2
            // 
            this.clipboardToolStripMenuItem2.Name = "clipboardToolStripMenuItem2";
            this.clipboardToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.clipboardToolStripMenuItem2.Text = "Clipboard";
            // 
            // fileToolStripMenuItem2
            // 
            this.fileToolStripMenuItem2.Name = "fileToolStripMenuItem2";
            this.fileToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
            this.fileToolStripMenuItem2.Text = "File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(163, 6);
            // 
            // viewCodeToolStripMenuItem
            // 
            this.viewCodeToolStripMenuItem.Name = "viewCodeToolStripMenuItem";
            this.viewCodeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.viewCodeToolStripMenuItem.Text = "View Code";
            this.viewCodeToolStripMenuItem.Click += new System.EventHandler(this.viewCodeToolStripMenuItem_Click);
            // 
            // viewDesignerToolStripMenuItem
            // 
            this.viewDesignerToolStripMenuItem.Name = "viewDesignerToolStripMenuItem";
            this.viewDesignerToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.viewDesignerToolStripMenuItem.Text = "View Designer";
            this.viewDesignerToolStripMenuItem.Click += new System.EventHandler(this.viewDesignerToolStripMenuItem_Click);
            // 
            // viewPermissionsToolStripMenuItem
            // 
            this.viewPermissionsToolStripMenuItem.Name = "viewPermissionsToolStripMenuItem";
            this.viewPermissionsToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.viewPermissionsToolStripMenuItem.Text = "View Permissions";
            // 
            // viewDataToolStripMenuItem
            // 
            this.viewDataToolStripMenuItem.Name = "viewDataToolStripMenuItem";
            this.viewDataToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.viewDataToolStripMenuItem.Text = "View Data";
            this.viewDataToolStripMenuItem.Click += new System.EventHandler(this.viewDataToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(163, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(163, 6);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            // 
            // DatabaseInspectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 512);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.DatabaseTreeView);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DatabaseInspectorForm";
            this.Text = "Database Inspector";
            this.Load += new System.EventHandler(this.DatabaseInspectorForm_Load);
            this.InspectorContextMenuStrip.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip InspectorContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip TableNodeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ImageList InspectorImageList;
        private System.Windows.Forms.TreeView DatabaseTreeView;
        private System.Windows.Forms.ContextMenuStrip ColumnNameContextMenuStrip;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem dataComparisonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem scriptAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cREATEToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newQueryWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dROPToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newQueryWindowToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clipboardToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cREATEAndDROPToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newQueryWindowToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem clipboardToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem viewCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDesignerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewPermissionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
    }
}