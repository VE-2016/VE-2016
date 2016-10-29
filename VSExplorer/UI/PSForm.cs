using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class PSForm : Form
    {
        public PSForm()
        {
            InitializeComponent();

            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

            controls = consoleControls1;

            this.FormClosing += PSForm_FormClosing;

            StartPS();
        }

        private void PSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            controls.SendCommand("exit" + "\n");
        }

        private ConsoleControls controls { get; set; }

        private PSListenerConsoleSample c { get; set; }

        public void StartPS()
        {
            starter s = start;

            s.BeginInvoke(new AsyncCallback(Process), "null");
        }

        public delegate void starter();

        public void start()
        {
            c = new PSListenerConsoleSample(controls);

            c.start("get-command > a.txt");
        }

        public void Process(IAsyncResult ar)
        {
        }

        private ArrayList C { get; set; }

        public void LoadCommands()
        {
            if (File.Exists("a.txt") == false)
                return;

            C = new ArrayList();

            string[] cc = File.ReadAllLines("a.txt");
            int i = 0;
            while (i < cc.Length)
            {
                if (cc[i].StartsWith("--"))
                    break;
                i++;
            }
            i++;
            while (i < cc.Length)
            {
                string[] d = cc[i].Trim().Split("\r".ToCharArray());

                if (d.Length >= 1)

                    if (d[0] != "")
                        C.Add(d[0]);
                i++;
            }
            LoadList(C, controls.lb);
        }
        public void LoadList(ArrayList C, ListBox lb)
        {
            lb.Items.Clear();
            foreach (string s in C)
                lb.Items.Add(s);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            LoadCommands();
        }
    }
}
