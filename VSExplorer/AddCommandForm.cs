using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class AddCommandForm : Form
    {
        public AddCommandForm()
        {
            InitializeComponent();
            v = listView1;
            v.Resize += V_Resize;
            lb = listBox1;
            InitializeListView();

            CommandList();

            int w = v.Width;

            v.Columns[0].Width = w;
        }

        private void V_Resize(object sender, System.EventArgs e)
        {
            int w = v.Width;

            v.Columns[0].Width = w;
        }

        private ArrayList C { get; set; }

        private ArrayList CS { get; set; }

        private ListBox lb { get; set; }

        private ListView v { get; set; }

        public void CommandList()
        {
            Dictionary<string, Command> d = gui.Init();

            foreach (string s in d.Keys)
            {
                ListViewItem b = new ListViewItem();
                b.Text = s;
                v.Items.Add(b);
            }

            CS = gui.GetCategories();

            foreach (string s in CS)
            {
                lb.Items.Add(s);
            }
        }

        private ImageList img { get; set; }

        public void InitializeListView()
        {
            SuspendLayout();

            img = new ImageList();

            v.Clear();

            v.View = View.Details;

            v.LargeImageList = img;

            v.FullRowSelect = true;

            v.Columns.Add("");

            v.HeaderStyle = ColumnHeaderStyle.None;

            ResumeLayout();
        }

        public string command { get; set; }

        private void button1_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            if (listView1.SelectedIndices == null || v.SelectedIndices.Count <= 0)
            {
                Close();
                return;
            }

            int i = v.SelectedIndices[0];

            ListViewItem w = v.Items[i];

            string c = w.SubItems[0].Text;

            command = c;

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}