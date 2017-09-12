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
            if (!string.IsNullOrEmpty(FileName))
                Editor.Load(FileName);
            else Editor.Show();
            Editor.Show();
            //this.Deactivate += AvalonDocument_Deactivate;
            this.Resize += AvalonDocument_Resize;
        }

        public void OnActivated(bool activated)
        {
            Editor.dv.Activated(activated);
        }

        private void AvalonDocument_Resize(object sender, EventArgs e)
        {
            Editor.Size = this.Size;
            if (Editor.dv == null)
                return;
           // if (dv.Width != this.ClientSize.Width || dv.Height != this.ClientSize.Height)
            {
                Editor.dv.Width = this.ClientSize.Width;
                Editor.dv.Height = this.ClientSize.Height;
            }
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
