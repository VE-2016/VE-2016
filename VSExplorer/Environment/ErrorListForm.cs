using AIMS.Libraries.CodeEditor;
//using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.ScriptControl;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
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
            Me = new ArrayList();
            _warningsButton = toolStripButton3;
            _errorsButton = toolStripButton2;
            InitializeListView();
            InitializeDataGrid();
            LoadSettings();
            CodeEditorControl.IntErrors.ContentChanged += IntErrors_ContentChanged;
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox2.SelectedIndex = 0;
            ResumeLayout();
        }

       

        private void IntErrors_ContentChanged(object sender, EventArgs e)
        {
            Intellisense ie = CodeEditorControl.IntErrors;

            ArrayList E = ie.errors;

            DD = E;

            lv.Items.Clear();

            LoadResults();

            //MessageBox.Show(E.Count + " errors have been found");
        }

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
            if (CodeEditorControl.settings != null)
            {
                Settings s = CodeEditorControl.settings;

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

        private DataGridView dg { get; set; }

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
            Image image = Resources.Errors;
            img.Image = image;
            dg.Columns.Add(img);
            dg.Columns.Add("Code", "Code");
            dg.Columns.Add("Description", "Description");
            dg.Columns.Add("Project", "Project");
            dg.Columns.Add("File", "File");
            dg.Columns.Add("Line", "Line");
            dg.Columns.Add("Suppression", "Suppression");

            dg.CellMouseEnter += Dg_CellMouseEnter;

            dg.CellMouseLeave += Dg_CellMouseLeave;

            dg.SelectionChanged += Dg_SelectionChanged;

            dg.CellValueNeeded += Dg_CellValueNeeded;

            int i = 0;
            foreach (DataGridViewColumn column in dg.Columns)
            {
                if (i < 2)
                    column.Width = 25;
                if (i < 3)
                    column.Width = 45;
                else if (i < 4)
                    column.Width = 500;
                else break;

                i++;
            }

            //dg.Resize += Dg_Resize;

            ResumeLayout();

            //foreach (string c in b)
            //{
            //    rowId = dg.Rows.Add();
            //    row = dg.Rows[rowId];
            //    row.Cells[0].Value = "-";
            //    row.Cells[1].Value = c;
            //}
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
                wde.BeginInvoke(file, c.ToString(), p.ToString(), 100, callBack, "null");

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
                wde.BeginInvoke(file, c.ToString(), p.ToString(), 100, callBack, "null");

                //ef.OpenFileLine(file, c.ToString(), p);
            }
            else if (v.Tag.GetType() == typeof(IntError))
            {
                IntError b = v.Tag as IntError;

                string project = b.vp.FileName;
                string file = b.c.FileName;
                int p = b.e.Region.BeginLine;
                int c = b.e.Region.BeginColumn;
                int es = b.e.Region.EndLine;
                int ep = b.e.Region.BeginColumn;

                AsyncCallback callBack = new AsyncCallback(ProcessInformation);
                workerDisplayLine wde = DisplayLine;
                wde.BeginInvoke(file, c.ToString(), p.ToString(), 100, callBack, "null");

                //ef.OpenFileLine(file, c.ToString(), p);
            }
        }


    

   


    public delegate void workerDisplayLine(string file, string line, string c, int g);


    private void DisplayLine(string file, string line, string c, int g)
    {
        ef.BeginInvoke(new Action(() => { ef.OpenFileXY(file, line, c, g); }));
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

        public void LoadResults()
        {
            ToolStripComboBox b = toolStripComboBox2;

            //lv.Items.Clear();

            dg.Rows.Clear();

            if (b.SelectedIndex == 0 || b.SelectedIndex == 2)
            {
                LoadIntellisenseResults();
            }
            if (b.SelectedIndex == 0 || b.SelectedIndex == 1)
            {
                LoadCompileResults(Es, Ws, Me);
            }

            dg.Refresh();
        }

        public void _LoadIntellisenseResults()
        {
            ScriptControl scr = null;

            VSSolution vs = null;

            string filename = "";

            if (ef != null)
                if (ef.scr != null)
                    scr = ef.scr;

            Document d = scr.GetActiveDocument();

            filename = d.FileName;

            ToolStripComboBox b = toolStripComboBox1;

            int i = b.SelectedIndex;

            VSProject vp = ef.GetVSSolution().MainVSProject;

            if (vp != null)
                if (i == 2)
                {
                    foreach (IntError e in DD)
                    {
                        if (vp != null)
                            if (e.vp != vp)
                                continue;

                        ListViewItem v = new ListViewItem();
                        v.Text = "";

                        int rowId = dg.Rows.Add();
                        DataGridViewRow row = dg.Rows[rowId];

                        row.Cells[0].Value = "";
                        row.Cells[0].Value = "e.Code";
                        row.Cells[0].Value = e.e.Message;
                        row.Cells[0].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                        row.Cells[0].Value = Path.GetFileName(e.c.FileName);
                        row.Cells[0].Value = e.e.Region.BeginLine.ToString();
                        row.Cells[0].Value = "project";

                        v.Checked = true;

                        v.Tag = e;

                        lv.Items.Add(v);
                    }

                    return;
                }

            if (i == 3)
            {
                foreach (IntError e in DD)
                {
                    if (filename != "")
                        if (e.c.FileName != filename)
                            continue;

                    ListViewItem v = new ListViewItem();
                    v.Text = "";
                    v.SubItems.Add("");
                    v.SubItems.Add("e.Code");
                    v.SubItems.Add(e.e.Message);
                    v.SubItems.Add(Path.GetFileNameWithoutExtension(e.vp.FileName));
                    v.SubItems.Add(Path.GetFileName(e.c.FileName));
                    v.SubItems.Add(e.e.Region.BeginLine.ToString());
                    v.SubItems.Add("project");

                    v.Checked = true;

                    v.Tag = e;

                    lv.Items.Add(v);
                }

                return;
            }

            foreach (IntError e in DD)
            {
                ListViewItem v = new ListViewItem();
                v.Text = "";
                v.SubItems.Add("");
                v.SubItems.Add("e.Code");
                v.SubItems.Add(e.e.Message);
                v.SubItems.Add(Path.GetFileNameWithoutExtension(e.c.FileName));
                v.SubItems.Add(Path.GetFileName(e.c.FileName));
                v.SubItems.Add(e.e.Region.BeginLine.ToString());
                v.SubItems.Add("project");

                v.Checked = true;

                v.Tag = e;

                lv.Items.Add(v);
            }
        }

        public ArrayList LI { get; set; }

        private static object s_obs = new object();

        public void LoadIntellisenseResults()
        {
            lock (s_obs)
            {
                if (ef == null)
                    return;

                ScriptControl scr = null;

                VSSolution vs = null;

                string filename = "";

                if (ef != null)
                    if (ef.scr != null)
                        scr = ef.scr;

                ToolStripComboBox b = toolStripComboBox1;

                int i = b.SelectedIndex;

                vs = ef.GetVSSolution();

                VSProject vp = null;
                if (vs != null)
                    vp = vs.MainVSProject;

                dg.SuspendLayout();

                LI = new ArrayList();

                if (vp != null)
                    if (i == 2)
                    {
                        foreach (IntError e in DD)
                        {
                            if (vp != null)
                                if (e.vp != vp)
                                    continue;

                            int rowId = dg.Rows.Add();
                            DataGridViewRow row = dg.Rows[rowId];



                            row.Cells[0].Value = "";
                            row.Cells[1].Value = Resources.Errors;
                            row.Cells[2].Value = "e.Code";
                            row.Cells[3].Value = e.e.Message;
                            row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                            row.Cells[5].Value = Path.GetFileName(e.c.FileName);
                            row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                            row.Cells[7].Value = "project";

                            string message = e.e.Message;

                            if (message.StartsWith("UnknownIdentifier") == true)
                                MessageBox.Show("Unknown identifier");

                            //v.Checked = true;

                            row.Tag = e;

                            //lv.Items.Add(v);
                        }
                        dg.ResumeLayout();
                        return;
                    }

                if (i == 3)
                {
                    Document d = scr.GetActiveDocument();

                    filename = d.FileName;

                    foreach (IntError e in DD)
                    {
                        if (filename != "")
                            if (e.c.FileName != filename)
                                continue;

                        int rowId = dg.Rows.Add();
                        DataGridViewRow row = dg.Rows[rowId];
                        row.InheritedStyle.BackColor = Color.FromKnownColor(KnownColor.Control);

                        row.Cells[0].Value = "";
                        row.Cells[1].Value = Resources.Errors;
                        row.Cells[2].Value = "e.Code";
                        row.Cells[3].Value = e.e.Message;
                        row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                        row.Cells[5].Value = Path.GetFileName(e.c.FileName);
                        row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                        row.Cells[7].Value = "project";

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

                        row.Tag = e;

                        //lv.Items.Add(v);
                    }
                    dg.ResumeLayout();
                    return;
                }

                ArrayList F = ef.scr.GetOpenFiles();

                foreach (IntError e in DD)
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
                    row.Cells[1].Value = Resources.Errors;
                    row.Cells[2].Value = "e.Code";
                    row.Cells[3].Value = e.e.Message;
                    row.Cells[4].Value = Path.GetFileNameWithoutExtension(e.vp.FileName);
                    row.Cells[5].Value = Path.GetFileName(e.c.FileName);
                    row.Cells[6].Value = e.e.Region.BeginLine.ToString();
                    row.Cells[7].Value = "project";

                    string message = e.e.Message;

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
            if(Es != null)
            foreach (Microsoft.Build.Framework.BuildErrorEventArgs e in Es)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];

                row.Cells[0].Value = "";
                row.Cells[1].Value = Resources.Errors;
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
            if(Ws != null)
            foreach (Microsoft.Build.Framework.BuildWarningEventArgs e in Ws)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];

                //ListViewItem v = new ListViewItem();
                row.Cells[0].Value = "";
                row.Cells[1].Value = Resources.warnings;
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
        public void AddCompileResults(ArrayList es, ArrayList ws, ArrayList me)
        {
            //Es = es;
            //Ws = ws;
            //Me = me;

            SuspendLayout();
            if(W == null)
            W = new ArrayList();

            foreach (Microsoft.Build.Framework.BuildErrorEventArgs e in es)
            {
                int rowId = dg.Rows.Add();
                DataGridViewRow row = dg.Rows[rowId];

                row.Cells[0].Value = "";
                row.Cells[1].Value = Resources.Errors;
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
                row.Cells[1].Value = Resources.warnings;
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

        private bool _showErrors = true;

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //if (lv == null)
            //   return;

            if (Es == null)
                return;

            // lv.Items.Clear();

            dg.Rows.Clear();

            if (_showErrors == true)
                _showErrors = false;
            else
            {
                foreach (DataGridViewRow r in Es)
                {
                    //if (v.Checked == true)
                    //    lv.Items.Add(v);

                    dg.Rows.Add(r);
                }
                _showErrors = true;
            }

            if (dg.Rows.Count > 0)
                _errorsButton.Text = "Errors " + dg.Rows.Count.ToString();
            else
                _errorsButton.Text = "Errors";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (W == null)
                return;

            if (lv == null)
                return;

            //lv.Items.Clear();

            dg.Rows.Clear();

            //foreach (ListViewItem v in W)
            //{
            //    if (v.Checked == false)
            //        lv.Items.Add(v);

            //}

            foreach (DataGridViewRow r in W)
            {
                dg.Rows.Add(r);
            }

            if (dg.Rows.Count > 0)
                _warningsButton.Text = "Warnings " + dg.Rows.Count.ToString();
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
                    e.Graphics.DrawImage(Resources.ViewWarning_8934_24, e.SubItem.Bounds.Location);
                    e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + Resources.ViewWarning_8934_24.Width), e.SubItem.Bounds.Location.Y);
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
    }
}