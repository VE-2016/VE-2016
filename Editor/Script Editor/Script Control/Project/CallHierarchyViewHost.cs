//using System.Windows.Threading;

using AvalonEdit.Editor;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

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