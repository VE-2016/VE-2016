using ScriptControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using VSProvider;
using WinExplorer.UI.Views;

namespace WinExplorer.UI
{
    public partial class DataSourceForm : Form
    {
        public DataSourceForm()
        {
            InitializeComponent();

            ShowInfoPanel();

            g = treeView1;

            tbContext = contextMenuStrip1;

            Init();

            ExplorerForms.ef.event_SelectedProjectChanged += Ef_SelectedProjectChanged;

            g.MouseClick += G_MouseClick;

            this.Resize += DataSourceForm_Resize;
        }

        private void DataSourceForm_Resize(object sender, EventArgs e)
        {
        }

        private void G_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode node = g.SelectedNode;

            tbContext.Tag = node;

            //if(Control.MouseButtons == MouseButtons.Right)
            tbContext.Show(g, e.Location);
        }

        private ContextMenuStrip tbContext { get; set; }

        private ImageList img { get; set; }

        public void Init()
        {
            img = new ImageList();
            img.Images.Add("database", ve_resource.Database_256x);
            img.Images.Add("data", ve_resource.DataSourceView_16x);
            img.Images.Add("class", ve_resource.Class_yellow_16x);
            img.Images.Add("namespace", ve_resource.Namespace_16x);
            img.Images.Add("string", ve_resource.String_16x);
            img.Images.Add("checked", ve_resource.CheckBox_16x);
            img.Images.Add("content", ve_resource.TextContentControl_16x);
            g.ImageList = img;
        }

        private Dictionary<VSProject, ArrayList> dataSource { get; set; }

        private VSProject vp { get; set; }

        public VSProject GetVSProject()
        {
            VSProject p = ExplorerForms.ef.GetVSProject();

            if (p != null)
            {
                vp = p;
                return vp;
            }
            if (p == null)
                p = ExplorerForms.ef.GetVSSolution().MainVSProject;
            vp = p;
            return vp;
        }

        private void Ef_SelectedProjectChanged(object sender, VSProvider.VSProject vp)
        {
            if (dataSource == null)
                dataSource = ExplorerForms.ef.Command_Get_DataSources();

            this.vp = vp;

            if (vp == null)
                return;

            if (dataSource.ContainsKey(vp))
            {
                P = dataSource[vp];
            }
            else
            {
                dataSource.Add(vp, new ArrayList());
                P = dataSource[vp];
            }

            this.BeginInvoke((new Action(() => { LoadDataObjects(P); })));
        }

        private DataSourceWizard dw { get; set; }

        private TreeView g { get; set; }

        public ArrayList P { get; set; }

        private void ImportDataObjects(ArrayList L, bool append = false)
        {
            if (append == false)
                P = L;

            //ShowInfoPanel(true);

            //MessageBox.Show("objects - " + L.Count);

            g.Nodes.Clear();

            foreach (TreeNode ns in L)
            {
                TreeNode ng = new TreeNode(ns.Text);
                g.Nodes.Add(ng);
                DataSourceItem d = ns.Tag as DataSourceItem;
                if (d != null)
                {
                    ng.Text = d.Namespace + " - " + d.TypeName;

                    UpdateObjects(d);

                    P.Add(d);
                }
            }
        }

        private void UpdateObjects(DataSourceItem d)
        {
            string folder = vp.GetProjectFolder() + "\\" + "Properties\\DataSources";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string file = d.FullName + ".datasource";

            string b = AppDomain.CurrentDomain.BaseDirectory;

            string c = b + "Templates\\datasource";

            string e = c + "\\" + "d.datasource";

            string cc = File.ReadAllText(e);

            cc = cc.Replace("<...>", d.TypeName);

            cc = cc.Replace("<....>", d.AssemblyQualifiedName);

            FileStream fs = File.Create(folder + "\\" + file, 10000, FileOptions.None);

            fs.Close();

            File.WriteAllText(folder + "\\" + file, cc);

            vp.AddDataSource(file);
        }

        private void LoadDataset(DataSet data, TreeNode ns, string file)
        {
            foreach (DataTable row in data.Tables)
            {
                TreeNode ng = new TreeNode(row.TableName);
                ng.ImageKey = "database";
                ns.Nodes.Add(ng);
                string c = Path.GetFileNameWithoutExtension(file);

                TableInfo info = new TableInfo();
                info.datasets = data;
                info.name = c;
                info.tableName = row.TableName;
                ns.Tag = info;

                {
                    DataTable b = row;

                    foreach (DataColumn r in b.Columns)
                    {
                        string cs = r.ColumnName;

                        TreeNode nodes = new TreeNode(cs.ToString());
                        nodes.ImageKey = "data";
                        ng.Nodes.Add(nodes);
                        ng.Tag = info;
                    }
                    //TreeNode ns = new TreeNode(cs.ToString());
                    //ng.Nodes.Add(ns);
                }
            }
        }

        private void ImportDatabaseObjects(dataset data)
        {
            //if (append == false)
            //    P = L;

            //ShowInfoPanel(true);

            //MessageBox.Show("objects - " + L.Count);

            g.Nodes.Clear();

            datasets = new DataSet();

            foreach (DataTable row in data.ns)
            {
                TreeNode ng = new TreeNode(row.TableName);
                g.Nodes.Add(ng);
                //foreach (DataColumn cs in row.Table.Columns)
                {
                    DataTable b = row;// DataSourceWizard.GetTableSchema(data.connection, row[2].ToString());

                    datasets.Tables.Add(b);

                    foreach (DataColumn r in b.Columns)
                    {
                        string cs = r.ColumnName;
                        TreeNode nodes = new TreeNode(cs.ToString());
                        ng.Nodes.Add(nodes);
                    }
                }
            }
            string bb = AppDomain.CurrentDomain.BaseDirectory;

            vp = ExplorerForms.ef.GetVSProject();

            string s = vp.GetProjectFolder();

            string d = s + "\\" + data.name + ".xsd";

            datasets.WriteXmlSchema(d);

            ExplorerForms.ef.Command_TakeTypedDataset(d);

            vp.AddXSDItem(data.name);
        }

