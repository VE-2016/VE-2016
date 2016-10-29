using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AIMS.Libraries.Forms.Docking;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class ProjectExplorer : DockableWindow
    {
        private TreeNode _rootNode = null;
        private TreeNode _referencesNode = null;
        public event EventHandler<ExplorerClickEventArgs> FileClick;
        public event EventHandler<ExplorerLabelEditEventArgs> FileNameChanged;
        public event EventHandler NewItemAdd;
        public event EventHandler FileItemDeleted;
        public event EventHandler AddRefrenceItem;
        public event EventHandler AddWebRefrenceItem;

        public ProjectExplorer()
        {
            InitializeComponent();
            InitilizeTree();
        }

        protected virtual void OnFileClick(ExplorerClickEventArgs e)
        {
            if (FileClick != null)
            {
                FileClick(this, e);
            }
        }

        protected virtual void OnNewItemAdd()
        {
            if (NewItemAdd != null)
            {
                NewItemAdd(this, null);
            }
        }

        protected virtual void OnAddRefrenceItem(TreeNode node)
        {
            if (AddRefrenceItem != null)
            {
                AddRefrenceItem(node, null);
            }
        }

        protected virtual void OnAddWebRefrenceItem(TreeNode node)
        {
            if (AddWebRefrenceItem != null)
            {
                AddWebRefrenceItem(node, null);
            }
        }

        protected virtual void OnFileItemDeleted(TreeNode node)
        {
            if (FileItemDeleted != null)
            {
                TreeNode parent = node.Parent;
                FileItemDeleted(node, null);
                DeleteWebReferenceFolder(parent);
            }
        }

        private void DeleteWebReferenceFolder(TreeNode Item)
        {
            if (Item.Nodes.Count == 0)
            {
                Item.Remove();
                UpdateLanguageUI(_rootNode);
            }
        }

        protected virtual void OnFileNameChanged(ExplorerLabelEditEventArgs e)
        {
            if (FileNameChanged != null)
            {
                FileNameChanged(this, e);
            }
        }

        public TreeNode RefrenceNode
        {
            get { return _referencesNode; }
        }
        private void InitilizeTree()
        {
            tvwSolutionExplorer.Nodes.Clear();
            _rootNode = tvwSolutionExplorer.Nodes.Add("Project");
            _rootNode.Expand();
            _rootNode.Tag = NodeType.Project;
            _referencesNode = _rootNode.Nodes.Add("References");
            _referencesNode.Tag = NodeType.References;
            UpdateLanguageUI(_rootNode);
            tvwSolutionExplorer.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(tvwSolutionExplorer_NodeDblClick);
            tvwSolutionExplorer.KeyUp += new KeyEventHandler(tvwSolutionExplorer_KeyUp);
            tvwSolutionExplorer.AfterLabelEdit += new NodeLabelEditEventHandler(tvwSolutionExplorer_AfterLabelEdit);
        }

        private bool ValidateFolderName(TreeNode startNode, string NewName)
        {
            foreach (TreeNode t in startNode.Nodes)
            {
                if (t.Text.ToLower() == NewName.ToLower() && (NodeType)t.Tag == NodeType.Folder)
                {
                    return false;
                }
                else
                    ValidateFolderName(t, NewName);
            }
            return true;
        }

        private void tvwSolutionExplorer_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            tvwSolutionExplorer.LabelEdit = false;
            if (e.Label == null)
            {
                e.CancelEdit = true;
                return;
            }

            TreeNode node = e.Node;
            NodeType t = (NodeType)node.Tag;
            if (t == NodeType.Folder)
            {
                e.CancelEdit = true;
                TreeNode parentNode = node.Parent;
                if (ValidateFolderName(parentNode, e.Label))
                {
                    e.Node.Text = e.Label;
                }
                else
                {
                    MessageBox.Show("Folder Name '" + e.Label + "' already exists in current hirerachy.Please select another name.");
                }
                return;
            }

            if (Path.GetFileNameWithoutExtension(e.Label).Length == 0)
            {
                e.CancelEdit = true;
                return;
            }
            string newName = Path.GetFileNameWithoutExtension(e.Label);
            string oldExtension = Path.GetExtension(e.Node.Text);

            string validName = newName + oldExtension;

            ExplorerLabelEditEventArgs evnt = new ExplorerLabelEditEventArgs(validName, e.Node.Text);
            OnFileNameChanged(evnt);
            e.CancelEdit = true;
            if (evnt.Cancel == false)
                e.Node.Text = validName;
        }

        public void StartLabelEdit(TreeNode node)
        {
            NodeType t = (NodeType)node.Tag;
            if (t == NodeType.File || t == NodeType.Folder)
            {
                tvwSolutionExplorer.LabelEdit = true;
                node.BeginEdit();
            }
        }
        private void tvwSolutionExplorer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                TreeNode node = tvwSolutionExplorer.SelectedNode;
                if (node != null)
                {
                    StartLabelEdit(node);
                }
            }
        }

        private void tvwSolutionExplorer_NodeDblClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            NodeType t = (NodeType)e.Node.Tag;
            if (t == NodeType.File)
                OnFileClick(new ExplorerClickEventArgs(e.Node.Text));
            UpdateImage(e.Node);
        }

        private ScriptLanguage _scriptLanguage = ScriptLanguage.CSharp;
        public ScriptLanguage Language
        {
            get { return _scriptLanguage; }
            set
            {
                _scriptLanguage = value;
                UpdateLanguageUI(_rootNode);
            }
        }

        public void AddWebReference(string fileName)
        {
            TreeNode parent = GetWebReferenceNode();
            if (parent.IsExpanded == false) parent.Expand();
            TreeNode node = parent.Nodes.Add(fileName, fileName);
            node.Tag = NodeType.File;
            UpdateImage(node);
        }

        public TreeNode GetWebReferenceNode()
        {
            TreeNode refNode = null;
            foreach (TreeNode node in _referencesNode.Nodes)
            {
                NodeType t = (NodeType)node.Tag;
                if (t == NodeType.WebRererenceFolder)
                {
                    refNode = node;
                    break;
                }
            }
            if (refNode == null)
            {
                refNode = _referencesNode.Nodes.Add("Web Reference");
                refNode.Tag = NodeType.WebRererenceFolder;
            }
            if (!refNode.IsExpanded) refNode.Expand();
            UpdateImage(refNode);
            return refNode;
        }

        public void AddFile(string fileName)
        {
            TreeNode selfolder = tvwSolutionExplorer.SelectedNode;
            NodeType t = NodeType.Project;
            if (selfolder != null)
                t = (NodeType)selfolder.Tag;
            if (t != NodeType.Folder) selfolder = null;

            TreeNode parent = (selfolder == null ? _rootNode : selfolder);
            if (parent.IsExpanded == false) parent.Expand();
            TreeNode node = parent.Nodes.Add(fileName, fileName);
            node.Tag = NodeType.File;
            UpdateImage(node);
        }

        public void ActiveNode(string fileName)
        {
            tvwSolutionExplorer.SelectedNode = null;
        }

        public TreeNode AddFolder(string folderName)
        {
            TreeNode selfolder = tvwSolutionExplorer.SelectedNode;
            NodeType t = (NodeType)selfolder.Tag;
            if (t != NodeType.Folder) selfolder = null;
            TreeNode parent = (selfolder == null ? _rootNode : selfolder);
            TreeNode node = parent.Nodes.Add(folderName);
            if (parent.IsExpanded == false) parent.Expand();
            node.Tag = NodeType.Folder;
            UpdateImage(node);
            return node;
        }

        public void AddRefrence(string referenceName)
        {
            TreeNode node = _referencesNode.Nodes.Add(referenceName);
            node.Tag = NodeType.Reference;
            UpdateImage(node);
        }


        private void UpdateImage(TreeNode node)
        {
            NodeType t = (NodeType)node.Tag;
            switch (t)
            {
                case NodeType.Project:
                    node.ImageKey = (_scriptLanguage == ScriptLanguage.CSharp ? "CSharpProject.ico" : "VbProject.ico");
                    break;
                case NodeType.References:
                case NodeType.WebRererenceFolder:
                    node.ImageKey = (node.IsExpanded ? "ReferencesOpen.ico" : "ReferencesClosed.ico");
                    break;
                case NodeType.Folder:
                    node.ImageKey = (node.IsExpanded ? "OpenFolder.ico" : "ClosedFolder.ico");
                    break;
                case NodeType.File:
                    node.ImageKey = (_scriptLanguage == ScriptLanguage.CSharp ? "CSharpFile.ico" : "VbFile.ico");
                    node.Text = Path.GetFileNameWithoutExtension(node.Text) + (_scriptLanguage == ScriptLanguage.CSharp ? ".cs" : ".vb");
                    break;
                case NodeType.Reference:
                    node.ImageKey = "Reference.ico";
                    break;
            }
            node.SelectedImageKey = node.ImageKey;
        }
        private void UpdateLanguageUI(TreeNode StartNode)
        {
            UpdateImage(StartNode);
            foreach (TreeNode node in StartNode.Nodes)
            {
                UpdateLanguageUI(node);    //Rec..
            }
        }

        private void tvwSolutionExplorer_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            tvwSolutionExplorer.SelectedNode = node;

            ContextMenuStrip c = null;
            if (e.Button == MouseButtons.Right && node != null)
            {
                NodeType t = (NodeType)node.Tag;
                switch (t)
                {
                    case NodeType.Project:
                        tProjectAdd.Visible = true;
                        tProjectBuild.Visible = true;
                        tProjectAdd.Visible = true;
                        tProjectClean.Visible = true;
                        tProjectProp.Visible = true;
                        tProjectSep1.Visible = true;
                        tProjectSep2.Visible = true;
                        tProjectSep3.Visible = true;
                        c = cMenuProject;
                        break;
                    case NodeType.References:
                        tProjectAdd.Visible = false;
                        tProjectBuild.Visible = false;
                        tProjectAdd.Visible = false;
                        tProjectClean.Visible = false;
                        tProjectProp.Visible = false;
                        tProjectSep1.Visible = false;
                        tProjectSep2.Visible = false;
                        tProjectSep3.Visible = false;
                        c = cMenuProject;
                        break;
                    case NodeType.Folder:
                        c = cMenuFolder;
                        break;
                    case NodeType.File:
                        tFileCopy.Visible = true;
                        tFileCut.Visible = true;
                        tFileOpen.Visible = true;
                        tFilePaste.Visible = true;
                        tFileRename.Visible = true;
                        tFileSep1.Visible = true;
                        tFileSep2.Visible = true;
                        c = cMenuFile;
                        break;
                    case NodeType.Reference:
                        tFileCopy.Visible = false;
                        tFileCut.Visible = false;
                        tFileOpen.Visible = false;
                        tFilePaste.Visible = false;
                        tFileRename.Visible = false;
                        tFileSep1.Visible = false;
                        tFileSep2.Visible = false;
                        c = cMenuFile;
                        break;
                }

                if (c != null)
                {
                    c.Show(tvwSolutionExplorer, e.Location);
                }
            }
            UpdateImage(node);
        }

        private void AddNewItem(object sender, EventArgs e)
        {
            OnNewItemAdd();
        }

        private void AddExistingItem(object sender, EventArgs e)
        {
        }

        private void AddNewFolder(object sender, EventArgs e)
        {
            TreeNode node = AddFolder("New Folder");
            StartLabelEdit(node);
        }

        private void RenameItem(object sender, EventArgs e)
        {
            TreeNode node = tvwSolutionExplorer.SelectedNode;
            if (node != null)
            {
                StartLabelEdit(node);
            }
        }

        private void DeleteFolder(TreeNode root)
        {
            foreach (TreeNode node in root.Nodes)
            {
                if ((NodeType)node.Tag == NodeType.File)
                {
                    OnFileItemDeleted(node);
                }
                else if ((NodeType)node.Tag == NodeType.Folder)
                {
                    DeleteFolder(node);
                    node.Remove();
                }
            }
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            TreeNode node = tvwSolutionExplorer.SelectedNode;
            if (node != null)
            {
                switch ((NodeType)node.Tag)
                {
                    case NodeType.File:
                        OnFileItemDeleted(node);
                        break;
                    case NodeType.Folder:
                        DeleteFolder(node);
                        node.Remove();
                        break;
                    case NodeType.Reference:
                        break;
                    case NodeType.Project:
                    case NodeType.References:
                        break;
                }
            }
        }

        private void OpenItem(object sender, EventArgs e)
        {
            TreeNode node = tvwSolutionExplorer.SelectedNode;
            NodeType t = (NodeType)node.Tag;
            if (t == NodeType.File)
                OnFileClick(new ExplorerClickEventArgs(node.Text));
        }

        private void CutItem(object sender, EventArgs e)
        {
        }

        private void CopyItem(object sender, EventArgs e)
        {
        }

        private void PasteItem(object sender, EventArgs e)
        {
        }

        private void BuildProject(object sender, EventArgs e)
        {
        }

        private void CleanProject(object sender, EventArgs e)
        {
        }

        private void AddRefrence(object sender, EventArgs e)
        {
            OnAddRefrenceItem(_referencesNode);
        }

        private void ProjectProperties(object sender, EventArgs e)
        {
        }

        private void AddWebReference(object sender, EventArgs e)
        {
            OnAddWebRefrenceItem(_referencesNode);
        }
    }
    public class ExplorerClickEventArgs : EventArgs
    {
        public string FileName = "";
        public ExplorerClickEventArgs(string fileName)
        {
            FileName = fileName;
        }
    }

    public class ExplorerLabelEditEventArgs : EventArgs
    {
        public string OldName = "";
        public string NewName = "";
        public bool Cancel = false;
        public ExplorerLabelEditEventArgs(string newName, string oldName)
        {
            OldName = oldName;
            NewName = newName;
            Cancel = false;
        }
    }

    public enum NodeType
    {
        Project = 1,
        File,
        Folder,
        References,
        Reference,
        WebRererenceFolder
    }
}