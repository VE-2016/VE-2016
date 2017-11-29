#region License

// Copyright 2005-2009 Paul Kohler (https://github.com/paul-kohler-au/minisqlquery). All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (Ms-PL)
// http://minisqlquery.codeplex.com/license

#endregion License

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;
using Microsoft.Win32;
using MiniSqlQuery.Core.DbModel;
using ScriptControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//using Microsoft.SqlServer.Management.Smo..Wmi;

namespace WinExplorer.UI
{
    /// <summary>The database inspector form.</summary>
    public partial class DatabaseInspectorForm : Form
    {
        /// <summary>The root tag.</summary>
        private static readonly object RootTag = new object();

        /// <summary>The tables tag.</summary>
        private static readonly object TablesTag = new object();

        /// <summary>The views tag.</summary>
        private static readonly object ViewsTag = new object();

        /// <summary>The _meta data service.</summary>
        private IDatabaseSchemaService _metaDataService;

        /// <summary>The _model.</summary>
        private DbModelInstance _model;

        private List<string> _models;

        /// <summary>The _populated.</summary>
        private bool _populated;

        /// <summary>The _right clicked model object.</summary>
        private IDbModelNamedObject _rightClickedModelObject;

        /// <summary>The _right clicked node.</summary>
        private TreeNode _rightClickedNode;

        /// <summary>The _sql writer.</summary>
        private ISqlWriter _sqlWriter;

        /// <summary>The _tables node.</summary>
        private TreeNode _tablesNode;

        /// <summary>The _views node.</summary>
        private TreeNode _viewsNode;

        /// <summary>Initializes a new instance of the <see cref="DatabaseInspectorForm"/> class.</summary>
        /// <param name="services">The services.</param>
        /// <param name="hostWindow">The host window.</param>
        public DatabaseInspectorForm()
        {
            InitializeComponent();

            v = DatabaseTreeView;

            ts = toolStrip1;
            ts.GripStyle = ToolStripGripStyle.Hidden;
            tbContext = contextMenuStrip1;
            CreateImageList();
            DatabaseTreeView.Nodes.Clear();
            //TreeNode root = CreateRootNodes();
            //root.Nodes.Add("Loading problem - check connection details and reset...");
            //DatabaseTreeView.Nodes.Add(root);
            // DatabaseTreeView.ExpandAll();

            sql = SQLServerInstances.LocalSqlServerInstancesgSqlWmi(ProviderArchitecture.Use64bit);

            foreach (ServerInstance s in sql)
            {
                TreeNode node = new TreeNode();
                node.Text = s.Name;
                //node.Tag = s;
                node.ImageKey = "server";
                node.SelectedImageKey = "server";
                v.Nodes.Add(node);
                ServerConnection c = new ServerConnection("http:\\localhost", "sa", "sa");
                Server server = new Server(c);
                node.Tag = server;
                this.server = server;
            }
        }

        private List<ServerInstance> sql { get; set; }

        private Server server { get; set; }

        private TreeView v { get; set; }

        private ImageList g { get; set; }

        private ToolStrip ts { get; set; }

        private ContextMenuStrip tbContext { get; set; }

        public void CreateImageList()
        {
            g = new ImageList();
            g.Images.Add("folder", ve_resource.FolderOpen_16x);
            g.Images.Add("column", ve_resource.Column_16x);
            g.Images.Add("database", ve_resource.Database_16x);
            g.Images.Add("table", ve_resource.Table_16x);
            g.Images.Add("server", ve_resource.ServerSettings_16x);
            g.Images.Add("PK", ve_resource.Key_32x);
            g.Images.Add("FK", ve_resource.ForeignKey_32x);

            v.ImageList = g;
        }

        /// <summary>Gets ColumnMenu.</summary>
        public ContextMenuStrip ColumnMenu
        {
            get { return ColumnNameContextMenuStrip; }
        }

        /// <summary>Gets DbSchema.</summary>
        public DbModelInstance DbSchema
        {
            get { return _model; }
        }

        /// <summary>Gets RightClickedModelObject.</summary>
        public IDbModelNamedObject RightClickedModelObject
        {
            get { return _rightClickedModelObject; }
        }

        /// <summary>Gets RightClickedTableName.</summary>
        public string RightClickedTableName
        {
            get
            {
                if (_rightClickedNode == null)
                {
                    return null;
                }

                return _rightClickedNode.Text;
            }
        }

        /// <summary>Gets TableMenu.</summary>
        public ContextMenuStrip TableMenu
        {
            get { return TableNodeContextMenuStrip; }
        }

        /// <summary>The load database details.</summary>
        public void LoadDatabaseDetails(DataConnection dc)
        {
            ExecLoadDatabaseDetails(dc);
        }

