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
using System.Windows.Threading;


using System.Collections.ObjectModel;
using System.IO;
using AvalonEdit.Editor;

namespace AvalonEdit.Editor
{
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
    [DesignerSerializer("System.ComponentModel.Design.Serialization.TypeCodeDomSerializer , System.Design", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
    public class Editor_WinformsHost : System.Windows.Forms.Integration.ElementHost
    {
        public EditorWindow dv;// = new EditorWindow();

        

        public Editor_WinformsHost()
        {


            dv = Activator.CreateInstance(typeof(EditorWindow)) as EditorWindow;
            base.Child = dv;
            dv.Width = 800;
            dv.Height = 800;
            this.Dock = DockStyle.Fill;
            this.Size = new System.Drawing.Size(800, 800);
            
        }

        
        public void Load(string file)
        {
            this.Show();
            dv.LoadFile(file);

        }
    }
}
