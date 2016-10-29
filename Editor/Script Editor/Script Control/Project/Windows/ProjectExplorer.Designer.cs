namespace AIMS.Libraries.Scripting.ScriptControl
{
    partial class ProjectExplorer
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Node0");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.tvwSolutionExplorer = new System.Windows.Forms.TreeView();
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.cMenuFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderNewItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderExisting = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tFolderCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tFolderDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tFolderRename = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenuProject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tProjectBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectClean = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tProjectAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectNewItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectExistingItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tProjectAddRef = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.tProjectProp = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenuFile = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tFileCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tFileCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tFilePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.tFileDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tFileRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tProjectAddWebRef = new System.Windows.Forms.ToolStripMenuItem();
            this.cMenuFolder.SuspendLayout();
            this.cMenuProject.SuspendLayout();
            this.cMenuFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvwSolutionExplorer
            // 
            this.tvwSolutionExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvwSolutionExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvwSolutionExplorer.FullRowSelect = true;
            this.tvwSolutionExplorer.ImageKey = "CSharpProject.ico";
            this.tvwSolutionExplorer.ImageList = this.ilImages;
            this.tvwSolutionExplorer.Indent = 23;
            this.tvwSolutionExplorer.ItemHeight = 18;
            this.tvwSolutionExplorer.Location = new System.Drawing.Point(0, 0);
            this.tvwSolutionExplorer.Margin = new System.Windows.Forms.Padding(0);
            this.tvwSolutionExplorer.Name = "tvwSolutionExplorer";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Node0";
            this.tvwSolutionExplorer.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvwSolutionExplorer.SelectedImageIndex = 0;
            this.tvwSolutionExplorer.ShowRootLines = false;
            this.tvwSolutionExplorer.Size = new System.Drawing.Size(292, 273);
            this.tvwSolutionExplorer.TabIndex = 0;
            this.tvwSolutionExplorer.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvwSolutionExplorer_NodeMouseClick);
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Fuchsia;
            this.ilImages.Images.SetKeyName(0, "VbProject.ico");
            this.ilImages.Images.SetKeyName(1, "CSharpFile.ico");
            this.ilImages.Images.SetKeyName(2, "CSharpProject.ico");
            this.ilImages.Images.SetKeyName(3, "OpenFolder.ico");
            this.ilImages.Images.SetKeyName(4, "Reference.ico");
            this.ilImages.Images.SetKeyName(5, "ReferencesClosed.ico");
            this.ilImages.Images.SetKeyName(6, "ReferencesOpen.ico");
            this.ilImages.Images.SetKeyName(7, "VbFile.ico");
            this.ilImages.Images.SetKeyName(8, "ClosedFolder.ico");
            // 
            // cMenuFolder
            // 
            this.cMenuFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.toolStripSeparator1,
            this.tFolderCut,
            this.tFolderCopy,
            this.tFolderPaste,
            this.tFolderDelete,
            this.toolStripSeparator2,
            this.tFolderRename});
            this.cMenuFolder.Name = "cMenuExplorer";
            this.cMenuFolder.Size = new System.Drawing.Size(125, 148);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tFolderNewItem,
            this.tFolderExisting,
            this.tFolderNewFolder});
            this.addToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // tFolderNewItem
            // 
            this.tFolderNewItem.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.NewItem;
            this.tFolderNewItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderNewItem.Name = "tFolderNewItem";
            this.tFolderNewItem.Size = new System.Drawing.Size(147, 22);
            this.tFolderNewItem.Text = "New Item";
            this.tFolderNewItem.Click += new System.EventHandler(this.AddNewItem);
            // 
            // tFolderExisting
            // 
            this.tFolderExisting.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.ExistingItem;
            this.tFolderExisting.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderExisting.Name = "tFolderExisting";
            this.tFolderExisting.Size = new System.Drawing.Size(147, 22);
            this.tFolderExisting.Text = "Existing Item";
            this.tFolderExisting.Click += new System.EventHandler(this.AddExistingItem);
            // 
            // tFolderNewFolder
            // 
            this.tFolderNewFolder.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.NewFolder;
            this.tFolderNewFolder.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderNewFolder.Name = "tFolderNewFolder";
            this.tFolderNewFolder.Size = new System.Drawing.Size(147, 22);
            this.tFolderNewFolder.Text = "New Folder";
            this.tFolderNewFolder.Click += new System.EventHandler(this.AddNewFolder);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(121, 6);
            // 
            // tFolderCut
            // 
            this.tFolderCut.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Cut;
            this.tFolderCut.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderCut.Name = "tFolderCut";
            this.tFolderCut.Size = new System.Drawing.Size(124, 22);
            this.tFolderCut.Text = "Cut";
            this.tFolderCut.Click += new System.EventHandler(this.CutItem);
            // 
            // tFolderCopy
            // 
            this.tFolderCopy.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Copy;
            this.tFolderCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderCopy.Name = "tFolderCopy";
            this.tFolderCopy.Size = new System.Drawing.Size(124, 22);
            this.tFolderCopy.Text = "Copy";
            this.tFolderCopy.Click += new System.EventHandler(this.CopyItem);
            // 
            // tFolderPaste
            // 
            this.tFolderPaste.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Paste;
            this.tFolderPaste.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderPaste.Name = "tFolderPaste";
            this.tFolderPaste.Size = new System.Drawing.Size(124, 22);
            this.tFolderPaste.Text = "Paste";
            this.tFolderPaste.Click += new System.EventHandler(this.PasteItem);
            // 
            // tFolderDelete
            // 
            this.tFolderDelete.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Delete;
            this.tFolderDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFolderDelete.Name = "tFolderDelete";
            this.tFolderDelete.Size = new System.Drawing.Size(124, 22);
            this.tFolderDelete.Text = "Delete";
            this.tFolderDelete.Click += new System.EventHandler(this.DeleteItem);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(121, 6);
            // 
            // tFolderRename
            // 
            this.tFolderRename.Name = "tFolderRename";
            this.tFolderRename.Size = new System.Drawing.Size(124, 22);
            this.tFolderRename.Text = "Rename";
            this.tFolderRename.Click += new System.EventHandler(this.RenameItem);
            // 
            // cMenuProject
            // 
            this.cMenuProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tProjectBuild,
            this.tProjectClean,
            this.tProjectSep1,
            this.tProjectAdd,
            this.tProjectSep2,
            this.tProjectAddRef,
            this.tProjectAddWebRef,
            this.tProjectSep3,
            this.tProjectProp});
            this.cMenuProject.Name = "cMenuExplorer";
            this.cMenuProject.Size = new System.Drawing.Size(198, 176);
            // 
            // tProjectBuild
            // 
            this.tProjectBuild.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Build;
            this.tProjectBuild.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectBuild.Name = "tProjectBuild";
            this.tProjectBuild.Size = new System.Drawing.Size(197, 22);
            this.tProjectBuild.Text = "Build";
            this.tProjectBuild.Click += new System.EventHandler(this.BuildProject);
            // 
            // tProjectClean
            // 
            this.tProjectClean.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectClean.Name = "tProjectClean";
            this.tProjectClean.Size = new System.Drawing.Size(197, 22);
            this.tProjectClean.Text = "Clean";
            this.tProjectClean.Click += new System.EventHandler(this.CleanProject);
            // 
            // tProjectSep1
            // 
            this.tProjectSep1.Name = "tProjectSep1";
            this.tProjectSep1.Size = new System.Drawing.Size(194, 6);
            // 
            // tProjectAdd
            // 
            this.tProjectAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tProjectNewItem,
            this.tProjectExistingItem,
            this.tProjectNewFolder});
            this.tProjectAdd.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectAdd.Name = "tProjectAdd";
            this.tProjectAdd.Size = new System.Drawing.Size(197, 22);
            this.tProjectAdd.Text = "Add";
            // 
            // tProjectNewItem
            // 
            this.tProjectNewItem.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.NewItem;
            this.tProjectNewItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectNewItem.Name = "tProjectNewItem";
            this.tProjectNewItem.Size = new System.Drawing.Size(147, 22);
            this.tProjectNewItem.Text = "New Item";
            this.tProjectNewItem.Click += new System.EventHandler(this.AddNewItem);
            // 
            // tProjectExistingItem
            // 
            this.tProjectExistingItem.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.ExistingItem;
            this.tProjectExistingItem.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectExistingItem.Name = "tProjectExistingItem";
            this.tProjectExistingItem.Size = new System.Drawing.Size(147, 22);
            this.tProjectExistingItem.Text = "Existing Item";
            this.tProjectExistingItem.Click += new System.EventHandler(this.AddExistingItem);
            // 
            // tProjectNewFolder
            // 
            this.tProjectNewFolder.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.NewFolder;
            this.tProjectNewFolder.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectNewFolder.Name = "tProjectNewFolder";
            this.tProjectNewFolder.Size = new System.Drawing.Size(147, 22);
            this.tProjectNewFolder.Text = "New Folder";
            this.tProjectNewFolder.Click += new System.EventHandler(this.AddNewFolder);
            // 
            // tProjectSep2
            // 
            this.tProjectSep2.Name = "tProjectSep2";
            this.tProjectSep2.Size = new System.Drawing.Size(194, 6);
            // 
            // tProjectAddRef
            // 
            this.tProjectAddRef.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectAddRef.Name = "tProjectAddRef";
            this.tProjectAddRef.Size = new System.Drawing.Size(197, 22);
            this.tProjectAddRef.Text = "Add Reference ...";
            this.tProjectAddRef.Click += new System.EventHandler(this.AddRefrence);
            // 
            // tProjectSep3
            // 
            this.tProjectSep3.Name = "tProjectSep3";
            this.tProjectSep3.Size = new System.Drawing.Size(194, 6);
            // 
            // tProjectProp
            // 
            this.tProjectProp.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Properties;
            this.tProjectProp.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tProjectProp.Name = "tProjectProp";
            this.tProjectProp.Size = new System.Drawing.Size(197, 22);
            this.tProjectProp.Text = "Properties";
            this.tProjectProp.Click += new System.EventHandler(this.ProjectProperties);
            // 
            // cMenuFile
            // 
            this.cMenuFile.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tFileOpen,
            this.tFileSep1,
            this.tFileCut,
            this.tFileCopy,
            this.tFilePaste,
            this.tFileDelete,
            this.tFileSep2,
            this.tFileRename});
            this.cMenuFile.Name = "cMenuExplorer";
            this.cMenuFile.Size = new System.Drawing.Size(125, 148);
            // 
            // tFileOpen
            // 
            this.tFileOpen.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Open;
            this.tFileOpen.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFileOpen.Name = "tFileOpen";
            this.tFileOpen.Size = new System.Drawing.Size(124, 22);
            this.tFileOpen.Text = "Open";
            this.tFileOpen.Click += new System.EventHandler(this.OpenItem);
            // 
            // tFileSep1
            // 
            this.tFileSep1.Name = "tFileSep1";
            this.tFileSep1.Size = new System.Drawing.Size(121, 6);
            // 
            // tFileCut
            // 
            this.tFileCut.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Cut;
            this.tFileCut.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFileCut.Name = "tFileCut";
            this.tFileCut.Size = new System.Drawing.Size(124, 22);
            this.tFileCut.Text = "Cut";
            this.tFileCut.Click += new System.EventHandler(this.CutItem);
            // 
            // tFileCopy
            // 
            this.tFileCopy.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Copy;
            this.tFileCopy.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFileCopy.Name = "tFileCopy";
            this.tFileCopy.Size = new System.Drawing.Size(124, 22);
            this.tFileCopy.Text = "Copy";
            this.tFileCopy.Click += new System.EventHandler(this.CopyItem);
            // 
            // tFilePaste
            // 
            this.tFilePaste.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Paste;
            this.tFilePaste.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFilePaste.Name = "tFilePaste";
            this.tFilePaste.Size = new System.Drawing.Size(124, 22);
            this.tFilePaste.Text = "Paste";
            this.tFilePaste.Click += new System.EventHandler(this.PasteItem);
            // 
            // tFileDelete
            // 
            this.tFileDelete.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Delete;
            this.tFileDelete.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.tFileDelete.Name = "tFileDelete";
            this.tFileDelete.Size = new System.Drawing.Size(124, 22);
            this.tFileDelete.Text = "Delete";
            this.tFileDelete.Click += new System.EventHandler(this.DeleteItem);
            // 
            // tFileSep2
            // 
            this.tFileSep2.Name = "tFileSep2";
            this.tFileSep2.Size = new System.Drawing.Size(121, 6);
            // 
            // tFileRename
            // 
            this.tFileRename.Name = "tFileRename";
            this.tFileRename.Size = new System.Drawing.Size(124, 22);
            this.tFileRename.Text = "Rename";
            this.tFileRename.Click += new System.EventHandler(this.RenameItem);
            // 
            // tProjectAddWebRef
            // 
            this.tProjectAddWebRef.Name = "tProjectAddWebRef";
            this.tProjectAddWebRef.Size = new System.Drawing.Size(197, 22);
            this.tProjectAddWebRef.Text = "Add Web Reference ...";
            this.tProjectAddWebRef.Click += new System.EventHandler(this.AddWebReference);
            // 
            // ProjectExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.tvwSolutionExplorer);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProjectExplorer";
            this.TabText = "Project Explorer";
            this.Text = "Project Explorer";
            this.cMenuFolder.ResumeLayout(false);
            this.cMenuProject.ResumeLayout(false);
            this.cMenuFile.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvwSolutionExplorer;
        private System.Windows.Forms.ImageList ilImages;
        private System.Windows.Forms.ContextMenuStrip cMenuFolder;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tFolderNewItem;
        private System.Windows.Forms.ToolStripMenuItem tFolderExisting;
        private System.Windows.Forms.ToolStripMenuItem tFolderNewFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tFolderCut;
        private System.Windows.Forms.ToolStripMenuItem tFolderCopy;
        private System.Windows.Forms.ToolStripMenuItem tFolderPaste;
        private System.Windows.Forms.ToolStripMenuItem tFolderDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tFolderRename;
        private System.Windows.Forms.ContextMenuStrip cMenuProject;
        private System.Windows.Forms.ToolStripMenuItem tProjectBuild;
        private System.Windows.Forms.ToolStripMenuItem tProjectClean;
        private System.Windows.Forms.ToolStripSeparator tProjectSep1;
        private System.Windows.Forms.ToolStripMenuItem tProjectAdd;
        private System.Windows.Forms.ToolStripMenuItem tProjectNewItem;
        private System.Windows.Forms.ToolStripMenuItem tProjectExistingItem;
        private System.Windows.Forms.ToolStripMenuItem tProjectNewFolder;
        private System.Windows.Forms.ToolStripSeparator tProjectSep2;
        private System.Windows.Forms.ToolStripMenuItem tProjectAddRef;
        private System.Windows.Forms.ToolStripSeparator tProjectSep3;
        private System.Windows.Forms.ToolStripMenuItem tProjectProp;
        private System.Windows.Forms.ContextMenuStrip cMenuFile;
        private System.Windows.Forms.ToolStripMenuItem tFileOpen;
        private System.Windows.Forms.ToolStripSeparator tFileSep1;
        private System.Windows.Forms.ToolStripMenuItem tFileCut;
        private System.Windows.Forms.ToolStripMenuItem tFileCopy;
        private System.Windows.Forms.ToolStripMenuItem tFilePaste;
        private System.Windows.Forms.ToolStripMenuItem tFileDelete;
        private System.Windows.Forms.ToolStripSeparator tFileSep2;
        private System.Windows.Forms.ToolStripMenuItem tFileRename;
        private System.Windows.Forms.ToolStripMenuItem tProjectAddWebRef;
    }
}