        /// <summary>The navigate to.</summary>
        /// <param name="modelObject">The model object.</param>
        public void NavigateTo(IDbModelNamedObject modelObject)
        {
            if (modelObject == null)
            {
                return;
            }

            // todo - ensure expanded

            switch (modelObject.ObjectType)
            {
                case ObjectTypes.Table:
                    foreach (TreeNode treeNode in _tablesNode.Nodes)
                    {
                        IDbModelNamedObject obj = treeNode.Tag as IDbModelNamedObject;
                        if (obj != null && modelObject == obj)
                        {
                            SelectNode(treeNode);
                        }
                    }

                    break;

                case ObjectTypes.View:
                    foreach (TreeNode treeNode in _viewsNode.Nodes)
                    {
                        IDbModelNamedObject obj = treeNode.Tag as IDbModelNamedObject;
                        if (obj != null && modelObject == obj)
                        {
                            SelectNode(treeNode);
                        }
                    }

                    break;

                case ObjectTypes.Column:
                    DbModelColumn modelColumn = modelObject as DbModelColumn;
                    if (modelColumn != null)
                    {
                        foreach (TreeNode treeNode in _tablesNode.Nodes)
                        {
                            // only look in the tables nodw for FK refs
                            DbModelTable modelTable = treeNode.Tag as DbModelTable;
                            if (modelTable != null && modelTable == modelColumn.ParentTable)
                            {
                                // now find the column in the child nodes
                                foreach (TreeNode columnNode in treeNode.Nodes)
                                {
                                    DbModelColumn modelReferingColumn = columnNode.Tag as DbModelColumn;
                                    if (modelReferingColumn != null && modelReferingColumn == modelColumn)
                                    {
                                        SelectNode(columnNode);
                                    }
                                }
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>The build image key.</summary>
        /// <param name="column">The column.</param>
        /// <returns>The build image key.</returns>
        private string BuildImageKey(DbModelColumn column)
        {
            string imageKey = column.ObjectType;
            if (column.IsRowVersion)
            {
                imageKey += "-RowVersion";
            }
            else
            {
                if (column.IsKey)
                {
                    imageKey += "-PK";
                }

                if (column.ForeignKeyReference != null)
                {
                    imageKey += "-FK";
                }
            }

            return imageKey;
        }

        /// <summary>The build tree from db model.</summary>
        /// <param name="connection">The connection.</param>
        private void BuildTreeFromDbModel(string connection)
        {
            //DatabaseTreeView.Nodes.Clear();
            TreeNode root = CreateRootNodes();
            root.ToolTipText = connection;

            //if (_model.Tables != null)
            //{
            //    foreach (DbModelTable table in _model.Tables)
            //    {
            //        CreateTreeNodes(table);
            //    }
            //}

            //if (_model.Views != null)
            //{
            //    foreach (DbModelView view in _model.Views)
            //    {
            //        CreateTreeNodes(view);
            //    }
            //}

            int i = 0;
            foreach (string d in _models)
            {
                TreeNode node = new TreeNode(d);
                node.Tag = "database";
                node.ImageKey = "database";
                if (i < 4)
                    SysDatabases.Nodes.Add(node);
                else
                    Databases.Nodes.Add(node);

                i++;

                // node.Nodes.Add(new TreeNode("Empty"));
            }

            DatabaseTreeView.Nodes.Add(root);
            DatabaseTreeView.ExpandAll();
        }

        public void BuildTreeFromDbModel(DbModelInstance model, TreeNode nodes)
        {
            nodes.Tag = model;

            if (model.Tables != null)
            {
                TreeNode ng = null;

                foreach (TreeNode b in nodes.Nodes)
                    if (b.Text == "Tables")
                        ng = b;
                TreeNode node = new TreeNode();
                node.Text = "System Tables";
                node.SelectedImageKey = "folder";
                ng.Nodes.Add(node);
                node = new TreeNode();
                node.Text = "File Tables";
                node.SelectedImageKey = "folder";
                ng.Nodes.Add(node);
                node = new TreeNode();
                node.Text = "External Tables";
                node.SelectedImageKey = "folder";
                ng.Nodes.Add(node);

                //nodes.Nodes.Add(ng);

                foreach (DbModelTable table in model.Tables)
                {
                    CreateTreeNodes(table, ng);
                }
            }

            if (model.Views != null)
            {
                TreeNode ng = null;

                foreach (TreeNode b in nodes.Nodes)
                    if (b.Text == "Views")
                        ng = b;
                TreeNode views = null;
                foreach (TreeNode b in ng.Nodes)
                    if (b.Text == "System Views")
                        views = b;

                //nodes.Nodes.Add(ng);

                foreach (DbModelView view in model.Views)
                {
                    CreateTreeNodes_View(view, ng);
                }
                foreach (string v in model.vw)
                    views.Nodes.Add(new TreeNode(v));
            }
            TreeNode ns = null;
            foreach (TreeNode b in nodes.Nodes)
                if (b.Text == "Programmability")
                    ns = b;
            TreeNode bg = null;
            foreach (TreeNode b in ns.Nodes)
                if (b.Text == "Stored Procedures")
                    bg = b;

            CreateTreeNodes_StoredProcedures(bg);
        }

        private TreeNode SysDatabases { get; set; }

        private TreeNode Databases { get; set; }

        private TreeNode Security { get; set; }

        private TreeNode LinkedServerLogins { get; set; }

        private TreeNode Logins { get; set; }

        private TreeNode ServerRoles { get; set; }

        private TreeNode Credentials { get; set; }

        private TreeNode CryptographicProviders { get; set; }

        private TreeNode Audits { get; set; }

        private TreeNode ServerAuditSpecifications { get; set; }

        private TreeNode EventSessions { get; set; }

        private TreeNode ServerObjects { get; set; }

        private TreeNode EndPoints { get; set; }

        private TreeNode LinkedServers { get; set; }

        private TreeNode Triggers { get; set; }

        private TreeNode ErrorMessages { get; set; }

        private TreeNode ServerEventNotifications { get; set; }

        /// <summary>The create root nodes.</summary>
        /// <returns></returns>
        private TreeNode CreateRootNodes()
        {
            TreeNode root = new TreeNode("ve_resource.Database");
            root.ImageKey = "Database";
            root.SelectedImageKey = "Database";
            root.ContextMenuStrip = InspectorContextMenuStrip;
            root.Tag = RootTag;

            _tablesNode = new TreeNode("ve_resource.Tables");
            _tablesNode.ImageKey = "Tables";
            _tablesNode.SelectedImageKey = "Tables";
            _tablesNode.Tag = TablesTag;

            _viewsNode = new TreeNode("ve_resource.Views");
            _viewsNode.ImageKey = "Views";
            _viewsNode.SelectedImageKey = "Views";
            _viewsNode.Tag = ViewsTag;

            //root.Nodes.Add(_tablesNode);
            //root.Nodes.Add(_viewsNode);

            TreeNode ng = new TreeNode("Databases");

            root.Nodes.Add(ng);

            Databases = ng;

            TreeNode node = new TreeNode("System Databases");

            ng.Nodes.Add(node);

            SysDatabases = node;

            ng = new TreeNode("Security");

            root.Nodes.Add(ng);

            Security = ng;

            ng = new TreeNode("Linked Server Logins");

            Security.Nodes.Add(ng);

            LinkedServerLogins = ng;

            ng = new TreeNode("Logins");

            Security.Nodes.Add(ng);

            Logins = ng;

            ng = new TreeNode("Server Roles");

            Security.Nodes.Add(ng);

            TreeNode ns = new TreeNode("sysadmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("bulkadmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("dbcreator");
            ng.Nodes.Add(ns);
            ns = new TreeNode("diskadmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("processadmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("publlic");
            ng.Nodes.Add(ns);
            ns = new TreeNode("securityadmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("serveradmin");
            ng.Nodes.Add(ns);
            ns = new TreeNode("setupadmin");
            ng.Nodes.Add(ns);

            ServerRoles = ng;

            ng = new TreeNode("Credentials");

            Security.Nodes.Add(ng);

            Credentials = ng;

            ng = new TreeNode("Cryptographic Providers");

            Security.Nodes.Add(ng);

            CryptographicProviders = ng;

            ng = new TreeNode("Audits");

            Security.Nodes.Add(ng);

            Audits = ng;

            ng = new TreeNode("Server Audit Specifications");

            Security.Nodes.Add(ng);

            ServerAuditSpecifications = ng;

            ng = new TreeNode("Event Sessions");

            Security.Nodes.Add(ng);

            EventSessions = ng;

            ng = new TreeNode("Server Objects");

            root.Nodes.Add(ng);

            ServerObjects = ng;

            ng = new TreeNode("Replication");
            root.Nodes.Add(ng);
            ng = new TreeNode("PolyBase");
            root.Nodes.Add(ng);
            ng = new TreeNode("Always On High Availability");
            root.Nodes.Add(ng);
            ng = new TreeNode("Management");
            root.Nodes.Add(ng);

            TreeNode bp = new TreeNode("Policy Management");
            ng.Nodes.Add(bp);

            TreeNode bb = new TreeNode("Policies");
            bp.Nodes.Add(bb);
            bb = new TreeNode("Conditions");
            bp.Nodes.Add(bb);
            bb = new TreeNode("Facets");
            bp.Nodes.Add(bb);

            bp = new TreeNode("Data Collection");
            ng.Nodes.Add(bp);
            bp = new TreeNode("Resource Governor");
            ng.Nodes.Add(bp);
            bp = new TreeNode("Extended Events");
            ng.Nodes.Add(bp);
            bp = new TreeNode("Maintenance Plan");
            ng.Nodes.Add(bp);
            bp = new TreeNode("SQL Server Logs");
            ng.Nodes.Add(bp);
            bp = new TreeNode("Server Mail");
            ng.Nodes.Add(bp);
            bp = new TreeNode("Legacy");
            ng.Nodes.Add(bp);

            ng = new TreeNode("Endpoints");

            ServerObjects.Nodes.Add(ng);

            EndPoints = ng;

            ns = new TreeNode("System Endpoints");
            ng.Nodes.Add(ns);

            TreeNode rn = new TreeNode("Database Mirroring");
            ns.Nodes.Add(rn);
            rn = new TreeNode("Service Broker");
            ns.Nodes.Add(rn);
            rn = new TreeNode("SOAP");
            ns.Nodes.Add(rn);
            rn = new TreeNode("TSQL");
            ns.Nodes.Add(rn);

            ns = new TreeNode("Database Mirroring");
            ng.Nodes.Add(ns);
            ns = new TreeNode("Service Broker");
            ng.Nodes.Add(ns);
            ns = new TreeNode("SOAP");
            ng.Nodes.Add(ns);
            ns = new TreeNode("TSQL");
            ng.Nodes.Add(ns);

            ng = new TreeNode("Linked Servers");

            ServerObjects.Nodes.Add(ng);

            LinkedServers = ng;

            ng = new TreeNode("Triggers");

            ServerObjects.Nodes.Add(ng);

            Triggers = ng;

            ng = new TreeNode("Error Messages");

            ServerObjects.Nodes.Add(ng);

            ErrorMessages = ng;

            ng = new TreeNode("Server Event Notifications");

            ServerObjects.Nodes.Add(ng);

            ServerEventNotifications = ng;

            return root;
        }

        /// <summary>The create tree nodes.</summary>
        /// <param name="table">The table.</param>
        private void CreateTreeNodes(DbModelTable table)
        {
            TreeNode tableNode = new TreeNode(table.FullName);
            tableNode.Name = table.FullName;
            tableNode.ImageKey = table.ObjectType;
            tableNode.SelectedImageKey = table.ObjectType;
            tableNode.ContextMenuStrip = TableNodeContextMenuStrip;
            tableNode.Tag = table;

            foreach (DbModelColumn column in table.Columns)
            {
                string friendlyColumnName = column.Name;
                TreeNode columnNode = new TreeNode(friendlyColumnName);
                columnNode.Name = column.Name;
                string imageKey = BuildImageKey(column);
                columnNode.ImageKey = imageKey;
                columnNode.SelectedImageKey = imageKey;
                columnNode.ContextMenuStrip = ColumnNameContextMenuStrip;
                columnNode.Tag = column;
                columnNode.Text = column.Name;
                tableNode.Nodes.Add(columnNode);
            }

            switch (table.ObjectType)
            {
                case ObjectTypes.Table:
                    _tablesNode.Nodes.Add(tableNode);
                    break;

                case ObjectTypes.View:
                    _viewsNode.Nodes.Add(tableNode);
                    break;
            }
        }

        public string cs { get; set; }

        public string ExecStoredProcedure(string obs)
        {
            string name = "";

            using (SqlConnection conn = new SqlConnection(cs))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("sp_helptrigger", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@tabname", obs));

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        name = rdr["TRIGGER_NAME"].ToString();
                    }
                }
            }

            return name;
        }

        private void CreateTreeNodes(DbModelTable table, TreeNode nodes)
        {
            TreeNode ng = new TreeNode("Columns");

            TreeNode tableNode = new TreeNode(table.FullName);
            tableNode.Name = table.FullName;
            tableNode.ImageKey = table.ObjectType;
            tableNode.SelectedImageKey = table.ObjectType;
            tableNode.ContextMenuStrip = TableNodeContextMenuStrip;
            tableNode.Tag = table;

            tableNode.Nodes.Add(ng);

            foreach (DbModelColumn column in table.Columns)
            {
                string friendlyColumnName = column.Name;
                TreeNode columnNode = new TreeNode(friendlyColumnName);
                columnNode.Name = column.Name;
                string imageKey = BuildImageKey(column);
                columnNode.ImageKey = "column";
                columnNode.SelectedImageKey = "column";
                columnNode.ContextMenuStrip = ColumnNameContextMenuStrip;
                columnNode.Tag = column;
                columnNode.Text = column.Name + "(" + column.DbType.Summary + ")";
                ng.Nodes.Add(columnNode);
            }

            nodes.Nodes.Add(tableNode);

            ng = new TreeNode("Keys");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Constraints");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Triggers");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Indexes");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Statistics");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);
        }

        private void CreateTreeNodes_View(DbModelTable table, TreeNode nodes)
        {
            TreeNode ng = new TreeNode("Columns");

            TreeNode tableNode = new TreeNode(table.FullName);
            tableNode.Name = table.FullName;
            tableNode.ImageKey = table.ObjectType;
            tableNode.SelectedImageKey = table.ObjectType;
            tableNode.ContextMenuStrip = TableNodeContextMenuStrip;
            tableNode.Tag = table;

            tableNode.Nodes.Add(ng);

            foreach (DbModelColumn column in table.Columns)
            {
                string friendlyColumnName = column.Name;
                TreeNode columnNode = new TreeNode(friendlyColumnName);
                columnNode.Name = column.Name;
                string imageKey = BuildImageKey(column);
                columnNode.ImageKey = imageKey;
                columnNode.SelectedImageKey = imageKey;
                columnNode.ContextMenuStrip = ColumnNameContextMenuStrip;
                columnNode.Tag = column;
                columnNode.Text = column.Name;

                ng.Nodes.Add(columnNode);
            }

            nodes.Nodes.Add(tableNode);

            ng = new TreeNode("Triggers");

            ng.Tag = table.FullName;

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Indexes");

            tableNode.Nodes.Add(ng);

            ng = new TreeNode("Statistics");

            tableNode.Nodes.Add(ng);
        }

        private void CreateTreeNodes_StoredProcedures(TreeNode nodes)
        {
            List<string> list = GetDatabaseStoredProcedures(database);

            foreach (string s in list)
            {
                string[] cc = s.Split(" ".ToCharArray());

                int i = 0;
                while (cc[i] == "")
                    i++;

                TreeNode ng = new TreeNode(cc[i]);

                ng.Tag = "Stored Procedure - " + cc[i];

                nodes.Nodes.Add(ng);
            }
        }

        /// <summary>The database inspector form_ load.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DatabaseInspectorForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>The database tree view_ before expand.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DatabaseTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            if (node != null && node.Tag == RootTag && !_populated)
            {
                _populated = true;

                //            bool ok = ExecLoadDatabaseDetails();

                //if (ok && DatabaseTreeView.Nodes.Count > 0)
                //{
                //	DatabaseTreeView.Nodes[0].Expand();
                //}
                //else
                //{
                //	e.Cancel = true;
                //}
            }
        }

        /// <summary>The database tree view_ node mouse click.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void DatabaseTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                IDbModelNamedObject namedObject = node.Tag as IDbModelNamedObject;
                _rightClickedModelObject = namedObject;

                if (namedObject != null &&
                    (namedObject.ObjectType == ObjectTypes.Table || namedObject.ObjectType == ObjectTypes.View))
                {
                    _rightClickedNode = node;
                    tbContext.Show(v, e.Location);
                }
                else
                {
                    _rightClickedNode = null;
                }
            }
        }

