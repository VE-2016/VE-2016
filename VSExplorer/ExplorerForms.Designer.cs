/*
 *
 */

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;

namespace WinExplorer
{
    partial class ExplorerForms
    {
        private IContainer _components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
                _components.Dispose();

            base.Dispose(disposing);
        }

        #region Form Layout

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton11 = new System.Windows.Forms.ToolStripButton();
            this._mainTreeView = new System.Windows.Forms.TreeView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.treeView2 = new WinExplorer.TreeViews();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton13 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton14 = new System.Windows.Forms.ToolStripButton();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.treeView4 = new System.Windows.Forms.TreeView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.treeView1 = new WinExplorer.UI.TreeViewEx();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.treeView3 = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem55 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem54 = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeSourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.addReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.referenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsMainProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem29 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem22 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem48 = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem28 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem21 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.unloadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.viewProjectFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderInFileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSBuildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCMDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerShellToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem49 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem30 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem53 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem52 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.existingProjetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.newItemToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.existingItemToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.existingProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSolutionFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem51 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem50 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreNugetPackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.websiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectFromSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.projectSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem78 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem87 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem66 = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.newWebSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.existingProjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.existingWebSiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem76 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.recentProjectsAndSolutionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem70 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem69 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem72 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem73 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem74 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem71 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem75 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncommentSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem57 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem58 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem59 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem56 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.solutionExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem83 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem82 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem60 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem61 = new System.Windows.Forms.ToolStripMenuItem();
            this.classViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem62 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.errorListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputWindowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem64 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem84 = new System.Windows.Forms.ToolStripMenuItem();
            this.findResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findResults1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findResults2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem65 = new System.Windows.Forms.ToolStripMenuItem();
            this.commandWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem81 = new System.Windows.Forms.ToolStripSeparator();
            this.powerShellWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xmlDocumentOutlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syntaxTreeVisualizatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem79 = new System.Windows.Forms.ToolStripMenuItem();
            this.standardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outlineWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nUnitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pROJECTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addWindowsFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addUserControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addComponentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem46 = new System.Windows.Forms.ToolStripSeparator();
            this.addNewItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addExistingItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem47 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem67 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem68 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.addReferenceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addProjectReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAnalyzerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAsStartupProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectDependenciesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectBuildOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.projectPropertiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bUILDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runCodeAnalysisOnSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.buildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runCodeAnalysisOnProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.configurationManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem80 = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakpointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exceptionSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDiagnosticToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.immediateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.watchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.startDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startWithoutDebuggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performanceProfilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachToProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.profilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.stepInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tOOLSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.externalToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importAndExportSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.custToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tESTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vSExplorerTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem85 = new System.Windows.Forms.ToolStripSeparator();
            this.windowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastRunTestsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wINDOWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllDocumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.hELPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem31 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewInWebBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewInApplicationWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutApplicationStudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.guiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip4 = new System.Windows.Forms.ToolStrip();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewInObjectBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewInFileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip5 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.decompileTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addConnectedServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAnalyzerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.managedNuGetPackagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem86 = new System.Windows.Forms.ToolStripSeparator();
            this.scopeToThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip6 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem23 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem24 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem27 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem77 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem25 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem26 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.contextMenuStrip7 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem32 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem33 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem34 = new System.Windows.Forms.ToolStripMenuItem();
            this.newItemToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exisitngItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem44 = new System.Windows.Forms.ToolStripSeparator();
            this.windowsFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem45 = new System.Windows.Forms.ToolStripSeparator();
            this.componentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.classToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem35 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem36 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem37 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem38 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem43 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem39 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem42 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem41 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem40 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.contextMenuStrip4.SuspendLayout();
            this.contextMenuStrip5.SuspendLayout();
            this.contextMenuStrip6.SuspendLayout();
            this.contextMenuStrip7.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(22, 21);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(892, 481);
            this.splitContainer1.SplitterDistance = 435;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(435, 481);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Controls.Add(this._mainTreeView);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(427, 455);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Folders";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton11});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(421, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton11
            // 
            this.toolStripButton11.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton11.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton11.Name = "toolStripButton11";
            this.toolStripButton11.Size = new System.Drawing.Size(87, 22);
            this.toolStripButton11.Text = "Open Solution";
            this.toolStripButton11.Click += new System.EventHandler(this.toolStripButton11_Click);
            // 
            // _mainTreeView
            // 
            this._mainTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._mainTreeView.Location = new System.Drawing.Point(3, 31);
            this._mainTreeView.Name = "_mainTreeView";
            this._mainTreeView.Size = new System.Drawing.Size(421, 487);
            this._mainTreeView.TabIndex = 0;
            this._mainTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._mainTreeView_AfterSelect);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(427, 455);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Project";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.treeView2);
            this.splitContainer3.Panel1.Controls.Add(this.toolStrip5);
            this.splitContainer3.Panel1.Controls.Add(this.textBox4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.treeView4);
            this.splitContainer3.Size = new System.Drawing.Size(421, 449);
            this.splitContainer3.SplitterDistance = 370;
            this.splitContainer3.TabIndex = 3;
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.context1 = null;
            this.treeView2.context2 = null;
            this.treeView2.Location = new System.Drawing.Point(6, 55);
            this.treeView2.Name = "treeView2";
            this.treeView2.Size = new System.Drawing.Size(412, 302);
            this.treeView2.TabIndex = 3;
            // 
            // toolStrip5
            // 
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton13,
            this.toolStripButton14});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(421, 25);
            this.toolStrip5.TabIndex = 0;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // toolStripButton13
            // 
            this.toolStripButton13.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton13.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.toolStripButton13.Name = "toolStripButton13";
            this.toolStripButton13.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton13.Text = "toolStripButton13";
            this.toolStripButton13.ToolTipText = "Create Class View";
            this.toolStripButton13.Click += new System.EventHandler(this.toolStripButton13_Click);
            // 
            // toolStripButton14
            // 
            this.toolStripButton14.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton14.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton14.Name = "toolStripButton14";
            this.toolStripButton14.Size = new System.Drawing.Size(33, 22);
            this.toolStripButton14.Text = "Refs";
            this.toolStripButton14.Click += new System.EventHandler(this.toolStripButton14_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(5, 28);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(314, 20);
            this.textBox4.TabIndex = 2;
            // 
            // treeView4
            // 
            this.treeView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView4.Location = new System.Drawing.Point(0, 0);
            this.treeView4.Name = "treeView4";
            this.treeView4.Size = new System.Drawing.Size(421, 75);
            this.treeView4.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.treeView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(427, 455);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "VSolution";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.context1 = null;
            this.treeView1.context2 = null;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(427, 455);
            this.treeView1.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button2);
            this.tabPage5.Controls.Add(this.textBox2);
            this.tabPage5.Controls.Add(this.textBox1);
            this.tabPage5.Controls.Add(this.richTextBox1);
            this.tabPage5.Controls.Add(this.button1);
            this.tabPage5.Controls.Add(this.listBox1);
            this.tabPage5.Controls.Add(this.toolStrip2);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(427, 455);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "New File";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 250);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(320, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Add existing file";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(9, 223);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(320, 20);
            this.textBox2.TabIndex = 6;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(320, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "Existing file templates";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(9, 279);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(320, 218);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(320, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Load template as ...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 65);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(320, 121);
            this.listBox1.TabIndex = 3;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton3});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(421, 25);
            this.toolStrip2.TabIndex = 1;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(35, 22);
            this.toolStripButton3.Text = "New";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.textBox3);
            this.tabPage6.Controls.Add(this.progressBar1);
            this.tabPage6.Controls.Add(this.toolStrip3);
            this.tabPage6.Controls.Add(this.treeView3);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(427, 455);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Recent";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(9, 32);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(412, 20);
            this.textBox3.TabIndex = 3;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(4, 424);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(417, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // toolStrip3
            // 
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton5,
            this.toolStripButton6,
            this.toolStripButton2});
            this.toolStrip3.Location = new System.Drawing.Point(3, 3);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(421, 25);
            this.toolStrip3.TabIndex = 1;
            this.toolStrip3.Text = "toolStrip3";
            this.toolStrip3.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip3_ItemClicked);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Image = global::WinExplorer.ve_resource.FolderOpen_16x;
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(56, 22);
            this.toolStripButton5.Text = "Open";
            this.toolStripButton5.Click += new System.EventHandler(this.toolStripButton5_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.Image = global::WinExplorer.ve_resource.Refresh_16x;
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(60, 22);
            this.toolStripButton6.Text = "Delete";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::WinExplorer.ve_resource.ClearCollection_16x;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(71, 22);
            this.toolStripButton2.Text = "Clear All";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click_1);
            // 
            // treeView3
            // 
            this.treeView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView3.Location = new System.Drawing.Point(3, 58);
            this.treeView3.Name = "treeView3";
            this.treeView3.Size = new System.Drawing.Size(418, 360);
            this.treeView3.TabIndex = 0;
            this.treeView3.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView3_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer2.Size = new System.Drawing.Size(453, 481);
            this.splitContainer2.SplitterDistance = 234;
            this.splitContainer2.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripMenuItem55,
            this.toolStripMenuItem54,
            this.toolStripMenuItem5,
            this.toolStripMenuItem8,
            this.addReferenceToolStripMenuItem,
            this.toolStripMenuItem7,
            this.addToolStripMenuItem,
            this.toolStripMenuItem6,
            this.setAsMainProjectToolStripMenuItem,
            this.runToolStripMenuItem,
            this.toolStripMenuItem29,
            this.toolStripMenuItem22,
            this.toolStripMenuItem48,
            this.removeToolStripMenuItem,
            this.toolStripMenuItem28,
            this.toolStripMenuItem21,
            this.toolStripSeparator4,
            this.unloadProjectToolStripMenuItem,
            this.toolStripSeparator5,
            this.viewProjectFileToolStripMenuItem,
            this.openFolderInFileExplorerToolStripMenuItem,
            this.projectPropertiesToolStripMenuItem,
            this.mSBuildProjectToolStripMenuItem,
            this.openCMDToolStripMenuItem,
            this.powerShellToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(219, 540);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem3.Text = "Build";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem2.Text = "Rebuild";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem4.Text = "Clean";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem55
            // 
            this.toolStripMenuItem55.Name = "toolStripMenuItem55";
            this.toolStripMenuItem55.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem55.Text = "View";
            // 
            // toolStripMenuItem54
            // 
            this.toolStripMenuItem54.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeSourceCodeToolStripMenuItem});
            this.toolStripMenuItem54.Name = "toolStripMenuItem54";
            this.toolStripMenuItem54.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem54.Text = "Analyze";
            // 
            // analyzeSourceCodeToolStripMenuItem
            // 
            this.analyzeSourceCodeToolStripMenuItem.Name = "analyzeSourceCodeToolStripMenuItem";
            this.analyzeSourceCodeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.analyzeSourceCodeToolStripMenuItem.Text = "Analyze Source Code";
            this.analyzeSourceCodeToolStripMenuItem.Click += new System.EventHandler(this.analyzeSourceCodeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem8.Text = "Scope to This";
            // 
            // addReferenceToolStripMenuItem
            // 
            this.addReferenceToolStripMenuItem.Name = "addReferenceToolStripMenuItem";
            this.addReferenceToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.addReferenceToolStripMenuItem.Text = "Add Reference";
            this.addReferenceToolStripMenuItem.Click += new System.EventHandler(this.addReferenceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem7.Text = "Build Dependencies";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newItemToolStripMenuItem,
            this.existingItemToolStripMenuItem,
            this.toolStripMenuItem20,
            this.toolStripSeparator3,
            this.referenceToolStripMenuItem,
            this.formToolStripMenuItem,
            this.controlToolStripMenuItem,
            this.newFolderToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // newItemToolStripMenuItem
            // 
            this.newItemToolStripMenuItem.Name = "newItemToolStripMenuItem";
            this.newItemToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.newItemToolStripMenuItem.Text = "New Item";
            this.newItemToolStripMenuItem.Click += new System.EventHandler(this.newItemToolStripMenuItem_Click);
            // 
            // existingItemToolStripMenuItem
            // 
            this.existingItemToolStripMenuItem.Name = "existingItemToolStripMenuItem";
            this.existingItemToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.existingItemToolStripMenuItem.Text = "Existing Item";
            this.existingItemToolStripMenuItem.Click += new System.EventHandler(this.existingItemToolStripMenuItem_Click);
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            this.toolStripMenuItem20.Size = new System.Drawing.Size(157, 22);
            this.toolStripMenuItem20.Text = "New Folder";
            this.toolStripMenuItem20.Click += new System.EventHandler(this.toolStripMenuItem20_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(154, 6);
            // 
            // referenceToolStripMenuItem
            // 
            this.referenceToolStripMenuItem.Name = "referenceToolStripMenuItem";
            this.referenceToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.referenceToolStripMenuItem.Text = "Reference";
            // 
            // formToolStripMenuItem
            // 
            this.formToolStripMenuItem.Name = "formToolStripMenuItem";
            this.formToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.formToolStripMenuItem.Text = "Windows Form ";
            this.formToolStripMenuItem.Click += new System.EventHandler(this.formToolStripMenuItem_Click);
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.controlToolStripMenuItem.Text = "Control";
            this.controlToolStripMenuItem.Click += new System.EventHandler(this.controlToolStripMenuItem_Click);
            // 
            // newFolderToolStripMenuItem
            // 
            this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
            this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.newFolderToolStripMenuItem.Text = "New Folder";
            this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(215, 6);
            // 
            // setAsMainProjectToolStripMenuItem
            // 
            this.setAsMainProjectToolStripMenuItem.Name = "setAsMainProjectToolStripMenuItem";
            this.setAsMainProjectToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.setAsMainProjectToolStripMenuItem.Text = "Set as StartUp Project";
            this.setAsMainProjectToolStripMenuItem.Click += new System.EventHandler(this.setAsMainProjectToolStripMenuItem_Click);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // toolStripMenuItem29
            // 
            this.toolStripMenuItem29.Name = "toolStripMenuItem29";
            this.toolStripMenuItem29.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem22
            // 
            this.toolStripMenuItem22.Name = "toolStripMenuItem22";
            this.toolStripMenuItem22.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem22.Text = "Cut";
            // 
            // toolStripMenuItem48
            // 
            this.toolStripMenuItem48.Name = "toolStripMenuItem48";
            this.toolStripMenuItem48.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem48.Text = "Copy";
            this.toolStripMenuItem48.Click += new System.EventHandler(this.toolStripMenuItem48_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem28
            // 
            this.toolStripMenuItem28.Name = "toolStripMenuItem28";
            this.toolStripMenuItem28.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem28.Text = "Paste";
            this.toolStripMenuItem28.Click += new System.EventHandler(this.toolStripMenuItem28_Click);
            // 
            // toolStripMenuItem21
            // 
            this.toolStripMenuItem21.Name = "toolStripMenuItem21";
            this.toolStripMenuItem21.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem21.Text = "Rename";
            this.toolStripMenuItem21.Click += new System.EventHandler(this.toolStripMenuItem21_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(215, 6);
            // 
            // unloadProjectToolStripMenuItem
            // 
            this.unloadProjectToolStripMenuItem.Name = "unloadProjectToolStripMenuItem";
            this.unloadProjectToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.unloadProjectToolStripMenuItem.Text = "Unload Project";
            this.unloadProjectToolStripMenuItem.Click += new System.EventHandler(this.unloadProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(215, 6);
            // 
            // viewProjectFileToolStripMenuItem
            // 
            this.viewProjectFileToolStripMenuItem.Name = "viewProjectFileToolStripMenuItem";
            this.viewProjectFileToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.viewProjectFileToolStripMenuItem.Text = "View Project File";
            this.viewProjectFileToolStripMenuItem.Click += new System.EventHandler(this.viewProjectFileToolStripMenuItem_Click);
            // 
            // openFolderInFileExplorerToolStripMenuItem
            // 
            this.openFolderInFileExplorerToolStripMenuItem.Name = "openFolderInFileExplorerToolStripMenuItem";
            this.openFolderInFileExplorerToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.openFolderInFileExplorerToolStripMenuItem.Text = "Open Folder in File Explorer";
            // 
            // projectPropertiesToolStripMenuItem
            // 
            this.projectPropertiesToolStripMenuItem.Name = "projectPropertiesToolStripMenuItem";
            this.projectPropertiesToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.projectPropertiesToolStripMenuItem.Text = "Project properties";
            this.projectPropertiesToolStripMenuItem.Click += new System.EventHandler(this.projectPropertiesToolStripMenuItem_Click);
            // 
            // mSBuildProjectToolStripMenuItem
            // 
            this.mSBuildProjectToolStripMenuItem.Name = "mSBuildProjectToolStripMenuItem";
            this.mSBuildProjectToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.mSBuildProjectToolStripMenuItem.Text = "MS Build Project";
            this.mSBuildProjectToolStripMenuItem.Click += new System.EventHandler(this.mSBuildProjectToolStripMenuItem_Click);
            // 
            // openCMDToolStripMenuItem
            // 
            this.openCMDToolStripMenuItem.Name = "openCMDToolStripMenuItem";
            this.openCMDToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.openCMDToolStripMenuItem.Text = "Open CMD";
            // 
            // powerShellToolStripMenuItem
            // 
            this.powerShellToolStripMenuItem.Name = "powerShellToolStripMenuItem";
            this.powerShellToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
            this.powerShellToolStripMenuItem.Text = "PowerShell";
            this.powerShellToolStripMenuItem.Click += new System.EventHandler(this.powerShellToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.toolStripMenuItem17,
            this.toolStripMenuItem18,
            this.toolStripMenuItem49,
            this.toolStripMenuItem30,
            this.toolStripSeparator1,
            this.toolStripMenuItem53,
            this.toolStripMenuItem52,
            this.toolStripMenuItem19,
            this.existingProjectToolStripMenuItem,
            this.viewSolutionFileToolStripMenuItem,
            this.toolStripMenuItem51,
            this.toolStripSeparator10,
            this.toolStripMenuItem50,
            this.toolStripSeparator9,
            this.propertiesToolStripMenuItem,
            this.restoreNugetPackagesToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(202, 330);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.newProjectToolStripMenuItem.Text = "Rebuild Solution";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem17.Text = "Build Solution";
            this.toolStripMenuItem17.Click += new System.EventHandler(this.toolStripMenuItem17_Click);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem18.Text = "Clean Solution";
            this.toolStripMenuItem18.Click += new System.EventHandler(this.toolStripMenuItem18_Click);
            // 
            // toolStripMenuItem49
            // 
            this.toolStripMenuItem49.Name = "toolStripMenuItem49";
            this.toolStripMenuItem49.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem49.Text = "Batch build...";
            // 
            // toolStripMenuItem30
            // 
            this.toolStripMenuItem30.Name = "toolStripMenuItem30";
            this.toolStripMenuItem30.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem30.Text = "Configuration Manager";
            this.toolStripMenuItem30.Click += new System.EventHandler(this.toolStripMenuItem30_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // toolStripMenuItem53
            // 
            this.toolStripMenuItem53.Name = "toolStripMenuItem53";
            this.toolStripMenuItem53.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem53.Text = "Project Order";
            // 
            // toolStripMenuItem52
            // 
            this.toolStripMenuItem52.Name = "toolStripMenuItem52";
            this.toolStripMenuItem52.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem52.Text = "Project Dependencies";
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem1,
            this.existingProjetToolStripMenuItem,
            this.toolStripSeparator2,
            this.newItemToolStripMenuItem1,
            this.existingItemToolStripMenuItem1,
            this.newFolderToolStripMenuItem1});
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem19.Text = "Add ";
            // 
            // newProjectToolStripMenuItem1
            // 
            this.newProjectToolStripMenuItem1.Name = "newProjectToolStripMenuItem1";
            this.newProjectToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.newProjectToolStripMenuItem1.Text = "New Project";
            this.newProjectToolStripMenuItem1.Click += new System.EventHandler(this.newProjectToolStripMenuItem1_Click);
            // 
            // existingProjetToolStripMenuItem
            // 
            this.existingProjetToolStripMenuItem.Name = "existingProjetToolStripMenuItem";
            this.existingProjetToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.existingProjetToolStripMenuItem.Text = "Existing Project";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(151, 6);
            // 
            // newItemToolStripMenuItem1
            // 
            this.newItemToolStripMenuItem1.Name = "newItemToolStripMenuItem1";
            this.newItemToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.newItemToolStripMenuItem1.Text = "New Item";
            this.newItemToolStripMenuItem1.Click += new System.EventHandler(this.newItemToolStripMenuItem1_Click);
            // 
            // existingItemToolStripMenuItem1
            // 
            this.existingItemToolStripMenuItem1.Name = "existingItemToolStripMenuItem1";
            this.existingItemToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.existingItemToolStripMenuItem1.Text = "Existing Item";
            // 
            // newFolderToolStripMenuItem1
            // 
            this.newFolderToolStripMenuItem1.Name = "newFolderToolStripMenuItem1";
            this.newFolderToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
            this.newFolderToolStripMenuItem1.Text = "New Folder";
            // 
            // existingProjectToolStripMenuItem
            // 
            this.existingProjectToolStripMenuItem.Name = "existingProjectToolStripMenuItem";
            this.existingProjectToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.existingProjectToolStripMenuItem.Text = "Existing Project";
            this.existingProjectToolStripMenuItem.Click += new System.EventHandler(this.existingProjectToolStripMenuItem_Click);
            // 
            // viewSolutionFileToolStripMenuItem
            // 
            this.viewSolutionFileToolStripMenuItem.Name = "viewSolutionFileToolStripMenuItem";
            this.viewSolutionFileToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.viewSolutionFileToolStripMenuItem.Text = "View Solution File";
            this.viewSolutionFileToolStripMenuItem.Click += new System.EventHandler(this.viewSolutionFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem51
            // 
            this.toolStripMenuItem51.Name = "toolStripMenuItem51";
            this.toolStripMenuItem51.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem51.Text = "Rename";
            this.toolStripMenuItem51.Click += new System.EventHandler(this.toolStripMenuItem51_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(198, 6);
            // 
            // toolStripMenuItem50
            // 
            this.toolStripMenuItem50.Name = "toolStripMenuItem50";
            this.toolStripMenuItem50.Size = new System.Drawing.Size(201, 22);
            this.toolStripMenuItem50.Text = "Open Folder in File View";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(198, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // restoreNugetPackagesToolStripMenuItem
            // 
            this.restoreNugetPackagesToolStripMenuItem.Name = "restoreNugetPackagesToolStripMenuItem";
            this.restoreNugetPackagesToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.restoreNugetPackagesToolStripMenuItem.Text = "Restore Nuget Packages";
            this.restoreNugetPackagesToolStripMenuItem.Click += new System.EventHandler(this.restoreNugetPackagesToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.pROJECTToolStripMenuItem,
            this.bUILDToolStripMenuItem,
            this.toolStripMenuItem80,
            this.tOOLSToolStripMenuItem,
            this.tESTToolStripMenuItem,
            this.wINDOWToolStripMenuItem,
            this.hELPToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1049, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.toolStripMenuItem16,
            this.toolStripMenuItem87,
            this.toolStripSeparator15,
            this.toolStripMenuItem66,
            this.toolStripSeparator16,
            this.toolStripMenuItem14,
            this.toolStripMenuItem13,
            this.toolStripSeparator25,
            this.toolStripMenuItem76,
            this.exportTemplateToolStripMenuItem,
            this.toolStripMenuItem11,
            this.toolStripMenuItem15,
            this.recentProjectsAndSolutionsToolStripMenuItem,
            this.toolStripMenuItem12,
            this.exitToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(40, 20);
            this.toolStripMenuItem1.Text = "FILE";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem2,
            this.websiteToolStripMenuItem,
            this.repositoryToolStripMenuItem,
            this.fileToolStripMenuItem,
            this.projectFromSourcesToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // projectToolStripMenuItem2
            // 
            this.projectToolStripMenuItem2.Image = global::WinExplorer.ve_resource.WebURL_32x;
            this.projectToolStripMenuItem2.Name = "projectToolStripMenuItem2";
            this.projectToolStripMenuItem2.Size = new System.Drawing.Size(183, 22);
            this.projectToolStripMenuItem2.Text = "Project";
            this.projectToolStripMenuItem2.Click += new System.EventHandler(this.projectToolStripMenuItem2_Click);
            // 
            // websiteToolStripMenuItem
            // 
            this.websiteToolStripMenuItem.Image = global::WinExplorer.ve_resource.WebURL_32x;
            this.websiteToolStripMenuItem.Name = "websiteToolStripMenuItem";
            this.websiteToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.websiteToolStripMenuItem.Text = "Website";
            // 
            // repositoryToolStripMenuItem
            // 
            this.repositoryToolStripMenuItem.Name = "repositoryToolStripMenuItem";
            this.repositoryToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.repositoryToolStripMenuItem.Text = "Repository";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // projectFromSourcesToolStripMenuItem
            // 
            this.projectFromSourcesToolStripMenuItem.Name = "projectFromSourcesToolStripMenuItem";
            this.projectFromSourcesToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.projectFromSourcesToolStripMenuItem.Text = "Project from sources";
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectSolutionToolStripMenuItem,
            this.toolStripSeparator26,
            this.toolStripMenuItem78});
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem16.Text = "Open";
            // 
            // projectSolutionToolStripMenuItem
            // 
            this.projectSolutionToolStripMenuItem.Name = "projectSolutionToolStripMenuItem";
            this.projectSolutionToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.projectSolutionToolStripMenuItem.Text = "Project / Solution";
            this.projectSolutionToolStripMenuItem.Click += new System.EventHandler(this.projectSolutionToolStripMenuItem_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(163, 6);
            // 
            // toolStripMenuItem78
            // 
            this.toolStripMenuItem78.Image = global::WinExplorer.ve_resource.FolderOpen_16x;
            this.toolStripMenuItem78.Name = "toolStripMenuItem78";
            this.toolStripMenuItem78.Size = new System.Drawing.Size(166, 22);
            this.toolStripMenuItem78.Text = "Open File";
            // 
            // toolStripMenuItem87
            // 
            this.toolStripMenuItem87.Image = global::WinExplorer.ve_resource.ShowStartPage_256x;
            this.toolStripMenuItem87.Name = "toolStripMenuItem87";
            this.toolStripMenuItem87.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem87.Text = "Start Page";
            this.toolStripMenuItem87.Click += new System.EventHandler(this.toolStripMenuItem87_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripMenuItem66
            // 
            this.toolStripMenuItem66.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem2,
            this.newWebSiteToolStripMenuItem,
            this.toolStripSeparator14,
            this.existingProjectToolStripMenuItem1,
            this.existingWebSiteToolStripMenuItem});
            this.toolStripMenuItem66.Name = "toolStripMenuItem66";
            this.toolStripMenuItem66.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem66.Text = "Add";
            // 
            // newProjectToolStripMenuItem2
            // 
            this.newProjectToolStripMenuItem2.Name = "newProjectToolStripMenuItem2";
            this.newProjectToolStripMenuItem2.Size = new System.Drawing.Size(163, 22);
            this.newProjectToolStripMenuItem2.Text = "New Project";
            // 
            // newWebSiteToolStripMenuItem
            // 
            this.newWebSiteToolStripMenuItem.Name = "newWebSiteToolStripMenuItem";
            this.newWebSiteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.newWebSiteToolStripMenuItem.Text = "New Web Site";
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(160, 6);
            // 
            // existingProjectToolStripMenuItem1
            // 
            this.existingProjectToolStripMenuItem1.Name = "existingProjectToolStripMenuItem1";
            this.existingProjectToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.existingProjectToolStripMenuItem1.Text = "Existing Project";
            // 
            // existingWebSiteToolStripMenuItem
            // 
            this.existingWebSiteToolStripMenuItem.Name = "existingWebSiteToolStripMenuItem";
            this.existingWebSiteToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.existingWebSiteToolStripMenuItem.Text = "Existing Web Site";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem14.Text = "Close Solution";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.toolStripMenuItem14_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem13.Text = "Close All";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.toolStripMenuItem13_Click);
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripMenuItem76
            // 
            this.toolStripMenuItem76.Image = global::WinExplorer.ve_resource.SaveAll_256x;
            this.toolStripMenuItem76.Name = "toolStripMenuItem76";
            this.toolStripMenuItem76.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.toolStripMenuItem76.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem76.Text = "Save All";
            // 
            // exportTemplateToolStripMenuItem
            // 
            this.exportTemplateToolStripMenuItem.Name = "exportTemplateToolStripMenuItem";
            this.exportTemplateToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.exportTemplateToolStripMenuItem.Text = "Export Template";
            this.exportTemplateToolStripMenuItem.Click += new System.EventHandler(this.exportTemplateToolStripMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(227, 6);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(230, 22);
            this.toolStripMenuItem15.Text = "Recent Files";
            // 
            // recentProjectsAndSolutionsToolStripMenuItem
            // 
            this.recentProjectsAndSolutionsToolStripMenuItem.Name = "recentProjectsAndSolutionsToolStripMenuItem";
            this.recentProjectsAndSolutionsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.recentProjectsAndSolutionsToolStripMenuItem.Text = "Recent Projects and Solutions";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(227, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F3)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem10,
            this.toolStripMenuItem9,
            this.toolStripMenuItem70,
            this.toolStripMenuItem69,
            this.toolStripSeparator22,
            this.toolStripMenuItem72,
            this.toolStripMenuItem73,
            this.toolStripMenuItem74,
            this.toolStripMenuItem71,
            this.toolStripMenuItem75,
            this.toolStripSeparator24,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator23,
            this.findToolStripMenuItem,
            this.advancedToolStripMenuItem,
            this.bookmarksToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.editToolStripMenuItem.Text = "EDIT";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Image = global::WinExplorer.ve_resource.Undo_16x;
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem10.Text = "Undo";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.toolStripMenuItem10_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Image = global::WinExplorer.ve_resource.Redo_16x;
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem9.Text = "Redo";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
            // 
            // toolStripMenuItem70
            // 
            this.toolStripMenuItem70.Name = "toolStripMenuItem70";
            this.toolStripMenuItem70.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem70.Text = "Undo last Global Action";
            // 
            // toolStripMenuItem69
            // 
            this.toolStripMenuItem69.Name = "toolStripMenuItem69";
            this.toolStripMenuItem69.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem69.Text = "Redo last Global Action";
            this.toolStripMenuItem69.Click += new System.EventHandler(this.toolStripMenuItem69_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(196, 6);
            // 
            // toolStripMenuItem72
            // 
            this.toolStripMenuItem72.Image = global::WinExplorer.ve_resource.Cut_24x;
            this.toolStripMenuItem72.Name = "toolStripMenuItem72";
            this.toolStripMenuItem72.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.toolStripMenuItem72.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem72.Text = "Cut";
            this.toolStripMenuItem72.Click += new System.EventHandler(this.toolStripMenuItem72_Click);
            // 
            // toolStripMenuItem73
            // 
            this.toolStripMenuItem73.Image = global::WinExplorer.ve_resource.Copy_32x;
            this.toolStripMenuItem73.Name = "toolStripMenuItem73";
            this.toolStripMenuItem73.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMenuItem73.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem73.Text = "Copy";
            this.toolStripMenuItem73.Click += new System.EventHandler(this.toolStripMenuItem73_Click);
            // 
            // toolStripMenuItem74
            // 
            this.toolStripMenuItem74.Image = global::WinExplorer.ve_resource.Paste_16x;
            this.toolStripMenuItem74.Name = "toolStripMenuItem74";
            this.toolStripMenuItem74.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripMenuItem74.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem74.Text = "Paste";
            // 
            // toolStripMenuItem71
            // 
            this.toolStripMenuItem71.Name = "toolStripMenuItem71";
            this.toolStripMenuItem71.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem71.Text = "Paste Special";
            // 
            // toolStripMenuItem75
            // 
            this.toolStripMenuItem75.Image = global::WinExplorer.ve_resource.Cancel_256x;
            this.toolStripMenuItem75.Name = "toolStripMenuItem75";
            this.toolStripMenuItem75.Size = new System.Drawing.Size(199, 22);
            this.toolStripMenuItem75.Text = "Delete";
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(196, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(196, 6);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem1,
            this.findAndReplaceToolStripMenuItem});
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.findToolStripMenuItem.Text = "Find and Replace";
            // 
            // findToolStripMenuItem1
            // 
          
            // 
            // commentSectionToolStripMenuItem
            // 
            this.commentSectionToolStripMenuItem.Name = "commentSectionToolStripMenuItem";
            this.commentSectionToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.commentSectionToolStripMenuItem.Text = "Comment Section";
            this.commentSectionToolStripMenuItem.Click += new System.EventHandler(this.commentSectionToolStripMenuItem_Click);
            // 
            // uncommentSectionToolStripMenuItem
            // 
            this.uncommentSectionToolStripMenuItem.Name = "uncommentSectionToolStripMenuItem";
            this.uncommentSectionToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.uncommentSectionToolStripMenuItem.Text = "Uncomment Section";
            this.uncommentSectionToolStripMenuItem.Click += new System.EventHandler(this.uncommentSectionToolStripMenuItem_Click);
            // 
            // bookmarksToolStripMenuItem
            // 
            this.bookmarksToolStripMenuItem.Name = "bookmarksToolStripMenuItem";
            this.bookmarksToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.bookmarksToolStripMenuItem.Text = "Bookmarks";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem57,
            this.toolStripMenuItem58,
            this.toolStripMenuItem59,
            this.toolStripMenuItem56,
            this.toolStripSeparator11,
            this.solutionExplorerToolStripMenuItem,
            this.toolStripMenuItem83,
            this.toolStripMenuItem82,
            this.toolStripSeparator12,
            this.toolStripMenuItem60,
            this.toolStripMenuItem61,
            this.classViewToolStripMenuItem,
            this.toolStripMenuItem62,
            this.toolStripSeparator13,
            this.errorListToolStripMenuItem,
            this.outputWindowToolStripMenuItem1,
            this.toolStripMenuItem64,
            this.toolStripMenuItem84,
            this.findResultsToolStripMenuItem,
            this.toolStripMenuItem65,
            this.toolStripSeparator27,
            this.toolStripMenuItem79,
            this.recentProjectsToolStripMenuItem,
            this.fileExplorerToolStripMenuItem,
            this.propertyGridToolStripMenuItem,
            this.outputWindowToolStripMenuItem,
            this.codeAnalysisToolStripMenuItem,
            this.outlineWindowToolStripMenuItem,
            this.nUnitToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.viewToolStripMenuItem.Text = "VIEW";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem57
            // 
            this.toolStripMenuItem57.Name = "toolStripMenuItem57";
            this.toolStripMenuItem57.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem57.Text = "Code";
            // 
            // toolStripMenuItem58
            // 
            this.toolStripMenuItem58.Name = "toolStripMenuItem58";
            this.toolStripMenuItem58.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem58.Text = "Designer";
            // 
            // toolStripMenuItem59
            // 
            this.toolStripMenuItem59.Name = "toolStripMenuItem59";
            this.toolStripMenuItem59.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem59.Text = "Open";
            // 
            // toolStripMenuItem56
            // 
            this.toolStripMenuItem56.Name = "toolStripMenuItem56";
            this.toolStripMenuItem56.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem56.Text = "Open with";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(213, 6);
            // 
            // solutionExplorerToolStripMenuItem
            // 
            this.solutionExplorerToolStripMenuItem.Name = "solutionExplorerToolStripMenuItem";
            this.solutionExplorerToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.solutionExplorerToolStripMenuItem.Text = "Solution Explorer";
            this.solutionExplorerToolStripMenuItem.Click += new System.EventHandler(this.solutionExplorerToolStripMenuItem_Click);
            // 
            // toolStripMenuItem83
            // 
            this.toolStripMenuItem83.Name = "toolStripMenuItem83";
            this.toolStripMenuItem83.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem83.Text = "Server Explorer";
            this.toolStripMenuItem83.Click += new System.EventHandler(this.toolStripMenuItem83_Click);
            // 
            // toolStripMenuItem82
            // 
            this.toolStripMenuItem82.Name = "toolStripMenuItem82";
            this.toolStripMenuItem82.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem82.Text = "SQL Server Object Explorer";
            this.toolStripMenuItem82.Click += new System.EventHandler(this.toolStripMenuItem82_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(213, 6);
            // 
            // toolStripMenuItem60
            // 
            this.toolStripMenuItem60.Name = "toolStripMenuItem60";
            this.toolStripMenuItem60.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.toolStripMenuItem60.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem60.Text = "Bookmark Window";
            // 
            // toolStripMenuItem61
            // 
            this.toolStripMenuItem61.Name = "toolStripMenuItem61";
            this.toolStripMenuItem61.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem61.Text = "Call Hierarchy";
            // 
            // classViewToolStripMenuItem
            // 
            this.classViewToolStripMenuItem.Name = "classViewToolStripMenuItem";
            this.classViewToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.classViewToolStripMenuItem.Text = "Class View";
            this.classViewToolStripMenuItem.Click += new System.EventHandler(this.classViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem62
            // 
            this.toolStripMenuItem62.Name = "toolStripMenuItem62";
            this.toolStripMenuItem62.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem62.Text = "Object Browser";
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(213, 6);
            // 
            // errorListToolStripMenuItem
            // 
            this.errorListToolStripMenuItem.Name = "errorListToolStripMenuItem";
            this.errorListToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.errorListToolStripMenuItem.Text = "Error List";
            this.errorListToolStripMenuItem.Click += new System.EventHandler(this.errorListToolStripMenuItem_Click);
            // 
            // outputWindowToolStripMenuItem1
            // 
            this.outputWindowToolStripMenuItem1.Name = "outputWindowToolStripMenuItem1";
            this.outputWindowToolStripMenuItem1.Size = new System.Drawing.Size(216, 22);
            this.outputWindowToolStripMenuItem1.Text = "Output window";
            this.outputWindowToolStripMenuItem1.Click += new System.EventHandler(this.outputWindowToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem64
            // 
            this.toolStripMenuItem64.Name = "toolStripMenuItem64";
            this.toolStripMenuItem64.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem64.Text = "TaskList";
            // 
            // toolStripMenuItem84
            // 
            this.toolStripMenuItem84.Name = "toolStripMenuItem84";
            this.toolStripMenuItem84.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem84.Text = "Toolbox";
            this.toolStripMenuItem84.Click += new System.EventHandler(this.toolStripMenuItem84_Click);
            // 
            // findResultsToolStripMenuItem
            // 
            this.findResultsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findResults1ToolStripMenuItem,
            this.findResults2ToolStripMenuItem});
            this.findResultsToolStripMenuItem.Name = "findResultsToolStripMenuItem";
            this.findResultsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.findResultsToolStripMenuItem.Text = "Find results";
            this.findResultsToolStripMenuItem.Click += new System.EventHandler(this.findResultsToolStripMenuItem_Click);
            // 
            // findResults1ToolStripMenuItem
            // 
            this.findResults1ToolStripMenuItem.Name = "findResults1ToolStripMenuItem";
            this.findResults1ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.findResults1ToolStripMenuItem.Text = "Find Results 1";
            this.findResults1ToolStripMenuItem.Click += new System.EventHandler(this.findResults1ToolStripMenuItem_Click);
            // 
            // findResults2ToolStripMenuItem
            // 
            this.findResults2ToolStripMenuItem.Name = "findResults2ToolStripMenuItem";
            this.findResults2ToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.findResults2ToolStripMenuItem.Text = "Find Results 2";
            this.findResults2ToolStripMenuItem.Click += new System.EventHandler(this.findResults2ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem65
            // 
            this.toolStripMenuItem65.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commandWindowToolStripMenuItem,
            this.webBrowserToolStripMenuItem,
            this.dataSourcesToolStripMenuItem,
            this.toolStripMenuItem81,
            this.powerShellWindowToolStripMenuItem,
            this.xmlDocumentOutlineToolStripMenuItem,
            this.syntaxTreeVisualizatorToolStripMenuItem});
            this.toolStripMenuItem65.Name = "toolStripMenuItem65";
            this.toolStripMenuItem65.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem65.Text = "Other Windows";
            // 
            // commandWindowToolStripMenuItem
            // 
            this.commandWindowToolStripMenuItem.Name = "commandWindowToolStripMenuItem";
            this.commandWindowToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.commandWindowToolStripMenuItem.Text = "Command Window";
            this.commandWindowToolStripMenuItem.Click += new System.EventHandler(this.commandWindowToolStripMenuItem_Click);
            // 
            // webBrowserToolStripMenuItem
            // 
            this.webBrowserToolStripMenuItem.Name = "webBrowserToolStripMenuItem";
            this.webBrowserToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.webBrowserToolStripMenuItem.Text = "Web Browser";
            // 
            // dataSourcesToolStripMenuItem
            // 
            this.dataSourcesToolStripMenuItem.Image = global::WinExplorer.ve_resource.DataSourceView_16x;
            this.dataSourcesToolStripMenuItem.Name = "dataSourcesToolStripMenuItem";
            this.dataSourcesToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.dataSourcesToolStripMenuItem.Text = "Data Sources";
            this.dataSourcesToolStripMenuItem.Click += new System.EventHandler(this.dataSourcesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem81
            // 
            this.toolStripMenuItem81.Name = "toolStripMenuItem81";
            this.toolStripMenuItem81.Size = new System.Drawing.Size(193, 6);
            this.toolStripMenuItem81.Click += new System.EventHandler(this.toolStripMenuItem81_Click);
            // 
            // powerShellWindowToolStripMenuItem
            // 
            this.powerShellWindowToolStripMenuItem.Name = "powerShellWindowToolStripMenuItem";
            this.powerShellWindowToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.powerShellWindowToolStripMenuItem.Text = "PowerShell Window";
            this.powerShellWindowToolStripMenuItem.Click += new System.EventHandler(this.powerShellWindowToolStripMenuItem_Click);
            // 
            // xmlDocumentOutlineToolStripMenuItem
            // 
            this.xmlDocumentOutlineToolStripMenuItem.Name = "xmlDocumentOutlineToolStripMenuItem";
            this.xmlDocumentOutlineToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.xmlDocumentOutlineToolStripMenuItem.Text = "XmlDocument Outline";
            this.xmlDocumentOutlineToolStripMenuItem.Click += new System.EventHandler(this.xmlDocumentOutlineToolStripMenuItem_Click);
            // 
            // syntaxTreeVisualizatorToolStripMenuItem
            // 
            this.syntaxTreeVisualizatorToolStripMenuItem.Name = "syntaxTreeVisualizatorToolStripMenuItem";
            this.syntaxTreeVisualizatorToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.syntaxTreeVisualizatorToolStripMenuItem.Text = "Syntax Tree Visualizator";
            this.syntaxTreeVisualizatorToolStripMenuItem.Click += new System.EventHandler(this.syntaxTreeVisualizatorToolStripMenuItem_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            this.toolStripSeparator27.Size = new System.Drawing.Size(213, 6);
            // 
            // toolStripMenuItem79
            // 
            this.toolStripMenuItem79.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.standardToolStripMenuItem,
            this.buildToolStripMenuItem1,
            this.debugToolStripMenuItem,
            this.toolStripSeparator28,
            this.customizeToolStripMenuItem});
            this.toolStripMenuItem79.Name = "toolStripMenuItem79";
            this.toolStripMenuItem79.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItem79.Text = "Toolbars";
            // 
            // standardToolStripMenuItem
            // 
            this.standardToolStripMenuItem.Name = "standardToolStripMenuItem";
            this.standardToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.standardToolStripMenuItem.Text = "Standard";
            // 
            // buildToolStripMenuItem1
            // 
            this.buildToolStripMenuItem1.Name = "buildToolStripMenuItem1";
            this.buildToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
            this.buildToolStripMenuItem1.Text = "Build";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            this.toolStripSeparator28.Size = new System.Drawing.Size(127, 6);
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.customizeToolStripMenuItem.Text = "Customize";
            this.customizeToolStripMenuItem.Click += new System.EventHandler(this.customizeToolStripMenuItem_Click);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.recentProjectsToolStripMenuItem.Text = "Recent projects";
            this.recentProjectsToolStripMenuItem.Click += new System.EventHandler(this.recentProjectsToolStripMenuItem_Click);
            // 
            // fileExplorerToolStripMenuItem
            // 
            this.fileExplorerToolStripMenuItem.Name = "fileExplorerToolStripMenuItem";
            this.fileExplorerToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.fileExplorerToolStripMenuItem.Text = "File Explorer";
            this.fileExplorerToolStripMenuItem.Click += new System.EventHandler(this.fileExplorerToolStripMenuItem_Click);
            // 
            // propertyGridToolStripMenuItem
            // 
            this.propertyGridToolStripMenuItem.Image = global::WinExplorer.ve_resource.Property_256x;
            this.propertyGridToolStripMenuItem.Name = "propertyGridToolStripMenuItem";
            this.propertyGridToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.propertyGridToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.propertyGridToolStripMenuItem.Text = "Properties Window";
            this.propertyGridToolStripMenuItem.Click += new System.EventHandler(this.propertyGridToolStripMenuItem_Click);
            // 
            // outputWindowToolStripMenuItem
            // 
            this.outputWindowToolStripMenuItem.Name = "outputWindowToolStripMenuItem";
            this.outputWindowToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.outputWindowToolStripMenuItem.Text = "Output window";
            this.outputWindowToolStripMenuItem.Click += new System.EventHandler(this.outputWindowToolStripMenuItem_Click);
            // 
            // codeAnalysisToolStripMenuItem
            // 
            this.codeAnalysisToolStripMenuItem.Name = "codeAnalysisToolStripMenuItem";
            this.codeAnalysisToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.codeAnalysisToolStripMenuItem.Text = "Code Analysis";
            this.codeAnalysisToolStripMenuItem.Click += new System.EventHandler(this.codeAnalysisToolStripMenuItem_Click);
            // 
            // outlineWindowToolStripMenuItem
            // 
            this.outlineWindowToolStripMenuItem.Name = "outlineWindowToolStripMenuItem";
            this.outlineWindowToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.outlineWindowToolStripMenuItem.Text = "Outline window";
            this.outlineWindowToolStripMenuItem.Click += new System.EventHandler(this.outlineWindowToolStripMenuItem_Click);
            // 
            // nUnitToolStripMenuItem
            // 
            this.nUnitToolStripMenuItem.Name = "nUnitToolStripMenuItem";
            this.nUnitToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.nUnitToolStripMenuItem.Text = "NUnit";
            this.nUnitToolStripMenuItem.Click += new System.EventHandler(this.nUnitToolStripMenuItem_Click);
            // 
            // pROJECTToolStripMenuItem
            // 
            this.pROJECTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addWindowsFormToolStripMenuItem,
            this.addUserControlToolStripMenuItem,
            this.addComponentToolStripMenuItem,
            this.addNewClassToolStripMenuItem,
            this.toolStripMenuItem46,
            this.addNewItemToolStripMenuItem,
            this.addExistingItemToolStripMenuItem,
            this.toolStripMenuItem47,
            this.toolStripMenuItem67,
            this.toolStripMenuItem68,
            this.toolStripSeparator19,
            this.addReferenceToolStripMenuItem1,
            this.addProjectReferenceToolStripMenuItem,
            this.addAnalyzerToolStripMenuItem,
            this.setAsStartupProjectToolStripMenuItem,
            this.projectDependenciesToolStripMenuItem,
            this.projectBuildOrderToolStripMenuItem,
            this.toolStripSeparator20,
            this.projectPropertiesToolStripMenuItem1});
            this.pROJECTToolStripMenuItem.Name = "pROJECTToolStripMenuItem";
            this.pROJECTToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.pROJECTToolStripMenuItem.Text = "PROJECT";
            // 
            // addWindowsFormToolStripMenuItem
            // 
            this.addWindowsFormToolStripMenuItem.Image = global::WinExplorer.ve_resource.AddForm_16x;
            this.addWindowsFormToolStripMenuItem.Name = "addWindowsFormToolStripMenuItem";
            this.addWindowsFormToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addWindowsFormToolStripMenuItem.Text = "Add Windows Form";
            // 
            // addUserControlToolStripMenuItem
            // 
            this.addUserControlToolStripMenuItem.Name = "addUserControlToolStripMenuItem";
            this.addUserControlToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addUserControlToolStripMenuItem.Text = "Add User Control";
            // 
            // addComponentToolStripMenuItem
            // 
            this.addComponentToolStripMenuItem.Image = global::WinExplorer.ve_resource.AddComponent_16x;
            this.addComponentToolStripMenuItem.Name = "addComponentToolStripMenuItem";
            this.addComponentToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addComponentToolStripMenuItem.Text = "Add Component";
            // 
            // addNewClassToolStripMenuItem
            // 
            this.addNewClassToolStripMenuItem.Name = "addNewClassToolStripMenuItem";
            this.addNewClassToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addNewClassToolStripMenuItem.Text = "Add New Class";
            // 
            // toolStripMenuItem46
            // 
            this.toolStripMenuItem46.Name = "toolStripMenuItem46";
            this.toolStripMenuItem46.Size = new System.Drawing.Size(221, 6);
            // 
            // addNewItemToolStripMenuItem
            // 
            this.addNewItemToolStripMenuItem.Image = global::WinExplorer.ve_resource.NewItem_16x;
            this.addNewItemToolStripMenuItem.Name = "addNewItemToolStripMenuItem";
            this.addNewItemToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.addNewItemToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addNewItemToolStripMenuItem.Text = "Add New Item";
            // 
            // addExistingItemToolStripMenuItem
            // 
            this.addExistingItemToolStripMenuItem.Name = "addExistingItemToolStripMenuItem";
            this.addExistingItemToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addExistingItemToolStripMenuItem.Text = "Add Existing Item";
            // 
            // toolStripMenuItem47
            // 
            this.toolStripMenuItem47.Name = "toolStripMenuItem47";
            this.toolStripMenuItem47.Size = new System.Drawing.Size(221, 6);
            // 
            // toolStripMenuItem67
            // 
            this.toolStripMenuItem67.Name = "toolStripMenuItem67";
            this.toolStripMenuItem67.Size = new System.Drawing.Size(224, 22);
            this.toolStripMenuItem67.Text = "Exclude from Project";
            // 
            // toolStripMenuItem68
            // 
            this.toolStripMenuItem68.Name = "toolStripMenuItem68";
            this.toolStripMenuItem68.Size = new System.Drawing.Size(224, 22);
            this.toolStripMenuItem68.Text = "Show All Files";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(221, 6);
            // 
            // addReferenceToolStripMenuItem1
            // 
            this.addReferenceToolStripMenuItem1.Name = "addReferenceToolStripMenuItem1";
            this.addReferenceToolStripMenuItem1.Size = new System.Drawing.Size(224, 22);
            this.addReferenceToolStripMenuItem1.Text = "Add Reference";
            // 
            // addProjectReferenceToolStripMenuItem
            // 
            this.addProjectReferenceToolStripMenuItem.Name = "addProjectReferenceToolStripMenuItem";
            this.addProjectReferenceToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addProjectReferenceToolStripMenuItem.Text = "Add Project Reference";
            // 
            // addAnalyzerToolStripMenuItem
            // 
            this.addAnalyzerToolStripMenuItem.Name = "addAnalyzerToolStripMenuItem";
            this.addAnalyzerToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addAnalyzerToolStripMenuItem.Text = "Add Analyzer";
            // 
            // setAsStartupProjectToolStripMenuItem
            // 
            this.setAsStartupProjectToolStripMenuItem.Image = global::WinExplorer.ve_resource.StartupProject_32x;
            this.setAsStartupProjectToolStripMenuItem.Name = "setAsStartupProjectToolStripMenuItem";
            this.setAsStartupProjectToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.setAsStartupProjectToolStripMenuItem.Text = "Set as Startup Project";
            // 
            // projectDependenciesToolStripMenuItem
            // 
            this.projectDependenciesToolStripMenuItem.Name = "projectDependenciesToolStripMenuItem";
            this.projectDependenciesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.projectDependenciesToolStripMenuItem.Text = "Project Dependencies";
            // 
            // projectBuildOrderToolStripMenuItem
            // 
            this.projectBuildOrderToolStripMenuItem.Name = "projectBuildOrderToolStripMenuItem";
            this.projectBuildOrderToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.projectBuildOrderToolStripMenuItem.Text = "Project Build Order";
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(221, 6);
            // 
            // projectPropertiesToolStripMenuItem1
            // 
            this.projectPropertiesToolStripMenuItem1.Name = "projectPropertiesToolStripMenuItem1";
            this.projectPropertiesToolStripMenuItem1.Size = new System.Drawing.Size(224, 22);
            this.projectPropertiesToolStripMenuItem1.Text = "Project Properties";
            // 
            // bUILDToolStripMenuItem
            // 
            this.bUILDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildSolutionToolStripMenuItem,
            this.rebuildSolutionToolStripMenuItem,
            this.cleanSolutionToolStripMenuItem,
            this.runCodeAnalysisOnSolutionToolStripMenuItem,
            this.toolStripSeparator17,
            this.buildProjectToolStripMenuItem,
            this.rebuildProjectToolStripMenuItem,
            this.cleanProjectToolStripMenuItem,
            this.publishProjectToolStripMenuItem,
            this.runCodeAnalysisOnProjectToolStripMenuItem,
            this.toolStripSeparator18,
            this.configurationManagerToolStripMenuItem});
            this.bUILDToolStripMenuItem.Name = "bUILDToolStripMenuItem";
            this.bUILDToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.bUILDToolStripMenuItem.Text = "BUILD";
            // 
            // buildSolutionToolStripMenuItem
            // 
            this.buildSolutionToolStripMenuItem.Image = global::WinExplorer.ve_resource.BuildSolution_256x;
            this.buildSolutionToolStripMenuItem.Name = "buildSolutionToolStripMenuItem";
            this.buildSolutionToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.buildSolutionToolStripMenuItem.Text = "Build solution";
            this.buildSolutionToolStripMenuItem.Click += new System.EventHandler(this.buildSolutionToolStripMenuItem_Click);
            // 
            // rebuildSolutionToolStripMenuItem
            // 
            this.rebuildSolutionToolStripMenuItem.Name = "rebuildSolutionToolStripMenuItem";
            this.rebuildSolutionToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.rebuildSolutionToolStripMenuItem.Text = "Rebuild solution";
            // 
            // cleanSolutionToolStripMenuItem
            // 
            this.cleanSolutionToolStripMenuItem.Name = "cleanSolutionToolStripMenuItem";
            this.cleanSolutionToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.cleanSolutionToolStripMenuItem.Text = "Clean solution";
            // 
            // runCodeAnalysisOnSolutionToolStripMenuItem
            // 
            this.runCodeAnalysisOnSolutionToolStripMenuItem.Name = "runCodeAnalysisOnSolutionToolStripMenuItem";
            this.runCodeAnalysisOnSolutionToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.runCodeAnalysisOnSolutionToolStripMenuItem.Text = "Run Code Analysis on Solution";
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(233, 6);
            // 
            // buildProjectToolStripMenuItem
            // 
            this.buildProjectToolStripMenuItem.Name = "buildProjectToolStripMenuItem";
            this.buildProjectToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.buildProjectToolStripMenuItem.Text = "Build Project";
            // 
            // rebuildProjectToolStripMenuItem
            // 
            this.rebuildProjectToolStripMenuItem.Name = "rebuildProjectToolStripMenuItem";
            this.rebuildProjectToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.rebuildProjectToolStripMenuItem.Text = "Rebuild Project";
            // 
            // cleanProjectToolStripMenuItem
            // 
            this.cleanProjectToolStripMenuItem.Name = "cleanProjectToolStripMenuItem";
            this.cleanProjectToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.cleanProjectToolStripMenuItem.Text = "Clean Project";
            // 
            // publishProjectToolStripMenuItem
            // 
            this.publishProjectToolStripMenuItem.Name = "publishProjectToolStripMenuItem";
            this.publishProjectToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.publishProjectToolStripMenuItem.Text = "Publish Project";
            // 
            // runCodeAnalysisOnProjectToolStripMenuItem
            // 
            this.runCodeAnalysisOnProjectToolStripMenuItem.Name = "runCodeAnalysisOnProjectToolStripMenuItem";
            this.runCodeAnalysisOnProjectToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.runCodeAnalysisOnProjectToolStripMenuItem.Text = "Run Code Analysis on Project";
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(233, 6);
            // 
            // configurationManagerToolStripMenuItem
            // 
            this.configurationManagerToolStripMenuItem.Name = "configurationManagerToolStripMenuItem";
            this.configurationManagerToolStripMenuItem.Size = new System.Drawing.Size(236, 22);
            this.configurationManagerToolStripMenuItem.Text = "Configuration Manager";
            // 
            // toolStripMenuItem80
            // 
            this.toolStripMenuItem80.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowsToolStripMenuItem,
            this.graphicsToolStripMenuItem,
            this.toolStripSeparator29,
            this.startDebuggingToolStripMenuItem,
            this.startWithoutDebuggingToolStripMenuItem,
            this.performanceProfilerToolStripMenuItem,
            this.attachToProcessToolStripMenuItem,
            this.profilerToolStripMenuItem,
            this.toolStripSeparator31,
            this.stepInToolStripMenuItem,
            this.stepOutToolStripMenuItem});
            this.toolStripMenuItem80.Name = "toolStripMenuItem80";
            this.toolStripMenuItem80.Size = new System.Drawing.Size(56, 20);
            this.toolStripMenuItem80.Text = "DEBUG";
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.breakpointsToolStripMenuItem,
            this.exceptionSettingsToolStripMenuItem,
            this.outputToolStripMenuItem,
            this.showDiagnosticToolsToolStripMenuItem,
            this.toolStripSeparator30,
            this.immediateToolStripMenuItem,
            this.watchToolStripMenuItem});
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.windowsToolStripMenuItem.Text = "Windows";
            // 
            // breakpointsToolStripMenuItem
            // 
            this.breakpointsToolStripMenuItem.Image = global::WinExplorer.ve_resource.BreakpointWindow_16x;
            this.breakpointsToolStripMenuItem.Name = "breakpointsToolStripMenuItem";
            this.breakpointsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.breakpointsToolStripMenuItem.Text = "Breakpoints";
            this.breakpointsToolStripMenuItem.Click += new System.EventHandler(this.breakpointsToolStripMenuItem_Click);
            // 
            // exceptionSettingsToolStripMenuItem
            // 
            this.exceptionSettingsToolStripMenuItem.Image = global::WinExplorer.ve_resource.ExceptionSettings_16x;
            this.exceptionSettingsToolStripMenuItem.Name = "exceptionSettingsToolStripMenuItem";
            this.exceptionSettingsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.exceptionSettingsToolStripMenuItem.Text = "Exception Settings";
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.Image = global::WinExplorer.ve_resource.Output_256x;
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.outputToolStripMenuItem.Text = "Output";
            // 
            // showDiagnosticToolsToolStripMenuItem
            // 
            this.showDiagnosticToolsToolStripMenuItem.Name = "showDiagnosticToolsToolStripMenuItem";
            this.showDiagnosticToolsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.showDiagnosticToolsToolStripMenuItem.Text = "Show Diagnostic Tools";
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            this.toolStripSeparator30.Size = new System.Drawing.Size(190, 6);
            // 
            // immediateToolStripMenuItem
            // 
            this.immediateToolStripMenuItem.Image = global::WinExplorer.ve_resource.ImmediateWindow_16x;
            this.immediateToolStripMenuItem.Name = "immediateToolStripMenuItem";
            this.immediateToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.immediateToolStripMenuItem.Text = "Immediate";
            // 
            // watchToolStripMenuItem
            // 
            this.watchToolStripMenuItem.Name = "watchToolStripMenuItem";
            this.watchToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.watchToolStripMenuItem.Text = "Watch";
            this.watchToolStripMenuItem.Click += new System.EventHandler(this.watchToolStripMenuItem_Click);
            // 
            // graphicsToolStripMenuItem
            // 
            this.graphicsToolStripMenuItem.Name = "graphicsToolStripMenuItem";
            this.graphicsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.graphicsToolStripMenuItem.Text = "Graphics";
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            this.toolStripSeparator29.Size = new System.Drawing.Size(200, 6);
            // 
            // startDebuggingToolStripMenuItem
            // 
            this.startDebuggingToolStripMenuItem.Image = global::WinExplorer.ve_resource.Run_256x;
            this.startDebuggingToolStripMenuItem.Name = "startDebuggingToolStripMenuItem";
            this.startDebuggingToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.startDebuggingToolStripMenuItem.Text = "Start Debugging";
            // 
            // startWithoutDebuggingToolStripMenuItem
            // 
            this.startWithoutDebuggingToolStripMenuItem.Image = global::WinExplorer.ve_resource.StartWithoutDebug_16x;
            this.startWithoutDebuggingToolStripMenuItem.Name = "startWithoutDebuggingToolStripMenuItem";
            this.startWithoutDebuggingToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.startWithoutDebuggingToolStripMenuItem.Text = "Start without debugging";
            // 
            // performanceProfilerToolStripMenuItem
            // 
            this.performanceProfilerToolStripMenuItem.Name = "performanceProfilerToolStripMenuItem";
            this.performanceProfilerToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.performanceProfilerToolStripMenuItem.Text = "Performance Profiler";
            // 
            // attachToProcessToolStripMenuItem
            // 
            this.attachToProcessToolStripMenuItem.Name = "attachToProcessToolStripMenuItem";
            this.attachToProcessToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.attachToProcessToolStripMenuItem.Text = "Attach to process";
            // 
            // profilerToolStripMenuItem
            // 
            this.profilerToolStripMenuItem.Name = "profilerToolStripMenuItem";
            this.profilerToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.profilerToolStripMenuItem.Text = "Profiler";
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            this.toolStripSeparator31.Size = new System.Drawing.Size(200, 6);
            // 
            // stepInToolStripMenuItem
            // 
            this.stepInToolStripMenuItem.Name = "stepInToolStripMenuItem";
            this.stepInToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.stepInToolStripMenuItem.Text = "Step in";
            // 
            // stepOutToolStripMenuItem
            // 
            this.stepOutToolStripMenuItem.Name = "stepOutToolStripMenuItem";
            this.stepOutToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.stepOutToolStripMenuItem.Text = "Step out";
            // 
            // tOOLSToolStripMenuItem
            // 
            this.tOOLSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.externalToolsToolStripMenuItem,
            this.importAndExportSettingsToolStripMenuItem,
            this.custToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.tOOLSToolStripMenuItem.Name = "tOOLSToolStripMenuItem";
            this.tOOLSToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.tOOLSToolStripMenuItem.Text = "TOOLS";
            // 
            // externalToolsToolStripMenuItem
            // 
            this.externalToolsToolStripMenuItem.Name = "externalToolsToolStripMenuItem";
            this.externalToolsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.externalToolsToolStripMenuItem.Text = "External Tools";
            this.externalToolsToolStripMenuItem.Click += new System.EventHandler(this.externalToolsToolStripMenuItem_Click);
            // 
            // importAndExportSettingsToolStripMenuItem
            // 
            this.importAndExportSettingsToolStripMenuItem.Name = "importAndExportSettingsToolStripMenuItem";
            this.importAndExportSettingsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.importAndExportSettingsToolStripMenuItem.Text = "Import and Export Settings";
            // 
            // custToolStripMenuItem
            // 
            this.custToolStripMenuItem.Name = "custToolStripMenuItem";
            this.custToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.custToolStripMenuItem.Text = "Customize ";
            this.custToolStripMenuItem.Click += new System.EventHandler(this.custToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // tESTToolStripMenuItem
            // 
            this.tESTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedTestsToolStripMenuItem,
            this.runTestsToolStripMenuItem,
            this.testSettingsToolStripMenuItem,
            this.vSExplorerTestsToolStripMenuItem,
            this.toolStripMenuItem85,
            this.windowToolStripMenuItem1,
            this.lastRunTestsToolStripMenuItem});
            this.tESTToolStripMenuItem.Name = "tESTToolStripMenuItem";
            this.tESTToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.tESTToolStripMenuItem.Text = "TEST";
            // 
            // selectedTestsToolStripMenuItem
            // 
            this.selectedTestsToolStripMenuItem.Enabled = false;
            this.selectedTestsToolStripMenuItem.Name = "selectedTestsToolStripMenuItem";
            this.selectedTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.selectedTestsToolStripMenuItem.Text = "Selected Tests";
            // 
            // runTestsToolStripMenuItem
            // 
            this.runTestsToolStripMenuItem.Name = "runTestsToolStripMenuItem";
            this.runTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.runTestsToolStripMenuItem.Text = "Run tests";
            this.runTestsToolStripMenuItem.Click += new System.EventHandler(this.runTestsToolStripMenuItem_Click);
            // 
            // testSettingsToolStripMenuItem
            // 
            this.testSettingsToolStripMenuItem.Name = "testSettingsToolStripMenuItem";
            this.testSettingsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.testSettingsToolStripMenuItem.Text = "Test settings";
            this.testSettingsToolStripMenuItem.Click += new System.EventHandler(this.testSettingsToolStripMenuItem_Click);
            // 
            // vSExplorerTestsToolStripMenuItem
            // 
            this.vSExplorerTestsToolStripMenuItem.Name = "vSExplorerTestsToolStripMenuItem";
            this.vSExplorerTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.vSExplorerTestsToolStripMenuItem.Text = "VSExplorer tests";
            this.vSExplorerTestsToolStripMenuItem.Click += new System.EventHandler(this.vSExplorerTestsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem85
            // 
            this.toolStripMenuItem85.Name = "toolStripMenuItem85";
            this.toolStripMenuItem85.Size = new System.Drawing.Size(153, 6);
            // 
            // windowToolStripMenuItem1
            // 
            this.windowToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testExplorerToolStripMenuItem});
            this.windowToolStripMenuItem1.Name = "windowToolStripMenuItem1";
            this.windowToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
            this.windowToolStripMenuItem1.Text = "Windows";
            // 
            // testExplorerToolStripMenuItem
            // 
            this.testExplorerToolStripMenuItem.Name = "testExplorerToolStripMenuItem";
            this.testExplorerToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.testExplorerToolStripMenuItem.Text = "Test Explorer";
            this.testExplorerToolStripMenuItem.Click += new System.EventHandler(this.testExplorerToolStripMenuItem_Click);
            // 
            // lastRunTestsToolStripMenuItem
            // 
            this.lastRunTestsToolStripMenuItem.Enabled = false;
            this.lastRunTestsToolStripMenuItem.Name = "lastRunTestsToolStripMenuItem";
            this.lastRunTestsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.lastRunTestsToolStripMenuItem.Text = "Last Run Tests";
            // 
            // wINDOWToolStripMenuItem
            // 
            this.wINDOWToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeAllDocumentsToolStripMenuItem,
            this.toolStripSeparator21});
            this.wINDOWToolStripMenuItem.Name = "wINDOWToolStripMenuItem";
            this.wINDOWToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.wINDOWToolStripMenuItem.Text = "WINDOW";
            // 
            // closeAllDocumentsToolStripMenuItem
            // 
            this.closeAllDocumentsToolStripMenuItem.Image = global::WinExplorer.ve_resource.CloseDocumentGroup_16x;
            this.closeAllDocumentsToolStripMenuItem.Name = "closeAllDocumentsToolStripMenuItem";
            this.closeAllDocumentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.closeAllDocumentsToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.closeAllDocumentsToolStripMenuItem.Text = "Close All Documents";
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(221, 6);
            // 
            // hELPToolStripMenuItem
            // 
            this.hELPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpFilesToolStripMenuItem,
            this.toolStripMenuItem31,
            this.aboutApplicationStudioToolStripMenuItem,
            this.projectToolStripMenuItem1,
            this.guiToolStripMenuItem});
            this.hELPToolStripMenuItem.Name = "hELPToolStripMenuItem";
            this.hELPToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.hELPToolStripMenuItem.Text = "HELP";
            this.hELPToolStripMenuItem.Click += new System.EventHandler(this.hELPToolStripMenuItem_Click);
            // 
            // viewHelpFilesToolStripMenuItem
            // 
            this.viewHelpFilesToolStripMenuItem.Image = global::WinExplorer.ve_resource.StatusHelp_256x;
            this.viewHelpFilesToolStripMenuItem.Name = "viewHelpFilesToolStripMenuItem";
            this.viewHelpFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.viewHelpFilesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.viewHelpFilesToolStripMenuItem.Text = "View Help files";
            this.viewHelpFilesToolStripMenuItem.Click += new System.EventHandler(this.viewHelpFilesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem31
            // 
            this.toolStripMenuItem31.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewInWebBrowserToolStripMenuItem,
            this.viewInApplicationWindowToolStripMenuItem});
            this.toolStripMenuItem31.Name = "toolStripMenuItem31";
            this.toolStripMenuItem31.Size = new System.Drawing.Size(208, 22);
            this.toolStripMenuItem31.Text = "Set Help Preference";
            // 
            // viewInWebBrowserToolStripMenuItem
            // 
            this.viewInWebBrowserToolStripMenuItem.Name = "viewInWebBrowserToolStripMenuItem";
            this.viewInWebBrowserToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.viewInWebBrowserToolStripMenuItem.Text = "View in Web browser";
            // 
            // viewInApplicationWindowToolStripMenuItem
            // 
            this.viewInApplicationWindowToolStripMenuItem.Name = "viewInApplicationWindowToolStripMenuItem";
            this.viewInApplicationWindowToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.viewInApplicationWindowToolStripMenuItem.Text = "View in Application window";
            // 
            // aboutApplicationStudioToolStripMenuItem
            // 
            this.aboutApplicationStudioToolStripMenuItem.Name = "aboutApplicationStudioToolStripMenuItem";
            this.aboutApplicationStudioToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.aboutApplicationStudioToolStripMenuItem.Text = "About Application Studio";
            this.aboutApplicationStudioToolStripMenuItem.Click += new System.EventHandler(this.aboutApplicationStudioToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem1
            // 
            this.projectToolStripMenuItem1.Name = "projectToolStripMenuItem1";
            this.projectToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.projectToolStripMenuItem1.Text = "project";
            this.projectToolStripMenuItem1.Click += new System.EventHandler(this.projectToolStripMenuItem1_Click);
            // 
            // guiToolStripMenuItem
            // 
            this.guiToolStripMenuItem.Name = "guiToolStripMenuItem";
            this.guiToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.guiToolStripMenuItem.Text = "gui";
            this.guiToolStripMenuItem.Click += new System.EventHandler(this.guiToolStripMenuItem_Click);
            // 
            // toolStrip4
            // 
            this.toolStrip4.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip4.Location = new System.Drawing.Point(7, 0);
            this.toolStrip4.Name = "toolStrip4";
            this.toolStrip4.Size = new System.Drawing.Size(111, 25);
            this.toolStrip4.TabIndex = 4;
            this.toolStrip4.Text = "toolStrip4";
            this.toolStrip4.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip4_ItemClicked);
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(273, 3);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(333, 25);
            this.miniToolStrip.TabIndex = 1;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewInObjectBrowserToolStripMenuItem,
            this.removeToolStripMenuItem1});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(196, 48);
            this.contextMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip3_Opening);
            // 
            // viewInObjectBrowserToolStripMenuItem
            // 
            this.viewInObjectBrowserToolStripMenuItem.Name = "viewInObjectBrowserToolStripMenuItem";
            this.viewInObjectBrowserToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.viewInObjectBrowserToolStripMenuItem.Text = "View in Object Browser";
            // 
            // removeToolStripMenuItem1
            // 
            this.removeToolStripMenuItem1.Name = "removeToolStripMenuItem1";
            this.removeToolStripMenuItem1.Size = new System.Drawing.Size(195, 22);
            this.removeToolStripMenuItem1.Text = "Remove";
            this.removeToolStripMenuItem1.Click += new System.EventHandler(this.removeToolStripMenuItem1_Click);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewInFileExplorerToolStripMenuItem});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(179, 26);
            // 
            // viewInFileExplorerToolStripMenuItem
            // 
            this.viewInFileExplorerToolStripMenuItem.Name = "viewInFileExplorerToolStripMenuItem";
            this.viewInFileExplorerToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.viewInFileExplorerToolStripMenuItem.Text = "View in File Explorer";
            this.viewInFileExplorerToolStripMenuItem.Click += new System.EventHandler(this.viewInFileExplorerToolStripMenuItem_Click);
            // 
            // contextMenuStrip5
            // 
            this.contextMenuStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decompileTypeToolStripMenuItem,
            this.typeInfoToolStripMenuItem,
            this.addConnectedServiceToolStripMenuItem,
            this.addAnalyzerToolStripMenuItem1,
            this.managedNuGetPackagesToolStripMenuItem,
            this.toolStripMenuItem86,
            this.scopeToThisToolStripMenuItem});
            this.contextMenuStrip5.Name = "contextMenuStrip5";
            this.contextMenuStrip5.Size = new System.Drawing.Size(223, 142);
            // 
            // decompileTypeToolStripMenuItem
            // 
            this.decompileTypeToolStripMenuItem.Name = "decompileTypeToolStripMenuItem";
            this.decompileTypeToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.decompileTypeToolStripMenuItem.Text = "Add Reference";
            this.decompileTypeToolStripMenuItem.Click += new System.EventHandler(this.decompileTypeToolStripMenuItem_Click);
            // 
            // typeInfoToolStripMenuItem
            // 
            this.typeInfoToolStripMenuItem.Name = "typeInfoToolStripMenuItem";
            this.typeInfoToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.typeInfoToolStripMenuItem.Text = "Add Service Reference";
            this.typeInfoToolStripMenuItem.Click += new System.EventHandler(this.typeInfoToolStripMenuItem_Click);
            // 
            // addConnectedServiceToolStripMenuItem
            // 
            this.addConnectedServiceToolStripMenuItem.Name = "addConnectedServiceToolStripMenuItem";
            this.addConnectedServiceToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.addConnectedServiceToolStripMenuItem.Text = "Add Connected Service";
            // 
            // addAnalyzerToolStripMenuItem1
            // 
            this.addAnalyzerToolStripMenuItem1.Name = "addAnalyzerToolStripMenuItem1";
            this.addAnalyzerToolStripMenuItem1.Size = new System.Drawing.Size(222, 22);
            this.addAnalyzerToolStripMenuItem1.Text = "Add Analyzer";
            // 
            // managedNuGetPackagesToolStripMenuItem
            // 
            this.managedNuGetPackagesToolStripMenuItem.Image = global::WinExplorer.ve_resource.NugetPackage;
            this.managedNuGetPackagesToolStripMenuItem.Name = "managedNuGetPackagesToolStripMenuItem";
            this.managedNuGetPackagesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.managedNuGetPackagesToolStripMenuItem.Text = "Managed NuGet Packages...";
            this.managedNuGetPackagesToolStripMenuItem.Click += new System.EventHandler(this.managedNuGetPackagesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem86
            // 
            this.toolStripMenuItem86.Name = "toolStripMenuItem86";
            this.toolStripMenuItem86.Size = new System.Drawing.Size(219, 6);
            // 
            // scopeToThisToolStripMenuItem
            // 
            this.scopeToThisToolStripMenuItem.Name = "scopeToThisToolStripMenuItem";
            this.scopeToThisToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.scopeToThisToolStripMenuItem.Text = "Scope To This";
            // 
            // contextMenuStrip6
            // 
            this.contextMenuStrip6.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openWithToolStripMenuItem,
            this.viewSourceToolStripMenuItem,
            this.toolStripMenuItem23,
            this.toolStripMenuItem24,
            this.toolStripMenuItem27,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripMenuItem77,
            this.pasteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripMenuItem25,
            this.toolStripMenuItem26});
            this.contextMenuStrip6.Name = "contextMenuStrip6";
            this.contextMenuStrip6.Size = new System.Drawing.Size(184, 242);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openWithToolStripMenuItem
            // 
            this.openWithToolStripMenuItem.Name = "openWithToolStripMenuItem";
            this.openWithToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.openWithToolStripMenuItem.Text = "Open With";
            this.openWithToolStripMenuItem.Click += new System.EventHandler(this.openWithToolStripMenuItem_Click);
            // 
            // viewSourceToolStripMenuItem
            // 
            this.viewSourceToolStripMenuItem.Name = "viewSourceToolStripMenuItem";
            this.viewSourceToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.viewSourceToolStripMenuItem.Text = "View Source";
            // 
            // toolStripMenuItem23
            // 
            this.toolStripMenuItem23.Name = "toolStripMenuItem23";
            this.toolStripMenuItem23.Size = new System.Drawing.Size(180, 6);
            // 
            // toolStripMenuItem24
            // 
            this.toolStripMenuItem24.Name = "toolStripMenuItem24";
            this.toolStripMenuItem24.Size = new System.Drawing.Size(183, 22);
            this.toolStripMenuItem24.Text = "Exclude from Project";
            // 
            // toolStripMenuItem27
            // 
            this.toolStripMenuItem27.Name = "toolStripMenuItem27";
            this.toolStripMenuItem27.Size = new System.Drawing.Size(180, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem77
            // 
            this.toolStripMenuItem77.Name = "toolStripMenuItem77";
            this.toolStripMenuItem77.Size = new System.Drawing.Size(183, 22);
            this.toolStripMenuItem77.Text = "Paste";
            this.toolStripMenuItem77.Click += new System.EventHandler(this.toolStripMenuItem77_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.pasteToolStripMenuItem.Text = "Delete";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // toolStripMenuItem25
            // 
            this.toolStripMenuItem25.Name = "toolStripMenuItem25";
            this.toolStripMenuItem25.Size = new System.Drawing.Size(180, 6);
            // 
            // toolStripMenuItem26
            // 
            this.toolStripMenuItem26.Name = "toolStripMenuItem26";
            this.toolStripMenuItem26.Size = new System.Drawing.Size(183, 22);
            this.toolStripMenuItem26.Text = "Properties";
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = "F:\\Application-Studio\\MSBuildProjects2\\QuickSharp.WinFormExplorer\\bin\\Debug\\CHM\\i" +
    "ndex.chm";
            // 
            // contextMenuStrip7
            // 
            this.contextMenuStrip7.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem32,
            this.toolStripMenuItem33,
            this.toolStripMenuItem34,
            this.toolStripSeparator6,
            this.toolStripMenuItem35,
            this.toolStripSeparator7,
            this.toolStripMenuItem36,
            this.toolStripMenuItem37,
            this.toolStripMenuItem38,
            this.toolStripMenuItem43,
            this.toolStripMenuItem39,
            this.toolStripMenuItem42,
            this.toolStripMenuItem41,
            this.toolStripSeparator8,
            this.toolStripMenuItem40});
            this.contextMenuStrip7.Name = "contextMenuStrip6";
            this.contextMenuStrip7.Size = new System.Drawing.Size(219, 270);
            // 
            // toolStripMenuItem32
            // 
            this.toolStripMenuItem32.Name = "toolStripMenuItem32";
            this.toolStripMenuItem32.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem32.Text = "Open";
            // 
            // toolStripMenuItem33
            // 
            this.toolStripMenuItem33.Name = "toolStripMenuItem33";
            this.toolStripMenuItem33.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem33.Text = "Open With";
            // 
            // toolStripMenuItem34
            // 
            this.toolStripMenuItem34.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newItemToolStripMenuItem2,
            this.exisitngItemToolStripMenuItem,
            this.newFolderToolStripMenuItem2,
            this.toolStripMenuItem44,
            this.windowsFormToolStripMenuItem,
            this.userControlToolStripMenuItem,
            this.toolStripMenuItem45,
            this.componentToolStripMenuItem,
            this.classToolStripMenuItem});
            this.toolStripMenuItem34.Name = "toolStripMenuItem34";
            this.toolStripMenuItem34.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem34.Text = "Add";
            this.toolStripMenuItem34.Click += new System.EventHandler(this.toolStripMenuItem34_Click);
            // 
            // newItemToolStripMenuItem2
            // 
            this.newItemToolStripMenuItem2.Name = "newItemToolStripMenuItem2";
            this.newItemToolStripMenuItem2.Size = new System.Drawing.Size(154, 22);
            this.newItemToolStripMenuItem2.Text = "New Item";
            this.newItemToolStripMenuItem2.Click += new System.EventHandler(this.newItemToolStripMenuItem2_Click);
            // 
            // exisitngItemToolStripMenuItem
            // 
            this.exisitngItemToolStripMenuItem.Name = "exisitngItemToolStripMenuItem";
            this.exisitngItemToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.exisitngItemToolStripMenuItem.Text = "Exisitng Item";
            // 
            // newFolderToolStripMenuItem2
            // 
            this.newFolderToolStripMenuItem2.Name = "newFolderToolStripMenuItem2";
            this.newFolderToolStripMenuItem2.Size = new System.Drawing.Size(154, 22);
            this.newFolderToolStripMenuItem2.Text = "New Folder";
            this.newFolderToolStripMenuItem2.Click += new System.EventHandler(this.newFolderToolStripMenuItem2_Click);
            // 
            // toolStripMenuItem44
            // 
            this.toolStripMenuItem44.Name = "toolStripMenuItem44";
            this.toolStripMenuItem44.Size = new System.Drawing.Size(151, 6);
            // 
            // windowsFormToolStripMenuItem
            // 
            this.windowsFormToolStripMenuItem.Name = "windowsFormToolStripMenuItem";
            this.windowsFormToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.windowsFormToolStripMenuItem.Text = "Windows Form";
            this.windowsFormToolStripMenuItem.Click += new System.EventHandler(this.windowsFormToolStripMenuItem_Click);
            // 
            // userControlToolStripMenuItem
            // 
            this.userControlToolStripMenuItem.Name = "userControlToolStripMenuItem";
            this.userControlToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.userControlToolStripMenuItem.Text = "User Control";
            // 
            // toolStripMenuItem45
            // 
            this.toolStripMenuItem45.Name = "toolStripMenuItem45";
            this.toolStripMenuItem45.Size = new System.Drawing.Size(151, 6);
            // 
            // componentToolStripMenuItem
            // 
            this.componentToolStripMenuItem.Name = "componentToolStripMenuItem";
            this.componentToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.componentToolStripMenuItem.Text = "Component";
            // 
            // classToolStripMenuItem
            // 
            this.classToolStripMenuItem.Name = "classToolStripMenuItem";
            this.classToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.classToolStripMenuItem.Text = "Class";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem35
            // 
            this.toolStripMenuItem35.Name = "toolStripMenuItem35";
            this.toolStripMenuItem35.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem35.Text = "Exclude from Project";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem36
            // 
            this.toolStripMenuItem36.Name = "toolStripMenuItem36";
            this.toolStripMenuItem36.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem36.Text = "Cut";
            // 
            // toolStripMenuItem37
            // 
            this.toolStripMenuItem37.Name = "toolStripMenuItem37";
            this.toolStripMenuItem37.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem37.Text = "Copy";
            this.toolStripMenuItem37.Click += new System.EventHandler(this.toolStripMenuItem37_Click);
            // 
            // toolStripMenuItem38
            // 
            this.toolStripMenuItem38.Name = "toolStripMenuItem38";
            this.toolStripMenuItem38.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem38.Text = "Delete";
            this.toolStripMenuItem38.Click += new System.EventHandler(this.toolStripMenuItem38_Click);
            // 
            // toolStripMenuItem43
            // 
            this.toolStripMenuItem43.Name = "toolStripMenuItem43";
            this.toolStripMenuItem43.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem43.Text = "Paste";
            this.toolStripMenuItem43.Click += new System.EventHandler(this.toolStripMenuItem43_Click);
            // 
            // toolStripMenuItem39
            // 
            this.toolStripMenuItem39.Name = "toolStripMenuItem39";
            this.toolStripMenuItem39.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem39.Text = "Rename";
            this.toolStripMenuItem39.Click += new System.EventHandler(this.toolStripMenuItem39_Click);
            // 
            // toolStripMenuItem42
            // 
            this.toolStripMenuItem42.Name = "toolStripMenuItem42";
            this.toolStripMenuItem42.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem41
            // 
            this.toolStripMenuItem41.Name = "toolStripMenuItem41";
            this.toolStripMenuItem41.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem41.Text = "Open Folder in File Explorer";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(215, 6);
            // 
            // toolStripMenuItem40
            // 
            this.toolStripMenuItem40.Name = "toolStripMenuItem40";
            this.toolStripMenuItem40.Size = new System.Drawing.Size(218, 22);
            this.toolStripMenuItem40.Text = "Properties";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.panel1);
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1049, 535);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1049, 560);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 514);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1049, 21);
            this.panel1.TabIndex = 3;
            // 
            // ExplorerForms
            // 
            this.ClientSize = new System.Drawing.Size(1049, 584);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.helpProvider1.SetHelpKeyword(this, "Explorer");
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ExplorerForms";
            this.helpProvider1.SetShowHelp(this, true);
            this.Text = "Application Studio";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExplorerForm_FormClosing);
            this.Load += new System.EventHandler(this.ExplorerForms_Load);
            this.ResizeBegin += new System.EventHandler(this.ExplorerForms_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.ExplorerForms_ResizeEnd);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip3.ResumeLayout(false);
            this.contextMenuStrip4.ResumeLayout(false);
            this.contextMenuStrip5.ResumeLayout(false);
            this.contextMenuStrip6.ResumeLayout(false);
            this.contextMenuStrip7.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OpenCMDToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            openCMD();
        }

        #endregion

        private SplitContainer splitContainer1;
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;
        private ToolStripMenuItem setAsMainProjectToolStripMenuItem;
        private SplitContainer splitContainer2;
        private ToolStripMenuItem projectPropertiesToolStripMenuItem;
        private ToolStripMenuItem addReferenceToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem newItemToolStripMenuItem;
        private ToolStripMenuItem existingItemToolStripMenuItem;
        private ToolStripMenuItem referenceToolStripMenuItem;
        private ToolStripMenuItem formToolStripMenuItem;
        private ToolStripMenuItem controlToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem newProjectToolStripMenuItem;
        private ToolStripMenuItem existingProjectToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        //private System.Windows.Forms.MenuSeparator menuSeparator1;
        private ToolStripMenuItem unloadProjectToolStripMenuItem;
        private ToolStripMenuItem viewSolutionFileToolStripMenuItem;
        private ToolStripMenuItem viewProjectFileToolStripMenuItem;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStrip toolStrip4;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton11;
        private TreeView _mainTreeView;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage5;
        private Button button2;
        private TextBox textBox2;
        private TextBox textBox1;
        private RichTextBox richTextBox1;
        private Button button1;
        private ListBox listBox1;
        private ToolStrip toolStrip2;
        private ToolStripButton toolStripButton3;
        private TabPage tabPage6;
        private TextBox textBox3;
        private ProgressBar progressBar1;
        private ToolStrip toolStrip3;
        private ToolStripButton toolStripButton5;
        private ToolStripButton toolStripButton6;
        private TreeView treeView3;
        private ToolStrip miniToolStrip;
        private ToolStripMenuItem newFolderToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip3;
        private ToolStripMenuItem viewInObjectBrowserToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem1;
        private ContextMenuStrip contextMenuStrip4;
        private ToolStripMenuItem viewInFileExplorerToolStripMenuItem;
        private ToolStripMenuItem openFolderInFileExplorerToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem tOOLSToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem exportTemplateToolStripMenuItem;
        private SplitContainer splitContainer3;
        private ToolStrip toolStrip5;
        private TextBox textBox4;
        private ToolStripButton toolStripButton13;
        private ToolStripButton toolStripButton14;
        private TreeView treeView4;
        private ToolStripMenuItem pROJECTToolStripMenuItem;
        private ToolStripMenuItem bUILDToolStripMenuItem;
        private ToolStripMenuItem wINDOWToolStripMenuItem;
        private ToolStripMenuItem hELPToolStripMenuItem;
        private ToolStripMenuItem runToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem1;
        private ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem commentSectionToolStripMenuItem;
        private ToolStripMenuItem uncommentSectionToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem buildSolutionToolStripMenuItem;
        private ToolStripMenuItem rebuildSolutionToolStripMenuItem;
        private ToolStripMenuItem cleanSolutionToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem10;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem addWindowsFormToolStripMenuItem;
        private ToolStripMenuItem addUserControlToolStripMenuItem;
        private ToolStripMenuItem solutionExplorerToolStripMenuItem;
        private ToolStripMenuItem recentProjectsToolStripMenuItem;
        private ToolStripMenuItem fileExplorerToolStripMenuItem;
        private ToolStripMenuItem outputWindowToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem11;
        private ToolStripMenuItem recentProjectsAndSolutionsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip5;
        private ToolStripMenuItem decompileTypeToolStripMenuItem;
        private ToolStripMenuItem typeInfoToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem toolStripMenuItem14;
        private ToolStripMenuItem classViewToolStripMenuItem;
        private ToolStripMenuItem propertyGridToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem projectSolutionToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem17;
        private ToolStripMenuItem toolStripMenuItem18;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem19;
        private ToolStripMenuItem newProjectToolStripMenuItem1;
        private ToolStripMenuItem existingProjetToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem newItemToolStripMenuItem1;
        private ToolStripMenuItem existingItemToolStripMenuItem1;
        private ToolStripMenuItem newFolderToolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem20;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItem22;
        private ToolStripMenuItem toolStripMenuItem21;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripSeparator toolStripSeparator5;
        private ContextMenuStrip contextMenuStrip6;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem openWithToolStripMenuItem;
        private ToolStripMenuItem viewSourceToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem23;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem24;
        private ToolStripSeparator toolStripMenuItem27;
        private ToolStripSeparator toolStripMenuItem25;
        private ToolStripMenuItem toolStripMenuItem26;
        private ToolStripMenuItem toolStripMenuItem28;
        private ToolStripSeparator toolStripMenuItem29;
        private ToolStripMenuItem toolStripMenuItem30;
        private ToolStripMenuItem aboutApplicationStudioToolStripMenuItem;
        private ToolStripMenuItem viewHelpFilesToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem31;
        private ToolStripMenuItem viewInWebBrowserToolStripMenuItem;
        private ToolStripMenuItem viewInApplicationWindowToolStripMenuItem;
        private HelpProvider helpProvider1;
        private ContextMenuStrip contextMenuStrip7;
        private ToolStripMenuItem toolStripMenuItem32;
        private ToolStripMenuItem toolStripMenuItem33;
        private ToolStripMenuItem toolStripMenuItem34;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem toolStripMenuItem35;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem toolStripMenuItem36;
        private ToolStripMenuItem toolStripMenuItem37;
        private ToolStripMenuItem toolStripMenuItem38;
        private ToolStripMenuItem toolStripMenuItem43;
        private ToolStripMenuItem toolStripMenuItem39;
        private ToolStripSeparator toolStripMenuItem42;
        private ToolStripMenuItem toolStripMenuItem41;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem toolStripMenuItem40;
        private ToolStripMenuItem newItemToolStripMenuItem2;
        private ToolStripMenuItem exisitngItemToolStripMenuItem;
        private ToolStripMenuItem newFolderToolStripMenuItem2;
        private ToolStripSeparator toolStripMenuItem44;
        private ToolStripMenuItem windowsFormToolStripMenuItem;
        private ToolStripMenuItem userControlToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem45;
        private ToolStripMenuItem componentToolStripMenuItem;
        private ToolStripMenuItem classToolStripMenuItem;
        private ToolStripMenuItem tESTToolStripMenuItem;
        private ToolStripMenuItem runTestsToolStripMenuItem;
        private ToolStripMenuItem testSettingsToolStripMenuItem;
        private ToolStripMenuItem addComponentToolStripMenuItem;
        private ToolStripMenuItem addNewClassToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem46;
        private ToolStripMenuItem addNewItemToolStripMenuItem;
        private ToolStripMenuItem addExistingItemToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem47;
        private ToolStripMenuItem addReferenceToolStripMenuItem1;
        private ToolStripMenuItem addProjectReferenceToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem48;
        private TreeViews treeView2;
        private /*TreeViews*/  UI.TreeViewEx treeView1;
        private AIMS.Libraries.Scripting.ScriptControl.ScriptControl scriptControl1;
        private ToolStripMenuItem outputWindowToolStripMenuItem1;
        private ToolStripMenuItem errorListToolStripMenuItem;
        private ToolStripMenuItem findResultsToolStripMenuItem;
        private ToolStripMenuItem projectToolStripMenuItem1;
        private ToolStripMenuItem mSBuildProjectToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem49;
        private ToolStripMenuItem toolStripMenuItem51;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem toolStripMenuItem50;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem toolStripMenuItem53;
        private ToolStripMenuItem toolStripMenuItem52;
        private ToolStripMenuItem findResults1ToolStripMenuItem;
        private ToolStripMenuItem findResults2ToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem55;
        private ToolStripMenuItem toolStripMenuItem54;
        private ToolStripMenuItem analyzeSourceCodeToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem57;
        private ToolStripMenuItem toolStripMenuItem58;
        private ToolStripMenuItem toolStripMenuItem59;
        private ToolStripMenuItem toolStripMenuItem56;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem codeAnalysisToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem toolStripMenuItem60;
        private ToolStripMenuItem toolStripMenuItem61;
        private ToolStripMenuItem toolStripMenuItem62;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem toolStripMenuItem64;
        private ToolStripMenuItem toolStripMenuItem65;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem toolStripMenuItem66;
        private ToolStripMenuItem newProjectToolStripMenuItem2;
        private ToolStripMenuItem newWebSiteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItem existingProjectToolStripMenuItem1;
        private ToolStripMenuItem existingWebSiteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripMenuItem runCodeAnalysisOnSolutionToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem buildProjectToolStripMenuItem;
        private ToolStripMenuItem rebuildProjectToolStripMenuItem;
        private ToolStripMenuItem cleanProjectToolStripMenuItem;
        private ToolStripMenuItem publishProjectToolStripMenuItem;
        private ToolStripMenuItem runCodeAnalysisOnProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripMenuItem configurationManagerToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem67;
        private ToolStripMenuItem toolStripMenuItem68;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem addAnalyzerToolStripMenuItem;
        private ToolStripMenuItem setAsStartupProjectToolStripMenuItem;
        private ToolStripMenuItem projectDependenciesToolStripMenuItem;
        private ToolStripMenuItem projectBuildOrderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem projectPropertiesToolStripMenuItem1;
        private ToolStripMenuItem closeAllDocumentsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripMenuItem toolStripMenuItem70;
        private ToolStripMenuItem toolStripMenuItem69;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem toolStripMenuItem72;
        private ToolStripMenuItem toolStripMenuItem73;
        private ToolStripMenuItem toolStripMenuItem74;
        private ToolStripMenuItem toolStripMenuItem71;
        private ToolStripMenuItem toolStripMenuItem75;
        private ToolStripSeparator toolStripSeparator24;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem bookmarksToolStripMenuItem;
        private ToolStripMenuItem outlineWindowToolStripMenuItem;
        private ToolStripMenuItem custToolStripMenuItem;
        private ToolStripMenuItem importAndExportSettingsToolStripMenuItem;
        private ToolStripMenuItem externalToolsToolStripMenuItem;
        private ToolStripMenuItem commandWindowToolStripMenuItem;
        private ToolStripMenuItem projectToolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator25;
        private ToolStripMenuItem toolStripMenuItem76;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem77;
        private ToolStripMenuItem vSExplorerTestsToolStripMenuItem;
        private ToolStripMenuItem nUnitToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator26;
        private ToolStripMenuItem toolStripMenuItem78;
        private ToolStripButton toolStripButton2;
        private ToolStripMenuItem openCMDToolStripMenuItem;
        private ToolStripMenuItem restoreNugetPackagesToolStripMenuItem;
        private ToolStripMenuItem powerShellToolStripMenuItem;
        private ToolStripMenuItem powerShellWindowToolStripMenuItem;
        private ToolStripMenuItem guiToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator27;
        private ToolStripMenuItem toolStripMenuItem79;
        private ToolStripMenuItem standardToolStripMenuItem;
        private ToolStripMenuItem buildToolStripMenuItem1;
        private ToolStripMenuItem debugToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator28;
        private ToolStripMenuItem customizeToolStripMenuItem;
        private ToolStripContainer toolStripContainer1;
        private ToolStripMenuItem toolStripMenuItem80;
        private ToolStripMenuItem windowsToolStripMenuItem;
        private ToolStripMenuItem breakpointsToolStripMenuItem;
        private ToolStripMenuItem graphicsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator29;
        private ToolStripMenuItem startDebuggingToolStripMenuItem;
        private ToolStripMenuItem startWithoutDebuggingToolStripMenuItem;
        private ToolStripMenuItem exceptionSettingsToolStripMenuItem;
        private ToolStripMenuItem outputToolStripMenuItem;
        private ToolStripMenuItem showDiagnosticToolsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator30;
        private ToolStripMenuItem immediateToolStripMenuItem;
        private ToolStripMenuItem performanceProfilerToolStripMenuItem;
        private ToolStripMenuItem attachToProcessToolStripMenuItem;
        private ToolStripMenuItem profilerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator31;
        private ToolStripMenuItem stepInToolStripMenuItem;
        private ToolStripMenuItem stepOutToolStripMenuItem;
        private ToolStripMenuItem watchToolStripMenuItem;
        private ToolStripMenuItem dataSourcesToolStripMenuItem;
        private ToolStripMenuItem webBrowserToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem81;
        private ToolStripMenuItem toolStripMenuItem83;
        private ToolStripMenuItem toolStripMenuItem82;
        private ToolStripMenuItem xmlDocumentOutlineToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem84;
        private ToolStripMenuItem addConnectedServiceToolStripMenuItem;
        private ToolStripMenuItem addAnalyzerToolStripMenuItem1;
        private ToolStripMenuItem managedNuGetPackagesToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem86;
        private ToolStripMenuItem scopeToThisToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem85;
        private ToolStripMenuItem windowToolStripMenuItem1;
        private ToolStripMenuItem testExplorerToolStripMenuItem;
        private ToolStripMenuItem selectedTestsToolStripMenuItem;
        private ToolStripMenuItem lastRunTestsToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem87;
        private ToolStripMenuItem repositoryToolStripMenuItem;
        private ToolStripMenuItem websiteToolStripMenuItem;
        private ToolStripMenuItem projectFromSourcesToolStripMenuItem;
        private ToolStripMenuItem syntaxTreeVisualizatorToolStripMenuItem;
        private Panel panel1;
    }
}