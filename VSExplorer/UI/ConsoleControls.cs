using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WinExplorer
{
    public partial class ConsoleControls : UserControl
    {
        public ConsoleControls()
        {
            InitializeComponent();

            rb = richTextBox1;

            lb = listBox1;

            lb.DoubleClick += Lb_DoubleClick;

            lb.BackColor = Color.FromKnownColor(KnownColor.Control);

            rb.KeyDown += Rb_KeyDown;

            nav = new NavigatorExt();
        }

        private void Lb_DoubleClick(object sender, EventArgs e)
        {
            int i = lb.SelectedIndex;
            if (i < 0)
                return;
            string d = lb.Items[i].ToString();
            string[] cc = d.Split(" ".ToCharArray());

            ArrayList L = new ArrayList();
            foreach (string s in cc)
                if (s != " ")
                    if (s != "")
                        L.Add(s);

            if (L.Count > 1)
            {
                MessageBox.Show("Loading " + L[1] as string);

                SendCommand(L[1] as string);
            }
        }

        public ListBox lb { get; set; }

        public void SendCommand(string c)
        {
            WriteLine("\n\n");

            WritePrompt();
            Write(c);
            string[] cc = rb.Text.Split("\n".ToCharArray());
            input = cc[cc.Length - 1];
            Task.Delay(100);
            EnterPressed = true;
        }

        public NavigatorExt nav { get; set; }

        private void changeLine(RichTextBox RTB, int line, string text)
        {
            int s1 = RTB.GetFirstCharIndexFromLine(line);
            int s2 = line < RTB.Lines.Count() - 1 ?
                      RTB.GetFirstCharIndexFromLine(line + 1) - 1 :
                      RTB.Text.Length;
            RTB.Select(s1, s2 - s1);
            RTB.SelectedText = text;
            rb.Select(rb.Text.Length, 0);
            rb.ScrollToCaret();
        }

        private void Rb_KeyDown(object sender, KeyEventArgs e)
        {
            if ((rb.SelectionStart <= inputStart) && e.KeyCode == Keys.Back) e.SuppressKeyPress = true;


            if (rb.SelectionStart < inputStart)
            {
                //  Allow arrows and Ctrl-C.
                if (!(e.KeyCode == Keys.Left ||
                    e.KeyCode == Keys.Right ||
                    e.KeyCode == Keys.Up ||
                    e.KeyCode == Keys.Down ||
                    (e.KeyCode == Keys.C && e.Control)))
                {
                    e.SuppressKeyPress = true;
                }
            }

            if (e.KeyCode == Keys.Tab)
            {
            }
            if (e.KeyCode == Keys.Up)
            {
                string c = nav.Prev();

                if (c != "")
                {
                    changeLine(rb, rb.Lines.Length - 1, ":\\>" + c);
                    input = ":\\>" + c;
                    rb.Refresh();
                    e.SuppressKeyPress = true;
                }
            }

            //  Is it the return key?
            if (e.KeyCode == Keys.Return)
            {
                string[] cc = rb.Text.Split("\n".ToCharArray());
                input = cc[cc.Length - 1];
                EnterPressed = true;
            }
        }

        private ArrayList _C = new ArrayList();

        public void CreateCommandList()
        {
            _C = new ArrayList();
        }

        public RichTextBox rb { get; set; }

        public string lastInput { get; set; }

        public int inputStart = -1;

        public bool EnterPressed = false;

        public string input = "";

        public void WritePrompt(KnownColor color = KnownColor.DarkGreen)
        {
            Invoke((Action)(() =>
            {
                if (rb.Text.EndsWith(":\\>") == true)
                    return;
                string[] cc = rb.Text.Split("\n".ToCharArray());

                if (cc[cc.Length - 1].Contains(":\\>") == true)
                    return;
                //  Write the output.
                rb.SelectionColor = Color.FromKnownColor(color);
                //rb.SelectedText += ":\\>";
                rb.AppendText(":\\>");
                inputStart = rb.SelectionStart;
                rb.Select(rb.Text.Length, 0);
                rb.ScrollToCaret();
                inputStart = rb.SelectionStart;
                rb.Focus();
            }));
        }

        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void Write(string output, KnownColor color = KnownColor.DarkGreen, bool prompt = false)
        {
            if (string.IsNullOrEmpty(lastInput) == false &&
                (output == lastInput || output.Replace("\r\n", "") == lastInput))
                return;

            Invoke((Action)(() =>
            {
                //  Write the output.
                rb.SelectionColor = Color.FromKnownColor(color);

                rb.AppendText(output);
                inputStart = rb.SelectionStart;
                rb.Focus();
            }));
        }


        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void WriteLine(string output, KnownColor color = KnownColor.DarkGreen, bool prompt = false)
        {
            if (string.IsNullOrEmpty(lastInput) == false &&
                (output == lastInput || output.Replace("\r\n", "") == lastInput))
                return;

            Invoke((Action)(() =>
            {
                rb.SelectionColor = Color.FromKnownColor(color);
                rb.AppendText(output + "\n");
                inputStart = rb.SelectionStart;
            }));
        }
        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public void WriteLine(char output, KnownColor color = KnownColor.Cyan)
        {
            Invoke((Action)(() =>
            {
                //  Write the output.
                rb.SelectionColor = Color.FromKnownColor(color);
                rb.SelectedText += output;
                rb.AppendText(Char.ToString(output));
                inputStart = rb.SelectionStart;
            }));
        }
        public string ReadLine()
        {
            starter s = start;
            s.Invoke();
            string c = input.Replace(":\\>", "");
            nav.Add(c);
            return c;
        }
        public delegate void starter();

        public void start()
        {
            while (true)
            {
                if (EnterPressed == true)
                {
                    EnterPressed = false;
                    return;
                }
                Task.Delay(400);
            }
        }
        public void Process(IAsyncResult ar)
        {
            EnterPressed = false;
        }
    }
}
