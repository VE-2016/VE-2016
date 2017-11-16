using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Xml;
using System.Xml.Linq;

namespace WinExplorer.UI
{
    public partial class SyntaxTreeForm : Form
    {
        public SyntaxTreeForm()
        {
            InitializeComponent();
            viewer = new WpfGraph.VirtualGraph();
            zoomButton = toolStripButton4;
            zoomButton.Click += ZoomButton_Click;
            layoutComboBox = toolStripComboBox1;
          
            this.SuspendLayout();
            
            this.AutoScroll = true;
            ElementHost ctrlHost = new ElementHost();
            ctrlHost.Dock = DockStyle.Fill;
            this.Controls.Add(ctrlHost);

            ctrlHost.Child = viewer;
            
           
            this.ResumeLayout();

        }

        ToolStripComboBox layoutComboBox { get; set; }


        private void ZoomButton_Click(object sender, EventArgs e)
        {
            if (viewer == null)
                return;
            //viewer.Zoom(1.0);
        }

        ToolStripButton zoomButton { get; set; }

        public WpfGraph.VirtualGraph viewer { get; set; }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
        public void OpenGraphFromXElement(XElement xe)
        {
            MemoryStream me = GenerateStreamFromString(xe.ToString());

            this.BeginInvoke(new Action(() =>
            {

                XmlReader xmlReader = XmlReader.Create(me, new XmlReaderSettings());
                //viewer.LoadGraph(DgmlParser.DgmlParser.Parse(xmlReader));
                File.WriteAllText("xe.dgml", xe.ToString());

            }));
            
        }
    }
    
}
