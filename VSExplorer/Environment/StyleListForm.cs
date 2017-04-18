using StyleCop;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class StyleListForm : Form
    {
        public StyleListForm()
        {
            InitializeComponent();
            lv = listView1;
            _warningsButton = toolStripButton3;
            _errorsButton = toolStripButton2;
            InitializeListView();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
        }

        public ExplorerForms ef { get; set; }

        public ListView lv { get; set; }

        public void InitializeListView()
        {
            SuspendLayout();

            //Size sz = this.Size;

            //lv.Size = new System.Drawing.Size(sz.Width , sz.Height );
            //lv.Location = new Point(0, 30);

            //lv.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            lv.Clear();

            lv.View = View.Details;

            lv.CheckBoxes = true;

            lv.FullRowSelect = true;

            lv.HideSelection = false;

            lv.Columns.Add("");
            lv.Columns.Add("");
            lv.Columns.Add("Code");
            lv.Columns.Add("Description");
            lv.Columns.Add("Project");
            lv.Columns.Add("File");
            lv.Columns.Add("Line");
            lv.Columns.Add("Suppression");

            int i = 0;
            foreach (ColumnHeader column in lv.Columns)
            {
                if (i < 3)
                    column.Width = 25;
                else if (i < 4)
                    column.Width = 500;
                else break;

                i++;
            }

            ResumeLayout();
        }

        public void ClearOutput()
        {
            Es = null;
            Ws = null;

            lv.Invoke(new Action(() => { lv.Items.Clear(); }));
            //            W = null;
            _errorsButton.Text = "Errors";
            _warningsButton.Text = "Warnings";
        }

        public ToolStripButton _errorsButton { get; set; }

        public ToolStripButton _warningsButton { get; set; }

        public ArrayList W { get; set; }

        public ArrayList Es { get; set; }

        public ArrayList Ws { get; set; }

        public void LoadLogItem(ArrayList Vs)
        {
            lv.Invoke(new Action(() =>
            {
                Ws = Vs;

                lv.VirtualListSize = Ws.Count;

                lv.VirtualMode = true;
            }));
        }

        public void LoadLogItems(ArrayList Vs)
        {
            SuspendLayout();
            foreach (StyleCop.ViolationEventArgs e in Vs)
            {
                ListViewItem v = new ListViewItem();
                v.Text = "";
                v.SubItems.Add("");
                v.SubItems.Add("");
                v.SubItems.Add(e.Message);
                v.SubItems.Add(e.SourceCode.Project.ToString());
                v.SubItems.Add(Path.GetFileName(e.Location.ToString()));
                v.SubItems.Add(e.LineNumber.ToString());
                v.SubItems.Add("project");

                //lv.Columns.Add("");
                //lv.Columns.Add("");
                //lv.Columns.Add("Code");
                //lv.Columns.Add("Description");
                //lv.Columns.Add("Project");
                //lv.Columns.Add("File");
                //lv.Columns.Add("Line");
                //lv.Columns.Add("Suppression");

                v.Checked = true;

                v.Tag = e;

                lv.Invoke(new Action(() => { lv.Items.Add(v); }));
            }
            ResumeLayout();
        }

        public void LoadCompileResults(ArrayList es, ArrayList ws, ArrayList me)
        {
            Es = es;
            Ws = ws;

            SuspendLayout();

            W = new ArrayList();

            foreach (Microsoft.Build.Framework.BuildErrorEventArgs e in Es)
            {
                ListViewItem v = new ListViewItem();
                v.Text = "";
                v.SubItems.Add("");
                v.SubItems.Add(e.Code);
                v.SubItems.Add(e.Message);
                v.SubItems.Add(Path.GetFileNameWithoutExtension(e.ProjectFile));
                v.SubItems.Add(Path.GetFileName(e.File));
                v.SubItems.Add(e.LineNumber.ToString());
                v.SubItems.Add("project");

                v.Checked = true;

                v.Tag = e;

                lv.Items.Add(v);

                W.Add(v);
            }

            foreach (Microsoft.Build.Framework.BuildWarningEventArgs e in Ws)
            {
                ListViewItem v = new ListViewItem();
                v.Text = "";
                v.SubItems.Add("");
                v.SubItems.Add(e.Code);
                v.SubItems.Add(e.Message);
                v.SubItems.Add(Path.GetFileNameWithoutExtension(e.ProjectFile));
                v.SubItems.Add(Path.GetFileName(e.File));
                v.SubItems.Add(e.LineNumber.ToString());

                v.Checked = false;

                v.Tag = e;

                lv.Items.Add(v);

                W.Add(v);
            }

            //foreach (Microsoft.Build.Framework.BuildMessageEventArgs e in me)
            //{
            //    ListViewItem v = new ListViewItem();
            //    v.Text = "";
            //    v.SubItems.Add("");
            //    v.SubItems.Add(e.Code);
            //    v.SubItems.Add(e.Message);
            //    v.SubItems.Add(Path.GetFileNameWithoutExtension(e.ProjectFile));
            //    v.SubItems.Add(Path.GetFileName(e.File));
            //    v.SubItems.Add(e.LineNumber.ToString());

            //    v.Checked = false;

            //    v.Tag = e;

            //    lv.Items.Add(v);

            //    W.Add(v);
            //}

            ResumeLayout();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (lv == null)
                return;

            if (W == null)
                return;

            lv.Items.Clear();

            foreach (ListViewItem v in W)
            {
                if (v.Checked == true)
                    lv.Items.Add(v);
            }

            if (lv.Items.Count > 0)
                _errorsButton.Text = "Errors " + lv.Items.Count.ToString();
            else
                _errorsButton.Text = "Errors";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (W == null)
                return;

            if (lv == null)
                return;

            lv.Items.Clear();

            foreach (ListViewItem v in W)
            {
                if (v.Checked == false)
                    lv.Items.Add(v);
            }

            if (lv.Items.Count > 0)
                _warningsButton.Text = "Warnings " + lv.Items.Count.ToString();
            else
                _warningsButton.Text = "Warnings";
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if ((e.Item.SubItems[1] == e.SubItem))
            {
                e.DrawDefault = false;
                e.DrawBackground();
                if (e.Item.Tag.GetType() == typeof(Microsoft.Build.Framework.BuildErrorEventArgs))
                {
                    e.Graphics.DrawImage(Resources.Error_6206, e.SubItem.Bounds.Location);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + Resources.Error_6206.Width), e.SubItem.Bounds.Location.Y);
                }
                else
                {
                    e.Graphics.DrawImage(Resources.warnings, e.SubItem.Bounds.Location);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + Resources.warnings.Width), e.SubItem.Bounds.Location.Y);
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv.SelectedIndices.Count <= 0)
                return;
            int i = lv.SelectedIndices[0];
            ListViewItem v = lv.Items[i];

            if (ef == null)
                return;

            if (v.Tag == null)
                return;
            if (v.Tag.GetType() == typeof(Microsoft.Build.Framework.BuildErrorEventArgs))
            {
                Microsoft.Build.Framework.BuildErrorEventArgs b = v.Tag as Microsoft.Build.Framework.BuildErrorEventArgs;

                string project = b.ProjectFile;
                string file = Path.GetDirectoryName(b.ProjectFile) + "\\" + b.File;
                int c = b.LineNumber;
                int p = b.ColumnNumber;
                int es = b.EndLineNumber;
                int ep = b.EndColumnNumber;

                ef.OpenFileLine(file, c.ToString(), p);
            }
            else if (v.Tag.GetType() == typeof(Microsoft.Build.Framework.BuildWarningEventArgs))
            {
                Microsoft.Build.Framework.BuildWarningEventArgs b = v.Tag as Microsoft.Build.Framework.BuildWarningEventArgs;

                string project = b.ProjectFile;
                string file = Path.GetDirectoryName(b.ProjectFile) + "\\" + b.File;
                int c = b.LineNumber;
                int p = b.ColumnNumber;
                int es = b.EndLineNumber;
                int ep = b.EndColumnNumber;

                ef.OpenFileLine(file, c.ToString(), p);
            }
            else if (v.Tag.GetType() == typeof(ViolationEventArgs))
            {
                ViolationEventArgs b = v.Tag as ViolationEventArgs;

                string project = b.SourceCode.Path;
                string file = Path.GetDirectoryName(project) + "\\" + b.SourceCode.Name;
                int c = b.LineNumber;
                int p = b.LineNumber;

                //ef.BeginInvoke(new Action(() => { ef.OpenFileLineHighlight(file, c.ToString(), p); }));

                AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                GotoWarning g = Warning;
                g.BeginInvoke(file, c.ToString(), p, callBack, "null");
            }
        }

        private static void ProcessInformation(IAsyncResult result)
        {
        }

        public delegate void GotoWarning(string s, string c, int p);

        public void Warning(string s, string c, int p)
        {
            ef.OpenFileLineHighlight(s, c, p);
        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < 0)
                return;

            StyleCop.ViolationEventArgs ee = Ws[e.ItemIndex] as StyleCop.ViolationEventArgs;

            ListViewItem v = new ListViewItem();
            v.Text = "";
            v.SubItems.Add("");
            v.SubItems.Add("");
            v.SubItems.Add(ee.Message);
            v.SubItems.Add(Path.GetFileName(ee.SourceCode.Project.ToString()));
            v.SubItems.Add(ee.SourceCode.Path);
            v.SubItems.Add(ee.LineNumber.ToString());
            v.SubItems.Add("project");

            v.Checked = true;

            v.Tag = ee;

            e.Item = v;
        }
    }
}