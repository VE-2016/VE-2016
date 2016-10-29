using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer
{
    public class ToolStripCreator
    {
              

        private ContextMenuStrip _treeViewMenu;
        private ToolStripMenuItem UI_TREE_MENU_OPEN;
        private ToolStripSeparator UI_TREE_MENU_SEP_1;
        private ToolStripMenuItem UI_TREE_MENU_RENAME;
        private ToolStripMenuItem UI_TREE_MENU_CLONE;
        private ToolStripMenuItem UI_TREE_MENU_DELETE;
        private ToolStripSeparator UI_TREE_MENU_SEP_2;
        private ToolStripMenuItem UI_TREE_MENU_CREATE_FOLDER;


        private ToolStripSeparator UI_TREE_MENU_SEP_3;
        private ToolStripMenuItem UI_TREE_MENU_SET_AS_CURRENT_DIR;

     
        public TreeView _mainTreeView { get; set; }

        //public TreeViewMenu _treeViewMenu { get; set; }
        
            const string UI_TREE_MENU_LOAD_PROJECT_NAME = "Project";

            public void doinit()
            {

                UI_TREE_MENU_OPEN = new ToolStripMenuItem();
                UI_TREE_MENU_SEP_1 = new ToolStripSeparator();
                UI_TREE_MENU_RENAME = new ToolStripMenuItem();
                UI_TREE_MENU_CLONE = new ToolStripMenuItem();
                UI_TREE_MENU_DELETE = new ToolStripMenuItem();
                UI_TREE_MENU_SEP_2 = new ToolStripSeparator();
                UI_TREE_MENU_CREATE_FOLDER = new ToolStripMenuItem();





                //UI_TREE_MENU_LOAD_PROJECT.DisplayStyle = ToolStripItemDisplayStyle.Text;
                //UI_TREE_MENU_LOAD_PROJECT.Name = UI_TREE_MENU_LOAD_PROJECT_NAME;
                //UI_TREE_MENU_LOAD_PROJECT.Text = "Load Project";
                ////UI_TREE_MENU_LOAD_PROJECT.Image = Resources.NewFolder;
                ////UI_TREE_MENU_LOAD_PROJECT.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                //UI_TREE_MENU_LOAD_PROJECT.Click += new System.EventHandler(UI_TREE_MENU_LOAD_PROJECT_Click);

                //UI_TREE_MENU_LOAD_PROJECT_DIR.DisplayStyle = ToolStripItemDisplayStyle.Text;
                //UI_TREE_MENU_LOAD_PROJECT_DIR.Name = UI_TREE_MENU_LOAD_PROJECT_NAME;
                //UI_TREE_MENU_LOAD_PROJECT_DIR.Text = "DIR";
                ////UI_TREE_MENU_LOAD_PROJECT.Image = Resources.NewFolder;
                ////UI_TREE_MENU_LOAD_PROJECT.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                //UI_TREE_MENU_LOAD_PROJECT_DIR.Click += new System.EventHandler(UI_TREE_MENU_LOAD_PROJECT_DIR_Click);

                //UI_TREE_MENU_BUILD_PROJECT.DisplayStyle = ToolStripItemDisplayStyle.Text;
                //UI_TREE_MENU_BUILD_PROJECT.Name = "Build";
                //UI_TREE_MENU_BUILD_PROJECT.Text = "Build";
                ////UI_TREE_MENU_LOAD_PROJECT.Image = Resources.NewFolder;
                ////UI_TREE_MENU_LOAD_PROJECT.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                //UI_TREE_MENU_BUILD_PROJECT.Click += new System.EventHandler(UI_TREE_MENU_BUILD_PROJECT_Click);


                //UI_TREE_MENU_RUN_PROJECT.DisplayStyle = ToolStripItemDisplayStyle.Text;
                //UI_TREE_MENU_RUN_PROJECT.Name = "Run";
                //UI_TREE_MENU_RUN_PROJECT.Text = "Run";
                ////UI_TREE_MENU_LOAD_PROJECT.Image = Resources.NewFolder;
                ////UI_TREE_MENU_LOAD_PROJECT.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                //UI_TREE_MENU_RUN_PROJECT.Click += new System.EventHandler(UI_TREE_MENU_RUN_PROJECT_Click);



                _treeViewMenu.Opening += new CancelEventHandler(treeViewMenu_Opening);
                _treeViewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                UI_TREE_MENU_OPEN,
                UI_TREE_MENU_SEP_1,
                UI_TREE_MENU_RENAME,
                UI_TREE_MENU_CLONE,
                UI_TREE_MENU_DELETE,
                UI_TREE_MENU_SEP_2//,
         //       UI_TREE_MENU_SEP_3,
        //        UI_TREE_MENU_SET_AS_CURRENT_DIR
            });

                UI_TREE_MENU_OPEN.Name = Constants.UI_TREE_MENU_OPEN;
                UI_TREE_MENU_OPEN.Text = Resources.TreeMenuOpen;
                UI_TREE_MENU_OPEN.Image = Resources.OpenFolder;
                UI_TREE_MENU_OPEN.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                UI_TREE_MENU_OPEN.ShortcutKeyDisplayString = Resources.EnterKey;
                UI_TREE_MENU_OPEN.Click += new System.EventHandler(UI_TREE_MENU_OPEN_Click);

                UI_TREE_MENU_SEP_1.Name = Constants.UI_TREE_MENU_SEP_1;

                UI_TREE_MENU_RENAME.Name = Constants.UI_TREE_MENU_RENAME;
                UI_TREE_MENU_RENAME.Text = Resources.TreeMenuRename;
                UI_TREE_MENU_RENAME.ShortcutKeys = Keys.F2;
                UI_TREE_MENU_RENAME.Click += new System.EventHandler(UI_TREE_MENU_RENAME_Click);

                UI_TREE_MENU_CLONE.Name = Constants.UI_TREE_MENU_CLONE;
                UI_TREE_MENU_CLONE.Text = Resources.TreeMenuClone;
                UI_TREE_MENU_CLONE.Click += new System.EventHandler(UI_TREE_MENU_CLONE_Click);

                UI_TREE_MENU_DELETE.Name = Constants.UI_TREE_MENU_DELETE;
                UI_TREE_MENU_DELETE.Text = Resources.TreeMenuDelete;
                UI_TREE_MENU_DELETE.ShortcutKeys = Keys.Delete;
                UI_TREE_MENU_DELETE.Click += new System.EventHandler(UI_TREE_MENU_DELETE_Click);

                UI_TREE_MENU_SEP_1.Name = Constants.UI_TREE_MENU_SEP_1;

                UI_TREE_MENU_CREATE_FOLDER.Name = Constants.UI_TREE_MENU_CREATE_FOLDER;
                UI_TREE_MENU_CREATE_FOLDER.Text = Resources.TreeMenuCreateFolder;
                UI_TREE_MENU_CREATE_FOLDER.Image = Resources.NewFolder;
                UI_TREE_MENU_CREATE_FOLDER.ImageTransparentColor = System.Drawing.Color.Fuchsia;
                UI_TREE_MENU_CREATE_FOLDER.Click += new System.EventHandler(UI_TREE_MENU_CREATE_FOLDER_Click);





                UI_TREE_MENU_SEP_3.Name = Constants.UI_TREE_MENU_SEP_3;

                UI_TREE_MENU_SET_AS_CURRENT_DIR.Name = Constants.UI_TREE_MENU_SET_AS_CURRENT_DIR;
                UI_TREE_MENU_SET_AS_CURRENT_DIR.Text = Resources.TreeMenuSetAsCurrentDir;
                UI_TREE_MENU_SET_AS_CURRENT_DIR.Click += new System.EventHandler(UI_TREE_MENU_SET_AS_CURRENT_DIR_Click);

                _mainTreeView.ContextMenuStrip = _treeViewMenu;

            }



            private void treeViewMenu_Opening(object sender, CancelEventArgs e)
            {
                UI_TREE_MENU_OPEN.Enabled = false;
                UI_TREE_MENU_RENAME.Enabled = false;
                UI_TREE_MENU_CLONE.Enabled = false;
                UI_TREE_MENU_DELETE.Enabled = false;
                UI_TREE_MENU_CREATE_FOLDER.Enabled = false;
                UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = false;

                if (_mainTreeView.Nodes.Count == 0) return;
                if (_mainTreeView.SelectedNode == null) return;

                UI_TREE_MENU_OPEN.Enabled = true;
                UI_TREE_MENU_RENAME.Enabled = true;
                UI_TREE_MENU_DELETE.Enabled = true;

                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                if (NodeIsFolder(node))
                {
                    UI_TREE_MENU_CREATE_FOLDER.Enabled = true;
                    UI_TREE_MENU_SET_AS_CURRENT_DIR.Enabled = true;

                    /*
                     * Don't allow the root node to be deleted or renamed.
                     */

                    if (node.Parent == null)
                    {
                        UI_TREE_MENU_RENAME.Enabled = false;
                        UI_TREE_MENU_DELETE.Enabled = false;
                    }
                }
                else
                {
                    UI_TREE_MENU_CLONE.Enabled = true;
                }
            }

            private void UI_TREE_MENU_OPEN_Click(object sender, EventArgs e)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                OpenNode(node);
            }

            private void UI_TREE_MENU_RENAME_Click(object sender, EventArgs e)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                RenameNode(node);
            }

            private void UI_TREE_MENU_CLONE_Click(object sender, EventArgs e)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                CloneNode(node);
            }

            private void UI_TREE_MENU_DELETE_Click(object sender, EventArgs e)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;
                if (node.Parent == null) return;

                DeleteNode(node);
            }

            private void UI_TREE_MENU_CREATE_FOLDER_Click(object sender, EventArgs e)
            {
                TreeNode node = _mainTreeView.SelectedNode;
                if (node == null) return;

                string path = node.Tag as string;
                if (path == null) return;

                CreateFolder(path);
            }


            private void UI_TREE_MENU_SET_AS_CURRENT_DIR_Click(object sender, EventArgs e)
            {
                string path = null;

                TreeNode node = _mainTreeView.SelectedNode;

                if (node == null)
                    path = _rootFolder;
                else if (NodeIsFolder(node))
                    path = node.Tag as string;

                if (path == null) return;

                Directory.SetCurrentDirectory(path);
                _applicationManager.NotifyFileSystemChange();
            }



    }
}
