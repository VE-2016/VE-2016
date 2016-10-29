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
    public partial class Output : DockableWindow
    {
        public Output()
        {
            InitializeComponent();
        }

        public void AppendLine(string msg)
        {
            txtOutput.Text += msg + Environment.NewLine;
        }
    }
}