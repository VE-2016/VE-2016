using AIMS.Libraries.Scripting.ScriptControl;

//using AIMS.Libraries.Forms.Docking;

using DockProject;
using System;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WinExplorer
{
    public partial class DockForm : Form
    {
        public DockForm()
        {
            InitializeComponent();

            SuspendLayout();

            dock = new WeifenLuo.WinFormsUI.Docking.DockPanel();

            dock.Dock = DockStyle.Fill;

            dock.DocumentStyle = DocumentStyle.DockingWindow;

            dock.SupportDeeplyNestedContent = false;

            //dock.AllowEndUserDocking = false;

            //dock.AllowEndUserNestedDocking = false;

            dock.Theme = new VS2013BlueTheme();

            this.Controls.Add(dock);

            scr = new ScriptControl(this);

            scr.Dock = DockStyle.Fill;

            ResumeLayout();

            LoadTW("Explorer");
            LoadTW("Output");
            LoadTW("Folders");
            LoadTW("Recent");

            LoadDocumentWindow();
        }

        private DockPanel dock { get; set; }

        public ScriptControl scr { get; set; }

        public ToolWindow tw { get; set; }

        public void LoadDocumentWindow()
        {
            tw = new ToolWindow();
            tw.Controls.Add(scr);
            tw.FormBorderStyle = FormBorderStyle.None;
            //tw.ControlBox = false;
            tw.Text = String.Empty;
            tw.TopLevel = false;
            //((DockContent)tw).AllowEndUserDocking = false;
            //tw.DockAreas = DockAreas.DockTop;

            tw.TabText = "Documents";

            tw.Show(dock, DockState.DockTop);
        }

        public ToolWindow ww { get; set; }

        public void LoadTW(string name)
        {
            ww = new ToolWindow();
            ww.Text = name;
            //tw.Controls.Add(scr);
            ww.FormBorderStyle = FormBorderStyle.None;
            //ww.ControlBox = false;
            ww.TabText = name;
            ww.TopLevel = false;
            //ww.DockAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom;

            ww.Show(dock);
        }
    }
}