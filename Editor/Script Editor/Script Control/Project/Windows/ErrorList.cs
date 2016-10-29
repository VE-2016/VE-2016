using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AIMS.Libraries.Forms.Docking;

namespace AIMS.Libraries.Scripting.ScriptControl
{
    public partial class ErrorList : DockableWindow
    {
        public event EventHandler<ListViewItemEventArgs> ItemDoubleClick = null;
        public ScriptControl ParentSc = null;
        public ErrorList(ScriptControl scriptControl)
        {
            ParentSc = scriptControl;
            InitializeComponent();
        }

        private void lvErrorList_Resize(object sender, EventArgs e)
        {
            lvErrorList.Columns[2].Width = lvErrorList.Width
            - (lvErrorList.Columns[0].Width +
                lvErrorList.Columns[1].Width +
                lvErrorList.Columns[3].Width +
                lvErrorList.Columns[4].Width +
                lvErrorList.Columns[5].Width
              );
        }

        public int ParserErrorCount
        {
            get { return lvErrorList.Items.Count; }
        }

        public void ConvertToLanguage(ScriptLanguage OldLang, ScriptLanguage NewLanguage)
        {
            if (lvErrorList.Items.Count > 0)
            {
                string OldExt = (OldLang == ScriptLanguage.CSharp ? ".cs" : ".vb");
                string NewExt = (NewLanguage == ScriptLanguage.CSharp ? ".cs" : ".vb");

                foreach (ListViewItem lvExisting in lvErrorList.Items)
                {
                    lvExisting.Tag = lvExisting.Tag.ToString().Replace(OldExt, NewExt);
                    lvExisting.SubItems[3].Text = lvExisting.SubItems[3].Text.Replace(OldExt, NewExt);
                }
            }
        }

        public void ComilerErrors(Document doc, System.CodeDom.Compiler.CompilerErrorCollection Errors)
        {
            //delete all previous compiler errors
            string fileName = "";
            foreach (ListViewItem lvExisting in lvErrorList.Items)
            {
                if (Convert.ToInt32(lvExisting.SubItems[1].Text) > 0)   //compiler Warnings and errors
                {
                    fileName = lvExisting.SubItems[3].Text;
                    doc = ParentSc.ShowFile(fileName);
                    if (doc != null) doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
                    lvExisting.Remove();
                }
            }
            //Add Errors
            ListViewItem lvItem;
            if (Errors.HasErrors || Errors.HasWarnings)
            {
                foreach (System.CodeDom.Compiler.CompilerError e in Errors)
                {
                    int lineNo = (e.Line - 1);
                    int ColNo = (e.Column - 1);

                    lineNo = lineNo < 0 ? 0 : lineNo;
                    ColNo = ColNo < 0 ? 0 : ColNo;

                    fileName = System.IO.Path.GetFileName(e.FileName);
                    if (e.IsWarning)
                    {
                        lvItem = new ListViewItem(new string[] { "", "2", e.ErrorText, fileName, lineNo.ToString(), ColNo.ToString() });

                        lvItem.StateImageIndex = 2;
                    }
                    else
                    {
                        lvItem = new ListViewItem(new string[] { "", "1", e.ErrorText, fileName, lineNo.ToString(), ColNo.ToString() });
                        lvItem.StateImageIndex = 3;
                    }

                    lvItem.Tag = fileName + "-" + lineNo.ToString() + "-" + ColNo.ToString();
                    lvErrorList.Items.Add(lvItem);
                    doc = ParentSc.ShowFile(fileName);
                    if (doc != null) doc.HighlightError(lineNo, ColNo, e.IsWarning, e.ErrorText);
                }
            }
            UpdateSummary();
        }

        public void ProjectErrors(Document doc, NRefactory.Parser.Errors p)
        {
            ListViewItem lvItem;
            string msg = p.ErrorOutput.Trim();
            int lineNo = (p.LineNo - 1);
            int ColNo = (p.ColumnNo - 1);

            string fileName = doc.FileName;
            lineNo = lineNo < 0 ? 0 : lineNo;
            ColNo = ColNo < 0 ? 0 : ColNo;
            try
            {
                lvItem = new ListViewItem(new string[] { "", "0", msg, fileName, lineNo.ToString(), ColNo.ToString() });
                lvItem.Tag = fileName + "-" + lineNo.ToString() + "-" + ColNo.ToString();
                foreach (ListViewItem lvExisting in lvErrorList.Items)
                {
                    if (lvExisting.Tag.ToString() == lvItem.Tag.ToString())
                    {
                        try
                        {
                            doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
                        }
                        finally
                        {
                            lvExisting.Remove();
                        }
                    }
                    if (Convert.ToInt32("0" + lvExisting.SubItems[1].Text) == 0 && lvExisting.SubItems[3].Text == fileName) //Parser Errors
                    {
                        try
                        {
                            doc.HighlightRemove(Convert.ToInt32("0" + lvExisting.SubItems[4].Text), Convert.ToInt32("0" + lvExisting.SubItems[5].Text));
                        }
                        finally
                        {
                            lvExisting.Remove();
                        }
                    }
                }
                if (p.Count == 0) return;
                doc.HighlightError(lineNo, ColNo, false, msg);
                lvItem.StateImageIndex = 3;
                lvErrorList.Items.Add(lvItem);
            }
            finally
            {
                UpdateSummary();
            }
        }
        private void UpdateSummary()
        {
            if (lvErrorList.Items.Count > 0)
                this.Text = "Error List (" + lvErrorList.Items.Count + ")";
            else
                this.Text = "Error List";
            this.TabText = this.Text;
        }

        private void lvErrorList_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem item = lvErrorList.SelectedItems[0];
            string FileName = item.SubItems[3].Text;
            int lineNo = Convert.ToInt32("0" + item.SubItems[4].Text);
            int colNo = Convert.ToInt32("0" + item.SubItems[5].Text);
            OnItemDoubleClick(new ListViewItemEventArgs(FileName, lineNo, colNo));
        }

        protected virtual void OnItemDoubleClick(ListViewItemEventArgs e)
        {
            if (ItemDoubleClick != null)
            {
                ItemDoubleClick(this, e);
            }
        }
    }

    public class ListViewItemEventArgs : EventArgs
    {
        public string FileName = "";
        public int ColumnNo = 0;
        public int LineNo = 0;
        public ListViewItemEventArgs(string fileName, int lineNo, int colNo)
        {
            FileName = fileName;
            LineNo = lineNo;
            ColumnNo = colNo;
        }
    }
}