        public List<string> GetTableKeys(string table_name)
        {
            List<string> list = new List<string>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.objects WHERE  type = 'PK'  AND parent_object_id = OBJECT_ID('" + table_name + "')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('" + table_name + "')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetTableIndexes(string table_name)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.indexes where object_id = OBJECT_ID('" + table_name + "')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //string b = "";
                            //int i = 0;
                            //while(i < dr.FieldCount)
                            //   b += " " + dr[i++] as string;
                            string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetTableConstraints(string table_name)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.objects WHERE type_desc LIKE '%CONSTRAINT%' AND parent_object_id = OBJECT_ID('" + table_name + "')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetTableStatistics(string table_name)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.stats WHERE object_id = OBJECT_ID('" + table_name + "')", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }
            return list;
        }

        // SELECT* FROM sys.all_objects where type in ('FN','AF','FS','FT','IF','TF')

        // --AF = Aggregate function(CLR)
        //--C = CHECK constraint
        //--D = DEFAULT(constraint or stand-alone)
        //--F = FOREIGN KEY constraint
        //--PK = PRIMARY KEY constraint
        //--P = SQL stored procedure
        //--PC = Assembly(CLR) stored procedure
        //--FN = SQL scalar-function
        //--FS = Assembly(CLR) scalar function
        //--FT = Assembly(CLR) table-valued function
        //--R = Rule(old-style, stand-alone)
        //--RF = Replication filter procedure
        //--SN = Synonym
        //--SQ = Service queue
        //--TA = Assembly(CLR) trigger
        //--TR = SQL trigger
        //--IF = SQL inlined table-valued function
        //--TF = SQL table-valued function
        //--U = Table(user-defined)
        //--UQ = UNIQUE constraint
        //--V = View
        //--X = Extended stored procedure
        //--IT = Internal table

