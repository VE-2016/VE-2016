using AvalonEdit.Editor;
using System;
using System.Threading.Tasks;
using VSProvider;
using WeifenLuo.WinFormsUI.Docking;

namespace ScriptControl.Properties
{
    public class AvalonDocument : DockContent
    {
        public AvalonEdit.Host.Editor_WinformsHost Editor { get; set; }

        public string FileName { get; set; }

        public VSSolution vs { get; set; }

        public AvalonDocument(VSSolution vs, string FileName = "")
        {
            this.FileName = FileName;

            this.vs = vs;
            Editor = new AvalonEdit.Host.Editor_WinformsHost();
            Editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Controls.Add(Editor);
            if (!string.IsNullOrEmpty(FileName))
                Editor.Load(FileName);
            else Editor.Show();
            Editor.Show();
            this.Resize += AvalonDocument_Resize;
        }

        public void OnActivated(bool activated)
        {
            if (Editor.dv != null)
            {
                var r = Editor.dv.Activated(activated);
                if (r.Status == TaskStatus.Faulted)
                    Editor.dv.referenced = false;
            }
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

        public string GetSelection()
        {
            string selectedText = "";
            selectedText = Editor.dv.GetSelectedText();
            return selectedText;
        }

        public void Save()
        {
            Editor.dv.Save();
        }

        public void StartSearchResults(Searcher searcher)
        {
            Editor.dv.StartSearchResults(searcher);
        }
        object obs = new object();
        ~AvalonDocument()
        {
            lock (obs)
            {
                if (!Editor.IsDisposed)
                    Editor.Dispose();
            }
        }
    }
}