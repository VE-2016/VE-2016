using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows;
using System.ComponentModel.Design.Serialization;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using System.Windows.Threading;


using System.Collections.ObjectModel;
using System.IO;
using AvalonEdit.Editor;

namespace AvalonEdit.Host
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    [DesignerSerializer("System.ComponentModel.Design.Serialization.TypeCodeDomSerializer , System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
    public class Editor_WinformsHost : System.Windows.Forms.Integration.ElementHost
    {
        public EditorWindow dv;// = new EditorWindow();



        public Editor_WinformsHost()
        {

            this.BackColor = System.Drawing.Color.White;

            string s = AppDomain.CurrentDomain.BaseDirectory + "AvalonEdit.Editor.dll";

            dv = EditorWindow.LoadXaml(s) as EditorWindow;

            dv.EditorWindows();//// dv = d[0] as EditorWindow;

            // dv = Activator.CreateInstance(typeof(EditorWindow)) as EditorWindow;
            base.Child = dv;
            // dv.Width = 800;
            //dv.Height = 800;
            //dv.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //dv.VerticalAlignment = VerticalAlignment.Stretch;
            //this.Dock = DockStyle.Fill;
            //this.Size = new System.Drawing.Size(800, 800);
            //this.Resize += Editor_WinformsHost_Resize;
        }
        Size size = new System.Windows.Size();
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
