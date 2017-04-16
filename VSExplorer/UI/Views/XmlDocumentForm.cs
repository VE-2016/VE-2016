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
using System.Xml;

namespace WinExplorer.UI.Views
{
    public partial class XmlDocumentForm : Form
    {
        public XmlDocumentForm()
        {
            InitializeComponent();
            v = treeView1;
            ExplorerForms.ef.event_SelectedProjectItemChanged += Ef_event_SelectedProjectItemChanged;
        }
        TreeView v { get; set; }
        private void Ef_event_SelectedProjectItemChanged(object sender, VSProvider.VSProject vp, string file)
        {
            if (String.IsNullOrEmpty(file))
                return;
            string ext = Path.GetExtension(file);
            if(ext != ".xml" && ext != ".xsd")
            {
                return;
            }
            if (!File.Exists(file))
                return;

            this.BeginInvoke(new Action(() => { LoadXmlDocument(file); }));

        }

        public void LoadXmlDocument(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            v.Nodes.Clear();

            foreach (XmlNode node in doc.ChildNodes)
            {
                TreeNode ns = new TreeNode();
                ns.Text = node.Name;
                v.Nodes.Add(ns);
                LoadChildNodes(ns, node);
            }
        }

        void LoadChildNodes(TreeNode node, XmlNode xml)
        {
            foreach(XmlNode ng in xml.ChildNodes)
            {
                TreeNode ns = new TreeNode();
                ns.Text = ng.Name;
                node.Nodes.Add(ns);
                LoadChildNodes(ns, ng);
            }
        }
        
    }
}