        private DataSet datasets { get; set; }

        private void LoadDataSet(string file, TreeView g)
        {
            string folder = vp.GetProjectFolder();
            TreeNode ng = new TreeNode(file);
            g.Nodes.Add(ng);
            datasets = new DataSet();
            datasets.ReadXmlSchema(folder + "\\" + file);
            LoadDataset(datasets, ng, file);
        }

        public TreeView GetXSDTreeNodes()
        {
            TreeView v = new TreeView();
            foreach (string s in P)
            {
                if (s.EndsWith(".xsd"))
                {
                    LoadDataSet(s, v);
                }
            }
            return v;
        }

        private void LoadDataObjects(ArrayList L, bool append = false)
        {
            if (append == false)
                P = L;

            //ShowInfoPanel(true);

            //MessageBox.Show("objects - " + L.Count);

            g.Nodes.Clear();

            string folder = vp.GetProjectFolder();

            foreach (string s in L)
            {
                if (s.EndsWith(".xsd"))
                {
                    LoadDataSet(s, g);
                    continue;
                }
                string fullname = "";
                string name = "";
                XmlDocument xd = new XmlDocument();
                xd.Load(folder + "\\" + s);
                XmlNode node = xd.DocumentElement.ChildNodes[0];

                name = xd.DocumentElement.Attributes[0].Value;

                //foreach (XmlNode node in nodelist) // for each <testcase> node
                {
                    try
                    {
                        fullname = node.InnerText;

                        Type T = Type.GetType(fullname);
                        TreeNode ng = new TreeNode(name);
                        ng.ImageKey = "class";
                        g.Nodes.Add(ng);

                        var bb = T.GetMembers();
                        foreach (MemberInfo m in bb)
                        {
                            TreeNode ns = new TreeNode();
                            ns.Text = m.Name;
                            ng.Nodes.Add(ns);
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Error in reading XML", "xmlError", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            GetVSProject();
            dw = new DataSourceWizard();
            dw.vp = vp;

            DialogResult r = dw.ShowDialog();
            if (r != DialogResult.OK)
                return;
            if (dw.wz == DataSourceWizard.wizard.dataobjectsready)
            {
                ImportDataObjects(dw.dataSourceObjects, true);
            }
            else if (dw.wz == DataSourceWizard.wizard.databaseobjectsready)
            {
                ImportDatabaseObjects(dw.data);
            }
        }

        public void SaveConnection(string c)
        {
            if (File.Exists("Extensions//Databases//Connections//datasource.settings") == false)
            {
                FileStream fs = File.Create("Extensions//Databases//Connections//datasource.settings");
                fs.Close();
            }
            File.AppendAllText("Extensions//Databases//Connections//datasource.settings", "\n" + c);
        }

        public void LoadConnections()
        {
            if (File.Exists("Extensions//Databases//Connections//datasource.settings") == false)
            {
                FileStream fs = File.Create("Extensions//Databases//Connections//datasource.settings");
                fs.Close();
            }
            string[] cs = File.ReadAllLines("Extensions//Databases//Connections//datasource.settings");

            //cbcs.Items.Clear();
            //foreach (string c in cs)
            //    if (c != "")
            //        cbcs.Items.Add(c);
            //if (cbcs.Items.Count > 0)
            //    cbcs.SelectedIndex = 0;
        }

        private Label label { get; set; }

        private Button button { get; set; }

        public void ShowInfoPanel(bool hide = false)
        {
            if (!hide)
            {
                label = new Label();
                label.Dock = DockStyle.Fill;

                this.Controls.Add(label);

                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Text = "Data Source component";

                button = new Button();
                button.Text = "Add new data source";
                button.BackColor = Color.Transparent;
                button.Dock = DockStyle.Bottom;
                button.FlatStyle = FlatStyle.Flat;
                //button.FlatAppearance.BorderColor = Color.Transparent;
                button.FlatAppearance.BorderSize = 0;

                this.Controls.Add(button);
            }
            else
            {
                this.Controls.Remove(label);

                this.Controls.Remove(button);
            }
        }

        private void previewDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = g.SelectedNode;
            TableInfo info = node.Tag as TableInfo;
            TreeView v = GetXSDTreeNodes();
            v.Tag = info;
            DataPreviewForm dpf = new DataPreviewForm();
            dpf.LoadData(v);
            dpf.ShowDialog();
        }

        private void editDatasetWithDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = g.SelectedNode;
            if (node == null)
                return;
            TableInfo info = node.Tag as TableInfo;
            if (info == null)
                return;
            DataSet datasets = info.datasets;
            if (datasets == null)
                return;

            DocumentForm df = ExplorerForms.ef.Command_OpenDocumentForm("Designer");
            DesignerForm dvf = new DesignerForm();
            dvf.FormBorderStyle = FormBorderStyle.None;
            dvf.TopLevel = false;
            dvf.Dock = DockStyle.Fill;
            dvf.LoadModel(datasets);
            dvf.Show();
            df.Controls.Add(dvf);
        }
    }
}