        public List<string> GetSystemFunctions(string function)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.all_objects where type_desc LIKE '%FUNCTION%'", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        //SELECT* FROM sys.objects WHERE type='U'

        public List<string> GetSchema()
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT  s.Name, u.* FROM    sys.schemas s INNER JOIN sys.sysusers u ON u.uid = s.principal_id SELECT * FROM sys.objects WHERE type='U'", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        //SELECT name FROM master..sysxlogins WHERE sid IS NOT NULL

        public List<string> GetLogins()
        {
            cs = GetConnectionString("");

            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name AS Login_Name, type_desc AS Account_Type FROM sys.server_principals WHERE TYPE IN('U', 'S', 'G') and name not like '%##%' ORDER BY name, type_desc", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        //SELECT* FROM sys.database_principals

        public List<string> GetDatabaseUsers()
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT* FROM sys.database_principals", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetDatabaseRoles()
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("Select [name] From sysusers Where issqlrole = 1", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetStoredProcedure(string procedure_name)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("select t1.[name] as [SP_name],t2.[name] as [Parameter_name], t3.[name] as [Type], t2.[Length], t2.colorder as [Param_order] from sysobjects t1 inner join syscolumns t2 on t1.[id] = t2.[id] inner join systypes t3 on t2.xtype = t3.xtype where t1.[name] = '" + procedure_name + "' order by[Param_order]", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetDatabaseStoredProcedures(string database)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Set up a command with the given query and associate //type = 'PK' OR
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.sysobjects WHERE type = 'P'", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string b = "";
                            int i = 0;
                            while (i < dr.FieldCount)
                                b += " " + dr[i++] as string;
                            // string b = dr[1].ToString();
                            list.Add(b);
                        }
                    }
                }
            }

