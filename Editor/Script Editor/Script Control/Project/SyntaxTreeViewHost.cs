using AvalonEdit.Editor;
using Microsoft.CodeAnalysis;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using VSProvider;

namespace AvalonEdit.Host
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    [DesignerSerializer("System.ComponentModel.Design.Serialization.TypeCodeDomSerializer , System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
    public class SyntaxTreeViewHost : System.Windows.Forms.Integration.ElementHost
    {
        public SyntaxTreeViewer dv;

        public SyntaxTreeViewHost(ScriptControl.ScriptControl scr = null)
        {
            this.BackColor = System.Drawing.Color.White;

            dv = new SyntaxTreeViewer();

            this.BackColorTransparent = true;

            base.Child = dv;

            if (scr != null)
                scr.handler += Scr_handler;
        }

        private void Scr_handler(object sender, ScriptControl.Properties.AvalonDocument e)
        {
            if (this.Visible == false)
                return;
            Load(e.FileName, e.vs);
        }

        private Size size = new System.Windows.Size();

        private void Editor_WinformsHost_Resize(object sender, EventArgs e)
        {
            if (dv == null)
                return;
            if (dv.Width != this.ClientSize.Width || dv.Height != this.ClientSize.Height)
            {
                dv.Width = this.ClientSize.Width;
                dv.Height = this.ClientSize.Height;
            }
        }

        public void Load(string filename, VSSolution vs)
        {
            if (vs == null)
                return;
            SyntaxTree syntaxTree = vs.GetSyntaxTree(filename);
            dv.LoadSyntaxTree(syntaxTree);
        }
    }
}