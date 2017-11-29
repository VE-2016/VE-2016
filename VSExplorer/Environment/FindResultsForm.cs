using ScriptControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class FindResultsForm : Form
    {
        public FindResultsForm()
        {
            InitializeComponent();

            rb = richTextBoxEx1;

            rb.HideSelection = false;

            rb.MouseClick += Rb_MouseClick;

            outputs = new Dictionary<string, ArrayList>();

            c = toolStripComboBox1;
        }

        public ExplorerForms ef { get; set; }

        public ToolStripComboBox c { get; set; }

        public Dictionary<string, ArrayList> outputs { get; set; }

        public ArrayList AddOrUpdateOutput(string output)
        {
            if (outputs.ContainsKey(output) == false)
            {
                c.Items.Add(output);
                c.SelectedItem = output;
                ArrayList R = new ArrayList();
                outputs.Add(output, R);
                return R;
            }
            else
            {
                c.SelectedItem = output;
                return outputs[output];
            }
        }

        public void AddOrUpdateOutputs(string output)
        {
            this.BeginInvoke(new Action(() => { C = AddOrUpdateOutput(output); }));
        }

        private ArrayList C { get; set; }

        public void SetOutput(string output)
        {
            C = AddOrUpdateOutput(output);
        }

        public void AddOutput(string s)
        {
            if (C == null)
                SetOutput("undefined");

            C.Add(s);

            rb.AppendText(s);
        }

        private void Rb_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.rb.Lines.Count() <= 0)
                return;

            this.rb.WordWrap = false;
            int cursorPosition = this.rb.SelectionStart;
            int lineIndex = this.rb.GetLineFromCharIndex(cursorPosition);
            string lineText = this.rb.Lines[lineIndex];
            //this.rb.WordWrap = true;

            //MessageBox.Show("Selected line " + lineText);

            HighlightLine(lineIndex, Color.Blue);
        }

        private int _elines = 0;
        private int _elength = 0;

        public string text { get; set; }

        public void HighlightLine(int index, Color color)
        {
            var lines = rb.Lines;
            if (index < 0 || index >= lines.Length)
                return;
            var start = rb.GetFirstCharIndexFromLine(index);  // Get the 1st char index of the appended text
            var length = lines[index].Length;
            rb.Select(start, length);                 // Select from there to the end
            //rb.SelectionBackColor = color;
            _elines = start;
            _elength = length;

            if (ef == null)
                return;

            string texts = lines[index];

            string[] d = texts.Split("\t".ToCharArray());

            if (d.Length < 3)
                return;

            // ef.OpenFileXY(d[0], d[1], d[2], text.Length);

            AsyncCallback callBack = new AsyncCallback(ProcessInformation);
            workerDisplayLine wde = DisplayLine;
            wde.BeginInvoke(d[0], 1, 2, text.Length, callBack, "null");
        }

        public delegate void workerDisplayLine(string file, int line, int c, int g);

        private void DisplayLine(string file, int line, int c, int g)
        {
            ef.BeginInvoke(new Action(() => { ef.OpenFileXY(file, c, line/*, c*/, g); }));
        }

        private static void ProcessInformation(IAsyncResult result)
        {
        }

        private RichTextBox rb { get; set; }

        public ArrayList results { get; set; }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
        }

        public void ClearLog()
        {
            results = null;

            rb.Text = "";
        }

        public void LogLine(string s)
        {
            AddOutput(s + "\n");

            //rb.AppendText(s + "\n");

            rb.Select(0, 0);
        }

        private Document document { get; set; }

        public Document OpenDocuments(string text)
        {
            this.Controls.Remove(rb);

            // if (FileOpened(Name) != null)
            //     return null;

            //string contents = "";// Parser.ProjectParser.GetFileContents(Name);

            try
            {
                //   contents = File.ReadAllText(Name);
            }
            catch (Exception e) { };

            //if (contents == string.Empty)
            //    return null;
            Document doc = new Document(true);
            doc.FileName = Name;
            doc.Text = Path.GetFileNameWithoutExtension(Name);
            doc.Tag = "USERDOCUMENT";
            doc.HideOnClose = false;
            doc.FormBorderStyle = FormBorderStyle.None;
            doc.Dock = DockStyle.Fill;
            // doc.BackColor = SystemColors.InactiveBorder;
            //doc.Contents = text;
            //doc.ScriptLanguage = _scriptLanguage;
            //doc.vp = pp;
            //doc.LoadVSProject(pp);
            //Language lg = doc.lgs;// syntaxDocument1.Parser.Language;

            //doc.dc = dc;

            //doc.bmp = bmp;

            //if (doc.bmp != null)
            //{
            //    doc.bmp.MakeTransparent(Color.White);
            //    System.IntPtr icH = doc.bmp.GetHicon();
            //    Icon ico = Icon.FromHandle(icH);
            //    doc.bmp.Dispose();
            //    doc.Icon = ico;
            //}

            doc.TopLevel = false;

            //DocumentEvents(doc, true);
            //doc.Show(dockContainer1, DockState.Document);
            //doc.Contents = contents;

            this.Controls.Add(doc);

            doc.Show();

            //doc.ParseContentsNow();

            doc.Refresh();

            document = doc;

            return doc;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            rb.Clear();
        }
    }
}