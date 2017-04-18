using AIMS.Libraries.Scripting.ScriptControl;
using Microsoft.CSharp;
using Oracle.ManagedDataAccess.Client;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Resources.Tools;
using System.Windows.Forms;
using VSProvider;

namespace WinExplorer.UI
{
    public partial class DataSourceWizard : Form
    {
        public DataSourceWizard()
        {
            InitializeComponent();

            tabControl1.Visible = false;

            cbcs = comboBox1;

            db = treeView2;

            LoadTab(0);

            Init();

            Init_DatabaseModel();

            LoadConnections();

            db.BeforeExpand += Db_BeforeExpand;
        }

        private void Db_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes.Count == 1)
                if (node.Nodes[0].Text == "Oracle")
                {
                    DataRow d = node.Tag as DataRow;
                    if (d == null)
                        return;
                    node.Nodes.Clear();
                    GetColumnsOracle(node, node.Text);
                }
        }

        private ListView v { get; set; }

        private ImageList g { get; set; }

        private ComboBox cbcs { get; set; }

        private TreeView db { get; set; }

        public VSProject vp { get; set; }

        public void Init()
        {
            g = new ImageList();

            g.ImageSize = new Size(32, 32);

            g.Images.Add("database", Resources.Database_256x);
            g.Images.Add("data", Resources.DataSourceView_16x);

            v = listView1;

            v.LargeImageList = g;

            ListView listView = listView1;

            // Initialize the ListView control.
            listView.BackColor = Color.White;
            listView.ForeColor = Color.Black;
            listView.Dock = DockStyle.Fill;
            listView.View = View.LargeIcon;

            listView.Columns.Add("Name", "Name", 400);

            ListViewItem c = new ListViewItem("Connect to Database");
            c.ImageKey = "database";
            v.Items.Add(c);
            c = new ListViewItem("Connect to Service");
            c.ImageKey = "database";
            v.Items.Add(c);
            c = new ListViewItem("Connect to Object");
            c.ImageKey = "database";
            v.Items.Add(c);
            c = new ListViewItem("Connect to SharePoint service");
            c.ImageKey = "database";
            v.Items.Add(c);

            v.SelectedIndexChanged += V_SelectedIndexChanged;
        }

        public void Init_DatabaseModel()
        {
            ListView listView = listView2;

            ListView v = listView2;

            ImageList g = new ImageList();

            g.ImageSize = new Size(32, 32);

            g.Images.Add("database", Resources.Database_256x);
            g.Images.Add("web", Resources.ConnectWeb_16x);

            v.LargeImageList = g;

            // Initialize the ListView control.
            listView.BackColor = Color.White;
            listView.ForeColor = Color.Black;
            listView.Dock = DockStyle.Fill;
            listView.View = View.LargeIcon;

            listView.Columns.Add("Name", "Name", 400);

            ListViewItem c = new ListViewItem("Database Model");
            c.ImageKey = "database";
            v.Items.Add(c);

            db.CheckBoxes = true;

            ImageList gd = new ImageList();
            gd.Images.Add("database", Resources.Database_256x);
            gd.Images.Add("web", Resources.ConnectWeb_16x);
            db.ImageList = gd;
        }

        private void V_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (v.SelectedIndices == null || v.SelectedIndices.Count <= 0)
                return;

            int index = v.SelectedIndices[0];

            if (index >= texts.Length)
                return;

            richTextBox1.Text = texts[index];
        }

        private string[] texts = { "Lets you connect to a database and choose the database objects for your application.", "Opens the Add Service Reference dialog box that lets you create a connection to a service that returns the data for your application.", "Lets you choose objects that can later be used to generate data-bound controls.", "Lets you connect to a SharePoint site and choose the SharePoint objects for your application." };

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
        }

        public void LoadTab(int index)
        {
            tabPage1.Controls.Remove(panel1);
            panel9.Controls.Clear();
            if (index == 0)
            {
                ChoosePanel(panel1);
            }
            else if (index == 2)
            {
                ChoosePanel(panel5);
            }
            else if (index == 3)
            {
                ChoosePanel(panel10);
            }
            else if (index == 4)
            {
                ChoosePanel(panel14);
            }
            else if (index == 5)
            {
                ChoosePanel(panel18);
                LoadFromConnection();
            }
        }

        public void ChoosePanel(Panel panel)
        {
            panel9.Controls.Clear();
            panel9.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private string dataSourceType { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            if (v.SelectedIndices == null || v.SelectedIndices.Count <= 0)
                return;
            int index = v.SelectedIndices[0];

            button1.Enabled = true;

            if (dataSourceType == "database_connection")
            {
                wz = wizard.databaseobjects;
                dataSourceType = "tables";
                prev = 4;
                LoadTab(5);
            }
            else
            if (dataSourceType == "database")
            {
                prev = 3;
                dataSourceType = "database_connection";
                LoadTab(4);
            }
            else if (index == 0)
            {
                prev = 0;
                dataSourceType = "database";
                LoadTab(3);
            }
            else
            if (index == 2)
            {
                wz = wizard.dataobjects;
                dataSourceType = "objects";
                prev = 0;
                LoadTab(2);
                LoadAssemblies();
            }
        }

        private int prev = -1;

        private void button1_Click(object sender, EventArgs e)
        {
            if (prev == -1)
                return;
            dataSourceType = "";
            LoadTab(prev);
        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {
        }

        private ImageList img { get; set; }

        private void LoadAssemblies()
        {
            TreeView g = treeView1;

            g.AfterCheck += G_AfterCheck;

            g.CheckBoxes = true;

            img = new ImageList();
            img.Images.Add("class", Resources.Class_yellow_16x);
            img.Images.Add("namespace", Resources.Namespace_16x);
            img.Images.Add("string", Resources.String_16x);
            img.Images.Add("checked", Resources.CheckBox_16x);
            img.Images.Add("content", Resources.TextContentControl_16x);

            g.ImageList = img;

            ExplorerForms ef = ExplorerForms.ef;

            VSSolution vs = ef.GetVSSolution();

            if (vs == null)
            {
                MessageBox.Show("Open and build VS Solution first");
                return;
            }

            ArrayList L = vs.GetExecutables(VSSolution.OutputType.both);

            foreach (string s in L)
            {
                TreeNode node = new TreeNode(Path.GetFileName(s));

                node.ImageKey = "namespace";

                g.Nodes.Add(node);

                if (!File.Exists(s))
                    continue;

                VSProject vpp = vs.GetProjectbyExec(s);

                if (vp != null)
                    if (vp != vpp)
                        continue;

                try
                {
                    Assembly assembly = Assembly.LoadFrom(s);

                    var namespaces = assembly.GetTypes()
                             .Select(t => t.Namespace)
                             .Distinct();

                    foreach (string c in namespaces)
                    {
                        TreeNode nodes = new TreeNode(c);

                        nodes.ImageKey = "namespace";

                        node.Nodes.Add(nodes);

                        var types = assembly.GetTypes()
                             .Where(t => t.Namespace == c)
                             .Distinct();
                        foreach (Type b in types)
                        {
                            TreeNode ng = new TreeNode();
                            ng.Text = b.Name;
                            ng.ImageKey = "class";
                            DataSourceItem data = new DataSourceItem();
                            data.FileName = s;
                            data.TypeName = b.Name;
                            data.Namespace = b.Namespace;
                            data.FullName = b.FullName;
                            data.AssemblyQualifiedName = b.AssemblyQualifiedName;
                            data.vs = vs;
                            data.vp = vpp;
                            ng.Tag = data;

                            nodes.Nodes.Add(ng);

                            var bb = b.GetMembers()
                             .Where(t => t.MemberType == (MemberTypes.Property));

                            foreach (MemberInfo d in bb)
                            {
                                TreeNode bg = new TreeNode(" " + d.Name);
                                bg.ImageKey = "string";
                                bg.Tag = d;
                                ng.Nodes.Add(bg);

                                if ((d as PropertyInfo).PropertyType.IsEnum || (d as PropertyInfo).PropertyType == typeof(bool))
                                    bg.ImageKey = "checked";
                                else bg.ImageKey = "content";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        private void G_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node == null)
                return;
            TreeNode ng = null;
            if (node.Checked == true)
            {
                foreach (TreeNode ns in node.Nodes)
                {
                    ns.Checked = true;
                    ng = ns;

                    //while (ng.Parent != null)
                    //{
                    //    ng = ng.Parent;
                    //    ng.Checked = true;

                    //}
                }
            }
            else if (node.Checked == false)
            {
                foreach (TreeNode ns in node.Nodes)
                    ns.Checked = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadAssemblies();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ChooseDataSourceForm cdf = new ChooseDataSourceForm();
            DialogResult r = cdf.ShowDialog();
            if (r != DialogResult.OK)
                return;
            cbcs.Items.Add(cdf.shortcut);

            SaveConnection(cdf.shortcut);

            dataSourceType = "database_connection";
        }

        public void SaveConnection(string c)
        {
            if (File.Exists("Extensions//Databases//Connections//connections.settings") == false)
            {
                FileStream fs = File.Create("Extensions//Databases//Connections//connections.settings");
                fs.Close();
            }
            File.AppendAllText("Extensions//Databases//Connections//connections.settings", "\n" + c);
        }

        public void LoadConnections()
        {
            if (File.Exists("Extensions//Databases//Connections//connections.settings") == false)
            {
                FileStream fs = File.Create("Extensions//Databases//Connections//connections.settings");
                fs.Close();
            }
            string[] cs = File.ReadAllLines("Extensions//Databases//Connections//connections.settings");
            cbcs.Items.Clear();
            foreach (string c in cs)
                cbcs.Items.Add(c);
        }

        public static ArrayList GetConnections()
        {
            if (File.Exists("Extensions//Databases//Connections//connections.settings") == false)
            {
                FileStream fs = File.Create("Extensions//Databases//Connections//connections.settings");
                fs.Close();
            }
            string[] cs = File.ReadAllLines("Extensions//Databases//Connections//connections.settings");
            ArrayList L = new ArrayList();
            L.AddRange(cs.ToArray());
            return L;
        }

        public static List<string> GetTables(SqlConnection connection, string schemas, dataset data = null)
        {
            //using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema(schemas);
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[2].ToString());
                    //if (data != null)
                    //    data.ns.Add(row);
                }
                if (data != null)
                    data.SchemaTables = schema;

                connection.Close();

                return TableNames;
            }
        }

        public static bool CanConnect(string connectionString)
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static List<DataRow> GetTablesSchema(DbConnection connection, string schemas, dataset data = null)
        {
            //using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema(schemas);
                List<DataRow> TableNames = new List<DataRow>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row);

                    //  if (data != null)
                    //      data.ns.Add(row);
                }
                if (data != null)
                    data.SchemaTables = schema;

                connection.Close();

                return TableNames;
            }
        }

        public static DataTable GetTableSchema(SqlConnection con, string table)
        {
            DataTable schema = null;

            {
                con.Open();
                using (var schemaCommand = new SqlCommand("SELECT * FROM " + table, con))
                {
                    var reader = schemaCommand.ExecuteReader(CommandBehavior.SchemaOnly);
                    {
                        schema = reader.GetSchemaTable();

                        con.Close();

                        return schema;
                    }
                }
                con.Close();
            }
        }

        public void GetColumnsOracle(TreeNode node, string name)
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = data.connectionString;
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            DataTable b = PullData_Oracle(conn, name);

            foreach (DataColumn r in b.Columns)
            {
                string cs = r.ColumnName;

                TreeNode ng = new TreeNode(cs.ToString());
                node.Nodes.Add(ng);
            }
            node.Tag = b;
        }

        static public void GetColumnsOracle(TreeNode node, string name, string connectionString)
        {
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connectionString;
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            DataTable b = PullData_Oracle(conn, name);

            foreach (DataColumn r in b.Columns)
            {
                string cs = r.ColumnName;

                TreeNode ng = new TreeNode(cs.ToString());
                node.Nodes.Add(ng);
            }
            node.Tag = b;
        }

        private TextBox tb_dataset { get; set; }

        static public void LoadfromOracleConnection(TreeNode db, string connectionString)
        {
            OracleConnection conn = new OracleConnection();
            // conn.ConnectionString =
            // "Data Source=localhost;" +
            //// "Initial Catalog=DataBaseName;" +
            ////"Integrated Security=SSPI; +
            // "User id=sa;" +
            // "Password=sa;";
            conn.ConnectionString = connectionString;
            try
            {
                //conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            //data.connection = conn;

            List<DataRow> tables = GetTablesSchema(conn, "Tables"); AddConnectionForm_Oracle.GetTables(connectionString);

            db.Nodes.Clear();
            TreeNode node = new TreeNode("Tables");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[1].ToString();
                TreeNode nodes = new TreeNode(name);

                //DataTable b = PullData_Oracle(conn, name);// GetTableSchema(conn, name);

                //foreach (DataColumn r in b.Columns)
                //{
                //    string cs = r.ColumnName;

                //    TreeNode ng = new TreeNode(cs.ToString());
                //    nodes.Nodes.Add(ng);
                //}
                nodes.Tag = row;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
                TreeNode ns = new TreeNode();
                ns.Text = "Oracle";
                nodes.Nodes.Add(ns);
            }

            tables = GetTablesSchema(conn, "Views");
            node = new TreeNode("Views");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[1].ToString();
                TreeNode nodes = new TreeNode(name);

                //DataTable b = PullData_Oracle(conn, name);// GetTableSchema(conn, name);

                //foreach (DataColumn r in b.Columns)
                //{
                //    string cs = r.ColumnName;

                //    TreeNode ng = new TreeNode(cs.ToString());
                //    nodes.Nodes.Add(ng);
                //}
                nodes.Tag = row;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
                TreeNode ns = new TreeNode();
                ns.Text = "Oracle";
                nodes.Nodes.Add(ns);
            }

            //tables = GetTables(conn, "Procedures");
            //node = new TreeNode("Procedures");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            //tables = GetTables(conn, "UserDefinedTypes");
            //node = new TreeNode("UserDefinedTypes");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            //wz = wizard.databaseobjects;
        }

        public void LoadfromOracleConnection(TreeView db, string connectionString)
        {
            OracleConnection conn = new OracleConnection();
            // conn.ConnectionString =
            // "Data Source=localhost;" +
            //// "Initial Catalog=DataBaseName;" +
            ////"Integrated Security=SSPI; +
            // "User id=sa;" +
            // "Password=sa;";
            conn.ConnectionString = connectionString;
            try
            {
                //conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            //data.connection = conn;

            List<DataRow> tables = AddConnectionForm_Oracle.GetTables(connectionString);

            db.Nodes.Clear();
            TreeNode node = new TreeNode("Tables");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[1].ToString();
                TreeNode nodes = new TreeNode(name);

                //DataTable b = PullData_Oracle(conn, name);// GetTableSchema(conn, name);

                //foreach (DataColumn r in b.Columns)
                //{
                //    string cs = r.ColumnName;

                //    TreeNode ng = new TreeNode(cs.ToString());
                //    nodes.Nodes.Add(ng);
                //}
                nodes.Tag = row;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
                TreeNode ns = new TreeNode();
                ns.Text = "Oracle";
                nodes.Nodes.Add(ns);
            }

            tables = GetTablesSchema(conn, "Views");
            node = new TreeNode("Views");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[1].ToString();
                TreeNode nodes = new TreeNode(name);

                //DataTable b = PullData_Oracle(conn, name);// GetTableSchema(conn, name);

                //foreach (DataColumn r in b.Columns)
                //{
                //    string cs = r.ColumnName;

                //    TreeNode ng = new TreeNode(cs.ToString());
                //    nodes.Nodes.Add(ng);
                //}
                nodes.Tag = row;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
                TreeNode ns = new TreeNode();
                ns.Text = "Oracle";
                nodes.Nodes.Add(ns);
            }

            //tables = GetTables(conn, "Procedures");
            //node = new TreeNode("Procedures");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            //tables = GetTables(conn, "UserDefinedTypes");
            //node = new TreeNode("UserDefinedTypes");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            wz = wizard.databaseobjects;
        }

        public void LoadFromConnection()
        {
            data = new dataset();

            string c = cbcs.Text;

            string[] dd = c.Split("-".ToCharArray());

            string provider = dd[0].Trim();

            c = dd[1].Trim();

            string[] cc = c.Split(";".ToCharArray());

            textBox1.Text = cc[cc.Length - 2].Replace("Initial Catalog=", "");

            data.name = textBox1.Text;

            if (provider.ToLower() == "oracle")
            {
                DataConnection d = new DataConnection();
                d.Load(c);
                if (d.dict.ContainsKey("Data Source"))
                    data.name = d.dict["Data Source"];
                data.connectionString = c;
                LoadfromOracleConnection(db, c);
                return;
            }

            SqlConnection conn = new SqlConnection();
            // conn.ConnectionString =
            // "Data Source=localhost;" +
            //// "Initial Catalog=DataBaseName;" +
            ////"Integrated Security=SSPI; +
            // "User id=sa;" +
            // "Password=sa;";
            conn.ConnectionString = c;
            try
            {
                //conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            data.connection = conn;

            List<DataRow> tables = GetTablesSchema(conn, "Tables");

            db.Nodes.Clear();
            TreeNode node = new TreeNode("Tables");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[2].ToString();
                TreeNode nodes = new TreeNode(name);

                DataTable b = PullData(conn, name);// GetTableSchema(conn, name);

                foreach (DataColumn r in b.Columns)
                {
                    string cs = r.ColumnName;

                    TreeNode ng = new TreeNode(cs.ToString());
                    nodes.Nodes.Add(ng);
                }
                nodes.Tag = b;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
            }

            tables = GetTablesSchema(conn, "Views");
            node = new TreeNode("Views");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[2].ToString();
                TreeNode nodes = new TreeNode(name);

                DataTable b = PullData(conn, name);// GetTableSchema(conn, name);

                foreach (DataColumn r in b.Columns)
                {
                    string cs = r.ColumnName;

                    TreeNode ng = new TreeNode(cs.ToString());
                    nodes.Nodes.Add(ng);
                }
                nodes.Tag = b;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
            }

            //tables = GetTables(conn, "Procedures");
            //node = new TreeNode("Procedures");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            //tables = GetTables(conn, "UserDefinedTypes");
            //node = new TreeNode("UserDefinedTypes");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            wz = wizard.databaseobjects;
        }

        public static void LoadFromConnection(TreeNode db, DataConnection dc)
        {
            dataset data = new dataset();

            string c = dc.connectionString;

            string provider = dc.GetConnectionStringProvider();

            c = dc.GetConnectionStringr();

            if (provider == "Oracle")
                LoadfromOracleConnection(db, c);
            else LoadFromSQLServerConnection(db, dc);
        }

        public static void LoadFromSQLServerConnection(TreeNode db, DataConnection dc)
        {
            dataset data = new dataset();

            string c = dc.connectionString;

            string[] cc = c.Split(";".ToCharArray());

            SqlConnection conn = new SqlConnection();
            // conn.ConnectionString =
            // "Data Source=localhost;" +
            //// "Initial Catalog=DataBaseName;" +
            ////"Integrated Security=SSPI; +
            // "User id=sa;" +
            // "Password=sa;";
            conn.ConnectionString = dc.GetConnectionStringr();
            try
            {
                //conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            data.connection = conn;

            List<DataRow> tables = GetTablesSchema(conn, "Tables");

            db.Nodes.Clear();
            TreeNode node = new TreeNode("Tables");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[2].ToString();
                TreeNode nodes = new TreeNode(name);

                DataTable b = PullData(conn, name);// GetTableSchema(conn, name);

                foreach (DataColumn r in b.Columns)
                {
                    string cs = r.ColumnName;

                    TreeNode ng = new TreeNode(cs.ToString());
                    nodes.Nodes.Add(ng);
                }
                nodes.Tag = b;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
            }

            tables = GetTablesSchema(conn, "Views");
            node = new TreeNode("Views");
            db.Nodes.Add(node);
            foreach (DataRow row in tables)
            {
                string name = row[2].ToString();
                TreeNode nodes = new TreeNode(name);

                DataTable b = PullData(conn, name);// GetTableSchema(conn, name);

                foreach (DataColumn r in b.Columns)
                {
                    string cs = r.ColumnName;

                    TreeNode ng = new TreeNode(cs.ToString());
                    nodes.Nodes.Add(ng);
                }
                nodes.Tag = b;
                nodes.StateImageKey = "data";
                node.Nodes.Add(nodes);
            }

            //tables = GetTables(conn, "Procedures");
            //node = new TreeNode("Procedures");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}

            //tables = GetTables(conn, "UserDefinedTypes");
            //node = new TreeNode("UserDefinedTypes");
            //db.Nodes.Add(node);
            //foreach (string s in tables)
            //{
            //    TreeNode nodes = new TreeNode(s);
            //    node.Nodes.Add(nodes);
            //}
        }

        public void LoadDatabaseTables()
        {
        }

        public DataTable fillDataTable(string table)
        {
            string query = "SELECT * FROM dstut.dbo.[" + table + "]";

            using (SqlConnection sqlConn = new SqlConnection("conSTR"))
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                sqlConn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }

        public static void Designercs()
        {
            //CreateResXFile();

            StreamWriter sw = new StreamWriter(@".\DemoResources.cs");
            string[] errors = null;
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CodeCompileUnit code = StronglyTypedResourceBuilder.Create("schema.xsd", "DemoResources",
                                                                       "DemoApp", provider,
                                                                       false, out errors);
            if (errors.Length > 0)
                foreach (var error in errors)
                    Console.WriteLine(error);

            provider.GenerateCodeFromCompileUnit(code, sw, new CodeGeneratorOptions());
            sw.Close();
        }

        static public DataTable PullData(SqlConnection conn, string tableName)
        {
            //string connString = @"your connection string here";
            string query = "select * from " + tableName + " where 1 = 0";

            DataTable dataTable = new DataTable(tableName);

            //SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();
            return dataTable;
        }

        static public DataTable PullData_Oracle(OracleConnection conn, string tableName)
        {
            //string connString = @"your connection string here";
            string query = "select * from " + tableName + " where 1 = 0";

            DataTable dataTable = new DataTable(tableName);

            //SqlConnection conn = new SqlConnection(connString);
            OracleCommand cmd = new OracleCommand(query, conn);
            conn.Open();

            // create data adapter
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            try
            {
                da.Fill(dataTable);
            }
            catch (Exception ex) { }
            conn.Close();
            da.Dispose();
            return dataTable;
        }

        private static void CreateResXFile()
        {
            Bitmap logo = new Bitmap(@".\Logo.bmp");

            ResXResourceWriter rw = new ResXResourceWriter(@".\Demo.resx");
            rw.AddResource("Logo", logo);
            rw.AddResource("AppTitle", "Demo Application");
            rw.Generate();
            rw.Close();
        }

        public void GenerateXSD()
        {
            string xmlFilePath = @"myxmlfile.xml";
            string xsdOutputPath = @"myxmlfile.xsd";

            DataSet ds = new DataSet();

            System.IO.FileStream fsReadXml = new System.IO.FileStream(xmlFilePath, System.IO.FileMode.Open);
            ds.ReadXml(fsReadXml);

            ds.WriteXmlSchema(xsdOutputPath);
        }

        /// <summary>
        /// Compiles the input string and saves it in memory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="referencedAssemblies"></param>
        /// <returns></returns>
        public static CompilerResults LoadScriptsToMemory(string source, List<string> referencedAssemblies)
        {
            CompilerParameters parameters = new CompilerParameters();

            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = false;

            //Add the required assemblies
            foreach (string reference in referencedAssemblies)
                parameters.ReferencedAssemblies.Add(reference);

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.Location.Contains("Microsoft.Xna") || asm.Location.Contains("Gibbo.Library")
                    || asm.Location.Contains("System"))
                {
                    parameters.ReferencedAssemblies.Add(asm.Location);
                }
            }

            return Compile(parameters, source);
        }

        private static CompilerResults Compile(CompilerParameters parameters, string source)
        {
            CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");
            return compiler.CompileAssemblyFromSource(parameters, source);
        }

        public enum wizard
        {
            start,
            dataobjects,
            dataobjectsready,
            database,
            databaseobjects,
            databaseobjectsready
        }

        public wizard wz = wizard.start;

        private void LoadNodes(TreeNode node, ArrayList L)
        {
            foreach (TreeNode ns in node.Nodes)
            {
                if (ns.Checked == true)
                    L.Add(ns);
                else LoadNodes(ns, L);
            }
        }

        public dataset data { get; set; }

        private void LoadDatabaseNodes(TreeNode node, ArrayList L)
        {
            foreach (TreeNode ng in node.Nodes)
            {
                if (ng.Checked == true)
                    data.ns.Add((DataTable)ng.Tag);
                else LoadDatabaseNodes(ng, L);
            }
        }

        public ArrayList dataSourceObjects { get; set; }

        public void ImportDataObjects(ArrayList L)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ArrayList L = new ArrayList();

            if (wz == wizard.dataobjects)
            {
                // MessageBox.Show("Import data objects selected");

                TreeView g = treeView1;

                foreach (TreeNode ns in g.Nodes)
                {
                    LoadNodes(ns, L);
                }

                dataSourceObjects = L;

                wz = wizard.dataobjectsready;

                MessageBox.Show("Total number of selected nodes " + L.Count.ToString());

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else if (wz == wizard.databaseobjects)
            {
                MessageBox.Show("Import database objects selected");

                data.ns = new List<DataTable>();

                foreach (TreeNode ns in db.Nodes)
                {
                    LoadDatabaseNodes(ns, L);
                }
                wz = wizard.databaseobjectsready;

                MessageBox.Show("Total number of selected nodes " + L.Count.ToString());

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }
    }

    public class dataset
    {
        public DataTable SchemaTables { get; set; }

        public List<DataTable> ns { get; set; }

        public SqlConnection connection { get; set; }

        public string connectionString { get; set; }

        public string name { get; set; }

        public dataset()
        {
            ns = new List<DataTable>();
        }
    }

    public class TableInfo
    {
        public string name { get; set; }

        public string tableName { get; set; }

        public dataset data { get; set; }

        public DataSet datasets { get; set; }

        public DataTable GetDataTable()
        {
            Type T = Type.GetType("WinExplorer." + name);

            object obs = Activator.CreateInstance(T);

            DataSet d = obs as DataSet;

            DataTable table = d.Tables[tableName];

            return table;
        }

        public SqlDataAdapter GetDataTableAdapter()
        {
            Type T = Type.GetType(name);

            object obs = Activator.CreateInstance(T);

            DataSet d = obs as DataSet;

            DataTable table = d.Tables[tableName];

            //string adapter = name + "." + tableName + "TableAdapter";

            //T = Type.GetType(adapter);

            //obs = Activator.CreateInstance(T);

            //PropertyInfo []p = T.GetProperties().Where(s => s.Name == "Adapter").ToArray();

            SqlDataAdapter da = null;

            //if (p == null || p.Length >= 1)
            //    return da;

            //da = p[0].GetValue(obs) as SqlDataAdapter;

            //p = T.GetProperties().Where(s => s.Name == "Connection").ToArray();

            //p[0].SetValue(da, new SqlConnection(""));

            return da;
        }
    }

    public class DataConnection
    {
        public Dictionary<string, string> dict { get; set; }
        public string connectionString { get; set; }

        public void Load(string c)
        {
            connectionString = c;
            Parse();
        }

        public void Parse()
        {
            dict = new Dictionary<string, string>();
            string[] bb = connectionString.Split(";".ToCharArray());
            foreach (string b in bb)
            {
                if (String.IsNullOrEmpty(b))
                    continue;
                string[] cc = b.Split("=".ToCharArray());
                if (cc.Length <= 1)
                    continue;
                string key = cc[0].Trim();
                string value = cc[1].Trim();
                if (dict.ContainsKey(key))
                    continue;
                dict.Add(key, value);
            }
        }

        public void SetKeyValue(string key, string value)
        {
            dict[key] = value;
        }

        public string GetConnectionString()
        {
            string c = "";
            foreach (string key in dict.Keys)
                c += key + "=" + dict[key] + ";";
            string[] cc = c.Split("-".ToCharArray());

            return cc[1].Trim();
        }

        public string GetConnectionStringProvider()
        {
            string[] cc = connectionString.Split("-".ToCharArray());

            return cc[0].Trim();
        }

        public string GetConnectionStringr()
        {
            string[] cc = connectionString.Split("-".ToCharArray());

            return cc[1].Trim();
        }
    }
}