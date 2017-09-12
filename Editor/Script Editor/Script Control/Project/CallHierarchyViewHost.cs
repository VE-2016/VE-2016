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
    public class CallHierarchyViewer_WinformsHost : System.Windows.Forms.Integration.ElementHost
    {
        public CallHierarchyViewer dv;
        
        public CallHierarchyViewer_WinformsHost()
        {

            this.BackColor = System.Drawing.Color.White;
            
            dv = new CallHierarchyViewer();

            this.BackColorTransparent = true;

            base.Child = dv;
           
        }
        
       
        public void Load(string file)
        {
            this.Show();
            
        }
    }
}
