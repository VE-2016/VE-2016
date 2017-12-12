using Microsoft.CodeAnalysis;
using ScriptControl;
using ScriptControl.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer
{
    public partial class ErrorListForm : Form
    {
        public ErrorListForm()
        {
            InitializeComponent();
            SuspendLayout();
            lv = listView1;
            DD = new ArrayList();
            Es = new ArrayList();
            Ws = new ArrayList();
            W = new ArrayList();
            Me = new ArrayList();
            Ep = new ArrayList();
            Wp = new ArrayList();
            Mp = new ArrayList();
            _warningsButton = toolStripButton3;
            _errorsButton = toolStripButton2;
            _messagesButton = toolStripButton4;
            //InitializeListView();
            InitializeDataGrid();
            LoadSettings();
            //CodeEditorControl.IntErrors.ContentChanged += IntError_ContentChanged;
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;
            VSSolution.Errors += VSSolution_Errors;
            ResumeLayout();
        }

        private void VSSolution_Errors(object sender, OpenFileEventArgs e)
        {
            //MessageBox.Show("Errors found - " + e.Errors.Count);

            string filename = e.filename;

            filename = Path.GetFileName(filename);

            var ns = dg.Rows.Cast<DataGridViewRow>().Where(s => (string)s.Cells["File"].Value == filename).ToList();

            foreach (var b in ns)
            {
                dg.Rows.Remove(b);
                Diagnostic dc = b.Tag as Diagnostic;
                RemoveFromHash(dc);
            }

            LoadIntellisenseResults(e.Errors);
        }

        private Dictionary<int, Diagnostic> hc { get; set; }

        private ToolStripButton _messagesButton { get; set; }

        public void LoadEF(ExplorerForms c)
        {
            ef = c;
            ef.scr.activeDocument += Scr_activeDocument;
        }

        private void Scr_activeDocument(object sender, EventArgs e)
        {
            // LoadResults();
        }

        public ArrayList DD { get; set; }

        public void LoadSettings()
        {
            if (ScriptControl.ScriptControl.settings != null)
            {
                Settings s = ScriptControl.ScriptControl.settings;

                if (s.Theme == "VS2012Light")
                {
                    lv.BackColor = Color.FromKnownColor(KnownColor.Control);
                    lv.BackColor = Color.FromKnownColor(KnownColor.Control);
                    colorListViewHeader(ref listView1, Color.FromKnownColor(KnownColor.Control), Color.Black);
                }
            }
        }

        //List view header formatters
        public static void colorListViewHeader(ref ListView list, Color backColor, Color foreColor)
        {
            list.OwnerDraw = true;
            list.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => headerDraw(sender, e, backColor, foreColor)
                );
            list.DrawItem += new DrawListViewItemEventHandler(bodyDraw);
        }

        private static void headerDraw(object sender, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Header.Text, e.Font, new SolidBrush(foreColor), e.Bounds);
            e.DrawDefault = false;
        }

        private static void bodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
        }

        public ExplorerForms ef { get; set; }

        public ListView lv { get; set; }

        public void InitializeListView()
        {
            SuspendLayout();

            lv.Clear();

            lv.View = View.Details;

            lv.CheckBoxes = true;

            lv.FullRowSelect = true;

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

        public DataGridView dg { get; set; }

        public void InitializeDataGrid()
        {
            SuspendLayout();

            dg = new DataGridView();

            this.Controls.Add(dg);

            dg.Location = lv.Location;

            dg.Size = lv.Size;

            dg.Anchor = lv.Anchor;

            lv.Visible = false;

            dg.Columns.Clear();

            dg.Rows.Clear();

            dg.RowHeadersVisible = false;

            dg.BackgroundColor = Color.FromKnownColor(KnownColor.Control);

            dg.DefaultCellStyle.SelectionBackColor = Color.FromKnownColor(KnownColor.InactiveCaption);

            dg.DefaultCellStyle.SelectionForeColor = Color.Black;

            dg.EnableHeadersVisualStyles = false;
            dg.ColumnHeadersDefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Control);

            dg.AllowUserToAddRows = false;

            dg.AllowUserToOrderColumns = false;

            dg.AllowUserToResizeRows = false;

            dg.EditMode = DataGridViewEditMode.EditProgrammatically;

            dg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dg.CellBorderStyle = DataGridViewCellBorderStyle.None;

            dg.GridColor = Color.Blue;

            dg.BorderStyle = BorderStyle.None;

            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dg.Columns.Add("Show Details", "");
            DataGridViewImageColumn img = new DataGridViewImageColumn();
            Image image = ve_resource.Breakall_6323;
            img.Image = image;
            dg.Columns.Add(img);
            dg.Columns.Add("Code", "Code");
            dg.Columns.Add("Description", "Description");
            dg.Columns.Add("Project", "Project");
            dg.Columns.Add("File", "File");
            dg.Columns.Add("Line", "Line");
            dg.Columns.Add("Suppression", "Suppression");

            dg.Columns[0].Width = 30;
            dg.Columns[1].Width = 30;

            dg.CellMouseEnter += Dg_CellMouseEnter;

            dg.CellMouseLeave += Dg_CellMouseLeave;

            dg.CellContentDoubleClick += Dg_CellContentDoubleClick;

            dg.CellValueNeeded += Dg_CellValueNeeded;

            ResumeLayout();
        }

        private void Dg_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Dg_SelectionChanged(null, null);
        }

        private void Dg_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            int c = e.ColumnIndex;
            int r = e.RowIndex;
        }

        public void SetVirtualMode()
        {
            dg.VirtualMode = true;
        }

        private void Dg_SelectionChanged(object sender, EventArgs e)
        {
            if (dg.SelectedRows.Count <= 0)
                return;
            int i = dg.SelectedRows[0].Index;

            DataGridViewRow v = dg.Rows[i];

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

                AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                workerDisplayLine wde = DisplayLine;
                wde.BeginInvoke(file, c, p, 100, callBack, "null");

                // ef.OpenFileLine(file, c.ToString(), p);
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

                AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                workerDisplayLine wde = DisplayLine;
                wde.BeginInvoke(file, c, p, 100, callBack, "null");

                //ef.OpenFileLine(file, c.ToString(), p);
            }
            else //if (v.Tag.GetType() == typeof(Diagnostic))
            {
                Diagnostic b = v.Tag as Diagnostic;

                if (b != null)
                {
                    //string project = b.vp.FileName;
                    if (b.Location == Microsoft.CodeAnalysis.Location.None)
                        return;
                    if (b.Location.SourceTree == null)
                        return;
                    string file = b.Location.SourceTree.FilePath;

                    Microsoft.CodeAnalysis.FileLinePositionSpan c = b.Location.GetLineSpan();
                    int ps = c.StartLinePosition.Line;
                    int cs = 0;
                    int es = c.StartLinePosition.Character;

                    int start = b.Location.SourceSpan.Start;
                    int length = b.Location.SourceSpan.Length;

                    AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                    workerDisplayLine wde = DisplayLine;
                    wde.BeginInvoke(file, start, length, 100, callBack, "null");

                    //ef.OpenFileLine(file, c.ToString(), p);
                }
            }
        }

        public delegate void workerDisplayLine(string file, int start, int length, int g);

        private void DisplayLine(string file, int start, int length, int g)
        {
            ef.BeginInvoke(new Action(() => { ef.OpenFileXY(file, start, length, g); }));
        }

        private void Dg_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                dg.Columns[e.ColumnIndex].HeaderCell.Style.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void Dg_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                dg.Columns[e.ColumnIndex].HeaderCell.Style.BackColor = Color.LightBlue;
            }
        }

        private void Dg_Resize(object sender, EventArgs e)
        {
            Size s = dg.Size;

            int p = 0;

            int i = 0;
            foreach (DataGridViewColumn column in dg.Columns)
            {
                if (p != 3)
                    p += column.Width;
            }

            dg.Columns[3].Width = s.Width - p;
        }

        public void ClearOutput()
        {
            Es = new ArrayList();
            Ws = new ArrayList();
            W = new ArrayList();
            dg.Rows.Clear();

            //   lv.Invoke(new Action(() => { lv.Items.Clear(); }));
            //            W = null;
            _errorsButton.Text = "Errors";
            _warningsButton.Text = "Warnings";
        }

        public ToolStripButton _errorsButton { get; set; }

        public ToolStripButton _warningsButton { get; set; }

        public ArrayList W { get; set; }
        public ArrayList Es { get; set; }
        public ArrayList Ws { get; set; }

        public ArrayList Mp { get; set; }
        public ArrayList Ep { get; set; }
        public ArrayList Wp { get; set; }

        public void LoadResults()
        {
            ToolStripComboBox b = toolStripComboBox2;

            //lv.Items.Clear();

            dg.Rows.Clear();

            if (b.SelectedIndex == 0 || b.SelectedIndex == 2)
            {
                // LoadIntellisenseResults(hc);
            }
            if (b.SelectedIndex == 0 || b.SelectedIndex == 1)
            {
                LoadCompileResults(Es, Ws, Me);
            }

            dg.Refresh();
        }

        public void _LoadIntellisenseResults()
        {
            //ScriptControl.ScripControlscr = null;

            //VSSolution vs = null;

            //string filename = "";

            //if (ef != null)
            //    if (ef.scr != null)
            //        scr = ef.scr;

            //IMS.Libraries.Scripting.ScriptControl.Document d = scr.GetActiveDocument();

            //filename = d.FileName;

            //ToolStripComboBox b = toolStripComboBox1;

            //int i = b.SelectedIndex;

            //VSProject vp = ef.GetVSSolution().MainVSProject;

            //if (vp != null)
            //    if (i == 2)
            //    {
            //        foreach (IntErrors e in DD)
            //        {
            //            if (vp != null)
            //                if (e.vp != vp)
            //                    continue;

            //            ListViewItem v = new ListViewItem();
            //            v.Text = "";

            //            int rowId = dg.Rows.Add();
            //            DataGridViewRow row = dg.Rows[rowId];

            //            row.Cells[0].Value = "";
            //            row.Cells[0].Value = "e.Code";
            //            row.Cells[0].Value = e.e.Message;
            //            row.Cells[0].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
            //            row.Cells[0].Value = Path.GetFileName(e.file);
            //            row.Cells[0].Value = e.e.Region.BeginLine.ToString();
            //            row.Cells[0].Value = "project";

            //            v.Checked = true;

            //            v.Tag = e;

            //            lv.Items.Add(v);
            //       }

            //        return;
            //    }

            //if (i == 3)
            //{
            //    foreach (IntErrors e in DD)
            //    {
            //        if (filename != "")
            //            if (e.file != filename)
            //                continue;

            //        ListViewItem v = new ListViewItem();
            //        v.Text = "";
            //        v.SubItems.Add("");
            //        v.SubItems.Add("e.Code");
            //        v.SubItems.Add(e.e.Message);
            //        v.SubItems.Add(Path.GetFileNameWithoutExtension(e.vp.FileName));
            //        v.SubItems.Add(Path.GetFileName(e.file));
            //        v.SubItems.Add(e.e.Region.BeginLine.ToString());
            //        v.SubItems.Add("project");

            //        v.Checked = true;

            //        v.Tag = e;

            //        lv.Items.Add(v);
            //    }

            //    return;
            //}

            //foreach (IntErrors e in DD)
            //{
            //    ListViewItem v = new ListViewItem();
            //    v.Text = "";
            //    v.SubItems.Add("");
            //    v.SubItems.Add("e.Code");
            //    v.SubItems.Add(e.e.Message);
            //    v.SubItems.Add(Path.GetFileNameWithoutExtension(e.file));
            //    v.SubItems.Add(Path.GetFileName(e.file));
            //    v.SubItems.Add(e.e.Region.BeginLine.ToString());
            //    v.SubItems.Add("project");

            //    v.Checked = true;

            //    v.Tag = e;

            //    lv.Items.Add(v);
            //}
        }

        public ArrayList LI { get; set; }

        private static object s_obs = new object();

        private Dictionary<int, DataGridViewRow> hcd = new Dictionary<int, DataGridViewRow>();

        public void RemoveFromHash(Diagnostic dc)
        {
            int hash = dc.GetMessage().GetHashCode();

            if (hcd.ContainsKey(hash))
                hcd.Remove(hash);
        }

        public void LoadIntellisenseResults(List<Diagnostic> hc)
        {
            lock (s_obs)
            {
                if (ef == null)
                    return;

                ScriptControl.ScriptControl scr = null;

                VSSolution vs = null;

                string filename = "";

                if (ef != null)
                    if (ef.scr != null)
                        scr = ef.scr;

                ToolStripComboBox b = toolStripComboBox1;

                int i = b.SelectedIndex;

                vs = ef.GetVSSolution();

                VSProject vp = null;
                //if (vs != null)
                //    vp = vs.MainVSProject;

                //dg.SuspendLayout();

                //LI = new ArrayList();

                //if (vp != null)
                //    if (i == 2)
                //    {
                //        foreach (IntErrors e in DD)
                //        {
                //            if (vp != null)
                //                if (e.vp != vp)
                //                    continue;

                //            int rowId = dg.Rows.Add();
                //            DataGridViewRow row = dg.Rows[rowId];

                //            row.Cells[0].Value = "";
                //            row.Cells[1].Value = ve_resource.Breakall_6323;
                //            row.Cells[2].Value = "e.Code";
                //            row.Cells[3].Value = e.e.Message;
                //            row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                //            row.Cells[5].Value = Path.GetFileName(e.file);
                //            row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                //            row.Cells[7].Value = "project";

                //            string message = e.e.Message;

                //            //if (message.StartsWith("UnknownIdentifier") == true)
                //            //    MessageBox.Show("Unknown identifier");

                //            //v.Checked = true;

                //            row.Tag = e;

                //            //lv.Items.Add(v);
                //        }
                //        dg.ResumeLayout();
                //        return;
                //    }

                //if (i == 3 || i == 0)
                {
                    if (hc != null)
                    {
                    }

                    //filename = d.FileName;

                    foreach (/*IntErrors e in DD*/ Diagnostic hs in hc)
                    {
                        //if (filename != "")
                        //    if (e.file != filename)
                        //        continue;

                        Diagnostic dc = hs;// hc[hs];

                        filename = dc.Location.SourceTree.FilePath;

                        AvalonDocument d = scr.FileOpened(filename, false);

                        int hash = dc.GetMessage().GetHashCode();

                        if (hcd.ContainsKey(hash))
                            continue;

                        int line = 0;
                        string file = "";
                        if (dc.Location != Microsoft.CodeAnalysis.Location.None)
                        {
                            if (dc.Location.SourceTree != null)
                            {
                                file = dc.Location.SourceTree.FilePath;// syntaxTree.FilePath;
                                FileLinePositionSpan c = dc.Location.GetLineSpan();
                                line = c.StartLinePosition.Line;
                            }
                        }

                        int rowId = dg.Rows.Add();
                        DataGridViewRow row = dg.Rows[rowId];
                        row.InheritedStyle.BackColor = Color.FromKnownColor(KnownColor.Control);
                        //dg.Rows.Remove(row);
                        //dg.Rows.Insert(0, row);
                        row.Cells[0].Value = dc.Descriptor.Category;
                        if (dc.Severity.ToString() == "Error")
                        {
                            row.Cells[1].Value = new Bitmap(ve_resource.Breakall_6323, 15, 15);
                            Ep.Add(row);
                        }
                        else if (dc.Severity.ToString() == "Warning")
                        {
                            row.Cells[1].Value = new Bitmap(ve_resource.StatusWarning_16x, 15, 15);
                            Wp.Add(row);
                        }
                        else
                        {
                            row.Cells[1].Value = new Bitmap(ve_resource.StatusHelp_256x, 15, 15);
                            Mp.Add(row);
                        }

                        row.Cells[2].Value = dc.Descriptor.Id;
                        row.Cells[3].Value = dc.GetMessage();

                        if (dc.Location != Microsoft.CodeAnalysis.Location.None)
                            if (vp != null)
                                row.Cells[4].Value = Path.GetFileNameWithoutExtension(vp.FileName);
                            else if (dc.Location != Microsoft.CodeAnalysis.Location.None)
                                if (dc.Location.SourceTree != null)
                                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(dc.Location.SourceTree.FilePath);
                        row.Cells[5].Value = Path.GetFileName(file);
                        if(line > 0)
                        row.Cells[6].Value = d.GetLineExtended(line) + 1;
                        row.Cells[7].Value = "project";

                        //row.Cells[0].Value = "";
                        //row.Cells[1].Value = ve_resource.Errors;
                        //row.Cells[2].Value = "e.Code";
                        //row.Cells[3].Value = e.e.Message;
                        //row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                        //row.Cells[5].Value = Path.GetFileName(e.file);
                        //row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                        //row.Cells[7].Value = "project";

                        //ListViewItem v = new ListViewItem();
                        //v.Text = "";
                        //v.SubItems.Add("");
                        //v.SubItems.Add("e.Code");
                        //v.SubItems.Add(e.e.Message);
                        //v.SubItems.Add(Path.GetFileNameWithoutExtension(e.vp.FileName));
                        //v.SubItems.Add(Path.GetFileName(e.c.FileName));
                        //v.SubItems.Add(e.e.Region.BeginLine.ToString());
                        //v.SubItems.Add("project");

                        //v.Checked = true;

                        row.Tag = dc;

                        dg.Rows.Remove(row);
                        dg.Rows.Insert(0, row);

                        hcd.Add(hash, row);

                        //lv.Items.Add(v);
                    }

                    dg.ResumeLayout();

                    //ef.scr.FocusActiveDocument();
                    return;
                }

                //ArrayList F = ef.scr.GetOpenFiles();
                /*
                foreach (IntErrors e in DD)
                {
                    //ListViewItem v = new ListViewItem();
                    //v.Text = "";
                    //v.SubItems.Add("");
                    //v.SubItems.Add("e.Code");
                    //v.SubItems.Add(e.e.Message);
                    //v.SubItems.Add(Path.GetFileNameWithoutExtension(e.c.FileName));
                    //v.SubItems.Add(Path.GetFileName(e.c.FileName));
                    //v.SubItems.Add(e.e.Region.BeginLine.ToString());
                    //v.SubItems.Add("project");

                    //v.Checked = true;

                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];
                    row.InheritedStyle.BackColor = Color.FromKnownColor(KnownColor.Control);

                    row.Cells[0].Value = "";
                    row.Cells[1].Value = ve_resource.Errors;
                    row.Cells[2].Value = "e.Code";
                    row.Cells[3].Value = e.e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                    row.Cells[5].Value = Path.GetFileName(e.file);
                    row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                    row.Cells[7].Value = "project";

                    //string message = e.e.Message;

                    //if (message.StartsWith("UnknownIdentifier") == true)
                    //{
                    //    // MessageBox.Show("Unknown identifier");

                    //    string[] cc = e.e.Message.Split(" ".ToCharArray());

                    //    Document d = ef.scr.FileOpened(e.c.FileName);

                    //    //foreach (Row r in d.Editor.Document)
                    //    //    foreach (Word w in r)
                    //    //        w.HasError = false;

                    //    if (d != null)
                    //    {
                    //        // MessageBox.Show("File has been found");

                    //        if (e.e.Region.BeginLine < 0)
                    //            continue;

                    //        d.SuspendLayout();

                    //        Row r = d.Editor.Document[e.e.Region.BeginLine];

                    //        foreach (Word w in r)
                    //        {
                    //            string[] bb = w.Text.Split(";".ToCharArray());

                    //            foreach (string dd in bb)

                    //                if (dd == cc[1])
                    //                {
                    //                    // MessageBox.Show("Identifier found");

                    //                    w.HasError = true;

                    //                    //d.Refresh();
                    //                }
                    //        }

                    //        d.ResumeLayout();
                    //    }

                    //}

                    row.Tag = e;
                }

                ef.scr.LoadErrors(DD);

                dg.ResumeLayout();
                */
            }
        }

        public ArrayList Me { get; set; }

        public void _LoadCompileResults(ArrayList es, ArrayList ws, ArrayList me)
        {
            Es = es;
            Ws = ws;
            Me = me;

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

        public void LoadCompileResults(ArrayList es, ArrayList ws, ArrayList me)
        {
            Es = es;
            Ws = ws;
            Me = me;

            SuspendLayout();

            W = new ArrayList();
            if (Es != null)
                foreach (Microsoft.Build.Framework.BuildErrorEventArgs e in Es)
                {
                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];

                    row.Cells[0].Value = "";
                    row.Cells[1].Value = ve_resource.Breakall_6323;
                    row.Cells[2].Value = e.Code;
                    row.Cells[3].Value = e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.ProjectFile);
                    row.Cells[5].Value = Path.GetFileName(e.File);
                    row.Cells[6].Value = e.LineNumber.ToString();
                    row.Cells[7].Value = "project";

                    //v.Checked = true;

                    row.Tag = e;

                    //lv.Items.Add(v);

                    W.Add(row);
                }
            if (Ws != null)
                foreach (Microsoft.Build.Framework.BuildWarningEventArgs e in Ws)
                {
                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];

                    //ListViewItem v = new ListViewItem();
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = ve_resource.Breakall_6323;
                    row.Cells[2].Value = e.Code;
                    row.Cells[3].Value = e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.ProjectFile);
                    row.Cells[5].Value = Path.GetFileName(e.File);
                    row.Cells[6].Value = e.LineNumber.ToString();
                    row.Cells[7].Value = "project";

                    //v.Checked = false;

                    row.Tag = e;

                    //lv.Items.Add(v);

                    W.Add(row);
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

        private object obs = new object();

        public void AddCompileResults(ArrayList es, ArrayList ws, ArrayList me)
        {
            lock (obs)
            {
                //Es = es;
                //Ws = ws;
                //Me = me;

                SuspendLayout();
                if (W == null)
                    W = new ArrayList();

                foreach (Microsoft.Build.Framework.BuildErrorEventArgs e in es)
                {
                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];

                    row.Cells[0].Value = "";
                    row.Cells[1].Value = ve_resource.Breakall_6323;
                    row.Cells[2].Value = e.Code;
                    row.Cells[3].Value = e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.ProjectFile);
                    row.Cells[5].Value = Path.GetFileName(e.File);
                    row.Cells[6].Value = e.LineNumber.ToString();
                    row.Cells[7].Value = "project";

                    //v.Checked = true;

                    row.Tag = e;

                    //lv.Items.Add(v);

                    W.Add(row);
                }

                foreach (Microsoft.Build.Framework.BuildWarningEventArgs e in ws)
                {
                    int rowId = dg.Rows.Add();
                    DataGridViewRow row = dg.Rows[rowId];

                    //ListViewItem v = new ListViewItem();
                    row.Cells[0].Value = "";
                    row.Cells[1].Value = ve_resource.Breakall_6323;
                    row.Cells[2].Value = e.Code;
                    row.Cells[3].Value = e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.ProjectFile);
                    row.Cells[5].Value = Path.GetFileName(e.File);
                    row.Cells[6].Value = e.LineNumber.ToString();
                    row.Cells[7].Value = "project";

                    //v.Checked = false;

                    row.Tag = e;

                    //lv.Items.Add(v);

                    W.Add(row);
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
        }

        private bool _showErrors = true;

        private bool _showWarnings = true;

        private bool _showMessages = true;

        private void Filter()
        {
            dg.Columns[0].Width = 30;
            dg.Columns[1].Width = 30;

            dg.Rows.Clear();

            int _ec = 0;
            int _Ec = 0;
            _Ec = Es.Count + Ep.Count;
            if (_showErrors == true)
            {
                foreach (DataGridViewRow r in Es)
                {
                    _ec++;
                    dg.Rows.Add(r);
                }
                foreach (DataGridViewRow r in Ep)
                {
                    _ec++;
                    dg.Rows.Add(r);
                }
                _errorsButton.Text = _Ec.ToString() + " Errors ";
            }
            else _errorsButton.Text = "0 of " + _Ec.ToString() + " Errors ";

            int _wc = 0;
            int _Wc = 0;
            _Wc = W.Count + Wp.Count;
            if (_showWarnings == true)
            {
                foreach (DataGridViewRow r in W)
                {
                    _wc++;
                    dg.Rows.Add(r);
                }
                foreach (DataGridViewRow r in Wp)
                {
                    //if (v.Checked == true)
                    //    lv.Items.Add(v);
                    _wc++;
                    dg.Rows.Add(r);
                }
                _warningsButton.Text = _Wc.ToString() + " Warnings";
            }
            else _warningsButton.Text = "0 of " + _Wc.ToString() + " Warnings";

            int _mc = 0;
            int _Mc = 0;
            _Mc = Me.Count + Mp.Count;
            if (_showMessages == true)
            {
                foreach (DataGridViewRow r in Me)
                {
                    _mc++;
                    dg.Rows.Add(r);
                }
                foreach (DataGridViewRow r in Mp)
                {
                    _mc++;
                    dg.Rows.Add(r);
                }
                _messagesButton.Text = _Mc.ToString() + " Messages";
            }
            else _messagesButton.Text = "0 of " + _Mc.ToString() + " Messages";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (_showErrors == true)
                _showErrors = false;
            else _showErrors = true;
            Filter();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (_showWarnings == true)
                _showWarnings = false;
            else _showWarnings = true;
            Filter();
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if ((e.Item.SubItems[1] == e.SubItem))
            {
                e.DrawDefault = false;
                e.DrawBackground();
                if (e.Item.Tag.GetType() == typeof(Microsoft.Build.Framework.BuildErrorEventArgs))
                {
                    e.Graphics.DrawImage(ve_resource.Breakall_6323, e.SubItem.Bounds.Location);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + ve_resource.Breakall_6323.Width), e.SubItem.Bounds.Location.Y);
                }
                else
                {
                    e.Graphics.DrawImage(ve_resource.View_16x, e.SubItem.Bounds.Location);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + ve_resource.Breakall_6323.Width), e.SubItem.Bounds.Location.Y);
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

                //ef.OpenFileLine(file, c.ToString(), p);

                AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                workerDisplayError wde = DisplayError;
                wde.BeginInvoke(file, c.ToString(), p, callBack, "null");
            }
        }

        public delegate void workerDisplayError(string file, string line, int c);

        private void DisplayError(string file, string line, int c)
        {
            ef.BeginInvoke(new Action(() => { ef.OpenFileLine(file, line, c); }));
        }

        private static void ProcessInformation(IAsyncResult result)
        {
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox b = toolStripComboBox2;

            int i = b.SelectedIndex;

            if (i < 0)
                return;

            LoadResults();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox b = toolStripComboBox1;

            LoadResults();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (_showMessages == true)
                _showMessages = false;
            else _showMessages = true;
            Filter();
        }
    }
}