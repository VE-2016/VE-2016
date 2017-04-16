namespace AIMS.Libraries.Scripting.ScriptControl
{
    partial class ScriptControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptControl));
            this.ScriptStatus = new System.Windows.Forms.StatusStrip();
            this.tsMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tCursorPos = new System.Windows.Forms.ToolStripStatusLabel();
            this.CtrlToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbSelectLanguage = new System.Windows.Forms.ToolStripDropDownButton();
            this.cNetToolStripMenuItemCSharp = new System.Windows.Forms.ToolStripMenuItem();
            this.vBNetToolStripMenuItemVbNet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCut = new System.Windows.Forms.ToolStripButton();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.tsbPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbComment = new System.Windows.Forms.ToolStripButton();
            this.tsbUnComment = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUndo = new System.Windows.Forms.ToolStripButton();
            this.tsbRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbBuild = new System.Windows.Forms.ToolStripButton();
            this.tsbRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbToggleBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsbPreBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsbNextBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsbDelAllBookmark = new System.Windows.Forms.ToolStripButton();
            this.tsbDelallBreakPoints = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbFind = new System.Windows.Forms.ToolStripButton();
            this.tsbReplace = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbErrorList = new System.Windows.Forms.ToolStripButton();
            this.tsbSolutionExplorer = new System.Windows.Forms.ToolStripButton();
            this.syntaxDocument1 = new AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument(this.components);
            this.ScriptStatus.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ScriptStatus
            // 
            this.ScriptStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMessage,
            this.tsProgress,
            this.tCursorPos});
            this.ScriptStatus.Location = new System.Drawing.Point(0, 413);
            this.ScriptStatus.Name = "ScriptStatus";
            this.ScriptStatus.Size = new System.Drawing.Size(633, 22);
            this.ScriptStatus.SizingGrip = false;
            this.ScriptStatus.TabIndex = 1;
            this.ScriptStatus.Text = "statusStrip1";
            // 
            // tsMessage
            // 
            this.tsMessage.AutoSize = false;
            this.tsMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsMessage.Name = "tsMessage";
            this.tsMessage.Size = new System.Drawing.Size(618, 17);
            this.tsMessage.Spring = true;
            this.tsMessage.Text = "Please wait..";
            this.tsMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsProgress
            // 
            this.tsProgress.AutoSize = false;
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(200, 16);
            this.tsProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tsProgress.Visible = false;
            // 
            // tCursorPos
            // 
            this.tCursorPos.Name = "tCursorPos";
            this.tCursorPos.Size = new System.Drawing.Size(0, 17);
            this.tCursorPos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tCursorPos.ToolTipText = "Cursor Position";
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(589, 301);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(633, 388);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(633, 413);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbSelectLanguage,
            this.toolStripSeparator7,
            this.toolStripButton1,
            this.tsbNew,
            this.tsbSave,
            this.toolStripSeparator1,
            this.tsbCut,
            this.tsbCopy,
            this.tsbPaste,
            this.toolStripSeparator5,
            this.tsbComment,
            this.tsbUnComment,
            this.toolStripSeparator2,
            this.tsbUndo,
            this.tsbRedo,
            this.toolStripSeparator3,
            this.tsbBuild,
            this.tsbRun,
            this.toolStripSeparator4,
            this.tsbToggleBookmark,
            this.tsbPreBookmark,
            this.tsbNextBookmark,
            this.tsbDelAllBookmark,
            this.tsbDelallBreakPoints,
            this.toolStripSeparator6,
            this.tsbFind,
            this.tsbReplace,
            this.toolStripSeparator8,
            this.tsbErrorList,
            this.tsbSolutionExplorer});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(589, 25);
            this.toolStrip1.TabIndex = 4;
            // 
            // tsbSelectLanguage
            // 
            this.tsbSelectLanguage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSelectLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cNetToolStripMenuItemCSharp,
            this.vBNetToolStripMenuItemVbNet});
            this.tsbSelectLanguage.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProject_CSCodefile;
            this.tsbSelectLanguage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelectLanguage.Name = "tsbSelectLanguage";
            this.tsbSelectLanguage.Size = new System.Drawing.Size(29, 22);
            this.tsbSelectLanguage.Text = "Select Language";
            // 
            // cNetToolStripMenuItemCSharp
            // 
            this.cNetToolStripMenuItemCSharp.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProject_CSCodefile;
            this.cNetToolStripMenuItemCSharp.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.cNetToolStripMenuItemCSharp.Name = "cNetToolStripMenuItemCSharp";
            this.cNetToolStripMenuItemCSharp.Size = new System.Drawing.Size(114, 22);
            this.cNetToolStripMenuItemCSharp.Text = "C# .Net";
            this.cNetToolStripMenuItemCSharp.ToolTipText = "Select C# as programming Language";
            this.cNetToolStripMenuItemCSharp.Click += new System.EventHandler(this.cNetToolStripMenuItemCSharp_Click);
            // 
            // vBNetToolStripMenuItemVbNet
            // 
            this.vBNetToolStripMenuItemVbNet.BackColor = System.Drawing.Color.Transparent;
            this.vBNetToolStripMenuItemVbNet.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProject_VBCodefile;
            this.vBNetToolStripMenuItemVbNet.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.vBNetToolStripMenuItemVbNet.Name = "vBNetToolStripMenuItemVbNet";
            this.vBNetToolStripMenuItemVbNet.Size = new System.Drawing.Size(114, 22);
            this.vBNetToolStripMenuItemVbNet.Text = "VB .Net";
            this.vBNetToolStripMenuItemVbNet.ToolTipText = "Select Vb.Net as programming Language";
            this.vBNetToolStripMenuItemVbNet.Click += new System.EventHandler(this.vBNetToolStripMenuItemVbNet_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(40, 22);
            this.toolStripButton1.Text = "Open";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tsbNew
            // 
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.NewDocument;
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(23, 22);
            this.tsbNew.Text = "New";
            this.tsbNew.Click += new System.EventHandler(this.tsbNew_Click);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Save;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Save";
            this.tsbSave.Click += new System.EventHandler(this.OnSave);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbCut
            // 
            this.tsbCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCut.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Cut;
            this.tsbCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCut.Name = "tsbCut";
            this.tsbCut.Size = new System.Drawing.Size(23, 22);
            this.tsbCut.Text = "Cut";
            this.tsbCut.Click += new System.EventHandler(this.tsbCut_Click);
            // 
            // tsbCopy
            // 
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopy.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Copy;
            this.tsbCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Size = new System.Drawing.Size(23, 22);
            this.tsbCopy.Text = "Copy";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            // 
            // tsbPaste
            // 
            this.tsbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPaste.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Paste;
            this.tsbPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPaste.Name = "tsbPaste";
            this.tsbPaste.Size = new System.Drawing.Size(23, 22);
            this.tsbPaste.Text = "Paste";
            this.tsbPaste.Click += new System.EventHandler(this.tsbPaste_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbComment
            // 
            this.tsbComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbComment.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.CodeComment;
            this.tsbComment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbComment.Name = "tsbComment";
            this.tsbComment.Size = new System.Drawing.Size(23, 22);
            this.tsbComment.Text = "Comment out the selected lines.";
            this.tsbComment.Click += new System.EventHandler(this.tsbComment_Click);
            // 
            // tsbUnComment
            // 
            this.tsbUnComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUnComment.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.CodeUnComment;
            this.tsbUnComment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUnComment.Name = "tsbUnComment";
            this.tsbUnComment.Size = new System.Drawing.Size(23, 22);
            this.tsbUnComment.Text = "Uncomment the selected lines.";
            this.tsbUnComment.Click += new System.EventHandler(this.tsbUnComment_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbUndo
            // 
            this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUndo.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Edit_Undo;
            this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUndo.Name = "tsbUndo";
            this.tsbUndo.Size = new System.Drawing.Size(23, 22);
            this.tsbUndo.Text = "Undo";
            this.tsbUndo.Click += new System.EventHandler(this.tsbUndo_Click);
            // 
            // tsbRedo
            // 
            this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRedo.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Edit_Redo;
            this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRedo.Name = "tsbRedo";
            this.tsbRedo.Size = new System.Drawing.Size(23, 22);
            this.tsbRedo.Text = "Redo";
            this.tsbRedo.Click += new System.EventHandler(this.tsbRedo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbBuild
            // 
            this.tsbBuild.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbBuild.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Build;
            this.tsbBuild.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbBuild.Name = "tsbBuild";
            this.tsbBuild.Size = new System.Drawing.Size(23, 22);
            this.tsbBuild.Text = "Build";
            this.tsbBuild.Click += new System.EventHandler(this.tsbBuild_Click);
            // 
            // tsbRun
            // 
            this.tsbRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRun.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Run;
            this.tsbRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRun.Name = "tsbRun";
            this.tsbRun.Size = new System.Drawing.Size(23, 22);
            this.tsbRun.Text = "Run";
            this.tsbRun.Click += new System.EventHandler(this.tsbRun_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbToggleBookmark
            // 
            this.tsbToggleBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbToggleBookmark.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.BookMarkToggle;
            this.tsbToggleBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbToggleBookmark.Name = "tsbToggleBookmark";
            this.tsbToggleBookmark.Size = new System.Drawing.Size(23, 22);
            this.tsbToggleBookmark.Text = "Toggle Bookmark";
            this.tsbToggleBookmark.Click += new System.EventHandler(this.tsbToggleBookmark_Click);
            // 
            // tsbPreBookmark
            // 
            this.tsbPreBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPreBookmark.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.BookMarkPre;
            this.tsbPreBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPreBookmark.Name = "tsbPreBookmark";
            this.tsbPreBookmark.Size = new System.Drawing.Size(23, 22);
            this.tsbPreBookmark.Text = "Move to previous bookmark.";
            this.tsbPreBookmark.Click += new System.EventHandler(this.tsbPreBookmark_Click);
            // 
            // tsbNextBookmark
            // 
            this.tsbNextBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNextBookmark.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.BookMarkNext;
            this.tsbNextBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNextBookmark.Name = "tsbNextBookmark";
            this.tsbNextBookmark.Size = new System.Drawing.Size(23, 22);
            this.tsbNextBookmark.Text = "Move to next bookmark.";
            this.tsbNextBookmark.Click += new System.EventHandler(this.tsbNextBookmark_Click);
            // 
            // tsbDelAllBookmark
            // 
            this.tsbDelAllBookmark.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelAllBookmark.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.BookMarkDelete;
            this.tsbDelAllBookmark.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelAllBookmark.Name = "tsbDelAllBookmark";
            this.tsbDelAllBookmark.Size = new System.Drawing.Size(23, 22);
            this.tsbDelAllBookmark.Text = "Delete all bookmarks.";
            this.tsbDelAllBookmark.Click += new System.EventHandler(this.tsbDelAllBookmark_Click);
            // 
            // tsbDelallBreakPoints
            // 
            this.tsbDelallBreakPoints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDelallBreakPoints.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.DelBreakpoints;
            this.tsbDelallBreakPoints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDelallBreakPoints.Name = "tsbDelallBreakPoints";
            this.tsbDelallBreakPoints.Size = new System.Drawing.Size(23, 22);
            this.tsbDelallBreakPoints.Text = "Delete all breakpoints.";
            this.tsbDelallBreakPoints.Click += new System.EventHandler(this.tsbDelallBreakPoints_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbFind
            // 
            this.tsbFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbFind.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Find;
            this.tsbFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFind.Name = "tsbFind";
            this.tsbFind.Size = new System.Drawing.Size(23, 22);
            this.tsbFind.Text = "Find & Replace";
            this.tsbFind.Click += new System.EventHandler(this.tsbFind_Click);
            // 
            // tsbReplace
            // 
            this.tsbReplace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbReplace.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.FindNext;
            this.tsbReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReplace.Name = "tsbReplace";
            this.tsbReplace.Size = new System.Drawing.Size(23, 22);
            this.tsbReplace.Text = "toolStripButton1";
            this.tsbReplace.Click += new System.EventHandler(this.tsbReplace_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbErrorList
            // 
            this.tsbErrorList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbErrorList.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.Output;
            this.tsbErrorList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbErrorList.Name = "tsbErrorList";
            this.tsbErrorList.Size = new System.Drawing.Size(23, 22);
            this.tsbErrorList.Text = "Error List";
            this.tsbErrorList.Click += new System.EventHandler(this.tsbErrorList_Click);
            // 
            // tsbSolutionExplorer
            // 
            this.tsbSolutionExplorer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSolutionExplorer.Image = global::AIMS.Libraries.Scripting.ScriptControl.Properties.Resources.VSProjectExplorer;
            this.tsbSolutionExplorer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSolutionExplorer.Name = "tsbSolutionExplorer";
            this.tsbSolutionExplorer.Size = new System.Drawing.Size(23, 22);
            this.tsbSolutionExplorer.Text = "Project References";
            this.tsbSolutionExplorer.ToolTipText = "Project References";
            this.tsbSolutionExplorer.Click += new System.EventHandler(this.tsbSolutionExplorer_Click);
            // 
            // syntaxDocument1
            // 
            this.syntaxDocument1.csd = null;
            this.syntaxDocument1.Lines = new string[] {
        ""};
            this.syntaxDocument1.MaxUndoBufferSize = 1000;
            this.syntaxDocument1.Modified = false;
            this.syntaxDocument1.UndoStep = 0;
            // 
            // ScriptControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.ScriptStatus);
            this.DoubleBuffered = true;
            this.Name = "ScriptControl";
            this.Size = new System.Drawing.Size(633, 435);
            this.ScriptStatus.ResumeLayout(false);
            this.ScriptStatus.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument syntaxDocument1;
        private System.Windows.Forms.StatusStrip ScriptStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsMessage;
        private System.Windows.Forms.ToolStripProgressBar tsProgress;
        private System.Windows.Forms.ToolTip CtrlToolTip;
        public System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton tsbSelectLanguage;
        private System.Windows.Forms.ToolStripMenuItem cNetToolStripMenuItemCSharp;
        private System.Windows.Forms.ToolStripMenuItem vBNetToolStripMenuItemVbNet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton tsbNew;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbCut;
        private System.Windows.Forms.ToolStripButton tsbCopy;
        private System.Windows.Forms.ToolStripButton tsbPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton tsbComment;
        private System.Windows.Forms.ToolStripButton tsbUnComment;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsbUndo;
        private System.Windows.Forms.ToolStripButton tsbRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsbBuild;
        private System.Windows.Forms.ToolStripButton tsbRun;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton tsbToggleBookmark;
        private System.Windows.Forms.ToolStripButton tsbPreBookmark;
        private System.Windows.Forms.ToolStripButton tsbNextBookmark;
        private System.Windows.Forms.ToolStripButton tsbDelAllBookmark;
        private System.Windows.Forms.ToolStripButton tsbDelallBreakPoints;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton tsbFind;
        private System.Windows.Forms.ToolStripButton tsbReplace;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton tsbErrorList;
        private System.Windows.Forms.ToolStripButton tsbSolutionExplorer;
        private System.Windows.Forms.ToolStripStatusLabel tCursorPos;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel1;
    }
}