            return list;
        }

        public List<string> GetDatabaseList(string conString)
        {
            List<string> list = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
            }
            return list;
        }

        private List<string> views { get; set; }

        public List<string> GetDatabaseViewsList(string conString)
        {
            List<string> views = new List<string>();

            // Open connection to the database
            //string conString = "server=DESKTOP-COMP;uid=sa;pwd=sa; database=northwind";

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM sys.system_views", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            views.Add(dr[0].ToString());
                        }
                    }
                }
            }

            return views;
        }

        //SELECT[TABLE_NAME] FROM INFORMATION_SCHEMA.VIEWS

        /// <summary>The exec load database details.</summary>
        /// <returns>The exec load database details.</returns>
        public bool ExecLoadDatabaseDetails(DataConnection dc)
        {
            bool populate = false;
            string connection = string.Empty;
            bool success = false;

            try
            {
                if (_metaDataService == null)
                {
                    _metaDataService = DatabaseMetaDataService.Create("SQL Server");
                }

                connection = dc.GetConnectionStringr();
                populate = true;
            }
            catch (Exception exp)
            {
                string msg = string.Format(
                    "{0}\r\n\r\nCheck the connection and select 'Reset Database Connection'.",
                    exp.Message);
            }
            finally
            {
            }

            if (populate)
            {
                try
                {
                    _model = _metaDataService.GetDbObjectModel(dc.GetConnectionStringr());
                    _models = GetDatabaseList(dc.GetConnectionStringr());
                    // Data Source = DESKTOP - COMP; Initial Catalog = master; Integrated Security = False; User ID = sa; Password = sa
                }
                finally
                {
                }
                BuildTreeFromDbModel(connection);

                success = true;
            }
            else
            {
                _populated = false;
                DatabaseTreeView.CollapseAll();
            }

            return success;
        }

        public DbModelInstance LoadDatabase(string c)
        {
            string connection = string.Empty;
            dc.SetKeyValue("Initial Catalog", c);
            cs = dc.GetConnectionString();// GetConnectionString(c);

            DbModelInstance model = null;

            try
            {
                {
                    _metaDataService = DatabaseMetaDataService.Create("System.Data.SqlClient");
                }

                connection = _metaDataService.GetDescription();
            }
            catch (Exception exp)
            {
                string msg = string.Format(
                    "{0}\r\n\r\nCheck the connection and select 'Reset Database Connection'.",
                    exp.Message);
            }
            finally
            {
            }

            {
                try
                {
                    model = _metaDataService.GetDbObjectModel(cs);
                    //_models = GetDatabaseList(_services.Settings.ConnectionDefinition.ConnectionString);
                    // Data Source = DESKTOP - COMP; Initial Catalog = master; Integrated Security = False; User ID = sa; Password = sa
                    model.vw = GetDatabaseViewsList(cs);
                }
                finally
                {
                }
            }

            return model;
        }

        private string ds = "Data Source = DESKTOP-COMP; Integrated Security = False; User ID = sa; Password = sa";

        private string dd = "; Initial Catalog = ";

        private string database = "";

        public string GetConnectionString(string s)
        {
            database = s;

            return ds + dd + s;
        }

        /// <summary>The select node.</summary>
        /// <param name="treeNode">The tree node.</param>
        private void SelectNode(TreeNode treeNode)
        {
            if (treeNode.Parent != null)
            {
                treeNode.Parent.EnsureVisible();
            }

            treeNode.EnsureVisible();
            DatabaseTreeView.SelectedNode = treeNode;
            treeNode.Expand();
            DatabaseTreeView.Focus();
        }

        public TreeNode LoadDatabaseNodes(TreeNode nodes)
        {
            TreeNode ng = new TreeNode("Database Diagrams");

            nodes.Nodes.Add(ng);

            ng = new TreeNode("Tables");

            nodes.Nodes.Add(ng);

            ng = new TreeNode("Views");

            nodes.Nodes.Add(ng);

            TreeNode ngs = new TreeNode("System Views");

            ng.Nodes.Add(ngs);

            ng = new TreeNode("External Resources");

            nodes.Nodes.Add(ng);

            TreeNode wn = new TreeNode("External Data Sources");

            ng.Nodes.Add(wn);

            wn = new TreeNode("External File Formats");

            ng.Nodes.Add(wn);

            ng = new TreeNode("Synonyms");

            nodes.Nodes.Add(ng);

            ng = new TreeNode("Programmability");

            nodes.Nodes.Add(ng);

            ngs = new TreeNode("Stored Procedures");

            ng.Nodes.Add(ngs);

            TreeNode ns = new TreeNode("System Stored Procedures");

            ngs.Nodes.Add(ns);

            ns = new TreeNode("Functions");

            ng.Nodes.Add(ns);

            TreeNode nv = new TreeNode("Table-valued Functions");

            nv.Tag = "System Functions - TF";

            ns.Nodes.Add(nv);

            nv = new TreeNode("Scalar-valued Functions");

            ns.Nodes.Add(nv);

            nv = new TreeNode("Aggregate Functions");

            ns.Nodes.Add(nv);

            nv = new TreeNode("System Functions");

            ns.Nodes.Add(nv);

            TreeNode bv = new TreeNode("Aggregate Functions");

            bv.Tag = "System Functions - AF";

            nv.Nodes.Add(bv);

            TreeNode rn = new TreeNode("Avg()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Binary_checksum()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Checksum()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Checksum_agg()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Count()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Count_big()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Grouping()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Grouping_Id()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Min()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Max()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Stdev()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Stdevp()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Sum()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Var()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Varp()");
            bv.Nodes.Add(rn);

            bv = new TreeNode("Configuration Functions");

            nv.Nodes.Add(bv);

            rn = new TreeNode("ConnectionProperty()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@Datefirst()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@Dbts()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@LangId()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@Language()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@LockTimeout()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@Max_Connections()");
            bv.Nodes.Add(rn);

            bv = new TreeNode("Cursor Functions");

            nv.Nodes.Add(bv);

            rn = new TreeNode("@@Cursor_Rows()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("Cursor_Status()");
            bv.Nodes.Add(rn);
            rn = new TreeNode("@@Fetch_Status()");
            bv.Nodes.Add(rn);

            bv = new TreeNode("Date and Time Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Mathematical Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Metadata Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Other Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Hierarchy Id Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Rowset Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Security Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("String Functions");

            nv.Nodes.Add(bv);

            bv = new TreeNode("Text and Image Functions");

            nv.Nodes.Add(bv);

            ns = new TreeNode("Database Triggers");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Assemblies");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Types");

            ng.Nodes.Add(ns);

            bv = new TreeNode("System Data Types");

            ns.Nodes.Add(bv);

            bv = new TreeNode("User-Defined Data Types");

            ns.Nodes.Add(bv);

            bv = new TreeNode("User-Defined Table Types");

            ns.Nodes.Add(bv);

            bv = new TreeNode("User-Defined Types");

            ns.Nodes.Add(bv);

            bv = new TreeNode("XML Schema Collections");

            ns.Nodes.Add(bv);

            ns = new TreeNode("Rules");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Defaults");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Database Triggers");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Plan Guides");

            ng.Nodes.Add(ns);

            ns = new TreeNode("Sequences");

            ng.Nodes.Add(ns);

            ng = new TreeNode("Service Broker");

            nodes.Nodes.Add(ng);

            rn = new TreeNode("Message Types");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Contracts");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Queues");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Routes");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Remote Service Binding");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Broker Priorities");
            ng.Nodes.Add(rn);

            ng = new TreeNode("Storage");

            nodes.Nodes.Add(ng);

            rn = new TreeNode("Full Text Catalogs");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Partition Schemes");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Partition Functions");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Full Text Stoplists");
            ng.Nodes.Add(rn);
            rn = new TreeNode("Search Property Lists");
            ng.Nodes.Add(rn);

            ng = new TreeNode("Security");

            nodes.Nodes.Add(ng);

            rn = new TreeNode("Users");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Roles");
            ng.Nodes.Add(rn);

            bv = new TreeNode("Database Roles");
            rn.Nodes.Add(bv);

            bv = new TreeNode("Application Roles");
            rn.Nodes.Add(bv);

            rn = new TreeNode("Schemas");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Asymmetric Keys");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Certificates");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Symmetric Keys");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Always Encrypted Keys");
            ng.Nodes.Add(rn);

            bv = new TreeNode("Column Master Keys");
            rn.Nodes.Add(bv);

            bv = new TreeNode("Column Encryption Keys");
            rn.Nodes.Add(bv);

            rn = new TreeNode("Database Audit Specifications");
            ng.Nodes.Add(rn);

            rn = new TreeNode("Security Policies");
            ng.Nodes.Add(rn);

            return ng;
        }

        private void DatabaseTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.Tag == "database")
            {
                node.Nodes.Clear();

                DbModelInstance model = LoadDatabase(node.Text);

                LoadDatabaseNodes(node);

                BuildTreeFromDbModel(model, node);
            }
            else if (node.Text == "Triggers")
            {
                string name = ExecStoredProcedure(node.Tag as string);

                if (name == "")
                    return;

                TreeNode ng = new TreeNode(name);

                node.Nodes.Add(ng);
            }
            else if (node.Text == "Keys")
            {
                List<string> names = GetTableKeys(node.Tag as string);

                if (names.Count <= 0)
                    return;

                TreeNode ns = node.Parent;

                DbModelTable table = ns.Tag as DbModelTable;

                TreeNode ng = null;

                //foreach (string name in names)
                {
                    foreach (DbModelColumn c in table.Columns)
                    {
                        if (c.IsKey || c.HasFK)
                        {
                            ng = new TreeNode();
                            ng.Text = c.FullName;
                            ng.Tag = c;

                            if (c.ForeignKeyReference != null)
                            {
                                ng.ImageKey = "FK";
                                ng.SelectedImageKey = "FK";
                            }
                            else
                            {
                                ng.ImageKey = "PK";
                                ng.SelectedImageKey = "PK";
                            }
                            node.Nodes.Add(ng);
                        }
                    }
                }
            }
            else if (node.Text == "Indexes")
            {
                List<string> names = GetTableIndexes(node.Tag as string);

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Constraints")
            {
                List<string> names = GetTableConstraints(node.Tag as string);

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Statistics")
            {
                List<string> names = GetTableStatistics(node.Tag as string);

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Schemas")
            {
                List<string> names = GetSchema();

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Database Roles")
            {
                List<string> names = GetDatabaseRoles();

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Users")
            {
                List<string> names = GetDatabaseUsers();

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else if (node.Text == "Logins")
            {
                List<string> names = GetLogins();

                if (names.Count <= 0)
                    return;

                TreeNode ng = null;

                foreach (string name in names)
                {
                    ng = new TreeNode(name);

                    node.Nodes.Add(ng);
                }
            }
            else
            {
                string data = node.Tag as string;

                if (data != null)
                {
                    if ((data).StartsWith("Stored Procedure") == true)
                    {
                        string procedure = data.Replace("Stored Procedure - ", "");

                        List<string> names = GetStoredProcedure(procedure);

                        if (names.Count <= 0)
                            return;

                        TreeNode nodes = new TreeNode("Parameters");

                        node.Nodes.Add(nodes);

                        TreeNode ng = null;

                        foreach (string name in names)
                        {
                            ng = new TreeNode(name);

                            nodes.Nodes.Add(ng);
                        }
                    }
                    else if ((data).StartsWith("System Functions") == true)
                    {
                        string function = data.Replace("System Functions - ", "");

                        List<string> names = GetSystemFunctions(function);

                        if (names.Count <= 0)
                            return;

                        //TreeNode nodes = new TreeNode("Parameters");

                        //node.Nodes.Add(nodes);

                        TreeNode ng = null;

                        foreach (string name in names)
                        {
                            ng = new TreeNode(name);

                            node.Nodes.Add(ng);
                        }
                    }
                }
            }
            ExplorerForms.ef.Command_SetPropertyGrid(node.Tag);
        }

        private DataConnection dc { get; set; }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
        }

        public static bool EnumerateSQLInstances()
        {
            return true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SQLServerSelectForm form = new SQLServerSelectForm();
            DialogResult r = form.ShowDialog();
            if (r != DialogResult.OK)
                return;
            dc = form.dc;

            LoadDatabaseDetails(dc);
        }

        private void viewDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm df = ExplorerForms.ef.Command_OpenDocumentForm("tables");
            DataViewerForm dvf = new DataViewerForm();
            dvf.FormBorderStyle = FormBorderStyle.None;
            dvf.TopLevel = false;
            dvf.Dock = DockStyle.Fill;

            dvf.Load(_rightClickedNode.Tag as DbModelObjectBase);
            dvf.Show();
            df.Controls.Add(dvf);
        }

        private void viewDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DocumentForm df = ExplorerForms.ef.Command_OpenDocumentForm("tables");
            ViewDesignerForm dvf = new ViewDesignerForm();
            dvf.FormBorderStyle = FormBorderStyle.None;
            dvf.TopLevel = false;
            dvf.Dock = DockStyle.Fill;
            dvf.Show();
            df.Controls.Add(dvf);
        }

        private void viewCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DbModelTable b = _rightClickedNode.Tag as DbModelTable;
            ScriptingOptions sc = new ScriptingOptions();
            sc.ScriptData = true;
            sc.ScriptSchema = false;
            string s = SQLScripter.CreateTableScript("localhost", "sa", "sa", "database", b.Name, sc);

            DocumentForm df = ExplorerForms.ef.Command_OpenDocumentForm("tables");
            ViewCodeForm dvf = new ViewCodeForm(s);
            dvf.FormBorderStyle = FormBorderStyle.None;
            dvf.TopLevel = false;
            dvf.Dock = DockStyle.Fill;
            dvf.Show();
            df.Controls.Add(dvf);
        }

        private void newQueryWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DbModelTable b = _rightClickedNode.Tag as DbModelTable;
            ScriptingOptions sc = new ScriptingOptions();
            sc.ScriptData = false;
            sc.ScriptSchema = true;
            string s = SQLScripter.CreateTableScript("localhost", "sa", "sasa", "database", b.Name, sc);

            DocumentForm df = ExplorerForms.ef.Command_OpenDocumentForm("tables");
            ViewCodeForm dvf = new ViewCodeForm(s);
            dvf.FormBorderStyle = FormBorderStyle.None;
            dvf.TopLevel = false;
            dvf.Dock = DockStyle.Fill;
            dvf.Show();
            df.Controls.Add(dvf);
        }

        /// <summary>
        /// Method returns the correct SQL namespace to use to detect SQL Server instances.
        /// </summary>
        /// <returns>namespace to use to detect SQL Server instances</returns>
    }

    public class SQLScripter
    {
        public static string CreateTableScript(string datasource, string user, string password, string databases, string tableName, ScriptingOptions sc)
        {
            var server = new Server(new ServerConnection { ConnectionString = new SqlConnectionStringBuilder { DataSource = @datasource, UserID = user, Password = password }.ToString() });
            server.ConnectionContext.Connect();
            var database = server.Databases[databases];

            {
                //foreach(Table table in database.Tables)
                {
                    Table table = database.Tables[tableName];
                    //if (table.Name == tableName)
                    {
                        //                        ScriptingOptions sc = new ScriptingOptions();
                        //                        sc.ScriptSchema = false;
                        //                        sc.ScriptData = true;

                        // s = "";
                        var scripter = new Scripter(server);
                        scripter.Options = sc;
                        var script = scripter.EnumScript(new SqlSmoObject[] { table });
                        // return table.Script().ToString();////var script = scripter.Script(new SqlSmoObject[] { table });
                        //return script.ToString();
                        StringBuilder b = new StringBuilder();
                        foreach (string line in script)
                        {
                            b.AppendLine(line);
                        }
                        return b.ToString();
                    }
                }
            }
            return "";
        }
    }

    public class SQLServerInstances
    {
        //private static List<string> GetLocalSqlServerInstancesByCallingSqlBrowser()
        //{
        //    DataTable dt = SmoApplication.EnumAvailableSqlServers(true);
        //    return dt.Rows.Cast<DataRow>()
        //        .Select(v => v.Field<string>("Name"))
        //        .ToList();
        //}

        private static List<string> GetLocalSqlServerInstancesByCallingSqlWmi32()
        {
            return LocalSqlServerInstancesByCallingSqlWmi(ProviderArchitecture.Use32bit);
        }

        private static List<string> GetLocalSqlServerInstancesByCallingSqlWmi64()
        {
            return LocalSqlServerInstancesByCallingSqlWmi(ProviderArchitecture.Use64bit);
        }

        private static List<string> LocalSqlServerInstancesByCallingSqlWmi(ProviderArchitecture providerArchitecture)
        {
            try
            {
                ManagedComputer managedComputer32 = new ManagedComputer();
                managedComputer32.ConnectionSettings.ProviderArchitecture = providerArchitecture;

                const string defaultSqlInstanceName = "MSSQLSERVER";
                return managedComputer32.ServerInstances.Cast<ServerInstance>()
                    .Select(v =>
                        (string.IsNullOrEmpty(v.Name) || string.Equals(v.Name, defaultSqlInstanceName, StringComparison.OrdinalIgnoreCase)) ?
                            v.Parent.Name : string.Format("{0}\\{1}", v.Parent.Name, v.Name))
                    .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch (SmoException ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<string>();
            }
        }

        public static List<ServerInstance> LocalSqlServerInstancesgSqlWmi(ProviderArchitecture providerArchitecture)
        {
            try
            {
                ManagedComputer managedComputer32 = new ManagedComputer();
                managedComputer32.ConnectionSettings.ProviderArchitecture = providerArchitecture;

                const string defaultSqlInstanceName = "MSSQLSERVER";
                List<ServerInstance> list = new List<ServerInstance>();
                foreach (ServerInstance s in managedComputer32.ServerInstances)
                    list.Add(s);
                return list;
            }
            catch (SmoException ex)
            {
                Console.WriteLine(ex.Message);
                return new List<ServerInstance>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<ServerInstance>();
            }
        }

        private static List<string> GetLocalSqlServerInstancesByReadingRegInstalledInstances()
        {
            try
            {
                // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\InstalledInstances
                string[] instances = null;
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server"))
                {
                    if (rk != null)
                    {
                        instances = (string[])rk.GetValue("InstalledInstances");
                    }

                    instances = instances ?? new string[] { };
                }

                return GetLocalSqlServerInstances(instances);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<string>();
            }
        }

        private static List<string> GetLocalSqlServerInstancesByReadingRegInstanceNames()
        {
            try
            {
                // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL
                string[] instances = null;
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL"))
                {
                    if (rk != null)
                    {
                        instances = rk.GetValueNames();
                    }

                    instances = instances ?? new string[] { };
                }

                return GetLocalSqlServerInstances(instances);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<string>();
            }
        }

        private static List<string> GetLocalSqlServerInstances(string[] instanceNames)
        {
            string machineName = System.Environment.MachineName;

            const string defaultSqlInstanceName = "MSSQLSERVER";
            return instanceNames.Select(v =>
                    (string.IsNullOrEmpty(v) || string.Equals(v, defaultSqlInstanceName, StringComparison.OrdinalIgnoreCase)) ?
                        machineName : string.Format("{0}\\{1}", machineName, v))
                .ToList();
        }

        public static DataTable LocalNetworkSQLInstances()
        {
            // Retrieve the enumerator instance and then the data.
            SqlDataSourceEnumerator instance =
              SqlDataSourceEnumerator.Instance;

            System.Data.DataTable table = instance.GetDataSources();

            // Display the contents of the table.
            return table;
        }
    }
}