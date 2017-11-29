//using System.Windows.Threading;

using AvalonEdit.Editor;
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows;

namespace AvalonEdit.Host
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    [DesignerSerializer("System.ComponentModel.Design.Serialization.TypeCodeDomSerializer , System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
    public class Editor_WinformsHost : System.Windows.Forms.Integration.ElementHost
    {
        public EditorWindow dv;

        public Editor_WinformsHost()
        {
            this.BackColor = System.Drawing.Color.White;

            string s = AppDomain.CurrentDomain.BaseDirectory + "AvalonEdit.Editor.dll";

            dv = new EditorWindow();

            this.BackColorTransparent = true;

            base.Child = dv;
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

        public void Load(string file)
        {
            this.Show();
            dv.LoadFile(file);
        }
    }
}