using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using AIMS.Libraries.Scripting.NRefactory;
using System.IO;

namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class NewFileDialog : Form
    {
        private ScriptLanguage _Language = ScriptLanguage.CSharp;
        private StringCollection _CreatedFileNames = null;
        private SelectedItemType _selectedItemType = SelectedItemType.None;
        public NewFileDialog(ScriptLanguage Lang, StringCollection CreatedFileNames)
        {
            _Language = Lang;
            _CreatedFileNames = CreatedFileNames;
            InitializeComponent();
            InitilizeListView(Lang);
        }

        private void InitilizeListView(ScriptLanguage Lang)
        {
            if (_Language == ScriptLanguage.CSharp)
            {
                ListViewItem listViewItem1 = new ListViewItem("Class", "Code_ClassCS.ico");
                listViewItem1.ToolTipText = "An empty class definiton.";
                ListViewItem listViewItem2 = new ListViewItem("Interface", "Code_CodeFileCS.ico");
                listViewItem2.ToolTipText = "An empty interface definition.";

                listViewItem1.Selected = true;
                listView1.Items.AddRange(new ListViewItem[] {
                listViewItem1,
                listViewItem2});
            }
            else
            {
                ListViewItem listViewItem1 = new ListViewItem("Class", "CodeClass.ico");
                listViewItem1.ToolTipText = "An empty class definiton.";
                ListViewItem listViewItem2 = new ListViewItem("Interface", "Code_CodeFile.ico");
                listViewItem2.ToolTipText = "An empty interface definition.";
                listViewItem1.Selected = true;
                listView1.Items.AddRange(new ListViewItem[] {
                listViewItem1,
                listViewItem2});
            }
        }

        private string GenerateValidFileName(ListViewItem lvItem)
        {
            string baseName = "";
            switch (lvItem.Text)
            {
                case "Class":
                    baseName = "Class";
                    _selectedItemType = SelectedItemType.Class;
                    break;
                case "Interface":
                    baseName = "Interface";
                    _selectedItemType = SelectedItemType.Interface;
                    break;
            }

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

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem lvItem = listView1.SelectedItems[0];
                txtHint.Text = lvItem.ToolTipText;
                txtName.Text = GenerateValidFileName(lvItem);
            }
        }

        public string FileName
        {
            get
            {
                string name = txtName.Text;
                if (name.Length > 0)
                {
                    if (name.LastIndexOf('.') > 0)
                        return name;
                    else
                        return name + (_Language == ScriptLanguage.CSharp ? ".cs" : ".vb");
                }
                else return "";
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            _selectedItemType = SelectedItemType.None;
            this.Hide();
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            if (_CreatedFileNames != null)
            {
                if (_CreatedFileNames.Contains(txtName.Text) == true)
                {
                    MessageBox.Show(this, "A file with the name " + txtName.Text + " already exists in the current project. Please give a unique name to the item you are adding , or delete the existing item first.", "AIMS Script Editor");
                    return;
                }
            }
            this.Hide();
        }

        public static string GetDefaultNamespace(AIMS.Libraries.Scripting.ScriptControl.Project.IProject project, string fileName)
        {
            if (project == null)
                throw new ArgumentNullException("project");
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            string relPath = FileUtility.GetRelativePath(project.Directory, Path.GetDirectoryName(project.Directory + fileName));
            string[] subdirs = relPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            StringBuilder standardNameSpace = new StringBuilder(project.RootNamespace);
            foreach (string subdir in subdirs)
            {
                if (subdir == "." || subdir == ".." || subdir.Length == 0)
                    continue;
                if (subdir.Equals("src", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (subdir.Equals("source", StringComparison.OrdinalIgnoreCase))
                    continue;
                standardNameSpace.Append('.');
                standardNameSpace.Append(NewFileDialog.GenerateValidClassOrNamespaceName(subdir, true));
            }
            return standardNameSpace.ToString();
        }

        public SelectedItemType SelectedItemType
        {
            get { return _selectedItemType; }
        }
        internal static string GenerateValidClassOrNamespaceName(string className, bool allowDot)
        {
            if (className == null)
                throw new ArgumentNullException("className");
            className = className.Trim();
            if (className.Length == 0)
                return string.Empty;
            StringBuilder nameBuilder = new StringBuilder();
            if (className[0] != '_' && !char.IsLetter(className, 0))
                nameBuilder.Append('_');
            for (int idx = 0; idx < className.Length; ++idx)
            {
                if (Char.IsLetterOrDigit(className[idx]) || className[idx] == '_')
                {
                    nameBuilder.Append(className[idx]);
                }
                else if (className[idx] == '.' && allowDot)
                {
                    nameBuilder.Append('.');
                }
                else
                {
                    nameBuilder.Append('_');
                }
            }
            return nameBuilder.ToString();
        }
    }
    public enum SelectedItemType
    {
        None,
        Class,
        Interface
    }
}