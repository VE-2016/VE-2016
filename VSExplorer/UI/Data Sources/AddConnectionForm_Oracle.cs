using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Oracle.ManagedDataAccess.Client;

namespace WinExplorer.UI
{
    public partial class AddConnectionForm_Oracle : Form
    {
        public AddConnectionForm_Oracle()
        {
            InitializeComponent();

            cb = comboBox3;

            ca = comboBox2;

            Init();
        }

        public void Init()
        {
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Items.Clear();
            foreach(string name in Enum.GetNames(typeof(Oracle)))
            {
                cb.Items.Add(name);
            }
        }

        ComboBox cb { get; set; }

        ComboBox ca { get; set; }

        private void button5_Click(object sender, EventArgs e)
        { 

            Connect();


         

            //DataTable databases = conn.GetSchema("Databases");
            //foreach (DataRow database in databases.Rows)
            //{
            //    String databaseName = database["database_name"] as string;
            //    short dbID = (short)database["dbid"];
                
            //    cb.Items.Add(databaseName);
            //}

            //conn.Close();
        }
        string GetUser()
        {
            return textBox2.Text;
        }
        string GetPassword()
        {
            return textBox3.Text;
        }

        bool GetAuthentification()
        {
            return checkBox1.Checked;
        }
        bool GetDBAprivilage()
        {
            return checkBox3.Checked;
        }
        string GetServerName()
        {
            return comboBox2.Text;
        }
        string GetDatabase()
        {
            return cb.Text;
        }

        string GetConnectionString()
        {
            //con.ConnectionString = "User Id=sys;Password=sys;DBA Privilege=sysdba;Data Source=orcl";
            string c = "";
            c += "Data Source=" + GetServerName() + ";";
            bool d = GetAuthentification();
            if(d == true)
            {
                c+= "Integrated Security=SSPI;";
            }
            else
            {
                c += "User Id=" + GetUser() +";";
                c += "Password=" + GetPassword() + ";";
            }
            d = GetDBAprivilage();
            if (d == true)
                c += "DBA Privilege = sysdba;";

            shortcut = c;

            return c;
        }

        public string shortcut { get; set; }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
            
        }
        public static List<DataRow> GetTables(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                List<DataRow> TableNames = new List<DataRow>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row);
                }
                return TableNames;
            }
        }
        public static List<DataRow> GetViews(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Views");
                List<DataRow> TableNames = new List<DataRow>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row);
                }
                return TableNames;
            }
        }
        public static List<string> GetSchemas(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Users");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[0].ToString());
                }
                return TableNames;
            }
        }
        public static List<string> GetPackages(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Packages");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[1].ToString());
                }
                return TableNames;
            }
        }
        public static List<string> GetProcedures(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Procedures");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[1].ToString());
                }
                return TableNames;
            }
        }
        public static List<string> GetFunctions(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Functions");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[1].ToString());
                }
                return TableNames;
            }
        }
        public static List<string> GetTriggers(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Triggers");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[1].ToString());
                }
                return TableNames;
            }
        }
        public static List<string> GetIndexes(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Indexes");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[1].ToString());
                }
                return TableNames;
            }
        }
        public static DataTable GetTableSchema(string connectionString, string table)
        {
         
            OracleConnection connection = new OracleConnection(connectionString);
            {

                connection.Open();
                using (var schemaCommand = new OracleCommand("SELECT * FROM " + table, connection))
                {

                    var reader = schemaCommand.ExecuteReader();
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);

                        connection.Close();

                        return dataTable;
                    }
                }

            }
        }
    
        public static List<string> GetRoles(string connectionString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                DataTable schema = GetTableSchema(connectionString,"DBA_ROLES");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[0].ToString());
                }
                return TableNames;
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            shortcut = GetConnectionString();

            

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseDataSourceForm cdf = new ChooseDataSourceForm();
            cdf.ShowDialog();
        }
        OracleConnection con;

        void Connect()
        {
            string c = GetConnectionString();
            con = new OracleConnection();
            //con.ConnectionString = "User Id=sys;Password=sys;DBA Privilege=sysdba;Data Source=orcl";
            con.ConnectionString = GetConnectionString();
            con.Open();
            //Console.WriteLine("Connected to Oracle" + con.ServerVersion);

            //string []cc = ChooseDataSourceForm.GetOdbcDriverNames();
            //DataTable d = ChooseDataSourceForm.GetProviderFactoryClasses();
            con.Close();

            GetTables(c);

            GetViews(c);

            List<string> cs = GetSchemas(c);

            List<string> d = GetIndexes(c);

            List<string> r = GetRoles(c);
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }
    }

    public enum Oracle
    {
        Tables,
        Views,
        Relational_Tables,
        Object_Tables,
        XML_Tables,
        Relational_Views,
        XML_Views,
        Object_Views,
        Procedures,
        Functions,
        Packages,
        Synonims,
        Sequences,
        User_Defined_Types,
        Java_Classes,
        NET_Assemblies,
        XML_Schemas,
        Roles,
        Users,
        Queues,
        QueueTables,
        ADDM_Tasks,
        AWR_Snapshots,
        Pluggable_Databases,
        Materialized_Views,
        Triggers,
        Table_Triggers,
        View_Triggers,
        DatabaseTriggers,
        SchemaTriggers
    }
    
    }
