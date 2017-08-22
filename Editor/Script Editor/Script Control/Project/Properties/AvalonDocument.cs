using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

using AvalonEdit.Host;

namespace AIMS.Libraries.Scripting.ScriptControl.Properties
{
    public class AvalonDocument : DockContent
    {

        public AvalonEdit.Host.Editor_WinformsHost Editor { get; set; }

        public string FileName { get; set; }

        public AvalonDocument(string FileName = "")
        {
           

            this.FileName = FileName;
            Editor = new AvalonEdit.Host.Editor_WinformsHost();
            Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(Editor);
            if(!string.IsNullOrEmpty(FileName))
            Editor.Load(FileName);
            Editor.Show();
            
        }
        public int GetLineExtended(int line)
        {
            return Editor.dv.GetLineExtended(line);
        }
        public void LoadText(string content)
        {
            Editor.dv.LoadContent(content);
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AvalonDocument
            // 
            this.ClientSize = new System.Drawing.Size(513, 400);
            this.Name = "AvalonDocument";
            this.ResumeLayout(false);

        }
    }
}
