using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DockingTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            dockPanel1.DocumentStyle = AIMS.Libraries.Forms.Docking.DocumentStyles.DockingWindow;
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MyDocument doc = new MyDocument();
            doc.Text = "No:XX";
            doc.Show(dockPanel1, AIMS.Libraries.Forms.Docking.DockState.DockLeft);
            
            for (int i = 0; i < 10; i++)
            {

                MyDocument doc2 = new MyDocument();
                doc2.Text = "No:" + i.ToString();
                doc2.Show(dockPanel1, AIMS.Libraries.Forms.Docking.DockState.Document);
            }

            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            dockPanel1.SaveAsXml(@"c:\\testdock.xml");
        }
    }
}