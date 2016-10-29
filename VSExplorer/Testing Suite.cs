using System;
using System.Windows.Forms;

using utils;

namespace WinExplorer
{
    public partial class Testing_Suite : Form
    {
        public Testing_Suite()
        {
            InitializeComponent();

            ts = new testing();
        }

        public testing ts { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            string ns = textBox1.Text;

            int iters = 10;
            try
            {
                System.Convert.ToInt32(ns);
            }
            catch (Exception ee) { }

            ts.iters = iters;
        }
    }
}