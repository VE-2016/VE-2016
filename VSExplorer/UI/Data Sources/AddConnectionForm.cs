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

namespace WinExplorer.UI
{
    public partial class AddConnectionForm : Form
    {
        public AddConnectionForm()
        {
            InitializeComponent();

            cb = comboBox3;

            ca = comboBox2;
        }

        ComboBox cb { get; set; }

        ComboBox ca { get; set; }

        private void button5_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection();
            // conn.ConnectionString =
            // "Data Source=localhost;" +
            //// "Initial Catalog=DataBaseName;" +
            ////"Integrated Security=SSPI; + 
            // "User id=sa;" +
            // "Password=sa;";
            conn.ConnectionString = GetConnectionString();
            try
            {
                conn.Open();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Connection failed " + ex.Message);
                return;
            }

            DataTable databases = conn.GetSchema("Databases");
            foreach (DataRow database in databases.Rows)
            {
                String databaseName = database["database_name"] as string;
                short dbID = (short)database["dbid"];
                
                cb.Items.Add(databaseName);
            }

            conn.Close();
        }
        string GetUser()
        {
            return textBox2.Text;
        }
        string GetPassword()
        {
            return textBox3.Text;
        }

        string GetAuthentification()
        {
            int index = ca.SelectedIndex;
            if (index == 0)
                return "SSPI";
            else return "";
        }
        string GetServerName()
        {
            return comboBox1.Text;
        }
        string GetDatabase()
        {
            return cb.Text;
        }

        string GetConnectionString()
        {
            string c = "";
            c += "Data Source=" + GetServerName() + ";";
            string d = GetAuthentification();
            if(d != "")
            {
                c+= "Integrated Security=SSPI;";
            }
            else
            {
                c += "User Id=" + GetUser() +";";
                c += "Password=" + GetPassword() + ";";
            }
            d = GetDatabase();
            if (d != "")
                c += "Initial Catalog=" + d + ";";

            shortcut = c;

            return c;
        }

        public string shortcut { get; set; }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
            
        }
        public static List<string> GetTables(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                List<string> TableNames = new List<string>();
                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[2].ToString());
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
    }
}
